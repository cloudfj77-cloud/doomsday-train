using System;
using System.Collections.Generic;
using System.Linq;

public class CraftingRecipe
{
	public List<CraftingFilter> requiredTools;

	public List<CraftingFilter> requiredMaterials;

	public Func<List<GameItem>, List<GameItem>, List<GameItem>> craftFunc;

	public RichTextBuilder description;

	public string customName { get; set; }

	public int remainingCraftAmount { get; set; }

	public CraftingRecipe(string name = null)
	{
		customName = name;
		craftFunc = (List<GameItem> _, List<GameItem> _) => new List<GameItem>();
		requiredTools = new List<CraftingFilter>();
		requiredMaterials = new List<CraftingFilter>();
		description = new RichTextBuilder();
		remainingCraftAmount = -1;
	}

	public CraftingRecipe SetRemainingCraftAmount(int amount)
	{
		remainingCraftAmount = amount;
		return this;
	}

	public CraftingRecipe AddToolIdentifier(string identifier, int amount = -1)
	{
		return AddToolIdentifier(DirectoryMaster.Item(identifier)?.name ?? "Unknown", amount, identifier);
	}

	public CraftingRecipe AddMaterialIdentifier(string identifier, int amount = -1)
	{
		return AddMaterialIdentifier(DirectoryMaster.Item(identifier)?.name ?? "Unknown", amount, identifier);
	}

	public CraftingRecipe AddToolIdentifier(string displayName, string identifier, params string[] additionalIdentifiers)
	{
		return AddTool(new CraftingIdentifierFilter(displayName, identifier, additionalIdentifiers));
	}

	public CraftingRecipe AddMaterialIdentifier(string displayName, string identifier, params string[] additionalIdentifiers)
	{
		return AddMaterial(new CraftingIdentifierFilter(displayName, identifier, additionalIdentifiers));
	}

	public CraftingRecipe AddToolIdentifier(string displayName, int powerAmount, string identifier, params string[] additionalIdentifiers)
	{
		return AddTool(new CraftingIdentifierFilter(displayName, powerAmount, identifier, additionalIdentifiers));
	}

	public CraftingRecipe AddMaterialIdentifier(string displayName, int amount, string identifier, params string[] additionalIdentifiers)
	{
		return AddMaterial(new CraftingIdentifierFilter(displayName, amount, identifier, additionalIdentifiers));
	}

	public CraftingRecipe AddToolTag(string displayName, string tag, params string[] additionalTags)
	{
		return AddTool(new CraftingTagsFilter(displayName, tag, additionalTags));
	}

	public CraftingRecipe AddMaterialTag(string displayName, string tag, params string[] additionalTags)
	{
		return AddMaterial(new CraftingTagsFilter(displayName, tag, additionalTags));
	}

	public CraftingRecipe AddToolTag(string displayName, int powerAmount, string tag, params string[] additionalTags)
	{
		return AddTool(new CraftingTagsFilter(displayName, powerAmount, tag, additionalTags));
	}

	public CraftingRecipe AddMaterialTag(string displayName, int amount, string tag, params string[] additionalTags)
	{
		return AddMaterial(new CraftingTagsFilter(displayName, amount, tag, additionalTags));
	}

	public CraftingRecipe AddTool(string displayName, Func<GameItem, bool> func)
	{
		return AddTool(new CraftingCustomFilter(displayName, func));
	}

	public CraftingRecipe AddMaterial(string displayName, Func<GameItem, bool> func)
	{
		return AddMaterial(new CraftingCustomFilter(displayName, func));
	}

	public CraftingRecipe AddTool(string displayName, int powerAmount, Func<GameItem, bool> func)
	{
		return AddTool(new CraftingCustomFilter(displayName, powerAmount, func));
	}

	public CraftingRecipe AddMaterial(string displayName, int amount, Func<GameItem, bool> func)
	{
		return AddMaterial(new CraftingCustomFilter(displayName, amount, func));
	}

	public CraftingRecipe AddTool(CraftingFilter filter)
	{
		requiredTools.Add(filter);
		return this;
	}

	public CraftingRecipe AddMaterial(CraftingFilter filter)
	{
		requiredMaterials.Add(filter);
		return this;
	}

	public CraftingRecipe SetDescription(string description)
	{
		this.description = new RichTextBuilder();
		this.description.Add(description);
		return this;
	}

	public CraftingRecipe SetDescription(RichTextBuilder description)
	{
		this.description = description;
		return this;
	}

	public CraftingRecipe SetCraftingFunc(Func<List<GameItem>, List<GameItem>, List<GameItem>> func)
	{
		craftFunc = func;
		return this;
	}

	public CraftingRecipe SetCraftingConstantFunc(string itemIdentifier, int amount = 1)
	{
		if (!DirectoryMaster.Has<GameItem>(itemIdentifier))
		{
			Debug.LogWarning("Crafting output identifier '" + itemIdentifier + "' is not in item directory.");
		}
		craftFunc = (List<GameItem> _, List<GameItem> _) => new List<GameItem> { DirectoryMaster.Item(itemIdentifier).SetAmount(amount) };
		return this;
	}

	private List<(CraftingFilter, List<GameItem>)> SortFilters(List<GameItem> allItems, List<CraftingFilter> requiredFilter, bool isAmount)
	{
		List<(CraftingFilter, List<GameItem>)> list = new List<(CraftingFilter, List<GameItem>)>();
		foreach (CraftingFilter item in requiredFilter)
		{
			List<GameItem> list2 = new List<GameItem>();
			int num = 0;
			foreach (GameItem allItem in allItems)
			{
				if (!item.Filter(allItem))
				{
					continue;
				}
				if (isAmount)
				{
					int num2 = ((item.amount < 0) ? 1 : item.amount);
					num += allItem.unitCount;
					list2.Add(allItem);
					if (num >= num2)
					{
						break;
					}
				}
				else
				{
					list2.Add(allItem);
				}
			}
			list.Add((item, list2));
		}
		return list;
	}

	public List<(CraftingFilter, List<GameItem>)> FilterTools(List<GameItem> allItems)
	{
		return SortFilters(allItems, requiredTools, isAmount: false);
	}

	public List<(CraftingFilter, List<GameItem>)> FilterMaterials(List<GameItem> allItems)
	{
		return SortFilters(allItems, requiredMaterials, isAmount: true);
	}

	public bool CanCraft(List<GameItem> allItems)
	{
		if (remainingCraftAmount == 0)
		{
			return false;
		}
		List<(CraftingFilter, List<GameItem>)> toolItems = FilterTools(allItems);
		List<(CraftingFilter, List<GameItem>)> materialItems = FilterMaterials(allItems);
		if (!VerifyTools(toolItems))
		{
			return false;
		}
		if (!VerifyMaterials(materialItems))
		{
			return false;
		}
		return true;
	}

	private static bool VerifyTools(List<(CraftingFilter, List<GameItem>)> toolItems)
	{
		foreach (var (craftingFilter, source) in toolItems)
		{
			if (craftingFilter.amount < 0)
			{
				if (!source.Any())
				{
					return false;
				}
			}
			else if (source.Select((GameItem item) => PowerHelper.GetAvailableEnergyFromItem(item, useBandwidth: false)).DefaultIfEmpty(0).Max() < craftingFilter.amount)
			{
				return false;
			}
		}
		return true;
	}

	private static bool VerifyMaterials(List<(CraftingFilter, List<GameItem>)> materialItems)
	{
		foreach (var materialItem in materialItems)
		{
			CraftingFilter item = materialItem.Item1;
			List<GameItem> item2 = materialItem.Item2;
			int num = ((item.amount < 0) ? 1 : item.amount);
			if (item2.Select((GameItem gi) => gi.unitCount).Sum() < num)
			{
				return false;
			}
		}
		return true;
	}

	public List<GameItem> CraftWithoutMaterials()
	{
		return craftFunc?.Invoke(new List<GameItem>(), new List<GameItem>()) ?? new List<GameItem>();
	}

	public List<GameItem> Craft(List<GameItem> allItems)
	{
		if (remainingCraftAmount == 0)
		{
			return new List<GameItem>();
		}
		List<(CraftingFilter, List<GameItem>)> list = FilterTools(allItems);
		List<(CraftingFilter, List<GameItem>)> list2 = FilterMaterials(allItems);
		if (!VerifyTools(list))
		{
			return new List<GameItem>();
		}
		if (!VerifyMaterials(list2))
		{
			return new List<GameItem>();
		}
		List<GameItem> list3 = new List<GameItem>();
		List<GameItem> list4 = new List<GameItem>();
		foreach (var item6 in list)
		{
			List<GameItem> item2 = item6.Item2;
			list3.AddRange(item2);
		}
		foreach (var item7 in list2)
		{
			List<GameItem> item3 = item7.Item2;
			list4.AddRange(item3);
		}
		List<GameItem> result = craftFunc?.Invoke(list3, list4) ?? new List<GameItem>();
		foreach (var item8 in list)
		{
			var (filter, list5) = item8;
			if (filter.amount >= 0)
			{
				GameItem gameItem = list5.Find((GameItem item) => PowerHelper.GetAvailableEnergyFromItem(item, useBandwidth: false) >= filter.amount);
				if (gameItem != null)
				{
					PowerHelper.TryRemoveEnergy(gameItem, filter.amount, useBandwidth: false);
				}
			}
		}
		foreach (var item9 in list2)
		{
			CraftingFilter item4 = item9.Item1;
			List<GameItem> item5 = item9.Item2;
			int num = item4.amount;
			foreach (GameItem item10 in item5)
			{
				if (num <= 0)
				{
					break;
				}
				int num2 = Math.Min(num, item10.unitCount);
				item10.SetAmount(item10.unitCount - num2);
				num -= num2;
				if (item10.unitCount <= 0)
				{
					item10.Destroy();
				}
			}
		}
		if (remainingCraftAmount > 0)
		{
			remainingCraftAmount--;
		}
		return result;
	}

	public bool CanCraft(GraphNodeStorage input)
	{
		return CanCraft(GraphUtils.FindAllChildrenType<GameItem>(input));
	}

	public List<GameItem> TryCraftInside(GraphNodeStorage input, GraphNodeStorage output)
	{
		List<GameItem> allItems = GraphUtils.FindAllChildrenType<GameItem>(input);
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem item in Craft(allItems))
		{
			if (GraphUtils.TryAcceptAll(output, item) > 0)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public RichTextBuilder GetCraftingDetails(List<GameItem> allItems)
	{
		List<(CraftingFilter, List<GameItem>)> list = FilterTools(allItems);
		List<(CraftingFilter, List<GameItem>)> list2 = FilterMaterials(allItems);
		RichTextBuilder richTextBuilder = new RichTextBuilder();
		richTextBuilder.Add(description);
		richTextBuilder.AddLine(" ");
		richTextBuilder.AddLine(" ");
		if (remainingCraftAmount >= 0)
		{
			richTextBuilder.AddLine("Remaining Attempts:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
			richTextBuilder.AddLine(remainingCraftAmount.ToString() ?? "", ignoreEmpty: true, (remainingCraftAmount > 0) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightRed);
			richTextBuilder.AddLine(" ");
		}
		if (list.Any())
		{
			richTextBuilder.AddLine("Required Tools:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
		}
		foreach (var (craftingFilter, list3) in list)
		{
			if (craftingFilter.amount < 0)
			{
				if (list3.Any())
				{
					richTextBuilder.Add("·", RenderHandler.ColorPalette.LightGreen).Add(craftingFilter.displayName);
					richTextBuilder.AddLine(" (Found)", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
				}
				else
				{
					richTextBuilder.Add("·", RenderHandler.ColorPalette.LightRed).Add(craftingFilter.displayName);
					richTextBuilder.AddLine(" (Not Found)", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
				}
			}
			else if (list3.Any())
			{
				GameItem gameItem = null;
				int num = -1;
				foreach (GameItem item3 in list3)
				{
					if (PowerHelper.IsAvailableEnergyFromItem(item3))
					{
						int availableEnergyFromItem = PowerHelper.GetAvailableEnergyFromItem(item3, useBandwidth: false);
						if (availableEnergyFromItem > num)
						{
							gameItem = item3;
							num = availableEnergyFromItem;
						}
						if (num > craftingFilter.amount)
						{
							break;
						}
					}
				}
				if (num < 0)
				{
					num = 0;
				}
				string text = num + "/" + craftingFilter.amount;
				RenderHandler.ColorPalette color = ((num < craftingFilter.amount) ? RenderHandler.ColorPalette.LightRed : RenderHandler.ColorPalette.LightGreen);
				if (gameItem == null)
				{
					_ = craftingFilter.displayName;
				}
				else
				{
					gameItem.GetDisplayName();
				}
				richTextBuilder.Add("·", color).Add(craftingFilter.displayName);
				richTextBuilder.AddLine(" (" + text + "kW)", ignoreEmpty: true, color);
			}
			else
			{
				richTextBuilder.Add("·", RenderHandler.ColorPalette.LightRed).Add(craftingFilter.displayName);
				string text2 = "0/" + craftingFilter.amount;
				richTextBuilder.AddLine(" (" + text2 + "kW)", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			}
		}
		if (list.Any())
		{
			richTextBuilder.AddLine(" ");
		}
		richTextBuilder.AddLine("Required Materials:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
		foreach (var item4 in list2)
		{
			CraftingFilter item = item4.Item1;
			List<GameItem> item2 = item4.Item2;
			int num2 = ((item.amount < 0) ? 1 : item.amount);
			int num3 = item2.Select((GameItem gi) => gi.unitCount).Sum();
			if (num3 > num2)
			{
				num3 = num2;
			}
			RenderHandler.ColorPalette color2 = ((num3 < num2) ? RenderHandler.ColorPalette.LightRed : RenderHandler.ColorPalette.LightGreen);
			richTextBuilder.Add("·", color2).Add(item.displayName);
			richTextBuilder.AddLine(" (" + num3 + "/" + num2 + ")", ignoreEmpty: true, color2);
		}
		return richTextBuilder;
	}

	public RichTextBuilder GetVendingDetails(List<GameItem> allItems)
	{
		List<(CraftingFilter, List<GameItem>)> list = FilterTools(allItems);
		FilterMaterials(allItems);
		RichTextBuilder richTextBuilder = new RichTextBuilder();
		richTextBuilder.Add(description);
		richTextBuilder.AddLine(" ");
		richTextBuilder.AddLine(" ");
		if (remainingCraftAmount >= 0)
		{
			richTextBuilder.AddLine("Remaining Stock:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
			richTextBuilder.AddLine(remainingCraftAmount.ToString() ?? "", ignoreEmpty: true, (remainingCraftAmount > 0) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightRed);
			richTextBuilder.AddLine(" ");
		}
		richTextBuilder.AddLine("Required Power:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
		foreach (var (craftingFilter, list2) in list)
		{
			if (craftingFilter.amount < 0)
			{
				if (list2.Any())
				{
					richTextBuilder.Add("·", RenderHandler.ColorPalette.LightGreen).Add(craftingFilter.displayName);
					richTextBuilder.AddLine(" (Found)", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
				}
				else
				{
					richTextBuilder.Add("·", RenderHandler.ColorPalette.LightRed).Add(craftingFilter.displayName);
					richTextBuilder.AddLine(" (Not Found)", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
				}
			}
			else if (list2.Any())
			{
				GameItem gameItem = null;
				int num = -1;
				foreach (GameItem item in list2)
				{
					if (PowerHelper.IsAvailableEnergyFromItem(item))
					{
						int availableEnergyFromItem = PowerHelper.GetAvailableEnergyFromItem(item, useBandwidth: false);
						if (availableEnergyFromItem > num)
						{
							gameItem = item;
							num = availableEnergyFromItem;
						}
						if (num > craftingFilter.amount)
						{
							break;
						}
					}
				}
				if (num < 0)
				{
					num = 0;
				}
				string text = num + "/" + craftingFilter.amount;
				RenderHandler.ColorPalette color = ((num < craftingFilter.amount) ? RenderHandler.ColorPalette.LightRed : RenderHandler.ColorPalette.LightGreen);
				string text2 = ((gameItem != null) ? gameItem.GetDisplayName() : craftingFilter.displayName);
				richTextBuilder.Add("·", color).Add(text2);
				richTextBuilder.AddLine(" (" + text + "kW)", ignoreEmpty: true, color);
			}
			else
			{
				richTextBuilder.Add("·", RenderHandler.ColorPalette.LightRed).Add(craftingFilter.displayName);
				string text3 = "0/" + craftingFilter.amount;
				richTextBuilder.AddLine(" (" + text3 + "kW)", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			}
		}
		richTextBuilder.AddLine(" ");
		return richTextBuilder;
	}
}

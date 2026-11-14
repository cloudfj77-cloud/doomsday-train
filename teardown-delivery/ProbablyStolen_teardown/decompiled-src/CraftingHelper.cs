using System;
using System.Collections.Generic;
using System.Linq;

public class CraftingHelper
{
	public enum CraftingTags
	{
		Table,
		Blueprint
	}

	public static CraftingRecipeManager InitCraftingBlueprint(GameItem item)
	{
		CraftingRecipeManager result;
		if (item.recipeManager == null)
		{
			result = (CraftingRecipeManager)(item.recipeManager = new CraftingRecipeManager());
		}
		else
		{
			if (!(item.recipeManager is CraftingRecipeManager craftingRecipeManager))
			{
				return null;
			}
			result = craftingRecipeManager;
		}
		item.EnableTag(CraftingTags.Blueprint);
		return result;
	}

	public static CraftingRecipeManager InitCraftingTable(GameItem item)
	{
		item.EnableTag(CraftingTags.Table);
		PixelWindow pixelWindow = new PixelWindow(canMove: true, null, WindowStyle.Machinery);
		item.SetContentWindow(pixelWindow);
		int width = 10;
		int num = 16;
		int num2 = 30;
		int num3 = num - 3;
		int num4 = 3;
		int num5 = num2 / 2;
		GameGridInventory inputInventory = new GameGridInventory(width, num);
		GameSlotInventory outputSlot = new GameSlotInventory(3, 3);
		ScrollableRichTextElement recipeDescription = new ScrollableRichTextElement(constWidth: true);
		recipeDescription.ResizePixels(UISettings.current.fontWidth * num2, UISettings.current.fontHeight * num3);
		MachineryButton recipeCraftButton = new MachineryButton("Craft", null, ShipScene.defaultButtonWidth);
		MachineryButton machineryButton = new MachineryButton("Choose", null, ShipScene.defaultButtonWidth);
		recipeCraftButton.Disable();
		MachineryPanelRow machineryPanelRow = new MachineryPanelRow();
		pixelWindow.Attach(machineryPanelRow);
		GridPixelElement gridPixelElement = new GridPixelElement(1, 2, isCentered: true);
		GridPixelElement gridPixelElement2 = new GridPixelElement(2, 1, isCentered: true);
		gridPixelElement.Attach(recipeDescription);
		gridPixelElement.Attach(gridPixelElement2, 0, UISettings.current.fontHeight * num4);
		gridPixelElement2.Attach(machineryButton, UISettings.current.fontWidth * num5);
		gridPixelElement2.Attach(recipeCraftButton, UISettings.current.fontWidth * num5);
		machineryPanelRow.Add(MachineHelper.Panel(null, inputInventory, "input"));
		machineryPanelRow.Add(new MachineryPanel((string)null, (PixelElement)gridPixelElement, "recipe"));
		machineryPanelRow.Add(MachineHelper.Panel(null, outputSlot, "output"));
		CraftingRecipe selectedRecipe = null;
		GameItem selectedItem = null;
		CraftingRecipeManager craftingRecipeManager;
		if (item.recipeManager == null)
		{
			craftingRecipeManager = new CraftingRecipeManager();
			item.recipeManager = craftingRecipeManager;
		}
		else
		{
			if (!(item.recipeManager is CraftingRecipeManager craftingRecipeManager2))
			{
				return null;
			}
			craftingRecipeManager = craftingRecipeManager2;
		}
		Action updateRecipeDescription = delegate
		{
			List<GameItem> allItems = GraphUtils.FindAllChildrenType<GameItem>(inputInventory);
			CraftingRecipe craftingRecipe3 = selectedRecipe;
			if (craftingRecipe3 != null && craftingRecipe3.CanCraft(allItems))
			{
				recipeCraftButton.Enable();
			}
			else
			{
				recipeCraftButton.Disable();
			}
			if (selectedItem != null && selectedRecipe != null)
			{
				RichTextBuilder richTextBuilder = new RichTextBuilder();
				richTextBuilder.AddLine(selectedItem.GetDisplayName(), ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
				richTextBuilder.Add(selectedRecipe.GetCraftingDetails(allItems));
				recipeDescription.SetRichText(richTextBuilder);
				outputSlot.SetBackgroundFadeSprite(selectedItem.spriteAtlasPath, selectedItem.spritePath);
			}
			else
			{
				outputSlot.ClearBackgroundSprite();
			}
		};
		machineryButton.SetCallback(delegate
		{
			List<CraftingRecipe> list = item.FindAllRecipes();
			Dictionary<GameItem, CraftingRecipe> recipeDict = new Dictionary<GameItem, CraftingRecipe>();
			foreach (CraftingRecipe recipe in list)
			{
				recipe.CraftWithoutMaterials().ForEach(delegate(GameItem item)
				{
					recipeDict.Add(item, recipe);
				});
			}
			foreach (var (gameItem2, craftingRecipe2) in recipeDict)
			{
				gameItem2.SetAllMayFalse();
				gameItem2.EnableTag("CHOICE_PREVIEW_TAG");
				if (craftingRecipe2.customName != null)
				{
					gameItem2.name = craftingRecipe2.customName;
				}
				else
				{
					gameItem2.name = "Craft: " + gameItem2.GetDisplayName();
					if (gameItem2.unitCount > 1)
					{
						gameItem2.name = gameItem2.name + " (x" + gameItem2.unitCount + ")";
					}
				}
			}
			OverlayHandler.current.ShowChooseItem(recipeDict.Keys.ToList(), "Choose Item", delegate(GameItem item)
			{
				selectedRecipe = recipeDict.GetValueOrDefault(item);
				selectedItem = item;
				updateRecipeDescription();
			});
		});
		recipeCraftButton.SetCallback(delegate
		{
			if (selectedItem != null && selectedRecipe != null && outputSlot.childItems.Count() < 1)
			{
				outputSlot.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
				selectedRecipe.TryCraftInside(inputInventory, outputSlot);
				outputSlot.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
			}
		});
		outputSlot.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		pixelWindow.AddInventoryStateChangeAction(updateRecipeDescription);
		return craftingRecipeManager;
	}

	public static VendingRecipeManager InitVendingTable(GameItem item)
	{
		item.EnableTag(CraftingTags.Table);
		PixelWindow pixelWindow = new PixelWindow(canMove: true, null, WindowStyle.Machinery);
		item.SetContentWindow(pixelWindow);
		int num = 30;
		int num2 = 16 - 3;
		int num3 = 3;
		int num4 = num / 2;
		GameSlotInventory inputInventory = new GameSlotInventory().SetBackgroundFadeSprite("Items/itematlas", "energy_credit");
		GameSlotInventory outputSlot = new GameSlotInventory(3, 3);
		ScrollableRichTextElement recipeDescription = new ScrollableRichTextElement(constWidth: true);
		recipeDescription.ResizePixels(UISettings.current.fontWidth * num, UISettings.current.fontHeight * num2);
		MachineryButton recipeCraftButton = new MachineryButton("Purchase", null, ShipScene.defaultButtonWidth);
		MachineryButton machineryButton = new MachineryButton("Select", null, ShipScene.defaultButtonWidth);
		recipeCraftButton.Disable();
		MachineryPanelRow machineryPanelRow = new MachineryPanelRow();
		pixelWindow.Attach(machineryPanelRow);
		GridPixelElement gridPixelElement = new GridPixelElement(1, 2, isCentered: true);
		GridPixelElement gridPixelElement2 = new GridPixelElement(2, 1, isCentered: true);
		gridPixelElement.Attach(recipeDescription);
		gridPixelElement.Attach(gridPixelElement2, 0, UISettings.current.fontHeight * num3);
		gridPixelElement2.Attach(machineryButton, UISettings.current.fontWidth * num4);
		gridPixelElement2.Attach(recipeCraftButton, UISettings.current.fontWidth * num4);
		machineryPanelRow.Add(MachineHelper.Panel(null, inputInventory, "input"));
		machineryPanelRow.Add(new MachineryPanel((string)null, (PixelElement)gridPixelElement, "recipe"));
		machineryPanelRow.Add(MachineHelper.Panel(null, outputSlot, "output"));
		CraftingRecipe selectedRecipe = null;
		GameItem selectedItem = null;
		VendingRecipeManager vendingRecipeManager = null;
		if (item.recipeManager == null)
		{
			vendingRecipeManager = new VendingRecipeManager();
			item.recipeManager = vendingRecipeManager;
		}
		else
		{
			if (!(item.recipeManager is VendingRecipeManager vendingRecipeManager2))
			{
				return null;
			}
			vendingRecipeManager = vendingRecipeManager2;
		}
		Action updateRecipeDescription = delegate
		{
			List<GameItem> allItems = GraphUtils.FindAllChildrenType<GameItem>(inputInventory);
			CraftingRecipe craftingRecipe3 = selectedRecipe;
			if (craftingRecipe3 != null && craftingRecipe3.CanCraft(allItems))
			{
				recipeCraftButton.Enable();
			}
			else
			{
				recipeCraftButton.Disable();
			}
			if (selectedItem != null && selectedRecipe != null)
			{
				RichTextBuilder richTextBuilder = new RichTextBuilder();
				richTextBuilder.AddLine(selectedItem.GetDisplayName(), ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
				richTextBuilder.Add(selectedRecipe.GetVendingDetails(allItems));
				recipeDescription.SetRichText(richTextBuilder);
				outputSlot.SetBackgroundFadeSprite(selectedItem.spriteAtlasPath, selectedItem.spritePath);
			}
			else
			{
				outputSlot.ClearBackgroundSprite();
			}
		};
		machineryButton.SetCallback(delegate
		{
			List<CraftingRecipe> list = item.FindAllRecipes();
			Dictionary<GameItem, CraftingRecipe> recipeDict = new Dictionary<GameItem, CraftingRecipe>();
			foreach (CraftingRecipe recipe in list)
			{
				recipe.CraftWithoutMaterials().ForEach(delegate(GameItem item)
				{
					recipeDict.Add(item, recipe);
				});
			}
			foreach (var (gameItem2, craftingRecipe2) in recipeDict)
			{
				gameItem2.SetAllMayFalse();
				gameItem2.EnableTag("CHOICE_PREVIEW_TAG");
				if (craftingRecipe2.customName != null)
				{
					gameItem2.name = craftingRecipe2.customName;
				}
				else
				{
					gameItem2.name = "Vend: " + gameItem2.GetDisplayName();
					if (gameItem2.unitCount > 1)
					{
						gameItem2.name = gameItem2.name + " (x" + gameItem2.unitCount + ")";
					}
				}
			}
			OverlayHandler.current.ShowChooseItem(recipeDict.Keys.ToList(), "Choose Item", delegate(GameItem item)
			{
				selectedRecipe = recipeDict.GetValueOrDefault(item);
				selectedItem = item;
				updateRecipeDescription();
			});
		});
		recipeCraftButton.SetCallback(delegate
		{
			if (selectedItem != null && selectedRecipe != null && outputSlot.childItems.Count() < 1)
			{
				outputSlot.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
				selectedRecipe.TryCraftInside(inputInventory, outputSlot);
				outputSlot.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
			}
		});
		outputSlot.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		pixelWindow.AddInventoryStateChangeAction(updateRecipeDescription);
		return vendingRecipeManager;
	}
}

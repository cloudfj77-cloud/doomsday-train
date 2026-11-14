using System.Collections.Generic;
using System.Linq;

public class Base : GraphNodeStorage
{
	public List<CraftingRecipe> upgrades;

	public int currentUpgrade;

	public string displayName { get; set; }

	public GameGridInventory insideInventory { get; private set; }

	public GameGridInventory outsideInventory { get; private set; }

	public GameGridScrollableInventory systemsInventory { get; private set; }

	public List<GraphNodeStorage> parents => new List<GraphNodeStorage>();

	public List<GraphNodeStorage> children
	{
		get
		{
			List<GraphNodeStorage> list = new List<GraphNodeStorage>();
			list.AddRange(new GraphNodeStorage[3] { insideInventory, outsideInventory, systemsInventory });
			return list;
		}
	}

	public Base(string displayName = "Base")
	{
		this.displayName = displayName;
		insideInventory = new GameGridInventory(12, 20);
		outsideInventory = new GameGridInventory(8, 15);
		systemsInventory = new GameGridScrollableInventory(6, 20);
		systemsInventory.EnableTag("systems");
		systemsInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => item.IsTag("systems");
		currentUpgrade = 0;
		CraftingRecipe craftingRecipe = new CraftingRecipe("Hull Upgrade 1").SetDescription("Upgrades the size of the interior from 14x20 to 20x20.");
		craftingRecipe.AddToolIdentifier("Wrench", "wrench_orange");
		craftingRecipe.AddMaterial(new CraftingIdentifierFilter("Tough building material (steel_sheet, ...)", 5, "steel_sheet", "plasteel_sheet", "durasteel_sheet", "magic_wood_planks"));
		craftingRecipe.AddMaterial(new CraftingIdentifierFilter("Binding material (nuts, nails, ...)", 10, "nuts_metal", "nails_metal"));
		craftingRecipe.SetCraftingFunc(delegate
		{
			insideInventory.SetShape(18, 20);
			systemsInventory.SetShape(6, 20);
			GameMaster.current.shipScene.Validate();
			return new List<GameItem>();
		});
		CraftingRecipe craftingRecipe2 = new CraftingRecipe("Hull Upgrade 2").SetDescription("Upgrades the size of the interior from 20x20 to 24x24.");
		craftingRecipe2.AddToolIdentifier("Wrench", "wrench_orange");
		craftingRecipe2.AddMaterial(new CraftingIdentifierFilter("Tough building material (steel_sheet, ...)", 10, "steel_sheet", "plasteel_sheet", "durasteel_sheet", "magic_wood_planks"));
		craftingRecipe2.AddMaterial(new CraftingIdentifierFilter("Binding material (nuts, nails, ...)", 20, "nuts_metal", "nails_metal"));
		craftingRecipe2.SetCraftingFunc(delegate
		{
			insideInventory.SetShape(20, 24);
			systemsInventory.SetShape(6, 24);
			GameMaster.current.shipScene.Validate();
			return new List<GameItem>();
		});
		upgrades = new List<CraftingRecipe>();
		upgrades.Add(craftingRecipe);
		upgrades.Add(craftingRecipe2);
	}

	public List<GameItem> GetAllItemsInside()
	{
		return GraphUtils.FindAllChildrenType<GameItem>(insideInventory);
	}

	public List<GameItem> GetAllItemsOutside()
	{
		return GraphUtils.FindAllChildrenType<GameItem>(outsideInventory);
	}

	public List<GameItem> GetAllItems()
	{
		List<GameItem> allItemsInside = GetAllItemsInside();
		allItemsInside.AddRange(GetAllItemsOutside());
		return allItemsInside;
	}

	public bool CanUpgrade()
	{
		if (currentUpgrade >= upgrades.Count())
		{
			return false;
		}
		return upgrades[currentUpgrade].CanCraft(GetAllItems());
	}

	public void Upgrade()
	{
		if (CanUpgrade())
		{
			upgrades[currentUpgrade].Craft(GetAllItems());
			GameMaster.current.shipScene.Validate();
			currentUpgrade++;
		}
	}

	public RichTextBuilder GetUpgradeRichText()
	{
		if (currentUpgrade >= upgrades.Count())
		{
			return new RichTextBuilder().Add("No more upgrades available");
		}
		return upgrades[currentUpgrade].GetCraftingDetails(GetAllItems());
	}

	public void Validate()
	{
	}

	public bool MayHaveValidInventorySlot(GameItem item)
	{
		foreach (GraphNodeStorage child in children)
		{
			if (child.MayHaveValidInventorySlot(item))
			{
				return true;
			}
		}
		return false;
	}

	public SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		foreach (GraphNodeStorage child in children)
		{
			SlotMarker slotMarker = child.TryFindOneValidInventorySlot(item);
			if (slotMarker.IsValid())
			{
				return slotMarker;
			}
		}
		return null;
	}

	public bool AddParent(GraphNodeStorage node)
	{
		return false;
	}

	public bool RemoveParent(GraphNodeStorage node)
	{
		return false;
	}
}

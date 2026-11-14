using System;
using System.Collections.Generic;

public class ShipSystemDirectory : ItemDirectory
{
	protected override void InitDirectory()
	{
		Add("makeshift_storage_bay", () => MakeshiftStorageBay());
		Add("storage_bay", () => StorageBay());
		Add("storage_bay_large", () => StorageBayLarge());
		Add("storage_bay_expansion_kit", () => StorageExpansionHelper.CreateKit());
		Add("machine_bay", () => MachineBay());
		Add("machine_bay_ext", () => MachineBayExt());
		Add("storage_bay_gun", () => GunsmithStorageBay());
		Add("storage_bay_chem", () => ChemistStorageBay());
		Add("smuggler_bay_mini", () => MiniSmugglerBay());
		Add("smuggler_bay", () => SmugglerBay());
		Add("smuggler_bay_mod", () => SmugglerBayMod());
		Add("hypertech_bay", () => HyperTechBay());
	}

	private GameItem MakeshiftStorageBay()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow("1100001111100000011100000000010000000000000000000000000000110000000001100000100110000110011110111111", 10);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "storage_bay");
		gameItem.unitValue = 250L;
		ContainerHelper.InitContainerItem(item2, gameItem);
		gameItem.name = LocHelper.GetLocalizedItem("item_makeshift_storage_bay_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_makeshift_storage_bay_desc");
		gameItem.EnableTag("SYSTEM_TAG");
		gameItem.EnableTag("SYSTEM_TAG_UTILITY");
		gameItem.SetGameItemType("STORAGE");
		gameItem.SetGameItemType("MAKESHIFT");
		return gameItem;
	}

	private GameItem StorageBay()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(10, 10);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "storage_bay");
		gameItem.unitValue = 300L;
		ContainerHelper.InitContainerItem(item2, gameItem);
		gameItem.name = LocHelper.GetLocalizedItem("item_storage_bay_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_storage_bay_desc");
		gameItem.EnableTag("SYSTEM_TAG");
		gameItem.EnableTag("SYSTEM_TAG_UTILITY");
		gameItem.SetGameItemType("STORAGE");
		ToggleAppearanceHelper.MakeSpriteCyclable(gameItem, "Items/items_ship", "storage_bay", "storage_bay1", "storage_bay2");
		return gameItem;
	}

	private GameItem StorageBayLarge()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(17, 10);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "storage_bay_large");
		gameItem.unitValue = 850L;
		ContainerHelper.InitContainerItem(item2, gameItem);
		gameItem.name = LocHelper.GetLocalizedItem("item_storage_bay_large_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_storage_bay_large_desc");
		gameItem.EnableTag("SYSTEM_TAG");
		gameItem.EnableTag("SYSTEM_TAG_UTILITY");
		gameItem.SetGameItemType("STORAGE");
		ToggleAppearanceHelper.MakeSpriteCyclable(gameItem, "Items/items_ship", "storage_bay_large", "storage_bay_large1", "storage_bay_large2");
		return gameItem;
	}

	private GameItem GunsmithStorageBay()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(17, 10);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "storage_bay_gun");
		gameItem.unitValue = 400L;
		List<string> allowedTags = new List<string> { "FIREARM", "GUN_MOD", "AMMUNITION" };
		List<string> allowedId = new List<string> { "gun_part", "printer_plastic", "screwdriver", "tazer", "stun_gun" };
		ContainerHelper.InitContainerItem(item2, gameItem, allowedTags, allowedId);
		gameItem.name = LocHelper.GetLocalizedItem("item_storage_bay_gun_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_storage_bay_gun_desc");
		gameItem.EnableTag("SYSTEM_TAG");
		gameItem.EnableTag("SYSTEM_TAG_UTILITY");
		gameItem.SetGameItemType("STORAGE");
		return gameItem;
	}

	private GameItem ChemistStorageBay()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(17, 10);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "storage_bay_chem");
		gameItem.unitValue = 400L;
		List<string> allowedTags = new List<string> { "CHEMICAL_SUPPLIES", "LIQUID_CONTAINER_TAG", "NARCOTIC", "MEDICAL", "WATER_PURIFICATION_SUPPLY", "CHEMICAL_PRODUCT_TAG" };
		List<string> list = new List<string> { "aquascan", "chem_scanner" };
		list.Add("chem_module");
		ContainerHelper.InitContainerItem(item2, gameItem, allowedTags, list);
		gameItem.name = LocHelper.GetLocalizedItem("item_storage_bay_chem_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_storage_bay_chem_desc");
		gameItem.EnableTag("SYSTEM_TAG");
		gameItem.EnableTag("SYSTEM_TAG_UTILITY");
		gameItem.SetGameItemType("STORAGE");
		return gameItem;
	}

	private GameItem MachineBay()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(6, 8);
		PixelWindow item2 = tuple.Item1;
		GameInventory itemInventory = tuple.Item2;
		GameItem box = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/items_tool2", "machine_bay0");
		box.unitValue = 300L;
		ContainerHelper.InitContainerItem(itemInventory, box);
		box.name = LocHelper.GetLocalizedItem("item_machine_bay_name");
		box.shortDescription = LocHelper.GetLocalizedItem("item_machine_bay_desc");
		box.EnableTag("SYSTEM_TAG");
		box.EnableTag("SYSTEM_TAG_UTILITY");
		box.SetGameItemType("STORAGE");
		itemInventory.mayInventoryAddItemFunc = delegate(GameItem addedItem, GameInventory inv)
		{
			if (!GeneralHelper.IsItemOwned(addedItem))
			{
				return false;
			}
			GameItem parentItem = inv.GetParentItem();
			if (parentItem != null && !GeneralHelper.IsItemOwned(parentItem))
			{
				return false;
			}
			if (addedItem.IsTag("BACKPACK_TAG"))
			{
				return false;
			}
			return (!addedItem.IsTag("STORAGE") || ContainerHelper.IsNestingAllowed(addedItem, parentItem)) ? true : false;
		};
		itemInventory.onSlotAddItemEndFunc = delegate
		{
			int count2 = itemInventory.childItems.Count;
			count2 = Math.Clamp(count2, 0, 4);
			box.SetSpriteAndShape("Items/items_tool2", "machine_bay" + count2);
		};
		itemInventory.onSlotRemoveItemEndFunc = delegate
		{
			int count = itemInventory.childItems.Count;
			count = Math.Clamp(count, 0, 4);
			box.SetSpriteAndShape("Items/items_tool2", "machine_bay" + count);
		};
		return box;
	}

	private GameItem MachineBayExt()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(11, 14);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_tool2", "machine_bay_ext");
		gameItem.unitValue = 1200L;
		ContainerHelper.InitContainerItem(item2, gameItem);
		gameItem.name = LocHelper.GetLocalizedItem("item_machine_bay_ext_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_machine_bay_ext_desc");
		gameItem.EnableTag("SYSTEM_TAG");
		gameItem.EnableTag("SYSTEM_TAG_UTILITY");
		gameItem.SetGameItemType("STORAGE");
		item2.mayInventoryAddItemFunc = delegate(GameItem addedItem, GameInventory inv)
		{
			if (!GeneralHelper.IsItemOwned(addedItem))
			{
				return false;
			}
			GameItem parentItem = inv.GetParentItem();
			if (parentItem != null && !GeneralHelper.IsItemOwned(parentItem))
			{
				return false;
			}
			if (addedItem.IsTag("BACKPACK_TAG"))
			{
				return false;
			}
			return (!addedItem.IsTag("STORAGE") || ContainerHelper.IsNestingAllowed(addedItem, parentItem)) ? true : false;
		};
		return gameItem;
	}

	private GameItem HyperTechBay()
	{
		GameGridInventory gameGridInventory = new GameGridInventory(16, 30);
		GameGridInventory gameGridInventory2 = new GameGridInventory(16, 30);
		MachineryTabs machineryTabs = new MachineryTabs();
		machineryTabs.AddTab(LocHelper.GetLocalizedMechanic("mech_storage_bay_tab", 1), MachineHelper.WrapInIntake(gameGridInventory));
		machineryTabs.AddTab(LocHelper.GetLocalizedMechanic("mech_storage_bay_tab", 2), MachineHelper.WrapInIntake(gameGridInventory2));
		PixelWindow contentWindow = MachineHelper.CreateMachineryWindow(true, MachineHelper.Panel(null, machineryTabs, "storage"));
		GameItem gameItem = Empty().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_tool2", "hypertech_bay");
		gameItem.unitValue = 10000L;
		ContainerHelper.InitContainerItem(gameGridInventory, gameItem);
		gameItem.name = LocHelper.GetLocalizedItem("item_hypertech_bay_name");
		StorageExpansionHelper.InitTabNames(gameItem);
		gameItem.EnableTag("SYSTEM_TAG");
		gameItem.EnableTag("SYSTEM_TAG_UTILITY");
		gameItem.SetGameItemType("STORAGE");
		gameGridInventory.mayInventoryAddItemFunc = delegate(GameItem addedItem, GameInventory inv)
		{
			if (!GeneralHelper.IsItemOwned(addedItem))
			{
				return false;
			}
			GameItem parentItem = inv.GetParentItem();
			if (parentItem != null && !GeneralHelper.IsItemOwned(parentItem))
			{
				return false;
			}
			if (addedItem.IsTag("BACKPACK_TAG"))
			{
				return false;
			}
			return (!addedItem.IsTag("STORAGE") || ContainerHelper.IsNestingAllowed(addedItem, parentItem)) ? true : false;
		};
		gameGridInventory2.mayInventoryAddItemFunc = gameGridInventory.mayInventoryAddItemFunc;
		return gameItem;
	}

	private GameItem SmugglerBay()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(5, 5);
		PixelWindow item2 = tuple.Item1;
		GameInventory item3 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/items_ship", "smuggler_bay");
		item3.identifier = "smuggler_bay";
		gameItem.unitValue = 250L;
		ContainerHelper.InitSmugglerBay(item3, gameItem);
		gameItem.name = LocHelper.GetLocalizedItem("item_smuggler_bay_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_smuggler_bay_desc");
		gameItem.EnableTag("CONTAINER_TAG");
		gameItem.EnableTag("ITEM_HIDDEN_TAG");
		gameItem.SetGameItemType("STORAGE");
		item3.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item3.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
		});
		item3.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item3.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
		});
		gameItem.EnableTag("SYSTEM_TAG");
		gameItem.EnableTag("SYSTEM_TAG_UTILITY");
		return gameItem;
	}

	private GameItem SmugglerBayMod()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(8, 8);
		PixelWindow item2 = tuple.Item1;
		GameInventory item3 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/items_ship", "smuggler_bay_large");
		item3.identifier = "smuggler_bay";
		gameItem.unitValue = 400L;
		ContainerHelper.InitSmugglerBay(item3, gameItem);
		gameItem.name = LocHelper.GetLocalizedItem("item_smuggler_bay_mod_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_smuggler_bay_mod_desc");
		gameItem.EnableTag("CONTAINER_TAG");
		gameItem.EnableTag("ITEM_HIDDEN_TAG");
		gameItem.SetGameItemType("STORAGE");
		item3.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item3.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
		});
		item3.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item3.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
		});
		gameItem.EnableTag("SYSTEM_TAG");
		gameItem.EnableTag("SYSTEM_TAG_UTILITY");
		return gameItem;
	}

	private GameItem MiniSmugglerBay()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(3, 3);
		PixelWindow item2 = tuple.Item1;
		GameInventory item3 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/items_ship", "smuggler_bay_mini");
		item3.identifier = "smuggler_bay";
		gameItem.unitValue = 150L;
		ContainerHelper.InitSmugglerBay(item3, gameItem);
		gameItem.name = LocHelper.GetLocalizedItem("item_smuggler_bay_mini_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_smuggler_bay_mini_desc");
		gameItem.EnableTag("CONTAINER_TAG");
		gameItem.EnableTag("ITEM_HIDDEN_TAG");
		gameItem.SetGameItemType("STORAGE");
		AudioHelper.InitSmallMachine(gameItem);
		item3.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item3.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
		});
		item3.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item3.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
		});
		gameItem.EnableTag("SYSTEM_TAG");
		gameItem.EnableTag("SYSTEM_TAG_UTILITY");
		return gameItem;
	}
}

using System.Collections.Generic;
using System.Linq;

public class MachineFurnace
{
	public static GameItem Furnace()
	{
		(PixelWindow, GameSlotInventory, GameInventory, GameGridInventory, GameInventory, GameSlotInventory) tuple = CreateMachineInventoryWindow(8, 8);
		GameInventory item2 = tuple.Item4;
		PixelWindow item3 = tuple.Item1;
		GameSlotInventory itemInventoryBattery = tuple.Item2;
		GameInventory itemInventoryModule = tuple.Item3;
		GameInventory itemInventoryLeft = item2;
		GameInventory itemInventoryRight = tuple.Item5;
		GameSlotInventory item4 = tuple.Item6;
		item4.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => item.identifier.Equals("postit");
		item4.SetBackgroundFadeSprite("Items/items_tool", "postit");
		itemInventoryLeft.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				return false;
			}
			if (item.identifier.Equals("junk") && HaveJunkModule(itemInventoryModule))
			{
				return true;
			}
			return item.identifier.Equals("scrap_metal") || item.identifier.Equals("common_ore") || item.identifier.Equals("flux_agent") || item.identifier.Equals("advanced_flux_agent");
		};
		itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		GameItem furnace = ItemDirectory.CreateEmptyItem().SetContentWindow(item3).SetSpriteAndShape("Items/items_tool2", "furnace");
		furnace.name = LocHelper.GetLocalizedItem("item_furnace_name");
		furnace.shortDescription = LocHelper.GetLocalizedItem("item_furnace_desc");
		furnace.unitValue = 200L;
		furnace.EnableTag("CONTAINER_TAG");
		furnace.EnableTag("STANDARD_MACHINE_TAG");
		furnace.EnableTag("manufacture");
		MachineHelper.SetupBatterySlot(itemInventoryBattery, furnace);
		MachineryHelper.InitMachinery(furnace, 8);
		string[] tags = new string[2] { "MODULE_TYPE_UNIVERSAL", "MODULE_TYPE_FURNACE" };
		ContainerHelper.AllowOnlyTaggedItemsOr(itemInventoryModule, tags, ownedOnly: true);
		itemInventoryModule.onSlotAddItemEndFunc = delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			ModuleEffectHelper.OnAddedModuleToGrid(addedItem, furnace, itemInventoryModule.childItems);
			if (addedItem.IsTag("MODULE_TAG"))
			{
				ModuleHelper.ComputeModuleEffect(itemInventoryModule, addedItem, furnace);
				ModuleHelper.ApplyBasicModuleEffect(itemInventoryModule, addedItem, furnace);
			}
			MachineryHelper.UpdatePowerUsageProcessTypeMachine(furnace);
		};
		itemInventoryModule.onSlotRemoveItemEndFunc = delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			ModuleEffectHelper.OnRemovedModuleFromGrid(addedItem, furnace, itemInventoryModule.childItems);
			if (addedItem.IsTag("MODULE_TAG"))
			{
				ModuleHelper.ComputeModuleEffect(itemInventoryModule, addedItem, furnace);
				ModuleHelper.ApplyBasicModuleEffect(itemInventoryModule, addedItem, furnace);
			}
			MachineryHelper.UpdatePowerUsageProcessTypeMachine(furnace);
		};
		furnace.onCycleEndSlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			itemInventoryRight.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
			{
				if (!GeneralHelper.IsItemOwned(item))
				{
					return false;
				}
				return item.IsGameItemType("MATERIAL") ? true : false;
			};
			GameItem gameItem = DirectoryMaster.Item("metal_ingot");
			bool num = GraphUtils.CanAccept(itemInventoryRight, gameItem);
			gameItem.Destroy();
			if (num)
			{
				List<GameItem> first = GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "scrap_metal");
				List<GameItem> second = GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "common_ore");
				List<GameItem> list = first.Concat(second).ToList();
				GameItem gameItem2 = GraphUtils.FindChildType(itemInventoryLeft, (GameItem item) => item.identifier == "flux_agent");
				GameItem gameItem3 = GraphUtils.FindChildType(itemInventoryLeft, (GameItem item) => item.identifier == "advanced_flux_agent");
				GameItem gameItem4 = GraphUtils.FindChildType(itemInventoryModule, (GameItem item) => item.identifier == "furnace_module_blast");
				GameItem gameItem5 = GraphUtils.FindChildType(itemInventoryModule, (GameItem item) => item.identifier == "furnace_module_junk");
				bool flag = false;
				int num2 = 2;
				if (gameItem4 != null)
				{
					num2 = 3;
				}
				if (list.Count >= num2 && itemInventoryBattery.childItem != null)
				{
					int machinePowerUsage = MachineryHelper.GetMachinePowerUsage(item);
					if (PowerHelper.DrawPowerSource(itemInventoryBattery.childItem, machinePowerUsage))
					{
						int num3 = MachineryHelper.GetCurrentQualityBonus(furnace) / 100;
						if (gameItem4 != null)
						{
							num3--;
						}
						if (gameItem3 != null)
						{
							if (num3 < 1)
							{
								num3 = 1;
							}
						}
						else if (gameItem2 != null && num3 < 1)
						{
							num3 = 1;
							gameItem2.Destroy();
						}
						List<GameItem> range = list.GetRange(list.Count - num2, num2);
						GameItem gameItem6 = DirectoryMaster.Item("metal_ingot");
						IngotPurityHelper.SetPurityFromInput(gameItem6, range, num3);
						GraphUtils.EmporiumTryAdd(itemInventoryRight, gameItem6);
						if (gameItem4 != null)
						{
							GameItem gameItem7 = DirectoryMaster.Item("metal_ingot");
							IngotPurityHelper.SetPurityFromInput(gameItem7, range, num3);
							GraphUtils.EmporiumTryAdd(itemInventoryRight, gameItem7);
						}
						foreach (GameItem item6 in range)
						{
							item6.Destroy();
						}
						flag = true;
						MachineHelper.OnMachineActioned(furnace, itemInventoryModule);
					}
				}
				if (!flag && gameItem5 != null)
				{
					List<GameItem> list2 = GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "junk");
					if (list2.Count >= 6 && itemInventoryBattery.childItem != null)
					{
						GameItem gameItem8 = DirectoryMaster.Item("scrap_metal");
						bool num4 = GraphUtils.CanAccept(itemInventoryRight, gameItem8);
						gameItem8.Destroy();
						int machinePowerUsage2 = MachineryHelper.GetMachinePowerUsage(item);
						if (num4 && PowerHelper.DrawPowerSource(itemInventoryBattery.childItem, machinePowerUsage2))
						{
							for (int i = 0; i < 6; i++)
							{
								list2[i].Destroy();
							}
							for (int j = 0; j < 4; j++)
							{
								GameItem item5 = DirectoryMaster.Item("scrap_metal");
								GraphUtils.EmporiumTryAdd(itemInventoryRight, item5);
							}
							MachineHelper.OnMachineActioned(furnace, itemInventoryModule);
						}
					}
				}
				itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
			}
		};
		return furnace;
	}

	public static void Produce()
	{
	}

	public static (PixelWindow, GameSlotInventory, GameInventory, GameGridInventory, GameInventory, GameSlotInventory) CreateMachineInventoryWindow(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 5)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory("10000010001000001110000010001000001", 7);
		GameGridInventory gameGridInventory2 = new GameGridInventory(9, 6);
		GameGridInventory gameGridInventory3 = new GameGridInventory(9, 6);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_furnace_label_module"), gameGridInventory, "module"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_furnace_label_input"), gameGridInventory2, "input"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_furnace_label_output"), gameGridInventory3, "output"), MachineHelper.NotePanel(gameSlotInventory2)), gameSlotInventory, gameGridInventory, gameGridInventory2, gameGridInventory3, gameSlotInventory2);
	}

	public static GameItem CreateNote()
	{
		GameItem gameItem = DirectoryMaster.Item("postit");
		gameItem.name = LocHelper.GetLocalizedItem("item_furnace_instruction_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_furnace_instruction_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_furnace_instruction_short");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 0L;
		return gameItem;
	}

	public static bool HaveJunkModule(GameInventory moduleInventory)
	{
		return GraphUtils.FindChildType(moduleInventory, (GameItem item) => item.identifier == "furnace_module_junk") != null;
	}
}

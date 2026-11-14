using System;
using System.Collections.Generic;

public class MachinePrinter
{
	public const string PRINTER_CHIP_TAG = "PRINTER_CHIP_TAG";

	public const string PRINTER_MATERIAL_TAG = "PRINTER_MATERIAL_TAG";

	public const string PRINTER_METAL_PRINTING_TAG = "Metal Printing Module";

	public const string PRINTER_CNC_TAG = "CNC Module";

	public const string PRINTER_TAG = "PRINTER_TAG";

	public const string PRINTER_FRAME_CHIP_TAG = "PRINTER_FRAME_CHIP_TAG";

	public const string PRINTER_GUN_PART_CHIP_TAG = "PRINTER_GUN_PART_CHIP_TAG";

	public const string PRINTER_GUN_PART_COUNT_INT = "PRINTER_GUN_PART_COUNT_INT";

	public const int GUN_PART_COUNT_DEFAULT = 3;

	private static readonly Dictionary<string, int> GUN_PART_COUNT_BY_PURITY = new Dictionary<string, int>
	{
		{ "INGOT_PURITY_VERY_LOW", 2 },
		{ "INGOT_PURITY_LOW", 3 },
		{ "INGOT_PURITY_FAIR", 3 },
		{ "INGOT_PURITY_HIGH", 4 },
		{ "INGOT_PURITY_VERY_HIGH", 4 },
		{ "INGOT_PURITY_PERFECT", 5 }
	};

	public const string PRINTER_MELEE_CHIP_TAG = "PRINTER_MELEE_CHIP_TAG";

	public const string PRINTER_MELEE_BATCH_MODULE_TAG = "PRINTER_MELEE_BATCH_MODULE_TAG";

	public const int MELEE_BATCH_OUTPUT_BONUS = 1;

	public const string PRINTER_INGOT_QUALITY_BONUS_INT = "PRINTER_INGOT_QUALITY_BONUS_INT";

	private static readonly Dictionary<string, int> INGOT_QUALITY_BONUS = new Dictionary<string, int>
	{
		{ "INGOT_PURITY_VERY_LOW", -30 },
		{ "INGOT_PURITY_LOW", -20 },
		{ "INGOT_PURITY_FAIR", 0 },
		{ "INGOT_PURITY_HIGH", 20 },
		{ "INGOT_PURITY_VERY_HIGH", 30 },
		{ "INGOT_PURITY_PERFECT", 100 }
	};

	public const int MELEE_ROLL_MIN = 20;

	public const int MELEE_ROLL_MAX = 30;

	public const int MELEE_CONDITION_FAIR_SCORE = 25;

	public const int MELEE_CONDITION_GOOD_SCORE = 60;

	public const int MELEE_CONDITION_EXCELLENT_SCORE = 90;

	public const int OUTPUT_WIDTH = 9;

	public const int OUTPUT_HEIGHT = 5;

	public const int QUALITY_RATIO = 4;

	public static GameItem Printer()
	{
		(PixelWindow, GameSlotInventory, GameGridInventory, GameGridInventory, GameSlotInventory, GameGridInventory, GameSlotInventory) tuple = CreateMachineInventoryWindow(8, 8);
		PixelWindow item2 = tuple.Item1;
		GameSlotInventory itemInventoryBattery = tuple.Item2;
		GameGridInventory itemInventoryModule = tuple.Item3;
		GameGridInventory itemInventoryLeft = tuple.Item4;
		GameSlotInventory chipInv = tuple.Item5;
		GameGridInventory itemInventoryRight = tuple.Item6;
		GameSlotInventory item3 = tuple.Item7;
		item3.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => item.identifier.Equals("postit");
		item3.SetBackgroundFadeSprite("Items/items_tool", "postit");
		chipInv.SetBackgroundFadeSprite("Items/items_tool", "printer_chip_pistol");
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(item2).SetSpriteAndShape("Items/items_tool2", "printer_off");
		machine.name = LocHelper.GetLocalizedItem("item_3d_printer_name");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_3d_printer_desc");
		machine.unitValue = 350L;
		machine.EnableTag("CONTAINER_TAG");
		machine.EnableTag("manufacture");
		machine.EnableTag("STANDARD_MACHINE_TAG");
		machine.EnableTag("PRINTER_TAG");
		MachineryHelper.InitMachinery(machine, 8);
		MachineProgressHelper.InitProgressTypeMachine(machine, 24);
		machine.ModifyTag("MACHINE_QUALITY_RATIO_INT", delegate(TagState state)
		{
			state.Enable().SetInt(4);
		});
		MachineHelper.SetupBatterySlot(itemInventoryBattery, machine);
		ContainerHelper.AllowOnlyTaggedItem(chipInv, "PRINTER_CHIP_TAG", ownedOnly: true);
		ContainerHelper.AllowOnlyOwnedItemsExcludeContainers(itemInventoryRight);
		ContainerHelper.AllowOnlyOwnedItemsExcludeContainers(itemInventoryLeft);
		RefuseTurboBoosters(itemInventoryLeft);
		RefuseTurboBoosters(itemInventoryRight);
		string[] tags = new string[2] { "MODULE_TYPE_UNIVERSAL", "MODULE_TYPE_PRINTER" };
		ContainerHelper.AllowOnlyTaggedItemsOr(itemInventoryModule, tags, ownedOnly: true);
		itemInventoryModule.onSlotAddItemEndFunc = delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			ModuleEffectHelper.OnAddedModuleToGrid(addedItem, machine, itemInventoryModule.childItems);
			if (addedItem.IsTag("MODULE_TAG"))
			{
				ModuleHelper.ComputeModuleEffect(itemInventoryModule, addedItem, machine);
				ModuleHelper.ApplyBasicModuleEffect(itemInventoryModule, addedItem, machine);
			}
			MachineryHelper.UpdatePowerUsageProcessTypeMachine(machine);
		};
		itemInventoryModule.onSlotRemoveItemEndFunc = delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			ModuleEffectHelper.OnRemovedModuleFromGrid(addedItem, machine, itemInventoryModule.childItems);
			if (addedItem.IsTag("MODULE_TAG"))
			{
				ModuleHelper.ComputeModuleEffect(itemInventoryModule, addedItem, machine);
				ModuleHelper.ApplyBasicModuleEffect(itemInventoryModule, addedItem, machine);
			}
			MachineryHelper.UpdatePowerUsageProcessTypeMachine(machine);
		};
		GameSlotInventory gameSlotInventory = chipInv;
		gameSlotInventory.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameSlotInventory.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			AudioManager.Instance.Play3("module_insert");
		});
		GameSlotInventory gameSlotInventory2 = chipInv;
		gameSlotInventory2.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameSlotInventory2.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			AudioManager.Instance.Play3("module_remove");
		});
		machine.onCycleEndSlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			item.ModifyTag("MACHINE_QUALITY_RATIO_INT", delegate(TagState state)
			{
				state.Enable().SetInt(4);
			});
			SyncModuleTags(item, itemInventoryModule);
			int machinePowerUsage = MachineryHelper.GetMachinePowerUsage(item);
			if (MachineHelper.CanPower(item, itemInventoryBattery))
			{
				if (MachineProgressHelper.GetMachineState(item).Equals("STATE_READY"))
				{
					if (chipInv.childItem != null && MachineProgressHelper.HasRequiredAdditionalItems(chipInv.childItem, itemInventoryLeft.childItems) && MachineProgressHelper.CanStart(chipInv.childItem, item))
					{
						CaptureIngotQuality(item, chipInv.childItem, itemInventoryLeft.childItems);
						MachineProgressHelper.ConsumeRequiredAdditionalItems(chipInv.childItem, itemInventoryLeft.childItems);
						GeneralHelper.LockItem(chipInv.childItem);
						PowerHelper.DrawPowerSource(itemInventoryBattery.childItem, machinePowerUsage);
						MachineProgressHelper.StartProgressTypeMachine(item, chipInv.childItem);
						MachineHelper.LockModuleInv(item);
						MachineProgressHelper.ContinueProgressTypeMachine(item);
						item.SetSpriteAndShape("Items/items_tool2", "printer_on");
						if (MachineProgressHelper.IsProgressTypeMachineFinished(item))
						{
							FinishPrinting(item, chipInv, itemInventoryRight, itemInventoryModule);
						}
						MachineHelper.OnMachineActioned(machine, itemInventoryModule);
					}
				}
				else if (MachineProgressHelper.GetMachineState(item).Equals("STATE_WORKING"))
				{
					PowerHelper.DrawPowerSource(itemInventoryBattery.childItem, machinePowerUsage);
					item.SetSpriteAndShape("Items/items_tool2", "printer_on");
					MachineProgressHelper.ContinueProgressTypeMachine(item);
					if (MachineProgressHelper.IsProgressTypeMachineFinished(item))
					{
						FinishPrinting(item, chipInv, itemInventoryRight, itemInventoryModule);
					}
					MachineHelper.OnMachineActioned(machine, itemInventoryModule);
				}
			}
		};
		return machine;
	}

	public static void SyncModuleTags(GameItem printer, GameGridInventory moduleBay)
	{
		bool flag = false;
		bool flag2 = false;
		foreach (GameItem childItem in moduleBay.childItems)
		{
			if (childItem.IsID("printer_module_metal"))
			{
				flag = true;
			}
			if (childItem.IsID("printer_module_cnc"))
			{
				flag2 = true;
			}
		}
		if (flag)
		{
			printer.EnableTag("Metal Printing Module");
		}
		else
		{
			printer.DisableTag("Metal Printing Module");
		}
		if (flag2)
		{
			printer.EnableTag("CNC Module");
		}
		else
		{
			printer.DisableTag("CNC Module");
		}
	}

	private static void RefuseTurboBoosters(GameInventory inventory)
	{
		Func<GameItem, GameInventory, bool> previousRule = inventory.mayInventoryAddItemFunc;
		inventory.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => !MachineTurboBoosterAdv.IsTurboBooster(item) && (previousRule == null || previousRule(item, inv));
	}

	public static void FinishPrinting(GameItem item, GameSlotInventory chipInv, GameGridInventory itemInventoryRight, GameGridInventory itemInventoryModule)
	{
		GameItem childItem = chipInv.childItem;
		List<GameItem> finishedItem = MachineProgressHelper.GetFinishedItem(childItem);
		bool flag = childItem?.IsTag("PRINTER_MELEE_CHIP_TAG") ?? false;
		if (flag)
		{
			string @string = childItem.GetTagReadonly("PROGRESS_ITEM_TARGET_ITEM_ID_TAG").GetString();
			int num = CountMeleeBatchModules(item);
			for (int i = 0; i < num; i++)
			{
				finishedItem.Add(DirectoryMaster.Item(@string));
			}
		}
		else if (childItem != null && childItem.IsTag("PRINTER_GUN_PART_CHIP_TAG"))
		{
			string string2 = childItem.GetTagReadonly("PROGRESS_ITEM_TARGET_ITEM_ID_TAG").GetString();
			int num2 = (item.IsTag("PRINTER_GUN_PART_COUNT_INT") ? item.GetTagReadonly("PRINTER_GUN_PART_COUNT_INT").GetInt() : 3);
			finishedItem.Clear();
			for (int j = 0; j < num2; j++)
			{
				finishedItem.Add(DirectoryMaster.Item(string2));
			}
		}
		foreach (GameItem item2 in finishedItem)
		{
			GraphUtils.EmporiumTryAdd(itemInventoryRight, item2);
			HandlePrintedItemQuality(item2, item, itemInventoryModule, flag);
		}
		MachineProgressHelper.FinishProgressTypeMachine(item);
		GeneralHelper.UnlockItem(childItem);
		MachineHelper.UnlockModuleInv(item);
		item.SetSpriteAndShape("Items/items_tool2", "printer_off");
	}

	public static void HandlePrintedItemQuality(GameItem printedItem, GameItem printer, GameGridInventory moduleInv, bool rollsCondition = false)
	{
		if (printedItem != null)
		{
			if (rollsCondition)
			{
				ApplyPrintedMeleeCondition(printedItem, printer);
			}
			else if (printedItem.IsTag("GUN_FRAME_TAG"))
			{
				int quality = ModuleHelper.GetAjustedQuality(printer) + printer.GetTagReadonly("PRINTER_INGOT_QUALITY_BONUS_INT").GetInt();
				GunFrameHelper.SetPrintQuality(printedItem, quality);
			}
			else if (printedItem.IsTag("ADV_GUN_TAG"))
			{
				int ajustedQuality = ModuleHelper.GetAjustedQuality(printer);
				GunHelper.MultiplyBaseStat(printedItem, ajustedQuality);
			}
		}
	}

	public static int CountMeleeBatchModules(GameItem printer)
	{
		int num = 0;
		foreach (GameItem module in MachineHelper.GetModules(printer))
		{
			if (module.IsTag("PRINTER_MELEE_BATCH_MODULE_TAG"))
			{
				num++;
			}
		}
		return num;
	}

	private static void CaptureIngotQuality(GameItem printer, GameItem chip, List<GameItem> tray)
	{
		List<GameItem> list = ((chip != null) ? MachineProgressHelper.GetItemsToConsume(chip, tray) : new List<GameItem>());
		if (chip != null && chip.IsTag("PRINTER_GUN_PART_CHIP_TAG"))
		{
			int partCount = 3;
			foreach (GameItem item in list)
			{
				if (item.IsTag("INGOT_PURITY_TAG"))
				{
					string purity = IngotPurityHelper.GetPurity(item);
					if (purity != null && GUN_PART_COUNT_BY_PURITY.TryGetValue(purity, out var value))
					{
						partCount = value;
					}
					break;
				}
			}
			printer.ModifyTag("PRINTER_GUN_PART_COUNT_INT", delegate(TagState state)
			{
				state.Enable().SetInt(partCount);
			});
		}
		int bonus = 0;
		if (chip != null && (chip.IsTag("PRINTER_MELEE_CHIP_TAG") || chip.IsTag("PRINTER_FRAME_CHIP_TAG")))
		{
			int num = 0;
			int num2 = 0;
			foreach (GameItem item2 in list)
			{
				if (item2.IsTag("INGOT_PURITY_TAG"))
				{
					string purity2 = IngotPurityHelper.GetPurity(item2);
					if (purity2 != null && INGOT_QUALITY_BONUS.TryGetValue(purity2, out var value2))
					{
						num += value2;
						num2++;
					}
				}
			}
			if (num2 > 0)
			{
				bonus = num / num2;
			}
		}
		printer.ModifyTag("PRINTER_INGOT_QUALITY_BONUS_INT", delegate(TagState state)
		{
			state.Enable().SetInt(bonus);
		});
	}

	public static void ApplyPrintedMeleeCondition(GameItem weapon, GameItem printer)
	{
		int num = RNG.GetRandomInt(20, 30) + ModuleHelper.GetAjustedQuality(printer) + printer.GetTagReadonly("PRINTER_INGOT_QUALITY_BONUS_INT").GetInt();
		ItemFeatureList.EquipmentCondition equipementCondtion = ((num < 25) ? ItemFeatureList.EquipmentCondition.Poor : ((num < 60) ? ItemFeatureList.EquipmentCondition.Fair : ((num >= 90) ? ItemFeatureList.EquipmentCondition.Excellent : ItemFeatureList.EquipmentCondition.Good)));
		weapon.RemoveFeatureByCategory("CATEGORY_EQUIPMENT_CONDITION");
		weapon.AddItemFeature(ItemFeatureList.EquipementConditionFeature(equipementCondtion));
	}

	public static (PixelWindow, GameSlotInventory, GameGridInventory, GameGridInventory, GameSlotInventory, GameGridInventory, GameSlotInventory) CreateMachineInventoryWindow(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 5)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory("10000010001000001110000010001000001", 7);
		GameGridInventory gameGridInventory2 = new GameGridInventory(9, 6);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		GameGridInventory gameGridInventory3 = new GameGridInventory(9, 5);
		GameSlotInventory gameSlotInventory3 = new GameSlotInventory();
		PixelWindow item = MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_printer_label_module"), gameGridInventory, "module"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_printer_label_input"), gameGridInventory2, "input"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_printer_label_chip"), gameSlotInventory2, "chip"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_printer_label_output"), gameGridInventory3, "output"), MachineHelper.NotePanel(gameSlotInventory3));
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		gameSlotInventory2.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		gameGridInventory2.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory3.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		return (item, gameSlotInventory, gameGridInventory, gameGridInventory2, gameSlotInventory2, gameGridInventory3, gameSlotInventory3);
	}

	public static GameItem CreateNote()
	{
		GameItem gameItem = DirectoryMaster.Item("postit");
		gameItem.name = LocHelper.GetLocalizedItem("item_3d_printer_instruction_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_3d_printer_instruction_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_3d_printer_instruction_short");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 0L;
		return gameItem;
	}
}

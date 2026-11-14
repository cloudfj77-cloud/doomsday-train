using System.Collections.Generic;
using UnityEngine;

public class MachineHydroponic
{
	public const int BASE_PROCESSING_SPEED = 8;

	public const int BASE_POWER_USAGE = 8;

	public const string HYDROPONIC_MACHINE_TAG = "HYDROPONIC_MACHINE_TAG";

	public const string ATLAS = "Items/items_hydroponic_plant";

	private const int COL_BATTERY = 0;

	private const int COL_MODULE = 1;

	private const int COL_WATER = 2;

	private const int COL_SEED = 3;

	private const int COL_OUTPUT = 4;

	private const int COL_NOTE = 5;

	public static GameItem Hydroponic()
	{
		var (contentWindow, batterySlot, moduleGrid, waterSlot, inputSlot, outputGrid, manualSlot) = CreateMachineInventoryWindow();
		SetupInputSlot(inputSlot);
		SetupWaterSlot(waterSlot);
		SetupOutputGrid(outputGrid);
		MachineHelper.SetupManualSlot(manualSlot);
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_hydroponic_plant", "hydroponic");
		gameItem.name = LocHelper.GetLocalizedItem("item_hydroponic_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_hydroponic_desc");
		gameItem.flavorText = "";
		gameItem.unitValue = 300L;
		gameItem.EnableTag("CONTAINER_TAG");
		gameItem.EnableTag("manufacture");
		gameItem.EnableTag("HYDROPONIC_MACHINE_TAG");
		gameItem.EnableTag("STANDARD_MACHINE_TAG");
		MachineHelper.SetupBatterySlot(batterySlot, gameItem);
		MachineryHelper.InitMachinery(gameItem, 8);
		MachineProgressHelper.InitProgressTypeMachine(gameItem, 8, useDefaultTooltip: false);
		gameItem.ModifyTag("MACHINE_PERFORMANCE_RATIO_INT", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
		gameItem.ModifyTag("MACHINE_QUALITY_RATIO_INT", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
		MachineHelper.SetupModuleBay(moduleGrid, gameItem, null, new string[2] { "MODULE_TYPE_UNIVERSAL", "MODULE_TYPE_HYDROPONIC" });
		gameItem.onCycleEndSlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			GameItem childItem2 = inputSlot.childItem;
			string machineState = MachineProgressHelper.GetMachineState(item);
			if (childItem2 == null)
			{
				if (machineState.Equals("STATE_WORKING"))
				{
					MachineProgressHelper.FinishProgressTypeMachine(item);
					MachineHelper.UnlockModuleInv(item);
				}
				UpdateSprite(item);
			}
			else if (!MachineHelper.CanPower(item, batterySlot) || !CanWater(item))
			{
				UpdateSprite(item);
			}
			else
			{
				if (machineState.Equals("STATE_READY"))
				{
					MachineHelper.TryDrawCyclePower(item, batterySlot);
					HydroponicHelper.StartGrowing(childItem2, item);
					childItem2.name = LocHelper.GetLocalizedMechanic("mech_hydroponic_growing_prefix", childItem2.name);
					GrowOneCycle(item, childItem2, moduleGrid);
				}
				else if (machineState.Equals("STATE_WORKING"))
				{
					MachineHelper.TryDrawCyclePower(item, batterySlot);
					GrowOneCycle(item, childItem2, moduleGrid);
				}
				UpdateSprite(item);
			}
		};
		gameItem.getItemSpriteModifierForItemFunc = delegate(GameItem self)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			if (MachineProgressHelper.GetMachineState(self).Equals("STATE_WORKING"))
			{
				GameItem childItem = GetInputSlot(self).childItem;
				if (childItem != null)
				{
					string harvestItemId = HydroponicHelper.GetHarvestItemId(childItem);
					Sprite sprite = RenderHandler.LoadFromAtlas("Items/items_hydroponic_plant", harvestItemId + "_p");
					if (HydroponicHelper.IsPerennial(childItem) && HydroponicHelper.IsRipening(childItem))
					{
						Sprite sprite2 = RenderHandler.TryLoadFromAtlas("Items/items_hydroponic_plant", harvestItemId + "_p2");
						if (sprite2 != null)
						{
							sprite = sprite2;
						}
					}
					list.Add(new ItemSpriteModifier(sprite, new Vector2(0f, 0f), 0, isFlipped: false, 0, haveMask: false, Color.white));
				}
			}
			return list;
		};
		UpdateSprite(gameItem);
		return gameItem;
	}

	private static void GrowOneCycle(GameItem machine, GameItem seed, GameGridInventory moduleGrid)
	{
		MachineProgressHelper.ContinueProgressTypeMachine(machine);
		int @int = machine.GetTagReadonly("CURRENT_PROCESSING_SPEED_TAG").GetInt();
		HydroponicHelper.ContinueGrowing(seed, @int, machine);
		MachineHelper.OnMachineActioned(machine, moduleGrid);
	}

	public static GameSlotInventory GetWaterSlot(GameItem hydroponic)
	{
		return (GameSlotInventory)MachineHelper.GetBodyStorage(hydroponic, 2);
	}

	public static GameItem GetWaterSlotItem(GameItem hydroponic)
	{
		return GetWaterSlot(hydroponic).childItem;
	}

	public static GameSlotInventory GetInputSlot(GameItem hydroponic)
	{
		return (GameSlotInventory)MachineHelper.GetBodyStorage(hydroponic, 3);
	}

	public static GameGridInventory GetOutputSlot(GameItem hydroponic)
	{
		return (GameGridInventory)MachineHelper.GetBodyStorage(hydroponic, 4);
	}

	private static bool IsSeedInSlot(GameItem hydroponic)
	{
		return GetInputSlot(hydroponic).childItem != null;
	}

	private static void UpdateSprite(GameItem hydroponic)
	{
		bool flag = MachineProgressHelper.GetMachineState(hydroponic).Equals("STATE_WORKING") && MachineHelper.CanPower(hydroponic) && CanWater(hydroponic);
		hydroponic.SetSpriteAndShape("Items/items_hydroponic_plant", flag ? "hydroponic" : "hydroponic_off");
		hydroponic.Validate();
	}

	private static bool CanWater(GameItem hydroponic)
	{
		GameItem waterSlotItem = GetWaterSlotItem(hydroponic);
		GameItem childItem = GetInputSlot(hydroponic).childItem;
		if (childItem == null || waterSlotItem == null)
		{
			return false;
		}
		return WaterHelper.GetCurrentCapacityML(waterSlotItem) >= HydroponicHelper.GetWaterUsePerCycle(childItem);
	}

	public static void HandleWaterConsumption(GameItem hydroponic)
	{
		GameItem waterSlotItem = GetWaterSlotItem(hydroponic);
		GameItem childItem = GetInputSlot(hydroponic).childItem;
		if (childItem != null && waterSlotItem != null)
		{
			WaterHelper.RemoveMl(waterSlotItem, HydroponicHelper.GetWaterUsePerCycle(childItem));
		}
	}

	public static string GetGrowStatus(GameItem hydroponic)
	{
		bool flag = MachineHelper.CanPower(hydroponic);
		bool flag2 = CanWater(hydroponic);
		if (!flag && !flag2)
		{
			return LocHelper.GetLocalizedMechanic("mech_hydroponic_status_no_water_power");
		}
		if (!flag)
		{
			return LocHelper.GetLocalizedMechanic("mech_hydroponic_status_no_power");
		}
		if (!flag2)
		{
			return LocHelper.GetLocalizedMechanic("mech_hydroponic_status_no_water");
		}
		if (MachineProgressHelper.GetMachineState(hydroponic).Equals("STATE_READY"))
		{
			if (!IsSeedInSlot(hydroponic))
			{
				return LocHelper.GetLocalizedMechanic("mech_hydroponic_status_awaiting_seed");
			}
			return LocHelper.GetLocalizedMechanic("mech_hydroponic_status_starting");
		}
		return LocHelper.GetLocalizedMechanic("mech_hydroponic_status_growing");
	}

	private static void SetupInputSlot(GameSlotInventory inputSlot)
	{
		inputSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && item.IsTag("SEED") && !item.IsTag("CONTAINER_TAG");
		inputSlot.SetBackgroundFadeSprite("Items/items_hydroponic_plant", "nutrifruit_seed");
		inputSlot.mayInventoryRemoveItemFunc = delegate(GameItem item, GameInventory gameInv)
		{
			GameItem parentItem = gameInv.GetParentItem();
			return parentItem == null || !parentItem.IsTag("HYDROPONIC_MACHINE_TAG") || !MachineProgressHelper.GetMachineState(parentItem).Equals("STATE_WORKING");
		};
	}

	private static void SetupWaterSlot(GameSlotInventory waterSlot)
	{
		waterSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && item.IsTag("LIQUID_CONTAINER_TAG");
		waterSlot.SetBackgroundFadeSprite("Items/water_container4", "xl");
	}

	private static void SetupOutputGrid(GameGridInventory outputGrid)
	{
		outputGrid.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
	}

	public static (PixelWindow, GameSlotInventory, GameGridInventory, GameSlotInventory, GameSlotInventory, GameGridInventory, GameSlotInventory) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory("10000010001000001110000010001000001", 7);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory3 = new GameSlotInventory();
		GameGridInventory gameGridInventory2 = new GameGridInventory(9, 6);
		GameSlotInventory gameSlotInventory4 = new GameSlotInventory();
		gameSlotInventory3.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory2.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_hydroponic_label_module"), gameGridInventory, "module"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_hydroponic_label_water"), gameSlotInventory2, "water"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_hydroponic_label_seed"), gameSlotInventory3, "seed"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_hydroponic_label_output"), gameGridInventory2, "output"), MachineHelper.NotePanel(gameSlotInventory4)), gameSlotInventory, gameGridInventory, gameSlotInventory2, gameSlotInventory3, gameGridInventory2, gameSlotInventory4);
	}

	public static GameItem CreateNote()
	{
		GameItem gameItem = MachineHelper.CreateManual("item_hydroponic_instruction_name", "item_hydroponic_instruction_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_hydroponic_instruction_short");
		return gameItem;
	}

	public static void CreateMachineTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("HYDROPONIC_MACHINE_TAG"))
		{
			if (MachineProgressHelper.GetMachineState(item).Equals("STATE_WORKING"))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_hydroponic_tooltip_status", GetGrowStatus(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
				var (num, b) = MachineProgressHelper.GetProgress(item);
				builder.AddProgressBar((float)num / (float)Mathf.Max(1, b), 10, RenderHandler.ColorPalette.LightGreen, RenderHandler.ColorPalette.VeryDarkGreen, null, " ");
				builder.AddLine(num + "/" + b, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			else
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_hydroponic_tooltip_ready"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
	}
}

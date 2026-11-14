using System;
using TMPro;

public class MachineHeatingPlate
{
	public const string HEATING_PLATE_STATE = "HEATING_PLATE_STATE";

	public const int LOW_TEMP = 99;

	public const int HIGH_TEMP = 160;

	public const string CURRENT_TARGET_TEMP = "CURRENT_TARGET_TEMP";

	private const int TEMP_TEXT_SIZE = 9;

	public static GameItem HeatingPlate()
	{
		(MachineryPanelRow, PixelWindow, GameGridInventory, GameSlotInventory, MachineryText, MachineryRadioList) tuple = CreateMachineInventoryWindow(8, 8);
		MachineryPanelRow item2 = tuple.Item1;
		PixelWindow itemWindow = tuple.Item2;
		GameGridInventory item3 = tuple.Item3;
		GameSlotInventory containerSlot = tuple.Item4;
		MachineryText tempLabel = tuple.Item5;
		MachineryRadioList modes = tuple.Item6;
		MachineryButton buttonMinus = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_heating_plate_btn_minus"));
		MachineryText tempControl = new MachineryText(MetaHelper.FormatTemperature(99), 9, HorizontalAlignmentOptions.Center, uppercase: false).SetFillWidth(fill: false);
		MachineryButton buttonPlus = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_heating_plate_btn_plus"));
		buttonMinus.SetFillWidth(fill: false);
		buttonPlus.SetFillWidth(fill: false);
		item2.Add(MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_heating_plate_label_target_temp"), MachineryStack.Horizontal(buttonMinus, tempControl, buttonPlus), "target"));
		SetupWaterSlot(containerSlot);
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(itemWindow).SetSpriteAndShape("Items/items_tool2", "heating_plate");
		containerSlot.onSlotAddItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool2", "heating_plate_b");
		};
		containerSlot.onSlotRemoveItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool2", "heating_plate");
		};
		MachineBlender.SetupChemModuleGrid(item3, machine);
		machine.name = LocHelper.GetLocalizedItem("item_heating_plate_name");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_heating_plate_desc");
		machine.flavorText = "";
		machine.unitValue = 250L;
		machine.EnableTag("CONTAINER_TAG");
		machine.SetGameItemType("MACHINE");
		AudioHelper.InitMetalMachine(machine);
		machine.ModifyTag("CURRENT_TARGET_TEMP", delegate(TagState state)
		{
			state.Enable().SetInt(99);
		});
		modes.group.onChanged = delegate(int index)
		{
			machine.ModifyTag("HEATING_PLATE_STATE", delegate(TagState state)
			{
				state.Enable().SetInt(index);
			});
		};
		int @int = machine.GetTagReadonly("HEATING_PLATE_STATE").GetInt();
		modes.Select((@int == 1 || @int == 2) ? @int : 0);
		machine.onOneSecondItemFunc = delegate
		{
			int int2 = machine.GetTagReadonly("HEATING_PLATE_STATE").GetInt();
			if (int2 >= 0 && int2 <= 2 && modes.selected != int2)
			{
				modes.Select(int2);
			}
			if (MachineBlender.GetUpgradeCount(machine) < 2)
			{
				buttonMinus.Hide();
				buttonPlus.Hide();
			}
			else
			{
				buttonMinus.Show();
				buttonPlus.Show();
			}
			int fixedTarget = MachineBlender.GetUpgradeCount(machine) switch
			{
				0 => 99, 
				1 => 160, 
				_ => -1, 
			};
			if (fixedTarget >= 0 && machine.GetTagReadonly("CURRENT_TARGET_TEMP").GetInt() != fixedTarget)
			{
				machine.ModifyTag("CURRENT_TARGET_TEMP", delegate(TagState state)
				{
					state.Enable().SetInt(fixedTarget);
				}, skipValidate: true);
			}
			tempControl.SetText(MetaHelper.FormatTemperature(machine.GetTagReadonly("CURRENT_TARGET_TEMP").GetInt()));
			if (containerSlot.childItem != null && itemWindow.IsVisible())
			{
				tempLabel.SetText(MetaHelper.FormatTemperature((int)containerSlot.childItem.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble()));
			}
			else
			{
				tempLabel.SetText(LocHelper.GetLocalizedMechanic("mech_heating_plate_temp_na"));
			}
			if (int2 != 0 && containerSlot.childItem != null && !WaterHelper.IsEmpty(containerSlot.childItem))
			{
				HandleHeating(containerSlot, int2, machine);
			}
		};
		buttonMinus.SetCallback(delegate
		{
			int targetTemp2 = machine.GetTagReadonly("CURRENT_TARGET_TEMP").GetInt() - 5;
			targetTemp2 = Math.Max(targetTemp2, 26);
			machine.ModifyTag("CURRENT_TARGET_TEMP", delegate(TagState state)
			{
				state.Enable().SetInt(targetTemp2);
			});
			tempControl.SetText(MetaHelper.FormatTemperature(machine.GetTagReadonly("CURRENT_TARGET_TEMP").GetInt()));
		});
		buttonPlus.SetCallback(delegate
		{
			int targetTemp = machine.GetTagReadonly("CURRENT_TARGET_TEMP").GetInt() + 5;
			if (MachineBlender.GetUpgradeCount(machine) == 0)
			{
				targetTemp = Math.Min(targetTemp, 99);
			}
			else
			{
				targetTemp = Math.Min(targetTemp, 160);
			}
			machine.ModifyTag("CURRENT_TARGET_TEMP", delegate(TagState state)
			{
				state.Enable().SetInt(targetTemp);
			});
			tempControl.SetText(MetaHelper.FormatTemperature(machine.GetTagReadonly("CURRENT_TARGET_TEMP").GetInt()));
		});
		return machine;
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("heating_plate"))
		{
			int upgradeCount = MachineBlender.GetUpgradeCount(item);
			int num = ((upgradeCount == 0) ? 99 : 160);
			builder.Add(LocHelper.GetLocalizedMechanic("mech_heating_plate_max_temp", num), RenderHandler.ColorPalette.LightRed);
			if (upgradeCount < 3)
			{
				builder.Add(LocHelper.GetLocalizedMechanic("mech_heating_plate_speed_slow"), RenderHandler.ColorPalette.LightRed);
			}
			else if (upgradeCount == 3)
			{
				builder.Add(LocHelper.GetLocalizedMechanic("mech_heating_plate_speed_fast"), RenderHandler.ColorPalette.LightRed);
			}
			else
			{
				builder.Add(LocHelper.GetLocalizedMechanic("mech_heating_plate_speed_very_fast"), RenderHandler.ColorPalette.LightRed);
			}
			builder.AddLine();
		}
	}

	public static void HandleHeating(GameSlotInventory containerSlot, int heatingState, GameItem machine)
	{
		double num = 10.0;
		if (heatingState == 2)
		{
			num = 30.0;
		}
		double @double = containerSlot.childItem.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
		int totalVolume = WaterHelper.GetTotalVolume(containerSlot.childItem);
		int @int = machine.GetTagReadonly("CURRENT_TARGET_TEMP").GetInt();
		if (MachineBlender.GetUpgradeCount(machine) == 3)
		{
			num *= 4.0;
		}
		else if (MachineBlender.GetUpgradeCount(machine) == 4)
		{
			num *= 16.0;
		}
		double newTemp = HeatLiquidOnStove(@double, @int, totalVolume, num);
		containerSlot.childItem.ModifyTag("CONTAINER_TEMP_TAG", delegate(TagState state)
		{
			state.SetDouble(newTemp);
		}, skipValidate: true);
		machine.EnableTag("HEATING_TAG", skipValidate: true);
	}

	private static double HeatLiquidOnStove(double currentTemp, double stoveTemp, int volumeUL, double heatingFactor = 0.1)
	{
		if (volumeUL <= 0)
		{
			return currentTemp;
		}
		if (currentTemp >= stoveTemp)
		{
			return currentTemp;
		}
		double num = (stoveTemp - currentTemp) * (heatingFactor / 350.0) + 1.0;
		return Math.Min(currentTemp + num, stoveTemp);
	}

	private static void SetupWaterSlot(GameSlotInventory waterSlot)
	{
		waterSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && item.IsTag("LIQUID_CONTAINER_TAG");
		waterSlot.SetBackgroundFadeSprite("Items/water_container4", "xl");
	}

	public static (MachineryPanelRow, PixelWindow, GameGridInventory, GameSlotInventory, MachineryText, MachineryRadioList) CreateMachineInventoryWindow(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		GameGridInventory gameGridInventory = new GameGridInventory(4, 4);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		MachineryText machineryText = new MachineryText(LocHelper.GetLocalizedMechanic("mech_heating_plate_temp_none"), 9, HorizontalAlignmentOptions.Center, uppercase: false);
		MachineryRadioList machineryRadioList = new MachineryRadioList(new string[3]
		{
			LocHelper.GetLocalizedMechanic("mech_heating_plate_state_off"),
			LocHelper.GetLocalizedMechanic("mech_heating_plate_state_low"),
			LocHelper.GetLocalizedMechanic("mech_heating_plate_state_high")
		});
		PixelWindow pixelWindow = MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_heating_plate_label_module"), gameGridInventory, "module"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_heating_plate_label_container"), gameSlotInventory, "container"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_heating_plate_label_temp"), machineryText, "temp"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_heating_plate_label_control"), machineryRadioList, "control"));
		return ((MachineryPanelRow)pixelWindow.childElement, pixelWindow, gameGridInventory, gameSlotInventory, machineryText, machineryRadioList);
	}
}

using System;

public class MachineBottlePrinter
{
	public const string PRINTER_UPGRADE_COUNT_TAG = "BOTTLE_PRINTER_UPGRADE_COUNT_TAG";

	public const int NORMAL_UNLOCK_COST = 3;

	public const int LARGE_UNLOCK_COST = 6;

	public const int OUTPUT_BASE_WIDTH = 4;

	public const int OUTPUT_HEIGHT = 4;

	public const int SMALL_PRINT_COST = 4;

	public const int NORMAL_PRINT_COST = 6;

	public const int LARGE_PRINT_COST = 11;

	public static GameItem BottlePrinter()
	{
		var (contentWindow, itemInventory, outputGrid, machineryButton, buttonNormal, buttonLarge) = CreateMachineInventoryWindow();
		outputGrid.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && item.IsTag("LIQUID_CONTAINER_TAG");
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_tool", "bottle_printer");
		machine.name = LocHelper.GetLocalizedItem("item_bottle_printer_name");
		machine.flavorText = LocHelper.GetLocalizedItem("item_bottle_printer_flavor");
		MachineHelper.SetupBatterySlot(itemInventory, machine);
		machine.EnableTag("IMPORTANT_TAG");
		machine.EnableTag("CONTAINER_TAG");
		machine.unitValue = 100L;
		machineryButton.SetCallback(delegate
		{
			TryPrint("small_bottled_water", 4);
		});
		buttonNormal.SetCallback(delegate
		{
			if (IsNormalUnlocked(machine))
			{
				TryPrint("bottled_water", 6);
			}
		});
		buttonLarge.SetCallback(delegate
		{
			if (IsLargeUnlocked(machine))
			{
				TryPrint("large_bottled_water", 11);
			}
		});
		GameItem gameItem = machine;
		gameItem.onItemTargetThisFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onItemTargetThisFunc, (Action<GameItem, GameItem>)delegate
		{
			ApplyUpgradeState();
		});
		machine.onLoaded = delegate(GameItem loaded)
		{
			loaded.EnableTag("CONTAINER_TAG");
			ApplyUpgradeState();
		};
		ApplyUpgradeState();
		AudioHelper.InitSmallMachine(machine);
		return machine;
		void ApplyUpgradeState()
		{
			int num = 4;
			if (IsNormalUnlocked(machine))
			{
				num++;
			}
			if (IsLargeUnlocked(machine))
			{
				num++;
			}
			outputGrid.SetShape(num, 4);
			outputGrid.Validate();
			machine.contentWindow.Validate();
			machine.Validate();
			if (IsNormalUnlocked(machine))
			{
				buttonNormal.Enable();
			}
			else
			{
				buttonNormal.Disable();
			}
			if (IsLargeUnlocked(machine))
			{
				buttonLarge.Enable();
			}
			else
			{
				buttonLarge.Disable();
			}
		}
		void TryPrint(string bottleId, int cost)
		{
			if (itemInventory.childItems.Count != 0 && PowerHelper.CanDrawPowerSource(itemInventory.childItem, cost))
			{
				GameItem item2 = DirectoryMaster.Item(bottleId);
				if (GraphUtils.CanAccept(outputGrid, item2))
				{
					GraphUtils.TryAcceptAll(outputGrid, item2);
					PowerHelper.DrawPowerSource(itemInventory.childItem, cost);
				}
			}
		}
	}

	public static int GetUpgradeCount(GameItem printer)
	{
		return printer.GetTagReadonly("BOTTLE_PRINTER_UPGRADE_COUNT_TAG").GetInt();
	}

	public static bool IsNormalUnlocked(GameItem printer)
	{
		return GetUpgradeCount(printer) >= 3;
	}

	public static bool IsLargeUnlocked(GameItem printer)
	{
		return GetUpgradeCount(printer) >= 9;
	}

	public static bool CanUpgradePrinter(GameItem printer)
	{
		if (printer == null || !printer.identifier.Equals("bottle_printer"))
		{
			return false;
		}
		return GetUpgradeCount(printer) < 9;
	}

	public static void UpgradePrinter(GameItem printer, GameItem upgradeItem)
	{
		if (CanUpgradePrinter(printer))
		{
			int count = GetUpgradeCount(printer);
			printer.ModifyTag("BOTTLE_PRINTER_UPGRADE_COUNT_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(count + 1);
			});
			printer.unitValue += upgradeItem.unitValue;
			StartingPerkContent.ConsumeUpgradeMaterial(upgradeItem);
			EmporiumAudioPlayer.Instance.ratchet.Play();
		}
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("bottle_printer"))
		{
			int upgradeCount = GetUpgradeCount(item);
			builder.AddLine(" ");
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_bottle_printer_tooltip_print_small", 4), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			if (IsNormalUnlocked(item))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_bottle_printer_tooltip_print_normal", 6), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			else
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_bottle_printer_tooltip_upgrade", upgradeCount, 3), ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
			if (IsLargeUnlocked(item))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_bottle_printer_tooltip_print_large", 11), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			else if (IsNormalUnlocked(item))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_bottle_printer_tooltip_upgrade", upgradeCount - 3, 6), ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
		}
	}

	public static (PixelWindow, GameSlotInventory, GameGridInventory, MachineryButton, MachineryButton, MachineryButton) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory(4, 4);
		MachineryButton machineryButton = new MachineryButton(LocHelper.GetLocalizedMechanicOrDefault("mech_bottle_printer_btn_print_small", "Print Small Bottle"));
		MachineryButton machineryButton2 = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_bottle_printer_btn_print_normal"));
		MachineryButton machineryButton3 = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_bottle_printer_btn_print_large"));
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_bottle_printer_label_output"), gameGridInventory, "output"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_bottle_printer_label_control"), MachineryStack.Vertical(machineryButton, machineryButton2, machineryButton3), "control")), gameSlotInventory, gameGridInventory, machineryButton, machineryButton2, machineryButton3);
	}
}

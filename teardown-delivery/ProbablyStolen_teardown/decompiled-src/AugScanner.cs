public class AugScanner
{
	public const string AUG_SCANNER_TAG = "AUG_SCANNER_TAG";

	public const int ACTIVATION_COST = 25;

	public static (PixelWindow, GameSlotInventory) CreateWindow()
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		return (MachineHelper.CreateMachineryWindow(true, MachineHelper.PowerPanel(gameSlotInventory)), gameSlotInventory);
	}

	public static GameSlotInventory GetBatterySlot(GameItem scanner)
	{
		return MachineHelper.GetBodyStorage(scanner, 0) as GameSlotInventory;
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("AUG_SCANNER_TAG"))
		{
			return;
		}
		builder.AddLine(" ");
		GameSlotInventory batterySlot = GetBatterySlot(item);
		RenderHandler.ColorPalette color = ((batterySlot?.childItem != null && PowerHelper.CanDrawPowerSource(batterySlot.childItem, 25)) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.Red);
		builder.Add(LocHelper.GetLocalizedMechanic("mech_machinery_required_energy"), color);
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_aug_scanner_per_scan", 25), ignoreEmpty: true, color);
		StoreClientInstance currentClientInstance = PlayerStore.Instance.currentClientInstance;
		if (currentClientInstance != null)
		{
			StoreClient clientBlueprint = currentClientInstance.GetClientBlueprint();
			if (!clientBlueprint.isAugScanned)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_aug_scanner_status_ready"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
			}
			else if (clientBlueprint.isAug)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_aug_scanner_status_aug_detected"), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			else
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_aug_scanner_status_no_aug"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
		}
	}
}

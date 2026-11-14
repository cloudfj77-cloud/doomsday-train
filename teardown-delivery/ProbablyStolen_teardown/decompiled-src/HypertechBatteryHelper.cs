public class HypertechBatteryHelper
{
	public const string HYPERTECH_BATTERY_TAG = "HYPERTECH_BATTERY_TAG";

	public const string HYPERTECH_BATTERY_OUTPUT_INT = "HYPERTECH_BATTERY_OUTPUT_INT";

	public const int DEFAULT_OUTPUT = 12;

	public const string HYPERTECH_BATTERY_UPGRADE_COUNT_INT = "HYPERTECH_BATTERY_UPGRADE_COUNT_INT";

	public const int OUTPUT_UPGRADE_STEP = 10;

	public static void InitHypertechBattery(GameItem item, int output = 12)
	{
		PowerHelper.InitPowerSourceItem(item, 0, 0);
		item.EnableTag("HYPERTECH_BATTERY_TAG");
		item.ModifyTag("HYPERTECH_BATTERY_OUTPUT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(output);
		});
		item.mayInventoryAddItemFunc = delegate(GameItem self, GameInventory inv)
		{
			GameItem gameItem = inv?.GetParentItem();
			if (gameItem == null)
			{
				return true;
			}
			if (gameItem.IsID("recharger"))
			{
				return false;
			}
			if (gameItem.IsTag(EquipmentHelper.EQUIPMENT_TAG))
			{
				return false;
			}
			return !gameItem.IsTag("COMPLEXE_EQUIPMENT_TAG");
		};
	}

	public static bool IsHypertechBattery(GameItem item)
	{
		return item?.IsTag("HYPERTECH_BATTERY_TAG") ?? false;
	}

	public static int GetOutput(GameItem battery)
	{
		return battery.GetTagReadonly("HYPERTECH_BATTERY_OUTPUT_INT").GetInt();
	}

	public static void SetOutput(GameItem battery, int output)
	{
		battery.ModifyTag("HYPERTECH_BATTERY_OUTPUT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(output);
		});
	}

	public static int GetUpgradeCount(GameItem battery)
	{
		return battery.GetTagReadonly("HYPERTECH_BATTERY_UPGRADE_COUNT_INT").GetInt();
	}

	public static bool CanUpgradeOutput(GameItem target)
	{
		return IsHypertechBattery(target);
	}

	public static void UpgradeOutput(GameItem battery, GameItem upgradeItem)
	{
		if (CanUpgradeOutput(battery))
		{
			SetOutput(battery, GetOutput(battery) + 10);
			battery.ModifyTag("HYPERTECH_BATTERY_UPGRADE_COUNT_INT", delegate(TagState state)
			{
				state.Enable().SetInt(GetUpgradeCount(battery) + 1);
			});
			battery.unitValue += upgradeItem.unitValue;
			StartingPerkContent.ConsumeUpgradeMaterial(upgradeItem);
			EmporiumAudioPlayer.Instance.ratchet.Play();
		}
	}

	public static bool CanSupply(GameItem battery, int amount)
	{
		return amount <= GetOutput(battery);
	}

	public static GameItem GetHostMachine(GameItem battery)
	{
		GameItem gameItem = battery?.parentInventory?.GetParentItem();
		if (gameItem == null || !gameItem.IsTag("machinery_drawn_power"))
		{
			return null;
		}
		return gameItem;
	}

	public static GameItem FindHypertechBattery(GameItem machine)
	{
		if (machine == null)
		{
			return null;
		}
		foreach (GameItem item in machine.FindAllChildItems())
		{
			if (IsHypertechBattery(item))
			{
				return item;
			}
		}
		return null;
	}

	public static bool CanHostDraw(GameItem machine)
	{
		GameItem gameItem = FindHypertechBattery(machine);
		if (gameItem == null)
		{
			return true;
		}
		return CanSupply(gameItem, machine.GetTagReadonly("machinery_drawn_power").GetInt());
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (IsHypertechBattery(item))
		{
			int output = GetOutput(item);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_hypertech_cell_output", output), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_hypertech_cell_upgrade", 10, GetUpgradeCount(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			GameItem hostMachine = GetHostMachine(item);
			if (hostMachine != null)
			{
				int @int = hostMachine.GetTagReadonly("machinery_drawn_power").GetInt();
				bool flag = @int <= output;
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_hypertech_cell_connected", hostMachine.name, @int), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
				builder.AddLine(flag ? LocHelper.GetLocalizedMechanic("mech_hypertech_cell_draw_ok") : LocHelper.GetLocalizedMechanic("mech_hypertech_cell_draw_over"), ignoreEmpty: true, flag ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightOrange);
			}
		}
	}
}

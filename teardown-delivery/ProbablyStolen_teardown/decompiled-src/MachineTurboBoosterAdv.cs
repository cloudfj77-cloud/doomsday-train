using System.Collections.Generic;

public class MachineTurboBoosterAdv
{
	public const string TURBO_BOOST_ADV = "TURBO_BOOST_ADV";

	public const int POWER_USE = 5;

	public const int POWER_TARGET = 15;

	public const string CURRENT_CHARGE_TAG = "CURRENT_CHARGE_TAG";

	public const string TURBO_READY_TAG = "TURBO_READY_TAG";

	public const string TURBO_BOOST_HYPERTECH = "TURBO_BOOST_HYPERTECH";

	public const string HYPERTECH_CHARGE_RATE_TAG = "HYPERTECH_CHARGE_RATE_TAG";

	public const int HYPERTECH_BASE_RATE = 5;

	public const int HYPERTECH_UPGRADE_RATE = 5;

	public const string TURBO_BOOST_ELIGIBLE_TAG = "TURBO_BOOST_ELIGIBLE_TAG";

	private static readonly HashSet<string> turboEligibleLegacyIds = new HashSet<string> { "fridge", "evaporator" };

	public static bool IsTurboBooster(GameItem item)
	{
		if (item != null)
		{
			if (!item.IsID("turbo_booster") && !item.IsTag("TURBO_BOOST_ADV"))
			{
				return item.IsTag("TURBO_BOOST_HYPERTECH");
			}
			return true;
		}
		return false;
	}

	public static (PixelWindow, GameSlotInventory) CreateTurboBoosterWindow(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory)), gameSlotInventory);
	}

	public static bool IsTurboBoostEligible(GameItem gameItem)
	{
		if (!gameItem.IsTag("STANDARD_MACHINE_TAG"))
		{
			return gameItem.IsTag("TURBO_BOOST_ELIGIBLE_TAG");
		}
		return true;
	}

	public static bool CanBoostNow(GameItem machine)
	{
		if (IsTurboBoostEligible(machine) && CurrentConverterHelper.CanHostDraw(machine))
		{
			return HypertechBatteryHelper.CanHostDraw(machine);
		}
		return false;
	}

	public static void LoadTurboBoostEligibleTag(GameItem item)
	{
		if (turboEligibleLegacyIds.Contains(item.identifier))
		{
			item.EnableTag("TURBO_BOOST_ELIGIBLE_TAG");
		}
	}

	public static void CreateMachineTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("TURBO_BOOST_ADV"))
		{
			RenderHandler.ColorPalette color = (IsTurboReady(item) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightOrange);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_turbo_booster_status", GetStatus(item)), ignoreEmpty: true, color);
			int @int = item.GetTagReadonly("CURRENT_CHARGE_TAG").GetInt();
			builder.AddProgressBar((float)@int / 15f, 10, RenderHandler.ColorPalette.LightGreen, RenderHandler.ColorPalette.VeryDarkGreen, null, " ");
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_turbo_booster_progress", @int, 15f), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		}
		else if (item.IsTag("TURBO_BOOST_HYPERTECH"))
		{
			RenderHandler.ColorPalette color2 = (IsTurboReady(item) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightOrange);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_turbo_booster_status", GetStatus(item)), ignoreEmpty: true, color2);
			int int2 = item.GetTagReadonly("CURRENT_CHARGE_TAG").GetInt();
			int hypertechChargeRate = GetHypertechChargeRate(item);
			int num = int2 / 15;
			int num2 = int2 - num * 15;
			builder.AddProgressBar((float)num2 / 15f, 10, RenderHandler.ColorPalette.LightGreen, RenderHandler.ColorPalette.VeryDarkGreen, null, " ");
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_turbo_hypertech_charge", num2, 15), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			if (num > 0)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_turbo_hypertech_stored", num), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_turbo_hypertech_speed", hypertechChargeRate, GetHypertechLevel(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_turbo_hypertech_upgrade_hint"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
		}
	}

	public static int GetHypertechChargeRate(GameItem item)
	{
		int @int = item.GetTagReadonly("HYPERTECH_CHARGE_RATE_TAG").GetInt();
		if (@int <= 0)
		{
			return 5;
		}
		return @int;
	}

	public static int GetHypertechLevel(GameItem item)
	{
		return 1 + (GetHypertechChargeRate(item) - 5) / 5;
	}

	public static bool CanUpgradeHypertech(GameItem item)
	{
		return item.IsTag("TURBO_BOOST_HYPERTECH");
	}

	public static void UpgradeHypertech(GameItem item)
	{
		int newRate = GetHypertechChargeRate(item) + 5;
		item.ModifyTag("HYPERTECH_CHARGE_RATE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(newRate);
		});
		SetHypertechPowerTags(item);
	}

	public static void SetHypertechPowerTags(GameItem item)
	{
		int rate = GetHypertechChargeRate(item);
		item.ModifyTag("machinery_drawn_power", delegate(TagState state)
		{
			state.Enable().SetInt(rate);
		});
		item.ModifyTag("MACHINERY_DRAWN_POWER_BASE", delegate(TagState state)
		{
			state.Enable().SetInt(rate);
		});
	}

	public static void RefreshHypertechReady(GameItem item)
	{
		if (item.GetTagReadonly("CURRENT_CHARGE_TAG").GetInt() >= 15)
		{
			item.EnableTag("TURBO_READY_TAG");
		}
		else
		{
			item.DisableTag("TURBO_READY_TAG");
		}
	}

	public static void BoostMachine(GameItem machine)
	{
		machine.onCycleEndEarlySlotItemFunc?.Invoke(machine, machine.parentInventory, machine.GetIncompleteSlotInInventory());
		machine.onCycleEndSlotItemFunc?.Invoke(machine, machine.parentInventory, machine.GetIncompleteSlotInInventory());
	}

	public static string GetStatus(GameItem item)
	{
		if (item.IsTag("TURBO_READY_TAG"))
		{
			return LocHelper.GetLocalizedMechanic("mech_turbo_booster_status_ready");
		}
		return LocHelper.GetLocalizedMechanic("mech_turbo_booster_status_recharging");
	}

	public static bool IsTurboReady(GameItem item)
	{
		return item.IsTag("TURBO_READY_TAG");
	}

	public static void UpdateSprite(GameItem item, GameSlotInventory batterySlot)
	{
		if (item.IsTag("TURBO_READY_TAG"))
		{
			item.SetSpriteAndShape("Items/items_tool", "turbo_booster_adv_ready");
		}
		else if (batterySlot.childItem != null && PowerHelper.CanDrawPowerSource(batterySlot.childItem, 5))
		{
			item.SetSpriteAndShape("Items/items_tool", "turbo_booster_adv_charging");
		}
		else
		{
			item.SetSpriteAndShape("Items/items_tool", "turbo_booster_adv");
		}
	}
}

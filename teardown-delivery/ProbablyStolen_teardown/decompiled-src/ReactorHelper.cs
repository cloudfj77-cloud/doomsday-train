using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ReactorHelper
{
	public const string REACTOR_ID = "makeshift_reactor";

	public const string REACTOR_TAG = "MAKESHIFT_REACTOR_TAG";

	public const string INTEGRITY_INT = "REACTOR_INTEGRITY_INT";

	public const string MAX_INTEGRITY_INT = "REACTOR_MAX_INTEGRITY_INT";

	public const string UPGRADES_INT = "REACTOR_UPGRADES_INT";

	public const string WASTE_PROGRESS_INT = "REACTOR_WASTE_PROGRESS_INT";

	public const int START_INTEGRITY = 100;

	public const int BASE_DECAY = 18;

	public const int POWER_THRESHOLD = 25;

	public const int WARNING_INTEGRITY = 50;

	public const int SCRAP_REPAIR = 60;

	public const int JUNK_REPAIR = 30;

	public const int JUNK_MAX_LOSS = 5;

	public const int MAX_UPGRADES = 18;

	public const int BASE_CHARGE = 1;

	public const int MAX_CHARGE = 4;

	public const int UPGRADES_PER_CHARGE_STEP = 5;

	public const int DAMAGE_PER_WASTE = 160;

	public const int STOCK_REACTOR_VALUE = 150;

	private const string SPRITE_ATLAS = "Items/items_ship";

	private const string SPRITE = "makeshift_reactor";

	private static bool explodedTonight;

	public static GameItem CreateReactor()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_ship", "makeshift_reactor");
		gameItem.name = LocHelper.GetLocalizedItem("item_makeshift_reactor_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_makeshift_reactor_desc", 25, 60, 30, 5);
		gameItem.unitValue = 0L;
		gameItem.EnableTag("MAKESHIFT_REACTOR_TAG");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.SetGameItemType("MACHINE");
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			loaded.SetGameItemType("MACHINE");
		});
		gameItem.ModifyTag("REACTOR_INTEGRITY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(100);
		});
		gameItem.ModifyTag("REACTOR_MAX_INTEGRITY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(100);
		});
		gameItem.ModifyTag("REACTOR_UPGRADES_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("REACTOR_WASTE_PROGRESS_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		AudioHelper.InitMetalMachine(gameItem);
		gameItem.onCycleEndEarlySlotItemFunc = delegate(GameItem self, GameInventory _, SlotMarker _)
		{
			ChargeAdjacent(self);
		};
		return gameItem;
	}

	public static bool IsReactor(GameItem item)
	{
		return item?.IsTag("MAKESHIFT_REACTOR_TAG") ?? false;
	}

	public static int GetIntegrity(GameItem reactor)
	{
		return reactor.GetTagReadonly("REACTOR_INTEGRITY_INT").GetInt();
	}

	public static int GetMaxIntegrity(GameItem reactor)
	{
		return reactor.GetTagReadonly("REACTOR_MAX_INTEGRITY_INT").GetInt();
	}

	public static int GetUpgrades(GameItem reactor)
	{
		return reactor.GetTagReadonly("REACTOR_UPGRADES_INT").GetInt();
	}

	public static int GetDecay(GameItem reactor)
	{
		return Mathf.Max(0, 18 - GetUpgrades(reactor));
	}

	public static int GetChargePerNight(GameItem reactor)
	{
		return Mathf.Min(4, 1 + GetUpgrades(reactor) / 5);
	}

	public static bool ProvidesPower(GameItem reactor)
	{
		return GetIntegrity(reactor) > 25;
	}

	private static void SetIntegrity(GameItem reactor, int value)
	{
		reactor.ModifyTag("REACTOR_INTEGRITY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(value);
		});
	}

	private static List<GameItem> FindReactors()
	{
		return EmporiumEntry.Instance.GetAllItems().Where(IsReactor).ToList();
	}

	public static bool IsStorePowered()
	{
		return FindReactors().Any(ProvidesPower);
	}

	public static void TryOfferStockReactor(List<GameItem> playerItems)
	{
		if (StartingPerk.IsMakeshiftReactor() && !playerItems.Any(IsReactor))
		{
			GameItem gameItem = DirectoryMaster.Item("makeshift_reactor");
			gameItem.unitValue = 150L;
			PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
		}
	}

	public static void OnNight()
	{
		foreach (GameItem item in FindReactors())
		{
			int decay = GetDecay(item);
			int integrity = GetIntegrity(item);
			int num = Mathf.Max(0, integrity - decay);
			SetIntegrity(item, num);
			int wasteProgress = item.GetTagReadonly("REACTOR_WASTE_PROGRESS_INT").GetInt() + (integrity - num);
			while (wasteProgress >= 160)
			{
				wasteProgress -= 160;
				GraphUtils.EmporiumTryAdd(item.parentInventory ?? EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nuclear_waste"));
			}
			item.ModifyTag("REACTOR_WASTE_PROGRESS_INT", delegate(TagState state)
			{
				state.Enable().SetInt(wasteProgress);
			});
			if (num <= 0)
			{
				explodedTonight = true;
				continue;
			}
			if (num <= 25)
			{
				PlayerStore.Instance.AddNightLog(NoPowerText(), "cc7755");
			}
			if (WillExplodeNextNight(item))
			{
				PlayerStore.Instance.AddNightLog(CriticalText(), "9c3738");
			}
		}
	}

	public static bool WillExplodeNextNight(GameItem reactor)
	{
		int decay = GetDecay(reactor);
		if (decay > 0)
		{
			return GetIntegrity(reactor) - decay <= 0;
		}
		return false;
	}

	private static string NoPowerText()
	{
		return LocHelper.GetLocalizedMechanic("mech_nightlog_reactor_no_power");
	}

	private static string CriticalText()
	{
		return LocHelper.GetLocalizedMechanic("mech_nightlog_reactor_critical");
	}

	public static bool ConsumeExplosion()
	{
		bool result = explodedTonight;
		explodedTonight = false;
		return result;
	}

	private static void ChargeAdjacent(GameItem reactor)
	{
		int chargePerNight = GetChargePerNight(reactor);
		if (chargePerNight <= 0 || !ProvidesPower(reactor))
		{
			return;
		}
		foreach (GameItem item in reactor.FindAdjacentType<GameItem>())
		{
			if (item.IsTag("power_source_item"))
			{
				PowerHelper.ChargeBattery(item, chargePerNight);
			}
			else if (item.IsTag("MACHINE_RECHARGABLE_TAG"))
			{
				MachineHelper.RechargeMachineBattery(item, chargePerNight);
			}
		}
	}

	public static bool CanRepair(GameItem target)
	{
		if (IsReactor(target))
		{
			return GetIntegrity(target) < GetMaxIntegrity(target);
		}
		return false;
	}

	public static void RepairWithScrap(GameItem reactor, GameItem scrap)
	{
		if (CanRepair(reactor))
		{
			SetIntegrity(reactor, Mathf.Min(GetMaxIntegrity(reactor), GetIntegrity(reactor) + 60));
			scrap.Destroy();
			OnRepaired();
		}
	}

	public static void RepairWithJunk(GameItem reactor, GameItem junk)
	{
		if (CanRepair(reactor))
		{
			int max = Mathf.Max(0, GetMaxIntegrity(reactor) - 5);
			reactor.ModifyTag("REACTOR_MAX_INTEGRITY_INT", delegate(TagState state)
			{
				state.Enable().SetInt(max);
			});
			SetIntegrity(reactor, Mathf.Min(max, GetIntegrity(reactor) + 30));
			junk.Destroy();
			OnRepaired();
		}
	}

	public static bool CanUpgrade(GameItem target)
	{
		if (IsReactor(target))
		{
			return GetUpgrades(target) < 18;
		}
		return false;
	}

	public static void Upgrade(GameItem reactor, GameItem electronic)
	{
		if (CanUpgrade(reactor))
		{
			int upgrades = GetUpgrades(reactor) + 1;
			reactor.ModifyTag("REACTOR_UPGRADES_INT", delegate(TagState state)
			{
				state.Enable().SetInt(upgrades);
			});
			StartingPerkContent.ConsumeUpgradeMaterial(electronic);
			EmporiumAudioPlayer.Instance.ratchet.Play();
		}
	}

	private static void OnRepaired()
	{
		EmporiumAudioPlayer.Instance.ratchet.Play();
		if (StartingPerk.IsMakeshiftReactor())
		{
			PlayerStore.Instance.UpdatePowerState();
			PlayerStore.Instance.LoadPower();
		}
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (IsReactor(item))
		{
			int integrity = GetIntegrity(item);
			RenderHandler.ColorPalette color = ((integrity <= 25) ? RenderHandler.ColorPalette.LightRed : ((integrity <= 50) ? RenderHandler.ColorPalette.LightOrange : RenderHandler.ColorPalette.LightGreen));
			builder.AddLine(" ");
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_reactor_tooltip_integrity", integrity, GetMaxIntegrity(item)), ignoreEmpty: true, color);
			if (!ProvidesPower(item))
			{
				builder.AddLine(NoPowerText(), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			}
			if (WillExplodeNextNight(item))
			{
				builder.AddLine(CriticalText(), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_reactor_tooltip_decay", GetDecay(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_reactor_tooltip_upgrades", GetUpgrades(item), 18), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			int chargePerNight = GetChargePerNight(item);
			if (chargePerNight > 0)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_reactor_tooltip_charge", chargePerNight), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
	}
}

using UnityEngine;

public class CurrentConverterHelper
{
	public const string CURRENT_CONVERTER_TAG = "CURRENT_CONVERTER_TAG";

	public const string CURRENT_CONVERTER_ON_BOOL = "CURRENT_CONVERTER_ON_BOOL";

	public const string CURRENT_CONVERTER_RATE_FLOAT = "CURRENT_CONVERTER_RATE_FLOAT";

	public const float DEFAULT_RATE = 1.5f;

	public const string CURRENT_CONVERTER_UPGRADE_COUNT_INT = "CURRENT_CONVERTER_UPGRADE_COUNT_INT";

	public const int MAX_UPGRADES = 5;

	public const float UPGRADE_RATE_REDUCTION = 0.1f;

	public const string SPRITE_SHEET = "Items/items_tool2";

	public const string SPRITE_ON = "current_converter";

	public const string SPRITE_OFF = "current_converter_off";

	public static bool isNightCycle;

	public const string IDENTIFIER = "current_converter";

	public static int GetJacksonStockCount()
	{
		int num = 1;
		if (NetworkUpgrade.IsJackson1Unlocked())
		{
			num++;
		}
		if (NetworkUpgrade.IsJackson2Unlocked())
		{
			num++;
		}
		return num;
	}

	public static void InitCurrentConverter(GameItem item, float rate = 1.5f)
	{
		PowerHelper.InitPowerSourceItem(item, 0, 0);
		item.RemoveGameItemType("ENERGY_CELL");
		item.SetGameItemType("TOOL");
		item.soundDragStart = "small_machine_drag";
		item.soundDragEnd = "small_machine_drop";
		item.EnableTag("CURRENT_CONVERTER_TAG");
		item.ModifyTag("CURRENT_CONVERTER_ON_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(val: true);
		});
		item.ModifyTag("CURRENT_CONVERTER_RATE_FLOAT", delegate(TagState state)
		{
			state.Enable().SetFloat(rate);
		});
		item.ModifyTag("CURRENT_CONVERTER_UPGRADE_COUNT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
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
		item.mayToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.canToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.onToggleSlotItemFunc = delegate(GameItem self, GameInventory _, SlotMarker _)
		{
			SetOn(self, !IsOn(self));
			GetHostMachine(self)?.Validate();
			AudioManager.Instance.Play3("click_vintage");
		};
		RefreshSprite(item);
	}

	public static bool IsCurrentConverter(GameItem item)
	{
		return item?.IsTag("CURRENT_CONVERTER_TAG") ?? false;
	}

	public static bool IsOn(GameItem converter)
	{
		return converter.GetTagReadonly("CURRENT_CONVERTER_ON_BOOL").GetBool();
	}

	public static void SetOn(GameItem converter, bool on)
	{
		converter.ModifyTag("CURRENT_CONVERTER_ON_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(on);
		});
		RefreshSprite(converter);
	}

	public static void RefreshSprite(GameItem converter)
	{
		converter.SetSpriteQuick("Items/items_tool2", IsOn(converter) ? "current_converter" : "current_converter_off");
	}

	public static void OnLoad(GameItem item)
	{
		if (IsCurrentConverter(item))
		{
			item.RemoveGameItemType("ENERGY_CELL");
			item.SetGameItemType("TOOL");
			RefreshSprite(item);
		}
	}

	public static float GetRate(GameItem converter)
	{
		return GetStoredRate(converter) * (1f - (float)GetNetworkDiscountPercent() / 100f);
	}

	private static float GetStoredRate(GameItem converter)
	{
		float @float = converter.GetTagReadonly("CURRENT_CONVERTER_RATE_FLOAT").GetFloat();
		if (!(@float > 0f))
		{
			return 1.5f;
		}
		return @float;
	}

	public static int GetNetworkDiscountPercent()
	{
		return NetworkUpgrade.GetPowerHijackTier() switch
		{
			3 => 50, 
			2 => 25, 
			1 => 15, 
			_ => 0, 
		};
	}

	public static int GetUpgradeCount(GameItem converter)
	{
		return converter.GetTagReadonly("CURRENT_CONVERTER_UPGRADE_COUNT_INT").GetInt();
	}

	public static bool CanUpgradeEfficiency(GameItem converter)
	{
		if (IsCurrentConverter(converter))
		{
			return GetUpgradeCount(converter) < 5;
		}
		return false;
	}

	public static void UpgradeEfficiency(GameItem converter, GameItem upgradeItem)
	{
		if (CanUpgradeEfficiency(converter))
		{
			int count = GetUpgradeCount(converter);
			converter.ModifyTag("CURRENT_CONVERTER_UPGRADE_COUNT_INT", delegate(TagState state)
			{
				state.Enable().SetInt(count + 1);
			});
			float newRate = Mathf.Max(0f, GetStoredRate(converter) - 0.1f);
			converter.ModifyTag("CURRENT_CONVERTER_RATE_FLOAT", delegate(TagState state)
			{
				state.Enable().SetFloat(newRate);
			});
			converter.unitValue += upgradeItem.unitValue;
			StartingPerkContent.ConsumeUpgradeMaterial(upgradeItem);
			EmporiumAudioPlayer.Instance.ratchet.Play();
		}
	}

	public static int ToCredits(int energy, float rate)
	{
		return Mathf.CeilToInt((float)energy * rate);
	}

	public static GameItem GetHostMachine(GameItem converter)
	{
		GameItem gameItem = converter?.parentInventory?.GetParentItem();
		if (gameItem == null || !gameItem.IsTag("machinery_drawn_power"))
		{
			return null;
		}
		return gameItem;
	}

	public static int GetHostUsage(GameItem host)
	{
		return host.GetTagReadonly("machinery_drawn_power").GetInt();
	}

	public static GameItem FindConverter(GameItem machine)
	{
		if (machine == null)
		{
			return null;
		}
		foreach (GameItem item in machine.FindAllChildItems())
		{
			if (IsCurrentConverter(item))
			{
				return item;
			}
		}
		return null;
	}

	public static bool IsBlackedOut()
	{
		StoreEventManager storeEventManager = StoreStation.Instance?.storeEventManager;
		if (storeEventManager != null && storeEventManager.IsBlackoutEventActive())
		{
			return storeEventManager.IsPlayerAffectedByBlackout();
		}
		return false;
	}

	public static bool IsServiceActive()
	{
		return StoreService.GetCurrentConverterService()?.IsActive() ?? false;
	}

	public static bool CanSupply(GameItem converter, int amount)
	{
		if (!IsOn(converter))
		{
			return false;
		}
		if (IsBlackedOut())
		{
			return false;
		}
		if (isNightCycle)
		{
			return IsServiceActive();
		}
		return PlayerStore.Instance.CanPay(ToCredits(amount, GetRate(converter)));
	}

	public static bool Supply(GameItem converter, int amount)
	{
		if (!CanSupply(converter, amount))
		{
			return false;
		}
		if (!isNightCycle)
		{
			PlayerStore.Instance.playerCash -= ToCredits(amount, GetRate(converter));
		}
		return true;
	}

	public static bool CanHostDraw(GameItem machine)
	{
		GameItem gameItem = FindConverter(machine);
		if (gameItem == null)
		{
			return true;
		}
		return CanSupply(gameItem, GetHostUsage(machine));
	}

	public static void UpdateServiceCost(StoreService service)
	{
		bool flag = false;
		float num = 0f;
		foreach (GameItem item in PlayerStore.Instance.FindAllItem())
		{
			if (!IsCurrentConverter(item) || item.IsTag("ITEM_BEHIND_LOCK_TAG"))
			{
				continue;
			}
			if (GeneralHelper.IsItemOwned(item))
			{
				flag = true;
			}
			if (IsOn(item))
			{
				GameItem hostMachine = GetHostMachine(item);
				if (hostMachine != null)
				{
					num += (float)GetHostUsage(hostMachine) * GetRate(item);
				}
			}
		}
		if (flag && !service.introduced)
		{
			service.introduced = true;
			service.isUsing = true;
		}
		service.unlocked = service.introduced;
		service.cost = ((!IsBlackedOut()) ? Mathf.CeilToInt(num) : 0);
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (IsCurrentConverter(item))
		{
			bool flag = IsOn(item);
			builder.AddLine(flag ? LocHelper.GetLocalizedMechanic("mech_current_converter_status_on") : LocHelper.GetLocalizedMechanic("mech_current_converter_status_off"), ignoreEmpty: true, flag ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightOrange);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_current_converter_rate", GetRate(item).ToString("0.00")), ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			GameItem hostMachine = GetHostMachine(item);
			if (hostMachine != null)
			{
				int hostUsage = GetHostUsage(hostMachine);
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_current_converter_connected", hostMachine.name, hostUsage), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_current_converter_nightly_cost", ToCredits(hostUsage, GetRate(item))), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			builder.AddLine(" ");
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_fridge_tooltip_upgrade_header"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_fridge_tooltip_upgrade_efficiency", LocHelper.GetLocalizedItem("item_common_electronic_name"), GetUpgradeCount(item), 5), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
	}
}

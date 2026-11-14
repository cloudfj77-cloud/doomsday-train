using System;
using System.Collections.Generic;

public class MachineFridge
{
	public const string FRIDGE_TAG = "FRIDGE_TAG";

	public const string CHILLED_FEATURE_ID = "chilled";

	public const string CHILLED_KEPT_COLD_TAG = "CHILLED_KEPT_COLD_TAG";

	public const int FRIDGE_POWER_USE = 2;

	public const int CHILLED_VALUE_BONUS = 20;

	public const string FROZEN_FEATURE_ID = "frozen";

	public const string LEGACY_FROZEN_DESSERT_FEATURE_ID = "frozenDessert";

	public const string FROZEN_KEPT_FROZEN_TAG = "FROZEN_KEPT_FROZEN_TAG";

	public const int FROZEN_VALUE_BONUS = 25;

	public const string ICE_COLD_FEATURE_ID = "iceCold";

	public const string ICE_COLD_KEPT_TAG = "ICE_COLD_KEPT_TAG";

	public const int ICE_COLD_VALUE_BONUS = 25;

	public const string FRIDGE_STORAGE_UPGRADE_COUNT_TAG = "FRIDGE_STORAGE_UPGRADE_COUNT_TAG";

	public const string FRIDGE_POWER_UPGRADE_COUNT_TAG = "FRIDGE_POWER_UPGRADE_COUNT_TAG";

	public const int FRIDGE_MAX_UPGRADES = 7;

	public const int STORAGE_UPGRADE_POWER_INCREASE = 2;

	public const int POWER_UPGRADE_REDUCTION = 2;

	public const int FRIDGE_MIN_POWER_USE = 2;

	public const int CHILL_ZONE_WIDTH = 5;

	public const int CHILL_ZONE_BASE_HEIGHT = 2;

	public const int FREEZE_ZONE_WIDTH = 3;

	public const int FREEZE_ZONE_BASE_HEIGHT = 2;

	public const int CHILL_TARGET_TEMP = 3;

	public const int FREEZE_TARGET_TEMP = -18;

	public const double CHILL_DEGREES_PER_SECOND = 0.5;

	public const double FREEZE_DEGREES_PER_SECOND = 1.0;

	public static GameItem Fridge()
	{
		(PixelWindow, GameSlotInventory, GameGridInventory, GameGridInventory) tuple = CreateFridgeWindow();
		PixelWindow item2 = tuple.Item1;
		GameSlotInventory batterySlot = tuple.Item2;
		GameGridInventory chillZone = tuple.Item3;
		GameGridInventory freezeZone = tuple.Item4;
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(item2).SetSpriteAndShape("Items/items_ship", "fridge");
		machine.name = LocHelper.GetLocalizedItem("item_fridge_name");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_fridge_desc");
		machine.unitValue = 100L;
		machine.SetGameItemType("MACHINE");
		machine.EnableTag("FRIDGE_TAG");
		machine.EnableTag("CONTAINER_TAG");
		machine.EnableTag("TURBO_BOOST_ELIGIBLE_TAG");
		machine.ModifyTag("FRIDGE_STORAGE_UPGRADE_COUNT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		machine.ModifyTag("FRIDGE_POWER_UPGRADE_COUNT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		AudioHelper.InitMetalMachine(machine);
		MachineHelper.SetupBatterySlot(batterySlot, machine);
		MachineryHelper.InitMachinery(machine, 2);
		chillZone.mayInventoryAddItemFunc = (GameItem fi, GameInventory _) => MayZoneAcceptItem(fi);
		freezeZone.mayInventoryAddItemFunc = (GameItem fi, GameInventory _) => MayZoneAcceptItem(fi);
		machine.onCycleEndSlotItemFunc = delegate
		{
			List<GameItem> list = new List<GameItem>();
			list.AddRange(chillZone.childItems);
			list.AddRange(freezeZone.childItems);
			if (list.Count == 0 || !MachineHelper.TryDrawCyclePower(machine, batterySlot))
			{
				return;
			}
			foreach (GameItem item3 in list)
			{
				if (IsChillable(item3))
				{
					ChillItem(item3);
				}
			}
			foreach (GameItem childItem in chillZone.childItems)
			{
				if (IsSpirit(childItem))
				{
					ChillItem(childItem);
				}
			}
			foreach (GameItem childItem2 in freezeZone.childItems)
			{
				if (IsFreezable(childItem2))
				{
					FreezeItem(childItem2);
				}
				else if (IsSpirit(childItem2))
				{
					IceColdItem(childItem2);
				}
			}
		};
		machine.onOneSecondItemFunc = delegate
		{
			if (PowerHelper.CanDrawPowerSource(batterySlot.childItem, 1))
			{
				CoolZoneContainers(chillZone, 3.0, 0.5);
				CoolZoneContainers(freezeZone, -18.0, 1.0);
			}
		};
		machine.onLoaded = delegate(GameItem item)
		{
			ApplyStorageSize(item);
			UpdateFridgePowerUsage(item);
		};
		return machine;
	}

	public static GameGridInventory GetChillZone(GameItem fridge)
	{
		return MachineHelper.GetBodyStorage(fridge, 1) as GameGridInventory;
	}

	public static GameGridInventory GetFreezeZone(GameItem fridge)
	{
		return MachineHelper.GetBodyStorage(fridge, 2) as GameGridInventory;
	}

	public static bool CanUpgradeStorage(GameItem fridge)
	{
		if (fridge == null || !fridge.IsTag("FRIDGE_TAG"))
		{
			return false;
		}
		return fridge.GetTagReadonly("FRIDGE_STORAGE_UPGRADE_COUNT_TAG").GetInt() < 7;
	}

	public static bool CanUpgradePowerEfficiency(GameItem fridge)
	{
		if (fridge == null || !fridge.IsTag("FRIDGE_TAG"))
		{
			return false;
		}
		return fridge.GetTagReadonly("FRIDGE_POWER_UPGRADE_COUNT_TAG").GetInt() < 7;
	}

	public static void UpgradeStorage(GameItem fridge, GameItem upgradeItem)
	{
		if (CanUpgradeStorage(fridge))
		{
			int count = fridge.GetTagReadonly("FRIDGE_STORAGE_UPGRADE_COUNT_TAG").GetInt();
			fridge.ModifyTag("FRIDGE_STORAGE_UPGRADE_COUNT_TAG", delegate(TagState state)
			{
				state.SetInt(count + 1);
			});
			ApplyStorageSize(fridge);
			UpdateFridgePowerUsage(fridge);
			fridge.unitValue += upgradeItem.unitValue;
			StartingPerkContent.ConsumeUpgradeMaterial(upgradeItem);
			EmporiumAudioPlayer.Instance.ratchet.Play();
		}
	}

	public static void UpgradePowerEfficiency(GameItem fridge, GameItem upgradeItem)
	{
		if (fridge != null && fridge.IsTag("FRIDGE_TAG") && CanUpgradePowerEfficiency(fridge))
		{
			int count = fridge.GetTagReadonly("FRIDGE_POWER_UPGRADE_COUNT_TAG").GetInt();
			fridge.ModifyTag("FRIDGE_POWER_UPGRADE_COUNT_TAG", delegate(TagState state)
			{
				state.SetInt(count + 1);
			});
			UpdateFridgePowerUsage(fridge);
			fridge.unitValue += upgradeItem.unitValue;
			StartingPerkContent.ConsumeUpgradeMaterial(upgradeItem);
			EmporiumAudioPlayer.Instance.ratchet.Play();
		}
	}

	public static void ApplyStorageSize(GameItem fridge)
	{
		int @int = fridge.GetTagReadonly("FRIDGE_STORAGE_UPGRADE_COUNT_TAG").GetInt();
		GameGridInventory chillZone = GetChillZone(fridge);
		GameGridInventory freezeZone = GetFreezeZone(fridge);
		chillZone.SetShape(5, 2 + @int);
		freezeZone.SetShape(3, 2 + @int);
		chillZone.Validate();
		freezeZone.Validate();
		fridge.contentWindow.Validate();
		fridge.Validate();
	}

	public static void UpdateFridgePowerUsage(GameItem fridge)
	{
		int @int = fridge.GetTagReadonly("FRIDGE_STORAGE_UPGRADE_COUNT_TAG").GetInt();
		int int2 = fridge.GetTagReadonly("FRIDGE_POWER_UPGRADE_COUNT_TAG").GetInt();
		int total = 2 + @int * 2 - int2 * 2;
		if (total < 2)
		{
			total = 2;
		}
		fridge.ModifyTag("machinery_drawn_power", delegate(TagState state)
		{
			state.Enable().SetInt(total);
		});
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("FRIDGE_TAG"))
		{
			int @int = item.GetTagReadonly("FRIDGE_STORAGE_UPGRADE_COUNT_TAG").GetInt();
			int int2 = item.GetTagReadonly("FRIDGE_POWER_UPGRADE_COUNT_TAG").GetInt();
			builder.AddLine(" ");
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_fridge_tooltip_upgrade_header"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_fridge_tooltip_upgrade_storage", LocHelper.GetLocalizedItem("item_scrap_metal_name"), @int, 7), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_fridge_tooltip_upgrade_efficiency", LocHelper.GetLocalizedItem("item_common_electronic_name"), int2, 7), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
	}

	private static void CoolZoneContainers(GameGridInventory zone, double targetTemp, double degreesPerSecond)
	{
		foreach (GameItem childItem in zone.childItems)
		{
			if (!childItem.IsTag("LIQUID_CONTAINER_TAG") || WaterHelper.IsEmpty(childItem))
			{
				continue;
			}
			double @double = childItem.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
			double newTemp = ((@double > targetTemp) ? Math.Max(@double - degreesPerSecond, targetTemp) : Math.Min(@double + degreesPerSecond, targetTemp));
			if (newTemp != @double)
			{
				childItem.ModifyTag("CONTAINER_TEMP_TAG", delegate(TagState state)
				{
					state.Enable().SetDouble(newTemp);
				}, skipValidate: true);
			}
			childItem.EnableTag("HEATING_TAG", skipValidate: true);
		}
	}

	public static bool MayZoneAcceptItem(GameItem item)
	{
		if (!GeneralHelper.IsItemOwned(item))
		{
			return false;
		}
		if (item.IsGameItemType("MACHINE"))
		{
			return false;
		}
		if (item.IsGameItemType("STORAGE"))
		{
			return false;
		}
		if (item.identifier.Equals("wine_bottle"))
		{
			return false;
		}
		if (item.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return true;
		}
		if (IsChillable(item))
		{
			return true;
		}
		if (IsFreezable(item))
		{
			return true;
		}
		if (IsSpirit(item))
		{
			return true;
		}
		return false;
	}

	public static bool IsChillable(GameItem item)
	{
		if (!item.IsGameItemType("BREVAGE") && !item.IsGameItemType("WATER_RATION"))
		{
			return item.identifier.Equals("bottled_water_premium");
		}
		return true;
	}

	public static bool IsSpirit(GameItem item)
	{
		return item.identifier.Equals("nudka");
	}

	public static void ChillItem(GameItem item)
	{
		item.DisableTag("ICE_COLD_KEPT_TAG", skipValidate: true);
		item.RemoveItemFeatureByID("iceCold");
		item.EnableTag("CHILLED_KEPT_COLD_TAG");
		item.AddItemFeature(CreateChilledFeature());
		foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(item))
		{
			if (IsChillable(item2))
			{
				item2.EnableTag("CHILLED_KEPT_COLD_TAG");
				item2.AddItemFeature(CreateChilledFeature());
			}
		}
	}

	public static ItemFeature CreateChilledFeature()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Normale);
		itemFeature.identifier = "chilled";
		itemFeature.category = "chilled";
		itemFeature.valueStage = ItemFeature.ValueStage.Innate;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_chilled");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(20);
		itemFeature.isExposable = false;
		return itemFeature;
	}

	public static bool IsFreezable(GameItem item)
	{
		return item.identifier.Equals("beis_icecream");
	}

	public static void FreezeItem(GameItem item)
	{
		item.EnableTag("FROZEN_KEPT_FROZEN_TAG");
		item.AddItemFeature(CreateFrozenFeature());
		foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(item))
		{
			if (IsFreezable(item2))
			{
				item2.EnableTag("FROZEN_KEPT_FROZEN_TAG");
				item2.AddItemFeature(CreateFrozenFeature());
			}
		}
	}

	public static ItemFeature CreateFrozenFeature()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Normale);
		itemFeature.identifier = "frozen";
		itemFeature.category = "frozen";
		itemFeature.valueStage = ItemFeature.ValueStage.Innate;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_frozen");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(25);
		itemFeature.isExposable = false;
		return itemFeature;
	}

	public static void IceColdItem(GameItem item)
	{
		item.DisableTag("CHILLED_KEPT_COLD_TAG", skipValidate: true);
		item.RemoveItemFeatureByID("chilled");
		item.EnableTag("ICE_COLD_KEPT_TAG");
		item.AddItemFeature(CreateIceColdFeature());
	}

	public static ItemFeature CreateIceColdFeature()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Normale);
		itemFeature.identifier = "iceCold";
		itemFeature.category = "iceCold";
		itemFeature.valueStage = ItemFeature.ValueStage.Innate;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_ice_cold");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(25);
		itemFeature.isExposable = false;
		return itemFeature;
	}

	public static void ClearColdMarkers()
	{
		foreach (GameItem item in PlayerStore.Instance.FindAllItem())
		{
			item.DisableTag("CHILLED_KEPT_COLD_TAG", skipValidate: true);
			item.DisableTag("FROZEN_KEPT_FROZEN_TAG", skipValidate: true);
			item.DisableTag("ICE_COLD_KEPT_TAG", skipValidate: true);
		}
	}

	public static void HandleColdFeatureExpiry()
	{
		foreach (GameItem item in PlayerStore.Instance.FindAllItem())
		{
			if (item.IsTag("CHILLED_KEPT_COLD_TAG"))
			{
				item.DisableTag("CHILLED_KEPT_COLD_TAG");
			}
			else if (item.FindItemFeatureByID("chilled") != null)
			{
				item.RemoveItemFeatureByID("chilled");
			}
			if (item.IsTag("FROZEN_KEPT_FROZEN_TAG"))
			{
				item.DisableTag("FROZEN_KEPT_FROZEN_TAG");
			}
			else if (item.FindItemFeatureByID("frozen") != null)
			{
				item.RemoveItemFeatureByID("frozen");
			}
			if (item.IsTag("ICE_COLD_KEPT_TAG"))
			{
				item.DisableTag("ICE_COLD_KEPT_TAG");
			}
			else if (item.FindItemFeatureByID("iceCold") != null)
			{
				item.RemoveItemFeatureByID("iceCold");
			}
			if (item.FindItemFeatureByID("frozenDessert") != null)
			{
				item.RemoveItemFeatureByID("frozenDessert");
			}
		}
	}

	public static (PixelWindow, GameSlotInventory, GameGridInventory, GameGridInventory) CreateFridgeWindow(bool isDraggable = true, bool isCentered = true)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory(5, 2);
		GameGridInventory gameGridInventory2 = new GameGridInventory(3, 2);
		PixelWindow item = MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_fridge_label_chill"), gameGridInventory, "chill"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_fridge_label_freeze"), gameGridInventory2, "freeze"));
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameGridInventory2.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		return (item, gameSlotInventory, gameGridInventory, gameGridInventory2);
	}
}

using System.Collections.Generic;

public class GeneralHelper
{
	public const string COLOR_GREEN = "5c6f46";

	public const string COLOR_RED = "9c3738";

	public const string COLOR_ORANGE = "cc7755";

	public const string WEIGHT_TAG = "WEIGHT_TAG";

	public const string IMPORTANT_TAG = "IMPORTANT_TAG";

	public const string CONFIGURE_TAG = "CONFIGURE_TAG";

	public const string REPO_ELIGIBLE_OVERRIDE_TAG = "REPO_ELIGIBLE_OVERRIDE_TAG";

	public const string IS_OWNED_TAG = "IS_OWNED_TAG";

	public const string JUST_SOLD_TAG = "JUST_SOLD_TAG";

	public const string LOCKED_TAG = "LOCKED_TAG";

	public const string IS_UNLOAD_DISABLED_TAG = "IS_UNLOAD_DISABLED_TAG";

	public const string PARENT_ITEM_ID = "PARENT_ITEM_ID";

	public const string CUSTOM_NAME_TAG = "CUSTOM_NAME_TAG";

	public const string CUSTOM_NAME_MODE_TAG = "CUSTOM_NAME_MODE_TAG";

	public const string IS_ITEM_TOGGLE_ON_TAG = "IS_ITEM_TOGGLE_ON_TAG";

	public const string IS_TOOLTIP_DISABLED_TAG = "IS_TOOLTIP_DISABLED_TAG";

	public const string CAN_USE_ON_NON_OWNED_TAG = "CAN_USE_ON_NON_OWNED_TAG";

	public const string HAZARDOUS_WASTE_TAG = "HAZARDOUS_WASTE_TAG";

	private static readonly HashSet<string> nonOwnedCapableToolIds = new HashSet<string> { "magnifier", "xray_scanner", "chem_scanner", "water_test_strip", "aquascan", "plant_analyser", "gun_part" };

	private static readonly HashSet<string> nonOwnedTagRevokedIds = new HashSet<string> { "uv_filter" };

	private static readonly HashSet<string> hazardousWasteIds = new HashSet<string> { "nuclear_waste" };

	private static readonly HashSet<string> importantStoryItemIds = new HashSet<string> { "luxury_supply_crate", "self_refilling_neuroactive_perfume" };

	public const string DISPLAY_ONLY_TAG = "DISPLAY_ONLY_TAG";

	public static bool IsHazardousWaste(GameItem item)
	{
		return item?.IsTag("HAZARDOUS_WASTE_TAG") ?? false;
	}

	public static void LoadHazardousWasteTag(GameItem item)
	{
		if (hazardousWasteIds.Contains(item.identifier))
		{
			item.EnableTag("HAZARDOUS_WASTE_TAG");
		}
	}

	public static void LoadImportantStoryItemTag(GameItem item)
	{
		if (importantStoryItemIds.Contains(item.identifier))
		{
			item.EnableTag("IMPORTANT_TAG");
		}
	}

	public static void LoadNonOwnedToolTag(GameItem item)
	{
		if (nonOwnedCapableToolIds.Contains(item.identifier))
		{
			item.EnableTag("CAN_USE_ON_NON_OWNED_TAG");
		}
		else if (nonOwnedTagRevokedIds.Contains(item.identifier))
		{
			item.DisableTag("CAN_USE_ON_NON_OWNED_TAG");
		}
	}

	public static void SetParentItem(GameItem vendingMachineCore, GameItem parentItem)
	{
		vendingMachineCore.ModifyTag("PARENT_ITEM_ID", delegate(TagState state)
		{
			state.SetInt(parentItem.uniqueId);
		});
	}

	public static GameItem GetParentItem(GameItem vendingMachineCore)
	{
		return EmporiumEntry.Instance.FindItemByUniqueID(vendingMachineCore.GetTagReadonly("PARENT_ITEM_ID").GetInt());
	}

	public static void SetDisplayOnly(GameItem gameItem)
	{
		gameItem.EnableTag("DISPLAY_ONLY_TAG");
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
	}

	public static bool IsDisplayOnly(GameItem gameItem)
	{
		return gameItem?.IsTag("DISPLAY_ONLY_TAG") ?? false;
	}

	public static void LockItem(GameItem gameItem)
	{
		gameItem.EnableTag("LOCKED_TAG");
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
	}

	public static void UnlockItem(GameItem gameItem)
	{
		gameItem.DisableTag("LOCKED_TAG");
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => true;
	}

	public static bool IsItemLocked(GameItem gameItem)
	{
		return gameItem.IsTag("LOCKED_TAG");
	}

	public static void LoadLockItem(GameItem gameItem)
	{
		if (gameItem.IsTag("LOCKED_TAG"))
		{
			gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		}
	}

	public static void SetItemWeight(GameItem item, int weight)
	{
		item.ModifyTag("WEIGHT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(weight);
		});
		if (item.GetIndexByCategory("weight") != -1)
		{
			PriceHelper.HandleWeightPrice(item, weight);
		}
		else
		{
			InitWeight(item, weight);
		}
	}

	public static void SetItemWeightFake(GameItem item, int fakeWeight, int actualWeight)
	{
		item.ModifyTag("WEIGHT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(actualWeight);
		});
		if (item.GetIndexByCategory("weight") != -1)
		{
			PriceHelper.HandleWeightPrice(item, fakeWeight);
		}
		else
		{
			InitWeightFake(item, fakeWeight, actualWeight);
		}
	}

	public static void InitWeight(GameItem item, int weight)
	{
		item.ModifyTag("WEIGHT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(weight);
		});
		PriceHelper.HandleWeightPrice(item, weight);
	}

	public static void InitWeightFake(GameItem item, int fakeWeight, int actualWeight)
	{
		item.ModifyTag("WEIGHT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(actualWeight);
		});
		PriceHelper.HandleWeightPrice(item, fakeWeight);
	}

	public static int GetWeight(GameItem item)
	{
		return item.GetTagReadonly("WEIGHT_TAG").valueInt;
	}

	public static void SetItemOwned(GameItem item, bool own = true)
	{
		if (own)
		{
			item.EnableTag("IS_OWNED_TAG");
		}
		else
		{
			item.DisableTag("IS_OWNED_TAG");
		}
	}

	public static bool IsItemOwned(GameItem item)
	{
		return item.IsTag("IS_OWNED_TAG");
	}

	public static bool IsItemOwned(GameItem item1, GameItem item2)
	{
		if (IsItemOwned(item1))
		{
			return IsItemOwned(item2);
		}
		return false;
	}

	public static bool IsItemEffectivelyOwned(GameItem item)
	{
		if (!IsItemOwned(item))
		{
			return false;
		}
		for (GameItem gameItem = ((item.parentInventory == null) ? null : GraphUtils.FindParentType<GameItem>(item.parentInventory)); gameItem != null; gameItem = ((gameItem.parentInventory == null) ? null : GraphUtils.FindParentType<GameItem>(gameItem.parentInventory)))
		{
			if (!IsItemOwned(gameItem))
			{
				return false;
			}
		}
		return true;
	}

	public static void CreateGeneralItemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!IsItemEffectivelyOwned(item))
		{
			builder.Add(LocHelper.GetLocalizedMechanic("mech_tooltip_not_owned"), RenderHandler.ColorPalette.LightRed);
		}
	}

	public static void TrackBuyPrice(GameItem gameItem, int boughtPrice)
	{
		gameItem.EnableTag("BOUGHT_PRICE_TAG");
		gameItem.ModifyTag("BOUGHT_PRICE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(boughtPrice);
		});
	}

	public static string CreditIcon(bool spaced = false)
	{
		if (spaced)
		{
			return " <sprite name=\"c\" tint=1 >";
		}
		return "<sprite name=\"c\" tint=1 >";
	}

	public static void DestroyGameItems(List<GameItem> items)
	{
		if (items != null && items.Count != 0)
		{
			for (int num = items.Count - 1; num >= 0; num--)
			{
				items[num]?.Destroy();
			}
		}
	}

	public static bool CanAddItemToInv(GameItem gameItem, GameItem invItem)
	{
		if (invItem == null)
		{
			return true;
		}
		return IsItemOwned(gameItem) == IsItemOwned(invItem);
	}

	public static bool MayInteractNonOwnedActivate(GameItem item)
	{
		if (item.IsTag("DOCUMENT"))
		{
			return !item.IsGameItemType("LOTTERY");
		}
		return false;
	}

	public static bool MayPlayerInsertInto(GameItem containerItem)
	{
		if (containerItem == null)
		{
			return true;
		}
		if (!IsItemOwned(containerItem))
		{
			return GunHelper.IsServiceGun(containerItem);
		}
		return true;
	}

	public static bool MayPlayerInsertInto(GameInventory targetInventory)
	{
		if (targetInventory == null)
		{
			return true;
		}
		return MayPlayerInsertInto(GraphUtils.FindParentType<GameItem>(targetInventory));
	}

	public static bool MayOwnershipAcceptItem(GameItem item, GameInventory inventory)
	{
		if (!IsItemOwned(item))
		{
			return true;
		}
		GameItem gameItem = GraphUtils.FindParentType<GameItem>(inventory);
		if (gameItem == null)
		{
			return true;
		}
		if (!IsItemOwned(gameItem))
		{
			return GunHelper.IsServiceGun(gameItem);
		}
		return true;
	}

	public static bool IsInsideNonOwnedContainer(GameItem item)
	{
		if (item?.parentInventory == null)
		{
			return false;
		}
		GameItem gameItem = GraphUtils.FindParentType<GameItem>(item.parentInventory);
		if (gameItem == null)
		{
			return false;
		}
		if (GunHelper.IsServiceGun(gameItem))
		{
			return !IsItemOwned(item);
		}
		return !IsItemOwned(gameItem);
	}

	public static string GetDisplayNameFromID(string itemID)
	{
		if (string.IsNullOrEmpty(itemID))
		{
			return "[Invalid ID]";
		}
		if (PlayerStore.Instance.itemDisplayNameCache.TryGetValue(itemID, out var value))
		{
			return value;
		}
		GameItem gameItem = DirectoryMaster.Item(itemID);
		if (gameItem == null)
		{
			return "[Unknown Item]";
		}
		string name = gameItem.name;
		PlayerStore.Instance.itemDisplayNameCache[itemID] = name;
		return name;
	}

	public static void SetCustomName(GameItem item, string customName)
	{
		if (string.IsNullOrEmpty(customName))
		{
			item.DisableTag("CUSTOM_NAME_TAG");
			return;
		}
		item.ModifyTag("CUSTOM_NAME_TAG", delegate(TagState state)
		{
			state.Enable().SetString(customName);
		});
	}

	public static void RemoveCustomName(GameItem item)
	{
		item.ModifyTag("CUSTOM_NAME_TAG", delegate(TagState state)
		{
			state.SetString("");
		});
		item.DisableTag("CUSTOM_NAME_TAG");
	}

	public static string GetCustomName(GameItem item)
	{
		if (!item.IsTag("CUSTOM_NAME_TAG"))
		{
			return null;
		}
		return item.GetTagReadonly("CUSTOM_NAME_TAG").valueString;
	}

	public static bool HasCustomName(GameItem item)
	{
		return item.IsTag("CUSTOM_NAME_TAG");
	}

	public static void SetCustomNameModel(GameItem item, int mode)
	{
		item.ModifyTag("CUSTOM_NAME_MODE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(mode);
		});
	}
}

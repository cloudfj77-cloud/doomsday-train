using System;
using System.Collections.Generic;

public class ContainerHelper
{
	public const string POUCH_TAG = "POUCH_TAG";

	public const string BACKPACK_TAG = "BACKPACK_TAG";

	public const string CONTAINER_TAG = "CONTAINER_TAG";

	public const string TRACK_OPENED_TAG = "TRACK_OPENED_TAG";

	public const string OPENED_TAG = "OPENED_TAG";

	public const string CONTAINER_TOOLTIP_TAG = "CONTAINER_TOOLTIP_TAG";

	public const string SAFE_TAG = "SAFE_TAG";

	public const string ITEM_MEDICAL_TAG = "ITEM_MEDICAL_TAG";

	public const string PRE_CONTAINER_TAG = "PRE_CONTAINER_TAG";

	public const string NESTABLE_DEVICE_TAG = "NESTABLE_DEVICE_TAG";

	public const string STORABLE_PACKAGE_TAG = "STORABLE_PACKAGE_TAG";

	public static void AllowOnlyTaggedItem(GameInventory itemInventory, string tag, bool ownedOnly = false, bool excludeContainer = false)
	{
		itemInventory.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
		{
			if (ownedOnly && !GeneralHelper.IsItemOwned(item))
			{
				return false;
			}
			if (excludeContainer && item.IsTag("CONTAINER_TAG"))
			{
				return false;
			}
			return item.IsTag(tag) ? true : false;
		};
	}

	public static void AllowOnlyTaggedItemOwn(GameInventory itemInventory, GameItem invItem, string tag, bool excludeContainer = false)
	{
		itemInventory.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
		{
			if (excludeContainer && item.IsTag("CONTAINER_TAG"))
			{
				return false;
			}
			return item.IsTag(tag) && GeneralHelper.CanAddItemToInv(item, invItem);
		};
	}

	public static void AllowOnlyTaggedItemsOr(GameInventory itemInventory, string[] tags, bool ownedOnly = false)
	{
		if (Array.Exists(tags, ModuleHelper.IsModuleTypeTag))
		{
			itemInventory.EnableTag("NO_HOPPER_INVENTORY_TAG");
		}
		itemInventory.mayInventoryAddItemFunc = (Func<GameItem, GameInventory, bool>)Delegate.Combine(itemInventory.mayInventoryAddItemFunc, (Func<GameItem, GameInventory, bool>)delegate(GameItem item, GameInventory _)
		{
			if (ownedOnly && !GeneralHelper.IsItemOwned(item))
			{
				return false;
			}
			string[] array = tags;
			foreach (string tagName in array)
			{
				if (item.IsTag(tagName))
				{
					return true;
				}
			}
			return false;
		});
	}

	public static void AllowOnlyTaggedItemsAnd(GameInventory itemInventory, string[] tags)
	{
		itemInventory.mayInventoryAddItemFunc = (Func<GameItem, GameInventory, bool>)Delegate.Combine(itemInventory.mayInventoryAddItemFunc, (Func<GameItem, GameInventory, bool>)delegate(GameItem item, GameInventory _)
		{
			string[] array = tags;
			foreach (string tagName in array)
			{
				if (!item.IsTag(tagName))
				{
					return false;
				}
			}
			return true;
		});
	}

	public static void AllowOnlyOwnedItems(GameInventory itemInventory)
	{
		itemInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) ? true : false;
	}

	public static void AllowOnlyOwnedItemsExcludeContainers(GameInventory itemInventory)
	{
		itemInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => (GeneralHelper.IsItemOwned(item) && !item.IsTag("CONTAINER_TAG")) ? true : false;
	}

	public static void InitPouchItem(GameItem item)
	{
		item.EnableTag("POUCH_TAG");
		item.SetType("POUCH_TAG");
		item.shortDescription = " Can be attached to a Modular Backpack to expand it's storage capacity.";
	}

	public static bool IsNestableDevice(GameItem item)
	{
		return item.IsTag("NESTABLE_DEVICE_TAG");
	}

	public static bool IsStorablePackage(GameItem item)
	{
		if (item.IsTag("STORABLE_PACKAGE_TAG"))
		{
			return !item.IsTag("BACKPACK_TAG");
		}
		return false;
	}

	public static bool IsNestingAllowed(GameItem addedItem, GameItem parentItem)
	{
		if (IsNestableDevice(addedItem))
		{
			return true;
		}
		if (IsStorablePackage(addedItem) && parentItem != null)
		{
			return parentItem.IsGameItemType("STORAGE");
		}
		return false;
	}

	public static void InitContainerItem(GameInventory itemInventory, GameItem item, List<string> allowedTags = null, List<string> allowedId = null)
	{
		item.EnableTag("IMPORTANT_TAG");
		item.EnableTag("CONTAINER_TAG");
		itemInventory.mayInventoryAddItemFunc = (Func<GameItem, GameInventory, bool>)Delegate.Combine(itemInventory.mayInventoryAddItemFunc, (Func<GameItem, GameInventory, bool>)delegate(GameItem addedItem, GameInventory inv)
		{
			if (!GeneralHelper.IsItemOwned(addedItem))
			{
				return false;
			}
			GameItem parentItem = inv.GetParentItem();
			if (parentItem != null && !GeneralHelper.IsItemOwned(parentItem))
			{
				return false;
			}
			if ((addedItem.IsTag("BACKPACK_TAG") || addedItem.IsTag("CONTAINER_TAG")) && !IsNestingAllowed(addedItem, parentItem))
			{
				return false;
			}
			if (addedItem.IsGameItemType("MACHINE") && !IsNestableDevice(addedItem))
			{
				return false;
			}
			if (allowedTags != null && addedItem.IsOneOfTags(allowedTags))
			{
				return true;
			}
			if (allowedId != null && allowedId.Contains(addedItem.identifier))
			{
				return true;
			}
			return allowedTags == null;
		});
	}

	public static bool CanContainerStoredItem(GameItem addedItem, GameInventory inv)
	{
		if (!GeneralHelper.IsItemOwned(addedItem))
		{
			return false;
		}
		GameItem parentItem = inv.GetParentItem();
		if (parentItem != null && !GeneralHelper.IsItemOwned(parentItem))
		{
			return false;
		}
		if ((addedItem.IsTag("BACKPACK_TAG") || addedItem.IsTag("CONTAINER_TAG")) && !IsNestableDevice(addedItem))
		{
			return false;
		}
		if (addedItem.IsGameItemType("MACHINE"))
		{
			return false;
		}
		return true;
	}

	public static void InitSmugglerBay(GameInventory itemInventory, GameItem item)
	{
		item.EnableTag("IMPORTANT_TAG");
		item.EnableTag("CONTAINER_TAG");
		itemInventory.mayInventoryAddItemFunc = (Func<GameItem, GameInventory, bool>)Delegate.Combine(itemInventory.mayInventoryAddItemFunc, (Func<GameItem, GameInventory, bool>)delegate(GameItem addedItem, GameInventory inv)
		{
			if (!GeneralHelper.IsItemOwned(addedItem))
			{
				return false;
			}
			GameItem parentItem = inv.GetParentItem();
			if (parentItem != null && !GeneralHelper.IsItemOwned(parentItem))
			{
				return false;
			}
			if (addedItem.identifier == "makeshift_storage_bay")
			{
				return false;
			}
			if (addedItem.identifier == "storage_bay")
			{
				return false;
			}
			if (addedItem.identifier == "storage_bay_large")
			{
				return false;
			}
			if (addedItem.identifier == "storage_bay_gun")
			{
				return false;
			}
			if (addedItem.identifier == "storage_bay_chem")
			{
				return false;
			}
			if (addedItem.identifier == "hypertech_bay")
			{
				return false;
			}
			if (addedItem.identifier == "machine_bay")
			{
				return false;
			}
			if (addedItem.identifier == "machine_bay_ext")
			{
				return false;
			}
			if (addedItem.identifier == "smuggler_bay")
			{
				return false;
			}
			if (addedItem.identifier == "smuggler_bay_mod")
			{
				return false;
			}
			if (addedItem.identifier == "smuggler_bay_mini")
			{
				return false;
			}
			if (addedItem.IsTag("BACKPACK_TAG"))
			{
				return false;
			}
			return !addedItem.IsTag("CAGE_TAG");
		});
	}

	public static void InitContainerItemIncludeContainer(GameInventory itemInventory, GameItem item)
	{
		item.EnableTag("IMPORTANT_TAG");
		item.EnableTag("CONTAINER_TAG");
		itemInventory.mayInventoryAddItemFunc = (Func<GameItem, GameInventory, bool>)Delegate.Combine(itemInventory.mayInventoryAddItemFunc, (Func<GameItem, GameInventory, bool>)delegate(GameItem addedItem, GameInventory inv)
		{
			if (!GeneralHelper.IsItemOwned(addedItem))
			{
				return false;
			}
			return !addedItem.identifier.Equals("machine_bay");
		});
	}

	public static void InitContainerItemTagOnly(GameInventory itemInventory, GameItem item)
	{
		item.EnableTag("IMPORTANT_TAG");
		item.EnableTag("CONTAINER_TAG");
	}

	public static void InitContainerItemTagOnly(GameItem item)
	{
		item.EnableTag("IMPORTANT_TAG");
		item.EnableTag("CONTAINER_TAG");
	}

	public static void InitBackpackItem(GameInventory itemInventory, GameItem item)
	{
		item.EnableTag("BACKPACK_TAG");
		InitContainerItem(itemInventory, item);
		item.SetType("BACKPACK_TAG");
		item.EnableTag("IMPORTANT_TAG");
	}

	public static void InitModularBackpack(GameItem item)
	{
		item.shortDescription += " This backpack allows attaching additional Modular Pouches to expand its storage capacity.";
	}

	public static void CreateArmorItemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("BACKPACK_TAG"))
		{
			if (item.isCombatBackpack)
			{
				builder.AddLine(" +Satchel can be accessed during combat", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			else
			{
				builder.AddLine(" -Backpack cannot be accessed during combat", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
		}
	}

	public static void SetBasicContainerValueChangeRule(GameItem box, GameInventory itemInventory)
	{
		itemInventory.onSlotAddItemEndFunc = delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			box.unitValue += item.unitValue;
		};
		itemInventory.onSlotRemoveItemEndFunc = delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			box.unitValue -= item.unitValue;
		};
	}

	public static void SetLateContainerValueChangeRule(GameItem box, GameInventory itemInventory)
	{
		itemInventory.onSlotAddItemEndFunc = delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			box.lateUnitValue += item.unitValue;
		};
		itemInventory.onSlotRemoveItemEndFunc = delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			box.lateUnitValue -= item.unitValue;
		};
	}

	public static void CreateContainerTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("SCRATCHER_TAG") && (item.IsTag("CONTAINER_TAG") || item.IsTag("CONTAINER_TOOLTIP_TAG")))
		{
			builder.AddLine(" " + LocHelper.GetLocalizedMechanic("mech_container_tooltip"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
		}
	}

	public static void AddContainerTooltipTag(GameItem item)
	{
		item.EnableTag("CONTAINER_TOOLTIP_TAG");
	}

	public static void RemoveContainerTooltipTag(GameItem item)
	{
		item.DisableTag("CONTAINER_TOOLTIP_TAG");
	}
}

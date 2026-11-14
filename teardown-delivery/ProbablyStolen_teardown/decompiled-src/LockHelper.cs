public class LockHelper
{
	public const string LOCKED_CONTAINER_TAG = "LOCKED_CONTAINER_TAG";

	public const string LOCK_ID = "LOCK_ID";

	public const string LOCK_STATUS = "LOCK_STATUS";

	public const string LOCK_ID_SEC = "LOCK_ID_SEC";

	public const string LOCK_ID_COMMAND = "LOCK_ID_COMMAND";

	public const string LOCK_ID_MED = "LOCK_ID_MED";

	public const string LOCK_ID_SUP = "LOCK_ID_SUP";

	public const string LOCK_ID_SER = "LOCK_ID_SER";

	public const string LOCK_ID_SCI = "LOCK_ID_SCI";

	public const string LOCK_ID_ENG = "LOCK_ID_ENG";

	public const string ITEM_BEHIND_LOCK_TAG = "ITEM_BEHIND_LOCK_TAG";

	public static void InitLockedContainerItem(GameItem gameItem, string lockId, GameInventory gameInventory)
	{
		gameItem.EnableTag("LOCKED_CONTAINER_TAG");
		gameItem.ModifyTag("LOCK_ID", delegate(TagState state)
		{
			state.Enable().SetString(lockId);
		});
		gameItem.EnableTag("CONTAINER_TAG");
		gameItem.ModifyTag("LOCK_STATUS", delegate(TagState state)
		{
			state.Enable().SetBool(val: true);
		});
		gameItem.SetGameItemType("SUPPLY_CRATE");
		MetaHelper.SetIgnoreChildValue(gameItem);
	}

	public static void CreateLockerContainerTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("LOCKED_CONTAINER_TAG") && item.GetTagReadonly("LOCK_STATUS").GetBool())
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_lock_locked"), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
		}
	}

	public static void LockUpContainer(GameItem gameItem)
	{
		if (gameItem.contentWindow != null)
		{
			SetChildBehindLock(gameItem);
			gameItem.contentWindow.Hide();
			gameItem.contentWindow.isLocked = true;
			((GameGridInventory)gameItem.contentWindow.child).mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		}
	}

	public static void LoadLockedContainer(GameItem gameItem)
	{
		if (gameItem.IsTag("LOCKED_CONTAINER_TAG"))
		{
			if (gameItem.GetTagReadonly("LOCK_STATUS").GetBool())
			{
				LockUpContainer(gameItem);
			}
			else
			{
				UnlockContainer(gameItem, force: true);
			}
		}
	}

	public static void UnlockContainer(GameItem item, bool force = false)
	{
		if (!item.IsTag("LOCKED_CONTAINER_TAG") || !(item.GetTagReadonly("LOCK_STATUS").GetBool() || force))
		{
			return;
		}
		item.ModifyTag("LOCK_STATUS", delegate(TagState state)
		{
			state.Enable().SetBool(val: false);
		});
		if (item.IsTag("SUPPLY_CRATE"))
		{
			item.SetSpriteAndShape(item.spriteAtlasPath, item.spritePath + "_u");
		}
		item.RemoveGameItemType("SUPPLY_CRATE");
		item.EnableTag("IMPORTANT_TAG");
		item.onUnlockedFunc?.Invoke(item);
		if (item.contentWindow == null)
		{
			return;
		}
		if (!force)
		{
			PreBuiltItemHelper.TryRefillForBetterLuck(item);
		}
		PreBuiltItemHelper.MarkCrateScanned(item);
		DisableChildBehindLock(item);
		item.unitValue = 15L;
		ContrabandHelper.RemoveContrabandStatus(item);
		StolenHelper.RemoveStolenStatus(item);
		MetaHelper.RemoveIgnoreChildValue(item);
		item.name = item.name.Replace("(" + LocHelper.GetLocalizedMechanic("mech_lock_sealed") + ")", "").Trim();
		item.contentWindow.isLocked = false;
		if (item.contentWindow.child == null || !(item.contentWindow.child is GameInventory gameInventory))
		{
			return;
		}
		gameInventory.mayInventoryAddItemFunc = delegate(GameItem addedItem, GameInventory inv)
		{
			if (!GeneralHelper.IsItemOwned(addedItem))
			{
				return false;
			}
			if ((addedItem.IsTag("BACKPACK_TAG") || addedItem.IsTag("CONTAINER_TAG") || addedItem.IsTag("LOCKED_CONTAINER_TAG")) && !ContainerHelper.IsStorablePackage(addedItem))
			{
				return false;
			}
			return !addedItem.IsTag("STORAGE");
		};
	}

	public static bool IsLockedContainer(GameItem item)
	{
		if (GeneralHelper.IsItemOwned(item) && item.IsTag("LOCKED_CONTAINER_TAG"))
		{
			return true;
		}
		return false;
	}

	public static bool IsCurrentlyLocked(GameItem item)
	{
		if (!IsLockedContainer(item))
		{
			return false;
		}
		return item.GetTagReadonly("LOCK_STATUS").GetBool();
	}

	public static string GetLock(GameItem item)
	{
		if (item.IsTag("LOCKED_CONTAINER_TAG"))
		{
			return item.GetTagReadonly("LOCK_ID").GetString();
		}
		return "";
	}

	public static bool CanOpenLock(GameItem box, string keyLockId)
	{
		if (keyLockId.Equals("LOCK_ID_COMMAND"))
		{
			return true;
		}
		if (GetLock(box).Equals(keyLockId))
		{
			return true;
		}
		return false;
	}

	public static void TryOpenLock(GameItem key, GameItem box, string keyLockId)
	{
		if (IsCurrentlyLocked(box))
		{
			if (CanOpenLock(box, keyLockId))
			{
				EmporiumAudioPlayer.Instance.audioClickClick.Play();
				key.Destroy();
				UnlockContainer(box);
			}
			else
			{
				EmporiumAudioPlayer.Instance.audioNegativeTwoBeep.Play();
			}
		}
	}

	public static void SetChildBehindLock(GameItem gameItem)
	{
		foreach (GameItem childItem in ((GameInventory)gameItem.contentWindow.children[0]).childItems)
		{
			childItem.EnableTag("ITEM_BEHIND_LOCK_TAG");
		}
	}

	public static void DisableChildBehindLock(GameItem gameItem)
	{
		foreach (GameItem childItem in ((GameInventory)gameItem.contentWindow.children[0]).childItems)
		{
			childItem.DisableTag("ITEM_BEHIND_LOCK_TAG");
		}
	}
}

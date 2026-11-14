using System;

public class SalvageHelper
{
	public const string SALVAGABLE_ITEM_TAG = "SALVAGABLE_ITEM_TAG";

	public const string DESTROY_AFTER_SALVAGE_BOOL = "DESTROY_AFTER_SALVAGE_BOOL";

	public const string POST_SALVAGE_ITEM_ID_STRING = "POST_SALVAGE_ITEM_ID_STRING";

	public const string SALVAGE_TOOL_TAG = "SALVAGE_TOOL_TAG";

	public const string WELDING_QUALITY_INT = "WELDING_QUALITY_INT";

	public static void InitSalvagableItem(GameItem item, bool destroy_after_salvage = true, string postSalvageItemId = "")
	{
		item.EnableTag("SALVAGABLE_ITEM_TAG");
		item.EnableTag("DESTROY_AFTER_SALVAGE_BOOL");
		item.ModifyTag("DESTROY_AFTER_SALVAGE_BOOL", delegate(TagState state)
		{
			state.SetBool(destroy_after_salvage);
		});
		item.EnableTag("POST_SALVAGE_ITEM_ID_STRING");
		item.ModifyTag("POST_SALVAGE_ITEM_ID_STRING", delegate(TagState state)
		{
			state.SetString(postSalvageItemId);
		});
	}

	public static void InitSalvageTool(GameItem item, int quality)
	{
		item.EnableTag("SALVAGE_TOOL_TAG");
		item.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		item.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanSalvage(s, t)));
		item.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanSalvage(s, t)));
		item.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(item.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			TrySalvage(s, t);
		});
		item.EnableTag("WELDING_QUALITY_INT");
		item.ModifyTag("WELDING_QUALITY_INT", delegate(TagState state)
		{
			state.SetInt(quality);
		});
	}

	public static bool CanSalvage(GameItem tool, GameItem target)
	{
		if (target.IsTag("SALVAGABLE_ITEM_TAG"))
		{
			return true;
		}
		return false;
	}

	public static void TrySalvage(GameItem tool, GameItem target)
	{
		if (target.IsTag("SALVAGABLE_ITEM_TAG"))
		{
			target.TryUnloadAll();
			if (target.GetTagReadonly("DESTROY_AFTER_SALVAGE_BOOL").GetBool())
			{
				target.Destroy();
				return;
			}
			GameInventory parentInventory = target.parentInventory;
			target.Destroy();
			string @string = target.GetTagReadonly("POST_SALVAGE_ITEM_ID_STRING").GetString();
			GraphUtils.TryAcceptAll(parentInventory, DirectoryMaster.Item(@string));
		}
	}
}

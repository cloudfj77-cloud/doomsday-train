using System;

public class CanteenHelper
{
	public const int CANTEEN_CAPACITY = 2000;

	public const int CANTEEN_MAX_HYDRATION_PER_SIP = 30;

	public const string CANTEEN_TAG = "canteen";

	public const string CANTEEN_MAX_TAG = "max_storage";

	public const string CANTEEN_CURRENT_TAG = "current_storage";

	public const string CANTEEN_IS_SIP_BOOL_TAG = "is_canteen_sip";

	[Obsolete("CanteenHelper is not Deprecated. Used LiquidContainerHelper instead")]
	public static void InitCanteenItem(GameItem item, int capacity = 2000)
	{
		item.EnableTag("canteen");
		item.ModifyTag("max_storage", delegate(TagState state)
		{
			state.Enable().SetInt(2000);
		});
		item.ModifyTag("current_storage", delegate(TagState state)
		{
			state.Enable().SetInt(2000);
		});
		item.ModifyTag("is_canteen_sip", delegate(TagState state)
		{
			state.Enable().SetBool(val: true);
		});
		item.mayToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.canToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.onToggleSlotItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _)
		{
			bool currentState = !item.GetTagReadonly("is_canteen_sip").GetBool();
			item.ModifyTag("is_canteen_sip", delegate(TagState state)
			{
				state.Enable().SetBool(currentState);
			});
		};
	}

	[Obsolete("CanteenHelper is not Deprecated. Used LiquidContainerHelper instead")]
	public static void FillCanteen(GameItem canteenItem, GameItem sourceItem)
	{
		if (!canteenItem.IsTag("canteen") || !sourceItem.IsTag("editable_item") || sourceItem.GetTagReadonly("add_thirst").GetInt() <= 0)
		{
			return;
		}
		int water = sourceItem.GetTagReadonly("add_thirst").GetInt();
		int current_storage = canteenItem.GetTagReadonly("current_storage").GetInt();
		int max_storage = canteenItem.GetTagReadonly("max_storage").GetInt();
		if (water + current_storage <= max_storage)
		{
			canteenItem.ModifyTag("current_storage", delegate(TagState state)
			{
				state.SetInt(water + current_storage);
			});
		}
		else
		{
			canteenItem.ModifyTag("current_storage", delegate(TagState state)
			{
				state.SetInt(max_storage);
			});
		}
	}

	[Obsolete("CanteenHelper is not Deprecated. Used LiquidContainerHelper instead")]
	public static void CheckCanteen(GameCharacterItem GCI)
	{
		GameItem gameItem = GraphUtils.FindChildType(GCI, (GameItem item) => item.identifier == "canteen");
		if (gameItem != null && gameItem.GetTagReadonly("is_canteen_sip").GetBool())
		{
			SipFromCanteen(gameItem, GCI);
		}
	}

	[Obsolete("CanteenHelper is not Deprecated. Used LiquidContainerHelper instead")]
	public static void SipFromCanteen(GameItem item, GameCharacterItem GCI)
	{
		int missingHydration = GCI.maxThirst - GCI.currentThirst;
		if (missingHydration > 30)
		{
			missingHydration = 30;
		}
		int current_storage = item.GetTagReadonly("current_storage").GetInt();
		if (current_storage < missingHydration)
		{
			missingHydration = current_storage;
		}
		item.ModifyTag("current_storage", delegate(TagState state)
		{
			state.SetInt(current_storage - missingHydration);
		});
		GCI.currentThirst += missingHydration;
	}

	[Obsolete("CanteenHelper is not Deprecated. Used LiquidContainerHelper instead")]
	public static void CreateCanteenTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("canteen"))
		{
			int @int = item.GetTagReadonly("current_storage").GetInt();
			int int2 = item.GetTagReadonly("max_storage").GetInt();
			builder.AddLine(" ");
			builder.AddProgressBar((float)@int / (float)int2, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
			builder.AddLine("(" + @int + "/" + (float)int2 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			bool @bool = item.GetTagReadonly("is_canteen_sip").GetBool();
			RenderHandler.ColorPalette color = RenderHandler.ColorPalette.LightCyan;
			string text = "The crew will drink from this canteen";
			if (!@bool)
			{
				text = "The crew will not drink from this canteen";
				color = RenderHandler.ColorPalette.Red;
			}
			builder.AddLine(" ");
			builder.AddLine(text, ignoreEmpty: true, color);
			builder.AddLine(" ");
		}
	}
}

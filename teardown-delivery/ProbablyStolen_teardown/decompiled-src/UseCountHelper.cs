using System;
using UnityEngine;

public class UseCountHelper
{
	public const string USE_COUNT_ITEM_TAG = "USE_COUNT_ITEM_TAG";

	public const string MAX_USE_COUNT_INT = "MAX_USE_COUNT_INT";

	public const string CURRENT_USE_COUNT_INT = "CURRENT_USE_COUNT_INT";

	public const string USE_COUNT_TRANSFERABLE_BOOL = "USE_COUNT_TRANSFERABLE_BOOL";

	public const string USE_COUNT_DESTROY_ON_EMPTY_BOOL = "USE_COUNT_DESTROY_ON_EMPTY_BOOL";

	public const string USE_COUNT_AUTO_VALUE_BOOL = "USE_COUNT_AUTO_VALUE_BOOL";

	public const string USE_COUNT_BASE_VALUE_INT = "USE_COUNT_BASE_VALUE_INT";

	public const string USE_COUNT_VALUE_PER_USE_INT = "USE_COUNT_VALUE_PER_USE_INT";

	public const string USE_COUNT_REFILL_DAYS_INT = "USE_COUNT_REFILL_DAYS_INT";

	public const string USE_COUNT_REFILL_PROGRESS_INT = "USE_COUNT_REFILL_PROGRESS_INT";

	public static void InitUseCountItem(GameItem item, int maxUseCount, bool transferable = true, bool destroyOnEmpty = true, bool useAutoValue = false, int baseValue = 0, int valuePerUse = 0)
	{
		item.ModifyTag("USE_COUNT_ITEM_TAG", delegate(TagState state)
		{
			state.Enable().SetBool(val: true);
		});
		item.ModifyTag("MAX_USE_COUNT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(maxUseCount);
		});
		item.ModifyTag("CURRENT_USE_COUNT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(maxUseCount);
		});
		item.ModifyTag("USE_COUNT_TRANSFERABLE_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(transferable);
		});
		item.ModifyTag("USE_COUNT_DESTROY_ON_EMPTY_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(destroyOnEmpty);
		});
		if (useAutoValue)
		{
			item.ModifyTag("USE_COUNT_AUTO_VALUE_BOOL", delegate(TagState state)
			{
				state.Enable().SetBool(val: true);
			});
			item.ModifyTag("USE_COUNT_BASE_VALUE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(baseValue);
			});
			item.ModifyTag("USE_COUNT_VALUE_PER_USE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(valuePerUse);
			});
		}
		if (transferable)
		{
			item.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => s.identifier.Equals(t.identifier)));
			item.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => s.identifier.Equals(t.identifier)));
		}
		item.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(item.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (s.GetTagReadonly("USE_COUNT_TRANSFERABLE_BOOL").GetBool() && s.identifier.Equals(t.identifier) && GeneralHelper.IsItemOwned(s, t))
			{
				int @int = s.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt();
				int int2 = t.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt();
				int int3 = t.GetTagReadonly("MAX_USE_COUNT_INT").GetInt();
				int b = Mathf.Max(0, int3 - int2);
				int num = Mathf.Min(@int, b);
				int remaining = @int - num;
				int newTargetCurrent = int2 + num;
				t.ModifyTag("CURRENT_USE_COUNT_INT", delegate(TagState state)
				{
					state.Enable().SetInt(newTargetCurrent);
				});
				s.ModifyTag("CURRENT_USE_COUNT_INT", delegate(TagState state)
				{
					state.Enable().SetInt(remaining);
				});
				if (t.GetTagReadonly("USE_COUNT_AUTO_VALUE_BOOL").GetBool())
				{
					int int4 = t.GetTagReadonly("USE_COUNT_BASE_VALUE_INT").GetInt();
					int int5 = t.GetTagReadonly("USE_COUNT_VALUE_PER_USE_INT").GetInt();
					t.unitValue = int4 + newTargetCurrent * int5;
				}
				if (s.GetTagReadonly("USE_COUNT_AUTO_VALUE_BOOL").GetBool())
				{
					int int6 = s.GetTagReadonly("USE_COUNT_BASE_VALUE_INT").GetInt();
					int int7 = s.GetTagReadonly("USE_COUNT_VALUE_PER_USE_INT").GetInt();
					s.unitValue = int6 + remaining * int7;
				}
				if (s.GetTagReadonly("USE_COUNT_DESTROY_ON_EMPTY_BOOL").GetBool() && remaining == 0)
				{
					s.Destroy();
				}
			}
		});
	}

	public static void Use(GameItem item, int charge = 1)
	{
		if (item.IsTag("USE_COUNT_ITEM_TAG"))
		{
			int @int = item.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt();
			int newCharge = Mathf.Max(0, @int - charge);
			item.ModifyTag("CURRENT_USE_COUNT_INT", delegate(TagState state)
			{
				state.Enable().SetInt(newCharge);
			});
			bool @bool = item.GetTagReadonly("USE_COUNT_DESTROY_ON_EMPTY_BOOL").GetBool();
			if (item.GetTagReadonly("USE_COUNT_AUTO_VALUE_BOOL").GetBool())
			{
				int int2 = item.GetTagReadonly("USE_COUNT_BASE_VALUE_INT").GetInt();
				int int3 = item.GetTagReadonly("USE_COUNT_VALUE_PER_USE_INT").GetInt();
				item.unitValue = int2 + newCharge * int3;
			}
			if (@bool && newCharge <= 0)
			{
				item.Destroy();
			}
		}
	}

	public static bool CanUse(GameItem item, int charge = 1)
	{
		if (item.IsTag("USE_COUNT_ITEM_TAG") && item.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt() >= charge)
		{
			return true;
		}
		return false;
	}

	public static void CreateUseCountItemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("USE_COUNT_ITEM_TAG"))
		{
			return;
		}
		int @int = item.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt();
		int num = item.GetTagReadonly("MAX_USE_COUNT_INT").GetInt();
		if (num == 0)
		{
			num = 1;
		}
		builder.Add(LocHelper.GetLocalizedMechanic("mech_use_count_use_left"));
		builder.AddProgressBar((float)@int / (float)num, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
		builder.AddLine("(" + @int + "/" + (float)num + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		if (item.IsTag("USE_COUNT_REFILL_DAYS_INT"))
		{
			int num2 = Mathf.Max(1, item.GetTagReadonly("USE_COUNT_REFILL_DAYS_INT").GetInt());
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_use_count_refill", num2), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			if (@int < num)
			{
				int int2 = item.GetTagReadonly("USE_COUNT_REFILL_PROGRESS_INT").GetInt();
				int num3 = Mathf.Max(1, num2 - int2);
				string text = ((num3 == 1) ? LocHelper.GetLocalizedMechanic("mech_use_count_refill_tomorrow") : LocHelper.GetLocalizedMechanic("mech_use_count_refill_next", num3));
				builder.AddLine(text, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
	}

	public static void InitAutoRefill(GameItem item, int daysPerUse)
	{
		item.ModifyTag("USE_COUNT_REFILL_DAYS_INT", delegate(TagState state)
		{
			state.Enable().SetInt(Mathf.Max(1, daysPerUse));
		});
		item.ModifyTag("USE_COUNT_REFILL_PROGRESS_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
	}

	public static void OnNight()
	{
		foreach (GameItem allItem in EmporiumEntry.Instance.GetAllItems())
		{
			if (allItem == null || !allItem.IsTag("USE_COUNT_ITEM_TAG") || !allItem.IsTag("USE_COUNT_REFILL_DAYS_INT"))
			{
				continue;
			}
			int @int = allItem.GetTagReadonly("MAX_USE_COUNT_INT").GetInt();
			if (allItem.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt() >= @int)
			{
				allItem.ModifyTag("USE_COUNT_REFILL_PROGRESS_INT", delegate(TagState state)
				{
					state.Enable().SetInt(0);
				});
				continue;
			}
			int num = Mathf.Max(1, allItem.GetTagReadonly("USE_COUNT_REFILL_DAYS_INT").GetInt());
			int progress = allItem.GetTagReadonly("USE_COUNT_REFILL_PROGRESS_INT").GetInt() + 1;
			if (progress >= num)
			{
				ReplenishUseCount(allItem, 1);
				progress = 0;
			}
			allItem.ModifyTag("USE_COUNT_REFILL_PROGRESS_INT", delegate(TagState state)
			{
				state.Enable().SetInt(progress);
			});
		}
	}

	public static void ModMaxUseCount(GameItem item, int amount)
	{
		if (item.IsTag("USE_COUNT_ITEM_TAG"))
		{
			int @int = item.GetTagReadonly("MAX_USE_COUNT_INT").GetInt();
			int int2 = item.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt();
			int newMax = Mathf.Max(0, @int + amount);
			int newCurrent = Mathf.Min(newMax, int2);
			item.ModifyTag("MAX_USE_COUNT_INT", delegate(TagState state)
			{
				state.Enable().SetInt(newMax);
			});
			item.ModifyTag("CURRENT_USE_COUNT_INT", delegate(TagState state)
			{
				state.Enable().SetInt(newCurrent);
			});
		}
	}

	public static void ReplenishUseCount(GameItem item, int amount)
	{
		if (item.IsTag("USE_COUNT_ITEM_TAG"))
		{
			int @int = item.GetTagReadonly("MAX_USE_COUNT_INT").GetInt();
			int int2 = item.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt();
			int newCurrent = Mathf.Min(@int, int2 + amount);
			item.ModifyTag("CURRENT_USE_COUNT_INT", delegate(TagState state)
			{
				state.Enable().SetInt(newCurrent);
			});
			int num = newCurrent - int2;
			if (item.GetTagReadonly("USE_COUNT_AUTO_VALUE_BOOL").GetBool() && num > 0)
			{
				int int3 = item.GetTagReadonly("USE_COUNT_BASE_VALUE_INT").GetInt();
				int int4 = item.GetTagReadonly("USE_COUNT_VALUE_PER_USE_INT").GetInt();
				item.unitValue = int3 + newCurrent * int4;
			}
		}
	}

	public static void SetCurrentUseCount(GameItem item, int amount)
	{
		if (item.IsTag("USE_COUNT_ITEM_TAG"))
		{
			int @int = item.GetTagReadonly("MAX_USE_COUNT_INT").GetInt();
			int newCurrent = Mathf.Clamp(amount, 0, @int);
			item.ModifyTag("CURRENT_USE_COUNT_INT", delegate(TagState state)
			{
				state.Enable().SetInt(newCurrent);
			});
			if (item.GetTagReadonly("USE_COUNT_AUTO_VALUE_BOOL").GetBool())
			{
				int int2 = item.GetTagReadonly("USE_COUNT_BASE_VALUE_INT").GetInt();
				int int3 = item.GetTagReadonly("USE_COUNT_VALUE_PER_USE_INT").GetInt();
				item.unitValue = int2 + newCurrent * int3;
			}
		}
	}
}

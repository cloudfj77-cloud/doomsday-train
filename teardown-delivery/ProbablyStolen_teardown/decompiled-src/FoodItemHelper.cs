using System;
using System.Collections.Generic;

public class FoodItemHelper
{
	public const string EDITABLE_TAG = "editable_item";

	public const int REQUIRED_WATER_VOLUME_RAMEN = 375;

	public const string BIO_PRINTABLE_TAG = "BIO_PRINTABLE_TAG";

	public const string FRESHNESS_TAG = "FRESHNESS_TAG";

	public const string CANNED_FOOD_TAG = "CANNED_FOOD_TAG";

	public const string MEAT_FOOD_TAG = "MEAT_FOOD_TAG";

	public const string CALORIE_VALUE_TAG = "CALORIE_VALUE_TAG";

	public static bool IsMeatFood(GameItem item)
	{
		if (item.IsTag("MEAT_FOOD_TAG"))
		{
			return true;
		}
		return !string.IsNullOrEmpty(item.GetTagReadonly("MEAT_TYPE_TAG").GetString());
	}

	public static void CreateFoodItemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("CALORIE_VALUE_TAG"))
		{
			int @int = item.GetTagReadonly("CALORIE_VALUE_TAG").GetInt();
			string key = (item.IsTag("MEAT_FOOD_TAG") ? "mech_food_calories_meat" : "mech_food_calories");
			builder.AddLine(" " + LocHelper.GetLocalizedMechanic(key, @int), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
	}

	public static void InitWaterSource(GameItem item)
	{
		item.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("canteen")));
		item.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("canteen")));
		item.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(item.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t.IsTag("canteen"))
			{
				CanteenHelper.FillCanteen(t, s);
				s.Destroy();
			}
		});
	}

	public static void CanCookCupNoodle(GameItem item)
	{
		item.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => "cup_noodle".Equals(t?.identifier)));
		item.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => "cup_noodle".Equals(t?.identifier)));
		item.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(item.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t.identifier.Equals("cup_noodle") && !t.IsTag("cooked") && LiquidContainerHelper.CanRemoveVolume(s, 375))
			{
				t.EnableTag("cooked");
				FoodItemDirectory.TransformEdible(t, 375);
				LiquidContainerHelper.RemoveVolume(s, 375);
			}
		});
	}

	public static void InitFreshFood(GameItem item, int freshness = 10)
	{
		item.EnableTag("FRESH_FOOD_TAG");
		item.ModifyTag("FRESHNESS_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(freshness);
		});
	}

	public static void CloneFreshness(GameItem source, GameItem target)
	{
		target.EnableTag("FRESH_FOOD_TAG");
		target.ModifyTag("FRESHNESS_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(source.GetTagReadonly("FRESHNESS_TAG").valueInt);
		});
	}

	public static string GetFreshnessString(GameItem item)
	{
		int valueInt = item.GetTagReadonly("FRESHNESS_TAG").valueInt;
		if (valueInt <= 0)
		{
			return "rotten";
		}
		if (valueInt <= 3)
		{
			return "spoiling";
		}
		if (valueInt <= 7)
		{
			return "stale";
		}
		if (valueInt <= 12)
		{
			return "";
		}
		return "fresh";
	}

	public static void SplitFood(GameItem item)
	{
		List<GameCharacterItem> currentPartyGCI = MetaHelper.GetCurrentPartyGCI();
		int num = item.GetTagReadonly("add_thirst").valueInt / currentPartyGCI.Count;
		foreach (GameCharacterItem item2 in currentPartyGCI)
		{
			item2.currentThirst += num;
			if (item2.currentThirst > item2.maxThirst)
			{
				item2.currentThirst = item2.maxThirst;
			}
		}
	}

	public static bool CanThisFoodBeSplited(GameItem item)
	{
		return item.GetTagReadonly("add_thirst").valueInt >= 0;
	}

	public static void SetCalories(GameItem item, int calories)
	{
		item.ModifyTag("CALORIE_VALUE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(calories);
		});
	}

	public static void InitFoodItem(GameItem item, int calories, bool isCanned, bool isMeat = false)
	{
		if (isCanned)
		{
			item.EnableTag("CANNED_FOOD_TAG");
		}
		if (isMeat)
		{
			item.EnableTag("MEAT_FOOD_TAG");
		}
		item.ModifyTag("CALORIE_VALUE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(calories);
		});
	}
}

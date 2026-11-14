using System;
using UnityEngine;

public class GutterflowPelletHelper
{
	public const string GUTTERFLOW_PELLET_TAG = "GUTTERFLOW_PELLET_TAG";

	public const string GUTTERFLOW_TAINTED_TAG = "GUTTERFLOW_TAINTED_TAG";

	public const string CATEGORY_TRASH_GRADE = "CATEGORY_TRASH_GRADE";

	public const string TRASH_GRADE_ID = "trash_grade";

	public const string PELLET_ID = "gutterflow_pellet";

	public const int DEFAULT_KCAL = 4000;

	public const int KCAL_PER_CREDIT = 100;

	public const int MIN_PIECE_KCAL = 500;

	public const float TAINT_THRESHOLD = 0.25f;

	public const int TRASH_GRADE_VALUE_MOD = -75;

	public static bool IsPellet(GameItem item)
	{
		return item?.IsTag("GUTTERFLOW_PELLET_TAG") ?? false;
	}

	public static int GetKcal(GameItem pellet)
	{
		return pellet.GetTagReadonly("CALORIE_VALUE_TAG").GetInt();
	}

	public static GameItem CreatePellet(int kcal = 4000)
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetName(LocHelper.GetLocalizedItem("item_gutterflow_pellet_name")).SetSpriteAndShape("Items/items_aquaculture", "fish_food");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_gutterflow_pellet_desc");
		gameItem.EnableTag("GUTTERFLOW_PELLET_TAG");
		FoodItemHelper.InitFoodItem(gameItem, kcal, isCanned: false);
		SetKcal(gameItem, kcal);
		gameItem.mayActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s) && CanSplit(s);
		gameItem.canActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s) && CanSplit(s);
		gameItem.onActivateSlotItemFunc = delegate(GameItem s, GameInventory inv, SlotMarker _)
		{
			Split(s);
		};
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && CanMerge(s, t)));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && CanMerge(s, t)));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			Merge(s, t);
		});
		AudioHelper.InitProduce(gameItem);
		return gameItem;
	}

	public static void SetKcal(GameItem pellet, int kcal)
	{
		if (kcal <= 0)
		{
			pellet.Destroy();
			return;
		}
		pellet.ModifyTag("CALORIE_VALUE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(kcal);
		});
		pellet.unitValue = kcal / 100;
	}

	public static bool CanSplit(GameItem pellet)
	{
		return GetKcal(pellet) >= 1000;
	}

	public static bool CanMerge(GameItem source, GameItem target)
	{
		if (IsPellet(target) && target != source)
		{
			return GetKcal(source) + GetKcal(target) <= 4000;
		}
		return false;
	}

	public static int Draw(GameItem pellet, int wanted)
	{
		int kcal = GetKcal(pellet);
		int num = Mathf.Clamp(wanted, 0, kcal);
		if (num <= 0)
		{
			return 0;
		}
		SetKcal(pellet, kcal - num);
		return num;
	}

	private static void Split(GameItem pellet)
	{
		if (CanSplit(pellet))
		{
			int kcal = GetKcal(pellet);
			int num = kcal / 2;
			GameItem gameItem = ItemSpawner.Spawn("gutterflow_pellet");
			if (gameItem != null)
			{
				SetKcal(gameItem, num);
				SetKcal(pellet, kcal - num);
				GraphUtils.EmporiumTryAdd(pellet.parentInventory, gameItem);
				AudioManager.Instance.Play3("click_vintage");
			}
		}
	}

	private static void Merge(GameItem source, GameItem target)
	{
		if (CanMerge(source, target))
		{
			int kcal = GetKcal(source);
			if (kcal > 0)
			{
				SetKcal(target, GetKcal(target) + kcal);
				source.Destroy();
				AudioManager.Instance.Play3("click_vintage");
			}
		}
	}

	public static void ApplyTrashGrade(GameItem meat)
	{
		meat.EnableTag("GUTTERFLOW_TAINTED_TAG");
		StripFoodType(meat);
		if (meat.FindItemFeatureByCategory("CATEGORY_TRASH_GRADE") == null)
		{
			ItemCondition itemCondition = new ItemCondition();
			itemCondition.category = "CATEGORY_TRASH_GRADE";
			itemCondition.display = LocHelper.GetLocalizedMechanic("item_feature_trash_grade");
			itemCondition.modValue = -75;
			itemCondition.identifier = "trash_grade";
			ItemFeature itemFeature = new ItemFeature("CATEGORY_TRASH_GRADE");
			itemFeature.identifier = "trash_grade";
			itemFeature.SetConditions(itemCondition, itemCondition);
			itemFeature.isFeatureMatch = true;
			meat.AddItemFeature(itemFeature);
		}
	}

	public static void StripFoodType(GameItem meat)
	{
		meat.RemoveGameItemType("FOOD");
	}

	public static bool IsTainted(GameItem meat)
	{
		return meat.IsTag("GUTTERFLOW_TAINTED_TAG");
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (IsPellet(item))
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_gutterflow_pellet_trash_rule", Mathf.RoundToInt(25f)), ignoreEmpty: true, RenderHandler.ColorPalette.Yellow);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_gutterflow_pellet_controls"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
		}
		else if (IsTainted(item))
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_trash_grade_not_food"), ignoreEmpty: true, RenderHandler.ColorPalette.Yellow);
		}
	}
}

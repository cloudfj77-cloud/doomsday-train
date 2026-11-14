using System;
using UnityEngine;

public class IndustrialFishFeedHelper
{
	public const string INDUSTRIAL_FISH_FEED_TAG = "INDUSTRIAL_FISH_FEED_TAG";

	public const string FEED_ID = "industrial_fish_feed";

	public const int DEFAULT_KCAL = 5000;

	public const int KCAL_PER_CREDIT = 50;

	public const int MIN_PIECE_KCAL = 500;

	public const float GROWTH_RATE = 0.75f;

	public static bool IsFeed(GameItem item)
	{
		return item?.IsTag("INDUSTRIAL_FISH_FEED_TAG") ?? false;
	}

	public static int GetKcal(GameItem feed)
	{
		return feed.GetTagReadonly("CALORIE_VALUE_TAG").GetInt();
	}

	public static GameItem CreateFeed(int kcal = 5000)
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetName(LocHelper.GetLocalizedItem("item_industrial_fish_feed_name")).SetSpriteAndShape("Items/items_aquaculture", "fish_food2");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_industrial_fish_feed_desc");
		gameItem.EnableTag("INDUSTRIAL_FISH_FEED_TAG");
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

	public static void SetKcal(GameItem feed, int kcal)
	{
		if (kcal <= 0)
		{
			feed.Destroy();
			return;
		}
		feed.ModifyTag("CALORIE_VALUE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(kcal);
		});
		feed.unitValue = kcal / 50;
	}

	public static bool CanSplit(GameItem feed)
	{
		return GetKcal(feed) >= 1000;
	}

	public static bool CanMerge(GameItem source, GameItem target)
	{
		if (IsFeed(target) && target != source)
		{
			return GetKcal(source) + GetKcal(target) <= 5000;
		}
		return false;
	}

	public static int Draw(GameItem feed, int wanted)
	{
		int kcal = GetKcal(feed);
		int num = Mathf.Clamp(wanted, 0, kcal);
		if (num <= 0)
		{
			return 0;
		}
		SetKcal(feed, kcal - num);
		return num;
	}

	private static void Split(GameItem feed)
	{
		if (CanSplit(feed))
		{
			int kcal = GetKcal(feed);
			int num = kcal / 2;
			GameItem gameItem = ItemSpawner.Spawn("industrial_fish_feed");
			if (gameItem != null)
			{
				SetKcal(gameItem, num);
				SetKcal(feed, kcal - num);
				GraphUtils.EmporiumTryAdd(feed.parentInventory, gameItem);
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

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (IsFeed(item))
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_industrial_fish_feed_growth", Mathf.RoundToInt(75f)), ignoreEmpty: true, RenderHandler.ColorPalette.Yellow);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_industrial_fish_feed_controls"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
		}
	}
}

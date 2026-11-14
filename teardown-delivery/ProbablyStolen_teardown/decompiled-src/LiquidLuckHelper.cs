using System.Collections.Generic;
using UnityEngine;

public static class LiquidLuckHelper
{
	public const string IDENTIFIER = "liquid_luck";

	public const string LIQUID_LUCK_TAG = "LIQUID_LUCK_TAG";

	public const int VALUE = 100;

	public const int LUCK_BONUS = 75;

	public const int DAYS = 3;

	private const string ATLAS = "Items/items_misc";

	private const string BOTTLE_SPRITE = "liquid_luck_bottle";

	private const string LABEL_SPRITE = "liquid_luck_label";

	private const string LIQUID_ATLAS = "Items/water_container3";

	private const string LIQUID_PREFIX = "mini";

	private const string FILL = "100";

	private const int RUSTWATER_PURITY = 9300;

	private static string rustwaterHex;

	public static GameItem CreateLiquidLuck()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_misc", "liquid_luck_bottle");
		gameItem.name = LocHelper.GetLocalizedItem("item_liquid_luck_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_liquid_luck_flavor");
		gameItem.unitValue = 100L;
		gameItem.EnableManufacturer("georgio");
		gameItem.EnableTag("LIQUID_LUCK_TAG");
		gameItem.SetGameItemType("WATER_RATION");
		gameItem.getItemSpriteModifierForItemFunc = delegate
		{
			string rustwaterColor = GetRustwaterColor();
			return new List<ItemSpriteModifier>
			{
				new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/water_container3", "mini_" + Liquid.GetColorBase(rustwaterColor) + "_100"), Vector2.zero, 0, isFlipped: false, 0, haveMask: false, MetaHelper.HexToColor(rustwaterColor)),
				new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/water_container3", "mini_hl_100"), Vector2.zero),
				new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_misc", "liquid_luck_label"), Vector2.zero)
			};
		};
		gameItem.mayActivateSlotItemFunc = (GameItem curItem, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(curItem);
		gameItem.canActivateSlotItemFunc = (GameItem curItem, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(curItem);
		gameItem.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker _)
		{
			Drink(curItem, curInv);
		};
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private static void Drink(GameItem item, GameInventory inventory)
	{
		if (GeneralHelper.IsItemOwned(item) && inventory != null && PlayerStore.Instance?.healthData != null)
		{
			PlayerStore.Instance.healthData.liquidLuckDaysLeft += 3;
			AudioManager.Instance.Play3("drink");
			GridShape tempShape = item.modifiedShape.Clone();
			inventory.Expel(item);
			GameItem gameItem = DirectoryMaster.Item("mini_bottle");
			if (inventory.TryInventorySlot(gameItem, tempShape).TryAcceptOnce() <= 0)
			{
				GraphUtils.EmporiumTryAdd(inventory, gameItem);
			}
			gameItem.Validate();
		}
	}

	public static int GetActiveLuckBonus(HealthData healthData)
	{
		if (healthData == null || healthData.liquidLuckDaysLeft <= 0)
		{
			return 0;
		}
		return 75;
	}

	public static void OnSleep(HealthData healthData)
	{
		if (healthData.liquidLuckDaysLeft > 0)
		{
			healthData.liquidLuckDaysLeft--;
			if (healthData.liquidLuckDaysLeft == 0)
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_liquid_luck_wears_off"), "cc7755");
			}
		}
	}

	public static string GetRustwaterColor()
	{
		if (rustwaterHex != null)
		{
			return rustwaterHex;
		}
		float r = 0f;
		float g = 0f;
		float b = 0f;
		float total = 0f;
		float num = 0.93f;
		Mix(Liquid.GetLiquidFromId("water"), num);
		foreach (KeyValuePair<string, float> contaminant in WaterPremadeHelper.rustWaterContaminantDistribution)
		{
			Mix(Liquid.Liquids.Find((Liquid l) => l.currentPart == contaminant.Key), (1f - num) * contaminant.Value);
		}
		rustwaterHex = ((total > 0f) ? MetaHelper.ColorToHex(new Color(r / total, g / total, b / total)) : "000000");
		return rustwaterHex;
		void Mix(Liquid liquid, float share)
		{
			if (liquid != null && !(share <= 0f) && ColorUtility.TryParseHtmlString("#" + liquid.color, out var color))
			{
				float num2 = share * (float)Mathf.Max(1, liquid.colorMultiplier);
				r += color.r * num2;
				g += color.g * num2;
				b += color.b * num2;
				total += num2;
			}
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

public static class WaterTankHelper
{
	public const string IDENTIFIER = "water_tank";

	public const string WATER_TANK_TAG = "WATER_TANK_TAG";

	public const int START_ML = 20000;

	public const int MAX_ML = 50000;

	public const int SCRAP_ML = 2000;

	private static readonly Dictionary<int, int> INGOT_ML_BY_LEVEL = new Dictionary<int, int>
	{
		{ 6, 4000 },
		{ 7, 6000 },
		{ 8, 8000 },
		{ 9, 10000 },
		{ 10, 14000 }
	};

	public const int INGOT_ML_DEFAULT = 6000;

	public const int VALUE = 350;

	public static bool IsTank(GameItem item)
	{
		return item?.IsTag("WATER_TANK_TAG") ?? false;
	}

	public static int GetCapacityMl(GameItem tank)
	{
		return tank.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt() / 1000;
	}

	public static bool CanUpgrade(GameItem tank)
	{
		if (IsTank(tank) && GeneralHelper.IsItemOwned(tank))
		{
			return GetCapacityMl(tank) < 50000;
		}
		return false;
	}

	public static int GetIngotMl(GameItem ingot)
	{
		string purity = IngotPurityHelper.GetPurity(ingot);
		if (!string.IsNullOrEmpty(purity) && IngotPurityHelper.ingotPurities.TryGetValue(purity, out var value) && INGOT_ML_BY_LEVEL.TryGetValue(value.PurityLevel, out var value2))
		{
			return value2;
		}
		return 6000;
	}

	public static void Upgrade(GameItem tank, GameItem material)
	{
		if (CanUpgrade(tank) && material != null && GeneralHelper.IsItemOwned(material))
		{
			int num = (material.IsID("scrap_metal") ? 2000 : GetIngotMl(material));
			int capacity = Mathf.Min(50000, GetCapacityMl(tank) + num);
			tank.ModifyTag("LIQUID_CONTAINER_CAPACITY", delegate(TagState state)
			{
				state.Enable().SetInt(capacity * 1000);
			});
			tank.unitValue += material.unitValue;
			StartingPerkContent.ConsumeUpgradeMaterial(material);
			EmporiumAudioPlayer.Instance.ratchet.Play();
			tank.Validate();
		}
	}

	public static GameItem Create()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_tool2", "water_tank");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", "item_water_tank_name", "Water Tank");
		gameItem.shortDescription = LocHelper.GetLocalizedOrDefault("Item", "item_water_tank_desc", "A large liquid container. Use scrap metal or a metal ingot on it to enlarge it.");
		gameItem.identifier = "water_tank";
		gameItem.EnableTag("WATER_TANK_TAG");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		WaterHelper.InitLiquidContainerItem(gameItem, 20000, 350, isFilterContainer: false, isGraduated: true);
		AudioHelper.InitSolidMetal(gameItem);
		return gameItem;
	}
}

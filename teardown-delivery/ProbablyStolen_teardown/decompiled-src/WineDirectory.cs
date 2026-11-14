public class WineDirectory : ItemDirectory
{
	protected override void InitDirectory()
	{
		Add("wine_yeast", () => WineYeast());
		Add("wine_superyeast", () => WineSuperYeast());
		Add("wine_yeast_red", () => WineYeastRed());
		Add("wine_yeast_infinite", () => YeastBioreactor());
		Add("wine_bottle", () => WineBottle());
		Add("wine_berry", () => WineBerry());
	}

	private GameItem WineYeast()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_winemaking", "yeast");
		gameItem.name = LocHelper.GetLocalizedItem("item_wine_yeast_name");
		gameItem.unitValue = 10L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_wine_yeast_flavor");
		WineHelper.InitYeast(gameItem, 2);
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		return gameItem;
	}

	private GameItem WineSuperYeast()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_winemaking", "superyeast");
		gameItem.name = LocHelper.GetLocalizedItem("item_wine_superyeast_name");
		gameItem.unitValue = 20L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_wine_superyeast_flavor");
		WineHelper.InitYeast(gameItem, 4);
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		return gameItem;
	}

	private GameItem WineYeastRed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_winemaking", "illegal_yeast");
		gameItem.name = LocHelper.GetLocalizedItem("item_wine_yeast_red_name");
		gameItem.unitValue = 8L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_wine_yeast_red_desc");
		WineHelper.InitIllegalYeast(gameItem, 2);
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		return gameItem;
	}

	private GameItem YeastBioreactor()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_winemaking", "yeast_gen");
		gameItem.name = LocHelper.GetLocalizedItem("item_wine_yeast_infinite_name");
		gameItem.unitValue = 250L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_wine_yeast_infinite_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_wine_yeast_infinite_flavor");
		WineHelper.InitInfiniteYeast(gameItem, 2);
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		return gameItem;
	}

	private GameItem WineAirlock()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_winemaking", "fermentation_airlock");
		gameItem.name = "Fermentation Airlock";
		gameItem.unitValue = 10L;
		gameItem.shortDescription = "Install it on a 750ml plastic water bottle to convert it into a fermentation vessel.";
		gameItem.flavorText = "A device used in fermentation to allow gasses to escape without letting contaminants in.";
		return gameItem;
	}

	private GameItem WineBottle()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_winemaking", "wine_bottle");
		gameItem.name = LocHelper.GetLocalizedItem("item_wine_bottle_name");
		gameItem.unitValue = 12L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_wine_bottle_desc");
		WineHelper.InitWineBottle(gameItem);
		AudioHelper.InitGlass(gameItem);
		return gameItem;
	}

	private GameItem WineBerry()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_winemaking", "wine_berry");
		gameItem.name = LocHelper.GetLocalizedItem("item_wine_berry_name");
		gameItem.unitValue = 14L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_wine_berry_flavor");
		WineHelper.InitBerry(gameItem);
		gameItem.SetGameItemType("FOOD");
		FoodItemHelper.InitFoodItem(gameItem, 150, isCanned: false);
		AudioHelper.InitProduce(gameItem);
		InspectableHelper.InitInspectableItem(gameItem);
		PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "bloomberry_seed");
		return gameItem;
	}
}

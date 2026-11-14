using System;

public class FoodItemDirectory : ItemDirectory
{
	public const string XENO_MEAT_TAG = "XENO_MEAT_TAG";

	public const string RED_IMP_BEER_TAG = "RED_IMP_BEER_TAG";

	protected override void InitDirectory()
	{
		Add("processed_milk", () => ProcessedMilk());
		Add("processed_juice", () => ProcessedJuice());
		Add("processed_meat", () => ProcessedMeat());
		Add("bottle_hot_sauce", () => BottleHotSauce());
		Add("bottled_water_premium", () => BottledWaterPremium());
		Add("red_beer", () => RedBeer());
		Add("empty_beer_bottle", () => EmptyBeerBottle());
		Add("bottled_water", () => BottledWater());
		Add("small_bottled_water", () => SmallBottledWater());
		Add("mini_bottle", () => MiniBottle());
		Add("liquid_luck", () => LiquidLuckHelper.CreateLiquidLuck());
		Add("med_bottle_red", () => MedBottleRed());
		Add("med_bottle_blue", () => MedBottleBlue());
		Add("med_bottle_violet", () => MedBottleViolet());
		Add("med_bottle_orange", () => MedBottleOrange());
		Add("med_bottle_small_pink", () => MedBottleSmallPink());
		Add("1ml_dropper", () => Dropper1ml());
		Add("5ml_dropper", () => Dropper5ml());
		Add("25ml_cylinder", () => Cylinder25ml());
		Add("50ml_cylinder", () => Cylinder50ml());
		Add("100ml_cylinder", () => Cylinder100ml());
		Add("large_bottled_water", () => LargeBottledWater());
		Add("water_jug", () => WaterJug());
		Add("soda_red", () => SodaRed());
		Add("energy_drink", () => EnergyDrink());
		Add("nudka", () => Nudka());
		Add("galaxy_blend", () => GalaxyBlend());
		Add("cat_bar", () => CatBar());
		Add("li_eat_snackbar", () => LiEatSnackbar());
		Add("small_raw_meat", () => SmallRawMeat());
		Add("morsel", () => Morsel());
		Add("small_morsel", () => SmallMorsel());
		Add("raw_meat", () => RawMeat());
		Add("fat_meat", () => FatMeat());
		Add("meat_scrap", () => MeatScrap());
		Add("cup_noodle", () => CupNoodle());
		Add("water_ration", () => WaterRation());
		Add("water_ration_small", () => WaterRationSmall());
		Add("processed_cheese", () => ProcessedCheese());
		Add("beis_icecream", () => BeisIcecream());
	}

	private GameItem BeisIcecream()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_beis_icecream_name")).SetSpriteAndShape("Items/items_food", "beis_icecream");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_beis_icecream_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_beis_icecream_desc");
		gameItem.unitValue = 40L;
		gameItem.SetGameItemType("FOOD");
		gameItem.SetGameItemType("TREAT");
		AudioHelper.InitCartonBox(gameItem);
		FoodItemHelper.InitFoodItem(gameItem, 1500, isCanned: false);
		return gameItem;
	}

	private GameItem ProcessedMilk()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_processed_milk_name")).SetSpriteAndShape("Items/items_food", "items_food_pilk_alt");
		gameItem.unitValue = 16L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_processed_milk_flavor");
		gameItem.SetGameItemType("PROCESSED_FOOD");
		gameItem.SetGameItemType("BREVAGE");
		gameItem.SetGameItemType("FOOD");
		AudioHelper.InitCartonBox(gameItem);
		FoodItemHelper.InitFoodItem(gameItem, 600, isCanned: false);
		return gameItem;
	}

	private GameItem ProcessedJuice()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_processed_juice_name")).SetSpriteAndShape("Items/items_food", "items_food_juice");
		gameItem.unitValue = 14L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_processed_juice_desc");
		gameItem.SetGameItemType("PROCESSED_FOOD");
		gameItem.SetGameItemType("BREVAGE");
		gameItem.SetGameItemType("FOOD");
		AudioHelper.InitCartonBox(gameItem);
		FoodItemHelper.InitFoodItem(gameItem, 450, isCanned: false);
		return gameItem;
	}

	private GameItem ProcessedMeat()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_processed_meat_name")).SetSpriteAndShape("Items/items_food", "processed_meat");
		gameItem.SetGameItemType("FOOD");
		gameItem.EnableManufacturer("davre");
		AudioHelper.InitCannedFood(gameItem);
		gameItem.unitValue = 25L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_processed_meat_desc");
		gameItem.SetGameItemType("PROCESSED_FOOD");
		FoodItemHelper.InitFoodItem(gameItem, 800, isCanned: true, isMeat: true);
		return gameItem;
	}

	private GameItem BottleHotSauce()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_bottle_hot_sauce_name")).SetSpriteAndShape("Items/items_food", "hot_sauce");
		gameItem.SetGameItemType("FOOD");
		gameItem.AddAmenities("amenities_nutrition", 1, 100);
		gameItem.unitValue = 50L;
		AudioHelper.InitGlass(gameItem);
		return gameItem;
	}

	private GameItem BottledWaterPremium()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_bottled_water_premium_name")).SetSpriteAndShape("Items/items_food", "water_bottle_full_premium");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_bottled_water_premium_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_bottled_water_premium_flavor");
		gameItem.unitValue = 325L;
		AudioHelper.InitPlasticBottle(gameItem);
		gameItem.SetGameItemType("LUXURY_ITEM");
		gameItem.SetGameItemType("WATER");
		return gameItem;
	}

	private GameItem RedBeer()
	{
		GameItem item = Empty().SetName(LocHelper.GetLocalizedItem("item_red_beer_name")).SetSpriteAndShape("Items/items_food", "red_beer");
		item.flavorText = LocHelper.GetLocalizedItem("item_red_beer_flavor");
		item.unitValue = 18L;
		item.EnableManufacturer("spacer");
		item.EnableTag("RED_IMP_BEER_TAG");
		item.SetGameItemType("SUBSTANCE");
		item.SetGameItemType("ALCOHOL");
		item.SetGameItemType("BREVAGE");
		ConsumableHelper.InitAlcohol(item, 1);
		item.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			PlayerStore.Instance.healthData.ConsumeAlcohol(item.GetTagReadonly("ALCOHOL_VALUE").GetInt());
			GridShape tempShape = item.modifiedShape.Clone();
			curInv.Expel(item);
			GameItem gameItem = DirectoryMaster.Item("empty_beer_bottle");
			if (curInv.TryInventorySlot(gameItem, tempShape).TryAcceptOnce() <= 0)
			{
				GraphUtils.EmporiumTryAdd(curInv, gameItem);
			}
			gameItem.Validate();
		};
		AudioHelper.InitGlass(item);
		return item;
	}

	private GameItem EmptyBeerBottle()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_empty_beer_bottle_name")).SetSpriteAndShape("Items/items_food", "empty_beer_bottle");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_empty_beer_bottle_desc");
		gameItem.unitValue = 5L;
		AudioHelper.InitGlass(gameItem);
		gameItem.SetGameItemType("JUNK");
		return gameItem;
	}

	private GameItem BottledWater()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_bottled_water_name")).SetSpriteAndShape("Items/water_container3", "normal");
		gameItem.unitValue = 5L;
		gameItem.identifier = "bottled_water";
		WaterHelper.InitLiquidContainerItem(gameItem, 750, 5, isFilterContainer: false, isGraduated: false, isDropper: false, isTransparentContainer: true);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem SmallBottledWater()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_small_bottled_water_name")).SetSpriteAndShape("Items/water_container3", "small");
		gameItem.unitValue = 4L;
		gameItem.identifier = "small_bottled_water";
		WaterHelper.InitLiquidContainerItem(gameItem, 450, 4, isFilterContainer: false, isGraduated: false, isDropper: false, isTransparentContainer: true);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem MiniBottle()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_mini_bottle_name")).SetSpriteAndShape("Items/water_container3", "mini");
		gameItem.unitValue = 3L;
		gameItem.identifier = "mini_bottle";
		WaterHelper.InitLiquidContainerItem(gameItem, 250, 3, isFilterContainer: false, isGraduated: false, isDropper: false, isTransparentContainer: true);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem MedBottleRed()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_med_bottle_red_name")).SetSpriteAndShape("Items/items_medical", "med_bottle_red");
		gameItem.unitValue = 3L;
		gameItem.identifier = "med_bottle_red";
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_med_bottle_red_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_med_bottle_red_desc");
		WaterHelper.InitLiquidContainerItem(gameItem, 250, 3);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem MedBottleBlue()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_med_bottle_blue_name")).SetSpriteAndShape("Items/items_medical", "med_bottle_blue");
		gameItem.unitValue = 3L;
		gameItem.identifier = "med_bottle_blue";
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_med_bottle_blue_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_med_bottle_blue_desc");
		WaterHelper.InitLiquidContainerItem(gameItem, 250, 3);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem MedBottleViolet()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_med_bottle_violet_name")).SetSpriteAndShape("Items/items_medical", "med_bottle_violet");
		gameItem.unitValue = 3L;
		gameItem.identifier = "med_bottle_violet";
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_med_bottle_violet_desc");
		WaterHelper.InitLiquidContainerItem(gameItem, 250, 3);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem MedBottleOrange()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_med_bottle_orange_name")).SetSpriteAndShape("Items/items_medical", "med_bottle_orange");
		gameItem.unitValue = 3L;
		gameItem.identifier = "med_bottle_orange";
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_med_bottle_orange_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_med_bottle_orange_desc");
		WaterHelper.InitLiquidContainerItem(gameItem, 250, 3);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem MedBottleSmallPink()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_med_bottle_small_pink_name")).SetSpriteAndShape("Items/items_medical", "med_bottle_small_pink");
		gameItem.unitValue = 1L;
		gameItem.identifier = "med_bottle_small_pink";
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_med_bottle_small_pink_desc");
		WaterHelper.InitLiquidContainerItem(gameItem, 150, 3);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem Dropper1ml()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_1ml_dropper_name")).SetSpriteAndShape("Items/water_container3", "1ml");
		gameItem.unitValue = 120L;
		gameItem.identifier = "1ml_dropper";
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_1ml_dropper_flavor");
		WaterHelper.InitLiquidContainerItem(gameItem, 1, 120, isFilterContainer: false, isGraduated: true, isDropper: true);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem Dropper5ml()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_5ml_dropper_name")).SetSpriteAndShape("Items/water_container3", "5ml");
		gameItem.unitValue = 10L;
		gameItem.identifier = "5ml_dropper";
		WaterHelper.InitLiquidContainerItem(gameItem, 5, 10, isFilterContainer: false, isGraduated: true, isDropper: true);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem Cylinder25ml()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_25ml_cylinder_name")).SetSpriteAndShape("Items/water_container3", "25ml");
		gameItem.unitValue = 7L;
		gameItem.identifier = "25ml_cylinder";
		WaterHelper.InitLiquidContainerItem(gameItem, 25, 7, isFilterContainer: false, isGraduated: true, isDropper: false, isTransparentContainer: true);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem Cylinder50ml()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_50ml_cylinder_name")).SetSpriteAndShape("Items/water_container3", "50ml");
		gameItem.unitValue = 10L;
		gameItem.identifier = "50ml_cylinder";
		WaterHelper.InitLiquidContainerItem(gameItem, 50, 10, isFilterContainer: false, isGraduated: true, isDropper: false, isTransparentContainer: true);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem Cylinder100ml()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_100ml_cylinder_name")).SetSpriteAndShape("Items/water_container3", "100ml");
		gameItem.unitValue = 15L;
		gameItem.identifier = "100ml_cylinder";
		WaterHelper.InitLiquidContainerItem(gameItem, 100, 15, isFilterContainer: false, isGraduated: true, isDropper: false, isTransparentContainer: true);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem LargeBottledWater()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_large_bottled_water_name")).SetSpriteAndShape("Items/water_container3", "large");
		gameItem.unitValue = 9L;
		gameItem.identifier = "large_bottled_water";
		WaterHelper.InitLiquidContainerItem(gameItem, 2000, 9, isFilterContainer: false, isGraduated: false, isDropper: false, isTransparentContainer: true);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem WaterJug()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_water_jug_name")).SetSpriteAndShape("Items/water_container4", "xl");
		gameItem.SetValue(15L);
		gameItem.identifier = "water_jug";
		WaterHelper.InitLiquidContainerItem(gameItem, 6000, 15, isFilterContainer: false, isGraduated: false, isDropper: false, isTransparentContainer: true, 4);
		AudioHelper.InitPlasticBottle(gameItem);
		return gameItem;
	}

	private GameItem SodaRed()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_soda_red_name")).SetSpriteAndShape("Items/items_food", "items_food_soda_red");
		TransformEdible(gameItem, 350);
		gameItem.AddAmenities("amenities_nutrition", 2, 1);
		gameItem.unitValue = 15L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_soda_red_desc");
		gameItem.SetGameItemType("TREAT");
		gameItem.SetGameItemType("BREVAGE");
		AudioHelper.InitAluminumCan(gameItem);
		return gameItem;
	}

	private GameItem EnergyDrink()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_energy_drink_name")).SetSpriteAndShape("Items/items_food", "energy_drink");
		TransformEdible(gameItem, 350);
		gameItem.AddAmenities("amenities_nutrition", 3, 1);
		gameItem.unitValue = 18L;
		gameItem.SetGameItemType("TREAT");
		gameItem.SetGameItemType("BREVAGE");
		AudioHelper.InitAluminumCan(gameItem);
		return gameItem;
	}

	private GameItem Nudka()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_nudka_name")).SetSpriteAndShape("Items/items_food", "nudka");
		gameItem.AddAmenities("amenities_nutrition", 2, 20);
		gameItem.unitValue = 85L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_nudka_desc");
		gameItem.SetGameItemType("ALCOHOL");
		gameItem.SetGameItemType("SUBSTANCE");
		ConsumableHelper.InitAlcohol(gameItem, 4);
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (GameItem s, GameItem t) => WaterHelper.CanPourNudka(t);
		gameItem.canThisTargetItemFunc = (GameItem s, GameItem t) => WaterHelper.CanPourNudka(t);
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (WaterHelper.CanPourNudka(t))
			{
				WaterHelper.PourNudka(t);
				AudioHelper.PourWater();
				s.Destroy();
			}
		};
		AudioHelper.InitGlass(gameItem);
		return gameItem;
	}

	private GameItem GalaxyBlend()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_galaxy_blend_name")).SetSpriteAndShape("Items/items_food", "juice_pouch");
		TransformEdible(gameItem, 350);
		gameItem.EnableManufacturer("pastel");
		gameItem.unitValue = 25L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_galaxy_blend_flavor");
		gameItem.SetGameItemType("PROCESSED_FOOD");
		FoodItemHelper.InitFoodItem(gameItem, 450, isCanned: false);
		AudioHelper.InitSoftPlasticItem(gameItem);
		return gameItem;
	}

	private GameItem CatBar()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_cat_bar_name")).SetSpriteAndShape("Items/items_food", "cat_bar");
		gameItem.AddAmenities("amenities_nutrition", 4, 2);
		gameItem.unitValue = 15L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_cat_bar_flavor");
		gameItem.SetGameItemType("TREAT");
		gameItem.SetGameItemType("FOOD");
		FoodItemHelper.InitFoodItem(gameItem, 210, isCanned: false);
		AudioHelper.InitSoftPlasticItem(gameItem);
		return gameItem;
	}

	private GameItem LiEatSnackbar()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_li_eat_snackbar_name")).SetSpriteAndShape("Items/items_food", "li_eat_snackbar");
		gameItem.AddAmenities("amenities_nutrition", 4, 2);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_li_eat_snackbar_desc");
		gameItem.unitValue = 17L;
		gameItem.SetGameItemType("TREAT");
		gameItem.SetGameItemType("FOOD");
		AudioHelper.InitEdibleAudio(gameItem);
		FoodItemHelper.InitFoodItem(gameItem, 250, isCanned: false);
		AudioHelper.InitSoftPlasticItem(gameItem);
		return gameItem;
	}

	private GameItem ProcessedCheese()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_processed_cheese_name")).SetSpriteAndShape("Items/items_food", "cheese");
		gameItem.AddAmenities("amenities_nutrition", 4, 2);
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_processed_cheese_flavor");
		gameItem.unitValue = 20L;
		gameItem.SetGameItemType("FOOD");
		gameItem.SetGameItemType("PROCESSED_FOOD");
		FoodItemHelper.InitFoodItem(gameItem, 450, isCanned: false);
		AudioHelper.InitEdibleAudio(gameItem);
		return gameItem;
	}

	private GameItem RawMeat()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_raw_meat_name")).SetSpriteAndShape("Items/items_food", "raw_meat");
		AudioHelper.InitFoodAudio(gameItem);
		gameItem.unitValue = 159L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_raw_meat_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_raw_meat_flavor");
		gameItem.EnableTag("BIO_PRINTABLE_TAG");
		gameItem.ModifyTag("BIO_PRINTING_REQUIRED_WORK_INT", delegate(TagState state)
		{
			state.Enable().SetInt(40);
		});
		gameItem.SetGameItemType("FOOD");
		gameItem.SetGameItemType("LUXURY_ITEM");
		FoodItemHelper.InitFoodItem(gameItem, 1200, isCanned: false, isMeat: true);
		return gameItem;
	}

	private GameItem SmallRawMeat()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_small_raw_meat_name")).SetSpriteAndShape("Items/items_food", "small_raw_meat");
		AudioHelper.InitFoodAudio(gameItem);
		gameItem.unitValue = 65L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_small_raw_meat_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_small_raw_meat_flavor");
		gameItem.EnableTag("BIO_PRINTABLE_TAG");
		gameItem.ModifyTag("BIO_PRINTING_REQUIRED_WORK_INT", delegate(TagState state)
		{
			state.Enable().SetInt(25);
		});
		gameItem.SetGameItemType("FOOD");
		FoodItemHelper.InitFoodItem(gameItem, 550, isCanned: false, isMeat: true);
		return gameItem;
	}

	private GameItem Morsel()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_morsel_name")).SetSpriteAndShape("Items/items_food", "morsel");
		AudioHelper.InitFoodAudio(gameItem);
		gameItem.unitValue = 30L;
		gameItem.EnableTag("BIO_PRINTABLE_TAG");
		gameItem.ModifyTag("BIO_PRINTING_REQUIRED_WORK_INT", delegate(TagState state)
		{
			state.Enable().SetInt(25);
		});
		gameItem.SetGameItemType("FOOD");
		FoodItemHelper.InitFoodItem(gameItem, 275, isCanned: false, isMeat: true);
		return gameItem;
	}

	private GameItem SmallMorsel()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_small_morsel_name")).SetSpriteAndShape("Items/items_food", "morsel_small");
		AudioHelper.InitFoodAudio(gameItem);
		gameItem.unitValue = 15L;
		gameItem.EnableTag("BIO_PRINTABLE_TAG");
		gameItem.ModifyTag("BIO_PRINTING_REQUIRED_WORK_INT", delegate(TagState state)
		{
			state.Enable().SetInt(15);
		});
		gameItem.SetGameItemType("FOOD");
		FoodItemHelper.InitFoodItem(gameItem, 140, isCanned: false, isMeat: true);
		return gameItem;
	}

	private GameItem FatMeat()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_fat_meat_name")).SetSpriteAndShape("Items/items_food", "fat_meat");
		AudioHelper.InitFoodAudio(gameItem);
		gameItem.unitValue = 40L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_fat_meat_desc");
		gameItem.SetGameItemType("FOOD");
		FoodItemHelper.InitFoodItem(gameItem, 1250, isCanned: false, isMeat: true);
		return gameItem;
	}

	private GameItem MeatScrap()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_meat_scrap_name")).SetSpriteAndShape("Items/items_food", "meat_scrap");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_meat_scrap_desc");
		gameItem.unitValue = 12L;
		FoodItemHelper.InitFoodItem(gameItem, 350, isCanned: false, isMeat: true);
		return gameItem;
	}

	private GameItem CupNoodle()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_cup_noodle_name")).SetSpriteAndShape("Items/items_food", "cup_noodle");
		gameItem.SetGameItemType("FOOD");
		AudioHelper.InitFoodAudio(gameItem);
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			if (loaded.IsTag("cooked"))
			{
				TransformEdible(loaded, 375);
			}
		});
		gameItem.unitValue = 20L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_cup_noodle_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_cup_noodle_flavor");
		gameItem.EnableManufacturer("davre");
		FoodItemHelper.InitFoodItem(gameItem, 500, isCanned: false);
		return gameItem;
	}

	private GameItem WaterRation()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_water_ration_name")).SetSpriteAndShape("Items/items_food", "water_ration");
		gameItem.unitValue = 30L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_water_ration_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_water_ration_flavor");
		gameItem.SetGameItemType("WATER_RATION");
		SetupWaterRationPour(gameItem, 750);
		AudioHelper.InitSoftPlasticItem(gameItem);
		return gameItem;
	}

	private GameItem WaterRationSmall()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("item_water_ration_small_name")).SetSpriteAndShape("Items/items_food", "water_ration_small");
		gameItem.unitValue = 15L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_water_ration_small_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_water_ration_small_flavor");
		gameItem.SetGameItemType("WATER_RATION");
		SetupWaterRationPour(gameItem, 375);
		AudioHelper.InitSoftPlasticItem(gameItem);
		return gameItem;
	}

	private static bool CanPourRation(GameItem container, int volumeMl)
	{
		if (!container.IsTag("WINE_CONTAINER_TAG"))
		{
			return WaterHelper.CanPourVolume(container, volumeMl);
		}
		return false;
	}

	private static bool MayRationTarget(GameItem container, int volumeMl)
	{
		if (!container.IsTag("WINE_CONTAINER_TAG"))
		{
			return CanPourRation(container, volumeMl);
		}
		return true;
	}

	private static void SetupWaterRationPour(GameItem item, int volumeMl)
	{
		item.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.canSelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.mayThisTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && MayRationTarget(t, volumeMl);
		item.canThisTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && MayRationTarget(t, volumeMl);
		item.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (CanPourRation(t, volumeMl))
			{
				WaterHelper.AddBaseWater(t, volumeMl * 1000);
				AudioHelper.PourWater();
				s.Destroy();
			}
		};
	}

	public static GameItem TransformEdible(GameItem item, int thirst)
	{
		item.SetGameItemType("FOOD");
		item.EnableTag("editable_item");
		item.SetType("editable_item");
		item.ModifyTag("add_thirst", delegate(TagState state)
		{
			state.Enable().SetInt(thirst);
		});
		AudioHelper.InitDrinkableAudio(item);
		return item;
	}
}

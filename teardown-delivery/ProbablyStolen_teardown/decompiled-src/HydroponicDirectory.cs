using System;

public class HydroponicDirectory : ItemDirectory
{
	protected override void InitDirectory()
	{
		Add("nutrifruit", () => Nutrifruit());
		Add("nutrifruit_seed", () => NutrifruitSeed());
		Add("kotton_fiber", () => KottonPlant());
		Add("kotton_seed", () => KottonSeed());
		Add("dream_cap", () => DreamCap());
		Add("dream_cap_seed", () => DreamCapSeed());
		Add("meal_cap", () => MealCap());
		Add("nightmare_cap", () => NightmareCap());
		Add("plant_analyser", () => PlantAnalyser());
		Add("coffee_bean", () => CoffeeBean());
		Add("fillerweed", () => Fillerweed());
		Add("fillerweed_seed", () => FillerweedSeed());
		Add("hydroreed_seed", () => HydroreedSeed());
		Add("bloomberry_seed", () => BloomberrySeed());
		Add("plant_dna_disk", () => PlantDnaDisk());
	}

	private GameItem Nutrifruit()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "nutrifruit");
		gameItem.name = LocHelper.GetLocalizedItem("item_nutrifruit_name");
		FoodItemDirectory.TransformEdible(gameItem, 100);
		gameItem.EnableManufacturer("pastel");
		gameItem.unitValue = 12L;
		gameItem.identifier = "nutrifruit";
		gameItem.SetGameItemType("FOOD");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_nutrifruit_desc");
		ChemicalCompositionHelper.SetPotatoComposition(gameItem, 250f);
		FoodItemHelper.SetCalories(gameItem, 240);
		AudioHelper.InitProduce(gameItem);
		PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "nutrifruit_seed");
		return gameItem;
	}

	private GameItem KottonPlant()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "kotton_plant");
		gameItem.name = LocHelper.GetLocalizedItem("item_kotton_fiber_name");
		gameItem.EnableManufacturer("pastel");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_kotton_fiber_desc");
		gameItem.SetGameItemType("MATERIAL");
		gameItem.unitValue = 46L;
		AudioHelper.InitLeafy(gameItem);
		return gameItem;
	}

	private GameItem NutrifruitSeed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "nutrifruit_seed");
		gameItem.name = LocHelper.GetLocalizedItem("item_nutrifruit_seed_name");
		gameItem.EnableManufacturer("pastel");
		HydroponicHelper.InitSeedItem(gameItem, "nutrifruit_seed");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_nutrifruit_seed_desc");
		gameItem.SetGameItemType("SEED");
		gameItem.unitValue = 40L;
		if (PlantList.IsNutrifruitFreed())
		{
			PlantGeneticsHelper.SetTraitRank(gameItem, "infertile", 0);
		}
		PlantList.RefreshNutrifruitStrain(gameItem);
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, new Action<GameItem>(PlantList.RefreshNutrifruitStrain));
		return gameItem;
	}

	private GameItem KottonSeed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "kotton_plant_seed");
		gameItem.name = LocHelper.GetLocalizedItem("item_kotton_seed_name");
		gameItem.EnableManufacturer("pastel");
		HydroponicHelper.InitSeedItem(gameItem, "kotton_seed");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_kotton_seed_desc");
		gameItem.SetGameItemType("SEED");
		gameItem.unitValue = 20L;
		return gameItem;
	}

	private GameItem DreamCapSeed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "dream_cap_seed");
		gameItem.name = LocHelper.GetLocalizedItem("item_dream_cap_seed_name");
		HydroponicHelper.InitSeedItem(gameItem, "dream_cap_seed");
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		gameItem.SetGameItemType("SEED");
		gameItem.unitValue = 24L;
		return gameItem;
	}

	private GameItem CoffeeBean()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "coffee_bean");
		gameItem.name = LocHelper.GetLocalizedItem("item_coffee_bean_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_coffee_bean_desc");
		gameItem.unitValue = 28L;
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetGameItemType("SEED");
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true);
		ChemicalCompositionHelper.SetCoffeeBeanComposition(gameItem, 80f);
		HydroponicHelper.InitSeedItem(gameItem, "coffee_bean");
		PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "coffee_bean");
		return gameItem;
	}

	private GameItem DreamCap()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "dream_cap");
		gameItem.name = LocHelper.GetLocalizedItem("item_dream_cap_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_dream_cap_desc");
		gameItem.unitValue = 23L;
		gameItem.SetGameItemType("CHEMICAL_SUPPLIES");
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		ChemicalCompositionHelper.SetDreamCapComposition(gameItem, 32.5f);
		AudioHelper.InitProduce(gameItem);
		PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "dream_cap_seed");
		return gameItem;
	}

	private GameItem MealCap()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "meal_cap");
		gameItem.name = LocHelper.GetLocalizedItem("item_meal_cap_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_meal_cap_desc");
		gameItem.unitValue = 19L;
		gameItem.SetGameItemType("FOOD");
		ChemicalCompositionHelper.SetDreamCapComposition(gameItem, 32.5f, isComestible: true);
		AudioHelper.InitProduce(gameItem);
		return gameItem;
	}

	private GameItem NightmareCap()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "nightmare_cap");
		gameItem.name = LocHelper.GetLocalizedItem("item_nightmare_cap_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_nightmare_cap_desc");
		gameItem.unitValue = 26L;
		gameItem.SetGameItemType("CHEMICAL_SUPPLIES");
		gameItem.SetGameItemType("POISON");
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		ChemicalCompositionHelper.SetNightmareCapComposition(gameItem, 32.5f);
		AudioHelper.InitProduce(gameItem);
		PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "dream_cap_seed", "farm_module_nightmare_cap");
		return gameItem;
	}

	private GameItem PlantAnalyser()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "plant_analyser");
		gameItem.name = LocHelper.GetLocalizedItem("item_plant_analyser_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_plant_analyser_desc");
		gameItem.SetGameItemType("FARMING_SUPPLIES");
		gameItem.SetGameItemType("TOOL");
		AudioHelper.InitScanner(gameItem);
		gameItem.SetValue(200L);
		gameItem.EnableTag("CAN_USE_ON_NON_OWNED_TAG");
		gameItem.maySelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		gameItem.canSelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		gameItem.mayItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (!GeneralHelper.IsItemOwned(s))
			{
				return false;
			}
			return (t.IsTag("SEED") && !t.IsTag("CONTAINER_TAG")) ? true : false;
		};
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => true;
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (t.IsTag("SEED"))
			{
				PlantAnalyserUIManager.Instance.OpenUI(t);
				AudioManager.Instance.Play3("scanner_activate");
			}
		};
		return gameItem;
	}

	private GameItem FillerweedSeed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "fillerweed_seed");
		gameItem.name = LocHelper.GetLocalizedItem("item_fillerweed_seed_name");
		HydroponicHelper.InitSeedItem(gameItem, "fillerweed_seed");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_fillerweed_seed_desc");
		gameItem.SetGameItemType("SEED");
		gameItem.unitValue = 9L;
		return gameItem;
	}

	private GameItem Fillerweed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "fillerweed");
		gameItem.name = LocHelper.GetLocalizedItem("item_fillerweed_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_fillerweed_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_fillerweed_flavor");
		gameItem.unitValue = 11L;
		gameItem.SetGameItemType("FOOD");
		FoodItemHelper.InitFoodItem(gameItem, 259, isCanned: false);
		AudioHelper.InitLeafy(gameItem);
		PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "fillerweed_seed");
		return gameItem;
	}

	private GameItem HydroreedSeed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "hydroreed_seed");
		gameItem.name = LocHelper.GetLocalizedItem("item_hydroreed_seed_name");
		HydroponicHelper.InitSeedItem(gameItem, "hydroreed_seed");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_hydroreed_seed_desc");
		gameItem.SetGameItemType("SEED");
		gameItem.unitValue = 20L;
		return gameItem;
	}

	private GameItem PlantDnaDisk()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "dna_disk_black_empty");
		gameItem.unitValue = 140L;
		PlantGeneticsHelper.InitDisk(gameItem);
		AudioHelper.InitTinyItem(gameItem);
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, new Action<GameItem>(PlantGeneticsHelper.UpdateDiskDisplay));
		return gameItem;
	}

	private GameItem BloomberrySeed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "bloomberry_seed");
		gameItem.name = LocHelper.GetLocalizedItem("item_bloomberry_seed_name");
		gameItem.EnableManufacturer("pastel");
		HydroponicHelper.InitSeedItem(gameItem, "bloomberry_seed");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_bloomberry_seed_desc");
		gameItem.SetGameItemType("SEED");
		gameItem.unitValue = 40L;
		return gameItem;
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

public static class AquacultureItems
{
	public const string ATLAS = "Items/items_aquaculture";

	private const string FISH_MEAT_GRADE = "4";

	public const string TINY_FISH_FILLET = "tiny_fish_fillet";

	public const string FISH_SCRAP = "fish_scrap";

	public const string SMALL_FISH_FILLET = "small_fish_fillet";

	public const string FISH_FILLET = "fish_fillet";

	public const string LARGE_FISH_FILLET = "large_fish_fillet";

	public const string SMALL_EEL_FILLET = "small_eel_fillet";

	public const string EEL_FILLET = "eel_fillet";

	public const string CAVIAR = "caviar";

	public const string SKIN_GRAFT = "tilapia_skin_graft";

	public const string STURGEON_ROE = "sturgeon_roe";

	public const int TINY_FISH_FILLET_KCAL = 100;

	public const int FISH_SCRAP_KCAL = 100;

	public const int SMALL_FISH_FILLET_KCAL = 200;

	public const int FISH_FILLET_KCAL = 400;

	public const int LARGE_FISH_FILLET_KCAL = 900;

	public const int SMALL_EEL_FILLET_KCAL = 250;

	public const int EEL_FILLET_KCAL = 500;

	public const int SKIN_GRAFT_GRAMS = 200;

	public const int SKIN_GRAFT_VALUE = 24;

	public const int SKIN_GRAFT_HEAL_POINTS = 2;

	public const int MEDICAL_ROE_HEAL_POINTS = 3;

	public const int EGG_JAR_GRAMS = 100;

	public const int EGG_JAR_KCAL = 225;

	public const int TINY_FISH_FILLET_VALUE = 13;

	public const int FISH_SCRAP_VALUE = 4;

	public const int SMALL_FISH_FILLET_VALUE = 26;

	public const int FISH_FILLET_VALUE = 57;

	public const int LARGE_FISH_FILLET_VALUE = 143;

	public const int SMALL_EEL_FILLET_VALUE = 68;

	public const int EEL_FILLET_VALUE = 136;

	public const int CAVIAR_VALUE = 240;

	public const int STURGEON_ROE_VALUE = 120;

	public const string GENERIC_FISH_MEAT_TYPE = "FISH";

	public static readonly (string id, int kcal)[] TILAPIA_MEAT = new(string, int)[4]
	{
		("large_fish_fillet", 900),
		("fish_fillet", 400),
		("small_fish_fillet", 200),
		("tiny_fish_fillet", 100)
	};

	public static readonly (string id, int kcal)[] EEL_MEAT = new(string, int)[3]
	{
		("eel_fillet", 500),
		("small_eel_fillet", 250),
		("tiny_fish_fillet", 100)
	};

	public const int EEL_JUVENILE_PRICE = 100;

	public const int ELECTRIC_EEL_JUVENILE_PRICE = 100;

	public const int STURGEON_ADOLESCENT_PRICE = 500;

	public const int FISH_SCRAP_MIN_KCAL = 25;

	public static List<string> BreakDownMeat(FishSpecies species, int kcal)
	{
		List<string> list = new List<string>();
		(string, int)[] array = species?.meatItems ?? TILAPIA_MEAT;
		for (int i = 0; i < array.Length; i++)
		{
			var (item, num) = array[i];
			while (kcal >= num)
			{
				list.Add(item);
				kcal -= num;
			}
		}
		return list;
	}

	public static int GetMeatKcal(string identifier)
	{
		return identifier switch
		{
			"tiny_fish_fillet" => 100, 
			"fish_scrap" => 100, 
			"small_fish_fillet" => 200, 
			"fish_fillet" => 400, 
			"large_fish_fillet" => 900, 
			"small_eel_fillet" => 250, 
			"eel_fillet" => 500, 
			_ => 0, 
		};
	}

	public static int GetMeatValue(string identifier)
	{
		return identifier switch
		{
			"tiny_fish_fillet" => 13, 
			"fish_scrap" => 4, 
			"small_fish_fillet" => 26, 
			"fish_fillet" => 57, 
			"large_fish_fillet" => 143, 
			"small_eel_fillet" => 68, 
			"eel_fillet" => 136, 
			_ => 0, 
		};
	}

	private static GameItem Fish(FishSpecies species, string name = null)
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetName(name ?? species.GetDisplayName()).SetSpriteAndShape(species.spriteAtlas, species.stageSprites[2]);
		gameItem.unitValue = 0L;
		string flavorText = species.GetFlavorText();
		if (!string.IsNullOrEmpty(flavorText))
		{
			gameItem.flavorText = flavorText;
		}
		AquacultureHelper.InitFishItem(gameItem, species);
		AudioHelper.InitMeaty(gameItem);
		return gameItem;
	}

	public static GameItem Tilapia()
	{
		return Fish(FishSpecies.Tilapia);
	}

	public static GameItem GutterTilapia()
	{
		return Fish(FishSpecies.GutterTilapia);
	}

	public static GameItem GraftTilapia()
	{
		return Fish(FishSpecies.GraftTilapia);
	}

	public static GameItem Eel()
	{
		GameItem gameItem = Fish(FishSpecies.Eel);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_eel_desc");
		return gameItem;
	}

	public static GameItem ElectricEel()
	{
		GameItem gameItem = Fish(FishSpecies.ElectricEel);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_electric_eel_desc");
		return gameItem;
	}

	public static GameItem Sturgeon()
	{
		GameItem gameItem = Fish(FishSpecies.Sturgeon);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sturgeon_desc");
		return gameItem;
	}

	private static GameItem Roe(FishSpecies species, int value)
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetName(species.GetRoeName(fertilized: false)).SetSpriteAndShape(species.spriteAtlas, species.stageSprites[0]);
		gameItem.SetGameItemType("FOOD");
		FoodItemHelper.InitFoodItem(gameItem, 60, isCanned: false);
		gameItem.unitValue = value;
		AquacultureHelper.InitRoeItem(gameItem, species);
		AudioHelper.InitProduce(gameItem);
		return gameItem;
	}

	public static GameItem TilapiaRoe()
	{
		return Roe(FishSpecies.Tilapia, 30);
	}

	public static GameItem GutterTilapiaRoe()
	{
		return Roe(FishSpecies.GutterTilapia, 15);
	}

	public static GameItem GraftTilapiaRoe()
	{
		GameItem gameItem = Roe(FishSpecies.GraftTilapia, 60);
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_graft_tilapia_roe_flavor");
		InitMedicalRoe(gameItem);
		return gameItem;
	}

	private static void InitMedicalRoe(GameItem item)
	{
		item.RemoveGameItemType("FOOD");
		item.SetGameItemType("MEDICAL");
		item.SetGameItemType("FOOD");
		item.EnableTag("ITEM_MEDICAL_TAG");
		item.canActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => CanUse(s);
		item.mayActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => CanUse(s);
		item.onActivateSlotItemFunc = delegate(GameItem s, GameInventory _, SlotMarker _)
		{
			if (CanUse(s))
			{
				PlayerStore.Instance.healthData.isWoundStable = true;
				PlayerStore.Instance.healthData.woundState = Mathf.Max(0, PlayerStore.Instance.healthData.woundState - 3);
				s.Destroy();
				PlayerStore.Instance.healthData.OnHealingItemUsed();
			}
		};
		static bool CanUse(GameItem roe)
		{
			if (GeneralHelper.IsItemOwned(roe) && !roe.IsTag("FISH_FERTILIZED_TAG"))
			{
				return !PlayerStore.Instance.healthData.isWoundStable;
			}
			return false;
		}
	}

	private static GameItem Fry(FishSpecies species)
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetName(species.GetFryName()).SetSpriteAndShape(species.spriteAtlas, species.stageSprites[1]);
		gameItem.unitValue = 0L;
		AquacultureHelper.InitFryItem(gameItem, species);
		AudioHelper.InitProduce(gameItem);
		return gameItem;
	}

	public static GameItem TilapiaFry()
	{
		return Fry(FishSpecies.Tilapia);
	}

	public static GameItem GutterTilapiaFry()
	{
		return Fry(FishSpecies.GutterTilapia);
	}

	public static GameItem GraftTilapiaFry()
	{
		return Fry(FishSpecies.GraftTilapia);
	}

	public static GameItem TilapiaSkinGraft()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetName(LocHelper.GetLocalizedItem("item_tilapia_skin_graft_name")).SetSpriteAndShape("Items/items_medical", "bandage_fish");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_tilapia_skin_graft_desc");
		gameItem.canActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s) && !PlayerStore.Instance.healthData.isWoundStable;
		gameItem.mayActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s) && !PlayerStore.Instance.healthData.isWoundStable;
		gameItem.onActivateSlotItemFunc = delegate(GameItem s, GameInventory _, SlotMarker _)
		{
			PlayerStore.Instance.healthData.isWoundStable = true;
			PlayerStore.Instance.healthData.woundState = Mathf.Max(0, PlayerStore.Instance.healthData.woundState - 2);
			s.Destroy();
			PlayerStore.Instance.healthData.OnHealingItemUsed();
		};
		gameItem.EnableProficiency("MEDICAL_PROFICIENCY");
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		gameItem.SetGameItemType("MEDICAL");
		gameItem.unitValue = 24L;
		AudioHelper.InitTextile(gameItem);
		return gameItem;
	}

	public static GameItem SpawnFertilizedRoe(FishSpecies species)
	{
		GameItem gameItem = ItemSpawner.Spawn(species.roeIdentifier);
		if (gameItem != null)
		{
			AquacultureHelper.SetFertilized(gameItem);
		}
		return gameItem;
	}

	public static GameItem SpawnFish(FishSpecies species, string sex, int age)
	{
		GameItem gameItem = ItemSpawner.Spawn(species.fishIdentifier);
		if (gameItem == null)
		{
			return null;
		}
		gameItem.ModifyTag("FISH_SEX_TAG", delegate(TagState state)
		{
			state.Enable().SetString(sex);
		});
		gameItem.ModifyTag("FISH_AGE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(age);
		});
		AquacultureHelper.SeedKcalEatenForAge(gameItem, species);
		AquacultureHelper.UpdateNameAndSprite(gameItem);
		return gameItem;
	}

	private static GameItem Meat(string name, string atlas, string sprite, int value, int kcal)
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetName(name).SetSpriteAndShape(atlas, sprite);
		AudioHelper.InitFoodAudio(gameItem);
		gameItem.unitValue = value;
		gameItem.SetGameItemType("FOOD");
		FoodItemHelper.InitFoodItem(gameItem, kcal, isCanned: false, isMeat: true);
		gameItem.ModifyTag("MEAT_TYPE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("FISH");
		});
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			if (GutterflowPelletHelper.IsTainted(loaded))
			{
				GutterflowPelletHelper.StripFoodType(loaded);
			}
		});
		return gameItem;
	}

	public static GameItem TinyFishFillet()
	{
		return Meat(LocHelper.GetLocalizedItem("item_tiny_fish_fillet_name"), "Items/items_aquaculture", "meat_fish_tiny4", 13, 100);
	}

	public static GameItem FishScrap()
	{
		GameItem gameItem = Meat(LocHelper.GetLocalizedItem("item_fish_scrap_name"), "Items/items_aquaculture", "fishscrap", 4, 100);
		gameItem.RemoveGameItemType("FOOD");
		return gameItem;
	}

	public static int GetLeftoverScrapKcal(FishSpecies species, int kcal)
	{
		int num = 0;
		foreach (string item in BreakDownMeat(species, kcal))
		{
			num += GetMeatKcal(item);
		}
		int num2 = kcal - num;
		if (num2 < 25)
		{
			return 0;
		}
		return num2;
	}

	public static int GetFishScrapValue(int kcal)
	{
		return Mathf.Max(1, Mathf.RoundToInt((float)kcal * 4f / 100f));
	}

	public static GameItem SpawnFishScrap(int kcal)
	{
		GameItem gameItem = ItemSpawner.Spawn("fish_scrap");
		if (gameItem == null)
		{
			return null;
		}
		gameItem.ModifyTag("CALORIE_VALUE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(kcal);
		});
		gameItem.unitValue = GetFishScrapValue(kcal);
		return gameItem;
	}

	public static GameItem SmallFishFillet()
	{
		return Meat(LocHelper.GetLocalizedItem("item_small_fish_fillet_name"), "Items/items_aquaculture", "meat_fish_small4", 26, 200);
	}

	public static GameItem FishFillet()
	{
		return Meat(LocHelper.GetLocalizedItem("item_fish_fillet_name"), "Items/items_aquaculture", "meat_fish4", 57, 400);
	}

	public static GameItem LargeFishFillet()
	{
		GameItem gameItem = Meat(LocHelper.GetLocalizedItem("item_large_fish_fillet_name"), "Items/items_aquaculture", "meat_fish_large4", 143, 900);
		gameItem.SetGameItemType("LUXURY_ITEM");
		return gameItem;
	}

	private static GameItem EggJar(string name, string sprite, int value)
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetName(name).SetSpriteAndShape("Items/items_aquaculture", sprite);
		AudioHelper.InitFoodAudio(gameItem);
		gameItem.unitValue = value;
		gameItem.SetGameItemType("FOOD");
		gameItem.SetGameItemType("LUXURY_ITEM");
		FoodItemHelper.InitFoodItem(gameItem, 225, isCanned: false, isMeat: true);
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			if (GutterflowPelletHelper.IsTainted(loaded))
			{
				GutterflowPelletHelper.StripFoodType(loaded);
			}
		});
		return gameItem;
	}

	public static GameItem Caviar()
	{
		return EggJar(LocHelper.GetLocalizedItem("item_caviar_name"), "caviar", 240);
	}

	public static GameItem SturgeonRoe()
	{
		return EggJar(LocHelper.GetLocalizedItem("item_sturgeon_roe_name"), "roe_sturgeon", 120);
	}

	public static GameItem SmallEelFillet()
	{
		return EelMeat(LocHelper.GetLocalizedItem("item_small_eel_fillet_name"), "meat_eel_tiny", 68, 250);
	}

	public static GameItem EelFillet()
	{
		return EelMeat(LocHelper.GetLocalizedItem("item_eel_fillet_name"), "meat_eel_small", 136, 500);
	}

	private static GameItem EelMeat(string name, string sprite, int value, int kcal)
	{
		GameItem gameItem = Meat(name, "Items/items_aquaculture", sprite, value, kcal);
		gameItem.SetGameItemType("LUXURY_ITEM");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_eel_fillet_flavor");
		return gameItem;
	}
}

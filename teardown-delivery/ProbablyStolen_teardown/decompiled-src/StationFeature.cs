using System.Collections.Generic;

public class StationFeature : TagSystem
{
	public const string IS_CRISIS_TAG = "IS_CRISIS_TAG";

	public string identifier;

	public string displayName;

	public List<string> discountedItemTagList;

	public List<int> discountedItemDiscountList;

	public int odd;

	public bool spawnBuilding;

	public List<string> incompatibleFeature;

	public static List<StationFeature> TraderStationFeature = new List<StationFeature>
	{
		CreateEnergyFarm(),
		CreateNutrifruitFarm(),
		CreateFoundry(),
		CreateCivilUnrest(),
		CreateCivilWar(),
		CreateFuelShortage(),
		CreateFuelSurplus(),
		CreateFoodShortage(),
		CreateFoodSurplus(),
		CreateMedShortage(),
		CreateMedSurplus()
	};

	public static List<int> FeatureOddTable = null;

	public const string ENERGY_FARM = "ENERGY_FARM";

	public const string NUTRIFRUIT_FARM = "NUTRIFRUIT_FARM";

	public const string FOUNDRY = "FOUNDRY";

	public const string CIVIL_UNREST = "CIVIL_UNREST";

	public const string CIVIL_WAR = "CIVIL_WAR";

	public const string FUEL_SHORTAGE = "FUEL_SHORTAGE";

	public const string FUEL_SURPLUS = "FUEL_SURPLUS";

	public const string FOOD_SHORTAGE = "FOOD_SHORTAGE";

	public const string FOOD_SURPLUS = "FOOD_SURPLUS";

	public const string MED_SHORTAGE = "MED_SHORTAGE";

	public const string MED_SURPLUS = "MED_SURPLUS";

	public const string BLOCKADE = "BLOCKADE";

	public const string FREE_PORT = "FREE_PORT";

	public const string NON_BUILDING_WILDCARD = "NON_BUILDING_WILDCARD";

	public StationFeature()
	{
		identifier = "";
		displayName = "";
		discountedItemTagList = new List<string>();
		discountedItemDiscountList = new List<int>();
		spawnBuilding = false;
		incompatibleFeature = new List<string>();
	}

	public static void InitCrisisFeature(StationFeature feature)
	{
		feature.EnableTag("IS_CRISIS_TAG");
	}

	public static void GenerateOddTable()
	{
		FeatureOddTable = new List<int>();
		foreach (StationFeature item in TraderStationFeature)
		{
			FeatureOddTable.Add(item.odd);
		}
	}

	public static StationFeature PickSpecialFeature()
	{
		if (RNG.Roll(20))
		{
			return CreateBlockade();
		}
		if (RNG.Roll(20))
		{
			return CreateFreePort();
		}
		return null;
	}

	public static StationFeature PickRandomFeature()
	{
		if (FeatureOddTable == null)
		{
			GenerateOddTable();
		}
		int index = RNG.GetOdd(FeatureOddTable);
		return TraderStationFeature[index];
	}

	public static StationFeature PickCompatibleFeature(List<StationFeature> currentFeatures)
	{
		List<StationFeature> list = new List<StationFeature>();
		List<string> list2 = new List<string>();
		foreach (StationFeature currentFeature in currentFeatures)
		{
			list2.AddRange(currentFeature.incompatibleFeature);
		}
		foreach (StationFeature item in TraderStationFeature)
		{
			if (!list2.Contains(item.identifier))
			{
				list.Add(item);
			}
		}
		List<int> list3 = new List<int>();
		foreach (StationFeature item2 in list)
		{
			list3.Add(item2.odd);
		}
		int index = RNG.GetOdd(list3);
		return list[index];
	}

	public static StationFeature CreateEnergyFarm()
	{
		return new StationFeature
		{
			identifier = "ENERGY_FARM",
			displayName = "Energy Farm",
			odd = 50,
			spawnBuilding = true,
			discountedItemTagList = new List<string> { "power_source_item" },
			discountedItemDiscountList = new List<int> { -25 }
		};
	}

	public static StationFeature CreateFoundry()
	{
		return new StationFeature
		{
			identifier = "FOUNDRY",
			displayName = "Foundry",
			odd = 50,
			spawnBuilding = true,
			discountedItemTagList = new List<string> { "ORE_TAG", "SCRAP_METAL", "MATERIAL_TAG", "CONSTRUCTION_MATERIAL_TAG" },
			discountedItemDiscountList = new List<int> { 25, 25, -25, -25 }
		};
	}

	public static StationFeature CreateNutrifruitFarm()
	{
		return new StationFeature
		{
			identifier = "NUTRIFRUIT_FARM",
			displayName = "Nutrifruit Farm",
			odd = 50,
			spawnBuilding = true,
			discountedItemTagList = new List<string> { "FOOD" },
			discountedItemDiscountList = new List<int> { -25 }
		};
	}

	public static StationFeature CreateCivilUnrest()
	{
		StationFeature obj = new StationFeature
		{
			identifier = "CIVIL_UNREST",
			displayName = "Civil Unrest",
			odd = 10,
			spawnBuilding = false
		};
		InitCrisisFeature(obj);
		obj.discountedItemTagList = new List<string> { "FOOD", "LIQUID_CONTAINER_TAG", "ITEM_MEDICAL_TAG" };
		obj.discountedItemDiscountList = new List<int> { 33, 33, 33 };
		obj.incompatibleFeature = new List<string> { "CIVIL_UNREST", "CIVIL_WAR" };
		return obj;
	}

	public static StationFeature CreateCivilWar()
	{
		StationFeature obj = new StationFeature
		{
			identifier = "CIVIL_WAR",
			displayName = "Civil War",
			odd = 5,
			spawnBuilding = false
		};
		InitCrisisFeature(obj);
		obj.discountedItemTagList = new List<string> { "FOOD", "LIQUID_CONTAINER_TAG", "ITEM_MEDICAL_TAG" };
		obj.discountedItemDiscountList = new List<int> { 66, 66, 66 };
		obj.incompatibleFeature = new List<string> { "CIVIL_WAR", "CIVIL_UNREST" };
		return obj;
	}

	public static StationFeature CreateFuelShortage()
	{
		StationFeature obj = new StationFeature
		{
			identifier = "FUEL_SHORTAGE",
			displayName = "Fuel Shortage",
			odd = 50,
			spawnBuilding = false
		};
		InitCrisisFeature(obj);
		obj.discountedItemTagList = new List<string> { "EXOTIC_FUEL_TAG" };
		obj.discountedItemDiscountList = new List<int> { 33 };
		obj.incompatibleFeature = new List<string> { "FUEL_SHORTAGE", "FUEL_SURPLUS" };
		return obj;
	}

	public static StationFeature CreateFuelSurplus()
	{
		return new StationFeature
		{
			identifier = "FUEL_SURPLUS",
			displayName = "Fuel Surplus",
			odd = 50,
			spawnBuilding = false,
			discountedItemTagList = new List<string> { "EXOTIC_FUEL_TAG" },
			discountedItemDiscountList = new List<int> { -33 },
			incompatibleFeature = new List<string> { "FUEL_SURPLUS", "FUEL_SHORTAGE" }
		};
	}

	public static StationFeature CreateFoodShortage()
	{
		StationFeature obj = new StationFeature
		{
			identifier = "FOOD_SHORTAGE",
			displayName = "Food Shortage",
			odd = 50,
			spawnBuilding = false
		};
		InitCrisisFeature(obj);
		obj.discountedItemTagList = new List<string> { "FOOD", "LIQUID_CONTAINER_TAG" };
		obj.discountedItemDiscountList = new List<int> { 33, 33 };
		obj.incompatibleFeature = new List<string> { "FOOD_SHORTAGE", "FOOD_SURPLUS" };
		return obj;
	}

	public static StationFeature CreateFoodSurplus()
	{
		return new StationFeature
		{
			identifier = "FOOD_SURPLUS",
			displayName = "Food Surplus",
			odd = 50,
			spawnBuilding = false,
			discountedItemTagList = new List<string> { "FOOD", "LIQUID_CONTAINER_TAG" },
			discountedItemDiscountList = new List<int> { -33, -33 },
			incompatibleFeature = new List<string> { "FOOD_SURPLUS", "FOOD_SHORTAGE" }
		};
	}

	public static StationFeature CreateMedShortage()
	{
		StationFeature obj = new StationFeature
		{
			identifier = "MED_SHORTAGE",
			displayName = "Med Shortage",
			odd = 50,
			spawnBuilding = false
		};
		InitCrisisFeature(obj);
		obj.discountedItemTagList = new List<string> { "ITEM_MEDICAL_TAG" };
		obj.discountedItemDiscountList = new List<int> { 33 };
		obj.incompatibleFeature = new List<string> { "MED_SHORTAGE", "MED_SURPLUS" };
		return obj;
	}

	public static StationFeature CreateMedSurplus()
	{
		return new StationFeature
		{
			identifier = "MED_SURPLUS",
			displayName = "Med Surplus",
			odd = 50,
			spawnBuilding = false,
			discountedItemTagList = new List<string> { "ITEM_MEDICAL_TAG" },
			discountedItemDiscountList = new List<int> { -33 },
			incompatibleFeature = new List<string> { "MED_SURPLUS", "MED_SHORTAGE" }
		};
	}

	public static StationFeature CreateBlockade()
	{
		StationFeature obj = new StationFeature
		{
			identifier = "BLOCKADE",
			displayName = "Blockade",
			odd = 5,
			spawnBuilding = false
		};
		InitCrisisFeature(obj);
		obj.discountedItemTagList = new List<string> { "FOOD", "LIQUID_CONTAINER_TAG", "ITEM_MEDICAL_TAG" };
		obj.discountedItemDiscountList = new List<int> { 200, 200, 200 };
		obj.incompatibleFeature = new List<string> { "NON_BUILDING_WILDCARD" };
		return obj;
	}

	public static StationFeature CreateFreePort()
	{
		return new StationFeature
		{
			identifier = "FREE_PORT",
			displayName = "Free Port",
			odd = 5,
			spawnBuilding = false,
			discountedItemTagList = new List<string>(),
			discountedItemDiscountList = new List<int>(),
			incompatibleFeature = new List<string>()
		};
	}
}

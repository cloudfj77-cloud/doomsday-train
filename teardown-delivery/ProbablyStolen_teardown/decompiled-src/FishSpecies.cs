using System.Collections.Generic;

public class FishSpecies
{
	public string speciesID;

	public string nameKey;

	public string flavorKey;

	public string roeNameKey;

	public string roeFertilizedNameKey;

	public string fryNameKey;

	public string displayName;

	public string flavorText;

	public string roeIdentifier;

	public string fryIdentifier;

	public string fishIdentifier;

	public int groupCount = 6;

	public int[] stageStartDays;

	public int[] stageWeightGrams;

	public int[] stageWaterNeedMl;

	public int[] stagePowerPerNight;

	public int oldAgeDays;

	public float maintenanceKcalPer100gPerNight;

	public float growthKcalPerGram;

	public float conversionEfficiency;

	public float maxHunger;

	public float maxHealth;

	public float wasteMlPer100gPerNight;

	public FishDiet diet;

	public float meatGrowthBoost;

	public const float TILAPIA_MEAT_GROWTH_BOOST = 0.25f;

	public bool ignoresWaste;

	public bool pelletProof;

	public float wasteSensitivity = 1f;

	public int skinGraftGrams;

	public bool roeIsMedical;

	public float remediationMlPer100gPerNight;

	public int breedChancePercent;

	public float breedHungerRatio;

	public (string id, int kcal)[] meatItems;

	public string fixedSex;

	public float eggGramsPerNight;

	public float eggLoadBodyRatio;

	public float eggKcalPerGram;

	public float surgeryHealthCostFull;

	public float surgeryHealthCostEarly;

	public float eggRegrowHealthRatio;

	public int surgeryCaviarJars;

	public int surgeryMixedJars;

	public string spriteAtlas = "Items/items_aquaculture";

	public string[] stageSprites;

	public string fertilizedRoeSprite;

	public const string SPRITE_ATLAS = "Items/items_aquaculture";

	public static readonly FishSpecies Tilapia = new FishSpecies
	{
		speciesID = "FISH_TILAPIA",
		nameKey = "item_tilapia_name",
		flavorKey = "item_tilapia_flavor",
		roeNameKey = "item_tilapia_roe_name",
		roeFertilizedNameKey = "item_tilapia_roe_fertilized_name",
		fryNameKey = "item_tilapia_fry_name",
		roeIdentifier = "tilapia_roe",
		fryIdentifier = "tilapia_fry",
		fishIdentifier = "tilapia",
		groupCount = 6,
		stageStartDays = new int[6] { 0, 1, 3, 5, 8, 13 },
		stageWeightGrams = new int[6] { 0, 0, 50, 200, 450, 600 },
		stageWaterNeedMl = new int[6] { 100, 100, 100, 350, 750, 1000 },
		oldAgeDays = 27,
		maintenanceKcalPer100gPerNight = 2f,
		growthKcalPerGram = 1.2f,
		conversionEfficiency = 0.9f,
		meatItems = AquacultureItems.TILAPIA_MEAT,
		maxHunger = 700f,
		maxHealth = 100f,
		wasteMlPer100gPerNight = 0.5f,
		diet = FishDiet.Omnivore,
		meatGrowthBoost = 0.25f,
		breedChancePercent = 15,
		breedHungerRatio = 0.5f,
		stageSprites = new string[6] { "roe", "fries", "tilapia_small1", "tilapia1", "tilapia_large1", "tilapia_large1" },
		fertilizedRoeSprite = "roe_fertilized"
	};

	public static readonly FishSpecies GutterTilapia = new FishSpecies
	{
		speciesID = "FISH_GUTTER_TILAPIA",
		nameKey = "item_gutter_tilapia_name",
		flavorKey = "item_gutter_tilapia_flavor",
		roeNameKey = "item_gutter_tilapia_roe_name",
		roeFertilizedNameKey = "item_gutter_tilapia_roe_fertilized_name",
		fryNameKey = "item_gutter_tilapia_fry_name",
		roeIdentifier = "gutter_tilapia_roe",
		fryIdentifier = "gutter_tilapia_fry",
		fishIdentifier = "gutter_tilapia",
		groupCount = 6,
		stageStartDays = new int[6] { 0, 1, 3, 6, 10, 16 },
		stageWeightGrams = new int[6] { 0, 0, 50, 200, 450, 600 },
		stageWaterNeedMl = new int[6] { 100, 100, 100, 350, 750, 1000 },
		oldAgeDays = 33,
		maintenanceKcalPer100gPerNight = 2f,
		growthKcalPerGram = 1.2f,
		conversionEfficiency = 0.85f,
		meatItems = AquacultureItems.TILAPIA_MEAT,
		maxHunger = 700f,
		maxHealth = 100f,
		wasteMlPer100gPerNight = 0.5f,
		diet = FishDiet.Omnivore,
		meatGrowthBoost = 0.25f,
		ignoresWaste = true,
		pelletProof = true,
		remediationMlPer100gPerNight = 0.5f,
		breedChancePercent = 15,
		breedHungerRatio = 0.5f,
		stageSprites = new string[6] { "roe3", "fries", "tilapia_small3", "tilapia3", "tilapia_large3", "tilapia_large3" },
		fertilizedRoeSprite = "roe_fertilized3"
	};

	public static readonly FishSpecies GraftTilapia = new FishSpecies
	{
		speciesID = "FISH_GRAFT_TILAPIA",
		nameKey = "item_graft_tilapia_name",
		flavorKey = "item_graft_tilapia_flavor",
		roeNameKey = "item_graft_tilapia_roe_name",
		roeFertilizedNameKey = "item_graft_tilapia_roe_fertilized_name",
		fryNameKey = "item_graft_tilapia_fry_name",
		roeIdentifier = "graft_tilapia_roe",
		fryIdentifier = "graft_tilapia_fry",
		fishIdentifier = "graft_tilapia",
		groupCount = 6,
		stageStartDays = new int[6] { 0, 1, 3, 5, 8, 13 },
		stageWeightGrams = new int[6] { 0, 0, 50, 200, 450, 600 },
		stageWaterNeedMl = new int[6] { 100, 100, 100, 350, 750, 1000 },
		oldAgeDays = 27,
		maintenanceKcalPer100gPerNight = 2f,
		growthKcalPerGram = 1.2f,
		conversionEfficiency = 0.7f,
		meatItems = AquacultureItems.TILAPIA_MEAT,
		maxHunger = 700f,
		maxHealth = 100f,
		wasteMlPer100gPerNight = 1f,
		diet = FishDiet.Omnivore,
		meatGrowthBoost = 0.25f,
		wasteSensitivity = 2f,
		skinGraftGrams = 200,
		roeIsMedical = true,
		breedChancePercent = 15,
		breedHungerRatio = 0.5f,
		stageSprites = new string[6] { "roe2", "fries", "tilapia_small2", "tilapia2", "tilapia_large2", "tilapia_large2" },
		fertilizedRoeSprite = "roe_fertilized2"
	};

	public static readonly FishSpecies Eel = new FishSpecies
	{
		speciesID = "FISH_EEL",
		nameKey = "item_eel_name",
		roeIdentifier = null,
		fryIdentifier = null,
		fishIdentifier = "eel",
		groupCount = 0,
		stageStartDays = new int[6] { 0, 0, 0, 5, 10, 18 },
		stageWeightGrams = new int[6] { 0, 0, 150, 600, 1400, 2400 },
		stageWaterNeedMl = new int[6] { 0, 0, 200, 600, 1200, 2000 },
		oldAgeDays = 36,
		maintenanceKcalPer100gPerNight = 3f,
		growthKcalPerGram = 1.2f,
		conversionEfficiency = 0.9f,
		meatItems = AquacultureItems.EEL_MEAT,
		maxHunger = 800f,
		maxHealth = 120f,
		wasteMlPer100gPerNight = 2f,
		diet = FishDiet.Carnivore,
		breedChancePercent = 0,
		breedHungerRatio = 1f,
		stageSprites = new string[6] { null, null, "eel_small1", "eel1", "eel_large1", "eel_large1" }
	};

	public static readonly FishSpecies ElectricEel = new FishSpecies
	{
		speciesID = "FISH_ELECTRIC_EEL",
		nameKey = "item_electric_eel_name",
		roeIdentifier = null,
		fryIdentifier = null,
		fishIdentifier = "electric_eel",
		groupCount = 0,
		stageStartDays = new int[6] { 0, 0, 0, 5, 10, 18 },
		stageWeightGrams = new int[6] { 0, 0, 150, 600, 1400, 2400 },
		stageWaterNeedMl = new int[6] { 0, 0, 200, 600, 1200, 2000 },
		oldAgeDays = 36,
		stagePowerPerNight = new int[6] { 0, 0, 1, 2, 3, 3 },
		maintenanceKcalPer100gPerNight = 5.25f,
		growthKcalPerGram = 1.2f,
		conversionEfficiency = 0.75f,
		meatItems = AquacultureItems.EEL_MEAT,
		maxHunger = 1200f,
		maxHealth = 120f,
		wasteMlPer100gPerNight = 2f,
		diet = FishDiet.Carnivore,
		breedChancePercent = 0,
		breedHungerRatio = 1f,
		stageSprites = new string[6] { null, null, "eel_small2", "eel2", "eel_large2", "eel_large2" }
	};

	public static readonly FishSpecies Sturgeon = new FishSpecies
	{
		speciesID = "FISH_STURGEON",
		nameKey = "item_sturgeon_name",
		roeIdentifier = null,
		fryIdentifier = null,
		fishIdentifier = "sturgeon",
		groupCount = 0,
		fixedSex = "FEMALE",
		stageStartDays = new int[6] { 0, 0, 0, 10, 18, 34 },
		stageWeightGrams = new int[6] { 0, 0, 500, 1500, 3500, 5000 },
		stageWaterNeedMl = new int[6] { 0, 0, 500, 1500, 3000, 4000 },
		oldAgeDays = 80,
		maintenanceKcalPer100gPerNight = 2f,
		growthKcalPerGram = 1.2f,
		conversionEfficiency = 0.9f,
		meatItems = AquacultureItems.TILAPIA_MEAT,
		maxHunger = 800f,
		maxHealth = 150f,
		wasteMlPer100gPerNight = 1f,
		diet = FishDiet.Carnivore,
		breedChancePercent = 0,
		breedHungerRatio = 1f,
		eggGramsPerNight = 40f,
		eggLoadBodyRatio = 0.15f,
		eggKcalPerGram = 2.5f,
		surgeryHealthCostFull = 0.2f,
		surgeryHealthCostEarly = 0.3f,
		eggRegrowHealthRatio = 0.9f,
		surgeryCaviarJars = 10,
		surgeryMixedJars = 6,
		stageSprites = new string[6] { null, null, "sturgeon", "sturgeon", "sturgeon", "sturgeon" }
	};

	public static readonly Dictionary<string, FishSpecies> All = new Dictionary<string, FishSpecies>
	{
		{ Tilapia.speciesID, Tilapia },
		{ GutterTilapia.speciesID, GutterTilapia },
		{ GraftTilapia.speciesID, GraftTilapia },
		{ Eel.speciesID, Eel },
		{ ElectricEel.speciesID, ElectricEel },
		{ Sturgeon.speciesID, Sturgeon }
	};

	public bool LaysEggs => eggGramsPerNight > 0f;

	public bool CanBreed
	{
		get
		{
			if (breedChancePercent > 0)
			{
				return !string.IsNullOrEmpty(roeIdentifier);
			}
			return false;
		}
	}

	public string GetDisplayName()
	{
		if (nameKey == null)
		{
			return displayName;
		}
		return LocHelper.GetLocalizedItem(nameKey);
	}

	public string GetFlavorText()
	{
		if (nameKey == null)
		{
			return flavorText;
		}
		if (flavorKey == null)
		{
			return null;
		}
		return LocHelper.GetLocalizedItem(flavorKey);
	}

	public string GetRoeName(bool fertilized)
	{
		string text = (fertilized ? roeFertilizedNameKey : roeNameKey);
		if (text != null)
		{
			return LocHelper.GetLocalizedItem(text);
		}
		return GetDisplayName() + (fertilized ? " Roe (fertilized)" : " Roe");
	}

	public string GetFryName()
	{
		if (fryNameKey == null)
		{
			return GetDisplayName() + " Fry";
		}
		return LocHelper.GetLocalizedItem(fryNameKey);
	}

	public FishStage GetStage(int age)
	{
		int result = 0;
		for (int num = stageStartDays.Length - 1; num >= 0; num--)
		{
			if (age >= stageStartDays[num])
			{
				result = num;
				break;
			}
		}
		return (FishStage)result;
	}

	public static string GetStageDisplay(FishStage stage)
	{
		return stage switch
		{
			FishStage.Roe => LocHelper.GetLocalizedMechanic("husbandry_stage_roe"), 
			FishStage.Fry => LocHelper.GetLocalizedMechanic("husbandry_stage_fry"), 
			FishStage.Juvenile => LocHelper.GetLocalizedMechanic("husbandry_stage_juvenile"), 
			FishStage.Adolescent => LocHelper.GetLocalizedMechanic("husbandry_stage_adolescent"), 
			FishStage.Adult => LocHelper.GetLocalizedMechanic("husbandry_stage_adult"), 
			FishStage.Elder => LocHelper.GetLocalizedMechanic("husbandry_stage_elder"), 
			_ => LocHelper.GetLocalizedMechanic("husbandry_stage_unknown"), 
		};
	}
}

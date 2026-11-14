using System.Collections.Generic;

public class ItemConditionList
{
	public static List<ItemCondition> conditionOpenBox = new List<ItemCondition>
	{
		CreateOpenBox(),
		CreateFactorySealed()
	};

	public static List<ItemCondition> conditionRusted = new List<ItemCondition>
	{
		CreateRustFree(),
		CreateRusted()
	};

	public static List<ItemCondition> conditionDented = new List<ItemCondition>
	{
		CreateDented(),
		CreateNonDented()
	};

	public static List<ItemCondition> conditionExpired = new List<ItemCondition>
	{
		CreateExpired(),
		CreateNonExpired()
	};

	public static List<ItemCondition> conditionIngotPurity = new List<ItemCondition>
	{
		CreatePerfectQualityIngot(),
		CreateVeryHighQualityIngot(),
		CreateHighQualityIngot(),
		CreateFairQualityIngot(),
		CreateLowQualityIngot(),
		CreateVeryLowQualityIngot()
	};

	public static List<ItemCondition> conditionWaterPurity = new List<ItemCondition>
	{
		CreatePureWater(),
		CreateHighQualityWater(),
		CreateBaseWater(),
		CreateGhostWater(),
		CreateRustWater(),
		CreateGutterflow()
	};

	public static List<ItemCondition> conditionFoodFreshness = new List<ItemCondition>
	{
		CreateFoodVeryFresh(),
		CreateFoodFresh(),
		CreateFoodStale(),
		CreateFoodSpoiled()
	};

	public static List<ItemCondition> conditionGenuineCigarette = new List<ItemCondition>
	{
		CreateCounterfeitCigarette(),
		CreateGenuineCigarette()
	};

	public static List<ItemCondition> conditionGenuineStamp = new List<ItemCondition>
	{
		CreateCounterfeitStamp(),
		CreateGenuineStamp()
	};

	public static List<ItemCondition> conditionGenuineInjector = new List<ItemCondition>
	{
		CreateCounterfeitInjector(),
		CreateExpiredInjector(),
		CreateUnusableInjector(),
		CreateGenuineInjector()
	};

	public static List<ItemCondition> conditionModuleStuck = new List<ItemCondition>
	{
		CreateModuleNormal(),
		CreateModuleStuck()
	};

	public static List<ItemCondition> conditionFree = new List<ItemCondition>
	{
		CreateFreeFree(),
		CreateFreeNormal()
	};

	public static List<ItemCondition> conditionEquipmentCondition = new List<ItemCondition>
	{
		CreateEquipementConditionNew(),
		CreateEquipementConditionExcellent(),
		CreateEquipementConditionGood(),
		CreateEquipementConditionFair(),
		CreateEquipementConditionPoor()
	};

	public static List<ItemCondition> conditionReplica = new List<ItemCondition>
	{
		CreateReplica(),
		CreateNonReplica()
	};

	public static List<ItemCondition> conditionChemicalPurity = new List<ItemCondition>
	{
		CreateLabPure(),
		CreatePharmaGrade(),
		CreateNearPharma(),
		CreateStreetStandard(),
		CreateCutProduct(),
		CreateJunkGrade(),
		CreateStarchGrade()
	};

	public static List<ItemCondition> conditionWineQuality = new List<ItemCondition>
	{
		CreateMasterVintage(),
		CreateReserve(),
		CreateProductionWine(),
		CreateHomebrew(),
		CreateBucketBrew(),
		CreateUndrinkable()
	};

	public static Dictionary<string, List<ItemCondition>> conditionLookup = new Dictionary<string, List<ItemCondition>>
	{
		{ "CATEGORY_OPEN_BOX", conditionOpenBox },
		{ "CATEGORY_RUSTED", conditionRusted },
		{ "CATEGORY_DENTED", conditionDented },
		{ "CATEGORY_EXPIRED", conditionExpired },
		{ "CATEGORY_INGOT_PURITY", conditionIngotPurity },
		{ "CATEGORY_WATER_PURITY", conditionWaterPurity },
		{ "CATEGORY_GENUINE_CIGARETTE", conditionGenuineCigarette },
		{ "CATEGORY_GENUINE_STAMP", conditionGenuineStamp },
		{ "CATEGORY_GENUINE_INJECTOR", conditionGenuineInjector },
		{ "CATEGORY_FREE", conditionFree },
		{ "CATEGORY_EQUIPMENT_CONDITION", conditionEquipmentCondition },
		{ "CATEGORY_REPLICA", conditionReplica },
		{ "CATEGORY_CHEMICAL_PURITY", conditionChemicalPurity },
		{ "CATEGORY_WINE_QUALITY", conditionWineQuality }
	};

	public const int SHABBY_VALUE_MOD = -75;

	public const int SECOND_HAND_VALUE_MOD = -50;

	public static ItemCondition CreateFreeFree()
	{
		return new ItemCondition
		{
			category = "CATEGORY_FREE",
			display = LocHelper.GetLocalizedMechanic("mech_condition_free"),
			modValue = -999,
			identifier = "freeFree"
		};
	}

	public static ItemCondition CreateFreeNormal()
	{
		return new ItemCondition
		{
			category = "CATEGORY_FREE",
			display = LocHelper.GetLocalizedMechanic("mech_condition_normal"),
			modValue = 0,
			identifier = "freeNormal"
		};
	}

	public static ItemCondition CreateExpired()
	{
		return new ItemCondition
		{
			category = "CATEGORY_EXPIRED",
			display = LocHelper.GetLocalizedMechanic("mech_condition_expired"),
			modValue = -10,
			identifier = "expired"
		};
	}

	public static ItemCondition CreateNonExpired()
	{
		return new ItemCondition
		{
			category = "CATEGORY_EXPIRED",
			display = LocHelper.GetLocalizedMechanic("mech_condition_non_expired"),
			modValue = 0,
			identifier = "non_expired"
		};
	}

	public static ItemCondition CreateDented()
	{
		return new ItemCondition
		{
			category = "CATEGORY_DENTED",
			display = LocHelper.GetLocalizedMechanic("mech_condition_dented"),
			modValue = -5,
			identifier = "dented"
		};
	}

	public static ItemCondition CreateNonDented()
	{
		return new ItemCondition
		{
			category = "CATEGORY_DENTED",
			display = LocHelper.GetLocalizedMechanic("mech_condition_hidden"),
			modValue = 0,
			identifier = "non_dented",
			hiddenAsPublic = true
		};
	}

	public static ItemCondition CreateOpenBox()
	{
		return new ItemCondition
		{
			category = "CATEGORY_OPEN_BOX",
			display = LocHelper.GetLocalizedMechanic("mech_condition_open_box"),
			modValue = 0,
			identifier = "open_box"
		};
	}

	public static ItemCondition CreateFactorySealed()
	{
		return new ItemCondition
		{
			category = "CATEGORY_OPEN_BOX",
			display = LocHelper.GetLocalizedMechanic("mech_condition_factory_sealed"),
			modValue = 10,
			identifier = "factory_sealed"
		};
	}

	public static ItemCondition CreateRustFree()
	{
		return new ItemCondition
		{
			category = "CATEGORY_RUSTED",
			display = LocHelper.GetLocalizedMechanic("mech_condition_hidden"),
			modValue = 0,
			identifier = "rust_free",
			hiddenAsPublic = true
		};
	}

	public static ItemCondition CreateRusted()
	{
		return new ItemCondition
		{
			category = "CATEGORY_RUSTED",
			display = LocHelper.GetLocalizedMechanic("mech_condition_rusted"),
			modValue = -40,
			identifier = "rusted"
		};
	}

	public static ItemCondition CreatePerfectQualityIngot()
	{
		return new ItemCondition
		{
			category = "CATEGORY_INGOT_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_perfect_purity"),
			modValue = 200,
			identifier = "INGOT_PURITY_PERFECT"
		};
	}

	public static ItemCondition CreateVeryHighQualityIngot()
	{
		return new ItemCondition
		{
			category = "CATEGORY_INGOT_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_very_high_purity"),
			modValue = 50,
			identifier = "INGOT_PURITY_VERY_HIGH"
		};
	}

	public static ItemCondition CreateHighQualityIngot()
	{
		return new ItemCondition
		{
			category = "CATEGORY_INGOT_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_high_purity"),
			modValue = 25,
			identifier = "INGOT_PURITY_HIGH"
		};
	}

	public static ItemCondition CreateFairQualityIngot()
	{
		return new ItemCondition
		{
			category = "CATEGORY_INGOT_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_fair_purity"),
			modValue = 0,
			identifier = "INGOT_PURITY_FAIR"
		};
	}

	public static ItemCondition CreateLowQualityIngot()
	{
		return new ItemCondition
		{
			category = "CATEGORY_INGOT_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_low_purity"),
			modValue = -15,
			identifier = "INGOT_PURITY_LOW"
		};
	}

	public static ItemCondition CreateVeryLowQualityIngot()
	{
		return new ItemCondition
		{
			category = "CATEGORY_INGOT_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_very_low_purity"),
			modValue = -25,
			identifier = "INGOT_PURITY_VERY_LOW"
		};
	}

	public static ItemCondition CreatePureWater()
	{
		return new ItemCondition
		{
			category = "CATEGORY_WATER_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_pure_water"),
			modValue = 100,
			identifier = WaterFeatureHelper.PURE_WATER_DISPLAY,
			customValue = 9990
		};
	}

	public static ItemCondition CreateHighQualityWater()
	{
		return new ItemCondition
		{
			category = "CATEGORY_WATER_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_high_quality_water"),
			modValue = 50,
			identifier = WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY,
			customValue = 9900
		};
	}

	public static ItemCondition CreateBaseWater()
	{
		return new ItemCondition
		{
			category = "CATEGORY_WATER_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_basewater"),
			modValue = 0,
			identifier = WaterFeatureHelper.BASE_WATER_DISPLAY,
			customValue = 9800
		};
	}

	public static ItemCondition CreateGhostWater()
	{
		return new ItemCondition
		{
			category = "CATEGORY_WATER_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_ghostwater"),
			modValue = -33,
			identifier = WaterFeatureHelper.GHOSTWATER_DISPLAY,
			customValue = 9600
		};
	}

	public static ItemCondition CreateRustWater()
	{
		return new ItemCondition
		{
			category = "CATEGORY_WATER_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_rustwater"),
			modValue = -75,
			identifier = WaterFeatureHelper.RUST_WATER_DISPLAY,
			customValue = 9000
		};
	}

	public static ItemCondition CreateGutterflow()
	{
		return new ItemCondition
		{
			category = "CATEGORY_WATER_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_gutterflow"),
			modValue = -90,
			identifier = WaterFeatureHelper.GUTTERFLOW_WATER_DISPLAY,
			customValue = 3500
		};
	}

	public static ItemCondition CreateFoodVeryFresh()
	{
		return new ItemCondition
		{
			category = "CATEGORY_FRESHNESS",
			display = LocHelper.GetLocalizedMechanic("mech_condition_very_fresh"),
			modValue = 15,
			identifier = "very_fresh"
		};
	}

	public static ItemCondition CreateFoodFresh()
	{
		return new ItemCondition
		{
			category = "CATEGORY_FRESHNESS",
			display = LocHelper.GetLocalizedMechanic("mech_condition_fresh"),
			modValue = 0,
			identifier = "fresh"
		};
	}

	public static ItemCondition CreateFoodStale()
	{
		return new ItemCondition
		{
			category = "CATEGORY_FRESHNESS",
			display = LocHelper.GetLocalizedMechanic("mech_condition_stale"),
			modValue = -15,
			identifier = "stale"
		};
	}

	public static ItemCondition CreateFoodSpoiled()
	{
		return new ItemCondition
		{
			category = "CATEGORY_FRESHNESS",
			display = LocHelper.GetLocalizedMechanic("mech_condition_spoiled"),
			modValue = -50,
			identifier = "spoiled"
		};
	}

	public static ItemCondition CreateCounterfeitCigarette()
	{
		return new ItemCondition
		{
			category = "CATEGORY_GENUINE_CIGARETTE",
			display = LocHelper.GetLocalizedMechanic("mech_condition_counterfeit"),
			modValue = 0,
			modValueDisplay = -50,
			isTransformative = true,
			identifier = "counterfeitCigarette",
			compareValue = -50,
			onActivateActionId = "counterfeitCigarette"
		};
	}

	public static ItemCondition CreateGenuineCigarette()
	{
		return new ItemCondition
		{
			category = "CATEGORY_GENUINE_CIGARETTE",
			display = LocHelper.GetLocalizedMechanic("mech_condition_genuine"),
			modValue = 0,
			modValueDisplay = 0,
			isTransformative = true,
			identifier = "genuineCigarette",
			compareValue = 0,
			onActivateActionId = "genuineCigarette"
		};
	}

	public static ItemCondition CreateCounterfeitStamp()
	{
		return new ItemCondition
		{
			category = "CATEGORY_GENUINE_STAMP",
			display = LocHelper.GetLocalizedMechanic("mech_condition_counterfeit"),
			modValue = 0,
			modValueDisplay = 0,
			isTransformative = true,
			identifier = "counterfeitStamp",
			compareValue = -90,
			onActivateActionId = "counterfeitStamp"
		};
	}

	public static ItemCondition CreateGenuineStamp()
	{
		return new ItemCondition
		{
			category = "CATEGORY_GENUINE_STAMP",
			display = LocHelper.GetLocalizedMechanic("mech_condition_genuine"),
			modValue = 0,
			modValueDisplay = 100,
			isTransformative = true,
			identifier = "genuineStamp",
			compareValue = 0
		};
	}

	public static ItemCondition CreateCounterfeitInjector()
	{
		return new ItemCondition
		{
			category = "CATEGORY_GENUINE_INJECTOR",
			display = LocHelper.GetLocalizedMechanic("mech_condition_counterfeit"),
			modValue = 0,
			modValueDisplay = -75,
			isTransformative = true,
			identifier = "counterfeitInjector",
			compareValue = -75,
			onActivateActionId = "counterfeitInjector"
		};
	}

	public static ItemCondition CreateUnusableInjector()
	{
		return new ItemCondition
		{
			category = "CATEGORY_GENUINE_INJECTOR",
			display = LocHelper.GetLocalizedMechanic("mech_condition_unusable"),
			modValue = 0,
			modValueDisplay = -90,
			isTransformative = true,
			identifier = "unusableInjector",
			compareValue = -90,
			onActivateActionId = "unusableInjector"
		};
	}

	public static ItemCondition CreateExpiredInjector()
	{
		return new ItemCondition
		{
			category = "CATEGORY_GENUINE_INJECTOR",
			display = LocHelper.GetLocalizedMechanic("mech_condition_expired"),
			modValue = 0,
			modValueDisplay = -50,
			isTransformative = true,
			identifier = "expiredInjector",
			compareValue = -50,
			onActivateActionId = "expiredInjector"
		};
	}

	public static ItemCondition CreateGenuineInjector()
	{
		return new ItemCondition
		{
			category = "CATEGORY_GENUINE_INJECTOR",
			display = LocHelper.GetLocalizedMechanic("mech_condition_genuine"),
			modValue = 0,
			modValueDisplay = 0,
			isTransformative = true,
			identifier = "genuineInjector",
			compareValue = 0,
			onActivateActionId = "genuineInjector"
		};
	}

	public static ItemCondition CreateModuleNormal()
	{
		return new ItemCondition
		{
			category = "CATEGORY_FRESHNESS",
			display = LocHelper.GetLocalizedMechanic("mech_condition_normal"),
			modValue = 0,
			identifier = "module_normale"
		};
	}

	public static ItemCondition CreateShabby()
	{
		return new ItemCondition
		{
			category = "CATEGORY_SHABBY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_shabby"),
			modValue = -75,
			identifier = "shabby"
		};
	}

	public static ItemCondition CreateSecondHand()
	{
		return new ItemCondition
		{
			category = "CATEGORY_SECOND_HAND",
			display = LocHelper.GetLocalizedMechanic("mech_condition_second_hand"),
			modValue = -50,
			identifier = "second_hand"
		};
	}

	public static ItemCondition CreateModuleStuck()
	{
		return new ItemCondition
		{
			category = "CATEGORY_FRESHNESS",
			display = LocHelper.GetLocalizedMechanic("mech_condition_stuck"),
			modValue = -75,
			identifier = "module_stuck"
		};
	}

	public static ItemCondition CreateEquipementConditionNew()
	{
		return new ItemCondition
		{
			category = "CATEGORY_EQUIPMENT_CONDITION",
			display = LocHelper.GetLocalizedMechanic("mech_condition_equip_new"),
			modValue = 25,
			identifier = "equipment_condition_new"
		};
	}

	public static ItemCondition CreateEquipementConditionExcellent()
	{
		return new ItemCondition
		{
			category = "CATEGORY_EQUIPMENT_CONDITION",
			display = LocHelper.GetLocalizedMechanic("mech_condition_equip_excellent"),
			modValue = 12,
			identifier = "equipment_condition_excellent"
		};
	}

	public static ItemCondition CreateEquipementConditionGood()
	{
		return new ItemCondition
		{
			category = "CATEGORY_EQUIPMENT_CONDITION",
			display = LocHelper.GetLocalizedMechanic("mech_condition_equip_good"),
			modValue = 0,
			identifier = "equipment_condition_good"
		};
	}

	public static ItemCondition CreateEquipementConditionFair()
	{
		return new ItemCondition
		{
			category = "CATEGORY_EQUIPMENT_CONDITION",
			display = LocHelper.GetLocalizedMechanic("mech_condition_equip_fair"),
			modValue = -12,
			identifier = "equipment_condition_fair"
		};
	}

	public static ItemCondition CreateEquipementConditionPoor()
	{
		return new ItemCondition
		{
			category = "CATEGORY_EQUIPMENT_CONDITION",
			display = LocHelper.GetLocalizedMechanic("mech_condition_equip_poor"),
			modValue = -25,
			identifier = "equipment_condition_poor"
		};
	}

	public static ItemCondition CreateReplica()
	{
		return new ItemCondition
		{
			category = "CATEGORY_REPLICA",
			display = LocHelper.GetLocalizedMechanic("mech_condition_replica"),
			modValue = -75,
			identifier = "replica"
		};
	}

	public static ItemCondition CreateNonReplica()
	{
		return new ItemCondition
		{
			category = "CATEGORY_REPLICA",
			display = LocHelper.GetLocalizedMechanic("mech_condition_functional"),
			modValue = 0,
			identifier = "non_replica",
			hiddenAsPublic = true
		};
	}

	public static ItemCondition CreateLabPure()
	{
		return new ItemCondition
		{
			category = "CATEGORY_CHEMICAL_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_lab_pure"),
			modValue = 135,
			identifier = "lab_pure"
		};
	}

	public static ItemCondition CreatePharmaGrade()
	{
		return new ItemCondition
		{
			category = "CATEGORY_CHEMICAL_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_pharma_grade"),
			modValue = 85,
			identifier = "pharma_grade"
		};
	}

	public static ItemCondition CreateNearPharma()
	{
		return new ItemCondition
		{
			category = "CATEGORY_CHEMICAL_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_near_pharma"),
			modValue = 40,
			identifier = "near_pharma"
		};
	}

	public static ItemCondition CreateStreetStandard()
	{
		return new ItemCondition
		{
			category = "CATEGORY_CHEMICAL_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_street_standard"),
			modValue = 0,
			identifier = "street_standard"
		};
	}

	public static ItemCondition CreateCutProduct()
	{
		return new ItemCondition
		{
			category = "CATEGORY_CHEMICAL_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_cut_product"),
			modValue = -45,
			identifier = "cut_product"
		};
	}

	public static ItemCondition CreateJunkGrade()
	{
		return new ItemCondition
		{
			category = "CATEGORY_CHEMICAL_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_junk_grade"),
			modValue = -75,
			identifier = "junk_grade"
		};
	}

	public static ItemCondition CreateStarchGrade()
	{
		return new ItemCondition
		{
			category = "CATEGORY_CHEMICAL_PURITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_starch_grade"),
			modValue = -95,
			identifier = "starch_grade"
		};
	}

	public static ItemCondition CreateMasterVintage()
	{
		return new ItemCondition
		{
			category = "CATEGORY_WINE_QUALITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_wine_master_vintage"),
			modValue = 75,
			identifier = "wine_quality_master_vintage"
		};
	}

	public static ItemCondition CreateReserve()
	{
		return new ItemCondition
		{
			category = "CATEGORY_WINE_QUALITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_wine_reserve"),
			modValue = 50,
			identifier = "wine_quality_reserve"
		};
	}

	public static ItemCondition CreateProductionWine()
	{
		return new ItemCondition
		{
			category = "CATEGORY_WINE_QUALITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_wine_production"),
			modValue = 25,
			identifier = "wine_quality_production"
		};
	}

	public static ItemCondition CreateHomebrew()
	{
		return new ItemCondition
		{
			category = "CATEGORY_WINE_QUALITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_wine_homebrew"),
			modValue = 0,
			identifier = "wine_quality_homebrew"
		};
	}

	public static ItemCondition CreateBucketBrew()
	{
		return new ItemCondition
		{
			category = "CATEGORY_WINE_QUALITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_wine_bucket_brew"),
			modValue = -15,
			identifier = "wine_quality_bucket_brew"
		};
	}

	public static ItemCondition CreateUndrinkable()
	{
		return new ItemCondition
		{
			category = "CATEGORY_WINE_QUALITY",
			display = LocHelper.GetLocalizedMechanic("mech_condition_wine_undrinkable"),
			modValue = -100,
			identifier = "wine_quality_undrinkable"
		};
	}
}

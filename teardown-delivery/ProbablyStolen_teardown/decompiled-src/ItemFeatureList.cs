using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemFeatureList
{
	public enum InjectorConditionType
	{
		Genuine,
		Counterfeit,
		Expired,
		Unusable
	}

	public enum EquipmentCondition
	{
		New,
		Excellent,
		Good,
		Fair,
		Poor
	}

	public const string CATEGORY_FREE = "CATEGORY_FREE";

	public const string CATEGORY_OPEN_BOX = "CATEGORY_OPEN_BOX";

	public const string CATEGORY_RUSTED = "CATEGORY_RUSTED";

	public const string CATEGORY_EXPIRED = "CATEGORY_EXPIRED";

	public const string CATEGORY_DENTED = "CATEGORY_DENTED";

	public const string CATEGORY_INGOT_PURITY = "CATEGORY_INGOT_PURITY";

	public const string CATEGORY_WATER_PURITY = "CATEGORY_WATER_PURITY";

	public const string CATEGORY_FRESHNESS = "CATEGORY_FRESHNESS";

	public const string CATEGORY_GENUINE_CIGARETTE = "CATEGORY_GENUINE_CIGARETTE";

	public const string CATEGORY_GENUINE_STAMP = "CATEGORY_GENUINE_STAMP";

	public const string CATEGORY_GENUINE_INJECTOR = "CATEGORY_GENUINE_INJECTOR";

	public const string CATEGORY_MODULE_STUCK = "CATEGORY_MODULE_STUCK";

	public const string CATEGORY_EQUIPMENT_CONDITION = "CATEGORY_EQUIPMENT_CONDITION";

	public const string CATEGORY_REPLICA = "CATEGORY_REPLICA";

	public const string CATEGORY_CHEMICAL_PURITY = "CATEGORY_CHEMICAL_PURITY";

	public const string CATEGORY_WINE_QUALITY = "CATEGORY_WINE_QUALITY";

	public const string CATEGORY_SHABBY = "CATEGORY_SHABBY";

	public const string CATEGORY_SECOND_HAND = "CATEGORY_SECOND_HAND";

	public static Dictionary<string, string> CategoryNames = new Dictionary<string, string>
	{
		{
			"CATEGORY_OPEN_BOX",
			LocHelper.GetLocalizedMechanic("item_feature_category_open_box")
		},
		{
			"CATEGORY_RUSTED",
			LocHelper.GetLocalizedMechanic("item_feature_category_rusted")
		},
		{
			"CATEGORY_EXPIRED",
			LocHelper.GetLocalizedMechanic("item_feature_category_expired")
		},
		{
			"CATEGORY_DENTED",
			LocHelper.GetLocalizedMechanic("item_feature_category_dented")
		},
		{
			"CATEGORY_INGOT_PURITY",
			LocHelper.GetLocalizedMechanic("item_feature_category_ingot_purity")
		},
		{
			"CATEGORY_WATER_PURITY",
			LocHelper.GetLocalizedMechanic("item_feature_category_water_purity")
		},
		{
			"CATEGORY_FRESHNESS",
			LocHelper.GetLocalizedMechanic("item_feature_category_freshness")
		},
		{
			"CATEGORY_GENUINE_CIGARETTE",
			LocHelper.GetLocalizedMechanic("item_feature_category_cigarette")
		},
		{
			"CATEGORY_GENUINE_STAMP",
			LocHelper.GetLocalizedMechanic("item_feature_category_stamp")
		},
		{
			"CATEGORY_GENUINE_INJECTOR",
			LocHelper.GetLocalizedMechanic("item_feature_category_injector")
		},
		{
			"CATEGORY_MODULE_STUCK",
			LocHelper.GetLocalizedMechanic("item_feature_category_module_stuck")
		},
		{
			"CATEGORY_FREE",
			LocHelper.GetLocalizedMechanic("item_feature_category_free")
		},
		{
			"CATEGORY_CHEMICAL_PURITY",
			LocHelper.GetLocalizedMechanic("item_feature_category_chemical_purity")
		},
		{
			"CATEGORY_SHABBY",
			LocHelper.GetLocalizedMechanic("item_feature_category_shabby")
		},
		{
			"CATEGORY_SECOND_HAND",
			LocHelper.GetLocalizedMechanic("mech_condition_second_hand")
		}
	};

	public static Dictionary<string, Func<ItemFeature>> SpecialFeature = new Dictionary<string, Func<ItemFeature>>
	{
		{ "discountedBuy", DiscountedBuy },
		{ "merchantDiscountBuy", MerchantDiscountBuy },
		{ "discountedBuy20", DiscountedBuy20 },
		{ "discountedBuy30", DiscountedBuy30 },
		{ "discountedBuy25", DiscountedBuy25 },
		{ "discountedBuy50", DiscountedBuy50 },
		{ "discountedBuy100", DiscountedBuy100 },
		{ "discountedBuy200", DiscountedBuy200 },
		{ "discountedBuy400", DiscountedBuy400 },
		{ "premiumBuy", PremiumBuy },
		{ "premiumBuy15", PremiumBuy15 },
		{ "premiumBuyHypertech", PremiumBuyHypertech },
		{ "retailMarkupBuy", RetailMarkupBuy },
		{ "discountedLowContraband", DiscountedLowContraband },
		{ "discountedMidContraband", DiscountedMidContraband },
		{ "discountedHighContraband", DiscountedHighContraband },
		{ "discountedCriticalContraband", DiscountedCriticalContraband },
		{ "bribeDonation", Donation }
	};

	public static ItemFeature Discount(int mod)
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "discount";
		itemFeature.category = "discount";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_client_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(mod);
		return itemFeature;
	}

	public static ItemFeature FreeFeature()
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_FREE");
		itemFeature.SetConditions(ItemConditionList.CreateFreeFree(), ItemConditionList.CreateFreeFree());
		itemFeature.identifier = "free";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		itemFeature.isFeatureMatch = true;
		return itemFeature;
	}

	public static ItemFeature RetailMarkUp()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporarySelling);
		itemFeature.identifier = "retailMarkUp";
		itemFeature.category = "retailMarkUp";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_retail_markup");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(PlayerStore.Instance.retailMarkup);
		return itemFeature;
	}

	public static ItemFeature VendingMachineBonus()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporarySelling);
		itemFeature.identifier = "vendingMachineBonus";
		itemFeature.category = "vendingMachineBonus";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_vending_machine_bonus");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(10);
		return itemFeature;
	}

	public static ItemFeature Donation()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporarySelling);
		itemFeature.identifier = "bribeDonation";
		itemFeature.category = "bribeDonation";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_donation");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-999);
		return itemFeature;
	}

	public static ItemFeature NeuroActivePerfumeSelling()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporarySelling);
		itemFeature.identifier = "neuroActivePerfumeSelling";
		itemFeature.category = "neuroActivePerfumeSelling";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_neuro_active_perfume");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(15);
		return itemFeature;
	}

	public static ItemFeature NeuroActivePerfumeBuying()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "neuroActivePerfumeBuying";
		itemFeature.category = "neuroActivePerfumeBuying";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_neuro_active_perfume");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-15);
		return itemFeature;
	}

	public static ItemFeature PheromonePerfumeSelling()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporarySelling);
		itemFeature.identifier = "pheromonePerfumeSelling";
		itemFeature.category = "pheromonePerfumeSelling";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_pheromone_perfume");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(25);
		return itemFeature;
	}

	public static ItemFeature PheromonePerfumeBuying()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "pheromonePerfumeBuying";
		itemFeature.category = "pheromonePerfumeBuying";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_pheromone_perfume");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-25);
		return itemFeature;
	}

	public static void RemovePerfumeFeatures(GameItem gameItem)
	{
		gameItem.RemoveFeatureByCategory("neuroActivePerfumeSelling");
		gameItem.RemoveFeatureByCategory("neuroActivePerfumeBuying");
		gameItem.RemoveFeatureByCategory("pheromonePerfumeSelling");
		gameItem.RemoveFeatureByCategory("pheromonePerfumeBuying");
	}

	public static ItemFeature StolenBonusBuying()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "stolenBonusBuying";
		itemFeature.category = "stolenBonusBuying";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_five_finger_iv");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-25);
		return itemFeature;
	}

	public static ItemFeature StolenBonusBuyingHigh()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "stolenBonusBuyingHigh";
		itemFeature.category = "stolenBonusBuyingHigh";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_five_finger_iii");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-20);
		return itemFeature;
	}

	public static ItemFeature StolenBonusBuyingMid()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "stolenBonusBuyingMid";
		itemFeature.category = "stolenBonusBuyingMid";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_five_finger_ii");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-15);
		return itemFeature;
	}

	public static ItemFeature StolenBonusBuyingLow()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "stolenBonusBuyingLow";
		itemFeature.category = "stolenBonusBuyingLow";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_five_finger_i");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-10);
		return itemFeature;
	}

	public static ItemFeature AddictionBuyPenalty()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "addictionBuyPenalty";
		itemFeature.category = "addictionBuyPenalty";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_negotiation_penalty_addiction");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(PlayerStore.Instance.healthData.GetTotalAddictionPenalty());
		return itemFeature;
	}

	public static ItemFeature AddictionSellPenalty()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporarySelling);
		itemFeature.identifier = "addictionSellPenalty";
		itemFeature.category = "addictionSellPenalty";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_negotiation_penalty_addiction");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-PlayerStore.Instance.healthData.GetTotalAddictionPenalty());
		return itemFeature;
	}

	public static ItemFeature AwfulNegotiatorBuyPenalty()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "awfulNegotiatorBuyPenalty";
		itemFeature.category = "awfulNegotiatorBuyPenalty";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_negotiation_penalty_awful_negotiator");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(5);
		return itemFeature;
	}

	public static ItemFeature AwfulNegotiatorSellPenalty()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporarySelling);
		itemFeature.identifier = "awfulNegotiatorSellPenalty";
		itemFeature.category = "awfulNegotiatorSellPenalty";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_negotiation_penalty_awful_negotiator");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-5);
		return itemFeature;
	}

	public static ItemFeature ConfrontSucessDiscount()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "confrontSucessDiscount";
		itemFeature.category = "confrontSucessDiscount";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		itemFeature.SetPublicDisplay(LocHelper.GetLocalizedMechanic("item_feature_good_inspection_discount"));
		itemFeature.SetActualDisplay(LocHelper.GetLocalizedMechanic("item_feature_good_inspection_discount"));
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-15);
		return itemFeature;
	}

	public static ItemFeature ContrabandLowMarkUp()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Special);
		itemFeature.identifier = "contrabandLowMarkUp";
		itemFeature.category = "contrabandMarkUp";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_contraband_low");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(PlayerStore.Instance.contrabandMarkupLow);
		return itemFeature;
	}

	public static ItemFeature ContrabandMidMarkUp()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Special);
		itemFeature.identifier = "contrabandMidMarkUp";
		itemFeature.category = "contrabandMarkUp";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_contraband_mid");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(PlayerStore.Instance.contrabandMarkupMid);
		return itemFeature;
	}

	public static ItemFeature ContrabandHighMarkUp()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Special);
		itemFeature.identifier = "contrabandHighMarkUp";
		itemFeature.category = "contrabandMarkUp";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_contraband_high");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(PlayerStore.Instance.contrabandMarkupHigh);
		return itemFeature;
	}

	public static ItemFeature ContrabandCriticalMarkUp()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Special);
		itemFeature.identifier = "contrabandCriticalMarkUp";
		itemFeature.category = "contrabandMarkUp";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_contraband_critical");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(PlayerStore.Instance.contrabandMarkupCritial);
		return itemFeature;
	}

	public static ItemFeature RestrictedLowMarkUp()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Special);
		itemFeature.identifier = "restrictedLowMarkUp";
		itemFeature.category = "restritedMarkUp";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_restricted_class_i");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(PlayerStore.Instance.contrabandMarkupLow / 2);
		return itemFeature;
	}

	public static ItemFeature RestrictedMidMarkUp()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Special);
		itemFeature.identifier = "restrictedMidMarkUp";
		itemFeature.category = "restritedMarkUp";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_restricted_class_ii");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(PlayerStore.Instance.contrabandMarkupMid / 2);
		return itemFeature;
	}

	public static ItemFeature RestrictedHighMarkUp()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Special);
		itemFeature.identifier = "restrictedHighMarkUp";
		itemFeature.category = "restritedMarkUp";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_restricted_class_iii");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(PlayerStore.Instance.contrabandMarkupHigh / 2);
		return itemFeature;
	}

	public static ItemFeature SupplierDiscount()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Temporary);
		itemFeature.identifier = "supplierDiscount";
		itemFeature.category = "supplierDiscount";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_supplier_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-15);
		return itemFeature;
	}

	public static ItemFeature Pourboire()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporarySelling);
		itemFeature.identifier = "tips";
		itemFeature.category = "tips";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_tips");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(15);
		return itemFeature;
	}

	public static ItemFeature DiscountedBuy()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountedBuy";
		itemFeature.category = "discountedBuy";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_clearance_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-15);
		return itemFeature;
	}

	public static ItemFeature MerchantDiscountBuy()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "merchantDiscountBuy";
		itemFeature.category = "discountedBuy";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_clearance_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-Difficulty.Rules.merchantDiscount);
		return itemFeature;
	}

	public static ItemFeature DiscountedBuy20()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountedBuy20";
		itemFeature.category = "discountedBuy20";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_clearance_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-20);
		return itemFeature;
	}

	public static ItemFeature DiscountedBuy30()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountedBuy30";
		itemFeature.category = "discountedBuy30";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-30);
		return itemFeature;
	}

	public static ItemFeature DiscountedLowContraband()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountedLowContraband";
		itemFeature.category = "discountedLowContraband";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-PlayerStore.Instance.contrabandMarkupLow / 2);
		return itemFeature;
	}

	public static ItemFeature DiscountedMidContraband()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountedMidContraband";
		itemFeature.category = "discountedMidContraband";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-PlayerStore.Instance.contrabandMarkupMid / 2);
		return itemFeature;
	}

	public static ItemFeature DiscountedHighContraband()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountedHighContraband";
		itemFeature.category = "discountedHighContraband";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-PlayerStore.Instance.contrabandMarkupHigh / 2);
		return itemFeature;
	}

	public static ItemFeature DiscountedCriticalContraband()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountedCriticalContraband";
		itemFeature.category = "discountedCriticalContraband";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-PlayerStore.Instance.contrabandMarkupCritial / 2);
		return itemFeature;
	}

	public static ItemFeature DiscountedBuy25()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountedBuy25";
		itemFeature.category = "discountedBuy25";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-25);
		return itemFeature;
	}

	public static ItemFeature DiscountedBuy50()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountedBuy50";
		itemFeature.category = "discountedBuy50";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-50);
		return itemFeature;
	}

	public static ItemFeature DiscountedBuy100()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountedBuy100";
		itemFeature.category = "discountedBuy100";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-100);
		return itemFeature;
	}

	public static ItemFeature DiscountedBuy200()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountedBuy200";
		itemFeature.category = "discountedBuy200";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-200);
		return itemFeature;
	}

	public static ItemFeature DiscountedBuy400()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountedBuy400";
		itemFeature.category = "discountedBuy400";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-400);
		return itemFeature;
	}

	public static ItemFeature PremiumBuy()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "premiumBuy";
		itemFeature.category = "premiumBuy";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_supplier_markup");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(SupplierMarkup(50));
		return itemFeature;
	}

	public static ItemFeature InventorMarkup()
	{
		int inventorMarkup = Difficulty.Rules.inventorMarkup;
		if (inventorMarkup == 0)
		{
			return null;
		}
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "inventorMarkup";
		itemFeature.category = "inventorMarkup";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic((inventorMarkup > 0) ? "item_feature_supplier_markup" : "item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(inventorMarkup);
		return itemFeature;
	}

	public static ItemFeature InventorEventPrice(int percent)
	{
		if (percent == 0)
		{
			return null;
		}
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "inventorEventPrice";
		itemFeature.category = "inventorEventPrice";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic((percent > 0) ? "item_feature_supplier_markup" : "item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(percent);
		return itemFeature;
	}

	public static ItemFeature ScalperMarkup(int percent)
	{
		if (percent == 0)
		{
			return null;
		}
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "scalperMarkup";
		itemFeature.category = "scalperMarkup";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_supplier_markup");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(percent);
		return itemFeature;
	}

	private static int SupplierMarkup(int basePercent)
	{
		return Mathf.RoundToInt((float)basePercent * Difficulty.Rules.supplierMarkupMultiplier);
	}

	public static ItemFeature PremiumBuy15()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "premiumBuy15";
		itemFeature.category = "premiumBuy15";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_supplier_markup");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(Difficulty.Rules.retiredSupplierMarkup);
		return itemFeature;
	}

	public static ItemFeature PremiumBuyHypertech()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "premiumBuyHypertech";
		itemFeature.category = "premiumBuyHypertech";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_supplier_markup");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(SupplierMarkup(100) / 2);
		return itemFeature;
	}

	public static ItemFeature RetailMarkupBuy()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "retailMarkupBuy";
		itemFeature.category = "retailMarkupBuy";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_retail_markup");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(15);
		return itemFeature;
	}

	public static ItemFeature DiscountBuy15()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountBuy15";
		itemFeature.category = "discountBuy15";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-15);
		return itemFeature;
	}

	public static ItemFeature DiscountSell50()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "discountSell50";
		itemFeature.category = "discountSell50";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-50);
		return itemFeature;
	}

	public static ItemFeature FirstTimeDiscount()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Special);
		itemFeature.identifier = "firstTimeDiscount";
		itemFeature.category = "customerDiscount";
		itemFeature.featureType = ItemFeature.FeatureType.TemporaryBuying;
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_new_customer_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-Difficulty.Rules.newCustomerDiscount);
		return itemFeature;
	}

	public static ItemFeature VarietyBuyerBonus()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "varietyBuyerBonus";
		itemFeature.category = "varietyBuyerBonus";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_variety_buyer_bonus");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(15);
		return itemFeature;
	}

	public static ItemFeature TouristTax()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "touristTax";
		itemFeature.category = "touristTax";
		itemFeature.valueStage = ItemFeature.ValueStage.Market;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_tourist_tax");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(15);
		return itemFeature;
	}

	public static ItemFeature CigaretteAuthenticity()
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_GENUINE_CIGARETTE");
		itemFeature.SetConditions(ItemConditionList.CreateGenuineCigarette(), ItemConditionList.CreateGenuineCigarette());
		itemFeature.identifier = "realGenuine";
		itemFeature.isFeatureMatch = true;
		return itemFeature;
	}

	public static ItemFeature CigaretteFakeAuthenticity()
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_GENUINE_CIGARETTE");
		itemFeature.SetConditions(ItemConditionList.CreateGenuineCigarette(), ItemConditionList.CreateCounterfeitCigarette());
		itemFeature.identifier = "fakeGenuine";
		itemFeature.isFeatureMatch = false;
		return itemFeature;
	}

	public static ItemFeature CigaretteCounterfeit()
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_GENUINE_CIGARETTE");
		itemFeature.SetConditions(ItemConditionList.CreateCounterfeitCigarette(), ItemConditionList.CreateCounterfeitCigarette());
		itemFeature.identifier = "realCounterfeit";
		itemFeature.isFeatureMatch = true;
		return itemFeature;
	}

	public static ItemFeature CigaretteFakeCounterfeit()
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_GENUINE_CIGARETTE");
		itemFeature.SetConditions(ItemConditionList.CreateCounterfeitCigarette(), ItemConditionList.CreateGenuineCigarette());
		itemFeature.identifier = "fakeCounterfeit";
		itemFeature.isFeatureMatch = false;
		return itemFeature;
	}

	public static ItemFeature StampAuthenticity()
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_GENUINE_STAMP");
		itemFeature.SetConditions(ItemConditionList.CreateGenuineStamp(), ItemConditionList.CreateGenuineStamp());
		itemFeature.identifier = "realGenuine";
		itemFeature.isFeatureMatch = true;
		return itemFeature;
	}

	public static ItemFeature StampFakeAuthenticity()
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_GENUINE_STAMP");
		itemFeature.SetConditions(ItemConditionList.CreateGenuineStamp(), ItemConditionList.CreateCounterfeitStamp());
		itemFeature.identifier = "fakeGenuine";
		itemFeature.isFeatureMatch = false;
		return itemFeature;
	}

	public static ItemFeature StampCounterfeit()
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_GENUINE_STAMP");
		itemFeature.SetConditions(ItemConditionList.CreateCounterfeitStamp(), ItemConditionList.CreateCounterfeitStamp());
		itemFeature.identifier = "realCounterfeit";
		itemFeature.isFeatureMatch = true;
		return itemFeature;
	}

	public static ItemFeature StampFakeCounterfeit()
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_GENUINE_STAMP");
		itemFeature.SetConditions(ItemConditionList.CreateCounterfeitCigarette(), ItemConditionList.CreateGenuineCigarette());
		itemFeature.identifier = "fakeCounterfeit";
		itemFeature.isFeatureMatch = false;
		return itemFeature;
	}

	public static ItemFeature InjectorFeature(InjectorConditionType fake, InjectorConditionType real)
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_GENUINE_INJECTOR");
		itemFeature.identifier = "CATEGORY_GENUINE_INJECTOR";
		itemFeature.SetConditions(fake switch
		{
			InjectorConditionType.Genuine => ItemConditionList.CreateGenuineInjector(), 
			InjectorConditionType.Counterfeit => ItemConditionList.CreateCounterfeitInjector(), 
			InjectorConditionType.Expired => ItemConditionList.CreateExpiredInjector(), 
			_ => ItemConditionList.CreateUnusableInjector(), 
		}, real switch
		{
			InjectorConditionType.Genuine => ItemConditionList.CreateGenuineInjector(), 
			InjectorConditionType.Counterfeit => ItemConditionList.CreateCounterfeitInjector(), 
			InjectorConditionType.Expired => ItemConditionList.CreateExpiredInjector(), 
			_ => ItemConditionList.CreateUnusableInjector(), 
		});
		itemFeature.isFeatureMatch = fake == real;
		return itemFeature;
	}

	public static ItemFeature EquipementConditionFeature(EquipmentCondition equipementCondtion)
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_EQUIPMENT_CONDITION");
		itemFeature.identifier = "CATEGORY_EQUIPMENT_CONDITION";
		itemFeature.isFeatureMatch = true;
		itemFeature.useCondition = true;
		switch (equipementCondtion)
		{
		case EquipmentCondition.New:
			itemFeature.fakeCondition = ItemConditionList.CreateEquipementConditionNew();
			itemFeature.realCondition = ItemConditionList.CreateEquipementConditionNew();
			break;
		case EquipmentCondition.Excellent:
			itemFeature.fakeCondition = ItemConditionList.CreateEquipementConditionExcellent();
			itemFeature.realCondition = ItemConditionList.CreateEquipementConditionExcellent();
			break;
		case EquipmentCondition.Good:
			itemFeature.fakeCondition = ItemConditionList.CreateEquipementConditionGood();
			itemFeature.realCondition = ItemConditionList.CreateEquipementConditionGood();
			break;
		case EquipmentCondition.Fair:
			itemFeature.fakeCondition = ItemConditionList.CreateEquipementConditionFair();
			itemFeature.realCondition = ItemConditionList.CreateEquipementConditionFair();
			break;
		default:
			itemFeature.fakeCondition = ItemConditionList.CreateEquipementConditionPoor();
			itemFeature.realCondition = ItemConditionList.CreateEquipementConditionPoor();
			break;
		}
		return itemFeature;
	}

	public static ItemFeature ModuleStuckFeature()
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_MODULE_STUCK");
		itemFeature.identifier = "moduleStuck";
		itemFeature.fakeCondition = ItemConditionList.CreateModuleStuck();
		itemFeature.realCondition = ItemConditionList.CreateModuleStuck();
		itemFeature.useCondition = true;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_module_stuck");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		return itemFeature;
	}

	public static ItemFeature ShabbyFeature()
	{
		return new ItemFeature("CATEGORY_SHABBY")
		{
			identifier = "shabby",
			fakeCondition = ItemConditionList.CreateShabby(),
			realCondition = ItemConditionList.CreateShabby(),
			useCondition = true,
			isFeatureMatch = true
		};
	}

	public static ItemFeature SecondHandFeature()
	{
		return new ItemFeature("CATEGORY_SECOND_HAND")
		{
			identifier = "second_hand",
			fakeCondition = ItemConditionList.CreateSecondHand(),
			realCondition = ItemConditionList.CreateSecondHand(),
			useCondition = true,
			isFeatureMatch = true
		};
	}

	public static ItemFeature ReplicaFeature()
	{
		return new ItemFeature("CATEGORY_REPLICA")
		{
			identifier = "replica",
			fakeCondition = ItemConditionList.CreateReplica(),
			realCondition = ItemConditionList.CreateNonReplica(),
			useCondition = true
		};
	}

	public static ItemFeature CreateModuleFeature(string displayName, int slot, int valueMod)
	{
		ItemFeature itemFeature = new ItemFeature("module_" + slot);
		itemFeature.identifier = "module_" + slot;
		itemFeature.useCondition = false;
		itemFeature.SetPublicDisplay(displayName);
		itemFeature.SetActualDisplay(displayName);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(valueMod);
		return itemFeature;
	}

	public static ItemFeature BargainDiscount(int percent)
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "bargainDiscount";
		itemFeature.category = "bargainDiscount";
		itemFeature.valueStage = ItemFeature.ValueStage.Final;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_bargain_discount");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-Math.Abs(percent));
		return itemFeature;
	}

	public static ItemFeature BargainMarkup(int percent)
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "bargainMarkup";
		itemFeature.category = "bargainMarkup";
		itemFeature.valueStage = ItemFeature.ValueStage.Final;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_bargain_markup");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(Math.Abs(percent));
		return itemFeature;
	}

	public static ItemFeature BlackmailDiscount(int percent)
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "blackmailDiscount";
		itemFeature.category = "blackmail";
		itemFeature.valueStage = ItemFeature.ValueStage.Final;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_blackmail");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-Math.Abs(percent));
		return itemFeature;
	}

	public static ItemFeature BlackmailMarkup(int percent)
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "blackmailMarkup";
		itemFeature.category = "blackmail";
		itemFeature.valueStage = ItemFeature.ValueStage.Final;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_blackmail");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(Math.Abs(percent));
		return itemFeature;
	}

	public static ItemFeature BargainBuyingMarkup(int percent)
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "bargainBuyingMarkup";
		itemFeature.category = "bargainBuyingMarkup";
		itemFeature.valueStage = ItemFeature.ValueStage.Final;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_buying_markup_offer");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(Math.Abs(percent));
		return itemFeature;
	}

	public static ItemFeature BargainBuyingDiscount(int percent)
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "bargainBuyingDiscount";
		itemFeature.category = "bargainBuyingDiscount";
		itemFeature.valueStage = ItemFeature.ValueStage.Final;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_buying_discount_demand");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-Math.Abs(percent));
		return itemFeature;
	}

	public static ItemFeature ThreatenDiscount(int percent)
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.TemporaryBuying);
		itemFeature.identifier = "threatenDiscount";
		itemFeature.category = "threaten";
		itemFeature.valueStage = ItemFeature.ValueStage.Final;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_threaten");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(-Math.Abs(percent));
		return itemFeature;
	}

	public static ItemFeature ThreatenMarkup(int percent)
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.ClientTemporary);
		itemFeature.identifier = "threatenMarkup";
		itemFeature.category = "threaten";
		itemFeature.valueStage = ItemFeature.ValueStage.Final;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("item_feature_threaten");
		itemFeature.SetPublicDisplay(localizedMechanic);
		itemFeature.SetActualDisplay(localizedMechanic);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(Math.Abs(percent));
		return itemFeature;
	}

	public static ItemFeature CreateWineQualityFeature(int wineScore)
	{
		ItemFeature itemFeature = new ItemFeature("CATEGORY_WINE_QUALITY");
		itemFeature.identifier = "CATEGORY_WINE_QUALITY";
		itemFeature.isFeatureMatch = true;
		itemFeature.useCondition = true;
		ItemCondition itemCondition = ((wineScore >= 6) ? ItemConditionList.CreateMasterVintage() : ((wineScore == 5) ? ItemConditionList.CreateReserve() : ((wineScore >= 3) ? ItemConditionList.CreateProductionWine() : (wineScore switch
		{
			2 => ItemConditionList.CreateHomebrew(), 
			1 => ItemConditionList.CreateBucketBrew(), 
			_ => ItemConditionList.CreateUndrinkable(), 
		}))));
		itemFeature.SetConditions(itemCondition, itemCondition);
		return itemFeature;
	}
}

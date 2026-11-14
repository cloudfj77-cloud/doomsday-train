using System;
using System.Collections.Generic;
using UnityEngine;

public class Liquid
{
	public string displayName;

	public string identifier;

	public string part;

	public string currentPart;

	public int baseMaxPurityFromMachine;

	public int basePurifierRemoveAmount;

	public bool canBeRemovedByPurifier = true;

	public int particleSize = 10;

	public double filterMinimumPurity;

	public int filterDurabilityUsage = 2;

	public int filterVolumeRemoved = 1000;

	public double boilingPoint;

	public bool isEvaporative;

	public double lethality;

	public int addictiveness;

	public double euphoria;

	public string color = "67aaa5";

	public int colorMultiplier = 1;

	public int metabolismRatePerDay;

	public Action<GameItem> OnEnterSystem;

	public Action<GameItem> OnLeftSystem;

	public const string LIQUID_WATER = "water";

	public const string LIQUID_HEAVY_METAL = "heavy_metal";

	public const string LIQUID_MICROBE = "microbe";

	public const string LIQUID_ORGANIC_WASTE = "organic_waste";

	public const string LIQUID_MINERAL = "mineral";

	public const string LIQUID_CHEMICAL_CONTAMINANT = "chemical_contaminant";

	public const string LIQUID_PHYSICAL_CONTAMINANT = "physical_contaminant";

	public const string LIQUID_MICROPLASTIC = "microplastic";

	public const string LIQUID_DREAM_CAP_MUSH = "dream_cap_mush";

	public const string LIQUID_DREAM_CAP_EXTRACT = "dream_cap_extract";

	public const string LIQUID_PLANT_MATTER = "plant_matter";

	public const string LIQUID_ALCOHOL = "alcohol";

	public const string LIQUID_CAFFEINE = "caffeine";

	public const string LIQUID_PROTEIN = "protein";

	public const string LIQUID_FAT = "fat";

	public const string LIQUID_STARCH = "starch";

	public const string LIQUID_SUGAR = "sugar";

	public const string LIQUID_COFFEE_MUSH = "coffee_mush";

	public const string LIQUID_CONTRACEPTIVE = "contraceptive";

	public const string LIQUID_ZYANIDE = "zyanide";

	public const string LIQUID_NIGHTMARE_EXTRACT = "nightmare_extract";

	public const string LIQUID_NIGHTMARE_CAP_MUSH = "nightmare_cap_mush";

	public const string LIQUID_CHLORINE = "chlorine";

	public const string LIQUID_SYNAPREST = "synaprest";

	public const string LIQUID_FANTA = "fanta";

	public const string LIQUID_METHYLOL = "methylol";

	public const string LIQUID_REDUCING_AGENT = "reducing_agent";

	public const string LIQUID_ACID = "acid";

	public const string LIQUID_BASE = "base";

	public const string LIQUID_STABILIZING_AGENT = "stabilizing_agent";

	public const string LIQUID_DEBRIS = "debris";

	public const string LIQUID_LIPOBOND_AGENT = "lipobond_agent";

	public const string LIQUID_LIVESTOCK_STEROID = "livestock_steroid";

	public const string LIQUID_LIVESTOCK_IMMUNITY = "livestock_immunity";

	public const string LIQUID_WINE_EXTRACT = "wine_extract";

	public const string LIQUID_NOMORE = "nomore";

	public const string FILTER_SIZE_TAG = "FILTER_SIZE_TAG";

	public static List<Liquid> Liquids = InitLiquid();

	public static Dictionary<string, Liquid> LiquidDict = InitLiquidDict();

	public static List<Liquid> IngestableLiquids = Liquids.FindAll((Liquid l) => l.metabolismRatePerDay > 0);

	public static List<Liquid> InitLiquid()
	{
		List<Liquid> list = new List<Liquid>();
		Liquid liquid = new Liquid();
		liquid.displayName = LocHelper.GetLocalizedMechanic("liquid_water");
		liquid.SetIdentifier("water");
		liquid.baseMaxPurityFromMachine = 0;
		liquid.basePurifierRemoveAmount = 0;
		liquid.canBeRemovedByPurifier = false;
		liquid.particleSize = 10;
		liquid.SetBoilingPoint(100.0);
		list.Add(liquid);
		Liquid liquid2 = new Liquid();
		liquid2.displayName = LocHelper.GetLocalizedMechanic("liquid_heavy_metal");
		liquid2.SetIdentifier("heavy_metal");
		liquid2.baseMaxPurityFromMachine = 25;
		liquid2.basePurifierRemoveAmount = 2000;
		liquid2.particleSize = 12;
		liquid2.color = "44464c";
		liquid2.colorMultiplier = 4;
		list.Add(liquid2);
		Liquid liquid3 = new Liquid();
		liquid3.displayName = LocHelper.GetLocalizedMechanic("liquid_organic_waste");
		liquid3.SetIdentifier("organic_waste");
		liquid3.baseMaxPurityFromMachine = 10;
		liquid3.basePurifierRemoveAmount = 5000;
		liquid3.particleSize = 40;
		liquid3.filterMinimumPurity = 0.0016;
		liquid3.filterDurabilityUsage = 1;
		liquid3.color = "955c4b";
		liquid3.colorMultiplier = 7;
		list.Add(liquid3);
		Liquid liquid4 = new Liquid();
		liquid4.displayName = LocHelper.GetLocalizedMechanic("liquid_microplastic");
		liquid4.SetIdentifier("microplastic");
		liquid4.baseMaxPurityFromMachine = 18;
		liquid4.basePurifierRemoveAmount = 2500;
		liquid4.particleSize = 25;
		liquid4.filterMinimumPurity = 0.002;
		liquid4.filterVolumeRemoved = 1500;
		liquid4.color = "44464c";
		list.Add(liquid4);
		Liquid liquid5 = new Liquid();
		liquid5.displayName = LocHelper.GetLocalizedMechanic("liquid_microbes");
		liquid5.SetIdentifier("microbe");
		liquid5.baseMaxPurityFromMachine = 9;
		liquid5.basePurifierRemoveAmount = 5000;
		liquid5.particleSize = 50;
		liquid5.color = "3d823f";
		liquid5.colorMultiplier = 4;
		list.Add(liquid5);
		Liquid liquid6 = new Liquid();
		liquid6.displayName = LocHelper.GetLocalizedMechanic("liquid_chemical_contaminant");
		liquid6.SetIdentifier("chemical_contaminant");
		liquid6.baseMaxPurityFromMachine = 16;
		liquid6.basePurifierRemoveAmount = 3000;
		liquid6.particleSize = 2;
		liquid6.color = "7ab053";
		liquid6.colorMultiplier = 6;
		list.Add(liquid6);
		Liquid liquid7 = new Liquid();
		liquid7.displayName = LocHelper.GetLocalizedMechanic("liquid_physical_contaminant");
		liquid7.SetIdentifier("physical_contaminant");
		liquid7.baseMaxPurityFromMachine = 7;
		liquid7.basePurifierRemoveAmount = 5000;
		liquid7.particleSize = 100;
		liquid7.filterMinimumPurity = 0.0016;
		liquid7.filterVolumeRemoved = 1500;
		liquid7.color = "44464c";
		list.Add(liquid7);
		Liquid liquid8 = new Liquid();
		liquid8.displayName = LocHelper.GetLocalizedMechanic("liquid_mineral");
		liquid8.SetIdentifier("mineral");
		liquid8.baseMaxPurityFromMachine = 7;
		liquid8.basePurifierRemoveAmount = 5000;
		liquid8.particleSize = 30;
		liquid8.filterMinimumPurity = 0.0016;
		liquid8.filterDurabilityUsage = 1;
		liquid8.filterVolumeRemoved = 2000;
		liquid8.color = "44464c";
		list.Add(liquid8);
		Liquid liquid9 = new Liquid();
		liquid9.displayName = LocHelper.GetLocalizedMechanic("liquid_dream_cap_mush");
		liquid9.SetIdentifier("dream_cap_mush");
		liquid9.baseMaxPurityFromMachine = 12;
		liquid9.basePurifierRemoveAmount = 4000;
		liquid9.particleSize = 200;
		liquid9.euphoria = 0.04;
		liquid9.color = "955c4b";
		list.Add(liquid9);
		Liquid liquid10 = new Liquid();
		liquid10.displayName = LocHelper.GetLocalizedMechanic("liquid_dream_cap_extract");
		liquid10.SetIdentifier("dream_cap_extract");
		liquid10.baseMaxPurityFromMachine = 12;
		liquid10.basePurifierRemoveAmount = 4000;
		liquid10.particleSize = 5;
		liquid10.euphoria = 1.0;
		liquid10.color = "bb8a65";
		list.Add(liquid10);
		Liquid liquid11 = new Liquid();
		liquid11.displayName = LocHelper.GetLocalizedMechanic("liquid_plant_matter");
		liquid11.SetIdentifier("plant_matter");
		liquid11.baseMaxPurityFromMachine = 12;
		liquid11.basePurifierRemoveAmount = 4000;
		liquid11.particleSize = 200;
		liquid11.color = "4d5438";
		list.Add(liquid11);
		Liquid liquid12 = new Liquid();
		liquid12.displayName = LocHelper.GetLocalizedMechanic("liquid_alcohol");
		liquid12.SetIdentifier("alcohol");
		liquid12.baseMaxPurityFromMachine = 12;
		liquid12.basePurifierRemoveAmount = 4000;
		liquid12.particleSize = 10;
		liquid12.SetBoilingPoint(78.37);
		liquid12.color = "ffffff";
		list.Add(liquid12);
		Liquid liquid13 = new Liquid();
		liquid13.displayName = LocHelper.GetLocalizedMechanic("liquid_caffeine");
		liquid13.SetIdentifier("caffeine");
		liquid13.baseMaxPurityFromMachine = 12;
		liquid13.basePurifierRemoveAmount = 4000;
		liquid13.particleSize = 5;
		liquid13.color = "7d4f4a";
		list.Add(liquid13);
		Liquid liquid14 = new Liquid();
		liquid14.displayName = LocHelper.GetLocalizedMechanic("liquid_protein");
		liquid14.SetIdentifier("protein");
		liquid14.baseMaxPurityFromMachine = 12;
		liquid14.basePurifierRemoveAmount = 4000;
		liquid14.particleSize = 5;
		list.Add(liquid14);
		Liquid liquid15 = new Liquid();
		liquid15.displayName = LocHelper.GetLocalizedMechanic("liquid_fat_oil");
		liquid15.SetIdentifier("fat");
		liquid15.baseMaxPurityFromMachine = 12;
		liquid15.basePurifierRemoveAmount = 4000;
		liquid15.particleSize = 5;
		list.Add(liquid15);
		Liquid liquid16 = new Liquid();
		liquid16.displayName = LocHelper.GetLocalizedMechanic("liquid_starch");
		liquid16.SetIdentifier("starch");
		liquid16.baseMaxPurityFromMachine = 12;
		liquid16.basePurifierRemoveAmount = 4000;
		liquid16.particleSize = 5;
		list.Add(liquid16);
		Liquid liquid17 = new Liquid();
		liquid17.displayName = LocHelper.GetLocalizedMechanic("liquid_sugar");
		liquid17.SetIdentifier("sugar");
		liquid17.baseMaxPurityFromMachine = 12;
		liquid17.basePurifierRemoveAmount = 4000;
		liquid17.particleSize = 5;
		list.Add(liquid17);
		Liquid liquid18 = new Liquid();
		liquid18.displayName = LocHelper.GetLocalizedMechanic("liquid_coffee_bean_mush");
		liquid18.SetIdentifier("coffee_mush");
		liquid18.baseMaxPurityFromMachine = 12;
		liquid18.basePurifierRemoveAmount = 4000;
		liquid18.particleSize = 200;
		liquid18.color = "4a2f38";
		list.Add(liquid18);
		Liquid liquid19 = new Liquid();
		liquid19.displayName = LocHelper.GetLocalizedMechanic("liquid_contraceptive_hormones");
		liquid19.SetIdentifier("contraceptive");
		liquid19.baseMaxPurityFromMachine = 12;
		liquid19.basePurifierRemoveAmount = 4000;
		liquid19.particleSize = 5;
		list.Add(liquid19);
		Liquid liquid20 = new Liquid();
		liquid20.displayName = LocHelper.GetLocalizedMechanic("liquid_zyanide");
		liquid20.SetIdentifier("zyanide");
		liquid20.baseMaxPurityFromMachine = 12;
		liquid20.basePurifierRemoveAmount = 4000;
		liquid20.particleSize = 5;
		liquid20.lethality = 1.66;
		liquid20.color = "275654";
		list.Add(liquid20);
		Liquid liquid21 = new Liquid();
		liquid21.displayName = LocHelper.GetLocalizedMechanic("liquid_nightmare_extract");
		liquid21.SetIdentifier("nightmare_extract");
		liquid21.baseMaxPurityFromMachine = 12;
		liquid21.basePurifierRemoveAmount = 4000;
		liquid21.particleSize = 5;
		liquid21.lethality = 4.0;
		liquid21.euphoria = 0.25;
		liquid21.color = "9c3738";
		liquid21.colorMultiplier = 4;
		list.Add(liquid21);
		Liquid liquid22 = new Liquid();
		liquid22.displayName = LocHelper.GetLocalizedMechanic("liquid_nightmare_cap_mush");
		liquid22.SetIdentifier("nightmare_cap_mush");
		liquid22.baseMaxPurityFromMachine = 12;
		liquid22.basePurifierRemoveAmount = 4000;
		liquid22.particleSize = 200;
		liquid22.lethality = 0.6;
		liquid22.euphoria = 0.1;
		liquid22.color = "74313b";
		liquid21.colorMultiplier = 4;
		list.Add(liquid22);
		Liquid liquid23 = new Liquid();
		liquid23.displayName = LocHelper.GetLocalizedMechanic("liquid_chlorine");
		liquid23.SetIdentifier("chlorine");
		liquid23.baseMaxPurityFromMachine = 12;
		liquid23.basePurifierRemoveAmount = 4000;
		liquid23.particleSize = 5;
		liquid23.color = "84a251";
		list.Add(liquid23);
		Liquid liquid24 = new Liquid();
		liquid24.displayName = LocHelper.GetLocalizedMechanic("liquid_synaprest");
		liquid24.SetIdentifier("synaprest");
		liquid24.baseMaxPurityFromMachine = 12;
		liquid24.basePurifierRemoveAmount = 4000;
		liquid24.particleSize = 5;
		liquid24.color = "79746e";
		list.Add(liquid24);
		Liquid liquid25 = new Liquid();
		liquid25.displayName = LocHelper.GetLocalizedMechanic("liquid_fanta");
		liquid25.SetIdentifier("fanta");
		liquid25.baseMaxPurityFromMachine = 12;
		liquid25.basePurifierRemoveAmount = 4000;
		liquid25.particleSize = 5;
		liquid25.euphoria = 10.0;
		liquid25.lethality = 2.0;
		liquid25.color = "a272ac";
		list.Add(liquid25);
		Liquid liquid26 = new Liquid();
		liquid26.displayName = LocHelper.GetLocalizedMechanic("liquid_n_methylol_base");
		liquid26.SetIdentifier("methylol");
		liquid26.baseMaxPurityFromMachine = 12;
		liquid26.basePurifierRemoveAmount = 4000;
		liquid26.particleSize = 5;
		liquid26.color = "355271";
		list.Add(liquid26);
		Liquid liquid27 = new Liquid();
		liquid27.displayName = LocHelper.GetLocalizedMechanic("liquid_reducing_agent");
		liquid27.SetIdentifier("reducing_agent");
		liquid27.baseMaxPurityFromMachine = 12;
		liquid27.basePurifierRemoveAmount = 4000;
		liquid27.particleSize = 5;
		liquid27.color = "67aaa5";
		list.Add(liquid27);
		Liquid liquid28 = new Liquid();
		liquid28.displayName = LocHelper.GetLocalizedMechanic("liquid_acid");
		liquid28.SetIdentifier("acid");
		liquid28.baseMaxPurityFromMachine = 12;
		liquid28.basePurifierRemoveAmount = 4000;
		liquid28.particleSize = 5;
		liquid28.lethality = 0.1;
		liquid28.color = "7ab053";
		list.Add(liquid28);
		Liquid liquid29 = new Liquid();
		liquid29.displayName = LocHelper.GetLocalizedMechanic("liquid_base");
		liquid29.SetIdentifier("base");
		liquid29.baseMaxPurityFromMachine = 12;
		liquid29.basePurifierRemoveAmount = 4000;
		liquid29.particleSize = 5;
		liquid29.lethality = 0.1;
		liquid29.color = "67aaa5";
		list.Add(liquid29);
		Liquid liquid30 = new Liquid();
		liquid30.displayName = LocHelper.GetLocalizedMechanic("liquid_stabilizing_agent");
		liquid30.SetIdentifier("stabilizing_agent");
		liquid30.baseMaxPurityFromMachine = 12;
		liquid30.basePurifierRemoveAmount = 4000;
		liquid30.particleSize = 5;
		liquid30.color = "8abfbb";
		list.Add(liquid30);
		Liquid liquid31 = new Liquid();
		liquid31.displayName = LocHelper.GetLocalizedMechanic("liquid_insoluble_debris");
		liquid31.SetIdentifier("debris");
		liquid31.baseMaxPurityFromMachine = 0;
		liquid31.basePurifierRemoveAmount = 6000;
		liquid31.particleSize = 200;
		liquid31.color = "44464c";
		list.Add(liquid31);
		Liquid liquid32 = new Liquid();
		liquid32.displayName = LocHelper.GetLocalizedMechanic("liquid_lipobond_agent");
		liquid32.SetIdentifier("lipobond_agent");
		liquid32.baseMaxPurityFromMachine = 12;
		liquid32.basePurifierRemoveAmount = 4000;
		liquid32.particleSize = 5;
		liquid32.SetBoilingPoint(55.0);
		liquid32.color = "cc7755";
		list.Add(liquid32);
		Liquid liquid33 = new Liquid();
		liquid33.displayName = LocHelper.GetLocalizedMechanic("liquid_livestock_steroid");
		liquid33.SetIdentifier("livestock_steroid");
		liquid33.baseMaxPurityFromMachine = 12;
		liquid33.basePurifierRemoveAmount = 4000;
		liquid33.metabolismRatePerDay = 5;
		liquid33.particleSize = 5;
		liquid33.color = "ffcc00";
		list.Add(liquid33);
		liquid33.OnEnterSystem = delegate(GameItem animal)
		{
			if (animal.IsTag("ANIMAL_ITEM_TAG"))
			{
				GeneticHelper.ModBonusPercent(animal, "ANIMAL_GROWTH_RATE_TAG", 0.2f);
				GeneticHelper.ModBonusPercent(animal, "ANIMAL_IMMUNITY_TAG", -0.4f);
				GeneticHelper.ModBonusPercent(animal, "ANIMAL_HUNGER_RATE_TAG", 0.25f);
			}
		};
		liquid33.OnLeftSystem = delegate(GameItem animal)
		{
			if (animal.IsTag("ANIMAL_ITEM_TAG"))
			{
				GeneticHelper.ModBonusPercent(animal, "ANIMAL_GROWTH_RATE_TAG", -0.2f);
				GeneticHelper.ModBonusPercent(animal, "ANIMAL_IMMUNITY_TAG", 0.4f);
				GeneticHelper.ModBonusPercent(animal, "ANIMAL_HUNGER_RATE_TAG", -0.25f);
			}
		};
		Liquid liquid34 = new Liquid();
		liquid34.displayName = LocHelper.GetLocalizedMechanic("liquid_livestock_immunity");
		liquid34.SetIdentifier("livestock_immunity");
		liquid34.baseMaxPurityFromMachine = 12;
		liquid34.basePurifierRemoveAmount = 4000;
		liquid34.metabolismRatePerDay = 5;
		liquid34.particleSize = 5;
		liquid34.color = "00ccff";
		list.Add(liquid34);
		liquid34.OnEnterSystem = delegate(GameItem animal)
		{
			if (animal.IsTag("ANIMAL_ITEM_TAG"))
			{
				GeneticHelper.ModBonusPercent(animal, "ANIMAL_IMMUNITY_TAG", 0.75f);
				GeneticHelper.ModBonusPercent(animal, "ANIMAL_HUNGER_RATE_TAG", 0.25f);
			}
		};
		liquid34.OnLeftSystem = delegate(GameItem animal)
		{
			if (animal.IsTag("ANIMAL_ITEM_TAG"))
			{
				GeneticHelper.ModBonusPercent(animal, "ANIMAL_IMMUNITY_TAG", -0.75f);
				GeneticHelper.ModBonusPercent(animal, "ANIMAL_HUNGER_RATE_TAG", -0.25f);
			}
		};
		Liquid liquid35 = new Liquid();
		liquid35.displayName = LocHelper.GetLocalizedMechanic("liquid_wine_extract");
		liquid35.SetIdentifier("wine_extract");
		liquid35.baseMaxPurityFromMachine = 12;
		liquid35.basePurifierRemoveAmount = 4000;
		liquid35.particleSize = 5;
		liquid35.color = "955699";
		liquid35.colorMultiplier = 150;
		list.Add(liquid35);
		Liquid liquid36 = new Liquid();
		liquid36.displayName = LocHelper.GetLocalizedMechanic("liquid_nomore");
		liquid36.SetIdentifier("nomore");
		liquid36.baseMaxPurityFromMachine = 12;
		liquid36.basePurifierRemoveAmount = 4000;
		liquid36.particleSize = 5;
		liquid36.color = "d98cb3";
		list.Add(liquid36);
		return list;
	}

	public static Dictionary<string, Liquid> InitLiquidDict()
	{
		Dictionary<string, Liquid> dictionary = new Dictionary<string, Liquid>();
		foreach (Liquid liquid in Liquids)
		{
			dictionary[liquid.identifier] = liquid;
		}
		return dictionary;
	}

	public static string GetCurrentPartFromId(string identifier)
	{
		if (LiquidDict.ContainsKey(identifier))
		{
			return LiquidDict[identifier].currentPart;
		}
		Debug.LogError("Liquid with identifier " + identifier + " not found.");
		return null;
	}

	public static Liquid GetLiquidFromId(string identifier)
	{
		if (LiquidDict.ContainsKey(identifier))
		{
			return LiquidDict[identifier];
		}
		Debug.LogError("Liquid with identifier " + identifier + " not found.");
		return null;
	}

	public int GetMaxPurityFromMachine(double purityMultiplier)
	{
		return (int)((double)baseMaxPurityFromMachine * purityMultiplier);
	}

	public int GetPurifierRemoveAmount(double amountMultiplier)
	{
		return (int)((double)basePurifierRemoveAmount * amountMultiplier);
	}

	private void SetBoilingPoint(double temp)
	{
		boilingPoint = temp;
		isEvaporative = true;
	}

	private void SetIdentifier(string id)
	{
		identifier = id;
		part = "PART_" + id.ToUpperInvariant();
		currentPart = "CURRENT_PART_" + id.ToUpperInvariant();
	}

	public bool IsLiquid(string id)
	{
		if (identifier.Equals(id))
		{
			return true;
		}
		return false;
	}

	public static string GetColorBase(string hexColor)
	{
		if (string.IsNullOrWhiteSpace(hexColor))
		{
			return "dark";
		}
		if (!ColorUtility.TryParseHtmlString("#" + hexColor, out var rgbColor))
		{
			return "dark";
		}
		if (hexColor.Equals("ffffff") || hexColor.Equals("FFFFFF"))
		{
			return "blue";
		}
		Color.RGBToHSV(rgbColor, out var H, out var S, out var V);
		if (V < 0.25f)
		{
			return "dark";
		}
		if (S < 0.25f)
		{
			return "dark";
		}
		float num = H * 360f;
		if (num >= 170f && num <= 260f)
		{
			return "blue";
		}
		if (num <= 60f || num >= 330f)
		{
			return "red";
		}
		return "dark";
	}
}

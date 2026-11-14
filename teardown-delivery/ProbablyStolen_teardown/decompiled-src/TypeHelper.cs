using System.Collections.Generic;
using UnityEngine;

public class TypeHelper
{
	public const string TYPE_TAG = "TYPE_TAG";

	public const int MAX_TYPE = 20;

	public const string FOOD = "FOOD";

	public const string MEDICAL = "MEDICAL";

	public const string MATERIAL = "MATERIAL";

	public const string HOUSEHOLD_GOOD = "HOUSEHOLD_GOOD";

	public const string SUBSTANCE = "SUBSTANCE";

	public const string WEAPON = "WEAPON";

	public const string NOT_FOR_RESALE = "NOT_FOR_RESALE";

	public static readonly List<string> MAJOR_TYPES_LIST = new List<string>
	{
		"FOOD", "MEDICAL", "MATERIAL", "HOUSEHOLD_GOOD", "SUBSTANCE", "WEAPON", "WATER", "ARMOR", "TOOL", "MODULE",
		"AMMUNITION"
	};

	public static readonly List<string> ALL_TYPES_LIST = new List<string>
	{
		"FOOD", "MEDICAL", "MATERIAL", "HOUSEHOLD_GOOD", "SUBSTANCE", "WEAPON", "NOT_FOR_RESALE", "SUPPLY_CRATE", "TOOL", "MEDICAL_TOOL",
		"CONTRABAND", "RESTRICTED_1", "RESTRICTED_2", "RESTRICTED_3", "MAKESHIFT", "LOWTECH", "MIDLINE", "HIGHEND", "RARE", "LUXURY_ITEM",
		"ART", "WATER", "WATER_RATION", "FARMING_SUPPLIES", "CHEMICAL_SUPPLIES", "FRESH_FOOD", "PROCESSED_FOOD", "TREAT", "BREVAGE", "DOCUMENT",
		"ACCESS_CARD", "SEED", "MODIFIED_SEED", "BASIC_MATERIAL", "REFINED_MATERIAL", "ENERGY_CELL", "NARCOTIC", "POISON", "STORAGE", "WATER_PURIFICATION_SUPPLY",
		"OXYGEN_SUPPLY", "ALCOHOL", "MELEE_WEAPON", "FIREARM", "MODDABLE", "AMMUNITION", "MUNITION", "GUN_MOD", "MODULE", "NODE",
		"HYDROPONIC_MODULE", "ALARM_MODULE", "PRINTER_MODULE", "EVAPORATOR_MODULE", "MACHINE", "ORGAN", "ARMOR", "FILTER", "LOTTERY", "JUNK"
	};

	public const string SUPPLY_CRATE = "SUPPLY_CRATE";

	public const string TOOL = "TOOL";

	public const string MEDICAL_TOOL = "MEDICAL_TOOL";

	public const string ALL_TYPE = "ALL_TYPE";

	public const string ALL_TYPE_EXCEPT_CONTRABAND = "ALL_TYPE_EXCEPT_CONTRABAND";

	public const string CONTRABAND = "CONTRABAND";

	public const string RESTRICTED_1 = "RESTRICTED_1";

	public const string RESTRICTED_2 = "RESTRICTED_2";

	public const string RESTRICTED_3 = "RESTRICTED_3";

	public const string MAKESHIFT = "MAKESHIFT";

	public const string LOWTECH = "LOWTECH";

	public const string MIDLINE = "MIDLINE";

	public const string HIGHEND = "HIGHEND";

	public const string JUNK = "JUNK";

	public const string RARE = "RARE";

	public const string LUXURY_ITEM = "LUXURY_ITEM";

	public const string ART = "ART";

	public const string WATER = "WATER";

	public const string WATER_RATION = "WATER_RATION";

	public const string FARMING_SUPPLIES = "FARMING_SUPPLIES";

	public const string CHEMICAL_SUPPLIES = "CHEMICAL_SUPPLIES";

	public const string FRESH_FOOD = "FRESH_FOOD";

	public const string PROCESSED_FOOD = "PROCESSED_FOOD";

	public const string TREAT = "TREAT";

	public const string BREVAGE = "BREVAGE";

	public const string DOCUMENT = "DOCUMENT";

	public const string ACCESS_CARD = "ACCESS_CARD";

	public const string SEED = "SEED";

	public const string MODIFIED_SEED = "MODIFIED_SEED";

	public const string BASIC_MATERIAL = "BASIC_MATERIAL";

	public const string REFINED_MATERIAL = "REFINED_MATERIAL";

	public const string ENERGY_CELL = "ENERGY_CELL";

	public const string NARCOTIC = "NARCOTIC";

	public const string POISON = "POISON";

	public const string STORAGE = "STORAGE";

	public const string WATER_PURIFICATION_SUPPLY = "WATER_PURIFICATION_SUPPLY";

	public const string OXYGEN_SUPPLY = "OXYGEN_SUPPLY";

	public const string ALCOHOL = "ALCOHOL";

	public const string MELEE_WEAPON = "MELEE_WEAPON";

	public const string FIREARM = "FIREARM";

	public const string MODDABLE = "MODDABLE";

	public const string AMMUNITION = "AMMUNITION";

	public const string MUNITION = "MUNITION";

	public const string GUN_MOD = "GUN_MOD";

	public const string MODULE = "MODULE";

	public const string NODE = "NODE";

	public const string HYDROPONIC_MODULE = "HYDROPONIC_MODULE";

	public const string ALARM_MODULE = "ALARM_MODULE";

	public const string PRINTER_MODULE = "PRINTER_MODULE";

	public const string EVAPORATOR_MODULE = "EVAPORATOR_MODULE";

	public const string MACHINE = "MACHINE";

	public const string ORGAN = "ORGAN";

	public const string ARMOR = "ARMOR";

	public const string FILTER = "FILTER";

	public const string LOTTERY = "LOTTERY";

	private static readonly Color DefaultTypeColor = HexToColor("4f464b");

	private static readonly Color MedicalTypeColor = HexToColor("45918b");

	private static readonly Color LuxuryTypeColor = HexToColor("955699");

	private static readonly Color ToolTypeColor = HexToColor("9b4a3d");

	private static readonly Color WeaponTypeColor = HexToColor("9c3738");

	private static readonly Color WaterTypeColor = HexToColor("355271");

	private static readonly Color FoodTypeColor = HexToColor("3d823f");

	private static readonly Color SubstanceTypeColor = HexToColor("74313b");

	public static string GetTypeDisplayName(string typeConst)
	{
		return LocHelper.GetLocalizedMechanic("type_" + typeConst.ToLowerInvariant());
	}

	public static Color GetTypeColor(string typeConst)
	{
		switch (typeConst)
		{
		case "MEDICAL":
		case "MEDICAL_TOOL":
		case "ORGAN":
			return MedicalTypeColor;
		case "LUXURY_ITEM":
		case "ART":
			return LuxuryTypeColor;
		case "TOOL":
		case "MACHINE":
		case "MATERIAL":
		case "BASIC_MATERIAL":
		case "REFINED_MATERIAL":
		case "ENERGY_CELL":
		case "MODULE":
		case "NODE":
		case "HYDROPONIC_MODULE":
		case "ALARM_MODULE":
		case "PRINTER_MODULE":
		case "EVAPORATOR_MODULE":
		case "FILTER":
			return ToolTypeColor;
		case "WEAPON":
		case "MELEE_WEAPON":
		case "FIREARM":
		case "MODDABLE":
		case "GUN_MOD":
		case "AMMUNITION":
		case "MUNITION":
		case "ARMOR":
		case "CONTRABAND":
		case "RESTRICTED_1":
		case "RESTRICTED_2":
		case "RESTRICTED_3":
			return WeaponTypeColor;
		case "WATER":
		case "WATER_RATION":
		case "WATER_PURIFICATION_SUPPLY":
			return WaterTypeColor;
		case "FOOD":
		case "FRESH_FOOD":
		case "PROCESSED_FOOD":
		case "TREAT":
		case "BREVAGE":
		case "FARMING_SUPPLIES":
		case "SEED":
		case "MODIFIED_SEED":
			return FoodTypeColor;
		case "SUBSTANCE":
		case "NARCOTIC":
		case "ALCOHOL":
		case "POISON":
			return SubstanceTypeColor;
		default:
			return DefaultTypeColor;
		}
	}

	private static Color HexToColor(string hex)
	{
		ColorUtility.TryParseHtmlString("#" + hex, out var color);
		return color;
	}
}

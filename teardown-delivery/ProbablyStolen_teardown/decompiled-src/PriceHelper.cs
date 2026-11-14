using System.Collections.Generic;

public static class PriceHelper
{
	public const int NUTRIFRUIT_PRICE_PER_KILO = 15;

	public const int ENG_BOX_BASE_PRICE = 200;

	public const int MED_BOX_BASE_PRICE = 200;

	public const int EVIDENCE_BOX_BASE_PRICE = 200;

	public const int SERVICE_BOX_BASE_PRICE = 175;

	public const int OXYMORE_BASE_PRICE = 50;

	public const int SCRAP_METAL = 10;

	public const int ELECTRONIC_PARTS = 33;

	public const int PEAT = 25;

	public const int ENERGY_DRINK = 18;

	public const int GALAXY_BLEND = 25;

	public const int MEAT_CLEAVER = 65;

	public const int TOILET_PAPER = 25;

	public const int SEC_CARD = 100;

	public const int RED_IMP_BEER = 18;

	public const int LI_EAT_SNACKBAR = 17;

	public const int PROCESSED_CHEESE = 20;

	public const int RAW_MEAT = 159;

	public const int CROWBAR = 75;

	public const int KOTTON_FABRIC = 65;

	public const int SUPPLIES_MEDICAL = 40;

	public const int PILK_PRICE = 16;

	public const int PUICE_PRICE = 14;

	public const int NUTRIFRUIT_PRICE = 12;

	public const int GALAXY_BLEND_PRICE = 25;

	public const int WATER_RATION_PRICE = 30;

	public const int BANDAGE_PRICE = 8;

	public const int BOTTLED_WATER = 5;

	public const int LARGE_BOTTLED_WATER = 9;

	[ES3NonSerializable]
	public static Dictionary<string, int> basePriceDictionary = new Dictionary<string, int>();

	public static void InitDict()
	{
	}

	public static void HandleWeightPrice(GameItem gameItem, int weight)
	{
		if (basePriceDictionary.ContainsKey(gameItem.identifier))
		{
			gameItem.unitValue = GetWeightedPrice(weight, basePriceDictionary[gameItem.identifier]);
		}
	}

	public static int GetWeightedPrice(int weightGram, int pricePerKilo)
	{
		return weightGram * pricePerKilo / 1000;
	}

	public static bool IsUseWeightedPrice(GameItem gameItem)
	{
		if (basePriceDictionary.ContainsKey(gameItem.identifier))
		{
			return true;
		}
		return false;
	}
}

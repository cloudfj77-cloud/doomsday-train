public class ChemicalCompositionHelper
{
	public static void SetPotatoComposition(GameItem item, float weightGrams)
	{
		float num = weightGrams / 100f;
		int calories = (int)(weightGrams * 1.2f);
		FoodItemHelper.InitFoodItem(item, calories, isCanned: false);
		ChemicalHelper.InitChemItem(item, isBlendable: true);
		ChemicalHelper.SetChemContentML(item, "water", 78.55f * num);
		ChemicalHelper.SetChemContentML(item, "starch", 17.33f * num);
		ChemicalHelper.SetChemContentML(item, "protein", 2f * num);
		ChemicalHelper.SetChemContentML(item, "sugar", 0.67f * num);
		ChemicalHelper.SetChemContentML(item, "mineral", 0.67f * num);
		ChemicalHelper.SetChemContentML(item, "organic_waste", 0.33f * num);
		ChemicalHelper.SetChemContentML(item, "microbe", 0.33f * num);
		ChemicalHelper.SetChemContentML(item, "physical_contaminant", 0.12f * num);
	}

	public static void SetDreamCapComposition(GameItem item, float weightGrams, bool isComestible = false)
	{
		float num = weightGrams / 32.5f;
		ChemicalHelper.InitChemItem(item, isBlendable: true);
		int calories = (int)(weightGrams * 3f);
		if (isComestible)
		{
			calories = (int)(weightGrams * 6f);
			ChemicalHelper.SetChemContentML(item, "plant_matter", 22f * num);
		}
		else
		{
			ChemicalHelper.SetChemContentML(item, "dream_cap_mush", 27.333f * num);
		}
		FoodItemHelper.InitFoodItem(item, calories, isCanned: false);
		ChemicalHelper.SetChemContentML(item, "water", 4f * num);
		ChemicalHelper.SetChemContentML(item, "protein", 2f * num);
		ChemicalHelper.SetChemContentML(item, "mineral", 0.5f * num);
		ChemicalHelper.SetChemContentML(item, "microbe", 2f * num);
		ChemicalHelper.SetChemContentML(item, "physical_contaminant", 2f * num);
	}

	public static void SetNightmareCapComposition(GameItem item, float weightGrams)
	{
		float num = weightGrams / 32.5f;
		ChemicalHelper.InitChemItem(item, isBlendable: true);
		ChemicalHelper.SetChemContentML(item, "nightmare_cap_mush", 18f * num);
		ChemicalHelper.SetChemContentML(item, "water", 8f * num);
		ChemicalHelper.SetChemContentML(item, "protein", 2f * num);
		ChemicalHelper.SetChemContentML(item, "mineral", 0.5f * num);
		ChemicalHelper.SetChemContentML(item, "microbe", 5f * num);
		ChemicalHelper.SetChemContentML(item, "physical_contaminant", 2f * num);
	}

	public static void SetCoffeeBeanComposition(GameItem item, float weightGrams)
	{
		float num = weightGrams / 50f;
		ChemicalHelper.InitChemItem(item, isBlendable: true);
		ChemicalHelper.SetChemContentML(item, "water", 2f * num);
		ChemicalHelper.SetChemContentML(item, "fat", 5f * num);
		ChemicalHelper.SetChemContentML(item, "coffee_mush", 16.15f * num);
		ChemicalHelper.SetChemContentML(item, "caffeine", 2.75f * num);
		ChemicalHelper.SetChemContentML(item, "physical_contaminant", 5f * num);
		ChemicalHelper.SetChemContentML(item, "protein", 5f * num);
	}
}

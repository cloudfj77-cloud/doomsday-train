using System;
using System.Collections.Generic;

public class ChemFinisherRecipe
{
	public string identifier;

	public string displayName;

	public int requiredVolume;

	public bool isRequireDry;

	public string activeIngredientId;

	public const int NOMORE_INJECTOR_VOLUME = 4000;

	public static List<ChemFinisherRecipe> chemFinisherRecipes = InitRecipes();

	public static List<ChemFinisherRecipe> InitRecipes()
	{
		List<ChemFinisherRecipe> list = new List<ChemFinisherRecipe>();
		ChemFinisherRecipe item = new ChemFinisherRecipe
		{
			identifier = "dream_dust",
			displayName = LocHelper.GetLocalizedItem("item_dream_dust_name"),
			requiredVolume = 2000,
			isRequireDry = true,
			activeIngredientId = "dream_cap_extract"
		};
		list.Add(item);
		ChemFinisherRecipe item2 = new ChemFinisherRecipe
		{
			identifier = "nightmare_dust",
			displayName = LocHelper.GetLocalizedItem("item_nightmare_dust_name"),
			requiredVolume = 4000,
			isRequireDry = true,
			activeIngredientId = "nightmare_extract"
		};
		list.Add(item2);
		ChemFinisherRecipe item3 = new ChemFinisherRecipe
		{
			identifier = "caffeine_pill",
			displayName = LocHelper.GetLocalizedItem("item_caffeine_pill_name"),
			requiredVolume = 5000,
			isRequireDry = true,
			activeIngredientId = "caffeine"
		};
		list.Add(item3);
		ChemFinisherRecipe item4 = new ChemFinisherRecipe
		{
			identifier = "zyanide_pill",
			displayName = LocHelper.GetLocalizedItem("item_zyanide_pill_name"),
			requiredVolume = 600,
			isRequireDry = true,
			activeIngredientId = "zyanide"
		};
		list.Add(item4);
		ChemFinisherRecipe item5 = new ChemFinisherRecipe
		{
			identifier = "birth_control_pill",
			displayName = LocHelper.GetLocalizedItem("item_birth_control_pill_name"),
			requiredVolume = 100,
			isRequireDry = true,
			activeIngredientId = "contraceptive"
		};
		list.Add(item5);
		ChemFinisherRecipe item6 = new ChemFinisherRecipe
		{
			identifier = "fanta_pill",
			displayName = LocHelper.GetLocalizedItem("item_fanta_pill_name"),
			requiredVolume = 500,
			isRequireDry = true,
			activeIngredientId = "fanta"
		};
		list.Add(item6);
		ChemFinisherRecipe item7 = new ChemFinisherRecipe
		{
			identifier = "pink_alt_injector",
			displayName = LocHelper.GetLocalizedItem("item_pink_alt_injector_name"),
			requiredVolume = 4000,
			isRequireDry = true,
			activeIngredientId = "nomore"
		};
		list.Add(item7);
		return list;
	}

	public bool CanOutput(GameItem container)
	{
		if (WaterHelper.GetTotalVolume(container) < requiredVolume)
		{
			return false;
		}
		if (isRequireDry && WaterHelper.IsWet(container))
		{
			return false;
		}
		return true;
	}

	public static ChemFinisherRecipe GetRecipeById(string id)
	{
		return chemFinisherRecipes.Find((ChemFinisherRecipe recipe) => recipe.identifier.Equals(id));
	}

	public static void SetCompositionStreetStandard(GameItem chemicalItem, string id)
	{
		SetCompositionByPurity(chemicalItem, 60, 80, id);
	}

	public static void SetCompositionByPurity(GameItem chemicalItem, int minPurity, int maxPurity, string id)
	{
		ChemFinisherRecipe recipeById = GetRecipeById(id);
		SetCompositionByPurity(chemicalItem, minPurity, maxPurity, recipeById.requiredVolume, recipeById.activeIngredientId);
	}

	public static void SetCompositionByPurity(GameItem chemicalItem, int minPurity, int maxPurity, int requiredVolume, string activeIngredientId)
	{
		int minInclusive = (int)Math.Ceiling((double)requiredVolume * ((double)minPurity / 100.0));
		int maxExclusive = (int)Math.Floor((double)requiredVolume * ((double)maxPurity / 100.0));
		int randomIntExculsive = RNG.GetRandomIntExculsive(minInclusive, maxExclusive);
		int num = requiredVolume - randomIntExculsive;
		List<string> list = new List<string> { "physical_contaminant", "chemical_contaminant", "starch" };
		Dictionary<string, int> dictionary = new Dictionary<string, int> { [activeIngredientId] = randomIntExculsive };
		int count = list.Count;
		int num2 = num;
		for (int i = 0; i < count; i++)
		{
			int num3 = ((i != count - 1) ? RNG.GetRandomIntExculsive(0, num2 + 1) : num2);
			dictionary[list[i]] = num3;
			num2 -= num3;
		}
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			ChemicalHelper.SetChemContentUL(chemicalItem, item.Key, item.Value);
		}
	}
}

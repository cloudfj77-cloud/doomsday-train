using System.Collections.Generic;

public class AnimalSpecies
{
	public string speciesID;

	public string displayNameKey;

	public string spritePrefix;

	public int[] growthStages;

	public int[] weightStages;

	public List<string> geneticTraitIDs;

	public Dictionary<string, float> geneticBaseValues;

	public Dictionary<string, float> baseStats;

	public Dictionary<int, string> spriteByWeight;

	public string killSoundEffect;

	public string happySoundEffect;

	public float immunityThreshold;

	public float immunityRegenThreshold;

	public static readonly AnimalSpecies Rat = new AnimalSpecies
	{
		speciesID = "ANIMAL_RAT",
		displayNameKey = "husbandry_rat_name",
		spritePrefix = "rat",
		immunityThreshold = 50f,
		immunityRegenThreshold = 25f,
		killSoundEffect = "rat_kill",
		happySoundEffect = "rat_happy",
		growthStages = new int[6] { 0, 1, 3, 5, 11, 14 },
		weightStages = new int[6] { 5, 50, 200, 350, 450, 400 },
		spriteByWeight = new Dictionary<int, string>
		{
			{ 1000, "xxl" },
			{ 600, "xl" },
			{ 400, "l" },
			{ 180, "m" },
			{ 0, "s" }
		},
		geneticTraitIDs = new List<string> { "ANIMAL_GROWTH_RATE_TAG", "ANIMAL_HUNGER_RATE_TAG", "ANIMAL_FERTILITY_TAG", "ANIMAL_LITTER_SIZE_TAG", "ANIMAL_IMMUNITY_TAG", "ANIMAL_MAX_HEALTH_TAG", "ANIMAL_LONGEVITY_TAG" },
		geneticBaseValues = new Dictionary<string, float>
		{
			{ "ANIMAL_GROWTH_RATE_TAG", 1f },
			{ "ANIMAL_HUNGER_RATE_TAG", 65f },
			{ "ANIMAL_FERTILITY_TAG", 50f },
			{ "ANIMAL_LITTER_SIZE_TAG", 3f },
			{ "ANIMAL_IMMUNITY_TAG", 30f },
			{ "ANIMAL_MAX_HEALTH_TAG", 100f },
			{ "ANIMAL_LONGEVITY_TAG", 1f }
		},
		baseStats = new Dictionary<string, float>
		{
			{ "ANIMAL_GESTATION_PERIOD_TAG", 3f },
			{ "ANIMAL_FERTILITY_TAG", 50f },
			{ "ANIMAL_LITTER_SIZE_TAG", 3f },
			{ "ANIMAL_LITTER_SIZE_VARIANCE_TAG", 1f },
			{ "ANIMAL_HUNGER_TAG", 500f },
			{ "ANIMAL_MAX_HUNGER_TAG", 500f },
			{ "ANIMAL_GROWTH_RATE_TAG", -1f },
			{ "ANIMAL_HUNGER_RATE_TAG", -1f },
			{ "ANIMAL_MAX_MATING_COUNT_TAG", 3f },
			{ "ANIMAL_IMMUNITY_TAG", -1f },
			{ "ANIMAL_MAX_HEALTH_TAG", -1f },
			{ "ANIMAL_LONGEVITY_TAG", -1f },
			{ "ANIMAL_HEALTH_TAG", 0f }
		}
	};

	public static readonly Dictionary<string, AnimalSpecies> All = new Dictionary<string, AnimalSpecies> { { Rat.speciesID, Rat } };

	public string GetDisplayName()
	{
		return LocHelper.GetLocalizedItem(displayNameKey);
	}
}

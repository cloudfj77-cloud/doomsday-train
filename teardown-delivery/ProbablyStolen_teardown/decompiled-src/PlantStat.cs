using System.Collections.Generic;

public static class PlantStat
{
	public const string GROWTH_SPEED = "GROWTH_SPEED";

	public const string HARVEST_QUANTITY = "HARVEST_QUANTITY";

	public const string WATER_USE = "WATER_USE";

	public const string NUTRIMENT_RANGE = "NUTRIMENT_RANGE";

	public const string CONTAMINANT_SENSITIVITY = "CONTAMINANT_SENSITIVITY";

	public const string PLANT_HEALTH = "PLANT_HEALTH";

	public const string CALORIES = "CALORIES";

	public const string SEED_YIELD = "SEED_YIELD";

	public const string DURABILITY = "DURABILITY";

	public static readonly string[] All = new string[9] { "GROWTH_SPEED", "HARVEST_QUANTITY", "WATER_USE", "NUTRIMENT_RANGE", "CONTAMINANT_SENSITIVITY", "PLANT_HEALTH", "CALORIES", "SEED_YIELD", "DURABILITY" };

	public static readonly HashSet<string> HigherIsWorse = new HashSet<string> { "WATER_USE", "CONTAMINANT_SENSITIVITY" };

	public static string DisplayName(string stat)
	{
		return stat switch
		{
			"GROWTH_SPEED" => Name(stat, "Growth speed"), 
			"HARVEST_QUANTITY" => Name(stat, "Harvest quantity"), 
			"WATER_USE" => Name(stat, "Water use"), 
			"NUTRIMENT_RANGE" => Name(stat, "Nutriment tolerance"), 
			"CONTAMINANT_SENSITIVITY" => Name(stat, "Contaminant sensitivity"), 
			"PLANT_HEALTH" => Name(stat, "Starting health"), 
			"CALORIES" => Name(stat, "Calories"), 
			"SEED_YIELD" => Name(stat, "Seed yield"), 
			"DURABILITY" => Name(stat, "Harvest durability"), 
			_ => stat, 
		};
	}

	private static string Name(string stat, string english)
	{
		return PlantGeneticsText.StatName(stat.ToLowerInvariant(), english);
	}
}

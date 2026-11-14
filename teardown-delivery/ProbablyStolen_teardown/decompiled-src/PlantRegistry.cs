using System.Collections.Generic;

public static class PlantRegistry
{
	private static readonly Dictionary<string, PlantDefinition> plants;

	public static IEnumerable<PlantDefinition> All => plants.Values;

	static PlantRegistry()
	{
		plants = new Dictionary<string, PlantDefinition>();
		PlantList.Register();
	}

	public static PlantDefinition Add(PlantDefinition plant)
	{
		if (plant == null || string.IsNullOrEmpty(plant.id))
		{
			Debug.LogError("PlantRegistry.Add: a plant needs an id");
			return plant;
		}
		plants[plant.id] = plant;
		return plant;
	}

	public static bool Remove(string id)
	{
		return plants.Remove(id);
	}

	public static bool Contains(string id)
	{
		if (id != null)
		{
			return plants.ContainsKey(id);
		}
		return false;
	}

	public static PlantDefinition Get(string id)
	{
		if (id != null && plants.TryGetValue(id, out var value))
		{
			return value;
		}
		return null;
	}
}

using System.Collections.Generic;

public static class PlantTraitRegistry
{
	private static readonly Dictionary<string, PlantTrait> traits;

	private static readonly List<PlantTrait> ordered;

	public static IReadOnlyList<PlantTrait> All => ordered;

	static PlantTraitRegistry()
	{
		traits = new Dictionary<string, PlantTrait>();
		ordered = new List<PlantTrait>();
		PlantTraitList.Register();
	}

	public static PlantTrait Add(PlantTrait trait)
	{
		if (trait == null || string.IsNullOrEmpty(trait.id))
		{
			Debug.LogError("PlantTraitRegistry.Add: a trait needs an id");
			return trait;
		}
		if (traits.ContainsKey(trait.id))
		{
			ordered.RemoveAll((PlantTrait t) => t.id.Equals(trait.id));
		}
		traits[trait.id] = trait;
		ordered.Add(trait);
		return trait;
	}

	public static bool Remove(string id)
	{
		ordered.RemoveAll((PlantTrait t) => t.id.Equals(id));
		return traits.Remove(id);
	}

	public static bool Contains(string id)
	{
		if (id != null)
		{
			return traits.ContainsKey(id);
		}
		return false;
	}

	public static PlantTrait Get(string id)
	{
		if (id != null && traits.TryGetValue(id, out var value))
		{
			return value;
		}
		return null;
	}
}

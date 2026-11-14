using System;
using System.Collections.Generic;

public class RandomClientPool
{
	public class Entry
	{
		public Func<StoreClient> create;

		public int[] tierWeights;
	}

	public readonly string factionId;

	public readonly Func<int> getChance;

	public readonly List<Entry> entries = new List<Entry>();

	public RandomClientPool(string factionId, Func<int> getChance = null)
	{
		this.factionId = factionId;
		this.getChance = getChance;
	}

	public RandomClientPool Add(Func<StoreClient> create, int weight)
	{
		return Add(create, weight, weight, weight);
	}

	public RandomClientPool Add(Func<StoreClient> create, int weight, int weightFrom400, int weightFrom650)
	{
		entries.Add(new Entry
		{
			create = create,
			tierWeights = new int[3] { weight, weightFrom400, weightFrom650 }
		});
		return this;
	}

	public bool RollChance()
	{
		if (getChance != null)
		{
			return RNG.Roll(getChance());
		}
		return false;
	}

	public StoreClient Pick(int tier)
	{
		if (entries.Count == 0)
		{
			return null;
		}
		List<int> list = new List<int>(entries.Count);
		foreach (Entry entry in entries)
		{
			list.Add(entry.tierWeights[tier]);
		}
		return entries[RNG.GetOdd(list)].create();
	}
}

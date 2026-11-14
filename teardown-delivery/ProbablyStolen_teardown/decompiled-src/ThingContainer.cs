using System;
using System.Collections.Generic;
using System.Linq;

public class ThingContainer
{
	private Dictionary<EnergyType, long> thingDict;

	public double volume;

	public long heat;

	public ThingContainer(double volume = 1.0)
	{
		this.volume = volume;
		thingDict = new Dictionary<EnergyType, long>();
	}

	public long GetTotalMatter()
	{
		return GetValue(EnergyType.Matter);
	}

	public long GetTotalMass()
	{
		return GetValue(EnergyType.Matter);
	}

	public void BalanceHeat()
	{
	}

	private void Merge(Dictionary<EnergyType, long> source, Dictionary<EnergyType, long> target)
	{
		foreach (KeyValuePair<EnergyType, long> item in source)
		{
			if (target.ContainsKey(item.Key))
			{
				target[item.Key] += item.Value;
			}
			else
			{
				target.TryAdd(item.Key, item.Value);
			}
		}
	}

	public void Absorb(ThingContainer container)
	{
		Dictionary<EnergyType, long> dictionary = container.thingDict;
		container.thingDict = new Dictionary<EnergyType, long>();
		if (thingDict.Count() >= dictionary.Count())
		{
			Merge(dictionary, thingDict);
			return;
		}
		Merge(thingDict, dictionary);
		thingDict = dictionary;
	}

	public void Add(EnergyType thing, long value)
	{
		if (value < 0 || thing == EnergyType.None)
		{
			return;
		}
		List<KeyValuePair<EnergyType, long>> list = thingDict.Where((KeyValuePair<EnergyType, long> p) => p.Key.HasFlag(thing)).ToList();
		if (!list.Any())
		{
			thingDict.TryAdd(thing, value);
			return;
		}
		long num = value / list.Count();
		long num2 = value % list.Count();
		for (int i = 0; i < list.Count(); i++)
		{
			long value2 = list[i].Value + num + ((i < num2) ? 1 : 0);
			thingDict[list[i].Key] = value2;
		}
	}

	public long GetValue(EnergyType thing)
	{
		if (thing == EnergyType.None)
		{
			return 0L;
		}
		return (from p in thingDict
			where p.Key.HasFlag(thing)
			select p.Value).Sum();
	}

	public void Clear()
	{
		thingDict.Clear();
	}

	public long Remove(EnergyType thing, long value = long.MaxValue)
	{
		if (value < 0)
		{
			return 0L;
		}
		if (thing == EnergyType.None)
		{
			return value;
		}
		List<KeyValuePair<EnergyType, long>> list = thingDict.Where((KeyValuePair<EnergyType, long> p) => p.Key.HasFlag(thing)).ToList();
		if (!list.Any())
		{
			return value;
		}
		long totalValue = list.Select((KeyValuePair<EnergyType, long> p) => p.Value).Sum();
		long num = Math.Min(value, totalValue);
		long remainingAmount = totalValue - num;
		if (remainingAmount == 0L)
		{
			for (int i = 0; i < list.Count(); i++)
			{
				thingDict.Remove(list[i].Key);
			}
			return num;
		}
		List<long> list2 = list.Select((KeyValuePair<EnergyType, long> p) => (long)((double)p.Value / (double)totalValue * (double)remainingAmount)).ToList();
		long num2 = list2.Sum();
		long num3 = remainingAmount - num2;
		long num4 = num3 / list.Count();
		long num5 = num3 % list.Count();
		for (int j = 0; j < list.Count(); j++)
		{
			long num6 = list2[j] + num4 + ((j < num5) ? 1 : 0);
			if (num6 <= 0)
			{
				thingDict.Remove(list[j].Key);
			}
			else
			{
				thingDict[list[j].Key] = num6;
			}
		}
		return num;
	}
}

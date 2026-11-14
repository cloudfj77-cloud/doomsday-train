using System;
using System.Collections.Generic;
using UnityEngine;

public class PlantTrait
{
	public string id;

	public string englishName;

	public string englishDescription;

	public Func<string> nameProvider;

	public Func<string> descriptionProvider;

	public int complexity;

	public const int UNLIMITED_RANK = int.MaxValue;

	public int maxRank = 1;

	public string exclusiveGroup;

	public Dictionary<string, float> statPercentPerRank = new Dictionary<string, float>();

	public Action<PlantTraitContext> onHarvestItem;

	public Action<GameItem, int> onStartGrowing;

	public Action<GameItem, GameItem, int> onGrowTick;

	public Func<int, string> describeRank;

	public Func<PlantDefinition, bool> cropRequirement;

	public int complexityOffsetPerRank;

	public bool locked;

	public bool perennial;

	public string infusionLiquid;

	public List<(string moduleId, string liquidId)> conversions = new List<(string, string)>();

	public bool appliesToSeedProduce;

	public Func<GameItem, bool> seedRequirement;

	public List<string> incompatibleTraits = new List<string>();

	public string displayName
	{
		get
		{
			if (nameProvider == null)
			{
				return PlantGeneticsText.TraitName(id, englishName);
			}
			return nameProvider();
		}
		set
		{
			englishName = value;
		}
	}

	public string description
	{
		get
		{
			if (descriptionProvider == null)
			{
				return PlantGeneticsText.TraitDescription(id, englishDescription);
			}
			return descriptionProvider();
		}
		set
		{
			englishDescription = value;
		}
	}

	public bool IsUnlimited => maxRank == int.MaxValue;

	public bool IsNegative
	{
		get
		{
			if (complexity >= 0)
			{
				return complexityOffsetPerRank > 0;
			}
			return true;
		}
	}

	public bool IsChemical
	{
		get
		{
			if (string.IsNullOrEmpty(infusionLiquid))
			{
				return conversions.Count > 0;
			}
			return true;
		}
	}

	public bool IsSpecial
	{
		get
		{
			if (!perennial)
			{
				return IsChemical;
			}
			return true;
		}
	}

	public bool ActsOnProduce => onHarvestItem != null;

	public PlantTrait(string id, string displayName, int complexity, string description = "")
	{
		this.id = id;
		this.displayName = displayName;
		this.complexity = complexity;
		this.description = description;
	}

	public PlantTrait Stat(string stat, float percentAtRankOne)
	{
		statPercentPerRank[stat] = percentAtRankOne;
		return this;
	}

	public PlantTrait MaxRank(int rank)
	{
		maxRank = Mathf.Max(1, rank);
		return this;
	}

	public PlantTrait Unlimited()
	{
		maxRank = int.MaxValue;
		return this;
	}

	public PlantTrait Exclusive(string group)
	{
		exclusiveGroup = group;
		return this;
	}

	public PlantTrait OnHarvestItem(Action<PlantTraitContext> hook)
	{
		onHarvestItem = hook;
		return this;
	}

	public PlantTrait OnStartGrowing(Action<GameItem, int> hook)
	{
		onStartGrowing = hook;
		return this;
	}

	public PlantTrait OnGrowTick(Action<GameItem, GameItem, int> hook)
	{
		onGrowTick = hook;
		return this;
	}

	public PlantTrait Describe(Func<int, string> describe)
	{
		describeRank = describe;
		return this;
	}

	public PlantTrait Named(Func<string> name, Func<string> description)
	{
		nameProvider = name;
		descriptionProvider = description;
		return this;
	}

	public PlantTrait RequiresCrop(Func<PlantDefinition, bool> requirement)
	{
		cropRequirement = requirement;
		return this;
	}

	public PlantTrait ComplexityOffset(int perRank)
	{
		complexityOffsetPerRank = perRank;
		return this;
	}

	public PlantTrait Locked()
	{
		locked = true;
		return this;
	}

	public PlantTrait Perennial()
	{
		perennial = true;
		return this;
	}

	public PlantTrait Infuses(string liquidId)
	{
		infusionLiquid = liquidId;
		return this;
	}

	public PlantTrait AppliesToSeedProduce()
	{
		appliesToSeedProduce = true;
		return this;
	}

	public PlantTrait ConvertedBy(string moduleId, string liquidId)
	{
		conversions.Add((moduleId, liquidId));
		return this;
	}

	public IEnumerable<string> ProducedLiquids()
	{
		if (string.IsNullOrEmpty(infusionLiquid))
		{
			yield break;
		}
		yield return infusionLiquid;
		foreach (var conversion in conversions)
		{
			yield return conversion.liquidId;
		}
	}

	public PlantTrait RequiresSeed(Func<GameItem, bool> requirement)
	{
		seedRequirement = requirement;
		return this;
	}

	public PlantTrait IncompatibleWith(params string[] traitIds)
	{
		incompatibleTraits.AddRange(traitIds);
		return this;
	}

	public bool IsIncompatibleWith(string traitId)
	{
		return incompatibleTraits.Contains(traitId);
	}

	public float GetStatPercent(string stat, int rank)
	{
		if (rank <= 0 || !statPercentPerRank.TryGetValue(stat, out var value))
		{
			return 0f;
		}
		return PlantGeneticsHelper.RankedPercent(value, rank);
	}
}

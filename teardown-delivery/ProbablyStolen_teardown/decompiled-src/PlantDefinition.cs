using System;
using System.Collections.Generic;

public class PlantDefinition
{
	public string id;

	public string displayName;

	public string harvestItemId;

	public int requiredProgress = 10;

	public int harvestQuantity = 1;

	public int nutrimentLevel = 50;

	public int nutrimentRange = 50;

	public int mineralUseRate = 50;

	public double contaminantEffect = 1.0;

	public int waterUseRate = 100;

	public int startingHealth = 100;

	public int refruitProgress;

	public bool returnsSeeds;

	public bool producesDurableGoods;

	public Dictionary<string, int> innateTraits = new Dictionary<string, int>();

	public Action<PlantHarvestContext> onHarvest;

	public Action<GameItem, GameItem> onGrowTick;

	public bool HarvestIsOwnSeed => harvestItemId == id;

	public PlantDefinition(string id, string displayName, string harvestItemId, int requiredProgress, int harvestQuantity)
	{
		this.id = id;
		this.displayName = displayName;
		this.harvestItemId = harvestItemId;
		this.requiredProgress = requiredProgress;
		this.harvestQuantity = harvestQuantity;
	}

	public PlantDefinition Nutriment(int level, int range, int useRate)
	{
		nutrimentLevel = level;
		nutrimentRange = range;
		mineralUseRate = useRate;
		return this;
	}

	public PlantDefinition Contaminant(double effectMultiplier)
	{
		contaminantEffect = effectMultiplier;
		return this;
	}

	public PlantDefinition Water(int mlPerCycle)
	{
		waterUseRate = mlPerCycle;
		return this;
	}

	public PlantDefinition Refruit(int refruitProgress)
	{
		this.refruitProgress = refruitProgress;
		return this;
	}

	public PlantDefinition ReturnsSeeds()
	{
		returnsSeeds = true;
		return this;
	}

	public PlantDefinition DurableProduce()
	{
		producesDurableGoods = true;
		return this;
	}

	public PlantDefinition Innate(string traitId, int rank = 1)
	{
		innateTraits[traitId] = rank;
		return this;
	}

	public PlantDefinition OnHarvest(Action<PlantHarvestContext> harvest)
	{
		onHarvest = harvest;
		return this;
	}

	public PlantDefinition OnGrowTick(Action<GameItem, GameItem> tick)
	{
		onGrowTick = tick;
		return this;
	}

	public int GetInnateRank(string traitId)
	{
		if (!innateTraits.TryGetValue(traitId, out var value))
		{
			return 0;
		}
		return value;
	}
}

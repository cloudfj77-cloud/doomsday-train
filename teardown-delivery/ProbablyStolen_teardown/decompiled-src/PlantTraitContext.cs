using System.Collections.Generic;

public class PlantTraitContext
{
	public PlantTrait trait;

	public GameItem seed;

	public GameItem item;

	public int rank;

	public PlantHarvestContext harvest;

	public List<string> simulatedModules = new List<string>();

	public List<string> activeLiquids = new List<string>();

	public float Scale => PlantGeneticsHelper.RankScale(rank);

	public bool HasModule(string moduleId)
	{
		if (harvest != null)
		{
			return harvest.HasModule(moduleId);
		}
		return simulatedModules.Contains(moduleId);
	}

	public float RankedPercent(float percentAtRankOne)
	{
		return PlantGeneticsHelper.RankedPercent(percentAtRankOne, rank);
	}
}

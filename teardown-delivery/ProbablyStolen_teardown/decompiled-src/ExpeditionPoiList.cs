using System;
using System.Collections.Generic;

public class ExpeditionPoiList
{
	public static readonly Dictionary<string, Func<ExpeditionPoi>> AllPOI = new Dictionary<string, Func<ExpeditionPoi>>
	{
		{
			"junk_pile",
			() => JunkPile()
		},
		{
			"fresh_scrap_pile",
			() => FreshScrapPile()
		},
		{
			"scrap_pile",
			() => ScrapPile()
		}
	};

	public static ExpeditionPoi CreateExpeditionPoi(string id)
	{
		if (AllPOI.ContainsKey(id))
		{
			return AllPOI[id]();
		}
		Debug.LogError("Expedition POI with id " + id + " not found.");
		return null;
	}

	public static ExpeditionPoi JunkPile()
	{
		return new ExpeditionPoi
		{
			id = "junk_pile",
			displayName = "Worthless Junk Pile",
			value = 10,
			danger = 10,
			successChance = 85,
			actionDisplay = "I try scavenging the worthless junk pile...",
			poiType = ExpeditionPoi.ExpeditionPoiType.Scavenge,
			lootTable = "junkTable",
			isRepeatable = true
		};
	}

	public static ExpeditionPoi FreshScrapPile()
	{
		return new ExpeditionPoi
		{
			id = "fresh_scrap_pile",
			displayName = "Fresh Scrap Pile",
			value = 40,
			danger = 10,
			successChance = 85,
			actionDisplay = "I try scavenging the fresh scrap pile...",
			poiType = ExpeditionPoi.ExpeditionPoiType.Scavenge,
			lootTableGroup = "dumpingGroundFreshTG",
			isRepeatable = true
		};
	}

	public static ExpeditionPoi ScrapPile()
	{
		return new ExpeditionPoi
		{
			id = "scrap_pile",
			displayName = "Scrap Pile",
			value = 20,
			danger = 10,
			successChance = 85,
			actionDisplay = "I try scavenging the scrap pile...",
			poiType = ExpeditionPoi.ExpeditionPoiType.Scavenge,
			lootTableGroup = "dumpingGroundTG",
			isRepeatable = true
		};
	}
}

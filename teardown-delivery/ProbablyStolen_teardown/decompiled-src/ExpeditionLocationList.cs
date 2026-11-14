using System.Collections.Generic;

public class ExpeditionLocationList
{
	public static ExpeditionLocation DumpingGrounds()
	{
		return new ExpeditionLocation
		{
			id = "dumping_grounds",
			displayName = "Dumping Grounds",
			description = "An expansive junkyard filled with discarded scrap.",
			dangerRating = 1,
			footTraffic = 5,
			legality = 5,
			possibleDanger = new List<string> { "Hostile Scavengers", "Toxic Waste", "Mechanical Traps" },
			possibleLoot = new List<string> { "Scrap Metal", "Old Electronics", "Rare Components" },
			isUnlocked = true,
			POITable = "POI_dumpingGround"
		};
	}
}

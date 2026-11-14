using System.Collections.Generic;

public class ExpeditionLocation
{
	public string id;

	public string displayName;

	public string description;

	public int dangerRating;

	public int footTraffic;

	public int legality;

	public List<string> possibleDanger;

	public List<string> possibleLoot;

	public bool isUnlocked;

	public string POITable;

	public ExpeditionPoi PullPOI()
	{
		return ExpeditionPoiList.CreateExpeditionPoi(TableMaster.Roll(POITable));
	}

	public bool IsDumpingGrounds()
	{
		return id.Equals("dumping_grounds");
	}
}

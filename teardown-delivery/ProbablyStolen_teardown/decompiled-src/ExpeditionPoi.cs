public class ExpeditionPoi
{
	public enum ExpeditionPoiType
	{
		Scavenge,
		Combat,
		Thieving
	}

	public string id;

	public string displayName;

	public string actionDisplay;

	public int value = 10;

	public int danger = 10;

	public string lootTable;

	public string lootTableGroup;

	public bool isRepeatable = true;

	public ExpeditionPoiType poiType;

	public int successChance = 100;

	public float GetRewardRiskFactor()
	{
		return (float)value / (float)(value + danger);
	}

	public GameItem PullLoot()
	{
		if (lootTableGroup != null)
		{
			string text = TableGroupMaster.Roll(lootTableGroup);
			if (text == null)
			{
				Debug.LogError("Expedition POI " + id + " rolled null from loot table group " + lootTableGroup + ". Did you mean to set to table instead of tableGroup in the POI definition?");
				return null;
			}
			return ItemSpawner.Spawn(text);
		}
		if (!string.IsNullOrEmpty(lootTable))
		{
			string text2 = TableMaster.Roll(lootTable);
			if (text2 == null)
			{
				Debug.LogError("Expedition POI " + id + " rolled null from loot table " + lootTable + ". Did you mean to set to tableGroup instead of table in the POI definition?");
				return null;
			}
			return ItemSpawner.Spawn(text2);
		}
		Debug.LogError("Expedition POI " + id + " has no loot table or loot table group defined.");
		return null;
	}
}

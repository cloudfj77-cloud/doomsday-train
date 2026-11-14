public class ItemSpawner
{
	public static GameItem Spawn(string itemID)
	{
		if (itemID != null && PreBuiltItemHelper.preBuiltItems.ContainsKey(itemID))
		{
			return PreBuiltItemHelper.preBuiltItems[itemID]();
		}
		return DirectoryMaster.Item(itemID);
	}

	public static GameItem SpawnFromTableGroup(string tableGroupID)
	{
		return Spawn(TableGroupMaster.Roll(tableGroupID));
	}

	public static GameItem SpawnFromTable(string tableID)
	{
		return Spawn(TableMaster.Roll(tableID));
	}
}

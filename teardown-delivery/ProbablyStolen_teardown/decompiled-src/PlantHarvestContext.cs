using System.Collections.Generic;

public class PlantHarvestContext
{
	public PlantDefinition plant;

	public GameItem seed;

	public GameItem hydroponic;

	public GameGridInventory output;

	public List<GameItem> modules;

	public int health;

	public int harvestCount;

	public bool HasModule(string moduleId)
	{
		foreach (GameItem module in modules)
		{
			if (module.identifier.Equals(moduleId))
			{
				return true;
			}
		}
		return false;
	}

	public int CountModule(string moduleId)
	{
		int num = 0;
		foreach (GameItem module in modules)
		{
			if (module.identifier.Equals(moduleId))
			{
				num++;
			}
		}
		return num;
	}

	public void Output(GameItem item)
	{
		PlantHarvest.Output(this, item);
	}

	public void OutputById(string itemId, int count = 1)
	{
		for (int i = 0; i < count; i++)
		{
			Output(DirectoryMaster.Item(itemId));
		}
	}
}

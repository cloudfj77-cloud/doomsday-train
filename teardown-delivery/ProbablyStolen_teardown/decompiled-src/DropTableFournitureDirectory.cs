using System.Collections.Generic;

public class DropTableFournitureDirectory
{
	public static DropTable FournitureDropTableLivingQuarter(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 3;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 4;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 5;
			break;
		}
		dropTable.AddDrop(3, () => PopulateFurniture(DirectoryMaster.Item("bed")));
		dropTable.AddDrop(3, () => PopulateFurniture(DirectoryMaster.Item("mirrior_cabinet")));
		dropTable.AddDrop(3, () => PopulateFurniture(DirectoryMaster.Item("end_table")));
		return dropTable;
	}

	public static GameItem PopulateFurniture(GameItem fourniture)
	{
		Drops drops = new Drops();
		if (fourniture.identifier.Equals("bed"))
		{
			drops.probaTable.Add(DropTableDirectory.DropTableAmenities(DropDirectory.DropTableSize.DESTITUTE));
			drops.probaTable.Add(DropTableDirectory.DropTableEquipment(DropDirectory.DropTableSize.DESTITUTE));
			drops.probaTable.Add(DropTableDirectory.DropTableBasicMedical(DropDirectory.DropTableSize.DESTITUTE));
			drops.probaTable.Add(DropTableDirectory.DropTableFood(DropDirectory.DropTableSize.DESTITUTE));
		}
		List<GameItem> drops2 = drops.GetDrops();
		AddLootToFourniture(fourniture, drops2);
		return fourniture;
	}

	public static void AddLootToFourniture(GameItem fourniture, List<GameItem> items)
	{
		foreach (GameItem item in items)
		{
			if (GraphUtils.TryAcceptAll(fourniture, item) > 0)
			{
				Debug.Log("Not all loot could be inserted");
			}
		}
	}

	public static DropTable DropTableExemple(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 3;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 4;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 5;
			break;
		}
		return dropTable;
	}
}

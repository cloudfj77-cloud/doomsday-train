using System.Collections.Generic;

public class Drops
{
	public List<DropTable> alwaysTable { get; set; }

	public List<DropTable> probaTable { get; set; }

	public List<int> neverTable { get; set; }

	public Drops()
	{
		probaTable = new List<DropTable>();
		alwaysTable = new List<DropTable>();
		DropTable item = new DropTable
		{
			isAlways = true
		};
		alwaysTable.Add(item);
	}

	public void AddAlways(GameItem item)
	{
		alwaysTable[0].AddDrop(1, () => item);
	}

	public List<GameItem> GetDrops()
	{
		List<GameItem> list = new List<GameItem>();
		foreach (DropTable item in alwaysTable)
		{
			List<GameItem> dropped = item.GetDropped();
			list.AddRange(dropped);
		}
		foreach (DropTable item2 in probaTable)
		{
			List<GameItem> dropped2 = item2.GetDropped();
			list.AddRange(dropped2);
		}
		return list;
	}
}

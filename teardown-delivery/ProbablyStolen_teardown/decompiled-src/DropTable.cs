using System;
using System.Collections.Generic;
using UnityEngine;

public class DropTable
{
	public int totalWeight = -1;

	public bool isAlways;

	public int tableCount { get; set; }

	public int tableProba { get; set; }

	public List<int> rolls { get; set; }

	public List<DropTableItem> dropTableItems { get; set; }

	public DropTable()
	{
		tableCount = 1;
		tableProba = 100;
		rolls = new List<int>();
		dropTableItems = new List<DropTableItem>();
		isAlways = false;
	}

	public int GetSum()
	{
		int num = 0;
		foreach (DropTableItem dropTableItem in dropTableItems)
		{
			num += dropTableItem.weight;
		}
		totalWeight = num;
		return num;
	}

	protected List<GameItem> GetDroppedAlways()
	{
		List<GameItem> list = new List<GameItem>();
		foreach (DropTableItem dropTableItem in dropTableItems)
		{
			list.Add(dropTableItem.dropFunction(dropTableItem.GetQuantity()));
		}
		return list;
	}

	public List<GameItem> GetDropped()
	{
		if (isAlways)
		{
			return GetDroppedAlways();
		}
		return GetDroppedProba();
	}

	protected List<GameItem> GetDroppedProba()
	{
		List<GameItem> list = new List<GameItem>();
		int sum = GetSum();
		rolls.Clear();
		for (int i = 0; i < tableCount; i++)
		{
			rolls.Add(UnityEngine.Random.Range(0, sum));
		}
		if (tableProba == 100 || UnityEngine.Random.Range(0, 100) < tableProba)
		{
			foreach (int roll in rolls)
			{
				int num = 0;
				foreach (DropTableItem dropTableItem in dropTableItems)
				{
					if (roll < dropTableItem.weight + num)
					{
						list.Add(dropTableItem.dropFunction(dropTableItem.GetQuantity()));
						break;
					}
					num += dropTableItem.weight;
				}
			}
		}
		return list;
	}

	public void AddDrop(int weigth, Func<GameItem> itemFunction)
	{
		DropTableItem dropTableItem = new DropTableItem();
		dropTableItem.dropFunction = (int num) => itemFunction();
		dropTableItem.weight = weigth;
		dropTableItems.Add(dropTableItem);
	}
}

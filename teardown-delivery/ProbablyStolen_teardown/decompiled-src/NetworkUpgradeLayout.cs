using System;
using System.Collections.Generic;

public static class NetworkUpgradeLayout
{
	public class Cell
	{
		public string id;

		public int column;

		public int row;

		public bool connectorToNext;
	}

	public class Card
	{
		public string subtitle;

		public List<Cell> cells = new List<Cell>();

		public int rows = 1;

		public bool recommended;

		public int columns = 3;
	}

	public class Tab
	{
		public string tab;

		public List<Card> cards = new List<Card>();
	}

	public const int COLUMNS = 3;

	public const int RECOMMENDED_COLUMNS = 10;

	public const int MIN_ROWS = 1;

	public static List<Tab> Build(IList<NetworkUpgrade> definitionsInOrder)
	{
		List<Tab> list = new List<Tab>();
		Dictionary<string, Tab> tabsById = new Dictionary<string, Tab>();
		Dictionary<string, List<NetworkUpgrade>> dictionary = new Dictionary<string, List<NetworkUpgrade>>();
		Dictionary<string, string> cardOfId = new Dictionary<string, string>();
		GetOrAddTab(list, tabsById, "");
		foreach (NetworkUpgrade item in definitionsInOrder)
		{
			if (item != null && !string.IsNullOrEmpty(item.id))
			{
				string text = item.tab ?? "";
				string text2 = item.subtitle ?? "";
				Tab orAddTab = GetOrAddTab(list, tabsById, text);
				string text3 = text + "\n" + text2;
				if (!dictionary.TryGetValue(text3, out var value))
				{
					value = (dictionary[text3] = new List<NetworkUpgrade>());
					orAddTab.cards.Add(new Card
					{
						subtitle = text2
					});
				}
				value.Add(item);
				cardOfId[item.id] = text3;
			}
		}
		foreach (Tab item2 in list)
		{
			foreach (Card card2 in item2.cards)
			{
				string cardKey = item2.tab + "\n" + card2.subtitle;
				PlaceCard(card2, dictionary[cardKey], (string id) => cardOfId.TryGetValue(id, out var value2) && value2 == cardKey, 3);
			}
			List<NetworkUpgrade> list3 = new List<NetworkUpgrade>();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (NetworkUpgrade item3 in definitionsInOrder)
			{
				if (item3 != null && item3.recommended && !((item3.tab ?? "") != item2.tab) && hashSet.Add(item3.id))
				{
					list3.Add(item3);
				}
			}
			if (list3.Count > 0)
			{
				Card card = new Card
				{
					subtitle = "",
					recommended = true,
					columns = 10
				};
				PlaceCard(card, list3, hashSet.Contains, 10);
				item2.cards.Insert(0, card);
			}
		}
		return list;
	}

	private static Tab GetOrAddTab(List<Tab> tabs, Dictionary<string, Tab> tabsById, string tabId)
	{
		if (tabsById.TryGetValue(tabId, out var value))
		{
			return value;
		}
		value = (tabsById[tabId] = new Tab
		{
			tab = tabId
		});
		tabs.Add(value);
		return value;
	}

	private static void PlaceCard(Card card, List<NetworkUpgrade> members, Func<string, bool> isInCard, int columns)
	{
		Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (NetworkUpgrade member in members)
		{
			if (member.prerequisite == null)
			{
				continue;
			}
			foreach (string item in member.prerequisite)
			{
				if (isInCard(item))
				{
					if (!dictionary.TryGetValue(item, out var value))
					{
						value = (dictionary[item] = new List<string>());
					}
					value.Add(member.id);
					hashSet.Add(member.id);
					break;
				}
			}
		}
		HashSet<string> hashSet2 = new HashSet<string>();
		List<List<string>> list2 = new List<List<string>>();
		for (int i = 0; i < 2; i++)
		{
			foreach (NetworkUpgrade member2 in members)
			{
				if (hashSet2.Contains(member2.id) || (i == 0 && hashSet.Contains(member2.id)))
				{
					continue;
				}
				List<string> list3 = new List<string>();
				string text = member2.id;
				while (text != null && hashSet2.Add(text))
				{
					list3.Add(text);
					string text2 = null;
					if (dictionary.TryGetValue(text, out var value2))
					{
						foreach (string item2 in value2)
						{
							if (!hashSet2.Contains(item2))
							{
								text2 = item2;
								break;
							}
						}
					}
					text = text2;
				}
				list2.Add(list3);
			}
		}
		List<List<Cell>> list4 = new List<List<Cell>>();
		foreach (List<string> item3 in list2)
		{
			for (int j = 0; j < item3.Count; j += columns)
			{
				int num = Math.Min(columns, item3.Count - j);
				List<Cell> list5 = null;
				foreach (List<Cell> item4 in list4)
				{
					if (columns - item4.Count >= num)
					{
						list5 = item4;
						break;
					}
				}
				if (list5 == null)
				{
					list5 = new List<Cell>();
					list4.Add(list5);
				}
				for (int k = 0; k < num; k++)
				{
					list5.Add(new Cell
					{
						id = item3[j + k],
						connectorToNext = (k < num - 1)
					});
				}
			}
		}
		for (int l = 0; l < list4.Count; l++)
		{
			for (int m = 0; m < list4[l].Count; m++)
			{
				Cell cell = list4[l][m];
				cell.row = l;
				cell.column = m;
				card.cells.Add(cell);
			}
		}
		card.rows = Math.Max(1, list4.Count);
	}
}

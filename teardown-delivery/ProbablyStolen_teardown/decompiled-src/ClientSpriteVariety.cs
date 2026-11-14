using System.Collections.Generic;
using System.Linq;
using System.Text;
using QFSW.QC;

public static class ClientSpriteVariety
{
	public const int RECENT_DAYS = 4;

	public const int HISTORY_LENGTH = 80;

	private static StoreClientManager Manager
	{
		get
		{
			if (PlayerStore.Instance == null)
			{
				return null;
			}
			return PlayerStore.Instance.storeClientManager;
		}
	}

	private static int Today
	{
		get
		{
			if (StoreStation.Instance == null)
			{
				return 0;
			}
			return StoreStation.Instance.dayCounter;
		}
	}

	public static void OnClientShown(string spriteName)
	{
		StoreClientManager manager = Manager;
		if (manager != null && !string.IsNullOrEmpty(spriteName) && !(spriteName == "default"))
		{
			EnsureState(manager);
			manager.spriteLastShownDay[spriteName] = Today;
			manager.spriteShownHistory.Remove(spriteName);
			manager.spriteShownHistory.Add(spriteName);
			while (manager.spriteShownHistory.Count > 80)
			{
				manager.spriteShownHistory.RemoveAt(0);
			}
		}
	}

	public static void EnsureState(StoreClientManager manager)
	{
		if (manager.spriteLastShownDay == null)
		{
			manager.spriteLastShownDay = new Dictionary<string, int>();
		}
		if (manager.spriteShownHistory == null)
		{
			manager.spriteShownHistory = new List<string>();
		}
	}

	public static string PickGendered(List<string> poolA, List<string> poolB)
	{
		bool flag = RNG.Roll(50);
		return Pick(flag ? poolA : poolB, flag ? poolB : poolA);
	}

	public static string Pick(List<string> pool)
	{
		return Pick(pool, null);
	}

	public static string Pick(List<string> preferred, List<string> fallback)
	{
		if ((preferred == null || preferred.Count == 0) && (fallback == null || fallback.Count == 0))
		{
			return "default";
		}
		if (preferred == null || preferred.Count == 0)
		{
			preferred = fallback;
			fallback = null;
		}
		StoreClientManager manager = Manager;
		if (manager == null)
		{
			return preferred[RNG.GetRandomIntExculsive(0, preferred.Count)];
		}
		EnsureState(manager);
		HashSet<string> today = CollectToday(manager);
		string text = PickFresh(preferred, today, manager);
		if (text != null)
		{
			return text;
		}
		if (fallback != null && fallback.Count > 0)
		{
			text = PickFresh(fallback, today, manager);
			if (text != null)
			{
				return text;
			}
		}
		List<string> list = new List<string>(preferred);
		if (fallback != null)
		{
			list.AddRange(fallback);
		}
		return PickLeastRecent(list, manager, QueuedFaces(manager));
	}

	private static List<string> QueuedFaces(StoreClientManager manager)
	{
		List<string> list = new List<string>();
		if (manager.clientStack == null)
		{
			return list;
		}
		foreach (StoreClient item in manager.clientStack)
		{
			if (item != null && !string.IsNullOrEmpty(item.spriteName))
			{
				list.Add(item.spriteName);
			}
		}
		return list;
	}

	private static HashSet<string> CollectToday(StoreClientManager manager)
	{
		HashSet<string> hashSet = new HashSet<string>();
		int today = Today;
		foreach (KeyValuePair<string, int> item in manager.spriteLastShownDay)
		{
			if (item.Value == today)
			{
				hashSet.Add(item.Key);
			}
		}
		hashSet.UnionWith(QueuedFaces(manager));
		return hashSet;
	}

	private static string PickFresh(List<string> pool, HashSet<string> today, StoreClientManager manager)
	{
		int today2 = Today;
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		int num = int.MaxValue;
		foreach (string item in pool)
		{
			if (today.Contains(item))
			{
				continue;
			}
			if (!manager.spriteLastShownDay.TryGetValue(item, out var value) || today2 - value >= 4)
			{
				list.Add(item);
				continue;
			}
			if (value < num)
			{
				num = value;
				list2.Clear();
			}
			if (value == num)
			{
				list2.Add(item);
			}
		}
		if (list.Count > 0)
		{
			return list[RNG.GetRandomIntExculsive(0, list.Count)];
		}
		if (list2.Count > 0)
		{
			return list2[RNG.GetRandomIntExculsive(0, list2.Count)];
		}
		return null;
	}

	private static string PickLeastRecent(List<string> pool, StoreClientManager manager, List<string> queued)
	{
		string text = null;
		int num = int.MaxValue;
		List<string> list = new List<string>();
		foreach (string item in pool)
		{
			int num2 = queued.IndexOf(item);
			int num3 = ((num2 < 0) ? manager.spriteShownHistory.IndexOf(item) : (manager.spriteShownHistory.Count + num2));
			if (num3 < num)
			{
				num = num3;
				text = item;
				list.Clear();
				list.Add(item);
			}
			else if (num3 == num)
			{
				list.Add(item);
			}
		}
		if (list.Count > 1)
		{
			return list[RNG.GetRandomIntExculsive(0, list.Count)];
		}
		return text ?? pool[0];
	}

	[Command("client-faces", "Lists the customer faces shown recently, most recent last, with the day each was last seen", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string ListHistory()
	{
		StoreClientManager manager = Manager;
		if (manager == null)
		{
			return "no store loaded";
		}
		EnsureState(manager);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"day {Today}, {manager.spriteShownHistory.Count} faces remembered (RECENT_DAYS {4})");
		foreach (string item in manager.spriteShownHistory)
		{
			manager.spriteLastShownDay.TryGetValue(item, out var value);
			stringBuilder.AppendLine($"  {item}  (day {value})");
		}
		if (manager.clientStack != null && manager.clientStack.Count > 0)
		{
			stringBuilder.AppendLine("queued: " + string.Join(", ", from c in manager.clientStack
				where c != null
				select c.spriteName));
		}
		return stringBuilder.ToString();
	}
}

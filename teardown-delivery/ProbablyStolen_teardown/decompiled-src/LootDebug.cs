using System.Collections.Generic;
using System.Linq;
using System.Text;
using QFSW.QC;

public static class LootDebug
{
	[Command("loot-list", "Lists every loot table and table group, or the entries of one with their effective odds and the mod that added them", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string List(string id = null)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (string.IsNullOrEmpty(id))
		{
			stringBuilder.AppendLine("Loot tables:");
			foreach (string item in LootRegistry.TableIds.OrderBy((string x) => x))
			{
				stringBuilder.AppendLine("  " + Summary(item, LootRegistry.GetTableEntries(item)));
			}
			stringBuilder.AppendLine("Table groups:");
			foreach (string item2 in LootRegistry.GroupIds.OrderBy((string x) => x))
			{
				stringBuilder.AppendLine("  " + Summary(item2, LootRegistry.GetGroupEntries(item2)));
			}
			return stringBuilder.ToString();
		}
		bool flag = LootRegistry.HasTable(id);
		bool flag2 = LootRegistry.HasGroup(id);
		if (!flag && !flag2)
		{
			return "No loot table or table group named '" + id + "'.";
		}
		if (flag)
		{
			AppendEntries(stringBuilder, "Loot table", id, LootRegistry.GetTableEntries(id), isGroup: false);
		}
		if (flag2)
		{
			AppendEntries(stringBuilder, "Table group", id, LootRegistry.GetGroupEntries(id), isGroup: true);
		}
		return stringBuilder.ToString();
	}

	[Command("loot-roll", "Rolls a loot table or table group a number of times and tallies the results", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Roll(string id, int count = 1)
	{
		bool flag = LootRegistry.HasTable(id);
		bool flag2 = LootRegistry.HasGroup(id);
		if (!flag && !flag2)
		{
			return "No loot table or table group named '" + id + "'.";
		}
		if (flag && flag2)
		{
			return "'" + id + "' is both a loot table and a table group; rename one of them.";
		}
		count = ((count < 1) ? 1 : ((count > 100000) ? 100000 : count));
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		for (int i = 0; i < count; i++)
		{
			string key = (flag ? LootRegistry.Roll(id) : LootRegistry.RollGroup(id)) ?? "(null)";
			dictionary[key] = ((!dictionary.TryGetValue(key, out var value)) ? 1 : (value + 1));
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("{0} '{1}', {2} roll(s):", flag ? "Loot table" : "Table group", id, count));
		foreach (KeyValuePair<string, int> item in from p in dictionary
			orderby p.Value descending, p.Key
			select p)
		{
			stringBuilder.AppendLine($"  {item.Key,-32} {item.Value,7}  {100.0 * (double)item.Value / (double)count,6:0.00}%");
		}
		return stringBuilder.ToString();
	}

	private static string Summary(string id, List<LootEntry> entries)
	{
		int num = entries.Count((LootEntry e) => e.sourceModId != null || e.modifiedByModId != null);
		string text = ((num > 0) ? $", {num} touched by mods" : "");
		return $"{id,-32} {entries.Count,3} entries, total weight {TotalWeight(entries):0.##}{text}";
	}

	private static void AppendEntries(StringBuilder result, string kind, string id, List<LootEntry> entries, bool isGroup)
	{
		double num = TotalWeight(entries);
		result.AppendLine($"{kind} '{id}': {entries.Count} entries, total weight {num:0.##} (vanilla tables sum to {1000f:0})");
		foreach (LootEntry item in from e in entries
			orderby e.weight descending, e.id
			select e)
		{
			double num2 = ((num > 0.0 && item.weight > 0f) ? (100.0 * (double)item.weight / num) : 0.0);
			result.AppendLine($"  {item.id,-32} weight {item.weight,9:0.###}  {num2,7:0.000}%  {Note(item, isGroup)}");
		}
	}

	private static string Note(LootEntry entry, bool isGroup)
	{
		List<string> list = new List<string>();
		if (entry.sourceModId != null)
		{
			list.Add("added by " + entry.sourceModId);
		}
		if (entry.modifiedByModId != null)
		{
			list.Add("weight set by " + entry.modifiedByModId);
		}
		if (isGroup)
		{
			if (!LootRegistry.HasTable(entry.id))
			{
				list.Add("no loot table with this id");
			}
		}
		else if (!DirectoryMaster.Has<GameItem>(entry.id) && !PreBuiltItemHelper.preBuiltItems.ContainsKey(entry.id))
		{
			list.Add("not an item id");
		}
		if (list.Count <= 0)
		{
			return "";
		}
		return "(" + string.Join(", ", list) + ")";
	}

	private static double TotalWeight(List<LootEntry> entries)
	{
		double num = 0.0;
		foreach (LootEntry entry in entries)
		{
			if (entry.weight > 0f)
			{
				num += (double)entry.weight;
			}
		}
		return num;
	}
}

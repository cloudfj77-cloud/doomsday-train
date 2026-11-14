using System;
using System.Collections.Generic;
using RNGNeeds;
using UnityEngine;

public static class LootRegistry
{
	private enum ListKind
	{
		Table,
		Group
	}

	private enum OpType
	{
		Register,
		Clear,
		Add,
		Remove,
		SetWeight
	}

	private class Op
	{
		public ListKind kind;

		public OpType type;

		public string modId;

		public string listId;

		public string entryId;

		public float weight;

		public List<LootEntry> entries;
	}

	public const float VANILLA_TOTAL_WEIGHT = 1000f;

	private const string LOG = "[LootRegistry] ";

	private static readonly Dictionary<string, List<LootEntry>> vanillaTables = new Dictionary<string, List<LootEntry>>();

	private static readonly Dictionary<string, List<LootEntry>> vanillaGroups = new Dictionary<string, List<LootEntry>>();

	private static readonly Dictionary<string, List<LootEntry>> tables = new Dictionary<string, List<LootEntry>>();

	private static readonly Dictionary<string, List<LootEntry>> groups = new Dictionary<string, List<LootEntry>>();

	private static readonly List<Op> journal = new List<Op>();

	private static readonly HashSet<string> warned = new HashSet<string>();

	private static bool dirty = true;

	private static bool vanillaEverLoaded;

	private static bool tablesLoadedThisScene;

	private static bool groupsLoadedThisScene;

	public static IEnumerable<string> TableIds
	{
		get
		{
			RebuildIfDirty();
			return tables.Keys;
		}
	}

	public static IEnumerable<string> GroupIds
	{
		get
		{
			RebuildIfDirty();
			return groups.Keys;
		}
	}

	internal static void LoadVanillaTables(List<TableEntry> entries)
	{
		foreach (TableEntry entry in entries)
		{
			if (!string.IsNullOrEmpty(entry.tableId) && !(entry.lootTable == null))
			{
				vanillaTables[entry.tableId] = Flatten(entry.lootTable.table, entry.tableId, (string id) => id);
			}
		}
		tablesLoadedThisScene = true;
		OnVanillaLoaded();
	}

	internal static void LoadVanillaGroups(List<TableGroupEntry> entries)
	{
		foreach (TableGroupEntry entry in entries)
		{
			if (!string.IsNullOrEmpty(entry.tableGroupId) && !(entry.tableGroup == null))
			{
				vanillaGroups[entry.tableGroupId] = Flatten(entry.tableGroup.tableGroup, entry.tableGroupId, ResolveTableId);
			}
		}
		groupsLoadedThisScene = true;
		OnVanillaLoaded();
	}

	private static void OnVanillaLoaded()
	{
		vanillaEverLoaded = true;
		dirty = true;
		if (tablesLoadedThisScene && groupsLoadedThisScene)
		{
			tablesLoadedThisScene = false;
			groupsLoadedThisScene = false;
			RebuildIfDirty();
			ModHook.FireOnLootTablesLoaded();
		}
	}

	private static string ResolveTableId(LootTable asset)
	{
		if (asset == null)
		{
			return null;
		}
		string text = ((TableMaster.Instance != null) ? TableMaster.Instance.GetTableId(asset) : null);
		if (text != null)
		{
			return text;
		}
		text = asset.name;
		if (!vanillaTables.ContainsKey(text))
		{
			vanillaTables[text] = Flatten(asset.table, text, (string x) => x);
			WarnOnce("unlisted:" + text, "Loot table asset '" + asset.name + "' is used by a table group but is not listed on TableMaster. Registered it as '" + text + "'; add it to the TableMaster list.");
		}
		return text;
	}

	private static List<LootEntry> Flatten<T>(ProbabilityList<T> list, string listId, Func<T, string> toId)
	{
		List<LootEntry> list2 = new List<LootEntry>();
		if (list == null)
		{
			return list2;
		}
		double num = 0.0;
		for (int i = 0; i < list.ItemCount; i++)
		{
			ProbabilityItem<T> probabilityItem = list.GetProbabilityItem(i);
			if (probabilityItem.Enabled)
			{
				num += (double)probabilityItem.BaseProbability;
			}
		}
		if (num <= 0.0)
		{
			WarnOnce("empty:" + listId, "'" + listId + "' has no enabled entry with a probability above zero; it will never roll anything.");
			return list2;
		}
		for (int j = 0; j < list.ItemCount; j++)
		{
			ProbabilityItem<T> probabilityItem2 = list.GetProbabilityItem(j);
			if (!probabilityItem2.Enabled || probabilityItem2.BaseProbability <= 0f)
			{
				continue;
			}
			string id = toId(probabilityItem2.Value);
			if (string.IsNullOrEmpty(id))
			{
				WarnOnce($"blank:{listId}:{j}", $"'{listId}' entry {j} has an empty value and was skipped.");
				continue;
			}
			float num2 = (float)((double)probabilityItem2.BaseProbability / num * 1000.0);
			LootEntry lootEntry = list2.Find((LootEntry e) => e.id == id);
			if (lootEntry != null)
			{
				lootEntry.weight += num2;
			}
			else
			{
				list2.Add(new LootEntry(id, num2));
			}
		}
		return list2;
	}

	public static void WarnUnsupportedSettings<T>(ProbabilityList<T> list, string assetName)
	{
		if (list != null)
		{
			List<string> list2 = new List<string>();
			if (list.PreventRepeat != 0)
			{
				list2.Add("Prevent Repeat");
			}
			if (list.IsDepletable)
			{
				list2.Add("Depletable List");
			}
			if (list.PickCountMin != 1 || list.PickCountMax != 1)
			{
				list2.Add("Pick Count other than 1");
			}
			if (list.IsListInfluenced)
			{
				list2.Add("Influence Provider");
			}
			if (list2.Count != 0)
			{
				Debug.LogWarning("[LootRegistry] '" + assetName + "' uses " + string.Join(", ", list2) + ". Loot is rolled from the flattened weights only, so these settings have no effect in play.");
			}
		}
	}

	public static void RegisterTable(string modId, string tableId, IEnumerable<LootEntry> entries)
	{
		Record(ListKind.Table, OpType.Register, modId, tableId, null, 0f, CopyEntries(entries, modId, tableId));
	}

	public static void RegisterTable(string modId, string tableId, params (string itemId, float weight)[] entries)
	{
		RegisterTable(modId, tableId, ToEntries(entries));
	}

	public static void ClearTable(string modId, string tableId)
	{
		Record(ListKind.Table, OpType.Clear, modId, tableId);
	}

	public static void AddEntry(string modId, string tableId, string itemId, float weight)
	{
		Record(ListKind.Table, OpType.Add, modId, tableId, itemId, weight);
	}

	public static void RemoveEntry(string modId, string tableId, string itemId)
	{
		Record(ListKind.Table, OpType.Remove, modId, tableId, itemId);
	}

	public static void SetWeight(string modId, string tableId, string itemId, float weight)
	{
		Record(ListKind.Table, OpType.SetWeight, modId, tableId, itemId, weight);
	}

	public static void RegisterGroup(string modId, string groupId, IEnumerable<LootEntry> entries)
	{
		Record(ListKind.Group, OpType.Register, modId, groupId, null, 0f, CopyEntries(entries, modId, groupId));
	}

	public static void RegisterGroup(string modId, string groupId, params (string tableId, float weight)[] entries)
	{
		RegisterGroup(modId, groupId, ToEntries(entries));
	}

	public static void ClearGroup(string modId, string groupId)
	{
		Record(ListKind.Group, OpType.Clear, modId, groupId);
	}

	public static void AddGroupEntry(string modId, string groupId, string tableId, float weight)
	{
		Record(ListKind.Group, OpType.Add, modId, groupId, tableId, weight);
	}

	public static void RemoveGroupEntry(string modId, string groupId, string tableId)
	{
		Record(ListKind.Group, OpType.Remove, modId, groupId, tableId);
	}

	public static void SetGroupWeight(string modId, string groupId, string tableId, float weight)
	{
		Record(ListKind.Group, OpType.SetWeight, modId, groupId, tableId, weight);
	}

	public static bool HasTable(string tableId)
	{
		RebuildIfDirty();
		if (tableId != null)
		{
			return tables.ContainsKey(tableId);
		}
		return false;
	}

	public static bool HasGroup(string groupId)
	{
		RebuildIfDirty();
		if (groupId != null)
		{
			return groups.ContainsKey(groupId);
		}
		return false;
	}

	public static List<LootEntry> GetTableEntries(string tableId)
	{
		RebuildIfDirty();
		return Snapshot(tables, tableId);
	}

	public static List<LootEntry> GetGroupEntries(string groupId)
	{
		RebuildIfDirty();
		return Snapshot(groups, groupId);
	}

	public static string Roll(string tableId)
	{
		return RollInternal(tableId, null);
	}

	public static string RollGroup(string groupId)
	{
		return RollGroup(groupId, null);
	}

	public static string RollGroup(string groupId, Func<string, float> weightScale)
	{
		RebuildIfDirty();
		if (groupId == null || !groups.TryGetValue(groupId, out var value))
		{
			Debug.LogWarning("[LootRegistry] Table group '" + groupId + "' not found.");
			return null;
		}
		if (weightScale != null)
		{
			value = value.ConvertAll((LootEntry e) => e.Clone());
			foreach (LootEntry item in value)
			{
				item.weight *= Mathf.Max(0f, weightScale(item.id));
			}
		}
		string text = Pick(value);
		if (text == null)
		{
			WarnOnce("emptyroll:group:" + groupId, "Table group '" + groupId + "' has no entry with a positive weight.");
			return null;
		}
		return RollInternal(text, groupId);
	}

	private static string RollInternal(string tableId, string groupId)
	{
		RebuildIfDirty();
		if (tableId == null || !tables.TryGetValue(tableId, out var value))
		{
			Debug.LogWarning("[LootRegistry] Loot table '" + tableId + "' not found.");
			return null;
		}
		string text = Pick(value);
		if (text == null)
		{
			WarnOnce("emptyroll:table:" + tableId, "Loot table '" + tableId + "' has no entry with a positive weight.");
			return null;
		}
		LootRollContext obj = new LootRollContext
		{
			tableId = tableId,
			groupId = groupId,
			itemId = text
		};
		ModHook.FireOnLootRolled(obj);
		return obj.itemId;
	}

	private static string Pick(List<LootEntry> list)
	{
		double num = 0.0;
		foreach (LootEntry item in list)
		{
			if (item.weight > 0f)
			{
				num += (double)item.weight;
			}
		}
		if (num <= 0.0)
		{
			return null;
		}
		double num2 = (double)UnityEngine.Random.value * num;
		double num3 = 0.0;
		string result = null;
		foreach (LootEntry item2 in list)
		{
			if (!(item2.weight <= 0f))
			{
				num3 += (double)item2.weight;
				result = item2.id;
				if (num2 < num3)
				{
					return item2.id;
				}
			}
		}
		return result;
	}

	private static void Record(ListKind kind, OpType type, string modId, string listId, string entryId = null, float weight = 0f, List<LootEntry> entries = null)
	{
		string text = ((kind == ListKind.Table) ? "loot table" : "table group");
		if (string.IsNullOrEmpty(modId))
		{
			Debug.LogWarning("[LootRegistry] A mod edited " + text + " '" + listId + "' without passing its mod ID. Pass your manifest <ID> so the edit can be attributed.");
			modId = "unknown_mod";
		}
		if (string.IsNullOrEmpty(listId))
		{
			Debug.LogWarning("[LootRegistry] Mod '" + modId + "' passed an empty " + text + " ID; ignored.");
			return;
		}
		if ((type == OpType.Add || type == OpType.Remove || type == OpType.SetWeight) && string.IsNullOrEmpty(entryId))
		{
			Debug.LogWarning("[LootRegistry] Mod '" + modId + "' passed an empty entry ID for " + text + " '" + listId + "'; ignored.");
			return;
		}
		if (type == OpType.Add || type == OpType.SetWeight)
		{
			if (float.IsNaN(weight) || float.IsInfinity(weight))
			{
				Debug.LogWarning("[LootRegistry] Mod '" + modId + "' passed an invalid weight for '" + entryId + "' in " + text + " '" + listId + "'; ignored.");
				return;
			}
			if (type == OpType.Add && weight <= 0f)
			{
				Debug.LogWarning("[LootRegistry] " + $"Mod '{modId}' added '{entryId}' to {text} '{listId}' with weight {weight}; weights must be above zero. Ignored.");
				return;
			}
			if (weight < 0f)
			{
				Debug.LogWarning("[LootRegistry] Mod '" + modId + "' set a negative weight for '" + entryId + "' in " + text + " '" + listId + "'; clamped to 0.");
				weight = 0f;
			}
		}
		if (type == OpType.Register && (entries == null || entries.Count == 0))
		{
			Debug.LogWarning("[LootRegistry] Mod '" + modId + "' registered " + text + " '" + listId + "' with no valid entries; ignored.");
		}
		else
		{
			journal.Add(new Op
			{
				kind = kind,
				type = type,
				modId = modId,
				listId = listId,
				entryId = entryId,
				weight = weight,
				entries = entries
			});
			dirty = true;
		}
	}

	private static void RebuildIfDirty()
	{
		if (!dirty)
		{
			return;
		}
		dirty = false;
		CopyInto(vanillaTables, tables);
		CopyInto(vanillaGroups, groups);
		foreach (Op item in journal)
		{
			Apply(item);
		}
	}

	private static void Apply(Op op)
	{
		Dictionary<string, List<LootEntry>> dictionary = ((op.kind == ListKind.Table) ? tables : groups);
		string text = ((op.kind == ListKind.Table) ? "loot table" : "table group");
		if (op.type == OpType.Register)
		{
			if (dictionary.ContainsKey(op.listId))
			{
				WarnOnce($"register:{op.kind}:{op.listId}:{op.modId}", "Mod '" + op.modId + "' tried to register " + text + " '" + op.listId + "' but it already exists. To replace an existing one, clear it and add entries instead.");
			}
			else
			{
				dictionary[op.listId] = op.entries.ConvertAll((LootEntry e) => e.Clone());
			}
			return;
		}
		if (!dictionary.TryGetValue(op.listId, out var value))
		{
			if (vanillaEverLoaded)
			{
				WarnOnce($"missing:{op.kind}:{op.listId}:{op.modId}", "Mod '" + op.modId + "' edits " + text + " '" + op.listId + "', which does not exist. Register it first, or check the ID.");
			}
			return;
		}
		switch (op.type)
		{
		case OpType.Clear:
			value.Clear();
			break;
		case OpType.Add:
		{
			LootEntry lootEntry = value.Find((LootEntry e) => e.id == op.entryId);
			if (lootEntry != null)
			{
				lootEntry.weight = op.weight;
				if (lootEntry.sourceModId != op.modId)
				{
					lootEntry.modifiedByModId = op.modId;
				}
			}
			else
			{
				value.Add(new LootEntry(op.entryId, op.weight, op.modId));
			}
			break;
		}
		case OpType.Remove:
			if (value.RemoveAll((LootEntry e) => e.id == op.entryId) == 0 && vanillaEverLoaded)
			{
				WarnOnce($"remove:{op.kind}:{op.listId}:{op.entryId}:{op.modId}", "Mod '" + op.modId + "' removes '" + op.entryId + "' from " + text + " '" + op.listId + "', but it is not in there.");
			}
			break;
		case OpType.SetWeight:
		{
			LootEntry lootEntry = value.Find((LootEntry e) => e.id == op.entryId);
			if (lootEntry == null)
			{
				if (vanillaEverLoaded)
				{
					WarnOnce($"setweight:{op.kind}:{op.listId}:{op.entryId}:{op.modId}", "Mod '" + op.modId + "' sets the weight of '" + op.entryId + "' in " + text + " '" + op.listId + "', but it is not in there. Use Add to insert it.");
				}
			}
			else
			{
				lootEntry.weight = op.weight;
				if (lootEntry.sourceModId != op.modId)
				{
					lootEntry.modifiedByModId = op.modId;
				}
			}
			break;
		}
		}
	}

	private static List<LootEntry> ToEntries((string id, float weight)[] entries)
	{
		List<LootEntry> list = new List<LootEntry>();
		if (entries == null)
		{
			return list;
		}
		for (int i = 0; i < entries.Length; i++)
		{
			var (id, weight) = entries[i];
			list.Add(new LootEntry(id, weight));
		}
		return list;
	}

	private static List<LootEntry> CopyEntries(IEnumerable<LootEntry> entries, string modId, string listId)
	{
		List<LootEntry> list = new List<LootEntry>();
		if (entries == null)
		{
			return list;
		}
		foreach (LootEntry entry in entries)
		{
			if (entry == null || string.IsNullOrEmpty(entry.id))
			{
				Debug.LogWarning("[LootRegistry] Mod '" + modId + "' registered '" + listId + "' with an entry that has no ID; entry skipped.");
			}
			else if (float.IsNaN(entry.weight) || float.IsInfinity(entry.weight) || entry.weight <= 0f)
			{
				Debug.LogWarning("[LootRegistry] " + $"Mod '{modId}' registered '{listId}' with entry '{entry.id}' at weight {entry.weight}; weights must be above zero. Entry skipped.");
			}
			else
			{
				LootEntry lootEntry = list.Find((LootEntry e) => e.id == entry.id);
				if (lootEntry != null)
				{
					lootEntry.weight = entry.weight;
				}
				else
				{
					list.Add(new LootEntry(entry.id, entry.weight, modId));
				}
			}
		}
		return list;
	}

	private static void CopyInto(Dictionary<string, List<LootEntry>> source, Dictionary<string, List<LootEntry>> target)
	{
		target.Clear();
		foreach (KeyValuePair<string, List<LootEntry>> item in source)
		{
			target[item.Key] = item.Value.ConvertAll((LootEntry e) => e.Clone());
		}
	}

	private static List<LootEntry> Snapshot(Dictionary<string, List<LootEntry>> dict, string id)
	{
		if (id != null && dict.TryGetValue(id, out var value))
		{
			return value.ConvertAll((LootEntry e) => e.Clone());
		}
		return new List<LootEntry>();
	}

	private static void WarnOnce(string key, string message)
	{
		if (warned.Add(key))
		{
			Debug.LogWarning("[LootRegistry] " + message);
		}
	}
}

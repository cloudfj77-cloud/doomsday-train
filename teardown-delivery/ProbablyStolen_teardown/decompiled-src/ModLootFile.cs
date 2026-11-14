using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ModLootFile
{
	public const string FOLDER = "LootTables";

	public static void Load(LoadedMod mod)
	{
		string path = Path.Combine(mod.FolderPath, "LootTables");
		if (!System.IO.Directory.Exists(path))
		{
			return;
		}
		string[] files = System.IO.Directory.GetFiles(path, "*.json");
		Array.Sort(files, StringComparer.OrdinalIgnoreCase);
		string iD = mod.Manifest.ID;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		string[] array = files;
		foreach (string path2 in array)
		{
			string fileName = Path.GetFileName(path2);
			ModLootFileJson modLootFileJson;
			try
			{
				modLootFileJson = JsonUtility.FromJson<ModLootFileJson>(File.ReadAllText(path2));
			}
			catch (Exception ex)
			{
				Debug.LogError("[ModLoader] " + fileName + ": invalid JSON, file skipped (" + mod.Manifest.Name + "): " + ex.Message);
				continue;
			}
			if (modLootFileJson == null)
			{
				continue;
			}
			ModLootListJson[] array2 = modLootFileJson.tables ?? Array.Empty<ModLootListJson>();
			foreach (ModLootListJson modLootListJson in array2)
			{
				if (HasId(modLootListJson, fileName, mod, "table"))
				{
					LootRegistry.RegisterTable(iD, modLootListJson.id, ToEntries(modLootListJson.entries));
					num++;
				}
			}
			array2 = modLootFileJson.groups ?? Array.Empty<ModLootListJson>();
			foreach (ModLootListJson modLootListJson2 in array2)
			{
				if (HasId(modLootListJson2, fileName, mod, "group"))
				{
					LootRegistry.RegisterGroup(iD, modLootListJson2.id, ToEntries(modLootListJson2.entries));
					num2++;
				}
			}
			ModLootPatchJson[] array3 = modLootFileJson.patches ?? Array.Empty<ModLootPatchJson>();
			for (int j = 0; j < array3.Length; j++)
			{
				if (ApplyPatch(array3[j], iD, fileName, mod))
				{
					num3++;
				}
			}
		}
		if (files.Length != 0)
		{
			UnityEngine.Debug.Log($"[ModLoader] {mod.Manifest.Name}: {files.Length} loot file(s): {num} table(s), {num2} group(s), {num3} patch(es) loaded");
		}
	}

	private static bool HasId(ModLootListJson list, string fileName, LoadedMod mod, string kind)
	{
		if (list != null && !string.IsNullOrEmpty(list.id))
		{
			return true;
		}
		Debug.LogWarning("[ModLoader] " + fileName + ": a " + kind + " has no \"id\", skipped (" + mod.Manifest.Name + ")");
		return false;
	}

	private static bool ApplyPatch(ModLootPatchJson patch, string modId, string fileName, LoadedMod mod)
	{
		if (patch == null)
		{
			return false;
		}
		bool flag = !string.IsNullOrEmpty(patch.table);
		bool flag2 = !string.IsNullOrEmpty(patch.group);
		if (flag == flag2)
		{
			Debug.LogWarning("[ModLoader] " + fileName + ": a patch must name exactly one of \"table\" or \"group\", skipped (" + mod.Manifest.Name + ")");
			return false;
		}
		if (flag)
		{
			string table = patch.table;
			if (patch.clear)
			{
				LootRegistry.ClearTable(modId, table);
			}
			ModLootEntryJson[] array = patch.add ?? Array.Empty<ModLootEntryJson>();
			foreach (ModLootEntryJson modLootEntryJson in array)
			{
				if (modLootEntryJson != null)
				{
					LootRegistry.AddEntry(modId, table, modLootEntryJson.id, modLootEntryJson.weight);
				}
			}
			string[] array2 = patch.remove ?? Array.Empty<string>();
			foreach (string itemId in array2)
			{
				LootRegistry.RemoveEntry(modId, table, itemId);
			}
			array = patch.setWeight ?? Array.Empty<ModLootEntryJson>();
			foreach (ModLootEntryJson modLootEntryJson2 in array)
			{
				if (modLootEntryJson2 != null)
				{
					LootRegistry.SetWeight(modId, table, modLootEntryJson2.id, modLootEntryJson2.weight);
				}
			}
		}
		else
		{
			string group = patch.group;
			if (patch.clear)
			{
				LootRegistry.ClearGroup(modId, group);
			}
			ModLootEntryJson[] array = patch.add ?? Array.Empty<ModLootEntryJson>();
			foreach (ModLootEntryJson modLootEntryJson3 in array)
			{
				if (modLootEntryJson3 != null)
				{
					LootRegistry.AddGroupEntry(modId, group, modLootEntryJson3.id, modLootEntryJson3.weight);
				}
			}
			string[] array2 = patch.remove ?? Array.Empty<string>();
			foreach (string tableId in array2)
			{
				LootRegistry.RemoveGroupEntry(modId, group, tableId);
			}
			array = patch.setWeight ?? Array.Empty<ModLootEntryJson>();
			foreach (ModLootEntryJson modLootEntryJson4 in array)
			{
				if (modLootEntryJson4 != null)
				{
					LootRegistry.SetGroupWeight(modId, group, modLootEntryJson4.id, modLootEntryJson4.weight);
				}
			}
		}
		return true;
	}

	private static List<LootEntry> ToEntries(ModLootEntryJson[] entries)
	{
		List<LootEntry> list = new List<LootEntry>();
		ModLootEntryJson[] array = entries ?? Array.Empty<ModLootEntryJson>();
		foreach (ModLootEntryJson modLootEntryJson in array)
		{
			if (modLootEntryJson != null)
			{
				list.Add(new LootEntry(modLootEntryJson.id, modLootEntryJson.weight));
			}
		}
		return list;
	}
}

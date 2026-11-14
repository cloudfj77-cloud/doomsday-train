using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ModConfig
{
	[Serializable]
	private class ConfigFile
	{
		public List<ConfigFileEntry> entries = new List<ConfigFileEntry>();
	}

	[Serializable]
	private class ConfigFileEntry
	{
		public string key;

		public string value;
	}

	private static readonly Dictionary<string, ModConfig> configs = new Dictionary<string, ModConfig>();

	private readonly List<ConfigEntry> entries = new List<ConfigEntry>();

	private readonly Dictionary<string, string> rawValues = new Dictionary<string, string>();

	public static string RootFolder => Path.Combine(Application.persistentDataPath, "ModConfigs");

	public string ModId { get; }

	public string FilePath => Path.Combine(RootFolder, ModId + ".json");

	public IReadOnlyList<ConfigEntry> Entries => entries;

	public bool HasEntries => entries.Count > 0;

	public IEnumerable<string> Sections => entries.Select((ConfigEntry e) => e.Section).Distinct();

	public static ModConfig For(string modId)
	{
		if (string.IsNullOrEmpty(modId))
		{
			throw new ArgumentException("modId is required", "modId");
		}
		if (!ModLoader.IsValidModId(modId))
		{
			throw new ArgumentException("'" + modId + "' is not a valid mod id (letters, digits, '_', '-' and '.', not starting with '.')", "modId");
		}
		if (!configs.TryGetValue(modId, out var value))
		{
			value = new ModConfig(modId);
			value.Load();
			configs[modId] = value;
		}
		return value;
	}

	public static ModConfig Find(string modId)
	{
		if (modId == null || !configs.TryGetValue(modId, out var value))
		{
			return null;
		}
		return value;
	}

	public static bool HasSettings(string modId)
	{
		return Find(modId)?.HasEntries ?? false;
	}

	private ModConfig(string modId)
	{
		ModId = modId;
	}

	public ConfigEntry Get(string key)
	{
		return entries.Find((ConfigEntry e) => e.Key == key);
	}

	public ConfigEntry<bool> Define(string key, bool defaultValue, string section, Func<string> label, Func<string> tooltip = null, bool requiresRestart = false)
	{
		return Register(new ConfigEntry<bool>(defaultValue), key, section, label, tooltip, requiresRestart);
	}

	public ConfigEntry<int> Define(string key, int defaultValue, string section, Func<string> label, int? min = null, int? max = null, Func<string> tooltip = null, bool requiresRestart = false)
	{
		ConfigEntry<int> configEntry = new ConfigEntry<int>(defaultValue);
		if (min.HasValue && max.HasValue)
		{
			configEntry.SetRange(min.Value, max.Value);
		}
		return Register(configEntry, key, section, label, tooltip, requiresRestart);
	}

	public ConfigEntry<float> Define(string key, float defaultValue, string section, Func<string> label, float? min = null, float? max = null, Func<string> tooltip = null, bool requiresRestart = false)
	{
		ConfigEntry<float> configEntry = new ConfigEntry<float>(defaultValue);
		if (min.HasValue && max.HasValue)
		{
			configEntry.SetRange(min.Value, max.Value);
		}
		return Register(configEntry, key, section, label, tooltip, requiresRestart);
	}

	public ConfigEntry<string> Define(string key, string defaultValue, string section, Func<string> label, string[] choices = null, Func<string> tooltip = null, bool requiresRestart = false)
	{
		ConfigEntry<string> configEntry = new ConfigEntry<string>(defaultValue ?? "");
		if (choices != null && choices.Length != 0)
		{
			configEntry.Choices = choices;
		}
		return Register(configEntry, key, section, label, tooltip, requiresRestart);
	}

	public ConfigEntry<TEnum> DefineEnum<TEnum>(string key, TEnum defaultValue, string section, Func<string> label, Func<string> tooltip = null, bool requiresRestart = false) where TEnum : struct, Enum
	{
		ConfigEntry<TEnum> entry = new ConfigEntry<TEnum>(defaultValue)
		{
			Choices = Enum.GetNames(typeof(TEnum))
		};
		return Register(entry, key, section, label, tooltip, requiresRestart);
	}

	private T Register<T>(T entry, string key, string section, Func<string> label, Func<string> tooltip, bool requiresRestart) where T : ConfigEntry
	{
		if (string.IsNullOrEmpty(key))
		{
			Debug.LogError("[ModConfig] " + ModId + ": entry without a key ignored");
			return entry;
		}
		entry.Key = key;
		entry.Section = (string.IsNullOrEmpty(section) ? "General" : section);
		entry.Label = label ?? ((Func<string>)(() => key));
		entry.Tooltip = tooltip;
		entry.RequiresRestart = requiresRestart;
		entry.Config = this;
		int num = entries.FindIndex((ConfigEntry e) => e.Key == key);
		if (num >= 0)
		{
			Debug.LogWarning("[ModConfig] " + ModId + ": entry '" + key + "' defined twice, the later definition wins");
			entries[num] = entry;
		}
		else
		{
			entries.Add(entry);
		}
		if (rawValues.TryGetValue(key, out var value) && !entry.TryDeserialize(value))
		{
			Debug.LogWarning("[ModConfig] " + ModId + ": value '" + value + "' of '" + key + "' is not a " + entry.ValueType.Name + ", using the default");
		}
		return entry;
	}

	private void Load()
	{
		rawValues.Clear();
		try
		{
			if (!File.Exists(FilePath))
			{
				return;
			}
			ConfigFile configFile = JsonUtility.FromJson<ConfigFile>(File.ReadAllText(FilePath));
			if (configFile?.entries == null)
			{
				return;
			}
			foreach (ConfigFileEntry entry in configFile.entries)
			{
				if (!string.IsNullOrEmpty(entry.key))
				{
					rawValues[entry.key] = entry.value ?? "";
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[ModConfig] " + ModId + ": could not read " + FilePath + ": " + ex.Message);
		}
	}

	public void Save()
	{
		try
		{
			foreach (ConfigEntry entry in entries)
			{
				rawValues[entry.Key] = entry.Serialize();
			}
			ConfigFile configFile = new ConfigFile();
			foreach (ConfigEntry entry2 in entries)
			{
				configFile.entries.Add(new ConfigFileEntry
				{
					key = entry2.Key,
					value = rawValues[entry2.Key]
				});
			}
			foreach (KeyValuePair<string, string> rawValue in rawValues)
			{
				if (Get(rawValue.Key) == null)
				{
					configFile.entries.Add(new ConfigFileEntry
					{
						key = rawValue.Key,
						value = rawValue.Value
					});
				}
			}
			System.IO.Directory.CreateDirectory(RootFolder);
			File.WriteAllText(FilePath, JsonUtility.ToJson(configFile, prettyPrint: true));
		}
		catch (Exception ex)
		{
			Debug.LogError("[ModConfig] " + ModId + ": could not write " + FilePath + ": " + ex.Message);
		}
	}

	public void ResetToDefaults()
	{
		foreach (ConfigEntry entry in entries)
		{
			entry.ApplyBoxed(entry.BoxedDefault);
		}
		Save();
	}
}

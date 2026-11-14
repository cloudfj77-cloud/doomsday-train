using System;
using System.Globalization;
using UnityEngine;

public static class ModConfigWindow
{
	public const string WINDOW_ID_PREFIX = "mod_config:";

	private const float SLIDER_WIDTH = 224f;

	private const float FIELD_WIDTH = 245f;

	private const float BUTTON_WIDTH = 220f;

	private const float SECTION_GAP = 12f;

	private const int SEPARATOR_PADDING = 2;

	private const string TOOLTIP_LINK_PREFIX = "modcfg:";

	public static CustomUIWindow Open(LoadedMod mod)
	{
		if (mod == null)
		{
			return null;
		}
		ModConfig config = ModConfig.Find(mod.Manifest.ID);
		if (config == null || !config.HasEntries)
		{
			UnityEngine.Debug.Log("[ModConfig] '" + mod.Manifest.ID + "' has no configuration entries");
			return null;
		}
		CustomUIManager instance = CustomUIManager.Instance;
		if (instance == null)
		{
			Debug.LogError("[ModConfig] No CustomUIManager in the scene, cannot open the configuration panel");
			return null;
		}
		bool flag = StoreUIManager.Instance != null;
		string layerId = (flag ? "meta" : "menu");
		CustomUIWindow window = null;
		CustomUIBuilder customUIBuilder = instance.CreateWindow("mod_config:" + mod.Manifest.ID, mod.Manifest.Name, layerId, instance.optionUIPrefab).SetCloseOnEscape(!flag);
		customUIBuilder.BeginScrollFlexible();
		foreach (string section in config.Sections)
		{
			customUIBuilder.BeginRow(4f, TextAnchor.MiddleLeft);
			customUIBuilder.AddHeader(section, "section_" + section);
			customUIBuilder.End();
			customUIBuilder.AddSeparator(null, 2);
			foreach (ConfigEntry entry in config.Entries)
			{
				if (entry.Section == section)
				{
					BuildEntry(customUIBuilder, config, entry);
				}
			}
			customUIBuilder.AddSpace(12f);
		}
		customUIBuilder.End();
		customUIBuilder.AddSeparator(null, 2);
		customUIBuilder.BeginRow(12f, TextAnchor.MiddleCenter);
		customUIBuilder.AddFlexibleSpace();
		customUIBuilder.AddButton(LocHelper.GetLocalizedUI("ui_modcfg_reset"), delegate
		{
			ResetWidgets(window, config);
		}, "reset");
		customUIBuilder.Last.SetFixedWidth(220f);
		customUIBuilder.AddButton(LocHelper.GetLocalizedMainMenu("option_apply"), delegate
		{
			Apply(window, config);
		}, "apply");
		customUIBuilder.Last.SetFixedWidth(220f);
		customUIBuilder.AddFlexibleSpace();
		customUIBuilder.End();
		customUIBuilder.BeginRow(0f, TextAnchor.MiddleCenter);
		customUIBuilder.AddFlexibleSpace();
		customUIBuilder.AddWarning(LocHelper.GetLocalizedUI("ui_mods_restart_note"), "restart_note");
		customUIBuilder.AddFlexibleSpace();
		customUIBuilder.End();
		window = customUIBuilder.Show();
		window.Get("restart_note")?.SetVisible(visible: false);
		return window;
	}

	private static void BuildEntry(CustomUIBuilder b, ModConfig config, ConfigEntry entry)
	{
		string text = Safe(entry.Label, entry.Key);
		if (entry.Tooltip != null)
		{
			string text2 = "modcfg:" + config.ModId + ":" + entry.Key;
			TMPIconHover.CustomLinks[text2] = entry.Tooltip;
			text = text + " <link=\"" + text2 + "\"><sprite name=\"info\" tint=1></link>";
		}
		b.BeginRow(4f, TextAnchor.MiddleLeft);
		b.AddLabel(text, "label_" + entry.Key);
		b.AddFlexibleSpace();
		Type valueType = entry.ValueType;
		object boxedValue = entry.BoxedValue;
		if (valueType == typeof(bool))
		{
			b.AddToggle("", (bool)boxedValue, null, entry.Key);
		}
		else if (IsNumber(valueType) && entry.HasRange)
		{
			b.AddSlider(entry.Min, entry.Max, Convert.ToSingle(boxedValue, CultureInfo.InvariantCulture), null, valueType == typeof(int), entry.Key);
			b.Last.SetFixedWidth(224f);
		}
		else if (entry.Choices != null && entry.Choices.Length != 0)
		{
			b.AddDropdown(entry.Choices, Mathf.Max(0, Array.IndexOf(entry.Choices, ConfigEntry.Format(boxedValue))), null, entry.Key);
			b.Last.SetFixedWidth(245f);
		}
		else
		{
			b.AddInput("", ConfigEntry.Format(boxedValue), null, entry.Key);
			b.Last.SetFixedWidth(245f);
		}
		b.End();
	}

	private static bool IsNumber(Type type)
	{
		if (!(type == typeof(int)))
		{
			return type == typeof(float);
		}
		return true;
	}

	private static object ReadWidget(ConfigEntry entry, CustomUIElement element)
	{
		Type valueType = entry.ValueType;
		if (valueType == typeof(bool))
		{
			return element.GetBool();
		}
		if (IsNumber(valueType) && entry.HasRange)
		{
			float @float = element.GetFloat();
			if (!(valueType == typeof(int)))
			{
				return @float;
			}
			return Mathf.RoundToInt(@float);
		}
		if (entry.Choices != null && entry.Choices.Length != 0)
		{
			int index = element.GetIndex();
			if (index < 0 || index >= entry.Choices.Length)
			{
				return null;
			}
			if (!valueType.IsEnum)
			{
				return entry.Choices[index];
			}
			return Enum.Parse(valueType, entry.Choices[index]);
		}
		string @string = element.GetString();
		if (valueType == typeof(int))
		{
			if (!int.TryParse(@string, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
			{
				return null;
			}
			return result;
		}
		if (valueType == typeof(float))
		{
			if (!float.TryParse(@string, NumberStyles.Float, CultureInfo.InvariantCulture, out var result2))
			{
				return null;
			}
			return result2;
		}
		return @string;
	}

	private static void WriteWidget(ConfigEntry entry, CustomUIElement element, object value)
	{
		Type valueType = entry.ValueType;
		if (valueType == typeof(bool))
		{
			element.SetBool((bool)value);
		}
		else if (IsNumber(valueType) && entry.HasRange)
		{
			element.SetFloat(Convert.ToSingle(value, CultureInfo.InvariantCulture));
		}
		else if (entry.Choices != null && entry.Choices.Length != 0)
		{
			element.SetIndex(Mathf.Max(0, Array.IndexOf(entry.Choices, ConfigEntry.Format(value))));
		}
		else
		{
			element.SetString(ConfigEntry.Format(value));
		}
	}

	private static void Apply(CustomUIWindow window, ModConfig config)
	{
		if (window == null || !window.IsAlive)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		foreach (ConfigEntry entry in config.Entries)
		{
			CustomUIElement customUIElement = window.Get(entry.Key);
			if (customUIElement == null || !customUIElement.IsAlive)
			{
				continue;
			}
			object obj = ReadWidget(entry, customUIElement);
			if (obj == null)
			{
				WriteWidget(entry, customUIElement, entry.BoxedValue);
			}
			else if (entry.ApplyBoxed(obj))
			{
				flag = true;
				if (entry.RequiresRestart)
				{
					flag2 = true;
				}
				WriteWidget(entry, customUIElement, entry.BoxedValue);
			}
		}
		if (flag)
		{
			config.Save();
			ModHook.FireOnModConfigApplied(config.ModId);
		}
		if (flag2)
		{
			window.Get("restart_note")?.SetVisible(visible: true);
		}
	}

	private static void ResetWidgets(CustomUIWindow window, ModConfig config)
	{
		if (window == null || !window.IsAlive)
		{
			return;
		}
		foreach (ConfigEntry entry in config.Entries)
		{
			CustomUIElement customUIElement = window.Get(entry.Key);
			if (customUIElement != null && customUIElement.IsAlive)
			{
				WriteWidget(entry, customUIElement, entry.BoxedDefault);
			}
		}
	}

	private static T Safe<T>(Func<T> getter, T fallback)
	{
		if (getter == null)
		{
			return fallback;
		}
		try
		{
			return getter();
		}
		catch (Exception arg)
		{
			Debug.LogError($"[ModConfig] Label or tooltip callback threw: {arg}");
			return fallback;
		}
	}
}

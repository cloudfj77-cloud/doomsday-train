using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameOptionRegistry
{
	public enum Kind
	{
		Toggle,
		Slider,
		Dropdown,
		KeyBind
	}

	public interface IPendingValues
	{
		bool GetBool(string optionId);

		float GetFloat(string optionId);

		int GetIndex(string optionId);

		KeyCode GetKey(string optionId);
	}

	public class Entry
	{
		public string id;

		public string source;

		public Kind kind;

		public Func<string> label;

		public Func<string> tooltip;

		public Func<bool> getBool;

		public Action<bool> setBool;

		public Func<float> getFloat;

		public Action<float> setFloat;

		public float min;

		public float max = 1f;

		public bool wholeNumbers;

		public Func<float, string> valueText;

		public Func<int> getIndex;

		public Action<int> setIndex;

		public Func<IList<string>> options;

		public Func<KeyCode> getKey;

		public Action<KeyCode> setKey;

		public Func<IPendingValues, bool> interactable;

		public Func<IPendingValues, DropdownOverride> overrideOptions;
	}

	public class DropdownOverride
	{
		public IList<string> options;

		public int index;

		public Action<int> apply;
	}

	public class Section
	{
		public string id;

		public string source;

		public Func<string> label;

		public Func<string> note;

		public Func<IPendingValues, bool> noteVisible;

		public readonly List<Entry> entries = new List<Entry>();
	}

	public const string SECTION_GAMEPLAY = "gameplay";

	public const string SECTION_AUDIO = "audio";

	public const string SECTION_DISPLAY = "display";

	public const string SECTION_DIALOGUE = "dialogue";

	public const string SECTION_KEYBIND = "keybind";

	public const string OPTION_RESOLUTION = "resolution";

	public const string OPTION_FULLSCREEN = "fullscreen";

	public static readonly List<Section> Sections = new List<Section>();

	private static bool vanillaRegistered;

	private static bool monitorIs16by9 = true;

	private static EmporiumOption Opt => EmporiumOption.Instance;

	public static bool IsVanillaSection(string sectionId)
	{
		switch (sectionId)
		{
		default:
			return sectionId == "keybind";
		case "gameplay":
		case "audio":
		case "display":
		case "dialogue":
			return true;
		}
	}

	public static Section FindSection(string sectionId)
	{
		return Sections.Find((Section s) => s.id == sectionId);
	}

	public static Section GetOrAddSection(string sectionId, Func<string> label = null, string source = null)
	{
		Section section = FindSection(sectionId);
		if (section == null)
		{
			section = new Section
			{
				id = sectionId,
				source = source,
				label = (label ?? ((Func<string>)(() => sectionId)))
			};
			Sections.Add(section);
		}
		else if (label != null)
		{
			section.label = label;
		}
		return section;
	}

	public static Entry Find(string optionId)
	{
		foreach (Section section in Sections)
		{
			Entry entry = section.entries.Find((Entry e) => e.id == optionId);
			if (entry != null)
			{
				return entry;
			}
		}
		return null;
	}

	public static Entry Add(string sectionId, Entry entry)
	{
		if (entry == null || string.IsNullOrEmpty(entry.id))
		{
			Debug.LogError("[GameOption] Entry without an id ignored");
			return null;
		}
		Remove(entry.id);
		GetOrAddSection(sectionId).entries.Add(entry);
		return entry;
	}

	public static bool Remove(string optionId)
	{
		foreach (Section section in Sections)
		{
			int num = section.entries.FindIndex((Entry e) => e.id == optionId);
			if (num >= 0)
			{
				section.entries.RemoveAt(num);
				return true;
			}
		}
		return false;
	}

	public static int RemoveBySource(string source)
	{
		int num = 0;
		foreach (Section section in Sections)
		{
			num += section.entries.RemoveAll((Entry e) => e.source == source);
		}
		Sections.RemoveAll((Section s) => s.source == source && s.entries.Count == 0);
		return num;
	}

	public static Entry Toggle(string id, Func<string> label, Func<bool> get, Action<bool> set, Func<string> tooltip = null)
	{
		return new Entry
		{
			id = id,
			kind = Kind.Toggle,
			label = label,
			getBool = get,
			setBool = set,
			tooltip = tooltip
		};
	}

	public static Entry Slider(string id, Func<string> label, Func<float> get, Action<float> set, float min = 0f, float max = 1f, bool wholeNumbers = false, Func<string> tooltip = null, Func<float, string> valueText = null)
	{
		return new Entry
		{
			id = id,
			kind = Kind.Slider,
			label = label,
			getFloat = get,
			setFloat = set,
			min = min,
			max = max,
			wholeNumbers = wholeNumbers,
			tooltip = tooltip,
			valueText = valueText
		};
	}

	public static Entry Slider(string id, Func<string> label, Func<float> get, Action<float> set, float min, float max, bool wholeNumbers, Func<string> tooltip)
	{
		return Slider(id, label, get, set, min, max, wholeNumbers, tooltip, null);
	}

	public static Entry Dropdown(string id, Func<string> label, Func<IList<string>> options, Func<int> get, Action<int> set, Func<string> tooltip = null)
	{
		return new Entry
		{
			id = id,
			kind = Kind.Dropdown,
			label = label,
			options = options,
			getIndex = get,
			setIndex = set,
			tooltip = tooltip
		};
	}

	public static Entry KeyBind(string id, Func<string> label, Func<KeyCode> get, Action<KeyCode> set, Func<string> tooltip = null)
	{
		return new Entry
		{
			id = id,
			kind = Kind.KeyBind,
			label = label,
			getKey = get,
			setKey = set,
			tooltip = tooltip
		};
	}

	public static void RefreshMonitorInfo()
	{
		monitorIs16by9 = EmporiumOption.IsMonitor16by9();
	}

	public static void EnsureVanilla()
	{
		if (!vanillaRegistered)
		{
			vanillaRegistered = true;
			RegisterVanilla();
		}
	}

	private static Func<string> MainMenu(string key)
	{
		return () => LocHelper.GetLocalizedMainMenu(key);
	}

	private static Func<string> UI(string key)
	{
		return () => LocHelper.GetLocalizedUI(key);
	}

	private static bool FullscreenLetterboxed(IPendingValues pending)
	{
		if (pending.GetIndex("fullscreen") != 2)
		{
			return !monitorIs16by9;
		}
		return false;
	}

	private static bool ResolutionIgnored(IPendingValues pending)
	{
		if (monitorIs16by9)
		{
			return false;
		}
		return pending.GetIndex("fullscreen") switch
		{
			0 => true, 
			1 => EmporiumOption.GetBorderlessDivisors().Count == 1, 
			_ => false, 
		};
	}

	private static DropdownOverride ResolutionOverride(IPendingValues pending)
	{
		if (monitorIs16by9)
		{
			return null;
		}
		int index = pending.GetIndex("fullscreen");
		int systemWidth = Display.main.systemWidth;
		int systemHeight = Display.main.systemHeight;
		switch (index)
		{
		case 0:
			return new DropdownOverride
			{
				options = new List<string> { systemWidth + " x " + systemHeight }
			};
		case 1:
		{
			List<int> divisors = EmporiumOption.GetBorderlessDivisors();
			List<string> list = new List<string>(divisors.Count);
			foreach (int item in divisors)
			{
				list.Add(systemWidth / item + " x " + systemHeight / item);
			}
			return new DropdownOverride
			{
				options = list,
				index = Mathf.Max(0, divisors.IndexOf(Opt.borderlessDivisor)),
				apply = delegate(int i)
				{
					Opt.borderlessDivisor = divisors[Mathf.Clamp(i, 0, divisors.Count - 1)];
				}
			};
		}
		default:
			return null;
		}
	}

	private static IList<string> ResolutionOptions()
	{
		string[] array = new string[7] { "1920 x 1080", "960 x 540", "3840 x 2160", "1280 x 720", "1366 x 768", "1600 x 900", "2560 x 1440" };
		string localizedUI = LocHelper.GetLocalizedUI("option_graphic_recommended");
		List<string> list = new List<string>(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			list.Add((i <= 2) ? (array[i] + " (" + localizedUI + ")") : array[i]);
		}
		return list;
	}

	private static IList<string> FullscreenOptions()
	{
		return new List<string>
		{
			LocHelper.GetLocalizedUI("option_graphic_fullscreen"),
			LocHelper.GetLocalizedUI("option_graphic_borderless"),
			LocHelper.GetLocalizedUI("option_graphic_windowed")
		};
	}

	private static void RegisterVanilla()
	{
		Section orAddSection = GetOrAddSection("gameplay", MainMenu("option_game_setting"));
		orAddSection.entries.Add(Toggle("auto_call_next", MainMenu("option_auto_call_customer"), () => Opt.autoCallNext, delegate(bool v)
		{
			Opt.autoCallNext = v;
		}));
		orAddSection.entries.Add(Toggle("data_consent", MainMenu("option_data_consent"), () => Opt.consentAnalytics, delegate(bool v)
		{
			Opt.consentAnalytics = v;
		}));
		orAddSection.entries.Add(Toggle("fps_cap_background", MainMenu("option_fps_cap_background"), () => Opt.fpsCapWhileMinimized, delegate(bool v)
		{
			Opt.fpsCapWhileMinimized = v;
		}));
		orAddSection.entries.Add(Toggle("instant_dismiss", MainMenu("option_instant_customer"), () => Opt.instantDismissCustomer, delegate(bool v)
		{
			Opt.instantDismissCustomer = v;
		}));
		orAddSection.entries.Add(Toggle("instant_security", MainMenu("option_instant_security"), () => Opt.instantCallSecurity, delegate(bool v)
		{
			Opt.instantCallSecurity = v;
		}));
		orAddSection.entries.Add(Toggle("instant_call_next", MainMenu("option_instant_next_customer"), () => Opt.instantCallNextCustomer, delegate(bool v)
		{
			Opt.instantCallNextCustomer = v;
		}));
		orAddSection.entries.Add(Toggle("disable_sort_menu", MainMenu("option_disable_sort_menu"), () => Opt.disableSortMenu, delegate(bool v)
		{
			Opt.disableSortMenu = v;
		}));
		orAddSection.entries.Add(Toggle("sort_auto_rotate", MainMenu("option_sort_auto_rotate"), () => Opt.sortAutoRotate, delegate(bool v)
		{
			Opt.sortAutoRotate = v;
		}));
		orAddSection.entries.Add(Toggle("single_click_interact", MainMenu("option_single_click_interact"), () => Opt.singleClickInteract, delegate(bool v)
		{
			Opt.singleClickInteract = v;
		}, UI("ui_tooltip_single_click_interact_info")));
		orAddSection.entries.Add(Toggle("single_click_activate", MainMenu("option_single_click_activate"), () => Opt.singleClickActivate, delegate(bool v)
		{
			Opt.singleClickActivate = v;
		}, UI("ui_tooltip_single_click_activate_info")));
		orAddSection.entries.Add(Toggle("single_click_choice", () => LocHelper.GetLocalizedMainMenu("option_single_click_choice"), () => Opt.singleClickChoice, delegate(bool v)
		{
			Opt.singleClickChoice = v;
		}, () => LocHelper.GetLocalizedUI("ui_tooltip_single_click_choice_info")));
		orAddSection.entries.Add(Toggle("auto_open_negotiation", () => LocHelper.GetLocalizedMainMenu("option_auto_open_negotiation"), () => Opt.autoOpenNegotiationUI, delegate(bool v)
		{
			Opt.autoOpenNegotiationUI = v;
		}, () => LocHelper.GetLocalizedUI("ui_tooltip_auto_open_negotiation_info")));
		orAddSection.entries.Add(Toggle("move_bought_to_inventory", () => LocHelper.GetLocalizedOrDefault("MainMenu", "option_move_bought_to_inventory", "Move Bought Items to Inventory"), () => Opt.moveBoughtItemsToInventory, delegate(bool v)
		{
			Opt.moveBoughtItemsToInventory = v;
		}, () => LocHelper.GetLocalizedUIOrDefault("ui_tooltip_move_bought_to_inventory_info", "Items you buy or get in a barter go straight into your inventory. When off, they stay on the counter. Items that do not fit always stay on the counter.")));
		orAddSection.entries.Add(Toggle("newspaper_to_trash", () => LocHelper.GetLocalizedOrDefault("MainMenu", "option_newspaper_to_trash", "Send Newspaper to Trash Can"), () => Opt.newspaperToTrash, delegate(bool v)
		{
			Opt.newspaperToTrash = v;
		}, () => LocHelper.GetLocalizedUIOrDefault("ui_tooltip_newspaper_to_trash_info", "The morning newspaper is put in the trash can instead of on the counter. You can still take it out to read it.")));
		orAddSection.entries.Add(Toggle("disable_pop_culture_clients", () => LocHelper.GetLocalizedOrDefault("MainMenu", "option_disable_pop_culture_clients", "Disable Pop Culture Characters"), () => Opt.disablePopCultureClients, delegate(bool v)
		{
			Opt.disablePopCultureClients = v;
		}, () => LocHelper.GetLocalizedUIOrDefault("ui_tooltip_disable_pop_culture_clients_info", "Characters inspired by pop culture, such as the Case Enjoyer and the Foodie Tourist, no longer visit the store.")));
		orAddSection.entries.Add(Slider("save_history_days", () => LocHelper.GetLocalizedOrDefault("MainMenu", "option_save_history_days", "Saved days per run"), () => Opt.saveHistoryDays, delegate(float v)
		{
			Opt.saveHistoryDays = Mathf.RoundToInt(v);
		}, 1f, 10f, wholeNumbers: true, () => LocHelper.GetLocalizedUIOrDefault("ui_tooltip_save_history_days_info", "How many of the last days each run keeps on disk, the latest included. The earlier days can be loaded from the Load Game screen; the oldest is deleted when a new day is saved.")));
		orAddSection.entries.Add(Toggle("show_mod_source_tooltip", () => LocHelper.GetLocalizedMainMenu("option_show_mod_source_tooltip"), () => Opt.showModSourceInTooltip, delegate(bool v)
		{
			Opt.showModSourceInTooltip = v;
		}, () => LocHelper.GetLocalizedUI("ui_tooltip_show_mod_source_tooltip_info")));
		orAddSection.entries.Add(Toggle("show_mod_source_recorder", () => LocHelper.GetLocalizedMainMenu("option_show_mod_source_recorder"), () => Opt.showModSourceInRecorder, delegate(bool v)
		{
			Opt.showModSourceInRecorder = v;
		}, () => LocHelper.GetLocalizedUI("ui_tooltip_show_mod_source_recorder_info")));
		Section orAddSection2 = GetOrAddSection("audio", MainMenu("option_sounds"));
		orAddSection2.entries.Add(Slider("master_volume", MainMenu("option_master_volume"), () => Opt.masterVolume, delegate(float v)
		{
			Opt.masterVolume = v;
		}));
		orAddSection2.entries.Add(Slider("music_volume", MainMenu("music_volume"), () => Opt.musicVolume, delegate(float v)
		{
			Opt.musicVolume = v;
		}));
		orAddSection2.entries.Add(Slider("background_volume", () => LocHelper.GetLocalizedMainMenu("option_background_volume"), () => Opt.backgroundVolume, delegate(float v)
		{
			Opt.backgroundVolume = v;
		}));
		orAddSection2.entries.Add(Slider("dialogue_volume", MainMenu("option_dialogue_volume"), () => Opt.dialogVolume, delegate(float v)
		{
			Opt.dialogVolume = v;
		}));
		Section orAddSection3 = GetOrAddSection("display", MainMenu("option_graphic"));
		orAddSection3.note = MainMenu("option_note");
		orAddSection3.noteVisible = FullscreenLetterboxed;
		Entry entry = Dropdown("resolution", MainMenu("option_resolution"), ResolutionOptions, () => Opt.resolution, delegate(int v)
		{
			Opt.resolution = v;
		});
		entry.interactable = (IPendingValues pending) => !ResolutionIgnored(pending);
		entry.overrideOptions = ResolutionOverride;
		orAddSection3.entries.Add(entry);
		orAddSection3.entries.Add(Dropdown("fullscreen", MainMenu("option_fullscreen"), FullscreenOptions, () => Opt.fullscreenMode, delegate(int v)
		{
			Opt.fullscreenMode = v;
		}));
		orAddSection3.entries.Add(Toggle("enable_fps_cap", () => LocHelper.GetLocalizedOrDefault("MainMenu", "option_vsync", "VSync"), () => Opt.enableFpsCap, delegate(bool v)
		{
			Opt.enableFpsCap = v;
		}, () => LocHelper.GetLocalizedUIOrDefault("ui_tooltip_vsync_info", "Draws one frame per refresh of your monitor, so the picture never stutters or tears. Uses less power and keeps your graphics card cooler and quieter. When off, the game draws as many frames as it can.")));
		orAddSection3.entries.Add(Toggle("restore_display_after_focus", () => LocHelper.GetLocalizedOrDefault("MainMenu", "option_restore_display_after_focus", "Restore Display After Alt-Tab"), () => Opt.restoreDisplayAfterFocus, delegate(bool v)
		{
			Opt.restoreDisplayAfterFocus = v;
		}, () => LocHelper.GetLocalizedUIOrDefault("ui_tooltip_restore_display_after_focus_info", "When the game comes back from Alt-Tab as a small window instead of fullscreen, it switches back to the fullscreen mode you chose. Turn this off if the screen stays black after Alt-Tab, for example on Linux.")));
		Section orAddSection4 = GetOrAddSection("dialogue", MainMenu("option_dialogue"));
		orAddSection4.entries.Add(Slider("dialogue_speed", MainMenu("option_dialogue_speed"), () => Opt.dialogSpeed, delegate(float v)
		{
			Opt.dialogSpeed = v;
		}));
		orAddSection4.entries.Add(Toggle("typewriter_effect", MainMenu("option_dialogue_typewriter"), () => Opt.enableTypewriterEffect, delegate(bool v)
		{
			Opt.enableTypewriterEffect = v;
		}, UI("ui_tooltip_typewriter_effect_info")));
		orAddSection4.entries.Add(Toggle("auto_advance_dialogue", MainMenu("option_dialogue_auto_advance"), () => Opt.autoNextDialog, delegate(bool v)
		{
			Opt.autoNextDialog = v;
		}, UI("ui_tooltip_typewriter_auto_info")));
		orAddSection4.entries.Add(Slider("dialogue_bubble_scale", () => LocHelper.GetLocalizedOrDefault("MainMenu", "option_dialogue_bubble_scale", "Dialogue bubble size"), () => Mathf.Round(Opt.dialogBubbleScale / 0.1f), delegate(float v)
		{
			Opt.dialogBubbleScale = Mathf.Round(v) * 0.1f;
		}, Mathf.Round(5f), Mathf.Round(15f), wholeNumbers: true, () => LocHelper.GetLocalizedUIOrDefault("ui_tooltip_dialogue_bubble_scale_info", "Size of the speech bubbles, name tags and reply choices in the store and on the phone. 100% is the normal size."), (float v) => Mathf.RoundToInt(Mathf.Round(v) * 0.1f * 100f) + "%"));
		Section orAddSection5 = GetOrAddSection("keybind", MainMenu("option_keybind"));
		orAddSection5.note = () => GameOptionUIManager.KeyBindStatus;
		orAddSection5.noteVisible = (IPendingValues _) => !string.IsNullOrEmpty(GameOptionUIManager.KeyBindStatus);
		foreach (KeyBindHelper.Binding binding in KeyBindHelper.Bindings)
		{
			string bindingId = binding.id;
			orAddSection5.entries.Add(KeyBind(KeyBindOptionId(bindingId), binding.label, () => KeyBindHelper.Get(bindingId), delegate(KeyCode v)
			{
				KeyBindHelper.Set(bindingId, v);
			}));
		}
	}

	public static string KeyBindOptionId(string bindingId)
	{
		return "key_" + bindingId;
	}
}

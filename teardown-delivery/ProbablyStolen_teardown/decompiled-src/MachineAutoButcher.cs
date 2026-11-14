using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MachineAutoButcher
{
	private class PageSpec
	{
		public string captionKey;

		public string harvestTag;

		public string weightMinTag;

		public string weightMaxTag;

		public string ageMinTag;

		public string ageMaxTag;

		public string keepFemalesTag;

		public string keepMalesTag;

		public string countProtectedTag;

		public string[] toggleTags;

		public string[] toggleCaptionKeys;

		public bool[] toggleDefaults;

		public int weightMin;

		public int weightMax;

		public int weightStep;

		public int weightDefault;

		public int ageMin;

		public int ageMax;

		public int ageDefault;

		public int keepFemalesDefault = 2;

		public int keepMalesDefault = 1;
	}

	private class PageControls
	{
		public PageSpec spec;

		public Func<GameItem> machine;

		public MachineryStack root;

		public MachineryRadioList harvest;

		public MachineryRangeSlider weight;

		public MachineryText weightValue;

		public MachineryRangeSlider age;

		public MachineryText ageValue;

		public MachineryText keepFemalesLabel;

		public MachineryText keepMalesLabel;

		public MachineryRadioButton countProtected;

		public MachineryText countProtectedInfo;

		public MachinerySlider keepFemales;

		public MachineryText keepFemalesValue;

		public MachinerySlider keepMales;

		public MachineryText keepMalesValue;

		public MachineryRadioButton[] toggles;
	}

	private enum Outcome
	{
		Butchered,
		TrayFull,
		Skipped
	}

	private readonly struct Range
	{
		public readonly int low;

		public readonly int high;

		public readonly bool uncapped;

		public Range(int low, int high, bool uncapped)
		{
			this.low = low;
			this.high = high;
			this.uncapped = uncapped;
		}

		public bool Contains(float value)
		{
			if (value >= (float)low)
			{
				if (!uncapped)
				{
					return value <= (float)high;
				}
				return true;
			}
			return false;
		}
	}

	public const string AUTO_BUTCHER_TAG = "AUTO_BUTCHER_TAG";

	public const string IDENTIFIER = "auto_butcher";

	public const string ATLAS = "Items/items_tool2";

	public const string SPRITE = "gibber";

	public const int OUTPUT_WIDTH = 10;

	public const int OUTPUT_HEIGHT = 10;

	public const int KEEP_MAX = 10;

	public const int HARVEST_OFF = 0;

	public const int HARVEST_CARCASSES = 1;

	public const int HARVEST_WEIGHT = 2;

	public const int HARVEST_AGE = 3;

	private static readonly string[] HARVEST_CAPTION_KEYS = new string[4] { "mech_auto_butcher_harvest_off", "mech_auto_butcher_harvest_carcasses", "mech_auto_butcher_harvest_weight", "mech_auto_butcher_harvest_age" };

	private const int HARVEST_DEFAULT = 0;

	public const string CAGE_HARVEST_INT = "AUTO_BUTCHER_CAGE_HARVEST_INT";

	public const string CAGE_WEIGHT_MIN_INT = "AUTO_BUTCHER_CAGE_WEIGHT_MIN_INT";

	public const string CAGE_WEIGHT_MAX_INT = "AUTO_BUTCHER_CAGE_WEIGHT_MAX_INT";

	public const string CAGE_AGE_MIN_INT = "AUTO_BUTCHER_CAGE_AGE_MIN_INT";

	public const string CAGE_AGE_MAX_INT = "AUTO_BUTCHER_CAGE_AGE_MAX_INT";

	public const string CAGE_KEEP_FEMALES_INT = "AUTO_BUTCHER_CAGE_KEEP_FEMALES_INT";

	public const string CAGE_KEEP_MALES_INT = "AUTO_BUTCHER_CAGE_KEEP_MALES_INT";

	public const string CAGE_SPARE_PREGNANT_BOOL = "AUTO_BUTCHER_CAGE_SPARE_PREGNANT_BOOL";

	public const string CAGE_COUNT_PROTECTED_BOOL = "AUTO_BUTCHER_CAGE_COUNT_PROTECTED_BOOL";

	public const string CAGE_CULL_SICK_BOOL = "AUTO_BUTCHER_CAGE_CULL_SICK_BOOL";

	public const string CAGE_CULL_PRION_BOOL = "AUTO_BUTCHER_CAGE_CULL_PRION_BOOL";

	public const string TANK_HARVEST_INT = "AUTO_BUTCHER_TANK_HARVEST_INT";

	public const string TANK_WEIGHT_MIN_INT = "AUTO_BUTCHER_TANK_WEIGHT_MIN_INT";

	public const string TANK_WEIGHT_MAX_INT = "AUTO_BUTCHER_TANK_WEIGHT_MAX_INT";

	public const string TANK_AGE_MIN_INT = "AUTO_BUTCHER_TANK_AGE_MIN_INT";

	public const string TANK_AGE_MAX_INT = "AUTO_BUTCHER_TANK_AGE_MAX_INT";

	public const string TANK_KEEP_FEMALES_INT = "AUTO_BUTCHER_TANK_KEEP_FEMALES_INT";

	public const string TANK_KEEP_MALES_INT = "AUTO_BUTCHER_TANK_KEEP_MALES_INT";

	public const string TANK_SPARE_EGG_LAYERS_BOOL = "AUTO_BUTCHER_TANK_SPARE_EGG_LAYERS_BOOL";

	public const string TANK_COUNT_PROTECTED_BOOL = "AUTO_BUTCHER_TANK_COUNT_PROTECTED_BOOL";

	public const string TANK_FILLET_CARCASSES_BOOL = "AUTO_BUTCHER_TANK_FILLET_CARCASSES_BOOL";

	public const string TANK_CUTS_TO_DISPENSER_BOOL = "AUTO_BUTCHER_TANK_CUTS_TO_DISPENSER_BOOL";

	public const string TANK_COLLECT_ROE_BOOL = "AUTO_BUTCHER_TANK_COLLECT_ROE_BOOL";

	public const string LAST_COUNT_INT = "AUTO_BUTCHER_LAST_COUNT_INT";

	public const string LAST_ROE_INT = "AUTO_BUTCHER_LAST_ROE_INT";

	public const string DROP_TO_BUTCHER_BOOL = "AUTO_BUTCHER_DROP_TO_BUTCHER_BOOL";

	public const string KILL_STORE_ONLY_BOOL = "AUTO_BUTCHER_KILL_STORE_ONLY_BOOL";

	private const int SLIDER_WIDTH = 60;

	private const string COUNT_PROTECTED_LINK_ID = "auto_butcher_count_protected";

	private const string COUNT_PROTECTED_INFO_KEY = "mech_auto_butcher_info_count_protected";

	private const int CONTROL_BUTTON_WIDTH = 72;

	private static readonly PageSpec CagePage = new PageSpec
	{
		captionKey = "mech_auto_butcher_tab_cage",
		harvestTag = "AUTO_BUTCHER_CAGE_HARVEST_INT",
		weightMinTag = "AUTO_BUTCHER_CAGE_WEIGHT_MIN_INT",
		weightMaxTag = "AUTO_BUTCHER_CAGE_WEIGHT_MAX_INT",
		ageMinTag = "AUTO_BUTCHER_CAGE_AGE_MIN_INT",
		ageMaxTag = "AUTO_BUTCHER_CAGE_AGE_MAX_INT",
		countProtectedTag = "AUTO_BUTCHER_CAGE_COUNT_PROTECTED_BOOL",
		keepFemalesTag = "AUTO_BUTCHER_CAGE_KEEP_FEMALES_INT",
		keepMalesTag = "AUTO_BUTCHER_CAGE_KEEP_MALES_INT",
		toggleTags = new string[3] { "AUTO_BUTCHER_CAGE_SPARE_PREGNANT_BOOL", "AUTO_BUTCHER_CAGE_CULL_SICK_BOOL", "AUTO_BUTCHER_CAGE_CULL_PRION_BOOL" },
		toggleCaptionKeys = new string[3] { "mech_auto_butcher_toggle_spare_pregnant", "mech_auto_butcher_toggle_cull_sick", "mech_auto_butcher_toggle_cull_prion" },
		toggleDefaults = new bool[3] { true, true, true },
		weightMin = 100,
		weightMax = 1000,
		weightStep = 50,
		weightDefault = 400,
		ageMin = 1,
		ageMax = 30,
		ageDefault = 10
	};

	private static readonly PageSpec TankPage = new PageSpec
	{
		captionKey = "mech_auto_butcher_tab_tank",
		harvestTag = "AUTO_BUTCHER_TANK_HARVEST_INT",
		weightMinTag = "AUTO_BUTCHER_TANK_WEIGHT_MIN_INT",
		weightMaxTag = "AUTO_BUTCHER_TANK_WEIGHT_MAX_INT",
		ageMinTag = "AUTO_BUTCHER_TANK_AGE_MIN_INT",
		ageMaxTag = "AUTO_BUTCHER_TANK_AGE_MAX_INT",
		countProtectedTag = "AUTO_BUTCHER_TANK_COUNT_PROTECTED_BOOL",
		keepFemalesTag = "AUTO_BUTCHER_TANK_KEEP_FEMALES_INT",
		keepMalesTag = "AUTO_BUTCHER_TANK_KEEP_MALES_INT",
		toggleTags = new string[4] { "AUTO_BUTCHER_TANK_SPARE_EGG_LAYERS_BOOL", "AUTO_BUTCHER_TANK_FILLET_CARCASSES_BOOL", "AUTO_BUTCHER_TANK_CUTS_TO_DISPENSER_BOOL", "AUTO_BUTCHER_TANK_COLLECT_ROE_BOOL" },
		toggleCaptionKeys = new string[4] { "mech_auto_butcher_toggle_spare_egg_layers", "mech_auto_butcher_toggle_fillet_carcasses", "mech_auto_butcher_toggle_cuts_to_dispenser", "mech_auto_butcher_toggle_collect_roe" },
		toggleDefaults = new bool[4] { true, true, false, false },
		weightMin = 100,
		weightMax = 5000,
		weightStep = 100,
		weightDefault = 600,
		ageMin = 1,
		ageMax = 100,
		ageDefault = 20
	};

	private static readonly PageSpec[] Pages = new PageSpec[2] { CagePage, TankPage };

	private static Dictionary<string, int> clipboardInts;

	private static Dictionary<string, bool> clipboardBools;

	private const int TANK_CUTS_TO_DISPENSER_INDEX = 2;

	private static GameItem fitCheckItem;

	private static int HarvestModeCount => HARVEST_CAPTION_KEYS.Length;

	public static bool HasClipboard => clipboardInts != null;

	private static bool HarvestsLive(int harvest)
	{
		return harvest >= 2;
	}

	public static GameItem CreateAutoButcher()
	{
		GameItem machine = null;
		GameGridInventory gameGridInventory = new GameGridInventory(10, 10);
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		MachineryTabs tabs = new MachineryTabs();
		PageControls cageControls = BuildPage(CagePage, () => machine);
		tabs.AddTab(LocHelper.GetLocalizedMechanic(CagePage.captionKey), cageControls.root);
		PageControls tankControls = BuildPage(TankPage, () => machine);
		tabs.AddTab(LocHelper.GetLocalizedMechanic(TankPage.captionKey), tankControls.root);
		MachineryButton machineryButton = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_auto_butcher_btn_butcher_now"), null, 72).SetFillWidth(fill: false);
		MachineryButton machineryButton2 = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_auto_butcher_btn_copy"), null, 72).SetFillWidth(fill: false);
		MachineryButton pasteButton = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_auto_butcher_btn_paste"), null, 72).SetFillWidth(fill: false);
		MachineryButton machineryButton3 = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_auto_butcher_btn_butcher_content"), null, 72).SetFillWidth(fill: false);
		MachineryRadioButton dropToggle = new MachineryRadioButton(LocHelper.GetLocalizedMechanic("mech_auto_butcher_toggle_drop"), on: true, delegate(bool on)
		{
			machine?.ModifyTag("AUTO_BUTCHER_DROP_TO_BUTCHER_BOOL", delegate(TagState state)
			{
				state.Enable().SetBool(on);
			});
		});
		MachineryRadioButton storeToggle = null;
		storeToggle = new MachineryRadioButton(LocHelper.GetLocalizedMechanic("mech_auto_butcher_toggle_kill_store"), on: false, delegate(bool on)
		{
			machine?.ModifyTag("AUTO_BUTCHER_KILL_STORE_ONLY_BOOL", delegate(TagState state)
			{
				state.Enable().SetBool(on);
			});
			RefreshEnabled(cageControls);
			if (tankControls != null)
			{
				RefreshEnabled(tankControls);
			}
		});
		PixelWindow pixelWindow = MachineHelper.CreateMachineryWindow(true, MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_auto_butcher_label_settings"), tabs, "settings"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_auto_butcher_label_output"), MachineryStack.Vertical(new MachineryIntake(gameGridInventory), machineryButton, machineryButton3, machineryButton2, pasteButton, dropToggle, storeToggle), "output"));
		machine = ItemDirectory.CreateEmptyItem().SetContentWindow(pixelWindow).SetSpriteAndShape("Items/items_tool2", "gibber");
		machine.name = LocHelper.GetLocalizedItem("item_auto_butcher_name");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_auto_butcher_desc");
		machine.flavorText = LocHelper.GetLocalizedItem("item_auto_butcher_flavor");
		machine.unitValue = 400L;
		machine.EnableTag("AUTO_BUTCHER_TAG");
		machine.SetGameItemType("MACHINE");
		PageSpec[] pages = Pages;
		foreach (PageSpec spec in pages)
		{
			InitSettings(machine, spec);
		}
		machine.ModifyTag("AUTO_BUTCHER_LAST_COUNT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		machine.ModifyTag("AUTO_BUTCHER_LAST_ROE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		machine.ModifyTag("AUTO_BUTCHER_DROP_TO_BUTCHER_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(val: true);
		});
		machine.ModifyTag("AUTO_BUTCHER_KILL_STORE_ONLY_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(val: false);
		});
		AudioHelper.InitMetalMachine(machine);
		gameGridInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(machine, item) && IsTrayItem(item);
		bool pasteShown = true;
		machineryButton.SetCallback(delegate
		{
			Run(machine, manual: true);
			SyncAll();
		});
		machineryButton3.SetCallback(delegate
		{
			if (ButcherContent(machine))
			{
				AudioManager.Instance.Play3("blender");
			}
		});
		machineryButton2.SetCallback(delegate
		{
			CopySettings(machine);
			AudioManager.Instance.Play3("click_vintage");
			SyncAll();
		});
		pasteButton.SetCallback(delegate
		{
			if (PasteSettings(machine))
			{
				AudioManager.Instance.Play3("click_vintage");
				SyncAll();
			}
		});
		GameItem gameItem = machine;
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate
		{
			SyncAll();
		});
		machine.onOneSecondItemFunc = delegate
		{
			SyncStatus();
		};
		GameItem gameItem2 = machine;
		gameItem2.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem2.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			SelectHostTab();
		});
		pixelWindow.openCallback = (Action)Delegate.Combine(pixelWindow.openCallback, new Action(SelectHostTab));
		machine.onCycleEndLateSlotItemFunc = delegate(GameItem self, GameInventory _, SlotMarker _)
		{
			Run(self, manual: false);
		};
		SyncAll();
		return machine;
		void SelectHostTab()
		{
			GameItem host = GetHost(machine);
			if (host != null)
			{
				tabs.Select((!host.IsTag("CAGE_TAG")) ? Mathf.Min(1, tabs.count - 1) : 0);
			}
		}
		void SyncAll()
		{
			SyncPage(machine, cageControls);
			if (tankControls != null)
			{
				SyncPage(machine, tankControls);
			}
			dropToggle.SetOn(machine.GetTagReadonly("AUTO_BUTCHER_DROP_TO_BUTCHER_BOOL").GetBool());
			storeToggle.SetOn(IsKillAndStoreOnly(machine));
			RefreshEnabled(cageControls);
			if (tankControls != null)
			{
				RefreshEnabled(tankControls);
			}
			SyncStatus();
		}
		void SyncStatus()
		{
			bool hasClipboard = HasClipboard;
			if (hasClipboard != pasteShown)
			{
				pasteShown = hasClipboard;
				pasteButton.SetHidden(!hasClipboard);
			}
		}
	}

	private static bool IsTrayItem(GameItem item)
	{
		if (item == fitCheckItem)
		{
			return true;
		}
		if (item.IsTag("MEAT_TYPE_TAG") || FoodItemHelper.IsMeatFood(item))
		{
			return true;
		}
		if (item.IsTag("ANIMAL_ITEM_TAG"))
		{
			return HusbandryHelper.IsAnimalDead(item);
		}
		if (AquacultureHelper.IsCarcass(item))
		{
			return true;
		}
		if (item.identifier == "tilapia_skin_graft")
		{
			return true;
		}
		if (item.IsTag("FISH_ROE_TAG"))
		{
			return true;
		}
		return false;
	}

	private static void InitSettings(GameItem machine, PageSpec spec)
	{
		machine.ModifyTag(spec.harvestTag, delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		machine.ModifyTag(spec.weightMinTag, delegate(TagState state)
		{
			state.Enable().SetInt(spec.weightDefault);
		});
		machine.ModifyTag(spec.weightMaxTag, delegate(TagState state)
		{
			state.Enable().SetInt(spec.weightMax);
		});
		machine.ModifyTag(spec.ageMinTag, delegate(TagState state)
		{
			state.Enable().SetInt(spec.ageDefault);
		});
		machine.ModifyTag(spec.ageMaxTag, delegate(TagState state)
		{
			state.Enable().SetInt(spec.ageMax);
		});
		machine.ModifyTag(spec.keepFemalesTag, delegate(TagState state)
		{
			state.Enable().SetInt(spec.keepFemalesDefault);
		});
		machine.ModifyTag(spec.keepMalesTag, delegate(TagState state)
		{
			state.Enable().SetInt(spec.keepMalesDefault);
		});
		machine.ModifyTag(spec.countProtectedTag, delegate(TagState state)
		{
			state.Enable().SetBool(val: true);
		});
		for (int i = 0; i < spec.toggleTags.Length; i++)
		{
			bool on = spec.toggleDefaults[i];
			machine.ModifyTag(spec.toggleTags[i], delegate(TagState state)
			{
				state.Enable().SetBool(on);
			});
		}
	}

	private static PageControls BuildPage(PageSpec spec, Func<GameItem> machine)
	{
		PageControls page = new PageControls
		{
			spec = spec,
			machine = machine
		};
		page.harvest = new MachineryRadioList(HARVEST_CAPTION_KEYS.Select((string key) => LocHelper.GetLocalizedMechanic(key)).ToList());
		page.weightValue = new MachineryText(WeightCaption(spec.weightDefault, spec.weightMax, spec), 6, HorizontalAlignmentOptions.Left, uppercase: false);
		page.weight = new MachineryRangeSlider(spec.weightDefault, spec.weightMax).SetRange(spec.weightMin, spec.weightMax, spec.weightStep);
		page.ageValue = new MachineryText(AgeCaption(spec.ageDefault, spec.ageMax, spec), 6, HorizontalAlignmentOptions.Left, uppercase: false);
		page.age = new MachineryRangeSlider(spec.ageDefault, spec.ageMax).SetRange(spec.ageMin, spec.ageMax, 1f);
		page.keepFemalesValue = new MachineryText(spec.keepFemalesDefault.ToString(), 6, HorizontalAlignmentOptions.Left, uppercase: false).SetFillWidth(fill: false);
		page.keepFemales = new MachinerySlider(spec.keepFemalesDefault, 60).SetRange(0f, 10f, 1f).SetFillWidth(fill: false);
		page.keepMalesValue = new MachineryText(spec.keepMalesDefault.ToString(), 6, HorizontalAlignmentOptions.Left, uppercase: false).SetFillWidth(fill: false);
		page.keepMales = new MachinerySlider(spec.keepMalesDefault, 60).SetRange(0f, 10f, 1f).SetFillWidth(fill: false);
		page.countProtected = new MachineryRadioButton(LocHelper.GetLocalizedMechanic("mech_auto_butcher_toggle_count_protected"), on: true, delegate(bool on)
		{
			machine()?.ModifyTag(spec.countProtectedTag, delegate(TagState state)
			{
				state.Enable().SetBool(on);
			});
		});
		page.countProtectedInfo = new MachineryText("", 6, HorizontalAlignmentOptions.Left).SetFillWidth(fill: false).WithInfoLink("auto_butcher_count_protected", () => LocHelper.GetLocalizedMechanic("mech_auto_butcher_info_count_protected"));
		page.toggles = new MachineryRadioButton[spec.toggleTags.Length];
		for (int i = 0; i < spec.toggleTags.Length; i++)
		{
			string tag = spec.toggleTags[i];
			page.toggles[i] = new MachineryRadioButton(LocHelper.GetLocalizedMechanic(spec.toggleCaptionKeys[i]), spec.toggleDefaults[i], delegate(bool on)
			{
				machine()?.ModifyTag(tag, delegate(TagState state)
				{
					state.Enable().SetBool(on);
				});
			});
		}
		page.harvest.group.onChanged = delegate(int index)
		{
			machine()?.ModifyTag(spec.harvestTag, delegate(TagState state)
			{
				state.Enable().SetInt(index);
			});
			RefreshEnabled(page);
		};
		page.weight.onChanged = delegate(float lowValue, float highValue)
		{
			int lowGrams = Mathf.RoundToInt(lowValue);
			int highGrams = Mathf.RoundToInt(highValue);
			machine()?.ModifyTag(spec.weightMinTag, delegate(TagState state)
			{
				state.Enable().SetInt(lowGrams);
			});
			machine()?.ModifyTag(spec.weightMaxTag, delegate(TagState state)
			{
				state.Enable().SetInt(highGrams);
			});
			page.weightValue.SetText(WeightCaption(lowGrams, highGrams, spec));
		};
		page.age.onChanged = delegate(float lowValue, float highValue)
		{
			int lowDays = Mathf.RoundToInt(lowValue);
			int highDays = Mathf.RoundToInt(highValue);
			machine()?.ModifyTag(spec.ageMinTag, delegate(TagState state)
			{
				state.Enable().SetInt(lowDays);
			});
			machine()?.ModifyTag(spec.ageMaxTag, delegate(TagState state)
			{
				state.Enable().SetInt(highDays);
			});
			page.ageValue.SetText(AgeCaption(lowDays, highDays, spec));
		};
		page.keepFemales.onChanged = delegate(float value)
		{
			int count2 = Mathf.RoundToInt(value);
			machine()?.ModifyTag(spec.keepFemalesTag, delegate(TagState state)
			{
				state.Enable().SetInt(count2);
			});
			page.keepFemalesValue.SetText(count2.ToString());
		};
		page.keepMales.onChanged = delegate(float value)
		{
			int count = Mathf.RoundToInt(value);
			machine()?.ModifyTag(spec.keepMalesTag, delegate(TagState state)
			{
				state.Enable().SetInt(count);
			});
			page.keepMalesValue.SetText(count.ToString());
		};
		page.keepFemalesLabel = new MachineryText(LocHelper.GetLocalizedMechanic("mech_auto_butcher_keep_females"), 6, HorizontalAlignmentOptions.Left).SetFillWidth(fill: false);
		page.keepMalesLabel = new MachineryText(LocHelper.GetLocalizedMechanic("mech_auto_butcher_keep_males"), 6, HorizontalAlignmentOptions.Left).SetFillWidth(fill: false);
		MachineryStack machineryStack = MachineryStack.Vertical(new MachineryText(LocHelper.GetLocalizedMechanic("mech_auto_butcher_label_harvest"), 6, HorizontalAlignmentOptions.Left), page.harvest, page.weightValue, page.weight, page.ageValue, page.age, new MachineryText(LocHelper.GetLocalizedMechanic("mech_auto_butcher_label_keep"), 6, HorizontalAlignmentOptions.Left), MachineryStack.Horizontal(page.countProtected, page.countProtectedInfo), MachineryStack.Horizontal(page.keepFemalesLabel, page.keepFemales, page.keepFemalesValue), MachineryStack.Horizontal(page.keepMalesLabel, page.keepMales, page.keepMalesValue));
		MachineryRadioButton[] toggles = page.toggles;
		foreach (MachineryRadioButton element in toggles)
		{
			machineryStack.Add(element);
		}
		machineryStack.SetAlign(MachineryStack.Align.Start);
		page.root = machineryStack;
		RefreshEnabled(page);
		return page;
	}

	private static void RefreshEnabled(PageControls page)
	{
		int selected = page.harvest.selected;
		bool flag = HarvestsLive(selected);
		SetRangeRowEnabled(page.weight, page.weightValue, selected == 2);
		SetRangeRowEnabled(page.age, page.ageValue, selected == 3);
		SetRowEnabled(page.keepFemalesLabel, page.keepFemales, page.keepFemalesValue, flag);
		SetRowEnabled(page.keepMalesLabel, page.keepMales, page.keepMalesValue, flag);
		page.countProtected.SetDisabled(!flag);
		if (page.spec == TankPage)
		{
			GameItem gameItem = page.machine?.Invoke();
			bool disabled = gameItem != null && IsKillAndStoreOnly(gameItem);
			page.toggles[2].SetDisabled(disabled);
		}
	}

	private static string WeightCaption(int low, int high, PageSpec spec)
	{
		if (high < spec.weightMax)
		{
			return LocHelper.GetLocalizedMechanic("mech_auto_butcher_weight_range", low, high);
		}
		return LocHelper.GetLocalizedMechanic("mech_auto_butcher_weight_min", low);
	}

	private static string AgeCaption(int low, int high, PageSpec spec)
	{
		if (high < spec.ageMax)
		{
			return LocHelper.GetLocalizedMechanic("mech_auto_butcher_age_range", low, high);
		}
		return LocHelper.GetLocalizedMechanic("mech_auto_butcher_age_min", low);
	}

	private static void SetRangeRowEnabled(MachineryRangeSlider slider, MachineryText value, bool enabled)
	{
		slider.SetDisabled(!enabled);
		value.SetColor(enabled ? MachineryStyle.BODY_TEXT : MachineryStyle.BUTTON_TEXT_DISABLED);
	}

	private static void SetRowEnabled(MachineryText label, MachinerySlider slider, MachineryText value, bool enabled)
	{
		slider.SetDisabled(!enabled);
		Color color = (enabled ? MachineryStyle.BODY_TEXT : MachineryStyle.BUTTON_TEXT_DISABLED);
		label?.SetColor(color);
		value.SetColor(color);
	}

	private static void SyncPage(GameItem machine, PageControls page)
	{
		PageSpec spec = page.spec;
		int num = Mathf.Clamp(machine.GetTagReadonly(spec.harvestTag).GetInt(), 0, HarvestModeCount - 1);
		if (page.harvest.selected != num)
		{
			page.harvest.Select(num);
		}
		int @int = machine.GetTagReadonly(spec.weightMinTag).GetInt();
		int int2 = machine.GetTagReadonly(spec.weightMaxTag).GetInt();
		page.weight.SetInts(@int, int2);
		page.weightValue.SetText(WeightCaption(page.weight.GetLowInt(), page.weight.GetHighInt(), spec));
		int int3 = machine.GetTagReadonly(spec.ageMinTag).GetInt();
		int int4 = machine.GetTagReadonly(spec.ageMaxTag).GetInt();
		page.age.SetInts(int3, int4);
		page.ageValue.SetText(AgeCaption(page.age.GetLowInt(), page.age.GetHighInt(), spec));
		int int5 = machine.GetTagReadonly(spec.keepFemalesTag).GetInt();
		page.keepFemales.SetInt(int5);
		page.keepFemalesValue.SetText(int5.ToString());
		int int6 = machine.GetTagReadonly(spec.keepMalesTag).GetInt();
		page.keepMales.SetInt(int6);
		page.keepMalesValue.SetText(int6.ToString());
		for (int i = 0; i < spec.toggleTags.Length; i++)
		{
			page.toggles[i].SetOn(machine.GetTagReadonly(spec.toggleTags[i]).GetBool());
		}
		page.countProtected.SetOn(machine.GetTagReadonly(spec.countProtectedTag).GetBool());
		RefreshEnabled(page);
	}

	public static void CopySettings(GameItem machine)
	{
		clipboardInts = new Dictionary<string, int>();
		clipboardBools = new Dictionary<string, bool>();
		PageSpec[] pages = Pages;
		foreach (PageSpec pageSpec in pages)
		{
			clipboardInts[pageSpec.harvestTag] = machine.GetTagReadonly(pageSpec.harvestTag).GetInt();
			clipboardInts[pageSpec.weightMinTag] = machine.GetTagReadonly(pageSpec.weightMinTag).GetInt();
			clipboardInts[pageSpec.weightMaxTag] = machine.GetTagReadonly(pageSpec.weightMaxTag).GetInt();
			clipboardInts[pageSpec.ageMinTag] = machine.GetTagReadonly(pageSpec.ageMinTag).GetInt();
			clipboardInts[pageSpec.ageMaxTag] = machine.GetTagReadonly(pageSpec.ageMaxTag).GetInt();
			clipboardInts[pageSpec.keepFemalesTag] = machine.GetTagReadonly(pageSpec.keepFemalesTag).GetInt();
			clipboardInts[pageSpec.keepMalesTag] = machine.GetTagReadonly(pageSpec.keepMalesTag).GetInt();
			string[] toggleTags = pageSpec.toggleTags;
			foreach (string text in toggleTags)
			{
				clipboardBools[text] = machine.GetTagReadonly(text).GetBool();
			}
			clipboardBools[pageSpec.countProtectedTag] = machine.GetTagReadonly(pageSpec.countProtectedTag).GetBool();
		}
		clipboardBools["AUTO_BUTCHER_DROP_TO_BUTCHER_BOOL"] = machine.GetTagReadonly("AUTO_BUTCHER_DROP_TO_BUTCHER_BOOL").GetBool();
		clipboardBools["AUTO_BUTCHER_KILL_STORE_ONLY_BOOL"] = machine.GetTagReadonly("AUTO_BUTCHER_KILL_STORE_ONLY_BOOL").GetBool();
	}

	public static bool PasteSettings(GameItem machine)
	{
		if (!HasClipboard)
		{
			return false;
		}
		foreach (KeyValuePair<string, int> clipboardInt in clipboardInts)
		{
			int value = clipboardInt.Value;
			machine.ModifyTag(clipboardInt.Key, delegate(TagState state)
			{
				state.Enable().SetInt(value);
			});
		}
		foreach (KeyValuePair<string, bool> clipboardBool in clipboardBools)
		{
			bool value2 = clipboardBool.Value;
			machine.ModifyTag(clipboardBool.Key, delegate(TagState state)
			{
				state.Enable().SetBool(value2);
			});
		}
		return true;
	}

	public static GameItem GetHost(GameItem machine)
	{
		GameInventory gameInventory = machine?.parentInventory;
		GameItem gameItem = gameInventory?.GetParentItem();
		if (gameItem == null)
		{
			return null;
		}
		if (gameItem.IsTag("CAGE_TAG") && AnimalCage.GetButcherSlot(gameItem) == gameInventory)
		{
			return gameItem;
		}
		if (gameItem.IsTag("RAS_TAG") && MachineRAS.GetButcherSlot(gameItem) == gameInventory)
		{
			return gameItem;
		}
		return null;
	}

	public static GameGridInventory GetOutputGrid(GameItem machine)
	{
		return MachineHelper.GetBodyStorage(machine, 0) as GameGridInventory;
	}

	private static GameItem ButcherIn(GameSlotInventory slot)
	{
		GameItem gameItem = slot?.childItem;
		if (gameItem == null || !gameItem.IsTag("AUTO_BUTCHER_TAG"))
		{
			return null;
		}
		return gameItem;
	}

	public static void SetupHostSlot(GameItem host, GameSlotInventory butcherSlot, MachineryButton butcherButton)
	{
		butcherSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(host, item) && item.IsTag("AUTO_BUTCHER_TAG");
		butcherSlot.SetBackgroundFadeSprite("Items/items_tool2", "gibber");
		butcherButton.SetCallback(delegate
		{
			GameItem gameItem2 = ButcherIn(butcherSlot);
			if (gameItem2 != null)
			{
				Run(gameItem2, manual: true);
			}
		});
		butcherSlot.onSlotAddItemEndFunc = delegate
		{
			RefreshButton();
		};
		butcherSlot.onSlotRemoveItemEndFunc = delegate
		{
			RefreshButton();
		};
		GameItem gameItem = host;
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate
		{
			RefreshButton();
		});
		RefreshButton();
		void RefreshButton()
		{
			butcherButton.SetHidden(ButcherIn(butcherSlot) == null);
		}
	}

	public static void InitDropOnButcher(GameItem creature)
	{
		creature.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(creature.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && IsDropTarget(t)));
		creature.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(creature.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && IsDropTarget(t) && (!(s is GameItemElement gameItemElement) || !gameItemElement.IsDestroyed())));
		creature.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(creature.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (GeneralHelper.IsItemOwned(s, t) && IsDropTarget(t))
			{
				ButcherDropped(t, s);
			}
		});
	}

	private static bool IsDropTarget(GameItem target)
	{
		if (target != null && target.IsTag("AUTO_BUTCHER_TAG"))
		{
			return target.GetTagReadonly("AUTO_BUTCHER_DROP_TO_BUTCHER_BOOL").GetBool();
		}
		return false;
	}

	public static bool IsKillAndStoreOnly(GameItem machine)
	{
		return machine.GetTagReadonly("AUTO_BUTCHER_KILL_STORE_ONLY_BOOL").GetBool();
	}

	public static bool IsProtected(GameItem creature)
	{
		if (!AnimalMarkHelper.IsMarked(creature))
		{
			return GeneralHelper.HasCustomName(creature);
		}
		return true;
	}

	private static void ButcherDropped(GameItem machine, GameItem creature)
	{
		GameGridInventory outputGrid = GetOutputGrid(machine);
		if (outputGrid == null)
		{
			return;
		}
		bool num = creature.IsTag("ANIMAL_ITEM_TAG");
		string text = ((!num || HusbandryHelper.IsAnimalDead(creature)) ? null : HusbandryHelper.GetSpecies(creature)?.killSoundEffect);
		Outcome outcome = Outcome.Skipped;
		if (num)
		{
			outcome = ProcessAnimal(machine, creature, outputGrid);
		}
		else if (AquacultureHelper.IsFish(creature))
		{
			outcome = ProcessFish(machine, creature, outputGrid, null);
		}
		switch (outcome)
		{
		case Outcome.Butchered:
			if (text != null)
			{
				AudioManager.Instance.Play2(text);
			}
			AudioManager.Instance.Play3("blender");
			break;
		case Outcome.TrayFull:
			StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("mech_auto_butcher_notify_tray_full"));
			break;
		}
	}

	public static bool ButcherContent(GameItem machine)
	{
		GameGridInventory outputGrid = GetOutputGrid(machine);
		if (outputGrid == null)
		{
			return false;
		}
		bool result = false;
		foreach (GameItem item in outputGrid.childItems.ToList())
		{
			if (!IsProtected(item))
			{
				if (item.IsTag("ANIMAL_ITEM_TAG") && HusbandryHelper.IsAnimalDead(item))
				{
					HusbandryHelper.OnButcher(item);
					result = true;
				}
				else if (AquacultureHelper.IsCarcass(item))
				{
					AquacultureHelper.OnFillet(item);
					result = true;
				}
			}
		}
		return result;
	}

	public static void Run(GameItem machine, bool manual)
	{
		GameGridInventory outputGrid = GetOutputGrid(machine);
		if (outputGrid == null)
		{
			return;
		}
		GameItem host = GetHost(machine);
		int count = 0;
		int roe = 0;
		if (host == null)
		{
			GameInventory parentInventory = machine.parentInventory;
			if (parentInventory?.childItems == null || !GeneralHelper.IsItemOwned(machine))
			{
				return;
			}
			List<GameItem> items = parentInventory.childItems.Where((GameItem i) => i != machine && GeneralHelper.IsItemOwned(i)).ToList();
			RunCage(machine, items, outputGrid, ref count);
			RunTank(machine, items, null, outputGrid, ref count, ref roe);
		}
		else if (host.IsTag("CAGE_TAG"))
		{
			GameGridInventory cageInventory = AnimalCage.GetCageInventory(host);
			if (cageInventory != null)
			{
				RunCage(machine, cageInventory.childItems, outputGrid, ref count);
			}
		}
		else if (host.IsTag("RAS_TAG"))
		{
			GameGridInventory tankGrid = MachineRAS.GetTankGrid(host);
			if (tankGrid != null)
			{
				RunTank(machine, tankGrid.childItems, GetRasDispenser(machine, host), outputGrid, ref count, ref roe);
			}
		}
		machine.ModifyTag("AUTO_BUTCHER_LAST_COUNT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(count);
		});
		machine.ModifyTag("AUTO_BUTCHER_LAST_ROE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(roe);
		});
		if (manual && count > 0)
		{
			AudioManager.Instance.Play3("blender");
		}
	}

	private static void RunCage(GameItem machine, IEnumerable<GameItem> items, GameGridInventory tray, ref int count)
	{
		int harvest = machine.GetTagReadonly("AUTO_BUTCHER_CAGE_HARVEST_INT").GetInt();
		Range weight = ReadRange(machine, CagePage, forWeight: true);
		Range age = ReadRange(machine, CagePage, forWeight: false);
		int @int = machine.GetTagReadonly("AUTO_BUTCHER_CAGE_KEEP_FEMALES_INT").GetInt();
		int int2 = machine.GetTagReadonly("AUTO_BUTCHER_CAGE_KEEP_MALES_INT").GetInt();
		bool sparePregnant = machine.GetTagReadonly("AUTO_BUTCHER_CAGE_SPARE_PREGNANT_BOOL").GetBool();
		bool countProtected = machine.GetTagReadonly("AUTO_BUTCHER_CAGE_COUNT_PROTECTED_BOOL").GetBool();
		bool @bool = machine.GetTagReadonly("AUTO_BUTCHER_CAGE_CULL_SICK_BOOL").GetBool();
		bool bool2 = machine.GetTagReadonly("AUTO_BUTCHER_CAGE_CULL_PRION_BOOL").GetBool();
		if (harvest == 0)
		{
			return;
		}
		List<GameItem> source = items.Where((GameItem a) => a.IsTag("ANIMAL_ITEM_TAG")).ToList();
		List<GameItem> list = source.Where((GameItem a) => HusbandryHelper.IsAnimalDead(a) && !IsProtected(a)).ToList();
		List<GameItem> list2 = source.Where((GameItem a) => !HusbandryHelper.IsAnimalDead(a)).ToList();
		foreach (GameItem item in list2.ToList())
		{
			if (((@bool && item.IsTag("ANIMAL_DISEASED_TAG")) || (bool2 && item.IsTag("ANIMAL_PRION_TAG"))) && !IsProtected(item))
			{
				list.Add(item);
				list2.Remove(item);
			}
		}
		if (HarvestsLive(harvest))
		{
			foreach (IGrouping<string, GameItem> item2 in from a in list2
				group a by a.GetTagReadonly("ANIMAL_SPECIES_TAG").GetString())
			{
				List<GameItem> list3 = item2.Where((GameItem a) => IsCageCandidate(a, harvest, weight, age)).ToList();
				Func<GameItem, string> sex = (GameItem a) => a.GetTagReadonly("ANIMAL_SEXE_TAG").GetString();
				CountProtectedOutside(item2, list3, sex, countProtected, out var females, out var males);
				HashSet<GameItem> kept = PickKeepers(list3, @int - females, int2 - males, sex, (GameItem a) => a.GetTagReadonly("ANIMAL_AGE_TAG").GetInt(), (GameItem a) => !a.IsTag("ANIMAL_NEUTERED_TAG"), (GameItem a) => IsProtected(a) || (sparePregnant && a.IsTag("ANIMAL_PREGNANT_TAG")), (GameItem a) => countProtected || !IsProtected(a));
				list.AddRange(list3.Where((GameItem a) => !kept.Contains(a)));
			}
		}
		foreach (GameItem item3 in list)
		{
			if (ProcessAnimal(machine, item3, tray) == Outcome.Butchered)
			{
				count++;
			}
		}
	}

	private static bool IsCageCandidate(GameItem animal, int harvest, Range weight, Range age)
	{
		return harvest switch
		{
			2 => weight.Contains(animal.GetTagReadonly("ANIMAL_WEIGHT_TAG").GetFloat()), 
			3 => age.Contains(animal.GetTagReadonly("ANIMAL_AGE_TAG").GetInt()), 
			_ => false, 
		};
	}

	private static Range ReadRange(GameItem machine, PageSpec spec, bool forWeight)
	{
		int @int = machine.GetTagReadonly(forWeight ? spec.weightMinTag : spec.ageMinTag).GetInt();
		int int2 = machine.GetTagReadonly(forWeight ? spec.weightMaxTag : spec.ageMaxTag).GetInt();
		int num = (forWeight ? spec.weightMax : spec.ageMax);
		return new Range(@int, int2, int2 >= num);
	}

	private static void CountProtectedOutside(IEnumerable<GameItem> species, List<GameItem> candidates, Func<GameItem, string> sex, bool countProtected, out int females, out int males)
	{
		females = 0;
		males = 0;
		if (!countProtected)
		{
			return;
		}
		foreach (GameItem item in species)
		{
			if (!candidates.Contains(item) && IsProtected(item))
			{
				if (sex(item) == "FEMALE")
				{
					females++;
				}
				else if (sex(item) == "MALE")
				{
					males++;
				}
			}
		}
	}

	private static HashSet<GameItem> PickKeepers(List<GameItem> candidates, int keepFemales, int keepMales, Func<GameItem, string> sex, Func<GameItem, int> age, Func<GameItem, bool> canBreed, Func<GameItem, bool> protectedOne, Func<GameItem, bool> countsTowardQuota)
	{
		HashSet<GameItem> kept = new HashSet<GameItem>(candidates.Where(protectedOne));
		int num = kept.Count((GameItem a) => sex(a) == "FEMALE" && countsTowardQuota(a));
		foreach (GameItem item in candidates.Where((GameItem a) => sex(a) == "FEMALE" && canBreed(a) && !kept.Contains(a)).OrderBy(age))
		{
			if (num >= keepFemales)
			{
				break;
			}
			kept.Add(item);
			num++;
		}
		int num2 = kept.Count((GameItem a) => sex(a) == "MALE" && countsTowardQuota(a));
		foreach (GameItem item2 in candidates.Where((GameItem a) => sex(a) == "MALE" && canBreed(a) && !kept.Contains(a)).OrderBy(age))
		{
			if (num2 >= keepMales)
			{
				break;
			}
			kept.Add(item2);
			num2++;
		}
		return kept;
	}

	private static Outcome ProcessAnimal(GameItem machine, GameItem animal, GameGridInventory tray)
	{
		if (!IsKillAndStoreOnly(machine))
		{
			return TryButcherAnimal(animal, tray);
		}
		return TryStoreAnimal(animal, tray);
	}

	private static Outcome TryStoreAnimal(GameItem animal, GameGridInventory tray)
	{
		if (animal is GameItemElement gameItemElement && gameItemElement.IsDestroyed())
		{
			return Outcome.Skipped;
		}
		if (!FitsTray(tray, animal))
		{
			return Outcome.TrayFull;
		}
		if (!HusbandryHelper.IsAnimalDead(animal))
		{
			HusbandryHelper.OnAnimalDead(animal, playSound: false);
		}
		if (GraphUtils.CanAccept(tray, animal) && GraphUtils.TryAcceptAll(tray, animal) == 0)
		{
			return Outcome.Butchered;
		}
		return TryButcherAnimal(animal, tray);
	}

	private static bool FitsTray(GameGridInventory tray, GameItem creature)
	{
		fitCheckItem = creature;
		try
		{
			return GraphUtils.CanAccept(tray, creature);
		}
		finally
		{
			fitCheckItem = null;
		}
	}

	private static Outcome TryButcherAnimal(GameItem animal, GameGridInventory tray)
	{
		if (animal is GameItemElement gameItemElement && gameItemElement.IsDestroyed())
		{
			return Outcome.Skipped;
		}
		List<GameItem> list = HusbandryHelper.CreateButcherCuts(animal);
		if (list == null)
		{
			return Outcome.Skipped;
		}
		if (!PlaceAll(tray, list))
		{
			DiscardAll(list);
			return Outcome.TrayFull;
		}
		HusbandryHelper.CompleteButcher(animal);
		return Outcome.Butchered;
	}

	private static GameItem GetRasDispenser(GameItem machine, GameItem ras)
	{
		if (!machine.GetTagReadonly("AUTO_BUTCHER_TANK_CUTS_TO_DISPENSER_BOOL").GetBool())
		{
			return null;
		}
		GameItem gameItem = MachineRAS.GetDispenserSlot(ras)?.childItem;
		if (gameItem == null || !gameItem.IsTag("FEED_DISPENSER_TAG"))
		{
			return null;
		}
		return gameItem;
	}

	private static void RunTank(GameItem machine, IEnumerable<GameItem> items, GameItem dispenser, GameGridInventory tray, ref int count, ref int roe)
	{
		items = items.ToList();
		if (machine.GetTagReadonly("AUTO_BUTCHER_TANK_COLLECT_ROE_BOOL").GetBool())
		{
			foreach (GameItem item in items.Where((GameItem c) => c.IsTag("FISH_ROE_TAG") && !c.IsTag("FISH_FERTILIZED_TAG")).ToList())
			{
				if (!GraphUtils.CanAccept(tray, item) || GraphUtils.TryAcceptAll(tray, item) > 0)
				{
					break;
				}
				roe++;
			}
		}
		int harvest = machine.GetTagReadonly("AUTO_BUTCHER_TANK_HARVEST_INT").GetInt();
		Range weight = ReadRange(machine, TankPage, forWeight: true);
		Range age = ReadRange(machine, TankPage, forWeight: false);
		int @int = machine.GetTagReadonly("AUTO_BUTCHER_TANK_KEEP_FEMALES_INT").GetInt();
		int int2 = machine.GetTagReadonly("AUTO_BUTCHER_TANK_KEEP_MALES_INT").GetInt();
		bool spareEggLayers = machine.GetTagReadonly("AUTO_BUTCHER_TANK_SPARE_EGG_LAYERS_BOOL").GetBool();
		bool countProtected = machine.GetTagReadonly("AUTO_BUTCHER_TANK_COUNT_PROTECTED_BOOL").GetBool();
		bool @bool = machine.GetTagReadonly("AUTO_BUTCHER_TANK_FILLET_CARCASSES_BOOL").GetBool();
		if (harvest == 0)
		{
			return;
		}
		List<GameItem> source = items.Where(AquacultureHelper.IsFish).ToList();
		List<GameItem> list = (@bool ? source.Where((GameItem f) => AquacultureHelper.IsDead(f) && !IsProtected(f)).ToList() : new List<GameItem>());
		if (HarvestsLive(harvest))
		{
			foreach (IGrouping<string, GameItem> item2 in from f in source.Where((GameItem f) => !AquacultureHelper.IsDead(f)).ToList()
				group f by f.GetTagReadonly("FISH_SPECIES_TAG").GetString())
			{
				List<GameItem> list2 = item2.Where((GameItem f) => IsTankCandidate(f, harvest, weight, age)).ToList();
				Func<GameItem, string> sex = (GameItem f) => f.GetTagReadonly("FISH_SEX_TAG").GetString();
				CountProtectedOutside(item2, list2, sex, countProtected, out var females, out var males);
				HashSet<GameItem> kept = PickKeepers(list2, @int - females, int2 - males, sex, (GameItem f) => f.GetTagReadonly("FISH_AGE_TAG").GetInt(), (GameItem _) => true, (GameItem f) => IsProtected(f) || (spareEggLayers && AquacultureHelper.GetEggTargetGrams(f) > 0), (GameItem f) => countProtected || !IsProtected(f));
				list.AddRange(list2.Where((GameItem f) => !kept.Contains(f)));
			}
		}
		foreach (GameItem item3 in list)
		{
			if (ProcessFish(machine, item3, tray, dispenser) == Outcome.Butchered)
			{
				count++;
			}
		}
	}

	private static bool IsTankCandidate(GameItem fish, int harvest, Range weight, Range age)
	{
		return harvest switch
		{
			2 => weight.Contains(AquacultureHelper.GetWeightGrams(fish)), 
			3 => age.Contains(fish.GetTagReadonly("FISH_AGE_TAG").GetInt()), 
			_ => false, 
		};
	}

	private static Outcome ProcessFish(GameItem machine, GameItem fish, GameGridInventory tray, GameItem dispenser)
	{
		if (!IsKillAndStoreOnly(machine))
		{
			return TryButcherFish(fish, tray, dispenser);
		}
		return TryStoreFish(fish, tray);
	}

	private static Outcome TryStoreFish(GameItem fish, GameGridInventory tray)
	{
		if (fish is GameItemElement gameItemElement && gameItemElement.IsDestroyed())
		{
			return Outcome.Skipped;
		}
		if (!FitsTray(tray, fish))
		{
			return Outcome.TrayFull;
		}
		if (!AquacultureHelper.IsDead(fish))
		{
			AquacultureHelper.OnFishDead(fish);
		}
		if (GraphUtils.CanAccept(tray, fish) && GraphUtils.TryAcceptAll(tray, fish) == 0)
		{
			return Outcome.Butchered;
		}
		return TryButcherFish(fish, tray, null);
	}

	private static Outcome TryButcherFish(GameItem fish, GameGridInventory tray, GameItem dispenser)
	{
		if (fish is GameItemElement gameItemElement && gameItemElement.IsDestroyed())
		{
			return Outcome.Skipped;
		}
		List<GameItem> list = AquacultureHelper.CreateFilletPieces(fish);
		if (list == null)
		{
			return Outcome.Skipped;
		}
		List<GameItem> list2 = new List<GameItem>();
		List<GameItem> list3 = new List<GameItem>();
		int num = ((dispenser != null) ? (40000 - dispenser.GetTagReadonly("STORED_KCAL_TAG").GetInt()) : 0);
		foreach (GameItem item in list)
		{
			int num2 = (IsFishMeat(item) ? item.GetTagReadonly("CALORIE_VALUE_TAG").GetInt() : (-1));
			if (dispenser != null && num2 >= 0 && num2 <= num)
			{
				list2.Add(item);
				num -= num2;
			}
			else
			{
				list3.Add(item);
			}
		}
		if (!PlaceAll(tray, list3))
		{
			DiscardAll(list);
			return Outcome.TrayFull;
		}
		fish.Destroy();
		foreach (GameItem item2 in list2)
		{
			if (!MachineFeedDispenser.TryBlendItem(dispenser, item2))
			{
				GraphUtils.EmporiumTryAdd(tray, item2);
			}
		}
		return Outcome.Butchered;
	}

	private static bool IsFishMeat(GameItem piece)
	{
		if (!piece.IsTag("CALORIE_VALUE_TAG"))
		{
			return false;
		}
		if (piece.identifier != "caviar")
		{
			return piece.identifier != "sturgeon_roe";
		}
		return false;
	}

	private static bool PlaceAll(GameGridInventory tray, List<GameItem> pieces)
	{
		foreach (GameItem piece in pieces)
		{
			if (piece != null)
			{
				if (!GraphUtils.CanAccept(tray, piece))
				{
					return false;
				}
				if (GraphUtils.TryAcceptAll(tray, piece) > 0 || piece.parentInventory == null)
				{
					return false;
				}
			}
		}
		return true;
	}

	private static void DiscardAll(List<GameItem> pieces)
	{
		foreach (GameItem piece in pieces)
		{
			if (piece != null)
			{
				if (piece.parentInventory != null)
				{
					piece.Destroy();
				}
				else
				{
					GraphUtils.DestroyIfUnplaced(piece);
				}
			}
		}
	}

	private static string GetLastRunText(GameItem machine)
	{
		int @int = machine.GetTagReadonly("AUTO_BUTCHER_LAST_COUNT_INT").GetInt();
		int int2 = machine.GetTagReadonly("AUTO_BUTCHER_LAST_ROE_INT").GetInt();
		if (int2 <= 0)
		{
			return LocHelper.GetLocalizedMechanic("mech_auto_butcher_tooltip_last_run", @int);
		}
		return LocHelper.GetLocalizedMechanic("mech_auto_butcher_tooltip_last_run_roe", @int, int2);
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("AUTO_BUTCHER_TAG"))
		{
			builder.AddLine(" ");
			builder.AddLine(GetLastRunText(item), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		}
	}
}

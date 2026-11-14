using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class CustomUIDebugPanel
{
	private class PanelDef
	{
		public string id;

		public string title;

		public string[] categories;

		public bool searchable;

		public bool developerOnly;

		public string search = "";

		public readonly List<(Entry entry, CustomUIElement element)> buttons = new List<(Entry, CustomUIElement)>();

		public CustomUIElement matchLabel;
	}

	private class Entry
	{
		public string category;

		public string name;

		public Action action;

		public bool tall;

		public string source;

		public string info;
	}

	public const string WINDOW_ID = "debug_panel";

	public const string MODDING_WINDOW_ID = "debug_panel_modding";

	public const string MARKETING_WINDOW_ID = "debug_panel_marketing";

	public const string STORY_WINDOW_ID = "debug_panel_story";

	public const string CATEGORY_STORE = "Store";

	public const string CATEGORY_COMMAND = "Command";

	public const string CATEGORY_SPAWN = "Spawn";

	public const string CATEGORY_SPECIAL = "Special";

	public const string CATEGORY_MODDING = "Modding";

	public const string CATEGORY_MARKETING_SCENE = "Scene";

	public const string CATEGORY_MARKETING_KITS = "Kits";

	public const string CATEGORY_MARKETING_ITEMS = "Items";

	public const string CATEGORY_MARKETING_CUSTOMERS = "Customers";

	public const string CATEGORY_MARKETING = "Marketing";

	public const string CATEGORY_STORY_BUTCHER = "Butcher";

	public const string CATEGORY_STORY_LOU = "Lou";

	public const string CATEGORY_STORY_ELEANOR = "Eleanor";

	public const string CATEGORY_STORY_SERPICO = "Serpico";

	public const string CATEGORY_STORY_DAYS = "Story Days";

	private static readonly string[] CATEGORIES = new string[14]
	{
		"Store", "Command", "Spawn", "Special", "Modding", "Scene", "Kits", "Items", "Customers", "Butcher",
		"Lou", "Eleanor", "Serpico", "Story Days"
	};

	private static readonly PanelDef MAIN_PANEL = new PanelDef
	{
		id = "debug_panel",
		title = "Debug Panel",
		categories = new string[4] { "Store", "Command", "Spawn", "Special" },
		searchable = true
	};

	private static readonly PanelDef MODDING_PANEL = new PanelDef
	{
		id = "debug_panel_modding",
		title = "Modding Debug Panel",
		categories = new string[1] { "Modding" }
	};

	private static readonly PanelDef MARKETING_PANEL = new PanelDef
	{
		id = "debug_panel_marketing",
		title = "Marketing Debug Panel",
		categories = new string[4] { "Scene", "Kits", "Items", "Customers" },
		searchable = true,
		developerOnly = true
	};

	private static readonly PanelDef STORY_PANEL = new PanelDef
	{
		id = "debug_panel_story",
		title = "Story Debug Panel",
		categories = new string[4] { "Butcher", "Lou", "Eleanor", "Serpico" },
		searchable = true,
		developerOnly = true
	};

	private static readonly PanelDef[] PANELS = new PanelDef[4] { MAIN_PANEL, MODDING_PANEL, MARKETING_PANEL, STORY_PANEL };

	private const float COLUMN_WIDTH = 190f;

	private const float COLUMN_SPACING = 4f;

	private const float BUTTON_HEIGHT = 30f;

	private const float TALL_BUTTON_HEIGHT = 60f;

	private const float BUTTON_SPACING = 2f;

	private const float MIN_FONT_SIZE = 9f;

	private const float SCROLL_HEIGHT = 480f;

	private const float BACKGROUND_ALPHA = 0.8f;

	private const int MARGIN = 2;

	private const int SIDE_MARGIN = 4;

	private const float BUTTON_WIDTH = 190f;

	private const float HEADER_HEIGHT = 32f;

	private const float CATEGORY_LABEL_HEIGHT = 20f;

	private const float LAYOUT_SPACING = 4f;

	private const float SEARCH_HEIGHT = 32f;

	private const float MATCH_LABEL_WIDTH = 90f;

	private static readonly Color HIGHLIGHT_COLOR = new Color(1f, 0.82f, 0.3f);

	private static readonly Color BUTTON_COLOR = Color.white;

	private static readonly List<Entry> entries = new List<Entry>();

	private static bool registered;

	private static bool registering;

	private const string INFO_TOGGLE_TOOLTIP = "For clean screenshots and footage: hides the tooltips and hover outlines that would otherwise pop up over the shot.";

	private const string INFO_TOGGLE_IDLE_ANIMATION = "For still shots: makes the customers stand still instead of swaying, so a screenshot catches them in a steady pose.";

	private const string INFO_TOGGLE_CUSTOMER_SPRITE_SWAP = "For showing off the cast: the customer at the counter keeps changing into other characters from the game, so one clip shows many faces.";

	private const string INFO_TOGGLE_VERBOSE_LOG = "Shows the game's Debug.Log output (console, Player.log and GameLog files). Off every session by default; warnings and errors always show, and so do mods' own log lines.";

	private const string INFO_BUYOUT_ENDING = "Buys the store with cash at today's price (credits are topped up first if short) and ends the run with the buyout ending, to test the game over screen and the run summary. Skips the buyout achievements.";

	private const string INFO_BUY_STORE_DEED = "Buys the store with cash at today's price (credits are topped up first if short), puts the deed on the counter and queues what follows the deed (Joe Wild's visit, in the editor), without the ending screen. Ends a running mortgage. Skips the buyout achievements.";

	private const string INFO_ARREST_ENDING = "Ends the run with the arrest ending, to test the game over screen and the run summary.";

	private static readonly string[] INFO_BADGE_ART = new string[8] { "..####..", ".######.", "###..###", "########", "###..###", "###..###", ".##..##.", "..####.." };

	private const float INFO_BADGE_SIZE = 16f;

	private const int INFO_BADGE_GAP = 4;

	private static Sprite infoBadgeSprite;

	private static float WindowWidth(PanelDef panel)
	{
		return (float)panel.categories.Length * 190f + (float)(panel.categories.Length - 1) * 4f + 16f;
	}

	private static float WindowHeight(PanelDef panel)
	{
		float num = 552f;
		if (panel.searchable)
		{
			num += 36f;
		}
		return num;
	}

	public static void Add(string category, string name, Action action, bool tall = false, string source = null, string info = null)
	{
		if (category == "Marketing")
		{
			category = (DevTools.IsFullEnabled ? "Scene" : "Special");
		}
		if (Array.IndexOf(CATEGORIES, category) < 0)
		{
			Debug.LogWarning("[CustomUI] Debug panel: unknown category '" + category + "' for '" + name + "', using Special");
			category = "Special";
		}
		entries.Add(new Entry
		{
			category = category,
			name = name,
			action = action,
			tall = tall,
			source = source,
			info = info
		});
		if (!registering)
		{
			RebuildIfOpen();
		}
	}

	public static int Remove(string source)
	{
		if (string.IsNullOrEmpty(source))
		{
			return 0;
		}
		int num = entries.RemoveAll((Entry e) => e.source == source);
		if (num > 0)
		{
			RebuildIfOpen();
		}
		return num;
	}

	private static void RebuildIfOpen()
	{
		if (CustomUIManager.Instance == null)
		{
			return;
		}
		PanelDef[] pANELS = PANELS;
		foreach (PanelDef panelDef in pANELS)
		{
			CustomUIWindow window = CustomUIManager.Instance.GetWindow(panelDef.id);
			if (window != null && window.IsAlive)
			{
				bool isVisible = window.IsVisible;
				Open(panelDef);
				if (!isVisible)
				{
					CustomUIManager.Instance.GetWindow(panelDef.id)?.Hide();
				}
			}
		}
	}

	private static void RegisterFunctions()
	{
		if (registered)
		{
			return;
		}
		registered = true;
		registering = true;
		Add("Store", "Set Credit to 0", DebugPanel.SetCreditToZero);
		Add("Store", "Add 500 Credit", delegate
		{
			PlayerStore.Instance.ModCash(500);
		});
		Add("Store", "Add 10000 Credit", DebugPanel.Add10000Credits);
		Add("Store", "Set All Rep to Max", DebugPanel.SetRepToMax);
		Add("Store", "Set All Rep to Min", DebugPanel.SetRepToMin);
		Add("Store", "Set Security Rep to Min", SetSecurityRepToMin);
		Add("Store", "Buy Store + Deed (Keep Playing)", DebugBuyStoreWithDeed, tall: false, null, "Buys the store with cash at today's price (credits are topped up first if short), puts the deed on the counter and queues what follows the deed (Joe Wild's visit, in the editor), without the ending screen. Ends a running mortgage. Skips the buyout achievements.");
		Add("Store", "Ending: Instant Buyout (Win)", DebugBuyoutEnding, tall: false, null, "Buys the store with cash at today's price (credits are topped up first if short) and ends the run with the buyout ending, to test the game over screen and the run summary. Skips the buyout achievements.");
		Add("Store", "Ending: Arrested (Loss)", DebugArrestEnding, tall: false, null, "Ends the run with the arrest ending, to test the game over screen and the run summary.");
		Add("Command", "Toggle Dev Mode", delegate
		{
			PlayerStore.Instance.devMode = !PlayerStore.Instance.devMode;
			Debug.Log("Dev Mode: " + PlayerStore.Instance.devMode);
		}, tall: true);
		Add("Command", "Toggle Verbose Log", VerboseLog.Toggle, tall: false, null, "Shows the game's Debug.Log output (console, Player.log and GameLog files). Off every session by default; warnings and errors always show, and so do mods' own log lines.");
		Add("Command", "Skip Night (Machines Only)", DebugPanel.SkipNightMachine, tall: true);
		Add("Command", "Unlock Shotgun", delegate
		{
			Shotgun.Unlock();
			UnityEngine.Debug.Log("[Shotgun] Unlocked: " + PlayerStore.Instance.isShootingUnlocked);
		});
		Add("Command", "Force Clear Inv", DebugPanel.ClearMainInv);
		Add("Command", "Force Clear Trashcan", DebugPanel.ForceClearTrashcan);
		Add("Command", "Open Wild UI", DebugPanel.OpenWildUI);
		Add("Command", "Queue Basic Gun Order (LL)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListGun.CreateLLGunBuyer());
		});
		Add("Command", "Queue Mid Gun Order (Merc)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListGun.CreateMercGunBuyer());
		});
		Add("Command", "Queue High Gun Order (UL)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListGun.CreateULBuyer());
		});
		Add("Spawn", "Open Debug Spawner", OpenDebugSpawner, tall: true);
		Add("Spawn", "Spawn Aquaculture Kit", DebugPanel.SpawnAquacultureKit);
		Add("Spawn", "Spawn Hydroponic Kit", DebugPanel.SpawnHydroponicKit);
		Add("Spawn", "Spawn Plant DNA Kit", DebugPanel.SpawnPlantDnaKit);
		Add("Spawn", "Spawn Gun Kit", DebugPanel.SpawnGunKit);
		Add("Spawn", "Spawn Rat Ranching Kit", DebugPanel.SpawnRatKit);
		Add("Special", "Open Sprite Viewer", CustomUIDebugWindows.OpenSpriteDictViewer);
		Add("Special", "Open Customer Viewer", CustomUIDebugWindows.OpenCustomerDictViewer, tall: true);
		Add("Special", "Afterhour Events", CustomUIAfterhourEventsPanel.Toggle);
		Add("Special", "Modding Panel", OpenModdingPanel, tall: true);
		if (DevTools.IsFullEnabled)
		{
			Add("Special", "Legacy Panel", ToggleLegacyPanel);
			Add("Special", "Marketing Panel", OpenMarketingPanel, tall: true);
			Add("Special", "Story Panel", OpenStoryPanel, tall: true);
		}
		Add("Modding", "Back to Debug Panel", delegate
		{
			BackToMain(MODDING_PANEL);
		});
		Add("Modding", "Paper Editor", PaperEditor.ToggleWindow, tall: true);
		Add("Modding", "Reload Mod Books", delegate
		{
			int num3 = PaperBookRegistry.ReloadAll();
			UnityEngine.Debug.Log($"[PaperUI] Reload: {num3} book(s) changed");
		});
		Add("Modding", "Close Paper Books", PaperBookRegistry.CloseAll);
		Add("Modding", "Open Mods Folder", delegate
		{
			if (!System.IO.Directory.Exists(ModLoader.ModsPath))
			{
				System.IO.Directory.CreateDirectory(ModLoader.ModsPath);
			}
			Application.OpenURL("file:///" + Path.GetFullPath(ModLoader.ModsPath).Replace('\\', '/'));
		});
		Add("Modding", "Sprite Viewer", CustomUIDebugWindows.OpenSpriteDictViewer);
		Add("Modding", "Paper Sprite Picker", delegate
		{
			PaperEditorSpritePicker.Open(null, "Vanilla paper sprites (click copies the key)", delegate(string key)
			{
				GUIUtility.systemCopyBuffer = key;
				UnityEngine.Debug.Log("[PaperUI] Copied sprite key '" + key + "'");
			});
		});
		if (!DevTools.IsFullEnabled)
		{
			registering = false;
			return;
		}
		Add("Butcher", "Back to Debug Panel", delegate
		{
			BackToMain(STORY_PANEL);
		}, tall: true);
		Add("Butcher", "Toggle Butcher Test Mode", delegate
		{
			StoreClientData.butcherTestMode = !StoreClientData.butcherTestMode;
			UnityEngine.Debug.Log("[Butcher] Test mode: " + (StoreClientData.butcherTestMode ? "ON, every step comes the next day" : "OFF"));
		});
		Add("Butcher", "Queue The Butcher", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListWanted.CreateWanted2());
		});
		Add("Lou", "Toggle Lou Test Mode", delegate
		{
			StoreClientData.louTestMode = !StoreClientData.louTestMode;
			UnityEngine.Debug.Log("[Lou] Test mode: " + (StoreClientData.louTestMode ? "ON, every step comes the next day" : "OFF"));
		});
		Add("Lou", "Plant Bomb", delegate
		{
			LouBombHelper.PlantBomb();
			LouBombHelper.QueueVisit(PlayerStore.Instance.storeClientManager);
		});
		Add("Lou", "Drop Box Of Toys", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListWanted.CreateLouBoxDrop());
		});
		Add("Lou", "Collect Box Of Toys", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListWanted.CreateLouBoxCollect());
		});
		Add("Lou", "Disguise Visit", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListWanted.CreateLouDisguise());
		});
		Add("Lou", "Collect Demand", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListWanted.CreateLouDemandCollect());
		});
		Add("Lou", "First Visit", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListWanted.CreateWanted5());
		});
		Add("Lou", "Morning Visit (Boo)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListWanted.CreateWanted5Morning());
		});
		Add("Lou", "Operative: Intro (Report)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(LouOperativeHelper.CreateOperativeIntro(isShotPath: false));
		});
		Add("Lou", "Operative: Intro (Shot)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(LouOperativeHelper.CreateOperativeIntro(isShotPath: true));
		});
		Add("Lou", "Operative: Reward", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(LouOperativeHelper.CreateOperativeReward());
		});
		Add("Lou", "Operative: Trap Failed", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(LouOperativeHelper.CreateOperativeTrapFailed());
		});
		Add("Lou", "Operative: Questioning", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(LouOperativeHelper.CreateOperativeQuestioning());
		});
		Add("Lou", "Spawn Transmitter", delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("republic_transmitter", isOwend: true);
		});
		Add("Lou", "Spawn Etched Round", delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("lou_etched_round", isOwend: true);
		});
		Add("Lou", "Give Locker + Letter", LouOperativeHelper.DebugGiveLocker);
		Add("Lou", "Give Thank-You + Kit", LouOperativeHelper.DebugGiveThankYou);
		Add("Lou", "Mark Shot Today", delegate
		{
			LouOperativeHelper.OnLouShot();
			UnityEngine.Debug.Log("[Lou] " + LouOperativeHelper.DebugDescribe());
		});
		Add("Lou", "Mark Captured Today", delegate
		{
			PlayerStore.Instance.storeClientData.louCapturedDay = StoreStation.Instance.dayCounter;
			PlayerStore.Instance.CloseWantedPoster("wanted5");
			UnityEngine.Debug.Log("[Lou] " + LouOperativeHelper.DebugDescribe());
		});
		Add("Lou", "Log State", delegate
		{
			UnityEngine.Debug.Log("[Lou] " + LouOperativeHelper.DebugDescribe());
		});
		Add("Lou", "Reset Story", delegate
		{
			LouOperativeHelper.DebugResetStory();
			UnityEngine.Debug.Log("[Lou] Story reset: never met, no operative, no shot; her items removed");
		});
		Add("Eleanor", "1 Intro", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.EleanorIntro());
		});
		Add("Eleanor", "2", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Eleanor2());
		});
		Add("Eleanor", "3", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Eleanor3());
		});
		Add("Eleanor", "Uncle (Crate)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.CreateEleanorUncle());
		});
		Add("Eleanor", "Warned (Uncle Reported)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.EleanorWarned());
		});
		Add("Eleanor", "4", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Eleanor4());
		});
		Add("Eleanor", "5", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Eleanor5());
		});
		Add("Eleanor", "Bail Officer", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.CreateEleanorBailOfficer());
		});
		Add("Eleanor", "Freed (Thank You)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.EleanorFreed());
		});
		Add("Eleanor", "6 (Plan)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Eleanor6());
		});
		Add("Eleanor", "Scavenging Visit", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.EleanorScav());
		});
		Add("Eleanor", "7 (Data Drive)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Eleanor7());
		});
		Add("Eleanor", "Thug's Threat", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.CreateEleanorThreat());
		});
		Add("Eleanor", "8 (Choice)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Eleanor8());
		});
		Add("Eleanor", "9 (Conclusion)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Eleanor9());
		});
		Add("Eleanor", "Free Nutrifruit Now", delegate
		{
			PlantList.FreeNutrifruit();
		});
		Add("Eleanor", "Savings +200", delegate
		{
			StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
			bool num2 = storeClientData.eleanorSavings < 800;
			storeClientData.eleanorSavings += 200;
			if (num2 && storeClientData.eleanorSavings >= 800)
			{
				storeClientData.eleanorSavingsDoneDay = StoreStation.Instance.dayCounter;
			}
			UnityEngine.Debug.Log("[Eleanor] Savings: " + PlayerStore.Instance.storeClientData.eleanorSavings + " / " + 800);
		});
		Add("Eleanor", "Spawn Crate", delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("luxury_supply_crate"), isOwend: true);
		});
		Add("Eleanor", "Reset Story", delegate
		{
			PlayerStore.Instance.storeClientData.eleanorArrested = false;
			PlayerStore.Instance.storeClientData.eleanorArrestDay = -1;
			PlayerStore.Instance.storeClientData.eleanorArrestVisit = 0;
			PlayerStore.Instance.storeClientData.eleanorBailChoice = 0;
			PlayerStore.Instance.storeClientData.eleanorBailDay = -1;
			PlayerStore.Instance.storeClientData.eleanorFreedVisitDay = -1;
			PlayerStore.Instance.storeClientData.eleanorHelpedTimes = 0;
			PlayerStore.Instance.storeClientData.eleanorSavings = 0;
			PlayerStore.Instance.storeClientData.eleanorPlanDay = -1;
			PlayerStore.Instance.storeClientData.eleanorScavLastDay = -1;
			PlayerStore.Instance.storeClientData.eleanorUncleReported = false;
			PlayerStore.Instance.storeClientData.eleanorWarnedDay = -1;
			PlayerStore.Instance.storeClientData.eleanorSavingsDoneDay = -1;
			PlayerStore.Instance.storeClientData.eleanorDriveVisitDay = -1;
			PlayerStore.Instance.storeClientData.eleanorThreatVisitDay = -1;
			PlayerStore.Instance.storeClientData.eleanorDriveChoice = 0;
			PlayerStore.Instance.storeClientData.eleanorChoiceDay = -1;
			PlayerStore.Instance.storeClientData.eleanorConclusionDay = -1;
			UnityEngine.Debug.Log("[Eleanor] Story reset: not arrested, never helped, no savings, plan not started");
		});
		Add("Story Days", "Log Anchors", delegate
		{
			UnityEngine.Debug.Log("[StorylineSchedule] " + StorylineSchedule.Describe());
		});
		string[] array = new string[5] { "georgio", "lun", "eleanor", "nellis", "serpico" };
		foreach (string text in array)
		{
			string id = text;
			Add("Story Days", "Re-anchor " + id, delegate
			{
				int num = StorylineSchedule.DebugReanchor(id, StoreStation.Instance.dayCounter);
				UnityEngine.Debug.Log("[StorylineSchedule] " + id + " re-anchored on day " + num + " (today " + StoreStation.Instance.dayCounter + ")");
			});
		}
		Add("Serpico", "Toggle Serpico Test Mode", delegate
		{
			UnityEngine.Debug.Log("[Serpico] " + SerpicoStoryline.TestMode(SerpicoStoryline.IsTestMode ? "off" : "on"));
		});
		Add("Serpico", "1 (Cadet)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Serpico1());
		});
		Add("Serpico", "3", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Serpico3());
		});
		Add("Serpico", "4", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Serpico4());
		});
		Add("Serpico", "5 (Officer Warning)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Serpico5());
		});
		Add("Serpico", "6 (Flees)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Serpico6());
		});
		Add("Serpico", "Georgio's Letter", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(SerpicoStoryline.DebugGeorgioLetter());
		});
		Add("Serpico", "Feed Him (2000 kcal)", delegate
		{
			SerpicoStoryline.DebugFeed();
			UnityEngine.Debug.Log("[Serpico] Fed: " + 2000 + " kcal");
		});
		Add("Serpico", "Officer Back (Bounty)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(SerpicoStoryline.DebugOfficerReturn());
		});
		Add("Serpico", "7 (Document)", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientUniqueList.Serpico7());
		});
		Add("Serpico", "Ending: Revolution", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(SerpicoStoryline.DebugConclusion(1));
		});
		Add("Serpico", "Ending: Black Market", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(SerpicoStoryline.DebugConclusion(2));
		});
		Add("Serpico", "Ending: Security", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(SerpicoStoryline.DebugConclusion(3));
		});
		Add("Serpico", "Article: Found Dead", SerpicoStoryline.DebugDeathNews);
		Add("Serpico", "Article: Revolution", SerpicoStoryline.DebugRevolutionNews);
		Add("Serpico", "Article: Security", SerpicoStoryline.DebugSecurityNews);
		Add("Serpico", "Reset Story", delegate
		{
			SerpicoStoryline.ResetProgress();
			UnityEngine.Debug.Log("[Serpico] Story reset: not reported, no letter, not fed, no document choice");
		});
		Add("Scene", "Back to Debug Panel", delegate
		{
			BackToMain(MARKETING_PANEL);
		}, tall: true);
		Add("Scene", "Toggle Tooltip", DebugPanel.ToggleCinematicMode, tall: false, null, "For clean screenshots and footage: hides the tooltips and hover outlines that would otherwise pop up over the shot.");
		Add("Scene", "Toggle Idle Animation", DebugPanel.ToggleIdleAnimation, tall: false, null, "For still shots: makes the customers stand still instead of swaying, so a screenshot catches them in a steady pose.");
		Add("Scene", "Toggle Customer Sprite Swap", DebugPanel.ToggleCustomerSpriteSwap, tall: false, null, "For showing off the cast: the customer at the counter keeps changing into other characters from the game, so one clip shows many faces.");
		Add("Scene", "Toggle Aug Sprite Swap", DebugPanel.ToggleAugSpriteSwap);
		Add("Scene", "Set Random Day and Credit", DebugPanel.SetRandomDayAndCredit);
		Add("Scene", "Setup Marketplace Request", DebugPanel.SetupMarketplaceRequests);
		Add("Kits", "Gun Kit", DebugPanel.SpawnGunKit);
		Add("Kits", "Farm Kit", DebugPanel.SpawnHydroponicKit);
		Add("Kits", "Plant DNA Kit", DebugPanel.SpawnPlantDnaKit);
		Add("Kits", "Chemistry Kit", DebugPanel.SpawnMarketingChemKit);
		Add("Kits", "Lottery Kit", DebugPanel.SpawnLotteryKit);
		Add("Kits", "Cigarette Inspection Kit", DebugPanel.SpawnCigaretteInspectionKit);
		Add("Kits", "Aquaculture Kit", DebugPanel.SpawnMarketingAquacultureKit);
		Add("Items", "Add Food Item", DebugPanel.SpawnMarketingFood);
		Add("Items", "Add Medical Item", DebugPanel.SpawnMarketingMedical);
		Add("Items", "Add Household Item", DebugPanel.SpawnMarketingHousehold);
		Add("Items", "Add Material Item", DebugPanel.SpawnMarketingMaterials);
		Add("Items", "Add Shady Item", DebugPanel.SpawnMarketingShady);
		Add("Items", "Add Business Card", DebugPanel.SpawnBusinessCards);
		Add("Items", "Add Music Cassettes", DebugPanel.SpawnMusicCassettes);
		Add("Items", "Add Music Cassettes (Album 2)", DebugPanel.SpawnMusicCassettes2);
		Add("Items", "Add Hypertech Item", DebugPanel.SpawnHypertechItems);
		Add("Customers", "Queue Normal Seller", DebugPanel.QueueTrailerSeller);
		Add("Customers", "Queue Normal Buyer", DebugPanel.QueueTrailerBuyer);
		Add("Customers", "Queue Security Inspection", DebugPanel.QueueTrailerInspection);
		Add("Customers", "Queue Appraisal", DebugPanel.QueueRandomAppraisal);
		Add("Customers", "Queue Landlord", DebugPanel.QueueLandlord);
		Add("Customers", "Queue Unique Character 1 Intro", DebugPanel.QueueInsuranceIntro);
		Add("Customers", "Queue Unique Character 2 Intro", DebugPanel.QueueWanted4Intro);
		Add("Customers", "Queue Scam Cigarette Seller", DebugPanel.QueueScamCigaretteSeller);
		Add("Customers", "Queue Custom Gun Buyer", DebugPanel.QueueCustomGunBuyer);
		registering = false;
	}

	public static void OpenModdingPanel()
	{
		OpenSubPanel(MODDING_PANEL);
	}

	public static void OpenMarketingPanel()
	{
		OpenSubPanel(MARKETING_PANEL);
	}

	public static void OpenStoryPanel()
	{
		OpenSubPanel(STORY_PANEL);
	}

	private static void OpenSubPanel(PanelDef panel)
	{
		if (!(CustomUIManager.Instance == null) && (!panel.developerOnly || DevTools.IsFullEnabled))
		{
			CustomUIManager.Instance.GetWindow("debug_panel")?.Hide();
			CustomUIWindow window = CustomUIManager.Instance.GetWindow(panel.id);
			if (window != null && window.IsAlive)
			{
				window.Show();
			}
			else
			{
				Open(panel);
			}
		}
	}

	private static void BackToMain(PanelDef panel)
	{
		if (!(CustomUIManager.Instance == null))
		{
			CustomUIManager.Instance.GetWindow(panel.id)?.Hide();
			CustomUIWindow window = CustomUIManager.Instance.GetWindow("debug_panel");
			if (window != null && window.IsAlive)
			{
				window.Show();
			}
			else
			{
				Open(MAIN_PANEL);
			}
		}
	}

	private static void OpenDebugSpawner()
	{
		CustomUIWindow customUIWindow = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("debug_item_spawner") : null);
		if (customUIWindow != null && customUIWindow.IsAlive)
		{
			customUIWindow.Show();
		}
		else
		{
			CustomUIItemSpawner.Open();
		}
	}

	private static void DebugBuyoutEnding()
	{
		PlayerStore instance = PlayerStore.Instance;
		int storeBuyPrice = instance.GetStoreBuyPrice();
		if (instance.playerCash < storeBuyPrice)
		{
			instance.ModCash(storeBuyPrice - instance.playerCash);
		}
		instance.playerCash -= storeBuyPrice;
		RunStats.OnStoreBoughtWithCash(storeBuyPrice);
		instance.IsPropertyPaid = true;
		HideDebugWindows();
		instance.ExecuteGameOver("buyout");
	}

	private static void DebugArrestEnding()
	{
		HideDebugWindows();
		PlayerStore.Instance.ExecuteGameOver("arrest");
	}

	private static void DebugBuyStoreWithDeed()
	{
		PlayerStore instance = PlayerStore.Instance;
		if (instance.IsPropertyPaid)
		{
			Debug.Log("[Debug] The store is already owned");
			return;
		}
		int storeBuyPrice = instance.GetStoreBuyPrice();
		if (instance.playerCash < storeBuyPrice)
		{
			instance.ModCash(storeBuyPrice - instance.playerCash);
		}
		instance.playerCash -= storeBuyPrice;
		RunStats.OnStoreBoughtWithCash(storeBuyPrice);
		instance.isPayingMortgage = false;
		instance.IsPropertyPaid = true;
		EmporiumAudioPlayer.Instance.PlayRegister();
		instance.storeClientManager.ReplaceLoanSharkWithRandomClient();
		instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("deed"), isOwend: true);
		instance.QueueStoreOwnedVisits();
		HideDebugWindows();
		Debug.Log("[Debug] Bought the store for " + storeBuyPrice + " credits, deed on the counter");
	}

	private static void HideDebugWindows()
	{
		if (!(CustomUIManager.Instance == null))
		{
			PanelDef[] pANELS = PANELS;
			foreach (PanelDef panelDef in pANELS)
			{
				CustomUIManager.Instance.GetWindow(panelDef.id)?.Hide();
			}
		}
	}

	private static void SetSecurityRepToMin()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId == "FACTION_SECURITY")
			{
				storeReputation.ModReputation(-400.0);
				StoreReputation.UpdateReputation();
			}
		}
	}

	public static void Toggle()
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[CustomUI] No CustomUIManager, cannot open the debug panel");
			return;
		}
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("debug_panel");
		if (window != null && window.IsAlive)
		{
			window.Toggle();
		}
		else
		{
			Open();
		}
	}

	public static void Open()
	{
		Open(MAIN_PANEL);
	}

	private static void Open(PanelDef panel)
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[CustomUI] No CustomUIManager, cannot open the debug panel");
			return;
		}
		RegisterFunctions();
		CustomUIWindow window = CustomUIManager.Instance.GetWindow(panel.id);
		Vector2? vector = ((window != null && window.IsAlive) ? new Vector2?(window.Rect.anchoredPosition) : null);
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow(panel.id, panel.title, "rear").SetSize(WindowWidth(panel), WindowHeight(panel)).SetDraggable(draggable: true)
			.SetCloseOnEscape();
		panel.buttons.Clear();
		panel.matchLabel = null;
		if (panel.searchable)
		{
			BuildSearchBar(customUIBuilder, panel);
		}
		customUIBuilder.BeginRow();
		string[] categories = panel.categories;
		foreach (string category in categories)
		{
			BuildCategoryColumn(customUIBuilder, panel, category);
		}
		customUIBuilder.End();
		if (vector.HasValue)
		{
			customUIBuilder.SetPosition(vector.Value);
		}
		else
		{
			customUIBuilder.Center();
		}
		CustomUIWindow window2 = customUIBuilder.Show();
		SetBackgroundAlpha(window2, 0.8f);
		CompactWindowPadding(window2);
		ForwardDragsToWindow(window2);
		if (panel.searchable)
		{
			ApplySearch(panel);
		}
	}

	public static void ForwardDragsToWindow(CustomUIWindow window)
	{
		if (window == null || window.root == null)
		{
			return;
		}
		UIDragHandler uIDragHandler = ((window.frame != null && window.frame.dragHandle != null) ? window.frame.dragHandle : window.root.GetComponent<UIDragHandler>());
		if (uIDragHandler == null)
		{
			return;
		}
		ScrollRect[] componentsInChildren = window.root.GetComponentsInChildren<ScrollRect>(includeInactive: true);
		foreach (ScrollRect scrollRect in componentsInChildren)
		{
			RectTransform rectTransform = ((scrollRect.viewport != null) ? scrollRect.viewport : ((scrollRect.content != null) ? (scrollRect.content.parent as RectTransform) : null));
			if (!(rectTransform == null))
			{
				CustomUIDragForwarder customUIDragForwarder = rectTransform.GetComponent<CustomUIDragForwarder>();
				if (customUIDragForwarder == null)
				{
					customUIDragForwarder = rectTransform.gameObject.AddComponent<CustomUIDragForwarder>();
				}
				customUIDragForwarder.target = uIDragHandler;
			}
		}
	}

	private static void CompactWindowPadding(CustomUIWindow window)
	{
		if (window != null)
		{
			SetPadding((window.root != null) ? window.root.GetComponent<VerticalLayoutGroup>() : null, -1f, 4);
			SetPadding((window.ContentRoot != null) ? window.ContentRoot.GetComponent<VerticalLayoutGroup>() : null, -1f, 4);
		}
	}

	private static void SetPadding(HorizontalOrVerticalLayoutGroup group, float spacing = -1f, int side = 2)
	{
		if (!(group == null))
		{
			group.padding = new RectOffset(side, side, 2, 2);
			if (spacing >= 0f)
			{
				group.spacing = spacing;
			}
		}
	}

	public static void SetBackgroundAlpha(CustomUIWindow window, float alpha)
	{
		Image image = ((window != null && window.root != null) ? window.root.GetComponent<Image>() : null);
		if (!(image == null))
		{
			Color color = image.color;
			color.a = alpha;
			image.color = color;
		}
	}

	private static void BuildCategoryColumn(CustomUIBuilder builder, PanelDef panel, string category)
	{
		builder.BeginColumn();
		builder.AddLabel("<b>" + category + "</b>");
		builder.Last?.SetPreferredSize(190f, 20f);
		if (builder.Last != null && builder.Last.text != null)
		{
			builder.Last.text.alignment = TextAlignmentOptions.Center;
		}
		builder.BeginScroll(480f);
		ConfigureScroll(builder.CurrentContainer);
		int num = 0;
		foreach (Entry entry in entries)
		{
			if (!(entry.category != category))
			{
				num++;
				Entry captured = entry;
				builder.AddButton(entry.name, delegate
				{
					Run(captured);
				});
				StyleButton(builder.Last, entry.tall);
				panel.buttons.Add((entry, builder.Last));
				string text = entry.info;
				if (!string.IsNullOrEmpty(entry.info))
				{
					AddInfoBadge(builder.Last);
				}
				if (!string.IsNullOrEmpty(entry.source))
				{
					string text2 = "Added by mod '" + entry.source + "'";
					text = (string.IsNullOrEmpty(text) ? text2 : (text + "\n\n" + text2));
				}
				if (!string.IsNullOrEmpty(text))
				{
					builder.WithTooltip(text);
				}
			}
		}
		if (num == 0)
		{
			builder.AddLabel("(no functions yet)");
		}
		builder.End();
		builder.End();
	}

	private static void BuildSearchBar(CustomUIBuilder builder, PanelDef panel)
	{
		builder.BeginRow();
		builder.AddInput("search buttons...", panel.search, null, null, delegate(string value)
		{
			panel.search = value ?? "";
			ApplySearch(panel);
		});
		builder.Last?.SetPreferredSize(-1f, 32f);
		builder.AddLabel("");
		builder.Last?.SetFixedWidth(90f);
		builder.Last?.SetPreferredSize(-1f, 32f);
		if (builder.Last != null && builder.Last.text != null)
		{
			builder.Last.text.alignment = TextAlignmentOptions.MidlineRight;
		}
		panel.matchLabel = builder.Last;
		builder.End();
	}

	private static void ApplySearch(PanelDef panel)
	{
		string[] array = (panel.search ?? "").Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
		int num = 0;
		foreach (var button in panel.buttons)
		{
			Entry item = button.entry;
			CustomUIElement item2 = button.element;
			bool flag = array.Length != 0 && MatchesAllWords(item.name, array);
			if (flag)
			{
				num++;
			}
			SetHighlighted(item2, flag);
		}
		if (panel.matchLabel != null && panel.matchLabel.IsAlive)
		{
			panel.matchLabel.SetText((array.Length == 0) ? "" : ((num == 1) ? "1 match" : $"{num} matches"));
		}
	}

	private static bool MatchesAllWords(string name, string[] words)
	{
		foreach (string value in words)
		{
			if (name.IndexOf(value, StringComparison.OrdinalIgnoreCase) < 0)
			{
				return false;
			}
		}
		return true;
	}

	private static void SetHighlighted(CustomUIElement element, bool highlighted)
	{
		if (element != null && element.IsAlive)
		{
			if (element.image != null)
			{
				element.image.color = (highlighted ? HIGHLIGHT_COLOR : BUTTON_COLOR);
			}
			if (element.text != null)
			{
				element.text.fontStyle = (highlighted ? FontStyles.Bold : FontStyles.Normal);
			}
		}
	}

	private static void ConfigureScroll(RectTransform scrollContent)
	{
		ConfigureColumnScroll(scrollContent, 190f, 2f);
	}

	public static void ConfigureColumnScroll(RectTransform scrollContent, float columnWidth, float spacing)
	{
		Transform transform = scrollContent;
		while (transform != null && transform.name != "scroll")
		{
			transform = transform.parent;
		}
		LayoutElement layoutElement = ((transform != null) ? transform.GetComponent<LayoutElement>() : null);
		if (layoutElement != null)
		{
			layoutElement.flexibleWidth = 0f;
			layoutElement.preferredWidth = columnWidth;
			layoutElement.minWidth = columnWidth;
		}
		ScrollRect scrollRect = ((transform != null) ? transform.GetComponentInChildren<ScrollRect>(includeInactive: true) : null);
		if (scrollRect != null)
		{
			scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
		}
		scrollContent.anchoredPosition = new Vector2(0f, scrollContent.anchoredPosition.y);
		scrollContent.sizeDelta = new Vector2(0f, scrollContent.sizeDelta.y);
		SetPadding(scrollContent.GetComponent<VerticalLayoutGroup>(), spacing, 0);
	}

	private static void StyleButton(CustomUIElement element, bool tall)
	{
		if (element != null)
		{
			element.SetPreferredSize(190f, tall ? 60f : 30f);
			if (element.text != null)
			{
				element.text.alignment = TextAlignmentOptions.Center;
				element.text.enableWordWrapping = false;
				element.text.overflowMode = TextOverflowModes.Overflow;
				element.text.fontSizeMax = element.text.fontSize;
				element.text.fontSizeMin = 9f;
				element.text.enableAutoSizing = true;
			}
		}
	}

	private static void AddInfoBadge(CustomUIElement button)
	{
		if (button != null && button.IsAlive)
		{
			HorizontalLayoutGroup component = button.root.GetComponent<HorizontalLayoutGroup>();
			int num = ((component != null) ? component.padding.right : 4);
			GameObject gameObject = new GameObject("info_badge", typeof(RectTransform), typeof(Image));
			RectTransform obj = gameObject.transform as RectTransform;
			obj.SetParent(button.root.transform, worldPositionStays: false);
			obj.anchorMin = new Vector2(1f, 0.5f);
			obj.anchorMax = new Vector2(1f, 0.5f);
			obj.pivot = new Vector2(1f, 0.5f);
			obj.anchoredPosition = new Vector2(-num, 0f);
			obj.sizeDelta = new Vector2(16f, 16f);
			gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
			Image component2 = gameObject.GetComponent<Image>();
			component2.sprite = GetInfoBadgeSprite();
			component2.raycastTarget = false;
			if (button.text != null)
			{
				component2.color = button.text.color;
			}
			int num2 = num + 16 + 4;
			if (component != null)
			{
				component.padding = new RectOffset(component.padding.left, num2, component.padding.top, component.padding.bottom);
			}
			else if (button.text != null)
			{
				Vector4 margin = button.text.margin;
				margin.z = num2;
				button.text.margin = margin;
			}
		}
	}

	private static Sprite GetInfoBadgeSprite()
	{
		if (infoBadgeSprite != null)
		{
			return infoBadgeSprite;
		}
		int num = INFO_BADGE_ART.Length;
		Texture2D texture2D = new Texture2D(num, num, TextureFormat.RGBA32, mipChain: false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp,
			name = "debug_info_badge"
		};
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				texture2D.SetPixel(j, num - 1 - i, (INFO_BADGE_ART[i][j] == '#') ? Color.white : Color.clear);
			}
		}
		texture2D.Apply();
		infoBadgeSprite = Sprite.Create(texture2D, new Rect(0f, 0f, num, num), new Vector2(0.5f, 0.5f), 100f);
		infoBadgeSprite.name = "debug_info_badge";
		return infoBadgeSprite;
	}

	private static void Run(Entry entry)
	{
		if (entry.action == null)
		{
			return;
		}
		try
		{
			entry.action();
		}
		catch (Exception arg)
		{
			Debug.LogError($"[CustomUI] Debug function '{entry.name}' threw: {arg}");
		}
	}

	public static void ToggleLegacyPanel()
	{
		if (DevTools.IsFullEnabled)
		{
			GameObject gameObject = ((StoreUIManager.Instance != null) ? StoreUIManager.Instance.debugPanel : null);
			if (gameObject == null)
			{
				Debug.LogWarning("[CustomUI] No legacy debug panel in this scene");
			}
			else
			{
				gameObject.SetActive(!gameObject.activeSelf);
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour
{
	public GameObject debugEventPanel;

	public TextMeshProUGUI currentEventTMP;

	public TextMeshProUGUI cooldownEventTMP;

	public GameObject debugPricePanel;

	public TextMeshProUGUI priceDebugTMP;

	public TextMeshProUGUI priceDebugTMP2;

	public TextMeshProUGUI priceDebugTMP3;

	public GameObject debugButtonPrefab;

	public Transform spawnGroup;

	public Transform kitGroup;

	public Transform uiGroup;

	public Transform cmdGroup;

	public Transform marketingGroup;

	private const float SPRITE_SWAP_INTERVAL = 0.8f;

	private static List<string> spriteList = new List<string>
	{
		"mentor", "joe", "sawyer", "officer_lun", "grandpa_cheng", "serpico", "eleanor1", "george", "landlord", "wanted1",
		"wanted2", "wanted3", "wanted4", "wanted5", "wanted6", "wanted7", "wanted8", "wanted9", "maleScav1", "maleScav2",
		"maleScav3", "maleScav4", "femaleScav1", "femaleScav2", "femaleScav3"
	};

	private static List<string> augSpriteList = new List<string>
	{
		"augLLMale1", "augLLMale2", "augLLMale3", "augLLFemale1", "augLLFemale2", "augLLFemale3", "augBMMale1", "augBMMale2", "augBMMale3", "augBMFemale1",
		"augBMFemale2", "augBMFemale3", "augULMale1", "augULMale2", "augULMale3", "augULFemale1", "augULFemale2", "augULFemale3"
	};

	private static bool isQuickCustomerSpriteSwapping = false;

	private static int currentSpriteIndex = 0;

	private static float spriteSwapTimer = 0f;

	private static bool isQuickAugSpriteSwapping = false;

	private static int currentAugSpriteIndex = 0;

	private static float augSpriteSwapTimer = 0f;

	private static MonoBehaviour spriteSwapHost;

	public static DebugPanel Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		AddProceduralDebugFunction();
	}

	private void Update()
	{
		if (debugEventPanel.activeSelf)
		{
			if (StoreStation.Instance.storeEventManager.eventTracker.Count == 0)
			{
				cooldownEventTMP.text = "No events on cooldown.";
			}
			else
			{
				string text = "";
				foreach (string key in StoreStation.Instance.storeEventManager.eventTracker.Keys)
				{
					text = text + key + " : Cooldown" + StoreStation.Instance.storeEventManager.eventTracker[key] + "\n";
				}
				cooldownEventTMP.text = text;
			}
			if (StoreStation.Instance.storeEventManager.GetActiveEvents().Count == 0)
			{
				currentEventTMP.text = "No events is activate.";
			}
			else
			{
				string text2 = "";
				foreach (StoreEvent activeEvent in StoreStation.Instance.storeEventManager.GetActiveEvents())
				{
					text2 = ((activeEvent.eventType != 0) ? (text2 + activeEvent.identifier + " : Duration " + activeEvent.duration + "\n") : (text2 + "<b>" + activeEvent.identifier + " : Duration " + activeEvent.duration + "</b>\n"));
				}
				currentEventTMP.text = text2;
			}
		}
		_ = debugPricePanel.activeSelf;
	}

	public static void Add100Credits()
	{
		PlayerStore.Instance.ModCash(100);
	}

	public static void Remove100Credits()
	{
		PlayerStore.Instance.ModCash(-100);
	}

	public static void Add10000Credits()
	{
		PlayerStore.Instance.ModCash(10000);
	}

	public static void SetCreditToZero()
	{
		PlayerStore.Instance.playerCash = 0;
	}

	public static void SpawnGenuineStamp()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRealStamp());
	}

	public static void SpawnFakeStamp()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateFakeStamp());
	}

	public static void SpawnDebugKit()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("newspaper"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("labeler"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("magnifier"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cigarette_guide"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("fuel_guide"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("stamp_guide"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("logo_checker"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("intel_sec"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wanted_paper"));
	}

	public void OpenDebugSpawnMenu()
	{
		base.gameObject.SetActive(value: false);
		DebugHandler.current.ToggleSpawnMenu();
	}

	public static void EnableDevMode()
	{
		PlayerStore.Instance.devMode = true;
	}

	public static void DisableDevMode()
	{
		PlayerStore.Instance.devMode = false;
	}

	public static void StealRandomItem()
	{
		PlayerStore.Instance.StealRandomEligibleItem();
	}

	public static void SpawnWaterTestingKit()
	{
		WaterHelper.SpawnWaterTestingKit();
	}

	public static void SetActiveAlcoholAddiction()
	{
		if (!HealthData.IsAlcoholAddicted())
		{
			PlayerStore.Instance.startingPerks.Add(StartingPerkList.AlcoholAddiction());
		}
		PlayerStore.Instance.healthData.daysSinceAlcoholConsumption = 3;
		Debug.Log("Alcohol addiction set. Active penalty: " + PlayerStore.Instance.healthData.GetTotalAddictionPenalty() + "%");
	}

	public static void SimulateSoldMislabeledGhostwater()
	{
		PlayerStore.Instance.secData.SellDirtyWater(3000);
		Debug.Log("Dirty water tracker: " + PlayerStore.Instance.secData.dirtyWaterTracker + " mL. Threat note arrives at next shutter open.");
	}

	public static void CommitCrime()
	{
		PlayerStore.Instance.secData.CommitCrime("BRIBERY", 800);
		PlayerStore.Instance.secData.CommitCrime("CARD_TRAFFICKING", 1200);
		PlayerStore.Instance.secData.evidenceLevel = 98;
	}

	public static void ForceCloseShutter()
	{
		StoreShutterButton.Instance.CloseShutter();
	}

	public static void ForceOpenShutter()
	{
		StoreShutterButton.Instance.OpenShutter();
	}

	public static void SpawnCrateAndCard()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.LootCrateEngineering());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.LootCrateEvidence());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.LootCrateMedical());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.LootCrateSecurity());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.LootCrateService());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("sec_keycard"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("sec_keycard"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("med_keycard"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("eng_keycard"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cmd_keycard"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("ser_keycard"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("sup_keycard"));
	}

	public static void SpawnSmugglerKit()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("sec_keycard"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("desequencer"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("smuggler_bay"));
	}

	public static void ForceClearTrashcan()
	{
		PlayerStore.Instance.ClearTrash();
	}

	public static void ClearMainInv()
	{
		for (int num = EmporiumEntry.Instance.invElement.childItems.Count - 1; num >= 0; num--)
		{
			EmporiumEntry.Instance.invElement.childItems[num].CloseContentWindow(includeChildren: true);
			EmporiumEntry.Instance.invElement.childItems[num].Destroy();
		}
	}

	public static void OpenWildUI()
	{
		WildNetworkUI.Open(debug: true);
	}

	public static void SpawnGenuineCigarette()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.GenuineCigarette());
	}

	public static void SpawnCounterfeitCigarette()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.FakeCigarette());
	}

	public static void SkipNightMachine()
	{
		PlayerStore.Instance.FindAllItem().ForEach(delegate(GameItem i)
		{
			i.onCycleEndEarlySlotItemFunc?.Invoke(i, i.parentInventory, i.GetIncompleteSlotInInventory());
		});
		MachineFridge.ClearColdMarkers();
		PlayerStore.Instance.FindAllItem().ForEach(delegate(GameItem i)
		{
			i.onCycleEndSlotItemFunc?.Invoke(i, i.parentInventory, i.GetIncompleteSlotInInventory());
		});
		PlayerStore.Instance.FindAllItem().ForEach(delegate(GameItem i)
		{
			i.onCycleEndLateSlotItemFunc?.Invoke(i, i.parentInventory, i.GetIncompleteSlotInInventory());
		});
		MachineFridge.HandleColdFeatureExpiry();
	}

	public static void EventManagerStartDay()
	{
		StoreStation.Instance.storeEventManager.OnDayStart();
	}

	public static void EventManagerEndDay()
	{
		StoreStation.Instance.storeEventManager.OnDayEnd();
	}

	public void OpenDebugEventPanel()
	{
		debugEventPanel.SetActive(value: true);
	}

	public void OpenDebugPricePanel()
	{
		debugPricePanel.SetActive(value: true);
	}

	public static void StartWaterShortageEvent()
	{
		StoreStation.Instance.storeEventManager.ActivateEvent(EventListNormal.CreateWaterTreatmentPlantBreakdown());
	}

	public static void StartWaterSurplusEvent()
	{
		StoreStation.Instance.storeEventManager.ActivateEvent(EventListNormal.CreateMinerReturnFromIceAsteroid());
	}

	public static void SkipNightNormal()
	{
		StoreStation.Instance.dayCounter++;
		PlayerStore.Instance.dayUntilRent--;
	}

	public static void DismissNextClient()
	{
		if (PlayerStore.Instance.storeClientManager.clientStack.Count != 0)
		{
			PlayerStore.Instance.storeClientManager.clientStack.RemoveAt(0);
			PlayerStore.Instance.LogQueuedClient();
		}
	}

	public static void SpawnFarmingKit()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("aquascan"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nutrient_tablet"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nutrient_tablet"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("kotton_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("kotton_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nutrifruit_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nutrifruit_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("hydroreed_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("hydroreed_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("farm_module_nutrifruit"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("farm_module_dream_cap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("farm_module_kotton_loom"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("farm_module_nightmare_cap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("plant_analyser"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("hydroreed_filter"));
		GameItem gameItem = PreBuiltItemHelper.CreateHydroponic();
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem);
		GraphUtils.TryAcceptAll(gameItem, PreBuiltItemHelper.EnergyCreditExt());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, WaterPremadeHelper.AccurateGhostWater("large_bottled_water"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, WaterPremadeHelper.AccurateBaseWater("large_bottled_water"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, WaterPremadeHelper.AccurateHighQualityWater("large_bottled_water"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateNutrientSolution());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.FullNutrientTabletBox());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("module_bay_expansion_kit"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("module_bay_expansion_kit"));
	}

	public static void SpawnBrokenMachineKit()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateBrokenFurnace());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateBrokenAlarm());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateBrokenBrokenMoistureFarm());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("screwdriver"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("welder"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wire_cutter"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("common_electronic"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("system_module_corrupt"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("module_extractor"));
	}

	public static void SpawnRuinedMachines()
	{
		string[] array = new string[3] { "ruined_machine_small", "ruined_machine_medium", "ruined_machine_large" };
		foreach (string identifier in array)
		{
			for (int j = 0; j < 3; j++)
			{
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item(identifier));
			}
		}
		GameItem gameItem = DirectoryMaster.Item("toolbox");
		gameItem.ModifyTag("TOOLBOX_UNLOCKED_SLOTS_INT", delegate(TagState s)
		{
			s.Enable().SetInt(4);
		});
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem);
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("screwdriver"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("welder"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("module_extractor_advanced"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("wire_cutter"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.EnergyCredit(100));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.EnergyCredit(100));
	}

	public static void SpawnGunKit()
	{
		GameItem gameItem = PreBuiltItemHelper.CreatePrinter();
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem);
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreatePrinter());
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit_ext"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("printer_chip_pistol"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("printer_plastic"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("printer_plastic"));
		gameItem.onCycleEndSlotItemFunc?.Invoke(gameItem, gameItem.parentInventory, gameItem.GetIncompleteSlotInInventory());
		string[] array = new string[28]
		{
			"printer_chip_pistol_double", "printer_chip_rds", "printer_chip_laser_sight", "printer_chip_silencer", "printer_chip_22_mag", "printer_chip_22_mag_high", "printer_chip_22_mag_ext", "printer_chip_10_mag", "printer_chip_10_mag_high", "printer_chip_10_mag_ext",
			"printer_chip_box_cutter", "printer_chip_kitchen_knife", "printer_chip_hatchet", "printer_chip_kitchen_cleaver", "printer_chip_combat_knife", "printer_chip_combat_machete", "printer_chip_handmade_pistol_frame", "printer_chip_heavy_handmade_pistol_frame", "printer_chip_gun_part", "printer_module_metal",
			"printer_module_melee_batch", "printer_module_cnc", "handmade_pistol_frame", "heavy_handmade_pistol_frame", "metal_ingot", "metal_ingot", "metal_ingot", "metal_ingot"
		};
		foreach (string identifier in array)
		{
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item(identifier));
		}
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("screwdriver"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wire_cutter"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("gun_part"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("gun_part"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("smg"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("smg"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("laser"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("compensator"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("rds"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("bipod"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("silencer2"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("silencer2"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("gl"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("stock"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("short_stock"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("stock"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("short_stock"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("mag_10mm_smg_high"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("mag_10mm_smg"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("mag_10mm_smg"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("grip"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("printer_plastic"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("printer_plastic"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("printed_gun"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("printed_gun"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("mag_22_high"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("mag_22"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("match_barrel"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("trigger_group"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("ammo_display"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("flashlight"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("heavy_pistol_ammo"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("heavy_pistol_ammo"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("heavy_pistol_ammo"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("heavy_pistol_ammo_p"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("heavy_pistol_ammo_p"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("heavy_pistol_ammo_p"));
	}

	public static void SetRepToMax()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			storeReputation.ModReputation(400.0);
			StoreReputation.UpdateReputation();
		}
	}

	public static void SetRepToMin()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			storeReputation.ModReputation(-400.0);
			StoreReputation.UpdateReputation();
		}
	}

	public static void SpawnInjectors()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRealGenuineInjector());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRealUnusableInjector());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRealExpiredInjector());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRealCounterfeitInjector());
	}

	public static void SpawnPlasma()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreatePlasmaRandom(300));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreatePlasmaRandom(300));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreatePlasmaRandom(300));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreatePlasmaRandom(300));
	}

	public static void QueueRandomFutureEvent()
	{
		StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateMiningExpeditionReturn(), 1, IsKnown: true);
		StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateNutrifruitHarvest(2), 5, IsKnown: true);
		StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateStoreRoomExcavatedDuration(2), 3, IsKnown: true);
		StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateWaterTreatmentPlantBreakdownDuration(2), 6, IsKnown: true);
	}

	public static void SpawnHydroponicKit()
	{
		for (int i = 0; i < 4; i++)
		{
			GameItem gameItem = PreBuiltItemHelper.CreateHydroponic();
			GraphUtils.TryAcceptAll(gameItem, PreBuiltItemHelper.EnergyCreditExt());
			GraphUtils.TryAcceptAll(gameItem, WaterPremadeHelper.AccurateBaseWater());
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem);
			switch (i)
			{
			case 0:
				GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("nutrifruit_seed"));
				break;
			case 1:
				GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("dream_cap_seed"));
				break;
			case 2:
				GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("kotton_seed"));
				break;
			case 3:
				GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("hydroreed_seed"));
				break;
			}
			GraphUtils.TryAcceptAll(gameItem, ItemSpawner.Spawn("random_quality_module"));
			GraphUtils.TryAcceptAll(gameItem, ItemSpawner.Spawn("random_efficiency_module"));
			GraphUtils.TryAcceptAll(gameItem, ItemSpawner.Spawn("random_efficiency_module"));
			GraphUtils.TryAcceptAll(gameItem, ItemSpawner.Spawn("random_quality_module"));
			for (int j = 0; j < 6; j++)
			{
				gameItem.onCycleEndSlotItemFunc?.Invoke(gameItem, gameItem.parentInventory, gameItem.GetIncompleteSlotInInventory());
			}
			switch (i)
			{
			case 0:
				GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("nutrifruit_seed"));
				break;
			case 1:
				GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("dream_cap_seed"));
				break;
			case 2:
				GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("kotton_seed"));
				break;
			case 3:
				GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("hydroreed_seed"));
				break;
			}
			gameItem.onCycleEndSlotItemFunc?.Invoke(gameItem, gameItem.parentInventory, gameItem.GetIncompleteSlotInInventory());
		}
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nutrifruit_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nutrifruit_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nutrifruit_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap_seed"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nutrifruit_seed_packet"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nightmare_cap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("plant_analyser"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nutrifruit"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nutrifruit"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateNutrifruitLarge());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateNutrifruitLarge());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateNutrifruitHuge());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("hatchet"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.FullNutrientTabletBox());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, WaterPremadeHelper.AccurateBaseWater("large_bottled_water"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, WaterPremadeHelper.AccurateBaseWater("large_bottled_water"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, WaterPremadeHelper.AccurateBaseWater("small_bottled_water"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("hydroponic_guide"));
		NetworkUpgrade.GetUpgradeById("GUTTERFLOW_TAP").Unlock();
		NetworkUpgrade.GetUpgradeById("RUSTWATER_TAP").Unlock();
	}

	public static void SpawnPlantDnaKit()
	{
		GameInventory invElement = EmporiumEntry.Instance.invElement;
		GameItem gameItem = PreBuiltItemHelper.CreateDnaManipulator();
		GraphUtils.TryAcceptAll(gameItem, PreBuiltItemHelper.EnergyCreditExt());
		GraphUtils.TryAcceptAll(invElement, gameItem);
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("dna_manipulator_module_stabilizer"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("dna_manipulator_module_positive_filter"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("dna_manipulator_module_negative_filter"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("dna_manipulator_module_precision"));
		GraphUtils.TryAcceptAll(invElement, ItemSpawner.Spawn("random_quality_module"));
		GraphUtils.TryAcceptAll(invElement, ItemSpawner.Spawn("random_performance_module"));
		for (int i = 0; i < 3; i++)
		{
			GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("system_capped_neural_core"));
		}
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("gene_scanner"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("plant_analyser"));
		for (int j = 0; j < 6; j++)
		{
			GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("plant_dna_disk"));
		}
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("fast_grower"));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("fast_grower"));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("fast_grower", 2));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("fast_grower", 10));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("bountiful"));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("delicious"));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("edible"));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("hardy"));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("drought_resistant"));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("slow_grower"));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("unappetizing"));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("perennial"));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk(PlantTraitList.ProductionId("caffeine")));
		GraphUtils.TryAcceptAll(invElement, CreateWrittenDnaDisk("potent"));
		GameItem gameItem2 = DirectoryMaster.Item("hypertech_bay");
		if (gameItem2 != null)
		{
			GraphUtils.TryAcceptAll(invElement, gameItem2);
		}
		foreach (PlantDefinition item in PlantRegistry.All)
		{
			GameItem gameItem3 = DirectoryMaster.Item(item.id);
			if (gameItem3 != null)
			{
				if (gameItem2 != null)
				{
					GraphUtils.TryAcceptAll(gameItem2, gameItem3);
				}
				else
				{
					GraphUtils.TryAcceptAll(invElement, gameItem3);
				}
			}
		}
		GraphUtils.TryAcceptAll(invElement, CreateModifiedSeed("nutrifruit_seed", ("fast_grower", 3), ("bountiful", 2), ("hardy", 1)));
		GraphUtils.TryAcceptAll(invElement, CreateModifiedSeed("kotton_seed", ("edible", 1), ("delicious", 2)));
		GraphUtils.TryAcceptAll(invElement, CreateModifiedSeed("fillerweed_seed", ("fast_grower", 1), ("thirsty", 1)));
		GameItem gameItem4 = CreateModifiedSeed("coffee_bean", ("bountiful", 1));
		PlantGeneticsHelper.AddHeat(gameItem4, PlantTraitRegistry.Get("bountiful"));
		PlantGeneticsHelper.AddHeat(gameItem4, PlantTraitRegistry.Get("bountiful"));
		GraphUtils.TryAcceptAll(invElement, gameItem4);
		GameItem gameItem5 = PreBuiltItemHelper.CreateHydroponic();
		GraphUtils.TryAcceptAll(gameItem5, PreBuiltItemHelper.EnergyCreditExt());
		GraphUtils.TryAcceptAll(gameItem5, WaterPremadeHelper.AccurateBaseWater());
		GraphUtils.TryAcceptAll(invElement, gameItem5);
		GraphUtils.TryAcceptAll(invElement, PreBuiltItemHelper.EnergyCreditExt());
		GraphUtils.TryAcceptAll(invElement, WaterPremadeHelper.AccurateBaseWater("large_bottled_water"));
		GraphUtils.TryAcceptAll(invElement, PreBuiltItemHelper.FullNutrientTabletBox());
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("hatchet"));
	}

	private static GameItem CreateWrittenDnaDisk(string traitId, int rank = 1)
	{
		GameItem gameItem = DirectoryMaster.Item("plant_dna_disk");
		PlantGeneticsHelper.WriteDisk(gameItem, PlantTraitRegistry.Get(traitId), rank);
		return gameItem;
	}

	private static GameItem CreateModifiedSeed(string seedId, params (string traitId, int rank)[] traits)
	{
		GameItem gameItem = DirectoryMaster.Item(seedId);
		for (int i = 0; i < traits.Length; i++)
		{
			var (traitId, rank) = traits[i];
			PlantGeneticsHelper.SetTraitRank(gameItem, traitId, rank);
		}
		return gameItem;
	}

	public static void SpawnChemKit()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("heating_plate"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("blender"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateEvaporator());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("chem_finisher"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("coffee_bean"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("coffee_bean"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_dust"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_dust"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_dust"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("birth_control_pill"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("caffeine_pill"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("large_bottled_water"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("aquascan"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("kotton_fabric"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("chem_scanner"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateChemicalBottle("alcohol"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.EnergyCredit(100));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("1ml_dropper"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("5ml_dropper"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("25ml_cylinder"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("50ml_cylinder"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("100ml_cylinder"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("chem_module"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("chem_module"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("chem_module"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("chem_module"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("fanta_pill"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("common_chemical"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("restricted_chemical"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("module_extractor"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("evaporator_module_solvent_recovery"));
	}

	public static void SpawnRatKit()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("rat"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("rat"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRat("MALE", 5));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRat("FEMALE", 5));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("animal_cage"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("livestock_feed_dispenser"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("auto_butcher"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("intake_hopper"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("outlet_hopper"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("marking_pen"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("scrap_metal"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("scrap_metal"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("metal_ingot"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("metal_ingot"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("nutrifruit"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("nutrifruit"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("nutrifruit"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("nutrifruit"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("nutrifruit"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("wire_cutter"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("kitchen_cleaver"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("labeler_blue"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("serum_red"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("serum_green"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("gene_scanner"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("processed_cheese"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("small_morsel"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("morsel"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("small_raw_meat"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("vacuum_robot"));
	}

	public static void SpawnAquacultureKit()
	{
		GameItem item = SpawnRunningRAS(withDispenser: true);
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, item);
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("ras_water_tank"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("water_filter"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.EnergyCredit(100));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("kitchen_cleaver"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("auto_butcher"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("intake_hopper"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("outlet_hopper"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("marking_pen"));
		int age = FishSpecies.Tilapia.stageStartDays[4];
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFish(FishSpecies.Tilapia, "MALE", age));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFish(FishSpecies.Tilapia, "FEMALE", age));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn(FishSpecies.Tilapia.fishIdentifier));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn(FishSpecies.Tilapia.fryIdentifier));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFertilizedRoe(FishSpecies.Tilapia));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn(FishSpecies.Tilapia.roeIdentifier));
		int age2 = FishSpecies.GutterTilapia.stageStartDays[4];
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFish(FishSpecies.GutterTilapia, "MALE", age2));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFish(FishSpecies.GutterTilapia, "FEMALE", age2));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFertilizedRoe(FishSpecies.GutterTilapia));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn(FishSpecies.GutterTilapia.roeIdentifier));
		int age3 = FishSpecies.GraftTilapia.stageStartDays[4];
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFish(FishSpecies.GraftTilapia, "MALE", age3));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFish(FishSpecies.GraftTilapia, "FEMALE", age3));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFertilizedRoe(FishSpecies.GraftTilapia));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn(FishSpecies.GraftTilapia.roeIdentifier));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("tilapia_skin_graft"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("tiny_fish_fillet"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("fish_scrap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("small_fish_fillet"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("fish_fillet"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("large_fish_fillet"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("processed_cheese"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("processed_cheese"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("gutterflow_pellet"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("industrial_fish_feed"));
		string[] array = new string[6] { "raw_meat", "small_raw_meat", "morsel", "small_morsel", "fat_meat", "meat_scrap" };
		foreach (string itemID in array)
		{
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn(itemID));
		}
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFish(FishSpecies.Eel, "MALE", 0));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFish(FishSpecies.Eel, "FEMALE", 0));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("small_eel_fillet"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("eel_fillet"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFish(FishSpecies.ElectricEel, "MALE", 0));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFish(FishSpecies.ElectricEel, "FEMALE", FishSpecies.ElectricEel.stageStartDays[5]));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, AquacultureItems.SpawnFish(FishSpecies.Sturgeon, "FEMALE", FishSpecies.Sturgeon.stageStartDays[3]));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, SpawnGravidSturgeon());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("caviar"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("sturgeon_roe"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("surgery_tool"));
	}

	public static GameItem SpawnRunningRAS(bool withDispenser, int extraColumns = 0, int extraRows = 0)
	{
		GameItem gameItem = ItemSpawner.Spawn("ras");
		GraphUtils.TryAcceptAll(gameItem, PreBuiltItemHelper.EnergyCredit(100));
		GameItem gameItem2 = ItemSpawner.Spawn("ras_water_tank");
		WaterHelper.FillWithPureWater(gameItem2);
		GraphUtils.TryAcceptAll(gameItem, gameItem2);
		for (int i = 0; i < 4; i++)
		{
			GraphUtils.TryAcceptAll(gameItem, ItemSpawner.Spawn("water_filter"));
		}
		if (withDispenser)
		{
			GraphUtils.TryAcceptAll(gameItem, ItemSpawner.Spawn("livestock_feed_dispenser"));
		}
		if (extraColumns > 0 || extraRows > 0)
		{
			gameItem.ModifyTag("RAS_WIDTH_UPGRADE_COUNT_TAG", delegate(TagState state)
			{
				state.SetInt(Mathf.Min(extraColumns, 4));
			});
			gameItem.ModifyTag("RAS_HEIGHT_UPGRADE_COUNT_TAG", delegate(TagState state)
			{
				state.SetInt(Mathf.Min(extraRows, 4));
			});
			MachineRAS.ApplyTankSize(gameItem);
			MachineRAS.UpdatePowerUsage(gameItem);
		}
		return gameItem;
	}

	public static GameItem SpawnGravidSturgeon()
	{
		GameItem gameItem = AquacultureItems.SpawnFish(FishSpecies.Sturgeon, "FEMALE", FishSpecies.Sturgeon.stageStartDays[4]);
		if (gameItem == null)
		{
			return null;
		}
		int load = AquacultureHelper.GetEggTargetGrams(gameItem);
		gameItem.ModifyTag("FISH_EGG_GRAMS_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(load);
		});
		gameItem.ModifyTag("FISH_KCAL_EATEN_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(state.GetFloat() + (float)load * FishSpecies.Sturgeon.eggKcalPerGram);
		});
		gameItem.ModifyTag("FISH_KCAL_GROWN_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(state.GetFloat() + (float)load * FishSpecies.Sturgeon.eggKcalPerGram);
		});
		return gameItem;
	}

	public static void ToggleCinematicMode()
	{
		PlayerStore.Instance.cinematicMode = !PlayerStore.Instance.cinematicMode;
	}

	public static void ToggleIdleAnimation()
	{
		IdleAnimator.SetGloballyEnabled(!IdleAnimator.GloballyEnabled);
	}

	public static void SetRandomDayAndCredit()
	{
		PlayerStore.Instance.playerCash = UnityEngine.Random.Range(50, 10000);
		StoreStation.Instance.dayCounter = UnityEngine.Random.Range(7, 100);
	}

	public static void SetupMarketplaceRequests()
	{
		NetworkUpgrade.GetUpgradeById("MARKETPLACE_REQUESTS").Unlock();
		StoreRequest.ActivateRequest(StoreRequestList.ULRareItemRequest());
		StoreRequest.ActivateRequest(StoreRequestList.CrowbarSawyer());
		StoreRequest.ActivateRequest(StoreRequestList.ButcherCleaver());
		StoreRequest.ActivateRequest(StoreRequestList.GeorgioToiletPaper());
		StoreRequest.ActivateRequest(StoreRequestList.LandlordSpecial2());
		StoreRequest.ActivateRequest(StoreRequestList.LandlordSpecial());
		StoreRequest.ActivateRequest(StoreRequestList.LLConstruction());
		StoreRequest.ActivateRequest(StoreRequestList.LunRetirementParty());
		StoreRequest.ActivateRequest(StoreRequestList.SawyerBeer());
	}

	public static void ToggleCustomerSpriteSwap()
	{
		isQuickCustomerSpriteSwapping = !isQuickCustomerSpriteSwapping;
		StartSpriteSwapIfNeeded();
	}

	public static void ToggleAugSpriteSwap()
	{
		isQuickAugSpriteSwapping = !isQuickAugSpriteSwapping;
		StartSpriteSwapIfNeeded();
	}

	private static void StartSpriteSwapIfNeeded()
	{
		if ((isQuickCustomerSpriteSwapping || isQuickAugSpriteSwapping) && !(spriteSwapHost != null) && !(StoreUIManager.Instance == null))
		{
			spriteSwapHost = StoreUIManager.Instance;
			spriteSwapHost.StartCoroutine(SpriteSwapLoop());
		}
	}

	private static IEnumerator SpriteSwapLoop()
	{
		while (isQuickCustomerSpriteSwapping || isQuickAugSpriteSwapping)
		{
			yield return null;
			if (PlayerStore.Instance == null || PlayerStore.Instance.currentClientInstance == null)
			{
				continue;
			}
			if (isQuickCustomerSpriteSwapping && spriteList.Count > 0)
			{
				spriteSwapTimer += Time.deltaTime;
				if (spriteSwapTimer >= 0.8f)
				{
					spriteSwapTimer -= 0.8f;
					StoreUIManager.Instance.clientGameObject.image.sprite = SpriteDict.Instance.GetSprite(spriteList[currentSpriteIndex]);
					currentSpriteIndex++;
					if (currentSpriteIndex >= spriteList.Count)
					{
						currentSpriteIndex = 0;
					}
				}
			}
			if (!isQuickAugSpriteSwapping || augSpriteList.Count <= 0)
			{
				continue;
			}
			augSpriteSwapTimer += Time.deltaTime;
			if (augSpriteSwapTimer >= 0.8f)
			{
				augSpriteSwapTimer -= 0.8f;
				StoreUIManager.Instance.clientGameObject.image.sprite = SpriteDict.Instance.GetSprite(augSpriteList[currentAugSpriteIndex]);
				currentAugSpriteIndex++;
				if (currentAugSpriteIndex >= augSpriteList.Count)
				{
					currentAugSpriteIndex = 0;
				}
			}
		}
		spriteSwapHost = null;
	}

	public static void SpawnMarketingChemKit()
	{
		GameItem gameItem = DirectoryMaster.Item("heating_plate");
		GameItem gameItem2 = DirectoryMaster.Item("blender");
		GameItem gameItem3 = PreBuiltItemHelper.CreateEvaporator();
		GameItem gameItem4 = DirectoryMaster.Item("chem_finisher");
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem);
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem2);
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem3);
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem4);
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("100ml_cylinder"));
		GraphUtils.TryAcceptAll(gameItem2, DirectoryMaster.Item("100ml_cylinder"));
		GraphUtils.TryAcceptAll(gameItem3, DirectoryMaster.Item("100ml_cylinder"));
		GraphUtils.TryAcceptAll(gameItem4, DirectoryMaster.Item("100ml_cylinder"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_cap"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("coffee_bean"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("coffee_bean"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_dust"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_dust"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_dust"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("birth_control_pill"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("caffeine_pill"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("large_bottled_water"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("aquascan"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("kotton_fabric"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("chem_scanner"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateChemicalBottle("alcohol"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.EnergyCredit(100));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("1ml_dropper"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("5ml_dropper"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("25ml_cylinder"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("50ml_cylinder"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("100ml_cylinder"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("100ml_cylinder"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("chem_module"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("chem_module"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("chem_module"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("chem_module"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("fanta_pill"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("fanta_pill"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("fanta_pill"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("common_chemical"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("restricted_chemical"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("module_extractor"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("evaporator_module_solvent_recovery"));
		GameItem gameItem5 = DirectoryMaster.Item("mini_bottle");
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem5);
		GameItem gameItem6 = DirectoryMaster.Item("mini_bottle");
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem6);
		GameItem gameItem7 = DirectoryMaster.Item("mini_bottle");
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem7);
		WaterHelper.AddLiquid(gameItem5, "acid", 250000);
		WaterHelper.AddLiquid(gameItem6, "physical_contaminant", 250000);
		WaterHelper.AddLiquid(gameItem7, "dream_cap_mush", 250000);
		GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuildChemHelper.CreateLipobondAgentBottle());
		GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuildChemHelper.CreateAcidBottle());
		GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuildChemHelper.CreateBaseBottle());
		GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuildChemHelper.CreateFantaPrecusorBottle());
		GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuildChemHelper.CreateReducingAgentBottle());
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("chemistry_guide"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nuclear_waste"));
	}

	public static void SpawnLotteryKit()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("scratcher_stack"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("scratcher_stack"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("scratcher"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("scratcher"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("scratcher"));
	}

	public static void SpawnCigaretteInspectionKit()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cigarette_color"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cigarette_guide"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("magnifier"));
	}

	public static void SpawnMarketingAquacultureKit()
	{
		NetworkUpgrade upgradeById = NetworkUpgrade.GetUpgradeById("GUTTERFLOW_TAP");
		NetworkUpgrade upgradeById2 = NetworkUpgrade.GetUpgradeById("RUSTWATER_TAP");
		bool flag = false;
		if (upgradeById != null && !upgradeById.IsUnlocked())
		{
			upgradeById.Unlock();
			flag = true;
		}
		if (upgradeById2 != null && (flag || !upgradeById2.IsUnlocked()))
		{
			upgradeById2.Unlock();
		}
		GameItem gameItem = SpawnRunningRAS(withDispenser: false);
		StockGrid(MachineRAS.GetTankGrid(gameItem), SpawnFishAtStage(FishSpecies.Tilapia, "MALE", FishStage.Adult), SpawnFishAtStage(FishSpecies.Tilapia, "FEMALE", FishStage.Elder), ItemSpawner.Spawn("industrial_fish_feed"), SpawnFishAtStage(FishSpecies.Tilapia, "FEMALE", FishStage.Adolescent), ItemSpawner.Spawn(FishSpecies.Tilapia.fryIdentifier), SpawnFishAtStage(FishSpecies.Tilapia, "MALE", FishStage.Juvenile), SpawnFishAtStage(FishSpecies.Tilapia, "FEMALE", FishStage.Juvenile), SpawnFishAtStage(FishSpecies.Tilapia, "MALE", FishStage.Juvenile), AquacultureItems.SpawnFertilizedRoe(FishSpecies.Tilapia));
		Add(gameItem);
		GameItem gameItem2 = SpawnRunningRAS(withDispenser: true, 2, 4);
		GameGridInventory tankGrid = MachineRAS.GetTankGrid(gameItem2);
		bool flag2 = true;
		FishStage[] array = new FishStage[4]
		{
			FishStage.Adult,
			FishStage.Elder,
			FishStage.Adolescent,
			FishStage.Juvenile
		};
		foreach (FishStage stage in array)
		{
			FishSpecies[] array2 = new FishSpecies[2]
			{
				FishSpecies.Eel,
				FishSpecies.ElectricEel
			};
			foreach (FishSpecies species in array2)
			{
				StockGrid(tankGrid, SpawnFishAtStage(species, flag2 ? "MALE" : "FEMALE", stage));
				flag2 = !flag2;
			}
		}
		StockGrid(tankGrid, ItemSpawner.Spawn("fish_scrap"), ItemSpawner.Spawn("fish_scrap"));
		StockDispenser(gameItem2);
		Add(gameItem2);
		GameItem gameItem3 = SpawnRunningRAS(withDispenser: true, 1, 4);
		GameGridInventory tankGrid2 = MachineRAS.GetTankGrid(gameItem3);
		array = new FishStage[4]
		{
			FishStage.Juvenile,
			FishStage.Adolescent,
			FishStage.Adult,
			FishStage.Elder
		};
		foreach (FishStage stage2 in array)
		{
			StockGrid(tankGrid2, SpawnFishAtStage(FishSpecies.Sturgeon, "FEMALE", stage2));
		}
		StockGrid(tankGrid2, ItemSpawner.Spawn("fish_scrap"), ItemSpawner.Spawn("fish_scrap"));
		StockDispenser(gameItem3);
		Add(gameItem3);
		AddCopies("ras_water_tank", 2);
		AddCopies(FishSpecies.Tilapia.roeIdentifier, 2);
		Add(AquacultureItems.SpawnFertilizedRoe(FishSpecies.Tilapia));
		Add(AquacultureItems.SpawnFertilizedRoe(FishSpecies.GutterTilapia));
		Add(ItemSpawner.Spawn(FishSpecies.GraftTilapia.roeIdentifier));
		Add(AquacultureItems.SpawnFertilizedRoe(FishSpecies.GraftTilapia));
		AddCopies("sturgeon_roe", 2);
		Add(ItemSpawner.Spawn("kitchen_knife"));
		Add(ItemSpawner.Spawn("surgery_tool"));
		Add(SpawnGravidSturgeon());
		AddCopies("industrial_fish_feed", 2);
		Add(ItemSpawner.Spawn("gutterflow_pellet"));
		GameGridInventory showcaseElement = EmporiumEntry.Instance.showcaseElement;
		string[] array3 = new string[6] { "large_fish_fillet", "fish_fillet", "small_fish_fillet", "small_fish_fillet", "tiny_fish_fillet", "tiny_fish_fillet" };
		for (int i = 0; i < array3.Length; i++)
		{
			GameItem item2 = ItemSpawner.Spawn(array3[i]);
			if (!PlaceLowest(showcaseElement, item2))
			{
				Add(item2);
			}
		}
		AddCopies("tiny_fish_fillet", 2);
		AddCopies("small_fish_fillet", 2);
		AddCopies("fish_fillet", 2);
		Add(ItemSpawner.Spawn("large_fish_fillet"));
		AddCopies("fish_scrap", 3);
		Add(SpawnFishAtStage(FishSpecies.Tilapia, "MALE", FishStage.Adult));
		Add(SpawnFishAtStage(FishSpecies.Tilapia, "FEMALE", FishStage.Adult));
		Add(SpawnFishAtStage(FishSpecies.Tilapia, "FEMALE", FishStage.Adult));
		AddCopies("small_eel_fillet", 2);
		AddCopies("eel_fillet", 2);
		static void Add(GameItem item)
		{
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, item);
		}
		static void AddCopies(string identifier, int count)
		{
			for (int k = 0; k < count; k++)
			{
				Add(ItemSpawner.Spawn(identifier));
			}
		}
	}

	private static GameItem SpawnFishAtStage(FishSpecies species, string sex, FishStage stage)
	{
		return AquacultureItems.SpawnFish(species, sex, species.stageStartDays[(int)stage]);
	}

	private static bool PlaceLowest(GameGridInventory grid, GameItem item)
	{
		if (grid == null || item == null)
		{
			return false;
		}
		for (int num = grid.inventoryShape.height - 1; num >= 0; num--)
		{
			for (int i = 0; i < grid.inventoryShape.width; i++)
			{
				GridShape tempShape = new GridShapeBuilder(item.modifiedShape).SetPosition(i, num).Build();
				SlotMarker slotMarker = grid.TryInventorySlot(item, tempShape);
				if (slotMarker != null && slotMarker.numTransfer > 0 && slotMarker.TryAcceptOnce() > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static void StockGrid(GameGridInventory grid, params GameItem[] items)
	{
		foreach (GameItem item in items)
		{
			GraphUtils.TryAcceptAllOrDestroy(grid, item, "marketing aquaculture kit");
		}
	}

	private static void StockDispenser(GameItem ras)
	{
		GameItem gameItem = MachineRAS.GetDispenserSlot(ras)?.childItem;
		if (gameItem != null)
		{
			StockGrid(MachineFeedDispenser.GetStorageGrid(gameItem), ItemSpawner.Spawn("fish_scrap"), ItemSpawner.Spawn("tiny_fish_fillet"), ItemSpawner.Spawn("tiny_fish_fillet"));
		}
	}

	public static void SpawnMarketingFood()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("energy_drink"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("energy_drink"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("soda_red"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("soda_red"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("processed_meat"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("processed_meat"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("processed_meat"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nutrifruit"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nutrifruit"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("li_eat_snackbar"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cat_bar"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("galaxy_blend"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("red_beer"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("empty_beer_bottle"));
	}

	public static void SpawnMarketingMedical()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("salve"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("bandage_item"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("bandage_item"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("hemostatic_bandage_item"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_dust"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_dust"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_dust"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_dust"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("med_bottle_red"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("med_bottle_violet"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("med_bottle_blue"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("med_bottle_small_pink"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("med_bottle_small_pink"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("med_bottle_orange"));
	}

	public static void SpawnMarketingHousehold()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("toilet_paper"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("toilet_paper"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("pack_condom"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("paper_towel"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("box_tampon"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("newspaper"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("kitchen_knife"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("shampoo"));
	}

	public static void SpawnMarketingMaterials()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("junk"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("junk"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("junk"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("scrap_metal"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("scrap_metal"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("scrap_metal"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("common_electronic"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("common_electronic"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("pipe_weapon"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("common_ore"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("common_ore"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("turbo_booster"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nuts_metal"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nuts_metal"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("printer_plastic"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("printer_plastic"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("printer_plastic"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("crowbar"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wire"));
	}

	public static void SpawnMarketingShady()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("smuggler_bay"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("sec_keycard"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cmd_keycard"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_dust"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("dream_dust"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("fanta_pill"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("fanta_pill"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("combat_machete"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("c4"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("nuclear_waste"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("revolver"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("exchange_directive"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("module_extractor_advanced"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("kitchen_cleaver"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("raw_meat"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("fat_meat"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("blood_bag"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("blood_bag"));
	}

	public static void SpawnBusinessCards()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("joe_card"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("card_brewer"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("card_rev"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("card_mining"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("card_pharma"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("card_chem"));
	}

	public static void SpawnMusicCassettes()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette1"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette2"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette3"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette4"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette5"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette6"));
	}

	public static void SpawnMusicCassettes2()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette8"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette9"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette10"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette11"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette12"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette13"));
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("cassette14"));
	}

	public static void SpawnHypertechItems()
	{
		string[] array = new string[4] { "hypertech_electronic", "energy_credit_hyp", "hypertech_bay", "turbo_booster_hypertech" };
		foreach (string itemID in array)
		{
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn(itemID));
		}
	}

	public static void QueueScamCigaretteSeller()
	{
		PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListStory.CreateStoryCigaretteVendorTrailer());
	}

	public static void QueueCustomGunBuyer()
	{
		PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListGun.CreateStoryGunBuyer());
	}

	public static void QueueInsuranceIntro()
	{
		PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListStory.CreateInsuranceIntroduction());
	}

	public static void QueueWanted4Intro()
	{
		PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListWanted.CreateWanted4());
	}

	public static void QueueLandlord()
	{
		StoreStation.Instance.dayCounter = 7;
		PlayerStore.Instance.dayUntilRent = 0;
		PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientList.CreateLandlordClient());
	}

	public static void QueueTrailerSeller()
	{
		PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientList.CreateTrailerScavGeneral());
	}

	public static void QueueTrailerBuyer()
	{
		PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientList.CreateTrailerSpacerFoodClient());
	}

	public static void QueueTrailerInspection()
	{
		PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientList.CreateTrailerInspectionClient());
	}

	public static void QueueRandomAppraisal()
	{
		PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListAppraisal.CreateAppraisalRandom());
	}

	public static void AddProceduralDebugFunction()
	{
		AddNewDebugAction("Spawn Ribwich", delegate
		{
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRibwichRandom());
		}, Instance.spawnGroup);
		AddNewDebugAction("Spawn Hand Kit", delegate
		{
			for (int num = 0; num < 8; num++)
			{
				GameItem gameItem2 = DirectoryMaster.Item("hand");
				AppHandHelper.InitRandom(gameItem2);
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem2);
			}
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("hand_guide"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("magnifier"));
		}, Instance.spawnGroup);
		AddNewDebugAction("Spawn 9 Ruined Machines (3 per size)", delegate
		{
			SpawnRuinedMachines();
		}, Instance.spawnGroup);
		AddNewDebugAction("Add 100<sprite name=\"c\" tint=1 >", delegate
		{
			Add100Credits();
		}, Instance.spawnGroup);
		AddNewDebugAction("Remove 100<sprite name=\"c\" tint=1 >", delegate
		{
			Remove100Credits();
		}, Instance.spawnGroup);
		AddNewDebugAction("Add 10000<sprite name=\"c\" tint=1 >", delegate
		{
			Add10000Credits();
		}, Instance.spawnGroup);
		AddNewDebugAction("Set Credit to 0", delegate
		{
			SetCreditToZero();
		}, Instance.spawnGroup);
		AddNewDebugAction("Unlock Vending Machines", delegate
		{
			VendingMachineHelper.UnlockVendingMachine();
			VendingFountainHelper.UnlockVendingFountain();
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("common_electronic"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("common_electronic"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("common_electronic"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("common_electronic"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("common_electronic"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("metal_ingot"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("metal_ingot"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("metal_ingot"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.EnergyCreditExt());
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.EnergyCreditExt());
		}, Instance.spawnGroup);
		AddNewDebugAction("Spawn Backpacks & Containers", delegate
		{
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("satchel"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("backpack_small"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("backpack_medium"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("backpack_large"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("smuggler_bay_mod"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("machine_bay_ext"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("storage_bay"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("smuggler_bay"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("machine_bay"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("powerblock"));
		}, Instance.spawnGroup);
		AddNewDebugAction("Open Commissary UI", delegate
		{
			CommissaryUIManager.Instance.OpenUI();
		}, Instance.uiGroup);
		AddNewDebugAction("Add Marketplace Requests", delegate
		{
			StoreRequest.ActivateRequest(StoreRequestList.ULRareItemRequest());
			StoreRequest.ActivateRequest(StoreRequestList.CrowbarSawyer());
			StoreRequest.ActivateRequest(StoreRequestList.ButcherCleaver());
			StoreRequest.ActivateRequest(StoreRequestList.GeorgioToiletPaper());
			StoreRequest.ActivateRequest(StoreRequestList.LandlordSpecial2());
			StoreRequest.ActivateRequest(StoreRequestList.LandlordSpecial());
			StoreRequest.ActivateRequest(StoreRequestList.LLConstruction());
			StoreRequest.ActivateRequest(StoreRequestList.LunRetirementParty());
			StoreRequest.ActivateRequest(StoreRequestList.SawyerBeer());
		}, Instance.cmdGroup);
		AddNewDebugAction("Test Lottery", delegate
		{
			LotteryHelper.OnDrawWiningNumberDebug();
		}, Instance.cmdGroup);
		AddNewDebugAction("Toggle Dev Mode", delegate
		{
			PlayerStore.Instance.devMode = !PlayerStore.Instance.devMode;
			Debug.Log("Dev Mode: " + PlayerStore.Instance.devMode);
		}, Instance.cmdGroup);
		AddNewDebugAction("Set Alcohol Addiction (Active)", delegate
		{
			SetActiveAlcoholAddiction();
		}, Instance.cmdGroup);
		AddNewDebugAction("Simulate Sold Mislabeled Water", delegate
		{
			SimulateSoldMislabeledGhostwater();
		}, Instance.cmdGroup);
		AddNewDebugAction("Gun Kit", SpawnGunKit, Instance.marketingGroup);
		AddNewDebugAction("Farm Kit", SpawnHydroponicKit, Instance.marketingGroup);
		AddNewDebugAction("Plant DNA Kit", SpawnPlantDnaKit, Instance.marketingGroup);
		AddNewDebugAction("Chemistry Kit", SpawnMarketingChemKit, Instance.marketingGroup);
		AddNewDebugAction("Lottery", SpawnLotteryKit, Instance.marketingGroup);
		AddNewDebugAction("Toggle Tooltip", ToggleCinematicMode, Instance.marketingGroup);
		AddNewDebugAction("Add Food Item", SpawnMarketingFood, Instance.marketingGroup);
		AddNewDebugAction("Add Medical Item", SpawnMarketingMedical, Instance.marketingGroup);
		AddNewDebugAction("Add Household Item", SpawnMarketingHousehold, Instance.marketingGroup);
		AddNewDebugAction("Add Material Item", SpawnMarketingMaterials, Instance.marketingGroup);
		AddNewDebugAction("Add Shady Item", SpawnMarketingShady, Instance.marketingGroup);
		AddNewDebugAction("Add Business Card", SpawnBusinessCards, Instance.marketingGroup);
		AddNewDebugAction("Add Music Cassettes", SpawnMusicCassettes, Instance.marketingGroup);
		AddNewDebugAction("Add Music Cassettes (Album 2)", SpawnMusicCassettes2, Instance.marketingGroup);
		AddNewDebugAction("Add Cigarette Inspection Kit", SpawnCigaretteInspectionKit, Instance.marketingGroup);
		AddNewDebugAction("Queue Scam Cigarette Seller", QueueScamCigaretteSeller, Instance.marketingGroup);
		AddNewDebugAction("Queue Custom Gun Buyer", QueueCustomGunBuyer, Instance.marketingGroup);
		AddNewDebugAction("Queue Unique Character 1 Intro", QueueInsuranceIntro, Instance.marketingGroup);
		AddNewDebugAction("Queue Unique Character 2 Intro", QueueWanted4Intro, Instance.marketingGroup);
		AddNewDebugAction("Queue Landlord", QueueLandlord, Instance.marketingGroup);
		AddNewDebugAction("Queue Normal Seller", QueueTrailerSeller, Instance.marketingGroup);
		AddNewDebugAction("Queue Normal Buyer", QueueTrailerBuyer, Instance.marketingGroup);
		AddNewDebugAction("Queue Security Inspection", QueueTrailerInspection, Instance.marketingGroup);
		AddNewDebugAction("Queue Appraisal", QueueRandomAppraisal, Instance.marketingGroup);
		AddNewDebugAction("Set Random Day and Credit", SetRandomDayAndCredit, Instance.marketingGroup);
		AddNewDebugAction("Setup Marketplace Request", SetupMarketplaceRequests, Instance.marketingGroup);
		AddNewDebugAction("Toggle Idle Animation", ToggleIdleAnimation, Instance.marketingGroup);
		AddNewDebugAction("Toggle Customer Sprite Swap", ToggleCustomerSpriteSwap, Instance.marketingGroup);
		AddNewDebugAction("Toggle Aug Sprite Swap", ToggleAugSpriteSwap, Instance.marketingGroup);
		AddNewDebugAction("Test Ribwich", delegate
		{
			PlayerStore.Instance.appraiserLevel = Mathf.Max(PlayerStore.Instance.appraiserLevel, 1);
			PlayerStore.Instance.isAppraisalIntroduced = true;
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("ribwich_guide"));
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListAppraisal.CreateAppraisalRandom());
		}, Instance.cmdGroup);
		AddNewDebugAction("Set All Rep To 0", delegate
		{
			foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
			{
				storeReputation.ModReputation(0.0);
				StoreReputation.UpdateReputation();
			}
		}, Instance.cmdGroup);
		AddNewDebugAction("Log All Owned Items", delegate
		{
			EmporiumEntry.Instance.GetAllOwnedItems().ForEach(delegate(GameItem item)
			{
				Debug.Log(item.name + " - ID: " + item.identifier);
			});
		}, Instance.cmdGroup);
		AddNewDebugAction("Toggle Item Ownership", delegate
		{
			Instance.gameObject.SetActive(value: false);
			DebugOwnershipToggleMode.Activate((Instance.priceDebugTMP != null) ? Instance.priceDebugTMP.font : null);
		}, Instance.cmdGroup);
		AddNewDebugAction("Spawn Module Kit", delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_performance_module"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_efficiency_module"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_quality_module"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_overclock_module"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_eco_module"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_fineness_module"));
		}, Instance.kitGroup);
		AddNewDebugAction("Spawn Module Kit 2", delegate
		{
			for (int j = 0; j < 5; j++)
			{
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("random_efficiency_module"));
			}
			for (int k = 0; k < 5; k++)
			{
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("random_overclock_module"));
			}
			for (int l = 0; l < 5; l++)
			{
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("random_quality_module"));
			}
			for (int m = 0; m < 5; m++)
			{
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("random_fineness_module"));
			}
			for (int n = 0; n < 5; n++)
			{
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("random_node"));
			}
			GameItem item2 = ItemSpawner.Spawn("random_efficiency_module");
			ModuleEffectHelper.AddEffectToModule(item2, "MODULE_EFFECT_OVERVOLTED");
			ModuleEffectHelper.AddEffectToModule(item2, "MODULE_EFFECT_RECEIVER");
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, item2);
			GameItem gameItem = ItemSpawner.Spawn("moisture_farm");
			GameItem item3 = PreBuiltItemHelper.EnergyCreditExt();
			GameItem item4 = ItemSpawner.Spawn("water_jug");
			GraphUtils.TryAcceptAll(gameItem, item3);
			GraphUtils.TryAcceptAll(gameItem, item4);
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem);
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("module_bay_expansion_kit"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("module_bay_expansion_kit"));
		}, Instance.kitGroup);
		AddNewDebugAction("Spawn Rat Kit", delegate
		{
			SpawnRatKit();
		}, Instance.kitGroup);
		AddNewDebugAction("Spawn Aquaculture Kit", delegate
		{
			SpawnAquacultureKit();
		}, Instance.kitGroup);
		AddNewDebugAction("Quick Debug Kit", delegate
		{
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("test_water"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("test_water2"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("test_water3"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("test_water4"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("test_water5"));
		}, Instance.kitGroup);
		AddNewDebugAction("Unlock Shotgun", delegate
		{
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("aug_scanner"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("aug_id_guide"));
			Shotgun.Unlock();
		}, Instance.cmdGroup);
		AddNewDebugAction("Queue Trailer Aug Client", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientList.CreateTrailerAug());
		}, Instance.cmdGroup);
		AddNewDebugAction("Queue Aug T4/T3/T2/T1", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientList.CreateAugMed(1));
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientList.CreateAugMed(2));
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientList.CreateAugMed(3));
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientList.CreateAugMed(4));
		}, Instance.cmdGroup);
		AddNewDebugAction("Guarantee Aug Next Day", delegate
		{
			PlayerStore.Instance.isAugIntroduced = true;
			PlayerStore.Instance.isAugEnded = false;
			if (PlayerStore.Instance.augPresence <= 0)
			{
				PlayerStore.Instance.augPresence = 15;
			}
			PlayerStore.Instance.nextAugSpawnDay = StoreStation.Instance.dayCounter + 1;
		}, Instance.cmdGroup);
		AddNewDebugAction("Queue Shady Inspector", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientList.CreateInspectionClient(1));
		}, Instance.cmdGroup);
		AddNewDebugAction("Queue Security Requisition", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientList.CreateSecRequisitionOfficer());
		}, Instance.cmdGroup);
		AddNewDebugAction("Queue Revolution Quartermaster", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientListRev.CreateRevSupplyBuyerTrailer());
		}, Instance.cmdGroup);
		AddNewDebugAction("Queue UL Contra Buyer", delegate
		{
			PlayerStore.Instance.storeClientManager.AddNextClient(StoreClientList.CreateUpperLowerVisitor());
		}, Instance.cmdGroup);
		AddNewDebugAction("Spawn Wine Kit", delegate
		{
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wine_yeast"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wine_superyeast"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wine_yeast_red"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wine_bottle"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wine_berry"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wine_berry"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wine_berry"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wine_berry"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("water_ration"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("water_ration"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("water_ration"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wine_yeast_infinite"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wine_rack"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("energy_credit_breeder"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("wine_book"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("magnifier"));
			int[] array = new int[6] { 0, 1, 2, 3, 5, 6 };
			foreach (int wineScore in array)
			{
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, WineHelper.CreateFinishedWineForDebug(wineScore));
			}
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, WineHelper.CreateFinishedWineForDebug(0, counterfeit: true));
		}, Instance.spawnGroup);
		AddNewDebugAction("Set Sec Rep to Min", delegate
		{
			foreach (StoreReputation storeReputation2 in PlayerStore.Instance.storeReputations)
			{
				if (storeReputation2.factionId == "FACTION_SECURITY")
				{
					storeReputation2.ModReputation(-400.0);
					StoreReputation.UpdateReputation();
				}
			}
		}, Instance.cmdGroup);
		AddNewDebugAction("Open Sprite Viewer", delegate
		{
			CustomUIDebugWindows.OpenSpriteDictViewer();
		}, Instance.cmdGroup);
		AddNewDebugAction("Open Customer Viewer", delegate
		{
			CustomUIDebugWindows.OpenCustomerDictViewer();
		}, Instance.cmdGroup);
	}

	public static void AddNewDebugAction(string buttonName, Action action, Transform parent)
	{
		GameObject obj = UnityEngine.Object.Instantiate(Instance.debugButtonPrefab, parent);
		obj.GetComponentInChildren<TextMeshProUGUI>().text = buttonName;
		obj.GetComponent<Button>().onClick.AddListener(delegate
		{
			action();
		});
	}
}

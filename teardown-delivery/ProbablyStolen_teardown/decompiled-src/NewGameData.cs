using System;
using System.Collections.Generic;
using UnityEngine;

public class NewGameData : MonoBehaviour
{
	public enum StartType
	{
		NONE,
		WATER_MERCHANT,
		APPRENTICE,
		SCAV,
		FENCE,
		MOONSHINER,
		DRUG_DEALER,
		ORGAN_TRADER,
		PHARMACIST,
		STREET_FOOD_VENDOR,
		FARMER,
		GUNSMITH,
		ROCK_BOTTOM,
		RANCHER,
		MODDED,
		AQUAFARMER
	}

	public StartType startType;

	public string startScenarioId = "";

	public bool skipIntro;

	public bool endlessMode;

	public string difficultyId = "normal";

	public bool hardMode;

	public bool disableTutorialHint;

	public int saveSlot = -1;

	public string saveFile;

	public List<StartingPerk> startingPerks = new List<StartingPerk>();

	public bool isInMainMenu = true;

	public bool isShowcase;

	public bool isLoadFromMainMenu;

	public static NewGameData Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public static void HardReset()
	{
		UnityEngine.Object.Destroy(Instance.gameObject);
		Instance = null;
		Instance = new GameObject("NewSingleton").AddComponent<NewGameData>();
		UnityEngine.Object.DontDestroyOnLoad(Instance.gameObject);
		Instance.isInMainMenu = true;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public static void HandleInitialItem()
	{
		GameInventory invElement = EmporiumEntry.Instance.invElement;
		if (Instance == null)
		{
			return;
		}
		StoreUIManager.Instance.FinishBlackscreenSequence(fast: true);
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.dossier, DirectoryMaster.Item("mentor_contract"));
		if (Instance.startType != StartType.ROCK_BOTTOM && !StartingPerk.IsTapehead())
		{
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.cassettePlayer, DirectoryMaster.Item("cassette3d"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.cassettePlayer, DirectoryMaster.Item("cassette4d"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.cassettePlayer, DirectoryMaster.Item("cassette5d"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.cassettePlayer, DirectoryMaster.Item("cassette6d"));
		}
		if (Instance.startType != 0)
		{
			if (Instance.startType == StartType.WATER_MERCHANT)
			{
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("water_guide"));
				GraphUtils.EmporiumTryAdd(invElement, PreBuiltItemHelper.FullWaterTestStripBox());
				GraphUtils.EmporiumTryAdd(invElement, PreBuiltItemHelper.FullWaterPurificationTabletBox());
				GameItem gameItem = DirectoryMaster.Item("large_bottled_water");
				WaterHelper.FillWithGhostwater(gameItem);
				GraphUtils.EmporiumTryAdd(invElement, gameItem);
				GameItem gameItem2 = DirectoryMaster.Item("water_jug");
				WaterHelper.FillWithRustWaterLowEnd(gameItem2);
				GraphUtils.EmporiumTryAdd(invElement, gameItem2);
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("large_bottled_water"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("bottled_water"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("small_bottled_water"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("water_pitcher"));
				GraphUtils.EmporiumTryAdd(invElement, PreBuiltItemHelper.WaterFilter());
				if (PlayerStore.Instance.skipIntro)
				{
					GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("sign_water"));
				}
				GameItem gameItem3 = ItemFeatureHelper.SetShabby(DirectoryMaster.Item("portable_water_purifier"));
				DurabilityHelper.SetCurrentDurability(gameItem3, 75);
				GraphUtils.EmporiumTryAdd(invElement, gameItem3);
				GameItem gameItem4 = ItemFeatureHelper.SetShabby(DirectoryMaster.Item("bottle_printer"));
				GraphUtils.TryAcceptAll(gameItem4, PreBuiltItemHelper.EnergyCredit(25));
				GraphUtils.EmporiumTryAdd(invElement, gameItem4);
				PlayerStore.Instance.playerCash -= 200;
				PlayerStore.Instance.networkUpgrade["WATER_TRADER"].state = 2;
			}
			else if (Instance.startType == StartType.APPRENTICE)
			{
				PlayerStore.Instance.playerCash -= 50;
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("storage_bay")));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("processed_meat"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("processed_meat"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("processed_juice"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("toothpaste"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("toothpaste"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("toilet_paper"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("toilet_paper"));
				if (PlayerStore.Instance.skipIntro)
				{
					GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("sign_food"));
				}
				if (PlayerStore.Instance.skipIntro)
				{
					GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("sign_household"));
				}
			}
			else if (Instance.startType == StartType.SCAV)
			{
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("scav_token"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("ingot_book"));
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("metal_scanner")));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("flashlight"));
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("satchel")));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("topical_bandage_item"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("topical_bandage_item"));
				GameItem gameItem5 = ItemFeatureHelper.SetShabby(PreBuiltItemHelper.CreateFurnace());
				GraphUtils.TryAcceptAll(gameItem5, PreBuiltItemHelper.EnergyCredit(25));
				GraphUtils.EmporiumTryAdd(invElement, gameItem5);
				if (PlayerStore.Instance.skipIntro)
				{
					GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("sign_material"));
				}
				PlayerStore.Instance.networkUpgrade["RUINED_MACHINE_UNLOCK"].state = 2;
				PlayerStore.Instance.playerCash -= 200;
			}
			else if (Instance.startType == StartType.FENCE)
			{
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("smuggler_bay")));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("dream_dust"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("dream_dust"));
				GraphUtils.EmporiumTryAdd(invElement, PreBuiltItemHelper.CreateHeavyHandmadeGun(ItemFeatureList.EquipmentCondition.Fair));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("heavy_pistol_ammo"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("heavy_pistol_ammo"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("med_keycard"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("eng_keycard"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("sec_keycard"));
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("black_lamp")));
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("black_lamp")));
				PlayerStore.Instance.playerCash -= 150;
				PlayerStore.Instance.startingRent += 50;
				PlayerStore.Instance.rentValue = PlayerStore.Instance.startingRent;
				StoreReputation.ModBMReputation(30);
				StoreReputation.ModSecReputation(-30);
				PlayerStore.Instance.exchangeManager.isExchangeUnlocked = true;
			}
			else if (Instance.startType == StartType.FARMER)
			{
				PlayerStore.Instance.playerCash -= 150;
				PlayerStore.Instance.startingRent += 40;
				PlayerStore.Instance.rentValue = PlayerStore.Instance.startingRent;
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("hydroponic_guide"));
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(PreBuiltItemHelper.CreateOldHydroponic1()));
				GameItem gameItem6 = ItemFeatureHelper.SetShabby(PreBuiltItemHelper.CreateHydroponic());
				GraphUtils.TryAcceptAll(gameItem6, PreBuiltItemHelper.EnergyCredit(100));
				gameItem6.name = "Decrepit Hydroponic System";
				GraphUtils.EmporiumTryAdd(invElement, gameItem6);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("water_ration_small"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("water_ration_small"));
				GameItem gameItem7 = DirectoryMaster.Item("large_bottled_water");
				WaterHelper.FillWithGhostwater(gameItem7);
				GraphUtils.TryAcceptAll(gameItem6, gameItem7);
				GameItem gameItem8 = DirectoryMaster.Item("system_module_eco");
				GraphUtils.TryAcceptAll(gameItem6, gameItem8);
				ModuleHelper.SetModuleStuck(gameItem8);
				GameItem gameItem9 = DirectoryMaster.Item("system_module_eco");
				GraphUtils.TryAcceptAll(gameItem6, gameItem9);
				ModuleHelper.SetModuleStuck(gameItem9);
				GameItem gameItem10 = DirectoryMaster.Item("system_module_ruined");
				GraphUtils.TryAcceptAll(gameItem6, gameItem10);
				ModuleHelper.SetModuleStuck(gameItem10);
				GameItem gameItem11 = DirectoryMaster.Item("system_module_ruined");
				GraphUtils.TryAcceptAll(gameItem6, gameItem11);
				ModuleHelper.SetModuleStuck(gameItem11);
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("nutrifruit_seed"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("nutrifruit_seed"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.FullNutrientTabletBox());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("fillerweed_seed"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("fillerweed_seed"));
				PlayerStore.Instance.networkUpgrade["RETIRED_FARMER"].state = 2;
			}
			else if (Instance.startType == StartType.GUNSMITH)
			{
				PlayerStore.Instance.playerCash -= 450;
				PlayerStore.Instance.startingRent += 50;
				PlayerStore.Instance.rentValue = PlayerStore.Instance.startingRent;
				GraphUtils.EmporiumTryAdd(invElement, PreBuiltItemHelper.CreateOldGunPrinter());
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("sign_firearm"));
				for (int i = 0; i < 4; i++)
				{
					GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("printer_plastic"));
				}
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("storage_bay_gun")));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("small_pistol_ammo_nl"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("small_pistol_ammo_nl"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("small_pistol_ammo_nl"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("small_pistol_ammo_nl"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("small_pistol_ammo_nl"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("small_pistol_ammo_nl"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("small_pistol_ammo"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("small_pistol_ammo"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("small_pistol_ammo"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("small_pistol_ammo"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("small_pistol_ammo"));
				GameItem item = DirectoryMaster.Item("handmade_pistol");
				GunHelper.SetEquipmentCondition(item, ItemFeatureList.EquipmentCondition.Poor);
				GraphUtils.EmporiumTryAdd(invElement, item);
				GraphUtils.EmporiumTryAdd(invElement, PreBuiltItemHelper.CreateHandmadeGun());
				GameItem item2 = DirectoryMaster.Item("printed_gun");
				GunHelper.SetEquipmentCondition(item2, ItemFeatureList.EquipmentCondition.Poor);
				GraphUtils.EmporiumTryAdd(invElement, item2);
				GameItem item3 = DirectoryMaster.Item("printed_gun");
				GunHelper.SetEquipmentCondition(item3, ItemFeatureList.EquipmentCondition.New);
				GraphUtils.EmporiumTryAdd(invElement, item3);
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("silencer"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("rds"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("mag_22"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("mag_22"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("mag_22_high"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("laser_toy"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("silencer_makeshift"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("laser"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("internal_springs_improved"));
				StoreReputation.ModSecReputation(25);
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("screwdriver"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.showcaseElement, DirectoryMaster.Item("permit_gun_1"));
				PermitHelper.GrantedFromPermit(DirectoryMaster.Item("permit_gun_1"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("gun_part"));
				PlayerStore.Instance.networkUpgrade["RETIRED_GUNSMITH"].state = 2;
				PlayerStore.Instance.networkUpgrade["GUN_PERMIT_I"].state = 2;
			}
			else if (Instance.startType == StartType.DRUG_DEALER)
			{
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("blender")));
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("heating_plate")));
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(PreBuiltItemHelper.CreateEvaporator(40)));
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("chem_finisher")));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("1ml_dropper"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("5ml_dropper"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("25ml_cylinder"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("50ml_cylinder"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("100ml_cylinder"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("bottled_water"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("bottled_water"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("coffee_bean"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("water_pitcher"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("kotton_fabric"));
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("chem_scanner")));
				GraphUtils.EmporiumTryAdd(invElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("smuggler_bay_mini")));
				StoreService.GetCartelProtectionService().unlocked = true;
				PlayerStore.Instance.networkUpgrade["RETIRED_CHEMIST"].state = 2;
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("chemistry_guide"));
				StoreReputation.ModSecReputation(-15);
				PlayerStore.Instance.playerCash -= 300;
			}
			else if (Instance.startType == StartType.ROCK_BOTTOM)
			{
				PlayerStore.Instance.playerCash = 0;
			}
			else if (Instance.startType == StartType.RANCHER)
			{
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, AnimalCage.SpawnCageWithDispenser(shabby: true));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRat("MALE", 5, defaultStats: true));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRat("FEMALE", 5, defaultStats: true));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRat("MALE", 3, defaultStats: true));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRat("FEMALE", 3, defaultStats: true));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRat("MALE", 1, defaultStats: true));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateRat("FEMALE", 1, defaultStats: true));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("kitchen_cleaver"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("labeler_blue"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, ItemSpawner.Spawn("marking_pen"));
				GameItem item4 = DirectoryMaster.Item("serum_green");
				UseCountHelper.SetCurrentUseCount(item4, 2);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, item4);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateNutrifruitSmall());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateNutrifruitSmall());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateNutrifruitSmall());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateNutrifruitSmall());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateNutrifruitSmall());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, PreBuiltItemHelper.CreateNutrifruitSmall());
				PlayerStore.Instance.networkUpgrade["RETIRED_RANCHER"].state = 2;
				AchievementTracker.Instance.CheckRats();
			}
			else if (Instance.startType == StartType.AQUAFARMER)
			{
				PlayerStore.Instance.playerCash -= 450;
				PlayerStore.Instance.startingRent += 40;
				PlayerStore.Instance.rentValue = PlayerStore.Instance.startingRent;
				for (int j = 0; j < 2; j++)
				{
					GameItem gameItem12 = ItemFeatureHelper.SetShabby(ItemSpawner.Spawn("ras"));
					GraphUtils.TryAcceptAll(gameItem12, ItemSpawner.Spawn("ras_water_tank"));
					GraphUtils.TryAcceptAll(gameItem12, PreBuiltItemHelper.EnergyCredit(50));
					GraphUtils.EmporiumTryAdd(invElement, gameItem12);
				}
				int age = FishSpecies.Tilapia.stageStartDays[4];
				GraphUtils.EmporiumTryAdd(invElement, AquacultureItems.SpawnFish(FishSpecies.Tilapia, "MALE", age));
				GraphUtils.EmporiumTryAdd(invElement, AquacultureItems.SpawnFish(FishSpecies.Tilapia, "FEMALE", age));
				GraphUtils.EmporiumTryAdd(invElement, AquacultureItems.SpawnFertilizedRoe(FishSpecies.Tilapia));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("industrial_fish_feed"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("industrial_fish_feed"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("scrap_metal"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("common_electronic"));
				GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("kitchen_knife"));
				GraphUtils.EmporiumTryAdd(invElement, ItemSpawner.Spawn("marking_pen"));
				for (int k = 0; k < 2; k++)
				{
					GameItem gameItem13 = DirectoryMaster.Item("water_jug");
					WaterHelper.FillWithGhostwater(gameItem13);
					GraphUtils.EmporiumTryAdd(invElement, gameItem13);
				}
				PlayerStore.Instance.networkUpgrade["AQUACULTURE"].state = 2;
			}
			else if (Instance.startType == StartType.MODDED)
			{
				StartingScenario startingScenario = StartingScenario.Get(Instance.startScenarioId);
				if (startingScenario == null)
				{
					Debug.LogWarning("[NewGameData] Mod scenario '" + Instance.startScenarioId + "' is not registered; starting with the base store");
				}
				else if (startingScenario.onStart != null)
				{
					try
					{
						startingScenario.onStart();
					}
					catch (Exception ex)
					{
						Debug.LogError("[NewGameData] onStart of scenario '" + startingScenario.id + "' threw: " + ex);
					}
				}
			}
		}
		if (StartingPerk.IsSentinel())
		{
			GraphUtils.EmporiumTryAdd(invElement, PreBuiltItemHelper.CreateRealGenuineInjector());
			GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("injector_guide"));
			GraphUtils.EmporiumTryAdd(invElement, IntelHelper.CreateSentinel());
			GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("magnifier"));
			PlayerStore.Instance.isInjectorIntroduced = true;
		}
		if (StartingPerk.IsGamblingAddiction())
		{
			GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("scratcher"));
		}
		StartingPerkContent.InitWelfareProgram();
		if (StartingPerk.IsAlcoholAddiction())
		{
			GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("nudka"));
		}
		if (StartingPerk.IsNicotineAddiction())
		{
			GraphUtils.EmporiumTryAdd(invElement, PreBuiltItemHelper.CounterfeitCigarette());
		}
		if (StartingPerk.IsNarcoticAddiction())
		{
			GraphUtils.EmporiumTryAdd(invElement, DirectoryMaster.Item("pink_injector"));
		}
		ModHook.FireOnNewGameStarted(StartingScenario.ResolveId(Instance.startType, Instance.startScenarioId));
	}

	public static string GetStartDisplayName(StartType startType)
	{
		return GetStartDisplayName(startType, (PlayerStore.Instance != null) ? PlayerStore.Instance.startScenarioId : null);
	}

	public static string GetStartDisplayName(StartType startType, string startScenarioId)
	{
		switch (startType)
		{
		case StartType.MODDED:
		{
			StartingScenario startingScenario = StartingScenario.Get(startScenarioId);
			if (startingScenario == null)
			{
				return LocHelper.GetLocalizedNewGame("unknown_start_type");
			}
			return startingScenario.GetTitle();
		}
		case StartType.WATER_MERCHANT:
			return LocHelper.GetLocalizedNewGame("water_merchant");
		case StartType.APPRENTICE:
			return LocHelper.GetLocalizedNewGame("generalist");
		case StartType.SCAV:
			return LocHelper.GetLocalizedNewGame("junker");
		case StartType.FENCE:
			return LocHelper.GetLocalizedNewGame("fence");
		case StartType.MOONSHINER:
			return LocHelper.GetLocalizedNewGame("moonshiner");
		case StartType.DRUG_DEALER:
			return LocHelper.GetLocalizedNewGame("chemist");
		case StartType.ORGAN_TRADER:
			return LocHelper.GetLocalizedNewGame("organ_trader");
		case StartType.PHARMACIST:
			return LocHelper.GetLocalizedNewGame("pharmacist");
		case StartType.STREET_FOOD_VENDOR:
			return LocHelper.GetLocalizedNewGame("street_food_vendor");
		case StartType.FARMER:
			return LocHelper.GetLocalizedNewGame("hydroponic_farmer");
		case StartType.GUNSMITH:
			return LocHelper.GetLocalizedNewGame("gunsmith");
		case StartType.ROCK_BOTTOM:
			return LocHelper.GetLocalizedNewGame("rock_bottom");
		case StartType.RANCHER:
			return LocHelper.GetLocalizedNewGame("rancher");
		case StartType.AQUAFARMER:
			return LocHelper.GetLocalizedOrDefault("NewGame", "aquafarmer", "Aquafarmer");
		default:
			return LocHelper.GetLocalizedNewGame("unknown_start_type");
		}
	}
}

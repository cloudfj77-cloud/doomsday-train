using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerStore
{
	public enum StoreState
	{
		CLOSED,
		OPEN,
		MORNING
	}

	public enum PlayerLocation
	{
		IN_STORE,
		INVENTOR
	}

	private static PlayerStore instance = null;

	private static readonly object padlock = new object();

	[ES3Serializable]
	public string storeName;

	[ES3Serializable]
	public string playerName;

	[ES3Serializable]
	public int playerCash;

	[ES3Serializable]
	public int wildFavor;

	[ES3Serializable]
	public int saveSlotId = -1;

	[ES3Serializable]
	public Dictionary<string, int> storeUIZoomLevels = new Dictionary<string, int>();

	[ES3Serializable]
	public Dictionary<string, AfterhourLocationState> afterhourState = new Dictionary<string, AfterhourLocationState>();

	[ES3Serializable]
	public Dictionary<string, string> modData = new Dictionary<string, string>();

	[ES3Serializable]
	public RunStats runStats = new RunStats();

	[ES3Serializable]
	public string runID = "";

	[ES3Serializable]
	public int runNumber;

	[ES3Serializable]
	public int machineryBoughtCount;

	[ES3Serializable]
	public float runPlayTimeSeconds;

	private float sessionStartTime;

	public float totalPlayTimeSeconds;

	[ES3Serializable]
	public DateTime lastSaveTime;

	[ES3Serializable]
	public int dayUntilRent;

	[ES3Serializable]
	public int rentValue;

	[ES3Serializable]
	public int dayUntilLoan;

	[ES3Serializable]
	public int loanValue;

	[ES3Serializable]
	public int temporaryRent;

	[ES3Serializable]
	public int startingRent = 200;

	public const int STARTING_LOAN = 200;

	public const int DAY_IN_WEEK = 7;

	public const int STARTING_CASH = 600;

	[ES3Serializable]
	public int retailMarkup = 15;

	[ES3Serializable]
	public int contrabandMarkupLow = 35;

	[ES3Serializable]
	public int contrabandMarkupMid = 70;

	[ES3Serializable]
	public int contrabandMarkupHigh = 100;

	[ES3Serializable]
	public int contrabandMarkupCritial = 150;

	public const int CONTRABAND_MARKUP_CRITICAL = 150;

	[ES3Serializable]
	public StoreClientManager storeClientManager;

	[ES3Serializable]
	public int baseStoreAttractiveness = 250;

	[ES3Serializable]
	public int projectorAttractivenessBonus;

	[ES3Serializable]
	public int dealMakerBargainBonus;

	[ES3Serializable]
	public bool dealMakerActivatedThisCustomer;

	[ES3Serializable]
	public int garbageLevel;

	[ES3Serializable]
	public bool isPowerOn = true;

	[ES3Serializable]
	public int dayWithoutInsurance;

	[ES3Serializable]
	public int lastInspectionLossDay = -1;

	[ES3Serializable]
	public int lastInspectionLossValue;

	[ES3Serializable]
	public int lastTheftDay = -1;

	[ES3Serializable]
	public int lastTheftValue;

	[ES3Serializable]
	public int foreclosePercentage = 50;

	[ES3Serializable]
	public int contrabandFinePercentage = 25;

	[ES3Serializable]
	public List<string> futurStoreClientIdQueue;

	[ES3Serializable]
	public List<int> futurStoreClientDayQueue;

	[ES3NonSerializable]
	public StoreClientInstance currentClientInstance;

	[ES3NonSerializable]
	public GameItem currentNegociatedItem;

	[ES3NonSerializable]
	public StoreClientInstance lastVisitedClient;

	[ES3NonSerializable]
	public List<Action> nightlyAction;

	[ES3NonSerializable]
	private GameGridInventory gridInv;

	public const string BOUGHT_PRICE_TAG = "BOUGHT_PRICE_TAG";

	[ES3Serializable]
	public List<string> nightLogs;

	[ES3Serializable]
	public Queue<string> dialogHistory;

	[ES3Serializable]
	public List<StoreReputation> storeReputations;

	[ES3NonSerializable]
	public int startingDay = 1;

	[ES3Serializable]
	public bool tutorialEnable = true;

	[ES3Serializable]
	public bool bargainTutorialPlayed;

	[ES3Serializable]
	public bool enableStoryClient = true;

	[ES3NonSerializable]
	public bool cinematicMode;

	[ES3Serializable]
	public bool skipIntro;

	[ES3Serializable]
	public bool endlessMode;

	[ES3NonSerializable]
	public bool devMode;

	[ES3NonSerializable]
	public bool isProduction = true;

	[ES3NonSerializable]
	public const bool IS_LIMITED_BUILD = false;

	public const bool DEV_CONSOLE_FORCE_AVAILABLE = false;

	public const string VERSION_NUMBER = "VERSION 100-Rev12";

	public static readonly string VERSION = "VERSION 100-Rev12";

	public const string BUILD_TYPE_INTERNAL_PLAYTEST = "INTERNAL_PLAYTEST";

	public const string BUILD_TYPE_PUBLIC_PLAYTEST = "PUBLIC_PLAYTEST";

	public const string BUILD_TYPE = "INTERNAL_PLAYTEST";

	public static readonly NewGameData.StartType[] LIMITED_BUILD_START_TYPES = new NewGameData.StartType[3]
	{
		NewGameData.StartType.APPRENTICE,
		NewGameData.StartType.RANCHER,
		NewGameData.StartType.ROCK_BOTTOM
	};

	[ES3Serializable]
	public string saveVersion = "";

	public const int newspaperUnlockDay = 8;

	[ES3Serializable]
	public bool isNewspaperUnlocked;

	[ES3Serializable]
	public bool isVisualInspectionIntroduced;

	[ES3Serializable]
	public bool isDisplayCaseIntroduced;

	[ES3Serializable]
	public bool displayCaseTimingTipShown;

	[ES3Serializable]
	public bool isContrabandIntroduced;

	[ES3Serializable]
	public int animalsButcheredTotal;

	public int hopperLabelCounter;

	[ES3Serializable]
	public bool isSkinningKnifeRewardGiven;

	[ES3Serializable]
	public int scratchersStartedTotal;

	[ES3Serializable]
	public int scratchersFinishedTotal;

	[ES3Serializable]
	public int scratcherWinningsTotal;

	[ES3Serializable]
	public bool isGamblersCoinRewardGiven;

	[ES3Serializable]
	public bool isStolenItemIntroduced;

	[ES3Serializable]
	public bool isStampIntroduced;

	[ES3Serializable]
	public bool isInjectorIntroduced;

	[ES3Serializable]
	public bool isAppraisalIntroduced;

	[ES3Serializable]
	public bool isHandIntroduced;

	[ES3Serializable]
	public bool isFuelIntroduced;

	[ES3Serializable]
	public bool isRibwichIntroduced;

	[ES3Serializable]
	public int appraisalIntroducedDay;

	[ES3Serializable]
	public int appraiserLevel;

	[ES3Serializable]
	public double appraisalProgress;

	[ES3Serializable]
	public bool appraisalPromotionDeclined;

	[ES3Serializable]
	public int appraisalPromotionDay = -1;

	[ES3Serializable]
	public List<string> appraisalBlacklist = new List<string>();

	[ES3Serializable]
	public bool isBargainIntroduced;

	[ES3Serializable]
	public Dictionary<string, NetworkUpgrade> networkUpgrade;

	[ES3Serializable]
	public List<StoreService> storeService;

	[ES3Serializable]
	public Dictionary<long, StorePhoneClient> PhoneClientDict;

	[ES3NonSerializable]
	public bool correctReportFileToday;

	[ES3NonSerializable]
	public bool wantedPersonVisitedToday;

	[ES3NonSerializable]
	public string wantedPersonVisitedName = "";

	[ES3NonSerializable]
	public string wantedPersonVisitedId = "";

	[ES3Serializable]
	public bool isSecurityAlarmActive;

	[ES3Serializable]
	public List<string> unseenMinorClient;

	[ES3NonSerializable]
	public Dictionary<string, Func<StoreClient>> unseenMinorDict;

	[ES3Serializable]
	public CommissaryData commissaryData;

	[ES3Serializable]
	public int wrongfulReportCount;

	[ES3Serializable]
	public int wrongfulReportMax = 5;

	[ES3NonSerializable]
	public Action onSecAlarmLeftAction;

	[ES3Serializable]
	public Dictionary<string, int> wantedStatus;

	[ES3NonSerializable]
	public bool isClientArrived;

	[ES3NonSerializable]
	public bool isClientBeingArrested;

	[ES3NonSerializable]
	public bool isHandlingShootout;

	[ES3Serializable]
	public int odinBudget = 350;

	[ES3Serializable]
	public bool IsPropertyPaid;

	[ES3Serializable]
	public int mortgageWeekLeft;

	[ES3Serializable]
	public bool isPayingMortgage;

	public const int LEGACY_DOWN_PAYMENT = 3000;

	public const int LEGACY_MORTGAGE_WEEK_COUNT = 14;

	public const int BASE_COST = 12000;

	[ES3Serializable]
	public int mortgageWeeklyPayment;

	[ES3Serializable]
	public int mortgageDownPaid;

	[ES3Serializable]
	public int mortgageTotalWeeks;

	[ES3Serializable]
	public int mortgageInterestRateTaken;

	[ES3Serializable]
	public bool IsMortgageOfferDeclined;

	[ES3Serializable]
	public int propertyBoughtAt;

	[ES3Serializable]
	public int loanOfferedToGeorgio;

	[ES3Serializable]
	public int dayWhenLoanOfferedGeorgio;

	[ES3Serializable]
	public ExchangeManager exchangeManager;

	[ES3Serializable]
	public StoreClientData storeClientData;

	[ES3Serializable]
	public bool isHardMode;

	[ES3Serializable]
	public string difficultyId = "";

	[ES3Serializable]
	public bool soldStolenOrContrabandThisRun;

	[ES3Serializable]
	public Dictionary<string, string> itemDisplayNameCache;

	[ES3Serializable]
	public List<(string, int)> ActivePermits;

	[ES3Serializable]
	public int cartelRequiredCount = 10;

	[ES3Serializable]
	public int cartelRequiredPurity = 40;

	[ES3Serializable]
	public int cartelExitStage;

	[ES3Serializable]
	public bool isCartelExited;

	[ES3Serializable]
	public int visitLeftTonight;

	[ES3Serializable]
	public int lastAfterhourOutingDay = -1;

	[ES3Serializable]
	public PlayerLocation playerLocation;

	[ES3Serializable]
	public int scavengingAttempts;

	[ES3Serializable]
	public int dumpingGroundMajorWoundImmunitySearchesLeft;

	[ES3Serializable]
	public int dumpingGroundMinorWoundImmunitySearchesLeft;

	[ES3Serializable]
	public bool dumpingGroundSatchelFound;

	[ES3Serializable]
	public bool dumpingGroundKidsBackpackFound;

	[ES3Serializable]
	public List<SoldItem> soldItemsTracker;

	[ES3Serializable]
	public bool isVandalized;

	[ES3Serializable]
	public int daySinceLastVandalism;

	[ES3Serializable]
	public List<string> seenRequest;

	[ES3Serializable]
	public Dictionary<int, List<string>> takenLotteryNumbers;

	[ES3Serializable]
	public Dictionary<int, Vector2> contentWindowPositions = new Dictionary<int, Vector2>();

	[ES3Serializable]
	public string currentWinningLotteryNumber = "";

	[ES3Serializable]
	public int currentLotteryEdition = 1405;

	[ES3Serializable]
	public ExpeditionData expeditionData;

	[ES3Serializable]
	public bool isVendingMachineUnlocked;

	public int vendingMislabelCount;

	public int vendingMislabelFraudValue;

	public int vendingMislabelEndangermentValue;

	public double vendingMislabelRepLoss;

	public double fountainMislabelRepLossLL;

	public double fountainMislabelRepLossUL;

	public int requestMislabelCount;

	public int requestMislabelFraudValue;

	public int requestMislabelEndangermentValue;

	public Dictionary<string, double> requestMislabelRepLoss = new Dictionary<string, double>();

	[ES3Serializable]
	public bool isVendingFountainUnlocked;

	private const int WRONGFUL_REPORT_FINE = 50;

	[ES3Serializable]
	public bool isShootingUnlocked;

	[ES3Serializable]
	public bool isGeneScannerScanning;

	public const int APPRAISAL_JOIN_COST = 85;

	public const int DAY_UNTIL_MODULE_TRAIT = 21;

	public int daySinceLastBrowseUpgrade;

	public bool isNetworkIntroduced;

	public int networkIntroducedDay = -1;

	public bool networkReminderShown;

	public int displayCaseSetupMissedDays;

	public bool displayCaseReminderShown;

	[ES3Serializable]
	public StoreState storeState;

	[ES3NonSerializable]
	public bool callNextWhenPossible;

	[ES3NonSerializable]
	public int lockAllFlag;

	[ES3NonSerializable]
	public int lockClientClickFlag;

	[ES3NonSerializable]
	public int lockStorePhysicalFlag;

	[ES3NonSerializable]
	public int lockAllInventoryFlag;

	[ES3NonSerializable]
	public int lockNextBellFlag;

	[ES3NonSerializable]
	public int lockShutterButtonFlag;

	[ES3Serializable]
	public List<StoreRequest> storeRequests;

	[ES3Serializable]
	public SecData secData;

	[ES3Serializable]
	public HealthData healthData;

	[ES3Serializable]
	public bool isSecurityRecordIntroduced;

	[ES3Serializable]
	public bool isBrokenMachineIntroduced;

	[ES3Serializable]
	public int currentUniqueId = 1;

	[ES3Serializable]
	public string mainInvJSON;

	[ES3Serializable]
	public string backInvJSON;

	[ES3Serializable]
	public string trashcanInvJSON;

	[ES3Serializable]
	public string docInvJSON;

	[ES3Serializable]
	public string cassettePlayerInvJSON;

	[ES3Serializable]
	public string showcaseInvJSON;

	[ES3Serializable]
	public string soldInvJSON;

	[ES3Serializable]
	public string hirelingInvJSON;

	[ES3Serializable]
	public string hiddenInvJSON;

	[ES3Serializable]
	public string vendingMachineInvJSON;

	[ES3Serializable]
	public string vendingFountainInvJSON;

	[ES3Serializable]
	public bool storePosterIsPulled;

	[ES3Serializable]
	public NewGameData.StartType startType;

	[ES3Serializable]
	public string startScenarioId = "";

	[ES3Serializable]
	public string startLocale = "";

	[ES3Serializable]
	public List<ItemFeature> savedItemFeatureList;

	[ES3Serializable]
	public List<StartingPerk> startingPerks;

	[ES3Serializable]
	public int marketplaceUnlockedDay = -1;

	[ES3Serializable]
	public bool hasBusinessPermit;

	public const int BUSINESS_PERMIT_STARTING_DAY = 24;

	public int augPresence;

	public const int DEFAULT_AUG_PRESENCE = 15;

	public bool isAugIntroduced;

	public const int AUG_INTRODUCED_DAY = 12;

	public bool isAugEnded;

	public int nextAugSpawnDay = -1;

	public const int AUG_ALL_CLEAR_DAYS = 50;

	public const int AUG_ALL_CLEAR_PERMIT_DAYS = 25;

	public bool isAfterhoursIntroduced;

	private Dictionary<string, GameItem> saveBags = new Dictionary<string, GameItem>();

	public const int SKIP_INTRO_BURGLARY_GRACE_DAYS = 3;

	public const int PROPERTY_INCREASE_START_DAY = 50;

	public const int SKINNING_KNIFE_BUTCHER_COUNT = 100;

	public static bool HasInstance => instance != null;

	public static PlayerStore Instance
	{
		get
		{
			lock (padlock)
			{
				if (instance == null)
				{
					Debug.Log("A new PlayerStore has been created");
					instance = new PlayerStore();
				}
				return instance;
			}
		}
	}

	public AfterhourLocationState GetAfterhourState(string locationId)
	{
		if (afterhourState == null)
		{
			afterhourState = new Dictionary<string, AfterhourLocationState>();
		}
		if (!afterhourState.TryGetValue(locationId, out var value) || value == null)
		{
			value = new AfterhourLocationState();
			afterhourState[locationId] = value;
		}
		return value;
	}

	public Dictionary<string, string> ModData()
	{
		if (modData == null)
		{
			modData = new Dictionary<string, string>();
		}
		return modData;
	}

	public static bool IsStartTypeInLimitedBuild(NewGameData.StartType startType)
	{
		return Array.IndexOf(LIMITED_BUILD_START_TYPES, startType) >= 0;
	}

	public static int GetBusinessPermitPrice()
	{
		return Difficulty.Rules.vendorPermitFee;
	}

	public static int GetAugIntroducedDay()
	{
		if (!StartingPerk.IsAugMenace())
		{
			return 12;
		}
		return 1;
	}

	public bool IsAugAllClearDue(int day)
	{
		if (StartingPerk.IsAugMenace())
		{
			return false;
		}
		if (day < GetAugIntroducedDay() + 50 || augPresence > 0)
		{
			return false;
		}
		if (!hasBusinessPermit)
		{
			return false;
		}
		int num = ((runStats != null) ? runStats.permitBoughtDay : (-1));
		if (num >= 0)
		{
			return day >= num + 25;
		}
		return true;
	}

	public int GetUniqueID()
	{
		currentUniqueId++;
		return currentUniqueId;
	}

	public PlayerStore()
	{
		NewGameData.Instance.isInMainMenu = false;
		Reset();
		commissaryData = new CommissaryData();
		nightLogs = new List<string>();
		storeClientManager = new StoreClientManager();
		futurStoreClientIdQueue = new List<string>();
		futurStoreClientDayQueue = new List<int>();
		gridInv = new GameGridInventory(10, 10);
		dialogHistory = new Queue<string>();
		nightlyAction = new List<Action>();
		networkUpgrade = new Dictionary<string, NetworkUpgrade>();
		storeService = new List<StoreService>();
		PhoneClientDict = new Dictionary<long, StorePhoneClient>();
		storeRequests = new List<StoreRequest>();
		secData = new SecData();
		healthData = new HealthData();
		wantedStatus = new Dictionary<string, int>();
		exchangeManager = new ExchangeManager();
		startingPerks = new List<StartingPerk>();
		itemDisplayNameCache = new Dictionary<string, string>();
		ActivePermits = new List<(string, int)>();
		soldItemsTracker = new List<SoldItem>();
		seenRequest = new List<string>();
		takenLotteryNumbers = new Dictionary<int, List<string>>();
		expeditionData = new ExpeditionData();
	}

	public void StartNewGame()
	{
		Reset();
		soldStolenOrContrabandThisRun = false;
		runStats = new RunStats();
		if (NewGameData.Instance != null)
		{
			difficultyId = Difficulty.ResolveId(NewGameData.Instance.difficultyId, NewGameData.Instance.hardMode);
			isHardMode = Difficulty.IsHardOrAbove();
			startType = NewGameData.Instance.startType;
			startScenarioId = NewGameData.Instance.startScenarioId ?? "";
		}
		TutorialUIManager.Instance.TutorialStart();
		StoreClientListMinor.InitMinorClientList();
		StoreStation.Instance.dayCounter = startingDay;
		StoreBookData.InitBookDatas();
		PhoneClientDict = StorePhoneClient.InitPhoneClientDict();
		PriceHelper.InitDict();
		StoreReputation.InitReputation();
		secData = new SecData();
		healthData = new HealthData();
		networkUpgrade = NetworkUpgrade.InitUpgradeDict();
		storeService = StoreService.InitService();
		StoreService.ApplyDifficultyStart();
		ModHook.FireOnDifficultyApplied(Difficulty.Current.id);
		rentValue = startingRent;
		loanValue = 200;
		dayUntilRent = 6;
		dayUntilLoan = 12;
		playerCash = 600;
		storeState = StoreState.MORNING;
		storeClientData = new StoreClientData();
		exchangeManager.ResetPromotional();
		saveVersion = VERSION;
		startLocale = LocHelper.GetCurrentLocaleCode();
		augPresence = 15;
		DebugInit();
		EmporiumEntry.Instance.frontInvWindow.Hide();
		if (NewGameData.Instance == null)
		{
			Debug.Log("Started Without The Main Menu");
		}
		else
		{
			skipIntro = NewGameData.Instance.skipIntro;
			endlessMode = NewGameData.Instance.endlessMode;
			Debug.Log("Started From The Main Menu as " + NewGameData.Instance.startType.ToString() + " with skipIntro=" + skipIntro + " endlessMode=" + endlessMode);
			if (!skipIntro)
			{
				dumpingGroundMajorWoundImmunitySearchesLeft = 20;
				dumpingGroundMinorWoundImmunitySearchesLeft = 10;
			}
			startingPerks = NewGameData.Instance.startingPerks;
			if (StartingPerk.IsMinimalist() || StartingPerk.IsAscetic())
			{
				EmporiumEntry.Instance.SetSmallInv();
			}
		}
		healthData.InitStartingAddictionWithdrawal();
		EmporiumOption.Instance.OnArriveGameScene();
		totalPlayTimeSeconds = SaveFiles.IndexLoad("totalPlayTimeSeconds", 0f);
		sessionStartTime = Time.realtimeSinceStartup;
		StoreVendingFountain.Instance.gameObject.SetActive(value: false);
		StoreVendingMachine.Instance.gameObject.SetActive(value: false);
	}

	public void InitialSave()
	{
		int item = (saveSlotId = SaveFiles.NextFreeSlotId());
		runID = Guid.NewGuid().ToString();
		int value = SaveFiles.IndexLoad("totalRunCount", 0) + 1;
		SaveFiles.IndexSave("totalRunCount", value);
		runNumber = value;
		List<int> list = SaveFiles.IndexLoad("activeSaveSlotId", new List<int>());
		if (!list.Contains(item))
		{
			list.Add(item);
		}
		SaveFiles.IndexSave("activeSaveSlotId", list);
		SaveGame();
		EmporiumAnalytics.TrackNewGame();
	}

	public void SaveGame()
	{
		savedItemFeatureList = new List<ItemFeature>();
		mainInvJSON = EncodeSaveItem("save_bag", EmporiumEntry.Instance.invElement, EmporiumEntry.Instance.invWindow);
		backInvJSON = EncodeSaveItem("back_inv_save_bag", EmporiumEntry.Instance.backInvinvElement, EmporiumEntry.Instance.backInvWindow);
		hiddenInvJSON = EncodeSaveItem("hidden_save_bag", EmporiumEntry.Instance.hiddenElement, EmporiumEntry.Instance.hiddenWindow);
		showcaseInvJSON = EncodeSaveItem("showcase_save_bag", EmporiumEntry.Instance.showcaseElement, EmporiumEntry.Instance.showcaseWindow);
		soldInvJSON = EncodeSaveItem("sold_save_bag", EmporiumEntry.Instance.soldElement, null);
		hirelingInvJSON = EncodeSaveItem("save_bag", EmporiumEntry.Instance.hirelingInv, EmporiumEntry.Instance.hirelingWindow);
		vendingMachineInvJSON = EncodeSaveItem("save_bag", EmporiumEntry.Instance.vendingMachineElement, EmporiumEntry.Instance.vendingMachineWindow);
		vendingFountainInvJSON = EncodeSaveItem("save_bag", EmporiumEntry.Instance.vendingFountainElement, EmporiumEntry.Instance.vendingFountainWindow);
		trashcanInvJSON = EncodeItem(EmporiumEntry.Instance.trashcan);
		docInvJSON = EncodeItem(EmporiumEntry.Instance.dossier);
		cassettePlayerInvJSON = EncodeSaveItem("save_bag", EmporiumEntry.Instance.cassettePlayerElement, EmporiumEntry.Instance.cassettePlayerWindow);
		StorePoster.Instance.SaveStorePoster();
		float num = Time.realtimeSinceStartup - sessionStartTime;
		sessionStartTime = Time.realtimeSinceStartup;
		runPlayTimeSeconds += num;
		totalPlayTimeSeconds += num;
		if (WindowsHandler.current != null && WindowsHandler.current.visibleWindows != null)
		{
			foreach (PixelWindow visibleWindow in WindowsHandler.current.visibleWindows)
			{
				if (visibleWindow == null)
				{
					continue;
				}
				foreach (GameItem parentItem in visibleWindow.parentItems)
				{
					parentItem.RememberContentWindowPositionIfMine(visibleWindow);
				}
			}
		}
		string rotated = null;
		try
		{
			SaveFiles.WithIORetry(delegate
			{
				if (rotated == null)
				{
					rotated = SaveFiles.RotateIntoHistory(saveSlotId);
				}
				WriteSlotFile();
			});
		}
		catch (IOException ex)
		{
			Debug.LogError($"[SaveGame] Slot file {saveSlotId} stayed locked, save skipped this cycle: {ex.Message}");
			SaveFiles.RestoreFromHistory(saveSlotId, rotated);
		}
		catch (Exception ex2)
		{
			Debug.LogError($"[SaveGame] Slot file {saveSlotId} unreadable ({ex2.Message}). Quarantining and rewriting.");
			if (SaveFiles.QuarantineFile(SaveFiles.Slot(saveSlotId)))
			{
				WriteSlotFile();
			}
			else
			{
				Debug.LogError($"[SaveGame] Could not quarantine slot {saveSlotId}, save skipped this cycle.");
				SaveFiles.RestoreFromHistory(saveSlotId, rotated);
			}
		}
		SaveFiles.IndexSave("totalPlayTimeSeconds", totalPlayTimeSeconds);
		SavePreviewData savePreviewData = SavePreviewData.GenerateData();
		SaveFiles.IndexSave("preview_" + saveSlotId, savePreviewData);
		SaveUIManager.UpdateCacheEntry(saveSlotId, savePreviewData);
		Debug.Log("Game Saved in Slot: " + saveSlotId);
	}

	private void WriteSlotFile()
	{
		using ES3Writer eS3Writer = ES3Writer.Create(new ES3Settings(SaveFiles.Slot(saveSlotId)));
		eS3Writer.Write<PlayerStore>("playerStore", this);
		eS3Writer.Write<StoreStation>("storeStation", StoreStation.Instance);
		eS3Writer.Save();
	}

	private GameItem GetSaveBag(string id)
	{
		if (saveBags.TryGetValue(id, out var value) && value is GameItemElement gameItemElement && !gameItemElement.IsDestroyed())
		{
			return value;
		}
		value = DirectoryMaster.Item(id);
		saveBags[id] = value;
		return value;
	}

	public string EncodeSaveItem(string id, PixelElement pixelElement, PixelWindow pixelWindow)
	{
		GameItem saveBag = GetSaveBag(id);
		saveBag.contentWindow.Attach(pixelElement);
		string result = JsonUtility.ToJson(new SaveState
		{
			saveItems = SaveManager.EncodeNodes(saveBag)
		});
		pixelWindow?.Attach(pixelElement);
		return result;
	}

	public string EncodeItem(GameItem gameItem)
	{
		return JsonUtility.ToJson(new SaveState
		{
			saveItems = SaveManager.EncodeNodes(gameItem)
		});
	}

	public void LoadGame()
	{
		saveSlotId = NewGameData.Instance.saveSlot;
		string loadFile = SaveFiles.ResolveLoadFile(NewGameData.Instance);
		if (!ES3.FileExists(loadFile))
		{
			Debug.LogError($"[LoadGame] Save file missing for slot {saveSlotId}. Cleaning index.");
			SaveFiles.DeleteSlot(saveSlotId);
			return;
		}
		ModHook.FireOnGameLoadedInit();
		SaveFiles.WithIORetry(delegate
		{
			ES3.LoadInto("playerStore", loadFile, this);
		});
		SaveFiles.WithIORetry(delegate
		{
			ES3.LoadInto("storeStation", loadFile, StoreStation.Instance);
		});
		contrabandMarkupCritial = 150;
		StoreService.HealIntroducedAfterLoad();
		StoreClientData.HealButcherArrestAfterLoad();
		NetworkUpgrade.AddMissingAfterLoad();
		StoreReputation.AddMissingPerksAfterLoad();
		difficultyId = Difficulty.ResolveId(difficultyId, isHardMode);
		isHardMode = Difficulty.IsHardOrAbove();
		ModHook.FireOnDifficultyApplied(difficultyId);
		saveSlotId = NewGameData.Instance.saveSlot;
		ModHook.FireOnGameLoadedEarly();
		totalPlayTimeSeconds = SaveFiles.IndexLoad("totalPlayTimeSeconds", 0f);
		sessionStartTime = Time.realtimeSinceStartup;
		Debug.Log("Loaded Skip " + skipIntro);
		if (StartingPerk.IsMinimalist() || StartingPerk.IsAscetic())
		{
			EmporiumEntry.Instance.SetSmallInv();
		}
		LoadPower();
		StorePoster.LoadStorePoster();
		Shotgun.LoadShotgun();
		if (PCUIManager.Instance != null && PCUIManager.Instance.storePC != null)
		{
			PCUIManager.Instance.storePC.RefreshState();
		}
		ModHook.FireOnGameLoadedNormal();
		DecodeSaveItem(mainInvJSON, EmporiumEntry.Instance.invElement);
		DecodeSaveItem(backInvJSON, EmporiumEntry.Instance.backInvinvElement);
		DecodeSaveItem(hiddenInvJSON, EmporiumEntry.Instance.hiddenElement);
		DecodeSaveItem(showcaseInvJSON, EmporiumEntry.Instance.showcaseElement);
		DecodeSaveItem(soldInvJSON, EmporiumEntry.Instance.soldElement);
		DecodeSaveItem(hirelingInvJSON, EmporiumEntry.Instance.hirelingInv);
		DecodeSaveItem(vendingMachineInvJSON, EmporiumEntry.Instance.vendingMachineElement);
		DecodeSaveItem(vendingFountainInvJSON, EmporiumEntry.Instance.vendingFountainElement);
		DecodeSaveItem(trashcanInvJSON, (GameInventory)EmporiumEntry.Instance.trashcan.contentWindow.child);
		DecodeSaveItem(docInvJSON, (GameInventory)EmporiumEntry.Instance.dossier.contentWindow.child);
		if (!string.IsNullOrEmpty(cassettePlayerInvJSON))
		{
			EmporiumEntry.Instance.cassettePlayerElement.Expel(EmporiumEntry.Instance.cassettePlayer);
			DecodeSaveItem(cassettePlayerInvJSON, EmporiumEntry.Instance.cassettePlayerElement);
			if (EmporiumEntry.Instance.cassettePlayerElement.childItem != null)
			{
				EmporiumEntry.Instance.cassettePlayer = EmporiumEntry.Instance.cassettePlayerElement.childItem;
			}
		}
		List<GameItem> allItems = EmporiumEntry.Instance.GetAllItems();
		foreach (GameItem item in allItems)
		{
			item.RemoveAllFeature();
		}
		Dictionary<int, GameItem> dictionary = new Dictionary<int, GameItem>();
		foreach (GameItem item2 in allItems)
		{
			if (!dictionary.ContainsKey(item2.uniqueId))
			{
				dictionary.Add(item2.uniqueId, item2);
			}
		}
		foreach (ItemFeature savedItemFeature in savedItemFeatureList)
		{
			if (dictionary.TryGetValue(savedItemFeature.parentItemUniqueId, out var value))
			{
				value.itemFeatures.Add(savedItemFeature);
			}
		}
		savedItemFeatureList.Clear();
		saveSlotId = NewGameData.Instance.saveSlot;
		AlwayCall();
		Debug.Log("activated barter count " + exchangeManager.activeBarter.Count);
		Debug.Log("Loaded save slot: " + saveSlotId);
		if (isVendingMachineUnlocked)
		{
			StoreVendingMachine.Instance.gameObject.SetActive(value: true);
			StoreVendingMachine.Instance.UpdateSprite();
		}
		else
		{
			StoreVendingMachine.Instance.gameObject.SetActive(value: false);
		}
		if (isVendingFountainUnlocked)
		{
			StoreVendingFountain.Instance.gameObject.SetActive(value: true);
			StoreVendingFountain.Instance.UpdateSprite();
		}
		else
		{
			StoreVendingFountain.Instance.gameObject.SetActive(value: false);
		}
		if (NewGameData.Instance.isLoadFromMainMenu)
		{
			NewGameData.Instance.isLoadFromMainMenu = false;
			EmporiumAnalytics.TrackLoadGame();
		}
		PermitHelper.ReapplyPermits();
		Shotgun.ReturnIfLostWithPermit();
		ModHook.FireOnGameLoadedLate();
	}

	public static PlayerStore PreLoadStore()
	{
		if (NewGameData.Instance != null && NewGameData.Instance.saveSlot != -1)
		{
			return ES3.Load<PlayerStore>("playerStore", SaveFiles.ResolveLoadFile(NewGameData.Instance));
		}
		return null;
	}

	public void DecodeSaveItem(string JSON, GameInventory gameInventory)
	{
		GameItem gameItem = SaveManager.DecodeNodes(JsonUtility.FromJson<SaveState>(JSON).saveItems);
		foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(gameItem))
		{
			MachineHelper.LoadUpgrade(item2);
			SpriteHelper.UpdateWaterContainerSprite(item2);
			GeneralHelper.LoadLockItem(item2);
			PlantTraitList.LoadEdibleProduce(item2);
			PlantTraitList.LoadTasteFeature(item2);
			GeneralHelper.LoadNonOwnedToolTag(item2);
			GeneralHelper.LoadImportantStoryItemTag(item2);
			GeneralHelper.LoadHazardousWasteTag(item2);
			ScavHelper.LoadScavTokenTag(item2);
			LockHelper.LoadLockedContainer(item2);
			MachineBrokenHelper.LoadBrokenMachine(item2);
			MachineHelper.OnLoad(item2);
			MachineTurboBoosterAdv.LoadTurboBoostEligibleTag(item2);
			CurrentConverterHelper.OnLoad(item2);
			if (item2.onLoaded != null)
			{
				item2.onLoaded(item2);
			}
		}
		if (gameInventory == null)
		{
			return;
		}
		foreach (GraphNodeStorage item3 in gameItem.contentWindow.child.children.ToList())
		{
			((GameInventory)gameItem.contentWindow.child).Expel((GameItem)item3);
			GameItem item = item3 as GameItem;
			gameInventory.UncheckedAccept(item);
			GraphUtils.DestroyIfUnplaced(item, "load: target inventory refused a saved item");
		}
		gameItem.Destroy();
	}

	public void AlwayCall()
	{
		if (isCartelExited)
		{
			StoreReputation.RemoveCartelFaction();
		}
		if (NetworkUpgrade.IsUnlocked("GUTTERFLOW_TAP"))
		{
			EmporiumEntry.Instance.faucet.shortDescription = LocHelper.GetLocalizedMechanic("mech_gutterflow_tap");
		}
		else if (NetworkUpgrade.IsUnlocked("RUSTWATER_TAP"))
		{
			EmporiumEntry.Instance.faucet.shortDescription = LocHelper.GetLocalizedMechanic("mech_rustwater_tap");
		}
		StoreClientListMinor.InitUnseenMinorDict();
		StoreStation.Instance.storeEventManager.CheckBlackOutEvent(noLog: true);
		if (NetworkUpgrade.IsMarketplaceRequestsUnlocked())
		{
			StoreUIManager.Instance.pcScreenObject.gameObject.SetActive(value: true);
		}
		else
		{
			StoreUIManager.Instance.pcScreenObject.gameObject.SetActive(value: false);
		}
	}

	public void DebugInit()
	{
	}

	public bool CanStartDay()
	{
		return true;
	}

	public void OpenShutter()
	{
		storeState = StoreState.OPEN;
		AchievementTracker.Instance.CheckShiny();
	}

	public void OnLateShutterOpen()
	{
		if (StoreReputation.IsPerkUnlocked("LLTIPS") && RNG.Roll(4))
		{
			StoreStation.Instance.storeEventManager.PickRumorEvent();
		}
		if (StoreReputation.IsPerkUnlocked("LLLYNCH"))
		{
			SetupWaitingClient("mob");
		}
		if (StoreReputation.IsPerkUnlocked("SEC_IMMEDIATE_ARREST"))
		{
			SetupWaitingClient("sec");
		}
		if (StoreReputation.IsPerkUnlocked("UL_HITMAN"))
		{
			SetupWaitingClient("hitman");
		}
		if (StoreReputation.IsPerkUnlocked("REV_HIT"))
		{
			SetupRevBombing();
		}
		if (StoreReputation.IsPerkUnlocked("CARTEL_RETIRED"))
		{
			SetupWaitingClient("cartel");
		}
		if (currentClientInstance == null && StoreClientData.CanLouMorningVisit(StoreStation.Instance.dayCounter))
		{
			SetupWaitingClient("lou");
		}
	}

	public void BeginDay()
	{
		StallWatchdog.Crumb("begin day");
		StoreStation.Instance.StartDay();
		Instance.dayUntilRent--;
		VendingMachineHelper.SettleMislabelledSales();
		storeClientManager.OnNewDay();
		secData.OnNewDay();
		AfterhourEvents.OnNewDay();
		commissaryData.OnNewDay();
	}

	public void UpdateServiceCost()
	{
		foreach (StoreService item in storeService)
		{
			item.UpdateCost();
			if (item.unlocked && item.currentCooldown > 0)
			{
				item.currentCooldown--;
			}
		}
	}

	public void SetupRevBombing()
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.frontInvinvElement, DirectoryMaster.Item("c4_set"));
		StoreUIManager.Instance.TriggerBombing();
	}

	public void SetupWaitingClient(string id)
	{
		StoreUIManager.Instance.clientGameObject.SetImmediatlyArrive();
		if (id.Equals("mob"))
		{
			StoreUIManager.Instance.secGameObject.SetImmediatlyArrive();
			StoreUIManager.Instance.secGameObject2.SetImmediatlyArrive();
			StoreUIManager.Instance.secGameObject.image.sprite = SpriteDict.Instance.ShowClientSprite(SpriteDict.GetRandomLowerLevelSpriteName());
			StoreUIManager.Instance.clientGameObject.image.sprite = SpriteDict.Instance.ShowClientSprite(SpriteDict.GetRandomLowerLevelSpriteName());
			StoreUIManager.Instance.secGameObject2.image.sprite = SpriteDict.Instance.ShowClientSprite(SpriteDict.GetRandomLowerLevelSpriteName());
			currentClientInstance = StoreClientInstance.CreateClientInstance(StoreClientListMorning.CreateMobLeader());
		}
		else if (id.Equals("sec"))
		{
			StoreUIManager.Instance.secGameObject.SetImmediatlyArrive();
			StoreUIManager.Instance.secGameObject2.SetImmediatlyArrive();
			StoreUIManager.Instance.secGameObject.image.sprite = SpriteDict.Instance.ShowClientSprite(SpriteDict.GetRandomSecSpriteName());
			StoreUIManager.Instance.clientGameObject.image.sprite = SpriteDict.Instance.ShowClientSprite(SpriteDict.GetRandomSecSpriteName());
			StoreUIManager.Instance.secGameObject2.image.sprite = SpriteDict.Instance.ShowClientSprite(SpriteDict.GetRandomSecSpriteName());
			currentClientInstance = StoreClientInstance.CreateClientInstance(StoreClientListMorning.CreateSecLeader());
		}
		else if (id.Equals("hitman"))
		{
			StoreUIManager.Instance.clientGameObject.image.sprite = SpriteDict.Instance.ShowClientSprite(SpriteDict.GetRandomMercSpriteName());
			currentClientInstance = StoreClientInstance.CreateClientInstance(StoreClientListMorning.CreateULHitman());
		}
		else if (id.Equals("cartel"))
		{
			StoreUIManager.Instance.clientGameObject.image.sprite = SpriteDict.Instance.ShowClientSprite(SpriteDict.GetRandomShadySpriteName());
			currentClientInstance = StoreClientInstance.CreateClientInstance(StoreClientListMorning.CreateCartelRetired());
		}
		else if (id.Equals("lou"))
		{
			StoreUIManager.Instance.clientGameObject.image.sprite = SpriteDict.Instance.ShowClientSprite("wanted5");
			currentClientInstance = StoreClientInstance.CreateClientInstance(StoreClientListWanted.CreateWanted5Morning());
		}
		StoreUIManager.Instance.RunMorningDialogue();
	}

	public void OnShutterOpened()
	{
		if (StoreStation.Instance.dayCounter >= 8)
		{
			isNewspaperUnlocked = true;
		}
		CheckCrime();
		SoldItem.HandleSoldItem();
		callNextWhenPossible = false;
		Instance.lockShutterButtonFlag = 0;
		storeClientManager.GenerateClient();
		HandleFutureClientQueue();
		int num = storeClientManager.clientStack.RemoveAll((StoreClient c) => c == null);
		if (num != 0)
		{
			Debug.Log($"<color=red>Removed {num} null clients.</color>");
		}
		LogQueuedClient();
		RefreshCounterItem();
	}

	public void LogQueuedClient()
	{
		string text = "";
		foreach (StoreClient item in storeClientManager.clientStack)
		{
			if (item != null && item.identifier != null)
			{
				text = text + item.identifier + ", ";
			}
			else if (item != null)
			{
				Debug.Log("A StoreClient do not have a valid ID" + item.displayName);
			}
			else
			{
				Debug.Log("A StoreClient is not correctly initialized");
			}
		}
		Debug.Log(text);
	}

	public void GetNextClient()
	{
		storeClientManager.HoldBackQueuedPopCultureClients();
		if (storeClientManager.clientStack.Count != 0)
		{
			StoreClient storeClient = storeClientManager.clientStack[0];
			StoreClientMono.Instance.image.sprite = SpriteDict.Instance.ShowClientSprite(storeClient.GetSprite());
			storeClientManager.clientStack.RemoveAt(0);
			storeClientManager.TrackClientVisit(storeClient);
			RunStats.OnCustomerArrived(storeClient);
			NegociationUIManager.Instance.currentNegociatedItem = null;
			StallWatchdog.Crumb("client " + storeClient.identifier);
			currentClientInstance = StoreClientInstance.CreateClientInstance(storeClient);
		}
	}

	public bool CanNextClient()
	{
		if (storeClientManager.clientStack.Count == 0)
		{
			return false;
		}
		if (StoreUIManager.Instance.isShot || isHandlingShootout)
		{
			return false;
		}
		if (isClientBeingArrested)
		{
			return false;
		}
		if (currentClientInstance != null)
		{
			return false;
		}
		return true;
	}

	public void TryCallNextClient()
	{
		if (Instance.CanNextClient())
		{
			Instance.GetNextClient();
			StoreUIManager.Instance.QueueClientArriveAnimation();
		}
	}

	public bool IsClientPresent()
	{
		if (currentClientInstance != null)
		{
			return true;
		}
		return false;
	}

	public bool IsClientDimissable()
	{
		if (currentClientInstance != null && currentClientInstance.GetClientBlueprint().dismissable)
		{
			return true;
		}
		return false;
	}

	public void QueueFuturClient(string storeClientId, int day)
	{
		if (storeState == StoreState.CLOSED)
		{
			futurStoreClientIdQueue.Add(storeClientId);
			futurStoreClientDayQueue.Add(day);
		}
		else if (storeState == StoreState.OPEN)
		{
			futurStoreClientIdQueue.Add(storeClientId);
			futurStoreClientDayQueue.Add(day);
		}
		else if (storeState == StoreState.MORNING)
		{
			futurStoreClientIdQueue.Add(storeClientId);
			futurStoreClientDayQueue.Add(day + 1);
		}
	}

	public void HandleFutureClientQueue()
	{
		if (futurStoreClientDayQueue.Count == 0)
		{
			return;
		}
		for (int i = 0; i < futurStoreClientDayQueue.Count; i++)
		{
			if (futurStoreClientIdQueue[i] == null)
			{
				Debug.Log("<color=red>A future client is null</color>");
				continue;
			}
			futurStoreClientDayQueue[i]--;
			Debug.Log("Future Client Queue: " + futurStoreClientIdQueue[i] + "/ In " + futurStoreClientDayQueue[i]);
		}
		int num = 0;
		for (int num2 = futurStoreClientIdQueue.Count - 1; num2 >= 0; num2--)
		{
			if (futurStoreClientIdQueue[num2] == null)
			{
				futurStoreClientIdQueue.RemoveAt(num2);
				futurStoreClientDayQueue.RemoveAt(num2);
				num++;
			}
		}
		if (num > 0)
		{
			Debug.Log($"<color=red>Removed {num} null clients from the future queue.</color>");
		}
		int num3 = 0;
		while (num3 < futurStoreClientIdQueue.Count)
		{
			if (futurStoreClientDayQueue[num3] <= 0)
			{
				storeClientManager.AddClient(StoreClientListDict.CreateStoreClient(futurStoreClientIdQueue[num3]));
				futurStoreClientIdQueue.RemoveAt(num3);
				futurStoreClientDayQueue.RemoveAt(num3);
			}
			else
			{
				num3++;
			}
		}
	}

	public void PayRentCheck()
	{
		CanPayRent();
	}

	public void RentIncrease()
	{
		rentValue = (int)((float)rentValue * Difficulty.Rules.rentIncreaseMultiplier);
	}

	public bool CanPayRent()
	{
		if (playerCash >= rentValue + temporaryRent)
		{
			return true;
		}
		return false;
	}

	public void OnRentPaid()
	{
		dayUntilRent = 7;
		EmporiumAudioPlayer.Instance.PlayRegister();
		int num = rentValue + temporaryRent;
		playerCash -= num;
		RunStats.OnRentPaid(num, playerCash);
		RentIncrease();
		temporaryRent = 0;
	}

	public void OnRentNotPaid()
	{
		NetworkUpgrade.GetUpgradeById("LANDLORD_GRACE").state = 1;
		dayUntilRent = 7;
		temporaryRent += rentValue;
		RunStats.OnRentMissed();
		RentIncrease();
	}

	public float GetCurrentRunPlayTime()
	{
		return runPlayTimeSeconds + (Time.realtimeSinceStartup - sessionStartTime);
	}

	public float GetCurrentTotalPlayTime()
	{
		return totalPlayTimeSeconds + (Time.realtimeSinceStartup - sessionStartTime);
	}

	public int GetRentDayCounter()
	{
		int dayCounter = StoreStation.Instance.dayCounter;
		return (7 - dayCounter % 7) % 7;
	}

	public int GetLoanDayCounter()
	{
		return dayUntilLoan;
	}

	public void ExecuteGameOver(string ending = "rent")
	{
		EmporiumAnalytics.TrackGameOver(ending);
		RunStats.OnRunEnded(StoreStation.Instance.dayCounter, GetTotalEstimatedValue(), playerCash);
		GameOverUIManager.Instance.SetEnding(ending);
		if (ending.Equals("rent"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_rent_reason"), LocHelper.GetLocalizedMechanic("mech_end_rent_desc"));
		}
		else if (ending.Equals("arrest"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_arrest_reason"), LocHelper.GetLocalizedMechanic("mech_end_arrest_desc"));
		}
		else if (ending.Equals("execution"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_execution_reason"), LocHelper.GetLocalizedMechanic("mech_end_execution_desc"));
		}
		else if (ending.Equals("buyout"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_buyout_reason"), LocHelper.GetLocalizedMechanic("mech_end_buyout_desc"), showContinuePlay: true);
		}
		else if (ending.Equals("buyout_mortgage"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_buyout_reason"), LocHelper.GetLocalizedMechanic("mech_end_buyout_mortgage_desc"), showContinuePlay: true);
		}
		else if (ending.Equals("repo"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_repo_reason"), LocHelper.GetLocalizedMechanic("mech_end_repo_desc"));
		}
		else if (ending.Equals("mob"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_mob_reason"), LocHelper.GetLocalizedMechanic("mech_end_mob_desc"));
		}
		else if (ending.Equals("sec"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_sec_reason"), LocHelper.GetLocalizedMechanic("mech_end_sec_desc"));
		}
		else if (ending.Equals("hitman"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_hitman_reason"), LocHelper.GetLocalizedMechanic("mech_end_hitman_desc"));
		}
		else if (ending.Equals("sentinel"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_sentinel_reason"), LocHelper.GetLocalizedMechanic("mech_end_sentinel_desc"));
		}
		else if (ending.Equals("bombed"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_bombed_reason"), LocHelper.GetLocalizedMechanic("mech_end_bombed_desc"));
		}
		else if (ending.Equals("lou_bombed"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_bombed_reason"), LocHelper.GetLocalizedMechanic("mech_end_lou_bombed_desc"));
		}
		else if (ending.Equals("lou_bombed_debt"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_bombed_reason"), LocHelper.GetLocalizedMechanic("mech_end_lou_debt_desc"));
		}
		else if (ending.Equals("cartel_retired"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_cartel_retired_reason"), LocHelper.GetLocalizedMechanic("mech_end_cartel_retired_desc"));
		}
		else if (ending.Equals("vendor_permit"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_vendor_permit_reason"), LocHelper.GetLocalizedMechanic("mech_end_vendor_permit_desc"));
		}
		else if (ending.Equals("aug"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_aug_reason"), LocHelper.GetLocalizedMechanic("mech_end_aug_desc"));
		}
		else if (ending.Equals("reactor"))
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_reactor_reason"), LocHelper.GetLocalizedMechanic("mech_end_reactor_desc"));
		}
		else
		{
			GameOverUIManager.Instance.DisplayUI(LocHelper.GetLocalizedMechanic("mech_end_rent_reason"), LocHelper.GetLocalizedMechanic("mech_end_rent_desc"));
		}
	}

	public int GetRentValue()
	{
		return rentValue + temporaryRent;
	}

	public bool CanDismissCurrentClient()
	{
		return currentClientInstance.GetClientBlueprint().dismissable;
	}

	public void TryPlayerDismissCurrentClient(bool callNextWhenPossible = true)
	{
		if (currentClientInstance != null)
		{
			StoreClient clientBlueprint = currentClientInstance.GetClientBlueprint();
			Func<bool> tryInterceptDismiss = clientBlueprint.tryInterceptDismiss;
			if (tryInterceptDismiss != null)
			{
				clientBlueprint.tryInterceptDismiss = null;
				if (tryInterceptDismiss())
				{
					return;
				}
			}
		}
		DismissCurrentClient(callNextWhenPossible);
	}

	public void DismissCurrentClient(bool callNextWhenPossible = true, bool right = true, bool interruptDialog = true)
	{
		if (currentClientInstance != null)
		{
			Instance.lastVisitedClient = currentClientInstance;
			if (currentClientInstance.GetClientBlueprint().isAug && !StoreUIManager.Instance.isShot)
			{
				AugHelper.OnAugLeftAlive();
			}
			if (currentClientInstance.GetClientBlueprint().OnDismiss != null)
			{
				currentClientInstance.GetClientBlueprint().OnDismiss();
			}
			currentClientInstance = null;
			dealMakerBargainBonus = 0;
			dealMakerActivatedThisCustomer = false;
			RemoveNotOwnedItemFromTable();
			EmporiumEntry.Instance.RemoveTemporaryClientFeatureFromItems();
			NegociationUIManager.Instance.currentNegociatedItem = null;
			StoreUIManager.Instance.OnClientStartLeaving(right);
			InspectionUIManager.Instance.CloseConfrontPanel();
			Instance.callNextWhenPossible = callNextWhenPossible;
			if (EmporiumOption.Instance.autoCallNext)
			{
				instance.callNextWhenPossible = true;
			}
			if (interruptDialog)
			{
				DialogUIManager.Instance.InterruptDialog();
				DialogUIManager.Instance.HideChoiceBubble();
			}
		}
	}

	public void RemoveNotOwnedItemFromTable()
	{
		EmporiumEntry.Instance.DeselectDragItem(nonOwnedOnly: true);
		for (int num = EmporiumEntry.Instance.frontInvinvElement.childItems.Count - 1; num >= 0; num--)
		{
			if (!GeneralHelper.IsItemOwned(EmporiumEntry.Instance.frontInvinvElement.childItems[num]))
			{
				GunHelper.ReturnPlayerParts(EmporiumEntry.Instance.frontInvinvElement.childItems[num]);
				EmporiumEntry.Instance.frontInvinvElement.childItems[num].Destroy();
			}
		}
		for (int num2 = EmporiumEntry.Instance.backInvinvElement.childItems.Count - 1; num2 >= 0; num2--)
		{
			if (!GeneralHelper.IsItemOwned(EmporiumEntry.Instance.backInvinvElement.childItems[num2]))
			{
				GunHelper.ReturnPlayerParts(EmporiumEntry.Instance.backInvinvElement.childItems[num2]);
				EmporiumEntry.Instance.backInvinvElement.childItems[num2].Destroy();
			}
		}
	}

	public void EndDay()
	{
		StallWatchdog.Crumb("end day");
		storeState = StoreState.CLOSED;
		Debug.Log("Ending Day");
		storeClientManager.DismissClientStack();
		UpdateServiceCost();
	}

	public void EndNight()
	{
		if (currentClientInstance != null)
		{
			StoreUIManager.Instance.HideClient();
			DismissCurrentClient(callNextWhenPossible: false);
		}
		StoreUIManager.Instance.ClearArrestState();
		RunStats.OnDayEnded(StoreStation.Instance.dayCounter, GetTotalEstimatedValue(), playerCash);
		ModHook.FireOnGoingSleepEarly();
		Instance.projectorAttractivenessBonus = 0;
		ReactorHelper.OnNight();
		UpdatePowerState();
		CurrentConverterHelper.isNightCycle = true;
		FindAllItem().ForEach(delegate(GameItem i)
		{
			i.onCycleEndEarlySlotItemFunc?.Invoke(i, i.parentInventory, i.GetIncompleteSlotInInventory());
		});
		List<string> list = new List<string> { "INSURANCE_SERVICE", "BUY_REFERAL", "SELL_REFERAL", "WILD_FIXER", "CRIMINEL_NETWORK" };
		foreach (StoreService item in storeService)
		{
			if (item.unlocked && item.isUsing)
			{
				if (list.Contains(item.id))
				{
					wildFavor += (int)((float)item.cost * Difficulty.Rules.wildServiceFavorRefund);
				}
				playerCash -= item.cost;
				RunStats.OnServicePaid(item.id, item.cost);
				item.currentCooldown = item.totalCooldown;
			}
		}
		ModHook.FireOnHandlingNightlyServicesEarly();
		HandleOtherServices();
		HandleJanitorial();
		HandlePower();
		HandleInsurance();
		HandleBuyerReferal();
		HandleSellerReferal();
		HandleCriminelNetwork();
		HandleVandalism();
		HandleButcherHunting();
		LouBombHelper.OnNight();
		LouOperativeHelper.OnNight();
		UseCountHelper.OnNight();
		HandleInspectionEarlyWarning();
		HandleWantedPersonVistedCheck();
		HandleUpgradeTip();
		HandleSecurityWarning();
		HandleAugWarning();
		HandleOperation();
		HandleAfterhourDebuff();
		StartingPerkContent.HandleAgoraphobiaReady();
		ModHook.FireOnHandlingNightlyServicesLate();
		expeditionData.OnDayEnd();
		if (StoreStation.GetDayCounter() % 7 == 0)
		{
			exchangeManager.ResetPromotional();
		}
		healthData.OnSleep();
		StorePhoneClient.OnEndDay();
		EmporiumEntry.Instance.ClearFrontInv();
		DebrisHelper.OnNight();
		ChemicalHelper.RunNightlyChemicalSimulation();
		MachineFridge.ClearColdMarkers();
		HopperHelper.RunWindow(1);
		FindAllItem().ForEach(delegate(GameItem i)
		{
			StolenHelper.UpdateStolenItem(i);
			i.onCycleEndSlotItemFunc?.Invoke(i, i.parentInventory, i.GetIncompleteSlotInInventory());
		});
		HopperHelper.RunWindow(2);
		FindAllItem().ForEach(delegate(GameItem i)
		{
			i.onCycleEndLateSlotItemFunc?.Invoke(i, i.parentInventory, i.GetIncompleteSlotInInventory());
		});
		HopperHelper.RunWindow(3);
		CurrentConverterHelper.isNightCycle = false;
		MachineFridge.HandleColdFeatureExpiry();
		instance.storeState = StoreState.MORNING;
		Instance.lockShutterButtonFlag = 0;
		if (EmporiumEntry.Instance.vendingFountainElement.childItem != null)
		{
			EmporiumEntry.Instance.vendingFountainElement.childItem.contentWindow.Hide();
		}
		if (EmporiumEntry.Instance.vendingMachineElement.childItem != null)
		{
			EmporiumEntry.Instance.vendingMachineElement.childItem.contentWindow.Hide();
		}
		ModHook.FireOnGoingSleepLate();
	}

	public List<GameItem> FindAllItem(bool isOwn = true)
	{
		List<GameItem> list = new List<GameItem>();
		list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.frontInvWindow));
		list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.backInvWindow));
		list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.backInvinvElementCounter));
		list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.invWindow));
		list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.showcaseWindow));
		list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.trashInvWindow));
		list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.docInvWindow));
		list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.hiddenWindow));
		list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.afterhourWindow));
		list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.vendingFountainWindow));
		list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.vendingMachineWindow));
		if (isOwn)
		{
			list = list.Where((GameItem item) => item.IsTag("IS_OWNED_TAG")).ToList();
		}
		return list.Distinct().ToList();
	}

	public bool IsPlayerOwnThisItem(string id)
	{
		foreach (GameItem item in FindAllItem())
		{
			if (item.identifier.Equals(id))
			{
				return true;
			}
		}
		return false;
	}

	public bool CanEndDay()
	{
		foreach (StoreClient item in storeClientManager.clientStack)
		{
			if (!item.dismissable)
			{
				return false;
			}
		}
		if (currentClientInstance != null && !currentClientInstance.GetClientBlueprint().dismissable)
		{
			return false;
		}
		return true;
	}

	public void HandleBuyerReferal()
	{
		if (StoreService.GetBuyReferalService().unlocked && StoreService.GetBuyReferalService().isUsing)
		{
			NightLog.BuyerReferral();
		}
	}

	public void HandleSellerReferal()
	{
		if (StoreService.GetSellReferalService().unlocked && StoreService.GetSellReferalService().isUsing)
		{
			NightLog.SellerReferral();
		}
	}

	public void HandleCriminelNetwork()
	{
		if (StoreService.GetCriminelNetworkService().unlocked && StoreService.GetCriminelNetworkService().isUsing)
		{
			NightLog.CriminalNetwork();
		}
	}

	public void HandleInspectionEarlyWarning()
	{
		if (StoreService.GetInspectionEarlyWarningService().unlocked && !StoreReputation.IsPerkUnlocked("SEC_VIP") && storeClientManager.IsInspectionCountdownActive() && StoreService.GetInspectionEarlyWarningService().isUsing)
		{
			if (instance.storeClientManager.dayUntilInspection == 3)
			{
				NightLog.InspectionWarningIn2Days();
			}
			if (instance.storeClientManager.dayUntilInspection == 2)
			{
				NightLog.InspectionWarningTomorrow();
			}
			if (instance.storeClientManager.dayUntilInspection == 1)
			{
				NightLog.InspectionWarningImminent();
			}
		}
	}

	public void HandleWantedPersonVistedCheck()
	{
		if (wantedPersonVisitedToday && !correctReportFileToday)
		{
			NightLog.WantedPersonVisited(wantedPersonVisitedName);
			EmporiumAnalytics.TrackWantedNotReported();
		}
		wantedPersonVisitedName = "";
		wantedPersonVisitedId = "";
		wantedPersonVisitedToday = false;
		correctReportFileToday = false;
	}

	public void HandleUpgradeTip()
	{
		if (playerCash >= GetRentValue() * 4)
		{
			NightLog.UpgradeTip();
		}
	}

	public void HandleSecurityWarning()
	{
		if (secData.IsStringOperation())
		{
			NightLog.StingOperationWarning();
		}
		if (secData.evidenceLevel > 90)
		{
			NightLog.SecurityInvestigationNearComplete();
		}
	}

	public void HandleAugWarning()
	{
		if (Difficulty.Rules.augWarning && isAugIntroduced && !isAugEnded && nextAugSpawnDay >= 0)
		{
			int num = StoreStation.Instance.dayCounter + 1;
			int augIntroducedDay = GetAugIntroducedDay();
			if ((StartingPerk.IsAugMenace() || (num != augIntroducedDay + 1 && num != augIntroducedDay + 6)) && !IsAugAllClearDue(num) && num >= nextAugSpawnDay)
			{
				NightLog.AugIncoming();
			}
		}
	}

	public void HandleOperation()
	{
		StoreStation.Instance.storeOperationManager.OnDayStart();
	}

	public void HandleAfterhourDebuff()
	{
		if (healthData.wentToAfterhoursLastNight && !StartingPerk.IsShortSleeper())
		{
			NightLog.AfterhourDebuff();
		}
	}

	public void OnCriminalVisit(StoreClient storeClient)
	{
		if (storeClient.isWanted)
		{
			storeClientManager.arrestedClients.Add(storeClient.identifier);
			if (!storeClient.isStealthWanted)
			{
				wantedPersonVisitedName = storeClient.realName;
				wantedPersonVisitedId = storeClient.identifier;
				wantedPersonVisitedToday = true;
				correctReportFileToday = false;
			}
		}
	}

	public void HandleOtherServices()
	{
		if (StoreService.GetWildFixerService().isUsing)
		{
			secData.OnFixerUsed();
			NightLog.FixerUsed();
		}
	}

	public void HandleJanitorial()
	{
		if (!StoreService.GetJanitorialService().unlocked)
		{
			return;
		}
		if (!StoreService.GetJanitorialService().isUsing)
		{
			garbageLevel += 2;
			if (garbageLevel > 10)
			{
				garbageLevel = 10;
			}
			NightLog.StorefrontNotCleaned(NightLog.GetGarbageDescription(garbageLevel));
			return;
		}
		if (exchangeManager.isExchangeUnlocked && !StoreReputation.IsPerkUnlocked("BM_DISTRUSTED"))
		{
			exchangeManager.Reset();
			exchangeManager.HandleExchange();
		}
		ClearTrash();
		exchangeManager.TransferBuffer();
		NightLog.StorefrontClean();
		if (isVandalized)
		{
			isVandalized = false;
			daySinceLastVandalism = 0;
			NightLog.VandalismCleaned();
		}
	}

	public void ClearTrash()
	{
		garbageLevel = 0;
		if (EmporiumEntry.Instance.trashcanInvElement == null)
		{
			return;
		}
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.trashcan);
		if (list.Count == 0)
		{
			return;
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (!GeneralHelper.IsHazardousWaste(list[num]) && !IsKeptInTrash(list[num]))
			{
				list[num].CloseContentWindow(includeChildren: true);
				list[num].Destroy();
			}
		}
	}

	private static bool IsKeptInTrash(GameItem item)
	{
		if (item.IsID("smuggler_bay_mod"))
		{
			return true;
		}
		return GraphUtils.FindParentType(item, (GameItem parent) => parent.IsID("smuggler_bay_mod")) != null;
	}

	public void UpdatePowerState()
	{
		if (StartingPerk.IsMakeshiftReactor())
		{
			isPowerOn = ReactorHelper.IsStorePowered();
		}
		else
		{
			isPowerOn = StoreService.GetPowerService().isUsing || NetworkUpgrade.IsPowerHijackUnlocked();
		}
	}

	public void HandlePower()
	{
		UpdatePowerState();
		if (!isPowerOn)
		{
			NightLog.PowerOff();
			StoreUIManager.Instance.darknessPanel.SetActive(value: true);
		}
		else
		{
			NightLog.PowerOn();
			StoreUIManager.Instance.darknessPanel.SetActive(value: false);
		}
	}

	public void LoadPower()
	{
		if (!isPowerOn)
		{
			StoreUIManager.Instance.darknessPanel.SetActive(value: true);
		}
		else
		{
			StoreUIManager.Instance.darknessPanel.SetActive(value: false);
		}
	}

	public void HandleInsurance()
	{
		if (!StoreService.GetInsuranceService().unlocked)
		{
			return;
		}
		if (skipIntro && StoreStation.Instance.dayCounter < startingDay + 3)
		{
			NightLog.BurglaryGrace(startingDay + 3 - StoreStation.Instance.dayCounter);
			return;
		}
		if (isSecurityAlarmActive)
		{
			MachineAlarm.Operate(MachineAlarm.FindAlarmeItem());
		}
		if (StoreService.GetCartelProtectionService().isUsing)
		{
			NightLog.CartelProtectionActive();
		}
		else
		{
			if (StartingPerk.IsAlert())
			{
				return;
			}
			if (!StoreService.GetInsuranceService().isUsing)
			{
				dayWithoutInsurance++;
				NightLog.InsuranceUnpaid(dayWithoutInsurance);
				int num = 33;
				if (StoreReputation.IsPerkUnlocked("LLTARGET"))
				{
					num += 33;
				}
				if (StoreReputation.IsPerkUnlocked("SEC_PROTECTION"))
				{
					num -= 16;
				}
				if (StoreReputation.IsPerkUnlocked("SEC_VIP"))
				{
					num = -100000;
				}
				if (RNG.Roll(num + dayWithoutInsurance * 5))
				{
					if (isSecurityAlarmActive && RNG.Roll(MachineAlarm.GetAlarmeStopRate() + 1))
					{
						NightLog.BurglaryAlarmStopped();
					}
					else
					{
						GameItem randomBurglaryEligibleItem = GetRandomBurglaryEligibleItem();
						if (randomBurglaryEligibleItem != null)
						{
							if (isSecurityAlarmActive)
							{
								NightLog.BurglaryAlarmFailed(randomBurglaryEligibleItem.GetDisplayName());
							}
							else
							{
								NightLog.BurglaryNoAlarm(randomBurglaryEligibleItem.GetDisplayName());
							}
							StealItem(randomBurglaryEligibleItem);
						}
					}
				}
			}
			else
			{
				dayWithoutInsurance = 0;
				NightLog.InsurancePaid();
				if (RNG.Roll(6))
				{
					GameItem randomBurglaryEligibleItem2 = GetRandomBurglaryEligibleItem();
					if (randomBurglaryEligibleItem2 != null)
					{
						NightLog.BurglaryInsured(randomBurglaryEligibleItem2.GetDisplayName());
					}
				}
			}
			isSecurityAlarmActive = false;
		}
	}

	public void HandleVandalism()
	{
		daySinceLastVandalism++;
		if (storeClientData.johnSmithReturnReportedDay >= 0 && !storeClientData.isJohnSmithVandalismDone && StoreStation.Instance.dayCounter >= storeClientData.johnSmithReturnReportedDay + 1)
		{
			storeClientData.isJohnSmithVandalismDone = true;
			storeClientData.isJohnSmithVandalVisitPending = true;
			daySinceLastVandalism = 0;
			isVandalized = true;
			AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_vandalized_john_smith"), "9c3738");
		}
		if (storeClientData.butcherArrestedDay >= 0 && !storeClientData.isButcherVandalismDone && StoreStation.Instance.dayCounter >= storeClientData.butcherArrestedDay + StoreClientData.ButcherDelay(7) - 1)
		{
			storeClientData.isButcherVandalismDone = true;
			storeClientData.butcherVandalismMorningDay = StoreStation.Instance.dayCounter + 1;
			daySinceLastVandalism = 0;
			isVandalized = true;
			AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_vandalism_butcher"), "9c3738");
		}
		if (!isVandalized && daySinceLastVandalism >= 2)
		{
			foreach (StoreReputation storeReputation in storeReputations)
			{
				if (storeReputation.GetReputation() <= -20 && !isVandalized && RNG.Roll(Math.Abs(storeReputation.GetReputation()) / 5))
				{
					daySinceLastVandalism = 0;
					isVandalized = true;
					NightLog.VandalizedByFaction(storeReputation.factionId);
					break;
				}
			}
		}
		if (isVandalized)
		{
			NightLog.StoreStillVandalized();
		}
	}

	public void HandleButcherHunting()
	{
		StoreService butcherHuntingService = StoreService.GetButcherHuntingService();
		if (butcherHuntingService != null && butcherHuntingService.IsActive())
		{
			storeClientData.isButcherHuntOver = true;
			butcherHuntingService.isUsing = false;
			butcherHuntingService.unlocked = false;
			AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_butcher_hunted", LocHelper.GetLocalizedClientName("name_the_butcher")), "5c6f46");
		}
	}

	public void StealItem(GameItem gameItem)
	{
		if (gameItem != null)
		{
			lastTheftDay = StoreStation.Instance.dayCounter;
			lastTheftValue = (int)gameItem.GetNegociatedValue();
			RunStats.OnBurglary(lastTheftValue);
			gameItem.Destroy();
		}
	}

	public void StealRandomEligibleItem()
	{
		StealItem(GetRandomBurglaryEligibleItem());
	}

	public GameItem GetRandomItem()
	{
		List<GameItem> list = (from item in EmporiumEntry.Instance.GetAllOwnedItems()
			where !item.IsTag("IMPORTANT_TAG")
			select item).ToList();
		if (list.Count < 1)
		{
			return null;
		}
		int randomInt = RNG.GetRandomInt(0, list.Count - 1);
		return list[randomInt];
	}

	public GameItem GetRandomBurglaryEligibleItem()
	{
		List<GameItem> list = (from item in EmporiumEntry.Instance.GetAllOwnedItems(isBurglary: true)
			where IsEligibleForRepo(item)
			select item).ToList();
		if (list.Count < 1)
		{
			return null;
		}
		int randomInt = RNG.GetRandomInt(0, list.Count - 1);
		return list[randomInt];
	}

	public List<GameItem> GetAllRepoEligibleItems()
	{
		return (from item in EmporiumEntry.Instance.GetAllOwnedItems(isBurglary: true)
			where IsEligibleForRepo(item)
			select item).ToList();
	}

	public bool IsEligibleForRepo(GameItem gameItem)
	{
		if (gameItem.IsTag("REPO_ELIGIBLE_OVERRIDE_TAG"))
		{
			return true;
		}
		if (gameItem.IsGameItemType("DOCUMENT"))
		{
			return false;
		}
		if (gameItem.IsGameItemType("CONTAINER_TAG"))
		{
			return false;
		}
		if (gameItem.IsTag("MODULE_STUCK_TAG"))
		{
			return false;
		}
		if (gameItem.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L) == 0L)
		{
			return false;
		}
		if (gameItem.IsRootedInSmugglerBay())
		{
			return false;
		}
		if (gameItem.IsTag("IMPORTANT_TAG"))
		{
			return false;
		}
		if (LouToyBoxHelper.IsInsideLouBox(gameItem))
		{
			return false;
		}
		if (gameItem.IsGameItemType("MACHINE"))
		{
			return false;
		}
		if (gameItem.IsTag("LOCKED_TAG"))
		{
			return false;
		}
		if (gameItem.IsTag("ITEM_BEHIND_LOCK_TAG"))
		{
			return false;
		}
		if (gameItem.parentInventory != null && gameItem.parentInventory.IsInsertLocked() && gameItem.parentInventory.IsRemoveLocked())
		{
			return false;
		}
		return true;
	}

	public bool IsEnoughForRepo()
	{
		List<GameItem> allRepoEligibleItems = GetAllRepoEligibleItems();
		int num = 0;
		foreach (GameItem item in allRepoEligibleItems)
		{
			num += (int)item.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L);
		}
		if (num >= mortgageWeeklyPayment)
		{
			return true;
		}
		Debug.Log("Repo not possible");
		return false;
	}

	public List<GameItem> FindRepoItems(List<GameItem> items, int targetValue)
	{
		items = items.OrderByDescending((GameItem i) => i.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L)).ToList();
		List<GameItem> bestFit = new List<GameItem>();
		int currentSum = 0;
		foreach (GameItem item in items)
		{
			if (currentSum + item.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L) <= targetValue)
			{
				bestFit.Add(item);
				currentSum += (int)item.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L);
			}
		}
		if ((double)currentSum * 1.25 < (double)targetValue)
		{
			GameItem gameItem = (from i in items
				where !bestFit.Contains(i)
				orderby Math.Abs(targetValue - (currentSum + i.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L)))
				select i).FirstOrDefault();
			if (gameItem != null)
			{
				bestFit.Add(gameItem);
				currentSum += (int)gameItem.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L);
			}
		}
		foreach (GameItem item2 in bestFit)
		{
			Debug.Log(item2.name + " " + item2.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L));
		}
		Debug.Log(currentSum);
		return bestFit;
	}

	public void Repo()
	{
		List<GameItem> list = Instance.FindRepoItems(Instance.GetAllRepoEligibleItems(), Instance.mortgageWeeklyPayment);
		string listName = "";
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num] != null)
			{
				listName = listName + list[num].name + "\n";
				list[num].Destroy();
				list.RemoveAt(num);
			}
		}
		GameItem gameItem = DirectoryMaster.Item("slip");
		gameItem.name = "Repo Slip";
		gameItem.ModifyTag("SLIP_TITLE", delegate(TagState state)
		{
			state.Enable().SetString("Repo Slip");
		});
		gameItem.ModifyTag("SLIP_BODY", delegate(TagState state)
		{
			state.Enable().SetString(listName);
		});
		Instance.AddDirectSellingItemToTableMiddle(gameItem, isOwend: true);
		dayUntilRent = 7;
	}

	public int GetCurrentStoreAttractiveness()
	{
		int trashAttPenalty = GetTrashAttPenalty();
		return baseStoreAttractiveness - trashAttPenalty + projectorAttractivenessBonus;
	}

	public int GetTrashAttPenalty()
	{
		double num = (double)garbageLevel * 5.0 / 100.0;
		if (garbageLevel <= 2)
		{
			num = 0.0;
		}
		return (int)((double)baseStoreAttractiveness * num);
	}

	public List<GameItem> StartInspection()
	{
		List<GameItem> list = new List<GameItem>();
		int value = GenerateContrabandFineFromItem(list);
		RemoveItemFromStoreInventory(list);
		PaidOrForeclose(value);
		return list;
	}

	public void PaidOrForeclose(int value)
	{
		if (CanPay(value))
		{
			PayMoney(value, playRegisterSound: true);
			return;
		}
		int foreclosedValue = value - playerCash;
		playerCash = 0;
		List<GameItem> foreclosedItem = GetForeclosedItem(foreclosedValue);
		ForecloseItems(foreclosedItem);
	}

	public void RemoveItemFromStoreInventory(List<GameItem> gameItems)
	{
		GraphUtils.TryDestroyAll(gameItems);
	}

	public List<GameItem> GetForeclosedItem(int foreclosedValue)
	{
		int num = 100 / foreclosePercentage;
		return SpendBudgetGreedy(gridInv.childItems, num * foreclosedValue);
	}

	public List<GameItem> SpendBudgetGreedy(List<GameItem> items, int value)
	{
		items = items.OrderBy((GameItem item) => item.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L)).ToList();
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem item in items)
		{
			if (value >= item.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L))
			{
				list.Add(item);
				value -= (int)item.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L);
			}
		}
		return list;
	}

	public void ForecloseItems(List<GameItem> items)
	{
		RemoveItemFromStoreInventory(items);
	}

	public void GetLoan()
	{
	}

	public void PaidLoan()
	{
	}

	public int GetClientInQueueCount()
	{
		return storeClientManager.clientStack.Count;
	}

	public GameGridInventory GetStoreInventory()
	{
		return gridInv;
	}

	public List<GameItem> CheckInventoryWithTag(string tag)
	{
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem childItem in gridInv.childItems)
		{
			if (childItem.IsTag(tag) && !list.Contains(childItem))
			{
				list.Add(childItem);
			}
		}
		return list;
	}

	public List<GameItem> CheckInventoryWithTag(List<string> tags)
	{
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem childItem in gridInv.childItems)
		{
			foreach (string tag in tags)
			{
				if (childItem.IsTag(tag) && !list.Contains(childItem))
				{
					list.Add(childItem);
				}
			}
		}
		return list;
	}

	public bool CanPay(int price)
	{
		if (price <= playerCash)
		{
			return true;
		}
		return false;
	}

	public int GenerateContrabandFineFromItem(List<GameItem> foundContraband)
	{
		long num = 0L;
		foreach (GameItem item in foundContraband)
		{
			num += item.GetNegociatedValue();
		}
		int num2 = 100 / contrabandFinePercentage;
		return (int)(num / num2);
	}

	public void PayMoney(int amount, bool playRegisterSound = false)
	{
		playerCash -= amount;
		if (playRegisterSound)
		{
			EmporiumAudioPlayer.Instance.PlayRegister();
		}
	}

	public int GetCash()
	{
		return playerCash;
	}

	public void AddClientSellingItemToTable()
	{
	}

	public void AddDirectSellingItemToTable(string identifier, bool isOwend = false, bool isStolen = false, bool ignorePerk = false, int forcedHeat = 0)
	{
		TryAddDirectSellingItemToTable(identifier, isOwend, isStolen, ignorePerk, forcedHeat);
	}

	public bool TryAddDirectSellingItemToTable(string identifier, bool isOwend = false, bool isStolen = false, bool ignorePerk = false, int forcedHeat = 0)
	{
		if (EmporiumEntry.Instance != null && EmporiumEntry.Instance.frontInvinvElement != null && !EmporiumEntry.Instance.frontInvinvElement.CanTakeAnything())
		{
			GraphUtils.ReportUnplaced(identifier, 1, "front counter full, offer skipped");
			return false;
		}
		return TryAddDirectSellingItemToTable(DirectoryMaster.Item(identifier), isOwend, isStolen, ignorePerk, forcedHeat);
	}

	public void AddDirectSellingItemToTable(GameItem gameItem, bool isOwend = false, bool isStolen = false, bool ignorePerk = false, int forcedHeat = 0)
	{
		TryAddDirectSellingItemToTable(gameItem, isOwend, isStolen, ignorePerk, forcedHeat);
	}

	public bool TryAddDirectSellingItemToTable(GameItem gameItem, bool isOwend = false, bool isStolen = false, bool ignorePerk = false, int forcedHeat = 0)
	{
		if (gameItem == null)
		{
			Debug.LogWarning("AddDirectSellingItemToTable called with null item (identifier not registered in this build variant?)");
			return false;
		}
		if (!ignorePerk && StoreReputation.IsPerkUnlocked("LLDISTRUSTED") && currentClientInstance != null && currentClientInstance.GetClientBlueprint().clientFaction == "FACTION_LOWER_LEVEL" && ContrabandHelper.IsContraband(gameItem))
		{
			GraphUtils.DestroyIfUnplaced(gameItem, "distrusted faction, contraband offer withheld");
			return false;
		}
		if (!ignorePerk && StoreReputation.IsPerkUnlocked("BM_DISTRUSTED") && currentClientInstance != null && currentClientInstance.GetClientBlueprint().clientFaction == "FACTION_BLACK_MARKET" && ContrabandHelper.IsContraband(gameItem))
		{
			GraphUtils.DestroyIfUnplaced(gameItem, "distrusted faction, contraband offer withheld");
			return false;
		}
		GeneralHelper.SetItemOwned(gameItem, isOwend);
		if (isStolen && isStolenItemIntroduced)
		{
			StolenHelper.InitStolenItem(gameItem, forcedHeat);
		}
		if (!GraphUtils.TryAcceptAllOrDestroy(EmporiumEntry.Instance.frontInvinvElement, gameItem, "front counter full"))
		{
			return false;
		}
		StartingPerkContent.HandleGamblingAddictionItemAdded(gameItem);
		return true;
	}

	public void AddDirectSellingItemToTableMiddle(GameItem gameItem, bool isOwend = false)
	{
		GeneralHelper.SetItemOwned(gameItem, isOwend);
		GraphUtils.TryAcceptAllMid(EmporiumEntry.Instance.frontInvinvElement, gameItem);
		GraphUtils.DestroyIfUnplaced(gameItem, "front counter full");
	}

	public void AddDirectToWeightedTable(GameItem gameItem, bool isOwend = false)
	{
		GeneralHelper.SetItemOwned(gameItem, isOwend);
		GraphUtils.TryAcceptAllOrDestroy(EmporiumEntry.Instance.backInvinvElement, gameItem, "back counter full");
	}

	public void RemoveClientSellingItemFromTable()
	{
	}

	public bool CanBuyItemFromTable()
	{
		if (currentClientInstance.GetClientBlueprint().isRegularSellDisable)
		{
			return false;
		}
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count == 0)
		{
			return false;
		}
		int num = (int)currentNegociatedItem.GetNegociatedValue();
		if (playerCash >= num)
		{
			return true;
		}
		return false;
	}

	public bool CanSellThisItem(GameItem gameItem, bool ignoreBudget = false)
	{
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count == 0)
		{
			return false;
		}
		StoreClient clientBlueprint = instance.currentClientInstance.GetClientBlueprint();
		if (!clientBlueprint.IsClientBuyingThisItem(gameItem))
		{
			return false;
		}
		if (clientBlueprint.CanClientExposeAnyFeatureDeep(gameItem))
		{
			Debug.Log("Can sell item");
			return false;
		}
		if (!ignoreBudget && clientBlueprint.useClientBudget && !clientBlueprint.HasBudgetLeftToBuy(gameItem))
		{
			return false;
		}
		if (clientBlueprint.isRequireVariety)
		{
			if (clientBlueprint.IsItemInVarietyList(gameItem))
			{
				return false;
			}
			return true;
		}
		return true;
	}

	public bool CanSellItemFromTable(bool ignoreBudget = false)
	{
		return CanSellThisItem(currentNegociatedItem, ignoreBudget);
	}

	public void BuyItemFromTable()
	{
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count != 0 && CanBuyItemFromTable())
		{
			BuyItem(currentNegociatedItem);
		}
	}

	public void BuyItem(GameItem item)
	{
		instance.currentClientInstance.GetClientBlueprint();
		int num = 0;
		num += (int)item.GetNegociatedValue();
		OnItemBought(item, num);
		playerCash -= num;
		MoveDealItemToInventory(item);
		if (item.IsRootedInBackInv() && (item.parentInventory == null || item.parentInventory.GetParentItem() == null))
		{
			item.RemoveFeatureRelatedToEvent();
			ItemFeatureList.RemovePerfumeFeatures(item);
			item.OnAddedToWeightedArea(ignoreOnItemAddedToWeighted: true);
		}
	}

	public void MoveDealItemToInventory(GameItem item)
	{
		if (EmporiumOption.MoveBoughtItemsToInventory)
		{
			if (MapUIManager.Instance.isOutside)
			{
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.afterhourInventory, item);
			}
			else
			{
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, item);
			}
		}
	}

	public void OnItemBought(GameItem gameItem, int cost)
	{
		gameItem.DisableTag("ALREADY_CONFRONTED_TAG");
		StoreClient clientBlueprint = instance.currentClientInstance.GetClientBlueprint();
		if (clientBlueprint != null)
		{
			clientBlueprint.OnItemSold(gameItem, cost);
			StoreReputation.OnItemTraded(clientBlueprint.clientFaction, cost, cost, clientBlueprint, isPlayerSold: false, gameItem);
		}
		else
		{
			Debug.Log("ERROR: Client was null when bought item");
		}
		GeneralHelper.TrackBuyPrice(gameItem, cost);
		RunStats.OnItemBought(gameItem, cost, clientBlueprint);
		if (gameItem.IsGameItemType("MACHINE") || gameItem.IsGameItemType("MODULE") || gameItem.IsGameItemType("NODE"))
		{
			machineryBoughtCount++;
			EmporiumAnalytics.TrackItemOfInterestAcquisition(gameItem);
		}
		GeneralHelper.SetItemOwned(gameItem);
		gameItem.RemoveFeatureRelatedToEvent();
		gameItem.RemoveFeatureTemporaryBuying();
	}

	public void BuyAllItemFromTable()
	{
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count == 0 || !CanBuyItemFromTable())
		{
			return;
		}
		int num = 0;
		foreach (GameItem childItem in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			GeneralHelper.SetItemOwned(childItem);
			num += (int)childItem.GetNegociatedValue();
			playerCash -= num;
		}
	}

	public void SellItem(GameItem item)
	{
		StoreClient storeClient = null;
		if (instance.currentClientInstance != null)
		{
			storeClient = instance.currentClientInstance.GetClientBlueprint();
		}
		if (storeClient != null && storeClient.isRequireVariety)
		{
			storeClient.TrackItemVariety(item);
		}
		PerformOnSoldCheck(item, storeClient);
		int num = 0;
		num += (int)item.GetNegociatedValue();
		int value = num;
		if (storeClient != null)
		{
			if (storeClient.useClientBudget)
			{
				if (storeClient.GetBudget() < num)
				{
					num = storeClient.GetBudget();
				}
				storeClient.ModBudget(-num);
				if (storeClient.GetBudget() < 0)
				{
					storeClient.OverrideBudget(0);
				}
			}
			storeClient.OnItemBought(item, num);
		}
		playerCash += num;
		if (storeClient != null)
		{
			StoreReputation.OnItemTraded(storeClient.clientFaction, value, num, storeClient, isPlayerSold: true, item);
		}
		int boughtFor = (item.IsTag("BOUGHT_PRICE_TAG") ? item.GetTagReadonly("BOUGHT_PRICE_TAG").GetInt() : 0);
		StallWatchdog.Crumb("sold " + item.identifier + " for " + num);
		RunStats.OnItemSold(item, num, boughtFor, storeClient);
		GeneralHelper.SetItemOwned(item, own: false);
		SoldItem.TrackItem(item, storeClient, num);
	}

	public void PerformOnSoldCheck(GameItem item, StoreClient currentClient)
	{
		item.CloseContentWindow(includeChildren: true);
		List<GameItem> list = new List<GameItem> { item };
		foreach (GameItem item2 in item.FindAllChildItems())
		{
			if (!item2.IsTag("ITEM_BEHIND_LOCK_TAG"))
			{
				list.Add(item2);
			}
		}
		foreach (GameItem item3 in list)
		{
			StoreReputation.CheckSoldNaloxgone(item3);
			secData.CheckSoldContraband(item3, currentClient);
			secData.CheckSoldDirtyWater(item3, currentClient);
			secData.CheckSoldEndangermentItem(item3, currentClient);
			AmmoBoxHelper.CheckSoldAmmoBox(item3, currentClient);
		}
	}

	public void SellItemFromTable()
	{
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count == 0 || TryReinspectBeforeSale(currentNegociatedItem))
		{
			return;
		}
		if (CanSellItemFromTable())
		{
			NegociationUIManager.Instance.audioSourceCashRegister.Play();
			SellItem(currentNegociatedItem);
			StoreClient clientBlueprint = Instance.currentClientInstance.GetClientBlueprint();
			if (clientBlueprint.acceptDealDialogue == null)
			{
				return;
			}
			if (!clientBlueprint.acceptDealDialoguePlayed)
			{
				if (DialogUIManager.Instance.StartDialogue(clientBlueprint.acceptDealDialogue))
				{
					clientBlueprint.acceptDealDialoguePlayed = true;
				}
			}
			else
			{
				clientBlueprint.OnDealAccepted();
			}
		}
		else if (CanSellItemFromTable(ignoreBudget: true))
		{
			NegociationUIManager.Instance.OpenConfirmPanel();
		}
	}

	public bool TryReinspectBeforeSale(GameItem item)
	{
		if (item == null || currentClientInstance == null)
		{
			return false;
		}
		StoreClient clientBlueprint = currentClientInstance.GetClientBlueprint();
		if (!clientBlueprint.CanClientExposeAnyFeatureDeep(item))
		{
			return false;
		}
		clientBlueprint.InspectItemDeep(item);
		NegociationUIManager.Instance.UpdateUI(item);
		return true;
	}

	public void SellAllItemFromTable()
	{
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count != 0 && CanSellItemFromTable())
		{
			int num = 0;
			for (int num2 = EmporiumEntry.Instance.backInvinvElement.childItems.Count - 1; num2 >= 0; num2--)
			{
				GameItem gameItem = EmporiumEntry.Instance.backInvinvElement.childItems[num2];
				GeneralHelper.SetItemOwned(gameItem, own: false);
				gameItem.Destroy();
				num += (int)gameItem.GetNegociatedValue();
				playerCash += num;
			}
		}
	}

	public void AddedItemToClientArea(GameItem gameItem)
	{
		gameItem.OnAddedToClientArea();
		gameItem.AddFeatureRelatedToEvent();
	}

	public void RemovedItemToClientArea(GameItem gameItem)
	{
		if (currentNegociatedItem != null)
		{
			currentNegociatedItem.parentInventory?.ClearHighlight();
		}
		if (currentClientInstance != null && currentClientInstance.GetClientBlueprint().onItemRemovedToCounters != null)
		{
			currentClientInstance.GetClientBlueprint().onItemRemovedToCounters(gameItem);
		}
		gameItem.RemoveFeatureRelatedToEvent();
	}

	public void AddedItemToWeightedArea(GameItem gameItem)
	{
		currentNegociatedItem = gameItem;
		gameItem.OnAddedToWeightedArea();
		OnWeightedAreaChanged();
		TutorialUIManager.Instance.BuyItemDrag();
		HandleAutoNegoUIDisplay(gameItem);
	}

	public void AddedItemToDisplayCase(GameItem gameItem)
	{
		gameItem.OnAddedToShowcaseArea();
		BlackLampHelper.RefreshLampSprite(gameItem);
		ClienteleUIManager.Instance.RefreshUI();
		TutorialUIManager.Instance.OnItemAddedToDisplayCase();
		ShowDisplayCaseTimingTipOnce();
	}

	private void ShowDisplayCaseTimingTipOnce()
	{
		if (!displayCaseTimingTipShown && Difficulty.Rules.displayCaseTip && !IsRockBottomStart() && !(GenericConfirmationUIManager.Instance == null) && storeState == StoreState.OPEN)
		{
			displayCaseTimingTipShown = true;
			GenericConfirmationUIManager.Instance.OpenPanelSingleButton(LocHelper.GetLocalizedMechanic("mech_display_case_timing_tip"), LocHelper.GetLocalizedMainMenu("confirm"), delegate
			{
				GenericConfirmationUIManager.Instance.ClosePanel();
			});
		}
	}

	public void RemovedItemToDisplayCase(GameItem gameItem)
	{
		BlackLampHelper.RefreshLampSprite(gameItem);
		ClienteleUIManager.Instance.RefreshUI();
	}

	public void RemovedItemToWeightedArea(GameItem gameItem)
	{
		if (gameItem == currentNegociatedItem)
		{
			currentNegociatedItem = null;
			if (EmporiumEntry.Instance.backInvinvElement.childItems.Count != 0)
			{
				currentNegociatedItem = EmporiumEntry.Instance.backInvinvElement.childItems.Last();
			}
		}
		gameItem.OnRemovedFromWeightedArea();
		OnWeightedAreaChanged();
	}

	public void HandleAutoNegoUIDisplay(GameItem gameItem)
	{
		if (!EmporiumOption.AutoOpenNegotiationUI || currentClientInstance == null || !Instance.isClientArrived)
		{
			return;
		}
		if (GetCurrentClientIntent() == StoreClient.ClientIntent.BUY)
		{
			if (GeneralHelper.IsItemOwned(gameItem) && currentClientInstance.GetClientBlueprint().IsClientBuyingThisItem(gameItem))
			{
				NegociationUIManager.Instance.OpenUI();
			}
		}
		else if (GetCurrentClientIntent() == StoreClient.ClientIntent.SELL)
		{
			if (!GeneralHelper.IsItemOwned(gameItem))
			{
				NegociationUIManager.Instance.OpenUI();
			}
		}
		else if (GetCurrentClientIntent() == StoreClient.ClientIntent.SELLNBUY)
		{
			if (!GeneralHelper.IsItemOwned(gameItem))
			{
				NegociationUIManager.Instance.OpenUI();
			}
			else if (GeneralHelper.IsItemOwned(gameItem) && currentClientInstance.GetClientBlueprint().IsClientBuyingThisItem(gameItem))
			{
				NegociationUIManager.Instance.OpenUI();
			}
		}
		else if (GetCurrentClientIntent() == StoreClient.ClientIntent.INFORMATION_DEALER)
		{
			NegociationUIManager.Instance.OpenUI();
		}
		else if (GetCurrentClientIntent() == StoreClient.ClientIntent.BARTER)
		{
			NegociationUIManager.Instance.OpenUI();
		}
		else if (GetCurrentClientIntent() == StoreClient.ClientIntent.PROCUREMENT_OFFER)
		{
			NegociationUIManager.Instance.OpenUI();
		}
		else if (GetCurrentClientIntent() == StoreClient.ClientIntent.APPRAISAL_SERVICE && !GeneralHelper.IsItemOwned(gameItem))
		{
			NegociationUIManager.Instance.OpenUI();
		}
	}

	public void AddedItemToMainInvArea(GameItem gameItem)
	{
		if (gameItem.identifier.Equals("tutorial_page"))
		{
			TutorialUIManager.Instance.OnControlClick();
		}
		OnMainInvAreaChange();
	}

	public void RemovedItemToMainInvArea(GameItem gameItem)
	{
		OnMainInvAreaChange();
	}

	public StoreClient.ClientIntent GetCurrentClientIntent()
	{
		if (currentClientInstance != null)
		{
			return currentClientInstance.GetClientBlueprint().clientIntent;
		}
		return StoreClient.ClientIntent.UNDEFINED;
	}

	public void OnMainInvAreaChange()
	{
	}

	public void PlacedItemForSelling(GameItem targetItem)
	{
		StoreClient clientBlueprint = currentClientInstance.GetClientBlueprint();
		if (currentClientInstance.GetClientBlueprint().isRequireVariety && currentClientInstance.GetClientBlueprint().IsItemInVarietyList(targetItem))
		{
			NegociationUIManager.Instance.UpdateUI(targetItem);
			DialogUIManager.Instance.StartDialogue(StoreClientDialogList.CreateAlreadyBoughtDialog(clientBlueprint.displayName));
		}
		else if (clientBlueprint.noAcceptingContraband && ContrabandHelper.IsContraband(targetItem))
		{
			NegociationUIManager.Instance.UpdateUI(targetItem);
			DialogUIManager.Instance.StartDialogue(StoreClientDialogList.CreateNoContrabandDialog(clientBlueprint.displayName));
		}
		else if (currentClientInstance.GetClientBlueprint().IsClientBuyingThisItem(targetItem))
		{
			NegociationUIManager.Instance.UpdateUI(targetItem);
			foreach (GameItem inspectableChild in StoreClient.GetInspectableChildren(targetItem))
			{
				if (currentClientInstance.GetClientBlueprint().CanClientExposeAnyFeature(inspectableChild))
				{
					currentClientInstance.GetClientBlueprint().OnItemPlaced(inspectableChild);
				}
			}
			if (currentClientInstance.GetClientBlueprint().CanClientExposeAnyFeature(targetItem))
			{
				currentClientInstance.GetClientBlueprint().OnItemPlaced(targetItem);
			}
			else if (currentClientInstance.GetClientBlueprint().placeRightItemWhenSellingToDialogue != null)
			{
				currentClientInstance.GetClientBlueprint().placeRightItemWhenSellingToDialogue.priority = 4;
				DialogUIManager.Instance.StartDialogue(currentClientInstance.GetClientBlueprint().placeRightItemWhenSellingToDialogue);
			}
		}
		else
		{
			NegociationUIManager.Instance.UpdateUI(targetItem);
			if (currentClientInstance.GetClientBlueprint().placedWrongItemWhenSellingToDialogue != null && currentClientInstance.GetClientBlueprint().clientIntent != StoreClient.ClientIntent.SELLNBUY)
			{
				currentClientInstance.GetClientBlueprint().placedWrongItemWhenSellingToDialogue.priority = 4;
				DialogUIManager.Instance.StartDialogue(currentClientInstance.GetClientBlueprint().placedWrongItemWhenSellingToDialogue);
			}
		}
	}

	public void PlacedItemForBuying(GameItem targetItem)
	{
		NegociationUIManager.Instance.UpdateUI(targetItem);
	}

	public void PlacedItemForBarter(GameItem targetItem)
	{
		NegociationUIManager.Instance.UpdateUI();
		if (currentClientInstance.GetClientBlueprint().isBusinessComplete || !GeneralHelper.IsItemOwned(targetItem))
		{
			return;
		}
		string itemPlacedResponse = currentClientInstance.GetClientBlueprint().barterOffer.GetItemPlacedResponse();
		if (!string.IsNullOrEmpty(itemPlacedResponse))
		{
			Dialogue dialogue = new Dialogue();
			dialogue.priority = 4;
			dialogue.SetText(currentClientInstance.GetClientBlueprint().displayName, itemPlacedResponse);
			if (currentClientInstance.GetClientBlueprint().isMainDialogueStarted)
			{
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		}
	}

	public void OnWeightedAreaChanged()
	{
		NegociationUIManager.Instance.ClearUI();
		NotebookUIManager.Instance.UpdateUI();
		if (currentClientInstance == null)
		{
			return;
		}
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count >= 1)
		{
			GameItem gameItem = currentNegociatedItem;
			if (gameItem == null || gameItem.parentInventory != EmporiumEntry.Instance.backInvinvElement)
			{
				gameItem = (currentNegociatedItem = EmporiumEntry.Instance.backInvinvElement.childItems.Last());
			}
			StoreClient.ClientIntent clientIntent = currentClientInstance.GetClientBlueprint().clientIntent;
			if (GeneralHelper.IsItemOwned(gameItem) && (clientIntent == StoreClient.ClientIntent.BUY || clientIntent == StoreClient.ClientIntent.SELLNBUY))
			{
				PlacedItemForSelling(gameItem);
			}
			else if (!GeneralHelper.IsItemOwned(gameItem) && (clientIntent == StoreClient.ClientIntent.SELL || clientIntent == StoreClient.ClientIntent.SELLNBUY))
			{
				PlacedItemForBuying(gameItem);
			}
			else if (currentClientInstance.GetClientBlueprint().clientIntent == StoreClient.ClientIntent.BARTER)
			{
				PlacedItemForBarter(gameItem);
			}
			else if (currentClientInstance.GetClientBlueprint().clientIntent == StoreClient.ClientIntent.INFORMATION_DEALER)
			{
				NegociationUIManager.Instance.UpdateUI();
			}
			else if (currentClientInstance.GetClientBlueprint().clientIntent == StoreClient.ClientIntent.PROCUREMENT_OFFER)
			{
				NegociationUIManager.Instance.UpdateUI();
			}
			else if (currentClientInstance.GetClientBlueprint().clientIntent == StoreClient.ClientIntent.PROCUREMENT_COLLECT)
			{
				NegociationUIManager.Instance.UpdateUI();
				if (currentClientInstance.GetClientBlueprint().isBusinessComplete)
				{
					return;
				}
				BarterResult barterResult = currentClientInstance.GetClientBlueprint().procurementData.getBarterResult();
				if (barterResult.isAccepted)
				{
					Dialogue dialogue = new Dialogue();
					dialogue.priority = 4;
					dialogue.SetText(currentClientInstance.GetClientBlueprint().displayName, barterResult.response);
					if (currentClientInstance.GetClientBlueprint().isMainDialogueStarted)
					{
						DialogUIManager.Instance.StartDialogue(dialogue);
					}
				}
			}
			else if (currentClientInstance.GetClientBlueprint().clientIntent == StoreClient.ClientIntent.WHOLESALE)
			{
				NegociationUIManager.Instance.UpdateUI();
			}
			else if (currentClientInstance.GetClientBlueprint().clientIntent == StoreClient.ClientIntent.APPRAISAL_SERVICE)
			{
				NegociationUIManager.Instance.UpdateUI(gameItem);
			}
		}
		else if (EmporiumEntry.Instance.backInvinvElement.childItems.Count == 0)
		{
			NegociationUIManager.Instance.currentNegociatedItem = null;
			NegociationUIManager.Instance.ClearUI();
		}
	}

	public void ExamineWeightedItem()
	{
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count != 0)
		{
			ExamineItem(currentNegociatedItem);
		}
	}

	public void ExamineItem(GameItem item)
	{
	}

	public GameItem GetCurrentlyNegociatedItem()
	{
		return currentNegociatedItem;
	}

	public void AcceptBarter()
	{
		List<GameItem> list = EmporiumEntry.Instance.frontInvinvElement.childItems.Where((GameItem item) => !GeneralHelper.IsItemOwned(item)).ToList();
		currentClientInstance.GetClientBlueprint().CompleteBusiness();
		currentClientInstance.GetClientBlueprint().barterOffer.OnBarterAccepted();
		EmporiumEntry.Instance.TransferOwnershipFrontInv();
		EmporiumEntry.Instance.TransferOwnershipBackInv();
		EmporiumEntry.Instance.SwapInv();
		foreach (GameItem item in EmporiumEntry.Instance.backInvinvElement.childItems.ToList())
		{
			item.RemoveFeatureRelatedToEvent();
			item.RemoveFeatureTemporaryBuying();
		}
		foreach (GameItem item2 in list)
		{
			MoveDealItemToInventory(item2);
		}
	}

	public void OnItemsSold(List<GameItem> gameItems)
	{
		foreach (GameItem gameItem in gameItems)
		{
			secData.CheckSoldContraband(gameItem, currentClientInstance.GetClientBlueprint());
			secData.CheckSoldDirtyWater(gameItem, currentClientInstance.GetClientBlueprint());
			SoldItem.TrackItem(gameItem, currentClientInstance.GetClientBlueprint(), (int)gameItem.GetNegociatedValue());
		}
	}

	public int GetWholesalePrice()
	{
		if (!currentClientInstance.GetClientBlueprint().isOfferWholesale)
		{
			return -1;
		}
		int wholesaleDiscount = currentClientInstance.GetClientBlueprint().wholesaleDiscount;
		List<GameItem> allNonOwnedItem = EmporiumEntry.Instance.GetAllNonOwnedItem();
		int num = 0;
		foreach (GameItem item in allNonOwnedItem)
		{
			num += (int)item.GetNegociatedValue();
		}
		return RNG.CalculatePercentageOff(num, wholesaleDiscount);
	}

	public string CheckMostStockType()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (GameItem childItem in EmporiumEntry.Instance.invElement.childItems)
		{
			foreach (string item in childItem.GetGameItemType())
			{
				if (dictionary.ContainsKey(item))
				{
					dictionary[item]++;
				}
				else
				{
					dictionary[item] = 1;
				}
			}
		}
		foreach (GameItem childItem2 in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			foreach (string item2 in childItem2.GetGameItemType())
			{
				if (dictionary.ContainsKey(item2))
				{
					dictionary[item2]++;
				}
				else
				{
					dictionary[item2] = 1;
				}
			}
		}
		string result = null;
		int num = 0;
		foreach (KeyValuePair<string, int> item3 in dictionary)
		{
			if (item3.Value > num)
			{
				num = item3.Value;
				result = item3.Key;
			}
		}
		return result;
	}

	public string CheckMostStockTypeShowcase()
	{
		if (EmporiumEntry.Instance.showcaseElement.childItems.Count == 0)
		{
			return "";
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			foreach (string item in childItem.GetGameItemType())
			{
				if (dictionary.ContainsKey(item))
				{
					dictionary[item]++;
				}
				else
				{
					dictionary[item] = 1;
				}
			}
		}
		string result = "";
		int num = 0;
		foreach (KeyValuePair<string, int> item2 in dictionary)
		{
			if (item2.Value > num)
			{
				num = item2.Value;
				result = item2.Key;
			}
		}
		return result;
	}

	public List<KeyValuePair<string, int>> GetTypesByValue(List<GameItem> items, List<string> typesToInclude)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (GameItem item in items)
		{
			int num = (int)item.GetNegociatedValue();
			foreach (string item2 in item.GetGameItemType())
			{
				if (typesToInclude.Contains(item2))
				{
					if (dictionary.ContainsKey(item2))
					{
						dictionary[item2] += num;
					}
					else
					{
						dictionary[item2] = num;
					}
				}
				if (item2.Equals("WATER_RATION"))
				{
					if (dictionary.ContainsKey("WATER"))
					{
						dictionary["WATER"] += num;
					}
					else
					{
						dictionary["WATER"] = num;
					}
				}
				if (item2.Equals("ENERGY_CELL") && !item.IsGameItemType("MATERIAL") && typesToInclude.Contains("MATERIAL"))
				{
					if (dictionary.ContainsKey("MATERIAL"))
					{
						dictionary["MATERIAL"] += num;
					}
					else
					{
						dictionary["MATERIAL"] = num;
					}
				}
			}
		}
		return dictionary.OrderByDescending((KeyValuePair<string, int> kv) => kv.Value).ToList();
	}

	public List<KeyValuePair<string, int>> GetMarketModifierType(List<string> typesToInclude)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (StoreEvent activeEvent in StoreStation.Instance.storeEventManager.GetActiveEvents())
		{
			foreach (NegociationData negociationData in activeEvent.negociationDatas)
			{
				if (typesToInclude.Contains(negociationData.identifier))
				{
					dictionary[negociationData.identifier] += negociationData.value;
				}
			}
		}
		return dictionary.OrderByDescending((KeyValuePair<string, int> kv) => kv.Value).ToList();
	}

	public string CheckMostStockTypeMainInv()
	{
		if (EmporiumEntry.Instance.invElement.childItems.Count == 0)
		{
			return "";
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (GameItem childItem in EmporiumEntry.Instance.invElement.childItems)
		{
			foreach (string item in childItem.GetGameItemType())
			{
				if (dictionary.ContainsKey(item))
				{
					dictionary[item]++;
				}
				else
				{
					dictionary[item] = 1;
				}
			}
		}
		string result = "";
		int num = 0;
		dictionary.Remove("DOCUMENT");
		foreach (KeyValuePair<string, int> item2 in dictionary)
		{
			if (item2.Value > num)
			{
				num = item2.Value;
				result = item2.Key;
			}
		}
		return result;
	}

	public void ApplyReportFactionChange(StoreClient storeClient)
	{
		if (storeClient.reportImprovesFaction)
		{
			StoreReputation.ModReputation(storeClient.clientFaction, Math.Abs(storeClient.reputationPenalty));
		}
		else if (storeClient.reportDegradesFaction)
		{
			StoreReputation.ModReputation(storeClient.clientFaction, -Math.Abs(storeClient.reputationPenalty));
		}
	}

	public void GrantWantedCaptureReward(StoreClient storeClient, bool isShot = false)
	{
		int reportReward = storeClient.reportReward;
		reportReward = StartingPerkContent.HandleReportRewardConvict(reportReward);
		reportReward = StartingPerkContent.HandleReportRewardModelCitizen(reportReward);
		playerCash += reportReward;
		StoreReputation.ModSecReputation(StartingPerkContent.HandleReportRewardModelCitizen(StartingPerkContent.HandleReportRewardConvict(5)));
		GameItem gameItem = DirectoryMaster.Item("sec_slip");
		NegociationUIManager.Instance.audioSourceCashRegister.Play();
		wantedStatus[storeClient.wantedPosterId] = 1;
		if (WantedUIManager.Instance.gameObject.activeSelf)
		{
			WantedUIManager.Instance.OnArrested(storeClient.wantedPosterId);
		}
		AchievementTracker.Instance.CheckModelCitizen();
		string body = (isShot ? LocHelper.GetLocalizedMechanicOrDefault("mech_report_shot_wanted_body", "{0} Credits has been added to your account. {1} was neutralized at your counter and logged as a capture.", reportReward, storeClient.realName) : LocHelper.GetLocalizedMechanic("mech_report_wanted_body", reportReward, storeClient.realName));
		body += "\n\n";
		body += LocHelper.GetLocalizedMechanic("mech_report_rep_improved");
		gameItem.ModifyTag("title", delegate(TagState state)
		{
			state.Enable().SetString(LocHelper.GetLocalizedMechanic("mech_report_title"));
		});
		gameItem.ModifyTag("body", delegate(TagState state)
		{
			state.Enable().SetString(body);
		});
		Instance.AddDirectSellingItemToTable(gameItem, isOwend: true);
	}

	public void OnWantedClientShot(StoreClient storeClient)
	{
		ApplyReportFactionChange(storeClient);
		if (storeClient.onCaptureAction != null)
		{
			storeClient.onCaptureAction();
		}
		GrantWantedCaptureReward(storeClient, isShot: true);
	}

	public void CloseWantedPoster(StoreClient storeClient)
	{
		if (storeClient != null && storeClient.isWanted && !storeClient.isStealthWanted && (!wantedStatus.TryGetValue(storeClient.wantedPosterId, out var value) || value != 1))
		{
			wantedStatus[storeClient.wantedPosterId] = 1;
			if (WantedUIManager.Instance.gameObject.activeSelf)
			{
				WantedUIManager.Instance.OnArrested(storeClient.wantedPosterId);
			}
		}
	}

	public void CloseWantedPoster(string posterId)
	{
		if (!string.IsNullOrEmpty(posterId) && (!wantedStatus.TryGetValue(posterId, out var value) || value != 1))
		{
			wantedStatus[posterId] = 1;
			if (WantedUIManager.Instance.gameObject.activeSelf)
			{
				WantedUIManager.Instance.OnArrested(posterId);
			}
		}
	}

	public void OnCustomerReported()
	{
		if (currentClientInstance == null || StoreUIManager.Instance.isShot)
		{
			return;
		}
		StoreClient storeClient = currentClientInstance.GetClientBlueprint();
		if (!storeClient.CanReport())
		{
			return;
		}
		DialogUIManager.Instance.FlushScheduledEndActions();
		DialogUIManager.Instance.InterruptDialog();
		DialogUIManager.Instance.HideChoiceBubble();
		if (currentClientInstance == null || currentClientInstance.GetClientBlueprint() != storeClient || !storeClient.CanReport())
		{
			return;
		}
		Func<bool> tryInterceptReport = storeClient.tryInterceptReport;
		if (tryInterceptReport != null)
		{
			storeClient.tryInterceptReport = null;
			if (tryInterceptReport())
			{
				return;
			}
		}
		EmporiumAnalytics.TrackCustomerReported(storeClient);
		storeClient.reported = true;
		StoreUIManager.Instance.StoreGlass.ToggleShutterDoor();
		EmporiumAudioPlayer.Instance.alarm2.Play();
		if (storeClient.onReportAction != null)
		{
			storeClient.onReportAction();
		}
		DialogUIManager.Instance.InterruptDialog();
		DialogUIManager.Instance.HideChoiceBubble();
		if (storeClient.isAug)
		{
			Instance.DismissCurrentClient(callNextWhenPossible: false);
			StoreUIManager.Instance.isReportedAug = true;
			StoreUIManager.Instance.StartArrestSequence(2.5f);
			return;
		}
		DialogUIManager.Instance.StartDialogue(StoreClientDialogList.ClientAlarmInitialReactionDialog());
		if (storeClient.isWanted || (!storeClient.IsSellingContraband() && !storeClient.IsSellingStolen()))
		{
			ApplyReportFactionChange(storeClient);
		}
		string reportTitle = LocHelper.GetLocalizedMechanic("mech_report_title");
		if (storeClient.isWanted)
		{
			Debug.Log("Correct Report of Wanted Customers");
			if (storeClient.identifier == wantedPersonVisitedId)
			{
				correctReportFileToday = true;
			}
			onSecAlarmLeftAction = delegate
			{
				GrantWantedCaptureReward(storeClient);
			};
			if (storeClient.onCaptureAction != null)
			{
				storeClient.onCaptureAction();
			}
		}
		else if (storeClient.IsSellingContraband() || storeClient.IsSellingStolen())
		{
			Debug.Log("Correct Report of Selling Contraband/Stolen Customers");
			onSecAlarmLeftAction = CreateStolenReportReward(storeClient, storeClient.reportDegradesFaction);
			if (storeClient.onCaptureAction != null)
			{
				storeClient.onCaptureAction();
			}
		}
		else
		{
			Debug.Log("Wrongful Report");
			onSecAlarmLeftAction = delegate
			{
				StoreReputation.ModSecReputation(-8);
				GameItem gameItem = DirectoryMaster.Item("sec_slip");
				wrongfulReportCount++;
				RunStats.OnWrongfulReport();
				string body = LocHelper.GetLocalizedMechanic("mech_report_wrongful");
				if (playerCash >= 100)
				{
					playerCash -= 50;
					body += LocHelper.GetLocalizedMechanic("mech_report_fine", 50);
					NegociationUIManager.Instance.audioSourceCashRegister.Play();
				}
				if (wrongfulReportCount + 1 == wrongfulReportMax)
				{
					body += LocHelper.GetLocalizedMechanic("mech_report_next_arrest");
				}
				else if (wrongfulReportCount + 2 == wrongfulReportMax)
				{
					body += LocHelper.GetLocalizedMechanic("mech_report_severe_consequences");
				}
				body += "\n\n";
				body += LocHelper.GetLocalizedMechanic("mech_report_rep_decreased");
				gameItem.ModifyTag("title", delegate(TagState state)
				{
					state.Enable().SetString(reportTitle);
				});
				gameItem.ModifyTag("body", delegate(TagState state)
				{
					state.Enable().SetString(body);
				});
				Instance.AddDirectSellingItemToTable(gameItem, isOwend: true);
			};
		}
		StoreUIManager.Instance.StartArrestSequence();
	}

	public Action CreateStolenReportReward(StoreClient storeClient, bool degradeFaction)
	{
		int rewardAmount = ContrabandHelper.GetReportReward(EmporiumEntry.Instance.GetAllNonOwnedItem());
		int contrabandLevel = ContrabandHelper.GetHighestContrabandLevelInt(EmporiumEntry.Instance.GetAllNonOwnedItem());
		rewardAmount += 50;
		rewardAmount = StartingPerkContent.HandleReportRewardConvict(rewardAmount);
		rewardAmount = StartingPerkContent.HandleReportRewardModelCitizen(rewardAmount);
		contrabandLevel = StartingPerkContent.HandleReportRewardConvict(contrabandLevel);
		contrabandLevel = StartingPerkContent.HandleReportRewardModelCitizen(contrabandLevel);
		string reportTitle = LocHelper.GetLocalizedMechanic("mech_report_title");
		return delegate
		{
			playerCash += rewardAmount;
			StoreReputation.ModSecReputation(contrabandLevel);
			if (degradeFaction)
			{
				StoreReputation.ReportedClient(storeClient.clientFaction);
			}
			GameItem gameItem = DirectoryMaster.Item("sec_slip");
			NegociationUIManager.Instance.audioSourceCashRegister.Play();
			string key = ((contrabandLevel == 0) ? "mech_report_thief_body" : "mech_report_contraband_body");
			string body = LocHelper.GetLocalizedMechanic(key, rewardAmount);
			body += "\n\n";
			body += LocHelper.GetLocalizedMechanic("mech_report_rep_slightly_improved");
			if (degradeFaction && StoreReputation.IsFactionValid(storeClient.clientFaction))
			{
				body += LocHelper.GetLocalizedMechanic("mech_report_faction_degraded", StoreReputation.GetFactionNameByID(storeClient.clientFaction));
			}
			gameItem.ModifyTag("title", delegate(TagState state)
			{
				state.Enable().SetString(reportTitle);
			});
			gameItem.ModifyTag("body", delegate(TagState state)
			{
				state.Enable().SetString(body);
			});
			Instance.AddDirectSellingItemToTable(gameItem, isOwend: true);
			AchievementTracker.Instance.CheckModelCitizen();
		};
	}

	public bool ArrestCurrentClientForStolenGoods()
	{
		if (currentClientInstance == null || StoreUIManager.Instance.isShot || isClientBeingArrested)
		{
			return false;
		}
		StoreClient clientBlueprint = currentClientInstance.GetClientBlueprint();
		DialogUIManager.Instance.FlushScheduledEndActions();
		DialogUIManager.Instance.InterruptDialog();
		DialogUIManager.Instance.HideChoiceBubble();
		if (currentClientInstance == null || currentClientInstance.GetClientBlueprint() != clientBlueprint)
		{
			return false;
		}
		clientBlueprint.arrestable = false;
		onSecAlarmLeftAction = CreateStolenReportReward(clientBlueprint, degradeFaction: false);
		StoreUIManager.Instance.StartArrestSequence();
		return true;
	}

	public int GetTotalEstimatedValue()
	{
		int num = 0;
		foreach (GameItem childItem in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem))
			{
				num += (int)childItem.GetNegociatedValue();
			}
		}
		foreach (GameItem childItem2 in EmporiumEntry.Instance.frontInvinvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem2))
			{
				num += (int)childItem2.GetNegociatedValue();
			}
		}
		foreach (GameItem childItem3 in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem3))
			{
				num += (int)childItem3.GetNegociatedValue();
			}
		}
		foreach (GameItem childItem4 in EmporiumEntry.Instance.invElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem4))
			{
				num += (int)childItem4.GetNegociatedValue();
			}
		}
		foreach (GameItem childItem5 in EmporiumEntry.Instance.docInvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem5))
			{
				num += (int)childItem5.GetNegociatedValue();
			}
		}
		foreach (GameItem childItem6 in EmporiumEntry.Instance.trashcanInvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem6))
			{
				num += (int)childItem6.GetNegociatedValue();
			}
		}
		foreach (GameItem childItem7 in EmporiumEntry.Instance.vendingFountainElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem7))
			{
				num += (int)childItem7.GetNegociatedValue();
			}
		}
		foreach (GameItem childItem8 in EmporiumEntry.Instance.vendingMachineElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem8))
			{
				num += (int)childItem8.GetNegociatedValue();
			}
		}
		return num + playerCash;
	}

	public static void ResetData()
	{
		instance = null;
	}

	public static bool IsHardMode()
	{
		return Difficulty.IsHardOrAbove();
	}

	public void HandleSkipIntro()
	{
		if (NewGameData.Instance != null)
		{
			skipIntro = NewGameData.Instance.skipIntro;
		}
		if (!skipIntro)
		{
			return;
		}
		Debug.Log("Skiping Intro");
		isAppraisalIntroduced = false;
		isContrabandIntroduced = true;
		isStolenItemIntroduced = true;
		isDisplayCaseIntroduced = true;
		isNewspaperUnlocked = true;
		isVisualInspectionIntroduced = true;
		isStampIntroduced = !StartingPerk.IsBannedCommissary();
		isSecurityRecordIntroduced = true;
		isAfterhoursIntroduced = true;
		isBargainIntroduced = true;
		isInjectorIntroduced = true;
		storeClientData.isJacksonIntroduced = true;
		StorePhoneClient wildPhoneClient = StorePhoneClient.GetWildPhoneClient();
		wildPhoneClient.phoneState = StorePhoneClient.PhoneState.Regular;
		wildPhoneClient.dialedBefore = true;
		isNetworkIntroduced = true;
		tutorialEnable = false;
		enableStoryClient = false;
		StoreService.GetJanitorialService().introduced = true;
		StoreService.GetJanitorialService().unlocked = true;
		StoreService.GetInsuranceService().introduced = true;
		StoreService.GetInsuranceService().unlocked = true;
		GameItem gameItem = DirectoryMaster.Item("tutorial_book");
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNoteBasicTrading());
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNoteFood());
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNote13());
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNote2());
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNote21());
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNoteContraband());
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNoteInspection());
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNoteDisplayCase());
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNoteVisualInspection());
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNoteAfterhours());
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNoteStolenItem());
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNoteUpgrade());
		bool num = !StartingPerk.IsBannedCommissary();
		if (num)
		{
			GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNoteCommissary());
		}
		GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateLabelerNote());
		if (num)
		{
			GraphUtils.TryAcceptAll(gameItem, IntelHelper.CreateMentorNoteStamp());
		}
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.dossier, gameItem);
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.dossier, DirectoryMaster.Item("joe_card"));
		if (!StartingPerk.IsKnownSnitch())
		{
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.dossier, DirectoryMaster.Item("wanted_paper"));
		}
		if (!IsRockBottomStart())
		{
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("magnifier"));
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("labeler"));
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("topical_bandage_item"));
			if (!IsJunkerStart())
			{
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("fanny_pack"));
			}
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.dossier, DirectoryMaster.Item("cigarette_guide"));
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.dossier, DirectoryMaster.Item("cigarette_color"));
			if (!StartingPerk.IsSentinel())
			{
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.dossier, DirectoryMaster.Item("injector_guide"));
			}
			if (!StartingPerk.IsBannedCommissary())
			{
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.dossier, DirectoryMaster.Item("stamp_guide"));
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.dossier, DirectoryMaster.Item("logo_checker"));
			}
		}
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.dossier, DirectoryMaster.Item("intel_sec"));
		StoreStation.Instance.storeEventManager.HandleNewEventComestic();
		StoreStation.Instance.storeEventManager.HandleNewEventComestic();
		if (StoreStation.Instance.storeEventManager.GetActiveEvents().Count == 2)
		{
			StoreStation.Instance.storeEventManager.HandleNewEventComestic();
		}
	}

	public static bool IsInstanceExist()
	{
		if (instance == null)
		{
			return false;
		}
		return true;
	}

	public int GetShowcaseTypeVisitorCount()
	{
		int num = 1;
		if (StoreService.GetBuyReferalService().isUsing)
		{
			num++;
		}
		if (NetworkUpgrade.IsShowcaseIUnlocked())
		{
			num++;
			if (instance.isVandalized)
			{
				num--;
			}
		}
		if (NetworkUpgrade.IsShowcaseIIUnlocked())
		{
			num++;
			if (instance.isVandalized)
			{
				num--;
			}
		}
		if (instance.isVandalized)
		{
			num--;
		}
		if (num < 1)
		{
			num = 1;
		}
		return num;
	}

	public void ModCash(int value, bool playSound = true)
	{
		playerCash += value;
		if (playSound)
		{
			NegociationUIManager.Instance.audioSourceCashRegister.Play();
		}
	}

	public void Reset()
	{
		currentClientInstance = null;
		lockAllFlag = 0;
		lockClientClickFlag = 0;
		lockStorePhysicalFlag = 0;
		lockAllInventoryFlag = 0;
		lockNextBellFlag = 0;
		lockShutterButtonFlag = 0;
	}

	public void ClientFlagUp()
	{
		lockNextBellFlag++;
		Debug.Log("<color=orange>ClientFlagUp</color>");
	}

	public void ClientFlagDown()
	{
		if (lockNextBellFlag <= 0)
		{
			Debug.LogWarning("<color=green>ClientFlagDown</color> called at " + lockNextBellFlag + ", clamping to 0");
			lockNextBellFlag = 0;
		}
		else
		{
			lockNextBellFlag--;
			Debug.Log("<color=green>ClientFlagDown</color>");
		}
	}

	public bool IsCurrentClientAcceptMarkup()
	{
		if (currentClientInstance == null)
		{
			return false;
		}
		if (Difficulty.Rules.retailMarkup <= 0)
		{
			return false;
		}
		return currentClientInstance.GetClientBlueprint().acceptRetailMarkup;
	}

	public bool IsStartType(NewGameData.StartType startType)
	{
		return this.startType == startType;
	}

	public int GetStoreBuyPrice(bool applyDownPayment = false)
	{
		if (applyDownPayment)
		{
			return GetStoreBuyPrice() - GetMortgageDownPayment();
		}
		DifficultyRules rules = Difficulty.Rules;
		int propertyBasePrice = rules.propertyBasePrice;
		int num = 0;
		if (StoreStation.Instance.dayCounter > 50)
		{
			num = (StoreStation.Instance.dayCounter - 50) * rules.propertyPriceIncreasePerDay;
			if (num > rules.propertyPriceIncreaseCap)
			{
				num = rules.propertyPriceIncreaseCap;
			}
		}
		int num2 = 0;
		return propertyBasePrice + num - num2;
	}

	public int GetMortgageDownPayment()
	{
		int val = Mathf.RoundToInt((float)GetStoreBuyPrice() * Difficulty.Rules.mortgageDownPaymentShare / 500f) * 500;
		return Math.Max(500, val);
	}

	public static int GetMortgageWeeks()
	{
		return Math.Max(1, Difficulty.Rules.mortgageWeeks);
	}

	public int GetMortgageWeeklyPaymentPreview()
	{
		return CalculateWeeklyPayment(GetStoreBuyPrice(applyDownPayment: true), Difficulty.Rules.mortgageInterestRate, GetMortgageWeeks());
	}

	public bool CanBeOfferedMortgage()
	{
		return playerCash >= 2 * GetMortgageDownPayment();
	}

	public static int CalculateWeeklyPayment(int loanAmount, int annualInterestRate, int loanDurationInWeeks)
	{
		int num = loanAmount * annualInterestRate / 100 / 52 * loanDurationInWeeks;
		int num2 = loanAmount + num;
		int num3 = num2 / loanDurationInWeeks;
		if (num2 % loanDurationInWeeks != 0)
		{
			num3++;
		}
		return num3;
	}

	public bool CanBuyOutStore()
	{
		if (instance.GetCash() >= GetStoreBuyPrice() && !IsPropertyPaid)
		{
			return true;
		}
		return false;
	}

	public bool IsPayingRent()
	{
		if (isPayingMortgage || IsPropertyPaid)
		{
			return false;
		}
		return true;
	}

	public void BuyOutStore()
	{
		int storeBuyPrice = GetStoreBuyPrice();
		playerCash -= storeBuyPrice;
		RunStats.OnStoreBoughtWithCash(storeBuyPrice);
		IsPropertyPaid = true;
		EmporiumAudioPlayer.Instance.PlayRegister();
		storeClientManager.ReplaceLoanSharkWithRandomClient();
		OnStoreOwned();
	}

	public void OnStoreOwned()
	{
		AchievementTracker.Instance.CheckBuyout();
		AchievementTracker.Instance.CheckHonestMerchant();
		AchievementTracker.Instance.CheckRagToRiches();
		AchievementTracker.Instance.CheckNothingToRiches();
		AchievementTracker.Instance.CheckProdigious();
		QueueStoreOwnedVisits();
	}

	public void QueueStoreOwnedVisits()
	{
		storeClientManager.AddNextClient(StoreClientListStory.CreateWildDeedVisit());
	}

	public void TakeMortgage()
	{
		EmporiumAudioPlayer.Instance.PlayRegister();
		int mortgageDownPayment = GetMortgageDownPayment();
		int storeBuyPrice = GetStoreBuyPrice(applyDownPayment: true);
		int num = (mortgageTotalWeeks = (mortgageWeekLeft = GetMortgageWeeks()));
		mortgageDownPaid = mortgageDownPayment;
		mortgageInterestRateTaken = Difficulty.Rules.mortgageInterestRate;
		isPayingMortgage = true;
		playerCash -= mortgageDownPayment;
		mortgageWeeklyPayment = CalculateWeeklyPayment(storeBuyPrice, mortgageInterestRateTaken, num);
		propertyBoughtAt = storeBuyPrice;
		RunStats.OnMortgageTaken(mortgageDownPayment + storeBuyPrice, mortgageDownPayment, mortgageWeeklyPayment, num);
	}

	public void OnMortgagePaidOff()
	{
		isPayingMortgage = false;
		IsPropertyPaid = true;
		RunStats.OnMortgagePaidOff();
		OnStoreOwned();
	}

	public void PayWeeklyMortgage()
	{
		dayUntilRent = 7;
		EmporiumAudioPlayer.Instance.PlayRegister();
		mortgageWeekLeft--;
		playerCash -= mortgageWeeklyPayment;
		RunStats.OnMortgagePayment(mortgageWeeklyPayment, playerCash);
	}

	public void CheckCrime()
	{
		secData.dirtyWaterTracker -= 5;
		secData.mislabeledWaterTracker -= 5;
		secData.bribeTracker -= 5;
		if (secData.dirtyWaterTracker < 0)
		{
			secData.dirtyWaterTracker = 0;
		}
		if (secData.mislabeledWaterTracker < 0)
		{
			secData.mislabeledWaterTracker = 0;
		}
		if (secData.bribeTracker < 0)
		{
			secData.bribeTracker = 0;
		}
		if (secData.dirtyWaterTracker >= 2500)
		{
			var (num, dirtyWaterTracker) = RNG.ModuloWithCount(secData.dirtyWaterTracker, 500);
			secData.CommitCrime("ENDANGERMENT", num * 20);
			secData.dirtyWaterTracker = dirtyWaterTracker;
			Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRandomGhostwaterThreatNote(), isOwend: true);
		}
		if (secData.mislabeledWaterTracker >= 2500)
		{
			var (num2, mislabeledWaterTracker) = RNG.ModuloWithCount(secData.mislabeledWaterTracker, 500);
			secData.CommitCrime("FRAUD", num2 * 20);
			secData.mislabeledWaterTracker = mislabeledWaterTracker;
			Debug.Log("Mislabeled Water Crime Committed");
		}
		if (secData.badChemTrackerFraud >= 300)
		{
			var (amount, badChemTrackerFraud) = RNG.ModuloWithCount(secData.badChemTrackerFraud, 300);
			secData.CommitCrime("FRAUD", amount);
			secData.badChemTrackerFraud = badChemTrackerFraud;
		}
		if (secData.badChemTrackerEndangerment >= 300)
		{
			var (amount2, badChemTrackerEndangerment) = RNG.ModuloWithCount(secData.badChemTrackerEndangerment, 300);
			secData.CommitCrime("ENDANGERMENT", amount2);
			secData.badChemTrackerEndangerment = badChemTrackerEndangerment;
		}
		if (secData.endangermentItemTracker >= 300)
		{
			var (amount3, endangermentItemTracker) = RNG.ModuloWithCount(secData.endangermentItemTracker, 300);
			secData.CommitCrime("ENDANGERMENT", amount3);
			secData.endangermentItemTracker = endangermentItemTracker;
		}
		if (secData.badAmmoTrackerLegal >= 300)
		{
			secData.badAmmoTrackerLegal -= 300;
			secData.CommitCrime("FRAUD", 300);
		}
		if (secData.badAmmoTracker >= 500)
		{
			secData.badAmmoTracker = 0;
			Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRandomAmmoThreatNote(), isOwend: true);
		}
		if (secData.bribeTracker >= 250)
		{
			secData.bribeTracker -= 250;
			secData.CommitCrime("BRIBERY", 250);
		}
	}

	public bool IsJunkerStart()
	{
		return startType == NewGameData.StartType.SCAV;
	}

	public bool IsFenceStart()
	{
		return startType == NewGameData.StartType.FENCE;
	}

	public bool IsWaterMerchantStart()
	{
		return startType == NewGameData.StartType.WATER_MERCHANT;
	}

	public bool IsFarmerStart()
	{
		return startType == NewGameData.StartType.FARMER;
	}

	public bool IsGunsmithStart()
	{
		return startType == NewGameData.StartType.GUNSMITH;
	}

	public bool IsChemistStart()
	{
		return startType == NewGameData.StartType.DRUG_DEALER;
	}

	public bool IsRockBottomStart()
	{
		return startType == NewGameData.StartType.ROCK_BOTTOM;
	}

	public bool IsAquafarmerStart()
	{
		return startType == NewGameData.StartType.AQUAFARMER;
	}

	public bool IsRancherStart()
	{
		return startType == NewGameData.StartType.RANCHER;
	}

	public void AddNightLog(string log, string hexColor)
	{
		if (!hexColor.StartsWith("#"))
		{
			hexColor = "#" + hexColor;
		}
		string text = hexColor.TrimStart('#');
		nightLogs.Add("<color=#" + text + ">" + log + "</color>");
	}

	public void AddNightLog(string log)
	{
		nightLogs.Add(log);
	}

	public static StoreClientData GetStoreClientData()
	{
		if (instance == null)
		{
			return null;
		}
		return instance.storeClientData;
	}

	public bool HasCredit(int amount)
	{
		if (playerCash >= amount)
		{
			return true;
		}
		return false;
	}

	public void CheckSkinningKnifeReward()
	{
		if (animalsButcheredTotal >= 100 && !isSkinningKnifeRewardGiven)
		{
			isSkinningKnifeRewardGiven = true;
			AddDirectSellingItemToTable(ItemSpawner.Spawn("skinning_knife"), isOwend: true);
		}
	}

	public void CheckButcherCleaver()
	{
		if (!storeClientData.isButcherCleaverLeft && storeClientData.butcherVandalismMorningDay >= 0 && StoreStation.Instance.dayCounter >= storeClientData.butcherVandalismMorningDay + StoreClientData.ButcherDelay(3))
		{
			storeClientData.isButcherCleaverLeft = true;
			storeClientData.butcherCleaverDay = StoreStation.Instance.dayCounter;
			AddDirectSellingItemToTable(ItemSpawner.Spawn("kitchen_cleaver"), isOwend: true);
			AddDirectSellingItemToTable(StoreClientListWanted.CreateButcherNote(), isOwend: true);
		}
	}

	public bool IsPlayerHaveMachine()
	{
		List<GameItem> allOwnedItemsByTag = EmporiumEntry.Instance.GetAllOwnedItemsByTag("STANDARD_MACHINE_TAG");
		Debug.Log("Player have " + allOwnedItemsByTag.Count + " standard machines.");
		return allOwnedItemsByTag.Count > 0;
	}

	public void RefreshCounterItem()
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.frontInvinvElement.childItems)
		{
			childItem.RemoveFeatureRelatedToEvent();
			ItemFeatureList.RemovePerfumeFeatures(childItem);
			childItem.OnAddedToWeightedArea(ignoreOnItemAddedToWeighted: true);
		}
		foreach (GameItem childItem2 in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			childItem2.RemoveFeatureRelatedToEvent();
			ItemFeatureList.RemovePerfumeFeatures(childItem2);
			childItem2.OnAddedToWeightedArea(ignoreOnItemAddedToWeighted: true);
		}
	}

	public int GetAppraisalJoinCost()
	{
		return 85 * Difficulty.Rules.appraisalJoinCostMultiplier;
	}
}

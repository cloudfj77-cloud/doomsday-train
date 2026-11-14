using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoreClientManager
{
	[ES3NonSerializable]
	public List<StoreClient> clientStack;

	[ES3NonSerializable]
	public List<StoreClient> chemicalFeedbackStack;

	[ES3Serializable]
	public Dictionary<string, int> clientTracker;

	[ES3Serializable]
	public Dictionary<string, int> permanentClientTracker;

	[ES3Serializable]
	public Dictionary<string, int> uniqueClientTracker;

	[ES3Serializable]
	public Dictionary<string, int> spriteLastShownDay;

	[ES3Serializable]
	public List<string> spriteShownHistory;

	[ES3Serializable]
	public List<string> arrestedClients;

	[ES3Serializable]
	public List<string> possibleRandomWantedClient;

	[ES3Serializable]
	public bool isTodayWantedQueued;

	[ES3Serializable]
	public int daySinceWantedWasQueued;

	[ES3Serializable]
	public bool isAugSpawnHandledToday;

	public const int DAY_UNTIL_NORMALE_CLIENT_SPAWN = 13;

	public const int DAY_UNTIL_INTEL_CLIENT_SPAWN = 13;

	public const int DAY_UNTIL_MINOR_CLIENT_SPAWN = 8;

	[ES3Serializable]
	public int daySinceInformationClient;

	[ES3Serializable]
	public int gunBuyerDaysMissedBasic;

	[ES3Serializable]
	public int gunBuyerDaysMissedMid;

	[ES3Serializable]
	public int gunBuyerDaysMissedHigh;

	public int appraisalDaysMissed;

	[ES3Serializable]
	public int secretAdmirerDaysWithoutGift;

	[ES3Serializable]
	public bool secretAdmirerInjectorGiven;

	[ES3Serializable]
	public bool secretAdmirerBlackInjectorGiven;

	[ES3Serializable]
	public int daySinceInspection;

	[ES3Serializable]
	public int dayUntilInspection = 6;

	[ES3Serializable]
	public bool inspectionSeeded;

	public const int INSPECTION_TRUSTED_BONUS_MIN = 2;

	public const int INSPECTION_TRUSTED_BONUS_MAX = 4;

	public const int INSPECTION_MIN_TRAFFICKING_CRIME = 50;

	public const int MINIMUM_ATT_DISPLAY_CASE = 100;

	public const int ONE_NORMAL_ATT_THRESHOLD = 200;

	public const int TWO_NORMAL_ATT_THRESHOLD = 250;

	public const int THREE_NORMAL_ATT_THRESHOLD = 500;

	public const int EXTRA_BUDGET_10_THRESHOLD = 300;

	public const int EXTRA_BUDGET_25_THRESHOLD = 450;

	public const int EXTRA_BUDGET_50_THRESHOLD = 600;

	public const int EXTRA_BUDGET_75_THRESHOLD = 750;

	public const int EXTRA_BUDGET_100_THRESHOLD = 900;

	public const int RICH_CUSTOMER_400_THRESHOLD = 400;

	public const int RICH_CUSTOMER_650_THRESHOLD = 650;

	public const int WILD_BAILOUT_DECLINE_DAYS = 5;

	public const int WILD_BAILOUT_TOLERANCE_DAYS = 2;

	public const int WILD_BAILOUT_WINDOW = 7;

	[ES3Serializable]
	public bool wildBailoutGiven;

	[ES3Serializable]
	public List<int> wealthHistory;

	public const string STAMP_SELLER_CLIENT_ID = "scavStamp";

	public static readonly HashSet<string> POP_CULTURE_CLIENT_IDS = new HashSet<string> { "caseEnjoyer", "caseEnjoyerReturning", "foodieTourist", "foodieTouristReturning" };

	public const string WANTED_INTRO_CLIENT_ID = "wanted_intro";

	public const int WANTED_BASE_CHANCE = 4;

	public const int WANTED_CHANCE_PER_DAY = 2;

	public const int WANTED_GUARANTEED_DAYS = 16;

	private const int AUG_DISGUISE_CHANCE = 80;

	public StoreClientManager()
	{
		clientStack = new List<StoreClient>();
		clientTracker = new Dictionary<string, int>();
		permanentClientTracker = new Dictionary<string, int>();
		arrestedClients = new List<string>();
		chemicalFeedbackStack = new List<StoreClient>();
		uniqueClientTracker = new Dictionary<string, int>();
		spriteLastShownDay = new Dictionary<string, int>();
		spriteShownHistory = new List<string>();
		wealthHistory = new List<int>();
		possibleRandomWantedClient = new List<string> { "wanted1", "wanted2", "wanted3", "wanted6", "wanted7", "wanted8", "wanted9" };
	}

	public void GenerateClient()
	{
		isAugSpawnHandledToday = false;
		StorylineSchedule.Resolve(StoreStation.Instance.dayCounter);
		StoreClientData.MigrateEleanorBail(StoreStation.Instance.dayCounter);
		HandleDebugClient();
		ModHook.FireOnGenerateCustomerVeryEarly(this);
		HandleShowCaseSecurity();
		HandleImportantScriptedClient();
		HandleWildBailout();
		HandleCartelClient();
		HandleEleanorEarlyClient();
		HandleShowcaseClient();
		HandleChemicalFeedbackClient();
		HandleSkipClient();
		HandleAlwayClient();
		HandleClientFromEvent();
		HandleClientFromIntro();
		HandleInspectionClient();
		ModHook.FireOnGenerateCustomerEarly(this);
		HandleSupplierClient();
		HandleContentUnlockClient();
		HandleCharacterLine();
		HandleExpeditionClient();
		HandleInformationClient();
		HandleInventorClient();
		HandleMinorClient();
		ModHook.FireOnGenerateCustomerNormal(this);
		HandleAugClient(isEarlyCall: true);
		HandleWantedClient();
		HandleNormalClient();
		HandleAugClient();
		ModHook.FireOnGenerateCustomerLate(this);
		HandlePerkClient();
		HandleMarketClient();
		HandleServiceClient();
		HandleSawyerCrewClient();
		HandleEleanorUncleClient();
		CheckRentDay();
		ClientGenCheck();
		HandleJohnSmithVandalVisit();
		LouBombHelper.QueueVisit(this);
		LouOperativeHelper.QueueVisits(this);
		ModHook.FireOnGenerateCustomerVeryLate(this);
	}

	public void OnNewDay()
	{
		isTodayWantedQueued = false;
		daySinceWantedWasQueued++;
	}

	public StoreClient PickClient()
	{
		int num = 50;
		StoreClient storeClient = null;
		string text = ComputeClientType();
		while (storeClient == null && num > 0)
		{
			num--;
			storeClient = PickRandomClient(text);
			if (!IsClientValid(storeClient))
			{
				storeClient = null;
			}
		}
		if (storeClient == null && text != "FACTION_LOWER_LEVEL")
		{
			num = 50;
			while (storeClient == null && num > 0)
			{
				num--;
				storeClient = PickRandomClient();
				if (!IsClientValid(storeClient))
				{
					storeClient = null;
				}
			}
		}
		if (storeClient == null)
		{
			Debug.Log("Failed to find a cooldown client");
		}
		TrackClient(storeClient);
		return storeClient;
	}

	public StoreClient PickRandomClient(string faction = "")
	{
		return StoreClientList.GetRandomClientPool(faction).Pick(GetAttractivenessTier());
	}

	public static int GetAttractivenessTier()
	{
		int currentStoreAttractiveness = PlayerStore.Instance.GetCurrentStoreAttractiveness();
		if (currentStoreAttractiveness >= 650)
		{
			return 2;
		}
		if (currentStoreAttractiveness >= 400)
		{
			return 1;
		}
		return 0;
	}

	public StoreClient PickRandomValidClient(string type = "")
	{
		StoreClient storeClient = null;
		int num = 50;
		while (storeClient == null && num > 0)
		{
			num--;
			storeClient = PickRandomClient(type);
			if (!IsClientValid(storeClient))
			{
				storeClient = null;
			}
		}
		return storeClient;
	}

	public static StoreClient PickShopperClient()
	{
		string text = PlayerStore.Instance.CheckMostStockTypeMainInv();
		Debug.Log("Shopper Client Type " + text);
		string text2 = ShowcaseHelper.CreateClientByStockType(text);
		if (text2 == null)
		{
			return null;
		}
		return StoreClientListDict.CreateStoreClient(text2);
	}

	public static StoreClient PickSupplierClient()
	{
		int odd = RNG.GetOdd(StoreClientList.sellClientListProba);
		return StoreClientList.sellClientList[odd]();
	}

	public static bool IsClientBlockedByBannedCommissary(StoreClient storeClient)
	{
		if (StartingPerk.IsBannedCommissary())
		{
			return "scavStamp".Equals(storeClient.identifier);
		}
		return false;
	}

	public static bool IsClientBlockedByPopCulture(StoreClient storeClient)
	{
		if (EmporiumOption.DisablePopCultureClients)
		{
			return POP_CULTURE_CLIENT_IDS.Contains(storeClient.identifier);
		}
		return false;
	}

	public static void HoldBackPopCultureClient(StoreClient storeClient)
	{
		PlayerStore instance = PlayerStore.Instance;
		if (instance == null || instance.futurStoreClientIdQueue == null)
		{
			return;
		}
		for (int i = 0; i < instance.futurStoreClientIdQueue.Count; i++)
		{
			if (instance.futurStoreClientIdQueue[i] == storeClient.identifier && instance.futurStoreClientDayQueue[i] > 0)
			{
				return;
			}
		}
		instance.QueueFuturClient(storeClient.identifier, (instance.storeState != PlayerStore.StoreState.MORNING) ? 1 : 0);
	}

	public void HoldBackQueuedPopCultureClients()
	{
		if (!EmporiumOption.DisablePopCultureClients)
		{
			return;
		}
		foreach (StoreClient item in clientStack.Where(IsClientBlockedByPopCulture).ToList())
		{
			clientStack.Remove(item);
			HoldBackPopCultureClient(item);
		}
	}

	public static bool IsClientValid(StoreClient storeClient)
	{
		if (IsClientBlockedByBannedCommissary(storeClient) || IsClientBlockedByPopCulture(storeClient))
		{
			return false;
		}
		if (IsClientOnCooldown(storeClient))
		{
			return false;
		}
		return true;
	}

	public static bool IsClientOnCooldown(StoreClient storeClient)
	{
		if (PlayerStore.Instance.storeClientManager.clientTracker.ContainsKey(storeClient.identifier))
		{
			if (PlayerStore.Instance.storeClientManager.clientTracker[storeClient.identifier] == 0)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public static bool IsClientOnCooldown(string clientId)
	{
		if (PlayerStore.Instance.storeClientManager.clientTracker.ContainsKey(clientId))
		{
			if (PlayerStore.Instance.storeClientManager.clientTracker[clientId] == 0)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public void ClientGenCheck()
	{
		RemoveDuplicateClientsByIdentifier("securityInspector");
		RemoveDuplicateClientsByIdentifier("inventor");
		HashSet<string> seenIdentifiers = new HashSet<string>();
		clientStack.RemoveAll(delegate(StoreClient client)
		{
			if (client == null || client.identifier == null)
			{
				return false;
			}
			if (seenIdentifiers.Contains(client.identifier))
			{
				Debug.Log("<color=yellow>Removed duplicate client: " + client.identifier + "</color>");
				return true;
			}
			seenIdentifiers.Add(client.identifier);
			return false;
		});
	}

	public void RemoveClientById(int clientId)
	{
		clientStack.RemoveAll((StoreClient client) => client?.identifier.Equals(clientId) ?? false);
	}

	public void RemoveDuplicateClientsByIdentifier(string id)
	{
		bool firstInstanceFound = false;
		clientStack.RemoveAll(delegate(StoreClient client)
		{
			if (client == null || client.identifier == null)
			{
				return false;
			}
			if (!client.identifier.Equals(id))
			{
				return false;
			}
			if (!firstInstanceFound)
			{
				firstInstanceFound = true;
				return false;
			}
			return true;
		});
	}

	public void ReplaceLoanSharkWithRandomClient()
	{
		for (int i = 0; i < clientStack.Count; i++)
		{
			StoreClient storeClient = clientStack[i];
			if (storeClient != null && storeClient.identifier != null && (storeClient.identifier.Equals("loan_shark") || storeClient.identifier.Equals("loan_shark2") || storeClient.identifier.Equals("loan_shark_collect")))
			{
				StoreClient storeClient2 = PickClient();
				if (storeClient2 != null)
				{
					clientStack[i] = storeClient2;
					continue;
				}
				clientStack.RemoveAt(i);
				i--;
			}
		}
	}

	public void AddClient(StoreClient storeClient)
	{
		if (storeClient == null)
		{
			Debug.Log("ERROR Adding a null Client");
		}
		else if (!IsClientBlockedByKnownSnitch(storeClient) && !IsClientBlockedByBannedCommissary(storeClient))
		{
			if (IsClientBlockedByPopCulture(storeClient))
			{
				HoldBackPopCultureClient(storeClient);
				return;
			}
			clientStack.Add(storeClient);
			TrackClient(storeClient);
		}
	}

	public static bool IsClientBlockedByKnownSnitch(StoreClient storeClient)
	{
		if (!StartingPerk.IsKnownSnitch())
		{
			return false;
		}
		if ("wanted_intro".Equals(storeClient.identifier))
		{
			return true;
		}
		if (storeClient.isWanted)
		{
			return StoreClientListWanted.POSTER_IDS.Contains(storeClient.wantedPosterId);
		}
		return false;
	}

	public void TryAddClient(StoreClient storeClient, bool noDuplicate = true)
	{
		if (storeClient == null)
		{
			Debug.Log("ERROR Adding a null Client");
		}
		else if (!IsClientOnCooldown(storeClient) && (!noDuplicate || !clientStack.Any((StoreClient c) => c.identifier.Equals(storeClient.identifier))))
		{
			AddClient(storeClient);
		}
	}

	public void AddNextClient(StoreClient storeClient)
	{
		if (storeClient == null)
		{
			Debug.Log("ERROR Adding a null Client");
		}
		else if (!IsClientBlockedByKnownSnitch(storeClient) && !IsClientBlockedByBannedCommissary(storeClient))
		{
			if (IsClientBlockedByPopCulture(storeClient))
			{
				HoldBackPopCultureClient(storeClient);
				return;
			}
			clientStack.Insert(0, storeClient);
			TrackClient(storeClient);
		}
	}

	public void CheckRentDay()
	{
		if (PlayerStore.Instance.GetRentDayCounter() == 0 && !PlayerStore.Instance.IsPropertyPaid)
		{
			if (PlayerStore.Instance.isPayingMortgage)
			{
				if (PlayerStore.Instance.playerCash < PlayerStore.Instance.mortgageWeeklyPayment && PlayerStore.Instance.loanOfferedToGeorgio != 0 && StoreStation.Instance.dayCounter > PlayerStore.Instance.dayWhenLoanOfferedGeorgio + 5)
				{
					AddClient(StoreClientUniqueList.CreateGeorgioLoanPayback());
				}
				bool flag = false;
				foreach (StoreClient item in clientStack)
				{
					if (item.identifier.Equals("merchant"))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					AddClient(StoreClientList.CreateMerchant());
				}
				AddClient(StoreClientList.CreateLoanSharkCollect());
			}
			else
			{
				if (PlayerStore.Instance.playerCash < PlayerStore.Instance.rentValue && PlayerStore.Instance.loanOfferedToGeorgio != 0 && StoreStation.Instance.dayCounter > PlayerStore.Instance.dayWhenLoanOfferedGeorgio + 5)
				{
					AddClient(StoreClientUniqueList.CreateGeorgioLoanPayback());
				}
				if (!IsClientQueued("merchant"))
				{
					AddClient(StoreClientList.CreateMerchant());
				}
				AddClient(StoreClientList.CreateLandlordClient());
			}
			if (uniqueClientTracker.ContainsKey("loan_shark") && PlayerStore.Instance.CanBeOfferedMortgage() && !PlayerStore.Instance.isPayingMortgage && !PlayerStore.Instance.IsMortgageOfferDeclined && !PlayerStore.Instance.IsPropertyPaid)
			{
				AddClient(StoreClientList.CreateLoanShark2());
			}
		}
		else if (PlayerStore.Instance.GetRentDayCounter() == 0 && PlayerStore.Instance.IsPropertyPaid && !IsClientQueued("merchant"))
		{
			AddClient(StoreClientList.CreateMerchant());
		}
	}

	private bool IsClientQueued(string identifier)
	{
		foreach (StoreClient item in clientStack)
		{
			if (item.identifier.Equals(identifier))
			{
				return true;
			}
		}
		return false;
	}

	public void HandleSkipClient()
	{
		if (!PlayerStore.Instance.skipIntro)
		{
			return;
		}
		if (StoreStation.Instance.dayCounter == 1)
		{
			if (PlayerStore.Instance.IsFenceStart() || StartingPerk.IsSawyerCrew())
			{
				AddClient(StoreClientListStory.CreateJanitorUndergroundExchangeIntroductionFence());
			}
			if (PlayerStore.Instance.IsGunsmithStart())
			{
				AddClient(StoreClientListGun.CreateLLGunBuyer());
			}
			AddClient(StoreClientList.CreateScavGeneral());
			AddClient(PickSupplierClient());
		}
		if (StoreStation.Instance.dayCounter == 2)
		{
			AddClient(StoreClientListWanted.CreateWanted4());
			if (PlayerStore.Instance.IsGunsmithStart())
			{
				AddClient(StoreClientListGun.CreateLLGunSeller());
			}
		}
		_ = StoreStation.Instance.dayCounter;
		_ = 3;
		if (StoreStation.Instance.dayCounter == 4 && PlayerStore.Instance.IsGunsmithStart())
		{
			AddClient(StoreClientListGun.CreateLLGunBuyer());
		}
		if (StoreStation.Instance.dayCounter == 5)
		{
			AddClient(StoreClientListStory.CreateStoryAppraisalBoard());
		}
	}

	public void HandleServiceClient()
	{
		if (StoreService.GetBuyReferalService().IsActive())
		{
			StoreClient storeClient = PickShopperClient();
			if (storeClient != null)
			{
				AddClient(storeClient);
			}
		}
		if (StoreService.GetSellReferalService().IsActive())
		{
			StoreClient storeClient2 = PickSupplierClient();
			if (storeClient2 != null)
			{
				AddClient(storeClient2);
			}
		}
		StoreService criminelNetworkService = StoreService.GetCriminelNetworkService();
		if (criminelNetworkService != null && criminelNetworkService.IsActive())
		{
			StoreReputation.ModBMReputation(2);
			if (RNG.Roll(50))
			{
				AddClient(StoreClientList.CreateCriminalBuyerThief());
			}
			else
			{
				AddClient(StoreClientList.CreateCriminelMerc());
			}
		}
		StoreService hypertechSmugglerService = StoreService.GetHypertechSmugglerService();
		if (hypertechSmugglerService != null && hypertechSmugglerService.IsActive())
		{
			AddClient(StoreClientListSupplier.CreateHypertechSmuggler());
		}
	}

	private void ComputeInspectionCooldownRange(out int min, out int max)
	{
		int totalTraffickingCrimes = PlayerStore.Instance.secData.GetTotalTraffickingCrimes();
		if (totalTraffickingCrimes >= 750)
		{
			min = 1;
			max = 2;
		}
		else if (totalTraffickingCrimes >= 600)
		{
			min = 2;
			max = 2;
		}
		else if (totalTraffickingCrimes >= 450)
		{
			min = 2;
			max = 3;
		}
		else if (totalTraffickingCrimes >= 300)
		{
			min = 3;
			max = 3;
		}
		else if (totalTraffickingCrimes >= 150)
		{
			min = 3;
			max = 4;
		}
		else
		{
			min = 3;
			max = 5;
		}
		if (StoreReputation.IsPerkUnlocked("SEC_TRUSTED"))
		{
			min += 2;
			max += 4;
		}
		if (StoreReputation.IsPerkUnlocked("SEC_THOROUGH_INSPECTION"))
		{
			min--;
			max--;
		}
		if (min < 1)
		{
			min = 1;
		}
		if (max < 2)
		{
			max = 2;
		}
	}

	public void ResetInspection()
	{
		ComputeInspectionCooldownRange(out var min, out var max);
		dayUntilInspection = RNG.GetRandomInt(min, max);
		inspectionSeeded = true;
	}

	public bool IsInspectionCountdownActive()
	{
		if (StoreReputation.IsPerkUnlocked("SEC_VIP"))
		{
			return false;
		}
		if (PlayerStore.Instance.secData.GetTotalTraffickingCrimes() < 50)
		{
			return false;
		}
		return inspectionSeeded;
	}

	public void HandleInspectionClient()
	{
		if (StoreReputation.IsPerkUnlocked("SEC_VIP") || PlayerStore.Instance.secData.GetTotalTraffickingCrimes() < 50)
		{
			return;
		}
		if (!inspectionSeeded)
		{
			ResetInspection();
			return;
		}
		ComputeInspectionCooldownRange(out var _, out var max);
		if (dayUntilInspection > max)
		{
			dayUntilInspection = max;
		}
		dayUntilInspection--;
		if (dayUntilInspection <= 0)
		{
			if (StoreReputation.IsPerkUnlocked("SEC_THOROUGH_INSPECTION"))
			{
				AddClient(StoreClientList.CreateInspectionClient());
			}
			else
			{
				AddClient(StoreClientList.CreateInspectionClient(PlayerStore.Instance.secData.GetInspectorPersonality()));
			}
			ResetInspection();
		}
	}

	public void HandleCartelClient()
	{
		if (PlayerStore.Instance.startType == NewGameData.StartType.DRUG_DEALER && !PlayerStore.Instance.isCartelExited)
		{
			if (StoreStation.Instance.dayCounter == 1)
			{
				AddClient(StoreClientListCartel.CreateCartelIntro());
			}
			else if (StoreStation.Instance.dayCounter == 2)
			{
				AddClient(StoreClientListCartel.CreateCartelSec());
				AddClient(StoreClientListBarter.CreateAddictGeneral());
			}
			else if (StoreStation.Instance.dayCounter == 3)
			{
				AddClient(StoreClientListCartel.CreateDealer());
			}
			else if (StoreStation.Instance.dayCounter % 7 == 4)
			{
				AddClient(StoreClientListCartel.CreateCartelHandlerCheckin());
			}
			else if (StoreStation.Instance.dayCounter % 7 == 1)
			{
				AddClient(StoreClientListCartel.CreateCartelHandlerWeekly());
			}
			if (StoreStation.Instance.dayCounter == 9 || StoreStation.Instance.dayCounter == 18 || StoreStation.Instance.dayCounter == 27)
			{
				AddClient(StoreClientListCartel.CreateCartelWoundedSoldier());
			}
		}
	}

	public void HandleShowCaseSecurity()
	{
		if (ShowcaseHelper.IsShowCaseContainSeriousContraband() || ShowcaseHelper.IsShowCaseContainSeriousStolen())
		{
			AddClient(StoreClientList.CreateInspectionClient(0, fromShowcase: true));
		}
	}

	public void HandleDebugClient()
	{
		_ = PlayerStore.Instance.devMode;
	}

	public void HandleSawyerCrewClient()
	{
		if (StoreStation.Instance.dayCounter == 1 && !PlayerStore.Instance.skipIntro && StartingPerk.IsSawyerCrew() && !PlayerStore.Instance.exchangeManager.isExchangeUnlocked && !PlayerStore.Instance.storeClientData.isSawyerDead)
		{
			AddClient(StoreClientListStory.CreateJanitorUndergroundExchangeIntroductionFence());
		}
	}

	public void HandleAlwayClient()
	{
		AppraisalRankHelper.HandlePromotionClient(this);
		_ = StoreStation.Instance.dayCounter;
		_ = 1;
		_ = StoreStation.Instance.dayCounter;
		_ = 2;
		if (StoreStation.Instance.dayCounter == 10)
		{
			NetworkUpgrade.IsMarketplaceRequestsUnlocked();
		}
		if (StoreStation.Instance.dayCounter == 18)
		{
			AddClient(StoreClientUniqueList.LandlordWholesale());
		}
		if (StoreStation.Instance.dayCounter == 25 && !PlayerStore.Instance.exchangeManager.isExchangeUnlocked && !PlayerStore.Instance.storeClientData.isSawyerDead)
		{
			AddClient(StoreClientListStory.CreateJanitorUndergroundExchangeIntroduction());
		}
		if (StoreClientData.CanRecurrentJuanAppear() && StoreStation.GetDayCounter() % 8 == 1)
		{
			AddClient(StoreClientListWanted.CreateWanted7());
		}
		StoreClientData.ScheduleLouFirstVisit();
		if (StoreClientData.CanLouFirstVisit(StoreStation.GetDayCounter()))
		{
			AddClient(StoreClientListWanted.CreateWanted5());
		}
		if (StoreClientData.CanLouDropBox(StoreStation.GetDayCounter()))
		{
			AddClient(StoreClientListWanted.CreateLouBoxDrop());
		}
		if (StoreClientData.CanLouCollectBox(StoreStation.GetDayCounter()))
		{
			AddClient(StoreClientListWanted.CreateLouBoxCollect());
		}
		if (StoreClientData.CanLouDisguise(StoreStation.GetDayCounter()))
		{
			AddClient(StoreClientListWanted.CreateLouDisguise());
		}
		if (StoreClientData.CanLouCollectDemand(StoreStation.GetDayCounter()))
		{
			AddClient(StoreClientListWanted.CreateLouDemandCollect());
		}
		if (StoreClientData.CanBaileyReturn(StoreStation.GetDayCounter()))
		{
			AddClient(StoreClientListWanted.CreateWanted1Return());
		}
		if (StoreClientData.CanButcherFirstVisitForTest())
		{
			AddClient(StoreClientListWanted.CreateWanted2());
		}
		if (StoreClientData.CanButcherReturn(StoreStation.GetDayCounter()))
		{
			AddClient(StoreClientListWanted.CreateWanted2Return());
		}
		if (StoreClientData.CanJohnSmithWeekly(StoreStation.GetDayCounter()))
		{
			AddClient(StoreClientListWanted.CreateWanted8Weekly());
		}
		if (StoreClientData.CanSawyerOfferButcherHelp(StoreStation.GetDayCounter()) && !PlayerStore.Instance.storeClientData.isSawyerDead)
		{
			AddClient(StoreClientListWanted.CreateSawyerButcherOffer());
		}
		if (LotteryHelper.IsLotteryUnlocked() && StoreStation.GetDayCounter() % 7 == 4)
		{
			AddClient(StoreClientList.CreateLotteryRep());
		}
		if (PlayerStore.Instance.storeClientData.isJacksonIntroduced && !PlayerStore.Instance.storeClientData.isModuleIntroduced && PlayerStore.Instance.IsPlayerHaveMachine())
		{
			AddClient(StoreClientListStory.CreateInventorModuleIntro());
		}
		if (24 == StoreStation.Instance.dayCounter && !PlayerStore.Instance.hasBusinessPermit)
		{
			AddClient(StoreClientListStory.CreateMentorPermit());
		}
		if (12 == StoreStation.Instance.dayCounter && !PlayerStore.Instance.isAugIntroduced)
		{
			AddClient(StoreClientListStory.CreateMentorShotgun());
		}
		if (38 <= StoreStation.Instance.dayCounter && StoreStation.Instance.dayCounter % 7 == 3 && !PlayerStore.Instance.hasBusinessPermit)
		{
			if (45 <= StoreStation.Instance.dayCounter)
			{
				PlayerStore.Instance.storeClientData.businessPermitBribeAmount += StoreClientList.GetBribeAmount();
				if (PlayerStore.Instance.storeClientData.businessPermitBribeAmount <= 10)
				{
					PlayerStore.Instance.storeClientData.businessPermitBribeAmount = 10;
				}
			}
			AddClient(StoreClientList.CreateBusinessPermit());
		}
		HandleBlackmailClient();
		HandleScalperClient();
		HandleCaseEnjoyerClient();
	}

	public void HandleCaseEnjoyerClient()
	{
		if (EmporiumOption.DisablePopCultureClients)
		{
			return;
		}
		StoreClientData storeClientData = PlayerStore.GetStoreClientData();
		int dayCounter = StoreStation.Instance.dayCounter;
		if (!storeClientData.caseEnjoyerVisited)
		{
			if (dayCounter > 1 && StoreClientUniqueList.CountCaseEnjoyerItems() >= 2)
			{
				storeClientData.caseEnjoyerVisited = true;
				storeClientData.caseEnjoyerReturnDay = dayCounter + 7;
				AddClient(StoreClientUniqueList.CreateCaseEnjoyer());
			}
		}
		else if (!storeClientData.caseEnjoyerReturned && storeClientData.caseEnjoyerReturnDay > 0 && dayCounter >= storeClientData.caseEnjoyerReturnDay)
		{
			storeClientData.caseEnjoyerReturned = true;
			AddClient(StoreClientUniqueList.CreateCaseEnjoyerReturning());
		}
	}

	public void HandleScalperClient()
	{
		if (StartingPerk.IsSentinel() && StoreStation.Instance.dayCounter % 7 == 6)
		{
			AddClient(StoreClientListSpec.CreateScalper());
		}
	}

	public void HandleCharacterLine()
	{
		if (!PlayerStore.Instance.endlessMode)
		{
			CheckStoryLineGeorgio();
			CheckStoryLineOfficerLun();
			CheckStoryLineEleanor();
			CheckStoryLineSerpico();
			if (PlayerStore.Instance.loanOfferedToGeorgio != 0 && PlayerStore.Instance.GetRentDayCounter() != 0 && StoreStation.Instance.dayCounter > PlayerStore.Instance.dayWhenLoanOfferedGeorgio + 25)
			{
				AddClient(StoreClientUniqueList.CreateGeorgioLoanPayback());
			}
		}
	}

	public void HandleJohnSmithVandalVisit()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.isJohnSmithVandalVisitPending)
		{
			storeClientData.isJohnSmithVandalVisitPending = false;
			AddNextClient(StoreClientListWanted.CreateWanted8Vandal());
		}
	}

	public void HandleImportantScriptedClient()
	{
		if (!uniqueClientTracker.ContainsKey("loan_shark") && PlayerStore.Instance.GetCash() >= PlayerStore.Instance.GetMortgageDownPayment() && !PlayerStore.Instance.IsPropertyPaid)
		{
			AddClient(StoreClientList.CreateLoanSharkIntro());
		}
		if (PermitHelper.IsGunPermitI() && StoreReputation.GetSecReputation() < 0)
		{
			AddClient(StoreClientListSecurity.CreateRevokePermitOfficer());
		}
		if (PermitHelper.IsGunPermitII() && StoreReputation.GetSecReputation() < 30)
		{
			AddClient(StoreClientListSecurity.CreateRevokePermitOfficer());
		}
		if (PermitHelper.IsGunPermitIII() && StoreReputation.GetSecReputation() < 60)
		{
			AddClient(StoreClientListSecurity.CreateRevokePermitOfficer());
		}
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.liNextVisitDay >= 0 && StoreStation.Instance.dayCounter >= storeClientData.liNextVisitDay)
		{
			int liNextVisit = storeClientData.liNextVisit;
			storeClientData.liNextVisitDay = -1;
			if (!PlayerStore.Instance.wantedStatus.TryGetValue("wanted6", out var value) || value != 1)
			{
				AddClient((liNextVisit == 2) ? StoreClientListWanted.CreateWanted6Finale() : StoreClientListWanted.CreateWanted6Supply());
			}
		}
		StoreClientData storeClientData2 = PlayerStore.Instance.storeClientData;
		if (storeClientData2.johnSmithReportedDay >= 0 && !storeClientData2.isJohnSmithReturnQueued && StoreStation.Instance.dayCounter >= storeClientData2.johnSmithReportedDay + 2)
		{
			storeClientData2.isJohnSmithReturnQueued = true;
			AddClient(StoreClientListWanted.CreateWanted8Return());
		}
	}

	public void HandleWildBailout()
	{
		if (!wildBailoutGiven && Difficulty.Rules.wildBailout && !PlayerStore.Instance.IsRockBottomStart())
		{
			UpdateWealthTrend();
			StorePhoneClient wildPhoneClient = StorePhoneClient.GetWildPhoneClient();
			if (wildPhoneClient != null && wildPhoneClient.phoneState == StorePhoneClient.PhoneState.Regular && IsWealthTrendingDown())
			{
				AddClient(StoreClientListStory.CreateWildBailout());
				wildBailoutGiven = true;
			}
		}
	}

	private void UpdateWealthTrend()
	{
		wealthHistory.Add(PlayerStore.Instance.GetTotalEstimatedValue());
		int num = 8;
		while (wealthHistory.Count > num)
		{
			wealthHistory.RemoveAt(0);
		}
	}

	private bool IsWealthTrendingDown()
	{
		if (wealthHistory.Count < 8)
		{
			return false;
		}
		int num = wealthHistory[0];
		int num2 = wealthHistory[^1];
		if (num <= num2)
		{
			return false;
		}
		int num3 = 0;
		for (int i = 1; i < wealthHistory.Count; i++)
		{
			if (wealthHistory[i] >= wealthHistory[i - 1])
			{
				num3++;
			}
		}
		return num3 <= 2;
	}

	public void HandleClientFromIntro()
	{
		if (!PlayerStore.Instance.enableStoryClient)
		{
			return;
		}
		Debug.Log("Handling Client From Story, Day: " + StoreStation.Instance.dayCounter);
		if (!PlayerStore.Instance.isSecurityRecordIntroduced && PlayerStore.Instance.secData.GetTotalCrimeValue() > 0)
		{
			PlayerStore.Instance.isSecurityRecordIntroduced = true;
			AddClient(StoreClientListStory.CreateMentorSecRecord());
		}
		if (StoreStation.Instance.dayCounter == 1)
		{
			AddClient(StoreClientListStory.CreateMentorClient());
			AddClient(StoreClientListStory.CreateFoodSellerStory());
			AddClient(StoreClientListStory.CreateFoodBuyerStory());
			AddClient(StoreClientListStory.CreateFoodBuyerStory2());
			AddClient(StoreClientListStory.CreateSecWantedIntroduction());
		}
		if (StoreStation.Instance.dayCounter == 2)
		{
			AddClient(StoreClientList.CreateJunkerSellOnly());
			AddClient(StoreClientListTier.CreateLowerRepairman());
			AddClient(StoreClientListStory.CreateFoodSellerStory2());
			AddClient(StoreClientListStory.CreateMentorAfterhours());
		}
		if (StoreStation.Instance.dayCounter == 3)
		{
			AddClient(StoreClientListStory.CreateMentorClientVisualInspection());
			AddClient(StoreClientListStory.CreateStoryCigaretteVendor());
			AddClient(StoreClientListTier.CreateScavFood());
			AddClient(StoreClientList.CreateScavGeneral());
			AddClient(StoreClientListWanted.CreateWanted3());
		}
		if (StoreStation.Instance.dayCounter == 4)
		{
			AddClient(StoreClientListStory.CreateStoryCigaretteVendor2());
			AddClient(StoreClientList.CreateUpperLowerVisitor());
			AddClient(StoreClientList.CreateFlexiBuyer());
			AddClient(StoreClientListStory.CreateInsuranceIntroduction());
		}
		if (StoreStation.Instance.dayCounter == 5)
		{
			AddClient(StoreClientListStory.CreateStoryScrapSeller());
			AddClient(StoreClientList.CreateFlexiBuyer());
			AddClient(StoreClientListSupplier.CreateScavFoodSeller());
			AddClient(StoreClientListStory.CreateMentorDisplayCase());
		}
		if (StoreStation.Instance.dayCounter == 6)
		{
			AddClient(StoreClientListBarter.CreateBasicFoodBarterTutorial());
			AddClient(StoreClientList.CreateFlexiBuyer());
			AddClient(StoreClientListStory.CreateInventorIntro());
		}
		if (StoreStation.Instance.dayCounter == 7)
		{
			AddClient(StoreClientListStory.CreateNetworkIntroduction());
			AddClient(StoreClientList.CreateScavGeneral());
			AddClient(StoreClientListStory.CreateMentorClientEventTip());
		}
		if (StoreStation.Instance.dayCounter == 8)
		{
			AddClient(StoreClientListStory.CreateMentorBargainIntro());
			AddClient(StoreClientListStory.CreateFoodSellerStory3());
			AddClient(StoreClientList.CreateFlexiBuyer());
			AddClient(StoreClientListStory.CreateStoryAppraisalBoard());
			AddClient(StoreClientList.CreateInventor());
		}
		if (StoreStation.Instance.dayCounter == 9)
		{
			AddClient(StoreClientListStory.CreateSecWantedIntroduction2());
			AddClient(StoreClientList.CreateLowerLevelChemist(carryContraband: true));
			AddClient(StoreClientListStory.CreateOffDutyOfficier());
			if (PlayerStore.Instance.isAppraisalIntroduced)
			{
				AddClient(StoreClientListAppraisal.CreateAppraisal1());
			}
		}
		if (StoreStation.Instance.dayCounter == 10)
		{
			if (!StartingPerk.IsSawyerCrew() && !PlayerStore.Instance.storeClientData.isSawyerDead)
			{
				AddClient(StoreClientListStory.CreateJanitorIntroduction());
			}
			AddClient(StoreClientListWanted.CreateWanted4());
			AddClient(StoreClientListTier.CreateMaintenanceWorker());
			AddClient(StoreClientList.CreateScavGeneral());
		}
		if (StoreStation.Instance.dayCounter == 11)
		{
			AddClient(StoreClientList.CreateStampSeller());
			AddClient(StoreClientListStory.CreateLuckyScavStory());
		}
		if (StoreStation.Instance.dayCounter == 12)
		{
			AddClient(StoreClientList.CreateStampSeller());
			AddClient(StoreClientList.CreateScavGeneral());
			AddClient(StoreClientList.CreateInventor());
		}
		if (StoreStation.Instance.dayCounter == 13)
		{
			AddClient(StoreClientList.CreateShadyPharmacist());
			AddClient(StoreClientListBarter.CreateAddict());
			AddClient(StoreClientList.CreateThief());
			AddClient(StoreClientUniqueList.CreateFoodieTourist());
		}
		if (StoreStation.Instance.dayCounter == 14)
		{
			AddClient(StoreClientListStory.CreateMentorClientOutro());
			if (StoreClientListInformation.HasAvailableInformation())
			{
				AddClient(StoreClientListInformation.CreateShadyCustomer());
			}
		}
		_ = StoreStation.Instance.dayCounter;
		_ = 15;
		_ = StoreStation.Instance.dayCounter;
		_ = 16;
	}

	public void HandleClientFromStoryLine()
	{
		if (StoreStation.Instance.dayCounter == 3)
		{
			AddClient(StoreClientListChurch.CreateUniquePreacher());
		}
		if (StoreStation.Instance.dayCounter == 5)
		{
			AddClient(StoreClientListChurch.CreateChurchAbby1());
		}
		if (StoreStation.Instance.dayCounter == 10)
		{
			AddClient(StoreClientListChurch.CreateUniquePreacher2());
		}
		if (StoreStation.Instance.dayCounter == 12)
		{
			AddClient(StoreClientListChurch.CreateChurchAbby2());
		}
		if (StoreStation.Instance.dayCounter == 19)
		{
			AddClient(StoreClientListChurch.CreateChurchAbby3());
		}
		if (StoreStation.Instance.dayCounter == 26)
		{
			AddClient(StoreClientListChurch.CreateChurchAbby4());
		}
		if (StoreStation.Instance.dayCounter == 33)
		{
			AddClient(StoreClientListChurch.CreateChurchAbby5());
		}
		if (StoreStation.Instance.dayCounter == 40)
		{
			AddClient(StoreClientListChurch.CreateChurchUniqueEngineer());
		}
		if (StoreStation.Instance.dayCounter == 43)
		{
			AddClient(StoreClientListChurch.CreateChurchAbbyEnding());
		}
		if (StoreStation.Instance.dayCounter == 47)
		{
			AddClient(StoreClientListChurch.CreateChurchUniqueEngineer2());
		}
	}

	public void HandleClientFromEvent()
	{
		foreach (StoreEvent activeEvent in StoreStation.Instance.storeEventManager.GetActiveEvents())
		{
			if (!string.IsNullOrEmpty(activeEvent.addClientFromEventActionId) && StoreEventActionDict.actions.TryGetValue(activeEvent.addClientFromEventActionId, out var value))
			{
				value?.Invoke();
			}
		}
	}

	public void UpdateTracker()
	{
		foreach (string item in new List<string>(clientTracker.Keys))
		{
			if (clientTracker[item] > 0)
			{
				clientTracker[item] -= 1;
			}
		}
	}

	public void TrackClient(StoreClient storeClient)
	{
		if (storeClient == null)
		{
			Debug.Log("ERROR Tracking a null Client");
		}
		else if (!storeClient.cooldownStartsOnArrival)
		{
			if (!clientTracker.ContainsKey(storeClient.identifier))
			{
				clientTracker.Add(storeClient.identifier, storeClient.cooldown);
			}
			else
			{
				clientTracker[storeClient.identifier] = storeClient.cooldown;
			}
		}
	}

	public void TrackClientVisit(StoreClient storeClient)
	{
		if (storeClient != null)
		{
			if (!permanentClientTracker.ContainsKey(storeClient.identifier))
			{
				permanentClientTracker.Add(storeClient.identifier, 1);
			}
			else
			{
				permanentClientTracker[storeClient.identifier]++;
			}
			if (storeClient.cooldownStartsOnArrival)
			{
				clientTracker[storeClient.identifier] = storeClient.cooldown;
			}
		}
	}

	public void HandleShowcaseClient()
	{
		if ((!PlayerStore.Instance.skipIntro && StoreStation.Instance.dayCounter == 1) || PlayerStore.Instance.GetCurrentStoreAttractiveness() < 100)
		{
			return;
		}
		foreach (StoreClient item in ShowcaseHelper.HandleDisplayCase())
		{
			if (item != null)
			{
				Debug.Log("Display Client Add: " + item.identifier);
				AddClient(item);
			}
		}
	}

	public void HandlePerkClient()
	{
		if (StoreReputation.IsPerkUnlocked("SEC_SURPLUS") && StoreStation.Instance.dayCounter % 7 == 3)
		{
			AddClient(StoreClientList.CreateSecSurplusMerchant());
		}
		else if (StoreReputation.IsPerkUnlocked("SEC_REQUISITION") && StoreStation.Instance.dayCounter % 7 == 4 && StoreStation.Instance.dayCounter > 7)
		{
			AddClient(StoreClientList.CreateSecRequisitionOfficer());
		}
		if (StoreReputation.IsPerkUnlocked("LLRAREGOOD") && (StoreStation.Instance.dayCounter % 7 == 5 || StoreStation.Instance.dayCounter % 7 == 1))
		{
			if (RNG.Roll(50))
			{
				AddClient(StoreClientList.CreateLowerLevelRareMerchant());
			}
			else if (RNG.Roll(25))
			{
				AddClient(StoreClientList.CreateGunsmith());
			}
			else if (RNG.Roll(25))
			{
				AddClient(StoreClientList.CreateRareLowerLevelChemist());
			}
		}
		if (StoreReputation.IsPerkUnlocked("BM_RELIABLE") && StoreStation.Instance.dayCounter % 7 == 1)
		{
			AddClient(StoreClientList.CreateThief());
		}
		if (StoreReputation.IsPerkUnlocked("UL_HOUSEHOLD_NAME") && RNG.Roll(20))
		{
			StoreClient storeClient = PickRandomValidClient("FACTION_UPPER_LEVEL");
			if (storeClient != null)
			{
				AddClient(storeClient);
			}
		}
		int discerningShopperChance = ShowcaseHelper.GetDiscerningShopperChance();
		if (discerningShopperChance > 0 && !IsClientOnCooldown("ulDiscerningShopper") && RNG.Roll(discerningShopperChance))
		{
			AddClient(StoreClientList.CreateDiscerningShopper());
		}
		if (StoreReputation.IsPerkUnlocked("UL_CHARITY") && !IsClientOnCooldown("upperLevelCharity") && IsCharityComing())
		{
			StoreClient storeClient2 = StoreClientList.CreateUpperLevelCharity();
			if (storeClient2 != null)
			{
				AddClient(storeClient2);
			}
		}
		if (StoreReputation.IsPerkUnlocked("BM_RELIABLE") && RNG.Roll(20))
		{
			AddClient(StoreClientList.CreatePettyThief());
		}
		if (StoreReputation.IsPerkUnlocked("REV_QUARTERMASTER") && StoreStation.Instance.dayCounter % 7 == 0)
		{
			AddClient(StoreClientListRev.CreateRevQuartermaster());
		}
		if (StoreReputation.IsPerkUnlocked("REV_RAID_LOOT") && StoreStation.Instance.dayCounter % 7 == 5)
		{
			AddClient(StoreClientListRev.CreateRevRaider());
		}
	}

	public void HandleNormalClient()
	{
		UpdateTracker();
		int normalCustomerCount = GetNormalCustomerCount();
		int num = 0;
		List<(string, int, string)> displayCasePrediction = ShowcaseHelper.GetDisplayCasePrediction();
		ShowcaseHelper.ClearPendingSupplierPicks();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (var (text, chance, attractedItemId) in displayCasePrediction)
		{
			if (num >= normalCustomerCount)
			{
				break;
			}
			if (RNG.Roll(chance))
			{
				StoreClient storeClient = StoreClientListDict.CreateStoreClient(text);
				if (storeClient != null)
				{
					storeClient.attractedItemId = attractedItemId;
					AddClient(storeClient);
					hashSet.Add(text);
					num++;
				}
			}
		}
		ShowcaseHelper.OnGunBuyersRolled(displayCasePrediction, hashSet);
		AppraisalRankHelper.OnCustomerRolled(displayCasePrediction, hashSet);
		if ((!PlayerStore.Instance.skipIntro && StoreStation.Instance.dayCounter <= 13) || (PlayerStore.Instance.skipIntro && StoreStation.Instance.dayCounter == 1))
		{
			return;
		}
		int num2 = normalCustomerCount - num;
		if (isTodayWantedQueued && num2 > 0)
		{
			num2--;
		}
		for (int i = 0; i < num2; i++)
		{
			StoreClient storeClient2 = PickClient();
			if (storeClient2 != null)
			{
				AddClient(storeClient2);
			}
		}
	}

	public int GetNormalCustomerCount()
	{
		int currentStoreAttractiveness = PlayerStore.Instance.GetCurrentStoreAttractiveness();
		int num = 0;
		if (StoreReputation.IsPerkUnlocked("LLPOORREVIEWS") && RNG.Roll(100))
		{
			num--;
		}
		if (StoreReputation.IsPerkUnlocked("LLREFERAL") && RNG.Roll(100))
		{
			num++;
		}
		if (currentStoreAttractiveness >= 200 && !PlayerStore.Instance.isVandalized)
		{
			num++;
		}
		if (currentStoreAttractiveness >= 250 && !PlayerStore.Instance.isVandalized)
		{
			num++;
		}
		if (currentStoreAttractiveness >= 500 && !PlayerStore.Instance.isVandalized)
		{
			num++;
		}
		if (StartingPerk.IsSideStreet())
		{
			num--;
		}
		if (StartingPerk.IsPrimeRealEstate())
		{
			num++;
		}
		if (PlayerStore.Instance.healthData.wentToAfterhoursLastNight && !StartingPerk.IsShortSleeper())
		{
			num--;
		}
		return Mathf.Clamp(num, 0, 6);
	}

	public void HandleInformationClient()
	{
		daySinceInformationClient++;
		if ((PlayerStore.Instance.skipIntro || StoreStation.Instance.dayCounter > 13) && StoreClientListInformation.HasAvailableInformation())
		{
			if (daySinceInformationClient >= 7)
			{
				AddClient(StoreClientListInformation.CreateShadyCustomer());
				daySinceInformationClient = 0;
			}
			else if (daySinceInformationClient >= 4 && RNG.Roll(25))
			{
				AddClient(StoreClientListInformation.CreateShadyCustomer());
				daySinceInformationClient = 0;
			}
		}
	}

	public void HandleSupplierClient()
	{
		if (StoreStation.Instance.dayCounter == 7)
		{
			AddClient(StoreClientList.CreateSupplierIntro());
		}
		else if (StoreStation.Instance.dayCounter != 0 && StoreStation.Instance.dayCounter % 7 == 0)
		{
			AddClient(StoreClientList.CreateSupplier());
		}
	}

	public void HandleInventorClient()
	{
		if (!StoreReputation.IsPerkUnlocked("LLAVOIDED"))
		{
			if (StoreStation.Instance.dayCounter >= 12 && StoreStation.Instance.dayCounter % 7 == 5)
			{
				AddClient(StoreClientList.CreateInventor());
			}
			else
			{
				HandleJacksonStorage();
			}
		}
	}

	public void HandleAdapativeClient()
	{
		if (StoreStation.Instance.dayCounter > 13 && EmporiumEntry.Instance.invElement.childItems.Count < 12)
		{
			StoreClient storeClient = PickSupplierClient();
			if (storeClient != null)
			{
				AddClient(storeClient);
				Debug.Log("AdapativeClient: " + storeClient.displayName);
			}
		}
	}

	public void HandleMarketClient()
	{
		List<KeyValuePair<string, int>> marketModifierType = PlayerStore.Instance.GetMarketModifierType(TypeHelper.MAJOR_TYPES_LIST);
		int threshold = 30;
		marketModifierType.RemoveAll((KeyValuePair<string, int> tv) => tv.Value < threshold && tv.Value > -threshold);
		foreach (KeyValuePair<string, int> item in marketModifierType)
		{
			if (item.Value >= threshold)
			{
				Debug.Log("Market Client Check: " + item.Key + " Value: " + item.Value);
				if (item.Key.Equals("FOOD"))
				{
					TryAddClient(StoreClientListShortage.CreateFoodShortageClient());
				}
				if (item.Key.Equals("MEDICAL"))
				{
					TryAddClient(StoreClientListShortage.CreateMedicalShortageClient());
				}
				if (item.Key.Equals("SUBSTANCE"))
				{
					TryAddClient(StoreClientListShortage.CreateSubstanceShortageClient());
				}
				if (item.Key.Equals("MATERIAL"))
				{
					TryAddClient(StoreClientListShortage.CreateMaterialShortageClient());
				}
				if (item.Key.Equals("WEAPON") || item.Key.Equals("ARMOR"))
				{
					TryAddClient(StoreClientListShortage.CreateWeaponArmorShortageClient());
				}
			}
			else if (item.Value <= -threshold)
			{
				Debug.Log("Market Client Check: " + item.Key + " Value: " + item.Value);
				if (item.Key.Equals("FOOD"))
				{
					TryAddClient(StoreClientListSurplus.CreateFoodSurplusClient());
				}
				if (item.Key.Equals("MATERIAL"))
				{
					TryAddClient(StoreClientListSurplus.CreateMaterialSurplusClient());
				}
				if (item.Key.Equals("MEDICAL"))
				{
					TryAddClient(StoreClientListSurplus.CreateMedicalSurplusClient());
				}
				if (item.Key.Equals("SUBSTANCE"))
				{
					TryAddClient(StoreClientListSurplus.CreateSubstanceSurplusClient());
				}
				if (item.Key.Equals("WEAPON") || item.Key.Equals("ARMOR"))
				{
					TryAddClient(StoreClientListSurplus.CreateWeaponArmorSurplusClient());
				}
			}
		}
	}

	public void DismissClientStack()
	{
		clientStack.Clear();
	}

	public StoreClient GetRandomNewMinorClient()
	{
		if (PlayerStore.Instance.unseenMinorClient.Count == 0)
		{
			return null;
		}
		int randomIntExculsive = RNG.GetRandomIntExculsive(0, PlayerStore.Instance.unseenMinorClient.Count);
		if (PlayerStore.Instance.unseenMinorClient == null)
		{
			Debug.Log("Warning: Possible Error in Minor Client Generation");
			return null;
		}
		string id = PlayerStore.Instance.unseenMinorClient[randomIntExculsive];
		PlayerStore.Instance.unseenMinorClient.RemoveAt(randomIntExculsive);
		return StoreClientListMinor.CreateClientFromId(id);
	}

	public void HandleMinorClient()
	{
		if (PlayerStore.Instance.endlessMode)
		{
			return;
		}
		int num = 0;
		int num2 = StoreStation.Instance.dayCounter;
		if (!PlayerStore.Instance.skipIntro)
		{
			num2 -= 8;
		}
		if (num2 <= 0)
		{
			return;
		}
		if (num2 > 2)
		{
			if (num2 <= 7)
			{
				num = 1;
			}
			else if (num2 <= 15)
			{
				if (RNG.Roll(50))
				{
					num = 1;
				}
			}
			else if (num2 <= 25)
			{
				if (RNG.Roll(25))
				{
					num = 1;
				}
			}
			else if (RNG.Roll(10))
			{
				num = 1;
			}
		}
		while (num > 0)
		{
			StoreClient randomNewMinorClient = GetRandomNewMinorClient();
			if (randomNewMinorClient != null)
			{
				AddClient(randomNewMinorClient);
			}
			num--;
		}
	}

	public List<StoreClient> GetClientStack()
	{
		return clientStack;
	}

	public StoreClient GetRandomValidWantedClient()
	{
		if (StartingPerk.IsKnownSnitch())
		{
			return null;
		}
		possibleRandomWantedClient.RemoveAll((string client) => arrestedClients.Contains(client));
		if (possibleRandomWantedClient.Count == 0)
		{
			return null;
		}
		string key = possibleRandomWantedClient[RNG.GetRandomIntExculsive(0, possibleRandomWantedClient.Count)];
		return StoreClientListWanted.wantedClientList[key]();
	}

	public void HandleWantedClient()
	{
		if (isTodayWantedQueued)
		{
			return;
		}
		if ((!PlayerStore.Instance.skipIntro && StoreStation.Instance.dayCounter <= 13) || StoreStation.Instance.dayCounter < 5)
		{
			daySinceWantedWasQueued = 0;
		}
		else if (RNG.Roll(GetRandomClientSpawnChance()))
		{
			StoreClient randomValidWantedClient = GetRandomValidWantedClient();
			if (randomValidWantedClient != null)
			{
				AddClient(randomValidWantedClient);
				isTodayWantedQueued = true;
				daySinceWantedWasQueued = 0;
				Debug.Log("Wanted Client Queued");
			}
		}
	}

	public int GetRandomClientSpawnChance()
	{
		if (daySinceWantedWasQueued >= 16)
		{
			return 100;
		}
		return Mathf.Min(100, 4 + 2 * Mathf.Max(0, daySinceWantedWasQueued - 1));
	}

	public void HandleContentUnlockClient()
	{
		int num;
		if (Difficulty.Rules.wildReminders)
		{
			num = ((!PlayerStore.Instance.IsRockBottomStart()) ? 1 : 0);
			if (num != 0 && PlayerStore.Instance.isNetworkIntroduced && StoreStation.Instance.dayCounter >= 5)
			{
				PlayerStore.Instance.daySinceLastBrowseUpgrade++;
				if (PlayerStore.Instance.daySinceLastBrowseUpgrade >= 6)
				{
					PlayerStore.Instance.daySinceLastBrowseUpgrade = 0;
					AddClient(StoreClientListStory.CreateUpgradeReminder());
				}
			}
		}
		else
		{
			num = 0;
		}
		if (num != 0 && PlayerStore.Instance.isNetworkIntroduced && !PlayerStore.Instance.skipIntro && !PlayerStore.Instance.networkReminderShown && StoreStation.Instance.dayCounter - PlayerStore.Instance.networkIntroducedDay >= 5)
		{
			StorePhoneClient phoneClientByNumber = StorePhoneClient.GetPhoneClientByNumber(8376L);
			if (phoneClientByNumber != null && !phoneClientByNumber.dialedBefore)
			{
				PlayerStore.Instance.networkReminderShown = true;
				AddClient(StoreClientListStory.CreateNetworkReminder());
			}
		}
		if (PlayerStore.Instance.isDisplayCaseIntroduced && !PlayerStore.Instance.skipIntro && !PlayerStore.Instance.displayCaseReminderShown)
		{
			if (ShowcaseHelper.HasShowcaseSpecialItem() && ShowcaseHelper.HasMerchandise())
			{
				PlayerStore.Instance.displayCaseSetupMissedDays = 0;
			}
			else
			{
				PlayerStore.Instance.displayCaseSetupMissedDays++;
				if (PlayerStore.Instance.displayCaseSetupMissedDays >= 2)
				{
					PlayerStore.Instance.displayCaseReminderShown = true;
					AddClient(StoreClientListStory.CreateMentorDisplayCaseReminder());
				}
			}
		}
		if (StoreStation.Instance.dayCounter == 2 && !PlayerStore.Instance.isBrokenMachineIntroduced && PlayerStore.Instance.IsJunkerStart())
		{
			PlayerStore.Instance.isBrokenMachineIntroduced = true;
			AddClient(StoreClientListStory.CreateBrokenMachineIntroduction());
		}
		int dayCounter = StoreStation.Instance.dayCounter;
		if (StoreStation.Instance.dayCounter % 7 == 6)
		{
			AddClient(StoreClientList.CreateFlexiBuyer());
		}
		if (StoreStation.Instance.dayCounter >= 50 && StoreStation.Instance.dayCounter % 7 == 3)
		{
			AddClient(StoreClientList.CreateFlexiBuyer());
		}
		if (StoreStation.Instance.dayCounter != 0 && StoreStation.Instance.dayCounter % 7 == 3)
		{
			if (PlayerStore.Instance.IsJunkerStart() || NetworkUpgrade.IsRuinedMachineUnlocked())
			{
				AddClient(StoreClientListSpec.CreateRetiredJunker());
			}
			if (PlayerStore.Instance.IsWaterMerchantStart() || NetworkUpgrade.IsWaterTraderUnlocked())
			{
				AddClient(StoreClientListSpec.CreateRetiredWaterMerchant());
			}
		}
		if (StoreStation.Instance.dayCounter != 0 && StoreStation.Instance.dayCounter % 7 == 4 && (PlayerStore.Instance.IsGunsmithStart() || NetworkUpgrade.IsRetiredGunsmithUnlocked()))
		{
			AddClient(StoreClientListSpec.CreateRetiredGunsmith());
		}
		if (StoreStation.Instance.dayCounter != 0 && StoreStation.Instance.dayCounter % 7 == 5)
		{
			if (PlayerStore.Instance.IsFarmerStart() || NetworkUpgrade.IsRetiredFarmerUnlocked())
			{
				AddClient(StoreClientListSpec.CreateRetiredFarmer());
			}
			if (NetworkUpgrade.IsWinemakerUnlocked())
			{
				AddClient(StoreClientListSpec.CreateRetiredWinemaker());
			}
		}
		if (StoreStation.Instance.dayCounter != 0 && StoreStation.Instance.dayCounter % 7 == 5 && (PlayerStore.Instance.IsRancherStart() || NetworkUpgrade.IsRetiredRancherUnlocked()))
		{
			AddClient(StoreClientListSpec.CreateRetiredRancher());
		}
		if (StoreStation.Instance.dayCounter != 0 && StoreStation.Instance.dayCounter % 7 == 2 && NetworkUpgrade.IsAquacultureUnlocked())
		{
			AddClient(StoreClientListSpec.CreateRetiredAquafarmer());
		}
		if ((StoreStation.Instance.dayCounter % 7 == 3 || StoreStation.Instance.dayCounter % 7 == 6) && (PlayerStore.Instance.IsChemistStart() || NetworkUpgrade.IsRetiredChemistUnlocked()))
		{
			AddClient(StoreClientListCartel.CreateShadyFarmer());
		}
		if (dayCounter > 7 && (dayCounter % 7 == 2 || dayCounter % 7 == 5) && (PlayerStore.Instance.IsChemistStart() || NetworkUpgrade.IsRetiredChemistUnlocked()))
		{
			AddClient(StoreClientListCartel.CreateDealer());
		}
		if (StoreStation.Instance.dayCounter != 0 && StoreStation.Instance.dayCounter % 7 == 2 && (PlayerStore.Instance.IsChemistStart() || NetworkUpgrade.IsRetiredChemistUnlocked()))
		{
			AddClient(StoreClientListSpec.CreateRetiredChemist());
		}
		int num2 = StoreStation.Instance.dayCounter - 43;
		bool num3 = num2 >= 0 && num2 <= 42 && num2 % 14 == 0;
		bool flag = !PlayerStore.Instance.isVendingMachineUnlocked || !PlayerStore.Instance.isVendingFountainUnlocked;
		if (num3 && flag && !PlayerStore.Instance.IsPropertyPaid)
		{
			AddClient(StoreClientList.CreateLandlordVendingMachineClient());
		}
	}

	public void HandleChemicalFeedbackClient()
	{
		foreach (StoreClient item in chemicalFeedbackStack)
		{
			AddClient(item);
		}
		chemicalFeedbackStack.Clear();
	}

	public void CheckStoryLineGeorgio()
	{
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("georgio"))
		{
			AddClient(StoreClientUniqueList.CreateGeorgioIntro());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("georgio") + 3)
		{
			AddClient(StoreClientUniqueList.CreateGeorgioIntro2());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("georgio") + 6)
		{
			AddClient(StoreClientUniqueList.CreateGeorgioBackpack());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("georgio") + 9 && !StartingPerk.IsMaimed())
		{
			AddClient(StoreClientUniqueList.CreateGeorgioAssistance());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("georgio") + 12)
		{
			AddClient(StoreClientUniqueList.CreateGeorgio2());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("georgio") + 16)
		{
			AddClient(StoreClientUniqueList.CreateGeorgio3());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("georgio") + 21)
		{
			AddClient(StoreClientUniqueList.CreateGeorgioLuckIntro());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("georgio") + 26)
		{
			AddClient(StoreClientUniqueList.CreateGeorgioLuckSale());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("georgio") + 31)
		{
			AddClient(StoreClientUniqueList.CreateGeorgio4());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("georgio") + 37)
		{
			AddClient(StoreClientUniqueList.CreateGeorgioLoan());
		}
		if (StoreStation.Instance.dayCounter >= 3 && PlayerStore.GetStoreClientData().nuclearWasteDumped > 0 && !PlayerStore.GetStoreClientData().georgioNuclearWasteVisited)
		{
			PlayerStore.GetStoreClientData().georgioNuclearWasteVisited = true;
			AddClient(StoreClientUniqueList.CreateGeorgioNuclearWaste());
		}
	}

	public void CheckStoryLineOfficerLun()
	{
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("lun"))
		{
			AddClient(StoreClientUniqueList.CreateOfficerLunVisit());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("lun") + 1)
		{
			AddClient(StoreClientUniqueList.CreateOfficerLunShot());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("lun") + 2)
		{
			AddClient(StoreClientUniqueList.CreateLunCriminel());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("lun") + 5)
		{
			AddClient(StoreClientUniqueList.CreateOfficerLunBackOnDuty());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("lun") + 9 && PlayerStore.Instance.storeClientData.lunAddictionLevel > 0)
		{
			AddClient(StoreClientUniqueList.CreateOfficerLunSurplus());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("lun") + 13 && PlayerStore.Instance.storeClientData.lunAddictionLevel > 0 && !PlayerStore.Instance.storeClientData.officerLunArrested)
		{
			AddClient(StoreClientUniqueList.CreateOfficerLunInspection());
		}
	}

	public void CheckStoryLineEleanor()
	{
		if (PlayerStore.Instance.storeClientData.isEleanorDead)
		{
			HandleEleanorLeak();
			return;
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("eleanor"))
		{
			AddClient(StoreClientUniqueList.EleanorIntro());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("eleanor") + 2)
		{
			AddClient(StoreClientUniqueList.Eleanor2());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("eleanor") + 4)
		{
			AddClient(StoreClientUniqueList.Eleanor3());
		}
		if (StoreClientData.CanEleanorBailOfficerVisit(StoreStation.Instance.dayCounter))
		{
			AddClient(StoreClientUniqueList.CreateEleanorBailOfficer());
		}
		if (StoreClientData.CanEleanorFreedVisit(StoreStation.Instance.dayCounter))
		{
			AddClient(StoreClientUniqueList.EleanorFreed());
		}
		if (StoreClientData.CanEleanorPlanVisit(StoreStation.Instance.dayCounter))
		{
			AddClient(StoreClientUniqueList.Eleanor6());
		}
		else if (StoreClientData.CanEleanorScavVisit(StoreStation.Instance.dayCounter))
		{
			AddClient(StoreClientUniqueList.EleanorScav());
		}
		else if (StoreClientData.CanEleanorDriveVisit(StoreStation.Instance.dayCounter))
		{
			AddClient(StoreClientUniqueList.Eleanor7());
		}
		else if (StoreClientData.CanEleanorThreatVisit(StoreStation.Instance.dayCounter))
		{
			AddClient(StoreClientUniqueList.CreateEleanorThreat());
		}
		else if (StoreClientData.CanEleanorChoiceVisit(StoreStation.Instance.dayCounter))
		{
			AddClient(StoreClientUniqueList.Eleanor8());
		}
		else if (StoreClientData.CanEleanorConclusionVisit(StoreStation.Instance.dayCounter))
		{
			AddClient(StoreClientUniqueList.Eleanor9());
		}
		HandleEleanorLeak();
	}

	private void HandleEleanorLeak()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.eleanorDriveChoice == 2 && !storeClientData.nutrifruitSeedsFreed && StoreStation.Instance.dayCounter >= storeClientData.eleanorChoiceDay + 1)
		{
			PlantList.FreeNutrifruit();
		}
	}

	public void HandleEleanorUncleClient()
	{
		if (!PlayerStore.Instance.endlessMode && !PlayerStore.Instance.storeClientData.isEleanorDead && StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("eleanor") + 5)
		{
			AddClient(StoreClientUniqueList.CreateEleanorUncle());
		}
	}

	public void HandleEleanorEarlyClient()
	{
		if (!PlayerStore.Instance.endlessMode && !PlayerStore.Instance.storeClientData.isEleanorDead)
		{
			int dayCounter = StoreStation.Instance.dayCounter;
			if (StoreClientData.CanEleanorWarnedVisit(dayCounter))
			{
				AddClient(StoreClientUniqueList.EleanorWarned());
			}
			if (dayCounter == StoreClientData.GetEleanorVisit4Day())
			{
				AddClient(StoreClientUniqueList.Eleanor4());
			}
			if (dayCounter == StoreClientData.GetEleanorVisit5Day() && PlayerStore.Instance.storeClientData.eleanorHelpedTimes > 0 && !PlayerStore.Instance.storeClientData.eleanorArrested)
			{
				AddClient(StoreClientUniqueList.Eleanor5());
			}
		}
	}

	public void CheckStoryLineNellis()
	{
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("nellis"))
		{
			AddClient(StoreClientUniqueList.Nellis1());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("nellis") + 4)
		{
			AddClient(StoreClientUniqueList.Nellis2());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("nellis") + 8)
		{
			AddClient(StoreClientUniqueList.Nellis3());
		}
		if (StoreStation.Instance.dayCounter == StorylineSchedule.StartDay("nellis") + 12)
		{
			AddClient(StoreClientUniqueList.Nellis4());
		}
	}

	public void CheckStoryLineSerpico()
	{
		SerpicoStoryline.HandleDay(this, StoreStation.Instance.dayCounter);
	}

	public void HandleBlackmailClient()
	{
		if (StoreStation.Instance.dayCounter == 1 && StartingPerk.IsBlackmail())
		{
			AddClient(StoreClientListSpec.CreateBlackmailerIntro());
		}
		else if (StoreStation.Instance.dayCounter % 7 == 1 && StartingPerk.IsBlackmail())
		{
			AddClient(StoreClientListSpec.CreateBlackmailer());
		}
	}

	public void HandleJacksonStorage()
	{
		int itemCountById = EmporiumEntry.Instance.GetItemCountById("storage_bay_large");
		bool flag = false;
		if (itemCountById == 0 && StoreStation.Instance.dayCounter >= 12 && PlayerStore.Instance.GetCash() >= 850 && !IsClientOnCooldown("inventorStorage"))
		{
			flag = true;
		}
		if (itemCountById == 1 && StoreStation.Instance.dayCounter >= 40 && PlayerStore.Instance.GetCash() >= 850 && !IsClientOnCooldown("inventorStorage"))
		{
			flag = true;
		}
		if (itemCountById == 2 && StoreStation.Instance.dayCounter >= 70 && PlayerStore.Instance.GetCash() >= 850 && !IsClientOnCooldown("inventorStorage"))
		{
			flag = true;
		}
		int itemCountById2 = EmporiumEntry.Instance.GetItemCountById("machine_bay_ext");
		if (itemCountById2 == 0 && StoreStation.Instance.dayCounter >= 30 && PlayerStore.Instance.GetCash() >= 1200 && !IsClientOnCooldown("inventorStorage"))
		{
			flag = true;
		}
		if (itemCountById2 == 1 && StoreStation.Instance.dayCounter >= 55 && PlayerStore.Instance.GetCash() >= 1200 && !IsClientOnCooldown("inventorStorage"))
		{
			flag = true;
		}
		if (itemCountById2 == 2 && StoreStation.Instance.dayCounter >= 85 && PlayerStore.Instance.GetCash() >= 1200 && !IsClientOnCooldown("inventorStorage"))
		{
			flag = true;
		}
		if (flag)
		{
			AddClient(StoreClientList.CreateInventorStorage());
		}
	}

	public string ComputeClientType()
	{
		foreach (RandomClientPool factionPool in StoreClientList.factionPools)
		{
			if (factionPool.RollChance())
			{
				return factionPool.factionId;
			}
		}
		return "FACTION_LOWER_LEVEL";
	}

	public static int GetBlackMarketClientChance()
	{
		int num = 10;
		if (StoreReputation.GetBMReputation() < 0)
		{
			num -= 3;
		}
		if (StoreReputation.GetBMReputation() < 25)
		{
			num -= 3;
		}
		if (StoreReputation.GetBMReputation() >= 25)
		{
			num += 4;
		}
		if (StoreReputation.GetBMReputation() >= 50)
		{
			num += 3;
		}
		return num;
	}

	public static int GetUpperLevelClientChance()
	{
		int num = 8;
		if (StoreReputation.IsPerkUnlocked("UL_TOURISTTRAP"))
		{
			num -= 3;
		}
		if (StoreReputation.GetULReputation() >= 25)
		{
			num += 4;
		}
		if (StoreReputation.GetULReputation() >= 50)
		{
			num += 3;
		}
		return num;
	}

	public static int GetRevolutionClientChance()
	{
		if (StoreReputation.IsPerkUnlocked("REV_BLACKLISTED"))
		{
			return 0;
		}
		int revReputation = StoreReputation.GetRevReputation();
		if (revReputation >= 50)
		{
			return 9;
		}
		if (revReputation >= 25)
		{
			return 6;
		}
		if (revReputation >= 0)
		{
			return 3;
		}
		return 2;
	}

	public bool IsCharityComing()
	{
		bool result = false;
		if (CommissaryData.GetTotalTypeMarketPriceModifier("FOOD") >= 66)
		{
			result = true;
		}
		if (CommissaryData.GetTotalTypeMarketPriceModifier("ENERGY_CELL") >= 66)
		{
			result = true;
		}
		if (CommissaryData.GetTotalTypeMarketPriceModifier("WEAPON") >= 66)
		{
			result = true;
		}
		if (CommissaryData.GetTotalTypeMarketPriceModifier("WATER") >= 66)
		{
			result = true;
		}
		if (CommissaryData.GetTotalTypeMarketPriceModifier("MEDICAL") >= 66)
		{
			result = true;
		}
		return result;
	}

	public static int GetCustomerTotalVisitedCount(string identifier)
	{
		if (PlayerStore.Instance.storeClientManager.permanentClientTracker.TryGetValue(identifier, out var value))
		{
			return value;
		}
		return 0;
	}

	public void HandleExpeditionClient()
	{
		if (!PlayerStore.Instance.expeditionData.isExpeditionUnlocked || !ShowcaseHelper.IsShowcaseContainItemById("sign_expedition"))
		{
			return;
		}
		foreach (Hireling hireling in PlayerStore.Instance.expeditionData.hirelings)
		{
			if (hireling.status == Hireling.HirelingStatus.Complete)
			{
				AddClient(StoreClientListSpec.CreateHirelingClientReturn(hireling));
			}
			else if (hireling.status == Hireling.HirelingStatus.Available)
			{
				hireling.Rest();
				AddClient(StoreClientListSpec.CreateHirelingClient(hireling));
			}
			else if (hireling.status == Hireling.HirelingStatus.Injured)
			{
				hireling.Rest();
				hireling.UpdateInjuredStatus();
			}
		}
	}

	public void HandleAugClient(bool isEarlyCall = false)
	{
		if ((isEarlyCall && RNG.Roll(50)) || !PlayerStore.Instance.isAugIntroduced || isAugSpawnHandledToday || PlayerStore.Instance.isAugEnded)
		{
			return;
		}
		int augIntroducedDay = PlayerStore.GetAugIntroducedDay();
		if (!StartingPerk.IsAugMenace() && (StoreStation.Instance.dayCounter == augIntroducedDay + 1 || StoreStation.Instance.dayCounter == augIntroducedDay + 6))
		{
			isAugSpawnHandledToday = true;
			AddClient(StoreClientList.CreateAugMed());
			return;
		}
		if (PlayerStore.Instance.IsAugAllClearDue(StoreStation.Instance.dayCounter))
		{
			PlayerStore.Instance.isAugEnded = true;
			AddClient(StoreClientListSecurity.CreateAugAllClearOfficer());
			return;
		}
		if (PlayerStore.Instance.nextAugSpawnDay < 0)
		{
			PlayerStore.Instance.nextAugSpawnDay = augIntroducedDay + 6 + RNG.GetRandomInt(6, 8);
		}
		if (StoreStation.Instance.dayCounter >= PlayerStore.Instance.nextAugSpawnDay)
		{
			isAugSpawnHandledToday = true;
			AddAugClient();
			PlayerStore.Instance.nextAugSpawnDay = StoreStation.Instance.dayCounter + RNG.GetRandomInt(6, 8);
		}
	}

	private void AddAugClient()
	{
		if (RNG.Roll(80))
		{
			StoreClient storeClient = FindEventCustomerAugCandidate();
			if (storeClient != null)
			{
				AugHelper.MakeDisguisedAug(storeClient, AugHelper.RollAugTier());
				Debug.Log($"<color=orange>[Aug] Disguised aug queued from EVENT customer '{storeClient.identifier}' (event: {storeClient.eventSourceId}, tier {storeClient.augTier})</color>");
				return;
			}
			StoreClient storeClient2 = ShowcaseHelper.CreateDisplayCaseAugBuyer();
			if (storeClient2 != null)
			{
				storeClient2.identifier += "_disguised_aug";
				AugHelper.MakeDisguisedAug(storeClient2, AugHelper.RollAugTier());
				AddClient(storeClient2);
				Debug.Log($"<color=orange>[Aug] Disguised aug queued from DISPLAY CASE buyer '{storeClient2.identifier}' (type: {storeClient2.attractedType}, tier {storeClient2.augTier})</color>");
				return;
			}
		}
		if (RNG.GetRandomInt(0, 1) == 0)
		{
			AddClient(StoreClientList.CreateAugMed());
		}
		else
		{
			AddClient(StoreClientList.CreateAugMaterial());
		}
	}

	private StoreClient FindEventCustomerAugCandidate()
	{
		List<StoreClient> list = clientStack.Where((StoreClient c) => c != null && !c.isAug && !string.IsNullOrEmpty(c.eventSourceId) && c.dismissable && !c.isSecurity && c.clientFaction != "FACTION_SECURITY" && c.clientFaction != "FACTION_REVOLUTION" && c.IsClientBuying() && (c.clientBuyingTagList.Contains("MATERIAL") || c.clientBuyingTagList.Contains("MEDICAL"))).ToList();
		if (list.Count == 0)
		{
			return null;
		}
		return list[RNG.GetRandomIntExculsive(0, list.Count)];
	}
}

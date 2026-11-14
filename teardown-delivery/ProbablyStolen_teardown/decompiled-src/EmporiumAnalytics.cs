using System;
using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

public class EmporiumAnalytics
{
	public static readonly bool OVERWRITE_DISABLE_ANALYTICS = true;

	private const string ENV_INTERNAL_PLAYTEST = "internal_playtest";

	private const string ENV_PUBLIC_PLAYTEST = "public-playtest";

	public static bool isAnalyticsInitialized = false;

	public static async Task InitAnalytics()
	{
		if (OVERWRITE_DISABLE_ANALYTICS || isAnalyticsInitialized || EmporiumOption.Instance == null || !EmporiumOption.Instance.consentAnalytics)
		{
			return;
		}
		Debug.Log("Initializing Analytics...");
		try
		{
			await UnityServices.InitializeAsync(SetEnvironment());
			AnalyticsService.Instance.StartDataCollection();
			isAnalyticsInitialized = true;
		}
		catch (Exception ex)
		{
			Debug.LogError("Analytics initialization failed: " + ex.Message);
		}
	}

	public static void StopAnalytics()
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			AnalyticsService.Instance.StopDataCollection();
			isAnalyticsInitialized = false;
			Debug.Log("Analytics stopped.");
		}
		catch (Exception ex)
		{
			Debug.LogError("Analytics stop failed: " + ex.Message);
		}
	}

	public static InitializationOptions SetEnvironment()
	{
		InitializationOptions initializationOptions = new InitializationOptions();
		initializationOptions.SetEnvironmentName("internal_playtest");
		return initializationOptions;
	}

	public static void TrackNewGame()
	{
		if (isAnalyticsInitialized)
		{
			try
			{
				NewGameStartedEvent e = new NewGameStartedEvent
				{
					Spec = NewGameData.GetStartDisplayName(NewGameData.Instance.startType),
					RunNumber = PlayerStore.Instance.runNumber,
					RunId = PlayerStore.Instance.runID,
					SkipIntro = NewGameData.Instance.skipIntro,
					HardMode = NewGameData.Instance.hardMode,
					EndlessMode = NewGameData.Instance.endlessMode
				};
				AnalyticsService.Instance.RecordEvent(e);
			}
			catch (Exception ex)
			{
				Debug.LogError("TrackNewGame failed: " + ex.Message);
			}
			TrackStartingPerks();
		}
	}

	public static void TrackWildUpgradePurchased(string upgradeId)
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			WildUpgradePurchasedEvent e = new WildUpgradePurchasedEvent
			{
				RunId = PlayerStore.Instance.runID,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				CurrentCredit = PlayerStore.Instance.playerCash,
				UpgradeId = upgradeId,
				TotalPlayTimeSeconds = PlayerStore.Instance.GetCurrentTotalPlayTime(),
				RunPlayTimeSeconds = PlayerStore.Instance.GetCurrentRunPlayTime()
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackWildUpgradePurchased failed: " + ex.Message);
		}
	}

	public static void TrackWildUIOpened()
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			WildUIOpenedEvent e = new WildUIOpenedEvent
			{
				RunId = PlayerStore.Instance.runID,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				CurrentCredit = PlayerStore.Instance.playerCash,
				TotalPlayTimeSeconds = PlayerStore.Instance.GetCurrentTotalPlayTime(),
				RunPlayTimeSeconds = PlayerStore.Instance.GetCurrentRunPlayTime()
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackWildUIOpened failed: " + ex.Message);
		}
	}

	public static void TrackLeaveToMainMenu()
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			int dayCounter = StoreStation.Instance.dayCounter;
			int lastInspectionLossDay = PlayerStore.Instance.lastInspectionLossDay;
			int lastTheftDay = PlayerStore.Instance.lastTheftDay;
			LeaveToMainMenuEvent e = new LeaveToMainMenuEvent
			{
				RunId = PlayerStore.Instance.runID,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				Day = dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				CurrentCredit = PlayerStore.Instance.playerCash,
				DaysSinceInspectionLoss = ((lastInspectionLossDay >= 0) ? (dayCounter - lastInspectionLossDay) : 0),
				LastInspectionLossValue = PlayerStore.Instance.lastInspectionLossValue,
				DaysSinceTheft = ((lastTheftDay >= 0) ? (dayCounter - lastTheftDay) : 0),
				LastTheftValue = PlayerStore.Instance.lastTheftValue,
				TotalPlayTimeSeconds = PlayerStore.Instance.GetCurrentTotalPlayTime(),
				RunPlayTimeSeconds = PlayerStore.Instance.GetCurrentRunPlayTime()
			};
			AnalyticsService.Instance.RecordEvent(e);
			AnalyticsService.Instance.Flush();
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackLeaveToMainMenu failed: " + ex.Message);
		}
	}

	public static void TrackGameExit()
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			int dayCounter = StoreStation.Instance.dayCounter;
			int lastInspectionLossDay = PlayerStore.Instance.lastInspectionLossDay;
			int lastTheftDay = PlayerStore.Instance.lastTheftDay;
			GameExitEvent e = new GameExitEvent
			{
				RunId = PlayerStore.Instance.runID,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				Day = dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				CurrentCredit = PlayerStore.Instance.playerCash,
				DaysSinceInspectionLoss = ((lastInspectionLossDay >= 0) ? (dayCounter - lastInspectionLossDay) : 0),
				LastInspectionLossValue = PlayerStore.Instance.lastInspectionLossValue,
				DaysSinceTheft = ((lastTheftDay >= 0) ? (dayCounter - lastTheftDay) : 0),
				LastTheftValue = PlayerStore.Instance.lastTheftValue,
				TotalPlayTimeSeconds = PlayerStore.Instance.GetCurrentTotalPlayTime(),
				RunPlayTimeSeconds = PlayerStore.Instance.GetCurrentRunPlayTime()
			};
			AnalyticsService.Instance.RecordEvent(e);
			AnalyticsService.Instance.Flush();
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackGameExit failed: " + ex.Message);
		}
	}

	public static void TrackLoadGame()
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			int dayCounter = StoreStation.Instance.dayCounter;
			int lastInspectionLossDay = PlayerStore.Instance.lastInspectionLossDay;
			int lastTheftDay = PlayerStore.Instance.lastTheftDay;
			LoadGameEvent e = new LoadGameEvent
			{
				RunId = PlayerStore.Instance.runID,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				Day = dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				CurrentCredit = PlayerStore.Instance.playerCash,
				DaysSinceInspectionLoss = ((lastInspectionLossDay >= 0) ? (dayCounter - lastInspectionLossDay) : 0),
				LastInspectionLossValue = PlayerStore.Instance.lastInspectionLossValue,
				DaysSinceTheft = ((lastTheftDay >= 0) ? (dayCounter - lastTheftDay) : 0),
				LastTheftValue = PlayerStore.Instance.lastTheftValue
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackLoadGame failed: " + ex.Message);
		}
	}

	public static void TrackBusinessPermitBought()
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			BusinessPermitBoughtEvent e = new BusinessPermitBoughtEvent
			{
				RunId = PlayerStore.Instance.runID,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				CurrentCredit = PlayerStore.Instance.playerCash,
				TotalPlayTimeSeconds = PlayerStore.Instance.GetCurrentTotalPlayTime(),
				RunPlayTimeSeconds = PlayerStore.Instance.GetCurrentRunPlayTime()
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackBusinessPermitBought failed: " + ex.Message);
		}
	}

	public static void TrackInspectorArrived(int personality, bool fromShowcase)
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			string inspectorType = personality switch
			{
				1 => "shady", 
				2 => "lazy", 
				_ => "normal", 
			};
			InspectorArrivedEvent e = new InspectorArrivedEvent
			{
				InspectorType = inspectorType,
				RunId = PlayerStore.Instance.runID,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				IsFromShowcase = fromShowcase,
				HasSecurityGrace = NetworkUpgrade.IsSecGraceUnlocked()
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackInspectorArrived failed: " + ex.Message);
		}
	}

	public static void TrackInspectionBribe(int bribeAmount)
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			InspectionBribeEvent e = new InspectionBribeEvent
			{
				RunId = PlayerStore.Instance.runID,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				HasSecurityGrace = NetworkUpgrade.IsSecGraceUnlocked(),
				BribeAmount = bribeAmount
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackInspectionBribe failed: " + ex.Message);
		}
	}

	public static void TrackOnInspected(string outcomeType, int lostCount, int lostValue, bool usedGrace)
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			bool hasSmugglerBay = false;
			foreach (GameItem childItem in EmporiumEntry.Instance.invElement.childItems)
			{
				if (childItem.identifier == "smuggler_bay" || childItem.identifier == "smuggler_bay_mini")
				{
					hasSmugglerBay = true;
					break;
				}
			}
			OnInspectedEvent e = new OnInspectedEvent
			{
				OutcomeType = outcomeType,
				HasSmugglerBay = hasSmugglerBay,
				TotalLostItemValue = lostValue,
				TotalLostItemCount = lostCount,
				RunId = PlayerStore.Instance.runID,
				HasSecurityGrace = NetworkUpgrade.IsSecGraceUnlocked(),
				UsedSecurityGrace = usedGrace
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackOnInspected failed: " + ex.Message);
		}
	}

	public static void TrackAppraisalGuildInteraction(string action)
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			AppraisalGuildInteractionEvent e = new AppraisalGuildInteractionEvent
			{
				Action = action,
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				RunId = PlayerStore.Instance.runID,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				TotalPlayTimeSeconds = PlayerStore.Instance.GetCurrentTotalPlayTime(),
				RunPlayTimeSeconds = PlayerStore.Instance.GetCurrentRunPlayTime()
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackAppraisalGuildInteraction failed: " + ex.Message);
		}
	}

	public static void TrackAppraisalSent(GameItem gameItem, int sentValue, int actualValue)
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			AppraisalSentEvent e = new AppraisalSentEvent
			{
				ItemId = gameItem.identifier,
				ActualValue = actualValue,
				SentValue = sentValue,
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				RunId = PlayerStore.Instance.runID,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				TotalPlayTimeSeconds = PlayerStore.Instance.GetCurrentTotalPlayTime(),
				RunPlayTimeSeconds = PlayerStore.Instance.GetCurrentRunPlayTime()
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackAppraisalSent failed: " + ex.Message);
		}
	}

	public static void TrackConfront(string itemId, string confrontType, string outcome)
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			ConfrontEvent e = new ConfrontEvent
			{
				ItemId = itemId,
				Outcome = outcome,
				ConfrontType = confrontType,
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				RunId = PlayerStore.Instance.runID,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				TotalPlayTimeSeconds = PlayerStore.Instance.GetCurrentTotalPlayTime(),
				RunPlayTimeSeconds = PlayerStore.Instance.GetCurrentRunPlayTime()
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackConfront failed: " + ex.Message);
		}
	}

	public static void TrackItemOfInterestAcquisition(GameItem gameItem)
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			ItemOfInterestAcquisitionEvent e = new ItemOfInterestAcquisitionEvent
			{
				RunId = PlayerStore.Instance.runID,
				MachineId = gameItem.identifier,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				MachineryBoughtCount = PlayerStore.Instance.machineryBoughtCount,
				RunPlayTimeSeconds = PlayerStore.Instance.GetCurrentRunPlayTime(),
				CurrentCredit = PlayerStore.Instance.playerCash
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackItemOfInterestAcquisition failed: " + ex.Message);
		}
	}

	public static void TrackWantedNotReported()
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			WantedNotReportedEvent e = new WantedNotReportedEvent
			{
				CustomerId = PlayerStore.Instance.wantedPersonVisitedId,
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				RunId = PlayerStore.Instance.runID,
				CurrentCredit = PlayerStore.Instance.playerCash
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackWantedNotReported failed: " + ex.Message);
		}
	}

	public static void TrackCustomerReported(StoreClient storeClient)
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			string reportType = (storeClient.isWanted ? "wanted" : (storeClient.IsSellingContraband() ? "contraband" : ((!storeClient.IsSellingStolen()) ? "legal" : "stolen")));
			CustomerReportedEvent e = new CustomerReportedEvent
			{
				CustomerId = storeClient.identifier,
				ReportType = reportType,
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				RunId = PlayerStore.Instance.runID,
				CurrentCredit = PlayerStore.Instance.playerCash
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackCustomerReported failed: " + ex.Message);
		}
	}

	private static string AfterhourActivityName()
	{
		string text = ((MapUIManager.Instance != null) ? MapUIManager.Instance.LastVisitedTonightId : "");
		if (text == null || text.Length != 0)
		{
			return text switch
			{
				"dumping_ground" => "SCAVENGING", 
				"commissary" => "COMMISSARY", 
				"dr_jackson" => "UPGRADE_MERCHANT", 
				_ => text, 
			};
		}
		return "STORE";
	}

	public static void TrackOnSleep()
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			OnSleepEvent e = new OnSleepEvent
			{
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				CurrentCredit = PlayerStore.Instance.playerCash,
				EstimatedTotalWealth = PlayerStore.Instance.GetTotalEstimatedValue(),
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				RunId = PlayerStore.Instance.runID,
				AfterHourActivity = AfterhourActivityName(),
				RepLowerLevel = (StoreReputation.GetStoreReputation("FACTION_LOWER_LEVEL")?.GetReputation() ?? 0),
				RepUpperLevel = (StoreReputation.GetStoreReputation("FACTION_UPPER_LEVEL")?.GetReputation() ?? 0),
				RepSecurity = (StoreReputation.GetStoreReputation("FACTION_SECURITY")?.GetReputation() ?? 0),
				RepBlackMarket = (StoreReputation.GetStoreReputation("FACTION_BLACK_MARKET")?.GetReputation() ?? 0),
				RepRevolution = (StoreReputation.GetStoreReputation("FACTION_REVOLUTION")?.GetReputation() ?? 0),
				RepTourist = (StoreReputation.GetStoreReputation("FACTION_TOURIST")?.GetReputation() ?? 0),
				RepChurch = (StoreReputation.GetStoreReputation("FACTION_CHURCH")?.GetReputation() ?? 0),
				RepCartel = (StoreReputation.GetStoreReputation("FACTION_CARTEL")?.GetReputation() ?? 0),
				RunPlayTimeSeconds = PlayerStore.Instance.GetCurrentRunPlayTime(),
				TotalPlayTimeSeconds = PlayerStore.Instance.GetCurrentTotalPlayTime(),
				IsInsuranceActive = (StoreService.GetInsuranceService()?.IsActive() ?? false),
				IsAlarmActive = PlayerStore.Instance.isSecurityAlarmActive
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackOnSleep failed: " + ex.Message);
		}
	}

	public static void TrackGameOver(string reason)
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		try
		{
			GameOverEvent e = new GameOverEvent
			{
				Reason = reason,
				Spec = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
				Day = StoreStation.Instance.dayCounter,
				RunNumber = PlayerStore.Instance.runNumber,
				RunId = PlayerStore.Instance.runID,
				RunPlayTimeSeconds = PlayerStore.Instance.GetCurrentRunPlayTime(),
				TotalPlayTimeSeconds = PlayerStore.Instance.GetCurrentTotalPlayTime()
			};
			AnalyticsService.Instance.RecordEvent(e);
			AnalyticsService.Instance.Flush();
		}
		catch (Exception ex)
		{
			Debug.LogError("TrackGameOver failed: " + ex.Message);
		}
	}

	public static void TrackStartingPerks()
	{
		if (!isAnalyticsInitialized)
		{
			return;
		}
		string startDisplayName = NewGameData.GetStartDisplayName(NewGameData.Instance.startType);
		int runNumber = PlayerStore.Instance.runNumber;
		string runID = PlayerStore.Instance.runID;
		bool skipIntro = NewGameData.Instance.skipIntro;
		bool hardMode = NewGameData.Instance.hardMode;
		bool endlessMode = NewGameData.Instance.endlessMode;
		foreach (StartingPerk startingPerk in PlayerStore.Instance.startingPerks)
		{
			try
			{
				StartingPerkSelectedEvent e = new StartingPerkSelectedEvent
				{
					PerkId = startingPerk.id,
					Spec = startDisplayName,
					RunNumber = runNumber,
					RunId = runID,
					SkipIntro = skipIntro,
					HardMode = hardMode,
					EndlessMode = endlessMode
				};
				AnalyticsService.Instance.RecordEvent(e);
			}
			catch (Exception ex)
			{
				Debug.LogError("TrackStartingPerks failed for perk " + startingPerk.id + ": " + ex.Message);
			}
		}
	}
}

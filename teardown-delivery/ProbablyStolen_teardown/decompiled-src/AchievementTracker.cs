using Steamworks;
using UnityEngine;

public class AchievementTracker : MonoBehaviour
{
	private const string ACH_TUTORIAL = "ACH_TUTORIAL";

	private const string ACH_BUYOUT = "ACH_BUYOUT";

	private const string ACH_HONEST_MERCHANT = "ACH_HONEST_MERCHANT";

	private const string ACH_AUTHORIZED = "ACH_AUTHORIZED";

	private const string ACH_RATS = "ACH_RATS";

	private const string ACH_RAG_TO_RICHES = "ACH_RAG_TO_RICHES";

	private const string ACH_NOTHING_TO_RICHES = "ACH_NOTHING_TO_RICHES";

	private const string ACH_PERFECT_MEDIATOR = "ACH_PERFECT_MEDIATOR";

	private const string ACH_LUCKY = "ACH_LUCKY";

	private const string ACH_SETTLED = "ACH_SETTLED";

	private const string ACH_RICH = "ACH_RICH";

	private const string ACH_LAB_PURE = "ACH_LAB_PURE";

	private const string ACH_HUGE_NUTRIFRUIT = "ACH_HUGE_NUTRIFRUIT";

	private const string ACH_PRODIGIOUS = "ACH_PRODIGIOUS";

	private const string ACH_SHINY = "ACH_SHINY";

	private const string ACH_OVERLOAD = "ACH_OVERLOAD";

	private const string ACH_MODEL_CITIZEN = "ACH_MODEL_CITIZEN";

	private const string ACH_AUG_HUNTER = "ACH_AUG_HUNTER";

	private const string ACH_LOWER_LEVELS_SPECIAL = "ACH_LOWER_LEVELS_SPECIAL";

	private const string ACH_MASTER_APPRAISER = "ACH_MASTER_APPRAISER";

	private const string ACH_INDUSTRIOUS = "ACH_INDUSTRIOUS";

	private const string STAT_PERFECT_APPRAISALS = "STAT_PERFECT_APPRAISALS";

	private const string STAT_CRIMINALS_REPORTED = "STAT_CRIMINALS_REPORTED";

	private const string STAT_AUGS_SHOT = "STAT_AUGS_SHOT";

	private const int WEALTH_SETTLED = 10000;

	private const int WEALTH_RICH = 100000;

	private const int ATTRACTIVENESS_SHINY = 1000;

	private const int MACHINES_INDUSTRIOUS = 10;

	private const int QUALITY_OVERLOAD = 250;

	private const int REPUTATION_MAX = 200;

	private const int PRODIGIOUS_DAY_LIMIT = 50;

	private const int PERFECT_APPRAISALS_REQUIRED = 10;

	private const int CRIMINALS_REPORTED_REQUIRED = 10;

	private const int AUGS_SHOT_REQUIRED = 10;

	private bool statsReady;

	private Callback<UserStatsReceived_t> userStatsReceived;

	public static AchievementTracker Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		UnityEngine.Debug.Log($"[AchievementTracker] SteamManager.Initialized = {SteamManager.Initialized}");
		if (SteamManager.Initialized)
		{
			userStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
			SteamUserStats.RequestUserStats(SteamUser.GetSteamID());
		}
	}

	private void Update()
	{
	}

	private void OnUserStatsReceived(UserStatsReceived_t cb)
	{
		if (cb.m_nGameID == (uint)SteamUtils.GetAppID() && cb.m_eResult == EResult.k_EResultOK)
		{
			statsReady = true;
		}
	}

	private void Unlock(string apiName)
	{
		if (SteamManager.Initialized && statsReady && !(SteamUserStats.GetAchievement(apiName, out var pbAchieved) && pbAchieved))
		{
			SteamUserStats.SetAchievement(apiName);
			SteamUserStats.StoreStats();
		}
	}

	private void IncrementCountAndUnlock(string statName, int threshold, string achApiName)
	{
		if (SteamManager.Initialized && statsReady && !(SteamUserStats.GetAchievement(achApiName, out var pbAchieved) && pbAchieved))
		{
			SteamUserStats.GetStat(statName, out int pData);
			pData++;
			SteamUserStats.SetStat(statName, pData);
			if (pData >= threshold)
			{
				SteamUserStats.SetAchievement(achApiName);
			}
			SteamUserStats.StoreStats();
		}
	}

	public void CheckTutorialComplete()
	{
		Unlock("ACH_TUTORIAL");
	}

	public void CheckBuyout()
	{
		Unlock("ACH_BUYOUT");
	}

	public void CheckHonestMerchant()
	{
		if (PlayerStore.Instance != null && !PlayerStore.Instance.soldStolenOrContrabandThisRun)
		{
			Unlock("ACH_HONEST_MERCHANT");
		}
	}

	public void CheckAuthorized()
	{
		Unlock("ACH_AUTHORIZED");
	}

	public void CheckRats()
	{
		Unlock("ACH_RATS");
	}

	public void CheckRagToRiches()
	{
		if (PlayerStore.Instance != null && PlayerStore.Instance.IsRockBottomStart())
		{
			Unlock("ACH_RAG_TO_RICHES");
		}
	}

	public void CheckNothingToRiches()
	{
		if (PlayerStore.Instance != null && PlayerStore.Instance.IsRockBottomStart() && Difficulty.IsHardOrAbove())
		{
			Unlock("ACH_NOTHING_TO_RICHES");
		}
	}

	public void CheckPerfectMediator()
	{
		if (PlayerStore.Instance == null || PlayerStore.Instance.storeReputations == null)
		{
			return;
		}
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.GetReputation() >= 200)
			{
				Unlock("ACH_PERFECT_MEDIATOR");
				break;
			}
		}
	}

	public void CheckLucky(bool isJackpot)
	{
		if (isJackpot)
		{
			Unlock("ACH_LUCKY");
		}
	}

	public void CheckSettled()
	{
		if (PlayerStore.Instance != null && PlayerStore.Instance.GetTotalEstimatedValue() > 10000)
		{
			Unlock("ACH_SETTLED");
		}
	}

	public void CheckRich()
	{
		if (PlayerStore.Instance != null && PlayerStore.Instance.GetTotalEstimatedValue() > 100000)
		{
			Unlock("ACH_RICH");
		}
	}

	public void CheckLabPure(GameItem outputItem)
	{
		if (outputItem != null && ChemicalProductHelper.GetCurrentChemicalConditionId(outputItem) == "lab_pure")
		{
			Unlock("ACH_LAB_PURE");
		}
	}

	public void CheckHugeNutrifruit()
	{
		Unlock("ACH_HUGE_NUTRIFRUIT");
	}

	public void CheckProdigious()
	{
		if (StoreStation.Instance != null && StoreStation.Instance.dayCounter < 50)
		{
			Unlock("ACH_PRODIGIOUS");
		}
	}

	public void CheckShiny()
	{
		if (PlayerStore.Instance != null && PlayerStore.Instance.GetCurrentStoreAttractiveness() >= 1000)
		{
			Unlock("ACH_SHINY");
		}
	}

	public void CheckOverload(int qualityBonus)
	{
		if (qualityBonus >= 250)
		{
			Unlock("ACH_OVERLOAD");
		}
	}

	public void CheckModelCitizen()
	{
		IncrementCountAndUnlock("STAT_CRIMINALS_REPORTED", 10, "ACH_MODEL_CITIZEN");
	}

	public void CheckAugHunter()
	{
		StoreClientInstance storeClientInstance = ((PlayerStore.Instance != null) ? PlayerStore.Instance.currentClientInstance : null);
		if (storeClientInstance != null && storeClientInstance.GetClientBlueprint() != null && storeClientInstance.GetClientBlueprint().isAug)
		{
			IncrementCountAndUnlock("STAT_AUGS_SHOT", 10, "ACH_AUG_HUNTER");
		}
	}

	public void CheckLowerLevelsSpecial()
	{
		Unlock("ACH_LOWER_LEVELS_SPECIAL");
	}

	public void CheckMasterAppraiser(bool isPerfect)
	{
		if (isPerfect)
		{
			IncrementCountAndUnlock("STAT_PERFECT_APPRAISALS", 10, "ACH_MASTER_APPRAISER");
		}
	}

	public void CheckIndustrious()
	{
		if (EmporiumEntry.Instance == null)
		{
			return;
		}
		int num = 0;
		foreach (GameItem allOwnedItem in EmporiumEntry.Instance.GetAllOwnedItems())
		{
			if (allOwnedItem.IsGameItemType("MACHINE"))
			{
				num++;
			}
		}
		if (num >= 10)
		{
			Unlock("ACH_INDUSTRIOUS");
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreClientData
{
	public const int GEORGIO_STORYLINE_STARTING_DAY = 6;

	public const int LUN_STORYLINE_STARTING_DAY = 11;

	public const int ELEANOR_STORYLINE_STARTING_DAY = 23;

	public const int NELLIS_STORYLINE_STARTING_DAY = 21;

	public const int SERPICO_STORYLINE_STARTING_DAY = 40;

	[ES3Serializable]
	public Dictionary<string, int> storylineAnchors = new Dictionary<string, int>();

	[ES3Serializable]
	public bool storylineAnchorsSeeded;

	[ES3Serializable]
	public int scalperFirstVisitDay = -1;

	[ES3Serializable]
	public int lunAddictionLevel;

	[ES3Serializable]
	public bool officerLunArrested;

	[ES3Serializable]
	public bool isJuanMet;

	public const int LI_SUPPLY_VISIT = 1;

	public const int LI_FINALE_VISIT = 2;

	public const int LI_FIRST_RETURN_DELAY = 3;

	public const int LI_SUPPLY_RETRY_DELAY = 7;

	public const int LI_FINALE_BASE_DAYS = 17;

	public const int LI_FINALE_MIN_DAYS = 2;

	public const int LI_DAYS_PER_COMMON = 4;

	public const int LI_DAYS_PER_RESTRICTED = 7;

	[ES3Serializable]
	public int liNextVisitDay = -1;

	[ES3Serializable]
	public int liNextVisit;

	[ES3Serializable]
	public bool isJuanReportPleaDone;

	[ES3Serializable]
	public bool isButcherReportWarningDone;

	[ES3Serializable]
	public bool isSawyerDead;

	[ES3Serializable]
	public bool isEleanorDead;

	public const int BUTCHER_RETURN_INTERVAL = 7;

	[ES3Serializable]
	public int butcherMetDay = -1;

	public const int BUTCHER_VANDALISM_DELAY = 7;

	[ES3Serializable]
	public int butcherArrestedDay = -1;

	[ES3Serializable]
	public bool isButcherVandalismDone;

	public const int BUTCHER_CLEAVER_DELAY = 3;

	[ES3Serializable]
	public int butcherVandalismMorningDay = -1;

	[ES3Serializable]
	public bool isButcherCleaverLeft;

	[ES3Serializable]
	public int butcherCleaverDay = -1;

	public const int SAWYER_BUTCHER_OFFER_DELAY = 1;

	public const int BUTCHER_HUNTING_COST = 500;

	[ES3Serializable]
	public bool isSawyerButcherOfferDone;

	[ES3Serializable]
	public bool isButcherHuntOver;

	public static bool butcherTestMode;

	public const int LOU_TEMPTING_CASH = 75;

	public const int LOU_FIRST_VISIT_DEADLINE = 14;

	public const int LOU_ESCAPE_DELAY = 3;

	[ES3Serializable]
	public int louEligibleDay = -1;

	[ES3Serializable]
	public int louFirstVisitDay = -1;

	[ES3Serializable]
	public int louMetDay = -1;

	[ES3Serializable]
	public int louArrestedDay = -1;

	[ES3Serializable]
	public int louShotDay = -1;

	[ES3Serializable]
	public int louOperativeMetDay = -1;

	[ES3Serializable]
	public string louOperativeSprite = "";

	[ES3Serializable]
	public bool isLouTransmitterGiven;

	[ES3Serializable]
	public bool isLouAmmoLeft;

	[ES3Serializable]
	public int louLockerDay = -1;

	[ES3Serializable]
	public bool isLouLockerShort;

	[ES3Serializable]
	public int louLockerPaidDay = -1;

	[ES3Serializable]
	public int louCapturedDay = -1;

	[ES3Serializable]
	public bool isLouOperativeRewardGiven;

	[ES3Serializable]
	public bool isLouOperativeSuspicionApplied;

	[ES3Serializable]
	public bool isLouOperativeQuestioned;

	[ES3Serializable]
	public int louRevealDay = -1;

	[ES3Serializable]
	public int louTrapDay = -1;

	[ES3Serializable]
	public bool isLouTrapWon;

	[ES3Serializable]
	public bool isLouThankYouGiven;

	public static bool louTestMode;

	[ES3Serializable]
	public int louMorningDay = -1;

	public const int LOU_BOX_DROP_DELAY = 4;

	public const int LOU_BOX_COLLECT_DELAY = 1;

	[ES3Serializable]
	public List<string> louBoughtItemIds = new List<string>();

	[ES3Serializable]
	public int louBombVisitDay = -1;

	[ES3Serializable]
	public int louBoxDropDay = -1;

	[ES3Serializable]
	public int louBoxCollectDay = -1;

	[ES3Serializable]
	public bool isLouBoxNotPlaced;

	[ES3Serializable]
	public bool isLouBombPlaced;

	[ES3Serializable]
	public bool isLouBombVisitPending;

	public const int LOU_DISGUISE_DELAY = 4;

	public const int LOU_DEMAND_DELAY = 7;

	[Obsolete("The minimum is half her demand, which scales with difficulty: use GetLouDemandMinimum()")]
	public const int LOU_DEMAND_MINIMUM = 500;

	[ES3Serializable]
	public int louDisguiseDay = -1;

	[ES3Serializable]
	public int louDemandGiven;

	[ES3Serializable]
	public int louDemandDay = -1;

	public const int BAILEY_RETURN_INTERVAL = 14;

	[ES3Serializable]
	public int jamesBaileyMetDay = -1;

	public const int JOHN_SMITH_WEEKLY_INTERVAL = 7;

	[ES3Serializable]
	public int johnSmithMetDay = -1;

	public const int JOHN_SMITH_RETURN_DELAY = 2;

	[ES3Serializable]
	public int johnSmithReportedDay = -1;

	[ES3Serializable]
	public bool isJohnSmithReturnQueued;

	[ES3Serializable]
	public int johnSmithReturnReportedDay = -1;

	[ES3Serializable]
	public bool isJohnSmithVandalismDone;

	[ES3Serializable]
	public bool isJohnSmithVandalVisitPending;

	[ES3Serializable]
	public int lotteryMetTime;

	[ES3Serializable]
	public bool haveVendorPermit;

	[ES3Serializable]
	public bool vendorPermitIntroSeen;

	[ES3Serializable]
	public int eleanorFund = 500;

	[ES3Serializable]
	public int eleanorHelpedTimes;

	[ES3Serializable]
	public bool eleanorArrested;

	public const int ELEANOR_BAIL_OFFICER_DELAY = 2;

	public const int ELEANOR_FREED_DELAY = 1;

	public const int ELEANOR_BAIL_COST = 500;

	public const int ELEANOR_BAIL_SEC_REP_REQUIRED = 40;

	public const int ELEANOR_BAIL_NONE = 0;

	public const int ELEANOR_BAIL_PAID = 1;

	public const int ELEANOR_BAIL_VOUCHED = 2;

	public const int ELEANOR_BAIL_DECLINED = 3;

	[ES3Serializable]
	public int eleanorArrestDay = -1;

	[ES3Serializable]
	public int eleanorArrestVisit;

	[ES3Serializable]
	public int eleanorBailChoice;

	[ES3Serializable]
	public int eleanorBailDay = -1;

	[ES3Serializable]
	public int eleanorFreedVisitDay = -1;

	public const int ELEANOR_WARNED_RETURN_DELAY = 3;

	[ES3Serializable]
	public bool eleanorUncleReported;

	[ES3Serializable]
	public int eleanorWarnedDay = -1;

	public const int ELEANOR_PLAN_DELAY = 4;

	public const int ELEANOR_SCAV_INTERVAL = 4;

	public const int ELEANOR_INVESTIGATOR_COST = 800;

	[ES3Serializable]
	public int eleanorSavings;

	[ES3Serializable]
	public int eleanorPlanDay = -1;

	[ES3Serializable]
	public int eleanorScavLastDay = -1;

	public const int ELEANOR_DRIVE_DELAY = 4;

	[ES3Serializable]
	public int eleanorSavingsDoneDay = -1;

	[ES3Serializable]
	public int eleanorDriveVisitDay = -1;

	public const int ELEANOR_THREAT_DELAY = 1;

	public const int ELEANOR_DRIVE_BOUNTY = 300;

	[ES3Serializable]
	public int eleanorThreatVisitDay = -1;

	public const int ELEANOR_CHOICE_DELAY = 1;

	public const int ELEANOR_CONCLUSION_DELAY = 2;

	public const int ELEANOR_CHOICE_NONE = 0;

	public const int ELEANOR_CHOICE_RETURNED = 1;

	public const int ELEANOR_CHOICE_LEAKED = 2;

	[ES3Serializable]
	public int eleanorDriveChoice;

	[ES3Serializable]
	public int eleanorChoiceDay = -1;

	[ES3Serializable]
	public int eleanorConclusionDay = -1;

	[ES3Serializable]
	public bool nutrifruitSeedsFreed;

	[ES3Serializable]
	public int jacksonInvestment;

	[ES3Serializable]
	public bool isModuleIntroduced;

	[ES3Serializable]
	public bool isJacksonIntroduced;

	[ES3Serializable]
	public int businessPermitOfficialState;

	public int businessPermitBribeAmount;

	[ES3Serializable]
	public int nuclearWasteDumped;

	[ES3Serializable]
	public bool georgioNuclearWasteVisited;

	[ES3Serializable]
	public int nuclearWasteLeftOnCounter;

	[ES3Serializable]
	public bool caseEnjoyerVisited;

	[ES3Serializable]
	public int caseEnjoyerReturnDay = -1;

	[ES3Serializable]
	public bool caseEnjoyerReturned;

	[ES3Serializable]
	public bool serpicoReported;

	[ES3Serializable]
	public bool serpicoLetterDelivered;

	[ES3Serializable]
	public int serpicoFoodKcal;

	[ES3Serializable]
	public int serpicoFoodExtraDays;

	[ES3Serializable]
	public int serpicoFoodExtraDayLastDay = -1;

	[ES3Serializable]
	public int serpicoDocumentChoice;

	[ES3Serializable]
	public int serpicoTestStartDay = -1;

	public static int GetLiFinaleDelay(int common, int restricted)
	{
		return Mathf.Max(2, 17 - common * 4 - restricted * 7);
	}

	public static void ScheduleLiVisit(int visit, int delayDays)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		storeClientData.liNextVisit = visit;
		storeClientData.liNextVisitDay = StoreStation.Instance.dayCounter + delayDays;
	}

	public static bool IsButcherHunting()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.isButcherCleaverLeft)
		{
			return !storeClientData.isButcherHuntOver;
		}
		return false;
	}

	public static bool CanSawyerOfferButcherHelp(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (IsButcherHunting() && !storeClientData.isSawyerButcherOfferDone && storeClientData.butcherCleaverDay >= 0)
		{
			return day >= storeClientData.butcherCleaverDay + ButcherDelay(1);
		}
		return false;
	}

	public static int ButcherDelay(int days)
	{
		if (!butcherTestMode)
		{
			return days;
		}
		return 1;
	}

	public static bool CanButcherFirstVisitForTest()
	{
		if (butcherTestMode && PlayerStore.Instance.storeClientData.butcherMetDay < 0)
		{
			return !IsButcherCaptured();
		}
		return false;
	}

	public static void HealButcherArrestAfterLoad()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData != null && storeClientData.butcherArrestedDay < 0 && IsButcherCaptured())
		{
			storeClientData.butcherArrestedDay = StoreStation.Instance.dayCounter;
		}
	}

	private static bool IsButcherCaptured()
	{
		if (PlayerStore.Instance.wantedStatus.TryGetValue("wanted2", out var value))
		{
			return value == 1;
		}
		return false;
	}

	public static bool CanButcherReturn(int day)
	{
		int num = PlayerStore.Instance.storeClientData.butcherMetDay;
		if (num < 0 || day <= num)
		{
			return false;
		}
		if ((day - num) % ButcherDelay(7) != 0)
		{
			return false;
		}
		return !IsButcherCaptured();
	}

	public static int LouDelay(int days)
	{
		if (!louTestMode)
		{
			return days;
		}
		return 1;
	}

	public static void ScheduleLouFirstVisit(bool isPermitPurchase = false)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		int dayCounter = StoreStation.Instance.dayCounter;
		if (!louTestMode && storeClientData.louFirstVisitDay >= 0)
		{
			return;
		}
		if (louTestMode)
		{
			if (!isPermitPurchase && storeClientData.louMetDay < 0 && (storeClientData.louFirstVisitDay < 0 || storeClientData.louFirstVisitDay > dayCounter))
			{
				storeClientData.louFirstVisitDay = dayCounter;
			}
		}
		else if (PlayerStore.Instance.hasBusinessPermit)
		{
			if (storeClientData.louEligibleDay < 0)
			{
				storeClientData.louEligibleDay = (isPermitPurchase ? (dayCounter + 1) : dayCounter);
			}
			if (!isPermitPurchase && dayCounter >= storeClientData.louEligibleDay && (IsLouTemptingMorning() || dayCounter >= storeClientData.louEligibleDay + 14))
			{
				storeClientData.louFirstVisitDay = dayCounter;
			}
		}
	}

	private static bool IsLouTemptingMorning()
	{
		PlayerStore instance = PlayerStore.Instance;
		if (instance.playerCash < 75)
		{
			return true;
		}
		if (instance.GetRentDayCounter() != 0 || instance.IsPropertyPaid)
		{
			return false;
		}
		if (!instance.isPayingMortgage)
		{
			return !instance.CanPayRent();
		}
		return instance.playerCash < instance.mortgageWeeklyPayment;
	}

	public static bool CanLouFirstVisit(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.louFirstVisitDay >= 0 && day >= storeClientData.louFirstVisitDay && storeClientData.louMetDay < 0)
		{
			if (PlayerStore.Instance.wantedStatus.TryGetValue("wanted5", out var value))
			{
				return value != 1;
			}
			return true;
		}
		return false;
	}

	public static bool CanLouMorningVisit(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (LouOperativeHelper.IsLouGamesStopped())
		{
			return false;
		}
		if (storeClientData.louArrestedDay >= 0 && storeClientData.louMorningDay < 0)
		{
			return day >= storeClientData.louArrestedDay + LouDelay(3);
		}
		return false;
	}

	public static bool CanLouDropBox(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (LouOperativeHelper.IsLouGamesStopped())
		{
			return false;
		}
		if (storeClientData.louBombVisitDay >= 0 && storeClientData.louBoxDropDay < 0)
		{
			return day >= storeClientData.louBombVisitDay + LouDelay(4);
		}
		return false;
	}

	public static bool CanLouCollectBox(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (LouOperativeHelper.IsLouGamesStopped())
		{
			return false;
		}
		if (storeClientData.louBoxDropDay >= 0 && storeClientData.louBoxCollectDay < 0)
		{
			return day >= storeClientData.louBoxDropDay + LouDelay(1);
		}
		return false;
	}

	public static bool CanLouDisguise(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (LouOperativeHelper.IsLouGamesStopped())
		{
			return false;
		}
		if (storeClientData.louBoxCollectDay >= 0 && storeClientData.louDisguiseDay < 0)
		{
			return day >= storeClientData.louBoxCollectDay + LouDelay(4);
		}
		return false;
	}

	public static bool CanLouCollectDemand(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (LouOperativeHelper.IsLouGamesStopped())
		{
			return false;
		}
		if (storeClientData.louDisguiseDay >= 0 && storeClientData.louDemandDay < 0)
		{
			return day >= storeClientData.louDisguiseDay + LouDelay(7);
		}
		return false;
	}

	public static int GetLouDemandValue()
	{
		return Difficulty.Rules.louDemandValue;
	}

	public static int GetLouDemandMinimum()
	{
		return GetLouDemandValue() / 2;
	}

	public static int GetLouDemandRemaining()
	{
		return Mathf.Max(0, GetLouDemandValue() - PlayerStore.Instance.storeClientData.louDemandGiven);
	}

	public static bool CanBaileyReturn(int day)
	{
		int num = PlayerStore.Instance.storeClientData.jamesBaileyMetDay;
		if (num < 0 || day <= num)
		{
			return false;
		}
		if ((day - num) % 14 != 0)
		{
			return false;
		}
		if (PlayerStore.Instance.wantedStatus.TryGetValue("wanted1", out var value))
		{
			return value != 1;
		}
		return true;
	}

	public static bool CanJohnSmithWeekly(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.johnSmithMetDay < 0 || day <= storeClientData.johnSmithMetDay)
		{
			return false;
		}
		if ((day - storeClientData.johnSmithMetDay) % 7 != 0)
		{
			return false;
		}
		if (PlayerStore.Instance.wantedStatus.TryGetValue("wanted8", out var value) && value == 1)
		{
			return false;
		}
		return storeClientData.johnSmithReportedDay < 0;
	}

	public static void MigrateEleanorBail(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.eleanorArrested && storeClientData.eleanorArrestDay < 0)
		{
			storeClientData.eleanorArrestDay = day;
			if (storeClientData.eleanorArrestVisit == 0)
			{
				storeClientData.eleanorArrestVisit = 5;
			}
			Debug.Log("[Eleanor] Arrest from before the bail update anchored on day " + day + ": the bail officer comes in " + 2 + " days");
		}
	}

	public static bool CanEleanorBailOfficerVisit(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.eleanorArrested && storeClientData.eleanorArrestDay >= 0 && storeClientData.eleanorBailChoice == 0)
		{
			return day >= storeClientData.eleanorArrestDay + 2;
		}
		return false;
	}

	public static bool CanEleanorFreedVisit(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if ((storeClientData.eleanorBailChoice == 1 || storeClientData.eleanorBailChoice == 2) && storeClientData.eleanorFreedVisitDay < 0)
		{
			return day >= storeClientData.eleanorBailDay + 1;
		}
		return false;
	}

	public static int GetEleanorVisit4Day()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (!storeClientData.eleanorUncleReported)
		{
			return StorylineSchedule.StartDay("eleanor") + 6;
		}
		if (storeClientData.eleanorWarnedDay >= 0)
		{
			return storeClientData.eleanorWarnedDay + 3;
		}
		return -1;
	}

	public static int GetEleanorVisit5Day()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.eleanorArrestVisit == 4)
		{
			if (storeClientData.eleanorFreedVisitDay >= 0)
			{
				return storeClientData.eleanorFreedVisitDay + 2;
			}
			return -1;
		}
		int eleanorVisit4Day = GetEleanorVisit4Day();
		if (eleanorVisit4Day >= 0)
		{
			return eleanorVisit4Day + 2;
		}
		return -1;
	}

	public static int GetEleanorPlanBaseDay()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.eleanorArrestVisit == 5)
		{
			return storeClientData.eleanorFreedVisitDay;
		}
		return GetEleanorVisit5Day();
	}

	public static bool CanEleanorWarnedVisit(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.eleanorUncleReported && storeClientData.eleanorWarnedDay < 0 && !storeClientData.eleanorArrested)
		{
			return day >= StorylineSchedule.StartDay("eleanor") + 6;
		}
		return false;
	}

	public static int GetEleanorSavingsRemaining()
	{
		return Mathf.Max(0, 800 - PlayerStore.Instance.storeClientData.eleanorSavings);
	}

	public static bool CanEleanorPlanVisit(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		int eleanorPlanBaseDay = GetEleanorPlanBaseDay();
		if (storeClientData.eleanorPlanDay < 0 && storeClientData.eleanorHelpedTimes > 0 && !storeClientData.eleanorArrested && eleanorPlanBaseDay >= 0)
		{
			return day >= eleanorPlanBaseDay + 4;
		}
		return false;
	}

	public static bool CanEleanorThreatVisit(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.eleanorDriveVisitDay >= 0 && storeClientData.eleanorThreatVisitDay < 0)
		{
			return day >= storeClientData.eleanorDriveVisitDay + 1;
		}
		return false;
	}

	public static bool CanEleanorChoiceVisit(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.eleanorThreatVisitDay >= 0 && storeClientData.eleanorDriveChoice == 0)
		{
			return day >= storeClientData.eleanorThreatVisitDay + 1;
		}
		return false;
	}

	public static bool CanEleanorConclusionVisit(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.eleanorDriveChoice != 0 && storeClientData.eleanorConclusionDay < 0)
		{
			return day >= storeClientData.eleanorChoiceDay + 2;
		}
		return false;
	}

	public static bool CanEleanorDriveVisit(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.eleanorSavingsDoneDay >= 0 && storeClientData.eleanorDriveVisitDay < 0 && !storeClientData.eleanorArrested)
		{
			return day >= storeClientData.eleanorSavingsDoneDay + 4;
		}
		return false;
	}

	public static bool CanEleanorScavVisit(int day)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.eleanorPlanDay < 0 || storeClientData.eleanorSavings >= 800 || storeClientData.eleanorArrested)
		{
			return false;
		}
		int num = Mathf.Max(storeClientData.eleanorPlanDay, storeClientData.eleanorScavLastDay);
		return day >= num + 4;
	}

	public static bool CanRecurrentJuanAppear()
	{
		if (!PlayerStore.Instance.storeClientData.isJuanMet)
		{
			return false;
		}
		if (PlayerStore.Instance.wantedStatus.TryGetValue("wanted7", out var value))
		{
			return value != 1;
		}
		return true;
	}

	public static void AcquiredVendorPermit()
	{
		PlayerStore.Instance.storeClientData.haveVendorPermit = true;
		Debug.Log("Player acquired vendor permit.");
	}

	public static int GetDrJacksonInvestmentBonus()
	{
		return PlayerStore.Instance.storeClientData.jacksonInvestment;
	}
}

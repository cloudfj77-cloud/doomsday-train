using UnityEngine;

public class StoreReputationPerk
{
	[ES3Serializable]
	public string perkId;

	[ES3Serializable]
	public int perkThreshold;

	[ES3Serializable]
	public int perkType;

	[ES3Serializable]
	public bool isUnlocked;

	[ES3Serializable]
	public string iconName;

	[ES3Serializable]
	public bool firstTimeUnlockActionDone;

	[ES3Serializable]
	public bool isAlwaysActive;

	public const int LL_REFERRAL_CHANCE = 100;

	public const int LL_REFERRAL_CUSTOMER_COUNT = 1;

	private const float LL_TIGHTLIPS_EVIDENCE_MULTIPLIER = 0.5f;

	private const int LL_PILLAR_DISCOUNT = 15;

	public const int LL_POOR_REVIEWS_CHANCE = 100;

	private const int LL_POOR_REVIEWS_CUSTOMER_COUNT = 1;

	public const int LL_TARGET_INSURANCE_MULTIPLIER = 2;

	private const float SEC_WATCHLIST_INVESTIGATION_MULTIPLIER = 1.2f;

	private const float SEC_PRIORITY_INVESTIGATION_MULTIPLIER = 1.5f;

	private const int SEC_RELIABLE_VENDOR_BUDGET_INCREASE = 50;

	private const float BM_HONOR_REPUTATION_LOSS_MULTIPLIER = 1.5f;

	private const int UL_TIPS_BONUS = 15;

	private const int UL_HOUSEHOLD_NAME_CHANCE = 20;

	private const int UL_HOUSEHOLD_NAME_CUSTOMER_COUNT = 1;

	private const int UL_TOP_REVIEW_BUDGET_INCREASE = 25;

	private const float UL_PETITION_REPUTATION_LOSS_MULTIPLIER = 2f;

	private const float UL_PETITION_INVESTIGATION_MULTIPLIER = 1.5f;

	private const int UL_DISCERNING_TASTE_BASE_CHANCE = 1;

	private const int UL_DISCERNING_TASTE_REP_DIVISOR = 5;

	public const int UL_DISCERNING_TASTE_COOLDOWN = 5;

	private const int CARTEL_RELIABLE_CHEMIST_COST_REDUCTION = 50;

	private static readonly object[] NoArgs = new object[0];

	private static readonly object[] ArgsLLReferral = new object[2] { 100, 1 };

	private static readonly object[] ArgsLLTightLips = new object[1] { 0.5f };

	private static readonly object[] ArgsLLPillar = new object[1] { 15 };

	private static readonly object[] ArgsLLPoorReviews = new object[2] { 100, 1 };

	private static readonly object[] ArgsLLTarget = new object[1] { 2 };

	private static readonly object[] ArgsSecWatchlist = new object[1] { 1.2f };

	private static readonly object[] ArgsSecPriority = new object[1] { 1.5f };

	private static readonly object[] ArgsSecReliableVendor = new object[1] { 50 };

	private static readonly object[] ArgsBMHonor = new object[1] { 1.5f };

	private static readonly object[] ArgsULTips = new object[1] { 15 };

	private static readonly object[] ArgsULHouseholdName = new object[2] { 20, 1 };

	private static readonly object[] ArgsULTopReview = new object[1] { 25 };

	private static readonly object[] ArgsULPetition = new object[2] { 2f, 1.5f };

	private static readonly object[] ArgsCartelReliableChemist = new object[1] { 50 };

	public const string LLREFERAL = "LLREFERAL";

	public const string LLTIPS = "LLTIPS";

	public const string LLRAREGOOD = "LLRAREGOOD";

	public const string LLTIGHTLIPS = "LLTIGHTLIPS";

	public const string LLPILLAR = "LLPILLAR";

	public const string LLPOORREVIEWS = "LLPOORREVIEWS";

	public const string LLDISTRUSTED = "LLDISTRUSTED";

	public const string LLTARGET = "LLTARGET";

	public const string LLLYNCH = "LLLYNCH";

	public const string LLAVOIDED = "LLAVOIDED";

	public const string SECALARM_IGNORED = "SEC_ALARM_IGNORED";

	public const string SEC_WATCHLIST = "SEC_WATCHLIST";

	public const string SEC_PRIORITY = "SEC_PRIORITY";

	public const string SEC_THOROUGH_INSPECTION = "SEC_THOROUGH_INSPECTION";

	public const string SEC_CONTRABAND_TOLERANCE = "SEC_CONTRABAND_TOLERANCE";

	public const string SEC_IMMEDIATE_ARREST = "SEC_IMMEDIATE_ARREST";

	public const string SEC_PROTECTION = "SEC_PROTECTION";

	public const string SEC_SURPLUS = "SEC_SURPLUS";

	public const string SEC_VIP = "SEC_VIP";

	public const string SEC_TRUSTED = "SEC_TRUSTED";

	public const string SEC_REQUISITION = "SEC_REQUISITION";

	public const string SEC_RELIABLE_VENDOR = "SEC_RELIABLE_VENDOR";

	public const string BM_Distrusted = "BM_DISTRUSTED";

	public const string BM_UNINSURABLE = "BM_UNINSURABLE";

	public const string BM_SMALL_TIME_BROKER = "BM_SMALL_TIME_BROKER";

	public const string BM_RELIABLE = "BM_RELIABLE";

	public const string BM_NETWORK = "BM_NETWORK";

	public const string BM_HONOR = "BM_HONOR";

	public const string UL_GOSSIP = "UL_GOSSIP";

	public const string UL_TIPS = "UL_TIPS";

	public const string UL_HOUSEHOLD_NAME = "UL_HOUSEHOLD_NAME";

	public const string UL_TOP_REVIEW = "UL_TOP_REVIEW";

	public const string UL_CHARITY = "UL_CHARITY";

	public const string UL_DISCERNING_TASTE = "UL_DISCERNING_TASTE";

	public const string UL_TOURISTTRAP = "UL_TOURISTTRAP";

	public const string UL_HITMAN = "UL_HITMAN";

	public const string UL_PETITION = "UL_PETITION";

	public const string REV_HIT = "REV_HIT";

	public const string REV_BLACKLISTED = "REV_BLACKLISTED";

	public const string REV_REQUEST = "REV_REQUEST";

	public const string REV_RAID_LOOT = "REV_RAID_LOOT";

	public const string REV_QUARTERMASTER = "REV_QUARTERMASTER";

	public const string REV_FRIEND = "REV_FRIEND";

	public const string CARTEL_RETIRED = "CARTEL_RETIRED";

	public const string CARTEL_INCREASED_STIPEND = "CARTEL_INCREASED_STIPEND";

	public const string CARTEL_RELIABLE_CHEMIST = "CARTEL_RELIABLE_CHEMIST";

	public const string CARTEL_AUTHORIZED_EXIT = "CARTEL_AUTHORIZED_EXIT";

	public string GetLocalizedPerkName()
	{
		return LocHelper.GetLocalizedRepPerkTable("rep_perk_" + perkId + "_name");
	}

	public string GetLocalizedDescription()
	{
		return LocHelper.GetLocalizedRepPerkTable("rep_perk_" + perkId + "_desc", GetDescriptionArguments());
	}

	public string GetLocalizedFlavorText()
	{
		return LocHelper.GetLocalizedRepPerkTable("rep_perk_" + perkId + "_flavor");
	}

	public static int GetDiscerningTasteChance()
	{
		return 1 + Mathf.Max(0, StoreReputation.GetULReputation()) / 5;
	}

	private object[] GetDescriptionArguments()
	{
		return perkId switch
		{
			"LLREFERAL" => ArgsLLReferral, 
			"LLTIGHTLIPS" => ArgsLLTightLips, 
			"LLPILLAR" => ArgsLLPillar, 
			"LLPOORREVIEWS" => ArgsLLPoorReviews, 
			"LLTARGET" => ArgsLLTarget, 
			"SEC_WATCHLIST" => ArgsSecWatchlist, 
			"SEC_PRIORITY" => ArgsSecPriority, 
			"SEC_RELIABLE_VENDOR" => ArgsSecReliableVendor, 
			"BM_HONOR" => ArgsBMHonor, 
			"UL_TIPS" => ArgsULTips, 
			"UL_HOUSEHOLD_NAME" => ArgsULHouseholdName, 
			"UL_TOP_REVIEW" => ArgsULTopReview, 
			"UL_PETITION" => ArgsULPetition, 
			"CARTEL_RELIABLE_CHEMIST" => ArgsCartelReliableChemist, 
			"UL_DISCERNING_TASTE" => new object[2]
			{
				GetDiscerningTasteChance(),
				5
			}, 
			_ => NoArgs, 
		};
	}

	public static StoreReputationPerk LLReferral()
	{
		return new StoreReputationPerk
		{
			perkId = "LLREFERAL",
			perkThreshold = 20,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk LLTips()
	{
		return new StoreReputationPerk
		{
			perkId = "LLTIPS",
			perkThreshold = 60,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk LLRareGood()
	{
		return new StoreReputationPerk
		{
			perkId = "LLRAREGOOD",
			perkThreshold = 40,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk LLTightLips()
	{
		return new StoreReputationPerk
		{
			perkId = "LLTIGHTLIPS",
			perkThreshold = 80,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk LLPillar()
	{
		return new StoreReputationPerk
		{
			perkId = "LLPILLAR",
			perkThreshold = 100,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk LLPoorReviews()
	{
		return new StoreReputationPerk
		{
			perkId = "LLPOORREVIEWS",
			perkThreshold = -20,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk LLDistrusted()
	{
		return new StoreReputationPerk
		{
			perkId = "LLDISTRUSTED",
			perkThreshold = -40,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk LLTarget()
	{
		return new StoreReputationPerk
		{
			perkId = "LLTARGET",
			perkThreshold = -60,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk LLAvoided()
	{
		return new StoreReputationPerk
		{
			perkId = "LLAVOIDED",
			perkThreshold = -80,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk LLLynch()
	{
		return new StoreReputationPerk
		{
			perkId = "LLLYNCH",
			perkThreshold = -100,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk SecRequisition()
	{
		return new StoreReputationPerk
		{
			perkId = "SEC_REQUISITION",
			perkThreshold = 0,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk SecAlarmIgnored()
	{
		return new StoreReputationPerk
		{
			perkId = "SEC_ALARM_IGNORED",
			perkThreshold = -40,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk SecWatchlist()
	{
		return new StoreReputationPerk
		{
			perkId = "SEC_WATCHLIST",
			perkThreshold = -20,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk SecPriority()
	{
		return new StoreReputationPerk
		{
			perkId = "SEC_PRIORITY",
			perkThreshold = -60,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk SecThoroughInspection()
	{
		return new StoreReputationPerk
		{
			perkId = "SEC_THOROUGH_INSPECTION",
			perkThreshold = -80,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk SecContrabandTolerance()
	{
		return new StoreReputationPerk
		{
			perkId = "SEC_CONTRABAND_TOLERANCE",
			perkThreshold = -100,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk SecImmediateArrest()
	{
		return new StoreReputationPerk
		{
			perkId = "SEC_IMMEDIATE_ARREST",
			perkThreshold = -100,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk SecProtection()
	{
		return new StoreReputationPerk
		{
			perkId = "SEC_PROTECTION",
			perkThreshold = 40,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk SecSurplus()
	{
		return new StoreReputationPerk
		{
			perkId = "SEC_SURPLUS",
			perkThreshold = 20,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk SecReliableVendor()
	{
		return new StoreReputationPerk
		{
			perkId = "SEC_RELIABLE_VENDOR",
			perkThreshold = 80,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk SecVIP()
	{
		return new StoreReputationPerk
		{
			perkId = "SEC_VIP",
			perkThreshold = 100,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk SecTrusted()
	{
		return new StoreReputationPerk
		{
			perkId = "SEC_TRUSTED",
			perkThreshold = 60,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk BMNetwork()
	{
		return new StoreReputationPerk
		{
			perkId = "BM_NETWORK",
			perkThreshold = 60,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk BMDistrusted()
	{
		return new StoreReputationPerk
		{
			perkId = "BM_DISTRUSTED",
			perkThreshold = -40,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk BMUninsurable()
	{
		return new StoreReputationPerk
		{
			perkId = "BM_UNINSURABLE",
			perkThreshold = -60,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk BMBroker()
	{
		return new StoreReputationPerk
		{
			perkId = "BM_SMALL_TIME_BROKER",
			perkThreshold = 20,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk BMHonor()
	{
		return new StoreReputationPerk
		{
			perkId = "BM_HONOR",
			perkThreshold = 0,
			perkType = 1,
			iconName = "Referal",
			isAlwaysActive = true
		};
	}

	public static StoreReputationPerk BMReliable()
	{
		return new StoreReputationPerk
		{
			perkId = "BM_RELIABLE",
			perkThreshold = 40,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk ULDiscerningTaste()
	{
		return new StoreReputationPerk
		{
			perkId = "UL_DISCERNING_TASTE",
			perkThreshold = 0,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk ULGossip()
	{
		return new StoreReputationPerk
		{
			perkId = "UL_GOSSIP",
			perkThreshold = 60,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk ULTips()
	{
		return new StoreReputationPerk
		{
			perkId = "UL_TIPS",
			perkThreshold = 20,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk ULCharity()
	{
		return new StoreReputationPerk
		{
			perkId = "UL_CHARITY",
			perkThreshold = 40,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk ULHouseholdName()
	{
		return new StoreReputationPerk
		{
			perkId = "UL_HOUSEHOLD_NAME",
			perkThreshold = 80,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk ULTopReview()
	{
		return new StoreReputationPerk
		{
			perkId = "UL_TOP_REVIEW",
			perkThreshold = 100,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk ULTouristTrap()
	{
		return new StoreReputationPerk
		{
			perkId = "UL_TOURISTTRAP",
			perkThreshold = -40,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk ULPetition()
	{
		return new StoreReputationPerk
		{
			perkId = "UL_PETITION",
			perkThreshold = -60,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk ULHitman()
	{
		return new StoreReputationPerk
		{
			perkId = "UL_HITMAN",
			perkThreshold = -100,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk RevFriend()
	{
		return new StoreReputationPerk
		{
			perkId = "REV_FRIEND",
			perkThreshold = 60,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk RevHit()
	{
		return new StoreReputationPerk
		{
			perkId = "REV_HIT",
			perkThreshold = -100,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk RevBlacklisted()
	{
		return new StoreReputationPerk
		{
			perkId = "REV_BLACKLISTED",
			perkThreshold = -40,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk RevRequest()
	{
		return new StoreReputationPerk
		{
			perkId = "REV_REQUEST",
			perkThreshold = 20,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk RevRaidLoot()
	{
		return new StoreReputationPerk
		{
			perkId = "REV_RAID_LOOT",
			perkThreshold = 80,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk RevQuartermaster()
	{
		return new StoreReputationPerk
		{
			perkId = "REV_QUARTERMASTER",
			perkThreshold = 40,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk CartelRetired()
	{
		return new StoreReputationPerk
		{
			perkId = "CARTEL_RETIRED",
			perkThreshold = -100,
			perkType = -1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk CartelIncreasedStipend()
	{
		return new StoreReputationPerk
		{
			perkId = "CARTEL_INCREASED_STIPEND",
			perkThreshold = 25,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk CartelReliableChemist()
	{
		return new StoreReputationPerk
		{
			perkId = "CARTEL_RELIABLE_CHEMIST",
			perkThreshold = 50,
			perkType = 1,
			iconName = "Referal"
		};
	}

	public static StoreReputationPerk CartelAuthorizedExit()
	{
		return new StoreReputationPerk
		{
			perkId = "CARTEL_AUTHORIZED_EXIT",
			perkThreshold = 100,
			perkType = 1,
			iconName = "Referal"
		};
	}
}

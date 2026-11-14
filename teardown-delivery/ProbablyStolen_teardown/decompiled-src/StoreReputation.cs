using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class StoreReputation
{
	[ES3Serializable]
	private double amount;

	[ES3Serializable]
	public string factionDisplay;

	[ES3Serializable]
	public string factionId;

	[ES3Serializable]
	public string factionColor1;

	[ES3Serializable]
	public string factionColor2;

	[ES3Serializable]
	public string factionColor3;

	[ES3Serializable]
	public List<StoreReputationPerk> perks;

	public const int REP_TRADE_DIVISOR = 400;

	[ES3Serializable]
	public int strength;

	[ES3Serializable]
	public int influence;

	[ES3Serializable]
	public int maxInfluence;

	[ES3Serializable]
	public int actionCooldown;

	[ES3Serializable]
	public bool canVandalized = true;

	[ES3Serializable]
	public int currentReportPenalty = -8;

	[ES3Serializable]
	public float repLossMultiplier = 1f;

	public const string FACTION_LOWER_LEVEL = "FACTION_LOWER_LEVEL";

	public const string FACTION_UPPER_LEVEL = "FACTION_UPPER_LEVEL";

	public const string FACTION_SECURITY = "FACTION_SECURITY";

	public const string FACTION_BLACK_MARKET = "FACTION_BLACK_MARKET";

	public const string FACTION_REVOLUTION = "FACTION_REVOLUTION";

	public const string FACTION_TOURIST = "FACTION_TOURIST";

	public const string FACTION_CHURCH = "FACTION_CHURCH";

	public const string FACTION_CARTEL = "FACTION_CARTEL";

	public StoreReputation()
	{
		perks = new List<StoreReputationPerk>();
	}

	public int GetReputation()
	{
		return (int)amount;
	}

	public double GetReputationExact()
	{
		return amount;
	}

	public void SetReputation(double value)
	{
		amount = value;
		CheckReputation();
	}

	public void ModReputation(double modValue)
	{
		if (StartingPerk.IsBlabbermouth() && modValue > 0.0 && (factionId.Equals("FACTION_BLACK_MARKET") || factionId.Equals("FACTION_REVOLUTION")))
		{
			modValue *= 0.5;
		}
		if (StartingPerk.IsSycophant() && modValue > 0.0 && factionId.Equals("FACTION_UPPER_LEVEL"))
		{
			modValue *= 1.25;
		}
		if (factionId.Equals("FACTION_BLACK_MARKET") && modValue < 0.0)
		{
			modValue *= 2.0;
		}
		if (IsPerkUnlocked("UL_PETITION") && factionId.Equals("FACTION_SECURITY") && modValue < 0.0)
		{
			modValue *= 2.0;
		}
		if (modValue > 0.0)
		{
			modValue *= (double)Difficulty.Rules.reputationGainMultiplier;
		}
		else if (modValue < 0.0)
		{
			modValue *= (double)Difficulty.Rules.reputationLossMultiplier;
		}
		amount += modValue;
		CheckReputation();
	}

	private void CheckReputation()
	{
		amount = math.clamp(amount, -200.0, 200.0);
		UpdateReputation();
	}

	public static bool IsPerkUnlocked(string perkId)
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			foreach (StoreReputationPerk perk in storeReputation.perks)
			{
				if (perk.perkId.Equals(perkId))
				{
					return perk.isUnlocked || perk.isAlwaysActive;
				}
			}
		}
		return false;
	}

	public static void UpdateReputation()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			foreach (StoreReputationPerk perk in storeReputation.perks)
			{
				if (perk.perkThreshold < 0)
				{
					if (storeReputation.amount <= (double)perk.perkThreshold)
					{
						perk.isUnlocked = true;
					}
					else
					{
						perk.isUnlocked = false;
					}
				}
				else if (storeReputation.amount >= (double)perk.perkThreshold)
				{
					perk.isUnlocked = true;
				}
				else
				{
					perk.isUnlocked = false;
				}
			}
		}
	}

	public static string GetFactionNameByID(string id)
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals(id))
			{
				return storeReputation.factionDisplay;
			}
		}
		return "NONE";
	}

	public static bool IsFactionValid(string id)
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals(id))
			{
				return true;
			}
		}
		return false;
	}

	public static string GetFactionColorByID(string id)
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals(id))
			{
				return storeReputation.factionColor3;
			}
		}
		return "8397bb";
	}

	public static StoreReputation GetStoreReputation(string id)
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals(id))
			{
				return storeReputation;
			}
		}
		return null;
	}

	public static void OnItemTraded(string factionId, int value, int originalValue, StoreClient storeClient, bool isPlayerSold, GameItem gameItem)
	{
		value = math.abs(value);
		if (Difficulty.Rules.tradeFavorRate > 0f)
		{
			PlayerStore.Instance.wildFavor += RNG.MultiplyAndRound(math.abs(originalValue), Difficulty.Rules.tradeFavorRate);
		}
		float num = (float)storeClient.GetTradeRepMultiplier(isPlayerSold);
		if (isPlayerSold && value != originalValue)
		{
			num += 1f;
		}
		value = BargainUIManager.GetPreBargainValue(gameItem, value);
		value = RNG.MultiplyAndRound(value, num);
		StoreReputation storeReputation = GetStoreReputation(factionId);
		if (storeReputation != null)
		{
			if (!isPlayerSold && StolenHelper.IsStolenItem(gameItem))
			{
				value = RNG.MultiplyAndRound(value, StolenHelper.GetStolenItemTradingRepBonus(gameItem));
			}
			if (!isPlayerSold && ContrabandHelper.IsSeriousContraband(gameItem))
			{
				value = RNG.MultiplyAndRound(value, (float)ContrabandHelper.GetContrabandLevelInInt(gameItem) / 2f + 1f);
			}
			if (factionId.Equals("FACTION_LOWER_LEVEL"))
			{
				value = RNG.MultiplyAndRound(value, 0.66);
			}
			if (factionId.Equals("FACTION_UPPER_LEVEL"))
			{
				value = RNG.MultiplyAndRound(value, 1.2000000476837158);
			}
			if (StartingPerk.IsDiplomatic() && value > 0)
			{
				value = RNG.MultiplyAndRound(value, 1.3300000429153442);
			}
			storeReputation.ModReputation((double)value / 400.0);
		}
	}

	public static void InitReputation()
	{
		PlayerStore.Instance.storeReputations = new List<StoreReputation>();
		StoreReputation storeReputation = new StoreReputation();
		storeReputation.factionDisplay = LocHelper.GetLocalizedClientName("faction_name_lower_level");
		storeReputation.factionId = "FACTION_LOWER_LEVEL";
		storeReputation.amount = 0.0;
		storeReputation.factionColor1 = "8397bb";
		storeReputation.factionColor2 = "645355";
		storeReputation.factionColor3 = "98907c";
		storeReputation.perks = CreatePerks("FACTION_LOWER_LEVEL");
		PlayerStore.Instance.storeReputations.Add(storeReputation);
		StoreReputation storeReputation2 = new StoreReputation();
		storeReputation2.factionDisplay = LocHelper.GetLocalizedClientName("faction_name_upper_level");
		storeReputation2.factionId = "FACTION_UPPER_LEVEL";
		storeReputation2.amount = 0.0;
		storeReputation2.factionColor1 = "42a2aa";
		storeReputation2.factionColor2 = "743d70";
		storeReputation2.factionColor3 = "ac7b9a";
		storeReputation2.perks = CreatePerks("FACTION_UPPER_LEVEL");
		PlayerStore.Instance.storeReputations.Add(storeReputation2);
		StoreReputation storeReputation3 = new StoreReputation();
		storeReputation3.factionDisplay = LocHelper.GetLocalizedClientName("faction_name_security");
		storeReputation3.factionId = "FACTION_SECURITY";
		storeReputation3.amount = 0.0;
		storeReputation3.factionColor1 = "c07d7d";
		storeReputation3.factionColor2 = "853737";
		storeReputation3.factionColor3 = "b16556";
		storeReputation3.perks = CreatePerks("FACTION_SECURITY");
		PlayerStore.Instance.storeReputations.Add(storeReputation3);
		StoreReputation storeReputation4 = new StoreReputation();
		storeReputation4.factionDisplay = LocHelper.GetLocalizedClientName("faction_name_black_market");
		storeReputation4.factionId = "FACTION_BLACK_MARKET";
		storeReputation4.amount = 0.0;
		storeReputation4.factionColor1 = "515b66";
		storeReputation4.factionColor2 = "44464c";
		storeReputation4.factionColor3 = "737064";
		storeReputation4.perks = CreatePerks("FACTION_BLACK_MARKET");
		storeReputation4.repLossMultiplier = 1.5f;
		PlayerStore.Instance.storeReputations.Add(storeReputation4);
		StoreReputation storeReputation5 = new StoreReputation();
		storeReputation5.factionDisplay = LocHelper.GetLocalizedClientName("faction_name_revolution");
		storeReputation5.factionId = "FACTION_REVOLUTION";
		storeReputation5.amount = 0.0;
		storeReputation5.factionColor1 = "548100";
		storeReputation5.factionColor2 = "413f2d";
		storeReputation5.factionColor3 = "848c60";
		storeReputation5.perks = CreatePerks("FACTION_REVOLUTION");
		PlayerStore.Instance.storeReputations.Add(storeReputation5);
		if (PlayerStore.Instance.startType == NewGameData.StartType.DRUG_DEALER)
		{
			StoreReputation storeReputation6 = new StoreReputation();
			storeReputation6.factionDisplay = LocHelper.GetLocalizedClientName("faction_name_cartel");
			storeReputation6.factionId = "FACTION_CARTEL";
			storeReputation6.amount = 0.0;
			storeReputation6.factionColor1 = "515b66";
			storeReputation6.factionColor2 = "44464c";
			storeReputation6.factionColor3 = "737064";
			storeReputation6.perks = CreatePerks("FACTION_CARTEL");
			PlayerStore.Instance.storeReputations.Add(storeReputation6);
		}
	}

	public static List<StoreReputationPerk> CreatePerks(string factionId)
	{
		return factionId switch
		{
			"FACTION_LOWER_LEVEL" => new List<StoreReputationPerk>
			{
				StoreReputationPerk.LLReferral(),
				StoreReputationPerk.LLTips(),
				StoreReputationPerk.LLRareGood(),
				StoreReputationPerk.LLTightLips(),
				StoreReputationPerk.LLPillar(),
				StoreReputationPerk.LLPoorReviews(),
				StoreReputationPerk.LLDistrusted(),
				StoreReputationPerk.LLLynch(),
				StoreReputationPerk.LLTarget(),
				StoreReputationPerk.LLAvoided()
			}, 
			"FACTION_UPPER_LEVEL" => new List<StoreReputationPerk>
			{
				StoreReputationPerk.ULTouristTrap(),
				StoreReputationPerk.ULPetition(),
				StoreReputationPerk.ULDiscerningTaste(),
				StoreReputationPerk.ULTips(),
				StoreReputationPerk.ULCharity(),
				StoreReputationPerk.ULGossip(),
				StoreReputationPerk.ULHouseholdName(),
				StoreReputationPerk.ULTopReview()
			}, 
			"FACTION_SECURITY" => new List<StoreReputationPerk>
			{
				StoreReputationPerk.SecRequisition(),
				StoreReputationPerk.SecAlarmIgnored(),
				StoreReputationPerk.SecImmediateArrest(),
				StoreReputationPerk.SecThoroughInspection(),
				StoreReputationPerk.SecWatchlist(),
				StoreReputationPerk.SecPriority(),
				StoreReputationPerk.SecProtection(),
				StoreReputationPerk.SecSurplus(),
				StoreReputationPerk.SecVIP(),
				StoreReputationPerk.SecTrusted(),
				StoreReputationPerk.SecReliableVendor()
			}, 
			"FACTION_BLACK_MARKET" => new List<StoreReputationPerk>
			{
				StoreReputationPerk.BMDistrusted(),
				StoreReputationPerk.BMUninsurable(),
				StoreReputationPerk.BMBroker(),
				StoreReputationPerk.BMHonor(),
				StoreReputationPerk.BMReliable(),
				StoreReputationPerk.BMNetwork()
			}, 
			"FACTION_REVOLUTION" => new List<StoreReputationPerk>
			{
				StoreReputationPerk.RevBlacklisted(),
				StoreReputationPerk.RevRaidLoot(),
				StoreReputationPerk.RevFriend(),
				StoreReputationPerk.RevHit(),
				StoreReputationPerk.RevQuartermaster(),
				StoreReputationPerk.RevRequest()
			}, 
			"FACTION_CARTEL" => new List<StoreReputationPerk>
			{
				StoreReputationPerk.CartelRetired(),
				StoreReputationPerk.CartelIncreasedStipend(),
				StoreReputationPerk.CartelReliableChemist(),
				StoreReputationPerk.CartelAuthorizedExit()
			}, 
			_ => new List<StoreReputationPerk>(), 
		};
	}

	public static void AddMissingPerksAfterLoad()
	{
		if (PlayerStore.Instance.storeReputations == null)
		{
			return;
		}
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.perks == null)
			{
				storeReputation.perks = new List<StoreReputationPerk>();
			}
			List<StoreReputationPerk> list = CreatePerks(storeReputation.factionId);
			for (int i = 0; i < list.Count; i++)
			{
				StoreReputationPerk perk = list[i];
				if (!storeReputation.perks.Exists((StoreReputationPerk saved) => saved.perkId == perk.perkId))
				{
					storeReputation.perks.Insert(Mathf.Min(i, storeReputation.perks.Count), perk);
				}
			}
		}
		UpdateReputation();
	}

	public static void ModReputation(string factionId, int value, bool notify = true)
	{
		if (value == 0)
		{
			return;
		}
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals(factionId))
			{
				if (storeReputation.factionId.Equals("FACTION_LOWER_LEVEL") && value > 0)
				{
					PlayerStore.Instance.wildFavor += value * 40;
				}
				storeReputation.ModReputation(value);
				if (notify)
				{
					ReputationUIManager.Instance.NotifyReputation(factionId, value);
				}
			}
		}
		AchievementTracker.Instance.CheckPerfectMediator();
	}

	public static void ModReputationRaw(string factionId, int value, bool notify = true)
	{
		if (value == 0)
		{
			return;
		}
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals(factionId))
			{
				storeReputation.ModReputationRaw(value);
				if (notify)
				{
					ReputationUIManager.Instance.NotifyReputation(factionId, value);
				}
			}
		}
		AchievementTracker.Instance.CheckPerfectMediator();
	}

	public void ModReputationRaw(double modValue)
	{
		amount += modValue;
		CheckReputation();
	}

	public static void ReportedClient(string factionId)
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals(factionId))
			{
				storeReputation.ModReputation(storeReputation.currentReportPenalty);
				ReputationUIManager.Instance.NotifyReputation(factionId, storeReputation.currentReportPenalty);
				storeReputation.currentReportPenalty -= 2;
				if (storeReputation.currentReportPenalty <= -20)
				{
					storeReputation.currentReportPenalty = -20;
				}
			}
		}
	}

	public static int GetSecReputation()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals("FACTION_SECURITY"))
			{
				return (int)storeReputation.amount;
			}
		}
		return 0;
	}

	public static int GetRevReputation()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals("FACTION_REVOLUTION"))
			{
				return (int)storeReputation.amount;
			}
		}
		return 0;
	}

	public static int GetBMReputation()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals("FACTION_BLACK_MARKET"))
			{
				return (int)storeReputation.amount;
			}
		}
		return 0;
	}

	public static int GetCartelReputation()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals("FACTION_CARTEL"))
			{
				return (int)storeReputation.amount;
			}
		}
		return 0;
	}

	public static void ModSecReputation(int amount)
	{
		ModReputation("FACTION_SECURITY", amount);
	}

	public static void ModRevReputation(int amount)
	{
		ModReputation("FACTION_REVOLUTION", amount);
	}

	public static void ModNCReputation(int amount)
	{
		ModReputation("FACTION_CHURCH", amount);
	}

	public static void ModCartelReputation(int amount)
	{
		ModReputation("FACTION_CARTEL", amount);
	}

	public static void RemoveCartelFaction()
	{
		PlayerStore.Instance.storeReputations.RemoveAll((StoreReputation rep) => rep.factionId.Equals("FACTION_CARTEL"));
	}

	public void ModProgressReputation(int progressAmount)
	{
		ModReputation((double)progressAmount / 400.0);
	}

	public static StoreReputation GetSecFaction()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals("FACTION_SECURITY"))
			{
				return storeReputation;
			}
		}
		return null;
	}

	public static StoreReputation GetRevFaction()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals("FACTION_REVOLUTION"))
			{
				return storeReputation;
			}
		}
		return null;
	}

	public static StoreReputation GetBMFaction()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals("FACTION_BLACK_MARKET"))
			{
				return storeReputation;
			}
		}
		return null;
	}

	public static StoreReputation GetULFaction()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals("FACTION_UPPER_LEVEL"))
			{
				return storeReputation;
			}
		}
		return null;
	}

	public static int GetLLReputation()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals("FACTION_LOWER_LEVEL"))
			{
				return (int)storeReputation.amount;
			}
		}
		return 0;
	}

	public static int GetULReputation()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			if (storeReputation.factionId.Equals("FACTION_UPPER_LEVEL"))
			{
				return (int)storeReputation.amount;
			}
		}
		return 0;
	}

	public static void ModLLReputation(int amount)
	{
		ModReputation("FACTION_LOWER_LEVEL", amount);
	}

	public static void ModBMReputation(int amount)
	{
		ModReputation("FACTION_BLACK_MARKET", amount);
	}

	public static void ModULReputation(int amount)
	{
		ModReputation("FACTION_UPPER_LEVEL", amount);
	}

	public static void ModCurrentClientReputation(int amount, bool notify = true)
	{
		if (PlayerStore.Instance.currentClientInstance != null)
		{
			ModReputation(PlayerStore.Instance.currentClientInstance.GetClientBlueprint().clientFaction, amount, notify);
		}
	}

	public static string GetReputationStatus(int amount)
	{
		if (amount > 0)
		{
			if (amount < 10)
			{
				return LocHelper.GetLocalizedMechanic("rep_neutral");
			}
			if (amount < 20)
			{
				return LocHelper.GetLocalizedMechanic("rep_known");
			}
			if (amount < 30)
			{
				return LocHelper.GetLocalizedMechanic("rep_established");
			}
			if (amount < 40)
			{
				return LocHelper.GetLocalizedMechanic("rep_recognized");
			}
			if (amount < 50)
			{
				return LocHelper.GetLocalizedMechanic("rep_dependable");
			}
			if (amount < 60)
			{
				return LocHelper.GetLocalizedMechanic("rep_recommended");
			}
			if (amount < 70)
			{
				return LocHelper.GetLocalizedMechanic("rep_trusted");
			}
			if (amount < 80)
			{
				return LocHelper.GetLocalizedMechanic("rep_highly_rated");
			}
			if (amount < 90)
			{
				return LocHelper.GetLocalizedMechanic("rep_renowned");
			}
			if (amount < 100)
			{
				return LocHelper.GetLocalizedMechanic("rep_esteemed");
			}
			if (amount <= 200)
			{
				return LocHelper.GetLocalizedMechanic("rep_prestigious");
			}
			return "";
		}
		if (amount > -10)
		{
			return LocHelper.GetLocalizedMechanic("rep_neutral");
		}
		if (amount > -20)
		{
			return LocHelper.GetLocalizedMechanic("rep_inconsistent");
		}
		if (amount > -30)
		{
			return LocHelper.GetLocalizedMechanic("rep_disfavored");
		}
		if (amount > -40)
		{
			return LocHelper.GetLocalizedMechanic("rep_unreliable");
		}
		if (amount > -50)
		{
			return LocHelper.GetLocalizedMechanic("rep_shady");
		}
		if (amount > -60)
		{
			return LocHelper.GetLocalizedMechanic("rep_dubious");
		}
		if (amount > -70)
		{
			return LocHelper.GetLocalizedMechanic("rep_questionable");
		}
		if (amount > -80)
		{
			return LocHelper.GetLocalizedMechanic("rep_notorious");
		}
		if (amount > -90)
		{
			return LocHelper.GetLocalizedMechanic("rep_disreputable");
		}
		if (amount > -100)
		{
			return LocHelper.GetLocalizedMechanic("rep_avoided");
		}
		if (amount > -150)
		{
			return LocHelper.GetLocalizedMechanic("rep_boycotted");
		}
		if (amount >= -200)
		{
			return LocHelper.GetLocalizedMechanic("rep_blacklisted");
		}
		return "";
	}

	public static void CheckSoldNaloxgone(GameItem gameItem)
	{
		if (gameItem.identifier.Equals("pure_white_injector") && gameItem.IsConditionActive("freeNormal"))
		{
			ModLLReputation(-4);
		}
		else if (gameItem.identifier.Equals("pure_white_injector") && gameItem.IsConditionActive("freeFree"))
		{
			ModLLReputation(1);
		}
	}

	public static StoreReputationPerk GetPerkByID(string perkId)
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			foreach (StoreReputationPerk perk in storeReputation.perks)
			{
				if (perk.perkId.Equals(perkId))
				{
					return perk;
				}
			}
		}
		return null;
	}
}

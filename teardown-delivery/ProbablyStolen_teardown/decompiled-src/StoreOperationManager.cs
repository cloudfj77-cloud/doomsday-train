using System.Collections.Generic;

public class StoreOperationManager
{
	[ES3Serializable]
	public int lowerLevelUnrest = 25;

	[ES3Serializable]
	public int upperLevelFriendliness = 50;

	[ES3Serializable]
	public int revPower;

	[ES3Serializable]
	public int secPower;

	[ES3Serializable]
	public int blackMarketPower;

	[ES3Serializable]
	public List<string> availableOperations = new List<string>();

	[ES3NonSerializable]
	public static List<StoreEventBlueprint> operationBlueprints = new List<StoreEventBlueprint>
	{
		new StoreEventBlueprint(OperationListRev.MobilizationSecret, 10, "rev_mobilization_secret"),
		new StoreEventBlueprint(OperationListRev.Mobilization, 10, "rev_mobilization"),
		new StoreEventBlueprint(OperationListRev.RaidUpperLevel, 10, "rev_raid_upper_level"),
		new StoreEventBlueprint(OperationListRev.RaidSecurity, 10, "raid_security"),
		new StoreEventBlueprint(OperationListSec.IncreasePatrols, 10, "sec_increase_patrols"),
		new StoreEventBlueprint(OperationListSec.Crackdown, 10, "crackdown"),
		new StoreEventBlueprint(OperationListSec.RaidBlackMarket, 10, "sec_raid_black_market"),
		new StoreEventBlueprint(OperationListSec.RaidRevolution, 10, "sec_raid_revolution"),
		new StoreEventBlueprint(OperationListSec.BudgetIncrease, 10, "sec_budget_increase"),
		new StoreEventBlueprint(OperationListSec.MartialLaw, 10, "sec_martial_law"),
		new StoreEventBlueprint(OperationListLL.Riot, 10, "ll_riot"),
		new StoreEventBlueprint(OperationListLL.WorkerStrike, 10, "ll_worker_strike"),
		new StoreEventBlueprint(OperationListUL.MedicalAid, 10, "ul_medical_aid"),
		new StoreEventBlueprint(OperationListUL.HouseholdRelief, 10, "ul_household_relief"),
		new StoreEventBlueprint(OperationListBM.GangWar, 10, "bm_gang_war"),
		new StoreEventBlueprint(OperationListBM.IllegalWeaponShipment, 10, "bm_illegal_weapon_shipment")
	};

	public void OnDayStart()
	{
		Debug.Log("Handling Operation");
		HandleLowerLevel();
		HandleRevolution();
		HandleSecurity();
		HandleUpperLevel();
		HandleBlackMarket();
		int num = 0;
		foreach (StoreEvent futurEvent in StoreStation.Instance.storeEventManager.futurEvents)
		{
			if (futurEvent.isFactionOperation && !futurEvent.IsKnown && CanPlayerKnowFactionOperation(futurEvent.sourceFaction))
			{
				futurEvent.IsKnown = true;
				num++;
			}
		}
		if (num > 0)
		{
			NightLog.FactionOperationDateShared();
		}
		foreach (StoreEvent futurEvent2 in StoreStation.Instance.storeEventManager.futurEvents)
		{
			if (futurEvent2.isFactionOperation)
			{
				Debug.Log("Currently Queued Operation: " + futurEvent2.identifier);
			}
		}
	}

	public void HandleBlackMarket()
	{
		if (StoreStation.Instance.dayCounter <= 9)
		{
			return;
		}
		if (CanFactionQueueOperation("FACTION_BLACK_MARKET"))
		{
			if (blackMarketPower >= 85)
			{
				AddPossibleOperation("bm_gang_war");
			}
			AddPossibleOperation("bm_illegal_weapon_shipment");
			QueueOperation();
		}
		if (blackMarketPower < 0)
		{
			blackMarketPower = 0;
		}
		if (blackMarketPower > 100)
		{
			blackMarketPower = 100;
		}
	}

	public void HandleUpperLevel()
	{
		if (upperLevelFriendliness <= 48)
		{
			upperLevelFriendliness += 2;
		}
		else
		{
			upperLevelFriendliness++;
		}
		if (CanFactionQueueOperation("FACTION_UPPER_LEVEL"))
		{
			if (upperLevelFriendliness >= 50 && lowerLevelUnrest >= 50)
			{
				AddPossibleOperation("ul_medical_aid");
			}
			if (upperLevelFriendliness >= 50 && lowerLevelUnrest <= 35)
			{
				AddPossibleOperation("ul_household_relief");
			}
		}
		if (upperLevelFriendliness < 0)
		{
			upperLevelFriendliness = 0;
		}
		if (upperLevelFriendliness > 100)
		{
			upperLevelFriendliness = 100;
		}
	}

	public void HandleLowerLevel()
	{
		if (lowerLevelUnrest >= 27)
		{
			lowerLevelUnrest -= 2;
		}
		else if (lowerLevelUnrest <= 23)
		{
			lowerLevelUnrest -= 2;
		}
		if (CanFactionQueueOperation("FACTION_LOWER_LEVEL"))
		{
			if (lowerLevelUnrest >= 85)
			{
				AddPossibleOperation("ll_riot");
			}
			if (lowerLevelUnrest >= 50)
			{
				AddPossibleOperation("ll_worker_strike");
			}
			QueueOperation();
		}
		if (lowerLevelUnrest < 0)
		{
			lowerLevelUnrest = 0;
		}
		if (lowerLevelUnrest > 100)
		{
			lowerLevelUnrest = 100;
		}
	}

	public void HandleRevolution()
	{
		if (StoreStation.Instance.dayCounter <= 7)
		{
			return;
		}
		availableOperations.Clear();
		if (CanFactionQueueOperation("FACTION_REVOLUTION"))
		{
			if (revPower <= 25)
			{
				AddPossibleOperation("rev_mobilization_secret");
			}
			if (revPower >= 50)
			{
				AddPossibleOperation("rev_mobilization");
			}
			if (revPower >= 15)
			{
				AddPossibleOperation("rev_raid_upper_level");
			}
			if (revPower >= 25)
			{
				AddPossibleOperation("raid_security");
			}
			QueueOperation();
		}
		if (revPower < 0)
		{
			revPower = 0;
		}
		if (revPower > 100)
		{
			revPower = 100;
		}
	}

	public void HandleSecurity()
	{
		if (StoreStation.Instance.dayCounter <= 5)
		{
			return;
		}
		availableOperations.Clear();
		secPower += 4;
		if (secPower > 100)
		{
			secPower = 100;
		}
		if (CanFactionQueueOperation("FACTION_SECURITY"))
		{
			if (secPower >= 20 && blackMarketPower >= 40)
			{
				AddPossibleOperation("crackdown");
			}
			if (secPower >= 20 && (revPower >= 33 || lowerLevelUnrest >= 33 || blackMarketPower >= 33))
			{
				AddPossibleOperation("sec_increase_patrols");
			}
			if (secPower >= 15 && revPower >= 40)
			{
				AddPossibleOperation("sec_raid_revolution");
			}
			if (secPower >= 15 && blackMarketPower >= 40)
			{
				AddPossibleOperation("sec_raid_black_market");
			}
			if (secPower >= 50 && (revPower >= 65 || lowerLevelUnrest >= 65 || blackMarketPower >= 65))
			{
				AddPossibleOperation("sec_martial_law");
			}
			if (secPower <= 5)
			{
				AddPossibleOperation("sec_budget_increase");
			}
			QueueOperation();
		}
		if (secPower < 0)
		{
			secPower = 0;
		}
		if (secPower > 100)
		{
			secPower = 100;
		}
	}

	public void QueueOperation()
	{
		if (availableOperations.Count == 0)
		{
			Debug.Log("No Available Operation to Queue");
			return;
		}
		string randomElement = RNG.GetRandomElement(availableOperations);
		bool flag = false;
		foreach (StoreEventBlueprint operationBlueprint in operationBlueprints)
		{
			if (operationBlueprint.identifier.Equals(randomElement))
			{
				Debug.Log("Queueing New Operation: " + randomElement);
				flag = true;
				StoreStation.Instance.storeEventManager.QueueFuturEvent(operationBlueprint.storeEventFunc(), 4, IsKnown: false);
				break;
			}
		}
		if (!flag)
		{
			Debug.Log("Error: Operation Blueprint not found for " + randomElement + " Did you forget to add it to the list?");
		}
		availableOperations.Clear();
	}

	public bool CanPlayerKnowFactionOperation(string faction)
	{
		if (faction.Equals("FACTION_LOWER_LEVEL") && StoreReputation.IsPerkUnlocked("LLTIPS"))
		{
			return true;
		}
		if (faction.Equals("FACTION_SECURITY") && StoreReputation.IsPerkUnlocked("SEC_TRUSTED"))
		{
			return true;
		}
		if (faction.Equals("FACTION_UPPER_LEVEL") && StoreReputation.IsPerkUnlocked("UL_GOSSIP"))
		{
			return true;
		}
		if (faction.Equals("FACTION_REVOLUTION") && StoreReputation.IsPerkUnlocked("REV_FRIEND"))
		{
			return true;
		}
		if (faction.Equals("FACTION_BLACK_MARKET") && StoreReputation.IsPerkUnlocked("BM_NETWORK"))
		{
			return true;
		}
		return false;
	}

	public bool CanFactionQueueOperation(string faction)
	{
		foreach (StoreEvent activeEvent in StoreStation.Instance.storeEventManager.GetActiveEvents())
		{
			if (activeEvent.isFactionOperation && activeEvent.sourceFaction.Equals(faction))
			{
				return false;
			}
		}
		foreach (StoreEvent futurEvent in StoreStation.Instance.storeEventManager.futurEvents)
		{
			if (futurEvent.isFactionOperation && futurEvent.sourceFaction.Equals(faction))
			{
				return false;
			}
		}
		return true;
	}

	public bool AddPossibleOperation(string operationID)
	{
		if (!availableOperations.Contains(operationID) && CanQueueThisOperation(operationID))
		{
			availableOperations.Add(operationID);
			return true;
		}
		return false;
	}

	public bool CanQueueThisOperation(string operationID)
	{
		if (IsOperationActive(operationID))
		{
			return false;
		}
		if (IsOperationOnCooldown(operationID))
		{
			return false;
		}
		if (IsOperationInQueue(operationID))
		{
			return false;
		}
		return true;
	}

	public bool IsOperationActive(string operationID)
	{
		foreach (StoreEvent activeEvent in StoreStation.Instance.storeEventManager.GetActiveEvents())
		{
			if (activeEvent.identifier.Equals(operationID))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsOperationInQueue(string operationID)
	{
		foreach (StoreEvent futurEvent in StoreStation.Instance.storeEventManager.futurEvents)
		{
			if (futurEvent.identifier.Equals(operationID))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsOperationOnCooldown(string operationID)
	{
		if (StoreStation.Instance.storeEventManager.eventTracker.TryGetValue(operationID, out var value) && value > 0)
		{
			return true;
		}
		return false;
	}
}

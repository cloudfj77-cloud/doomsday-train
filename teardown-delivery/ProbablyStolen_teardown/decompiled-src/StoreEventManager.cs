using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoreEventManager
{
	public enum EventDayType
	{
		FirstDay,
		MiddleDay,
		LastDay,
		NotPartOfEvent,
		StartAndFinish
	}

	[ES3Serializable]
	private List<StoreEvent> activeEvents;

	[ES3Serializable]
	public Dictionary<string, int> eventTracker;

	[ES3Serializable]
	public List<StoreEvent> futurEvents;

	[ES3Serializable]
	public List<int> futurEventsDay;

	public const int DAY_UNTIL_NORMAL_EVENT_SPAWN = 5;

	[ES3NonSerializable]
	public static List<StoreEventBlueprint> normalEventBlueprints = new List<StoreEventBlueprint>
	{
		new StoreEventBlueprint(EventListNormal.CruiseShipDocked, 10, "cruiseShipDocked"),
		new StoreEventBlueprint(EventListNormal.CreateConstructionProject, 15, "construction_project"),
		new StoreEventBlueprint(EventListNormal.CreateMiningExpeditionReturn, 15, "miningExpeditionReturn"),
		new StoreEventBlueprint(EventListNormal.CreateStoreRoomExcavated, 15, "storeroom_excavated"),
		new StoreEventBlueprint(EventListNormal.CreateSeedHeist, 15, "seedShortage"),
		new StoreEventBlueprint(EventListNormal.CreateBadNutrifruitHarvest, 15, "badNutrifruitHarvest"),
		new StoreEventBlueprint(EventListNormal.CreateWaterTreatmentPlantBreakdown, 15, "water_treatment_breakdown"),
		new StoreEventBlueprint(EventListNormal.CreateScheduledTreatmentPlantMaintenance, 15, "scheduled_treatment_maintenance"),
		new StoreEventBlueprint(EventListNormal.CreateMinerReturnFromIceAsteroid, 15, "miner_return_ice_asteroid"),
		new StoreEventBlueprint(EventListNormal.CreateLowerLevelMobJustice, 8, "lower_level_mob_justice"),
		new StoreEventBlueprint(EventListNormal.CreateOxymoreWave, 6, "oxymoreWave"),
		new StoreEventBlueprint(EventListNormal.CreateCounterfeitMedicalSuppliesDiscovered, 15, "medicalSuppliesShortage"),
		new StoreEventBlueprint(EventListNormal.CreatePowerShortage, 7, "powerShortage"),
		new StoreEventBlueprint(EventListNormal.SchedulePowerPlantMaintenance, 10, "powerPlantMaintenance"),
		new StoreEventBlueprint(EventListNormal.StationWideBlackout, 4, "stationWideBlackout")
	};

	[ES3NonSerializable]
	public static List<StoreEventBlueprint> threatEventBlueprints = new List<StoreEventBlueprint>();

	[ES3NonSerializable]
	public static List<StoreEventBlueprint> cosmeticEventBlueprints = new List<StoreEventBlueprint>
	{
		new StoreEventBlueprint(EventListCosmetic.ScheduledEletricalMaintenance, 10, "ScheduledEletricalMaintenance"),
		new StoreEventBlueprint(EventListCosmetic.NutrifruitGrow, 10, "NutrifruitGrow"),
		new StoreEventBlueprint(EventListCosmetic.PeatContent, 10, "PeatContent"),
		new StoreEventBlueprint(EventListCosmetic.RecyclingAwarenessCampaign, 10, "RecyclingAwarenessCampaign"),
		new StoreEventBlueprint(EventListCosmetic.ArtistShowcase, 10, "ArtistShowcase"),
		new StoreEventBlueprint(EventListCosmetic.MaintenanceCrewStrike, 10, "MaintenanceCrewStrike"),
		new StoreEventBlueprint(EventListCosmetic.TechDemoDay, 10, "TechDemoDay"),
		new StoreEventBlueprint(EventListCosmetic.NewTransitLine, 10, "NewTransitLine"),
		new StoreEventBlueprint(EventListCosmetic.WaterFountainUnveiled, 10, "WaterFountainUnveiled"),
		new StoreEventBlueprint(EventListCosmetic.CommunityGardenExpansion, 10, "CommunityGardenExpansion"),
		new StoreEventBlueprint(EventListCosmetic.ArtContestHeld, 10, "ArtContestHeld"),
		new StoreEventBlueprint(EventListCosmetic.CulturalCelebration, 10, "CulturalCelebration"),
		new StoreEventBlueprint(EventListCosmetic.RoutineSpacewalkTraining, 10, "RoutineSpacewalkTraining")
	};

	public List<StoreEvent> GetActiveEvents()
	{
		SortEventByPriority();
		return new List<StoreEvent>(activeEvents);
	}

	public void SortEventByPriority()
	{
		activeEvents = activeEvents.OrderByDescending((StoreEvent item) => item.importance).ToList();
	}

	public StoreEventManager()
	{
		activeEvents = new List<StoreEvent>();
		futurEvents = new List<StoreEvent>();
		futurEventsDay = new List<int>();
		eventTracker = new Dictionary<string, int>();
	}

	public void OnDayStart()
	{
		HandleFuturEvent();
		UpdateTracker();
		foreach (StoreEvent activeEvent in activeEvents)
		{
			activeEvent.OnDayStart();
		}
		QueueEvent();
		CheckBlackOutEvent();
	}

	public void CheckBlackOutEvent(bool noLog = false)
	{
		if (StartingPerk.IsMakeshiftReactor())
		{
			return;
		}
		if (IsBlackoutEventActive() && !IsPlayerAffectedByBlackout())
		{
			if (!noLog)
			{
				NightLog.BlackoutBypassed();
			}
		}
		else if (IsBlackoutEventActive())
		{
			StoreUIManager.Instance.darknessPanel.SetActive(value: true);
			StoreUIManager.Instance.OnPowerOff();
			if (!noLog)
			{
				NightLog.BlackoutActive();
			}
		}
	}

	public bool IsBlackoutEventActive()
	{
		foreach (StoreEvent activeEvent in activeEvents)
		{
			if (activeEvent.identifier.Equals("stationWideBlackout"))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsPlayerAffectedByBlackout()
	{
		if (!NetworkUpgrade.IsPowerHijackUnlocked())
		{
			return !StartingPerk.IsMakeshiftReactor();
		}
		return false;
	}

	public void DeactivateEvent(StoreEvent storeEvent)
	{
		if (activeEvents.Contains(storeEvent))
		{
			storeEvent.OnEventRemove();
			activeEvents.Remove(storeEvent);
		}
	}

	public void OnDayEnd()
	{
		for (int num = activeEvents.Count - 1; num >= 0; num--)
		{
			activeEvents[num].OnDayEnd();
			if (activeEvents[num].duration <= 0)
			{
				DeactivateEvent(activeEvents[num]);
			}
		}
	}

	public void LogEvent()
	{
		foreach (StoreEvent activeEvent in activeEvents)
		{
			activeEvent.PrintEvent();
		}
	}

	public void ActivateEvent(StoreEvent storeEvent, bool debugLog = false, bool force = false)
	{
		if (storeEvent == null)
		{
			Debug.Log("<color=red>A null event was attempted to be activated.</color>");
			return;
		}
		storeEvent.IsKnown = true;
		if (!force)
		{
			if (IsEventActive(storeEvent))
			{
				ExtendActiveEvent(GetActiveEvent(storeEvent.identifier), storeEvent);
				return;
			}
			if (!IsEventCompatible(storeEvent) || (StoreStation.Instance.IsStationOnLockdown() && storeEvent.isBlockByLockDown))
			{
				return;
			}
			if (!IsEventCompatiblePreCheck(storeEvent))
			{
				List<StoreEvent> imcompatibleEventPreCheck = GetImcompatibleEventPreCheck(storeEvent);
				if (imcompatibleEventPreCheck.Count == 1 && (!imcompatibleEventPreCheck[0].IsPriority || !storeEvent.IsPriority))
				{
					if (imcompatibleEventPreCheck[0].IsPriority && !storeEvent.IsPriority)
					{
						return;
					}
					if (!imcompatibleEventPreCheck[0].IsPriority && storeEvent.IsPriority)
					{
						Debug.Log("Deactivating Incompatible Event: " + imcompatibleEventPreCheck[0].newsName);
						DeactivateEvent(imcompatibleEventPreCheck[0]);
					}
					else if (!imcompatibleEventPreCheck[0].IsPriority && !storeEvent.IsPriority)
					{
						Debug.Log("Deactivating Incompatible Event: " + imcompatibleEventPreCheck[0].newsName);
						DeactivateEvent(imcompatibleEventPreCheck[0]);
					}
				}
			}
		}
		Debug.Log("Activating Event: " + storeEvent.newsName);
		storeEvent.OnEventActive();
		activeEvents.Add(storeEvent);
		ApplyCooldown(storeEvent, storeEvent.duration + storeEvent.cooldown);
	}

	private void ExtendActiveEvent(StoreEvent running, StoreEvent incoming)
	{
		running.IsKnown = true;
		int num = incoming.duration - running.duration;
		if (num > 0 && !running.isPermanent)
		{
			running.duration += num;
			running.totalDuration += num;
			Debug.Log("Extending Event: " + running.newsName + " by " + num + " day(s)");
			foreach (StoreRequest storeRequest in PlayerStore.Instance.storeRequests)
			{
				if (storeRequest.sourceEventId == running.identifier)
				{
					storeRequest.duration = Mathf.Max(storeRequest.duration, running.duration + 1);
				}
			}
		}
		ApplyCooldown(running, running.duration + running.cooldown);
	}

	private void ApplyCooldown(StoreEvent storeEvent, int days)
	{
		if (storeEvent.eventType != StoreEvent.EventType.COSMETIC)
		{
			eventTracker.TryGetValue(storeEvent.identifier, out var value);
			if (days > value)
			{
				eventTracker[storeEvent.identifier] = days;
			}
		}
	}

	public void ApplyCooldownAfterEnd(StoreEvent storeEvent)
	{
		if (storeEvent.eventType == StoreEvent.EventType.COSMETIC)
		{
			return;
		}
		int num = storeEvent.cooldown;
		for (int i = 0; i < futurEvents.Count; i++)
		{
			if (futurEvents[i].identifier.Equals(storeEvent.identifier))
			{
				num = Mathf.Max(num, futurEventsDay[i] + futurEvents[i].duration + futurEvents[i].cooldown);
			}
		}
		eventTracker[storeEvent.identifier] = num;
	}

	public int GetEventCountByType(StoreEvent.EventType eventType)
	{
		int num = 0;
		foreach (StoreEvent activeEvent in activeEvents)
		{
			if (activeEvent.eventType == eventType)
			{
				num++;
			}
		}
		return num;
	}

	public void HandleFuturEvent()
	{
		for (int i = 0; i < futurEvents.Count; i++)
		{
			futurEventsDay[i]--;
		}
		for (int num = futurEventsDay.Count - 1; num >= 0; num--)
		{
			if (futurEventsDay[num] <= 0)
			{
				Debug.Log("Activating Queue Event " + futurEvents[num].newsName);
				ActivateEvent(futurEvents[num]);
				futurEventsDay.RemoveAt(num);
				futurEvents.RemoveAt(num);
			}
		}
	}

	public void QueueFuturEvent(StoreEvent storeEvent, int day, bool IsKnown)
	{
		storeEvent.totalDuration = storeEvent.duration;
		storeEvent.startDay = StoreStation.Instance.dayCounter + day;
		storeEvent.IsKnown = IsKnown;
		futurEvents.Add(storeEvent);
		futurEventsDay.Add(day);
		ApplyCooldown(storeEvent, day + storeEvent.duration + storeEvent.cooldown);
	}

	public void RemoveAllEvent(string id)
	{
		for (int num = futurEvents.Count - 1; num >= 0; num--)
		{
			if (futurEvents[num].identifier.Equals(id))
			{
				futurEvents.RemoveAt(num);
				futurEventsDay.RemoveAt(num);
			}
		}
		for (int num2 = activeEvents.Count - 1; num2 >= 0; num2--)
		{
			if (activeEvents[num2].identifier.Equals(id))
			{
				DeactivateEvent(activeEvents[num2]);
			}
		}
	}

	public void QueueEvent()
	{
		HandleDebugEvent();
		HandleLotteryEvent();
		HandleNewEventNormale();
		HandleNewEventComestic();
		if (StoreStation.Instance.dayCounter == 7 && !PlayerStore.Instance.skipIntro)
		{
			ActivateEvent(EventListUnique.CreateEvidenceLockerBreakIn());
		}
	}

	public void HandleLotteryEvent()
	{
		if (LotteryHelper.IsLotteryUnlocked() && StoreStation.Instance.dayCounter % 7 == 6)
		{
			ActivateEvent(EventListSpecial.LotteryDraw());
		}
	}

	public void HandleDebugEvent()
	{
		_ = PlayerStore.Instance.devMode;
	}

	public void HandleNewEventNormale()
	{
		if ((StoreStation.Instance.dayCounter <= 5 && !PlayerStore.Instance.skipIntro) || GetEventCounter() >= 3)
		{
			return;
		}
		int chance = 25;
		if (GetEventCounter() == 0)
		{
			chance = 50;
		}
		if (GetEventCounter() == 1)
		{
			chance = 35;
		}
		int randomInt = RNG.GetRandomInt(1, 1);
		if (!RNG.Roll(chance))
		{
			return;
		}
		for (int i = 0; i < randomInt; i++)
		{
			StoreEvent storeEvent = PickRandomEventByType(StoreEvent.EventType.NORMALE);
			if (storeEvent != null && IsEventCompatiblePreCheck(storeEvent))
			{
				ActivateEvent(storeEvent);
			}
		}
	}

	public void HandleNewEventComestic()
	{
		int num = 0;
		while (activeEvents.Count < 2 && num < 50)
		{
			num++;
			StoreEvent storeEvent = PickRandomEventByType(StoreEvent.EventType.COSMETIC);
			if (storeEvent != null)
			{
				ActivateEvent(storeEvent);
			}
		}
	}

	private static bool IsEventRetired(string identifier)
	{
		if (identifier == "seedShortage")
		{
			return PlantList.IsNutrifruitFreed();
		}
		return false;
	}

	public StoreEvent PickRandomEventByType(StoreEvent.EventType eventType)
	{
		List<StoreEventBlueprint> list = (eventType switch
		{
			StoreEvent.EventType.NORMALE => normalEventBlueprints, 
			StoreEvent.EventType.THREAT => threatEventBlueprints, 
			_ => cosmeticEventBlueprints, 
		}).Where((StoreEventBlueprint e) => !IsEventOnCooldown(e.identifier) && !IsEventRetired(e.identifier)).ToList();
		if (list.Count == 0)
		{
			Debug.LogWarning($"No available events for type {eventType} (all on cooldown).");
			return null;
		}
		int odd = RNG.GetOdd(list.Select((StoreEventBlueprint e) => e.odd).ToList());
		if (list[odd] == null || list[odd].storeEventFunc == null)
		{
			Debug.Log("Warning: Probable Error in PickRandomEventByType");
			return null;
		}
		return list[odd].storeEventFunc();
	}

	public bool IsEventActive(string identifier)
	{
		foreach (StoreEvent activeEvent in activeEvents)
		{
			if (activeEvent.identifier.Equals(identifier))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsEventActive(StoreEvent storeEvent)
	{
		return IsEventActive(storeEvent.identifier);
	}

	public bool IsEventActiveInDay(string identifier, int inDay)
	{
		for (int i = 0; i < futurEvents.Count; i++)
		{
			if (futurEvents[i].identifier.Equals(identifier) && futurEventsDay[i] == inDay)
			{
				return true;
			}
		}
		return false;
	}

	public StoreEvent GetActiveEvent(string id)
	{
		foreach (StoreEvent activeEvent in activeEvents)
		{
			if (activeEvent.identifier.Equals(id))
			{
				return activeEvent;
			}
		}
		return null;
	}

	public bool IsEventCompatible(StoreEvent storeEvent)
	{
		return IsEventCompatible(storeEvent.identifier);
	}

	public bool IsEventCompatible(string identifier)
	{
		foreach (StoreEvent activeEvent in activeEvents)
		{
			if (activeEvent.imcompatibleEvents.Contains(identifier))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsEventCompatiblePreCheck(StoreEvent storeEvent)
	{
		return true;
	}

	public List<StoreEvent> GetImcompatibleEventPreCheck(StoreEvent storeEvent)
	{
		List<StoreEvent> list = new List<StoreEvent>();
		foreach (StoreEvent activeEvent in activeEvents)
		{
			for (int i = 0; i < activeEvent.negociationDatas.Count; i++)
			{
				string itemType = activeEvent.negociationDatas[i].itemType;
				int value = activeEvent.negociationDatas[i].value;
				for (int j = 0; j < storeEvent.negociationDatas.Count; j++)
				{
					string itemType2 = storeEvent.negociationDatas[j].itemType;
					int value2 = storeEvent.negociationDatas[j].value;
					if (itemType == itemType2 && ((value > 0 && value2 < 0) || (value < 0 && value2 > 0)) && !list.Contains(activeEvent))
					{
						list.Add(activeEvent);
					}
				}
			}
		}
		return list;
	}

	public void UpdateTracker()
	{
		foreach (string item in new List<string>(eventTracker.Keys))
		{
			if (eventTracker[item] > 0)
			{
				eventTracker[item] -= 1;
			}
			if (eventTracker[item] == 0)
			{
				eventTracker.Remove(item);
			}
		}
	}

	public bool IsEventOnCooldown(string identifier)
	{
		if (eventTracker.ContainsKey(identifier))
		{
			return eventTracker[identifier] > 0;
		}
		return false;
	}

	public int GetEventCounter()
	{
		int num = 0;
		foreach (StoreEvent activeEvent in activeEvents)
		{
			if (activeEvent.eventType != StoreEvent.EventType.COSMETIC)
			{
				num++;
			}
		}
		return num;
	}

	public void PickRumorEvent()
	{
		StoreEvent storeEvent = PickRandomEventByType(StoreEvent.EventType.NORMALE);
		if (storeEvent != null)
		{
			int randomInt = RNG.GetRandomInt(2, 3);
			RNG.GetRandomInt(2, 3);
			StoreStation.Instance.storeEventManager.QueueFuturEvent(storeEvent.CreateAndAddSlip(randomInt), randomInt, IsKnown: true);
		}
	}

	public List<StoreEvent> GetActiveEventOnDay(int day, bool excludeCosmetic = true, bool excludeUnknown = true)
	{
		List<StoreEvent> list = new List<StoreEvent>();
		int dayCounter = StoreStation.Instance.dayCounter;
		foreach (StoreEvent activeEvent in activeEvents)
		{
			if ((!excludeCosmetic || activeEvent.eventType != StoreEvent.EventType.COSMETIC) && (!excludeUnknown || activeEvent.IsKnown) && (!StartingPerk.IsUnsubscribed() || activeEvent.eventType == StoreEvent.EventType.INNATE))
			{
				int startDay = activeEvent.startDay;
				int num = startDay + activeEvent.totalDuration - 1;
				if (day >= startDay && day <= num)
				{
					list.Add(activeEvent);
				}
			}
		}
		for (int i = 0; i < futurEvents.Count; i++)
		{
			if ((!excludeCosmetic || futurEvents[i].eventType != StoreEvent.EventType.COSMETIC) && (!excludeUnknown || futurEvents[i].IsKnown) && (!StartingPerk.IsUnsubscribed() || futurEvents[i].eventType == StoreEvent.EventType.INNATE))
			{
				StoreEvent storeEvent = futurEvents[i];
				int num2 = dayCounter + futurEventsDay[i];
				int num3 = num2 + storeEvent.totalDuration - 1;
				if (day >= num2 && day <= num3)
				{
					list.Add(storeEvent);
				}
			}
		}
		return list;
	}

	public static EventDayType GetEventDayType(StoreEvent storeEvent, int day)
	{
		int num = storeEvent.startDay + storeEvent.totalDuration - 1;
		if (day < storeEvent.startDay || day > num)
		{
			return EventDayType.NotPartOfEvent;
		}
		if (storeEvent.totalDuration == 1 && day == storeEvent.startDay)
		{
			return EventDayType.StartAndFinish;
		}
		if (day == storeEvent.startDay)
		{
			return EventDayType.FirstDay;
		}
		if (day == num)
		{
			return EventDayType.LastDay;
		}
		return EventDayType.MiddleDay;
	}

	public static int GetShortageType(string type)
	{
		int num = 0;
		foreach (StoreEvent activeEvent in StoreStation.Instance.storeEventManager.activeEvents)
		{
			foreach (NegociationData negociationData in activeEvent.negociationDatas)
			{
				if (negociationData.itemType.Equals(type))
				{
					num += negociationData.value;
				}
			}
		}
		if (num > 75)
		{
			return 2;
		}
		if (num > 30)
		{
			return 1;
		}
		if (num < -75)
		{
			return -2;
		}
		if (num < -30)
		{
			return -1;
		}
		return 0;
	}
}

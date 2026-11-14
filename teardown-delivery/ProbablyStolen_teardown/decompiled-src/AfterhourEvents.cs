using System;
using System.Collections.Generic;
using UnityEngine;

public static class AfterhourEvents
{
	private static readonly List<AfterhourEventDefinition> definitions;

	private static int Today
	{
		get
		{
			if (StoreStation.Instance == null)
			{
				return 0;
			}
			return StoreStation.Instance.dayCounter;
		}
	}

	private static PlayerStore Store
	{
		get
		{
			if (!PlayerStore.HasInstance)
			{
				return null;
			}
			return PlayerStore.Instance;
		}
	}

	static AfterhourEvents()
	{
		definitions = new List<AfterhourEventDefinition>();
		VanillaAfterhourEvents.RegisterAll();
	}

	public static List<AfterhourEventDefinition> GetAll()
	{
		return definitions;
	}

	public static AfterhourEventDefinition GetDefinition(string id)
	{
		foreach (AfterhourEventDefinition definition in definitions)
		{
			if (definition.id == id)
			{
				return definition;
			}
		}
		return null;
	}

	public static void Register(AfterhourEventDefinition definition)
	{
		if (definition == null || string.IsNullOrEmpty(definition.id) || string.IsNullOrEmpty(definition.locationId))
		{
			Debug.LogError("[AfterhourEvents] Register needs an event with an id and a location id");
			return;
		}
		for (int i = 0; i < definitions.Count; i++)
		{
			if (definitions[i].id == definition.id)
			{
				definitions[i] = definition;
				return;
			}
		}
		definitions.Add(definition);
	}

	public static int RemoveSource(string source)
	{
		return definitions.RemoveAll((AfterhourEventDefinition definition) => definition.source == source);
	}

	public static List<AfterhourEventDefinition> GetActive(string locationId)
	{
		List<AfterhourEventDefinition> list = new List<AfterhourEventDefinition>();
		if (Store == null)
		{
			return list;
		}
		AfterhourLocationState afterhourState = Store.GetAfterhourState(locationId);
		if (afterhourState == null || afterhourState.activeEvents == null)
		{
			return list;
		}
		foreach (AfterhourActiveEvent activeEvent in afterhourState.activeEvents)
		{
			if (Today <= activeEvent.endsOnDay)
			{
				AfterhourEventDefinition definition = GetDefinition(activeEvent.eventId);
				if (definition != null)
				{
					list.Add(definition);
				}
			}
		}
		return list;
	}

	public static bool IsActive(string eventId)
	{
		AfterhourEventDefinition definition = GetDefinition(eventId);
		if (definition == null || Store == null)
		{
			return false;
		}
		AfterhourActiveEvent afterhourActiveEvent = Store.GetAfterhourState(definition.locationId).FindActive(eventId);
		if (afterhourActiveEvent != null)
		{
			return Today <= afterhourActiveEvent.endsOnDay;
		}
		return false;
	}

	public static int GetDaysLeft(string eventId)
	{
		int endsOnDay = GetEndsOnDay(eventId);
		if (endsOnDay >= 0)
		{
			return endsOnDay - Today + 1;
		}
		return 0;
	}

	public static int GetEndsOnDay(string eventId)
	{
		AfterhourEventDefinition definition = GetDefinition(eventId);
		if (definition == null || Store == null)
		{
			return -1;
		}
		AfterhourActiveEvent afterhourActiveEvent = Store.GetAfterhourState(definition.locationId).FindActive(eventId);
		if (afterhourActiveEvent == null || Today > afterhourActiveEvent.endsOnDay)
		{
			return -1;
		}
		return afterhourActiveEvent.endsOnDay;
	}

	public static bool Start(string eventId, int durationDays = 0)
	{
		AfterhourEventDefinition definition = GetDefinition(eventId);
		if (definition == null || Store == null)
		{
			Debug.LogWarning("[AfterhourEvents] Cannot start unknown event '" + eventId + "'");
			return false;
		}
		AfterhourLocationState afterhourState = Store.GetAfterhourState(definition.locationId);
		int num = Today + Mathf.Max(1, (durationDays > 0) ? durationDays : definition.durationDays) - 1;
		AfterhourActiveEvent afterhourActiveEvent = afterhourState.FindActive(eventId);
		if (afterhourActiveEvent != null && Today <= afterhourActiveEvent.endsOnDay)
		{
			afterhourActiveEvent.endsOnDay = Mathf.Max(afterhourActiveEvent.endsOnDay, num);
			return true;
		}
		if (afterhourActiveEvent != null)
		{
			afterhourState.activeEvents.Remove(afterhourActiveEvent);
		}
		int num2 = AfterhourLocation.Get(definition.locationId)?.MaxConcurrentEvents ?? 0;
		if (num2 > 0 && GetActive(definition.locationId).Count >= num2)
		{
			return false;
		}
		afterhourState.activeEvents.Add(new AfterhourActiveEvent
		{
			eventId = definition.id,
			endsOnDay = num
		});
		Run(definition.onStart, definition.id, "onStart");
		ModHook.FireOnAfterhourEventStarted(definition.locationId, definition.id);
		return true;
	}

	public static bool End(string eventId)
	{
		AfterhourEventDefinition definition = GetDefinition(eventId);
		if (definition == null || Store == null)
		{
			return false;
		}
		return EndOn(definition.locationId, eventId);
	}

	public static void EndAll(string locationId)
	{
		if (Store == null)
		{
			return;
		}
		AfterhourLocationState afterhourState = Store.GetAfterhourState(locationId);
		if (afterhourState.activeEvents == null)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (AfterhourActiveEvent activeEvent in afterhourState.activeEvents)
		{
			list.Add(activeEvent.eventId);
		}
		foreach (string item in list)
		{
			EndOn(locationId, item);
		}
	}

	private static bool EndOn(string locationId, string eventId)
	{
		AfterhourLocationState afterhourState = Store.GetAfterhourState(locationId);
		AfterhourActiveEvent afterhourActiveEvent = afterhourState.FindActive(eventId);
		if (afterhourActiveEvent == null)
		{
			return false;
		}
		afterhourState.activeEvents.Remove(afterhourActiveEvent);
		AfterhourEventDefinition definition = GetDefinition(eventId);
		if (definition != null)
		{
			Run(definition.onEnd, definition.id, "onEnd");
		}
		ModHook.FireOnAfterhourEventEnded(locationId, eventId);
		return true;
	}

	public static void OnNewDay()
	{
		if (Store == null)
		{
			return;
		}
		foreach (AfterhourLocation item in AfterhourLocation.GetAll())
		{
			AfterhourLocationState afterhourState = Store.GetAfterhourState(item.Id);
			if (afterhourState.activeEvents == null)
			{
				afterhourState.activeEvents = new List<AfterhourActiveEvent>();
			}
			List<string> list = new List<string>();
			foreach (AfterhourActiveEvent activeEvent in afterhourState.activeEvents)
			{
				if (Today > activeEvent.endsOnDay)
				{
					list.Add(activeEvent.eventId);
				}
			}
			foreach (string item2 in list)
			{
				EndOn(item.Id, item2);
			}
			if (!item.IsDiscovered())
			{
				continue;
			}
			bool flag = false;
			foreach (AfterhourEventDefinition definition in definitions)
			{
				if (!(definition.locationId != item.Id) && definition.IsScheduledOn(Today) && afterhourState.FindActive(definition.id) == null && (definition.canStart == null || AfterhourLocation.Safe(definition.canStart, item)) && Start(definition.id))
				{
					flag = true;
				}
			}
			if (flag)
			{
				continue;
			}
			foreach (AfterhourEventDefinition definition2 in definitions)
			{
				if (!(definition2.locationId != item.Id) && !(definition2.dailyChancePercent <= 0f) && afterhourState.FindActive(definition2.id) == null && (definition2.canStart == null || AfterhourLocation.Safe(definition2.canStart, item)) && RNG.Roll(definition2.dailyChancePercent))
				{
					Start(definition2.id);
				}
			}
		}
	}

	private static void Run(Action action, string eventId, string name)
	{
		if (action == null)
		{
			return;
		}
		try
		{
			action();
		}
		catch (Exception arg)
		{
			Debug.LogError($"[AfterhourEvents] event '{eventId}' {name} threw: {arg}");
		}
	}
}

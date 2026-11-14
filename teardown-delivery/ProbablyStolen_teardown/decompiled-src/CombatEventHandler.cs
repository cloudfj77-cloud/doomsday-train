using System.Collections.Generic;

public class CombatEventHandler
{
	private static List<CombatEvent> eventsToBeRemoved = new List<CombatEvent>();

	public List<CombatEvent> events { get; set; }

	public CombatEventHandler()
	{
		events = new List<CombatEvent>();
	}

	public void AddEvent(CombatEvent addedEvent, CombatManager combatManager)
	{
		if (addedEvent.InitCallback != null)
		{
			addedEvent.InitCallback(combatManager, addedEvent);
		}
		events.Add(addedEvent);
	}

	public void HandleTurnStartEvents(CombatManager combatManager)
	{
		foreach (CombatEvent @event in events)
		{
			if (@event.updateType == CombatEvent.UpdateType.TurnStart && @event.UpdateEvent(combatManager))
			{
				eventsToBeRemoved.Add(@event);
			}
		}
		RemoveExpiredEvent();
	}

	public void HandleTurnEndEvents(CombatManager combatManager)
	{
		foreach (CombatEvent @event in events)
		{
			if (@event.updateType == CombatEvent.UpdateType.TurnEnd && @event.UpdateEvent(combatManager))
			{
				eventsToBeRemoved.Add(@event);
			}
		}
		RemoveExpiredEvent();
	}

	public void RemoveExpiredEvent()
	{
		foreach (CombatEvent item in eventsToBeRemoved)
		{
			events.Remove(item);
		}
		eventsToBeRemoved.Clear();
	}

	public void HandleBattleStartEvents()
	{
	}

	public void HandleBattleEndEvents()
	{
	}
}

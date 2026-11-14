using System.Collections.Generic;
using UnityEngine;

public class EventTriggerManager : MonoBehaviour
{
	public static EventTriggerManager current { get; private set; }

	public List<EventTrigger> eventTriggerList { get; } = new List<EventTrigger>();


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
		PriorityTrigger.InitBehaviours(eventTriggerList);
	}

	public void CheckEventTriggers()
	{
		foreach (EventTrigger eventTrigger in eventTriggerList)
		{
			try
			{
				if (!(eventTrigger.Filter() ?? true) || !(eventTrigger.Trigger() ?? false))
				{
					continue;
				}
				EventData currentEvent = GameMaster.current.shipManager.currentEvent;
				if (currentEvent == null || (eventTrigger.OverrideExistingEvent(currentEvent) ?? false))
				{
					EventData eventData = eventTrigger.GenerateEvent();
					if (eventData != null)
					{
						GameMaster.current.shipManager.SetEvent(eventData);
						OverlayHandler.current.ShowEvent();
					}
				}
			}
			catch
			{
			}
		}
	}
}

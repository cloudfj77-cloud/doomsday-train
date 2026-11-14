using System.Collections.Generic;

public class AfterhourLocationState
{
	public int visits;

	public int lastVisitDay;

	public Dictionary<string, int> counters = new Dictionary<string, int>();

	public List<AfterhourActiveEvent> activeEvents = new List<AfterhourActiveEvent>();

	public int GetCounter(string key, int fallback = 0)
	{
		if (counters == null || !counters.TryGetValue(key, out var value))
		{
			return fallback;
		}
		return value;
	}

	public void SetCounter(string key, int value)
	{
		if (counters == null)
		{
			counters = new Dictionary<string, int>();
		}
		counters[key] = value;
	}

	public AfterhourActiveEvent FindActive(string eventId)
	{
		if (activeEvents == null)
		{
			activeEvents = new List<AfterhourActiveEvent>();
		}
		foreach (AfterhourActiveEvent activeEvent in activeEvents)
		{
			if (activeEvent.eventId == eventId)
			{
				return activeEvent;
			}
		}
		return null;
	}
}

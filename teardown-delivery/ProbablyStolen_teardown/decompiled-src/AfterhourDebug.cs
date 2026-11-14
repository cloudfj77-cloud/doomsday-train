using System.Text;
using QFSW.QC;

public static class AfterhourDebug
{
	[Command("afterhour-events", "Lists every afterhour location with its running events, then every registered event definition", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string List()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Locations:");
		foreach (AfterhourLocation item in AfterhourLocation.GetAll())
		{
			stringBuilder.AppendLine($"  {item.Id} ({item.Source}) discovered={item.IsDiscovered()} available={item.IsAvailable()} visits={item.State?.visits ?? 0}");
			foreach (AfterhourEventDefinition item2 in AfterhourEvents.GetActive(item.Id))
			{
				stringBuilder.AppendLine($"    running: {item2.id} until day {AfterhourEvents.GetEndsOnDay(item2.id)}");
			}
		}
		stringBuilder.AppendLine("Event definitions:");
		foreach (AfterhourEventDefinition item3 in AfterhourEvents.GetAll())
		{
			stringBuilder.Append($"  {item3.id} on {item3.locationId} ({item3.source}) {item3.durationDays}d daily={item3.dailyChancePercent}%");
			foreach (AfterhourEffect effect in item3.effects)
			{
				if (!string.IsNullOrEmpty(effect.modifierKey))
				{
					stringBuilder.Append($" {effect.modifierKey}={effect.modifierValue}");
				}
			}
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}

	[Command("afterhour-event-start", "Starts an afterhour event now for a number of days (0: its own duration)", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Start(string eventId, int days = 0)
	{
		if (AfterhourEvents.GetDefinition(eventId) == null)
		{
			return "No afterhour event named '" + eventId + "' (afterhour-events lists them).";
		}
		if (!AfterhourEvents.Start(eventId, days))
		{
			return "'" + eventId + "' could not start (location full?).";
		}
		return $"'{eventId}' runs until day {AfterhourEvents.GetEndsOnDay(eventId)}.";
	}

	[Command("afterhour-event-end", "Ends a running afterhour event, or every event of a location when given a location id", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string End(string id)
	{
		if (AfterhourEvents.GetDefinition(id) != null)
		{
			if (!AfterhourEvents.End(id))
			{
				return "'" + id + "' was not running.";
			}
			return "'" + id + "' ended.";
		}
		if (AfterhourLocation.Get(id) != null)
		{
			AfterhourEvents.EndAll(id);
			return "Every event on '" + id + "' ended.";
		}
		return "No afterhour event or location named '" + id + "'.";
	}
}

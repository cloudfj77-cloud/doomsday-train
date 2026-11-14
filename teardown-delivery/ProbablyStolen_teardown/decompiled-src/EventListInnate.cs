public class EventListInnate
{
	public static StoreEvent CreateRentDayEvent()
	{
		StoreEvent obj = new StoreEvent
		{
			identifier = "rent_day",
			newsName = LocHelper.GetLocalizedMechanic("event_rent_due"),
			displayName = LocHelper.GetLocalizedMechanic("event_rent_due"),
			newsDescription = "",
			duration = 1,
			cooldown = 0,
			eventType = StoreEvent.EventType.INNATE
		};
		obj.addClientFromEventActionId = obj.identifier + "addClientAction";
		obj.isHiddenFromNewsPaper = true;
		obj.IsKnown = true;
		return obj;
	}

	public static StoreEvent CreateMortgagePaymentEvent()
	{
		StoreEvent obj = new StoreEvent
		{
			identifier = "mortgage_payment",
			newsName = LocHelper.GetLocalizedMechanic("event_mortgage_payment"),
			displayName = LocHelper.GetLocalizedMechanic("event_mortgage_payment"),
			newsDescription = "",
			duration = 1,
			cooldown = 0,
			eventType = StoreEvent.EventType.INNATE
		};
		obj.addClientFromEventActionId = obj.identifier + "addClientAction";
		obj.isHiddenFromNewsPaper = true;
		obj.IsKnown = true;
		return obj;
	}

	public static StoreEvent CreateCartelVisitEvent()
	{
		StoreEvent obj = new StoreEvent
		{
			identifier = "cartel_visit",
			newsName = LocHelper.GetLocalizedMechanic("event_cartel_visit"),
			displayName = LocHelper.GetLocalizedMechanic("event_cartel_visit"),
			newsDescription = "",
			duration = 1,
			cooldown = 0,
			eventType = StoreEvent.EventType.INNATE
		};
		obj.addClientFromEventActionId = obj.identifier + "addClientAction";
		obj.isHiddenFromNewsPaper = true;
		obj.IsKnown = true;
		return obj;
	}
}

public class EventListUnique
{
	public static StoreEvent Wanted3CapturedEvent()
	{
		return new StoreEvent
		{
			identifier = "Wanted3CapturedEvent",
			newsName = LocHelper.GetLocalizedMechanic("event_wanted3_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_wanted3_desc"),
			duration = 1,
			importance = 4,
			eventArea = StoreEvent.EventArea.ALL,
			eventType = StoreEvent.EventType.COSMETIC
		};
	}

	public static StoreEvent CreateLouAdmiralAssassinated()
	{
		StoreEvent obj = new StoreEvent
		{
			identifier = "louAdmiralAssassinated",
			newsName = LocHelper.GetLocalizedMechanic("event_lou_admiral_news")
		};
		obj.displayName = obj.newsName;
		obj.newsDescription = LocHelper.GetLocalizedMechanic("event_lou_admiral_desc");
		obj.duration = 1;
		obj.importance = 8;
		obj.eventArea = StoreEvent.EventArea.ALL;
		obj.eventType = StoreEvent.EventType.COSMETIC;
		return obj;
	}

	public static StoreEvent CreateLouWarehouseShootout()
	{
		StoreEvent obj = new StoreEvent
		{
			identifier = "louWarehouseShootout",
			newsName = LocHelper.GetLocalizedMechanicOrDefault("event_lou_warehouse_news", "Shootout in the Lower Levels")
		};
		obj.displayName = obj.newsName;
		obj.newsDescription = LocHelper.GetLocalizedMechanicOrDefault("event_lou_warehouse_desc", "Residents of the Lower Levels reported sustained gunfire from an abandoned warehouse in the old freight district last night. Security found the building rigged with tripwires and, inside, the bodies of several heavily armed individuals in plain clothes, none carrying station identification. Nobody has come forward to claim them.");
		obj.duration = 1;
		obj.importance = 8;
		obj.eventArea = StoreEvent.EventArea.ALL;
		obj.eventType = StoreEvent.EventType.COSMETIC;
		return obj;
	}

	public static StoreEvent CreateEleanorLeakNews()
	{
		StoreEvent obj = new StoreEvent
		{
			identifier = "eleanorLeak",
			newsName = LocHelper.GetLocalizedMechanic("event_eleanor_leak_news")
		};
		obj.displayName = obj.newsName;
		obj.newsDescription = LocHelper.GetLocalizedMechanic("event_eleanor_leak_desc");
		obj.duration = 1;
		obj.importance = 9;
		obj.eventArea = StoreEvent.EventArea.ALL;
		obj.eventType = StoreEvent.EventType.COSMETIC;
		return obj;
	}

	public static StoreEvent CreateEvidenceLockerBreakIn()
	{
		StoreEvent obj = new StoreEvent
		{
			identifier = "evidenceLockerBreakIn",
			newsName = LocHelper.GetLocalizedMechanic("event_evidence_locker_news"),
			displayName = LocHelper.GetLocalizedMechanic("event_evidence_locker_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_evidence_locker_desc"),
			duration = 1,
			importance = 6,
			eventType = StoreEvent.EventType.NORMALE,
			eventArea = StoreEvent.EventArea.LOWER
		};
		obj.addClientFromEventActionId = obj.identifier + "addClientAction";
		obj.onActiveActionId = "evidenceLockerBreakInOnActive";
		return obj;
	}
}

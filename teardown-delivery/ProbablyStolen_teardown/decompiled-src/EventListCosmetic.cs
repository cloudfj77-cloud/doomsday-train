public class EventListCosmetic
{
	public static StoreEvent Example()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 1,
			importance = 1
		};
	}

	public static StoreEvent AllQuiet()
	{
		return new StoreEvent
		{
			identifier = "allQuiet",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_all_quiet_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_all_quiet_desc"),
			duration = 1,
			eventType = StoreEvent.EventType.COSMETIC,
			eventArea = StoreEvent.EventArea.ALL,
			importance = 1,
			cooldown = 0
		};
	}

	public static StoreEvent ScheduledEletricalMaintenance()
	{
		return new StoreEvent
		{
			identifier = "ScheduledEletricalMaintenance",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_electrical_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_electrical_desc"),
			duration = 1,
			eventType = StoreEvent.EventType.COSMETIC,
			eventArea = StoreEvent.EventArea.ALL,
			importance = 2,
			cooldown = 0
		};
	}

	public static StoreEvent NutrifruitGrow()
	{
		return new StoreEvent
		{
			identifier = "NutrifruitGrow",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_nutrifruit_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_nutrifruit_desc"),
			duration = 1,
			eventType = StoreEvent.EventType.COSMETIC,
			eventArea = StoreEvent.EventArea.ALL,
			importance = 1,
			cooldown = 0
		};
	}

	public static StoreEvent PeatContent()
	{
		return new StoreEvent
		{
			identifier = "PeatContent",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_peat_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_peat_desc"),
			duration = 1,
			eventType = StoreEvent.EventType.COSMETIC,
			eventArea = StoreEvent.EventArea.ALL,
			importance = 1,
			cooldown = 0
		};
	}

	public static StoreEvent RecyclingAwarenessCampaign()
	{
		return new StoreEvent
		{
			identifier = "RecyclingAwarenessCampaign",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_recycling_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_recycling_desc"),
			duration = 1,
			importance = 1,
			eventArea = StoreEvent.EventArea.ALL,
			eventType = StoreEvent.EventType.COSMETIC,
			cooldown = 0
		};
	}

	public static StoreEvent ArtistShowcase()
	{
		return new StoreEvent
		{
			identifier = "ArtistShowcase",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_artist_showcase_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_artist_showcase_desc"),
			duration = 1,
			importance = 1,
			eventArea = StoreEvent.EventArea.ALL,
			eventType = StoreEvent.EventType.COSMETIC,
			cooldown = 0
		};
	}

	public static StoreEvent MaintenanceCrewStrike()
	{
		return new StoreEvent
		{
			identifier = "MaintenanceCrewStrike",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_maintenance_strike_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_maintenance_strike_desc"),
			duration = 1,
			importance = 1,
			eventArea = StoreEvent.EventArea.ALL,
			eventType = StoreEvent.EventType.COSMETIC,
			cooldown = 0
		};
	}

	public static StoreEvent TechDemoDay()
	{
		return new StoreEvent
		{
			identifier = "TechDemoDay",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_tech_demo_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_tech_demo_desc"),
			duration = 1,
			importance = 1,
			eventArea = StoreEvent.EventArea.ALL,
			eventType = StoreEvent.EventType.COSMETIC,
			cooldown = 0
		};
	}

	public static StoreEvent NewTransitLine()
	{
		return new StoreEvent
		{
			identifier = "NewTransitLine",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_transit_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_transit_desc"),
			duration = 1,
			importance = 1,
			eventArea = StoreEvent.EventArea.ALL,
			eventType = StoreEvent.EventType.COSMETIC,
			cooldown = 0
		};
	}

	public static StoreEvent WaterFountainUnveiled()
	{
		return new StoreEvent
		{
			identifier = "WaterFountainUnveiled",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_fountain_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_fountain_desc"),
			duration = 1,
			importance = 1,
			eventArea = StoreEvent.EventArea.ALL,
			eventType = StoreEvent.EventType.COSMETIC,
			cooldown = 0
		};
	}

	public static StoreEvent CommunityGardenExpansion()
	{
		return new StoreEvent
		{
			identifier = "CommunityGardenExpansion",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_garden_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_garden_desc"),
			duration = 1,
			importance = 1,
			eventArea = StoreEvent.EventArea.ALL,
			eventType = StoreEvent.EventType.COSMETIC,
			cooldown = 0
		};
	}

	public static StoreEvent ArtContestHeld()
	{
		return new StoreEvent
		{
			identifier = "ArtContestHeld",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_art_contest_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_art_contest_desc"),
			duration = 1,
			importance = 1,
			eventArea = StoreEvent.EventArea.ALL,
			eventType = StoreEvent.EventType.COSMETIC,
			cooldown = 0
		};
	}

	public static StoreEvent CulturalCelebration()
	{
		return new StoreEvent
		{
			identifier = "CulturalCelebration",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_cultural_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_cultural_desc"),
			duration = 1,
			importance = 1,
			eventArea = StoreEvent.EventArea.ALL,
			eventType = StoreEvent.EventType.COSMETIC,
			cooldown = 0
		};
	}

	public static StoreEvent RoutineSpacewalkTraining()
	{
		return new StoreEvent
		{
			identifier = "RoutineSpacewalkTraining",
			newsName = LocHelper.GetLocalizedMechanic("event_cosmetic_spacewalk_news"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_cosmetic_spacewalk_desc"),
			duration = 1,
			importance = 1,
			eventArea = StoreEvent.EventArea.ALL,
			eventType = StoreEvent.EventType.COSMETIC,
			cooldown = 0
		};
	}

	public static StoreEvent NewDepartementHead()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 8
		};
	}

	public static StoreEvent DepartementHeadMissing()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 8
		};
	}

	public static StoreEvent DepartementHeadFoundDead()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 8
		};
	}

	public static StoreEvent DepartementHeadAssasinated()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 8
		};
	}

	public static StoreEvent DepartementHeadFired()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 8
		};
	}
}

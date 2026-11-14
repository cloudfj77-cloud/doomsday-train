public class EventUnused
{
	public static StoreEvent CreateFoodShipmentIncoming()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 4,
			eventType = StoreEvent.EventType.NORMALE,
			eventArea = StoreEvent.EventArea.ALL
		};
	}

	public static StoreEvent CreateFoodShipmentArrived()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3
		};
	}

	public static StoreEvent CreateFoodShipmentLate()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3
		};
	}

	public static StoreEvent CreateFoodShipmentMissing()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3
		};
	}

	public static StoreEvent CreateFoodShipmentArrivedLate()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3
		};
	}

	public static StoreEvent FoodShortage()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3
		};
	}

	public static StoreEvent Outbreak()
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

	public static StoreEvent IncreasedMurderRate()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 1
		};
	}

	public static StoreEvent SerialKiller()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 5
		};
	}

	public static StoreEvent KillingSpreeStopped()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 5
		};
	}

	public static StoreEvent SerialKillerStopped()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 4
		};
	}

	public static StoreEvent MartialLaw()
	{
		return new StoreEvent
		{
			identifier = "martialLaw",
			newsName = "Martial Law",
			newsDescription = "Martial law has been declared to reestablish order on the station. Businesses can expect less customer as resident must stay home except for essential activities. Security checkpoint will be established over every level.",
			duration = 3,
			eventType = StoreEvent.EventType.NORMALE,
			eventArea = StoreEvent.EventArea.LOWER,
			importance = 8
		};
	}

	public static StoreEvent MartialLawLifted()
	{
		return new StoreEvent
		{
			identifier = "martialLawLifted",
			newsName = "Martial Law Lifted",
			newsDescription = "Martial law has been lifted and the station return back to normal",
			duration = 1
		};
	}

	public static StoreEvent SecurityIncrease()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 4
		};
	}

	public static StoreEvent SecurityBudgetCut()
	{
		return new StoreEvent
		{
			identifier = "SecurityBudgetCut",
			newsName = "Security Department Budget Cut",
			displayName = "Security Budget Cut",
			newsDescription = "Significant budget cuts has been implemented on the station's security. These measures aims to improve the cost effectiveness. All schedules inspections has been pushed back. All actives investigation will be impacted by the budget cut.",
			duration = 1,
			onActiveActionId = "SecurityBudgetCutOnActive",
			importance = 5
		};
	}

	public static StoreEvent CivilUnrest()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3
		};
	}

	public static StoreEvent Discontent()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3
		};
	}

	public static StoreEvent Protest()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3
		};
	}

	public static StoreEvent CoupEtat()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 10
		};
	}

	public static StoreEvent TerrorAttack()
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

	public static StoreEvent TerroristCellDismantled()
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

	public static StoreEvent CreateExempleEvent()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 4,
			eventType = StoreEvent.EventType.NORMALE,
			eventArea = StoreEvent.EventArea.ALL
		};
	}

	public static StoreEvent CreateNutrifruitBoom()
	{
		return new StoreEvent
		{
			identifier = "",
			newsName = "",
			newsDescription = "",
			duration = 3,
			importance = 4,
			eventType = StoreEvent.EventType.NORMALE,
			eventArea = StoreEvent.EventArea.ALL
		};
	}
}

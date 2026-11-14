public class EventListThreat
{
	public static StoreEvent StationBlockaded()
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

	public static StoreEvent PirateRaid()
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

	public static StoreEvent RepublicMarineArrived()
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

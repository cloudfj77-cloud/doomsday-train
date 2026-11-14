public class OverworldStation : OverworldPOI
{
	public enum StationType
	{
		None,
		Waystation,
		Military,
		Hydroponics,
		Hospital,
		Research
	}

	private const int MILITARY_STATION_CHANCE = 15;

	private const int HYDROPONIC_STATION_CHANCE = 20;

	private const int HOSTIPITAL_STATION_CHANCE = 20;

	private const int RESEARCH_STATION_CHANCE = 15;

	private const int WAYSTATION_STATION_CHANCE = 20;

	private const int STATION_SIZE_LARGE_CHANCE = 25;

	private const int STATION_SIZE_MEDIUM_CHANCE = 35;

	private const int STATION_SIZE_SMALL_CHANCE = 0;

	private const int STATION_HOSTILE_CHANCE = 75;

	public bool isHostile;

	public Schematic.StationSize size;

	public StationType type;

	public Station generatedStation;

	public void GenerateDisplayName(bool isTrade)
	{
		displayName = "";
		customSpritePath = "trade_station";
		if (size == Schematic.StationSize.small)
		{
			displayName += "Small ";
		}
		else if (size == Schematic.StationSize.medium)
		{
			displayName = displayName ?? "";
		}
		else if (size == Schematic.StationSize.medium)
		{
			displayName += "Large ";
		}
		if (type == StationType.Waystation)
		{
			displayName += "Waystation";
		}
		else if (type == StationType.Hospital)
		{
			displayName += "Hospital Station";
		}
		else if (type == StationType.Military)
		{
			displayName += "Military Station";
		}
		else if (type == StationType.Hydroponics)
		{
			displayName += "Hydroponics Station";
		}
		else if (type == StationType.Research)
		{
			displayName += "Research Station";
		}
		if (isTrade)
		{
			displayName = "Trade Station";
			customSpritePath = "trade_station";
		}
	}

	public OverworldStation PrepareEncounter(bool forceTrade = false)
	{
		isHostile = PickStationHostility();
		size = PickStationSize();
		type = PickStationType();
		if (forceTrade)
		{
			isHostile = false;
		}
		GenerateDisplayName(!isHostile);
		generatedStation = SchematicStandard.SchematicStandardPrepare(size, displayName, type, isHostile).Generate();
		return this;
	}

	public static bool PickStationHostility()
	{
		if (RNG.Roll(75))
		{
			return true;
		}
		return false;
	}

	public static Schematic.StationSize PickStationSize()
	{
		int[] oddArray = new int[3] { 0, 35, 25 };
		Schematic.StationSize[] obj = new Schematic.StationSize[3]
		{
			Schematic.StationSize.small,
			Schematic.StationSize.medium,
			Schematic.StationSize.large
		};
		int odd = RNG.GetOdd(oddArray);
		return obj[odd];
	}

	public static StationType PickStationType()
	{
		int[] oddArray = new int[5] { 15, 20, 20, 15, 20 };
		StationType[] obj = new StationType[5]
		{
			StationType.Military,
			StationType.Hydroponics,
			StationType.Hospital,
			StationType.Research,
			StationType.Waystation
		};
		int odd = RNG.GetOdd(oddArray);
		return obj[odd];
	}
}

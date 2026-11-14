public class StoreClientFactionSetup
{
	public const string SCAV = "SCAV";

	public const string WORKER = "WORKER";

	public const string LOWER_LEVEL_CITIZEN = "LOWER_LEVEL_CITIZEN";

	public const string SECURITY = "SECURITY";

	public const string UPPER_LEVEL_CITIZEN = "UPPER_LEVEL_CITIZEN";

	public const string TOURIST = "TOURIST";

	public const string REV = "REV";

	public const string BLACK_MARKET = "BLACK_MARKET";

	public static void InitScav(StoreClient storeClient)
	{
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.AddTag("SCAV");
	}

	public static void InitWorker(StoreClient storeClient)
	{
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.AddTag("WORKER");
	}

	public static void InitLowerLevelCitizen(StoreClient storeClient)
	{
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.AddTag("LOWER_LEVEL_CITIZEN");
	}

	public static void InitSecurity(StoreClient storeClient)
	{
		storeClient.clientFaction = "FACTION_SECURITY";
		storeClient.AddTag("SECURITY");
		storeClient.isSecurity = true;
	}

	public static void InitUpperLevelCitizen(StoreClient storeClient)
	{
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.AddTag("UPPER_LEVEL_CITIZEN");
	}

	public static void InitTourist(StoreClient storeClient)
	{
		storeClient.clientFaction = "FACTION_TOURIST";
		storeClient.AddTag("TOURIST");
		storeClient.SetTourist();
	}

	public static void InitRev(StoreClient storeClient)
	{
		storeClient.clientFaction = "FACTION_REVOLUTION";
		storeClient.AddTag("REV");
	}

	public static void InitBlackMarket(StoreClient storeClient)
	{
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.AddTag("BLACK_MARKET");
	}

	public static void InitChurch(StoreClient storeClient)
	{
		storeClient.clientFaction = "FACTION_CHURCH";
		storeClient.AddTag("FACTION_CHURCH");
	}

	public static void InitCartel(StoreClient storeClient)
	{
		storeClient.clientFaction = "FACTION_CARTEL";
		storeClient.AddTag("FACTION_CARTEL");
	}
}

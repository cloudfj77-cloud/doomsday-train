public static class VanillaAfterhourEvents
{
	public const string FRESH_DUMP = "fresh_dump";

	public const float FRESH_DUMP_JUNK_CHANGE = -50f;

	public const float FRESH_DUMP_INJURY_CHANGE = 25f;

	public const AfterhourWeekday FRESH_DUMP_DAY = AfterhourWeekday.Wednesday;

	public const string SPECIAL_STOCK = "special_stock";

	public const float SPECIAL_STOCK_PRICE_CHANGE = 150f;

	public const AfterhourWeekday SPECIAL_STOCK_DAY = AfterhourWeekday.Monday;

	public const string JACKSON_CLOSED = "jackson_closed";

	public const string JACKSON_DISCOUNT = "jackson_discount";

	public const float JACKSON_DISCOUNT_CHANGE = -10f;

	public const AfterhourWeekday JACKSON_DISCOUNT_DAY = AfterhourWeekday.Sunday;

	public const string COMMISSARY_BURGLARY = "commissary_burglary";

	public const string FRESH_RESTOCK = "fresh_restock";

	public const float FRESH_RESTOCK_PERCENT_OF_MAX = 35f;

	public const AfterhourWeekday FRESH_RESTOCK_DAY = AfterhourWeekday.Friday;

	public const string GEORGIO_ASSISTANCE = "georgio_assistance";

	public const int GEORGIO_EXTRA_ATTEMPTS = 5;

	public const int GEORGIO_DAYS = 1;

	public const float GEORGIO_DAILY_CHANCE = 0f;

	public const int GEORGIO_PAID_DAYS = 3;

	public const int GEORGIO_PAID_PRICE = 50;

	public static void RegisterAll()
	{
		AfterhourEvents.Register(new AfterhourEventDefinition
		{
			id = "fresh_dump",
			locationId = "dumping_ground",
			title = () => LocHelper.GetLocalizedMechanic("mech_afterhour_event_fresh_dump_title"),
			description = () => LocHelper.GetLocalizedMechanic("mech_afterhour_event_fresh_dump_desc"),
			durationDays = 1
		}.Every(AfterhourWeekday.Wednesday).AddEffect(() => LocHelper.GetLocalizedMechanic("mech_afterhour_effect_junk_less", 50f), "junk_chance", -50f, AfterhourEffectPolarity.Good).AddEffect(() => LocHelper.GetLocalizedMechanic("mech_afterhour_effect_injury_more", 25f), "injury_chance", 25f, AfterhourEffectPolarity.Bad));
		AfterhourEvents.Register(new AfterhourEventDefinition
		{
			id = "georgio_assistance",
			locationId = "dumping_ground",
			title = () => LocHelper.GetLocalizedMechanic("mech_afterhour_event_georgio_title"),
			description = () => LocHelper.GetLocalizedMechanic("mech_afterhour_event_georgio_desc"),
			durationDays = 1,
			dailyChancePercent = 0f
		}.AddEffect(() => LocHelper.GetLocalizedMechanic("mech_afterhour_effect_no_injuries"), "injury_immunity", 1f, AfterhourEffectPolarity.Good).AddEffect(() => LocHelper.GetLocalizedMechanic("mech_afterhour_effect_extra_attempts", 5), "scav_attempts", 5f, AfterhourEffectPolarity.Good));
		AfterhourEvents.Register(new AfterhourEventDefinition
		{
			id = "jackson_closed",
			locationId = "dr_jackson",
			title = () => LocHelper.GetLocalizedMechanic("mech_afterhour_event_jackson_closed_title"),
			description = () => LocHelper.GetLocalizedMechanic("mech_afterhour_event_jackson_closed_desc"),
			durationDays = 1,
			closesLocation = true,
			toneOverride = AfterhourEventTone.Bad,
			canStart = () => !NetworkUpgrade.IsUnlocked("JACKSON_SPECIAL_STOCK")
		}.Every(AfterhourWeekday.Monday));
		AfterhourEvents.Register(new AfterhourEventDefinition
		{
			id = "special_stock",
			locationId = "dr_jackson",
			title = () => LocHelper.GetLocalizedMechanic("mech_afterhour_event_special_stock_title"),
			durationDays = 1,
			canStart = () => NetworkUpgrade.IsUnlocked("JACKSON_SPECIAL_STOCK")
		}.Every(AfterhourWeekday.Monday).AddEffect(() => LocHelper.GetLocalizedMechanic("mech_afterhour_effect_rare_stock"), null, 0f, AfterhourEffectPolarity.Good).AddEffect(() => LocHelper.GetLocalizedMechanic("mech_afterhour_effect_prices_higher", 150f), "inventor_sell_price", 150f, AfterhourEffectPolarity.Bad));
		AfterhourEvents.Register(new AfterhourEventDefinition
		{
			id = "jackson_discount",
			locationId = "dr_jackson",
			title = () => LocHelper.GetLocalizedMechanic("mech_afterhour_event_discount_title"),
			durationDays = 1
		}.Every(default(AfterhourWeekday)).AddEffect(() => LocHelper.GetLocalizedMechanic("mech_afterhour_effect_prices_lower", 10f), "inventor_sell_price", -10f, AfterhourEffectPolarity.Good));
		AfterhourEvents.Register(new AfterhourEventDefinition
		{
			id = "commissary_burglary",
			locationId = "commissary",
			title = () => LocHelper.GetLocalizedMechanic("mech_afterhour_event_burglary_title"),
			description = () => LocHelper.GetLocalizedMechanic("mech_afterhour_event_burglary_desc"),
			durationDays = 1
		}.AddEffect(() => LocHelper.GetLocalizedMechanic("mech_afterhour_effect_reduced_stock"), null, 0f, AfterhourEffectPolarity.Bad));
		AfterhourEvents.Register(new AfterhourEventDefinition
		{
			id = "fresh_restock",
			locationId = "commissary",
			title = () => LocHelper.GetLocalizedMechanic("mech_afterhour_event_fresh_restock_title"),
			durationDays = 1
		}.Every(AfterhourWeekday.Friday).AddEffect(() => LocHelper.GetLocalizedMechanic("mech_afterhour_effect_restock_percent", 35f), "commissary_restock_percent_of_max", 35f, AfterhourEffectPolarity.Good));
	}

	public static void OnSpecialStockUnlocked()
	{
		if (AfterhourEvents.IsActive("jackson_closed"))
		{
			AfterhourEvents.End("jackson_closed");
			AfterhourEvents.Start("special_stock");
		}
	}
}

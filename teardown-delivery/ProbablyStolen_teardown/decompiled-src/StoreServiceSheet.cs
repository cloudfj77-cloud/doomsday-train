public class StoreServiceSheet
{
	public const int HYPERTECH_SMUGGLER_SERVICE_COST = 500;

	public const int HYPERTECH_SMUGGLER_SERVICE_COOLDOWN = 5;

	public static StoreService CreatePowerService()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_power"),
			id = "POWER_SERVICE",
			cost = 5,
			unlocked = true,
			isUsing = true,
			tooltip = "ui_info_service_power"
		};
	}

	public static StoreService CreateJanitorialService()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_janitorial"),
			id = "JANITORIAL_SERVICE",
			cost = 3,
			unlocked = false,
			tooltip = "ui_info_service_janitorial"
		};
	}

	public static StoreService CreateInsuranceService()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_insurance"),
			id = "INSURANCE_SERVICE",
			cost = 4,
			unlocked = false,
			tooltip = "ui_info_service_insurance"
		};
	}

	public static StoreService CreateCartelProtection()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_cartel"),
			id = "CARTEL_PROTECTION",
			cost = 0,
			unlocked = false,
			isMandatory = true
		};
	}

	public static StoreService CreateInspectionInformationService()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_inspection"),
			id = "SEC_INFORMANT",
			cost = 10,
			unlocked = false
		};
	}

	public static StoreService CreateBuyReferalService()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_buy_referal"),
			id = "BUY_REFERAL",
			cost = 10,
			unlocked = false
		};
	}

	public static StoreService CreateSellReferalService()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_sell_referal"),
			id = "SELL_REFERAL",
			cost = 10,
			unlocked = false
		};
	}

	public static StoreService CreateCriminelNetwork()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_criminel_network"),
			id = "CRIMINEL_NETWORK",
			cost = 45,
			unlocked = false,
			totalCooldown = 3
		};
	}

	public static StoreService CreateCurrentConverterService()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_current_converter"),
			id = "CURRENT_CONVERTER_SERVICE",
			cost = 0,
			unlocked = false
		};
	}

	public static StoreService CreateHypertechSmugglerService()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_hypertech_smuggler"),
			id = "HYPERTECH_SMUGGLER_SERVICE",
			cost = 500,
			unlocked = false,
			totalCooldown = 5
		};
	}

	public static StoreService CreateDebrisRemovalService()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_debris_removal"),
			id = "DEBRIS_REMOVAL_SERVICE",
			cost = 50,
			unlocked = false,
			isUsing = false
		};
	}

	public static StoreService CreateButcherHuntingService()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_butcher_hunting"),
			id = "BUTCHER_HUNTING_SERVICE",
			cost = 500,
			unlocked = false,
			isUsing = false
		};
	}

	public static StoreService CreateWildFixerService()
	{
		return new StoreService
		{
			displayName = LocHelper.GetLocalizedMechanic("service_wild_fixer"),
			id = "WILD_FIXER",
			cost = 25,
			unlocked = false
		};
	}
}

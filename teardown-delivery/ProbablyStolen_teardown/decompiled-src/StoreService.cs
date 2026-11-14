using System.Collections.Generic;

public class StoreService
{
	[ES3Serializable]
	public string id;

	[ES3Serializable]
	public string displayName;

	[ES3Serializable]
	public string description;

	[ES3Serializable]
	public int cost;

	[ES3Serializable]
	public bool unlocked;

	[ES3Serializable]
	public bool introduced;

	[ES3Serializable]
	public bool isUsing;

	[ES3Serializable]
	public int totalCooldown;

	[ES3Serializable]
	public int currentCooldown;

	[ES3Serializable]
	public bool isMandatory;

	[ES3Serializable]
	public string tooltip;

	public const string POWER_SERVICE = "POWER_SERVICE";

	public const string INSURANCE_SERVICE = "INSURANCE_SERVICE";

	public const string JANITORIAL_SERVICE = "JANITORIAL_SERVICE";

	public const string SEC_INFORMANT = "SEC_INFORMANT";

	public const string WILD_FIXER = "WILD_FIXER";

	public const string BUY_REFERAL = "BUY_REFERAL";

	public const string SELL_REFERAL = "SELL_REFERAL";

	public const string CRIMINEL_NETWORK = "CRIMINEL_NETWORK";

	public const string CARTEL_PROTECTION = "CARTEL_PROTECTION";

	public const string CURRENT_CONVERTER_SERVICE = "CURRENT_CONVERTER_SERVICE";

	public const string DEBRIS_REMOVAL_SERVICE = "DEBRIS_REMOVAL_SERVICE";

	public const string BUTCHER_HUNTING_SERVICE = "BUTCHER_HUNTING_SERVICE";

	public const string HYPERTECH_SMUGGLER_SERVICE = "HYPERTECH_SMUGGLER_SERVICE";

	public const int JANITORIAL_BASE_COST = 3;

	public static List<StoreService> InitService()
	{
		return new List<StoreService>
		{
			StoreServiceSheet.CreatePowerService(),
			StoreServiceSheet.CreateInsuranceService(),
			StoreServiceSheet.CreateJanitorialService(),
			StoreServiceSheet.CreateInspectionInformationService(),
			StoreServiceSheet.CreateBuyReferalService(),
			StoreServiceSheet.CreateSellReferalService(),
			StoreServiceSheet.CreateCriminelNetwork(),
			StoreServiceSheet.CreateWildFixerService(),
			StoreServiceSheet.CreateCartelProtection(),
			StoreServiceSheet.CreateDebrisRemovalService(),
			StoreServiceSheet.CreateButcherHuntingService(),
			StoreServiceSheet.CreateCurrentConverterService(),
			StoreServiceSheet.CreateHypertechSmugglerService()
		};
	}

	public bool IsActive()
	{
		if (unlocked && isUsing)
		{
			return true;
		}
		return false;
	}

	public static StoreService GetPowerService()
	{
		return FindServiceByID("POWER_SERVICE");
	}

	public static StoreService GetJanitorialService()
	{
		return FindServiceByID("JANITORIAL_SERVICE");
	}

	public static StoreService GetInsuranceService()
	{
		return FindServiceByID("INSURANCE_SERVICE");
	}

	public static StoreService GetInspectionEarlyWarningService()
	{
		return FindServiceByID("SEC_INFORMANT");
	}

	public static StoreService GetBuyReferalService()
	{
		return FindServiceByID("BUY_REFERAL");
	}

	public static StoreService GetWildFixerService()
	{
		return FindServiceByID("WILD_FIXER");
	}

	public static StoreService GetSellReferalService()
	{
		return FindServiceByID("SELL_REFERAL");
	}

	public static StoreService GetCriminelNetworkService()
	{
		return FindServiceByID("CRIMINEL_NETWORK");
	}

	public static StoreService GetCartelProtectionService()
	{
		return FindServiceByID("CARTEL_PROTECTION");
	}

	public static StoreService GetCurrentConverterService()
	{
		return FindServiceByID("CURRENT_CONVERTER_SERVICE");
	}

	public static StoreService GetHypertechSmugglerService()
	{
		return FindServiceByID("HYPERTECH_SMUGGLER_SERVICE");
	}

	public static StoreService GetDebrisRemovalService()
	{
		return FindServiceByID("DEBRIS_REMOVAL_SERVICE");
	}

	public static StoreService GetButcherHuntingService()
	{
		return FindServiceByID("BUTCHER_HUNTING_SERVICE");
	}

	public static StoreService FindServiceByID(string id)
	{
		foreach (StoreService item in PlayerStore.Instance.storeService)
		{
			if (item.id.Equals(id))
			{
				return item;
			}
		}
		return null;
	}

	private static bool IsCartelProtectionActive()
	{
		return GetCartelProtectionService()?.unlocked ?? false;
	}

	public static void HealIntroducedAfterLoad()
	{
		if (GetCurrentConverterService() == null)
		{
			PlayerStore.Instance.storeService.Add(StoreServiceSheet.CreateCurrentConverterService());
		}
		if (GetHypertechSmugglerService() == null)
		{
			PlayerStore.Instance.storeService.Add(StoreServiceSheet.CreateHypertechSmugglerService());
		}
		if (GetDebrisRemovalService() == null)
		{
			PlayerStore.Instance.storeService.Add(StoreServiceSheet.CreateDebrisRemovalService());
		}
		if (GetButcherHuntingService() == null)
		{
			PlayerStore.Instance.storeService.Add(StoreServiceSheet.CreateButcherHuntingService());
		}
		foreach (StoreService item in PlayerStore.Instance.storeService)
		{
			if (!item.introduced)
			{
				if (item.unlocked)
				{
					item.introduced = true;
				}
				else if (item.id.Equals("INSURANCE_SERVICE") && (IsCartelProtectionActive() || StoreReputation.IsPerkUnlocked("BM_UNINSURABLE")))
				{
					item.introduced = true;
				}
			}
		}
	}

	public static void ApplyDifficultyStart()
	{
		DifficultyRules rules = Difficulty.Rules;
		GetPowerService().cost += rules.startPowerCostBonus;
		GetJanitorialService().cost += rules.startJanitorialCostBonus;
		GetInsuranceService().cost += rules.startInsuranceCostBonus;
		PlayerStore.Instance.retailMarkup = rules.retailMarkup;
		NetworkUpgrade.GetUpgradeById("LANDLORD_GRACE").state = ((!rules.landlordGraceOwnedAtStart) ? 1 : 2);
	}

	public void UpdateCost()
	{
		if (id.Equals("WILD_FIXER"))
		{
			int wildFixerTier = NetworkUpgrade.GetWildFixerTier();
			cost = NetworkUpgradeList.WILD_FIXER_FEE[wildFixerTier - 1];
			switch (wildFixerTier)
			{
			case 3:
				displayName = LocHelper.GetLocalizedNetworkUpgrade("network_WILD_FIXER3_title");
				break;
			case 2:
				displayName = LocHelper.GetLocalizedNetworkUpgrade("network_WILD_FIXER2_title");
				break;
			default:
				displayName = LocHelper.GetLocalizedMechanic("service_wild_fixer");
				break;
			}
		}
		if (id.Equals("INSURANCE_SERVICE"))
		{
			unlocked = introduced && !IsCartelProtectionActive() && !StoreReputation.IsPerkUnlocked("BM_UNINSURABLE");
		}
		if (id.Equals("JANITORIAL_SERVICE"))
		{
			unlocked = introduced || StartingPerk.IsSawyerCrew();
			cost = StoreStation.Instance.dayCounter / 10 + 3;
			if (PlayerStore.Instance.isVandalized)
			{
				cost = StoreStation.Instance.dayCounter / 10 + 75;
			}
		}
		if (StartingPerk.IsSawyerCrew() && id.Equals("JANITORIAL_SERVICE"))
		{
			cost = 0;
			if (PlayerStore.Instance.isVandalized)
			{
				cost = StoreStation.Instance.dayCounter / 5 + 30;
			}
		}
		if (id.Equals("JANITORIAL_SERVICE") && StoreReputation.IsPerkUnlocked("BM_DISTRUSTED"))
		{
			cost *= 2;
		}
		if (id.Equals("POWER_SERVICE"))
		{
			cost = (StoreStation.Instance.dayCounter / 10 + 5) * Difficulty.Rules.powerCostMultiplier;
			if (NetworkUpgrade.IsPowerHijackUnlocked())
			{
				cost = 0;
				isUsing = true;
			}
			if (StoreStation.Instance.storeEventManager.IsBlackoutEventActive())
			{
				cost = 0;
			}
			if (StartingPerk.IsMakeshiftReactor())
			{
				cost = 0;
				isUsing = false;
				unlocked = false;
			}
		}
		else if (id.Equals("CURRENT_CONVERTER_SERVICE"))
		{
			CurrentConverterHelper.UpdateServiceCost(this);
		}
		else if (id.Equals("DEBRIS_REMOVAL_SERVICE"))
		{
			unlocked = DebrisHelper.HasServicePile();
			cost = 50;
			if (!unlocked)
			{
				isUsing = false;
			}
		}
		else if (id.Equals("BUTCHER_HUNTING_SERVICE"))
		{
			unlocked = StoreClientData.IsButcherHunting() && PlayerStore.Instance.storeClientData.isSawyerButcherOfferDone;
			cost = 500;
			displayName = LocHelper.GetLocalizedMechanic("service_butcher_hunting");
			if (!unlocked)
			{
				isUsing = false;
			}
		}
		else if (id.Equals("INSURANCE_SERVICE"))
		{
			cost = PlayerStore.Instance.GetTotalEstimatedValue() / 400 + 3;
			if (StoreReputation.IsPerkUnlocked("LLTARGET"))
			{
				cost *= 2;
			}
			cost *= Difficulty.Rules.insuranceCostMultiplier;
			if (StartingPerk.IsWildFavorite())
			{
				cost = RNG.MultiplyAndRound(cost, 0.5);
			}
			if (StartingPerk.IsWildBadSide())
			{
				cost = RNG.MultiplyAndRound(cost, 1.5);
			}
		}
	}

	public bool IsOnCooldown()
	{
		if (currentCooldown == 0)
		{
			return false;
		}
		return true;
	}
}

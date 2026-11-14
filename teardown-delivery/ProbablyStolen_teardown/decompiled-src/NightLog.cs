public class NightLog
{
	public static void AugLeftAlive()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_aug_left_alive"), "cc7755");
	}

	public static void AddNewPromotionalOffer()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_promo_offer"));
	}

	public static void BuyerReferral()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_buyer_referral"));
	}

	public static void SellerReferral()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_seller_referral"));
	}

	public static void CriminalNetwork()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_criminal_network"));
	}

	public static void InspectionWarningIn2Days()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_inspection_2days"));
	}

	public static void InspectionWarningTomorrow()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_inspection_tomorrow"), "cc7755");
	}

	public static void InspectionWarningImminent()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_inspection_imminent"), "cc7755");
	}

	public static void AugIncoming()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_aug_incoming"), "9c3738");
	}

	public static void WantedPersonVisited(string name)
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_wanted_visited", name));
	}

	public static void UpgradeTip()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_upgrade_tip"));
	}

	public static void StingOperationWarning()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_sting_warning"));
	}

	public static void SecurityInvestigationNearComplete()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_investigation_near"), "cc7755");
	}

	public static void AgoraphobiaReady()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_agoraphobia_ready"), "5c6f46");
	}

	public static void AfterhourDebuff()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_afterhour_debuff"), "9c3738");
	}

	public static void FixerUsed()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_fixer_used"));
	}

	public static void StorefrontNotCleaned(string additional)
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_storefront_not_cleaned", additional), "cc7755");
	}

	public static void StorefrontClean()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_storefront_clean"));
	}

	public static void VandalismCleaned()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_vandalism_cleaned"));
	}

	public static string GetGarbageDescription(int garbageLevel)
	{
		if (garbageLevel <= 2)
		{
			return LocHelper.GetLocalizedMechanic("mech_nightlog_garbage_clean");
		}
		if (garbageLevel <= 5)
		{
			return LocHelper.GetLocalizedMechanic("mech_nightlog_garbage_some");
		}
		if (garbageLevel <= 8)
		{
			return LocHelper.GetLocalizedMechanic("mech_nightlog_garbage_lot");
		}
		return LocHelper.GetLocalizedMechanic("mech_nightlog_garbage_extreme");
	}

	public static void PowerOff()
	{
		if (!StartingPerk.IsMakeshiftReactor())
		{
			PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_power_off"), "cc7755");
		}
	}

	public static void PowerOn()
	{
		if (!NetworkUpgrade.IsPowerHijackUnlocked() && !StartingPerk.IsMakeshiftReactor())
		{
			PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_power_on"));
		}
	}

	public static void CartelProtectionActive()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_cartel_protection"));
	}

	public static void InsuranceUnpaid(int dayWithoutInsurance)
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_insurance_unpaid", dayWithoutInsurance));
	}

	public static void BurglaryGrace(int nightsLeft)
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_burglary_grace", nightsLeft));
	}

	public static void BurglaryAlarmStopped()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_burglary_alarm_stopped"));
	}

	public static void BurglaryAlarmFailed(string itemName)
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_burglary_alarm_failed", itemName), "9c3738");
	}

	public static void BurglaryNoAlarm(string itemName)
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_burglary_no_alarm", itemName), "9c3738");
	}

	public static void AlarmNoPower(int energyUsage)
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanicOrDefault("mech_nightlog_alarm_no_power", "The security alarm did not arm: its cell cannot supply the {0} kJ it draws per night.", energyUsage), "cc7755");
	}

	public static void InsurancePaid()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_insurance_paid"));
	}

	public static void BurglaryInsured(string itemName)
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_burglary_insured", itemName));
	}

	public static void VandalizedByFaction(string factionId)
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic(factionId switch
		{
			"FACTION_LOWER_LEVEL" => "mech_nightlog_vandalized_lower", 
			"FACTION_SECURITY" => "mech_nightlog_vandalized_security", 
			"FACTION_UPPER_LEVEL" => "mech_nightlog_vandalized_upper", 
			"FACTION_REVOLUTION" => "mech_nightlog_vandalized_revolution", 
			"FACTION_CARTEL" => "mech_nightlog_vandalized_cartel", 
			_ => "mech_nightlog_vandalized_generic", 
		}), "9c3738");
	}

	public static void StoreStillVandalized()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_store_vandalized"), "cc7755");
	}

	public static void FactionOperationDateShared()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_faction_op_date"), "5c6f46");
	}

	public static void ContactReady(string contactName)
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_contact_ready", contactName), "5c6f46");
	}

	public static void BlackoutBypassed()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_blackout_bypassed"), "5c6f46");
	}

	public static void BlackoutActive()
	{
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_blackout_active"), "cc7755");
	}
}

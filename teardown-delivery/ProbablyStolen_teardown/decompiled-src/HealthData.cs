using System.Collections.Generic;
using UnityEngine;

public class HealthData
{
	public const int ADDICTION_CURE_DAY = 20;

	public const int ADDICTION_DEBUFF_DAY = 3;

	public int daysSinceNicotineConsumption;

	public int daysSinceAlcoholConsumption;

	public int daysSinceNarcoticConsumption;

	public const int NO_NARCOTIC_RELIEF = 999;

	public int daysSinceNarcoticRelief = 999;

	public int daysSinceSentinelInjection;

	public int sentinelPool;

	public const int SENTINEL_WARNING_DAY = 6;

	public const int SENTINEL_OVERDUE_DAY = 7;

	public const int SENTINEL_SEVERE_DAY = 10;

	public const int SENTINEL_CRITICAL_DAY = 14;

	public const int SENTINEL_ADVANCED_POOL = 10;

	public const int SENTINEL_FINAL_POOL = 15;

	public const int SENTINEL_DEATH_POOL = 20;

	public const int IMMUNIVAX_REGULAR_RESET = 0;

	public const int IMMUNIVAX_EXPIRED_RESET = 3;

	public const int IMMUNIVAX_COUNTERFEIT_RESET = 3;

	public int woundState;

	public bool isWoundStable = true;

	public bool isWoundedFresh;

	public const int SERIOUS_WOUND_THRESHOLD = 5;

	public bool wentToAfterhoursLastNight;

	public int luck;

	public int liquidLuckDaysLeft;

	public static int GetLuck()
	{
		HealthData healthData = PlayerStore.Instance?.healthData;
		if (healthData == null)
		{
			return 0;
		}
		return healthData.luck + LotteryHelper.GetCarriedLuckBonus() + LiquidLuckHelper.GetActiveLuckBonus(healthData);
	}

	public static bool RollLuck()
	{
		return RNG.Roll(GetLuck());
	}

	public static bool RollBadLuck()
	{
		int num = GetLuck();
		if (num >= 0)
		{
			return false;
		}
		return RNG.Roll(-num);
	}

	public static bool IsSeriouslyWounded()
	{
		return PlayerStore.Instance.healthData.woundState > 5;
	}

	public static void HandleNightlyWound()
	{
		PlayerStore.Instance.healthData.isWoundedFresh = false;
		if (PlayerStore.Instance.healthData.woundState == 0)
		{
			PlayerStore.Instance.healthData.isWoundStable = true;
			return;
		}
		if (PlayerStore.Instance.healthData.woundState > 10)
		{
			PlayerStore.Instance.healthData.woundState = 10;
			if (!PlayerStore.Instance.healthData.isWoundStable)
			{
				PlayerStore.Instance.healthData.isWoundStable = true;
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_wound_stabilized"), "5c6f46");
				PlayerStore.Instance.healthData.woundState--;
				return;
			}
		}
		if (PlayerStore.Instance.healthData.isWoundStable)
		{
			int num = PlayerStore.Instance.healthData.woundState;
			PlayerStore.Instance.healthData.woundState -= 4;
			if (PlayerStore.Instance.healthData.woundState < 0)
			{
				PlayerStore.Instance.healthData.woundState = 0;
			}
			if (PlayerStore.Instance.healthData.woundState == 0)
			{
				PlayerStore.Instance.healthData.isWoundStable = true;
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_wound_healed"), "5c6f46");
			}
			else if (num > 5 && PlayerStore.Instance.healthData.woundState <= 5)
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_wound_mobility_regained"), "5c6f46");
			}
			else
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_wound_minor_healing"), "5c6f46");
			}
		}
		else if (RNG.Roll(10))
		{
			PlayerStore.Instance.healthData.woundState++;
			if (PlayerStore.Instance.healthData.woundState == 6)
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_wound_major_lost_mobility"), "9c3738");
			}
			else if (PlayerStore.Instance.healthData.woundState > 6)
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_wound_major_worsened"), "9c3738");
			}
			else
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_wound_minor_worsened"), "9c3738");
			}
		}
		else if (RNG.Roll(50))
		{
			PlayerStore.Instance.healthData.isWoundStable = true;
			PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_wound_stabilized"), "5c6f46");
		}
	}

	public static void ReceiveMinorWound()
	{
		PlayerStore.Instance.healthData.isWoundedFresh = true;
		PlayerStore.Instance.healthData.woundState += 2;
		PlayerStore.Instance.healthData.isWoundStable = false;
		StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_wound_minor_received"), "sec");
	}

	public static void ReceiveMajorWound()
	{
		PlayerStore.Instance.healthData.isWoundedFresh = true;
		PlayerStore.Instance.healthData.woundState += 5;
		PlayerStore.Instance.healthData.isWoundStable = false;
		StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_wound_major_received"), "sec");
	}

	public static bool IsNicotineAddicted()
	{
		foreach (StartingPerk startingPerk in PlayerStore.Instance.startingPerks)
		{
			if (startingPerk.id.Equals("nicotine_addiction"))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsAlcoholAddicted()
	{
		foreach (StartingPerk startingPerk in PlayerStore.Instance.startingPerks)
		{
			if (startingPerk.id.Equals("alcohol_addiction"))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsNarcoticAddicted()
	{
		foreach (StartingPerk startingPerk in PlayerStore.Instance.startingPerks)
		{
			if (startingPerk.id.Equals("narcotic_addiction"))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsGamblingAddicted()
	{
		foreach (StartingPerk startingPerk in PlayerStore.Instance.startingPerks)
		{
			if (startingPerk.id.Equals("gambling_addiction"))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsSentinel()
	{
		foreach (StartingPerk startingPerk in PlayerStore.Instance.startingPerks)
		{
			if (startingPerk.id.Equals("sentinel"))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CanRead()
	{
		return !StartingPerk.IsIlliterate();
	}

	public void InitStartingAddictionWithdrawal()
	{
		if (IsNicotineAddicted())
		{
			daysSinceNicotineConsumption = 3;
		}
		if (IsAlcoholAddicted())
		{
			daysSinceAlcoholConsumption = 3;
		}
		if (IsNarcoticAddicted())
		{
			daysSinceNarcoticConsumption = 3;
		}
	}

	public void OnSleep()
	{
		HandleNightlyWound();
		LiquidLuckHelper.OnSleep(this);
		if (IsNicotineAddicted())
		{
			daysSinceNicotineConsumption++;
			if (daysSinceNicotineConsumption >= 20)
			{
				StartingPerk.RemovePerk("nicotine_addiction");
				daysSinceNicotineConsumption = 0;
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_addiction_overcome_nicotine"), "5c6f46");
			}
			else if (HasNicotineDebuff())
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_addiction_withdrawal_nicotine"), "cc7755");
			}
		}
		if (IsAlcoholAddicted())
		{
			daysSinceAlcoholConsumption++;
			if (daysSinceAlcoholConsumption >= 20)
			{
				StartingPerk.RemovePerk("alcohol_addiction");
				daysSinceAlcoholConsumption = 0;
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_addiction_overcome_alcohol"), "5c6f46");
			}
			else if (HasAlcoholDebuff())
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_addiction_withdrawal_alcohol"), "cc7755");
			}
		}
		if (IsNarcoticAddicted())
		{
			daysSinceNarcoticConsumption++;
			if (daysSinceNarcoticRelief < 999)
			{
				daysSinceNarcoticRelief++;
			}
			if (daysSinceNarcoticConsumption >= 20)
			{
				StartingPerk.RemovePerk("narcotic_addiction");
				daysSinceNarcoticConsumption = 0;
				daysSinceNarcoticRelief = 999;
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_addiction_overcome_narcotic"), "5c6f46");
			}
			else if (HasNarcoticDebuff())
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_addiction_withdrawal_narcotic"), "cc7755");
			}
		}
		if (IsSentinel())
		{
			daysSinceSentinelInjection++;
			if (daysSinceSentinelInjection >= 14)
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_sentinel_critical"), "9c3738");
				sentinelPool += 3;
			}
			else if (daysSinceSentinelInjection >= 10)
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_sentinel_severe"), "9c3738");
				sentinelPool += 2;
			}
			else if (daysSinceSentinelInjection >= 7)
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_sentinel_moderate"), "9c3738");
				sentinelPool++;
			}
			else if (daysSinceSentinelInjection >= 6)
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_sentinel_mild"), "cc7755");
			}
			if (sentinelPool >= 20)
			{
				PlayerStore.Instance.ExecuteGameOver("sentinel");
			}
			else
			{
				AddSentinelDamageLog();
			}
		}
		if (LotteryHelper.IsLotteryUnlocked() && StoreStation.Instance.dayCounter % 7 == 5)
		{
			LotteryHelper.OnDrawWiningNumber();
		}
	}

	private void AddSentinelDamageLog()
	{
		if (sentinelPool <= 0)
		{
			return;
		}
		if (sentinelPool + NextOverdueDamage() >= 20)
		{
			PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanicOrDefault("health_sentinel_collapse_lethal", "Your body cannot take another missed injection. One more late injection will kill you."), "9c3738");
		}
		else if (daysSinceSentinelInjection >= 6)
		{
			if (sentinelPool >= 15)
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_sentinel_collapse_final"), "9c3738");
			}
			else if (sentinelPool >= 10)
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("health_sentinel_collapse_advanced"), "cc7755");
			}
			else
			{
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanicOrDefault("health_sentinel_collapse_light", "Your body carries some lasting damage from the disorder. It is not dangerous yet, but it will not go away."), "cc7755");
			}
		}
	}

	private int NextOverdueDamage()
	{
		int num = Mathf.Max(daysSinceSentinelInjection + 1, 7);
		if (num >= 14)
		{
			return 3;
		}
		if (num >= 10)
		{
			return 2;
		}
		return 1;
	}

	public void ConsumeNicotine(int value)
	{
		StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_suppressed"));
		daysSinceNicotineConsumption = -value;
		RefreshAddictionPenaltyFeatures();
	}

	public void ConsumeAlcohol(int value)
	{
		StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_suppressed"));
		daysSinceAlcoholConsumption = -value;
		RefreshAddictionPenaltyFeatures();
	}

	public void ConsumeNarcotic(int value)
	{
		StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_suppressed"));
		SetNarcoticCover(value);
	}

	public void ConsumeNarcotic(GameItem item)
	{
		int @int = item.GetTagReadonly("NARCOTIC_VALUE").GetInt();
		if (!ChemicalProductHelper.HasChemicalPurity(item))
		{
			ConsumeNarcotic(@int);
			return;
		}
		double purity = ChemicalProductHelper.GetPurity(item);
		if (purity < 30.0)
		{
			StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_suppressed_none"));
			return;
		}
		int narcoticPurityBonus = GetNarcoticPurityBonus(purity);
		if (narcoticPurityBonus > 0)
		{
			StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_suppressed_strong"));
		}
		else if (narcoticPurityBonus < 0)
		{
			StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_suppressed_weak"));
		}
		else
		{
			StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_suppressed"));
		}
		SetNarcoticCover(@int + narcoticPurityBonus);
	}

	private static int GetNarcoticPurityBonus(double purity)
	{
		if (purity >= 99.0)
		{
			return 3;
		}
		if (purity >= 90.0)
		{
			return 2;
		}
		if (purity >= 80.0)
		{
			return 1;
		}
		if (purity >= 60.0)
		{
			return 0;
		}
		if (purity >= 45.0)
		{
			return -1;
		}
		return -2;
	}

	public void ConsumeNarcoticRelief(int cover)
	{
		StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_suppressed"));
		SetNarcoticRelief(cover);
	}

	public void ConsumeNarcoticRelief(GameItem item, int cover)
	{
		if (!ChemicalProductHelper.HasChemicalPurity(item))
		{
			ConsumeNarcoticRelief(cover);
			return;
		}
		double purity = ChemicalProductHelper.GetPurity(item);
		if (purity < 30.0)
		{
			StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_suppressed_none"));
			return;
		}
		int narcoticPurityBonus = GetNarcoticPurityBonus(purity);
		if (narcoticPurityBonus > 0)
		{
			StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_suppressed_strong"));
		}
		else if (narcoticPurityBonus < 0)
		{
			StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_suppressed_weak"));
		}
		else
		{
			StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_suppressed"));
		}
		SetNarcoticRelief(cover + narcoticPurityBonus);
	}

	private void SetNarcoticRelief(int cover)
	{
		daysSinceNarcoticRelief = Mathf.Min(daysSinceNarcoticRelief, -cover);
		RefreshAddictionPenaltyFeatures();
	}

	private void SetNarcoticCover(int cover)
	{
		daysSinceNarcoticConsumption = Mathf.Min(daysSinceNarcoticConsumption, -cover);
		RefreshAddictionPenaltyFeatures();
	}

	public void RefreshAddictionPenaltyFeatures()
	{
		if (PlayerStore.Instance.currentClientInstance != null)
		{
			StoreClient.ClientIntent clientIntent = PlayerStore.Instance.currentClientInstance.GetClientBlueprint().clientIntent;
			RefreshAddictionPenaltyFeatures(EmporiumEntry.Instance.frontInvinvElement.childItems, clientIntent);
			RefreshAddictionPenaltyFeatures(EmporiumEntry.Instance.backInvinvElement.childItems, clientIntent);
		}
	}

	private void RefreshAddictionPenaltyFeatures(List<GameItem> items, StoreClient.ClientIntent intent)
	{
		bool flag = HasAnyAddiction() && GetTotalAddictionPenalty() != 0;
		foreach (GameItem item in items)
		{
			item.RemoveItemFeatureByID("addictionBuyPenalty");
			item.RemoveItemFeatureByID("addictionSellPenalty");
			if (flag)
			{
				if (!GeneralHelper.IsItemOwned(item) && (intent == StoreClient.ClientIntent.SELL || intent == StoreClient.ClientIntent.SELLNBUY))
				{
					item.AddItemFeature(ItemFeatureList.AddictionBuyPenalty());
				}
				if (GeneralHelper.IsItemOwned(item) && (intent == StoreClient.ClientIntent.BUY || intent == StoreClient.ClientIntent.SELLNBUY))
				{
					item.AddItemFeature(ItemFeatureList.AddictionSellPenalty());
				}
			}
		}
	}

	public void ConsumeImmunivax(GameItem gameItem)
	{
		ItemFeature itemFeature = gameItem.FindItemFeatureByID("CATEGORY_GENUINE_INJECTOR");
		if (itemFeature != null)
		{
			ItemCondition realCondition = itemFeature.realCondition;
			string text = ((daysSinceSentinelInjection >= 10) ? LocHelper.GetLocalizedMechanic("health_immunivax_feel_much_better") : ((daysSinceSentinelInjection >= 7) ? LocHelper.GetLocalizedMechanic("health_immunivax_feel_better") : ((daysSinceSentinelInjection < 2) ? LocHelper.GetLocalizedMechanic("health_immunivax_feel_none") : LocHelper.GetLocalizedMechanic("health_immunivax_feel_slightly"))));
			string text2;
			if (realCondition.identifier.Equals("genuineInjector"))
			{
				daysSinceSentinelInjection = 0;
				text2 = LocHelper.GetLocalizedMechanic("health_immunivax_smooth");
			}
			else if (realCondition.identifier.Equals("expiredInjector"))
			{
				daysSinceSentinelInjection = 3;
				text2 = LocHelper.GetLocalizedMechanic("health_immunivax_mostly_smooth");
			}
			else if (realCondition.identifier.Equals("counterfeitInjector"))
			{
				daysSinceSentinelInjection = RNG.GetRandomInt(3, 5);
				text2 = LocHelper.GetLocalizedMechanic("health_immunivax_off");
			}
			else
			{
				text2 = "";
			}
			StoreUIManager.Instance.Notify(text2 + text);
		}
	}

	public bool HasNicotineDebuff()
	{
		if (IsNicotineAddicted())
		{
			return daysSinceNicotineConsumption >= 3;
		}
		return false;
	}

	public bool HasAlcoholDebuff()
	{
		if (IsAlcoholAddicted())
		{
			return daysSinceAlcoholConsumption >= 3;
		}
		return false;
	}

	public bool HasNarcoticDebuff()
	{
		if (IsNarcoticAddicted())
		{
			return Mathf.Min(daysSinceNarcoticConsumption, daysSinceNarcoticRelief) >= 3;
		}
		return false;
	}

	public static bool CanUseSynaprest()
	{
		return HasAnyAddiction();
	}

	public static bool CanUseImmunivax()
	{
		return IsSentinel();
	}

	public static bool HasAnyAddiction()
	{
		if (!IsNicotineAddicted() && !IsAlcoholAddicted() && !IsNarcoticAddicted())
		{
			return IsGamblingAddicted();
		}
		return true;
	}

	public static bool CanUseAlcohol()
	{
		return IsAlcoholAddicted();
	}

	public static bool CanUseNarcotic()
	{
		return IsNarcoticAddicted();
	}

	public static bool CanUseNicotine()
	{
		return IsNicotineAddicted();
	}

	public List<string> GetAddictions()
	{
		List<string> list = new List<string>();
		if (IsNicotineAddicted())
		{
			list.Add("nicotine_addiction");
		}
		if (IsAlcoholAddicted())
		{
			list.Add("alcohol_addiction");
		}
		if (IsNarcoticAddicted())
		{
			list.Add("narcotic_addiction");
		}
		if (IsGamblingAddicted())
		{
			list.Add("gambling_addiction");
		}
		return list;
	}

	public void RemoveOneRandomAddiction()
	{
		List<string> addictions = GetAddictions();
		if (addictions.Count > 0)
		{
			string perkId = addictions[Random.Range(0, addictions.Count)];
			if (StartingPerk.GetPerkById(perkId) != null)
			{
				StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("health_addiction_cured", StartingPerk.GetPerkById(perkId).GetLocalizedDisplayName()));
			}
			StartingPerk.RemovePerk(perkId);
			RefreshAddictionPenaltyFeatures();
		}
	}

	public int GetTotalAddictionPenalty()
	{
		int num = 0;
		if (HasAlcoholDebuff())
		{
			num += 8;
		}
		if (HasNarcoticDebuff())
		{
			num += 8;
		}
		if (HasNicotineDebuff())
		{
			num += 8;
		}
		return num;
	}

	public void OnHealingItemUsed()
	{
		isWoundedFresh = false;
	}
}

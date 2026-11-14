using System;
using System.Collections.Generic;

public class SecData
{
	public const string WEAPON_TRAFFICKING = "WEAPON_TRAFFICKING";

	public const string DRUG_TRAFFICKING = "DRUG_TRAFFICKING";

	public const string CARD_TRAFFICKING = "CARD_TRAFFICKING";

	public const string CONTRABAND_TRAFFICKING = "CONTRABAND_TRAFFICKING";

	public const string BRIBERY = "BRIBERY";

	public const string FRAUD = "FRAUD";

	public const string ENDANGERMENT = "ENDANGERMENT";

	public const string FENCING = "FENCING";

	public const string OPERATING_WITHOUT_LICENSE = "OPERATING_WITHOUT_LICENSE";

	public const string MANSLAUGHTER = "MANSLAUGHTER";

	public const string ENDANGERMENT_ITEM_TAG = "ENDANGERMENT_ITEM_TAG";

	[ES3Serializable]
	public List<string> crimeTypes;

	[ES3Serializable]
	public List<int> crimeAmount;

	[ES3Serializable]
	public List<int> crimeAmountTotal;

	[ES3Serializable]
	public int evidenceLevel;

	[ES3Serializable]
	public int recommendedAction;

	[ES3Serializable]
	public string assignedCaseOfficer = "None";

	[ES3Serializable]
	public int dirtyWaterTracker;

	[ES3Serializable]
	public int mislabeledWaterTracker;

	[ES3Serializable]
	public int badChemTrackerFraud;

	[ES3Serializable]
	public int badChemTrackerEndangerment;

	[ES3Serializable]
	public int bribeTracker;

	[ES3Serializable]
	public int badAmmoTracker;

	[ES3Serializable]
	public int badAmmoTrackerLegal;

	[ES3Serializable]
	public int endangermentItemTracker;

	[ES3Serializable]
	public string note = "";

	[ES3Serializable]
	public int state;

	public SecData()
	{
		crimeTypes = new List<string>();
		crimeAmount = new List<int>();
		crimeAmountTotal = new List<int>();
	}

	public bool IsStringOperation()
	{
		return GetContrabandTotalCrimeValue() > 1000;
	}

	public bool IsReliableInspector()
	{
		return GetBriberyAmount() >= 500;
	}

	public bool IsFrequentInspection()
	{
		return GetTotalTraffickingCrimes() >= 1000;
	}

	public int GetContrabandTotalCrimeValue()
	{
		int num = 0;
		for (int i = 0; i < crimeTypes.Count; i++)
		{
			if (IsContrabandCrime(crimeTypes[i]))
			{
				num += crimeAmount[i];
			}
		}
		return num;
	}

	public bool IsContrabandCrime(string crime)
	{
		if (!crime.Equals("WEAPON_TRAFFICKING") && !crime.Equals("DRUG_TRAFFICKING") && !crime.Equals("CARD_TRAFFICKING"))
		{
			return crime.Equals("CONTRABAND_TRAFFICKING");
		}
		return true;
	}

	public void OnFixerUsed()
	{
		int wildFixerTier = NetworkUpgrade.GetWildFixerTier();
		float num = (float)NetworkUpgradeList.WILD_FIXER_PERCENT[wildFixerTier - 1] / 100f;
		int num2 = NetworkUpgradeList.WILD_FIXER_FLAT[wildFixerTier - 1];
		for (int i = 0; i < crimeAmount.Count; i++)
		{
			if (crimeAmount[i] > 0)
			{
				crimeAmount[i] -= (int)((float)crimeAmount[i] * num);
			}
			else
			{
				crimeAmount[i] = 0;
			}
			crimeAmount[i] -= num2;
			if (crimeAmount[i] < 0)
			{
				crimeAmount[i] = 0;
			}
		}
	}

	public void OnNewDay()
	{
		for (int i = 0; i < crimeAmount.Count; i++)
		{
			if (evidenceLevel < 80)
			{
				crimeAmount[i] -= 20;
			}
			if (crimeAmount[i] < 0)
			{
				crimeAmount[i] = 0;
			}
		}
		int totalCrimeValue = GetTotalCrimeValue();
		int num = 0;
		if (totalCrimeValue >= 100)
		{
			if (totalCrimeValue < 300)
			{
				num = 2;
			}
			else if (totalCrimeValue < 850)
			{
				num = 6;
			}
			else if (totalCrimeValue < 1200)
			{
				num = 9;
			}
			else if (totalCrimeValue < 1500)
			{
				num = 12;
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("sec_data_investigation_fast"), "cc7755");
			}
			else if (totalCrimeValue < 2500)
			{
				num = 20;
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("sec_data_investigation_very_fast"), "cc7755");
			}
			else
			{
				num = 33;
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("sec_data_investigation_very_fast"), "cc7755");
			}
		}
		if (StoreReputation.IsPerkUnlocked("SEC_TRUSTED"))
		{
			num /= 2;
		}
		if (StoreReputation.IsPerkUnlocked("SEC_PRIORITY"))
		{
			num = RNG.MultiplyAndRound(num, 1.5);
		}
		else if (StoreReputation.IsPerkUnlocked("SEC_WATCHLIST"))
		{
			num = RNG.MultiplyAndRound(num, 1.2000000476837158);
		}
		if (StoreReputation.IsPerkUnlocked("UL_PETITION"))
		{
			num = RNG.MultiplyAndRound(num, 1.5);
		}
		evidenceLevel += num;
		if (evidenceLevel >= 100)
		{
			evidenceLevel = 100;
			if (state != 0 && !IsOfficerPending())
			{
				Debug.Log("Sentence officer missing with an open case, re-arming dispatch");
				state = 0;
			}
			SendOfficer();
		}
	}

	public static bool IsSentenceOfficerId(string clientId)
	{
		if (clientId == null)
		{
			return false;
		}
		if (!clientId.Equals("securityOfficer") && !clientId.Equals("securityOfficerFine") && !clientId.Equals("securityOfficerArrest"))
		{
			return clientId.Equals("securityOfficerExecution");
		}
		return true;
	}

	public bool IsOfficerPending()
	{
		PlayerStore instance = PlayerStore.Instance;
		if (instance == null)
		{
			return false;
		}
		if (instance.futurStoreClientIdQueue != null)
		{
			foreach (string item in instance.futurStoreClientIdQueue)
			{
				if (IsSentenceOfficerId(item))
				{
					return true;
				}
			}
		}
		if (instance.storeClientManager != null && instance.storeClientManager.clientStack != null)
		{
			foreach (StoreClient item2 in instance.storeClientManager.clientStack)
			{
				if (item2 != null && IsSentenceOfficerId(item2.identifier))
				{
					return true;
				}
			}
		}
		if (instance.currentClientInstance != null && IsSentenceOfficerId(instance.currentClientInstance.GetClientBlueprint().identifier))
		{
			return true;
		}
		return false;
	}

	public void ResetCrime()
	{
		for (int i = 0; i < crimeAmount.Count; i++)
		{
			crimeAmount[i] = 0;
		}
		evidenceLevel = 0;
		state = 0;
	}

	public void SendOfficer()
	{
		if (state == 0 && GetTotalCrimeValue() > 0)
		{
			Debug.Log("Sending Officer");
			state = 1;
			PlayerStore.Instance.QueueFuturClient("securityOfficer", 0);
		}
	}

	public int GetTotalTraffickingCrimes()
	{
		int num = 0;
		for (int i = 0; i < crimeTypes.Count; i++)
		{
			if (crimeTypes[i].ToUpperInvariant().Contains("TRAFFICKING") || crimeTypes[i].Equals("FENCING"))
			{
				num += crimeAmount[i];
			}
		}
		return num;
	}

	public int GetCrimeCount()
	{
		int num = 0;
		for (int i = 0; i < crimeAmount.Count; i++)
		{
			if (crimeAmount[i] > 0)
			{
				num++;
			}
		}
		return num;
	}

	public string GetBiggestCrime()
	{
		if (crimeTypes == null || crimeAmount == null || crimeTypes.Count == 0 || crimeAmount.Count == 0)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_no_crimes");
		}
		List<(string, int)> list = new List<(string, int)>();
		for (int i = 0; i < crimeTypes.Count; i++)
		{
			list.Add((crimeTypes[i], crimeAmount[i]));
		}
		list.Sort(((string crime, int amount) a, (string crime, int amount) b) => b.amount.CompareTo(a.amount));
		int num = Math.Min(3, list.Count);
		List<string> list2 = new List<string>();
		for (int j = 0; j < num; j++)
		{
			list2.Add(GetCrimeString(list[j].Item1));
		}
		if (list2.Count == 1)
		{
			return list2[0];
		}
		if (list2.Count == 2)
		{
			return list2[0] + LocHelper.GetLocalizedMechanic("sec_data_and") + list2[1];
		}
		if (list2.Count == 3)
		{
			return list2[0] + LocHelper.GetLocalizedMechanic("sec_data_list_sep") + list2[1] + LocHelper.GetLocalizedMechanic("sec_data_and") + list2[2];
		}
		return "ERROR";
	}

	public int GetTotalCrimeValue()
	{
		int num = 0;
		foreach (int item in crimeAmount)
		{
			num += item;
		}
		return num;
	}

	public string GetEvidenceLevelDisplay()
	{
		if (GetTotalCrimeValue() < 100 || evidenceLevel < 0)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_not_applicable");
		}
		return evidenceLevel + "/100";
	}

	public string GetProjectedSentence()
	{
		int totalCrimeValue = GetTotalCrimeValue();
		if (totalCrimeValue < 100)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_not_applicable");
		}
		if (totalCrimeValue < 300)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_sentence_fine");
		}
		if (totalCrimeValue < 1500)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_sentence_incarceration");
		}
		return LocHelper.GetLocalizedMechanic("sec_data_sentence_execution");
	}

	public string GetAction()
	{
		List<string> list = new List<string>();
		if (IsReliableInspector())
		{
			list.Add(LocHelper.GetLocalizedMechanic("sec_data_action_reliable_inspector"));
		}
		if (IsFrequentInspection())
		{
			list.Add(LocHelper.GetLocalizedMechanic("sec_data_action_additional_inspection"));
		}
		if (IsStringOperation())
		{
			list.Add(LocHelper.GetLocalizedMechanic("sec_data_action_sting"));
		}
		return string.Join(",", list);
	}

	public int GetInspectorPersonality()
	{
		if (IsReliableInspector())
		{
			return 0;
		}
		return RNG.GetRandomInt(0, 2);
	}

	public int GetCrimeIndex(string crimeID)
	{
		for (int i = 0; i < crimeTypes.Count; i++)
		{
			if (crimeTypes[i].Equals(crimeID))
			{
				return i;
			}
		}
		return -1;
	}

	public int GetBriberyAmount()
	{
		for (int i = 0; i < crimeTypes.Count; i++)
		{
			if (crimeTypes[i].Equals("BRIBERY"))
			{
				return crimeAmount[i];
			}
		}
		return 0;
	}

	public string GetOperation()
	{
		if (GetTotalCrimeValue() < 100 || evidenceLevel < 0)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_none");
		}
		if (evidenceLevel < 90)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_operation_investigation");
		}
		if (evidenceLevel < 100)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_operation_planning");
		}
		return LocHelper.GetLocalizedMechanic("sec_data_operation_execute");
	}

	public string GetCaseOfficer(int totalCrimeValue)
	{
		if (totalCrimeValue <= 100)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_none");
		}
		if (totalCrimeValue < 300)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_officer_jeff");
		}
		if (totalCrimeValue < 850)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_officer_valentine");
		}
		if (totalCrimeValue < 1500)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_officer_huang");
		}
		if (totalCrimeValue < 2500)
		{
			return LocHelper.GetLocalizedMechanic("sec_data_officer_page");
		}
		return LocHelper.GetLocalizedMechanic("sec_data_officer_carlson");
	}

	public string GetSuspectedCrimeDisplay()
	{
		string text = "";
		for (int i = 0; i < crimeAmount.Count; i++)
		{
			if (crimeAmount[i] > 0)
			{
				text = text + GetCrimeString(crimeTypes[i]) + " (" + crimeAmount[i] + "<sprite name=\"c\" tint=1 >)\n";
			}
		}
		int totalCrimeValue = GetTotalCrimeValue();
		if (totalCrimeValue > 0)
		{
			text += LocHelper.GetLocalizedMechanic("sec_data_total_display", totalCrimeValue);
		}
		return text;
	}

	public string GetCrimeString(string crimeID)
	{
		if (crimeID.Equals("WEAPON_TRAFFICKING"))
		{
			return LocHelper.GetLocalizedMechanic("sec_data_crime_weapon");
		}
		if (crimeID.Equals("DRUG_TRAFFICKING"))
		{
			return LocHelper.GetLocalizedMechanic("sec_data_crime_drug");
		}
		if (crimeID.Equals("CARD_TRAFFICKING"))
		{
			return LocHelper.GetLocalizedMechanic("sec_data_crime_card");
		}
		if (crimeID.Equals("CONTRABAND_TRAFFICKING"))
		{
			return LocHelper.GetLocalizedMechanic("sec_data_crime_contraband");
		}
		if (crimeID.Equals("BRIBERY"))
		{
			return LocHelper.GetLocalizedMechanic("sec_data_crime_bribery");
		}
		if (crimeID.Equals("FRAUD"))
		{
			return LocHelper.GetLocalizedMechanic("sec_data_crime_fraud");
		}
		if (crimeID.Equals("ENDANGERMENT"))
		{
			return LocHelper.GetLocalizedMechanic("sec_data_crime_endangerment");
		}
		if (crimeID.Equals("FENCING"))
		{
			return LocHelper.GetLocalizedMechanic("sec_data_crime_fencing");
		}
		if (crimeID.Equals("OPERATING_WITHOUT_LICENSE"))
		{
			return LocHelper.GetLocalizedMechanic("sec_data_crime_no_license");
		}
		if (crimeID.Equals("MANSLAUGHTER"))
		{
			return LocHelper.GetLocalizedMechanic("sec_data_crime_manslaughter");
		}
		return LocHelper.GetLocalizedMechanic("sec_data_crime_other");
	}

	public void CommitCrime(string crimeID, int amount)
	{
		if (StartingPerk.IsIntimidating())
		{
			amount = RNG.MultiplyAndRound(amount, 0.6699999570846558);
		}
		if (StartingPerk.IsBlabbermouth())
		{
			amount = RNG.MultiplyAndRound(amount, 1.3300000429153442);
		}
		int num = crimeTypes.IndexOf(crimeID);
		if (num != -1)
		{
			crimeAmount[num] += amount;
			crimeAmountTotal[num] += amount;
		}
		else
		{
			crimeTypes.Add(crimeID);
			crimeAmount.Add(amount);
			crimeAmountTotal.Add(amount);
		}
	}

	public void CommitBribery(int amount)
	{
		bribeTracker += amount;
	}

	public void SellDirtyWater(int volumeML)
	{
		dirtyWaterTracker += volumeML;
	}

	public void SellMislabeledWater(int volumeML)
	{
		mislabeledWaterTracker += volumeML;
		Debug.Log(mislabeledWaterTracker + " ml of mislabeled water sold tracked.");
	}

	public void SellBadAmmo(int value, bool isIllegalDeal = false)
	{
		if (!isIllegalDeal)
		{
			badAmmoTrackerLegal += value;
		}
		badAmmoTracker += value;
	}

	public void CheckSoldContraband(GameItem gameItem, StoreClient client)
	{
		if (client != null && client.isNoContributeToEvidence)
		{
			return;
		}
		if (gameItem.IsTag("STOLEN_TAG"))
		{
			int @int = gameItem.GetTagReadonly("STOLEN_VALUE_INT").GetInt();
			int a = (int)gameItem.GetNegociatedValue();
			a = RNG.MultiplyAndRound(a, (float)@int / 100f);
			a = ApplyCrimeModifier(client, a);
			CommitCrime("FENCING", a);
		}
		if (gameItem.IsTag("CONTRABAND_ITEM_TAG"))
		{
			int num = (int)gameItem.GetNegociatedValue();
			if (client != null)
			{
				num = ApplyCrimeModifier(client, num);
			}
			if (gameItem.IsGameItemType("SUBSTANCE"))
			{
				CommitCrime("DRUG_TRAFFICKING", num);
			}
			else if (gameItem.IsGameItemType("WEAPON"))
			{
				CommitCrime("WEAPON_TRAFFICKING", num);
			}
			else if (gameItem.IsGameItemType("ACCESS_CARD"))
			{
				CommitCrime("CARD_TRAFFICKING", num);
			}
			else
			{
				CommitCrime("CONTRABAND_TRAFFICKING", num);
			}
		}
	}

	public void CheckSoldEndangermentItem(GameItem gameItem, StoreClient client)
	{
		if ((client == null || !client.isNoContributeToEvidence) && gameItem.IsTag("ENDANGERMENT_ITEM_TAG"))
		{
			endangermentItemTracker += (int)gameItem.GetNegociatedValue();
		}
	}

	public void CheckDeliveredStolen(GameItem gameItem, int value, string factionId)
	{
		if (gameItem.IsTag("STOLEN_TAG"))
		{
			int @int = gameItem.GetTagReadonly("STOLEN_VALUE_INT").GetInt();
			value = RNG.MultiplyAndRound(value, (float)@int / 100f);
			value = ApplyCrimeModifier(factionId, value);
			CommitCrime("FENCING", value);
		}
	}

	public int ApplyCrimeModifier(StoreClient client, int value)
	{
		if (client == null)
		{
			return value;
		}
		return ApplyCrimeModifier(client.clientFaction, value);
	}

	public int ApplyCrimeModifier(string factionId, int value)
	{
		switch (factionId)
		{
		case "FACTION_BLACK_MARKET":
			return value / 2;
		case "FACTION_REVOLUTION":
			return value / 3;
		case "FACTION_LOWER_LEVEL":
			if (StoreReputation.IsPerkUnlocked("LLTIGHTLIPS"))
			{
				return value / 2;
			}
			break;
		}
		if (factionId == "FACTION_CARTEL")
		{
			return 0;
		}
		return value;
	}

	public void CheckSoldDirtyWater(GameItem gameItem, StoreClient client)
	{
		if (!gameItem.IsTag("LIQUID_CONTAINER_TAG") || WaterHelper.IsEmpty(gameItem))
		{
			return;
		}
		ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(gameItem);
		if (waterFeature == null || (client != null && client.IsFaction("FACTION_TOURIST")))
		{
			return;
		}
		bool isGhostwater;
		float mislabeledWaterWeight = GetMislabeledWaterWeight(waterFeature, out isGhostwater);
		if (!(mislabeledWaterWeight <= 0f))
		{
			Debug.Log("Sold unexposed mislabeled/dangerous water! " + waterFeature.GetBaseRealDisplay() + " labelled " + waterFeature.GetBaseFakeDisplay());
			int volumeML = (int)((float)WaterHelper.GetCurrentCapacityML(gameItem) * mislabeledWaterWeight);
			if (isGhostwater)
			{
				SellDirtyWater(volumeML);
			}
			else
			{
				SellMislabeledWater(volumeML);
			}
		}
	}

	public static float GetMislabeledWaterWeight(ItemFeature itemFeature, out bool isGhostwater)
	{
		isGhostwater = false;
		if (itemFeature == null || itemFeature.IsExposed() || itemFeature.IsFeatureMatch())
		{
			return 0f;
		}
		string baseRealDisplay = itemFeature.GetBaseRealDisplay();
		string baseFakeDisplay = itemFeature.GetBaseFakeDisplay();
		if (baseRealDisplay.Equals(WaterFeatureHelper.GHOSTWATER_DISPLAY))
		{
			isGhostwater = true;
			if (baseFakeDisplay.Equals(WaterFeatureHelper.BASE_WATER_DISPLAY))
			{
				return 1f;
			}
			if (baseFakeDisplay.Equals(WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY))
			{
				return 2f;
			}
			if (baseFakeDisplay.Equals(WaterFeatureHelper.PURE_WATER_DISPLAY))
			{
				return 4f;
			}
			return 0f;
		}
		if (baseRealDisplay.Equals(WaterFeatureHelper.GUTTERFLOW_WATER_DISPLAY) && baseFakeDisplay.Equals(WaterFeatureHelper.RUST_WATER_DISPLAY))
		{
			return 0.25f;
		}
		if (baseRealDisplay.Equals(WaterFeatureHelper.BASE_WATER_DISPLAY))
		{
			if (baseFakeDisplay.Equals(WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY))
			{
				return 1f;
			}
			if (baseFakeDisplay.Equals(WaterFeatureHelper.PURE_WATER_DISPLAY))
			{
				return 2f;
			}
		}
		if (baseRealDisplay.Equals(WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY) && baseFakeDisplay.Equals(WaterFeatureHelper.PURE_WATER_DISPLAY))
		{
			return 1.5f;
		}
		return 0f;
	}

	public void CheckSoldChem(GameItem gameItem, SoldItem soldItem)
	{
		int num = ChemicalFeedbackHelper.GetItemFeedbackLevel(gameItem);
		if (ChemicalFeedbackHelper.IsNarcotic(gameItem))
		{
			num -= ChemicalFeedbackHelper.GetEuphoriaLevel(gameItem);
		}
		if (num > 0)
		{
			if (num <= 3)
			{
				badChemTrackerFraud += (int)gameItem.GetNegociatedValue();
			}
			else
			{
				badChemTrackerEndangerment += (int)gameItem.GetNegociatedValue() * 2;
			}
		}
	}

	public int GetFineValue()
	{
		return GetTotalCrimeValue();
	}
}

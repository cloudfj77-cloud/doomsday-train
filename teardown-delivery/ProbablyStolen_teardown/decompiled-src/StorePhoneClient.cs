using System.Collections.Generic;

public class StorePhoneClient
{
	public enum PhoneState
	{
		None,
		NotResponding,
		Responding,
		Introduction,
		Regular,
		Cooldown
	}

	public enum PhoneClientType
	{
		None,
		Supplier
	}

	public const long WILD_CLIENT_NUMBER = 8376L;

	public const long RED_IMP_REP_NUMBER = 8815L;

	public const long MINING_NUMBER = 56371L;

	public const long PHARMA_NUMBER = 4615L;

	public const long CHEM_NUMBER = 3319L;

	public const long REV_MERCHANT_NUMBER = 51189L;

	[ES3Serializable]
	public PhoneClientType phoneClientType;

	[ES3Serializable]
	public PhoneState phoneState;

	[ES3Serializable]
	public string displayName;

	[ES3Serializable]
	public string locID;

	[ES3Serializable]
	public string dialogFuncId = "";

	[ES3Serializable]
	public int cooldownDuration;

	[ES3Serializable]
	public int currentCooldown;

	[ES3Serializable]
	public bool dialedBefore;

	[ES3Serializable]
	public List<string> alias;

	[ES3Serializable]
	public int amountSpent;

	[ES3Serializable]
	public int stockType;

	public StorePhoneClient()
	{
		phoneState = PhoneState.NotResponding;
		alias = new List<string>();
	}

	public string GetLocalizedDisplayName()
	{
		return LocHelper.GetLocalizedClientName(locID);
	}

	public Dialogue GetCallDialog()
	{
		if (PhoneClientList.DialogFuncs.TryGetValue(dialogFuncId, out var value))
		{
			return value(this);
		}
		return PhoneDialogList.BugDialog();
	}

	public static bool IsNumberExist(long number)
	{
		if (PlayerStore.Instance.PhoneClientDict.ContainsKey(number))
		{
			return true;
		}
		return false;
	}

	public static bool IsPhoneClientAtStore(long number)
	{
		if (PlayerStore.Instance.currentClientInstance == null)
		{
			return false;
		}
		if (PlayerStore.Instance.PhoneClientDict.ContainsKey(number) && PlayerStore.Instance.PhoneClientDict[number].alias.Contains(PlayerStore.Instance.currentClientInstance.GetClientBlueprint().identifier))
		{
			return true;
		}
		return false;
	}

	public void UseService()
	{
		phoneState = PhoneState.Cooldown;
		currentCooldown = cooldownDuration;
	}

	public bool ShownInPhoneBook()
	{
		if (phoneState == PhoneState.Cooldown || phoneState == PhoneState.Regular)
		{
			if (dialedBefore)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public void OnNewDay()
	{
		if (currentCooldown > 0 && phoneState == PhoneState.Cooldown)
		{
			currentCooldown--;
		}
		if (currentCooldown <= 0 && phoneState == PhoneState.Cooldown)
		{
			NightLog.ContactReady(GetLocalizedDisplayName());
			phoneState = PhoneState.Regular;
		}
	}

	public static Dictionary<long, StorePhoneClient> InitPhoneClientDict()
	{
		return new Dictionary<long, StorePhoneClient>
		{
			{
				8376L,
				PhoneClientList.CreateWildClient()
			},
			{
				8815L,
				PhoneClientList.CreateRedImpRep()
			},
			{
				56371L,
				PhoneClientList.CreateMining()
			},
			{
				4615L,
				PhoneClientList.CreatePharma()
			},
			{
				3319L,
				PhoneClientList.CreateChem()
			},
			{
				51189L,
				PhoneClientList.CreateRevMerchant()
			}
		};
	}

	public static void OnEndDay()
	{
		foreach (KeyValuePair<long, StorePhoneClient> item in PlayerStore.Instance.PhoneClientDict)
		{
			item.Value.OnNewDay();
		}
	}

	public static StorePhoneClient GetWildPhoneClient()
	{
		return PlayerStore.Instance.PhoneClientDict[8376L];
	}

	public static StorePhoneClient GetBrewerPhoneClient()
	{
		return PlayerStore.Instance.PhoneClientDict[8815L];
	}

	public static StorePhoneClient GetMiningPhoneClient()
	{
		return PlayerStore.Instance.PhoneClientDict[56371L];
	}

	public static StorePhoneClient GetPharmaPhoneClient()
	{
		return PlayerStore.Instance.PhoneClientDict[4615L];
	}

	public static StorePhoneClient GetChemPhoneClient()
	{
		return PlayerStore.Instance.PhoneClientDict[3319L];
	}

	public static StorePhoneClient GetRevMerchantClient()
	{
		return PlayerStore.Instance.PhoneClientDict[51189L];
	}

	public static StorePhoneClient GetPhoneClientByNumber(long number)
	{
		if (PlayerStore.Instance.PhoneClientDict.ContainsKey(number))
		{
			return PlayerStore.Instance.PhoneClientDict[number];
		}
		return null;
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class PhoneClientList : MonoBehaviour
{
	[ES3NonSerializable]
	public static Dictionary<string, Func<StorePhoneClient, Dialogue>> DialogFuncs = new Dictionary<string, Func<StorePhoneClient, Dialogue>>
	{
		{
			"Test",
			(StorePhoneClient _) => PhoneDialogList.PhoneDialogTest()
		},
		{
			"Sec",
			(StorePhoneClient _) => PhoneDialogList.PhoneDialogSec()
		},
		{
			"Wild",
			(StorePhoneClient _) => PhoneDialogList.PhoneWild()
		},
		{
			"RedImp",
			(StorePhoneClient client) => client.phoneState switch
			{
				StorePhoneClient.PhoneState.Introduction => PhoneDialogList.RedImpRepIntro(), 
				StorePhoneClient.PhoneState.Regular => PhoneDialogList.RedImpRepRegular(), 
				StorePhoneClient.PhoneState.Cooldown => PhoneDialogList.RedImpRepCooldown(), 
				_ => PhoneDialogList.BugDialog(), 
			}
		},
		{
			"Pharma",
			(StorePhoneClient client) => client.phoneState switch
			{
				StorePhoneClient.PhoneState.Introduction => PhoneDialogList.PharmaIntro(), 
				StorePhoneClient.PhoneState.Regular => PhoneDialogList.PharmaRegular(), 
				StorePhoneClient.PhoneState.Cooldown => PhoneDialogList.PharmaCooldown(), 
				_ => PhoneDialogList.BugDialog(), 
			}
		},
		{
			"Mining",
			(StorePhoneClient client) => client.phoneState switch
			{
				StorePhoneClient.PhoneState.Introduction => PhoneDialogList.MiningIntro(), 
				StorePhoneClient.PhoneState.Regular => PhoneDialogList.MiningRegular(), 
				StorePhoneClient.PhoneState.Cooldown => PhoneDialogList.MiningCooldown(), 
				_ => PhoneDialogList.BugDialog(), 
			}
		},
		{
			"Bug",
			(StorePhoneClient _) => PhoneDialogList.BugDialog()
		},
		{
			"Chem",
			(StorePhoneClient client) => client.phoneState switch
			{
				StorePhoneClient.PhoneState.Introduction => PhoneDialogList.ChemistIntro(), 
				StorePhoneClient.PhoneState.Regular => PhoneDialogList.ChemistRegular(), 
				StorePhoneClient.PhoneState.Cooldown => PhoneDialogList.ChemistCooldown(), 
				_ => PhoneDialogList.BugDialog(), 
			}
		},
		{
			"Commissary",
			(StorePhoneClient _) => PhoneDialogList.CommissaryPhoneDialog()
		},
		{
			"RevMerchant",
			(StorePhoneClient client) => (client.phoneState == StorePhoneClient.PhoneState.Cooldown) ? PhoneDialogList.GenericCooldown(client.GetLocalizedDisplayName()) : PhoneDialogList.RevMerchantPhoneDialog()
		}
	};

	public static StorePhoneClient CreateTestClient()
	{
		return new StorePhoneClient
		{
			displayName = "Unknown",
			dialogFuncId = "Test"
		};
	}

	public static StorePhoneClient CreateSecPhoneClient()
	{
		return new StorePhoneClient
		{
			displayName = "Security Phone Operator",
			phoneState = StorePhoneClient.PhoneState.Regular,
			dialogFuncId = "Sec"
		};
	}

	public static StorePhoneClient CreateCommissaryPhone()
	{
		return new StorePhoneClient
		{
			displayName = "Commissary Automated Phone System",
			cooldownDuration = 0,
			phoneState = StorePhoneClient.PhoneState.Regular,
			dialogFuncId = "Commissary"
		};
	}

	public static StorePhoneClient CreateWildClient()
	{
		return new StorePhoneClient
		{
			displayName = "Joe Wild's Assistant",
			dialogFuncId = "Wild",
			locID = "name_joe_assistant"
		};
	}

	public static StorePhoneClient CreateRedImpRep()
	{
		return new StorePhoneClient
		{
			displayName = "Red Imp Rep",
			cooldownDuration = 4,
			dialogFuncId = "RedImp",
			phoneClientType = StorePhoneClient.PhoneClientType.Supplier,
			locID = "name_red_imp_rep"
		};
	}

	public static StorePhoneClient CreatePharma()
	{
		return new StorePhoneClient
		{
			displayName = "Dale",
			cooldownDuration = 7,
			dialogFuncId = "Pharma",
			phoneClientType = StorePhoneClient.PhoneClientType.Supplier,
			locID = "name_dale"
		};
	}

	public static StorePhoneClient CreateMining()
	{
		return new StorePhoneClient
		{
			displayName = "Caulson",
			cooldownDuration = 5,
			dialogFuncId = "Mining",
			phoneClientType = StorePhoneClient.PhoneClientType.Supplier,
			locID = "name_caulson"
		};
	}

	public static StorePhoneClient CreateChem()
	{
		return new StorePhoneClient
		{
			displayName = "Ashish",
			cooldownDuration = 4,
			dialogFuncId = "Chem",
			phoneClientType = StorePhoneClient.PhoneClientType.Supplier,
			locID = "name_ashish"
		};
	}

	public static StorePhoneClient CreateRevMerchant()
	{
		return new StorePhoneClient
		{
			displayName = "Odin Haraldsson",
			cooldownDuration = 5,
			phoneState = StorePhoneClient.PhoneState.NotResponding,
			dialogFuncId = "RevMerchant",
			alias = new List<string> { "wanted4", "wanted4Normal" },
			locID = "name_odin"
		};
	}
}

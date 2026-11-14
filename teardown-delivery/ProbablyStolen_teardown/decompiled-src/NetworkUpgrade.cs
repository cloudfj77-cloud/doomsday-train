using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkUpgrade
{
	[ES3Serializable]
	public string subtitle;

	[ES3Serializable]
	public int cost;

	[ES3Serializable]
	public string id;

	[ES3Serializable]
	public bool isRepeatable;

	[ES3Serializable]
	public int cooldownCurrent;

	[ES3Serializable]
	public int cooldownDuration;

	[ES3Serializable]
	public List<string> prerequisite;

	[ES3Serializable]
	public bool lockInDemo;

	[ES3Serializable]
	public int state;

	[ES3NonSerializable]
	public string tab = "";

	[ES3NonSerializable]
	public string iconKey;

	[ES3NonSerializable]
	public string source;

	[ES3NonSerializable]
	public bool recommended;

	[ES3NonSerializable]
	public bool hidden;

	[ES3NonSerializable]
	public bool heldUntilUsed;

	[ES3NonSerializable]
	public Func<string> titleProvider;

	[ES3NonSerializable]
	public Func<string> subtitleProvider;

	[ES3NonSerializable]
	public Func<string> descriptionProvider;

	[ES3NonSerializable]
	public Func<string> alreadyBoughtProvider;

	public const string DEFAULT_TAB = "";

	private static readonly List<NetworkUpgrade> modDefinitions = new List<NetworkUpgrade>();

	private static readonly Dictionary<string, Func<string>> tabLabels = new Dictionary<string, Func<string>>();

	private static Dictionary<string, NetworkUpgrade> definitions;

	private static List<NetworkUpgrade> definitionOrder;

	public const string BREWER = "BREWER";

	public const string CHEMIST = "CHEMIST";

	public const string PHARMA = "PHARMA";

	public const string MINER = "MINER";

	public const string BREWER_SHAREHOLDER = "BREWER_SHAREHOLDER";

	public const string MINER_SHAREHOLDER = "MINER_SHAREHOLDER";

	public const string CHEMIST_SHAREHOLDER = "CHEMIST_SHAREHOLDER";

	public const string PHARMA_SHAREHOLDER = "PHARMA_SHAREHOLDER";

	public const string LANDLORD_GRACE = "LANDLORD_GRACE";

	public const string SEC_GRACE = "SEC_GRACE";

	public const string INSPECTION_INFORMANT = "INSPECTION_INFORMANT";

	public const string BUY_REFERAL = "BUY_REFERAL";

	public const string SELL_REFERAL = "SELL_REFERAL";

	public const string CRIMINEL_NETWORK = "CRIMINEL_NETWORK";

	public const string POWER_HIJACK = "POWER_HIJACK";

	public const string POWER_HIJACK2 = "POWER_HIJACK2";

	public const string POWER_HIJACK3 = "POWER_HIJACK3";

	public const string COMMERCIAL_POWER = "COMMERCIAL_POWER";

	public const string SHOWCASE_I = "SHOWCASE_I";

	public const string SHOWCASE_II = "SHOWCASE_II";

	public const string WILD_FIXER = "WILD_FIXER";

	public const string WILD_FIXER2 = "WILD_FIXER2";

	public const string WILD_FIXER3 = "WILD_FIXER3";

	public const string EVIDENCE_REMOVAL = "EVIDENCE_REMOVAL";

	public const string GUTTERFLOW_TAP = "GUTTERFLOW_TAP";

	public const string RUSTWATER_TAP = "RUSTWATER_TAP";

	public const string RUINED_MACHINE_UNLOCK = "RUINED_MACHINE_UNLOCK";

	public const string RETIRED_FARMER = "RETIRED_FARMER";

	public const string RETIRED_GUNSMITH = "RETIRED_GUNSMITH";

	public const string RETIRED_CHEMIST = "RETIRED_CHEMIST";

	public const string RETIRED_RANCHER = "RETIRED_RANCHER";

	public const string WATER_TRADER = "WATER_TRADER";

	public const string GUN_PERMIT_I = "GUN_PERMIT_I";

	public const string GUN_PERMIT_II = "GUN_PERMIT_II";

	public const string GUN_PERMIT_III = "GUN_PERMIT_III";

	public const string MARKETPLACE_REQUESTS = "MARKETPLACE_REQUESTS";

	public const string JACKSON1 = "JACKSON1";

	public const string JACKSON2 = "JACKSON2";

	public const string JACKSON_SPECIAL_STOCK = "JACKSON_SPECIAL_STOCK";

	public const string RENOVATION1 = "RENOVATION1";

	public const string RENOVATION2 = "RENOVATION2";

	public const string RENOVATION3 = "RENOVATION3";

	public const string VENDING_MACHINE = "VENDING_MACHINE";

	public const string VENDING_FOUNTAIN = "VENDING_FOUNTAIN";

	public const string WINEMAKER = "WINEMAKER";

	public const string AQUACULTURE = "AQUACULTURE";

	public const string AQUACULTURE_II = "AQUACULTURE_II";

	public const string PLANT_DNA = "PLANT_DNA";

	public const string DOSSIER_UPGRADE = "DOSSIER_UPGRADE";

	public const string TRASHCAN_UPGRADE = "TRASHCAN_UPGRADE";

	public const string HYPERTECH_SMUGGLER = "HYPERTECH_SMUGGLER";

	public const string ENDORSEMENT1 = "ENDORSEMENT1";

	public const string ENDORSEMENT2 = "ENDORSEMENT2";

	public const string ENDORSEMENT3 = "ENDORSEMENT3";

	private static HashSet<string> demoLockedIds;

	private static readonly Dictionary<string, Func<string>> subtitleFromOtherTable = new Dictionary<string, Func<string>>
	{
		{
			"Gunsmith",
			() => LocHelper.GetLocalizedNewGame("gunsmith")
		},
		{
			"Water",
			() => LocHelper.GetLocalizedNewGame("water_merchant")
		},
		{
			"Hydroponic",
			() => LocHelper.GetLocalizedNewGame("hydroponic_farmer")
		},
		{
			"Aquaculture",
			() => LocHelper.GetLocalizedNetworkUpgrade("network_AQUACULTURE_title")
		}
	};

	private static readonly object[] NoArgs = new object[0];

	private static readonly object[] ArgsEvidenceRemoval = new object[1] { 14 };

	private static readonly object[] ArgsCriminelNetwork = new object[1] { 3 };

	private static readonly object[] ArgsCommercialPower = new object[1] { 100 };

	private static readonly object[] ArgsShowcase = new object[1] { 1 };

	private static readonly object[] ArgsGunPermitI = new object[1] { 20 };

	private static readonly object[] ArgsGunPermitII = new object[1] { 50 };

	private static readonly object[] ArgsGunPermitIII = new object[1] { 80 };

	private static readonly object[] ArgsRenovation = new object[1] { 150 };

	private static readonly object[] ArgsBrewerShareholder = new object[2] { 15, 1 };

	private static readonly object[] ArgsMinerShareholder = new object[2] { 15, 6 };

	private static readonly object[] ArgsChemistShareholder = new object[2] { 15, 8 };

	private static readonly object[] ArgsPharmaShareholder = new object[2] { 15, 5 };

	private static readonly HashSet<string> unknownPrerequisiteWarned = new HashSet<string>();

	public NetworkUpgrade(string id)
	{
		this.id = id;
		prerequisite = new List<string>();
	}

	public NetworkUpgrade CopyForRuntime()
	{
		NetworkUpgrade networkUpgrade = new NetworkUpgrade(id)
		{
			subtitle = subtitle,
			cost = cost,
			isRepeatable = isRepeatable,
			cooldownCurrent = cooldownCurrent,
			cooldownDuration = cooldownDuration,
			lockInDemo = lockInDemo,
			state = state,
			tab = tab,
			iconKey = iconKey,
			source = source,
			recommended = recommended,
			hidden = hidden,
			heldUntilUsed = heldUntilUsed,
			titleProvider = titleProvider,
			subtitleProvider = subtitleProvider,
			descriptionProvider = descriptionProvider,
			alreadyBoughtProvider = alreadyBoughtProvider
		};
		if (prerequisite != null)
		{
			networkUpgrade.prerequisite.AddRange(prerequisite);
		}
		return networkUpgrade;
	}

	public static void RegisterModUpgrade(NetworkUpgrade definition, Action onUnlock)
	{
		if (definition == null || string.IsNullOrEmpty(definition.id))
		{
			UnityEngine.Debug.LogError("[NetworkUpgrade] RegisterModUpgrade needs a definition with an id");
			return;
		}
		modDefinitions.RemoveAll((NetworkUpgrade d) => d.id == definition.id);
		modDefinitions.Add(definition);
		if (onUnlock != null)
		{
			NetworkUpgradeList.UnlockActions[definition.id] = onUnlock;
		}
		else
		{
			NetworkUpgradeList.UnlockActions.Remove(definition.id);
		}
		InvalidateDefinitions();
		if (PlayerStore.Instance != null && PlayerStore.Instance.networkUpgrade != null && !PlayerStore.Instance.networkUpgrade.ContainsKey(definition.id))
		{
			PlayerStore.Instance.networkUpgrade.Add(definition.id, definition.CopyForRuntime());
		}
	}

	public static int RemoveModUpgrades(string source)
	{
		int num = 0;
		for (int num2 = modDefinitions.Count - 1; num2 >= 0; num2--)
		{
			if (!(modDefinitions[num2].source != source))
			{
				NetworkUpgradeList.UnlockActions.Remove(modDefinitions[num2].id);
				modDefinitions.RemoveAt(num2);
				num++;
			}
		}
		if (num > 0)
		{
			InvalidateDefinitions();
		}
		return num;
	}

	public static void RegisterTabLabel(string tab, Func<string> label)
	{
		if (!string.IsNullOrEmpty(tab))
		{
			if (label == null)
			{
				tabLabels.Remove(tab);
			}
			else
			{
				tabLabels[tab] = label;
			}
		}
	}

	public static void InvalidateDefinitions()
	{
		definitions = null;
		definitionOrder = null;
		demoLockedIds = null;
	}

	private static void EnsureDefinitions()
	{
		if (definitions == null)
		{
			definitions = InitUpgradeDict();
			definitionOrder = new List<NetworkUpgrade>(definitions.Values);
		}
	}

	public static NetworkUpgrade GetDefinition(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		EnsureDefinitions();
		if (!definitions.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public static List<NetworkUpgrade> GetDefinitionsInOrder()
	{
		EnsureDefinitions();
		return new List<NetworkUpgrade>(definitionOrder);
	}

	public static string GetTabLabel(string tab)
	{
		if (string.IsNullOrEmpty(tab))
		{
			return LocHelper.GetLocalizedUI("network_ui_tab_basic");
		}
		string key = "network_ui_tab_" + tab;
		if (LocHelper.HasLocalizedUI(key))
		{
			return LocHelper.GetLocalizedUI(key);
		}
		if (tabLabels.TryGetValue(tab, out var value))
		{
			return SafeText(value, tab);
		}
		return tab;
	}

	private static string SafeText(Func<string> provider, string fallback)
	{
		try
		{
			return provider() ?? fallback;
		}
		catch (Exception arg)
		{
			UnityEngine.Debug.LogError($"[NetworkUpgrade] Text provider threw: {arg}");
			return fallback;
		}
	}

	public static Dictionary<string, NetworkUpgrade> InitUpgradeDict()
	{
		Dictionary<string, NetworkUpgrade> dictionary = new Dictionary<string, NetworkUpgrade>
		{
			{
				"BREWER",
				NetworkUpgradeList.CreateBrewer()
			},
			{
				"CHEMIST",
				NetworkUpgradeList.CreateChemist()
			},
			{
				"PHARMA",
				NetworkUpgradeList.CreatePharma()
			},
			{
				"MINER",
				NetworkUpgradeList.CreateMiner()
			},
			{
				"BREWER_SHAREHOLDER",
				NetworkUpgradeList.CreateBrewerShareholder()
			},
			{
				"MINER_SHAREHOLDER",
				NetworkUpgradeList.CreateMinerShareholder()
			},
			{
				"CHEMIST_SHAREHOLDER",
				NetworkUpgradeList.CreateChemistShareholder()
			},
			{
				"PHARMA_SHAREHOLDER",
				NetworkUpgradeList.CreatePharmaShareholder()
			},
			{
				"LANDLORD_GRACE",
				NetworkUpgradeList.CreateLandlordGrace()
			},
			{
				"SEC_GRACE",
				NetworkUpgradeList.CreatePrisonGrace()
			},
			{
				"INSPECTION_INFORMANT",
				NetworkUpgradeList.CreateInspectionInformation()
			},
			{
				"WILD_FIXER",
				NetworkUpgradeList.CreateFixer()
			},
			{
				"WILD_FIXER2",
				NetworkUpgradeList.CreateFixer2()
			},
			{
				"WILD_FIXER3",
				NetworkUpgradeList.CreateFixer3()
			},
			{
				"EVIDENCE_REMOVAL",
				NetworkUpgradeList.CreateEvidenceRemoval()
			},
			{
				"BUY_REFERAL",
				NetworkUpgradeList.CreateBuyReferal()
			},
			{
				"SELL_REFERAL",
				NetworkUpgradeList.CreateSellReferal()
			},
			{
				"CRIMINEL_NETWORK",
				NetworkUpgradeList.CreateCriminelNetwork()
			},
			{
				"POWER_HIJACK",
				NetworkUpgradeList.CreatePowerHijack()
			},
			{
				"POWER_HIJACK2",
				NetworkUpgradeList.CreatePowerHijack2()
			},
			{
				"POWER_HIJACK3",
				NetworkUpgradeList.CreatePowerHijack3()
			},
			{
				"COMMERCIAL_POWER",
				NetworkUpgradeList.CreateCommercialPower()
			},
			{
				"SHOWCASE_I",
				NetworkUpgradeList.CreateShowcaseI()
			},
			{
				"SHOWCASE_II",
				NetworkUpgradeList.CreateShowcaseII()
			},
			{
				"GUTTERFLOW_TAP",
				NetworkUpgradeList.CreateGutterflowTap()
			},
			{
				"RUSTWATER_TAP",
				NetworkUpgradeList.CreateRustwaterTap()
			},
			{
				"RUINED_MACHINE_UNLOCK",
				NetworkUpgradeList.CreateRuinedMachineUnlock()
			},
			{
				"RETIRED_FARMER",
				NetworkUpgradeList.CreateRetiredFarmerUnlock()
			},
			{
				"RETIRED_GUNSMITH",
				NetworkUpgradeList.CreateRetiredGunsmithUnlock()
			},
			{
				"GUN_PERMIT_I",
				NetworkUpgradeList.CreateGunPermitI()
			},
			{
				"GUN_PERMIT_II",
				NetworkUpgradeList.CreateGunPermitII()
			},
			{
				"GUN_PERMIT_III",
				NetworkUpgradeList.CreateGunPermitIII()
			},
			{
				"RETIRED_CHEMIST",
				NetworkUpgradeList.CreateRetiredChemistUnlock()
			},
			{
				"RETIRED_RANCHER",
				NetworkUpgradeList.CreateRetiredRancherUnlock()
			},
			{
				"WATER_TRADER",
				NetworkUpgradeList.CreateWaterTraderUnlock()
			},
			{
				"WINEMAKER",
				NetworkUpgradeList.CreateWinemakerUnlock()
			},
			{
				"AQUACULTURE",
				NetworkUpgradeList.CreateAquacultureUnlock()
			},
			{
				"AQUACULTURE_II",
				NetworkUpgradeList.CreateAquacultureIIUnlock()
			},
			{
				"PLANT_DNA",
				NetworkUpgradeList.CreatePlantDnaUnlock()
			},
			{
				"DOSSIER_UPGRADE",
				NetworkUpgradeList.CreateDossierUpgrade()
			},
			{
				"TRASHCAN_UPGRADE",
				NetworkUpgradeList.CreateTrashcanUpgrade()
			},
			{
				"VENDING_MACHINE",
				NetworkUpgradeList.CreateVendingMachineUpgrade()
			},
			{
				"VENDING_FOUNTAIN",
				NetworkUpgradeList.CreateVendingFountainUpgrade()
			},
			{
				"MARKETPLACE_REQUESTS",
				NetworkUpgradeList.CreateMarketplaceRequestsUnlock()
			},
			{
				"JACKSON1",
				NetworkUpgradeList.CreateJacksonInvestment1()
			},
			{
				"JACKSON2",
				NetworkUpgradeList.CreateJacksonInvestment2()
			},
			{
				"JACKSON_SPECIAL_STOCK",
				NetworkUpgradeList.CreateJacksonSpecialStock()
			},
			{
				"RENOVATION1",
				NetworkUpgradeList.CreateRenovation1()
			},
			{
				"RENOVATION2",
				NetworkUpgradeList.CreateRenovation2()
			},
			{
				"RENOVATION3",
				NetworkUpgradeList.CreateRenovation3()
			},
			{
				"HYPERTECH_SMUGGLER",
				NetworkUpgradeList.CreateHypertechSmugglerReferral()
			},
			{
				"ENDORSEMENT1",
				NetworkUpgradeList.CreateEndorsement1()
			},
			{
				"ENDORSEMENT2",
				NetworkUpgradeList.CreateEndorsement2()
			},
			{
				"ENDORSEMENT3",
				NetworkUpgradeList.CreateEndorsement3()
			}
		};
		foreach (NetworkUpgrade modDefinition in modDefinitions)
		{
			if (dictionary.ContainsKey(modDefinition.id))
			{
				UnityEngine.Debug.LogWarning("[NetworkUpgrade] Mod '" + modDefinition.source + "' registered upgrade '" + modDefinition.id + "', which already exists; ignored");
			}
			else
			{
				dictionary.Add(modDefinition.id, modDefinition.CopyForRuntime());
			}
		}
		return dictionary;
	}

	public static void AddMissingAfterLoad()
	{
		if (PlayerStore.Instance.networkUpgrade == null)
		{
			PlayerStore.Instance.networkUpgrade = new Dictionary<string, NetworkUpgrade>();
		}
		foreach (KeyValuePair<string, NetworkUpgrade> item in InitUpgradeDict())
		{
			if (!PlayerStore.Instance.networkUpgrade.ContainsKey(item.Key))
			{
				PlayerStore.Instance.networkUpgrade.Add(item.Key, item.Value);
			}
		}
		if (PlayerStore.Instance.isVendingMachineUnlocked)
		{
			MarkUnlockedSilently("VENDING_MACHINE");
		}
		if (PlayerStore.Instance.isVendingFountainUnlocked)
		{
			MarkUnlockedSilently("VENDING_FOUNTAIN");
		}
		if (PlayerStore.Instance.networkUpgrade.TryGetValue("PLANT_DNA", out var value) && !value.IsUnlocked())
		{
			value.cost = 1250;
		}
	}

	public static bool IsLockedInDemo(string id)
	{
		if (demoLockedIds == null)
		{
			demoLockedIds = new HashSet<string>();
			foreach (KeyValuePair<string, NetworkUpgrade> item in InitUpgradeDict())
			{
				if (item.Value.lockInDemo)
				{
					demoLockedIds.Add(item.Key);
				}
			}
		}
		return demoLockedIds.Contains(id);
	}

	public static bool IsHiddenForRun(string id)
	{
		if (StartingPerk.IsMakeshiftReactor())
		{
			switch (id)
			{
			case "POWER_HIJACK":
			case "POWER_HIJACK2":
			case "POWER_HIJACK3":
			case "COMMERCIAL_POWER":
				return true;
			}
		}
		if (GetDefinition(id)?.tab == "Advanced" && !PlayerStore.Instance.IsPropertyPaid)
		{
			return true;
		}
		return false;
	}

	public static int GetWildFixerTier()
	{
		Dictionary<string, NetworkUpgrade> dictionary = PlayerStore.Instance?.networkUpgrade;
		if (dictionary == null)
		{
			return 1;
		}
		if (dictionary.TryGetValue("WILD_FIXER3", out var value) && value.IsUnlocked())
		{
			return 3;
		}
		if (dictionary.TryGetValue("WILD_FIXER2", out var value2) && value2.IsUnlocked())
		{
			return 2;
		}
		return 1;
	}

	public static int GetPowerHijackTier()
	{
		Dictionary<string, NetworkUpgrade> dictionary = PlayerStore.Instance?.networkUpgrade;
		if (dictionary == null)
		{
			return 0;
		}
		if (dictionary.TryGetValue("POWER_HIJACK3", out var value) && value.IsUnlocked())
		{
			return 3;
		}
		if (dictionary.TryGetValue("POWER_HIJACK2", out var value2) && value2.IsUnlocked())
		{
			return 2;
		}
		if (dictionary.TryGetValue("POWER_HIJACK", out var value3) && value3.IsUnlocked())
		{
			return 1;
		}
		return 0;
	}

	public static int GetEndorsementTier()
	{
		Dictionary<string, NetworkUpgrade> dictionary = PlayerStore.Instance?.networkUpgrade;
		if (dictionary == null)
		{
			return 0;
		}
		int num = 0;
		string[] array = new string[3] { "ENDORSEMENT1", "ENDORSEMENT2", "ENDORSEMENT3" };
		foreach (string key in array)
		{
			if (dictionary.TryGetValue(key, out var value) && value.IsUnlocked())
			{
				num++;
			}
		}
		return num;
	}

	public string GetLocalizedTitle()
	{
		NetworkUpgrade definition = GetDefinition(id);
		if (definition != null && definition.titleProvider != null)
		{
			return SafeText(definition.titleProvider, id);
		}
		return LocHelper.GetLocalizedNetworkUpgrade("network_" + id + "_title");
	}

	public string GetLocalizedSubtitle()
	{
		NetworkUpgrade definition = GetDefinition(id);
		return GetLocalizedSubtitle(definition?.subtitle ?? subtitle, definition?.subtitleProvider);
	}

	public static string GetLocalizedSubtitle(string subtitle, Func<string> provider = null)
	{
		if (provider != null)
		{
			return SafeText(provider, subtitle ?? "");
		}
		if (string.IsNullOrEmpty(subtitle))
		{
			return "";
		}
		if (subtitleFromOtherTable.TryGetValue(subtitle, out var value))
		{
			return SafeText(value, subtitle);
		}
		string key = "network_subtitle_" + subtitle;
		if (LocHelper.HasLocalizedNetworkUpgrade(key))
		{
			return LocHelper.GetLocalizedNetworkUpgrade(key);
		}
		return subtitle;
	}

	public string GetLocalizedDescription()
	{
		NetworkUpgrade definition = GetDefinition(id);
		if (definition != null && definition.descriptionProvider != null)
		{
			return SafeText(definition.descriptionProvider, "");
		}
		return LocHelper.GetLocalizedNetworkUpgrade("network_" + id + "_desc", GetDescriptionArguments());
	}

	private object[] GetDescriptionArguments()
	{
		switch (id)
		{
		case "EVIDENCE_REMOVAL":
			return ArgsEvidenceRemoval;
		case "CRIMINEL_NETWORK":
			return ArgsCriminelNetwork;
		case "COMMERCIAL_POWER":
			return ArgsCommercialPower;
		case "SHOWCASE_I":
			return ArgsShowcase;
		case "SHOWCASE_II":
			return ArgsShowcase;
		case "GUN_PERMIT_I":
			return ArgsGunPermitI;
		case "GUN_PERMIT_II":
			return ArgsGunPermitII;
		case "GUN_PERMIT_III":
			return ArgsGunPermitIII;
		case "RENOVATION1":
		case "RENOVATION2":
		case "RENOVATION3":
			return ArgsRenovation;
		case "BREWER_SHAREHOLDER":
			return ArgsBrewerShareholder;
		case "MINER_SHAREHOLDER":
			return ArgsMinerShareholder;
		case "CHEMIST_SHAREHOLDER":
			return ArgsChemistShareholder;
		case "PHARMA_SHAREHOLDER":
			return ArgsPharmaShareholder;
		default:
			return NoArgs;
		}
	}

	public string GetLocalizedAlreadyBought()
	{
		NetworkUpgrade definition = GetDefinition(id);
		if (definition != null && definition.alreadyBoughtProvider != null)
		{
			return SafeText(definition.alreadyBoughtProvider, "");
		}
		return LocHelper.GetLocalizedNetworkUpgrade("network_" + id + "_already_bought");
	}

	public bool IsReady()
	{
		if (!IsHavePrerequisite())
		{
			return false;
		}
		if (cooldownDuration != 0 && cooldownCurrent != 0)
		{
			return false;
		}
		if (state == 1)
		{
			return true;
		}
		return false;
	}

	public void OnDayStart()
	{
		if (cooldownDuration != 0)
		{
			cooldownCurrent--;
			if (cooldownCurrent < 0)
			{
				cooldownCurrent = 0;
			}
		}
	}

	public bool IsUnlocked()
	{
		if (state == 2)
		{
			return true;
		}
		return false;
	}

	public void Unlock()
	{
		state = 2;
		if (!string.IsNullOrEmpty(id) && NetworkUpgradeList.UnlockActions.TryGetValue(id, out var value))
		{
			value?.Invoke();
		}
		if (cooldownDuration != 0)
		{
			if (!(GetDefinition(id) ?? this).heldUntilUsed)
			{
				state = 1;
			}
			cooldownCurrent = cooldownDuration;
		}
		ModHook.FireOnNetworkUpgradeUnlocked(id);
	}

	public bool IsHavePrerequisite()
	{
		if (prerequisite.Count == 0)
		{
			return true;
		}
		foreach (string item in prerequisite)
		{
			if (!TryGetRuntime(item, out var upgrade) || !upgrade.IsUnlocked())
			{
				return false;
			}
		}
		return true;
	}

	public string GetMissingPrerequisite()
	{
		List<string> list = new List<string>();
		foreach (string item in prerequisite)
		{
			if (!TryGetRuntime(item, out var upgrade))
			{
				list.Add(item);
			}
			else if (!upgrade.IsUnlocked())
			{
				list.Add(upgrade.GetLocalizedTitle());
			}
		}
		if (list.Count <= 0)
		{
			return "";
		}
		return string.Join(", ", list) + ".";
	}

	public int GetCost()
	{
		float num = Difficulty.Rules.upgradePriceMultiplier;
		if (StoreReputation.IsPerkUnlocked("LLPILLAR"))
		{
			num *= 0.85f;
		}
		if (StartingPerk.IsWildFavorite())
		{
			num *= 0.9f;
		}
		if (StartingPerk.IsWildBadSide())
		{
			num *= 1.1f;
		}
		if (Mathf.Approximately(num, 1f))
		{
			return cost;
		}
		return RNG.MultiplyAndRound(cost, num);
	}

	public static bool IsUnlocked(string id)
	{
		if (!PlayerStore.Instance.networkUpgrade.ContainsKey(id))
		{
			return false;
		}
		return PlayerStore.Instance.networkUpgrade[id].IsUnlocked();
	}

	private static bool TryGetRuntime(string id, out NetworkUpgrade upgrade)
	{
		upgrade = null;
		if (string.IsNullOrEmpty(id) || PlayerStore.Instance == null || PlayerStore.Instance.networkUpgrade == null)
		{
			return false;
		}
		if (PlayerStore.Instance.networkUpgrade.TryGetValue(id, out upgrade))
		{
			return true;
		}
		if (unknownPrerequisiteWarned.Add(id))
		{
			UnityEngine.Debug.LogWarning("[NetworkUpgrade] Prerequisite '" + id + "' is not an upgrade of this run; treated as not unlocked");
		}
		return false;
	}

	public static bool IsBrewerUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["BREWER"].IsUnlocked();
	}

	public static bool IsChemistUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["CHEMIST"].IsUnlocked();
	}

	public static bool IsCommercialPowerUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["COMMERCIAL_POWER"].IsUnlocked();
	}

	public static bool IsPharmaUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["PHARMA"].IsUnlocked();
	}

	public static bool IsMinerUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["MINER"].IsUnlocked();
	}

	public static bool IsLandlordGraceUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["LANDLORD_GRACE"].IsUnlocked();
	}

	public static bool IsSecGraceUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["SEC_GRACE"].IsUnlocked();
	}

	public static bool IsInspectionInformantUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["INSPECTION_INFORMANT"].IsUnlocked();
	}

	public static bool IsBuyReferalUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["BUY_REFERAL"].IsUnlocked();
	}

	public static bool IsSellReferalUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["SELL_REFERAL"].IsUnlocked();
	}

	public static bool IsShowcaseIUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["SHOWCASE_I"].IsUnlocked();
	}

	public static bool IsShowcaseIIUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["SHOWCASE_II"].IsUnlocked();
	}

	public static bool IsRetiredFarmerUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["RETIRED_FARMER"].IsUnlocked();
	}

	public static bool IsWinemakerUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["WINEMAKER"].IsUnlocked();
	}

	public static bool IsRetiredRancherUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["RETIRED_RANCHER"].IsUnlocked();
	}

	public static bool IsAquacultureUnlocked()
	{
		return IsUnlocked("AQUACULTURE");
	}

	public static bool IsAquacultureIIUnlocked()
	{
		return IsUnlocked("AQUACULTURE_II");
	}

	public static bool IsPlantDnaUnlocked()
	{
		return IsUnlocked("PLANT_DNA");
	}

	public static bool IsDossierUpgradeUnlocked()
	{
		return IsUnlocked("DOSSIER_UPGRADE");
	}

	public static bool IsVendingMachineUpgradeUnlocked()
	{
		return IsUnlocked("VENDING_MACHINE");
	}

	public static bool IsVendingFountainUpgradeUnlocked()
	{
		return IsUnlocked("VENDING_FOUNTAIN");
	}

	public static void MarkUnlockedSilently(string id)
	{
		if (PlayerStore.Instance != null && PlayerStore.Instance.networkUpgrade != null && PlayerStore.Instance.networkUpgrade.TryGetValue(id, out var value) && value.cooldownDuration == 0)
		{
			value.state = 2;
		}
	}

	public static bool IsTrashcanUpgradeUnlocked()
	{
		return IsUnlocked("TRASHCAN_UPGRADE");
	}

	public static bool IsRetiredChemistUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["RETIRED_CHEMIST"].IsUnlocked();
	}

	public static NetworkUpgrade GetUpgradeById(string id)
	{
		return PlayerStore.Instance.networkUpgrade[id];
	}

	public static bool IsRetiredGunsmithUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["RETIRED_GUNSMITH"].IsUnlocked();
	}

	public static bool IsGunPermitIUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["GUN_PERMIT_I"].IsUnlocked();
	}

	public static bool IsGunPermitIIUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["GUN_PERMIT_II"].IsUnlocked();
	}

	public static bool IsGunPermitIIIUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["GUN_PERMIT_III"].IsUnlocked();
	}

	public static bool IsPowerHijackUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["POWER_HIJACK"].IsUnlocked();
	}

	public static bool IsMarketplaceRequestsUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["MARKETPLACE_REQUESTS"].IsUnlocked();
	}

	public static bool IsJackson1Unlocked()
	{
		return PlayerStore.Instance.networkUpgrade["JACKSON1"].IsUnlocked();
	}

	public static bool IsJackson2Unlocked()
	{
		return PlayerStore.Instance.networkUpgrade["JACKSON2"].IsUnlocked();
	}

	public static bool IsWaterTraderUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["WATER_TRADER"].IsUnlocked();
	}

	public static bool IsRuinedMachineUnlocked()
	{
		return PlayerStore.Instance.networkUpgrade["RUINED_MACHINE_UNLOCK"].IsUnlocked();
	}
}

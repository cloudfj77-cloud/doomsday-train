using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ShowcaseHelper
{
	public const int T2_SPAWN_DAY = 0;

	public const int T3_SPAWN_DAY = 14;

	private static readonly string[] t1WaterClientIds = new string[2] { "thirstySpacer", "scavWaterShopper" };

	private static readonly string[] t1FoodClientIds = new string[3] { "spacerFoodI", "hungrySpacer", "spacerChef" };

	private static readonly string[] t1MedicalClientIds = new string[3] { "spacerMedical", "sickChildCaretaker", "lowLevelDoctor" };

	private static readonly string[] t1MaterialClientIds = new string[3] { "lowerRepairman", "scrap_byer", "maintenanceWorker" };

	private static readonly string[] t1HouseholdClientIds = new string[3] { "spacerHouseholdItem", "youngSpacerHousehold", "familyMother" };

	private static readonly string[] t1WeaponClientIds = new string[1] { "scavenger" };

	private static readonly string[] t2WeaponClientIds = new string[1] { "selfDefenseEnthusiast" };

	private static readonly string[] t3WeaponClientIds = new string[1] { "privateSecurityContractor" };

	private static readonly string[] t1SubstanceClientIds = new string[1] { "desperateAddict" };

	private static readonly string[] t2SubstanceClientIds = new string[1] { "wornOutSpacer" };

	private static readonly string[] t3SubstanceClientIds = new string[1] { "upperLevelHedonist" };

	public const string SHOWCASE_SPECIAL_ITEM_TAG = "SHOWCASE_SPECIAL_ITEM_TAG";

	public const string SHOWCASE_SPECIAL_ITEM_TYPE = "SHOWCASE_SPECIAL_ITEM_TYPE";

	private static readonly (string clientId, string permitId, int baseChance)[] GUN_BUYERS = new(string, string, int)[3]
	{
		("ll_gun_buyer", "permit_gun_1", 50),
		("merc_gun_buyer", "permit_gun_2", 40),
		("ul_gun_buyer", "permit_gun_3", 35)
	};

	public const string SIGN_FIREARM = "sign_firearm";

	public const string SIGN_FIREARM_HEAVY = "sign_firearm_heavy";

	public const int FIREARM_SIGN_CHANCE = 40;

	private static readonly Dictionary<string, string> pendingSupplierPicks = new Dictionary<string, string>();

	private static readonly string[] AUG_DISGUISE_TYPES = new string[2] { "MATERIAL", "MEDICAL" };

	public static void InitShowcaseSpecialItem(GameItem item, string type)
	{
		item.ModifyTag("SHOWCASE_SPECIAL_ITEM_TAG", delegate(TagState state)
		{
			state.Enable().SetString(type);
		});
	}

	public static bool HasShowcaseSpecialItem()
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (childItem.IsTag("SHOWCASE_SPECIAL_ITEM_TAG"))
			{
				return true;
			}
		}
		return false;
	}

	public static bool HasMerchandise()
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (!childItem.IsTag("SHOWCASE_SPECIAL_ITEM_TAG"))
			{
				return true;
			}
		}
		return false;
	}

	private static string PickRandomId(string[] ids)
	{
		return ids[RNG.GetRandomIntExculsive(0, ids.Length)];
	}

	public static int GetTypeCount(string type)
	{
		int num = 0;
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (childItem.GetGameItemType().Contains(type))
			{
				num++;
			}
		}
		return num;
	}

	private static bool CountsAsDisplayType(GameItem gameItem, string type)
	{
		if (gameItem.GetGameItemType().Contains(type))
		{
			return true;
		}
		if (type.Equals("MATERIAL"))
		{
			return gameItem.IsGameItemType("ENERGY_CELL");
		}
		return false;
	}

	public static int GetTypeTotalValue(string type)
	{
		int num = 0;
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (CountsAsDisplayType(childItem, type))
			{
				num += (int)childItem.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L);
			}
		}
		return num;
	}

	private static int GetGunBuyerDaysMissed(int index)
	{
		StoreClientManager storeClientManager = PlayerStore.Instance.storeClientManager;
		return index switch
		{
			0 => storeClientManager.gunBuyerDaysMissedBasic, 
			1 => storeClientManager.gunBuyerDaysMissedMid, 
			_ => storeClientManager.gunBuyerDaysMissedHigh, 
		};
	}

	private static void SetGunBuyerDaysMissed(int index, int days)
	{
		StoreClientManager storeClientManager = PlayerStore.Instance.storeClientManager;
		switch (index)
		{
		case 0:
			storeClientManager.gunBuyerDaysMissedBasic = days;
			break;
		case 1:
			storeClientManager.gunBuyerDaysMissedMid = days;
			break;
		default:
			storeClientManager.gunBuyerDaysMissedHigh = days;
			break;
		}
	}

	public static int GetGunBuyerChance(int index)
	{
		int num = Mathf.Min(100, GUN_BUYERS[index].baseChance * (2 + GetGunBuyerDaysMissed(index)) / 2);
		float num2 = Mathf.Max(0f, 1f + (float)GunOrderData.GetMarketModifierPercent() / 100f);
		return Mathf.Clamp(Mathf.RoundToInt((float)num * num2), 0, 100);
	}

	private static void AddGunBuyerPredictions(List<(string, int, string)> prediction)
	{
		for (int i = 0; i < GUN_BUYERS.Length; i++)
		{
			if (IsShowcaseContainItemById(GUN_BUYERS[i].permitId))
			{
				prediction.Add((GUN_BUYERS[i].clientId, GetGunBuyerChance(i), GUN_BUYERS[i].permitId));
			}
		}
	}

	public static void OnGunBuyersRolled(List<(string, int, string)> prediction, HashSet<string> spawnedIds)
	{
		for (int i = 0; i < GUN_BUYERS.Length; i++)
		{
			string clientId = GUN_BUYERS[i].clientId;
			if (prediction.Exists(((string, int, string) entry) => entry.Item1 == clientId))
			{
				SetGunBuyerDaysMissed(i, (!spawnedIds.Contains(clientId)) ? (GetGunBuyerDaysMissed(i) + 1) : 0);
			}
		}
	}

	private static string GetFirearmSellerId()
	{
		List<string> obj = new List<string> { "ll_gun_seller", "merc_gun_seller" };
		List<int> oddArray = new List<int> { 100, 90 };
		return obj[RNG.GetOdd(oddArray)];
	}

	public static string GetChanceSuffix(string clientId)
	{
		for (int i = 0; i < GUN_BUYERS.Length; i++)
		{
			if (!(GUN_BUYERS[i].clientId != clientId) && (GetGunBuyerDaysMissed(i) > 0 || GunOrderData.GetMarketModifierPercent() != 0))
			{
				return "*";
			}
		}
		return "";
	}

	private static string PickOnce(string signId, Func<string> roll)
	{
		if (!pendingSupplierPicks.TryGetValue(signId, out var value))
		{
			value = roll();
			pendingSupplierPicks[signId] = value;
		}
		return value;
	}

	public static void ClearPendingSupplierPicks()
	{
		pendingSupplierPicks.Clear();
	}

	public static List<(string, int, string)> GetDisplayCasePrediction()
	{
		List<(string, int, string)> list = new List<(string, int, string)>();
		AddGunBuyerPredictions(list);
		int num = GetShowCaseContainItemCount("scav_token") * 20;
		if (num > 0)
		{
			list.Add(("scavGeneral", num, "scav_token"));
		}
		foreach (var (item, num2) in BlackLampHelper.GetActiveInvitations())
		{
			if (num2 > 0)
			{
				list.Add((item, num2, "black_lamp"));
			}
		}
		if (IsShowcaseContainItemById("sign_water"))
		{
			list.Add((GetRandomWaterSupplierId(), 75, "sign_water"));
		}
		if (IsShowcaseContainItemById("sign_food"))
		{
			list.Add((PickOnce("sign_food", GetFoodSupplierId), 75, "sign_food"));
		}
		if (IsShowcaseContainItemById("sign_material"))
		{
			list.Add((PickOnce("sign_material", GetMaterialSupplierId), 75, "sign_material"));
		}
		if (IsShowcaseContainItemById("sign_ruined_machine"))
		{
			list.Add(("machine_scrapper", 75, "sign_ruined_machine"));
		}
		if (IsShowcaseContainItemById("sign_medical"))
		{
			list.Add((PickOnce("sign_medical", GetMedSupplierId), 75, "sign_medical"));
		}
		if (IsShowcaseContainItemById("sign_household"))
		{
			list.Add((PickOnce("sign_household", GetHouseholdSupplierId), 75, "sign_household"));
		}
		GameItem displayedCertificate = AppraisalRankHelper.GetDisplayedCertificate();
		if (displayedCertificate != null)
		{
			list.Add(("scavAppraisalRandom", AppraisalRankHelper.GetAttractionChance(), displayedCertificate.identifier));
		}
		if (IsShowcaseContainItemById("sign_firearm"))
		{
			string item2 = PickOnce("sign_firearm", GetFirearmSellerId);
			list.Add((item2, 40, "sign_firearm"));
		}
		if (IsShowcaseContainItemById("sign_firearm_heavy"))
		{
			list.Add(("ul_gun_seller", 25, "sign_firearm_heavy"));
		}
		return list;
	}

	private static string GetRandomWaterSupplierId()
	{
		if (!NetworkUpgrade.IsWaterTraderUnlocked())
		{
			return "honestBadWaterSeller";
		}
		return (StoreStation.Instance.dayCounter % 7) switch
		{
			1 => "waterCompany", 
			2 => "waterSellerScav", 
			3 => "honestBadWaterSeller", 
			4 => "waterHishonestSellerScav", 
			5 => "waterCompany", 
			6 => "waterSellerScav", 
			_ => "honestBadWaterSeller", 
		};
	}

	private static string GetFoodSupplierId()
	{
		string[] array = new string[2] { "scavGenericFoodSeller", "thiefGenericFoodSeller" };
		int num = Mathf.Max(0, StoreReputation.GetLLReputation() + 25);
		int num2 = Mathf.Max(0, StoreReputation.GetBMReputation());
		int[] oddArray = new int[2] { num, num2 };
		return array[RNG.GetOdd(oddArray)];
	}

	private static string GetMaterialSupplierId()
	{
		string[] array = new string[2] { "scavGenericMaterialSeller", "thiefGenericMaterialSeller" };
		int num = Mathf.Max(0, StoreReputation.GetLLReputation() + 25);
		int num2 = Mathf.Max(0, StoreReputation.GetBMReputation());
		int[] oddArray = new int[2] { num, num2 };
		return array[RNG.GetOdd(oddArray)];
	}

	private static string GetMedSupplierId()
	{
		string[] array = new string[2] { "scavGenericMedSeller", "thiefGenericMedSeller" };
		int num = Mathf.Max(0, StoreReputation.GetLLReputation() + 25);
		int num2 = Mathf.Max(0, StoreReputation.GetBMReputation());
		int[] oddArray = new int[2] { num, num2 };
		return array[RNG.GetOdd(oddArray)];
	}

	private static string GetHouseholdSupplierId()
	{
		string[] array = new string[2] { "scavGenericHouseholdSeller", "thiefGenericHouseholdSeller" };
		int num = Mathf.Max(0, StoreReputation.GetLLReputation() + 25);
		int num2 = Mathf.Max(0, StoreReputation.GetBMReputation());
		int[] oddArray = new int[2] { num, num2 };
		return array[RNG.GetOdd(oddArray)];
	}

	private static string GetLuxurySupplierId()
	{
		return "";
	}

	public static int GetDiscerningShopperChance()
	{
		if (!StoreReputation.IsPerkUnlocked("UL_DISCERNING_TASTE"))
		{
			return 0;
		}
		if (!PlayerStore.Instance.skipIntro && StoreStation.Instance.dayCounter <= 13)
		{
			return 0;
		}
		if (GetTypeCount("LUXURY_ITEM") == 0)
		{
			return 0;
		}
		if (GetDiscerningShopperCooldownLeft() > 1)
		{
			return 0;
		}
		return StoreReputationPerk.GetDiscerningTasteChance();
	}

	public static int GetDiscerningShopperCooldownLeft()
	{
		PlayerStore.Instance.storeClientManager.clientTracker.TryGetValue("ulDiscerningShopper", out var value);
		return value;
	}

	public static int GetDiscerningShopperCooldownOpenings()
	{
		if (!StoreReputation.IsPerkUnlocked("UL_DISCERNING_TASTE"))
		{
			return 0;
		}
		return Mathf.Max(0, GetDiscerningShopperCooldownLeft() - 1);
	}

	public static List<StoreClient> HandleDisplayCase()
	{
		List<StoreClient> list = new List<StoreClient>();
		foreach (var item3 in GetDisplayCaseCustomer())
		{
			string item = item3.clientId;
			string item2 = item3.type;
			StoreClient storeClient = StoreClientListDict.CreateStoreClient(item);
			if (storeClient != null)
			{
				storeClient.attractedType = item2;
				list.Add(storeClient);
			}
		}
		return list;
	}

	public static List<string> GetDisplayCaseType()
	{
		List<string> list = new List<string>();
		if (EmporiumEntry.Instance.showcaseElement.childItems.Count != 0)
		{
			List<KeyValuePair<string, int>> typesByValue = PlayerStore.Instance.GetTypesByValue(EmporiumEntry.Instance.showcaseElement.childItems, TypeHelper.MAJOR_TYPES_LIST);
			int num = Math.Min(PlayerStore.Instance.GetShowcaseTypeVisitorCount(), typesByValue.Count);
			for (int i = 0; i < num; i++)
			{
				if (typesByValue[i].Value > 0)
				{
					list.Add(typesByValue[i].Key);
				}
			}
		}
		return list;
	}

	public static List<(string clientId, string type)> GetDisplayCaseCustomer()
	{
		List<(string, string)> list = new List<(string, string)>();
		foreach (string item in GetDisplayCaseType())
		{
			string text = CreateClientByStockType(item);
			if (text != null)
			{
				list.Add((text, item));
			}
			Debug.Log("Display Case : " + item);
		}
		return list;
	}

	public static StoreClient CreateDisplayCaseAugBuyer()
	{
		List<string> list = (from t in GetDisplayCaseType()
			where AUG_DISGUISE_TYPES.Contains(t)
			select t).ToList();
		if (list.Count == 0)
		{
			return null;
		}
		string text = list[RNG.GetRandomIntExculsive(0, list.Count)];
		string text2 = CreateClientByStockType(text);
		if (text2 == null)
		{
			return null;
		}
		StoreClient storeClient = StoreClientListDict.CreateStoreClient(text2);
		if (storeClient == null)
		{
			return null;
		}
		storeClient.attractedType = text;
		return storeClient;
	}

	public static string GetMostCommonWaterTypeInDisplay()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (!childItem.IsTag("WATER"))
			{
				continue;
			}
			ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(childItem);
			if (waterFeature != null && waterFeature.fakeCondition != null)
			{
				string identifier = waterFeature.fakeCondition.identifier;
				if (string.IsNullOrEmpty(identifier))
				{
					continue;
				}
				if (!dictionary.ContainsKey(identifier))
				{
					dictionary[identifier] = 0;
				}
				dictionary[identifier] += (int)childItem.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L);
			}
			if (childItem.IsGameItemType("WATER_RATION"))
			{
				if (!dictionary.ContainsKey(WaterFeatureHelper.BASE_WATER_DISPLAY))
				{
					dictionary[WaterFeatureHelper.BASE_WATER_DISPLAY] = 0;
				}
				dictionary[WaterFeatureHelper.BASE_WATER_DISPLAY] += (int)childItem.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L);
			}
		}
		if (dictionary.Count == 0)
		{
			return "";
		}
		return dictionary.OrderByDescending((KeyValuePair<string, int> kvp) => kvp.Value).First().Key;
	}

	public static string CreateClientByStockType(string type)
	{
		if (type != null && !type.Equals(""))
		{
			int typeTotalValue = GetTypeTotalValue(type);
			Debug.Log("Showcase Type it :" + type);
			int currentStoreAttractiveness = PlayerStore.Instance.GetCurrentStoreAttractiveness();
			if (type.Equals("WATER"))
			{
				string mostCommonWaterTypeInDisplay = GetMostCommonWaterTypeInDisplay();
				bool flag = mostCommonWaterTypeInDisplay.Equals(WaterFeatureHelper.PURE_WATER_DISPLAY);
				bool flag2 = mostCommonWaterTypeInDisplay.Equals(WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY);
				if ((typeTotalValue >= 350 || flag) && StoreStation.Instance.dayCounter >= 14)
				{
					return "waterStockpiler";
				}
				if ((typeTotalValue >= 145 || flag || flag2) && StoreStation.Instance.dayCounter >= 0)
				{
					return "waterSpace2";
				}
				if (currentStoreAttractiveness >= 400)
				{
					return "waterSpace2";
				}
				return PickRandomId(t1WaterClientIds);
			}
			if (type.Equals("FOOD"))
			{
				if (typeTotalValue >= 350 && StoreStation.Instance.dayCounter >= 14)
				{
					return "privateChef";
				}
				if (typeTotalValue >= 145 && StoreStation.Instance.dayCounter >= 0)
				{
					return "expeditionSupplier";
				}
				if (currentStoreAttractiveness >= 400)
				{
					return "expeditionSupplier";
				}
				return PickRandomId(t1FoodClientIds);
			}
			if (type.Equals("MATERIAL") || type.Equals("MODULE") || type.Equals("ENERGY_CELL") || type.Equals("TOOL"))
			{
				if (typeTotalValue >= 350 && StoreStation.Instance.dayCounter >= 14)
				{
					return "stationEngineer";
				}
				if (typeTotalValue >= 145 && StoreStation.Instance.dayCounter >= 0)
				{
					return "lowerInventor";
				}
				if (type.Equals("MODULE") || type.Equals("TOOL"))
				{
					return "lowerInventor";
				}
				if (currentStoreAttractiveness >= 400)
				{
					return "lowerInventor";
				}
				return PickRandomId(t1MaterialClientIds);
			}
			if (type.Equals("MEDICAL"))
			{
				if (typeTotalValue >= 350 && StoreStation.Instance.dayCounter >= 14)
				{
					return "hospitalAdmin";
				}
				if (typeTotalValue >= 145 && StoreStation.Instance.dayCounter >= 0)
				{
					return "clinicNurse";
				}
				if (currentStoreAttractiveness >= 400)
				{
					return "clinicNurse";
				}
				return PickRandomId(t1MedicalClientIds);
			}
			if (type.Equals("HOUSEHOLD_GOOD"))
			{
				if (typeTotalValue >= 300 && StoreStation.Instance.dayCounter >= 14)
				{
					return "upperMaid";
				}
				if (typeTotalValue >= 145 && StoreStation.Instance.dayCounter >= 0)
				{
					return "citizenHousehold";
				}
				if (currentStoreAttractiveness >= 400)
				{
					return "citizenHousehold";
				}
				return PickRandomId(t1HouseholdClientIds);
			}
			if (type.Equals("WEAPON") || type.Equals("ARMOR") || type.Equals("AMMUNITION"))
			{
				if (typeTotalValue >= 350 && StoreStation.Instance.dayCounter >= 14)
				{
					return PickRandomId(t3WeaponClientIds);
				}
				if (typeTotalValue >= 145 && StoreStation.Instance.dayCounter >= 0)
				{
					return PickRandomId(t2WeaponClientIds);
				}
				if (currentStoreAttractiveness >= 400)
				{
					return PickRandomId(t2WeaponClientIds);
				}
				return PickRandomId(t1WeaponClientIds);
			}
			if (type.Equals("SUBSTANCE"))
			{
				if (typeTotalValue >= 350 && StoreStation.Instance.dayCounter >= 14)
				{
					return PickRandomId(t3SubstanceClientIds);
				}
				if (typeTotalValue >= 145 && StoreStation.Instance.dayCounter >= 0)
				{
					return PickRandomId(t2SubstanceClientIds);
				}
				if (currentStoreAttractiveness >= 400)
				{
					return PickRandomId(t2SubstanceClientIds);
				}
				return PickRandomId(t1SubstanceClientIds);
			}
		}
		Debug.Log("CreateClientByStockType was not able to find a suitable customer");
		return null;
	}

	public static bool IsShowcaseContainItemById(string id)
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (childItem.identifier.Equals(id))
			{
				return true;
			}
		}
		return false;
	}

	public static int GetShowCaseContainItemCount(string id)
	{
		int num = 0;
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (childItem.identifier.Equals(id))
			{
				num++;
			}
		}
		return num;
	}

	public static bool IsShowCaseContainSeriousContraband()
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (childItem.IsTag("CONTRABAND_ITEM_TAG"))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsShowCaseContainSeriousStolen()
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (childItem.IsTag("STOLEN_TAG") && RNG.Roll(StolenHelper.GetFoundChance(childItem)))
			{
				StolenHelper.SetHeat(childItem, 100);
				return true;
			}
		}
		return false;
	}
}

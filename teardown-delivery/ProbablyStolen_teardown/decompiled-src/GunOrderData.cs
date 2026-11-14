using System;
using System.Collections.Generic;
using UnityEngine;

public class GunOrderData
{
	public const int TIER_BASIC = 1;

	public const int TIER_MID = 2;

	public const int TIER_HIGH = 3;

	public int tier = 1;

	public List<GunOrderRequirement> mandatoryRequirements = new List<GunOrderRequirement>();

	public List<GunOrderRequirement> bonusRequirements = new List<GunOrderRequirement>();

	public int baseReward;

	public string dialogText = "";

	public bool isBestOrder;

	public const int BEST_PREMIUM_PERCENT = 50;

	private static readonly string[] PRICE_EVENT_TYPES = new string[2] { "WEAPON", "FIREARM" };

	public bool isServiceOrder;

	private string serviceGunId;

	private ItemFeatureList.EquipmentCondition serviceGunCondition;

	private GameItem serviceGun;

	private static readonly (string id, int weight)[][] SERVICE_GUNS = new(string, int)[4][]
	{
		null,
		new(string, int)[2]
		{
			("handmade_pistol", 70),
			("printed_gun", 30)
		},
		new(string, int)[2]
		{
			("heavy_handmade_pistol", 60),
			("glock_receiver", 40)
		},
		new(string, int)[2]
		{
			("smg", 60),
			("shotgun", 40)
		}
	};

	private static readonly int[] SERVICE_FEE = new int[4] { 0, 30, 60, 100 };

	private static readonly int[] SERVICE_MANDATORY_STATS = new int[4] { 0, 1, 2, 2 };

	private static readonly int[] SERVICE_SIDE_STATS = new int[4] { 0, 2, 2, 3 };

	private static readonly int[] SERVICE_STAT_STEP = new int[4] { 0, 8, 10, 12 };

	private static readonly int[] SERVICE_SECOND_STEP_REWARD = new int[4] { 0, 15, 25, 40 };

	private static readonly int[] SERVICE_SIDE_STEP_REWARD = new int[4] { 0, 12, 20, 32 };

	private static readonly string[] SERVICE_STATS = new string[6] { "GUN_CURRENT_FIREPOWER_TAG", "GUN_CURRENT_STYLE_TAG", "GUN_CURRENT_RELIABILITY_TAG", "GUN_CURRENT_RECOIL_TAG", "GUN_CURRENT_ERGONOMICS_TAG", "GUN_CURRENT_ACCURACY_TAG" };

	private static readonly string[][] TIER_GUN_IDS = new string[4][]
	{
		new string[0],
		new string[2] { "printed_gun", "handmade_pistol" },
		new string[3] { "heavy_handmade_pistol", "glock_receiver", "revolver" },
		new string[2] { "smg", "shotgun" }
	};

	public const int SERVICE_ORDER_CHANCE_WITHOUT_GUN = 50;

	public static GameItem ResolveOrderGun()
	{
		StoreClient storeClient = PlayerStore.Instance.currentClientInstance?.GetClientBlueprint();
		if (storeClient?.gunOrderData != null && storeClient.gunOrderData.isServiceOrder)
		{
			return storeClient.gunOrderData.GetPlacedServiceGun();
		}
		GameItem currentNegociatedItem = PlayerStore.Instance.currentNegociatedItem;
		if (IsOrderGun(currentNegociatedItem))
		{
			return currentNegociatedItem;
		}
		List<GameItem> childItems = EmporiumEntry.Instance.backInvinvElement.childItems;
		for (int num = childItems.Count - 1; num >= 0; num--)
		{
			if (IsOrderGun(childItems[num]))
			{
				return childItems[num];
			}
		}
		return null;
	}

	private static bool IsOrderGun(GameItem item)
	{
		if (item != null && item.IsTag("ADV_GUN_TAG"))
		{
			return item.parentInventory == EmporiumEntry.Instance.backInvinvElement;
		}
		return false;
	}

	public bool IsAccepted()
	{
		GameItem gameItem = ResolveOrderGun();
		if (gameItem == null)
		{
			return false;
		}
		foreach (GunOrderRequirement mandatoryRequirement in mandatoryRequirements)
		{
			if (!mandatoryRequirement.IsMet(gameItem))
			{
				return false;
			}
		}
		return true;
	}

	public static int ApplyRewardModifiers(int amount)
	{
		int num = 100 + GetMarketModifierPercent() + PlayerStore.Instance.retailMarkup;
		return Mathf.RoundToInt((float)(amount * num) / 100f);
	}

	public static int GetMarketModifierPercent()
	{
		int num = 0;
		string[] pRICE_EVENT_TYPES = PRICE_EVENT_TYPES;
		foreach (string type in pRICE_EVENT_TYPES)
		{
			num += CommissaryData.GetTotalTypeMarketPriceModifier(type);
		}
		return num;
	}

	public static List<NegociationData> GetPriceEventLines()
	{
		List<NegociationData> list = new List<NegociationData>();
		foreach (StoreEvent activeEvent in StoreStation.Instance.storeEventManager.GetActiveEvents())
		{
			foreach (NegociationData negociationData in activeEvent.negociationDatas)
			{
				if (negociationData.value != 0 && Array.IndexOf(PRICE_EVENT_TYPES, negociationData.itemType) >= 0)
				{
					list.Add(negociationData);
				}
			}
		}
		return list;
	}

	public int GetBaseReward()
	{
		return ApplyRewardModifiers(baseReward);
	}

	public static int GetMarketPrice(GameItem gun)
	{
		if (gun == null)
		{
			return 0;
		}
		return (int)gun.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: true, 0L);
	}

	public static int GetBestPremium(GameItem gun)
	{
		return Mathf.RoundToInt((float)(GetMarketPrice(gun) * 50) / 100f);
	}

	public int CountBonusMet(GameItem gun)
	{
		if (gun == null)
		{
			return 0;
		}
		int num = 0;
		foreach (GunOrderRequirement bonusRequirement in bonusRequirements)
		{
			if (bonusRequirement.IsMet(gun))
			{
				num++;
			}
		}
		return num;
	}

	public int GetTotalReward(GameItem gun)
	{
		if (isBestOrder)
		{
			return GetMarketPrice(gun) + GetBestPremium(gun);
		}
		int num = GetBaseReward();
		if (isServiceOrder)
		{
			num += GetPartsValue(gun);
		}
		foreach (GunOrderRequirement bonusRequirement in bonusRequirements)
		{
			if (gun != null && bonusRequirement.IsMet(gun))
			{
				num += bonusRequirement.GetReward();
			}
		}
		return num;
	}

	public string GetTotalRewardDisplay()
	{
		GameItem gameItem = ResolveOrderGun();
		if (gameItem == null)
		{
			return "";
		}
		if (IsAccepted())
		{
			return GetTotalReward(gameItem) + GeneralHelper.CreditIcon();
		}
		return "";
	}

	public void OnOrderAccepted()
	{
		PlayerStore.Instance.ModCash(GetTotalReward(ResolveOrderGun()));
	}

	private static string RequestText(string key, string english)
	{
		return LocHelper.GetLocalizedOrDefault("Dialogue", key, english);
	}

	private static GunOrderRequirement Req(string id, int value, int reward = 0)
	{
		return new GunOrderRequirement
		{
			id = id,
			value = value,
			reward = reward
		};
	}

	public static GunOrderData CreateBestOrder(int tier, int caliber)
	{
		GunOrderData gunOrderData = new GunOrderData();
		gunOrderData.tier = tier;
		gunOrderData.isBestOrder = true;
		gunOrderData.baseReward = 0;
		gunOrderData.dialogText = LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_gun_order_req_best", "Give me your best {0}. I pay what it is worth, and half again on top.", GunHelper.GetCaliberName(caliber));
		gunOrderData.mandatoryRequirements.Add(new GunOrderRequirement
		{
			id = "GUN_CALIBER_TAG",
			value = caliber,
			isCaliber = true
		});
		return gunOrderData;
	}

	public static bool PlayerHasGunOfTier(int tier)
	{
		string[] array = TIER_GUN_IDS[Mathf.Clamp(tier, 1, 3)];
		foreach (GameItem allOwnedItem in EmporiumEntry.Instance.GetAllOwnedItems())
		{
			if (allOwnedItem.IsTag("ADV_GUN_TAG") && Array.IndexOf(array, allOwnedItem.identifier) >= 0)
			{
				return true;
			}
		}
		return false;
	}

	public static bool ShouldFavorServiceOrder(int tier)
	{
		if (!PlayerHasGunOfTier(tier))
		{
			return RNG.Roll(50);
		}
		return false;
	}

	public static GunOrderData CreateServiceOrder(int tier)
	{
		tier = Mathf.Clamp(tier, 1, 3);
		GunOrderData gunOrderData = new GunOrderData();
		gunOrderData.tier = tier;
		gunOrderData.isServiceOrder = true;
		gunOrderData.baseReward = SERVICE_FEE[tier] + RNG.GetRandomInt(0, 10);
		(string, int)[] array = SERVICE_GUNS[tier];
		List<int> list = new List<int>();
		(string, int)[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			(string, int) tuple = array2[i];
			list.Add(tuple.Item2);
		}
		gunOrderData.serviceGunId = array[RNG.GetOdd(list)].Item1;
		int randomInt = RNG.GetRandomInt(0, 99);
		gunOrderData.serviceGunCondition = ((randomInt < 40) ? ItemFeatureList.EquipmentCondition.Poor : ((randomInt < 80) ? ItemFeatureList.EquipmentCondition.Fair : ItemFeatureList.EquipmentCondition.Good));
		gunOrderData.dialogText = LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_gun_order_req_service", "Can you work on my {0}? I want it to do better. I pay for the parts you put on it.", GeneralHelper.GetDisplayNameFromID(gunOrderData.serviceGunId));
		return gunOrderData;
	}

	public GameItem CreateServiceGun()
	{
		if (!isServiceOrder || serviceGun != null)
		{
			return null;
		}
		GameItem gameItem = DirectoryMaster.Item(serviceGunId);
		if (gameItem == null)
		{
			return null;
		}
		GunHelper.SetEquipmentCondition(gameItem, serviceGunCondition);
		gameItem.EnableTag("SERVICE_GUN_TAG");
		serviceGun = gameItem;
		List<string> list = new List<string>(SERVICE_STATS);
		if (serviceGunId == "shotgun")
		{
			list.Remove("GUN_CURRENT_FIREPOWER_TAG");
		}
		int num = SERVICE_STAT_STEP[tier];
		mandatoryRequirements.Clear();
		bonusRequirements.Clear();
		for (int i = 0; i < SERVICE_MANDATORY_STATS[tier]; i++)
		{
			if (list.Count <= 0)
			{
				break;
			}
			string text = TakeRandom(list);
			int @int = gameItem.GetTagReadonly(text).GetInt();
			mandatoryRequirements.Add(Req(text, @int + num - 1));
			bonusRequirements.Add(Req(text, @int + 2 * num - 1, SERVICE_SECOND_STEP_REWARD[tier]));
		}
		for (int j = 0; j < SERVICE_SIDE_STATS[tier]; j++)
		{
			if (list.Count <= 0)
			{
				break;
			}
			string text2 = TakeRandom(list);
			int int2 = gameItem.GetTagReadonly(text2).GetInt();
			bonusRequirements.Add(Req(text2, int2 + num - 1, SERVICE_SIDE_STEP_REWARD[tier]));
		}
		return gameItem;
	}

	private static string TakeRandom(List<string> list)
	{
		int randomIntExculsive = RNG.GetRandomIntExculsive(0, list.Count);
		string result = list[randomIntExculsive];
		list.RemoveAt(randomIntExculsive);
		return result;
	}

	public GameItem GetPlacedServiceGun()
	{
		if (serviceGun == null)
		{
			return null;
		}
		if (serviceGun is GameItemElement gameItemElement && gameItemElement.IsDestroyed())
		{
			return null;
		}
		GraphNodeStorage parentInventory = serviceGun.parentInventory;
		if (parentInventory == EmporiumEntry.Instance.backInvinvElement || parentInventory == EmporiumEntry.Instance.frontInvinvElement)
		{
			return serviceGun;
		}
		return null;
	}

	public void ReleaseServiceGun(bool anywhere = false)
	{
		GameItem gameItem = (anywhere ? serviceGun : GetPlacedServiceGun());
		if (gameItem == null || (gameItem is GameItemElement gameItemElement && gameItemElement.IsDestroyed()))
		{
			serviceGun = null;
			return;
		}
		GunHelper.ReturnPlayerParts(gameItem);
		gameItem.Destroy();
		serviceGun = null;
	}

	public static int GetPartsValue(GameItem gun)
	{
		int num = GunHelper.GetServiceRepairValue(gun);
		foreach (GameItem playerPart in GunHelper.GetPlayerParts(gun))
		{
			num += (int)playerPart.GetCurrentValue(useRetailMarkup: false, withChild: false, includeEvents: true, forceMarkup: true, 0L);
		}
		return num;
	}

	public static GunOrderData CreateBasicOrder()
	{
		return new GunOrderData
		{
			tier = 1,
			baseReward = 86 + RNG.GetRandomInt(0, 10),
			mandatoryRequirements = { Req("GUN_CURRENT_FIREPOWER_TAG", 17) },
			dialogText = RequestText("dialog_gun_order_req_accurate", "Need something accurate."),
			bonusRequirements = 
			{
				Req("GUN_CURRENT_ACCURACY_TAG", 20, 46),
				Req("GUN_CURRENT_ACCURACY_TAG", 24, 46),
				Req("GUN_CURRENT_ACCURACY_TAG", 30, 46),
				Req("GUN_CURRENT_RELIABILITY_TAG", 24, 46)
			}
		};
	}

	public static GunOrderData CreateBasicOrderFirepower()
	{
		return new GunOrderData
		{
			tier = 1,
			baseReward = 63 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_firepower", "Need something with serious firepower."),
			mandatoryRequirements = { Req("GUN_CURRENT_FIREPOWER_TAG", 11) },
			bonusRequirements = 
			{
				Req("GUN_CURRENT_FIREPOWER_TAG", 14, 34),
				Req("GUN_CURRENT_FIREPOWER_TAG", 17, 40),
				Req("GUN_CURRENT_FIREPOWER_TAG", 20, 40),
				Req("GUN_CURRENT_FIREPOWER_TAG", 22, 40)
			}
		};
	}

	public static GunOrderData CreateBasicOrderBig()
	{
		return new GunOrderData
		{
			tier = 1,
			baseReward = 98 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_big", "Need something big and intimidating."),
			mandatoryRequirements = { Req("GUN_CURRENT_FIREPOWER_TAG", 11) },
			bonusRequirements = 
			{
				Req("GUN_SIZE_TAG", 10, 12),
				Req("GUN_SIZE_TAG", 12, 17),
				Req("GUN_SIZE_TAG", 14, 23)
			}
		};
	}

	public static GunOrderData CreateBasicOrderSuppressed()
	{
		return new GunOrderData
		{
			tier = 1,
			baseReward = 86 + RNG.GetRandomInt(0, 10),
			mandatoryRequirements = { Req("GUN_CURRENT_FIREPOWER_TAG", 17) },
			dialogText = RequestText("dialog_gun_order_req_suppressed", "Got something accurate? Maybe a suppressed one?"),
			bonusRequirements = 
			{
				Req("GUN_CURRENT_ACCURACY_TAG", 20, 46),
				Req("GUN_CURRENT_ACCURACY_TAG", 24, 46),
				Req("GUN_CURRENT_ACCURACY_TAG", 30, 46),
				Req("GUN_CURRENT_RELIABILITY_TAG", 24, 46),
				Req("GUN_SUPPRESSED_TAG", 0, 22),
				Req("GUN_SUPPRESSED_TAG", 1, 53)
			}
		};
	}

	public static GunOrderData CreateBasicOrderStylish()
	{
		return new GunOrderData
		{
			tier = 1,
			baseReward = 69 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_stylish", "I need something stylish."),
			mandatoryRequirements = { Req("GUN_CURRENT_STYLE_TAG", 4) },
			bonusRequirements = 
			{
				Req("GUN_CURRENT_STYLE_TAG", 7, 34),
				Req("GUN_CURRENT_STYLE_TAG", 14, 63),
				Req("GUN_CURRENT_STYLE_TAG", 17, 57),
				Req("GUN_CURRENT_STYLE_TAG", 20, 52),
				Req("GUN_CURRENT_STYLE_TAG", 23, 46)
			}
		};
	}

	public static GunOrderData CreateBasicOrderGeneral()
	{
		return new GunOrderData
		{
			tier = 1,
			baseReward = 92 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_general_basic", "Nothing fancy. A gun that shoots straight and does not jam."),
			mandatoryRequirements = 
			{
				Req("GUN_CURRENT_FIREPOWER_TAG", 15),
				Req("GUN_CURRENT_RELIABILITY_TAG", 23),
				Req("GUN_CURRENT_ACCURACY_TAG", 23)
			},
			bonusRequirements = 
			{
				Req("GUN_CURRENT_FIREPOWER_TAG", 21, 29),
				Req("GUN_CURRENT_STYLE_TAG", 17, 29),
				Req("GUN_CURRENT_RELIABILITY_TAG", 32, 34),
				Req("GUN_CURRENT_RECOIL_TAG", 69, 29),
				Req("GUN_CURRENT_ERGONOMICS_TAG", 54, 29),
				Req("GUN_CURRENT_ACCURACY_TAG", 32, 34),
				Req("GUN_SIZE_TAG", 11, 23),
				Req("GUN_SUPPRESSED_TAG", 0, 17),
				Req("GUN_SUPPRESSED_TAG", 1, 40)
			}
		};
	}

	public static GunOrderData CreateBasicOrderHandling()
	{
		return new GunOrderData
		{
			tier = 1,
			baseReward = 100 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_handling", "I want something that handles well, steady and easy in the hand."),
			mandatoryRequirements = 
			{
				Req("GUN_CURRENT_ERGONOMICS_TAG", 44),
				Req("GUN_CURRENT_RECOIL_TAG", 54)
			},
			bonusRequirements = 
			{
				Req("GUN_CURRENT_ERGONOMICS_TAG", 59, 35),
				Req("GUN_CURRENT_ERGONOMICS_TAG", 69, 40),
				Req("GUN_CURRENT_RECOIL_TAG", 74, 35),
				Req("GUN_CURRENT_RECOIL_TAG", 84, 40),
				Req("GUN_CURRENT_ACCURACY_TAG", 29, 25)
			}
		};
	}

	public static GunOrderData CreateMidOrderSuppressed()
	{
		return new GunOrderData
		{
			tier = 2,
			baseReward = 190 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_reliable_suppressed", "I need something reliable, bonus if it's suppressed."),
			mandatoryRequirements = { Req("GUN_CURRENT_RELIABILITY_TAG", 32) },
			bonusRequirements = 
			{
				Req("GUN_CURRENT_RELIABILITY_TAG", 39, 37),
				Req("GUN_CURRENT_RELIABILITY_TAG", 41, 45),
				Req("GUN_CURRENT_RELIABILITY_TAG", 45, 50),
				Req("GUN_CURRENT_RELIABILITY_TAG", 47, 50),
				Req("GUN_SUPPRESSED_TAG", 0, 27),
				Req("GUN_SUPPRESSED_TAG", 1, 63)
			}
		};
	}

	public static GunOrderData CreateMidOrderStylish()
	{
		return new GunOrderData
		{
			tier = 2,
			baseReward = 190 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_stylish", "I need something stylish."),
			mandatoryRequirements = { Req("GUN_CURRENT_STYLE_TAG", 25) },
			bonusRequirements = 
			{
				Req("GUN_CURRENT_STYLE_TAG", 31, 25),
				Req("GUN_CURRENT_STYLE_TAG", 34, 30),
				Req("GUN_CURRENT_STYLE_TAG", 41, 35),
				Req("GUN_CURRENT_STYLE_TAG", 47, 45),
				Req("GUN_CURRENT_STYLE_TAG", 51, 55)
			}
		};
	}

	public static GunOrderData CreateMidOrderBig()
	{
		return new GunOrderData
		{
			tier = 2,
			baseReward = 190 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_bigger", "I need something intimidating, the bigger the better."),
			mandatoryRequirements = { Req("GUN_CURRENT_FIREPOWER_TAG", 25) },
			bonusRequirements = 
			{
				Req("GUN_SIZE_TAG", 10, 40),
				Req("GUN_SIZE_TAG", 11, 45),
				Req("GUN_SIZE_TAG", 12, 50),
				Req("GUN_SIZE_TAG", 13, 55)
			}
		};
	}

	public static GunOrderData CreateMidOrderFirepower()
	{
		return new GunOrderData
		{
			tier = 2,
			baseReward = 190 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_firepower", "Need something with serious firepower."),
			mandatoryRequirements = { Req("GUN_CURRENT_FIREPOWER_TAG", 26) },
			bonusRequirements = 
			{
				Req("GUN_CURRENT_FIREPOWER_TAG", 28, 40),
				Req("GUN_CURRENT_FIREPOWER_TAG", 32, 45),
				Req("GUN_CURRENT_FIREPOWER_TAG", 35, 50),
				Req("GUN_CURRENT_FIREPOWER_TAG", 38, 55)
			}
		};
	}

	public static GunOrderData CreateMidOrderGeneral()
	{
		return new GunOrderData
		{
			tier = 2,
			baseReward = 200 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_general_mid", "I want a proper sidearm, well rounded."),
			mandatoryRequirements = 
			{
				Req("GUN_CURRENT_FIREPOWER_TAG", 25),
				Req("GUN_CURRENT_RELIABILITY_TAG", 29),
				Req("GUN_CURRENT_ACCURACY_TAG", 33)
			},
			bonusRequirements = 
			{
				Req("GUN_CURRENT_FIREPOWER_TAG", 35, 40),
				Req("GUN_CURRENT_STYLE_TAG", 37, 35),
				Req("GUN_CURRENT_RELIABILITY_TAG", 41, 40),
				Req("GUN_CURRENT_RECOIL_TAG", 39, 35),
				Req("GUN_CURRENT_ERGONOMICS_TAG", 49, 35),
				Req("GUN_CURRENT_ACCURACY_TAG", 47, 40),
				Req("GUN_SIZE_TAG", 11, 30),
				Req("GUN_SUPPRESSED_TAG", 0, 22),
				Req("GUN_SUPPRESSED_TAG", 1, 53)
			}
		};
	}

	public static GunOrderData CreateMidOrderHandling()
	{
		return new GunOrderData
		{
			tier = 2,
			baseReward = 200 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_handling", "I want something that handles well, steady and easy in the hand."),
			mandatoryRequirements = 
			{
				new GunOrderRequirement
				{
					id = "GUN_CALIBER_TAG",
					value = 10,
					isCaliber = true
				},
				Req("GUN_CURRENT_ERGONOMICS_TAG", 39),
				Req("GUN_CURRENT_RECOIL_TAG", 29)
			},
			bonusRequirements = 
			{
				Req("GUN_CURRENT_ERGONOMICS_TAG", 49, 35),
				Req("GUN_CURRENT_ERGONOMICS_TAG", 59, 40),
				Req("GUN_CURRENT_RECOIL_TAG", 34, 35),
				Req("GUN_CURRENT_RECOIL_TAG", 44, 40),
				Req("GUN_CURRENT_RECOIL_TAG", 52, 45)
			}
		};
	}

	public static GunOrderData CreateHighOrderFirepower()
	{
		return new GunOrderData
		{
			tier = 3,
			baseReward = 350 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_firepower", "Need something with serious firepower."),
			mandatoryRequirements = { Req("GUN_CURRENT_FIREPOWER_TAG", 59) },
			bonusRequirements = 
			{
				Req("GUN_CURRENT_FIREPOWER_TAG", 64, 65),
				Req("GUN_CURRENT_FIREPOWER_TAG", 69, 70),
				Req("GUN_CURRENT_FIREPOWER_TAG", 74, 75),
				Req("GUN_CURRENT_FIREPOWER_TAG", 79, 80)
			}
		};
	}

	public static GunOrderData CreateHighOrderStyle()
	{
		return new GunOrderData
		{
			tier = 3,
			baseReward = 350 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_stylish", "I need something stylish."),
			mandatoryRequirements = { Req("GUN_CURRENT_STYLE_TAG", 52) },
			bonusRequirements = 
			{
				Req("GUN_CURRENT_STYLE_TAG", 55, 65),
				Req("GUN_CURRENT_STYLE_TAG", 58, 70),
				Req("GUN_CURRENT_STYLE_TAG", 63, 75),
				Req("GUN_CURRENT_STYLE_TAG", 68, 80)
			}
		};
	}

	public static GunOrderData CreateHighOrderBig()
	{
		return new GunOrderData
		{
			tier = 3,
			baseReward = 350 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_big", "Need something big and intimidating."),
			mandatoryRequirements = { Req("GUN_SIZE_TAG", 16) },
			bonusRequirements = 
			{
				Req("GUN_SIZE_TAG", 17, 80),
				Req("GUN_SIZE_TAG", 19, 85),
				Req("GUN_SIZE_TAG", 22, 90),
				Req("GUN_SIZE_TAG", 25, 95)
			}
		};
	}

	public static GunOrderData CreateHighOrderReliability()
	{
		return new GunOrderData
		{
			tier = 3,
			baseReward = 350 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_reliable_suppressed", "I need something reliable, bonus if it's suppressed."),
			mandatoryRequirements = { Req("GUN_CURRENT_RELIABILITY_TAG", 59) },
			bonusRequirements = 
			{
				Req("GUN_CURRENT_RELIABILITY_TAG", 64, 75),
				Req("GUN_CURRENT_RELIABILITY_TAG", 67, 80),
				Req("GUN_CURRENT_RELIABILITY_TAG", 70, 95),
				Req("GUN_CURRENT_RELIABILITY_TAG", 74, 100),
				Req("GUN_SUPPRESSED_TAG", 0, 33),
				Req("GUN_SUPPRESSED_TAG", 1, 77)
			}
		};
	}

	public static GunOrderData CreateHighOrderHandling()
	{
		return new GunOrderData
		{
			tier = 3,
			baseReward = 360 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_handling", "I want something that handles well, steady and easy in the hand."),
			mandatoryRequirements = 
			{
				new GunOrderRequirement
				{
					id = "GUN_CALIBER_TAG",
					value = 10,
					isCaliber = true
				},
				Req("GUN_CURRENT_ERGONOMICS_TAG", 54),
				Req("GUN_CURRENT_RECOIL_TAG", 24)
			},
			bonusRequirements = 
			{
				Req("GUN_CURRENT_ERGONOMICS_TAG", 69, 60),
				Req("GUN_CURRENT_ERGONOMICS_TAG", 79, 65),
				Req("GUN_CURRENT_RECOIL_TAG", 39, 60),
				Req("GUN_CURRENT_RECOIL_TAG", 54, 65),
				Req("GUN_CURRENT_RECOIL_TAG", 69, 70)
			}
		};
	}

	public static GunOrderData CreateHighOrderGeneral()
	{
		return new GunOrderData
		{
			tier = 3,
			baseReward = 360 + RNG.GetRandomInt(0, 10),
			dialogText = RequestText("dialog_gun_order_req_general_high", "A serious piece, no weak spots. Every detail you get right adds to the price."),
			mandatoryRequirements = 
			{
				Req("GUN_CURRENT_FIREPOWER_TAG", 57),
				Req("GUN_CURRENT_RELIABILITY_TAG", 57),
				Req("GUN_CURRENT_ACCURACY_TAG", 44)
			},
			bonusRequirements = 
			{
				Req("GUN_CURRENT_FIREPOWER_TAG", 74, 65),
				Req("GUN_CURRENT_STYLE_TAG", 64, 65),
				Req("GUN_CURRENT_RELIABILITY_TAG", 71, 70),
				Req("GUN_CURRENT_RECOIL_TAG", 39, 60),
				Req("GUN_CURRENT_ERGONOMICS_TAG", 69, 60),
				Req("GUN_CURRENT_ACCURACY_TAG", 61, 65),
				Req("GUN_SIZE_TAG", 19, 60),
				Req("GUN_SUPPRESSED_TAG", 0, 30),
				Req("GUN_SUPPRESSED_TAG", 1, 70)
			}
		};
	}
}

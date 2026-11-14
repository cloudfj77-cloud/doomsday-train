using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreClientListGun
{
	[ES3NonSerializable]
	private static readonly LocalizedLines CustomGunOrderMessages = LocalizedLines.Dialogue("dialog_gun_order_1", "dialog_gun_order_2", "dialog_gun_order_3", "dialog_gun_order_4", "dialog_gun_order_5");

	private static readonly LocalizedLines SellGunDialogMessages = LocalizedLines.Dialogue("dialog_gun_sell_1", "dialog_gun_sell_2", "dialog_gun_sell_3", "dialog_gun_sell_4", "dialog_gun_sell_5", "dialog_gun_sell_6");

	public const int FOLLOW_UP_AMMO_BASE_CHANCE = 50;

	public const int FOLLOW_UP_MELEE_BASE_CHANCE = 40;

	public const int FOLLOW_UP_MUNITION_BASE_CHANCE = 30;

	public const int FOLLOW_UP_CHANCE_PER_BONUS = 5;

	private static readonly int[] MUNITION_BUDGET = new int[4] { 0, 0, 150, 350 };

	public const int AMMO_BUDGET_PERCENT_OF_GUN_VALUE = 33;

	private static readonly int[] MELEE_BUDGET = new int[4] { 0, 60, 120, 250 };

	public static string GetRandomCustomGunOrderMessage()
	{
		int randomIntExculsive = RNG.GetRandomIntExculsive(0, CustomGunOrderMessages.Length);
		return CustomGunOrderMessages[randomIntExculsive];
	}

	public static string GetRandomSellGunDialogMessage()
	{
		int randomIntExculsive = RNG.GetRandomIntExculsive(0, SellGunDialogMessages.Length);
		return SellGunDialogMessages[randomIntExculsive];
	}

	public static StoreClient CreateLLGunBuyer()
	{
		return CreateLLGunBuyer(GetRandomBasicOrder());
	}

	public static StoreClient CreateLLGunBuyer(GunOrderData order)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "ll_gun_buyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_citizen");
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.GUNSMITH;
		storeClient.gunOrderData = order;
		storeClient.isBargainAvailable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, GetRandomCustomGunOrderMessage()).NextDialogue().SetText(storeClient.displayName, storeClient.gunOrderData.dialogText)
			.SetEndAction(delegate
			{
				OnOrderGreetingEnd(storeClient);
			});
		storeClient.tryInterceptDismiss = () => TryStayForMeleeWeapon(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMercGunBuyer()
	{
		return CreateMercGunBuyer(GetRandomMidOrder());
	}

	public static StoreClient CreateMercGunBuyer(GunOrderData order)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "merc_gun_buyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_mercenary");
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomMercSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.GUNSMITH;
		storeClient.gunOrderData = order;
		storeClient.isBargainAvailable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, GetRandomCustomGunOrderMessage()).NextDialogue().SetText(storeClient.displayName, storeClient.gunOrderData.dialogText)
			.SetEndAction(delegate
			{
				OnOrderGreetingEnd(storeClient);
			});
		storeClient.tryInterceptDismiss = () => TryStayForMeleeWeapon(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateULBuyer()
	{
		return CreateULBuyer(GetRandomHighOrder());
	}

	public static StoreClient CreateULBuyer(GunOrderData order)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "ul_gun_buyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_level_citizen");
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomUpperSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.GUNSMITH;
		storeClient.gunOrderData = order;
		storeClient.isBargainAvailable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, GetRandomCustomGunOrderMessage()).NextDialogue().SetText(storeClient.displayName, storeClient.gunOrderData.dialogText)
			.SetEndAction(delegate
			{
				OnOrderGreetingEnd(storeClient);
			});
		storeClient.tryInterceptDismiss = () => TryStayForMeleeWeapon(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateStoryGunBuyer()
	{
		StoreClient storeClient = CreateULBuyer();
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_revolutionary_fighter");
		StoreClientFactionSetup.InitRev(storeClient);
		storeClient.spriteName = "trailer_rev1";
		return storeClient;
	}

	private static void OnOrderGreetingEnd(StoreClient storeClient)
	{
		GameItem gameItem = storeClient.gunOrderData?.CreateServiceGun();
		if (gameItem != null)
		{
			StoreClient storeClient2 = storeClient;
			storeClient2.OnDismiss = (Action)Delegate.Combine(storeClient2.OnDismiss, (Action)delegate
			{
				storeClient.gunOrderData?.ReleaseServiceGun(anywhere: true);
			});
			GeneralHelper.SetItemOwned(gameItem, own: false);
			if (GraphUtils.TryAcceptAllMid(EmporiumEntry.Instance.frontInvinvElement, gameItem) <= 0)
			{
				PlayerStore.Instance.AddDirectToWeightedTable(gameItem);
			}
		}
		NegociationUIManager.Instance.OpenUI();
	}

	public static GunOrderData GetRandomBasicOrder()
	{
		if (GunOrderData.ShouldFavorServiceOrder(1))
		{
			return GunOrderData.CreateServiceOrder(1);
		}
		return RNG.GetRandomIntExculsive(0, 9) switch
		{
			8 => GunOrderData.CreateServiceOrder(1), 
			0 => GunOrderData.CreateBasicOrder(), 
			1 => GunOrderData.CreateBasicOrderSuppressed(), 
			2 => GunOrderData.CreateBasicOrderBig(), 
			3 => GunOrderData.CreateBasicOrderFirepower(), 
			4 => GunOrderData.CreateBasicOrderStylish(), 
			5 => GunOrderData.CreateBasicOrderGeneral(), 
			6 => GunOrderData.CreateBasicOrderHandling(), 
			_ => GunOrderData.CreateBestOrder(1, 22), 
		};
	}

	public static GunOrderData GetRandomMidOrder()
	{
		if (GunOrderData.ShouldFavorServiceOrder(2))
		{
			return GunOrderData.CreateServiceOrder(2);
		}
		return RNG.GetRandomIntExculsive(0, 8) switch
		{
			7 => GunOrderData.CreateServiceOrder(2), 
			0 => GunOrderData.CreateMidOrderBig(), 
			1 => GunOrderData.CreateMidOrderSuppressed(), 
			2 => GunOrderData.CreateMidOrderStylish(), 
			3 => GunOrderData.CreateMidOrderFirepower(), 
			4 => GunOrderData.CreateMidOrderGeneral(), 
			5 => GunOrderData.CreateMidOrderHandling(), 
			_ => GunOrderData.CreateBestOrder(2, 10), 
		};
	}

	public static GunOrderData GetRandomHighOrder()
	{
		if (GunOrderData.ShouldFavorServiceOrder(3))
		{
			return GunOrderData.CreateServiceOrder(3);
		}
		return RNG.GetRandomIntExculsive(0, 8) switch
		{
			7 => GunOrderData.CreateServiceOrder(3), 
			0 => GunOrderData.CreateHighOrderBig(), 
			1 => GunOrderData.CreateHighOrderFirepower(), 
			2 => GunOrderData.CreateHighOrderReliability(), 
			3 => GunOrderData.CreateHighOrderStyle(), 
			4 => GunOrderData.CreateHighOrderGeneral(), 
			5 => GunOrderData.CreateHighOrderHandling(), 
			_ => GunOrderData.CreateBestOrder(3, 10), 
		};
	}

	public static int GetMunitionBudget(int tier)
	{
		return MUNITION_BUDGET[Mathf.Clamp(tier, 1, MUNITION_BUDGET.Length - 1)];
	}

	public static int GetAmmoBudget(GameItem deliveredGun)
	{
		if (deliveredGun == null)
		{
			return 0;
		}
		return Mathf.RoundToInt((float)(deliveredGun.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L) * 33) / 100f);
	}

	public static int GetMeleeBudget(int tier)
	{
		return MELEE_BUDGET[Mathf.Clamp(tier, 1, MELEE_BUDGET.Length - 1)];
	}

	public static Dialogue CreateDeliveryFollowUp(StoreClient storeClient, GameItem gun)
	{
		if (storeClient == null || gun == null)
		{
			return null;
		}
		int num = ((storeClient.gunOrderData == null) ? 1 : storeClient.gunOrderData.tier);
		int num2 = ((storeClient.gunOrderData != null) ? storeClient.gunOrderData.CountBonusMet(gun) : 0) * 5;
		bool flag = RNG.Roll(50 + num2);
		bool flag2 = RNG.Roll(40 + num2);
		bool flag3 = num >= 2 && RNG.Roll(30 + num2);
		int num3 = (gun.IsTag("GUN_CALIBER_TAG") ? gun.GetTagReadonly("GUN_CALIBER_TAG").GetInt() : 0);
		if (num3 == 0)
		{
			flag = false;
		}
		if (!flag && !flag2 && !flag3)
		{
			return null;
		}
		int budget = (flag ? GetAmmoBudget(gun) : 0) + (flag2 ? GetMeleeBudget(num) : 0) + (flag3 ? GetMunitionBudget(num) : 0);
		ConvertToBuyer(storeClient, num, budget, flag ? num3 : 0, flag2, flag3);
		List<string> list = new List<string>();
		list.Add(LocHelper.GetLocalizedDialogue("dialog_water_scavenger_right_item"));
		if (flag)
		{
			list.Add(LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_gun_order_followup_want_ammo", "Got {0} ammo for it?", GunHelper.GetCaliberName(num3)));
		}
		if (flag2)
		{
			list.Add(LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_gun_order_followup_want_melee", "I also want a backup for close quarters."));
		}
		if (flag3)
		{
			list.Add(LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_gun_order_followup_want_munition", "Got any {0} or explosive?", TypeHelper.GetTypeDisplayName("MUNITION")));
		}
		list.Add(LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_gun_order_followup_budget", "I can spend {0} more.", storeClient.GetBudget()));
		string text = string.Join(" ", list);
		Dialogue dialogue = new Dialogue().SetText(storeClient.displayName, StoreClientDialogList.GetRandomDialogAcceptedString());
		dialogue.priority = 4;
		dialogue.NextDialogue().SetText(storeClient.displayName, text);
		return dialogue;
	}

	public static bool TryStayForMeleeWeapon(StoreClient storeClient)
	{
		if (storeClient == null || storeClient.clientIntent != StoreClient.ClientIntent.GUNSMITH || storeClient.isBusinessComplete)
		{
			return false;
		}
		int tier = ((storeClient.gunOrderData == null) ? 1 : storeClient.gunOrderData.tier);
		storeClient.gunOrderData?.ReleaseServiceGun();
		ConvertToBuyer(storeClient, tier, GetMeleeBudget(tier), 0, wantsMelee: true, wantsMunition: false);
		Dialogue dialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_gun_order_stay_melee", "Hold on. Got a melee weapon? I can spend {0}.", storeClient.GetBudget()));
		dialogue.priority = 4;
		DialogUIManager.Instance.StartDialogue(dialogue, forcedStart: true);
		return true;
	}

	private static void ConvertToBuyer(StoreClient storeClient, int tier, int budget, int ammoCaliber, bool wantsMelee, bool wantsMunition)
	{
		storeClient.tryInterceptDismiss = null;
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(budget);
		storeClient.noAcceptingContraband = false;
		storeClient.clientBuyingTagList.Clear();
		if (wantsMunition)
		{
			storeClient.clientBuyingTagList.Add("MUNITION");
		}
		if (wantsMelee)
		{
			storeClient.clientBuyingTagList.Add("MELEE_WEAPON");
			if (tier >= 2 && !storeClient.clientBlackTagList.Contains("JUNK"))
			{
				storeClient.clientBlackTagList.Add("JUNK");
			}
			if (tier >= 3)
			{
				if (!storeClient.clientBlackTagList.Contains("TOOL"))
				{
					storeClient.clientBlackTagList.Add("TOOL");
				}
				if (!storeClient.clientBlackTagList.Contains("HOUSEHOLD_GOOD"))
				{
					storeClient.clientBlackTagList.Add("HOUSEHOLD_GOOD");
				}
			}
		}
		if (ammoCaliber != 0)
		{
			storeClient.willAcceptItem = (GameItem item) => GunHelper.IsAmmunitionOfCaliber(item, ammoCaliber);
		}
		else
		{
			storeClient.willAcceptItem = null;
		}
		NegociationUIManager.Instance.RefreshUI();
	}

	public static StoreClient CreateLLGunSeller()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "ll_gun_seller";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_citizen");
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.gunOrderData = GetRandomBasicOrder();
		storeClient.clientItemFeatureBuying.Add("discountedBuy20");
		storeClient.mainDialogue.SetText(storeClient.displayName, GetRandomSellGunDialogMessage()).SetEndAction(delegate
		{
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHandmadeGun(ItemFeatureList.EquipmentCondition.Poor));
			}
			else
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHandmadeGun(ItemFeatureList.EquipmentCondition.Fair));
			}
			if (RNG.Roll(40))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_22");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_22_high");
			}
			if (RNG.Roll(20))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_22_ext");
			}
			if (RNG.Roll(20))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("laser");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("laser_toy");
			}
			if (RNG.Roll(20))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("rds");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("rds_makeshift");
			}
			if (RNG.Roll(20))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("silencer");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("silencer_makeshift");
			}
			if (RNG.Roll(12))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("pistol_switch");
			}
			if (RNG.Roll(12))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("internal_springs");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("internal_springs_improved");
			}
			if (RNG.Roll(12))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("trigger_group");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo_nl");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo_nl");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo_nl");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo_nl");
			}
			if (RNG.Roll(12))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo");
			}
			if (RNG.Roll(12))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo");
			}
			if (RNG.Roll(12))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo");
			}
			if (RNG.Roll(12))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo");
			}
			if (RNG.Roll(10))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo_p");
			}
			if (RNG.Roll(10))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo_p");
			}
			if (RNG.Roll(10))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo_p");
			}
			if (RNG.Roll(10))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo_p");
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMercGunSeller()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "merc_gun_seller";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_mercenary");
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomMercSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.gunOrderData = GetRandomBasicOrder();
		storeClient.clientItemFeatureBuying.Add("discountedBuy20");
		storeClient.mainDialogue.SetText(storeClient.displayName, GetRandomSellGunDialogMessage()).SetEndAction(delegate
		{
			ItemFeatureList.EquipmentCondition equipmentCondition = (RNG.Roll(50) ? ItemFeatureList.EquipmentCondition.Poor : ItemFeatureList.EquipmentCondition.Fair);
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRevolver(equipmentCondition));
			}
			else
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHeavyHandmadeGun(equipmentCondition));
			}
			if (RNG.Roll(40))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_high");
			}
			if (RNG.Roll(20))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_ext");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("laser");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("rds");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("silencer");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("internal_springs");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("internal_springs_improved");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("trigger_group");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("match_barrel");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("pistol_switch");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("ammo_display");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("grip_gel");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_nl");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_nl");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_nl");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_nl");
			}
			if (RNG.Roll(12))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
			}
			if (RNG.Roll(12))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
			}
			if (RNG.Roll(12))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
			}
			if (RNG.Roll(12))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
			}
			if (RNG.Roll(10))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_p");
			}
			if (RNG.Roll(10))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_p");
			}
			if (RNG.Roll(10))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_p");
			}
			if (RNG.Roll(10))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_p");
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateULGunSeller()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "ul_gun_seller";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_level_citizen");
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomMercSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.gunOrderData = GetRandomBasicOrder();
		storeClient.clientItemFeatureBuying.Add("discountedBuy20");
		storeClient.mainDialogue.SetText(storeClient.displayName, GetRandomSellGunDialogMessage()).SetEndAction(delegate
		{
			if (RNG.Roll(75))
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceHandgun(ItemFeatureList.EquipmentCondition.Poor));
				}
				else
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceHandgun(ItemFeatureList.EquipmentCondition.Fair));
				}
			}
			else
			{
				GameItem gameItem = PreBuiltItemHelper.CreatePoliceSMG(RNG.Roll(50) ? ItemFeatureList.EquipmentCondition.Poor : ItemFeatureList.EquipmentCondition.Fair);
				if (RNG.Roll(30))
				{
					PreBuiltItemHelper.FitGunMod(gameItem, "bipod");
				}
				if (RNG.Roll(20))
				{
					PreBuiltItemHelper.FitGunMod(gameItem, "mag_10mm_smg_box");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
			}
			if (RNG.Roll(40))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_high");
			}
			if (RNG.Roll(18))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_ext");
			}
			if (RNG.Roll(40))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_smg");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_smg_high");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("laser");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("laser2");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("rds");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("silencer");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("silencer2");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("compensator");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("internal_springs");
			}
			if (RNG.Roll(30))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("internal_springs_improved");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("trigger_group");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("match_barrel");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("pistol_switch");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("ammo_display");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("grip_gel");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
			}
			if (RNG.Roll(13))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_p");
			}
			if (RNG.Roll(13))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_p");
			}
			if (RNG.Roll(13))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_p");
			}
			if (RNG.Roll(13))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_p");
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

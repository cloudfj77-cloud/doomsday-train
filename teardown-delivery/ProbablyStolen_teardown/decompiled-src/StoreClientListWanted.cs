using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreClientListWanted
{
	public static readonly HashSet<string> POSTER_IDS = new HashSet<string> { "wanted1", "wanted2", "wanted3", "wanted4", "wanted5", "wanted6", "wanted7", "wanted8", "wanted9" };

	public static Dictionary<string, Func<StoreClient>> wantedClientList = new Dictionary<string, Func<StoreClient>>
	{
		{ "wanted1", CreateWanted1 },
		{ "wanted2", CreateWanted2 },
		{ "wanted3", CreateWanted3 },
		{ "wanted4", CreateWanted4 },
		{ "wanted4Normal", CreateWanted4Normal },
		{ "wanted6", CreateWanted6 },
		{ "wanted7", CreateWanted7 },
		{ "wanted8", CreateWanted8 },
		{ "wanted9", CreateWanted9 }
	};

	public const int BAILEY_WHOLESALE = -33;

	public const int BAILEY_RETURN_ROLL = 35;

	public const int BUTCHER_RETURN_WHOLESALE = -20;

	public const int ARTHUR_SPARED_REVOLUTION_REP = 15;

	public const int LI_FINALE_NOMORE_INJECTORS = 2;

	public const int LI_FORMULA_NOTE_VALUE = 400;

	public static StoreClient CreateWanted1()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted1";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_citizen");
		storeClient.spriteName = "wanted1";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-33);
		storeClient.SetWanted(300, LocHelper.GetLocalizedClientName("name_james_bailey"));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted1_1")).SetStartAction(delegate
		{
			if (PlayerStore.Instance.storeClientData.jamesBaileyMetDay < 0)
			{
				PlayerStore.Instance.storeClientData.jamesBaileyMetDay = StoreStation.Instance.dayCounter;
			}
		}).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted1_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted1_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("stun_baton");
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceHandgun(ItemFeatureList.EquipmentCondition.Fair));
				PlayerStore.Instance.AddDirectSellingItemToTable("sec_keycard");
			});
		storeClient.tryInterceptReport = delegate
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted1_plea_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted1_plea_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted1_plea_3"));
			DialogUIManager.Instance.StartDialogue(dialogue);
			return true;
		};
		SetBaileyArrestDialogue(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWanted1Return()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted1_return";
		storeClient.wantedPosterId = "wanted1";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_citizen");
		storeClient.spriteName = "wanted1";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-33);
		storeClient.SetWanted(300, LocHelper.GetLocalizedClientName("name_james_bailey"));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted1_return_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted1_return_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted1_return_3"))
			.SetEndAction(delegate
			{
				int chance = Math.Min(100, 35 + StoreStation.Instance.dayCounter);
				PlayerStore.Instance.AddDirectSellingItemToTable("stun_baton");
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceHandgun(ItemFeatureList.EquipmentCondition.Fair));
				}
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("sec_keycard");
				}
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(RNG.Roll(50) ? PreBuiltItemHelper.LootCrateSecurity() : PreBuiltItemHelper.LootCrateEvidence());
				}
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("stun_gun");
				}
				int randomInt = RNG.GetRandomInt(3, 6);
				for (int i = 0; i < randomInt; i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
				}
			});
		SetBaileyArrestDialogue(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static void SetBaileyArrestDialogue(StoreClient storeClient)
	{
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_wanted1_arrest_client")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_wanted1_arrest_sec_1"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_wanted1_arrest_sec_2"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
	}

	public static StoreClient CreateWanted2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_citizen");
		storeClient.spriteName = "wanted2";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-25);
		storeClient.SetWanted(450, LocHelper.GetLocalizedClientName("name_the_butcher"));
		storeClient.reportImprovesFaction = true;
		SetupButcherSurvivesShot(storeClient);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted2_1")).SetStartAction(delegate
		{
			if (PlayerStore.Instance.storeClientData.butcherMetDay < 0)
			{
				PlayerStore.Instance.storeClientData.butcherMetDay = StoreStation.Instance.dayCounter;
			}
		}).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted2_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted2_3"))
			.SetEndAction(delegate
			{
				GameItem gameItem = DirectoryMaster.Item("satchel");
				GameItem gameItem2 = DirectoryMaster.Item("postit");
				gameItem2.name = LocHelper.GetLocalizedItem("item_scav_journal_name");
				gameItem2.shortDescription = "";
				gameItem2.flavorText = LocHelper.GetLocalizedItem("item_scav_journal_flavor");
				gameItem2.unitBaseValue = 0L;
				GraphUtils.TryAcceptAll(gameItem, gameItem2);
				GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("processed_meat"));
				GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("common_electronic"));
				GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("scav_token"));
				GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_drink"));
				PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
				PlayerStore.Instance.AddDirectSellingItemToTable("crowbar");
				PlayerStore.Instance.AddDirectSellingItemToTable("raw_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("small_raw_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("small_raw_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("fat_meat");
			});
		if (!PlayerStore.Instance.storeClientData.isButcherReportWarningDone)
		{
			storeClient.tryInterceptReport = delegate
			{
				PlayerStore.Instance.storeClientData.isButcherReportWarningDone = true;
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted2_warning_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted2_warning_2"))
					.NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted2_warning_3"));
				DialogUIManager.Instance.StartDialogue(dialogue);
				return true;
			};
		}
		SetButcherArrestDialogue(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWanted2Return()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted2_return";
		storeClient.wantedPosterId = "wanted2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_citizen");
		storeClient.spriteName = "wanted2";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-20);
		storeClient.SetWanted(450, LocHelper.GetLocalizedClientName("name_the_butcher"));
		storeClient.reportImprovesFaction = true;
		SetupButcherSurvivesShot(storeClient);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted2_return_1")).SetEndAction(delegate
		{
			int chance = Math.Min(100, (StoreStation.Instance.dayCounter * 5 + 1) / 2);
			int num = RollButcherCount(0, 3, chance);
			int num2 = RollButcherCount(1, 3, chance);
			int num3 = RollButcherCount(0, 2, chance);
			for (int i = 0; i < num; i++)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("raw_meat", isOwend: false, isStolen: true);
			}
			for (int j = 0; j < num2; j++)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_raw_meat", isOwend: false, isStolen: true);
			}
			for (int k = 0; k < num3; k++)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("fat_meat", isOwend: false, isStolen: true);
			}
		});
		SetButcherArrestDialogue(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static GameItem CreateButcherNote()
	{
		return PreBuiltItemHelper.CreateHandnote(LocHelper.GetLocalizedMechanic("note_butcher_cleaver_body_1") + "\n" + LocHelper.GetLocalizedMechanic("note_butcher_cleaver_body_2") + "\n" + LocHelper.GetLocalizedMechanic("note_butcher_cleaver_body_3"), "", LocHelper.GetLocalizedClientName("name_the_butcher"));
	}

	public static StoreClient CreateSawyerButcherOffer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "sawyerButcherOffer";
		StoreClientListStory.SetupSawyer(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_sawyer");
		storeClient.spriteName = "sawyer";
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		string localizedClientName = LocHelper.GetLocalizedClientName("name_the_butcher");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sawyer_butcher_1", localizedClientName)).SetStartAction(delegate
		{
			PlayerStore.Instance.storeClientData.isSawyerButcherOfferDone = true;
		}).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sawyer_butcher_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sawyer_butcher_3", 500))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sawyer_butcher_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	private static int RollButcherCount(int min, int max, int chance)
	{
		int num = min;
		for (int i = min; i < max; i++)
		{
			if (RNG.Roll(chance))
			{
				num++;
			}
		}
		return num;
	}

	private static void SetupButcherSurvivesShot(StoreClient storeClient)
	{
		storeClient.survivesShot = true;
		storeClient.shotSurvivedDialogue = delegate
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetPriority(7);
			dialogue.SetText("CLIENT", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_wanted2_shot_1", "ARGH! You'll pay for that")).SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
			return dialogue;
		};
	}

	private static void SetButcherArrestDialogue(StoreClient storeClient)
	{
		storeClient.onCaptureAction = delegate
		{
			PlayerStore.Instance.storeClientData.butcherArrestedDay = StoreStation.Instance.dayCounter;
		};
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_wanted2_arrest_client")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_wanted2_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
	}

	public static StoreClient CreateWanted3()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted3";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_citizen");
		storeClient.spriteName = "wanted3";
		storeClient.clientFaction = "FACTION_REVOLUTION";
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetWanted(100, LocHelper.GetLocalizedClientName("name_arthur_lafleur"));
		storeClient.SetBudget(125 + RNG.GetRandomInt(0, 25));
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted3_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted3_2"))
			.SetEndAction(delegate
			{
			});
		storeClient.onCaptureAction = delegate
		{
			StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListUnique.Wanted3CapturedEvent(), 1, IsKnown: false);
		};
		bool isSpared = false;
		storeClient.tryInterceptReport = delegate
		{
			isSpared = true;
			Dialogue dialogue = new Dialogue();
			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted3_plea_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted3_plea_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted3_plea_3"));
			DialogUIManager.Instance.StartDialogue(dialogue);
			storeClient.tryInterceptDismiss = delegate
			{
				DialogUIManager.Instance.StartDialogue(new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted3_thanks")).SetEndAction(delegate
				{
					LeaveIfStillHere(storeClient);
				}));
				return true;
			};
			return true;
		};
		storeClient.OnDismiss = delegate
		{
			if (isSpared && !storeClient.reported)
			{
				StoreReputation.ModReputation("FACTION_REVOLUTION", 15);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWanted4()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted4";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_odin");
		storeClient.spriteName = "wanted4";
		storeClient.clientFaction = "FACTION_REVOLUTION";
		storeClient.dismissable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.SetWanted(300, LocHelper.GetLocalizedClientName("name_odin"));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted4_1")).SetStartAction(delegate
		{
			storeClient.dismissable = true;
		}).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted4_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted4_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted4_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted4_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted4_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted4_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted4_8"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("card_rev"), isOwend: true);
				StorePhoneClient.GetRevMerchantClient().phoneState = StorePhoneClient.PhoneState.Regular;
			});
		storeClient.onCaptureAction = delegate
		{
			StorePhoneClient.GetRevMerchantClient().phoneState = StorePhoneClient.PhoneState.NotResponding;
		};
		storeClient.onReportAction = delegate
		{
			StorePhoneClient.GetRevMerchantClient().phoneState = StorePhoneClient.PhoneState.NotResponding;
		};
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wanted4_arrest_client")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_wanted4_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWanted4Normal()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted4Normal";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_odin");
		storeClient.spriteName = "wanted4";
		storeClient.clientFaction = "FACTION_REVOLUTION";
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetWanted(350, LocHelper.GetLocalizedClientName("name_odin"));
		storeClient.wantedPosterId = "wanted4";
		storeClient.SetBudget(PlayerStore.Instance.odinBudget);
		storeClient.CanExposeChem = true;
		storeClient.canClientExposeFeature = ClientCanExposeFunc.OdinExposeCapacity;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted4normal_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted4normal_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted4normal_3"))
			.SetEndAction(delegate
			{
			});
		storeClient.onItemAddedToWeighted = delegate(GameItem addedItem)
		{
			int contrabandLevel = ContrabandHelper.GetContrabandLevel(addedItem);
			if (contrabandLevel == 1)
			{
				addedItem.AddItemFeature(ItemFeatureList.DiscountedLowContraband());
			}
			if (contrabandLevel == 2)
			{
				addedItem.AddItemFeature(ItemFeatureList.DiscountedMidContraband());
			}
			if (contrabandLevel == 3)
			{
				addedItem.AddItemFeature(ItemFeatureList.DiscountedHighContraband());
			}
			if (contrabandLevel == 4)
			{
				addedItem.AddItemFeature(ItemFeatureList.DiscountedCriticalContraband());
			}
		};
		storeClient.clientBuyingTagList.Add("AMMUNITION");
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("MELEE_WEAPON");
		storeClient.clientBuyingTagList.Add("ACCESS_CARD");
		storeClient.clientBuyingTagList.Add("MUNITION");
		storeClient.clientBuyingTagList.Add("SUPPLY_CRATE");
		storeClient.clientBuyingTagList.Add("ARMOR");
		storeClient.clientBuyingTagList.Add("POISON");
		storeClient.onCaptureAction = delegate
		{
		};
		storeClient.onReportAction = delegate
		{
			StorePhoneClient.GetRevMerchantClient().phoneState = StorePhoneClient.PhoneState.NotResponding;
		};
		storeClient.onItemBought = delegate(GameItem item, int cost)
		{
			int num = 0;
			if (cost >= 15)
			{
				num = (int)((float)cost * 0.15f);
			}
			else if (cost >= 10)
			{
				num = 2;
			}
			else if (cost >= 8)
			{
				num = 1;
			}
			PlayerStore.Instance.odinBudget += num;
			if (PlayerStore.Instance.odinBudget > 1500)
			{
				PlayerStore.Instance.odinBudget = 1500;
			}
			Debug.Log("Odin Budget Increase: " + num);
		};
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wanted4normal_arrest_client")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_wanted4normal_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation(IgnoreExposeFunc: true);
		return storeClient;
	}

	public static StoreClient CreateWanted5()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted5";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lou");
		storeClient.spriteName = "wanted5";
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.reportDegradesFaction = false;
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetWanted(1500, LocHelper.GetLocalizedClientName("name_lou"));
		storeClient.SetVariety();
		storeClient.SetBudget(250);
		storeClient.clientBuyingTagList.Add("MELEE_WEAPON");
		storeClient.clientBuyingIdList.Add("welder");
		storeClient.clientBuyingIdList.Add("screwdriver");
		storeClient.onItemBought = delegate(GameItem item, int value)
		{
			PlayerStore.Instance.storeClientData.louBoughtItemIds.Add(item.identifier);
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5_1")).SetStartAction(delegate
		{
			if (PlayerStore.Instance.storeClientData.louMetDay < 0)
			{
				PlayerStore.Instance.storeClientData.louMetDay = StoreStation.Instance.dayCounter;
			}
		}).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5_tools_1"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5_tools_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5_8"))
			.SetEndAction(delegate
			{
			});
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wanted5_arrest_client_1")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_wanted5_arrest_sec_1"))
			.NextDialogue()
			.SetText(title2, LocHelper.GetLocalizedDialogue("dialog_wanted5_arrest_sec_2"))
			.NextDialogue()
			.SetText(title2, LocHelper.GetLocalizedDialogue("dialog_wanted5_arrest_sec_3"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wanted5_arrest_client_2"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wanted5_arrest_client_3"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.onCaptureAction = delegate
		{
			PlayerStore.Instance.storeClientData.louArrestedDay = StoreStation.Instance.dayCounter;
		};
		LouOperativeHelper.SetupLouShootable(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWanted5Morning()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted5morning";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lou");
		storeClient.spriteName = "wanted5";
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.reportDegradesFaction = false;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.arrestable = false;
		storeClient.nonShootable = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5morning_1")).SetStartAction(delegate
		{
			if (PlayerStore.Instance.storeClientData.louMorningDay < 0)
			{
				PlayerStore.Instance.storeClientData.louMorningDay = StoreStation.Instance.dayCounter;
			}
		}).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5morning_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5morning_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5morning_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5morning_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5morning_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5morning_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5morning_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5morning_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5morning_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5morning_11"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted5morning_12"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		LouOperativeHelper.SetupLouShootable(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLouBombVisit()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "louBombVisit";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lou");
		storeClient.spriteName = "wanted5";
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.reportDegradesFaction = false;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.arrestable = false;
		storeClient.nonShootable = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lou_bomb_1")).SetStartAction(delegate
		{
			PlayerStore.Instance.storeClientData.isLouBombVisitPending = false;
			if (PlayerStore.Instance.storeClientData.louBombVisitDay < 0)
			{
				PlayerStore.Instance.storeClientData.louBombVisitDay = StoreStation.Instance.dayCounter;
			}
		}).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lou_bomb_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lou_bomb_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lou_bomb_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		LouOperativeHelper.SetupLouShootable(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLouBoxDrop()
	{
		StoreClient storeClient = CreateLouTalkClient("louBoxDrop");
		bool isGiven = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_drop_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_drop_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_drop_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_drop_4"))
			.SetStartAction(delegate
			{
				if (!isGiven)
				{
					isGiven = true;
					LouToyBoxHelper.GiveToyBox();
					if (PlayerStore.Instance.storeClientData.louBoxDropDay < 0)
					{
						PlayerStore.Instance.storeClientData.louBoxDropDay = StoreStation.Instance.dayCounter;
					}
				}
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_drop_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_drop_6"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		LouOperativeHelper.SetupLouShootable(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLouBoxCollect()
	{
		StoreClient storeClient = CreateLouTalkClient("louBoxCollect");
		string displayName = storeClient.displayName;
		Dialogue takeBack = new Dialogue();
		storeClient.mainDialogue.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_collect_1")).SetNextDialogue(takeBack);
		bool isDecided = false;
		takeBack.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_collect_2")).SetStartAction(delegate
		{
			if (!isDecided)
			{
				isDecided = true;
				PlayerStore.Instance.storeClientData.louBoxCollectDay = StoreStation.Instance.dayCounter;
				takeBack.SetNextDialogue(CreateLouBoxOutcome(storeClient, LouToyBoxHelper.CollectBox()));
			}
		});
		LouOperativeHelper.SetupLouShootable(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static Dialogue CreateLouBoxOutcome(StoreClient storeClient, LouToyBoxHelper.Outcome outcome)
	{
		string displayName = storeClient.displayName;
		Dialogue dialogue = new Dialogue();
		if (outcome == LouToyBoxHelper.Outcome.Taken)
		{
			dialogue.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_taken_1")).NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_taken_2"))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_taken_3"))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_taken_4"))
				.SetEndAction(delegate
				{
					PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
					LouToyBoxHelper.PlantRealBomb();
				});
			return dialogue;
		}
		Dialogue dialogue2 = new Dialogue();
		Dialogue dialogue3 = new Dialogue();
		if (outcome == LouToyBoxHelper.Outcome.Peeked)
		{
			dialogue.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_peeked_1")).NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_peeked_2"))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_peeked_3"))
				.SetNextDialogue(dialogue2);
			dialogue3.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_peeked_4"));
		}
		else
		{
			dialogue.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_untouched_1")).NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_untouched_2"))
				.SetNextDialogue(dialogue2);
			dialogue3.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_lou_box_see_you"));
		}
		string localizedOrDefault = LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_box_gift", "And good Nutrifruits get a treat. Go on, pick one.");
		storeClient.SetBarterOffer(CreateLouGift());
		dialogue2.SetText(displayName, localizedOrDefault).SetEndAction(delegate
		{
			storeClient.barterOffer.InstanciateOffer();
		});
		storeClient.repeatDialogue = new Dialogue().SetText(displayName, localizedOrDefault);
		storeClient.acceptDealDialogue = new Dialogue().SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_box_gift_accept", "Ooh, good choice.")).SetNextDialogue(dialogue3);
		dialogue3.SetEndAction(delegate
		{
			PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
		});
		return dialogue;
	}

	private static BarterOffer CreateLouGift()
	{
		return BarterOfferList.CreateFreeRewardChoice(LocHelper.GetLocalizedUIOrDefault("ui_lou_reward_body", "Lou's thank-you gift: pick one."), delegate
		{
			AddLouGift("turbo_booster_adv", 1);
		}, delegate
		{
			AddLouGift("smuggler_bay", 1);
		}, delegate
		{
			AddLouGift("backpack_small", 1);
		}, delegate
		{
			AddLouGift("module_bay_expansion_kit", 2);
		});
	}

	private static void AddLouGift(string id, int count)
	{
		for (int i = 0; i < count; i++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTableMiddle(ItemFeatureHelper.SetShabby(DirectoryMaster.Item(id, isOwned: false)));
		}
	}

	private static StoreClient CreateLouTalkClient(string identifier)
	{
		return new StoreClient
		{
			identifier = identifier,
			displayName = LocHelper.GetLocalizedClientName("name_lou"),
			spriteName = "wanted5",
			clientFaction = "FACTION_BLACK_MARKET",
			reportDegradesFaction = false,
			clientIntent = StoreClient.ClientIntent.DIALOGUE,
			dismissable = false,
			arrestable = false,
			nonShootable = true
		};
	}

	public static StoreClient CreateLouDisguise()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "louDisguise";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_security_inspector");
		storeClient.spriteName = "lou_sec";
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.reportDegradesFaction = false;
		storeClient.clientIntent = StoreClient.ClientIntent.INSPECTION;
		storeClient.bubbleColor = "sec";
		storeClient.dismissable = true;
		storeClient.arrestable = false;
		storeClient.nonShootable = true;
		string lou = LocHelper.GetLocalizedClientName("name_lou");
		Dialogue demand = new Dialogue();
		demand.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_1")).NextDialogue().SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_2"))
			.NextDialogue()
			.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_3"))
			.NextDialogue()
			.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_4"))
			.NextDialogue()
			.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_5"))
			.NextDialogue()
			.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_6"))
			.NextDialogue()
			.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_7"))
			.NextDialogue()
			.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_8"))
			.NextDialogue()
			.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_9", StoreClientData.GetLouDemandValue()))
			.SetStartAction(delegate
			{
				StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
				if (storeClientData.louDisguiseDay < 0)
				{
					storeClientData.louDisguiseDay = StoreStation.Instance.dayCounter;
				}
			})
			.NextDialogue()
			.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_10"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		bool isRevealed = false;
		Action breakCharacter = delegate
		{
			if (!isRevealed)
			{
				isRevealed = true;
				storeClient.tryInterceptDismiss = null;
				storeClient.dismissable = false;
				storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
				storeClient.displayName = lou;
				storeClient.bubbleColor = "DEFAULT";
			}
		};
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_choice_accept")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("sec_shoot_ellipsis"))
			.NextDialogue()
			.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_accept_1"))
			.SetStartAction(breakCharacter)
			.NextDialogue()
			.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_accept_2"))
			.SetNextDialogue(demand);
		Dialogue dialogue2 = new Dialogue();
		dialogue2.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_last_open")).AddChoice(DirectoryMaster.Item("yes"), LocHelper.GetLocalizedDialogue("sec_inspection_accept_label"), "", dialogue, delegate
		{
		});
		storeClient.mainDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_inspection_routine_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_inspection_routine_2"))
			.SetNextDialogue(dialogue2);
		storeClient.tryInterceptDismiss = delegate
		{
			if (isRevealed)
			{
				return false;
			}
			Dialogue dialogue3 = new Dialogue();
			dialogue3.SetPriority(7);
			dialogue3.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_dismiss_1")).SetStartAction(breakCharacter).NextDialogue()
				.SetText(lou, LocHelper.GetLocalizedDialogue("dialog_lou_disguise_dismiss_2"))
				.SetNextDialogue(demand);
			DialogUIManager.Instance.HideChoiceBubble();
			return DialogUIManager.Instance.StartDialogue(dialogue3);
		};
		LouOperativeHelper.SetupLouShootable(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLouDemandCollect()
	{
		StoreClient storeClient = CreateLouTalkClient("louDemandCollect");
		string name = storeClient.displayName;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("FIREARM");
		storeClient.clientBuyingTagList.Add("MELEE_WEAPON");
		storeClient.clientBuyingTagList.Add("AMMUNITION");
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.clientItemFeatureBuying.Add("bribeDonation");
		storeClient.isNoContributeToEvidence = true;
		storeClient.tradeReputationMultiplier = 0.0;
		storeClient.isBargainAvailable = false;
		storeClient.acceptRetailMarkup = false;
		storeClient.canClientExposeFeature = ClientCanExposeFunc.TradeBoardExposeCapacity;
		Action endAction = delegate
		{
			if (PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint() == storeClient)
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			}
		};
		bool isSettled = false;
		bool isPaid = false;
		Action settle = delegate
		{
			isSettled = true;
			storeClient.tryInterceptDismiss = null;
			storeClient.dismissable = false;
			storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
			DialogUIManager.Instance.HideChoiceBubble();
			PlayerStore.Instance.storeClientData.louDemandDay = StoreStation.Instance.dayCounter;
		};
		Action queueNews = delegate
		{
			StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListUnique.CreateLouAdmiralAssassinated(), 1, IsKnown: false);
		};
		string localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_lou_demand_even");
		Dialogue cashRest = new Dialogue();
		cashRest.SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_cash")).NextDialogue().SetText(name, localizedDialogue)
			.SetEndAction(endAction);
		Dialogue cashAll = new Dialogue();
		cashAll.SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_cash_all")).NextDialogue().SetText(name, localizedDialogue)
			.SetEndAction(endAction);
		Dialogue bombed = new Dialogue();
		bombed.SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_bomb_1")).NextDialogue().SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_bomb_2"))
			.NextDialogue()
			.SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_bomb_3"))
			.NextDialogue()
			.SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_bomb_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
				LouToyBoxHelper.PlantRealBomb("lou_bombed_debt");
			});
		Dialogue shortLine = new Dialogue();
		shortLine.SetPriority(7);
		shortLine.SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_short", StoreClientData.GetLouDemandRemaining())).SetStartAction(delegate
		{
			if (!isSettled)
			{
				int louDemandRemaining = StoreClientData.GetLouDemandRemaining();
				shortLine.SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_short", louDemandRemaining));
				int num2 = Mathf.Min(louDemandRemaining, Mathf.Max(0, PlayerStore.Instance.playerCash));
				settle();
				if (PlayerStore.Instance.storeClientData.louDemandGiven + num2 < StoreClientData.GetLouDemandMinimum())
				{
					shortLine.SetNextDialogue(bombed);
				}
				else
				{
					if (num2 > 0)
					{
						PlayerStore.Instance.ModCash(-num2);
					}
					queueNews();
					shortLine.SetNextDialogue((num2 < louDemandRemaining) ? cashAll : cashRest);
				}
			}
		});
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_lou_demand_choice_done")).SetNextDialogue(shortLine);
		Dialogue dialogue2 = new Dialogue();
		dialogue2.SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_wait")).SetStartAction(delegate
		{
			if (!(isSettled || isPaid))
			{
				storeClient.clientIntent = StoreClient.ClientIntent.BUY;
				storeClient.dismissable = true;
			}
		}).AddChoice(DirectoryMaster.Item("$"), "", "", dialogue, delegate
		{
		});
		storeClient.resumeAfterConfrontDialogue = dialogue2;
		storeClient.mainDialogue.SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_1")).NextDialogue().SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_2", StoreClientData.GetLouDemandRemaining()))
			.NextDialogue()
			.SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_3"))
			.SetNextDialogue(dialogue2);
		Dialogue enough = new Dialogue();
		enough.SetPriority(7);
		enough.SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_enough")).SetStartAction(delegate
		{
			if (!isSettled)
			{
				settle();
				queueNews();
			}
		}).NextDialogue()
			.SetText(name, localizedDialogue)
			.SetEndAction(endAction);
		Dialogue remainingLine = new Dialogue();
		remainingLine.SetPriority(6);
		remainingLine.SetText(name, GetLouRemainingLine()).SetStartAction(delegate
		{
			remainingLine.SetText(name, GetLouRemainingLine());
		}).SetNextDialogue(dialogue2);
		Dialogue liked = new Dialogue();
		liked.SetPriority(6);
		liked.SetText(name, LocHelper.GetLocalizedDialogue("dialog_lou_demand_progress")).SetNextDialogue(remainingLine);
		bool isLikedInDeal = false;
		Func<bool> intercept = null;
		intercept = delegate
		{
			if (!isSettled)
			{
				DialogUIManager.Instance.HideChoiceBubble();
				bool num = DialogUIManager.Instance.StartDialogue(isPaid ? enough : shortLine);
				if (!num)
				{
					storeClient.tryInterceptDismiss = intercept;
				}
				return num;
			}
			return false;
		};
		storeClient.tryInterceptDismiss = intercept;
		storeClient.onItemBought = delegate(GameItem item, int value)
		{
			item.RemoveItemFeatureByID("bribeDonation");
			if (!(isSettled || isPaid))
			{
				PlayerStore.Instance.storeClientData.louDemandGiven += Mathf.Max(0, (int)item.GetNegociatedValue());
				if (item.IsGameItemType("FIREARM") && !item.IsGameItemType("MAKESHIFT") && !item.IsGameItemType("LOWTECH"))
				{
					isLikedInDeal = true;
				}
				if (StoreClientData.GetLouDemandRemaining() <= 0)
				{
					isPaid = true;
					storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
				}
			}
		};
		storeClient.onDealAccepted = delegate
		{
			if (!isSettled)
			{
				Dialogue dialogue3 = (isPaid ? enough : (isLikedInDeal ? liked : remainingLine));
				isLikedInDeal = false;
				DialogUIManager.Instance.HideChoiceBubble();
				DialogUIManager.Instance.StartDialogue(dialogue3, forcedStart: true);
			}
		};
		LouOperativeHelper.SetupLouShootable(storeClient);
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation(IgnoreExposeFunc: true);
		return storeClient;
	}

	private static string GetLouRemainingLine()
	{
		return LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_demand_remaining", "Still {0} Credits' worth to go.", StoreClientData.GetLouDemandRemaining());
	}

	public static StoreClient CreateWanted6()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted6";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_li_bei_wen");
		storeClient.spriteName = "wanted6";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWanted(400, LocHelper.GetLocalizedClientName("name_li_bei_wen"));
		SetupLiShot(storeClient);
		storeClient.SetWholesale(50);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_8"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("pink_alt_injector");
				PlayerStore.Instance.AddDirectSellingItemToTable("pink_alt_injector");
				PlayerStore.Instance.AddDirectSellingItemToTable("pink_alt_injector");
				PlayerStore.Instance.AddDirectSellingItemToTable("pink_alt_injector");
				PlayerStore.Instance.AddDirectSellingItemToTable("pink_alt_injector");
				PlayerStore.Instance.AddDirectSellingItemToTable("pink_alt_injector");
				PlayerStore.Instance.AddDirectSellingItemToTable("pink_alt_injector");
				PlayerStore.Instance.AddDirectSellingItemToTable("pink_alt_injector");
			});
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wanted6_arrest_client")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_wanted6_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		storeClient.tryInterceptReport = delegate
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_plea_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_plea_2"));
			DialogUIManager.Instance.StartDialogue(dialogue);
			return true;
		};
		AddLiThanksOnLeave(storeClient);
		storeClient.OnDismiss = delegate
		{
			if (!storeClient.reported)
			{
				StoreClientData.ScheduleLiVisit(1, 3);
			}
		};
		return storeClient;
	}

	private static void AddLiThanksOnLeave(StoreClient storeClient)
	{
		string thanks = LocHelper.GetLocalizedDialogue("dialog_wanted6_thanks");
		if (storeClient.acceptDealDialogue != null)
		{
			storeClient.acceptDealDialogue.SetText("CLIENT", thanks);
			Dialogue acceptDealDialogue = storeClient.acceptDealDialogue;
			acceptDealDialogue.startAction = (Action)Delegate.Combine(acceptDealDialogue.startAction, (Action)delegate
			{
				storeClient.tryInterceptDismiss = null;
			});
		}
		storeClient.tryInterceptDismiss = delegate
		{
			DialogUIManager.Instance.StartDialogue(new Dialogue().SetText(storeClient.displayName, thanks).SetEndAction(delegate
			{
				LeaveIfStillHere(storeClient);
			}));
			return true;
		};
	}

	private static void LeaveIfStillHere(StoreClient storeClient)
	{
		if (PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint() == storeClient)
		{
			PlayerStore.Instance.DismissCurrentClient();
		}
	}

	public static StoreClient CreateWanted6Supply()
	{
		StoreClient storeClient = CreateLiReturnClient("wanted6_supply");
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.clientBuyingIdList.Add("common_chemical");
		storeClient.clientBuyingIdList.Add("restricted_chemical");
		storeClient.clientItemFeatureBuying.Add("bribeDonation");
		storeClient.isNoContributeToEvidence = true;
		int common = 0;
		int restricted = 0;
		bool hasEnough = false;
		string leaveLine = LocHelper.GetLocalizedDialogue("dialog_wanted6_supply_leave");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_supply_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_supply_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_supply_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_supply_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_supply_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_supply_6"));
		string noneLine = LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_wanted6_supply_none", "Okay, I'll come back in a week. Please have some chemical supplies ready by then.");
		storeClient.tryInterceptDismiss = delegate
		{
			DialogUIManager.Instance.StartDialogue(new Dialogue().SetText(storeClient.displayName, noneLine).SetEndAction(delegate
			{
				LeaveIfStillHere(storeClient);
			}));
			return true;
		};
		storeClient.onItemBought = delegate(GameItem item, int value)
		{
			item.RemoveItemFeatureByID("bribeDonation");
			if (item.identifier == "restricted_chemical")
			{
				restricted++;
			}
			else
			{
				common++;
			}
			if (!hasEnough)
			{
				if (StoreClientData.GetLiFinaleDelay(common, restricted) <= 2)
				{
					hasEnough = true;
					storeClient.tryInterceptDismiss = null;
					DialogUIManager.Instance.HideChoiceBubble();
					storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
					Dialogue dialogue = new Dialogue();
					dialogue.SetPriority(7);
					dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_wanted6_supply_enough")).NextDialogue().SetText("CLIENT", leaveLine)
						.SetEndAction(delegate
						{
							LeaveIfStillHere(storeClient);
						});
					DialogUIManager.Instance.StartDialogue(dialogue);
				}
				else
				{
					storeClient.tryInterceptDismiss = delegate
					{
						DialogUIManager.Instance.StartDialogue(new Dialogue().SetText(storeClient.displayName, leaveLine).SetEndAction(delegate
						{
							LeaveIfStillHere(storeClient);
						}));
						return true;
					};
				}
			}
		};
		storeClient.OnDismiss = delegate
		{
			if (!storeClient.reported)
			{
				if (common + restricted > 0)
				{
					StoreClientData.ScheduleLiVisit(2, StoreClientData.GetLiFinaleDelay(common, restricted));
				}
				else
				{
					StoreClientData.ScheduleLiVisit(1, 7);
				}
			}
		};
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWanted6Finale()
	{
		StoreClient storeClient = CreateLiReturnClient("wanted6_finale");
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		bool isGiven = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_finale_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_finale_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_finale_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted6_finale_4"))
			.SetStartAction(delegate
			{
				if (!isGiven)
				{
					isGiven = true;
					PlayerStore.Instance.AddDirectSellingItemToTable(CreateLiFormulaNote(), isOwend: true);
					for (int i = 0; i < 2; i++)
					{
						GameItem gameItem = DirectoryMaster.Item("pink_alt_injector");
						MedsItemDirectory.SetNomoreInjectorPurity(gameItem, 99, 100);
						PlayerStore.Instance.AddDirectSellingItemToTable(gameItem, isOwend: true);
					}
				}
			})
			.SetEndAction(delegate
			{
				LeaveIfStillHere(storeClient);
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static GameItem CreateLiFormulaNote()
	{
		GameItem gameItem = PreBuiltItemHelper.CreateHandnote(LocHelper.GetLocalizedUI("book_chem_p2_s1_title") + "\n" + LocHelper.GetLocalizedMechanic("note_li_formula_s1_body") + "\n" + LocHelper.GetLocalizedUI("book_chem_p2_s2_title") + "\n" + LocHelper.GetLocalizedMechanic("note_li_formula_s2_body") + "\n" + LocHelper.GetLocalizedUI("book_chem_p2_s3_title") + "\n" + LocHelper.GetLocalizedMechanic("note_li_formula_s3_body") + "\n" + LocHelper.GetLocalizedMechanic("note_li_formula_s4_title") + "\n" + LocHelper.GetLocalizedMechanic("note_li_formula_s4_body") + "\n" + LocHelper.GetLocalizedMechanic("note_li_formula_s4_note") + "\n" + LocHelper.GetLocalizedMechanic("note_li_formula_s5_title") + "\n" + LocHelper.GetLocalizedMechanic("note_li_formula_s5_body"), LocHelper.GetLocalizedItem("item_pink_alt_injector_name"), LocHelper.GetLocalizedClientName("name_li_bei_wen"), 2);
		gameItem.name = LocHelper.GetLocalizedItem("item_li_formula_name");
		gameItem.unitValue = 400L;
		return gameItem;
	}

	private static void SetupLiShot(StoreClient storeClient)
	{
		storeClient.shotCountsAsCapture = true;
		storeClient.shotCaptureDialogue = delegate
		{
			string title = "SEC";
			Dialogue dialogue = new Dialogue();
			dialogue.SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "sec_shoot_li_1", "No augs on this one... hold on. Li Bei Wen? The chemist?")).NextDialogue().SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "sec_shoot_li_2", "Oh, you did good, Citizen. That man was dangerous. Very, VERY dangerous."))
				.NextDialogue()
				.SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "sec_shoot_li_3", "Hehe, I'll probably get a bonus from the corporate rep for closing this case."))
				.NextDialogue()
				.SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "sec_shoot_li_4", "The bounty's yours. We'll take the body."));
			return dialogue;
		};
	}

	private static StoreClient CreateLiReturnClient(string identifier)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = identifier;
		storeClient.wantedPosterId = "wanted6";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_li_bei_wen");
		storeClient.spriteName = "wanted6";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetWanted(400, LocHelper.GetLocalizedClientName("name_li_bei_wen"));
		SetupLiShot(storeClient);
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_wanted6_arrest_client")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_wanted6_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.AddBasicDialog();
		return storeClient;
	}

	public static StoreClient CreateWanted7()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted7";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_worker");
		storeClient.spriteName = "wanted7";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWanted(125, LocHelper.GetLocalizedClientName("name_juan"));
		storeClient.SetWholesale(-20);
		if (!PlayerStore.Instance.storeClientData.isJuanReportPleaDone)
		{
			storeClient.tryInterceptReport = delegate
			{
				PlayerStore.Instance.storeClientData.isJuanReportPleaDone = true;
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted7_plea_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted7_plea_2"));
				DialogUIManager.Instance.StartDialogue(dialogue);
				string thanks = LocHelper.GetLocalizedDialogue("dialog_wanted7_plea_thanks");
				if (storeClient.acceptDealDialogue != null)
				{
					storeClient.acceptDealDialogue.SetText("CLIENT", thanks);
					Dialogue acceptDealDialogue = storeClient.acceptDealDialogue;
					acceptDealDialogue.startAction = (Action)Delegate.Combine(acceptDealDialogue.startAction, (Action)delegate
					{
						storeClient.tryInterceptDismiss = null;
					});
				}
				storeClient.tryInterceptDismiss = delegate
				{
					DialogUIManager.Instance.StartDialogue(new Dialogue().SetText(storeClient.displayName, thanks).SetEndAction(delegate
					{
						if (PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint() == storeClient)
						{
							PlayerStore.Instance.DismissCurrentClient();
						}
					}));
					return true;
				};
				return true;
			};
		}
		string text = (PlayerStore.Instance.storeClientData.isJuanMet ? LocHelper.GetLocalizedDialogue("dialog_wanted7_return_2") : LocHelper.GetLocalizedDialogue("dialog_wanted7_2"));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted7_1")).SetStartAction(delegate
		{
			PlayerStore.Instance.storeClientData.isJuanMet = true;
		}).NextDialogue()
			.SetText(storeClient.displayName, text)
			.SetEndAction(delegate
			{
				int chance = 25 + StoreStation.Instance.dayCounter * 2;
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat", isOwend: false, isStolen: true);
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat", isOwend: false, isStolen: true);
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_milk", isOwend: false, isStolen: true);
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_milk", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_milk", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_milk", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_milk", isOwend: false, isStolen: true);
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_juice", isOwend: false, isStolen: true);
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_juice", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_juice", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_juice", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_juice", isOwend: false, isStolen: true);
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWanted8()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted8";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_john_smith");
		storeClient.spriteName = "wanted8";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetWanted(125, LocHelper.GetLocalizedClientName("name_john_smith"));
		storeClient.SetBudget(125 + RNG.GetRandomInt(0, 50));
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_1")).SetStartAction(delegate
		{
			if (PlayerStore.Instance.storeClientData.johnSmithMetDay < 0)
			{
				PlayerStore.Instance.storeClientData.johnSmithMetDay = StoreStation.Instance.dayCounter;
			}
		}).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_6"))
			.SetEndAction(delegate
			{
			});
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wanted8_arrest_client")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_wanted8_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.onCaptureAction = delegate
		{
			PlayerStore.Instance.storeClientData.johnSmithReportedDay = StoreStation.Instance.dayCounter;
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWanted8Weekly()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted8_weekly";
		storeClient.wantedPosterId = "wanted8";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_john_smith");
		storeClient.spriteName = "wanted8";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetWanted(125, LocHelper.GetLocalizedClientName("name_john_smith"));
		storeClient.SetBudget(125 + RNG.GetRandomInt(0, 50) + StoreStation.GetDayCounter() * 4);
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_food_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_food_2", storeClient.GetBudget()));
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wanted8_arrest_client")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_wanted8_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.onCaptureAction = delegate
		{
			PlayerStore.Instance.storeClientData.johnSmithReportedDay = StoreStation.Instance.dayCounter;
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWanted8Return()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted8_return";
		storeClient.wantedPosterId = "wanted8";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_john_smith");
		storeClient.spriteName = "wanted8";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetWanted(125, LocHelper.GetLocalizedClientName("name_john_smith"));
		storeClient.SetBudget(100 + RNG.GetRandomInt(0, 50));
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_return_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_return_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_return_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_return_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_return_5"));
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wanted8_return_arrest_client")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_wanted8_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.onCaptureAction = delegate
		{
			PlayerStore.Instance.storeClientData.johnSmithReturnReportedDay = StoreStation.Instance.dayCounter;
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWanted8Vandal()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted8_vandal";
		storeClient.wantedPosterId = "wanted8";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_john_smith");
		storeClient.spriteName = "wanted8";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.SetWanted(125, LocHelper.GetLocalizedClientName("name_john_smith"), isStealthWanted: true);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_vandal_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_vandal_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted8_vandal_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient();
			});
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wanted8_vandal_arrest_client")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_wanted8_vandal_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWanted9()
	{
		StoreClient storeClient = StoreClientList.CreateInspectionClient();
		storeClient.identifier = "wanted9";
		storeClient.spriteName = "wanted9";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.dismissable = true;
		storeClient.arrestable = true;
		storeClient.SetWanted(300, LocHelper.GetLocalizedClientName("name_leroy"));
		storeClient.reputationPenalty = 0;
		if (Difficulty.Rules.impostorTell && StoreReputation.IsPerkUnlocked("SEC_VIP"))
		{
			Dialogue dialogue = StoreClientDialogList.SecurityDialogInspectionLastLine();
			dialogue.SetText("CLIENT", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_wanted9_vip_last_open", "So, uh. Open your cargo hold and step aside. Quickly, before my shift... I mean, before the end of my shift."));
			storeClient.mainDialogue = new Dialogue();
			storeClient.mainDialogue.SetText("CLIENT", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_wanted9_vip_1", "Remain calm. Please. Remain... calm.")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_wanted9_vip_2", "This is a routine security inspection. A routine one. Very routine."))
				.NextDialogue()
				.SetText("CLIENT", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_wanted9_vip_3", "This one is... special. Orders from the top."))
				.SetNextDialogue(dialogue);
		}
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wanted9_arrest_client")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_wanted9_arrest_sec_1"))
			.NextDialogue()
			.SetText(title2, LocHelper.GetLocalizedDialogue("dialog_wanted9_arrest_sec_2"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		return storeClient;
	}
}

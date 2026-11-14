using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreClientUniqueList
{
	public const int CASE_ENJOYER_ITEM_THRESHOLD = 2;

	public const int CASE_ENJOYER_RETURN_DAYS = 7;

	public const int CASE_ENJOYER_BUDGET = 1000;

	private const string CASE_ENJOYER_CRATE = "evidence_box";

	private const string CASE_ENJOYER_CARD = "sec_keycard";

	private const string CASE_ENJOYER_PRIZE = "glass_shard_shiv";

	private static readonly string[] CASE_ENJOYER_SPIN_ITEMS = new string[15]
	{
		"c4", "combat_knife", "combat_machete", "crowbar", "glock_receiver", "heavy_handmade_pistol", "pipe_weapon", "printed_gun", "revolver", "shotgun",
		"stun_gun", "stun_baton", "tazer", "system_uncapped_neural_core", "smg"
	};

	private static readonly string[] CASE_ENJOYER_SPIN_FINAL_ITEMS = new string[2] { "smg", "shotgun" };

	private const float CASE_SPIN_FIRST_GAP = 0.05f;

	private const float CASE_SPIN_LAST_GAP = 0.7f;

	private const float CASE_SPIN_SLOWDOWN = 1.075f;

	private const float CASE_LINE_WAIT_MAX = 6f;

	private const float CASE_LINE_READ_BEAT = 1f;

	private const float CASE_UNLOCK_BEAT = 0.3f;

	private const float CASE_CURSE_DELAY = 1.5f;

	private const float CASE_GOLD_WAIT_MAX = 2f;

	public const int ELEANOR4_CREDITS_GIFT = 50;

	public const string ELEANOR_SCAV_SPRITE = "eleanor_scav";

	public const int ELEANOR_SCAV_WHOLESALE = 30;

	public const int ELEANOR_REV_WEAPON_BUDGET = 350;

	public const string ELEANOR_CRATE_ID = "luxury_supply_crate";

	public const string ELEANOR_UNCLE_SPRITE = "maleBM5";

	public const int ELEANOR_GIFT_PURIFIER_DURABILITY = 175;

	public const int ELEANOR_GIFT_PERFUME_USES = 5;

	public const int SERPICO_BOUNTY = 300;

	public const int SERPICO7_SECURITY_REP_REQUIRED = 50;

	public static StoreClient CreateGeorgioIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgio1";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.wholesaleDiscount = -15;
		storeClient.isOfferWholesale = true;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_5"))
			.SetEndAction(delegate
			{
				for (int i = 0; i < 4; i++)
				{
					GameItem gameItem = DirectoryMaster.Item("scrap_metal");
					gameItem.name = LocHelper.GetLocalizedItem("item_ancient_alien_relics_name");
					gameItem.flavorText = LocHelper.GetLocalizedItem("item_ancient_alien_relics_flavor");
					gameItem.shortDescription = "";
					PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
				}
			});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_client_1")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_sec_1"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateGeorgioIntro2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgio1_1";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.wholesaleDiscount = -75;
		storeClient.isOfferWholesale = true;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro2_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro2_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro2_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro2_5"))
			.SetEndAction(delegate
			{
				for (int i = 0; i < 4; i++)
				{
					GameItem gameItem = DirectoryMaster.Item("large_bottled_water");
					gameItem.name = LocHelper.GetLocalizedItem("item_dehydrated_water_name");
					gameItem.flavorText = "";
					gameItem.shortDescription = "";
					PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
				}
			});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_client_1")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_sec_1"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateGeorgioBackpack()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgio_backpack";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.wholesaleDiscount = -40;
		storeClient.isOfferWholesale = true;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_backpack_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_backpack_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_backpack_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("kids_backpack_large"));
			});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_client_1")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_sec_1"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateGeorgioAssistance()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgio_assistance";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		int price = 50;
		int nights = 3;
		Dialogue dialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_assist_5", price, nights));
		Dialogue lineAccept = new Dialogue().SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_georgio_assist_choice_accept", price));
		Dialogue lineDecline = new Dialogue().SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_georgio_loan_choice_decline"));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_assist_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_assist_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_assist_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_assist_4"))
			.SetNextDialogue(dialogue);
		dialogue.AddChoice(DirectoryMaster.Item("$"), "no choice", "", lineAccept, delegate
		{
			lineAccept.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_assist_accept_1")).SetEndAction(delegate
			{
				if (AfterhourEvents.Start("georgio_assistance", nights))
				{
					PlayerStore.Instance.ModCash(-price);
				}
			})
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_assist_accept_2"))
				.SetEndAction(delegate
				{
					DialogUIManager.Instance.CurrentClientLeave();
				});
		}, () => PlayerStore.Instance.GetCash() >= price);
		dialogue.AddChoice(DirectoryMaster.Item("$"), "no choice", "", lineDecline, delegate
		{
			lineDecline.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_assist_decline_1")).SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_client_1")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_sec_1"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateGeorgio2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgio2";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.wholesaleDiscount = -25;
		storeClient.isOfferWholesale = true;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio2_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio2_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio2_4"))
			.SetEndAction(delegate
			{
				GameItem gameItem = DirectoryMaster.Item("packet_red_cigarette");
				InsCigaretteHelper.SetConterfeitGeorgio1(gameItem);
				gameItem.AddItemFeature(ItemFeatureList.CigaretteFakeAuthenticity());
				PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
				UniqueItemHelper.SetUniqueItemId(gameItem, "georgio2_cigarette1");
				GameItem gameItem2 = DirectoryMaster.Item("packet_red_cigarette");
				InsCigaretteHelper.SetConterfeitGeorgio2(gameItem2);
				gameItem2.AddItemFeature(ItemFeatureList.CigaretteFakeAuthenticity());
				PlayerStore.Instance.AddDirectSellingItemToTable(gameItem2);
				UniqueItemHelper.SetUniqueItemId(gameItem2, "georgio2_cigarette2");
			});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio2_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio2_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_georgio2_arrest_client")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio2_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateGeorgio3()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgio3";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.SetWanted(100, "", isStealthWanted: true);
		storeClient.SetWholesale(-25);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio3_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio3_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio3_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill", isOwend: false, isStolen: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill", isOwend: false, isStolen: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill", isOwend: false, isStolen: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("box_tampon", isOwend: false, isStolen: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("shampoo", isOwend: false, isStolen: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper", isOwend: false, isStolen: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper", isOwend: false, isStolen: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper", isOwend: false, isStolen: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("toothpaste", isOwend: false, isStolen: true);
			})
			.NextDialogue()
			.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_georgio3_player_1"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio3_5"));
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio3_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio3_done_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio3_done_2"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_georgio3_arrest_client")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio3_arrest_sec_1"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio3_arrest_sec_2"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateGeorgio4()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgio4";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.SetWanted(100, "", isStealthWanted: true);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio4_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio4_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio4_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio4_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateEvidence(), isOwend: false, isStolen: true);
			})
			.NextDialogue()
			.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_georgio4_player_1"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio4_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio4_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio4_8"));
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio4_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio4_done_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio4_done_2"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave(0f);
			});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_georgio4_arrest_client")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio4_arrest_sec_1"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio4_arrest_sec_2"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateGeorgioLuckIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgio_luck_intro";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_luck_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_luck_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_luck_intro_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("liquid_luck"), isOwend: true);
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_client_1")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_sec_1"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateGeorgioLuckSale()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgio_luck_sale";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.wholesaleDiscount = -20;
		storeClient.isOfferWholesale = true;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_luck_sale_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_luck_sale_2"))
			.SetEndAction(delegate
			{
				for (int i = 0; i < 3; i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(DirectoryMaster.Item("liquid_luck"));
				}
			});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_intro_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_client_1")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_sec_1"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateGeorgioNuclearWaste()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgio_nuclear_waste";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_nuclear_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_nuclear_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_nuclear_3"))
			.SetEndAction(delegate
			{
				for (int i = 0; i < PlayerStore.Instance.storeClientData.nuclearWasteDumped; i++)
				{
					GameItem gameItem = DirectoryMaster.Item("nuclear_waste");
					PlayerStore.Instance.AddDirectSellingItemToTable(gameItem, isOwend: true);
				}
				PlayerStore.GetStoreClientData().nuclearWasteDumped = 0;
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_nuclear_4"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_client_1")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio_arrest_sec_1"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static GameItem CreateGeorgioIouNote(int amount)
	{
		return PreBuiltItemHelper.CreateHandnote(LocHelper.GetLocalizedMechanic("note_georgio_iou_body_1", amount) + "\n" + LocHelper.GetLocalizedMechanic("note_georgio_iou_body_2"), LocHelper.GetLocalizedMechanic("note_georgio_iou_title"), LocHelper.GetLocalizedMechanic("note_georgio_iou_sign"), 2);
	}

	public static StoreClient CreateGeorgioLoan()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgioLoan";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		Dialogue lastLine = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_5"));
		Dialogue line25 = new Dialogue();
		Dialogue line26 = new Dialogue();
		Dialogue lineNo = new Dialogue().SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_georgio_loan_choice_decline"));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_4"))
			.SetNextDialogue(lastLine);
		lastLine.SetStartAction(delegate
		{
			int value25 = PlayerStore.Instance.GetCash() / 4;
			int value26 = PlayerStore.Instance.GetCash() / 2;
			line25.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_georgio_loan_choice_offer", value25));
			line26.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_georgio_loan_choice_offer", value26));
			lastLine.AddChoice(DirectoryMaster.Item("$"), "no choice", "", line25, delegate
			{
				line25.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_accept_small_1")).NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_accept_small_2"))
					.SetEndAction(delegate
					{
						PlayerStore.Instance.ModCash(-value25);
						PlayerStore.Instance.loanOfferedToGeorgio = value25;
						RunStats.OnGeorgioLoan(value25);
						PlayerStore.Instance.dayWhenLoanOfferedGeorgio = StoreStation.Instance.dayCounter;
						PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("scav_token"), isOwend: true);
						PlayerStore.Instance.AddDirectSellingItemToTableMiddle(CreateGeorgioIouNote(value25), isOwend: true);
					})
					.NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_promise_1"))
					.NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_promise_2"))
					.SetEndAction(delegate
					{
						DialogUIManager.Instance.CurrentClientLeave();
					});
			});
			lastLine.AddChoice(DirectoryMaster.Item("$"), "no choice", "", line26, delegate
			{
				line26.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_accept_large_1")).NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_accept_large_2"))
					.SetEndAction(delegate
					{
						PlayerStore.Instance.ModCash(-value26);
						PlayerStore.Instance.loanOfferedToGeorgio = value26;
						RunStats.OnGeorgioLoan(value26);
						PlayerStore.Instance.dayWhenLoanOfferedGeorgio = StoreStation.Instance.dayCounter;
						PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("scav_token"), isOwend: true);
						PlayerStore.Instance.AddDirectSellingItemToTableMiddle(CreateGeorgioIouNote(value26), isOwend: true);
					})
					.NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_promise_1"))
					.NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_promise_2"))
					.SetEndAction(delegate
					{
						DialogUIManager.Instance.CurrentClientLeave();
					});
			});
			lastLine.AddChoice(DirectoryMaster.Item("$"), "no choice", "", lineNo, delegate
			{
				lineNo.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_decline_1")).NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_decline_2"))
					.NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_loan_decline_3"))
					.SetEndAction(delegate
					{
						DialogUIManager.Instance.CurrentClientLeave();
					});
			});
		});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_georgio_loan_arrest_client")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio_loan_arrest_sec_1"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_georgio_loan_arrest_sec_2"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateGeorgioLoanPayback()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgioLoanPayback";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_payback_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_payback_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_payback_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_payback_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.ModCash(PlayerStore.Instance.loanOfferedToGeorgio * 4);
				RunStats.OnGeorgioPayback(PlayerStore.Instance.loanOfferedToGeorgio * 4);
				PlayerStore.Instance.loanOfferedToGeorgio = 0;
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_georgio_payback_done"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateFoodieTourist()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "foodieTourist";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_foodie_tourist");
		storeClient.spriteName = SpriteDict.GetFoodieSpriteName();
		storeClient.clientFaction = "FACTION_TOURIST";
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.QueueFuturClient("foodieTouristReturning", 2);
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.clientBuyingTagList.Add("BREVAGE");
		storeClient.placeRightItemWhenSellingToDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_item_placed"));
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_foodie_arrest_client")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_foodie_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static int CountCaseEnjoyerItems()
	{
		int num = 0;
		foreach (GameItem allOwnedItem in EmporiumEntry.Instance.GetAllOwnedItems())
		{
			if (allOwnedItem.IsGameItemType("ACCESS_CARD") || allOwnedItem.IsGameItemType("SUPPLY_CRATE"))
			{
				num++;
			}
		}
		return num;
	}

	private static string CaseEnjoyerName()
	{
		return LocHelper.GetLocalizedClientName("name_case_enjoyer");
	}

	public static StoreClient CreateCaseEnjoyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "caseEnjoyer";
		storeClient.displayName = CaseEnjoyerName();
		storeClient.spriteName = SpriteDict.GetCaseEnjoyerSpriteName();
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_case_enjoyer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_case_enjoyer_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_case_enjoyer_3"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCaseEnjoyerReturning()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "caseEnjoyerReturning";
		storeClient.displayName = CaseEnjoyerName();
		storeClient.spriteName = SpriteDict.GetCaseEnjoyerSpriteName();
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.AcceptingContrabandOverride = true;
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(1000);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_case_enjoyer_returning_1"));
		storeClient.clientBuyingTagList.Add("ACCESS_CARD");
		storeClient.clientBuyingTagList.Add("SUPPLY_CRATE");
		storeClient.AddBasicDialog();
		bool sceneStarted = false;
		Dialogue openingScene = CreateCaseOpeningScene(storeClient, TryBeginScene);
		storeClient.allDoneDialogue = openingScene;
		storeClient.tryInterceptDismiss = delegate
		{
			if (sceneStarted || StoreUIManager.Instance.isShot || PlayerStore.Instance.isClientBeingArrested)
			{
				return false;
			}
			DialogUIManager.Instance.StartDialogue(openingScene, forcedStart: true);
			return true;
		};
		storeClient.CompleteClientCreation();
		return storeClient;
		bool TryBeginScene()
		{
			if (sceneStarted)
			{
				return false;
			}
			sceneStarted = true;
			return true;
		}
	}

	private static Dialogue CreateCaseOpeningScene(StoreClient storeClient, Func<bool> tryBegin)
	{
		string displayName = storeClient.displayName;
		Dialogue scene = new Dialogue();
		scene.priority = 4;
		scene.SetStartAction(delegate
		{
			if (tryBegin())
			{
				storeClient.nonShootable = true;
				storeClient.arrestable = false;
				storeClient.clientBuyingTagList.Clear();
				storeClient.clientBuyingIdList.Clear();
				storeClient.SetBudget(0);
				if (NegociationUIManager.Instance != null && NegociationUIManager.Instance.IsOpen)
				{
					NegociationUIManager.Instance.CloseUI();
				}
				DialogUIManager.Instance.StartCoroutine(CaseOpeningRoutine(PlayerStore.Instance.currentClientInstance, displayName, scene));
			}
		}).SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_case_enjoyer_open_1"));
		return scene;
	}

	private static Dialogue CaseSceneLine(string displayName, string key)
	{
		Dialogue dialogue = new Dialogue().SetText(displayName, LocHelper.GetLocalizedDialogue(key));
		dialogue.priority = 4;
		return dialogue;
	}

	private static void DestroyShown(GameItem item)
	{
		if (item != null && (!(item is GameItemElement gameItemElement) || !gameItemElement.IsDestroyed()))
		{
			item.Destroy();
		}
	}

	private static GameItem ShowSpinItem(string id)
	{
		GameItem gameItem = DirectoryMaster.Item(id, isOwned: false);
		GeneralHelper.SetDisplayOnly(gameItem);
		PlayerStore.Instance.AddDirectSellingItemToTableMiddle(gameItem);
		return gameItem;
	}

	private static IEnumerator CaseOpeningRoutine(StoreClientInstance owner, string displayName, Dialogue openingLine)
	{
		GameItem crate = ShowSpinItem("evidence_box");
		GameItem card = ShowSpinItem("sec_keycard");
		yield return null;
		for (float waited = 0f; waited < 6f; waited += Time.deltaTime)
		{
			if (!DialogUIManager.Instance.IsLineStillDisplaying(openingLine))
			{
				break;
			}
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		if (!StillHere())
		{
			DestroyShown(crate);
			DestroyShown(card);
			yield break;
		}
		EmporiumAudioPlayer.Instance.audioClickClick.Play();
		AudioManager.Instance.TryPlayOne(crate.soundContainerOpen);
		yield return new WaitForSeconds(0.3f);
		DestroyShown(crate);
		DestroyShown(card);
		if (!StillHere())
		{
			yield break;
		}
		List<string> cycle = new List<string>();
		string[] cASE_ENJOYER_SPIN_ITEMS = CASE_ENJOYER_SPIN_ITEMS;
		foreach (string text in cASE_ENJOYER_SPIN_ITEMS)
		{
			if (DirectoryMaster.Has<GameItem>(text))
			{
				cycle.Add(text);
			}
		}
		List<string> finals = new List<string>();
		cASE_ENJOYER_SPIN_ITEMS = CASE_ENJOYER_SPIN_FINAL_ITEMS;
		foreach (string text2 in cASE_ENJOYER_SPIN_ITEMS)
		{
			if (DirectoryMaster.Has<GameItem>(text2))
			{
				finals.Add(text2);
			}
		}
		List<float> gaps = new List<float>();
		for (float num = 0.05f; num < 0.7f; num *= 1.075f)
		{
			gaps.Add(num);
		}
		GameItem shown = null;
		Dialogue gold = null;
		for (int i = 0; i < gaps.Count; i++)
		{
			if (!StillHere())
			{
				DestroyShown(shown);
				yield break;
			}
			int num2 = gaps.Count - i;
			string text3 = ((num2 > finals.Count) ? ((cycle.Count > 0) ? cycle[i % cycle.Count] : null) : finals[finals.Count - num2]);
			DestroyShown(shown);
			shown = ((text3 != null) ? ShowSpinItem(text3) : null);
			AudioManager.Instance.Play3("click_vintage");
			if (finals.Count > 0 && num2 == finals.Count)
			{
				gold = CaseSceneLine(displayName, "dialog_case_enjoyer_open_2");
				DialogUIManager.Instance.StartDialogue(gold, forcedStart: true);
			}
			yield return new WaitForSeconds(gaps[i]);
		}
		for (float held = 0f; held < 2f; held += Time.deltaTime)
		{
			if (!DialogUIManager.Instance.IsLineStillDisplaying(gold))
			{
				break;
			}
			yield return null;
		}
		DestroyShown(shown);
		if (!StillHere())
		{
			yield break;
		}
		GameItem item = DirectoryMaster.Item("glass_shard_shiv");
		if (GraphUtils.TryAcceptAllMid(EmporiumEntry.Instance.frontInvinvElement, item) > 0)
		{
			GraphUtils.EmporiumTryAdd(item);
		}
		AudioManager.Instance.Play3("click_vintage");
		EmporiumAudioPlayer.Instance.PlayMatched();
		DialogUIManager.Instance.StartDialogue(CaseSceneLine(displayName, "dialog_case_enjoyer_open_3"), forcedStart: true);
		yield return new WaitForSeconds(1.5f);
		if (!StillHere())
		{
			yield break;
		}
		Dialogue dialogue = CaseSceneLine(displayName, "dialog_case_enjoyer_open_4");
		dialogue.SetEndAction(delegate
		{
			if (StillHere())
			{
				DialogUIManager.Instance.CurrentClientLeave();
			}
		});
		DialogUIManager.Instance.StartDialogue(dialogue, forcedStart: true);
		bool StillHere()
		{
			if (owner != null && PlayerStore.Instance.currentClientInstance == owner && !StoreUIManager.Instance.isShot)
			{
				return !PlayerStore.Instance.isClientBeingArrested;
			}
			return false;
		}
	}

	public static StoreClient CreateFoodieTouristReturning()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "foodieTouristReturning";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_foodie_tourist");
		storeClient.spriteName = SpriteDict.GetFoodieSpriteName();
		storeClient.clientFaction = "FACTION_TOURIST";
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(750);
		storeClient.SetVariety();
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_returning_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_returning_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_returning_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_returning_4"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.clientBuyingTagList.Add("BREVAGE");
		storeClient.placeRightItemWhenSellingToDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_item_placed"));
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_foodie_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_foodie_arrest_client")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_foodie_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient UniqueEvidenceSeller()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "evidenceSeller";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_thief");
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_evidence_seller_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_evidence_seller_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateEvidence(), isOwend: false, isStolen: true);
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateOfficerLunVisit()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "officerLunVisit";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lun");
		storeClient.spriteName = "officer_lun";
		storeClient.bubbleColor = "sec";
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(50);
		storeClient.arrestable = false;
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.isNoContributeToEvidence = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_visit_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_visit_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_visit_3"))
			.SetEndAction(delegate
			{
			});
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.IsClientBuyingThisItem(targetItem))
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_visit_wrong_item"));
				dialogue.priority = 4;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
			else
			{
				Dialogue dialogue2 = new Dialogue();
				dialogue2.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_visit_right_item"));
				dialogue2.priority = 4;
				DialogUIManager.Instance.StartDialogue(dialogue2);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_visit_accept"));
		storeClient.CompleteSecClient();
		return storeClient;
	}

	public static StoreClient CreateOfficerLunShot()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "officerLunShot";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lun");
		storeClient.spriteName = "officer_lun";
		storeClient.bubbleColor = "sec";
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.IsPersonalConsumption = false;
		storeClient.SetBudget(100);
		storeClient.arrestable = false;
		storeClient.AcceptingContrabandOverride = true;
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.clientBuyingTagList.Add("NARCOTIC");
		storeClient.isNoContributeToEvidence = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_shot_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_shot_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_shot_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_shot_4"))
			.SetEndAction(delegate
			{
			});
		storeClient.onItemBought = delegate(GameItem targetItem, int amount)
		{
			if (targetItem.IsGameItemType("NARCOTIC"))
			{
				PlayerStore.Instance.storeClientData.lunAddictionLevel++;
				Debug.Log("Lun addiction level: " + PlayerStore.Instance.storeClientData.lunAddictionLevel);
			}
		};
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_shot_accept"));
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.IsClientBuyingThisItem(targetItem))
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_shot_wrong_item"));
				dialogue.priority = 4;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
			else if (ContrabandHelper.IsContraband(targetItem))
			{
				Dialogue dialogue2 = new Dialogue();
				dialogue2.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_shot_contraband"));
				dialogue2.priority = 4;
				DialogUIManager.Instance.StartDialogue(dialogue2);
			}
			else
			{
				Dialogue dialogue3 = new Dialogue();
				dialogue3.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_shot_right_item"));
				dialogue3.priority = 4;
				DialogUIManager.Instance.StartDialogue(dialogue3);
			}
		};
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLunCriminel()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lunCriminel";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_citizen");
		storeClient.spriteName = SpriteDict.GetRandomBMMaleSpriteName();
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWanted(150, "", isStealthWanted: true);
		storeClient.SetWholesale(25);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_crim_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_crim_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("stun_baton", isOwend: false, isStolen: false, ignorePerk: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("flashbang_grenade", isOwend: false, isStolen: false, ignorePerk: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("smoke_grenade", isOwend: false, isStolen: false, ignorePerk: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo", isOwend: false, isStolen: false, ignorePerk: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo", isOwend: false, isStolen: false, ignorePerk: true);
			});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_crim_accept"));
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_lun_crim_arrest_client")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_lun_crim_arrest_sec_1"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_lun_crim_arrest_sec_2"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_lun_crim_arrest_client_2"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateOfficerLunBackOnDuty()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "officerLunBackOnDuty";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lun");
		storeClient.spriteName = "officer_lun";
		storeClient.bubbleColor = "sec";
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.AcceptingContrabandOverride = true;
		storeClient.SetBudget(94);
		storeClient.arrestable = false;
		storeClient.clientBuyingTagList.Add("NARCOTIC");
		storeClient.clientBuyingIdList.Add("black_injector");
		storeClient.isNoContributeToEvidence = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_6"))
			.SetEndAction(delegate
			{
			});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_accept"));
		storeClient.onItemBought = delegate(GameItem targetItem, int amount)
		{
			if (targetItem.identifier.Equals("black_injector"))
			{
				DialogUIManager.PlayQuickDialogue(5, storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_black_injector_thanks"));
				PlayerStore.Instance.storeClientData.lunAddictionLevel -= 10;
			}
			else if (targetItem.IsGameItemType("NARCOTIC"))
			{
				PlayerStore.Instance.storeClientData.lunAddictionLevel += 2;
			}
		};
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (storeClient.isMainDialogueStarted)
			{
				if (!storeClient.IsClientBuyingThisItem(targetItem))
				{
					Dialogue dialogue = new Dialogue();
					dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_wrong_item"));
					dialogue.priority = 4;
					DialogUIManager.Instance.StartDialogue(dialogue);
				}
				else if (targetItem.identifier.Equals("black_injector"))
				{
					Dialogue dialogue2 = new Dialogue();
					dialogue2.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_blackinjector_no")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_blackinjector_no2"));
					dialogue2.priority = 4;
					DialogUIManager.Instance.StartDialogue(dialogue2);
				}
				else if (ContrabandHelper.IsContraband(targetItem))
				{
					Dialogue dialogue3 = new Dialogue();
					dialogue3.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_contraband"));
					dialogue3.priority = 4;
					DialogUIManager.Instance.StartDialogue(dialogue3);
				}
				else
				{
					Dialogue dialogue4 = new Dialogue();
					dialogue4.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_right_item"));
					dialogue4.priority = 4;
					DialogUIManager.Instance.StartDialogue(dialogue4);
				}
			}
		};
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateOfficerLunSurplus()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "officerLunSurplus";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lun");
		storeClient.spriteName = "officer_lun";
		storeClient.bubbleColor = "sec";
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(33);
		storeClient.SetWanted(100, "", isStealthWanted: true);
		storeClient.reputationPenalty = 0;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_surplus_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_surplus_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_surplus_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_surplus_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_surplus_5"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceHandgun());
				PlayerStore.Instance.AddDirectSellingItemToTable("flashbang_grenade");
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_p");
				PlayerStore.Instance.AddDirectSellingItemToTable("tazer");
			});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_surplus_accept"));
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_lun_surplus_arrest_client_1")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_lun_surplus_arrest_sec_1"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_lun_surplus_arrest_sec_2"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_lun_surplus_arrest_client_2"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_lun_surplus_arrest_sec_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.storeClientData.officerLunArrested = true;
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateOfficerLunInspection()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "officerLunInspection";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lun");
		storeClient.spriteName = "officer_lun";
		storeClient.bubbleColor = "sec";
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.SetWholesale(33);
		storeClient.SetWanted(200, "", isStealthWanted: true);
		storeClient.reputationPenalty = 0;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_inspect_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_inspect_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, "...")
			.NextDialogue()
			.SetText(storeClient.displayName, "...")
			.SetEndAction(delegate
			{
				bool flag = false;
				List<GameItem> list = new List<GameItem>();
				foreach (GameItem childItem in EmporiumEntry.Instance.invElement.childItems)
				{
					if (childItem.IsTag("NARCOTIC"))
					{
						list.Add(childItem);
						flag = true;
					}
				}
				foreach (GameItem childItem2 in EmporiumEntry.Instance.frontInvinvElement.childItems)
				{
					if (childItem2.IsTag("NARCOTIC"))
					{
						list.Add(childItem2);
						flag = true;
					}
				}
				foreach (GameItem childItem3 in EmporiumEntry.Instance.backInvinvElement.childItems)
				{
					if (childItem3.IsTag("NARCOTIC"))
					{
						list.Add(childItem3);
						flag = true;
					}
				}
				foreach (GameItem childItem4 in EmporiumEntry.Instance.showcaseElement.childItems)
				{
					if (childItem4.IsTag("NARCOTIC"))
					{
						list.Add(childItem4);
						flag = true;
					}
				}
				if (flag)
				{
					Dialogue dialogue = new Dialogue();
					dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_inspect_drugs_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_inspect_drugs_2"))
						.SetEndAction(delegate
						{
							DialogUIManager.Instance.CurrentClientLeave(0f);
						});
					dialogue.priority = 6;
					DialogUIManager.Instance.StartDialogue(dialogue);
					GeneralHelper.DestroyGameItems(list);
				}
				else
				{
					Dialogue dialogue2 = new Dialogue();
					dialogue2.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_inspect_clear")).SetEndAction(delegate
					{
						DialogUIManager.Instance.CurrentClientLeave();
					});
					dialogue2.priority = 6;
					DialogUIManager.Instance.StartDialogue(dialogue2);
				}
			});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lun_duty_accept"));
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_lun_inspect_arrest_client_1")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_lun_inspect_arrest_sec_1"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_lun_inspect_arrest_client_2"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_lun_inspect_arrest_sec_2"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_lun_inspect_arrest_sec_3"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_lun_inspect_arrest_sec_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.storeClientData.officerLunArrested = true;
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static void SetupEleanor(StoreClient storeClient)
	{
		storeClient.nonShootable = false;
		storeClient.forceShootable = true;
		storeClient.holsterResumesDialogue = true;
		storeClient.unholsterDialogue = () => new Dialogue().SetText("CLIENT", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_gun_drawn", "Whoa, you are scaring me..."));
		storeClient.onShotDead = delegate
		{
			PlayerStore.GetStoreClientData().isEleanorDead = true;
		};
	}

	public static StoreClient EleanorIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "eleanorIntro";
		SetupEleanor(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_eleanor");
		storeClient.spriteName = SpriteDict.GetUniqueEleanorNormal();
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.SetBudget(300);
		storeClient.SetVariety();
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_intro_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_intro_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_intro_7"))
			.SetEndAction(delegate
			{
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Eleanor2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "eleanor2";
		SetupEleanor(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_eleanor");
		storeClient.spriteName = SpriteDict.GetUniqueEleanorNormal();
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.clientBuyingTagList.Add("HOUSEHOLD_GOOD");
		storeClient.SetBudget(200);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor2_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor2_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor2_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor2_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor2_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor2_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor2_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor2_9"))
			.SetEndAction(delegate
			{
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Eleanor3()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "eleanor3";
		SetupEleanor(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_eleanor");
		storeClient.spriteName = SpriteDict.GetUniqueEleanorNoJewel();
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(50);
		storeClient.isBargainAvailable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor3_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor3_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor3_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor3_no_family", "I don't have any family left."))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor3_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor3_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor3_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor3_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor3_8"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("bottled_water_premium"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("box_tampon"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("neuroactive_perfume"));
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Eleanor4()
	{
		StoreClient storeClient = new StoreClient
		{
			identifier = "eleanor4"
		};
		SetupEleanor(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_eleanor");
		storeClient.spriteName = SpriteDict.GetUniqueEleanorSad();
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(0);
		storeClient.isBargainAvailable = false;
		bool isCreditsGiven = false;
		Action choiceAction = delegate
		{
			if (!isCreditsGiven && PlayerStore.Instance.GetCash() >= 50)
			{
				isCreditsGiven = true;
				PlayerStore.Instance.ModCash(-50);
				PlayerStore.Instance.storeClientData.eleanorHelpedTimes++;
			}
		};
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("PLAYER", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor4_choice_credits", "[Give her {0} Credits]", 50)).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor4_accept"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave(0f);
			});
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor4_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor4_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor4_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor4_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor4_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor4_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor4_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor4_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor4_9"))
			.SetEndAction(delegate
			{
			})
			.AddChoice(DirectoryMaster.Item("$"), "no choice", "", dialogue, choiceAction, () => PlayerStore.Instance.GetCash() >= 50);
		storeClient.tradeReputationMultiplier = 2.0;
		storeClient.acceptDealDialogue = new Dialogue();
		storeClient.acceptDealDialogue.SetPriority(6);
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor4_accept")).SetStartAction(delegate
		{
			DialogUIManager.Instance.HideChoiceBubble();
		}).SetEndAction(delegate
		{
			PlayerStore.Instance.storeClientData.eleanorHelpedTimes++;
			DialogUIManager.Instance.CurrentClientLeave(0f);
		});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.clientBuyingTagList.Add("TREAT");
		storeClient.clientBuyingTagList.Add("BREVAGE");
		AddEleanorCrateTrap(storeClient, 4);
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Eleanor5()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "eleanor5";
		SetupEleanor(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_eleanor");
		storeClient.spriteName = "eleanor_scav";
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(33);
		storeClient.isBargainAvailable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor5_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor5_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor5_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor5_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor5_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor5_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor5_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor5_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor5_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor5_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor5_11"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("junkTable"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("junkTable"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("junkTable"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("junkTable"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("materialTable"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("materialTable"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("materialTable"));
			});
		AddEleanorCrateTrap(storeClient, 5);
		storeClient.OnDismiss = ClearEleanorCrateHeat;
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static void ClearEleanorCrateHeat()
	{
		if (EmporiumEntry.Instance == null)
		{
			return;
		}
		foreach (GameItem allItem in EmporiumEntry.Instance.GetAllItems())
		{
			if (allItem.identifier == "luxury_supply_crate")
			{
				StolenHelper.RemoveStolenStatus(allItem);
			}
		}
	}

	public static StoreClient Eleanor6()
	{
		StoreClient storeClient = CreateEleanorSeller("eleanor6");
		string displayName = storeClient.displayName;
		storeClient.mainDialogue.SetStartAction(delegate
		{
			StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
			if (storeClientData.eleanorPlanDay < 0)
			{
				storeClientData.eleanorPlanDay = StoreStation.Instance.dayCounter;
			}
		}).SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor6_1")).NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor6_2"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor6_3"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor6_4", 800))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor6_5"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor6_6"))
			.SetEndAction(StoreClientList.AddScavGeneralStock);
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient EleanorScav()
	{
		StoreClient storeClient = CreateEleanorSeller("eleanorScav");
		string displayName = storeClient.displayName;
		storeClient.mainDialogue.SetStartAction(delegate
		{
			PlayerStore.Instance.storeClientData.eleanorScavLastDay = StoreStation.Instance.dayCounter;
		}).SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_scav_1")).NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_scav_2", StoreClientData.GetEleanorSavingsRemaining()))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_scav_3"))
			.SetEndAction(StoreClientList.AddScavGeneralStock);
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Eleanor7()
	{
		StoreClient obj = new StoreClient
		{
			identifier = "eleanor7"
		};
		SetupEleanor(obj);
		obj.displayName = LocHelper.GetLocalizedClientName("name_eleanor");
		obj.spriteName = "eleanor_scav";
		StoreClientFactionSetup.InitUpperLevelCitizen(obj);
		obj.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		obj.dismissable = false;
		string displayName = obj.displayName;
		obj.mainDialogue.SetStartAction(delegate
		{
			StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
			if (storeClientData.eleanorDriveVisitDay < 0)
			{
				storeClientData.eleanorDriveVisitDay = StoreStation.Instance.dayCounter;
			}
		}).SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_1")).NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_2"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_3"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_4"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_5"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_6"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_7"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_8"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_9"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_10"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_11"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_12"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor7_13"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		obj.AddBasicDialog();
		obj.CompleteClientCreation();
		return obj;
	}

	public static StoreClient CreateEleanorThreat()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "eleanorThreat";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_black_market_thug");
		storeClient.spriteName = "maleBM5";
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.arrestable = false;
		storeClient.nonShootable = true;
		string displayName = storeClient.displayName;
		storeClient.mainDialogue.SetStartAction(delegate
		{
			StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
			if (storeClientData.eleanorThreatVisitDay < 0)
			{
				storeClientData.eleanorThreatVisitDay = StoreStation.Instance.dayCounter;
			}
		}).SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_threat_1")).NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_threat_2"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_threat_3"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_threat_4"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_threat_5", 300))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_threat_6"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Eleanor8()
	{
		StoreClient obj = new StoreClient
		{
			identifier = "eleanor8"
		};
		SetupEleanor(obj);
		obj.displayName = LocHelper.GetLocalizedClientName("name_eleanor");
		obj.spriteName = "eleanor_scav";
		StoreClientFactionSetup.InitUpperLevelCitizen(obj);
		obj.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		obj.dismissable = false;
		string displayName = obj.displayName;
		Action<int> choose = delegate(int choice)
		{
			StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
			if (storeClientData.eleanorDriveChoice == 0)
			{
				storeClientData.eleanorDriveChoice = choice;
				storeClientData.eleanorChoiceDay = StoreStation.Instance.dayCounter;
			}
		};
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_eleanor8_choice_return")).NextDialogue().SetStartAction(delegate
		{
			choose(1);
		})
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor8_return_1"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor8_return_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
				PlayerStore.Instance.ModCash(300);
				StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("mech_notify_eleanor_drive_bounty", 300));
			});
		Dialogue dialogue2 = new Dialogue();
		dialogue2.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_eleanor8_choice_leak")).NextDialogue().SetStartAction(delegate
		{
			bool num = PlayerStore.Instance.storeClientData.eleanorDriveChoice == 0;
			choose(2);
			if (num)
			{
				StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListUnique.CreateEleanorLeakNews(), 1, IsKnown: false);
			}
		})
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor8_leak_1"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor8_leak_2"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor8_leak_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		Dialogue dialogue3 = new Dialogue();
		dialogue3.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor8_4")).AddChoice(DirectoryMaster.Item("yes"), "", "", dialogue, delegate
		{
		}).AddChoice(DirectoryMaster.Item("yes"), "", "", dialogue2, delegate
		{
		});
		obj.mainDialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_eleanor8_player")).NextDialogue().SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor8_1"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor8_2"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor8_3"))
			.SetNextDialogue(dialogue3);
		obj.AddBasicDialog();
		obj.CompleteClientCreation();
		return obj;
	}

	public static StoreClient Eleanor9()
	{
		StoreClient storeClient = new StoreClient();
		bool num = PlayerStore.Instance.storeClientData.eleanorDriveChoice == 2;
		storeClient.identifier = "eleanor9";
		SetupEleanor(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_eleanor");
		string name = storeClient.displayName;
		Action startAction = delegate
		{
			StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
			if (storeClientData.eleanorConclusionDay < 0)
			{
				storeClientData.eleanorConclusionDay = StoreStation.Instance.dayCounter;
			}
		};
		bool isGiven = false;
		Action startAction2 = delegate
		{
			if (!isGiven)
			{
				isGiven = true;
				GameItem gameItem = ItemFeatureHelper.SetSecondHand(DirectoryMaster.Item("self_refilling_neuroactive_perfume"));
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(gameItem, isOwend: true);
			}
		};
		if (!num)
		{
			storeClient.spriteName = "eleanor_scav";
			StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
			storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
			storeClient.dismissable = false;
			storeClient.mainDialogue.SetStartAction(startAction).SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_free_1")).NextDialogue()
				.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_free_2"))
				.NextDialogue()
				.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_free_3"))
				.NextDialogue()
				.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_thanks"))
				.NextDialogue()
				.SetStartAction(startAction2)
				.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_free_gift"))
				.NextDialogue()
				.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_free_gift_2"))
				.NextDialogue()
				.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_free_bye"))
				.NextDialogue()
				.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_farewell_1"))
				.NextDialogue()
				.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_farewell_2"))
				.NextDialogue()
				.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_farewell_3"))
				.SetEndAction(delegate
				{
					PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
				});
			storeClient.AddBasicDialog();
			storeClient.CompleteClientCreation();
			return storeClient;
		}
		storeClient.spriteName = "eleanor_rev";
		StoreClientFactionSetup.InitRev(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.SetBudget(350);
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetStartAction(startAction).SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_rev_1")).NextDialogue()
			.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_rev_2"))
			.NextDialogue()
			.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_rev_3"))
			.NextDialogue()
			.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_thanks"))
			.NextDialogue()
			.SetStartAction(startAction2)
			.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_rev_gift"))
			.NextDialogue()
			.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_rev_gun"))
			.SetEndAction(delegate
			{
				storeClient.dismissable = true;
			});
		bool isFarewellStarted = false;
		Func<Dialogue> createFarewell = delegate
		{
			Dialogue dialogue2 = new Dialogue();
			dialogue2.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_rev_bye")).SetStartAction(delegate
			{
				isFarewellStarted = true;
				storeClient.tryInterceptDismiss = null;
				storeClient.dismissable = false;
				DialogUIManager.Instance.HideChoiceBubble();
			}).NextDialogue()
				.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_farewell_1"))
				.NextDialogue()
				.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_farewell_2"))
				.NextDialogue()
				.SetText(name, LocHelper.GetLocalizedDialogue("dialog_eleanor9_farewell_3"))
				.SetEndAction(delegate
				{
					if (PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint() == storeClient)
					{
						PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
					}
				});
			return dialogue2;
		};
		storeClient.tryInterceptDismiss = delegate
		{
			if (isFarewellStarted)
			{
				return false;
			}
			Dialogue dialogue = createFarewell();
			dialogue.SetPriority(7);
			return DialogUIManager.Instance.StartDialogue(dialogue);
		};
		storeClient.AddBasicDialog();
		storeClient.allDoneDialogue = createFarewell();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static StoreClient CreateEleanorSeller(string identifier)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = identifier;
		SetupEleanor(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_eleanor");
		storeClient.spriteName = "eleanor_scav";
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(30);
		storeClient.staysAfterFailedBargain = true;
		bool isEnoughThisDeal = false;
		int paidThisDeal = 0;
		storeClient.onItemSold = delegate(GameItem item, int paid)
		{
			StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
			bool flag2 = storeClientData.eleanorSavings < 800;
			storeClientData.eleanorSavings += Mathf.Max(0, paid);
			paidThisDeal += Mathf.Max(0, paid);
			if (flag2 && storeClientData.eleanorSavings >= 800)
			{
				isEnoughThisDeal = true;
				storeClientData.eleanorSavingsDoneDay = StoreStation.Instance.dayCounter;
			}
		};
		storeClient.onDealAccepted = delegate
		{
			bool flag = paidThisDeal > 0;
			paidThisDeal = 0;
			if (!isEnoughThisDeal)
			{
				if (flag && PlayerStore.Instance.storeClientData.eleanorSavings < 800 && (PlayerStore.Instance.currentClientInstance == null || !StoreClientInstance.IsClientBusinessDone(PlayerStore.Instance.currentClientInstance)))
				{
					Dialogue dialogue = new Dialogue();
					dialogue.SetPriority(7);
					dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor6_to_go", StoreClientData.GetEleanorSavingsRemaining()));
					DialogUIManager.Instance.StartDialogue(dialogue, forcedStart: true);
				}
			}
			else
			{
				isEnoughThisDeal = false;
				Dialogue dialogue2 = new Dialogue();
				dialogue2.SetPriority(7);
				dialogue2.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_scav_enough_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_scav_enough_2"));
				DialogUIManager.Instance.StartDialogue(dialogue2, forcedStart: true);
			}
		};
		storeClient.AddBasicDialog();
		Dialogue allDone = new Dialogue();
		allDone.SetPriority(4);
		allDone.SetText(storeClient.displayName, EleanorAllDoneLine());
		allDone.SetStartAction(delegate
		{
			allDone.SetText(storeClient.displayName, EleanorAllDoneLine());
		});
		allDone.SetEndAction(delegate
		{
			PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
		});
		storeClient.allDoneDialogue = allDone;
		return storeClient;
	}

	private static string EleanorAllDoneLine()
	{
		if (PlayerStore.Instance.storeClientData.eleanorSavings >= 800)
		{
			return StoreClientDialogList.GetRandomDialogDoneString();
		}
		return LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_scav_all_done", "That's all I have today. {0} more Credits and I can hire the investigator.", StoreClientData.GetEleanorSavingsRemaining());
	}

	public static StoreClient CreateEleanorUncle()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "eleanorUncle";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_eleanor_uncle");
		storeClient.spriteName = "maleBM5";
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isBargainAvailable = false;
		storeClient.nonShootable = true;
		storeClient.dismissable = false;
		storeClient.arrestable = false;
		storeClient.reportDegradesFaction = false;
		GameItem crate = null;
		Action action = delegate
		{
			if (crate == null)
			{
				crate = DirectoryMaster.Item("luxury_supply_crate");
				crate.AddItemFeature(ItemFeatureList.FreeFeature());
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(crate);
				storeClient.arrestable = true;
			}
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_uncle_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_uncle_2"))
			.SetEndAction(action)
			.NextDialogue()
			.SetStartAction(action)
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_uncle_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_uncle_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_uncle_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_uncle_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_uncle_7"))
			.SetEndAction(delegate
			{
				storeClient.dismissable = true;
			});
		storeClient.onDealAccepted = delegate
		{
			storeClient.dismissable = true;
			if (crate != null && crate.parentInventory != null)
			{
				crate.RemoveItemFeatureByID("free");
			}
		};
		storeClient.onReportAction = delegate
		{
			if (crate != null && crate.parentInventory != null && GeneralHelper.IsItemOwned(crate))
			{
				if (EmporiumEntry.Instance.frontInvinvElement.childItems.Contains(crate) || EmporiumEntry.Instance.backInvinvElement.childItems.Contains(crate))
				{
					GeneralHelper.SetItemOwned(crate, own: false);
				}
				else
				{
					PlayerStore.Instance.AddDirectSellingItemToTableMiddle(crate);
				}
			}
		};
		storeClient.onCaptureAction = delegate
		{
			storeClient.displayName = LocHelper.GetLocalizedClientName("name_black_market_thug");
			PlayerStore.Instance.storeClientData.eleanorUncleReported = true;
		};
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_eleanor_uncle_arrest_sec_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_eleanor_uncle_arrest_client_1"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_eleanor_uncle_arrest_sec_2"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_eleanor_uncle_arrest_client_2"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_eleanor_uncle_arrest_sec_3"))
			.SetEndAction(delegate
			{
				if (crate != null && crate.parentInventory != null && !GeneralHelper.IsItemOwned(crate))
				{
					crate.Destroy();
				}
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.AddBasicDialog();
		storeClient.allDoneDialogue = new Dialogue();
		storeClient.allDoneDialogue.priority = 4;
		storeClient.allDoneDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_uncle_handed", "Good. Her hands, not yours. I'll know."));
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient EleanorWarned()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "eleanorWarned";
		SetupEleanor(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_eleanor");
		storeClient.spriteName = SpriteDict.GetUniqueEleanorNoJewel();
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		string displayName = storeClient.displayName;
		BarterOffer barterOffer = BarterOfferList.CreateFreeRewardChoice(LocHelper.GetLocalizedUI("ui_eleanor_reward_body"), delegate
		{
			GameItem gameItem2 = DirectoryMaster.Item("portable_water_purifier");
			DurabilityHelper.SetDurability(gameItem2, 175, DurabilityHelper.GetMaxDurability(gameItem2), DurabilityHelper.GetMaxDurability(gameItem2));
			PlayerStore.Instance.AddDirectSellingItemToTableMiddle(gameItem2);
		}, delegate
		{
			GameItem gameItem = DirectoryMaster.Item("pheromone_perfume");
			UseCountHelper.SetCurrentUseCount(gameItem, 5);
			PlayerStore.Instance.AddDirectSellingItemToTableMiddle(gameItem);
		});
		storeClient.SetBarterOffer(barterOffer);
		Dialogue dialogue = new Dialogue();
		storeClient.mainDialogue.SetStartAction(delegate
		{
			StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
			if (storeClientData.eleanorWarnedDay < 0)
			{
				storeClientData.eleanorWarnedDay = StoreStation.Instance.dayCounter;
			}
		}).SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_eleanor_warned_player")).NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_warned_1"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_warned_2"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_warned_3"))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_warned_4"))
			.SetNextDialogue(dialogue);
		dialogue.SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_warned_5")).SetEndAction(delegate
		{
			storeClient.barterOffer.InstanciateOffer();
		});
		storeClient.repeatDialogue = dialogue;
		storeClient.CompleteClientCreation();
		storeClient.acceptDealDialogue = new Dialogue().SetText(displayName, LocHelper.GetLocalizedDialogue("dialog_eleanor_warned_leave")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		return storeClient;
	}

	private static void AddEleanorCrateTrap(StoreClient eleanor, int visit)
	{
		bool isSprung = false;
		StoreClient storeClient = eleanor;
		storeClient.onItemAddedToCounters = (Action<GameItem>)Delegate.Combine(storeClient.onItemAddedToCounters, (Action<GameItem>)delegate(GameItem item)
		{
			if (!isSprung && item != null && !(item.identifier != "luxury_supply_crate") && GeneralHelper.IsItemOwned(item))
			{
				isSprung = true;
				DialogUIManager.Instance.StartCoroutine(MetaHelper.WaitAndDo(0.3f, delegate
				{
					bool num = PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint() == eleanor;
					bool flag = EmporiumEntry.Instance.backInvinvElement.childItems.Contains(item) || EmporiumEntry.Instance.frontInvinvElement.childItems.Contains(item);
					if (!num || !flag)
					{
						isSprung = false;
					}
					else
					{
						GeneralHelper.SetItemOwned(item, own: false);
						eleanor.customOnArrestDialogue = CreateEleanorArrest(item);
						if (PlayerStore.Instance.ArrestCurrentClientForStolenGoods())
						{
							StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
							storeClientData.eleanorArrested = true;
							storeClientData.eleanorArrestDay = StoreStation.Instance.dayCounter;
							storeClientData.eleanorArrestVisit = visit;
						}
						else
						{
							GeneralHelper.SetItemOwned(item);
							eleanor.customOnArrestDialogue = null;
							isSprung = false;
						}
					}
				}));
			}
		});
	}

	private static Dialogue CreateEleanorArrest(GameItem crate)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_eleanor_arrest_sec_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_eleanor_arrest_client_1"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_eleanor_arrest_sec_2"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_eleanor_arrest_client_2"))
			.SetEndAction(delegate
			{
				if (crate != null && crate.parentInventory != null)
				{
					crate.Destroy();
				}
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		return dialogue;
	}

	public static StoreClient CreateEleanorBailOfficer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "eleanorBailOfficer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_security_officer");
		storeClient.spriteName = SpriteDict.GetRandomSecSpriteName();
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.arrestable = false;
		storeClient.nonShootable = true;
		string displayName = storeClient.displayName;
		Action endAction = delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		};
		Action<int> choose = delegate(int choice)
		{
			StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
			if (storeClientData.eleanorBailChoice == 0)
			{
				storeClientData.eleanorBailChoice = choice;
				storeClientData.eleanorBailDay = StoreStation.Instance.dayCounter;
			}
		};
		Action choiceAction = delegate
		{
			if (PlayerStore.Instance.storeClientData.eleanorBailChoice == 0 && PlayerStore.Instance.GetCash() >= 500)
			{
				PlayerStore.Instance.ModCash(-500);
				choose(1);
			}
		};
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("PLAYER", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_bail_choice_pay", "[Pay her bail ({0} Credits)]", 500)).NextDialogue().SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_bail_paid", "Paid in full. She'll be out by tomorrow."))
			.SetEndAction(endAction);
		Dialogue dialogue2 = new Dialogue();
		dialogue2.SetText("PLAYER", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_bail_choice_vouch", "[Vouch for her ({0} Security rep+)]", 40)).NextDialogue().SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_bail_vouched", "Ok, you've done a few favors for us. We will let this slide."))
			.SetEndAction(endAction);
		Dialogue dialogue3 = new Dialogue();
		dialogue3.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_georgio_loan_choice_decline")).NextDialogue().SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_bail_declined", "Suit yourself. She stays where she is."))
			.SetEndAction(endAction);
		storeClient.mainDialogue.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_bail_1", "Shopkeeper. A word about the girl we picked up here, Eleanor.")).NextDialogue().SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_bail_2", "She swears someone set her up. She says you might be willing to post her bail."))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_bail_3", "Bail is set at {0} Credits.", 500))
			.AddChoice(DirectoryMaster.Item("$"), "no choice", "", dialogue, choiceAction, () => PlayerStore.Instance.GetCash() >= 500)
			.AddChoice(DirectoryMaster.Item("$"), "no choice", "", dialogue2, delegate
			{
				choose(2);
			}, () => StoreReputation.GetSecReputation() >= 40)
			.AddChoice(DirectoryMaster.Item("$"), "no choice", "", dialogue3, delegate
			{
				choose(3);
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient EleanorFreed()
	{
		StoreClient obj = new StoreClient
		{
			identifier = "eleanorFreed"
		};
		SetupEleanor(obj);
		obj.displayName = LocHelper.GetLocalizedClientName("name_eleanor");
		obj.spriteName = SpriteDict.GetUniqueEleanorSad();
		StoreClientFactionSetup.InitUpperLevelCitizen(obj);
		obj.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		obj.dismissable = false;
		obj.arrestable = false;
		string displayName = obj.displayName;
		obj.mainDialogue.SetStartAction(delegate
		{
			StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
			if (storeClientData.eleanorFreedVisitDay < 0)
			{
				storeClientData.eleanorFreedVisitDay = StoreStation.Instance.dayCounter;
				storeClientData.eleanorArrested = false;
				storeClientData.eleanorHelpedTimes++;
			}
		}).SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_freed_1", "Security let me go. They said it was thanks to you.")).NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_freed_2", "That crate wasn't mine and I don't have an uncle."))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_freed_3", "Someone wanted me behind bars, just like my mother."))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_eleanor_freed_4", "I won't forget this. Thank you."))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		obj.CompleteClientCreation();
		return obj;
	}

	public static StoreClient Serpico1()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "serpico1";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_serpico");
		storeClient.spriteName = "serpico";
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.arrestable = false;
		storeClient.SetBudget(100);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico1_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico1_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico1_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico1_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico1_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico1_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico1_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico1_8"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("TREAT");
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Serpico3()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "serpico3";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_serpico");
		storeClient.spriteName = "serpico";
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.arrestable = false;
		storeClient.SetBudget(120);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico3_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico3_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico3_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico3_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico3_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico3_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico3_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico3_8"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("ALCOHOL");
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Serpico4()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "serpico4";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_serpico");
		storeClient.spriteName = "serpico";
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.arrestable = false;
		storeClient.SetBudget(120);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico4_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico4_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico4_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico4_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico4_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico4_6"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.clientBuyingTagList.Add("BREVAGE");
		storeClient.clientBuyingTagList.Add("TREAT");
		storeClient.clientBlackTagList.Add("ALCOHOL");
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Serpico5()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "serpico5";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_security_officer");
		storeClient.spriteName = SpriteDict.GetRandomSecSpriteName();
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.arrestable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico5_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico5_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico5_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico5_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico5_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico5_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico5_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico5_8"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.clientBuyingTagList.Add("ALCOHOL");
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Serpico6()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "serpico6";
		SerpicoStoryline.SetupShootable(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_serpico");
		storeClient.spriteName = "serpico";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.SetWanted(300);
		storeClient.onCaptureAction = delegate
		{
			PlayerStore.GetStoreClientData().serpicoReported = true;
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico6_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico6_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico6_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico6_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico6_5"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave(0f);
			});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_client")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_sec_1"))
			.SetStartAction(delegate
			{
				AudioManager.Instance.Play2("pistol_burst");
			})
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.clientGameObject.gameObject.SetActive(value: false);
			})
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_sec_2"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_sec_3"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_sec_4"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.clientBuyingTagList.Add("ALCOHOL");
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Serpico7()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "serpico7";
		SerpicoStoryline.SetupShootable(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_serpico");
		storeClient.spriteName = "serpico_scav";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.SetWanted(600);
		storeClient.onCaptureAction = delegate
		{
			PlayerStore.GetStoreClientData().serpicoDocumentChoice = 4;
		};
		Dialogue dialogue = new Dialogue();
		Dialogue dialogue2 = new Dialogue();
		Dialogue dialogue3 = new Dialogue();
		Dialogue dialogue4 = new Dialogue();
		dialogue2.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_serpico7_choice_rev")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_rev_1"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_rev_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_rev_3"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		dialogue3.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_serpico7_choice_bm")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_bm_1"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_bm_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_bm_3"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		dialogue4.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_serpico7_choice_sec", 50)).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_sec_1"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_sec_2"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_5"))
			.SetNextDialogue(dialogue);
		dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico7_ask")).AddChoice(DirectoryMaster.Item("$"), "-", "", dialogue2, delegate
		{
			PlayerStore.GetStoreClientData().serpicoDocumentChoice = 1;
		}).AddChoice(DirectoryMaster.Item("$"), "-", "", dialogue3, delegate
		{
			PlayerStore.GetStoreClientData().serpicoDocumentChoice = 2;
		})
			.AddChoice(DirectoryMaster.Item("$"), "-", "", dialogue4, delegate
			{
				PlayerStore.GetStoreClientData().serpicoDocumentChoice = 3;
			}, () => StoreReputation.GetSecReputation() >= 50);
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_client")).NextDialogue().SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_sec_1"))
			.SetStartAction(delegate
			{
				AudioManager.Instance.Play2("pistol_burst");
			})
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.clientGameObject.gameObject.SetActive(value: false);
			})
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_sec_2"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_sec_3"))
			.NextDialogue()
			.SetText("SEC", LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_sec_4"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.clientBuyingTagList.Add("ALCOHOL");
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Nellis1()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "nellis1";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_nellis");
		storeClient.spriteName = "nellis";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(120);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis1_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis1_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis1_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis1_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis1_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis1_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis1_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis1_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis1_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis1_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis1_11"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis1_12"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("TREAT");
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Nellis2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "nellis2";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_nellis");
		storeClient.spriteName = "nellis";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(140);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis2_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis2_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis2_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis2_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis2_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis2_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis2_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis2_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis2_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis2_11"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis2_12"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("TREAT");
		storeClient.clientBuyingTagList.Add("ALCOHOL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Nellis3()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "nellis3";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_nellis");
		storeClient.spriteName = "nellis";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(50);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis3_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis3_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis3_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("system_module_corrupt"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("system_module_corrupt"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("system_module_corrupt"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("system_module_ruined"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("system_module_ruined"));
			});
		storeClient.clientBuyingTagList.Add("TREAT");
		storeClient.clientBuyingTagList.Add("ALCOHOL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient Nellis4()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "nellis4";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_nellis");
		storeClient.spriteName = "nellis";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(160);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis4_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis4_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis4_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis4_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis4_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis4_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_nellis4_7"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("TREAT");
		storeClient.clientBuyingTagList.Add("ALCOHOL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient LandlordWholesale()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "landlord_wholesale";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_landlord");
		storeClient.spriteName = "landlord";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.alwaysRejectBadBargain = true;
		storeClient.arrestable = false;
		storeClient.SetWholesale(-10);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_ws_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_ws_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_ws_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_ws_4"))
			.SetEndAction(delegate
			{
				GameItem gameItem = ItemSpawner.Spawn("neuroactive_perfume");
				PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("cat_bar"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("li_eat_snackbar"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("toilet_paper"));
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient GrandpaChengIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "grandpa_cheng_intro";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_grandpa_cheng");
		storeClient.spriteName = "grandpa_cheng";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.clientBlackTagList.Add("LUXURY_ITEM");
		storeClient.arrestable = false;
		storeClient.SetBudget(200);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_intro_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_intro_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_intro_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_intro_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_intro_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_intro_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_intro_11"))
			.SetEndAction(delegate
			{
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient GrandpaChengReferral()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "grandpa_cheng_referral";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_citizen");
		storeClient.spriteName = SpriteDict.GetRandomLowerMaleSpriteName();
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.clientBlackTagList.Add("LUXURY_ITEM");
		storeClient.SetBudget(150);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ref_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ref_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ref_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ref_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ref_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ref_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ref_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ref_8"))
			.SetEndAction(delegate
			{
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient GrandpaCheng2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "grandpa_cheng_2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_grandpa_cheng");
		storeClient.spriteName = "grandpa_cheng";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.clientBlackTagList.Add("LUXURY_ITEM");
		storeClient.arrestable = false;
		storeClient.SetBudget(200);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng2_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng2_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng2_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng2_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng2_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng2_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng2_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng2_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng2_10"))
			.SetEndAction(delegate
			{
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient GrandpaCheng3()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "grandpa_cheng_3";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_grandpa_cheng");
		storeClient.spriteName = "grandpa_cheng";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.arrestable = false;
		storeClient.SetWholesale(-33);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng3_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng3_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng3_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng3_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng3_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng3_6"))
			.SetEndAction(delegate
			{
				GameItem gameItem = ItemSpawner.Spawn("neuroactive_perfume");
				PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("nudka"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("bottled_water_premium"));
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient SawyerGrandpaCheng()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "sawyer_grandpa_cheng";
		StoreClientListStory.SetupSawyer(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_sawyer");
		storeClient.spriteName = "sawyer";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.arrestable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sawyer_cheng_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sawyer_cheng_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sawyer_cheng_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sawyer_cheng_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sawyer_cheng_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sawyer_cheng_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sawyer_cheng_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sawyer_cheng_8"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient GrandpaChengIll()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "grandpa_cheng_ill";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_grandpa_cheng");
		storeClient.spriteName = "grandpa_cheng";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.arrestable = false;
		storeClient.isBargainAvailable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ill_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ill_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ill_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ill_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ill_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ill_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ill_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ill_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cheng_ill_9"))
			.SetEndAction(delegate
			{
				for (int i = 0; i < 4; i++)
				{
					GameItem gameItem = DirectoryMaster.Item("processed_meat");
					gameItem.AddItemFeature(ItemFeatureList.FreeFeature());
					PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
				}
				for (int j = 0; j < 5; j++)
				{
					GameItem gameItem2 = DirectoryMaster.Item("nutrifruit");
					gameItem2.AddItemFeature(ItemFeatureList.FreeFeature());
					PlayerStore.Instance.AddDirectSellingItemToTable(gameItem2);
				}
				for (int k = 0; k < 3; k++)
				{
					GameItem gameItem3 = DirectoryMaster.Item("galaxy_blend");
					gameItem3.AddItemFeature(ItemFeatureList.FreeFeature());
					PlayerStore.Instance.AddDirectSellingItemToTable(gameItem3);
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient WildGrandpaCheng()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wild_grandpa_cheng";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_joe");
		storeClient.spriteName = "joe";
		storeClient.bubbleColor = "pink";
		storeClient.nonShootable = true;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = true;
		storeClient.arrestable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_cheng_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_cheng_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_cheng_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_cheng_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_cheng_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_cheng_6"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("scav_token"), isOwend: true);
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}
}

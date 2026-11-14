using System;
using System.Linq;

public class StoreClientListStory
{
	public const int SAWYER_SHOT_BM_PENALTY = 50;

	public static StoreClient CreateMentorAfterhours()
	{
		return new StoreClient
		{
			identifier = "mentorAfterhours",
			displayName = LocHelper.GetLocalizedClientName("name_the_mentor"),
			spriteName = "mentor",
			clientIntent = StoreClient.ClientIntent.DIALOGUE,
			dismissable = false,
			mainDialogue = StoreClientDialogList.MentorAfterhoursDialog()
		};
	}

	public static StoreClient CreateMentorClient()
	{
		return new StoreClient
		{
			identifier = "mentor",
			displayName = LocHelper.GetLocalizedClientName("name_the_mentor"),
			spriteName = "mentor",
			clientIntent = StoreClient.ClientIntent.DIALOGUE,
			dismissable = false,
			mainDialogue = StoreClientDialogList.MentorIntroDialog()
		};
	}

	public static StoreClient CreateMentorClientEventTip()
	{
		return new StoreClient
		{
			identifier = "mentor_event_tip",
			displayName = LocHelper.GetLocalizedClientName("name_the_mentor"),
			spriteName = "mentor",
			clientIntent = StoreClient.ClientIntent.DIALOGUE,
			dismissable = false,
			mainDialogue = StoreClientDialogList.MentorIntroDialog2()
		};
	}

	public static StoreClient CreateMentorClientVisualInspection()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "mentor_visual_inspection";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_the_mentor");
		storeClient.spriteName = "mentor";
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_visual_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_visual_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_visual_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_visual_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_visual_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_visual_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_visual_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_visual_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_visual_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_visual_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_visual_11"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.isVisualInspectionIntroduced = true;
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteVisualInspection(), isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("cigarette_guide", isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("cigarette_color", isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("magnifier", isOwend: true);
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateMentorClientOutro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "mentor_outro";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_the_mentor");
		storeClient.spriteName = "mentor";
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_outro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_outro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_outro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_outro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_outro_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_outro_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_outro_7"))
			.SetEndAction(delegate
			{
				AchievementTracker.Instance.CheckTutorialComplete();
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateMentorDisplayCase()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "mentor_display_case";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_the_mentor");
		storeClient.spriteName = "mentor";
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_3"))
			.SetEndAction(delegate
			{
				if (PlayerStore.Instance.startType == NewGameData.StartType.APPRENTICE)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("sign_food", isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("sign_household", isOwend: true);
				}
				else if (PlayerStore.Instance.startType == NewGameData.StartType.WATER_MERCHANT)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("sign_water", isOwend: true);
				}
				else if (PlayerStore.Instance.startType == NewGameData.StartType.SCAV)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("sign_material", isOwend: true);
				}
				else
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("sign_food", isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("sign_household", isOwend: true);
				}
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteDisplayCase(), isOwend: true);
				PlayerStore.Instance.isDisplayCaseIntroduced = true;
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_reminder_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_reminder_7"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateMentorDisplayCaseReminder()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "mentor_display_case_reminder";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_the_mentor");
		storeClient.spriteName = "mentor";
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_reminder_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_reminder_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_reminder_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_reminder_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_reminder_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_reminder_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_display_case_reminder_7"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static void SetupSawyer(StoreClient storeClient)
	{
		storeClient.forceShootable = true;
		storeClient.onShotDead = delegate
		{
			PlayerStore.Instance.storeClientData.isSawyerDead = true;
			StoreReputation.ModBMReputation(-50);
			PlayerStore.Instance.exchangeManager.isExchangeUnlocked = false;
			StoreService janitorialService = StoreService.GetJanitorialService();
			janitorialService.unlocked = false;
			janitorialService.isUsing = false;
		};
	}

	public static StoreClient CreateJanitorIntroduction()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "janitorIntroduction";
		SetupSawyer(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_sawyer");
		storeClient.spriteName = "sawyer";
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_intro_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_intro_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_intro_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_intro_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_intro_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_intro_10"))
			.SetEndAction(delegate
			{
				StoreService.GetJanitorialService().introduced = true;
				StoreService.GetJanitorialService().unlocked = true;
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateJanitorUndergroundExchangeIntroduction()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "janitorUEIntroduction";
		SetupSawyer(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_sawyer");
		storeClient.spriteName = "sawyer";
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_8"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.exchangeManager.isExchangeUnlocked = true;
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("exchange_directive"), isOwend: true);
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_11"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateJanitorUndergroundExchangeIntroductionFence()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "janitorUEIntroductionFence";
		SetupSawyer(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_sawyer");
		storeClient.spriteName = "sawyer";
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_fence_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_fence_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_fence_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.exchangeManager.isExchangeUnlocked = true;
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("exchange_directive"), isOwend: true);
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_fence_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_fence_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_fence_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_fence_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_fence_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_janitor_ue_fence_9"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateInsuranceIntroduction()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "insuranceIntroduction";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_joe_wild");
		storeClient.spriteName = "joe";
		storeClient.bubbleColor = "pink";
		storeClient.nonShootable = true;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_insurance_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_insurance_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_insurance_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_insurance_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_insurance_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_insurance_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_insurance_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_insurance_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_insurance_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_insurance_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_insurance_11"))
			.SetEndAction(delegate
			{
				StoreService.GetInsuranceService().introduced = true;
				StoreService.GetInsuranceService().unlocked = true;
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateNetworkIntroduction()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wild2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_joe_wild");
		storeClient.spriteName = "joe";
		storeClient.bubbleColor = "pink";
		storeClient.nonShootable = true;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_network_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_network_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_network_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_network_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_network_intro_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_network_intro_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_network_intro_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_network_intro_8"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.wildFavor += Difficulty.Rules.networkIntroFavor;
				PlayerStore.Instance.isNetworkIntroduced = true;
				PlayerStore.Instance.networkIntroducedDay = StoreStation.Instance.dayCounter;
				WildNetworkUI.Open();
				TutorialUIManager.Instance.isIntroBrowsingUpgrade = true;
			});
		return storeClient;
	}

	public static StoreClient CreateUpgradeReminder()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wild_upgrade_reminder";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_joe_wild");
		storeClient.spriteName = "joe";
		storeClient.bubbleColor = "pink";
		storeClient.nonShootable = true;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_upgrade_reminder_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_upgrade_reminder_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_upgrade_reminder_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_upgrade_reminder_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.daySinceLastBrowseUpgrade = 0;
				WildNetworkUI.Open();
			});
		return storeClient;
	}

	public static StoreClient CreateNetworkReminder()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wild_network_reminder";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_joe_wild");
		storeClient.spriteName = "joe";
		storeClient.bubbleColor = "pink";
		storeClient.nonShootable = true;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_network_reminder_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_network_reminder_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_network_reminder_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_network_reminder_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_network_reminder_5"))
			.SetEndAction(delegate
			{
				if (!EmporiumEntry.Instance.HaveItem("joe_card", ownedOnly: false))
				{
					PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("joe_card"), isOwend: true);
				}
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateWildBailout()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wild_bailout";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_joe_wild");
		storeClient.spriteName = "joe";
		storeClient.bubbleColor = "pink";
		storeClient.nonShootable = true;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_bailout_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_bailout_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_bailout_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.wildFavor += 250;
				EmporiumAudioPlayer.Instance.PlayRegister();
				storeClient.dismissable = true;
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_bailout_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.daySinceLastBrowseUpgrade = 0;
				WildNetworkUI.Open();
			});
		return storeClient;
	}

	public static StoreClient CreateMarketplaceIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wildMarketplaceIntro";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_joe_wild");
		storeClient.spriteName = "joe";
		storeClient.bubbleColor = "pink";
		storeClient.nonShootable = true;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = true;
		storeClient.arrestable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_marketplace_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_marketplace_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_marketplace_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_marketplace_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_marketplace_intro_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_marketplace_intro_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_marketplace_intro_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_marketplace_intro_8"))
			.SetEndAction(delegate
			{
				WildNetworkUI.Open();
			});
		return storeClient;
	}

	public static StoreClient CreateWildDeedVisit()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wild_deed_visit";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_joe_wild");
		storeClient.spriteName = "joe";
		storeClient.bubbleColor = "pink";
		storeClient.nonShootable = true;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_deed_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_deed_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_deed_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_deed_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_deed_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_deed_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_deed_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_deed_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wild_deed_9"))
			.SetEndAction(delegate
			{
				if (!EmporiumEntry.Instance.HaveItem("joe_card", ownedOnly: false))
				{
					PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("joe_card"), isOwend: true);
				}
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateFoodSellerStory()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "food_scav_story";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_seller_1")).SetStartAction(delegate
		{
			TutorialUIManager.Instance.CloseAllTutorial();
		}).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_seller_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend");
				PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend");
				PlayerStore.Instance.AddDirectSellingItemToTable("cat_bar");
				PlayerStore.Instance.AddDirectSellingItemToTable("soda_red");
				PlayerStore.Instance.AddDirectSellingItemToTable("soda_red");
				TutorialUIManager.Instance.BuyCustomerArrived();
			});
		storeClient.OnDismiss = delegate
		{
			TutorialUIManager.Instance.BuyItemDrag();
			TutorialUIManager.Instance.CloseAllTutorial();
		};
		storeClient.OnArrival = delegate
		{
			TutorialUIManager.Instance.OnTutorialCustomerArrived();
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWantedBait()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wantedBait";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_citizen");
		storeClient.spriteName = "wanted_bait1";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(RNG.GetRandomInt(50, 75));
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted_bait_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_wanted_bait_2"))
			.SetEndAction(delegate
			{
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSecWantedIntroduction()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted_intro";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_officer_lun");
		storeClient.spriteName = "officer_lun";
		storeClient.bubbleColor = "sec";
		storeClient.dismissable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro_5"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("wanted_paper"), isOwend: true);
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro_11"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro_12"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		storeClient.OnArrival = delegate
		{
			TutorialUIManager.Instance.OnLunArrived();
		};
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSecWantedIntroduction2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wanted_intro2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_officer_lun");
		storeClient.spriteName = "officer_lun";
		storeClient.bubbleColor = "sec";
		storeClient.dismissable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro2_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro2_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro2_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro2_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro2_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro2_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_wanted_intro2_8"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateFoodSellerStory2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "food_scav_story2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -15;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_seller2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_seller2_2"))
			.SetEndAction(delegate
			{
				NegociationUIManager.Instance.OpenUI();
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateFoodSellerStory3()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "food_scav_story3";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -15;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_seller2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_seller2_2"))
			.SetEndAction(delegate
			{
				TutorialUIManager.Instance.OnBargainTutorialCustomerArrived();
				NegociationUIManager.Instance.OpenUI();
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateStoryScrapSeller()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "story_scrap_seller";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-33);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_scrap_seller_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_scrap_seller_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_scrap_seller_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_scrap_seller_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_scrap_seller_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_scrap_seller_6"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateStoryCigaretteVendor()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "story_cigarette_vendor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_cig_vendor_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_cig_vendor_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_cig_vendor_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(PreBuiltItemHelper.FakeCigaretteStory());
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateStoryCigaretteVendorTrailer()
	{
		StoreClient storeClient = CreateStoryCigaretteVendor();
		storeClient.spriteName = "trailer_scav1";
		return storeClient;
	}

	public static StoreClient CreateLuckyScavStory()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "luckyScavStory";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lucky_scav_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lucky_scav_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lucky_scav_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateEngineering());
				StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateMeteorShower(), 1, IsKnown: false);
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateStoryAppraisalBoard()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "appraisalBoardIntro";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_appraiser_board_member");
		storeClient.spriteName = SpriteDict.GetClerkSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		Dialogue option1 = new Dialogue();
		Dialogue dialogue = new Dialogue();
		Dialogue dialogue2 = new Dialogue();
		Dialogue lastDialogue = new Dialogue();
		lastDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_board_fee", PlayerStore.Instance.GetAppraisalJoinCost()));
		option1.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_choice_join", PlayerStore.Instance.GetAppraisalJoinCost())).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_join_1"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_join_2"))
			.SetStartAction(delegate
			{
				EmporiumAnalytics.TrackAppraisalGuildInteraction("join");
				PlayerStore.Instance.playerCash -= PlayerStore.Instance.GetAppraisalJoinCost();
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteAppraisal(), isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("hand_guide", isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("magnifier", isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("cert1", isOwend: true);
				PlayerStore.Instance.isAppraisalIntroduced = true;
				PlayerStore.Instance.appraisalIntroducedDay = StoreStation.Instance.dayCounter;
			})
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_join_3"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_join_4"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_choice_decline")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_decline_1"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_decline_2"))
			.SetEndAction(delegate
			{
				EmporiumAnalytics.TrackAppraisalGuildInteraction("reject");
				DialogUIManager.Instance.CurrentClientLeave();
			});
		dialogue2.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_choice_later")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_later_1"))
			.SetEndAction(delegate
			{
				EmporiumAnalytics.TrackAppraisalGuildInteraction("ask_to_come_back_later");
				PlayerStore.Instance.QueueFuturClient("appraisalBoardIntro2", 7);
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_board_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_board_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_board_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_board_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_board_intro_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_board_intro_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_board_intro_7"))
			.SetStartAction(delegate
			{
				if (!lastDialogue.choices.Exists(((GameItem, Dialogue, Action) c) => c.Item2 == option1))
				{
					lastDialogue.AddChoice(DirectoryMaster.Item("$"), "", "", option1, delegate
					{
					}, () => PlayerStore.Instance.playerCash >= PlayerStore.Instance.GetAppraisalJoinCost());
				}
			})
			.nextDialogue = lastDialogue;
		lastDialogue.AddChoice(DirectoryMaster.Item("$"), "", "", dialogue, delegate
		{
		}).AddChoice(DirectoryMaster.Item("$"), "", "", dialogue2, delegate
		{
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateStoryAppraisalBoard2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "appraisalBoardIntro2";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_appraiser_board_member");
		storeClient.spriteName = SpriteDict.GetClerkSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		Dialogue option1 = new Dialogue();
		Dialogue dialogue = new Dialogue();
		Dialogue dialogue2 = new Dialogue();
		Dialogue lastDialogue = new Dialogue();
		lastDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_board2_fee"));
		option1.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_choice_join", PlayerStore.Instance.GetAppraisalJoinCost())).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_join_1"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_join_2"))
			.SetStartAction(delegate
			{
				EmporiumAnalytics.TrackAppraisalGuildInteraction("join");
				PlayerStore.Instance.playerCash -= PlayerStore.Instance.GetAppraisalJoinCost();
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteAppraisal(), isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("hand_guide", isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("magnifier", isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("cert1", isOwend: true);
				PlayerStore.Instance.isAppraisalIntroduced = true;
			})
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_join_3"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board2_join_4"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_choice_decline")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_decline_1"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_decline_2"))
			.SetEndAction(delegate
			{
				EmporiumAnalytics.TrackAppraisalGuildInteraction("reject");
				DialogUIManager.Instance.CurrentClientLeave();
			});
		dialogue2.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_choice_later")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_board_later_1"))
			.SetEndAction(delegate
			{
				EmporiumAnalytics.TrackAppraisalGuildInteraction("ask_to_come_back_later");
				PlayerStore.Instance.QueueFuturClient("appraisalBoardIntro2", 7);
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_board2_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_board2_intro_2"))
			.SetStartAction(delegate
			{
				if (!lastDialogue.choices.Exists(((GameItem, Dialogue, Action) c) => c.Item2 == option1))
				{
					lastDialogue.AddChoice(DirectoryMaster.Item("$"), "", "", option1, delegate
					{
					}, () => PlayerStore.Instance.playerCash >= PlayerStore.Instance.GetAppraisalJoinCost());
				}
			})
			.nextDialogue = lastDialogue;
		lastDialogue.AddChoice(DirectoryMaster.Item("$"), "", "", dialogue, delegate
		{
		}).AddChoice(DirectoryMaster.Item("$"), "", "", dialogue2, delegate
		{
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateStoryCigaretteVendor2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "story_cigarette_vendor2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_cig_vendor2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_cig_vendor2_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(PreBuiltItemHelper.FakeCigarette());
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateStoryWaterVendor()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "storyWaterVendor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_citizen");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-12);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_water_vendor_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_water_vendor_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateBaseWater());
				PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GhostWaterAsBaseWater());
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateStoryWaterBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "story_water_buyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(120);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_water_buyer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_water_buyer_2"))
			.SetEndAction(delegate
			{
			});
		storeClient.willAcceptItem = delegate(GameItem targetItem)
		{
			if (targetItem.IsGameItemType("WATER_RATION"))
			{
				return true;
			}
			if (!targetItem.IsTag("WATER"))
			{
				return false;
			}
			return WaterFeatureHelper.GetCurrentWaterDisplay(targetItem).Equals(WaterFeatureHelper.BASE_WATER_DISPLAY) ? true : false;
		};
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.willAcceptItem(targetItem))
			{
				string localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_story_water_buyer_wrong");
				WaterFeatureHelper.OnWrongItemPlaceWaterClient(storeClient, targetItem, localizedDialogue);
			}
			else
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_water_buyer_right"));
				dialogue.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateStoryWaterBuyer2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "story_water_buyer2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(120);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_water_buyer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_water_buyer_2"))
			.SetEndAction(delegate
			{
			});
		storeClient.willAcceptItem = delegate(GameItem targetItem)
		{
			if (targetItem.IsGameItemType("WATER_RATION"))
			{
				return true;
			}
			if (!targetItem.IsTag("WATER"))
			{
				return false;
			}
			return WaterFeatureHelper.GetCurrentWaterDisplay(targetItem).Equals(WaterFeatureHelper.BASE_WATER_DISPLAY) ? true : false;
		};
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.willAcceptItem(targetItem))
			{
				string localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_story_water_buyer_wrong");
				WaterFeatureHelper.OnWrongItemPlaceWaterClient(storeClient, targetItem, localizedDialogue);
			}
			else
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_water_buyer_right"));
				dialogue.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		storeClient.canClientExposeFeature = ClientCanExposeFunc.ClientCanExposeWaterTestingStrip;
		return storeClient;
	}

	public static StoreClient CreateFoodBuyerStory()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "spacerFoodStory";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(61 + RNG.GetRandomInt(0, 8));
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_buyer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_buyer_2", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
				TutorialUIManager.Instance.OnSellArrived();
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.OnDismiss = delegate
		{
			TutorialUIManager.Instance.CloseAllTutorial();
		};
		storeClient.OnArrival = delegate
		{
			TutorialUIManager.Instance.OnTutorialCustomerArrived();
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateOffDutyOfficier()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "offDutyOfficerStoryTreat";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_off_duty_officer");
		storeClient.spriteName = SpriteDict.GetRandomOffDutySecSpriteName();
		storeClient.clientFaction = (storeClient.clientFaction = "FACTION_SECURITY");
		storeClient.SetBudget(100 + RNG.GetRandomInt(0, 15));
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_off_duty_officer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_off_duty_officer_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_off_duty_officer_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_off_duty_officer_4", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("TREAT");
		storeClient.clientBuyingTagList.Add("CIGARETTE_TAG");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		storeClient.CompleteSecClient();
		return storeClient;
	}

	public static StoreClient CreateOffDutyOfficerBeer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "offDutyOfficerStoryBeer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_off_duty_officer");
		storeClient.SetBudget(75 + RNG.GetRandomInt(0, 25));
		storeClient.AddBasicCheck();
		storeClient.spriteName = SpriteDict.GetRandomOffDutySecSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_off_duty_officer_beer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_off_duty_officer_beer_2", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("ALCOHOL");
		storeClient.CompleteClientCreation();
		storeClient.CompleteSecClient();
		return storeClient;
	}

	public static StoreClient CreateFoodBuyerStory2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "spacerFoodStory2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(75 + RNG.GetRandomInt(0, 25));
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_buyer2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_buyer2_2", storeClient.GetBudget()))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_buyer2_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_buyer2_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_story_food_buyer2_5"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.clientBuyingTagList.Add("BREVAGE");
		storeClient.SetVariety();
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLoanClerkEarly()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "loanClerk0";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_loan_clerk");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_early_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_early_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_early_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_early_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_early_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_early_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_early_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_early_8"))
			.SetEndAction(delegate
			{
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLoanClerk()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "loanClerk1";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_loan_clerk");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_11"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_12"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk_12"))
			.SetEndAction(delegate
			{
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLoanClerk2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "loanClerk2";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_loan_clerk");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk2_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk2_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_clerk2_4"))
			.SetEndAction(delegate
			{
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateInventorIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "inventorIntro";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_dr._jackson");
		storeClient.spriteName = SpriteDict.GetInventorSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.nonShootable = true;
		storeClient.isMultiBuyDisabled = true;
		storeClient.isBargainAvailable = false;
		storeClient.dismissable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_intro_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_intro_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_intro_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_intro_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_intro_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_intro_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_intro_11"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.storeClientData.isJacksonIntroduced = true;
				storeClient.dismissable = true;
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(PreBuiltItemHelper.CreateFurnace());
			});
		storeClient.onItemAddedToWeighted = delegate(GameItem item)
		{
			foreach (GameItem item in EmporiumEntry.Instance.backInvinvElement.childItems.ToList())
			{
				if (item != item && !GeneralHelper.IsItemOwned(item))
				{
					GraphUtils.TryAcceptAll(EmporiumEntry.Instance.frontInvinvElement, item);
				}
			}
			if (!GeneralHelper.IsItemOwned(item))
			{
				item.AddItemFeature(ItemFeatureList.FirstTimeDiscount());
			}
		};
		storeClient.onItemAddedToCounters = delegate(GameItem item)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				item.AddItemFeature(ItemFeatureList.FirstTimeDiscount());
			}
		};
		storeClient.onItemRemovedToCounters = delegate(GameItem item)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				item.itemFeatures.Remove(item.FindItemFeatureByID("firstTimeDiscount"));
			}
		};
		storeClient.onDealAccepted = delegate
		{
			PlayerStore.Instance.RemoveNotOwnedItemFromTable();
		};
		storeClient.acceptDealDialogue = new Dialogue();
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_accept_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_accept_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCredit(50), isOwend: true);
				if (PlayerStore.Instance.IsPlayerOwnThisItem("furnace"))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("ingot_book", isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent", isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent", isOwend: true);
				}
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_accept_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_accept_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_accept_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_accept_6"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateInventorModuleIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "inventorModuleIntro";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_dr._jackson");
		storeClient.spriteName = SpriteDict.GetInventorSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.nonShootable = true;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isBargainAvailable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_module_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.storeClientData.isModuleIntroduced = true;
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("system_module_performance"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("system_module_efficiancy"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("system_module_quality"));
		}).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_module_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_module_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_module_4"))
			.SetEndAction(delegate
			{
			});
		storeClient.onItemAddedToWeighted = delegate(GameItem item)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				item.AddItemFeature(ItemFeatureList.FirstTimeDiscount());
			}
		};
		storeClient.onItemRemovedToWeighted = delegate(GameItem item)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				item.itemFeatures.Remove(item.FindItemFeatureByID("firstTimeDiscount"));
			}
		};
		storeClient.onItemAddedToCounters = delegate(GameItem item)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				item.AddItemFeature(ItemFeatureList.FirstTimeDiscount());
			}
		};
		storeClient.onItemRemovedToCounters = delegate(GameItem item)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				item.itemFeatures.Remove(item.FindItemFeatureByID("firstTimeDiscount"));
			}
		};
		storeClient.allDoneDialogue = new Dialogue();
		storeClient.allDoneDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_module_accept_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_module_accept_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_module_accept_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_module_accept_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateMentorSecRecord()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "mentor_sec_record";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_the_mentor");
		storeClient.spriteName = "mentor";
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_sec_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.isVisualInspectionIntroduced = true;
			PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("intel_sec"), isOwend: true);
		}).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_sec_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_sec_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_sec_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_sec_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_sec_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_sec_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_sec_8"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateMentorPermit()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "mentor_permit";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_the_mentor");
		storeClient.spriteName = "mentor";
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_permit_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_permit_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_permit_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_permit_4", PlayerStore.GetBusinessPermitPrice()))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_permit_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_permit_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_permit_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_permit_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_permit_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_permit_10"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}

	public static StoreClient CreateBrokenMachineIntroduction()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "brokenMachineIntro";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scrapper");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.dismissable = false;
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_broken_machine_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_broken_machine_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_broken_machine_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_broken_machine_intro_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.isBrokenMachineIntroduced = true;
				PlayerStore.Instance.AddDirectSellingItemToTable("ruined_machine_small");
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMentorShotgun()
	{
		return new StoreClient
		{
			identifier = "mentor_shotgun",
			displayName = LocHelper.GetLocalizedClientName("name_the_mentor"),
			spriteName = "mentor",
			clientIntent = StoreClient.ClientIntent.DIALOGUE,
			dismissable = false,
			mainDialogue = StoreClientDialogList.MentorShotgunDialog()
		};
	}

	public static StoreClient CreateMentorBargainIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "mentor_bargain";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_the_mentor");
		storeClient.spriteName = "mentor";
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_bargain_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.isBargainIntroduced = true;
		}).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_bargain_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_bargain_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_bargain_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_bargain_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_bargain_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_bargain_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_mentor_bargain_8"))
			.SetEndAction(delegate
			{
				NegociationUIManager.Instance.CloseUI();
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return storeClient;
	}
}

public class StoreClientDialogList
{
	private static readonly LocalizedLines acceptedDealDialogues = LocalizedLines.Dialogue("dialog_deal_accepted_1", "dialog_deal_accepted_2", "dialog_deal_accepted_3", "dialog_deal_accepted_4", "dialog_deal_accepted_5", "dialog_deal_accepted_6", "dialog_deal_accepted_7", "dialog_deal_accepted_8", "dialog_deal_accepted_9", "dialog_deal_accepted_10");

	private static readonly LocalizedLines doneDealDialogues = LocalizedLines.Dialogue("dialog_deal_done_1", "dialog_deal_done_2", "dialog_deal_done_3", "dialog_deal_done_4", "dialog_deal_done_5", "dialog_deal_done_6", "dialog_deal_done_7", "dialog_deal_done_8", "dialog_deal_done_9", "dialog_deal_done_10", "dialog_deal_done_11", "dialog_deal_done_12", "dialog_deal_done_13", "dialog_deal_done_14");

	private static readonly LocalizedLines arrestClient = LocalizedLines.Dialogue("dialog_arrest_client_1", "dialog_arrest_client_2", "dialog_arrest_client_3", "dialog_arrest_client_4", "dialog_arrest_client_5");

	private static readonly LocalizedLines arrestClientBlackMarket = LocalizedLines.Dialogue("dialog_arrest_blackmarket_1", "dialog_arrest_blackmarket_2", "dialog_arrest_blackmarket_3", "dialog_arrest_blackmarket_4", "dialog_arrest_blackmarket_5");

	private static readonly LocalizedLines arrestClientCriminal = LocalizedLines.Dialogue("dialog_arrest_criminal_1", "dialog_arrest_criminal_2", "dialog_arrest_criminal_3", "dialog_arrest_criminal_4", "dialog_arrest_criminal_5", "dialog_arrest_criminal_6");

	private static readonly LocalizedLines arrestSec = LocalizedLines.Dialogue("dialog_arrest_sec_1", "dialog_arrest_sec_2", "dialog_arrest_sec_3", "dialog_arrest_sec_4", "dialog_arrest_sec_5", "dialog_arrest_sec_6");

	public static string GetRandomDialogAcceptedString()
	{
		return acceptedDealDialogues[RNG.GetRandomInt(0, acceptedDealDialogues.Count - 1)];
	}

	public static Dialogue GetRandomDialogAccepted()
	{
		Dialogue dialogue = new Dialogue().SetText("CLIENT", GetRandomDialogAcceptedString());
		dialogue.priority = 4;
		return dialogue;
	}

	public static string GetRandomDialogDoneString()
	{
		return doneDealDialogues[RNG.GetRandomInt(0, doneDealDialogues.Count - 1)];
	}

	public static Dialogue GetRandomDialogDone()
	{
		Dialogue dialogue = new Dialogue().SetText("CLIENT", GetRandomDialogDoneString());
		dialogue.priority = 4;
		return dialogue;
	}

	public static Dialogue CreateOutOfBudgetDialogue(string clientName)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 4;
		if (PlayerStore.Instance.currentClientInstance == null || PlayerStore.Instance.currentClientInstance.GetClientBlueprint().GetBudget() == 0)
		{
			dialogue.SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_out_of_budget")).SetEndAction(delegate
			{
			});
		}
		else
		{
			dialogue.SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_out_of_budget_amount", PlayerStore.Instance.currentClientInstance.GetClientBlueprint().GetBudget())).SetEndAction(delegate
			{
			});
		}
		return dialogue;
	}

	public static Dialogue CreateAlreadyBoughtDialog(string clientName)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 4;
		dialogue.SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_already_bought")).SetEndAction(delegate
		{
		});
		return dialogue;
	}

	public static Dialogue CreateNoContrabandDialog(string clientName)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 4;
		dialogue.SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_no_contraband")).SetEndAction(delegate
		{
		});
		return dialogue;
	}

	public static Dialogue CreateDefaultAllDoneDialog(string clientName)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 4;
		dialogue.SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_all_done_1")).NextDialogue().SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_all_done_2"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		return dialogue;
	}

	public static Dialogue CreateShutterDownClose(string clientName)
	{
		Dialogue dialogue = new Dialogue();
		if (RNG.Roll(10))
		{
			dialogue.SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_shutter_1")).NextDialogue().SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_shutter_2"))
				.SetEndAction(delegate
				{
				});
		}
		else if (RNG.Roll(10))
		{
			dialogue.SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_shutter_3"));
		}
		else
		{
			dialogue.SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_shutter_4"));
		}
		return dialogue;
	}

	public static Dialogue CreateAfterBuyingFromClientDialog(string clientName)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 4;
		dialogue.SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_after_buy"));
		return dialogue;
	}

	public static Dialogue CreateAfterSellingToClientDialog(string clientName)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 4;
		if (PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint().useClientBudget && PlayerStore.Instance.currentClientInstance.GetClientBlueprint().GetBudget() < 10)
		{
			dialogue.SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_after_sell_done"));
		}
		else
		{
			dialogue.SetText(clientName, LocHelper.GetLocalizedDialogue("dialog_util_after_sell"));
		}
		return dialogue;
	}

	public static Dialogue MentorAfterhoursDialog()
	{
		Dialogue dialogue = new Dialogue();
		string localizedClientName = LocHelper.GetLocalizedClientName("name_mentor");
		dialogue.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_afterhours_1")).NextDialogue().SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_afterhours_2"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_afterhours_3"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_afterhours_4"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_afterhours_5"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_afterhours_6"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_afterhours_7"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.isAfterhoursIntroduced = true;
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteAfterhours(), isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("topical_bandage_item", isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("fanny_pack", isOwend: true);
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return dialogue;
	}

	public static Dialogue MentorIntroDialog()
	{
		Dialogue dialogue = new Dialogue();
		string localizedClientName = LocHelper.GetLocalizedClientName("name_mentor");
		dialogue.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro_1")).NextDialogue().SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro_2"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro_3"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteBasicTrading(), isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteFood(), isOwend: true);
				TutorialUIManager.Instance.OnMentorDone();
			})
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro_5"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro_6"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro_7"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro_8"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro_9"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro_10"))
			.NextDialogue()
			.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_mentor_intro_player"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
				DialogSettingUIManager.Instance.OpenUI();
			});
		return dialogue;
	}

	public static Dialogue MentorIntroDialog2()
	{
		Dialogue dialogue = new Dialogue();
		string localizedClientName = LocHelper.GetLocalizedClientName("name_mentor");
		dialogue.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro2_1")).NextDialogue().SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro2_2"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro2_3"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro2_4"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro2_5"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro2_6"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_intro2_7"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNote2(), isOwend: true);
				StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateMiningExpeditionReturn(), 1, IsKnown: true);
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return dialogue;
	}

	public static Dialogue SecurityDialogInspectionCleared()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 7;
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_cleared_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_cleared_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return dialogue;
	}

	public static Dialogue SecurityDialogInspectionWarning()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 7;
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_warning_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_warning_2"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_warning_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return dialogue;
	}

	public static Dialogue SecurityDialogInspectionSeizure()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 7;
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_seizure_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_seizure_2"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_seizure_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return dialogue;
	}

	public static Dialogue SecurityDialogInspectionSeizureStolen()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 7;
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_seizure_stolen_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_seizure_2"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_seizure_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return dialogue;
	}

	public static Dialogue SecurityDialogOnContrabandOnCounter()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 6;
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_contraband_counter_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_contraband_counter_2"))
			.NextDialogue()
			.SetText("CLIENT", "...")
			.NextDialogue()
			.SetText("CLIENT", "...")
			.SetEndAction(delegate
			{
				ContrabandHelper.StartInspection(0, fromShowcase: true);
			});
		return dialogue;
	}

	public static Dialogue SecurityDialogOnStolenOnCounter()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 6;
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_stolen_counter_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_contraband_counter_2"))
			.NextDialogue()
			.SetText("CLIENT", "...")
			.NextDialogue()
			.SetText("CLIENT", "...")
			.SetEndAction(delegate
			{
				ContrabandHelper.StartInspection(0, fromShowcase: true);
			});
		return dialogue;
	}

	public static Dialogue SecurityDialogInspectionLastLine(int personality = 0, bool fromShowcase = false)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_choice_accept"));
		Dialogue lastLine = new Dialogue();
		lastLine.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_last_open")).SetStartAction(delegate
		{
			EmporiumAnalytics.TrackInspectorArrived(personality, fromShowcase);
			if (personality == 1)
			{
				int bribeAmount = ContrabandHelper.GetBribeAmount();
				if (PlayerStore.Instance.CanPay(bribeAmount))
				{
					Dialogue dialogue2 = new Dialogue();
					dialogue2.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_choice_bribe", bribeAmount));
					dialogue2.NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_bribe_accept")).SetEndAction(delegate
					{
						EmporiumAnalytics.TrackInspectionBribe(bribeAmount);
						PlayerStore.Instance.PayMoney(bribeAmount, playRegisterSound: true);
						RunStats.OnInspectorBribe(bribeAmount);
						PlayerStore.Instance.secData.CommitBribery(bribeAmount);
						PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
					});
					lastLine.AddChoice(DirectoryMaster.Item("yes"), LocHelper.GetLocalizedDialogue("sec_inspection_bribe_label", bribeAmount), "", dialogue2, delegate
					{
					});
				}
			}
		}).SetEndAction(delegate
		{
		})
			.AddChoice(DirectoryMaster.Item("yes"), LocHelper.GetLocalizedDialogue("sec_inspection_accept_label"), "", dialogue, delegate
			{
			});
		dialogue.NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("sec_shoot_ellipsis")).NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("sec_shoot_ellipsis"))
			.SetEndAction(delegate
			{
				ContrabandHelper.StartInspection(personality, fromShowcase);
			});
		return lastLine;
	}

	public static Dialogue SecurityDialogInspectionFine(int fine)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 7;
		if (PlayerStore.Instance.playerCash < fine)
		{
			Dialogue dialogue2 = new Dialogue();
			dialogue2.priority = 7;
			Dialogue dialogue3 = dialogue2.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_fine_contraband")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_fine_amount", fine))
				.NextDialogue()
				.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_fine_no_money_player"));
			if (NetworkUpgrade.IsSecGraceUnlocked())
			{
				dialogue3.NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_grace_1")).NextDialogue()
					.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_grace_2"))
					.NextDialogue()
					.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_grace_3"))
					.SetEndAction(delegate
					{
						PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
						NetworkUpgrade.GetUpgradeById("SEC_GRACE").state = 1;
					});
				return dialogue2;
			}
			dialogue3.NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_fine_arrest")).SetEndAction(delegate
			{
				if (PlayerStore.Instance.playerCash >= fine)
				{
					PlayerStore.Instance.PayMoney(fine, playRegisterSound: true);
					PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
				}
				else
				{
					PlayerStore.Instance.ExecuteGameOver("arrest");
					PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
				}
			});
			return dialogue2;
		}
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_fine_contraband")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_fine_seized"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_fine_pay_now", fine))
			.NextDialogue()
			.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_fine_pay_player"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_fine_thanks"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.PayMoney(fine, playRegisterSound: true);
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return dialogue;
	}

	public static Dialogue SecurityDialogInspectionArrest()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 7;
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_arrest_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_arrest_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.ExecuteGameOver("arrest");
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return dialogue;
	}

	public static Dialogue SecurityDialogInspectionGrace()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 7;
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_arrest_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_grace_1"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_grace_2"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_grace_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
				NetworkUpgrade.GetUpgradeById("SEC_GRACE").state = 1;
			});
		return dialogue;
	}

	public static Dialogue SecurityOnShowLowLevel()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_show_low_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_show_low_2"))
			.SetEndAction(delegate
			{
			});
		return dialogue;
	}

	public static Dialogue SecurityOnShowLowLevelBad()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_show_low_bad")).SetEndAction(delegate
		{
		});
		return dialogue;
	}

	public static string GetRandomArrestClient()
	{
		if (PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint().clientFaction == "FACTION_BLACK_MARKET")
		{
			return arrestClientBlackMarket[RNG.GetRandomInt(0, arrestClientBlackMarket.Count - 1)];
		}
		return arrestClient[RNG.GetRandomInt(0, arrestClient.Count - 1)];
	}

	public static string GetRandomArrestClientCriminal()
	{
		return arrestClientCriminal[RNG.GetRandomInt(0, arrestClientCriminal.Count - 1)];
	}

	public static string GetRandomArrestSec()
	{
		return arrestSec[RNG.GetRandomInt(0, arrestSec.Count - 1)];
	}

	public static Dialogue ClientArrestDialog()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 7;
		dialogue.SetText("CLIENT", GetRandomArrestClient()).NextDialogue().SetText("SEC", GetRandomArrestSec())
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		return dialogue;
	}

	public static Dialogue ClientAlarmInitialReactionDialog()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 6;
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_arrest_alarm_reaction"));
		return dialogue;
	}

	public static Dialogue ClientArrestDialogCriminal()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("CLIENT", GetRandomArrestClientCriminal()).NextDialogue().SetText("SEC", GetRandomArrestSec())
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		return dialogue;
	}

	public static Dialogue WildNetworkContinue()
	{
		Dialogue dialogue = new Dialogue();
		string title = "CLIENT";
		dialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wild_network_1")).NextDialogue().SetText(title, LocHelper.GetLocalizedDialogue("dialog_wild_network_2"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wild_network_3"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wild_network_4"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wild_network_5"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wild_network_6"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wild_network_7"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("joe_card"), isOwend: true);
				StorePhoneClient.GetWildPhoneClient().phoneState = StorePhoneClient.PhoneState.Regular;
			})
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("dialog_wild_network_8"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return dialogue;
	}

	public static Dialogue SecAugDialog()
	{
		Dialogue dialogue = new Dialogue();
		string title = "SEC";
		dialogue.SetText(title, LocHelper.GetLocalizedDialogue("sec_aug_gone")).NextDialogue().SetText(title, LocHelper.GetLocalizedDialogue("sec_aug_if_you_see"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("sec_aug_shoot_on_sight"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("sec_aug_hacked_comms"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("sec_aug_stay_safe"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.StartLeaveSec();
				PlayerStore.Instance.isClientBeingArrested = false;
			});
		return dialogue;
	}

	public static Dialogue SecShootoutDialog(int situation)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 7;
		string title = "SEC";
		Dialogue dialogue2 = dialogue.SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_got_call")).NextDialogue().SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_check_aug"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_ellipsis"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_ellipsis"));
		switch (situation)
		{
		case 0:
			dialogue2.NextDialogue().SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_good_0")).NextDialogue()
				.SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_good_1"))
				.NextDialogue()
				.SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_good_2"))
				.SetEndAction(delegate
				{
					AugHelper.OnGoodKill();
					AugHelper.OnShootoutLeave();
				});
			break;
		case 2:
		{
			StoreClient shot = AugHelper.GetShotClient()?.GetClientBlueprint();
			Dialogue dialogue3 = ((shot != null && shot.shotCaptureDialogue != null) ? shot.shotCaptureDialogue() : null);
			if (dialogue3 == null)
			{
				dialogue3 = SecShotWantedGenericDialog((shot != null) ? shot.realName : "");
			}
			Dialogue dialogue4 = dialogue3;
			for (Dialogue dialogue5 = dialogue3; dialogue5 != null; dialogue5 = dialogue5.nextDialogue)
			{
				dialogue5.priority = dialogue.priority;
				dialogue4 = dialogue5;
			}
			dialogue2.SetNextDialogue(dialogue3);
			dialogue4.SetEndAction(delegate
			{
				if (shot != null)
				{
					PlayerStore.Instance.OnWantedClientShot(shot);
				}
				AugHelper.OnShootoutLeave();
			});
			break;
		}
		default:
			dialogue2.NextDialogue().SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_bad_0")).NextDialogue()
				.SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_bad_1"))
				.NextDialogue()
				.SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_arrest"))
				.NextDialogue()
				.SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_arrest2"))
				.NextDialogue()
				.SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_arrest3"))
				.SetEndAction(delegate
				{
					AugHelper.CleanupKill();
				});
			break;
		}
		return dialogue;
	}

	public static Dialogue SecShotWantedGenericDialog(string wantedName)
	{
		string title = "SEC";
		Dialogue dialogue = new Dialogue();
		dialogue.SetText(title, LocHelper.GetLocalizedDialogue("sec_shoot_bad_0")).NextDialogue().SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "sec_shoot_wanted_1", "Wait. I know this face. {0}, from the wanted list.", wantedName))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "sec_shoot_wanted_2", "That gun is for Augs, Shopkeeper. Point it at a citizen again and we'll have a very different conversation."))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "sec_shoot_wanted_3", "But nobody's going to lose sleep over a dangerous criminal. We'll log it as a capture. The bounty's yours."));
		return dialogue;
	}

	public static Dialogue MentorShotgunDialog()
	{
		Dialogue dialogue = new Dialogue();
		string localizedClientName = LocHelper.GetLocalizedClientName("name_mentor");
		dialogue.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_shotgun_1")).NextDialogue().SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_shotgun_2"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_shotgun_3"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_shotgun_4"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_shotgun_5"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_shotgun_6"))
			.SetEndAction(delegate
			{
				Shotgun.Unlock();
			})
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_shotgun_7"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_shotgun_8"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_shotgun_9"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_shotgun_10"))
			.NextDialogue()
			.SetText(localizedClientName, LocHelper.GetLocalizedDialogue("dialog_mentor_shotgun_11"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteShotgun(), isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("aug_id_guide", isOwend: true);
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return dialogue;
	}
}

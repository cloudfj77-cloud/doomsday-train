using System.Collections.Generic;

public class StoreClientListCartel
{
	public const string CARTEL_HANDLER_NAME = "Cartel Handler";

	private const int CARTEL_COUNT_STEP = 5;

	private const int CARTEL_PURITY_STEP = 5;

	private const int CARTEL_PURITY_CAP = 70;

	private const int FINAL_EXIT_STAGE = 4;

	public static StoreClient CreateCartelIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "cartelIntro";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_cartel_handler");
		storeClient.spriteName = SpriteDict.GetCartelHandlerSpriteName();
		StoreClientFactionSetup.InitCartel(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_intro_5", PlayerStore.Instance.cartelRequiredCount))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_intro_6", PlayerStore.Instance.cartelRequiredPurity))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_intro_7"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap", isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap", isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap", isOwend: true);
				if (PlayerStore.Instance.cartelRequiredCount >= 15)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap", isOwend: true);
				}
				if (PlayerStore.Instance.cartelRequiredCount >= 25)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap", isOwend: true);
				}
				if (PlayerStore.Instance.cartelRequiredCount >= 35)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap", isOwend: true);
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("common_chemical", isOwend: true);
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_intro_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_intro_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_intro_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_intro_11"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_intro_12"))
			.SetEndAction(delegate
			{
				StoreService.GetInsuranceService().unlocked = false;
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCartelSec()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "cartelSec";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_security_officer");
		storeClient.spriteName = SpriteDict.GetRandomSecSpriteName();
		StoreClientFactionSetup.InitCartel(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		Dialogue reportDialog = new Dialogue();
		Dialogue noReportDialog = new Dialogue();
		reportDialog.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_cartel_sec_choice_report"));
		noReportDialog.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_cartel_sec_choice_no_report"));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_sec_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_sec_2"))
			.AddChoice(DirectoryMaster.Item("$"), "-", "", reportDialog, delegate
			{
				reportDialog.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_sec_report_response")).SetEndAction(delegate
				{
					PlayerStore.Instance.storeClientManager.AddNextClient(CreateCartelSecSnitched());
					PlayerStore.Instance.DismissCurrentClient();
				});
			})
			.AddChoice(DirectoryMaster.Item("$"), "-", "", noReportDialog, delegate
			{
				noReportDialog.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_sec_no_report_response")).SetEndAction(delegate
				{
					PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
				});
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCartelSecSnitched()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "cartelSecSnitched";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_cartel_handler");
		storeClient.spriteName = SpriteDict.GetCartelHandlerSpriteName();
		StoreClientFactionSetup.InitCartel(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_snitched_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_snitched_2"))
			.SetEndAction(delegate
			{
				StoreReputation.ModCartelReputation(-20);
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_snitched_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_snitched_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_snitched_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_snitched_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_snitched_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_snitched_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_snitched_9"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCartelHandlerCheckin()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "cartelHandlerCheckin";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_cartel_handler");
		storeClient.spriteName = SpriteDict.GetCartelHandlerSpriteName();
		StoreClientFactionSetup.InitCartel(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		Dialogue needIngredientDialog = new Dialogue();
		Dialogue allGoodDialog = new Dialogue();
		needIngredientDialog.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_choice_ingredient", GetRepCost()));
		allGoodDialog.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_choice_ok"));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_1"));
		Dialogue countNode = storeClient.mainDialogue.NextDialogue();
		countNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_2", PlayerStore.Instance.cartelRequiredCount));
		countNode.SetStartAction(delegate
		{
			countNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_2", PlayerStore.Instance.cartelRequiredCount));
		});
		Dialogue purityNode = countNode.NextDialogue();
		purityNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_3", PlayerStore.Instance.cartelRequiredPurity));
		purityNode.SetStartAction(delegate
		{
			purityNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_3", PlayerStore.Instance.cartelRequiredPurity));
		});
		Dialogue dialogue = purityNode.NextDialogue();
		dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_4"));
		dialogue.SetStartAction(delegate
		{
			needIngredientDialog.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_choice_ingredient", GetRepCost()));
		});
		dialogue.AddChoice(DirectoryMaster.Item("$"), "-", "", needIngredientDialog, delegate
		{
			int repCost = GetRepCost();
			if (repCost == -6)
			{
				needIngredientDialog.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_reliable_1")).NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_reliable_2"))
					.SetEndAction(delegate
					{
						AddCartelMaterial();
						StoreReputation.ModCartelReputation(repCost);
						PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
					});
			}
			else
			{
				needIngredientDialog.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_unreliable_1")).NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_unreliable_2"))
					.NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_unreliable_3"))
					.SetEndAction(delegate
					{
						AddCartelMaterial();
						StoreReputation.ModCartelReputation(repCost);
						PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
					});
			}
		}).AddChoice(DirectoryMaster.Item("$"), "-", "", allGoodDialog, delegate
		{
			allGoodDialog.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_checkin_all_good")).SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
		static int GetRepCost()
		{
			if (!StoreReputation.IsPerkUnlocked("CARTEL_RELIABLE_CHEMIST"))
			{
				return -12;
			}
			return -6;
		}
	}

	public static StoreClient CreateCartelHandlerWeekly()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "cartelHandlerWeekly";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_cartel_handler");
		storeClient.spriteName = SpriteDict.GetCartelHandlerSpriteName();
		StoreClientFactionSetup.InitCartel(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		Dialogue yesDialog = new Dialogue();
		yesDialog.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_choice_continue"));
		Dialogue countAskNode = storeClient.mainDialogue;
		countAskNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_1", PlayerStore.Instance.cartelRequiredCount));
		countAskNode.SetStartAction(delegate
		{
			countAskNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_1", PlayerStore.Instance.cartelRequiredCount));
		});
		Dialogue purityAskNode = countAskNode.NextDialogue();
		purityAskNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_2", PlayerStore.Instance.cartelRequiredPurity));
		purityAskNode.SetStartAction(delegate
		{
			purityAskNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_2", PlayerStore.Instance.cartelRequiredPurity));
		});
		purityAskNode.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_3")).AddChoice(DirectoryMaster.Item("$"), "-", "", yesDialog, delegate
		{
			List<GameItem> handedDust = new List<GameItem>();
			EmporiumEntry.Instance.GetCountersItems().ForEach(delegate(GameItem item)
			{
				if (item.IsID("dream_dust"))
				{
					handedDust.Add(item);
					GeneralHelper.SetItemOwned(item, own: false);
				}
			});
			int count = handedDust.Count;
			if (count == 0)
			{
				yesDialog.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_no_dust_1")).NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_no_dust_2"))
					.NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_no_dust_3"))
					.SetEndAction(delegate
					{
						AudioManager.Instance.Play2("gunshot");
						PlayerStore.Instance.ExecuteGameOver("cartel_retired");
					});
			}
			else
			{
				double num = 0.0;
				foreach (GameItem item in handedDust)
				{
					num += item.GetTagReadonly("CHEMICAL_PRODUCT_PURITY_TAG").GetDouble();
				}
				double averagePurity = num / (double)count;
				int reward;
				Dialogue dialogue;
				if (count < PlayerStore.Instance.cartelRequiredCount)
				{
					reward = 0;
					dialogue = yesDialog.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_short_1")).NextDialogue()
						.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_short_2", PlayerStore.Instance.cartelRequiredCount))
						.NextDialogue()
						.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_short_3"));
					dialogue.SetEndAction(delegate
					{
						int num2 = PlayerStore.Instance.cartelRequiredCount - count;
						StoreReputation.ModCartelReputation(-4 * num2);
						ConsumeDust(handedDust);
					});
				}
				else if (count == PlayerStore.Instance.cartelRequiredCount)
				{
					reward = 1;
					dialogue = yesDialog.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_met"));
					dialogue.SetEndAction(delegate
					{
						StoreReputation.ModCartelReputation(count);
						ConsumeDust(handedDust);
					});
				}
				else
				{
					reward = 1;
					dialogue = yesDialog.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_weekly_exceeded"));
					dialogue.SetEndAction(delegate
					{
						StoreReputation.ModCartelReputation(count);
						ConsumeDust(handedDust);
					});
				}
				dialogue.SetNextDialogue(BuildPurityChain(storeClient, reward, averagePurity));
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static void ConsumeDust(List<GameItem> handedDust)
	{
		foreach (GameItem item in handedDust)
		{
			if (item != null && item.parentInventory != null)
			{
				item.Destroy();
			}
		}
	}

	private static Dialogue BuildPurityChain(StoreClient storeClient, int reward, double averagePurity)
	{
		Dialogue dialogue = new Dialogue();
		Dialogue dialogue2 = dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_purity_intro")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_purity_pause"));
		if (averagePurity <= (double)(PlayerStore.Instance.cartelRequiredPurity / 4))
		{
			dialogue2.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_purity_scam_1")).NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_purity_scam_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_purity_scam_3"))
				.SetEndAction(delegate
				{
					AudioManager.Instance.Play2("gunshot");
					PlayerStore.Instance.ExecuteGameOver("cartel_retired");
				});
			return dialogue;
		}
		if (averagePurity <= (double)(PlayerStore.Instance.cartelRequiredPurity - 5))
		{
			Dialogue dialogue3 = dialogue2.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_purity_low_1")).NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_purity_low_2"));
			dialogue3.SetEndAction(delegate
			{
				StoreReputation.ModCartelReputation(-(int)((double)PlayerStore.Instance.cartelRequiredPurity - averagePurity));
			});
			dialogue3.SetNextDialogue(BuildNextWeekChain(storeClient, reward));
			return dialogue;
		}
		if (averagePurity <= (double)(PlayerStore.Instance.cartelRequiredPurity + 10))
		{
			Dialogue dialogue4 = dialogue2.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_purity_good"));
			dialogue4.SetEndAction(delegate
			{
				StoreReputation.ModCartelReputation(-(int)((double)PlayerStore.Instance.cartelRequiredPurity - averagePurity));
			});
			dialogue4.SetNextDialogue(BuildNextWeekChain(storeClient, reward + 1));
			return dialogue;
		}
		Dialogue dialogue5 = dialogue2.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_purity_high_1")).NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_purity_high_2"));
		dialogue5.SetEndAction(delegate
		{
			StoreReputation.ModCartelReputation((int)(averagePurity - (double)PlayerStore.Instance.cartelRequiredPurity) / 4);
		});
		dialogue5.SetNextDialogue(BuildNextWeekChain(storeClient, reward + 1));
		return dialogue;
	}

	private static int GetNextWeekCountTarget()
	{
		return PlayerStore.Instance.cartelRequiredCount + 5;
	}

	private static int GetNextWeekPurityTarget()
	{
		int num = PlayerStore.Instance.cartelRequiredPurity + 5;
		if (num < 70)
		{
			return num;
		}
		return 70;
	}

	private static Dialogue BuildNextWeekChain(StoreClient storeClient, int reward)
	{
		Dialogue chain = new Dialogue();
		chain.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_next_week_1"));
		chain.SetStartAction(delegate
		{
			DialogUIManager.Instance.FlushScheduledEndActions();
			if (IsExitAuthorized() && PlayerStore.Instance.cartelExitStage >= 4)
			{
				PlayerStore.Instance.cartelExitStage++;
				chain.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_next_week_4"));
				chain.SetEndAction(delegate
				{
					PayOut(commitTerms: false);
				});
				chain.SetNextDialogue(BuildExitOkayChain(storeClient));
			}
		});
		Dialogue targetCountNode = chain.NextDialogue();
		targetCountNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_next_week_2", GetNextWeekCountTarget()));
		targetCountNode.SetStartAction(delegate
		{
			targetCountNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_next_week_2", GetNextWeekCountTarget()));
		});
		Dialogue targetPurityNode = targetCountNode.NextDialogue();
		targetPurityNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_next_week_3", GetNextWeekPurityTarget()));
		targetPurityNode.SetStartAction(delegate
		{
			targetPurityNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_next_week_3", GetNextWeekPurityTarget()));
		});
		Dialogue dialogue = targetPurityNode.NextDialogue();
		dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_next_week_4"));
		dialogue.SetEndAction(delegate
		{
			PayOut(commitTerms: true);
		});
		Dialogue farewellNode = dialogue.NextDialogue();
		farewellNode.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_next_week_5"));
		farewellNode.SetStartAction(delegate
		{
			if (IsExitAuthorized())
			{
				Dialogue nextDialogue = PlayerStore.Instance.cartelExitStage switch
				{
					0 => BuildExitChain(storeClient, "dialog_cartel_exit1_1", "dialog_cartel_exit1_2", "dialog_cartel_exit1_3"), 
					1 => BuildExitChain(storeClient, "dialog_cartel_exit2_1", "dialog_cartel_exit2_2", "dialog_cartel_exit2_3"), 
					2 => BuildExitChain(storeClient, "dialog_cartel_exit3_1", "dialog_cartel_exit3_2", "dialog_cartel_exit3_3"), 
					3 => BuildExitChain(storeClient, "dialog_cartel_exit4_1", "dialog_cartel_exit4_2", "dialog_cartel_exit4_3"), 
					_ => BuildExitOkayChain(storeClient), 
				};
				PlayerStore.Instance.cartelExitStage++;
				farewellNode.SetNextDialogue(nextDialogue);
			}
		});
		farewellNode.SetEndAction(delegate
		{
			if (farewellNode.nextDialogue == null)
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			}
		});
		return chain;
		void PayOut(bool commitTerms)
		{
			if (commitTerms)
			{
				PlayerStore.Instance.cartelRequiredCount = GetNextWeekCountTarget();
				PlayerStore.Instance.cartelRequiredPurity = GetNextWeekPurityTarget();
			}
			int num = 150;
			if (StoreReputation.IsPerkUnlocked("CARTEL_INCREASED_STIPEND"))
			{
				num += StoreReputation.GetCartelReputation() * 4;
			}
			PlayerStore.Instance.ModCash(num);
			if (reward >= 2)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("chem_module", isOwend: true);
			}
			AddCartelMaterial();
		}
	}

	private static bool IsExitAuthorized()
	{
		return StoreReputation.GetPerkByID("CARTEL_AUTHORIZED_EXIT")?.isUnlocked ?? false;
	}

	private static Dialogue BuildExitChain(StoreClient storeClient, string key1, string key2, string key3)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue(key1)).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue(key2))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue(key3))
			.SetEndAction(delegate
			{
				StoreReputationPerk perkByID = StoreReputation.GetPerkByID("CARTEL_AUTHORIZED_EXIT");
				if (perkByID != null)
				{
					perkByID.perkThreshold += 25;
				}
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return dialogue;
	}

	private static Dialogue BuildExitOkayChain(StoreClient storeClient)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_exit_ok_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_exit_ok_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_exit_ok_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_exit_ok_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_exit_ok_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_exit_ok_6"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.isCartelExited = true;
				StoreReputation.RemoveCartelFaction();
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
				StoreService.GetCartelProtectionService().unlocked = false;
				StoreService.GetInsuranceService().introduced = true;
				StoreService.GetInsuranceService().unlocked = true;
			});
		return dialogue;
	}

	public static void AddCartelMaterial()
	{
		PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap", isOwend: true);
		PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap", isOwend: true);
		PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap", isOwend: true);
		PlayerStore.Instance.AddDirectSellingItemToTable("common_chemical", isOwend: true);
	}

	public static StoreClient CreateCartelWoundedSoldier()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "cartelWoundedSoldier";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_cartel_soldier");
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		StoreClientFactionSetup.InitCartel(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.dismissable = true;
		storeClient.SetBudget(150);
		storeClient.tradeReputationMultiplier = 3.0;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_soldier_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_soldier_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_soldier_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_cartel_soldier_4"));
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateShadyFarmer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "shadyFarmer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_shady_farmer");
		storeClient.spriteName = SpriteDict.GetRandomFarmerSpriteName();
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.dismissable = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shady_farmer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shady_farmer_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateSmallDreamCap());
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateSmallDreamCap());
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap");
				if (RNG.Roll(75))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap");
				}
				if (RNG.Roll(75))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap");
				}
				if (RNG.Roll(75))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("coffee_bean");
				}
				if (RNG.Roll(75))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("coffee_bean");
				}
				if (RNG.Roll(75))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("kotton_fabric");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateNutrifruitNormal());
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateNutrifruitNormal());
				if (RNG.Roll(75))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("nightmare_cap");
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateDealer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "street_dealer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_dealer");
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.IsPersonalConsumption = false;
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.dismissable = true;
		storeClient.SetBudget(200 + StoreStation.Instance.dayCounter * 4);
		storeClient.clientItemFeatureBuying.Add("discountedBuy");
		storeClient.acceptRetailMarkup = false;
		storeClient.CanExposeChem = true;
		storeClient.clientBuyingTagList.Add("NARCOTIC");
		if (StoreClientManager.GetCustomerTotalVisitedCount("street_dealer") == 0)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_dealer_first_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_dealer_first_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_dealer_first_3"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_dealer_first_4"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_dealer_first_5"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_dealer_first_6"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_dealer_first_7"));
		}
		else
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_dealer_return_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_dealer_return_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_dealer_return_3"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_dealer_return_4"));
		}
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

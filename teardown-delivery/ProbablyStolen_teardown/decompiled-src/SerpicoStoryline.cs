using QFSW.QC;
using UnityEngine;

public static class SerpicoStoryline
{
	public const int FOOD_KCAL_NEEDED = 2000;

	public const int BOUNTY_RAISED = 600;

	public const int REPUTATION_REWARD = 12;

	public const int GEORGIO_FOOD_PRICE = 150;

	public const int GEORGIO_FOOD_LL_REP_REQUIRED = 40;

	public const int CONCLUSION_BETRAYAL_NET = 15;

	public const int CHOICE_NONE = 0;

	public const int CHOICE_REVOLUTION = 1;

	public const int CHOICE_BLACK_MARKET = 2;

	public const int CHOICE_SECURITY = 3;

	public const int CHOICE_REPORTED = 4;

	private const float FOOD_CHECK_INTERVAL = 0.5f;

	private static float nextFoodCheck;

	private static bool enoughFoodNotified;

	private static StoreClientData Data => PlayerStore.GetStoreClientData();

	public static bool IsTestMode => Data.serpicoTestStartDay > 0;

	private static int StartDay
	{
		get
		{
			if (!IsTestMode)
			{
				return StorylineSchedule.StartDay("serpico");
			}
			return Data.serpicoTestStartDay;
		}
	}

	public static int Serpico1Day => StartDay;

	public static int Serpico3Day => Serpico1Day + Gap(2);

	public static int Serpico4Day => Serpico3Day + Gap(1);

	public static int Serpico5Day => Serpico4Day + Gap(2);

	public static int Serpico6Day => Serpico5Day + Gap(1);

	public static int LetterDay => Serpico6Day + Gap(2);

	public static int FoodNights => (IsTestMode ? 1 : 3) + Data.serpicoFoodExtraDays;

	public static int OutcomeFoodDay => LetterDay + FoodNights;

	public static int OfficerDay
	{
		get
		{
			if (!IsTestMode)
			{
				return OutcomeFoodDay + 1;
			}
			return OutcomeFoodDay;
		}
	}

	public static int DocumentDay => OfficerDay + Gap(2);

	public static int DocumentNewsDay => DocumentDay + 1;

	public static int ConclusionDay => DocumentDay + Gap(3);

	private static int Gap(int normal)
	{
		if (!IsTestMode)
		{
			return normal;
		}
		return 1;
	}

	public static void HandleDay(StoreClientManager manager, int day)
	{
		if (day == Serpico1Day)
		{
			manager.AddClient(StoreClientUniqueList.Serpico1());
		}
		if (day == Serpico3Day)
		{
			manager.AddClient(StoreClientUniqueList.Serpico3());
		}
		if (day == Serpico4Day)
		{
			manager.AddClient(StoreClientUniqueList.Serpico4());
		}
		if (day == Serpico5Day)
		{
			manager.AddClient(StoreClientUniqueList.Serpico5());
		}
		if (day == Serpico6Day)
		{
			manager.AddClient(StoreClientUniqueList.Serpico6());
		}
		StoreClientData data = Data;
		if (IsReported())
		{
			return;
		}
		if (day == LetterDay)
		{
			manager.AddClient(CreateGeorgioLetter());
		}
		if (!data.serpicoLetterDelivered)
		{
			return;
		}
		if (day == OutcomeFoodDay && !IsFed())
		{
			StoreStation.Instance.storeEventManager.ActivateEvent(CreateDeathNews());
		}
		if (!IsFed())
		{
			return;
		}
		if (day == OfficerDay)
		{
			manager.AddClient(CreateOfficerReturn());
		}
		if (day == DocumentDay)
		{
			manager.AddClient(StoreClientUniqueList.Serpico7());
		}
		if (day == DocumentNewsDay)
		{
			if (data.serpicoDocumentChoice == 1)
			{
				StoreStation.Instance.storeEventManager.ActivateEvent(CreateRevolutionNews());
			}
			if (data.serpicoDocumentChoice == 3)
			{
				StoreStation.Instance.storeEventManager.ActivateEvent(CreateSecurityNews());
			}
		}
		if (day == ConclusionDay && IsConcludingChoice(data.serpicoDocumentChoice))
		{
			manager.AddClient(CreateConclusion(data.serpicoDocumentChoice));
		}
	}

	[Command("serpico-test", "Replays Serpico's storyline from tomorrow, one encounter per day (\"off\" restores the normal days)", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string TestMode(string mode = "on")
	{
		if (PlayerStore.Instance == null || StoreStation.Instance == null)
		{
			return "Load a game first.";
		}
		StoreClientData data = Data;
		if (mode == "off")
		{
			data.serpicoTestStartDay = -1;
			return "Serpico test mode off: normal days from day " + StorylineSchedule.StartDay("serpico") + ".";
		}
		data.serpicoTestStartDay = StoreStation.Instance.dayCounter + 1;
		ResetProgress();
		return "Serpico test mode on, progress reset. Days: Serpico1 " + Serpico1Day + ", Serpico3 " + Serpico3Day + ", Serpico4 " + Serpico4Day + ", officer warning " + Serpico5Day + ", flees " + Serpico6Day + ", Georgio's letter " + LetterDay + " (food that night), dead or saved " + OutcomeFoodDay + ", officer back " + OfficerDay + ", document " + DocumentDay + ", article " + DocumentNewsDay + ", ending " + ConclusionDay + ".";
	}

	public static bool IsFed()
	{
		return Data.serpicoFoodKcal >= 2000;
	}

	public static bool IsReported()
	{
		if (Data.serpicoReported)
		{
			return true;
		}
		if (PlayerStore.Instance.wantedStatus != null && PlayerStore.Instance.wantedStatus.TryGetValue("serpico6", out var value))
		{
			return value == 1;
		}
		return false;
	}

	private static bool IsConcludingChoice(int choice)
	{
		if (choice != 1 && choice != 2)
		{
			return choice == 3;
		}
		return true;
	}

	public static bool IsFoodWindowOpen()
	{
		StoreClientData data = Data;
		if (IsReported() || !data.serpicoLetterDelivered || IsFed())
		{
			return false;
		}
		int dayCounter = StoreStation.Instance.dayCounter;
		if (dayCounter >= LetterDay)
		{
			return dayCounter < LetterDay + FoodNights;
		}
		return false;
	}

	private static int GetFoodKcalOn(GameInventory ground)
	{
		int num = 0;
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(ground))
		{
			if (item.IsGameItemType("FOOD") && item.IsTag("CALORIE_VALUE_TAG"))
			{
				num += item.GetTagReadonly("CALORIE_VALUE_TAG").GetInt() * Mathf.Max(1, item.unitCount);
			}
		}
		return num;
	}

	public static void OnArriveDumpingGround()
	{
		nextFoodCheck = 0f;
		enoughFoodNotified = false;
	}

	public static void TickDumpingGround(GameInventory ground)
	{
		if (!enoughFoodNotified && ground != null && !(Time.time < nextFoodCheck) && IsFoodWindowOpen())
		{
			nextFoodCheck = Time.time + 0.5f;
			if (Data.serpicoFoodKcal + GetFoodKcalOn(ground) >= 2000)
			{
				enoughFoodNotified = true;
				StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("mech_notify_serpico_food_enough"));
			}
		}
	}

	public static void OnLeaveDumpingGround(GameInventory ground)
	{
		if (ground == null || !IsFoodWindowOpen())
		{
			return;
		}
		int foodKcalOn = GetFoodKcalOn(ground);
		if (foodKcalOn <= 0)
		{
			return;
		}
		StoreClientData data = Data;
		data.serpicoFoodKcal += foodKcalOn;
		string localizedMechanic = LocHelper.GetLocalizedMechanic("mech_nightlog_serpico_food", foodKcalOn, Mathf.Min(data.serpicoFoodKcal, 2000), 2000);
		if (IsFed())
		{
			PlayerStore.Instance.AddNightLog(localizedMechanic, "5c6f46");
			return;
		}
		PlayerStore.Instance.AddNightLog(localizedMechanic);
		int dayCounter = StoreStation.Instance.dayCounter;
		if (data.serpicoFoodExtraDayLastDay != dayCounter)
		{
			data.serpicoFoodExtraDayLastDay = dayCounter;
			data.serpicoFoodExtraDays++;
		}
		int num = OutcomeFoodDay - (StoreStation.Instance.dayCounter + 1);
		PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanicOrDefault("mech_nightlog_serpico_food_extra_day", "The food buys Serpico one more day. Nights left to bring the rest: {0}", num));
	}

	private static StoreClient CreateGeorgioLetter()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "georgioSerpicoLetter";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_georgio");
		storeClient.spriteName = "george";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		string name = storeClient.displayName;
		int price = 150;
		bool cannotGo = CannotReachDumpingGround();
		Dialogue dialogue = storeClient.mainDialogue.SetStartAction(delegate
		{
			if (!Data.serpicoLetterDelivered)
			{
				Data.serpicoLetterDelivered = true;
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(CreateLetter(), isOwend: true);
			}
		}).SetText(name, LocHelper.GetLocalizedDialogue("dialog_georgio_serpico_letter_1")).NextDialogue()
			.SetText(name, LocHelper.GetLocalizedDialogue("dialog_georgio_serpico_letter_2"))
			.NextDialogue()
			.SetText(name, LocHelper.GetLocalizedDialogue("dialog_georgio_serpico_letter_3"));
		Dialogue dialogue2;
		if (cannotGo)
		{
			dialogue2 = new Dialogue().SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_georgio_serpico_food_unable_3", "For {0} Credits, Georgio drops them off for you. Otherwise, Georgio fears the young man will not make it.", price));
			dialogue.NextDialogue().SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_georgio_serpico_food_unable_1", "Hmm... but Georgio hears you cannot go to the Dumping Ground these days.")).NextDialogue()
				.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_georgio_serpico_food_unable_2", "No worries! Georgio happens to have a crate of Nutrifruits."))
				.SetNextDialogue(dialogue2);
		}
		else
		{
			dialogue2 = new Dialogue().SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_georgio_serpico_food_offer_2", "For {0} Credits, Georgio drops them off for you. You do not have to bring a single bite!", price));
			dialogue.NextDialogue().SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_georgio_serpico_food_offer_1", "The young man is very hungry, yes? Georgio happens to have a crate of Nutrifruits.")).SetNextDialogue(dialogue2);
		}
		Dialogue lineAccept = new Dialogue().SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_georgio_assist_choice_accept", price));
		Dialogue lineDecline = new Dialogue().SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_georgio_loan_choice_decline"));
		dialogue2.AddChoice(DirectoryMaster.Item("$"), "no choice", "", lineAccept, delegate
		{
			lineAccept.NextDialogue().SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_georgio_serpico_food_accept_1", "Excellent! The young man will eat like a king tonight.")).SetEndAction(delegate
			{
				PayGeorgioForFood(price);
			})
				.NextDialogue()
				.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_georgio_serpico_food_accept_2", "A friend of yours is a friend of Georgio!"))
				.SetEndAction(delegate
				{
					DialogUIManager.Instance.CurrentClientLeave();
				});
		}, () => PlayerStore.Instance.GetCash() >= price);
		Dialogue lineFavor = new Dialogue().SetText("PLAYER", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_georgio_serpico_food_favor_choice", "[Call in a favor ({0} Lower Levels rep+)]", 40));
		dialogue2.AddChoice(DirectoryMaster.Item("$"), "no choice", "", lineFavor, delegate
		{
			lineFavor.NextDialogue().SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_georgio_serpico_food_favor_1", "For a friend of the Lower Levels? Say no more.")).SetEndAction(delegate
			{
				DeliverGeorgioFood();
				DialogUIManager.Instance.CurrentClientLeave();
			});
		}, () => StoreReputation.GetLLReputation() >= 40);
		dialogue2.AddChoice(DirectoryMaster.Item("$"), "no choice", "", lineDecline, delegate
		{
			lineDecline.NextDialogue().SetText(name, cannotGo ? LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_georgio_serpico_food_unable_decline", "Georgio understands. Let us hope the young man finds food on his own.") : LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_georgio_serpico_food_decline", "No problem, my friend. Georgio trusts you to feed him yourself.")).SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static bool CannotReachDumpingGround()
	{
		if (!StartingPerk.IsMaimed())
		{
			return HealthData.IsSeriouslyWounded();
		}
		return true;
	}

	private static void PayGeorgioForFood(int price)
	{
		if (!IsFed() && PlayerStore.Instance.GetCash() >= price)
		{
			PlayerStore.Instance.ModCash(-price);
			DeliverGeorgioFood();
		}
	}

	private static void DeliverGeorgioFood()
	{
		if (!IsFed())
		{
			Data.serpicoFoodKcal = 2000;
			PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanicOrDefault("mech_nightlog_serpico_food_georgio", "Georgio left a crate of Nutrifruits for Serpico at the Dumping Ground."), "5c6f46");
		}
	}

	private static GameItem CreateLetter()
	{
		return PreBuiltItemHelper.CreateHandnote(LocHelper.GetLocalizedMechanic("note_serpico_letter_body_1") + "\n" + LocHelper.GetLocalizedMechanic("note_serpico_letter_body_2") + "\n" + LocHelper.GetLocalizedMechanic("note_serpico_letter_body_3", 2000) + "\n" + LocHelper.GetLocalizedMechanic("note_serpico_letter_body_4", FoodNights), "", LocHelper.GetLocalizedClientName("name_serpico"), 2);
	}

	private static StoreClient CreateOfficerReturn()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "serpicoOfficerReturn";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_security_officer");
		storeClient.spriteName = SpriteDict.GetRandomSecSpriteName();
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.arrestable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_officer_return_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_officer_return_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_officer_return_3", 600))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_officer_return_4"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static StoreClient CreateConclusion(int choice)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "serpicoConclusion";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_serpico");
		bool flag = choice != 3;
		storeClient.nonShootable = !flag;
		storeClient.arrestable = flag;
		Dialogue dialogue;
		switch (choice)
		{
		case 1:
			storeClient.spriteName = "serpico_rev";
			StoreClientFactionSetup.InitRev(storeClient);
			dialogue = storeClient.mainDialogue.SetStartAction(delegate
			{
				StoreReputation.ModRevReputation(12);
			}).SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_rev_1")).NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_rev_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_rev_3"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_rev_4"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_rev_5"));
			break;
		case 3:
			storeClient.spriteName = "serpico";
			StoreClientFactionSetup.InitSecurity(storeClient);
			dialogue = storeClient.mainDialogue.SetStartAction(delegate
			{
				StoreReputation.ModSecReputation(12);
			}).SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_sec_1")).NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_sec_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_sec_3"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_sec_4"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_sec_5"));
			break;
		default:
			storeClient.spriteName = "serpico_scav";
			StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
			dialogue = storeClient.mainDialogue.SetStartAction(delegate
			{
				StoreReputation.ModBMReputation(12);
			}).SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_bm_1")).NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_bm_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_bm_3"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_bm_4"));
			break;
		}
		BarterOffer barterOffer = BarterOfferList.CreateFreeRewardChoice(LocHelper.GetLocalizedUI("ui_serpico_reward_body"), delegate
		{
			AddReward("storage_bay_large", 1);
		}, delegate
		{
			AddReward("module_bay_expansion_kit", 4);
		}, delegate
		{
			AddReward("current_converter", 2);
		});
		storeClient.SetBarterOffer(barterOffer);
		dialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_bm_5")).SetEndAction(delegate
		{
			storeClient.barterOffer.InstanciateOffer();
		});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_bm_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_bm_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.repeatDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_serpico_end_bm_5"));
		if (flag)
		{
			SetupConclusionWanted(storeClient, choice);
		}
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static void SetupShootable(StoreClient storeClient)
	{
		storeClient.nonShootable = false;
		storeClient.holsterResumesDialogue = true;
		storeClient.unholsterDialogue = () => new Dialogue().SetText("CLIENT", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_serpico_gun_drawn", "Woah, what's going on? Put that thing away!"));
		storeClient.shotCountsAsCapture = true;
		storeClient.shotCaptureDialogue = CreateShotDialogue;
	}

	private static Dialogue CreateShotDialogue()
	{
		string title = "SEC";
		Dialogue dialogue = new Dialogue();
		dialogue.SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "sec_shoot_serpico_1", "<i>*turns the body over*</i> Hold on. Cadet Serpico.")).NextDialogue().SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "sec_shoot_serpico_2", "<i>*chuckles*</i> The kid who couldn't keep his mouth shut. The boys are going to love this."))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "sec_shoot_serpico_3", "Now, strictly speaking, that gun of yours is for Augs only."))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "sec_shoot_serpico_4", "But between us? You just saved the department a lot of paperwork."))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "sec_shoot_serpico_5", "We'll write it up as a capture. The bounty's yours."));
		return dialogue;
	}

	private static void SetupConclusionWanted(StoreClient storeClient, int choice)
	{
		storeClient.SetWanted(600);
		storeClient.reportDegradesFaction = false;
		storeClient.onCaptureAction = delegate
		{
			Data.serpicoDocumentChoice = 4;
			int amount = -27;
			if (choice == 1)
			{
				StoreReputation.ModRevReputation(amount);
			}
			else
			{
				StoreReputation.ModBMReputation(amount);
			}
		};
		storeClient.customOnArrestDialogue = CreateConclusionArrestDialogue(storeClient, choice);
		SetupShootable(storeClient);
	}

	private static Dialogue CreateConclusionArrestDialogue(StoreClient storeClient, int choice)
	{
		string displayName = storeClient.displayName;
		string title = "SEC";
		Dialogue dialogue = new Dialogue();
		Dialogue dialogue2;
		string localizedOrDefault;
		if (choice == 1)
		{
			dialogue2 = dialogue.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_serpico_end_rev_arrest_client_1", "Wait. You're reporting me? Now?")).NextDialogue().SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_serpico_end_rev_arrest_sec_1", "Well, look who it is. The cadet turned Revolutionary."))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_serpico_end_rev_arrest_client_2", "It's already out there. Every name. Arresting me changes nothing."))
				.NextDialogue()
				.SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_serpico_end_rev_arrest_sec_2", "Who said anything about arresting?"));
			localizedOrDefault = LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_serpico_end_rev_arrest_sec_3", "Let's get him bagged.");
		}
		else
		{
			dialogue2 = dialogue.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_serpico_end_bm_arrest_client_1", "No... We were leaving tonight. My family is waiting at the docks.")).NextDialogue().SetText(title, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_serpico_end_bm_arrest_sec_1", "Then they'll wait a long time."));
			localizedOrDefault = LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_serpico_end_bm_arrest_sec_2", "Good call, Citizen. One less loose end.");
		}
		dialogue2.NextDialogue().SetText(title, LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_sec_1")).SetStartAction(delegate
		{
			AudioManager.Instance.Play2("pistol_burst");
		})
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.clientGameObject.gameObject.SetActive(value: false);
			})
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_sec_2"))
			.NextDialogue()
			.SetText(title, LocHelper.GetLocalizedDialogue("dialog_serpico6_arrest_sec_3"))
			.NextDialogue()
			.SetText(title, localizedOrDefault)
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		return dialogue;
	}

	private static void AddReward(string id, int count)
	{
		for (int i = 0; i < count; i++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTableMiddle(ItemFeatureHelper.SetShabby(DirectoryMaster.Item(id, isOwned: false)));
		}
	}

	private static StoreEvent CreateNews(string id, string newsName, string newsDescription)
	{
		StoreEvent obj = new StoreEvent
		{
			identifier = id,
			newsName = newsName
		};
		obj.displayName = obj.newsName;
		obj.newsDescription = newsDescription;
		obj.duration = 1;
		obj.importance = 4;
		obj.eventArea = StoreEvent.EventArea.ALL;
		obj.eventType = StoreEvent.EventType.COSMETIC;
		return obj;
	}

	private static StoreEvent CreateDeathNews()
	{
		return CreateNews("serpicoFoundDead", LocHelper.GetLocalizedMechanic("event_serpico_dead_news"), LocHelper.GetLocalizedMechanic("event_serpico_dead_desc"));
	}

	private static StoreEvent CreateRevolutionNews()
	{
		return CreateNews("serpicoEvidencePublished", LocHelper.GetLocalizedMechanic("event_serpico_rev_news"), LocHelper.GetLocalizedMechanic("event_serpico_rev_desc"));
	}

	private static StoreEvent CreateSecurityNews()
	{
		return CreateNews("serpicoEvidenceInvestigation", LocHelper.GetLocalizedMechanic("event_serpico_sec_news"), LocHelper.GetLocalizedMechanic("event_serpico_sec_desc"));
	}

	public static StoreClient DebugGeorgioLetter()
	{
		return CreateGeorgioLetter();
	}

	public static StoreClient DebugOfficerReturn()
	{
		return CreateOfficerReturn();
	}

	public static StoreClient DebugConclusion(int choice)
	{
		return CreateConclusion(choice);
	}

	public static void DebugDeathNews()
	{
		StoreStation.Instance.storeEventManager.ActivateEvent(CreateDeathNews());
	}

	public static void DebugRevolutionNews()
	{
		StoreStation.Instance.storeEventManager.ActivateEvent(CreateRevolutionNews());
	}

	public static void DebugSecurityNews()
	{
		StoreStation.Instance.storeEventManager.ActivateEvent(CreateSecurityNews());
	}

	public static void DebugFeed()
	{
		Data.serpicoFoodKcal = 2000;
	}

	public static void ResetProgress()
	{
		StoreClientData data = Data;
		data.serpicoReported = false;
		data.serpicoLetterDelivered = false;
		data.serpicoFoodKcal = 0;
		data.serpicoFoodExtraDays = 0;
		data.serpicoFoodExtraDayLastDay = -1;
		data.serpicoDocumentChoice = 0;
		if (PlayerStore.Instance.wantedStatus != null)
		{
			PlayerStore.Instance.wantedStatus.Remove("serpico6");
			PlayerStore.Instance.wantedStatus.Remove("serpico7");
			PlayerStore.Instance.wantedStatus.Remove("serpicoConclusion");
		}
	}
}

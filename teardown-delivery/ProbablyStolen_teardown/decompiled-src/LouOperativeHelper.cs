using System;
using System.Collections.Generic;
using UnityEngine;

public static class LouOperativeHelper
{
	public const string TRANSMITTER_ID = "republic_transmitter";

	public const string ETCHED_ROUND_ID = "lou_etched_round";

	public const string LOCKER_TAG = "LOU_LOCKER_TAG";

	public const string LOCKER_ID = "lou_locker";

	public const int LOCKER_WIDTH = 10;

	public const int LOCKER_HEIGHT = 7;

	public const string WANTED5_POSTER_ID = "wanted5";

	public const int OPERATIVE_ADVANCE = 750;

	public const int OPERATIVE_REWARD = 750;

	public const int TRAP_VALUE = 500;

	public const int LOCKER_DELAY = 3;

	public const int LOCKER_NIGHTS = 3;

	public const int QUESTIONING_DELAY = 2;

	public const int THANK_YOU_DELAY = 2;

	public const int THANK_YOU_GRACE = 3;

	public const int SEC_SUSPICION_PENALTY = 12;

	private static readonly HashSet<string> LOU_VISIT_IDS = new HashSet<string> { "wanted5", "wanted5morning", "louBombVisit", "louBoxDrop", "louBoxCollect", "louDisguise", "louDemandCollect" };

	private static bool isRevealScheduled = false;

	public const string OPERATIVE_SPRITE = "maleMerc3";

	public static int GetLockerValue()
	{
		return StoreClientData.GetLouDemandValue() / 2;
	}

	public static bool IsLouGamesStopped()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.louShotDay < 0 && storeClientData.louCapturedDay < 0)
		{
			return storeClientData.louRevealDay >= 0;
		}
		return true;
	}

	public static bool IsOperativeMet()
	{
		return PlayerStore.Instance.storeClientData.louOperativeMetDay >= 0;
	}

	private static bool IsLockerResolved(StoreClientData data)
	{
		if (data.louCapturedDay < 0 && data.louLockerPaidDay < 0)
		{
			return data.isLouLockerShort;
		}
		return true;
	}

	private static string LouName()
	{
		return LocHelper.GetLocalizedClientName("name_lou");
	}

	private static string PlayerName()
	{
		string playerName = PlayerStore.Instance.playerName;
		if (!string.IsNullOrEmpty(playerName))
		{
			return playerName;
		}
		return LocHelper.GetLocalizedUIOrDefault("ui_lou_player_name_fallback", "Shopkeeper");
	}

	public static GameItem CreateTransmitter()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_misc", "republic_transmitter");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", "item_republic_transmitter_name", "Republic Transmitter");
		gameItem.shortDescription = LocHelper.GetLocalizedOrDefault("Item", "item_republic_transmitter_desc", "A tiny transmitter given to you by the Republic Operative.");
		gameItem.flavorText = LocHelper.GetLocalizedOrDefault("Item", "item_republic_transmitter_flavor", "Hide it in something Lou takes away with her.");
		gameItem.unitValue = 0L;
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("NOT_FOR_RESALE");
		gameItem.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem self, GameInventory inventory, SlotMarker _)
		{
			OnTransmitterMoved(inventory);
		});
		return gameItem;
	}

	public static GameItem InitEtchedRound(GameItem round)
	{
		round.shortDescription = "";
		round.flavorText = LocHelper.GetLocalizedOrDefault("Item", "item_lou_etched_round_flavor", "A single round, left on your counter overnight. \"{0}\" is etched into the casing.", PlayerName());
		return round;
	}

	public static GameItem CreateLocker(GameItem box)
	{
		LockHelper.UnlockContainer(box, force: true);
		box.name = LocHelper.GetLocalizedOrDefault("Item", "item_lou_locker_name", "Lou's Locker");
		box.shortDescription = "";
		box.flavorText = LocHelper.GetLocalizedOrDefault("Item", "item_lou_locker_flavor", "Empty, unlocked, waiting. Fill it and leave it on the front counter when you close.");
		box.EnableTag("LOU_LOCKER_TAG");
		box.EnableTag("IMPORTANT_TAG");
		return box;
	}

	private static GameItem CreateLockerLetter()
	{
		return PreBuiltItemHelper.CreateHandnote(LocHelper.GetLocalizedMechanicOrDefault("note_lou_locker_body", "Nutrifruit. That stung. I'm not mad, I'm IMPRESSED. But a girl has expenses. Fill this locker with {0} Credits' worth of goods and leave it on your front counter when you close. You have {1} nights. The night after that, the whole store goes boom. Kisses.", GetLockerValue(), 3), "", LouName());
	}

	private static GameItem CreateThankYouNote()
	{
		return PreBuiltItemHelper.CreateHandnote(LocHelper.GetLocalizedMechanicOrDefault("note_lou_thank_you_body", "Nutrifruit! You came through. I knew you had it in you. A little something for the shop. Kisses."), "", LouName());
	}

	public static void SetupLouShootable(StoreClient storeClient)
	{
		storeClient.nonShootable = false;
		storeClient.forceShootable = true;
		storeClient.holsterResumesDialogue = true;
		storeClient.unholsterDialogue = () => new Dialogue().SetText(LouName(), LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_gun_drawn", "Ooh. Careful with that, Nutrifruit. You only get one shot at me."));
		storeClient.survivesShot = true;
		storeClient.onShotSurvived = OnLouShot;
		storeClient.shotSurvivedDialogue = delegate
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetPriority(7);
			dialogue.SetText(LouName(), LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_shot_1", "AHH!")).SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
			return dialogue;
		};
		Action firstLineStart = storeClient.mainDialogue.startAction;
		storeClient.mainDialogue.SetStartAction(delegate
		{
			if (firstLineStart != null)
			{
				firstLineStart();
			}
			if (IsTransmitterOnCounter())
			{
				ScheduleReveal();
			}
		});
	}

	public static void OnLouShot()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.louShotDay < 0)
		{
			storeClientData.louShotDay = StoreStation.Instance.dayCounter;
			storeClientData.isLouBombVisitPending = false;
		}
	}

	private static bool IsCounter(GameInventory inventory)
	{
		if (inventory == null)
		{
			return false;
		}
		EmporiumEntry instance = EmporiumEntry.Instance;
		if (inventory != instance.frontInvinvElement && inventory != instance.backInvinvElement)
		{
			return inventory == instance.backInvinvElementCounter;
		}
		return true;
	}

	private static bool IsTransmitterOnCounter()
	{
		EmporiumEntry instance = EmporiumEntry.Instance;
		GameGridInventory[] array = new GameGridInventory[3] { instance.frontInvinvElement, instance.backInvinvElement, instance.backInvinvElementCounter };
		foreach (GameGridInventory gameGridInventory in array)
		{
			if (gameGridInventory == null)
			{
				continue;
			}
			foreach (GraphNodeStorage child in gameGridInventory.children)
			{
				if (child is GameItem { identifier: "republic_transmitter" })
				{
					return true;
				}
			}
		}
		return false;
	}

	private static StoreClient CurrentLouVisit()
	{
		if (PlayerStore.Instance.currentClientInstance == null)
		{
			return null;
		}
		StoreClient clientBlueprint = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
		if (clientBlueprint == null || !LOU_VISIT_IDS.Contains(clientBlueprint.identifier))
		{
			return null;
		}
		return clientBlueprint;
	}

	private static void OnTransmitterMoved(GameInventory inventory)
	{
		if (IsCounter(inventory) && CurrentLouVisit() != null)
		{
			ScheduleReveal();
		}
	}

	private static void ScheduleReveal()
	{
		StoreClientData data = PlayerStore.Instance.storeClientData;
		if (data.louRevealDay >= 0 || isRevealScheduled)
		{
			return;
		}
		isRevealScheduled = true;
		StoreUIManager.Instance.StartCoroutine(MetaHelper.WaitAndDo(0.3f, delegate
		{
			isRevealScheduled = false;
			StoreClient storeClient = CurrentLouVisit();
			if (storeClient != null && data.louRevealDay < 0 && IsTransmitterOnCounter())
			{
				DialogUIManager.Instance.HideChoiceBubble();
				Dialogue dialogue = CreateRevealDialogue(storeClient);
				if (DialogUIManager.Instance.StartDialogue(dialogue, forcedStart: true))
				{
					data.louRevealDay = StoreStation.Instance.dayCounter;
					RemoveTransmitters();
				}
			}
		}));
	}

	private static void RemoveTransmitters()
	{
		foreach (GameItem item in EmporiumEntry.Instance.GetAllItems().FindAll((GameItem i) => i != null && i.identifier == "republic_transmitter"))
		{
			item.Destroy();
		}
	}

	private static Dialogue CreateRevealDialogue(StoreClient storeClient)
	{
		string name = LouName();
		storeClient.displayName = name;
		storeClient.bubbleColor = "DEFAULT";
		storeClient.tryInterceptDismiss = null;
		storeClient.resumeAfterConfrontDialogue = null;
		storeClient.onItemBought = null;
		storeClient.onDealAccepted = null;
		storeClient.barterOffer = null;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.clientBuyingTagList.Clear();
		storeClient.clientBuyingIdList.Clear();
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("FIREARM");
		storeClient.clientBuyingTagList.Add("MELEE_WEAPON");
		storeClient.clientBuyingTagList.Add("AMMUNITION");
		storeClient.clientBuyingTagList.Add("MEDICAL");
		if (!storeClient.clientItemFeatureBuying.Contains("bribeDonation"))
		{
			storeClient.clientItemFeatureBuying.Add("bribeDonation");
		}
		storeClient.isNoContributeToEvidence = true;
		storeClient.tradeReputationMultiplier = 0.0;
		storeClient.isBargainAvailable = false;
		storeClient.acceptRetailMarkup = false;
		Action endAction = delegate
		{
			if (PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint() == storeClient)
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			}
		};
		int given = 0;
		bool isSettled = false;
		bool isPaid = false;
		Action<bool> settle = delegate(bool isWon)
		{
			if (!isSettled)
			{
				isSettled = true;
				storeClient.tryInterceptDismiss = null;
				storeClient.dismissable = false;
				storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
				DialogUIManager.Instance.HideChoiceBubble();
				StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
				storeClientData.louTrapDay = StoreStation.Instance.dayCounter;
				storeClientData.isLouTrapWon = isWon;
				if (isWon)
				{
					StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListUnique.CreateLouWarehouseShootout(), 1, IsKnown: false);
				}
			}
		};
		Dialogue wonTail = new Dialogue();
		wonTail.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_enough_2", "Thank you, Nutrifruit. You will hear from me.")).SetEndAction(endAction);
		Dialogue dialogue = new Dialogue();
		dialogue.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_enough_1", "That will do. That will do nicely.")).SetNextDialogue(wonTail);
		Dialogue declined = new Dialogue();
		declined.SetPriority(7);
		declined.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_short_2", "It will have to do. Wish me luck, Nutrifruit.")).SetStartAction(delegate
		{
			settle(obj: false);
		}).SetEndAction(endAction);
		int remaining = 0;
		bool isAsking = false;
		Action choiceAction = delegate
		{
			if (!isSettled)
			{
				if (PlayerStore.Instance.GetCash() >= remaining)
				{
					PlayerStore.Instance.ModCash(-remaining);
				}
				settle(obj: true);
			}
		};
		Dialogue payLine = new Dialogue();
		payLine.SetText("PLAYER", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_choice_pay", "[Give her {0} Credits]", remaining)).SetNextDialogue(dialogue);
		Dialogue dialogue2 = new Dialogue();
		dialogue2.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_georgio_loan_choice_decline")).SetNextDialogue(declined);
		Dialogue ask = new Dialogue();
		ask.SetPriority(7);
		ask.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_credits", "I know a weapons dealer around here. {0} Credits would cover the rest.", remaining)).AddChoice(DirectoryMaster.Item("$"), "no choice", "", payLine, choiceAction, () => PlayerStore.Instance.GetCash() >= remaining).AddChoice(DirectoryMaster.Item("$"), "no choice", "", dialogue2, delegate
		{
			settle(obj: false);
		});
		Dialogue outcome = new Dialogue();
		outcome.SetPriority(7);
		outcome.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_short_1", "It might not be enough. Not to sound ungrateful.")).SetStartAction(delegate
		{
			if (given >= 500)
			{
				outcome.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_enough_1", "That will do. That will do nicely."));
				outcome.SetNextDialogue(wonTail);
				settle(obj: true);
			}
			else
			{
				remaining = 500 - given;
				outcome.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_short_1", "It might not be enough. Not to sound ungrateful."));
				ask.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_credits", "I know a weapons dealer around here. {0} Credits would cover the rest.", remaining));
				payLine.SetText("PLAYER", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_choice_pay", "[Give her {0} Credits]", remaining));
				outcome.SetNextDialogue(ask);
				isAsking = true;
				storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
				storeClient.dismissable = false;
				DialogUIManager.Instance.HideChoiceBubble();
			}
		});
		Dialogue dialogue3 = new Dialogue();
		dialogue3.SetText("PLAYER", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_choice_done", "[That's All I Have]")).SetNextDialogue(outcome);
		Dialogue dialogue4 = new Dialogue();
		dialogue4.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_wait", "Put it on the counter. Whatever you can spare.")).SetStartAction(delegate
		{
			if (!(isSettled || isPaid))
			{
				storeClient.clientIntent = StoreClient.ClientIntent.BUY;
				storeClient.dismissable = true;
			}
		}).AddChoice(DirectoryMaster.Item("$"), "", "", dialogue3, delegate
		{
		});
		Dialogue more = new Dialogue();
		more.SetPriority(6);
		more.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_trap_more", "Good. More, if you have it.")).SetNextDialogue(dialogue4);
		Func<bool> intercept = null;
		intercept = delegate
		{
			if (!isSettled)
			{
				DialogUIManager.Instance.HideChoiceBubble();
				bool num = DialogUIManager.Instance.StartDialogue(isAsking ? declined : outcome);
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
				given += Mathf.Max(0, (int)item.GetNegociatedValue());
				if (given >= 500)
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
				DialogUIManager.Instance.HideChoiceBubble();
				DialogUIManager.Instance.StartDialogue(isPaid ? outcome : more, forcedStart: true);
			}
		};
		Dialogue dialogue5 = new Dialogue();
		dialogue5.SetPriority(7);
		dialogue5.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_reveal_1", "...Where did you get that?")).NextDialogue().SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_reveal_2", "I know that little toy. Republic issue."))
			.NextDialogue()
			.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_reveal_3", "You could have hidden it in my things. You showed it to me instead."))
			.NextDialogue()
			.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_reveal_4", "They will come for me now. Fine. Let them come."))
			.NextDialogue()
			.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_reveal_5", "I am sorry about before. Now I need to ask for your help."))
			.NextDialogue()
			.SetText(name, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_reveal_6", "I need weapons, ammo, meds. Everything you can spare. Please."))
			.SetNextDialogue(dialogue4);
		return dialogue5;
	}

	public static bool TryCaptureThroughContainer(GameItem container)
	{
		if (container == null)
		{
			return false;
		}
		List<GameItem> list = GraphUtils.FindChildrenType(container, (GameItem item) => item.identifier == "republic_transmitter", (GameItem _) => true);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (GameItem item in list)
		{
			item.Destroy();
		}
		OnLouCaptured();
		return true;
	}

	private static void OnLouCaptured()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.louCapturedDay < 0)
		{
			storeClientData.louCapturedDay = StoreStation.Instance.dayCounter;
			storeClientData.isLouBombVisitPending = false;
			PlayerStore.Instance.CloseWantedPoster("wanted5");
		}
	}

	private static void GiveLocker()
	{
		GameGridInventory frontInvinvElement = EmporiumEntry.Instance.frontInvinvElement;
		GameItem gameItem = DirectoryMaster.Item("lou_locker");
		GeneralHelper.SetItemOwned(gameItem);
		if (GraphUtils.TryAcceptAllMid(frontInvinvElement, gameItem) > 0)
		{
			GraphUtils.TryAcceptAll(frontInvinvElement, gameItem);
		}
		GameItem gameItem2 = CreateLockerLetter();
		GeneralHelper.SetItemOwned(gameItem2);
		if (gameItem.parentInventory != null)
		{
			GraphUtils.TryAcceptAll(frontInvinvElement, gameItem2);
		}
		if (gameItem.parentInventory == null || gameItem2.parentInventory == null)
		{
			gameItem.Destroy();
			gameItem2.Destroy();
		}
		else
		{
			PlayerStore.Instance.storeClientData.louLockerDay = StoreStation.Instance.dayCounter;
		}
	}

	private static GameItem FindLockerOnFrontCounter()
	{
		foreach (GraphNodeStorage child in EmporiumEntry.Instance.frontInvinvElement.children)
		{
			if (child is GameItem gameItem && gameItem.IsTag("LOU_LOCKER_TAG"))
			{
				return gameItem;
			}
		}
		return null;
	}

	private static int GetContentValue(GameItem container)
	{
		long num = 0L;
		foreach (GameItem item in GraphUtils.FindChildrenType(container, (GameItem item) => item.identifier != "republic_transmitter", (GameItem _) => true))
		{
			num += Math.Max(0L, item.GetNegociatedValue());
		}
		return (int)Math.Min(2147483647L, num);
	}

	public static void CreateLockerTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item != null && item.IsTag("LOU_LOCKER_TAG"))
		{
			StoreClientData storeClientData = PlayerStore.Instance?.storeClientData;
			if (storeClientData != null && storeClientData.louLockerDay >= 0 && !IsLockerResolved(storeClientData))
			{
				int contentValue = GetContentValue(item);
				int lockerValue = GetLockerValue();
				int b = storeClientData.louLockerDay + StoreClientData.LouDelay(3) - StoreStation.Instance.dayCounter;
				builder.AddLine(" ");
				builder.AddLine(LocHelper.GetLocalizedMechanicOrDefault("mech_lou_locker_value", "{0} / {1} Credits' worth Lou asked for.", contentValue, lockerValue), ignoreEmpty: true, (contentValue >= lockerValue) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.Red);
				builder.AddLine(LocHelper.GetLocalizedMechanicOrDefault("mech_lou_locker_nights", "Nights left: {0}.", Mathf.Max(0, b)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
	}

	public static void OnNight()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.louLockerDay < 0 || IsLockerResolved(storeClientData))
		{
			return;
		}
		GameItem gameItem = FindLockerOnFrontCounter();
		if (gameItem == null)
		{
			return;
		}
		bool flag = TryCaptureThroughContainer(gameItem);
		int contentValue = GetContentValue(gameItem);
		gameItem.Destroy();
		if (!flag)
		{
			if (contentValue >= GetLockerValue())
			{
				storeClientData.louLockerPaidDay = StoreStation.Instance.dayCounter;
				StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListUnique.CreateLouAdmiralAssassinated(), 1, IsKnown: false);
			}
			else
			{
				storeClientData.isLouLockerShort = true;
			}
		}
	}

	public static void OnMorning()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		int dayCounter = StoreStation.Instance.dayCounter;
		if (storeClientData.louShotDay >= 0 && !storeClientData.isLouAmmoLeft && dayCounter >= storeClientData.louShotDay + 1)
		{
			storeClientData.isLouAmmoLeft = true;
			PlayerStore.Instance.AddDirectSellingItemToTable("lou_etched_round", isOwend: true);
		}
		if (storeClientData.louShotDay >= 0 && storeClientData.louLockerDay < 0 && storeClientData.louCapturedDay < 0 && storeClientData.louTrapDay < 0 && dayCounter >= storeClientData.louShotDay + StoreClientData.LouDelay(3))
		{
			GiveLocker();
		}
		if (storeClientData.louLockerDay >= 0 && storeClientData.louCapturedDay < 0 && storeClientData.louLockerPaidDay < 0 && (storeClientData.isLouLockerShort || dayCounter >= storeClientData.louLockerDay + StoreClientData.LouDelay(3)))
		{
			storeClientData.isLouLockerShort = true;
			foreach (GameItem item in EmporiumEntry.Instance.GetAllItems().FindAll((GameItem i) => i?.IsTag("LOU_LOCKER_TAG") ?? false))
			{
				item.Destroy();
			}
			LouToyBoxHelper.PlantRealBomb("lou_bombed_debt");
		}
		else
		{
			if (storeClientData.isLouThankYouGiven)
			{
				return;
			}
			int thankYouDay = GetThankYouDay(storeClientData);
			if (thankYouDay >= 0 && dayCounter >= thankYouDay)
			{
				storeClientData.isLouThankYouGiven = true;
				if (dayCounter <= thankYouDay + 3)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(CreateThankYouNote(), isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemFeatureHelper.SetShabby(DirectoryMaster.Item("storage_bay_expansion_kit")), isOwend: true);
				}
			}
		}
	}

	private static int GetThankYouDay(StoreClientData data)
	{
		if (data.louTrapDay >= 0)
		{
			if (!data.isLouTrapWon)
			{
				return -1;
			}
			return data.louTrapDay + 1;
		}
		if (data.louLockerPaidDay >= 0)
		{
			return data.louLockerPaidDay + StoreClientData.LouDelay(2);
		}
		if (data.louDemandDay >= 0 && data.louCapturedDay < 0)
		{
			return data.louDemandDay + StoreClientData.LouDelay(2);
		}
		return -1;
	}

	public static void DebugGiveLocker()
	{
		GiveLocker();
	}

	public static void DebugGiveThankYou()
	{
		PlayerStore.Instance.storeClientData.isLouThankYouGiven = true;
		PlayerStore.Instance.AddDirectSellingItemToTable(CreateThankYouNote(), isOwend: true);
		PlayerStore.Instance.AddDirectSellingItemToTable(ItemFeatureHelper.SetShabby(DirectoryMaster.Item("storage_bay_expansion_kit")), isOwend: true);
	}

	public static string DebugDescribe()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		int value;
		return "day " + StoreStation.Instance.dayCounter + " | first visit " + storeClientData.louFirstVisitDay + " met " + storeClientData.louMetDay + " arrested " + storeClientData.louArrestedDay + " morning " + storeClientData.louMorningDay + " bomb " + storeClientData.louBombVisitDay + " box drop " + storeClientData.louBoxDropDay + " collect " + storeClientData.louBoxCollectDay + " disguise " + storeClientData.louDisguiseDay + " demand " + storeClientData.louDemandDay + " | shot " + storeClientData.louShotDay + " ammo " + storeClientData.isLouAmmoLeft + " locker " + storeClientData.louLockerDay + " short " + storeClientData.isLouLockerShort + " locker paid " + storeClientData.louLockerPaidDay + " | operative met " + storeClientData.louOperativeMetDay + " transmitter " + storeClientData.isLouTransmitterGiven + " captured " + storeClientData.louCapturedDay + " rewarded " + storeClientData.isLouOperativeRewardGiven + " questioned " + storeClientData.isLouOperativeQuestioned + " suspicion " + storeClientData.isLouOperativeSuspicionApplied + " | reveal " + storeClientData.louRevealDay + " trap " + storeClientData.louTrapDay + " won " + storeClientData.isLouTrapWon + " thank-you " + storeClientData.isLouThankYouGiven + " | games stopped " + IsLouGamesStopped() + " poster " + (PlayerStore.Instance.wantedStatus.TryGetValue("wanted5", out value) ? value.ToString() : "open");
	}

	public static void DebugResetStory()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		storeClientData.louEligibleDay = -1;
		storeClientData.louFirstVisitDay = -1;
		storeClientData.louMetDay = -1;
		storeClientData.louArrestedDay = -1;
		storeClientData.louMorningDay = -1;
		storeClientData.louBoughtItemIds.Clear();
		storeClientData.louBombVisitDay = -1;
		storeClientData.isLouBombPlaced = false;
		storeClientData.isLouBombVisitPending = false;
		storeClientData.louBoxDropDay = -1;
		storeClientData.louBoxCollectDay = -1;
		storeClientData.isLouBoxNotPlaced = false;
		storeClientData.louDisguiseDay = -1;
		storeClientData.louDemandGiven = 0;
		storeClientData.louDemandDay = -1;
		storeClientData.louShotDay = -1;
		storeClientData.louOperativeMetDay = -1;
		storeClientData.louOperativeSprite = "";
		storeClientData.isLouTransmitterGiven = false;
		storeClientData.isLouAmmoLeft = false;
		storeClientData.louLockerDay = -1;
		storeClientData.isLouLockerShort = false;
		storeClientData.louLockerPaidDay = -1;
		storeClientData.louCapturedDay = -1;
		storeClientData.isLouOperativeRewardGiven = false;
		storeClientData.isLouOperativeSuspicionApplied = false;
		storeClientData.isLouOperativeQuestioned = false;
		storeClientData.louRevealDay = -1;
		storeClientData.louTrapDay = -1;
		storeClientData.isLouTrapWon = false;
		storeClientData.isLouThankYouGiven = false;
		PlayerStore.Instance.wantedStatus.Remove("wanted5");
		foreach (GameItem item in EmporiumEntry.Instance.GetAllItems().FindAll((GameItem i) => i != null && (i.identifier == "republic_transmitter" || i.IsTag("LOU_LOCKER_TAG") || i.IsTag("LOU_TOY_BOX_TAG") || i.identifier == "lou_bomb")))
		{
			item.Destroy();
		}
	}

	public static void QueueVisits(StoreClientManager manager)
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		int dayCounter = StoreStation.Instance.dayCounter;
		if (!storeClientData.isLouTransmitterGiven)
		{
			bool flag = storeClientData.louShotDay >= 0;
			bool flag2 = storeClientData.louArrestedDay >= 0 && storeClientData.louBoxCollectDay < 0 && storeClientData.louRevealDay < 0;
			if (flag || flag2)
			{
				manager.AddNextClient(CreateOperativeIntro(flag));
			}
		}
		else if (storeClientData.louCapturedDay >= 0 && dayCounter > storeClientData.louCapturedDay && !storeClientData.isLouOperativeRewardGiven)
		{
			manager.AddNextClient(CreateOperativeReward());
		}
		else if (storeClientData.louTrapDay >= 0 && !storeClientData.isLouTrapWon && dayCounter > storeClientData.louTrapDay && !storeClientData.isLouOperativeRewardGiven)
		{
			manager.AddNextClient(CreateOperativeTrapFailed());
		}
		else
		{
			int num = ((storeClientData.louLockerPaidDay >= 0) ? storeClientData.louLockerPaidDay : storeClientData.louDemandDay);
			if (num >= 0 && storeClientData.louCapturedDay < 0 && storeClientData.louRevealDay < 0 && !storeClientData.isLouOperativeQuestioned && dayCounter >= num + StoreClientData.LouDelay(2))
			{
				manager.AddNextClient(CreateOperativeQuestioning());
			}
		}
	}

	private static string OperativeName()
	{
		return LocHelper.GetLocalizedOrDefault("ClientName", "name_republic_operative", "Republic Operative");
	}

	private static string OperativeSprite()
	{
		return "maleMerc3";
	}

	private static StoreClient CreateOperativeClient(string identifier)
	{
		return new StoreClient
		{
			identifier = identifier,
			displayName = OperativeName(),
			spriteName = OperativeSprite(),
			clientFaction = "FACTION_SECURITY",
			reportDegradesFaction = false,
			clientIntent = StoreClient.ClientIntent.DIALOGUE,
			dismissable = false,
			arrestable = false,
			nonShootable = true,
			bubbleColor = "blue"
		};
	}

	private static Action LeaveAction()
	{
		return delegate
		{
			PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
		};
	}

	public static StoreClient CreateOperativeIntro(bool isShotPath)
	{
		StoreClient storeClient = CreateOperativeClient(isShotPath ? "louOperativeIntroShot" : "louOperativeIntro");
		string displayName = storeClient.displayName;
		Action startAction = delegate
		{
			StoreClientData storeClientData2 = PlayerStore.Instance.storeClientData;
			if (storeClientData2.louOperativeMetDay < 0)
			{
				storeClientData2.louOperativeMetDay = StoreStation.Instance.dayCounter;
			}
		};
		Action startAction2 = delegate
		{
			StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
			if (!storeClientData.isLouTransmitterGiven)
			{
				storeClientData.isLouTransmitterGiven = true;
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("republic_transmitter"), isOwend: true);
				if (isShotPath)
				{
					PlayerStore.Instance.ModCash(750);
				}
			}
		};
		if (isShotPath)
		{
			storeClient.mainDialogue.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_1", "Shopkeeper, my team and I have been tracking a woman by the name of Lou for a while now.")).SetStartAction(startAction).NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_shot_2", "The woman you shot yesterday. You are the first person in years to put a hole in her. Congratulations."))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_shot_3", "She is a traitor to the Republic and a terrorist. And she does not forgive. She will haunt you."))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_cooperate", "Your best chance is to cooperate with us."))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_shot_4", "Take this transmitter. She will come for payback. Whatever she makes you hand over, hide it inside."))
				.SetStartAction(startAction2)
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_5", "When it reaches her hideout, we move in. There is intel in there the Republic needs extracted."))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_shot_6", "{0} Credits now, for your trouble. Another {1} when we have her. Not a word of this to anyone.", 750, 750))
				.SetEndAction(LeaveAction());
		}
		else
		{
			storeClient.mainDialogue.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_1", "Shopkeeper, my team and I have been tracking a woman by the name of Lou for a while now.")).SetStartAction(startAction).NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_rep_2", "We believe you just met her. She is a traitor to the Republic and a terrorist."))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_rep_3", "Those regular Security officers will not hold her."))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_rep_3b", "She will come back here, and she will want to play."))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_cooperate", "Your best chance is to cooperate with us."))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_rep_4", "She is a psychopath, but a predictable one. Be patient. Play along."))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_rep_5", "Take this transmitter. Hide it inside something she takes away with her. A box, a crate, anything that leaves with her."))
				.SetStartAction(startAction2)
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_5", "When it reaches her hideout, we move in. There is intel in there the Republic needs extracted."))
				.NextDialogue()
				.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_rep_7", "Help us capture her and {0} Credits are yours. Not a word of this to anyone.", 750))
				.SetEndAction(LeaveAction());
		}
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateOperativeReward()
	{
		StoreClient storeClient = CreateOperativeClient("louOperativeReward");
		string displayName = storeClient.displayName;
		storeClient.mainDialogue.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_reward_1", "It is done. We have her.")).NextDialogue().SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_reward_2", "The Republic is grateful. Here's your {0} Credits.", 750))
			.SetStartAction(PayReward)
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_reward_3", "She is going to rot in a black site for the rest of her life."))
			.SetEndAction(LeaveAction());
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateOperativeTrapFailed()
	{
		StoreClient storeClient = CreateOperativeClient("louOperativeTrapFailed");
		string displayName = storeClient.displayName;
		storeClient.mainDialogue.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_fail_1", "It is done. We have her. It cost us two good people.")).NextDialogue().SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_fail_2", "Somehow she knew we were coming. Funny, that."))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_fail_3", "Your {0} Credits, as agreed. The Republic keeps its word.", 750))
			.SetStartAction(PayReward)
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_fail_4", "Security will be keeping a close eye on this store from now on. Just in case."))
			.SetStartAction(delegate
			{
				StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
				if (!storeClientData.isLouOperativeSuspicionApplied)
				{
					storeClientData.isLouOperativeSuspicionApplied = true;
					StoreReputation.ModSecReputation(-12);
				}
			})
			.SetEndAction(LeaveAction());
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static void PayReward()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (!storeClientData.isLouOperativeRewardGiven)
		{
			storeClientData.isLouOperativeRewardGiven = true;
			PlayerStore.Instance.ModCash(750);
			PlayerStore.Instance.CloseWantedPoster("wanted5");
		}
	}

	public static StoreClient CreateOperativeQuestioning()
	{
		StoreClient storeClient = CreateOperativeClient("louOperativeQuestioning");
		string displayName = storeClient.displayName;
		storeClient.mainDialogue.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_ask_1", "You read the news, I take it. An admiral of the Republic, dead on this station.")).SetStartAction(delegate
		{
			PlayerStore.Instance.storeClientData.isLouOperativeQuestioned = true;
		}).NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_ask_2", "She did this, and we could have stopped her."))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_ask_2b", "I thought we had a mutual understanding about her."))
			.NextDialogue()
			.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_lou_operative_ask_3", "You are lucky we have to leave soon to track her down, or we would have looked further into your involvement."))
			.SetEndAction(LeaveAction());
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

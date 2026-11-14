public class StoreClientListUnused
{
	public static StoreClient CreateBilly1()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "billy1";
		storeClient.displayName = "Billy";
		storeClient.spriteName = "billy";
		storeClient.clientIntent = StoreClient.ClientIntent.BARTER;
		storeClient.useClientBudget = false;
		storeClient.clientBuyingTagList.Add("FOOD");
		Dialogue dialogueReport = new Dialogue();
		dialogueReport.SetText("PLAYER", "Sorry, can't help.");
		Dialogue dialogueStop = new Dialogue();
		dialogueStop.SetText("PLAYER", "Go get a job.");
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello, I am Billy").NextDialogue().SetText(storeClient.displayName, "Can you please spare some food?")
			.NextDialogue()
			.SetText(storeClient.displayName, "I don't have any money.")
			.AddChoice(DirectoryMaster.Item("$"), "", "", dialogueReport, delegate
			{
				storeClient.barterOffer.InstanciateOffer();
				dialogueReport.NextDialogue().SetText(storeClient.displayName, "I guess the economy is tough on small business too.").NextDialogue()
					.SetText(storeClient.displayName, "Thank anyway.")
					.SetEndAction(delegate
					{
						PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
					});
			})
			.AddChoice(DirectoryMaster.Item("$"), "", "", dialogueStop, delegate
			{
				dialogueStop.NextDialogue().SetText("PLAYER", "This is not a charity.").NextDialogue()
					.SetText(storeClient.displayName, "Fine, I'll be back.")
					.SetEndAction(delegate
					{
						PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
					});
			});
		storeClient.barterOffer = new BarterOffer();
		storeClient.barterOffer.barterTextBody = "Spare Food?";
		storeClient.barterOffer.onInstance = delegate
		{
		};
		storeClient.barterOffer.getBarterResult = delegate
		{
			BarterResult barterResult = new BarterResult();
			if (EmporiumEntry.Instance.IsBackInvContainItemTag("FOOD"))
			{
				barterResult.isAccepted = true;
				barterResult.response = "That look good!";
			}
			else
			{
				barterResult.isAccepted = false;
				barterResult.response = "Can you spare some food?";
			}
			return barterResult;
		};
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, "Thank you!!");
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, "Have a great day!!!").SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateBilly2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "billy2";
		storeClient.displayName = "Billy";
		storeClient.spriteName = "billy";
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(100);
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello, I am back").NextDialogue().SetText(storeClient.displayName, "I am here to buy some equipments.")
			.NextDialogue()
			.SetText(storeClient.displayName, "it's for my new job!")
			.NextDialogue()
			.SetText("PLAYER", "You got a job?")
			.NextDialogue()
			.SetText(storeClient.displayName, "It's in security.")
			.NextDialogue()
			.SetText(storeClient.displayName, "I need a flashlight")
			.NextDialogue()
			.SetText(storeClient.displayName, "and some kind of weapon.")
			.NextDialogue()
			.SetText(storeClient.displayName, "If you don't have any,")
			.NextDialogue()
			.SetText(storeClient.displayName, "I'll find it somewhere else.")
			.SetEndAction(delegate
			{
			});
		storeClient.placeRightItemWhenSellingToDialogue = new Dialogue().SetText(storeClient.displayName, "That's good.");
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, "Deal.");
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, "Wish me luck for my first day!").SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateBilly3()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "billy3";
		storeClient.displayName = "Billy";
		storeClient.spriteName = "billy";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello, It's me, Billy!").NextDialogue().SetText(storeClient.displayName, "I got some stuffs for sale.")
			.SetEndAction(delegate
			{
			})
			.NextDialogue()
			.SetText("PLAYER", "Where did you get this?")
			.NextDialogue()
			.SetText(storeClient.displayName, "It's a bonus from work.")
			.NextDialogue()
			.SetText(storeClient.displayName, "You gonna take it or what?")
			.SetEndAction(delegate
			{
			});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, "Deal.");
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, "See you around!").SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateBillySecOfficier()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "billy3Officier";
		storeClient.displayName = "Security Officer";
		storeClient.spriteName = SpriteDict.GetRandomSecSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello merchant.").NextDialogue().SetText(storeClient.displayName, "We are looking for a suspect,")
			.NextDialogue()
			.SetText(storeClient.displayName, "who robbed the Upper Level Medbay storage earlier.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Have you seen anyone suspicious, selling meds?")
			.SetEndAction(delegate
			{
			});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, "Deal.");
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, "See you around!").SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		storeClient.CompleteSecClient();
		return storeClient;
	}

	public static StoreClient CreateRevManifesto()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "revManifesto";
		storeClient.displayName = "Revolutionary";
		storeClient.spriteName = SpriteDict.GetRandomRevSpriteName();
		StoreClientFactionSetup.InitRev(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.SetWanted(25, "", isStealthWanted: true);
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello friend.").NextDialogue().SetText(storeClient.displayName, "Does your landlord raise your rent every week?")
			.NextDialogue()
			.SetText(storeClient.displayName, "You know that the clinics here barely have the essential medicine.")
			.NextDialogue()
			.SetText(storeClient.displayName, "While the Upper Levels are trying to extend their lifespan.")
			.NextDialogue()
			.SetText(storeClient.displayName, "We break our backs just to survive, yet somehow, it is never enough.")
			.NextDialogue()
			.SetText(storeClient.displayName, "This isn't bad luck. It's injustice.")
			.NextDialogue()
			.SetText(storeClient.displayName, "The MegaCorps are milking us dry.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Station Command does nothing to help us.")
			.NextDialogue()
			.SetText(storeClient.displayName, "And Security? They uphold the law, which mean protecting the status quo.")
			.NextDialogue()
			.SetText(storeClient.displayName, "They have so much, yet they share so little.")
			.NextDialogue()
			.SetText(storeClient.displayName, "But there are more of us than there are of them. ")
			.NextDialogue()
			.SetText(storeClient.displayName, "Together, we could change things.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Here, take this. Read it when you have a quiet moment.")
			.SetEndAction(delegate
			{
			});
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText("CLIENT", "You snitched on me?").NextDialogue().SetText("CLIENT", "You can't arrest me! I didn't do anything illegal!")
			.NextDialogue()
			.SetText("SEC", "How about inciting a riot?")
			.NextDialogue()
			.SetText("SEC", "You are coming with us!")
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateAddictTrailer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "addictGeneralTrailer";
		storeClient.displayName = "Addict";
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.clientBuyingTagList.Add("SUBSTANCE");
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hey man, do you have anything to take the edge off?").NextDialogue().SetText(storeClient.displayName, "<i>*SPASM*</i>")
			.NextDialogue()
			.SetText(storeClient.displayName, "Oxymore, Dream dust, anything.")
			.SetEndAction(delegate
			{
			});
		storeClient.repeatDialogue = new Dialogue();
		storeClient.repeatDialogue.SetText(storeClient.displayName, "I said I want to barter these for drugs").NextDialogue().SetText(storeClient.displayName, "Do you have some or not?!");
		storeClient.onItemAddedToWeighted = delegate(GameItem item)
		{
			if (item.IsTag("SUBSTANCE"))
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, "That's the stuff!");
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateTrailerShadyBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "shadyBuyerStory";
		storeClient.displayName = "Shady Buyer";
		storeClient.spriteName = "sha3";
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.clientBuyingTagList.Add("ORGAN");
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hey man, do you have anything to take the edge off?").NextDialogue().SetText(storeClient.displayName, "<i>*SPASM*</i>")
			.NextDialogue()
			.SetText(storeClient.displayName, "Oxymore, Dream dust, anything.")
			.SetEndAction(delegate
			{
			});
		storeClient.repeatDialogue = new Dialogue();
		storeClient.repeatDialogue.SetText(storeClient.displayName, "I said I want to barter these for drugs").NextDialogue().SetText(storeClient.displayName, "Do you have some or not?!");
		storeClient.onItemAddedToWeighted = delegate(GameItem item)
		{
			if (item.IsTag("ORGAN"))
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, "Excellent");
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateDebugClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "debug_client";
		storeClient.displayName = "Debug Client";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.buyPriceModifier = -10;
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello,").SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterTestStripBox());
			PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
		});
		storeClient.clientBuyingTagList.Add("SUBSTANCE");
		storeClient.canClientExposeFeature = ClientCanExposeFunc.BasicClientExposeCapacity;
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateDebugClient2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "debug_client";
		storeClient.displayName = "Debug Client";
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(999);
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello,").SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GutterflowAsBaseWater());
			PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateBaseWater());
		});
		storeClient.clientBuyingTagList.Add("ALL_TYPE");
		storeClient.canClientExposeFeature = ClientCanExposeFunc.BasicClientExposeCapacity;
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateDebugStamp()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "debug_client";
		storeClient.displayName = "Debug Client";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello,").SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateFakeStamp());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateFakeStamp());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateFakeStamp());
		});
		storeClient.clientBuyingTagList.Add("SUBSTANCE");
		storeClient.canClientExposeFeature = ClientCanExposeFunc.BasicClientExposeCapacity;
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateButcherClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "butcher";
		storeClient.displayName = "Butcher";
		if (RNG.Roll(50))
		{
			storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		}
		else
		{
			storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		}
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateTrailerCodewordClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "codewordClientTrailer";
		storeClient.displayName = "???";
		storeClient.spriteName = SpriteDict.GetRandomMerchantSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, "The true flower has bloom in the edge of the night.").SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePaperWithText("|%]{-\\%+-*+?%@%~-]}+&-(*|%*|&]]^%*&(*+-}{?"), isOwend: true);
			PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateTrailerGunBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "gunBuyerTrader";
		storeClient.displayName = "Militiaman";
		storeClient.spriteName = "wanted3";
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, "You got guns?").NextDialogue().SetText(storeClient.displayName, "I need them without serial numbers")
			.NextDialogue()
			.SetText("PLAYER", "Sorry, Can't help you.")
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateTrailerChemist()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "trailerChemist";
		storeClient.displayName = "Lower Levels Chemist";
		storeClient.spriteName = SpriteDict.GetRandomScientistSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -20;
		storeClient.mainDialogue.SetText(storeClient.displayName, "I just cooked up a new batch.").NextDialogue().SetText(storeClient.displayName, "You want some?")
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateTrailerWeaponSeller()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "trailerWeaponSeller";
		storeClient.displayName = "Lower Levels Dealer";
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -20;
		storeClient.mainDialogue.SetText(storeClient.displayName, "I got your message.").SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("glock_receiver");
			PlayerStore.Instance.AddDirectSellingItemToTable("glock_receiver");
			PlayerStore.Instance.AddDirectSellingItemToTable("antique_grenade");
			PlayerStore.Instance.AddDirectSellingItemToTable("antique_grenade");
			PlayerStore.Instance.AddDirectSellingItemToTable("antique_grenade");
			PlayerStore.Instance.AddDirectSellingItemToTable("antique_grenade");
			PlayerStore.Instance.AddDirectSellingItemToTable("satchel");
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateTrailerCultist()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "cultistTrailer";
		storeClient.displayName = "Cultist";
		storeClient.spriteName = "cul1";
		storeClient.bubbleColor = "dark";
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, "<i>Greeting shopkeeper.</i>").NextDialogue().SetText(storeClient.displayName, "<i>We require more blood.</i>")
			.SetEndAction(delegate
			{
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateTrailerGreytide()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "greytide";
		storeClient.displayName = "Lower Levels Citizen";
		storeClient.spriteName = SpriteDict.GetGreytideSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -RNG.GetRandomInt(15, 25);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, "How much I can get for these?").SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("stun_baton");
			PlayerStore.Instance.AddDirectSellingItemToTable("glock_receiver");
			PlayerStore.Instance.AddDirectSellingItemToTable("sec_keycard");
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

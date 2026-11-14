public class StoreClientListBarter
{
	public static StoreClient CreateAddict()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "addictBarter";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scav_addict");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.SetBarterOffer(BarterOfferList.CreateAddictBarter());
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_2"))
			.SetEndAction(delegate
			{
				storeClient.barterOffer.InstanciateOffer();
			});
		storeClient.repeatDialogue = new Dialogue();
		storeClient.repeatDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_repeat_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_repeat_2"));
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateAddictGeneral()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "addictGeneralBarter";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scav_addict");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.alwaysFeedback = true;
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.SetBarterOffer(BarterOfferList.CreateAddictGeneralBarter());
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_gen_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_gen_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_gen_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_gen_4"))
			.SetEndAction(delegate
			{
				storeClient.barterOffer.InstanciateOffer();
			});
		storeClient.repeatDialogue = new Dialogue();
		storeClient.repeatDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_gen_repeat_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_gen_repeat_2"));
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_gen_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_gen_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateBasicFoodBarterTutorial()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "storyFoodBarter";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_poor_scav");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.SetBarterOffer(BarterOfferList.CreateBarterFoodMultiOffer());
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_tutorial_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_tutorial_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_tutorial_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_tutorial_4"))
			.SetEndAction(delegate
			{
				storeClient.barterOffer.InstanciateOffer();
			});
		storeClient.repeatDialogue = new Dialogue();
		storeClient.repeatDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_tutorial_repeat"));
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_tutorial_accept")).SetEndAction(delegate
		{
			TutorialUIManager.Instance.CloseBarterTutorial();
		});
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_tutorial_done")).SetEndAction(delegate
		{
			TutorialUIManager.Instance.CloseBarterTutorial();
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateAddictBlood()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "addictBarterBlood";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_addict");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BARTER;
		storeClient.SetBarterOffer(new BarterOffer());
		storeClient.barterOffer.barterTextBody = "Barter for a OxyMore Injector";
		storeClient.barterOffer.onInstance = delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
			PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
		};
		storeClient.barterOffer.getBarterResult = delegate
		{
			BarterResult barterResult = new BarterResult();
			if (EmporiumEntry.Instance.IsBackInvContainItem("pink_injector"))
			{
				barterResult.isAccepted = true;
				barterResult.response = LocHelper.GetLocalizedDialogue("dialog_barter_addict_blood_yes");
			}
			else
			{
				barterResult.isAccepted = false;
				barterResult.response = LocHelper.GetLocalizedDialogue("dialog_barter_addict_blood_no");
			}
			return barterResult;
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_blood_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_blood_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_blood_3"))
			.SetEndAction(delegate
			{
				storeClient.barterOffer.InstanciateOffer();
			});
		storeClient.repeatDialogue = new Dialogue();
		storeClient.repeatDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_repeat_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_repeat_2"));
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_blood_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_addict_blood_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateScavBarter()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavBarter";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scav");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BARTER;
		storeClient.SetBarterOffer(new BarterOffer());
		storeClient.barterOffer.barterTextBody = "Barter for a Moonshine";
		storeClient.barterOffer.onInstance = delegate
		{
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("nuts_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
			}
			else
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("paper_towel");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("shampoo");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("paper_towel");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("shampoo");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper");
				}
			}
		};
		storeClient.barterOffer.getBarterResult = delegate
		{
			BarterResult barterResult = new BarterResult();
			if (EmporiumEntry.Instance.IsBackInvContainItem("moonshine"))
			{
				barterResult.isAccepted = true;
				barterResult.response = LocHelper.GetLocalizedDialogue("dialog_barter_scav_yes");
			}
			else
			{
				barterResult.isAccepted = false;
				barterResult.response = LocHelper.GetLocalizedDialogue("dialog_barter_scav_no");
			}
			return barterResult;
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_scav_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_scav_2"))
			.SetEndAction(delegate
			{
				storeClient.barterOffer.InstanciateOffer();
			});
		storeClient.repeatDialogue = new Dialogue();
		storeClient.repeatDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_scav_repeat"));
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_scav_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_scav_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLowerLevelChemistBarter()
	{
		return CreateLowerLevelChemistBarter(carryContraband: true);
	}

	public static StoreClient CreateLowerLevelChemistBarter(bool carryContraband = true)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lowerLevelChemistBarter";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_chemist");
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BARTER;
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.SetBarterOffer(new BarterOffer());
		storeClient.barterOffer.barterTextBody = "Barter: Everything on offer for 2 Food Items";
		storeClient.barterOffer.onInstance = delegate
		{
			int randomInt = RNG.GetRandomInt(0, 1);
			if (carryContraband)
			{
				randomInt = RNG.GetRandomInt(0, 2);
			}
			switch (randomInt)
			{
			case 0:
				PlayerStore.Instance.AddDirectSellingItemToTable("salve");
				break;
			case 1:
				PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill");
				PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill");
				PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill");
				PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill");
				break;
			case 2:
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
				break;
			}
		};
		storeClient.barterOffer.getBarterResult = delegate
		{
			BarterResult barterResult = new BarterResult();
			if (EmporiumEntry.Instance.IsBackInvContainItemTag("FOOD", 2))
			{
				barterResult.isAccepted = true;
				barterResult.response = LocHelper.GetLocalizedDialogue("dialog_barter_chemist_yes");
			}
			else
			{
				barterResult.isAccepted = false;
				barterResult.response = LocHelper.GetLocalizedDialogue("dialog_barter_chemist_no");
			}
			return barterResult;
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_chemist_1")).SetEndAction(delegate
		{
			storeClient.barterOffer.InstanciateOffer();
		});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_chemist_accept"));
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_chemist_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateNormalFoodBarter()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "normalFoodBarter";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_poor_scav");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.SetBarterOffer(BarterOfferList.CreateBarterFoodMultiOffer());
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_normal_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_normal_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_normal_3"))
			.SetEndAction(delegate
			{
				storeClient.barterOffer.InstanciateOffer();
			});
		storeClient.repeatDialogue = new Dialogue();
		storeClient.repeatDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_normal_repeat"));
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_normal_accept")).SetEndAction(delegate
		{
		});
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_barter_food_normal_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

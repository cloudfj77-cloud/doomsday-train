using System;

public class StoreClientListRev
{
	public static StoreClient CreateRevRaider()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "revRaider";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_revolution_raider");
		storeClient.spriteName = SpriteDict.GetRandomRevSpriteName();
		StoreClientFactionSetup.InitRev(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		int wholesale = 20 + (StoreReputation.GetRevReputation() - 40) / 4;
		storeClient.SetWholesale(wholesale);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_rev_raider_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_rev_raider_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_rev_raider_3"))
			.SetEndAction(delegate
			{
				if (RNG.Roll(12))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("energy_credit_dis", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(6))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("energy_credit_breeder", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(7.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("turbo_booster_adv", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(7.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("advanced_flux_agent", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(6))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("module_extractor_advanced", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(9))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("module_bay_expansion_kit", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_performance_module"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_quality_module"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_efficiency_module"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_overclock_module"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_eco_module"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_fineness_module"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("blue_blood_bag", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(37.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(37.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
				}
				if (RNG.Roll(15))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceHandgun(), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(10.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceSMG(), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("small_raw_meat", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("small_raw_meat", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("small_raw_meat", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_medical", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_medical", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper");
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("toothpaste");
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("shampoo");
				}
				if (RNG.Roll(7.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
				}
				if (RNG.Roll(7.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
				}
				if (RNG.Roll(7.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
				}
				if (RNG.Roll(45))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_chemical", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("restricted_chemical", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(22.5f))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("module_extractor", isOwend: false, isStolen: true);
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper");
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateRevQuartermaster()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "revQuartermaster";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_revolution_quartermaster");
		storeClient.spriteName = SpriteDict.GetRandomRevSpriteName();
		StoreClientFactionSetup.InitRev(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		int wholesale = 20 + (StoreReputation.GetRevReputation() - 40) / 4;
		storeClient.SetWholesale(wholesale);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_rev_quartermaster_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_rev_quartermaster_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_rev_quartermaster_3"))
			.SetEndAction(delegate
			{
				if (RNG.Roll(25))
				{
					int value = 5 + StoreReputation.GetRevReputation() / 20;
					value = Math.Clamp(value, 5, 12);
					for (int i = 0; i < value; i++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("common_ore");
					}
				}
				else if (RNG.Roll(25))
				{
					int value2 = 5 + StoreReputation.GetRevReputation() / 20;
					value2 = Math.Clamp(value2, 5, 12);
					for (int j = 0; j < value2; j++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
					}
				}
				else
				{
					int value3 = 6 + StoreReputation.GetRevReputation() / 20;
					value3 = Math.Clamp(value3, 5, 15);
					for (int k = 0; k < value3; k++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
					}
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateRevolutionaryBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "revolutionaryBuyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_revolutionary_fighter");
		storeClient.spriteName = SpriteDict.GetRandomRevSpriteName();
		StoreClientFactionSetup.InitRev(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(RNG.GetRandomInt(150, 200) + StoreStation.Instance.dayCounter * 2);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_rev_buyer_1", "Hey, I need some supplies.")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_rev_buyer_2", "We are making a move soon."))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_rev_buyer_3", "You got weapons and meds?"));
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("MUNITION");
		storeClient.clientBuyingTagList.Add("AMMUNITION");
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateRevSupplyBuyerTrailer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "revSupplyBuyerTrailer";
		storeClient.displayName = "Revolution Quartermaster";
		storeClient.spriteName = SpriteDict.GetRandomRevSpriteName();
		StoreClientFactionSetup.InitRev(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(500);
		storeClient.cooldown = 3;
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("AMMUNITION");
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.mainDialogue.SetText(storeClient.displayName, "The Revolution is bleeding, comrade.").NextDialogue().SetText(storeClient.displayName, "Our fighters need supplies before the next push.")
			.NextDialogue()
			.SetText(storeClient.displayName, "I'm here to buy whatever you can spare.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Firearms, ammunition, medical supplies.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Every round keeps the cause alive.")
			.NextDialogue()
			.SetText(storeClient.displayName, "Name your price.");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

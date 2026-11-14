public class StoreClientListSurplus
{
	public static StoreClient CreateFoodSurplusClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "food_surplus_client";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_trader");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.cooldown = 2;
		if (StoreEventManager.GetShortageType("FOOD") == -2)
		{
			storeClient.SetWholesale(-20);
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_surplus_food_oversupply")).SetEndAction(delegate
			{
				for (int j = 0; j < 5; j++)
				{
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"));
					}
				}
				for (int k = 0; k < 5; k++)
				{
					if (RNG.Roll(25))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"));
					}
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
			});
		}
		else
		{
			storeClient.SetWholesale(-15);
			storeClient.mainDialogue.SetText(storeClient.displayName, "").NextDialogue().SetText(storeClient.displayName, "")
				.SetEndAction(delegate
				{
					for (int i = 0; i < 5; i++)
					{
						if (RNG.Roll(50))
						{
							PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"));
						}
					}
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				});
		}
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMedicalSurplusClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "medical_surplus_client";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_vendor");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.cooldown = 2;
		if (StoreEventManager.GetShortageType("MEDICAL") == -2)
		{
			storeClient.SetWholesale(-20);
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_surplus_medical_oversupply")).SetEndAction(delegate
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("bandage_item");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("topical_bandage_item");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("salve");
				}
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("bandage_item");
				}
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("topical_bandage_item");
				}
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
				}
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill");
				}
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
				}
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("salve");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("bandage_item");
			});
		}
		else
		{
			storeClient.SetWholesale(-15);
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_surplus_medical_normal")).SetEndAction(delegate
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("bandage_item");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("topical_bandage_item");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("salve");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("bandage_item");
			});
		}
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSubstanceSurplusClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "substance_surplus_client";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_shifty_dealer");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.cooldown = 2;
		if (StoreEventManager.GetShortageType("SUBSTANCE") == -2)
		{
			storeClient.SetWholesale(-20);
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_surplus_substance_oversupply")).SetEndAction(delegate
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("fanta_pill");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("fanta_pill");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("fanta_pill");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
			});
		}
		else
		{
			storeClient.SetWholesale(-15);
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_surplus_substance_normal")).SetEndAction(delegate
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("fanta_pill");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("fanta_pill");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
			});
		}
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWeaponArmorSurplusClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "weapon_armor_surplus_client";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_weapon_trader");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.cooldown = 3;
		if (StoreEventManager.GetShortageType("WEAPON") == -2 || StoreEventManager.GetShortageType("ARMOR") == -2)
		{
			storeClient.SetWholesale(-20);
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_surplus_weapon_oversupply")).SetEndAction(delegate
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("combat_knife");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("box_cutter");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("hatchet");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("combat_knife");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("box_cutter");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("hatchet");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("armor_lining");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("armor_lining");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("armor_lining");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("kitchen_knife");
			});
		}
		else
		{
			storeClient.SetWholesale(-15);
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_surplus_weapon_normal")).SetEndAction(delegate
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("combat_knife");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("box_cutter");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("hatchet");
				}
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("combat_knife");
				}
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("box_cutter");
				}
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("hatchet");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("armor_lining");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("armor_lining");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("kitchen_knife");
			});
		}
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMaterialSurplusClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "material_surplus_client";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_trader");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.cooldown = 3;
		if (StoreEventManager.GetShortageType("MATERIAL") == -2)
		{
			storeClient.SetWholesale(-20);
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_surplus_material_oversupply")).SetEndAction(delegate
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_ore");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_ore");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_ore");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
			});
		}
		else
		{
			storeClient.SetWholesale(-15);
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_surplus_material_normal")).SetEndAction(delegate
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_ore");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_ore");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
			});
		}
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

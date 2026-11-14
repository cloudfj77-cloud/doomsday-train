public class StoreClientListShortage
{
	public static StoreClient CreateFoodShortageClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "food_shortage_client";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_citizen");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.SetBudget(100 + RNG.GetRandomInt(25, 75));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.cooldown = 2;
		if (StoreEventManager.GetShortageType("FOOD") == 2)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_food_sev2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_food_sev2_2"));
		}
		else
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_food_sev1_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_food_sev1_2"));
		}
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMedicalShortageClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "medical_shortage_client";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_worried_resident");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.SetBudget(120 + RNG.GetRandomInt(30, 80));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.cooldown = 2;
		if (StoreEventManager.GetShortageType("MEDICAL") == 2)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_medical_sev2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_medical_sev2_2"));
		}
		else
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_medical_sev1_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_medical_sev1_2"));
		}
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSubstanceShortageClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "substance_shortage_client";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_shady_addict");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.SetBudget(80 + RNG.GetRandomInt(20, 50));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.cooldown = 2;
		if (StoreEventManager.GetShortageType("SUBSTANCE") == 2)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_substance_sev2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_substance_sev2_2"));
		}
		else
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_substance_sev1_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_substance_sev1_2"));
		}
		storeClient.clientBuyingTagList.Add("SUBSTANCE");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWeaponArmorShortageClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "weapon_armor_shortage_client";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_tense_local");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.SetBudget(200 + RNG.GetRandomInt(50, 100));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.cooldown = 3;
		if (StoreEventManager.GetShortageType("WEAPON") == 2 || StoreEventManager.GetShortageType("ARMOR") == 2)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_weapon_sev2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_weapon_sev2_2"));
		}
		else if (StoreEventManager.GetShortageType("WEAPON") == 1 || StoreEventManager.GetShortageType("ARMOR") == 1)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_weapon_sev1_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_weapon_sev1_2"));
		}
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("ARMOR");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMaterialShortageClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "material_shortage_client";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scrap_dealer");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.SetBudget(150 + RNG.GetRandomInt(40, 90));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.cooldown = 3;
		if (StoreEventManager.GetShortageType("MATERIAL") == 2)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_material_sev2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_material_sev2_2"));
		}
		else
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_material_sev1_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shortage_material_sev1_2"));
		}
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

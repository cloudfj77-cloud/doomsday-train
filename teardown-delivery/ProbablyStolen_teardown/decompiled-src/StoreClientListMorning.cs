public class StoreClientListMorning
{
	public static StoreClient CreateMobLeader()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "morningMobLeader";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_mob_leader");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.dismissable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_morning_mob_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_morning_mob_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_morning_mob_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_morning_mob_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.ExecuteGameOver("mob");
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSecLeader()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "morningSecLeader";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_security_officer");
		storeClient.clientFaction = "FACTION_SECURITY";
		storeClient.spriteName = SpriteDict.GetRandomSecSpriteName();
		storeClient.dismissable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_morning_sec_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_morning_sec_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_morning_sec_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.ExecuteGameOver("sec");
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateULHitman()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "morningULHitman";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_hitman");
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomMercSpriteName();
		storeClient.dismissable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_morning_hitman_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.ExecuteGameOver("hitman");
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCartelRetired()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "cartelRetired";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_cartel_hitman");
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_morning_cartel_retired_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.ExecuteGameOver("cartel_retired");
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

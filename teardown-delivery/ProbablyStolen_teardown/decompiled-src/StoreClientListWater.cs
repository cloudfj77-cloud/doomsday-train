public class StoreClientListWater
{
	public static StoreClient GetRandomWaterSupplier()
	{
		if (RNG.Roll(20))
		{
			return CreateHonestWaterSeller();
		}
		if (RNG.Roll(12))
		{
			return CreateWaterCompany();
		}
		if (RNG.Roll(7))
		{
			return CreateDishonestWaterSellScav();
		}
		return CreateWaterSellScav();
	}

	public static StoreClient CreateWaterCompany()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "waterCompany";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_citizen");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-RNG.GetRandomInt(20, 25));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_company_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_water_company_2"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_water_company_3"))
			.SetEndAction(delegate
			{
				int randomInt = RNG.GetRandomInt(1, 2);
				for (int i = 0; i < randomInt; i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GetRandomWater());
				}
				if (RNG.Roll(15))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GhostWaterAsBaseWater());
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateBaseWater("large_bottled_water"));
				}
				if (RNG.Roll(10))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GhostWaterAsBaseWater("large_bottled_water"));
				}
				int randomInt2 = RNG.GetRandomInt(1, 2);
				for (int j = 0; j < randomInt2; j++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GetRandomWater());
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWaterSellScav()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "waterSellerScav";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scav");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-RNG.GetRandomInt(15, 20));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_scav_1")).SetEndAction(delegate
		{
			int randomInt = RNG.GetRandomInt(3, 5);
			for (int i = 0; i < randomInt; i++)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GetRandomWater());
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateDishonestWaterSellScav()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "waterHishonestSellerScav";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scav");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-RNG.GetRandomInt(30, 40));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_dishonest_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_water_dishonest_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateBaseWater());
				if (RNG.Roll(75))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateBaseWater());
				}
				if (RNG.Roll(75))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GhostWaterAsBaseWater());
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GhostWaterAsBaseWater());
				}
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GhostWaterAsPureWater());
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateBaseWater());
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateHonestWaterSeller()
	{
		StoreClient obj = new StoreClient
		{
			identifier = "honestBadWaterSeller",
			displayName = LocHelper.GetLocalizedClientName("name_scav"),
			spriteName = SpriteDict.GetRandomScavSpriteName()
		};
		StoreClientFactionSetup.InitScav(obj);
		obj.clientIntent = StoreClient.ClientIntent.SELL;
		obj.SetWholesale(-RNG.GetRandomInt(15, 20));
		SetHonestWaterSellerDialog(obj);
		obj.AddBasicDialog();
		obj.CompleteClientCreation();
		return obj;
	}

	public static void SetHonestWaterSellerDialog(StoreClient storeClient)
	{
		if (RNG.Roll(75))
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_honest_base_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_water_honest_base_2"))
				.SetEndAction(delegate
				{
					if (NetworkUpgrade.IsWaterTraderUnlocked())
					{
						int randomInt2 = RNG.GetRandomInt(1, 2);
						for (int j = 0; j < randomInt2; j++)
						{
							PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateBaseWater());
						}
						PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateBaseWater("large_bottled_water"));
					}
					else
					{
						int randomInt3 = RNG.GetRandomInt(3, 5);
						for (int k = 0; k < randomInt3; k++)
						{
							PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("water_ration"));
						}
					}
				});
			return;
		}
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_honest_ghost_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_water_honest_ghost_2"))
			.SetEndAction(delegate
			{
				int randomInt = RNG.GetRandomInt(2, 4);
				for (int i = 0; i < randomInt; i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateGhostWater());
				}
				PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateGhostWater("large_bottled_water"));
			});
	}
}

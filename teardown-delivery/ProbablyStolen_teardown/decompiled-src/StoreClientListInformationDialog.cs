public class StoreClientListInformationDialog
{
	public static void SetHarvestDialog(StoreClient storeClient)
	{
		int duration = RNG.GetRandomInt(2, 4);
		int timer = RNG.GetRandomInt(1, 4);
		string[] array = new string[3]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_harvest_intro_1"),
			LocHelper.GetLocalizedDialogue("dialog_info_harvest_intro_2"),
			LocHelper.GetLocalizedDialogue("dialog_info_harvest_intro_3")
		};
		string[] array2 = new string[3]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_harvest_mid_1", timer),
			LocHelper.GetLocalizedDialogue("dialog_info_harvest_mid_2", timer),
			LocHelper.GetLocalizedDialogue("dialog_info_harvest_mid_3", timer)
		};
		string[] array3 = new string[3]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_harvest_outcome_1"),
			LocHelper.GetLocalizedDialogue("dialog_info_harvest_outcome_2"),
			LocHelper.GetLocalizedDialogue("dialog_info_harvest_outcome_3")
		};
		string[] array4 = new string[3]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_harvest_conclusion_1", duration),
			LocHelper.GetLocalizedDialogue("dialog_info_harvest_conclusion_2", duration),
			LocHelper.GetLocalizedDialogue("dialog_info_harvest_conclusion_3", duration)
		};
		string text = array[RNG.GetRandomInt(0, array.Length - 1)];
		string text2 = array2[RNG.GetRandomInt(0, array2.Length - 1)];
		string text3 = array3[RNG.GetRandomInt(0, array3.Length - 1)];
		string text4 = array4[RNG.GetRandomInt(0, array4.Length - 1)];
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, text).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_harvest_listen"))
			.NextDialogue()
			.SetText(storeClient.displayName, text2)
			.NextDialogue()
			.SetText(storeClient.displayName, text3)
			.SetNextDialogue(storeClient.acceptDealDialogueLastLine);
		storeClient.acceptDealDialogueLastLine.SetText(storeClient.displayName, text4).SetEndAction(delegate
		{
			StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateNutrifruitHarvest(duration).CreateAndAddSlip(timer), timer, IsKnown: true);
		});
	}

	public static void SetBadHarvestDialog(StoreClient storeClient)
	{
		int duration = 3;
		int timer = 3;
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_bad_harvest_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_bad_harvest_listen"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_bad_harvest_mid", timer))
			.SetNextDialogue(storeClient.acceptDealDialogueLastLine);
		storeClient.acceptDealDialogueLastLine.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_bad_harvest_last", duration)).SetEndAction(delegate
		{
			StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateBadNutrifruitHarvestDuration(duration).CreateAndAddSlip(timer), timer, IsKnown: true);
		});
	}

	public static void SetMedicalSuppliesShortageDialog(StoreClient storeClient)
	{
		int duration = RNG.GetRandomInt(2, 4);
		int timer = RNG.GetRandomInt(1, 4);
		string[] array = new string[3]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_med_intro_1"),
			LocHelper.GetLocalizedDialogue("dialog_info_med_intro_2"),
			LocHelper.GetLocalizedDialogue("dialog_info_med_intro_3")
		};
		string[] array2 = new string[3]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_med_mid_1", timer),
			LocHelper.GetLocalizedDialogue("dialog_info_med_mid_2", timer),
			LocHelper.GetLocalizedDialogue("dialog_info_med_mid_3", timer)
		};
		string[] array3 = new string[3]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_med_conclusion_1", duration),
			LocHelper.GetLocalizedDialogue("dialog_info_med_conclusion_2", duration),
			LocHelper.GetLocalizedDialogue("dialog_info_med_conclusion_3", duration)
		};
		string text = array[RNG.GetRandomInt(0, array.Length - 1)];
		string text2 = array2[RNG.GetRandomInt(0, array2.Length - 1)];
		string text3 = array3[RNG.GetRandomInt(0, array3.Length - 1)];
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, text).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_med_scoop"))
			.NextDialogue()
			.SetText(storeClient.displayName, text2)
			.SetNextDialogue(storeClient.acceptDealDialogueLastLine);
		storeClient.acceptDealDialogueLastLine.SetText(storeClient.displayName, text3).SetEndAction(delegate
		{
			StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateCounterfeitMedicalSuppliesDiscoveredDuration(duration).CreateAndAddSlip(timer), timer, IsKnown: true);
		});
	}

	public static void SetStoreRoomExcavatedDialog(StoreClient storeClient)
	{
		int duration = 2;
		int timer = 1;
		string[] array = new string[4]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_intro_1"),
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_intro_2"),
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_intro_3"),
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_intro_4")
		};
		string[] array2 = new string[4]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_mid_1"),
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_mid_2"),
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_mid_3"),
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_mid_4")
		};
		string[] array3 = new string[4]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_outcome_1"),
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_outcome_2"),
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_outcome_3"),
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_outcome_4")
		};
		string[] array4 = new string[4]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_conclusion_1", duration),
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_conclusion_2", duration),
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_conclusion_3", duration),
			LocHelper.GetLocalizedDialogue("dialog_info_storeroom_conclusion_4", duration)
		};
		string text = array[RNG.GetRandomInt(0, array.Length - 1)];
		string text2 = array2[RNG.GetRandomInt(0, array2.Length - 1)];
		string text3 = array3[RNG.GetRandomInt(0, array3.Length - 1)];
		string text4 = array4[RNG.GetRandomInt(0, array4.Length - 1)];
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, text).NextDialogue().SetText(storeClient.displayName, text2)
			.NextDialogue()
			.SetText(storeClient.displayName, text3)
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_storeroom_tomorrow"))
			.SetNextDialogue(storeClient.acceptDealDialogueLastLine);
		storeClient.acceptDealDialogueLastLine.SetText(storeClient.displayName, text4).SetEndAction(delegate
		{
			StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateStoreRoomExcavatedDuration(duration).CreateAndAddSlip(timer), timer, IsKnown: true);
		});
	}

	public static void SetMiningExpeditionReturnDialog(StoreClient storeClient)
	{
		int duration = RNG.GetRandomInt(1, 2);
		int timer = RNG.GetRandomInt(1, 3);
		string[] array = new string[3]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_mining_intro_1"),
			LocHelper.GetLocalizedDialogue("dialog_info_mining_intro_2"),
			LocHelper.GetLocalizedDialogue("dialog_info_mining_intro_3")
		};
		string[] array2 = new string[4]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_mining_mid_1"),
			LocHelper.GetLocalizedDialogue("dialog_info_mining_mid_2"),
			LocHelper.GetLocalizedDialogue("dialog_info_mining_mid_3"),
			LocHelper.GetLocalizedDialogue("dialog_info_mining_mid_4")
		};
		string[] array3 = new string[4]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_mining_outcome_1"),
			LocHelper.GetLocalizedDialogue("dialog_info_mining_outcome_2"),
			LocHelper.GetLocalizedDialogue("dialog_info_mining_outcome_3"),
			LocHelper.GetLocalizedDialogue("dialog_info_mining_outcome_4")
		};
		string[] array4 = new string[4]
		{
			LocHelper.GetLocalizedDialogue("dialog_info_mining_conclusion_1", duration),
			LocHelper.GetLocalizedDialogue("dialog_info_mining_conclusion_2", duration),
			LocHelper.GetLocalizedDialogue("dialog_info_mining_conclusion_3", duration),
			LocHelper.GetLocalizedDialogue("dialog_info_mining_conclusion_4", duration)
		};
		string text = array[RNG.GetRandomInt(0, array.Length - 1)];
		string text2 = array2[RNG.GetRandomInt(0, array2.Length - 1)];
		string text3 = array3[RNG.GetRandomInt(0, array3.Length - 1)];
		string text4 = array4[RNG.GetRandomInt(0, array4.Length - 1)];
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, text).NextDialogue().SetText(storeClient.displayName, text2)
			.NextDialogue()
			.SetText(storeClient.displayName, text3)
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_mining_timer", timer))
			.SetNextDialogue(storeClient.acceptDealDialogueLastLine);
		storeClient.acceptDealDialogueLastLine.SetText(storeClient.displayName, text4).SetEndAction(delegate
		{
			StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateMiningExpeditionReturnDuration(duration).CreateAndAddSlip(timer), timer, IsKnown: true);
		});
	}

	public static void SetWaterTreatmentBreakdownDialog(StoreClient storeClient)
	{
		int duration = 3;
		int timer = 3;
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_water_breakdown_listen")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_water_breakdown_1", timer))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_water_breakdown_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_water_breakdown_3"))
			.SetNextDialogue(storeClient.acceptDealDialogueLastLine);
		storeClient.acceptDealDialogueLastLine.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_water_breakdown_last", duration)).SetEndAction(delegate
		{
			StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateWaterTreatmentPlantBreakdownDuration(duration).CreateAndAddSlip(timer), timer, IsKnown: true);
		});
	}

	public static void SetMinerReturnIceAsteroidDialog(StoreClient storeClient)
	{
		int duration = 2;
		int timer = 2;
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_ice_miner_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_ice_miner_2", timer))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_ice_miner_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_ice_miner_4"))
			.SetNextDialogue(storeClient.acceptDealDialogueLastLine);
		storeClient.acceptDealDialogueLastLine.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_ice_miner_last", duration)).SetEndAction(delegate
		{
			StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateMinerReturnFromIceAsteroidDuration(duration).CreateAndAddSlip(timer), timer, IsKnown: true);
		});
	}
}

public class StoreClientListProcurement
{
	public static StoreClient CreateNurseProcurementOffer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "nurseProcurementOffer";
		storeClient.displayName = "Nurse";
		storeClient.clientIntent = StoreClient.ClientIntent.PROCUREMENT_OFFER;
		storeClient.procurementData = new ProcurementData();
		storeClient.procurementData.descriptionOffer = "5 Blood Bags in 3 days";
		storeClient.procurementData.descriptionCollect = "5 Blood Bags";
		storeClient.procurementData.clientName = storeClient.displayName;
		storeClient.procurementData.finalReward = 100 + RNG.GetRandomInt(0, 25);
		storeClient.procurementData.day = 1;
		storeClient.procurementData.getBarterResult = delegate
		{
			BarterResult barterResult = new BarterResult();
			int num = 0;
			foreach (GameItem childItem in EmporiumEntry.Instance.backInvinvElement.childItems)
			{
				if (childItem.identifier.Equals("blood_bag"))
				{
					num++;
				}
			}
			if (num == 5)
			{
				barterResult.isAccepted = true;
				barterResult.response = "That's everything";
			}
			else
			{
				barterResult.isAccepted = false;
			}
			return barterResult;
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello, my clinic is in need of blood supply.").NextDialogue().SetText(storeClient.displayName, "Can you supply 5 Blood Bags in 3 days?")
			.NextDialogue()
			.SetText(storeClient.displayName, "We are willing to pay 100 credits");
		storeClient.acceptDealDialogue = new Dialogue();
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, "Good.").SetEndAction(delegate
		{
			CreateNurseProcurementCollect().procurementData = storeClient.procurementData;
		});
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, "See you in 3 days").SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static StoreClient CreateNurseProcurementCollect()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "nurseProcurementCollect";
		storeClient.displayName = "Nurse";
		storeClient.clientIntent = StoreClient.ClientIntent.PROCUREMENT_COLLECT;
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hello, I am back to collect the blood bags").NextDialogue().SetText(storeClient.displayName, "You have them I hope");
		storeClient.acceptDealDialogue = new Dialogue();
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, "Excellent, here is your credits.").SetEndAction(delegate
		{
		});
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, "Goodbye").SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMachinistProcurementOffer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "machinistProcurementOffer";
		storeClient.displayName = "Machinist";
		storeClient.spriteName = SpriteDict.GetRandomEngSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.PROCUREMENT_OFFER;
		storeClient.procurementData = new ProcurementData();
		storeClient.procurementData.descriptionOffer = "3 Metal Ingots";
		storeClient.procurementData.descriptionCollect = "3 Metal Ingots";
		storeClient.procurementData.clientName = storeClient.displayName;
		storeClient.procurementData.finalReward = 180 + RNG.GetRandomInt(15, 30);
		storeClient.procurementData.day = 4;
		storeClient.procurementData.getBarterResult = delegate
		{
			BarterResult barterResult = new BarterResult();
			int num = 0;
			foreach (GameItem childItem in EmporiumEntry.Instance.backInvinvElement.childItems)
			{
				if (childItem.identifier.Equals("metal_ingot"))
				{
					num++;
				}
			}
			if (num == 10)
			{
				barterResult.isAccepted = true;
				barterResult.response = "That's everything.";
			}
			else
			{
				barterResult.isAccepted = false;
			}
			return barterResult;
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hey there, I'm working on a project and need some supplies.").NextDialogue().SetText(storeClient.displayName, "Can you get 3 Metal Ingots in 4 days?")
			.NextDialogue()
			.SetText(storeClient.displayName, "I'll pay " + storeClient.procurementData.finalReward + " credits for it.");
		storeClient.acceptDealDialogue = new Dialogue();
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, "Great, I'll be back.").SetEndAction(delegate
		{
			StoreClient storeClient2 = CreateMachinistProcurementCollect();
			storeClient2.procurementData = storeClient.procurementData;
			PlayerStore.Instance.AddDirectSellingItemToTable(storeClient2.procurementData.CreateSlip(), isOwend: true);
		});
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, "See you in 4 days.").SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static StoreClient CreateMachinistProcurementCollect()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "machinistProcurementCollect";
		storeClient.displayName = "Machinist";
		storeClient.clientIntent = StoreClient.ClientIntent.PROCUREMENT_COLLECT;
		storeClient.mainDialogue.SetText(storeClient.displayName, "Hey, I'm here to pick up those Metal Ingots.").NextDialogue().SetText(storeClient.displayName, "You got them, right?");
		storeClient.acceptDealDialogue = new Dialogue();
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, "Excellent, here's your payment.").SetEndAction(delegate
		{
		});
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, "Catch you later.").SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

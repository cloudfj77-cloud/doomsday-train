using System;
using System.Collections.Generic;

public class StoreClientListInformation
{
	public static List<Action<StoreClient>> informationList = new List<Action<StoreClient>>
	{
		StoreClientListInformationDialog.SetHarvestDialog,
		StoreClientListInformationDialog.SetBadHarvestDialog,
		StoreClientListInformationDialog.SetMedicalSuppliesShortageDialog,
		StoreClientListInformationDialog.SetStoreRoomExcavatedDialog,
		StoreClientListInformationDialog.SetMiningExpeditionReturnDialog,
		StoreClientListInformationDialog.SetWaterTreatmentBreakdownDialog,
		StoreClientListInformationDialog.SetMinerReturnIceAsteroidDialog
	};

	public static List<int> informationListProba = new List<int> { 100, 150, 150, 100, 100, 150, 100 };

	public static readonly List<string> informationEventIds = new List<string> { "nutrifruitHarvest", "badNutrifruitHarvest", "medicalSuppliesShortage", "storeroom_excavated", "miningExpeditionReturn", "water_treatment_breakdown", "miner_return_ice_asteroid" };

	private static readonly LocalizedLines line1 = LocalizedLines.Dialogue("dialog_info_shady_line1_1", "dialog_info_shady_line1_2", "dialog_info_shady_line1_3");

	private static readonly LocalizedLines line2 = LocalizedLines.Dialogue("dialog_info_shady_line2_1", "dialog_info_shady_line2_2", "dialog_info_shady_line2_3", "dialog_info_shady_line2_4");

	private static readonly LocalizedLines line3 = LocalizedLines.Dialogue("dialog_info_shady_line3_1", "dialog_info_shady_line3_2", "dialog_info_shady_line3_3", "dialog_info_shady_line3_4");

	public static bool HasAvailableInformation()
	{
		return GetAvailableInformationIndices().Count > 0;
	}

	private static List<int> GetAvailableInformationIndices()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < informationList.Count; i++)
		{
			if (!StoreStation.Instance.storeEventManager.IsEventOnCooldown(informationEventIds[i]))
			{
				list.Add(i);
			}
		}
		return list;
	}

	public static StoreClient CreateBeggerInformation()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "beggerInformation";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_begger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.INFORMATION_DEALER;
		storeClient.serviceCost = 50;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_begger_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_begger_2"))
			.SetEndAction(delegate
			{
				NegociationUIManager.Instance.OpenUI();
			});
		storeClient.acceptDealDialogue = new Dialogue();
		storeClient.acceptDealDialogueLastLine = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_begger_accept_last")).SetEndAction(delegate
		{
			StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListNormal.CreateNutrifruitHarvest(), 3, IsKnown: true);
		});
		storeClient.acceptDealDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_begger_accept_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_begger_accept_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_begger_accept_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_begger_accept_4"))
			.SetNextDialogue(storeClient.acceptDealDialogueLastLine);
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_begger_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.OnDismiss = delegate
		{
			if (storeClient.isBusinessComplete)
			{
				storeClient.acceptDealDialogueLastLine.ExecuteEndAction();
			}
		};
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateShadyCustomer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "shadyCustomerInformation";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_shady_customer");
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.clientIntent = StoreClient.ClientIntent.INFORMATION_DEALER;
		storeClient.serviceCost = 50;
		storeClient.mainDialogue.SetText(storeClient.displayName, line1[RNG.GetRandomInt(0, line1.Count - 1)]).NextDialogue().SetText(storeClient.displayName, line2[RNG.GetRandomInt(0, line2.Count - 1)])
			.NextDialogue()
			.SetText(storeClient.displayName, line3[RNG.GetRandomInt(0, line3.Count - 1)])
			.SetEndAction(delegate
			{
				storeClient.OnIntroduced();
				NegociationUIManager.Instance.OpenUI();
			});
		storeClient.acceptDealDialogue = new Dialogue();
		storeClient.acceptDealDialogueLastLine = new Dialogue();
		HandleInformationDialog(storeClient);
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_info_shady_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.OnDismiss = delegate
		{
			if (storeClient.isBusinessComplete)
			{
				storeClient.acceptDealDialogueLastLine.ExecuteEndAction();
			}
		};
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static void HandleInformationDialog(StoreClient storeClient)
	{
		List<int> availableInformationIndices = GetAvailableInformationIndices();
		if (availableInformationIndices.Count == 0)
		{
			for (int i = 0; i < informationList.Count; i++)
			{
				availableInformationIndices.Add(i);
			}
		}
		List<int> list = new List<int>();
		foreach (int item in availableInformationIndices)
		{
			list.Add(informationListProba[item]);
		}
		int index = availableInformationIndices[RNG.GetOdd(list)];
		informationList[index](storeClient);
	}
}

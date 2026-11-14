using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreClientListAppraisal
{
	private static readonly LocalizedLines scavengerRequestLines = LocalizedLines.Dialogue("dialog_appraisal_request_1", "dialog_appraisal_request_2", "dialog_appraisal_request_3", "dialog_appraisal_request_4", "dialog_appraisal_request_5");

	private static readonly LocalizedLines scavengerAdditionalLines = LocalizedLines.Dialogue("dialog_appraisal_additional_1", "dialog_appraisal_additional_2", "dialog_appraisal_additional_3", "dialog_appraisal_additional_4", "dialog_appraisal_additional_5");

	public static StoreClient CreateAppraisal1()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavAppraisal";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.APPRAISAL_SERVICE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_request_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_additional_1"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHandRandom());
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHandRandom());
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateAppraisalRandom()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavAppraisalRandom";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.APPRAISAL_SERVICE;
		storeClient.mainDialogue.SetText(storeClient.displayName, scavengerRequestLines[RNG.GetRandomIntExculsive(0, scavengerRequestLines.Count)]).NextDialogue().SetText(storeClient.displayName, scavengerAdditionalLines[RNG.GetRandomIntExculsive(0, scavengerAdditionalLines.Count)])
			.SetEndAction(delegate
			{
				List<AppraisalItemType> pool = AppraisalItemRegistry.GetPool(AppraisalRankHelper.GetRank());
				Func<GameItem> func = ((pool.Count > 0) ? pool[UnityEngine.Random.Range(0, pool.Count)].createRandom : (AppraisalItemRegistry.GetLowestRankType()?.createRandom ?? new Func<GameItem>(PreBuiltItemHelper.CreateHandRandom)));
				int num = UnityEngine.Random.Range(2, 5);
				for (int i = 0; i < num; i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(func());
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreatePromotionOffer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "appraisalBoardPromotion";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_appraiser_board_member");
		storeClient.spriteName = SpriteDict.GetClerkSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		int num = Mathf.Min(3, AppraisalRankHelper.GetRank() + 1);
		int cost = AppraisalRankHelper.GetPromotionCost(num);
		bool flag = AppraisalRankHelper.PromotionUnlocksItems(num);
		Dialogue dialogue = new Dialogue();
		Dialogue dialogue2 = new Dialogue();
		Dialogue dialogue3 = new Dialogue();
		Dialogue dialogue4 = new Dialogue();
		dialogue4.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_promo_fee", cost));
		bool promoted = false;
		Action choiceAction = delegate
		{
			if (!promoted && PlayerStore.Instance.playerCash >= cost)
			{
				promoted = true;
				PlayerStore.Instance.ModCash(-cost);
				AppraisalRankHelper.Promote();
			}
		};
		Dialogue dialogue5 = dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_chem_feedback_choice_accept", cost)).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_promo_accept_1"));
		if (flag)
		{
			dialogue5 = dialogue5.NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_promo_accept_items"));
		}
		dialogue5.NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_promo_accept_2")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		dialogue2.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_landlord_vending_player_decline")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_promo_later"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		dialogue3.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_appraisal_promo_choice_never")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_promo_never_1"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_appraisal_promo_never_2"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		Dialogue dialogue6 = storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_board2_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_promo_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_promo_3", AppraisalRankHelper.GetRankName(num)))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_promo_4"));
		if (flag)
		{
			dialogue6 = dialogue6.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_appraisal_promo_items"));
		}
		dialogue6.nextDialogue = dialogue4;
		dialogue4.AddChoice(DirectoryMaster.Item("$"), "", "", dialogue, choiceAction, () => PlayerStore.Instance.playerCash >= cost).AddChoice(DirectoryMaster.Item("$"), "", "", dialogue2, AppraisalRankHelper.DeclinePromotionForNow).AddChoice(DirectoryMaster.Item("$"), "", "", dialogue3, AppraisalRankHelper.DeclinePromotionForGood);
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

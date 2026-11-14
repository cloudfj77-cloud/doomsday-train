using System.Collections.Generic;

public class StoreClientListTierSubstance
{
	private static readonly LocalizedLines desperateAddictGreetingLines = LocalizedLines.Dialogue("dialog_desperate_addict_greeting_1", "dialog_desperate_addict_greeting_2", "dialog_desperate_addict_greeting_3");

	private static readonly LocalizedLines desperateAddictRequestLines = LocalizedLines.Dialogue("dialog_desperate_addict_request_1", "dialog_desperate_addict_request_2", "dialog_desperate_addict_request_3");

	private static List<string> desperateAddictBudgetKeys = new List<string> { "dialog_desperate_addict_budget_1", "dialog_desperate_addict_budget_2", "dialog_desperate_addict_budget_3" };

	private static readonly LocalizedLines wornOutSpacerGreetingLines = LocalizedLines.Dialogue("dialog_worn_spacer_greeting_1", "dialog_worn_spacer_greeting_2", "dialog_worn_spacer_greeting_3");

	private static readonly LocalizedLines wornOutSpacerRequestLines = LocalizedLines.Dialogue("dialog_worn_spacer_request_1", "dialog_worn_spacer_request_2", "dialog_worn_spacer_request_3");

	private static List<string> wornOutSpacerBudgetKeys = new List<string> { "dialog_worn_spacer_budget_1", "dialog_worn_spacer_budget_2", "dialog_worn_spacer_budget_3" };

	private static readonly LocalizedLines upperLevelHedonistGreetingLines = LocalizedLines.Dialogue("dialog_upper_hedonist_greeting_1", "dialog_upper_hedonist_greeting_2", "dialog_upper_hedonist_greeting_3");

	private static readonly LocalizedLines upperLevelHedonistRequestLines = LocalizedLines.Dialogue("dialog_upper_hedonist_request_1", "dialog_upper_hedonist_request_2", "dialog_upper_hedonist_request_3");

	private static List<string> upperLevelHedonistBudgetKeys = new List<string> { "dialog_upper_hedonist_budget_1", "dialog_upper_hedonist_budget_2", "dialog_upper_hedonist_budget_3" };

	public static StoreClient CreateDesperateAddict()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "desperateAddict";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_desperate_addict");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.AcceptingContrabandOverride = true;
		storeClient.SetBudget(100 + RNG.GetRandomInt(0, 25));
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, desperateAddictGreetingLines[RNG.GetRandomIntExculsive(0, desperateAddictGreetingLines.Count)]).NextDialogue().SetText(storeClient.displayName, desperateAddictRequestLines[RNG.GetRandomIntExculsive(0, desperateAddictRequestLines.Count)])
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue(RNG.GetRandomElement(desperateAddictBudgetKeys), storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.willAcceptItem = (GameItem targetItem) => ChemicalProductHelper.HasChemicalPurity(targetItem) ? ChemicalProductHelper.WillAcceptNarcotic(targetItem) : targetItem.IsTag("SUBSTANCE");
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			Dialogue dialogue = new Dialogue();
			if (!storeClient.willAcceptItem(targetItem))
			{
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_desperate_addict_wrong_item"));
			}
			else
			{
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_desperate_addict_right_item"));
			}
			DialogUIManager.Instance.StartDialogue(dialogue);
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWornOutSpacer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "wornOutSpacer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_worn_out_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(145 + RNG.GetRandomInt(0, 35) + StoreStation.GetDayCounter() / 4);
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, wornOutSpacerGreetingLines[RNG.GetRandomIntExculsive(0, wornOutSpacerGreetingLines.Count)]).NextDialogue().SetText(storeClient.displayName, wornOutSpacerRequestLines[RNG.GetRandomIntExculsive(0, wornOutSpacerRequestLines.Count)])
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue(RNG.GetRandomElement(wornOutSpacerBudgetKeys), storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.willAcceptItem = (GameItem targetItem) => ChemicalProductHelper.HasChemicalPurity(targetItem) ? ChemicalProductHelper.WillAcceptNarcotic(targetItem, 2) : targetItem.IsTag("SUBSTANCE");
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			Dialogue dialogue = new Dialogue();
			if (!storeClient.willAcceptItem(targetItem))
			{
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_worn_spacer_wrong_item"));
			}
			else
			{
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_worn_spacer_right_item"));
			}
			DialogUIManager.Instance.StartDialogue(dialogue);
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateUpperLevelHedonist()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "upperLevelHedonist";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_level_hedonist");
		storeClient.spriteName = SpriteDict.GetRandomUpperSpriteName();
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.SetBudget(350 + RNG.GetRandomInt(0, 85) + StoreStation.GetDayCounter());
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, upperLevelHedonistGreetingLines[RNG.GetRandomIntExculsive(0, upperLevelHedonistGreetingLines.Count)]).NextDialogue().SetText(storeClient.displayName, upperLevelHedonistRequestLines[RNG.GetRandomIntExculsive(0, upperLevelHedonistRequestLines.Count)])
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue(RNG.GetRandomElement(upperLevelHedonistBudgetKeys), storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.willAcceptItem = (GameItem targetItem) => ChemicalProductHelper.HasChemicalPurity(targetItem) ? ChemicalProductHelper.WillAcceptNarcotic(targetItem, 2) : targetItem.IsTag("SUBSTANCE");
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			Dialogue dialogue = new Dialogue();
			if (!storeClient.willAcceptItem(targetItem))
			{
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_hedonist_wrong_item"));
			}
			else
			{
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_hedonist_right_item"));
			}
			DialogUIManager.Instance.StartDialogue(dialogue);
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

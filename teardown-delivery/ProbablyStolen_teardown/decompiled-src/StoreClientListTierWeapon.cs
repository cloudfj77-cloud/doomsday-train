using System;
using System.Collections.Generic;

public class StoreClientListTierWeapon
{
	private static Func<StoreClient>[] t1WeaponClient = new Func<StoreClient>[1] { CreateScavenger };

	private static Func<StoreClient>[] t2WeaponClient = new Func<StoreClient>[1] { CreateSelfDefenseEnthusiast };

	private static Func<StoreClient>[] t3WeaponClient = new Func<StoreClient>[1] { CreatePrivateSecurityContractor };

	private static readonly LocalizedLines scavengerGreetingLines = LocalizedLines.Dialogue("dialog_scavenger_greeting_1", "dialog_scavenger_greeting_2", "dialog_scavenger_greeting_3");

	private static readonly LocalizedLines scavengerMidLines = LocalizedLines.Dialogue("dialog_scavenger_mid_1", "dialog_scavenger_mid_2", "dialog_scavenger_mid_3");

	private static readonly LocalizedLines scavengerRequestLines = LocalizedLines.Dialogue("dialog_scavenger_request_1", "dialog_scavenger_request_2", "dialog_scavenger_request_3");

	private static List<string> scavengerBudgetKeys = new List<string> { "dialog_scavenger_budget_1", "dialog_scavenger_budget_2", "dialog_scavenger_budget_3" };

	private static readonly LocalizedLines selfDefenseGreetingLines = LocalizedLines.Dialogue("dialog_selfdefense_greeting_1", "dialog_selfdefense_greeting_2", "dialog_selfdefense_greeting_3");

	private static readonly LocalizedLines selfDefenseRequestLines = LocalizedLines.Dialogue("dialog_selfdefense_request_1", "dialog_selfdefense_request_2", "dialog_selfdefense_request_3");

	private static List<string> selfDefenseBudgetKeys = new List<string> { "dialog_selfdefense_budget_1", "dialog_selfdefense_budget_2", "dialog_selfdefense_budget_3" };

	private static readonly LocalizedLines privateSecurityGreetingLines = LocalizedLines.Dialogue("dialog_privatesec_greeting_1", "dialog_privatesec_greeting_2", "dialog_privatesec_greeting_3");

	private static readonly LocalizedLines privateSecurityRequestLines = LocalizedLines.Dialogue("dialog_privatesec_request_1", "dialog_privatesec_request_2", "dialog_privatesec_request_3");

	private static List<string> privateSecurityBudgetKeys = new List<string> { "dialog_privatesec_budget_1", "dialog_privatesec_budget_2", "dialog_privatesec_budget_3" };

	public static StoreClient GetRandomT1WeaponClient()
	{
		int randomIntExculsive = RNG.GetRandomIntExculsive(0, t1WeaponClient.Length);
		return t1WeaponClient[randomIntExculsive]();
	}

	public static StoreClient GetRandomT2WeaponClient()
	{
		int randomIntExculsive = RNG.GetRandomIntExculsive(0, t2WeaponClient.Length);
		return t2WeaponClient[randomIntExculsive]();
	}

	public static StoreClient GetRandomT3WeaponClient()
	{
		int randomIntExculsive = RNG.GetRandomIntExculsive(0, t3WeaponClient.Length);
		return t3WeaponClient[randomIntExculsive]();
	}

	public static StoreClient CreateScavenger()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavenger";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.SetBudget(80 + RNG.GetRandomInt(0, 40));
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, scavengerGreetingLines[RNG.GetRandomIntExculsive(0, scavengerGreetingLines.Count)]).NextDialogue().SetText(storeClient.displayName, scavengerMidLines[RNG.GetRandomIntExculsive(0, scavengerMidLines.Count)])
			.NextDialogue()
			.SetText(storeClient.displayName, scavengerRequestLines[RNG.GetRandomIntExculsive(0, scavengerRequestLines.Count)])
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue(RNG.GetRandomElement(scavengerBudgetKeys), storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.willAcceptItem = (GameItem targetItem) => targetItem.IsTag("WEAPON") || targetItem.IsTag("ARMOR") || targetItem.IsTag("AMMUNITION");
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("ARMOR");
		storeClient.clientBuyingTagList.Add("AMMUNITION");
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			Dialogue dialogue = new Dialogue();
			if (!storeClient.willAcceptItem(targetItem))
			{
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scavenger_wrong_item"));
			}
			else
			{
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scavenger_right_item"));
			}
			DialogUIManager.Instance.StartDialogue(dialogue);
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSelfDefenseEnthusiast()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "selfDefenseEnthusiast";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_self_defense_enthusiast");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(145 + RNG.GetRandomInt(0, 50) + StoreStation.GetDayCounter() / 4);
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("AMMUNITION");
		storeClient.clientBuyingTagList.Add("ARMOR");
		storeClient.mainDialogue.SetText(storeClient.displayName, selfDefenseGreetingLines[RNG.GetRandomIntExculsive(0, selfDefenseGreetingLines.Count)]).NextDialogue().SetText(storeClient.displayName, selfDefenseRequestLines[RNG.GetRandomIntExculsive(0, selfDefenseRequestLines.Count)])
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue(RNG.GetRandomElement(selfDefenseBudgetKeys), storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.willAcceptItem = (GameItem targetItem) => targetItem.IsTag("WEAPON") || targetItem.IsTag("AMMUNITION") || targetItem.IsTag("ARMOR");
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			Dialogue dialogue = new Dialogue();
			if (!storeClient.willAcceptItem(targetItem))
			{
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_selfdefense_wrong_item"));
			}
			else
			{
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_selfdefense_right_item"));
			}
			DialogUIManager.Instance.StartDialogue(dialogue);
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreatePrivateSecurityContractor()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "privateSecurityContractor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_private_security_contractor");
		storeClient.spriteName = SpriteDict.GetRandomMercSpriteName();
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.SetBudget(500 + RNG.GetRandomInt(0, 200) + StoreStation.GetDayCounter());
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, privateSecurityGreetingLines[RNG.GetRandomIntExculsive(0, privateSecurityGreetingLines.Count)]).NextDialogue().SetText(storeClient.displayName, privateSecurityRequestLines[RNG.GetRandomIntExculsive(0, privateSecurityRequestLines.Count)])
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue(RNG.GetRandomElement(privateSecurityBudgetKeys), storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.willAcceptItem = delegate(GameItem targetItem)
		{
			if (targetItem.IsGameItemType("MAKESHIFT") || targetItem.IsGameItemType("HOUSEHOLD_GOOD"))
			{
				return false;
			}
			return targetItem.IsGameItemType("WEAPON") || targetItem.IsGameItemType("AMMUNITION") || targetItem.IsGameItemType("ARMOR");
		};
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			Dialogue dialogue = new Dialogue();
			if (!storeClient.willAcceptItem(targetItem))
			{
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_privatesec_wrong_item"));
			}
			else
			{
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_privatesec_right_item"));
			}
			DialogUIManager.Instance.StartDialogue(dialogue);
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}

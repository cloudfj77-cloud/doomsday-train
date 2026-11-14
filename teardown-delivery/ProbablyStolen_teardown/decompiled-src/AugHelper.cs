using System;
using System.Collections.Generic;
using UnityEngine;

public class AugHelper
{
	public const int SHOT_AUG = 0;

	public const int SHOT_INNOCENT = 1;

	public const int SHOT_WANTED_CAPTURE = 2;

	private const int AUG_TIER_RAMP_HOLD_DAYS = 7;

	private static readonly LocalizedLines unholsterReactions = LocalizedLines.Dialogue("aug_dialog_unholster_0", "aug_dialog_unholster_1", "aug_dialog_unholster_2", "aug_dialog_unholster_3", "aug_dialog_unholster_4");

	private static readonly LocalizedLines unholsterReactions2 = LocalizedLines.Dialogue("aug_dialog_unholster2_0", "aug_dialog_unholster2_1", "aug_dialog_unholster2_2");

	private static readonly LocalizedLines unholsterReactionsObvious = LocalizedLines.Dialogue("aug_dialog_unholster_ob_0", "aug_dialog_unholster_ob_1", "aug_dialog_unholster_ob_2");

	private static readonly LocalizedLines unholsterReactionsObvious2 = LocalizedLines.Dialogue("aug_dialog_unholster_ob2_0", "aug_dialog_unholster_ob2_1", "aug_dialog_unholster_ob2_2");

	public static StoreClientInstance GetShotClient()
	{
		return StoreUIManager.Instance.shotClientInstance ?? PlayerStore.Instance.currentClientInstance;
	}

	public static int GetShootResult()
	{
		StoreClientInstance shotClient = GetShotClient();
		if (shotClient == null)
		{
			Debug.LogError("[AugHelper] GetShootResult: no shot client available, defaulting to good kill.");
			return 0;
		}
		StoreClient clientBlueprint = shotClient.GetClientBlueprint();
		if (clientBlueprint.isAug)
		{
			return 0;
		}
		if (clientBlueprint.shotCountsAsCapture && clientBlueprint.isWanted)
		{
			return 2;
		}
		return 1;
	}

	public static void OnShootoutLeave()
	{
		StoreUIManager.Instance.StartBlackscreenSequence(playFullSequence: true, delegate
		{
			StoreUIManager.Instance.body.SetActive(value: false);
		}, delegate
		{
			StoreUIManager.Instance.OnArrestDialoguePlayed();
		}, 0.25f, 0.1f);
	}

	public static void OnGoodKill()
	{
		int num = StoreStation.Instance.dayCounter - PlayerStore.GetAugIntroducedDay();
		int amount = Mathf.Clamp(100 + num, 100, 250);
		amount = StartingPerkContent.HandleReportRewardConvict(amount);
		amount = StartingPerkContent.HandleReportRewardModelCitizen(amount);
		if (StartingPerk.IsAugMenace())
		{
			amount = RNG.MultiplyAndRound(amount, 1.25);
		}
		PlayerStore.Instance.ModCash(amount);
		RunStats.OnAugShot();
		int amount2 = StartingPerkContent.HandleReportRewardModelCitizen(StartingPerkContent.HandleReportRewardConvict(2));
		StoreReputation.ModRevReputation(amount2);
		StoreReputation.ModBMReputation(amount2);
		StoreReputation.ModLLReputation(amount2);
		StoreReputation.ModSecReputation(StartingPerkContent.HandleReportRewardModelCitizen(StartingPerkContent.HandleReportRewardConvict(8)));
		PlayerStore.Instance.augPresence -= Difficulty.Rules.augKillPresenceDrop;
		if (PlayerStore.Instance.augPresence < 0)
		{
			PlayerStore.Instance.augPresence = 0;
		}
		GameItem gameItem = DirectoryMaster.Item("sec_slip");
		NegociationUIManager.Instance.audioSourceCashRegister.Play();
		string title = LocHelper.GetLocalizedMechanic("mech_report_title");
		string body = LocHelper.GetLocalizedMechanic("mech_aug_kill_slip", amount);
		gameItem.ModifyTag("title", delegate(TagState state)
		{
			state.Enable().SetString(title);
		});
		gameItem.ModifyTag("body", delegate(TagState state)
		{
			state.Enable().SetString(body);
		});
		PlayerStore.Instance.AddDirectSellingItemToTable(gameItem, isOwend: true);
	}

	public static void CleanupKill()
	{
		StoreClientInstance shotClient = GetShotClient();
		if (shotClient != null)
		{
			PlayerStore.Instance.CloseWantedPoster(shotClient.GetClientBlueprint());
		}
		OnShootoutLeave();
		int amount = 250;
		if (StoreStation.Instance.dayCounter >= 25)
		{
			amount = 500;
		}
		if (StoreStation.Instance.dayCounter >= 50)
		{
			amount = 750;
		}
		PlayerStore.Instance.secData.CommitCrime("MANSLAUGHTER", amount);
		StoreReputation.ModRevReputation(-10);
		StoreReputation.ModBMReputation(-10);
		StoreReputation.ModLLReputation(-15);
		StoreReputation.ModSecReputation(-15);
		StoreClient storeClient = shotClient?.GetClientBlueprint();
		if (storeClient != null && storeClient.onShotDead != null)
		{
			storeClient.onShotDead();
		}
	}

	public static void OnAugLeftAlive()
	{
		Debug.Log("Aug left alive, increasing presence");
		NightLog.AugLeftAlive();
		PlayerStore.Instance.augPresence += Difficulty.Rules.augLeftAlivePresenceRise;
	}

	private static string RandCleanBudget(int budget)
	{
		List<string> list = new List<string>();
		list.Add(LocHelper.GetLocalizedDialogue("aug_dialog_budget_clean_0", budget));
		list.Add(LocHelper.GetLocalizedDialogue("aug_dialog_budget_clean_1", budget));
		list.Add(LocHelper.GetLocalizedDialogue("aug_dialog_budget_clean_2", budget));
		List<string> list2 = list;
		return list2[RNG.GetRandomInt(0, list2.Count - 1)];
	}

	private static string RandSuspiciousBudget(int budget)
	{
		List<string> list = new List<string>();
		list.Add(LocHelper.GetLocalizedDialogue("aug_dialog_budget_sus_0", budget));
		list.Add(LocHelper.GetLocalizedDialogue("aug_dialog_budget_sus_1", budget));
		list.Add(LocHelper.GetLocalizedDialogue("aug_dialog_budget_sus_2", budget));
		List<string> list2 = list;
		return list2[RNG.GetRandomInt(0, list2.Count - 1)];
	}

	private static Dialogue BuildTieredAugDialogue(int tier, bool showBudget, int budget, List<string[]> tier1, List<string[]> tier2WithHint, List<string[]> tier2Clean, List<string[]> tier3TwoHints, List<string[]> tier3OneHint, List<string[]> tier4)
	{
		string title = "CLIENT";
		Dialogue dialogue = new Dialogue();
		if (tier <= 1)
		{
			string[] array = tier1[RNG.GetRandomInt(0, tier1.Count - 1)];
			Dialogue dialogue2 = dialogue.SetText(title, array[0]).NextDialogue().SetText(title, array[1]);
			if (showBudget)
			{
				dialogue2.NextDialogue().SetText(title, RandCleanBudget(budget));
			}
		}
		else
		{
			switch (tier)
			{
			case 2:
			{
				if (showBudget && RNG.Roll(50))
				{
					string[] array5 = tier2Clean[RNG.GetRandomInt(0, tier2Clean.Count - 1)];
					dialogue.SetText(title, array5[0]).NextDialogue().SetText(title, array5[1])
						.NextDialogue()
						.SetText(title, RandSuspiciousBudget(budget));
					break;
				}
				string[] array6 = tier2WithHint[RNG.GetRandomInt(0, tier2WithHint.Count - 1)];
				Dialogue dialogue4 = dialogue.SetText(title, array6[0]).NextDialogue().SetText(title, array6[1]);
				if (showBudget)
				{
					dialogue4.NextDialogue().SetText(title, RandCleanBudget(budget));
				}
				break;
			}
			case 3:
			{
				if (showBudget && RNG.Roll(50))
				{
					string[] array3 = tier3OneHint[RNG.GetRandomInt(0, tier3OneHint.Count - 1)];
					dialogue.SetText(title, array3[0]).NextDialogue().SetText(title, array3[1])
						.NextDialogue()
						.SetText(title, RandSuspiciousBudget(budget));
					break;
				}
				string[] array4 = tier3TwoHints[RNG.GetRandomInt(0, tier3TwoHints.Count - 1)];
				Dialogue dialogue3 = dialogue.SetText(title, array4[0]).NextDialogue().SetText(title, array4[1]);
				if (showBudget)
				{
					dialogue3.NextDialogue().SetText(title, RandCleanBudget(budget));
				}
				break;
			}
			default:
			{
				string[] array2 = tier4[RNG.GetRandomInt(0, tier4.Count - 1)];
				dialogue.SetText(title, array2[0]).NextDialogue().SetText(title, array2[1])
					.NextDialogue()
					.SetText(title, array2[2]);
				break;
			}
			}
		}
		return dialogue;
	}

	private static Dialogue CreateRandomAugBuyDialogue(string itemKey, int budget, int tier, List<string[]> tier2TypeHints, List<string[]> tier3TwoHints, List<string[]> tier3TypeOneHint, List<string[]> tier4)
	{
		string localizedDialogue = LocHelper.GetLocalizedDialogue(itemKey);
		List<string[]> list = new List<string[]>();
		list.Add(new string[2]
		{
			LocHelper.GetLocalizedDialogue("aug_dialog_tier1_0_a", localizedDialogue),
			LocHelper.GetLocalizedDialogue("aug_dialog_tier1_0_b")
		});
		list.Add(new string[2]
		{
			LocHelper.GetLocalizedDialogue("aug_dialog_tier1_1_a", localizedDialogue),
			LocHelper.GetLocalizedDialogue("aug_dialog_tier1_1_b")
		});
		List<string[]> tier5 = list;
		list = new List<string[]>();
		list.Add(new string[2]
		{
			LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_a", localizedDialogue),
			LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_b")
		});
		list.Add(new string[2]
		{
			LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_1_a", localizedDialogue),
			LocHelper.GetLocalizedDialogue("aug_dialog_tier1_1_b")
		});
		List<string[]> tier2Clean = list;
		list = new List<string[]>(tier2TypeHints);
		list.Add(new string[2]
		{
			LocHelper.GetLocalizedDialogue("aug_dialog_tier2_hint_req_a", localizedDialogue),
			LocHelper.GetLocalizedDialogue("aug_dialog_tier2_hint_req_b")
		});
		list.Add(new string[2]
		{
			LocHelper.GetLocalizedDialogue("aug_dialog_tier2_hint_unit_a", localizedDialogue),
			LocHelper.GetLocalizedDialogue("aug_dialog_tier2_hint_unit_b")
		});
		List<string[]> tier2WithHint = list;
		list = new List<string[]>(tier3TypeOneHint);
		list.Add(new string[2]
		{
			LocHelper.GetLocalizedDialogue("aug_dialog_tier3_req_a", localizedDialogue),
			LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_b")
		});
		List<string[]> tier3OneHint = list;
		return BuildTieredAugDialogue(tier, RNG.Roll(50), budget, tier5, tier2WithHint, tier2Clean, tier3TwoHints, tier3OneHint, tier4);
	}

	public static Dialogue CreateRandomMedAugDialogue(int budget, int tier)
	{
		string localizedDialogue = LocHelper.GetLocalizedDialogue("aug_dialog_item_medical");
		return CreateRandomAugBuyDialogue("aug_dialog_item_medical", budget, tier, new List<string[]>
		{
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_hint_unit_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t2_0_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t2_1_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t2_2_b")
			}
		}, new List<string[]>
		{
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t3_0_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t3_0_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t3_1_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t3_1_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t3_2_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t3_2_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t3_3_b")
			}
		}, new List<string[]>
		{
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_hint_unit_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t3o_0_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t2_1_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t3o_2_b")
			}
		}, new List<string[]>
		{
			new string[3]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t4_0_a"),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t4_0_b"),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t4_0_c", budget)
			},
			new string[3]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t4_1_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t4_1_b"),
				LocHelper.GetLocalizedDialogue("aug_dialog_med_t4_1_c", budget)
			}
		});
	}

	public static Dialogue CreateRandomMaterialAugDialogue(int budget, int tier)
	{
		string localizedDialogue = LocHelper.GetLocalizedDialogue("aug_dialog_item_materials");
		return CreateRandomAugBuyDialogue("aug_dialog_item_materials", budget, tier, new List<string[]>
		{
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_hint_unit_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t2_0_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t2_1_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t2_2_b")
			}
		}, new List<string[]>
		{
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t3_0_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t3_1_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t3_1_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t3_2_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t3_2_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t3_3_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t3_3_b")
			}
		}, new List<string[]>
		{
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_hint_unit_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t3o_0_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t2_1_b")
			},
			new string[2]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_tier2_clean_0_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t3o_2_b")
			}
		}, new List<string[]>
		{
			new string[3]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t4_0_a", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t4_0_b"),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t4_0_c", budget)
			},
			new string[3]
			{
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t4_1_a"),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t4_1_b", localizedDialogue),
				LocHelper.GetLocalizedDialogue("aug_dialog_mat_t4_1_c", budget)
			}
		});
	}

	public static int RollAugTier()
	{
		float t = Mathf.Clamp01((float)(((StoreStation.Instance != null) ? StoreStation.Instance.dayCounter : 0) - PlayerStore.GetAugIntroducedDay() - 7) / 100f);
		float num = Mathf.Lerp(5f, 75f, t);
		float num2 = Mathf.Lerp(15f, 25f, t);
		float num3 = Mathf.Lerp(40f, 0f, t);
		float num4 = RNG.GetRandomInt(0, 99);
		if (num4 < num)
		{
			return 1;
		}
		if (num4 < num + num2)
		{
			return 2;
		}
		if (num4 < num + num2 + num3)
		{
			return 3;
		}
		return 4;
	}

	private static bool IsInTierHoldWindow()
	{
		return ((StoreStation.Instance != null) ? StoreStation.Instance.dayCounter : 0) - PlayerStore.GetAugIntroducedDay() <= 7;
	}

	public static void SetRandomAugFaction(StoreClient storeClient, int augTier)
	{
		List<string> list = new List<string> { "FACTION_UPPER_LEVEL", "FACTION_BLACK_MARKET", "FACTION_LOWER_LEVEL" };
		string text = list[RNG.GetRandomIntExculsive(0, list.Count)];
		storeClient.displayName = LocHelper.GetLocalizedClientName(FactionNameKey(text));
		bool flag = augTier >= 2 && RNG.Roll(20);
		bool flag2 = RNG.Roll(Mathf.Clamp(45 - augTier * 5, 25, 40));
		if (augTier <= 2 && !flag && !flag2 && IsInTierHoldWindow())
		{
			flag2 = true;
		}
		if (flag2)
		{
			string faction = (flag ? RandomOtherCoreFaction(text) : text);
			storeClient.spriteName = GetRandomAugSprite(faction);
		}
		else if (flag)
		{
			storeClient.spriteName = RandomMismatchSprite(text);
		}
		else
		{
			storeClient.spriteName = GetRandomFactionSprite(text);
		}
	}

	public static void MakeDisguisedAug(StoreClient storeClient, int augTier)
	{
		storeClient.isAug = true;
		storeClient.augTier = augTier;
		int randomInt = RNG.GetRandomInt(0, 99);
		StoreClient.AugIdleTell augIdleTell;
		int num;
		if (randomInt < 35)
		{
			augIdleTell = StoreClient.AugIdleTell.Freeze;
			num = 1;
		}
		else if (randomInt < 55)
		{
			augIdleTell = StoreClient.AugIdleTell.Normal;
			num = 2;
		}
		else if (randomInt < 75)
		{
			augIdleTell = StoreClient.AugIdleTell.Seizure;
			num = 0;
		}
		else if (randomInt < 85)
		{
			augIdleTell = StoreClient.AugIdleTell.Freeze;
			num = 2;
		}
		else if (randomInt < 95)
		{
			augIdleTell = StoreClient.AugIdleTell.Seizure;
			num = 1;
		}
		else
		{
			augIdleTell = StoreClient.AugIdleTell.Seizure;
			num = 2;
		}
		storeClient.augIdleTell = augIdleTell;
		switch (num)
		{
		case 1:
			storeClient.spriteName = RandomMismatchSprite(storeClient.clientFaction);
			break;
		case 2:
			storeClient.spriteName = GetRandomAugSprite(storeClient.clientFaction);
			break;
		}
	}

	private static string FactionNameKey(string faction)
	{
		if (faction.Equals("FACTION_UPPER_LEVEL"))
		{
			return "name_upper_level_citizen";
		}
		if (faction.Equals("FACTION_LOWER_LEVEL"))
		{
			return "name_lower_levels_citizen";
		}
		return "name_thief";
	}

	private static string GetRandomFactionSprite(string faction)
	{
		if (faction.Equals("FACTION_UPPER_LEVEL"))
		{
			return SpriteDict.GetRandomUpperSpriteName();
		}
		if (faction.Equals("FACTION_LOWER_LEVEL"))
		{
			return SpriteDict.GetRandomLowerLevelSpriteName();
		}
		return SpriteDict.GetRandomShadySpriteName();
	}

	private static string GetRandomAugSprite(string faction)
	{
		if (faction.Equals("FACTION_UPPER_LEVEL"))
		{
			return SpriteDict.GetRandomAugUpperSpriteName();
		}
		if (faction.Equals("FACTION_BLACK_MARKET"))
		{
			return SpriteDict.GetRandomAugBMSpriteName();
		}
		return SpriteDict.GetRandomAugLowerSpriteName();
	}

	private static string RandomOtherCoreFaction(string faction)
	{
		List<string> list = new List<string> { "FACTION_UPPER_LEVEL", "FACTION_BLACK_MARKET", "FACTION_LOWER_LEVEL" };
		list.Remove(faction);
		return list[RNG.GetRandomIntExculsive(0, list.Count)];
	}

	private static string RandomMismatchSprite(string ownFaction)
	{
		List<Func<string>> list = new List<Func<string>>
		{
			SpriteDict.GetRandomUpperSpriteName,
			SpriteDict.GetRandomLowerLevelSpriteName,
			SpriteDict.GetRandomShadySpriteName,
			SpriteDict.GetRandomScavSpriteName,
			SpriteDict.GetRandomFarmerSpriteName,
			SpriteDict.GetRandomChefSpriteName,
			SpriteDict.GetRandomMedSpriteName,
			SpriteDict.GetRandomEngSpriteName,
			SpriteDict.GetRandomMinerSpriteName,
			SpriteDict.GetRandomMercSpriteName,
			SpriteDict.GetRandomSciSpriteName
		};
		if (ownFaction != null)
		{
			if (ownFaction.Equals("FACTION_UPPER_LEVEL"))
			{
				list.Remove(SpriteDict.GetRandomUpperSpriteName);
			}
			else if (ownFaction.Equals("FACTION_LOWER_LEVEL"))
			{
				list.Remove(SpriteDict.GetRandomLowerLevelSpriteName);
			}
			else if (ownFaction.Equals("FACTION_BLACK_MARKET"))
			{
				list.Remove(SpriteDict.GetRandomShadySpriteName);
			}
		}
		return list[RNG.GetRandomIntExculsive(0, list.Count)]();
	}

	public static Dialogue GetClientReactionUnholster()
	{
		StoreClient storeClient = PlayerStore.Instance.currentClientInstance?.GetClientBlueprint();
		if (storeClient != null && storeClient.unholsterDialogue != null)
		{
			Dialogue dialogue = storeClient.unholsterDialogue();
			if (dialogue != null)
			{
				return dialogue;
			}
		}
		return GetRandomClientReactionUnholster();
	}

	public static Dialogue GetRandomClientReactionUnholster()
	{
		Dialogue dialogue = new Dialogue();
		string title = "CLIENT";
		if ((PlayerStore.Instance.currentClientInstance.GetClientBlueprint()?.augTier ?? 0) == 4)
		{
			dialogue.SetText(title, unholsterReactionsObvious[RNG.GetRandomInt(0, unholsterReactionsObvious.Count - 1)]).NextDialogue().SetText(title, unholsterReactionsObvious2[RNG.GetRandomInt(0, unholsterReactionsObvious2.Count - 1)]);
			return dialogue;
		}
		dialogue.SetText(title, unholsterReactions[RNG.GetRandomInt(0, unholsterReactions.Count - 1)]).NextDialogue().SetText(title, unholsterReactions2[RNG.GetRandomInt(0, unholsterReactions2.Count - 1)]);
		return dialogue;
	}
}

public class InsStampHelper
{
	public const string FOOD_STAMP_TAG = "FOOD_STAMP_TAG";

	public const string STAMP_SEAL = "STAMP_SEAL";

	public const string STAMP_LOGO = "STAMP_LOGO";

	public const string STAMP_CHIP = "STAMP_CHIP";

	public const string STAMP_LETTERING = "STAMP_LETTERING";

	public const string STAMP_AMOUNT = "STAMP_AMOUNT";

	public const string STAMP_SEAL_ISSUE = "STAMP_SEAL_ISSUE";

	public const string STAMP_LOGO_ISSUE = "STAMP_LOGO_ISSUE";

	public const string STAMP_CHIP_ISSUE = "STAMP_CHIP_ISSUE";

	public const string STAMP_LETTERING_ISSUE = "STAMP_LETTERING_ISSUE";

	public const string STAMP_AMOUNT_ISSUE = "STAMP_AMOUNT_ISSUE";

	public const int STAMP_SEAL_COUNT = 2;

	public const int STAMP_LOGO_COUNT = 6;

	public const int STAMP_CHIP_COUNT = 4;

	public const int STAMP_LETTERING_COUNT = 9;

	public static void Init(GameItem gameItem)
	{
		gameItem.EnableTag("FOOD_STAMP_TAG");
		InspectableHelper.InitInspectableItem(gameItem);
	}

	public static void SetGenuine(GameItem gameItem, int amount)
	{
		gameItem.ModifyTag("STAMP_LOGO", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("STAMP_CHIP", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("STAMP_LETTERING", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (amount >= 160)
		{
			gameItem.ModifyTag("STAMP_SEAL", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
		else
		{
			gameItem.ModifyTag("STAMP_SEAL", delegate(TagState state)
			{
				state.Enable().SetInt(-1);
			});
		}
		gameItem.ModifyTag("STAMP_AMOUNT", delegate(TagState state)
		{
			state.Enable().SetInt(amount);
		});
		gameItem.name = LocHelper.GetLocalizedMechanic("ins_stamp_name", amount);
		gameItem.unitValue = (long)(0.75 * (double)amount);
	}

	public static void SetCounterfeit(GameItem gameItem, int amount)
	{
		int chance = 10;
		int chance2 = 20;
		int chance3 = 20;
		int chance4 = 12;
		int chance5 = 20;
		gameItem.name = LocHelper.GetLocalizedMechanic("ins_stamp_name", amount);
		gameItem.unitValue = (long)(0.75 * (double)amount);
		gameItem.ModifyTag("STAMP_AMOUNT", delegate(TagState state)
		{
			state.Enable().SetInt(amount);
		});
		bool flag = false;
		int newAmount = amount;
		while (!flag)
		{
			if (RNG.Roll(chance2))
			{
				flag = true;
				gameItem.EnableTag("STAMP_LOGO_ISSUE");
				int logoRoll = RNG.GetRandomIntExculsive(0, 6);
				if (logoRoll == 0)
				{
					logoRoll = -1;
				}
				gameItem.ModifyTag("STAMP_LOGO", delegate(TagState state)
				{
					state.Enable().SetInt(logoRoll);
				});
			}
			if (RNG.Roll(chance5))
			{
				flag = true;
				gameItem.EnableTag("STAMP_CHIP_ISSUE");
				int chipRoll = RNG.GetRandomIntExculsive(0, 4);
				if (chipRoll == 0)
				{
					chipRoll = -1;
				}
				gameItem.ModifyTag("STAMP_CHIP", delegate(TagState state)
				{
					state.Enable().SetInt(chipRoll);
				});
			}
			if (RNG.Roll(chance3))
			{
				flag = true;
				gameItem.EnableTag("STAMP_LETTERING_ISSUE");
				int letterRoll = RNG.GetRandomIntExculsive(0, 9);
				if (letterRoll == 0)
				{
					letterRoll = -1;
				}
				gameItem.ModifyTag("STAMP_LETTERING", delegate(TagState state)
				{
					state.Enable().SetInt(letterRoll);
				});
			}
			if (RNG.Roll(chance4))
			{
				newAmount = amount + 10;
				flag = true;
				gameItem.EnableTag("STAMP_AMOUNT_ISSUE");
				gameItem.ModifyTag("STAMP_AMOUNT", delegate(TagState state)
				{
					state.Enable().SetInt(newAmount);
				});
				gameItem.name = LocHelper.GetLocalizedMechanic("ins_stamp_name", newAmount);
				gameItem.unitValue = (long)(0.75 * (double)newAmount);
			}
			if (newAmount >= 160)
			{
				gameItem.ModifyTag("STAMP_SEAL", delegate(TagState state)
				{
					state.Enable().SetInt(0);
				});
			}
			else
			{
				gameItem.ModifyTag("STAMP_SEAL", delegate(TagState state)
				{
					state.Enable().SetInt(-1);
				});
			}
			if (!RNG.Roll(chance))
			{
				continue;
			}
			flag = true;
			gameItem.EnableTag("STAMP_SEAL_ISSUE");
			if (newAmount >= 160)
			{
				gameItem.ModifyTag("STAMP_SEAL", delegate(TagState state)
				{
					state.Enable().SetInt(-1);
				});
			}
			else
			{
				gameItem.ModifyTag("STAMP_SEAL", delegate(TagState state)
				{
					state.Enable().SetInt(0);
				});
			}
		}
	}

	public static Dialogue CreateTalkDialogue(GameItem gameItem)
	{
		Dialogue dialogue = new Dialogue();
		if (gameItem.FindItemFeatureByCategory("CATEGORY_GENUINE_STAMP").realCondition.identifier.Equals("genuineStamp"))
		{
			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("ins_cig_talk_genuine")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_talk_thanks"))
				.SetEndAction(delegate
				{
					gameItem.FindItemFeatureByCategory("CATEGORY_GENUINE_STAMP").ExposeFeature();
					StoreReputation.ModCurrentClientReputation(2);
					InspectableHelper.OnConfrontSuccess(gameItem);
				});
		}
		else
		{
			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("ins_cig_talk_genuine")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_talk_genuine_deny"));
		}
		return dialogue;
	}

	public static Dialogue CreateConfrontDialogue(string reason, GameItem gameItem)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 6;
		if (reason.Equals("STAMP_LETTERING_ISSUE"))
		{
			LocHelper.GetLocalizedMechanic("ins_cig_issue_lettering");
		}
		if (reason.Equals("STAMP_LOGO_ISSUE"))
		{
			LocHelper.GetLocalizedMechanic("ins_cig_issue_logo");
		}
		if (reason.Equals("STAMP_SEAL_ISSUE"))
		{
			LocHelper.GetLocalizedMechanic("ins_cig_issue_seal");
		}
		if (reason.Equals("STAMP_AMOUNT_ISSUE"))
		{
			LocHelper.GetLocalizedMechanic("ins_stamp_issue_amount");
		}
		if (reason.Equals("STAMP_CHIP_ISSUE"))
		{
			LocHelper.GetLocalizedMechanic("ins_stamp_issue_chip");
		}
		if (InspectableHelper.IsConfrontCorrect(reason, gameItem))
		{
			dialogue.SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_stamp_confront_correct")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_stamp_confront_lower_price"))
				.SetEndAction(delegate
				{
					gameItem.FindItemFeatureByCategory("CATEGORY_GENUINE_STAMP").ExposeFeature();
					InspectableHelper.OnConfrontSuccess(gameItem);
				});
		}
		else
		{
			dialogue.SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_confront_wrong")).SetEndAction(delegate
			{
				InspectionUIManager.Instance.OnConfrontFailed();
			});
		}
		return dialogue;
	}

	public static void CreateDebugTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("FOOD_STAMP_TAG") && PlayerStore.Instance.devMode)
		{
			if (item.IsTag("STAMP_SEAL_ISSUE"))
			{
				builder.AddLine("STAMP_SEAL_ISSUE", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.IsTag("STAMP_LOGO_ISSUE"))
			{
				builder.AddLine("STAMP_LOGO_ISSUE", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.IsTag("STAMP_CHIP_ISSUE"))
			{
				builder.AddLine("STAMP_CHIP_ISSUE", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.IsTag("STAMP_AMOUNT_ISSUE"))
			{
				builder.AddLine("STAMP_AMOUNT_ISSUE", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.IsTag("STAMP_LETTERING_ISSUE"))
			{
				builder.AddLine("STAMP_LETTERING_ISSUE", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
		}
	}
}

using System.Collections.Generic;

public class InsCigaretteHelper
{
	public const string CIGARETTE_TAG = "CIGARETTE_TAG";

	public const string CIGARETTE_SEAL = "CIGARETTE_SEAL";

	public const string CIGARETTE_LOGO = "CIGARETTE_LOGO";

	public const string CIGARETTE_LETTERING = "CIGARETTE_LETTERING";

	public const string CIGARETTE_BOX = "CIGARETTE_BOX";

	public const string CIGARETTE_LETTERING_ISSUE = "CIGARETTE_LETTERING_ISSUE";

	public const string CIGARETTE_COLOR_ISSUE = "CIGARETTE_COLOR_ISSUE";

	public const string CIGARETTE_SEAL_ISSUE = "CIGARETTE_SEAL_ISSUE";

	public const string CIGARETTE_LOGO_ISSUE = "CIGARETTE_LOGO_ISSUE";

	public const string CIGARETTE_BOX_ISSUE = "CIGARETTE_BOX_ISSUE";

	public const int LETTERING_COUNT = 8;

	public const int LOGO_COUNT = 5;

	public const int SEAL_COUNT = 2;

	public const int BOX_COUNT = 1;

	public const int COLOR_COUNT = 4;

	public static void InitCigarette(GameItem gameItem)
	{
		gameItem.EnableTag("CIGARETTE_TAG");
	}

	public static void SetGenuine(GameItem gameItem)
	{
		gameItem.ModifyTag("CIGARETTE_SEAL", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("CIGARETTE_LOGO", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("CIGARETTE_LETTERING", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("CIGARETTE_BOX", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
	}

	public static void SetConterfeit(GameItem gameItem)
	{
		int chance = 10;
		int chance2 = 20;
		int chance3 = 30;
		int chance4 = 20;
		bool flag = false;
		gameItem.ModifyTag("CIGARETTE_SEAL", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("CIGARETTE_LOGO", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("CIGARETTE_LETTERING", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("CIGARETTE_BOX", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		RNG.GetRandomIntExculsive(0, 100);
		while (!flag)
		{
			if (RNG.Roll(chance))
			{
				flag = true;
				gameItem.EnableTag("CIGARETTE_SEAL_ISSUE");
				if (RNG.Roll(50))
				{
					gameItem.ModifyTag("CIGARETTE_SEAL", delegate(TagState state)
					{
						state.Enable().SetInt(1);
					});
				}
				else
				{
					gameItem.ModifyTag("CIGARETTE_SEAL", delegate(TagState state)
					{
						state.Enable().SetInt(-1);
					});
				}
			}
			if (RNG.Roll(chance2))
			{
				flag = true;
				gameItem.EnableTag("CIGARETTE_LOGO_ISSUE");
				int logoRoll = RNG.GetRandomIntExculsive(0, 5);
				if (logoRoll == 0)
				{
					logoRoll = -1;
				}
				gameItem.ModifyTag("CIGARETTE_LOGO", delegate(TagState state)
				{
					state.Enable().SetInt(logoRoll);
				});
			}
			if (RNG.Roll(chance3))
			{
				flag = true;
				gameItem.EnableTag("CIGARETTE_LETTERING_ISSUE");
				int letteringRoll = RNG.GetRandomIntExculsive(0, 8);
				if (letteringRoll == 0)
				{
					letteringRoll = -1;
				}
				gameItem.ModifyTag("CIGARETTE_LETTERING", delegate(TagState state)
				{
					state.Enable().SetInt(letteringRoll);
				});
			}
			if (RNG.Roll(chance4))
			{
				flag = true;
				gameItem.EnableTag("CIGARETTE_COLOR_ISSUE");
				int colorRoll = RNG.GetRandomInt(1, 4);
				gameItem.ModifyTag("CIGARETTE_BOX", delegate(TagState state)
				{
					state.Enable().SetInt(colorRoll);
				});
			}
		}
	}

	public static void SetConterfeitStory(GameItem gameItem)
	{
		gameItem.ModifyTag("CIGARETTE_SEAL", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		gameItem.EnableTag("CIGARETTE_SEAL_ISSUE");
		gameItem.ModifyTag("CIGARETTE_LOGO", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
		gameItem.EnableTag("CIGARETTE_LOGO_ISSUE");
		gameItem.ModifyTag("CIGARETTE_LETTERING", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("CIGARETTE_BOX", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
	}

	public static void SetConterfeitScreenshop(GameItem gameItem)
	{
		gameItem.ModifyTag("CIGARETTE_SEAL", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("CIGARETTE_LOGO", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("CIGARETTE_LETTERING", delegate(TagState state)
		{
			state.Enable().SetInt(6);
		});
		gameItem.ModifyTag("CIGARETTE_BOX", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
	}

	public static void SetRandom(GameItem gameItem)
	{
		RNG.Roll(25);
	}

	public static Dialogue GetCustomerConfrontDialogue(GameItem gameItem)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 6;
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_not_genuine"));
		List<string> list = new List<string>();
		if (gameItem.IsTag("CIGARETTE_LETTERING_ISSUE"))
		{
			list.Add(LocHelper.GetLocalizedMechanic("ins_cig_issue_lettering"));
		}
		if (gameItem.IsTag("CIGARETTE_COLOR_ISSUE"))
		{
			list.Add(LocHelper.GetLocalizedMechanic("ins_cig_issue_color"));
		}
		if (gameItem.IsTag("CIGARETTE_SEAL_ISSUE"))
		{
			list.Add(LocHelper.GetLocalizedMechanic("ins_cig_issue_seal"));
		}
		if (gameItem.IsTag("CIGARETTE_LOGO_ISSUE"))
		{
			list.Add(LocHelper.GetLocalizedMechanic("ins_cig_issue_logo"));
		}
		if (gameItem.IsTag("CIGARETTE_BOX_ISSUE"))
		{
			list.Add(LocHelper.GetLocalizedMechanic("ins_cig_issue_box"));
		}
		string text = string.Join(", ", list);
		dialogue.NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_issues_with", text));
		return dialogue;
	}

	public static Dialogue CreateConfrontDialogue(string reason, GameItem gameItem)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 6;
		string reason2 = "";
		if (reason.Equals("CIGARETTE_LETTERING_ISSUE"))
		{
			reason2 = LocHelper.GetLocalizedMechanic("ins_cig_issue_lettering");
		}
		if (reason.Equals("CIGARETTE_COLOR_ISSUE"))
		{
			reason2 = LocHelper.GetLocalizedMechanic("ins_cig_issue_color");
		}
		if (reason.Equals("CIGARETTE_SEAL_ISSUE"))
		{
			reason2 = LocHelper.GetLocalizedMechanic("ins_cig_issue_seal");
		}
		if (reason.Equals("CIGARETTE_LOGO_ISSUE"))
		{
			reason2 = LocHelper.GetLocalizedMechanic("ins_cig_issue_logo");
		}
		if (reason.Equals("CIGARETTE_BOX_ISSUE"))
		{
			reason2 = LocHelper.GetLocalizedMechanic("ins_cig_issue_box");
		}
		if (IsConfrontCorrect(reason, gameItem))
		{
			if (!UniqueItemHelper.IsHaveUniqueId(gameItem))
			{
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_confront_correct")).SetStartAction(delegate
				{
					gameItem.FindItemFeatureByCategory("CATEGORY_GENUINE_CIGARETTE").ExposeFeature();
					InspectableHelper.OnConfrontSuccess(gameItem);
				}).NextDialogue()
					.SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_confront_lower_price"));
			}
			else
			{
				SetUniqueGoodExposeDialog(gameItem, dialogue, reason2);
			}
		}
		else
		{
			dialogue.SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_confront_wrong")).SetStartAction(delegate
			{
				InspectionUIManager.Instance.OnConfrontFailed();
			});
		}
		return dialogue;
	}

	public static Dialogue CreateTalkDialogue(GameItem gameItem)
	{
		Dialogue dialogue = new Dialogue();
		if (gameItem.FindItemFeatureByCategory("CATEGORY_GENUINE_CIGARETTE").realCondition.identifier.Equals("genuineCigarette"))
		{
			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("ins_cig_talk_genuine")).SetStartAction(delegate
			{
				InspectableHelper.OnConfrontSuccess(gameItem);
				gameItem.FindItemFeatureByCategory("CATEGORY_GENUINE_CIGARETTE").ExposeFeature();
				StoreReputation.ModCurrentClientReputation(2);
			}).NextDialogue()
				.SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_talk_thanks"));
		}
		else
		{
			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("ins_cig_talk_genuine")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_talk_genuine_deny"));
		}
		return dialogue;
	}

	public static bool IsConfrontCorrect(string reason, GameItem item)
	{
		return item.IsTag(reason);
	}

	public static void CreateDebugTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("CIGARETTE_TAG") && PlayerStore.Instance.devMode)
		{
			if (item.IsTag("CIGARETTE_LETTERING_ISSUE"))
			{
				builder.AddLine("CIGARETTE_LETTERING_ISSUE", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.IsTag("CIGARETTE_COLOR_ISSUE"))
			{
				builder.AddLine("CIGARETTE_COLOR_ISSUE", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.IsTag("CIGARETTE_SEAL_ISSUE"))
			{
				builder.AddLine("CIGARETTE_SEAL_ISSUE", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.IsTag("CIGARETTE_LOGO_ISSUE"))
			{
				builder.AddLine("CIGARETTE_LOGO_ISSUE", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.IsTag("CIGARETTE_BOX_ISSUE"))
			{
				builder.AddLine("CIGARETTE_BOX_ISSUE", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
		}
	}

	public static void SetConterfeitGeorgio1(GameItem gameItem)
	{
		gameItem.ModifyTag("CIGARETTE_SEAL", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		gameItem.EnableTag("CIGARETTE_SEAL_ISSUE");
		gameItem.ModifyTag("CIGARETTE_LOGO", delegate(TagState state)
		{
			state.Enable().SetInt(3);
		});
		gameItem.EnableTag("CIGARETTE_LOGO_ISSUE");
		gameItem.ModifyTag("CIGARETTE_LETTERING", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("CIGARETTE_BOX", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
	}

	public static void SetConterfeitGeorgio2(GameItem gameItem)
	{
		gameItem.ModifyTag("CIGARETTE_SEAL", delegate(TagState state)
		{
			state.Enable().SetInt(-1);
		});
		gameItem.EnableTag("CIGARETTE_SEAL_ISSUE");
		gameItem.ModifyTag("CIGARETTE_LOGO", delegate(TagState state)
		{
			state.Enable().SetInt(-1);
		});
		gameItem.EnableTag("CIGARETTE_LOGO_ISSUE");
		gameItem.ModifyTag("CIGARETTE_LETTERING", delegate(TagState state)
		{
			state.Enable().SetInt(-1);
		});
		gameItem.EnableTag("CIGARETTE_LETTERING_ISSUE");
		gameItem.ModifyTag("CIGARETTE_BOX", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		gameItem.EnableTag("CIGARETTE_BOX_ISSUE");
		gameItem.EnableTag("CIGARETTE_COLOR_ISSUE");
	}

	public static void SetUniqueGoodExposeDialog(GameItem gameItem, Dialogue dialogue, string reason)
	{
		if (UniqueItemHelper.IsUniqueId(gameItem, "georgio2_cigarette1"))
		{
			dialogue.SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_georgio_didnt_know")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_georgio_lower_price"))
				.SetEndAction(delegate
				{
					gameItem.FindItemFeatureByCategory("CATEGORY_GENUINE_CIGARETTE").ExposeFeature();
					InspectableHelper.OnConfrontSuccess(gameItem);
				});
		}
		else if (UniqueItemHelper.IsUniqueId(gameItem, "georgio2_cigarette2"))
		{
			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("ins_cig_georgio_everything_wrong")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_georgio_ok"))
				.NextDialogue()
				.SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_georgio_lower_price"))
				.SetEndAction(delegate
				{
					gameItem.FindItemFeatureByCategory("CATEGORY_GENUINE_CIGARETTE").ExposeFeature();
					InspectableHelper.OnConfrontSuccess(gameItem);
				});
		}
	}
}

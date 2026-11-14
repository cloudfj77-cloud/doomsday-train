using System.Linq;

public class InsInjectorHelper
{
	public const string INS_INJECTOR_TAG = "INS_INJECTOR_TAG";

	public const string ISSUE_INDICATOR = "ISSUE_INDICATOR";

	public const string ISSUE_LABEL = "ISSUE_LABEL";

	public const string ISSUE_COLOR = "ISSUE_COLOR";

	public const string ISSUE_SERIAL = "ISSUE_SERIAL";

	public const string ISSUE_BODY = "ISSUE_BODY";

	public const string INJECTOR_INDICATOR = "INJECTOR_INDICATOR";

	public const string INJECTOR_LABEL = "INJECTOR_LABEL";

	public const string INJECTOR_COLOR = "INJECTOR_COLOR";

	public const string INJECTOR_BODY = "INJECTOR_BODY";

	public const string INJECTOR_SERIAL = "INJECTOR_SERIAL";

	public const string INJECTOR_COMPANY = "INJECTOR_COMPANY";

	public const string INJECTOR_MODEL = "INJECTOR_MODEL";

	public const string CORRECT_COMPANY = "Carmicheal Pharmaceutical";

	public const string CORRECT_MODEL = "Immunivax";

	public static int[] RECALLED_PREFIX = new int[7] { 250, 251, 301, 355, 385, 386, 421 };

	public static void Init(GameItem gameItem)
	{
		gameItem.EnableTag("INS_INJECTOR_TAG");
		InspectableHelper.InitInspectableItem(gameItem);
	}

	public static void SetGenuine(GameItem gameItem)
	{
		gameItem.ModifyTag("INJECTOR_INDICATOR", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("INJECTOR_BODY", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("INJECTOR_LABEL", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("INJECTOR_COLOR", delegate(TagState state)
		{
			state.Enable().SetInt(-1);
		});
		gameItem.ModifyTag("INJECTOR_SERIAL", delegate(TagState state)
		{
			state.Enable().SetString(GenerateCorrectSerial());
		});
		gameItem.ModifyTag("INJECTOR_COMPANY", delegate(TagState state)
		{
			state.Enable().SetString("Carmicheal Pharmaceutical");
		});
		gameItem.ModifyTag("INJECTOR_MODEL", delegate(TagState state)
		{
			state.Enable().SetString("Immunivax");
		});
	}

	public static void SetExpired(GameItem gameItem)
	{
		SetGenuine(gameItem);
		gameItem.ModifyTag("INJECTOR_INDICATOR", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		gameItem.EnableTag("ISSUE_INDICATOR");
	}

	public static void SetCounterfeit(GameItem gameItem)
	{
		SetGenuine(gameItem);
		if (RNG.Roll(25))
		{
			gameItem.ModifyTag("INJECTOR_SERIAL", delegate(TagState state)
			{
				state.Enable().SetString(GenerateInCorrectSerial());
			});
			gameItem.EnableTag("ISSUE_SERIAL");
		}
		else if (RNG.Roll(33))
		{
			gameItem.ModifyTag("INJECTOR_BODY", delegate(TagState state)
			{
				state.Enable().SetInt(-1);
			});
			gameItem.EnableTag("ISSUE_COLOR");
			gameItem.ModifyTag("INJECTOR_COLOR", delegate(TagState state)
			{
				state.Enable().SetInt(RNG.GetRandomInt(0, 1));
			});
		}
		else if (RNG.Roll(50))
		{
			gameItem.EnableTag("ISSUE_LABEL");
			gameItem.ModifyTag("INJECTOR_MODEL", delegate(TagState state)
			{
				state.Enable().SetString(GenerateIncorrectModel());
			});
		}
		else
		{
			gameItem.EnableTag("ISSUE_LABEL");
			gameItem.ModifyTag("INJECTOR_COMPANY", delegate(TagState state)
			{
				state.Enable().SetString(GenerateIncorrectCompany());
			});
		}
	}

	public static void SetUnusable(GameItem gameItem)
	{
		SetGenuine(gameItem);
		if (RNG.Roll(33))
		{
			gameItem.ModifyTag("INJECTOR_INDICATOR", delegate(TagState state)
			{
				state.Enable().SetInt(3);
			});
			gameItem.EnableTag("ISSUE_INDICATOR");
		}
		else if (RNG.Roll(50))
		{
			gameItem.ModifyTag("INJECTOR_BODY", delegate(TagState state)
			{
				state.Enable().SetInt(RNG.GetRandomInt(1, 3));
			});
			gameItem.EnableTag("ISSUE_BODY");
		}
		else
		{
			gameItem.ModifyTag("INJECTOR_SERIAL", delegate(TagState state)
			{
				state.Enable().SetString(GenerateRecalledSerial());
			});
			gameItem.EnableTag("ISSUE_SERIAL");
		}
	}

	public static GameItem CreateRealInjector()
	{
		return DirectoryMaster.Item("large_purple_injector");
	}

	public static GameItem CreateUnusableInjector()
	{
		GameItem gameItem = DirectoryMaster.Item("large_purple_injector");
		SetUnusable(gameItem);
		return gameItem;
	}

	public static GameItem CreateExpiredInjector()
	{
		GameItem gameItem = DirectoryMaster.Item("large_purple_injector");
		SetExpired(gameItem);
		return gameItem;
	}

	public static GameItem CreateCounterfeit()
	{
		GameItem gameItem = DirectoryMaster.Item("large_purple_injector");
		SetCounterfeit(gameItem);
		return gameItem;
	}

	public static string GenerateCorrectSerial()
	{
		int num = 0;
		int i = 0;
		int num2;
		for (num2 = 10; i < num2; i++)
		{
			num = RNG.GetRandomInt(125, 455);
			if (!RECALLED_PREFIX.Contains(num))
			{
				break;
			}
		}
		if (i >= num2)
		{
			num = 133;
		}
		int randomInt = RNG.GetRandomInt(1000, 9999);
		return $"{num}{randomInt}";
	}

	public static string GenerateRecalledSerial()
	{
		int num = RECALLED_PREFIX[RNG.GetRandomIntExculsive(0, RECALLED_PREFIX.Length - 1)];
		int randomInt = RNG.GetRandomInt(1000, 9999);
		return $"{num}{randomInt}";
	}

	public static string GenerateInCorrectSerial()
	{
		int chance = 20;
		if (RNG.Roll(chance))
		{
			return RNG.GetRandomInt(100000, 999999).ToString() ?? "";
		}
		if (RNG.Roll(chance))
		{
			return RNG.GetRandomInt(10000000, 99999999).ToString() ?? "";
		}
		if (RNG.Roll(chance))
		{
			return RNG.GetRandomInt(100, 124).ToString() + RNG.GetRandomInt(1000, 9999);
		}
		if (RNG.Roll(chance))
		{
			return RNG.GetRandomInt(456, 999).ToString() + RNG.GetRandomInt(1000, 9999);
		}
		return "";
	}

	public static string GenerateIncorrectCompany()
	{
		string[] array = new string[8] { "Micheal Pharmaceutical", "Carmicheal Inc.", "Carmiicheal Phamaceutical", "Carmicheal Pharmaceutical Ltd.", "Crmichal Pharmaceutical", "Carmicheal's Meds", "Carmicheal", "" };
		return array[RNG.GetRandomIntExculsive(0, array.Length)];
	}

	public static string GenerateIncorrectModel()
	{
		string[] array = new string[10] { "Immunivaxine", "Immunovax", "Imunivax", "ImmuniPro", "ImmuniVax 3000", "Im univax", "Immunivx", "Immunevax", "Imm nivax", "" };
		return array[RNG.GetRandomIntExculsive(0, array.Length)];
	}

	public static void CreateDebugTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("INS_INJECTOR_TAG") && PlayerStore.Instance.devMode)
		{
			if (item.IsTag("ISSUE_INDICATOR"))
			{
				builder.AddLine("ISSUE_INDICATOR", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.IsTag("ISSUE_LABEL"))
			{
				builder.AddLine("ISSUE_LABEL", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.IsTag("ISSUE_COLOR"))
			{
				builder.AddLine("ISSUE_COLOR", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.IsTag("ISSUE_SERIAL"))
			{
				builder.AddLine("ISSUE_SERIAL", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.IsTag("ISSUE_BODY"))
			{
				builder.AddLine("ISSUE_BODY", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
		}
	}

	public static Dialogue CreateConfrontDialogue(string reason, GameItem gameItem)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 6;
		if (reason.Equals("ISSUE_INDICATOR"))
		{
			LocHelper.GetLocalizedMechanic("ins_inj_issue_indicator");
		}
		if (reason.Equals("ISSUE_LABEL"))
		{
			LocHelper.GetLocalizedMechanic("ins_inj_issue_label");
		}
		if (reason.Equals("ISSUE_SERIAL"))
		{
			LocHelper.GetLocalizedMechanic("ins_inj_issue_serial");
		}
		if (reason.Equals("ISSUE_BODY"))
		{
			LocHelper.GetLocalizedMechanic("ins_inj_issue_body");
		}
		if (reason.Equals("ISSUE_COLOR"))
		{
			LocHelper.GetLocalizedMechanic("ins_cig_issue_color");
		}
		if (InspectableHelper.IsConfrontCorrect(reason, gameItem))
		{
			if (!UniqueItemHelper.IsHaveUniqueId(gameItem))
			{
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_confront_correct")).SetStartAction(delegate
				{
					gameItem.FindItemFeatureByCategory("CATEGORY_GENUINE_INJECTOR").ExposeFeature();
					InspectableHelper.OnConfrontSuccess(gameItem);
				}).NextDialogue()
					.SetText("CLIENT", LocHelper.GetLocalizedMechanic("ins_cig_confront_lower_price"));
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

	public static bool IsOnlyIssueRecalled(GameItem gameItem)
	{
		if (gameItem == null)
		{
			return false;
		}
		if (gameItem.IsTag("ISSUE_BODY") || gameItem.IsTag("ISSUE_COLOR") || gameItem.IsTag("ISSUE_LABEL") || gameItem.IsTag("ISSUE_INDICATOR"))
		{
			return false;
		}
		if (gameItem.IsTag("ISSUE_SERIAL"))
		{
			string @string = gameItem.GetTagReadonly("INJECTOR_SERIAL").GetString();
			if (!string.IsNullOrEmpty(@string) && @string.Length >= 3 && int.TryParse(@string.Substring(0, 3), out var result))
			{
				return RECALLED_PREFIX.Contains(result);
			}
			return false;
		}
		return false;
	}
}

using UnityEngine;

public class PlasmaHelper
{
	public const string PLASMA_CELL_TAG = "PLASMA_CELL_TAG";

	public const string PLASMA_CASING = "PLASMA_CASING";

	public const string PLASMA_CASING_CONDITION_CRACK = "PLASMA_CASING_CONDITION_CRACK";

	public const string PLASMA_CASING_CONDITION_UNCOVERED = "PLASMA_CASING_CONDITION_UNCOVERED";

	public const string PLASMA_CASING_CONDITION_REINFORCED = "PLASMA_CASING_CONDITION_REINFORCED";

	public const string PLASMA_CASING_CONDITION_WEAR = "PLASMA_CASING_CONDITION_WEAR";

	public const string PLASMA_SERIAL = "PLASMA_SERIAL";

	public const string PLASMA_COLOR = "PLASMA_COLOR";

	public const string PLASMA_SEAL = "PLASMA_SEAL";

	public const string NONE = "NONE";

	public const string CRACK = "CRACK";

	public const string LL = "LL";

	public const string ZHONGHUA = "ZHONGHUA";

	public const string REPUBLIC = "REPUBLIC";

	public const string IGNITION_LAB = "IGNITION_LAB";

	public const string STANDARD = "STANDARD";

	public const string HIGH_QUALITY = "HIGH_QUALITY";

	public const string POOR1 = "POOR1";

	public const string POOR2 = "POOR2";

	public const string WEAR = "WEAR";

	public const string WEAR_HEAVY = "WEAR_HEAVY";

	public const string UNCOVERED = "UNCOVERED";

	public const string REINFORCED = "REINFORCED";

	public static void InitPlasmaItem(GameItem gameItem)
	{
		gameItem.EnableTag("INSPECTABLE_TAG");
		AppraisalHelper.InitAppraisalItem(gameItem, 300);
		gameItem.EnableTag("PLASMA_CELL_TAG");
		InitDefaultPlasma(gameItem);
	}

	public static void InitDefaultPlasma(GameItem gameItem)
	{
		gameItem.ModifyTag("PLASMA_CASING", delegate(TagState state)
		{
			state.Enable().SetString("");
		});
		gameItem.ModifyTag("PLASMA_CASING_CONDITION_CRACK", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("PLASMA_CASING_CONDITION_WEAR", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("PLASMA_CASING_CONDITION_UNCOVERED", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("PLASMA_CASING_CONDITION_REINFORCED", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("PLASMA_COLOR", delegate(TagState state)
		{
			state.Enable().SetString("STANDARD");
		});
		gameItem.ModifyTag("PLASMA_SEAL", delegate(TagState state)
		{
			state.Enable().SetString("LL");
		});
	}

	public static void InitLabPlasma(GameItem gameItem)
	{
		gameItem.ModifyTag("PLASMA_SEAL", delegate(TagState state)
		{
			state.Enable().SetString("IGNITION_LAB");
		});
		gameItem.ModifyTag("PLASMA_CASING_CONDITION_CRACK", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("PLASMA_CASING_CONDITION_UNCOVERED", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("PLASMA_CASING_CONDITION_REINFORCED", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("PLASMA_CASING_CONDITION_WEAR", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("PLASMA_COLOR", delegate(TagState state)
		{
			state.Enable().SetString("HIGH_QUALITY");
		});
	}

	public static void InitZhongHuaPlasma(GameItem gameItem)
	{
		gameItem.ModifyTag("PLASMA_SEAL", delegate(TagState state)
		{
			state.Enable().SetString("ZHONGHUA");
		});
		gameItem.ModifyTag("PLASMA_CASING_CONDITION_CRACK", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("PLASMA_CASING_CONDITION_UNCOVERED", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		if (RNG.Roll(75))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_REINFORCED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_REINFORCED", delegate(TagState state)
			{
				state.Enable().SetString("REINFORCED");
			});
		}
		gameItem.ModifyTag("PLASMA_CASING_CONDITION_WEAR", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("PLASMA_COLOR", delegate(TagState state)
		{
			state.Enable().SetString("STANDARD");
		});
	}

	public static void InitLLPlasma(GameItem gameItem)
	{
		if (RNG.Roll(75))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_CRACK", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_CRACK", delegate(TagState state)
			{
				state.Enable().SetString("CRACK");
			});
		}
		if (RNG.Roll(75))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_UNCOVERED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_UNCOVERED", delegate(TagState state)
			{
				state.Enable().SetString("UNCOVERED");
			});
		}
		if (RNG.Roll(75))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_REINFORCED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_REINFORCED", delegate(TagState state)
			{
				state.Enable().SetString("REINFORCED");
			});
		}
		if (RNG.Roll(33))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_WEAR", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else if (RNG.Roll(33))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_WEAR", delegate(TagState state)
			{
				state.Enable().SetString("WEAR");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_WEAR", delegate(TagState state)
			{
				state.Enable().SetString("WEAR_HEAVY");
			});
		}
		if (RNG.Roll(33))
		{
			gameItem.ModifyTag("PLASMA_COLOR", delegate(TagState state)
			{
				state.Enable().SetString("STANDARD");
			});
		}
		else if (RNG.Roll(33))
		{
			gameItem.ModifyTag("PLASMA_COLOR", delegate(TagState state)
			{
				state.Enable().SetString("POOR1");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_COLOR", delegate(TagState state)
			{
				state.Enable().SetString("POOR2");
			});
		}
		gameItem.ModifyTag("PLASMA_SEAL", delegate(TagState state)
		{
			state.Enable().SetString("LL");
		});
	}

	public static void InitRepublicPlasma(GameItem gameItem)
	{
		gameItem.ModifyTag("PLASMA_SEAL", delegate(TagState state)
		{
			state.Enable().SetString("REPUBLIC");
		});
		gameItem.ModifyTag("PLASMA_CASING_CONDITION_CRACK", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		if (RNG.Roll(75))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_UNCOVERED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_UNCOVERED", delegate(TagState state)
			{
				state.Enable().SetString("UNCOVERED");
			});
		}
		if (RNG.Roll(75))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_REINFORCED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_REINFORCED", delegate(TagState state)
			{
				state.Enable().SetString("REINFORCED");
			});
		}
		if (RNG.Roll(33))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_WEAR", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else if (RNG.Roll(33))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_WEAR", delegate(TagState state)
			{
				state.Enable().SetString("WEAR");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_WEAR", delegate(TagState state)
			{
				state.Enable().SetString("WEAR_HEAVY");
			});
		}
		gameItem.ModifyTag("PLASMA_COLOR", delegate(TagState state)
		{
			state.Enable().SetString("STANDARD");
		});
	}

	public static void InitRandomPlasma(GameItem gameItem)
	{
		if (RNG.Roll(75))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_CRACK", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_CRACK", delegate(TagState state)
			{
				state.Enable().SetString("CRACK");
			});
		}
		if (RNG.Roll(75))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_UNCOVERED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_UNCOVERED", delegate(TagState state)
			{
				state.Enable().SetString("UNCOVERED");
			});
		}
		if (RNG.Roll(75))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_REINFORCED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_REINFORCED", delegate(TagState state)
			{
				state.Enable().SetString("REINFORCED");
			});
		}
		if (RNG.Roll(33))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_WEAR", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else if (RNG.Roll(33))
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_WEAR", delegate(TagState state)
			{
				state.Enable().SetString("WEAR");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_CASING_CONDITION_WEAR", delegate(TagState state)
			{
				state.Enable().SetString("WEAR_HEAVY");
			});
		}
		if (RNG.Roll(25))
		{
			gameItem.ModifyTag("PLASMA_COLOR", delegate(TagState state)
			{
				state.Enable().SetString("STANDARD");
			});
		}
		else if (RNG.Roll(25))
		{
			gameItem.ModifyTag("PLASMA_COLOR", delegate(TagState state)
			{
				state.Enable().SetString("HIGH_QUALITY");
			});
		}
		else if (RNG.Roll(25))
		{
			gameItem.ModifyTag("PLASMA_COLOR", delegate(TagState state)
			{
				state.Enable().SetString("POOR1");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_COLOR", delegate(TagState state)
			{
				state.Enable().SetString("POOR2");
			});
		}
		if (RNG.Roll(25))
		{
			gameItem.ModifyTag("PLASMA_SEAL", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		else if (RNG.Roll(25))
		{
			gameItem.ModifyTag("PLASMA_SEAL", delegate(TagState state)
			{
				state.Enable().SetString("ZHONGHUA");
			});
		}
		else if (RNG.Roll(25))
		{
			gameItem.ModifyTag("PLASMA_SEAL", delegate(TagState state)
			{
				state.Enable().SetString("REPUBLIC");
			});
		}
		else
		{
			gameItem.ModifyTag("PLASMA_SEAL", delegate(TagState state)
			{
				state.Enable().SetString("IGNITION_LAB");
			});
		}
	}

	public static int GetRandomFakePrice(GameItem gameItem, int LowerOffset, int upperOffset)
	{
		return GetCorrectValue(gameItem);
	}

	public static int GetCorrectValue(GameItem gameItem)
	{
		if (!gameItem.IsTag("PLASMA_CELL_TAG"))
		{
			return 0;
		}
		int num = 300;
		gameItem.GetTagReadonly("PLASMA_CASING").GetString();
		string @string = gameItem.GetTagReadonly("PLASMA_CASING_CONDITION_WEAR").GetString();
		string string2 = gameItem.GetTagReadonly("PLASMA_CASING_CONDITION_CRACK").GetString();
		string string3 = gameItem.GetTagReadonly("PLASMA_CASING_CONDITION_UNCOVERED").GetString();
		string string4 = gameItem.GetTagReadonly("PLASMA_CASING_CONDITION_REINFORCED").GetString();
		string string5 = gameItem.GetTagReadonly("PLASMA_COLOR").GetString();
		string string6 = gameItem.GetTagReadonly("PLASMA_SEAL").GetString();
		if (@string == "WEAR")
		{
			num -= 45;
		}
		else if (@string == "WEAR_HEAVY")
		{
			num -= 90;
		}
		if (string2 == "CRACK")
		{
			num -= 150;
		}
		if (string3 == "UNCOVERED")
		{
			num -= 30;
		}
		if (string4 == "REINFORCED")
		{
			num += 60;
		}
		switch (string5)
		{
		case "STANDARD":
			num = num;
			break;
		case "POOR1":
		case "POOR2":
			num -= 150;
			break;
		case "HIGH_QUALITY":
			num += 150;
			break;
		}
		if (!(string6 == "LL"))
		{
			goto IL_017f;
		}
		switch (string5)
		{
		case "STANDARD":
		case "POOR1":
		case "POOR2":
			break;
		default:
			goto IL_017f;
		}
		num += 60;
		goto IL_0255;
		IL_017f:
		if (string6 == "ZHONGHUA" && @string == "NONE" && string2 == "NONE" && string3 == "NONE" && string5 == "STANDARD")
		{
			num += 120;
		}
		else if (string6 == "REPUBLIC" && string2 != "CRACK" && string5 == "STANDARD")
		{
			num += 120;
		}
		else if (string6 == "IGNITION_LAB" && string4 == "NONE" && @string == "NONE" && string2 == "NONE" && string3 == "NONE" && string5 == "HIGH_QUALITY")
		{
			num += 750;
		}
		goto IL_0255;
		IL_0255:
		return Mathf.Max(num, 90);
	}

	public static void PrintDebugFuel(GameItem gameItem)
	{
		string @string = gameItem.GetTagReadonly("PLASMA_CASING_CONDITION_WEAR").GetString();
		string string2 = gameItem.GetTagReadonly("PLASMA_CASING_CONDITION_CRACK").GetString();
		string string3 = gameItem.GetTagReadonly("PLASMA_CASING_CONDITION_UNCOVERED").GetString();
		string string4 = gameItem.GetTagReadonly("PLASMA_CASING_CONDITION_REINFORCED").GetString();
		string string5 = gameItem.GetTagReadonly("PLASMA_COLOR").GetString();
		string string6 = gameItem.GetTagReadonly("PLASMA_SEAL").GetString();
		Debug.Log("Wear " + @string);
		Debug.Log("Crack " + string2);
		Debug.Log("Uncovered Screw " + string3);
		Debug.Log("Reinforced " + string4);
		Debug.Log("Color " + string5);
		Debug.Log("Seal " + string6);
		Debug.Log("Correct Value " + GetCorrectValue(gameItem));
	}
}

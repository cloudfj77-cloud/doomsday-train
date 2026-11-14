using UnityEngine;

public class AppHandHelper
{
	public const string HAND_TAG = "HAND_TAG";

	public const string HAND_MATERIAL = "HAND_MATERIAL";

	public const string HAND_INTERFACE = "HAND_INTERFACE";

	public const string HAND_INTERFACE_MARKING = "HAND_INTERFACE_MARKING";

	public const string HAND_CONDITION = "HAND_CONDITION";

	public const string HAND_ADDON_ARMORED = "HAND_ADDON_ARMORED";

	public const string HAND_ADDON_BLADED = "HAND_ADDON_BLADED";

	public const string NONE = "NONE";

	public const string MATERIAL_GOLD_PLATED = "GOLD_PLATED";

	public const string MATERIAL_CARBON_COMPOSITE = "CARBON_COMPOSITE";

	public const string MATERIAL_ALUMINUM_ALLOY = "ALUMINUM_ALLOY";

	public const string INTERFACE_NEURAL = "NEURAL";

	public const string INTERFACE_EMG_PLUS = "EMG_PLUS";

	public const string INTERFACE_EMG = "EMG";

	public const string INTERFACE_UNSPECIFIED = "UNSPECIFIED";

	public const string MARKING_FULL = "FULL";

	public const string MARKING_SYMBOL = "SYMBOL";

	public const string SPRITE_ATLAS = "Items/items_inspectable";

	public const string SPRITE_STEEL = "hand_steel";

	public const string SPRITE_CARBON = "hand_carbon";

	public const string SPRITE_GOLD = "hand_gold";

	public const string CONDITION_WEAR = "WEAR";

	public const string CONDITION_DAMAGE = "DAMAGE";

	public const string ADDON_ARMORED = "ARMORED";

	public const string ADDON_BLADED = "BLADED";

	public const int BASE_VALUE = 100;

	public const int MIN_VALUE = 50;

	public const int MATERIAL_GOLD_PLATED_BONUS = 200;

	public const int MATERIAL_CARBON_COMPOSITE_BONUS = 50;

	public const int MATERIAL_ALUMINUM_ALLOY_BONUS = 0;

	public const int INTERFACE_NEURAL_BONUS = 120;

	public const int INTERFACE_EMG_PLUS_BONUS = 45;

	public const int INTERFACE_EMG_BONUS = 0;

	public const int INTERFACE_UNSPECIFIED_PENALTY = -60;

	public const int CONDITION_WEAR_PENALTY = -35;

	public const int CONDITION_DAMAGE_PENALTY = -85;

	public const int ADDON_ARMORED_BONUS = 50;

	public const int ADDON_BLADED_BONUS = 80;

	public const int SET_TACTICAL_BONUS = 100;

	public const int SET_DELUXE_BONUS = 200;

	public static void InitAppHandItem(GameItem gameItem)
	{
		gameItem.EnableTag("INSPECTABLE_TAG");
		AppraisalHelper.InitAppraisalItem(gameItem, 100);
		gameItem.EnableTag("HAND_TAG");
		InitDefault(gameItem);
	}

	public static void InitDefault(GameItem gameItem)
	{
		gameItem.ModifyTag("HAND_MATERIAL", delegate(TagState state)
		{
			state.Enable().SetString("ALUMINUM_ALLOY");
		});
		gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
		{
			state.Enable().SetString("EMG");
		});
		gameItem.ModifyTag("HAND_INTERFACE_MARKING", delegate(TagState state)
		{
			state.Enable().SetString("FULL");
		});
		gameItem.ModifyTag("HAND_CONDITION", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("HAND_ADDON_ARMORED", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("HAND_ADDON_BLADED", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		RefreshSpriteForMaterial(gameItem);
	}

	public static void InitPremium(GameItem gameItem)
	{
		gameItem.ModifyTag("HAND_MATERIAL", delegate(TagState state)
		{
			state.Enable().SetString("GOLD_PLATED");
		});
		if (RNG.Roll(50))
		{
			gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
			{
				state.Enable().SetString("NEURAL");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
			{
				state.Enable().SetString("EMG_PLUS");
			});
		}
		gameItem.ModifyTag("HAND_INTERFACE_MARKING", delegate(TagState state)
		{
			state.Enable().SetString(RNG.Roll(50) ? "FULL" : "SYMBOL");
		});
		gameItem.ModifyTag("HAND_CONDITION", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("HAND_ADDON_ARMORED", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("HAND_ADDON_BLADED", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		RefreshSpriteForMaterial(gameItem);
	}

	public static void InitCombat(GameItem gameItem)
	{
		gameItem.ModifyTag("HAND_MATERIAL", delegate(TagState state)
		{
			state.Enable().SetString("CARBON_COMPOSITE");
		});
		gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
		{
			state.Enable().SetString("EMG_PLUS");
		});
		gameItem.ModifyTag("HAND_INTERFACE_MARKING", delegate(TagState state)
		{
			state.Enable().SetString(RNG.Roll(50) ? "FULL" : "SYMBOL");
		});
		string combatCondition = (RNG.Roll(60) ? "NONE" : "WEAR");
		gameItem.ModifyTag("HAND_CONDITION", delegate(TagState state)
		{
			state.Enable().SetString(combatCondition);
		});
		if (combatCondition == "NONE" && RNG.Roll(60))
		{
			gameItem.ModifyTag("HAND_ADDON_ARMORED", delegate(TagState state)
			{
				state.Enable().SetString("ARMORED");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_ADDON_ARMORED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		if (combatCondition == "NONE" && RNG.Roll(50))
		{
			gameItem.ModifyTag("HAND_ADDON_BLADED", delegate(TagState state)
			{
				state.Enable().SetString("BLADED");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_ADDON_BLADED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		RefreshSpriteForMaterial(gameItem);
	}

	public static void InitJunk(GameItem gameItem)
	{
		if (RNG.Roll(50))
		{
			gameItem.ModifyTag("HAND_MATERIAL", delegate(TagState state)
			{
				state.Enable().SetString("ALUMINUM_ALLOY");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_MATERIAL", delegate(TagState state)
			{
				state.Enable().SetString("CARBON_COMPOSITE");
			});
		}
		if (RNG.Roll(50))
		{
			gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
			{
				state.Enable().SetString("EMG");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
			{
				state.Enable().SetString("UNSPECIFIED");
			});
		}
		gameItem.ModifyTag("HAND_INTERFACE_MARKING", delegate(TagState state)
		{
			state.Enable().SetString(RNG.Roll(50) ? "FULL" : "SYMBOL");
		});
		if (RNG.Roll(50))
		{
			gameItem.ModifyTag("HAND_CONDITION", delegate(TagState state)
			{
				state.Enable().SetString("WEAR");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_CONDITION", delegate(TagState state)
			{
				state.Enable().SetString("DAMAGE");
			});
		}
		gameItem.ModifyTag("HAND_ADDON_ARMORED", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("HAND_ADDON_BLADED", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		RefreshSpriteForMaterial(gameItem);
	}

	public static void InitTactical(GameItem gameItem)
	{
		gameItem.ModifyTag("HAND_MATERIAL", delegate(TagState state)
		{
			state.Enable().SetString("CARBON_COMPOSITE");
		});
		if (RNG.Roll(50))
		{
			gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
			{
				state.Enable().SetString("NEURAL");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
			{
				state.Enable().SetString("EMG_PLUS");
			});
		}
		gameItem.ModifyTag("HAND_INTERFACE_MARKING", delegate(TagState state)
		{
			state.Enable().SetString(RNG.Roll(50) ? "FULL" : "SYMBOL");
		});
		gameItem.ModifyTag("HAND_CONDITION", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		gameItem.ModifyTag("HAND_ADDON_ARMORED", delegate(TagState state)
		{
			state.Enable().SetString("ARMORED");
		});
		gameItem.ModifyTag("HAND_ADDON_BLADED", delegate(TagState state)
		{
			state.Enable().SetString("BLADED");
		});
		RefreshSpriteForMaterial(gameItem);
	}

	public static void InitDeluxe(GameItem gameItem)
	{
		gameItem.ModifyTag("HAND_MATERIAL", delegate(TagState state)
		{
			state.Enable().SetString("GOLD_PLATED");
		});
		gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
		{
			state.Enable().SetString("NEURAL");
		});
		gameItem.ModifyTag("HAND_INTERFACE_MARKING", delegate(TagState state)
		{
			state.Enable().SetString(RNG.Roll(50) ? "FULL" : "SYMBOL");
		});
		gameItem.ModifyTag("HAND_CONDITION", delegate(TagState state)
		{
			state.Enable().SetString("NONE");
		});
		if (RNG.Roll(30))
		{
			gameItem.ModifyTag("HAND_ADDON_ARMORED", delegate(TagState state)
			{
				state.Enable().SetString("ARMORED");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_ADDON_ARMORED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		if (RNG.Roll(20))
		{
			gameItem.ModifyTag("HAND_ADDON_BLADED", delegate(TagState state)
			{
				state.Enable().SetString("BLADED");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_ADDON_BLADED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		RefreshSpriteForMaterial(gameItem);
	}

	public static void InitRandom(GameItem gameItem)
	{
		int randomInt = RNG.GetRandomInt(1, 100);
		if (randomInt <= 3)
		{
			InitDeluxe(gameItem);
			return;
		}
		if (randomInt <= 6)
		{
			InitTactical(gameItem);
			return;
		}
		int randomInt2 = RNG.GetRandomInt(1, 100);
		if (randomInt2 <= 15)
		{
			gameItem.ModifyTag("HAND_MATERIAL", delegate(TagState state)
			{
				state.Enable().SetString("GOLD_PLATED");
			});
		}
		else if (randomInt2 <= 55)
		{
			gameItem.ModifyTag("HAND_MATERIAL", delegate(TagState state)
			{
				state.Enable().SetString("CARBON_COMPOSITE");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_MATERIAL", delegate(TagState state)
			{
				state.Enable().SetString("ALUMINUM_ALLOY");
			});
		}
		int randomInt3 = RNG.GetRandomInt(1, 100);
		if (randomInt3 <= 20)
		{
			gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
			{
				state.Enable().SetString("NEURAL");
			});
		}
		else if (randomInt3 <= 45)
		{
			gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
			{
				state.Enable().SetString("EMG_PLUS");
			});
		}
		else if (randomInt3 <= 85)
		{
			gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
			{
				state.Enable().SetString("EMG");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_INTERFACE", delegate(TagState state)
			{
				state.Enable().SetString("UNSPECIFIED");
			});
		}
		gameItem.ModifyTag("HAND_INTERFACE_MARKING", delegate(TagState state)
		{
			state.Enable().SetString(RNG.Roll(50) ? "FULL" : "SYMBOL");
		});
		int randomInt4 = RNG.GetRandomInt(1, 100);
		string randomCondition;
		if (randomInt4 <= 50)
		{
			randomCondition = "NONE";
		}
		else if (randomInt4 <= 75)
		{
			randomCondition = "WEAR";
		}
		else
		{
			randomCondition = "DAMAGE";
		}
		gameItem.ModifyTag("HAND_CONDITION", delegate(TagState state)
		{
			state.Enable().SetString(randomCondition);
		});
		if (randomCondition == "NONE" && RNG.Roll(33))
		{
			gameItem.ModifyTag("HAND_ADDON_ARMORED", delegate(TagState state)
			{
				state.Enable().SetString("ARMORED");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_ADDON_ARMORED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		if (randomCondition == "NONE" && RNG.Roll(33))
		{
			gameItem.ModifyTag("HAND_ADDON_BLADED", delegate(TagState state)
			{
				state.Enable().SetString("BLADED");
			});
		}
		else
		{
			gameItem.ModifyTag("HAND_ADDON_BLADED", delegate(TagState state)
			{
				state.Enable().SetString("NONE");
			});
		}
		RefreshSpriteForMaterial(gameItem);
	}

	public static void RefreshSpriteForMaterial(GameItem gameItem)
	{
		if (gameItem.IsTag("HAND_TAG"))
		{
			string @string = gameItem.GetTagReadonly("HAND_MATERIAL").GetString();
			string spritePath = ((@string == "GOLD_PLATED") ? "hand_gold" : ((!(@string == "CARBON_COMPOSITE")) ? "hand_steel" : "hand_carbon"));
			gameItem.SetSpriteAndShape("Items/items_inspectable", spritePath);
		}
	}

	public static int GetCorrectValue(GameItem gameItem)
	{
		if (!gameItem.IsTag("HAND_TAG"))
		{
			return 0;
		}
		int num = 100;
		string @string = gameItem.GetTagReadonly("HAND_MATERIAL").GetString();
		string string2 = gameItem.GetTagReadonly("HAND_INTERFACE").GetString();
		string string3 = gameItem.GetTagReadonly("HAND_CONDITION").GetString();
		string string4 = gameItem.GetTagReadonly("HAND_ADDON_ARMORED").GetString();
		string string5 = gameItem.GetTagReadonly("HAND_ADDON_BLADED").GetString();
		if (@string == "GOLD_PLATED")
		{
			num += 200;
		}
		else if (@string == "CARBON_COMPOSITE")
		{
			num += 50;
		}
		switch (string2)
		{
		case "NEURAL":
			num += 120;
			break;
		case "EMG_PLUS":
			num += 45;
			break;
		case "UNSPECIFIED":
			num += -60;
			break;
		}
		if (string3 == "WEAR")
		{
			num += -35;
		}
		else if (string3 == "DAMAGE")
		{
			num += -85;
		}
		if (string4 == "ARMORED")
		{
			num += 50;
		}
		if (string5 == "BLADED")
		{
			num += 80;
		}
		if (IsTacticalSet(@string, string2, string3, string4, string5))
		{
			num += 100;
		}
		if (IsDeluxeSet(@string, string2, string3))
		{
			num += 200;
		}
		return Mathf.Max(num, 50);
	}

	public static bool IsTacticalSet(string material, string controlInterface, string condition, string addonArmored, string addonBladed)
	{
		if (material == "CARBON_COMPOSITE" && (controlInterface == "EMG_PLUS" || controlInterface == "NEURAL") && condition == "NONE" && addonArmored == "ARMORED")
		{
			return addonBladed == "BLADED";
		}
		return false;
	}

	public static bool IsDeluxeSet(string material, string controlInterface, string condition)
	{
		if (material == "GOLD_PLATED" && controlInterface == "NEURAL")
		{
			return condition == "NONE";
		}
		return false;
	}

	public static void CreateDebugTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("HAND_TAG") && PlayerStore.Instance.devMode)
		{
			builder.AddLine("Correct Value " + GetCorrectValue(item), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			builder.AddLine("Material: " + item.GetTagReadonly("HAND_MATERIAL").GetString(), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			builder.AddLine("Interface: " + item.GetTagReadonly("HAND_INTERFACE").GetString(), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			builder.AddLine("Marking: " + item.GetTagReadonly("HAND_INTERFACE_MARKING").GetString(), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			builder.AddLine("Condition: " + item.GetTagReadonly("HAND_CONDITION").GetString(), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			builder.AddLine("Armored: " + item.GetTagReadonly("HAND_ADDON_ARMORED").GetString(), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			builder.AddLine("Bladed: " + item.GetTagReadonly("HAND_ADDON_BLADED").GetString(), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			string @string = item.GetTagReadonly("HAND_MATERIAL").GetString();
			string string2 = item.GetTagReadonly("HAND_INTERFACE").GetString();
			string string3 = item.GetTagReadonly("HAND_CONDITION").GetString();
			string string4 = item.GetTagReadonly("HAND_ADDON_ARMORED").GetString();
			string string5 = item.GetTagReadonly("HAND_ADDON_BLADED").GetString();
			if (IsTacticalSet(@string, string2, string3, string4, string5))
			{
				builder.AddLine("Set: Tactical (+" + 100 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (IsDeluxeSet(@string, string2, string3))
			{
				builder.AddLine("Set: Deluxe (+" + 200 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
		}
	}
}

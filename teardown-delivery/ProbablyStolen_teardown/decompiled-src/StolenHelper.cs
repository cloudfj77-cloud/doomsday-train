public class StolenHelper
{
	public const string STOLEN_TAG = "STOLEN_TAG";

	public const string STOLEN_VALUE_INT = "STOLEN_VALUE_INT";

	public const int DEFAULT_HEAT_LOSS = 18;

	public const int MINIMAL_THRESHOLD = 17;

	public const int LOW_THRESHOLD = 50;

	public const int MID_THRESHOLD = 85;

	public const int HIGH_THRESHOLD = 140;

	public static void CreateStolenItemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("STOLEN_TAG"))
		{
			int @int = item.GetTagReadonly("STOLEN_VALUE_INT").GetInt();
			builder.Add(LocHelper.GetLocalizedMechanic("mech_stolen_tooltip", @int), RenderHandler.ColorPalette.LightRed);
		}
	}

	public static bool IsStolenItem(GameItem item)
	{
		return item.IsTag("STOLEN_TAG");
	}

	public static void SetHeat(GameItem item, int heat)
	{
		if (item.IsTag("STOLEN_TAG"))
		{
			item.ModifyTag("STOLEN_VALUE_INT", delegate(TagState state)
			{
				state.SetInt(heat);
			});
			return;
		}
		item.EnableTag("STOLEN_TAG");
		item.ModifyTag("STOLEN_VALUE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(heat);
		});
	}

	public static int GetFoundChance(GameItem item)
	{
		return item.GetTagReadonly("STOLEN_VALUE_INT").GetInt();
	}

	public static bool IsFound(GameItem item)
	{
		if (item.IsTag("STOLEN_TAG"))
		{
			return RNG.Roll(GetFoundChance(item));
		}
		return false;
	}

	public static void InitStolenItem(GameItem item, int forcedHeat = 0)
	{
		if (item.IsTag("STOLEN_TAG"))
		{
			return;
		}
		item.EnableTag("STOLEN_TAG");
		if (forcedHeat > 0)
		{
			item.ModifyTag("STOLEN_VALUE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(forcedHeat);
			});
			return;
		}
		int num = (int)item.GetBaseValue();
		if (num <= 17)
		{
			item.DisableTag("STOLEN_TAG");
			item.ModifyTag("STOLEN_VALUE_INT", delegate(TagState state)
			{
				state.SetInt(0);
			});
			item.DisableTag("STOLEN_VALUE_INT");
		}
		else if (num < 50)
		{
			item.ModifyTag("STOLEN_VALUE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(25);
			});
		}
		else if (num < 85)
		{
			item.ModifyTag("STOLEN_VALUE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(50);
			});
		}
		else if (num < 140)
		{
			item.ModifyTag("STOLEN_VALUE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(75);
			});
		}
		else
		{
			item.ModifyTag("STOLEN_VALUE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(100);
			});
		}
	}

	public static void RemoveStolenStatus(GameItem item)
	{
		if (item.IsTag("STOLEN_TAG"))
		{
			item.DisableTag("STOLEN_TAG");
			item.ModifyTag("STOLEN_VALUE_INT", delegate(TagState state)
			{
				state.SetInt(0);
			});
		}
	}

	public static void UpdateStolenItem(GameItem item)
	{
		if (!item.IsTag("STOLEN_TAG"))
		{
			return;
		}
		int heat = item.GetTagReadonly("STOLEN_VALUE_INT").GetInt();
		int num = 18;
		if (item.IsTag("CONTRABAND_LEVEL_LOW"))
		{
			num -= 2;
		}
		else if (item.IsTag("CONTRABAND_LEVEL_MID"))
		{
			num -= 4;
		}
		else if (item.IsTag("CONTRABAND_LEVEL_HIGH"))
		{
			num -= 6;
		}
		else if (item.IsTag("CONTRABAND_LEVEL_CRITICAL"))
		{
			num -= 8;
		}
		heat -= num;
		if (heat <= 0)
		{
			item.DisableTag("STOLEN_TAG");
			item.ModifyTag("STOLEN_VALUE_INT", delegate(TagState state)
			{
				state.SetInt(0);
			});
		}
		else
		{
			item.ModifyTag("STOLEN_VALUE_INT", delegate(TagState state)
			{
				state.SetInt(heat);
			});
		}
	}

	public static void ApplyStolenDiscount(GameItem item)
	{
		if (item.IsTag("STOLEN_TAG"))
		{
			int @int = item.GetTagReadonly("STOLEN_VALUE_INT").GetInt();
			if (@int <= 25)
			{
				item.AddItemFeature(ItemFeatureList.StolenBonusBuyingLow());
			}
			else if (@int <= 50)
			{
				item.AddItemFeature(ItemFeatureList.StolenBonusBuyingMid());
			}
			else if (@int <= 75)
			{
				item.AddItemFeature(ItemFeatureList.StolenBonusBuyingHigh());
			}
			else
			{
				item.AddItemFeature(ItemFeatureList.StolenBonusBuying());
			}
		}
	}

	public static float GetStolenItemTradingRepBonus(GameItem item)
	{
		if (item.IsTag("STOLEN_TAG"))
		{
			int @int = item.GetTagReadonly("STOLEN_VALUE_INT").GetInt();
			if (@int <= 25)
			{
				return 1.5f;
			}
			if (@int <= 50)
			{
				return 2f;
			}
			if (@int <= 75)
			{
				return 2.5f;
			}
			return 3f;
		}
		return 1f;
	}
}

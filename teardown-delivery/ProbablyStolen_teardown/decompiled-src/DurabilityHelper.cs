using UnityEngine;

public class DurabilityHelper
{
	public const string DURABILITY_TAG = "DURABILITY_TAG";

	public const string MAX_DURABILITY_TAG = "MAX_DURABILITY_TAG";

	public const string DURABILITY_FAKE_TAG = "DURABILITY_FAKE_TAG";

	public const string MAX_DURABILITY_FAKE_TAG = "MAX_DURABILITY_FAKE_TAG";

	public const string DEFAULT_DURABILITY_TAG = "DEFAULT_DURABILITY_TAG";

	public const string DURABILITY_PRICING_MODE_TAG = "DURABILITY_PRICING_MODE_TAG";

	public const string DURABILITY_ZERO_VALUE_TAG = "DURABILITY_ZERO_VALUE_TAG";

	public const int PRICING_MODE_1 = 1;

	public const int PRICING_MODE_2 = 2;

	public const string DURABILITY_INCONCLUSIVE = "DURABILITY_INCONCLUSIVE";

	public static void InitItemWithDurability(GameItem gameItem, int maxDurability)
	{
		gameItem.EnableTag("DURABILITY_TAG");
		if (!gameItem.IsAlreadyContainFeatureWithId("DURABILITY_TAG"))
		{
			gameItem.AddItemFeature(FeatureDurability());
		}
		SetDurability(gameItem, maxDurability, maxDurability, maxDurability);
	}

	public static void InitItemWithDurabilityPricingMode2(GameItem gameItem, int maxDurability, int valueAtZeroDurability)
	{
		gameItem.EnableTag("DURABILITY_TAG");
		if (!gameItem.IsAlreadyContainFeatureWithId("DURABILITY_TAG"))
		{
			gameItem.AddItemFeature(FeatureDurability());
		}
		gameItem.ModifyTag("DURABILITY_PRICING_MODE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
		gameItem.ModifyTag("DURABILITY_ZERO_VALUE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(valueAtZeroDurability);
		});
		SetDurability(gameItem, maxDurability, maxDurability, maxDurability);
	}

	public static ItemFeature FeatureDurability()
	{
		ItemFeature itemFeature = new ItemFeature();
		itemFeature.identifier = "DURABILITY_TAG";
		itemFeature.category = "DURABILITY_TAG";
		itemFeature.SetPublicDisplay("Durability: ");
		itemFeature.SetActualDisplay("Durability");
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(0);
		return itemFeature;
	}

	public static void SetDurability(GameItem gameItem, int real, int fake, int max, int def)
	{
		if (real == fake)
		{
			SetDurability(gameItem, real, max, def);
			return;
		}
		gameItem.ModifyTag("DURABILITY_TAG", delegate(TagState state)
		{
			state.SetInt(real);
		});
		gameItem.ModifyTag("DURABILITY_FAKE_TAG", delegate(TagState state)
		{
			state.SetInt(fake);
		});
		gameItem.ModifyTag("MAX_DURABILITY_TAG", delegate(TagState state)
		{
			state.SetInt(max);
		});
		ItemFeature itemFeature = gameItem.FindItemFeatureByID("DURABILITY_TAG");
		itemFeature.isFeatureMatch = false;
		itemFeature.SetPreExposedValueMod(GetValueModifier(gameItem, fake, max, def));
		itemFeature.SetValueModifier(GetValueModifier(gameItem, real, max, def));
		itemFeature.SetPublicDisplay(GetDisplayString(fake, max, def));
		itemFeature.SetActualDisplay(GetDisplayString(real, max, def));
	}

	public static Dialogue GetExposeDialog(ItemFeature itemFeature)
	{
		GameItem parentItem = itemFeature.GetParentItem();
		if (parentItem == null)
		{
			return null;
		}
		int realDurability = GetRealDurability(parentItem);
		int fakeDurability = GetFakeDurability(parentItem);
		string text = "";
		string text2 = "";
		if (realDurability > fakeDurability)
		{
			text = "The thing's durability is actually higher";
			text2 = "Really? Thank you for being honest.";
		}
		else
		{
			text = "The thing's durability is actually lower";
			text2 = "Okay, i'll drop the price.";
		}
		Dialogue dialogue = new Dialogue();
		dialogue.SetStartAction(delegate
		{
			itemFeature.ExposeFeature();
		}).SetText("PLAYER", text).NextDialogue()
			.SetText("CLIENT", text2);
		return dialogue;
	}

	public static void SetDurability(GameItem gameItem, int real, int max, int def)
	{
		gameItem.ModifyTag("DURABILITY_TAG", delegate(TagState state)
		{
			state.SetInt(real);
		});
		gameItem.ModifyTag("DURABILITY_FAKE_TAG", delegate(TagState state)
		{
			state.SetInt(real);
		});
		gameItem.ModifyTag("MAX_DURABILITY_TAG", delegate(TagState state)
		{
			state.SetInt(max);
		});
		gameItem.ModifyTag("DEFAULT_DURABILITY_TAG", delegate(TagState state)
		{
			state.SetInt(def);
		});
		ItemFeature itemFeature = gameItem.FindItemFeatureByID("DURABILITY_TAG");
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(GetValueModifier(gameItem, real, max, def));
		itemFeature.SetValueModifier(GetValueModifier(gameItem, real, max, def));
		itemFeature.SetPublicDisplay(GetDisplayString(real, max, def));
		itemFeature.SetActualDisplay(GetDisplayString(real, max, def));
	}

	public static int GetValueModifier(GameItem gameItem, int cur, int max, int def)
	{
		if (gameItem.GetTagReadonly("DURABILITY_PRICING_MODE_TAG").GetInt() == 2)
		{
			return GetValueModifierMode2(gameItem, cur, max);
		}
		return GetValueModifier(cur, max, def);
	}

	public static int GetValueModifier(int cur, int max, int def)
	{
		return -(100 - cur * 100 / max) / 2 - (100 - max * 100 / def) / 2;
	}

	private static int GetValueModifierMode2(GameItem gameItem, int cur, int max)
	{
		float num = gameItem.unitValue;
		if (num <= 0f || max <= 0)
		{
			return 0;
		}
		int @int = gameItem.GetTagReadonly("DURABILITY_ZERO_VALUE_TAG").GetInt();
		float num2 = Mathf.Clamp01((float)cur / (float)max);
		return Mathf.RoundToInt(((float)@int + (num - (float)@int) * num2 - num) / num * 100f);
	}

	public static string GetDisplayString(int cur, int max, int def)
	{
		return LocHelper.GetLocalizedMechanic("mech_durability_display", cur, max);
	}

	public static string GetFakeDisplayString(GameItem gameItem)
	{
		return LocHelper.GetLocalizedMechanic("mech_durability_display", GetFakeDurability(gameItem), GetMaxDurability(gameItem));
	}

	public static string GetRealDisplayString(GameItem gameItem)
	{
		return LocHelper.GetLocalizedMechanic("mech_durability_display", GetRealDurability(gameItem), GetMaxDurability(gameItem));
	}

	public static void ChangeDurability(GameItem gameItem, int amount)
	{
		int realDurability = GetRealDurability(gameItem);
		int fakeDurability = GetFakeDurability(gameItem);
		int maxDurability = GetMaxDurability(gameItem);
		int real = Mathf.Clamp(realDurability + amount, 0, maxDurability);
		int fake = Mathf.Clamp(fakeDurability + amount, 0, maxDurability);
		SetDurability(gameItem, real, fake, maxDurability, GetDefDurability(gameItem));
	}

	public static void SetCurrentDurability(GameItem gameItem, int value)
	{
		int maxDurability = GetMaxDurability(gameItem);
		int real = Mathf.Clamp(value, 0, maxDurability);
		SetDurability(gameItem, real, maxDurability, GetDefDurability(gameItem));
	}

	public static int GetRealDurability(GameItem gameItem)
	{
		return gameItem.GetTagReadonly("DURABILITY_TAG").GetInt();
	}

	public static int GetFakeDurability(GameItem gameItem)
	{
		return gameItem.GetTagReadonly("DURABILITY_FAKE_TAG").GetInt();
	}

	public static int GetMaxDurability(GameItem gameItem)
	{
		return gameItem.GetTagReadonly("MAX_DURABILITY_TAG").GetInt();
	}

	public static int GetDefDurability(GameItem gameItem)
	{
		return gameItem.GetTagReadonly("DEFAULT_DURABILITY_TAG").GetInt();
	}

	public static void CreateIngotTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("DURABILITY_TAG") && item.FindItemFeatureByID("DURABILITY_TAG").IsFeatureDiscovered())
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_tooltip_tested"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			builder.AddLine(GetRealDisplayString(item), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			builder.AddProgressBar((float)GetRealDurability(item) / (float)GetMaxDurability(item), 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
			builder.AddLine("");
		}
	}
}

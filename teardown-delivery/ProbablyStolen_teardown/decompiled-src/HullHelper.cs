public class HullHelper
{
	public const string HULL_INTEGRITY_INT = "HULL_INTEGRITY_INT";

	public const string HULL_INTEGRITY_MAX_INT = "HULL_INTEGRITY_MAX_INT";

	public const string HULL_ARMOR_INT = "HULL_ARMOR_INT";

	public const string HULL_MAINTNANCE_RATE_INT = "HULL_MAINTNANCE_RATE_INT";

	public const int MAKESHIFT_HULL_DEFAULT_INTEGRITY = 50;

	public const int MAKESHIFT_HULL_DEFAULT_ARMOR = -1;

	public const double MAKESHIFT_HULL_DEFAULT_MAINTNANCE_RATE = 1.5;

	public const int CIVILIAN_HULL_DEFAULT_INTEGRITY = 100;

	public const int CIVILIAN_HULL_DEFAULT_ARMOR = 0;

	public const double CIVILIAN_HULL_DEFAULT_MAINTNANCE_RATE = 1.0;

	public const int HEAVY_DUTY_HULL_DEFAULT_INTEGRITY = 125;

	public const int HEAVY_DUTY_HULL_DEFAULT_ARMOR = 1;

	public const double HEAVY_DUTY_HULL_DEFAULT_MAINTNANCE_RATE = 1.0;

	public const int ARMORED_HULL_DEFAULT_INTEGRITY = 150;

	public const int ARMORED_HULL_DEFAULT_ARMOR = 3;

	public const double ARMORED_HULL_DEFAULT_MAINTNANCE_RATE = 0.5;

	public static void InitMakeshiftHull(GameItem item, GameInventory invModule)
	{
		item.EnableTag("SYSTEM_TAG");
		item.EnableTag("SYSTEM_TAG_HULL");
		item.SetType("SYSTEM_TAG_HULL");
		item.ModifyTag("HULL_INTEGRITY_MAX_INT", delegate(TagState state)
		{
			state.Enable().SetInt(50);
		});
		item.ModifyTag("HULL_INTEGRITY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(50);
		});
		item.ModifyTag("HULL_ARMOR_INT", delegate(TagState state)
		{
			state.Enable().SetInt(-1);
		});
		item.ModifyTag("HULL_MAINTNANCE_RATE_INT", delegate(TagState state)
		{
			state.Enable().SetDouble(1.5);
		});
	}

	public static void InitCivilianHull(GameItem item, GameInventory invModule)
	{
		item.EnableTag("SYSTEM_TAG");
		item.EnableTag("SYSTEM_TAG_HULL");
		item.SetType("SYSTEM_TAG_HULL");
		item.ModifyTag("HULL_INTEGRITY_MAX_INT", delegate(TagState state)
		{
			state.Enable().SetInt(100);
		});
		item.ModifyTag("HULL_INTEGRITY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(100);
		});
		item.ModifyTag("HULL_ARMOR_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		item.ModifyTag("HULL_MAINTNANCE_RATE_INT", delegate(TagState state)
		{
			state.Enable().SetDouble(1.0);
		});
	}

	public static void InitHeavyDutyHull(GameItem item, GameInventory invModule)
	{
		item.EnableTag("SYSTEM_TAG");
		item.EnableTag("SYSTEM_TAG_HULL");
		item.SetType("SYSTEM_TAG_HULL");
		item.ModifyTag("HULL_INTEGRITY_MAX_INT", delegate(TagState state)
		{
			state.Enable().SetInt(125);
		});
		item.ModifyTag("HULL_INTEGRITY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(125);
		});
		item.ModifyTag("HULL_ARMOR_INT", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		item.ModifyTag("HULL_MAINTNANCE_RATE_INT", delegate(TagState state)
		{
			state.Enable().SetDouble(1.0);
		});
	}

	public static void InitArmoredHull(GameItem item, GameInventory invModule)
	{
		item.EnableTag("SYSTEM_TAG");
		item.EnableTag("SYSTEM_TAG_HULL");
		item.SetType("SYSTEM_TAG_HULL");
		item.ModifyTag("HULL_INTEGRITY_MAX_INT", delegate(TagState state)
		{
			state.Enable().SetInt(150);
		});
		item.ModifyTag("HULL_INTEGRITY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(150);
		});
		item.ModifyTag("HULL_ARMOR_INT", delegate(TagState state)
		{
			state.Enable().SetInt(3);
		});
		item.ModifyTag("HULL_MAINTNANCE_RATE_INT", delegate(TagState state)
		{
			state.Enable().SetDouble(0.5);
		});
	}

	public static void RepairHull(GameItem item, int repairAmount)
	{
		int current = item.GetTagReadonly("HULL_INTEGRITY_INT").GetInt();
		int @int = item.GetTagReadonly("HULL_INTEGRITY_MAX_INT").GetInt();
		double @double = item.GetTagReadonly("HULL_MAINTNANCE_RATE_INT").GetDouble();
		current = (int)((double)current * @double);
		if (current > @int)
		{
			current = @int;
		}
		item.ModifyTag("HULL_INTEGRITY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(current);
		});
	}

	public static void RemoveMaxHull(GameItem item, int amount)
	{
		int @int = item.GetTagReadonly("HULL_INTEGRITY_INT").GetInt();
		int int2 = item.GetTagReadonly("HULL_INTEGRITY_MAX_INT").GetInt();
		int newMax = int2 - amount;
		if (newMax < 1)
		{
			newMax = 1;
		}
		item.ModifyTag("HULL_INTEGRITY_MAX_INT", delegate(TagState state)
		{
			state.Enable().SetInt(newMax);
		});
		if (newMax < @int)
		{
			item.ModifyTag("HULL_INTEGRITY_INT", delegate(TagState state)
			{
				state.Enable().SetInt(newMax);
			});
		}
	}

	public static bool DamageHull(GameItem item, int damageAmount)
	{
		int current = item.GetTagReadonly("HULL_INTEGRITY_INT").GetInt();
		int @int = item.GetTagReadonly("HULL_ARMOR_INT").GetInt();
		damageAmount -= @int;
		if (damageAmount < 0)
		{
			damageAmount = 0;
		}
		current -= damageAmount;
		if (current <= 0)
		{
			item.ModifyTag("HULL_INTEGRITY_INT", delegate(TagState state)
			{
				state.SetInt(0);
			});
			return true;
		}
		item.ModifyTag("HULL_INTEGRITY_INT", delegate(TagState state)
		{
			state.SetInt(current);
		});
		return false;
	}

	public static void CreateHullSystemTooltip(RichTextBuilder builder, GameItem item)
	{
		int @int = item.GetTagReadonly("HULL_INTEGRITY_INT").GetInt();
		int int2 = item.GetTagReadonly("HULL_INTEGRITY_MAX_INT").GetInt();
		double @double = item.GetTagReadonly("HULL_MAINTNANCE_RATE_INT").GetDouble();
		int int3 = item.GetTagReadonly("HULL_ARMOR_INT").GetInt();
		builder.AddLine(" ", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		builder.Add("Hull Integrity: ", RenderHandler.ColorPalette.LightCyan);
		builder.AddProgressBar((float)@int / (float)int2, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
		builder.AddLine("(" + @int + "/" + (float)int2 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		builder.AddLine("Maintnance Rate: " + @double, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		builder.AddLine("Armor: " + int3, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
	}

	public static bool IsHullIntact(GameItem item)
	{
		int @int = item.GetTagReadonly("HULL_INTEGRITY_INT").GetInt();
		int int2 = item.GetTagReadonly("HULL_INTEGRITY_MAX_INT").GetInt();
		if (@int == int2)
		{
			return true;
		}
		return false;
	}

	public static int GetMissingHull(GameItem item)
	{
		int @int = item.GetTagReadonly("HULL_INTEGRITY_INT").GetInt();
		return item.GetTagReadonly("HULL_INTEGRITY_MAX_INT").GetInt() - @int;
	}
}

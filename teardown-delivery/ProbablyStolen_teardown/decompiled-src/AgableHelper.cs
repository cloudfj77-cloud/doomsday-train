public class AgableHelper
{
	public const string AGE_INT = "AGE_INT";

	public const string AGE_VALUE_MOD_PER_DAY_INT = "AGE_VALUE_MOD_PER_DAY_INT";

	public const int DEFAULT_AGE_VALUE_MOD_PER_DAY = 3;

	public static void InitAgable(GameItem item, int valueModPerDay = 3)
	{
		item.ModifyTag("AGE_VALUE_MOD_PER_DAY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(valueModPerDay);
		});
	}

	public static bool IsAgable(GameItem item)
	{
		return item.IsTag("AGE_VALUE_MOD_PER_DAY_INT");
	}

	public static bool HasAged(GameItem item)
	{
		return item.IsTag("AGE_INT");
	}

	public static int GetAge(GameItem item)
	{
		return item.GetTagReadonly("AGE_INT").GetInt();
	}

	public static void OnAge(GameItem item)
	{
		item.ModifyTag("AGE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(state.GetInt() + 1);
		});
		int num = (item.IsTag("AGE_VALUE_MOD_PER_DAY_INT") ? item.GetTagReadonly("AGE_VALUE_MOD_PER_DAY_INT").GetInt() : 3);
		item.unitValue += num;
	}

	public static void ResetAge(GameItem item)
	{
		item.ModifyTag("AGE_INT", delegate(TagState state)
		{
			state.Disable().SetInt(0);
		});
	}

	public static void AddAgingInfoTooltipLine(RichTextBuilder builder)
	{
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_aging_item_tooltip"), ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange, bold: false, italic: true);
	}

	public static void AddAgeTooltipLine(RichTextBuilder builder, GameItem item, string locKey = "mech_wine_age")
	{
		int age = GetAge(item);
		builder.AddLine(LocHelper.GetLocalizedMechanic(locKey, age), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
	}
}

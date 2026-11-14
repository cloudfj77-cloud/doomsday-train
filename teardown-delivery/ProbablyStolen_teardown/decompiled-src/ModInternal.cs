public class ModInternal
{
	public static void CreateTooltipEarly(RichTextBuilder builder, GameItem item)
	{
		ModHook.FireOnCreateTooltipEarly(builder, item);
	}

	public static void CreateDevTooltipEarly(RichTextBuilder builder, GameItem item)
	{
		ModHook.FireOnCreateDevTooltipEarly(builder, item);
	}

	public static void CreateTooltipLate(RichTextBuilder builder, GameItem item)
	{
		ModHook.FireOnCreateTooltipLate(builder, item);
	}

	public static void CreateDevTooltipLate(RichTextBuilder builder, GameItem item)
	{
		ModHook.FireOnCreateDevTooltipLate(builder, item);
	}
}

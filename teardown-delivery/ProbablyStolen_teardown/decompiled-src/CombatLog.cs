public class CombatLog
{
	public static bool IsVerbose;

	public static void log(string message, RenderHandler.ColorPalette color = RenderHandler.ColorPalette.VeryLightGray)
	{
		OverlayHandler.current.AppendLog(new RichTextBuilder().Add(message, color));
	}

	public static void slog(string message)
	{
		RichTextBuilder richTextBuilder = new RichTextBuilder();
		OverlayHandler.current.AppendLog(richTextBuilder.Add(message, RenderHandler.ColorPalette.Red, " "));
	}

	public static void vlog(string message)
	{
		if (IsVerbose)
		{
			log(message);
		}
	}
}

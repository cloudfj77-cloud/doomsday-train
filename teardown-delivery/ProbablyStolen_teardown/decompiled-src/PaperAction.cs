using System;

public static class PaperAction
{
	public const string CLOSE = "close";

	public const string ZOOM = "zoom";

	public const string FLIP_LEFT = "flip_left";

	public const string FLIP_RIGHT = "flip_right";

	public const string PAGE_INDICATOR = "page_indicator";

	public static readonly string[] Chrome = new string[5] { "close", "zoom", "flip_left", "flip_right", "page_indicator" };

	public static bool IsChrome(string action)
	{
		return Array.IndexOf(Chrome, action) >= 0;
	}
}

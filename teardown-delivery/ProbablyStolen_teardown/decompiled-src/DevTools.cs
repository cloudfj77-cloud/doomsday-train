using System.IO;
using UnityEngine;

public static class DevTools
{
	public const string MARKER_FILE = "dev.txt";

	public const string MARKER_FILE_FULL = "1dev.txt";

	public static readonly string[] MARKER_FILES = new string[2] { "dev.txt", "1dev.txt" };

	private static bool? enabled;

	private static bool? fullEnabled;

	public static bool IsEnabled
	{
		get
		{
			if (!enabled.HasValue)
			{
				enabled = IsFullEnabled || Application.isEditor || File.Exists(MarkerPath("dev.txt"));
				if (enabled.Value)
				{
					UnityEngine.Debug.Log("[DevTools] Developer tools enabled" + (IsFullEnabled ? " (full)" : "") + ": F9 debug panel and F8 console.");
				}
				else
				{
					UnityEngine.Debug.Log("[DevTools] Developer tools disabled: no dev.txt next to the executable (" + MarkerPath("dev.txt") + ").");
				}
			}
			return enabled.Value;
		}
	}

	public static bool IsFullEnabled
	{
		get
		{
			if (!fullEnabled.HasValue)
			{
				fullEnabled = Application.isEditor || File.Exists(MarkerPath("1dev.txt"));
			}
			return fullEnabled.Value;
		}
	}

	public static string MarkerPath(string markerFile)
	{
		return Path.GetFullPath(Path.Combine(Application.dataPath, "..", markerFile));
	}
}

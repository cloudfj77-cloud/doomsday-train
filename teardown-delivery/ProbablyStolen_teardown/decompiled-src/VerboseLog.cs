using UnityEngine;

public static class VerboseLog
{
	public static bool IsVerbose { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void ResetUnityLoggerFilter()
	{
		UnityEngine.Debug.unityLogger.filterLogType = LogType.Log;
	}

	public static void Toggle()
	{
		Set(!IsVerbose);
	}

	public static void Set(bool verbose)
	{
		IsVerbose = verbose;
		UnityEngine.Debug.Log("Verbose log: " + (verbose ? "on" : "off"));
	}
}

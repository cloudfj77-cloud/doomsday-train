using System;
using System.Diagnostics;
using UnityEngine;

internal static class Debug
{
	public static ILogger unityLogger => UnityEngine.Debug.unityLogger;

	public static bool isDebugBuild => UnityEngine.Debug.isDebugBuild;

	[HideInCallstack]
	public static void Log(object message)
	{
		if (VerboseLog.IsVerbose)
		{
			UnityEngine.Debug.Log(message);
		}
	}

	[HideInCallstack]
	public static void Log(object message, UnityEngine.Object context)
	{
		if (VerboseLog.IsVerbose)
		{
			UnityEngine.Debug.Log(message, context);
		}
	}

	[HideInCallstack]
	public static void LogFormat(string format, params object[] args)
	{
		if (VerboseLog.IsVerbose)
		{
			UnityEngine.Debug.LogFormat(format, args);
		}
	}

	[HideInCallstack]
	public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (VerboseLog.IsVerbose)
		{
			UnityEngine.Debug.LogFormat(context, format, args);
		}
	}

	[HideInCallstack]
	public static void LogWarning(object message)
	{
		UnityEngine.Debug.LogWarning(message);
	}

	[HideInCallstack]
	public static void LogWarning(object message, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogWarning(message, context);
	}

	[HideInCallstack]
	public static void LogWarningFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogWarningFormat(format, args);
	}

	[HideInCallstack]
	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
		UnityEngine.Debug.LogWarningFormat(context, format, args);
	}

	[HideInCallstack]
	public static void LogError(object message)
	{
		UnityEngine.Debug.LogError(message);
	}

	[HideInCallstack]
	public static void LogError(object message, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogError(message, context);
	}

	[HideInCallstack]
	public static void LogErrorFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogErrorFormat(format, args);
	}

	[HideInCallstack]
	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
		UnityEngine.Debug.LogErrorFormat(context, format, args);
	}

	[HideInCallstack]
	public static void LogException(Exception exception)
	{
		UnityEngine.Debug.LogException(exception);
	}

	[HideInCallstack]
	public static void LogException(Exception exception, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogException(exception, context);
	}

	[HideInCallstack]
	public static void LogAssertion(object message)
	{
	}

	[HideInCallstack]
	public static void LogAssertion(object message, UnityEngine.Object context)
	{
	}

	[HideInCallstack]
	[Conditional("UNITY_ASSERTIONS")]
	public static void Assert(bool condition)
	{
	}

	[HideInCallstack]
	[Conditional("UNITY_ASSERTIONS")]
	public static void Assert(bool condition, object message)
	{
	}

	[HideInCallstack]
	[Conditional("UNITY_ASSERTIONS")]
	public static void Assert(bool condition, string message)
	{
	}

	[HideInCallstack]
	[Conditional("UNITY_ASSERTIONS")]
	public static void Assert(bool condition, UnityEngine.Object context)
	{
	}

	[HideInCallstack]
	[Conditional("UNITY_ASSERTIONS")]
	public static void Assert(bool condition, object message, UnityEngine.Object context)
	{
	}

	public static void DrawLine(Vector3 start, Vector3 end)
	{
		UnityEngine.Debug.DrawLine(start, end);
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0f, bool depthTest = true)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	public static void DrawRay(Vector3 start, Vector3 dir)
	{
		UnityEngine.Debug.DrawRay(start, dir);
	}

	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration = 0f, bool depthTest = true)
	{
		UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}

	public static void Break()
	{
		UnityEngine.Debug.Break();
	}
}

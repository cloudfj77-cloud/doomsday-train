using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using Steamworks;
using UnityEngine;

public static class ModSteamGuard
{
	public const string HARMONY_ID = "dse.steamguard";

	private static Harmony harmony;

	private static HarmonyMethod prefix;

	private static HarmonyMethod processPrefix;

	private static readonly List<MethodBase> guarded = new List<MethodBase>();

	private static readonly List<MethodBase> guardedProcess = new List<MethodBase>();

	private static readonly ConcurrentQueue<(string modId, string call, bool process)> pending = new ConcurrentQueue<(string, string, bool)>();

	private static readonly HashSet<string> reported = new HashSet<string>();

	private static int blockedCount;

	public static int GuardedCount => guarded.Count + guardedProcess.Count;

	public static int BlockedCount => blockedCount;

	public static void Install()
	{
		if (harmony != null)
		{
			return;
		}
		harmony = new Harmony("dse.steamguard");
		ModSteamAuth.Install();
		prefix = new HarmonyMethod(AccessTools.Method(typeof(ModSteamGuard), "Prefix"))
		{
			priority = 800
		};
		processPrefix = new HarmonyMethod(AccessTools.Method(typeof(ModSteamGuard), "PrefixProcess"))
		{
			priority = 800
		};
		Stopwatch stopwatch = Stopwatch.StartNew();
		MethodInfo[] methods = typeof(Process).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		foreach (MethodInfo methodInfo in methods)
		{
			if (!(methodInfo.Name != "Start"))
			{
				try
				{
					harmony.Patch(methodInfo, processPrefix);
					guardedProcess.Add(methodInfo);
				}
				catch (Exception ex)
				{
					Debug.LogWarning($"[ModSteamGuard] Could not guard Process.Start({methodInfo.GetParameters().Length} args): {ex.Message}");
				}
			}
		}
		Assembly assembly = typeof(SteamUGC).Assembly;
		string[] runtimeGuardedTypes = ModSteamRules.RuntimeGuardedTypes;
		foreach (string text in runtimeGuardedTypes)
		{
			Type type = assembly.GetType("Steamworks." + text);
			if (type == null)
			{
				continue;
			}
			methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
			foreach (MethodInfo methodInfo2 in methods)
			{
				if (ModSteamRules.IsGuardedAtRuntime(text, methodInfo2.Name))
				{
					try
					{
						harmony.Patch(methodInfo2, prefix);
						guarded.Add(methodInfo2);
					}
					catch (Exception ex2)
					{
						Debug.LogWarning("[ModSteamGuard] Could not guard " + text + "." + methodInfo2.Name + ": " + ex2.Message);
					}
				}
			}
		}
		UnityEngine.Debug.Log($"[ModSteamGuard] {guarded.Count} Steam method(s) and {guardedProcess.Count} Process.Start overload(s) guarded in {stopwatch.ElapsedMilliseconds} ms");
	}

	public static void Verify()
	{
		if (harmony == null)
		{
			return;
		}
		foreach (MethodBase item in guarded.Concat(guardedProcess))
		{
			Patches patchInfo = Harmony.GetPatchInfo(item);
			if (patchInfo == null || !patchInfo.Prefixes.Any((Patch p) => p.owner == "dse.steamguard"))
			{
				Debug.LogError("[ModSteamGuard] The guard on " + item.DeclaringType?.Name + "." + item.Name + " was removed; reinstalling it.");
				try
				{
					harmony.Patch(item, guardedProcess.Contains(item) ? processPrefix : prefix);
				}
				catch (Exception ex)
				{
					Debug.LogError("[ModSteamGuard] Could not reinstall the guard on " + item.Name + ": " + ex.Message);
				}
			}
		}
	}

	public static void Drain()
	{
		(string, string, bool) result;
		while (pending.TryDequeue(out result))
		{
			if (reported.Add((result.Item1 ?? "?") + "|" + result.Item2))
			{
				ModLoader.OnSteamGuardTripped(result.Item1, result.Item2, result.Item3);
			}
		}
	}

	private static bool Prefix(MethodBase __originalMethod)
	{
		if (ModSteamAuth.IsIssuing && __originalMethod != null && __originalMethod.Name == "GetAuthSessionTicket" && __originalMethod.DeclaringType == typeof(SteamUser))
		{
			return true;
		}
		string item = (__originalMethod?.DeclaringType?.Name ?? "Steam") + "." + (__originalMethod?.Name ?? "?");
		Interlocked.Increment(ref blockedCount);
		string item2 = null;
		try
		{
			item2 = CallingMod();
		}
		catch
		{
		}
		pending.Enqueue((item2, item, false));
		return false;
	}

	private static bool PrefixProcess(MethodBase __originalMethod)
	{
		string text = null;
		try
		{
			text = CallingMod();
		}
		catch
		{
		}
		if (text == null)
		{
			return true;
		}
		Interlocked.Increment(ref blockedCount);
		pending.Enqueue((text, "Process.Start", true));
		return false;
	}

	private static string CallingMod()
	{
		return ModLoader.CallingModId();
	}
}

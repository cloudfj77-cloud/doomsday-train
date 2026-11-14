using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Steamworks;
using UnityEngine;

internal static class ModSteamAuth
{
	private sealed class Pending
	{
		public string modId;

		public string label;

		public HAuthTicket handle;

		public byte[] ticket;

		public Action<byte[]> onResult;

		public float deadline;

		public bool confirmed;
	}

	private const int MAX_TICKET_BYTES = 1024;

	private const float TIMEOUT_SECONDS = 20f;

	[ThreadStatic]
	private static bool issuing;

	private static int mainThreadId = -1;

	private static Callback<GetAuthSessionTicketResponse_t> response;

	private static readonly Dictionary<string, Pending> byMod = new Dictionary<string, Pending>(StringComparer.Ordinal);

	internal static bool IsIssuing => issuing;

	private static bool OnMainThread
	{
		get
		{
			if (mainThreadId >= 0)
			{
				return Thread.CurrentThread.ManagedThreadId == mainThreadId;
			}
			return false;
		}
	}

	internal static void Install()
	{
		if (mainThreadId < 0)
		{
			mainThreadId = Thread.CurrentThread.ManagedThreadId;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal static bool Request(string serverIdentity, Action<byte[]> onTicket)
	{
		string modId = ModLoader.CallingModId();
		if (modId == null)
		{
			UnityEngine.Debug.LogError("[ModSteamAuth] RequestSteamAuthTicket was called by code that belongs to no mod; refused.");
			return false;
		}
		LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId);
		string text = ((loadedMod != null) ? ModLoader.GetModLabel(loadedMod.Manifest) : modId);
		if (loadedMod == null || !loadedMod.IsEnabled)
		{
			UnityEngine.Debug.LogError("[" + text + "] RequestSteamAuthTicket refused: the mod is not running.");
			return false;
		}
		if (loadedMod.Manifest.Capabilities == null || !loadedMod.Manifest.Capabilities.Contains("steam-auth"))
		{
			UnityEngine.Debug.LogError("[" + text + "] RequestSteamAuthTicket refused: the manifest does not declare <Capability>steam-auth</Capability>.");
			return false;
		}
		if (!OnMainThread)
		{
			UnityEngine.Debug.LogError("[" + text + "] RequestSteamAuthTicket refused: call it on the main thread.");
			return false;
		}
		if (onTicket == null)
		{
			UnityEngine.Debug.LogError("[" + text + "] RequestSteamAuthTicket refused: onTicket is null.");
			return false;
		}
		SteamNetworkingIdentity pSteamNetworkingIdentity = default(SteamNetworkingIdentity);
		if (string.IsNullOrEmpty(serverIdentity) || !pSteamNetworkingIdentity.ParseString(serverIdentity) || (pSteamNetworkingIdentity.m_eType != ESteamNetworkingIdentityType.k_ESteamNetworkingIdentityType_SteamID && pSteamNetworkingIdentity.m_eType != ESteamNetworkingIdentityType.k_ESteamNetworkingIdentityType_IPAddress))
		{
			UnityEngine.Debug.LogError("[" + text + "] RequestSteamAuthTicket refused: the server identity must be \"steamid:<id>\" or \"ip:<address>:<port>\", got \"" + serverIdentity + "\".");
			return false;
		}
		if (!SteamManager.Initialized)
		{
			UnityEngine.Debug.LogWarning("[" + text + "] RequestSteamAuthTicket: Steam is not running, no ticket.");
			return false;
		}
		CancelFor(modId);
		byte[] array = new byte[1024];
		issuing = true;
		HAuthTicket authSessionTicket;
		uint pcbTicket;
		try
		{
			authSessionTicket = SteamUser.GetAuthSessionTicket(array, array.Length, out pcbTicket, ref pSteamNetworkingIdentity);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("[" + text + "] RequestSteamAuthTicket: Steam threw " + ex.GetType().Name + ": " + ex.Message);
			return false;
		}
		finally
		{
			issuing = false;
		}
		if (authSessionTicket == HAuthTicket.Invalid || pcbTicket == 0 || pcbTicket > array.Length)
		{
			UnityEngine.Debug.LogWarning("[" + text + "] RequestSteamAuthTicket: Steam gave no ticket.");
			return false;
		}
		byte[] array2 = new byte[pcbTicket];
		Array.Copy(array, array2, pcbTicket);
		if (response == null)
		{
			response = Callback<GetAuthSessionTicketResponse_t>.Create(OnResponse);
		}
		byMod[modId] = new Pending
		{
			modId = modId,
			label = text,
			handle = authSessionTicket,
			ticket = array2,
			onResult = onTicket,
			deadline = Time.unscaledTime + 20f
		};
		UnityEngine.Debug.Log("[" + text + "] Steam auth ticket requested for " + serverIdentity);
		return true;
	}

	private static void OnResponse(GetAuthSessionTicketResponse_t r)
	{
		Pending pending = null;
		foreach (Pending value in byMod.Values)
		{
			if (!value.confirmed && value.handle == r.m_hAuthTicket)
			{
				pending = value;
				break;
			}
		}
		if (pending != null)
		{
			pending.confirmed = true;
			if (r.m_eResult == EResult.k_EResultOK)
			{
				Answer(pending, pending.ticket);
				return;
			}
			UnityEngine.Debug.LogWarning($"[{pending.label}] Steam refused the auth ticket: {r.m_eResult}");
			byMod.Remove(pending.modId);
			CancelTicket(pending);
			Answer(pending, null);
		}
	}

	internal static void Update()
	{
		if (byMod.Count == 0)
		{
			return;
		}
		List<Pending> list = null;
		float unscaledTime = Time.unscaledTime;
		foreach (Pending value in byMod.Values)
		{
			if (!value.confirmed && unscaledTime >= value.deadline)
			{
				(list ?? (list = new List<Pending>())).Add(value);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (Pending item in list)
		{
			UnityEngine.Debug.LogWarning($"[{item.label}] Steam did not confirm the auth ticket within {20f:0} seconds; given up.");
			item.confirmed = true;
			byMod.Remove(item.modId);
			CancelTicket(item);
			Answer(item, null);
		}
	}

	internal static void Cancel()
	{
		string text = ModLoader.CallingModId();
		if (text != null)
		{
			if (!OnMainThread)
			{
				UnityEngine.Debug.LogWarning("[" + text + "] CancelSteamAuthTicket ignored: call it on the main thread.");
			}
			else
			{
				CancelFor(text);
			}
		}
	}

	internal static void CancelFor(string modId)
	{
		if (modId != null && byMod.TryGetValue(modId, out var value))
		{
			byMod.Remove(modId);
			value.onResult = null;
			CancelTicket(value);
		}
	}

	internal static void CancelAll()
	{
		if (byMod.Count == 0)
		{
			return;
		}
		foreach (string item in new List<string>(byMod.Keys))
		{
			CancelFor(item);
		}
	}

	private static void CancelTicket(Pending p)
	{
		if (p.handle == HAuthTicket.Invalid)
		{
			return;
		}
		HAuthTicket handle = p.handle;
		p.handle = HAuthTicket.Invalid;
		if (!SteamManager.Initialized)
		{
			return;
		}
		try
		{
			SteamUser.CancelAuthTicket(handle);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogWarning("[" + p.label + "] CancelAuthTicket threw: " + ex.Message);
		}
	}

	private static void Answer(Pending p, byte[] ticket)
	{
		Action<byte[]> onResult = p.onResult;
		p.onResult = null;
		if (onResult == null)
		{
			return;
		}
		try
		{
			onResult(ticket);
		}
		catch (Exception arg)
		{
			UnityEngine.Debug.LogError($"[{p.label}] The RequestSteamAuthTicket callback threw: {arg}");
		}
	}
}

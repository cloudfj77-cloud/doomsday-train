using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;
using UnityEngine;

public static class ModWorkshop
{
	public struct ItemStats
	{
		public DateTime createdUtc;

		public uint votesUp;

		public uint votesDown;

		public uint Ratings => votesUp + votesDown;
	}

	[Serializable]
	private class OwnerStore
	{
		public List<string> entries = new List<string>();
	}

	public const uint MAIN_APP_ID = 4348910u;

	public const string HUB_URL = "https://steamcommunity.com/app/4348910/workshop/";

	private static readonly HashSet<ulong> pendingDownloads = new HashSet<ulong>();

	private static Callback<ItemInstalled_t> itemInstalledCallback;

	private static Callback<DownloadItemResult_t> downloadResultCallback;

	private static CallResult<RemoteStorageUnsubscribePublishedFileResult_t> unsubscribeResult;

	private static Action<bool> pendingUnsubscribe;

	private static CallResult<SteamUGCQueryCompleted_t> detailsResult;

	private static readonly Queue<PublishedFileId_t[]> detailsBatches = new Queue<PublishedFileId_t[]>();

	private static PublishedFileId_t[] detailsInFlight;

	private static readonly HashSet<ulong> detailsPending = new HashSet<ulong>();

	private static readonly HashSet<ulong> detailsDone = new HashSet<ulong>();

	private static readonly Dictionary<ulong, string> previewUrls = new Dictionary<ulong, string>();

	private static readonly Dictionary<ulong, ItemStats> itemStats = new Dictionary<ulong, ItemStats>();

	public const string OWNERS_ES3_KEY = "workshopOwners";

	private static Dictionary<ulong, ulong> owners;

	private static bool ownersDirty;

	public static int PendingDownloads => pendingDownloads.Count;

	public static bool IsAvailable => SteamManager.Initialized;

	public static event Action<ulong> OnItemInstalled;

	public static event Action OnDownloadsChanged;

	public static event Action OnDetailsUpdated;

	public static string ItemPageUrl(ulong publishedFileId)
	{
		return $"https://steamcommunity.com/sharedfiles/filedetails/?id={publishedFileId}";
	}

	public static List<(ulong id, string folder)> GetInstalledItems()
	{
		List<(ulong, string)> list = new List<(ulong, string)>();
		if (!IsAvailable)
		{
			return list;
		}
		uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
		if (numSubscribedItems == 0)
		{
			return list;
		}
		PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
		uint subscribedItems = SteamUGC.GetSubscribedItems(array, numSubscribedItems);
		for (int i = 0; i < subscribedItems; i++)
		{
			PublishedFileId_t nPublishedFileID = array[i];
			uint itemState = SteamUGC.GetItemState(nPublishedFileID);
			if ((itemState & 4u) != 0 && SteamUGC.GetItemInstallInfo(nPublishedFileID, out var _, out var pchFolder, 1024u, out var _) && System.IO.Directory.Exists(pchFolder))
			{
				list.Add((nPublishedFileID.m_PublishedFileId, pchFolder));
				continue;
			}
			if ((itemState & 0x30) == 0)
			{
				SteamUGC.DownloadItem(nPublishedFileID, bHighPriority: true);
			}
			pendingDownloads.Add(nPublishedFileID.m_PublishedFileId);
			UnityEngine.Debug.Log($"[ModWorkshop] Item {nPublishedFileID.m_PublishedFileId} is subscribed but not installed yet; downloading.");
		}
		return list;
	}

	public static void EnsureCallbacks()
	{
		if (IsAvailable && itemInstalledCallback == null)
		{
			itemInstalledCallback = Callback<ItemInstalled_t>.Create(OnSteamItemInstalled);
			downloadResultCallback = Callback<DownloadItemResult_t>.Create(OnSteamDownloadResult);
		}
	}

	private static void OnSteamItemInstalled(ItemInstalled_t data)
	{
		if (data.m_unAppID.m_AppId == 4348910)
		{
			UnityEngine.Debug.Log($"[ModWorkshop] Item {data.m_nPublishedFileId.m_PublishedFileId} installed (restart required to load it).");
			ModLoader.MarkRestartRequired();
			ModWorkshop.OnItemInstalled?.Invoke(data.m_nPublishedFileId.m_PublishedFileId);
		}
	}

	private static void OnSteamDownloadResult(DownloadItemResult_t data)
	{
		if (data.m_unAppID.m_AppId == 4348910)
		{
			if (data.m_eResult != EResult.k_EResultOK)
			{
				Debug.LogWarning($"[ModWorkshop] Download of item {data.m_nPublishedFileId.m_PublishedFileId} failed: {data.m_eResult}");
			}
			if (pendingDownloads.Remove(data.m_nPublishedFileId.m_PublishedFileId))
			{
				ModWorkshop.OnDownloadsChanged?.Invoke();
			}
		}
	}

	public static void Unsubscribe(ulong publishedFileId, Action<bool> done)
	{
		if (!IsAvailable)
		{
			done?.Invoke(obj: false);
			return;
		}
		if (pendingUnsubscribe != null)
		{
			Debug.LogWarning("[ModWorkshop] An unsubscribe is already in flight");
			done?.Invoke(obj: false);
			return;
		}
		pendingUnsubscribe = done;
		if (unsubscribeResult == null)
		{
			unsubscribeResult = CallResult<RemoteStorageUnsubscribePublishedFileResult_t>.Create(OnUnsubscribed);
		}
		SteamAPICall_t hAPICall = SteamUGC.UnsubscribeItem(new PublishedFileId_t(publishedFileId));
		unsubscribeResult.Set(hAPICall);
	}

	private static void OnUnsubscribed(RemoteStorageUnsubscribePublishedFileResult_t data, bool ioFailure)
	{
		bool flag = !ioFailure && data.m_eResult == EResult.k_EResultOK;
		if (!flag)
		{
			Debug.LogWarning(string.Format("[ModWorkshop] Unsubscribe of {0} failed: {1}", data.m_nPublishedFileId.m_PublishedFileId, ioFailure ? "IO failure" : data.m_eResult.ToString()));
		}
		Action<bool> action = pendingUnsubscribe;
		pendingUnsubscribe = null;
		action?.Invoke(flag);
	}

	public static void RequestDetails(IEnumerable<ulong> publishedFileIds)
	{
		if (!IsAvailable)
		{
			return;
		}
		List<PublishedFileId_t> list = new List<PublishedFileId_t>();
		foreach (ulong publishedFileId in publishedFileIds)
		{
			if (publishedFileId != 0L && !detailsDone.Contains(publishedFileId) && detailsPending.Add(publishedFileId))
			{
				list.Add(new PublishedFileId_t(publishedFileId));
			}
		}
		for (int i = 0; i < list.Count; i += 50)
		{
			detailsBatches.Enqueue(list.GetRange(i, Math.Min(50, list.Count - i)).ToArray());
		}
		SendNextDetailsBatch();
	}

	public static bool IsDetailsPending(ulong publishedFileId)
	{
		return detailsPending.Contains(publishedFileId);
	}

	public static bool HasDetails(ulong publishedFileId)
	{
		return detailsDone.Contains(publishedFileId);
	}

	public static bool TryGetPreviewUrl(ulong publishedFileId, out string url)
	{
		return previewUrls.TryGetValue(publishedFileId, out url);
	}

	public static bool TryGetStats(ulong publishedFileId, out ItemStats stats)
	{
		return itemStats.TryGetValue(publishedFileId, out stats);
	}

	private static void EnsureOwners()
	{
		if (owners != null)
		{
			return;
		}
		owners = new Dictionary<ulong, ulong>();
		try
		{
			if (!ES3.KeyExists("workshopOwners"))
			{
				return;
			}
			OwnerStore ownerStore = JsonUtility.FromJson<OwnerStore>(ES3.Load<string>("workshopOwners"));
			if (ownerStore?.entries == null)
			{
				return;
			}
			foreach (string entry in ownerStore.entries)
			{
				string[] array = entry.Split('|');
				if (array.Length == 2 && ulong.TryParse(array[0], out var result) && ulong.TryParse(array[1], out var result2))
				{
					owners[result] = result2;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[ModWorkshop] Could not read the owner cache: " + ex.Message);
		}
	}

	private static void SaveOwners()
	{
		try
		{
			OwnerStore ownerStore = new OwnerStore();
			foreach (KeyValuePair<ulong, ulong> owner in owners)
			{
				ownerStore.entries.Add(owner.Key + "|" + owner.Value);
			}
			ES3.Save("workshopOwners", JsonUtility.ToJson(ownerStore));
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[ModWorkshop] Could not save the owner cache: " + ex.Message);
		}
	}

	public static bool TryGetOwner(ulong publishedFileId, out ulong ownerSteamId)
	{
		EnsureOwners();
		return owners.TryGetValue(publishedFileId, out ownerSteamId);
	}

	private static void SendNextDetailsBatch()
	{
		if (detailsInFlight != null || detailsBatches.Count == 0)
		{
			return;
		}
		detailsInFlight = detailsBatches.Dequeue();
		UGCQueryHandle_t uGCQueryHandle_t = SteamUGC.CreateQueryUGCDetailsRequest(detailsInFlight, (uint)detailsInFlight.Length);
		if (uGCQueryHandle_t == UGCQueryHandle_t.Invalid)
		{
			FinishDetailsBatch(ok: false);
			return;
		}
		SteamAPICall_t steamAPICall_t = SteamUGC.SendQueryUGCRequest(uGCQueryHandle_t);
		if (steamAPICall_t == SteamAPICall_t.Invalid)
		{
			SteamUGC.ReleaseQueryUGCRequest(uGCQueryHandle_t);
			FinishDetailsBatch(ok: false);
			return;
		}
		if (detailsResult == null)
		{
			detailsResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnDetailsReceived);
		}
		detailsResult.Set(steamAPICall_t);
	}

	private static void OnDetailsReceived(SteamUGCQueryCompleted_t data, bool ioFailure)
	{
		bool flag = !ioFailure && data.m_eResult == EResult.k_EResultOK;
		if (flag)
		{
			for (uint num = 0u; num < data.m_unNumResultsReturned; num++)
			{
				if (SteamUGC.GetQueryUGCResult(data.m_handle, num, out var pDetails))
				{
					if (SteamUGC.GetQueryUGCPreviewURL(data.m_handle, num, out var pchURL, 256u) && !string.IsNullOrEmpty(pchURL))
					{
						previewUrls[pDetails.m_nPublishedFileId.m_PublishedFileId] = pchURL;
					}
					if (pDetails.m_eResult == EResult.k_EResultOK && pDetails.m_rtimeCreated != 0)
					{
						itemStats[pDetails.m_nPublishedFileId.m_PublishedFileId] = new ItemStats
						{
							createdUtc = DateTimeOffset.FromUnixTimeSeconds(pDetails.m_rtimeCreated).UtcDateTime,
							votesUp = pDetails.m_unVotesUp,
							votesDown = pDetails.m_unVotesDown
						};
					}
					if (pDetails.m_ulSteamIDOwner != 0L)
					{
						EnsureOwners();
						owners[pDetails.m_nPublishedFileId.m_PublishedFileId] = pDetails.m_ulSteamIDOwner;
						ownersDirty = true;
					}
				}
			}
			if (ownersDirty)
			{
				ownersDirty = false;
				SaveOwners();
			}
		}
		else
		{
			Debug.LogWarning("[ModWorkshop] Item details query failed: " + (ioFailure ? "IO failure" : data.m_eResult.ToString()));
		}
		SteamUGC.ReleaseQueryUGCRequest(data.m_handle);
		FinishDetailsBatch(flag);
	}

	private static void FinishDetailsBatch(bool ok)
	{
		PublishedFileId_t[] array = detailsInFlight;
		for (int i = 0; i < array.Length; i++)
		{
			PublishedFileId_t publishedFileId_t = array[i];
			detailsPending.Remove(publishedFileId_t.m_PublishedFileId);
			if (ok)
			{
				detailsDone.Add(publishedFileId_t.m_PublishedFileId);
			}
		}
		detailsInFlight = null;
		ModWorkshop.OnDetailsUpdated?.Invoke();
		SendNextDetailsBatch();
	}

	public static void OpenItemPage(ulong publishedFileId)
	{
		OpenUrl(ItemPageUrl(publishedFileId));
	}

	public static void OpenHub()
	{
		OpenUrl("https://steamcommunity.com/app/4348910/workshop/");
	}

	private static void OpenUrl(string url)
	{
		if (SteamManager.Initialized && SteamUtils.IsOverlayEnabled())
		{
			SteamFriends.ActivateGameOverlayToWebPage(url);
		}
		else
		{
			Application.OpenURL(url);
		}
	}
}

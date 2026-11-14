using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public static class ModTrust
{
	[Serializable]
	public class StoppedEntry
	{
		public string id;

		public string hash;

		public string reason;

		public string at;
	}

	[Serializable]
	private class Store
	{
		public List<ModTrustRecord> records = new List<ModTrustRecord>();

		public List<StoppedEntry> stopped = new List<StoppedEntry>();
	}

	public const string ES3_KEY = "modTrust";

	private static Store store = new Store();

	private static bool loaded;

	public static IReadOnlyList<ModTrustRecord> Records
	{
		get
		{
			EnsureLoaded();
			return store.records;
		}
	}

	public static IReadOnlyList<StoppedEntry> Stopped
	{
		get
		{
			EnsureLoaded();
			return store.stopped;
		}
	}

	public static void EnsureLoaded()
	{
		if (loaded)
		{
			return;
		}
		loaded = true;
		try
		{
			if (ES3.KeyExists("modTrust"))
			{
				Store store = JsonUtility.FromJson<Store>(ES3.Load<string>("modTrust"));
				if (store != null && store.records != null)
				{
					ModTrust.store = store;
				}
				Store store2 = ModTrust.store;
				if (store2.stopped == null)
				{
					store2.stopped = new List<StoppedEntry>();
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[ModTrust] Could not read the trust store, starting empty: " + ex.Message);
			ModTrust.store = new Store();
		}
	}

	private static void Save()
	{
		ES3.Save("modTrust", JsonUtility.ToJson(store));
	}

	public static ModTrustRecord Get(string id)
	{
		EnsureLoaded();
		return store.records.Find((ModTrustRecord r) => r.id == id);
	}

	public static ModTrustRecord Approve(LoadedMod mod, ModFingerprint fingerprint, IEnumerable<string> detected)
	{
		EnsureLoaded();
		ModTrustRecord modTrustRecord = Get(mod.Manifest.ID);
		string text = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
		ModTrustRecord record = new ModTrustRecord
		{
			id = mod.Manifest.ID,
			source = mod.Source.ToString(),
			workshopId = mod.WorkshopId.ToString(CultureInfo.InvariantCulture),
			manifestHash = fingerprint.ManifestHash,
			files = fingerprint.FileEntries(),
			combinedHash = fingerprint.Combined,
			declared = ModCapability.Order(mod.Manifest.Capabilities).ToList(),
			detected = ModCapability.Order(detected).ToList(),
			gameVersion = Application.version,
			approvedAt = text,
			updatedAt = text,
			developerMode = (modTrustRecord?.developerMode ?? false),
			scannerVersion = 3
		};
		store.records.RemoveAll((ModTrustRecord r) => r.id == record.id);
		store.records.Add(record);
		Save();
		return record;
	}

	public static void Refresh(ModTrustRecord record, ModFingerprint fingerprint, IEnumerable<string> detected)
	{
		record.manifestHash = fingerprint.ManifestHash;
		record.files = fingerprint.FileEntries();
		record.combinedHash = fingerprint.Combined;
		if (detected != null)
		{
			record.detected = ModCapability.Order(detected).ToList();
		}
		record.updatedAt = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
		record.scannerVersion = 3;
		Save();
	}

	public static void MarkStopped(string id, string hash, string reason)
	{
		EnsureLoaded();
		if (!string.IsNullOrEmpty(hash) && !store.stopped.Exists((StoppedEntry s) => s.hash == hash))
		{
			store.stopped.Add(new StoppedEntry
			{
				id = id,
				hash = hash,
				reason = reason,
				at = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)
			});
			Save();
		}
	}

	public static bool IsStopped(string hash, out StoppedEntry entry)
	{
		EnsureLoaded();
		entry = (string.IsNullOrEmpty(hash) ? null : store.stopped.Find((StoppedEntry s) => s.hash == hash));
		return entry != null;
	}

	public static bool Revoke(string id)
	{
		EnsureLoaded();
		int num = store.records.RemoveAll((ModTrustRecord r) => r.id == id);
		if (num > 0)
		{
			Save();
		}
		return num > 0;
	}

	public static bool SetDeveloperMode(string id, bool on)
	{
		ModTrustRecord modTrustRecord = Get(id);
		if (modTrustRecord == null)
		{
			return false;
		}
		modTrustRecord.developerMode = on;
		Save();
		return true;
	}

	public static ModTrustState CompareFiles(ModTrustRecord record, ModFingerprint fingerprint, ModSource source, ulong workshopId)
	{
		if (record == null)
		{
			return ModTrustState.Unknown;
		}
		if (record.source != source.ToString())
		{
			return ModTrustState.Changed;
		}
		if (record.workshopId != workshopId.ToString(CultureInfo.InvariantCulture))
		{
			return ModTrustState.Changed;
		}
		if (record.combinedHash == fingerprint.Combined)
		{
			return ModTrustState.Match;
		}
		return ModTrustState.Updated;
	}

	public static bool CapabilitiesWithin(ModTrustRecord record, IEnumerable<string> detected)
	{
		return ModCapability.Undeclared(detected, record.detected).Count == 0;
	}
}

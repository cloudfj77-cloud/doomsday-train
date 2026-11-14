using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class ModTrustList
{
	[Serializable]
	public class BlockedEntry
	{
		public string id;

		public string hash;

		public string reason;
	}

	[Serializable]
	public class VerifiedEntry
	{
		public string id;

		public string hash;
	}

	[Serializable]
	public class WorkshopEntry
	{
		public string id;

		public string reason;
	}

	[Serializable]
	public class PublisherEntry
	{
		public string steamId;

		public string reason;
	}

	[Serializable]
	public class FileEntry
	{
		public string hash;

		public string reason;
	}

	[Serializable]
	public class AuthorEntry
	{
		public string author;

		public string reason;
	}

	[Serializable]
	public class KillSwitch
	{
		public bool allMods;

		public bool exceptVerified;

		public string message;
	}

	[Serializable]
	public class Payload
	{
		public string issued;

		public List<BlockedEntry> blocked = new List<BlockedEntry>();

		public List<VerifiedEntry> verified = new List<VerifiedEntry>();

		public List<WorkshopEntry> blockedWorkshopItems = new List<WorkshopEntry>();

		public List<PublisherEntry> blockedPublishers = new List<PublisherEntry>();

		public List<FileEntry> blockedFiles = new List<FileEntry>();

		public List<AuthorEntry> blockedAuthors = new List<AuthorEntry>();

		public KillSwitch killSwitch = new KillSwitch();
	}

	[Serializable]
	private class Envelope
	{
		public string payload;

		public string signature;
	}

	public const string URL = "https://raw.githubusercontent.com/dragonpan2/probably-stolen-modding/main/trust-list.json";

	private const string PUBLIC_KEY_MODULUS_BASE64 = "td0YND9zDm46UuHrDQWwWahdYhsHkMOEXSxZXxPUtF1j/eQO77GMWE0Lp3FTKd1IbW6CPq1lsKJtSRs8I9Hbl4IUWg24c5jyVDM0wpymOCzajaKeKSPeh5ChVP9y1+KPgovKghWdHHyzeWBhGsNC//Y24HA2Sjkt8wezZTaNh8UeRS2oVwUGyH64YFUO8XYSnQIVXkEtP2tdpxTwPLwQHyp9dBEoY+Bs06WqKouqWpmAlABpsVl/fU9rPSaOLmJChCouh4ZKWNDRrmkgPAKN1oNAY9JfkCdK+O9zl6c1UjFujJvTa5ix3zf+Z7ymYuNlC6D9Jk5tK7GjbDrTo660Ew==";

	private const int FETCH_TIMEOUT_SECONDS = 5;

	private const int STALE_AFTER_DAYS = 30;

	private static Payload current;

	private static bool cacheLoaded;

	private static bool fetching;

	public static bool IsConfigured => !string.IsNullOrEmpty("td0YND9zDm46UuHrDQWwWahdYhsHkMOEXSxZXxPUtF1j/eQO77GMWE0Lp3FTKd1IbW6CPq1lsKJtSRs8I9Hbl4IUWg24c5jyVDM0wpymOCzajaKeKSPeh5ChVP9y1+KPgovKghWdHHyzeWBhGsNC//Y24HA2Sjkt8wezZTaNh8UeRS2oVwUGyH64YFUO8XYSnQIVXkEtP2tdpxTwPLwQHyp9dBEoY+Bs06WqKouqWpmAlABpsVl/fU9rPSaOLmJChCouh4ZKWNDRrmkgPAKN1oNAY9JfkCdK+O9zl6c1UjFujJvTa5ix3zf+Z7ymYuNlC6D9Jk5tK7GjbDrTo660Ew==");

	public static bool HasList => current != null;

	public static DateTime? Issued
	{
		get
		{
			if (current == null)
			{
				return null;
			}
			return ParseIssued(current.issued);
		}
	}

	public static bool IsUnsignedDevList { get; private set; }

	public static string LastFetchError { get; private set; }

	private static string CachePath => Path.Combine(Application.persistentDataPath, "mod-trust-list.json");

	public static bool KillSwitchActive
	{
		get
		{
			if (current?.killSwitch != null)
			{
				return current.killSwitch.allMods;
			}
			return false;
		}
	}

	public static string KillSwitchMessage => current?.killSwitch?.message ?? "";

	public static bool IsStale
	{
		get
		{
			if (!IsConfigured)
			{
				return false;
			}
			DateTime? issued = Issued;
			if (!issued.HasValue)
			{
				return LastFetchError != null;
			}
			return (DateTime.UtcNow - issued.Value).TotalDays > 30.0;
		}
	}

	public static event Action OnChanged;

	public static void LoadCache()
	{
		if (cacheLoaded)
		{
			return;
		}
		cacheLoaded = true;
		if (!IsConfigured)
		{
			UnityEngine.Debug.Log("[ModTrustList] No public key configured; the remote list is disabled.");
			return;
		}
		try
		{
			if (File.Exists(CachePath))
			{
				if (TryParse(File.ReadAllText(CachePath), requireSignature: true, out var payload, out var error))
				{
					current = payload;
					UnityEngine.Debug.Log(string.Format("[ModTrustList] Cached list issued {0}: {1} blocked, {2} verified{3}", payload.issued, payload.blocked.Count, payload.verified.Count, (payload.killSwitch != null && payload.killSwitch.allMods) ? ", kill switch ON" : ""));
				}
				else
				{
					Debug.LogWarning("[ModTrustList] Cached list rejected (" + error + "); deleting it.");
					File.Delete(CachePath);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[ModTrustList] Could not read the cached list: " + ex.Message);
		}
	}

	public static IEnumerator Fetch()
	{
		if (!IsConfigured || fetching)
		{
			yield break;
		}
		fetching = true;
		using UnityWebRequest request = UnityWebRequest.Get("https://raw.githubusercontent.com/dragonpan2/probably-stolen-modding/main/trust-list.json");
		request.timeout = 5;
		yield return request.SendWebRequest();
		fetching = false;
		if (request.result != UnityWebRequest.Result.Success)
		{
			LastFetchError = request.error;
			UnityEngine.Debug.Log("[ModTrustList] Fetch failed (" + request.error + "); keeping the cached list.");
			yield break;
		}
		Apply(request.downloadHandler.text, fromNetwork: true);
	}

	private static void Apply(string json, bool fromNetwork)
	{
		if (!TryParse(json, requireSignature: true, out var payload, out var error))
		{
			LastFetchError = error;
			Debug.LogWarning("[ModTrustList] Fetched list rejected: " + error);
			return;
		}
		DateTime? dateTime = ParseIssued(payload.issued);
		if (!dateTime.HasValue)
		{
			LastFetchError = "missing issued date";
			Debug.LogWarning("[ModTrustList] Fetched list has no valid 'issued' date; ignored.");
			return;
		}
		if (current != null)
		{
			DateTime? issued = Issued;
			if (issued.HasValue)
			{
				DateTime valueOrDefault = issued.GetValueOrDefault();
				if (dateTime.Value <= valueOrDefault)
				{
					UnityEngine.Debug.Log("[ModTrustList] Fetched list is not newer than the cached one; nothing to do.");
					LastFetchError = null;
					return;
				}
			}
		}
		try
		{
			File.WriteAllText(CachePath, json);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[ModTrustList] Could not cache the list: " + ex.Message);
		}
		current = payload;
		IsUnsignedDevList = false;
		LastFetchError = null;
		UnityEngine.Debug.Log(string.Format("[ModTrustList] {0} list issued {1}: {2} blocked, {3} verified{4}", fromNetwork ? "Fetched" : "Loaded", payload.issued, payload.blocked.Count, payload.verified.Count, (payload.killSwitch != null && payload.killSwitch.allMods) ? ", kill switch ON" : ""));
		ModTrustList.OnChanged?.Invoke();
	}

	public static bool LoadUnsigned(string path, out string error)
	{
		error = null;
		if (PlayerStore.Instance == null || !PlayerStore.Instance.devMode)
		{
			error = "devMode is off";
			return false;
		}
		try
		{
			Payload payload = JsonUtility.FromJson<Payload>(File.ReadAllText(path));
			if (payload == null)
			{
				error = "not a payload";
				return false;
			}
			Normalize(payload);
			current = payload;
			IsUnsignedDevList = true;
			ModTrustList.OnChanged?.Invoke();
			return true;
		}
		catch (Exception ex)
		{
			error = ex.Message;
			return false;
		}
	}

	public static ModListVerdict Check(string id, ModFingerprint fingerprint, ulong workshopId, ulong ownerSteamId, out string reason)
	{
		return Check(id, null, fingerprint, workshopId, ownerSteamId, out reason);
	}

	public static ModListVerdict Check(string id, string author, ModFingerprint fingerprint, ulong workshopId, ulong ownerSteamId, out string reason)
	{
		reason = null;
		if (current == null || fingerprint == null)
		{
			return ModListVerdict.None;
		}
		string combined = fingerprint.Combined;
		foreach (FileEntry blockedFile in current.blockedFiles)
		{
			if (blockedFile == null || string.IsNullOrEmpty(blockedFile.hash))
			{
				continue;
			}
			foreach (var file in fingerprint.Files)
			{
				string item = file.hash;
				if (string.Equals(blockedFile.hash, item, StringComparison.OrdinalIgnoreCase))
				{
					reason = blockedFile.reason ?? "";
					return ModListVerdict.Blocked;
				}
			}
		}
		string text = AuthorKey(author);
		if (text.Length > 0)
		{
			foreach (AuthorEntry blockedAuthor in current.blockedAuthors)
			{
				if (blockedAuthor != null && !(AuthorKey(blockedAuthor.author) != text))
				{
					reason = blockedAuthor.reason ?? "";
					return ModListVerdict.Blocked;
				}
			}
		}
		foreach (BlockedEntry item2 in current.blocked)
		{
			if (item2 != null && !(item2.id != id) && (item2.hash == "*" || string.Equals(item2.hash, combined, StringComparison.OrdinalIgnoreCase)))
			{
				reason = item2.reason ?? "";
				return ModListVerdict.Blocked;
			}
		}
		if (workshopId != 0L)
		{
			string text2 = workshopId.ToString(CultureInfo.InvariantCulture);
			foreach (WorkshopEntry blockedWorkshopItem in current.blockedWorkshopItems)
			{
				if (blockedWorkshopItem != null && !(blockedWorkshopItem.id != text2))
				{
					reason = blockedWorkshopItem.reason ?? "";
					return ModListVerdict.Blocked;
				}
			}
		}
		if (ownerSteamId != 0L)
		{
			string text3 = ownerSteamId.ToString(CultureInfo.InvariantCulture);
			foreach (PublisherEntry blockedPublisher in current.blockedPublishers)
			{
				if (blockedPublisher != null && !(blockedPublisher.steamId != text3))
				{
					reason = blockedPublisher.reason ?? "";
					return ModListVerdict.Blocked;
				}
			}
		}
		foreach (VerifiedEntry item3 in current.verified)
		{
			if (item3 != null && item3.id == id && string.Equals(item3.hash, combined, StringComparison.OrdinalIgnoreCase))
			{
				return ModListVerdict.Verified;
			}
		}
		return ModListVerdict.None;
	}

	public static string AuthorKey(string author)
	{
		if (string.IsNullOrEmpty(author))
		{
			return "";
		}
		string text = author;
		try
		{
			text = author.Normalize(NormalizationForm.FormKC);
		}
		catch (Exception)
		{
		}
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		string text2 = text;
		foreach (char c in text2)
		{
			if (!char.IsWhiteSpace(c) && char.GetUnicodeCategory(c) != UnicodeCategory.Format)
			{
				stringBuilder.Append(char.ToLowerInvariant(c));
			}
		}
		return stringBuilder.ToString();
	}

	public static bool IsPaused(ModListVerdict verdict)
	{
		if (current?.killSwitch == null || !current.killSwitch.allMods)
		{
			return false;
		}
		if (current.killSwitch.exceptVerified)
		{
			return verdict != ModListVerdict.Verified;
		}
		return true;
	}

	private static bool TryParse(string json, bool requireSignature, out Payload payload, out string error)
	{
		payload = null;
		error = null;
		Envelope envelope;
		try
		{
			envelope = JsonUtility.FromJson<Envelope>(json);
		}
		catch (Exception ex)
		{
			error = "not JSON: " + ex.Message;
			return false;
		}
		if (envelope == null || string.IsNullOrEmpty(envelope.payload))
		{
			error = "no payload";
			return false;
		}
		byte[] array;
		try
		{
			array = Convert.FromBase64String(envelope.payload);
		}
		catch (FormatException)
		{
			error = "payload is not base64";
			return false;
		}
		if (requireSignature)
		{
			if (string.IsNullOrEmpty(envelope.signature))
			{
				error = "no signature";
				return false;
			}
			byte[] signature;
			try
			{
				signature = Convert.FromBase64String(envelope.signature);
			}
			catch (FormatException)
			{
				error = "signature is not base64";
				return false;
			}
			if (!Verify(array, signature))
			{
				error = "signature does not verify";
				return false;
			}
		}
		try
		{
			payload = JsonUtility.FromJson<Payload>(Encoding.UTF8.GetString(array));
		}
		catch (Exception ex4)
		{
			error = "payload is not a list: " + ex4.Message;
			return false;
		}
		if (payload == null)
		{
			error = "empty payload";
			return false;
		}
		Normalize(payload);
		return true;
	}

	private static void Normalize(Payload payload)
	{
		Payload payload2 = payload;
		if (payload2.blocked == null)
		{
			payload2.blocked = new List<BlockedEntry>();
		}
		payload2 = payload;
		if (payload2.verified == null)
		{
			payload2.verified = new List<VerifiedEntry>();
		}
		payload2 = payload;
		if (payload2.blockedWorkshopItems == null)
		{
			payload2.blockedWorkshopItems = new List<WorkshopEntry>();
		}
		payload2 = payload;
		if (payload2.blockedPublishers == null)
		{
			payload2.blockedPublishers = new List<PublisherEntry>();
		}
		payload2 = payload;
		if (payload2.blockedFiles == null)
		{
			payload2.blockedFiles = new List<FileEntry>();
		}
		payload2 = payload;
		if (payload2.blockedAuthors == null)
		{
			payload2.blockedAuthors = new List<AuthorEntry>();
		}
		payload2 = payload;
		if (payload2.killSwitch == null)
		{
			payload2.killSwitch = new KillSwitch();
		}
	}

	private static bool Verify(byte[] data, byte[] signature)
	{
		try
		{
			using RSA rSA = RSA.Create();
			rSA.ImportParameters(new RSAParameters
			{
				Modulus = Convert.FromBase64String("td0YND9zDm46UuHrDQWwWahdYhsHkMOEXSxZXxPUtF1j/eQO77GMWE0Lp3FTKd1IbW6CPq1lsKJtSRs8I9Hbl4IUWg24c5jyVDM0wpymOCzajaKeKSPeh5ChVP9y1+KPgovKghWdHHyzeWBhGsNC//Y24HA2Sjkt8wezZTaNh8UeRS2oVwUGyH64YFUO8XYSnQIVXkEtP2tdpxTwPLwQHyp9dBEoY+Bs06WqKouqWpmAlABpsVl/fU9rPSaOLmJChCouh4ZKWNDRrmkgPAKN1oNAY9JfkCdK+O9zl6c1UjFujJvTa5ix3zf+Z7ymYuNlC6D9Jk5tK7GjbDrTo660Ew=="),
				Exponent = new byte[3] { 1, 0, 1 }
			});
			return rSA.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[ModTrustList] Signature check failed to run: " + ex.Message);
			return false;
		}
	}

	private static DateTime? ParseIssued(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (!DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var result))
		{
			return null;
		}
		return result;
	}

	public static string Describe()
	{
		if (!IsConfigured)
		{
			return "remote list disabled (no public key)";
		}
		if (current == null)
		{
			return "no list yet" + ((LastFetchError != null) ? (" (last fetch: " + LastFetchError + ")") : "");
		}
		return string.Format("issued {0}, {1} blocked, {2} blocked Workshop item(s), {3} blocked publisher(s), {4} blocked file(s), {5} blocked author(s), {6} verified, kill switch {7}{8}{9}", current.issued, current.blocked.Count, current.blockedWorkshopItems.Count, current.blockedPublishers.Count, current.blockedFiles.Count, current.blockedAuthors.Count, current.verified.Count, KillSwitchActive ? "ON" : "off", IsUnsignedDevList ? ", UNSIGNED DEV LIST" : "", (LastFetchError != null) ? (", last fetch: " + LastFetchError) : "");
	}
}

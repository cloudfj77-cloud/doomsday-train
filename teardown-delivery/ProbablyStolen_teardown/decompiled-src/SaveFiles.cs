using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public static class SaveFiles
{
	private class HistoryFileOnDisk
	{
		public string file;

		public string absPath;

		public int dayCount;

		public DateTime stamp;
	}

	public class RepairResult
	{
		public int slotId;

		public string runName;

		public int promotedDay = -1;

		public int quarantined;
	}

	public const string Index = "saves_index.es3";

	private const string HISTORY_KEY_PREFIX = "history_";

	private const string HISTORY_STAMP_FORMAT = "yyyyMMdd-HHmmss";

	private static readonly Regex HistoryStem = new Regex("^save_(\\d+)_day(\\d+)_(\\d{8}-\\d{6})$");

	private static bool s_healingIndex = false;

	private const int IO_RETRY_ATTEMPTS = 3;

	private const int IO_RETRY_DELAY_MS = 150;

	private const int INTEGRITY_PROBE_BYTES = 4096;

	private const int HEAD_FIELDS_BYTES = 262144;

	private static readonly Regex HeadCash = new Regex("\"playerCash\":(-?\\d+)");

	private static readonly Regex HeadVersion = new Regex("\"saveVersion\":\"([^\"]*)\"");

	private static readonly List<RepairResult> recentRepairs = new List<RepairResult>();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void RepairSettingsFileAtStartup()
	{
		try
		{
			RepairSettingsFile();
		}
		catch (Exception ex)
		{
			Debug.LogError("[SaveFiles] Settings file check failed: " + ex.Message);
		}
	}

	public static bool RepairSettingsFile()
	{
		string abs = new ES3Settings().FullPath;
		if (!File.Exists(abs))
		{
			return false;
		}
		string reason = null;
		bool flag = !IsSaveFileIntact(abs, out reason);
		if (!flag)
		{
			try
			{
				ES3.KeyExists("__settings_integrity_probe__");
			}
			catch (Exception ex)
			{
				flag = true;
				reason = ex.Message;
			}
		}
		if (!flag)
		{
			return false;
		}
		string quarantined = abs + ".corrupt-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
		WithIORetry(delegate
		{
			if (File.Exists(quarantined))
			{
				File.Delete(quarantined);
			}
			File.Move(abs, quarantined);
		});
		Debug.LogWarning("[SaveFiles] The settings file was unreadable (" + reason + "); moved it to " + quarantined + " and continuing with default settings");
		return true;
	}

	public static string Slot(int slotId)
	{
		return $"save_{slotId}.es3";
	}

	public static string HistoryFile(int slotId, int dayCount, DateTime stamp)
	{
		return string.Format("save_{0}_day{1}_{2}.es3", slotId, dayCount, stamp.ToString("yyyyMMdd-HHmmss"));
	}

	public static bool IsHistoryStem(string stem, out int slotId)
	{
		int dayCount;
		DateTime stamp;
		return TryParseHistoryName(stem, out slotId, out dayCount, out stamp);
	}

	public static bool TryParseHistoryName(string stem, out int slotId, out int dayCount, out DateTime stamp)
	{
		slotId = -1;
		dayCount = 0;
		stamp = DateTime.MinValue;
		Match match = HistoryStem.Match(stem);
		if (!match.Success)
		{
			return false;
		}
		slotId = int.Parse(match.Groups[1].Value);
		dayCount = int.Parse(match.Groups[2].Value);
		DateTime.TryParseExact(match.Groups[3].Value, "yyyyMMdd-HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out stamp);
		return true;
	}

	public static string AbsolutePath(string relativeName)
	{
		return Path.Combine(Application.persistentDataPath, relativeName);
	}

	public static List<SaveHistoryEntry> LoadHistory(int slotId)
	{
		List<SaveHistoryEntry> list = IndexLoad("history_" + slotId, new List<SaveHistoryEntry>()) ?? new List<SaveHistoryEntry>();
		int count = list.Count;
		list.RemoveAll((SaveHistoryEntry e) => e == null || string.IsNullOrEmpty(e.file) || !File.Exists(AbsolutePath(e.file)));
		list.Sort((SaveHistoryEntry a, SaveHistoryEntry b) => b.saveDateTime.CompareTo(a.saveDateTime));
		if (list.Count != count)
		{
			SaveHistory(slotId, list);
		}
		return list;
	}

	private static void SaveHistory(int slotId, List<SaveHistoryEntry> history)
	{
		if (history.Count == 0)
		{
			IndexDeleteKey("history_" + slotId);
		}
		else
		{
			IndexSave("history_" + slotId, history);
		}
	}

	public static string RotateIntoHistory(int slotId)
	{
		string text = AbsolutePath(Slot(slotId));
		if (!File.Exists(text))
		{
			return null;
		}
		SavePreviewData savePreviewData = IndexLoad<SavePreviewData>("preview_" + slotId, null);
		DateTime dateTime = ((savePreviewData != null && savePreviewData.lastSaveDateTime.Year > 1) ? savePreviewData.lastSaveDateTime : File.GetLastWriteTime(text));
		int dayCount = savePreviewData?.dayCount ?? 0;
		string historyName = HistoryFile(slotId, dayCount, dateTime);
		string text2 = AbsolutePath(historyName);
		if (File.Exists(text2))
		{
			File.Delete(text2);
		}
		File.Move(text, text2);
		List<SaveHistoryEntry> list = LoadHistory(slotId);
		list.RemoveAll((SaveHistoryEntry e) => e.file == historyName);
		list.Insert(0, new SaveHistoryEntry
		{
			file = historyName,
			dayCount = dayCount,
			credit = (savePreviewData?.credit ?? 0),
			saveDateTime = dateTime,
			version = ((savePreviewData != null) ? savePreviewData.version : "")
		});
		TrimHistory(list, EmporiumOption.SaveHistoryDays - 1);
		SaveHistory(slotId, list);
		return historyName;
	}

	private static void TrimHistory(List<SaveHistoryEntry> history, int keepCount)
	{
		keepCount = Mathf.Max(0, keepCount);
		while (history.Count > keepCount)
		{
			SaveHistoryEntry saveHistoryEntry = history[history.Count - 1];
			history.RemoveAt(history.Count - 1);
			try
			{
				string abs = AbsolutePath(saveHistoryEntry.file);
				if (File.Exists(abs))
				{
					WithIORetry(delegate
					{
						File.Delete(abs);
					});
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[SaveFiles] Could not delete old history file " + saveHistoryEntry.file + ": " + ex.Message);
			}
		}
	}

	public static void RestoreFromHistory(int slotId, string historyName)
	{
		if (string.IsNullOrEmpty(historyName))
		{
			return;
		}
		try
		{
			string latest = AbsolutePath(Slot(slotId));
			string historyAbs = AbsolutePath(historyName);
			if (File.Exists(historyAbs) && !File.Exists(latest))
			{
				WithIORetry(delegate
				{
					File.Move(historyAbs, latest);
				});
			}
			List<SaveHistoryEntry> list = LoadHistory(slotId);
			list.RemoveAll((SaveHistoryEntry e) => e.file == historyName);
			SaveHistory(slotId, list);
		}
		catch (Exception ex)
		{
			Debug.LogError($"[SaveFiles] Could not restore {historyName} as the latest save of slot {slotId}: {ex.Message}");
		}
	}

	public static string ResolveLoadFile(NewGameData data)
	{
		if (data == null)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(data.saveFile))
		{
			if (File.Exists(AbsolutePath(data.saveFile)))
			{
				return data.saveFile;
			}
			Debug.LogError($"[SaveFiles] Earlier day {data.saveFile} is missing, loading the latest day of slot {data.saveSlot} instead");
			data.saveFile = null;
		}
		return Slot(data.saveSlot);
	}

	public static int DuplicateSlot(int slotId, string copyName)
	{
		string source = AbsolutePath(Slot(slotId));
		if (!File.Exists(source))
		{
			return -1;
		}
		SavePreviewData savePreviewData = IndexLoad<SavePreviewData>("preview_" + slotId, null);
		int newSlotId = NextFreeSlotId();
		try
		{
			WithIORetry(delegate
			{
				File.Copy(source, AbsolutePath(Slot(newSlotId)), overwrite: false);
			});
		}
		catch (Exception ex)
		{
			Debug.LogError($"[SaveFiles] Could not duplicate slot {slotId}: {ex.Message}");
			return -1;
		}
		List<SaveHistoryEntry> list = LoadHistory(slotId);
		List<SaveHistoryEntry> list2 = new List<SaveHistoryEntry>(list.Count);
		foreach (SaveHistoryEntry entry in list)
		{
			string copiedName = HistoryFile(newSlotId, entry.dayCount, entry.saveDateTime);
			try
			{
				WithIORetry(delegate
				{
					File.Copy(AbsolutePath(entry.file), AbsolutePath(copiedName), overwrite: true);
				});
				list2.Add(new SaveHistoryEntry
				{
					file = copiedName,
					dayCount = entry.dayCount,
					credit = entry.credit,
					saveDateTime = entry.saveDateTime,
					version = entry.version
				});
			}
			catch (Exception ex2)
			{
				Debug.LogWarning($"[SaveFiles] Earlier day {entry.file} was not copied with slot {slotId}: {ex2.Message}");
			}
		}
		if (list2.Count > 0)
		{
			SaveHistory(newSlotId, list2);
		}
		SavePreviewData savePreviewData2 = ((savePreviewData != null) ? savePreviewData.CloneForSlot(newSlotId) : BuildPreviewFromStore(ReadStoreForPreview(new ES3Settings(AbsolutePath(Slot(newSlotId)), ES3.Location.File)), newSlotId, AbsolutePath(Slot(newSlotId))));
		savePreviewData2.customName = copyName;
		RegisterSlot(newSlotId, savePreviewData2);
		Debug.Log($"[SaveFiles] Duplicated slot {slotId} as slot {newSlotId} with {list2.Count} earlier day(s)");
		return newSlotId;
	}

	public static void RenameSlot(int slotId, string name)
	{
		SavePreviewData savePreviewData = IndexLoad<SavePreviewData>("preview_" + slotId, null);
		if (savePreviewData != null)
		{
			savePreviewData.customName = (string.IsNullOrWhiteSpace(name) ? null : name.Trim());
			IndexSave("preview_" + slotId, savePreviewData);
			SaveUIManager.UpdateCacheEntry(slotId, savePreviewData);
		}
	}

	public static void WithIORetry(Action fileOperation)
	{
		int num = 0;
		while (true)
		{
			try
			{
				fileOperation();
				break;
			}
			catch (IOException ex) when (num < 3)
			{
				Debug.LogWarning($"[SaveFiles] File locked ({ex.Message}), retry {num + 1}/{3}");
				Thread.Sleep(150);
			}
			num++;
		}
	}

	public static T IndexLoad<T>(string key, T defaultValue)
	{
		try
		{
			T result = defaultValue;
			WithIORetry(delegate
			{
				result = ES3.Load(key, "saves_index.es3", defaultValue);
			});
			return result;
		}
		catch (IOException ex)
		{
			Debug.LogError("[SaveFiles] Index read of '" + key + "' skipped, file stayed locked: " + ex.Message);
			return defaultValue;
		}
		catch (Exception cause)
		{
			if (!TryHealIndex(cause))
			{
				return defaultValue;
			}
			try
			{
				return ES3.Load(key, "saves_index.es3", defaultValue);
			}
			catch (Exception ex2)
			{
				Debug.LogError("[SaveFiles] Index read of '" + key + "' failed even after rebuild: " + ex2.Message);
				return defaultValue;
			}
		}
	}

	public static void IndexSave<T>(string key, T value)
	{
		try
		{
			WithIORetry(delegate
			{
				ES3.Save(key, value, "saves_index.es3");
			});
		}
		catch (IOException ex)
		{
			Debug.LogError("[SaveFiles] Index write of '" + key + "' skipped, file stayed locked: " + ex.Message);
		}
		catch (Exception cause)
		{
			if (!TryHealIndex(cause))
			{
				return;
			}
			try
			{
				ES3.Save(key, value, "saves_index.es3");
			}
			catch (Exception ex2)
			{
				Debug.LogError("[SaveFiles] Index write of '" + key + "' failed even after rebuild: " + ex2.Message);
			}
		}
	}

	public static void IndexDeleteKey(string key)
	{
		try
		{
			WithIORetry(delegate
			{
				ES3.DeleteKey(key, "saves_index.es3");
			});
		}
		catch (IOException ex)
		{
			Debug.LogError("[SaveFiles] Index delete of '" + key + "' skipped, file stayed locked: " + ex.Message);
		}
		catch (Exception cause)
		{
			if (!TryHealIndex(cause))
			{
				return;
			}
			try
			{
				ES3.DeleteKey(key, "saves_index.es3");
			}
			catch
			{
			}
		}
	}

	private static bool TryHealIndex(Exception cause)
	{
		if (s_healingIndex)
		{
			return false;
		}
		s_healingIndex = true;
		try
		{
			Debug.LogError("[SaveFiles] Index file is unreadable (" + cause.Message + "). Quarantining and rebuilding it.");
			if (!QuarantineFile("saves_index.es3"))
			{
				return false;
			}
			SaveUIManager.InvalidateCache();
			ScanAndRegisterOrphans();
			return true;
		}
		catch (Exception arg)
		{
			Debug.LogError($"[SaveFiles] Index rebuild failed: {arg}");
			return false;
		}
		finally
		{
			s_healingIndex = false;
		}
	}

	public static bool QuarantineFile(string relativeName)
	{
		if (string.IsNullOrEmpty(relativeName) || Path.GetFileName(relativeName) != relativeName)
		{
			Debug.LogError("[SaveFiles] QuarantineFile takes a file name inside the save folder, not '" + relativeName + "'");
			return false;
		}
		string abs = Path.Combine(Application.persistentDataPath, relativeName);
		if (!File.Exists(abs))
		{
			return true;
		}
		string quarantined = abs + ".corrupt-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
		try
		{
			WithIORetry(delegate
			{
				if (File.Exists(quarantined))
				{
					File.Delete(quarantined);
				}
				File.Move(abs, quarantined);
			});
			Debug.LogWarning("[SaveFiles] Quarantined unreadable file to " + quarantined);
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError("[SaveFiles] Could not quarantine " + relativeName + ": " + ex.Message);
			return false;
		}
	}

	public static bool IsSaveFileIntact(string absPath, out string reason)
	{
		reason = null;
		try
		{
			FileInfo fileInfo = new FileInfo(absPath);
			if (!fileInfo.Exists)
			{
				reason = "missing";
				return false;
			}
			if (fileInfo.Length == 0L)
			{
				reason = "empty";
				return false;
			}
			byte[] array = new byte[4096];
			using (FileStream fileStream = new FileStream(absPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				int count = fileStream.Read(array, 0, array.Length);
				int num = FirstContentByte(array, count);
				if (num != 123)
				{
					reason = ((num < 0) ? "no content" : ((num == 0) ? "zero-filled" : "not a save file"));
					return false;
				}
				fileStream.Seek(Math.Max(0L, fileInfo.Length - array.Length), SeekOrigin.Begin);
				count = fileStream.Read(array, 0, array.Length);
				int num2 = LastContentByte(array, count);
				if (num2 != 125)
				{
					reason = ((num2 == 0) ? "cut short, the end is zero-filled" : "cut short");
					return false;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			reason = ex.Message;
			return false;
		}
	}

	private static bool IsJsonWhitespace(byte b)
	{
		if (b != 32 && b != 9 && b != 13)
		{
			return b == 10;
		}
		return true;
	}

	private static int FirstContentByte(byte[] buffer, int count)
	{
		int i = 0;
		if (count >= 3 && buffer[0] == 239 && buffer[1] == 187 && buffer[2] == 191)
		{
			i = 3;
		}
		for (; i < count; i++)
		{
			if (!IsJsonWhitespace(buffer[i]))
			{
				return buffer[i];
			}
		}
		return -1;
	}

	private static int LastContentByte(byte[] buffer, int count)
	{
		for (int num = count - 1; num >= 0; num--)
		{
			if (!IsJsonWhitespace(buffer[num]))
			{
				return buffer[num];
			}
		}
		return -1;
	}

	private static void ReadHeadFields(string absPath, out int credit, out string version)
	{
		credit = 0;
		version = "";
		try
		{
			byte[] array = new byte[262144];
			int count;
			using (FileStream fileStream = new FileStream(absPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				count = fileStream.Read(array, 0, array.Length);
			}
			string @string = Encoding.UTF8.GetString(array, 0, count);
			Match match = HeadCash.Match(@string);
			if (match.Success)
			{
				int.TryParse(match.Groups[1].Value, out credit);
			}
			Match match2 = HeadVersion.Match(@string);
			if (match2.Success)
			{
				version = match2.Groups[1].Value;
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[SaveFiles] Could not read the head of " + absPath + ": " + ex.Message);
		}
	}

	private static List<HistoryFileOnDisk> FindHistoryFilesOnDisk(int slotId)
	{
		List<HistoryFileOnDisk> list = new List<HistoryFileOnDisk>();
		string[] files;
		try
		{
			files = System.IO.Directory.GetFiles(Application.persistentDataPath, $"save_{slotId}_day*.es3");
		}
		catch (Exception ex)
		{
			Debug.LogWarning($"[SaveFiles] Could not list the earlier days of slot {slotId}: {ex.Message}");
			return list;
		}
		string[] array = files;
		foreach (string text in array)
		{
			if (TryParseHistoryName(Path.GetFileNameWithoutExtension(text), out var slotId2, out var dayCount, out var stamp) && slotId2 == slotId)
			{
				list.Add(new HistoryFileOnDisk
				{
					file = Path.GetFileName(text),
					absPath = text,
					dayCount = dayCount,
					stamp = stamp
				});
			}
		}
		list.Sort(delegate(HistoryFileOnDisk a, HistoryFileOnDisk b)
		{
			int num = b.stamp.CompareTo(a.stamp);
			return (num == 0) ? b.dayCount.CompareTo(a.dayCount) : num;
		});
		return list;
	}

	private static void RestoreHistoryFromDisk(int slotId)
	{
		List<SaveHistoryEntry> list = LoadHistory(slotId);
		List<HistoryFileOnDisk> list2 = FindHistoryFilesOnDisk(slotId);
		if (list2.Count == 0)
		{
			return;
		}
		List<SaveHistoryEntry> list3 = new List<SaveHistoryEntry>(list2.Count);
		bool flag = false;
		foreach (HistoryFileOnDisk day in list2)
		{
			SaveHistoryEntry saveHistoryEntry = list.Find((SaveHistoryEntry k) => k != null && k.file == day.file);
			if (saveHistoryEntry == null)
			{
				if (!IsSaveFileIntact(day.absPath, out var reason))
				{
					Debug.LogWarning($"[SaveFiles] Earlier day {day.file} of slot {slotId} is damaged ({reason}) and is not listed");
					continue;
				}
				ReadHeadFields(day.absPath, out var credit, out var version);
				saveHistoryEntry = new SaveHistoryEntry
				{
					file = day.file,
					dayCount = day.dayCount,
					credit = credit,
					saveDateTime = day.stamp,
					version = version
				};
				flag = true;
			}
			list3.Add(saveHistoryEntry);
		}
		if (flag)
		{
			TrimHistory(list3, EmporiumOption.SaveHistoryDays - 1);
			SaveHistory(slotId, list3);
			Debug.Log($"[SaveFiles] Listed {list3.Count} earlier day(s) of slot {slotId} from the save folder");
		}
	}

	public static List<RepairResult> TakeRecentRepairs()
	{
		List<RepairResult> result = new List<RepairResult>(recentRepairs);
		recentRepairs.Clear();
		return result;
	}

	public static int RepairDamagedRuns()
	{
		int num = 0;
		foreach (int item in IndexLoad("activeSaveSlotId", new List<int>()).ToList())
		{
			string text = AbsolutePath(Slot(item));
			bool flag = File.Exists(text);
			if ((!flag || !IsSaveFileIntact(text, out var _)) && (flag || FindHistoryFilesOnDisk(item).Count != 0) && RepairDamagedRun(item, null) != null)
			{
				num++;
			}
		}
		return num;
	}

	public static RepairResult RepairDamagedRun(int slotId, List<int> knownActive)
	{
		string relativeName = Slot(slotId);
		string latest = AbsolutePath(relativeName);
		bool flag = File.Exists(latest);
		string reason = null;
		if (flag && IsSaveFileIntact(latest, out reason))
		{
			return null;
		}
		List<HistoryFileOnDisk> list = FindHistoryFilesOnDisk(slotId);
		if (!flag && list.Count == 0)
		{
			return null;
		}
		SavePreviewData savePreviewData = IndexLoad<SavePreviewData>("preview_" + slotId, null);
		RepairResult repairResult = new RepairResult
		{
			slotId = slotId,
			runName = savePreviewData?.GetDisplayName()
		};
		if (flag)
		{
			Debug.LogError($"[SaveFiles] Latest save of slot {slotId} is damaged ({reason}), moving it aside");
			if (!QuarantineFile(relativeName))
			{
				return null;
			}
			repairResult.quarantined++;
		}
		List<SaveHistoryEntry> list2 = IndexLoad("history_" + slotId, new List<SaveHistoryEntry>()) ?? new List<SaveHistoryEntry>();
		HistoryFileOnDisk promoted = null;
		List<SaveHistoryEntry> list3 = new List<SaveHistoryEntry>();
		foreach (HistoryFileOnDisk day in list)
		{
			if (!IsSaveFileIntact(day.absPath, out var reason2))
			{
				Debug.LogError($"[SaveFiles] Earlier day {day.file} of slot {slotId} is damaged ({reason2}), moving it aside");
				if (QuarantineFile(day.file))
				{
					repairResult.quarantined++;
				}
				continue;
			}
			if (promoted == null)
			{
				try
				{
					WithIORetry(delegate
					{
						File.Move(day.absPath, latest);
					});
					promoted = day;
				}
				catch (Exception ex)
				{
					Debug.LogError($"[SaveFiles] Could not make {day.file} the latest save of slot {slotId}: {ex.Message}");
					goto IL_01e3;
				}
				continue;
			}
			goto IL_01e3;
			IL_01e3:
			SaveHistoryEntry saveHistoryEntry = list2.Find((SaveHistoryEntry k) => k != null && k.file == day.file);
			if (saveHistoryEntry == null)
			{
				ReadHeadFields(day.absPath, out var credit, out var version);
				saveHistoryEntry = new SaveHistoryEntry
				{
					file = day.file,
					dayCount = day.dayCount,
					credit = credit,
					saveDateTime = day.stamp,
					version = version
				};
			}
			list3.Add(saveHistoryEntry);
		}
		if (promoted == null)
		{
			Debug.LogError($"[SaveFiles] Slot {slotId} has no intact save left and is removed from the list (its files were moved aside, not deleted)");
			UnregisterSlot(slotId);
			knownActive?.Remove(slotId);
			recentRepairs.Add(repairResult);
			return repairResult;
		}
		SaveHistoryEntry saveHistoryEntry2 = list2.Find((SaveHistoryEntry k) => k != null && k.file == promoted.file);
		int credit2;
		string version2;
		if (saveHistoryEntry2 != null)
		{
			credit2 = saveHistoryEntry2.credit;
			version2 = saveHistoryEntry2.version;
		}
		else
		{
			ReadHeadFields(latest, out credit2, out version2);
		}
		if (savePreviewData == null)
		{
			try
			{
				savePreviewData = BuildPreviewFromStore(ReadStoreForPreview(new ES3Settings(latest, ES3.Location.File)), slotId, latest);
			}
			catch (Exception ex2)
			{
				Debug.LogError($"[SaveFiles] Could not read the promoted save of slot {slotId} for its preview, listing it with what the file name says: {ex2.Message}");
				savePreviewData = new SavePreviewData
				{
					slotId = slotId,
					startName = "",
					startScenarioId = "",
					startLocale = ""
				};
			}
		}
		savePreviewData.slotId = slotId;
		savePreviewData.dayCount = promoted.dayCount;
		savePreviewData.credit = credit2;
		savePreviewData.lastSaveDateTime = promoted.stamp;
		if (!string.IsNullOrEmpty(version2))
		{
			savePreviewData.version = version2;
		}
		RegisterSlot(slotId, savePreviewData);
		if (knownActive != null && !knownActive.Contains(slotId))
		{
			knownActive.Add(slotId);
		}
		SaveHistory(slotId, list3);
		repairResult.runName = savePreviewData.GetDisplayName();
		repairResult.promotedDay = promoted.dayCount;
		Debug.LogWarning($"[SaveFiles] Slot {slotId} continues from day {promoted.dayCount} ({promoted.file}), {list3.Count} earlier day(s) remain");
		recentRepairs.Add(repairResult);
		return repairResult;
	}

	private static void UnregisterSlot(int slotId)
	{
		List<int> list = IndexLoad("activeSaveSlotId", new List<int>());
		if (list.Remove(slotId))
		{
			IndexSave("activeSaveSlotId", list);
		}
		IndexDeleteKey("preview_" + slotId);
		IndexDeleteKey("history_" + slotId);
		SaveUIManager.RemoveCacheEntry(slotId);
	}

	public static int ImportFromFile(string absoluteSourcePath)
	{
		if (!File.Exists(absoluteSourcePath))
		{
			return -1;
		}
		if (!IsSaveFileIntact(absoluteSourcePath, out var reason))
		{
			Debug.LogWarning("[SaveFiles] Cannot import damaged save file " + absoluteSourcePath + ": " + reason);
			return -1;
		}
		ES3Settings settings = new ES3Settings(absoluteSourcePath, ES3.Location.File);
		try
		{
			if (!ES3.KeyExists("playerStore", settings))
			{
				return -1;
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[SaveFiles] Cannot import unreadable save file " + absoluteSourcePath + ": " + ex.Message);
			return -1;
		}
		int num = NextFreeSlotId();
		string path = Slot(num);
		string destFileName = Path.Combine(Application.persistentDataPath, path);
		File.Copy(absoluteSourcePath, destFileName, overwrite: false);
		SavePreviewData preview = BuildPreviewFromStore(ReadStoreForPreview(settings), num, absoluteSourcePath);
		RegisterSlot(num, preview);
		Debug.Log($"[SaveFiles] Imported save as slot {num} from {absoluteSourcePath}");
		return num;
	}

	public static List<int> ScanAndRegisterOrphans()
	{
		List<int> list = new List<int>();
		List<int> list2 = IndexLoad("activeSaveSlotId", new List<int>());
		string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath, "save_*.es3");
		foreach (string text in files)
		{
			try
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				int result;
				bool flag = int.TryParse(fileNameWithoutExtension.Substring("save_".Length), out result) && result >= 0 && fileNameWithoutExtension == "save_" + result;
				if (flag && list2.Contains(result))
				{
					continue;
				}
				if (IsHistoryStem(fileNameWithoutExtension, out var slotId))
				{
					if (!list2.Contains(slotId))
					{
						RepairResult repairResult = RepairDamagedRun(slotId, list2);
						if (repairResult != null && repairResult.promotedDay >= 0)
						{
							list.Add(slotId);
						}
					}
					continue;
				}
				if (!IsSaveFileIntact(text, out var reason))
				{
					if (flag)
					{
						RepairResult repairResult2 = RepairDamagedRun(result, list2);
						if (repairResult2 != null && repairResult2.promotedDay >= 0)
						{
							list.Add(result);
						}
					}
					else
					{
						Debug.LogWarning("[SaveFiles] Skipped damaged save file " + text + ": " + reason);
					}
					continue;
				}
				ES3Settings settings = new ES3Settings(text, ES3.Location.File);
				if (ES3.KeyExists("playerStore", settings))
				{
					if (!flag)
					{
						result = NextFreeSlotId(list2);
						string text2 = Path.Combine(Application.persistentDataPath, Slot(result));
						File.Move(text, text2);
						settings = new ES3Settings(text2, ES3.Location.File);
						Debug.Log($"[SaveFiles] Moved non-canonical save {text} to slot {result}");
					}
					SavePreviewData savePreviewData = BuildPreviewFromStore(ReadStoreForPreview(settings), result, AbsolutePath(Slot(result)));
					list2.Add(result);
					IndexSave("preview_" + result, savePreviewData);
					SaveUIManager.UpdateCacheEntry(result, savePreviewData);
					list.Add(result);
					if (flag)
					{
						RestoreHistoryFromDisk(result);
					}
					Debug.Log($"[SaveFiles] Registered orphan slot {result} from {text}");
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[SaveFiles] Skipped unreadable save file " + text + ": " + ex.Message);
			}
		}
		if (list.Count > 0)
		{
			IndexSave("activeSaveSlotId", list2);
			int num = list2.Max();
			int num2 = IndexLoad("slotIdGenerator", 0);
			if (num >= num2)
			{
				IndexSave("slotIdGenerator", num + 1);
			}
		}
		return list;
	}

	public static int NextFreeSlotId(List<int> knownActive = null)
	{
		int i;
		for (i = IndexLoad("slotIdGenerator", 0); ES3.FileExists(Slot(i)) || (knownActive != null && knownActive.Contains(i)); i++)
		{
		}
		IndexSave("slotIdGenerator", i + 1);
		return i;
	}

	public static void RegisterSlot(int slotId, SavePreviewData preview)
	{
		List<int> list = IndexLoad("activeSaveSlotId", new List<int>());
		if (!list.Contains(slotId))
		{
			list.Add(slotId);
		}
		IndexSave("activeSaveSlotId", list);
		IndexSave("preview_" + slotId, preview);
		SaveUIManager.UpdateCacheEntry(slotId, preview);
	}

	public static void DeleteSlot(int slotId)
	{
		List<int> list = IndexLoad("activeSaveSlotId", new List<int>());
		list.Remove(slotId);
		IndexSave("activeSaveSlotId", list);
		IndexDeleteKey("preview_" + slotId);
		if (ES3.FileExists(Slot(slotId)))
		{
			ES3.DeleteFile(Slot(slotId));
		}
		List<SaveHistoryEntry> history = LoadHistory(slotId);
		TrimHistory(history, 0);
		SaveHistory(slotId, history);
		SaveUIManager.RemoveCacheEntry(slotId);
	}

	private static PlayerStore ReadStoreForPreview(ES3Settings settings)
	{
		PlayerStore playerStore = (PlayerStore)FormatterServices.GetUninitializedObject(typeof(PlayerStore));
		ES3.LoadInto("playerStore", playerStore, settings);
		return playerStore;
	}

	private static SavePreviewData BuildPreviewFromStore(PlayerStore store, int slotId, string absolutePath)
	{
		DateTime lastSaveDateTime = DateTime.Now;
		try
		{
			if (File.Exists(absolutePath))
			{
				lastSaveDateTime = File.GetLastWriteTime(absolutePath);
			}
		}
		catch
		{
		}
		return new SavePreviewData
		{
			slotId = slotId,
			credit = store.playerCash,
			lastSaveDateTime = lastSaveDateTime,
			startName = NewGameData.GetStartDisplayName(store.startType, store.startScenarioId),
			version = store.saveVersion,
			startType = store.startType,
			startScenarioId = store.startScenarioId,
			startLocale = store.startLocale,
			dayCount = 0
		};
	}
}

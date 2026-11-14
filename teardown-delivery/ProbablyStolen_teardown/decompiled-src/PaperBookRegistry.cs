using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class PaperBookRegistry
{
	public class Entry
	{
		public string fullId;

		public string modId;

		public string bookId;

		public PaperBookData data;

		public string path;

		public DateTime lastWrite;

		public PaperUI open;

		public readonly List<Action<PaperUI>> onOpened = new List<Action<PaperUI>>();

		public string registeredItemId;

		public bool IsFileBacked => !string.IsNullOrEmpty(path);

		public string Title
		{
			get
			{
				if (data != null && !string.IsNullOrEmpty(data.title))
				{
					return data.title;
				}
				return bookId;
			}
		}
	}

	public const string BOOKS_FOLDER = "Books";

	private static readonly Dictionary<string, Entry> entries = new Dictionary<string, Entry>();

	private static readonly List<string> extraBookRoots = new List<string>();

	private static bool hooked;

	private static ModItemDirectory liveDirectory;

	public static IEnumerable<Entry> All => entries.Values;

	public static IEnumerable<Entry> Loaded => entries.Values.Where((Entry e) => e.data != null);

	public static event Action Changed;

	public static string FullId(string modId, string bookId)
	{
		if (!string.IsNullOrEmpty(modId))
		{
			return modId + ":" + bookId;
		}
		return bookId;
	}

	public static IEnumerable<Entry> OfMod(string modId)
	{
		return entries.Values.Where((Entry e) => e.modId == modId);
	}

	public static Entry Get(string fullId)
	{
		if (string.IsNullOrEmpty(fullId) || !entries.TryGetValue(fullId, out var value))
		{
			return null;
		}
		return value;
	}

	public static Entry Find(string modId, string bookId)
	{
		return Get(FullId(modId, bookId));
	}

	public static Entry Resolve(string id)
	{
		Entry entry = Get(id);
		if (entry != null)
		{
			return entry;
		}
		if (string.IsNullOrEmpty(id) || id.Contains(':'))
		{
			return null;
		}
		List<Entry> list = entries.Values.Where((Entry e) => e.data != null && e.bookId == id).ToList();
		if (list.Count != 1)
		{
			return null;
		}
		return list[0];
	}

	public static string BooksFolder(LoadedMod mod)
	{
		return Path.Combine(mod.FolderPath, "Books");
	}

	public static void LoadMod(LoadedMod mod)
	{
		string path = BooksFolder(mod);
		if (!System.IO.Directory.Exists(path))
		{
			return;
		}
		int num = 0;
		string[] files = System.IO.Directory.GetFiles(path, "*.json");
		foreach (string file in files)
		{
			if (LoadFile(mod.Manifest.ID, file) != null)
			{
				num++;
			}
		}
		if (num > 0)
		{
			UnityEngine.Debug.Log($"[PaperUI] {ModLoader.GetModLabel(mod.Manifest)}: {num} book(s) loaded");
		}
	}

	public static int LoadFolder(string modId, string folder)
	{
		if (string.IsNullOrEmpty(folder) || !System.IO.Directory.Exists(folder))
		{
			return 0;
		}
		int num = 0;
		string[] files = System.IO.Directory.GetFiles(folder, "*.json");
		foreach (string file in files)
		{
			Entry entry = entries.Values.FirstOrDefault((Entry e) => e.IsFileBacked && PathsEqual(e.path, file));
			if (entry == null)
			{
				if (LoadFile(modId, file) != null)
				{
					num++;
				}
			}
			else if (ReloadFromDisk(entry))
			{
				num++;
			}
		}
		return num;
	}

	public static Entry LoadFile(string modId, string file)
	{
		if (!IsAllowedBookPath(modId, file))
		{
			Debug.LogError("[PaperUI] '" + modId + "': refusing to read '" + file + "', outside the mod folders or not a .json file");
			return null;
		}
		string json;
		try
		{
			json = File.ReadAllText(file);
		}
		catch (Exception ex)
		{
			Debug.LogError("[PaperUI] Cannot read " + file + ": " + ex.Message);
			return null;
		}
		PaperBookData paperBookData = PaperJson.ParseBook(json, Path.GetFileName(file));
		if (paperBookData == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(paperBookData.id))
		{
			paperBookData.id = Path.GetFileNameWithoutExtension(file);
		}
		Entry entry = Register(modId, paperBookData, file);
		if (entry != null)
		{
			entry.lastWrite = File.GetLastWriteTimeUtc(file);
		}
		return entry;
	}

	public static Entry Register(string modId, PaperBookData data, string path = null)
	{
		if (data == null || string.IsNullOrEmpty(data.id))
		{
			Debug.LogError("[PaperUI] A book needs an id");
			return null;
		}
		PaperJson.Normalize(data);
		EnsureHooked();
		if (!IsSafeToken(data.id))
		{
			Debug.LogError("[PaperUI] Book id '" + data.id + "' is not valid (letters, digits, '_', '-' and '.', no separators)");
			return null;
		}
		if (path != null && !IsAllowedBookPath(modId, path))
		{
			Debug.LogError("[PaperUI] Book '" + data.id + "' of '" + modId + "': path '" + path + "' is outside the mod folders or not a .json file; the book is registered without a file");
			path = null;
		}
		string text = FullId(modId, data.id);
		if (!entries.TryGetValue(text, out var value))
		{
			value = new Entry
			{
				fullId = text,
				modId = modId,
				bookId = data.id
			};
			entries[text] = value;
		}
		value.data = data;
		if (path != null)
		{
			value.path = path;
		}
		if (value.open != null && value.open.IsAlive)
		{
			value.open.ReplaceData(data);
			value.open.Rebuild();
		}
		RegisterItemNow(value);
		PaperBookRegistry.Changed?.Invoke();
		return value;
	}

	public static bool Unregister(string fullId)
	{
		if (!entries.TryGetValue(fullId, out var value))
		{
			return false;
		}
		if (value.open != null)
		{
			value.open.Close();
		}
		UnregisterItem(value);
		entries.Remove(fullId);
		PaperBookRegistry.Changed?.Invoke();
		return true;
	}

	public static int RemoveMod(string modId)
	{
		List<string> list = (from e in entries.Values
			where e.modId == modId
			select e.fullId).ToList();
		foreach (string item in list)
		{
			Unregister(item);
		}
		return list.Count;
	}

	public static PaperUI Open(string id)
	{
		Entry entry = Resolve(id);
		if (entry == null)
		{
			Debug.LogWarning("[PaperUI] No book '" + id + "'");
			return null;
		}
		return Open(entry);
	}

	public static PaperUI Open(Entry entry)
	{
		if (entry == null || entry.data == null)
		{
			Debug.LogWarning("[PaperUI] Book '" + entry?.fullId + "' has no data yet (its file is not loaded or failed to parse)");
			return null;
		}
		if (entry.open != null && entry.open.IsAlive)
		{
			entry.open.Show();
			return entry.open;
		}
		PaperUI paperUI = PaperUI.FromData(entry.fullId, entry.data, entry.modId);
		entry.open = paperUI;
		paperUI.Show();
		if (!paperUI.IsAlive)
		{
			entry.open = null;
			return null;
		}
		FireOpened(entry, paperUI);
		paperUI.RefreshPage();
		paperUI.Rebuilt += delegate(PaperUI rebuilt)
		{
			if (entry.open == rebuilt)
			{
				rebuilt.ResetBindings();
				FireOpened(entry, rebuilt);
				rebuilt.RefreshPage();
			}
		};
		return paperUI;
	}

	private static void FireOpened(Entry entry, PaperUI ui)
	{
		foreach (Action<PaperUI> item in entry.onOpened.ToList())
		{
			try
			{
				item(ui);
			}
			catch (Exception arg)
			{
				Debug.LogError($"[PaperUI] OnOpened callback for '{entry.fullId}' threw: {arg}");
			}
		}
		ModHook.FireOnPaperOpened(entry.fullId, ui);
	}

	public static PaperUI GetOpen(string id)
	{
		Entry entry = Resolve(id);
		if (entry == null || entry.open == null || !entry.open.IsAlive)
		{
			return null;
		}
		return entry.open;
	}

	public static bool IsOpen(string id)
	{
		return GetOpen(id)?.IsVisible ?? false;
	}

	public static void Close(string id)
	{
		Entry entry = Resolve(id);
		if (entry?.open != null)
		{
			entry.open.Hide();
		}
	}

	public static void CloseAll()
	{
		foreach (Entry value in entries.Values)
		{
			if (value.open != null && value.open.IsAlive)
			{
				value.open.Hide();
			}
		}
	}

	public static void OnOpened(string fullId, Action<PaperUI> callback)
	{
		if (callback != null)
		{
			Entry entry = Get(fullId);
			if (entry == null)
			{
				entry = new Entry
				{
					fullId = fullId,
					modId = SplitMod(fullId),
					bookId = SplitBook(fullId),
					data = null
				};
				entries[fullId] = entry;
			}
			entry.onOpened.Add(callback);
			if (entry.open != null && entry.open.IsAlive)
			{
				callback(entry.open);
			}
		}
	}

	private static string SplitMod(string fullId)
	{
		int num = fullId.IndexOf(':');
		if (num <= 0)
		{
			return null;
		}
		return fullId.Substring(0, num);
	}

	private static string SplitBook(string fullId)
	{
		int num = fullId.IndexOf(':');
		if (num <= 0)
		{
			return fullId;
		}
		return fullId.Substring(num + 1);
	}

	private static bool IsSafeToken(string id)
	{
		if (string.IsNullOrEmpty(id) || id.Length > 128 || id[0] == '.')
		{
			return false;
		}
		foreach (char c in id)
		{
			if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && (c < '0' || c > '9') && c != '_' && c != '-' && c != '.')
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsAllowedBookPath(string modId, string path)
	{
		try
		{
			if (!path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			string fullPath = Path.GetFullPath(path);
			if (IsUnder(fullPath, ModLoader.ModsPath))
			{
				return true;
			}
			foreach (string extraBookRoot in extraBookRoots)
			{
				if (IsUnder(fullPath, extraBookRoot))
				{
					return true;
				}
			}
			LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId);
			return loadedMod != null && loadedMod.FolderPath != null && IsUnder(fullPath, loadedMod.FolderPath);
		}
		catch
		{
			return false;
		}
	}

	private static string GetAllowedEntryPath(Entry entry, string action)
	{
		string path = entry.path;
		string modId = entry.modId;
		string text;
		try
		{
			text = Path.GetFullPath(path);
		}
		catch
		{
			text = null;
		}
		if (text != null && IsAllowedBookPath(modId, text))
		{
			return text;
		}
		if (action != null)
		{
			Debug.LogError("[PaperUI] Book '" + entry.bookId + "' of '" + modId + "': refusing to " + action + " '" + path + "', outside the mod folders or not a .json file");
		}
		return null;
	}

	private static bool IsUnder(string fullPath, string root)
	{
		string value = Path.GetFullPath(root).TrimEnd('\\', '/') + Path.DirectorySeparatorChar;
		return fullPath.StartsWith(value, StringComparison.OrdinalIgnoreCase);
	}

	public static bool Save(Entry entry)
	{
		if (entry == null || !entry.IsFileBacked)
		{
			return false;
		}
		string allowedEntryPath = GetAllowedEntryPath(entry, "write");
		if (allowedEntryPath == null)
		{
			return false;
		}
		try
		{
			string directoryName = Path.GetDirectoryName(allowedEntryPath);
			if (!string.IsNullOrEmpty(directoryName) && !System.IO.Directory.Exists(directoryName))
			{
				System.IO.Directory.CreateDirectory(directoryName);
			}
			File.WriteAllText(allowedEntryPath, PaperJson.Serialize(entry.data));
			entry.lastWrite = File.GetLastWriteTimeUtc(allowedEntryPath);
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError("[PaperUI] Cannot write " + allowedEntryPath + ": " + ex.Message);
			return false;
		}
	}

	public static bool HasChangedOnDisk(Entry entry)
	{
		if (entry == null || !entry.IsFileBacked)
		{
			return false;
		}
		string allowedEntryPath = GetAllowedEntryPath(entry, null);
		if (allowedEntryPath == null || !File.Exists(allowedEntryPath))
		{
			return false;
		}
		return File.GetLastWriteTimeUtc(allowedEntryPath) != entry.lastWrite;
	}

	public static bool ReloadFromDisk(Entry entry, bool force = false)
	{
		if (entry == null || !entry.IsFileBacked)
		{
			return false;
		}
		if (!force && !HasChangedOnDisk(entry))
		{
			return false;
		}
		string modId = entry.modId;
		string allowedEntryPath = GetAllowedEntryPath(entry, "read");
		if (allowedEntryPath == null)
		{
			return false;
		}
		if (!File.Exists(allowedEntryPath))
		{
			Debug.LogWarning("[PaperUI] " + allowedEntryPath + " is gone; keeping the loaded book");
			entry.lastWrite = DateTime.MinValue;
			return false;
		}
		string json;
		try
		{
			json = File.ReadAllText(allowedEntryPath);
		}
		catch (Exception ex)
		{
			Debug.LogError("[PaperUI] Cannot read " + allowedEntryPath + ": " + ex.Message);
			return false;
		}
		entry.lastWrite = File.GetLastWriteTimeUtc(allowedEntryPath);
		PaperBookData paperBookData = PaperJson.ParseBook(json, Path.GetFileName(allowedEntryPath));
		if (paperBookData == null)
		{
			return false;
		}
		if (!string.IsNullOrEmpty(paperBookData.id) && paperBookData.id != entry.bookId)
		{
			Debug.LogWarning("[PaperUI] " + Path.GetFileName(allowedEntryPath) + ": the id changed to '" + paperBookData.id + "'; the book keeps '" + entry.bookId + "' (rename the file to rename the book)");
		}
		paperBookData.id = entry.bookId;
		return Register(modId, paperBookData, allowedEntryPath) != null;
	}

	public static void RefreshItem(Entry entry)
	{
		if (entry != null && entry.data != null)
		{
			RegisterItemNow(entry);
		}
	}

	public static int ReloadAll()
	{
		int num = 0;
		foreach (LoadedMod mod in ModLoader.Mods)
		{
			if (!mod.IsEnabled)
			{
				continue;
			}
			string path = BooksFolder(mod);
			if (!System.IO.Directory.Exists(path))
			{
				continue;
			}
			string[] files = System.IO.Directory.GetFiles(path, "*.json");
			foreach (string file in files)
			{
				Entry entry = entries.Values.FirstOrDefault((Entry e) => e.IsFileBacked && PathsEqual(e.path, file));
				if (entry == null)
				{
					if (LoadFile(mod.Manifest.ID, file) != null)
					{
						num++;
					}
				}
				else if (ReloadFromDisk(entry))
				{
					num++;
				}
			}
		}
		return num;
	}

	private static bool PathsEqual(string a, string b)
	{
		return string.Equals(Path.GetFullPath(a), Path.GetFullPath(b), StringComparison.OrdinalIgnoreCase);
	}

	private static void EnsureHooked()
	{
		if (!hooked)
		{
			hooked = true;
			ModHook.OnModItemDirectoryInit += OnItemDirectoryInit;
		}
	}

	private static void OnItemDirectoryInit(ModItemDirectory directory)
	{
		liveDirectory = directory;
		foreach (Entry value in entries.Values)
		{
			if (value.data?.item != null && !string.IsNullOrEmpty(value.data.item.id))
			{
				value.registeredItemId = value.data.item.id;
				Entry captured = value;
				directory.Add(value.data.item.id, () => CreateItem(captured));
			}
		}
	}

	private static void RegisterItemNow(Entry entry)
	{
		ModItemDirectory modItemDirectory = LiveDirectory();
		if (modItemDirectory == null)
		{
			return;
		}
		string text = ((entry.data?.item != null && !string.IsNullOrEmpty(entry.data.item.id)) ? entry.data.item.id : null);
		if (entry.registeredItemId != null && entry.registeredItemId != text)
		{
			modItemDirectory.Remove(entry.registeredItemId);
			entry.registeredItemId = null;
		}
		if (text != null)
		{
			Entry captured = entry;
			modItemDirectory.Remove(text);
			modItemDirectory.Add(text, () => CreateItem(captured));
			entry.registeredItemId = text;
		}
	}

	private static void UnregisterItem(Entry entry)
	{
		if (entry.registeredItemId != null)
		{
			ModItemDirectory modItemDirectory = LiveDirectory();
			if (modItemDirectory != null)
			{
				modItemDirectory.Remove(entry.registeredItemId);
			}
			entry.registeredItemId = null;
		}
	}

	private static ModItemDirectory LiveDirectory()
	{
		if ((object)liveDirectory == null || liveDirectory.factoryDictionary == null)
		{
			return null;
		}
		return liveDirectory;
	}

	public static GameItem CreateItem(Entry entry)
	{
		PaperItemData paperItemData = entry.data?.item;
		if (paperItemData == null)
		{
			return null;
		}
		GameItem gameItem = ItemDirectory.CreateEmptyItem(paperItemData.id);
		string modId = entry.modId;
		gameItem.name = ((!string.IsNullOrEmpty(paperItemData.nameLoc) && modId != null) ? ModHelper.GetLocalized(modId, paperItemData.nameLoc) : ((!string.IsNullOrEmpty(paperItemData.name)) ? paperItemData.name : entry.Title));
		gameItem.shortDescription = ((!string.IsNullOrEmpty(paperItemData.descriptionLoc) && modId != null) ? ModHelper.GetLocalized(modId, paperItemData.descriptionLoc) : (paperItemData.description ?? ""));
		ApplyItemSprite(gameItem, paperItemData.sprite, modId);
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		if (paperItemData.important)
		{
			gameItem.EnableTag("IMPORTANT_TAG");
		}
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		BookHelper.InitPaperGuide(gameItem);
		gameItem.unitValue = paperItemData.value;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		string fullId = entry.fullId;
		gameItem.onActivateSlotItemFunc = delegate
		{
			Open(fullId);
		};
		if (modId != null)
		{
			LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId);
			ModHelper.SetSource(gameItem, modId, (loadedMod != null) ? ModLoader.GetModLabel(loadedMod.Manifest) : modId);
		}
		return gameItem;
	}

	private static void ApplyItemSprite(GameItem item, string sprite, string modId)
	{
		if (string.IsNullOrEmpty(sprite))
		{
			sprite = "items_misc/manual1";
		}
		int num = sprite.IndexOf('/');
		if (num > 0)
		{
			string text = sprite.Substring(0, num);
			if (!text.StartsWith("Items/"))
			{
				text = "Items/" + text;
			}
			item.SetSpriteAndShape(text, sprite.Substring(num + 1));
		}
		else if (modId != null)
		{
			item.SetSpriteAndShapeFromMod(modId, sprite);
		}
		else
		{
			item.SetSpriteAndShape("Items/items_misc", sprite);
		}
	}

	public static GameItem SpawnItem(string id)
	{
		Entry entry = Resolve(id);
		if (entry?.data?.item == null || string.IsNullOrEmpty(entry.data.item.id))
		{
			Debug.LogWarning("[PaperUI] Book '" + id + "' has no item");
			return null;
		}
		return ItemSpawner.Spawn(entry.data.item.id);
	}
}

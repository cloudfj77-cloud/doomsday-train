using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class PaperEditor : MonoBehaviour
{
	private enum DragMode
	{
		None,
		Move,
		Resize,
		Marquee,
		Panel
	}

	public enum AlignMode
	{
		Left,
		CenterX,
		Right,
		Top,
		CenterY,
		Bottom
	}

	public readonly List<PaperElementData> Selection = new List<PaperElementData>();

	public int ActiveTab;

	public bool snapToGrid = true;

	public bool snapToElements = true;

	public bool showRulers = true;

	public bool showIds = true;

	public float gridSize = 2f;

	public float editorZoom = 1f;

	public bool inspectMode;

	private static readonly List<PaperElementData> clipboard = new List<PaperElementData>();

	private const float AUTOSAVE_DELAY = 0.6f;

	private const float WATCH_INTERVAL = 1f;

	private const float SNAP_THRESHOLD = 3f;

	private const float MIN_SIZE = 1f;

	private bool dirty;

	private float dirtyTimer;

	private float watchTimer;

	private PaperBookData knownData;

	private bool applyingOwnData;

	private Locale originalLocale;

	private DragMode drag;

	private Vector2 dragStart;

	private Vector3 dragStartScreen;

	private bool dragMoved;

	private readonly Dictionary<PaperElementData, Rect> dragOrigins = new Dictionary<PaperElementData, Rect>();

	private readonly Dictionary<PaperElementData, Vector2> dragOriginEnds = new Dictionary<PaperElementData, Vector2>();

	private Rect dragOriginBounds;

	private PaperEditorOverlay.Handle dragHandle;

	private Vector2 panelStart;

	private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

	private bool reshowNextUpdate;

	private bool stopNextUpdate;

	private bool focused;

	private DateTime lastFailedReload;

	public static PaperEditor Instance { get; private set; }

	public PaperBookRegistry.Entry Entry { get; private set; }

	public PaperBookData Data => Entry?.data;

	public PaperUI Ui { get; private set; }

	public PaperEditorOverlay Overlay { get; private set; }

	public PaperEditorHistory History { get; } = new PaperEditorHistory();


	public string EditingTemplate { get; private set; }

	public string Status { get; private set; } = "";


	public bool IsEditing
	{
		get
		{
			if (Entry != null && Entry.data != null && Ui != null)
			{
				return Ui.IsAlive;
			}
			return false;
		}
	}

	public bool Dirty => dirty;

	public PaperPageData CurrentPageData
	{
		get
		{
			if (Ui == null)
			{
				return null;
			}
			return Data.GetPage(Ui.CurrentPage);
		}
	}

	public PaperTemplateData EditingTemplateData
	{
		get
		{
			if (EditingTemplate == null)
			{
				return null;
			}
			return Data.GetTemplate(EditingTemplate);
		}
	}

	public bool CanPaste => clipboard.Count > 0;

	private static bool Shift
	{
		get
		{
			if (!Input.GetKey(KeyCode.LeftShift))
			{
				return Input.GetKey(KeyCode.RightShift);
			}
			return true;
		}
	}

	private static bool Ctrl
	{
		get
		{
			if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftMeta))
			{
				return Input.GetKey(KeyCode.RightMeta);
			}
			return true;
		}
	}

	private static bool Alt
	{
		get
		{
			if (!Input.GetKey(KeyCode.LeftAlt))
			{
				return Input.GetKey(KeyCode.RightAlt);
			}
			return true;
		}
	}

	public event Action Changed;

	public static PaperEditor Ensure()
	{
		if (Instance != null)
		{
			return Instance;
		}
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[PaperEditor] No CustomUIManager, cannot open the paper editor");
			return null;
		}
		GameObject obj = new GameObject("PaperEditor");
		obj.transform.SetParent(CustomUIManager.Instance.transform, worldPositionStays: false);
		Instance = obj.AddComponent<PaperEditor>();
		return Instance;
	}

	public static void ToggleWindow()
	{
		PaperEditor paperEditor = Ensure();
		if (!(paperEditor == null))
		{
			if (PaperEditorWindow.IsOpen)
			{
				PaperEditorWindow.Hide();
			}
			else
			{
				PaperEditorWindow.Open(paperEditor);
			}
		}
	}

	public static void OpenWindow()
	{
		PaperEditor paperEditor = Ensure();
		if (paperEditor != null)
		{
			PaperEditorWindow.Open(paperEditor);
		}
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		PaperBookRegistry.Changed += OnRegistryChanged;
	}

	private void OnDestroy()
	{
		PaperBookRegistry.Changed -= OnRegistryChanged;
		if (Instance == this)
		{
			StopEditing();
			Instance = null;
		}
	}

	public void SetStatus(string text)
	{
		Status = text ?? "";
		this.Changed?.Invoke();
	}

	public void RaiseChanged()
	{
		this.Changed?.Invoke();
	}

	public void Edit(PaperBookRegistry.Entry entry)
	{
		if (entry == null || entry.data == null)
		{
			return;
		}
		if (!CustomUIManager.HasLayer("paper"))
		{
			SetStatus("Load a save first: paper books live on the store scene's paper canvas.");
		}
		else if (Entry != entry || !IsEditing)
		{
			StopEditing();
			Entry = entry;
			knownData = entry.data;
			Ui = PaperBookRegistry.Open(entry);
			if (Ui == null)
			{
				Entry = null;
				SetStatus("Could not open the book.");
				return;
			}
			Ui.EditorMode = true;
			Ui.PreviewTemplate = null;
			EditingTemplate = null;
			Ui.Rebuilt += OnUiRebuilt;
			Ui.Closed += OnUiClosed;
			Ui.Rebuild();
			Ui.Show();
			Overlay = new PaperEditorOverlay(this);
			Overlay.Attach(Ui);
			Selection.Clear();
			History.Reset(Data, Keys());
			ApplyEditorZoom();
			PaperEditorWindow.ClearIssues();
			focused = true;
			dirty = false;
			SetStatus("Editing " + entry.fullId + (entry.IsFileBacked ? "" : " (not file-backed: changes are not saved)"));
		}
	}

	public void StopEditing()
	{
		if (Ui != null)
		{
			Ui.Rebuilt -= OnUiRebuilt;
			Ui.Closed -= OnUiClosed;
			if (dirty)
			{
				Save();
			}
			if (Ui.IsAlive)
			{
				Ui.EditorMode = false;
				Ui.PreviewTemplate = null;
				if (Ui.Rect != null)
				{
					Ui.Rect.localScale = Vector3.one;
				}
				Ui.Rebuild();
			}
		}
		Overlay?.Destroy();
		Overlay = null;
		Ui = null;
		Entry = null;
		knownData = null;
		EditingTemplate = null;
		Selection.Clear();
		drag = DragMode.None;
		inspectMode = false;
		RestoreLocale();
		this.Changed?.Invoke();
	}

	private void OnUiRebuilt(PaperUI ui)
	{
		if (ui == Ui)
		{
			Overlay?.Attach(Ui);
			RestoreSelection(Keys());
			Overlay?.Refresh();
			ApplyEditorZoom();
		}
	}

	private void OnUiClosed(PaperUI ui)
	{
		if (ui == Ui)
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				Selection.Clear();
				reshowNextUpdate = true;
			}
			else
			{
				stopNextUpdate = true;
			}
		}
	}

	private void OnRegistryChanged()
	{
		if (!applyingOwnData && Entry != null)
		{
			if (Entry.data == null)
			{
				StopEditing();
				SetStatus("The book was removed.");
			}
			else if (Entry.data != knownData)
			{
				knownData = Entry.data;
				ValidateView();
				RestoreSelection(Keys());
				History.Push(Data, Keys(), "reload from disk");
				dirty = false;
				PaperEditorWindow.ClearIssues();
				SetStatus($"Reloaded from disk {DateTime.Now:HH:mm:ss}");
			}
		}
	}

	private void ValidateView()
	{
		if (EditingTemplate != null && Data != null && !Data.templates.Any((PaperTemplateData t) => t.id == EditingTemplate))
		{
			EditingTemplate = null;
			if (Ui != null)
			{
				Ui.PreviewTemplate = null;
				Ui.Rebuild();
			}
		}
	}

	public PaperBookRegistry.Entry NewBook(string modId, string bookId, string title, string starterId, string background)
	{
		LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId);
		if (loadedMod == null || loadedMod.Source != 0)
		{
			SetStatus("Pick a local mod (Workshop folders are read-only).");
			return null;
		}
		bookId = PaperEditorFiles.SanitizeId(bookId);
		if (string.IsNullOrEmpty(bookId))
		{
			SetStatus("The book needs an id (letters, digits, underscore).");
			return null;
		}
		string text = Path.Combine(PaperBookRegistry.BooksFolder(loadedMod), bookId + ".json");
		if (File.Exists(text) || PaperBookRegistry.Find(modId, bookId) != null)
		{
			SetStatus("A book '" + bookId + "' already exists in " + ModLoader.GetModLabel(loadedMod.Manifest) + ".");
			return null;
		}
		PaperBookData paperBookData = PaperStarters.NewBook(starterId, bookId, title);
		if (!string.IsNullOrEmpty(background) && background != paperBookData.background)
		{
			paperBookData.background = background;
			Sprite sprite = PaperAssets.Resolve(background, modId, quiet: true);
			Vector2 vector = ((sprite != null) ? sprite.rect.size : new Vector2((paperBookData.designWidth > 0f) ? paperBookData.designWidth : 314f, (paperBookData.designHeight > 0f) ? paperBookData.designHeight : 402f));
			Vector2 vector2 = PaperAssets.DisplaySizeOf(background);
			if (vector2 == Vector2.zero)
			{
				vector2 = vector * 2f;
			}
			paperBookData.width = vector2.x;
			paperBookData.height = vector2.y;
			paperBookData.designWidth = 0f;
			paperBookData.designHeight = 0f;
			paperBookData.chrome = PaperStarters.DefaultChrome(vector, vector2.x / vector.x);
			paperBookData.templates.Clear();
			foreach (PaperPageData page in paperBookData.pages)
			{
				page.template = null;
				page.slots.Clear();
			}
		}
		applyingOwnData = true;
		PaperBookRegistry.Entry entry = PaperBookRegistry.Register(modId, paperBookData, text);
		applyingOwnData = false;
		if (entry == null)
		{
			return null;
		}
		PaperBookRegistry.Save(entry);
		Edit(entry);
		SetStatus("Created " + text);
		return entry;
	}

	public void Save()
	{
		if (Entry == null || !Entry.IsFileBacked)
		{
			dirty = false;
		}
		else if (PaperBookRegistry.Save(Entry))
		{
			dirty = false;
			Status = $"Saved {DateTime.Now:HH:mm:ss}  {Entry.path}";
			PaperEditorWindow.UpdateStatus(Status);
		}
		else
		{
			dirtyTimer = 3f;
			SetStatus("Save failed (file locked?), retrying; see the console.");
		}
	}

	public void ReloadFromDisk()
	{
		if (Entry != null && Entry.IsFileBacked)
		{
			if (dirty)
			{
				Save();
			}
			if (!PaperBookRegistry.ReloadFromDisk(Entry, force: true))
			{
				SetStatus("Reload failed; see the console.");
			}
		}
	}

	public List<PaperElementData> EditableElements()
	{
		if (EditingTemplate != null)
		{
			PaperTemplateData paperTemplateData = Data.templates.FirstOrDefault((PaperTemplateData t) => t.id == EditingTemplate);
			if (paperTemplateData == null)
			{
				return new List<PaperElementData>();
			}
			return paperTemplateData.elements;
		}
		PaperPageData currentPageData = CurrentPageData;
		if (currentPageData == null)
		{
			return new List<PaperElementData>();
		}
		return currentPageData.elements;
	}

	public bool IsChrome(PaperElementData element)
	{
		if (Data != null)
		{
			return Data.chrome.Contains(element);
		}
		return false;
	}

	public bool IsTemplateElementOnPage(PaperElementData element)
	{
		if (Data == null || EditingTemplate != null)
		{
			return false;
		}
		PaperPageData currentPageData = CurrentPageData;
		return ((currentPageData != null) ? Data.GetTemplate(currentPageData.template) : null)?.elements.Contains(element) ?? false;
	}

	public bool IsEditable(PaperElementData element)
	{
		if (element == null || Data == null)
		{
			return false;
		}
		if (element.locked)
		{
			return false;
		}
		if (IsChrome(element))
		{
			return true;
		}
		if (IsTemplateElementOnPage(element))
		{
			return false;
		}
		return EditableElements().Contains(element);
	}

	public bool IsBuiltInTemplate(string templateId)
	{
		if (Data != null && Data.templates.All((PaperTemplateData t) => t.id != templateId))
		{
			return PaperStarters.GetBuiltInTemplate(templateId) != null;
		}
		return false;
	}

	public string KeyOf(PaperElementData element)
	{
		if (Data == null || element == null)
		{
			return null;
		}
		int num = Data.chrome.IndexOf(element);
		if (num >= 0)
		{
			return $"chrome/{num}";
		}
		for (int i = 0; i < Data.pages.Count; i++)
		{
			num = Data.pages[i].elements.IndexOf(element);
			if (num >= 0)
			{
				return $"page/{i}/{num}";
			}
		}
		foreach (PaperTemplateData template in Data.templates)
		{
			num = template.elements.IndexOf(element);
			if (num >= 0)
			{
				return $"template/{template.id}/{num}";
			}
		}
		PaperTemplateData paperTemplateData = PaperStarters.BuiltInTemplateIds().Select(PaperStarters.GetBuiltInTemplate).FirstOrDefault((PaperTemplateData t) => t.elements.Contains(element));
		if (paperTemplateData != null)
		{
			return $"builtin/{paperTemplateData.id}/{paperTemplateData.elements.IndexOf(element)}";
		}
		return null;
	}

	public PaperElementData FromKey(string key)
	{
		if (Data == null || string.IsNullOrEmpty(key))
		{
			return null;
		}
		string[] parts = key.Split('/');
		try
		{
			switch (parts[0])
			{
			case "chrome":
				return Data.chrome[int.Parse(parts[1])];
			case "page":
				return Data.pages[int.Parse(parts[1])].elements[int.Parse(parts[2])];
			case "template":
				return Data.templates.First((PaperTemplateData t) => t.id == parts[1]).elements[int.Parse(parts[2])];
			case "builtin":
				return PaperStarters.GetBuiltInTemplate(parts[1]).elements[int.Parse(parts[2])];
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	public List<string> Keys()
	{
		return (from k in Selection.Select(KeyOf)
			where k != null
			select k).ToList();
	}

	private void RestoreSelection(List<string> keys)
	{
		Selection.Clear();
		foreach (string key in keys)
		{
			PaperElementData paperElementData = FromKey(key);
			if (paperElementData != null && !Selection.Contains(paperElementData))
			{
				Selection.Add(paperElementData);
			}
		}
	}

	public bool IsSelected(PaperElementData element)
	{
		return Selection.Contains(element);
	}

	public void Select(PaperElementData element, bool add = false)
	{
		if (!add)
		{
			Selection.Clear();
		}
		if (element != null && !Selection.Contains(element))
		{
			Selection.Add(element);
		}
		Overlay?.UpdateLive();
		this.Changed?.Invoke();
	}

	public void SelectMany(IEnumerable<PaperElementData> elements, bool add = false)
	{
		if (!add)
		{
			Selection.Clear();
		}
		foreach (PaperElementData element in elements)
		{
			if (element != null && !Selection.Contains(element))
			{
				Selection.Add(element);
			}
		}
		Overlay?.UpdateLive();
		this.Changed?.Invoke();
	}

	public void ToggleSelect(PaperElementData element)
	{
		if (element != null)
		{
			if (!Selection.Remove(element))
			{
				Selection.Add(element);
			}
			Overlay?.UpdateLive();
			this.Changed?.Invoke();
		}
	}

	public void ClearSelection()
	{
		if (Selection.Count != 0)
		{
			Selection.Clear();
			Overlay?.UpdateLive();
			this.Changed?.Invoke();
		}
	}

	public void SelectAll()
	{
		SelectMany(from e in EditableElements()
			where !e.locked
			select e);
	}

	public Rect BoundsOf(PaperElementData element)
	{
		PaperUI.Binding binding = Ui?.GetBinding(element);
		if (binding == null || binding.rect == null || Ui.Content == null)
		{
			return element.Bounds;
		}
		Vector3[] array = new Vector3[4];
		binding.rect.GetWorldCorners(array);
		Vector2 vector = Ui.Content.InverseTransformPoint(array[0]);
		Vector2 vector2 = Ui.Content.InverseTransformPoint(array[2]);
		Vector2 designSize = Ui.DesignSize;
		return new Rect(vector.x + designSize.x * 0.5f, designSize.y * 0.5f - vector2.y, vector2.x - vector.x, vector2.y - vector.y);
	}

	public Rect SelectionBounds()
	{
		bool flag = false;
		Rect result = Rect.zero;
		foreach (PaperElementData item in Selection)
		{
			Rect rect = BoundsOf(item);
			if (!flag)
			{
				result = rect;
				flag = true;
			}
			else
			{
				result = Rect.MinMaxRect(Mathf.Min(result.xMin, rect.xMin), Mathf.Min(result.yMin, rect.yMin), Mathf.Max(result.xMax, rect.xMax), Mathf.Max(result.yMax, rect.yMax));
			}
		}
		return result;
	}

	public List<PaperElementData> MovableSelection()
	{
		return Selection.Where((PaperElementData e) => IsEditable(e) && string.IsNullOrEmpty(e.flow)).ToList();
	}

	public bool CanMoveSelection()
	{
		return MovableSelection().Count > 0;
	}

	public bool CanResizeSelection()
	{
		if (Selection.Count == 1 && MovableSelection().Count == 1)
		{
			return !Selection[0].IsLine;
		}
		return false;
	}

	private PaperElementData HitElement(Vector2 point)
	{
		if (Ui == null)
		{
			return null;
		}
		List<PaperUI.Binding> list = Ui.VisibleBindings();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			PaperElementData data = list[num].data;
			if ((!data.locked || Selection.Contains(data)) && BoundsOf(data).Contains(point))
			{
				return data;
			}
		}
		return null;
	}

	public void Commit(string label, bool rebuild = true)
	{
		if (IsEditing)
		{
			PaperJson.Normalize(Data);
			if (rebuild)
			{
				Ui.Rebuild();
			}
			else
			{
				Overlay?.Refresh();
			}
			History.Push(Data, Keys(), label);
			dirty = true;
			dirtyTimer = 0.6f;
			this.Changed?.Invoke();
		}
	}

	public void PreviewText(PaperElementData element, string text)
	{
		element.text = text;
		PaperUI.Binding binding = Ui?.GetBinding(element);
		if (binding?.text != null && string.IsNullOrEmpty(element.loc) && string.IsNullOrEmpty(element.slot))
		{
			binding.text.text = text;
		}
		Overlay?.UpdateLive();
	}

	private void ApplyData(PaperBookData data, List<string> selectionKeys, string status)
	{
		if (Entry != null && data != null)
		{
			PaperJson.Normalize(data);
			applyingOwnData = true;
			PaperBookRegistry.Register(Entry.modId, data, Entry.path);
			applyingOwnData = false;
			knownData = Entry.data;
			Ui = PaperBookRegistry.GetOpen(Entry.fullId) ?? Ui;
			if (Ui != null && !Ui.EditorMode)
			{
				Ui.EditorMode = true;
				Ui.Rebuild();
			}
			ValidateView();
			RestoreSelection(selectionKeys);
			Overlay?.Refresh();
			dirty = true;
			dirtyTimer = 0.6f;
			PaperEditorWindow.ClearIssues();
			SetStatus(status);
		}
	}

	public void Undo()
	{
		PaperEditorHistory.Snapshot snapshot = History.Undo();
		if (snapshot != null)
		{
			ApplyData(PaperJson.Deserialize<PaperBookData>(snapshot.json), snapshot.selection, "Undo: " + snapshot.label);
		}
	}

	public void Redo()
	{
		PaperEditorHistory.Snapshot snapshot = History.Redo();
		if (snapshot != null)
		{
			ApplyData(PaperJson.Deserialize<PaperBookData>(snapshot.json), snapshot.selection, "Redo: " + snapshot.label);
		}
	}

	public PaperElementData AddElement(PaperElementData element, bool toChrome = false, bool centered = true)
	{
		if (!IsEditing || element == null)
		{
			return null;
		}
		List<PaperElementData> list = (toChrome ? Data.chrome : EditableElements());
		if (!toChrome && EditingTemplate == null && CurrentPageData == null)
		{
			AddPage();
			list = EditableElements();
		}
		if (centered)
		{
			if (element.IsLine)
			{
				Rect rect = element.LineBounds();
				element.Translate(Mathf.Round((Ui.DesignSize.x - rect.width) * 0.5f) - rect.x, Mathf.Round((Ui.DesignSize.y - rect.height) * 0.5f) - rect.y);
			}
			else
			{
				element.x = Mathf.Round((Ui.DesignSize.x - element.w) * 0.5f);
				element.y = Mathf.Round((Ui.DesignSize.y - element.h) * 0.5f);
			}
		}
		element.id = UniqueId(element.id);
		list.Add(element);
		Selection.Clear();
		Selection.Add(element);
		Commit("add " + element.type);
		return element;
	}

	public string UniqueId(string wanted, PaperElementData except = null)
	{
		if (string.IsNullOrEmpty(wanted) || Data == null)
		{
			return wanted;
		}
		HashSet<string> hashSet = new HashSet<string>(from e in Data.AllElements()
			where e != except && !string.IsNullOrEmpty(e.id)
			select e.id);
		if (!hashSet.Contains(wanted))
		{
			return wanted;
		}
		int i;
		for (i = 2; hashSet.Contains($"{wanted}_{i}"); i++)
		{
		}
		return $"{wanted}_{i}";
	}

	private List<PaperElementData> CloneSet(IEnumerable<PaperElementData> sources, List<PaperElementData> target, float offsetX, float offsetY)
	{
		List<PaperElementData> list = new List<PaperElementData>();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (PaperElementData source in sources)
		{
			PaperElementData paperElementData = source.Clone();
			paperElementData.Translate(offsetX, offsetY);
			if (!string.IsNullOrEmpty(paperElementData.id))
			{
				string text = UniqueId(paperElementData.id);
				if (text != paperElementData.id)
				{
					dictionary[paperElementData.id] = text;
				}
				paperElementData.id = text;
			}
			list.Add(paperElementData);
			target?.Add(paperElementData);
		}
		foreach (PaperElementData item in list)
		{
			if (!string.IsNullOrEmpty(item.flow) && dictionary.TryGetValue(item.flow, out var value))
			{
				item.flow = value;
			}
		}
		return list;
	}

	public void DeleteSelection()
	{
		List<PaperElementData> list = Selection.Where((PaperElementData e) => IsEditable(e) || IsChrome(e)).ToList();
		if (list.Count == 0)
		{
			return;
		}
		foreach (PaperElementData element in list)
		{
			Data.chrome.Remove(element);
			foreach (PaperPageData page in Data.pages)
			{
				page.elements.Remove(element);
			}
			foreach (PaperTemplateData template in Data.templates)
			{
				template.elements.Remove(element);
			}
			if (!element.IsFlow || string.IsNullOrEmpty(element.id))
			{
				continue;
			}
			foreach (PaperElementData item in (from c in Data.AllElements()
				where c.flow == element.id
				select c).ToList())
			{
				item.flow = null;
				item.x = element.x;
				item.y = element.y;
				if (item.w <= 0f)
				{
					item.w = element.w;
				}
				if (item.h <= 0f)
				{
					item.h = 20f;
				}
			}
		}
		Selection.Clear();
		Commit("delete");
	}

	public void Copy()
	{
		List<PaperElementData> list = Selection.ToList();
		if (list.Count == 0)
		{
			return;
		}
		clipboard.Clear();
		foreach (PaperElementData item in list)
		{
			clipboard.Add(item.Clone());
		}
		SetStatus($"Copied {clipboard.Count} element(s)");
	}

	public void Cut()
	{
		Copy();
		DeleteSelection();
	}

	public void Paste()
	{
		if (!IsEditing || clipboard.Count == 0)
		{
			return;
		}
		List<PaperElementData> list = EditableElements();
		if (EditingTemplate == null && CurrentPageData == null)
		{
			AddPage();
		}
		list = EditableElements();
		Selection.Clear();
		foreach (PaperElementData item in CloneSet(clipboard, list, 6f, 6f))
		{
			if (item.IsChromeAction && list != Data.chrome)
			{
				item.action = null;
			}
			Selection.Add(item);
		}
		Commit("paste");
	}

	public void Duplicate()
	{
		List<PaperElementData> list = Selection.Where((PaperElementData e) => IsEditable(e) || IsChrome(e)).ToList();
		if (list.Count == 0)
		{
			return;
		}
		Selection.Clear();
		foreach (IGrouping<List<PaperElementData>, PaperElementData> item in from g in list.GroupBy(ContainerOf)
			where g.Key != null
			select g)
		{
			List<PaperElementData> key = item.Key;
			List<PaperElementData> list2 = item.OrderBy(key.IndexOf).ToList();
			List<PaperElementData> list3 = CloneSet(list2, null, 6f, 6f);
			int num = key.IndexOf(list2[list2.Count - 1]) + 1;
			foreach (PaperElementData item2 in list3)
			{
				if (item2.IsChromeAction && key != Data.chrome)
				{
					item2.action = null;
				}
				key.Insert(num++, item2);
				Selection.Add(item2);
			}
		}
		Commit("duplicate");
	}

	public List<PaperElementData> ContainerOf(PaperElementData element)
	{
		if (Data.chrome.Contains(element))
		{
			return Data.chrome;
		}
		foreach (PaperPageData page in Data.pages)
		{
			if (page.elements.Contains(element))
			{
				return page.elements;
			}
		}
		foreach (PaperTemplateData template in Data.templates)
		{
			if (template.elements.Contains(element))
			{
				return template.elements;
			}
		}
		return null;
	}

	public void Nudge(float dx, float dy)
	{
		List<PaperElementData> list = MovableSelection();
		if (list.Count == 0)
		{
			return;
		}
		foreach (PaperElementData item in list)
		{
			item.Translate(dx, dy);
		}
		Commit("nudge");
	}

	public void SetRect(PaperElementData element, float x, float y, float w, float h)
	{
		element.x = x;
		element.y = y;
		element.w = Mathf.Max(1f, w);
		element.h = Mathf.Max(1f, h);
		Commit("edit rect");
	}

	public void Align(AlignMode mode)
	{
		List<PaperElementData> list = MovableSelection();
		if (list.Count == 0)
		{
			return;
		}
		Rect rect = ((list.Count > 1) ? SelectionBounds() : new Rect(0f, 0f, Ui.DesignSize.x, Ui.DesignSize.y));
		foreach (PaperElementData item in list)
		{
			Rect bounds = BoundsOf(item);
			switch (mode)
			{
			case AlignMode.Left:
				SetBoxX(item, bounds, rect.xMin);
				break;
			case AlignMode.CenterX:
				SetBoxX(item, bounds, Mathf.Round((rect.center.x - bounds.width * 0.5f) * 2f) * 0.5f);
				break;
			case AlignMode.Right:
				SetBoxX(item, bounds, rect.xMax - bounds.width);
				break;
			case AlignMode.Top:
				SetBoxY(item, bounds, rect.yMin);
				break;
			case AlignMode.CenterY:
				SetBoxY(item, bounds, Mathf.Round((rect.center.y - bounds.height * 0.5f) * 2f) * 0.5f);
				break;
			case AlignMode.Bottom:
				SetBoxY(item, bounds, rect.yMax - bounds.height);
				break;
			}
		}
		Commit("align");
	}

	public void Distribute(bool horizontal)
	{
		List<PaperElementData> list = MovableSelection();
		if (list.Count < 3)
		{
			return;
		}
		list.Sort((PaperElementData a, PaperElementData b) => (!horizontal) ? BoundsOf(a).yMin.CompareTo(BoundsOf(b).yMin) : BoundsOf(a).xMin.CompareTo(BoundsOf(b).xMin));
		Rect rect = BoundsOf(list[0]);
		Rect rect2 = BoundsOf(list[list.Count - 1]);
		float num = (horizontal ? (rect2.xMax - rect.xMin) : (rect2.yMax - rect.yMin));
		float num2 = list.Sum((PaperElementData e) => (!horizontal) ? BoundsOf(e).height : BoundsOf(e).width);
		float num3 = (num - num2) / (float)(list.Count - 1);
		float num4 = (horizontal ? rect.xMin : rect.yMin);
		foreach (PaperElementData item in list)
		{
			Rect bounds = BoundsOf(item);
			if (horizontal)
			{
				SetBoxX(item, bounds, Mathf.Round(num4 * 2f) * 0.5f);
				num4 += bounds.width + num3;
			}
			else
			{
				SetBoxY(item, bounds, Mathf.Round(num4 * 2f) * 0.5f);
				num4 += bounds.height + num3;
			}
		}
		Commit("distribute");
	}

	private static void SetBoxX(PaperElementData element, Rect bounds, float x)
	{
		if (element.IsLine)
		{
			element.Translate(x - bounds.xMin, 0f);
		}
		else
		{
			element.x = x;
		}
	}

	private static void SetBoxY(PaperElementData element, Rect bounds, float y)
	{
		if (element.IsLine)
		{
			element.Translate(0f, y - bounds.yMin);
		}
		else
		{
			element.y = y;
		}
	}

	public void MatchSize(bool width, bool height)
	{
		List<PaperElementData> list = MovableSelection();
		if (list.Count < 2)
		{
			return;
		}
		PaperElementData paperElementData = list[0];
		foreach (PaperElementData item in list.Skip(1))
		{
			if (width)
			{
				item.w = paperElementData.w;
			}
			if (height)
			{
				item.h = paperElementData.h;
			}
		}
		Commit("match size");
	}

	public void ZOrder(int direction)
	{
		List<PaperElementData> list = Selection.Where((PaperElementData e) => IsEditable(e) || IsChrome(e)).ToList();
		if (list.Count == 0)
		{
			return;
		}
		IEnumerable<PaperElementData> enumerable2;
		if (direction <= 0)
		{
			IEnumerable<PaperElementData> enumerable = list;
			enumerable2 = enumerable;
		}
		else
		{
			enumerable2 = list.AsEnumerable().Reverse();
		}
		foreach (PaperElementData item in enumerable2)
		{
			List<PaperElementData> list2 = ContainerOf(item);
			if (list2 != null)
			{
				int num = list2.IndexOf(item);
				int num2 = direction switch
				{
					2 => list2.Count - 1, 
					-2 => 0, 
					_ => Mathf.Clamp(num + direction, 0, list2.Count - 1), 
				};
				if (num2 != num)
				{
					list2.RemoveAt(num);
					list2.Insert(num2, item);
				}
			}
		}
		Commit("z-order");
	}

	public bool IsOwned(PaperElementData element)
	{
		if (element == null || Data == null)
		{
			return false;
		}
		if (!IsChrome(element))
		{
			if (!IsTemplateElementOnPage(element))
			{
				return EditableElements().Contains(element);
			}
			return false;
		}
		return true;
	}

	public void SetLocked(bool locked)
	{
		List<PaperElementData> list = Selection.Where(IsOwned).ToList();
		if (list.Count == 0)
		{
			return;
		}
		foreach (PaperElementData item in list)
		{
			item.locked = locked;
		}
		Commit(locked ? "lock" : "unlock", rebuild: false);
	}

	public void SetVisible(bool visible)
	{
		List<PaperElementData> list = Selection.Where(IsOwned).ToList();
		if (list.Count == 0)
		{
			return;
		}
		foreach (PaperElementData item in list)
		{
			item.visible = visible;
		}
		Commit(visible ? "show" : "hide");
	}

	public void RenameElement(PaperElementData element, string newId)
	{
		if (element == null)
		{
			return;
		}
		string oldId = element.id;
		string text = (string.IsNullOrEmpty(newId) ? null : PaperEditorFiles.SanitizeId(newId));
		string text2 = ((text == null) ? null : UniqueId(text, element));
		if (text2 == oldId)
		{
			return;
		}
		element.id = text2;
		if (element.IsFlow && !string.IsNullOrEmpty(oldId))
		{
			List<PaperElementData> list = ContainerOf(element);
			if (list != null)
			{
				foreach (PaperElementData item in list.Where((PaperElementData c) => c.flow == oldId))
				{
					item.flow = text2;
				}
			}
		}
		Commit("id");
	}

	public void SetFlow(string flowId)
	{
		foreach (PaperElementData item in Selection.Where(IsEditable))
		{
			if (!item.IsFlow)
			{
				item.flow = (string.IsNullOrEmpty(flowId) ? null : flowId);
				if (!string.IsNullOrEmpty(flowId))
				{
					item.autoHeight = item.IsText;
				}
			}
		}
		Commit("flow");
	}

	public void GoToPage(int index)
	{
		if (IsEditing)
		{
			if (EditingTemplate != null)
			{
				StopEditingTemplate(select: false);
			}
			Ui.FlipTo(Mathf.Clamp(index, 0, PaperCount() - 1));
			Selection.Clear();
			Overlay?.Refresh();
			this.Changed?.Invoke();
		}
	}

	private int PaperCount()
	{
		return Mathf.Max(1, Data.pages.Count);
	}

	public void AddPage(string template = null)
	{
		PaperPageData currentPageData = CurrentPageData;
		PaperPageData item = new PaperPageData
		{
			name = $"Page {Data.pages.Count + 1}",
			template = (template ?? currentPageData?.template)
		};
		Data.pages.Add(item);
		Selection.Clear();
		Commit("add page");
		GoToPage(Data.pages.Count - 1);
	}

	public void DuplicatePage()
	{
		PaperPageData currentPageData = CurrentPageData;
		if (currentPageData != null)
		{
			PaperPageData paperPageData = PaperJson.Clone(currentPageData);
			paperPageData.name = currentPageData.name + " copy";
			paperPageData.elements = new List<PaperElementData>();
			CloneSet(currentPageData.elements, paperPageData.elements, 0f, 0f);
			Data.pages.Insert(Ui.CurrentPage + 1, paperPageData);
			Selection.Clear();
			Commit("duplicate page");
			GoToPage(Ui.CurrentPage + 1);
		}
	}

	public void DeletePage()
	{
		if (Data.pages.Count > 1 && CurrentPageData != null)
		{
			int currentPage = Ui.CurrentPage;
			Data.pages.RemoveAt(currentPage);
			Selection.Clear();
			Commit("delete page");
			GoToPage(Mathf.Clamp(currentPage, 0, Data.pages.Count - 1));
		}
	}

	public void MovePage(int direction)
	{
		int currentPage = Ui.CurrentPage;
		int num = currentPage + direction;
		if (num >= 0 && num < Data.pages.Count)
		{
			PaperPageData item = Data.pages[currentPage];
			Data.pages.RemoveAt(currentPage);
			Data.pages.Insert(num, item);
			Commit("move page");
			GoToPage(num);
		}
	}

	public void RenamePage(string name)
	{
		PaperPageData currentPageData = CurrentPageData;
		if (currentPageData != null)
		{
			currentPageData.name = name ?? "";
			Commit("rename page", rebuild: false);
		}
	}

	public void SetPageTemplate(string templateId)
	{
		PaperPageData currentPageData = CurrentPageData;
		if (currentPageData != null)
		{
			currentPageData.template = (string.IsNullOrEmpty(templateId) ? null : templateId);
			Selection.Clear();
			Commit("page template");
		}
	}

	public void SetSlot(string slot, string value)
	{
		PaperPageData currentPageData = CurrentPageData;
		if (currentPageData != null && !string.IsNullOrEmpty(slot))
		{
			if (string.IsNullOrEmpty(value))
			{
				currentPageData.slots.Remove(slot);
			}
			else
			{
				currentPageData.slots[slot] = value;
			}
			Commit("slot " + slot);
		}
	}

	public List<string> CurrentSlots()
	{
		PaperPageData currentPageData = CurrentPageData;
		PaperTemplateData paperTemplateData = ((currentPageData != null) ? Data.GetTemplate(currentPageData.template) : null);
		if (paperTemplateData == null)
		{
			return new List<string>();
		}
		return (from e in paperTemplateData.elements
			where !string.IsNullOrEmpty(e.slot)
			select e.slot).Distinct().ToList();
	}

	public List<string> TemplateIds(bool includeBuiltIn)
	{
		List<string> list = Data.templates.Select((PaperTemplateData t) => t.id).ToList();
		if (includeBuiltIn)
		{
			foreach (string item in PaperStarters.BuiltInTemplateIds())
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public void EditTemplate(string templateId)
	{
		if (!string.IsNullOrEmpty(templateId) && IsEditing)
		{
			if (IsBuiltInTemplate(templateId))
			{
				PaperTemplateData item = PaperJson.Clone(PaperStarters.GetBuiltInTemplate(templateId));
				Data.templates.Add(item);
			}
			EditingTemplate = templateId;
			Ui.PreviewTemplate = templateId;
			Selection.Clear();
			Commit("edit template " + templateId);
		}
	}

	public void StopEditingTemplate(bool select = true)
	{
		if (EditingTemplate != null)
		{
			EditingTemplate = null;
			Ui.PreviewTemplate = null;
			Selection.Clear();
			Commit("back to pages");
		}
	}

	public void CreateTemplateFromSelection(string templateId)
	{
		templateId = PaperEditorFiles.SanitizeId(templateId);
		if (string.IsNullOrEmpty(templateId) || Data.templates.Any((PaperTemplateData t) => t.id == templateId))
		{
			SetStatus("Template id missing or already used.");
			return;
		}
		PaperPageData page = CurrentPageData;
		List<PaperElementData> list = Selection.Where((PaperElementData e) => page != null && page.elements.Contains(e)).ToList();
		PaperTemplateData paperTemplateData = new PaperTemplateData
		{
			id = templateId
		};
		foreach (PaperElementData item in list)
		{
			paperTemplateData.elements.Add(item.Clone());
		}
		Data.templates.Add(paperTemplateData);
		if (page != null && string.IsNullOrEmpty(page.template))
		{
			foreach (PaperElementData item2 in list)
			{
				page.elements.Remove(item2);
			}
			page.template = templateId;
		}
		Selection.Clear();
		Commit("new template");
	}

	public void DeleteTemplate(string templateId)
	{
		PaperTemplateData paperTemplateData = Data.templates.FirstOrDefault((PaperTemplateData t) => t.id == templateId);
		if (paperTemplateData == null)
		{
			return;
		}
		Data.templates.Remove(paperTemplateData);
		foreach (PaperPageData page in Data.pages)
		{
			if (page.template == templateId)
			{
				page.template = ((PaperStarters.GetBuiltInTemplate(templateId) != null) ? templateId : null);
			}
		}
		if (EditingTemplate == templateId)
		{
			EditingTemplate = null;
			Ui.PreviewTemplate = null;
		}
		Selection.Clear();
		Commit("delete template");
	}

	public void DetachTemplate()
	{
		PaperPageData currentPageData = CurrentPageData;
		PaperTemplateData paperTemplateData = ((currentPageData != null) ? Data.GetTemplate(currentPageData.template) : null);
		if (paperTemplateData == null)
		{
			return;
		}
		List<PaperElementData> list = CloneSet(paperTemplateData.elements, null, 0f, 0f);
		foreach (PaperElementData item in list)
		{
			if (!string.IsNullOrEmpty(item.slot) && currentPageData.slots.TryGetValue(item.slot, out var value))
			{
				if (item.IsImage)
				{
					item.sprite = value;
				}
				else
				{
					item.text = value;
				}
			}
			item.slot = null;
		}
		currentPageData.elements.InsertRange(0, list);
		currentPageData.template = null;
		currentPageData.slots.Clear();
		Selection.Clear();
		Commit("detach template");
	}

	public void SetTitle(string title)
	{
		Data.title = title ?? "";
		Commit("title", rebuild: false);
	}

	public void SetBackground(string key)
	{
		if (!string.IsNullOrEmpty(key))
		{
			Data.background = key;
			Vector2 vector = PaperAssets.DisplaySizeOf(key);
			if (vector != Vector2.zero)
			{
				Data.width = vector.x;
				Data.height = vector.y;
			}
			Commit("background");
		}
	}

	public void SetDisplaySize(float width, float height)
	{
		Data.width = Mathf.Max(0f, width);
		Data.height = Mathf.Max(0f, height);
		Commit("display size");
	}

	public void SetDesignSize(float width, float height)
	{
		Data.designWidth = Mathf.Max(0f, width);
		Data.designHeight = Mathf.Max(0f, height);
		Commit("design size");
	}

	public void SetBorder(float left, float bottom, float right, float top)
	{
		Data.border = ((left <= 0f && bottom <= 0f && right <= 0f && top <= 0f) ? null : new float[4] { left, bottom, right, top });
		Commit("border");
	}

	public void SetFlags(bool sounds, bool draggable, bool zoomable)
	{
		Data.sounds = sounds;
		Data.draggable = draggable;
		Data.zoomable = zoomable;
		Commit("flags");
	}

	public void ResetChrome(bool close, bool zoom, bool flip, bool pageIndicator)
	{
		Data.chrome = PaperStarters.DefaultChrome(Ui.DesignSize, Ui.DisplayScale, close, zoom, flip, pageIndicator);
		Selection.Clear();
		Commit("chrome");
	}

	public void SetItem(PaperItemData item)
	{
		Data.item = item;
		PaperBookRegistry.RefreshItem(Entry);
		Commit("item", rebuild: false);
	}

	public void SetPanelPosition(float x, float y)
	{
		Data.posX = x;
		Data.posY = y;
		if (Ui?.Rect != null)
		{
			Ui.Rect.anchoredPosition = new Vector2(x, y);
		}
		Commit("panel position", rebuild: false);
	}

	public void SetEditorZoom(float zoom)
	{
		editorZoom = Mathf.Clamp(zoom, 0.25f, 4f);
		ApplyEditorZoom();
		Overlay?.Refresh();
		this.Changed?.Invoke();
	}

	private void ApplyEditorZoom()
	{
		if (Ui?.Rect != null)
		{
			Ui.Rect.localScale = Vector3.one * editorZoom;
		}
	}

	public List<Locale> Locales()
	{
		if (LocalizationSettings.AvailableLocales == null)
		{
			return new List<Locale>();
		}
		return LocalizationSettings.AvailableLocales.Locales;
	}

	public void SetPreviewLocale(Locale locale)
	{
		if (!(locale == null))
		{
			if (originalLocale == null)
			{
				originalLocale = LocalizationSettings.SelectedLocale;
			}
			StartCoroutine(PreviewLocaleRoutine(locale));
		}
	}

	private IEnumerator PreviewLocaleRoutine(Locale locale)
	{
		yield return PaperUI.SwitchLocale(locale);
		if (IsEditing)
		{
			Ui.Rebuild();
		}
		this.Changed?.Invoke();
	}

	private void RestoreLocale()
	{
		if (!(originalLocale == null))
		{
			if (base.isActiveAndEnabled)
			{
				StartCoroutine(PaperUI.SwitchLocale(originalLocale));
			}
			else
			{
				LocalizationSettings.SelectedLocale = originalLocale;
			}
			originalLocale = null;
		}
	}

	private void Update()
	{
		PaperEditorWindow.Tick();
		if (stopNextUpdate)
		{
			stopNextUpdate = false;
			StopEditing();
			SetStatus("Book closed; pick it in the Book tab to continue.");
			return;
		}
		if (!IsEditing)
		{
			if (Entry != null)
			{
				StopEditing();
			}
			return;
		}
		if (reshowNextUpdate)
		{
			reshowNextUpdate = false;
			Ui.Show();
			Overlay?.Refresh();
			this.Changed?.Invoke();
		}
		if (dirty)
		{
			dirtyTimer -= Time.unscaledDeltaTime;
			if (dirtyTimer <= 0f)
			{
				Save();
			}
		}
		watchTimer -= Time.unscaledDeltaTime;
		if (watchTimer <= 0f)
		{
			watchTimer = 1f;
			if (!dirty && PaperBookRegistry.HasChangedOnDisk(Entry))
			{
				DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(Entry.path);
				if (lastWriteTimeUtc != lastFailedReload && !PaperBookRegistry.ReloadFromDisk(Entry))
				{
					lastFailedReload = lastWriteTimeUtc;
					SetStatus("The file on disk has errors (see the console); fix it and it reloads.");
				}
			}
		}
		if (inspectMode)
		{
			PaperEditorInspect.Update(this);
			return;
		}
		Vector2 point = MouseDesign();
		bool flag = drag != 0 || PointerOverBook();
		if (Input.GetMouseButtonDown(0))
		{
			focused = flag || PaperEditorWindow.PointerOverWindow();
		}
		UpdateReadout(point, flag);
		HandleMouse(point, flag);
		Overlay?.UpdateLive();
		if (focused && PaperEditorWindow.IsOpen && !StoreUIManager.IsTextFieldFocused())
		{
			HandleKeys();
		}
	}

	public Vector2 MouseDesign()
	{
		if (Ui?.Content == null)
		{
			return Vector2.zero;
		}
		Canvas componentInParent = Ui.Root.GetComponentInParent<Canvas>();
		Camera cam = ((componentInParent != null && componentInParent.renderMode != 0) ? componentInParent.worldCamera : null);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(Ui.Content, Input.mousePosition, cam, out var localPoint);
		return new Vector2(localPoint.x + Ui.DesignSize.x * 0.5f, Ui.DesignSize.y * 0.5f - localPoint.y);
	}

	public bool PointerOverBook()
	{
		if (EventSystem.current == null || Ui?.Root == null)
		{
			return false;
		}
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = Input.mousePosition
		};
		raycastResults.Clear();
		EventSystem.current.RaycastAll(eventData, raycastResults);
		if (raycastResults.Count == 0)
		{
			return false;
		}
		return raycastResults[0].gameObject.transform.IsChildOf(Ui.Root.transform);
	}

	private void UpdateReadout(Vector2 point, bool overBook)
	{
		if (Overlay != null)
		{
			string text = "";
			if (overBook || drag != 0)
			{
				text = $"x {point.x:0.#}  y {point.y:0.#}";
			}
			if (Selection.Count == 1)
			{
				Rect rect = BoundsOf(Selection[0]);
				text += $"   [{Selection[0].DisplayName}] x {rect.x:0.#} y {rect.y:0.#} w {rect.width:0.#} h {rect.height:0.#}";
			}
			else if (Selection.Count == 2)
			{
				Rect rect2 = BoundsOf(Selection[0]);
				Rect rect3 = BoundsOf(Selection[1]);
				float num = Mathf.Max(0f, Mathf.Max(rect2.xMin, rect3.xMin) - Mathf.Min(rect2.xMax, rect3.xMax));
				float num2 = Mathf.Max(0f, Mathf.Max(rect2.yMin, rect3.yMin) - Mathf.Min(rect2.yMax, rect3.yMax));
				text += $"   gap x {num:0.#}  y {num2:0.#}   centers dx {rect3.center.x - rect2.center.x:0.#} dy {rect3.center.y - rect2.center.y:0.#}";
			}
			else if (Selection.Count > 2)
			{
				Rect rect4 = SelectionBounds();
				text += $"   {Selection.Count} selected  w {rect4.width:0.#} h {rect4.height:0.#}";
			}
			Overlay.SetReadout(text);
		}
	}

	private void HandleMouse(Vector2 point, bool overBook)
	{
		if (Input.GetMouseButtonDown(0) && overBook && drag == DragMode.None)
		{
			dragStart = point;
			dragStartScreen = Input.mousePosition;
			dragMoved = false;
			if (Alt)
			{
				drag = DragMode.Panel;
				panelStart = Ui.Rect.anchoredPosition;
				return;
			}
			if (Overlay.HitHandle(point, out dragHandle))
			{
				drag = DragMode.Resize;
				CaptureOrigins();
				return;
			}
			PaperElementData paperElementData = HitElement(point);
			if (paperElementData != null)
			{
				if (Shift)
				{
					ToggleSelect(paperElementData);
				}
				else if (!IsSelected(paperElementData))
				{
					Select(paperElementData);
				}
				if (IsTemplateElementOnPage(paperElementData) && Selection.Count == 1)
				{
					SetStatus("Template element: use Pages > Edit template to move it, or Detach template.");
				}
				drag = (CanMoveSelection() ? DragMode.Move : DragMode.None);
				if (drag == DragMode.Move)
				{
					CaptureOrigins();
				}
			}
			else
			{
				if (!Shift)
				{
					ClearSelection();
				}
				drag = DragMode.Marquee;
			}
		}
		else
		{
			if (drag == DragMode.None)
			{
				return;
			}
			if (Input.GetMouseButton(0))
			{
				Vector2 delta = point - dragStart;
				switch (drag)
				{
				case DragMode.Move:
					if (dragMoved || !(delta.sqrMagnitude < 0.25f))
					{
						dragMoved = true;
						ApplyMove(delta, !Ctrl);
					}
					break;
				case DragMode.Resize:
					dragMoved = true;
					ApplyResize(delta, !Ctrl);
					break;
				case DragMode.Marquee:
					Overlay.ShowMarquee(RectFrom(dragStart, point));
					break;
				case DragMode.Panel:
				{
					Canvas componentInParent = Ui.Root.GetComponentInParent<Canvas>();
					float num = ((componentInParent != null) ? componentInParent.scaleFactor : 1f);
					Vector3 vector = Input.mousePosition - dragStartScreen;
					Ui.Rect.anchoredPosition = panelStart + new Vector2(vector.x, vector.y) / num;
					break;
				}
				}
			}
			else
			{
				if (!Input.GetMouseButtonUp(0))
				{
					return;
				}
				switch (drag)
				{
				case DragMode.Move:
					Overlay.HideGuides();
					if (dragMoved)
					{
						Commit("move");
					}
					else
					{
						History.Push(Data, Keys(), "select");
					}
					break;
				case DragMode.Resize:
					Overlay.HideGuides();
					Commit("resize");
					break;
				case DragMode.Marquee:
				{
					Overlay.HideMarquee();
					Rect rect = RectFrom(dragStart, point);
					if (rect.width > 0.5f || rect.height > 0.5f)
					{
						List<PaperElementData> elements = (from b in Ui.VisibleBindings()
							select b.data into e
							where !e.locked && BoundsOf(e).Overlaps(rect)
							select e).ToList();
						SelectMany(elements, Shift);
					}
					break;
				}
				case DragMode.Panel:
					Data.posX = Ui.Rect.anchoredPosition.x;
					Data.posY = Ui.Rect.anchoredPosition.y;
					Commit("move panel", rebuild: false);
					break;
				}
				drag = DragMode.None;
			}
		}
	}

	private static Rect RectFrom(Vector2 a, Vector2 b)
	{
		return Rect.MinMaxRect(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
	}

	private void CaptureOrigins()
	{
		dragOrigins.Clear();
		dragOriginEnds.Clear();
		foreach (PaperElementData item in MovableSelection())
		{
			dragOrigins[item] = new Rect(item.x, item.y, item.w, item.h);
			if (item.IsLine)
			{
				dragOriginEnds[item] = new Vector2(item.x2, item.y2);
			}
		}
		dragOriginBounds = SelectionBounds();
	}

	private void ApplyMove(Vector2 delta, bool snap)
	{
		if (dragOrigins.Count == 0)
		{
			return;
		}
		Rect moved = new Rect(dragOriginBounds.position + delta, dragOriginBounds.size);
		float? guideX = null;
		float? guideY = null;
		if (snap)
		{
			delta += SnapOffset(moved, out guideX, out guideY);
		}
		Overlay.ShowGuides(guideX, guideY);
		foreach (KeyValuePair<PaperElementData, Rect> dragOrigin in dragOrigins)
		{
			PaperElementData key = dragOrigin.Key;
			key.x = Round(dragOrigin.Value.x + delta.x);
			key.y = Round(dragOrigin.Value.y + delta.y);
			if (dragOriginEnds.TryGetValue(key, out var value))
			{
				key.x2 = Round(value.x + delta.x);
				key.y2 = Round(value.y + delta.y);
			}
			LiveRect(key);
		}
		Overlay.UpdateLive();
	}

	private void ApplyResize(Vector2 delta, bool snap)
	{
		if (dragOrigins.Count != 1)
		{
			return;
		}
		PaperElementData paperElementData = dragOrigins.Keys.First();
		Rect rect = dragOrigins[paperElementData];
		float num = rect.xMin;
		float num2 = rect.xMax;
		float num3 = rect.yMin;
		float num4 = rect.yMax;
		bool flag = dragHandle == PaperEditorOverlay.Handle.TopLeft || dragHandle == PaperEditorOverlay.Handle.Left || dragHandle == PaperEditorOverlay.Handle.BottomLeft;
		bool flag2 = dragHandle == PaperEditorOverlay.Handle.TopRight || dragHandle == PaperEditorOverlay.Handle.Right || dragHandle == PaperEditorOverlay.Handle.BottomRight;
		bool flag3 = dragHandle == PaperEditorOverlay.Handle.TopLeft || dragHandle == PaperEditorOverlay.Handle.Top || dragHandle == PaperEditorOverlay.Handle.TopRight;
		bool flag4 = dragHandle == PaperEditorOverlay.Handle.BottomLeft || dragHandle == PaperEditorOverlay.Handle.Bottom || dragHandle == PaperEditorOverlay.Handle.BottomRight;
		if (flag)
		{
			num += delta.x;
		}
		if (flag2)
		{
			num2 += delta.x;
		}
		if (flag3)
		{
			num3 += delta.y;
		}
		if (flag4)
		{
			num4 += delta.y;
		}
		float? x = null;
		float? y = null;
		if (snap)
		{
			if (flag)
			{
				bool snapped;
				float num5 = SnapValue(num, horizontal: true, out snapped);
				if (snapped)
				{
					num = num5;
					x = num5;
				}
			}
			if (flag2)
			{
				bool snapped2;
				float num6 = SnapValue(num2, horizontal: true, out snapped2);
				if (snapped2)
				{
					num2 = num6;
					x = num6;
				}
			}
			if (flag3)
			{
				bool snapped3;
				float num7 = SnapValue(num3, horizontal: false, out snapped3);
				if (snapped3)
				{
					num3 = num7;
					y = num7;
				}
			}
			if (flag4)
			{
				bool snapped4;
				float num8 = SnapValue(num4, horizontal: false, out snapped4);
				if (snapped4)
				{
					num4 = num8;
					y = num8;
				}
			}
		}
		Overlay.ShowGuides(x, y);
		if (num2 - num < 1f)
		{
			if (flag)
			{
				num = num2 - 1f;
			}
			else
			{
				num2 = num + 1f;
			}
		}
		if (num4 - num3 < 1f)
		{
			if (flag3)
			{
				num3 = num4 - 1f;
			}
			else
			{
				num4 = num3 + 1f;
			}
		}
		paperElementData.x = Round(num);
		paperElementData.y = Round(num3);
		paperElementData.w = Round(num2 - num);
		paperElementData.h = Round(num4 - num3);
		LiveRect(paperElementData);
		Overlay.UpdateLive();
	}

	private void LiveRect(PaperElementData element)
	{
		if (element.IsLine)
		{
			Ui.RefreshLine(element);
			return;
		}
		PaperUI.Binding binding = Ui.GetBinding(element);
		if (binding?.rect != null && string.IsNullOrEmpty(element.flow))
		{
			PaperUI.SetRect(binding.rect, element.x, element.y, element.w, element.h);
		}
	}

	private static float Round(float value)
	{
		return Mathf.Round(value * 2f) * 0.5f;
	}

	private List<float> SnapTargets(bool horizontal)
	{
		List<float> list = new List<float>();
		float num = (horizontal ? Ui.DesignSize.x : Ui.DesignSize.y);
		list.Add(0f);
		list.Add(num * 0.5f);
		list.Add(num);
		if (snapToElements)
		{
			foreach (PaperUI.Binding item in Ui.VisibleBindings())
			{
				if (!Selection.Contains(item.data))
				{
					Rect rect = BoundsOf(item.data);
					if (horizontal)
					{
						list.Add(rect.xMin);
						list.Add(rect.center.x);
						list.Add(rect.xMax);
					}
					else
					{
						list.Add(rect.yMin);
						list.Add(rect.center.y);
						list.Add(rect.yMax);
					}
				}
			}
		}
		return list;
	}

	private float SnapValue(float value, bool horizontal, out bool snapped)
	{
		float result = value;
		float num = 3f / Mathf.Max(0.25f, editorZoom);
		snapped = false;
		if (snapToGrid && gridSize > 0f)
		{
			float num2 = Mathf.Round(value / gridSize) * gridSize;
			float num3 = Mathf.Abs(num2 - value);
			if (num3 < num)
			{
				result = num2;
				num = num3;
				snapped = true;
			}
		}
		foreach (float item in SnapTargets(horizontal))
		{
			float num4 = Mathf.Abs(item - value);
			if (num4 < num)
			{
				result = item;
				num = num4;
				snapped = true;
			}
		}
		return result;
	}

	private Vector2 SnapOffset(Rect moved, out float? guideX, out float? guideY)
	{
		guideX = null;
		guideY = null;
		Vector2 zero = Vector2.zero;
		float num = 3f / Mathf.Max(0.25f, editorZoom);
		float num2 = num;
		float[] array = new float[3]
		{
			moved.xMin,
			moved.center.x,
			moved.xMax
		};
		foreach (float num3 in array)
		{
			bool snapped;
			float num4 = SnapValue(num3, horizontal: true, out snapped);
			float num5 = Mathf.Abs(num4 - num3);
			if (snapped && num5 < num2)
			{
				num2 = num5;
				zero.x = num4 - num3;
				guideX = num4;
			}
		}
		float num6 = num;
		array = new float[3]
		{
			moved.yMin,
			moved.center.y,
			moved.yMax
		};
		foreach (float num7 in array)
		{
			bool snapped2;
			float num8 = SnapValue(num7, horizontal: false, out snapped2);
			float num9 = Mathf.Abs(num8 - num7);
			if (snapped2 && num9 < num6)
			{
				num6 = num9;
				zero.y = num8 - num7;
				guideY = num8;
			}
		}
		return zero;
	}

	private void HandleKeys()
	{
		bool ctrl = Ctrl;
		bool shift = Shift;
		float num = (shift ? 10f : 1f);
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			Nudge(0f - num, 0f);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			Nudge(num, 0f);
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			Nudge(0f, 0f - num);
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			Nudge(0f, num);
		}
		if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
		{
			DeleteSelection();
		}
		if (Input.GetKeyDown(KeyCode.PageUp))
		{
			GoToPage(Ui.CurrentPage - 1);
		}
		if (Input.GetKeyDown(KeyCode.PageDown))
		{
			GoToPage(Ui.CurrentPage + 1);
		}
		if (Input.GetKeyDown(KeyCode.LeftBracket))
		{
			ZOrder(shift ? (-2) : (-1));
		}
		if (Input.GetKeyDown(KeyCode.RightBracket))
		{
			ZOrder((!shift) ? 1 : 2);
		}
		if (!ctrl)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (shift)
			{
				Redo();
			}
			else
			{
				Undo();
			}
		}
		if (Input.GetKeyDown(KeyCode.Y))
		{
			Redo();
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			Copy();
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			Cut();
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			Paste();
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			Duplicate();
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			SelectAll();
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			Save();
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			SetLocked(!Selection.All((PaperElementData e) => e.locked));
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			SetVisible(!Selection.All((PaperElementData e) => e.visible));
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			snapToGrid = !snapToGrid;
			SetStatus(snapToGrid ? "Grid snapping on" : "Grid snapping off");
		}
	}
}

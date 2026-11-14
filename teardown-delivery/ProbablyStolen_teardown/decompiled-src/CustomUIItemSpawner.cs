using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public static class CustomUIItemSpawner
{
	private class Cell
	{
		public string identifier;

		public GameObject root;

		public CustomUIElement image;

		public CustomUIElement label;
	}

	private class CellClickHandler : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public const KeyCode COPY_KEY = KeyCode.F2;

		public Action onLeftClick;

		public Action onRightClick;

		public string identifier;

		private static CellClickHandler hovered;

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				onLeftClick?.Invoke();
			}
			else if (eventData.button == PointerEventData.InputButton.Right)
			{
				onRightClick?.Invoke();
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			hovered = this;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (hovered == this)
			{
				hovered = null;
			}
		}

		private void OnDisable()
		{
			if (hovered == this)
			{
				hovered = null;
			}
		}

		private void Update()
		{
			if (!(hovered != this) && Input.GetKeyDown(KeyCode.F2))
			{
				GUIUtility.systemCopyBuffer = identifier;
				Debug.Log("[CustomUI] Item spawner: copied '" + identifier + "' to the clipboard");
			}
		}
	}

	public const string WINDOW_ID = "debug_item_spawner";

	private const string COUNT_LABEL_ID = "count";

	private const string ALL_GROUPS = "All";

	private const string RECENT_GROUP = "Recent";

	private const string FAVORITES_GROUP = "Favorites";

	private const int MIN_FILTER_LENGTH = 2;

	private const int GRID_COLUMNS = 6;

	private const float ICON_SIZE = 56f;

	private const float CELL_WIDTH = 100f;

	private const float LABEL_HEIGHT = 16f;

	private const float CELL_HEIGHT = 78f;

	private const float WINDOW_WIDTH = 680f;

	private const float WINDOW_HEIGHT = 620f;

	private const float SCROLL_HEIGHT = 520f;

	private const int STACK_AMOUNT = 10;

	private const int ICONS_PER_FRAME = 20;

	private const string SAVE_FILE = "debug_spawner.es3";

	private const string RECENT_KEY = "recent";

	private const string FAVORITES_KEY = "favorites";

	private const int RECENT_MAX = 20;

	private static readonly Color FAVORITE_LABEL_COLOR = new Color(1f, 0.85f, 0.3f);

	private static string filter = "";

	private static string group = null;

	private static bool stackTen = false;

	private static List<string> recent;

	private static HashSet<string> favorites;

	private static RectTransform listHost;

	private static readonly Dictionary<string, Cell> cellsByIdentifier = new Dictionary<string, Cell>();

	private static int shownCount;

	private static Color? defaultLabelColor;

	private static bool dirty;

	private static Coroutine saveRoutine;

	private static bool quitHookInstalled;

	private const float SAVE_DELAY_SECONDS = 1.5f;

	private const string FILTER_ID = "filter";

	private const float SEARCH_DELAY_SECONDS = 0.3f;

	private static string pendingFilter;

	private static Coroutine searchRoutine;

	private static Coroutine focusRoutine;

	private static readonly Dictionary<string, Sprite> iconCache = new Dictionary<string, Sprite>();

	private static Coroutine iconRoutine;

	public static void Toggle()
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[CustomUI] No CustomUIManager, cannot open the item spawner");
			return;
		}
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("debug_item_spawner");
		if (window != null && window.IsAlive)
		{
			window.Toggle();
			if (window.IsVisible)
			{
				FocusFilter();
			}
		}
		else
		{
			Open();
		}
	}

	public static void Open()
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[CustomUI] No CustomUIManager, cannot open the item spawner");
			return;
		}
		StopIconRoutine();
		StopSearchRoutine();
		listHost = null;
		EnsureLoaded();
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("debug_item_spawner");
		Vector2? vector = ((window != null && window.IsAlive) ? new Vector2?(window.Rect.anchoredPosition) : null);
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("debug_item_spawner", "Item Spawner", "rear").SetSize(680f, 620f).SetDraggable(draggable: true)
			.SetCloseOnEscape()
			.OnClose(FlushSave);
		List<string> groups = BuildGroups();
		if (groups.Count <= 3)
		{
			customUIBuilder.AddLabel("No item directories are registered in this scene.");
			customUIBuilder.Center().Show();
			return;
		}
		if (group == null || !groups.Contains(group))
		{
			group = "All";
		}
		int initialIndex = Mathf.Max(0, groups.IndexOf(group));
		customUIBuilder.BeginRow();
		customUIBuilder.AddInput("filter by identifier...", filter, OnFilterSubmitted, "filter", OnFilterTyping);
		customUIBuilder.Last?.SetPreferredSize(220f, 0f);
		customUIBuilder.AddDropdown(groups, initialIndex, delegate(int i)
		{
			if (i >= 0 && i < groups.Count)
			{
				group = groups[i];
			}
			RebuildList();
		});
		customUIBuilder.Last?.SetPreferredSize(170f, 0f);
		customUIBuilder.AddToggle("Stack x" + 10, stackTen, delegate(bool v)
		{
			stackTen = v;
		});
		customUIBuilder.WithTooltip("Spawn " + 10 + " units per click. Holding Shift while clicking does the same.");
		customUIBuilder.AddFlexibleSpace();
		customUIBuilder.AddLabel("", "count");
		customUIBuilder.End();
		customUIBuilder.AddLabel("Click: spawn into the store. Shift+click: x" + 10 + ". Right-click: toggle favorite. Filter updates as you type (" + 2 + "+ characters).");
		customUIBuilder.BeginScroll(520f);
		listHost = customUIBuilder.CurrentContainer;
		customUIBuilder.End();
		if (vector.HasValue)
		{
			customUIBuilder.SetPosition(vector.Value);
		}
		else
		{
			customUIBuilder.Center();
		}
		customUIBuilder.Show();
		RebuildList();
		FocusFilter();
	}

	private static void OnFilterTyping(string value)
	{
		pendingFilter = value ?? "";
		StopSearchRoutine();
		if (CustomUIManager.Instance != null)
		{
			searchRoutine = CustomUIManager.Instance.StartCoroutine(SearchAfterPause());
		}
	}

	private static void OnFilterSubmitted(string value)
	{
		StopSearchRoutine();
		ApplyFilter(value);
	}

	private static IEnumerator SearchAfterPause()
	{
		yield return new WaitForSecondsRealtime(0.3f);
		searchRoutine = null;
		ApplyFilter(pendingFilter);
	}

	private static void ApplyFilter(string value)
	{
		string text = EffectiveFilter();
		filter = value ?? "";
		if (EffectiveFilter() != text)
		{
			RebuildList();
		}
	}

	private static void StopSearchRoutine()
	{
		if (searchRoutine != null && CustomUIManager.Instance != null)
		{
			CustomUIManager.Instance.StopCoroutine(searchRoutine);
		}
		searchRoutine = null;
	}

	private static void FocusFilter()
	{
		if (!(CustomUIManager.Instance == null))
		{
			if (focusRoutine != null)
			{
				CustomUIManager.Instance.StopCoroutine(focusRoutine);
			}
			focusRoutine = CustomUIManager.Instance.StartCoroutine(FocusFilterNextFrame());
		}
	}

	private static IEnumerator FocusFilterNextFrame()
	{
		yield return null;
		focusRoutine = null;
		CustomUIWindow customUIWindow = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("debug_item_spawner") : null);
		if (customUIWindow == null || !customUIWindow.IsAlive || !customUIWindow.IsVisible)
		{
			yield break;
		}
		CustomUIElement customUIElement = customUIWindow.Get("filter");
		if (!(customUIElement?.input == null))
		{
			if (EventSystem.current != null)
			{
				EventSystem.current.SetSelectedGameObject(customUIElement.input.gameObject);
			}
			customUIElement.input.ActivateInputField();
		}
	}

	private static void RebuildList()
	{
		CustomUIWindow customUIWindow = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("debug_item_spawner") : null);
		if (customUIWindow == null || !customUIWindow.IsAlive || listHost == null)
		{
			return;
		}
		StopIconRoutine();
		CustomUIBuilder.ClearContainer(listHost);
		cellsByIdentifier.Clear();
		List<string> list = BuildShownIdentifiers();
		SetShownCount(list.Count);
		CustomUIBuilder customUIBuilder = new CustomUIBuilder(CustomUIManager.Instance, customUIWindow, listHost);
		if (list.Count == 0)
		{
			customUIBuilder.AddLabel(EmptyListHint());
			return;
		}
		List<Cell> list2 = new List<Cell>(list.Count);
		customUIBuilder.BeginGrid(6, 100f, 78f);
		foreach (string item in list)
		{
			iconCache.TryGetValue(item, out var value);
			customUIBuilder.BeginColumn(2f, TextAnchor.UpperCenter);
			Cell cell = new Cell
			{
				identifier = item,
				root = customUIBuilder.CurrentContainer.gameObject
			};
			customUIBuilder.AddImage(value, 56f, 56f);
			MakeCellInteractive(customUIBuilder.Last, item);
			cell.image = customUIBuilder.Last;
			customUIBuilder.AddLabel(item);
			CenterCellLabel(customUIBuilder.Last);
			cell.label = customUIBuilder.Last;
			if (!defaultLabelColor.HasValue && cell.label?.text != null)
			{
				defaultLabelColor = cell.label.text.color;
			}
			ApplyFavoriteTint(cell);
			customUIBuilder.End();
			list2.Add(cell);
			cellsByIdentifier[item] = cell;
		}
		customUIBuilder.End();
		iconRoutine = CustomUIManager.Instance.StartCoroutine(FillIcons(list2));
	}

	private static void SetShownCount(int count)
	{
		shownCount = count;
		CustomUIWindow customUIWindow = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("debug_item_spawner") : null);
		if (customUIWindow != null && customUIWindow.IsAlive)
		{
			customUIWindow.Get("count")?.SetText(count + " / " + VisibleIdentifiers().Count);
		}
	}

	private static void ApplyFavoriteTint(Cell cell)
	{
		if (cell?.label != null && !(cell.label.text == null))
		{
			if (favorites.Contains(cell.identifier))
			{
				cell.label.SetTextColor(FAVORITE_LABEL_COLOR);
			}
			else if (defaultLabelColor.HasValue)
			{
				cell.label.SetTextColor(defaultLabelColor.Value);
			}
		}
	}

	private static string EmptyListHint()
	{
		if (EffectiveFilter().Length > 0)
		{
			return "No identifier matches '" + filter + "'.";
		}
		if (group == "Recent")
		{
			return "Nothing spawned yet. Spawned items show up here, newest first.";
		}
		if (group == "Favorites")
		{
			return "No favorites yet. Right-click an item to add it.";
		}
		return "This group is empty.";
	}

	private static List<string> BuildGroups()
	{
		List<string> list = DirectoryMaster.GetDictionaryClassNameList<GameItem>().Select(DirectoryDisplayName).Distinct()
			.OrderBy((string g) => g, StringComparer.OrdinalIgnoreCase)
			.ToList();
		list.Insert(0, "Favorites");
		list.Insert(0, "Recent");
		list.Insert(0, "All");
		return list;
	}

	private static string DirectoryDisplayName(string className)
	{
		string text = className;
		if (text.EndsWith("ItemDirectory"))
		{
			text = text.Substring(0, text.Length - "ItemDirectory".Length);
		}
		else if (text.EndsWith("Directory"))
		{
			text = text.Substring(0, text.Length - "Directory".Length);
		}
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return className;
	}

	private static string EffectiveFilter()
	{
		if (filter.Length < 2)
		{
			return "";
		}
		return filter;
	}

	private static List<string> VisibleIdentifiers()
	{
		return (from id in DirectoryMaster.GetIdentifierList<GameItem>()
			where !ItemSpawnerHiddenList.IsHidden(id)
			select id).ToList();
	}

	private static List<string> BuildShownIdentifiers()
	{
		EnsureLoaded();
		List<string> list = VisibleIdentifiers();
		if (group == "Recent")
		{
			HashSet<string> hashSet = new HashSet<string>(list);
			return recent.Where(hashSet.Contains).Where(Matches).ToList();
		}
		IEnumerable<string> source;
		if (group == "All")
		{
			source = list;
		}
		else if (group == "Favorites")
		{
			HashSet<string> hashSet2 = new HashSet<string>(list);
			source = favorites.Where(hashSet2.Contains);
		}
		else
		{
			source = from id in (from c in DirectoryMaster.GetDictionaryClassNameList<GameItem>()
					where DirectoryDisplayName(c) == @group
					select c).SelectMany((string c) => DirectoryMaster.GetIdentifierList<GameItem>(c))
				where !ItemSpawnerHiddenList.IsHidden(id)
				select id;
		}
		return source.Where(Matches).Distinct().OrderBy((string id) => id, StringComparer.OrdinalIgnoreCase)
			.ToList();
	}

	private static bool Matches(string identifier)
	{
		string text = EffectiveFilter();
		if (text.Length == 0)
		{
			return true;
		}
		return identifier.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;
	}

	private static void EnsureLoaded()
	{
		if (recent != null && favorites != null)
		{
			return;
		}
		recent = new List<string>();
		favorites = new HashSet<string>();
		if (!quitHookInstalled)
		{
			quitHookInstalled = true;
			Application.quitting += FlushSave;
		}
		try
		{
			if (ES3.KeyExists("recent", "debug_spawner.es3"))
			{
				recent = ES3.Load<List<string>>("recent", "debug_spawner.es3") ?? new List<string>();
			}
			if (ES3.KeyExists("favorites", "debug_spawner.es3"))
			{
				List<string> list = ES3.Load<List<string>>("favorites", "debug_spawner.es3");
				if (list != null)
				{
					favorites = new HashSet<string>(list);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[CustomUI] Could not load item spawner recent/favorites: " + ex.Message);
		}
	}

	private static void MarkDirty()
	{
		dirty = true;
		if (saveRoutine == null && CustomUIManager.Instance != null)
		{
			saveRoutine = CustomUIManager.Instance.StartCoroutine(SaveAfterDelay());
		}
	}

	private static IEnumerator SaveAfterDelay()
	{
		yield return new WaitForSecondsRealtime(1.5f);
		saveRoutine = null;
		FlushSave();
	}

	private static void FlushSave()
	{
		if (!dirty || recent == null || favorites == null)
		{
			return;
		}
		dirty = false;
		try
		{
			ES3.Save("recent", recent, "debug_spawner.es3");
			ES3.Save("favorites", favorites.ToList(), "debug_spawner.es3");
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[CustomUI] Could not save item spawner recent/favorites: " + ex.Message);
		}
	}

	private static void AddRecent(string identifier)
	{
		EnsureLoaded();
		recent.Remove(identifier);
		recent.Insert(0, identifier);
		if (recent.Count > 20)
		{
			recent.RemoveRange(20, recent.Count - 20);
		}
		MarkDirty();
	}

	private static void ToggleFavorite(string identifier)
	{
		EnsureLoaded();
		bool flag = !favorites.Remove(identifier);
		if (flag)
		{
			favorites.Add(identifier);
		}
		MarkDirty();
		Debug.Log("[CustomUI] '" + identifier + "' " + (flag ? "added to" : "removed from") + " favorites");
		if (cellsByIdentifier.TryGetValue(identifier, out var value) && !(value.root == null))
		{
			ApplyFavoriteTint(value);
			if (!flag && group == "Favorites")
			{
				value.root.SetActive(value: false);
				cellsByIdentifier.Remove(identifier);
				SetShownCount(shownCount - 1);
			}
		}
	}

	private static void MakeCellInteractive(CustomUIElement element, string identifier)
	{
		if (element != null && !(element.image == null))
		{
			element.image.raycastTarget = true;
			element.image.preserveAspect = true;
			element.SetTooltip(identifier + "\nClick: spawn 1, Shift+click: spawn " + 10 + "\nRight-click: toggle favorite\n" + KeyCode.F2.ToString() + ": copy id");
			CellClickHandler cellClickHandler = element.root.AddComponent<CellClickHandler>();
			cellClickHandler.identifier = identifier;
			cellClickHandler.onLeftClick = delegate
			{
				Spawn(identifier);
			};
			cellClickHandler.onRightClick = delegate
			{
				ToggleFavorite(identifier);
			};
		}
	}

	private static void CenterCellLabel(CustomUIElement element)
	{
		if (element != null && !(element.text == null))
		{
			element.text.alignment = TextAlignmentOptions.Center;
			element.text.enableWordWrapping = false;
			element.text.fontSizeMax = element.text.fontSize;
			element.text.fontSizeMin = 1f;
			element.text.enableAutoSizing = true;
			element.SetPreferredSize(100f, 16f);
		}
	}

	private static void Spawn(string identifier)
	{
		if (EmporiumEntry.Instance == null || EmporiumEntry.Instance.invElement == null)
		{
			Debug.LogWarning("[CustomUI] No store inventory in this scene, cannot spawn '" + identifier + "'");
			return;
		}
		GameItem gameItem = DirectoryMaster.Item(identifier);
		if (gameItem != null)
		{
			bool flag = stackTen || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			if (flag)
			{
				gameItem.SetAmount(10);
			}
			GraphUtils.EmporiumTryAdd(gameItem);
			Debug.Log("[CustomUI] Spawned '" + identifier + "' x" + ((!flag) ? 1 : 10));
			AddRecent(identifier);
			if (group == "Recent" && cellsByIdentifier.TryGetValue(identifier, out var value) && value.root != null)
			{
				value.root.transform.SetAsFirstSibling();
			}
		}
	}

	private static void StopIconRoutine()
	{
		if (iconRoutine != null && CustomUIManager.Instance != null)
		{
			CustomUIManager.Instance.StopCoroutine(iconRoutine);
		}
		iconRoutine = null;
	}

	private static IEnumerator FillIcons(List<Cell> cells)
	{
		int budget = 20;
		foreach (Cell cell in cells)
		{
			if (!iconCache.TryGetValue(cell.identifier, out var value))
			{
				value = ResolveIcon(cell.identifier);
				iconCache[cell.identifier] = value;
				budget--;
			}
			if (value != null && cell.image != null && cell.image.IsAlive && cell.image.image != null && cell.image.image.sprite != value)
			{
				cell.image.image.sprite = value;
			}
			if (budget <= 0)
			{
				budget = 20;
				yield return null;
			}
		}
		iconRoutine = null;
	}

	private static Sprite ResolveIcon(string identifier)
	{
		GameItem gameItem = null;
		try
		{
			gameItem = DirectoryMaster.Create<GameItem>(identifier);
			if (gameItem is GameItemElement gameItemElement && gameItemElement.background != null && gameItemElement.background.image != null)
			{
				return gameItemElement.background.image.sprite;
			}
			return null;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[CustomUI] Factory for '" + identifier + "' threw while resolving its icon: " + ex.Message);
			return null;
		}
		finally
		{
			if (gameItem != null)
			{
				foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(gameItem))
				{
					item.Destroy();
				}
				gameItem.Destroy();
			}
		}
	}
}

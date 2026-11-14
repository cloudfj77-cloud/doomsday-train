using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomUIBuilder
{
	private class TabContext
	{
		public RectTransform tabBar;

		public RectTransform pageHost;

		public List<(Button button, GameObject page)> tabs = new List<(Button, GameObject)>();
	}

	private readonly CustomUIManager manager;

	private readonly CustomUIWindow window;

	private readonly Stack<RectTransform> parents = new Stack<RectTransform>();

	private readonly Stack<TabContext> tabContexts = new Stack<TabContext>();

	public CustomUIElement Last { get; private set; }

	public CustomUIWindow Handle => window;

	private RectTransform CurrentParent => parents.Peek();

	public RectTransform CurrentContainer => parents.Peek();

	public CustomUIBuilder(CustomUIManager manager, CustomUIWindow window)
	{
		this.manager = manager;
		this.window = window;
		parents.Push(window.ContentRoot);
	}

	public CustomUIBuilder(CustomUIManager manager, CustomUIWindow window, RectTransform container)
	{
		this.manager = manager;
		this.window = window;
		parents.Push((container != null) ? container : window.ContentRoot);
	}

	public static void ClearContainer(RectTransform container)
	{
		if (!(container == null))
		{
			for (int num = container.childCount - 1; num >= 0; num--)
			{
				Transform child = container.GetChild(num);
				child.gameObject.SetActive(value: false);
				child.SetParent(null, worldPositionStays: false);
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
	}

	public static void RebuildLayout(RectTransform root)
	{
		if (!(root == null))
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(root);
		}
	}

	public CustomUIBuilder SetSize(float width, float height)
	{
		window.SetSize(width, height);
		return this;
	}

	public CustomUIBuilder SetPosition(Vector2 anchoredPosition)
	{
		window.SetPosition(anchoredPosition);
		return this;
	}

	public CustomUIBuilder Center()
	{
		window.Center();
		return this;
	}

	public CustomUIBuilder OnClose(Action callback)
	{
		window.onClose = callback;
		return this;
	}

	public CustomUIBuilder SetCloseOnEscape(bool value = true)
	{
		window.closeOnEscape = value;
		return this;
	}

	public CustomUIBuilder SetDraggable(bool draggable)
	{
		if (window.frame != null && window.frame.dragHandle != null)
		{
			window.frame.dragHandle.enabled = draggable;
		}
		else if (draggable && window.root != null && window.root.GetComponent<UIDragHandler>() == null)
		{
			window.root.AddComponent<UIDragHandler>();
		}
		return this;
	}

	public CustomUIWindow Show()
	{
		window.Show();
		return window;
	}

	public CustomUIWindow GetWindow()
	{
		window.Hide();
		return window;
	}

	public CustomUIBuilder UseMetaPrefabs(bool meta = true)
	{
		window.useMetaPrefabs = meta;
		return this;
	}

	public CustomUIBuilder UseMachineryStyle(bool machinery = true)
	{
		window.useMachineryStyle = machinery;
		return this;
	}

	private CustomUIElement Spawn(CustomUIManager.ElementKind kind, string id)
	{
		string text = kind.ToString().ToLowerInvariant();
		GameObject gameObject = manager.SpawnElement(kind, CurrentParent, window, string.IsNullOrEmpty(id) ? text : (text + "_" + id));
		if (gameObject == null)
		{
			Debug.LogError("[CustomUI] No prefab assigned for '" + text + "' on CustomUIManager, element skipped");
		}
		CustomUIElement customUIElement = CustomUIElement.Bind(gameObject, id);
		if (!string.IsNullOrEmpty(id))
		{
			window.RegisterElement(customUIElement);
		}
		Last = customUIElement;
		return customUIElement;
	}

	public CustomUIBuilder AddLabel(string text, string id = null)
	{
		Spawn(CustomUIManager.ElementKind.Label, id).SetText(text);
		return this;
	}

	public CustomUIBuilder AddHeader(string text, string id = null)
	{
		Spawn(CustomUIManager.ElementKind.Header, id).SetText(text);
		return this;
	}

	public CustomUIBuilder AddWarning(string text, string id = null)
	{
		Spawn(CustomUIManager.ElementKind.Warning, id).SetText(text);
		return this;
	}

	public CustomUIBuilder AddSeparator(string id = null, int verticalPadding = 0)
	{
		if (verticalPadding <= 0)
		{
			Spawn(CustomUIManager.ElementKind.Separator, id);
			return this;
		}
		GameObject gameObject = NewChild("separator_pad");
		VerticalLayoutGroup verticalLayoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
		ConfigureGroup(verticalLayoutGroup, 0f);
		verticalLayoutGroup.padding = new RectOffset(0, 0, verticalPadding, verticalPadding);
		parents.Push(gameObject.transform as RectTransform);
		Spawn(CustomUIManager.ElementKind.Separator, id);
		parents.Pop();
		return this;
	}

	public CustomUIBuilder AddButton(string text, Action onClick, string id = null)
	{
		CustomUIElement customUIElement = Spawn(CustomUIManager.ElementKind.Button, id);
		customUIElement.SetText(text);
		customUIElement.OnClick(onClick);
		return this;
	}

	public CustomUIBuilder AddCloseButton(Action onClick, string id = null)
	{
		Spawn(CustomUIManager.ElementKind.CloseButton, id).OnClick(onClick);
		return this;
	}

	public CustomUIBuilder AddIconButton(Sprite sprite, float size, Action onClick, Sprite highlightSprite = null, string id = null)
	{
		CustomUIElement customUIElement = Spawn(CustomUIManager.ElementKind.CloseButton, id);
		if (sprite != null)
		{
			customUIElement.SetSprite(sprite);
		}
		customUIElement.SetPreferredSize(size, size);
		if (customUIElement.button != null)
		{
			if (highlightSprite != null)
			{
				SpriteState spriteState = customUIElement.button.spriteState;
				spriteState.highlightedSprite = highlightSprite;
				spriteState.pressedSprite = highlightSprite;
				spriteState.selectedSprite = null;
				customUIElement.button.spriteState = spriteState;
				customUIElement.button.transition = Selectable.Transition.SpriteSwap;
			}
			else
			{
				customUIElement.button.transition = Selectable.Transition.ColorTint;
			}
		}
		customUIElement.OnClick(onClick);
		return this;
	}

	public CustomUIBuilder AddImage(Sprite sprite, float width = -1f, float height = -1f, string id = null)
	{
		CustomUIElement customUIElement = Spawn(CustomUIManager.ElementKind.Image, id);
		if (sprite != null)
		{
			customUIElement.SetSprite(sprite);
		}
		if (width > 0f || height > 0f)
		{
			customUIElement.SetPreferredSize(width, height);
		}
		return this;
	}

	public CustomUIBuilder AddImage(string spriteKey, float width = -1f, float height = -1f, string id = null)
	{
		return AddImage(manager.GetSprite(spriteKey), width, height, id);
	}

	public CustomUIBuilder AddToggle(string label, bool initial, Action<bool> onChanged = null, string id = null)
	{
		CustomUIElement customUIElement = Spawn(CustomUIManager.ElementKind.Toggle, id);
		if (string.IsNullOrEmpty(label))
		{
			if (customUIElement.text != null)
			{
				customUIElement.text.gameObject.SetActive(value: false);
			}
		}
		else
		{
			customUIElement.SetText(label);
		}
		customUIElement.SetBool(initial);
		if (onChanged != null)
		{
			customUIElement.OnToggle(onChanged);
		}
		return this;
	}

	public CustomUIBuilder AddSlider(float min, float max, float initial, Action<float> onChanged = null, bool wholeNumbers = false, string id = null)
	{
		CustomUIElement customUIElement = Spawn(CustomUIManager.ElementKind.Slider, id);
		if (customUIElement.slider != null)
		{
			customUIElement.slider.minValue = min;
			customUIElement.slider.maxValue = max;
			customUIElement.slider.wholeNumbers = wholeNumbers;
		}
		customUIElement.SetFloat(initial);
		if (onChanged != null)
		{
			customUIElement.OnSlider(onChanged);
		}
		return this;
	}

	public CustomUIBuilder AddInput(string placeholder, string initial = "", Action<string> onChanged = null, string id = null, Action<string> onTyping = null)
	{
		CustomUIElement customUIElement = Spawn(CustomUIManager.ElementKind.Input, id);
		if (customUIElement.input != null && customUIElement.input.placeholder is TMP_Text tMP_Text)
		{
			tMP_Text.text = placeholder;
		}
		customUIElement.SetString(initial);
		if (onChanged != null)
		{
			customUIElement.OnInput(onChanged);
		}
		if (onTyping != null)
		{
			customUIElement.OnInputChanged(onTyping);
		}
		return this;
	}

	public CustomUIBuilder AddDropdown(IList<string> options, int initialIndex = 0, Action<int> onChanged = null, string id = null)
	{
		CustomUIElement customUIElement = Spawn(CustomUIManager.ElementKind.Dropdown, id);
		customUIElement.SetOptions(options);
		customUIElement.SetIndex(initialIndex);
		if (onChanged != null)
		{
			customUIElement.OnDropdown(onChanged);
		}
		return this;
	}

	public CustomUIBuilder AddProgressBar(float initial = 0f, string id = null)
	{
		Spawn(CustomUIManager.ElementKind.ProgressBar, id).SetProgress(initial);
		return this;
	}

	public CustomUIBuilder AddSpace(float pixels)
	{
		LayoutElement layoutElement = NewChild("space").AddComponent<LayoutElement>();
		if (CurrentParent.GetComponent<HorizontalLayoutGroup>() != null)
		{
			layoutElement.preferredWidth = pixels;
		}
		else
		{
			layoutElement.preferredHeight = pixels;
		}
		return this;
	}

	public CustomUIBuilder AddFlexibleSpace()
	{
		LayoutElement layoutElement = NewChild("flexible_space").AddComponent<LayoutElement>();
		if (CurrentParent.GetComponent<HorizontalLayoutGroup>() != null)
		{
			layoutElement.flexibleWidth = 1f;
		}
		else
		{
			layoutElement.flexibleHeight = 1f;
		}
		return this;
	}

	public CustomUIBuilder WithTooltip(string tooltipText)
	{
		Last?.SetTooltip(tooltipText);
		return this;
	}

	private GameObject NewChild(string name)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.transform.SetParent(CurrentParent, worldPositionStays: false);
		return gameObject;
	}

	private static void ConfigureGroup(HorizontalOrVerticalLayoutGroup group, float spacing, TextAnchor alignment = TextAnchor.UpperLeft)
	{
		group.spacing = spacing;
		group.childControlWidth = true;
		group.childControlHeight = true;
		group.childForceExpandWidth = false;
		group.childForceExpandHeight = false;
		group.childAlignment = alignment;
	}

	public CustomUIBuilder BeginRow(float spacing = 4f, TextAnchor alignment = TextAnchor.UpperLeft)
	{
		GameObject gameObject = NewChild("row");
		ConfigureGroup(gameObject.AddComponent<HorizontalLayoutGroup>(), spacing, alignment);
		parents.Push(gameObject.transform as RectTransform);
		return this;
	}

	public CustomUIBuilder BeginColumn(float spacing = 4f, TextAnchor alignment = TextAnchor.UpperLeft)
	{
		GameObject gameObject = NewChild("column");
		ConfigureGroup(gameObject.AddComponent<VerticalLayoutGroup>(), spacing, alignment);
		parents.Push(gameObject.transform as RectTransform);
		return this;
	}

	public CustomUIBuilder BeginPanel(float width = -1f, float height = -1f, TextAnchor alignment = TextAnchor.MiddleCenter, float padding = 8f, float spacing = 4f, string id = null)
	{
		GameObject gameObject = manager.SpawnElement(CustomUIManager.ElementKind.Panel, CurrentParent, window, string.IsNullOrEmpty(id) ? "panel" : ("panel_" + id));
		if (gameObject == null)
		{
			gameObject = NewChild("panel");
		}
		VerticalLayoutGroup verticalLayoutGroup = gameObject.GetComponent<VerticalLayoutGroup>();
		if (verticalLayoutGroup == null)
		{
			verticalLayoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
		}
		ConfigureGroup(verticalLayoutGroup, spacing, alignment);
		int num = Mathf.RoundToInt(padding);
		verticalLayoutGroup.padding = new RectOffset(num, num, num, num);
		LayoutElement layoutElement = gameObject.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = gameObject.AddComponent<LayoutElement>();
		}
		if (width > 0f)
		{
			layoutElement.minWidth = width;
			layoutElement.preferredWidth = width;
			layoutElement.flexibleWidth = 0f;
		}
		else
		{
			layoutElement.flexibleWidth = 1f;
		}
		if (height > 0f)
		{
			layoutElement.minHeight = height;
			layoutElement.preferredHeight = height;
			layoutElement.flexibleHeight = 0f;
		}
		CustomUIElement customUIElement = CustomUIElement.Bind(gameObject, id);
		if (!string.IsNullOrEmpty(id))
		{
			window.RegisterElement(customUIElement);
		}
		Last = customUIElement;
		parents.Push(gameObject.transform as RectTransform);
		return this;
	}

	public CustomUIBuilder BeginGrid(int columns, float cellWidth, float cellHeight, float spacing = 4f)
	{
		GameObject gameObject = NewChild("grid");
		GridLayoutGroup gridLayoutGroup = gameObject.AddComponent<GridLayoutGroup>();
		gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		gridLayoutGroup.constraintCount = Mathf.Max(1, columns);
		gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
		gridLayoutGroup.spacing = new Vector2(spacing, spacing);
		parents.Push(gameObject.transform as RectTransform);
		return this;
	}

	public CustomUIBuilder BeginScroll(float height)
	{
		return BeginScrollInternal(delegate(LayoutElement layout)
		{
			layout.preferredHeight = height;
			layout.flexibleWidth = 1f;
		});
	}

	public CustomUIBuilder BeginScrollFlexible(float minHeight = 0f)
	{
		return BeginScrollInternal(delegate(LayoutElement layout)
		{
			layout.minHeight = minHeight;
			layout.flexibleHeight = 1f;
			layout.flexibleWidth = 1f;
		});
	}

	private CustomUIBuilder BeginScrollInternal(Action<LayoutElement> configure)
	{
		GameObject gameObject = manager.SpawnElement(CustomUIManager.ElementKind.ScrollView, CurrentParent, window, "scroll");
		if (gameObject == null)
		{
			Debug.LogError("[CustomUI] No scrollViewPrefab assigned, BeginScroll falls back to a plain column");
			return BeginColumn();
		}
		LayoutElement layoutElement = gameObject.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = gameObject.AddComponent<LayoutElement>();
		}
		configure(layoutElement);
		ScrollRect componentInChildren = gameObject.GetComponentInChildren<ScrollRect>(includeInactive: true);
		if (componentInChildren == null || componentInChildren.content == null)
		{
			Debug.LogError("[CustomUI] scrollViewPrefab has no ScrollRect with a content, using prefab root as parent");
			parents.Push(gameObject.transform as RectTransform);
		}
		else
		{
			parents.Push(componentInChildren.content);
		}
		return this;
	}

	public CustomUIBuilder End()
	{
		if (parents.Count <= 1)
		{
			Debug.LogWarning("[CustomUI] End() called with no open container");
			return this;
		}
		parents.Pop();
		return this;
	}

	public CustomUIBuilder BeginTabs(float spacing = 4f)
	{
		GameObject gameObject = NewChild("tabs");
		ConfigureGroup(gameObject.AddComponent<VerticalLayoutGroup>(), spacing);
		parents.Push(gameObject.transform as RectTransform);
		GameObject gameObject2 = NewChild("tab_bar");
		ConfigureGroup(gameObject2.AddComponent<HorizontalLayoutGroup>(), spacing);
		GameObject gameObject3 = NewChild("tab_pages");
		ConfigureGroup(gameObject3.AddComponent<VerticalLayoutGroup>(), spacing);
		tabContexts.Push(new TabContext
		{
			tabBar = (gameObject2.transform as RectTransform),
			pageHost = (gameObject3.transform as RectTransform)
		});
		return this;
	}

	public CustomUIBuilder BeginTab(string tabName)
	{
		if (tabContexts.Count == 0)
		{
			Debug.LogError("[CustomUI] BeginTab called outside BeginTabs, opening a plain column instead");
			return BeginColumn();
		}
		TabContext ctx = tabContexts.Peek();
		Button button = null;
		GameObject gameObject = manager.SpawnElement(CustomUIManager.ElementKind.Button, ctx.tabBar, window, "tab_" + tabName);
		if (gameObject != null)
		{
			TextMeshProUGUI componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
			if (componentInChildren != null)
			{
				componentInChildren.text = tabName;
			}
			button = gameObject.GetComponentInChildren<Button>(includeInactive: true);
		}
		else
		{
			Debug.LogError("[CustomUI] No buttonPrefab assigned, tab has no clickable header");
		}
		GameObject gameObject2 = new GameObject("page_" + tabName, typeof(RectTransform));
		gameObject2.transform.SetParent(ctx.pageHost, worldPositionStays: false);
		ConfigureGroup(gameObject2.AddComponent<VerticalLayoutGroup>(), 4f);
		ctx.tabs.Add((button, gameObject2));
		int index = ctx.tabs.Count - 1;
		if (button != null)
		{
			button.onClick.AddListener(delegate
			{
				SelectTab(ctx, index);
			});
		}
		parents.Push(gameObject2.transform as RectTransform);
		return this;
	}

	public CustomUIBuilder EndTabs()
	{
		if (tabContexts.Count == 0)
		{
			Debug.LogWarning("[CustomUI] EndTabs called without BeginTabs");
			return this;
		}
		TabContext tabContext = tabContexts.Pop();
		End();
		if (tabContext.tabs.Count > 0)
		{
			SelectTab(tabContext, 0);
		}
		return this;
	}

	private static void SelectTab(TabContext ctx, int index)
	{
		for (int i = 0; i < ctx.tabs.Count; i++)
		{
			var (button, gameObject) = ctx.tabs[i];
			if (gameObject != null)
			{
				gameObject.SetActive(i == index);
			}
			if (button != null)
			{
				button.interactable = i != index;
			}
		}
	}
}

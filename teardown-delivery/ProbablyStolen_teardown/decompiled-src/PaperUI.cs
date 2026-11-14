using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class PaperUI
{
	public class Binding
	{
		public PaperElementData data;

		public GameObject go;

		public RectTransform rect;

		public TextMeshProUGUI text;

		public Image image;

		public Button button;

		public CustomUIElement handle;
	}

	public const string LAYER = "paper";

	private readonly Dictionary<string, Action> actions = new Dictionary<string, Action>();

	private readonly Dictionary<string, Binding> byId = new Dictionary<string, Binding>();

	private readonly List<Dictionary<PaperElementData, Binding>> pageBindings = new List<Dictionary<PaperElementData, Binding>>();

	private readonly Dictionary<PaperElementData, Binding> chromeBindings = new Dictionary<PaperElementData, Binding>();

	private readonly Dictionary<PaperElementData, Binding> previewBindings = new Dictionary<PaperElementData, Binding>();

	private readonly List<GameObject> pageObjects = new List<GameObject>();

	private GameObject chromeObject;

	private GameObject previewObject;

	private readonly List<Binding> pageIndicators = new List<Binding>();

	private readonly List<Binding> flipLeftButtons = new List<Binding>();

	private readonly List<Binding> flipRightButtons = new List<Binding>();

	private StoreUI storeUI;

	private UIDragHandler dragHandler;

	private PaperUIPanel panel;

	private bool built;

	private bool dirty;

	private bool shownOnce;

	private PaperPageData lastPage;

	private const float HIDDEN_ALPHA = 0.35f;

	private static readonly Color DEFAULT_SILHOUETTE = new Color(0.16f, 0.17f, 0.2f, 1f);

	private const float ARROW_WING_ANGLE = 32f;

	public string Id { get; }

	public string ModId { get; }

	public PaperBookData Data { get; private set; }

	public GameObject Root { get; private set; }

	public RectTransform Rect
	{
		get
		{
			if (!(Root != null))
			{
				return null;
			}
			return Root.transform as RectTransform;
		}
	}

	public RectTransform Content { get; private set; }

	public Vector2 DesignSize { get; private set; }

	public Vector2 DisplaySize { get; private set; }

	public float DisplayScale { get; private set; } = 1f;


	public int CurrentPage { get; private set; }

	public int PageCount => Data.pages.Count;

	public bool IsAlive => Root != null;

	public bool IsVisible
	{
		get
		{
			if (Root != null)
			{
				return Root.activeSelf;
			}
			return false;
		}
	}

	public bool EditorMode { get; set; }

	public string PreviewTemplate { get; set; }

	public PaperElementData Last { get; private set; }

	public event Action<PaperUI, int> PageShown;

	public event Action<PaperUI> Closed;

	public event Action<PaperUI, string> ActionFired;

	public event Action<PaperUI> Rebuilt;

	private PaperUI(string id, PaperBookData data, string modId)
	{
		Id = id;
		ModId = modId;
		Data = data;
		PaperJson.Normalize(data);
	}

	public static PaperUI Create(string id, string background, float width = 0f, float height = 0f, string modId = null)
	{
		string text = ((!string.IsNullOrEmpty(modId) && id.StartsWith(modId + ":")) ? id.Substring(modId.Length + 1) : id);
		PaperBookData paperBookData = new PaperBookData
		{
			id = text,
			title = text,
			background = background,
			width = width,
			height = height
		};
		PaperUI paperUI = new PaperUI(id, paperBookData, modId);
		paperUI.ResolveSizes(out var design, out var display);
		paperBookData.chrome = PaperStarters.DefaultChrome(design, display.x / Mathf.Max(1f, design.x));
		return paperUI;
	}

	public static PaperUI FromData(string id, PaperBookData data, string modId)
	{
		return new PaperUI(id, data, modId);
	}

	public PaperUI AddPage(string name = null, string template = null)
	{
		lastPage = new PaperPageData
		{
			name = (name ?? $"Page {Data.pages.Count + 1}"),
			template = template
		};
		Data.pages.Add(lastPage);
		MarkDirty();
		return this;
	}

	private PaperPageData CurrentAuthoringPage()
	{
		if (lastPage == null)
		{
			if (Data.pages.Count == 0)
			{
				AddPage();
			}
			lastPage = Data.pages[Data.pages.Count - 1];
		}
		return lastPage;
	}

	private PaperUI AddElement(PaperElementData element)
	{
		CurrentAuthoringPage().elements.Add(element);
		Last = element;
		MarkDirty();
		return this;
	}

	public PaperUI AddText(float x, float y, float w, float h, string text, string style = "body", string id = null, string align = "left", float fontSize = 0f)
	{
		return AddElement(new PaperElementData
		{
			type = "text",
			x = x,
			y = y,
			w = w,
			h = h,
			text = text,
			style = style,
			id = id,
			align = align,
			fontSize = fontSize
		});
	}

	public PaperUI AddLocalizedText(float x, float y, float w, float h, string locKey, string style = "body", string id = null, string align = "left", float fontSize = 0f)
	{
		return AddElement(new PaperElementData
		{
			type = "text",
			x = x,
			y = y,
			w = w,
			h = h,
			loc = locKey,
			style = style,
			id = id,
			align = align,
			fontSize = fontSize
		});
	}

	public PaperUI AddTextBelow(string text, float gap = 6f, string style = "body", string id = null, float minHeight = 12f)
	{
		PaperElementData last = Last;
		float x = last?.x ?? 20f;
		float y = ((last != null) ? (last.y + last.h + gap) : 20f);
		float w = last?.w ?? Mathf.Max(40f, DesignSizeOrDefault().x - 40f);
		return AddElement(new PaperElementData
		{
			type = "text",
			x = x,
			y = y,
			w = w,
			h = minHeight,
			text = text,
			style = style,
			id = id,
			autoHeight = true
		});
	}

	public PaperUI AddImage(float x, float y, float w, float h, string spriteKey, string fit = "stretch", string id = null)
	{
		return AddElement(new PaperElementData
		{
			type = "image",
			x = x,
			y = y,
			w = w,
			h = h,
			sprite = spriteKey,
			fit = fit,
			id = id
		});
	}

	public PaperUI AddButton(float x, float y, float w, float h, string label, Action onClick, string id = null, string action = null)
	{
		string text = action ?? id ?? $"button_{Data.pages.Count}_{CurrentAuthoringPage().elements.Count}";
		if (onClick != null)
		{
			actions[text] = onClick;
		}
		return AddElement(new PaperElementData
		{
			type = "button",
			x = x,
			y = y,
			w = w,
			h = h,
			text = label,
			style = "button",
			id = id,
			action = text
		});
	}

	public PaperUI AddIconButton(float x, float y, float w, float h, string spriteKey, Action onClick, string id = null, string action = null, string hoverSpriteKey = null)
	{
		string text = action ?? id ?? $"button_{Data.pages.Count}_{CurrentAuthoringPage().elements.Count}";
		if (onClick != null)
		{
			actions[text] = onClick;
		}
		return AddElement(new PaperElementData
		{
			type = "button",
			x = x,
			y = y,
			w = w,
			h = h,
			sprite = spriteKey,
			hoverSprite = hoverSpriteKey,
			id = id,
			action = text
		});
	}

	public PaperUI AddFlow(float x, float y, float w, float h, string flowId, float spacing = 6f, string align = "left")
	{
		return AddElement(new PaperElementData
		{
			type = "flow",
			x = x,
			y = y,
			w = w,
			h = h,
			id = flowId,
			spacing = spacing,
			align = align
		});
	}

	public PaperUI AddFlowText(string flowId, string text, string style = "body", string id = null, float width = 0f)
	{
		return AddElement(new PaperElementData
		{
			type = "text",
			flow = flowId,
			w = width,
			h = 0f,
			text = text,
			style = style,
			id = id,
			autoHeight = true
		});
	}

	public PaperUI AddLine(float x, float y, float x2, float y2, float thickness = 2f, string color = null, string arrow = "none", string id = null)
	{
		return AddElement(new PaperElementData
		{
			type = "line",
			x = x,
			y = y,
			x2 = x2,
			y2 = y2,
			thickness = thickness,
			color = color,
			arrow = (arrow ?? "none"),
			id = id
		});
	}

	public PaperUI AddSilhouette(float x, float y, float w, float h, string spriteKey, string color, float scale = 1f, string id = null)
	{
		return AddElement(new PaperElementData
		{
			type = "image",
			x = x,
			y = y,
			w = w,
			h = h,
			sprite = spriteKey,
			fit = "native",
			scale = scale,
			silhouette = true,
			color = color,
			id = id
		});
	}

	public PaperUI SetSlot(string slot, string value)
	{
		CurrentAuthoringPage().slots[slot] = value;
		MarkDirty();
		return this;
	}

	public PaperUI SetChrome(bool close = true, bool zoom = true, bool flip = true, bool pageIndicator = true)
	{
		ResolveSizes(out var design, out var display);
		Data.chrome = PaperStarters.DefaultChrome(design, display.x / Mathf.Max(1f, design.x), close, zoom, flip, pageIndicator);
		MarkDirty();
		return this;
	}

	public PaperUI SetSize(float width, float height)
	{
		Data.width = width;
		Data.height = height;
		MarkDirty();
		return this;
	}

	public PaperUI SetPosition(float x, float y)
	{
		Data.posX = x;
		Data.posY = y;
		if (Rect != null)
		{
			Rect.anchoredPosition = new Vector2(x, y);
		}
		return this;
	}

	public PaperUI SetSounds(bool enabled)
	{
		Data.sounds = enabled;
		return this;
	}

	public PaperUI OnAction(string action, Action callback)
	{
		if (string.IsNullOrEmpty(action))
		{
			return this;
		}
		if (callback == null)
		{
			actions.Remove(action);
		}
		else
		{
			actions[action] = callback;
		}
		return this;
	}

	public PaperUI OnPageShown(Action<PaperUI, int> callback)
	{
		if (callback != null)
		{
			PageShown += callback;
		}
		return this;
	}

	private void MarkDirty()
	{
		if (built)
		{
			dirty = true;
		}
	}

	private Vector2 DesignSizeOrDefault()
	{
		if (DesignSize.x > 0f)
		{
			return DesignSize;
		}
		ResolveSizes(out var design, out var _);
		return design;
	}

	public PaperUI Show()
	{
		StallWatchdog.Crumb("paper " + Id);
		if (!built || dirty)
		{
			Build();
		}
		if (Root == null)
		{
			return this;
		}
		bool activeSelf = Root.activeSelf;
		Root.SetActive(value: true);
		Root.transform.SetAsLastSibling();
		PaperPanelFocusStamp.BumpPanel(Root);
		if (!activeSelf || !shownOnce)
		{
			PlayPickup();
		}
		shownOnce = true;
		ShowPage(Mathf.Clamp(CurrentPage, 0, Mathf.Max(0, PageCount - 1)), playSound: false);
		return this;
	}

	public void Hide()
	{
		if (Root != null && Root.activeSelf)
		{
			Root.SetActive(value: false);
		}
	}

	public void Close()
	{
		if (!(Root == null))
		{
			bool activeSelf = Root.activeSelf;
			GameObject root = Root;
			Root = null;
			built = false;
			if (panel != null)
			{
				panel.owner = null;
			}
			UnityEngine.Object.Destroy(root);
			if (activeSelf)
			{
				PlayDrop();
				this.Closed?.Invoke(this);
			}
		}
	}

	public void Toggle()
	{
		if (IsVisible)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	public PaperUI Apply()
	{
		if (built)
		{
			Build();
		}
		return this;
	}

	public void Rebuild()
	{
		bool isVisible = IsVisible;
		if (Root != null)
		{
			Data.posX = Rect.anchoredPosition.x;
		}
		if (Root != null)
		{
			Data.posY = Rect.anchoredPosition.y;
		}
		Build();
		if (Root != null)
		{
			Root.SetActive(isVisible);
			if (isVisible)
			{
				ShowPage(CurrentPage, playSound: false);
			}
		}
		this.Rebuilt?.Invoke(this);
	}

	public static IEnumerator SwitchLocale(Locale locale)
	{
		if (!(locale == null))
		{
			yield return WaitForLocalization();
			if (LocalizationSettings.SelectedLocale != locale)
			{
				LocalizationSettings.SelectedLocale = locale;
			}
			yield return WaitForLocalization();
			yield return null;
		}
	}

	private static IEnumerator WaitForLocalization()
	{
		AsyncOperationHandle<LocalizationSettings> init = LocalizationSettings.InitializationOperation;
		while (init.IsValid() && !init.IsDone)
		{
			yield return null;
		}
		AsyncOperationHandle strings = LocalizationSettings.StringDatabase.PreloadOperation;
		while (strings.IsValid() && !strings.IsDone)
		{
			yield return null;
		}
		AsyncOperationHandle assets = LocalizationSettings.AssetDatabase.PreloadOperation;
		while (assets.IsValid() && !assets.IsDone)
		{
			yield return null;
		}
	}

	public void ReplaceData(PaperBookData data)
	{
		if (data != null)
		{
			Data = data;
			PaperJson.Normalize(data);
			lastPage = null;
			Last = null;
			MarkDirty();
		}
	}

	public void ResetBindings()
	{
		this.PageShown = null;
		this.ActionFired = null;
		actions.Clear();
	}

	public void RefreshPage()
	{
		if (Root != null && Root.activeSelf)
		{
			ShowPage(CurrentPage, playSound: false);
		}
	}

	internal void NotifyHidden()
	{
		if (!(Root == null))
		{
			PlayDrop();
			this.Closed?.Invoke(this);
		}
	}

	internal void NotifyDestroyed(bool wasOpen)
	{
		bool num = built;
		Root = null;
		Content = null;
		built = false;
		if (num && wasOpen)
		{
			this.Closed?.Invoke(this);
		}
	}

	public void FlipTo(int page)
	{
		if (page >= 0 && page < PageCount && page != CurrentPage)
		{
			ShowPage(page, playSound: true);
		}
	}

	public void FlipLeft()
	{
		FlipTo(CurrentPage - 1);
	}

	public void FlipRight()
	{
		FlipTo(CurrentPage + 1);
	}

	private void ShowPage(int page, bool playSound)
	{
		if (Root == null)
		{
			return;
		}
		CurrentPage = Mathf.Clamp(page, 0, Mathf.Max(0, PageCount - 1));
		bool flag = previewObject != null;
		for (int i = 0; i < pageObjects.Count; i++)
		{
			if (pageObjects[i] != null)
			{
				pageObjects[i].SetActive(!flag && i == CurrentPage);
			}
		}
		if (previewObject != null)
		{
			previewObject.SetActive(value: true);
		}
		if (pageIndicators.Count > 0)
		{
			string text = ((PageCount > 0) ? LocHelper.GetLocalizedUIOrDefault("ui_page_indicator", "{0}/{1}", CurrentPage + 1, PageCount) : "");
			foreach (Binding pageIndicator in pageIndicators)
			{
				if (pageIndicator.text != null)
				{
					pageIndicator.text.text = text;
				}
			}
		}
		bool flag2 = EditorMode || CurrentPage > 0;
		bool flag3 = EditorMode || CurrentPage < PageCount - 1;
		foreach (Binding flipLeftButton in flipLeftButtons)
		{
			if (flipLeftButton.go != null)
			{
				flipLeftButton.go.SetActive(EditorMode || (flag2 && flipLeftButton.data.visible));
			}
		}
		foreach (Binding flipRightButton in flipRightButtons)
		{
			if (flipRightButton.go != null)
			{
				flipRightButton.go.SetActive(EditorMode || (flag3 && flipRightButton.data.visible));
			}
		}
		if (playSound && Data.sounds && !EditorMode && EmporiumAudioPlayer.Instance != null && EmporiumAudioPlayer.Instance.flip1 != null)
		{
			EmporiumAudioPlayer.Instance.flip1.Play();
		}
		this.PageShown?.Invoke(this, CurrentPage);
	}

	public CustomUIElement Get(string elementId)
	{
		Binding binding = GetBinding(elementId);
		if (binding != null && binding.handle != null)
		{
			return binding.handle;
		}
		Debug.LogWarning("[PaperUI] Book '" + Id + "' has no element '" + elementId + "'");
		return null;
	}

	public bool Has(string elementId)
	{
		if (!string.IsNullOrEmpty(elementId))
		{
			return byId.ContainsKey(elementId);
		}
		return false;
	}

	public PaperUI SetText(string elementId, string text)
	{
		Binding binding = GetBinding(elementId);
		if (binding != null && binding.text != null)
		{
			binding.text.text = text;
		}
		return this;
	}

	public PaperUI SetSprite(string elementId, string spriteKey)
	{
		Binding binding = GetBinding(elementId);
		if (binding != null && binding.image != null)
		{
			Sprite sprite = PaperAssets.Resolve(spriteKey, ModId);
			if (sprite != null)
			{
				ApplySprite(binding.image, sprite, binding.data);
			}
		}
		return this;
	}

	public PaperUI SetVisible(string elementId, bool visible)
	{
		Binding binding = GetBinding(elementId);
		if (binding != null && binding.go != null)
		{
			binding.go.SetActive(visible);
		}
		return this;
	}

	private Binding GetBinding(string elementId)
	{
		if (string.IsNullOrEmpty(elementId))
		{
			return null;
		}
		if (CurrentPage < pageBindings.Count)
		{
			foreach (Binding value2 in pageBindings[CurrentPage].Values)
			{
				if (value2.data.id == elementId)
				{
					return value2;
				}
			}
		}
		if (!byId.TryGetValue(elementId, out var value))
		{
			return null;
		}
		return value;
	}

	public Binding GetBinding(PaperElementData element)
	{
		if (element == null)
		{
			return null;
		}
		if (chromeBindings.TryGetValue(element, out var value))
		{
			return value;
		}
		if (previewObject != null && previewBindings.TryGetValue(element, out var value2))
		{
			return value2;
		}
		if (CurrentPage < pageBindings.Count && pageBindings[CurrentPage].TryGetValue(element, out var value3))
		{
			return value3;
		}
		return null;
	}

	public List<Binding> VisibleBindings()
	{
		List<Binding> list = new List<Binding>();
		if (previewObject != null)
		{
			list.AddRange(previewBindings.Values);
		}
		else if (CurrentPage < pageBindings.Count)
		{
			list.AddRange(pageBindings[CurrentPage].Values);
		}
		list.AddRange(chromeBindings.Values);
		list.Sort((Binding a, Binding b) => SiblingOrder(a).CompareTo(SiblingOrder(b)));
		return list;
	}

	private static int SiblingOrder(Binding binding)
	{
		if (binding.rect == null)
		{
			return 0;
		}
		return ((binding.rect.parent != null && binding.rect.parent.name == "chrome") ? 100000 : 0) + binding.rect.GetSiblingIndex() + ((binding.data.flow != null) ? 50000 : 0);
	}

	public bool IsOverflowing(PaperElementData element)
	{
		Binding binding = GetBinding(element);
		if (binding == null || binding.text == null || element.autoHeight)
		{
			return false;
		}
		binding.text.ForceMeshUpdate();
		return binding.text.isTextOverflowing;
	}

	public string ResolveText(PaperElementData element, PaperPageData page)
	{
		if (!string.IsNullOrEmpty(element.slot) && page != null && page.slots != null && page.slots.TryGetValue(element.slot, out var value))
		{
			return value;
		}
		if (!string.IsNullOrEmpty(element.loc))
		{
			if (string.IsNullOrEmpty(ModId))
			{
				return LocHelper.GetLocalizedUIOrDefault(element.loc, element.loc);
			}
			return ModHelper.GetLocalized(ModId, element.loc);
		}
		if (element.translations != null && element.translations.Count > 0)
		{
			string currentLocaleCode = LocHelper.GetCurrentLocaleCode();
			if (!string.IsNullOrEmpty(currentLocaleCode) && element.translations.TryGetValue(currentLocaleCode, out var value2) && !string.IsNullOrEmpty(value2))
			{
				return value2;
			}
		}
		if (string.IsNullOrEmpty(element.text) && EditorMode && !string.IsNullOrEmpty(element.preview))
		{
			return element.preview;
		}
		if (string.IsNullOrEmpty(element.text) && EditorMode && !string.IsNullOrEmpty(element.slot) && page == null)
		{
			return "[" + element.slot + "]";
		}
		return element.text ?? "";
	}

	public string ResolveSpriteKey(PaperElementData element, PaperPageData page)
	{
		if (!string.IsNullOrEmpty(element.slot) && page != null && page.slots != null && page.slots.TryGetValue(element.slot, out var value))
		{
			return value;
		}
		if (string.IsNullOrEmpty(element.sprite) && element.IsButton && element.IsChromeAction)
		{
			return DefaultChromeSprite(element.action);
		}
		return element.sprite;
	}

	public static string DefaultChromeSprite(string action)
	{
		return action switch
		{
			"close" => "paper_buttons_close1", 
			"zoom" => "paper_buttons_screen_0", 
			"flip_left" => "paper_buttons_l_1", 
			"flip_right" => "paper_buttons1", 
			_ => null, 
		};
	}

	private void ResolveSizes(out Vector2 design, out Vector2 display)
	{
		Sprite sprite = PaperAssets.Resolve(Data.background, ModId, quiet: true);
		Vector2 vector = ((sprite != null) ? sprite.rect.size : new Vector2(314f, 402f));
		design = new Vector2((Data.designWidth > 0f) ? Data.designWidth : vector.x, (Data.designHeight > 0f) ? Data.designHeight : vector.y);
		Vector2 vector2 = PaperAssets.DisplaySizeOf(Data.background);
		display = new Vector2((Data.width > 0f) ? Data.width : ((vector2.x > 0f) ? vector2.x : (design.x * 2f)), (Data.height > 0f) ? Data.height : ((vector2.y > 0f) ? vector2.y : (design.y * 2f)));
	}

	private void Build()
	{
		dirty = false;
		PaperJson.Normalize(Data);
		if (Root == null)
		{
			if (CustomUIManager.Instance == null)
			{
				Debug.LogError("[PaperUI] No CustomUIManager in the scene, cannot build a paper book");
				return;
			}
			RectTransform layerRoot = CustomUIManager.Instance.GetLayerRoot("paper");
			Root = new GameObject("paper_" + Id, typeof(RectTransform));
			Root.transform.SetParent(layerRoot, worldPositionStays: false);
			Root.SetActive(value: false);
			Root.AddComponent<Image>().raycastTarget = true;
			storeUI = Root.AddComponent<StoreUI>();
			storeUI.isPaperUI = true;
			dragHandler = Root.AddComponent<UIDragHandler>();
			Root.AddComponent<PaperPanelFocusStamp>();
			panel = Root.AddComponent<PaperUIPanel>();
			panel.owner = this;
		}
		else
		{
			for (int num = Root.transform.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(Root.transform.GetChild(num).gameObject);
			}
		}
		byId.Clear();
		pageBindings.Clear();
		chromeBindings.Clear();
		previewBindings.Clear();
		pageObjects.Clear();
		pageIndicators.Clear();
		flipLeftButtons.Clear();
		flipRightButtons.Clear();
		chromeObject = null;
		previewObject = null;
		ResolveSizes(out var design, out var display);
		DesignSize = design;
		DisplaySize = display;
		DisplayScale = ((design.x > 0f) ? (display.x / design.x) : 1f);
		RectTransform rect = Rect;
		Vector2 anchorMin = (rect.anchorMax = new Vector2(0.5f, 0.5f));
		rect.anchorMin = anchorMin;
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.sizeDelta = display;
		rect.anchoredPosition = new Vector2(Data.posX, Data.posY);
		Image component = Root.GetComponent<Image>();
		Sprite sprite2 = (component.sprite = PaperAssets.ResolveBackground(Data, ModId));
		component.color = ((sprite2 != null) ? Color.white : new Color(0.9f, 0.85f, 0.75f, 1f));
		component.type = ((sprite2 != null && sprite2.border != Vector4.zero) ? Image.Type.Sliced : Image.Type.Simple);
		component.preserveAspect = false;
		storeUI.zoomable = Data.zoomable && !EditorMode;
		dragHandler.enabled = Data.draggable && !EditorMode;
		GameObject gameObject = new GameObject("content", typeof(RectTransform));
		gameObject.transform.SetParent(Root.transform, worldPositionStays: false);
		Content = gameObject.transform as RectTransform;
		RectTransform content = Content;
		anchorMin = (Content.anchorMax = new Vector2(0.5f, 0.5f));
		content.anchorMin = anchorMin;
		Content.pivot = new Vector2(0.5f, 0.5f);
		Content.sizeDelta = design;
		Content.anchoredPosition = Vector2.zero;
		Content.localScale = new Vector3(DisplayScale, (design.y > 0f) ? (display.y / design.y) : DisplayScale, 1f);
		if (EditorMode)
		{
			CanvasGroup canvasGroup = gameObject.AddComponent<CanvasGroup>();
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = true;
		}
		for (int i = 0; i < Data.pages.Count; i++)
		{
			PaperPageData paperPageData = Data.pages[i];
			GameObject gameObject2 = new GameObject($"page_{i}", typeof(RectTransform));
			StretchToParent(gameObject2.transform as RectTransform, Content);
			Dictionary<PaperElementData, Binding> dictionary = new Dictionary<PaperElementData, Binding>();
			PaperTemplateData template = Data.GetTemplate(paperPageData.template);
			List<PaperElementData> list = new List<PaperElementData>();
			if (template != null)
			{
				list.AddRange(template.elements);
			}
			list.AddRange(paperPageData.elements);
			BuildElements(list, gameObject2.transform as RectTransform, paperPageData, dictionary);
			pageBindings.Add(dictionary);
			pageObjects.Add(gameObject2);
			gameObject2.SetActive(value: false);
		}
		if (!string.IsNullOrEmpty(PreviewTemplate))
		{
			PaperTemplateData template2 = Data.GetTemplate(PreviewTemplate);
			if (template2 != null)
			{
				previewObject = new GameObject("template_preview", typeof(RectTransform));
				StretchToParent(previewObject.transform as RectTransform, Content);
				BuildElements(template2.elements, previewObject.transform as RectTransform, null, previewBindings);
			}
		}
		chromeObject = new GameObject("chrome", typeof(RectTransform));
		StretchToParent(chromeObject.transform as RectTransform, Content);
		BuildElements(Data.chrome, chromeObject.transform as RectTransform, null, chromeBindings);
		built = true;
	}

	private static void StretchToParent(RectTransform rect, RectTransform parent)
	{
		rect.SetParent(parent, worldPositionStays: false);
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
	}

	private void BuildElements(List<PaperElementData> elements, RectTransform parent, PaperPageData page, Dictionary<PaperElementData, Binding> bindings)
	{
		Dictionary<string, RectTransform> dictionary = new Dictionary<string, RectTransform>();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (PaperElementData element in elements)
		{
			if (element == null || !element.IsFlow)
			{
				continue;
			}
			if (!element.visible && !EditorMode)
			{
				if (!string.IsNullOrEmpty(element.id))
				{
					hashSet.Add(element.id);
				}
				continue;
			}
			Binding binding2 = (bindings[element] = BuildFlow(element, parent));
			if (!string.IsNullOrEmpty(element.id))
			{
				dictionary[element.id] = binding2.rect;
				byId[element.id] = binding2;
			}
		}
		foreach (PaperElementData element2 in elements)
		{
			if (element2 == null || element2.IsFlow || (!element2.visible && !EditorMode) || (!string.IsNullOrEmpty(element2.flow) && hashSet.Contains(element2.flow)))
			{
				continue;
			}
			RectTransform rectTransform = parent;
			bool flag = false;
			if (!string.IsNullOrEmpty(element2.flow))
			{
				if (dictionary.TryGetValue(element2.flow, out var value))
				{
					rectTransform = value;
					flag = true;
				}
				else
				{
					Debug.LogWarning("[PaperUI] Book '" + Id + "': element '" + element2.DisplayName + "' refers to unknown flow '" + element2.flow + "'");
				}
			}
			Binding binding3 = element2.type switch
			{
				"image" => BuildImage(element2, rectTransform, page), 
				"button" => BuildButton(element2, rectTransform, page), 
				"line" => BuildLine(element2, rectTransform), 
				_ => BuildText(element2, rectTransform, page), 
			};
			if (flag)
			{
				ConfigureFlowChild(binding3, element2, rectTransform);
			}
			if (!element2.visible && EditorMode)
			{
				FadeHidden(binding3.go);
			}
			bindings[element2] = binding3;
			if (!string.IsNullOrEmpty(element2.id))
			{
				byId[element2.id] = binding3;
			}
			if (element2.action == "page_indicator")
			{
				pageIndicators.Add(binding3);
			}
			else if (element2.action == "flip_left")
			{
				flipLeftButtons.Add(binding3);
			}
			else if (element2.action == "flip_right")
			{
				flipRightButtons.Add(binding3);
			}
		}
	}

	private static void FadeHidden(GameObject go)
	{
		CanvasGroup canvasGroup = go.GetComponent<CanvasGroup>();
		if (canvasGroup == null)
		{
			canvasGroup = go.AddComponent<CanvasGroup>();
		}
		canvasGroup.alpha = 0.35f;
	}

	public static void SetRect(RectTransform rect, float x, float y, float width, float height)
	{
		Vector2 anchorMin = (rect.anchorMax = new Vector2(0f, 1f));
		rect.anchorMin = anchorMin;
		rect.pivot = new Vector2(0f, 1f);
		rect.anchoredPosition = new Vector2(x, 0f - y);
		rect.sizeDelta = new Vector2(width, height);
	}

	private static Binding NewBinding(PaperElementData element, RectTransform parent, string name)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		RectTransform rect = gameObject.transform as RectTransform;
		SetRect(rect, element.x, element.y, element.w, element.h);
		return new Binding
		{
			data = element,
			go = gameObject,
			rect = rect
		};
	}

	private Binding BuildFlow(PaperElementData element, RectTransform parent)
	{
		Binding binding = NewBinding(element, parent, "flow_" + element.DisplayName);
		VerticalLayoutGroup verticalLayoutGroup = binding.go.AddComponent<VerticalLayoutGroup>();
		verticalLayoutGroup.spacing = element.spacing;
		verticalLayoutGroup.padding = new RectOffset(0, 0, 0, 0);
		verticalLayoutGroup.childControlWidth = true;
		verticalLayoutGroup.childControlHeight = true;
		verticalLayoutGroup.childForceExpandWidth = false;
		verticalLayoutGroup.childForceExpandHeight = false;
		verticalLayoutGroup.childAlignment = ((element.align == "center") ? TextAnchor.UpperCenter : ((element.align == "right") ? TextAnchor.UpperRight : TextAnchor.UpperLeft));
		binding.handle = CustomUIElement.Bind(binding.go, element.id);
		return binding;
	}

	private static void ConfigureFlowChild(Binding binding, PaperElementData element, RectTransform flow)
	{
		LayoutElement layoutElement = binding.go.AddComponent<LayoutElement>();
		if (element.w > 0f)
		{
			layoutElement.preferredWidth = element.w;
			layoutElement.minWidth = element.w;
		}
		else
		{
			layoutElement.flexibleWidth = 1f;
			layoutElement.preferredWidth = flow.sizeDelta.x;
		}
		if (!element.autoHeight && element.h > 0f)
		{
			layoutElement.preferredHeight = element.h;
			layoutElement.minHeight = element.h;
		}
		ContentSizeFitter component = binding.go.GetComponent<ContentSizeFitter>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	private Binding BuildText(PaperElementData element, RectTransform parent, PaperPageData page)
	{
		Binding binding = NewBinding(element, parent, "text_" + element.DisplayName);
		GameObject go = binding.go;
		go.SetActive(value: false);
		PaperTextStyle paperTextStyle = PaperTextStyle.Get(element.style);
		TextMeshProUGUI textMeshProUGUI = go.AddComponent<TextMeshProUGUI>();
		TMP_FontAsset tMP_FontAsset = paperTextStyle.Font();
		if (tMP_FontAsset != null)
		{
			textMeshProUGUI.font = tMP_FontAsset;
		}
		textMeshProUGUI.fontSize = ((element.fontSize > 0f) ? element.fontSize : paperTextStyle.size);
		textMeshProUGUI.fontStyle = paperTextStyle.fontStyle;
		textMeshProUGUI.color = PaperTextStyle.ParseColor(element.color, paperTextStyle.color);
		textMeshProUGUI.alignment = PaperTextStyle.Alignment(element.align, element.valign);
		textMeshProUGUI.enableWordWrapping = element.wrap;
		textMeshProUGUI.enableAutoSizing = false;
		textMeshProUGUI.overflowMode = TextOverflowModes.Overflow;
		textMeshProUGUI.raycastTarget = element.raycast;
		textMeshProUGUI.richText = true;
		textMeshProUGUI.text = ResolveText(element, page);
		go.AddComponent<LocalizeStringEvent>();
		TMPLocalizer tMPLocalizer = go.AddComponent<TMPLocalizer>();
		tMPLocalizer.ChangeFontOnly = true;
		tMPLocalizer.fontRole = paperTextStyle.role;
		if (element.autoHeight && string.IsNullOrEmpty(element.flow))
		{
			go.AddComponent<LayoutElement>().minHeight = element.h;
			go.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		}
		go.SetActive(value: true);
		binding.text = textMeshProUGUI;
		binding.handle = CustomUIElement.Bind(go, element.id);
		return binding;
	}

	private Binding BuildImage(PaperElementData element, RectTransform parent, PaperPageData page)
	{
		Binding binding = NewBinding(element, parent, "image_" + element.DisplayName);
		Image image = binding.go.AddComponent<Image>();
		image.raycastTarget = element.raycast;
		Sprite sprite = PaperAssets.Resolve(ResolveSpriteKey(element, page), ModId);
		ApplySprite(image, sprite, element);
		if (sprite == null)
		{
			image.color = new Color(1f, 0f, 1f, EditorMode ? 0.4f : 0f);
		}
		binding.image = image;
		binding.handle = CustomUIElement.Bind(binding.go, element.id);
		return binding;
	}

	private static void ApplySprite(Image image, Sprite sprite, PaperElementData element)
	{
		Sprite sprite2 = sprite;
		if (sprite != null && element.silhouette)
		{
			Sprite sprite3 = PaperSpriteEffects.Silhouette(sprite, PaperTextStyle.ParseColor(element.color, DEFAULT_SILHOUETTE));
			if (sprite3 != null)
			{
				sprite2 = sprite3;
			}
		}
		if (sprite2 != null && !string.IsNullOrEmpty(element.fill))
		{
			sprite2 = PaperSpriteEffects.FillEnclosed(sprite2, PaperTextStyle.ParseColor(element.fill, Color.white));
		}
		image.sprite = sprite2;
		image.color = ((element.silhouette || string.IsNullOrEmpty(element.color)) ? Color.white : PaperTextStyle.ParseColor(element.color, Color.white));
		image.preserveAspect = element.fit == "fit";
		image.type = ((sprite2 != null && sprite2.border != Vector4.zero && element.fit == "stretch") ? Image.Type.Sliced : Image.Type.Simple);
		image.pixelsPerUnitMultiplier = 1f;
		RectTransform rectTransform = image.rectTransform;
		if (element.fit == "native" && sprite2 != null)
		{
			Vector2 vector = sprite2.rect.size * Mathf.Max(0.01f, element.scale);
			SetRect(rectTransform, element.x + (element.w - vector.x) * 0.5f, element.y + (element.h - vector.y) * 0.5f, vector.x, vector.y);
		}
		else if (string.IsNullOrEmpty(element.flow))
		{
			SetRect(rectTransform, element.x, element.y, element.w, element.h);
		}
	}

	private Binding BuildLine(PaperElementData element, RectTransform parent)
	{
		Binding binding = NewBinding(element, parent, "line_" + element.DisplayName);
		Color color = PaperTextStyle.ParseColor(element.color, PaperTextStyle.Get("body").color);
		binding.image = NewBar(binding.go.transform, "bar", color, element.raycast);
		bool num = element.arrow == "end" || element.arrow == "both";
		bool flag = element.arrow == "start" || element.arrow == "both";
		if (num)
		{
			NewBar(binding.go.transform, "head_end_a", color, raycast: false);
			NewBar(binding.go.transform, "head_end_b", color, raycast: false);
		}
		if (flag)
		{
			NewBar(binding.go.transform, "head_start_a", color, raycast: false);
			NewBar(binding.go.transform, "head_start_b", color, raycast: false);
		}
		LayoutLine(binding);
		binding.handle = CustomUIElement.Bind(binding.go, element.id);
		return binding;
	}

	private static Image NewBar(Transform parent, string name, Color color, bool raycast)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		Image image = gameObject.AddComponent<Image>();
		image.color = color;
		image.raycastTarget = raycast;
		return image;
	}

	private static void LayoutLine(Binding binding)
	{
		PaperElementData data = binding.data;
		Rect rect = data.LineBounds();
		SetRect(binding.rect, rect.x, rect.y, rect.width, rect.height);
		Vector2 vector = new Vector2(data.x - rect.x, data.y - rect.y);
		Vector2 vector2 = new Vector2(data.x2 - rect.x, data.y2 - rect.y);
		Vector2 vector3 = vector2 - vector;
		float magnitude = vector3.magnitude;
		vector3 = ((magnitude < 0.01f) ? Vector2.right : (vector3 / magnitude));
		float num = Mathf.Max(0.5f, data.thickness);
		float length = Mathf.Max(0f, data.arrowSize);
		RectTransform rect2 = binding.rect;
		PlaceBar(rect2.Find("bar"), (vector + vector2) * 0.5f, vector3, Mathf.Max(magnitude, num), num);
		PlaceWing(rect2.Find("head_end_a"), vector2, -vector3, length, num, 32f);
		PlaceWing(rect2.Find("head_end_b"), vector2, -vector3, length, num, -32f);
		PlaceWing(rect2.Find("head_start_a"), vector, vector3, length, num, 32f);
		PlaceWing(rect2.Find("head_start_b"), vector, vector3, length, num, -32f);
	}

	private static void PlaceWing(Transform wing, Vector2 tip, Vector2 back, float length, float thickness, float angle)
	{
		if (!(wing == null))
		{
			float f = angle * (MathF.PI / 180f);
			Vector2 vector = new Vector2(back.x * Mathf.Cos(f) - back.y * Mathf.Sin(f), back.x * Mathf.Sin(f) + back.y * Mathf.Cos(f));
			PlaceBar(wing, tip + vector * ((length - thickness * 0.5f) * 0.5f), vector, length + thickness * 0.5f, thickness);
		}
	}

	private static void PlaceBar(Transform bar, Vector2 center, Vector2 direction, float length, float thickness)
	{
		if (!(bar == null))
		{
			RectTransform obj = bar as RectTransform;
			Vector2 anchorMin = (obj.anchorMax = new Vector2(0f, 1f));
			obj.anchorMin = anchorMin;
			obj.pivot = new Vector2(0.5f, 0.5f);
			obj.anchoredPosition = new Vector2(center.x, 0f - center.y);
			obj.sizeDelta = new Vector2(length, thickness);
			obj.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(0f - direction.y, direction.x) * 57.29578f);
		}
	}

	public void RefreshLine(PaperElementData element)
	{
		Binding binding = GetBinding(element);
		if (binding != null && binding.rect != null && element.IsLine)
		{
			LayoutLine(binding);
		}
	}

	private Binding BuildButton(PaperElementData element, RectTransform parent, PaperPageData page)
	{
		Binding binding = NewBinding(element, parent, "button_" + element.DisplayName);
		GameObject go = binding.go;
		Image image = go.AddComponent<Image>();
		image.raycastTarget = true;
		int num;
		object key;
		if (!element.IsIconButton)
		{
			num = (element.IsChromeAction ? 1 : 0);
			if (num == 0)
			{
				key = "blue_button_2x";
				goto IL_005d;
			}
		}
		else
		{
			num = 1;
		}
		key = ResolveSpriteKey(element, page);
		goto IL_005d;
		IL_005d:
		Sprite sprite2 = (image.sprite = PaperAssets.Resolve((string)key, ModId));
		image.type = ((sprite2 != null && sprite2.border != Vector4.zero) ? Image.Type.Sliced : Image.Type.Simple);
		image.color = ((sprite2 != null) ? Color.white : new Color(0.3f, 0.5f, 0.8f, 1f));
		Button button = go.AddComponent<Button>();
		button.targetGraphic = image;
		Sprite sprite3 = (string.IsNullOrEmpty(element.hoverSprite) ? null : PaperAssets.Resolve(element.hoverSprite, ModId));
		if (sprite3 != null)
		{
			SpriteState spriteState = button.spriteState;
			spriteState.highlightedSprite = sprite3;
			spriteState.pressedSprite = sprite3;
			button.spriteState = spriteState;
			button.transition = Selectable.Transition.SpriteSwap;
		}
		else
		{
			button.transition = Selectable.Transition.ColorTint;
			ColorBlock colors = button.colors;
			colors.highlightedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
			colors.pressedColor = new Color(0.65f, 0.65f, 0.65f, 1f);
			button.colors = colors;
		}
		string action = element.action;
		button.onClick.AddListener(delegate
		{
			HandleAction(action);
		});
		if (num == 0)
		{
			GameObject gameObject = new GameObject("label", typeof(RectTransform));
			gameObject.SetActive(value: false);
			gameObject.transform.SetParent(go.transform, worldPositionStays: false);
			RectTransform obj = gameObject.transform as RectTransform;
			obj.anchorMin = Vector2.zero;
			obj.anchorMax = Vector2.one;
			obj.offsetMin = new Vector2(4f, 2f);
			obj.offsetMax = new Vector2(-4f, -2f);
			PaperTextStyle paperTextStyle = PaperTextStyle.Get((string.IsNullOrEmpty(element.style) || element.style == "body") ? "button" : element.style);
			TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
			TMP_FontAsset tMP_FontAsset = paperTextStyle.Font();
			if (tMP_FontAsset != null)
			{
				textMeshProUGUI.font = tMP_FontAsset;
			}
			textMeshProUGUI.fontSize = ((element.fontSize > 0f) ? element.fontSize : paperTextStyle.size);
			textMeshProUGUI.fontStyle = paperTextStyle.fontStyle;
			textMeshProUGUI.color = PaperTextStyle.ParseColor(element.color, paperTextStyle.color);
			textMeshProUGUI.alignment = TextAlignmentOptions.Center;
			textMeshProUGUI.enableWordWrapping = false;
			textMeshProUGUI.enableAutoSizing = false;
			textMeshProUGUI.overflowMode = TextOverflowModes.Overflow;
			textMeshProUGUI.raycastTarget = false;
			textMeshProUGUI.text = ResolveText(element, page);
			gameObject.AddComponent<LocalizeStringEvent>();
			TMPLocalizer tMPLocalizer = gameObject.AddComponent<TMPLocalizer>();
			tMPLocalizer.ChangeFontOnly = true;
			tMPLocalizer.fontRole = paperTextStyle.role;
			gameObject.SetActive(value: true);
			binding.text = textMeshProUGUI;
		}
		binding.image = image;
		binding.button = button;
		binding.handle = CustomUIElement.Bind(go, element.id);
		return binding;
	}

	private void HandleAction(string action)
	{
		if (EditorMode)
		{
			return;
		}
		this.ActionFired?.Invoke(this, action);
		switch (action)
		{
		case "close":
			Hide();
			return;
		case "zoom":
			if (storeUI != null && storeUI.zoomable)
			{
				storeUI.ToggleZoom();
			}
			return;
		case "flip_left":
			FlipLeft();
			return;
		case "flip_right":
			FlipRight();
			return;
		}
		if (string.IsNullOrEmpty(action) || !actions.TryGetValue(action, out var value))
		{
			return;
		}
		try
		{
			value();
		}
		catch (Exception arg)
		{
			Debug.LogError($"[PaperUI] Action '{action}' of book '{Id}' threw: {arg}");
		}
	}

	private void PlayPickup()
	{
		if (Data.sounds && !EditorMode && PaperUIManager.Instance != null && PaperUIManager.Instance.audioSourcePickup != null)
		{
			PaperUIManager.Instance.audioSourcePickup.Play();
		}
	}

	private void PlayDrop()
	{
		if (Data.sounds && !EditorMode && PaperUIManager.Instance != null && PaperUIManager.Instance.audioSourceDrop != null)
		{
			PaperUIManager.Instance.audioSourceDrop.Play();
		}
	}
}

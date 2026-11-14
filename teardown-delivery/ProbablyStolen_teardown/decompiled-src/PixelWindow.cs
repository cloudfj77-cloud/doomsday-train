using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PixelWindow : PixelElement, GraphNodeWindow, GraphNodeStorage, MachineryStorageOwner
{
	public readonly bool canMove;

	public readonly WindowStyle style;

	private readonly TreeNodeRender machineryHeader;

	private readonly TreeNodeRender machineryRule;

	private readonly TreeNodeRender machineryRuleBacking;

	private readonly TreeNodeRender machineryPlate;

	private readonly TreeNodeRender machineryIntake;

	private readonly List<MachineryButton> headerButtons = new List<MachineryButton>();

	private List<GraphNodeStorage> linkedContainerStorage = new List<GraphNodeStorage>();

	private readonly TreeNodeRender background;

	private readonly TreeNodeRender outsideBorder;

	private readonly TreeNodeRender topDragBorder;

	private readonly TreeNodeRender topCloseButton;

	private readonly TreeNodeRender topTitleBackground;

	private readonly TreeNodeRender bottomResizeButton;

	private readonly TreeNodeRender titleText;

	public bool forceRaycast;

	private bool hideBorder;

	private Transform preDockParent;

	private static readonly List<PixelWindow> dockedFloating = new List<PixelWindow>();

	private static bool dockLayoutHooked;

	private static long focusStampCounter;

	public bool isLocked;

	public List<GraphNodeStorage> parents => new List<GraphNodeStorage>(parentItems);

	public List<GraphNodeStorage> children => ChildStorage();

	public List<GameItem> parentItems { get; private set; }

	public GraphNodeStorage child
	{
		get
		{
			List<GraphNodeStorage> list = ChildStorage();
			if (list.Count <= 0)
			{
				return null;
			}
			return list[0];
		}
	}

	public bool canResize
	{
		get
		{
			if (childElement == null)
			{
				return false;
			}
			if (childElement.canResize && canMove)
			{
				return style == WindowStyle.Classic;
			}
			return false;
		}
	}

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	public PixelElement childElement { get; private set; }

	public int titleHeight { get; private set; }

	public int xPadding { get; set; }

	public int yPadding { get; set; }

	public bool hasTitle
	{
		get
		{
			if (titleString != null)
			{
				return titleString.Count() > 0;
			}
			return false;
		}
	}

	public string titleString { get; private set; }

	public float transparency { get; private set; }

	public bool isImportant { get; set; }

	public Action closeCallback { get; set; }

	public Action openCallback { get; set; }

	public long focusStamp { get; private set; }

	public Action stateChangeActionFunc { get; private set; }

	public PixelWindow(bool canMove = false, string title = null, WindowStyle style = WindowStyle.Classic)
		: this(100, 100, canMove, title, style)
	{
	}

	public PixelWindow(int width, int height, bool canMove, string title = null, WindowStyle style = WindowStyle.Classic)
		: this(width, height, canMove, 2, 2, title, style)
	{
	}

	public PixelWindow(int width, int height, bool canMove, int xPadding, int yPadding, string title = null, WindowStyle style = WindowStyle.Classic)
	{
		this.style = style;
		GraphHandler.windowGraphNodeRoots.Add(this);
		this.xPadding = xPadding;
		this.yPadding = yPadding;
		transparency = 0f;
		this.canMove = canMove;
		parentItems = new List<GameItem>();
		handler = TreeNodeRender.Instantiate(this, (title == null) ? null : ("Window (" + title + ")"));
		background = TreeNodeRender.Instantiate().SetParent(handler);
		machineryPlate = TreeNodeRender.Instantiate().SetParent(handler);
		machineryIntake = TreeNodeRender.Instantiate().SetParent(handler);
		machineryHeader = TreeNodeRender.Instantiate().SetParent(handler);
		machineryRuleBacking = TreeNodeRender.Instantiate().SetParent(handler);
		machineryRule = TreeNodeRender.Instantiate().SetParent(handler);
		machineryPlate.image.enabled = false;
		machineryIntake.image.enabled = false;
		machineryHeader.image.enabled = false;
		machineryRuleBacking.image.enabled = false;
		machineryRule.image.enabled = false;
		inner = TreeNodeRender.Instantiate().SetParent(handler);
		topTitleBackground = TreeNodeRender.Instantiate().SetParent(handler);
		titleText = TreeNodeRender.InstantiateText().SetParent(handler);
		outsideBorder = TreeNodeRender.Instantiate().SetParent(handler);
		topDragBorder = TreeNodeRender.Instantiate().SetParent(handler);
		bottomResizeButton = TreeNodeRender.Instantiate().SetParent(handler);
		topCloseButton = TreeNodeRender.Instantiate().SetParent(handler);
		bottomResizeButton.InitEventHandler();
		topCloseButton.InitEventHandler();
		background.InitEventHandler();
		titleString = title;
		titleText.text.text = titleString ?? "";
		titleText.text.color = RenderHandler.current.textColorRed;
		titleText.text.fontStyle = FontStyles.Bold;
		titleText.text.SetNativeSize();
		titleText.text.enabled = hasTitle;
		topTitleBackground.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "text_background");
		topTitleBackground.image.SetNativeSize();
		outsideBorder.image.sprite = RenderHandler.current.outside;
		outsideBorder.image.SetNativeSize();
		outsideBorder.image.fillCenter = false;
		topDragBorder.image.sprite = RenderHandler.current.borderOutsideTopRounded;
		topDragBorder.image.SetNativeSize();
		background.image.sprite = RenderHandler.current.fill;
		background.image.SetNativeSize();
		background.image.raycastTarget = canMove || forceRaycast;
		if (!canMove)
		{
			ToBack();
		}
		Vector2 savedOrigin = default(Vector2);
		Vector2 savedPointer = default(Vector2);
		background.eventHandler.mouseBeginDragFunc.Add(delegate(PointerEventData r)
		{
			savedOrigin = rectTransform.anchoredPosition;
			savedPointer = rectTransform.parent.InverseTransformPoint(r.position);
			ToFront();
			return true;
		});
		background.eventHandler.mouseDragFunc.Add(delegate(PointerEventData r)
		{
			Vector2 vector3 = rectTransform.parent.InverseTransformPoint(r.position);
			rectTransform.anchoredPosition = savedOrigin + (vector3 - savedPointer);
			FixBounds();
			ToFront();
			rectTransform.position = new Vector3(Mathf.RoundToInt(rectTransform.position.x), Mathf.RoundToInt(rectTransform.position.y), rectTransform.position.z);
			return true;
		});
		background.eventHandler.mouseEndDragFunc.Add(delegate
		{
			ToFront();
			return true;
		});
		background.eventHandler.mouseDownFunc.Add(delegate
		{
			ToFront();
			return true;
		});
		Sprite xButtonSprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_top_button_x");
		Sprite xButtonFlatSprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_top_button_x_select");
		Sprite xButtonInvertedSprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_top_button_x_inverted");
		titleHeight = (int)xButtonSprite.rect.height;
		bool isButtonPressed = false;
		topCloseButton.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_top_button_x");
		topCloseButton.image.SetNativeSize();
		topCloseButton.eventHandler.mouseDownFunc.Add(delegate
		{
			topCloseButton.image.sprite = xButtonInvertedSprite;
			isButtonPressed = true;
			ToFront();
			return true;
		});
		topCloseButton.eventHandler.mouseEnterFunc.Add(delegate
		{
			topCloseButton.image.sprite = (isButtonPressed ? xButtonInvertedSprite : xButtonFlatSprite);
			return true;
		});
		topCloseButton.eventHandler.mouseExitFunc.Add(delegate
		{
			topCloseButton.image.sprite = xButtonSprite;
			return true;
		});
		topCloseButton.eventHandler.mouseUpFunc.Add(delegate
		{
			topCloseButton.image.sprite = xButtonSprite;
			isButtonPressed = false;
			return true;
		});
		topCloseButton.eventHandler.mouseClickFunc.Add(delegate
		{
			closeCallback?.Invoke();
			Hide();
			if (parentItems.Any())
			{
				AudioManager.Instance.TryPlayOne(parentItems.First().soundContainerClose, parentItems.First().soundContainerOpen);
			}
			return true;
		});
		Sprite resizeButtonSprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_bottom_resizable_button");
		Sprite resizeButtonDownSprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_bottom_resizable_button_down");
		bottomResizeButton.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_bottom_resizable_button");
		bottomResizeButton.image.SetNativeSize();
		bottomResizeButton.eventHandler.mouseEnterFunc.Add(delegate
		{
			bottomResizeButton.image.sprite = resizeButtonDownSprite;
			return true;
		});
		bottomResizeButton.eventHandler.mouseExitFunc.Add(delegate
		{
			bottomResizeButton.image.sprite = resizeButtonSprite;
			return true;
		});
		bottomResizeButton.eventHandler.mouseDragFunc.Add(delegate(PointerEventData r)
		{
			bottomResizeButton.image.sprite = resizeButtonDownSprite;
			Vector2 position = r.position;
			Vector2 max = RenderHandler.current.windowUICanvas.pixelRect.max;
			Vector2 center = bottomResizeButton.rectTransform.rect.center;
			Vector2 vector = rectTransform.parent.TransformPoint(center);
			position.x = Mathf.Clamp(position.x, 0f, max.x - vector.x);
			position.y = Mathf.Clamp(position.y, max.y - vector.y, max.y);
			Vector2 vector2 = rectTransform.InverseTransformPoint(position);
			ResizePixels((int)(vector2.x + center.x), (int)(0f - vector2.y - center.y));
			ToFront();
			return true;
		});
		bottomResizeButton.eventHandler.mouseEndDragFunc.Add(delegate
		{
			bottomResizeButton.image.sprite = resizeButtonSprite;
			return true;
		});
		topDragBorder.image.enabled = true;
		outsideBorder.image.enabled = true;
		background.image.enabled = true;
		topCloseButton.image.enabled = true;
		bottomResizeButton.image.enabled = true;
		topTitleBackground.image.enabled = true;
		if (style == WindowStyle.Machinery)
		{
			InitMachineryChrome();
		}
		ResizePixels(width, height);
		Validate();
		Hide();
	}

	public PixelWindow SetTitle(string title)
	{
		if (style == WindowStyle.Machinery)
		{
			titleString = title;
			titleText.text.text = title ?? "";
			Validate();
			return this;
		}
		Validate();
		int num = (int)(((float)(widthPixels - xPadding) - topCloseButton.rectTransform.sizeDelta.x - (float)xPadding - (float)xPadding - 0f) / (float)UISettings.current.fontWidth);
		if (num < 4)
		{
			title = "";
			num = 4;
		}
		if (title.Length > num)
		{
			title = title.Substring(0, num - 3) + "...";
		}
		titleString = title;
		if (title != null)
		{
			titleText.rectTransform.sizeDelta = new Vector2(UISettings.current.fontWidth * title.Length, titleText.rectTransform.sizeDelta.y);
		}
		Validate();
		return this;
	}

	public PixelWindow SetTransparency(float transparency)
	{
		this.transparency = transparency;
		if (transparency > 0f)
		{
			outsideBorder.image.sprite = RenderHandler.current.outsideCrop;
			topDragBorder.image.sprite = RenderHandler.current.borderOutsideTopRoundedCrop;
		}
		else
		{
			outsideBorder.image.sprite = RenderHandler.current.outside;
			topDragBorder.image.sprite = RenderHandler.current.borderOutsideTopRounded;
		}
		Validate();
		return this;
	}

	public void Validate()
	{
		if (style == WindowStyle.Machinery)
		{
			MachineryValidate();
			return;
		}
		int num = xPadding;
		int num2 = yPadding + ((canMove || hasTitle) ? (titleHeight + yPadding) : 0);
		int num3 = num + xPadding;
		int num4 = num2 + yPadding;
		childElement?.ResizePixels(widthPixels - num3, heightPixels - num4);
		childElement?.Validate();
		widthPixels = (num3 + childElement?.widthPixels).GetValueOrDefault();
		heightPixels = (num4 + childElement?.heightPixels).GetValueOrDefault();
		bool flag = closeCallback != null;
		topCloseButton.image.enabled = canMove || flag;
		background.image.raycastTarget = canMove || forceRaycast;
		topDragBorder.image.enabled = canMove || hasTitle || flag;
		bottomResizeButton.image.enabled = canResize;
		if (transparency == 1f)
		{
			outsideBorder.image.enabled = false;
			background.image.enabled = false;
			topDragBorder.image.enabled = false;
		}
		else
		{
			outsideBorder.image.enabled = true;
			background.image.enabled = true;
			outsideBorder.image.color = new Color(1f, 1f, 1f, 1f - transparency);
			background.image.color = new Color(1f, 1f, 1f, 1f - transparency);
			topDragBorder.image.color = new Color(1f, 1f, 1f, 1f - transparency);
		}
		topTitleBackground.image.enabled = hasTitle;
		titleText.text.enabled = hasTitle;
		if (topDragBorder.image.enabled)
		{
			outsideBorder.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels - 2);
			outsideBorder.rectTransform.anchoredPosition = new Vector2(0f, -2f);
			background.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels - 2);
			background.rectTransform.anchoredPosition = new Vector2(0f, -2f);
			background.image.raycastPadding = new Vector4(0f, 0f, 0f, -2f);
			titleText.text.text = titleString;
			if (RenderHandler.current.windowUICanvas.scaleFactor == 1f)
			{
				titleText.text.fontStyle = FontStyles.Normal;
			}
			else
			{
				titleText.text.fontStyle = FontStyles.Bold;
			}
			titleText.text.verticalAlignment = VerticalAlignmentOptions.Middle;
			titleText.text.enableWordWrapping = false;
			float num5 = LayoutUtility.GetPreferredWidth(titleText.text.rectTransform);
			if (num5 <= 1f)
			{
				num5 = titleText.rectTransform.sizeDelta.x;
			}
			titleText.rectTransform.sizeDelta = new Vector2(num5, 20f);
			titleText.rectTransform.anchoredPosition = new Vector2(xPadding + 2 + 2, 0f);
		}
		else
		{
			outsideBorder.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
			outsideBorder.rectTransform.anchoredPosition = new Vector2(0f, 0f);
			background.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
			background.rectTransform.anchoredPosition = new Vector2(0f, 0f);
			background.image.raycastPadding = new Vector4(0f, 0f, 0f, 0f);
		}
		topDragBorder.rectTransform.sizeDelta = new Vector2(widthPixels, topDragBorder.rectTransform.sizeDelta.y);
		topCloseButton.rectTransform.anchoredPosition = new Vector2((float)widthPixels - topCloseButton.rectTransform.sizeDelta.x - (float)xPadding, -yPadding);
		topTitleBackground.rectTransform.anchoredPosition = new Vector2(xPadding - 1, -yPadding + 1);
		topTitleBackground.rectTransform.sizeDelta = new Vector2(titleText.rectTransform.sizeDelta.x + 7f + 4f, titleHeight + 2);
		bottomResizeButton.rectTransform.anchoredPosition = new Vector2((float)widthPixels - bottomResizeButton.rectTransform.sizeDelta.x, (float)(-heightPixels) + bottomResizeButton.rectTransform.sizeDelta.y);
		inner.rectTransform.anchoredPosition = new Vector2(xPadding, -num2);
		if (hideBorder)
		{
			outsideBorder.image.enabled = false;
			background.image.enabled = false;
		}
	}

	public void ResizePixels(int width, int height)
	{
		if (width <= 0)
		{
			width = 0;
		}
		if (height <= 0)
		{
			height = 0;
		}
		widthPixels = width;
		heightPixels = height;
		Validate();
	}

	private Canvas GetBoundsCanvas()
	{
		Canvas canvas = ((handler != null) ? handler.GetComponentInParent<Canvas>(includeInactive: true) : null);
		if (!(canvas != null))
		{
			return RenderHandler.current.windowUICanvas;
		}
		return canvas.rootCanvas;
	}

	private void EnsureFloatingDock()
	{
		if (canMove)
		{
			if (!dockLayoutHooked)
			{
				dockLayoutHooked = true;
				LetterboxManager.LayoutChanged += OnLetterboxLayoutChanged;
			}
			Canvas dockCanvas = LetterboxManager.DockCanvas;
			RectTransform dockRoot = LetterboxManager.DockRoot;
			if (!(dockCanvas == null) && !(dockRoot == null) && !(handler == null) && !(rectTransform.parent == dockRoot))
			{
				dockCanvas.scaleFactor = RenderHandler.current.windowUICanvas.scaleFactor;
				preDockParent = rectTransform.parent;
				rectTransform.SetParent(dockRoot, worldPositionStays: false);
				dockedFloating.Add(this);
			}
		}
	}

	private Vector2 DockAnchorOffset()
	{
		Canvas dockCanvas = LetterboxManager.DockCanvas;
		RectTransform dockRoot = LetterboxManager.DockRoot;
		if (dockCanvas == null || dockRoot == null || handler == null || rectTransform.parent != dockRoot)
		{
			return Vector2.zero;
		}
		float num = ((dockCanvas.scaleFactor > 0f) ? dockCanvas.scaleFactor : 1f);
		float num2 = Screen.height - (LetterboxManager.GameAreaY + LetterboxManager.GameAreaHeight);
		return new Vector2((float)LetterboxManager.GameAreaX / num, 0f - num2 / num);
	}

	public static void ReclampDockedWindows()
	{
		dockedFloating.RemoveAll((PixelWindow w) => w == null || w.handler == null);
		Canvas dockCanvas = LetterboxManager.DockCanvas;
		if (dockCanvas == null)
		{
			return;
		}
		dockCanvas.scaleFactor = RenderHandler.current.windowUICanvas.scaleFactor;
		foreach (PixelWindow item in dockedFloating)
		{
			item.FixBounds();
		}
	}

	private static void OnLetterboxLayoutChanged()
	{
		dockedFloating.RemoveAll((PixelWindow w) => w == null || w.handler == null);
		if (LetterboxManager.DockCanvas != null)
		{
			return;
		}
		foreach (PixelWindow item in dockedFloating)
		{
			if (!(item.preDockParent == null))
			{
				item.rectTransform.SetParent(item.preDockParent, worldPositionStays: false);
				item.FixBounds();
			}
		}
		dockedFloating.Clear();
	}

	public void FixBounds()
	{
		Canvas boundsCanvas = GetBoundsCanvas();
		Vector2 max = boundsCanvas.pixelRect.max;
		max.y = boundsCanvas.pixelRect.min.y;
		Vector2 vector = rectTransform.parent.InverseTransformPoint(max);
		vector.y = 0f - vector.y;
		Vector2 sizeDelta = outsideBorder.rectTransform.sizeDelta;
		if ((float)heightPixels > vector.y)
		{
			heightPixels = (int)vector.y;
		}
		if ((float)widthPixels > vector.x)
		{
			widthPixels = (int)vector.x;
		}
		Vector2 anchoredPosition = rectTransform.anchoredPosition;
		anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, 0f, vector.x - sizeDelta.x);
		anchoredPosition.y = 0f - Mathf.Clamp(0f - anchoredPosition.y, 0f, vector.y - sizeDelta.y);
		rectTransform.anchoredPosition = anchoredPosition;
	}

	private void FixTooltipBounds(Vector2 worldTranslation)
	{
		Canvas boundsCanvas = GetBoundsCanvas();
		Vector2 max = boundsCanvas.pixelRect.max;
		max.y = boundsCanvas.pixelRect.min.y;
		Vector2 vector = rectTransform.parent.InverseTransformPoint(max);
		vector.y = 0f - vector.y;
		Vector2 sizeDelta = outsideBorder.rectTransform.sizeDelta;
		if ((float)heightPixels > vector.y)
		{
			heightPixels = (int)vector.y;
		}
		if ((float)widthPixels > vector.x)
		{
			widthPixels = (int)vector.x;
		}
		Vector2 anchoredPosition = rectTransform.anchoredPosition;
		if (anchoredPosition.x + sizeDelta.x > vector.x)
		{
			anchoredPosition.x -= sizeDelta.x + worldTranslation.x * 2f;
		}
		if (0f - anchoredPosition.y + sizeDelta.y > vector.y)
		{
			anchoredPosition.y += sizeDelta.y + worldTranslation.y * 2f;
			if (anchoredPosition.y > 0f)
			{
				anchoredPosition.y = 0f;
			}
		}
		rectTransform.anchoredPosition = anchoredPosition;
	}

	public static long NextFocusStamp()
	{
		return ++focusStampCounter;
	}

	private void RestoreOwnersContentTabs()
	{
		foreach (GameItem parentItem in parentItems)
		{
			parentItem.RestoreContentTabsIfMine(this);
		}
	}

	public PixelWindow Show()
	{
		StallWatchdog.Crumb("window " + CrumbName());
		if (isLocked)
		{
			return this;
		}
		handler.gameObject.SetActive(value: true);
		RestoreOwnersContentTabs();
		focusStamp = NextFocusStamp();
		if (canMove)
		{
			UIFamilyLayer.BringWindowsToFront(this);
		}
		EnsureFloatingDock();
		rectTransform.anchoredPosition = DockAnchorOffset();
		WindowsHandler.TryAddWindow(this);
		GraphUtils.FindAllChildrenType<GameInventory>(child).ForEach(delegate(GameInventory inv)
		{
			inv.ClearHighlight();
		});
		openCallback?.Invoke();
		return this;
	}

	public PixelWindow Show(Vector2 mousePosition, bool fixTooltipBounds = false)
	{
		Show(mousePosition, Vector2.zero, fixTooltipBounds);
		return this;
	}

	public PixelWindow Show(Vector2 mousePosition, Vector2 worldTranslation, bool fixTooltipBounds = false)
	{
		if (isLocked)
		{
			return this;
		}
		StallWatchdog.Crumb("window " + CrumbName());
		handler.gameObject.SetActive(value: true);
		RestoreOwnersContentTabs();
		focusStamp = NextFocusStamp();
		if (canMove)
		{
			UIFamilyLayer.BringWindowsToFront(this);
		}
		EnsureFloatingDock();
		rectTransform.anchoredPosition = (Vector2)rectTransform.parent.InverseTransformPoint(mousePosition) + worldTranslation;
		if (fixTooltipBounds)
		{
			FixTooltipBounds(worldTranslation);
		}
		else
		{
			FixBounds();
		}
		ToFront();
		rectTransform.position = new Vector3(Mathf.RoundToInt(rectTransform.position.x), Mathf.RoundToInt(rectTransform.position.y), rectTransform.position.z);
		WindowsHandler.TryAddWindow(this);
		openCallback?.Invoke();
		return this;
	}

	public PixelWindow Center()
	{
		Canvas boundsCanvas = GetBoundsCanvas();
		Vector2 max = boundsCanvas.pixelRect.max;
		max.y = boundsCanvas.pixelRect.min.y;
		Vector2 vector = rectTransform.parent.InverseTransformPoint(max);
		vector.y = 0f - vector.y;
		Vector2 sizeDelta = outsideBorder.rectTransform.sizeDelta;
		int num = (int)((vector.x - sizeDelta.x) / 2f);
		int num2 = (int)((vector.y - sizeDelta.y) / 2f);
		rectTransform.anchoredPosition = new Vector2(num, -num2);
		return this;
	}

	public PixelWindow Hide()
	{
		foreach (GameItem parentItem in parentItems)
		{
			parentItem.RememberContentWindowPositionIfMine(this);
		}
		handler.gameObject.SetActive(value: false);
		UIFamilyLayer.ReleaseWindow(this);
		WindowsHandler.TryRemoveWindow(this);
		return this;
	}

	public void Destroy()
	{
		if (!(handler == null))
		{
			Hide();
			UIFamilyLayer.ReleaseWindow(this);
			dockedFloating.Remove(this);
			GraphHandler.windowGraphNodeRoots.Remove(this);
			parentItems.Clear();
			UnityEngine.Object.Destroy(handler.gameObject);
		}
	}

	private string CrumbName()
	{
		if (parentItems.Count > 0 && parentItems[0] != null)
		{
			return parentItems[0].identifier;
		}
		if (!string.IsNullOrEmpty(titleString))
		{
			return titleString;
		}
		return "untitled";
	}

	public PixelWindow Lock()
	{
		Hide();
		isLocked = true;
		return this;
	}

	public PixelWindow Unlock()
	{
		isLocked = false;
		Validate();
		return this;
	}

	public bool IsVisible()
	{
		return handler.gameObject.activeSelf;
	}

	public (int, int) Size()
	{
		return (widthPixels, heightPixels);
	}

	public bool Attach(PixelElement element)
	{
		if (element == null)
		{
			return Detach();
		}
		if (childElement != null)
		{
			Detach(childElement);
		}
		childElement = element;
		if (element is GraphNodeStorage graphNodeStorage)
		{
			graphNodeStorage.AddParent(this);
		}
		if (element is MachineryContainer machineryContainer)
		{
			machineryContainer.owner = this;
			RelinkStorage();
		}
		element.SetRenderingParent(this);
		element.handler.rectTransform.anchoredPosition = Vector2.zero;
		int num = 2 * LeftPadding();
		int num2 = TopPadding() + BottomPadding();
		widthPixels = (num + childElement?.widthPixels).GetValueOrDefault();
		heightPixels = (num2 + childElement?.heightPixels).GetValueOrDefault();
		Validate();
		return true;
	}

	public bool Detach()
	{
		if (childElement != null)
		{
			return Detach(childElement);
		}
		return false;
	}

	public bool Detach(PixelElement element)
	{
		if (childElement != element)
		{
			return false;
		}
		if (element is GraphNodeStorage graphNodeStorage)
		{
			graphNodeStorage.RemoveParent(this);
		}
		childElement = null;
		if (element is MachineryContainer machineryContainer)
		{
			RelinkStorage();
			machineryContainer.owner = null;
		}
		element?.RemoveRenderingParent(this);
		Validate();
		return true;
	}

	public void ToFront(bool force = false)
	{
		focusStamp = NextFocusStamp();
		if (canMove)
		{
			UIFamilyLayer.BringWindowsToFront(this);
		}
		if (canMove || force)
		{
			handler.rectTransform.SetAsLastSibling();
		}
		if (WindowsHandler.ContainsWindow(this))
		{
			WindowsHandler.TryRemoveWindow(this);
			WindowsHandler.TryAddWindow(this);
		}
	}

	public void ToBack(bool force = false)
	{
		if (!canMove || force)
		{
			handler.rectTransform.SetAsFirstSibling();
		}
	}

	public int XClamp(int x)
	{
		throw new NotImplementedException();
	}

	public int YClamp(int y)
	{
		throw new NotImplementedException();
	}

	public int WidthPixelsClamp(int width)
	{
		throw new NotImplementedException();
	}

	public int HeightPixelsClamp(int height)
	{
		throw new NotImplementedException();
	}

	public bool AddParent(GraphNodeStorage node)
	{
		if (!(node is GameItem))
		{
			return false;
		}
		if (parentItems.Contains(node))
		{
			return false;
		}
		parentItems.Add((GameItem)node);
		GraphHandler.windowGraphNodeRoots.Remove(this);
		return true;
	}

	public bool RemoveParent(GraphNodeStorage node)
	{
		if (!(node is GameItem))
		{
			return false;
		}
		bool result = parentItems.Remove((GameItem)node);
		if (parentItems.Count == 0 && !GraphHandler.windowGraphNodeRoots.Contains(this))
		{
			GraphHandler.windowGraphNodeRoots.Add(this);
		}
		return result;
	}

	public bool MayHaveValidInventorySlot(GameItem item)
	{
		return child?.MayHaveValidInventorySlot(item) ?? false;
	}

	public SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		return child?.TryFindOneValidInventorySlot(item) ?? null;
	}

	public PixelWindow AddInventoryStateChangeAction(Action action)
	{
		if (action == null)
		{
			return this;
		}
		if (stateChangeActionFunc == null)
		{
			stateChangeActionFunc = action;
		}
		else
		{
			stateChangeActionFunc = (Action)Delegate.Combine(stateChangeActionFunc, action);
		}
		return this;
	}

	private bool IsBareMachineryChild()
	{
		if (childElement != null)
		{
			return !(childElement is MachineryPanelRow);
		}
		return false;
	}

	private int LeftPadding()
	{
		if (style == WindowStyle.Machinery)
		{
			if (!IsBareMachineryChild())
			{
				return 0;
			}
			return 8;
		}
		return xPadding;
	}

	private int TopPadding()
	{
		if (style == WindowStyle.Machinery)
		{
			return 21 + (IsBareMachineryChild() ? 8 : 0);
		}
		return yPadding + ((canMove || hasTitle) ? (titleHeight + yPadding) : 0);
	}

	private int BottomPadding()
	{
		if (style == WindowStyle.Machinery)
		{
			if (!IsBareMachineryChild())
			{
				return 0;
			}
			return 8;
		}
		return yPadding;
	}

	private List<GraphNodeStorage> ChildStorage()
	{
		List<GraphNodeStorage> list = new List<GraphNodeStorage>();
		MachineryContainerUtils.Collect(childElement, list);
		return list;
	}

	public void RelinkStorage()
	{
		List<GraphNodeStorage> current = ((childElement is MachineryContainer) ? ChildStorage() : new List<GraphNodeStorage>());
		MachineryContainerUtils.Relink(this, linkedContainerStorage, current);
		linkedContainerStorage = current;
	}

	private void InitMachineryChrome()
	{
		xPadding = 0;
		yPadding = 0;
		MachineryStyle.SetSliced(machineryHeader, MachineryStyle.BevelFrame);
		machineryHeader.image.raycastTarget = false;
		MachineryStyle.SetFlat(machineryRuleBacking, MachineryStyle.PANEL);
		machineryRuleBacking.image.raycastTarget = false;
		MachineryStyle.SetFlat(machineryRule, MachineryStyle.RULE);
		machineryRule.image.raycastTarget = false;
		MachineryStyle.SetSliced(machineryPlate, MachineryStyle.BevelFrame);
		machineryPlate.image.raycastTarget = false;
		MachineryStyle.SetSliced(machineryIntake, MachineryStyle.Intake);
		machineryIntake.image.raycastTarget = false;
		MachineryStyle.InitText(titleText, 10, MachineryStyle.TITLE_TEXT, HorizontalAlignmentOptions.Center, uppercase: true);
		titleText.text.overflowMode = TextOverflowModes.Ellipsis;
		titleText.text.text = titleString ?? "";
	}

	private void MachineryValidate()
	{
		bool flag = IsBareMachineryChild();
		int num = LeftPadding();
		int num2 = TopPadding();
		int num3 = BottomPadding();
		childElement?.ResizePixels(widthPixels - 2 * num, heightPixels - num2 - num3);
		childElement?.Validate();
		widthPixels = 2 * num + (childElement?.widthPixels ?? 0);
		heightPixels = num2 + (childElement?.heightPixels ?? 0) + num3;
		outsideBorder.image.enabled = false;
		topDragBorder.image.enabled = false;
		topTitleBackground.image.enabled = false;
		bottomResizeButton.image.enabled = false;
		outsideBorder.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		outsideBorder.rectTransform.anchoredPosition = Vector2.zero;
		background.image.sprite = null;
		background.image.color = new Color(1f, 1f, 1f, 0f);
		background.image.enabled = true;
		background.image.raycastTarget = canMove || forceRaycast;
		background.image.raycastPadding = Vector4.zero;
		background.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		background.rectTransform.anchoredPosition = Vector2.zero;
		MachineryStyle.Place(machineryHeader, 0, 0, widthPixels, 20);
		MachineryStyle.Place(machineryRuleBacking, 0, 20, widthPixels, 1);
		MachineryStyle.Place(machineryRule, 4, 20, Mathf.Max(0, widthPixels - 8), 1);
		machineryPlate.image.enabled = flag;
		machineryIntake.image.enabled = flag;
		if (flag)
		{
			int num4 = 21;
			MachineryStyle.Place(machineryPlate, 0, num4, widthPixels, heightPixels - num4);
			MachineryStyle.Place(machineryIntake, 4, num4 + 4, widthPixels - 8, heightPixels - num4 - 8);
		}
		bool flag2 = canMove || closeCallback != null;
		topCloseButton.image.enabled = flag2;
		int num5 = widthPixels - 2;
		if (flag2)
		{
			num5 -= 16;
			topCloseButton.rectTransform.anchoredPosition = new Vector2(num5, -2f);
			num5 -= 2;
		}
		foreach (MachineryButton headerButton in headerButtons)
		{
			if (!headerButton.isHidden)
			{
				num5 -= headerButton.widthPixels;
				headerButton.handler.rectTransform.anchoredPosition = new Vector2(num5, -(2 + (16 - headerButton.heightPixels) / 2));
				num5 -= 2;
			}
		}
		int num6 = widthPixels - num5;
		titleText.text.enabled = hasTitle;
		titleText.text.text = titleString ?? "";
		int num7 = Mathf.Max(0, widthPixels - 2 * num6);
		if ((hasTitle ? MachineryStyle.TextWidth(titleText) : 0) <= num7)
		{
			titleText.text.horizontalAlignment = HorizontalAlignmentOptions.Center;
			titleText.rectTransform.anchoredPosition = new Vector2(num6, 0f);
			titleText.rectTransform.sizeDelta = new Vector2(num7, 20f);
		}
		else
		{
			titleText.text.horizontalAlignment = HorizontalAlignmentOptions.Left;
			titleText.rectTransform.anchoredPosition = new Vector2(4f, 0f);
			titleText.rectTransform.sizeDelta = new Vector2(Mathf.Max(0, widthPixels - 4 - num6), 20f);
		}
		inner.rectTransform.anchoredPosition = new Vector2(num, -num2);
	}

	public PixelWindow AddHeaderButton(MachineryButton button)
	{
		if (button == null || headerButtons.Contains(button))
		{
			return this;
		}
		headerButtons.Add(button);
		button.handler.SetParent(handler);
		Validate();
		return this;
	}

	public PixelWindow RemoveHeaderButton(MachineryButton button)
	{
		if (button == null || !headerButtons.Remove(button))
		{
			return this;
		}
		button.handler.ResetParent();
		Validate();
		return this;
	}

	public void HideBorder()
	{
		background.image.enabled = false;
		outsideBorder.image.enabled = false;
		hideBorder = true;
	}

	public void ShowBorder()
	{
		background.image.enabled = true;
		outsideBorder.image.enabled = true;
		hideBorder = false;
	}
}

using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class TreeNodeRender : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IInitializePotentialDragHandler, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IPointerDoubleClickHandler, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IUpdateSelectedHandler, IScrollHandler
{
	public MultiEventHandler eventHandler;

	public PixelWindow window { get; private set; }

	public PixelElement element { get; private set; }

	public TreeNodeRender parent { get; private set; }

	public RectTransform rectTransform { get; private set; }

	public Image image { get; private set; }

	public RectMask2D mask { get; private set; }

	public TextMeshProUGUI text { get; private set; }

	public static TreeNodeRender InstantiateAsRoot(Transform transform)
	{
		return InstantiateAsRoot(transform, isText: false);
	}

	public static TreeNodeRender InstantiateAsRoot(Transform transform, bool isText)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(isText ? RenderHandler.current.renderNodeTextPrefab : RenderHandler.current.renderNodePrefab, transform);
		gameObject.name = (isText ? "Node (Text)" : "Node");
		TreeNodeRender component = gameObject.GetComponent<TreeNodeRender>();
		if (component == null)
		{
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		component.rectTransform = gameObject.GetComponent<RectTransform>();
		component.image = gameObject.GetComponent<Image>();
		component.mask = gameObject.GetComponent<RectMask2D>();
		component.text = gameObject.GetComponent<TextMeshProUGUI>();
		return component;
	}

	public static TreeNodeRender Instantiate()
	{
		return Instantiate(RenderHandler.current.tempRoot, isText: false);
	}

	public static TreeNodeRender InstantiateText()
	{
		TreeNodeRender treeNodeRender = Instantiate(RenderHandler.current.tempRoot, isText: true);
		ApplyLocaleFont(treeNodeRender);
		return treeNodeRender;
	}

	private static void ApplyLocaleFont(TreeNodeRender handler)
	{
		if (handler == null || handler.text == null || FontLoader.Instance == null)
		{
			return;
		}
		Locale selectedLocale = LocalizationSettings.SelectedLocale;
		if (!(selectedLocale == null))
		{
			TMP_FontAsset fontByRole = FontLoader.Instance.GetFontByRole(FontLoader.FontRole.Noto_Sans, selectedLocale);
			if (fontByRole != null)
			{
				handler.text.font = fontByRole;
			}
		}
	}

	private static TreeNodeRender Instantiate(TreeNodeRender parentNode, bool isText)
	{
		return InstantiateAsRoot(RenderHandler.current.tempRoot.transform, isText).SetParent(parentNode);
	}

	public static TreeNodeRender Instantiate(PixelElement element, string editorObjectName = "Element")
	{
		TreeNodeRender treeNodeRender = Instantiate(RenderHandler.current.tempRoot, isText: false);
		treeNodeRender.gameObject.name = editorObjectName + " Node";
		treeNodeRender.element = element;
		return treeNodeRender;
	}

	public static TreeNodeRender Instantiate(PixelWindow window, string editorObjectName = "Window")
	{
		TreeNodeRender treeNodeRender = Instantiate(RenderHandler.current.windowRoot, isText: false);
		treeNodeRender.gameObject.name = editorObjectName + " Node";
		treeNodeRender.window = window;
		return treeNodeRender;
	}

	public TreeNodeRender InitEventHandler()
	{
		if ((object)image != null)
		{
			image.raycastTarget = true;
		}
		if ((object)text != null)
		{
			text.raycastTarget = true;
		}
		eventHandler = new MultiEventHandler();
		return this;
	}

	public TreeNodeRender SetParentOverlayRoot()
	{
		return SetParent(RenderHandler.current.overlayRoot);
	}

	public TreeNodeRender SetParentTempRoot()
	{
		return SetParent(RenderHandler.current.tempRoot);
	}

	public TreeNodeRender SetParentWindowRoot()
	{
		return SetParent(RenderHandler.current.windowRoot);
	}

	public TreeNodeRender SetParent(TreeNodeRender parent)
	{
		return SetParent(parent, worldPositionStays: false);
	}

	public TreeNodeRender SetParent(TreeNodeRender parent, bool worldPositionStays)
	{
		if ((object)parent == null)
		{
			return this;
		}
		rectTransform.SetParent(parent.rectTransform, worldPositionStays);
		if (worldPositionStays)
		{
			Vector3 localPosition = rectTransform.localPosition;
			if (localPosition.z != 0f)
			{
				localPosition.z = 0f;
				rectTransform.localPosition = localPosition;
			}
		}
		this.parent = parent;
		return this;
	}

	public TreeNodeRender ResetParent()
	{
		SetParent(RenderHandler.current.tempRoot, worldPositionStays: true);
		return this;
	}

	public TreeNodeRender RemoveParent()
	{
		rectTransform.SetParent(RenderHandler.current.tempRoot.rectTransform);
		parent = null;
		return this;
	}

	public PixelWindow FindParentWindow()
	{
		if (window != null)
		{
			return window;
		}
		if ((object)parent == null)
		{
			return null;
		}
		return parent.FindParentWindow();
	}

	public PixelElement FindParentElement()
	{
		if (element != null)
		{
			return element;
		}
		if ((object)parent == null)
		{
			return null;
		}
		return parent.FindParentElement();
	}

	public T FindParentElement<T>()
	{
		PixelWindow pixelWindow = window;
		if (pixelWindow is T)
		{
			return (T)(object)((pixelWindow is T) ? pixelWindow : null);
		}
		PixelElement pixelElement = element;
		if (pixelElement is T)
		{
			return (T)pixelElement;
		}
		if ((object)parent == null)
		{
			return default(T);
		}
		return parent.FindParentElement<T>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		eventHandler?.mouseClickFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		eventHandler?.mouseMoveFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		eventHandler?.mouseUpFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		eventHandler?.mouseDownFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
	}

	public void OnInitializePotentialDrag(PointerEventData eventData)
	{
		if (!(EventSystem.current.currentSelectedGameObject == base.gameObject))
		{
			eventHandler?.mousePotentialDragFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!(EventSystem.current.currentSelectedGameObject == base.gameObject))
		{
			eventHandler?.mouseDragFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
			CustomEventHandler.ClearDoubleClickTarget();
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!(EventSystem.current.currentSelectedGameObject == base.gameObject))
		{
			eventHandler?.mouseBeginDragFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
			CustomEventHandler.ClearDoubleClickTarget();
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (!(EventSystem.current.currentSelectedGameObject == base.gameObject))
		{
			eventHandler?.mouseEndDragFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
			CustomEventHandler.ClearDoubleClickTarget();
		}
	}

	public void OnDrop(PointerEventData eventData)
	{
		if (!(EventSystem.current.currentSelectedGameObject == base.gameObject))
		{
			eventHandler?.mouseDropFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
			CustomEventHandler.ClearDoubleClickTarget();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		eventHandler?.mouseEnterFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
		CustomEventHandler.ClearDoubleClickTarget();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		eventHandler?.mouseExitFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
		CustomEventHandler.ClearDoubleClickTarget();
	}

	public void OnScroll(PointerEventData eventData)
	{
		eventHandler?.mouseScrollFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
		CustomEventHandler.ClearDoubleClickTarget();
	}

	public void OnPointerDoubleClick(PointerEventData eventData)
	{
		eventHandler?.mouseDoubleClickFunc?.Any((Func<PointerEventData, bool> f) => f(eventData));
		CustomEventHandler.ClearDoubleClickTarget();
	}

	public void OnSelect(BaseEventData eventData)
	{
		eventHandler?.selectFunc?.Any((Func<BaseEventData, bool> f) => f(eventData));
		CustomEventHandler.ClearDoubleClickTarget();
	}

	public void OnDeselect(BaseEventData eventData)
	{
		eventHandler?.deselectFunc?.Any((Func<BaseEventData, bool> f) => f(eventData));
		CustomEventHandler.ClearDoubleClickTarget();
	}

	public void OnUpdateSelected(BaseEventData eventData)
	{
		eventHandler?.updateSelectedFunc?.Any((Func<BaseEventData, bool> f) => f(eventData));
	}
}

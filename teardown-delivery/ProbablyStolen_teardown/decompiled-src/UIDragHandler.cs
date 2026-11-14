using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragHandler : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	private RectTransform rectTransform;

	private Canvas canvas;

	private CanvasGroup canvasGroup;

	private Image image;

	[SerializeField]
	private bool pixelPerfect = true;

	public bool ignoreClamp;

	public bool ignoreTransparent;

	public static bool isDraggingAnyUI = false;

	public string id;

	public static string draggingID = "";

	public List<GameObject> moveFrontObjects;

	private static UIDragHandler currentDragger;

	private Transform preDockParent;

	private int preDockSiblingIndex;

	private Vector3 preDockLocalScale;

	private bool isDocked;

	private static readonly List<UIDragHandler> dockedPanels = new List<UIDragHandler>();

	private static bool dockLayoutHooked;

	public float DockScaleRatio { get; private set; } = 1f;


	private static void ReleaseDragLatch()
	{
		currentDragger = null;
		isDraggingAnyUI = false;
		draggingID = "";
	}

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvas = GetComponentInParent<Canvas>();
		canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
		image = GetComponent<Image>();
		if (ignoreTransparent && image != null)
		{
			image.alphaHitTestMinimumThreshold = 0.05f;
		}
	}

	public void MoveFront()
	{
		if (moveFrontObjects == null || moveFrontObjects.Count == 0)
		{
			return;
		}
		foreach (GameObject moveFrontObject in moveFrontObjects)
		{
			if (moveFrontObject != null && moveFrontObject.activeInHierarchy)
			{
				moveFrontObject.transform.SetAsLastSibling();
			}
		}
	}

	public void SetHomeScale(float homeScale)
	{
		if (isDocked)
		{
			preDockLocalScale = new Vector3(homeScale, homeScale, preDockLocalScale.z);
		}
	}

	private void EnsurePanelDock()
	{
		if (!dockLayoutHooked)
		{
			dockLayoutHooked = true;
			LetterboxManager.LayoutChanged += OnLetterboxLayoutChanged;
		}
		RectTransform dockRoot = LetterboxManager.DockRoot;
		if (!(dockRoot == null) && !(rectTransform.parent == dockRoot) && !(rectTransform.parent == null) && !(rectTransform.parent.GetComponent<Canvas>() == null))
		{
			preDockParent = rectTransform.parent;
			preDockSiblingIndex = rectTransform.GetSiblingIndex();
			preDockLocalScale = rectTransform.localScale;
			rectTransform.SetParent(dockRoot, worldPositionStays: true);
			canvas = GetComponentInParent<Canvas>();
			FlattenLocalZ();
			isDocked = true;
			DockScaleRatio = ((preDockLocalScale.x != 0f) ? (rectTransform.localScale.x / preDockLocalScale.x) : 1f);
			dockedPanels.Add(this);
		}
	}

	private void RestoreFromDock()
	{
		if (this == null || !isDocked)
		{
			return;
		}
		isDocked = false;
		DockScaleRatio = 1f;
		dockedPanels.Remove(this);
		if (!(rectTransform == null) && !(preDockParent == null) && base.gameObject.scene.isLoaded)
		{
			rectTransform.SetParent(preDockParent, worldPositionStays: true);
			rectTransform.SetSiblingIndex(preDockSiblingIndex);
			canvas = GetComponentInParent<Canvas>(includeInactive: true);
			FlattenLocalZ();
			rectTransform.localScale = preDockLocalScale;
			if (!ignoreClamp && canvas != null)
			{
				ClampToWindow();
			}
		}
	}

	private void FlattenLocalZ()
	{
		Vector3 localPosition = rectTransform.localPosition;
		localPosition.z = 0f;
		rectTransform.localPosition = localPosition;
	}

	private static void OnLetterboxLayoutChanged()
	{
		dockedPanels.RemoveAll((UIDragHandler p) => p == null);
		if (!(LetterboxManager.DockCanvas != null))
		{
			for (int num = dockedPanels.Count - 1; num >= 0; num--)
			{
				dockedPanels[num].RestoreFromDock();
			}
			dockedPanels.Clear();
		}
	}

	public static void ReclampDockedPanels()
	{
		dockedPanels.RemoveAll((UIDragHandler p) => p == null);
		Canvas dockCanvas = LetterboxManager.DockCanvas;
		if (dockCanvas == null)
		{
			return;
		}
		foreach (UIDragHandler dockedPanel in dockedPanels)
		{
			Canvas canvas = ((dockedPanel.preDockParent != null) ? dockedPanel.preDockParent.GetComponentInParent<Canvas>() : null);
			if (canvas != null && dockCanvas.scaleFactor > 0f)
			{
				float num = canvas.rootCanvas.scaleFactor / dockCanvas.scaleFactor;
				dockedPanel.rectTransform.localScale = dockedPanel.preDockLocalScale * num;
				dockedPanel.DockScaleRatio = num;
			}
			if (!dockedPanel.ignoreClamp)
			{
				dockedPanel.ClampToWindow();
			}
		}
	}

	public static List<GameObject> GetActiveDockedPanels(Transform homeParent)
	{
		dockedPanels.RemoveAll((UIDragHandler p) => p == null);
		List<UIDragHandler> list = dockedPanels.FindAll((UIDragHandler p) => p.preDockParent == homeParent && p.gameObject.activeSelf);
		list.Sort((UIDragHandler a, UIDragHandler b) => a.rectTransform.GetSiblingIndex().CompareTo(b.rectTransform.GetSiblingIndex()));
		return list.ConvertAll((UIDragHandler p) => p.gameObject);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && CanDrag())
		{
			EnsurePanelDock();
			rectTransform.SetAsLastSibling();
			MoveFront();
			isDraggingAnyUI = true;
			currentDragger = this;
			if (!string.IsNullOrEmpty(id))
			{
				draggingID = id;
			}
			else
			{
				draggingID = "";
			}
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && CanDrag())
		{
			rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
			if (pixelPerfect)
			{
				SnapToPixelGrid();
			}
			if (!ignoreClamp)
			{
				ClampToWindow();
			}
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (pixelPerfect)
			{
				SnapToPixelGrid();
			}
			ReleaseDragLatch();
		}
	}

	private void OnEnable()
	{
		LetterboxManager.RunNextUpdate(delegate
		{
			if (!(this == null) && base.isActiveAndEnabled)
			{
				EnsurePanelDock();
				if (!ignoreClamp && canvas != null)
				{
					ClampToWindow();
				}
				LetterboxManager.RunNextUpdate(ReclampDockedPanels);
			}
		});
	}

	private void OnDisable()
	{
		if (currentDragger == this)
		{
			ReleaseDragLatch();
		}
		if (isDocked)
		{
			LetterboxManager.RunNextUpdate(RestoreFromDock);
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus && currentDragger == this)
		{
			ReleaseDragLatch();
		}
	}

	private void Update()
	{
		if (!(currentDragger != this) && !Input.GetMouseButton(0))
		{
			ReleaseDragLatch();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		rectTransform.SetAsLastSibling();
		MoveFront();
		PaperPanelFocusStamp component = GetComponent<PaperPanelFocusStamp>();
		if (component != null)
		{
			component.Bump();
		}
	}

	private void SnapToPixelGrid()
	{
		float scaleFactor = canvas.scaleFactor;
		Vector2 anchoredPosition = rectTransform.anchoredPosition;
		anchoredPosition.x = Mathf.Round(anchoredPosition.x * scaleFactor) / scaleFactor;
		anchoredPosition.y = Mathf.Round(anchoredPosition.y * scaleFactor) / scaleFactor;
		rectTransform.anchoredPosition = anchoredPosition;
	}

	private void ClampToWindow()
	{
		RectTransform obj = rectTransform.parent as RectTransform;
		Vector3[] array = new Vector3[4];
		Vector3[] array2 = new Vector3[4];
		obj.GetWorldCorners(array);
		rectTransform.GetWorldCorners(array2);
		Vector3 zero = Vector3.zero;
		float num = array2[2].x - array2[0].x;
		float num2 = array2[1].y - array2[0].y;
		float num3 = num * 0.75f;
		float num4 = num2 * 0.75f;
		if (array2[0].x < array[0].x - num3)
		{
			zero.x = array[0].x - num3 - array2[0].x;
		}
		if (array2[2].x > array[2].x + num3)
		{
			zero.x = array[2].x + num3 - array2[2].x;
		}
		if (array2[0].y < array[0].y - num4)
		{
			zero.y = array[0].y - num4 - array2[0].y;
		}
		if (array2[1].y > array[1].y + num4)
		{
			zero.y = array[1].y + num4 - array2[1].y;
		}
		rectTransform.position += zero;
		if (pixelPerfect)
		{
			SnapToPixelGrid();
		}
	}

	public bool CanDrag()
	{
		if (ScratchInput.isScratching)
		{
			return false;
		}
		return true;
	}
}

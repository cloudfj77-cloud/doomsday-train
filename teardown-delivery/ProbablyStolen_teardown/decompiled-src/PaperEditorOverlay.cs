using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaperEditorOverlay
{
	public enum Handle
	{
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		BottomLeft,
		Left
	}

	private class Box
	{
		public PaperUI.Binding binding;

		public GameObject go;

		public RectTransform[] edges = new RectTransform[4];

		public Image[] edgeImages = new Image[4];

		public TextMeshProUGUI label;

		public RectTransform labelRect;
	}

	private static readonly Color BOX_PAGE = new Color(0.2f, 0.75f, 1f, 0.75f);

	private static readonly Color BOX_CHROME = new Color(1f, 0.6f, 0.15f, 0.8f);

	private static readonly Color BOX_TEMPLATE = new Color(0.75f, 0.45f, 1f, 0.8f);

	private static readonly Color BOX_LOCKED = new Color(0.6f, 0.6f, 0.6f, 0.7f);

	private static readonly Color BOX_FLOW = new Color(0.3f, 1f, 0.6f, 0.6f);

	private static readonly Color BOX_OVERFLOW = new Color(1f, 0.2f, 0.2f, 0.95f);

	private static readonly Color SELECTION = new Color(1f, 0.9f, 0.2f, 1f);

	private static readonly Color HANDLE_FILL = new Color(1f, 0.95f, 0.6f, 1f);

	private static readonly Color MARQUEE = new Color(0.4f, 0.7f, 1f, 0.2f);

	private static readonly Color GUIDE = new Color(1f, 0.3f, 0.9f, 0.9f);

	private static readonly Color RULER = new Color(0f, 0f, 0f, 0.55f);

	private static readonly Color LABEL = new Color(1f, 1f, 1f, 0.95f);

	private static readonly Color LABEL_PLATE = new Color(0f, 0f, 0f, 0.6f);

	private readonly PaperEditor editor;

	private PaperUI ui;

	private GameObject root;

	private RectTransform rootRect;

	private RectTransform boxLayer;

	private RectTransform topLayer;

	private readonly Dictionary<PaperElementData, Box> boxes = new Dictionary<PaperElementData, Box>();

	private readonly RectTransform[] selectionEdges = new RectTransform[4];

	private readonly RectTransform[] handles = new RectTransform[8];

	private RectTransform marquee;

	private RectTransform guideX;

	private RectTransform guideY;

	private TextMeshProUGUI readout;

	private RectTransform readoutRect;

	private readonly List<GameObject> rulerObjects = new List<GameObject>();

	private TMP_FontAsset font;

	public bool Visible
	{
		set
		{
			if (root != null)
			{
				root.SetActive(value);
			}
		}
	}

	private float Px => 1f / Mathf.Max(0.01f, ((ui != null) ? ui.DisplayScale : 1f) * editor.editorZoom);

	public PaperEditorOverlay(PaperEditor editor)
	{
		this.editor = editor;
	}

	public void Attach(PaperUI target)
	{
		Destroy();
		ui = target;
		if (ui != null && !(ui.Content == null))
		{
			root = new GameObject("editor_overlay", typeof(RectTransform));
			rootRect = root.transform as RectTransform;
			Stretch(rootRect, ui.Content);
			CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
			canvasGroup.blocksRaycasts = false;
			canvasGroup.interactable = false;
			boxLayer = NewLayer("boxes");
			topLayer = NewLayer("top");
			font = PaperTextStyle.Get("label").Font();
			for (int i = 0; i < 4; i++)
			{
				selectionEdges[i] = NewQuad(topLayer, "sel_edge", SELECTION);
			}
			for (int j = 0; j < 8; j++)
			{
				handles[j] = NewQuad(topLayer, "handle", HANDLE_FILL);
			}
			marquee = NewQuad(topLayer, "marquee", MARQUEE);
			guideX = NewQuad(topLayer, "guide_x", GUIDE);
			guideY = NewQuad(topLayer, "guide_y", GUIDE);
			marquee.gameObject.SetActive(value: false);
			guideX.gameObject.SetActive(value: false);
			guideY.gameObject.SetActive(value: false);
			readout = NewLabel(topLayer, "readout", out readoutRect);
			Refresh();
		}
	}

	public void Destroy()
	{
		if (root != null)
		{
			Object.Destroy(root);
		}
		root = null;
		boxes.Clear();
		rulerObjects.Clear();
	}

	private RectTransform NewLayer(string name)
	{
		RectTransform obj = new GameObject(name, typeof(RectTransform)).transform as RectTransform;
		Stretch(obj, rootRect);
		return obj;
	}

	private static void Stretch(RectTransform rect, RectTransform parent)
	{
		rect.SetParent(parent, worldPositionStays: false);
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
		rect.localScale = Vector3.one;
	}

	private static RectTransform NewQuad(RectTransform parent, string name, Color color)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		Image image = gameObject.AddComponent<Image>();
		image.color = color;
		image.raycastTarget = false;
		RectTransform obj = gameObject.transform as RectTransform;
		PaperUI.SetRect(obj, 0f, 0f, 1f, 1f);
		return obj;
	}

	private TextMeshProUGUI NewLabel(RectTransform parent, string name, out RectTransform rect)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		Image image = gameObject.AddComponent<Image>();
		image.color = LABEL_PLATE;
		image.raycastTarget = false;
		rect = gameObject.transform as RectTransform;
		PaperUI.SetRect(rect, 0f, 0f, 40f, 8f);
		GameObject gameObject2 = new GameObject("text", typeof(RectTransform));
		gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		RectTransform obj = gameObject2.transform as RectTransform;
		Stretch(obj, rect);
		obj.offsetMin = new Vector2(1f, 0f);
		obj.offsetMax = new Vector2(-1f, 0f);
		TextMeshProUGUI textMeshProUGUI = gameObject2.AddComponent<TextMeshProUGUI>();
		if (font != null)
		{
			textMeshProUGUI.font = font;
		}
		textMeshProUGUI.fontSize = 5f;
		textMeshProUGUI.color = LABEL;
		textMeshProUGUI.enableWordWrapping = false;
		textMeshProUGUI.overflowMode = TextOverflowModes.Overflow;
		textMeshProUGUI.alignment = TextAlignmentOptions.MidlineLeft;
		textMeshProUGUI.raycastTarget = false;
		return textMeshProUGUI;
	}

	public void Refresh()
	{
		if (root == null || ui == null)
		{
			return;
		}
		HashSet<PaperElementData> hashSet = new HashSet<PaperElementData>();
		foreach (PaperUI.Binding item in ui.VisibleBindings())
		{
			if (!(item.rect == null))
			{
				hashSet.Add(item.data);
				if (!boxes.TryGetValue(item.data, out var value))
				{
					value = MakeBox(item);
					boxes[item.data] = value;
				}
				value.binding = item;
			}
		}
		List<PaperElementData> list = new List<PaperElementData>();
		foreach (KeyValuePair<PaperElementData, Box> box in boxes)
		{
			if (!hashSet.Contains(box.Key))
			{
				list.Add(box.Key);
			}
		}
		foreach (PaperElementData item2 in list)
		{
			if (boxes[item2].go != null)
			{
				Object.Destroy(boxes[item2].go);
			}
			boxes.Remove(item2);
		}
		RefreshRulers();
		UpdateLive();
	}

	private Box MakeBox(PaperUI.Binding binding)
	{
		Box box = new Box
		{
			binding = binding
		};
		box.go = new GameObject("box", typeof(RectTransform));
		box.go.transform.SetParent(boxLayer, worldPositionStays: false);
		Stretch(box.go.transform as RectTransform, boxLayer);
		for (int i = 0; i < 4; i++)
		{
			box.edges[i] = NewQuad(box.go.transform as RectTransform, "edge", BOX_PAGE);
			box.edgeImages[i] = box.edges[i].GetComponent<Image>();
		}
		box.label = NewLabel(box.go.transform as RectTransform, "id", out box.labelRect);
		return box;
	}

	public void UpdateLive()
	{
		if (root == null || ui == null)
		{
			return;
		}
		float px = Px;
		foreach (KeyValuePair<PaperElementData, Box> box in boxes)
		{
			PaperElementData key = box.Key;
			Box value = box.Value;
			if (!(value.binding.rect == null))
			{
				Rect r = editor.BoundsOf(key);
				Color color = ColorFor(key);
				float thickness = px;
				Outline(value.edges, r, thickness);
				Image[] edgeImages = value.edgeImages;
				for (int i = 0; i < edgeImages.Length; i++)
				{
					edgeImages[i].color = color;
				}
				bool flag = editor.showIds && !string.IsNullOrEmpty(key.DisplayName);
				value.label.transform.parent.gameObject.SetActive(flag);
				if (flag)
				{
					value.label.text = key.DisplayName;
					value.label.fontSize = 5f * px * 2f;
					float a = Mathf.Min(r.width, value.label.GetPreferredValues(value.label.text).x + 3f * px);
					a = Mathf.Max(a, 10f * px);
					PaperUI.SetRect(value.labelRect, r.x, r.y - 7f * px * 2f, a, 7f * px * 2f);
					value.label.color = ((color.a > 0.5f) ? LABEL : new Color(1f, 1f, 1f, 0.6f));
				}
			}
		}
		bool flag2 = editor.Selection.Count > 0;
		Rect r2 = (flag2 ? editor.SelectionBounds() : Rect.zero);
		RectTransform[] array = selectionEdges;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(flag2);
		}
		bool flag3 = flag2 && editor.CanResizeSelection();
		array = handles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(flag3);
		}
		if (flag2)
		{
			Outline(selectionEdges, r2, px * 1.5f);
			if (flag3)
			{
				float num = 5f * px * 1.6f;
				for (int j = 0; j < 8; j++)
				{
					Vector2 vector = HandleCenter(r2, (Handle)j);
					PaperUI.SetRect(handles[j], vector.x - num * 0.5f, vector.y - num * 0.5f, num, num);
				}
			}
		}
		PositionReadout();
	}

	private Color ColorFor(PaperElementData element)
	{
		Color result = (editor.IsChrome(element) ? BOX_CHROME : (editor.IsTemplateElementOnPage(element) ? BOX_TEMPLATE : ((!element.IsFlow) ? BOX_PAGE : BOX_FLOW)));
		if (element.locked)
		{
			result = BOX_LOCKED;
		}
		if (element.IsText && ui.IsOverflowing(element))
		{
			result = BOX_OVERFLOW;
		}
		if (!element.visible)
		{
			result.a *= 0.4f;
		}
		return result;
	}

	public static Vector2 HandleCenter(Rect r, Handle handle)
	{
		return handle switch
		{
			Handle.TopLeft => new Vector2(r.xMin, r.yMin), 
			Handle.Top => new Vector2(r.center.x, r.yMin), 
			Handle.TopRight => new Vector2(r.xMax, r.yMin), 
			Handle.Right => new Vector2(r.xMax, r.center.y), 
			Handle.BottomRight => new Vector2(r.xMax, r.yMax), 
			Handle.Bottom => new Vector2(r.center.x, r.yMax), 
			Handle.BottomLeft => new Vector2(r.xMin, r.yMax), 
			_ => new Vector2(r.xMin, r.center.y), 
		};
	}

	public bool HitHandle(Vector2 point, out Handle handle)
	{
		handle = Handle.TopLeft;
		if (editor.Selection.Count == 0 || !editor.CanResizeSelection())
		{
			return false;
		}
		Rect r = editor.SelectionBounds();
		float num = 5f * Px * 1.6f;
		for (int i = 0; i < 8; i++)
		{
			Vector2 vector = HandleCenter(r, (Handle)i);
			if (Mathf.Abs(point.x - vector.x) <= num && Mathf.Abs(point.y - vector.y) <= num)
			{
				handle = (Handle)i;
				return true;
			}
		}
		return false;
	}

	private static void Outline(RectTransform[] edges, Rect r, float thickness)
	{
		PaperUI.SetRect(edges[0], r.xMin - thickness, r.yMin - thickness, r.width + 2f * thickness, thickness);
		PaperUI.SetRect(edges[1], r.xMin - thickness, r.yMax, r.width + 2f * thickness, thickness);
		PaperUI.SetRect(edges[2], r.xMin - thickness, r.yMin, thickness, r.height);
		PaperUI.SetRect(edges[3], r.xMax, r.yMin, thickness, r.height);
	}

	public void ShowMarquee(Rect r)
	{
		if (!(marquee == null))
		{
			marquee.gameObject.SetActive(value: true);
			PaperUI.SetRect(marquee, r.xMin, r.yMin, Mathf.Max(0.5f, r.width), Mathf.Max(0.5f, r.height));
		}
	}

	public void HideMarquee()
	{
		if (marquee != null)
		{
			marquee.gameObject.SetActive(value: false);
		}
	}

	public void ShowGuides(float? x, float? y)
	{
		if (!(guideX == null) && ui != null)
		{
			guideX.gameObject.SetActive(x.HasValue);
			guideY.gameObject.SetActive(y.HasValue);
			float px = Px;
			if (x.HasValue)
			{
				PaperUI.SetRect(guideX, x.Value - px * 0.5f, 0f, px, ui.DesignSize.y);
			}
			if (y.HasValue)
			{
				PaperUI.SetRect(guideY, 0f, y.Value - px * 0.5f, ui.DesignSize.x, px);
			}
		}
	}

	public void HideGuides()
	{
		ShowGuides(null, null);
	}

	public void SetReadout(string text)
	{
		if (!(readout == null))
		{
			readout.text = text;
			PositionReadout();
		}
	}

	private void PositionReadout()
	{
		if (!(readout == null) && ui != null)
		{
			float px = Px;
			readout.fontSize = 5f * px * 2f;
			bool flag = !string.IsNullOrEmpty(readout.text);
			readoutRect.gameObject.SetActive(flag);
			if (flag)
			{
				float width = readout.GetPreferredValues(readout.text).x + 4f * px;
				float height = 8f * px * 2f;
				PaperUI.SetRect(readoutRect, 0f, ui.DesignSize.y + 2f * px, width, height);
			}
		}
	}

	private void RefreshRulers()
	{
		foreach (GameObject rulerObject in rulerObjects)
		{
			if (rulerObject != null)
			{
				Object.Destroy(rulerObject);
			}
		}
		rulerObjects.Clear();
		if (editor.showRulers && ui != null)
		{
			float px = Px;
			float num = PickRulerStep(ui.DesignSize.x);
			RectTransform rectTransform = NewQuad(boxLayer, "ruler_top", RULER);
			PaperUI.SetRect(rectTransform, 0f, -4f * px, ui.DesignSize.x, 4f * px);
			rulerObjects.Add(rectTransform.gameObject);
			RectTransform rectTransform2 = NewQuad(boxLayer, "ruler_left", RULER);
			PaperUI.SetRect(rectTransform2, -4f * px, 0f, 4f * px, ui.DesignSize.y);
			rulerObjects.Add(rectTransform2.gameObject);
			for (float num2 = 0f; num2 <= ui.DesignSize.x + 0.01f; num2 += num)
			{
				RectTransform rectTransform3 = NewQuad(boxLayer, "tick", new Color(1f, 1f, 1f, 0.9f));
				PaperUI.SetRect(rectTransform3, num2 - px * 0.5f, -4f * px, px, 4f * px);
				rulerObjects.Add(rectTransform3.gameObject);
				RectTransform rect;
				TextMeshProUGUI textMeshProUGUI = NewLabel(boxLayer, "ruler_label", out rect);
				textMeshProUGUI.text = Mathf.RoundToInt(num2).ToString();
				textMeshProUGUI.fontSize = 4f * px * 2f;
				rect.GetComponent<Image>().color = Color.clear;
				PaperUI.SetRect(rect, num2 + px, -11f * px, 20f * px, 7f * px);
				rulerObjects.Add(rect.gameObject);
			}
			for (float num3 = 0f; num3 <= ui.DesignSize.y + 0.01f; num3 += num)
			{
				RectTransform rectTransform4 = NewQuad(boxLayer, "tick", new Color(1f, 1f, 1f, 0.9f));
				PaperUI.SetRect(rectTransform4, -4f * px, num3 - px * 0.5f, 4f * px, px);
				rulerObjects.Add(rectTransform4.gameObject);
				RectTransform rect2;
				TextMeshProUGUI textMeshProUGUI2 = NewLabel(boxLayer, "ruler_label", out rect2);
				textMeshProUGUI2.text = Mathf.RoundToInt(num3).ToString();
				textMeshProUGUI2.fontSize = 4f * px * 2f;
				textMeshProUGUI2.alignment = TextAlignmentOptions.MidlineRight;
				rect2.GetComponent<Image>().color = Color.clear;
				PaperUI.SetRect(rect2, -26f * px, num3 + px, 20f * px, 7f * px);
				rulerObjects.Add(rect2.gameObject);
			}
		}
	}

	private static float PickRulerStep(float size)
	{
		if (size <= 150f)
		{
			return 10f;
		}
		if (size <= 400f)
		{
			return 25f;
		}
		if (size <= 900f)
		{
			return 50f;
		}
		return 100f;
	}
}

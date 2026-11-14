using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class PaperEditorInspect
{
	public class Info
	{
		public GameObject target;

		public string path;

		public Sprite sprite;

		public string spriteKey;

		public TextMeshProUGUI text;

		public string style;

		public float designFontSize;

		public Color color;

		public Vector2 designSize;

		public bool isButton;

		public string Summary
		{
			get
			{
				List<string> list = new List<string> { path };
				if (sprite != null)
				{
					list.Add("sprite: " + sprite.name + ((spriteKey != null) ? ("  (catalog key " + spriteKey + ")") : "  (not in the catalog: run Build > Collect Paper UI Assets)"));
				}
				if (text != null)
				{
					list.Add(string.Format("text: {0}\nfont: {1} {2:0.#} px  ->  style {3}, size {4:0.#}\ncolour {5}", Trim(text.text), (text.font != null) ? text.font.name : "?", text.fontSize, style, designFontSize, PaperTextStyle.ToHex(text.color)));
				}
				if (isButton)
				{
					list.Add("button");
				}
				list.Add($"size: {designSize.x:0.#} x {designSize.y:0.#} design units");
				return string.Join("\n", list);
			}
		}

		private static string Trim(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return "";
			}
			value = value.Replace("\n", " ");
			if (value.Length <= 60)
			{
				return value;
			}
			return value.Substring(0, 60) + "...";
		}
	}

	private static readonly List<RaycastResult> results = new List<RaycastResult>();

	public static Info Hovered { get; private set; }

	public static Info Pinned { get; private set; }

	public static void Clear()
	{
		Hovered = null;
		Pinned = null;
	}

	public static void Update(PaperEditor editor)
	{
		if (EventSystem.current == null || editor.Ui?.Root == null)
		{
			return;
		}
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = Input.mousePosition
		};
		results.Clear();
		EventSystem.current.RaycastAll(eventData, results);
		Info info = null;
		foreach (RaycastResult result in results)
		{
			GameObject gameObject = result.gameObject;
			if (!gameObject.transform.IsChildOf(editor.Ui.Root.transform))
			{
				if (!PaperEditorWindow.Contains(gameObject) && !(gameObject.GetComponentInParent<CustomUIWindowFrame>() != null))
				{
					info = Describe(gameObject, editor);
				}
				break;
			}
		}
		bool num = info?.target != Hovered?.target;
		Hovered = info;
		if (num && Pinned == null)
		{
			editor.RaiseChanged();
		}
		if (Input.GetMouseButtonDown(0) && info != null)
		{
			Pinned = info;
			editor.SetStatus("Pinned: " + info.path);
		}
	}

	private static Info Describe(GameObject go, PaperEditor editor)
	{
		Info info = new Info
		{
			target = go,
			path = Path(go.transform)
		};
		Image component = go.GetComponent<Image>();
		if (component != null && component.sprite != null)
		{
			info.sprite = component.sprite;
			info.spriteKey = PaperAssets.KeyOf(component.sprite);
		}
		TextMeshProUGUI textMeshProUGUI = go.GetComponent<TextMeshProUGUI>();
		if (textMeshProUGUI == null)
		{
			textMeshProUGUI = go.GetComponentInChildren<TextMeshProUGUI>();
		}
		if (textMeshProUGUI != null)
		{
			info.text = textMeshProUGUI;
			info.style = PaperTextStyle.GuessFromFont(textMeshProUGUI.font, textMeshProUGUI.fontSize);
			info.designFontSize = textMeshProUGUI.fontSize / Mathf.Max(0.01f, editor.Ui.DisplayScale);
			info.color = textMeshProUGUI.color;
		}
		info.isButton = go.GetComponent<Button>() != null || go.GetComponentInParent<Button>() != null;
		RectTransform rectTransform = go.transform as RectTransform;
		if (rectTransform != null)
		{
			Vector3[] array = new Vector3[4];
			rectTransform.GetWorldCorners(array);
			Canvas componentInParent = go.GetComponentInParent<Canvas>();
			float num = ((componentInParent != null) ? componentInParent.scaleFactor : 1f);
			Vector2 vector = new Vector2(Mathf.Abs(array[2].x - array[0].x), Mathf.Abs(array[2].y - array[0].y)) / num;
			info.designSize = vector / Mathf.Max(0.01f, editor.Ui.DisplayScale);
		}
		return info;
	}

	private static string Path(Transform t)
	{
		List<string> list = new List<string>();
		int num = 0;
		while (t != null && num < 4)
		{
			list.Insert(0, t.name);
			t = t.parent;
			num++;
		}
		return string.Join("/", list);
	}

	public static void AddLikeThis(PaperEditor editor)
	{
		Info info = Pinned ?? Hovered;
		if (info == null || !editor.IsEditing)
		{
			return;
		}
		PaperElementData element;
		if (info.text != null && (info.sprite == null || !info.isButton))
		{
			element = new PaperElementData
			{
				type = "text",
				text = info.text.text,
				style = info.style,
				fontSize = Mathf.Round(info.designFontSize * 10f) / 10f,
				color = PaperTextStyle.ToHex(info.text.color),
				align = ((info.text.horizontalAlignment == HorizontalAlignmentOptions.Center) ? "center" : ((info.text.horizontalAlignment == HorizontalAlignmentOptions.Right) ? "right" : "left")),
				wrap = info.text.enableWordWrapping,
				w = Mathf.Max(10f, Mathf.Round(info.designSize.x)),
				h = Mathf.Max(6f, Mathf.Round(info.designSize.y))
			};
		}
		else
		{
			if (!(info.sprite != null))
			{
				editor.SetStatus("Nothing to copy from that element (no sprite, no text).");
				return;
			}
			if (info.spriteKey == null)
			{
				editor.SetStatus("'" + info.sprite.name + "' is not in the paper asset catalog; run Build > Collect Paper UI Assets in the Unity editor and try again.");
				return;
			}
			element = new PaperElementData
			{
				type = (info.isButton ? "button" : "image"),
				sprite = info.spriteKey,
				action = (info.isButton ? "button" : null),
				w = Mathf.Max(2f, Mathf.Round(info.designSize.x)),
				h = Mathf.Max(2f, Mathf.Round(info.designSize.y))
			};
		}
		editor.AddElement(element);
		editor.SetStatus("Added an element like " + info.path);
	}
}

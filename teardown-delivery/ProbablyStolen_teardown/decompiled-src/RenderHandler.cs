using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RenderHandler : MonoBehaviour
{
	public enum ColorPalette
	{
		None = -1,
		White = 12895408,
		VeryLightGray = 11058895,
		LightGray = 8624059,
		Gray = 8950922,
		DarkGray = 5198736,
		VeryDarkGray = 4275576,
		Black = 2297671,
		LightPurple = 11567781,
		Purple = 10312861,
		DarkPurple = 8407419,
		VeryLightRed = 12822958,
		LightRed = 12615037,
		Red = 12472652,
		DarkRed = 9454675,
		VeryDarkRed = 6502234,
		LightOrange = 12092762,
		Orange = 11362567,
		DarkOrange = 8932401,
		LightYellow = 9672266,
		Yellow = 8877056,
		DarkYellow = 6511905,
		VeryLightGreen = 10533268,
		LightGreen = 8035658,
		Green = 5538048,
		DarkGreen = 4875297,
		VeryDarkGreen = 4212545,
		LightTurquoise = 4366987,
		Turquoise = 35154,
		DarkTurquoise = 1206625,
		VeryLightCyan = 8699076,
		LightCyan = 4367018,
		Cyan = 35216,
		DarkCyan = 1206656,
		VeryDarkCyan = 2378096,
		LightBlue = 4365501,
		Blue = 31927,
		DarkBlue = 1204883,
		EGreen = 8698449,
		ERed = 10177085
	}

	public enum BackgroundColor
	{
		None = -1,
		Red,
		Green,
		Blue,
		White,
		Black,
		Tan
	}

	public enum BackgroundType
	{
		Grid,
		Smooth,
		Border
	}

	public static RenderHandler current;

	public GameObject windowUICanvasObject;

	[NonSerialized]
	public Canvas windowUICanvas;

	public GameObject renderNodePrefab;

	public GameObject renderNodeTextPrefab;

	public GameObject windowRootObject;

	public GameObject tempRootObject;

	public GameObject overlayRootObject;

	[NonSerialized]
	public TreeNodeRender overlayRoot;

	[NonSerialized]
	public TreeNodeRender tempRoot;

	[NonSerialized]
	public TreeNodeRender windowRoot;

	[NonSerialized]
	public TreeNodeRender scenesRoot;

	[NonSerialized]
	public Color textColorWhite = new Color32(168, 190, 207, byte.MaxValue);

	[NonSerialized]
	public Color textColorRed = new Color32(213, 210, 184, byte.MaxValue);

	[NonSerialized]
	public Color textColorDarkRed = new Color32(99, 55, 90, byte.MaxValue);

	public static Sprite unknownSprite;

	[NonSerialized]
	public Sprite healthBarOutline;

	[NonSerialized]
	public Sprite healthBarBackground;

	[NonSerialized]
	public Sprite healthBar;

	[NonSerialized]
	public Sprite healthIcon;

	[NonSerialized]
	public Sprite healthIconLow;

	[NonSerialized]
	public Sprite moraleBarOutline;

	[NonSerialized]
	public Sprite moraleBarBackground;

	[NonSerialized]
	public Sprite moraleBar;

	[NonSerialized]
	public Sprite moraleIcon;

	[NonSerialized]
	public Sprite moraleIconLow;

	private static readonly Dictionary<string, Dictionary<string, Sprite>> atlasCache = new Dictionary<string, Dictionary<string, Sprite>>();

	[NonSerialized]
	public Sprite fill;

	[NonSerialized]
	public Sprite redSquare;

	[NonSerialized]
	public Sprite greenSquare;

	[NonSerialized]
	public Sprite blueSquare;

	[NonSerialized]
	public Sprite whiteSquare;

	[NonSerialized]
	public Sprite blackSquare;

	[NonSerialized]
	public Sprite tanSquare;

	[NonSerialized]
	public Sprite red;

	[NonSerialized]
	public Sprite green;

	[NonSerialized]
	public Sprite blue;

	[NonSerialized]
	public Sprite white;

	[NonSerialized]
	public Sprite black;

	[NonSerialized]
	public Sprite tan;

	[NonSerialized]
	public Sprite redBorder;

	[NonSerialized]
	public Sprite greenBorder;

	[NonSerialized]
	public Sprite blueBorder;

	[NonSerialized]
	public Sprite whiteBorder;

	[NonSerialized]
	public Sprite blackBorder;

	[NonSerialized]
	public Sprite tanBorder;

	[NonSerialized]
	public Sprite inside;

	[NonSerialized]
	public Sprite outside;

	[NonSerialized]
	public Sprite outsideCrop;

	[NonSerialized]
	public Sprite outsideBottomRight;

	[NonSerialized]
	public Sprite outsideBottomLeft;

	[NonSerialized]
	public Sprite outsideBottom;

	[NonSerialized]
	public Sprite outsideTopRight;

	[NonSerialized]
	public Sprite outsideTopLeft;

	[NonSerialized]
	public Sprite outsideTop;

	[NonSerialized]
	public Sprite outsideLeft;

	[NonSerialized]
	public Sprite outsideRight;

	[NonSerialized]
	public Sprite borderOutsideTopRounded;

	[NonSerialized]
	public Sprite borderOutsideTopRoundedCrop;

	public List<Color32> color32Palette = (from c in Enum.GetValues(typeof(ColorPalette)).Cast<ColorPalette>().Skip(1)
		select GetColorFromPalette(c)).ToList();

	public Canvas OverlayCanvas
	{
		get
		{
			if (overlayRootObject == null)
			{
				return null;
			}
			Canvas componentInParent = overlayRootObject.GetComponentInParent<Canvas>(includeInactive: true);
			if (!(componentInParent != null))
			{
				return null;
			}
			return componentInParent.rootCanvas;
		}
	}

	public RectTransform GetTempTransform()
	{
		return tempRoot.rectTransform;
	}

	public void ResetTempTransform()
	{
		tempRoot.rectTransform.sizeDelta = Vector2.zero;
		tempRoot.rectTransform.pivot = new Vector2(0f, 1f);
		tempRoot.rectTransform.anchorMin = new Vector2(0f, 1f);
		tempRoot.rectTransform.anchorMax = new Vector2(0f, 1f);
		tempRoot.rectTransform.anchoredPosition = new Vector2(0f, 0f);
		tempRoot.rectTransform.localRotation = Quaternion.identity;
		tempRoot.rectTransform.localScale = new Vector3(1f, 1f, 1f);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
		unknownSprite = LoadFromAtlas("Items/itematlas", "unknown");
		windowUICanvas = windowUICanvasObject.GetComponent<Canvas>();
		overlayRoot = TreeNodeRender.InstantiateAsRoot(overlayRootObject.transform);
		tempRoot = TreeNodeRender.InstantiateAsRoot(tempRootObject.transform);
		windowRoot = TreeNodeRender.InstantiateAsRoot(windowRootObject.transform);
		fill = LoadFromAtlas("Window/elements_outer", "background_fill");
		Sprite[] array = Resources.LoadAll<Sprite>("Window/background");
		Dictionary<string, Sprite> backgroundDict = new Dictionary<string, Sprite>();
		Array.ForEach(array, delegate(Sprite s)
		{
			backgroundDict.Add(s.name, s);
		});
		redSquare = backgroundDict.GetValueOrDefault("grid_red");
		greenSquare = backgroundDict.GetValueOrDefault("grid_green");
		blueSquare = backgroundDict.GetValueOrDefault("grid_blue");
		whiteSquare = backgroundDict.GetValueOrDefault("grid_white");
		blackSquare = backgroundDict.GetValueOrDefault("grid_black");
		tanSquare = backgroundDict.GetValueOrDefault("grid_tan");
		red = backgroundDict.GetValueOrDefault("red");
		green = backgroundDict.GetValueOrDefault("green");
		blue = backgroundDict.GetValueOrDefault("blue");
		white = backgroundDict.GetValueOrDefault("white");
		black = backgroundDict.GetValueOrDefault("black");
		tan = backgroundDict.GetValueOrDefault("tan");
		redBorder = backgroundDict.GetValueOrDefault("border_red");
		greenBorder = backgroundDict.GetValueOrDefault("border_green");
		blueBorder = backgroundDict.GetValueOrDefault("border_blue");
		whiteBorder = backgroundDict.GetValueOrDefault("border_white");
		blackBorder = backgroundDict.GetValueOrDefault("border_black");
		tanBorder = backgroundDict.GetValueOrDefault("border_tan");
		Sprite[] array2 = Resources.LoadAll<Sprite>("Window/border");
		Dictionary<string, Sprite> borderDict = new Dictionary<string, Sprite>();
		Array.ForEach(array2, delegate(Sprite s)
		{
			borderDict.Add(s.name, s);
		});
		inside = borderDict.GetValueOrDefault("inside");
		outside = borderDict.GetValueOrDefault("outside");
		outsideCrop = borderDict.GetValueOrDefault("outside_crop");
		outsideBottomRight = borderDict.GetValueOrDefault("outside_bottom_right");
		outsideBottomLeft = borderDict.GetValueOrDefault("outside_bottom_left");
		outsideBottom = borderDict.GetValueOrDefault("outside_bottom");
		outsideTopRight = borderDict.GetValueOrDefault("outside_top_right");
		outsideTopLeft = borderDict.GetValueOrDefault("outside_top_left");
		outsideTop = borderDict.GetValueOrDefault("outside_top");
		outsideLeft = borderDict.GetValueOrDefault("outside_left");
		outsideRight = borderDict.GetValueOrDefault("outside_right");
		borderOutsideTopRounded = LoadFromAtlas("Window/elements_outer", "border_outside_top_rounded");
		borderOutsideTopRoundedCrop = LoadFromAtlas("Window/elements_outer", "border_outside_top_rounded_crop");
	}

	private static Sprite LoadFromAtlasRaw(string atlasPath, string name)
	{
		if (name == null || atlasPath == null)
		{
			return null;
		}
		if (!atlasCache.TryGetValue(atlasPath, out var value))
		{
			Sprite[] array = Resources.LoadAll<Sprite>(atlasPath);
			if (array.Length < 1)
			{
				return null;
			}
			value = new Dictionary<string, Sprite>(array.Length);
			Sprite[] array2 = array;
			foreach (Sprite sprite in array2)
			{
				value[sprite.name] = sprite;
			}
			atlasCache[atlasPath] = value;
		}
		if (!value.TryGetValue(name, out var value2))
		{
			return null;
		}
		return value2;
	}

	public static Sprite LoadFromAtlas(string atlasPath, string name)
	{
		Sprite sprite = LoadFromAtlasRaw(atlasPath, name);
		if ((object)sprite == null)
		{
			Debug.LogWarning("Sprite name '" + name + "' in atlas '" + atlasPath + "' was not found. Loading dummy sprite.");
			return unknownSprite;
		}
		return sprite;
	}

	public static Sprite TryLoadFromAtlas(string atlasPath, string name)
	{
		return LoadFromAtlasRaw(atlasPath, name);
	}

	public static Sprite LoadFromFile(string spritePath)
	{
		Sprite sprite = Resources.Load<Sprite>(spritePath);
		if ((object)sprite == null)
		{
			Debug.LogWarning("Sprite path '" + spritePath + "' was not found. Loading dummy sprite.");
			return unknownSprite;
		}
		return sprite;
	}

	public void SetGridFillBackground(GridShape shape, int x, int y, TreeNodeRender background, TreeNodeRender r0, TreeNodeRender r1, TreeNodeRender r2, TreeNodeRender r3, Sprite freeSprite = null, Sprite solidSprite = null, bool bevel = true)
	{
		background.gameObject.SetActive(value: false);
		r0.gameObject.SetActive(value: false);
		r1.gameObject.SetActive(value: false);
		r2.gameObject.SetActive(value: false);
		r3.gameObject.SetActive(value: false);
		background.image.enabled = true;
		r0.image.enabled = true;
		r1.image.enabled = true;
		r2.image.enabled = true;
		r3.image.enabled = true;
		background.rectTransform.anchoredPosition = new Vector2(x * UISettings.current.gridSize, -y * UISettings.current.gridSize);
		r0.rectTransform.anchoredPosition = background.rectTransform.anchoredPosition;
		r1.rectTransform.anchoredPosition = background.rectTransform.anchoredPosition;
		r2.rectTransform.anchoredPosition = background.rectTransform.anchoredPosition;
		r3.rectTransform.anchoredPosition = background.rectTransform.anchoredPosition;
		if (shape.GetLocal(x, y) == 0)
		{
			background.image.sprite = ((freeSprite != null) ? freeSprite : tanSquare);
			background.gameObject.SetActive(value: true);
			background.image.SetNativeSize();
			return;
		}
		if (solidSprite != null)
		{
			background.image.sprite = solidSprite;
			background.gameObject.SetActive(value: true);
		}
		else
		{
			background.image.sprite = fill;
			background.gameObject.SetActive(value: false);
		}
		if (!bevel)
		{
			background.image.SetNativeSize();
			return;
		}
		if (shape.GetLocal(x + 1, y) == 0)
		{
			r0.image.sprite = outsideRight;
			r0.gameObject.SetActive(value: true);
		}
		else if (shape.GetLocal(x + 1, y - 1) == 0)
		{
			r0.image.sprite = outsideTopRight;
			r0.gameObject.SetActive(value: true);
		}
		if (shape.GetLocal(x - 1, y) == 0)
		{
			r1.image.sprite = outsideLeft;
			r1.gameObject.SetActive(value: true);
		}
		else if (shape.GetLocal(x - 1, y + 1) == 0)
		{
			r1.image.sprite = outsideBottomLeft;
			r1.gameObject.SetActive(value: true);
		}
		if (shape.GetLocal(x, y + 1) == 0)
		{
			r2.image.sprite = outsideBottom;
			r2.gameObject.SetActive(value: true);
		}
		else if (shape.GetLocal(x + 1, y + 1) == 0)
		{
			r2.image.sprite = outsideBottomRight;
			r2.gameObject.SetActive(value: true);
		}
		if (shape.GetLocal(x, y - 1) == 0)
		{
			r3.image.sprite = outsideTop;
			r3.gameObject.SetActive(value: true);
		}
		else if (shape.GetLocal(x - 1, y - 1) == 0)
		{
			r3.image.sprite = outsideTopLeft;
			r3.gameObject.SetActive(value: true);
		}
		background.image.SetNativeSize();
		r0.image.SetNativeSize();
		r1.image.SetNativeSize();
		r2.image.SetNativeSize();
		r3.image.SetNativeSize();
	}

	public static T RaycastFindParentElement<T>(Vector2 screenPosition, TreeNodeRender ignore = null)
	{
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = screenPosition
		};
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		foreach (RaycastResult item in list)
		{
			TreeNodeRender component = item.gameObject.GetComponent<TreeNodeRender>();
			if ((object)component != null && !(component == ignore))
			{
				return component.FindParentElement<T>();
			}
		}
		return default(T);
	}

	public static T RaycastElement<T>(Vector2 screenPosition, TreeNodeRender ignore = null)
	{
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = screenPosition
		};
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		foreach (RaycastResult item in list)
		{
			TreeNodeRender component = item.gameObject.GetComponent<TreeNodeRender>();
			if ((object)component != null && !(component == ignore))
			{
				return (component.FindParentElement() is T val) ? val : default(T);
			}
		}
		return default(T);
	}

	private static string InsertColorTag(string text, string hexColor)
	{
		return "<color=#" + hexColor + ">" + text + "</color>";
	}

	public static string GetStringFromColorPalette(ColorPalette color)
	{
		return $"#{(int)color:x6}";
	}

	public static Color32 GetColorFromPalette(ColorPalette color)
	{
		byte r = (byte)((uint)((int)color >> 16) & 0xFFu);
		byte g = (byte)((uint)((int)color >> 8) & 0xFFu);
		byte b = (byte)(color & (ColorPalette)255);
		return new Color32(r, g, b, byte.MaxValue);
	}

	public static string ApplyRichTextColorTag(string text, ColorPalette color)
	{
		if (color != ColorPalette.None)
		{
			int num = (int)color;
			return InsertColorTag(text, num.ToString("x6"));
		}
		return text;
	}

	public void SetHighlightColor(Image image, BackgroundColor color, BackgroundType type)
	{
		switch (color)
		{
		case BackgroundColor.Red:
			image.sprite = type switch
			{
				BackgroundType.Smooth => red, 
				BackgroundType.Grid => redSquare, 
				_ => redBorder, 
			};
			image.enabled = true;
			break;
		case BackgroundColor.Green:
			image.sprite = type switch
			{
				BackgroundType.Smooth => green, 
				BackgroundType.Grid => greenSquare, 
				_ => greenBorder, 
			};
			image.enabled = true;
			break;
		case BackgroundColor.Blue:
			image.sprite = type switch
			{
				BackgroundType.Smooth => blue, 
				BackgroundType.Grid => blueSquare, 
				_ => blueBorder, 
			};
			image.enabled = true;
			break;
		case BackgroundColor.White:
			image.sprite = type switch
			{
				BackgroundType.Smooth => white, 
				BackgroundType.Grid => whiteSquare, 
				_ => whiteBorder, 
			};
			image.enabled = true;
			break;
		case BackgroundColor.Black:
			image.sprite = type switch
			{
				BackgroundType.Smooth => black, 
				BackgroundType.Grid => blackSquare, 
				_ => blackBorder, 
			};
			image.enabled = true;
			break;
		case BackgroundColor.Tan:
			image.sprite = type switch
			{
				BackgroundType.Smooth => tan, 
				BackgroundType.Grid => tanSquare, 
				_ => tanBorder, 
			};
			image.enabled = true;
			break;
		default:
			image.enabled = false;
			break;
		}
	}
}

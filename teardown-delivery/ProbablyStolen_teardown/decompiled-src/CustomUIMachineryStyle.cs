using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public static class CustomUIMachineryStyle
{
	public const float SCALE = 2f;

	private const float PPU_MULTIPLIER = 0.5f;

	private const string CHROME_ATLAS = "Window/elements_outer";

	private const float LABEL_DESIGN_SIZE = 8f;

	private const float LINE_HEIGHT_FACTOR = 1.6f;

	private static Sprite CloseNormal => RenderHandler.LoadFromAtlas("Window/elements_outer", "border_top_button_x");

	private static Sprite CloseHover => RenderHandler.LoadFromAtlas("Window/elements_outer", "border_top_button_x_select");

	private static float BodySize => 16f;

	private static float ButtonTextSize => 14f;

	private static float TitleSize => 20f;

	private static float ButtonHeight => 32f;

	private static float CloseSize => 32f;

	private static float ContentPad => 16f;

	public static TMP_FontAsset Font()
	{
		if (FontLoader.Instance == null || LocalizationSettings.SelectedLocale == null)
		{
			return null;
		}
		return FontLoader.Instance.GetFontByRole(FontLoader.FontRole.Noto_Sans, LocalizationSettings.SelectedLocale);
	}

	public static GameObject CreateFrame(Transform parent)
	{
		GameObject gameObject = NewRect("machinery_window", parent);
		Image image = gameObject.AddComponent<Image>();
		Sliced(image, MachineryStyle.BevelFrame, Color.white);
		image.raycastTarget = true;
		float num = 40f;
		float num2 = 2f;
		float num3 = 8f;
		GameObject gameObject2 = NewRect("header", gameObject.transform);
		RectTransform obj = gameObject2.transform as RectTransform;
		obj.anchorMin = new Vector2(0f, 1f);
		obj.anchorMax = new Vector2(1f, 1f);
		obj.pivot = new Vector2(0.5f, 1f);
		obj.anchoredPosition = Vector2.zero;
		obj.sizeDelta = new Vector2(0f, num);
		Sliced(gameObject2.AddComponent<Image>(), MachineryStyle.BevelFrame, Color.white);
		GameObject gameObject3 = NewRect("title", gameObject2.transform);
		Stretch(gameObject3.transform as RectTransform, CloseSize + 8f, CloseSize + 8f, 0f, 0f);
		TextMeshProUGUI textMeshProUGUI = gameObject3.AddComponent<TextMeshProUGUI>();
		InitText(textMeshProUGUI, TitleSize, MachineryStyle.TITLE_TEXT, HorizontalAlignmentOptions.Center, uppercase: true, wrap: false);
		textMeshProUGUI.overflowMode = TextOverflowModes.Ellipsis;
		GameObject gameObject4 = NewRect("close", gameObject2.transform);
		RectTransform obj2 = gameObject4.transform as RectTransform;
		obj2.anchorMin = new Vector2(1f, 1f);
		obj2.anchorMax = new Vector2(1f, 1f);
		obj2.pivot = new Vector2(1f, 1f);
		obj2.anchoredPosition = new Vector2(-4f, -4f);
		obj2.sizeDelta = new Vector2(CloseSize, CloseSize);
		Image image2 = gameObject4.AddComponent<Image>();
		image2.sprite = CloseNormal;
		image2.type = Image.Type.Simple;
		Button button = gameObject4.AddComponent<Button>();
		SpriteSwap(button, image2, CloseHover, CloseHover);
		GameObject gameObject5 = NewRect("rule", gameObject.transform);
		RectTransform obj3 = gameObject5.transform as RectTransform;
		obj3.anchorMin = new Vector2(0f, 1f);
		obj3.anchorMax = new Vector2(1f, 1f);
		obj3.pivot = new Vector2(0.5f, 1f);
		obj3.anchoredPosition = new Vector2(0f, 0f - num);
		obj3.sizeDelta = new Vector2((0f - num3) * 2f, num2);
		Image image3 = gameObject5.AddComponent<Image>();
		Flat(image3, MachineryStyle.RULE);
		image3.raycastTarget = false;
		GameObject gameObject6 = NewRect("content", gameObject.transform);
		RectTransform rectTransform = gameObject6.transform as RectTransform;
		Stretch(rectTransform, ContentPad, ContentPad, num + num2 + ContentPad, ContentPad);
		VerticalLayoutGroup verticalLayoutGroup = gameObject6.AddComponent<VerticalLayoutGroup>();
		verticalLayoutGroup.spacing = 8f;
		verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
		verticalLayoutGroup.childControlWidth = true;
		verticalLayoutGroup.childControlHeight = true;
		verticalLayoutGroup.childForceExpandWidth = false;
		verticalLayoutGroup.childForceExpandHeight = false;
		UIDragHandler dragHandle = gameObject.AddComponent<UIDragHandler>();
		CustomUIWindowFrame customUIWindowFrame = gameObject.AddComponent<CustomUIWindowFrame>();
		customUIWindowFrame.contentRoot = rectTransform;
		customUIWindowFrame.titleText = textMeshProUGUI;
		customUIWindowFrame.closeButton = button;
		customUIWindowFrame.dragHandle = dragHandle;
		return gameObject;
	}

	public static GameObject Create(CustomUIManager.ElementKind kind, Transform parent)
	{
		return kind switch
		{
			CustomUIManager.ElementKind.Label => CreateText(parent, "label", BodySize, MachineryStyle.BODY_TEXT, uppercase: false), 
			CustomUIManager.ElementKind.Header => CreateText(parent, "header", TitleSize, MachineryStyle.TITLE_TEXT, uppercase: true), 
			CustomUIManager.ElementKind.Warning => CreateText(parent, "warning", BodySize, MachineryStyle.PROGRESS_FILL_DANGER, uppercase: false), 
			CustomUIManager.ElementKind.Button => CreateButton(parent), 
			CustomUIManager.ElementKind.CloseButton => CreateCloseButton(parent), 
			CustomUIManager.ElementKind.Image => CreateImage(parent), 
			CustomUIManager.ElementKind.Toggle => CreateToggle(parent), 
			CustomUIManager.ElementKind.Slider => CreateSlider(parent), 
			CustomUIManager.ElementKind.Separator => CreateSeparator(parent), 
			CustomUIManager.ElementKind.Panel => CreatePanel(parent), 
			CustomUIManager.ElementKind.ProgressBar => CreateProgressBar(parent), 
			_ => null, 
		};
	}

	public static void Reskin(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		Image[] componentsInChildren = go.GetComponentsInChildren<Image>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Sliced(componentsInChildren[i], MachineryStyle.PlateNotched, MachineryStyle.SLIDER_TRACK);
		}
		TextMeshProUGUI[] componentsInChildren2 = go.GetComponentsInChildren<TextMeshProUGUI>(includeInactive: true);
		foreach (TextMeshProUGUI textMeshProUGUI in componentsInChildren2)
		{
			TMP_FontAsset tMP_FontAsset = Font();
			if (tMP_FontAsset != null)
			{
				textMeshProUGUI.font = tMP_FontAsset;
			}
			textMeshProUGUI.color = MachineryStyle.BODY_TEXT;
			textMeshProUGUI.fontSize = BodySize;
		}
		Scrollbar[] componentsInChildren3 = go.GetComponentsInChildren<Scrollbar>(includeInactive: true);
		foreach (Scrollbar scrollbar in componentsInChildren3)
		{
			Image image = ((scrollbar.handleRect != null) ? scrollbar.handleRect.GetComponent<Image>() : null);
			if (image != null)
			{
				Sliced(image, MachineryStyle.PlateNotched, MachineryStyle.SLIDER_FILL);
			}
		}
	}

	private static GameObject CreateText(Transform parent, string name, float size, Color color, bool uppercase)
	{
		GameObject gameObject = NewRect(name, parent);
		InitText(gameObject.AddComponent<TextMeshProUGUI>(), size, color, HorizontalAlignmentOptions.Left, uppercase, wrap: true);
		gameObject.AddComponent<LayoutElement>().minHeight = Mathf.Ceil(size * 1.6f);
		return gameObject;
	}

	private static GameObject CreateButton(Transform parent)
	{
		GameObject gameObject = NewRect("button", parent);
		Image image = gameObject.AddComponent<Image>();
		Sliced(image, MachineryStyle.ButtonNormal, Color.white);
		image.raycastTarget = true;
		SpriteSwap(gameObject.AddComponent<Button>(), image, MachineryStyle.ButtonHover, MachineryStyle.ButtonPressed);
		HorizontalLayoutGroup horizontalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
		int num = Mathf.RoundToInt(12f);
		horizontalLayoutGroup.padding = new RectOffset(num, num, 0, 0);
		horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
		horizontalLayoutGroup.childControlWidth = true;
		horizontalLayoutGroup.childControlHeight = true;
		horizontalLayoutGroup.childForceExpandWidth = false;
		horizontalLayoutGroup.childForceExpandHeight = false;
		LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
		layoutElement.minHeight = ButtonHeight;
		layoutElement.preferredHeight = ButtonHeight;
		layoutElement.minWidth = 32f;
		InitText(NewRect("label", gameObject.transform).AddComponent<TextMeshProUGUI>(), ButtonTextSize, MachineryStyle.BUTTON_TEXT, HorizontalAlignmentOptions.Center, uppercase: false, wrap: false);
		return gameObject;
	}

	private static GameObject CreateCloseButton(Transform parent)
	{
		GameObject gameObject = NewRect("close_button", parent);
		Image image = gameObject.AddComponent<Image>();
		image.sprite = CloseNormal;
		image.type = Image.Type.Simple;
		image.raycastTarget = true;
		SpriteSwap(gameObject.AddComponent<Button>(), image, CloseHover, CloseHover);
		LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
		layoutElement.minWidth = CloseSize;
		layoutElement.minHeight = CloseSize;
		layoutElement.preferredWidth = CloseSize;
		layoutElement.preferredHeight = CloseSize;
		return gameObject;
	}

	private static GameObject CreateImage(Transform parent)
	{
		GameObject gameObject = NewRect("image", parent);
		Image image = gameObject.AddComponent<Image>();
		image.raycastTarget = false;
		image.preserveAspect = true;
		gameObject.AddComponent<LayoutElement>();
		return gameObject;
	}

	private static GameObject CreateToggle(Transform parent)
	{
		float num = 32f;
		GameObject gameObject = NewRect("toggle", parent);
		HorizontalLayoutGroup horizontalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
		horizontalLayoutGroup.spacing = 12f;
		horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
		horizontalLayoutGroup.childControlWidth = true;
		horizontalLayoutGroup.childControlHeight = true;
		horizontalLayoutGroup.childForceExpandWidth = false;
		horizontalLayoutGroup.childForceExpandHeight = false;
		LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
		layoutElement.minHeight = num;
		layoutElement.preferredHeight = num;
		GameObject gameObject2 = NewRect("Background", gameObject.transform);
		Image image = gameObject2.AddComponent<Image>();
		image.sprite = MachineryStyle.RadioOff;
		image.type = Image.Type.Simple;
		image.raycastTarget = true;
		LayoutElement layoutElement2 = gameObject2.AddComponent<LayoutElement>();
		layoutElement2.minWidth = num;
		layoutElement2.minHeight = num;
		layoutElement2.preferredWidth = num;
		layoutElement2.preferredHeight = num;
		GameObject gameObject3 = NewRect("Checkmark", gameObject2.transform);
		Stretch(gameObject3.transform as RectTransform, 0f, 0f, 0f, 0f);
		Image image2 = gameObject3.AddComponent<Image>();
		image2.sprite = MachineryStyle.RadioOn;
		image2.type = Image.Type.Simple;
		image2.raycastTarget = false;
		TextMeshProUGUI textMeshProUGUI = NewRect("Label", gameObject.transform).AddComponent<TextMeshProUGUI>();
		InitText(textMeshProUGUI, BodySize, MachineryStyle.BODY_TEXT, HorizontalAlignmentOptions.Left, uppercase: true, wrap: false);
		textMeshProUGUI.raycastTarget = true;
		Toggle toggle = gameObject.AddComponent<Toggle>();
		toggle.targetGraphic = image;
		toggle.graphic = image2;
		toggle.transition = Selectable.Transition.SpriteSwap;
		SpriteState spriteState = toggle.spriteState;
		spriteState.highlightedSprite = MachineryStyle.RadioOffHover;
		spriteState.pressedSprite = MachineryStyle.RadioPressed;
		spriteState.selectedSprite = null;
		toggle.spriteState = spriteState;
		return gameObject;
	}

	private static GameObject CreateSlider(Transform parent)
	{
		float a = 16f;
		float num = 8f;
		float num2 = 32f;
		GameObject gameObject = NewRect("slider", parent);
		LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
		layoutElement.minWidth = 192f;
		layoutElement.preferredHeight = Mathf.Max(a, 16f);
		layoutElement.minHeight = layoutElement.preferredHeight;
		layoutElement.flexibleWidth = 1f;
		float y = 16f;
		float num3 = 2f;
		GameObject gameObject2 = NewRect("Background", gameObject.transform);
		RectTransform obj = gameObject2.transform as RectTransform;
		obj.anchorMin = new Vector2(0f, 0.5f);
		obj.anchorMax = new Vector2(1f, 0.5f);
		obj.pivot = new Vector2(0.5f, 0.5f);
		obj.anchoredPosition = Vector2.zero;
		obj.sizeDelta = new Vector2(0f, num);
		Image image = gameObject2.AddComponent<Image>();
		Sliced(image, MachineryStyle.PlateNotched, MachineryStyle.SLIDER_TRACK);
		image.raycastTarget = true;
		GameObject gameObject3 = NewRect("Empty", gameObject2.transform);
		Stretch(gameObject3.transform as RectTransform, num3, num3, num3, num3);
		Image image2 = gameObject3.AddComponent<Image>();
		Flat(image2, MachineryStyle.SLIDER_EMPTY);
		image2.raycastTarget = false;
		GameObject gameObject4 = NewRect("Fill Area", gameObject.transform);
		RectTransform obj2 = gameObject4.transform as RectTransform;
		obj2.anchorMin = new Vector2(0f, 0.5f);
		obj2.anchorMax = new Vector2(1f, 0.5f);
		obj2.pivot = new Vector2(0.5f, 0.5f);
		obj2.anchoredPosition = Vector2.zero;
		obj2.sizeDelta = new Vector2(-2f * num3, num - 2f * num3);
		GameObject gameObject5 = NewRect("Fill", gameObject4.transform);
		RectTransform rectTransform = gameObject5.transform as RectTransform;
		Stretch(rectTransform, 0f, 0f, 0f, 0f);
		Image image3 = gameObject5.AddComponent<Image>();
		Flat(image3, MachineryStyle.SLIDER_FILL);
		image3.raycastTarget = false;
		GameObject gameObject6 = NewRect("Handle Slide Area", gameObject.transform);
		Stretch(gameObject6.transform as RectTransform, num2 * 0.5f, num2 * 0.5f, 0f, 0f);
		GameObject gameObject7 = NewRect("Handle", gameObject6.transform);
		RectTransform rectTransform2 = gameObject7.transform as RectTransform;
		rectTransform2.anchorMin = new Vector2(0f, 0.5f);
		rectTransform2.anchorMax = new Vector2(0f, 0.5f);
		rectTransform2.pivot = new Vector2(0.5f, 0.5f);
		rectTransform2.sizeDelta = new Vector2(num2, y);
		Image image4 = gameObject7.AddComponent<Image>();
		image4.sprite = MachineryStyle.SliderThumbNormal;
		image4.type = Image.Type.Simple;
		image4.preserveAspect = true;
		image4.raycastTarget = true;
		Slider slider = gameObject.AddComponent<Slider>();
		slider.fillRect = rectTransform;
		slider.handleRect = rectTransform2;
		slider.targetGraphic = image4;
		slider.direction = Slider.Direction.LeftToRight;
		slider.transition = Selectable.Transition.SpriteSwap;
		SpriteState spriteState = slider.spriteState;
		spriteState.highlightedSprite = MachineryStyle.SliderThumbHover;
		spriteState.pressedSprite = MachineryStyle.SliderThumbHover;
		spriteState.selectedSprite = null;
		slider.spriteState = spriteState;
		return gameObject;
	}

	private static GameObject CreateSeparator(Transform parent)
	{
		GameObject gameObject = NewRect("separator", parent);
		Image image = gameObject.AddComponent<Image>();
		Flat(image, MachineryStyle.RULE);
		image.raycastTarget = false;
		LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
		layoutElement.minHeight = 2f;
		layoutElement.preferredHeight = 2f;
		layoutElement.flexibleWidth = 1f;
		return gameObject;
	}

	private static GameObject CreatePanel(Transform parent)
	{
		GameObject gameObject = NewRect("panel", parent);
		Image image = gameObject.AddComponent<Image>();
		Sliced(image, MachineryStyle.BevelFrame, Color.white);
		image.raycastTarget = false;
		return gameObject;
	}

	private static GameObject CreateProgressBar(Transform parent)
	{
		float num = 16f;
		float num2 = 4f;
		GameObject gameObject = NewRect("progress", parent);
		Image image = gameObject.AddComponent<Image>();
		Sliced(image, MachineryStyle.ProgressFrame, Color.white);
		image.raycastTarget = false;
		LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
		layoutElement.minHeight = num;
		layoutElement.preferredHeight = num;
		layoutElement.minWidth = 140f;
		layoutElement.flexibleWidth = 1f;
		GameObject gameObject2 = NewRect("fill", gameObject.transform);
		Stretch(gameObject2.transform as RectTransform, num2, num2, num2, num2);
		Image image2 = gameObject2.AddComponent<Image>();
		Flat(image2, MachineryStyle.PROGRESS_FILL);
		image2.type = Image.Type.Filled;
		image2.fillMethod = Image.FillMethod.Horizontal;
		image2.fillOrigin = 0;
		image2.raycastTarget = false;
		gameObject.AddComponent<CustomUIProgressBar>().fillImage = image2;
		return gameObject;
	}

	private static GameObject NewRect(string name, Transform parent)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		return gameObject;
	}

	private static void Stretch(RectTransform rect, float left, float right, float top, float bottom)
	{
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.offsetMin = new Vector2(left, bottom);
		rect.offsetMax = new Vector2(0f - right, 0f - top);
	}

	private static void Sliced(Image image, Sprite sprite, Color color)
	{
		image.sprite = sprite;
		image.type = Image.Type.Sliced;
		image.pixelsPerUnitMultiplier = 0.5f;
		image.color = color;
	}

	private static void Flat(Image image, Color color)
	{
		image.sprite = MachineryStyle.White;
		image.type = Image.Type.Simple;
		image.color = color;
	}

	private static void SpriteSwap(Selectable selectable, Image target, Sprite hover, Sprite pressed)
	{
		selectable.targetGraphic = target;
		selectable.transition = Selectable.Transition.SpriteSwap;
		SpriteState spriteState = selectable.spriteState;
		spriteState.highlightedSprite = hover;
		spriteState.pressedSprite = pressed;
		spriteState.selectedSprite = null;
		selectable.spriteState = spriteState;
	}

	private static void InitText(TextMeshProUGUI text, float size, Color color, HorizontalAlignmentOptions horizontal, bool uppercase, bool wrap)
	{
		TMP_FontAsset tMP_FontAsset = Font();
		if (tMP_FontAsset != null)
		{
			text.font = tMP_FontAsset;
		}
		text.fontSize = size;
		text.color = color;
		text.fontStyle = (uppercase ? FontStyles.UpperCase : FontStyles.Normal);
		text.horizontalAlignment = horizontal;
		text.verticalAlignment = VerticalAlignmentOptions.Middle;
		text.enableWordWrapping = wrap;
		text.overflowMode = (wrap ? TextOverflowModes.Overflow : TextOverflowModes.Overflow);
		text.enableAutoSizing = false;
		text.richText = true;
		text.raycastTarget = false;
	}
}

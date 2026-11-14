using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public static class TransitUI
{
	public const string WINDOW_ID = "transit_system";

	private static CustomUIWindow window;

	private static TransitFrame frame;

	private static readonly List<AfterhourLocation> listed = new List<AfterhourLocation>();

	private static readonly List<TransitCardView> cards = new List<TransitCardView>();

	private static AfterhourLocation selected;

	private static int page;

	private static bool travelHovered;

	public static GameObject Root
	{
		get
		{
			if (window == null || !window.IsAlive)
			{
				return null;
			}
			return window.root;
		}
	}

	public static bool IsOpen
	{
		get
		{
			if (window != null && window.IsAlive)
			{
				return window.IsVisible;
			}
			return false;
		}
	}

	private static int PageCount => Mathf.Max(1, (listed.Count + 5 - 1) / 5);

	public static bool EnsureCreated(GameObject prefab)
	{
		if (window != null && window.IsAlive)
		{
			return true;
		}
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[TransitUI] No CustomUIManager in the scene, the transit window cannot be created");
			return false;
		}
		if (prefab == null)
		{
			Debug.LogError("[TransitUI] MapUIManager.transitPrefab is not assigned. Run Build > Generate Transit UI Prefab in the store scene.");
			return false;
		}
		window = CustomUIManager.Instance.CreateWindow("transit_system", null, "meta", prefab).GetWindow();
		frame = ((window.root != null) ? window.root.GetComponent<TransitFrame>() : null);
		if (frame == null)
		{
			Debug.LogError("[TransitUI] transitPrefab has no TransitFrame on its root; regenerate it");
			window.Close();
			window = null;
			return false;
		}
		window.root.name = "TransitSystem";
		frame.closeButton.onClick.AddListener(OnCloseClicked);
		frame.travelButton.onClick.AddListener(OnTravelClicked);
		frame.prevButton.onClick.AddListener(delegate
		{
			ShowPage(page - 1);
		});
		frame.nextButton.onClick.AddListener(delegate
		{
			ShowPage(page + 1);
		});
		frame.travelButton.gameObject.AddComponent<TransitHoverRelay>().onHover = delegate(bool entering)
		{
			travelHovered = entering;
			RefreshTravelButton();
		};
		window.root.SetActive(value: false);
		return true;
	}

	public static void Open(GameObject prefab)
	{
		if (EnsureCreated(prefab))
		{
			frame.title.text = LocHelper.GetLocalizedUI("ui_transit_title");
			frame.travelLabel.text = LocHelper.GetLocalizedUI("ui_transit_travel");
			selected = null;
			travelHovered = false;
			listed.Clear();
			listed.AddRange(AfterhourLocation.GetDiscovered());
			ShowPage(0);
			window.Show();
		}
	}

	public static void Close()
	{
		if (window != null && window.IsAlive)
		{
			window.Hide();
		}
		selected = null;
	}

	private static void OnCloseClicked()
	{
		EmporiumAudioPlayer.Instance.audioButton2.Play();
		MapUIManager.Instance.CloseUI();
	}

	private static void OnTravelClicked()
	{
		if (selected != null && selected.IsAvailable())
		{
			EmporiumAudioPlayer.Instance.audioButton2.Play();
			MapUIManager.Instance.Travel(selected);
		}
	}

	private static void ShowPage(int index)
	{
		page = Mathf.Clamp(index, 0, PageCount - 1);
		ClearChildren(frame.cardsHost);
		ClearChildren(frame.trackHost);
		cards.Clear();
		int num = page * 5;
		int num2 = Mathf.Min(5, listed.Count - num);
		NewImage(frame.trackHost, "TrackLine", 0f, 175f, 1728f, 16f, null, TransitStyle.TRACK).type = Image.Type.Simple;
		float num3 = (float)(5 - num2) * 352f * 0.5f;
		for (int i = 0; i < num2; i++)
		{
			AfterhourLocation location = listed[num + i];
			float num4 = num3 + (float)i * 352f;
			BuildStation(num4);
			cards.Add(BuildCard(location, num4));
		}
		if (selected != null && listed.IndexOf(selected) / 5 != page)
		{
			selected = null;
		}
		bool active = PageCount > 1;
		frame.prevButton.gameObject.SetActive(active);
		frame.nextButton.gameObject.SetActive(active);
		SetArrow(frame.prevButton, frame.prevImage, page > 0);
		SetArrow(frame.nextButton, frame.nextImage, page < PageCount - 1);
		RefreshSelection();
	}

	private static void BuildStation(float offset)
	{
		NewImage(frame.trackHost, "Connector", 129f + offset - 64f, 10f, 133f, 174f, frame.connector, Color.white).type = Image.Type.Simple;
		NewImage(frame.trackHost, "Station", 108f + offset - 64f, 161f, 42f, 42f, frame.station, Color.white).type = Image.Type.Simple;
	}

	private static TransitCardView BuildCard(AfterhourLocation location, float x)
	{
		bool flag = location.IsAvailable();
		RectTransform rectTransform = NewRect(frame.cardsHost, "Card_" + location.Id, x, 0f, 320f, 448f);
		Image image = rectTransform.gameObject.AddComponent<Image>();
		image.color = Color.clear;
		image.raycastTarget = true;
		NewImage(rectTransform, "Shadow", -8f, 8f, 320f, 448f, frame.cardPlate, new Color(0f, 0f, 0f, 0.25f));
		NewImage(rectTransform, "Plate", 0f, 0f, 320f, 448f, frame.cardPlate, Color.white);
		NewImage(rectTransform, "HeaderPlate", 0f, 0f, 320f, 42f, frame.cardHeaderPlate, new Color(1f, 1f, 1f, 0.05f));
		TextMeshProUGUI textMeshProUGUI = NewText(rectTransform, "Header", 2f, 0f, 316f, 42f, FontLoader.FontRole.Lubrifont, 24f, flag ? TransitStyle.CARD_HEADER_TEXT : WithAlpha(TransitStyle.CARD_HEADER_TEXT, 0.3f), TextAlignmentOptions.Center, wrap: false);
		textMeshProUGUI.fontStyle = FontStyles.UpperCase;
		textMeshProUGUI.text = location.GetTitle();
		WildNetworkUI.AutoSize(textMeshProUGUI, 12f);
		RectTransform rectTransform2 = NewRect(rectTransform, "Image", 12f, 54f, 296f, 168f);
		Image image2 = rectTransform2.gameObject.AddComponent<Image>();
		image2.color = TransitStyle.CARD_IMAGE_PLATE;
		image2.raycastTarget = false;
		rectTransform2.gameObject.AddComponent<RectMask2D>();
		Sprite sprite = ResolveImage(location.ImageKey);
		if (sprite != null)
		{
			float num = Mathf.Max(296f / sprite.rect.width, 168f / sprite.rect.height);
			float num2 = sprite.rect.width * num;
			float num3 = sprite.rect.height * num;
			Image image3 = NewImage(rectTransform2, "Picture", (296f - num2) * 0.5f, (168f - num3) * 0.5f, num2, num3, sprite, Color.white);
			image3.type = Image.Type.Simple;
			image3.preserveAspect = true;
		}
		else
		{
			NewText(rectTransform2, "Placeholder", 0f, 0f, 296f, 168f, FontLoader.FontRole.Noto_Sans_Normal, 24f, TransitStyle.CARD_PLACEHOLDER_TEXT, TextAlignmentOptions.Center, wrap: false).text = "IMAGE PLACEHOLDER";
		}
		if (!flag)
		{
			TextMeshProUGUI textMeshProUGUI2 = NewText(rectTransform, "Status", 12f, 240f, 296f, 25f, FontLoader.FontRole.Noto_Sans_Normal, 18f, TransitStyle.STATUS_UNAVAILABLE, TextAlignmentOptions.Center, wrap: false);
			textMeshProUGUI2.text = LocHelper.GetLocalizedUI("ui_transit_unavailable");
			WildNetworkUI.AutoSize(textMeshProUGUI2, 12f);
		}
		ScrollRect scroll = BuildEventList(rectTransform, location, flag ? 0f : 35f);
		Image outline = NewImage(rectTransform, "Outline", 0f, 0f, 320f, 448f, flag ? frame.cardOutlineNormal : frame.cardOutlineUnavailable, Color.white);
		TransitCardView transitCardView = rectTransform.gameObject.AddComponent<TransitCardView>();
		transitCardView.location = location;
		transitCardView.outline = outline;
		transitCardView.available = flag;
		transitCardView.scroll = scroll;
		return transitCardView;
	}

	private static ScrollRect BuildEventList(RectTransform card, AfterhourLocation location, float topOffset)
	{
		float y = 240f + topOffset;
		float height = 192f - topOffset;
		RectTransform rectTransform = NewRect(card, "Events", 16f, y, 288f, height);
		ScrollRect scrollRect = rectTransform.gameObject.AddComponent<ScrollRect>();
		RectTransform rectTransform2 = NewRect(rectTransform, "Viewport", 0f, 0f, 288f, height);
		rectTransform2.anchorMin = Vector2.zero;
		rectTransform2.anchorMax = Vector2.one;
		rectTransform2.offsetMin = Vector2.zero;
		rectTransform2.offsetMax = Vector2.zero;
		rectTransform2.gameObject.AddComponent<RectMask2D>();
		RectTransform rectTransform3 = NewRect(rectTransform2, "Content", 0f, 0f, 288f, height);
		rectTransform3.anchorMin = new Vector2(0f, 1f);
		rectTransform3.anchorMax = new Vector2(1f, 1f);
		rectTransform3.pivot = new Vector2(0f, 1f);
		rectTransform3.anchoredPosition = Vector2.zero;
		rectTransform3.sizeDelta = new Vector2(0f, 0f);
		VerticalLayoutGroup verticalLayoutGroup = rectTransform3.gameObject.AddComponent<VerticalLayoutGroup>();
		verticalLayoutGroup.childControlWidth = true;
		verticalLayoutGroup.childControlHeight = true;
		verticalLayoutGroup.childForceExpandWidth = true;
		verticalLayoutGroup.childForceExpandHeight = false;
		verticalLayoutGroup.spacing = 2f;
		ContentSizeFitter contentSizeFitter = rectTransform3.gameObject.AddComponent<ContentSizeFitter>();
		contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
		contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		List<AfterhourEventDefinition> activeEvents = location.ActiveEvents;
		for (int i = 0; i < activeEvents.Count; i++)
		{
			AfterhourEventDefinition afterhourEventDefinition = activeEvents[i];
			if (i > 0)
			{
				NewRect(rectTransform3, "Gap", 0f, 0f, 0f, 8f).gameObject.AddComponent<LayoutElement>().minHeight = 8f;
			}
			NewText(rectTransform3, "EventTitle", 0f, 0f, 288f, 25f, FontLoader.FontRole.Noto_Sans_Normal, 18f, ToneColor(afterhourEventDefinition.GetTone()), TextAlignmentOptions.Center, wrap: true).text = afterhourEventDefinition.GetTitle();
			string text = afterhourEventDefinition.GetDescription();
			string text2 = DurationText(afterhourEventDefinition.id);
			bool flag = string.IsNullOrEmpty(text);
			if (!string.IsNullOrEmpty(text2))
			{
				string text3 = "<b><color=#" + ColorUtility.ToHtmlStringRGB(TransitStyle.EVENT_DURATION_TEXT) + ">(" + text2 + ")</color></b>";
				text = (flag ? text3 : (text + " " + text3));
			}
			if (!string.IsNullOrEmpty(text))
			{
				TextMeshProUGUI textMeshProUGUI = NewText(rectTransform3, "EventDescription", 0f, 0f, 288f, 0f, FontLoader.FontRole.Noto_Sans_Normal, 18f, TransitStyle.EVENT_DESCRIPTION_TEXT, flag ? TextAlignmentOptions.Center : TextAlignmentOptions.TopLeft, wrap: true);
				textMeshProUGUI.richText = true;
				textMeshProUGUI.text = text;
			}
			foreach (string effectText in afterhourEventDefinition.GetEffectTexts())
			{
				TextMeshProUGUI textMeshProUGUI2 = NewText(rectTransform3, "EventEffect", 0f, 0f, 288f, 0f, FontLoader.FontRole.Noto_Sans_Normal, 18f, TransitStyle.EVENT_EFFECT_TEXT, TextAlignmentOptions.TopLeft, wrap: true);
				textMeshProUGUI2.richText = true;
				textMeshProUGUI2.text = "•<indent=" + 14f + "px>" + effectText + "</indent>";
			}
		}
		RectTransform rectTransform4 = NewRect(card, "Scrollbar", 306f, y, 6f, height);
		Image image = NewImage(rectTransform4, "Track", 2f, 0f, 2f, height, frame.scrollTrack, Color.white);
		image.rectTransform.anchorMin = new Vector2(0.5f, 0f);
		image.rectTransform.anchorMax = new Vector2(0.5f, 1f);
		image.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		image.rectTransform.anchoredPosition = Vector2.zero;
		image.rectTransform.sizeDelta = new Vector2(2f, 0f);
		RectTransform rectTransform5 = NewRect(rectTransform4, "SlidingArea", 0f, 0f, 0f, 0f);
		rectTransform5.anchorMin = Vector2.zero;
		rectTransform5.anchorMax = Vector2.one;
		rectTransform5.offsetMin = Vector2.zero;
		rectTransform5.offsetMax = Vector2.zero;
		RectTransform rectTransform6 = NewRect(rectTransform5, "Handle", 0f, 0f, 0f, 0f);
		rectTransform6.anchorMin = Vector2.zero;
		rectTransform6.anchorMax = Vector2.one;
		rectTransform6.offsetMin = Vector2.zero;
		rectTransform6.offsetMax = Vector2.zero;
		Image image2 = rectTransform6.gameObject.AddComponent<Image>();
		WildNetworkUI.ApplySprite(image2, frame.scrollThumb, Color.white);
		image2.raycastTarget = true;
		rectTransform4.gameObject.AddComponent<TransitClickSwallow>();
		Scrollbar scrollbar = rectTransform4.gameObject.AddComponent<Scrollbar>();
		scrollbar.handleRect = rectTransform6;
		scrollbar.targetGraphic = image2;
		scrollbar.direction = Scrollbar.Direction.BottomToTop;
		scrollbar.transition = Selectable.Transition.None;
		scrollRect.content = rectTransform3;
		scrollRect.viewport = rectTransform2;
		scrollRect.horizontal = false;
		scrollRect.vertical = true;
		scrollRect.movementType = ScrollRect.MovementType.Clamped;
		scrollRect.scrollSensitivity = 40f;
		scrollRect.verticalScrollbar = scrollbar;
		scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
		scrollRect.verticalNormalizedPosition = 1f;
		return scrollRect;
	}

	private static Color ToneColor(AfterhourEventTone tone)
	{
		return tone switch
		{
			AfterhourEventTone.Good => TransitStyle.EVENT_GOOD, 
			AfterhourEventTone.Bad => TransitStyle.EVENT_BAD, 
			_ => TransitStyle.EVENT_MIXED, 
		};
	}

	private static string DurationText(string eventId)
	{
		int daysLeft = AfterhourEvents.GetDaysLeft(eventId);
		if (daysLeft <= 0)
		{
			return "";
		}
		if (daysLeft == 1)
		{
			return LocHelper.GetLocalizedMechanic("mech_afterhour_event_today_only");
		}
		return LocHelper.GetLocalizedMechanic("mech_afterhour_event_days_left", daysLeft);
	}

	private static Sprite ResolveImage(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		Sprite sprite = frame.GetImage(key);
		if (sprite == null && CustomUIManager.Instance != null)
		{
			sprite = CustomUIManager.Instance.GetSprite(key);
		}
		return sprite;
	}

	public static void Select(AfterhourLocation location)
	{
		if (location != null && location.IsAvailable())
		{
			selected = ((selected == location) ? null : location);
			EmporiumAudioPlayer.Instance.audioButton1.Play();
			RefreshSelection();
		}
	}

	private static void RefreshSelection()
	{
		foreach (TransitCardView card in cards)
		{
			card.outline.sprite = ((!card.available) ? frame.cardOutlineUnavailable : ((card.location == selected) ? frame.cardOutlineSelected : frame.cardOutlineNormal));
		}
		RefreshTravelButton();
	}

	private static void RefreshTravelButton()
	{
		bool flag = selected != null && selected.IsAvailable();
		frame.travelButton.interactable = flag;
		frame.travelGroup.alpha = (flag ? 1f : 0.3f);
		Color color = ((flag && travelHovered) ? TransitStyle.BUTTON_BORDER_HOVER : TransitStyle.BUTTON_BORDER);
		frame.travelOuter.color = color;
		frame.travelLabel.color = color;
	}

	private static void SetArrow(Button button, Image image, bool enabled)
	{
		button.interactable = enabled;
		image.color = new Color(1f, 1f, 1f, enabled ? 1f : 0.3f);
	}

	private static void ClearChildren(RectTransform container)
	{
		if (!(container == null))
		{
			for (int num = container.childCount - 1; num >= 0; num--)
			{
				Object.Destroy(container.GetChild(num).gameObject);
			}
		}
	}

	private static Color WithAlpha(Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, alpha);
	}

	private static RectTransform NewRect(Transform parent, string name, float x, float y, float width, float height)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		RectTransform obj = gameObject.transform as RectTransform;
		WildNetworkUI.SetRect(obj, x, y, width, height);
		return obj;
	}

	private static Image NewImage(Transform parent, string name, float x, float y, float width, float height, Sprite sprite, Color color)
	{
		Image image = NewRect(parent, name, x, y, width, height).gameObject.AddComponent<Image>();
		WildNetworkUI.ApplySprite(image, sprite, color);
		image.raycastTarget = false;
		return image;
	}

	private static TextMeshProUGUI NewText(Transform parent, string name, float x, float y, float width, float height, FontLoader.FontRole role, float size, Color color, TextAlignmentOptions alignment, bool wrap)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.SetActive(value: false);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		WildNetworkUI.SetRect(gameObject.transform as RectTransform, x, y, width, height);
		TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
		textMeshProUGUI.font = ((role == FontLoader.FontRole.Lubrifont) ? frame.fontLubrifont : frame.fontNotoRegular);
		textMeshProUGUI.fontSize = size;
		textMeshProUGUI.color = color;
		textMeshProUGUI.alignment = alignment;
		textMeshProUGUI.enableWordWrapping = wrap;
		textMeshProUGUI.enableAutoSizing = false;
		textMeshProUGUI.overflowMode = TextOverflowModes.Overflow;
		textMeshProUGUI.raycastTarget = false;
		gameObject.AddComponent<LocalizeStringEvent>();
		TMPLocalizer tMPLocalizer = gameObject.AddComponent<TMPLocalizer>();
		tMPLocalizer.ChangeFontOnly = true;
		tMPLocalizer.fontRole = role;
		gameObject.SetActive(value: true);
		return textMeshProUGUI;
	}
}

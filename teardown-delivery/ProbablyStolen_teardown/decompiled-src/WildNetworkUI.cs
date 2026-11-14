using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public static class WildNetworkUI
{
	private class TabView
	{
		public Button button;

		public TextMeshProUGUI label;
	}

	private class SlotView
	{
		public string id;

		public Image icon;

		public Image border;

		public Image padlock;

		public RectTransform tag;

		public Image tagImage;

		public TextMeshProUGUI tagText;
	}

	public const string WINDOW_ID = "wilds_network";

	private static CustomUIWindow window;

	private static WildNetworkFrame frame;

	private static List<NetworkUpgradeLayout.Tab> tabs;

	private static readonly List<TabView> tabViews = new List<TabView>();

	private static readonly List<SlotView> slots = new List<SlotView>();

	private static int currentTab;

	private static string selectedId;

	private static string hoveredId;

	private static readonly char[] statLabelTrim = new char[4] { ' ', ':', '：', '\u00a0' };

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

	public static void Open(bool debug = false)
	{
		if (IsOpen)
		{
			RefreshAll();
			return;
		}
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[WildNetworkUI] No CustomUIManager in the scene, cannot open Wild's Network");
			return;
		}
		if (CustomUIManager.Instance.networkUIPrefab == null)
		{
			Debug.LogError("[WildNetworkUI] CustomUIManager.networkUIPrefab is not assigned. Run Build > Generate Wild Network UI Prefab and assign UI3/Prefab/WildNetworkMenu.prefab.");
			return;
		}
		if (PlayerStore.Instance == null || PlayerStore.Instance.networkUpgrade == null)
		{
			Debug.LogError("[WildNetworkUI] No run loaded, cannot open Wild's Network");
			return;
		}
		window = CustomUIManager.Instance.CreateWindow("wilds_network", null, "rear", CustomUIManager.Instance.networkUIPrefab).OnClose(OnWindowClosed).GetWindow();
		frame = ((window.root != null) ? window.root.GetComponent<WildNetworkFrame>() : null);
		if (frame == null)
		{
			Debug.LogError("[WildNetworkUI] networkUIPrefab has no WildNetworkFrame on its root; regenerate it");
			window.Close();
			return;
		}
		window.SetTitle(LocHelper.GetLocalizedUI("inte_wild_network"));
		frame.useButton.onClick.RemoveAllListeners();
		frame.useButton.onClick.AddListener(OnUseClicked);
		NetworkUpgrade.InvalidateDefinitions();
		NetworkUpgrade.AddMissingAfterLoad();
		selectedId = null;
		hoveredId = null;
		currentTab = 0;
		BuildTabs();
		SelectTab(0);
		ShowInfo(null);
		RefreshStats();
		PlayerStore.Instance.daySinceLastBrowseUpgrade = 0;
		if (!debug)
		{
			EmporiumAnalytics.TrackWildUIOpened();
		}
		window.Show();
	}

	public static void Close()
	{
		if (IsOpen)
		{
			window.Close();
			return;
		}
		window = null;
		frame = null;
		tabViews.Clear();
		slots.Clear();
	}

	private static void OnWindowClosed()
	{
		window = null;
		frame = null;
		tabViews.Clear();
		slots.Clear();
		if (TutorialUIManager.Instance != null && TutorialUIManager.Instance.isIntroBrowsingUpgrade)
		{
			TutorialUIManager.Instance.isIntroBrowsingUpgrade = false;
			DialogUIManager.Instance.StartDialogue(StoreClientDialogList.WildNetworkContinue());
		}
		else if (PlayerStore.Instance != null && PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint().identifier.Equals("wild_upgrade_reminder"))
		{
			PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
		}
		if (EmporiumAudioPlayer.Instance != null)
		{
			EmporiumAudioPlayer.Instance.audioButton1.Play();
		}
	}

	public static void RefreshAll()
	{
		if (!IsOpen)
		{
			return;
		}
		foreach (SlotView slot in slots)
		{
			RefreshSlot(slot);
		}
		ShowInfo(selectedId);
		RefreshStats();
	}

	private static void BuildTabs()
	{
		tabs = NetworkUpgradeLayout.Build(NetworkUpgrade.GetDefinitionsInOrder().FindAll((NetworkUpgrade d) => !d.hidden && PlayerStore.Instance.networkUpgrade.ContainsKey(d.id) && !NetworkUpgrade.IsHiddenForRun(d.id)));
		ClearChildren(frame.tabBar);
		tabViews.Clear();
		float num = 0f;
		for (int i = 0; i < tabs.Count; i++)
		{
			int index = i;
			RectTransform rectTransform = NewRect(frame.tabBar, "tab_" + i, num, 0f, 256f, 42f);
			Image image = rectTransform.gameObject.AddComponent<Image>();
			ApplySprite(image, frame.tabNormal, Color.white);
			image.raycastTarget = true;
			Button button = rectTransform.gameObject.AddComponent<Button>();
			button.targetGraphic = image;
			button.transition = Selectable.Transition.SpriteSwap;
			SpriteState spriteState = button.spriteState;
			spriteState.highlightedSprite = frame.tabHover;
			spriteState.pressedSprite = frame.tabActive;
			spriteState.selectedSprite = frame.tabNormal;
			spriteState.disabledSprite = frame.tabActive;
			button.spriteState = spriteState;
			button.onClick.AddListener(delegate
			{
				SelectTab(index);
			});
			TextMeshProUGUI label = NewText(rectTransform, "label", 0f, 0f, 256f, 42f, FontLoader.FontRole.Lubrifont, 22f, WildNetworkStyle.TAB_TEXT, TextAlignmentOptions.Center, wrap: false);
			label.fontStyle = FontStyles.UpperCase;
			label.text = NetworkUpgrade.GetTabLabel(tabs[i].tab);
			AutoSize(label, 12f);
			WildNetworkSlot wildNetworkSlot = rectTransform.gameObject.AddComponent<WildNetworkSlot>();
			wildNetworkSlot.id = i.ToString();
			wildNetworkSlot.onHover = delegate(string _, bool entering)
			{
				if (index != currentTab && !(label == null))
				{
					label.color = (entering ? WildNetworkStyle.TAB_TEXT_HOVER : WildNetworkStyle.TAB_TEXT);
				}
			};
			tabViews.Add(new TabView
			{
				button = button,
				label = label
			});
			num += 268f;
		}
	}

	private static void SelectTab(int index)
	{
		if (tabs != null && tabs.Count != 0)
		{
			currentTab = Mathf.Clamp(index, 0, tabs.Count - 1);
			for (int i = 0; i < tabViews.Count; i++)
			{
				bool flag = i == currentTab;
				tabViews[i].button.interactable = !flag;
				tabViews[i].label.color = WildNetworkStyle.TAB_TEXT;
			}
			BuildGrid(tabs[currentTab]);
		}
	}

	private static void BuildGrid(NetworkUpgradeLayout.Tab tab)
	{
		ClearChildren(frame.gridContent);
		slots.Clear();
		hoveredId = null;
		float num = 0f;
		int num2 = 0;
		while (num2 < tab.cards.Count)
		{
			if (tab.cards[num2].recommended)
			{
				num += BuildCard(tab.cards[num2], 0f, num) + 16f;
				num2++;
				continue;
			}
			float num3 = 0f;
			int i;
			for (i = 0; i < 3 && num2 + i < tab.cards.Count && !tab.cards[num2 + i].recommended; i++)
			{
				NetworkUpgradeLayout.Card card = tab.cards[num2 + i];
				float x = (float)i * 378f;
				float b = BuildCard(card, x, num);
				num3 = Mathf.Max(num3, b);
			}
			num += num3 + 16f;
			num2 += i;
		}
		float y = Mathf.Max(0f, num - 16f);
		frame.gridContent.sizeDelta = new Vector2(frame.gridContent.sizeDelta.x, y);
		if (frame.scroll != null)
		{
			frame.scroll.verticalNormalizedPosition = 1f;
		}
	}

	private static float BuildCard(NetworkUpgradeLayout.Card card, float x, float y)
	{
		float num = WildNetworkStyle.CardHeight(card.rows);
		float width = (card.recommended ? 1118f : 362f);
		Color color = (card.recommended ? WildNetworkStyle.RECOMMENDED_PLATE : Color.white);
		RectTransform parent = NewRect(frame.gridContent, card.recommended ? "card_recommended" : ("card_" + card.subtitle), x, y, width, num);
		NewImage(parent, "frame", 0f, 0f, width, num, frame.cardPlate, color);
		NewImage(parent, "header", 0f, 0f, width, 42f, frame.cardHeaderPlate, color);
		TextMeshProUGUI textMeshProUGUI = NewText(parent, "title", 0f, 0f, width, 42f, FontLoader.FontRole.Lubrifont, 24f, card.recommended ? WildNetworkStyle.RECOMMENDED_TITLE : WildNetworkStyle.CARD_TITLE, TextAlignmentOptions.Center, wrap: false);
		textMeshProUGUI.fontStyle = FontStyles.UpperCase;
		textMeshProUGUI.text = CardTitle(card);
		AutoSize(textMeshProUGUI, 12f);
		bool[,] array = new bool[card.rows, card.columns];
		foreach (NetworkUpgradeLayout.Cell cell in card.cells)
		{
			float num2 = 31f + (float)cell.column * 108f;
			float num3 = 74f + (float)cell.row * 116f;
			array[cell.row, cell.column] = true;
			if (cell.connectorToNext)
			{
				NewImage(parent, "connector", num2 + 84f, num3 + 40f, 24f, 4f, frame.connector, Color.white);
			}
			BuildSlot(parent, cell.id, num2, num3);
		}
		for (int i = 0; i < card.rows; i++)
		{
			for (int j = 0; j < card.columns; j++)
			{
				if (!array[i, j])
				{
					BuildEmptySlot(parent, 31f + (float)j * 108f, 74f + (float)i * 116f);
				}
			}
		}
		return num;
	}

	private static string CardTitle(NetworkUpgradeLayout.Card card)
	{
		if (card.recommended)
		{
			return LocHelper.GetLocalizedUI("option_graphic_recommended");
		}
		Func<string> provider = null;
		foreach (NetworkUpgradeLayout.Cell cell in card.cells)
		{
			NetworkUpgrade definition = NetworkUpgrade.GetDefinition(cell.id);
			if (definition != null && definition.subtitleProvider != null)
			{
				provider = definition.subtitleProvider;
				break;
			}
		}
		return NetworkUpgrade.GetLocalizedSubtitle(card.subtitle, provider);
	}

	private static void BuildEmptySlot(RectTransform parent, float x, float y)
	{
		RectTransform rectTransform = NewRect(parent, "slot_empty", x, y, 84f, 84f);
		NewImage(rectTransform, "frame", 0f, 0f, 84f, 84f, frame.slotFrame, Color.white);
		float num = 76f;
		NewImage(rectTransform, "inset", 4f, 4f, num, num, frame.slotInset, Color.white);
		CanvasGroup canvasGroup = rectTransform.gameObject.AddComponent<CanvasGroup>();
		canvasGroup.alpha = WildNetworkStyle.SLOT_EMPTY_ALPHA.a;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;
	}

	private static void BuildSlot(RectTransform parent, string id, float x, float y)
	{
		float num = 84f;
		RectTransform rectTransform = NewRect(parent, "slot_" + id, x, y, num, num);
		NewImage(rectTransform, "frame", 0f, 0f, num, num, frame.slotFrame, Color.white).raycastTarget = true;
		float num2 = 6f;
		float num3 = 72f;
		NewImage(rectTransform, "icon_bg", num2, num2, num3, num3, null, WildNetworkStyle.SLOT_ICON_BG);
		Image image = NewImage(rectTransform, "icon", num2, num2, num3, num3, ResolveIcon(id), Color.white);
		image.preserveAspect = true;
		float num4 = num - 8f;
		NewImage(rectTransform, "inset", 4f, 4f, num4, num4, frame.slotInset, Color.white);
		Image border = NewImage(rectTransform, "border", 0f, 0f, num, num, frame.slotBorderNormal, Color.white);
		float num5 = 32f;
		Image image2 = NewImage(rectTransform, "padlock", (num - num5) * 0.5f, (num - num5) * 0.5f, num5, num5, frame.padlock, Color.white);
		image2.preserveAspect = true;
		image2.gameObject.SetActive(value: false);
		RectTransform rectTransform2 = NewRect(rectTransform, "price", (num - 56f) * 0.5f, 72f, 56f, 22f);
		Image image3 = rectTransform2.gameObject.AddComponent<Image>();
		ApplySprite(image3, frame.priceTagNormal, Color.white);
		image3.raycastTarget = false;
		TextMeshProUGUI textMeshProUGUI = NewText(rectTransform2, "cost", 0f, 0f, 56f, 22f, FontLoader.FontRole.Noto_Sans, 20f, WildNetworkStyle.PRICE_TAG_NORMAL_TEXT, TextAlignmentOptions.Center, wrap: false);
		StretchToParent(textMeshProUGUI.rectTransform);
		WildNetworkSlot wildNetworkSlot = rectTransform.gameObject.AddComponent<WildNetworkSlot>();
		wildNetworkSlot.id = id;
		wildNetworkSlot.onClick = OnSlotClicked;
		wildNetworkSlot.onHover = OnSlotHover;
		SlotView slotView = new SlotView
		{
			id = id,
			icon = image,
			border = border,
			padlock = image2,
			tag = rectTransform2,
			tagImage = image3,
			tagText = textMeshProUGUI
		};
		slots.Add(slotView);
		RefreshSlot(slotView);
	}

	private static Sprite ResolveIcon(string id)
	{
		NetworkUpgrade definition = NetworkUpgrade.GetDefinition(id);
		Sprite sprite = null;
		if (definition != null && !string.IsNullOrEmpty(definition.iconKey))
		{
			sprite = CustomUIManager.Instance.GetSprite(definition.iconKey);
			if (sprite == null)
			{
				sprite = frame.FindIcon(definition.iconKey);
			}
		}
		if (sprite == null)
		{
			sprite = frame.FindIcon(id);
		}
		if (sprite == null)
		{
			sprite = frame.FindIcon("BUY_REFERAL");
		}
		return sprite;
	}

	private static void RefreshSlot(SlotView view)
	{
		if (view != null && !(view.icon == null) && PlayerStore.Instance.networkUpgrade.TryGetValue(view.id, out var value))
		{
			bool flag = value.IsUnlocked();
			bool flag2 = value.IsHavePrerequisite();
			bool flag3 = false;
			int cost = value.GetCost();
			bool flag4 = cost <= PlayerStore.Instance.playerCash + PlayerStore.Instance.wildFavor;
			bool flag5 = value.IsReady() && flag2 && !flag3 && flag4;
			float a = (flag ? 0.7f : ((flag2 && !flag3) ? 1f : 0.5f));
			view.icon.color = new Color(1f, 1f, 1f, a);
			view.padlock.gameObject.SetActive(flag3);
			view.tag.gameObject.SetActive(!flag);
			if (!flag)
			{
				view.tagImage.sprite = (flag5 ? frame.priceTagNormal : frame.priceTagUnavailable);
				view.tagText.color = (flag5 ? WildNetworkStyle.PRICE_TAG_NORMAL_TEXT : WildNetworkStyle.PRICE_TAG_UNAVAILABLE_TEXT);
				view.tagText.text = cost.ToString();
				float num = Mathf.Max(56f, view.tagText.preferredWidth + 12f);
				view.tag.sizeDelta = new Vector2(num, 22f);
				view.tag.anchoredPosition = new Vector2((84f - num) * 0.5f, -72f);
			}
			RefreshSlotBorder(view, flag);
		}
	}

	private static void RefreshSlotBorder(SlotView view, bool unlocked)
	{
		bool flag = view.id == selectedId || view.id == hoveredId;
		view.border.sprite = (flag ? frame.slotBorderHover : (unlocked ? frame.slotBorderPurchased : frame.slotBorderNormal));
	}

	private static void OnSlotHover(string id, bool entering)
	{
		if (entering)
		{
			hoveredId = id;
		}
		else if (hoveredId == id)
		{
			hoveredId = null;
		}
		RefreshBordersOf(id);
	}

	private static void RefreshBordersOf(string id)
	{
		if (id == null || !PlayerStore.Instance.networkUpgrade.TryGetValue(id, out var value))
		{
			return;
		}
		bool unlocked = value.IsUnlocked();
		foreach (SlotView slot in slots)
		{
			if (slot.id == id && slot.border != null)
			{
				RefreshSlotBorder(slot, unlocked);
			}
		}
	}

	private static void OnSlotClicked(string id)
	{
		if (EmporiumAudioPlayer.Instance != null)
		{
			EmporiumAudioPlayer.Instance.audioLampClick.Play();
		}
		string id2 = selectedId;
		selectedId = id;
		RefreshBordersOf(id2);
		RefreshBordersOf(id);
		ShowInfo(id);
	}

	private static void ShowInfo(string id)
	{
		NetworkUpgrade value = null;
		if (id != null)
		{
			PlayerStore.Instance.networkUpgrade.TryGetValue(id, out value);
		}
		if (value == null)
		{
			frame.infoTitle.text = "";
			frame.infoSubtitle.text = "";
			frame.infoDescription.text = "";
			SetInfoLine(frame.infoCooldown, "", WildNetworkStyle.INFO_COOLDOWN);
			SetInfoLine(frame.infoMessage, "", WildNetworkStyle.INFO_WARNING);
			frame.useButton.gameObject.SetActive(value: false);
			return;
		}
		frame.infoTitle.text = value.GetLocalizedTitle();
		frame.infoSubtitle.text = value.GetLocalizedSubtitle();
		frame.infoDescription.text = value.GetLocalizedDescription();
		bool flag = value.cooldownDuration != 0 && value.cooldownCurrent != 0;
		SetInfoLine(frame.infoCooldown, flag ? LocHelper.GetLocalizedUI("network_ui_cooldown", value.cooldownCurrent) : "", WildNetworkStyle.INFO_COOLDOWN);
		bool flag2 = value.IsUnlocked();
		bool flag3 = value.IsHavePrerequisite();
		int cost = value.GetCost();
		string value2 = "";
		Color color = WildNetworkStyle.INFO_WARNING;
		bool flag4 = true;
		if (false)
		{
			value2 = LocHelper.GetLocalizedUI("network_ui_demo_locked");
			flag4 = false;
		}
		else if (value.isRepeatable && flag2)
		{
			value2 = value.GetLocalizedAlreadyBought();
			color = WildNetworkStyle.INFO_COOLDOWN;
			flag4 = false;
		}
		else if (!flag2 && !flag3)
		{
			value2 = LocHelper.GetLocalizedUI("network_ui_prerequisite", value.GetMissingPrerequisite());
			flag4 = false;
		}
		SetInfoLine(frame.infoMessage, value2, color);
		frame.useButton.gameObject.SetActive(flag4);
		if (flag4)
		{
			bool flag5 = value.IsReady() && !flag2 && cost <= PlayerStore.Instance.playerCash + PlayerStore.Instance.wildFavor;
			frame.useButton.interactable = flag5;
			frame.useLabel.text = LocHelper.GetLocalizedUI("inte_wild_unlock");
			frame.usePrice.text = LocHelper.GetLocalizedUI("network_ui_cost", cost);
			frame.useInnerBorder.color = (flag5 ? WildNetworkStyle.BUTTON_INNER : WildNetworkStyle.BUTTON_INNER_UNAVAILABLE);
			frame.useLabel.color = (flag5 ? WildNetworkStyle.BUTTON_TEXT : WildNetworkStyle.WithAlpha(WildNetworkStyle.BUTTON_TEXT, 0.3f));
			frame.usePrice.color = (flag5 ? WildNetworkStyle.BUTTON_TEXT : WildNetworkStyle.BUTTON_TEXT_UNAVAILABLE);
		}
	}

	private static void SetInfoLine(TextMeshProUGUI text, string value, Color color)
	{
		if (!(text == null))
		{
			text.text = value;
			text.color = color;
			text.gameObject.SetActive(!string.IsNullOrEmpty(value));
		}
	}

	private static void RefreshStats()
	{
		frame.fundsLabel.text = StatLabel(LocHelper.GetLocalizedUI("network_ui_available_credit", ""));
		frame.fundsValue.text = PlayerStore.Instance.playerCash.ToString();
		frame.favorLabel.text = StatLabel(LocHelper.GetLocalizedUI("network_ui_wild_favor", "")) + " <link=\"ui_wild_favor\"><sprite name=\"info\" tint=1></link>";
		frame.favorValue.text = PlayerStore.Instance.wildFavor.ToString();
	}

	private static string StatLabel(string formatted)
	{
		return (formatted ?? "").TrimEnd(statLabelTrim);
	}

	public static void AutoSize(TextMeshProUGUI text, float minSize)
	{
		if (!(text == null))
		{
			text.fontSizeMax = text.fontSize;
			text.fontSizeMin = minSize;
			text.enableAutoSizing = true;
		}
	}

	private static void OnUseClicked()
	{
		if (selectedId == null || !PlayerStore.Instance.networkUpgrade.TryGetValue(selectedId, out var value) || !value.IsReady() || value.IsUnlocked())
		{
			return;
		}
		int cost = value.GetCost();
		if (cost <= PlayerStore.Instance.playerCash + PlayerStore.Instance.wildFavor)
		{
			int cashSpent = 0;
			if (PlayerStore.Instance.wildFavor >= cost)
			{
				PlayerStore.Instance.wildFavor -= cost;
			}
			else
			{
				int num = cost - PlayerStore.Instance.wildFavor;
				PlayerStore.Instance.wildFavor = 0;
				PlayerStore.Instance.playerCash -= num;
				cashSpent = num;
			}
			RunStats.OnWildUpgradeBought(cashSpent);
			value.Unlock();
			EmporiumAnalytics.TrackWildUpgradePurchased(selectedId);
			if (EmporiumAudioPlayer.Instance != null)
			{
				EmporiumAudioPlayer.Instance.PlayRegister();
			}
			RefreshAll();
		}
	}

	private static void ClearChildren(RectTransform container)
	{
		if (!(container == null))
		{
			for (int num = container.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(container.GetChild(num).gameObject);
			}
		}
	}

	private static RectTransform NewRect(Transform parent, string name, float x, float y, float width, float height)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		RectTransform obj = gameObject.transform as RectTransform;
		SetRect(obj, x, y, width, height);
		return obj;
	}

	public static void SetRect(RectTransform rect, float x, float y, float width, float height)
	{
		rect.anchorMin = new Vector2(0f, 1f);
		rect.anchorMax = new Vector2(0f, 1f);
		rect.pivot = new Vector2(0f, 1f);
		rect.anchoredPosition = new Vector2(x, 0f - y);
		rect.sizeDelta = new Vector2(width, height);
	}

	private static void StretchToParent(RectTransform rect)
	{
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
	}

	private static Image NewImage(Transform parent, string name, float x, float y, float width, float height, Sprite sprite, Color color)
	{
		Image image = NewRect(parent, name, x, y, width, height).gameObject.AddComponent<Image>();
		ApplySprite(image, sprite, color);
		image.raycastTarget = false;
		return image;
	}

	public static void ApplySprite(Image image, Sprite sprite, Color color)
	{
		image.sprite = sprite;
		image.color = color;
		image.type = ((sprite != null && sprite.border != Vector4.zero) ? Image.Type.Sliced : Image.Type.Simple);
		image.pixelsPerUnitMultiplier = 1f;
	}

	private static TextMeshProUGUI NewText(Transform parent, string name, float x, float y, float width, float height, FontLoader.FontRole role, float size, Color color, TextAlignmentOptions alignment, bool wrap)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.SetActive(value: false);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		SetRect(gameObject.transform as RectTransform, x, y, width, height);
		TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
		textMeshProUGUI.font = ((role == FontLoader.FontRole.Lubrifont) ? frame.fontLubrifont : frame.fontNotoMedium);
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

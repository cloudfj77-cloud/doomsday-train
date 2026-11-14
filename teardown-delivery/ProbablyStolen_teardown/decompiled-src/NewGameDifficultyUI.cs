using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class NewGameDifficultyUI
{
	private class PlateView
	{
		public int index;

		public Image border;

		public bool hovered;
	}

	public const string WINDOW_ID = "new_game_difficulty";

	private static CustomUIWindow window;

	private static NewGameDifficultyFrame frame;

	private static List<Difficulty> difficulties = new List<Difficulty>();

	private static readonly List<PlateView> plates = new List<PlateView>();

	private static int selected;

	private static int page;

	private static bool startingGame;

	private static bool continueHovered;

	private static bool skipIntroForced;

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

	public static Difficulty Selected
	{
		get
		{
			if (window == null || selected < 0 || selected >= difficulties.Count)
			{
				return null;
			}
			return difficulties[selected];
		}
	}

	public static void Open()
	{
		if (window != null && window.IsAlive)
		{
			window.Show();
			RefreshOptions();
			return;
		}
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[NewGameDifficultyUI] No CustomUIManager in the scene, cannot open the difficulty screen");
			return;
		}
		if (CustomUIManager.Instance.newGameDifficultyPrefab == null)
		{
			Debug.LogError("[NewGameDifficultyUI] CustomUIManager.newGameDifficultyPrefab is not assigned. Run Build > Generate New Game Difficulty UI Prefab in the menu scene.");
			return;
		}
		window = CustomUIManager.Instance.CreateWindow("new_game_difficulty", null, "menu", CustomUIManager.Instance.newGameDifficultyPrefab).SetCloseOnEscape().OnClose(OnWindowClosed)
			.GetWindow();
		frame = ((window.root != null) ? window.root.GetComponent<NewGameDifficultyFrame>() : null);
		if (frame == null)
		{
			Debug.LogError("[NewGameDifficultyUI] newGameDifficultyPrefab has no NewGameDifficultyFrame on its root; regenerate it");
			window.Close();
			return;
		}
		startingGame = false;
		continueHovered = false;
		frame.headerTitle.text = LocHelper.GetLocalizedMainMenu("new_difficulty");
		frame.continueLabel.text = LocHelper.GetLocalizedMainMenu("new_start_game");
		frame.skipIntro.label.text = LocHelper.GetLocalizedMainMenu("new_skip_intro");
		frame.endless.label.text = LocHelper.GetLocalizedMainMenu("new_endless_mode");
		frame.backButton.onClick.AddListener(NewGameUIElements.PlayClick);
		frame.prevButton.onClick.RemoveAllListeners();
		frame.prevButton.onClick.AddListener(delegate
		{
			TurnPage(-1);
		});
		frame.nextButton.onClick.RemoveAllListeners();
		frame.nextButton.onClick.AddListener(delegate
		{
			TurnPage(1);
		});
		frame.continueButton.onClick.RemoveAllListeners();
		frame.continueButton.onClick.AddListener(OnStartClicked);
		WildNetworkSlot wildNetworkSlot = frame.continueButton.GetComponent<WildNetworkSlot>();
		if (wildNetworkSlot == null)
		{
			wildNetworkSlot = frame.continueButton.gameObject.AddComponent<WildNetworkSlot>();
		}
		wildNetworkSlot.id = "start";
		wildNetworkSlot.onHover = delegate(string _, bool entering)
		{
			continueHovered = entering;
			NewGameUIElements.RefreshContinueLook(frame, continueHovered);
		};
		WireCheckbox(frame.skipIntro, OnSkipIntroClicked, LocHelper.GetLocalizedMainMenu("main_skip_intro_tooltip"));
		WireCheckbox(frame.endless, OnEndlessClicked, LocHelper.GetLocalizedMainMenu("main_endless_mode_tooltip"));
		skipIntroForced = StartingScenario.Get(StartingScenario.ResolveId(NewGameData.Instance.startType, NewGameData.Instance.startScenarioId))?.skipIntro ?? false;
		if (skipIntroForced)
		{
			NewGameData.Instance.skipIntro = true;
		}
		difficulties = Difficulty.GetAll();
		selected = difficulties.FindIndex((Difficulty d) => d.id == Difficulty.ResolveId(NewGameData.Instance.difficultyId, NewGameData.Instance.hardMode));
		if (selected < 0)
		{
			selected = Mathf.Max(0, difficulties.FindIndex((Difficulty d) => d.id == "normal"));
		}
		page = ((difficulties.Count > 0) ? (selected / 3) : 0);
		BuildPlates();
		LayoutOptions();
		RefreshOptions();
		frame.continueButton.interactable = Selected != null;
		NewGameUIElements.RefreshContinueLook(frame, continueHovered);
		window.Show();
	}

	public static void Hide()
	{
		if (window != null && window.IsAlive)
		{
			window.Hide();
		}
	}

	public static void Close()
	{
		if (window != null && window.IsAlive)
		{
			window.Close();
			return;
		}
		window = null;
		frame = null;
		plates.Clear();
	}

	public static void CloseWithoutReturning()
	{
		startingGame = true;
		Close();
	}

	private static void OnWindowClosed()
	{
		window = null;
		frame = null;
		plates.Clear();
		if (!startingGame)
		{
			NewGamePerksUI.Open();
		}
		startingGame = false;
	}

	private static int PageCount()
	{
		return (difficulties.Count + 3 - 1) / 3;
	}

	private static void TurnPage(int direction)
	{
		int num = Mathf.Clamp(page + direction, 0, Mathf.Max(0, PageCount() - 1));
		if (num != page)
		{
			page = num;
			NewGameUIElements.PlayClick();
			BuildPlates();
		}
	}

	private static void BuildPlates()
	{
		NewGameUIElements.ClearChildren(frame.platesHost);
		plates.Clear();
		int num = page * 3;
		for (int i = 0; i < 3; i++)
		{
			int num2 = num + i;
			if (num2 >= difficulties.Count)
			{
				break;
			}
			BuildPlate(difficulties[num2], num2, (float)i * 306f);
		}
		RefreshPlates();
	}

	private static void BuildPlate(Difficulty difficulty, int index, float x)
	{
		RectTransform rectTransform = NewGameUIElements.NewRect(frame.platesHost, "plate_" + difficulty.id, x, 0f, 288f, 643f);
		Image image = rectTransform.gameObject.AddComponent<Image>();
		WildNetworkUI.ApplySprite(image, frame.frame2Square, NewGameStyle.DIFF_PLATE_BORDER_COLOR);
		image.raycastTarget = true;
		Sprite sprite = ((!string.IsNullOrEmpty(difficulty.iconKey) && CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetSprite(difficulty.iconKey) : null);
		bool num = sprite == null;
		float y = (num ? 16f : 201f);
		float y2 = (num ? 52f : 237f);
		float y3 = (num ? 90f : 175f);
		float num2 = (num ? 104f : 271f);
		if (!num)
		{
			NewGameUIElements.NewImage(rectTransform, "image", 60f, 10f, 164f, 164f, sprite, Color.white).preserveAspect = true;
		}
		NewGameUIElements.NewImage(rectTransform, "rule", 16f, y3, 256f, 2f, null, NewGameStyle.SEPARATOR);
		TextMeshProUGUI textMeshProUGUI = NewGameUIElements.NewText(frame, rectTransform, "title", 16f, y, 256f, 38f, FontLoader.FontRole.Noto_Sans_Normal, 28f, NewGameStyle.DIFF_TITLE, TextAlignmentOptions.Center, wrap: false);
		textMeshProUGUI.fontStyle = FontStyles.UpperCase;
		textMeshProUGUI.text = difficulty.GetTitle();
		WildNetworkUI.AutoSize(textMeshProUGUI, 12f);
		TextMeshProUGUI textMeshProUGUI2 = NewGameUIElements.NewText(frame, rectTransform, "subtitle", 16f, y2, 256f, 22f, FontLoader.FontRole.Noto_Sans, 16f, NewGameStyle.DIFF_SUBTITLE, TextAlignmentOptions.Center, wrap: false);
		textMeshProUGUI2.text = difficulty.GetSubtitle();
		WildNetworkUI.AutoSize(textMeshProUGUI2, 12f);
		float num3 = 643f - num2 - 16f;
		RectTransform rectTransform2 = NewGameUIElements.NewRect(rectTransform, "bullets", 18f, num2, 248f, num3);
		Image image2 = rectTransform2.gameObject.AddComponent<Image>();
		image2.color = Color.clear;
		image2.raycastTarget = true;
		rectTransform2.gameObject.AddComponent<RectMask2D>();
		RectTransform rectTransform3 = NewGameUIElements.NewRect(rectTransform2, "content", 0f, 0f, 248f, num3);
		rectTransform3.anchorMin = new Vector2(0f, 1f);
		rectTransform3.anchorMax = new Vector2(1f, 1f);
		rectTransform3.pivot = new Vector2(0f, 1f);
		rectTransform3.anchoredPosition = Vector2.zero;
		rectTransform3.sizeDelta = new Vector2(0f, num3);
		VerticalLayoutGroup verticalLayoutGroup = rectTransform3.gameObject.AddComponent<VerticalLayoutGroup>();
		verticalLayoutGroup.spacing = 2f;
		verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
		verticalLayoutGroup.childControlWidth = true;
		verticalLayoutGroup.childControlHeight = true;
		verticalLayoutGroup.childForceExpandWidth = true;
		verticalLayoutGroup.childForceExpandHeight = false;
		rectTransform3.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		foreach (string effectLine in Difficulty.GetEffectLines(difficulty))
		{
			BuildBulletRow(rectTransform3, effectLine);
		}
		ScrollRect scrollRect = rectTransform2.gameObject.AddComponent<ScrollRect>();
		scrollRect.content = rectTransform3;
		scrollRect.viewport = rectTransform2;
		scrollRect.horizontal = false;
		scrollRect.vertical = true;
		scrollRect.movementType = ScrollRect.MovementType.Clamped;
		scrollRect.scrollSensitivity = 30f;
		PlateView view = new PlateView
		{
			index = index,
			border = image
		};
		plates.Add(view);
		WildNetworkSlot wildNetworkSlot = rectTransform.gameObject.AddComponent<WildNetworkSlot>();
		wildNetworkSlot.id = index.ToString();
		wildNetworkSlot.onClick = OnPlateClicked;
		wildNetworkSlot.onHover = delegate(string _, bool entering)
		{
			view.hovered = entering;
			RefreshPlateBorder(view);
		};
	}

	private static void BuildBulletRow(RectTransform list, string line)
	{
		TextMeshProUGUI textMeshProUGUI = NewGameUIElements.NewText(frame, list, "row", 0f, 0f, 248f, 35f, FontLoader.FontRole.Noto_Sans_Normal, 16f, NewGameStyle.BULLET_TEXT, TextAlignmentOptions.TopLeft, wrap: true);
		textMeshProUGUI.margin = new Vector4(16f, 6f, 0f, 6f);
		textMeshProUGUI.text = line;
		float num = 16.4f;
		NewGameUIElements.NewImage(textMeshProUGUI.rectTransform, "bullet", 0f, num - 2f, 4f, 4f, null, NewGameStyle.BULLET);
	}

	private static void RefreshPlates()
	{
		foreach (PlateView plate in plates)
		{
			RefreshPlateBorder(plate);
		}
		NewGameUIElements.SetArrow(frame.prevButton, frame.prevImage, page > 0);
		NewGameUIElements.SetArrow(frame.nextButton, frame.nextImage, page + 1 < PageCount());
	}

	private static void RefreshPlateBorder(PlateView view)
	{
		if (!(view.border == null))
		{
			view.border.color = ((view.index == selected || view.hovered) ? NewGameStyle.DIFF_PLATE_BORDER_SELECTED : NewGameStyle.DIFF_PLATE_BORDER_COLOR);
		}
	}

	private static void OnPlateClicked(string id)
	{
		if (int.TryParse(id, out var result) && result >= 0 && result < difficulties.Count)
		{
			NewGameUIElements.PlayClick();
			selected = result;
			NewGameData.Instance.difficultyId = difficulties[result].id;
			NewGameData.Instance.hardMode = difficulties[result].IsAtLeast("hard");
			RefreshPlates();
		}
	}

	private static void WireCheckbox(NewGameCheckbox checkbox, UnityAction onClick, string tooltip)
	{
		checkbox.button.onClick.RemoveAllListeners();
		checkbox.button.onClick.AddListener(onClick);
		CustomUITooltipTrigger customUITooltipTrigger = checkbox.root.GetComponent<CustomUITooltipTrigger>();
		if (customUITooltipTrigger == null)
		{
			customUITooltipTrigger = checkbox.root.gameObject.AddComponent<CustomUITooltipTrigger>();
		}
		customUITooltipTrigger.tooltipText = tooltip;
		WildNetworkSlot wildNetworkSlot = checkbox.root.GetComponent<WildNetworkSlot>();
		if (wildNetworkSlot == null)
		{
			wildNetworkSlot = checkbox.root.gameObject.AddComponent<WildNetworkSlot>();
		}
		wildNetworkSlot.onHover = delegate(string _, bool entering)
		{
			if (!(checkbox.button == null) && !(checkbox.border == null) && checkbox.button.interactable)
			{
				checkbox.border.color = (entering ? NewGameStyle.DIFF_CHECKBOX_BORDER_HOVER : NewGameStyle.DIFF_CHECKBOX_BORDER);
			}
		};
	}

	private static void LayoutOptions()
	{
		float num = RowWidth(frame.skipIntro);
		float num2 = RowWidth(frame.endless);
		float num3 = num + 64f + num2;
		float num4 = Mathf.Round((1110f - num3) * 0.5f);
		PlaceRow(frame.skipIntro, num4, num);
		PlaceRow(frame.endless, num4 + num + 64f, num2);
	}

	private static float RowWidth(NewGameCheckbox checkbox)
	{
		checkbox.label.ForceMeshUpdate();
		return 44f + Mathf.Ceil(checkbox.label.preferredWidth);
	}

	private static void PlaceRow(NewGameCheckbox checkbox, float x, float width)
	{
		WildNetworkUI.SetRect(checkbox.root, x, 0f, width, 32f);
		float width2 = width - 32f - 12f;
		WildNetworkUI.SetRect(checkbox.label.rectTransform, 44f, 0f, width2, 32f);
	}

	private static void RefreshOptions()
	{
		SetCheckbox(frame.skipIntro, NewGameData.Instance.skipIntro, !skipIntroForced);
		SetCheckbox(frame.endless, NewGameData.Instance.endlessMode, interactable: true);
	}

	private static void SetCheckbox(NewGameCheckbox checkbox, bool on, bool interactable)
	{
		checkbox.mark.gameObject.SetActive(on);
		checkbox.button.interactable = interactable;
		checkbox.border.color = NewGameStyle.DIFF_CHECKBOX_BORDER;
		if (checkbox.group != null)
		{
			checkbox.group.alpha = (interactable ? 1f : 0.3f);
		}
	}

	private static void OnSkipIntroClicked()
	{
		if (skipIntroForced)
		{
			return;
		}
		NewGameUIElements.PlayClick();
		bool flag = !NewGameData.Instance.skipIntro;
		NewGameData.Instance.skipIntro = flag;
		RefreshOptions();
		MainMenuUIController menu = MainMenuUIController.Instance;
		if (flag && menu != null && menu.confirmPanelUIManager != null && SaveFiles.IndexLoad("totalRunCount", 0) == 0)
		{
			Hide();
			menu.darknessOverlay2.gameObject.SetActive(value: true);
			menu.confirmPanelUIManager.OpenPanel(LocHelper.GetLocalizedMainMenu("skip_intro_first_run_confirm"), delegate
			{
				menu.darknessOverlay2.gameObject.SetActive(value: false);
				Open();
			}, delegate
			{
				NewGameData.Instance.skipIntro = false;
				menu.darknessOverlay2.gameObject.SetActive(value: false);
				Open();
			});
		}
	}

	private static void OnEndlessClicked()
	{
		NewGameUIElements.PlayClick();
		NewGameData.Instance.endlessMode = !NewGameData.Instance.endlessMode;
		RefreshOptions();
	}

	private static void OnStartClicked()
	{
		Difficulty difficulty = Selected;
		if (difficulty != null && !(MainMenuUIController.Instance == null))
		{
			NewGameData.Instance.difficultyId = difficulty.id;
			NewGameData.Instance.hardMode = difficulty.IsAtLeast("hard");
			startingGame = true;
			Close();
			MainMenuUIController.Instance.OnPerkFinishedClick();
		}
	}
}

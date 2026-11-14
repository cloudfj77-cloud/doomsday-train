using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class NewGameSpecializationUI
{
	private class SlotView
	{
		public int index;

		public Image frameImage;
	}

	public const string WINDOW_ID = "new_game_specialization";

	private static CustomUIWindow window;

	private static NewGameSpecializationFrame frame;

	private static List<StartingScenario> scenarios = new List<StartingScenario>();

	private static readonly List<SlotView> slots = new List<SlotView>();

	private static int selected;

	private static int page;

	private static bool continueHovered;

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

	public static StartingScenario Selected
	{
		get
		{
			if (window == null || selected < 0 || selected >= scenarios.Count)
			{
				return null;
			}
			return scenarios[selected];
		}
	}

	public static void Open()
	{
		if (window != null && window.IsAlive)
		{
			window.Show();
			RefreshStrip();
			ShowScenario();
			return;
		}
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[NewGameSpecializationUI] No CustomUIManager in the scene, cannot open the new game screen");
			return;
		}
		if (CustomUIManager.Instance.newGameSpecializationPrefab == null)
		{
			Debug.LogError("[NewGameSpecializationUI] CustomUIManager.newGameSpecializationPrefab is not assigned. Run Build > Generate New Game Specialization UI Prefab in the menu scene.");
			return;
		}
		window = CustomUIManager.Instance.CreateWindow("new_game_specialization", null, "menu", CustomUIManager.Instance.newGameSpecializationPrefab).SetCloseOnEscape().OnClose(OnWindowClosed)
			.GetWindow();
		frame = ((window.root != null) ? window.root.GetComponent<NewGameSpecializationFrame>() : null);
		if (frame == null)
		{
			Debug.LogError("[NewGameSpecializationUI] newGameSpecializationPrefab has no NewGameSpecializationFrame on its root; regenerate it");
			window.Close();
			return;
		}
		frame.headerTitle.text = LocHelper.GetLocalizedMainMenu("new_specialization");
		frame.startWithLabel.text = LocHelper.GetLocalizedMainMenu("new_start_with");
		frame.continueLabel.text = LocHelper.GetLocalizedMainMenu("new_continue");
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
		frame.continueButton.onClick.AddListener(OnContinueClicked);
		WildNetworkSlot wildNetworkSlot = frame.continueButton.GetComponent<WildNetworkSlot>();
		if (wildNetworkSlot == null)
		{
			wildNetworkSlot = frame.continueButton.gameObject.AddComponent<WildNetworkSlot>();
		}
		wildNetworkSlot.id = "continue";
		wildNetworkSlot.onHover = delegate(string _, bool entering)
		{
			continueHovered = entering;
			NewGameUIElements.RefreshContinueLook(frame, continueHovered);
		};
		scenarios = StartingScenario.GetVisible();
		selected = IndexOfCurrentSelection();
		if (selected < 0)
		{
			selected = 0;
		}
		page = ((scenarios.Count > 0) ? (selected / 10) : 0);
		continueHovered = false;
		BuildStrip();
		ShowScenario();
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
		slots.Clear();
	}

	private static void OnWindowClosed()
	{
		window = null;
		frame = null;
		slots.Clear();
	}

	private static int IndexOfCurrentSelection()
	{
		if (NewGameData.Instance == null)
		{
			return -1;
		}
		string id = StartingScenario.ResolveId(NewGameData.Instance.startType, NewGameData.Instance.startScenarioId);
		if (string.IsNullOrEmpty(id))
		{
			return -1;
		}
		return scenarios.FindIndex((StartingScenario s) => s.id == id);
	}

	private static void TurnPage(int direction)
	{
		int num = PageCount();
		int num2 = Mathf.Clamp(page + direction, 0, Mathf.Max(0, num - 1));
		if (num2 != page)
		{
			page = num2;
			NewGameUIElements.PlayClick();
			BuildStrip();
		}
	}

	private static int PageCount()
	{
		return (scenarios.Count + 10 - 1) / 10;
	}

	private static void BuildStrip()
	{
		NewGameUIElements.ClearChildren(frame.stripSlots);
		slots.Clear();
		int num = page * 10;
		for (int i = 0; i < 10; i++)
		{
			int num2 = num + i;
			if (num2 >= scenarios.Count)
			{
				break;
			}
			StartingScenario startingScenario = scenarios[num2];
			RectTransform rectTransform = NewGameUIElements.NewRect(frame.stripSlots, "slot_" + startingScenario.id, (float)i * 76f, 0f, 64f, 64f);
			Image image = rectTransform.gameObject.AddComponent<Image>();
			WildNetworkUI.ApplySprite(image, frame.slotNormal, Color.white);
			image.raycastTarget = true;
			NewGameUIElements.FitPixelArt(NewGameUIElements.NewImage(rectTransform, "icon", 13f, 13f, 38f, 38f, ResolveIcon(startingScenario), Color.white), 0f, 0f, 64f, 38f);
			WildNetworkSlot wildNetworkSlot = rectTransform.gameObject.AddComponent<WildNetworkSlot>();
			wildNetworkSlot.id = num2.ToString();
			wildNetworkSlot.onClick = OnSlotClicked;
			slots.Add(new SlotView
			{
				index = num2,
				frameImage = image
			});
		}
		RefreshStrip();
	}

	private static void RefreshStrip()
	{
		foreach (SlotView slot in slots)
		{
			if (slot.frameImage != null)
			{
				slot.frameImage.sprite = ((slot.index == selected) ? frame.slotSelected : frame.slotNormal);
			}
		}
		NewGameUIElements.SetArrow(frame.prevButton, frame.prevImage, page > 0);
		NewGameUIElements.SetArrow(frame.nextButton, frame.nextImage, page + 1 < PageCount());
	}

	private static void OnSlotClicked(string id)
	{
		if (int.TryParse(id, out var result) && result >= 0 && result < scenarios.Count)
		{
			NewGameUIElements.PlayClick();
			selected = result;
			RefreshStrip();
			ShowScenario();
		}
	}

	private static Sprite ResolveIcon(StartingScenario scenario)
	{
		Sprite sprite = ((IconProvider.Instance != null) ? IconProvider.Instance.GetIcon(scenario.id) : null);
		if (sprite == null && !string.IsNullOrEmpty(scenario.iconKey))
		{
			sprite = frame.FindIcon(scenario.iconKey);
			if (sprite == null && CustomUIManager.Instance != null)
			{
				sprite = CustomUIManager.Instance.GetSprite(scenario.iconKey);
			}
		}
		if (sprite == null)
		{
			sprite = frame.FindIcon("icons_" + scenario.id);
		}
		if (sprite == null)
		{
			sprite = frame.FindIcon("icons_apprentice");
		}
		return sprite;
	}

	private static void ShowScenario()
	{
		StartingScenario startingScenario = Selected;
		NewGameUIElements.ClearChildren(frame.bulletsHost);
		if (startingScenario == null)
		{
			frame.title.text = "";
			frame.tag.text = "";
			frame.description.text = "";
			frame.warning.text = "";
			frame.continueButton.interactable = false;
			NewGameUIElements.RefreshContinueLook(frame, continueHovered);
			return;
		}
		frame.title.text = startingScenario.GetTitle();
		frame.tag.text = startingScenario.GetTag();
		frame.tag.color = NewGameStyle.TagColor(startingScenario.tagTone);
		frame.description.text = startingScenario.GetDescription();
		float num = 0f;
		foreach (string item in startingScenario.GetStartWith())
		{
			BuildBulletRow(item, num);
			num += 39f;
		}
		string text = "";
		Color color = NewGameStyle.WARNING;
		if (!startingScenario.IsAvailableInBuild())
		{
			text = LocHelper.GetLocalizedMainMenu("main_demo_notice");
			color = NewGameStyle.WARNING_DANGER;
		}
		else if (NewGameData.Instance != null && NewGameData.Instance.isShowcase && startingScenario.startType != NewGameData.StartType.APPRENTICE)
		{
			text = LocHelper.GetLocalizedNewGame("wip");
		}
		else if (startingScenario.skipIntro)
		{
			text = LocHelper.GetLocalizedMainMenu("new_intro_unavailable");
		}
		frame.warning.text = text;
		frame.warning.color = color;
		frame.continueButton.interactable = startingScenario.CanStart();
		NewGameUIElements.RefreshContinueLook(frame, continueHovered);
	}

	private static void BuildBulletRow(string line, float y)
	{
		RectTransform parent = NewGameUIElements.NewRect(frame.bulletsHost, "row", 0f, y, 902f, 35f);
		NewGameUIElements.NewImage(parent, "bullet", 0f, 15f, 4f, 4f, null, NewGameStyle.BULLET);
		TextMeshProUGUI textMeshProUGUI = NewGameUIElements.NewText(frame, parent, "text", 16f, 0f, 886f, 35f, FontLoader.FontRole.Noto_Sans_Normal, 24f, NewGameStyle.BULLET_TEXT, TextAlignmentOptions.MidlineLeft, wrap: false);
		textMeshProUGUI.text = line;
		WildNetworkUI.AutoSize(textMeshProUGUI, 14f);
	}

	private static void OnContinueClicked()
	{
		StartingScenario startingScenario = Selected;
		if (startingScenario != null && startingScenario.CanStart() && !(MainMenuUIController.Instance == null))
		{
			MainMenuUIController.Instance.OnScenarioContinue(startingScenario);
		}
	}
}

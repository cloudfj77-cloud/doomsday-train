using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class NewGamePerksUI
{
	private class SlotView
	{
		public StartingPerk perk;

		public bool inSelectedRow;

		public Image frameImage;

		public Image icon;

		public Image lockImage;

		public CanvasGroup group;

		public bool hovered;
	}

	public const string WINDOW_ID = "new_game_perks";

	public const int MAX_PERK_POINT = 3;

	public const int BASE_PERK_COUNT = 4;

	private static CustomUIWindow window;

	private static NewGamePerksFrame frame;

	private static readonly List<StartingPerk> sorted = new List<StartingPerk>();

	private static readonly List<SlotView> views = new List<SlotView>();

	private static int page;

	private static int selectedPage;

	private static bool firstRun;

	private static bool startingGame;

	private static bool continueHovered;

	private static NewGamePerkTooltip tooltip;

	private static readonly char[] labelTrim = new char[3] { ' ', ':', '：' };

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

	private static List<StartingPerk> SelectedPerks => NewGameData.Instance.startingPerks;

	public static void Open()
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[NewGamePerksUI] No CustomUIManager in the scene, cannot open the perk screen");
			return;
		}
		if (CustomUIManager.Instance.newGamePerksPrefab == null)
		{
			Debug.LogError("[NewGamePerksUI] CustomUIManager.newGamePerksPrefab is not assigned. Run Build > Generate New Game Perks UI Prefab in the menu scene.");
			return;
		}
		window = CustomUIManager.Instance.CreateWindow("new_game_perks", null, "menu", CustomUIManager.Instance.newGamePerksPrefab).SetCloseOnEscape().OnClose(OnWindowClosed)
			.GetWindow();
		frame = ((window.root != null) ? window.root.GetComponent<NewGamePerksFrame>() : null);
		if (frame == null)
		{
			Debug.LogError("[NewGamePerksUI] newGamePerksPrefab has no NewGamePerksFrame on its root; regenerate it");
			window.Close();
			return;
		}
		startingGame = false;
		continueHovered = false;
		page = 0;
		selectedPage = 0;
		firstRun = SaveFiles.IndexLoad("totalRunCount", 0) == 0;
		frame.headerTitle.text = LocHelper.GetLocalizedMainMenu("main_starting_perk");
		frame.availableLabel.text = TrimLabel(LocHelper.GetLocalizedMainMenu("main_available_perk"));
		frame.selectedLabel.text = TrimLabel(LocHelper.GetLocalizedMainMenu("main_selected_perk"));
		frame.continueLabel.text = LocHelper.GetLocalizedMainMenu("new_continue");
		if (firstRun)
		{
			frame.instruction.text = LocHelper.GetLocalizedMainMenu("main_perk_blocker");
			frame.instruction.color = NewGameStyle.WARNING_DANGER;
			NewGameData.Instance.startingPerks.Clear();
		}
		else
		{
			frame.instruction.text = LocHelper.GetLocalizedMainMenu("new_perk_instruction");
			frame.instruction.color = NewGameStyle.DESCRIPTION;
		}
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
		frame.selectedPrevButton.onClick.RemoveAllListeners();
		frame.selectedPrevButton.onClick.AddListener(delegate
		{
			TurnSelectedPage(-1);
		});
		frame.selectedNextButton.onClick.RemoveAllListeners();
		frame.selectedNextButton.onClick.AddListener(delegate
		{
			TurnSelectedPage(1);
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
		tooltip = NewGamePerkTooltip.Create(frame, window.root.transform, frame.perkTooltipLock);
		if (StartingPerkList.Perks == null)
		{
			StartingPerkList.InitStartingPerk();
		}
		sorted.Clear();
		sorted.AddRange(StartingPerkList.Perks);
		sorted.Sort(delegate(StartingPerk a, StartingPerk b)
		{
			int num = b.cost.CompareTo(a.cost);
			return (num == 0) ? string.CompareOrdinal(a.id, b.id) : num;
		});
		Rebuild();
		window.Show();
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
		views.Clear();
	}

	public static void CloseWithoutReturning()
	{
		startingGame = true;
		Close();
	}

	private static void OnWindowClosed()
	{
		HideTooltip();
		tooltip = null;
		window = null;
		frame = null;
		views.Clear();
		if (!startingGame)
		{
			NewGameSpecializationUI.Open();
		}
		startingGame = false;
	}

	private static string TrimLabel(string label)
	{
		return (label ?? "").TrimEnd(labelTrim);
	}

	private static bool IsSelected(string id)
	{
		foreach (StartingPerk selectedPerk in SelectedPerks)
		{
			if (selectedPerk.id == id)
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsLocked(StartingPerk perk)
	{
		return LockedBy(perk) != null;
	}

	private static StartingPerk LockedBy(StartingPerk perk)
	{
		foreach (StartingPerk selectedPerk in SelectedPerks)
		{
			if (!(selectedPerk.id == perk.id))
			{
				if (selectedPerk.incompatiblePerks != null && selectedPerk.incompatiblePerks.Contains(perk.id))
				{
					return selectedPerk;
				}
				if (perk.incompatiblePerks != null && perk.incompatiblePerks.Contains(selectedPerk.id))
				{
					return selectedPerk;
				}
			}
		}
		return null;
	}

	private static int UsedPoints()
	{
		int num = 0;
		foreach (StartingPerk selectedPerk in SelectedPerks)
		{
			num += selectedPerk.cost;
		}
		return num;
	}

	private static int MaxPerkCount()
	{
		int num = 4;
		foreach (StartingPerk selectedPerk in SelectedPerks)
		{
			num += selectedPerk.maxSlot;
		}
		return num;
	}

	private static bool CanStart()
	{
		if (UsedPoints() <= 3)
		{
			return SelectedPerks.Count <= MaxPerkCount();
		}
		return false;
	}

	private static void OnSlotClicked(string id)
	{
		if (firstRun)
		{
			return;
		}
		SlotView slotView = views.Find((SlotView v) => v.perk.id == id && v.hovered);
		if (slotView == null)
		{
			slotView = views.Find((SlotView v) => v.perk.id == id);
		}
		if (slotView == null)
		{
			return;
		}
		if (slotView.inSelectedRow || IsSelected(id))
		{
			if (!slotView.inSelectedRow)
			{
				return;
			}
			SelectedPerks.RemoveAll((StartingPerk p) => p.id == id);
		}
		else
		{
			if (IsLocked(slotView.perk))
			{
				return;
			}
			SelectedPerks.Add(slotView.perk);
		}
		NewGameUIElements.PlayClick();
		HideTooltip();
		Rebuild();
	}

	private static void TurnPage(int direction)
	{
		int num = PageCount();
		int num2 = Mathf.Clamp(page + direction, 0, Mathf.Max(0, num - 1));
		if (num2 != page)
		{
			page = num2;
			NewGameUIElements.PlayClick();
			Rebuild();
		}
	}

	private static int PageCount()
	{
		return (sorted.Count + 36 - 1) / 36;
	}

	private static int SelectedPageCount()
	{
		return (SelectedPerks.Count + 10 - 1) / 10;
	}

	private static void TurnSelectedPage(int direction)
	{
		int num = Mathf.Clamp(selectedPage + direction, 0, Mathf.Max(0, SelectedPageCount() - 1));
		if (num != selectedPage)
		{
			selectedPage = num;
			NewGameUIElements.PlayClick();
			Rebuild();
		}
	}

	private static void Rebuild()
	{
		NewGameUIElements.ClearChildren(frame.availableSlots);
		NewGameUIElements.ClearChildren(frame.selectedSlots);
		views.Clear();
		int num = page * 36;
		for (int i = 0; i < 36; i++)
		{
			int num2 = num + i;
			if (num2 >= sorted.Count)
			{
				break;
			}
			int num3 = i % 12;
			int num4 = i / 12;
			BuildSlot(frame.availableSlots, sorted[num2], (float)num3 * 76f, (float)num4 * 80f, inSelectedRow: false);
		}
		NewGameUIElements.SetArrow(frame.prevButton, frame.prevImage, page > 0);
		NewGameUIElements.SetArrow(frame.nextButton, frame.nextImage, page + 1 < PageCount());
		int count = SelectedPerks.Count;
		int num5 = 10;
		selectedPage = Mathf.Clamp(selectedPage, 0, Mathf.Max(0, SelectedPageCount() - 1));
		int num6 = selectedPage * num5;
		int num7 = Mathf.Min(num5, count - num6);
		float num8 = (float)num7 * 64f + (float)Mathf.Max(0, num7 - 1) * 12f;
		float num9 = (1110f - num8) * 0.5f;
		for (int j = 0; j < num7; j++)
		{
			BuildSlot(frame.selectedSlots, SelectedPerks[num6 + j], num9 + (float)j * 76f, 0f, inSelectedRow: true);
		}
		bool flag = count > num5;
		frame.selectedPrevButton.gameObject.SetActive(flag);
		frame.selectedNextButton.gameObject.SetActive(flag);
		if (flag)
		{
			NewGameUIElements.SetArrow(frame.selectedPrevButton, frame.selectedPrevImage, selectedPage > 0);
			NewGameUIElements.SetArrow(frame.selectedNextButton, frame.selectedNextImage, selectedPage + 1 < SelectedPageCount());
		}
		RefreshCounters();
		frame.continueButton.interactable = CanStart();
		NewGameUIElements.RefreshContinueLook(frame, continueHovered);
	}

	private static void BuildSlot(RectTransform host, StartingPerk perk, float x, float y, bool inSelectedRow)
	{
		RectTransform rectTransform = NewGameUIElements.NewRect(host, "perk_" + perk.id, x, y, 64f, 72f);
		Image image = rectTransform.gameObject.AddComponent<Image>();
		WildNetworkUI.ApplySprite(image, frame.perkSlotNormal, Color.white);
		image.raycastTarget = true;
		bool flag = !inSelectedRow && IsSelected(perk.id);
		bool flag2 = !inSelectedRow && !flag && IsLocked(perk);
		Image image2 = NewGameUIElements.NewImage(rectTransform, "icon", 0f, 0f, 64f, 64f, ResolveIcon(perk), flag2 ? NewGameStyle.PERK_LOCKED : NewGameStyle.PerkColor(perk.type));
		NewGameUIElements.FitPixelArt(image2, 0f, 0f, 64f, 64f);
		Image image3 = NewGameUIElements.NewImage(rectTransform, "lock", 24f, 23f, 16f, 18f, frame.perkSlotLock, Color.white);
		image3.gameObject.SetActive(flag2);
		CanvasGroup canvasGroup = rectTransform.gameObject.AddComponent<CanvasGroup>();
		canvasGroup.alpha = ((flag || (firstRun && !inSelectedRow)) ? 0.3f : 1f);
		canvasGroup.blocksRaycasts = true;
		SlotView view = new SlotView
		{
			perk = perk,
			inSelectedRow = inSelectedRow,
			frameImage = image,
			icon = image2,
			lockImage = image3,
			group = canvasGroup
		};
		views.Add(view);
		WildNetworkSlot wildNetworkSlot = rectTransform.gameObject.AddComponent<WildNetworkSlot>();
		wildNetworkSlot.id = perk.id;
		wildNetworkSlot.onClick = OnSlotClicked;
		wildNetworkSlot.onHover = delegate(string _, bool entering)
		{
			OnSlotHover(view, entering);
		};
	}

	private static void OnSlotHover(SlotView view, bool entering)
	{
		if (!(frame == null) && !(NewGameData.Instance == null))
		{
			view.hovered = entering;
			bool flag = !firstRun && (view.inSelectedRow || (!IsSelected(view.perk.id) && !IsLocked(view.perk)));
			if (view.frameImage != null)
			{
				view.frameImage.sprite = ((entering && flag) ? frame.perkSlotHover : frame.perkSlotNormal);
			}
			if (entering)
			{
				ShowTooltip(view.perk, view.inSelectedRow ? null : LockedBy(view.perk));
			}
			else
			{
				HideTooltip();
			}
		}
	}

	private static Sprite ResolveIcon(StartingPerk perk)
	{
		return StartingPerkIconLoader.GetIcon(perk);
	}

	private static void RefreshCounters()
	{
		int num = UsedPoints();
		int num2 = 3 - num;
		int count = SelectedPerks.Count;
		int num3 = MaxPerkCount();
		frame.pointsCounter.text = LocHelper.GetLocalizedMainMenu("new_perk_points_available", Value(num2.ToString(), num2 < 0));
		frame.perksCounter.text = LocHelper.GetLocalizedMainMenu("new_perk_count", Value(count + "/" + num3, count > num3));
	}

	private static string Value(string text, bool over)
	{
		Color color = (over ? NewGameStyle.PERKS_COUNTER_OVER : NewGameStyle.PERKS_COUNTER_VALUE);
		return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + text + "</color>";
	}

	private static void ShowTooltip(StartingPerk perk, StartingPerk lockedBy)
	{
		if (!(tooltip == null))
		{
			tooltip.Show(perk, lockedBy);
		}
	}

	private static void HideTooltip()
	{
		if (tooltip != null)
		{
			tooltip.Hide();
		}
	}

	private static void OnContinueClicked()
	{
		if (CanStart() && !(MainMenuUIController.Instance == null))
		{
			startingGame = true;
			HideTooltip();
			Close();
			MainMenuUIController.Instance.OnPerksContinue();
		}
	}
}

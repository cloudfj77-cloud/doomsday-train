using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class LoadGameUI
{
	private class Plate
	{
		public int index = -1;

		public int slotId = -1;

		public SavePreviewData data;

		public RectTransform root;

		public Image fill;

		public Image icon;

		public TextMeshProUGUI title;

		public TextMeshProUGUI info;

		public TextMeshProUGUI date;

		public TextMeshProUGUI version;

		public TMP_InputField rename;

		public RectTransform expanded;
	}

	public const string WINDOW_ID = "load_game";

	private const int NAME_LIMIT = 40;

	private const string DATE_FORMAT = "dd/MM/yyyy - HH:mm:ss";

	private const float PLATE_PITCH = 104f;

	private const float BIND_MARGIN = 96f;

	private static CustomUIWindow window;

	private static LoadGameFrame frame;

	private static readonly List<int> entries = new List<int>();

	private static readonly Dictionary<int, Plate> bound = new Dictionary<int, Plate>();

	private static readonly Stack<Plate> free = new Stack<Plate>();

	private static int expandedIndex = -1;

	private static List<SaveHistoryEntry> expandedHistory;

	private static float expandedExtra;

	private static Action dialogConfirmAction;

	private static Plate renamingPlate;

	private const int FULL_RELEASE_SAVE_VERSION = 100;

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

	public static void Open()
	{
		if (window != null && window.IsAlive)
		{
			SaveFiles.RepairDamagedRuns();
			window.Show();
			RebuildList();
			ExpandMostRecent();
			ShowRepairNotice(null);
			return;
		}
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[LoadGameUI] No CustomUIManager in the scene, cannot open the load game screen");
			return;
		}
		if (CustomUIManager.Instance.loadGamePrefab == null)
		{
			Debug.LogError("[LoadGameUI] CustomUIManager.loadGamePrefab is not assigned. Run Build > Generate Load Game UI Prefab in the menu scene.");
			return;
		}
		window = CustomUIManager.Instance.CreateWindow("load_game", null, "menu", CustomUIManager.Instance.loadGamePrefab).SetCloseOnEscape().OnClose(OnWindowClosed)
			.GetWindow();
		frame = ((window.root != null) ? window.root.GetComponent<LoadGameFrame>() : null);
		if (frame == null)
		{
			Debug.LogError("[LoadGameUI] loadGamePrefab has no LoadGameFrame on its root; regenerate it");
			window.Close();
			return;
		}
		frame.headerTitle.text = LocHelper.GetLocalizedMainMenu("load_game");
		frame.backButton.onClick.AddListener(NewGameUIElements.PlayClick);
		frame.discoverButton.onClick.RemoveAllListeners();
		frame.discoverButton.onClick.AddListener(OnDiscoverClicked);
		NewGameUIElements.SetTooltip(frame.discoverButton.gameObject, LocHelper.GetLocalizedUIOrDefault("ui_tooltip_discover_saves", "Scan the save folder for save files added by hand"));
		frame.dialogConfirm.WireHover("confirm");
		frame.dialogCancel.WireHover("cancel");
		frame.dialogConfirm.button.onClick.RemoveAllListeners();
		frame.dialogConfirm.button.onClick.AddListener(OnDialogConfirm);
		frame.dialogCancel.button.onClick.RemoveAllListeners();
		frame.dialogCancel.button.onClick.AddListener(OnDialogCancel);
		frame.dialogConfirm.label.text = LocHelper.GetLocalizedMainMenu("confirm");
		frame.dialogCancel.label.text = LocHelper.GetLocalizedOrDefault("MainMenu", "load_cancel", "Cancel");
		HideDialog();
		LayoutGroup component = frame.content.GetComponent<LayoutGroup>();
		if (component != null)
		{
			component.enabled = false;
		}
		ContentSizeFitter component2 = frame.content.GetComponent<ContentSizeFitter>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
		frame.scroll.onValueChanged.RemoveAllListeners();
		frame.scroll.onValueChanged.AddListener(delegate
		{
			RefreshVisible();
		});
		SaveFiles.RepairDamagedRuns();
		entries.Clear();
		entries.AddRange(SaveUIManager.GetOrderedSlotIds());
		ExpandMostRecent();
		RebuildList();
		window.Show();
		ScrollToTop();
		ShowRepairNotice(null);
	}

	private static string BuildRepairNotice()
	{
		List<SaveFiles.RepairResult> list = SaveFiles.TakeRecentRepairs();
		if (list.Count == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (SaveFiles.RepairResult item in list)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append("\n\n");
			}
			string text = (string.IsNullOrEmpty(item.runName) ? ("#" + item.slotId) : item.runName);
			if (item.promotedDay >= 0)
			{
				stringBuilder.Append(LocHelper.GetLocalizedOrDefault("MainMenu", "load_damaged_repaired", "The latest save of {0} was damaged (the computer shut down while it was being written) and has been moved aside. The run continues from day {1}.", text, item.promotedDay));
			}
			else
			{
				stringBuilder.Append(LocHelper.GetLocalizedOrDefault("MainMenu", "load_damaged_lost", "The save of {0} was damaged (the computer shut down while it was being written) and no earlier day is left. Its files have been moved aside in the save folder, not deleted.", text));
			}
		}
		return stringBuilder.ToString();
	}

	private static void ShowRepairNotice(string before)
	{
		string text = BuildRepairNotice();
		if (text != null || before != null)
		{
			string body = ((before == null) ? text : ((text == null) ? before : (before + "\n\n" + text)));
			ShowDialog(LocHelper.GetLocalizedMainMenu("load_game"), body, null, showCancel: false);
		}
	}

	private static void ExpandMostRecent()
	{
		List<SaveHistoryEntry> list = ((entries.Count > 0) ? SaveFiles.LoadHistory(entries[0]) : null);
		if (list != null && list.Count > 0)
		{
			SetExpandedState(0, list);
		}
		else
		{
			SetExpandedState(-1, null);
		}
	}

	public static void Close()
	{
		if (window != null && window.IsAlive)
		{
			window.Close();
		}
		else
		{
			OnWindowClosed();
		}
	}

	private static void OnWindowClosed()
	{
		window = null;
		frame = null;
		entries.Clear();
		bound.Clear();
		free.Clear();
		expandedIndex = -1;
		expandedHistory = null;
		dialogConfirmAction = null;
		renamingPlate = null;
	}

	private static void ScrollToTop()
	{
		if (!(frame == null) && !(frame.scroll == null))
		{
			frame.scroll.verticalNormalizedPosition = 1f;
			RefreshVisible();
		}
	}

	private static void RebuildList()
	{
		if (frame == null)
		{
			return;
		}
		int num = ((expandedIndex >= 0 && expandedIndex < entries.Count) ? entries[expandedIndex] : (-1));
		EndRename(cancel: true);
		entries.Clear();
		entries.AddRange(SaveUIManager.GetOrderedSlotIds());
		expandedIndex = ((num >= 0) ? entries.IndexOf(num) : (-1));
		if (expandedIndex < 0)
		{
			expandedHistory = null;
		}
		foreach (Plate value in bound.Values)
		{
			Unbind(value);
		}
		bound.Clear();
		UpdateContentHeight();
		RefreshVisible();
	}

	private static float PlateTop(int index)
	{
		float num = (float)index * 104f;
		if (expandedIndex >= 0 && index > expandedIndex)
		{
			num += expandedExtra;
		}
		return num;
	}

	private static float PlateBottom(int index)
	{
		return PlateTop(index) + 96f + ((index == expandedIndex) ? expandedExtra : 0f);
	}

	private static void UpdateContentHeight()
	{
		int count = entries.Count;
		float a = ((count > 0) ? PlateBottom(count - 1) : 0f);
		frame.content.sizeDelta = new Vector2(frame.content.sizeDelta.x, Mathf.Max(a, 797f));
	}

	private static void RefreshVisible()
	{
		if (frame == null || frame.content == null)
		{
			return;
		}
		float y = frame.content.anchoredPosition.y;
		float num = ((frame.scroll.viewport != null) ? frame.scroll.viewport.rect.height : 797f);
		if (num <= 0f)
		{
			num = 797f;
		}
		float num2 = y - 96f;
		float num3 = y + num + 96f;
		int num4 = -1;
		int num5 = -1;
		for (int i = 0; i < entries.Count; i++)
		{
			if (!(PlateBottom(i) <= num2))
			{
				if (PlateTop(i) >= num3)
				{
					break;
				}
				if (num4 < 0)
				{
					num4 = i;
				}
				num5 = i;
			}
		}
		List<int> list = null;
		foreach (KeyValuePair<int, Plate> item in bound)
		{
			if (item.Key < num4 || item.Key > num5 || num4 < 0)
			{
				if (list == null)
				{
					list = new List<int>();
				}
				list.Add(item.Key);
			}
		}
		if (list != null)
		{
			foreach (int item2 in list)
			{
				Unbind(bound[item2]);
				bound.Remove(item2);
			}
		}
		if (num4 < 0)
		{
			return;
		}
		for (int j = num4; j <= num5; j++)
		{
			if (!bound.ContainsKey(j))
			{
				Plate plate = ((free.Count > 0) ? free.Pop() : BuildPlateView());
				Bind(plate, j);
				bound[j] = plate;
			}
		}
	}

	private static Plate BuildPlateView()
	{
		Plate plate = new Plate();
		plate.root = NewGameUIElements.NewRect(frame.content, "Plate", 0f, 0f, 900f, 96f);
		RectTransform rectTransform = NewGameUIElements.NewRect(plate.root, "Body", 0f, 0f, 900f, 96f);
		plate.fill = rectTransform.gameObject.AddComponent<Image>();
		plate.fill.color = Color.clear;
		plate.fill.raycastTarget = true;
		WildNetworkSlot wildNetworkSlot = rectTransform.gameObject.AddComponent<WildNetworkSlot>();
		wildNetworkSlot.id = "plate";
		wildNetworkSlot.onClick = delegate
		{
			ToggleExpanded(plate);
		};
		NewGameUIElements.NewImage(rectTransform, "Border", 0f, 0f, 900f, 96f, frame.frame2Square, NewGameStyle.LOAD_PLATE_BORDER_COLOR);
		NewGameUIElements.NewImage(rectTransform, "IconPlate", 2f, 2f, 64f, 92f, null, NewGameStyle.LOAD_ICON_PLATE_COLOR);
		NewGameUIElements.NewImage(rectTransform, "Ring", 12f, 27f, 42f, 42f, frame.iconRing, Color.white);
		plate.icon = NewGameUIElements.NewImage(rectTransform, "Icon", 12f, 27f, 38f, 38f, null, Color.white);
		plate.title = NewGameUIElements.NewText(frame, rectTransform, "Title", 82f, 13f, 308f, 38f, FontLoader.FontRole.Lubrifont, 32f, NewGameStyle.LOAD_TITLE_COLOR, TextAlignmentOptions.MidlineLeft, wrap: false);
		WildNetworkUI.AutoSize(plate.title, 18f);
		plate.title.raycastTarget = true;
		WildNetworkSlot wildNetworkSlot2 = plate.title.gameObject.AddComponent<WildNetworkSlot>();
		wildNetworkSlot2.id = "rename";
		wildNetworkSlot2.onClick = delegate
		{
			BeginRename(plate);
		};
		wildNetworkSlot2.onHover = delegate(string _, bool entering)
		{
			plate.title.color = (entering ? NewGameStyle.BUTTON_HOVER : NewGameStyle.LOAD_TITLE_COLOR);
		};
		plate.info = NewGameUIElements.NewText(frame, rectTransform, "Info", 82f, 56f, 308f, 27f, FontLoader.FontRole.Noto_Sans_Normal, 20f, NewGameStyle.LOAD_INFO_COLOR, TextAlignmentOptions.MidlineLeft, wrap: false);
		WildNetworkUI.AutoSize(plate.info, 12f);
		plate.date = NewGameUIElements.NewText(frame, rectTransform, "Date", 310f, 21f, 260f, 25f, FontLoader.FontRole.Noto_Sans_Normal, 18f, NewGameStyle.LOAD_DATE_COLOR, TextAlignmentOptions.MidlineRight, wrap: false);
		plate.version = NewGameUIElements.NewText(frame, rectTransform, "Version", 310f, 50f, 260f, 25f, FontLoader.FontRole.Noto_Sans_Normal, 18f, NewGameStyle.LOAD_VERSION_COLOR, TextAlignmentOptions.MidlineRight, wrap: false);
		WildNetworkUI.AutoSize(plate.version, 12f);
		NewGameUIElements.NewImage(rectTransform, "Separator", 586f, 16f, 2f, 64f, null, NewGameStyle.SEPARATOR);
		NewGameCommonButton.Build(frame, rectTransform, "LoadLatest", 604f, 27f, 192f, 42f, LocHelper.GetLocalizedOrDefault("MainMenu", "load_latest", "Load Latest"), delegate
		{
			OnLoadClicked(plate.slotId, null);
		});
		float size = 32f;
		NewGameUIElements.NewIconButton(rectTransform, "Duplicate", 810f, 32f, size, frame.duplicateNormal, frame.duplicateHover, null, delegate
		{
			OnDuplicateClicked(plate.slotId);
		});
		NewGameUIElements.NewIconButton(rectTransform, "Delete", 854f, 32f, size, frame.deleteNormal, frame.deleteHover, null, delegate
		{
			OnDeleteClicked(plate.slotId);
		});
		return plate;
	}

	private static void Bind(Plate plate, int index)
	{
		plate.index = index;
		plate.slotId = entries[index];
		plate.data = SaveUIManager.GetPreview(plate.slotId);
		plate.root.gameObject.SetActive(value: true);
		WildNetworkUI.SetRect(plate.root, 0f, PlateTop(index), 900f, 96f);
		plate.fill.color = ((index == expandedIndex) ? NewGameStyle.LOAD_EXPANDED_FILL : Color.clear);
		SavePreviewData data = plate.data;
		if (data == null)
		{
			plate.title.text = "";
			plate.info.text = "";
			plate.date.text = "";
			plate.version.text = "";
			plate.icon.enabled = false;
			return;
		}
		try
		{
			plate.title.text = data.GetDisplayName();
			plate.info.text = DayAndMoney(data.dayCount, data.credit);
			plate.date.text = data.lastSaveDateTime.ToString("dd/MM/yyyy - HH:mm:ss");
			plate.version.text = LocHelper.GetLocalizedOrDefault("MainMenu", "load_version", "Version: {0}", data.version);
			Sprite sprite = ResolveIcon(data);
			plate.icon.sprite = sprite;
			plate.icon.enabled = sprite != null;
			if (sprite != null)
			{
				NewGameUIElements.FitPixelArt(plate.icon, 12f, 27f, 42f, 38f);
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"[LoadGameUI] Slot {plate.slotId} plate could not be fully filled: {arg}");
		}
		if (index == expandedIndex && expandedHistory != null)
		{
			BuildExpanded(plate, expandedHistory);
		}
	}

	private static void Unbind(Plate plate)
	{
		if (renamingPlate == plate)
		{
			EndRename(cancel: true);
		}
		if (plate.expanded != null)
		{
			UnityEngine.Object.Destroy(plate.expanded.gameObject);
		}
		plate.expanded = null;
		plate.index = -1;
		plate.slotId = -1;
		plate.data = null;
		plate.root.gameObject.SetActive(value: false);
		free.Push(plate);
	}

	private static Plate BoundPlate(int slotId)
	{
		foreach (Plate value in bound.Values)
		{
			if (value.slotId == slotId)
			{
				return value;
			}
		}
		return null;
	}

	private static string DayAndMoney(int dayCount, int credit)
	{
		return LocHelper.GetLocalizedMechanic("mech_day_label", dayCount) + " | " + LocHelper.GetLocalizedUI("network_ui_cost", credit);
	}

	private static Sprite ResolveIcon(SavePreviewData data)
	{
		Sprite sprite = ((IconProvider.Instance != null) ? IconProvider.Instance.GetIcon(data.startType, data.startScenarioId) : null);
		if (sprite == null && data.startType == NewGameData.StartType.MODDED)
		{
			StartingScenario startingScenario = StartingScenario.Get(data.startScenarioId);
			if (startingScenario != null && !string.IsNullOrEmpty(startingScenario.iconKey) && CustomUIManager.Instance != null)
			{
				sprite = CustomUIManager.Instance.GetSprite(startingScenario.iconKey);
			}
		}
		return sprite;
	}

	private static void ToggleExpanded(Plate plate)
	{
		if (renamingPlate != null || plate.index < 0)
		{
			return;
		}
		NewGameUIElements.PlayClick();
		int index = plate.index;
		if (expandedIndex == index)
		{
			SetExpanded(-1, null);
			return;
		}
		List<SaveHistoryEntry> list = SaveFiles.LoadHistory(plate.slotId);
		if (list.Count == 0)
		{
			SetExpanded(-1, null);
		}
		else
		{
			SetExpanded(index, list);
		}
	}

	private static void SetExpanded(int index, List<SaveHistoryEntry> history)
	{
		SetExpandedState(index, history);
		foreach (Plate value in bound.Values)
		{
			Unbind(value);
		}
		bound.Clear();
		UpdateContentHeight();
		RefreshVisible();
	}

	private static void SetExpandedState(int index, List<SaveHistoryEntry> history)
	{
		expandedIndex = index;
		expandedHistory = history;
		expandedExtra = ((history != null) ? (ExpandedBoxHeight(history.Count) - 2f) : 0f);
	}

	private static float ExpandedBoxHeight(int rowCount)
	{
		float num = (float)rowCount * 64f + (float)(rowCount - 1) * 10f;
		return 18f + num + 10f + 8f;
	}

	private static void BuildExpanded(Plate plate, List<SaveHistoryEntry> history)
	{
		int count = history.Count;
		float num = ExpandedBoxHeight(count);
		float height = num - 18f - 10f - 8f;
		RectTransform rectTransform = NewGameUIElements.NewRect(plate.root, "Expanded", 12f, 94f, 876f, num);
		Image image = rectTransform.gameObject.AddComponent<Image>();
		image.color = Color.clear;
		image.raycastTarget = true;
		NewGameUIElements.NewImage(rectTransform, "Border", 0f, 0f, 876f, num, frame.frame2Square, NewGameStyle.LOAD_PLATE_BORDER_COLOR);
		NewGameUIElements.NewImage(rectTransform, "BorderBottom", 0f, num - 8f, 876f, 8f, null, NewGameStyle.LOAD_PLATE_BORDER_COLOR);
		RectTransform parent = NewGameUIElements.NewRect(rectTransform, "Rows", 70f, 18f, 716f, height);
		for (int i = 0; i < count; i++)
		{
			SaveHistoryEntry saveHistoryEntry = history[i];
			float num2 = (float)i * 74f;
			RectTransform parent2 = NewGameUIElements.NewRect(parent, "Row", 0f, num2, 716f, 64f);
			TextMeshProUGUI textMeshProUGUI = NewGameUIElements.NewText(frame, parent2, "Info", 0f, 25f, 308f, 27f, FontLoader.FontRole.Noto_Sans_Normal, 20f, NewGameStyle.LOAD_INFO_COLOR, TextAlignmentOptions.MidlineLeft, wrap: false);
			WildNetworkUI.AutoSize(textMeshProUGUI, 12f);
			textMeshProUGUI.text = DayAndMoney(saveHistoryEntry.dayCount, saveHistoryEntry.credit);
			NewGameUIElements.NewText(frame, parent2, "Date", 228f, 25f, 260f, 25f, FontLoader.FontRole.Noto_Sans_Normal, 18f, NewGameStyle.LOAD_DATE_COLOR, TextAlignmentOptions.MidlineRight, wrap: false).text = saveHistoryEntry.saveDateTime.ToString("dd/MM/yyyy - HH:mm:ss");
			NewGameUIElements.NewImage(parent2, "Rule", 504f, 0f, 2f, 64f, null, NewGameStyle.SEPARATOR);
			string file = saveHistoryEntry.file;
			int slotId = plate.slotId;
			string version = saveHistoryEntry.version;
			NewGameCommonButton.Build(frame, parent2, "Load", 522f, 12f, 192f, 42f, LocHelper.GetLocalizedMainMenu("load_button"), delegate
			{
				OnLoadClicked(slotId, file, version);
			});
			if (i < count - 1)
			{
				NewGameUIElements.NewImage(parent, "RowSeparator", 0f, num2 + 64f + 4f, 716f, 2f, null, NewGameStyle.SEPARATOR);
			}
		}
		plate.expanded = rectTransform;
	}

	private static void OnLoadClicked(int slotId, string file, string version = null)
	{
		if (renamingPlate != null || slotId < 0)
		{
			return;
		}
		NewGameUIElements.PlayClick();
		SavePreviewData data = SaveUIManager.GetPreview(slotId);
		string text = file ?? SaveFiles.Slot(slotId);
		if (!File.Exists(SaveFiles.AbsolutePath(text)))
		{
			if (file == null)
			{
				SaveFiles.DeleteSlot(slotId);
			}
			else
			{
				SetExpanded(-1, null);
			}
			RebuildList();
			ShowDialog(LocHelper.GetLocalizedMainMenu("load_game"), LocHelper.GetLocalizedOrDefault("MainMenu", "load_save_missing", "Save file not found. The entry has been removed."), null, showCancel: false);
			return;
		}
		if (!SaveFiles.IsSaveFileIntact(SaveFiles.AbsolutePath(text), out var reason))
		{
			Debug.LogError("[LoadGameUI] " + text + " is damaged (" + reason + ") and will not be loaded");
			if (file == null)
			{
				SaveFiles.RepairDamagedRun(slotId, null);
			}
			else
			{
				SaveFiles.QuarantineFile(file);
			}
			SetExpanded(-1, null);
			RebuildList();
			ShowRepairNotice((file == null) ? null : LocHelper.GetLocalizedOrDefault("MainMenu", "load_save_damaged", "This save file is damaged (the computer shut down while it was being written) and cannot be loaded. It has been moved aside in the save folder."));
			return;
		}
		Action action = delegate
		{
			LoadSlot(slotId, file);
		};
		string currentLocale = LocHelper.GetCurrentLocaleCode();
		if (data != null && !string.IsNullOrEmpty(data.startLocale) && data.startLocale != currentLocale)
		{
			Action loadAfterLocale = action;
			action = delegate
			{
				ShowDialog(LocHelper.GetLocalizedMainMenu("load_game"), LocHelper.GetLocalizedMainMenu("load_locale_mismatch_warning", data.startLocale, currentLocale), loadAfterLocale, showCancel: true);
			};
		}
		if (IsDemoSaveVersion(version ?? data?.version))
		{
			ShowDialog(LocHelper.GetLocalizedMainMenu("load_game"), GetDemoSaveWarning(), action, showCancel: true);
		}
		else
		{
			action();
		}
	}

	private static bool IsDemoSaveVersion(string version)
	{
		if (string.IsNullOrEmpty(version))
		{
			return false;
		}
		Match match = Regex.Match(version, "\\d+");
		if (!match.Success || !int.TryParse(match.Value, out var result))
		{
			return false;
		}
		return result < 100;
	}

	private static string GetDemoSaveWarning()
	{
		return LocHelper.GetLocalizedOrDefault("MainMenu", "load_demo_save_warning", "You are about to load a save file from the demo. The save is compatible, but you will <color=#CC4849><b>miss the new characters' stories and other new content</b></color>, and the game <color=#CC4849><b>may be unstable</b></color>. We <color=#CC4849><b>strongly recommend starting a new game</b></color>. Load anyway?");
	}

	private static void LoadSlot(int slotId, string file)
	{
		NewGameData.Instance.saveSlot = slotId;
		NewGameData.Instance.saveFile = file;
		Close();
		MainMenuUIController.Instance.StartNewGameClick();
	}

	private static void OnDuplicateClicked(int slotId)
	{
		if (renamingPlate == null && slotId >= 0)
		{
			NewGameUIElements.PlayClick();
			SavePreviewData preview = SaveUIManager.GetPreview(slotId);
			string localizedOrDefault = LocHelper.GetLocalizedOrDefault("MainMenu", "load_copy_name", "{0} (copy)", (preview != null) ? preview.GetDisplayName() : "");
			if (SaveFiles.DuplicateSlot(slotId, localizedOrDefault) < 0)
			{
				ShowDialog(LocHelper.GetLocalizedMainMenu("load_game"), LocHelper.GetLocalizedOrDefault("MainMenu", "load_duplicate_failed", "The save could not be copied."), null, showCancel: false);
			}
			else
			{
				RebuildList();
			}
		}
	}

	private static void OnDeleteClicked(int slotId)
	{
		if (renamingPlate != null || slotId < 0)
		{
			return;
		}
		NewGameUIElements.PlayClick();
		ShowDialog(LocHelper.GetLocalizedOrDefault("MainMenu", "load_delete_title", "Delete Save"), LocHelper.GetLocalizedUI("ui_delete_save"), delegate
		{
			SaveFiles.DeleteSlot(slotId);
			if (expandedIndex >= 0 && expandedIndex < entries.Count && entries[expandedIndex] == slotId)
			{
				SetExpanded(-1, null);
			}
			RebuildList();
		}, showCancel: true);
	}

	private static void OnDiscoverClicked()
	{
		if (renamingPlate == null)
		{
			NewGameUIElements.PlayClick();
			List<int> list = SaveFiles.ScanAndRegisterOrphans();
			SaveFiles.RepairDamagedRuns();
			Debug.Log("[LoadGameUI] Discover registered " + list.Count + " save file(s)");
			RebuildList();
			ShowRepairNotice((list.Count == 0) ? LocHelper.GetLocalizedOrDefault("MainMenu", "load_discover_none", "No new save file was found in the save folder.") : LocHelper.GetLocalizedOrDefault("MainMenu", "load_discover_found", "{0} save file(s) added to the list.", list.Count));
		}
	}

	private static void BeginRename(Plate plate)
	{
		if (renamingPlate != null || plate.data == null || plate.index < 0)
		{
			return;
		}
		NewGameUIElements.PlayClick();
		if (plate.rename == null)
		{
			plate.rename = NewGameUIElements.NewInputField(frame, plate.title.transform.parent, "Rename", 82f, 13f, 308f, 38f, FontLoader.FontRole.Lubrifont, 32f, NewGameStyle.LOAD_TITLE_COLOR, 40);
			plate.rename.onEndEdit.AddListener(delegate
			{
				if (renamingPlate == plate)
				{
					EndRename(plate.rename.wasCanceled);
				}
			});
		}
		renamingPlate = plate;
		plate.title.gameObject.SetActive(value: false);
		plate.rename.gameObject.SetActive(value: true);
		plate.rename.text = plate.data.GetDisplayName();
		plate.rename.ActivateInputField();
		plate.rename.Select();
	}

	private static void EndRename(bool cancel)
	{
		Plate plate = renamingPlate;
		if (plate == null)
		{
			return;
		}
		renamingPlate = null;
		if (!cancel && plate.data != null && plate.slotId >= 0)
		{
			string text = plate.rename.text.Trim();
			string startDisplayName = NewGameData.GetStartDisplayName(plate.data.startType, plate.data.startScenarioId);
			if (text != plate.data.GetDisplayName())
			{
				SaveFiles.RenameSlot(plate.slotId, (text == startDisplayName) ? null : text);
				plate.data = SaveUIManager.GetPreview(plate.slotId) ?? plate.data;
				plate.title.text = plate.data.GetDisplayName();
			}
		}
		plate.rename.gameObject.SetActive(value: false);
		plate.title.gameObject.SetActive(value: true);
	}

	private static void ShowDialog(string title, string body, Action onConfirm, bool showCancel)
	{
		if (!(frame == null) && !(frame.dialogRoot == null))
		{
			dialogConfirmAction = onConfirm;
			frame.dialogTitle.text = title;
			frame.dialogText.text = body;
			frame.dialogCancel.button.gameObject.SetActive(showCancel);
			frame.dialogRoot.SetActive(value: true);
		}
	}

	private static void HideDialog()
	{
		dialogConfirmAction = null;
		if (frame != null && frame.dialogRoot != null)
		{
			frame.dialogRoot.SetActive(value: false);
		}
	}

	private static void OnDialogConfirm()
	{
		NewGameUIElements.PlayClick();
		Action action = dialogConfirmAction;
		HideDialog();
		action?.Invoke();
	}

	private static void OnDialogCancel()
	{
		NewGameUIElements.PlayClick();
		HideDialog();
	}
}

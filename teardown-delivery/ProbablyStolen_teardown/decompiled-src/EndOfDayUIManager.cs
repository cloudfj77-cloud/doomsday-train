using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class EndOfDayUIManager : MonoBehaviour
{
	public const string WINDOW_ID = "end_of_day_tablet";

	[Tooltip("UI3/Prefab/EndOfDayTablet.prefab, from Build > Generate Tablet UI Prefabs.")]
	public GameObject tabletPrefab;

	public GameObject UI;

	private CustomUIWindow window;

	private EndOfDayTabletFrame frame;

	private TabletButton endDayButton;

	private TabletUI.FooterRow fundsRow;

	private TabletUI.FooterRow totalRow;

	private readonly List<(StoreService service, TabletCheckbox checkbox)> rows = new List<(StoreService, TabletCheckbox)>();

	public static EndOfDayUIManager Instance { get; private set; }

	public bool IsOpen
	{
		get
		{
			if (UI != null)
			{
				return UI.activeSelf;
			}
			return false;
		}
	}

	public bool HasToggleableService
	{
		get
		{
			foreach (var row in rows)
			{
				if (!row.service.isMandatory && !row.service.IsOnCooldown())
				{
					return true;
				}
			}
			return false;
		}
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		CreateWindow();
	}

	private void CreateWindow()
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[EndOfDayUIManager] No CustomUIManager in the scene, the end-of-day tablet cannot be created");
			return;
		}
		if (tabletPrefab == null)
		{
			Debug.LogError("[EndOfDayUIManager] tabletPrefab is not assigned. Run Build > Generate Tablet UI Prefabs in the store scene.");
			return;
		}
		window = CustomUIManager.Instance.CreateWindow("end_of_day_tablet", null, "paper", tabletPrefab).GetWindow();
		frame = ((window.root != null) ? window.root.GetComponent<EndOfDayTabletFrame>() : null);
		if (frame == null)
		{
			Debug.LogError("[EndOfDayUIManager] tabletPrefab has no EndOfDayTabletFrame on its root; regenerate it");
			window.Close();
			window = null;
			return;
		}
		UI = window.root;
		UI.name = "EndOfDayTablet";
		frame.closeButton.onClick.AddListener(OnCloseButtonClick);
		TabletUI.NoKeyboardFocus(frame.closeButton);
		frame.arrowsWatermark.gameObject.SetActive(value: false);
		UI.SetActive(value: false);
	}

	public void RefreshUI()
	{
		if (frame == null)
		{
			return;
		}
		int num = 0;
		foreach (StoreService item in PlayerStore.Instance.storeService)
		{
			if (item.unlocked && item.isUsing)
			{
				num += item.cost;
			}
		}
		int playerCash = PlayerStore.Instance.playerCash;
		bool flag = num <= 0 || num <= playerCash;
		if (fundsRow.value != null)
		{
			fundsRow.value.text = TabletUI.Credits(playerCash);
		}
		if (totalRow.value != null)
		{
			totalRow.value.text = TabletUI.Credits(num);
			totalRow.SetColor(flag ? TabletStyle.INK : TabletStyle.RED);
		}
		if (endDayButton != null)
		{
			endDayButton.SetInteractable(flag);
		}
	}

	public void OpenUI()
	{
		if (!(UI == null))
		{
			UI.SetActive(value: true);
			PaperPanelFocusStamp.BumpPanel(UI);
			InitUI();
			StoreUIManager.Instance.EnableBackingBlock();
			TutorialUIManager.Instance.OnEndOfDayOpen();
		}
	}

	public void CloseUI(bool restorePaperUI = true)
	{
		TutorialUIManager.Instance.DisableNightlyServiceTutorial();
		if (UI != null)
		{
			UI.SetActive(value: false);
		}
		StoreUIManager.Instance.DisableBackingBlock();
		if (restorePaperUI && PaperUIManager.Instance != null)
		{
			PaperUIManager.Instance.RestorePaperUIClosedForSleep(morning: false);
		}
	}

	public void OnCloseButtonClick()
	{
		EmporiumAudioPlayer.Instance.audioButton2.Play();
		CloseUI();
	}

	public void InitUI()
	{
		if (frame == null)
		{
			return;
		}
		frame.sectionHeader.text = LocHelper.GetLocalizedUI("ui_end_of_day") + " " + StoreStation.Instance.dayCounter;
		frame.infoLine.text = LocHelper.GetLocalizedUI("ui_endofday_info");
		TabletUI.ClearChildren(frame.content);
		TabletUI.ClearChildren(frame.footer);
		TabletUI.ClearChildren(frame.buttonHost);
		rows.Clear();
		StoreService.GetDebrisRemovalService()?.UpdateCost();
		StoreService.GetCurrentConverterService()?.UpdateCost();
		foreach (StoreService item in PlayerStore.Instance.storeService)
		{
			bool flag = item.id.Equals("POWER_SERVICE") && NetworkUpgrade.IsPowerHijackUnlocked();
			if (!(!item.unlocked || flag))
			{
				BuildServiceRow(item);
			}
		}
		fundsRow = TabletUI.FooterLine(frame, frame.footer, 0f, LocHelper.GetLocalizedUI("ui_nego_your_funds"), "");
		totalRow = TabletUI.FooterLine(frame, frame.footer, 35f, LocHelper.GetLocalizedUI("ui_nego_total"), "");
		endDayButton = TabletUI.BigButton(frame, frame.buttonHost, 153f, 0f, LocHelper.GetLocalizedUI("ui_end_day"), OnEndDayButtonClick);
		TabletUI.LayoutSection(frame, 22f, withSeparator: true, 500f);
		RefreshUI();
		TabletUI.ScrollToTop(frame.scroll);
	}

	private void BuildServiceRow(StoreService storeService)
	{
		bool flag = storeService.IsOnCooldown();
		if (storeService.isMandatory)
		{
			storeService.isUsing = true;
		}
		if (flag)
		{
			storeService.isUsing = false;
		}
		RectTransform rectTransform = TabletUI.NewRect(TabletUI.RowRoot(frame.content, "Service", 30f), "Visuals", 0f, 0f, 502f, 30f);
		if (flag)
		{
			rectTransform.gameObject.AddComponent<CanvasGroup>().alpha = 0.3f;
		}
		TabletCheckbox tabletCheckbox = TabletUI.Checkbox(frame, rectTransform, 4f, 4f, storeService.isUsing, delegate(bool on)
		{
			OnToggled(storeService, on);
		});
		tabletCheckbox.SetInteractable(!storeService.isMandatory && !flag);
		string text = (flag ? LocHelper.GetLocalizedMechanic("mech_service_cooldown", storeService.displayName, storeService.currentCooldown) : storeService.displayName);
		TextMeshProUGUI textMeshProUGUI = TabletUI.NewText(frame, rectTransform, "Name", 32f, 0f, 360f, 28f, FontLoader.FontRole.Noto_Sans, 18f, TabletStyle.INK, TextAlignmentOptions.MidlineLeft);
		textMeshProUGUI.text = text;
		if (!string.IsNullOrEmpty(storeService.tooltip))
		{
			float x = 32f + Mathf.Min(TabletUI.MeasureWidth(textMeshProUGUI, text), 360f) + 4f;
			string tooltipKey = storeService.tooltip;
			TabletUI.InfoIcon(rectTransform, x, 7f, frame.infoDark, () => LocHelper.GetLocalizedUI(tooltipKey));
		}
		TabletUI.AutoSize(textMeshProUGUI);
		TextMeshProUGUI textMeshProUGUI2 = TabletUI.NewText(frame, rectTransform, "Cost", 368f, 0f, 130f, 28f, FontLoader.FontRole.Noto_Sans, 20f, TabletStyle.INK, TextAlignmentOptions.MidlineRight);
		textMeshProUGUI2.text = TabletUI.Credits(storeService.cost);
		TabletUI.AutoSize(textMeshProUGUI2);
		TabletUI.NewFlat(rectTransform, "Separator", 0f, 28f, 502f, 2f, TabletStyle.ROW_SEPARATOR);
		rows.Add((storeService, tabletCheckbox));
	}

	private void OnToggled(StoreService storeService, bool on)
	{
		TutorialUIManager.Instance.DisableNightlyServiceTutorial();
		EmporiumAudioPlayer.Instance.audioButton1.Play();
		storeService.isUsing = on;
		RefreshUI();
	}

	public void OnEndDayButtonClick()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		EmporiumAnalytics.TrackOnSleep();
		if (endDayButton != null)
		{
			endDayButton.gameObject.SetActive(value: false);
		}
		EmporiumAudioPlayer.Instance.audioButton2.Play();
		StoreUIManager.Instance.savingTMP.gameObject.SetActive(value: true);
		StoreUIManager.Instance.StartBlackscreenSequence(playFullSequence: true, delegate
		{
			StoreShutterButton.Instance.ForceImmediateClose();
			PlayerStore.Instance.storeState = PlayerStore.StoreState.MORNING;
			PlayerStore.Instance.lockShutterButtonFlag = 0;
			StoreUIManager.Instance.CloseAllUI();
			EmporiumAudioPlayer.Instance.Play();
			CloseUI(restorePaperUI: false);
			PlayerStore.Instance.EndNight();
			StoreStation.Instance.storeEventManager.OnDayEnd();
			PlayerStore.Instance.storeState = PlayerStore.StoreState.MORNING;
			PlayerStore.Instance.lockShutterButtonFlag = 0;
			StoreUIManager.Instance.savingTMP.gameObject.SetActive(value: false);
			RecorderUIManager.Instance.ClearAllElement();
			PlayerStore.Instance.BeginDay();
			TutorialUIManager.Instance.OnDayBegin();
			if (ReactorHelper.ConsumeExplosion())
			{
				PlayerStore.Instance.ExecuteGameOver("reactor");
			}
			else if (PlayerStore.Instance.augPresence >= 100 && (Difficulty.Rules.augGameOverAtFullPresence || StartingPerk.IsAugMenace()))
			{
				PlayerStore.Instance.ExecuteGameOver("aug");
			}
			else
			{
				PlayerStore.Instance.SaveGame();
			}
		}, delegate
		{
			if (!GameOverUIManager.Instance.uiPanel.activeSelf)
			{
				if (PaperUIManager.Instance != null)
				{
					PaperUIManager.Instance.RestorePaperUIClosedForSleep(morning: true);
				}
				StartOfDayUIManager.Instance.OpenUI();
			}
			stopwatch.Stop();
			Debug.Log($"OnEndDayButtonClick took {stopwatch.ElapsedMilliseconds} ms");
		}, 0.5f, 0.5f);
	}
}

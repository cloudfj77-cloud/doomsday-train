using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartOfDayUIManager : MonoBehaviour
{
	public GameObject UIPanel;

	public GameObject contentGroupObject;

	public GameObject startOfDayTMPPrefab;

	public GameObject displayCaseReminder;

	public GameObject rentReminder;

	public TextMeshProUGUI rentReminderTMP;

	public TextMeshProUGUI estimatedNetWorth;

	private const float LOG_LINE_MIN_FONT_SIZE = 11f;

	public static StartOfDayUIManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OpenUI()
	{
		ModHook.FireOnWakingUpEarly();
		ShowMorningReport();
		AchievementTracker.Instance.CheckSettled();
		AchievementTracker.Instance.CheckRich();
		AchievementTracker.Instance.CheckIndustrious();
		AchievementTracker.Instance.CheckShiny();
		ModHook.FireOnWakingUpLate();
	}

	public void ShowMorningReport()
	{
		AdvCalendarUIManager.Instance.OpenUIFromNightlyReport();
		InitPanel();
		EmporiumAudioPlayer.Instance.audioButton1.Play();
		if (PlayerStore.Instance.storeState == PlayerStore.StoreState.CLOSED)
		{
			Debug.Log("***Extra check in case of bug, Triggered***");
			PlayerStore.Instance.storeState = PlayerStore.StoreState.MORNING;
			PlayerStore.Instance.lockShutterButtonFlag = 0;
		}
	}

	public void HideUI()
	{
		UIPanel.gameObject.SetActive(value: false);
		EmporiumAudioPlayer.Instance.audioButton2.Play();
	}

	public void InitPanel()
	{
		for (int num = contentGroupObject.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(contentGroupObject.transform.GetChild(num).gameObject);
		}
		List<TextMeshProUGUI> list = new List<TextMeshProUGUI>();
		foreach (string nightLog in PlayerStore.Instance.nightLogs)
		{
			TextMeshProUGUI component = UnityEngine.Object.Instantiate(startOfDayTMPPrefab, contentGroupObject.transform).GetComponent<TextMeshProUGUI>();
			component.fontSizeMin = 11f;
			component.text = "-" + nightLog;
			list.Add(component);
		}
		if (list.Count > 0)
		{
			StartCoroutine(UniformizeLogLineSizes(list));
		}
		PlayerStore.Instance.nightLogs.Clear();
		displayCaseReminder.SetActive(PlayerStore.Instance.isDisplayCaseIntroduced);
		if (PlayerStore.Instance.GetRentDayCounter() <= 2 && !PlayerStore.Instance.IsPropertyPaid)
		{
			string text = (PlayerStore.Instance.isPayingMortgage ? LocHelper.GetLocalizedUI("ui_startofday_mortgage_keyword") : LocHelper.GetLocalizedUI("ui_startofday_rent_keyword"));
			int num2 = (PlayerStore.Instance.isPayingMortgage ? PlayerStore.Instance.mortgageWeeklyPayment : PlayerStore.Instance.rentValue);
			int rentDayCounter = PlayerStore.Instance.GetRentDayCounter();
			switch (rentDayCounter)
			{
			case 0:
				rentReminderTMP.text = LocHelper.GetLocalizedUI("ui_startofday_due_today", num2, text);
				break;
			case 1:
				rentReminderTMP.text = LocHelper.GetLocalizedUI("ui_startofday_due_tomorrow", num2, text);
				break;
			default:
				rentReminderTMP.text = LocHelper.GetLocalizedUI("ui_startofday_due_in_days", num2, text, rentDayCounter);
				break;
			}
			rentReminder.SetActive(value: true);
		}
		else
		{
			rentReminder.SetActive(value: false);
		}
		foreach (Action item in PlayerStore.Instance.nightlyAction)
		{
			item();
		}
		PlayerStore.Instance.nightlyAction.Clear();
	}

	private IEnumerator UniformizeLogLineSizes(List<TextMeshProUGUI> logLines)
	{
		yield return null;
		float num = float.MaxValue;
		foreach (TextMeshProUGUI logLine in logLines)
		{
			if (!(logLine == null) && logLine.enableAutoSizing)
			{
				num = Mathf.Min(num, logLine.fontSize);
			}
		}
		if (num == float.MaxValue)
		{
			yield break;
		}
		foreach (TextMeshProUGUI logLine2 in logLines)
		{
			if (!(logLine2 == null))
			{
				logLine2.enableAutoSizing = false;
				logLine2.fontSize = num;
			}
		}
	}

	public void OnStartDayButtonClicked()
	{
		HideUI();
		EmporiumAudioPlayer.Instance.Play();
	}
}

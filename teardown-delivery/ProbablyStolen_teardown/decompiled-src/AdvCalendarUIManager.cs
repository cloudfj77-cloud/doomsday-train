using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdvCalendarUIManager : MonoBehaviour
{
	public GameObject UIPanel;

	public GridLayoutGroup gridLayoutGroup;

	private bool isInit;

	private List<CalendarDayElement> dayElements;

	public GameObject dayElementPrefab;

	public TextMeshProUGUI detailNameTMP;

	public TextMeshProUGUI detailDescriptionTMP;

	public Sprite startAndFinish;

	public Sprite startOnly;

	public Sprite finishOnly;

	public Sprite middle;

	public Sprite startAndFinishTop;

	public Sprite startOnlyTop;

	public Sprite finishOnlyTop;

	public Sprite middleTop;

	public Button calendarButton;

	public Button nightlyReportButton;

	public Button statusButton;

	public GameObject calendarSection;

	public GameObject nightlyReportSection;

	public GameObject statusSection;

	public Color normalColor;

	public Color selectedColor;

	public TextMeshProUGUI totalWealthTMP;

	public TextMeshProUGUI currentCreditTMP;

	public TextMeshProUGUI redeemedTokenTMP;

	public GameObject redeemedTokenTitle;

	public TextMeshProUGUI storeAttractivenessTMP;

	public Image baseMeter;

	public Image trashMeter;

	public Image projectorMeter;

	public GameObject activePerksContainer;

	public GameObject perkElementPrefab;

	public Sprite highlightPerkBackground;

	public TextMeshProUGUI landlordTMP;

	public TextMeshProUGUI mentorTMP;

	public GameObject landlordNoticeBox;

	public GameObject mentorNoticeBox;

	private const int METER_MAX_ATTRACTIVENESS = 1000;

	public static AdvCalendarUIManager Instance { get; private set; }

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
		if (!isInit)
		{
			Init();
		}
		Reset();
		HandleInnateEvent();
		int dayCounter = StoreStation.Instance.dayCounter;
		int num = dayCounter % 7;
		if (num == 0)
		{
			num = 7;
		}
		int num2 = dayCounter - num + 1;
		for (int i = 0; i < dayElements.Count; i++)
		{
			if (i == 0)
			{
				dayElements[i].isFirstOnCalendar = true;
			}
			else
			{
				dayElements[i].isFirstOnCalendar = false;
			}
			CalendarDayElement calendarDayElement = dayElements[i];
			int num3 = num2 + i;
			CalendarDayElement previousDayElement = null;
			if (i > 0)
			{
				previousDayElement = dayElements[i - 1];
			}
			calendarDayElement.SetElement(num3, StoreStation.Instance.storeEventManager.GetActiveEventOnDay(num3), previousDayElement);
		}
		UIPanel.gameObject.SetActive(value: true);
		EmporiumAudioPlayer.Instance.audioButton1.Play();
		OnCalendarButtonClicked();
	}

	public void CloseUI()
	{
		UIPanel.gameObject.SetActive(value: false);
		EmporiumAudioPlayer.Instance.audioButton1.Play();
		StartingPerkElement.showTooltip = false;
		PerkUIController.Instance.HideTooltip();
	}

	public void HandleInnateEvent()
	{
		int dayCounter = StoreStation.Instance.dayCounter;
		if (PlayerStore.Instance.IsPayingRent())
		{
			if (dayCounter % 7 == 0 && !StoreStation.Instance.storeEventManager.IsEventActive("rent_day"))
			{
				StoreStation.Instance.storeEventManager.ActivateEvent(EventListInnate.CreateRentDayEvent());
			}
			int nextDay = GetNextDay(dayCounter, 7, 0);
			if (nextDay != dayCounter && !StoreStation.Instance.storeEventManager.IsEventActiveInDay("rent_day", nextDay - dayCounter))
			{
				StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListInnate.CreateRentDayEvent(), nextDay - dayCounter, IsKnown: true);
			}
			int nextDay2 = GetNextDay(dayCounter, 7, 1);
			if (nextDay2 != dayCounter && !StoreStation.Instance.storeEventManager.IsEventActiveInDay("rent_day", nextDay2 - dayCounter))
			{
				StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListInnate.CreateRentDayEvent(), nextDay2 - dayCounter, IsKnown: true);
			}
		}
		else if (PlayerStore.Instance.isPayingMortgage)
		{
			StoreStation.Instance.storeEventManager.RemoveAllEvent("rent_day");
			if (dayCounter % 7 == 0 && !StoreStation.Instance.storeEventManager.IsEventActive("mortgage_payment"))
			{
				StoreStation.Instance.storeEventManager.ActivateEvent(EventListInnate.CreateMortgagePaymentEvent());
			}
			int nextDay3 = GetNextDay(dayCounter, 7, 0);
			if (nextDay3 != dayCounter && !StoreStation.Instance.storeEventManager.IsEventActiveInDay("mortgage_payment", nextDay3 - dayCounter))
			{
				StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListInnate.CreateMortgagePaymentEvent(), nextDay3 - dayCounter, IsKnown: true);
			}
			int nextDay4 = GetNextDay(dayCounter, 7, 1);
			if (nextDay4 != dayCounter && !StoreStation.Instance.storeEventManager.IsEventActiveInDay("mortgage_payment", nextDay4 - dayCounter))
			{
				StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListInnate.CreateMortgagePaymentEvent(), nextDay4 - dayCounter, IsKnown: true);
			}
		}
		else if (PlayerStore.Instance.IsPropertyPaid)
		{
			StoreStation.Instance.storeEventManager.RemoveAllEvent("rent_day");
			StoreStation.Instance.storeEventManager.RemoveAllEvent("mortgage_payment");
		}
		if (PlayerStore.Instance.IsChemistStart() && !PlayerStore.Instance.isCartelExited)
		{
			if (dayCounter % 7 == 1 && !StoreStation.Instance.storeEventManager.IsEventActive("cartel_visit"))
			{
				StoreStation.Instance.storeEventManager.ActivateEvent(EventListInnate.CreateCartelVisitEvent());
			}
			int nextDay5 = GetNextDay(dayCounter, 1, 0);
			if (nextDay5 != dayCounter && !StoreStation.Instance.storeEventManager.IsEventActiveInDay("cartel_visit", nextDay5 - dayCounter))
			{
				StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListInnate.CreateCartelVisitEvent(), nextDay5 - dayCounter, IsKnown: true);
			}
			int nextDay6 = GetNextDay(dayCounter, 1, 1);
			if (nextDay6 != dayCounter && !StoreStation.Instance.storeEventManager.IsEventActiveInDay("cartel_visit", nextDay6 - dayCounter))
			{
				StoreStation.Instance.storeEventManager.QueueFuturEvent(EventListInnate.CreateCartelVisitEvent(), nextDay6 - dayCounter, IsKnown: true);
			}
		}
		else if (PlayerStore.Instance.IsChemistStart())
		{
			StoreStation.Instance.storeEventManager.RemoveAllEvent("cartel_visit");
		}
	}

	public static int GetNextDay(int today, int targetWeekday, int which)
	{
		int num = today % 7;
		int num2 = (targetWeekday - num + 7) % 7;
		if (num2 == 0)
		{
			num2 = 7;
		}
		return today + num2 + which * 7;
	}

	public void Reset()
	{
		detailNameTMP.text = "";
		detailDescriptionTMP.text = "";
	}

	public void Init()
	{
		isInit = true;
		dayElements = new List<CalendarDayElement>();
		for (int i = 0; i < 14; i++)
		{
			CalendarDayElement component = UnityEngine.Object.Instantiate(dayElementPrefab, gridLayoutGroup.transform).GetComponent<CalendarDayElement>();
			dayElements.Add(component);
		}
	}

	public void OnCalendarButtonClicked()
	{
		OnAnySectionButtonClicked();
		calendarButton.image.color = selectedColor;
		calendarSection.SetActive(value: true);
		if (PlayerStore.Instance.IsPropertyPaid)
		{
			landlordNoticeBox.SetActive(value: false);
			landlordTMP.text = "";
		}
		else if (PlayerStore.Instance.isPayingMortgage)
		{
			landlordNoticeBox.SetActive(value: true);
			landlordTMP.text = LocHelper.GetLocalizedMechanic("mech_rent_loan_shark", PlayerStore.Instance.mortgageWeeklyPayment, GetFormattedDayDisplay(PlayerStore.Instance.dayUntilRent));
		}
		else if (PlayerStore.Instance.temporaryRent == 0)
		{
			landlordNoticeBox.SetActive(value: true);
			landlordTMP.text = LocHelper.GetLocalizedMechanic("mech_rent_landlord", PlayerStore.Instance.rentValue, GetFormattedDayDisplay(PlayerStore.Instance.dayUntilRent));
		}
		else
		{
			landlordNoticeBox.SetActive(value: true);
			landlordTMP.text = LocHelper.GetLocalizedMechanic("mech_rent_landlord_missed", PlayerStore.Instance.rentValue, PlayerStore.Instance.temporaryRent, GetFormattedDayDisplay(PlayerStore.Instance.dayUntilRent));
		}
		if (PlayerStore.Instance.IsPropertyPaid)
		{
			mentorNoticeBox.SetActive(value: false);
			return;
		}
		mentorNoticeBox.SetActive(value: true);
		mentorTMP.text = LocHelper.GetLocalizedMechanic("mech_rent_property_value", PlayerStore.Instance.GetStoreBuyPrice());
	}

	public void OnNightlyReportButtonClicked()
	{
		OnAnySectionButtonClicked();
		UpdateAttractivenessSubsection();
		nightlyReportButton.image.color = selectedColor;
		nightlyReportSection.SetActive(value: true);
	}

	public void OnStatusButtonClicked()
	{
		OnAnySectionButtonClicked();
		statusButton.image.color = selectedColor;
		statusSection.SetActive(value: true);
		totalWealthTMP.text = PlayerStore.Instance.GetTotalEstimatedValue() + " " + LocHelper.GetLocalizedMechanic("mech_credits");
		currentCreditTMP.text = PlayerStore.Instance.playerCash + " " + LocHelper.GetLocalizedMechanic("mech_credits");
		if (PlayerStore.Instance.isStampIntroduced)
		{
			redeemedTokenTMP.gameObject.SetActive(value: true);
			redeemedTokenTitle.SetActive(value: true);
			redeemedTokenTMP.text = LocHelper.GetLocalizedMechanic("mech_commissary_token", PlayerStore.Instance.commissaryData.redeemedCredit);
		}
		else
		{
			redeemedTokenTMP.gameObject.SetActive(value: false);
			redeemedTokenTitle.SetActive(value: false);
		}
		foreach (StartingPerk startingPerk in PlayerStore.Instance.startingPerks)
		{
			StartingPerkElement component = UnityEngine.Object.Instantiate(perkElementPrefab, activePerksContainer.transform).GetComponent<StartingPerkElement>();
			component.perk = startingPerk;
			component.id = startingPerk.id;
			component.background.sprite = highlightPerkBackground;
		}
	}

	public void OnAnySectionButtonClicked()
	{
		calendarButton.image.color = normalColor;
		nightlyReportButton.image.color = normalColor;
		statusButton.image.color = normalColor;
		calendarSection.SetActive(value: false);
		nightlyReportSection.SetActive(value: false);
		statusSection.SetActive(value: false);
		for (int num = activePerksContainer.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(activePerksContainer.transform.GetChild(num).gameObject);
		}
	}

	public void OpenUIFromNightlyReport()
	{
		OpenUI();
		UpdateAttractivenessSubsection();
		OnNightlyReportButtonClicked();
	}

	public void UpdateAttractivenessSubsection()
	{
		int currentStoreAttractiveness = PlayerStore.Instance.GetCurrentStoreAttractiveness();
		int val = Math.Abs(PlayerStore.Instance.GetTrashAttPenalty());
		int num = Math.Clamp(currentStoreAttractiveness, 0, 1000);
		int num2 = Math.Min(PlayerStore.Instance.projectorAttractivenessBonus, num);
		int num3 = num - num2;
		int num4 = Math.Min(val, 1000 - num);
		if (num4 == 0)
		{
			trashMeter.gameObject.SetActive(value: false);
		}
		else
		{
			trashMeter.gameObject.SetActive(value: true);
			trashMeter.rectTransform.sizeDelta = new Vector2(num4, trashMeter.rectTransform.sizeDelta.y);
		}
		if (num2 == 0)
		{
			projectorMeter.gameObject.SetActive(value: false);
		}
		else
		{
			projectorMeter.gameObject.SetActive(value: true);
			projectorMeter.rectTransform.sizeDelta = new Vector2(num2, projectorMeter.rectTransform.sizeDelta.y);
		}
		baseMeter.rectTransform.sizeDelta = new Vector2(num3, baseMeter.rectTransform.sizeDelta.y);
		storeAttractivenessTMP.text = LocHelper.GetLocalizedMechanic("mech_store_attractiveness") + ": " + currentStoreAttractiveness + " <link=\"ui_info_store_attractiveness\"><sprite name=\"info\" tint=1></link>";
	}

	public void ToggleUI()
	{
		if (UIPanel.activeSelf)
		{
			CloseUI();
		}
		else
		{
			OpenUI();
		}
	}

	public static string GetFormattedDayDisplay(int day)
	{
		return day switch
		{
			0 => LocHelper.GetLocalizedMechanic("mech_rent_day_today"), 
			1 => LocHelper.GetLocalizedMechanic("mech_rent_day_tomorrow"), 
			_ => LocHelper.GetLocalizedMechanic("mech_rent_day_in_days", day), 
		};
	}
}

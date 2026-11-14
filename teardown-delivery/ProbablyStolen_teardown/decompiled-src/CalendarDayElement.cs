using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalendarDayElement : MonoBehaviour
{
	public VerticalLayoutGroup layoutGroup;

	public Image backgroundImage;

	public TextMeshProUGUI dayTMP;

	public GameObject eventElementPrefab;

	public List<CalendarEventElement> eventElements;

	public bool isInit;

	public int dayNumber;

	public List<StoreEvent> events;

	public bool isFirstOnCalendar;

	public GameObject todayBackground;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Init()
	{
		isInit = true;
		eventElements = new List<CalendarEventElement>();
		for (int i = 0; i < 5; i++)
		{
			CalendarEventElement component = Object.Instantiate(eventElementPrefab, layoutGroup.transform).GetComponent<CalendarEventElement>();
			eventElements.Add(component);
			component.gameObject.SetActive(value: false);
		}
	}

	public void SetPastDay()
	{
	}

	public void SetToday()
	{
		todayBackground.SetActive(value: true);
		dayTMP.color = MetaHelper.HexToColor("9c3738");
	}

	public void SetFutureDay()
	{
	}

	public void SetElement(int dayNumber, List<StoreEvent> events, CalendarDayElement previousDayElement)
	{
		if (!isInit)
		{
			Init();
		}
		Reset();
		this.events = events;
		int dayCounter = StoreStation.Instance.dayCounter;
		dayTMP.text = LocHelper.GetLocalizedMechanic("mech_day_label", dayNumber);
		this.dayNumber = dayNumber;
		if (dayNumber < dayCounter)
		{
			SetPastDay();
		}
		else if (dayNumber == dayCounter)
		{
			SetToday();
		}
		else
		{
			SetFutureDay();
		}
		HashSet<int> hashSet = new HashSet<int>();
		if (previousDayElement != null)
		{
			for (int i = 0; i < previousDayElement.eventElements.Count; i++)
			{
				CalendarEventElement calendarEventElement = previousDayElement.eventElements[i];
				if (!calendarEventElement.gameObject.activeSelf || calendarEventElement.storeEvent == null)
				{
					continue;
				}
				StoreEvent storeEvent = calendarEventElement.storeEvent;
				foreach (StoreEvent @event in events)
				{
					if (@event == storeEvent)
					{
						CalendarEventElement calendarEventElement2 = eventElements[i];
						calendarEventElement2.gameObject.SetActive(value: true);
						calendarEventElement2.dayNumber = dayNumber;
						calendarEventElement2.SetEvent(@event, isFirstOnCalendar);
						hashSet.Add(i);
						break;
					}
				}
			}
		}
		int j = 0;
		foreach (StoreEvent event2 in events)
		{
			bool flag = false;
			foreach (int item in hashSet)
			{
				if (eventElements[item].storeEvent == event2)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				for (; j < eventElements.Count && hashSet.Contains(j); j++)
				{
				}
				if (j >= eventElements.Count)
				{
					break;
				}
				CalendarEventElement calendarEventElement3 = eventElements[j];
				calendarEventElement3.gameObject.SetActive(value: true);
				calendarEventElement3.dayNumber = dayNumber;
				calendarEventElement3.SetEvent(event2, isFirstOnCalendar);
				hashSet.Add(j);
			}
		}
		if (!(previousDayElement != null))
		{
			return;
		}
		int num = -1;
		foreach (int item2 in hashSet)
		{
			if (item2 > num)
			{
				num = item2;
			}
		}
		for (int k = 0; k < previousDayElement.eventElements.Count; k++)
		{
			if (k <= num && previousDayElement.eventElements[k].gameObject.activeSelf && !hashSet.Contains(k) && !eventElements[k].gameObject.activeSelf)
			{
				CalendarEventElement calendarEventElement4 = eventElements[k];
				calendarEventElement4.gameObject.SetActive(value: true);
				calendarEventElement4.SetInvisibleElement();
				hashSet.Add(k);
			}
		}
	}

	public void Reset()
	{
		dayTMP.color = MetaHelper.HexToColor("44464c");
		todayBackground.SetActive(value: false);
		if (events != null)
		{
			events.Clear();
		}
		foreach (CalendarEventElement eventElement in eventElements)
		{
			eventElement.gameObject.SetActive(value: false);
			eventElement.Reset();
		}
	}
}

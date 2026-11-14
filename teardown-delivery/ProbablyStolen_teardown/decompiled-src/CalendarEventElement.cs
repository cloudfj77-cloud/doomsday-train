using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CalendarEventElement : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public TextMeshProUGUI eventTMP;

	public Image backgroundImage;

	public Image topImage;

	public StoreEvent storeEvent;

	public int dayNumber;

	public bool isInvisible;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!isInvisible)
		{
			AdvCalendarUIManager.Instance.detailNameTMP.text = storeEvent.GetDisplayName();
			if (storeEvent.negociationDatas.Count != 0)
			{
				AdvCalendarUIManager.Instance.detailDescriptionTMP.text = storeEvent.GetEventPriceEffect();
			}
			else
			{
				AdvCalendarUIManager.Instance.detailDescriptionTMP.text = "";
			}
		}
	}

	public void SetEvent(StoreEvent storeEvent, bool isTodayFirstDayOnCalendar)
	{
		isInvisible = false;
		eventTMP.text = storeEvent.GetDisplayName();
		this.storeEvent = storeEvent;
		topImage.color = MetaHelper.HexToColor("cc7755");
		backgroundImage.color = new Color(1f, 1f, 1f, 1f);
		HandleColor(storeEvent, topImage);
		StoreEventManager.EventDayType eventDayType = StoreEventManager.GetEventDayType(storeEvent, dayNumber);
		backgroundImage.sprite = GetBackgroundSprite(storeEvent, eventDayType);
		topImage.sprite = GetTopSprite(storeEvent, eventDayType);
		switch (eventDayType)
		{
		case StoreEventManager.EventDayType.StartAndFinish:
			backgroundImage.rectTransform.sizeDelta = new Vector2(152f, 38f);
			break;
		case StoreEventManager.EventDayType.FirstDay:
			backgroundImage.rectTransform.sizeDelta = new Vector2(164f, 38f);
			break;
		case StoreEventManager.EventDayType.LastDay:
			if (!isTodayFirstDayOnCalendar)
			{
				eventTMP.text = "";
			}
			backgroundImage.rectTransform.sizeDelta = new Vector2(164f, 38f);
			break;
		default:
			if (!isTodayFirstDayOnCalendar)
			{
				eventTMP.text = "";
			}
			backgroundImage.rectTransform.sizeDelta = new Vector2(164f, 38f);
			break;
		}
	}

	public static void HandleColor(StoreEvent storeEvent, Image topImage)
	{
		if (storeEvent.IsSurplus())
		{
			topImage.color = MetaHelper.HexToColor("9c3738");
		}
		if (storeEvent.IsShortage())
		{
			topImage.color = MetaHelper.HexToColor("5c6f46");
		}
		if (storeEvent.eventType == StoreEvent.EventType.INNATE)
		{
			topImage.color = MetaHelper.HexToColor("44464c");
		}
	}

	public static Sprite GetBackgroundSprite(StoreEvent storeEvent, StoreEventManager.EventDayType eventDayType)
	{
		return eventDayType switch
		{
			StoreEventManager.EventDayType.StartAndFinish => AdvCalendarUIManager.Instance.startAndFinish, 
			StoreEventManager.EventDayType.FirstDay => AdvCalendarUIManager.Instance.startOnly, 
			StoreEventManager.EventDayType.LastDay => AdvCalendarUIManager.Instance.finishOnly, 
			_ => AdvCalendarUIManager.Instance.middle, 
		};
	}

	public static Sprite GetTopSprite(StoreEvent storeEvent, StoreEventManager.EventDayType eventDayType)
	{
		return eventDayType switch
		{
			StoreEventManager.EventDayType.StartAndFinish => AdvCalendarUIManager.Instance.startAndFinishTop, 
			StoreEventManager.EventDayType.FirstDay => AdvCalendarUIManager.Instance.startOnlyTop, 
			StoreEventManager.EventDayType.LastDay => AdvCalendarUIManager.Instance.finishOnlyTop, 
			_ => AdvCalendarUIManager.Instance.middleTop, 
		};
	}

	public void SetInvisibleElement()
	{
		isInvisible = true;
		eventTMP.text = "";
		backgroundImage.color = new Color(0f, 0f, 0f, 0f);
		topImage.color = new Color(0f, 0f, 0f, 0f);
		storeEvent = null;
	}

	public void Reset()
	{
		storeEvent = null;
	}
}

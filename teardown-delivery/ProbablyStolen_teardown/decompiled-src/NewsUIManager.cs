using System.Collections.Generic;
using UnityEngine;

public class NewsUIManager : MonoBehaviour
{
	public AudioSource audioSourcePickup;

	public AudioSource audioSourceDrop;

	public GameObject uiGameObject;

	public List<NewsUIElement> allElements;

	private GameObject activeLayout;

	private List<NewsUIElement> activeLayoutElements;

	public GameObject layout1;

	public GameObject layout2;

	public GameObject layout3;

	public GameObject layout4;

	public List<NewsUIElement> layout1Elements;

	public List<NewsUIElement> layout2Elements;

	public List<NewsUIElement> layout3Elements;

	public List<NewsUIElement> layout4Elements;

	public static NewsUIManager Instance { get; private set; }

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
	}

	private void Update()
	{
	}

	public void ToggleUI()
	{
		if (uiGameObject.activeSelf)
		{
			audioSourceDrop.Play();
			uiGameObject.SetActive(value: false);
		}
		else
		{
			audioSourcePickup.Play();
			uiGameObject.SetActive(value: true);
			PopulateUI();
		}
	}

	public void CloseUI()
	{
		if (uiGameObject.activeSelf)
		{
			uiGameObject.SetActive(value: false);
			audioSourceDrop.Play();
		}
	}

	public void SelectLayout(List<StoreEvent> storeEvents)
	{
		HideAllLayouts();
		if (storeEvents.Count <= 1)
		{
			ActivateLayout(layout1, layout1Elements);
		}
		else if (storeEvents.Count == 2)
		{
			ActivateLayout(layout2, layout2Elements);
		}
		else if (storeEvents.Count == 3)
		{
			ActivateLayout(layout3, layout3Elements);
		}
		else
		{
			ActivateLayout(layout4, layout4Elements);
		}
	}

	public void HideAllElement()
	{
		foreach (NewsUIElement allElement in allElements)
		{
			allElement.gameObject.SetActive(value: false);
			allElement.ClearElement();
		}
	}

	public void PopulateUI()
	{
		List<StoreEvent> activeEvents = StoreStation.Instance.storeEventManager.GetActiveEvents();
		activeEvents.RemoveAll((StoreEvent e) => e.isHiddenFromNewsPaper);
		if (activeEvents.Count == 0)
		{
			StoreEvent storeEvent = EventListCosmetic.AllQuiet();
			StoreStation.Instance.storeEventManager.ActivateEvent(storeEvent, debugLog: false, force: true);
			activeEvents.Add(storeEvent);
		}
		SelectLayout(activeEvents);
		if (activeLayoutElements != null)
		{
			int num = Mathf.Min(activeEvents.Count, activeLayoutElements.Count);
			for (int i = 0; i < num; i++)
			{
				activeLayoutElements[i].gameObject.SetActive(value: true);
				activeLayoutElements[i].PopulateUI(activeEvents[i]);
			}
		}
	}

	private void ActivateLayout(GameObject layout, List<NewsUIElement> elements)
	{
		activeLayout = layout;
		activeLayoutElements = elements;
		layout.SetActive(value: true);
		foreach (NewsUIElement element in elements)
		{
			element.gameObject.SetActive(value: false);
			element.ClearElement();
		}
	}

	private void HideAllLayouts()
	{
		layout1.SetActive(value: false);
		layout2.SetActive(value: false);
		layout3.SetActive(value: false);
		layout4.SetActive(value: false);
	}
}

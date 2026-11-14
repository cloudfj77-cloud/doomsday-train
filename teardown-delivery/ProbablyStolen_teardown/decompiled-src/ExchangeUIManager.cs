using System.Collections.Generic;
using UnityEngine;

public class ExchangeUIManager : MonoBehaviour
{
	public GameObject panel;

	public GameObject promotionalSection;

	private List<ExchangeElement> exchangeElements;

	public static ExchangeUIManager Instance { get; private set; }

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
		exchangeElements = new List<ExchangeElement>();
		foreach (Transform item in promotionalSection.transform)
		{
			ExchangeElement component = item.GetComponent<ExchangeElement>();
			if (component != null)
			{
				exchangeElements.Add(component);
			}
		}
	}

	private void Update()
	{
	}

	public void OpenPanel(GameItem gameItem)
	{
		PaperUIManager.Instance.audioSourcePickup.Play();
		panel.SetActive(value: true);
		foreach (ExchangeElement exchangeElement in exchangeElements)
		{
			exchangeElement.gameObject.SetActive(value: false);
			if (PlayerStore.Instance.exchangeManager.activePromotionalBarterIds.Contains(exchangeElement.identifier))
			{
				exchangeElement.gameObject.SetActive(value: true);
			}
		}
	}

	public void ClosePanel()
	{
		PaperUIManager.Instance.audioSourceDrop.Play();
		panel.SetActive(value: false);
	}
}

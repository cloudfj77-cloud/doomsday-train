using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PCUIManager : MonoBehaviour
{
	public GridLayoutGroup gridLayout;

	private List<StoreRequestElement> storeRequestElements;

	public StorePC storePC;

	public GameObject prefab;

	public GameObject panel;

	public static PCUIManager Instance { get; private set; }

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
		storeRequestElements = new List<StoreRequestElement>();
	}

	private void Update()
	{
	}

	public void OpenUI()
	{
		panel.SetActive(value: true);
		EmporiumAudioPlayer.Instance.PlayMatched();
		OnChange();
	}

	public void HideUI()
	{
		panel.SetActive(value: false);
		EmporiumAudioPlayer.Instance.PlayUnMatched();
	}

	public void ToggleUI()
	{
		if (panel.activeSelf)
		{
			HideUI();
		}
		else
		{
			OpenUI();
		}
	}

	public void OnChange()
	{
		for (int num = storeRequestElements.Count - 1; num >= 0; num--)
		{
			Object.Destroy(storeRequestElements[num].gameObject);
		}
		storeRequestElements.Clear();
		foreach (StoreRequest storeRequest in PlayerStore.Instance.storeRequests)
		{
			StoreRequestElement component = Object.Instantiate(prefab, gridLayout.transform).GetComponent<StoreRequestElement>();
			component.Init(storeRequest);
			storeRequestElements.Add(component);
		}
	}
}

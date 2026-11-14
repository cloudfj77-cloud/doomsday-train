using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecorderUIManager : MonoBehaviour
{
	public GameObject UIPanel;

	public GameObject ScrollViewContent;

	public GameObject recorderElementPrefab;

	public GameObject seperatorPrefab;

	private List<TextMeshProUGUI> elementList;

	private List<GameObject> seperatorList;

	public AudioSource openAudio;

	public AudioSource closeAudio;

	public ScrollRect scrollRect;

	public static RecorderUIManager Instance { get; private set; }

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
		InitElement();
	}

	private void Update()
	{
	}

	public void InitElement()
	{
		elementList = new List<TextMeshProUGUI>();
		for (int i = 0; i < 100; i++)
		{
			TextMeshProUGUI component = Object.Instantiate(recorderElementPrefab, ScrollViewContent.transform).GetComponent<TextMeshProUGUI>();
			elementList.Add(component);
			component.gameObject.SetActive(value: false);
		}
		seperatorList = new List<GameObject>();
		for (int j = 0; j < 50; j++)
		{
			GameObject gameObject = Object.Instantiate(seperatorPrefab, ScrollViewContent.transform);
			seperatorList.Add(gameObject);
			gameObject.SetActive(value: false);
		}
	}

	public void OpenUI()
	{
		TutorialUIManager.Instance.OnRecorderOpen();
		UIPanel.SetActive(value: true);
		openAudio.Play();
		RefreshUI();
		scrollRect.normalizedPosition = new Vector2(0f, 0f);
	}

	public void RefreshUI()
	{
		if (!UIPanel.activeSelf)
		{
			return;
		}
		Canvas.ForceUpdateCanvases();
		bool flag = scrollRect.verticalNormalizedPosition <= 0.05f;
		foreach (TextMeshProUGUI element in elementList)
		{
			element.gameObject.SetActive(value: false);
		}
		foreach (GameObject seperator in seperatorList)
		{
			seperator.SetActive(value: false);
		}
		int num = 0;
		int num2 = 0;
		foreach (string item in PlayerStore.Instance.dialogHistory)
		{
			if (item.Equals("---"))
			{
				if (num2 >= seperatorList.Count)
				{
					Debug.LogError("Not enough seperator elements in RecorderUIManager!");
					continue;
				}
				seperatorList[num2].SetActive(value: true);
				seperatorList[num2].transform.SetAsLastSibling();
				num2++;
			}
			else if (num >= elementList.Count)
			{
				Debug.LogError("Not enough dialogue elements in RecorderUIManager!");
			}
			else
			{
				elementList[num].gameObject.SetActive(value: true);
				elementList[num].transform.SetAsLastSibling();
				elementList[num].text = " " + item;
				num++;
			}
		}
		if (flag)
		{
			StartCoroutine(SnapToBottomNextFrame());
		}
	}

	private IEnumerator SnapToBottomNextFrame()
	{
		yield return null;
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
		scrollRect.verticalNormalizedPosition = 0f;
		Canvas.ForceUpdateCanvases();
	}

	public void CloseUI()
	{
		closeAudio.Play();
		UIPanel.SetActive(value: false);
	}

	public void Toggle()
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

	public void AddSeperator()
	{
		DialogUIManager.Instance.UpdateDialogHistory("---");
	}

	public void ClearAllElement()
	{
		PlayerStore.Instance.dialogHistory.Clear();
	}
}

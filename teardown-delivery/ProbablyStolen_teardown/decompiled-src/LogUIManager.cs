using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogUIManager : MonoBehaviour
{
	public const string LOG_CONTENT_TAG = "LOG_CONTENT_TAG";

	public GameObject UIPanel;

	public GameObject ScrollViewContent;

	public GameObject recorderElementPrefab;

	private List<TextMeshProUGUI> elementList;

	public AudioSource openAudio;

	public AudioSource closeAudio;

	public ScrollRect scrollRect;

	public static LogUIManager Instance { get; private set; }

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
	}

	public void OpenUI(GameItem gameItem)
	{
		TutorialUIManager.Instance.OnRecorderOpen();
		string[] array = gameItem.GetTagReadonly("LOG_CONTENT_TAG").GetString().Split('/');
		UIPanel.SetActive(value: true);
		openAudio.Play();
		foreach (TextMeshProUGUI element in elementList)
		{
			element.gameObject.SetActive(value: false);
		}
		int num = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			elementList[num].gameObject.SetActive(value: true);
			elementList[num].text = " " + text;
			num++;
		}
		scrollRect.normalizedPosition = new Vector2(0f, 0f);
	}

	public void CloseUI()
	{
		closeAudio.Play();
		UIPanel.SetActive(value: false);
	}

	public void Toggle(GameItem gameItem)
	{
		if (UIPanel.activeSelf)
		{
			CloseUI();
		}
		else
		{
			OpenUI(gameItem);
		}
	}
}

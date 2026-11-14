using System.Collections.Generic;
using UnityEngine;

public class LabelerUIManager : MonoBehaviour
{
	public GameObject panel;

	public GameObject elementPrefab;

	public GameObject verticalLayoutGroup;

	public GameObject repeatButton;

	private GameItem curItem;

	private static Dictionary<string, string> lastLabels = new Dictionary<string, string>();

	public static LabelerUIManager Instance { get; private set; }

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

	public void OpenUI(GameItem gameItem)
	{
		panel.SetActive(value: true);
		curItem = gameItem;
		for (int num = verticalLayoutGroup.transform.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(verticalLayoutGroup.transform.GetChild(num).gameObject);
		}
		foreach (ItemFeature itemFeature in gameItem.itemFeatures)
		{
			if (LabelerHelper.IsEditableLabelFeature(itemFeature))
			{
				Object.Instantiate(elementPrefab, verticalLayoutGroup.transform).GetComponent<LabelerElement>().InitElement(itemFeature);
			}
		}
		repeatButton.SetActive(CanRepeat());
	}

	public void CloseUI()
	{
		SaveCurrentLabels();
		EmporiumAudioPlayer.Instance.Play();
		panel.SetActive(value: false);
		curItem = null;
	}

	private void SaveCurrentLabels()
	{
		lastLabels.Clear();
		foreach (Transform item in verticalLayoutGroup.transform)
		{
			LabelerElement component = item.GetComponent<LabelerElement>();
			if (!(component == null))
			{
				lastLabels[component.GetCategory()] = component.GetSelectedConditionIdentifier();
			}
		}
	}

	public bool CanRepeat()
	{
		if (lastLabels.Count == 0 || curItem == null)
		{
			return false;
		}
		foreach (ItemFeature itemFeature in curItem.itemFeatures)
		{
			if (lastLabels.ContainsKey(itemFeature.category))
			{
				return true;
			}
		}
		return false;
	}

	public void Repeat()
	{
		if (curItem == null)
		{
			return;
		}
		foreach (ItemFeature itemFeature in curItem.itemFeatures)
		{
			if (!lastLabels.TryGetValue(itemFeature.category, out var value))
			{
				continue;
			}
			foreach (ItemCondition item in ItemConditionList.conditionLookup[itemFeature.category])
			{
				if (item.identifier.Equals(value))
				{
					itemFeature.fakeCondition = item;
					itemFeature.SetExposed(value: false);
					break;
				}
			}
		}
		EmporiumAudioPlayer.Instance.audioLabeler.Play();
		OpenUI(curItem);
	}
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FuelUI : MonoBehaviour
{
	[Serializable]
	public class StringGameObjectPair
	{
		public string Key;

		public GameObject Value;
	}

	[SerializeField]
	private List<StringGameObjectPair> color = new List<StringGameObjectPair>();

	[SerializeField]
	private List<StringGameObjectPair> logo = new List<StringGameObjectPair>();

	[SerializeField]
	private List<StringGameObjectPair> condition = new List<StringGameObjectPair>();

	[SerializeField]
	private List<StringGameObjectPair> seal = new List<StringGameObjectPair>();

	private Dictionary<string, GameObject> colorDictionary;

	private Dictionary<string, GameObject> logoDictionary;

	private Dictionary<string, GameObject> conditionDictionary;

	private Dictionary<string, GameObject> sealDictionary;

	public TextMeshProUGUI bottomSerial;

	private void Awake()
	{
		InitializeDictionary();
	}

	private void InitializeDictionary()
	{
		colorDictionary = new Dictionary<string, GameObject>();
		foreach (StringGameObjectPair item in color)
		{
			if (!colorDictionary.ContainsKey(item.Key))
			{
				colorDictionary.Add(item.Key, item.Value);
			}
		}
		logoDictionary = new Dictionary<string, GameObject>();
		foreach (StringGameObjectPair item2 in logo)
		{
			if (!logoDictionary.ContainsKey(item2.Key))
			{
				logoDictionary.Add(item2.Key, item2.Value);
			}
		}
		conditionDictionary = new Dictionary<string, GameObject>();
		foreach (StringGameObjectPair item3 in condition)
		{
			if (!conditionDictionary.ContainsKey(item3.Key))
			{
				conditionDictionary.Add(item3.Key, item3.Value);
			}
		}
		sealDictionary = new Dictionary<string, GameObject>();
		foreach (StringGameObjectPair item4 in seal)
		{
			if (!sealDictionary.ContainsKey(item4.Key))
			{
				sealDictionary.Add(item4.Key, item4.Value);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void EnableGameObject(Dictionary<string, GameObject> dict, string key)
	{
		if (dict.ContainsKey(key))
		{
			dict[key].SetActive(value: true);
		}
	}

	public void OnInit(GameItem gameItem)
	{
		if (gameItem.IsTag("PLASMA_CELL_TAG"))
		{
			DisableAll();
			EnableGameObject(colorDictionary, gameItem.GetTagReadonly("PLASMA_COLOR").GetString());
			EnableGameObject(conditionDictionary, gameItem.GetTagReadonly("PLASMA_CASING_CONDITION_CRACK").GetString());
			EnableGameObject(conditionDictionary, gameItem.GetTagReadonly("PLASMA_CASING_CONDITION_WEAR").GetString());
			EnableGameObject(conditionDictionary, gameItem.GetTagReadonly("PLASMA_CASING_CONDITION_UNCOVERED").GetString());
			EnableGameObject(conditionDictionary, gameItem.GetTagReadonly("PLASMA_CASING_CONDITION_REINFORCED").GetString());
			EnableGameObject(sealDictionary, gameItem.GetTagReadonly("PLASMA_SEAL").GetString());
		}
	}

	public void DisableAll()
	{
		foreach (StringGameObjectPair item in color)
		{
			item.Value.SetActive(value: false);
		}
		foreach (StringGameObjectPair item2 in logo)
		{
			item2.Value.SetActive(value: false);
		}
		foreach (StringGameObjectPair item3 in condition)
		{
			item3.Value.SetActive(value: false);
		}
		foreach (StringGameObjectPair item4 in seal)
		{
			item4.Value.SetActive(value: false);
		}
		bottomSerial.text = "";
	}
}

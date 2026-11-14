using System.Collections.Generic;
using UnityEngine;

public class AppHandUI : MonoBehaviour
{
	public List<GameObject> steelHands;

	public List<GameObject> steelAddons;

	public List<GameObject> steelFullInterfaces;

	public List<GameObject> steelSymbolInterfaces;

	public List<GameObject> carbonHands;

	public List<GameObject> carbonAddons;

	public List<GameObject> carbonFullInterfaces;

	public List<GameObject> carbonSymbolInterfaces;

	public List<GameObject> goldHands;

	public List<GameObject> goldAddons;

	public List<GameObject> goldFullInterfaces;

	public List<GameObject> goldSymbolInterfaces;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnInit(GameItem gameItem)
	{
		if (!gameItem.IsTag("HAND_TAG"))
		{
			return;
		}
		DisableAll();
		string @string = gameItem.GetTagReadonly("HAND_MATERIAL").GetString();
		string string2 = gameItem.GetTagReadonly("HAND_INTERFACE").GetString();
		string string3 = gameItem.GetTagReadonly("HAND_INTERFACE_MARKING").GetString();
		string string4 = gameItem.GetTagReadonly("HAND_CONDITION").GetString();
		string string5 = gameItem.GetTagReadonly("HAND_ADDON_ARMORED").GetString();
		string string6 = gameItem.GetTagReadonly("HAND_ADDON_BLADED").GetString();
		List<GameObject> list;
		List<GameObject> list2;
		List<GameObject> list3;
		List<GameObject> list4;
		if (@string == "GOLD_PLATED")
		{
			list = goldHands;
			list2 = goldAddons;
			list3 = goldFullInterfaces;
			list4 = goldSymbolInterfaces;
		}
		else if (@string == "CARBON_COMPOSITE")
		{
			list = carbonHands;
			list2 = carbonAddons;
			list3 = carbonFullInterfaces;
			list4 = carbonSymbolInterfaces;
		}
		else
		{
			list = steelHands;
			list2 = steelAddons;
			list3 = steelFullInterfaces;
			list4 = steelSymbolInterfaces;
		}
		int idx = 0;
		if (string4 == "WEAR")
		{
			idx = 1;
		}
		else if (string4 == "DAMAGE")
		{
			idx = 2;
		}
		EnableAt(list, idx);
		if (string5 == "ARMORED")
		{
			EnableAt(list2, 0);
		}
		if (string6 == "BLADED")
		{
			EnableAt(list2, 1);
		}
		int num = -1;
		switch (string2)
		{
		case "EMG":
			num = 0;
			break;
		case "EMG_PLUS":
			num = 1;
			break;
		case "NEURAL":
			num = 2;
			break;
		}
		if (num >= 0)
		{
			if (string3 == "SYMBOL")
			{
				EnableAt(list4, num);
			}
			else
			{
				EnableAt(list3, num);
			}
		}
	}

	private void EnableAt(List<GameObject> list, int idx)
	{
		if (list != null && idx >= 0 && idx < list.Count && list[idx] != null)
		{
			list[idx].SetActive(value: true);
		}
	}

	private void DisableAll()
	{
		DisableList(steelHands);
		DisableList(steelAddons);
		DisableList(steelFullInterfaces);
		DisableList(steelSymbolInterfaces);
		DisableList(carbonHands);
		DisableList(carbonAddons);
		DisableList(carbonFullInterfaces);
		DisableList(carbonSymbolInterfaces);
		DisableList(goldHands);
		DisableList(goldAddons);
		DisableList(goldFullInterfaces);
		DisableList(goldSymbolInterfaces);
	}

	private void DisableList(List<GameObject> list)
	{
		if (list == null)
		{
			return;
		}
		foreach (GameObject item in list)
		{
			if (item != null)
			{
				item.SetActive(value: false);
			}
		}
	}
}

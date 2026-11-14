using System.Collections.Generic;
using UnityEngine;

public class PhoneBookUIManager : MonoBehaviour
{
	public GameObject layoutGroup;

	public GameObject contactElementPrefab;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OpenUI()
	{
		base.gameObject.SetActive(value: true);
		for (int num = layoutGroup.transform.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(layoutGroup.transform.GetChild(num).gameObject);
		}
		foreach (KeyValuePair<long, StorePhoneClient> item in PlayerStore.Instance.PhoneClientDict)
		{
			if (item.Value.ShownInPhoneBook())
			{
				Object.Instantiate(contactElementPrefab, layoutGroup.transform).GetComponent<ContactElement>().OnInit(item.Key);
			}
		}
	}

	public void CloseUI()
	{
		base.gameObject.SetActive(value: false);
	}
}

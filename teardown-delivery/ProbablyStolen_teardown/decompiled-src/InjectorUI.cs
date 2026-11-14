using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InjectorUI : MonoBehaviour
{
	public TextMeshProUGUI serialTMP;

	public TextMeshProUGUI companyTMP;

	public TextMeshProUGUI modelTMP;

	public List<GameObject> display;

	public List<GameObject> color;

	public List<GameObject> broken;

	public GameObject body;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnInit(GameItem gameItem)
	{
		if (!gameItem.IsTag("INS_INJECTOR_TAG"))
		{
			return;
		}
		DisableAll();
		int @int = gameItem.GetTagReadonly("INJECTOR_INDICATOR").GetInt();
		int int2 = gameItem.GetTagReadonly("INJECTOR_BODY").GetInt();
		int int3 = gameItem.GetTagReadonly("INJECTOR_COLOR").GetInt();
		string @string = gameItem.GetTagReadonly("INJECTOR_COMPANY").GetString();
		string string2 = gameItem.GetTagReadonly("INJECTOR_MODEL").GetString();
		string string3 = gameItem.GetTagReadonly("INJECTOR_SERIAL").GetString();
		if (@int != -1)
		{
			display[@int].SetActive(value: true);
		}
		if (int2 != -1)
		{
			body.SetActive(value: true);
			if (int2 != 0)
			{
				body.SetActive(value: false);
				broken[int2 - 1].SetActive(value: true);
			}
		}
		if (int3 != -1)
		{
			body.SetActive(value: false);
			color[int3].SetActive(value: true);
		}
		serialTMP.text = string3;
		companyTMP.text = @string;
		modelTMP.text = string2;
	}

	public void DisableAll()
	{
		body.SetActive(value: false);
		serialTMP.text = "";
		companyTMP.text = "";
		modelTMP.text = "";
		foreach (GameObject item in display)
		{
			item.SetActive(value: false);
		}
		foreach (GameObject item2 in color)
		{
			item2.SetActive(value: false);
		}
		foreach (GameObject item3 in broken)
		{
			item3.SetActive(value: false);
		}
	}
}

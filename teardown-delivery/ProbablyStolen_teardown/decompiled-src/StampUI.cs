using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StampUI : MonoBehaviour
{
	public TextMeshProUGUI amount;

	public List<GameObject> seals;

	public List<GameObject> letterings;

	public List<GameObject> logos;

	public List<GameObject> chips;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnInit(GameItem gameItem)
	{
		if (gameItem.IsTag("FOOD_STAMP_TAG"))
		{
			DisableAll();
			int @int = gameItem.GetTagReadonly("STAMP_SEAL").GetInt();
			int int2 = gameItem.GetTagReadonly("STAMP_LETTERING").GetInt();
			int int3 = gameItem.GetTagReadonly("STAMP_LOGO").GetInt();
			int int4 = gameItem.GetTagReadonly("STAMP_CHIP").GetInt();
			int int5 = gameItem.GetTagReadonly("STAMP_AMOUNT").GetInt();
			if (@int != -1)
			{
				seals[@int].SetActive(value: true);
			}
			if (int2 != -1)
			{
				letterings[int2].SetActive(value: true);
			}
			if (int3 != -1)
			{
				logos[int3].SetActive(value: true);
			}
			if (int4 != -1)
			{
				chips[int4].SetActive(value: true);
			}
			amount.text = LocHelper.GetLocalizedMechanic("ins_stamp_credits", int5);
		}
	}

	public void DisableAll()
	{
		foreach (GameObject seal in seals)
		{
			seal.SetActive(value: false);
		}
		foreach (GameObject lettering in letterings)
		{
			lettering.SetActive(value: false);
		}
		foreach (GameObject logo in logos)
		{
			logo.SetActive(value: false);
		}
		foreach (GameObject chip in chips)
		{
			chip.SetActive(value: false);
		}
	}
}

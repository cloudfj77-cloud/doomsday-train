using System.Collections.Generic;
using UnityEngine;

public class CigaretteUI : MonoBehaviour
{
	public List<GameObject> seals;

	public List<GameObject> letterings;

	public List<GameObject> logos;

	public List<GameObject> boxes;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnInit(GameItem gameItem)
	{
		if (gameItem.IsTag("CIGARETTE_TAG"))
		{
			DisableAll();
			int @int = gameItem.GetTagReadonly("CIGARETTE_SEAL").GetInt();
			int int2 = gameItem.GetTagReadonly("CIGARETTE_LETTERING").GetInt();
			int int3 = gameItem.GetTagReadonly("CIGARETTE_LOGO").GetInt();
			int int4 = gameItem.GetTagReadonly("CIGARETTE_BOX").GetInt();
			boxes[0].SetActive(value: true);
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
				boxes[int4].SetActive(value: true);
			}
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
		foreach (GameObject box in boxes)
		{
			box.SetActive(value: false);
		}
	}
}

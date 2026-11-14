using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RibwichUI : MonoBehaviour
{
	public GameObject meatNormal;

	public GameObject meatDouble;

	public List<GameObject> topBun;

	public List<GameObject> wrap;

	public List<GameObject> pickles;

	public List<GameObject> davreLogo;

	public List<GameObject> ribwichLogo;

	public GameObject barcode;

	public TextMeshProUGUI barcodeTMP;

	public GameObject facility;

	public TextMeshProUGUI facilityTMP;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnInit(GameItem gameItem)
	{
		if (gameItem.IsTag("RIBWICH_TAG"))
		{
			DisableAll();
			int @int = gameItem.GetTagReadonly("RIBWICH_MEAT_TAG").GetInt();
			int int2 = gameItem.GetTagReadonly("RIBWICH_BUN_TAG").GetInt();
			int int3 = gameItem.GetTagReadonly("RIBWICH_WRAP_TAG").GetInt();
			int int4 = gameItem.GetTagReadonly("RIBWICH_PICKLES_TAG").GetInt();
			int int5 = gameItem.GetTagReadonly("RIBWICH_DAVRE_LOGO_TAG").GetInt();
			int int6 = gameItem.GetTagReadonly("RIBWICH_RIBWICH_LOGO_TAG").GetInt();
			int int7 = gameItem.GetTagReadonly("RIBWICH_FACILITY_NUMBER_TAG").GetInt();
			int int8 = gameItem.GetTagReadonly("RIBWICH_PRODUCTION_MONTH_TAG").GetInt();
			int int9 = gameItem.GetTagReadonly("RIBWICH_PRODUCTION_YEAR_TAG").GetInt();
			if (gameItem.GetTagReadonly("RIBWICH_PRODUCTION_TAG").GetInt() == 0)
			{
				barcode.SetActive(value: true);
			}
			barcodeTMP.text = int8 + "/ \n" + int9;
			if (gameItem.GetTagReadonly("RIBWICH_FACILITY_TAG").GetInt() == 0)
			{
				facility.SetActive(value: true);
			}
			facilityTMP.text = "#" + int7;
			topBun[int2].SetActive(value: true);
			wrap[int3].SetActive(value: true);
			pickles[int4].SetActive(value: true);
			if (int5 >= 0)
			{
				davreLogo[int5].SetActive(value: true);
			}
			if (int6 >= 0)
			{
				ribwichLogo[int6].SetActive(value: true);
			}
			Debug.Log("RibwichUI: Ribwich initialized with meat option " + @int);
			switch (@int)
			{
			case 0:
				meatNormal.SetActive(value: true);
				meatDouble.SetActive(value: false);
				break;
			case 1:
				meatDouble.SetActive(value: true);
				meatNormal.SetActive(value: false);
				DisableList(topBun);
				DisableList(pickles);
				break;
			}
		}
	}

	public static void DisableList(List<GameObject> gameObjects)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			gameObject.SetActive(value: false);
		}
	}

	public void DisableAll()
	{
		meatNormal.SetActive(value: false);
		meatDouble.SetActive(value: false);
		foreach (GameObject item in topBun)
		{
			item.SetActive(value: false);
		}
		foreach (GameObject item2 in wrap)
		{
			item2.SetActive(value: false);
		}
		foreach (GameObject pickle in pickles)
		{
			pickle.SetActive(value: false);
		}
		foreach (GameObject item3 in davreLogo)
		{
			item3.SetActive(value: false);
		}
		foreach (GameObject item4 in ribwichLogo)
		{
			item4.SetActive(value: false);
		}
		barcode.SetActive(value: false);
		facility.SetActive(value: false);
	}
}

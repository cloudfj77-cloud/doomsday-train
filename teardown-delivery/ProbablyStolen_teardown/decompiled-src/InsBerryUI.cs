using UnityEngine;

public class InsBerryUI : MonoBehaviour
{
	public GameObject normal;

	public GameObject under1;

	public GameObject under2;

	public GameObject over1;

	public GameObject mold;

	public GameObject small;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnInit(GameItem gameItem)
	{
		if (!gameItem.IsTag("BERRY_TAG"))
		{
			return;
		}
		DisableAll();
		int @int = gameItem.GetTagReadonly("BERRY_CONDITION_INT").GetInt();
		int int2 = gameItem.GetTagReadonly("BERRY_VISUAL_VARIANT_INT").GetInt();
		switch (@int)
		{
		case -2:
			mold.SetActive(value: true);
			break;
		case -1:
			switch (int2)
			{
			case 1:
				under2.SetActive(value: true);
				break;
			case 2:
				over1.SetActive(value: true);
				break;
			default:
				under1.SetActive(value: true);
				break;
			}
			break;
		case 2:
			small.SetActive(value: true);
			break;
		default:
			normal.SetActive(value: true);
			break;
		}
	}

	public void DisableAll()
	{
		normal.SetActive(value: false);
		under1.SetActive(value: false);
		under2.SetActive(value: false);
		over1.SetActive(value: false);
		mold.SetActive(value: false);
		small.SetActive(value: false);
	}
}

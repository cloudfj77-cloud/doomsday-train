using TMPro;
using UnityEngine;

public class NotebookUIManager : MonoBehaviour
{
	public AudioSource audioSourcePickup;

	public AudioSource audioSourceDrop;

	public AudioSource audioSourceWriting;

	public GameObject panelObject;

	public TextMeshProUGUI itemName;

	public GameObject layoutGroup;

	public NotebookElement notebookElementPrefab;

	public GameObject contrabandType;

	public GameObject medicalType;

	public GameObject houseHoldType;

	public GameObject foodType;

	public GameObject materialType;

	public GameObject weaponType;

	public GameObject waterType;

	private bool updateQueued;

	public static NotebookUIManager Instance { get; private set; }

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

	public void UpdateUI()
	{
		updateQueued = true;
	}

	private void LateUpdate()
	{
		if (updateQueued && panelObject != null && panelObject.activeInHierarchy)
		{
			UpdateNow();
		}
	}

	private void FlushQueuedUpdate()
	{
		if (updateQueued)
		{
			UpdateNow();
		}
	}

	private void UpdateNow()
	{
		updateQueued = false;
		GameItem gameItem = ((PlayerStore.Instance != null) ? PlayerStore.Instance.currentNegociatedItem : null);
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count != 0 && gameItem != null)
		{
			itemName.text = gameItem.GetDisplayName();
			HandleType(gameItem);
			DestroyLayoutGroup();
			{
				foreach (ItemFeature itemFeature in gameItem.itemFeatures)
				{
					if (itemFeature.IsFeatureDiscovered())
					{
						NotebookElement notebookElement = Object.Instantiate(notebookElementPrefab, layoutGroup.transform);
						notebookElement.tmp.text = itemFeature.GetActualDisplay();
						notebookElement.itemFeature = itemFeature;
					}
				}
				return;
			}
		}
		ClearUI();
	}

	public void DestroyLayoutGroup()
	{
		for (int num = layoutGroup.transform.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(layoutGroup.transform.GetChild(num).gameObject);
		}
	}

	public void ClearUI()
	{
		DestroyLayoutGroup();
		itemName.text = "";
		ClearType();
	}

	public void Toggle()
	{
		panelObject.SetActive(!panelObject.activeSelf);
		if (!panelObject.activeSelf)
		{
			audioSourceDrop.Play();
			return;
		}
		FlushQueuedUpdate();
		audioSourcePickup.Play();
	}

	public void CloseUI()
	{
		if (panelObject.activeSelf)
		{
			panelObject.SetActive(value: false);
			audioSourceDrop.Play();
		}
	}

	public void OpenUI()
	{
		panelObject.SetActive(value: true);
		FlushQueuedUpdate();
	}

	public void OnFeatureDiscovered()
	{
		audioSourceWriting.Play();
	}

	public void ClearType()
	{
		contrabandType.SetActive(value: false);
		medicalType.SetActive(value: false);
		houseHoldType.SetActive(value: false);
		foodType.SetActive(value: false);
		materialType.SetActive(value: false);
		weaponType.SetActive(value: false);
		waterType.SetActive(value: false);
	}

	public void HandleType(GameItem gameItem)
	{
		ClearType();
		if (gameItem.GetGameItemType().Contains("CONTRABAND"))
		{
			contrabandType.SetActive(value: true);
		}
		if (gameItem.GetGameItemType().Contains("MEDICAL"))
		{
			medicalType.SetActive(value: true);
		}
		if (gameItem.GetGameItemType().Contains("HOUSEHOLD_GOOD"))
		{
			houseHoldType.SetActive(value: true);
		}
		if (gameItem.GetGameItemType().Contains("FOOD"))
		{
			foodType.SetActive(value: true);
		}
		if (gameItem.GetGameItemType().Contains("MATERIAL"))
		{
			materialType.SetActive(value: true);
		}
		if (gameItem.GetGameItemType().Contains("WEAPON"))
		{
			weaponType.SetActive(value: true);
		}
		if (gameItem.GetGameItemType().Contains("WATER"))
		{
			waterType.SetActive(value: true);
		}
	}
}

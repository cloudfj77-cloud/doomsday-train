using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CommissaryUIManager : MonoBehaviour
{
	public TextMeshProUGUI redeemedCredit;

	public GameObject panel;

	private List<CommissaryElement> commissaryElements;

	public GameObject elementParent;

	public static CommissaryUIManager Instance { get; private set; }

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
		commissaryElements = new List<CommissaryElement>();
	}

	private void Update()
	{
	}

	public void RegisterElement(CommissaryElement element)
	{
		commissaryElements.Add(element);
		RefreshUI();
	}

	public void OpenUI()
	{
		panel.SetActive(value: true);
		RefreshUI();
		EmporiumAudioPlayer.Instance.audioButton1.Play();
	}

	public void CloseUI()
	{
		panel.SetActive(value: false);
		EmporiumAudioPlayer.Instance.audioButton2.Play();
	}

	public void RefreshUI()
	{
		foreach (CommissaryElement commissaryElement in commissaryElements)
		{
			commissaryElement.stock.text = LocHelper.GetLocalizedMechanic("mech_commissary_in_stock", PlayerStore.Instance.commissaryData.GetStock(commissaryElement.id));
			commissaryElement.price.text = LocHelper.GetLocalizedMechanic("mech_commissary_token", PlayerStore.Instance.commissaryData.GetItemData(commissaryElement.id).price);
		}
		redeemedCredit.text = LocHelper.GetLocalizedMechanic("mech_commissary_token", PlayerStore.Instance.commissaryData.redeemedCredit);
	}

	public static bool CanBuy(string id)
	{
		if (PlayerStore.Instance.commissaryData.redeemedCredit < CommissaryData.GetItemDataStatic(id).price)
		{
			return false;
		}
		if (CommissaryData.GetItemDataStatic(id).StockCount <= 0)
		{
			return false;
		}
		return true;
	}

	public void OnRedeemAllButtonClick()
	{
		CommissaryData.RedeemAllStampInInventory();
		RefreshUI();
	}

	public void OnRedeemAllLottery()
	{
		int credit = 0;
		EmporiumEntry.Instance.GetAllOwnedItems().ToList().ForEach(delegate(GameItem item)
		{
			if (item.IsTag("LOTTERY_IS_WINNING_TAG"))
			{
				credit += (int)item.unitValue;
				item.Destroy();
			}
		});
		PlayerStore.Instance.ModCash(credit);
		ReputationUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("mech_commissary_lottery_redeemed", credit));
	}
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommissaryElement : MonoBehaviour
{
	public TextMeshProUGUI productName;

	public TextMeshProUGUI price;

	public TextMeshProUGUI stock;

	public Button buyButton;

	public string id;

	private void Start()
	{
		Register();
	}

	private void Update()
	{
	}

	public void OnBuyClick()
	{
		if (CommissaryUIManager.CanBuy(id))
		{
			PlayerStore.Instance.commissaryData.BuyItem(id);
			CommissaryUIManager.Instance.RefreshUI();
		}
	}

	public void Register()
	{
		CommissaryUIManager.Instance.RegisterElement(this);
	}
}

using UnityEngine;

public class StoreVendingMachine : MonoBehaviour, IClickable
{
	public SpriteRenderer image;

	public static StoreVendingMachine Instance { get; private set; }

	public void OnClick()
	{
		EmporiumEntry.Instance.vendingMachineElement.childItem.contentWindow.Show();
		EmporiumEntry.Instance.vendingMachineElement.childItem.contentWindow.Center();
	}

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

	public void UpdateSprite()
	{
		if (EmporiumEntry.Instance.vendingMachineElement.childItem != null)
		{
			int inventoryCapacity = SpriteHelper.GetInventoryCapacity(VendingMachineHelper.GetVendingInventory(EmporiumEntry.Instance.vendingMachineElement.childItem));
			string spriteNameFromCapacity = VendingMachineHelper.GetSpriteNameFromCapacity("vending", inventoryCapacity);
			Sprite sprite = RenderHandler.LoadFromAtlas("Items/items_vending", spriteNameFromCapacity);
			image.sprite = sprite;
		}
	}
}

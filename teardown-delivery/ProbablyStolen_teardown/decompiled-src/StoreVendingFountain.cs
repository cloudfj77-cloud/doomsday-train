using System.Collections;
using UnityEngine;

public class StoreVendingFountain : MonoBehaviour, IClickable
{
	public SpriteRenderer image;

	public static StoreVendingFountain Instance { get; private set; }

	public void OnClick()
	{
		EmporiumEntry.Instance.vendingFountainElement.childItem.contentWindow.Show();
		EmporiumEntry.Instance.vendingFountainElement.childItem.contentWindow.Center();
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
		StartCoroutine(UpdateSpriteCoroutine());
	}

	private IEnumerator UpdateSpriteCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			UpdateSprite();
		}
	}

	private void Update()
	{
	}

	public void UpdateSprite()
	{
		if (base.gameObject.activeSelf && PlayerStore.Instance.isVendingFountainUnlocked && EmporiumEntry.Instance.vendingFountainElement.childItem != null)
		{
			int totalCapacityPercentage = VendingFountainHelper.GetTotalCapacityPercentage(EmporiumEntry.Instance.vendingFountainElement.childItem);
			string spriteNameFromCapacity = VendingFountainHelper.GetSpriteNameFromCapacity("fountain", totalCapacityPercentage);
			Sprite sprite = RenderHandler.LoadFromAtlas("Items/items_vending", spriteNameFromCapacity);
			image.sprite = sprite;
		}
	}
}

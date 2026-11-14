using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommissaryData
{
	[ES3Serializable]
	public int redeemedCredit;

	[ES3Serializable]
	public List<CommissaryItemData> commissaryItemDatas;

	public const string MODIFIER_RESTOCK_PERCENT_OF_MAX = "commissary_restock_percent_of_max";

	public CommissaryData()
	{
		commissaryItemDatas = new List<CommissaryItemData>();
		CommissaryItemData item = new CommissaryItemData
		{
			identifier = "processed_meat",
			price = 25,
			StockCount = 0,
			restockPerDay = 2,
			maxStock = 12,
			mainType = "FOOD"
		};
		commissaryItemDatas.Add(item);
		CommissaryItemData item2 = new CommissaryItemData
		{
			identifier = "nutrifruit",
			price = 12,
			StockCount = 0,
			restockPerDay = 4,
			maxStock = 20,
			mainType = "FOOD"
		};
		commissaryItemDatas.Add(item2);
		CommissaryItemData item3 = new CommissaryItemData
		{
			identifier = "galaxy_blend",
			price = 25,
			StockCount = 0,
			restockPerDay = 2,
			maxStock = 10,
			mainType = "FOOD"
		};
		commissaryItemDatas.Add(item3);
		CommissaryItemData item4 = new CommissaryItemData
		{
			identifier = "processed_juice",
			price = 14,
			StockCount = 0,
			restockPerDay = 2,
			maxStock = 12,
			mainType = "FOOD"
		};
		commissaryItemDatas.Add(item4);
		CommissaryItemData item5 = new CommissaryItemData
		{
			identifier = "processed_milk",
			price = 16,
			StockCount = 0,
			restockPerDay = 2,
			maxStock = 12,
			mainType = "FOOD"
		};
		commissaryItemDatas.Add(item5);
		CommissaryItemData item6 = new CommissaryItemData
		{
			identifier = "water_ration",
			price = 30,
			StockCount = 0,
			restockPerDay = 2,
			maxStock = 8,
			mainType = "WATER"
		};
		commissaryItemDatas.Add(item6);
		CommissaryItemData item7 = new CommissaryItemData
		{
			identifier = "bandage_item",
			price = 8,
			StockCount = 0,
			restockPerDay = 2,
			maxStock = 8,
			mainType = "MEDICAL"
		};
		commissaryItemDatas.Add(item7);
		RestockTypes();
		RestockTypes();
	}

	public void OnNewDay()
	{
		RestockTypes();
		HandleEventRestock();
	}

	private void RestockTypes()
	{
		HandleTypeRestock("FOOD");
		HandleTypeRestock("BREVAGE");
		HandleTypeRestock("WATER");
		HandleTypeRestock("MEDICAL");
	}

	public void HandleEventRestock()
	{
		float num = AfterhourLocation.Get("commissary")?.GetModifier("commissary_restock_percent_of_max") ?? 0f;
		if (num <= 0f)
		{
			return;
		}
		foreach (CommissaryItemData commissaryItemData in commissaryItemDatas)
		{
			commissaryItemData.StockCount = Mathf.Min(commissaryItemData.maxStock, commissaryItemData.StockCount + Mathf.RoundToInt((float)commissaryItemData.maxStock * num / 100f));
		}
	}

	public void HandleTypeRestock(string type)
	{
		int stockMult = GetStockMult(type);
		if (stockMult < 0)
		{
			Debug.Log("Restocking " + type + " with a mult of " + stockMult);
		}
		foreach (CommissaryItemData commissaryItemData in commissaryItemDatas)
		{
			if (commissaryItemData.mainType.Equals(type))
			{
				commissaryItemData.StockCount += commissaryItemData.restockPerDay * stockMult;
				if (commissaryItemData.StockCount < 0)
				{
					commissaryItemData.StockCount = 0;
				}
				if (commissaryItemData.StockCount > commissaryItemData.maxStock)
				{
					commissaryItemData.StockCount = commissaryItemData.maxStock;
				}
			}
		}
	}

	public int GetStockMult(string type)
	{
		int totalTypeMarketPriceModifier = GetTotalTypeMarketPriceModifier(type);
		int result = 1;
		if (totalTypeMarketPriceModifier == 0)
		{
			return result;
		}
		if (totalTypeMarketPriceModifier > 0)
		{
			if (totalTypeMarketPriceModifier < 33)
			{
				return -2;
			}
			if (totalTypeMarketPriceModifier < 66)
			{
				return -3;
			}
			return -5;
		}
		if (totalTypeMarketPriceModifier > -33)
		{
			return 2;
		}
		if (totalTypeMarketPriceModifier > -66)
		{
			return 3;
		}
		return 4;
	}

	public static int GetTotalTypeMarketPriceModifier(string type)
	{
		int num = 0;
		foreach (StoreEvent activeEvent in StoreStation.Instance.storeEventManager.GetActiveEvents())
		{
			for (int i = 0; i < activeEvent.negociationDatas.Count; i++)
			{
				if (activeEvent.negociationDatas[i].itemType.Equals(type))
				{
					num += activeEvent.negociationDatas[i].value;
				}
			}
		}
		return num;
	}

	public void BuyItem(string id)
	{
		CommissaryItemData itemData = GetItemData(id);
		if (itemData != null && redeemedCredit >= itemData.price && itemData.StockCount > 0)
		{
			redeemedCredit -= itemData.price;
			itemData.StockCount--;
			EmporiumEntry.Instance.TryAddToAfterhourInv(DirectoryMaster.Item(id));
			NegociationUIManager.Instance.audioSourceCashRegister.Play();
		}
	}

	public int GetStock(string id)
	{
		foreach (CommissaryItemData commissaryItemData in commissaryItemDatas)
		{
			if (commissaryItemData.identifier.Equals(id))
			{
				return commissaryItemData.StockCount;
			}
		}
		return 0;
	}

	public CommissaryItemData GetItemData(string id)
	{
		foreach (CommissaryItemData commissaryItemData in commissaryItemDatas)
		{
			if (commissaryItemData.identifier.Equals(id))
			{
				return commissaryItemData;
			}
		}
		return null;
	}

	public static CommissaryItemData GetItemDataStatic(string id)
	{
		foreach (CommissaryItemData commissaryItemData in PlayerStore.Instance.commissaryData.commissaryItemDatas)
		{
			if (commissaryItemData.identifier.Equals(id))
			{
				return commissaryItemData;
			}
		}
		return null;
	}

	public static void RedeemAllStampInInventory()
	{
		EmporiumEntry.Instance.GetAllOwnedItems().ToList().ForEach(delegate(GameItem item)
		{
			if (item.IsTag("FOOD_STAMP_TAG"))
			{
				item.ActivateSlotItem();
			}
		});
	}
}

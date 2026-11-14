using System.Collections.Generic;
using System.Linq;

public class SoldItem
{
	public string itemId;

	public string itemName;

	public int itemUniqueId;

	public string clientId;

	public string clientName;

	public string clientSprite;

	public string factionId;

	public bool isPersonalUse;

	public bool isClientAddict;

	public int soldPrice;

	public bool CanReturn = true;

	public bool CanReturnAsBuyer = true;

	public bool alwaysFeedback;

	public int feedbackLevel;

	public static void TrackItem(GameItem gameItem, StoreClient storeClient, int soldPrice)
	{
		if (gameItem == null || storeClient == null)
		{
			Debug.LogError("DSError: TrackItem received null parameter");
			return;
		}
		Debug.Log("tracked price: " + (int)gameItem.GetNegociatedValue());
		SoldItem soldItem = new SoldItem
		{
			itemId = gameItem.identifier,
			itemName = gameItem.name,
			itemUniqueId = gameItem.uniqueId,
			clientId = storeClient.identifier,
			clientName = storeClient.displayName,
			clientSprite = storeClient.spriteName,
			factionId = storeClient.clientFaction,
			isPersonalUse = storeClient.IsPersonalConsumption,
			isClientAddict = storeClient.IsClientAddict,
			soldPrice = soldPrice,
			CanReturn = storeClient.CanReturn,
			CanReturnAsBuyer = storeClient.CanReturnAsBuyer,
			alwaysFeedback = storeClient.alwaysFeedback
		};
		if (storeClient.isAug)
		{
			soldItem.CanReturn = false;
			soldItem.CanReturnAsBuyer = false;
		}
		MarkStolenOrContrabandSale(gameItem);
		MoveToSoldBag(gameItem);
		PlayerStore.Instance.soldItemsTracker.Add(soldItem);
	}

	private static void MoveToSoldBag(GameItem gameItem)
	{
		GameGridInventory soldElement = EmporiumEntry.Instance.soldElement;
		if (gameItem.parentInventory != soldElement)
		{
			gameItem.parentInventory?.Expel(gameItem);
			if (!soldElement.UncheckedAccept(gameItem))
			{
				Debug.LogError("DSError: sold item could not be moved to the sold bag: " + gameItem.name);
			}
		}
	}

	public static void MarkStolenOrContrabandSale(GameItem gameItem)
	{
		if (gameItem == null)
		{
			return;
		}
		List<GameItem> list = new List<GameItem> { gameItem };
		foreach (GameItem item in gameItem.FindAllChildItems())
		{
			if (!item.IsTag("ITEM_BEHIND_LOCK_TAG"))
			{
				list.Add(item);
			}
		}
		foreach (GameItem item2 in list)
		{
			if (item2.IsTag("STOLEN_TAG") || item2.IsTag("CONTRABAND_ITEM_TAG"))
			{
				PlayerStore.Instance.soldStolenOrContrabandThisRun = true;
				break;
			}
		}
	}

	public static void HandleSoldItem()
	{
		foreach (SoldItem item in PlayerStore.Instance.soldItemsTracker)
		{
			GameItem gameItem = item.GetGameItem();
			if (gameItem == null)
			{
				Debug.LogWarning("HandleSoldItem found null item");
			}
			else if (ChemicalFeedbackHelper.CanReport(gameItem, item))
			{
				PlayerStore.Instance.secData.CheckSoldChem(gameItem, item);
			}
		}
		var (list, list2) = SelectEligibleChemicalItem();
		if (list.Count != list2.Count)
		{
			Debug.LogError("DSError Chem Feedback Error: eligibleChemicalItems count does not match relatedItems count");
		}
		for (int i = 0; i < list.Count; i++)
		{
			SoldItem soldItem = list[i];
			List<SoldItem> list3 = list2[i];
			int num = 0;
			bool flag = soldItem.alwaysFeedback;
			foreach (SoldItem item2 in list3)
			{
				int num2 = item2.feedbackLevel * 8;
				num += num2;
				if (RNG.Roll(num2))
				{
					Debug.Log($"ROLLED SUCCESS: Client {item2.clientName} item {item2.itemName} (% Chance: {num2})");
					flag = true;
				}
			}
			if (flag)
			{
				Debug.Log("Client " + soldItem.clientName + " chemical feedback: " + soldItem.itemName);
				ChemicalFeedbackHelper.GenerateReaction(soldItem.GetGameItem(), soldItem, list2[i]);
			}
			else
			{
				Debug.Log($"Client {soldItem.clientName} no reaction generated. Total Roll Chance: {num}%");
			}
		}
		PlayerStore.Instance.soldItemsTracker.Clear();
		for (int num3 = EmporiumEntry.Instance.soldElement.childItems.Count - 1; num3 >= 0; num3--)
		{
			EmporiumEntry.Instance.soldElement.childItems[num3]?.Destroy();
		}
	}

	public static (List<SoldItem>, List<List<SoldItem>>) SelectEligibleChemicalItem()
	{
		IEnumerable<IGrouping<string, SoldItem>> enumerable = from s in PlayerStore.Instance.soldItemsTracker.Where(delegate(SoldItem s)
			{
				GameItem gameItem = s.GetGameItem();
				return gameItem != null && gameItem.IsTag("CHEMICAL_INTENT_TAG") && s.isPersonalUse && !s.factionId.Equals("FACTION_TOURIST") && s.CanReturn;
			}).ToList()
			group s by s.clientId;
		List<SoldItem> list = new List<SoldItem>();
		List<List<SoldItem>> list2 = new List<List<SoldItem>>();
		foreach (IGrouping<string, SoldItem> item in enumerable)
		{
			SoldItem soldItem = null;
			int num = 0;
			List<SoldItem> list3 = new List<SoldItem>();
			foreach (SoldItem item2 in item)
			{
				int num2 = (item2.feedbackLevel = ChemicalFeedbackHelper.GetItemFeedbackLevel(item2.GetGameItem()));
				if (num2 > num)
				{
					num = num2;
					soldItem = item2;
				}
				if (num2 > 0)
				{
					list3.Add(item2);
				}
			}
			if (soldItem != null && num > 0)
			{
				list.Add(soldItem);
				list2.Add(list3);
			}
		}
		return (list, list2);
	}

	public GameItem GetGameItem()
	{
		GameItem itemFromSoldElement = EmporiumEntry.Instance.GetItemFromSoldElement(itemUniqueId);
		if (itemFromSoldElement == null)
		{
			Debug.LogWarning("GetGameItem found null item for sold item: " + itemName);
		}
		return itemFromSoldElement;
	}
}

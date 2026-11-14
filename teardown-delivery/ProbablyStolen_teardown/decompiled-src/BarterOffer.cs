using System;
using System.Collections.Generic;

public class BarterOffer
{
	public Action onInstance;

	public StoreClient ownerClient;

	public string barterTextBody;

	public List<BarterCondition> conditions = new List<BarterCondition>();

	public Func<BarterResult> getBarterResult;

	public string acceptedResponse = "";

	public string deniedResponse = "";

	public bool isFreeReward;

	public List<Action> offerSets = new List<Action>();

	public int currentSetIndex;

	public BarterOffer()
	{
		barterTextBody = "";
	}

	public bool UseOfferSets()
	{
		if (offerSets != null)
		{
			return offerSets.Count > 0;
		}
		return false;
	}

	public int OfferSetCount()
	{
		if (offerSets != null)
		{
			return offerSets.Count;
		}
		return 0;
	}

	public void AddOfferSet(Action set)
	{
		offerSets.Add(set);
	}

	public void ShowCurrentSet()
	{
		if (UseOfferSets())
		{
			EmporiumEntry.Instance.ClearFrontInvNonOwned();
			EmporiumEntry.Instance.ClearBackInvNonOwned();
			offerSets[currentSetIndex]?.Invoke();
		}
	}

	public void ShowSetByOffset(int offset)
	{
		if (UseOfferSets())
		{
			int count = offerSets.Count;
			currentSetIndex = ((currentSetIndex + offset) % count + count) % count;
			ShowCurrentSet();
		}
	}

	public void InstanciateOffer()
	{
		StoreClient storeClient = PlayerStore.Instance.currentClientInstance?.GetClientBlueprint();
		if (storeClient == null || (ownerClient != null && storeClient != ownerClient))
		{
			return;
		}
		if (UseOfferSets())
		{
			currentSetIndex = 0;
			ShowCurrentSet();
			PlayerStore.Instance.currentClientInstance.GetClientBlueprint().isIntroduced = true;
			if (isFreeReward)
			{
				NegociationUIManager.Instance.OpenUI();
			}
		}
		else if (onInstance != null)
		{
			onInstance();
			PlayerStore.Instance.currentClientInstance.GetClientBlueprint().isIntroduced = true;
		}
		else
		{
			Debug.Log("Warning, BarterOffer.onInstance was Null");
		}
	}

	public bool UseCondition()
	{
		if (conditions.Count == 0)
		{
			return false;
		}
		return true;
	}

	public string GetItemPlacedResponse()
	{
		if (isFreeReward)
		{
			return "";
		}
		if (UseCondition())
		{
			if (IsConditionAcceptabled())
			{
				if (acceptedResponse.Equals(""))
				{
					return LocHelper.GetLocalizedDialogue("dialog_barter_accept");
				}
				return acceptedResponse;
			}
			if (deniedResponse.Equals(""))
			{
				return LocHelper.GetLocalizedDialogue("dialog_barter_reject");
			}
			return deniedResponse;
		}
		return getBarterResult().response;
	}

	public bool IsConditionAcceptabled()
	{
		List<GameItem> ownedItemsBack = EmporiumEntry.Instance.GetOwnedItemsBack();
		List<GameItem> list = new List<GameItem>();
		foreach (BarterCondition condition in conditions)
		{
			bool flag = false;
			for (int i = 0; i < ownedItemsBack.Count; i++)
			{
				GameItem gameItem = ownedItemsBack[i];
				if (!list.Contains(gameItem) && condition.IsItemValid(gameItem))
				{
					list.Add(gameItem);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsBarterAcceptable()
	{
		if (isFreeReward)
		{
			return true;
		}
		if (UseCondition())
		{
			return IsConditionAcceptabled();
		}
		return getBarterResult().isAccepted;
	}

	public void OnBarterAccepted()
	{
		if (isFreeReward)
		{
			return;
		}
		if (UseCondition())
		{
			List<GameItem> ownedItemsBack = EmporiumEntry.Instance.GetOwnedItemsBack();
			List<GameItem> list = new List<GameItem>();
			foreach (BarterCondition condition in conditions)
			{
				for (int i = 0; i < ownedItemsBack.Count; i++)
				{
					GameItem gameItem = ownedItemsBack[i];
					if (!list.Contains(gameItem) && condition.IsItemValid(gameItem))
					{
						list.Add(gameItem);
						break;
					}
				}
			}
			PlayerStore.Instance.OnItemsSold(list);
		}
		else
		{
			PlayerStore.Instance.OnItemsSold(EmporiumEntry.Instance.backInvinvElement.childItems);
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

public class StoreRequest
{
	[ES3Serializable]
	public string title;

	[ES3Serializable]
	public string id;

	[ES3Serializable]
	public string description;

	[ES3Serializable]
	public string factionId;

	[ES3Serializable]
	public string requestedItem;

	[ES3Serializable]
	public bool useCustomItemName;

	[ES3Serializable]
	public string requesterName;

	[ES3Serializable]
	public int duration;

	[ES3Serializable]
	public string requiredItem;

	[ES3Serializable]
	public int requiredItemCount = 1;

	[ES3Serializable]
	public int creditReward;

	[ES3Serializable]
	public int reputationReward;

	[ES3Serializable]
	public bool isUnique;

	[ES3Serializable]
	public string sourceEventId = "";

	public static void StartDay()
	{
		if ((PlayerStore.Instance.skipIntro || StoreStation.GetDayCounter() > 8) && NetworkUpgrade.IsMarketplaceRequestsUnlocked())
		{
			if (StoreReputation.IsPerkUnlocked("REV_REQUEST") && StoreStation.Instance.dayCounter % 7 == 5 && RNG.Roll(80))
			{
				ActivateRequest(GetRandomRevRequest());
			}
			if (StoreReputation.IsPerkUnlocked("REV_REQUEST") && StoreStation.Instance.dayCounter % 7 == 1 && StartingPerk.IsRevolutionSympathizer())
			{
				ActivateRequest(GetRandomRevRequest());
			}
			_ = PlayerStore.Instance.devMode;
			HandleUpperLevelRequest();
			HandleLowerLevelRequest();
			HandleMinorScriptedRequest();
		}
	}

	public StoreRequest()
	{
		factionId = "";
		requesterName = "";
	}

	public static void HandleUpperLevelRequest()
	{
		int uLReputation = StoreReputation.GetULReputation();
		int chance = 8;
		if (uLReputation < -25)
		{
			chance = 4;
		}
		if (uLReputation >= 25)
		{
			chance = 12;
		}
		if (RNG.Roll(chance))
		{
			if (RNG.Roll(75))
			{
				ActivateRequest(StoreRequestList.ULRareItemRequest());
			}
			else
			{
				ActivateRequest(StoreRequestList.ULRareItemCharityRequest());
			}
		}
	}

	public static void HandleLowerLevelRequest(bool force = false)
	{
		int lLReputation = StoreReputation.GetLLReputation();
		int chance = 4;
		if (lLReputation < -25)
		{
			chance = 2;
		}
		if (lLReputation >= 25)
		{
			chance = 6;
		}
		if (force)
		{
			chance = 100;
		}
		if (RNG.Roll(chance))
		{
			ActivateRequest(StoreRequestList.LowerWorkerStayAwakeRequest());
		}
	}

	public static void HandleMinorScriptedRequest()
	{
		if (!PlayerStore.Instance.endlessMode)
		{
			if (StoreStation.GetDayCounter() + PlayerStore.Instance.marketplaceUnlockedDay == 4)
			{
				ActivateRequest(StoreRequestList.MotherGalaxyBlend());
			}
			if (StoreStation.GetDayCounter() + PlayerStore.Instance.marketplaceUnlockedDay == 6)
			{
				ActivateRequest(StoreRequestList.ButcherCleaver());
			}
			if (StoreStation.GetDayCounter() + PlayerStore.Instance.marketplaceUnlockedDay == 8)
			{
				ActivateRequest(StoreRequestList.LunSnackBar());
			}
			if (StoreStation.GetDayCounter() + PlayerStore.Instance.marketplaceUnlockedDay == 10)
			{
				ActivateRequest(StoreRequestList.LandlordSpecial());
			}
			if (StoreStation.GetDayCounter() + PlayerStore.Instance.marketplaceUnlockedDay == 14)
			{
				ActivateRequest(StoreRequestList.SawyerBeer());
			}
			if (StoreStation.GetDayCounter() + PlayerStore.Instance.marketplaceUnlockedDay == 18)
			{
				ActivateRequest(StoreRequestList.EnergyDrinkJackson());
			}
			if (StoreStation.GetDayCounter() + PlayerStore.Instance.marketplaceUnlockedDay == 22)
			{
				ActivateRequest(StoreRequestList.LandlordSpecial2());
			}
			if (StoreStation.GetDayCounter() + PlayerStore.Instance.marketplaceUnlockedDay == 26)
			{
				ActivateRequest(StoreRequestList.CrowbarSawyer());
			}
			if (StoreStation.GetDayCounter() + PlayerStore.Instance.marketplaceUnlockedDay == 31)
			{
				ActivateRequest(StoreRequestList.LunRetirementParty());
			}
			if (StoreStation.GetDayCounter() + PlayerStore.Instance.marketplaceUnlockedDay == 36)
			{
				ActivateRequest(StoreRequestList.LandlordSpecial3());
			}
			if (StoreStation.GetDayCounter() + PlayerStore.Instance.marketplaceUnlockedDay == 42)
			{
				ActivateRequest(StoreRequestList.UpperCitizenPetMeat());
			}
		}
	}

	public static void ActivateRequest(StoreRequest storeRequest, bool force = false)
	{
		if (CanActivateRequest(storeRequest) || force)
		{
			PlayerStore.Instance.storeRequests.Add(storeRequest);
			if (storeRequest.isUnique)
			{
				PlayerStore.Instance.seenRequest.Add(storeRequest.id);
			}
			PCUIManager.Instance.storePC.SetState(2);
		}
	}

	public static bool IsRequestActive(StoreRequest storeRequest)
	{
		foreach (StoreRequest storeRequest2 in PlayerStore.Instance.storeRequests)
		{
			if (storeRequest2.id.Equals(storeRequest.id))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CanActivateRequest(StoreRequest storeRequest)
	{
		if (!NetworkUpgrade.IsMarketplaceRequestsUnlocked())
		{
			return false;
		}
		if (storeRequest.isUnique && PlayerStore.Instance.seenRequest.Contains(storeRequest.id))
		{
			return false;
		}
		if (IsRequestActive(storeRequest))
		{
			return false;
		}
		return true;
	}

	public bool CanFillRequest()
	{
		List<GameItem> allOwnedItems = EmporiumEntry.Instance.GetAllOwnedItems();
		int num = 0;
		foreach (GameItem item in allOwnedItems)
		{
			if (CanThisItemFillRequest(item))
			{
				num++;
			}
		}
		if (num >= requiredItemCount)
		{
			return true;
		}
		return false;
	}

	public bool CanThisItemFillRequest(GameItem item)
	{
		if (item.identifier.Equals(requiredItem))
		{
			if (item.IsTag("ITEM_BEHIND_LOCK_TAG"))
			{
				return false;
			}
			if (item.IsTag("LOCKED_CONTAINER_TAG") && !item.GetTagReadonly("LOCK_STATUS").GetBool())
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public void FillRequest()
	{
		List<GameItem> allOwnedItems = EmporiumEntry.Instance.GetAllOwnedItems();
		int num = 0;
		for (int num2 = allOwnedItems.Count - 1; num2 >= 0; num2--)
		{
			if (CanThisItemFillRequest(allOwnedItems[num2]))
			{
				CheckDeliveredItem(allOwnedItems[num2]);
				allOwnedItems[num2].Destroy();
				num++;
				if (num >= requiredItemCount)
				{
					DispenseReward();
					PlayerStore.Instance.storeRequests.Remove(this);
					break;
				}
			}
		}
	}

	private void CheckDeliveredItem(GameItem item)
	{
		int paidValuePerItem = GetPaidValuePerItem(item);
		VendingMachineHelper.TrackMislabelledRequestItem(item, paidValuePerItem, factionId);
		SoldItem.MarkStolenOrContrabandSale(item);
		PlayerStore.Instance.secData.CheckDeliveredStolen(item, paidValuePerItem, factionId);
		foreach (GameItem item2 in item.FindAllChildItems())
		{
			if (!item2.IsTag("ITEM_BEHIND_LOCK_TAG"))
			{
				PlayerStore.Instance.secData.CheckDeliveredStolen(item2, VendingMachineHelper.GetVendingPrice(item2), factionId);
			}
		}
	}

	private int GetPaidValuePerItem(GameItem item)
	{
		int num = creditReward / Mathf.Max(1, requiredItemCount);
		if (num <= 0)
		{
			return VendingMachineHelper.GetVendingPrice(item);
		}
		return num;
	}

	public void DispenseReward()
	{
		StoreReputation.ModReputation(factionId, reputationReward, notify: false);
		PlayerStore.Instance.ModCash(creditReward);
	}

	public void OnDayStart()
	{
		duration--;
	}

	public string GetFactionDisplayName()
	{
		return StoreReputation.GetFactionNameByID(factionId);
	}

	public static StoreRequest GetRandomRevRequest()
	{
		return RNG.GetRandomElement(new List<StoreRequest>
		{
			StoreRequestList.RevOxymore(),
			StoreRequestList.RevScrapMetal(),
			StoreRequestList.RevSecCard(),
			StoreRequestList.RevPeat()
		});
	}

	public void SetCreditReward(int creditAmount, float multiplier)
	{
		creditReward = (int)((float)creditAmount * multiplier * (float)requiredItemCount);
	}
}

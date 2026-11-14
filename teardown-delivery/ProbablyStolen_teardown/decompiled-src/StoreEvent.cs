using System;
using System.Collections.Generic;
using System.Linq;

public class StoreEvent
{
	public enum EventType
	{
		NORMALE,
		THREAT,
		COSMETIC,
		INNATE
	}

	public enum EventArea
	{
		UPPER,
		LOWER,
		ALL
	}

	[ES3Serializable]
	public EventType eventType;

	[ES3Serializable]
	public EventArea eventArea;

	[ES3Serializable]
	public string identifier;

	[ES3Serializable]
	public string newsName;

	[ES3Serializable]
	public string displayName = "";

	[ES3Serializable]
	public string newsDescription;

	[ES3Serializable]
	public int cooldown = 3;

	[ES3Serializable]
	public int duration;

	[ES3Serializable]
	public int totalDuration;

	[ES3Serializable]
	public int startDay = -1;

	[ES3Serializable]
	public int currentDay;

	[ES3Serializable]
	public bool isPermanent;

	[ES3Serializable]
	public bool isHidden;

	[ES3Serializable]
	public bool isDurationVisible;

	[ES3Serializable]
	public List<NegociationData> negociationDatas;

	public Action AddClientFromEvent;

	[ES3Serializable]
	public string onUpdateActionId;

	[ES3Serializable]
	public string onActiveActionId;

	[ES3Serializable]
	public string onRemovedActionId;

	[ES3Serializable]
	public string addClientFromEventActionId;

	[ES3Serializable]
	public int importance;

	[ES3Serializable]
	public bool IsPriority;

	[ES3Serializable]
	public List<string> imcompatibleEvents;

	[ES3Serializable]
	public bool IsKnown;

	[ES3Serializable]
	public bool isHiddenFromNewsPaper;

	[ES3Serializable]
	public bool isFactionOperation;

	[ES3Serializable]
	public string sourceFaction = "";

	[ES3Serializable]
	public int civilUnrestMod;

	[ES3Serializable]
	public int upperLevelFriendlinessMod;

	[ES3Serializable]
	public int secPowerMod;

	[ES3Serializable]
	public int revPowerMod;

	[ES3Serializable]
	public int bmPowerMod;

	[ES3Serializable]
	public bool isLockDown;

	[ES3Serializable]
	public bool isBlockByLockDown;

	public StoreEvent()
	{
		newsName = "UNDEFINED";
		duration = 1;
		isPermanent = false;
		isHidden = false;
		isDurationVisible = false;
		imcompatibleEvents = new List<string>();
		negociationDatas = new List<NegociationData>();
	}

	public void OnEventActive()
	{
		totalDuration = duration;
		startDay = StoreStation.Instance.dayCounter;
		List<StoreRequest> storeRequests = PlayerStore.Instance.storeRequests;
		int num = storeRequests?.Count ?? 0;
		if (!string.IsNullOrEmpty(onActiveActionId) && StoreEventActionDict.actions.TryGetValue(onActiveActionId, out var value))
		{
			value?.Invoke();
		}
		if (storeRequests != null)
		{
			for (int i = num; i < storeRequests.Count; i++)
			{
				storeRequests[i].sourceEventId = identifier;
			}
		}
		if (!isLockDown)
		{
			return;
		}
		foreach (StoreEvent item in StoreStation.Instance.storeEventManager.GetActiveEvents().ToList())
		{
			if (item.isBlockByLockDown)
			{
				StoreStation.Instance.storeEventManager.DeactivateEvent(item);
			}
		}
	}

	public void OnDayStart()
	{
		currentDay++;
		if (!string.IsNullOrEmpty(onUpdateActionId) && StoreEventActionDict.actions.TryGetValue(onUpdateActionId, out var value))
		{
			value?.Invoke();
		}
	}

	public void OnDayEnd()
	{
		if (!isPermanent)
		{
			duration--;
		}
	}

	public void OnEventRemove()
	{
		if (!string.IsNullOrEmpty(onRemovedActionId) && StoreEventActionDict.actions.TryGetValue(onRemovedActionId, out var value))
		{
			value?.Invoke();
		}
		StoreStation.Instance.storeEventManager.ApplyCooldownAfterEnd(this);
	}

	public void PrintEvent()
	{
		Debug.Log(newsName);
	}

	public bool IsItemAffectedByThisEvent(GameItem gameItem)
	{
		foreach (NegociationData negociationData in negociationDatas)
		{
			if (negociationData.IsNegociationDataAffectItem(gameItem))
			{
				return true;
			}
		}
		return false;
	}

	private string GetSlipEventName()
	{
		if (!string.IsNullOrEmpty(displayName))
		{
			return displayName;
		}
		return newsName;
	}

	private string BuildSlipBody(int daysUntil)
	{
		if (daysUntil == 0)
		{
			return LocHelper.GetLocalizedMechanic("mech_intel_slip_body_today", GetSlipEventName(), duration);
		}
		return LocHelper.GetLocalizedMechanic("mech_intel_slip_body_days", GetSlipEventName(), daysUntil, duration);
	}

	public GameItem CreateSlip(int timer)
	{
		GameItem gameItem = DirectoryMaster.Item("slip");
		gameItem.name = LocHelper.GetLocalizedMechanic("mech_intel_slip_name");
		gameItem.ModifyTag("SLIP_TITLE", delegate(TagState state)
		{
			state.Enable().SetString(LocHelper.GetLocalizedMechanic("mech_intel_slip_title"));
		});
		gameItem.ModifyTag("SLIP_BODY", delegate(TagState state)
		{
			state.Enable().SetString(BuildSlipBody(timer));
		});
		gameItem.ModifyTag("SLIP_DATA", delegate(TagState state)
		{
			state.Enable().SetInt(timer);
		});
		gameItem.onCycleEndSlotItemFunc = delegate(GameItem curItem, GameInventory _, SlotMarker _)
		{
			int dueDate = curItem.GetTagReadonly("SLIP_DATA").GetInt() - 1;
			if (dueDate < 0)
			{
				curItem.Destroy();
			}
			else
			{
				curItem.ModifyTag("SLIP_DATA", delegate(TagState state)
				{
					state.Enable().SetInt(dueDate);
				});
				curItem.ModifyTag("SLIP_BODY", delegate(TagState state)
				{
					state.Enable().SetString(BuildSlipBody(dueDate));
				});
			}
		};
		return gameItem;
	}

	public StoreEvent CreateAndAddSlip(int timer)
	{
		PlayerStore.Instance.AddDirectSellingItemToTable(CreateSlip(timer), isOwend: true);
		return this;
	}

	public void InitNormalEvent()
	{
		eventType = EventType.NORMALE;
		eventArea = EventArea.ALL;
		addClientFromEventActionId = identifier + "addClientAction";
	}

	public void InitOperation(string faction)
	{
		isFactionOperation = true;
		sourceFaction = faction;
	}

	public string GetDisplayName()
	{
		if (!string.IsNullOrEmpty(displayName))
		{
			return displayName;
		}
		return newsName;
	}

	public string GetEventPriceEffect()
	{
		string text = "";
		foreach (NegociationData negociationData in negociationDatas)
		{
			if (negociationData.value != 0)
			{
				if (negociationData.value > 0)
				{
					text += "+";
				}
				text = text + negociationData.value + "% " + TypeHelper.GetTypeDisplayName(negociationData.itemType) + "\n";
			}
		}
		return text;
	}

	public bool IsSurplus()
	{
		int num = 0;
		foreach (NegociationData negociationData in negociationDatas)
		{
			num += negociationData.value;
		}
		return num > 0;
	}

	public bool IsShortage()
	{
		int num = 0;
		foreach (NegociationData negociationData in negociationDatas)
		{
			num += negociationData.value;
		}
		return num < 0;
	}
}

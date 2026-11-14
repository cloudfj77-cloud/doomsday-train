using System.Collections.Generic;

public class Expedition
{
	public string id;

	public Hireling hireling;

	public ExpeditionLocation expeditionLocation;

	public List<string> logs;

	public int turn;

	public int lookAtTurn;

	public List<ExpeditionPoi> spottedPoi;

	public Expedition(Hireling hireling, ExpeditionLocation expeditionLocation)
	{
		this.hireling = hireling;
		this.expeditionLocation = expeditionLocation;
		logs = new List<string>();
		spottedPoi = new List<ExpeditionPoi>();
	}

	public void ExecuteExpedition()
	{
		StartExpedition();
		for (int i = 0; i < turn; i++)
		{
			ContinueExpedition();
		}
		EndExpedition();
	}

	public void DisplayDebugLog()
	{
		Debug.Log("Expedition Log for " + hireling.displayName + " at " + expeditionLocation.displayName);
		for (int i = 0; i < logs.Count; i++)
		{
			Debug.Log(logs[i]);
		}
	}

	public void EndExpedition()
	{
		Log("Expedition ended.");
		hireling.status = Hireling.HirelingStatus.Complete;
	}

	public void StartExpedition()
	{
		Log("Arrived at " + expeditionLocation.displayName);
		InitTurn();
	}

	public void ContinueExpedition()
	{
		if (CanLookAround())
		{
			LookAround();
			return;
		}
		ExpeditionPoi expeditionPoi = ChoosePoiToExplore();
		if (expeditionPoi != null)
		{
			VisitPOI(expeditionPoi);
		}
	}

	public void InitTurn()
	{
		turn = 12;
	}

	public void ExpeditionTryAdd(GraphNodeStorage storage, GameItem item)
	{
		GameItem expeditionBackpack = ExpeditionHelper.GetExpeditionBackpack(hireling.id);
		if (expeditionBackpack != null && GraphUtils.CanAccept(expeditionBackpack, item))
		{
			GraphUtils.TryAcceptAll(expeditionBackpack, item);
		}
		else if (GraphUtils.CanAccept(storage, item))
		{
			GraphUtils.TryAcceptAll(storage, item);
		}
		else
		{
			Log("Could not carry " + item.name + ", got no space left.");
		}
	}

	public List<GameItem> GetAllItems()
	{
		return new List<GameItem>();
	}

	public bool CanLookAround()
	{
		if (spottedPoi.Count == 0 || lookAtTurn < turn - 2)
		{
			return true;
		}
		return false;
	}

	public void LookAround()
	{
		int randomInt = RNG.GetRandomInt(2, 4);
		List<ExpeditionPoi> list = new List<ExpeditionPoi>();
		for (int i = 0; i < randomInt; i++)
		{
			ExpeditionPoi expeditionPoi = expeditionLocation.PullPOI();
			if (expeditionPoi != null)
			{
				list.Add(expeditionPoi);
			}
		}
		string text = "spotted ";
		for (int j = 0; j < list.Count; j++)
		{
			text += list[j].displayName;
			if (j < list.Count - 1)
			{
				text += ", ";
			}
		}
		Log("Looking around... " + text);
		spottedPoi.AddRange(list);
		lookAtTurn = turn;
	}

	public void FoundItem(GameItem item)
	{
		if (item != null)
		{
			Log("Found " + item.name);
			ExpeditionTryAdd(ExpeditionHelper.GetHirelingEquipment(hireling.id), item);
		}
	}

	public ExpeditionPoi ChoosePoiToExplore()
	{
		if (spottedPoi.Count == 0)
		{
			return null;
		}
		spottedPoi.Sort((ExpeditionPoi a, ExpeditionPoi b) => b.GetRewardRiskFactor().CompareTo(a.GetRewardRiskFactor()));
		return spottedPoi[0];
	}

	public void VisitPOI(ExpeditionPoi poi)
	{
		Log(poi.actionDisplay);
		if (RNG.Roll(poi.successChance))
		{
			GameItem item = poi.PullLoot();
			FoundItem(item);
			spottedPoi.Remove(poi);
		}
		else
		{
			ExpeditionAccident.ActivateAccident(this);
		}
	}

	public void Log(string message)
	{
		logs.Add(message);
	}

	public GameItem GeneratorLogItem()
	{
		GameItem gameItem = ItemSpawner.Spawn("expedition_log");
		gameItem.ModifyTag("LOG_CONTENT_TAG", delegate(TagState state)
		{
			state.SetString(string.Join("/", logs.ToArray()));
		});
		return gameItem;
	}

	public bool CanContinue()
	{
		if (hireling.health <= 0)
		{
			return false;
		}
		return true;
	}
}

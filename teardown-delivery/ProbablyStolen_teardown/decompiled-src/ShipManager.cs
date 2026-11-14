using System.Collections.Generic;

public class ShipManager
{
	private readonly ShipScene shipScene;

	public GameItem ship { get; set; }

	public EventData currentEvent { get; set; }

	public bool hasEventRun { get; set; }

	public bool isTransit { get; set; }

	public int transitETA { get; set; }

	public bool isDocked { get; set; }

	public OverworldPOI nextPOI { get; set; }

	public ShipManager(ShipScene shipScene)
	{
		this.shipScene = shipScene;
	}

	public ShipManager StartTransit(int eta, OverworldPOI nextPOI)
	{
		GameMaster.current.raidManager.ClearStation();
		shipScene.StartShip(ship);
		currentEvent?.ForceEnd();
		currentEvent = null;
		OverlayHandler.current.ClearEventLog();
		hasEventRun = false;
		isTransit = true;
		transitETA = eta;
		isDocked = false;
		this.nextPOI = nextPOI;
		OverlayHandler.current.CreateNav(NavigationHelper.ScanForEncounters());
		SaveManager.Save();
		if (transitETA <= 0)
		{
			EndTransit();
		}
		return this;
	}

	public ShipManager EndTransit()
	{
		isTransit = false;
		if (nextPOI is OverworldStation overworldStation)
		{
			if (overworldStation.generatedStation == null)
			{
				overworldStation.PrepareEncounter();
			}
			GameMaster.current.raidManager.InitStation(overworldStation.generatedStation);
		}
		return this;
	}

	public ShipManager SetEvent(EventData eventData)
	{
		if (currentEvent != null)
		{
			currentEvent.ForceEnd();
		}
		currentEvent = eventData;
		currentEvent?.ValidateState();
		hasEventRun = true;
		OverlayHandler.current.PushEvent();
		return this;
	}

	public ShipManager StartShip(GameItem ship)
	{
		this.ship = ship;
		shipScene.StartShip(ship);
		return this;
	}

	public ShipManager Dock()
	{
		shipScene.Reset();
		GameMaster.current.raidManager.ResumeRaid();
		isDocked = true;
		return this;
	}

	public ShipManager Undock()
	{
		GameMaster.current.raidManager.LeaveRaid();
		shipScene.StartShip(ship);
		isDocked = false;
		return this;
	}

	public bool IsShipHaveItem(string identifier, int quantity = 1)
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(ship);
		int num = 0;
		foreach (GameItem item in list)
		{
			if (item.identifier.Equals(identifier))
			{
				num++;
				if (num >= quantity)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool RemoveItemFromShip(string identifier, int quantity = 1)
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(ship);
		int num = 0;
		foreach (GameItem item in list)
		{
			if (item.identifier.Equals(identifier))
			{
				num++;
				item.Destroy();
				if (num >= quantity)
				{
					return true;
				}
			}
		}
		return false;
	}

	public int GetShipMiningCapacity()
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(ship);
		int num = 0;
		foreach (GameItem item in list)
		{
			if (item.IsTag("MINING_DRILL_SYSTEM"))
			{
				num += item.GetTagReadonly("CURRENT_MINING_POWER_INT").GetInt();
			}
		}
		return num;
	}

	public int GetShipSalvageCapacity()
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(ship);
		int num = 0;
		foreach (GameItem item in list)
		{
			if (item.IsTag("SALVAGE_GANTRY_SYSTEM"))
			{
				num += item.GetTagReadonly("CURRENT_SALVAGE_POWER_INT").GetInt();
			}
		}
		return num;
	}

	public bool DamageHull(int amount)
	{
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(ship))
		{
			if (item.IsTag("SYSTEM_TAG_HULL"))
			{
				return HullHelper.DamageHull(item, amount);
			}
		}
		return false;
	}

	public int CheckMissingHullPoint()
	{
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(ship))
		{
			if (item.IsTag("SYSTEM_TAG_HULL"))
			{
				return HullHelper.GetMissingHull(item);
			}
		}
		return 0;
	}

	public void RepairFullHull(bool juryRigged)
	{
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(ship))
		{
			if (item.IsTag("SYSTEM_TAG_HULL"))
			{
				if (juryRigged)
				{
					HullHelper.RepairHull(item, 999);
					HullHelper.RemoveMaxHull(item, 1);
				}
				else
				{
					HullHelper.RepairHull(item, 999);
				}
			}
		}
	}

	public (bool, int, int, int) DamageHullVerbose(int amount)
	{
		foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(ship))
		{
			if (item2.IsTag("SYSTEM_TAG_HULL"))
			{
				int @int = item2.GetTagReadonly("HULL_INTEGRITY_INT").GetInt();
				int int2 = item2.GetTagReadonly("HULL_ARMOR_INT").GetInt();
				int num = 0;
				if (amount > int2)
				{
					num = amount - int2;
				}
				bool item = HullHelper.DamageHull(item2, num);
				@int = item2.GetTagReadonly("HULL_INTEGRITY_INT").GetInt();
				return (item, @int, int2, num);
			}
		}
		return (false, 0, 0, 0);
	}

	public int GetTotalAvailableEnergyCredit()
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(ship);
		int num = 0;
		foreach (GameItem item in list)
		{
			if (item.IsTag("power_source_item"))
			{
				num += item.GetTagReadonly("power_source_item_energy").GetInt();
			}
		}
		return num;
	}

	public int GetTotalAvailableFuel()
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(ship);
		int num = 0;
		foreach (GameItem item in list)
		{
			if (item.IsTag("EXOTIC_FUEL_TAG"))
			{
				num += item.unitCount;
			}
		}
		return num;
	}

	public bool RemoveExoticFuel(int amountToRemove)
	{
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(ship))
		{
			if (item.IsTag("EXOTIC_FUEL_TAG"))
			{
				if (amountToRemove < item.unitCount)
				{
					item.unitCount -= amountToRemove;
					return true;
				}
				amountToRemove -= item.unitCount;
				item.Destroy();
			}
		}
		return false;
	}

	public bool RemoveEnergyCredit(int amountToRemove)
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(ship);
		List<GameItem> list2 = new List<GameItem>();
		foreach (GameItem item in list)
		{
			if (item.IsTag("power_source_item"))
			{
				list2.Add(item);
			}
		}
		int num = 0;
		foreach (GameItem item2 in list2)
		{
			if (item2.IsTag("BREEDER_POWER_SOURCE_ITEM_TAG"))
			{
				num = PowerHelper.GetAvailableEnergyFromItem(item2, useBandwidth: false);
				if (amountToRemove <= num)
				{
					PowerHelper.TryRemoveEnergy(item2, amountToRemove, useBandwidth: false);
					return true;
				}
				PowerHelper.TryRemoveEnergy(item2, num, useBandwidth: false);
				amountToRemove -= num;
			}
		}
		foreach (GameItem item3 in list2)
		{
			if (!item3.IsTag("BREEDER_POWER_SOURCE_ITEM_TAG"))
			{
				num = PowerHelper.GetAvailableEnergyFromItem(item3, useBandwidth: false);
				if (amountToRemove <= num)
				{
					PowerHelper.TryRemoveEnergy(item3, amountToRemove, useBandwidth: false);
					return true;
				}
				PowerHelper.TryRemoveEnergy(item3, num, useBandwidth: false);
				amountToRemove -= num;
			}
		}
		return false;
	}

	public List<GameCharacterItem> GetCrew()
	{
		List<GameCharacterItem> list = GraphUtils.FindAllChildrenType<GameCharacterItem>(ship);
		list.RemoveAll((GameCharacterItem s) => s.isDead);
		return list;
	}

	public bool IsShipArmed()
	{
		foreach (GameCharacterItem item in GraphUtils.FindAllChildrenType<GameCharacterItem>(ship))
		{
			if (item.IsTag("SYSTEM_TAG_WEAPON"))
			{
				return true;
			}
		}
		return false;
	}

	public (int, int) InspectCargoHold()
	{
		List<GameItem> list = ContrabandHelper.FindAllContrabandItem(ship);
		int count = list.Count;
		int contrabandValue = ContrabandHelper.GetContrabandValue(list);
		if (ContrabandHelper.CheckIfContrabandWasFound(ship, contrabandValue))
		{
			ContrabandHelper.DisposeContraband(list);
			return (count, contrabandValue);
		}
		return (0, 0);
	}
}

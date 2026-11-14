using System.Collections.Generic;
using System.Linq;

public class TurnManager
{
	private readonly RaidManager raidManager;

	private readonly ShipManager shipManager;

	private readonly CombatEventHandler eventHandler;

	public const int MINUTES_PER_TURN = 20;

	public const int MINUTES_PER_HOUR = 100;

	public const int HOURS_PER_DAY = 10;

	public const int DAYS_PER_MONTH = 10;

	public const int MONTHS_PER_YEAR = 10;

	public int START_YEAR = 2501;

	public int START_DAY;

	public int START_MONTH = 4;

	public int turns { get; private set; }

	public bool isProcessingTurn { get; private set; }

	public TurnManager(ShipManager shipManager, RaidManager raidManager)
	{
		this.raidManager = raidManager;
		this.shipManager = shipManager;
		ResetTurns();
	}

	public int GetMinutes()
	{
		return turns * 20;
	}

	public int GetHours()
	{
		return GetMinutes() / 100;
	}

	public int GetDays()
	{
		return GetHours() / 10 + START_DAY;
	}

	public int GetMonths()
	{
		return GetDays() / 10 + START_MONTH;
	}

	public int GetYears()
	{
		return GetMonths() / 10 + START_YEAR;
	}

	public string GetMinutesFormatted()
	{
		int num = GetMinutes() % 100;
		if (num >= 10)
		{
			return num.ToString();
		}
		return "0" + num;
	}

	public string GetHoursFormatted()
	{
		return (GetHours() % 10).ToString();
	}

	public string GetDaysFormatted()
	{
		return (GetDays() % 10 + 1).ToString();
	}

	public string GetMonthsFormatted()
	{
		return (GetMonths() % 10 + 1).ToString();
	}

	public string GetYearsFormatted()
	{
		return GetYears().ToString();
	}

	public void ResetTurns()
	{
		turns = 0;
	}

	private void NextTurnRaid()
	{
		turns++;
		raidManager.currentRoom.turns++;
		raidManager.actionPointsRemaining = 100;
		PowerHelper.HandlePower();
		PowerHelper.HandlePower(isFromShip: false);
		raidManager.GetAllGameItem().ForEach(delegate(GameItem i)
		{
			i.onTurnEndSlotItemFunc?.Invoke(i, i.parentInventory, i.GetIncompleteSlotInInventory());
		});
		if (!raidManager.IsInCombat())
		{
			raidManager.GetAllCharacters().ForEach(delegate(GameCharacterItem c)
			{
				c.OnTurnStart();
				c.EntityTurnDone();
			});
		}
		else
		{
			raidManager.GetAllCharactersNotHere().ForEach(delegate(GameCharacterItem c)
			{
				c.OnTurnStart();
				c.EntityTurnDone();
			});
		}
	}

	private void NextTurnShip()
	{
		turns++;
		PowerHelper.HandlePower();
		if (raidManager.station != null)
		{
			PowerHelper.HandlePower(isFromShip: false);
		}
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(shipManager.ship);
		List<GameCharacterItem> list2 = GraphUtils.FindAllChildrenType<GameCharacterItem>(shipManager.ship);
		if (raidManager.station != null)
		{
			list.AddRange(raidManager.GetAllGameItem());
			list2.AddRange(raidManager.GetAllCharacters());
		}
		list.Distinct().ToList().ForEach(delegate(GameItem i)
		{
			i.onTurnEndSlotItemFunc?.Invoke(i, i.parentInventory, i.GetIncompleteSlotInInventory());
		});
		list2.Distinct().ToList().ForEach(delegate(GameCharacterItem c)
		{
			c.HandlePerkCrafty();
			c.OnTurnStart();
			c.EntityTurnDone();
		});
	}

	public void NextTurn()
	{
		isProcessingTurn = true;
		if (shipManager.currentEvent != null)
		{
			shipManager.currentEvent.IncrementTurn();
		}
		if (shipManager.isTransit)
		{
			shipManager.transitETA--;
			if (shipManager.transitETA <= 0)
			{
				shipManager.EndTransit();
			}
		}
		if (GameMaster.current.InRaid())
		{
			NextTurnRaid();
		}
		else
		{
			NextTurnShip();
		}
		EventTriggerManager.current.CheckEventTriggers();
		isProcessingTurn = false;
	}

	public void NextNTurns(int n)
	{
		for (int i = 0; i < n; i++)
		{
			NextTurn();
		}
	}
}

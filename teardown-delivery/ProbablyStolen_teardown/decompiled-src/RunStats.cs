using System;
using System.Collections.Generic;
using UnityEngine;

public class RunStats
{
	public int seriesStartDay = -1;

	public List<int> netWorthByDay = new List<int>();

	public List<int> creditsByDay = new List<int>();

	public int totalSales;

	public int itemsSold;

	public int stockSpent;

	public int salesToday;

	public int bestDaySales;

	public int bestDay = -1;

	public string bestFlipItem = "";

	public int bestFlipBought;

	public int bestFlipSold;

	public int bestFlipDay = -1;

	public string biggestSaleItem = "";

	public string biggestSaleClient = "";

	public int biggestSaleValue;

	public int biggestSaleDay = -1;

	public int vendingIncome;

	public int fountainIncome;

	public int rentPaid;

	public int rentWeeksPaid;

	public int rentWeeksMissed;

	public int powerSpent;

	public int janitorialSpent;

	public int insuranceSpent;

	public int wildNetworkSpent;

	public int wildUpgradesBought;

	public int closestCallCredits = -1;

	public int closestCallDay = -1;

	public bool closestCallMortgage;

	public int storeBoughtDay = -1;

	public int storePrice;

	public bool storeBoughtWithMortgage;

	public int mortgageTakenDay = -1;

	public int mortgageDownPayment;

	public int mortgageWeeklyPayment;

	public int mortgageWeeks;

	public int mortgageWeeksPaid;

	public int mortgagePaid;

	public int permitBoughtDay = -1;

	public int permitPrice;

	public int permitBribes;

	public int customersServed;

	public int georgioVisits;

	public int georgioBoughtFrom;

	public int georgioLent;

	public int georgioLentDay = -1;

	public int georgioPaidBack;

	public int georgioPaidBackDay = -1;

	public int scavengeAttempts;

	public int scavengedValue;

	public int injuriesMinor;

	public int injuriesMajor;

	public string bestFindItem = "";

	public int bestFindValue;

	public int bestFindDay = -1;

	public int drJacksonSpent;

	public int drJacksonMachinesBought;

	public int inspections;

	public int inspectionsPassed;

	public int goodsSeized;

	public int inspectorBribes;

	public int burglaries;

	public int burglaryLosses;

	public int worstBurglaryValue;

	public int worstBurglaryDay = -1;

	public int wrongfulReports;

	public int augsShot;

	public static RunStats Current
	{
		get
		{
			if (!PlayerStore.HasInstance)
			{
				return null;
			}
			PlayerStore instance = PlayerStore.Instance;
			if (instance.runStats == null)
			{
				instance.runStats = new RunStats();
			}
			return instance.runStats;
		}
	}

	private static int Today
	{
		get
		{
			if (StoreStation.Instance == null)
			{
				return 0;
			}
			return StoreStation.Instance.dayCounter;
		}
	}

	public static void RecordDay(int day, int netWorth, int credits)
	{
		RunStats current = Current;
		if (current == null)
		{
			return;
		}
		if (current.seriesStartDay < 0 || current.netWorthByDay.Count == 0)
		{
			current.seriesStartDay = day;
			current.netWorthByDay.Clear();
			current.creditsByDay.Clear();
		}
		int num = day - current.seriesStartDay;
		if (num >= 0)
		{
			while (current.netWorthByDay.Count < num)
			{
				current.netWorthByDay.Add(current.netWorthByDay[current.netWorthByDay.Count - 1]);
				current.creditsByDay.Add(current.creditsByDay[current.creditsByDay.Count - 1]);
			}
			if (num < current.netWorthByDay.Count)
			{
				current.netWorthByDay[num] = netWorth;
				current.creditsByDay[num] = credits;
			}
			else
			{
				current.netWorthByDay.Add(netWorth);
				current.creditsByDay.Add(credits);
			}
		}
	}

	public static void OnDayEnded(int day, int netWorth, int credits)
	{
		RunStats current = Current;
		if (current != null)
		{
			RecordDay(day, netWorth, credits);
			if (current.salesToday > current.bestDaySales)
			{
				current.bestDaySales = current.salesToday;
				current.bestDay = day;
			}
			current.salesToday = 0;
		}
	}

	public static void OnRunEnded(int day, int netWorth, int credits)
	{
		RunStats current = Current;
		if (current != null)
		{
			RecordDay(day, netWorth, credits);
			if (current.salesToday > current.bestDaySales)
			{
				current.bestDaySales = current.salesToday;
				current.bestDay = day;
			}
		}
	}

	public static void OnItemSold(GameItem item, int cost, int boughtFor, StoreClient client)
	{
		RunStats current = Current;
		if (current != null && item != null && cost > 0 && (client == null || !(client.identifier == "businessPermitOfficial")))
		{
			current.totalSales += cost;
			current.itemsSold++;
			current.salesToday += cost;
			string displayName = item.GetDisplayName();
			if (boughtFor > 0 && cost - boughtFor > current.bestFlipSold - current.bestFlipBought)
			{
				current.bestFlipItem = displayName;
				current.bestFlipBought = boughtFor;
				current.bestFlipSold = cost;
				current.bestFlipDay = Today;
			}
			if (client != null && cost > current.biggestSaleValue)
			{
				current.biggestSaleItem = displayName;
				current.biggestSaleClient = client.displayName;
				current.biggestSaleValue = cost;
				current.biggestSaleDay = Today;
			}
		}
	}

	public static void OnItemBought(GameItem item, int cost, StoreClient client)
	{
		RunStats current = Current;
		if (current == null || item == null || cost <= 0)
		{
			return;
		}
		if (MapUIManager.Instance != null && MapUIManager.Instance.Current != null && MapUIManager.Instance.Current.Id == "dr_jackson")
		{
			current.drJacksonSpent += cost;
			if (item.IsGameItemType("MACHINE") || item.IsGameItemType("MODULE") || item.IsGameItemType("NODE"))
			{
				current.drJacksonMachinesBought++;
			}
		}
		else
		{
			current.stockSpent += cost;
		}
		if (client != null && IsGeorgio(client.identifier))
		{
			current.georgioBoughtFrom += cost;
		}
	}

	public static bool IsGeorgio(string clientId)
	{
		if (!string.IsNullOrEmpty(clientId))
		{
			return clientId.StartsWith("georgio", StringComparison.Ordinal);
		}
		return false;
	}

	public static void OnCustomerArrived(StoreClient client)
	{
		RunStats current = Current;
		if (current != null && client != null)
		{
			current.customersServed++;
			if (IsGeorgio(client.identifier))
			{
				current.georgioVisits++;
			}
		}
	}

	public static void OnVendingSale(int value)
	{
		RunStats current = Current;
		if (current != null && value > 0)
		{
			current.vendingIncome += value;
		}
	}

	public static void OnFountainSale(int value)
	{
		RunStats current = Current;
		if (current != null && value > 0)
		{
			current.fountainIncome += value;
		}
	}

	public static void OnRentPaid(int amount, int creditsLeft)
	{
		RunStats current = Current;
		if (current != null)
		{
			current.rentPaid += amount;
			current.rentWeeksPaid++;
			OnPaymentMade(current, creditsLeft, mortgage: false);
		}
	}

	public static void OnRentMissed()
	{
		RunStats current = Current;
		if (current != null)
		{
			current.rentWeeksMissed++;
		}
	}

	private static void OnPaymentMade(RunStats s, int creditsLeft, bool mortgage)
	{
		if (creditsLeft >= 0 && (s.closestCallCredits < 0 || creditsLeft < s.closestCallCredits))
		{
			s.closestCallCredits = creditsLeft;
			s.closestCallDay = Today;
			s.closestCallMortgage = mortgage;
		}
	}

	public static void OnServicePaid(string serviceId, int cost)
	{
		RunStats current = Current;
		if (current != null && cost > 0)
		{
			switch (serviceId)
			{
			case "POWER_SERVICE":
				current.powerSpent += cost;
				break;
			case "JANITORIAL_SERVICE":
				current.janitorialSpent += cost;
				break;
			case "INSURANCE_SERVICE":
				current.insuranceSpent += cost;
				break;
			}
		}
	}

	public static void OnWildUpgradeBought(int cashSpent)
	{
		RunStats current = Current;
		if (current != null)
		{
			current.wildUpgradesBought++;
			if (cashSpent > 0)
			{
				current.wildNetworkSpent += cashSpent;
			}
		}
	}

	public static void OnPermitBought(int price)
	{
		RunStats current = Current;
		if (current != null)
		{
			current.permitBoughtDay = Today;
			current.permitPrice = price;
		}
	}

	public static void OnPermitBribe(int value)
	{
		RunStats current = Current;
		if (current != null && value > 0)
		{
			current.permitBribes += value;
		}
	}

	public static void OnStoreBoughtWithCash(int price)
	{
		RunStats current = Current;
		if (current != null)
		{
			current.storeBoughtDay = Today;
			current.storePrice = price;
			current.storeBoughtWithMortgage = false;
		}
	}

	public static void OnMortgageTaken(int price, int downPayment, int weeklyPayment, int weeks)
	{
		RunStats current = Current;
		if (current != null)
		{
			current.mortgageTakenDay = Today;
			current.storePrice = price;
			current.mortgageDownPayment = downPayment;
			current.mortgageWeeklyPayment = weeklyPayment;
			current.mortgageWeeks = weeks;
			current.mortgageWeeksPaid = 0;
			current.mortgagePaid = downPayment;
		}
	}

	public static void OnMortgagePayment(int amount, int creditsLeft)
	{
		RunStats current = Current;
		if (current != null)
		{
			current.mortgageWeeksPaid++;
			current.mortgagePaid += amount;
			OnPaymentMade(current, creditsLeft, mortgage: true);
		}
	}

	public static void OnMortgagePaidOff()
	{
		RunStats current = Current;
		if (current != null)
		{
			current.storeBoughtDay = Today;
			current.storeBoughtWithMortgage = true;
		}
	}

	public static void OnGeorgioLoan(int amount)
	{
		RunStats current = Current;
		if (current != null && amount > 0)
		{
			current.georgioLent += amount;
			current.georgioLentDay = Today;
		}
	}

	public static void OnGeorgioPayback(int amount)
	{
		RunStats current = Current;
		if (current != null && amount > 0)
		{
			current.georgioPaidBack += amount;
			current.georgioPaidBackDay = Today;
		}
	}

	public static void OnScavengeAttempt()
	{
		RunStats current = Current;
		if (current != null)
		{
			current.scavengeAttempts++;
		}
	}

	public static void OnScavengeInjury(bool major)
	{
		RunStats current = Current;
		if (current != null)
		{
			if (major)
			{
				current.injuriesMajor++;
			}
			else
			{
				current.injuriesMinor++;
			}
		}
	}

	public static void OnScavengedItems(List<GameItem> items)
	{
		RunStats current = Current;
		if (current == null || items == null)
		{
			return;
		}
		foreach (GameItem item in items)
		{
			if (item != null)
			{
				int num = Mathf.Max(0, (int)item.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L));
				current.scavengedValue += num;
				if (num > current.bestFindValue)
				{
					current.bestFindValue = num;
					current.bestFindItem = item.GetDisplayName();
					current.bestFindDay = Today;
				}
			}
		}
	}

	public static void OnInspection(bool passed, int seizedValue)
	{
		RunStats current = Current;
		if (current != null)
		{
			current.inspections++;
			if (passed)
			{
				current.inspectionsPassed++;
			}
			if (seizedValue > 0)
			{
				current.goodsSeized += seizedValue;
			}
		}
	}

	public static void OnInspectorBribe(int amount)
	{
		RunStats current = Current;
		if (current != null && amount > 0)
		{
			current.inspectorBribes += amount;
		}
	}

	public static void OnBurglary(int value)
	{
		RunStats current = Current;
		if (current != null)
		{
			current.burglaries++;
			current.burglaryLosses += Mathf.Max(0, value);
			if (value > current.worstBurglaryValue)
			{
				current.worstBurglaryValue = value;
				current.worstBurglaryDay = Today;
			}
		}
	}

	public static void OnWrongfulReport()
	{
		RunStats current = Current;
		if (current != null)
		{
			current.wrongfulReports++;
		}
	}

	public static void OnAugShot()
	{
		RunStats current = Current;
		if (current != null)
		{
			current.augsShot++;
		}
	}
}

using System;

public class StoreDate
{
	public int day;

	public int month;

	public int year;

	private static StoreDate today;

	public StoreDate(int day, int month, int year)
	{
		this.day = day;
		this.month = month;
		this.year = year;
	}

	public static StoreDate GetDateToday()
	{
		return today;
	}

	public static void SetDateToday(StoreDate storeDate)
	{
		today = storeDate;
	}

	public static void AdvanceDay()
	{
		DateTime dateTime = new DateTime(today.year, today.month, today.day).AddDays(1.0);
		today.day = dateTime.Day;
		today.month = dateTime.Month;
		today.year = dateTime.Year;
	}

	public static StoreDate GetRandomDateInFuture(int minDays, int maxDays)
	{
		DateTime dateTime = new DateTime(today.year, today.month, today.day);
		int randomInt = RNG.GetRandomInt(minDays, maxDays);
		DateTime dateTime2 = dateTime.AddDays(randomInt);
		return new StoreDate(dateTime2.Day, dateTime2.Month, dateTime2.Year);
	}

	public static StoreDate GetRandomDateInPast(int minDays, int maxDays)
	{
		DateTime dateTime = new DateTime(today.year, today.month, today.day);
		int randomInt = RNG.GetRandomInt(minDays, maxDays);
		DateTime dateTime2 = dateTime.AddDays(-randomInt);
		return new StoreDate(dateTime2.Day, dateTime2.Month, dateTime2.Year);
	}
}

using Unity.Services.Analytics;

public class WantedNotReportedEvent : Event
{
	public string CustomerId
	{
		set
		{
			SetParameter("customer_id", value);
		}
	}

	public int Day
	{
		set
		{
			SetParameter("day", value);
		}
	}

	public int RunNumber
	{
		set
		{
			SetParameter("run_number", value);
		}
	}

	public string RunId
	{
		set
		{
			SetParameter("run_id", value);
		}
	}

	public int CurrentCredit
	{
		set
		{
			SetParameter("current_credit", value);
		}
	}

	public WantedNotReportedEvent()
		: base("wanted_not_reported")
	{
	}
}

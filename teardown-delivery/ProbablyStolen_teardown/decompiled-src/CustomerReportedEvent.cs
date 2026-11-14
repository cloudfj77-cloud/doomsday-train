using Unity.Services.Analytics;

public class CustomerReportedEvent : Event
{
	public string CustomerId
	{
		set
		{
			SetParameter("customer_id", value);
		}
	}

	public string ReportType
	{
		set
		{
			SetParameter("report_type", value);
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

	public CustomerReportedEvent()
		: base("customer_reported")
	{
	}
}

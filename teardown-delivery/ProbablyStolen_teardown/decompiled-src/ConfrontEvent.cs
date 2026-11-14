using Unity.Services.Analytics;

public class ConfrontEvent : Event
{
	public string ItemId
	{
		set
		{
			SetParameter("item_id", value);
		}
	}

	public string Outcome
	{
		set
		{
			SetParameter("outcome", value);
		}
	}

	public string ConfrontType
	{
		set
		{
			SetParameter("confront_type", value);
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

	public string Spec
	{
		set
		{
			SetParameter("spec", value);
		}
	}

	public float TotalPlayTimeSeconds
	{
		set
		{
			SetParameter("total_play_time_seconds", value);
		}
	}

	public float RunPlayTimeSeconds
	{
		set
		{
			SetParameter("run_play_time_seconds", value);
		}
	}

	public ConfrontEvent()
		: base("confront")
	{
	}
}

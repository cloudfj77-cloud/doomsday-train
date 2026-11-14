using Unity.Services.Analytics;

public class GameExitEvent : Event
{
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

	public int CurrentCredit
	{
		set
		{
			SetParameter("current_credit", value);
		}
	}

	public int DaysSinceInspectionLoss
	{
		set
		{
			SetParameter("days_since_inspection_loss", value);
		}
	}

	public int LastInspectionLossValue
	{
		set
		{
			SetParameter("last_inspection_loss_value", value);
		}
	}

	public int DaysSinceTheft
	{
		set
		{
			SetParameter("days_since_theft", value);
		}
	}

	public int LastTheftValue
	{
		set
		{
			SetParameter("last_theft_value", value);
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

	public GameExitEvent()
		: base("game_exit")
	{
	}
}

using Unity.Services.Analytics;

public class OnSleepEvent : Event
{
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

	public int EstimatedTotalWealth
	{
		set
		{
			SetParameter("estimated_total_wealth", value);
		}
	}

	public string Spec
	{
		set
		{
			SetParameter("spec", value);
		}
	}

	public string RunId
	{
		set
		{
			SetParameter("run_id", value);
		}
	}

	public string AfterHourActivity
	{
		set
		{
			SetParameter("after_hour_activity", value);
		}
	}

	public int RepLowerLevel
	{
		set
		{
			SetParameter("rep_lower_level", value);
		}
	}

	public int RepUpperLevel
	{
		set
		{
			SetParameter("rep_upper_level", value);
		}
	}

	public int RepSecurity
	{
		set
		{
			SetParameter("rep_security", value);
		}
	}

	public int RepBlackMarket
	{
		set
		{
			SetParameter("rep_black_market", value);
		}
	}

	public int RepRevolution
	{
		set
		{
			SetParameter("rep_revolution", value);
		}
	}

	public int RepTourist
	{
		set
		{
			SetParameter("rep_tourist", value);
		}
	}

	public int RepChurch
	{
		set
		{
			SetParameter("rep_church", value);
		}
	}

	public int RepCartel
	{
		set
		{
			SetParameter("rep_cartel", value);
		}
	}

	public float RunPlayTimeSeconds
	{
		set
		{
			SetParameter("run_play_time_seconds", value);
		}
	}

	public float TotalPlayTimeSeconds
	{
		set
		{
			SetParameter("total_play_time_seconds", value);
		}
	}

	public bool IsInsuranceActive
	{
		set
		{
			SetParameter("is_insurance_active", value);
		}
	}

	public bool IsAlarmActive
	{
		set
		{
			SetParameter("is_alarm_active", value);
		}
	}

	public OnSleepEvent()
		: base("on_sleep")
	{
	}
}

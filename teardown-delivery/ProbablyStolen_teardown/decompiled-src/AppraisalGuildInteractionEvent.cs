using Unity.Services.Analytics;

public class AppraisalGuildInteractionEvent : Event
{
	public string Action
	{
		set
		{
			SetParameter("appraisal_action", value);
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

	public AppraisalGuildInteractionEvent()
		: base("appraisal_guild_interaction")
	{
	}
}

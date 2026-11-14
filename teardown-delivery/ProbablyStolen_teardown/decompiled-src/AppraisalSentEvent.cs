using Unity.Services.Analytics;

public class AppraisalSentEvent : Event
{
	public string ItemId
	{
		set
		{
			SetParameter("item_id", value);
		}
	}

	public int ActualValue
	{
		set
		{
			SetParameter("appraisal_actual_value", value);
		}
	}

	public int SentValue
	{
		set
		{
			SetParameter("appraisal_sent_value", value);
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

	public AppraisalSentEvent()
		: base("appraisal_sent")
	{
	}
}

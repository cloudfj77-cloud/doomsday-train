using Unity.Services.Analytics;

public class ItemOfInterestAcquisitionEvent : Event
{
	public string RunId
	{
		set
		{
			SetParameter("run_id", value);
		}
	}

	public string MachineId
	{
		set
		{
			SetParameter("machine_id", value);
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

	public int MachineryBoughtCount
	{
		set
		{
			SetParameter("machinery_bought_count", value);
		}
	}

	public float RunPlayTimeSeconds
	{
		set
		{
			SetParameter("run_play_time_seconds", value);
		}
	}

	public int CurrentCredit
	{
		set
		{
			SetParameter("current_credit", value);
		}
	}

	public ItemOfInterestAcquisitionEvent()
		: base("item_of_interest_acquisition")
	{
	}
}

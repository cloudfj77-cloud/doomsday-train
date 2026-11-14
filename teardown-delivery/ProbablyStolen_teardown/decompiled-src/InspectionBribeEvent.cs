using Unity.Services.Analytics;

public class InspectionBribeEvent : Event
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

	public bool HasSecurityGrace
	{
		set
		{
			SetParameter("has_security_grace", value);
		}
	}

	public int BribeAmount
	{
		set
		{
			SetParameter("bribe_amount", value);
		}
	}

	public InspectionBribeEvent()
		: base("inspection_bribe")
	{
	}
}

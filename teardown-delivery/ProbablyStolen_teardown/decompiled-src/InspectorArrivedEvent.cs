using Unity.Services.Analytics;

public class InspectorArrivedEvent : Event
{
	public string InspectorType
	{
		set
		{
			SetParameter("inspector_type", value);
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

	public bool IsFromShowcase
	{
		set
		{
			SetParameter("is_from_showcase", value);
		}
	}

	public bool HasSecurityGrace
	{
		set
		{
			SetParameter("has_security_grace", value);
		}
	}

	public InspectorArrivedEvent()
		: base("inspector_arrived")
	{
	}
}

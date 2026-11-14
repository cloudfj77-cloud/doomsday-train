using Unity.Services.Analytics;

public class OnInspectedEvent : Event
{
	public string OutcomeType
	{
		set
		{
			SetParameter("outcome_type", value);
		}
	}

	public bool HasSmugglerBay
	{
		set
		{
			SetParameter("has_smuggler_bay", value);
		}
	}

	public int TotalLostItemValue
	{
		set
		{
			SetParameter("total_lost_item_value", value);
		}
	}

	public int TotalLostItemCount
	{
		set
		{
			SetParameter("total_lost_item_count", value);
		}
	}

	public string RunId
	{
		set
		{
			SetParameter("run_id", value);
		}
	}

	public bool HasSecurityGrace
	{
		set
		{
			SetParameter("has_security_grace", value);
		}
	}

	public bool UsedSecurityGrace
	{
		set
		{
			SetParameter("used_security_grace", value);
		}
	}

	public OnInspectedEvent()
		: base("on_inspected")
	{
	}
}

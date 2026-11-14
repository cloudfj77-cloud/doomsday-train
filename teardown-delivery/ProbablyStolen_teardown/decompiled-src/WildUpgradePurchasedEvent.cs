using Unity.Services.Analytics;

public class WildUpgradePurchasedEvent : Event
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

	public string UpgradeId
	{
		set
		{
			SetParameter("upgrade_id", value);
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

	public WildUpgradePurchasedEvent()
		: base("wild_upgrade_purchased")
	{
	}
}

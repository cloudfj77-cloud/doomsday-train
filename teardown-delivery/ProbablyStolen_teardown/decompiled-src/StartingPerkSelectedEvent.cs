using Unity.Services.Analytics;

public class StartingPerkSelectedEvent : Event
{
	public string PerkId
	{
		set
		{
			SetParameter("perk_id", value);
		}
	}

	public string Spec
	{
		set
		{
			SetParameter("spec", value);
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

	public bool SkipIntro
	{
		set
		{
			SetParameter("skip_intro", value);
		}
	}

	public bool HardMode
	{
		set
		{
			SetParameter("hard_mode", value);
		}
	}

	public bool EndlessMode
	{
		set
		{
			SetParameter("endless_mode", value);
		}
	}

	public StartingPerkSelectedEvent()
		: base("starting_perk_selected")
	{
	}
}

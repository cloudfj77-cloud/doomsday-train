public class FactionStatusList
{
	public static FactionStatus MartialLaw()
	{
		return new FactionStatus
		{
			identifier = "martial_law",
			displayName = "Martial Law",
			duration = 6,
			strengthModifier = -7,
			orderModifier = 5,
			happinessModifier = -3,
			description = "Martial Law has been declared on the station, giving Security significantly more power by suspending individual rights and freedoms."
		};
	}

	public static FactionStatus SecurityCrackdown()
	{
		return new FactionStatus
		{
			identifier = "security_crackdown",
			displayName = "Security Crackdown",
			duration = 2,
			strengthModifier = -14,
			orderModifier = 8,
			happinessModifier = -4,
			description = "Security has launched a station-wide crackdown, increasing detentions and searches, but stretching resources thin."
		};
	}

	public static FactionStatus SecurityCheckpoint()
	{
		return new FactionStatus
		{
			identifier = "security_checkpoint",
			displayName = "Security Checkpoint",
			duration = 4,
			strengthModifier = -3,
			orderModifier = 3,
			happinessModifier = -1,
			description = "Security has set up checkpoints on the station, slowing down movement through key areas."
		};
	}

	public static FactionStatus SecurityBudgetCut()
	{
		return new FactionStatus
		{
			identifier = "security_budget_cut",
			displayName = "Security Budget Cut",
			duration = 5,
			strengthModifier = -3,
			orderModifier = -3,
			description = "Security is facing budget cuts, reducing their resource, which embolden criminal on the Station."
		};
	}

	public static FactionStatus SecurityBudgetIncrease()
	{
		return new FactionStatus
		{
			identifier = "security_budget_increase",
			displayName = "Security Budget Increase",
			duration = 5,
			strengthModifier = 3,
			orderModifier = 3,
			description = "Security has received a budget increase, causing some criminals to lay low."
		};
	}

	public static FactionStatus ReducedPatrol()
	{
		return new FactionStatus
		{
			identifier = "reduced_patrol",
			displayName = "Reduced Patrol",
			duration = 5,
			strengthModifier = 1,
			orderModifier = -2,
			description = "Security has reduced patrol coverage, leading to increased criminal activity and disorder."
		};
	}

	public static FactionStatus ExtraPatrol()
	{
		return new FactionStatus
		{
			identifier = "extra_patrol",
			displayName = "Extra Patrol",
			duration = 5,
			strengthModifier = -1,
			orderModifier = 2,
			happinessModifier = 2,
			description = "Security has increased patrol coverage, discouraging criminal activity and improving order."
		};
	}

	public static FactionStatus SurveillanceUpgrade()
	{
		return new FactionStatus
		{
			identifier = "surveillance_upgrade",
			displayName = "Surveillance Upgrade",
			duration = -1,
			strengthModifier = 0,
			orderModifier = 2,
			description = "New surveillance systems have been installed, improving security response times and reducing criminal activity."
		};
	}

	public static FactionStatus EmergencyLockdown()
	{
		return new FactionStatus
		{
			identifier = "emergency_lockdown",
			displayName = "Emergency Lockdown",
			duration = 2,
			strengthModifier = -15,
			orderModifier = 30,
			happinessModifier = -8,
			description = "An emergency lockdown has been initiated, cutting off movement and forcing order, at a heavy cost to resources."
		};
	}

	public static FactionStatus TacticalDeployment()
	{
		return new FactionStatus
		{
			identifier = "tactical_deployment",
			displayName = "ERT Deployment",
			duration = 4,
			strengthModifier = -5,
			orderModifier = 5,
			happinessModifier = 2,
			description = "Security has deployed Emergency Response Teams, increasing security presence and maintaining order."
		};
	}
}

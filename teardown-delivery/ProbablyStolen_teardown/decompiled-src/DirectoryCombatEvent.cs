public class DirectoryCombatEvent
{
	private static DirectoryCombatEvent instance = null;

	private static readonly object padlock = new object();

	public static DirectoryCombatEvent Instance
	{
		get
		{
			lock (padlock)
			{
				if (instance == null)
				{
					instance = new DirectoryCombatEvent();
				}
				return instance;
			}
		}
	}

	public CombatEvent CreateC4Event()
	{
		return new CombatEvent
		{
			name = "C4",
			description = "",
			duration = 1,
			InitCallback = CallbackC4EventInit,
			UpdateCallback = CallbackC4EventUpdate,
			ExpiredCallback = CallbackC4EventExpired,
			eventType = CombatEvent.EventType.Countdown,
			targetType = CombatEvent.TargetType.All,
			updateType = CombatEvent.UpdateType.TurnStart
		};
	}

	private void CallbackC4EventInit(CombatManager combatManager, CombatEvent combatEvent)
	{
		CombatLog.slog("C4 Planted! it's going to explode in " + combatEvent.duration + " turns!");
	}

	private void CallbackC4EventUpdate(CombatManager combatManager, CombatEvent combatEvent)
	{
		CombatLog.slog("The C4 is going to explode in " + combatEvent.duration + " turns!");
	}

	private void CallbackC4EventExpired(CombatManager combatManager, CombatEvent combatEvent)
	{
		CombatLog.slog("C4 Explode!!");
		combatManager.AppliedCombatEffectToAll(DirectoryMaster.CombatAbilityEffect("c4_damage"), null);
	}

	public CombatEvent CreateStormEvent()
	{
		return new CombatEvent
		{
			name = "Storm",
			description = "",
			duration = 3,
			InitCallback = CallbackStormEventInit,
			UpdateCallback = CallbackStormEventUpdate,
			ExpiredCallback = CallbackStormEventExpired,
			eventType = CombatEvent.EventType.Countdown,
			targetType = CombatEvent.TargetType.All,
			updateType = CombatEvent.UpdateType.TurnStart
		};
	}

	private void CallbackStormEventInit(CombatManager combatManager, CombatEvent combatEvent)
	{
		CombatLog.log("Storm Started! it's will last " + combatEvent.duration + " turns!");
	}

	private void CallbackStormEventUpdate(CombatManager combatManager, CombatEvent combatEvent)
	{
		CombatLog.log("The Storm is raging and will end in " + combatEvent.duration + " turns!");
		combatManager.AppliedCombatEffectToAll(DirectoryMaster.CombatAbilityEffect("storm_effect"), null);
	}

	private void CallbackStormEventExpired(CombatManager combatManager, CombatEvent combatEvent)
	{
		CombatLog.log("The Storm ended!");
	}
}

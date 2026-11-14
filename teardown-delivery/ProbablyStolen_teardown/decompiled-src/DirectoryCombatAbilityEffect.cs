using System.Collections.Generic;

public class DirectoryCombatAbilityEffect
{
	public Dictionary<int, CombatAbilityEffect> EffectList;

	private static DirectoryCombatAbilityEffect instance = null;

	private static readonly object padlock = new object();

	public static DirectoryCombatAbilityEffect Instance
	{
		get
		{
			lock (padlock)
			{
				if (instance == null)
				{
					instance = new DirectoryCombatAbilityEffect();
				}
				return instance;
			}
		}
	}

	private DirectoryCombatAbilityEffect()
	{
		EffectList = new Dictionary<int, CombatAbilityEffect>();
		CombatAbilityEffect value = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.overtimeAttack,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.health,
			duration = 3,
			value = -1,
			id = 0
		};
		EffectList.Add(0, value);
		CombatAbilityEffect value2 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.heal,
			abilityTargetType = CombatAbilityEffect.TargetType.all,
			affectedStat = CombatEntity.EffectType.health,
			duration = 0,
			value = 20,
			id = 1
		};
		EffectList.Add(1, value2);
		CombatAbilityEffect value3 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.attack,
			abilityTargetType = CombatAbilityEffect.TargetType.all,
			affectedStat = CombatEntity.EffectType.health,
			duration = 0,
			value = -20,
			id = 2
		};
		EffectList.Add(2, value3);
		CombatAbilityEffect value4 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.overtimeHeal,
			abilityTargetType = CombatAbilityEffect.TargetType.singleFriendly,
			affectedStat = CombatEntity.EffectType.health,
			duration = 5,
			value = 2,
			id = 3
		};
		EffectList.Add(3, value4);
		CombatAbilityEffect value5 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.buff,
			abilityTargetType = CombatAbilityEffect.TargetType.singleFriendly,
			affectedStat = CombatEntity.EffectType.accuracyPercentage,
			duration = 3,
			value = 25,
			id = 4,
			name = "Accuracy++"
		};
		EffectList.Add(4, value5);
		CombatAbilityEffect value6 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.buff,
			abilityTargetType = CombatAbilityEffect.TargetType.singleFriendly,
			affectedStat = CombatEntity.EffectType.criticalChanceDealing,
			duration = 3,
			value = 20,
			id = 5,
			name = "Critical Chance++"
		};
		EffectList.Add(5, value6);
		CombatAbilityEffect value7 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.attack,
			abilityTargetType = CombatAbilityEffect.TargetType.all,
			affectedStat = CombatEntity.EffectType.health,
			value = -50,
			id = 6
		};
		EffectList.Add(6, value7);
		CombatAbilityEffect value8 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.attack,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.health,
			value = -35,
			id = 7
		};
		EffectList.Add(7, value8);
		CombatAbilityEffect value9 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.stun,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.none,
			duration = 2,
			id = 8,
			name = "Stunned"
		};
		EffectList.Add(8, value9);
		CombatAbilityEffect value10 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.debuff,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.accuracyPercentage,
			value = -30,
			duration = 3,
			id = 9,
			name = "Accuracy-"
		};
		EffectList.Add(9, value10);
		CombatAbilityEffect value11 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.callback,
			abilityTargetType = CombatAbilityEffect.TargetType.self,
			callback = CallbackCombatScavenge,
			id = 10
		};
		EffectList.Add(10, value11);
		CombatAbilityEffect value12 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.buff,
			abilityTargetType = CombatAbilityEffect.TargetType.self,
			affectedStat = CombatEntity.EffectType.priority,
			value = 12,
			duration = 5,
			id = 12
		};
		EffectList.Add(12, value12);
		CombatAbilityEffect value13 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.setOverwatch,
			abilityTargetType = CombatAbilityEffect.TargetType.self,
			duration = 1,
			id = 13
		};
		EffectList.Add(13, value13);
		CombatAbilityEffect value14 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.setRetaliate,
			abilityTargetType = CombatAbilityEffect.TargetType.self,
			duration = 3,
			id = 14
		};
		EffectList.Add(14, value14);
		CombatAbilityEffect value15 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.callback,
			abilityTargetType = CombatAbilityEffect.TargetType.self,
			affectedStat = CombatEntity.EffectType.health,
			value = -80,
			id = 15,
			initCallback = CallbackEventC4
		};
		EffectList.Add(15, value15);
		CombatAbilityEffect value16 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.berserk,
			abilityTargetType = CombatAbilityEffect.TargetType.singleFriendly,
			duration = 3
		};
		EffectList.Add(16, value16);
		CombatAbilityEffect value17 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.attack,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.health,
			value = -15,
			id = 17
		};
		EffectList.Add(17, value17);
		CombatAbilityEffect value18 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.stun,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.none,
			duration = 1,
			id = 18
		};
		EffectList.Add(18, value18);
		CombatAbilityEffect value19 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.attack,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.health,
			value = -10,
			id = 19
		};
		EffectList.Add(19, value19);
		CombatAbilityEffect value20 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.stun,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.none,
			duration = 2,
			id = 20
		};
		EffectList.Add(20, value20);
		CombatAbilityEffect value21 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.overtimeAttack,
			abilityTargetType = CombatAbilityEffect.TargetType.singleFriendly,
			affectedStat = CombatEntity.EffectType.health,
			duration = 5,
			value = -4,
			id = 21
		};
		EffectList.Add(21, value21);
		CombatAbilityEffect value22 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.heal,
			abilityTargetType = CombatAbilityEffect.TargetType.singleFriendly,
			affectedStat = CombatEntity.EffectType.health,
			duration = 0,
			value = 40,
			id = 22
		};
		EffectList.Add(22, value22);
		CombatAbilityEffect value23 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.none,
			abilityTargetType = CombatAbilityEffect.TargetType.singleFriendly,
			id = 23
		};
		EffectList.Add(23, value23);
		CombatAbilityEffect value24 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.overtimeHeal,
			abilityTargetType = CombatAbilityEffect.TargetType.singleFriendly,
			affectedStat = CombatEntity.EffectType.health,
			duration = 4,
			value = 15,
			id = 24
		};
		EffectList.Add(24, value24);
		CombatAbilityEffect value25 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.attack,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.health,
			value = -17,
			id = 25
		};
		EffectList.Add(25, value25);
		CombatAbilityEffect value26 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.bleeding,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.health,
			duration = 3,
			value = -3,
			id = 26
		};
		EffectList.Add(26, value26);
		CombatAbilityEffect value27 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.attack,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.health,
			value = -22,
			id = 27
		};
		EffectList.Add(27, value27);
		CombatAbilityEffect value28 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.bleeding,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.health,
			duration = 3,
			value = -6,
			id = 28
		};
		EffectList.Add(28, value28);
		CombatAbilityEffect value29 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.attack,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.health,
			value = -22,
			id = 29
		};
		EffectList.Add(29, value29);
		CombatAbilityEffect value30 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.attack,
			abilityTargetType = CombatAbilityEffect.TargetType.singleEnemy,
			affectedStat = CombatEntity.EffectType.health,
			value = -22,
			id = 30
		};
		EffectList.Add(30, value30);
		CombatAbilityEffect value31 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.callback,
			abilityTargetType = CombatAbilityEffect.TargetType.singleFriendly,
			callback = CallbackEventBandage,
			id = 31
		};
		EffectList.Add(31, value31);
		CombatAbilityEffect value32 = new CombatAbilityEffect
		{
			abilityEffectType = CombatAbilityEffect.EffectType.buff,
			abilityTargetType = CombatAbilityEffect.TargetType.singleFriendly,
			id = 32,
			duration = 2,
			affectedStat = CombatEntity.EffectType.deathchance,
			value = 35
		};
		EffectList.Add(32, value32);
	}

	private void CallbackCombatScavenge(CombatManager combatManager, CombatEntity target, CombatEntity caster)
	{
		Debug.Log("Combat Scavenge Called");
	}

	private void CallbackEventC4(CombatManager combatManager, CombatEntity target, CombatEntity caster)
	{
		combatManager.combatEventHandler.AddEvent(DirectoryCombatEvent.Instance.CreateC4Event(), combatManager);
	}

	private void CallbackEventBandage(CombatManager combatManager, CombatEntity target, CombatEntity caster)
	{
		target.RemoveEffectByID("bleed_light");
		target.RemoveEffectByID("bleed_heavy");
	}
}

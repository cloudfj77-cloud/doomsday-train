public class CombatAbilityEffectInstantDirectory : CombatAbilityEffectDirectory
{
	protected override void InitDirectory()
	{
		Add("mass_heal", () => new CombatAbilityEffect("mass_heal", 20, 0, CombatAbilityEffect.EffectType.heal, CombatAbilityEffect.TargetType.all));
		Add("dev_damage_all", () => new CombatAbilityEffect("dev_damage_all", -1000, 0, CombatAbilityEffect.EffectType.attack, CombatAbilityEffect.TargetType.allEnemy));
		Add("c4_damage", () => new CombatAbilityEffect("c4_damage", -50, 1, CombatAbilityEffect.EffectType.attack, CombatAbilityEffect.TargetType.all));
		Add("weapon_attack", () => new CombatAbilityEffect("weapon_attack", -30, 1, CombatAbilityEffect.EffectType.attack));
		Add("glock_attack", () => new CombatAbilityEffect("glock_attack", -24, 1, CombatAbilityEffect.EffectType.attack));
		Add("ump_attack", () => new CombatAbilityEffect("ump_attack", 1, 1, CombatAbilityEffect.EffectType.attack, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.health, "DEFAULT EFFECT NAME", 5, 80, CombatAbilityEffect.ValueType.uniform, isPermanent: false, isInvisible: false, null, -8, -6));
		Add("laser_autocanon_attack", () => new CombatAbilityEffect("laser_autocanon_attack", 1, 1, CombatAbilityEffect.EffectType.attack, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.health, "DEFAULT EFFECT NAME", 3, 85, CombatAbilityEffect.ValueType.uniform, isPermanent: false, isInvisible: false, null, -15, -8));
		Add("combat_scavenge", () => new CombatAbilityEffect("combat_scavenge", 1, 1, CombatAbilityEffect.EffectType.callback, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.health, "DEFAULT EFFECT NAME", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: false, isInvisible: false, CallbackCombatScavenge));
		Add("event_c4_effect", () => new CombatAbilityEffect("event_c4_effect", -80, 1, CombatAbilityEffect.EffectType.callback, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.health, "DEFAULT EFFECT NAME", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: false, isInvisible: false, null, 0, 0, CallbackEventC4));
		Add("storm_effect", () => new CombatAbilityEffect("storm_effect", -15, 1, CombatAbilityEffect.EffectType.attack));
		Add("frag_damage", () => new CombatAbilityEffect("frag_damage", -25, 1, CombatAbilityEffect.EffectType.attack, CombatAbilityEffect.TargetType.allEnemy));
		Add("stun_baton_damage", () => new CombatAbilityEffect("stun_baton_damage", -10, 1, CombatAbilityEffect.EffectType.attack));
		Add("red_stim_heal", () => new CombatAbilityEffect("red_stim_heal", 55, 0, CombatAbilityEffect.EffectType.heal, CombatAbilityEffect.TargetType.singleFriendly));
		Add("white_stim_heal", () => new CombatAbilityEffect("white_stim_heal", 55, 0, CombatAbilityEffect.EffectType.heal, CombatAbilityEffect.TargetType.singleFriendly));
		Add("blue_stim_heal", () => new CombatAbilityEffect("blue_stim_heal", 0, 0, CombatAbilityEffect.EffectType.heal, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.health, "DEFAULT EFFECT NAME", 3, 50, CombatAbilityEffect.ValueType.uniform, isPermanent: false, isInvisible: false, null, 20, 5));
		Add("null", () => new CombatAbilityEffect("null", 1, 1, CombatAbilityEffect.EffectType.none, CombatAbilityEffect.TargetType.singleFriendly));
		Add("wait", () => new CombatAbilityEffect("wait", 0, 0, CombatAbilityEffect.EffectType.none, CombatAbilityEffect.TargetType.self));
		Add("knife_attack", () => new CombatAbilityEffect("knife_attack", -13, 1, CombatAbilityEffect.EffectType.attack));
		Add("combat_knife_attack", () => new CombatAbilityEffect("combat_knife_attack", -15, 1, CombatAbilityEffect.EffectType.attack));
		Add("cleaver_attack", () => new CombatAbilityEffect("cleaver_attack", -17, 1, CombatAbilityEffect.EffectType.attack));
		Add("combat_machete_attack", () => new CombatAbilityEffect("combat_machete_attack", -22, 1, CombatAbilityEffect.EffectType.attack));
		Add("baseball_bat_attack", () => new CombatAbilityEffect("baseball_bat_attack", -13, 1, CombatAbilityEffect.EffectType.attack));
		Add("lead_pipe_attack", () => new CombatAbilityEffect("lead_pipe_attack", -12, 1, CombatAbilityEffect.EffectType.attack));
		Add("hatchet_attack", () => new CombatAbilityEffect("hatchet_attack", -18, 1, CombatAbilityEffect.EffectType.attack));
		Add("crowbar_attack", () => new CombatAbilityEffect("crowbar_attack", -15, 1, CombatAbilityEffect.EffectType.attack));
		Add("fireaxe_attack", () => new CombatAbilityEffect("fireaxe_attack", -24, 1, CombatAbilityEffect.EffectType.attack));
		Add("fireaxe_big_attack", () => new CombatAbilityEffect("fireaxe_big_attack", -33, 1, CombatAbilityEffect.EffectType.attack));
		Add("blunt_melee_stun", () => new CombatAbilityEffect("blunt_melee_stun", 0, 1, CombatAbilityEffect.EffectType.stun, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.none, "DEFAULT EFFECT NAME", 1, 10));
		Add("bandage_stop_bleed", () => new CombatAbilityEffect("bandage_stop_bleed", 1, 1, CombatAbilityEffect.EffectType.callback, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.health, "DEFAULT EFFECT NAME", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: false, isInvisible: false, null, 0, 0, CallbackEventBandage));
		Add("bandage_stop_light_bleed", () => new CombatAbilityEffect("bandage_stop_light_bleed", 1, 1, CombatAbilityEffect.EffectType.callback, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.health, "DEFAULT EFFECT NAME", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: false, isInvisible: false, null, 0, 0, CallbackEventLightBandage));
		Add("stop_bleed", () => new CombatAbilityEffect("stop_bleed", 1, 1, CombatAbilityEffect.EffectType.callback, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.health, "Stop Bleed", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: false, isInvisible: false, null, 0, 0, CallbackEventStopBleed));
		Add("medical_assist_stun_reduce", () => new CombatAbilityEffect("medical_assist_stun_reduce", 1, 1, CombatAbilityEffect.EffectType.callback, CombatAbilityEffect.TargetType.singleFriendlyExceptSelf, CombatEntity.EffectType.none, "Stun Mitigation", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: false, isInvisible: false, null, 0, 0, CallbackEventStunReduction));
		Add("smelling_salt_stun_remove", () => new CombatAbilityEffect("smelling_salt_stun_remove", 1, 1, CombatAbilityEffect.EffectType.callback, CombatAbilityEffect.TargetType.singleFriendlyExceptSelf, CombatEntity.EffectType.none, "Stun Remove", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: false, isInvisible: false, null, 0, 0, CallbackEventStunRemove));
		Add("blood_red_heal", () => new CombatAbilityEffect("blood_red_heal", 35, 0, CombatAbilityEffect.EffectType.heal, CombatAbilityEffect.TargetType.singleFriendly));
		Add("xeno_scratch_attack", () => new CombatAbilityEffect("xeno_scratch_attack", -20, 0, CombatAbilityEffect.EffectType.attack));
		Add("xeno_laceration_attack", () => new CombatAbilityEffect("xeno_laceration_attack", -30, 0, CombatAbilityEffect.EffectType.attack));
		Add("xeno_consume_attack", () => new CombatAbilityEffect("xeno_consume_attack", -999, 0, CombatAbilityEffect.EffectType.attack, CombatAbilityEffect.TargetType.singleFriendly));
		Add("xeno_consume_heal", () => new CombatAbilityEffect("xeno_consume_heal", 100, 0, CombatAbilityEffect.EffectType.heal, CombatAbilityEffect.TargetType.self));
		Add("c4_breaching_damage", () => new CombatAbilityEffect("c4_breaching_damage", -30, 1, CombatAbilityEffect.EffectType.attack, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.health, "C4 Breaching"));
		Add("energy_hand_canon_normal", () => new CombatAbilityEffect("energy_hand_canon_normal", -1, 1, CombatAbilityEffect.EffectType.attack, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.health, "Energy Hand Canon Attack"));
		Add("energy_hand_canon_charged", () => new CombatAbilityEffect("energy_hand_canon_charged", -40, 1, CombatAbilityEffect.EffectType.attack, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.health, "Energy Hand Canon Charged Attack"));
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
		ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
		{
			CombatLog.log(target.name + "'s bleeding removed");
			SplashTextManager.Instance.DisplayFloatText(target, target.name + "'s bleeding removed", SplashTextManager.TextMode.Stun);
		}));
	}

	private void CallbackEventLightBandage(CombatManager combatManager, CombatEntity target, CombatEntity caster)
	{
		target.RemoveEffectByID("bleed_light");
		ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
		{
			CombatLog.log(target.name + "'s bleeding removed");
			SplashTextManager.Instance.DisplayFloatText(target, target.name + "'s light bleeding removed", SplashTextManager.TextMode.Stun);
		}));
	}

	private void CallbackEventStopBleed(CombatManager combatManager, CombatEntity target, CombatEntity caster)
	{
		target.RemoveEffectByID("bleed_light");
		target.RemoveEffectByID("bleed_heavy");
		target.RemoveEffectByID("red_stim_bleed");
		ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
		{
			CombatLog.log(target.name + "'s bleeding removed");
			SplashTextManager.Instance.DisplayFloatText(target, target.name + "'s bleeding removed", SplashTextManager.TextMode.Stun);
		}));
	}

	private void CallbackEventStunReduction(CombatManager combatManager, CombatEntity target, CombatEntity caster)
	{
		CombatAbilityEffect effectByType = ((GameCharacterItem)target).GetEffectByType(CombatAbilityEffect.EffectType.stun);
		if (effectByType != null)
		{
			effectByType.duration--;
			if (effectByType.duration == 0)
			{
				((GameCharacterItem)target).RemoveEffect(effectByType);
				ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
				{
					CombatLog.log(target.name + "'s stun is removed");
					SplashTextManager.Instance.DisplayFloatText(target, target.name + "'s stun is removed", SplashTextManager.TextMode.Heal);
				}));
			}
			else
			{
				ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
				{
					CombatLog.log(target.name + "'s stun duration decreased");
					SplashTextManager.Instance.DisplayFloatText(target, target.name + "'s duration decreased", SplashTextManager.TextMode.Heal);
				}));
			}
		}
		else
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(target.name + " get received any help from Medical Assistance");
			}));
		}
	}

	private void CallbackEventStunRemove(CombatManager combatManager, CombatEntity target, CombatEntity caster)
	{
		CombatAbilityEffect effectByType = ((GameCharacterItem)target).GetEffectByType(CombatAbilityEffect.EffectType.stun);
		if (effectByType != null)
		{
			((GameCharacterItem)target).RemoveEffect(effectByType);
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(target.name + "'s stun is removed");
				SplashTextManager.Instance.DisplayFloatText(target, target.name + "'s stun is removed", SplashTextManager.TextMode.Heal);
			}));
		}
		else
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(target.name + " didn't have any stun effect to be removed.");
			}));
		}
	}
}

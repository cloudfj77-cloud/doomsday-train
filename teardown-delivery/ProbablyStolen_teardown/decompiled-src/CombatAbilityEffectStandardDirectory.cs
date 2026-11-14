public class CombatAbilityEffectStandardDirectory : CombatAbilityEffectDirectory
{
	protected override void InitDirectory()
	{
		Add("combat_stim_acc_buff", () => new CombatAbilityEffect("combat_stim_acc_buff", 20, 3, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.accuracyPercentage, "Accuracy+"));
		Add("combat_stim_cc_buff", () => new CombatAbilityEffect("combat_stim_cc_buff", 33, 3, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.criticalChanceDealing, "Critical Chance++"));
		Add("flashbang_stun", () => new CombatAbilityEffect("flashbang_stun", 1, 2, CombatAbilityEffect.EffectType.stun, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.none, "Stunned"));
		Add("smoke_accuracy", () => new CombatAbilityEffect("smoke_accuracy", -30, 3, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.accuracyPercentage, "Accuracy-"));
		Add("molotov_cocktail_burn", () => new CombatAbilityEffect("molotov_cocktail_burn", -5, 4, CombatAbilityEffect.EffectType.burning, CombatAbilityEffect.TargetType.allEnemy, CombatEntity.EffectType.health, "Burning"));
		Add("lifeweed_accuracy", () => new CombatAbilityEffect("lifeweed_accuracy", -15, 5, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.accuracyPercentage, "Accuracy--"));
		Add("lifeweed_heal", () => new CombatAbilityEffect("lifeweed_heal", 1, 20, CombatAbilityEffect.EffectType.overtimeHeal, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.health, "Lifeweed Heal"));
		Add("aloe_vera_heal", () => new CombatAbilityEffect("aloe_vera_heal", 1, 15, CombatAbilityEffect.EffectType.overtimeHeal, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.health, "Aloe Vera Gel Heal"));
		Add("taunt_effect", () => new CombatAbilityEffect("taunt_effect", 12, 5, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.priority, "Taunting"));
		Add("overwatch_effect", () => new CombatAbilityEffect("overwatch_effect", 1, 1, CombatAbilityEffect.EffectType.setOverwatch, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.none, "Overwatch"));
		Add("retaliate_effect", () => new CombatAbilityEffect("retaliate_effect", 1, 3, CombatAbilityEffect.EffectType.setRetaliate, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.none, "Retaliate"));
		Add("berserk_effect", () => new CombatAbilityEffect("berserk_effect", 1, 3, CombatAbilityEffect.EffectType.berserk, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.health, "Berserk"));
		Add("stun_baton_stun", () => new CombatAbilityEffect("stun_baton_stun", 1, 1, CombatAbilityEffect.EffectType.stun, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.none));
		Add("c4_breaching_stun", () => new CombatAbilityEffect("c4_breaching_stun", 1, 2, CombatAbilityEffect.EffectType.stun, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.none, "C4 Breaching Stun"));
		Add("taser_stun", () => new CombatAbilityEffect("taser_stun", 1, 2, CombatAbilityEffect.EffectType.stun, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.none));
		Add("red_stim_bleed", () => new CombatAbilityEffect("red_stim_bleed", -2, 10, CombatAbilityEffect.EffectType.bleeding, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.health, "Internal bleeding"));
		Add("green_stim_heal", () => new CombatAbilityEffect("green_stim_heal", 15, 4, CombatAbilityEffect.EffectType.overtimeHeal, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.health, "Green Stim healing"));
		Add("bicarsole_heal", () => new CombatAbilityEffect("bicarsole_heal", 2, 40, CombatAbilityEffect.EffectType.overtimeHeal, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.health, "Bicarsole Healing"));
		Add("ocutol_acc", () => new CombatAbilityEffect("ocutol_acc", 5, 50, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.accuracyPercentage, "Ocutol Accuracy+"));
		Add("ocutol_critical", () => new CombatAbilityEffect("ocutol_critical", 5, 50, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.criticalChanceDealing, "Ocutol Crit Chance+"));
		Add("bleed_light", () => new CombatAbilityEffect("bleed_light", -2, 9, CombatAbilityEffect.EffectType.bleeding, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.health, "Light Bleeding"));
		Add("bleed_heavy", () => new CombatAbilityEffect("bleed_heavy", -4, 8, CombatAbilityEffect.EffectType.bleeding, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.health, "Heavy Bleeding"));
		Add("mushroom_death_resist", () => new CombatAbilityEffect("mushroom_death_resist", 35, 2, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.deathchance, "Death Resist++"));
		Add("coagulent", delegate
		{
			CombatAbilityEffect combatAbilityEffect2 = new CombatAbilityEffect("coagulent", 0, 3, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.none, "Boosted Coagulent");
			combatAbilityEffect2.EnableTag("antibleed");
			return combatAbilityEffect2;
		});
		Add("antitoxin", delegate
		{
			CombatAbilityEffect combatAbilityEffect = new CombatAbilityEffect("antitoxin", 0, 3, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.none, "Anti-Toxin");
			combatAbilityEffect.EnableTag("antitoxin");
			return combatAbilityEffect;
		});
		Add("mirage_dodge", () => new CombatAbilityEffect("mirage_dodge", 33, 3, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.dodge, "Dodge++"));
		Add("mark_dodge_debuff", () => new CombatAbilityEffect("mark_dodge_debuff", -15, 3, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.dodge, "Dodge-"));
		Add("mark_priority_increase", () => new CombatAbilityEffect("mark_priority_increase", 15, 3, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.priority, "Priority++"));
		Add("diversion_priority_increase", () => new CombatAbilityEffect("diversion_priority_increase", 18, 4, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.priority, "Priority++"));
		Add("spotter_accuracy_buff", () => new CombatAbilityEffect("spotter_accuracy_buff", 10, 3, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendlyExceptSelf, CombatEntity.EffectType.accuracyPercentage, "Acc+"));
		Add("take_aim_acc_buff", () => new CombatAbilityEffect("take_aim_acc_buff", 100, 1, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.accuracyPercentage, "Acc+++"));
		Add("take_aim_crit_chance_buff", () => new CombatAbilityEffect("take_aim_crit_chance_buff", 100, 1, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.criticalChanceDealing, "Crit Chance+++"));
		Add("take_aim_dodge_debuff", () => new CombatAbilityEffect("take_aim_dodge_debuff", -35, 1, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.dodge, "Dodge--"));
		Add("take_aim_reaction_buff", () => new CombatAbilityEffect("take_aim_reaction_buff", 10, 1, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.reaction, "Reac+++"));
		Add("brace_priority_decrease", () => new CombatAbilityEffect("brace_priority_decrease", -2, 2, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.priority, "Priority--"));
		Add("brace_dodge_buff", () => new CombatAbilityEffect("brace_dodge_buff", 12, 1, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.dodge, "dodge++"));
		Add("fixalin_dodge", () => new CombatAbilityEffect("fixalin_dodge", 5, 25, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.dodge, "dodge+"));
		Add("fixalin_speed", () => new CombatAbilityEffect("fixalin_speed", 3, 25, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.reaction, "reaction+"));
		Add("blood_red_max_health", () => new CombatAbilityEffect("blood_red_max_health", 35, 5, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.maxHealth, "max health++"));
		Add("xeno_warcry_acc_buff", () => new CombatAbilityEffect("xeno_warcry_acc_buff", 15, 3, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.allFriendly, CombatEntity.EffectType.accuracyPercentage, "acc++"));
		Add("xeno_mark_dodge_debuff", () => new CombatAbilityEffect("xeno_mark_dodge_debuff", -10, 3, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.dodge, "dodge--"));
		Add("xeno_mark_priority_increase", () => new CombatAbilityEffect("xeno_mark_priority_increase", 15, 3, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.priority, "priority++"));
		Add("xeno_prey_acc_buff", () => new CombatAbilityEffect("xeno_prey_acc_buff", 20, 2, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.accuracyPercentage, "acc++"));
		Add("xeno_prey_dodge_debuff", () => new CombatAbilityEffect("xeno_prey_dodge_debuff", -20, 2, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.dodge, "dodge--"));
		Add("xeno_irritant_acc_debuff", () => new CombatAbilityEffect("xeno_irritant_acc_debuff", -12, 2, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.allEnemy, CombatEntity.EffectType.accuracyPercentage, "acc--"));
		Add("xeno_toxin_attack", () => new CombatAbilityEffect("xeno_toxin_attack", -4, 5, CombatAbilityEffect.EffectType.overtimeAttack, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.health, "Xeno Toxin"));
		Add("xeno_empower_effect", () => new CombatAbilityEffect("xeno_empower_effect", 0, 1, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.none, "Empower"));
		Add("xeno_knockdown_stun", () => new CombatAbilityEffect("xeno_knockdown_stun", 1, 2, CombatAbilityEffect.EffectType.stun, CombatAbilityEffect.TargetType.singleEnemy, CombatEntity.EffectType.none));
		Add("xeno_healing_spray_heal", () => new CombatAbilityEffect("xeno_healing_spray_heal", 6, 5, CombatAbilityEffect.EffectType.overtimeHeal, CombatAbilityEffect.TargetType.singleFriendly));
		Add("spa_heal", () => new CombatAbilityEffect("spa_heal", 1, 100, CombatAbilityEffect.EffectType.overtimeHeal, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.health, "Spa Rested"));
		Add("spa_heal2", () => new CombatAbilityEffect("spa_heal2", 1, 100, CombatAbilityEffect.EffectType.overtimeHeal, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.health, "Spa Rested+"));
		Add("spa_acc", () => new CombatAbilityEffect("spa_acc", 5, 100, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.accuracyPercentage, "Spa Accuracy Buff", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: false, isInvisible: true));
		Add("spa_cc", () => new CombatAbilityEffect("spa_cc", 4, 100, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.criticalChanceDealing, "Spa Critical Chance", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: false, isInvisible: true));
		Add("spa_dodge", () => new CombatAbilityEffect("spa_dodge", 4, 100, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.dodge, "Spa Dodge", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: false, isInvisible: true));
		Add("spa_max_hp", () => new CombatAbilityEffect("spa_max_hp", 25, 100, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.maxHealth, "Spa Max Health", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: false, isInvisible: true));
	}
}

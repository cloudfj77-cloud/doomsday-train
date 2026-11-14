public class CombatAbilityEffectPermanentDirectory : CombatAbilityEffectDirectory
{
	protected override void InitDirectory()
	{
		Add("extreme_dehydration", delegate
		{
			CombatAbilityEffect combatAbilityEffect4 = new CombatAbilityEffect("extreme_dehydration", -6, 999, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.accuracyPercentage, "Extreme Dehydration", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true);
			combatAbilityEffect4.EnableTag(CombatAbilityEffect.EffectTag.survivalThirst);
			return combatAbilityEffect4;
		});
		Add("dehydration", delegate
		{
			CombatAbilityEffect combatAbilityEffect3 = new CombatAbilityEffect("dehydration", -4, 999, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.accuracyPercentage, "Dehydration", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true);
			combatAbilityEffect3.EnableTag(CombatAbilityEffect.EffectTag.survivalThirst);
			return combatAbilityEffect3;
		});
		Add("thirsty", delegate
		{
			CombatAbilityEffect combatAbilityEffect2 = new CombatAbilityEffect("thirsty", -2, 999, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.singleFriendly, CombatEntity.EffectType.accuracyPercentage, "Thirsty", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true);
			combatAbilityEffect2.EnableTag(CombatAbilityEffect.EffectTag.survivalThirst);
			return combatAbilityEffect2;
		});
		Add("well_hydrated", delegate
		{
			CombatAbilityEffect combatAbilityEffect = new CombatAbilityEffect("well_hydrated", 5, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.accuracyPercentage, "Well Hydrated", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true);
			combatAbilityEffect.EnableTag(CombatAbilityEffect.EffectTag.survivalThirst);
			return combatAbilityEffect;
		});
		Add("dead", () => new CombatAbilityEffect("dead", 1, 999, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.none, CombatEntity.EffectType.none, "Dead", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true));
	}
}

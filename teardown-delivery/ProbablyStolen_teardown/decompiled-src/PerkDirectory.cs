using System.Collections.Generic;

public class PerkDirectory : CombatAbilityEffectDirectory
{
	public static List<string> craftPerks;

	public static List<string> onEffectApplyPerks;

	protected override void InitDirectory()
	{
		craftPerks = new List<string>();
		onEffectApplyPerks = new List<string>();
		Add("perk_annoying", () => new CombatAbilityEffect("perk_annoying", 5, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.priority, "Annoying", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		Add("perk_discret", () => new CombatAbilityEffect("perk_discret", -3, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.priority, "Inconspicuous", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		Add("perk_healthy", () => new CombatAbilityEffect("perk_healthy", 25, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.maxHealth, "Picture of Health", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		Add("perk_sickly", () => new CombatAbilityEffect("perk_sickly", -25, 999, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.maxHealth, "Poor Constitution", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		Add("perk_tenacious", () => new CombatAbilityEffect("perk_tenacious", -20, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.deathchance, "Tenacious", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		Add("perk_center_mass_acc_buff", () => new CombatAbilityEffect("perk_center_mass_acc_buff", 200, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.accuracyPercentage, "Center Mass", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		Add("perk_center_mass_cc_debuff", () => new CombatAbilityEffect("perk_center_mass_cc_debuff", -200, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.deathchance, "Center Mass", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		Add("perk_hemophilia", () => new CombatAbilityEffect("perk_hemophilia", 0, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.none, "Hemophilia", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		onEffectApplyPerks.Add("perk_hemophilia");
		Add("perk_thrombophilia", () => new CombatAbilityEffect("perk_thrombophilia", 0, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.none, "Thrombophilia", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		onEffectApplyPerks.Add("perk_thrombophilia");
		Add("perk_venom_adapdation", () => new CombatAbilityEffect("perk_venom_adapdation", 0, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.none, "Venom Adapdation", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		onEffectApplyPerks.Add("perk_venom_adapdation");
		Add("perk_phagophobia", () => new CombatAbilityEffect("perk_phagophobia", 0, 999, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.none, "Medical Phagophobia", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		Add("perk_trypanophobia", () => new CombatAbilityEffect("perk_trypanophobia", 0, 999, CombatAbilityEffect.EffectType.debuff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.none, "Trypanophobia", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		Add("perk_crafty", () => new CombatAbilityEffect("perk_crafty", 0, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.none, "Crafty", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		craftPerks.Add("perk_crafty");
		Add("perk_pyromaniac", () => new CombatAbilityEffect("perk_pyromaniac", 0, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.none, "Pyromaniac", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
		craftPerks.Add("perk_pyromaniac");
		Add("perk_restorative_metabolism", () => new CombatAbilityEffect("perk_restorative_metabolism", 0, 999, CombatAbilityEffect.EffectType.buff, CombatAbilityEffect.TargetType.self, CombatEntity.EffectType.none, "Restorative Metabolism", 1, 100, CombatAbilityEffect.ValueType.standard, isPermanent: true, isInvisible: true));
	}
}

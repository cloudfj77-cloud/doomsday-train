using System;

public class AbilityItemHelper
{
	public static void CreateHoverTargetTooltip(RichTextBuilder builder, GameItem source, GameItem target)
	{
		if (source.effects.Count != 0 && source.bonusAccuracy != 1000 && target is GameCharacterItem)
		{
			int num = GraphUtils.FindParentType<GameCharacterItem>(source).accuracyPercentage + source.bonusAccuracy - ((GameCharacterItem)target).dodge;
			builder.AddLine("");
			builder.AddLine("Total hit chance:" + num, ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
		}
	}

	public static void CreateToolTipFromEffects(RichTextBuilder builder, GameItem item)
	{
		if (!((item.effects != null) & (item.effects.Count != 0)))
		{
			return;
		}
		foreach (CombatAbilityEffect effect in item.effects)
		{
			if (effect.abilityEffectType == CombatAbilityEffect.EffectType.callback || effect.abilityEffectType == CombatAbilityEffect.EffectType.none)
			{
				continue;
			}
			string text = "";
			RenderHandler.ColorPalette color = RenderHandler.ColorPalette.LightCyan;
			if (effect.repeat != 1)
			{
				text = text + "x" + effect.repeat + " ";
			}
			if (effect.effectApplyChance != 100)
			{
				text = text + effect.effectApplyChance + "% chance of ";
			}
			if (effect.abilityEffectType == CombatAbilityEffect.EffectType.heal)
			{
				text += "Heal ";
				color = RenderHandler.ColorPalette.LightGreen;
			}
			if (effect.abilityEffectType == CombatAbilityEffect.EffectType.overtimeHeal)
			{
				text += "Heal ";
				color = RenderHandler.ColorPalette.LightGreen;
			}
			else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.attack)
			{
				text += "Damage ";
				color = RenderHandler.ColorPalette.VeryLightRed;
			}
			else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.overtimeAttack)
			{
				text += "Damage ";
				color = RenderHandler.ColorPalette.VeryLightRed;
			}
			else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.bleeding)
			{
				text += "Bleed ";
				color = RenderHandler.ColorPalette.VeryLightRed;
			}
			else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.burning)
			{
				text += "Burn ";
				color = RenderHandler.ColorPalette.Red;
			}
			else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.toxin)
			{
				text += "Poison ";
				color = RenderHandler.ColorPalette.Green;
			}
			else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.buff)
			{
				text += "Buff ";
				color = RenderHandler.ColorPalette.LightOrange;
			}
			else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.debuff)
			{
				text += "Debuff ";
				color = RenderHandler.ColorPalette.Gray;
			}
			else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.stun)
			{
				text += "Stun ";
				color = RenderHandler.ColorPalette.LightYellow;
			}
			else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.setOverwatch)
			{
				text += "Set overwatch ";
				color = RenderHandler.ColorPalette.LightOrange;
			}
			else if (effect.abilityEffectType == CombatAbilityEffect.EffectType.setRetaliate)
			{
				text += "Set retaliate ";
				color = RenderHandler.ColorPalette.LightOrange;
			}
			if (effect.abilityTargetType == CombatAbilityEffect.TargetType.singleEnemy)
			{
				text += "an enemy ";
			}
			else if (effect.abilityTargetType == CombatAbilityEffect.TargetType.allEnemy)
			{
				text += "all enemies ";
			}
			else if (effect.abilityTargetType == CombatAbilityEffect.TargetType.all)
			{
				text += "all ";
			}
			else if (effect.abilityTargetType == CombatAbilityEffect.TargetType.self)
			{
				text += "self ";
			}
			else if (effect.abilityTargetType == CombatAbilityEffect.TargetType.singleFriendly)
			{
				text += "an ally ";
			}
			else if (effect.abilityTargetType == CombatAbilityEffect.TargetType.singleFriendlyExceptSelf)
			{
				text += "an ally except self ";
			}
			else if (effect.abilityTargetType == CombatAbilityEffect.TargetType.allFriendly)
			{
				text += "all allies ";
			}
			if (effect.abilityEffectType != CombatAbilityEffect.EffectType.stun && effect.affectedStat != CombatEntity.EffectType.none && effect.abilityEffectType != CombatAbilityEffect.EffectType.setOverwatch && effect.abilityEffectType != CombatAbilityEffect.EffectType.setRetaliate)
			{
				text = ((effect.valueType != CombatAbilityEffect.ValueType.uniform) ? (text + "by " + effect.value + " ") : (text + "by " + Math.Abs(effect.valueLow) + "-" + Math.Abs(effect.valueHigh) + " "));
			}
			if (effect.affectedStat != CombatEntity.EffectType.none)
			{
				if (effect.affectedStat == CombatEntity.EffectType.health)
				{
					text += "HP ";
				}
				else if (effect.affectedStat == CombatEntity.EffectType.accuracyPercentage)
				{
					text += "accuracy ";
				}
				else if (effect.affectedStat == CombatEntity.EffectType.dodge)
				{
					text += "dodge ";
				}
				else if (effect.affectedStat == CombatEntity.EffectType.priority)
				{
					text += "priority ";
				}
				else if (effect.affectedStat == CombatEntity.EffectType.reaction)
				{
					text += "reaction ";
				}
				else if (effect.affectedStat == CombatEntity.EffectType.maxHealth)
				{
					text += "Max HP ";
				}
				else if (effect.affectedStat == CombatEntity.EffectType.criticalChanceDealing)
				{
					text += "crit chance ";
				}
				else if (effect.affectedStat == CombatEntity.EffectType.criticalChanceReceving)
				{
					text += "crit chance resistance ";
				}
				else if (effect.affectedStat == CombatEntity.EffectType.none && effect.abilityEffectType != CombatAbilityEffect.EffectType.stun)
				{
					text = text + "with " + effect.name + " ";
				}
			}
			if (effect.duration != 0)
			{
				if (effect.abilityEffectType == CombatAbilityEffect.EffectType.attack || effect.abilityEffectType == CombatAbilityEffect.EffectType.overtimeAttack || effect.abilityEffectType == CombatAbilityEffect.EffectType.bleeding || effect.abilityEffectType == CombatAbilityEffect.EffectType.burning || effect.abilityEffectType == CombatAbilityEffect.EffectType.toxin || effect.abilityEffectType == CombatAbilityEffect.EffectType.heal || effect.abilityEffectType == CombatAbilityEffect.EffectType.overtimeHeal)
				{
					text += "/turn ";
				}
				text = ((effect.duration != 1) ? (text + "for " + effect.duration + " turns") : (text + "for " + effect.duration + " turn"));
			}
			builder.AddLine(text, ignoreEmpty: true, color);
		}
		if (item.bonusAccuracy != 1000 && item.bonusAccuracy != 0)
		{
			string text2 = "Accuracy Modifier: ";
			if (item.bonusAccuracy == 1000)
			{
				text2 += "ALWAYS ACCURATE";
			}
			else if (item.bonusAccuracy > 0)
			{
				text2 += "+";
				text2 += item.bonusAccuracy;
			}
			else
			{
				text2 += item.bonusAccuracy;
			}
			builder.AddLine(text2, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
	}
}

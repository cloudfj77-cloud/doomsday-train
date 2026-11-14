using System;

public class Survival
{
	private static readonly int MAX_THIRST = 1000;

	public static readonly int THIRST_LOSS = 15;

	private static readonly int THIRST_LOSS_IN_BASE = 7;

	private static readonly int EXTREME_DEHYDRATION_THRESHOLD = 0;

	private static readonly int DEHYDRATION_THRESHOLD = 150;

	private static readonly int THIRST_THRESHOLD = 500;

	private static readonly int WELL_HYDRATED = 800;

	public static void UpdateSurvival(GameCharacterItem entity, bool isInBase = false)
	{
		HandleRestorativeMetabolismPerk(entity);
		if (entity.IsTag("player"))
		{
			if (isInBase)
			{
				entity.currentThirst -= THIRST_LOSS_IN_BASE;
				CanteenHelper.CheckCanteen(entity);
				LiquidContainerHelper.CheckLiquidContainers(entity);
			}
			else
			{
				entity.currentThirst -= THIRST_LOSS;
				CanteenHelper.CheckCanteen(entity);
				LiquidContainerHelper.CheckLiquidContainers(entity);
			}
		}
		if (entity.currentThirst < 0)
		{
			entity.currentThirst = 0;
		}
		CheckSurvivalEffect(entity);
	}

	public static void CheckSurvivalEffect(GameCharacterItem entity)
	{
		entity.currentThirst = Math.Clamp(entity.currentThirst, 0, MAX_THIRST);
		entity.RemoveEffectByTag(CombatAbilityEffect.EffectTag.survivalHunger);
		if (entity.currentThirst == EXTREME_DEHYDRATION_THRESHOLD)
		{
			CombatAbilityEffect effect = DirectoryMaster.CombatAbilityEffect("extreme_dehydration");
			AddSurvivalEffect(entity, effect);
		}
		else if (entity.currentThirst <= DEHYDRATION_THRESHOLD)
		{
			CombatAbilityEffect effect2 = DirectoryMaster.CombatAbilityEffect("dehydration");
			AddSurvivalEffect(entity, effect2);
		}
		else if (entity.currentThirst <= THIRST_THRESHOLD)
		{
			CombatAbilityEffect effect3 = DirectoryMaster.CombatAbilityEffect("thirsty");
			AddSurvivalEffect(entity, effect3);
		}
		else if (entity.currentThirst >= WELL_HYDRATED)
		{
			CombatAbilityEffect effect4 = DirectoryMaster.CombatAbilityEffect("well_hydrated");
			AddSurvivalEffect(entity, effect4);
		}
		else
		{
			entity.RemoveEffectByTag(CombatAbilityEffect.EffectTag.survivalThirst);
		}
		if (entity.currentThirst <= 0)
		{
			entity.KillCurrentEntity();
		}
	}

	public static void AddSurvivalEffect(CombatEntity entity, CombatAbilityEffect effect)
	{
		if (effect.IsTag(CombatAbilityEffect.EffectTag.survivalThirst) && !entity.HaveEffect(effect.identifier))
		{
			entity.RemoveEffectByTag(CombatAbilityEffect.EffectTag.survivalThirst);
			entity.AppliedCombatEffect(effect, entity);
		}
	}

	public static void ConsumeDrink(GameCharacterItem entity)
	{
		CheckSurvivalEffect(entity);
	}

	public static void FullyRestore(GameCharacterItem entity)
	{
		entity.currentThirst = entity.maxThirst;
	}

	public static void HandleRestorativeMetabolismPerk(GameCharacterItem entity)
	{
		if (entity.HaveEffect("perk_restorative_metabolism") && entity.health < entity.maxHealth && entity.currentThirst > THIRST_THRESHOLD)
		{
			entity.currentThirst -= THIRST_LOSS;
			entity.HealEntity(1);
		}
	}
}

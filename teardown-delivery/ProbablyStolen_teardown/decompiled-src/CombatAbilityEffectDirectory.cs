using System;

public abstract class CombatAbilityEffectDirectory : Directory<CombatAbilityEffect>
{
	public override CombatAbilityEffect Empty()
	{
		return CreateEmptyCombatAbilityEffect();
	}

	public static CombatAbilityEffect CreateEmptyCombatAbilityEffect(string identifier = null)
	{
		return new CombatAbilityEffect
		{
			identifier = identifier
		};
	}

	public static CombatAbilityEffect CreateEmptyCombatAbilityEffect<K>(K identifierValue) where K : Enum
	{
		return new CombatAbilityEffect
		{
			identifier = TagSystem.EnumToString(identifierValue),
			identifierName = TagSystem.EnumToDisplayString(identifierValue)
		};
	}
}

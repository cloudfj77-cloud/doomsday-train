public class EffectHelper
{
	public static void UpdateBleedingEffect(GameCharacterItem gci)
	{
		CombatAbilityEffect effect = gci.GetEffect("bleed_light");
		if (effect != null && effect.duration > 21)
		{
			gci.RemoveEffect(effect);
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(gci.name + "'s light bleeding turn into heavy bleeding!");
			}));
			gci.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("bleed_heavy"), null);
		}
	}
}

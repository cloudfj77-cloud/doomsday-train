using Duckov.Buffs;

namespace ItemStatsSystem;

public class CheckBuffFilter : EffectFilter
{
	public Buff selfBuff;

	public Buff checkBuff;

	protected override bool OnEvaluate(EffectTriggerEventContext context)
	{
		if (selfBuff == null)
		{
			return false;
		}
		if (checkBuff == null)
		{
			return false;
		}
		if (selfBuff.Character == null)
		{
			return false;
		}
		return selfBuff.Character.HasBuff(checkBuff.ID);
	}
}

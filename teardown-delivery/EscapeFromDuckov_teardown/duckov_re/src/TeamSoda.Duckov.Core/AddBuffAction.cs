using Duckov.Buffs;
using ItemStatsSystem;

public class AddBuffAction : EffectAction
{
	private Buff selfBuff;

	public Buff buffPfb;

	private CharacterMainControl MainControl => base.Master?.Item?.GetCharacterMainControl();

	protected override void OnTriggered(bool positive)
	{
		if ((bool)MainControl)
		{
			CharacterMainControl fromWho = MainControl;
			if (!selfBuff)
			{
				selfBuff = GetComponentInParent<Buff>();
			}
			if ((bool)selfBuff)
			{
				fromWho = selfBuff.fromWho;
			}
			MainControl.AddBuff(buffPfb, fromWho);
		}
	}
}

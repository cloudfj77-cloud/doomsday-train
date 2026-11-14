using Duckov.Buffs;

namespace Duckov.PerkTrees.Behaviours;

public class AddBuff : PerkBehaviour
{
	public Buff prefab;

	protected override void OnUnlocked()
	{
		CharacterMainControl.Main.AddBuff(prefab);
	}
}

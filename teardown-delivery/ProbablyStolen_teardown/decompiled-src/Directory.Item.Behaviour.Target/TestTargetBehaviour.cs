namespace Directory.Item.Behaviour.Target;

public class TestTargetBehaviour : ItemTargetBehaviour
{
	public override bool? CanTarget(GameItem source, GameItem target)
	{
		if (source.IsTag(MeleeWeaponItemDirectory.MELEE_SHARP_WEAPON_TAG) && target is GameCharacterItem)
		{
			return true;
		}
		return false;
	}

	public override bool? OnTarget(GameItem source, GameItem target)
	{
		OverlayHandler.current.AppendLog(source.GetDisplayName() + " tickles " + target.GetDisplayName() + "!");
		return null;
	}

	public override string TargetTooltip(GameItem source, GameItem target)
	{
		return "Use " + source.GetDisplayName() + " to tickle " + target.GetDisplayName();
	}
}

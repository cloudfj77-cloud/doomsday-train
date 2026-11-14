using NodeCanvas.Framework;

public class TakeOutWeapon : ActionTask<AICharacterController>
{
	public enum weaponTypes
	{
		melee = -1,
		gun
	}

	public weaponTypes weaponType;

	protected override string OnInit()
	{
		return null;
	}

	protected override void OnExecute()
	{
		switch (weaponType)
		{
		case weaponTypes.melee:
			base.agent.CharacterMainControl.SwitchToWeapon(-1);
			break;
		case weaponTypes.gun:
			base.agent.CharacterMainControl.SwitchToWeapon(0);
			break;
		}
		EndAction(success: true);
	}

	protected override void OnStop()
	{
	}

	protected override void OnPause()
	{
	}
}

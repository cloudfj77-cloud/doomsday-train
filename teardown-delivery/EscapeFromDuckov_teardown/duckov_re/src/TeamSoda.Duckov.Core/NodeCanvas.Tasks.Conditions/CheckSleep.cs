using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Conditions;

public class CheckSleep : ConditionTask<AICharacterController>
{
	protected override string OnInit()
	{
		return null;
	}

	protected override void OnEnable()
	{
	}

	protected override void OnDisable()
	{
	}

	protected override bool OnCheck()
	{
		if (base.agent == null)
		{
			return false;
		}
		if (LevelManager.Instance == null)
		{
			return false;
		}
		if (LevelManager.Instance.ControllingCharacter == base.agent.CharacterMainControl)
		{
			return true;
		}
		return base.agent.Sleeping;
	}
}

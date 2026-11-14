using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions;

public class SetAimToRandomDirection : ActionTask<AICharacterController>
{
	public bool acordingToStartDirection;

	public Vector2 deltaRange;

	private bool waitingSearchResult;

	protected override string info => "Set aim to random direction";

	protected override string OnInit()
	{
		return null;
	}

	protected override void OnExecute()
	{
		Vector3 forward = Vector3.forward;
		float num = Random.Range(deltaRange.x, deltaRange.y);
		if (Random.Range(0f, 1f) < 0.5f)
		{
			num = 0f - num;
		}
		if (acordingToStartDirection)
		{
			forward = Quaternion.Euler(0f, num, 0f) * base.agent.startAimDirection;
		}
		else
		{
			Vector3 currentAimDirection = base.agent.CharacterMainControl.CurrentAimDirection;
			currentAimDirection.y = 0f;
			currentAimDirection.Normalize();
			forward = Quaternion.Euler(0f, num, 0f) * currentAimDirection;
		}
		base.agent.SetAimInput(forward, AimTypes.normalAim);
		EndAction(success: true);
	}

	protected override void OnUpdate()
	{
	}

	protected override void OnStop()
	{
	}

	protected override void OnPause()
	{
	}
}

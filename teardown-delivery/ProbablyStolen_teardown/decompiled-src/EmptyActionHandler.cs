using System.Collections.Generic;
using UnityEngine;

public abstract class EmptyActionHandler : InputActionHandler
{
	public override List<KeyCode> keyListeners => new List<KeyCode>();

	protected override bool OnEventInit()
	{
		return true;
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
	}

	protected override void OnEventRelease(ISet<KeyCode> keysReleased)
	{
	}

	protected override void OnEventMouseMove(Vector3 mousePosition, Vector3 mouseDelta)
	{
	}

	protected override void OnEventMouseScroll(Vector2 scrollDelta)
	{
	}

	protected override bool OnEventUpdate()
	{
		return false;
	}

	protected override void OnEventReset()
	{
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputActionHandler : MonoBehaviour
{
	[SerializeField]
	public int actionPriority;

	[NonSerialized]
	public bool lastEnabled;

	private bool initialized;

	public abstract List<KeyCode> keyListeners { get; }

	public int priority
	{
		get
		{
			return actionPriority;
		}
		set
		{
			actionPriority = value;
		}
	}

	private void Start()
	{
		TryInit();
		lastEnabled = base.enabled;
		InputActionManager.current.Add(this);
	}

	public void UpdatePress(ISet<KeyCode> keysPressed)
	{
		if (base.enabled && initialized && keysPressed.Count != 0)
		{
			OnEventPress(keysPressed);
		}
	}

	public void UpdateRelease(ISet<KeyCode> keysReleased)
	{
		if (base.enabled && initialized && keysReleased.Count != 0)
		{
			OnEventRelease(keysReleased);
		}
	}

	public void UpdateMouseMove(Vector3 mousePosition, Vector3 mouseDelta)
	{
		if (base.enabled && initialized && mouseDelta.sqrMagnitude != 0f)
		{
			OnEventMouseMove(mousePosition, mouseDelta);
		}
	}

	public void UpdateMouseScroll(Vector2 scrollDelta)
	{
		if (base.enabled && initialized && scrollDelta.sqrMagnitude != 0f)
		{
			OnEventMouseScroll(scrollDelta);
		}
	}

	public bool UpdateUpdate()
	{
		if (!base.enabled || !initialized)
		{
			return false;
		}
		return OnEventUpdate();
	}

	public void UpdateReset()
	{
		if (initialized)
		{
			OnEventReset();
		}
	}

	protected abstract bool OnEventInit();

	protected abstract void OnEventPress(ISet<KeyCode> keysPressed);

	protected abstract void OnEventRelease(ISet<KeyCode> keysReleased);

	protected abstract void OnEventMouseMove(Vector3 mousePosition, Vector3 mouseDelta);

	protected abstract void OnEventMouseScroll(Vector2 scrollDelta);

	protected abstract bool OnEventUpdate();

	protected abstract void OnEventReset();

	public bool Enable()
	{
		lastEnabled = true;
		if (base.enabled)
		{
			return false;
		}
		UpdateReset();
		base.enabled = true;
		return true;
	}

	public bool Disable()
	{
		lastEnabled = false;
		if (!base.enabled)
		{
			return false;
		}
		base.enabled = false;
		UpdateReset();
		return true;
	}

	private void TryInit()
	{
		if (OnEventInit())
		{
			initialized = true;
		}
	}

	public void Check()
	{
		if (!initialized)
		{
			TryInit();
		}
		else if (!base.enabled && lastEnabled)
		{
			lastEnabled = false;
			OnEventReset();
		}
		else if (base.enabled && !lastEnabled)
		{
			lastEnabled = true;
			OnEventReset();
		}
	}
}

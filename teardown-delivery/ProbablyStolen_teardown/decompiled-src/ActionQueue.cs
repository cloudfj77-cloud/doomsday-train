using System.Collections;
using UnityEngine;

public class ActionQueue
{
	private static ActionQueue currentInstance;

	private Deque<GameAction> actions = new Deque<GameAction>();

	private bool locked;

	public static ActionQueue current
	{
		get
		{
			if (currentInstance == null)
			{
				currentInstance = new ActionQueue();
			}
			return currentInstance;
		}
	}

	public void AddFirst(GameAction action)
	{
		actions.PushLeft(action);
	}

	public void AddLast(GameAction action)
	{
		actions.PushRight(action);
	}

	public void Add(GameAction action)
	{
		AddLast(action);
	}

	public void Append(GameAction action)
	{
		AddLast(action);
	}

	public void StartImmediately(GameAction action)
	{
		if (action != null)
		{
			OverlayHandler.current.StartCoroutine(AnimationCoroutine(action));
		}
	}

	public bool HasNext()
	{
		ClearInvalid();
		return !actions.IsEmpty;
	}

	public bool Next()
	{
		if (locked)
		{
			return false;
		}
		if (!HasNext())
		{
			return false;
		}
		locked = true;
		GameAction gameAction = actions.PopLeft();
		OverlayHandler.current.StartCoroutine(AnimationCoroutine(gameAction));
		OverlayHandler.current.StartCoroutine(NextCoroutine(gameAction, Time.timeAsDouble + gameAction.nextTimeSeconds));
		return true;
	}

	private void ClearInvalid()
	{
		while (!actions.IsEmpty && !actions.PeekLeft().IsValid())
		{
			actions.PopLeft();
		}
	}

	private IEnumerator AnimationCoroutine(GameAction gameAction, float fps = 30f)
	{
		gameAction.AnimationBegin(Time.timeAsDouble);
		if (fps < 0.01f)
		{
			fps = 0.01f;
		}
		if (fps > 240f)
		{
			fps = 240f;
		}
		int count = 0;
		while (gameAction.AnimationStep(Time.timeAsDouble) && !((float)count > 1000f * fps))
		{
			count++;
			yield return new WaitForSeconds(1f / fps);
		}
		gameAction.AnimationEnd(Time.timeAsDouble);
	}

	private IEnumerator NextCoroutine(GameAction gameAction, double nextTime, float timerPrecision = 0.1f)
	{
		bool doBlockUI = false;
		if (gameAction.blockUI)
		{
			doBlockUI = true;
		}
		if (doBlockUI)
		{
			OverlayHandler.current.BlockMouse();
		}
		gameAction.beginFunc?.Invoke();
		if (timerPrecision < 0.0001f)
		{
			timerPrecision = 0.0001f;
		}
		if (timerPrecision > 10f)
		{
			timerPrecision = 10f;
		}
		int count = 0;
		while (!(Time.timeAsDouble >= nextTime) && !((float)count > 1000f / timerPrecision))
		{
			count++;
			yield return new WaitForSeconds(timerPrecision);
		}
		gameAction.endFunc?.Invoke();
		locked = false;
		if (doBlockUI)
		{
			OverlayHandler.current.UnblockMouse();
		}
		Next();
	}
}

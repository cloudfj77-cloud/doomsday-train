using System;

public abstract class GameAction
{
	private bool _blockUI = true;

	public double nextTimeSeconds { get; protected set; }

	public Action beginFunc { get; private set; }

	public Action endFunc { get; private set; }

	public bool blockUI => _blockUI;

	public GameAction SetBeginFunc(Action beginFunc)
	{
		this.beginFunc = beginFunc;
		return this;
	}

	public GameAction SetEndFunc(Action endFunc)
	{
		this.endFunc = endFunc;
		return this;
	}

	public GameAction SetBlockUI(bool blockUI)
	{
		_blockUI = blockUI;
		return this;
	}

	public abstract bool IsValid();

	public abstract bool AnimationStep(double currentTime);

	public abstract void AnimationBegin(double currentTime);

	public abstract void AnimationEnd(double currentTime);
}

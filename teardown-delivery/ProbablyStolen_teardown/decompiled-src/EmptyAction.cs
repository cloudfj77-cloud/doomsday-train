public class EmptyAction : GameAction
{
	public EmptyAction(double waitNextSeconds = 0.0)
	{
		base.nextTimeSeconds = waitNextSeconds;
	}

	public override bool IsValid()
	{
		return true;
	}

	public override bool AnimationStep(double currentTime)
	{
		return false;
	}

	public override void AnimationBegin(double currentTime)
	{
	}

	public override void AnimationEnd(double currentTime)
	{
	}
}

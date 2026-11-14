public interface IHoldClickable : IClickable
{
	float HoldDuration { get; }

	bool ShouldRequireHold();
}

public abstract class EventTrigger : PriorityTrigger
{
	public virtual bool? Filter()
	{
		return null;
	}

	public virtual bool? Trigger()
	{
		return null;
	}

	public virtual bool? OverrideExistingEvent(EventData currentEvent)
	{
		return null;
	}

	public virtual EventData GenerateEvent()
	{
		return null;
	}
}

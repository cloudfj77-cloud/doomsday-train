public class TestTraderStationEventTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		if (GameMaster.current.worldTags.GetTag("TEST_TRADER_EVENT_STARTED").GetInt() > 0)
		{
			return false;
		}
		if (GameMaster.current.shipManager.nextPOI is OverworldStation { isHostile: false })
		{
			_ = GameMaster.current.shipManager.isTransit;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Trade Station Test Event", "Hello, this is a test event telling that you have arrived for the first time at a trade station!");
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("TEST_TRADER_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
		};
		eventData.endTurn = 3;
		eventData.turnAction = delegate(EventData e)
		{
			if (e.startTurn != e.turns)
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Event is almost ended! " + (e.endTurn - e.turns) + " turns left!", RenderHandler.ColorPalette.LightRed));
			}
		};
		return eventData;
	}
}

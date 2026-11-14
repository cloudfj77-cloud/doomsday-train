using System.Linq;

public class TestCoinEventTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		if (GameMaster.current.worldTags.GetTag("TEST_COIN_EVENT_STARTED").GetInt() > 1)
		{
			return false;
		}
		if (GameMaster.current.shipManager.ship.FindAllChildItems().FindAll((GameItem i) => i.identifier == "coin").Any())
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData e2 = new EventData().SetText("Test Event", "Hello, this is a test event!");
		e2.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("TEST_COIN_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
		};
		e2.endTurn = 3;
		e2.turnAction = delegate(EventData e)
		{
			if (e.startTurn != e.turns)
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Event is almost ended! " + (e.endTurn - e.turns) + " turns left!", RenderHandler.ColorPalette.LightRed));
			}
		};
		e2.endAction = delegate
		{
			GameMaster.current.shipManager.ship?.TryAcceptAll(DirectoryMaster.Item("green_injector").SetAmount(1));
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("A green injector has appeared in the ship's cargo!", RenderHandler.ColorPalette.LightGreen));
		};
		e2.AddChoice(DirectoryMaster.Item("coin"), "Give me some money!", "", null, null, delegate
		{
			GameMaster.current.shipManager.ship?.TryAcceptAll(DirectoryMaster.Item("coin").SetAmount(5));
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Some coins has appeared in the ship's cargo!", RenderHandler.ColorPalette.LightGreen));
			e2.ForceEnd();
			EventData @event = new EventData().SetText("Chained event", "This is the event that runs after spawning some money!");
			GameMaster.current.shipManager.SetEvent(@event);
		});
		return e2;
	}
}

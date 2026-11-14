public class SpaceGhostShipTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_GHOST_SPACE_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("ghost_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData e2 = new EventData().SetText("Drifting Ship", "You have arrived at a small transport ship drifting aimlessly into the space. \r\n        The ship is not responding to your communication attempt. ");
		e2.SetImage("event_ghost_ship");
		e2.text.AddLine("\r\n        Scanner shows that the ship is operational", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		e2.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_GHOST_SPACE_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		bool boarded = false;
		e2.AddChoice(DirectoryMaster.Item("search"), "Board and investigate the ship.", "", null, () => !boarded, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("After boarding the ship, your crew has not found anyone dead or alive on it. Some opened food are still fresh, cleary people were living on the ship recently. Stranger yet, all ship's systems are intact, there still credits and fuel on the ship."));
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your crew came back with the fuel and the credits they found from the preliminary search.", RenderHandler.ColorPalette.LightGreen));
			GameItem gameItem = DirectoryMaster.Item("backpack_medium");
			GameItem item = DirectoryMaster.Item("exotic_fuel").SetUnitCount(20);
			GameItem item2 = DirectoryMaster.Item("energy_credit");
			GraphUtils.TryAcceptAll(gameItem, item);
			GraphUtils.TryAcceptAll(gameItem, item2);
			OverlayHandler.current.ShowEvent();
			OverlayHandler.current.ShowInventory(gameItem);
			boarded = true;
		});
		e2.AddChoice(DirectoryMaster.Item("ok"), "Salvage the ship.", "", null, () => boarded, delegate
		{
		});
		e2.AddChoice(DirectoryMaster.Item("open_fire"), "(Hostile) Open fire on the ship.", "", null, null, delegate
		{
			e2.ForceEnd();
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return e2;
	}
}

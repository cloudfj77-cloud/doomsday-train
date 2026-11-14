public class SpaceCargoConvoyTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_CARGO_CONVOY_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("cargo_caravan_convoy") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Cargo Caravan Convoy", "You arrive near a fleet of cargo ship. You can see ship of all kind and size. Some are armed and armored, other looks barely space worthy.");
		eventData.SetImage("event_cargo_fleet_captain");
		eventData.text.AddLine("\r\nOne of the ship captain greet you. They are a group of independant traders and freighters travelling together for safety from one station to another. ", ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_CARGO_CONVOY_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		eventData.AddChoice(DirectoryMaster.Item("question"), "Ask for the direction of the nearest trade station.", "", null, null, delegate
		{
		});
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return eventData;
	}
}

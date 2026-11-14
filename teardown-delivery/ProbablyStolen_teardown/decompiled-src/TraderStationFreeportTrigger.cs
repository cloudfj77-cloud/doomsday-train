public class TraderStationFreeportTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("TRADER_STATION_FREEPORT_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldStation { isHostile: false } overworldStation && !GameMaster.current.shipManager.isTransit && !overworldStation.generatedStation.GetArrivalEvent().Equals("FREE_PORT"))
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Free Port", " You have arrived at a space station.");
		eventData.text.Add("\r\nThis station is a ");
		eventData.text.Add("Free Port ", RenderHandler.ColorPalette.LightGreen);
		eventData.text.AddLine("so no item is considered to be contraband here. Consequently those item are valued less here. ");
		eventData.text.AddLine("\r\nA station trafic controller contacts you and assign your ship a docking bay.");
		eventData.SetImage("event_freeport");
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("TRADER_STATION_FREEPORT_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return eventData;
	}
}

public class TraderStationBlockadeTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("TRADER_STATION_BLOCKADE_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldStation { isHostile: false } overworldStation && !GameMaster.current.shipManager.isTransit && !overworldStation.generatedStation.GetArrivalEvent().Equals("BLOCKADE"))
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData e2 = new EventData().SetText("Blockaded Trade Station", " You have arrived at a space station.");
		e2.text.AddLine("\r\nThis station is blockaded by a fleet of ship. Whom ever is siegeing the station is trying to starve them out!", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
		e2.startAction = delegate(EventData e)
		{
			e.BlockStation();
			TagState tag = GameMaster.current.worldTags.GetTag("TRADER_STATION_BLOCKADE_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		e2.AddChoice(DirectoryMaster.Item("ok"), "Attempt to sneak past the blockade.", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You have sucessfully sneak past the blockade and can now dock with the station.", RenderHandler.ColorPalette.LightGreen));
			e2.blockStation = false;
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return e2;
	}
}

public class SpaceDebrisFieldTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_DEBRIS_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("debris_field") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData e2 = new EventData().SetText("Debris Field", "You have arrived at a large debris field, it seems to be the remains of a massive space battle. Sadly, most of the debris seems to be to small to be scavanged");
		e2.SetImage("event_debris_field");
		e2.text.AddLine("\r\nDue to the danger of colliding with the debris field, you won't be able to use the FTL engine before getting clear of it.", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
		e2.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_DEBRIS_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		e2.AddChoice(DirectoryMaster.Item("ok"), "Try to navigate out of the debris field.", "", null, null, delegate
		{
			GameMaster.current.shipManager.DamageHull(10);
			e2.ForceEnd();
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return e2;
	}
}

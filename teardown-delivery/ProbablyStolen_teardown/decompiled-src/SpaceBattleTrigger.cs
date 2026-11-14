public class SpaceBattleTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_BATTLE_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("space_battle") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData e2 = new EventData().SetText("Space Battle", "You arrive in the middle of a massive space battle. Two fleets are attacking each other.");
		e2.SetImage("event_fleet_battle");
		e2.text.AddLine("\r\nYou made sure your transponder is on, but in the heat of the battle there is no garantee that they won't fire on a unknown ship on sight. You must get clear of the battle before jumping again.", ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
		e2.startAction = delegate(EventData e)
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_BATTLE_EVENT_STARTED");
			e.BlockTravel();
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Attempt to escape the battle.", "", null, null, delegate
		{
			if (RNG.Roll(50))
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You attempt to escape away from the battle with your ship and but during the heat of battle, your ship were fired upon.", RenderHandler.ColorPalette.LightRed));
				SpaceBattleHelper.TakingHit(25);
				e2.ForceEnd();
			}
			else
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You manage to safetly escape away from the battle without being fired upon!", RenderHandler.ColorPalette.LightGreen));
				e2.ForceEnd();
			}
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return e2;
	}
}

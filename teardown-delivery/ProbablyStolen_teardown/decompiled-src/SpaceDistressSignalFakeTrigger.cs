public class SpaceDistressSignalFakeTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_DISTRESS_SIGNAL_FAKE_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("distree_signal_fake") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData e2 = new EventData().SetText("Ship Distress Signal", "You have arrived to the source of the distress signal, an heavily armed pirate ship. ");
		e2.SetImage("raider");
		e2.text.AddLine("\r\nIt's trap.", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
		e2.text.AddLine("\r\nThe ship's captain has contacted you and is demanding that you pay 400 Energy Credit or they will open fire on your ship.", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
		e2.startAction = delegate(EventData e)
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_DISTRESS_SIGNAL_FAKE_EVENT_STARTED");
			e.BlockTravel();
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		GameMaster.current.shipManager.GetTotalAvailableEnergyCredit();
		e2.AddChoice(DirectoryMaster.Item("ok"), "(400 Energy Credit) Pay the bandit", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 400, null, delegate
		{
			GameMaster.current.shipManager.RemoveEnergyCredit(400);
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("The bandit is satisfied and informs you that you may leave peacefully.", RenderHandler.ColorPalette.LightGreen));
			e2.ForceEnd();
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Refuse to pay and attempt to escape.", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You attempt to escape away with your ship and the pirate has open fire on your ship!", RenderHandler.ColorPalette.LightRed));
			SpaceBattleHelper.TakingHit(10);
			e2.ForceEnd();
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return e2;
	}
}

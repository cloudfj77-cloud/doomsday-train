public class SpaceBanditTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_BANDIT_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("bandit_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData e2 = new EventData().SetText("Bandit Ship", "You have arrived near a unmarked military transport ship.");
		e2.SetImage("raider");
		e2.text.AddLine("\r\nThe ship's captain has contacted you and is demanding that you pay 400 Energy Credit for 'protection fee'.", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
		e2.text.AddLine("\r\nScanner shows that the ship is armed", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
		e2.startAction = delegate(EventData e)
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_BANDIT_EVENT_STARTED");
			e.BlockTravel();
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now and request more time.", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("The bandit accepts and informs you that they cannot garantee your protection for much longer without paying the fee.", RenderHandler.ColorPalette.LightGreen));
		});
		GameMaster.current.shipManager.GetTotalAvailableEnergyCredit();
		e2.AddChoice(DirectoryMaster.Item("ok"), "(400 Energy Credit) Pay the bandit", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 400, null, delegate
		{
			GameMaster.current.shipManager.RemoveEnergyCredit(400);
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("The bandit is satisfied and informs you that you may leave peacefully.", RenderHandler.ColorPalette.LightGreen));
			e2.ForceEnd();
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Open fire on the bandit ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
		{
		});
		e2.AddChoice(DirectoryMaster.Item("ok"), "[Military Ship] Identify yourself as warship and demand that the bandit transfer you 400 Credit instead or otherwise you might not garantee their safety.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("After realizing that they were trying to rob a warship, the bandit leader quickly transfer the credit before their ship jumps away.", RenderHandler.ColorPalette.LightGreen));
			GameItem gameItem = DirectoryMaster.Item("backpack_medium");
			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
			OverlayHandler.current.ShowInventory(gameItem);
			e2.ForceEnd();
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Refuse to pay and attempt to escape.", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You attempt to escape away with your ship and the pirate has open fire on your ship!", RenderHandler.ColorPalette.LightRed));
			SpaceBattleHelper.TakingHit(10);
			e2.ForceEnd();
		});
		return e2;
	}
}

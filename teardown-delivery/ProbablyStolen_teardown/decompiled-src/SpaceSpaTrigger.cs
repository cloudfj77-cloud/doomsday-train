public class SpaceSpaTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_SPA_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("spa_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData e2 = new EventData().SetText("Friendly Spa Ship", "You have arrived at a leisure ship. \r\nYou are greeted over the comms by an over-friendly attendant whom inform you that they are operating a spa ship and that their package start at 100 Energy Credit. The attendant mentions health benefits with the usage of the spa.");
		e2.SetImage("spa");
		e2.text.AddLine("\r\nYou have a total of " + GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() + " Energy Credit.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		e2.text.AddLine("\r\nScanner shows that the spa ship is unarmed", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		e2.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_SPA_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		GameMaster.current.shipManager.GetTotalAvailableEnergyCredit();
		GameItem gameItem;
		e2.AddChoice(gameItem = DirectoryMaster.Item("$"), "(100 Credit) Pay for the standard spa package for the crew.", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 100, null, delegate
		{
			GameMaster.current.shipManager.RemoveEnergyCredit(100);
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your crew spent sometime in the spa and returned relaxed and ready.", RenderHandler.ColorPalette.LightGreen));
			foreach (GameCharacterItem item in GameMaster.current.shipManager.GetCrew())
			{
				item.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_heal"), null);
				item.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_acc"), null);
				item.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_cc"), null);
			}
			e2.ForceEnd();
		});
		GameMaster.current.shipManager.GetTotalAvailableEnergyCredit();
		e2.AddChoice(DirectoryMaster.Item("$$"), "(150 Credit) Pay for the premium spa package for the crew.", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 150, null, delegate
		{
			GameMaster.current.shipManager.RemoveEnergyCredit(150);
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your crew spent some quality time in the spa and returned focused and ready.", RenderHandler.ColorPalette.LightGreen));
			foreach (GameCharacterItem item2 in GameMaster.current.shipManager.GetCrew())
			{
				item2.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_heal2"), null);
				item2.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_acc"), null);
				item2.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_cc"), null);
				item2.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_dodge"), null);
				item2.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_max_hp"), null);
			}
			e2.ForceEnd();
		});
		e2.AddChoice(DirectoryMaster.Item("rob"), "(Hostile) Rob the spa ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
		{
			e2.ForceEnd();
		});
		e2.AddChoice(DirectoryMaster.Item("open_fire"), "(Hostile) Open fire on the spa ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
		{
			e2.ForceEnd();
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return e2;
	}
}

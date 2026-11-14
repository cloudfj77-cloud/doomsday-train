public class SpaceNavySupplyTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_NAVY_SUPPLY_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("navy_supply_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Republic Navy Supply Ship", "You have arrived at a lone autonomous supply ship with the Republic marking.");
		eventData.text.AddLine("\r\nThis type of supply ship are known to carry military grade supplies and equipments. They are fully autonomous and carry no crew.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		eventData.text.AddLine("\r\nTheir onboard ship AI contacts you and warn you that they are part of the Republic Navy any hostile action will trigger retaliation from the Republic Navy.", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_NAVY_SUPPLY_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		eventData.AddChoice(DirectoryMaster.Item("open_fire"), "Open fire on the drone ship.", "", null, null, delegate
		{
		});
		GameItem container = DirectoryMaster.Item("lootbox_medium");
		bool cardUsed = false;
		eventData.AddChoice(DirectoryMaster.Item("keycard"), "[Republic Keycard] Use Keycard.", "", () => GameMaster.current.shipManager.IsShipHaveItem("republic_keycard") && !cardUsed, null, delegate
		{
			GameMaster.current.shipManager.RemoveItemFromShip("republic_keycard");
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n'Republic Supply Ship Ai: Level X access detected, unlocking access...", RenderHandler.ColorPalette.LightGray));
			GameItem item;
			for (int i = 0; i < 1; i++)
			{
				item = DirectoryMaster.Item("energy_credit");
				GraphUtils.TryAcceptAll(container, item);
			}
			item = DirectoryMaster.Item("armor_titan10");
			GraphUtils.TryAcceptAll(container, item);
			if (RNG.Roll(5))
			{
				GraphUtils.TryAcceptAll(container, DirectoryMaster.Item("republic_keycard"));
			}
			OverlayHandler.current.ShowInventory(container);
			cardUsed = true;
		});
		eventData.AddChoice(DirectoryMaster.Item("loot"), "Check Loot.", "", null, () => cardUsed, delegate
		{
			OverlayHandler.current.ShowInventory(container);
		});
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return eventData;
	}
}

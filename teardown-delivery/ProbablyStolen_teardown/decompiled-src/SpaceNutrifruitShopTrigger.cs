using System.Collections.Generic;

public class SpaceNutrifruitShopTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_NUTRIFRUIT_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("nutrifruit_drone") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Pastel Drone Ship", "You have arrived at a lone autonomous merchant drone ship with pastel inc.'s logo. The onboard ship AI have contacted you and offer to browse their wares while pitching about the quality of pastel's products.");
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_NUTRIFRUIT_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "pastel Drone Shop (Credit Only)";
		trader.BarterEnable(TradeSheet.GetGenericPastelShop());
		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(new List<string> { "power_source_item" }, trader);
		eventData.AddChoice(DirectoryMaster.Item("trade"), "Browse wares", "", null, null, delegate
		{
			trader.BarterShow();
		});
		eventData.AddChoice(DirectoryMaster.Item("open_fire"), "Open fire on the drone ship.", "", null, null, delegate
		{
		});
		GameItem container = DirectoryMaster.Item("lootbox_medium");
		bool cardUsed = false;
		eventData.AddChoice(DirectoryMaster.Item("keycard"), "[pastel Keycard] Use Keycard.", "", () => GameMaster.current.shipManager.IsShipHaveItem("pastel_keycard"), null, delegate
		{
			GameMaster.current.shipManager.RemoveItemFromShip("pastel_keycard");
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n'pastel Droneship: Level X access detected, unlocking access...", RenderHandler.ColorPalette.LightGray));
			foreach (GameItem childItem in trader.barterInventory.childItems)
			{
				childItem.DisableTag(GameCharacterItem.TAG_NOT_PURCHASED);
				GraphUtils.TryAcceptAll(container, childItem);
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

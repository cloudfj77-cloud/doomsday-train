using System.Collections.Generic;

public class SpaceRefugeeTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_REFUGEE_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("refugee_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData e2 = new EventData().SetText("Friendly Refugee Ship", "You have arrived to a makeshift ship. They are a group of a spacers banding together trying to survive in the Subspace.");
		e2.SetImage("event_refugee");
		if (GameMaster.current.shipManager.ship.identifier.Equals("refugee_ship"))
		{
			e2.text.AddLine("Their ship looks much like your own. The captain of the ship contacts you via the comms and greet you as an fellow spacer.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			e2.text.AddLine("The crew is transporting paying passengers between trader station and will be glad to barter for a fresh Nav Disk to a station or food and drink.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		}
		else
		{
			e2.text.AddLine("The captain of the ship contacts you via the comms. The crew is transporting paying passengers between trader station and will be glad to barter for a fresh Nav Disk to a station or food and drink.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		}
		e2.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_REFUGEE_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Makeshift Ship Barter";
		trader.BarterEnable(TradeSheet.GetRefugeeShipShop());
		List<string> tagList = new List<string> { "FOOD", "LIQUID_CONTAINER_TAG", "NAV_DISK_TRADER_STATION_TAG" };
		List<int> discountList = new List<int> { 20, 20, 50 };
		List<string> itemTags = new List<string> { "FOOD", "LIQUID_CONTAINER_TAG", "NAV_DISK_TRADER_STATION_TAG" };
		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
		e2.AddChoice(DirectoryMaster.Item("trade"), "Barter", "", null, null, delegate
		{
			trader.BarterShow();
		});
		e2.AddChoice(DirectoryMaster.Item("ok"), "(Hostile) Open fire on the makeshift ship.", "", null, null, delegate
		{
			e2.ForceEnd();
		});
		e2.AddChoice(DirectoryMaster.Item("ok"), "(Hostile) Rob the makeshift ship.", "", null, null, delegate
		{
			e2.ForceEnd();
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing.", "", null, null, delegate
		{
			e2.ForceEnd();
		});
		return e2;
	}
}

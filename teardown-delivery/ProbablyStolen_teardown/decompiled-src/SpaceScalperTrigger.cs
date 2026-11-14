public class SpaceScalperTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_SCALPER_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("scalper_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Travelling Merchant", "You arrive at a cargo ship. The captain contacts you and introduce themself as business man. They sell critially needed supplies at a premium price.");
		eventData.SetImage("event_scalper");
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Travelling Merchant";
		trader.BarterEnable(TradeSheet.GetTravellingMerchantShop());
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_SCALPER_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		eventData.AddChoice(DirectoryMaster.Item("trade"), "(Trade) See what they have to offer.", "", null, null, delegate
		{
			trader.BarterShow();
		});
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		eventData.AddChoice(DirectoryMaster.Item("rob"), "(Hostile) Rob the scalper.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
		{
		});
		eventData.AddChoice(DirectoryMaster.Item("open_fire"), "(Hostile) Open fire on the scalper.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
		{
		});
		return eventData;
	}
}

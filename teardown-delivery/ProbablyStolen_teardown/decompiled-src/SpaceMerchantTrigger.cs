public class SpaceMerchantTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_MERCHANT_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("merchant_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Travelling Merchant", "You arrive at a cargo ship. The captain contacts you and introduce themself as travelling merchant. They sell general supplies and are willing to buy all kind of items.");
		eventData.SetImage("merchant");
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Travelling Merchant";
		trader.BarterEnable(TradeSheet.GetTravellingMerchantShop());
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_MERCHANT_EVENT_STARTED");
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
		eventData.AddChoice(DirectoryMaster.Item("ok"), "(Hostile) Rob the merchant ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
		{
		});
		eventData.AddChoice(DirectoryMaster.Item("ok"), "(Hostile) Open fire on the merchant ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
		{
		});
		return eventData;
	}
}

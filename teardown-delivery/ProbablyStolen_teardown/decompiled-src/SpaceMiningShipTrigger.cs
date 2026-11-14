using System.Collections.Generic;

public class SpaceMiningShipTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_MINING_SHIP_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("mining_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Mining Ship", "You arrive at a small mining ship. It is equipped with a set of mining drill and behind it a large cargo hold.");
		eventData.SetImage("event_miner");
		eventData.text.AddLine("\r\nThe ship captain contacts you and ask if you are here to take Offload.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_MINING_SHIP_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Miner's Offload (Credit Only)";
		trader.BarterEnable(TradeSheet.GetMinerOffload());
		List<string> tagList = new List<string> { "ORE_TAG" };
		List<int> discountList = new List<int> { -50 };
		List<string> itemTags = new List<string> { "power_source_item", "NAV_DISK_MINING_TAG" };
		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
		eventData.AddChoice(DirectoryMaster.Item("trade"), "Yes", "", null, null, delegate
		{
			trader.BarterShow();
		});
		eventData.AddChoice(DirectoryMaster.Item("question"), "What is Offloading?", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\nThe ship captain explain to you that Offloading is when a miner sell their ore at a discount to a cargo ship, so they can return to mining and the cargo ship will take the ore and sell it at a refinery station for a profit.\r\nIt is more profitable for them to Offload and keep mining than transporting and finding a trade station with a refinery themself.", RenderHandler.ColorPalette.LightGray));
		});
		eventData.AddChoice(DirectoryMaster.Item("ok"), "Is there anything we can sell you?", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\nThe ship captain tell you that if you have visited a asteroid suitable for mining recently, they will gladly pay for the nav disk.", RenderHandler.ColorPalette.LightGreen));
		});
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return eventData;
	}
}

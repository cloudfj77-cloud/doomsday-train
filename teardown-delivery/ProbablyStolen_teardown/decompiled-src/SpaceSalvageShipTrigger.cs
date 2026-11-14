using System.Collections.Generic;

public class SpaceSalvageShipTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_SALVAGE_SHIP_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("salvage_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Salvage Ship", "You arrive at a small mining ship. It is equipped with cranes and robotic cutters and behind it a large cargo hold.");
		eventData.SetImage("event_scav");
		eventData.text.AddLine("\r\nThe ship captain contacts you and ask if you are here to take Offload.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_SALVAGE_SHIP_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Scav's Offload (Credit Only)";
		trader.BarterEnable(TradeSheet.GetSalvageOffload());
		List<string> tagList = new List<string> { "SCRAP_METAL", "MATERIAL_TAG", "CONSTRUCTION_MATERIAL_TAG" };
		List<int> discountList = new List<int> { -50, -50, -50 };
		List<string> itemTags = new List<string> { "power_source_item", "NAV_DISK_SALVAGE_TAG" };
		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
		eventData.AddChoice(DirectoryMaster.Item("trade"), "Yes", "", null, null, delegate
		{
			trader.BarterShow();
		});
		eventData.AddChoice(DirectoryMaster.Item("question"), "What is Offloading?", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\nThe ship captain explain to you that Offloading is when a scavenger sell their finding at a discount to a cargo ship, so they can return to salvaging and the cargo ship will take the finding and sell it at a trade station for a profit.\r\nIt is more profitable for them to Offload and keep salvaging than transporting, finding a trade station and sell the goods themself.", RenderHandler.ColorPalette.LightGreen));
		});
		eventData.AddChoice(DirectoryMaster.Item("ok"), "Is there anything we can sell you?", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\nThe ship captain tell you that if you have visited a fresh shipwreck recently, they are interested to buy the nav disk to that location.", RenderHandler.ColorPalette.LightGreen));
		});
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return eventData;
	}
}

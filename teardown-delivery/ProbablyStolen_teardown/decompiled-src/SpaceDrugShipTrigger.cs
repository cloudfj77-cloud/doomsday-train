using System.Collections.Generic;

public class SpaceDrugShipTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_DRUG_LAB_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("drug_lab_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Drug Lab Ship", "You arrive at the location of the broadcast. An inconspicuous cargo ship is the origin of the broadcast.");
		eventData.SetImage("drug_deal");
		eventData.text.AddLine("\r\nYou are contacted by their ship's captain, welcoming you and ask if you would like to browse their ware. They proudly annonce that everything on sale is made locally on their ship.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		eventData.text.AddLine("\r\nThey only accept Energy Credit as currency.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_DRUG_LAB_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Drug Lab (Credit Only)";
		trader.BarterEnable(TradeSheet.GetDrugShipShop());
		List<string> tagList = new List<string> { "ITEM_MEDICAL_TAG" };
		List<int> discountList = new List<int> { -50 };
		List<string> itemTags = new List<string> { "power_source_item" };
		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
		eventData.AddChoice(DirectoryMaster.Item("trade"), "Browse wares.", "", null, null, delegate
		{
			trader.BarterShow();
		});
		eventData.AddChoice(DirectoryMaster.Item("question"), "Ask for tip getting contraband past security checkpoint in the trade stations.", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\nblablabal", RenderHandler.ColorPalette.LightGray));
		});
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return eventData;
	}
}

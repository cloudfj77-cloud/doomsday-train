using System.Collections.Generic;

public class SpaceFuelTankerTrigger : EventTrigger
{
	public const string FUEL_TANKER_VISITED = "FUEL_TANKER_VISITED";

	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_FUEL_TANKER_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("stolen_fuel_tanker") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Fuel Tanker", "You arrive near a large tanker ship transporting exotic fuel. You are immediately contacted by the tanker's captain who seems eager.");
		eventData.SetImage("event_stolen_fuel");
		eventData.text.AddLine("\r\n`Want to buy some dirt cheap fuel? 50% off the market price! We take Credit only.'", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		eventData.text.AddLine("\r\nYou don't detect the ship's transponder.", ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Dirt cheap fuel (Energy Credit Only)";
		trader.BarterEnable(TradeSheet.GetStolenFuelStore());
		List<string> tagList = new List<string> { "EXOTIC_FUEL_TAG" };
		List<int> discountList = new List<int> { -50 };
		List<string> itemTags = new List<string> { "power_source_item" };
		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
		eventData.startAction = delegate
		{
			GameMaster.current.worldTags.EnableTag("FUEL_TANKER_VISITED");
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_FUEL_TANKER_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
			NavDiskHelper.HandleNavDiskOnArrivalToPoi();
		};
		eventData.AddChoice(DirectoryMaster.Item("trade"), "(Trader) Check out the fuel.", "", null, null, delegate
		{
			trader.BarterShow();
		});
		eventData.AddChoice(DirectoryMaster.Item("question"), "Why is it so cheap?", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n`Because we got it really cheap too.'", RenderHandler.ColorPalette.LightGray));
			GameMaster.current.worldTags.EnableTag("SPACE_FUEL_TANKER_EVENT_STARTED_STAGE_1");
		});
		eventData.AddChoice(DirectoryMaster.Item("question"), "You stole the tanker didn't you?", "", null, () => GameMaster.current.worldTags.IsTag("SPACE_FUEL_TANKER_EVENT_STARTED_STAGE_1"), delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n`Hey! Are you gonna buy some fuel or not?'", RenderHandler.ColorPalette.LightGray));
		});
		eventData.AddChoice(DirectoryMaster.Item("ok"), "(Hostile) Rob the fuel tanker", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n`What are you going to do if I refuse? Shoot this FUEL TANKER? Man, either buy the fuel or get the hell out of here.'", RenderHandler.ColorPalette.LightGray));
		});
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return eventData;
	}
}

using System.Collections.Generic;

public class SpaceButcherTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_BUTCHER_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("butcher_xeno_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Friendly Butcher Shop", "You arrive near a cargo ship. You are immediatly contacted by a man wearing a stained apron and holding a butcher knife: ");
		eventData.text.AddLine("\r\n    'Are you here to buy xeno meat? Energy Credit only'.", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
		eventData.text.AddLine("You can hear the sound of a cleaver and something dying painfully in the background.");
		eventData.SetImage("event_butcher");
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Xeno Butcher (Energy Credit Only)";
		trader.BarterEnable(TradeSheet.GetXenoButcherShop());
		List<string> tagList = new List<string> { "XENO_MEAT_TAG" };
		List<int> discountList = new List<int> { -20 };
		List<string> itemTags = new List<string> { "power_source_item" };
		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_BUTCHER_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		eventData.AddChoice(DirectoryMaster.Item("trade"), "(Browse Shop)", "", null, null, delegate
		{
			trader.BarterShow();
		});
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		bool questionAsked = false;
		eventData.AddChoice(DirectoryMaster.Item("question"), "Ask what are xeno meat and why anyone would buy it.", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("The butcher is annoyed by your question, but still anwser to it:  'Those meat are highly sought after because some believe that it will enhance their bedroom prowess, other believe it has unique medicinal proprieties, many buy it because it is expensive and eating it is demonstrate high social status. But I like it because of it's unique texture and flavor, it's a delicacy.  You have to be careful as this meat is banned in most station.'", RenderHandler.ColorPalette.LightGray));
			questionAsked = true;
		});
		eventData.AddChoice(DirectoryMaster.Item("question"), "Why are xeno meat banned in most stations?", "", null, () => questionAsked, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add(" Because of how dangerous ranching xeno is. The xeno are very smart sooner or later a rancher will slip and make a mistake and the xenos will escape. It will hide on the station, ambush anyone alone, start a hive and reproduce until they have the numbers to take the whole station. It only take one xeno to overrun a station. If you ever stumble on a station infested with xeno, that's probably a xeno ranching operation gone wrong.'", RenderHandler.ColorPalette.LightGray));
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add(" 'So they banned the xeno meat thinking it will reduce consumption and thus, lower demand and then the supply. In practice' demand for xeno meat has skyrocket after the ban, the meat has become even more desirable now that it is illegal. So if you can smuggle some in a station, you can flip those for big profit.'", RenderHandler.ColorPalette.LightGray));
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add(" 'Anyway, are you gonna buy some or what?'", RenderHandler.ColorPalette.LightGray));
		});
		return eventData;
	}
}

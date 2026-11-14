public class SpaceMedicTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_MEDIC_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("hospital_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Friendly Hospital Ship", "You arrive near a large white ship.");
		eventData.SetImage("spacer_doc");
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Hippocrates Order Donation Box";
		trader.BarterEnable(TradeSheet.GetDonationShop());
		eventData.text.AddLine("\r\nThe leader of the ship contacts you and introduce themself:\r\n'Hello, We are the Hippocrates Order and I am the chief medical officier of this ship. We try to provide free healthcare to whoever need it.'.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_MEDIC_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		eventData.AddChoice(DirectoryMaster.Item("question"), "Ask more information about the Hippocrates Order", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n'The Hippocrates Order are a group of people who believe in free universal healthcare, between the for profit hospitals and Carmicheal's prices on their medication, nobody can afford healthcare anymore.' So we try our best to help anyone in need of medical attention for free. ", RenderHandler.ColorPalette.LightGray));
		});
		eventData.AddChoice(DirectoryMaster.Item("question"), "How does the Hippocrates Order afford all this?", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n'Everything we have are funded with donation, there's many out there who share our belief and willing to give. You are welcome to make a donation too.' We will use the credit to pay for more staff and field better equipment.", RenderHandler.ColorPalette.LightGray));
		});
		eventData.AddChoice(DirectoryMaster.Item("trade"), "(Donate) Consider a donation to the Hippocrates Order.", "", null, null, delegate
		{
			trader.BarterShow();
		});
		eventData.AddChoice(DirectoryMaster.Item("ok"), "(Hostile) Rob the hospital ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
		{
		});
		eventData.AddChoice(DirectoryMaster.Item("ok"), "(Hostile) Open fire on the hospital ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
		{
		});
		bool used = false;
		eventData.AddChoice(DirectoryMaster.Item("heal"), "Get medical attention for the crew.", "", () => !used, null, delegate
		{
			foreach (GameCharacterItem item in GameMaster.current.shipManager.GetCrew())
			{
				GCIHelper.BasicFullHeal(item);
				GCIHelper.Bandage(item);
			}
			used = true;
		});
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return eventData;
	}
}

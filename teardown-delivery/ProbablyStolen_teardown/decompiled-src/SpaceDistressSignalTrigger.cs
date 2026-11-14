using System.Collections.Generic;

public class SpaceDistressSignalTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_DISTRESS_SIGNAL_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("distree_signal") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData e2 = new EventData().SetText("Ship Distress Signal", "You have arrived to the source of the distress signal, a small stationary cargo ship. ");
		e2.SetImage("event_distress");
		e2.text.AddLine("\n        The ship captain has contacted you and ask for your help, they are stranded without fuel and energy. They are offering the cargo they are transporting at a generous discount in exchange for fuel cell and energy credit.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Distressed Cargo (Credit & Fuel Only)";
		trader.BarterEnable(TradeSheet.GetDistressSignalCargoShop());
		List<string> tagList = new List<string> { "FOOD", "ITEM_MEDICAL_TAG", "backpack" };
		List<int> discountList = new List<int> { -50, -50, -50 };
		List<string> itemTags = new List<string> { "power_source_item", "EXOTIC_FUEL_TAG" };
		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
		e2.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_DISTRESS_SIGNAL_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		e2.AddChoice(DirectoryMaster.Item("coin"), "(Trade) See what they have to offer.", "", null, null, delegate
		{
			trader.BarterShow();
		});
		e2.AddChoice(DirectoryMaster.Item("coin"), "(Donate) Give 100 Energy Credit and 20 Exotic Fuel Cell.", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 100 && GameMaster.current.shipManager.GetTotalAvailableFuel() >= 20, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("The Crew is grateful that you gave them without asking for reward. The captain decided to offer you the nearby location where they dumped some cargo to reduce their fuel consumption.", RenderHandler.ColorPalette.LightGreen));
			GameMaster.current.worldTags.EnableTag("SPAWN_CARGO_POD_WORLD_TAG");
			GameMaster.current.shipManager.RemoveEnergyCredit(100);
			GameMaster.current.shipManager.RemoveExoticFuel(20);
			OverlayHandler.current.CreateNav(NavigationHelper.ScanForEncounters());
		});
		e2.AddChoice(DirectoryMaster.Item("coin"), "(Hostile) Threaten them to handover all of their cargo or you will open fire", "", null, null, delegate
		{
			GameMaster.current.shipManager.ship?.TryAcceptAll(DirectoryMaster.Item("coin").SetAmount(5));
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your crew has mine the asteroid", RenderHandler.ColorPalette.LightGreen));
			GameItem gameItem = DirectoryMaster.Item("backpack_small");
			GameItem item = DirectoryMaster.Item("red_injector");
			GraphUtils.TryAcceptAll(gameItem, item);
			OverlayHandler.current.ShowInventory(gameItem);
			e2.ForceEnd();
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now, It's not like they are going anywhere.", "", null, null, delegate
		{
		});
		return e2;
	}
}

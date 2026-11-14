using System.Collections.Generic;

public class SpaceRepairShipTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_REPAIR_SHIP_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("repair_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData e2 = new EventData().SetText("Repair Ship", "You arrive at a large repair ship. Aboard this ship, they have trained personnel and specialised equipments to repair any ship back to working order.");
		e2.SetImage("mechanic");
		e2.text.AddLine("\r\nThe repair ship's foreman greets you and ask you what do you need.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		int missingHullPoint = 0;
		e2.startAction = delegate(EventData e)
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_REPAIR_SHIP_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
			missingHullPoint = GameMaster.current.shipManager.CheckMissingHullPoint();
			if (missingHullPoint == 0)
			{
				e.text.AddLine("\r\nYour ship's hull is currently in perfect condition.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			else
			{
				e.text.AddLine("\r\nYour ship is currently missing " + missingHullPoint + " hull point.", ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
			}
		};
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Repair Ship (Credit Only)";
		trader.BarterEnable(TradeSheet.GetRepairShipShop());
		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(new List<string> { "power_source_item" }, trader);
		int juryPrice = missingHullPoint;
		int replacePrice = missingHullPoint * 2 + 10;
		e2.AddChoice(DirectoryMaster.Item("ok"), "(" + juryPrice + " Credit) Pay for Jury-Rigged Repair service.", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= juryPrice, null, delegate
		{
			GameMaster.current.shipManager.RemoveEnergyCredit(juryPrice);
			GameMaster.current.shipManager.RepairFullHull(juryRigged: true);
			e2.text.AddLine("\r\nYour ship was repaired to full, but the overral maximal hull intergrety has been reduced.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		});
		e2.AddChoice(DirectoryMaster.Item("ok"), "(" + replacePrice + " Credit) Pay for Replace and Repair service.", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= replacePrice, null, delegate
		{
			GameMaster.current.shipManager.RemoveEnergyCredit(replacePrice);
			GameMaster.current.shipManager.RepairFullHull(juryRigged: false);
			e2.text.AddLine("\r\nYour ship has been repair using proper replacement parts and it is now in factory new condition.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		});
		e2.AddChoice(DirectoryMaster.Item("trade"), "Browse wares.", "", null, null, delegate
		{
			trader.BarterShow();
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return e2;
	}
}

public class TraderStationInspectionTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("TRADER_STATION_INSPECTION_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldStation { isHostile: false } overworldStation && !GameMaster.current.shipManager.isTransit && overworldStation.generatedStation.GetArrivalEvent().Equals("INSPECTION"))
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData e2 = new EventData().SetText("Trade Station Arrival", "You have arrived at a space station!");
		e2.text.AddLine("\r\n");
		if (GameMaster.current.shipManager.nextPOI is OverworldStation overworldStation)
		{
			e2.text.Add(overworldStation.generatedStation.CreateStationDescription());
		}
		e2.text.AddLine("\r\nThe port authority greets your ship and a custom officier is demanding a inspection of the cargo hold before you can visit the station.", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
		e2.SetImage("event_inspection");
		e2.startAction = delegate(EventData e)
		{
			e.BlockStation();
			TagState tag = GameMaster.current.worldTags.GetTag("TRADER_STATION_INSPECTION_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		bool refusedInspection = false;
		e2.AddChoice(DirectoryMaster.Item("$"), "(Bribe) Offer 100 Energy Credit to avoid inspection.", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 100 && !refusedInspection, null, delegate
		{
			OfferSmallBribe(e2);
		});
		e2.AddChoice(DirectoryMaster.Item("$$"), "(Bribe) Offer 200 Energy Credit to avoid inspection.", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 200 && !refusedInspection, null, delegate
		{
			OfferLargeBribe(e2);
		});
		e2.AddChoice(DirectoryMaster.Item("ok"), "Accept the inspection", "", () => !refusedInspection, null, delegate
		{
			ConductInspection(e2);
		});
		e2.AddChoice(DirectoryMaster.Item("cancel"), "Refuse the inspection and leave the station.", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   You refuse the inspection and leave quickly before the Station's border service can pursuit you.", RenderHandler.ColorPalette.LightYellow));
			refusedInspection = true;
		});
		return e2;
	}

	public static void ConductInspection(EventData e)
	{
		OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier arrived on your ship and begin the inspection... ", RenderHandler.ColorPalette.LightYellow));
		var (num, num2) = GameMaster.current.shipManager.InspectCargoHold();
		if (num == 0)
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The inspection is finished and no contraband was found, you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
		}
		else
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The inspection is finished and " + num + " contrabands were found!", RenderHandler.ColorPalette.LightRed));
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The countrabands items are confiscated and you are order to be fined " + num2 + " Credit for attempting to smuggle contraband!", RenderHandler.ColorPalette.LightRed));
			if (GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= num2)
			{
				GameMaster.current.shipManager.RemoveEnergyCredit(num2);
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   You paid the fine and you are now clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
			}
			else
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   You do not have enough Credit to paid the fine. Your crew was arrested and jailed by the Station's security force.", RenderHandler.ColorPalette.LightRed));
			}
		}
		e.blockStation = false;
	}

	public static void OfferSmallBribe(EventData e)
	{
		bool flag = false;
		if (GameMaster.current.shipManager.nextPOI is OverworldStation overworldStation)
		{
			flag = overworldStation.generatedStation.IsStationHaveCrisis();
		}
		if (flag)
		{
			if (RNG.Roll(5))
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   Even considering the ongoing economic hardship on their station, the custom officier still refuse the bribe and is on route with a security team to conduct an inspection!", RenderHandler.ColorPalette.LightRed));
				ConductInspection(e);
			}
			else
			{
				GameMaster.current.shipManager.RemoveEnergyCredit(100);
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   Considering the ongoing crisis on their station, the custom officier quickly accept the bribe and you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
				e.blockStation = false;
			}
		}
		else if (RNG.Roll(20))
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier refuse the bribe and is on route with a security team to conduct an inspection!", RenderHandler.ColorPalette.LightRed));
			ConductInspection(e);
		}
		else
		{
			GameMaster.current.shipManager.RemoveEnergyCredit(100);
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier accept the bribe and you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
			e.blockStation = false;
		}
	}

	public static void OfferLargeBribe(EventData e)
	{
		bool flag = false;
		if (GameMaster.current.shipManager.nextPOI is OverworldStation overworldStation)
		{
			flag = overworldStation.generatedStation.IsStationHaveCrisis();
		}
		if (flag)
		{
			if (RNG.Roll(1))
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   Even considering the ongoing economic hardship on their station, the custom officier still refuse the bribe and is on route with a security team to conduct an inspection!", RenderHandler.ColorPalette.LightRed));
				ConductInspection(e);
			}
			else
			{
				GameMaster.current.shipManager.RemoveEnergyCredit(100);
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   Considering the ongoing crisis on their station, the custom officier quickly accept the bribe and you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
				e.blockStation = false;
			}
		}
		else if (RNG.Roll(10))
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier refuse the bribe and is on route with a security team to conduct an inspection!", RenderHandler.ColorPalette.LightRed));
			ConductInspection(e);
		}
		else
		{
			GameMaster.current.shipManager.RemoveEnergyCredit(100);
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier accept the bribe and you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
			e.blockStation = false;
		}
	}
}

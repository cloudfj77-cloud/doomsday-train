public class TutorialTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("TUTORIAL_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("tutorial_space") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Welcome To Subspace", "");
		eventData.text.Add("    Your ship and crew has been mysteriously warped into the dreaded");
		eventData.text.Add(" Subspace", RenderHandler.ColorPalette.LightCyan);
		eventData.text.AddLine(" during its last voyage!");
		eventData.text.Add("    You must explore and find");
		eventData.text.Add(" Energy Credit", RenderHandler.ColorPalette.LightGreen);
		eventData.text.Add(" and");
		eventData.text.Add(" Fuel Cell", RenderHandler.ColorPalette.LightGreen);
		eventData.text.Add(" to keep your ship powered and moving.");
		eventData.text.Add(" Food", RenderHandler.ColorPalette.LightGreen);
		eventData.text.Add(",");
		eventData.text.Add(" Water", RenderHandler.ColorPalette.LightGreen);
		eventData.text.Add(" and");
		eventData.text.Add(" Meds", RenderHandler.ColorPalette.LightGreen);
		eventData.text.Add(" to keep your crew alive.");
		eventData.SetImage("event_subspace");
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("TUTORIAL_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		eventData.AddChoice(DirectoryMaster.Item("question"), "Basic Tutorial On Navigation System", "", null, null, delegate
		{
			for (int l = 0; l < 1; l++)
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine(" "));
			}
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine("Tutorial on Navigation System:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true).Add("   A Navigation System is present on every ship. When the ship is powered,", RenderHandler.ColorPalette.LightGreen).Add(" the Navigation System will draw energy from the ship to charge it self. Once fully charged, you can perform a scan that will reveal", RenderHandler.ColorPalette.LightGreen)
				.Add(" Points of Interest (POI).", RenderHandler.ColorPalette.LightCyan, "", bold: true)
				.Add(" Like most Ship System, the Navigation System can be upgraded by inserting", RenderHandler.ColorPalette.LightGreen)
				.Add(" System Module.", RenderHandler.ColorPalette.LightCyan, "", bold: true)
				.AddLine(" The number of POI scanned depends on the upgrade that the Navigation System have.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("  Points of Interest (POI)", RenderHandler.ColorPalette.LightCyan, "", bold: true).Add(" can be a friendly or hostile space station that you can", RenderHandler.ColorPalette.LightGreen).Add(" dock", RenderHandler.ColorPalette.LightCyan, "", bold: true)
				.Add(" with and explore with your crew.", RenderHandler.ColorPalette.LightGreen)
				.AddLine(" POI can also be a unique or recurring event that take place in space.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
		});
		eventData.AddChoice(DirectoryMaster.Item("question"), "Basic Tutorial On Propulsion System", "", null, null, delegate
		{
			for (int k = 0; k < 1; k++)
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine(" "));
			}
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine("Tutorial on Propulsion System:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true).Add("  A Propulsion System is present on every ship. It's used to move the ship between Points of Interest.", RenderHandler.ColorPalette.LightGreen).Add(" It only consume", RenderHandler.ColorPalette.LightGreen)
				.Add(" Exotic Fuel Cell", RenderHandler.ColorPalette.LightCyan, "", bold: true)
				.AddLine(" placed inside the System's inventory", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("  Like most Ship System, the Propulsion System can be upgraded by inserting", RenderHandler.ColorPalette.LightGreen).Add(" System Module.", RenderHandler.ColorPalette.LightCyan, "", bold: true).AddLine(" Propulsion Speed and Fuel Consumption depends on the System's upgrade.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
		});
		eventData.AddChoice(DirectoryMaster.Item("question"), "Basic Tutorial On Ship Hull", "", null, null, delegate
		{
			for (int j = 0; j < 1; j++)
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine(" "));
			}
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine("Tutorial on Ship Hull:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true).Add("  The hull of your ship is the only thing protecting the crew against the vaccum of subspace!", RenderHandler.ColorPalette.LightGreen).Add(" If the hull's", RenderHandler.ColorPalette.LightGreen)
				.Add(" Integrity", RenderHandler.ColorPalette.LightCyan, "", bold: true)
				.Add(" reach 0, the ship will break and the crew will be killed!", RenderHandler.ColorPalette.LightGreen)
				.Add(" Armor", RenderHandler.ColorPalette.LightCyan, "", bold: true)
				.AddLine(" can reduce the damage the Hull take.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("  Ship Hull can be repaired at a", RenderHandler.ColorPalette.LightGreen).Add(" Ship Mechanic. Maintenance Rate", RenderHandler.ColorPalette.LightCyan, "", bold: true).AddLine(" is a multiplier defining how easy a Hull can be repaired.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
		});
		eventData.AddChoice(DirectoryMaster.Item("question"), "Basic Tutorial On crew survival", "", null, null, delegate
		{
			for (int i = 0; i < 1; i++)
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine(" "));
			}
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine("Tutorial on Crew Survival:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true).AddLine(" All crewmate have a", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen).Add(" Thirst", RenderHandler.ColorPalette.LightCyan, "", bold: true)
				.Add(" stat. If it reaches 0, the crewmate will die.", RenderHandler.ColorPalette.LightGreen)
				.Add(" Low thirst will give a penalty to that crewmate in combat,", RenderHandler.ColorPalette.LightOrange)
				.Add(" so make sure the crew stays hydrated. Drinks can be found in POI all over the Subspace.", RenderHandler.ColorPalette.LightGreen));
		});
		eventData.AddChoice(DirectoryMaster.Item("ok"), "Begin.", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You can start exploring the Subspace by starting a Scan in the ", RenderHandler.ColorPalette.LightGreen).Add("Navigation", RenderHandler.ColorPalette.LightCyan, "", bold: true).Add(" Interface", RenderHandler.ColorPalette.LightGreen));
		});
		return eventData;
	}
}

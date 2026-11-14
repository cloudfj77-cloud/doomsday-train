public class NewBehaviourScript : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_WRECK_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("shipwreck") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		int wreckState = RNG.GetRandomInt(0, 2);
		int shipSize = RNG.GetRandomInt(0, 2);
		string text = "";
		if (shipSize == 0)
		{
			text = "small";
		}
		else if (shipSize == 2)
		{
			text = "large";
		}
		string text2 = "";
		if (wreckState == 0)
		{
			text2 = "shattered";
		}
		else if (wreckState == 1)
		{
			text2 = "ruptured";
		}
		else if (wreckState == 2)
		{
			text2 = "disabled";
		}
		EventData eventData = new EventData().SetText("Shipwreck", "You have arrived to an " + text + " derelict ship. The delirect ship is " + text2 + ", you can salvage something from it. ");
		eventData.text.AddLine("\r\nYour ship has a total salvage power of " + GameMaster.current.shipManager.GetShipSalvageCapacity(), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		bool showLootOption = false;
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_WRECK_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
			NavDiskHelper.HandleNavDiskOnArrivalToPoi();
		};
		GameItem container = DirectoryMaster.Item("lootbox_medium");
		int debrisLevel = 0;
		int damageTaken = 0;
		eventData.AddChoice(DirectoryMaster.Item("recycle"), "Salvage the derelict ship.", "", null, null, delegate
		{
			OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
			{
				GameMaster.current.turnManager.NextTurn();
			}, null, delegate
			{
				AudioManager.Instance.Play("WELD");
			});
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your crew has salvage part of the wreck.", RenderHandler.ColorPalette.LightGreen));
			int shipSalvageCapacity = GameMaster.current.shipManager.GetShipSalvageCapacity();
			(damageTaken, debrisLevel) = SalvageSystemHelper.SalvageWreck(container, debrisLevel, shipSalvageCapacity, wreckState, shipSize);
			if (damageTaken != 0)
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("A collision has occured with the debris!", RenderHandler.ColorPalette.LightRed));
				SpaceBattleHelper.TakingHit(damageTaken);
			}
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Current debris level: " + debrisLevel + "%", RenderHandler.ColorPalette.LightYellow));
			showLootOption = true;
			OverlayHandler.current.ShowInventory(container);
		});
		eventData.AddChoice(DirectoryMaster.Item("loot"), "Check Salvaged Items.", "", () => showLootOption, null, delegate
		{
			OverlayHandler.current.ShowInventory(container);
		});
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return eventData;
	}
}

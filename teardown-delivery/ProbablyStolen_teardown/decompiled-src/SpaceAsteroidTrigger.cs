public class SpaceAsteroidTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_ASTEROID_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("mining_asteroid") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		int mineralLevel = RNG.GetRandomInt(0, 2);
		EventData eventData = new EventData().SetText("Mineral Rich Asteroid", "You have arrived to an large asteroid.");
		eventData.SetImage("event_asteroid");
		string text = "";
		if (mineralLevel == 0)
		{
			text = "adequat";
		}
		if (mineralLevel == 1)
		{
			text = "rich";
		}
		if (mineralLevel == 2)
		{
			text = "very rich";
		}
		int timeMined = 0;
		eventData.text.AddLine("\r\nScanner indicates that it is " + text + " in mineral content.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		eventData.text.AddLine("\r\nYour ship has a total mining power of " + GameMaster.current.shipManager.GetShipMiningCapacity(), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_ASTEROID_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
			NavDiskHelper.HandleNavDiskOnArrivalToPoi();
		};
		GameItem container = DirectoryMaster.Item("lootbox_medium");
		bool canCheckLootbag = false;
		eventData.AddChoice(DirectoryMaster.Item("mine"), "(End Turn) Mine the asteroid", "", null, null, delegate
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your crew has spent some time mining the asteroid."));
			OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
			{
				GameMaster.current.turnManager.NextTurn();
			}, null, delegate
			{
				AudioManager.Instance.Play("MINING_DRILL");
			});
			var (num, normalOre, rareOre) = MiningHelper.MineAsteroid(timeMined, mineralLevel, GameMaster.current.shipManager.GetShipMiningCapacity());
			if (num > 0)
			{
				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("An accident has occurs during mining!", RenderHandler.ColorPalette.LightRed));
				SpaceBattleHelper.TakingHit(num);
			}
			MiningHelper.AddMinedOre(container, normalOre, rareOre);
			timeMined++;
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("We have mined the asteroid " + timeMined + " time", RenderHandler.ColorPalette.LightYellow));
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("With the debris floating around from the mining operation there is a " + MiningHelper.GetDangerLevel(timeMined), RenderHandler.ColorPalette.LightYellow));
			OverlayHandler.current.ShowInventory(container);
			OverlayHandler.current.ShowEvent();
			canCheckLootbag = true;
		});
		eventData.AddChoice(DirectoryMaster.Item("loot"), "Check Mined ore.", "", () => canCheckLootbag, null, delegate
		{
			OverlayHandler.current.ShowInventory(container);
		});
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing For now.", "", null, null, delegate
		{
		});
		return eventData;
	}
}

public class SpaceCargoPodTrigger : EventTrigger
{
	public override bool? Trigger()
	{
		GameMaster.current.worldTags.GetTag("SPACE_CARGO_POD_EVENT_STARTED");
		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("cargo_pod") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
		{
			return true;
		}
		return null;
	}

	public override EventData GenerateEvent()
	{
		EventData eventData = new EventData().SetText("Drifting Cargo Pods", "You arrive near some cargo pod. For whatever reason they has been spaced and is drifting indefinetly.");
		eventData.SetImage("event_cargo_pod2");
		eventData.text.AddLine("\r\n        Your on-board scanners shows no abnormality.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		eventData.startAction = delegate
		{
			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_CARGO_POD_EVENT_STARTED");
			tag.SetInt(tag.GetInt() + 1);
			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
			{
				overworldSpace.isEventTriggered = true;
			}
		};
		GameItem container = DirectoryMaster.Item("lootbox_medium");
		FillCargoPod(container);
		eventData.AddChoice(DirectoryMaster.Item("loot"), "Secure and open the cargo pod.", "", null, null, delegate
		{
			OverlayHandler.current.ShowInventory(container);
		});
		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
		{
		});
		return eventData;
	}

	public static void FillCargoPod(GameItem container)
	{
		int randomInt = RNG.GetRandomInt(0, 6);
		if (randomInt == 0)
		{
			for (int i = 0; i < 6; i++)
			{
				GameItem item = DirectoryMaster.Item("raw_xeno_meat");
				GraphUtils.TryAcceptAll(container, item);
			}
		}
		if (randomInt == 1)
		{
			for (int j = 0; j < 4; j++)
			{
				GameItem item2 = DirectoryMaster.Item("energy_credit");
				GraphUtils.TryAcceptAll(container, item2);
			}
		}
		if (randomInt == 2)
		{
			for (int k = 0; k < 4; k++)
			{
				GameItem item3 = DirectoryMaster.Item("box_tampon");
				GraphUtils.TryAcceptAll(container, item3);
			}
		}
		if (randomInt == 3)
		{
			for (int l = 0; l < 6; l++)
			{
				GameItem item4 = DirectoryMaster.Item("pack_rib_sandwich");
				GraphUtils.TryAcceptAll(container, item4);
			}
		}
		if (randomInt == 4)
		{
			for (int m = 0; m < 8; m++)
			{
				GameItem item5 = DirectoryMaster.Item("common_ore");
				GraphUtils.TryAcceptAll(container, item5);
			}
		}
		if (randomInt == 5)
		{
			GraphUtils.TryAcceptAll(container, DropDirectory.FullFirstAidBox());
		}
		if (randomInt == 6)
		{
			for (int n = 0; n < 12; n++)
			{
				GameItem item6 = DirectoryMaster.Item("nutrifruit_packaged");
				GraphUtils.TryAcceptAll(container, item6);
			}
		}
	}
}

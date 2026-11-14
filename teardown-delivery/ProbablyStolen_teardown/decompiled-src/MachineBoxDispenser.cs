public class MachineBoxDispenser
{
	public static GameItem BoxDispenser()
	{
		var (contentWindow, itemInventory, outputGrid, machineryButton) = CreateMachineInventoryWindow();
		outputGrid.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) ? true : false;
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_tool", "bottle_printer");
		gameItem.name = "Box Dispenser";
		gameItem.flavorText = "A simple container that can dispense boxes";
		MachineHelper.SetupBatterySlot(itemInventory, gameItem);
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 250L;
		machineryButton.SetCallback(delegate
		{
			int amount = 1;
			if (itemInventory.childItems.Count != 0 && PowerHelper.CanDrawPowerSource(itemInventory.childItem, amount))
			{
				GameItem gameItem2 = DirectoryMaster.Item("ammo_box");
				if (GraphUtils.CanAccept(outputGrid, gameItem2))
				{
					GraphUtils.TryAcceptAll(outputGrid, gameItem2);
					PowerHelper.DrawPowerSource(itemInventory.childItem, amount);
				}
				else
				{
					gameItem2.Destroy();
				}
			}
		});
		return gameItem;
	}

	public static (PixelWindow, GameSlotInventory, GameInventory, MachineryButton) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory(2, 3);
		MachineryButton machineryButton = new MachineryButton("Dispense");
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel("Output", gameGridInventory, "output"), MachineHelper.Panel("Control", machineryButton, "control")), gameSlotInventory, gameGridInventory, machineryButton);
	}
}

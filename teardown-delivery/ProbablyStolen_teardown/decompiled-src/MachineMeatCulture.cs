using System.Collections.Generic;
using System.Linq;

public class MachineMeatCulture
{
	public static GameItem AutomaticMeatCultureLab()
	{
		var (contentWindow, itemInventoryLeft, itemInventoryRight) = CreateDoubleInventoryWindowSlotInput(8, 8);
		itemInventoryLeft.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => (item.identifier.Equals("small_raw_meat") || item.identifier.Equals("raw_meat")) ? true : false;
		((GameSlotInventory)itemInventoryLeft).SetBackgroundFadeSprite("Items/items_food", "raw_meat");
		itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_machine", "meat_culture_machine");
		gameItem.name = "Automatic Meat Culture Lab";
		gameItem.shortDescription = "Fully automatic meat culture lab that produce fresh lab grown meat from a sample";
		gameItem.EnableTag("manufacture");
		gameItem.SetGameItemType("MACHINE");
		gameItem.onCycleEndSlotItemFunc = delegate
		{
			itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
			List<GameItem> source = GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "small_raw_meat");
			List<GameItem> source2 = GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "raw_meat");
			if (source.Any())
			{
				GraphUtils.EmporiumTryAdd(itemInventoryRight, DirectoryMaster.Item("small_raw_meat"));
			}
			else if (source2.Any())
			{
				GraphUtils.EmporiumTryAdd(itemInventoryRight, DirectoryMaster.Item("raw_meat"));
			}
			itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		};
		return gameItem;
	}

	public static (PixelWindow, GameInventory, GameInventory) CreateDoubleInventoryWindowSlotInput(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory(width, height);
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.Panel(null, gameSlotInventory, "input"), MachineHelper.Panel(null, gameGridInventory, "storage")), gameSlotInventory, gameGridInventory);
	}
}

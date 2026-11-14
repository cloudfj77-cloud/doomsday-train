public class EquipmentDirectory : ItemDirectory
{
	protected override void InitDirectory()
	{
		Add("expedition_box", () => ExpeditionBox());
	}

	private GameItem ExpeditionBox()
	{
		PixelWindow item = CreateExpeditionBoxWindow(8, 12).Item1;
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_container", "expedition_box").SetContentWindow(item);
		gameItem.name = LocHelper.GetLocalizedItem("item_expedition_box_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_expedition_box_desc");
		gameItem.unitValue = 0L;
		return gameItem;
	}

	public static (PixelWindow, GameInventory, GameInventory, GameInventory, GameInventory, GameInventory) CreateExpeditionBoxWindow(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 25)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory(4, 4);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory(4, 4);
		gameSlotInventory.SetBackgroundFadeSprite("Items/items_weapons_melee", "crowbar");
		gameSlotInventory2.SetBackgroundFadeSprite("Items/items_weapons_melee", "crowbar");
		GameGridInventory gameGridInventory = new GameGridInventory(2, 4);
		GameGridInventory gameGridInventory2 = new GameGridInventory(2, 4);
		GameSlotInventory gameSlotInventory3 = new GameSlotInventory(4, 4, stealUI: true);
		gameSlotInventory3.SetBackgroundFadeSprite("Items/items_backpack2", "simple_backpack");
		gameSlotInventory3.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 5;
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
		gameGridInventory2.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.Panel(LocHelper.GetLocalizedItem("item_expedition_box_label_primary"), gameSlotInventory, "primary"), MachineHelper.Panel(LocHelper.GetLocalizedItem("item_expedition_box_label_secondary"), gameSlotInventory2, "secondary"), MachineHelper.Panel(LocHelper.GetLocalizedItem("item_expedition_box_label_pocket1"), gameGridInventory, "pocket1"), MachineHelper.Panel(LocHelper.GetLocalizedItem("item_expedition_box_label_pocket2"), gameGridInventory2, "pocket2"), MachineHelper.Panel(LocHelper.GetLocalizedItem("item_expedition_box_label_backpack"), gameSlotInventory3, "backpack")), gameSlotInventory, gameSlotInventory2, gameGridInventory, gameGridInventory2, gameSlotInventory3);
	}
}

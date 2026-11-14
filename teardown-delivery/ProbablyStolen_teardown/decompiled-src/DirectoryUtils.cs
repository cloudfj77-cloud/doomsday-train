using System;

public class DirectoryUtils
{
	public static (PixelWindow, GameInventory) CreateInventoryWindow(int width, int height, bool isDraggable = true)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable, null, WindowStyle.Machinery);
		GameGridInventory gameGridInventory = new GameGridInventory(width, height).SetMachineryStyle();
		pixelWindow.Attach(gameGridInventory);
		return (pixelWindow, gameGridInventory);
	}

	public static GameSlotInventory CreatePassengerSlot(bool countAsSeat = true, bool isPilotSeat = false)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory("Items/characteratlas", "character_outline");
		gameSlotInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item is GameCharacterItem;
		if (isPilotSeat)
		{
			gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		}
		else
		{
			gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		}
		if (countAsSeat)
		{
			gameSlotInventory.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameSlotInventory.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
			{
				if (addedItem is GameCharacterItem)
				{
					addedItem.EnableTag("GCI_SEATED_IN_SHIP_TAG");
				}
			});
			gameSlotInventory.onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameSlotInventory.onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
			{
				if (addedItem is GameCharacterItem)
				{
					addedItem.DisableTag("GCI_SEATED_IN_SHIP_TAG");
				}
			});
		}
		return gameSlotInventory;
	}

	public static (PixelWindow, GameInventory, GameInventory) CreateDoubleInventoryWindow(int width, int height, bool isDraggable = true)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1);
		GameGridInventory gameGridInventory = new GameGridInventory(width, height);
		GameGridInventory gameGridInventory2 = new GameGridInventory(width, height);
		gridPixelElement.Attach(gameGridInventory);
		gridPixelElement.Attach(gameGridInventory2);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameGridInventory, gameGridInventory2);
	}

	public static (PixelWindow, GameSlotInventory, GameSlotInventory, ButtonElement) CreatePasteDispencerWindow()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1, isCentered: true);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered: true);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 3, isCentered: true);
		TagElement element = new TagElement().SetText("Machine Output");
		TagElement element2 = new TagElement().SetText("Energy Credit");
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		ButtonElement buttonElement = new ButtonElement("Dispence", -1, -1, null, ButtonElement.Color.Green);
		gridPixelElement2.Attach(element, 10, 10);
		gridPixelElement2.Attach(gameSlotInventory, 10, 10);
		gridPixelElement3.Attach(element2, 10, 10);
		gridPixelElement3.Attach(gameSlotInventory2, 10, 10);
		gridPixelElement3.Attach(buttonElement, 10, 10);
		gridPixelElement.Attach(gridPixelElement2, 100, 100);
		gridPixelElement.Attach(gridPixelElement3, 100, 100);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameSlotInventory, gameSlotInventory2, buttonElement);
	}

	public static (PixelWindow, GameGridInventory) CreateBookWindow()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true, null, WindowStyle.Machinery);
		GameGridInventory gameGridInventory = new GameGridInventory(10, 10).SetMachineryStyle();
		pixelWindow.Attach(gameGridInventory);
		return (pixelWindow, gameGridInventory);
	}

	public static (PixelWindow, GameGridInventory) CreateGridWindow(int x, int y)
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true, null, WindowStyle.Machinery);
		GameGridInventory gameGridInventory = new GameGridInventory(x, y).SetMachineryStyle();
		pixelWindow.Attach(gameGridInventory);
		return (pixelWindow, gameGridInventory);
	}

	public static (PixelWindow, GameSlotInventory, GameSlotInventory, ButtonElement, ButtonElement) CreateWaterFountainWindow()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1, isCentered: true);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered: true);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered: true);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		GridPixelElement gridPixelElement4 = new GridPixelElement(1, 2, isCentered: true);
		GridPixelElement gridPixelElement5 = new GridPixelElement(1, 2, isCentered: true);
		TagElement element = new TagElement().SetText("Liquid Container");
		TagElement element2 = new TagElement().SetText("Energy Credit");
		gridPixelElement4.Attach(element, 10, 10);
		gridPixelElement4.Attach(gameSlotInventory, 10, 10);
		gridPixelElement5.Attach(element2, 10, 10);
		gridPixelElement5.Attach(gameSlotInventory2, 10, 10);
		gridPixelElement.Attach(gridPixelElement4, 100, 100);
		gridPixelElement.Attach(gridPixelElement2, 100, 100);
		gridPixelElement2.Attach(gridPixelElement5);
		gridPixelElement2.Attach(gridPixelElement3, 0, 25);
		ButtonElement buttonElement = new ButtonElement("Fill Bottle", -1, -1, null, ButtonElement.Color.Green);
		ButtonElement buttonElement2 = new ButtonElement("Drink", -1, -1, null, ButtonElement.Color.Green);
		gridPixelElement3.Attach(buttonElement, 0, 15);
		gridPixelElement3.Attach(buttonElement2, 0, 15);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameSlotInventory, gameSlotInventory2, buttonElement, buttonElement2);
	}

	public static (PixelWindow, GameSlotInventory, ButtonElement) CreateTrashPileWindow()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1, isCentered: true);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered: true);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered: true);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		gameSlotInventory.SetBackgroundFadeSprite("Items/items_material", "scrap_metal");
		GridPixelElement gridPixelElement4 = new GridPixelElement(1, 2, isCentered: true);
		TagElement element = new TagElement().SetText("Loot");
		gridPixelElement4.Attach(element, 10, 10);
		gridPixelElement4.Attach(gameSlotInventory, 10, 10);
		gridPixelElement.Attach(gridPixelElement4, 100, 100);
		gridPixelElement.Attach(gridPixelElement2, 100, 100);
		gridPixelElement2.Attach(gridPixelElement3, 0, 25);
		ButtonElement buttonElement = new ButtonElement("Search", -1, -1, null, ButtonElement.Color.Green);
		gridPixelElement3.Attach(buttonElement, 0, 15);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameSlotInventory, buttonElement);
	}

	public static (PixelWindow, GameGridInventory, ButtonElement) CreateIntelSpawnerWindow()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GridPixelElement gridPixelElement = new GridPixelElement(1, 1, isCentered: true);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered: true);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered: true);
		GameGridInventory gameGridInventory = new GameGridInventory(10, 10);
		gridPixelElement.Attach(gridPixelElement2, 100, 100);
		gridPixelElement2.Attach(gridPixelElement3, 0, 25);
		ButtonElement buttonElement = new ButtonElement("Spawn", -1, -1, null, ButtonElement.Color.Green);
		gridPixelElement3.Attach(buttonElement, 0, 15);
		gridPixelElement3.Attach(gameGridInventory, 0, 15);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameGridInventory, buttonElement);
	}

	public static (PixelWindow, GameGridInventory, ButtonElement) CreateEventDebuggerWindow()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GridPixelElement gridPixelElement = new GridPixelElement(1, 1, isCentered: true);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered: true);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered: true);
		GameGridInventory gameGridInventory = new GameGridInventory(10, 10);
		gridPixelElement.Attach(gridPixelElement2, 100, 100);
		gridPixelElement2.Attach(gridPixelElement3, 0, 25);
		ButtonElement buttonElement = new ButtonElement("Spawn", -1, -1, null, ButtonElement.Color.Green);
		gridPixelElement3.Attach(buttonElement, 0, 15);
		gridPixelElement3.Attach(gameGridInventory, 0, 15);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameGridInventory, buttonElement);
	}

	public static (PixelWindow, GameInventory, GameInventory) CreateDoubleInventoryWindowSlotInput(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1, isCentered);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory(width, height);
		gridPixelElement.Attach(gameSlotInventory, 0, 0, minWidth);
		gridPixelElement.Attach(gameGridInventory, 1);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameSlotInventory, gameGridInventory);
	}

	public static (PixelWindow, GameInventory, GameInventory) CreateEquipmentWindow(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1, isCentered);
		TagElement element = new TagElement().SetText("Modules");
		TagElement element2 = new TagElement().SetText("Battery");
		GameGridInventory gameGridInventory = new GameGridInventory(8, 2);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered);
		gridPixelElement2.Attach(element, 0, 0, minWidth);
		gridPixelElement2.Attach(gameGridInventory, 0, 1, minWidth);
		gridPixelElement3.Attach(element2, 0, 0, minWidth);
		gridPixelElement3.Attach(gameSlotInventory, 0, 1, minWidth);
		gridPixelElement.Attach(gridPixelElement2, 1);
		gridPixelElement.Attach(gridPixelElement3, 1, 0, minWidth);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameGridInventory, gameSlotInventory);
	}

	public static (PixelWindow, GameInventory) CreateModuleInv(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(1, 1, isCentered);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered);
		TagElement element = new TagElement().SetText("Modules");
		GameGridInventory gameGridInventory = new GameGridInventory(8, 2)
		{
			slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2
		};
		gridPixelElement2.Attach(element, 0, 0, minWidth);
		gridPixelElement2.Attach(gameGridInventory, 0, 1, minWidth);
		gridPixelElement.Attach(gridPixelElement2, 1);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameGridInventory);
	}

	public static (PixelWindow, GameInventory, GameInventory) CreateNavSystemWindow(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1, isCentered);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered);
		TagElement element = new TagElement().SetText("Modules");
		GameGridInventory gameGridInventory = new GameGridInventory(8, 2)
		{
			slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2
		};
		gridPixelElement2.Attach(element, 0, 0, minWidth);
		gridPixelElement2.Attach(gameGridInventory, 0, 1, minWidth);
		gridPixelElement.Attach(gridPixelElement2, 1);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered);
		TagElement element2 = new TagElement().SetText("Nav Disks");
		GameGridInventory gameGridInventory2 = new GameGridInventory(8, 2)
		{
			slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2
		};
		gridPixelElement3.Attach(element2, 0, 0, minWidth);
		gridPixelElement3.Attach(gameGridInventory2, 0, 1, minWidth);
		gridPixelElement.Attach(gridPixelElement3, 2);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameGridInventory, gameGridInventory2);
	}

	public static (PixelWindow, GameInventory, GameInventory) CreatePropulsionSystemWindow(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(3, 1, isCentered);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered);
		TagElement element = new TagElement().SetText("Modules");
		TagElement element2 = new TagElement().SetText("Fuel Intake");
		GameGridInventory gameGridInventory = new GameGridInventory(8, 2);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gridPixelElement2.Attach(element, 0, 0, minWidth);
		gridPixelElement2.Attach(gameGridInventory, 0, 1, minWidth);
		gridPixelElement3.Attach(element2, 0, 0, minWidth);
		gridPixelElement3.Attach(gameSlotInventory, 0, 1, minWidth);
		gridPixelElement.Attach(gridPixelElement2, 1);
		gridPixelElement.Attach(gridPixelElement3, 1, 0, minWidth);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameGridInventory, gameSlotInventory);
	}

	public static (PixelWindow, GameInventory, GameInventory) CreateAutonomousMedicalSystemWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1, isCentered);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered);
		TagElement element = new TagElement().SetText("Modules");
		TagElement element2 = new TagElement().SetText("Patient");
		GameGridInventory gameGridInventory = new GameGridInventory(8, 2);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		gameSlotInventory = new GameSlotInventory("Items/characteratlas", "character_outline")
		{
			mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item is GameCharacterItem
		};
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gridPixelElement2.Attach(element, 0, 0, minWidth);
		gridPixelElement2.Attach(gameGridInventory, 0, 1, minWidth);
		gridPixelElement3.Attach(element2, 0, 0, minWidth);
		gridPixelElement3.Attach(gameSlotInventory, 0, 1, minWidth);
		gridPixelElement.Attach(gridPixelElement2, 1);
		gridPixelElement.Attach(gridPixelElement3, 1, 0, minWidth);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameGridInventory, gameSlotInventory);
	}

	public static (PixelWindow, GameInventory, GameInventory) CreateArmorConditionerSystemWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1, isCentered);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered);
		TagElement element = new TagElement().SetText("Modules");
		TagElement element2 = new TagElement().SetText("Armor");
		GameGridInventory gameGridInventory = new GameGridInventory(8, 2);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		gameSlotInventory = new GameSlotInventory();
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gridPixelElement2.Attach(element, 0, 0, minWidth);
		gridPixelElement2.Attach(gameGridInventory, 0, 1, minWidth);
		gridPixelElement3.Attach(element2, 0, 0, minWidth);
		gridPixelElement3.Attach(gameSlotInventory, 0, 1, minWidth);
		gridPixelElement.Attach(gridPixelElement2, 1);
		gridPixelElement.Attach(gridPixelElement3, 1, 0, minWidth);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameGridInventory, gameSlotInventory);
	}

	public static (PixelWindow, GameInventory, GameInventory, GameInventory) CreateBioPrinterSystemWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(3, 1, isCentered);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered);
		GridPixelElement gridPixelElement4 = new GridPixelElement(1, 2, isCentered);
		TagElement element = new TagElement().SetText("Modules");
		TagElement element2 = new TagElement().SetText("Target Specimen");
		TagElement element3 = new TagElement().SetText("Output");
		GameGridInventory gameGridInventory = new GameGridInventory(8, 2);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory2 = new GameGridInventory(6, 6);
		gameSlotInventory = new GameSlotInventory();
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gridPixelElement2.Attach(element, 0, 0, minWidth);
		gridPixelElement2.Attach(gameGridInventory, 0, 1, minWidth);
		gridPixelElement3.Attach(element2, 0, 0, minWidth);
		gridPixelElement3.Attach(gameSlotInventory, 0, 1, minWidth);
		gridPixelElement4.Attach(element3, 0, 0, minWidth);
		gridPixelElement4.Attach(gameGridInventory2, 0, 1, minWidth);
		gridPixelElement.Attach(gridPixelElement2, 0, 0);
		gridPixelElement.Attach(gridPixelElement3, 1, 0, minWidth);
		gridPixelElement.Attach(gridPixelElement4, 2, 0, minWidth);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameGridInventory, gameSlotInventory, gameGridInventory2);
	}

	public static (PixelWindow, GameInventory, GameInventory, GameInventory) CreateWaterRecyclerSystemWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(3, 1, isCentered);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered);
		GridPixelElement gridPixelElement4 = new GridPixelElement(1, 2, isCentered);
		TagElement element = new TagElement().SetText("Modules");
		TagElement element2 = new TagElement().SetText("Internal Tank");
		TagElement element3 = new TagElement().SetText("Bottling");
		GameGridInventory gameGridInventory = new GameGridInventory(8, 2);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gridPixelElement2.Attach(element, 0, 0, minWidth);
		gridPixelElement2.Attach(gameGridInventory, 0, 1, minWidth);
		gridPixelElement3.Attach(element2, 0, 0, minWidth);
		gridPixelElement3.Attach(gameSlotInventory, 0, 1, minWidth);
		gridPixelElement4.Attach(element3, 0, 0, minWidth);
		gridPixelElement4.Attach(gameSlotInventory2, 0, 1, minWidth);
		gridPixelElement.Attach(gridPixelElement2, 1);
		gridPixelElement.Attach(gridPixelElement3, 1, 0, minWidth);
		gridPixelElement.Attach(gridPixelElement4, 2);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameGridInventory, gameSlotInventory, gameSlotInventory2);
	}

	public static (PixelWindow, GameInventory, GameInventory, GameInventory) CreateHydroponicSystemWindow(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(3, 1, isCentered);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered);
		GridPixelElement gridPixelElement4 = new GridPixelElement(1, 2, isCentered);
		TagElement element = new TagElement().SetText("Modules");
		TagElement element2 = new TagElement().SetText("Seed Intake");
		TagElement element3 = new TagElement().SetText("Output");
		GameGridInventory gameGridInventory = new GameGridInventory(8, 2);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory2 = new GameGridInventory(width, height);
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gridPixelElement2.Attach(element, 0, 0, minWidth);
		gridPixelElement2.Attach(gameGridInventory, 0, 1, minWidth);
		gridPixelElement3.Attach(element2, 0, 0, minWidth);
		gridPixelElement3.Attach(gameSlotInventory, 0, 1, minWidth);
		gridPixelElement4.Attach(element3, 0, 0, minWidth);
		gridPixelElement4.Attach(gameGridInventory2, 0, 1, minWidth);
		gridPixelElement.Attach(gridPixelElement2, 1);
		gridPixelElement.Attach(gridPixelElement3, 1, 0, minWidth);
		gridPixelElement.Attach(gridPixelElement4, 2);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameGridInventory, gameSlotInventory, gameGridInventory2);
	}

	public static (PixelWindow, GameInventory, GameInventory) CreateModularBackpackInv(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1, isCentered);
		GameSlotInventory gameSlotInventory = new GameSlotInventory(1, 1, stealUI: true, isUIDown: true);
		GameGridInventory gameGridInventory = new GameGridInventory(width, height);
		gridPixelElement.Attach(gameSlotInventory, 0, 0, minWidth);
		gridPixelElement.Attach(gameGridInventory, 1);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameSlotInventory, gameGridInventory);
	}

	public static (PixelWindow, GameInventory, GameInventory, GameInventory) CreateMediumModularBackpackInv(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(3, 1, isCentered);
		GameSlotInventory gameSlotInventory = new GameSlotInventory(1, 1, stealUI: true, isUIDown: true);
		GameGridInventory gameGridInventory = new GameGridInventory(width, height);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory(1, 1, stealUI: true, isUIDown: true);
		gridPixelElement.Attach(gameSlotInventory, 0, 0, minWidth);
		gridPixelElement.Attach(gameGridInventory, 1);
		gridPixelElement.Attach(gameSlotInventory2, 2, 0, minWidth);
		pixelWindow.Attach(gridPixelElement);
		return (pixelWindow, gameSlotInventory, gameGridInventory, gameSlotInventory2);
	}

	public static PixelWindow CreateInjectorPouchInv(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 0)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(3, 1, isCentered);
		GameSlotInventory gameSlotInventory = new GameSlotInventory(1, 1);
		ContainerHelper.AllowOnlyTaggedItem(gameSlotInventory, "injector");
		gameSlotInventory.SetBackgroundFadeSprite("Items/items_medical", "injector_background");
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory(1, 1);
		ContainerHelper.AllowOnlyTaggedItem(gameSlotInventory2, "injector");
		gameSlotInventory2.SetBackgroundFadeSprite("Items/items_medical", "injector_background");
		GameSlotInventory gameSlotInventory3 = new GameSlotInventory(1, 1);
		ContainerHelper.AllowOnlyTaggedItem(gameSlotInventory3, "injector");
		gameSlotInventory3.SetBackgroundFadeSprite("Items/items_medical", "injector_background");
		gridPixelElement.Attach(gameSlotInventory, 0, 0, minWidth);
		gridPixelElement.Attach(gameSlotInventory2, 1, 0, minWidth);
		gridPixelElement.Attach(gameSlotInventory3, 2, 0, minWidth);
		pixelWindow.Attach(gridPixelElement);
		return pixelWindow;
	}

	public static PixelWindow CreateIntelWindow(string title, string author, string content, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(1, 3, isCentered);
		TagElement element = new TagElement().SetText(title);
		TagElement element2 = new TagElement().SetText("By " + author);
		RichTextBuilder richTextBuilder = new RichTextBuilder(65).Add(content);
		RichTextElement richTextElement = new RichTextElement();
		richTextElement.SetRichText(richTextBuilder.Build());
		gridPixelElement.Attach(element, 0, 0);
		gridPixelElement.Attach(element2, 0, 1);
		gridPixelElement.Attach(richTextElement, 0, 2, minWidth);
		pixelWindow.Attach(gridPixelElement);
		return pixelWindow;
	}

	public static (PixelWindow, GameInventory) CreateSlotInventoryWindow(int width, int height, bool isDraggable = true)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable, null, WindowStyle.Machinery);
		GameSlotInventory gameSlotInventory = new GameSlotInventory(width, height, stealUI: true).SetMachineryStyle();
		pixelWindow.Attach(gameSlotInventory);
		return (pixelWindow, gameSlotInventory);
	}

	public static (PixelWindow, GameSlotInventory) CreateSlotInventoryWindow(bool isDraggable = true)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable, null, WindowStyle.Machinery);
		GameSlotInventory gameSlotInventory = new GameSlotInventory().SetMachineryStyle();
		pixelWindow.Attach(gameSlotInventory);
		return (pixelWindow, gameSlotInventory);
	}

	public static (PixelWindow, GameInventory) CreateInventoryWindow(string data, int width, bool isDraggable = true)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable, null, WindowStyle.Machinery);
		GameGridInventory gameGridInventory = new GameGridInventory(data, width).SetMachineryStyle();
		pixelWindow.Attach(gameGridInventory);
		return (pixelWindow, gameGridInventory);
	}

	public static PixelWindow CreatePassengerCabinWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		PixelWindow pixelWindow = new PixelWindow(isDraggable);
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1, isCentered);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered);
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered);
		gridPixelElement2.Attach(CreatePassengerSlot(), 0, 0, minWidth);
		gridPixelElement2.Attach(CreatePassengerSlot(), 0, 1, minWidth);
		gridPixelElement3.Attach(CreatePassengerSlot(), 0, 0, minWidth);
		gridPixelElement3.Attach(CreatePassengerSlot(), 0, 1, minWidth);
		gridPixelElement.Attach(gridPixelElement2, 1);
		gridPixelElement.Attach(gridPixelElement3, 1, 0, minWidth);
		pixelWindow.Attach(gridPixelElement);
		return pixelWindow;
	}
}

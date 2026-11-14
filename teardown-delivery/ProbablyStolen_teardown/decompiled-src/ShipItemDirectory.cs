public class ShipItemDirectory : ItemDirectory
{
	public const string EXOTIC_FUEL_TAG = "EXOTIC_FUEL_TAG";

	protected override void InitDirectory()
	{
	}

	private GameItem Ship()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GridPixelElement gridPixelElement = new GridPixelElement(1, 2);
		gridPixelElement.isCentered = true;
		gridPixelElement.justifyWidth = true;
		pixelWindow.Attach(gridPixelElement);
		GridPixelElement gridPixelElement2 = new GridPixelElement(3, 1);
		gridPixelElement2.isCentered = true;
		gridPixelElement2.justifyWidth = true;
		gridPixelElement.Attach(gridPixelElement2, 0, 0);
		GameSlotInventory gameSlotInventory = new GameSlotInventory("Items/characteratlas", "character_outline");
		gameSlotInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item is GameCharacterItem;
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gridPixelElement2.Attach(gameSlotInventory, 0, 0);
		gameSlotInventory = new GameSlotInventory("Items/characteratlas", "character_outline");
		gameSlotInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item is GameCharacterItem;
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 1;
		gridPixelElement2.Attach(gameSlotInventory, 1);
		GameItem gameItem = Empty().SetContentWindow(pixelWindow).SetSpriteAndShape("Items/items_ship", "light_dropship");
		GameGridInventory element = new GameGridInventory(14, 8);
		gridPixelElement.Attach(element, 0, 1);
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		return gameItem;
	}

	private GameItem CargoShip()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GridPixelElement gridPixelElement = new GridPixelElement(1, 3);
		gridPixelElement.isCentered = true;
		gridPixelElement.justifyWidth = true;
		pixelWindow.Attach(gridPixelElement);
		GridPixelElement gridPixelElement2 = new GridPixelElement(4, 1);
		gridPixelElement2.isCentered = true;
		gridPixelElement2.justifyWidth = true;
		gridPixelElement.Attach(gridPixelElement2, 0, 0);
		GameSlotInventory gameSlotInventory = new GameSlotInventory("Items/items_medical", "first_aid_box");
		gameSlotInventory.SetBackgroundFadeSprite("Items/items_medical", "first_aid_box");
		gameSlotInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item.identifier.Equals("firstaid_box");
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gridPixelElement2.Attach(gameSlotInventory, 0, 0);
		gridPixelElement2.Attach(DirectoryUtils.CreatePassengerSlot(countAsSeat: true, isPilotSeat: true), 1);
		gameSlotInventory = ShipHelper.CreateNavigationSlot();
		GameSlotInventory element = ShipHelper.CreateHullSlot();
		GameSlotInventory element2 = ShipHelper.CreatePowerSlot();
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 3);
		gridPixelElement3.isCentered = true;
		gridPixelElement3.justifyWidth = true;
		gridPixelElement3.Attach(gameSlotInventory, 0, 0);
		gridPixelElement3.Attach(element2, 0, 1);
		gridPixelElement3.Attach(element, 0, 2);
		gridPixelElement2.Attach(gridPixelElement3, 2);
		GameItem gameItem = Empty().SetContentWindow(pixelWindow).SetSpriteAndShape("Ships/TransportMedical", "Background");
		new GridPixelElement(1, 2);
		GridPixelElement gridPixelElement4 = new GridPixelElement(2, 1);
		GridPixelElement gridPixelElement5 = new GridPixelElement(1, 2);
		gridPixelElement5.Attach(DirectoryUtils.CreatePassengerSlot(), 0, 0);
		gridPixelElement5.Attach(DirectoryUtils.CreatePassengerSlot(), 0, 1);
		gridPixelElement4.Attach(gridPixelElement5, 0, 0);
		GameGridInventory element3 = new GameGridInventory(14, 14);
		gridPixelElement4.Attach(element3, 0, 1);
		gridPixelElement.Attach(gridPixelElement4, 0, 1);
		GridPixelElement gridPixelElement6 = new GridPixelElement(5, 1)
		{
			isCentered = true,
			justifyWidth = true
		};
		gridPixelElement.Attach(gridPixelElement6, 0, 2);
		GameSlotInventory element4 = ShipHelper.CreateServiceSlot();
		gridPixelElement6.Attach(element4, 0, 0);
		element4 = ShipHelper.CreateIndustrialSlot();
		gridPixelElement6.Attach(element4, 1);
		element4 = new GameSlotInventory("Items/items_container", "pouch_general_large");
		element4.SetBackgroundFadeSprite("Items/items_container", "pouch_general_large");
		gridPixelElement6.Attach(element4, 2);
		element4 = ShipHelper.CreatePropulsionSlot();
		gridPixelElement6.Attach(element4, 3);
		element4 = new GameSlotInventory("Items/items_container", "pouch_general_large");
		element4.SetBackgroundFadeSprite("Items/items_container", "pouch_general_large");
		gridPixelElement6.Attach(element4, 4);
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.name = "Sok-27C";
		CommonTooltipHelper.AddUniqueTypeTooltip(gameItem, "Small Cargo Ship");
		gameItem.EnableManufacturer("sokolov");
		gameItem.shortDescription = "This ship is the cargo variant of the Sok-27. A popular ship for young entreprising captain.";
		return gameItem;
	}

	private GameItem MilitaryTransportShip()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GridPixelElement gridPixelElement = new GridPixelElement(1, 3);
		gridPixelElement.isCentered = true;
		gridPixelElement.justifyWidth = true;
		pixelWindow.Attach(gridPixelElement);
		GridPixelElement gridPixelElement2 = new GridPixelElement(4, 1);
		gridPixelElement2.isCentered = true;
		gridPixelElement2.justifyWidth = true;
		gridPixelElement.Attach(gridPixelElement2, 0, 0);
		gridPixelElement2.Attach(DirectoryUtils.CreatePassengerSlot(countAsSeat: true, isPilotSeat: true), 0, 0);
		gridPixelElement2.Attach(DirectoryUtils.CreatePassengerSlot(countAsSeat: true, isPilotSeat: true), 1);
		GameSlotInventory element = ShipHelper.CreateNavigationSlot();
		GameSlotInventory element2 = ShipHelper.CreateHullSlot();
		GameSlotInventory element3 = ShipHelper.CreatePowerSlot();
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 3);
		gridPixelElement3.isCentered = true;
		gridPixelElement3.justifyWidth = true;
		gridPixelElement3.Attach(element, 0, 0);
		gridPixelElement3.Attach(element3, 0, 1);
		gridPixelElement3.Attach(element2, 0, 2);
		gridPixelElement2.Attach(gridPixelElement3, 2);
		GameItem gameItem = Empty().SetContentWindow(pixelWindow).SetSpriteAndShape("Ships/military_transport_ship", "Background");
		new GridPixelElement(1, 2);
		GridPixelElement gridPixelElement4 = new GridPixelElement(3, 1);
		GridPixelElement gridPixelElement5 = new GridPixelElement(1, 2);
		gridPixelElement5.Attach(DirectoryUtils.CreatePassengerSlot(), 0, 0);
		gridPixelElement5.Attach(DirectoryUtils.CreatePassengerSlot(), 0, 1);
		gridPixelElement4.Attach(gridPixelElement5, 0, 0);
		GameGridInventory element4 = new GameGridInventory(9, 14);
		gridPixelElement4.Attach(element4, 0, 1);
		GridPixelElement gridPixelElement6 = new GridPixelElement(1, 2);
		gridPixelElement6.Attach(DirectoryUtils.CreatePassengerSlot(), 0, 0);
		gridPixelElement6.Attach(DirectoryUtils.CreatePassengerSlot(), 0, 1);
		gridPixelElement4.Attach(gridPixelElement6, 0, 0);
		gridPixelElement.Attach(gridPixelElement4, 0, 1);
		GridPixelElement gridPixelElement7 = new GridPixelElement(5, 1)
		{
			isCentered = true,
			justifyWidth = true
		};
		gridPixelElement.Attach(gridPixelElement7, 0, 2);
		GameSlotInventory element5 = ShipHelper.CreateServiceSlot();
		gridPixelElement7.Attach(element5, 0, 0);
		element5 = ShipHelper.CreateIndustrialSlot();
		gridPixelElement7.Attach(element5, 1);
		element5 = ShipHelper.CreateUtilitySlot();
		gridPixelElement7.Attach(element5, 2);
		element5 = ShipHelper.CreatePropulsionSlot();
		gridPixelElement7.Attach(element5, 3);
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.name = "A-101 Albatross";
		CommonTooltipHelper.AddUniqueTypeTooltip(gameItem, "Small Military Transport");
		gameItem.EnableManufacturer("republic_navy");
		gameItem.shortDescription = "An armored and armed troop transport used by the Republic Navy. Often loaded with marines and launched from larger carrier ship to conduct anti-piracy operation.";
		return gameItem;
	}

	private GameItem UtilityShip()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GridPixelElement gridPixelElement = new GridPixelElement(1, 3);
		gridPixelElement.isCentered = true;
		gridPixelElement.justifyWidth = true;
		pixelWindow.Attach(gridPixelElement);
		GridPixelElement gridPixelElement2 = new GridPixelElement(5, 1);
		gridPixelElement2.isCentered = true;
		gridPixelElement2.justifyWidth = true;
		gridPixelElement.Attach(gridPixelElement2, 0, 0);
		GameSlotInventory element = ShipHelper.CreateExternalSlot();
		gridPixelElement2.Attach(element, 0, 0);
		gridPixelElement2.Attach(DirectoryUtils.CreatePassengerSlot(countAsSeat: true, isPilotSeat: true), 1);
		GameSlotInventory element2 = ShipHelper.CreateNavigationSlot();
		GameSlotInventory element3 = ShipHelper.CreateHullSlot();
		GameSlotInventory element4 = ShipHelper.CreatePowerSlot();
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 3);
		gridPixelElement3.isCentered = true;
		gridPixelElement3.justifyWidth = true;
		gridPixelElement3.Attach(element2, 0, 0);
		gridPixelElement3.Attach(element4, 0, 1);
		gridPixelElement3.Attach(element3, 0, 2);
		gridPixelElement2.Attach(gridPixelElement3, 2);
		element = ShipHelper.CreateExternalSlot();
		gridPixelElement2.Attach(element, 3);
		GameItem gameItem = Empty().SetContentWindow(pixelWindow).SetSpriteAndShape("Ships/utility_ship", "Background");
		new GridPixelElement(1, 2);
		GridPixelElement gridPixelElement4 = new GridPixelElement(3, 1);
		GridPixelElement gridPixelElement5 = new GridPixelElement(1, 2);
		gridPixelElement5.Attach(DirectoryUtils.CreatePassengerSlot(), 0, 0);
		gridPixelElement5.Attach(DirectoryUtils.CreatePassengerSlot(), 0, 1);
		gridPixelElement4.Attach(gridPixelElement5, 0, 0);
		GameGridInventory element5 = new GameGridInventory(9, 14);
		gridPixelElement4.Attach(element5, 0, 1);
		GridPixelElement gridPixelElement6 = new GridPixelElement(1, 2);
		GameSlotInventory element6 = ShipHelper.CreateServiceSlot();
		gridPixelElement6.Attach(element6, 0, 0);
		element6 = ShipHelper.CreateIndustrialSlot();
		gridPixelElement6.Attach(element6, 0, 1);
		gridPixelElement4.Attach(gridPixelElement6, 0, 0);
		gridPixelElement.Attach(gridPixelElement4, 0, 1);
		GridPixelElement gridPixelElement7 = new GridPixelElement(5, 1)
		{
			isCentered = true,
			justifyWidth = true
		};
		gridPixelElement.Attach(gridPixelElement7, 0, 2);
		element6 = ShipHelper.CreateServiceSlot();
		gridPixelElement7.Attach(element6, 0, 0);
		element6 = ShipHelper.CreateIndustrialSlot();
		gridPixelElement7.Attach(element6, 1);
		element6 = ShipHelper.CreateUtilitySlot();
		gridPixelElement7.Attach(element6, 2);
		element6 = ShipHelper.CreatePropulsionSlot();
		gridPixelElement7.Attach(element6, 3);
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.name = "Type-54 Bian Yuan";
		CommonTooltipHelper.AddUniqueTypeTooltip(gameItem, "Small Utility Ship");
		gameItem.EnableManufacturer("zhonghua");
		gameItem.shortDescription = "An heavy duty utility ship made for mining and salvaging operation.";
		return gameItem;
	}

	private GameItem RefugeeShip()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GridPixelElement gridPixelElement = new GridPixelElement(1, 3);
		gridPixelElement.isCentered = true;
		gridPixelElement.justifyWidth = true;
		pixelWindow.Attach(gridPixelElement);
		GridPixelElement gridPixelElement2 = new GridPixelElement(3, 1);
		gridPixelElement2.isCentered = true;
		gridPixelElement2.justifyWidth = true;
		gridPixelElement.Attach(gridPixelElement2, 0, 0);
		gridPixelElement2.Attach(DirectoryUtils.CreatePassengerSlot(countAsSeat: true, isPilotSeat: true), 0, 0);
		GameSlotInventory element = ShipHelper.CreateNavigationSlot();
		GameSlotInventory element2 = ShipHelper.CreateHullSlot();
		GameSlotInventory element3 = ShipHelper.CreatePowerSlot();
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 3);
		gridPixelElement3.isCentered = true;
		gridPixelElement3.justifyWidth = true;
		gridPixelElement3.Attach(element, 0, 0);
		gridPixelElement3.Attach(element3, 0, 1);
		gridPixelElement3.Attach(element2, 0, 2);
		gridPixelElement2.Attach(gridPixelElement3, 1);
		GameItem gameItem = Empty().SetContentWindow(pixelWindow).SetSpriteAndShape("Ships/refugee_ship", "Background");
		new GridPixelElement(1, 2);
		GridPixelElement gridPixelElement4 = new GridPixelElement(5, 1);
		GridPixelElement gridPixelElement5 = new GridPixelElement(1, 2);
		gridPixelElement5.Attach(DirectoryUtils.CreatePassengerSlot(), 0, 0);
		gridPixelElement5.Attach(DirectoryUtils.CreatePassengerSlot(), 0, 1);
		gridPixelElement4.Attach(gridPixelElement5, 0, 0);
		GameGridInventory element4 = new GameGridInventory(9, 14);
		gridPixelElement4.Attach(element4, 0, 1);
		GridPixelElement gridPixelElement6 = new GridPixelElement(1, 2);
		gridPixelElement6.Attach(DirectoryUtils.CreatePassengerSlot(), 0, 0);
		gridPixelElement6.Attach(DirectoryUtils.CreatePassengerSlot(), 0, 1);
		gridPixelElement4.Attach(gridPixelElement6, 0, 3);
		gridPixelElement.Attach(gridPixelElement4, 0, 1);
		GridPixelElement gridPixelElement7 = new GridPixelElement(5, 1)
		{
			isCentered = true,
			justifyWidth = true
		};
		gridPixelElement.Attach(gridPixelElement7, 0, 2);
		GameSlotInventory element5 = ShipHelper.CreateWildcardSlot();
		gridPixelElement7.Attach(element5, 0, 0);
		element5 = ShipHelper.CreateIndustrialSlot();
		gridPixelElement7.Attach(element5, 1);
		element5 = ShipHelper.CreateUtilitySlot();
		gridPixelElement7.Attach(element5, 2);
		element5 = ShipHelper.CreateUtilitySlot();
		gridPixelElement7.Attach(element5, 3);
		element5 = ShipHelper.CreatePropulsionSlot();
		gridPixelElement7.Attach(element5, 4);
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.name = "The Bus";
		CommonTooltipHelper.AddUniqueTypeTooltip(gameItem, "Makeshift Transport Ship");
		gameItem.EnableManufacturer("spacer");
		gameItem.shortDescription = "A barely spaceworthy ship, weld together from cargo containers, empty fuel tanks and anything else they got their hands on.";
		return gameItem;
	}
}

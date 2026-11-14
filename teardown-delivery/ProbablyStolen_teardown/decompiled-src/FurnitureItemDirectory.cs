public class FurnitureItemDirectory : ItemDirectory
{
	private const string SPRITE_POWERBLOCK = "powerblock";

	private const string SPRITE_POWERBLOCK_ON = "powerblock_on";

	private const string SPRITE_SHEET_TOOL = "Items/items_tool";

	protected override void InitDirectory()
	{
		Add("furnace", () => MachineFurnace.Furnace());
		Add("hydroponic", () => MachineHydroponic.Hydroponic());
		Add("plant_dna_manipulator", () => MachineDnaManipulator.DnaManipulator());
		Add("printer", () => MachinePrinter.Printer());
		Add("deal_maker", () => MachineDealMaker.DealMaker());
		Add("recharger", () => MachineRecharger.Recharger());
		Add("powerblock", () => CreatePowerBlock());
		Add("makeshift_reactor", () => ReactorHelper.CreateReactor());
		Add("security_alarm", () => MachineAlarm.Alarme());
		Add("storage_coordinator", () => MachineStorageCoordinator.StorageCoordinator());
		Add("mirage_projector", () => MachineProjector.Projector());
		Add("moisture_farm", () => MachineMoistureFarm.MoistureFarm());
		Add("water_purifier", () => MachinePurifier.WaterPurifier());
		Add("desequencer", () => MachineDesequencer.Desequencer());
		Add("bottle_printer", () => MachineBottlePrinter.BottlePrinter());
		Add("box_dispenser", () => MachineBoxDispenser.BoxDispenser());
		Add("heating_plate", () => MachineHeatingPlate.HeatingPlate());
		Add("blender", () => MachineBlender.Blender());
		Add("evaporator", () => MachineEvaporator.Evaporator());
		Add("chem_finisher", () => MachineChemFinisher.ChemFinisher());
		Add("cassette_player", () => MachineCassettePlayer.CassettePlayer());
		Add("vending_machine", () => VendingMachineHelper.VendingMachine());
		Add("vending_fountain", () => VendingFountainHelper.VendingFountain());
		Add("vending_machine_core", () => VendingMachineHelper.VendingMachineCore());
		Add("vending_fountain_core", () => VendingFountainHelper.VendingFountainCore());
		Add("test_gizmo", () => CreateTestGizmo());
		Add("wine_rack", () => MachineWineRack.AgeWell());
		Add("fridge", () => MachineFridge.Fridge());
	}

	private GameItem CreateTestGizmo()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GameItem result = Empty().SetName("Test Gizmo").SetContentWindow(pixelWindow).SetSpriteAndShape("Items/items_module", "core");
		GridPixelElement gridPixelElement = new GridPixelElement(3, 3, isCentered: true);
		pixelWindow.Attach(gridPixelElement);
		GridPixelElement gridPixelElement2 = new GridPixelElement(2, 3, isCentered: true);
		gridPixelElement.AttachPos(gridPixelElement2, 0, 2);
		PushButtonElement button0 = new PushButtonElement().SetColor(IndicatorElement.IndicatorColor.Cyan);
		gridPixelElement.Attach(button0, 100, 50);
		IndicatorElement ind0 = new IndicatorElement().SetColor(IndicatorElement.IndicatorColor.Red).Light();
		gridPixelElement.Attach(ind0, 30);
		IndicatorElement ind1 = new IndicatorElement();
		gridPixelElement.Attach(ind1, 30);
		button0.onDown = delegate
		{
			button0.ToggleLight();
			ind0.ToggleLight();
			ind1.ToggleLight();
		};
		PushButtonElement buttonSelect0 = new PushButtonElement().SetToggle(isToggle: true).SetColor(IndicatorElement.IndicatorColor.Red);
		PushButtonElement buttonSelect1 = new PushButtonElement().SetToggle(isToggle: true).SetColor(IndicatorElement.IndicatorColor.Yellow);
		PushButtonElement buttonSelect2 = new PushButtonElement().SetToggle(isToggle: true).SetColor(IndicatorElement.IndicatorColor.Green);
		buttonSelect0.onDown = delegate
		{
			buttonSelect0.Lock().Light();
			buttonSelect1.Reset().Unlock().Dark();
			buttonSelect2.Reset().Unlock().Dark();
		};
		buttonSelect1.onDown = delegate
		{
			buttonSelect0.Reset().Unlock().Dark();
			buttonSelect1.Lock().Light();
			buttonSelect2.Reset().Unlock().Dark();
		};
		buttonSelect2.onDown = delegate
		{
			buttonSelect0.Reset().Unlock().Dark();
			buttonSelect1.Reset().Unlock().Dark();
			buttonSelect2.Lock().Light();
		};
		buttonSelect0.Activate();
		TagElement element = new TagElement().SetText("Choice ( 10g)");
		TagElement element2 = new TagElement().SetText("Choice (200g)");
		TagElement element3 = new TagElement().SetText("Choice (360g)");
		gridPixelElement2.Attach(buttonSelect0);
		gridPixelElement2.Attach(element);
		gridPixelElement2.Attach(buttonSelect1);
		gridPixelElement2.Attach(element2);
		gridPixelElement2.Attach(buttonSelect2);
		gridPixelElement2.Attach(element3);
		return result;
	}

	private GameItem CreatePowerBlock()
	{
		GameItem item = Empty().SetName(LocHelper.GetLocalizedItem("item_powerblock_name")).SetSpriteAndShape("Items/items_tool", "powerblock");
		item.EnableTag("IMPORTANT_TAG");
		item.flavorText = LocHelper.GetLocalizedItem("item_powerblock_flavor");
		item.shortDescription = LocHelper.GetLocalizedItem("item_powerblock_desc");
		item.unitValue = 200L;
		AudioHelper.InitSmallMachine(item);
		item.SetGameItemType("MACHINE");
		item.onCycleEndEarlySlotItemFunc = delegate(GameItem gameItem, GameInventory _, SlotMarker _)
		{
			if (!PlayerStore.Instance.isPowerOn)
			{
				item.SetSprite("Items/items_tool", "powerblock");
				return;
			}
			item.SetSprite("Items/items_tool", "powerblock_on");
			int amount = 2;
			if (NetworkUpgrade.IsCommercialPowerUnlocked())
			{
				amount = 4;
			}
			foreach (GameItem item2 in gameItem.FindAdjacentType<GameItem>())
			{
				if (item2.IsTag("power_source_item"))
				{
					PowerHelper.ChargeBattery(item2, amount);
				}
				else if (item2.IsTag("MACHINE_RECHARGABLE_TAG"))
				{
					MachineHelper.RechargeMachineBattery(item2, amount);
				}
			}
		};
		return item;
	}
}

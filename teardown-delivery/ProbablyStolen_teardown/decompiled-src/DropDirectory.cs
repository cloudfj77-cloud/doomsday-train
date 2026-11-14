using System.Collections.Generic;

public class DropDirectory
{
	public enum DropTableSize
	{
		DESTITUTE = 1,
		POOR,
		MEDIOCRE,
		ADEQUAT,
		RICH,
		OPULENCE
	}

	private static DropDirectory instance = null;

	private static readonly object padlock = new object();

	public Drops LabDrops = new Drops();

	private DropTable LabDropTableProba = new DropTable();

	private DropTableItem cash = new DropTableItem();

	private DropTableItem injector = new DropTableItem();

	public static DropDirectory Instance
	{
		get
		{
			lock (padlock)
			{
				if (instance == null)
				{
					instance = new DropDirectory();
				}
				return instance;
			}
		}
	}

	public DropDirectory()
	{
		cash.dropFunction = (int num) => DirectoryMaster.Item("coin").SetStack(num);
		cash.weight = 5;
		injector.dropFunction = (int num) => DirectoryMaster.Item("red_injector").SetStack(num);
		injector.weight = 5;
		LabDrops.probaTable.Add(LabDropTableProba);
		LabDropTableProba.tableCount = 5;
		LabDropTableProba.dropTableItems.Add(cash);
		LabDropTableProba.dropTableItems.Add(injector);
	}

	public static Drops GetInfirmaryDrop()
	{
		Drops drops = new Drops();
		DropTable dropTable = new DropTable
		{
			tableCount = 6
		};
		drops.probaTable.Add(dropTable);
		DropTable dropTable2 = new DropTable();
		dropTable2.AddDrop(1, () => DirectoryMaster.Item("water_fountain"));
		drops.alwaysTable.Add(dropTable2);
		dropTable.AddDrop(5, () => DirectoryMaster.Item("red_injector"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("turquoise_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("black_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("white_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("blue_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("blood_red_injector"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("purple_injector"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("large_purple_injector"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("birth_control_pill"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("bicarsole_pill"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("ocutol_pill"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("blood_bag"));
		dropTable.AddDrop(1, () => FullOcutolPillBottle());
		dropTable.AddDrop(1, () => FullBicarsolePillBottle());
		dropTable.AddDrop(5, () => DirectoryMaster.Item("bandage_item"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("hemostatic_bandage_item"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("pill_bottle"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("pouch_injector"));
		dropTable.AddDrop(1, () => FirstAidPouchWithInjector());
		return drops;
	}

	public static Drops GetHallwayDrop()
	{
		Drops result = new Drops();
		DropTable dropTable = new DropTable();
		dropTable.tableCount = 1;
		dropTable.tableProba = 10;
		dropTable.AddDrop(1, () => DirectoryMaster.Item("water_fountain"));
		return result;
	}

	public static Drops GetMedicalStorageRoomDrop()
	{
		Drops drops = new Drops();
		drops.AddAlways(DirectoryMaster.Item("bandage_item"));
		drops.AddAlways(DirectoryMaster.Item("bandage_item"));
		drops.AddAlways(FullBicarsolePillBottle());
		drops.AddAlways(DirectoryMaster.Item("green_injector"));
		drops.probaTable.Add(DropTableDirectory.DropTableMedical(DropTableSize.ADEQUAT));
		return drops;
	}

	public static GameItem FirstAidPouchWithInjector()
	{
		GameItem gameItem = DirectoryMaster.Item("pouch_medical");
		gameItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("green_injector"))?.TryAcceptOnce();
		SlotMarker slotMarker = gameItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("hemostatic_bandage_item"));
		if (slotMarker != null)
		{
			slotMarker.TryAcceptOnce();
			return gameItem;
		}
		return gameItem;
	}

	public static GameItem FullBicarsolePillBottle()
	{
		GameItem gameItem = DirectoryMaster.Item("bicarsole_pill_bottle");
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("bicarsole_pill").SetAmount(7));
		return gameItem;
	}

	public static GameItem FullLunchBox()
	{
		GameItem gameItem = DirectoryMaster.Item("lunch_box");
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("soda_red"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("cat_bar"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("processed_juice").SetAmount(1));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("li_eat_snackbar"));
		return gameItem;
	}

	public static GameItem FullOcutolPillBottle()
	{
		GameItem gameItem = DirectoryMaster.Item("ocutol_pill_bottle");
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("ocutol_pill").SetAmount(7));
		return gameItem;
	}

	public static GameItem FullFixalinPillBottle()
	{
		GameItem gameItem = DirectoryMaster.Item("fixalin_pill_bottle");
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("fixalin_pill").SetAmount(7));
		return gameItem;
	}

	public static GameItem FullBearPawBox()
	{
		GameItem gameItem = DirectoryMaster.Item("bear_paw_box");
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("bear_paw_cookie").SetAmount(8));
		return gameItem;
	}

	public static GameItem FullFirstAidBox()
	{
		GameItem gameItem = DirectoryMaster.Item("firstaid_box");
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("hemostatic_bandage_item").SetAmount(2));
		GraphUtils.TryAcceptAll(gameItem, FullBicarsolePillBottle());
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("green_injector").SetAmount(1));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("black_injector").SetAmount(1));
		return gameItem;
	}

	public static GameItem FullIndustrialFridge()
	{
		GameItem gameItem = DirectoryMaster.Item("industrial_fridge");
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("small_raw_meat").SetAmount(3));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("fat_meat").SetAmount(2));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("raw_meat").SetAmount(1));
		return gameItem;
	}

	public static Drops GetKitchenDrop()
	{
		Drops drops = new Drops();
		DropTable dropTable = new DropTable
		{
			tableCount = 5
		};
		drops.probaTable.Add(dropTable);
		dropTable.AddDrop(1, () => DirectoryMaster.Item("kitchen_knife"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("kitchen_cleaver"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("pipe_weapon"));
		drops.probaTable.Add(DropTableDirectory.DropTableFood(DropTableSize.ADEQUAT));
		return drops;
	}

	public static Drops GetPantryDrop()
	{
		return new Drops
		{
			probaTable = { DropTableDirectory.DropTableFood(DropTableSize.ADEQUAT) }
		};
	}

	public static Drops GetHydroponicDrop()
	{
		return new Drops
		{
			probaTable = { DropTableDirectory.DropTableHydroponicItem(DropTableSize.ADEQUAT) }
		};
	}

	public static Drops GetCanningRoomDrop()
	{
		Drops drops = new Drops();
		drops.AddAlways(DirectoryMaster.Item("steel_sheet").SetAmount(3));
		drops.AddAlways(DirectoryMaster.Item("automatic_meat_processor"));
		drops.AddAlways(DirectoryMaster.Item("processed_meat").SetAmount(5));
		drops.AddAlways(FullIndustrialFridge());
		DropTable dropTable = new DropTable
		{
			tableCount = 2
		};
		drops.probaTable.Add(dropTable);
		dropTable.AddDrop(5, () => DirectoryMaster.Item("nuts_metal").SetAmount(3));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("steel_sheet").SetAmount(3));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("energy_credit"));
		return drops;
	}

	public static Drops GetCafeteriaDrop()
	{
		Drops obj = new Drops
		{
			probaTable = { DropTableDirectory.DropTableFood(DropTableSize.ADEQUAT) }
		};
		DropTable dropTable = new DropTable();
		dropTable.AddDrop(1, () => DirectoryMaster.Item("water_fountain"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("nutrition_paste_dispencer"));
		obj.alwaysTable.Add(dropTable);
		return obj;
	}

	public static Drops GetWorkshopDrop()
	{
		Drops drops = new Drops();
		DropTable dropTable = new DropTable
		{
			tableCount = 4
		};
		drops.probaTable.Add(dropTable);
		dropTable.AddDrop(5, () => DirectoryMaster.Item("nuts_metal").SetAmount(3));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("wrench_orange").SetAmount(1));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("steel_sheet").SetAmount(3));
		return drops;
	}

	public static Drops GetSupplyDepotDrop()
	{
		Drops drops = new Drops();
		DropTable dropTable = new DropTable
		{
			tableCount = 5
		};
		drops.probaTable.Add(dropTable);
		dropTable.AddDrop(5, () => DirectoryMaster.Item("nuts_metal").SetAmount(3));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("wrench_orange").SetAmount(1));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("steel_sheet").SetAmount(3));
		return drops;
	}

	public static Drops GetBarackDrop()
	{
		Drops drops = new Drops();
		new DropTable();
		drops.probaTable.Add(DropTableDirectory.DropTableAmenities(DropTableSize.MEDIOCRE));
		drops.probaTable.Add(DropTableDirectory.DropTableMilitary(DropTableSize.DESTITUTE));
		drops.probaTable.Add(DropTableDirectory.DropTableEquipment(DropTableSize.MEDIOCRE));
		return drops;
	}

	public static Drops GetCrewQuarterDrop()
	{
		Drops drops = new Drops();
		new DropTable();
		drops.probaTable.Add(DropTableDirectory.DropTableAmenities(DropTableSize.MEDIOCRE));
		drops.probaTable.Add(DropTableDirectory.DropTableEquipment(DropTableSize.MEDIOCRE));
		drops.probaTable.Add(DropTableDirectory.DropTableBasicMedical(DropTableSize.DESTITUTE));
		drops.probaTable.Add(DropTableDirectory.DropTableFood(DropTableSize.DESTITUTE));
		drops.probaTable.Add(DropTableFournitureDirectory.FournitureDropTableLivingQuarter(DropTableSize.DESTITUTE));
		return drops;
	}

	public static Drops GetSecurityCheckpointDrop()
	{
		return new Drops
		{
			probaTable = 
			{
				DropTableDirectory.DropTableMilitary(DropTableSize.DESTITUTE),
				DropTableDirectory.DropTableMedical(DropTableSize.POOR),
				DropTableDirectory.DropTableFood(DropTableSize.MEDIOCRE),
				DropTableDirectory.DropTableEquipment(DropTableSize.DESTITUTE)
			}
		};
	}

	public static Drops GetArmoryDrop()
	{
		return new Drops
		{
			probaTable = 
			{
				DropTableDirectory.DropTableMilitary(DropTableSize.RICH),
				DropTableDirectory.DropTableMedical(DropTableSize.MEDIOCRE)
			}
		};
	}

	public static Drops GetEngineeringDrop()
	{
		return new Drops
		{
			probaTable = { DropTableDirectory.DropTableEngineering(DropTableSize.MEDIOCRE) }
		};
	}

	public static Drops GetPowerBayDrop()
	{
		Drops drops = new Drops();
		drops.probaTable.Add(DropTableDirectory.DropTableEngineering(DropTableSize.POOR));
		GameItem gameItem = DirectoryMaster.Item("battery_hub");
		foreach (GameItem item in GetBatteryForStationBatteryHub())
		{
			GraphUtils.TryAcceptAll(gameItem, item);
		}
		drops.AddAlways(gameItem);
		drops.AddAlways(DirectoryMaster.Item("power_analyser"));
		return drops;
	}

	public static Drops GetEntranceDrop()
	{
		Drops drops = new Drops();
		drops.AddAlways(FullLunchBox());
		return drops;
	}

	public static Drops GetMedbayDrop()
	{
		Drops obj = new Drops
		{
			probaTable = { DropTableDirectory.DropTableMedical(DropTableSize.MEDIOCRE) }
		};
		DropTable dropTable = new DropTable();
		dropTable.AddDrop(1, () => DirectoryMaster.Item("water_fountain"));
		obj.alwaysTable.Add(dropTable);
		return obj;
	}

	public static Drops GetOperationTheaterDrop()
	{
		return new Drops
		{
			probaTable = { DropTableDirectory.DropTableMedical(DropTableSize.MEDIOCRE) }
		};
	}

	public static Drops GetBridgeDrop()
	{
		return new Drops
		{
			probaTable = 
			{
				DropTableDirectory.DropTableValuable(DropTableSize.DESTITUTE),
				DropTableDirectory.DropTableFood(DropTableSize.DESTITUTE),
				DropTableDirectory.DropTableMilitary(DropTableSize.DESTITUTE)
			}
		};
	}

	public static Drops GetLabMedicalDrop()
	{
		return new Drops
		{
			probaTable = { DropTableDirectory.DropTableMedicalRare(DropTableSize.MEDIOCRE) }
		};
	}

	public static Drops GetLabDefenseDrop()
	{
		return new Drops
		{
			probaTable = { DropTableDirectory.DropTableMilitary(DropTableSize.MEDIOCRE) }
		};
	}

	public static Drops GetLabDrop()
	{
		return new Drops
		{
			probaTable = 
			{
				DropTableDirectory.DropTableHydroponicItem(DropTableSize.POOR),
				DropTableDirectory.DropTableModule(DropTableSize.POOR),
				DropTableDirectory.DropTableValuable(DropTableSize.POOR)
			}
		};
	}

	public static Drops GetFreezerRoomDrop()
	{
		Drops drops = new Drops();
		DropTable dropTable = new DropTable
		{
			tableCount = 4
		};
		drops.probaTable.Add(dropTable);
		dropTable.AddDrop(7, () => DirectoryMaster.Item("small_raw_meat"));
		dropTable.AddDrop(7, () => DirectoryMaster.Item("raw_meat"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("beis_icecream"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("kitchen_cleaver"));
		return drops;
	}

	public static List<GameItem> GetBatteryForStationBatteryHub()
	{
		Drops drops = new Drops();
		DropTable dropTable = new DropTable
		{
			tableCount = RNG.GetRandomInt(1, 4)
		};
		drops.probaTable.Add(dropTable);
		dropTable.AddDrop(49, () => DirectoryMaster.Item("energy_credit"));
		dropTable.AddDrop(49, () => DirectoryMaster.Item("energy_credit_counterfeit"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("energy_credit_breeder"));
		List<GameItem> drops2 = drops.GetDrops();
		foreach (GameItem item in drops2)
		{
			PowerHelper.RandomizedEnergyCredit(item);
		}
		return drops2;
	}

	public static Drops GetRaiderDrop(int tableCount = 1)
	{
		Drops drops = new Drops();
		DropTable dropTable = new DropTable
		{
			tableCount = tableCount
		};
		drops.probaTable.Add(dropTable);
		dropTable.AddDrop(5, () => DirectoryMaster.Item("soda_red"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("bottled_water"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("small_bottled_water"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("bottled_water_premium"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("processed_milk"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("processed_juice"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("pack_rib_sandwich"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("cat_bar"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("bear_paw_cookie"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("nutrifruit"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("cup_noodle"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("box_tampon"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("packet_red_cigarette"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("packet_black_cigarette"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("pack_condom"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("energy_credit"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("armor_lining"));
		dropTable.AddDrop(1, () => FullBearPawBox());
		dropTable.AddDrop(1, () => DirectoryMaster.Item("canteen"));
		return drops;
	}
}

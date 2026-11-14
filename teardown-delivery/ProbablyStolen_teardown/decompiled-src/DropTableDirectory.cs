public class DropTableDirectory
{
	public static DropTable DropTableFood(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			dropTable.tableProba = 50;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 4;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 6;
			break;
		}
		dropTable.AddDrop(3, () => DirectoryMaster.Item("processed_milk"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("processed_juice"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("soda_red"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("large_soda"));
		dropTable.AddDrop(4, () => DirectoryMaster.Item("energy_drink"));
		dropTable.AddDrop(4, () => DirectoryMaster.Item("burger"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("pack_rib_sandwich"));
		dropTable.AddDrop(4, () => DirectoryMaster.Item("processed_meat"));
		dropTable.AddDrop(4, () => DirectoryMaster.Item("bottled_water"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("small_bottled_water"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("bottled_water_premium"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("peanut_butter_homemade"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("peanut_butter_green"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("box_cereal"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("cat_bar"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("nutrifruit_packaged"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("nutrichips"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("galaxy_blend"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("cup_noodle"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("bear_paw_cookie"));
		dropTable.AddDrop(1, () => DropDirectory.FullBearPawBox());
		dropTable.AddDrop(1, () => DirectoryMaster.Item("box_tea_bag"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("bottle_hot_sauce"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("water_tank"));
		return dropTable;
	}

	public static DropTable DropTableMilitary(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			dropTable.tableProba = 50;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 3;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 4;
			break;
		}
		dropTable.AddDrop(30, () => DirectoryMaster.Item("frag_grenade"));
		dropTable.AddDrop(30, () => DirectoryMaster.Item("smoke_grenade"));
		dropTable.AddDrop(30, () => DirectoryMaster.Item("flashbang_grenade"));
		dropTable.AddDrop(20, () => DirectoryMaster.Item("mirage_generator"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_small"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_small_modular"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("satchel"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_medium"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_medium_military"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_medium_modular"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_large"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_large_military"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_large_modular"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("pouch_hydration"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("combat_knife"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("combat_machete"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("glock_receiver"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("mp9_receiver"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("ump_receiver"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("armor_titan9"));
		dropTable.AddDrop(4, () => DirectoryMaster.Item("armor_titan10"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("armor_titan10l"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("armor_titan10h"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("armor_titan10s"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("tac_entry_device"));
		dropTable.AddDrop(25, () => DirectoryMaster.Item("c4"));
		return dropTable;
	}

	public static DropTable DropTableMedical(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 3;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 5;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 7;
			break;
		}
		dropTable.AddDrop(4, () => DirectoryMaster.Item("red_injector"));
		dropTable.AddDrop(4, () => DirectoryMaster.Item("turquoise_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("black_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("white_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("blue_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("blood_red_injector"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("purple_injector"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("large_purple_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("birth_control_pill"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("bicarsole_pill"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("ocutol_pill"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("fixalin_pill"));
		dropTable.AddDrop(4, () => DirectoryMaster.Item("smelling_salt"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("blood_bag"));
		dropTable.AddDrop(2, () => DropDirectory.FullOcutolPillBottle());
		dropTable.AddDrop(2, () => DropDirectory.FullBicarsolePillBottle());
		dropTable.AddDrop(2, () => DropDirectory.FullFixalinPillBottle());
		dropTable.AddDrop(3, () => DirectoryMaster.Item("pill_bottle"));
		dropTable.AddDrop(10, () => DirectoryMaster.Item("bandage_item"));
		dropTable.AddDrop(10, () => DirectoryMaster.Item("hemostatic_bandage_item"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("pouch_injector"));
		dropTable.AddDrop(1, () => DropDirectory.FirstAidPouchWithInjector());
		return dropTable;
	}

	public static DropTable DropTableMedicalRare(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 3;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 4;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 5;
			break;
		}
		dropTable.AddDrop(4, () => DirectoryMaster.Item("red_injector"));
		dropTable.AddDrop(4, () => DirectoryMaster.Item("turquoise_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("black_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("white_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("blue_injector"));
		dropTable.AddDrop(3, () => DirectoryMaster.Item("blood_red_injector"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("purple_injector"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("large_purple_injector"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("blood_bag"));
		dropTable.AddDrop(2, () => DropDirectory.FullOcutolPillBottle());
		dropTable.AddDrop(2, () => DropDirectory.FullBicarsolePillBottle());
		dropTable.AddDrop(2, () => DropDirectory.FullFixalinPillBottle());
		return dropTable;
	}

	public static DropTable DropTableValuable(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			dropTable.tableProba = 50;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 3;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 4;
			break;
		}
		dropTable.AddDrop(100, () => DirectoryMaster.Item("energy_credit"));
		dropTable.AddDrop(80, () => DirectoryMaster.Item("energy_credit_counterfeit"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("energy_credit_breeder"));
		return dropTable;
	}

	public static DropTable DropTableModule(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			dropTable.tableProba = 25;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 1;
			dropTable.tableProba = 50;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 3;
			break;
		}
		dropTable.AddDrop(30, () => DirectoryMaster.Item("equipement_module_performance"));
		dropTable.AddDrop(30, () => DirectoryMaster.Item("equipement_module_efficiancy"));
		dropTable.AddDrop(30, () => DirectoryMaster.Item("equipement_module_overclock"));
		dropTable.AddDrop(30, () => DirectoryMaster.Item("equipement_module_eco"));
		return dropTable;
	}

	public static DropTable DropTableEngineering(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			dropTable.tableProba = 50;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 3;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 4;
			break;
		}
		dropTable.AddDrop(100, () => DirectoryMaster.Item("energy_credit"));
		dropTable.AddDrop(100, () => DirectoryMaster.Item("energy_credit_counterfeit"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("energy_credit_breeder"));
		dropTable.AddDrop(50, () => DirectoryMaster.Item("exotic_fuel").SetUnitCount(RNG.GetRandomInt(3, 13)));
		dropTable.AddDrop(50, () => DirectoryMaster.Item("packet_black_cigarette"));
		dropTable.AddDrop(50, () => DirectoryMaster.Item("packet_red_cigarette"));
		dropTable.AddDrop(30, () => DirectoryMaster.Item("equipement_module_performance"));
		dropTable.AddDrop(30, () => DirectoryMaster.Item("equipement_module_efficiancy"));
		dropTable.AddDrop(30, () => DirectoryMaster.Item("equipement_module_overclock"));
		dropTable.AddDrop(30, () => DirectoryMaster.Item("equipement_module_eco"));
		return dropTable;
	}

	public static DropTable DropTableAmenities(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 3;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 4;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 6;
			break;
		}
		dropTable.AddDrop(5, () => DirectoryMaster.Item("box_tampon"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("packet_red_cigarette"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("packet_black_cigarette"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("pack_condom"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("toothpaste"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("shampoo"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("soap"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("toilet_paper"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("paper_towel"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("canteen"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("lockpicks"));
		dropTable.AddDrop(6, () => DirectoryMaster.Item("bedsheet"));
		return dropTable;
	}

	public static DropTable DropTableEquipment(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			dropTable.tableProba = 25;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 1;
			dropTable.tableProba = 50;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 3;
			break;
		}
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_small"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_small_modular"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("satchel"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_medium"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_medium_military"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("backpack_medium_modular"));
		dropTable.AddDrop(6, () => DirectoryMaster.Item("backpack_large"));
		dropTable.AddDrop(6, () => DirectoryMaster.Item("backpack_large_military"));
		dropTable.AddDrop(6, () => DirectoryMaster.Item("backpack_large_modular"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("pouch_injector"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("pouch_medical"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("pouch_general"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("pouch_general_large"));
		dropTable.AddDrop(12, () => DirectoryMaster.Item("pouch_hydration"));
		return dropTable;
	}

	public static DropTable DropTableBasicMedical(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			dropTable.tableProba = 50;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 3;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 4;
			break;
		}
		dropTable.AddDrop(4, () => DirectoryMaster.Item("green_injector"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("bicarsole_pill"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("bandage_item"));
		dropTable.AddDrop(5, () => DirectoryMaster.Item("hemostatic_bandage_item"));
		dropTable.AddDrop(2, () => DirectoryMaster.Item("smelling_salt"));
		return dropTable;
	}

	public static DropTable DropTableHydroponicItem(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 3;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 4;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 5;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 6;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 7;
			break;
		}
		dropTable.AddDrop(10, () => DirectoryMaster.Item("nutrifruit"));
		dropTable.AddDrop(10, () => DirectoryMaster.Item("nutrifruit_seed"));
		dropTable.AddDrop(10, () => DirectoryMaster.Item("kotton_plant"));
		dropTable.AddDrop(10, () => DirectoryMaster.Item("lifeweed"));
		dropTable.AddDrop(10, () => DirectoryMaster.Item("lifeweed_seed"));
		dropTable.AddDrop(1, () => DirectoryMaster.Item("water_tank"));
		return dropTable;
	}

	public static DropTable DropTableExemple(DropDirectory.DropTableSize size)
	{
		DropTable dropTable = new DropTable();
		switch (size)
		{
		case DropDirectory.DropTableSize.DESTITUTE:
			dropTable.tableCount = 1;
			break;
		case DropDirectory.DropTableSize.POOR:
			dropTable.tableCount = 2;
			break;
		case DropDirectory.DropTableSize.MEDIOCRE:
			dropTable.tableCount = 3;
			break;
		case DropDirectory.DropTableSize.ADEQUAT:
			dropTable.tableCount = 4;
			break;
		case DropDirectory.DropTableSize.RICH:
			dropTable.tableCount = 5;
			break;
		}
		return dropTable;
	}
}

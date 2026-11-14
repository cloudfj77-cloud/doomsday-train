public class Loadout
{
	public static void CreateDeserterMarineLoadout(GameCharacterItem gci)
	{
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("overwatch"));
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("mark_item_ability"));
		string[] list = new string[7] { "kitchen_knife", "combat_knife", "pipe_weapon", "kitchen_cleaver", "baseball_bat", "glock_receiver", "mp9_receiver" };
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list)));
		if (RNG.Roll(75))
		{
			string[] list2 = new string[3] { "smoke_grenade", "flashbang_grenade", "frag_grenade" };
			GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list2)));
		}
		if (RNG.Roll(50))
		{
			GameItem item = DirectoryMaster.Item(CombatAIHelper.GetOneFromList(new string[1] { "armor_titan10" }));
			ArmorHelper.ModifyArmor(item, RNG.GetRandomInt(40, 100), RNG.GetRandomInt(0, 40));
			GraphUtils.TryAcceptAll(gci, item);
		}
		if (RNG.Roll(10))
		{
			string[] list3 = new string[2] { "backpack_small", "satchel" };
			GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list3)));
		}
	}

	public static void CreateDeserterCombatMedicLoadout(GameCharacterItem gci)
	{
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("medical_assist_item_ability"));
		string[] list = new string[7] { "kitchen_knife", "combat_knife", "pipe_weapon", "kitchen_cleaver", "baseball_bat", "glock_receiver", "mp9_receiver" };
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list)));
		string[] list2 = new string[2] { "green_injector", "red_injector" };
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list2)));
		if (RNG.Roll(50))
		{
			GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("turquoise_injector"));
		}
		if (RNG.Roll(90))
		{
			GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("hemostatic_bandage_item"));
		}
	}

	public static void CreateXenoHeraldLoadout(GameCharacterItem gci)
	{
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_consume_item_ability"));
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_mark_item_ability"));
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_scratch_item_ability"));
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_warcry_item_ability"));
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_empower_item_ability"));
	}

	public static void CreateXenoHunterLoadout(GameCharacterItem gci)
	{
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_prey_item_ability"));
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_laceration_item_ability"));
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_knockdown_item_ability"));
	}

	public static void CreateXenoBrewerLoadout(GameCharacterItem gci)
	{
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_irritant_item_ability"));
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_toxin_item_ability"));
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("brace_item_ability"));
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_healing_spray_item_ability"));
	}

	public static void CreateXenoDroneLoadout(GameCharacterItem gci)
	{
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_scratch_item_ability"));
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("xeno_laceration_item_ability"));
	}

	public static void CreatePirateRookieLoadout(GameCharacterItem gci)
	{
		gci.currentThirst = RNG.GetRandomInt(200, 800);
		string[] list = new string[7] { "kitchen_knife", "combat_knife", "pipe_weapon", "kitchen_cleaver", "baseball_bat", "glock_receiver", "mp9_receiver" };
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list)));
		if (RNG.Roll(75))
		{
			string[] list2 = new string[4] { "smoke_grenade", "bandage_item", "hemostatic_bandage_item", "topical_bandage_item" };
			GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list2)));
		}
		if (RNG.Roll(5))
		{
			GameItem item = DirectoryMaster.Item(CombatAIHelper.GetOneFromList(new string[1] { "armor_titan10" }));
			ArmorHelper.ModifyArmor(item, RNG.GetRandomInt(40, 100), RNG.GetRandomInt(0, 40));
			GraphUtils.TryAcceptAll(gci, item);
		}
		else if (RNG.Roll(50))
		{
			string[] list3 = new string[3] { "green_injector", "red_injector", "blue_injector" };
			GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list3)));
		}
		string[] list4 = new string[6] { "processed_meat", "galaxy_blend", "cat_bar", "small_bottled_water", "bear_paw_cookie", "bottled_water" };
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list4)));
	}

	public static void CreatePirateVeteranLoadout(GameCharacterItem gci)
	{
		gci.currentThirst = RNG.GetRandomInt(600, 900);
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("overwatch"));
		string[] list = new string[7] { "kitchen_knife", "combat_knife", "pipe_weapon", "kitchen_cleaver", "baseball_bat", "glock_receiver", "mp9_receiver" };
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list)));
		string[] list2 = new string[4] { "smoke_grenade", "bandage_item", "hemostatic_bandage_item", "topical_bandage_item" };
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list2)));
		if (RNG.Roll(50))
		{
			GameItem item = DirectoryMaster.Item(CombatAIHelper.GetOneFromList(new string[1] { "armor_titan10" }));
			ArmorHelper.ModifyArmor(item, RNG.GetRandomInt(40, 100), RNG.GetRandomInt(0, 40));
			GraphUtils.TryAcceptAll(gci, item);
		}
		else if (RNG.Roll(50))
		{
			string[] list3 = new string[3] { "green_injector", "red_injector", "blue_injector" };
			GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list3)));
		}
		string[] list4 = new string[6] { "processed_meat", "galaxy_blend", "cat_bar", "small_bottled_water", "bear_paw_cookie", "bottled_water" };
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list4)));
	}

	public static void CreatePirateChiefLoadout(GameCharacterItem gci)
	{
		gci.currentThirst = RNG.GetRandomInt(900, 1000);
		string[] list = new string[7] { "kitchen_knife", "combat_knife", "pipe_weapon", "kitchen_cleaver", "baseball_bat", "glock_receiver", "mp9_receiver" };
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list)));
		string[] list2 = new string[2] { "smoke_grenade", "flashbang_grenade" };
		GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list2)));
		GameItem item = DirectoryMaster.Item(CombatAIHelper.GetOneFromList(new string[1] { "armor_titan10" }));
		ArmorHelper.ModifyArmor(item, RNG.GetRandomInt(40, 100), RNG.GetRandomInt(0, 40));
		GraphUtils.TryAcceptAll(gci, item);
		if (RNG.Roll(50))
		{
			string[] list3 = new string[3] { "green_injector", "red_injector", "blue_injector" };
			GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item(CombatAIHelper.GetOneFromList(list3)));
		}
		else
		{
			item = DirectoryMaster.Item(CombatAIHelper.GetOneFromList(new string[1] { "armor_titan10" }));
			ArmorHelper.ModifyArmor(item, RNG.GetRandomInt(40, 100), RNG.GetRandomInt(0, 40));
			GraphUtils.TryAcceptAll(gci, item);
		}
		GameItem gameItem = DirectoryMaster.Item("satchel");
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("hemostatic_bandage_item"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("hemostatic_bandage_item"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("blue_injector"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("turquoise_injector"));
		GraphUtils.TryAcceptAll(gci, gameItem);
	}
}

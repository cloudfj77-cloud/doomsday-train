public class GunsItemDirectory : ItemDirectory
{
	public const string ANTIQUE_FIREARM_TAG = "ANTIQUE_FIREARM_TAG";

	protected override void InitDirectory()
	{
		Add("printed_gun", () => PrintedGun());
		Add("handmade_pistol", () => CreateHandmadePistol());
		Add("heavy_handmade_pistol", () => CreateHeavyHandmadePistol());
		Add("glock_receiver", () => CreateGlockReceiver());
		Add("smg", () => CreateSMG());
		Add("shotgun", () => CreateShotgun());
		Add("revolver", () => CreateRevolver());
		Add("stun_gun", () => CreateStunGun());
		Add("small_pistol_ammo", () => SmallPistolRound());
		Add("small_pistol_ammo_p", () => SmallPistolRoundPenetration());
		Add("small_pistol_ammo_nl", () => SmallPistolRoundNonLethal());
		Add("heavy_pistol_ammo", () => HeavyPistolRound());
		Add("heavy_pistol_ammo_p", () => HeavyPistolRoundPenetration());
		Add("heavy_pistol_ammo_nl", () => HeavyPistolRoundNonLethal());
		Add("lou_etched_round", () => LouOperativeHelper.InitEtchedRound(HeavyPistolRound()));
		Add("tazer", () => Tazer());
		Add("handmade_pistol_frame", () => CreateHandmadePistolFrame());
		Add("heavy_handmade_pistol_frame", () => CreateHeavyHandmadePistolFrame());
	}

	private GameItem CreateHandmadePistolFrame()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_ranged_energy", "handmade_pistol_frame");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", "item_handmade_pistol_frame_name", "Nestfire Compact Frame");
		gameItem.EnableManufacturer("spacer");
		gameItem.SetValue(70L);
		GunFrameHelper.InitFrame(gameItem, "handmade_pistol", 1);
		return gameItem;
	}

	private GameItem CreateHeavyHandmadePistolFrame()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_ranged_energy", "heavy_handmade_pistol_frame");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", "item_heavy_handmade_pistol_frame_name", "Ironcraft 10 Frame");
		gameItem.EnableManufacturer("spacer");
		gameItem.SetValue(100L);
		GunFrameHelper.InitFrame(gameItem, "heavy_handmade_pistol", 2);
		return gameItem;
	}

	private GameItem PrintedGun()
	{
		(PixelWindow, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory) tuple = GunModHelper.CreateGunModWindow();
		PixelWindow item = tuple.Item1;
		GameSlotInventory item2 = tuple.Item2;
		GameSlotInventory item3 = tuple.Item3;
		GameSlotInventory item4 = tuple.Item4;
		GameSlotInventory item5 = tuple.Item5;
		GameSlotInventory item6 = tuple.Item6;
		GameSlotInventory item7 = tuple.Item7;
		GameSlotInventory item8 = tuple.Rest.Item1;
		GameSlotInventory item9 = tuple.Rest.Item2;
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_ranged_energy", "printed_gun").SetContentWindow(item);
		gameItem.name = LocHelper.GetLocalizedItem("item_printed_gun_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printed_gun_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_printed_gun_flavor");
		gameItem.EnableManufacturer("spacer");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("FIREARM");
		gameItem.SetGameItemType("MAKESHIFT");
		gameItem.SetValue(40L);
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		GunHelper.InitGun(gameItem, 15, 5, 15, 55, 40, 15, 22, 100);
		GunModHelper.EnableStockSlot(item2);
		GunModHelper.EnableRailSlot(item3);
		GunModHelper.EnableBarrelSlot(item6);
		GunModHelper.EnableMagazineSlot(item4, "pistol_reload_light");
		GunModHelper.EnableUnderbarrelSlot(item5);
		GunModHelper.EnableSpecialSlot(item7);
		GunModHelper.EnableSpecialSlot(item8);
		GunModHelper.EnableSpecialSlot(item9);
		AudioHelper.InitPistol(gameItem);
		GunHelper.SetEquipmentCondition(gameItem, ItemFeatureList.EquipmentCondition.New);
		return gameItem;
	}

	private GameItem CreateHandmadePistol()
	{
		(PixelWindow, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory) tuple = GunModHelper.CreateGunModWindow();
		PixelWindow item = tuple.Item1;
		GameSlotInventory item2 = tuple.Item2;
		GameSlotInventory item3 = tuple.Item3;
		GameSlotInventory item4 = tuple.Item4;
		GameSlotInventory item5 = tuple.Item5;
		GameSlotInventory item6 = tuple.Item6;
		GameSlotInventory item7 = tuple.Item7;
		GameSlotInventory item8 = tuple.Rest.Item1;
		GameSlotInventory item9 = tuple.Rest.Item2;
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_ranged_energy", "handmade_pistol").SetContentWindow(item);
		gameItem.name = LocHelper.GetLocalizedItem("item_handmade_pistol_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_handmade_pistol_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_handmade_pistol_flavor");
		gameItem.EnableManufacturer("spacer");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("FIREARM");
		gameItem.SetGameItemType("LOWTECH");
		gameItem.SetValue(120L);
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		AudioHelper.InitPistol(gameItem);
		GunHelper.InitGun(gameItem, 20, 15, 30, 65, 50, 30, 22, 100);
		GunModHelper.EnableStockSlot(item2);
		GunModHelper.EnableRailSlot(item3);
		GunModHelper.EnableBarrelSlot(item6);
		GunModHelper.EnableMagazineSlot(item4, "pistol_reload_light");
		GunModHelper.EnableUnderbarrelSlot(item5);
		GunModHelper.EnableSpecialSlot(item7);
		GunModHelper.EnableSpecialSlot(item8);
		GunModHelper.EnableSpecialSlot(item9);
		GunHelper.SetEquipmentCondition(gameItem, ItemFeatureList.EquipmentCondition.Good);
		return gameItem;
	}

	private GameItem CreateHeavyHandmadePistol()
	{
		(PixelWindow, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory) tuple = GunModHelper.CreateGunModWindow();
		PixelWindow item = tuple.Item1;
		GameSlotInventory item2 = tuple.Item2;
		GameSlotInventory item3 = tuple.Item3;
		GameSlotInventory item4 = tuple.Item4;
		GameSlotInventory item5 = tuple.Item5;
		GameSlotInventory item6 = tuple.Item6;
		GameSlotInventory item7 = tuple.Item7;
		GameSlotInventory item8 = tuple.Rest.Item1;
		GameSlotInventory item9 = tuple.Rest.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_weapons_ranged_energy", "heavy_handmade_pistol");
		gameItem.name = LocHelper.GetLocalizedItem("item_heavy_handmade_pistol_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_heavy_handmade_pistol_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_heavy_handmade_pistol_flavor");
		gameItem.EnableManufacturer("spacer");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("FIREARM");
		gameItem.SetGameItemType("LOWTECH");
		AudioHelper.InitGun(gameItem);
		GunHelper.InitGun(gameItem, 30, 30, 34, 30, 40, 40, 10, 80);
		GunModHelper.EnableStockSlot(item2);
		GunModHelper.EnableRailSlot(item3);
		GunModHelper.EnableBarrelSlot(item6);
		GunModHelper.EnableMagazineSlot(item4, "pistol_reload");
		GunModHelper.EnableUnderbarrelSlot(item5);
		GunModHelper.EnableSpecialSlot(item7);
		GunModHelper.EnableSpecialSlot(item8);
		GunModHelper.EnableSpecialSlot(item9);
		gameItem.SetValue(160L);
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		GunHelper.SetEquipmentCondition(gameItem, ItemFeatureList.EquipmentCondition.Good);
		return gameItem;
	}

	private GameItem CreateGlockReceiver()
	{
		(PixelWindow, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory) tuple = GunModHelper.CreateGunModWindow();
		PixelWindow item = tuple.Item1;
		GameSlotInventory item2 = tuple.Item2;
		GameSlotInventory item3 = tuple.Item3;
		GameSlotInventory item4 = tuple.Item4;
		GameSlotInventory item5 = tuple.Item5;
		GameSlotInventory item6 = tuple.Item6;
		GameSlotInventory item7 = tuple.Item7;
		GameSlotInventory item8 = tuple.Rest.Item1;
		GameSlotInventory item9 = tuple.Rest.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_weapons_ranged_energy", "glock_receiver");
		gameItem.name = LocHelper.GetLocalizedItem("item_glock_receiver_name");
		gameItem.EnableManufacturer("meridian");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_glock_receiver_desc");
		gameItem.unitValue = 200L;
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		AudioHelper.InitGun(gameItem);
		GunHelper.InitGun(gameItem, 30, 40, 42, 35, 50, 45, 10, 65);
		GunModHelper.EnableStockSlot(item2);
		GunModHelper.EnableRailSlot(item3);
		GunModHelper.EnableBarrelSlot(item6);
		GunModHelper.EnableMagazineSlot(item4, "pistol_reload");
		GunModHelper.EnableUnderbarrelSlot(item5);
		GunModHelper.EnableSpecialSlot(item7);
		GunModHelper.EnableSpecialSlot(item8);
		GunModHelper.EnableSpecialSlot(item9);
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("FIREARM");
		gameItem.SetGameItemType("MIDLINE");
		GunHelper.SetEquipmentCondition(gameItem, ItemFeatureList.EquipmentCondition.Good);
		return gameItem;
	}

	private GameItem CreateSMG()
	{
		(PixelWindow, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory) tuple = GunModHelper.CreateGunModWindow();
		PixelWindow item = tuple.Item1;
		GameSlotInventory item2 = tuple.Item2;
		GameSlotInventory item3 = tuple.Item3;
		GameSlotInventory item4 = tuple.Item4;
		GameSlotInventory item5 = tuple.Item5;
		GameSlotInventory item6 = tuple.Item6;
		GameSlotInventory item7 = tuple.Item7;
		GameSlotInventory item8 = tuple.Rest.Item1;
		GameSlotInventory item9 = tuple.Rest.Item2;
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_ranged_antique", "heavy_smg").SetContentWindow(item);
		gameItem.name = LocHelper.GetLocalizedItem("item_smg_name");
		gameItem.EnableManufacturer("meridian");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_smg_desc");
		gameItem.unitValue = 280L;
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		AudioHelper.InitHeavyGun(gameItem);
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("FIREARM");
		gameItem.SetGameItemType("MIDLINE");
		GunHelper.InitGun(gameItem, 65, 55, 65, 25, 60, 50, 10, 40);
		GunModHelper.EnableStockSlot(item2);
		GunModHelper.EnableRailSlot(item3);
		GunModHelper.EnableBarrelSlot(item6);
		GunModHelper.EnableMagazineSlot(item4, "smg_reload");
		GunModHelper.EnableUnderbarrelSlot(item5);
		GunModHelper.EnableSpecialSlot(item7);
		GunModHelper.EnableSpecialSlot(item8);
		GunModHelper.EnableSpecialSlot(item9);
		GunHelper.SetEquipmentCondition(gameItem, ItemFeatureList.EquipmentCondition.Good);
		return gameItem;
	}

	private GameItem CreateShotgun()
	{
		(PixelWindow, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory) tuple = GunModHelper.CreateGunModWindow();
		PixelWindow item = tuple.Item1;
		GameSlotInventory item2 = tuple.Item2;
		GameSlotInventory item3 = tuple.Item3;
		GameSlotInventory item4 = tuple.Item4;
		GameSlotInventory item5 = tuple.Item5;
		GameSlotInventory item6 = tuple.Item6;
		GameSlotInventory item7 = tuple.Item7;
		GameSlotInventory item8 = tuple.Rest.Item1;
		GameSlotInventory item9 = tuple.Rest.Item2;
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_ranged_antique", "shotgun").SetContentWindow(item);
		gameItem.name = LocHelper.GetLocalizedItem("item_shotgun_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_shotgun_desc");
		gameItem.EnableManufacturer("meridian");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("FIREARM");
		gameItem.SetGameItemType("MIDLINE");
		gameItem.SetValue(310L);
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		AudioHelper.InitHeavyGun(gameItem);
		GunHelper.InitGun(gameItem, 85, 75, 75, 30, 45, 50, 12, 35);
		GunModHelper.EnableStockSlot(item2);
		GunModHelper.EnableRailSlot(item3);
		GunModHelper.EnableBarrelSlot(item6);
		GunModHelper.EnableMagazineSlot(item4);
		GunModHelper.EnableUnderbarrelSlot(item5);
		GunModHelper.EnableSpecialSlot(item7);
		GunModHelper.EnableSpecialSlot(item8);
		GunModHelper.EnableSpecialSlot(item9);
		AudioHelper.InitUMP(gameItem);
		GunHelper.SetEquipmentCondition(gameItem, ItemFeatureList.EquipmentCondition.Good);
		return gameItem;
	}

	private GameItem CreateStunGun()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_ranged_energy", "stun_gun");
		gameItem.name = LocHelper.GetLocalizedItem("item_stun_gun_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_stun_gun_flavor");
		gameItem.EnableManufacturer("republic_navy");
		AudioHelper.InitGun(gameItem);
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MIDLINE");
		gameItem.SetValue(250L);
		return gameItem;
	}

	private GameItem CreateRevolver()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_ranged_energy", "revolver");
		gameItem.name = LocHelper.GetLocalizedItem("item_revolver_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_revolver_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_revolver_flavor");
		gameItem.EnableManufacturer("frontier");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("FIREARM");
		gameItem.SetGameItemType("MIDLINE");
		AudioHelper.InitGun(gameItem);
		gameItem.SetValue(240L);
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		GunHelper.InitGun(gameItem, 36, 50, 50, 38, 55, 52, 45, 60, moddable: false);
		GunHelper.SetEquipmentCondition(gameItem, ItemFeatureList.EquipmentCondition.Good);
		return gameItem;
	}

	private GameItem SmallPistolRound()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sec", "light_pistol_ammo");
		gameItem.name = LocHelper.GetLocalizedItem("item_small_pistol_ammo_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_small_pistol_ammo_flavor");
		gameItem.EnableManufacturer("spacer");
		gameItem.SetValue(20L);
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		gameItem.SetGameItemType("AMMUNITION");
		gameItem.ModifyTag("GUN_CALIBER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(22);
		});
		gameItem.ModifyTag("AMMO_TYPE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("NORMAL_AMMO");
		});
		return gameItem;
	}

	private GameItem SmallPistolRoundPenetration()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sec", "light_pistol_ammo_p");
		gameItem.name = LocHelper.GetLocalizedItem("item_small_pistol_ammo_p_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_small_pistol_ammo_p_flavor");
		gameItem.EnableManufacturer("spacer");
		gameItem.SetValue(26L);
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		gameItem.SetGameItemType("AMMUNITION");
		gameItem.ModifyTag("GUN_CALIBER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(22);
		});
		gameItem.ModifyTag("AMMO_TYPE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("ARMOR_PIERCING_AMMO");
		});
		return gameItem;
	}

	private GameItem SmallPistolRoundNonLethal()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sec", "light_pistol_ammo_nl");
		gameItem.name = LocHelper.GetLocalizedItem("item_small_pistol_ammo_nl_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_small_pistol_ammo_nl_flavor");
		gameItem.EnableManufacturer("spacer");
		gameItem.SetValue(14L);
		gameItem.SetGameItemType("AMMUNITION");
		gameItem.ModifyTag("GUN_CALIBER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(22);
		});
		gameItem.ModifyTag("AMMO_TYPE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("NON_LETHAL_AMMO");
		});
		return gameItem;
	}

	private GameItem HeavyPistolRound()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sec", "heavy_pistol_ammo");
		gameItem.name = LocHelper.GetLocalizedItem("item_heavy_pistol_ammo_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_heavy_pistol_ammo_flavor");
		gameItem.EnableManufacturer("spacer");
		gameItem.SetValue(30L);
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		gameItem.SetGameItemType("AMMUNITION");
		gameItem.ModifyTag("GUN_CALIBER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(10);
		});
		gameItem.ModifyTag("AMMO_TYPE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("NORMAL_AMMO");
		});
		return gameItem;
	}

	private GameItem HeavyPistolRoundPenetration()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sec", "heavy_pistol_ammo_p");
		gameItem.name = LocHelper.GetLocalizedItem("item_heavy_pistol_ammo_p_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_heavy_pistol_ammo_p_flavor");
		gameItem.EnableManufacturer("spacer");
		gameItem.SetValue(36L);
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		gameItem.SetGameItemType("AMMUNITION");
		gameItem.ModifyTag("GUN_CALIBER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(10);
		});
		gameItem.ModifyTag("AMMO_TYPE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("ARMOR_PIERCING_AMMO");
		});
		return gameItem;
	}

	private GameItem HeavyPistolRoundNonLethal()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sec", "heavy_pistol_ammo_nl");
		gameItem.name = LocHelper.GetLocalizedItem("item_heavy_pistol_ammo_nl_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_heavy_pistol_ammo_nl_flavor");
		gameItem.EnableManufacturer("spacer");
		gameItem.SetValue(24L);
		gameItem.SetGameItemType("AMMUNITION");
		gameItem.ModifyTag("GUN_CALIBER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(10);
		});
		gameItem.ModifyTag("AMMO_TYPE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("NON_LETHAL_AMMO");
		});
		return gameItem;
	}

	private GameItem Tazer()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sec", "tazer");
		gameItem.name = LocHelper.GetLocalizedItem("item_tazer_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_tazer_flavor");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MELEE_WEAPON");
		gameItem.SetValue(100L);
		gameItem.EnableManufacturer("blueshield");
		return gameItem;
	}
}

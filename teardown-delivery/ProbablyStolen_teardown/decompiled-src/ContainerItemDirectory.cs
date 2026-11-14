using System;

public class ContainerItemDirectory : ItemDirectory
{
	protected override void InitDirectory()
	{
		Add("save_bag", () => SaveBag());
		Add("back_inv_save_bag", () => BackInvSaveBag());
		Add("hidden_save_bag", () => HiddenInvSaveBag());
		Add("showcase_save_bag", () => ShowcaseInvSaveBag());
		Add("sold_save_bag", () => SoldInvSaveBag());
		Add("fanny_pack", () => FannyPack());
		Add("satchel", () => Satchel());
		Add("kids_backpack", () => KidsBackpack());
		Add("kids_backpack_large", () => KidsBackpackLarge());
		Add("backpack_small", () => BackpackSmall());
		Add("backpack_medium", () => BackpackMedium());
		Add("backpack_medium_military", () => BackpackMediumMilitary());
		Add("backpack_large", () => BackpackLarge());
		Add("backpack_large_military", () => BackpackLargeMilitary());
		Add("nutrifruit_seed_packet", () => NutrifruitSeedPacket());
		Add("sec_box", () => SecLootBox());
		Add("evidence_box", () => EvidenceLootBox());
		Add("lou_locker", () => LouOperativeHelper.CreateLocker(EvidenceLootBox(10, 7)));
		Add("med_box", () => MedLootBox());
		Add("eng_box", () => EngLootBox());
		Add("service_box", () => ServiceLootBox());
		Add("beerCase", () => BeerCase());
		Add("luxury_supply_crate", () => LuxurySupplyCrate());
	}

	private GameItem LuxurySupplyCrate()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_container", "eleanor_box");
		gameItem.name = LocHelper.GetLocalizedItem("item_luxury_supply_crate_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_luxury_supply_crate_desc");
		gameItem.unitValue = 300L;
		gameItem.SetGameItemType("LUXURY_ITEM");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		StolenHelper.InitStolenItem(gameItem, 100);
		gameItem.EnableTag("IMPORTANT_TAG");
		return gameItem;
	}

	private GameItem EquippableBack()
	{
		GameItem gameItem = Empty();
		gameItem.EnableTag("backpack");
		gameItem.EnableTag("equippable");
		return gameItem;
	}

	private GameItem BackpackSmall()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(6, 6);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack");
		AudioHelper.InitBackpack(gameItem);
		gameItem.unitValue = 300L;
		gameItem.name = LocHelper.GetLocalizedItem("item_backpack_small_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_backpack_small_desc");
		ContainerHelper.InitBackpackItem(item2, gameItem);
		return gameItem;
	}

	private GameItem SaveBag()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(24, 10).Item1;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack");
		gameItem.unitValue = 0L;
		gameItem.name = "Save Bag";
		return gameItem;
	}

	private GameItem BackInvSaveBag()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(18, 4).Item1;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack");
		gameItem.unitValue = 0L;
		gameItem.name = "Back Inv Save Bag";
		return gameItem;
	}

	private GameItem HiddenInvSaveBag()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(2, 2).Item1;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack");
		gameItem.unitValue = 0L;
		gameItem.name = "Hidden Save Bag";
		return gameItem;
	}

	private GameItem ShowcaseInvSaveBag()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(9, 7).Item1;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack");
		gameItem.unitValue = 0L;
		gameItem.name = "Hidden Save Bag";
		return gameItem;
	}

	private GameItem SoldInvSaveBag()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(40, 40).Item1;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack");
		gameItem.unitValue = 0L;
		gameItem.name = "Hidden Save Bag";
		return gameItem;
	}

	private GameItem BackpackMedium()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(6, 9);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack_medium");
		AudioHelper.InitBackpack(gameItem);
		gameItem.unitValue = 450L;
		gameItem.name = LocHelper.GetLocalizedItem("item_backpack_medium_name");
		ContainerHelper.InitBackpackItem(item2, gameItem);
		return gameItem;
	}

	private GameItem BackpackMediumMilitary()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(6, 9);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack_medium_military");
		AudioHelper.InitBackpack(gameItem);
		ArmorHelper.InitArmorItem(gameItem, 200, 5, 50, 0, 0, -3, -2);
		gameItem.unitValue = 130L;
		gameItem.name = LocHelper.GetLocalizedItem("item_backpack_medium_military_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_backpack_medium_military_desc");
		ContainerHelper.InitBackpackItem(item2, gameItem);
		return gameItem;
	}

	private GameItem BackpackLarge()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(8, 9);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack_large");
		AudioHelper.InitBackpack(gameItem);
		gameItem.unitValue = 650L;
		gameItem.name = LocHelper.GetLocalizedItem("item_backpack_large_name");
		ContainerHelper.InitBackpackItem(item2, gameItem);
		return gameItem;
	}

	private GameItem BackpackLargeMilitary()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(7, 11);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack_large_military");
		AudioHelper.InitBackpack(gameItem);
		ArmorHelper.InitArmorItem(gameItem, 200, 10, 50, 0, 0, -4, -3);
		gameItem.unitValue = 280L;
		gameItem.name = LocHelper.GetLocalizedItem("item_backpack_large_military_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_backpack_large_military_desc");
		ContainerHelper.InitBackpackItem(item2, gameItem);
		return gameItem;
	}

	private GameItem FannyPack()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(3, 3);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "fanny_pack");
		gameItem.EnableTag("backpack");
		gameItem.EnableTag("equippable");
		gameItem.isCombatBackpack = true;
		gameItem.unitValue = 50L;
		AudioHelper.InitBackpack(gameItem);
		gameItem.name = LocHelper.GetLocalizedItem("item_fanny_pack_name");
		ContainerHelper.InitBackpackItem(item2, gameItem);
		return gameItem;
	}

	private GameItem Satchel()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(6, 3);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "satchel");
		gameItem.EnableTag("backpack");
		gameItem.EnableTag("equippable");
		gameItem.isCombatBackpack = true;
		gameItem.unitValue = 150L;
		AudioHelper.InitBackpack(gameItem);
		gameItem.name = LocHelper.GetLocalizedItem("item_satchel_name");
		ContainerHelper.InitBackpackItem(item2, gameItem);
		return gameItem;
	}

	private GameItem KidsBackpack()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(5, 5);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "junor_backpack1");
		AudioHelper.InitBackpack(gameItem);
		gameItem.unitValue = 200L;
		gameItem.name = LocHelper.GetLocalizedItem("item_kids_backpack_name");
		ContainerHelper.InitBackpackItem(item2, gameItem);
		return gameItem;
	}

	private GameItem KidsBackpackLarge()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(5, 6);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "junor_backpack2");
		AudioHelper.InitBackpack(gameItem);
		gameItem.unitValue = 250L;
		gameItem.name = LocHelper.GetLocalizedItem("item_kids_backpack_large_name");
		ContainerHelper.InitBackpackItem(item2, gameItem);
		return gameItem;
	}

	private GameItem NutrifruitSeedPacket()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(3, 3);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_hydroponic_plant", "nutrifruit_seed_packet");
		gameItem.name = LocHelper.GetLocalizedItem("item_nutrifruit_seed_packet_name");
		gameItem.EnableManufacturer("pastel");
		gameItem.unitValue = 1L;
		gameItem.SetGameItemType("SEED");
		ContainerHelper.InitContainerItem(item2, gameItem);
		ContainerHelper.AllowOnlyTaggedItem(item2, "SEED", ownedOnly: true, excludeContainer: true);
		gameItem.EnableTag("STORABLE_PACKAGE_TAG");
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			loaded.EnableTag("STORABLE_PACKAGE_TAG");
		});
		return gameItem;
	}

	private GameItem SecLootBox()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(7, 5);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "sec_box");
		gameItem.name = LocHelper.GetLocalizedItem("item_sec_box_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sec_box_desc");
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		gameItem.unitValue = 200L;
		LockHelper.InitLockedContainerItem(gameItem, "LOCK_ID_SEC", item2);
		return gameItem;
	}

	private GameItem EngLootBox()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(7, 5);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "eng_box");
		gameItem.name = LocHelper.GetLocalizedItem("item_eng_box_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_eng_box_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_eng_box_flavor");
		gameItem.unitValue = 200L;
		LockHelper.InitLockedContainerItem(gameItem, "LOCK_ID_ENG", item2);
		return gameItem;
	}

	private GameItem MedLootBox()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(7, 5);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "med_box");
		gameItem.name = LocHelper.GetLocalizedItem("item_med_box_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_med_box_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_med_box_flavor");
		gameItem.unitValue = 200L;
		LockHelper.InitLockedContainerItem(gameItem, "LOCK_ID_MED", item2);
		return gameItem;
	}

	private GameItem EvidenceLootBox(int width = 7, int height = 5)
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(width, height);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "evidence_box");
		gameItem.name = LocHelper.GetLocalizedItem("item_evidence_box_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_evidence_box_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_evidence_box_desc");
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		gameItem.unitValue = 200L;
		LockHelper.InitLockedContainerItem(gameItem, "LOCK_ID_SEC", item2);
		return gameItem;
	}

	private GameItem ServiceLootBox()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(7, 5);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "service_box");
		gameItem.name = LocHelper.GetLocalizedItem("item_service_box_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_service_box_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_service_box_flavor");
		gameItem.unitValue = 175L;
		LockHelper.InitLockedContainerItem(gameItem, "LOCK_ID_SER", item2);
		return gameItem;
	}

	private GameItem BeerCase()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(8, 4);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem box = Empty().SetContentWindow(item).SetSpriteAndShape("Items/beer_case", "beer_case_0");
		box.name = LocHelper.GetLocalizedItem("item_beer_case_name");
		box.EnableManufacturer("spacer");
		box.SetGameItemType("SUBSTANCE");
		box.SetGameItemType("ALCOHOL");
		box.SetGameItemType("BREVAGE");
		box.EnableTag("PRE_CONTAINER_TAG");
		ContainerHelper.AllowOnlyTaggedItem(item2, "RED_IMP_BEER_TAG", ownedOnly: true);
		item2.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item2.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			UpdateBeerCaseSprite(box, parentInventory);
		});
		item2.onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item2.onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			UpdateBeerCaseSprite(box, parentInventory);
		});
		box.unitValue = 1L;
		return box;
	}

	private void UpdateBeerCaseSprite(GameItem gameItem, GameInventory itemInventory)
	{
		gameItem.SetSpriteAndShape("Items/beer_case", "beer_case_" + itemInventory.childItems.Count);
	}
}

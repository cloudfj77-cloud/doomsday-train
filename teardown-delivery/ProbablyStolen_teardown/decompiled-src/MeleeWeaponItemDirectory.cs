public class MeleeWeaponItemDirectory : ItemDirectory
{
	public static string MELEE_WEAPON_TAG = "MELEE_WEAPON_TAG";

	public static string MELEE_SHARP_WEAPON_TAG = "MELEE_SHARP_WEAPON_TAG";

	public static string MELEE_BLUNT_WEAPON_TAG = "MELEE_BLUNT_WEAPON_TAG";

	protected override void InitDirectory()
	{
		Add("kitchen_knife", () => CreateKitchenKnife());
		Add("box_cutter", () => CreateBoxCutter());
		Add("combat_knife", () => CreateCombatKnife());
		Add("kitchen_cleaver", () => CreateKitchenCleaver());
		Add("combat_machete", () => CreateCombatMachete());
		Add("pipe_weapon", () => CreatePipeWeapon());
		Add("stun_baton", () => StunBaton());
		Add("crowbar", () => CreateCrowbarWeapon());
		Add("hatchet", () => CreateHatchetWeapon());
		Add("glass_shard_shiv", () => CreateGlassShiv());
		Add("skinning_knife", () => CreateSkinningKnife());
	}

	private GameItem CreateGlassShiv()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_melee", "glass_shard_shiv");
		gameItem.name = LocHelper.GetLocalizedItem("item_glass_shard_shiv_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_glass_shard_shiv_flavor");
		gameItem.unitValue = 8L;
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MELEE_WEAPON");
		gameItem.SetGameItemType("MAKESHIFT");
		gameItem.SetGameItemType("JUNK");
		HusbandryHelper.InitBladeItem(gameItem);
		return gameItem;
	}

	private GameItem CreateSkinningKnife()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_melee", "skinning_knife");
		gameItem.name = LocHelper.GetLocalizedItem("item_skinning_knife_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_skinning_knife_flavor");
		gameItem.unitValue = 100L;
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MELEE_WEAPON");
		HusbandryHelper.InitBladeItem(gameItem);
		gameItem.EnableTag("BUTCHER_TALLY_TAG");
		return gameItem;
	}

	private GameItem CreateCombatKnife()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_melee", "combat_knife_2");
		gameItem.name = LocHelper.GetLocalizedItem("item_combat_knife_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_combat_knife_flavor");
		gameItem.unitValue = 75L;
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MELEE_WEAPON");
		HusbandryHelper.InitBladeItem(gameItem);
		return gameItem;
	}

	private GameItem CreateBoxCutter()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_melee", "box_cutter");
		gameItem.name = LocHelper.GetLocalizedItem("item_box_cutter_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_box_cutter_flavor");
		gameItem.unitValue = 28L;
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MELEE_WEAPON");
		gameItem.SetGameItemType("TOOL");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		HusbandryHelper.InitBladeItem(gameItem);
		return gameItem;
	}

	private GameItem CreateKitchenCleaver()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_melee", "kitchen_cleaver");
		gameItem.name = LocHelper.GetLocalizedItem("item_kitchen_cleaver_name");
		gameItem.unitValue = 65L;
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MELEE_WEAPON");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		HusbandryHelper.InitBladeItem(gameItem);
		return gameItem;
	}

	private GameItem CreateKitchenKnife()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_melee", "kitchen_knife");
		gameItem.name = LocHelper.GetLocalizedItem("item_kitchen_knife_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_kitchen_knife_flavor");
		gameItem.unitValue = 45L;
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MELEE_WEAPON");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		HusbandryHelper.InitBladeItem(gameItem);
		return gameItem;
	}

	private GameItem CreateCombatMachete()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_melee", "combat_machete");
		gameItem.name = LocHelper.GetLocalizedItem("item_combat_machete_name");
		gameItem.unitValue = 185L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_combat_machete_flavor");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MELEE_WEAPON");
		HusbandryHelper.InitBladeItem(gameItem);
		return gameItem;
	}

	private GameItem CreateCrowbarWeapon()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_melee", "crowbar");
		gameItem.name = LocHelper.GetLocalizedItem("item_crowbar_name");
		gameItem.unitValue = 75L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_crowbar_flavor");
		gameItem.SetGameItemType("TOOL");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MELEE_WEAPON");
		DebrisHelper.InitClearingTool(gameItem);
		gameItem.triggerOverwatch = true;
		return gameItem;
	}

	private GameItem CreateHatchetWeapon()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_melee", "hatchet");
		gameItem.name = LocHelper.GetLocalizedItem("item_hatchet_name");
		gameItem.unitValue = 55L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_hatchet_desc");
		gameItem.SetGameItemType("TOOL");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MELEE_WEAPON");
		HusbandryHelper.InitBladeItem(gameItem);
		HydroponicHelper.InitHatchetPlantRemoval(gameItem);
		gameItem.triggerOverwatch = true;
		return gameItem;
	}

	private GameItem CreatePipeWeapon()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_melee", "pipe");
		gameItem.name = LocHelper.GetLocalizedItem("item_pipe_weapon_name");
		gameItem.unitValue = 15L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_pipe_weapon_desc");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MELEE_WEAPON");
		gameItem.SetGameItemType("MAKESHIFT");
		gameItem.SetGameItemType("JUNK");
		gameItem.triggerOverwatch = true;
		return gameItem;
	}

	private GameItem StunBaton()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_melee", "stun_baton");
		gameItem.unitValue = 165L;
		gameItem.name = LocHelper.GetLocalizedItem("item_stun_baton_name");
		gameItem.SetGameItemType("MELEE_WEAPON");
		gameItem.SetGameItemType("WEAPON");
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_stun_baton_desc");
		return gameItem;
	}
}

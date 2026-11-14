public class ArmorItemDirectory : ItemDirectory
{
	protected override void InitDirectory()
	{
		Add("armor_lining", () => ArmorLining());
	}

	private GameItem ArmorLining()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_armor", "armor_lining");
		gameItem.name = LocHelper.GetLocalizedItem("item_armor_lining_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_armor_lining_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_armor_lining_flavor");
		gameItem.EnableManufacturer("pastel");
		ArmorHelper.InitArmorItem(gameItem, 100, 4, 0, 0);
		AudioHelper.InitArmour(gameItem);
		gameItem.unitValue = 60L;
		return gameItem;
	}
}

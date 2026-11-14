public class ConstructionItemDirectory : ItemDirectory
{
	public const string CONSTRUCTION_MATERIAL_TAG = "CONSTRUCTION_MATERIAL_TAG";

	protected override void InitDirectory()
	{
		Add("nuts_metal", () => Nuts());
	}

	private GameItem Nuts(int count = 1)
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_construction", "nuts_metal_pile");
		gameItem.unitCount = count;
		gameItem.name = LocHelper.GetLocalizedItem("item_nuts_metal_name");
		gameItem.SetGameItemType("MATERIAL");
		gameItem.unitValue = 14L;
		gameItem.SetType("MATERIAL_TAG");
		gameItem.EnableTag("MATERIAL");
		AudioHelper.InitTinyMetal(gameItem);
		return gameItem;
	}
}

using System.Collections.Generic;

public static class LouToyBoxHelper
{
	public enum Outcome
	{
		Untouched,
		Peeked,
		Taken
	}

	public const string TOY_BOX_TAG = "LOU_TOY_BOX_TAG";

	public const string TOY_TAG = "LOU_TOY_TAG";

	public const string TOY_COUNT_INT = "LOU_TOY_COUNT_INT";

	public const string OLD_ORDERS_ID = "lou_old_orders";

	private static readonly string[] DEFAULT_TOYS = new string[4] { "wire_cutter", "welder", "tazer", "kitchen_knife" };

	public const string LOU_BOMBED_ENDING = "lou_bombed";

	public const string LOU_DEBT_ENDING = "lou_bombed_debt";

	public static bool IsInsideLouBox(GameItem item)
	{
		GameItem gameItem = item?.parentInventory?.GetParentItem();
		int num = 0;
		while (gameItem != null && num < 32)
		{
			if (gameItem.IsTag("LOU_TOY_BOX_TAG") || gameItem.IsTag("LOU_LOCKER_TAG"))
			{
				return true;
			}
			gameItem = gameItem.parentInventory?.GetParentItem();
			num++;
		}
		return false;
	}

	public static GameItem CreateOldOrders()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_misc", "aug_id_guide");
		gameItem.name = LocHelper.GetLocalizedItem("item_lou_old_orders_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_lou_old_orders_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_lou_old_orders_flavor");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.unitValue = 0L;
		return gameItem;
	}

	public static GameItem CreateToyBox()
	{
		GameItem gameItem = DirectoryMaster.Item("evidence_box");
		List<GameItem> list = new List<GameItem> { DirectoryMaster.Item("lou_old_orders") };
		List<string> louBoughtItemIds = PlayerStore.Instance.storeClientData.louBoughtItemIds;
		string[] array = ((louBoughtItemIds != null && louBoughtItemIds.Count > 0) ? louBoughtItemIds.ToArray() : DEFAULT_TOYS);
		for (int i = 0; i < array.Length; i++)
		{
			GameItem gameItem2 = DirectoryMaster.Item(array[i]);
			if (gameItem2 != null)
			{
				list.Add(gameItem2);
			}
		}
		int placed = 0;
		foreach (GameItem item in list)
		{
			item.EnableTag("LOU_TOY_TAG");
			item.EnableTag("IMPORTANT_TAG");
			if (GraphUtils.TryAcceptAll(gameItem, item) == 0)
			{
				placed++;
			}
			else
			{
				GraphUtils.DestroyIfUnplaced(item, "Lou's box of toys is full");
			}
		}
		LockHelper.UnlockContainer(gameItem, force: true);
		gameItem.name = LocHelper.GetLocalizedItem("item_lou_toy_box_name");
		gameItem.shortDescription = "";
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_lou_toy_box_flavor");
		gameItem.EnableTag("LOU_TOY_BOX_TAG");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("TRACK_OPENED_TAG");
		gameItem.ModifyTag("LOU_TOY_COUNT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(placed);
		});
		return gameItem;
	}

	public static void GiveToyBox()
	{
		GameItem gameItem = CreateToyBox();
		GeneralHelper.SetItemOwned(gameItem);
		if (GraphUtils.TryAcceptAllMid(EmporiumEntry.Instance.frontInvinvElement, gameItem) != 0)
		{
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.frontInvinvElement, gameItem);
			if (gameItem.parentInventory == null)
			{
				PlayerStore.Instance.storeClientData.isLouBoxNotPlaced = true;
			}
		}
	}

	public static Outcome CollectBox()
	{
		GameItem gameItem = EmporiumEntry.Instance.GetAllItems().Find((GameItem item) => item?.IsTag("LOU_TOY_BOX_TAG") ?? false);
		if (gameItem == null)
		{
			if (!PlayerStore.Instance.storeClientData.isLouBoxNotPlaced)
			{
				return Outcome.Taken;
			}
			return Outcome.Untouched;
		}
		int @int = gameItem.GetTagReadonly("LOU_TOY_COUNT_INT").GetInt();
		int count = GraphUtils.FindChildrenType(gameItem, (GameItem item) => item.IsTag("LOU_TOY_TAG"), (GameItem _) => true).Count;
		bool flag = gameItem.IsTag("OPENED_TAG");
		LouOperativeHelper.TryCaptureThroughContainer(gameItem);
		foreach (GameItem item in GraphUtils.FindChildrenType(gameItem, (GameItem item) => !item.IsTag("LOU_TOY_TAG")))
		{
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.frontInvinvElement, item);
		}
		gameItem.Destroy();
		if (count < @int)
		{
			return Outcome.Taken;
		}
		if (!flag)
		{
			return Outcome.Untouched;
		}
		return Outcome.Peeked;
	}

	public static void PlantRealBomb(string ending = "lou_bombed")
	{
		GameItem item = DirectoryMaster.Item("c4_set");
		if (GraphUtils.TryAcceptAllMid(EmporiumEntry.Instance.frontInvinvElement, item) > 0)
		{
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.frontInvinvElement, item);
		}
		StoreUIManager.Instance.TriggerBombing(ending);
	}
}

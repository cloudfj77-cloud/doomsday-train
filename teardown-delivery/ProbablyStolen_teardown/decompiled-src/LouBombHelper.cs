using System.Collections.Generic;
using System.Linq;

public static class LouBombHelper
{
	public const string BOMB_ID = "lou_bomb";

	public const string BOMB_ARMED_TAG = "LOU_BOMB_ARMED_TAG";

	public const int LOU_BOMB_DELAY = 2;

	public static GameItem CreateBomb()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/itematlas", "c4");
		gameItem.name = LocHelper.GetLocalizedItem("item_lou_bomb_armed_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_lou_bomb_armed_desc");
		gameItem.unitValue = 0L;
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("LOU_BOMB_ARMED_TAG");
		gameItem.canActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s) && s.IsTag("LOU_BOMB_ARMED_TAG");
		gameItem.mayActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s) && s.IsTag("LOU_BOMB_ARMED_TAG");
		gameItem.onActivateSlotItemFunc = delegate(GameItem s, GameInventory _, SlotMarker _)
		{
			Disarm(s);
		};
		gameItem.soundActivate = "moddingPlastic1";
		return gameItem;
	}

	public static void Disarm(GameItem bomb)
	{
		if (bomb.IsTag("LOU_BOMB_ARMED_TAG"))
		{
			bomb.DisableTag("LOU_BOMB_ARMED_TAG");
			bomb.name = LocHelper.GetLocalizedItem("item_lou_bomb_fake_name");
			bomb.shortDescription = LocHelper.GetLocalizedItem("item_lou_bomb_fake_desc");
		}
	}

	public static void OnNight()
	{
		foreach (GameItem allItem in EmporiumEntry.Instance.GetAllItems())
		{
			if (allItem != null && allItem.identifier == "lou_bomb" && allItem.IsTag("LOU_BOMB_ARMED_TAG"))
			{
				Disarm(allItem);
			}
		}
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		if (storeClientData.louMorningDay >= 0 && !storeClientData.isLouBombPlaced && !LouOperativeHelper.IsLouGamesStopped() && StoreStation.Instance.dayCounter >= storeClientData.louMorningDay + StoreClientData.LouDelay(2) - 1)
		{
			PlantBomb();
		}
	}

	public static void PlantBomb()
	{
		StoreClientData storeClientData = PlayerStore.Instance.storeClientData;
		storeClientData.isLouBombPlaced = true;
		storeClientData.isLouBombVisitPending = true;
		GameItem gameItem = DirectoryMaster.Item("lou_bomb");
		if (!TryHide(gameItem))
		{
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, gameItem);
		}
	}

	private static bool TryHide(GameItem bomb)
	{
		foreach (IGrouping<int, (GameItem, int)> item in from c in FindOwnedContainers(EmporiumEntry.Instance.invElement)
			group c by c.depth into g
			orderby g.Key descending
			select g)
		{
			foreach (var item2 in item.OrderBy<(GameItem, int), int>(((GameItem container, int depth) _) => RNG.GetRandomInt(0, 10000)))
			{
				if (GraphUtils.TryAcceptAll(item2.Item1, bomb) == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static List<(GameItem, int)> FindOwnedContainers(GraphNodeStorage root)
	{
		List<(GameItem, int)> list = new List<(GameItem, int)>();
		HashSet<GraphNodeStorage> hashSet = new HashSet<GraphNodeStorage>();
		Queue<(GraphNodeStorage, int)> queue = new Queue<(GraphNodeStorage, int)>();
		queue.Enqueue((root, 0));
		while (queue.Count > 0)
		{
			var (graphNodeStorage, num) = queue.Dequeue();
			if (graphNodeStorage == null || !hashSet.Add(graphNodeStorage))
			{
				continue;
			}
			foreach (GraphNodeStorage child in graphNodeStorage.children)
			{
				int item = num;
				if (child is GameItem gameItem)
				{
					item = num + 1;
					if (gameItem.contentWindow != null && GeneralHelper.IsItemOwned(gameItem) && !gameItem.IsGameItemType("MACHINE"))
					{
						list.Add((gameItem, item));
					}
				}
				queue.Enqueue((child, item));
			}
		}
		return list;
	}

	public static void QueueVisit(StoreClientManager manager)
	{
		if (PlayerStore.Instance.storeClientData.isLouBombVisitPending && !LouOperativeHelper.IsLouGamesStopped())
		{
			manager.AddNextClient(StoreClientListWanted.CreateLouBombVisit());
		}
	}
}

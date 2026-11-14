using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager
{
	private static long currentUUID;

	private static Dictionary<GameItem, long> itemHeap;

	private static Dictionary<long, GameItem> itemDict;

	public static string saveJson;

	public static bool IsDecoding { get; private set; }

	private static void InitHeap()
	{
		currentUUID = 0L;
		if (itemHeap == null)
		{
			itemHeap = new Dictionary<GameItem, long>();
		}
		else
		{
			itemHeap.Clear();
		}
		if (itemDict == null)
		{
			itemDict = new Dictionary<long, GameItem>();
		}
		else
		{
			itemDict.Clear();
		}
	}

	private static void AddHeap(GameItem item)
	{
		if (!itemHeap.ContainsKey(item))
		{
			itemHeap.Add(item, currentUUID);
			itemDict.Add(currentUUID, item);
			currentUUID++;
		}
	}

	private static SaveItemNode EncodeNode(GameItem firstNode)
	{
		SaveItemNode saveItemNode = new SaveItemNode();
		saveItemNode.identifier = firstNode.identifier;
		saveItemNode.unitCount = firstNode.unitCount;
		saveItemNode.uuid = itemHeap[firstNode];
		saveItemNode.itemType = firstNode.GetType().FullName;
		saveItemNode.itemState = firstNode.state.Clone();
		saveItemNode.itemShape = new GridShapeBuilder(firstNode.shape);
		saveItemNode.itemModifiedShape = new GridShapeBuilder(firstNode.modifiedShape);
		saveItemNode.name = firstNode.name;
		saveItemNode.shortDescription = firstNode.shortDescription;
		saveItemNode.flavorText = firstNode.flavorText;
		saveItemNode.uniqueId = firstNode.uniqueId;
		saveItemNode.unitBaseValue = firstNode.unitBaseValue;
		saveItemNode.unitValue = firstNode.unitValue;
		saveItemNode.lateUnitValue = firstNode.lateUnitValue;
		saveItemNode.backupUnitValue = firstNode.backupUnitValue;
		saveItemNode.spriteAtlasPath = firstNode.spriteAtlasPath;
		saveItemNode.spritePath = firstNode.spritePath;
		saveItemNode.itemTypes = firstNode.itemTypes.ToList();
		foreach (ItemFeature itemFeature in firstNode.itemFeatures)
		{
			PlayerStore.Instance.savedItemFeatureList.Add(itemFeature);
		}
		int num = 0;
		Dictionary<GraphNodeStorage, int> dictionary = new Dictionary<GraphNodeStorage, int>();
		Dictionary<int, GraphNodeStorage> dictionary2 = new Dictionary<int, GraphNodeStorage>();
		Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>();
		queue.Enqueue(firstNode);
		while (queue.Count > 0)
		{
			GraphNodeStorage graphNodeStorage = queue.Dequeue();
			if (graphNodeStorage == firstNode)
			{
				foreach (GraphNodeStorage child in graphNodeStorage.children)
				{
					queue.Enqueue(child);
				}
				continue;
			}
			if (!(graphNodeStorage is GameItem))
			{
				if (!dictionary.ContainsKey(graphNodeStorage))
				{
					dictionary.Add(graphNodeStorage, num);
					dictionary2.Add(num, graphNodeStorage);
					num++;
				}
				foreach (GraphNodeStorage child2 in graphNodeStorage.children)
				{
					queue.Enqueue(child2);
				}
			}
			if (graphNodeStorage is GameItem gameItem)
			{
				long item = itemHeap[gameItem];
				int item2 = dictionary[gameItem.parentInventory];
				saveItemNode.childItems.Add(item);
				saveItemNode.childItemInventoryNode.Add(item2);
			}
		}
		return saveItemNode;
	}

	public static List<SaveItemNode> EncodeNodes(GraphNodeStorage firstNode)
	{
		InitHeap();
		Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>();
		queue.Enqueue(firstNode);
		while (queue.Count > 0)
		{
			GraphNodeStorage graphNodeStorage = queue.Dequeue();
			if (graphNodeStorage is GameItem item)
			{
				AddHeap(item);
			}
			foreach (GraphNodeStorage child in graphNodeStorage.children)
			{
				queue.Enqueue(child);
			}
		}
		List<SaveItemNode> list = new List<SaveItemNode>();
		foreach (var (firstNode2, _) in itemHeap)
		{
			list.Add(EncodeNode(firstNode2));
		}
		return list;
	}

	public static GameItem DecodeNodes(List<SaveItemNode> savedItems)
	{
		bool isDecoding = IsDecoding;
		IsDecoding = true;
		try
		{
			return DecodeNodesInternal(savedItems);
		}
		finally
		{
			IsDecoding = isDecoding;
		}
	}

	private static GameItem DecodeNodesInternal(List<SaveItemNode> savedItems)
	{
		InitHeap();
		GameItem gameItem = null;
		foreach (SaveItemNode savedItem in savedItems)
		{
			GameItem gameItem2 = ((!(savedItem.itemType == typeof(GameCharacterItem).FullName)) ? DirectoryMaster.Item(savedItem.identifier) : DirectoryMaster.CharacterItem(savedItem.identifier));
			gameItem2.unitCount = savedItem.unitCount;
			gameItem2.state = savedItem.itemState.Clone();
			gameItem2.modifiedState = savedItem.itemState.Clone();
			gameItem2.shape = savedItem.itemShape.Build();
			gameItem2.modifiedShape = savedItem.itemModifiedShape.Build();
			if (savedItem.name != null)
			{
				gameItem2.name = LocHelper.RelocalizeIfMissing(savedItem.name);
			}
			gameItem2.shortDescription = LocHelper.RelocalizeIfMissing(savedItem.shortDescription);
			gameItem2.flavorText = LocHelper.RelocalizeIfMissing(savedItem.flavorText);
			gameItem2.uniqueId = savedItem.uniqueId;
			gameItem2.unitBaseValue = savedItem.unitBaseValue;
			gameItem2.unitValue = savedItem.unitValue;
			gameItem2.lateUnitValue = savedItem.lateUnitValue;
			gameItem2.backupUnitValue = savedItem.backupUnitValue;
			gameItem2.itemTypes = savedItem.itemTypes.ToList();
			gameItem2.SetSprite(savedItem.spriteAtlasPath, savedItem.spritePath);
			StorageExpansionHelper.RestoreTabs(gameItem2);
			itemHeap.Add(gameItem2, savedItem.uuid);
			itemDict.Add(savedItem.uuid, gameItem2);
			savedItem.tempLink = gameItem2;
			if (gameItem == null)
			{
				gameItem = gameItem2;
			}
		}
		foreach (SaveItemNode savedItem2 in savedItems)
		{
			GraphNodeStorage tempLink = savedItem2.tempLink;
			Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>();
			queue.Enqueue(tempLink);
			int num = 0;
			Dictionary<GraphNodeStorage, int> dictionary = new Dictionary<GraphNodeStorage, int>();
			Dictionary<int, GraphNodeStorage> dictionary2 = new Dictionary<int, GraphNodeStorage>();
			while (queue.Count > 0)
			{
				GraphNodeStorage graphNodeStorage = queue.Dequeue();
				if (graphNodeStorage == tempLink)
				{
					foreach (GraphNodeStorage child in graphNodeStorage.children)
					{
						queue.Enqueue(child);
					}
				}
				else
				{
					if (graphNodeStorage is GameItem)
					{
						continue;
					}
					if (!dictionary.ContainsKey(graphNodeStorage))
					{
						dictionary.Add(graphNodeStorage, num);
						dictionary2.Add(num, graphNodeStorage);
						num++;
					}
					foreach (GraphNodeStorage child2 in graphNodeStorage.children)
					{
						queue.Enqueue(child2);
					}
				}
			}
			foreach (Tuple<long, int> item4 in savedItem2.childItems.Zip(savedItem2.childItemInventoryNode, Tuple.Create))
			{
				item4.Deconstruct(out var item, out var item2);
				long key = item;
				int key2 = item2;
				GameItem item3 = itemDict[key];
				if (!dictionary2.ContainsKey(key2))
				{
					Debug.LogWarning("Warning, child item load failure, Item ID:" + key + ", Inv ID:" + key2 + ", Parent ID:" + savedItem2.identifier + "(" + savedItem2.uuid + ")");
					continue;
				}
				try
				{
					((GameInventory)dictionary2[key2]).UncheckedAccept(item3);
				}
				catch (Exception)
				{
					Debug.LogWarning("Warning, GameInventory is invalid, Item ID:" + key + ", Inv ID:" + key2 + ", Parent ID:" + savedItem2.identifier + "(" + savedItem2.uuid + ")");
				}
			}
		}
		return gameItem;
	}

	public static bool HasSave()
	{
		return saveJson != null;
	}

	public static void Save()
	{
		StallWatchdog.Crumb("save");
		saveJson = JsonUtility.ToJson(new SaveState
		{
			saveItems = EncodeNodes(EmporiumEntry.Instance.invElement)
		});
	}

	public static void Load()
	{
		if (HasSave())
		{
			StallWatchdog.Crumb("load");
			GameItem ship = DecodeNodes(JsonUtility.FromJson<SaveState>(saveJson).saveItems);
			GameMaster.current.shipManager.ship = ship;
		}
	}
}

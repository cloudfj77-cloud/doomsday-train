using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GraphUtils
{
	private static readonly Dictionary<(string identifier, string context), int> unplacedCounts = new Dictionary<(string, string), int>();

	private static bool unplacedFlushScheduled;

	public static void EmporiumTryAdd(GraphNodeStorage currentNode, GameItem item)
	{
		if (item != null && !TryAddInto(currentNode, item) && (!(currentNode is GameInventory grid) || !HopperHelper.TryRouteOverflow(grid, item)) && (!(MapUIManager.Instance != null) || !MapUIManager.Instance.isOutside || item.parentInventory == EmporiumEntry.Instance.afterhourInventory || !TryAddInto(EmporiumEntry.Instance.afterhourWindow, item)) && !TryAddInto(EmporiumEntry.Instance.backInvinvElement, item) && !TryAddInto(EmporiumEntry.Instance.invElement, item) && !TryAddInto(EmporiumEntry.Instance.showcaseElement, item) && !TryAddInto(EmporiumEntry.Instance.frontInvinvElement, item))
		{
			DestroyIfUnplaced(item, "EmporiumTryAdd: every store inventory is full");
		}
	}

	private static bool TryAddInto(GraphNodeStorage node, GameItem item)
	{
		if (node == null)
		{
			return false;
		}
		if (!CanAccept(node, item))
		{
			return false;
		}
		if (TryAcceptAll(node, item) > 0)
		{
			return false;
		}
		if (item.parentInventory == null)
		{
			DestroyIfUnplaced(item);
		}
		return true;
	}

	public static bool DestroyIfUnplaced(GameItem item, string context = null)
	{
		if (item == null)
		{
			return false;
		}
		if (item.parentInventory != null)
		{
			return true;
		}
		if (item is GameItemElement gameItemElement && gameItemElement.IsDestroyed())
		{
			return false;
		}
		if (item.unitCount > 0)
		{
			ReportUnplaced(item.identifier, item.unitCount, context);
		}
		item.Destroy();
		return false;
	}

	public static void ReportUnplaced(string identifier, int count, string context = null)
	{
		(string, string) key = (identifier ?? "?", context ?? "");
		unplacedCounts[key] = (unplacedCounts.TryGetValue(key, out var value) ? value : 0) + count;
		if (!unplacedFlushScheduled)
		{
			unplacedFlushScheduled = true;
			LetterboxManager.RunNextUpdate(FlushUnplaced);
		}
	}

	private static void FlushUnplaced()
	{
		unplacedFlushScheduled = false;
		if (unplacedCounts.Count == 0)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder("Destroyed unplaced items:");
		foreach (KeyValuePair<(string, string), int> unplacedCount in unplacedCounts)
		{
			stringBuilder.Append(" '").Append(unplacedCount.Key.Item1).Append("' x")
				.Append(unplacedCount.Value);
			if (unplacedCount.Key.Item2.Length > 0)
			{
				stringBuilder.Append(" (").Append(unplacedCount.Key.Item2).Append(')');
			}
			stringBuilder.Append(';');
		}
		unplacedCounts.Clear();
		Debug.LogWarning(stringBuilder.ToString());
	}

	public static bool TryAcceptAllOrDestroy(GraphNodeStorage currentNode, GameItem item, string context = null)
	{
		if (item == null)
		{
			return false;
		}
		TryAcceptAll(currentNode, item);
		return DestroyIfUnplaced(item, context);
	}

	public static void EmporiumTryAdd(GameItem item)
	{
		EmporiumTryAdd(EmporiumEntry.Instance.invElement, item);
	}

	public static bool CanAccept(GraphNodeStorage currentNode, GameItem item)
	{
		if (currentNode.TryFindOneValidInventorySlot(item) == null)
		{
			return false;
		}
		return true;
	}

	public static int TryAccept(GraphNodeStorage currentNode, GameItem item)
	{
		if (item == null)
		{
			return 0;
		}
		return currentNode.TryFindOneValidInventorySlot(item)?.TryAcceptOnce(1) ?? (-1);
	}

	public static int TryAcceptOnce(GraphNodeStorage currentNode, GameItem item, int amount = -1)
	{
		if (item == null)
		{
			return 0;
		}
		if (amount < 0)
		{
			amount = item.unitCount;
		}
		if (amount <= 0)
		{
			return 0;
		}
		int num = 0;
		if (num >= amount)
		{
			return amount - num;
		}
		SlotMarker slotMarker = currentNode.TryFindOneValidInventorySlot(item);
		if (slotMarker == null)
		{
			return amount - num;
		}
		if (slotMarker.numTransfer <= 0)
		{
			return amount - num;
		}
		int num2 = slotMarker.TryAcceptOnce(amount - num);
		if (num2 == 0)
		{
			return amount - num;
		}
		num += num2;
		return amount - num;
	}

	public static int TryAcceptAll(GraphNodeStorage currentNode, GameItem item, int amount = -1)
	{
		if (item == null)
		{
			return 0;
		}
		if (currentNode == null)
		{
			return 0;
		}
		if (amount < 0)
		{
			amount = item.unitCount;
		}
		if (amount <= 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < 40; i++)
		{
			if (num >= amount)
			{
				break;
			}
			SlotMarker slotMarker = currentNode.TryFindOneValidInventorySlot(item);
			if (slotMarker == null || slotMarker.numTransfer <= 0)
			{
				break;
			}
			int num2 = slotMarker.TryAcceptOnce(amount - num);
			if (num2 == 0)
			{
				break;
			}
			num += num2;
		}
		return amount - num;
	}

	public static int TryAcceptAllMid(GameGridInventory currentNode, GameItem item, int amount = -1)
	{
		if (item == null)
		{
			return 0;
		}
		if (amount < 0)
		{
			amount = item.unitCount;
		}
		if (amount <= 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < 40; i++)
		{
			if (num >= amount)
			{
				break;
			}
			SlotMarker slotMarker = currentNode.TryFindOneValidInventorySlot_TopMiddle(item);
			if (slotMarker == null || slotMarker.numTransfer <= 0)
			{
				break;
			}
			int num2 = slotMarker.TryAcceptOnce(amount - num);
			if (num2 == 0)
			{
				break;
			}
			num += num2;
		}
		return amount - num;
	}

	public static int TryDestroyAll(List<GameItem> items, int amount = -1)
	{
		if (amount <= 0)
		{
			items.ForEach(delegate(GameItem item)
			{
				item.Destroy();
			});
			return 0;
		}
		int num = amount;
		foreach (GameItem item in items)
		{
			int num2 = Math.Min(num, item.unitCount);
			if (num2 == item.unitCount)
			{
				item.Destroy();
			}
			else
			{
				item.SetAmount(item.unitCount - num2);
			}
			num -= num2;
			if (num <= 0)
			{
				break;
			}
		}
		return num;
	}

	public static int CountAll(List<GameItem> items)
	{
		int num = 0;
		foreach (GameItem item in items)
		{
			num += item.unitCount;
		}
		return num;
	}

	public static bool FindParentEquals(GraphNodeStorage currentNode, GraphNodeStorage node)
	{
		if (currentNode == null || node == null)
		{
			return false;
		}
		Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>();
		queue.Enqueue(currentNode);
		while (queue.Count > 0)
		{
			GraphNodeStorage graphNodeStorage = queue.Dequeue();
			if (graphNodeStorage != currentNode && graphNodeStorage == node)
			{
				return true;
			}
			foreach (GraphNodeStorage parent in graphNodeStorage.parents)
			{
				queue.Enqueue(parent);
			}
		}
		return false;
	}

	public static bool FindChildEquals(GraphNodeStorage currentNode, GraphNodeStorage node)
	{
		if (currentNode == null || node == null)
		{
			return false;
		}
		Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>();
		queue.Enqueue(currentNode);
		while (queue.Count > 0)
		{
			GraphNodeStorage graphNodeStorage = queue.Dequeue();
			if (graphNodeStorage != currentNode && graphNodeStorage == node)
			{
				return true;
			}
			foreach (GraphNodeStorage child in graphNodeStorage.children)
			{
				queue.Enqueue(child);
			}
		}
		return false;
	}

	public static bool FindParentEqualsIgnoreType<T>(GraphNodeStorage currentNode, GraphNodeStorage node)
	{
		if (currentNode == null || node == null)
		{
			return false;
		}
		Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>();
		queue.Enqueue(currentNode);
		while (queue.Count > 0)
		{
			GraphNodeStorage graphNodeStorage = queue.Dequeue();
			if (graphNodeStorage != currentNode)
			{
				if (graphNodeStorage is T)
				{
					continue;
				}
				if (graphNodeStorage == node)
				{
					return true;
				}
			}
			foreach (GraphNodeStorage parent in graphNodeStorage.parents)
			{
				queue.Enqueue(parent);
			}
		}
		return false;
	}

	public static bool FindChildEqualsIgnoreType<T>(GraphNodeStorage currentNode, GraphNodeStorage node)
	{
		if (currentNode == null || node == null)
		{
			return false;
		}
		Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>();
		queue.Enqueue(currentNode);
		while (queue.Count > 0)
		{
			GraphNodeStorage graphNodeStorage = queue.Dequeue();
			if (graphNodeStorage != currentNode)
			{
				if (graphNodeStorage is T)
				{
					continue;
				}
				if (graphNodeStorage == node)
				{
					return true;
				}
			}
			foreach (GraphNodeStorage child in graphNodeStorage.children)
			{
				queue.Enqueue(child);
			}
		}
		return false;
	}

	public static T FindParentType<T>(GraphNodeStorage currentNode, Func<T, bool> filter = null, Func<T, bool> passthrough = null)
	{
		if (currentNode == null)
		{
			return default(T);
		}
		if (filter == null)
		{
			filter = (T c) => true;
		}
		if (passthrough == null)
		{
			passthrough = (T c) => false;
		}
		new HashSet<T>();
		Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>();
		queue.Enqueue(currentNode);
		while (queue.Count > 0)
		{
			GraphNodeStorage graphNodeStorage = queue.Dequeue();
			if (graphNodeStorage != currentNode && graphNodeStorage is T val && filter(val))
			{
				return val;
			}
			if (graphNodeStorage != currentNode && graphNodeStorage is T && (!(graphNodeStorage is T arg) || !passthrough(arg)))
			{
				continue;
			}
			foreach (GraphNodeStorage parent in graphNodeStorage.parents)
			{
				queue.Enqueue(parent);
			}
		}
		return default(T);
	}

	public static T FindChildType<T>(GraphNodeStorage currentNode, Func<T, bool> filter = null, Func<T, bool> passthrough = null)
	{
		if (currentNode == null)
		{
			return default(T);
		}
		if (filter == null)
		{
			filter = (T c) => true;
		}
		if (passthrough == null)
		{
			passthrough = (T c) => false;
		}
		Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>();
		queue.Enqueue(currentNode);
		while (queue.Count > 0)
		{
			GraphNodeStorage graphNodeStorage = queue.Dequeue();
			if (graphNodeStorage != currentNode && graphNodeStorage is T val && filter(val))
			{
				return val;
			}
			if (graphNodeStorage != currentNode && graphNodeStorage is T && (!(graphNodeStorage is T arg) || !passthrough(arg)))
			{
				continue;
			}
			foreach (GraphNodeStorage child in graphNodeStorage.children)
			{
				queue.Enqueue(child);
			}
		}
		return default(T);
	}

	public static List<T> FindChildrenType<T>(GraphNodeStorage currentNode, Func<T, bool> filter = null, Func<T, bool> passthrough = null)
	{
		if (currentNode == null)
		{
			return new List<T>();
		}
		if (filter == null)
		{
			filter = (T c) => true;
		}
		if (passthrough == null)
		{
			passthrough = (T c) => false;
		}
		HashSet<T> hashSet = new HashSet<T>();
		Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>();
		queue.Enqueue(currentNode);
		while (queue.Count > 0)
		{
			GraphNodeStorage graphNodeStorage = queue.Dequeue();
			if (graphNodeStorage != currentNode && graphNodeStorage is T val && filter(val))
			{
				hashSet.Add(val);
			}
			if (graphNodeStorage != currentNode && graphNodeStorage is T && (!(graphNodeStorage is T arg) || !passthrough(arg)))
			{
				continue;
			}
			foreach (GraphNodeStorage child in graphNodeStorage.children)
			{
				queue.Enqueue(child);
			}
		}
		return hashSet.ToList();
	}

	public static List<T> FindParentsType<T>(GraphNodeStorage currentNode, Func<T, bool> filter = null, Func<T, bool> passthrough = null)
	{
		if (currentNode == null)
		{
			return new List<T>();
		}
		if (filter == null)
		{
			filter = (T c) => true;
		}
		if (passthrough == null)
		{
			passthrough = (T c) => false;
		}
		HashSet<T> hashSet = new HashSet<T>();
		Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>();
		queue.Enqueue(currentNode);
		while (queue.Count > 0)
		{
			GraphNodeStorage graphNodeStorage = queue.Dequeue();
			if (graphNodeStorage != currentNode && graphNodeStorage is T val && filter(val))
			{
				hashSet.Add(val);
			}
			if (graphNodeStorage != currentNode && graphNodeStorage is T && (!(graphNodeStorage is T arg) || !passthrough(arg)))
			{
				continue;
			}
			foreach (GraphNodeStorage parent in graphNodeStorage.parents)
			{
				queue.Enqueue(parent);
			}
		}
		return hashSet.ToList();
	}

	public static bool SwapItemsInPlace(GameItem source, GameItem target)
	{
		if (source == null || target == null || source == target)
		{
			return false;
		}
		GameSlotInventory gameSlotInventory = ((EmporiumEntry.Instance != null) ? EmporiumEntry.Instance.swapBufferElement : null);
		if (gameSlotInventory == null)
		{
			return false;
		}
		GameInventory parentInventory = source.parentInventory;
		GameInventory parentInventory2 = target.parentInventory;
		if (parentInventory == null || parentInventory2 == null)
		{
			return false;
		}
		GridShape modifiedShape = source.modifiedShape;
		GridShape modifiedShape2 = target.modifiedShape;
		if (gameSlotInventory.childItem != null)
		{
			TryAcceptOnce(EmporiumEntry.Instance.invElement, gameSlotInventory.childItem);
			if (gameSlotInventory.childItem != null)
			{
				return false;
			}
		}
		if (TryAcceptOnce(gameSlotInventory, source) > 0)
		{
			return false;
		}
		if (!PlaceAtShape(target, parentInventory, modifiedShape))
		{
			PlaceAtShape(source, parentInventory, modifiedShape);
			return false;
		}
		if (!PlaceAtShape(source, parentInventory2, modifiedShape2))
		{
			PlaceAtShape(target, parentInventory2, modifiedShape2);
			PlaceAtShape(source, parentInventory, modifiedShape);
			return false;
		}
		return true;
	}

	private static bool PlaceAtShape(GameItem item, GameInventory inventory, GridShape placedShape)
	{
		if (item == null || inventory == null || placedShape == null)
		{
			return false;
		}
		int unitCount = item.unitCount;
		return new SlotMarker
		{
			item = item,
			inventory = inventory,
			itemGridShape = placedShape,
			numTransfer = unitCount
		}.TryAcceptOnce() >= unitCount;
	}

	public static List<T> FindAllChildrenType<T>(GraphNodeStorage currentNode, Func<T, bool> filter = null, Func<T, bool> passthrough = null)
	{
		if (currentNode == null)
		{
			return new List<T>();
		}
		if (filter == null)
		{
			filter = (T c) => true;
		}
		if (passthrough == null)
		{
			passthrough = (T c) => true;
		}
		HashSet<T> hashSet = new HashSet<T>();
		Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>();
		queue.Enqueue(currentNode);
		while (queue.Count > 0)
		{
			GraphNodeStorage graphNodeStorage = queue.Dequeue();
			if (graphNodeStorage != currentNode && graphNodeStorage is T val && filter(val))
			{
				hashSet.Add(val);
			}
			if (graphNodeStorage != currentNode && (!(graphNodeStorage is T arg) || !passthrough(arg)) && graphNodeStorage is T)
			{
				continue;
			}
			foreach (GraphNodeStorage child in graphNodeStorage.children)
			{
				queue.Enqueue(child);
			}
		}
		return hashSet.ToList();
	}

	public static bool CanInsertToActiveContainer(PixelWindow parentWindow, GameItem newItem, PixelWindow compareWindow)
	{
		if (compareWindow == parentWindow)
		{
			PixelWindow pixelWindow = WindowsHandler.current.visibleWindows.FindLast((PixelWindow w) => w != parentWindow && (w.TryFindOneValidInventorySlot(newItem)?.IsValid() ?? false));
			if (pixelWindow.canMove && !StoreUIManager.Instance.IsInnateWindow(pixelWindow) && pixelWindow != null)
			{
				return true;
			}
		}
		return false;
	}

	public static void InsertToActiveContainer(PixelWindow targetWindow, GameItem newItem)
	{
		if (targetWindow != null)
		{
			TryAcceptAll(targetWindow, newItem);
		}
	}
}

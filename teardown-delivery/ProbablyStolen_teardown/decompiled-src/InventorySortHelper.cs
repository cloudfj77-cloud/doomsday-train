using System;
using System.Collections.Generic;
using System.Linq;

public static class InventorySortHelper
{
	private sealed class Context
	{
		public GameGridInventory inventory;

		public GridShape inventoryShape;

		public List<GameItem> movingItems;

		public Dictionary<GameItem, int> preferredOrientation;

		public bool autoRotate;

		public bool fromEnd;

		public byte[,] itemLayers;
	}

	private sealed class Shelf
	{
		public int edgeRow;

		public int height;
	}

	private static readonly Dictionary<GameGridInventory, List<(GameItem item, GridShape shape)>> undoSnapshots = new Dictionary<GameGridInventory, List<(GameItem, GridShape)>>();

	private static bool IsAnchored(GameItem item)
	{
		if (!ItemPinHelper.IsPinned(item))
		{
			return DebrisHelper.IsDebris(item);
		}
		return true;
	}

	public static bool MaySort(GameGridInventory inventory)
	{
		if (inventory == null)
		{
			return false;
		}
		if (inventory.IsInsertLocked())
		{
			return false;
		}
		if (!GeneralHelper.MayPlayerInsertInto(inventory))
		{
			return false;
		}
		return inventory.childItems.Any((GameItem item) => !IsAnchored(item));
	}

	public static bool CanSort(GameGridInventory inventory)
	{
		if (!MaySort(inventory))
		{
			return false;
		}
		foreach (GameItem childItem in inventory.childItems)
		{
			if (!IsAnchored(childItem))
			{
				if (childItem is GameItemElement gameItemElement && gameItemElement.IsDestroyed())
				{
					return false;
				}
				if (!childItem.MayRemove() || childItem.MaxNumRemove() < childItem.unitCount)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool Sort(GameGridInventory inventory, bool bigFirst, bool fromEnd)
	{
		if (!CanSort(inventory))
		{
			return false;
		}
		bool sortAutoRotate = EmporiumOption.SortAutoRotate;
		GridShape inventoryShape = inventory.inventoryShape;
		List<GameItem> list = inventory.childItems.Where((GameItem item) => !IsAnchored(item)).ToList();
		List<GameItem> pinnedItems = inventory.childItems.Where((GameItem item) => IsAnchored(item)).ToList();
		Dictionary<GameItem, int> preferred = new Dictionary<GameItem, int>();
		foreach (GameItem item in list)
		{
			preferred[item] = (sortAutoRotate ? WideOrientation(item.modifiedShape) : item.modifiedShape.orientation);
		}
		list.Sort(delegate(GameItem a, GameItem b)
		{
			int num = OrientedHeight(a.modifiedShape, preferred[a]);
			int num2 = OrientedHeight(b.modifiedShape, preferred[b]);
			if (num != num2)
			{
				if (!bigFirst)
				{
					return num.CompareTo(num2);
				}
				return num2.CompareTo(num);
			}
			int num3 = CellCount(a.modifiedShape);
			int num4 = CellCount(b.modifiedShape);
			if (num3 != num4)
			{
				if (!bigFirst)
				{
					return num3.CompareTo(num4);
				}
				return num4.CompareTo(num3);
			}
			int num5 = string.Compare(a.identifier, b.identifier, StringComparison.Ordinal);
			if (num5 != 0)
			{
				return num5;
			}
			int num6 = a.modifiedShape.minY * inventoryShape.width + a.modifiedShape.minX;
			int value = b.modifiedShape.minY * inventoryShape.width + b.modifiedShape.minX;
			return num6.CompareTo(value);
		});
		Context context = new Context
		{
			inventory = inventory,
			inventoryShape = inventoryShape,
			movingItems = list,
			preferredOrientation = preferred,
			autoRotate = sortAutoRotate,
			fromEnd = fromEnd
		};
		List<(GameItem, GridShape)> list2 = PackShelves(NewOccupancy(context, pinnedItems)) ?? PackFirstFit(NewOccupancy(context, pinnedItems));
		if (list2 == null)
		{
			return false;
		}
		undoSnapshots[inventory] = list.Select((GameItem item) => (item: item, item.modifiedShape.Clone())).ToList();
		Commit(inventory, list2);
		return true;
	}

	public static bool MayUndo(GameGridInventory inventory)
	{
		if (inventory != null)
		{
			return undoSnapshots.ContainsKey(inventory);
		}
		return false;
	}

	public static bool CanUndo(GameGridInventory inventory)
	{
		return ComputeUndo(inventory) != null;
	}

	public static bool Undo(GameGridInventory inventory)
	{
		List<(GameItem, GridShape)> list = ComputeUndo(inventory);
		if (list == null)
		{
			return false;
		}
		undoSnapshots.Remove(inventory);
		Commit(inventory, list);
		return true;
	}

	private static List<(GameItem item, GridShape shape)> ComputeUndo(GameGridInventory inventory)
	{
		if (inventory == null)
		{
			return null;
		}
		if (!undoSnapshots.TryGetValue(inventory, out List<(GameItem, GridShape)> value))
		{
			return null;
		}
		if (inventory.IsInsertLocked())
		{
			return null;
		}
		if (!GeneralHelper.MayPlayerInsertInto(inventory))
		{
			return null;
		}
		List<(GameItem, GridShape)> list = new List<(GameItem, GridShape)>();
		foreach (var (gameItem, item) in value)
		{
			if ((!(gameItem is GameItemElement gameItemElement) || !gameItemElement.IsDestroyed()) && gameItem.parentInventory == inventory && !IsAnchored(gameItem))
			{
				if (!gameItem.MayRemove() || gameItem.MaxNumRemove() < gameItem.unitCount)
				{
					return null;
				}
				list.Add((gameItem, item));
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		List<GameItem> list2 = list.Select<(GameItem, GridShape), GameItem>(((GameItem item, GridShape shape) entry) => entry.item).ToList();
		GridShape inventoryShape = inventory.inventoryShape;
		byte[,] itemLayers = new byte[inventoryShape.width, inventoryShape.height];
		foreach (GameItem childItem in inventory.childItems)
		{
			if (!list2.Contains(childItem))
			{
				MarkOccupied(itemLayers, childItem.modifiedShape);
			}
		}
		foreach (var item4 in list)
		{
			GameItem item2 = item4.Item1;
			GridShape item3 = item4.Item2;
			GridShapeBuilder gridShapeBuilder = new GridShapeBuilder(item2.modifiedShape).SetTransform(item3);
			if (gridShapeBuilder.minX < 0 || gridShapeBuilder.minY < 0)
			{
				return null;
			}
			if (gridShapeBuilder.maxX >= inventoryShape.width || gridShapeBuilder.maxY >= inventoryShape.height)
			{
				return null;
			}
			if (!FitsOccupancy(gridShapeBuilder, itemLayers))
			{
				return null;
			}
			if (!inventory.CheckShapePlacement(item2, gridShapeBuilder, list2))
			{
				return null;
			}
			MarkOccupied(itemLayers, gridShapeBuilder);
		}
		return list;
	}

	private static Context NewOccupancy(Context context, List<GameItem> pinnedItems)
	{
		context.itemLayers = new byte[context.inventoryShape.width, context.inventoryShape.height];
		foreach (GameItem pinnedItem in pinnedItems)
		{
			MarkOccupied(context.itemLayers, pinnedItem.modifiedShape);
		}
		return context;
	}

	private static void Commit(GameGridInventory inventory, List<(GameItem item, GridShape shape)> placements)
	{
		foreach (var placement in placements)
		{
			GameItem item = placement.item;
			if (!(item is GameItemElement gameItemElement) || !gameItemElement.IsDestroyed())
			{
				item.parentInventory?.Expel(item);
			}
		}
		foreach (var (gameItem, transform) in placements)
		{
			if (!(gameItem is GameItemElement gameItemElement2) || !gameItemElement2.IsDestroyed())
			{
				gameItem.modifiedShape = new GridShapeBuilder(gameItem.modifiedShape).SetTransform(transform);
				inventory.UncheckedAccept(gameItem);
			}
		}
		inventory.Validate();
	}

	private static List<(GameItem item, GridShape shape)> PackShelves(Context context)
	{
		List<(GameItem, GridShape)> list = new List<(GameItem, GridShape)>();
		List<Shelf> list2 = new List<Shelf>();
		int height = context.inventoryShape.height;
		int num = (context.fromEnd ? (height - 1) : 0);
		foreach (GameItem movingItem in context.movingItems)
		{
			GridShape gridShape = null;
			foreach (Shelf item in list2)
			{
				foreach (int item2 in Orientations(context, movingItem))
				{
					int num2 = OrientedHeight(movingItem.modifiedShape, item2);
					if (num2 <= item.height)
					{
						int row = (context.fromEnd ? (item.edgeRow - num2 + 1) : item.edgeRow);
						gridShape = ScanRow(context, movingItem, item2, row);
						if (gridShape != null)
						{
							break;
						}
					}
				}
				if (gridShape != null)
				{
					break;
				}
			}
			if (gridShape == null)
			{
				foreach (int item3 in Orientations(context, movingItem))
				{
					int num3 = OrientedHeight(movingItem.modifiedShape, item3);
					int num4 = (context.fromEnd ? (num - num3 + 1) : num);
					if (num4 >= 0 && num4 + num3 <= height)
					{
						gridShape = ScanRow(context, movingItem, item3, num4);
						if (gridShape != null)
						{
							list2.Add(new Shelf
							{
								edgeRow = num,
								height = num3
							});
							num = (context.fromEnd ? (num - num3) : (num + num3));
							break;
						}
					}
				}
			}
			if (gridShape == null)
			{
				return null;
			}
			MarkOccupied(context.itemLayers, gridShape);
			list.Add((movingItem, gridShape));
		}
		return list;
	}

	private static GridShape ScanRow(Context context, GameItem item, int orientation, int row)
	{
		GridShapeBuilder gridShapeBuilder = new GridShapeBuilder(item.modifiedShape).SetOrientation(orientation);
		int num = context.inventoryShape.width - gridShapeBuilder.globalWidth;
		if (num < 0)
		{
			return null;
		}
		for (int i = 0; i <= num; i++)
		{
			int minX = (context.fromEnd ? (num - i) : i);
			gridShapeBuilder.SetPosition(minX, row);
			if (Fits(context, item, gridShapeBuilder))
			{
				return gridShapeBuilder.Build();
			}
		}
		return null;
	}

	private static List<(GameItem item, GridShape shape)> PackFirstFit(Context context)
	{
		List<(GameItem, GridShape)> list = new List<(GameItem, GridShape)>();
		foreach (GameItem movingItem in context.movingItems)
		{
			GridShape gridShape = null;
			foreach (int item in Orientations(context, movingItem))
			{
				gridShape = ScanAll(context, movingItem, item);
				if (gridShape != null)
				{
					break;
				}
			}
			if (gridShape == null)
			{
				return null;
			}
			MarkOccupied(context.itemLayers, gridShape);
			list.Add((movingItem, gridShape));
		}
		return list;
	}

	private static GridShape ScanAll(Context context, GameItem item, int orientation)
	{
		GridShapeBuilder gridShapeBuilder = new GridShapeBuilder(item.modifiedShape).SetOrientation(orientation);
		int num = context.inventoryShape.width - gridShapeBuilder.globalWidth;
		int num2 = context.inventoryShape.height - gridShapeBuilder.globalHeight;
		if (num < 0 || num2 < 0)
		{
			return null;
		}
		for (int i = 0; i <= num2; i++)
		{
			int minY = (context.fromEnd ? (num2 - i) : i);
			for (int j = 0; j <= num; j++)
			{
				int minX = (context.fromEnd ? (num - j) : j);
				gridShapeBuilder.SetPosition(minX, minY);
				if (Fits(context, item, gridShapeBuilder))
				{
					return gridShapeBuilder.Build();
				}
			}
		}
		return null;
	}

	private static bool Fits(Context context, GameItem item, GridShapeBuilder candidate)
	{
		if (!FitsOccupancy(candidate, context.itemLayers))
		{
			return false;
		}
		return context.inventory.CheckShapePlacement(item, candidate, context.movingItems);
	}

	private static IEnumerable<int> Orientations(Context context, GameItem item)
	{
		int preferred = context.preferredOrientation[item];
		yield return preferred;
		if (context.autoRotate)
		{
			yield return (preferred + 1) % 4;
		}
	}

	private static int WideOrientation(GridShape shape)
	{
		if (shape.globalWidth >= shape.globalHeight)
		{
			return shape.orientation;
		}
		return (shape.orientation + 1) % 4;
	}

	private static int OrientedHeight(GridShape shape, int orientation)
	{
		if (orientation % 2 != 0)
		{
			return shape.width;
		}
		return shape.height;
	}

	private static bool FitsOccupancy(GridShape shape, byte[,] itemLayers)
	{
		for (int i = shape.minY; i <= shape.maxY; i++)
		{
			for (int j = shape.minX; j <= shape.maxX; j++)
			{
				byte b = shape.Get(j, i);
				if (b > 0 && (itemLayers[j, i] & (1 << (int)b)) != 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	private static void MarkOccupied(byte[,] itemLayers, GridShape shape)
	{
		for (int i = shape.minY; i <= shape.maxY; i++)
		{
			for (int j = shape.minX; j <= shape.maxX; j++)
			{
				byte b = shape.Get(j, i);
				if (b > 0)
				{
					itemLayers[j, i] |= (byte)(1 << (int)b);
				}
			}
		}
	}

	private static int CellCount(GridShape shape)
	{
		int num = 0;
		for (int i = 0; i < shape.height; i++)
		{
			for (int j = 0; j < shape.width; j++)
			{
				if (shape.GetLocal(j, i) != 0)
				{
					num++;
				}
			}
		}
		return num;
	}
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameInventory : GameInventoryFunc, GraphNodeStorage
{
	public string identifier;

	public bool overrideLockInsert;

	public bool overrideLockRemove;

	public virtual bool SupportsGroupPlacement => false;

	public GraphNodeStorage parent { get; protected set; }

	public abstract List<GameItem> childItems { get; }

	public List<GraphNodeStorage> parents
	{
		get
		{
			List<GraphNodeStorage> list = new List<GraphNodeStorage>();
			if (parent != null)
			{
				list.Add(parent);
			}
			return list;
		}
	}

	public abstract List<GraphNodeStorage> children { get; }

	public GameItem GetParentItem()
	{
		List<GameItem> list = GraphUtils.FindParentsType<GameItem>(this);
		if (list.Count < 1)
		{
			return null;
		}
		return list[0];
	}

	public bool CheckAcceptParentShapeStateModifier(GameItem newItem, SlotMarker newSlot, GridShape newGridShape, TagSystem newState)
	{
		List<GameItem> list = GraphUtils.FindParentsType<GameItem>(this);
		if (list.Count < 1)
		{
			return true;
		}
		List<(GridShape, GridShape)> parentItemMods = new List<(GridShape, GridShape)>();
		List<(List<ItemStateModifier>, List<ItemStateModifier>)> parentItemStateMods = new List<(List<ItemStateModifier>, List<ItemStateModifier>)>();
		list.ForEach(delegate(GameItem pi)
		{
			parentItemMods.Add(newItem.GetParentShapeModifier(pi, newSlot, newGridShape, newState));
		});
		list.ForEach(delegate(GameItem pi)
		{
			parentItemStateMods.Add(newItem.GetParentStateModifier(pi, newSlot, newGridShape, newState));
		});
		foreach (var (gameItem, gridShape, gridShape2, addStateItem, addStateInv) in list.Zip(parentItemMods, (GameItem a, (GridShape, GridShape) b) => (a: a, b.Item1, b.Item2)).Zip(parentItemStateMods, ((GameItem a, GridShape, GridShape) a, (List<ItemStateModifier>, List<ItemStateModifier>) b) => (a.a, a.Item2, a.Item3, b.Item1, b.Item2)))
		{
			if (gridShape == null && gridShape2 == null)
			{
				continue;
			}
			var (tempShape, _, _, itemState) = gameItem.ComputeModifiedShapeState(gridShape, gridShape2, addStateItem, addStateInv, newItem);
			if (gameItem?.parentInventory != null)
			{
				SlotMarker slotMarker = gameItem.parentInventory.TryInventorySlot(gameItem, tempShape, itemState);
				if (slotMarker == null || slotMarker.numTransfer <= 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool CheckChangeParentShapeStateModifier(GameItem newItem, SlotMarker newSlot, GridShape newGridShape, TagSystem newState)
	{
		List<GameItem> list = GraphUtils.FindParentsType<GameItem>(this);
		if (list.Count < 1)
		{
			return true;
		}
		List<(GridShape, GridShape)> parentItemMods = new List<(GridShape, GridShape)>();
		List<(List<ItemStateModifier>, List<ItemStateModifier>)> parentItemStateMods = new List<(List<ItemStateModifier>, List<ItemStateModifier>)>();
		list.ForEach(delegate(GameItem pi)
		{
			parentItemMods.Add(newItem.GetParentShapeModifier(pi, newSlot, newGridShape, newState));
		});
		list.ForEach(delegate(GameItem pi)
		{
			parentItemStateMods.Add(newItem.GetParentStateModifier(pi, newSlot, newGridShape, newState));
		});
		foreach (var (gameItem, gridShape, gridShape2, addStateItem, addStateInv) in list.Zip(parentItemMods, (GameItem a, (GridShape, GridShape) b) => (a: a, b.Item1, b.Item2)).Zip(parentItemStateMods, ((GameItem a, GridShape, GridShape) a, (List<ItemStateModifier>, List<ItemStateModifier>) b) => (a.a, a.Item2, a.Item3, b.Item1, b.Item2)))
		{
			if (gridShape != null || gridShape2 != null)
			{
				var (tempShape, _, _, tempState) = gameItem.ComputeModifiedShapeState(gridShape, gridShape2, addStateItem, addStateInv, newItem);
				if (gameItem?.parentInventory != null && !gameItem.parentInventory.CheckInventorySlotStillValid(gameItem, tempShape, tempState))
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool CheckRemoveParentShapeModifier(GameItem item)
	{
		if (item.parentInventory != this)
		{
			return true;
		}
		List<GameItem> list = GraphUtils.FindParentsType<GameItem>(this);
		if (list.Count < 1)
		{
			return true;
		}
		if (!item.MayParentShapeModifier())
		{
			return true;
		}
		bool flag = true;
		foreach (GameItem item2 in list)
		{
			var (tempShape, _, _, itemState) = item2.ComputeModifiedShapeState(null, null, null, null, item);
			if (item2?.parentInventory != null)
			{
				bool num = flag;
				SlotMarker slotMarker = item2.parentInventory.TryInventorySlot(item2, tempShape, itemState);
				flag = num & (slotMarker != null && slotMarker.numTransfer > 0);
			}
		}
		return flag;
	}

	public abstract bool CheckInventorySlotStillValid(GameItem item, GridShape tempShape, TagSystem tempState);

	public SlotMarker TryInventorySlot(GameItem item, GridShape tempShape, TagSystem itemState = null)
	{
		if (itemState == null)
		{
			itemState = item.modifiedState;
		}
		return TryInventorySlot(item, item.unitCount, Vector2.negativeInfinity, tempShape, itemState);
	}

	public SlotMarker TryInventorySlot(GameItem item, int requestedNum, GridShape tempShape, TagSystem itemState = null)
	{
		if (itemState == null)
		{
			itemState = item.modifiedState;
		}
		return TryInventorySlot(item, requestedNum, Vector2.negativeInfinity, tempShape, itemState);
	}

	public abstract SlotMarker TryInventorySlot(GameItem item, int requestedNum, Vector2 itemGraphicsCenterPosition, GridShape tempShape, TagSystem itemState);

	public abstract SlotMarker GetItemInventorySlot(GameItem item);

	public abstract SlotMarker GetIncompleteItemInventorySlot(GameItem item);

	public abstract List<TreeNodeRender> HighlightSlot(SlotMarker slot, RenderHandler.BackgroundColor color, bool doReplace);

	public abstract void ClearHighlight();

	public virtual List<TreeNodeRender> HighlightWholeInventory(RenderHandler.BackgroundColor color)
	{
		return new List<TreeNodeRender>();
	}

	public virtual bool CheckShapePlacement(GameItem item, GridShape shape, List<GameItem> ignoredItems)
	{
		return false;
	}

	public abstract bool UncheckedAccept(GameItem item);

	public abstract bool Expel(GameItem item);

	public long GetTotalValue(List<GameCharacterItem> party = null, GameCharacterItem vendor = null)
	{
		return (from item in GraphUtils.FindAllChildrenType<GameItem>(this)
			select item.GetValue(party, vendor)).DefaultIfEmpty(0L).Sum();
	}

	public GameInventory UncheckedAcceptAll(List<GameItem> items)
	{
		items.ForEach(delegate(GameItem item)
		{
			UncheckedAccept(item);
		});
		return this;
	}

	public GameInventory TryAcceptAll(GameItem item)
	{
		GraphUtils.TryAcceptAll(this, item);
		return this;
	}

	public GameInventory ExpelAll()
	{
		childItems?.ForEach(delegate(GameItem i)
		{
			Expel(i);
		});
		return this;
	}

	public abstract bool IsAdjacent(SlotMarker slot, SlotMarker slot2);

	public abstract void Validate();

	public abstract bool MayHaveValidInventorySlot(GameItem item);

	public abstract SlotMarker TryFindOneValidInventorySlot(GameItem item);

	public abstract bool AddParent(GraphNodeStorage node);

	public abstract bool RemoveParent(GraphNodeStorage node);

	public bool IsInsertLocked()
	{
		if (overrideLockInsert)
		{
			return true;
		}
		return false;
	}

	public bool IsRemoveLocked()
	{
		if (overrideLockRemove)
		{
			return true;
		}
		return false;
	}

	public void LockInv()
	{
		overrideLockInsert = true;
		overrideLockRemove = true;
	}

	public void UnlockInv()
	{
		overrideLockInsert = false;
		overrideLockRemove = false;
	}
}

using System;

public class SlotMarker
{
	public GameItem item;

	public GameInventory inventory;

	public GridShape itemGridShape;

	public int index = -1;

	public GameItem targetItem;

	public int numTransfer;

	public int priority;

	public bool IsValid()
	{
		return numTransfer > 0;
	}

	public bool IsEquipmentSlot()
	{
		if (inventory == null)
		{
			return false;
		}
		return GraphUtils.FindParentType<GameItem>(inventory) is GameCharacterItem;
	}

	public static void StackItemUnchecked(GameItem item, GameItem targetItem, int amount = -1)
	{
		if (amount == -1)
		{
			amount = item.unitCount;
		}
		targetItem.unitCount += amount;
		item.unitCount -= amount;
		targetItem.Validate();
		item.Validate();
		item.onChangeAmountItemFunc?.Invoke(item);
		if (item.parentInventory != null)
		{
			SlotMarker incompleteItemInventorySlot = item.parentInventory.GetIncompleteItemInventorySlot(item);
			item.onSlotChangeAmountItemFunc?.Invoke(item, item.parentInventory, incompleteItemInventorySlot);
			item.parentInventory.onSlotChangeAmountItemFunc?.Invoke(item, item.parentInventory, incompleteItemInventorySlot);
		}
		targetItem.onChangeAmountItemFunc?.Invoke(targetItem);
		if (targetItem.parentInventory != null)
		{
			SlotMarker incompleteItemInventorySlot2 = targetItem.parentInventory.GetIncompleteItemInventorySlot(targetItem);
			targetItem.onSlotChangeAmountItemFunc?.Invoke(targetItem, targetItem.parentInventory, incompleteItemInventorySlot2);
			targetItem.parentInventory.onSlotChangeAmountItemFunc?.Invoke(targetItem, targetItem.parentInventory, incompleteItemInventorySlot2);
		}
		if (item.unitCount < 1)
		{
			item.parentInventory?.Expel(item);
		}
		OverlayHandler.current.UpdateInventoryStateUI();
	}

	public int TryAcceptOnce(int amount = -1)
	{
		if (item == null)
		{
			return 0;
		}
		if (amount < 0)
		{
			amount = numTransfer;
		}
		amount = Math.Min(amount, numTransfer);
		amount = Math.Min(amount, item.MaxNumRemove());
		if (amount <= 0)
		{
			return 0;
		}
		if (targetItem != null)
		{
			StackItemUnchecked(item, targetItem, amount);
			return amount;
		}
		if (inventory != null && itemGridShape != null)
		{
			if (amount < item.unitCount)
			{
				item = item.GetSplitItem(amount);
			}
			else
			{
				item.parentInventory?.Expel(item);
			}
			item.modifiedShape = new GridShapeBuilder(item.modifiedShape).SetTransform(itemGridShape);
			if (!inventory.UncheckedAccept(item))
			{
				return 0;
			}
			return amount;
		}
		return 0;
	}

	public int AcceptUnchecked()
	{
		if (item == null)
		{
			return 0;
		}
		if (inventory != null && itemGridShape != null)
		{
			item.modifiedShape = new GridShapeBuilder(item.modifiedShape).SetTransform(itemGridShape);
			if (!inventory.UncheckedAccept(item))
			{
				return 0;
			}
			return item.unitCount;
		}
		return 0;
	}
}

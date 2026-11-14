using System;
using System.Collections.Generic;
using UnityEngine;

public class MachineRecharger
{
	public const int CHARGE_RATE = 3;

	public static GameItem Recharger()
	{
		var (contentWindow, itemInventory) = CreateMachineInventoryWindow();
		itemInventory.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				return false;
			}
			return !PowerHelper.IsDisposablePowerSource(item) && item.IsTag("power_source_item");
		};
		GameItem machine2 = ItemDirectory.CreateEmptyItem().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_tool", "recharger_base");
		machine2.name = LocHelper.GetLocalizedItem("item_recharger_name");
		machine2.shortDescription = LocHelper.GetLocalizedItem("item_recharger_desc");
		machine2.EnableTag("IMPORTANT_TAG");
		machine2.unitValue = 150L;
		machine2.SetGameItemType("MACHINE");
		machine2.EnableTag("CONTAINER_TAG");
		machine2.EnableTag("manufacture");
		AudioHelper.InitSmallMachine(machine2);
		machine2.onCycleEndSlotItemFunc = delegate
		{
			if (PlayerStore.Instance.isPowerOn)
			{
				foreach (GameItem childItem in itemInventory.childItems)
				{
					if (childItem.IsTag("power_source_item"))
					{
						if (NetworkUpgrade.IsCommercialPowerUnlocked())
						{
							PowerHelper.ChargeBattery(childItem, 6);
						}
						else
						{
							PowerHelper.ChargeBattery(childItem, 3);
						}
					}
				}
			}
			machine2.Validate();
		};
		itemInventory.onSlotAddItemEndFunc = delegate
		{
			machine2.Validate();
		};
		itemInventory.onSlotRemoveItemEndFunc = delegate
		{
			machine2.Validate();
		};
		GameInventory gameInventory = itemInventory;
		gameInventory.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameInventory.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			AudioManager.Instance.Play3("battery_insert");
		});
		GameInventory gameInventory2 = itemInventory;
		gameInventory2.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameInventory2.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			AudioManager.Instance.Play3("battery_removed");
		});
		machine2.getItemSpriteModifierForItemFunc = delegate
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			int num = 0;
			foreach (GameItem childItem2 in itemInventory.childItems)
			{
				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas(childItem2.spriteAtlasPath, childItem2.spritePath), new Vector2(num * 16, 0f), 0, isFlipped: false, 0, haveMask: false, Color.white));
				num++;
			}
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_tool", "recharger_top"), new Vector2(0f, 0f), 0, isFlipped: false, 0, haveMask: false, Color.white));
			if (!PlayerStore.Instance.isPowerOn)
			{
				return list;
			}
			num = 0;
			foreach (GameItem childItem3 in itemInventory.childItems)
			{
				if (PowerHelper.IsPowerSourceFull(childItem3))
				{
					list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_tool", "recharger_g"), new Vector2(7 + num * 16, -8f), 0, isFlipped: false, 0, haveMask: false, Color.white));
				}
				else
				{
					list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_tool", "recharger_o"), new Vector2(7 + num * 16, -8f), 0, isFlipped: false, 0, haveMask: false, Color.white));
				}
				num++;
			}
			return list;
		};
		return machine2;
	}

	public static (PixelWindow, GameInventory) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true)
	{
		GameGridInventory gameGridInventory = new GameGridInventory(4, 2);
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_recharger_label_battery"), gameGridInventory, "batteries")), gameGridInventory);
	}
}

public class RatTrap
{
	public const string MOUSE_TRAP_TAG = "MOUSE_TRAP_TAG";

	public static GameItem CreateMouseTrap()
	{
		(PixelWindow, GameSlotInventory) tuple = CreateMouseTrapWindow();
		PixelWindow item2 = tuple.Item1;
		GameSlotInventory trapSlot = tuple.Item2;
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(item2).SetSpriteAndShape("Items/items_husbandry", "mouse_trap");
		machine.name = LocHelper.GetLocalizedItem("husbandry_mouse_trap_name");
		machine.flavorText = LocHelper.GetLocalizedItem("husbandry_mouse_trap_flavor");
		machine.unitValue = 50L;
		machine.EnableTag("MOUSE_TRAP_TAG");
		machine.EnableTag("CONTAINER_TAG");
		machine.EnableTag("NESTABLE_DEVICE_TAG");
		AudioHelper.InitPlasticMachinery(machine);
		trapSlot.SetBackgroundFadeSprite("Items/items_food", "cheese");
		trapSlot.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
		{
			if (!GeneralHelper.IsItemOwned(machine))
			{
				return false;
			}
			if (!GeneralHelper.IsItemOwned(item))
			{
				return false;
			}
			if (item.IsTag("BACKPACK_TAG") || item.IsTag("CONTAINER_TAG"))
			{
				return false;
			}
			if (item.IsGameItemType("FOOD") && item.identifier.Equals("processed_cheese"))
			{
				return true;
			}
			return item.IsTag("ANIMAL_ITEM_TAG") ? true : false;
		};
		trapSlot.onSlotAddItemEndFunc = delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			if (addedItem.identifier.Equals("processed_cheese"))
			{
				machine.SetSpriteAndShape("Items/items_husbandry", "mouse_trap_cheese");
			}
			else if (addedItem.IsTag("ANIMAL_ITEM_TAG") && addedItem.GetTagReadonly("ANIMAL_SPECIES_TAG").GetString().Equals("ANIMAL_RAT"))
			{
				if (addedItem.GetTagReadonly("ANIMAL_SEXE_TAG").GetString().Equals("MALE"))
				{
					machine.SetSpriteAndShape("Items/items_husbandry", "mouse_trap_m");
				}
				else
				{
					machine.SetSpriteAndShape("Items/items_husbandry", "mouse_trap_f");
				}
			}
		};
		trapSlot.onSlotRemoveItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_husbandry", "mouse_trap");
		};
		machine.onCycleEndSlotItemFunc = delegate
		{
			GameItem childItem = trapSlot.childItem;
			bool flag = childItem?.identifier.Equals("processed_cheese") ?? false;
			bool flag2 = childItem == null;
			if (flag || flag2)
			{
				if (RNG.Roll((!flag) ? 1 : 50))
				{
					if (flag)
					{
						childItem.Destroy();
					}
					string sex = (RNG.Roll(50) ? "MALE" : "FEMALE");
					int randomInt = RNG.GetRandomInt(3, 7);
					GameItem item3 = PreBuiltItemHelper.CreateRat(sex, randomInt);
					GraphUtils.TryAcceptAll(trapSlot, item3);
				}
				else if (flag && RNG.Roll(25))
				{
					childItem.Destroy();
				}
			}
		};
		machine.onLoaded = delegate(GameItem item)
		{
			item.EnableTag("NESTABLE_DEVICE_TAG");
		};
		return machine;
	}

	public static (PixelWindow, GameSlotInventory) CreateMouseTrapWindow()
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		gameSlotInventory.EnableTag("MOUSE_TRAP_TAG");
		return (MachineHelper.CreateMachineryWindow(true, MachineHelper.Panel(null, gameSlotInventory, "trap")), gameSlotInventory);
	}

	public static GameSlotInventory GetTrapSlot(GameItem trap)
	{
		return MachineHelper.GetBodyStorage(trap, 0) as GameSlotInventory;
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		item.IsTag("MOUSE_TRAP_TAG");
	}
}

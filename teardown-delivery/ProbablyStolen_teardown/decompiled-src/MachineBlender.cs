using System.Linq;

public class MachineBlender
{
	public static GameItem Blender()
	{
		var (contentWindow, moduleGrid, inputGrid, containerSlot, machineryButton) = CreateMachineInventoryWindow(8, 8);
		SetupInputGrid(inputGrid);
		SetupWaterSlot(containerSlot);
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_tool2", "blender");
		containerSlot.onSlotAddItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool2", "blender_b");
		};
		containerSlot.onSlotRemoveItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool2", "blender");
		};
		SetupChemModuleGrid(moduleGrid, machine);
		AudioHelper.InitMetalMachine(machine);
		machine.name = LocHelper.GetLocalizedItem("item_blender_name");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_blender_desc");
		machine.flavorText = "";
		machine.unitValue = 200L;
		machine.EnableTag("CONTAINER_TAG");
		machine.SetGameItemType("MACHINE");
		machineryButton.SetCallback(delegate
		{
			if (containerSlot.childItem == null || WaterHelper.IsFull(containerSlot.childItem))
			{
				return;
			}
			AudioManager.Instance.Play2("blender");
			foreach (GameItem item in inputGrid.childItems.ToList())
			{
				if (containerSlot.childItem == null || WaterHelper.IsFull(containerSlot.childItem))
				{
					break;
				}
				if (item.IsTag("BLENDABLE_TAG"))
				{
					int loss = GetLoss(machine);
					ChemicalHelper.BlendInto(item, containerSlot.childItem, loss);
				}
			}
		});
		return machine;
	}

	private static void SetupWaterSlot(GameSlotInventory waterSlot)
	{
		waterSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && item.IsTag("LIQUID_CONTAINER_TAG");
		waterSlot.SetBackgroundFadeSprite("Items/water_container4", "xl");
	}

	private static void SetupInputGrid(GameGridInventory inputGrid)
	{
		inputGrid.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => ContainerHelper.CanContainerStoredItem(item, inv);
	}

	public static void SetupChemModuleGrid(GameGridInventory moduleGrid, GameItem machine)
	{
		string[] tags = new string[1] { "MODULE_TYPE_CHEM" };
		ContainerHelper.AllowOnlyTaggedItemsOr(moduleGrid, tags, ownedOnly: true);
		moduleGrid.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		moduleGrid.onSlotAddItemEndFunc = delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			if (addedItem.IsTag("MODULE_TAG"))
			{
				int count2 = machine.GetTagReadonly("CHEMICAL_MACHINE_UPGRADE_COUNT").GetInt();
				ModuleHelper.SetModuleStuck(addedItem);
				machine.ModifyTag("CHEMICAL_MACHINE_UPGRADE_COUNT", delegate(TagState state)
				{
					state.Enable().SetInt(count2 + 1);
				});
			}
		};
		moduleGrid.onSlotRemoveItemEndFunc = delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			if (addedItem.IsTag("MODULE_TAG"))
			{
				int count = machine.GetTagReadonly("CHEMICAL_MACHINE_UPGRADE_COUNT").GetInt();
				machine.ModifyTag("CHEMICAL_MACHINE_UPGRADE_COUNT", delegate(TagState state)
				{
					state.Enable().SetInt(count - 1);
				});
			}
		};
	}

	public static (PixelWindow, GameGridInventory, GameGridInventory, GameSlotInventory, MachineryButton) CreateMachineInventoryWindow(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		GameGridInventory gameGridInventory = new GameGridInventory(4, 4);
		GameGridInventory gameGridInventory2 = new GameGridInventory(4, 4);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		MachineryButton machineryButton = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_blender_btn_start"));
		gameGridInventory2.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_blender_label_module"), gameGridInventory, "module"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_blender_label_input"), gameGridInventory2, "input"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_blender_label_container"), gameSlotInventory, "container"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_blender_label_control"), machineryButton, "control")), gameGridInventory, gameGridInventory2, gameSlotInventory, machineryButton);
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("blender"))
		{
			int loss = GetLoss(item);
			if (loss > 0)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_blender_content_lost", loss), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			}
			else
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_blender_content_lost", 0), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
		}
	}

	public static int GetUpgradeCount(GameItem item)
	{
		if (item.contentWindow == null || item.contentWindow.child == null)
		{
			return 0;
		}
		return ((GameInventory)MachineHelper.GetBodyStorage(item, 0)).childItems.FindAll((GameItem i) => i.IsTag("MODULE_TAG")).Count;
	}

	public static int GetLoss(GameItem item)
	{
		return GetUpgradeCount(item) switch
		{
			0 => 35, 
			1 => 20, 
			2 => 10, 
			3 => 5, 
			_ => 0, 
		};
	}
}

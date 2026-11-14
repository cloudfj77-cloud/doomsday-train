using UnityEngine;

public class MachineProjector
{
	public const string IS_PROJECTOR_ACTIVE = "IS_PROJECTOR_ACTIVE";

	public static GameItem Projector()
	{
		(PixelWindow, GameSlotInventory, GameInventory, GameSlotInventory) tuple = CreateMachineInventoryWindow();
		PixelWindow item2 = tuple.Item1;
		GameSlotInventory itemInventory = tuple.Item2;
		GameInventory itemInventoryModule = tuple.Item3;
		GameSlotInventory item3 = tuple.Item4;
		item3.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => item.identifier.Equals("postit");
		item3.SetBackgroundFadeSprite("Items/items_tool", "postit");
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(item2).SetSpriteAndShape("Items/items_tool2", "projector");
		machine.name = LocHelper.GetLocalizedItem("item_mirage_projector_name");
		machine.flavorText = LocHelper.GetLocalizedItem("item_mirage_projector_flavor");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_mirage_projector_desc");
		machine.unitValue = 400L;
		MachineHelper.SetupBatterySlot(itemInventory, machine);
		machine.EnableTag("STANDARD_MACHINE_TAG");
		machine.SetGameItemType("MACHINE");
		machine.ModifyTag("MACHINE_PERFORMANCE_RATIO_INT", delegate(TagState state)
		{
			state.Enable().SetInt(4);
		});
		machine.ModifyTag("MACHINE_QUALITY_RATIO_INT", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
		MachineryHelper.InitMachinery(machine, 16);
		AudioHelper.InitPlasticMachinery(machine);
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(itemInventoryModule, tags, ownedOnly: true);
		itemInventoryModule.onSlotAddItemEndFunc = delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			ModuleEffectHelper.OnAddedModuleToGrid(addedItem, machine, itemInventoryModule.childItems);
			if (addedItem.IsTag("MODULE_TAG"))
			{
				ModuleHelper.ComputeModuleEffect(itemInventoryModule, addedItem, machine);
				ModuleHelper.ApplyBasicModuleEffect(itemInventoryModule, addedItem, machine);
			}
			MachineryHelper.UpdatePowerUsageProcessTypeMachine(machine);
		};
		itemInventoryModule.onSlotRemoveItemEndFunc = delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			ModuleEffectHelper.OnRemovedModuleFromGrid(addedItem, machine, itemInventoryModule.childItems);
			if (addedItem.IsTag("MODULE_TAG"))
			{
				ModuleHelper.ComputeModuleEffect(itemInventoryModule, addedItem, machine);
				ModuleHelper.ApplyBasicModuleEffect(itemInventoryModule, addedItem, machine);
			}
			MachineryHelper.UpdatePowerUsageProcessTypeMachine(machine);
		};
		machine.onCycleEndSlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			int machinePowerUsage = MachineryHelper.GetMachinePowerUsage(item);
			if (itemInventory.childItems.Count != 0 && PowerHelper.DrawPowerSource(itemInventory.childItem, machinePowerUsage))
			{
				item.EnableTag("IS_PROJECTOR_ACTIVE");
				PlayerStore.Instance.projectorAttractivenessBonus = GetAttractivenessBoost(item);
				MachineHelper.OnMachineActioned(item, itemInventoryModule);
			}
			else
			{
				item.DisableTag("IS_PROJECTOR_ACTIVE");
			}
		};
		return machine;
	}

	public static (PixelWindow, GameSlotInventory, GameInventory, GameSlotInventory) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 20)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory("10000010001000001110000010001000001", 7);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_projector_label_module"), gameGridInventory, "module"), MachineHelper.NotePanel(gameSlotInventory2)), gameSlotInventory, gameGridInventory, gameSlotInventory2);
	}

	public static GameItem CreateNote()
	{
		GameItem gameItem = DirectoryMaster.Item("postit");
		gameItem.name = LocHelper.GetLocalizedItem("item_projector_instruction_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_projector_instruction_flavor");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 0L;
		return gameItem;
	}

	public static int GetAttractivenessBoost(GameItem item)
	{
		int num = Mathf.Max(1, item.GetTagReadonly("MACHINE_PERFORMANCE_RATIO_INT").GetInt());
		int num2 = Mathf.Max(1, item.GetTagReadonly("MACHINE_QUALITY_RATIO_INT").GetInt());
		int num3 = MachineryHelper.GetCurrentQualityBonus(item) / num2 + MachineryHelper.GetCurrentPerformanceBonus(item) / num;
		int num4 = 100 + num3;
		if (num4 < 0)
		{
			num4 = 0;
		}
		return num4;
	}

	public static void CreateProjectorTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("mirage_projector") && !item.IsTag("BROKEN_MACHINE_TAG"))
		{
			if (item.IsTag("IS_PROJECTOR_ACTIVE"))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_projector_attractiveness_active", GetAttractivenessBoost(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			else
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_projector_attractiveness_inactive", GetAttractivenessBoost(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
		}
	}
}

using TMPro;

public class MachineChemFinisher
{
	public const string SELECTED_CHEM_ID_TAG = "SELECTED_CHEM_ID_TAG";

	public static GameItem ChemFinisher()
	{
		(PixelWindow, GameGridInventory, GameSlotInventory, GameGridInventory, MachineryButton, MachineryButton, MachineryText, MachineryText, MachineryStack) tuple = CreateMachineInventoryWindow(8, 8);
		PixelWindow item2 = tuple.Item1;
		GameGridInventory item3 = tuple.Item2;
		GameSlotInventory containerSlot = tuple.Item3;
		GameGridInventory outputGrid = tuple.Item4;
		MachineryButton item4 = tuple.Item5;
		MachineryButton item5 = tuple.Item6;
		MachineryText targetLabel = tuple.Item7;
		MachineryText statusLabel = tuple.Rest.Item1;
		MachineryStack controlGrid = tuple.Rest.Item2;
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(item2).SetSpriteAndShape("Items/items_tool2", "chem_finisher");
		containerSlot.onSlotAddItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool2", "chem_finisher_b");
		};
		containerSlot.onSlotRemoveItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool2", "chem_finisher");
		};
		SetupWaterSlot(containerSlot);
		MachineBlender.SetupChemModuleGrid(item3, machine);
		machine.name = LocHelper.GetLocalizedItem("item_chem_finisher_name");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_chem_finisher_desc");
		machine.unitValue = 300L;
		machine.SetGameItemType("MACHINE");
		machine.EnableTag("CONTAINER_TAG");
		AudioHelper.InitMetalMachine(machine);
		item4.SetCallback(delegate
		{
			ChemFinisherUIManager.Instance.OpenUI(machine);
		});
		machine.onOneSecondItemFunc = delegate
		{
			if (!machine.IsTag("SELECTED_CHEM_ID_TAG"))
			{
				targetLabel.SetText(LocHelper.GetLocalizedMechanic("mech_chem_finisher_no_target"));
				statusLabel.SetText(LocHelper.GetLocalizedMechanic("mech_turbo_booster_status", LocHelper.GetLocalizedMechanic("mech_chem_finisher_status_no_target")));
			}
			else
			{
				ChemFinisherRecipe recipeById2 = ChemFinisherRecipe.GetRecipeById(machine.GetTagReadonly("SELECTED_CHEM_ID_TAG").GetString());
				targetLabel.SetText(recipeById2.displayName);
				controlGrid.Validate();
				if (CanOutput(machine, containerSlot))
				{
					statusLabel.SetText(LocHelper.GetLocalizedMechanic("mech_turbo_booster_status", LocHelper.GetLocalizedMechanic("mech_turbo_booster_status_ready")));
				}
				else if (containerSlot.childItem != null && recipeById2.isRequireDry && WaterHelper.IsWet(containerSlot.childItem))
				{
					statusLabel.SetText(LocHelper.GetLocalizedMechanic("mech_turbo_booster_status", LocHelper.GetLocalizedMechanic("mech_chem_finisher_status_wet_ingredient")));
				}
				else
				{
					statusLabel.SetText(LocHelper.GetLocalizedMechanic("mech_turbo_booster_status", LocHelper.GetLocalizedMechanic("mech_chem_finisher_status_not_enough_reagent")));
				}
			}
		};
		item5.SetCallback(delegate
		{
			if (CanOutput(machine, containerSlot))
			{
				string @string = machine.GetTagReadonly("SELECTED_CHEM_ID_TAG").GetString();
				ChemFinisherRecipe recipeById = ChemFinisherRecipe.GetRecipeById(@string);
				GameItem gameItem = DirectoryMaster.Item(@string);
				ChemicalHelper.RemoveAllLiquid(gameItem);
				ChemicalHelper.TransferLiquidToChem(containerSlot.childItem, gameItem, recipeById.requiredVolume);
				IntroduceImpurity(gameItem, GetLoss(machine));
				GraphUtils.TryAcceptAll(outputGrid, gameItem);
				ChemicalProductHelper.GradeProduct(gameItem);
				AchievementTracker.Instance.CheckLabPure(gameItem);
			}
		});
		outputGrid.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => ContainerHelper.CanContainerStoredItem(item, inv) ? true : false;
		return machine;
	}

	public static void IntroduceImpurity(GameItem chem, int percentAmount)
	{
		int totalVolume = ChemicalHelper.GetTotalVolume(chem);
		ChemicalHelper.RemoveVolume(chem, percentAmount);
		int volume = (int)((long)totalVolume * (long)percentAmount / 100);
		ChemicalHelper.AddChemContentUL(chem, "physical_contaminant", volume);
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("chem_finisher"))
		{
			int loss = GetLoss(item);
			if (loss > 0)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_chem_finisher_contaminant", loss), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			}
			else
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_chem_finisher_contaminant", 0), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
		}
	}

	public static int GetLoss(GameItem item)
	{
		return MachineBlender.GetUpgradeCount(item) switch
		{
			0 => 12, 
			1 => 6, 
			2 => 3, 
			3 => 1, 
			_ => 0, 
		};
	}

	public static bool CanOutput(GameItem machine, GameSlotInventory containerSlot)
	{
		if (containerSlot.childItem == null)
		{
			return false;
		}
		if (!machine.IsTag("SELECTED_CHEM_ID_TAG"))
		{
			return false;
		}
		if (WaterHelper.IsEmpty(containerSlot.childItem))
		{
			return false;
		}
		string @string = machine.GetTagReadonly("SELECTED_CHEM_ID_TAG").GetString();
		foreach (ChemFinisherRecipe chemFinisherRecipe in ChemFinisherRecipe.chemFinisherRecipes)
		{
			if (@string == chemFinisherRecipe.identifier && chemFinisherRecipe.CanOutput(containerSlot.childItem))
			{
				return true;
			}
		}
		return false;
	}

	private static void SetupWaterSlot(GameSlotInventory waterSlot)
	{
		waterSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && item.IsTag("LIQUID_CONTAINER_TAG");
		waterSlot.SetBackgroundFadeSprite("Items/water_container4", "xl");
		waterSlot.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 5;
	}

	public static (PixelWindow, GameGridInventory, GameSlotInventory, GameGridInventory, MachineryButton, MachineryButton, MachineryText, MachineryText, MachineryStack) CreateMachineInventoryWindow(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 5)
	{
		GameGridInventory gameGridInventory = new GameGridInventory(4, 4);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory2 = new GameGridInventory(6, 4);
		MachineryButton machineryButton = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_chem_finisher_btn_configure"));
		MachineryText machineryText = new MachineryText(LocHelper.GetLocalizedMechanic("mech_chem_finisher_no_target"), 6, HorizontalAlignmentOptions.Center, uppercase: false);
		MachineryText machineryText2 = new MachineryText(LocHelper.GetLocalizedMechanic("mech_chem_finisher_status_not_enough_reagent"), 6, HorizontalAlignmentOptions.Center, uppercase: false);
		MachineryButton machineryButton2 = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_chem_finisher_btn_output"));
		MachineryStack machineryStack = MachineryStack.Vertical(machineryButton, machineryText, machineryText2, machineryButton2);
		machineryButton.SetSize(75);
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_chem_finisher_label_module"), gameGridInventory, "module"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_chem_finisher_label_container"), gameSlotInventory, "container"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_chem_finisher_label_output"), gameGridInventory2, "output"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_chem_finisher_label_control"), machineryStack, "control")), gameGridInventory, gameSlotInventory, gameGridInventory2, machineryButton, machineryButton2, machineryText, machineryText2, machineryStack);
	}
}

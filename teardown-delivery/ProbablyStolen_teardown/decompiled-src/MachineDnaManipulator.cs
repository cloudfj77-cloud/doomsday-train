using System;
using UnityEngine;

public class MachineDnaManipulator
{
	public class OperationPlan
	{
		public string operation;

		public PlantTrait trait;

		public bool isStacking;

		public int successChance;

		public int progress;

		public string blockedReason = "";
	}

	public const int BASE_PROCESSING_SPEED = 10;

	public const int BASE_POWER_USAGE = 12;

	public const string DNA_MANIPULATOR_TAG = "DNA_MANIPULATOR_TAG";

	public const string OPERATION_STRING = "DNA_OPERATION_STRING";

	public const string OPERATION_TRAIT_STRING = "DNA_OPERATION_TRAIT_STRING";

	public const string OPERATION_SUCCESS_INT = "DNA_OPERATION_SUCCESS_INT";

	public const string OPERATION_STACKING_BOOL = "DNA_OPERATION_STACKING_BOOL";

	public const string OPERATION_APPLY = "apply";

	public const string OPERATION_EXTRACT = "extract";

	public const string KEEP_DISK_BOOL = "DNA_MANIPULATOR_KEEP_DISK_BOOL";

	public const string STABILIZER_MODULE_ID = "dna_manipulator_module_stabilizer";

	public const string POSITIVE_FILTER_MODULE_ID = "dna_manipulator_module_positive_filter";

	public const string NEGATIVE_FILTER_MODULE_ID = "dna_manipulator_module_negative_filter";

	public const string PRECISION_MODULE_ID = "dna_manipulator_module_precision";

	public const string PRECISION_TRAIT_STRING = "DNA_PRECISION_TRAIT_STRING";

	public const string ATLAS = "Items/items_hydroponic_plant";

	public const string SPRITE_IDLE = "dna_manipulator";

	public const string SPRITE_WORKING = "dna_manipulator_working";

	public const string DISK_SPRITE_ATLAS = "Items/items_hydroponic_plant";

	public const string DISK_SPRITE_BLANK = "dna_disk_black_empty";

	public const string DISK_SPRITE_WRITTEN = "dna_disk_black_full";

	public const string DISK_SPRITE_POSITIVE = "dna_disk_green_full";

	public const string DISK_SPRITE_NEGATIVE = "dna_disk_red_full";

	public const string DISK_SPRITE_SPECIAL = "dna_disk_blue_full";

	public const string DISK_SPRITE_CHEMICAL = "dna_disk_purple_full";

	private const int COL_BATTERY = 0;

	private const int COL_MODULE = 1;

	private const int COL_INPUT_DISK = 2;

	private const int COL_SEED = 3;

	private const int COL_EJECT = 6;

	private const int COL_OUTPUT_DISK = 4;

	private const int COL_NOTE = 5;

	public static string DiskSpriteFor(PlantTrait trait)
	{
		if (trait == null)
		{
			return "dna_disk_black_empty";
		}
		if (trait.IsChemical)
		{
			return "dna_disk_purple_full";
		}
		if (trait.IsSpecial)
		{
			return "dna_disk_blue_full";
		}
		if (!trait.IsNegative)
		{
			return "dna_disk_green_full";
		}
		return "dna_disk_red_full";
	}

	public static GameItem DnaManipulator()
	{
		(PixelWindow, GameSlotInventory, GameGridInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameGridInventory) tuple = CreateMachineInventoryWindow();
		PixelWindow item2 = tuple.Item1;
		GameSlotInventory batterySlot = tuple.Item2;
		GameGridInventory moduleGrid = tuple.Item3;
		GameSlotInventory item3 = tuple.Item4;
		GameSlotInventory item4 = tuple.Item5;
		GameSlotInventory item5 = tuple.Item6;
		GameSlotInventory item6 = tuple.Item7;
		GameGridInventory item7 = tuple.Rest.Item1;
		SetupInputDiskSlot(item3, item4);
		SetupSeedSlot(item4, item3, item5);
		SetupOutputDiskSlot(item5, item4);
		MachineHelper.SetupManualSlot(item6);
		SetupEjectGrid(item7);
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetContentWindow(item2).SetSpriteAndShape("Items/items_hydroponic_plant", "dna_manipulator");
		gameItem.name = PlantGeneticsText.ManipulatorName();
		gameItem.shortDescription = PlantGeneticsText.ManipulatorDescription();
		gameItem.flavorText = "";
		gameItem.unitValue = 1000L;
		gameItem.SetGameItemType("MACHINE");
		gameItem.EnableTag("CONTAINER_TAG");
		gameItem.EnableTag("manufacture");
		gameItem.EnableTag("DNA_MANIPULATOR_TAG");
		gameItem.EnableTag("STANDARD_MACHINE_TAG");
		AudioHelper.InitMetalMachine(gameItem);
		MachineHelper.SetupBatterySlot(batterySlot, gameItem);
		MachineryHelper.InitMachinery(gameItem, 12);
		MachineProgressHelper.InitProgressTypeMachine(gameItem, 10, useDefaultTooltip: false);
		gameItem.ModifyTag("DNA_MANIPULATOR_KEEP_DISK_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(val: false);
		});
		gameItem.mayToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.onToggleSlotItemFunc = delegate(GameItem self, GameInventory _, SlotMarker _)
		{
			self.ModifyTag("DNA_MANIPULATOR_KEEP_DISK_BOOL", delegate(TagState state)
			{
				state.Enable().SetBool(IsEjectEnabled(self));
			});
			AudioManager.Instance.Play3("click_vintage");
			self.Validate();
		};
		gameItem.ModifyTag("MACHINE_PERFORMANCE_RATIO_INT", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
		gameItem.ModifyTag("MACHINE_QUALITY_RATIO_INT", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
		gameItem.ModifyTag("DNA_OPERATION_STRING", delegate(TagState state)
		{
			state.Enable().SetString("");
		});
		MachineHelper.SetupModuleBay(moduleGrid, gameItem, null, new string[2] { "MODULE_TYPE_UNIVERSAL", "MODULE_TYPE_DNA_MANIPULATOR" });
		gameItem.onCycleEndSlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			string machineState = MachineProgressHelper.GetMachineState(item);
			if (machineState.Equals("STATE_READY"))
			{
				OperationPlan operationPlan = PlanOperation(item);
				if (operationPlan != null && operationPlan.blockedReason.Length == 0 && MachineHelper.TryDrawCyclePower(item, batterySlot))
				{
					StartOperation(item, operationPlan);
					WorkOneCycle(item, moduleGrid);
				}
			}
			else if (machineState.Equals("STATE_WORKING") && MachineHelper.TryDrawCyclePower(item, batterySlot))
			{
				WorkOneCycle(item, moduleGrid);
			}
			UpdateSprite(item);
		};
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, new Action<GameItem>(UpdateSprite));
		return gameItem;
	}

	public static void UpdateSprite(GameItem machine)
	{
		bool flag = MachineProgressHelper.GetMachineState(machine).Equals("STATE_WORKING") && MachineHelper.CanPower(machine);
		machine.SetSpriteAndShape("Items/items_hydroponic_plant", flag ? "dna_manipulator_working" : "dna_manipulator");
		machine.Validate();
	}

	private static void WorkOneCycle(GameItem machine, GameGridInventory moduleGrid)
	{
		MachineProgressHelper.ContinueProgressTypeMachine(machine);
		MachineHelper.OnMachineActioned(machine, moduleGrid);
		if (MachineProgressHelper.IsProgressTypeMachineFinished(machine))
		{
			FinishOperation(machine);
		}
	}

	public static OperationPlan PlanOperation(GameItem machine)
	{
		GameItem childItem = GetSeedSlot(machine).childItem;
		if (childItem == null)
		{
			return null;
		}
		GameItem childItem2 = GetInputDiskSlot(machine).childItem;
		GameItem childItem3 = GetOutputDiskSlot(machine).childItem;
		int effectiveComplexity = PlantGeneticsHelper.GetEffectiveComplexity(childItem);
		OperationPlan operationPlan = new OperationPlan
		{
			successChance = PlantGeneticsHelper.GetSuccessChance(effectiveComplexity, ModuleHelper.GetAjustedQuality(machine)),
			progress = PlantGeneticsHelper.GetOperationProgress(effectiveComplexity)
		};
		if (PlantGeneticsHelper.IsWrittenDisk(childItem2))
		{
			operationPlan.operation = "apply";
			operationPlan.trait = PlantGeneticsHelper.GetDiskTrait(childItem2);
			operationPlan.isStacking = PlantGeneticsHelper.GetTraitRank(childItem, operationPlan.trait.id) > 0;
			if (!PlantGeneticsHelper.CanApplyTrait(childItem, operationPlan.trait, out var reason))
			{
				operationPlan.blockedReason = reason;
			}
			return operationPlan;
		}
		if (PlantGeneticsHelper.IsBlankDisk(childItem3))
		{
			operationPlan.operation = "extract";
			PlantTrait precisionTrait = GetPrecisionTrait(machine);
			if (precisionTrait != null)
			{
				operationPlan.trait = precisionTrait;
			}
			if (!PlantGeneticsHelper.HasExtractableTrait(childItem, GetExtractionFilter(machine)))
			{
				operationPlan.blockedReason = ((precisionTrait != null) ? PlantGeneticsText.BlockPrecisionMissing(precisionTrait.displayName) : ((HasModule(machine, "dna_manipulator_module_positive_filter") && HasModule(machine, "dna_manipulator_module_negative_filter")) ? PlantGeneticsText.BlockBothFilters() : (HasModule(machine, "dna_manipulator_module_positive_filter") ? PlantGeneticsText.BlockNoPositiveTrait() : (HasModule(machine, "dna_manipulator_module_negative_filter") ? PlantGeneticsText.BlockNoNegativeTrait() : PlantGeneticsText.BlockNoTrait()))));
			}
			return operationPlan;
		}
		if (PlantGeneticsHelper.IsWrittenDisk(childItem3))
		{
			operationPlan.operation = "extract";
			operationPlan.trait = PlantGeneticsHelper.GetDiskTrait(childItem3);
			if (!PlantGeneticsHelper.CanExtractOntoDisk(childItem, childItem3, out var reason2))
			{
				operationPlan.blockedReason = reason2;
			}
			return operationPlan;
		}
		return null;
	}

	private static void StartOperation(GameItem machine, OperationPlan plan)
	{
		machine.ModifyTag("DNA_OPERATION_STRING", delegate(TagState state)
		{
			state.Enable().SetString(plan.operation);
		});
		machine.ModifyTag("DNA_OPERATION_TRAIT_STRING", delegate(TagState state)
		{
			state.Enable().SetString((plan.trait != null) ? plan.trait.id : "");
		});
		machine.ModifyTag("DNA_OPERATION_SUCCESS_INT", delegate(TagState state)
		{
			state.Enable().SetInt(plan.successChance);
		});
		machine.ModifyTag("DNA_OPERATION_STACKING_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(plan.isStacking);
		});
		MachineProgressHelper.StartProgressTypeMachine(machine, plan.progress);
		LockIfPresent(GetSeedSlot(machine).childItem);
		LockIfPresent(GetInputDiskSlot(machine).childItem);
		LockIfPresent(GetOutputDiskSlot(machine).childItem);
		MachineHelper.LockModuleInv(machine);
		AudioManager.Instance.Play3("module_insert");
		machine.Validate();
	}

	private static void FinishOperation(GameItem machine)
	{
		string @string = machine.GetTagReadonly("DNA_OPERATION_STRING").GetString();
		bool flag = RNG.Roll(machine.GetTagReadonly("DNA_OPERATION_SUCCESS_INT").GetInt());
		GameItem childItem = GetSeedSlot(machine).childItem;
		GameItem childItem2 = GetInputDiskSlot(machine).childItem;
		GameItem childItem3 = GetOutputDiskSlot(machine).childItem;
		UnlockIfPresent(childItem);
		UnlockIfPresent(childItem2);
		UnlockIfPresent(childItem3);
		MachineHelper.UnlockModuleInv(machine);
		if (childItem != null)
		{
			if (@string.Equals("apply"))
			{
				FinishApply(machine, childItem, childItem2, flag);
			}
			if (@string.Equals("extract"))
			{
				FinishExtract(machine, childItem, childItem3, flag);
			}
		}
		EjectDisk(machine, @string.Equals("apply") ? GetInputDiskSlot(machine) : GetOutputDiskSlot(machine));
		MachineProgressHelper.FinishProgressTypeMachine(machine);
		machine.ModifyTag("DNA_OPERATION_STRING", delegate(TagState state)
		{
			state.Enable().SetString("");
		});
		machine.ModifyTag("DNA_OPERATION_TRAIT_STRING", delegate(TagState state)
		{
			state.Enable().SetString("");
		});
		AudioManager.Instance.Play3(flag ? "scanner_activate" : "module_remove");
		machine.Validate();
	}

	private static void FinishApply(GameItem machine, GameItem seed, GameItem disk, bool success)
	{
		PlantTrait plantTrait = PlantTraitRegistry.Get(machine.GetTagReadonly("DNA_OPERATION_TRAIT_STRING").GetString());
		if (plantTrait != null)
		{
			machine.GetTagReadonly("DNA_OPERATION_STACKING_BOOL").GetBool();
			if (success && PlantGeneticsHelper.CanApplyTrait(seed, plantTrait, out var _))
			{
				PlantGeneticsHelper.ApplyTrait(seed, plantTrait, (disk == null) ? 1 : PlantGeneticsHelper.GetDiskRank(disk));
			}
			PlantGeneticsHelper.AddHeat(seed, plantTrait);
			if (success && disk != null && PlantGeneticsHelper.SpendDiskUse(disk))
			{
				disk.Destroy();
			}
			seed.Validate();
		}
	}

	private static void FinishExtract(GameItem machine, GameItem seed, GameItem disk, bool success)
	{
		bool flag = PlantGeneticsHelper.IsWrittenDisk(disk);
		PlantTrait plantTrait = (flag ? PlantGeneticsHelper.GetDiskTrait(disk) : PlantGeneticsHelper.PickRandomTrait(seed, GetExtractionFilter(machine)));
		if (flag && plantTrait != null && PlantGeneticsHelper.GetTraitRank(seed, plantTrait.id) <= 0)
		{
			plantTrait = null;
		}
		bool num = HasStabilizer(machine);
		if (success && plantTrait != null)
		{
			int num2 = PlantGeneticsHelper.ExtractionUses(seed, plantTrait);
			if (flag)
			{
				PlantGeneticsHelper.AddDiskUses(disk, num2);
			}
			else
			{
				PlantGeneticsHelper.WriteDisk(disk, plantTrait, 1, num2);
			}
		}
		if (num)
		{
			if (success && plantTrait != null)
			{
				PlantGeneticsHelper.RemoveTraitRank(seed, plantTrait);
			}
			PlantGeneticsHelper.AddHeat(seed, plantTrait);
			seed.Validate();
		}
		else
		{
			seed.Destroy();
		}
	}

	public static bool HasStabilizer(GameItem machine)
	{
		return HasModule(machine, "dna_manipulator_module_stabilizer");
	}

	public static PlantTrait GetPrecisionTrait(GameItem machine)
	{
		foreach (GameItem module in MachineHelper.GetModules(machine))
		{
			if (module.identifier.Equals("dna_manipulator_module_precision"))
			{
				PlantTrait plantTrait = PlantTraitRegistry.Get(module.GetTagReadonly("DNA_PRECISION_TRAIT_STRING").GetString());
				if (plantTrait != null)
				{
					return plantTrait;
				}
			}
		}
		return null;
	}

	public static void SetupPrecisionModule(GameItem module)
	{
		module.ModifyTag("DNA_PRECISION_TRAIT_STRING", delegate(TagState state)
		{
			state.Enable().SetString("");
		});
		module.canUseOutsideCombat = true;
		module.mayThisTargetItemFunc = (GameItem self, GameItem target) => GeneralHelper.IsItemOwned(self) && PlantGeneticsHelper.IsWrittenDisk(target);
		module.canThisTargetItemFunc = (GameItem self, GameItem target) => GeneralHelper.IsItemOwned(self) && PlantGeneticsHelper.IsWrittenDisk(target);
		module.onThisTargetItemFunc = delegate(GameItem self, GameItem target)
		{
			PlantTrait trait = PlantGeneticsHelper.GetDiskTrait(target);
			if (trait != null)
			{
				self.ModifyTag("DNA_PRECISION_TRAIT_STRING", delegate(TagState state)
				{
					state.Enable().SetString(trait.id);
				});
				RefreshPrecisionModule(self);
				AudioManager.Instance.Play3("scanner_activate");
			}
		};
		RefreshPrecisionModule(module);
	}

	public static void RefreshPrecisionModule(GameItem module)
	{
		PlantTrait plantTrait = PlantTraitRegistry.Get(module.GetTagReadonly("DNA_PRECISION_TRAIT_STRING").GetString());
		module.name = ((plantTrait != null) ? PlantGeneticsText.NamedWithTrait(PlantGeneticsText.PrecisionName(), plantTrait.displayName) : PlantGeneticsText.PrecisionName());
		module.shortDescription = PlantGeneticsText.PrecisionDescription();
		module.Validate();
	}

	private static bool HasModule(GameItem machine, string moduleId)
	{
		foreach (GameItem module in MachineHelper.GetModules(machine))
		{
			if (module.identifier.Equals(moduleId))
			{
				return true;
			}
		}
		return false;
	}

	private static Func<PlantTrait, bool> GetExtractionFilter(GameItem machine)
	{
		PlantTrait precision = GetPrecisionTrait(machine);
		if (precision != null)
		{
			return (PlantTrait trait) => trait.id.Equals(precision.id);
		}
		bool flag = HasModule(machine, "dna_manipulator_module_positive_filter");
		bool flag2 = HasModule(machine, "dna_manipulator_module_negative_filter");
		if (flag && flag2)
		{
			return (PlantTrait _) => false;
		}
		if (flag)
		{
			return (PlantTrait trait) => !trait.IsNegative;
		}
		if (flag2)
		{
			return (PlantTrait trait) => trait.IsNegative;
		}
		return null;
	}

	private static void LockIfPresent(GameItem item)
	{
		if (item != null)
		{
			GeneralHelper.LockItem(item);
		}
	}

	private static void UnlockIfPresent(GameItem item)
	{
		if (item != null)
		{
			GeneralHelper.UnlockItem(item);
		}
	}

	public static GameSlotInventory GetInputDiskSlot(GameItem machine)
	{
		return (GameSlotInventory)MachineHelper.GetBodyStorage(machine, 2);
	}

	public static GameSlotInventory GetSeedSlot(GameItem machine)
	{
		return (GameSlotInventory)MachineHelper.GetBodyStorage(machine, 3);
	}

	public static GameSlotInventory GetOutputDiskSlot(GameItem machine)
	{
		return (GameSlotInventory)MachineHelper.GetBodyStorage(machine, 4);
	}

	public static GameGridInventory GetEjectGrid(GameItem machine)
	{
		return MachineHelper.GetBodyStorage(machine, 6) as GameGridInventory;
	}

	public static bool IsEjectEnabled(GameItem machine)
	{
		return !machine.GetTagReadonly("DNA_MANIPULATOR_KEEP_DISK_BOOL").GetBool();
	}

	private static void SetupEjectGrid(GameGridInventory grid)
	{
		grid.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && PlantGeneticsHelper.IsDisk(item);
	}

	private static void EjectDisk(GameItem machine, GameSlotInventory slot)
	{
		if (!IsEjectEnabled(machine))
		{
			return;
		}
		GameItem childItem = slot.childItem;
		if (childItem != null && PlantGeneticsHelper.IsDisk(childItem))
		{
			GameGridInventory ejectGrid = GetEjectGrid(machine);
			if (ejectGrid != null)
			{
				GraphUtils.TryAcceptAll(ejectGrid, childItem);
			}
		}
	}

	private static void SetupInputDiskSlot(GameSlotInventory slot, GameSlotInventory seedSlot)
	{
		slot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && PlantGeneticsHelper.IsWrittenDisk(item) && PlantGeneticsHelper.IsDiskCompatibleWithSeed(item, seedSlot.childItem);
		slot.SetBackgroundFadeSprite("Items/items_hydroponic_plant", "dna_disk_black_full");
	}

	private static void SetupSeedSlot(GameSlotInventory slot, GameSlotInventory inputDiskSlot, GameSlotInventory outputDiskSlot)
	{
		slot.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				return false;
			}
			if (!item.IsTag("SEED_ITEM_TAG") || item.IsTag("CONTAINER_TAG"))
			{
				return false;
			}
			if (item.IsTag("GROWING_TAG"))
			{
				return false;
			}
			if (HydroponicHelper.GetPlant(item) == null)
			{
				return false;
			}
			string reason;
			return PlantGeneticsHelper.IsDiskCompatibleWithSeed(inputDiskSlot.childItem, item) && PlantGeneticsHelper.CanExtractOntoDisk(item, outputDiskSlot.childItem, out reason);
		};
		slot.SetBackgroundFadeSprite("Items/items_hydroponic_plant", "nutrifruit_seed");
	}

	private static void SetupOutputDiskSlot(GameSlotInventory slot, GameSlotInventory seedSlot)
	{
		slot.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
		{
			if (!GeneralHelper.IsItemOwned(item) || !PlantGeneticsHelper.IsDisk(item))
			{
				return false;
			}
			string reason;
			return PlantGeneticsHelper.IsBlankDisk(item) || PlantGeneticsHelper.CanExtractOntoDisk(seedSlot.childItem, item, out reason);
		};
		slot.SetBackgroundFadeSprite("Items/items_hydroponic_plant", "dna_disk_black_empty");
	}

	public static (PixelWindow, GameSlotInventory, GameGridInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameGridInventory) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory("10000010001000001110000010001000001", 7);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory3 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory4 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory5 = new GameSlotInventory();
		GameGridInventory gameGridInventory2 = new GameGridInventory(2, 2);
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory2.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory3.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory4.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(PlantGeneticsText.LabelModule(), gameGridInventory, "module"), MachineHelper.Panel(PlantGeneticsText.LabelTraitDisk(), gameSlotInventory2, "input_disk"), MachineHelper.Panel(PlantGeneticsText.LabelTarget(), gameSlotInventory3, "seed"), MachineHelper.Panel(PlantGeneticsText.LabelBlankDisk(), gameSlotInventory4, "output_disk"), MachineHelper.NotePanel(gameSlotInventory5), MachineHelper.Panel(PlantGeneticsText.LabelOutput(), gameGridInventory2, "eject")), gameSlotInventory, gameGridInventory, gameSlotInventory2, gameSlotInventory3, gameSlotInventory4, gameSlotInventory5, gameGridInventory2);
	}

	public static GameItem CreateNote()
	{
		GameItem gameItem = DirectoryMaster.Item("postit");
		gameItem.name = PlantGeneticsText.InstructionName();
		gameItem.flavorText = PlantGeneticsText.InstructionText();
		gameItem.shortDescription = PlantGeneticsText.InstructionShort();
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("MACHINE_MANUAL_TAG");
		gameItem.unitValue = 0L;
		return gameItem;
	}

	public static GameItem GetIncompatibleDragTarget(GameItem dragged, GameItem hoveredItem, GameInventory hoveredInventory, out string reason)
	{
		reason = "";
		if (dragged == null)
		{
			return null;
		}
		GameItem gameItem = null;
		if (hoveredItem != null && hoveredItem.IsTag("DNA_MANIPULATOR_TAG"))
		{
			gameItem = hoveredItem;
		}
		else
		{
			if (hoveredItem != null)
			{
				GameItem gameItem2 = hoveredItem.parentInventory?.GetParentItem();
				if (gameItem2 != null && gameItem2.IsTag("DNA_MANIPULATOR_TAG"))
				{
					gameItem = gameItem2;
					goto IL_006e;
				}
			}
			if (hoveredItem == null)
			{
				GameItem gameItem3 = hoveredInventory?.GetParentItem();
				if (gameItem3 != null && gameItem3.IsTag("DNA_MANIPULATOR_TAG"))
				{
					gameItem = gameItem3;
				}
			}
		}
		goto IL_006e;
		IL_006e:
		if (gameItem == null)
		{
			return null;
		}
		if (PlantGeneticsHelper.IsWrittenDisk(dragged))
		{
			reason = PlantGeneticsHelper.GetDiskIncompatibilityReason(dragged, GetSeedSlot(gameItem).childItem);
		}
		else if (dragged.IsTag("SEED_ITEM_TAG") && HydroponicHelper.GetPlant(dragged) != null)
		{
			reason = PlantGeneticsHelper.GetDiskIncompatibilityReason(GetInputDiskSlot(gameItem).childItem, dragged);
		}
		if (reason.Length <= 0)
		{
			return null;
		}
		return gameItem;
	}

	public static RichText CreateIncompatibleDragTooltip(RichTextBuilder builder, GameItem dragged, GameItem machine, string reason)
	{
		builder.Clear();
		builder.Add(PlantGeneticsText.DragRefused(dragged.GetDisplayName(), "<u>" + machine.GetDisplayName() + "</u>", reason), RenderHandler.ColorPalette.Red);
		return builder.Build();
	}

	public static void CreateMachineTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("DNA_MANIPULATOR_TAG"))
		{
			return;
		}
		builder.AddLine(" ");
		bool flag = IsEjectEnabled(item);
		builder.AddLine(flag ? PlantGeneticsText.EjectOn() : PlantGeneticsText.EjectOff(), ignoreEmpty: true, flag ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightOrange);
		builder.AddLine(PlantGeneticsText.EjectHint(), ignoreEmpty: true, RenderHandler.ColorPalette.Gray);
		builder.AddLine(" ");
		if (MachineProgressHelper.GetMachineState(item).Equals("STATE_WORKING"))
		{
			string @string = item.GetTagReadonly("DNA_OPERATION_STRING").GetString();
			PlantTrait plantTrait = PlantTraitRegistry.Get(item.GetTagReadonly("DNA_OPERATION_TRAIT_STRING").GetString());
			string text = ((@string.Equals("apply") && plantTrait != null) ? PlantGeneticsText.StatusApplying(plantTrait.displayName) : PlantGeneticsText.StatusExtracting());
			builder.AddLine(text, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			var (num, b) = MachineProgressHelper.GetProgress(item);
			builder.AddProgressBar((float)num / (float)Mathf.Max(1, b), 10, RenderHandler.ColorPalette.LightGreen, RenderHandler.ColorPalette.VeryDarkGreen, null, " ");
			builder.AddLine(num + "/" + b, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			builder.AddLine(PlantGeneticsText.SuccessChance(item.GetTagReadonly("DNA_OPERATION_SUCCESS_INT").GetInt()), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			return;
		}
		OperationPlan operationPlan = PlanOperation(item);
		if (operationPlan == null)
		{
			builder.AddLine(PlantGeneticsText.StatusReady(), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			return;
		}
		if (operationPlan.blockedReason.Length > 0)
		{
			builder.AddLine(PlantGeneticsText.CannotStart(operationPlan.blockedReason), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			return;
		}
		if (operationPlan.operation.Equals("apply"))
		{
			builder.AddLine(operationPlan.isStacking ? PlantGeneticsText.NextApplyStack(operationPlan.trait.displayName) : PlantGeneticsText.NextApply(operationPlan.trait.displayName), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		}
		else
		{
			bool flag2 = PlantGeneticsHelper.GetDiskTrait(GetOutputDiskSlot(item).childItem) != null;
			string text2 = ((operationPlan.trait == null) ? PlantGeneticsText.NextExtractRandom() : (flag2 ? PlantGeneticsText.NextExtractRefill(operationPlan.trait.displayName) : PlantGeneticsText.NextExtractTrait(operationPlan.trait.displayName)));
			builder.AddLine(text2, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			builder.AddLine(HasStabilizer(item) ? PlantGeneticsText.TargetKept() : PlantGeneticsText.TargetDestroyed(), ignoreEmpty: true, HasStabilizer(item) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightOrange);
		}
		builder.AddLine(PlantGeneticsText.SuccessChance(operationPlan.successChance), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_machine_required_work") + " " + operationPlan.progress, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
	}
}

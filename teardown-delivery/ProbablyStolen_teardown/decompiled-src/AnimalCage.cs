using System;
using UnityEngine;

public class AnimalCage
{
	public const string CAGE_TAG = "CAGE_TAG";

	public const string ALLOWED_IN_CAGE_TAG = "ALLOWED_IN_CAGE_TAG";

	public const string CAGE_HEALTH_TAG = "CAGE_HEALTH_TAG";

	public const string CAGE_MAX_HEALTH_TAG = "CAGE_MAX_HEALTH_TAG";

	public const string CAGE_SIZE_TAG = "CAGE_SIZE_TAG";

	public const int STARTING_HEALTH = 100;

	public const int MAX_HEALTH = 100;

	public const int CAGE_WEAR_DIVISOR = 4;

	public const int REPAIR_PERCENT_OF_MAX = 50;

	public const int UPGRADE_HEALTH_BONUS = 20;

	public const int STARTING_WIDTH = 10;

	public const int STARTING_HEIGHT = 3;

	public const int MAX_SIZE = 8;

	public const int SIZE_PER_UPGRADE = 1;

	public const int ESCAPE_HEALTH_THRESHOLD = 20;

	public const int ESCAPE_CHANCE_AT_THRESHOLD = 5;

	public const int ESCAPE_CHANCE_AT_ZERO = 25;

	private const int COL_CAGE = 0;

	private const int COL_DISPENSER = 1;

	private const int COL_BUTCHER = 2;

	public static GameItem CreateAnimalCage()
	{
		(PixelWindow, GameInventory, GameSlotInventory, MachineryButton, GameSlotInventory, MachineryButton) tuple = CreateCageWindow();
		PixelWindow item2 = tuple.Item1;
		GameInventory item3 = tuple.Item2;
		GameSlotInventory dispenserSlot = tuple.Item3;
		MachineryButton blendButton = tuple.Item4;
		GameSlotInventory item4 = tuple.Item5;
		MachineryButton item5 = tuple.Item6;
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(item2).SetSpriteAndShape("Items/items_husbandry", "animal_cage");
		machine.name = LocHelper.GetLocalizedMechanic("husbandry_cage_name");
		machine.shortDescription = LocHelper.GetLocalizedMechanic("husbandry_cage_desc");
		machine.unitValue = 250L;
		machine.EnableTag("CAGE_TAG");
		machine.ModifyTag("CAGE_HEALTH_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(100);
		});
		machine.ModifyTag("CAGE_MAX_HEALTH_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(100);
		});
		machine.ModifyTag("CAGE_SIZE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(3);
		});
		AudioHelper.InitMetalMachine(machine);
		machine.EnableTag("CONTAINER_TAG");
		machine.EnableTag("IMPORTANT_TAG");
		item3.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
		{
			if (!GeneralHelper.IsItemOwned(machine))
			{
				return false;
			}
			if (!GeneralHelper.IsItemOwned(item))
			{
				return false;
			}
			if (item.IsTag("CONTAINER_TAG"))
			{
				return false;
			}
			return (!item.IsGameItemType("MACHINE") || item.IsTag("ALLOWED_IN_CAGE_TAG")) ? true : false;
		};
		machine.onCycleEndSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			GameGridInventory cageInventory2 = GetCageInventory(curItem);
			int num = 0;
			bool flag = false;
			foreach (GameItem childItem in cageInventory2.childItems)
			{
				if (childItem.IsTag("ANIMAL_ITEM_TAG") && !childItem.IsTag("ANIMAL_IS_DEAD_TAG"))
				{
					num += childItem.GetItemSize();
					if (childItem.GetTagReadonly("ANIMAL_OVERCROWDING_TAG").GetInt() > 0)
					{
						flag = true;
					}
				}
			}
			if (num > 0)
			{
				int int2 = curItem.GetTagReadonly("CAGE_HEALTH_TAG").GetInt();
				int num2 = (flag ? (num * 2) : num);
				num2 /= 4;
				int newHealth = Mathf.Max(0, int2 - num2);
				curItem.ModifyTag("CAGE_HEALTH_TAG", delegate(TagState state)
				{
					state.SetInt(newHealth);
				});
			}
		};
		machine.onLoaded = delegate(GameItem item)
		{
			item.EnableTag("IMPORTANT_TAG");
			int @int = item.GetTagReadonly("CAGE_SIZE_TAG").GetInt();
			GameGridInventory cageInventory = GetCageInventory(item);
			cageInventory.SetShape(10, @int);
			cageInventory.Validate();
			machine.contentWindow.Validate();
			machine.Validate();
		};
		dispenserSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(machine, item) && item.IsTag("FEED_DISPENSER_TAG");
		dispenserSlot.SetBackgroundFadeSprite("Items/items_husbandry", "animal_feeder");
		blendButton.SetCallback(delegate
		{
			GameItem gameItem2 = DispenserIn(dispenserSlot);
			if (gameItem2 != null)
			{
				MachineFeedDispenser.Blend(gameItem2, MachineFeedDispenser.GetStorageGrid(gameItem2));
			}
		});
		dispenserSlot.onSlotAddItemEndFunc = delegate
		{
			RefreshBlendButton();
		};
		dispenserSlot.onSlotRemoveItemEndFunc = delegate
		{
			RefreshBlendButton();
		};
		GameItem gameItem = machine;
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate
		{
			RefreshBlendButton();
		});
		RefreshBlendButton();
		MachineAutoButcher.SetupHostSlot(machine, item4, item5);
		return machine;
		void RefreshBlendButton()
		{
			blendButton.SetHidden(DispenserIn(dispenserSlot) == null);
		}
	}

	public static GameItem SpawnCageWithDispenser(bool shabby = false)
	{
		GameItem gameItem = ItemSpawner.Spawn("animal_cage");
		if (gameItem == null)
		{
			return null;
		}
		if (shabby)
		{
			ItemFeatureHelper.SetShabby(gameItem);
		}
		GameItem gameItem2 = ItemSpawner.Spawn("livestock_feed_dispenser");
		if (gameItem2 != null)
		{
			if (shabby)
			{
				ItemFeatureHelper.SetShabby(gameItem2);
			}
			GraphUtils.TryAcceptAllOrDestroy(GetDispenserSlot(gameItem), gameItem2, "rancher starter cage");
		}
		return gameItem;
	}

	public static int GetEscapeChance(GameItem cage)
	{
		int @int = cage.GetTagReadonly("CAGE_HEALTH_TAG").GetInt();
		if (@int >= 20)
		{
			return 0;
		}
		return Mathf.RoundToInt(Mathf.Lerp(25f, 5f, (float)@int / 20f));
	}

	public static (PixelWindow, GameInventory, GameSlotInventory, MachineryButton, GameSlotInventory, MachineryButton) CreateCageWindow()
	{
		GameGridInventory gameGridInventory = new GameGridInventory(10, 3);
		gameGridInventory.EnableTag("CAGE_TAG");
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 1;
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		MachineryButton machineryButton = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_ras_btn_blend"));
		GridPixelElement gridPixelElement = new GridPixelElement(1, 2, isCentered: true);
		gridPixelElement.SetSpacing(0, 4);
		gridPixelElement.Attach(new MachineryIntake(gameSlotInventory), 0, 0, 5, 5);
		gridPixelElement.Attach(machineryButton, 0, 1, 5, 5);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		gameSlotInventory2.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		MachineryButton machineryButton2 = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_auto_butcher_btn_butcher_now"));
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered: true);
		gridPixelElement2.SetSpacing(0, 4);
		gridPixelElement2.Attach(new MachineryIntake(gameSlotInventory2), 0, 0, 5, 5);
		gridPixelElement2.Attach(machineryButton2, 0, 1, 5, 5);
		return (MachineHelper.CreateMachineryWindow(true, new int[3] { 1, 0, 2 }, MachineHelper.Panel(LocHelper.GetLocalizedMechanic("husbandry_cage_inv_label"), gameGridInventory, "cage"), new MachineryPanel(LocHelper.GetLocalizedMechanic("mech_ras_label_dispenser"), gridPixelElement, "dispenser"), new MachineryPanel(LocHelper.GetLocalizedMechanic("mech_auto_butcher_label_slot"), gridPixelElement2, "butcher")), gameGridInventory, gameSlotInventory, machineryButton, gameSlotInventory2, machineryButton2);
	}

	public static GameGridInventory GetCageInventory(GameItem cage)
	{
		return MachineHelper.GetBodyStorage(cage, 0) as GameGridInventory;
	}

	public static GameSlotInventory GetDispenserSlot(GameItem cage)
	{
		return (MachineHelper.GetBodyStorage(cage, 1) as GridPixelElement)?.children.Find((GraphNodeStorage n) => n is GameSlotInventory) as GameSlotInventory;
	}

	public static GameSlotInventory GetButcherSlot(GameItem cage)
	{
		return (MachineHelper.GetBodyStorage(cage, 2) as GridPixelElement)?.children.Find((GraphNodeStorage n) => n is GameSlotInventory) as GameSlotInventory;
	}

	private static GameItem DispenserIn(GameSlotInventory dispenserSlot)
	{
		GameItem gameItem = dispenserSlot?.childItem;
		if (gameItem == null || !gameItem.IsTag("FEED_DISPENSER_TAG"))
		{
			return null;
		}
		return gameItem;
	}

	public static GameItem GetSlottedDispenser(GameInventory cageGrid)
	{
		if (cageGrid == null || !cageGrid.IsTag("CAGE_TAG"))
		{
			return null;
		}
		return DispenserIn(GetDispenserSlot(cageGrid.GetParentItem()));
	}

	public static bool CanUpgradeSize(GameItem cage)
	{
		if (cage == null)
		{
			return false;
		}
		return cage.GetTagReadonly("CAGE_SIZE_TAG").GetInt() < 8;
	}

	public static bool CanRepair(GameItem cage)
	{
		if (cage == null)
		{
			return false;
		}
		return cage.GetTagReadonly("CAGE_HEALTH_TAG").GetInt() < cage.GetTagReadonly("CAGE_MAX_HEALTH_TAG").GetInt();
	}

	public static void UpgradeSize(GameItem cage, GameItem ingot)
	{
		if (CanUpgradeSize(cage))
		{
			int @int = cage.GetTagReadonly("CAGE_SIZE_TAG").GetInt();
			int newSize = Mathf.Min(@int + 1, 8);
			cage.ModifyTag("CAGE_SIZE_TAG", delegate(TagState state)
			{
				state.SetInt(newSize);
			});
			int newMaxHealth = cage.GetTagReadonly("CAGE_MAX_HEALTH_TAG").GetInt() + 20;
			cage.ModifyTag("CAGE_MAX_HEALTH_TAG", delegate(TagState state)
			{
				state.SetInt(newMaxHealth);
			});
			int newHealth = Mathf.Min(cage.GetTagReadonly("CAGE_HEALTH_TAG").GetInt() + 20, newMaxHealth);
			cage.ModifyTag("CAGE_HEALTH_TAG", delegate(TagState state)
			{
				state.SetInt(newHealth);
			});
			GetCageInventory(cage).SetShape(10, newSize);
			cage.contentWindow.Validate();
			cage.unitValue += 40L;
			StartingPerkContent.ConsumeUpgradeMaterial(ingot);
			EmporiumAudioPlayer.Instance.ratchet.Play();
		}
	}

	public static void Repair(GameItem cage, GameItem scrapMetal)
	{
		if (CanRepair(cage))
		{
			int @int = cage.GetTagReadonly("CAGE_HEALTH_TAG").GetInt();
			int int2 = cage.GetTagReadonly("CAGE_MAX_HEALTH_TAG").GetInt();
			int num = int2 * 50 / 100;
			int newHealth = Mathf.Min(@int + num, int2);
			cage.ModifyTag("CAGE_HEALTH_TAG", delegate(TagState state)
			{
				state.SetInt(newHealth);
			});
			scrapMetal.Destroy();
		}
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("CAGE_TAG"))
		{
			int @int = item.GetTagReadonly("CAGE_HEALTH_TAG").GetInt();
			int int2 = item.GetTagReadonly("CAGE_MAX_HEALTH_TAG").GetInt();
			int int3 = item.GetTagReadonly("CAGE_SIZE_TAG").GetInt();
			int num = 8 - int3;
			builder.AddLine(" ");
			builder.Add(LocHelper.GetLocalizedMechanic("husbandry_cage_durability"), RenderHandler.ColorPalette.LightCyan);
			builder.AddProgressBar((float)@int / (float)int2, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
			builder.AddLine("(" + @int + "/" + int2 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			MachineFeedDispenser.CreateSlottedTooltip(builder, DispenserIn(GetDispenserSlot(item)));
			builder.AddLine(" ");
			if (CanRepair(item))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("husbandry_cage_repair_hint"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
			}
			if (CanUpgradeSize(item))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("husbandry_cage_expand_hint", num), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
			}
			if (!CanUpgradeSize(item))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("husbandry_cage_max_size"), ignoreEmpty: true, RenderHandler.ColorPalette.Cyan);
			}
			if (!CanRepair(item))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("husbandry_cage_fully_repaired"), ignoreEmpty: true, RenderHandler.ColorPalette.Cyan);
			}
		}
	}
}

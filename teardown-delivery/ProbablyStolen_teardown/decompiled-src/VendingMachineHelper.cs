using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class VendingMachineHelper
{
	public const string VENDING_MACHINE_TAG = "VENDING_MACHINE_TAG";

	public const string VENDING_ATTRACTIVENESS = "VENDING_ATTRACTIVENESS";

	public const string VENDING_CAPACITY = "VENDING_CAPACITY";

	public const int MAX_WIDTH = 8;

	public const int STARTING_WIDTH = 8;

	public const int STARTING_HEIGHT = 2;

	public const int MAX_CAPACITY = 12;

	public const int STARTING_ATTRACTIVENESS = 15;

	public const int MAX_ATTRACTIVENESS = 100;

	public const int POWER_USE = 2;

	public const int CAPACITY_UPGRADE_POWER_INCREASE = 1;

	public const int ATTRACTIVENESS_PER_UPGRADE = 5;

	public const int MAX_CAPACITY_UPGRADES = 10;

	public const int MAX_ATTRACTIVENESS_UPGRADES = 17;

	public const string VENDING_BLACKLISTED_TAG = "VENDING_BLACKLISTED_TAG";

	public const string VENDING_CASH = "VENDING_CASH";

	public const int MISLABEL_REP_VALUE_DIVISOR = 100;

	public const int MISLABEL_REP_LOSS_CAP = 20;

	public const int GUARANTEED_SALE_MAX_VALUE = 50;

	public const float LUXURY_EXTRA_MULT = 0.5f;

	public const int VALUE_CURVE_PIVOT = 200;

	public static void UnlockVendingMachine(int charge = 0)
	{
		EmporiumEntry.Instance.vendingMachineWindow.Validate();
		PlayerStore.Instance.isVendingMachineUnlocked = true;
		NetworkUpgrade.MarkUnlockedSilently("VENDING_MACHINE");
		StoreVendingMachine.Instance.gameObject.SetActive(value: true);
		GameItem gameItem = ItemSpawner.Spawn("vending_machine");
		GameItem gameItem2 = ItemSpawner.Spawn("vending_machine_core");
		GeneralHelper.SetParentItem(gameItem2, gameItem);
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => true;
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.vendingMachineElement, gameItem);
		GraphUtils.TryAcceptAll(gameItem, gameItem2);
		if (charge > 0)
		{
			GameItem item = PreBuiltItemHelper.EnergyCredit(charge);
			GraphUtils.TryAcceptAll(gameItem, item);
		}
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		EmporiumEntry.Instance.Validate();
	}

	public static GameItem VendingMachineCore()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_module", "core");
		gameItem.name = LocHelper.GetLocalizedItem("item_vending_machine_core_name");
		return gameItem;
	}

	public static GameItem VendingMachine()
	{
		(PixelWindow, GameSlotInventory, GameInventory, MachineryText, MachineryButton, GameSlotInventory) tuple = CreateMachineInventoryWindow();
		PixelWindow item2 = tuple.Item1;
		GameSlotInventory powerInv = tuple.Item2;
		GameInventory invGrid = tuple.Item3;
		MachineryText cashLabel = tuple.Item4;
		MachineryButton item3 = tuple.Item5;
		GameSlotInventory item4 = tuple.Item6;
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(item2).SetSpriteAndShape("Items/items_vending", "vending0");
		machine.name = LocHelper.GetLocalizedItem("item_vending_machine_name");
		machine.flavorText = LocHelper.GetLocalizedItem("item_vending_machine_flavor");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_vending_machine_desc");
		MachineryHelper.InitMachinery(machine, 2);
		machine.EnableTag("IMPORTANT_TAG");
		machine.ModifyTag("VENDING_CASH", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		machine.ModifyTag("VENDING_ATTRACTIVENESS", delegate(TagState state)
		{
			state.SetInt(15);
		});
		machine.ModifyTag("VENDING_CAPACITY", delegate(TagState state)
		{
			state.SetInt(2);
		});
		machine.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		item4.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		item4.mayInventoryRemoveItemFunc = (GameItem item, GameInventory _) => false;
		invGrid.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
		{
			if (item.IsTag("VENDING_BLACKLISTED_TAG"))
			{
				return false;
			}
			if (!GeneralHelper.IsItemOwned(item))
			{
				return false;
			}
			if (ContrabandHelper.IsContraband(item))
			{
				return false;
			}
			if (item.IsTag("CONTAINER_TAG") || item.IsGameItemType("MACHINE"))
			{
				return false;
			}
			return (!item.IsTag("LIQUID_CONTAINER_TAG") || item.IsTag("WINE_CONTAINER_TAG")) ? true : false;
		};
		invGrid.onSlotAddItemEndFunc = delegate
		{
			UpdateSprite(machine, "vending");
		};
		invGrid.onSlotRemoveItemEndFunc = delegate
		{
			UpdateSprite(machine, "vending");
		};
		item2.openCallback = delegate
		{
			cashLabel.SetText(LocHelper.GetLocalizedMechanic("mech_vending_cash_label", machine.GetTagReadonly("VENDING_CASH").GetInt()));
		};
		item3.SetCallback(delegate
		{
			TransferCashToPlayer(machine);
			cashLabel.SetText(LocHelper.GetLocalizedMechanic("mech_vending_cash_label", machine.GetTagReadonly("VENDING_CASH").GetInt()));
		});
		machine.onLoaded = delegate(GameItem item)
		{
			int @int = item.GetTagReadonly("VENDING_CAPACITY").GetInt();
			GameGridInventory vendingInventory = GetVendingInventory(item);
			vendingInventory.SetShape(8, @int);
			vendingInventory.Validate();
			machine.contentWindow.Validate();
			machine.Validate();
			UpdateSprite(machine, "vending");
			UpdatePowerUsage(machine);
		};
		MachineHelper.SetupBatterySlot(powerInv, machine);
		machine.onCycleEndSlotItemFunc = delegate
		{
			if (powerInv.childItem != null && PowerHelper.CanDrawPowerSource(powerInv.childItem, MachineryHelper.GetMachinePowerUsage(machine)) && invGrid.childItems.Count != 0)
			{
				HandleNight(machine, invGrid);
				PowerHelper.DrawPowerSource(powerInv.childItem, MachineryHelper.GetMachinePowerUsage(machine));
				cashLabel.SetText(LocHelper.GetLocalizedMechanic("mech_vending_cash_label", machine.GetTagReadonly("VENDING_CASH").GetInt()));
			}
		};
		return machine;
	}

	public static void HandleNight(GameItem vendingMachine, GameInventory vendingInventory)
	{
		List<GameItem> eligibleItems = CheckEligibleItem(vendingMachine, vendingInventory.childItems);
		foreach (GameItem item in CheckSoldItem(vendingMachine, eligibleItems).ToList())
		{
			item.AddItemFeature(ItemFeatureList.VendingMachineBonus());
			int value = (int)item.GetCurrentValue(useRetailMarkup: true, withChild: true, includeEvents: false, forceMarkup: true, 0L);
			TrackMislabelledSale(item, value);
			SoldItem.MarkStolenOrContrabandSale(item);
			PlayerStore.Instance.PerformOnSoldCheck(item, null);
			item.Destroy();
			int currentCash = vendingMachine.GetTagReadonly("VENDING_CASH").GetInt();
			vendingMachine.ModifyTag("VENDING_CASH", delegate(TagState state)
			{
				state.SetInt(currentCash + value);
			});
			RunStats.OnVendingSale(value);
		}
		UpdateSprite(vendingMachine, "vending");
	}

	public static void TrackMislabelledSale(GameItem item, int soldValue)
	{
		if (IsMislabelled(item))
		{
			PlayerStore instance = PlayerStore.Instance;
			instance.vendingMislabelCount++;
			if (IsDangerousMislabel(item))
			{
				instance.vendingMislabelEndangermentValue += soldValue;
			}
			else
			{
				instance.vendingMislabelFraudValue += soldValue;
			}
			instance.vendingMislabelRepLoss += (double)soldValue / 100.0;
		}
	}

	public static void TrackMislabelledFountainSale(int labelPurity, double weightedValue)
	{
		PlayerStore instance = PlayerStore.Instance;
		instance.vendingMislabelCount++;
		double num = weightedValue / 100.0;
		double num2 = labelPurity switch
		{
			1 => 0.5, 
			0 => 1.0, 
			_ => 0.0, 
		};
		instance.fountainMislabelRepLossUL += num * num2;
		instance.fountainMislabelRepLossLL += num * (1.0 - num2);
	}

	public static void TrackMislabelledRequestItem(GameItem item, int value, string factionId)
	{
		if (!IsMislabelled(item))
		{
			return;
		}
		PlayerStore instance = PlayerStore.Instance;
		instance.requestMislabelCount++;
		if (IsReportingRequester(factionId))
		{
			if (IsDangerousMislabel(item))
			{
				instance.requestMislabelEndangermentValue += value;
			}
			else
			{
				instance.requestMislabelFraudValue += value;
			}
		}
		if (!string.IsNullOrEmpty(factionId))
		{
			instance.requestMislabelRepLoss.TryGetValue(factionId, out var value2);
			instance.requestMislabelRepLoss[factionId] = value2 + (double)value / 100.0;
		}
	}

	private static bool IsReportingRequester(string factionId)
	{
		if (factionId != "FACTION_BLACK_MARKET")
		{
			return factionId != "FACTION_REVOLUTION";
		}
		return false;
	}

	public static bool IsMislabelled(GameItem item)
	{
		List<GameItem> list = item.FindAllChildItems();
		list.Add(item);
		foreach (GameItem item2 in list)
		{
			if (item2.itemFeatures == null)
			{
				continue;
			}
			foreach (ItemFeature itemFeature in item2.itemFeatures)
			{
				if (!itemFeature.isDisabled && !itemFeature.IsExposed() && !itemFeature.IsFeatureMatch())
				{
					if (itemFeature.useCondition && itemFeature.IsRealFeatureCheaper())
					{
						return true;
					}
					if (!itemFeature.useCondition && itemFeature.isExposable && !itemFeature.IsRealFeatureCheaper())
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private static bool IsDangerousMislabel(GameItem item)
	{
		ItemFeature itemFeature = item.FindItemFeatureByCategory("CATEGORY_GENUINE_INJECTOR");
		if (itemFeature != null && itemFeature.useCondition && itemFeature.realCondition != null && !itemFeature.realCondition.identifier.Equals("genuineInjector") && !itemFeature.IsFeatureMatch())
		{
			return true;
		}
		if (!ChemicalFeedbackHelper.IsToxic(item))
		{
			return ChemicalFeedbackHelper.IsLethal(item);
		}
		return true;
	}

	public static void SettleMislabelledSales()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		SettleVendingMislabels(dictionary);
		SettleRequestMislabels(dictionary);
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			SettleMislabelRep(item.Key, item.Value);
		}
	}

	private static void AddRepLoss(Dictionary<string, int> repLoss, string factionId, int loss)
	{
		if (loss > 0)
		{
			repLoss.TryGetValue(factionId, out var value);
			repLoss[factionId] = value + loss;
		}
	}

	private static void SettleRequestMislabels(Dictionary<string, int> repLoss)
	{
		PlayerStore instance = PlayerStore.Instance;
		int requestMislabelCount = instance.requestMislabelCount;
		if (requestMislabelCount <= 0)
		{
			return;
		}
		if (instance.requestMislabelFraudValue > 0)
		{
			instance.secData.CommitCrime("FRAUD", instance.requestMislabelFraudValue);
		}
		if (instance.requestMislabelEndangermentValue > 0)
		{
			instance.secData.CommitCrime("ENDANGERMENT", instance.requestMislabelEndangermentValue);
		}
		instance.AddNightLog(LocHelper.GetLocalizedMechanicOrDefault("mech_nightlog_request_mislabel", "Customers found {0} mislabelled item(s) in what you delivered for their marketplace requests.", requestMislabelCount), "9c3738");
		foreach (KeyValuePair<string, double> item in instance.requestMislabelRepLoss)
		{
			AddRepLoss(repLoss, item.Key, Mathf.Max(1, Mathf.RoundToInt((float)item.Value)));
		}
		instance.requestMislabelCount = 0;
		instance.requestMislabelFraudValue = 0;
		instance.requestMislabelEndangermentValue = 0;
		instance.requestMislabelRepLoss.Clear();
	}

	private static void SettleVendingMislabels(Dictionary<string, int> repLoss)
	{
		PlayerStore instance = PlayerStore.Instance;
		int vendingMislabelCount = instance.vendingMislabelCount;
		if (vendingMislabelCount > 0)
		{
			if (instance.vendingMislabelFraudValue > 0)
			{
				instance.secData.CommitCrime("FRAUD", instance.vendingMislabelFraudValue);
			}
			if (instance.vendingMislabelEndangermentValue > 0)
			{
				instance.secData.CommitCrime("ENDANGERMENT", instance.vendingMislabelEndangermentValue);
			}
			instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_vending_mislabel", vendingMislabelCount), "9c3738");
			int num = Mathf.Max(0, StoreReputation.GetLLReputation());
			int num2 = Mathf.Max(0, StoreReputation.GetULReputation());
			double num3 = ((num + num2 > 0) ? ((double)num2 / (double)(num + num2)) : 0.5);
			double num4 = instance.vendingMislabelRepLoss * (1.0 - num3) + instance.fountainMislabelRepLossLL;
			double num5 = instance.vendingMislabelRepLoss * num3 + instance.fountainMislabelRepLossUL;
			int num6 = Mathf.Max(1, Mathf.RoundToInt((float)(num4 + num5)));
			double num7 = ((num4 + num5 > 0.0) ? (num5 / (num4 + num5)) : num3);
			int num8 = Mathf.RoundToInt((float)((double)num6 * num7));
			int loss = num6 - num8;
			AddRepLoss(repLoss, "FACTION_LOWER_LEVEL", loss);
			AddRepLoss(repLoss, "FACTION_UPPER_LEVEL", num8);
			instance.vendingMislabelCount = 0;
			instance.vendingMislabelFraudValue = 0;
			instance.vendingMislabelEndangermentValue = 0;
			instance.vendingMislabelRepLoss = 0.0;
			instance.fountainMislabelRepLossLL = 0.0;
			instance.fountainMislabelRepLossUL = 0.0;
		}
	}

	private static void SettleMislabelRep(string factionId, int loss)
	{
		loss = Mathf.Min(loss, 20);
		if (loss > 0)
		{
			StoreReputation.ModReputation(factionId, -loss, notify: false);
			PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedUI("ui_rep_lost", StoreReputation.GetFactionNameByID(factionId)) + " (-" + loss + ")", "9c3738");
		}
	}

	public static List<GameItem> CheckEligibleItem(GameItem vendingMachine, List<GameItem> contentItems)
	{
		List<GameItem> list = new List<GameItem>();
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (GameItem contentItem in contentItems)
		{
			if (GetVendingPrice(contentItem) <= 0 || HopperHelper.IsHopper(contentItem))
			{
				continue;
			}
			string identifier = contentItem.identifier;
			if (!dictionary.ContainsKey(identifier))
			{
				dictionary[identifier] = 1;
				list.Add(contentItem);
				continue;
			}
			dictionary[identifier]++;
			int num = dictionary[identifier];
			float num2 = Mathf.Pow(0.5f, num - 1);
			if (Random.value <= num2)
			{
				list.Add(contentItem);
			}
		}
		return list;
	}

	public static int GetVendingPrice(GameItem item)
	{
		return (int)item.GetCurrentValue(useRetailMarkup: true, withChild: true, includeEvents: false, forceMarkup: true, 0L);
	}

	public static float GetSaleChance(int attractiveness, GameItem item, int price)
	{
		float num = 1f;
		if (item.IsGameItemType("MODULE") || item.IsGameItemType("NODE") || item.IsGameItemType("GUN_MOD") || item.IsGameItemType("CHEMICAL_SUPPLIES"))
		{
			num = 0.15f;
		}
		else if (item.IsGameItemType("LUXURY_ITEM"))
		{
			num = 0.165f;
		}
		else if (item.IsGameItemType("MATERIAL"))
		{
			num = 0.33f;
		}
		else if (item.IsGameItemType("ENERGY_CELL") || item.IsGameItemType("TOOL"))
		{
			num = 0.5f;
		}
		else if (IsVendingConsumable(item))
		{
			num = 1.5f;
		}
		else if (price >= 100 && price <= 200)
		{
			num = 0.5f;
		}
		else if (price > 200)
		{
			num = 0.25f;
		}
		float num2 = 200f / (float)(200 + Mathf.Max(0, price));
		return (float)attractiveness * num * num2;
	}

	public static int GetGuaranteedSaleCount(int attractiveness)
	{
		if (attractiveness >= 50)
		{
			return 3;
		}
		if (attractiveness >= 25)
		{
			return 2;
		}
		return 1;
	}

	private static bool IsVendingConsumable(GameItem item)
	{
		if (!item.IsGameItemType("FOOD") && !item.IsGameItemType("WATER_RATION") && !item.IsGameItemType("BREVAGE") && !item.IsGameItemType("PROCESSED_FOOD"))
		{
			return item.IsGameItemType("TREAT");
		}
		return true;
	}

	public static List<GameItem> CheckSoldItem(GameItem vendingMachine, List<GameItem> eligibleItems)
	{
		int @int = vendingMachine.GetTagReadonly("VENDING_ATTRACTIVENESS").GetInt();
		List<GameItem> soldItems = new List<GameItem>();
		Dictionary<GameItem, int> prices = new Dictionary<GameItem, int>();
		foreach (GameItem eligibleItem in eligibleItems)
		{
			int vendingPrice = GetVendingPrice(eligibleItem);
			prices[eligibleItem] = vendingPrice;
			if (Random.value * 100f < GetSaleChance(@int, eligibleItem, vendingPrice))
			{
				soldItems.Add(eligibleItem);
			}
		}
		int guaranteedSaleCount = GetGuaranteedSaleCount(@int);
		if (soldItems.Count < guaranteedSaleCount)
		{
			foreach (GameItem item in eligibleItems.Where((GameItem item) => !soldItems.Contains(item) && !item.IsGameItemType("LUXURY_ITEM") && prices[item] <= 50).OrderByDescending(IsVendingConsumable).ToList())
			{
				if (soldItems.Count >= guaranteedSaleCount)
				{
					break;
				}
				soldItems.Add(item);
			}
		}
		return soldItems;
	}

	public static void TransferCashToPlayer(GameItem vendingMachine)
	{
		int @int = vendingMachine.GetTagReadonly("VENDING_CASH").GetInt();
		if (@int > 0)
		{
			PlayerStore.Instance.ModCash(@int);
			vendingMachine.ModifyTag("VENDING_CASH", delegate(TagState state)
			{
				state.SetInt(0);
			});
		}
	}

	public static (PixelWindow, GameSlotInventory, GameInventory, MachineryText, MachineryButton, GameSlotInventory) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 5)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory(8, 2);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		MachineryText machineryText = new MachineryText(LocHelper.GetLocalizedMechanic("mech_vending_cash_label", 0), 6, HorizontalAlignmentOptions.Center, uppercase: false);
		MachineryButton machineryButton = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_vending_btn_transfer"));
		GridPixelElement gridPixelElement = new GridPixelElement(1, 2, isCentered: true);
		gridPixelElement.SetSpacing(0, 4);
		gridPixelElement.Attach(machineryText, 0, 0, 60, 10);
		gridPixelElement.Attach(machineryButton, 0, 1, 60, 5);
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameGridInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 1;
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_vending_label_inventory"), gameGridInventory, "inventory"), new MachineryPanel(LocHelper.GetLocalizedMechanic("mech_vending_label_control"), gridPixelElement, "control"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_vending_label_core"), gameSlotInventory2, "core")), gameSlotInventory, gameGridInventory, machineryText, machineryButton, gameSlotInventory2);
	}

	public static bool CanUpgradedCapacity(GameItem vendingMachine)
	{
		if (vendingMachine == null)
		{
			return false;
		}
		return vendingMachine.GetTagReadonly("VENDING_CAPACITY").GetInt() < 12;
	}

	public static bool CanUpgradedAttractiveness(GameItem vendingMachine)
	{
		if (vendingMachine == null)
		{
			return false;
		}
		return vendingMachine.GetTagReadonly("VENDING_ATTRACTIVENESS").GetInt() < 100;
	}

	public static void UpgradeCapacity(GameItem core, GameItem item)
	{
		GameItem parentItem = GeneralHelper.GetParentItem(core);
		if (CanUpgradedCapacity(parentItem) && parentItem.identifier.Equals("vending_machine"))
		{
			int @int = parentItem.GetTagReadonly("VENDING_CAPACITY").GetInt();
			int newCapacity = Mathf.Min(@int + 1, 12);
			parentItem.ModifyTag("VENDING_CAPACITY", delegate(TagState state)
			{
				state.SetInt(newCapacity);
			});
			StartingPerkContent.ConsumeUpgradeMaterial(item);
			UpdatePowerUsage(parentItem);
			GetVendingInventory(parentItem).SetShape(8, newCapacity);
			EmporiumAudioPlayer.Instance.ratchet.Play();
			parentItem.contentWindow.Validate();
			UpdateSprite(parentItem, "vending");
		}
	}

	public static void UpgradeAttractiveness(GameItem core, GameItem item)
	{
		if (!core.identifier.Equals("vending_machine_core") && !core.identifier.Equals("vending_fountain_core"))
		{
			return;
		}
		GameItem parentItem = GeneralHelper.GetParentItem(core);
		if (CanUpgradedAttractiveness(parentItem))
		{
			int @int = parentItem.GetTagReadonly("VENDING_ATTRACTIVENESS").GetInt();
			int newAttractiveness = Mathf.Min(@int + 5, 100);
			parentItem.ModifyTag("VENDING_ATTRACTIVENESS", delegate(TagState state)
			{
				state.SetInt(newAttractiveness);
			});
			StartingPerkContent.ConsumeUpgradeMaterial(item);
			if (core.identifier.Equals("vending_fountain_core"))
			{
				VendingFountainHelper.UpdatePowerUsage(parentItem);
			}
			EmporiumAudioPlayer.Instance.ratchet.Play();
		}
	}

	public static int GetCapacityUpgradeCount(GameItem vendingMachine)
	{
		return Mathf.Clamp(vendingMachine.GetTagReadonly("VENDING_CAPACITY").GetInt() - 2, 0, 10);
	}

	public static int GetAttractivenessUpgradeCount(GameItem vendingMachine)
	{
		return Mathf.Clamp((vendingMachine.GetTagReadonly("VENDING_ATTRACTIVENESS").GetInt() - 15 + 5 - 1) / 5, 0, 17);
	}

	public static void UpdatePowerUsage(GameItem vendingMachine)
	{
		int total = 2 + GetCapacityUpgradeCount(vendingMachine);
		vendingMachine.ModifyTag("machinery_drawn_power", delegate(TagState state)
		{
			state.Enable().SetInt(total);
		});
		vendingMachine.ModifyTag("MACHINERY_DRAWN_POWER_BASE", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
	}

	public static void AddUpgradeHeader(RichTextBuilder builder)
	{
		builder.AddLine(" ");
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_fridge_tooltip_upgrade_header"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
	}

	public static void AddAttractivenessUpgradeLine(RichTextBuilder builder, GameItem vendingMachine)
	{
		builder.AddLine(LocHelper.GetLocalizedMechanicOrDefault("mech_vending_tooltip_upgrade_attractiveness_count", "Attractiveness ({0}): {1}/{2}", LocHelper.GetLocalizedItem("item_common_electronic_name"), GetAttractivenessUpgradeCount(vendingMachine), 17), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
	}

	public static GameGridInventory GetVendingInventory(GameItem vendingMachine)
	{
		return MachineHelper.GetBodyStorage(vendingMachine, 1) as GameGridInventory;
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("vending_machine_core"))
		{
			GameItem parentItem = GeneralHelper.GetParentItem(item);
			if (parentItem != null)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_vending_tooltip_markup"), ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_vending_tooltip_popular"), ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_vending_tooltip_attractiveness", parentItem.GetTagReadonly("VENDING_ATTRACTIVENESS").GetInt()), ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
				string localizedItem = LocHelper.GetLocalizedItem("item_metal_ingot_name");
				AddUpgradeHeader(builder);
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_fridge_tooltip_upgrade_storage", localizedItem, GetCapacityUpgradeCount(parentItem), 10), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
				AddAttractivenessUpgradeLine(builder, parentItem);
				builder.AddLine(LocHelper.GetLocalizedMechanicOrDefault("mech_vending_tooltip_upgrade_power", "Each {0} upgrade raises the nightly energy use by {1}.", localizedItem, 1), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
				MachineryHelper.CreateMachineryTooltip(builder, parentItem);
			}
		}
	}

	public static void UpdateSprite(GameItem gameItem, string prefix)
	{
		int inventoryCapacity = SpriteHelper.GetInventoryCapacity(GetVendingInventory(gameItem));
		SetSpriteFromCapacity(gameItem, prefix, inventoryCapacity);
		StoreVendingMachine.Instance.UpdateSprite();
	}

	public static string GetSpriteNameFromCapacity(string prefix, int currentCapacity)
	{
		if (currentCapacity == 0)
		{
			return prefix + "0";
		}
		if (currentCapacity <= 14)
		{
			return prefix + "10";
		}
		if (currentCapacity <= 35)
		{
			return prefix + "25";
		}
		if (currentCapacity <= 65)
		{
			return prefix + "50";
		}
		if (currentCapacity <= 80)
		{
			return prefix + "75";
		}
		if (currentCapacity <= 97)
		{
			return prefix + "90";
		}
		return prefix + "100";
	}

	public static void SetSpriteFromCapacity(GameItem gameItem, string prefix, int currentCapacity)
	{
		gameItem.SetSpriteAndShape("Items/items_vending", GetSpriteNameFromCapacity(prefix, currentCapacity));
	}
}

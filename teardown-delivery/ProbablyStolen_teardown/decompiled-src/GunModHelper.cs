using System.Collections.Generic;

public class GunModHelper
{
	public const string MOD_STOCK_TAG = "MOD_STOCK_TAG";

	public const string MOD_RAIL_TAG = "MOD_RAIL_TAG";

	public const string MOD_UNDER_RAIL_TAG = "MOD_UNDER_RAIL_TAG";

	public const string MOD_BARREL_TAG = "MOD_BARREL_TAG";

	public const string MOD_MAGAZINE_TAG = "MOD_MAGAZINE_TAG";

	public const string MOD_SPECIAL_TAG = "MOD_SPECIAL_TAG";

	public const string WEAPON_MOD_TAG = "WEAPON_MOD_TAG";

	public const string MOD_SUPPRESSOR_TAG = "MOD_SUPPRESSOR_TAG";

	public const string MOD_BASE_FIREPOWER_TAG = "MOD_BASE_FIREPOWER_TAG";

	public const string MOD_BASE_STYLE_TAG = "MOD_BASE_STYLE_TAG";

	public const string MOD_BASE_RELIABILITY_TAG = "MOD_BASE_RELIABILITY_TAG";

	public const string MOD_BASE_RECOIL_TAG = "MOD_BASE_RECOIL_TAG";

	public const string MOD_BASE_ERGONOMICS_TAG = "MOD_BASE_ERGONOMICS_TAG";

	public const string MOD_BASE_ACCURACY_TAG = "MOD_BASE_ACCURACY_TAG";

	private static readonly Dictionary<string, string> exclusiveGroupByModId = new Dictionary<string, string>();

	public static void InitGunModRail(GameItem item)
	{
		item.EnableTag("MOD_RAIL_TAG");
		AudioHelper.InitGunMod(item);
		InitGunMod(item);
	}

	public static void InitGunModUnderRail(GameItem item)
	{
		item.EnableTag("MOD_UNDER_RAIL_TAG");
		AudioHelper.InitGunMod(item);
		InitGunMod(item);
	}

	public static void InitGunModBarrel(GameItem item, bool isSuppressor = false)
	{
		if (isSuppressor)
		{
			item.EnableTag("MOD_SUPPRESSOR_TAG");
		}
		item.EnableTag("MOD_BARREL_TAG");
		AudioHelper.InitGunMod(item);
		InitGunMod(item);
	}

	public static void InitGunModMagazine(GameItem item)
	{
		item.EnableTag("MOD_MAGAZINE_TAG");
		InitGunMod(item);
	}

	public static void InitGunModStock(GameItem item)
	{
		item.EnableTag("MOD_STOCK_TAG");
		InitGunMod(item);
	}

	public static void InitGunModSpecial(GameItem item)
	{
		item.EnableTag("MOD_SPECIAL_TAG");
		AudioHelper.InitPlasticMachinery(item);
		InitGunMod(item);
	}

	public static void InitGunMod(GameItem item, int firepower = 0, int style = 0, int reliability = 0, int recoil = 0, int ergonomics = 0, int accuracy = 0)
	{
		item.ModifyTag("GUN_FIREPOWER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(firepower);
		});
		item.ModifyTag("GUN_STYLE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(style);
		});
		item.ModifyTag("GUN_RELIABILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(reliability);
		});
		item.ModifyTag("GUN_RECOIL_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(recoil);
		});
		item.ModifyTag("GUN_ERGONOMICS_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(ergonomics);
		});
		item.ModifyTag("GUN_ACCURACY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(accuracy);
		});
		item.EnableTag("WEAPON_MOD_TAG");
		item.SetGameItemType("GUN_MOD");
	}

	public static void InitGunModBaseStats(GameItem item, int firepower = 0, int style = 0, int reliability = 0, int recoil = 0, int ergonomics = 0, int accuracy = 0)
	{
		item.ModifyTag("MOD_BASE_FIREPOWER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(firepower);
		});
		item.ModifyTag("MOD_BASE_STYLE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(style);
		});
		item.ModifyTag("MOD_BASE_RELIABILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(reliability);
		});
		item.ModifyTag("MOD_BASE_RECOIL_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(recoil);
		});
		item.ModifyTag("MOD_BASE_ERGONOMICS_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(ergonomics);
		});
		item.ModifyTag("MOD_BASE_ACCURACY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(accuracy);
		});
	}

	public static (PixelWindow, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory) CreateGunModWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 5)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory3 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory4 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory5 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory6 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory7 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory8 = new GameSlotInventory();
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory2.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory3.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory4.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory5.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory6.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory7.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory8.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		PixelWindow pixelWindow = MachineHelper.CreateMachineryWindow(isDraggable, SlotPanel("mech_gun_tag_stock", gameSlotInventory5), SlotPanel("mech_gun_tag_rail", gameSlotInventory), SlotPanel("mech_gun_tag_magazine", gameSlotInventory3), SlotPanel("mech_gun_tag_underbarrel", gameSlotInventory4), SlotPanel("mech_gun_tag_barrel", gameSlotInventory2), SlotPanel("mech_gun_tag_special1", gameSlotInventory6), SlotPanel("mech_gun_tag_special2", gameSlotInventory7), SlotPanel("mech_gun_tag_special3", gameSlotInventory8));
		((MachineryPanelRow)pixelWindow.childElement).SetColumns(5);
		return (pixelWindow, gameSlotInventory5, gameSlotInventory, gameSlotInventory3, gameSlotInventory4, gameSlotInventory2, gameSlotInventory6, gameSlotInventory7, gameSlotInventory8);
		static MachineryPanel SlotPanel(string key, GameSlotInventory slot)
		{
			GridPixelElement gridPixelElement = new GridPixelElement(1, 1);
			gridPixelElement.Attach(new MachineryIntake(slot), 0, 0, 0, 0);
			return new MachineryPanel(LocHelper.GetLocalizedMechanic(key), gridPixelElement, key);
		}
	}

	public static void EnableStockSlot(GameSlotInventory slot)
	{
		GameItem parentItem = slot.GetParentItem();
		slot.mayInventoryAddItemFunc = (GameItem mod, GameInventory inv) => mod.IsTag("MOD_STOCK_TAG") && GeneralHelper.IsItemOwned(mod) && MayModGun(parentItem) && !HasConflictingMod(inv.GetParentItem(), mod, inv, sameModConflicts: false);
		slot.maxSlotAddItemFunc = (GameItem mod, GameInventory inv, SlotMarker slot) => mod.IsTag("MOD_STOCK_TAG") ? 1 : 0;
		slot.onSlotAddItemEndFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			PlayEffect("moddingPlastic1");
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
		slot.onSlotRemoveItemEndFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			PlayEffect("moddingPlastic2");
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
		slot.onSlotAddItemLateFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
		slot.onSlotRemoveItemLateFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
	}

	public static void EnableRailSlot(GameSlotInventory slot)
	{
		GameItem parentItem = slot.GetParentItem();
		slot.mayInventoryAddItemFunc = (GameItem mod, GameInventory inv) => mod.IsTag("MOD_RAIL_TAG") && GeneralHelper.IsItemOwned(mod) && MayModGun(parentItem) && !HasConflictingMod(inv.GetParentItem(), mod, inv, sameModConflicts: false);
		slot.maxSlotAddItemFunc = (GameItem mod, GameInventory inv, SlotMarker slot) => mod.IsTag("MOD_RAIL_TAG") ? 1 : 0;
		slot.onSlotAddItemEndFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			PlayEffect("cocking2-1");
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
		slot.onSlotRemoveItemEndFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			PlayEffect("cocking2-2");
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
	}

	public static void EnableBarrelSlot(GameSlotInventory slot)
	{
		GameItem parentItem = slot.GetParentItem();
		slot.mayInventoryAddItemFunc = (GameItem mod, GameInventory inv) => mod.IsTag("MOD_BARREL_TAG") && GeneralHelper.IsItemOwned(mod) && MayModGun(parentItem) && !HasConflictingMod(inv.GetParentItem(), mod, inv, sameModConflicts: false);
		slot.maxSlotAddItemFunc = (GameItem mod, GameInventory inv, SlotMarker slot) => mod.IsTag("MOD_BARREL_TAG") ? 1 : 0;
		slot.onSlotAddItemEndFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			PlayEffect("modding1");
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
		slot.onSlotRemoveItemEndFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			PlayEffect("modding2");
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
	}

	public static void EnableMagazineSlot(GameSlotInventory slot, string effectID = null)
	{
		GameItem parentItem = slot.GetParentItem();
		slot.mayInventoryAddItemFunc = (GameItem mod, GameInventory inv) => mod.IsTag("MOD_MAGAZINE_TAG") && GeneralHelper.IsItemOwned(mod) && MayModGun(parentItem) && !HasConflictingMod(inv.GetParentItem(), mod, inv, sameModConflicts: false);
		slot.maxSlotAddItemFunc = (GameItem mod, GameInventory inv, SlotMarker slot) => mod.IsTag("MOD_MAGAZINE_TAG") ? 1 : 0;
		slot.onSlotAddItemEndFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			PlayEffect(effectID);
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
		slot.onSlotRemoveItemEndFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			PlayEffect("cocking2-2");
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
	}

	public static void EnableUnderbarrelSlot(GameSlotInventory slot)
	{
		GameItem parentItem = slot.GetParentItem();
		slot.mayInventoryAddItemFunc = (GameItem mod, GameInventory inv) => mod.IsTag("MOD_UNDER_RAIL_TAG") && GeneralHelper.IsItemOwned(mod) && MayModGun(parentItem) && !HasConflictingMod(inv.GetParentItem(), mod, inv, sameModConflicts: false);
		slot.maxSlotAddItemFunc = (GameItem mod, GameInventory inv, SlotMarker slot) => mod.IsTag("MOD_UNDER_RAIL_TAG") ? 1 : 0;
		slot.onSlotAddItemEndFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			PlayEffect("modding1");
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
		slot.onSlotRemoveItemEndFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			PlayEffect("modding2");
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
	}

	public static void EnableSpecialSlot(GameSlotInventory slot)
	{
		GameItem parentItem = slot.GetParentItem();
		slot.mayInventoryAddItemFunc = (GameItem mod, GameInventory inv) => mod.IsTag("MOD_SPECIAL_TAG") && GeneralHelper.IsItemOwned(mod) && MayModGun(parentItem) && !HasConflictingMod(inv.GetParentItem(), mod, inv, sameModConflicts: true);
		slot.maxSlotAddItemFunc = (GameItem mod, GameInventory inv, SlotMarker slot) => mod.IsTag("MOD_SPECIAL_TAG") ? 1 : 0;
		slot.onSlotAddItemEndFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			PlayEffect("modding1");
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
		slot.onSlotRemoveItemEndFunc = delegate(GameItem mod, GameInventory inv, SlotMarker slot)
		{
			PlayEffect("modding2");
			GunHelper.UpdateCurrentGunStats(inv.GetParentItem());
		};
	}

	public static void RegisterExclusiveGroup(string group, params string[] modIds)
	{
		foreach (string key in modIds)
		{
			exclusiveGroupByModId[key] = group;
		}
	}

	public static string GetExclusiveGroup(GameItem mod)
	{
		if (mod == null || mod.identifier == null)
		{
			return null;
		}
		if (!exclusiveGroupByModId.TryGetValue(mod.identifier, out var value))
		{
			return null;
		}
		return value;
	}

	public static bool HasConflictingMod(GameItem gun, GameItem mod, GameInventory targetSlot, bool sameModConflicts)
	{
		if (gun == null || mod == null)
		{
			return false;
		}
		string exclusiveGroup = GetExclusiveGroup(mod);
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(gun))
		{
			if (item != mod && item.parentInventory != targetSlot)
			{
				if (sameModConflicts && item.IsID(mod.identifier))
				{
					return true;
				}
				if (exclusiveGroup != null && GetExclusiveGroup(item) == exclusiveGroup)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsGunAlreadyHaveThisMod(GameItem gun, GameItem mod)
	{
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(gun))
		{
			if (item.IsID(mod.identifier))
			{
				return true;
			}
		}
		return false;
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("WEAPON_MOD_TAG"))
		{
			CreateTooltipElement(item, builder, "GUN_FIREPOWER_TAG", "MOD_BASE_FIREPOWER_TAG");
			CreateTooltipElement(item, builder, "GUN_STYLE_TAG", "MOD_BASE_STYLE_TAG");
			CreateTooltipElement(item, builder, "GUN_RELIABILITY_TAG", "MOD_BASE_RELIABILITY_TAG");
			CreateTooltipElement(item, builder, "GUN_RECOIL_TAG", "MOD_BASE_RECOIL_TAG");
			CreateTooltipElement(item, builder, "GUN_ERGONOMICS_TAG", "MOD_BASE_ERGONOMICS_TAG");
			CreateTooltipElement(item, builder, "GUN_ACCURACY_TAG", "MOD_BASE_ACCURACY_TAG");
		}
	}

	public static void CreateTooltipElement(GameItem item, RichTextBuilder builder, string tag, string baseTag)
	{
		int @int = item.GetTagReadonly(tag).GetInt();
		int int2 = item.GetTagReadonly(baseTag).GetInt();
		if (int2 != 0)
		{
			builder.Add(" " + RNG.FormatIntDisplay(int2) + " " + GunHelper.IDToString(tag), GetColor(int2));
			builder.AddLine();
		}
		if (@int != 0)
		{
			builder.Add(" " + RNG.FormatPercentage(@int) + " " + GunHelper.IDToString(tag), GetColor(@int));
			builder.AddLine();
		}
	}

	public static RenderHandler.ColorPalette GetColor(int value)
	{
		RenderHandler.ColorPalette result = RenderHandler.ColorPalette.LightOrange;
		if (value > 0)
		{
			result = RenderHandler.ColorPalette.LightGreen;
		}
		return result;
	}

	public static bool MayModGun(GameItem gun)
	{
		if (!GeneralHelper.IsItemOwned(gun))
		{
			return GunHelper.IsServiceGun(gun);
		}
		return true;
	}

	public static void AllowOnlyWeapon(string[] weaponIds, GameItem item)
	{
		item.mayInventoryAddItemFunc = delegate(GameItem _, GameInventory inv)
		{
			GameItem parentItem = inv.GetParentItem();
			if (parentItem != null && MayModGun(parentItem) && parentItem.IsTag("ADV_GUN_TAG"))
			{
				if (parentItem.IsTag("CONTAINER_TAG"))
				{
					return true;
				}
				string[] array = weaponIds;
				foreach (string id in array)
				{
					if (parentItem.IsID(id))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		};
	}

	public static void PlayEffect(string effectID)
	{
		if (effectID != null && !(effectID == "") && AudioManager.Instance.CanPlay(effectID))
		{
			AudioManager.Instance.Play2(effectID);
		}
	}
}

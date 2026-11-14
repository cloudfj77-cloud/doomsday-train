using System.Collections.Generic;
using UnityEngine;

public class GunModDirectory : ItemDirectory
{
	private readonly string[] ALLOW_GUN_PISTOL = new string[4] { "printed_gun", "handmade_pistol", "heavy_handmade_pistol", "glock_receiver" };

	private readonly string[] ALLOW_GUN_BARREL = new string[1] { "smg" };

	private readonly string[] ALLOW_GUN_UNDERBARREL = new string[1] { "smg" };

	private readonly string[] ALLOW_GUN_GRIP = new string[2] { "smg", "shotgun" };

	private readonly string[] ALLOW_GUN_10MM_PISTOL = new string[2] { "heavy_handmade_pistol", "glock_receiver" };

	private readonly string[] ALLOW_GUN_SMG = new string[1] { "smg" };

	private readonly string[] ALLOW_GUN_22 = new string[2] { "printed_gun", "handmade_pistol" };

	private readonly string[] ALLOW_GUN_STOCK = new string[2] { "smg", "shotgun" };

	protected override void InitDirectory()
	{
		Add("rds", () => CreateRDS());
		Add("rds_makeshift", () => CreateRDSMakeshift());
		Add("silencer", () => CreateSilencer());
		Add("silencer2", () => CreateSilencerAdv());
		Add("silencer_makeshift", () => CreateSilencerMakeshift());
		Add("barrel", () => CreateBarrel());
		Add("compensator", () => CreateCompensator());
		Add("mag_10mm", () => Create10MMMag());
		Add("mag_10mm_high", () => Create10MMMagHigh());
		Add("mag_10mm_ext", () => Create10MMMagExt());
		Add("mag_22", () => Create22Mag());
		Add("mag_22_high", () => Create22MagHigh());
		Add("mag_22_ext", () => Create22MagExt());
		Add("mag_10mm_smg", () => Create10MMMagSMG());
		Add("mag_10mm_smg_high", () => Create10MMMagSMGHigh());
		Add("mag_10mm_smg_box", () => Create10MMMagSMGBox());
		Add("laser", () => CreateLaser());
		Add("laser2", () => CreateLaser2());
		Add("laser_toy", () => CreateLaserToy());
		Add("bipod", () => CreateBipod());
		Add("gl", () => CreateGL());
		Add("grip", () => CreateGrip());
		Add("stock", () => CreateStock());
		Add("short_stock", () => CreateShortStock());
		Add("internal_springs", () => CreateInternalSprings());
		Add("internal_springs_improved", () => CreateInternalSpringsImproved());
		Add("trigger_group", () => CreateTriggerGroup());
		Add("match_barrel", () => CreateMatchBarrel());
		Add("pistol_switch", () => CreatePistolSwitch());
		Add("ammo_display", () => CreateAmmoDisplay());
		Add("grip_gel", () => CreateGripGel());
		GunModHelper.RegisterExclusiveGroup("internal_springs", "internal_springs", "internal_springs_improved");
	}

	private GameItem CreateInternalSprings()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "special_mod");
		gameItem.name = LocHelper.GetLocalizedItem("item_internal_springs_name");
		gameItem.unitValue = 40L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_internal_springs_desc");
		GunModHelper.InitGunModSpecial(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 0, 25);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 0, 10);
		return gameItem;
	}

	private GameItem CreateInternalSpringsImproved()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "special_mod1");
		gameItem.name = LocHelper.GetLocalizedItem("item_internal_springs_improved_name");
		gameItem.unitValue = 20L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_internal_springs_improved_desc");
		GunModHelper.InitGunModSpecial(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 0, 12);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 0, 5);
		return gameItem;
	}

	private GameItem CreateTriggerGroup()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "special_mod");
		gameItem.name = LocHelper.GetLocalizedItem("item_trigger_group_name");
		gameItem.unitValue = 50L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_trigger_group_desc");
		GunModHelper.InitGunModSpecial(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 0, 0, 0, 25);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 0, 0, 0, 8);
		return gameItem;
	}

	private GameItem CreateMatchBarrel()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "special_mod");
		gameItem.name = LocHelper.GetLocalizedItem("item_match_barrel_name");
		gameItem.unitValue = 75L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_match_barrel_desc");
		GunModHelper.InitGunModSpecial(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 0, 0, 0, 0, 35);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 0, 0, 0, 0, 10);
		return gameItem;
	}

	private GameItem CreatePistolSwitch()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "special_mod");
		gameItem.name = LocHelper.GetLocalizedItem("item_pistol_switch_name");
		gameItem.unitValue = 40L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_pistol_switch_desc");
		GunModHelper.InitGunModSpecial(gameItem);
		GunModHelper.InitGunMod(gameItem, 90, 0, -25, -90);
		GunModHelper.InitGunModBaseStats(gameItem, 25, 0, -8, -25);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_PISTOL, gameItem);
		return gameItem;
	}

	private GameItem CreateAmmoDisplay()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "special_mod");
		gameItem.name = LocHelper.GetLocalizedItem("item_ammo_display_name");
		gameItem.unitValue = 70L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_ammo_display_desc");
		GunModHelper.InitGunModSpecial(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 30, 15, 0, 15);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 9, 5, 0, 5);
		return gameItem;
	}

	private GameItem CreateGripGel()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "special_mod");
		gameItem.name = LocHelper.GetLocalizedItem("item_grip_gel_name");
		gameItem.unitValue = 70L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_grip_gel_desc");
		GunModHelper.InitGunModSpecial(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 0, 0, 0, 30);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 0, 0, 0, 9);
		return gameItem;
	}

	private GameItem CreateRDS()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "rds_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_rds_name");
		gameItem.unitValue = 60L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_rds_desc");
		GunModHelper.InitGunModRail(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 20, 0, 0, 0, 25);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 5, 0, 0, 0, 10);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetRDSMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static void SetRDSMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("printed_gun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "rds_view2"), new Vector2(16f, 4f)));
		}
		if (parent.IsID("handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "rds_view2"), new Vector2(16f, 5f)));
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "rds_view2"), new Vector2(16f, 4f)));
		}
		if (parent.IsID("glock_receiver"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "rds_view2"), new Vector2(16f, 4f)));
		}
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "rds_view2"), new Vector2(32f, 5f)));
		}
		if (parent.IsID("shotgun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "rds_view2"), new Vector2(32f, 4f)));
		}
	}

	private GameItem CreateRDSMakeshift()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "rds_makeshift_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_rds_makeshift_name");
		gameItem.unitValue = 25L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_rds_makeshift_desc");
		GunModHelper.InitGunModRail(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, -5, -15, 0, 0, 15);
		GunModHelper.InitGunModBaseStats(gameItem, 0, -2, -5, 0, 0, 5);
		gameItem.SetGameItemType("MAKESHIFT");
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetMakeshiftRDSMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static void SetMakeshiftRDSMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("printed_gun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "rds_makeshift_view"), new Vector2(17f, 5f)));
		}
		if (parent.IsID("handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "rds_makeshift_view"), new Vector2(16f, 7f)));
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "rds_makeshift_view"), new Vector2(16f, 5f)));
		}
		if (parent.IsID("glock_receiver"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "rds_makeshift_view"), new Vector2(20f, 5f)));
		}
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "rds_makeshift_view"), new Vector2(32f, 5f)));
		}
		if (parent.IsID("shotgun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "rds_makeshift_view"), new Vector2(38f, 5f)));
		}
	}

	private GameItem CreateSilencer()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "silencer_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_silencer_name");
		gameItem.unitValue = 55L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_silencer_desc");
		GunModHelper.InitGunModBarrel(gameItem, isSuppressor: true);
		GunModHelper.InitGunMod(gameItem, 0, 25, 0, 15, -5);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 8, 0, 5, -2);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => SetSilencerModShape(parent, item);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetSilencerMod(parent, list);
			return list;
		};
		return gameItem;
	}

	private static GridShapeBuilder SetSilencerModShape(GameItem parent, GameItem item)
	{
		if (parent.IsID("printed_gun"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("handmade_pistol"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("glock_receiver"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("smg"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(7, 0, flipped: false, 0);
		}
		if (parent.IsID("shotgun"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(7, 0, flipped: false, 0);
		}
		return null;
	}

	private static void SetSilencerMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("printed_gun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "silencer_view"), new Vector2(62f, -3f)));
		}
		if (parent.IsID("handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "silencer_view"), new Vector2(60f, -1f)));
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "silencer_view"), new Vector2(62f, -3f)));
		}
		if (parent.IsID("glock_receiver"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "silencer_view"), new Vector2(63f, -4f)));
		}
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "silencer_view"), new Vector2(109f, -6f)));
		}
		if (parent.IsID("shotgun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "silencer_view"), new Vector2(109f, -4f)));
		}
	}

	private GameItem CreateSilencerAdv()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "silencer2_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_silencer2_name");
		gameItem.unitValue = 90L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_silencer2_desc");
		GunModHelper.InitGunModBarrel(gameItem, isSuppressor: true);
		GunModHelper.InitGunMod(gameItem, 0, 40, 0, 25, -10);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 12, 0, 8, -3);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => SetSilencerAdvShape(parent, item);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetSilencerAdvMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static GridShapeBuilder SetSilencerAdvShape(GameItem parent, GameItem item)
	{
		if (parent.IsID("printed_gun"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("handmade_pistol"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("glock_receiver"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("smg"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(7, 0, flipped: false, 0);
		}
		if (parent.IsID("shotgun"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(7, 0, flipped: false, 0);
		}
		return null;
	}

	public static void SetSilencerAdvMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("printed_gun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "silencer2_view"), new Vector2(62f, -5f)));
		}
		if (parent.IsID("handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "silencer2_view"), new Vector2(60f, -2f)));
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "silencer2_view"), new Vector2(61f, -4f)));
		}
		if (parent.IsID("glock_receiver"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "silencer2_view"), new Vector2(62f, -5f)));
		}
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "silencer2_view"), new Vector2(109f, -6f)));
		}
		if (parent.IsID("shotgun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "silencer2_view"), new Vector2(109f, -5f)));
		}
	}

	private GameItem CreateSilencerMakeshift()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "bottle_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_silencer_makeshift_name");
		gameItem.unitValue = 15L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_silencer_makeshift_desc");
		GunModHelper.InitGunModBarrel(gameItem, isSuppressor: true);
		GunModHelper.InitGunMod(gameItem, 0, -50, -35, 10, -30);
		GunModHelper.InitGunModBaseStats(gameItem, 0, -15, -10, 3, -9);
		gameItem.SetGameItemType("MAKESHIFT");
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => SetSilencerMakeshiftShape(parent, item);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetSilencerMakeshiftMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static GridShapeBuilder SetSilencerMakeshiftShape(GameItem parent, GameItem item)
	{
		if (parent.IsID("printed_gun"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("handmade_pistol"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("glock_receiver"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("smg"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(7, 0, flipped: false, 0);
		}
		if (parent.IsID("shotgun"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(7, 0, flipped: false, 0);
		}
		return null;
	}

	public static void SetSilencerMakeshiftMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("printed_gun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "bottle_view"), new Vector2(62f, -2f)));
		}
		if (parent.IsID("handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "bottle_view"), new Vector2(61f, -1f)));
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "bottle_view"), new Vector2(62f, -2f)));
		}
		if (parent.IsID("glock_receiver"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "bottle_view"), new Vector2(62f, -3f)));
		}
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "bottle_view"), new Vector2(109f, -3f)));
		}
		if (parent.IsID("shotgun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "bottle_view"), new Vector2(109f, -3f)));
		}
	}

	private GameItem CreateBarrel()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "barrel_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_barrel_name");
		gameItem.unitValue = 60L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_barrel_desc");
		GunModHelper.InitGunModBarrel(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 0, 0, 0, -20, 25);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 0, 0, 0, -6, 8);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_BARREL, gameItem);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => SetBarrelShape(parent, item);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetBarrelMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static GridShapeBuilder SetBarrelShape(GameItem parent, GameItem item)
	{
		if (parent.IsID("smg"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(7, 0, flipped: false, 0);
		}
		return null;
	}

	public static void SetBarrelMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "barrel_view"), new Vector2(109f, -3f)));
		}
	}

	private GameItem CreateCompensator()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "compensator_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_compensator_name");
		gameItem.unitValue = 50L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_compensator_desc");
		GunModHelper.InitGunModBarrel(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 10, 0, 30, -5);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 3, 0, 9, -2);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => SetCompensatorShape(parent, item);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetCompensatorMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static GridShapeBuilder SetCompensatorShape(GameItem parent, GameItem item)
	{
		if (parent.IsID("printed_gun"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("handmade_pistol"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("glock_receiver"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(4, 0, flipped: false, 0);
		}
		if (parent.IsID("smg"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(7, 0, flipped: false, 0);
		}
		if (parent.IsID("shotgun"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(7, 0, flipped: false, 0);
		}
		return null;
	}

	public static void SetCompensatorMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("printed_gun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "compensator_view"), new Vector2(63f, -5f)));
		}
		if (parent.IsID("handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "compensator_view"), new Vector2(61f, -4f)));
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "compensator_view"), new Vector2(62f, -5f)));
		}
		if (parent.IsID("glock_receiver"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "compensator_view"), new Vector2(63f, -5f)));
		}
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "compensator_view"), new Vector2(109f, -6f)));
		}
		if (parent.IsID("shotgun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "compensator_view"), new Vector2(109f, -5f)));
		}
	}

	private GameItem CreateLaser()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "laser_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_laser_name");
		gameItem.unitValue = 50L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_laser_desc");
		GunModHelper.InitGunModUnderRail(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 20, 0, 0, 20);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 6, 0, 0, 6);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem _, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetLaserMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static void SetLaserMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("printed_gun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser_view"), new Vector2(43f, -18f)));
		}
		if (parent.IsID("handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser_view"), new Vector2(37f, -15f)));
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser_view"), new Vector2(42f, -19f)));
		}
		if (parent.IsID("glock_receiver"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser_view"), new Vector2(43f, -21f)));
		}
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser_view"), new Vector2(88f, -18f)));
		}
		if (parent.IsID("shotgun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser_view"), new Vector2(91f, -19f)));
		}
	}

	private GameItem CreateLaser2()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "laser2_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_laser2_name");
		gameItem.unitValue = 100L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_laser2_desc");
		GunModHelper.InitGunModUnderRail(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 25, 0, 0, 25, 10);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 8, 0, 0, 8, 3);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem _, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetLaser2Mod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static void SetLaser2Mod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("printed_gun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser2_view"), new Vector2(43f, -18f)));
		}
		if (parent.IsID("handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser2_view"), new Vector2(37f, -15f)));
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser2_view"), new Vector2(42f, -19f)));
		}
		if (parent.IsID("glock_receiver"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser2_view"), new Vector2(43f, -21f)));
		}
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser2_view"), new Vector2(88f, -18f)));
		}
		if (parent.IsID("shotgun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser2_view"), new Vector2(91f, -19f)));
		}
	}

	private GameItem CreateLaserToy()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "laser_toy_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_laser_toy_name");
		gameItem.unitValue = 18L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_laser_toy_desc");
		GunModHelper.InitGunModUnderRail(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, -10, -15, 0, 15);
		GunModHelper.InitGunModBaseStats(gameItem, 0, -3, -5, 0, 5);
		gameItem.SetGameItemType("MAKESHIFT");
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem _, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetLaserToyMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static void SetLaserToyMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("printed_gun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser_toy_view"), new Vector2(36f, -14f)));
		}
		if (parent.IsID("handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser_toy_view"), new Vector2(34f, -10f)));
		}
		if (parent.IsID("heavy_handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser_toy_view"), new Vector2(34f, -14f)));
		}
		if (parent.IsID("glock_receiver"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser_toy_view"), new Vector2(35f, -17f)));
		}
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser_toy_view"), new Vector2(88f, -12f)));
		}
		if (parent.IsID("shotgun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "laser_toy_view"), new Vector2(77f, -20f)));
		}
	}

	private GameItem CreateBipod()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "bipod_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_bipod_name");
		gameItem.unitValue = 40L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_bipod_desc");
		GunModHelper.InitGunModUnderRail(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 0, 0, 40, -20);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 0, 0, 12, -6);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_UNDERBARREL, gameItem);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => parent.IsID("smg") ? new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(3, 2, flipped: false, 0) : null;
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			if (parent.IsID("smg"))
			{
				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "bipod_view"), new Vector2(58f, -17f)));
			}
			return list;
		};
		return gameItem;
	}

	private GameItem CreateGL()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "gl_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_gl_name");
		gameItem.unitValue = 150L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_gl_desc");
		GunModHelper.InitGunModUnderRail(gameItem);
		GunModHelper.InitGunMod(gameItem, 75, 30, 0, 25, -75);
		GunModHelper.InitGunModBaseStats(gameItem, 20, 9, 0, 8, -20);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_UNDERBARREL, gameItem);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => parent.IsID("smg") ? new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(3, 2, flipped: false, 0) : null;
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			if (parent.IsID("smg"))
			{
				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "gl_view"), new Vector2(54f, -17f)));
			}
			return list;
		};
		return gameItem;
	}

	private GameItem CreateGrip()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "grip_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_grip_name");
		gameItem.unitValue = 40L;
		GunModHelper.InitGunModUnderRail(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 15, 0, 15, 15);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 5, 0, 5, 5);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_GRIP, gameItem);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => SetGripShape(parent, item);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetGripMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static GridShapeBuilder SetGripShape(GameItem parent, GameItem item)
	{
		if (parent.IsID("smg"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width + 1, item.modifiedShape.height, 1).SetTransform(4, 1, flipped: false, 0);
		}
		if (parent.IsID("shotgun"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width + 1, item.modifiedShape.height, 1).SetTransform(3, 1, flipped: false, 0);
		}
		return null;
	}

	public static void SetGripMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "grip_view"), new Vector2(58f, -17f)));
		}
		if (parent.IsID("shotgun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "grip_view"), new Vector2(45f, -18f)));
		}
	}

	private GameItem Create10MMMag()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "10mm_mag_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_mag_10mm_name");
		gameItem.unitValue = 30L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_mag_10mm_desc");
		GunModHelper.InitGunModMagazine(gameItem);
		GunModHelper.InitGunMod(gameItem, 10);
		GunModHelper.InitGunModBaseStats(gameItem, 3);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_10MM_PISTOL, gameItem);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			Set10MMMagMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static void Set10MMMagMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("heavy_handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "10mm_mag_view"), new Vector2(-2f, -40f)));
		}
		if (parent.IsID("glock_receiver"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "10mm_mag_view"), new Vector2(-2f, -36f)));
		}
	}

	private GameItem Create10MMMagHigh()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "10mm_mag_high_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_mag_10mm_high_name");
		gameItem.unitValue = 40L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_mag_10mm_high_desc");
		GunModHelper.InitGunModMagazine(gameItem);
		GunModHelper.InitGunMod(gameItem, 20, 10, 0, 0, -10);
		GunModHelper.InitGunModBaseStats(gameItem, 6, 3, 0, 0, -3);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_10MM_PISTOL, gameItem);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			Set10MMMagExtMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static void Set10MMMagExtMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("heavy_handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "10mm_mag_view"), new Vector2(-2f, -40f)));
		}
		if (parent.IsID("glock_receiver"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "10mm_mag_view"), new Vector2(-2f, -36f)));
		}
	}

	private GameItem Create10MMMagExt()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "10mm_mag_ext_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_mag_10mm_ext_name");
		gameItem.unitValue = 50L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_mag_10mm_ext_desc");
		GunModHelper.InitGunModMagazine(gameItem);
		GunModHelper.InitGunMod(gameItem, 40, 20, 0, 0, -15);
		GunModHelper.InitGunModBaseStats(gameItem, 12, 6, 0, 0, -5);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_10MM_PISTOL, gameItem);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => Set10MMMagExtShape(parent, item);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			Set10MMMagExtModShape(parent, list);
			return list;
		};
		return gameItem;
	}

	public static GridShapeBuilder Set10MMMagExtShape(GameItem parent, GameItem item)
	{
		if (parent.IsID("heavy_handmade_pistol"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height + 1, 1).SetTransform(0, 0, flipped: false, 0);
		}
		if (parent.IsID("glock_receiver"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height + 1, 1).SetTransform(0, 0, flipped: false, 0);
		}
		return null;
	}

	public static void Set10MMMagExtModShape(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("heavy_handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "10mm_mag_ext_view"), new Vector2(-3f, -34f)));
		}
		if (parent.IsID("glock_receiver"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "10mm_mag_ext_view"), new Vector2(-2f, -31f)));
		}
	}

	private GameItem Create10MMMagSMG()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "10mm_mag_smg_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_mag_10mm_smg_name");
		gameItem.EnableManufacturer("meridian");
		gameItem.unitValue = 55L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_mag_10mm_smg_desc");
		GunModHelper.InitGunModMagazine(gameItem);
		GunModHelper.InitGunMod(gameItem, 12);
		GunModHelper.InitGunModBaseStats(gameItem, 4);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_SMG, gameItem);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => parent.IsID("smg") ? new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(3, 1, flipped: false, 0) : null;
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			if (parent.IsID("smg"))
			{
				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "10mm_mag_smg_view"), new Vector2(40f, -27f)));
			}
			return list;
		};
		return gameItem;
	}

	private GameItem Create10MMMagSMGHigh()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "10mm_mag_smg_high_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_mag_10mm_smg_high_name");
		gameItem.EnableManufacturer("meridian");
		gameItem.unitValue = 70L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_mag_10mm_smg_high_desc");
		GunModHelper.InitGunModMagazine(gameItem);
		GunModHelper.InitGunMod(gameItem, 25, 10, 0, 0, -10);
		GunModHelper.InitGunModBaseStats(gameItem, 8, 3, 0, 0, -3);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_SMG, gameItem);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => parent.IsID("smg") ? new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height, 1).SetTransform(2, 2, flipped: false, 0) : null;
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			if (parent.IsID("smg"))
			{
				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "10mm_mag_smg_high_view"), new Vector2(41f, -27f)));
			}
			return list;
		};
		return gameItem;
	}

	private GameItem Create10MMMagSMGBox()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "10mm_mag_smg_box_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_mag_10mm_smg_box_name");
		gameItem.EnableManufacturer("meridian");
		gameItem.unitValue = 85L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_mag_10mm_smg_box_desc");
		GunModHelper.InitGunModMagazine(gameItem);
		GunModHelper.InitGunMod(gameItem, 75, 20, -15, 0, -20);
		GunModHelper.InitGunModBaseStats(gameItem, 20, 6, -5, 0, -6);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_SMG, gameItem);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => SetSMGMagShape(parent, item);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetSMGMagMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static GridShapeBuilder SetSMGMagShape(GameItem parent, GameItem item)
	{
		if (parent.IsID("smg"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height + 1, 1).SetTransform(2, 1, flipped: false, 0);
		}
		return null;
	}

	public static void SetSMGMagMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "10mm_mag_smg_box_item"), new Vector2(33f, -25f)));
		}
	}

	private GameItem Create22Mag()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "22_mag_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_mag_22_name");
		gameItem.unitValue = 15L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_mag_22_desc");
		GunModHelper.InitGunModMagazine(gameItem);
		GunModHelper.InitGunMod(gameItem, 15);
		GunModHelper.InitGunModBaseStats(gameItem, 5);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_22, gameItem);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			Set22MagMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static void Set22MagMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("printed_gun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "22_mag_view"), new Vector2(-2f, -38f)));
		}
		if (parent.IsID("handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "22_mag_view"), new Vector2(0f, -37f)));
		}
	}

	private GameItem Create22MagHigh()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "22_mag_high_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_mag_22_high_name");
		gameItem.unitValue = 25L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_mag_22_high_desc");
		GunModHelper.InitGunModMagazine(gameItem);
		GunModHelper.InitGunMod(gameItem, 30, 10, 0, 0, -10);
		GunModHelper.InitGunModBaseStats(gameItem, 9, 3, 0, 0, -3);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_22, gameItem);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			Set22MagHighMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static void Set22MagHighMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("printed_gun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "22_mag_view"), new Vector2(-2f, -38f)));
		}
		if (parent.IsID("handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "22_mag_view"), new Vector2(0f, -37f)));
		}
	}

	private GameItem Create22MagExt()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "22_mag_ext_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_mag_22_ext_name");
		gameItem.unitValue = 35L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_mag_22_ext_desc");
		GunModHelper.InitGunModMagazine(gameItem);
		GunModHelper.InitGunMod(gameItem, 60, 20, 0, 0, -20);
		GunModHelper.InitGunModBaseStats(gameItem, 18, 6, 0, 0, -6);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_22, gameItem);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => Set22MagExtShape(parent, item);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			Set22MagExtModShape(parent, list);
			return list;
		};
		return gameItem;
	}

	public static GridShapeBuilder Set22MagExtShape(GameItem parent, GameItem item)
	{
		if (parent.IsID("printed_gun"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height + 1, 1).SetTransform(0, 0, flipped: false, 0);
		}
		if (parent.IsID("handmade_pistol"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height + 1, 1).SetTransform(0, 0, flipped: false, 0);
		}
		return null;
	}

	public static void Set22MagExtModShape(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("printed_gun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "22_mag_ext_view"), new Vector2(-1f, -39f)));
		}
		if (parent.IsID("handmade_pistol"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "22_mag_ext_view"), new Vector2(1f, -38f)));
		}
	}

	private GameItem CreateStock()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "stock_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_stock_name");
		gameItem.EnableManufacturer("meridian");
		gameItem.unitValue = 40L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_stock_desc");
		GunModHelper.InitGunModStock(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 20, 0, 50, -30);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 6, 0, 15, -9);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_STOCK, gameItem);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => SetStockShape(parent, item);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetStockMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static GridShapeBuilder SetStockShape(GameItem parent, GameItem item)
	{
		if (parent.IsID("shotgun"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height + 1, 1).SetTransform(-2, 0, flipped: false, 0);
		}
		if (parent.IsID("smg"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height + 1, 1).SetTransform(-2, 0, flipped: false, 0);
		}
		return null;
	}

	public static void SetStockMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("shotgun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "stock_view"), new Vector2(-36f, -4f)));
		}
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "stock_view"), new Vector2(-40f, -3f)));
		}
	}

	private GameItem CreateShortStock()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_mod", "short_stock_item");
		gameItem.name = LocHelper.GetLocalizedItem("item_short_stock_name");
		gameItem.EnableManufacturer("meridian");
		gameItem.unitValue = 35L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_short_stock_desc");
		GunModHelper.InitGunModStock(gameItem);
		GunModHelper.InitGunMod(gameItem, 0, 20, 0, 20, 10);
		GunModHelper.InitGunModBaseStats(gameItem, 0, 6, 0, 6, 3);
		GunModHelper.AllowOnlyWeapon(ALLOW_GUN_STOCK, gameItem);
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent) => SetShortStockShape(parent, item);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem parent)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			SetShortStockMod(parent, list);
			return list;
		};
		return gameItem;
	}

	public static GridShapeBuilder SetShortStockShape(GameItem parent, GameItem item)
	{
		if (parent.IsID("shotgun"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height + 1, 1).SetTransform(-1, 0, flipped: false, 0);
		}
		if (parent.IsID("smg"))
		{
			return new GridShapeBuilder().SetDataFill(item.modifiedShape.width, item.modifiedShape.height + 1, 1).SetTransform(-1, 0, flipped: false, 0);
		}
		return null;
	}

	public static void SetShortStockMod(GameItem parent, List<ItemSpriteModifier> list)
	{
		if (parent.IsID("shotgun"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "short_stock_view"), new Vector2(-21f, -4f)));
		}
		if (parent.IsID("smg"))
		{
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_mod", "short_stock_view"), new Vector2(-24f, -4f), 0, isFlipped: false, -1));
		}
	}
}

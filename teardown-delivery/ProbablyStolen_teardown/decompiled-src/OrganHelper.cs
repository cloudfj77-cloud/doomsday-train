using System;
using UnityEngine;

public class OrganHelper : MonoBehaviour
{
	public const string ORGAN_ITEM_TAG = "ORGAN_ITEM_TAG";

	public const string ORGAN_SLOT_TAG = "ORGAN_SLOT_TAG";

	public const string ORGAN_SLOT_TYPE = "ORGAN_SLOT_TYPE";

	public const string HEART_SLOT_ITEM = "HEART_SLOT_ITEM";

	public const string LUNG_SLOT_ITEM = "LUNG_SLOT_ITEM";

	public const string KIDNEY_SLOT_ITEM = "KIDNEY_SLOT_ITEM";

	public const string LIVER_SLOT_ITEM = "LIVER_SLOT_ITEM";

	public const string ORGAN_SLOT_INFO_LEFT = "ORGAN_SLOT_INFO_LEFT_TAG";

	public const string ORGAN_SLOT_INFO_RIGHT = "ORGAN_SLOT_INFO_RIGHT_TAG";

	public const string ORGAN_HEALTH_INT = "ORGAN_HEALTH_INT";

	public const string ORGAN_HEALTH_MAX_INT = "ORGAN_HEALTH_MAX_INT";

	public const int ORGAN_DECAY_RATE = 10;

	public const string ORGANIC_ORGAN_TAG = "ORGANIC_ORGAN_TAG";

	public const string HUMAN_ORGAN_TAG = "HUMAN_ORGAN_TAG";

	public const string SURGERY_ADD_TAG = "SURGERY_ADD_TAG";

	public const int HUMAN_HEART_DEFAULT_HEALTH = 35;

	public const int HUMAN_LUNG_DEFAULT_HEALTH = 50;

	public const int HUMAN_LIVER_DEFAULT_HEALTH = 40;

	public const int HUMAN_KIDNEY_DEFAULT_HEALTH = 60;

	public const string ORGAN_TRANSPORT_CONTAINER_TAG = "ORGAN_TRANSPORT_CONTAINER_TAG";

	public const string IS_ORGAN_STORE_IN_TRANSPORT_CONTAINER = "IS_ORGAN_STORE_IN_TRANSPORT_CONTAINER";

	public const string HARVESTED_ORGAN_TAG = "HARVESTED_ORGAN_TAG";

	public static void InitOrganItem(GameItem item)
	{
		item.EnableTag("ORGAN_ITEM_TAG");
		item.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => (item.IsTag("SURGERY_ADD_TAG") && inv.IsTag("ORGAN_SLOT_TAG")) || (!item.IsTag("SURGERY_ADD_TAG") && !inv.IsTag("ORGAN_SLOT_TAG"));
	}

	public static void InitHumanOrgan(GameItem item)
	{
		InitOrganItem(item);
		item.EnableTag("ORGANIC_ORGAN_TAG");
		item.EnableTag("HUMAN_ORGAN_TAG");
		item.onTurnEndSlotItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item.onTurnEndSlotItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			int current = item.GetTagReadonly("ORGAN_HEALTH_INT").GetInt();
			item.GetTagReadonly("ORGAN_HEALTH_MAX_INT").GetInt();
			if (!item.IsTag("IS_ORGAN_STORE_IN_TRANSPORT_CONTAINER") && !item.IsTag("SURGERY_ADD_TAG"))
			{
				current -= 10;
			}
			if (current <= 0)
			{
				parentInventory.Expel(item);
			}
			else
			{
				item.ModifyTag("ORGAN_HEALTH_INT", delegate(TagState state)
				{
					state.Enable().SetInt(current);
				});
			}
		});
	}

	public static void InitHeart(GameItem item)
	{
		InitHumanOrgan(item);
		item.EnableTag("HEART_SLOT_ITEM");
		item.SetType("HEART_SLOT_ITEM");
		item.ModifyTag("ORGAN_HEALTH_INT", delegate(TagState state)
		{
			state.Enable().SetInt(35);
		});
		item.ModifyTag("ORGAN_HEALTH_MAX_INT", delegate(TagState state)
		{
			state.Enable().SetInt(35);
		});
		item.ModifyTag("BIO_PRINTING_REQUIRED_WORK_INT", delegate(TagState state)
		{
			state.Enable().SetInt(100);
		});
	}

	public static void InitLung(GameItem item)
	{
		InitHumanOrgan(item);
		item.EnableTag("LUNG_SLOT_ITEM");
		item.SetType("LUNG_SLOT_ITEM");
		item.ModifyTag("ORGAN_HEALTH_INT", delegate(TagState state)
		{
			state.Enable().SetInt(50);
		});
		item.ModifyTag("ORGAN_HEALTH_MAX_INT", delegate(TagState state)
		{
			state.Enable().SetInt(50);
		});
		item.ModifyTag("BIO_PRINTING_REQUIRED_WORK_INT", delegate(TagState state)
		{
			state.Enable().SetInt(75);
		});
	}

	public static void InitLiver(GameItem item)
	{
		InitHumanOrgan(item);
		item.EnableTag("LIVER_SLOT_ITEM");
		item.SetType("LIVER_SLOT_ITEM");
		item.ModifyTag("ORGAN_HEALTH_INT", delegate(TagState state)
		{
			state.Enable().SetInt(40);
		});
		item.ModifyTag("ORGAN_HEALTH_MAX_INT", delegate(TagState state)
		{
			state.Enable().SetInt(40);
		});
		item.ModifyTag("BIO_PRINTING_REQUIRED_WORK_INT", delegate(TagState state)
		{
			state.Enable().SetInt(70);
		});
	}

	public static void InitKidney(GameItem item)
	{
		InitHumanOrgan(item);
		item.EnableTag("KIDNEY_SLOT_ITEM");
		item.SetType("KIDNEY_SLOT_ITEM");
		item.ModifyTag("ORGAN_HEALTH_INT", delegate(TagState state)
		{
			state.Enable().SetInt(60);
		});
		item.ModifyTag("ORGAN_HEALTH_MAX_INT", delegate(TagState state)
		{
			state.Enable().SetInt(60);
		});
		item.ModifyTag("BIO_PRINTING_REQUIRED_WORK_INT", delegate(TagState state)
		{
			state.Enable().SetInt(55);
		});
	}

	public static int GetOrganValue(GameItem item)
	{
		if (item.IsTag("ORGAN_ITEM_TAG"))
		{
			return 1;
		}
		return 0;
	}

	public static GameSlotInventory CreateHeartSlot()
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory("Items/items_construction", "steel_sheet_rough");
		gameSlotInventory.SetBackgroundFadeSprite("Items/items_construction", "steel_sheet_rough");
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 10;
		gameSlotInventory.SetTooltipName("[Circulatory Organ Slot]");
		gameSlotInventory.EnableTag("ORGAN_SLOT_TAG");
		string[] tags = new string[2] { "HEART_SLOT_ITEM", "SURGERY_ADD_TAG" };
		ContainerHelper.AllowOnlyTaggedItemsAnd(gameSlotInventory, tags);
		return gameSlotInventory;
	}

	public static GameSlotInventory CreateLungSlot(bool isLeft = true)
	{
		GameSlotInventory utilInv = new GameSlotInventory("Items/items_construction", "steel_sheet_rough");
		utilInv.SetBackgroundFadeSprite("Items/items_construction", "steel_sheet_rough");
		utilInv.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 10;
		utilInv.SetTooltipName("[Respiratory Organ Slot]");
		utilInv.EnableTag("ORGAN_SLOT_TAG");
		string[] tags = new string[2] { "LUNG_SLOT_ITEM", "SURGERY_ADD_TAG" };
		ContainerHelper.AllowOnlyTaggedItemsAnd(utilInv, tags);
		if (isLeft)
		{
			utilInv.EnableTag("ORGAN_SLOT_INFO_LEFT_TAG");
		}
		else
		{
			utilInv.EnableTag("ORGAN_SLOT_INFO_RIGHT_TAG");
		}
		GameSlotInventory gameSlotInventory = utilInv;
		gameSlotInventory.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameSlotInventory.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			if (addedItem.identifier.Equals("organ_lung"))
			{
				if (utilInv.IsTag("ORGAN_SLOT_INFO_LEFT_TAG"))
				{
					addedItem.SetSpriteAndShape("Items/items_organ", "organ_lung_left");
				}
				else
				{
					addedItem.SetSpriteAndShape("Items/items_organ", "organ_lung_right");
				}
			}
		});
		return utilInv;
	}

	public static GameSlotInventory CreateLiverSlot()
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory("Items/items_construction", "steel_sheet_rough");
		gameSlotInventory.SetBackgroundFadeSprite("Items/items_construction", "steel_sheet_rough");
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 10;
		gameSlotInventory.SetTooltipName("[Metabolism Organ Slot]");
		gameSlotInventory.EnableTag("ORGAN_SLOT_TAG");
		string[] tags = new string[2] { "LIVER_SLOT_ITEM", "SURGERY_ADD_TAG" };
		ContainerHelper.AllowOnlyTaggedItemsAnd(gameSlotInventory, tags);
		return gameSlotInventory;
	}

	public static GameSlotInventory CreateKidneySlot(bool isLeft = true)
	{
		GameSlotInventory utilInv = new GameSlotInventory("Items/items_construction", "steel_sheet_rough");
		utilInv.SetBackgroundFadeSprite("Items/items_construction", "steel_sheet_rough");
		utilInv.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 10;
		utilInv.SetTooltipName("[Filtration Organ Slot]");
		utilInv.EnableTag("ORGAN_SLOT_TAG");
		if (isLeft)
		{
			utilInv.EnableTag("ORGAN_SLOT_INFO_LEFT_TAG");
		}
		else
		{
			utilInv.EnableTag("ORGAN_SLOT_INFO_RIGHT_TAG");
		}
		string[] tags = new string[2] { "KIDNEY_SLOT_ITEM", "SURGERY_ADD_TAG" };
		ContainerHelper.AllowOnlyTaggedItemsAnd(utilInv, tags);
		GameSlotInventory gameSlotInventory = utilInv;
		gameSlotInventory.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameSlotInventory.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			if (addedItem.identifier.Equals("organ_kidney"))
			{
				if (utilInv.IsTag("ORGAN_SLOT_INFO_LEFT_TAG"))
				{
					addedItem.SetSpriteAndShape("Items/items_organ", "organ_kidney_left");
				}
				else
				{
					addedItem.SetSpriteAndShape("Items/items_organ", "organ_kidney_right");
				}
			}
		});
		return utilInv;
	}

	public static bool InstallOrgan(GameCharacterItem gci, GameItem item)
	{
		if (item.IsTag("ORGAN_ITEM_TAG"))
		{
			item.EnableTag("SURGERY_ADD_TAG");
			GraphUtils.TryAcceptAll(gci, item);
			return true;
		}
		return false;
	}

	public static bool RemoveOrgan(GameCharacterItem gci, GameItem item)
	{
		if (item.IsTag("ORGAN_ITEM_TAG"))
		{
			item.DisableTag("SURGERY_ADD_TAG");
			return true;
		}
		return false;
	}

	public static void HarvestOrgansFromDead(GameCharacterItem gci)
	{
	}

	public static void EquipHumanoidWithDefaultOrgan(GameCharacterItem gci)
	{
		InstallOrgan(gci, DirectoryMaster.Item("organ_lung"));
		InstallOrgan(gci, DirectoryMaster.Item("organ_heart"));
		InstallOrgan(gci, DirectoryMaster.Item("organ_lung"));
		InstallOrgan(gci, DirectoryMaster.Item("organ_kidney"));
		InstallOrgan(gci, DirectoryMaster.Item("organ_liver"));
		InstallOrgan(gci, DirectoryMaster.Item("organ_kidney"));
	}

	public static void CreateOrganItemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("ORGAN_ITEM_TAG"))
		{
			int @int = item.GetTagReadonly("ORGAN_HEALTH_INT").GetInt();
			int int2 = item.GetTagReadonly("ORGAN_HEALTH_MAX_INT").GetInt();
			builder.Add(" Health: ", RenderHandler.ColorPalette.LightCyan);
			builder.AddProgressBar((float)@int / (float)int2, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
			builder.AddLine("(" + @int + "/" + (float)int2 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			if (item.IsTag("HARVESTED_ORGAN_TAG") && !item.IsTag("SURGERY_ADD_TAG"))
			{
				builder.AddLine("Harvested Organ", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
			if (!item.IsTag("IS_ORGAN_STORE_IN_TRANSPORT_CONTAINER") && !item.IsTag("SURGERY_ADD_TAG"))
			{
				builder.AddLine("Deteriorating", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			}
		}
	}

	public static int GetOrganItemValue(GameItem item)
	{
		if (item.IsTag("ORGAN_ITEM_TAG"))
		{
			double num = item.GetTagReadonly("ORGAN_HEALTH_INT").GetInt();
			double num2 = item.GetTagReadonly("ORGAN_HEALTH_MAX_INT").GetInt();
			return (int)(num / num2 * 2.5 * num2);
		}
		return 0;
	}
}

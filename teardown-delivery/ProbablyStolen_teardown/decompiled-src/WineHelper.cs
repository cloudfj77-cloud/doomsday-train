using System.Collections.Generic;
using UnityEngine;

public class WineHelper
{
	public const int WINE_AGE_VALUE_MOD_PER_DAY = 3;

	public const string BERRY_CONDITION_INT = "BERRY_CONDITION_INT";

	public const string BERRY_VISUAL_VARIANT_INT = "BERRY_VISUAL_VARIANT_INT";

	public const string BERRY_TAG = "BERRY_TAG";

	public const string WINE_CONTAINER_TAG = "WINE_CONTAINER_TAG";

	public const string WINE_CONTAINER_STATE = "WINE_CONTAINER_STATE";

	public const int BASE_FERMENTATION_PERIOD = 4;

	public const string FERMENTATION_TIME_REMAINING_INT = "FERMENTATION_TIME_REMAINING_INT";

	public const string WINE_SCORE_INT = "WINE_SCORE_INT";

	public const string WINE_INGREDIENT_TAG = "WINE_INGREDIENT_TAG";

	public const string WINE_YEAST_TAG = "WINE_YEAST_TAG";

	public const string WINE_YEAST_POWER_INT = "WINE_YEAST_POWER_INT";

	public const string WINE_YEAST_REUSABLE_TAG = "WINE_YEAST_REUSABLE_TAG";

	public const string WINE_YEAST_ILLEGAL_TAG = "WINE_YEAST_ILLEGAL_TAG";

	public const string WINE_COUNTERFEIT_TAG = "WINE_COUNTERFEIT_TAG";

	public const string WINE_SPRITE_ATLAS = "Items/items_winemaking";

	public const int WINE_BOTTLE_CAPACITY_ML = 750;

	public const int FERMENTATION_MIN_ML = 749;

	public const int WINE_ALCOHOL_PERCENT = 20;

	public const int WINE_EXTRACT_PER_MILLE_MAX = 20;

	public const int WINE_COUNTERFEIT_EXTRACT_PER_MILLE = 8;

	public const int WINE_COUNTERFEIT_CONTAMINANT_PER_MILLE = 8;

	private static readonly int[] WATER_QUALITY_SCORE = new int[6] { 1, 1, 0, -1, -2, -3 };

	public static int GetWineQualityTier(GameItem item)
	{
		int @int = item.GetTagReadonly("WINE_SCORE_INT").GetInt();
		if (@int >= 6)
		{
			return 5;
		}
		if (@int == 5)
		{
			return 4;
		}
		if (@int >= 3)
		{
			return 3;
		}
		return @int switch
		{
			2 => 2, 
			1 => 1, 
			_ => 0, 
		};
	}

	public static int GetWaterQualityScore(GameItem bottle)
	{
		int purityArrayIndex = WaterFeatureHelper.GetPurityArrayIndex(WaterHelper.GetWaterPurity(bottle));
		return WATER_QUALITY_SCORE[Mathf.Clamp(purityArrayIndex, 0, WATER_QUALITY_SCORE.Length - 1)];
	}

	public static int GetWineExtractPerMille(GameItem bottle)
	{
		if (bottle.IsTag("WINE_COUNTERFEIT_TAG"))
		{
			return 8;
		}
		return GetWineQualityTier(bottle) * 20 / 5;
	}

	public static int GetWineContaminantPerMille(GameItem bottle)
	{
		if (!bottle.IsTag("WINE_COUNTERFEIT_TAG"))
		{
			return 0;
		}
		return 8;
	}

	public static void PourWineIntoContainer(GameItem bottle, GameItem container)
	{
		if (WaterHelper.CanPourVolume(container, 750))
		{
			int num = 750000;
			int num2 = num * 20 / 100;
			int num3 = num * GetWineExtractPerMille(bottle) / 1000;
			int num4 = num * GetWineContaminantPerMille(bottle) / 1000;
			WaterHelper.AddLiquid(container, "alcohol", num2);
			WaterHelper.AddLiquid(container, "wine_extract", num3);
			WaterHelper.AddLiquid(container, "chemical_contaminant", num4);
			WaterHelper.AddWater(container, 2, num - num2 - num3 - num4);
		}
	}

	public static bool CanModifyLiquid(GameItem item)
	{
		return item.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() == 0;
	}

	public static bool IsFermentingOrFinished(GameItem item)
	{
		if (item.IsTag("WINE_CONTAINER_TAG"))
		{
			return item.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() >= 1;
		}
		return false;
	}

	public static bool IsFinishedWine(GameItem item)
	{
		if (item.IsTag("WINE_CONTAINER_TAG"))
		{
			return item.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() == 2;
		}
		return false;
	}

	public static bool IsWaterRation(GameItem item)
	{
		if (!(item.identifier == "water_ration"))
		{
			return item.identifier == "water_ration_small";
		}
		return true;
	}

	private static int RationVolumeMl(GameItem ration)
	{
		if (!(ration.identifier == "water_ration_small"))
		{
			return 750;
		}
		return 375;
	}

	public static void PourRationIntoBottle(GameItem bottle, GameItem ration)
	{
		int num = RationVolumeMl(ration);
		WaterHelper.AddWater(bottle, 2, num * 1000, useRange: false, 0, 1, initWaterFeature: false);
		AudioHelper.PourWater();
		ration.Destroy();
		bottle.Validate();
	}

	public static void DrainBottle(GameItem bottle)
	{
		foreach (Liquid liquid in Liquid.Liquids)
		{
			bottle.ModifyTag(liquid.currentPart, delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
		bottle.Validate();
	}

	private static bool CanBottleSourceTarget(GameItem s, GameItem t)
	{
		if (!GeneralHelper.IsItemOwned(s))
		{
			return false;
		}
		if (CanModifyLiquid(s))
		{
			if (!t.IsTag("LIQUID_CONTAINER_TAG") || !GeneralHelper.IsItemOwned(t))
			{
				if (WaterHelper.IsDrain(t))
				{
					return !WaterHelper.IsEmpty(s);
				}
				return false;
			}
			return true;
		}
		if (WaterHelper.IsDrain(t))
		{
			return IsFermentingOrFinished(s);
		}
		if (IsFinishedWine(s))
		{
			return WaterHelper.CanPourVolume(t, 750);
		}
		return false;
	}

	private static bool CanFillBottle(GameItem source, GameItem bottle)
	{
		if (!CanModifyLiquid(bottle))
		{
			return false;
		}
		if (!GeneralHelper.IsItemOwned(bottle) || !GeneralHelper.IsItemOwned(source))
		{
			return false;
		}
		if (IsWaterRation(source))
		{
			return WaterHelper.CanPourVolume(bottle, RationVolumeMl(source));
		}
		if (source.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return !WaterHelper.IsFull(bottle);
		}
		return false;
	}

	private static void SetupBottleLiquidTransfer(GameItem item)
	{
		item.mayThisTargetItemFunc = (GameItem s, GameItem t) => CanBottleSourceTarget(s, t);
		item.canThisTargetItemFunc = (GameItem s, GameItem t) => CanBottleSourceTarget(s, t);
		item.onThisTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (WaterHelper.IsDrain(t))
			{
				if (CanModifyLiquid(s))
				{
					DrainBottle(s);
				}
				else
				{
					if (!IsFermentingOrFinished(s))
					{
						return;
					}
					ResetToEmptyBottle(s);
				}
				AudioHelper.DumpWater();
			}
			else if (CanModifyLiquid(s) && t.IsTag("LIQUID_CONTAINER_TAG"))
			{
				if (!WaterHelper.IsEmpty(s))
				{
					AudioHelper.PourWater();
				}
				WaterHelper.TransferLiquid(s, t);
			}
			else if (IsFinishedWine(s) && WaterHelper.CanPourVolume(t, 750))
			{
				PourWineIntoContainer(s, t);
				AudioHelper.PourWater();
				ResetToEmptyBottle(s);
			}
		};
		item.mayItemTargetThisFunc = (GameItem source, GameItem bottle) => CanFillBottle(source, bottle);
		item.canItemTargetThisFunc = (GameItem source, GameItem bottle) => CanFillBottle(source, bottle);
		item.onItemTargetThisFunc = delegate(GameItem source, GameItem bottle)
		{
			if (CanModifyLiquid(bottle) && IsWaterRation(source))
			{
				PourRationIntoBottle(bottle, source);
			}
		};
	}

	public static void InitWineBottle(GameItem item)
	{
		item.EnableTag("WINE_CONTAINER_TAG");
		AgableHelper.InitAgable(item);
		item.SetGameItemType("HOUSEHOLD_GOOD");
		item.EnableTag("LIQUID_CONTAINER_TAG");
		item.ModifyTag("LIQUID_CONTAINER_CAPACITY", delegate(TagState state)
		{
			state.Enable().SetInt(750000);
		});
		foreach (Liquid liquid in Liquid.Liquids)
		{
			item.ModifyTag(liquid.currentPart, delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
		ChemicalHelper.InitContainerTemp(item);
		ContainerHelper.AddContainerTooltipTag(item);
		SetupBottleLiquidTransfer(item);
		GameGridInventory gameGridInventory = new GameGridInventory(6, 2);
		ContainerHelper.AllowOnlyTaggedItemsOr(gameGridInventory, new string[2] { "WINE_INGREDIENT_TAG", "WINE_YEAST_TAG" }, ownedOnly: true);
		MachineryButton startButton = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_wine_start_fermenting"));
		startButton.SetCallback(delegate
		{
			TryStartFermentation(item);
			if (item.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() == 1)
			{
				startButton.SetText(LocHelper.GetLocalizedMechanic("mech_wine_fermenting_status"));
			}
		});
		PixelWindow contentWindow = MachineHelper.CreateMachineryWindow(true, MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_wine_label_ingredient"), gameGridInventory, "ingredients"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_blender_label_control"), startButton, "control"));
		item.onCycleEndSlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			switch (item.GetTagReadonly("WINE_CONTAINER_STATE").GetInt())
			{
			case 1:
			{
				int yeastPower = item.GetTagReadonly("WINE_YEAST_POWER_INT").GetInt();
				item.ModifyTag("FERMENTATION_TIME_REMAINING_INT", delegate(TagState s)
				{
					s.SetInt(s.GetInt() - yeastPower);
				});
				if (item.GetTagReadonly("FERMENTATION_TIME_REMAINING_INT").GetInt() <= 0)
				{
					FinishFermentation(item);
				}
				break;
			}
			case 2:
				OnAgeWine(item);
				break;
			}
		};
		item.getItemSpriteModifierForItemFunc = delegate(GameItem bottle)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			if (bottle.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() == 2)
			{
				string name = (bottle.IsTag("WINE_COUNTERFEIT_TAG") ? "wine_label_red" : ("wine_label" + GetWineQualityTier(bottle)));
				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_winemaking", name), new Vector2(0f, 0f), 0, isFlipped: false, 0, haveMask: false, Color.white));
			}
			return list;
		};
		item.mayActivateSlotItemFunc = (GameItem bottle, GameInventory _, SlotMarker _) => CanEmptyWineBottle(bottle);
		item.canActivateSlotItemFunc = (GameItem bottle, GameInventory _, SlotMarker _) => CanEmptyWineBottle(bottle);
		item.onActivateSlotItemFunc = delegate(GameItem bottle, GameInventory _, SlotMarker _)
		{
			EmptyWineBottle(bottle);
		};
		item.ModifyTag("WINE_CONTAINER_STATE", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		item.onLoaded = delegate(GameItem gameItem)
		{
			if (gameItem.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() != 0)
			{
				gameItem.DisableTag("LIQUID_CONTAINER_TAG");
				ContainerHelper.RemoveContainerTooltipTag(gameItem);
				gameItem.CloseContentWindow(includeChildren: true);
				gameItem.SetContentWindow(null);
				if (gameItem.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() == 2 && GetWineQualityTier(gameItem) > 0)
				{
					InitDrinkableWine(gameItem, gameItem.GetTagReadonly("ALCOHOL_VALUE").GetInt());
				}
				gameItem.Validate();
			}
		};
		item.SetContentWindow(contentWindow);
	}

	public static void InitDrinkableWine(GameItem item, int alcoholLevel)
	{
		ConsumableHelper.InitAlcohol(item, alcoholLevel);
		item.onActivateSlotItemFunc = delegate(GameItem bottle, GameInventory _, SlotMarker _)
		{
			PlayerStore.Instance.healthData.ConsumeAlcohol(bottle.GetTagReadonly("ALCOHOL_VALUE").GetInt());
			ResetToEmptyBottle(bottle);
		};
	}

	public static bool CanEmptyWineBottle(GameItem item)
	{
		if (GeneralHelper.IsItemOwned(item) && item.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() == 2 && !item.IsTag("WINE_COUNTERFEIT_TAG"))
		{
			return GetWineQualityTier(item) == 0;
		}
		return false;
	}

	public static void EmptyWineBottle(GameItem item)
	{
		if (CanEmptyWineBottle(item))
		{
			ResetToEmptyBottle(item);
		}
	}

	public static void ResetToEmptyBottle(GameItem item)
	{
		item.RemoveFeatureByCategory("CATEGORY_WINE_QUALITY");
		ContrabandHelper.RemoveContrabandStatus(item);
		item.RemoveGameItemType("ALCOHOL");
		item.RemoveGameItemType("SUBSTANCE");
		item.RemoveGameItemType("LUXURY_ITEM");
		item.DisableTag("WINE_COUNTERFEIT_TAG");
		item.DisableTag("ENDANGERMENT_ITEM_TAG");
		item.ModifyTag("ALCOHOL_VALUE", delegate(TagState s)
		{
			s.Disable().SetInt(0);
		});
		item.ModifyTag("WINE_SCORE_INT", delegate(TagState s)
		{
			s.Disable().SetInt(0);
		});
		AgableHelper.ResetAge(item);
		item.ModifyTag("FERMENTATION_TIME_REMAINING_INT", delegate(TagState s)
		{
			s.Disable().SetInt(0);
		});
		item.ModifyTag("WINE_YEAST_POWER_INT", delegate(TagState s)
		{
			s.Disable().SetInt(1);
		});
		item.name = LocHelper.GetLocalizedItem("item_wine_bottle_name");
		item.shortDescription = LocHelper.GetLocalizedItem("item_wine_bottle_desc");
		item.unitValue = 15L;
		item.SetSpriteAndShape("Items/items_winemaking", "wine_bottle");
		InitWineBottle(item);
		item.Validate();
	}

	public static bool TryStartFermentation(GameItem item)
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(item);
		int num = 0;
		bool flag = false;
		foreach (GameItem item2 in list)
		{
			if (item2.IsTag("WINE_INGREDIENT_TAG"))
			{
				num++;
			}
			if (item2.IsTag("WINE_YEAST_ILLEGAL_TAG"))
			{
				flag = true;
			}
		}
		if (num < 1 && !flag)
		{
			GenericConfirmationUIManager.Instance.OpenPanelSingleButton(LocHelper.GetLocalizedMechanic("mech_wine_no_ingredients"), LocHelper.GetLocalizedMainMenu("confirm"), delegate
			{
				GenericConfirmationUIManager.Instance.ClosePanel();
			});
			return false;
		}
		if (WaterHelper.GetCurrentCapacityML(item) < 749)
		{
			GenericConfirmationUIManager.Instance.OpenPanelSingleButton(LocHelper.GetLocalizedMechanic("mech_wine_no_water"), LocHelper.GetLocalizedMainMenu("confirm"), delegate
			{
				GenericConfirmationUIManager.Instance.ClosePanel();
			});
			return false;
		}
		StartFermentation(item);
		return true;
	}

	public static void StartFermentation(GameItem item)
	{
		if (item.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() != 0)
		{
			return;
		}
		item.ModifyTag("FERMENTATION_TIME_REMAINING_INT", delegate(TagState s)
		{
			s.Enable().SetInt(4);
		});
		item.ModifyTag("WINE_CONTAINER_STATE", delegate(TagState state)
		{
			state.SetInt(1);
		});
		int wineScore = item.GetTagReadonly("WINE_SCORE_INT").GetInt();
		bool flag = false;
		int num = 0;
		foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(item))
		{
			if (item2.IsTag("WINE_INGREDIENT_TAG"))
			{
				int num2 = item2.GetTagReadonly("BERRY_CONDITION_INT").GetInt();
				if (num2 < 0)
				{
					num2++;
				}
				wineScore += num2;
				num++;
			}
			if (item2.IsTag("WINE_YEAST_TAG"))
			{
				int yeastPower = item2.GetTagReadonly("WINE_YEAST_POWER_INT").GetInt();
				item.ModifyTag("WINE_YEAST_POWER_INT", delegate(TagState s)
				{
					s.Enable().SetInt(yeastPower);
				});
				flag = true;
				if (item2.IsTag("WINE_YEAST_ILLEGAL_TAG"))
				{
					item.EnableTag("WINE_COUNTERFEIT_TAG");
				}
			}
			if (item2.IsTag("WINE_YEAST_REUSABLE_TAG"))
			{
				item2.parentInventory?.Expel(item2);
				GraphUtils.EmporiumTryAdd(item2);
			}
			else
			{
				item2.Destroy();
			}
		}
		if (num >= 2)
		{
			wineScore++;
		}
		if (flag)
		{
			wineScore++;
		}
		wineScore += GetWaterQualityScore(item);
		item.ModifyTag("WINE_SCORE_INT", delegate(TagState s)
		{
			s.Enable().SetInt(wineScore);
		});
		if (!item.IsTag("WINE_YEAST_POWER_INT"))
		{
			item.ModifyTag("WINE_YEAST_POWER_INT", delegate(TagState s)
			{
				s.Enable().SetInt(1);
			});
		}
		DrainBottle(item);
		item.DisableTag("LIQUID_CONTAINER_TAG");
		ContainerHelper.RemoveContainerTooltipTag(item);
		item.RemoveGameItemType("HOUSEHOLD_GOOD");
		item.SetSpriteAndShape("Items/items_winemaking", "wine_fermenting");
		item.Validate();
		item.CloseContentWindow(includeChildren: true);
		item.SetContentWindow(null);
	}

	public static void FinishFermentation(GameItem item)
	{
		item.ModifyTag("WINE_CONTAINER_STATE", delegate(TagState state)
		{
			state.SetInt(2);
		});
		item.unitValue = 100L;
		if (item.IsTag("WINE_COUNTERFEIT_TAG"))
		{
			FinishCounterfeitWine(item);
			return;
		}
		item.name = LocHelper.GetLocalizedItem("item_wine_bloomberry_name");
		item.shortDescription = LocHelper.GetLocalizedItem("item_wine_bloomberry_desc");
		int @int = item.GetTagReadonly("WINE_SCORE_INT").GetInt();
		item.AddItemFeature(ItemFeatureList.CreateWineQualityFeature(@int));
		int wineQualityTier = GetWineQualityTier(item);
		if (wineQualityTier > 0)
		{
			InitDrinkableWine(item, 4 + wineQualityTier);
			item.SetGameItemType("SUBSTANCE");
			item.SetGameItemType("ALCOHOL");
			if (wineQualityTier >= 4)
			{
				item.SetGameItemType("LUXURY_ITEM");
			}
		}
		item.SetSpriteAndShape("Items/items_winemaking", "bottle" + wineQualityTier);
		item.Validate();
	}

	private static void FinishCounterfeitWine(GameItem item)
	{
		item.name = LocHelper.GetLocalizedItem("item_wine_gloomberry_name");
		item.shortDescription = LocHelper.GetLocalizedItem("item_wine_gloomberry_desc");
		ContrabandHelper.InitContrabandItem(item, 2);
		item.EnableTag("ENDANGERMENT_ITEM_TAG");
		item.unitValue = 44L;
		item.ModifyTag("WINE_SCORE_INT", delegate(TagState s)
		{
			s.Enable().SetInt(3);
		});
		InitDrinkableWine(item, 6);
		item.SetGameItemType("SUBSTANCE");
		item.SetGameItemType("ALCOHOL");
		item.SetSpriteAndShape("Items/items_winemaking", "bottle2");
		item.Validate();
	}

	public static GameItem CreateFinishedWineForDebug(int wineScore, bool counterfeit = false)
	{
		GameItem gameItem = DirectoryMaster.Item("wine_bottle");
		gameItem.ModifyTag("WINE_CONTAINER_STATE", delegate(TagState s)
		{
			s.Enable().SetInt(1);
		});
		gameItem.ModifyTag("WINE_SCORE_INT", delegate(TagState s)
		{
			s.Enable().SetInt(wineScore);
		});
		if (counterfeit)
		{
			gameItem.EnableTag("WINE_COUNTERFEIT_TAG");
		}
		DrainBottle(gameItem);
		gameItem.DisableTag("LIQUID_CONTAINER_TAG");
		ContainerHelper.RemoveContainerTooltipTag(gameItem);
		gameItem.RemoveGameItemType("HOUSEHOLD_GOOD");
		gameItem.CloseContentWindow(includeChildren: true);
		gameItem.SetContentWindow(null);
		FinishFermentation(gameItem);
		return gameItem;
	}

	public static void OnAgeWine(GameItem item)
	{
		if (!item.IsTag("WINE_COUNTERFEIT_TAG"))
		{
			AgableHelper.OnAge(item);
		}
	}

	public static void InitBerry(GameItem item)
	{
		item.EnableTag("BERRY_TAG");
		int condition = RNG.GetRandomInt(-2, 1);
		if (condition >= 0)
		{
			condition++;
		}
		item.ModifyTag("BERRY_CONDITION_INT", delegate(TagState state)
		{
			state.Enable().SetInt(condition);
		});
		item.ModifyTag("BERRY_VISUAL_VARIANT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(RNG.GetRandomInt(0, 2));
		});
		item.EnableTag("WINE_INGREDIENT_TAG");
	}

	public static void SetBerryCondition(GameItem item, int condition, int visualVariant = 0)
	{
		item.ModifyTag("BERRY_CONDITION_INT", delegate(TagState state)
		{
			state.Enable().SetInt(condition);
		});
		item.ModifyTag("BERRY_VISUAL_VARIANT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(visualVariant);
		});
	}

	public static void InitYeast(GameItem item, int power)
	{
		item.EnableTag("WINE_YEAST_TAG");
		item.ModifyTag("WINE_YEAST_POWER_INT", delegate(TagState state)
		{
			state.Enable().SetInt(power);
		});
	}

	public static void InitInfiniteYeast(GameItem item, int power)
	{
		InitYeast(item, power);
		item.EnableTag("WINE_YEAST_REUSABLE_TAG");
	}

	public static void InitIllegalYeast(GameItem item, int power)
	{
		InitYeast(item, power);
		item.EnableTag("WINE_YEAST_ILLEGAL_TAG");
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("WINE_CONTAINER_TAG") && item.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() == 0)
		{
			int currentCapacityML = WaterHelper.GetCurrentCapacityML(item);
			builder.AddLine(LocHelper.GetLocalizedMechanic("liquid_water") + ": " + currentCapacityML + " / " + 750 + " ml", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		if (GetWineQualityTier(item) > 0 && !item.IsTag("WINE_COUNTERFEIT_TAG"))
		{
			AgableHelper.AddAgingInfoTooltipLine(builder);
			if (AgableHelper.HasAged(item))
			{
				AgableHelper.AddAgeTooltipLine(builder, item);
			}
		}
		if (item.IsTag("WINE_SCORE_INT"))
		{
			if (item.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() == 1)
			{
				int @int = item.GetTagReadonly("FERMENTATION_TIME_REMAINING_INT").GetInt();
				int num = item.GetTagReadonly("WINE_YEAST_POWER_INT").GetInt();
				if (num < 1)
				{
					num = 1;
				}
				int num2 = (@int + num - 1) / num;
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_wine_fermenting_remaining", num2), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			if (PlayerStore.Instance.devMode)
			{
				int int2 = item.GetTagReadonly("WINE_SCORE_INT").GetInt();
				int int3 = item.GetTagReadonly("WINE_YEAST_POWER_INT").GetInt();
				builder.AddLine("Wine Score: " + int2, ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
				builder.AddLine("Yeast Power: " + int3, ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
			}
		}
		if (item.IsTag("BERRY_TAG") && PlayerStore.Instance.devMode)
		{
			builder.AddLine("Berry Quality: " + item.GetTagReadonly("BERRY_CONDITION_INT").GetInt(), ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
		}
	}
}

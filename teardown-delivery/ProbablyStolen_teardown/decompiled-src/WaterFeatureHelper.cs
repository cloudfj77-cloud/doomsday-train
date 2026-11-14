using System;

public class WaterFeatureHelper
{
	public static string PURE_WATER_DISPLAY = LocHelper.GetLocalizedMechanic("mech_condition_pure_water");

	public static string HIGH_QUALITY_WATER_DISPLAY = LocHelper.GetLocalizedMechanic("mech_condition_high_quality_water");

	public static string BASE_WATER_DISPLAY = LocHelper.GetLocalizedMechanic("mech_condition_basewater");

	public static string GHOSTWATER_DISPLAY = LocHelper.GetLocalizedMechanic("mech_condition_ghostwater");

	public static string RUST_WATER_DISPLAY = LocHelper.GetLocalizedMechanic("mech_condition_rustwater");

	public static string GUTTERFLOW_WATER_DISPLAY = LocHelper.GetLocalizedMechanic("mech_condition_gutterflow");

	public static string[] WATER_DISPLAY = new string[6] { PURE_WATER_DISPLAY, HIGH_QUALITY_WATER_DISPLAY, BASE_WATER_DISPLAY, GHOSTWATER_DISPLAY, RUST_WATER_DISPLAY, GUTTERFLOW_WATER_DISPLAY };

	public static readonly LocalizedLines WATER_DISPLAY_LOCALIZED = LocalizedLines.Mechanic("mech_condition_pure_water", "mech_condition_high_quality_water", "mech_condition_basewater", "mech_condition_ghostwater", "mech_condition_rustwater", "mech_condition_gutterflow");

	public const int PURE_WATER_INDEX = 0;

	public const int HIGH_QUALITY_WATER_INDEX = 1;

	public const int BASE_WATER_INDEX = 2;

	public const int GHOST_WATER_INDEX = 3;

	public const int RUST_WATER_INDEX = 4;

	public const int GUTTERFLOW_WATER_INDEX = 5;

	public const string PURE_WATER_VALUE_DISPLAY = "99.9%+";

	public const string HIGH_QUALITY_WATER_VALUE_DISPLAY = "99%-99.9%";

	public const string BASE_WATER_VALUE_DISPLAY = "98%-99%";

	public const string GHOST_WATER_VALUE_DISPLAY = "96%-98%";

	public const string RUST_WATER_VALUE_DISPLAY = "90%-96%";

	public const string GUTTERFLOW_WATER_VALUE_DISPLAY = "90%-";

	public static readonly string[] WATER_VALUE_DISPLAY = new string[6] { "99.9%+", "99%-99.9%", "98%-99%", "96%-98%", "90%-96%", "90%-" };

	public const int PURE_WATER_PRICE_PER_LITER = 200;

	public const int HIGH_QUALITY_WATER_PRICE_PER_LITER = 100;

	public const int BASE_WATER_PRICE_PER_LITER = 40;

	public const int GHOST_WATER_PRICE_PER_LITER = 27;

	public const int RUST_WATER_PRICE_PER_LITER = 20;

	public const int GUTTERFLOW_WATER_PRICE_PER_LITER = 10;

	public static readonly int[] WATER_PRICES = new int[6] { 200, 100, 40, 27, 20, 10 };

	public const int PURE_WATER_PURITY = 9990;

	public const int HIGH_QUALITY_WATER_PURITY = 9900;

	public const int BASE_WATER_PURITY = 9800;

	public const int GHOST_WATER_PURITY = 9600;

	public const int RUST_WATER_PURITY = 9000;

	public const int GUTTERFLOW_WATER_PURITY = 3500;

	public static readonly int[] WATER_PURITIES = new int[6] { 9990, 9900, 9800, 9600, 9000, 3500 };

	public const string WATER_PURITY_INCONCLUSIVE = "WATER_PURITY_INCONCLUSIVE";

	public static void OnLiquidContainerPourOutOf(GameItem gameItem)
	{
		if (!gameItem.IsTag("WINE_CONTAINER_TAG"))
		{
			int totalVolume = WaterHelper.GetTotalVolume(gameItem);
			_ = GetWaterFeature(gameItem).customIntValue1;
			gameItem.unitValue = GetWaterPrice(totalVolume);
			UpdateWaterFeature(gameItem, GetWaterFeature(gameItem).IsExposed());
			if (WaterHelper.IsEmpty(gameItem))
			{
				GetWaterFeature(gameItem).SetExposed(value: false);
				gameItem.DisableTag("STRIP_TESTED_TAG");
			}
			SpriteHelper.UpdateWaterContainerSprite(gameItem);
		}
	}

	public static void OnLiquidContainerPourInto(GameItem target, GameItem source, bool wasEmpty)
	{
		if (target.IsTag("WINE_CONTAINER_TAG"))
		{
			return;
		}
		bool flag = false;
		if (wasEmpty)
		{
			ItemFeature waterFeature = GetWaterFeature(source);
			int num = 0;
			ItemFeature waterFeature2 = GetWaterFeature(target);
			if (waterFeature.IsExposed())
			{
				num = (waterFeature2.customIntValue1 = WaterHelper.GetWaterPurity(source));
				waterFeature2.SetExposed(value: true);
				flag = true;
			}
			else
			{
				num = (waterFeature2.customIntValue1 = GetWaterFeature(source).customIntValue1);
				waterFeature2.SetExposed(value: false);
				flag = false;
			}
			if (source.IsTag("STRIP_TESTED_TAG"))
			{
				target.EnableTag("STRIP_TESTED_TAG");
			}
			int totalVolume = WaterHelper.GetTotalVolume(target);
			int waterPurity = WaterHelper.GetWaterPurity(target);
			waterFeature2.fakeCondition = GetConditionFromPurity(num);
			waterFeature2.realCondition = GetConditionFromPurity(waterPurity);
			waterFeature2.SetPublicDisplay(GetWaterDisplayName(num) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(num)] + " Purity)");
			waterFeature2.SetActualDisplay(GetWaterDisplayName(waterPurity) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(waterPurity)] + " Purity)");
			target.unitValue = GetWaterPrice(totalVolume);
		}
		else
		{
			target.DisableTag("STRIP_TESTED_TAG");
			int totalVolume2 = WaterHelper.GetTotalVolume(target);
			int customIntValue = GetWaterFeature(target).customIntValue1;
			int waterPurity2 = WaterHelper.GetWaterPurity(target);
			ItemFeature waterFeature3 = GetWaterFeature(target);
			waterFeature3.SetPublicDisplay(GetWaterDisplayName(customIntValue) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(customIntValue)] + " Purity)");
			waterFeature3.SetActualDisplay(GetWaterDisplayName(waterPurity2) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(waterPurity2)] + " Purity)");
			waterFeature3.fakeCondition = GetConditionFromPurity(customIntValue);
			waterFeature3.realCondition = GetConditionFromPurity(waterPurity2);
			target.unitValue = GetWaterPrice(totalVolume2);
			flag = false;
		}
		UpdateWaterFeature(target, flag);
		SpriteHelper.UpdateWaterContainerSprite(target);
		SpriteHelper.UpdateWaterContainerSprite(source);
	}

	public static void OnLiquidChanged(GameItem target, bool exposeFeature)
	{
		WaterHelper.UpdateContainerGoodType(target);
		target.DisableTag("STRIP_TESTED_TAG", skipValidate: true);
		int totalVolume = WaterHelper.GetTotalVolume(target);
		int customIntValue = GetWaterFeature(target).customIntValue1;
		int waterPurity = WaterHelper.GetWaterPurity(target);
		ItemFeature waterFeature = GetWaterFeature(target);
		waterFeature.isDisabled = WaterHelper.IsEmpty(target);
		waterFeature.SetPublicDisplay(GetWaterDisplayName(customIntValue) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(customIntValue)] + " Purity)");
		waterFeature.SetActualDisplay(GetWaterDisplayName(waterPurity) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(waterPurity)] + " Purity)");
		waterFeature.fakeCondition = GetConditionFromPurity(customIntValue);
		waterFeature.realCondition = GetConditionFromPurity(waterPurity);
		target.unitValue = GetWaterPrice(totalVolume);
		if (ItemMouseDragHandler.current.GetCurrentItem() != target && (ItemMultiSelectHandler.current == null || !ItemMultiSelectHandler.current.IsGroupDraggingItem(target)))
		{
			SpriteHelper.UpdateWaterContainerSprite(target);
		}
	}

	public static ItemFeature GetWaterFeature(GameItem gameItem)
	{
		if (!gameItem.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return null;
		}
		if (gameItem.IsTag("WINE_CONTAINER_TAG"))
		{
			ItemFeature itemFeature = new ItemFeature("CATEGORY_WATER_PURITY");
			itemFeature.identifier = "CATEGORY_WATER_PURITY";
			itemFeature.useCondition = true;
			itemFeature.fakeCondition = ItemConditionList.CreateBaseWater();
			itemFeature.realCondition = ItemConditionList.CreateBaseWater();
			itemFeature.customIntValue1 = 9850;
			itemFeature.SetExposed(value: false);
			return itemFeature;
		}
		ItemFeature itemFeature2 = gameItem.FindItemFeatureByCategory("CATEGORY_WATER_PURITY");
		if (itemFeature2 == null)
		{
			itemFeature2 = new ItemFeature("CATEGORY_WATER_PURITY");
			itemFeature2.identifier = "CATEGORY_WATER_PURITY";
			itemFeature2.useCondition = true;
			itemFeature2.fakeCondition = ItemConditionList.CreateBaseWater();
			itemFeature2.realCondition = ItemConditionList.CreateBaseWater();
			gameItem.AddItemFeature(itemFeature2);
		}
		return itemFeature2;
	}

	public static void InitWaterFeature(GameItem gameItem, bool isExpose = false)
	{
		int waterPurity = WaterHelper.GetWaterPurity(gameItem);
		ItemFeature waterFeature = GetWaterFeature(gameItem);
		waterFeature.identifier = "CATEGORY_WATER_PURITY";
		waterFeature.category = "CATEGORY_WATER_PURITY";
		waterFeature.isDisabled = WaterHelper.IsEmpty(gameItem);
		waterFeature.isFeatureMatch = true;
		waterFeature.SetPublicDisplay(GetWaterDisplayName(gameItem) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(waterPurity)] + " Purity)");
		waterFeature.SetActualDisplay(GetWaterDisplayName(gameItem) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(waterPurity)] + " Purity)");
		waterFeature.useCondition = true;
		waterFeature.fakeCondition = GetConditionFromPurity(waterPurity);
		waterFeature.realCondition = GetConditionFromPurity(waterPurity);
		waterFeature.customIntValue1 = waterPurity;
		waterFeature.SetExposed(isExpose);
		WaterHelper.UpdateContainerGoodType(gameItem);
		gameItem.unitValue = GetWaterPrice(gameItem);
		if (ItemMouseDragHandler.current.GetCurrentItem() != gameItem && (ItemMultiSelectHandler.current == null || !ItemMultiSelectHandler.current.IsGroupDraggingItem(gameItem)))
		{
			SpriteHelper.UpdateWaterContainerSprite(gameItem);
		}
	}

	public static ItemCondition GetConditionFromPurity(int purity)
	{
		return ItemConditionList.conditionWaterPurity[GetPurityArrayIndex(purity)];
	}

	public static void UpdateWaterFeature(GameItem gameItem, bool isExpose)
	{
		ItemFeature waterFeature = GetWaterFeature(gameItem);
		int customIntValue = waterFeature.customIntValue1;
		int waterPurity = WaterHelper.GetWaterPurity(gameItem);
		int totalVolume = WaterHelper.GetTotalVolume(gameItem);
		waterFeature.isDisabled = WaterHelper.IsEmpty(gameItem);
		WaterHelper.ResetLastScanResult(gameItem);
		if (isExpose)
		{
			waterFeature.customIntValue1 = waterPurity;
		}
		waterFeature.SetActualDisplay(GetWaterDisplayName(gameItem) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(waterPurity)] + " Purity)");
		if (customIntValue == waterPurity)
		{
			waterFeature.isFeatureMatch = true;
		}
		else
		{
			int waterPrice = GetWaterPrice(totalVolume);
			int waterPrice2 = GetWaterPrice(totalVolume);
			waterFeature.isFeatureMatch = false;
			waterFeature.realCondition = GetConditionFromPurity(waterPurity);
			int num = 0;
			waterFeature.SetExposed(isExpose);
			if (!isExpose)
			{
				gameItem.DisableTag("STRIP_TESTED_TAG", skipValidate: true);
				num = ((waterPrice != 0) ? (-(100 - waterPrice2 * 100 / waterPrice)) : 0);
			}
			else
			{
				num = waterPrice2;
			}
			waterFeature.SetValueModifier(num);
		}
		ChemicalHelper.HandleEmptyContainerTemp(gameItem);
	}

	public static void SetPresumedPurity(GameItem gameItem, int presumedPurity)
	{
		ItemFeature waterFeature = GetWaterFeature(gameItem);
		waterFeature.customIntValue1 = presumedPurity;
		waterFeature.fakeCondition = GetConditionFromPurity(presumedPurity);
		waterFeature.SetPublicDisplay(GetWaterDisplayName(presumedPurity) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(presumedPurity)] + " Purity)");
		waterFeature.SetExposed(value: false);
		gameItem.unitValue = GetWaterPrice(WaterHelper.GetTotalVolume(gameItem));
		UpdateWaterFeature(gameItem, isExpose: false);
	}

	public static void UpdateWaterFeatureFake(GameItem gameItem, int fakePurity)
	{
		int totalVolume = WaterHelper.GetTotalVolume(gameItem);
		int waterPrice = GetWaterPrice(totalVolume);
		gameItem.unitValue = waterPrice;
		int waterPurity = WaterHelper.GetWaterPurity(gameItem);
		int waterPrice2 = GetWaterPrice(totalVolume);
		ItemFeature waterFeature = GetWaterFeature(gameItem);
		waterFeature.fakeCondition = GetConditionFromPurity(fakePurity);
		waterFeature.realCondition = GetConditionFromPurity(waterPurity);
		waterFeature.customIntValue1 = fakePurity;
		waterFeature.SetPublicDisplay(GetWaterDisplayName(fakePurity) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(fakePurity)] + " Purity)");
		waterFeature.SetActualDisplay(GetWaterDisplayName(gameItem) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(waterPurity)] + " Purity)");
		waterFeature.isFeatureMatch = false;
		int valueModifier = -(100 - waterPrice2 * 100 / waterPrice);
		waterFeature.SetValueModifier(valueModifier);
		ChemicalHelper.HandleEmptyContainerTemp(gameItem);
	}

	public static Dialogue GetWaterFeatureDialog(ItemFeature itemFeature)
	{
		Dialogue dialogue = new Dialogue();
		GameItem parentItem = itemFeature.GetParentItem();
		if (parentItem == null)
		{
			return null;
		}
		int waterPurity = WaterHelper.GetWaterPurity(parentItem);
		int customIntValue = itemFeature.customIntValue1;
		UpdateWaterFeatureDialog(dialogue, waterPurity, customIntValue, itemFeature);
		return dialogue;
	}

	public static string GetWaterDisplayName(GameItem gameItem)
	{
		return GetWaterDisplayName(WaterHelper.GetWaterPurity(gameItem));
	}

	public static string GetCurrentWaterDisplay(GameItem gameItem)
	{
		if (!gameItem.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return "";
		}
		ItemFeature waterFeature = GetWaterFeature(gameItem);
		if (waterFeature.IsExposed())
		{
			return GetWaterDisplayName(gameItem);
		}
		return GetWaterDisplayName(waterFeature.GetCurrentItemCondition().customValue);
	}

	public static bool IsLabelledHazardousWater(GameItem gameItem)
	{
		if (!gameItem.IsTag("LIQUID_CONTAINER_TAG") || gameItem.IsTag("CHEMICAL_PRODUCT_TAG") || WaterHelper.IsEmpty(gameItem))
		{
			return false;
		}
		ItemFeature itemFeature = gameItem.FindItemFeatureByCategory("CATEGORY_WATER_PURITY");
		if (itemFeature == null || itemFeature.isDisabled)
		{
			return false;
		}
		return (itemFeature.IsExposed() ? WaterHelper.GetWaterPurity(gameItem) : itemFeature.GetCurrentItemCondition().customValue) < 9800;
	}

	public static string GetWaterDisplayName(int purity)
	{
		for (int i = 0; i < WATER_PURITIES.Length; i++)
		{
			if (purity >= WATER_PURITIES[i])
			{
				return WATER_DISPLAY_LOCALIZED[i];
			}
		}
		return LocHelper.GetLocalizedMechanic("mech_unknown_water");
	}

	public static int GetWaterIndex(GameItem gameItem)
	{
		int waterPurity = WaterHelper.GetWaterPurity(gameItem);
		for (int i = 0; i < WATER_PURITIES.Length; i++)
		{
			if (waterPurity >= WATER_PURITIES[i])
			{
				return i;
			}
		}
		Debug.LogError("ERROR: Water purity does not match any known category: " + waterPurity);
		return -1;
	}

	public static int GetWaterPrice(int totalLiquidParts)
	{
		int num = 40;
		if (num == 0)
		{
			return 0;
		}
		return (int)Math.Round((double)((long)num * (long)totalLiquidParts) / 1000000.0, MidpointRounding.AwayFromZero);
	}

	public static int GetWaterPrice(GameItem gameItem)
	{
		int num = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num += gameItem.GetTagReadonly(liquid.currentPart).GetInt();
		}
		return GetWaterPrice(num);
	}

	public static void HandleWaterCheckFlashlight(GameItem targetGameItem)
	{
		ItemFeature itemFeature = targetGameItem.FindItemFeatureByCategory("CATEGORY_WATER_PURITY");
		if (itemFeature != null)
		{
			if (WaterHelper.GetWaterPurity(targetGameItem) < 9600)
			{
				itemFeature.DiscoverFeature();
			}
			else if (targetGameItem.FindItemFeatureByID("WATER_PURITY_INCONCLUSIVE") == null)
			{
				targetGameItem.AddItemFeature(CreateFeaturePurityInconlusive());
			}
		}
	}

	public static bool CanBeCheckByFlashlight(GameItem gameItem)
	{
		if (WaterHelper.GetWaterPurity(gameItem) < 9600)
		{
			return true;
		}
		return false;
	}

	public static ItemFeature CreateFeaturePurityInconlusive()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Special);
		itemFeature.identifier = "WATER_PURITY_INCONCLUSIVE";
		itemFeature.category = "WATER_PURITY_INCONCLUSIVE";
		itemFeature.SetPublicHidden(val: true);
		itemFeature.SetFeatureDiscovered(value: true);
		itemFeature.SetPublicDisplay("Water Purity: Ghostwater or Better");
		itemFeature.SetActualDisplay("Water Purity: Ghostwater or Better");
		return itemFeature;
	}

	public static void HandleWaterCheckTestingStrip(GameItem targetGameItem)
	{
		ItemFeature itemFeature = targetGameItem.FindItemFeatureByCategory("CATEGORY_WATER_PURITY");
		targetGameItem.RemoveItemFeatureByID("WATER_PURITY_INCONCLUSIVE");
		targetGameItem.EnableTag("STRIP_TESTED_TAG");
		if (PlayerStore.Instance.currentClientInstance != null && !GeneralHelper.IsItemOwned(targetGameItem))
		{
			itemFeature.DiscoverFeature();
			return;
		}
		itemFeature.DiscoverFeature();
		itemFeature.ExposeFeature(lowkey: true);
		InitWaterFeature(targetGameItem, isExpose: true);
	}

	public static string FormatPurityPercentage(int purityValue)
	{
		return ((float)purityValue / 100f).ToString("F2") + "%";
	}

	public static string FormatPurityPercentageFromDouble(double purityValue)
	{
		return $"{purityValue:F2}%";
	}

	public static int GetPurityArrayIndex(int purity)
	{
		for (int i = 0; i < WATER_PURITIES.Length; i++)
		{
			if (purity >= WATER_PURITIES[i])
			{
				return i;
			}
		}
		return 5;
	}

	public static void CreateWaterFeatureTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("WINE_CONTAINER_TAG") && item.IsTag("LIQUID_CONTAINER_TAG") && WaterHelper.GetTotalVolume(item) != 0)
		{
			ItemFeature waterFeature = GetWaterFeature(item);
			if (waterFeature.IsFeatureDiscovered())
			{
				builder.Add(LocHelper.GetLocalizedMechanic("mech_tooltip_tested"), RenderHandler.ColorPalette.Blue);
				builder.AddLine(waterFeature.GetActualDisplay());
			}
		}
	}

	public static void UpdateWaterFeatureDialog(Dialogue dialogue, int actualPurity, int presumePurity, ItemFeature itemFeature)
	{
		if (actualPurity == presumePurity)
		{
			return;
		}
		int purityArrayIndex = GetPurityArrayIndex(actualPurity);
		if (actualPurity > presumePurity)
		{
			dialogue.SetStartAction(delegate
			{
				if (PlayerStore.Instance.currentClientInstance != null)
				{
					StoreReputation.ModReputation(PlayerStore.Instance.currentClientInstance.GetClientBlueprint().clientFaction, 3);
				}
				itemFeature.ExposeFeature();
			}).SetText("PLAYER", LocHelper.GetLocalizedMechanic("water_higher_player")).NextDialogue()
				.SetText("CLIENT", LocHelper.GetLocalizedMechanic("water_higher_client"));
			return;
		}
		dialogue.SetStartAction(delegate
		{
			itemFeature.ExposeFeature();
		});
		if (WATER_DISPLAY[purityArrayIndex] == RUST_WATER_DISPLAY)
		{
			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("water_rust_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("water_rust_client"));
		}
		else if (WATER_DISPLAY[purityArrayIndex] == GUTTERFLOW_WATER_DISPLAY)
		{
			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("water_gutter_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("water_gutter_client"));
		}
		else if (WATER_DISPLAY[purityArrayIndex] == GHOSTWATER_DISPLAY)
		{
			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("water_ghost_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("water_ghost_client"));
		}
		else if (WATER_DISPLAY[purityArrayIndex] == BASE_WATER_DISPLAY)
		{
			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("water_base_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("water_ghost_client"));
		}
		else
		{
			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("water_incorrect_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("water_incorrect_client"));
		}
	}

	public static void OnWrongItemPlaceWaterClient(StoreClient storeClient, GameItem targetItem, string wrongItemString = "")
	{
		if (storeClient.TrySayRemoveFilterLine(targetItem) || !GeneralHelper.IsItemOwned(targetItem))
		{
			return;
		}
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 4;
		if (!wrongItemString.Equals(""))
		{
			dialogue.SetText(storeClient.displayName, wrongItemString);
			DialogUIManager.Instance.StartDialogue(dialogue);
			return;
		}
		string currentWaterDisplay = GetCurrentWaterDisplay(targetItem);
		if (currentWaterDisplay.Equals(PURE_WATER_DISPLAY))
		{
			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_pure"));
		}
		else if (currentWaterDisplay.Equals(HIGH_QUALITY_WATER_DISPLAY))
		{
			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_high_quality"));
		}
		else if (currentWaterDisplay.Equals(BASE_WATER_DISPLAY))
		{
			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_base"));
		}
		else if (currentWaterDisplay.Equals(GHOSTWATER_DISPLAY))
		{
			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_ghost"));
		}
		else if (currentWaterDisplay.Equals(RUST_WATER_DISPLAY))
		{
			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_rust"));
		}
		else if (currentWaterDisplay.Equals(GUTTERFLOW_WATER_DISPLAY))
		{
			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_gutter"));
		}
		else
		{
			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_default"));
		}
		DialogUIManager.Instance.StartDialogue(dialogue);
	}

	public static void OnWaterLabelUpdated(GameItem currentWaterContainer)
	{
		UpdateWaterFeature(currentWaterContainer, isExpose: true);
	}

	public static void OnWaterLabelUpdated(ItemFeature itemFeature, int dropdownValue)
	{
		int customIntValue = 0;
		switch (dropdownValue)
		{
		case 0:
			customIntValue = 9990;
			break;
		case 1:
			customIntValue = 9900;
			break;
		case 2:
			customIntValue = 9800;
			break;
		case 3:
			customIntValue = 9600;
			break;
		case 4:
			customIntValue = 9000;
			break;
		case 5:
			customIntValue = 3500;
			break;
		}
		itemFeature.customIntValue1 = customIntValue;
	}

	public static void HandleLabelerWater()
	{
	}
}

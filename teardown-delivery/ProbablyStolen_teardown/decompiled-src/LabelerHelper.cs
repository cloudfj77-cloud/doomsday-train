public class LabelerHelper
{
	public const string LABELER_TAG = "LABELER_TAG";

	public static void InitLabelerItem(GameItem gameItem)
	{
		gameItem.EnableTag("LABELER_TAG");
	}

	public static void OnTargetItem(GameItem targetItem)
	{
		LabelerUIManager.Instance.OpenUI(targetItem);
	}

	public static void CreateLabeledItemTooltip(RichTextBuilder builder, GameItem item)
	{
		foreach (ItemFeature itemFeature in item.itemFeatures)
		{
			if (item.IsTag("LIQUID_CONTAINER_TAG") && WaterHelper.IsEmpty(item) && itemFeature.category.Equals("CATEGORY_WATER_PURITY"))
			{
				break;
			}
			if (itemFeature.IsExposed())
			{
				HandleLine(builder, itemFeature);
			}
			else if (!itemFeature.GetPublicHidden())
			{
				HandleLine(builder, itemFeature);
			}
		}
	}

	public static void HandleLine(RichTextBuilder builder, ItemFeature itemFeature)
	{
		RenderHandler.ColorPalette color = RenderHandler.ColorPalette.White;
		if (itemFeature.IsFeatureDiscovered())
		{
			color = ((!itemFeature.IsFeatureMatch()) ? RenderHandler.ColorPalette.LightRed : RenderHandler.ColorPalette.LightGreen);
		}
		builder.Add(LocHelper.GetLocalizedMechanic("mech_label_tooltip_prefix") + " ", color);
		builder.AddLine(itemFeature.GetBaseCurrentDisplay());
	}

	public static bool CanEditLabel(ItemFeature itemFeature)
	{
		if (itemFeature.category.Equals("CATEGORY_INGOT_PURITY"))
		{
			return false;
		}
		if (itemFeature.category.Equals("CATEGORY_EQUIPMENT_CONDITION"))
		{
			return false;
		}
		if (itemFeature.category.Equals("CATEGORY_WINE_QUALITY"))
		{
			return false;
		}
		return true;
	}

	public static bool IsEditableLabelFeature(ItemFeature itemFeature)
	{
		if (itemFeature.featureType != 0)
		{
			return false;
		}
		if (itemFeature.category == null)
		{
			return false;
		}
		if (!ItemConditionList.conditionLookup.ContainsKey(itemFeature.category))
		{
			return false;
		}
		return CanEditLabel(itemFeature);
	}

	public static int GetEditableLabelCount(GameItem item)
	{
		int num = 0;
		foreach (ItemFeature itemFeature in item.itemFeatures)
		{
			if (IsEditableLabelFeature(itemFeature))
			{
				num++;
			}
		}
		return num;
	}
}

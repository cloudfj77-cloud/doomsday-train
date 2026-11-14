public class ClientCanExposeFunc
{
	public static bool ALLExposeCapacity(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (ClientCanExposeWaterTestingStrip(itemFeature, storeClient))
		{
			return true;
		}
		if (ClientCanExposeCigarette(itemFeature))
		{
			return true;
		}
		return false;
	}

	public static bool OdinExposeCapacity(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (ClientCanExposeWaterTestingStrip(itemFeature, storeClient))
		{
			return true;
		}
		if (ClientCanExposeCigarette(itemFeature))
		{
			return true;
		}
		if (ClientCanExposeUnusableInjector(itemFeature))
		{
			return true;
		}
		if (itemFeature.category.Equals("CATEGORY_CHEMICAL_PURITY") && !itemFeature.IsFeatureMatch())
		{
			return true;
		}
		return false;
	}

	public static bool TradeBoardExposeCapacity(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (!ItemConditionList.conditionLookup.ContainsKey(itemFeature.category))
		{
			return false;
		}
		if (itemFeature.category.Equals("CATEGORY_WATER_PURITY") && IsEmptyLiquidContainer(itemFeature))
		{
			return false;
		}
		return !itemFeature.IsFeatureMatch();
	}

	public static bool ScavClientExposeCapacity(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (ClientCanExposeWaterFlashlight(itemFeature, storeClient))
		{
			return true;
		}
		if (ClientCanExposeCigarette(itemFeature))
		{
			return true;
		}
		if (ClientCanExposeUnusableInjector(itemFeature))
		{
			return true;
		}
		if (ClientCanExposeChemPurity(itemFeature, storeClient))
		{
			return true;
		}
		return false;
	}

	public static bool LLClientExposeCapacity(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (ClientCanExposeWaterTestingStrip(itemFeature, storeClient))
		{
			return true;
		}
		if (ClientCanExposeCigarette(itemFeature))
		{
			return true;
		}
		if (ClientCanExposeUnusableInjector(itemFeature))
		{
			return true;
		}
		if (ClientCanExposeChemPurity(itemFeature, storeClient))
		{
			return true;
		}
		return false;
	}

	public static bool SecClientExposeCapacity(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (ClientCanExposeWaterTestingStrip(itemFeature, storeClient))
		{
			return true;
		}
		if (ClientCanExposeCigarette(itemFeature))
		{
			return true;
		}
		if (ClientCanExposeUnusableInjector(itemFeature))
		{
			return true;
		}
		if (ClientCanExposeChemPurity(itemFeature, storeClient))
		{
			return true;
		}
		return false;
	}

	public static bool RevClientExposeCapacity(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (ClientCanExposeWaterTestingStrip(itemFeature, storeClient))
		{
			return true;
		}
		if (ClientCanExposeCigarette(itemFeature))
		{
			return true;
		}
		if (ClientCanExposeUnusableInjector(itemFeature))
		{
			return true;
		}
		if (ClientCanExposeChemPurity(itemFeature, storeClient))
		{
			return true;
		}
		return false;
	}

	public static bool TouristClientExposeCapacity(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (ClientCanExposeWaterFlashlight(itemFeature, storeClient))
		{
			return true;
		}
		if (ClientNoExposeInjector(itemFeature))
		{
			return true;
		}
		return false;
	}

	public static bool ULClientExposeCapacity(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (ClientCanExposeWaterFlashlight(itemFeature, storeClient))
		{
			return true;
		}
		if (ClientNoExposeInjector(itemFeature))
		{
			return true;
		}
		if (ClientCanExposeChemPurity(itemFeature, storeClient))
		{
			return true;
		}
		return false;
	}

	public static bool BasicClientExposeCapacity(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (ClientCanExposeWaterFlashlight(itemFeature, storeClient))
		{
			return true;
		}
		if (ClientCanExposeCigarette(itemFeature))
		{
			return true;
		}
		return false;
	}

	private static bool IsEmptyLiquidContainer(ItemFeature itemFeature)
	{
		GameItem parentItem = itemFeature.GetParentItem();
		if (parentItem != null)
		{
			return WaterHelper.IsEmpty(parentItem);
		}
		return true;
	}

	public static bool ClientCanExposeWaterFlashlight(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (itemFeature.category.Equals("CATEGORY_WATER_PURITY"))
		{
			if (IsEmptyLiquidContainer(itemFeature))
			{
				return false;
			}
			if (itemFeature.IsFeatureMatch())
			{
				return false;
			}
			if (itemFeature.GetBaseRealDisplay().Equals(WaterFeatureHelper.GUTTERFLOW_WATER_DISPLAY))
			{
				return true;
			}
			if (itemFeature.GetBaseRealDisplay().Equals(WaterFeatureHelper.RUST_WATER_DISPLAY))
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public static bool ClientCanExposeWaterTestingStrip(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (itemFeature.category.Equals("CATEGORY_WATER_PURITY"))
		{
			if (IsEmptyLiquidContainer(itemFeature))
			{
				return false;
			}
			if (itemFeature.IsFeatureMatch())
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public static bool ClientCanExposeCigarette(ItemFeature itemFeature)
	{
		if (itemFeature.category.Equals("CATEGORY_GENUINE_CIGARETTE"))
		{
			return true;
		}
		return false;
	}

	public static bool ClientCanExposeUnusableInjector(ItemFeature itemFeature)
	{
		if (itemFeature.category.Equals("CATEGORY_GENUINE_INJECTOR") && !itemFeature.IsFeatureMatch())
		{
			if (itemFeature.realCondition.identifier.Equals("unusableInjector") && InsInjectorHelper.IsOnlyIssueRecalled(itemFeature.GetParentItem()))
			{
				return false;
			}
			if (itemFeature.realCondition.identifier.Equals("unusableInjector"))
			{
				return true;
			}
			if (itemFeature.realCondition.identifier.Equals("counterfeitInjector") && !itemFeature.fakeCondition.identifier.Equals("unusableInjector"))
			{
				return true;
			}
			if (itemFeature.realCondition.identifier.Equals("expiredInjector") && !itemFeature.fakeCondition.identifier.Equals("unusableInjector") && !itemFeature.fakeCondition.identifier.Equals("counterfeitInjector"))
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public static bool ClientNoExposeInjector(ItemFeature itemFeature)
	{
		if (itemFeature.realCondition.identifier.Equals("unusableInjector") && InsInjectorHelper.IsOnlyIssueRecalled(itemFeature.GetParentItem()))
		{
			return false;
		}
		if (itemFeature.realCondition.identifier.Equals("unusableInjector"))
		{
			return true;
		}
		if (itemFeature.realCondition.identifier.Equals("expiredInjector") && !itemFeature.fakeCondition.identifier.Equals("unusableInjector") && !itemFeature.fakeCondition.identifier.Equals("counterfeitInjector"))
		{
			return true;
		}
		return false;
	}

	public static bool ClientCanExposeWaterTestingStrip(ItemFeature itemFeature)
	{
		if (itemFeature.category.Equals("CATEGORY_WATER_PURITY"))
		{
			return true;
		}
		return false;
	}

	public static bool ClientCanExposeChemPurity(ItemFeature itemFeature, StoreClient storeClient)
	{
		if (itemFeature.category.Equals("CATEGORY_CHEMICAL_PURITY"))
		{
			if (itemFeature.IsFeatureMatch())
			{
				return false;
			}
			if (storeClient.CanExposeChem)
			{
				return true;
			}
		}
		return false;
	}
}

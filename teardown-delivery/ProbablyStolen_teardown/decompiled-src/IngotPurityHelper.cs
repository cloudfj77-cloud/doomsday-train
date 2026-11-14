using System.Collections.Generic;
using System.Linq;

public class IngotPurityHelper
{
	public class IngotPurity
	{
		public int ValueMod;

		public int PurityLevel;

		public ItemCondition itemCondition;

		public string PurityTag { get; private set; }

		public string PurityDisplay { get; private set; }

		public IngotPurity(string purityTag, string purityDisplay, int valueMod, int level, ItemCondition itemCondition)
		{
			PurityTag = purityTag;
			PurityDisplay = purityDisplay;
			ValueMod = valueMod;
			PurityLevel = level;
			this.itemCondition = itemCondition;
		}

		public override string ToString()
		{
			return PurityDisplay + " (" + PurityTag + ")";
		}
	}

	public const string INGOT_PURITY_TAG = "INGOT_PURITY_TAG";

	public const string INGOT_PURITY_FAKE_TAG = "INGOT_PURITY_FAKE_TAG";

	public const string INGOT_PURITY_INCONCLUSIVE = "INGOT_PURITY_INCONCLUSIVE";

	public static Dictionary<string, IngotPurity> ingotPurities = new Dictionary<string, IngotPurity>
	{
		{
			"INGOT_PURITY_PERFECT",
			new IngotPurity("INGOT_PURITY_PERFECT", "Perfect Purity", 200, 10, ItemConditionList.CreatePerfectQualityIngot())
		},
		{
			"INGOT_PURITY_VERY_HIGH",
			new IngotPurity("INGOT_PURITY_VERY_HIGH", "Very High Purity", 50, 9, ItemConditionList.CreateVeryHighQualityIngot())
		},
		{
			"INGOT_PURITY_HIGH",
			new IngotPurity("INGOT_PURITY_HIGH", "High Purity", 25, 8, ItemConditionList.CreateHighQualityIngot())
		},
		{
			"INGOT_PURITY_FAIR",
			new IngotPurity("INGOT_PURITY_FAIR", "Fair Purity", 0, 7, ItemConditionList.CreateFairQualityIngot())
		},
		{
			"INGOT_PURITY_LOW",
			new IngotPurity("INGOT_PURITY_LOW", "Low Purity", -15, 6, ItemConditionList.CreateLowQualityIngot())
		},
		{
			"INGOT_PURITY_VERY_LOW",
			new IngotPurity("INGOT_PURITY_VERY_LOW", "Very Low Purity", -25, 5, ItemConditionList.CreateVeryLowQualityIngot())
		}
	};

	public const string INGOT_PURITY_PERFECT = "INGOT_PURITY_PERFECT";

	public const string INGOT_PURITY_VERY_HIGH = "INGOT_PURITY_VERY_HIGH";

	public const string INGOT_PURITY_HIGH = "INGOT_PURITY_HIGH";

	public const string INGOT_PURITY_FAIR = "INGOT_PURITY_FAIR";

	public const string INGOT_PURITY_LOW = "INGOT_PURITY_LOW";

	public const string INGOT_PURITY_VERY_LOW = "INGOT_PURITY_VERY_LOW";

	public static void InitIngotItem(GameItem gameItem)
	{
		gameItem.EnableTag("INGOT_PURITY_TAG");
		if (!gameItem.IsAlreadyContainFeatureWithId("INGOT_PURITY_TAG"))
		{
			gameItem.AddItemFeature(FeaturePurity());
		}
		SetPurity(gameItem, "INGOT_PURITY_FAIR");
	}

	public static void SetPurity(GameItem gameItem, string realPurity)
	{
		gameItem.ModifyTag("INGOT_PURITY_TAG", delegate(TagState state)
		{
			state.Enable().SetString(realPurity);
		});
		gameItem.ModifyTag("INGOT_PURITY_FAKE_TAG", delegate(TagState state)
		{
			state.Enable().SetString(realPurity);
		});
		ItemFeature itemFeature = gameItem.FindItemFeatureByID("INGOT_PURITY_TAG");
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(ingotPurities[realPurity].ValueMod);
		itemFeature.SetValueModifier(ingotPurities[realPurity].ValueMod);
		itemFeature.useCondition = true;
		itemFeature.fakeCondition = ingotPurities[realPurity].itemCondition;
		itemFeature.realCondition = ingotPurities[realPurity].itemCondition;
		itemFeature.SetPublicDisplay("Metal Purity: " + ingotPurities[realPurity].PurityDisplay);
		itemFeature.SetActualDisplay("Metal Purity: " + ingotPurities[realPurity].PurityDisplay);
	}

	public static void FurnaceSmelt()
	{
	}

	public static string GetPurity(GameItem gameItem)
	{
		return gameItem.GetTagReadonly("INGOT_PURITY_TAG").GetString();
	}

	public static string GetFakePurity(GameItem gameItem)
	{
		return gameItem.GetTagReadonly("INGOT_PURITY_FAKE_TAG").GetString();
	}

	public static void HandleIngotCheckFlashlight(GameItem gameItem)
	{
		if (gameItem.IsTag("INGOT_PURITY_TAG"))
		{
			if (IsDiscovableByFlashlight(gameItem))
			{
				gameItem.FindItemFeatureByID("INGOT_PURITY_TAG").DiscoverFeature();
			}
			else if (gameItem.FindItemFeatureByID("INGOT_PURITY_INCONCLUSIVE") == null)
			{
				ItemFeature itemFeature = CreateFeaturePurityInconlusive();
				gameItem.AddItemFeature(itemFeature);
				itemFeature.DiscoverFeature();
			}
		}
	}

	public static bool IsDiscovableByFlashlight(GameItem gameItem)
	{
		if (GetPurity(gameItem).Equals("INGOT_PURITY_VERY_LOW") || GetPurity(gameItem).Equals("INGOT_PURITY_LOW"))
		{
			return true;
		}
		return false;
	}

	public static bool IsDiscovableByDector(GameItem gameItem)
	{
		return true;
	}

	public static ItemFeature CreateFeaturePurityInconlusive()
	{
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Special);
		itemFeature.identifier = "INGOT_PURITY_INCONCLUSIVE";
		itemFeature.category = "INGOT_PURITY_INCONCLUSIVE";
		itemFeature.SetPublicHidden(val: true);
		itemFeature.SetFeatureDiscovered(value: true);
		itemFeature.SetPublicDisplay("Metal Purity: Fair or Better");
		itemFeature.SetActualDisplay("Metal Purity: Fair or Better");
		return itemFeature;
	}

	public static void HandleIngotCheckAnalyser(GameItem gameItem)
	{
		if (gameItem.IsTag("INGOT_PURITY_TAG"))
		{
			gameItem.FindItemFeatureByID("INGOT_PURITY_TAG").DiscoverFeature();
			gameItem.RemoveItemFeatureByID("INGOT_PURITY_INCONCLUSIVE");
		}
	}

	public static ItemFeature FeaturePurity()
	{
		ItemFeature itemFeature = new ItemFeature("INGOT_PURITY_TAG");
		itemFeature.identifier = "INGOT_PURITY_TAG";
		itemFeature.category = "CATEGORY_INGOT_PURITY";
		itemFeature.SetPublicDisplay("Metal Purity: " + ingotPurities["INGOT_PURITY_FAIR"].PurityDisplay);
		itemFeature.SetActualDisplay("Metal Purity: " + ingotPurities["INGOT_PURITY_FAIR"].PurityDisplay);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetPreExposedValueMod(0);
		itemFeature.useCondition = true;
		itemFeature.fakeCondition = ingotPurities["INGOT_PURITY_FAIR"].itemCondition;
		itemFeature.realCondition = ingotPurities["INGOT_PURITY_FAIR"].itemCondition;
		return itemFeature;
	}

	public static void SetPurityFromInput(GameItem gameItem, List<GameItem> consumedInputs, int fluxLevel = 0)
	{
		if (consumedInputs != null && consumedInputs.Count != 0)
		{
			int num = consumedInputs.Count((GameItem item) => item.identifier == "common_ore");
			int num2 = 2 * num / consumedInputs.Count;
			SetPurity(gameItem, GetPurityConst("INGOT_PURITY_LOW", num2 + fluxLevel));
		}
	}

	public static void SpawnFurnaceIngotTestingKit()
	{
		GameGridInventory invElement = EmporiumEntry.Instance.invElement;
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("ingot_book"));
		GameItem gameItem = PreBuiltItemHelper.CreateFurnace();
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
		GraphUtils.TryAcceptAll(invElement, gameItem);
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("scrap_metal"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("scrap_metal"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("scrap_metal"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("common_ore"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("common_ore"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("common_ore"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("metal_ingot"));
		GraphUtils.TryAcceptAll(invElement, PreBuiltItemHelper.RealIngotPurity("INGOT_PURITY_LOW"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("flux_agent"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("flux_agent"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("flux_agent"));
	}

	public static void CreateIngotTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("INGOT_PURITY_TAG") && item.FindItemFeatureByID("INGOT_PURITY_TAG").IsFeatureDiscovered())
		{
			builder.AddLine("[Tested] Ingot Purity:" + ingotPurities[GetPurity(item)].PurityDisplay, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
	}

	public static string GetPurityConst(string purityConst, int level)
	{
		if (ingotPurities.TryGetValue(purityConst, out var value))
		{
			int num = value.PurityLevel + level;
			foreach (KeyValuePair<string, IngotPurity> ingotPurity in ingotPurities)
			{
				if (ingotPurity.Value.PurityLevel == num)
				{
					return ingotPurity.Key;
				}
			}
			if (level > 0)
			{
				return ingotPurities.OrderByDescending((KeyValuePair<string, IngotPurity> p) => p.Value.PurityLevel).First().Key;
			}
			if (level < 0)
			{
				return ingotPurities.OrderBy((KeyValuePair<string, IngotPurity> p) => p.Value.PurityLevel).First().Key;
			}
		}
		return purityConst;
	}
}

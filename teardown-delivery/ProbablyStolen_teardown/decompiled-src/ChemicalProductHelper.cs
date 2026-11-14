public class ChemicalProductHelper
{
	public const string ACTIVATE_INGREDIENT_TAG = "ACTIVATE_INGREDIENT_TAG";

	public const string CHEMICAL_PRODUCT_TAG = "CHEMICAL_PRODUCT_TAG";

	public const string CHEMICAL_PRODUCT_PURITY_TAG = "CHEMICAL_PRODUCT_PURITY_TAG";

	public const int PURE_GRADE = 99;

	public const int PHARMA_GRADE = 90;

	public const int NEAR_PHARMA_GRADE = 80;

	public const int STREET_STANDARD_GRADE = 60;

	public const int CUT_PRODUCT_GRADE = 45;

	public const int JUNK_GRADE = 30;

	public const int STARCH_GRADE = 0;

	public const string CONDITION_LAB_PURE = "lab_pure";

	public const string CONDITION_PHARMA_GRADE = "pharma_grade";

	public const string CONDITION_NEAR_PHARMA = "near_pharma";

	public const string CONDITION_STREET_STANDARD = "street_standard";

	public const string CONDITION_CUT_PRODUCT = "cut_product";

	public const string CONDITION_JUNK_GRADE = "junk_grade";

	public const string CONDITION_STARCH_GRADE = "starch_grade";

	public static int GetGradeNumber(string conditionId)
	{
		return conditionId switch
		{
			"lab_pure" => 6, 
			"pharma_grade" => 5, 
			"near_pharma" => 4, 
			"street_standard" => 3, 
			"cut_product" => 2, 
			"junk_grade" => 1, 
			"starch_grade" => 0, 
			_ => -1, 
		};
	}

	public static int GetGradeDifference(string conditionIdA, string conditionIdB)
	{
		return GetGradeNumber(conditionIdA) - GetGradeNumber(conditionIdB);
	}

	public static void InitChemicalProduct(GameItem item, string liquidId)
	{
		item.EnableTag("CHEMICAL_PRODUCT_TAG");
		item.ModifyTag("ACTIVATE_INGREDIENT_TAG", delegate(TagState state)
		{
			state.Enable().SetString(liquidId);
		});
		GradeProduct(item);
	}

	public static ItemFeature GetChemicalFeature(GameItem item)
	{
		return item.FindItemFeatureByCategory("CATEGORY_CHEMICAL_PURITY");
	}

	public static string GetCurrentChemicalConditionId(GameItem item)
	{
		ItemFeature chemicalFeature = GetChemicalFeature(item);
		if (chemicalFeature == null)
		{
			return "";
		}
		return chemicalFeature.GetCurrentItemCondition().identifier;
	}

	public static void SetPurityFeature(GameItem item, ItemCondition realCondition, ItemCondition fakeCondition)
	{
		ItemFeature itemFeature = item.FindItemFeatureByCategory("CATEGORY_CHEMICAL_PURITY");
		if (itemFeature == null)
		{
			itemFeature = new ItemFeature("CATEGORY_CHEMICAL_PURITY");
			itemFeature.identifier = "chemical_purity";
			item.AddItemFeature(itemFeature);
		}
		itemFeature.SetConditions(fakeCondition, realCondition);
		if (realCondition.identifier.Equals(fakeCondition.identifier))
		{
			itemFeature.isFeatureMatch = true;
		}
	}

	public static void SetPurityFeature(GameItem item, ItemCondition condition)
	{
		SetPurityFeature(item, condition, condition);
	}

	public static void GradeProduct(GameItem item)
	{
		if (item.IsTag("CHEMICAL_PRODUCT_TAG"))
		{
			string @string = item.GetTagReadonly("ACTIVATE_INGREDIENT_TAG").GetString();
			int @int = item.GetTagReadonly(Liquid.GetCurrentPartFromId(@string)).GetInt();
			int totalVolume = ChemicalHelper.GetTotalVolume(item);
			double purityRatio = (double)@int / (double)totalVolume;
			purityRatio *= 100.0;
			item.ModifyTag("CHEMICAL_PRODUCT_PURITY_TAG", delegate(TagState state)
			{
				state.Enable().SetDouble(purityRatio);
			});
			if (purityRatio >= 99.0)
			{
				SetPurityFeature(item, ItemConditionList.CreateLabPure());
			}
			else if (purityRatio >= 90.0)
			{
				SetPurityFeature(item, ItemConditionList.CreatePharmaGrade());
			}
			else if (purityRatio >= 80.0)
			{
				SetPurityFeature(item, ItemConditionList.CreateNearPharma());
			}
			else if (purityRatio >= 60.0)
			{
				SetPurityFeature(item, ItemConditionList.CreateStreetStandard());
			}
			else if (purityRatio >= 45.0)
			{
				SetPurityFeature(item, ItemConditionList.CreateCutProduct());
			}
			else if (purityRatio >= 30.0)
			{
				SetPurityFeature(item, ItemConditionList.CreateJunkGrade());
			}
			else
			{
				SetPurityFeature(item, ItemConditionList.CreateStarchGrade());
			}
		}
	}

	public static double GetPurity(GameItem gameItem)
	{
		if (gameItem.IsTag("CHEMICAL_PRODUCT_PURITY_TAG"))
		{
			return gameItem.GetTagReadonly("CHEMICAL_PRODUCT_PURITY_TAG").GetDouble();
		}
		return 0.0;
	}

	public static Dialogue GetChemicalFeatureDialog(ItemFeature itemFeature)
	{
		Dialogue dialogue = new Dialogue();
		GameItem parentItem = itemFeature.GetParentItem();
		if (parentItem == null)
		{
			return null;
		}
		ItemCondition realCondition = GetChemicalFeature(parentItem).realCondition;
		ItemCondition fakeCondition = GetChemicalFeature(parentItem).fakeCondition;
		if (realCondition.modValue > fakeCondition.modValue)
		{
			dialogue.SetStartAction(delegate
			{
				if (PlayerStore.Instance.currentClientInstance != null)
				{
					StoreReputation.ModReputation(PlayerStore.Instance.currentClientInstance.GetClientBlueprint().clientFaction, 3);
				}
				itemFeature.ExposeFeature();
			}).SetText("PLAYER", LocHelper.GetLocalizedDialogue("chem_purity_higher_player")).NextDialogue()
				.SetText("CLIENT", LocHelper.GetLocalizedDialogue("chem_purity_higher_client"));
		}
		else
		{
			dialogue.SetStartAction(delegate
			{
				itemFeature.ExposeFeature();
			});
			if (realCondition.identifier.Equals("starch_grade"))
			{
				dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("chem_starch_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("chem_client_lower_price"));
			}
			else if (realCondition.identifier.Equals("junk_grade"))
			{
				dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("chem_junk_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("chem_client_lower_price"));
			}
			else if (realCondition.identifier.Equals("cut_product"))
			{
				dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("chem_cut_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("chem_cut_client"));
			}
			else if (realCondition.identifier.Equals("street_standard"))
			{
				dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("chem_street_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("chem_client_didnt_know"));
			}
			else if (realCondition.identifier.Equals("near_pharma"))
			{
				dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("chem_near_pharma_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("chem_client_didnt_know"));
			}
			else if (realCondition.identifier.Equals("pharma_grade"))
			{
				dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("chem_pharma_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("chem_client_didnt_know"));
			}
			else
			{
				dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("chem_incorrect_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("chem_incorrect_client"));
			}
		}
		return dialogue;
	}

	public static bool WillAcceptNarcotic(GameItem item, int grade = 1)
	{
		if (!item.IsTag("CHEMICAL_PRODUCT_TAG"))
		{
			return false;
		}
		string identifier = GetChemicalFeature(item).GetCurrentItemCondition().identifier;
		if (identifier.Equals("starch_grade") || identifier.Equals("junk_grade"))
		{
			return false;
		}
		if (grade >= 2 && identifier.Equals("cut_product"))
		{
			return false;
		}
		if (grade >= 3 && identifier.Equals("street_standard"))
		{
			return false;
		}
		return true;
	}

	public static bool HasChemicalPurity(GameItem item)
	{
		return item.IsTag("CHEMICAL_PRODUCT_TAG");
	}
}

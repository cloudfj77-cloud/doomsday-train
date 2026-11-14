using System;
using System.Collections.Generic;

public class ChemicalFeedbackHelper
{
	public enum ChemicalIntent
	{
		NONE,
		HYDRATION,
		NUTRITION,
		NARCOTIC,
		MEDICAL,
		POISON,
		BIRTH_CONTROL,
		SYNAPREST
	}

	public enum ReactionType
	{
		NONE,
		POSITIVE,
		NEGATIVE,
		NEUTRAL
	}

	public enum ReactionSeverity
	{
		NONE,
		MINOR,
		MODERATE,
		SEVERE
	}

	public const string CHEMICAL_INTENT_TAG = "CHEMICAL_INTENT_TAG";

	public const int LETHALITY_THRESHOLD = 1000;

	public const int TOXICITY_THRESHOLD = 750;

	public const int EXTREME_EUPHORIA_THRESHOLD = 2200;

	public const int STRONG_EUPHORIA_THRESHOLD = 1600;

	public const int MILD_EUPHORIA_THRESHOLD = 1000;

	public const int WEAK_EUPHORIA_THRESHOLD = 750;

	public const int BIRTH_CONTROL_THRESHOLD = 50;

	public const int SYNAPREST_THRESHOLD = 1000;

	public static void InitChemicalIntentItem(GameItem item, ChemicalIntent intent)
	{
		item.ModifyTag("CHEMICAL_INTENT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt((int)intent);
		});
	}

	public static ChemicalIntent GetChemicalIntent(GameItem item)
	{
		if (item == null)
		{
			return ChemicalIntent.NONE;
		}
		if (item.IsTag("CHEMICAL_INTENT_TAG"))
		{
			return (ChemicalIntent)item.GetTagReadonly("CHEMICAL_INTENT_TAG").GetInt();
		}
		return ChemicalIntent.NONE;
	}

	public static bool IsLethal(GameItem item)
	{
		double num = 0.0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			int @int = item.GetTagReadonly(liquid.currentPart).GetInt();
			num += (double)@int * liquid.lethality;
		}
		return num >= 1000.0;
	}

	public static int GetLethalDose(GameItem item)
	{
		double num = 0.0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			int @int = item.GetTagReadonly(liquid.currentPart).GetInt();
			num += (double)@int * liquid.lethality;
		}
		return (int)(num / 1000.0);
	}

	public static bool IsToxic(GameItem item)
	{
		double num = 0.0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			int @int = item.GetTagReadonly(liquid.currentPart).GetInt();
			num += (double)@int * liquid.lethality;
		}
		return num >= 750.0;
	}

	public static bool IsSynaprest(GameItem item)
	{
		if (item.GetTagReadonly(Liquid.GetCurrentPartFromId("synaprest")).GetInt() >= 1000)
		{
			return true;
		}
		return false;
	}

	public static bool IsBirthControl(GameItem item)
	{
		if (item.GetTagReadonly(Liquid.GetCurrentPartFromId("contraceptive")).GetInt() >= 50)
		{
			return true;
		}
		return false;
	}

	public static bool IsNarcotic(GameItem item)
	{
		double num = 0.0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			int @int = item.GetTagReadonly(liquid.currentPart).GetInt();
			num += (double)@int * liquid.euphoria;
		}
		return num >= 750.0;
	}

	public static string GetNarcoticDebugDisplay(GameItem item)
	{
		double num = 0.0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			int @int = item.GetTagReadonly(liquid.currentPart).GetInt();
			num += (double)@int * liquid.euphoria;
		}
		return num + "/" + 750;
	}

	public static int GetEuphoriaLevel(GameItem item)
	{
		double num = 0.0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			int @int = item.GetTagReadonly(liquid.currentPart).GetInt();
			num += (double)@int * liquid.euphoria;
		}
		if (num >= 2200.0)
		{
			return 4;
		}
		if (num >= 1600.0)
		{
			return 3;
		}
		if (num >= 1000.0)
		{
			return 2;
		}
		if (num >= 750.0)
		{
			return 1;
		}
		return 0;
	}

	public static int GetItemFeedbackLevel(GameItem item)
	{
		int num = 0;
		ChemicalIntent chemicalIntent = GetChemicalIntent(item);
		if (chemicalIntent == ChemicalIntent.NONE)
		{
			return 0;
		}
		int num2 = 0;
		int purityGradeDifference = GetPurityGradeDifference(item);
		num2 += purityGradeDifference;
		if (chemicalIntent != ChemicalIntent.POISON)
		{
			if (IsLethal(item))
			{
				num += 5;
			}
			else if (IsToxic(item))
			{
				num += 4;
			}
			if (chemicalIntent != ChemicalIntent.BIRTH_CONTROL)
			{
				if (IsBirthControl(item))
				{
					num += 2;
				}
			}
			else if (!IsBirthControl(item))
			{
				num += 2;
			}
			if (chemicalIntent != ChemicalIntent.SYNAPREST)
			{
				if (!IsSynaprest(item))
				{
				}
			}
			else if (!IsSynaprest(item))
			{
				num += 2;
			}
			if (chemicalIntent != ChemicalIntent.NARCOTIC)
			{
				if (IsNarcotic(item))
				{
					num += 2;
				}
			}
			else if (!IsNarcotic(item))
			{
				num += 4;
			}
			int num3 = GetEuphoriaLevel(item);
			if (num3 >= 3)
			{
				num3++;
			}
			if (num3 >= 4)
			{
				num3++;
			}
			double purity = ChemicalProductHelper.GetPurity(item);
			if (item.IsTag("CHEMICAL_PRODUCT_TAG") && purity < 30.0 && chemicalIntent != ChemicalIntent.NARCOTIC)
			{
				num2 += 2;
			}
			else if (item.IsTag("CHEMICAL_PRODUCT_TAG") && purity < 45.0 && chemicalIntent != ChemicalIntent.NARCOTIC)
			{
				num2++;
			}
			num += Math.Max(num3, num2);
		}
		if (chemicalIntent == ChemicalIntent.POISON)
		{
			if (!IsLethal(item))
			{
				num += 2;
			}
			if (!IsToxic(item))
			{
				num += 2;
			}
		}
		return num;
	}

	public static bool CanReport(GameItem item, SoldItem soldItem)
	{
		if (item == null)
		{
			Debug.LogWarning("CanReport called with null item");
			return false;
		}
		if (!item.IsTag("CHEMICAL_INTENT_TAG"))
		{
			return false;
		}
		bool result = true;
		if (soldItem.factionId == "FACTION_BLACK_MARKET" || soldItem.factionId == "FACTION_REVOLUTION" || ContrabandHelper.IsContraband(item))
		{
			result = false;
		}
		return result;
	}

	public static void GenerateReaction(GameItem item, SoldItem soldItem, List<SoldItem> relatedItems)
	{
		List<string> list = new List<string>();
		ChemicalIntent chemicalIntent = GetChemicalIntent(item);
		double purity = ChemicalProductHelper.GetPurity(item);
		item.IsTag("CHEMICAL_PRODUCT_TAG");
		bool flag = true;
		bool isAskingCompensation = false;
		bool flag2 = true;
		int num = -2;
		CanReport(item, soldItem);
		if (chemicalIntent == ChemicalIntent.POISON)
		{
			flag2 = false;
			if (!IsLethal(item) && !IsToxic(item))
			{
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_poison_no_harm", item.name));
				flag2 = false;
				isAskingCompensation = true;
			}
			else if (!IsLethal(item) && IsToxic(item))
			{
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_poison_not_finished", item.name));
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_poison_still_alive"));
				flag2 = false;
				isAskingCompensation = true;
			}
			if (IsNarcotic(item))
			{
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_poison_made_high"));
			}
		}
		else if (IsLethal(item) || IsToxic(item))
		{
			if (!IsLethal(item) && IsToxic(item))
			{
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_toxic_almost_killed", item.name));
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_toxic_what_the_hell"));
				num -= 5;
				flag2 = false;
				isAskingCompensation = true;
			}
			else if (IsLethal(item))
			{
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_lethal_killed_friend", item.name));
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_pay_for_this"));
				flag = false;
				num -= 15;
				flag2 = false;
				isAskingCompensation = true;
			}
		}
		else
		{
			if (chemicalIntent == ChemicalIntent.BIRTH_CONTROL && !IsBirthControl(item))
			{
				flag2 = false;
				isAskingCompensation = true;
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_birthcontrol_didnt_work", item.name));
				num -= 2;
				if (RNG.Roll(50))
				{
					list.Add(LocHelper.GetLocalizedMechanic("mech_chem_birthcontrol_pregnant_self"));
				}
				else
				{
					list.Add(LocHelper.GetLocalizedMechanic("mech_chem_birthcontrol_pregnant_her"));
				}
			}
			if (chemicalIntent == ChemicalIntent.SYNAPREST && !IsSynaprest(item))
			{
				flag2 = false;
				isAskingCompensation = true;
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_synaprest_didnt_work", item.name));
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_synaprest_still_addicted"));
				num -= 4;
				if (IsNarcotic(item))
				{
					list.Add(LocHelper.GetLocalizedMechanic("mech_chem_synaprest_made_high"));
					num--;
				}
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_synaprest_saved_credit"));
			}
			if (chemicalIntent == ChemicalIntent.NARCOTIC)
			{
				if (GetEuphoriaLevel(item) == 0)
				{
					flag2 = false;
					isAskingCompensation = true;
					num -= 2;
					list.Add(LocHelper.GetLocalizedMechanic("mech_chem_narcotic_no_effect", item.name));
					if (RNG.Roll(50))
					{
						list.Add(LocHelper.GetLocalizedMechanic("mech_chem_narcotic_any_drugs"));
					}
					list.Add(LocHelper.GetLocalizedMechanic("mech_chem_narcotic_no_high"));
				}
				else if (GetEuphoriaLevel(item) == 1)
				{
					flag2 = false;
					isAskingCompensation = true;
					num--;
					list.Add(LocHelper.GetLocalizedMechanic("mech_chem_narcotic_weak", item.name));
					list.Add(LocHelper.GetLocalizedMechanic("mech_chem_narcotic_barely_felt"));
				}
			}
			if ((chemicalIntent == ChemicalIntent.MEDICAL || chemicalIntent == ChemicalIntent.NUTRITION || chemicalIntent == ChemicalIntent.HYDRATION || chemicalIntent == ChemicalIntent.NARCOTIC) && IsNarcotic(item))
			{
				GetEuphoriaLevel(item);
				_ = 2;
			}
			if (chemicalIntent == ChemicalIntent.MEDICAL && purity < 45.0)
			{
				num -= 3;
				flag2 = false;
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_medical_weak", item.name));
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_medical_pretty_weak"));
			}
			if (chemicalIntent != ChemicalIntent.POISON && chemicalIntent != ChemicalIntent.BIRTH_CONTROL && IsBirthControl(item))
			{
				num--;
				flag2 = false;
				list.Add(LocHelper.GetLocalizedMechanic("mech_chem_side_effect_weird"));
			}
			if (GetPurityGradeDifference(item) > 0)
			{
				if (GetPurityGradeDifference(item) == 1)
				{
					flag2 = false;
					list.Add(LocHelper.GetLocalizedMechanic("mech_chem_purity_slightly_off", item.name));
					num--;
				}
				else
				{
					flag = true;
					flag2 = false;
					list.Add(LocHelper.GetLocalizedMechanic("mech_chem_purity_scam", item.name));
					list.Add(LocHelper.GetLocalizedMechanic("mech_chem_purity_way_off"));
					num -= 2;
				}
			}
		}
		if (flag2)
		{
			if (soldItem.CanReturnAsBuyer)
			{
				QueueChemicalReturningClient(soldItem);
			}
		}
		else if (!flag || RNG.Roll(50))
		{
			QueueHandnoteFeedback(list, soldItem, num);
		}
		else
		{
			QueueFeedbackClient(soldItem, relatedItems, list, isAskingCompensation, num);
		}
	}

	public static int GetPurityGradeDifference(GameItem gameItem)
	{
		if (!gameItem.IsTag("CHEMICAL_PRODUCT_TAG"))
		{
			return 0;
		}
		string identifier = gameItem.FindItemFeatureByCategory("CATEGORY_CHEMICAL_PURITY").GetCurrentItemCondition().identifier;
		string identifier2 = gameItem.FindItemFeatureByCategory("CATEGORY_CHEMICAL_PURITY").realCondition.identifier;
		return ChemicalProductHelper.GetGradeDifference(identifier, identifier2);
	}

	public static void QueueHandnoteFeedback(List<string> reactions, SoldItem soldItem, int reputationLoss)
	{
		if (reactions.Count == 0)
		{
			Debug.LogWarning("reactions HandnoteFeedback called with no reactions");
			return;
		}
		List<string> lines = reactions.ConvertAll((string line) => LocGenderHelper.ResolveForSoldItemClient(line, soldItem));
		PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHandNoteFromList(lines), isOwend: true);
		StoreReputation.ModReputation(soldItem.factionId, reputationLoss);
	}

	public static void QueueFeedbackClient(SoldItem soldItem, List<SoldItem> relatedItems, List<string> lines, bool isAskingCompensation, int reputationLoss)
	{
		PlayerStore.Instance.storeClientManager.chemicalFeedbackStack.Add(StoreClientListSpec.CreateChemicalFeedbackClient(soldItem, relatedItems, lines, isAskingCompensation, reputationLoss));
	}

	public static void QueueChemicalReturningClient(SoldItem soldItem)
	{
		PlayerStore.Instance.storeClientManager.chemicalFeedbackStack.Add(StoreClientListSpec.CreateChemicalReturningClient(soldItem));
	}

	public static void CancelFeedbackForShotClient(string clientId)
	{
		if (string.IsNullOrEmpty(clientId))
		{
			return;
		}
		foreach (SoldItem item in PlayerStore.Instance.soldItemsTracker)
		{
			if (item.clientId == clientId)
			{
				item.CanReturn = false;
				item.CanReturnAsBuyer = false;
			}
		}
	}

	public static void HandleFeedback(GameItem item)
	{
	}
}

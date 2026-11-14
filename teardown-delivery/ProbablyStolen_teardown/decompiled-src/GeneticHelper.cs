using UnityEngine;

public class GeneticHelper
{
	public const float MUTATION_CHANCE = 0.15f;

	public const float MUTATION_BASE_RANGE = 0.08f;

	public const float MUTATION_RESIST_SCALE = 2f;

	public const string ALLELE_A_SUFFIX = "_ALLELE_A";

	public const string ALLELE_B_SUFFIX = "_ALLELE_B";

	public const string PARTNER_ALLELE_A_SUFFIX = "_PARTNER_ALLELE_A";

	public const string PARTNER_ALLELE_B_SUFFIX = "_PARTNER_ALLELE_B";

	public const string BONUS_SUFFIX = "_BONUS";

	public const string BONUS_PERCENT_SUFFIX = "_BONUS_PERCENT";

	public static float GetExpressedValue(GameItem item, string geneID)
	{
		if (string.IsNullOrEmpty(geneID) || !item.IsTag(geneID + "_ALLELE_A"))
		{
			Debug.LogWarning("[GeneticHelper] GetExpressedValue: invalid or uninitialized gene ID '" + geneID + "' on item '" + item?.identifier + "'.");
			return 0f;
		}
		float @float = item.GetTagReadonly(geneID + "_ALLELE_A").GetFloat();
		float float2 = item.GetTagReadonly(geneID + "_ALLELE_B").GetFloat();
		return (@float + float2) / 2f;
	}

	public static float GetCurrentStat(GameItem item, string geneID)
	{
		if (string.IsNullOrEmpty(geneID) || !item.IsTag(geneID + "_ALLELE_A"))
		{
			Debug.LogWarning("[GeneticHelper] GetCurrentStat: invalid or uninitialized gene ID '" + geneID + "' on item '" + item?.identifier + "'.");
			return 0f;
		}
		float expressedValue = GetExpressedValue(item, geneID);
		float @float = item.GetTagReadonly(geneID + "_BONUS").GetFloat();
		float float2 = item.GetTagReadonly(geneID + "_BONUS_PERCENT").GetFloat();
		return (expressedValue + @float) * (1f + float2);
	}

	public static void ModBonusValue(GameItem item, string geneID, float bonusDelta)
	{
		if (string.IsNullOrEmpty(geneID) || !item.IsTag(geneID + "_ALLELE_A"))
		{
			Debug.LogWarning("[GeneticHelper] ModBonusValue: invalid or uninitialized gene ID '" + geneID + "' on item '" + item?.identifier + "'.");
			return;
		}
		item.ModifyTag(geneID + "_BONUS", delegate(TagState state)
		{
			float num = state.GetFloat() + bonusDelta;
			if (num == 0f)
			{
				state.Disable().SetFloat(0f);
			}
			else
			{
				state.Enable().SetFloat(num);
			}
		});
	}

	public static void ModBonusPercent(GameItem item, string geneID, float percentDelta)
	{
		if (string.IsNullOrEmpty(geneID) || !item.IsTag(geneID + "_ALLELE_A"))
		{
			Debug.LogWarning("[GeneticHelper] ModBonusPercent: invalid or uninitialized gene ID '" + geneID + "' on item '" + item?.identifier + "'.");
			return;
		}
		item.ModifyTag(geneID + "_BONUS_PERCENT", delegate(TagState state)
		{
			float num = state.GetFloat() + percentDelta;
			if (num == 0f)
			{
				state.Disable().SetFloat(0f);
			}
			else
			{
				state.Enable().SetFloat(num);
			}
		});
	}

	public static void InitGenetics(GameItem item, AnimalSpecies species)
	{
		foreach (string geneticTraitID in species.geneticTraitIDs)
		{
			float baseValue = species.geneticBaseValues[geneticTraitID];
			float alleleA = RandomizeAllele(baseValue);
			float alleleB = RandomizeAllele(baseValue);
			item.ModifyTag(geneticTraitID + "_ALLELE_A", delegate(TagState state)
			{
				state.Enable().SetFloat(alleleA);
			});
			item.ModifyTag(geneticTraitID + "_ALLELE_B", delegate(TagState state)
			{
				state.Enable().SetFloat(alleleB);
			});
			item.ModifyTag(geneticTraitID, delegate(TagState state)
			{
				state.Enable().SetFloat((alleleA + alleleB) / 2f);
			});
		}
	}

	public static void SetToDefault(GameItem item, AnimalSpecies species)
	{
		foreach (string geneticTraitID in species.geneticTraitIDs)
		{
			float baseValue = species.geneticBaseValues[geneticTraitID];
			item.ModifyTag(geneticTraitID + "_ALLELE_A", delegate(TagState state)
			{
				state.Enable().SetFloat(baseValue);
			});
			item.ModifyTag(geneticTraitID + "_ALLELE_B", delegate(TagState state)
			{
				state.Enable().SetFloat(baseValue);
			});
			item.ModifyTag(geneticTraitID, delegate(TagState state)
			{
				state.Enable().SetFloat(baseValue);
			});
		}
		float maxHealth = GetExpressedValue(item, "ANIMAL_MAX_HEALTH_TAG");
		item.ModifyTag("ANIMAL_HEALTH_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(maxHealth);
		});
		float fertility = GetExpressedValue(item, "ANIMAL_FERTILITY_TAG");
		item.ModifyTag("ANIMAL_CURRENT_FERTILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(fertility);
		});
	}

	private static float RandomizeAllele(float baseValue)
	{
		float num = (float)RNG.GetRandomDouble(0.5, 1.5);
		float num2 = (float)RNG.GetRandomDouble(0.5, 1.5);
		return baseValue * ((num + num2) / 2f);
	}

	public static void StorePartnerGenes(GameItem female, GameItem male, AnimalSpecies species)
	{
		foreach (string geneticTraitID in species.geneticTraitIDs)
		{
			float maleAlleleA = male.GetTagReadonly(geneticTraitID + "_ALLELE_A").GetFloat();
			float maleAlleleB = male.GetTagReadonly(geneticTraitID + "_ALLELE_B").GetFloat();
			female.ModifyTag(geneticTraitID + "_PARTNER_ALLELE_A", delegate(TagState state)
			{
				state.Enable().SetFloat(maleAlleleA);
			});
			female.ModifyTag(geneticTraitID + "_PARTNER_ALLELE_B", delegate(TagState state)
			{
				state.Enable().SetFloat(maleAlleleB);
			});
		}
	}

	public static void ClearPartnerGenes(GameItem female, AnimalSpecies species)
	{
		foreach (string geneticTraitID in species.geneticTraitIDs)
		{
			female.ModifyTag(geneticTraitID + "_PARTNER_ALLELE_A", delegate(TagState state)
			{
				state.Disable();
			});
			female.ModifyTag(geneticTraitID + "_PARTNER_ALLELE_B", delegate(TagState state)
			{
				state.Disable();
			});
		}
	}

	public static void ApplyInheritedGenetics(GameItem child, GameItem mother, AnimalSpecies species)
	{
		foreach (string geneticTraitID in species.geneticTraitIDs)
		{
			float baseValue = species.geneticBaseValues[geneticTraitID];
			float @float = mother.GetTagReadonly(geneticTraitID + "_ALLELE_A").GetFloat();
			float float2 = mother.GetTagReadonly(geneticTraitID + "_ALLELE_B").GetFloat();
			float float3 = mother.GetTagReadonly(geneticTraitID + "_PARTNER_ALLELE_A").GetFloat();
			float float4 = mother.GetTagReadonly(geneticTraitID + "_PARTNER_ALLELE_B").GetFloat();
			float inheritedA = (RNG.Roll(50) ? @float : float2);
			float inheritedB = (RNG.Roll(50) ? float3 : float4);
			inheritedA = MutateAllele(inheritedA, baseValue);
			inheritedB = MutateAllele(inheritedB, baseValue);
			child.ModifyTag(geneticTraitID + "_ALLELE_A", delegate(TagState state)
			{
				state.Enable().SetFloat(inheritedA);
			});
			child.ModifyTag(geneticTraitID + "_ALLELE_B", delegate(TagState state)
			{
				state.Enable().SetFloat(inheritedB);
			});
			float expressed = (inheritedA + inheritedB) / 2f;
			child.ModifyTag(geneticTraitID, delegate(TagState state)
			{
				state.Enable().SetFloat(expressed);
			});
		}
	}

	public static float MutateAllele(float value, float baseValue)
	{
		if (!RNG.Roll(15))
		{
			return value;
		}
		float mutationResistance = GetMutationResistance(value, baseValue);
		float num = 0.08f * (1f - mutationResistance);
		if (RNG.Roll(5))
		{
			num *= 3f;
		}
		float num2 = (float)RNG.GetRandomDouble(0f - num, num);
		if (value > baseValue)
		{
			num2 -= num * 0.25f * mutationResistance;
		}
		else if (value < baseValue)
		{
			num2 += num * 0.25f * mutationResistance;
		}
		return Mathf.Max(0.01f, value + value * num2);
	}

	public static float GetMutationResistance(float currentValue, float baseValue)
	{
		float num = Mathf.Abs(currentValue - baseValue) / baseValue;
		return 1f - 1f / (1f + num * 2f);
	}

	public static void SetTrait(GameItem item, AnimalSpecies species, string geneID, float multiplier)
	{
		float value2;
		float num = (species.geneticBaseValues.TryGetValue(geneID, out value2) ? value2 : 1f);
		float value = num * multiplier;
		item.ModifyTag(geneID + "_ALLELE_A", delegate(TagState state)
		{
			state.Enable().SetFloat(value);
		});
		item.ModifyTag(geneID + "_ALLELE_B", delegate(TagState state)
		{
			state.Enable().SetFloat(value);
		});
		item.ModifyTag(geneID, delegate(TagState state)
		{
			state.Enable().SetFloat(value);
		});
	}
}

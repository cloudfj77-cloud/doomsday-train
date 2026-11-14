public static class PlantGeneticsText
{
	private static string M(string key, string english, params object[] args)
	{
		return LocHelper.GetLocalizedMechanicOrDefault(key, english, args);
	}

	public static string TraitName(string traitId, string english)
	{
		return M("plant_trait_" + traitId + "_name", english);
	}

	public static string TraitDescription(string traitId, string english)
	{
		return M("plant_trait_" + traitId + "_desc", english);
	}

	public static string StatName(string statId, string english)
	{
		return LocHelper.GetLocalizedMechanic("plant_stat_" + statId);
	}

	public static string EffectComplexity(string signedOffset)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_effect_complexity", signedOffset);
	}

	public static string StatPercent(string statName, string signedPercent)
	{
		return statName + " " + signedPercent + "%";
	}

	public static string EffectSellPrice(string signedPercent)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_effect_sell_price", signedPercent);
	}

	public static string EffectActiveIngredients(string signedPercent)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_effect_active_ingredients", signedPercent);
	}

	public static string EffectPurifying(string percent, string ceilingPercent)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_effect_purifying", percent, ceilingPercent);
	}

	public static string EffectPolluting(string percent)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_effect_polluting", percent);
	}

	public static string ProductionName(string liquidName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_production_name", liquidName);
	}

	public static string ProductionDescription(string liquidName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_production_desc", liquidName);
	}

	public static string EffectProduction(string ml, string liquidName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_effect_production", ml, liquidName);
	}

	public static string EffectProductionContraband(string ml, string liquidName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_effect_production_contraband", ml, liquidName);
	}

	public static string ReadoutHeader(int complexity)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_readout_header", complexity);
	}

	public static string ReadoutHeaderHeat(int complexity, int heat)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_readout_header_heat", complexity, heat);
	}

	public static string ReadoutNoTraits()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_readout_no_traits");
	}

	public static string ReadoutTraitLine(string traitLabel, string effect)
	{
		return LocHelper.BulletLine(traitLabel + ": " + effect);
	}

	public static string TraitWithRank(string traitName, string romanRank)
	{
		return traitName + " " + romanRank;
	}

	public static string OriginLocked(string traitLabel)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_origin_locked", traitLabel);
	}

	public static string OriginInnate(string traitLabel)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_origin_innate", traitLabel);
	}

	public static string NoEffectSuffix(string traitLabel)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_no_effect_suffix", traitLabel);
	}

	public static string ReadoutInnateRemoved(string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_readout_innate_removed", traitName);
	}

	public static string DiskName()
	{
		return LocHelper.GetLocalizedItem("item_dna_disk_name");
	}

	public static string DiskBlankDescription()
	{
		return LocHelper.GetLocalizedItem("item_dna_disk_desc");
	}

	public static string NamedWithTrait(string name, string traitLabel)
	{
		return name + " (" + traitLabel + ")";
	}

	public static string DiskBlankTooltip()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_disk_blank_tooltip");
	}

	public static string DiskUsesRemaining(int uses)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_disk_uses_remaining", uses);
	}

	public static string TraitComplexity(string signedComplexity)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_trait_complexity", signedComplexity);
	}

	public static string TraitStacksTo(string romanRank)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_trait_stacks_to", romanRank);
	}

	public static string TraitExclusiveWith(string names)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_trait_exclusive_with", names);
	}

	public static string TraitNoEffectOn(string cropNames)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_trait_no_effect_on", cropNames);
	}

	public static string None()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_none");
	}

	public static string BlockNothingToApply()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_nothing_to_apply");
	}

	public static string BlockLockedApply(string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_locked_apply", traitName);
	}

	public static string BlockIncompatible(string traitName, string carriedTraitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_incompatible", traitName, carriedTraitName);
	}

	public static string BlockMaxRank(string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_max_rank", traitName);
	}

	public static string BlockExclusive(string blockingTraitName, string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_exclusive", blockingTraitName, traitName);
	}

	public static string BlockNoEffectOnCrop(string traitName, string cropName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_no_effect_on_crop", traitName, cropName);
	}

	public static string BlockNothingToActOn(string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_nothing_to_act_on", traitName);
	}

	public static string BlockLockedExtract(string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_locked_extract", traitName);
	}

	public static string BlockRefillMissing(string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_refill_missing", traitName);
	}

	public static string BlockPrecisionMissing(string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_precision_missing", traitName);
	}

	public static string BlockBothFilters()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_both_filters");
	}

	public static string BlockNoPositiveTrait()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_no_positive_trait");
	}

	public static string BlockNoNegativeTrait()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_no_negative_trait");
	}

	public static string BlockNoTrait()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_block_no_trait");
	}

	public static string DragRefused(string draggedName, string machineName, string reason)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_drag_refused", draggedName, machineName, reason);
	}

	public static string ManipulatorName()
	{
		return LocHelper.GetLocalizedItem("item_dna_manipulator_name");
	}

	public static string ManipulatorDescription()
	{
		return LocHelper.GetLocalizedItem("item_dna_manipulator_desc");
	}

	public static string LabelBattery()
	{
		return LocHelper.GetLocalizedMechanic("mech_hydroponic_label_battery");
	}

	public static string LabelModule()
	{
		return LocHelper.GetLocalizedMechanic("mech_hydroponic_label_module");
	}

	public static string LabelNote()
	{
		return LocHelper.GetLocalizedMechanic("mech_hydroponic_label_note");
	}

	public static string LabelTraitDisk()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_label_trait_disk");
	}

	public static string LabelTarget()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_label_target");
	}

	public static string LabelBlankDisk()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_label_blank_disk");
	}

	public static string LabelOutput()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_label_output");
	}

	public static string EjectOn()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_eject_on");
	}

	public static string EjectOff()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_eject_off");
	}

	public static string EjectHint()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_eject_hint");
	}

	public static string StatusApplying(string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_status_applying", traitName);
	}

	public static string StatusExtracting()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_status_extracting");
	}

	public static string StatusReady()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_status_ready");
	}

	public static string CannotStart(string reason)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_cannot_start", reason);
	}

	public static string NextApply(string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_next_apply", traitName);
	}

	public static string NextApplyStack(string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_next_apply_stack", traitName);
	}

	public static string NextExtractRandom()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_next_extract_random");
	}

	public static string NextExtractTrait(string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_next_extract_trait", traitName);
	}

	public static string NextExtractRefill(string traitName)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_next_extract_refill", traitName);
	}

	public static string TargetKept()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_target_kept");
	}

	public static string TargetDestroyed()
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_target_destroyed");
	}

	public static string SuccessChance(int percent)
	{
		return LocHelper.GetLocalizedMechanic("mech_dna_success_chance", percent);
	}

	public static string InstructionName()
	{
		return LocHelper.GetLocalizedItem("item_dna_manipulator_instruction_name");
	}

	public static string InstructionShort()
	{
		return LocHelper.GetLocalizedItem("item_dna_manipulator_instruction_short");
	}

	public static string InstructionText()
	{
		return LocHelper.GetLocalizedItem("item_dna_manipulator_instruction_flavor");
	}

	public static string StabilizerName()
	{
		return LocHelper.GetLocalizedItem("item_dna_module_stabilizer_name");
	}

	public static string StabilizerDescription()
	{
		return LocHelper.GetLocalizedItem("item_dna_module_stabilizer_desc");
	}

	public static string StabilizerEffect()
	{
		return LocHelper.GetLocalizedItem("item_dna_module_stabilizer_effect");
	}

	public static string PositiveFilterName()
	{
		return LocHelper.GetLocalizedItem("item_dna_module_positive_filter_name");
	}

	public static string PositiveFilterDescription()
	{
		return LocHelper.GetLocalizedItem("item_dna_module_positive_filter_desc");
	}

	public static string PositiveFilterEffect()
	{
		return LocHelper.GetLocalizedItem("item_dna_module_positive_filter_effect");
	}

	public static string NegativeFilterName()
	{
		return LocHelper.GetLocalizedItem("item_dna_module_negative_filter_name");
	}

	public static string NegativeFilterDescription()
	{
		return LocHelper.GetLocalizedItem("item_dna_module_negative_filter_desc");
	}

	public static string NegativeFilterEffect()
	{
		return LocHelper.GetLocalizedItem("item_dna_module_negative_filter_effect");
	}

	public static string PrecisionName()
	{
		return LocHelper.GetLocalizedItem("item_dna_module_precision_name");
	}

	public static string PrecisionDescription()
	{
		return LocHelper.GetLocalizedItem("item_dna_module_precision_desc");
	}

	public static string PrecisionEffect()
	{
		return LocHelper.GetLocalizedItem("item_dna_module_precision_effect");
	}
}

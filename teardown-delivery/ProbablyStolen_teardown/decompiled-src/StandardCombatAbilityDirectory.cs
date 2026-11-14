public class StandardCombatAbilityDirectory : CombatAbilityDirectory
{
	protected override void InitDirectory()
	{
		Add("nothing", () => StandardAbility("Nothing", "", 1000, false, "null"));
		Add("combat_scav", () => StandardAbility("Combat Scavenging", "", 1000, false, "combat_scavenge"));
		Add("smoke_grenade", () => StandardAbility("Tear Gas Grenade", "", 1000, false, "smoke_accuracy"));
		Add("flashbang", () => StandardAbility("Flashbang Grenade", "", 1000, false, "flashbang_stun"));
		Add("frag_grenade", () => StandardAbility("Fragmentation Grenade", "", 1000, false, "frag_damage"));
		Add("c4", () => StandardAbility("C4", "Plan a Composition 4 plastic explosive charge", 1000, false, "event_c4_effect"));
		Add("weapon_attack", () => StandardAbility("Weapon Attack", "Attack using a weapon", 0, true, "weapon_attack"));
		Add("mp9_attack_ability", () => StandardAbility("MP9 Attack", "Attack using a weapon", 5, true, "mp9_attack"));
		Add("glock_attack_ability", () => StandardAbility("Antique Handgun Attack", "Attack using a weapon", 5, true, "glock_attack"));
		Add("glock_attack_ability2", () => StandardAbility("Antique Handgun Attack", "Attack using a weapon", 10, true, "glock_attack"));
		Add("ump_attack_ability", () => StandardAbility("UMP Attack", "Attack using a weapon", 5, true, "ump_attack"));
		Add("overwatch", () => StandardAbility("Overwatch", "This crew will attack all attacking enemie", 1000, false, "overwatch_effect"));
		Add("retaliate", () => StandardAbility("Retaliate", "This crew will retaliate against all enemis targeting them", 1000, false, "retaliate_effect"));
		Add("stun_baton", () => StandardAbility("Stun Baton Attack", "", 5, true, "stun_baton_damage", "stun_baton_stun"));
		Add("taser", () => StandardAbility("Taser Attack", "", -5, false, "taser_stun"));
		Add("double_tap_attack", () => StandardAbility("Double Tap Attack", "Attack twice using a weapon", -15, true, "weapon_attack", "weapon_attack"));
		Add("combat_stim", () => StandardAbility("Combat Stimulant", "Administer a dose of Combat Stimulant which will improve the focus and perception of the reciprient", 1000, false, "combat_stim_acc_buff", "combat_stim_cc_buff"));
		Add("experimental_injector_red", () => StandardAbility("Experimental Injector (Red)", "", 1000, false, "red_stim_heal", "red_stim_bleed"));
		Add("experimental_injector_black", () => StandardAbility("Experimental Injector (Black)", "???", 1000, false, "null"));
		Add("experimental_injector_green", () => StandardAbility("Experimental Injector (Green)", "", 1000, false, "green_stim_heal"));
		Add("experimental_injector_white", () => StandardAbility("Experimental Injector (White)", "", 1000, false, "white_stim_heal"));
		Add("experimental_injector_purple", () => StandardAbility("Experimental Injector (Purple)", "", 1000, false, "white_stim_heal", "green_stim_heal", "red_stim_heal", "red_stim_bleed"));
		Add("birth_control_pill", () => StandardAbility("Birth Control Pill", "???", 1000, false, "null"));
		Add("bicarsole_pill", () => StandardAbility("Bicarsole Pill", "", 1000, false, "bicarsole_heal"));
		Add("food", () => StandardAbility("Food", "", 1000, false, "null"));
		Add("knife_attack", () => StandardAbility("Knife Attack", "Attack with the Kitchen Knife", 30, true, "knife_attack", "bleed_light"));
		Add("combat_knife_attack", () => StandardAbility("Combat Knife Attack", "", 30, true, "combat_knife_attack", "bleed_light"));
		Add("cleaver_attack", () => StandardAbility("Cleaver Attack", "Attack with the Cleaver Knife", 20, true, "cleaver_attack", "bleed_heavy"));
		Add("combat_machete_attack", () => StandardAbility("Combat Machete Attack", "", 25, true, "combat_machete_attack", "bleed_heavy"));
		Add("baseball_bat_attack", () => StandardAbility("Baseball Bat Attack", "Attack with the Baseball Bat", 25, true, "baseball_bat_attack", "blunt_melee_stun"));
		Add("lead_pipe_attack", () => StandardAbility("Lead Pipe Attack", "Attack with the Lead Pipe", 30, true, "lead_pipe_attack", "blunt_melee_stun"));
		Add("bandage_ability", () => StandardAbility("Bandage", "", 1000, false, "bandage_stop_bleed"));
		Add("stop_block_bleed", () => StandardAbility("Experimental Injector (Black)", "", 1000, false, "stop_bleed", "coagulent"));
		Add("mirage_generator", () => StandardAbility("Mirage Projector", "", 1000, false, "mirage_dodge"));
		Add("dev_damage", () => StandardAbility("Sandal Attack", "", 1000, false, "dev_damage_all"));
		Add("mark", () => StandardAbility("Mark", "", 1000, false, "mark_dodge_debuff", "mark_priority_increase"));
		Add("medical_proficiency", () => StandardAbility("Medical Equipment Proficiency", "This person is experienced with medical equipment and can use most medical equipment once per turn without ending their turn.", 1000, false, "null"));
		Add("medical_proficiency_2", () => StandardAbility("Medical Equipment Proficiency II", "This person is experienced with medical equipment and can use most medical equipment twice per turn without ending their turn.", 1000, false, "null"));
		Add("diversion_ability", () => StandardAbility("Diversion", "", 1000, false, "diversion_priority_increase"));
		Add("spotter_ability", () => StandardAbility("Spotter", "", 1000, false, "spotter_accuracy_buff"));
		Add("take_aim_ability", () => StandardAbility("Take Aim", "", 1000, false, "take_aim_acc_buff", "take_aim_crit_chance_buff", "take_aim_dodge_debuff"));
		Add("mark_ability", () => StandardAbility("Mark", "", 1000, false, "mark_dodge_debuff", "mark_priority_increase"));
		Add("medical_assist_ability", () => StandardAbility("Medical Assist", "", 1000, false, "medical_assist_stun_reduce"));
		Add("brace_ability", () => StandardAbility("Brace", "", 1000, false, "brace_priority_decrease", "brace_dodge_buff"));
	}

	private CombatAbility StandardAbility(string name = "", string description = "", int bonusAccuracy = 1000, bool triggerOverwatch = false, params string[] effectsList)
	{
		CombatAbility combatAbility = Empty();
		combatAbility.name = name;
		combatAbility.shortDescription = description;
		combatAbility.bonusAccuracy = bonusAccuracy;
		combatAbility.triggerOverwatch = triggerOverwatch;
		foreach (string identifier in effectsList)
		{
			combatAbility.effects.Add(DirectoryMaster.CombatAbilityEffect(identifier));
		}
		return combatAbility;
	}
}

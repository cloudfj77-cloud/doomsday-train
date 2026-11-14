using System.Collections.Generic;

public interface CombatAbility : DirectoryEntry
{
	const string ACTION_FREE_MODE = "action_free_mode";

	const string NONE_PROFICIENCY = "none";

	const string MEDICAL_PROFICIENCY = "MEDICAL_PROFICIENCY";

	const string MEDICAL_PROFICIENCY_CURRENT = "MEDICAL_PROFICIENCY_CURRENT";

	const string TECHNICAL_PROFICIENCY = "TECHNICAL_PROFICIENCY";

	const string TECHNICAL_PROFICIENCY_CURRENT = "TECHNICAL_PROFICIENCY_CURRENT";

	const string TACTICAL_PROFICIENCY = "TACTICAL_PROFICIENCY";

	const string TACTICAL_PROFICIENCY_CURRENT = "TACTICAL_PROFICIENCY_CURRENT";

	const string GHETTO_PROFICIENCY = "GHETTO_PROFICIENCY";

	const string GHETTO_PROFICIENCY_CURRENT = "GHETTO_PROFICIENCY_CURRENT";

	const string PROFICIENCY = "proficiency";

	const string _CURRENT = "_CURRENT";

	const int ALWAYSACCURATE = 1000;

	string name { get; set; }

	string shortDescription { get; set; }

	int bonusAccuracy { get; set; }

	List<CombatAbilityEffect> effects { get; set; }

	bool triggerOverwatch { get; set; }

	CombatAbilityEffect.TargetType GetPrimaryTarget();

	bool IsAlwaysAccurate();
}

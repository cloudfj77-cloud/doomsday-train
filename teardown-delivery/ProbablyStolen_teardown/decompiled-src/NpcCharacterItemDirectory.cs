public class NpcCharacterItemDirectory : CharacterItemDirectory
{
	protected override void InitDirectory()
	{
		Add("raider_technician", () => CharacterItemDirectory.Humanoid("Hostile Raider", isEnemy: true, DirectoryCombatEntity.Instance.BuildFieldTechnician));
		Add("raider_scav", () => CharacterItemDirectory.Humanoid("Hostile Raider", isEnemy: true, DirectoryCombatEntity.Instance.BuildScav));
		Add("raider_medic", () => CharacterItemDirectory.Humanoid("Hostile Raider", isEnemy: true, DirectoryCombatEntity.Instance.BuildCombatMedic));
		Add("raider_marine", () => CharacterItemDirectory.Humanoid("Hostile Raider", isEnemy: true, DirectoryCombatEntity.Instance.BuildMarine));
		Add("guard_marine", () => CharacterItemDirectory.Humanoid("Station Guard", Allegiance.Neutral, DirectoryCombatEntity.Instance.BuildMarine));
		Add("merchant", () => CharacterItemDirectory.Humanoid("Merchant", Allegiance.Neutral, DirectoryCombatEntity.Instance.BuildScav));
		Add("raider_scav_murderous", () => CharacterItemDirectory.Humanoid("Murderous Raider", isEnemy: true, DirectoryCombatEntity.Instance.BuildScav, DirectoryCombatAI.Instance.CreateMurderousAI()));
		Add("raider_technician_murderous", () => CharacterItemDirectory.Humanoid("Murderous Raider", isEnemy: true, DirectoryCombatEntity.Instance.BuildFieldTechnician, DirectoryCombatAI.Instance.CreateMurderousAI()));
		Add("raider_medic_murderous", () => CharacterItemDirectory.Humanoid("Murderous Raider", isEnemy: true, DirectoryCombatEntity.Instance.BuildCombatMedic, DirectoryCombatAI.Instance.CreateMurderousAI()));
		Add("raider_marine_overwatch", () => CharacterItemDirectory.Humanoid("Overwatch Raider", isEnemy: true, DirectoryCombatEntity.Instance.BuildMarine, DirectoryCombatAI.Instance.CreateOverwatchAI()));
		Add("raider_marine_sadistic", () => CharacterItemDirectory.Humanoid("Sadistic Raider", isEnemy: true, DirectoryCombatEntity.Instance.BuildMarine, DirectoryCombatAI.Instance.CreateSadisticAI()));
		Add("raider_boss", () => CharacterItemDirectory.Humanoid("Raider Boss", isEnemy: true, DirectoryCombatEntity.Instance.BuildRaiderBoss, DirectoryCombatAI.Instance.CreateRaiderBossAI()));
		Add("raider_junker", () => DirectoryCombatEntity.Instance.CreateJunker("Raider Junker", Allegiance.Hostile, DirectoryCombatAI.Instance.CreateJunkerAI()));
		Add("deserter_marine", () => CreateDeserterMarine());
		Add("deserter_combat_medic", () => CreateDeserterCombatMedic());
		Add("xeno_herald", () => CreateXenoHerald());
		Add("xeno_hunter", () => CreateXenoHunter());
		Add("xeno_brewer", () => CreateXenoBrewer());
		Add("xeno_drone", () => CreateXenoDrone());
		Add("pirate_rookie", () => CreatePirateRookie());
		Add("pirate_veteran", () => CreatePirateVeteran());
		Add("pirate_chief", () => CreatePirateChief());
	}

	public static GameCharacterItem CreateDeserterMarine()
	{
		return DirectoryCombatEntity.Instance.CreateMarine("Deserter Marine", Allegiance.Hostile, DirectoryCombatAI.Instance.CreateDeserterMarineAI());
	}

	public static GameCharacterItem CreateDeserterCombatMedic()
	{
		return DirectoryCombatEntity.Instance.CreateCombatMedic("Deserter Combat Medic", Allegiance.Hostile, DirectoryCombatAI.Instance.CreateDeserterCombatMedicAI());
	}

	public static GameCharacterItem CreateXenoHerald()
	{
		return DirectoryCombatEntity.Instance.CreateXenoHerald("Xeno Herald", Allegiance.Hostile, DirectoryCombatAI.Instance.CreateXenoHerealdAI());
	}

	public static GameCharacterItem CreateXenoHunter()
	{
		return DirectoryCombatEntity.Instance.CreateXenoHunter("Xeno Hunter", Allegiance.Hostile, DirectoryCombatAI.Instance.CreateXenoHunterAI());
	}

	public static GameCharacterItem CreateXenoBrewer()
	{
		return DirectoryCombatEntity.Instance.CreateXenoBrewer("Xeno Brewer", Allegiance.Hostile, DirectoryCombatAI.Instance.CreateXenoBrewerAI());
	}

	public static GameCharacterItem CreateXenoDrone()
	{
		return DirectoryCombatEntity.Instance.CreateXenoDrone("Xeno Drone", Allegiance.Hostile, DirectoryCombatAI.Instance.CreateXenoDroneAI());
	}

	public static GameCharacterItem CreatePirateRookie()
	{
		return DirectoryCombatEntity.Instance.CreatePirateRookie("Pirate Rookie", Allegiance.Hostile, DirectoryCombatAI.Instance.CreatePirateRookieAI());
	}

	public static GameCharacterItem CreatePirateVeteran()
	{
		return DirectoryCombatEntity.Instance.CreateMarine("Pirate Veteran", Allegiance.Hostile, DirectoryCombatAI.Instance.CreatePirateRookieAI());
	}

	public static GameCharacterItem CreatePirateChief()
	{
		return DirectoryCombatEntity.Instance.CreateMarine("Pirate Chief", Allegiance.Hostile, DirectoryCombatAI.Instance.CreatePirateRookieAI());
	}
}

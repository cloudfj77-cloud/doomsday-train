public class PlayerCharacterItemDirectory : CharacterItemDirectory
{
	protected override void InitDirectory()
	{
		Add("albert", () => DirectoryCombatEntity.Instance.CreateTrooper("Albert", Allegiance.Player));
		Add("benjamin", () => DirectoryCombatEntity.Instance.CreateCombatTechnician("Benjamin", Allegiance.Player));
		Add("charlotte", () => DirectoryCombatEntity.Instance.CreateCombatMedic("Charlotte", Allegiance.Player));
		Add("daniel", () => DirectoryCombatEntity.Instance.CreateMarine("Daniel", Allegiance.Player));
	}
}

public class TechnicianBackpackDirectory : ItemDirectory
{
	protected override void InitDirectory()
	{
	}

	private GameItem Capacitor()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "coin");
		gameItem.name = "Capacitor";
		TechnicianBackpackHelper.InitComplexeModule(gameItem, 0, 5);
		return gameItem;
	}

	private GameItem Charger()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "coin");
		gameItem.name = "Charger";
		TechnicianBackpackHelper.InitComplexeModule(gameItem, 1);
		return gameItem;
	}

	private GameItem ComplexeEfficiencyModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "coin");
		gameItem.name = "Compact Efficiency Module";
		TechnicianBackpackHelper.InitComplexeModule(gameItem, 0, 0, adjacent: true, -1);
		return gameItem;
	}

	private GameItem ComplexeEffectivenessModule(int effectiveness)
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "coin");
		gameItem.name = "Compact Effectiveness Module";
		TechnicianBackpackHelper.InitComplexeModule(gameItem, 0, 0, adjacent: true, 1, effectiveness);
		return gameItem;
	}

	private GameItem TurretCompactSystem()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "coin");
		PlayerAbilityItemDirectory.InitCombatItem(gameItem, new string[1] { "ump_attack" }, 0, "Turret Compact System", canUseOutsideCombat: false, triggerOverwatch: true);
		TechnicianBackpackHelper.InitComplexeSystem(gameItem, 5);
		AudioHelper.InitUMP(gameItem);
		gameItem.EnableProficiency("TECHNICAL_PROFICIENCY");
		return gameItem;
	}

	private GameItem LaserAutocanonSystem()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "coin");
		PlayerAbilityItemDirectory.InitCombatItem(gameItem, new string[1] { "laser_autocanon_attack" }, 0, "Laser Autocanon Compact System", canUseOutsideCombat: false, triggerOverwatch: true);
		TechnicianBackpackHelper.InitComplexeSystem(gameItem, 10);
		AudioHelper.InitUMP(gameItem);
		gameItem.EnableProficiency("TECHNICAL_PROFICIENCY");
		return gameItem;
	}

	private GameItem LoudspeakerCompactSystem()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "coin");
		PlayerAbilityItemDirectory.InitCombatItem(gameItem, new string[1] { "ump_attack" }, 0, "Loud Speaker Compact System", canUseOutsideCombat: false, triggerOverwatch: true);
		TechnicianBackpackHelper.InitComplexeSystem(gameItem, 5);
		AudioHelper.InitUMP(gameItem);
		gameItem.EnableProficiency("TECHNICAL_PROFICIENCY");
		return gameItem;
	}

	private GameItem TargetPainterCompactSystem()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "coin");
		PlayerAbilityItemDirectory.InitCombatItem(gameItem, new string[1] { "ump_attack" }, 0, "Target Painter Compact System", canUseOutsideCombat: false, triggerOverwatch: true);
		TechnicianBackpackHelper.InitComplexeSystem(gameItem, 5);
		AudioHelper.InitUMP(gameItem);
		gameItem.EnableProficiency("TECHNICAL_PROFICIENCY");
		return gameItem;
	}
}

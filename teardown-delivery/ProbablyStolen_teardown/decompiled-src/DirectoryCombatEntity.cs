public class DirectoryCombatEntity
{
	private static DirectoryCombatEntity instance = null;

	private static readonly object padlock = new object();

	private const int DEFAULT_PLAYER_DEATH_RATE = 40;

	public const int UNIVERSAL_BASIC_ACCURACY = 70;

	public const int XENO_BASIC_ACCURACY = 50;

	public static DirectoryCombatEntity Instance
	{
		get
		{
			lock (padlock)
			{
				if (instance == null)
				{
					instance = new DirectoryCombatEntity();
				}
				return instance;
			}
		}
	}

	private DirectoryCombatEntity()
	{
	}

	public CombatEntity BuildCombatMedic(CombatEntity gameCharacterItem, CombatAI combatAi = null, bool isPlayer = false)
	{
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.name = "Combat Medic";
		gameCharacterItem.reaction = 10;
		gameCharacterItem.accuracyPercentage += 5;
		if (combatAi != null)
		{
			gameCharacterItem.combatAI = combatAi;
		}
		if (isPlayer)
		{
			gameCharacterItem.deathchance = 40;
		}
		return gameCharacterItem;
	}

	public CombatEntity BuildMarine(CombatEntity gameCharacterItem, CombatAI combatAi = null, bool isPlayer = false)
	{
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.name = "Marine";
		gameCharacterItem.reaction = 11;
		gameCharacterItem.accuracyPercentage += 10;
		if (combatAi != null)
		{
			gameCharacterItem.combatAI = combatAi;
		}
		if (isPlayer)
		{
			gameCharacterItem.deathchance = 40;
		}
		return gameCharacterItem;
	}

	public CombatEntity BuildScav(CombatEntity gameCharacterItem, CombatAI combatAi = null, bool isPlayer = false)
	{
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.name = "Scav";
		gameCharacterItem.maxHealth = 90;
		gameCharacterItem.health = gameCharacterItem.maxHealth;
		gameCharacterItem.reaction = 7;
		gameCharacterItem.accuracyPercentage = gameCharacterItem.accuracyPercentage;
		if (combatAi != null)
		{
			gameCharacterItem.combatAI = combatAi;
		}
		if (isPlayer)
		{
			gameCharacterItem.deathchance = 40;
		}
		return gameCharacterItem;
	}

	public CombatEntity BuildFieldTechnician(CombatEntity gameCharacterItem, CombatAI combatAi = null, bool isPlayer = false)
	{
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.name = "Field Technician";
		gameCharacterItem.accuracyPercentage = gameCharacterItem.accuracyPercentage;
		if (combatAi != null)
		{
			gameCharacterItem.combatAI = combatAi;
		}
		if (isPlayer)
		{
			gameCharacterItem.deathchance = 40;
		}
		gameCharacterItem.reaction = 8;
		return gameCharacterItem;
	}

	public CombatEntity BuildRaiderBoss(CombatEntity gameCharacterItem, CombatAI combatAi = null, bool isPlayer = false)
	{
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.name = "Raider Boss";
		gameCharacterItem.accuracyPercentage = 75;
		if (combatAi != null)
		{
			gameCharacterItem.combatAI = combatAi;
		}
		if (isPlayer)
		{
			gameCharacterItem.deathchance = 40;
		}
		gameCharacterItem.reaction = 9;
		return gameCharacterItem;
	}

	public GameCharacterItem CreateJunker(string name, CharacterItemDirectory.Allegiance allegiance, CombatAI combatAi = null)
	{
		GameCharacterItem gameCharacterItem = CharacterItemDirectory.Humanoid(name, allegiance);
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.accuracyPercentage += 5;
		gameCharacterItem.reaction = 8;
		gameCharacterItem.ModifyTag("class_name", delegate(TagState state)
		{
			state.Enable().SetString("Junker");
		});
		GCIInit(gameCharacterItem, allegiance, combatAi);
		return gameCharacterItem;
	}

	public GameCharacterItem CreateMarine(string name, CharacterItemDirectory.Allegiance allegiance, CombatAI combatAi = null)
	{
		GameCharacterItem gameCharacterItem = CharacterItemDirectory.Humanoid(name, allegiance);
		gameCharacterItem.SetCombatStateSprites("Characters0", 0);
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.accuracyPercentage += 10;
		gameCharacterItem.reaction = 11;
		gameCharacterItem.ModifyTag("class_name", delegate(TagState state)
		{
			state.Enable().SetString("Marine");
		});
		GCIInit(gameCharacterItem, allegiance, combatAi);
		return gameCharacterItem;
	}

	public GameCharacterItem CreatePirateRookie(string name, CharacterItemDirectory.Allegiance allegiance, CombatAI combatAi = null)
	{
		GameCharacterItem gameCharacterItem = CharacterItemDirectory.Humanoid(name, allegiance);
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.accuracyPercentage += 10;
		gameCharacterItem.reaction = 9;
		gameCharacterItem.maxHealth = 80;
		gameCharacterItem.health = 80;
		gameCharacterItem.ModifyTag("class_name", delegate(TagState state)
		{
			state.Enable().SetString("Pirate");
		});
		GCIInit(gameCharacterItem, allegiance, combatAi);
		return gameCharacterItem;
	}

	public GameCharacterItem CreateCombatMedic(string name, CharacterItemDirectory.Allegiance allegiance, CombatAI combatAi = null)
	{
		GameCharacterItem gameCharacterItem = CharacterItemDirectory.Humanoid(name, allegiance);
		gameCharacterItem.SetCombatStateSprites("Characters0", 2);
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.accuracyPercentage += 5;
		gameCharacterItem.reaction = 10;
		gameCharacterItem.ModifyTag("class_name", delegate(TagState state)
		{
			state.Enable().SetString("Combat Medic");
		});
		GCIInit(gameCharacterItem, allegiance, combatAi);
		return gameCharacterItem;
	}

	public GameCharacterItem CreateCombatTechnician(string name, CharacterItemDirectory.Allegiance allegiance, CombatAI combatAi = null)
	{
		GameCharacterItem gameCharacterItem = CharacterItemDirectory.Humanoid(name, allegiance);
		gameCharacterItem.SetCombatStateSprites("Characters0", 3);
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.accuracyPercentage += 5;
		gameCharacterItem.maxHealth -= 15;
		gameCharacterItem.health = gameCharacterItem.maxHealth;
		gameCharacterItem.reaction = 9;
		gameCharacterItem.ModifyTag("class_name", delegate(TagState state)
		{
			state.Enable().SetString("Combat Technician");
		});
		GCIInit(gameCharacterItem, allegiance, combatAi);
		return gameCharacterItem;
	}

	public GameCharacterItem CreateTrooper(string name, CharacterItemDirectory.Allegiance allegiance, CombatAI combatAi = null)
	{
		GameCharacterItem gameCharacterItem = CharacterItemDirectory.Humanoid(name, allegiance);
		gameCharacterItem.SetCombatStateSprites("Characters0", 1);
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.accuracyPercentage = gameCharacterItem.accuracyPercentage;
		gameCharacterItem.maxHealth += 15;
		gameCharacterItem.health = gameCharacterItem.maxHealth;
		gameCharacterItem.reaction = 9;
		gameCharacterItem.ModifyTag("class_name", delegate(TagState state)
		{
			state.Enable().SetString("Trooper");
		});
		GCIInit(gameCharacterItem, allegiance, combatAi);
		return gameCharacterItem;
	}

	public GameCharacterItem CreateSharpshooter(string name, CharacterItemDirectory.Allegiance allegiance, CombatAI combatAi = null)
	{
		GameCharacterItem gameCharacterItem = CharacterItemDirectory.Humanoid(name, allegiance);
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.accuracyPercentage += 10;
		gameCharacterItem.maxHealth -= 15;
		gameCharacterItem.health = gameCharacterItem.maxHealth;
		gameCharacterItem.reaction = 11;
		gameCharacterItem.ModifyTag("class_name", delegate(TagState state)
		{
			state.Enable().SetString("SharpShooter");
		});
		GCIInit(gameCharacterItem, allegiance, combatAi);
		return gameCharacterItem;
	}

	public GameCharacterItem CreateXenoHerald(string name, CharacterItemDirectory.Allegiance allegiance, CombatAI combatAi = null)
	{
		GameCharacterItem gameCharacterItem = CharacterItemDirectory.Humanoid(name, allegiance);
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.accuracyPercentage = gameCharacterItem.accuracyPercentage;
		gameCharacterItem.reaction = 10;
		gameCharacterItem.ModifyTag("class_name", delegate(TagState state)
		{
			state.Enable().SetString("Xeno Herald");
		});
		gameCharacterItem.maxHealth += 10;
		gameCharacterItem.health = gameCharacterItem.maxHealth;
		GCIInit(gameCharacterItem, allegiance, combatAi);
		gameCharacterItem.SetSpecies("SPECIES_XENO");
		return gameCharacterItem;
	}

	public GameCharacterItem CreateXenoHunter(string name, CharacterItemDirectory.Allegiance allegiance, CombatAI combatAi = null)
	{
		GameCharacterItem gameCharacterItem = CharacterItemDirectory.Humanoid(name, allegiance);
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.accuracyPercentage = gameCharacterItem.accuracyPercentage;
		gameCharacterItem.reaction = 12;
		gameCharacterItem.ModifyTag("class_name", delegate(TagState state)
		{
			state.Enable().SetString("Xeno Hunter");
		});
		GCIInit(gameCharacterItem, allegiance, combatAi);
		gameCharacterItem.SetSpecies("SPECIES_XENO");
		return gameCharacterItem;
	}

	public GameCharacterItem CreateXenoBrewer(string name, CharacterItemDirectory.Allegiance allegiance, CombatAI combatAi = null)
	{
		GameCharacterItem gameCharacterItem = CharacterItemDirectory.Humanoid(name, allegiance);
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.accuracyPercentage = gameCharacterItem.accuracyPercentage;
		gameCharacterItem.reaction = 9;
		gameCharacterItem.ModifyTag("class_name", delegate(TagState state)
		{
			state.Enable().SetString("Xeno Brewer");
		});
		GCIInit(gameCharacterItem, allegiance, combatAi);
		gameCharacterItem.SetSpecies("SPECIES_XENO");
		return gameCharacterItem;
	}

	public GameCharacterItem CreateXenoDrone(string name, CharacterItemDirectory.Allegiance allegiance, CombatAI combatAi = null)
	{
		GameCharacterItem gameCharacterItem = CharacterItemDirectory.Humanoid(name, allegiance);
		gameCharacterItem.maxAction = 1;
		gameCharacterItem.action = 1;
		gameCharacterItem.accuracyPercentage = gameCharacterItem.accuracyPercentage;
		gameCharacterItem.reaction = 8;
		gameCharacterItem.maxHealth -= 10;
		gameCharacterItem.health = gameCharacterItem.maxHealth;
		gameCharacterItem.ModifyTag("class_name", delegate(TagState state)
		{
			state.Enable().SetString("Xeno Drone");
		});
		GCIInit(gameCharacterItem, allegiance, combatAi);
		gameCharacterItem.SetSpecies("SPECIES_XENO");
		return gameCharacterItem;
	}

	public void GCIInit(GameCharacterItem gci, bool isEnemy, CombatAI combatAi)
	{
		if (combatAi != null)
		{
			gci.combatAI = combatAi;
		}
		if (!isEnemy)
		{
			gci.deathchance = 40;
		}
	}

	public void GCIInit(GameCharacterItem gci, CharacterItemDirectory.Allegiance allegiance, CombatAI combatAi)
	{
		if (combatAi != null)
		{
			gci.combatAI = combatAi;
		}
		if (allegiance == CharacterItemDirectory.Allegiance.Player)
		{
			gci.deathchance = 40;
		}
	}
}

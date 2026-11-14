using System;

public abstract class CombatAbilityDirectory : Directory<CombatAbility>
{
	public override CombatAbility Empty()
	{
		return CreateEmptyCombatAbility();
	}

	public static CombatAbility CreateEmptyCombatAbility(string identifier = null)
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem(identifier);
		((DirectoryEntry)gameItem).identifier = identifier;
		return gameItem;
	}

	public static CombatAbility CreateEmptyCombatAbility<K>(K identifierValue) where K : Enum
	{
		string identifier = TagSystem.EnumToString(identifierValue);
		GameItem gameItem = ItemDirectory.CreateEmptyItem(identifier);
		gameItem.identifier = identifier;
		gameItem.identifierName = TagSystem.EnumToDisplayString(identifierValue);
		return gameItem;
	}
}

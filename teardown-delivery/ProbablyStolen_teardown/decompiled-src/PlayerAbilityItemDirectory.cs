using System;

public class PlayerAbilityItemDirectory : ItemDirectory
{
	protected override void InitDirectory()
	{
	}

	private GameItem MedicalProficiency(string spriteName, string[] effectList, int level)
	{
		string text = "Medical Equipment Proficiency";
		if (level == 2)
		{
			text += " II";
		}
		GameItem gameItem = CreateStandardCombatAbilityItem(spriteName, effectList, 1000, text);
		gameItem.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			GameCharacterItem gameCharacterItem2 = GraphUtils.FindParentType<GameCharacterItem>(inv);
			if (gameCharacterItem2 != null)
			{
				gameCharacterItem2.EnableTag("MEDICAL_PROFICIENCY");
				gameCharacterItem2.SetTagInt("MEDICAL_PROFICIENCY", level);
				gameCharacterItem2.EnableTag("MEDICAL_PROFICIENCY_CURRENT");
				gameCharacterItem2.SetTagInt("MEDICAL_PROFICIENCY_CURRENT", level);
			}
		});
		gameItem.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			GameCharacterItem gameCharacterItem = GraphUtils.FindParentType<GameCharacterItem>(inv);
			if (gameCharacterItem != null)
			{
				gameCharacterItem.DisableTag("MEDICAL_PROFICIENCY");
				gameCharacterItem.SetTagInt("MEDICAL_PROFICIENCY", 0);
				gameCharacterItem.DisableTag("MEDICAL_PROFICIENCY_CURRENT");
				gameCharacterItem.SetTagInt("MEDICAL_PROFICIENCY_CURRENT", 0);
			}
		});
		return gameItem;
	}

	private GameItem GenericProficiency(string spriteName, int level, string proficiencyType, string proficiencyCurrent, string proficiencyName, string affectedItemName)
	{
		string text = proficiencyName;
		string text2 = "once";
		if (level == 2)
		{
			text += " II";
			text2 = "twice";
		}
		string[] effectList = new string[1] { "null" };
		GameItem gameItem = CreateStandardCombatAbilityItem(spriteName, effectList, 1000, text);
		gameItem.shortDescription = "This person is experienced with " + affectedItemName + " and can use most " + affectedItemName + " during combat " + text2 + " per turn without ending their turn.";
		gameItem.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			GameCharacterItem gameCharacterItem2 = GraphUtils.FindParentType<GameCharacterItem>(inv);
			if (gameCharacterItem2 != null)
			{
				gameCharacterItem2.EnableTag(proficiencyType);
				gameCharacterItem2.SetTagInt(proficiencyType, level);
				gameCharacterItem2.EnableTag(proficiencyCurrent);
				gameCharacterItem2.SetTagInt(proficiencyCurrent, level);
			}
		});
		gameItem.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			GameCharacterItem gameCharacterItem = GraphUtils.FindParentType<GameCharacterItem>(inv);
			if (gameCharacterItem != null)
			{
				gameCharacterItem.DisableTag(proficiencyType);
				gameCharacterItem.SetTagInt(proficiencyType, 0);
				gameCharacterItem.DisableTag(proficiencyCurrent);
				gameCharacterItem.SetTagInt(proficiencyCurrent, 0);
			}
		});
		return gameItem;
	}

	private GameItem Diversion(string spriteName, string combatAbilityIdentifier)
	{
		GameItem gameItem = CreateStandardCombatAbilityItem(spriteName, new string[1] { "diversion_priority_increase" }, 1000, "Diversion");
		gameItem.canUseOutsideCombat = false;
		return gameItem;
	}

	private GameItem Spotter(string spriteName, string combatAbilityIdentifier)
	{
		GameItem gameItem = CreateStandardCombatAbilityItem(spriteName, new string[1] { "spotter_accuracy_buff" }, 1000, "Spotter");
		gameItem.canUseOutsideCombat = false;
		return gameItem;
	}

	private GameItem TakeAim(string spriteName, string combatAbilityIdentifier)
	{
		GameItem gameItem = CreateStandardCombatAbilityItem(spriteName, new string[1] { "take_aim_acc_buff, take_aim_reaction_buff" }, 1000, "Take Aim");
		gameItem.canUseOutsideCombat = false;
		return gameItem;
	}

	private GameItem MedicalAssist(string spriteName, string combatAbilityIdentifier)
	{
		GameItem gameItem = CreateGenericCombatAbilityItem(spriteName, combatAbilityIdentifier);
		gameItem.EnableProficiency("MEDICAL_PROFICIENCY");
		return gameItem;
	}

	private GameItem WaitAbilityItem(string spriteName, string combatAbilityIdentifier)
	{
		return CreateStandardCombatAbilityItem(spriteName, new string[1] { "wait" }, 1000, "Wait");
	}

	private GameItem OverwatchAbilityItem(string spriteName, string combatAbilityIdentifier)
	{
		GameItem gameItem = CreateStandardCombatAbilityItem(spriteName, new string[1] { "overwatch_effect" }, 1000, "Overwatch");
		gameItem.canUseOutsideCombat = false;
		return gameItem;
	}

	public static GameItem InitCombatItem(GameItem item, string[] effectList, int bonusAccuracy, string name, bool canUseOutsideCombat = true, bool triggerOverwatch = false)
	{
		item.name = name;
		item.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.canSelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.triggerOverwatch = triggerOverwatch;
		_ = effectList.LongLength;
		switch (DirectoryMaster.CombatAbilityEffect(effectList[0]).abilityTargetType)
		{
		case CombatAbilityEffect.TargetType.singleEnemy:
			item.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
			item.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
			break;
		case CombatAbilityEffect.TargetType.allEnemy:
			item.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
			item.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
			break;
		case CombatAbilityEffect.TargetType.singleFriendly:
			item.mayItemTargetItemFunc = (GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem;
			item.canItemTargetItemFunc = (GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem;
			break;
		case CombatAbilityEffect.TargetType.singleFriendlyExceptSelf:
			item.mayItemTargetItemFunc = (GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem && GraphUtils.FindParentType<GameCharacterItem>(s) != t;
			item.canItemTargetItemFunc = (GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem && GraphUtils.FindParentType<GameCharacterItem>(s) != t;
			break;
		case CombatAbilityEffect.TargetType.allFriendly:
			item.mayItemTargetItemFunc = (GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem;
			item.canItemTargetItemFunc = (GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem;
			break;
		case CombatAbilityEffect.TargetType.self:
			item.mayItemTargetItemFunc = (GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t;
			item.canItemTargetItemFunc = (GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t;
			break;
		case CombatAbilityEffect.TargetType.all:
			item.mayItemTargetItemFunc = (GameItem s, GameItem t) => t is GameCharacterItem;
			item.canItemTargetItemFunc = (GameItem s, GameItem t) => t is GameCharacterItem;
			break;
		}
		item.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			GameMaster.current.raidManager.CastAbility(s, t);
		};
		for (int i = 0; i < effectList.Length; i++)
		{
			item.effects.Add(DirectoryMaster.CombatAbilityEffect(effectList[i]));
		}
		item.bonusAccuracy = bonusAccuracy;
		return item;
	}

	private GameItem CreateStandardCombatAbilityItem(string spriteName, string[] effectList, int bonusAccuracy, string name, bool canUseOutsideCombat = true, bool triggerOverwatch = false)
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/abilityatlas", spriteName);
		InitCombatItem(gameItem, effectList, bonusAccuracy, name, canUseOutsideCombat, triggerOverwatch);
		gameItem.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => inv.IsTag("ability") ? true : false;
		gameItem.maxSlotAddItemFunc = delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			if (item.IsTag("character"))
			{
				string text = GraphUtils.FindParentType<GameCharacterItem>(inv)?.name;
				if (item.GetTagReadonly("character").valueString != text)
				{
					return 0;
				}
			}
			return 1;
		};
		gameItem.onSlotAddItemFunc = delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			string characterName = GraphUtils.FindParentType<GameCharacterItem>(inv)?.name;
			if (characterName != null)
			{
				item.ModifyTag("character", delegate(TagState state)
				{
					state.Enable().SetString(characterName);
				});
			}
		};
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			GameMaster.current.raidManager.CastAbility(s, t);
		};
		gameItem.EnableTag("ability");
		return gameItem;
	}

	[Obsolete("CreateGenericCombatAbilityItem is not supported, please use CreateStandardCombatAbilityItem()")]
	private GameItem CreateGenericCombatAbilityItem(string spriteName, string combatAbilityIdentifier)
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/abilityatlas", spriteName);
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		CombatAbility combatAbility = DirectoryMaster.CombatAbility(combatAbilityIdentifier);
		switch (combatAbility.GetPrimaryTarget())
		{
		case CombatAbilityEffect.TargetType.singleEnemy:
			gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
			gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
			break;
		case CombatAbilityEffect.TargetType.allEnemy:
			gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
			gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
			break;
		case CombatAbilityEffect.TargetType.singleFriendly:
			gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem;
			gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem;
			break;
		case CombatAbilityEffect.TargetType.singleFriendlyExceptSelf:
			gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem && GraphUtils.FindParentType<GameCharacterItem>(s) != t;
			gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem && GraphUtils.FindParentType<GameCharacterItem>(s) != t;
			break;
		case CombatAbilityEffect.TargetType.allFriendly:
			gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem;
			gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem;
			break;
		case CombatAbilityEffect.TargetType.self:
			gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t;
			gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t;
			break;
		case CombatAbilityEffect.TargetType.all:
			gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t is GameCharacterItem;
			gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t is GameCharacterItem;
			break;
		}
		gameItem.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => inv.IsTag("ability") ? true : false;
		gameItem.maxSlotAddItemFunc = delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			if (item.IsTag("character"))
			{
				string text = GraphUtils.FindParentType<GameCharacterItem>(inv)?.name;
				if (item.GetTagReadonly("character").valueString != text)
				{
					return 0;
				}
			}
			return 1;
		};
		gameItem.onSlotAddItemFunc = delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			string characterName = GraphUtils.FindParentType<GameCharacterItem>(inv)?.name;
			if (characterName != null)
			{
				item.ModifyTag("character", delegate(TagState state)
				{
					state.Enable().SetString(characterName);
				});
			}
		};
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			GameMaster.current.raidManager.CastAbility(s, t);
		};
		gameItem.EnableTag("ability");
		gameItem.ability = combatAbility;
		gameItem.name = gameItem.ability.name;
		gameItem.shortDescription = gameItem.ability.shortDescription;
		return gameItem;
	}
}

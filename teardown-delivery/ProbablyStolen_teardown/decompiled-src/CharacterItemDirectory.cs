using System;
using System.Linq;

public abstract class CharacterItemDirectory : Directory<GameCharacterItem>
{
	public enum Allegiance
	{
		Player,
		Friendly,
		Neutral,
		Hostile
	}

	public override GameCharacterItem Empty()
	{
		return CreateEmptyCharacterItem();
	}

	public static GameCharacterItem CreateEmptyCharacterItem(string identifier = null)
	{
		return new GameCharacterItem(identifier, "Items/itematlas", "unknown");
	}

	public static GameCharacterItem CreateEmptyCharacterItem<K>(K identifierValue) where K : Enum
	{
		return new GameCharacterItem(TagSystem.EnumToString(identifierValue), "Items/itematlas", "unknown")
		{
			identifierName = TagSystem.EnumToDisplayString(identifierValue)
		};
	}

	[Obsolete("Humanoid(isEnemy) is deprecated, please use Humanoid(Allegiance) instead.")]
	public static GameCharacterItem Humanoid(string name = "npc", bool isEnemy = false, Func<CombatEntity, CombatAI, bool, CombatEntity> combatEntityBuilder = null, CombatAI ai = null)
	{
		return Humanoid(name, isEnemy ? Allegiance.Hostile : Allegiance.Player, combatEntityBuilder, ai);
	}

	public static GameCharacterItem Humanoid(string name = "npc", Allegiance allegiance = Allegiance.Player, Func<CombatEntity, CombatAI, bool, CombatEntity> combatEntityBuilder = null, CombatAI ai = null)
	{
		GameCharacterItem gci = CreateEmptyCharacterItem();
		gci.SetCharacterSprite("Items/characteratlas", (allegiance == Allegiance.Hostile) ? "empty_character_red" : "empty_character_yellow");
		combatEntityBuilder?.Invoke(gci, ai, allegiance != Allegiance.Hostile);
		gci.ModifyTag("class_name", delegate(TagState state)
		{
			state.Enable().SetString(gci.name);
		});
		gci.name = name;
		if (allegiance == Allegiance.Hostile)
		{
			gci.EnableTag("enemy");
		}
		if (allegiance == Allegiance.Player)
		{
			gci.EnableTag("player");
		}
		if (allegiance == Allegiance.Friendly)
		{
			gci.EnableTag("friendly");
		}
		gci.onItemTargetSlotItemFunc = delegate(GameItem otherItem, GameItem thisItem, GameInventory thisInventory, SlotMarker thisSlot)
		{
			if (!GameMaster.current.InCombat() && otherItem.IsTag("butchering") && thisItem is GameCharacterItem { isDead: not false } gameCharacterItem2)
			{
				thisItem.TryUnloadAll();
				if (gameCharacterItem2.IsTag("SPECIES_HUMAN"))
				{
					GraphUtils.TryAcceptAll(thisInventory, DirectoryMaster.Item("raw_meat").SetAmount(2));
					GraphUtils.TryAcceptAll(thisInventory, DirectoryMaster.Item("small_raw_meat").SetAmount(4));
					GraphUtils.TryAcceptAll(thisInventory, DirectoryMaster.Item("fat_meat").SetAmount(1));
				}
				else if (gameCharacterItem2.IsTag("SPECIES_XENO"))
				{
					GraphUtils.TryAcceptAll(thisInventory, DirectoryMaster.Item("raw_xeno_meat").SetAmount(1));
					GraphUtils.TryAcceptAll(thisInventory, DirectoryMaster.Item("small_raw_xeno_meat").SetAmount(2));
				}
				thisItem.Destroy();
			}
		};
		gci.tooltipRichTextFunc = delegate(GameItem item, GameInventory _, SlotMarker _, GridShape _, TagSystem _)
		{
			RichTextBuilder richTextBuilder = new RichTextBuilder(40);
			if (item is GameCharacterItem gameCharacterItem)
			{
				richTextBuilder.AddLine(item.name + " (" + gameCharacterItem.GetTagReadonly("class_name").valueString + ")", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
				richTextBuilder.Add("Health: ", RenderHandler.ColorPalette.LightRed);
				richTextBuilder.AddProgressBar((float)gameCharacterItem.health / (float)gameCharacterItem.maxHealth, 10, RenderHandler.ColorPalette.LightRed, RenderHandler.ColorPalette.VeryDarkRed, null, " ");
				richTextBuilder.AddLine("(" + gameCharacterItem.health + "/" + (float)gameCharacterItem.maxHealth + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
				int num = 0;
				int num2 = 0;
				(num, num2) = gameCharacterItem.GetTotalCurrentArmorValue();
				if (num2 > 0)
				{
					richTextBuilder.Add(" Armor: ", RenderHandler.ColorPalette.LightCyan);
					richTextBuilder.AddProgressBar((float)num / (float)num2, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
					richTextBuilder.AddLine("(" + num + "/" + (float)num2 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
				}
				richTextBuilder.Add("Morale: ", RenderHandler.ColorPalette.LightPurple);
				richTextBuilder.AddProgressBar((float)gameCharacterItem.currentMorale / (float)gameCharacterItem.maxMorale, 10, RenderHandler.ColorPalette.LightPurple, RenderHandler.ColorPalette.DarkPurple, null, " ");
				richTextBuilder.AddLine("(" + gameCharacterItem.currentMorale + "%)", ignoreEmpty: true, RenderHandler.ColorPalette.LightPurple);
				richTextBuilder.Add("Thirst: ", RenderHandler.ColorPalette.LightTurquoise);
				richTextBuilder.AddProgressBar((float)gameCharacterItem.currentThirst / (float)gameCharacterItem.maxThirst, 10, RenderHandler.ColorPalette.LightTurquoise, RenderHandler.ColorPalette.DarkTurquoise, null, " ");
				richTextBuilder.AddLine("(" + gameCharacterItem.currentThirst / 10 + "%)", ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
				richTextBuilder.AddLine(" ");
				richTextBuilder.AddLine("Accuracy     : " + gameCharacterItem.accuracyPercentage + "%", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
				richTextBuilder.AddLine("Crital Chance: " + gameCharacterItem.criticalChanceDealing + "%", ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
				richTextBuilder.AddLine("Dodge        : " + gameCharacterItem.dodge + "%", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
				richTextBuilder.AddLine("Death Resist : " + (100 - gameCharacterItem.deathchance) + "%", ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
				richTextBuilder.AddLine("Priority     : " + gameCharacterItem.priority, ignoreEmpty: true, RenderHandler.ColorPalette.Yellow);
				richTextBuilder.AddLine("Reaction     : " + gameCharacterItem.reaction, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
				richTextBuilder.AddLine(" ");
				richTextBuilder.AddLine(gameCharacterItem.shortDescription);
				if (gameCharacterItem.abilityEffects.Any())
				{
					richTextBuilder.NextLine().AddLine("Status: ");
				}
				foreach (CombatAbilityEffect abilityEffect in gameCharacterItem.abilityEffects)
				{
					string text = abilityEffect.name;
					if (text == null || text.Length == 0)
					{
						text = ("unknown_" + abilityEffect.abilityEffectType).ToUpperInvariant();
					}
					if (!abilityEffect.isInvisible)
					{
						if (abilityEffect.isPermanent)
						{
							richTextBuilder.Add(text ?? "", RenderHandler.ColorPalette.None, ", ");
						}
						else
						{
							richTextBuilder.Add(text + " (" + abilityEffect.duration + ")", RenderHandler.ColorPalette.None, ", ");
						}
					}
				}
			}
			else
			{
				richTextBuilder.AddLine(item.name, ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
			}
			return richTextBuilder;
		};
		return gci;
	}
}

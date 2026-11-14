using System;
using System.Collections.Generic;
using System.Linq;

public class RaidManager
{
	public const int actionPointsPerTurn = 100;

	public int actionPointsRemaining;

	private readonly RaidScene raidScene;

	public Room currentRoom { get; set; }

	public Station station { get; set; }

	public GameCharacterRaidInventory playerParty { get; set; }

	public CombatMaster combatMaster { get; private set; }

	public RaidManager(RaidScene raidScene)
	{
		this.raidScene = raidScene;
		playerParty = new GameCharacterRaidInventory();
	}

	public void ClearStation()
	{
		if (station != null)
		{
			raidScene.Reset();
			currentRoom = null;
			station = null;
			combatMaster = null;
		}
	}

	public RaidManager InitStation(Station station)
	{
		ClearStation();
		playerParty = new GameCharacterRaidInventory();
		this.station = station;
		return this;
	}

	public RaidManager ResumeRaid()
	{
		if (station == null)
		{
			return this;
		}
		currentRoom = station.startRoom;
		currentRoom.ship = GameMaster.current.shipManager.ship;
		GraphUtils.FindChildrenType<GameCharacterItem>(GameMaster.current.shipManager.ship).ForEach(delegate(GameCharacterItem c)
		{
			playerParty.TryFindOneValidInventorySlot(c)?.TryAcceptOnce();
		});
		EnterRoom(station.startRoom);
		return this;
	}

	public RaidManager LeaveRaid()
	{
		EnterRoom(station?.startRoom);
		raidScene.Reset();
		currentRoom = null;
		combatMaster = null;
		return this;
	}

	public bool IsInCombat()
	{
		return combatMaster != null;
	}

	public List<GameCharacterItem> GetAllCharacters()
	{
		List<GameCharacterItem> allCharacters = new List<GameCharacterItem>();
		allCharacters.AddRange(playerParty.GetCharacterItems());
		foreach (Room item2 in station?.GetAllRooms() ?? new List<Room>())
		{
			if (item2.ship != null)
			{
				allCharacters.AddRange(GraphUtils.FindAllChildrenType<GameCharacterItem>(item2.ship));
			}
			if (item2.npcParty != null)
			{
				allCharacters.AddRange(item2.npcParty.GetCharacterItems());
			}
			item2.floorInventory.childItems.ForEach(delegate(GameItem i)
			{
				if (i is GameCharacterItem item)
				{
					allCharacters.Add(item);
				}
			});
		}
		return allCharacters;
	}

	public List<GameCharacterItem> GetAllPlayerCharacters()
	{
		return GetAllCharacters().FindAll((GameCharacterItem gci) => gci.IsTag("player"));
	}

	public Room FindItemRoom(GameItem item)
	{
		if (playerParty.childItems.Contains(item))
		{
			return currentRoom;
		}
		foreach (Room item2 in station?.GetAllRooms() ?? new List<Room>())
		{
			GameCharacterRaidInventory npcParty = item2.npcParty;
			if (npcParty != null && npcParty.childItems.Contains(item))
			{
				return item2;
			}
			if (item2.ship != null && GraphUtils.FindChildEquals(item2.ship, item))
			{
				return item2;
			}
			if (item2.floorInventory != null && GraphUtils.FindChildEquals(item2.floorInventory, item))
			{
				return item2;
			}
		}
		return null;
	}

	public List<GameItem> GetAllGameItem()
	{
		List<GameItem> allItems = new List<GameItem>();
		List<GameCharacterItem> allCharacters = GetAllCharacters();
		allItems.AddRange(allCharacters);
		allCharacters.ForEach(delegate(GameCharacterItem ci)
		{
			allItems.AddRange(GraphUtils.FindAllChildrenType<GameItem>(ci));
		});
		foreach (Room item in station?.GetAllRooms() ?? new List<Room>())
		{
			allItems.AddRange(GraphUtils.FindAllChildrenType<GameItem>(item.floorInventory));
			if (item.ship != null)
			{
				allItems.AddRange(GraphUtils.FindAllChildrenType<GameItem>(item.ship.contentWindow));
			}
		}
		return allItems.Distinct().ToList();
	}

	public List<GameItem> GetAllGameItemExcludeOnShip()
	{
		List<GameItem> allGameItem = GetAllGameItem();
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(GameMaster.current.shipManager.ship);
		list.Add(GameMaster.current.shipManager.ship);
		return allGameItem.Except(list).ToList();
	}

	public List<GameCharacterItem> GetAllCharactersNotHere()
	{
		List<GameCharacterItem> allCharacters = new List<GameCharacterItem>();
		foreach (Room item2 in station?.GetAllRooms() ?? new List<Room>())
		{
			if (item2 != currentRoom && item2.npcParty != null)
			{
				allCharacters.AddRange(item2.npcParty.GetCharacterItems());
			}
			item2.floorInventory.childItems.ForEach(delegate(GameItem i)
			{
				if (i is GameCharacterItem item)
				{
					allCharacters.Add(item);
				}
			});
		}
		return allCharacters;
	}

	public void EnterRoom(Room newRoom)
	{
		if (station != null)
		{
			if (newRoom != currentRoom && currentRoom != null)
			{
				currentRoom.ResetCompletedDialogue();
				OverlayHandler.current.HideBarter();
				WindowsHandler.HideAllMovable();
			}
			if (newRoom.type == Room.Type.NPC && newRoom.IsNPCHostile())
			{
				List<CombatEntity> combatEntities = playerParty.GetCombatEntities();
				List<CombatEntity> combatEntities2 = newRoom.npcParty.GetCombatEntities();
				combatMaster = new CombatMaster(combatEntities, combatEntities2);
			}
			else
			{
				combatMaster = null;
				GameMaster.current.turnManager.NextTurn();
			}
			if (newRoom.DoShowLoot())
			{
				raidScene.StartLoot(playerParty, newRoom.floorInventory, newRoom.displayName, newRoom.ship, newRoom.npcParty);
				playerParty.Unlock();
			}
			else if (newRoom.IsNPCDialogue())
			{
				ForceStartDialogue(newRoom.GetNextDialogue(), newRoom.npcParty);
			}
			else
			{
				raidScene.StartCombat(playerParty, newRoom.npcParty);
				playerParty.Lock();
				newRoom.npcParty.Lock();
				UpdateCombat();
			}
			currentRoom = newRoom;
		}
	}

	public void ForceStartDialogue(Dialogue dialogue, GameCharacterRaidInventory npcParty = null)
	{
		if (dialogue == null)
		{
			throw new Exception("Fatal exception, dialogue is null.");
		}
		if (npcParty == null && currentRoom != null)
		{
			npcParty = currentRoom.npcParty;
		}
		raidScene.StartDialogue(playerParty, npcParty);
		raidScene.UpdateDialogue(dialogue);
		playerParty?.Lock();
		npcParty?.Lock();
	}

	public bool CastAbility(GameItem abilityItemNode, GameItem targetItemNode)
	{
		bool flag = false;
		GameCharacterItem gameCharacterItem = GraphUtils.FindParentType<GameCharacterItem>(abilityItemNode);
		if (gameCharacterItem == null)
		{
			Debug.Log("? tried to use " + abilityItemNode?.GetDisplayName() + " on " + targetItemNode?.GetDisplayName() + "\nCaster is unknown! Currently UseAbility needs a valid caster and target");
		}
		if (gameCharacterItem != null && abilityItemNode != null)
		{
			if (targetItemNode != null)
			{
				if (targetItemNode is GameCharacterItem gameCharacterItem2)
				{
					if (combatMaster != null)
					{
						flag = combatMaster.SetNextAction(gameCharacterItem, abilityItemNode.ability, gameCharacterItem2);
					}
					else
					{
						List<CombatEntity> list = new List<CombatEntity>();
						list.Add(gameCharacterItem2);
						flag = CombatEntity.UseAbility(gameCharacterItem, abilityItemNode.ability, gameCharacterItem2, playerParty?.GetCombatEntities() ?? list, currentRoom?.npcParty?.GetCombatEntities() ?? list);
					}
				}
			}
		}
		if (flag)
		{
			UpdateCombat();
		}
		return flag;
	}

	private void UpdateCombat()
	{
		if (combatMaster == null)
		{
			return;
		}
		combatMaster.Next();
		playerParty.ValidateDeaths();
		currentRoom.npcParty?.ValidateDeaths();
		ActionQueue.current.Add(new EmptyAction().SetBeginFunc(delegate
		{
			playerParty.ValidateDeaths();
			currentRoom.npcParty?.ValidateDeaths();
			if (combatMaster.IsCombatDone())
			{
				if (playerParty.childItems.Count() > 0)
				{
					EnterRoom(currentRoom);
				}
				else
				{
					CheckValidParty();
				}
			}
		}));
		SyncCombatSelectCharacter();
	}

	public void CheckValidParty()
	{
		if (playerParty.childItems.Count() > 0)
		{
			return;
		}
		List<GameCharacterItem> source = GetAllPlayerCharacters().FindAll((GameCharacterItem gci) => !gci.isDead);
		if (source.Count() > 0)
		{
			Room room = FindItemRoom(source.First());
			if (room != null)
			{
				EnterRoom(room);
			}
			return;
		}
		OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder(50).AddLine("Everyone in your raid has perished.", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true).AddLine().AddLine("The end.")
			.Build(), null, delegate
		{
			OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
			{
				GameMaster.current.QuitToMenu();
			});
		}, canYes: true, showYes: false, "Confirm", "Recall");
	}

	public void SyncCombatSelectCharacter()
	{
		if (combatMaster != null)
		{
			raidScene.RestoreCharacterInventory();
			GameCharacterItem gameCharacterItem = playerParty.Find(combatMaster.WhichCurrentPlayer());
			if (gameCharacterItem != null)
			{
				raidScene.StealCharacterInventory(gameCharacterItem);
			}
		}
	}

	public void UseActionPoints(int actionPoints)
	{
		if (actionPoints > 0)
		{
			while (actionPoints >= actionPointsRemaining)
			{
				actionPoints -= actionPointsRemaining;
				GameMaster.current.turnManager.NextTurn();
			}
			actionPointsRemaining -= actionPoints;
		}
	}

	public void RecruitCharacter(GameCharacterItem character)
	{
		if (character != null)
		{
			character.parentInventory?.Expel(character);
			character.DisableTag("enemy");
			character.DisableTag("friendly");
			character.EnableTag("player");
			playerParty?.TryFindOneValidInventorySlot(character)?.TryAcceptOnce();
		}
	}
}

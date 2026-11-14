using System.Collections.Generic;
using System.Linq;

public class Room
{
	public enum Type
	{
		None,
		Ship,
		NPC
	}

	public enum Theme
	{
		Default,
		Service,
		Security,
		Military,
		Command,
		Engineering,
		Medical,
		Manufacturing,
		Research
	}

	public const string DOOR_TAG = "DOOR_TAG";

	public const string ROOM_ID_INT = "ROOM_ID_INT";

	public static int roomIdCounter = 1;

	public int roomId;

	public Type type;

	public GameItem ship;

	public GameCharacterRaidInventory npcParty;

	private List<Dialogue> dialoguesCompleted;

	public int visits;

	public int turns;

	private List<GameItem> lootToBeSpawn;

	public Theme theme;

	public string displayName { get; set; }

	public GameGridInventory floorInventory { get; private set; }

	public Dictionary<GameItem, Room> connections { get; private set; }

	public List<Dialogue> dialogues { get; }

	public Room(string displayName, Type type = Type.None)
	{
		this.type = type;
		this.displayName = displayName;
		floorInventory = new GameGridInventory(24, UISettings.current.floorLootGridHeight);
		connections = new Dictionary<GameItem, Room>();
		dialogues = new List<Dialogue>();
		dialoguesCompleted = new List<Dialogue>();
		theme = Theme.Default;
		roomId = roomIdCounter;
		roomIdCounter++;
		if (type == Type.NPC)
		{
			npcParty = new GameCharacterRaidInventory(flipped: true);
			npcParty.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => !item.IsTag("player");
		}
	}

	public Dialogue GetNextDialogue()
	{
		Dialogue dialogue = dialogues.Where((Dialogue d) => !d.IsDisabled() && !dialoguesCompleted.Contains(d)).FirstOrDefault();
		dialoguesCompleted.Add(dialogue);
		return dialogue;
	}

	public void ResetCompletedDialogue()
	{
		dialoguesCompleted.Clear();
	}

	public bool DoShowLoot()
	{
		if (IsNPCDialogue())
		{
			return false;
		}
		if (IsNPCHostile())
		{
			return false;
		}
		if (type == Type.None || type == Type.Ship)
		{
			return true;
		}
		(npcParty?.childItems?.Count()).GetValueOrDefault();
		return true;
	}

	public bool IsNPCHostile()
	{
		if (type == Type.NPC && (npcParty?.childItems?.Count()).GetValueOrDefault() > 0)
		{
			return (npcParty?.childItems?.Where((GameItem item) => item is GameCharacterItem gameCharacterItem && gameCharacterItem.IsTag("enemy")).Any()).GetValueOrDefault();
		}
		return false;
	}

	public bool IsNPCDialogue()
	{
		return dialogues.Where((Dialogue d) => !d.IsDisabled() && !dialoguesCompleted.Contains(d)).Any();
	}

	public bool AddNPC(GameCharacterItem npc)
	{
		return (npcParty?.TryFindOneValidInventorySlot(npc)?.TryAcceptOnce()).GetValueOrDefault() > 0;
	}

	public bool AddConnection(GameItem item, Room room)
	{
		item = item.CloneLinked();
		SlotMarker slotMarker = floorInventory.TryFindOneValidInventorySlot(item);
		if (slotMarker != null && !connections.ContainsKey(item))
		{
			slotMarker.TryAcceptOnce();
			connections.TryAdd(item, room);
			item.ModifyTag("ROOM_ID_INT", delegate(TagState state)
			{
				state.Enable().SetInt(room.roomId);
			});
			item.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
			item.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => GameMaster.current.raidManager.playerParty.childItems.Any();
			item.activateDefault = true;
			item.onActivateSlotItemFunc = delegate
			{
				OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
				{
					GameMaster.current.raidManager.EnterRoom(room);
				});
			};
			return true;
		}
		return false;
	}

	public bool AddWall(bool normalWallOnly = false)
	{
		GameItem gameItem = null;
		gameItem = ((!normalWallOnly) ? CreateRandomWall() : DirectoryMaster.Item("wall"));
		SlotMarker slotMarker = floorInventory.TryFindOneValidInventorySlot(gameItem);
		if (slotMarker != null && !connections.ContainsKey(gameItem))
		{
			slotMarker.TryAcceptOnce();
			return true;
		}
		return false;
	}

	public static GameItem CreateRandomWall()
	{
		if (RNG.GetRandomInt(0, 100) <= 5)
		{
			return DirectoryMaster.Item("wall_mold");
		}
		return DirectoryMaster.Item("wall");
	}

	public void AddLoot(List<GameItem> items)
	{
		foreach (GameItem item in items)
		{
			if (GraphUtils.TryAcceptAll(floorInventory, item) > 0)
			{
				Debug.Log(("Not all loot could be inserted in room", item.GetDisplayName(), displayName));
			}
		}
	}

	public void AttachLootToBeSpawn(List<GameItem> items)
	{
		lootToBeSpawn = items;
	}

	public void SpawnAttachedLoot()
	{
		if (lootToBeSpawn != null)
		{
			AddLoot(lootToBeSpawn);
		}
	}

	public GameItem GenerateDoorToCurrent()
	{
		GameItem gameItem = GameItem.Create("Items/connections", theme switch
		{
			Theme.Default => "simple_door_item_violet", 
			Theme.Command => "simple_door_item_dark_blue", 
			Theme.Engineering => "simple_door_item_orange", 
			Theme.Medical => "simple_door_item_light_green", 
			Theme.Military => "simple_door_item_black", 
			Theme.Security => "simple_door_item_red", 
			Theme.Service => "simple_door_item_green", 
			Theme.Manufacturing => "simple_door_item_light_red", 
			Theme.Research => "simple_door_item_white", 
			_ => "simple_door_item_violet", 
		});
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.name = "Door to " + displayName;
		gameItem.identifier = "door";
		AudioHelper.InitDoor(gameItem);
		gameItem.EnableTag("DOOR_TAG");
		return gameItem;
	}

	public void FullyConnectToRoom(Room targetRoom)
	{
		GameItem item = GenerateDoorToCurrent();
		GameItem item2 = targetRoom.GenerateDoorToCurrent();
		AddConnection(item2, targetRoom);
		targetRoom.AddConnection(item, this);
	}

	public List<GameCharacterItem> AddRaidersToRoom(List<GameCharacterItem> list)
	{
		foreach (GameCharacterItem item in list)
		{
			AddNPC(item);
		}
		return list;
	}
}

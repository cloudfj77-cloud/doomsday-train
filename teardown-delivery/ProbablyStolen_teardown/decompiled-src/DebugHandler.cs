using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugHandler : EmptyActionHandler
{
	public static DebugHandler current;

	[SerializeField]
	public KeyCode spawnKey;

	[SerializeField]
	public KeyCode quitToMenuKey;

	[SerializeField]
	public KeyCode killAllKey;

	[SerializeField]
	public KeyCode nextTurnKey;

	[SerializeField]
	public KeyCode copyIdentifierKey;

	[SerializeField]
	public KeyCode copyFlavorTextKey;

	[SerializeField]
	public KeyCode copyShortDescriptionKey;

	[SerializeField]
	public KeyCode unblockMouseKey;

	[SerializeField]
	public KeyCode multiplierKey;

	private bool isMultiplier;

	public override List<KeyCode> keyListeners => new KeyCode[7] { spawnKey, quitToMenuKey, killAllKey, nextTurnKey, copyIdentifierKey, multiplierKey, unblockMouseKey }.ToList();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	protected override bool OnEventInit()
	{
		return true;
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		if (!PlayerStore.Instance.devMode)
		{
			return;
		}
		if (keysPressed.Contains(multiplierKey))
		{
			isMultiplier = true;
		}
		if (keysPressed.Contains(spawnKey))
		{
			ToggleSpawnMenu();
		}
		if (keysPressed.Contains(quitToMenuKey))
		{
			GameMaster.current.QuitToMenu();
		}
		if (keysPressed.Contains(killAllKey))
		{
			if (!GameMaster.current.raidManager.IsInCombat())
			{
				return;
			}
			List<GameItem> list = GameMaster.current.raidManager.currentRoom?.npcParty?.childItems;
			if (list == null)
			{
				return;
			}
			foreach (GameItem item in list)
			{
				if (item is GameCharacterItem gameCharacterItem)
				{
					gameCharacterItem.KillCurrentEntity();
				}
			}
			GameMaster.current.raidManager.combatMaster.Next();
		}
		if (keysPressed.Contains(nextTurnKey) && GameMaster.current?.turnManager != null)
		{
			if (isMultiplier)
			{
				OverlayHandler.current.FadeBlock(0.1f, 0f, delegate
				{
					GameMaster.current.turnManager.NextNTurns(20);
				});
			}
			else
			{
				OverlayHandler.current.FadeBlock(0.1f, 0f, delegate
				{
					GameMaster.current.turnManager.NextTurn();
				});
			}
		}
		if (keysPressed.Contains(copyIdentifierKey))
		{
			GameItem gameItem = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
			if (gameItem != null && gameItem.identifier != null)
			{
				GUIUtility.systemCopyBuffer = gameItem.identifier;
			}
		}
		if (keysPressed.Contains(copyFlavorTextKey))
		{
			GameItem gameItem2 = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
			if (gameItem2 != null && gameItem2.identifier != null)
			{
				GUIUtility.systemCopyBuffer = gameItem2.identifier;
			}
		}
		if (keysPressed.Contains(copyShortDescriptionKey))
		{
			GameItem gameItem3 = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
			if (gameItem3 != null && gameItem3.shortDescription != null)
			{
				GUIUtility.systemCopyBuffer = gameItem3.shortDescription;
			}
		}
		if (keysPressed.Contains(unblockMouseKey))
		{
			OverlayHandler.current.ForceUnblockMouse();
		}
	}

	protected override void OnEventRelease(ISet<KeyCode> keysReleased)
	{
		if (PlayerStore.Instance.devMode && keysReleased.Contains(multiplierKey))
		{
			isMultiplier = false;
		}
	}

	public void ToggleSpawnMenu()
	{
		CustomUIItemSpawner.Toggle();
	}
}

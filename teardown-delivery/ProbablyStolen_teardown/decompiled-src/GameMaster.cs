using UnityEngine;

public class GameMaster : MonoBehaviour
{
	public int raidCounter;

	public bool isDebugMode;

	public TagSystem worldTags;

	public static GameMaster current { get; private set; }

	public MenuScene menuScene { get; private set; }

	public MenuManager menuManager { get; private set; }

	public RaidScene raidScene { get; private set; }

	public RaidManager raidManager { get; private set; }

	public ShipScene shipScene { get; private set; }

	public ShipManager shipManager { get; private set; }

	public TurnManager turnManager { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	private void Start()
	{
		shipScene = new ShipScene();
		shipManager = new ShipManager(shipScene);
		shipScene.SetShipWindowTransparency(0.2f);
		raidScene = new RaidScene();
		raidManager = new RaidManager(raidScene);
		menuScene = new MenuScene();
		menuManager = new MenuManager(menuScene);
		turnManager = new TurnManager(shipManager, raidManager);
		menuManager.StartMenu();
	}

	private void Update()
	{
		raidScene.Validate(force: false);
		shipScene.Validate(force: false);
		menuScene.Validate(force: false);
	}

	public void CloseAllScenes()
	{
		raidScene.Reset();
		raidScene.RestoreCharacterInventory();
		shipScene.Reset();
		menuScene.Reset();
		OverlayHandler.current.ClearLog();
	}

	public void NewGame()
	{
		CloseAllScenes();
		shipManager.StartShip(menuManager.selectedScenario.ship);
		shipManager.StartTransit(0, NavigationHelper.tutorialSpace);
		worldTags = new TagSystem();
		current.turnManager.NextTurn();
	}

	public void ContinueGame()
	{
		CloseAllScenes();
		SaveManager.Load();
		shipManager.StartShip(shipManager.ship);
		shipManager.StartTransit(shipManager.transitETA, shipManager.nextPOI);
	}

	public void QuitToMenu()
	{
		CloseAllScenes();
		menuManager.StartMenu();
	}

	public void QuitToDesktop()
	{
		Application.Quit();
	}

	public bool InRaid()
	{
		return false;
	}

	public bool InCombat()
	{
		if (!InRaid())
		{
			return false;
		}
		return raidManager.IsInCombat();
	}
}

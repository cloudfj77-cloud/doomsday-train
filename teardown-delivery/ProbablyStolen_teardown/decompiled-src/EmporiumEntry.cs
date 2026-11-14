using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EPOOutline;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.U2D;
using UnityEngine.UI;

public class EmporiumEntry : MonoBehaviour
{
	public static float sceneLoadStartTime = -1f;

	public PixelPerfectCamera pixelPerfectCamera;

	public Outliner outliner;

	public PixelWindow responseWindow;

	public GameGridInventory responseInventory;

	public PixelWindow responseWindowClosable;

	public GameGridInventory responseInventoryClosable;

	public List<(GameItem, Action)> choices;

	public PixelWindow afterhourWindow;

	public GameGridInventory afterhourInventory;

	public GameSlotInventory afterhourPocketSlotInvLeft;

	public GameSlotInventory afterhourPocketSlotInvBackpack;

	public GameSlotInventory afterhourPocketSlotInvRight;

	public GameGridInventory hirelingInv;

	public GameObject storeInteractables;

	public GameObject storePhysical;

	public float displayCaseX = 5.199f;

	public float displayCaseY = 27.9f;

	private const int COUNTER_WIDTH = 18;

	private const int COUNTER_HEIGHT = 4;

	public const int SHOWCASE_X = 7;

	public const int SHOWCASE_Y = 5;

	public const int FIXTURE_UPGRADE_X = 17;

	public const int FIXTURE_UPGRADE_Y = 10;

	public const int SHOWCASE_X_I = 8;

	public const int SHOWCASE_Y_I = 6;

	public const int SHOWCASE_X_II = 9;

	public const int SHOWCASE_Y_II = 7;

	private float oneSecondTimer;

	public int canvasWidth = -1;

	public int canvasHeight = -1;

	private bool leaveStoreSnapshotTaken;

	private readonly HashSet<GameObject> leaveStorePanels = new HashSet<GameObject>();

	private readonly List<(PixelWindow window, Vector2 position)> leaveStoreWindows = new List<(PixelWindow, Vector2)>();

	private const int FIXTURE_UID_DOSSIER = -101;

	private const int FIXTURE_UID_TRASHCAN = -102;

	private const int FIXTURE_UID_CASSETTE = -103;

	public float targetAspect = 1.7777778f;

	public static EmporiumEntry Instance { get; private set; }

	public PixelWindow invWindow { get; private set; }

	public GameGridInventory invElement { get; set; }

	public PixelWindow showcaseWindow { get; private set; }

	public GameGridInventory showcaseElement { get; private set; }

	public PixelWindow docInvWindow { get; private set; }

	public GameSlotInventory docInvElement { get; private set; }

	public PixelWindow trashInvWindow { get; private set; }

	public GameSlotInventory trashInvElement { get; private set; }

	public GameSlotInventory swapBufferElement { get; private set; }

	public PixelWindow drainInvWindow { get; private set; }

	public GameSlotInventory drainInvElement { get; private set; }

	public PixelWindow frontInvWindow { get; private set; }

	public GameGridInventory frontInvinvElement { get; private set; }

	public PixelWindow backInvWindow { get; private set; }

	public GameGridInventory backInvinvElement { get; private set; }

	public GameGridInventory backInvinvElementCounter { get; private set; }

	public PixelWindow bazarLeftInvWindow { get; private set; }

	public GameGridInventory bazarLeftinvElement { get; private set; }

	public PixelWindow hiddenWindow { get; private set; }

	public GameGridInventory hiddenElement { get; private set; }

	public PixelWindow faucetWindow { get; private set; }

	public GameSlotInventory faucetElement { get; private set; }

	public PixelWindow cassettePlayerWindow { get; private set; }

	public GameSlotInventory cassettePlayerElement { get; private set; }

	public PixelWindow vendingMachineWindow { get; private set; }

	public GameSlotInventory vendingMachineElement { get; private set; }

	public PixelWindow vendingFountainWindow { get; private set; }

	public GameSlotInventory vendingFountainElement { get; private set; }

	public GameGridInventory soldElement { get; private set; }

	public GameItem selectedBackpack { get; set; }

	public PixelWindow hirelingWindow { get; private set; }

	public GameItem dossier { get; set; }

	public GameItem trashcan { get; set; }

	public GameItem drain { get; set; }

	public GameItem faucet { get; set; }

	public GameItem cassettePlayer { get; set; }

	public GameGridInventory trashcanInvElement { get; set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		Instance = this;
		LetterboxManager.LayoutChanged += OnLetterboxLayoutChanged;
	}

	private void OnDestroy()
	{
		LetterboxManager.LayoutChanged -= OnLetterboxLayoutChanged;
	}

	private void OnLetterboxLayoutChanged()
	{
		if (!(Instance != this) && invWindow != null)
		{
			Validate();
		}
	}

	private void Start()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		AudioManager.Instance.PauseAllSource();
		Debug.Log("Starting EmporiumEntry...");
		TooltipContainer.isPaused = false;
		vendingFountainWindow = new PixelWindow();
		vendingFountainWindow.SetTransparency(1f);
		vendingFountainElement = new GameSlotInventory();
		vendingFountainElement.SetTransparency(1f);
		vendingFountainWindow.Attach(vendingFountainElement);
		vendingFountainWindow.Validate();
		vendingFountainWindow.handler.gameObject.name = "Node VendingFountainWindow";
		vendingMachineWindow = new PixelWindow();
		vendingMachineWindow.SetTransparency(1f);
		vendingMachineElement = new GameSlotInventory();
		vendingMachineElement.SetTransparency(1f);
		vendingMachineWindow.Attach(vendingMachineElement);
		vendingMachineWindow.Validate();
		vendingMachineWindow.handler.gameObject.name = "Node VendingMachineWindow";
		frontInvWindow = new PixelWindow();
		frontInvWindow.SetTransparency(0.99f);
		frontInvinvElement = new GameGridInventory(18, 4);
		frontInvinvElement.SetTransparency(0.99f);
		frontInvWindow.Attach(frontInvinvElement);
		frontInvWindow.Validate();
		frontInvWindow.Show();
		frontInvWindow.handler.gameObject.name = "Node FrontInvWindow";
		frontInvinvElement.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 8;
		showcaseWindow = new PixelWindow();
		showcaseWindow.SetTransparency(0.99f);
		showcaseElement = new GameGridInventory(7, 5);
		showcaseElement.SetTransparency(0.8f);
		showcaseWindow.Attach(showcaseElement);
		showcaseWindow.HideBorder();
		showcaseElement.HideBorder();
		showcaseWindow.Validate();
		showcaseWindow.Show();
		showcaseElement.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
		docInvWindow = new PixelWindow();
		docInvWindow.SetTransparency(1f);
		docInvElement = new GameSlotInventory();
		docInvElement.SetTransparency(1f);
		docInvWindow.Attach(docInvElement);
		docInvWindow.Validate();
		docInvWindow.Show();
		showcaseElement.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
		trashInvWindow = new PixelWindow();
		trashInvWindow.SetTransparency(1f);
		trashInvElement = new GameSlotInventory();
		trashInvElement.SetTransparency(1f);
		trashInvWindow.Attach(trashInvElement);
		trashInvWindow.Validate();
		trashInvWindow.Show();
		trashInvElement.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
		swapBufferElement = new GameSlotInventory();
		swapBufferElement.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
		swapBufferElement.handler.gameObject.name = "Node SwapBuffer";
		swapBufferElement.handler.gameObject.SetActive(value: false);
		drainInvWindow = new PixelWindow();
		drainInvWindow.SetTransparency(1f);
		drainInvElement = new GameSlotInventory();
		drainInvElement.SetTransparency(1f);
		drainInvWindow.Attach(drainInvElement);
		drainInvWindow.Validate();
		drainInvWindow.Show();
		drainInvElement.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
		invWindow = new PixelWindow();
		invElement = new GameGridInventory(24, 10);
		invWindow.Attach(invElement);
		invWindow.Validate();
		invWindow.Show();
		invElement.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
		backInvWindow = new PixelWindow();
		backInvWindow.SetTransparency(0.85f);
		backInvinvElement = new GameGridInventory(18, 4);
		backInvinvElement.SetTransparency(0.85f);
		backInvWindow.Attach(backInvinvElement);
		backInvWindow.HideBorder();
		backInvinvElement.HideBorder();
		backInvWindow.Validate();
		backInvWindow.Show();
		backInvinvElement.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
		backInvinvElementCounter = new GameGridInventory(18, 4);
		hiddenWindow = new PixelWindow();
		hiddenWindow.SetTransparency(0.99f);
		hiddenElement = new GameGridInventory(2, 2);
		hiddenElement.SetTransparency(0.8f);
		hiddenWindow.Attach(hiddenElement);
		hiddenWindow.HideBorder();
		hiddenElement.HideBorder();
		hiddenWindow.Validate();
		hiddenWindow.Show();
		hiddenElement.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
		hiddenWindow.rectTransform.gameObject.SetActive(value: false);
		faucetWindow = new PixelWindow();
		faucetWindow.SetTransparency(1f);
		faucetElement = new GameSlotInventory();
		faucetElement.SetTransparency(1f);
		faucetWindow.Attach(faucetElement);
		faucetWindow.Validate();
		faucetWindow.Show();
		faucetElement.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
		cassettePlayerWindow = new PixelWindow();
		cassettePlayerWindow.SetTransparency(1f);
		cassettePlayerElement = new GameSlotInventory();
		cassettePlayerElement.SetTransparency(1f);
		cassettePlayerWindow.Attach(cassettePlayerElement);
		cassettePlayerWindow.Validate();
		cassettePlayerWindow.Show();
		cassettePlayerElement.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
		hirelingWindow = new PixelWindow();
		hirelingWindow.SetTransparency(1f);
		hirelingInv = new GameGridInventory(10, 10);
		hirelingInv.SetTransparency(1f);
		hirelingWindow.Attach(hirelingInv);
		hirelingWindow.Validate();
		bazarLeftInvWindow = new PixelWindow();
		bazarLeftInvWindow.SetTransparency(0.5f);
		bazarLeftinvElement = new GameGridInventory(24, 4);
		bazarLeftinvElement.SetTransparency(0.8f);
		bazarLeftInvWindow.Attach(bazarLeftinvElement);
		bazarLeftInvWindow.Validate();
		bazarLeftinvElement.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
		SetupAfterhourInv();
		soldElement = new GameGridInventory(50, 50);
		DisableAllHandlers();
		EnableAllHandlers();
		GameGridInventory gameGridInventory = backInvinvElement;
		gameGridInventory.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameGridInventory.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			PlayerStore.Instance.AddedItemToWeightedArea(addedItem);
		});
		GameGridInventory gameGridInventory2 = backInvinvElement;
		gameGridInventory2.onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameGridInventory2.onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			PlayerStore.Instance.RemovedItemToWeightedArea(addedItem);
		});
		GameGridInventory gameGridInventory3 = invElement;
		gameGridInventory3.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameGridInventory3.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			PlayerStore.Instance.AddedItemToMainInvArea(addedItem);
		});
		GameGridInventory gameGridInventory4 = invElement;
		gameGridInventory4.onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameGridInventory4.onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			PlayerStore.Instance.RemovedItemToMainInvArea(addedItem);
		});
		GameGridInventory gameGridInventory5 = frontInvinvElement;
		gameGridInventory5.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameGridInventory5.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			PlayerStore.Instance.AddedItemToClientArea(addedItem);
		});
		GameGridInventory gameGridInventory6 = frontInvinvElement;
		gameGridInventory6.onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameGridInventory6.onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			PlayerStore.Instance.RemovedItemToClientArea(addedItem);
		});
		GameGridInventory gameGridInventory7 = showcaseElement;
		gameGridInventory7.onSlotAddItemLateFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameGridInventory7.onSlotAddItemLateFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			PlayerStore.Instance.AddedItemToDisplayCase(addedItem);
		});
		GameGridInventory gameGridInventory8 = showcaseElement;
		gameGridInventory8.onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameGridInventory8.onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			PlayerStore.Instance.RemovedItemToDisplayCase(addedItem);
		});
		GameGridInventory gameGridInventory9 = hiddenElement;
		gameGridInventory9.onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameGridInventory9.onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			if (!ContrabandHelper.IsSmugglerBay(addedItem))
			{
				addedItem.DisableTag("ITEM_HIDDEN_TAG");
			}
		});
		invElement.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => CanInsertIntoPlayerInventories(item);
		showcaseElement.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => CanInsertIntoPlayerInventories(item);
		hiddenElement.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => CanInsertIntoPlayerInventories(item);
		frontInvinvElement.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => (!GeneralHelper.IsHazardousWaste(item) || !MapUIManager.Instance.IsOutside) ? true : false;
		backInvinvElement.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => (!GeneralHelper.IsHazardousWaste(item) || !MapUIManager.Instance.IsOutside) ? true : false;
		InitResponseWindow();
		AddAlwayItems();
		PlayerStore preloadedStore = PlayerStore.PreLoadStore();
		LoadDisplayCaseUpgrade(preloadedStore);
		LoadFixtureUpgrades(preloadedStore);
		if (NewGameData.Instance != null && NewGameData.Instance.saveSlot != -1)
		{
			Debug.Log("Loaded save slot: " + NewGameData.Instance.saveSlot);
			PlayerStore.Instance.LoadGame();
			if (PlayerStore.Instance.nightLogs.Count > 0)
			{
				StartOfDayUIManager.Instance.ShowMorningReport();
			}
		}
		else
		{
			PlayerStore.Instance.StartNewGame();
			StartingPerkContent.HandleNewGamePerkBeforeItems();
			AddStartingItem();
			PlayerStore.Instance.HandleSkipIntro();
			StartingPerkContent.HandleNewGamePerk();
			PlayerStore.Instance.InitialSave();
		}
		LoadFaucetUpdate(PlayerStore.Instance);
		SetAspectRatio();
		HandleLocale();
		AudioManager.Instance.ResumeAllSource();
		frontInvWindow.Hide();
		EmporiumOption.Instance.ApplySetting(applyResolution: false);
		EmporiumOption.Instance.ApplyPixelPerfectCamera();
		frontInvWindow.handler.gameObject.transform.SetAsFirstSibling();
		vendingFountainWindow.handler.gameObject.transform.SetAsFirstSibling();
		vendingMachineWindow.handler.gameObject.transform.SetAsFirstSibling();
		StoreSoundPlayer.Instance.PlayStoreAmbient();
		Debug.Log($"[EmporiumEntry.Start] TOTAL load time: {stopwatch.ElapsedMilliseconds}ms");
		if (sceneLoadStartTime > 0f)
		{
			StartCoroutine(LogSceneTransitionOnFirstFrame());
		}
	}

	private IEnumerator LogSceneTransitionOnFirstFrame()
	{
		yield return new WaitForEndOfFrame();
		float num = (Time.realtimeSinceStartup - sceneLoadStartTime) * 1000f;
		Debug.Log($"[EmporiumEntry.Start] Scene transition (LoadScene call -> first frame rendered): {num:F0}ms");
		sceneLoadStartTime = -1f;
	}

	public void ResetFrontCounterGrid()
	{
		frontInvinvElement.SetShape(18, 4);
		frontInvinvElement.Validate();
	}

	public void ExpandFrontCounterGrid()
	{
		frontInvinvElement.SetShape(14, 21);
		frontInvinvElement.Validate();
	}

	private void ChangeLanguage(string localeCode)
	{
		StartCoroutine(LocHelper.SetLocale(localeCode));
	}

	public void LoadDisplayCaseUpgrade(PlayerStore preloadedStore)
	{
		if (preloadedStore != null && NewGameData.Instance != null && NewGameData.Instance.saveSlot != -1)
		{
			if (preloadedStore.networkUpgrade["SHOWCASE_II"].IsUnlocked())
			{
				UpgradeDisplayCaseII();
			}
			else if (preloadedStore.networkUpgrade["SHOWCASE_I"].IsUnlocked())
			{
				UpgradeDisplayCaseI();
			}
		}
	}

	public void LoadFixtureUpgrades(PlayerStore preloadedStore)
	{
		if (preloadedStore != null && preloadedStore.networkUpgrade != null && !(NewGameData.Instance == null) && NewGameData.Instance.saveSlot != -1)
		{
			if (preloadedStore.networkUpgrade.TryGetValue("DOSSIER_UPGRADE", out var value) && value.IsUnlocked())
			{
				UpgradeDossier();
			}
			if (preloadedStore.networkUpgrade.TryGetValue("TRASHCAN_UPGRADE", out var value2) && value2.IsUnlocked())
			{
				UpgradeTrashcan();
			}
		}
	}

	public void UpgradeDossier()
	{
		UpgradeFixtureGrid(dossier);
	}

	public void UpgradeTrashcan()
	{
		UpgradeFixtureGrid(trashcan);
	}

	private static void UpgradeFixtureGrid(GameItem fixture)
	{
		if (fixture != null && fixture.contentWindow != null && fixture.contentWindow.child is GameGridInventory gameGridInventory && (gameGridInventory.inventoryShape.width != 17 || gameGridInventory.inventoryShape.height != 10))
		{
			gameGridInventory.SetShape(17, 10);
			gameGridInventory.Validate();
			fixture.contentWindow.Validate();
		}
	}

	public void LoadFaucetUpdate(PlayerStore preloadedStore)
	{
		if (preloadedStore != null)
		{
			bool tapUnlocked = preloadedStore.networkUpgrade["RUSTWATER_TAP"].IsUnlocked() || preloadedStore.networkUpgrade["GUTTERFLOW_TAP"].IsUnlocked();
			ApplyFaucetState(tapUnlocked);
		}
	}

	public static bool IsWaterTapUnlocked()
	{
		if (!NetworkUpgrade.IsUnlocked("RUSTWATER_TAP"))
		{
			return NetworkUpgrade.IsUnlocked("GUTTERFLOW_TAP");
		}
		return true;
	}

	public void ApplyFaucetState(bool tapUnlocked)
	{
		faucet.SetSpriteAndShape("Items/items_furniture", tapUnlocked ? "faucet1" : "faucet");
		if (StoreUIManager.Instance != null && StoreUIManager.Instance.wallRenderer != null)
		{
			StoreUIManager.Instance.wallRenderer.sprite = (tapUnlocked ? StoreUIManager.Instance.wall1 : StoreUIManager.Instance.wall2);
		}
		if (StoreUIManager.Instance != null && StoreUIManager.Instance.wireRenderer != null)
		{
			StoreUIManager.Instance.wireRenderer.sprite = (tapUnlocked ? StoreUIManager.Instance.wire1 : StoreUIManager.Instance.wire2);
		}
		if (faucetWindow != null && faucetWindow.rectTransform != null)
		{
			if (tapUnlocked)
			{
				Vector2 anchoredPosition = faucetWindow.rectTransform.anchoredPosition;
				faucetWindow.Show();
				faucetWindow.rectTransform.anchoredPosition = anchoredPosition;
			}
			else
			{
				faucetWindow.Hide();
			}
		}
	}

	private void Update()
	{
		Validate(force: false);
		if (StoreUIManager.Instance.hoveringStoreUI)
		{
			DisableAllHandlersExceptDrag();
		}
		else if (DebugOwnershipToggleMode.Instance != null)
		{
			DisableAllHandlers();
		}
		else
		{
			EnableAllHandlers();
		}
		oneSecondTimer += Time.deltaTime;
		if (!(oneSecondTimer >= 1f))
		{
			return;
		}
		oneSecondTimer -= 1f;
		OneSecPrefixFunction();
		foreach (GameItem allItem in GetAllItems())
		{
			if (allItem != null && allItem.onOneSecondItemFunc != null)
			{
				allItem.onOneSecondItemFunc(allItem, allItem.parentInventory, null);
			}
		}
	}

	public void OneSecPrefixFunction()
	{
		PlayerStore.Instance.isGeneScannerScanning = false;
	}

	public void EnableAllHandlers()
	{
		ItemDefaultActivateHandler.current.Enable();
		ItemMouseDoubleClickHandler.current.Enable();
		ItemMouseDragHandler.current.Enable();
		ItemMouseHandler.current.Enable();
		ItemQuickTransferHandler.current.Enable();
		ItemQuickTransferAltHandler.current.Enable();
		ItemSelectHandler.current.Enable();
		ItemSelectHandler.current.auto = false;
		ItemTooltipHandler.current.Enable();
		ItemContextHandler.current.Enable();
	}

	public void DisableAllHandlers()
	{
		ItemDefaultActivateHandler.current.Disable();
		ItemMouseDoubleClickHandler.current.Disable();
		ItemMouseDragHandler.current.Disable();
		ItemMouseHandler.current.Disable();
		ItemQuickTransferHandler.current.Disable();
		ItemQuickTransferAltHandler.current.Disable();
		ItemSelectHandler.current.Disable();
		ItemTooltipHandler.current.Disable();
		ItemContextHandler.current.Disable();
	}

	public void DisableAllHandlersExceptDrag()
	{
		ItemDefaultActivateHandler.current.Disable();
		ItemMouseDoubleClickHandler.current.Disable();
		ItemMouseHandler.current.Disable();
		ItemQuickTransferHandler.current.Disable();
		ItemQuickTransferAltHandler.current.Disable();
		if (!ItemSelectHandler.current.IsEquipped)
		{
			ItemSelectHandler.current.Disable();
		}
		ItemTooltipHandler.current.Disable();
		ItemContextHandler.current.Disable();
	}

	public void Validate(bool force = true)
	{
		RectTransform component = RenderHandler.current.windowUICanvasObject.GetComponent<RectTransform>();
		int num = (int)component.sizeDelta.x;
		int num2 = (int)component.sizeDelta.y;
		if (num != canvasWidth || num2 != canvasHeight || force)
		{
			num = (int)component.sizeDelta.x;
			num2 = (int)component.sizeDelta.y;
			invWindow.rectTransform.anchoredPosition = new Vector2(num / 2 - invWindow.widthPixels / 2, -(num2 / 100 * 73));
			invWindow.Validate();
			backInvWindow.rectTransform.anchoredPosition = GetBackInvPosition(num, num2);
			backInvWindow.Validate();
			backInvinvElement.identifier = "backInv";
			frontInvWindow.rectTransform.anchoredPosition = GetFrontInvPosition(num, num2);
			frontInvWindow.Validate();
			showcaseWindow.rectTransform.anchoredPosition = new Vector2((float)num / 7.599f, 0f - (float)(num2 / 100) * 25.9f);
			showcaseWindow.Validate();
			docInvWindow.rectTransform.anchoredPosition = new Vector2((float)num / 1.2435f, -(num2 / 100 * 39));
			docInvWindow.Validate();
			hiddenWindow.rectTransform.anchoredPosition = new Vector2((float)num / 1.2195f, 0f - (float)(num2 / 100) * 17.5f);
			hiddenWindow.Validate();
			trashInvWindow.rectTransform.anchoredPosition = new Vector2((float)num / 5.2f, -(num2 / 100 * 69));
			trashInvWindow.Validate();
			faucetWindow.rectTransform.anchoredPosition = new Vector2((float)num / 12.4f, 0f - (float)(num2 / 100) * 52f);
			faucetWindow.Validate();
			cassettePlayerWindow.rectTransform.anchoredPosition = new Vector2((float)num / 1.0928f, 0f - (float)(num2 / 100) * 49f);
			cassettePlayerWindow.Validate();
			responseWindow.rectTransform.anchoredPosition = new Vector2(num / 2 - responseWindow.widthPixels / 2, 0f - (float)(num2 / 100) * 40f);
			responseWindow.Validate();
			responseWindowClosable.rectTransform.anchoredPosition = new Vector2(num / 2 - responseWindowClosable.widthPixels / 2, 0f - (float)(num2 / 100) * 40f);
			responseWindowClosable.Validate();
			afterhourWindow.Validate();
			afterhourWindow.rectTransform.anchoredPosition = new Vector2((float)num / 2.1f - (float)(afterhourWindow.widthPixels / 2), -(num2 - afterhourWindow.heightPixels));
			drainInvWindow.rectTransform.anchoredPosition = new Vector2(num / 100 * 7, -(num2 / 100 * 91));
			drainInvWindow.Validate();
			bazarLeftInvWindow.rectTransform.anchoredPosition = new Vector2(42f, -57f);
			bazarLeftInvWindow.Validate();
			vendingFountainWindow.rectTransform.anchoredPosition = new Vector2((float)(num / 100) * 36.9f, 0f - (float)(num2 / 100) * 14.9f);
			vendingFountainWindow.Validate();
			vendingMachineWindow.rectTransform.anchoredPosition = new Vector2((float)num / 100f * 58.6f, -(num2 / 100 * 15));
			vendingMachineWindow.Validate();
			UpdateScaleFactor();
			PixelWindow.ReclampDockedWindows();
			UIDragHandler.ReclampDockedPanels();
		}
		canvasWidth = num;
		canvasHeight = num2;
	}

	public void RestoreStoreUIAfterReturn()
	{
		if (!leaveStoreSnapshotTaken)
		{
			return;
		}
		leaveStoreSnapshotTaken = false;
		foreach (GameObject leaveStorePanel in leaveStorePanels)
		{
			if (!(leaveStorePanel == null) && !leaveStorePanel.activeSelf && (!(NegociationUIManager.Instance != null) || !(leaveStorePanel == NegociationUIManager.Instance.uiObject)) && (!(PhoneUIManager.Instance != null) || !(leaveStorePanel == PhoneUIManager.Instance.phonePanel)) && (!(EndOfDayUIManager.Instance != null) || !(leaveStorePanel == EndOfDayUIManager.Instance.UI)) && (!(StoreUIManager.Instance != null) || !(leaveStorePanel == StoreUIManager.Instance.backgroundBlocking)) && (!(StoreUIManager.Instance != null) || !(StoreUIManager.Instance.blackscreen != null) || !(leaveStorePanel == StoreUIManager.Instance.blackscreen.gameObject)) && (!(MapUIManager.Instance != null) || !MapUIManager.Instance.IsAfterhourPanel(leaveStorePanel)))
			{
				leaveStorePanel.SetActive(value: true);
			}
		}
		leaveStorePanels.Clear();
		foreach (var (pixelWindow, anchoredPosition) in leaveStoreWindows)
		{
			if (pixelWindow == null || pixelWindow.handler == null || pixelWindow.IsVisible())
			{
				continue;
			}
			bool flag = false;
			foreach (GameItem parentItem in pixelWindow.parentItems)
			{
				if (parentItem is GameItemElement gameItemElement && gameItemElement.IsDestroyed())
				{
					flag = true;
				}
			}
			if (!flag)
			{
				pixelWindow.Show();
				pixelWindow.rectTransform.anchoredPosition = anchoredPosition;
				pixelWindow.FixBounds();
			}
		}
		leaveStoreWindows.Clear();
	}

	public void OnLeaveStore()
	{
		bool recording = !leaveStoreSnapshotTaken;
		if (recording)
		{
			leaveStoreSnapshotTaken = true;
			leaveStorePanels.Clear();
			leaveStoreWindows.Clear();
			if (PaperUIManager.Instance != null && PaperUIManager.Instance.paperUIGroup != null)
			{
				foreach (Transform item in PaperUIManager.Instance.paperUIGroup.transform)
				{
					if (item.gameObject.activeSelf)
					{
						leaveStorePanels.Add(item.gameObject);
					}
				}
			}
			if (WindowsHandler.current != null && WindowsHandler.current.visibleWindows != null)
			{
				foreach (PixelWindow visibleWindow in WindowsHandler.current.visibleWindows)
				{
					if (visibleWindow != null && visibleWindow.canMove && visibleWindow.handler != null)
					{
						leaveStoreWindows.Add((visibleWindow, visibleWindow.rectTransform.anchoredPosition));
					}
				}
			}
		}
		ModHook.FireOnLeavingStoreEarly();
		if (TutorialUIManager.Instance != null)
		{
			TutorialUIManager.Instance.OnLeaveStore();
		}
		trashInvWindow.rectTransform.gameObject.SetActive(value: false);
		faucetWindow.rectTransform.gameObject.SetActive(value: false);
		drainInvWindow.rectTransform.gameObject.SetActive(value: false);
		cassettePlayerWindow.rectTransform.gameObject.SetActive(value: false);
		showcaseWindow.rectTransform.gameObject.SetActive(value: false);
		docInvWindow.rectTransform.gameObject.SetActive(value: false);
		frontInvWindow.rectTransform.gameObject.SetActive(value: false);
		backInvWindow.rectTransform.gameObject.SetActive(value: false);
		invWindow.rectTransform.gameObject.SetActive(value: false);
		vendingFountainWindow.rectTransform.gameObject.SetActive(value: false);
		vendingMachineWindow.rectTransform.gameObject.SetActive(value: false);
		hiddenWindow.rectTransform.gameObject.SetActive(value: false);
		if (PhoneUIManager.Instance != null && PhoneUIManager.Instance.phonePanel.activeSelf)
		{
			PhoneUIManager.Instance.CloseUI();
		}
		CloseIfOpen(ReputationUIManager.Instance?.uiPanel, delegate
		{
			ReputationUIManager.Instance.CloseUI();
		});
		CloseIfOpen(ClienteleUIManager.Instance?.panel, delegate
		{
			ClienteleUIManager.Instance.CloseUI();
		});
		CloseIfOpen(AdvCalendarUIManager.Instance?.UIPanel, delegate
		{
			AdvCalendarUIManager.Instance.CloseUI();
		});
		CloseIfOpen(PaperUIManager.Instance?.panel, delegate
		{
			PaperUIManager.Instance.CloseUI();
		});
		CloseIfOpen(HandnoteUIManager.Instance?.panel, delegate
		{
			HandnoteUIManager.Instance.ClosePanel();
		});
		CloseIfOpen(MultiPageUIManager.Instance?.panel, delegate
		{
			MultiPageUIManager.Instance.CloseUI();
		});
		CloseIfOpen(RecordUIManager.Instance?.UIPanel, delegate
		{
			RecordUIManager.Instance.CloseUI();
		});
		CloseIfOpen(LogUIManager.Instance?.UIPanel, delegate
		{
			LogUIManager.Instance.CloseUI();
		});
		CloseIfOpen(ChemScannerUIManager.Instance?.panel, delegate
		{
			ChemScannerUIManager.Instance.Close();
		});
		CloseIfOpen(WaterScannerUIManager.Instance?.panel, delegate
		{
			WaterScannerUIManager.Instance.Close();
		});
		CloseIfOpen(WaterStripUIManager.Instance?.panel, delegate
		{
			WaterStripUIManager.Instance.Close();
		});
		CloseIfOpen(ChemFinisherUIManager.Instance?.UIPanel, delegate
		{
			ChemFinisherUIManager.Instance.HideUI();
		});
		CloseIfOpen(PlantAnalyserUIManager.Instance?.UIPanel, delegate
		{
			PlantAnalyserUIManager.Instance.HideUI();
		});
		CloseIfOpen(RecorderUIManager.Instance?.UIPanel, delegate
		{
			RecorderUIManager.Instance.CloseUI();
		});
		CloseIfOpen(LabelerUIManager.Instance?.panel, delegate
		{
			LabelerUIManager.Instance.CloseUI();
		});
		CloseIfOpen(LabelerBlueUIManager.Instance?.panel, delegate
		{
			LabelerBlueUIManager.Instance.CloseUI();
		});
		CloseIfOpen(PCUIManager.Instance?.panel, delegate
		{
			PCUIManager.Instance.HideUI();
		});
		CloseIfOpen(ExchangeUIManager.Instance?.panel, delegate
		{
			ExchangeUIManager.Instance.ClosePanel();
		});
		CloseIfOpen(WildUIManager.Instance?.panel, delegate
		{
			WildUIManager.Instance.CloseUI();
		});
		if (WildNetworkUI.IsOpen)
		{
			WildNetworkUI.Close();
		}
		StoreUI[] array = UnityEngine.Object.FindObjectsOfType<StoreUI>();
		foreach (StoreUI storeUI in array)
		{
			SweepClose(storeUI.gameObject, UnderPlayerCanvas(storeUI.transform));
		}
		UIDragHandler[] array2 = UnityEngine.Object.FindObjectsOfType<UIDragHandler>();
		foreach (UIDragHandler uIDragHandler in array2)
		{
			Transform parent = uIDragHandler.transform.parent;
			if (parent != null && (parent.GetComponent<Canvas>() != null || parent == LetterboxManager.DockRoot))
			{
				SweepClose(uIDragHandler.gameObject, remember: true);
			}
		}
		foreach (GameItem allItem in GetAllItems())
		{
			if (allItem.contentWindow != null && allItem.contentWindow.IsVisible())
			{
				allItem.contentWindow.Hide();
			}
			if (allItem.examineWindow != null && allItem.examineWindow.IsVisible())
			{
				allItem.examineWindow.Hide();
			}
		}
		ClearFrontInv();
		storeInteractables.SetActive(value: false);
		storePhysical.SetActive(value: false);
		StoreUIManager.Instance.storeCalendar.dayTMP.gameObject.SetActive(value: false);
		StoreUIManager.Instance.storeCreditDisplay.creditDisplayText.gameObject.SetActive(value: false);
		StoreUIManager.Instance.clientCounter.SetActive(value: false);
		ModHook.FireOnLeavingStoreLate();
		void CloseIfOpen(GameObject panel, Action close)
		{
			if (panel != null && panel.activeSelf)
			{
				if (recording)
				{
					leaveStorePanels.Add(panel);
				}
				close();
			}
		}
		void SweepClose(GameObject panel, bool remember)
		{
			if (!(panel == null) && panel.activeSelf && (!(MapUIManager.Instance != null) || !MapUIManager.Instance.IsAfterhourPanel(panel)) && (!(StoreUIManager.Instance != null) || !(panel == StoreUIManager.Instance.darknessPanel)) && (!(StoreUIManager.Instance != null) || !(StoreUIManager.Instance.blackscreen != null) || !(panel == StoreUIManager.Instance.blackscreen.gameObject)) && (!(StoreUIManager.Instance != null) || !(StoreUIManager.Instance.escapeUIManager != null) || !(StoreUIManager.Instance.escapeUIManager.menuPanel != null) || !panel.transform.IsChildOf(StoreUIManager.Instance.escapeUIManager.menuPanel.transform)))
			{
				if (NegociationUIManager.Instance != null && panel == NegociationUIManager.Instance.uiObject)
				{
					NegociationUIManager.Instance.CloseUI();
				}
				else
				{
					if (recording && remember)
					{
						leaveStorePanels.Add(panel);
					}
					panel.SetActive(value: false);
				}
			}
		}
		static bool UnderPlayerCanvas(Transform t)
		{
			if (PaperUIManager.Instance != null && PaperUIManager.Instance.paperUIGroup != null && t.IsChildOf(PaperUIManager.Instance.paperUIGroup.transform))
			{
				return true;
			}
			if (LetterboxManager.DockRoot != null && t.IsChildOf(LetterboxManager.DockRoot))
			{
				return true;
			}
			return false;
		}
	}

	public void OnArriveStore(bool fromMap = false)
	{
		if (!fromMap)
		{
			PlayerStore.Instance.visitLeftTonight--;
		}
		ModHook.FireOnReturningStoreEarly();
		PlayerStore.Instance.playerLocation = PlayerStore.PlayerLocation.IN_STORE;
		if (TutorialUIManager.Instance != null)
		{
			TutorialUIManager.Instance.OnArriveStore();
		}
		trashInvWindow.rectTransform.gameObject.SetActive(value: true);
		faucetWindow.rectTransform.gameObject.SetActive(IsWaterTapUnlocked());
		drainInvWindow.rectTransform.gameObject.SetActive(value: true);
		cassettePlayerWindow.rectTransform.gameObject.SetActive(value: true);
		showcaseWindow.rectTransform.gameObject.SetActive(value: true);
		docInvWindow.rectTransform.gameObject.SetActive(value: true);
		backInvWindow.rectTransform.gameObject.SetActive(value: true);
		invWindow.rectTransform.gameObject.SetActive(value: true);
		StorePoster.LoadStorePoster();
		StoreUIManager.Instance.storeCalendar.dayTMP.gameObject.SetActive(value: true);
		StoreUIManager.Instance.storeCreditDisplay.creditDisplayText.gameObject.SetActive(value: true);
		StoreUIManager.Instance.clientCounter.SetActive(value: true);
		storeInteractables.SetActive(value: true);
		storePhysical.SetActive(value: true);
		ModHook.FireOnReturningStoreLate();
		Validate();
	}

	public void UpdateScaleFactor()
	{
		int gameAreaWidth = LetterboxManager.GameAreaWidth;
		int gameAreaHeight = LetterboxManager.GameAreaHeight;
		float num;
		if (gameAreaWidth == 960 && gameAreaHeight == 540)
		{
			num = 1f;
		}
		else if (gameAreaWidth == 1280 && gameAreaHeight == 720)
		{
			num = 1.335f;
		}
		else if (gameAreaWidth == 1366 && gameAreaHeight == 768)
		{
			num = 1.43f;
		}
		else if (gameAreaWidth == 1600 && gameAreaHeight == 900)
		{
			num = 1.66667f;
		}
		else if (gameAreaWidth == 1920 && gameAreaHeight == 1080)
		{
			num = 2f;
		}
		else if (gameAreaWidth == 2560 && gameAreaHeight == 1440)
		{
			num = 2.67f;
		}
		else if (gameAreaWidth == 3840 && gameAreaHeight == 2160)
		{
			num = 4f;
		}
		else
		{
			num = (float)gameAreaHeight / 540f;
			if (Mathf.Abs(num - Mathf.Round(num)) > 0.001f)
			{
				num *= 1.00125f;
			}
			if (num < 1f)
			{
				num = 1f;
			}
		}
		Debug.Log("UI scale for game area " + gameAreaWidth + "x" + gameAreaHeight + ": " + num);
		RenderHandler.current.windowUICanvas.scaleFactor = num;
		UIFamilyLayer.SyncScale();
		Canvas overlayCanvas = RenderHandler.current.OverlayCanvas;
		if (overlayCanvas != null)
		{
			overlayCanvas.scaleFactor = num;
			CanvasScaler component = overlayCanvas.GetComponent<CanvasScaler>();
			if (component != null)
			{
				component.scaleFactor = num;
			}
		}
	}

	public void ShowFrontWindow()
	{
		frontInvWindow.Show();
		frontInvWindow.rectTransform.anchoredPosition = new Vector2(backInvWindow.rectTransform.anchoredPosition.x, backInvWindow.rectTransform.anchoredPosition.y + (float)frontInvWindow.heightPixels);
	}

	public void HideFrontWindow()
	{
		frontInvWindow.Hide();
	}

	public void HideFrontArea()
	{
		Instance.HideFrontWindow();
	}

	public void ShowFrontArea()
	{
		Instance.ShowFrontWindow();
	}

	public void ClearFrontInv()
	{
		foreach (GameItem item in frontInvinvElement.childItems.ToList())
		{
			if (item.identifier.Equals("nuclear_waste"))
			{
				PlayerStore.GetStoreClientData().nuclearWasteLeftOnCounter++;
			}
			item.Destroy();
		}
	}

	public void ClearFrontInvNonOwned()
	{
		foreach (GameItem item in frontInvinvElement.childItems.ToList())
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				if (item.identifier.Equals("nuclear_waste"))
				{
					PlayerStore.GetStoreClientData().nuclearWasteLeftOnCounter++;
				}
				item.Destroy();
			}
		}
	}

	public void ClearBackInvNonOwned()
	{
		foreach (GameItem item in backInvinvElement.childItems.ToList())
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				if (item.identifier.Equals("nuclear_waste"))
				{
					PlayerStore.GetStoreClientData().nuclearWasteLeftOnCounter++;
				}
				item.Destroy();
			}
		}
	}

	public void ClearBackInv(bool all = true)
	{
		foreach (GameItem item in backInvinvElement.childItems.ToList())
		{
			if (all)
			{
				item.CloseContentWindow(includeChildren: true);
				item.Destroy();
			}
			else if (GeneralHelper.IsItemOwned(item))
			{
				item.CloseContentWindow(includeChildren: true);
				item.Destroy();
			}
		}
	}

	public List<GameItem> GetBackInvItems()
	{
		return backInvinvElement.childItems;
	}

	public List<GameItem> GetInvItems()
	{
		return invElement.childItems;
	}

	public List<GameItem> GetCountersItems()
	{
		return frontInvinvElement.childItems.Concat(backInvinvElement.childItems).ToList();
	}

	public void TransferOwnershipFrontInv()
	{
		foreach (GameItem childItem in frontInvinvElement.childItems)
		{
			GeneralHelper.SetItemOwned(childItem);
		}
	}

	public void TransferOwnershipBackInv()
	{
		foreach (GameItem childItem in backInvinvElement.childItems)
		{
			GeneralHelper.SetItemOwned(childItem);
		}
	}

	public void SwapInv()
	{
		foreach (GameItem item in frontInvinvElement.childItems.ToList())
		{
			GraphUtils.TryAcceptAll(backInvinvElement, item);
		}
	}

	public void AddStartingItem()
	{
		NewGameData.HandleInitialItem();
		if (PlayerStore.Instance.devMode)
		{
			GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("newspaper"));
		}
	}

	public void AddAlwayItems()
	{
		dossier = DirectoryMaster.Item("dossier");
		dossier.uniqueId = -101;
		GraphUtils.TryAcceptAll(docInvElement, dossier);
		trashcan = DirectoryMaster.Item("trashcan");
		trashcan.uniqueId = -102;
		GraphUtils.TryAcceptAll(trashInvElement, trashcan);
		drain = DirectoryMaster.Item("drain");
		GraphUtils.TryAcceptAll(drainInvElement, drain);
		faucet = DirectoryMaster.Item("faucet");
		GraphUtils.TryAcceptAll(faucetElement, faucet);
		cassettePlayer = DirectoryMaster.Item("cassette_player");
		cassettePlayer.uniqueId = -103;
		GraphUtils.TryAcceptAll(cassettePlayerElement, cassettePlayer);
	}

	public bool HaveItem(string identifier, bool ownedOnly = true)
	{
		foreach (GameItem allItem in GetAllItems())
		{
			if (allItem.identifier.Equals(identifier))
			{
				if (!ownedOnly)
				{
					return true;
				}
				if (GeneralHelper.IsItemOwned(allItem))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsBackInvContainItem(string identifier)
	{
		foreach (GameItem childItem in backInvinvElement.childItems)
		{
			if (childItem.identifier.Equals(identifier))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsBackInvContainItemTag(string tag)
	{
		foreach (GameItem childItem in backInvinvElement.childItems)
		{
			if (childItem.IsTag(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsBackInvContainItemTag(string tag, int count)
	{
		int num = 0;
		foreach (GameItem childItem in backInvinvElement.childItems)
		{
			if (childItem.IsTag(tag))
			{
				num++;
				if (num == count)
				{
					return true;
				}
			}
		}
		return false;
	}

	public List<GameItem> GetAllNonOwnedItem()
	{
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem childItem in backInvinvElement.childItems)
		{
			if (!GeneralHelper.IsItemOwned(childItem))
			{
				list.Add(childItem);
			}
		}
		foreach (GameItem childItem2 in frontInvinvElement.childItems)
		{
			if (!GeneralHelper.IsItemOwned(childItem2))
			{
				list.Add(childItem2);
			}
		}
		return list;
	}

	public List<GameItem> GetNonOwnedItemsBack()
	{
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem childItem in backInvinvElement.childItems)
		{
			if (!GeneralHelper.IsItemOwned(childItem))
			{
				list.Add(childItem);
			}
		}
		return list;
	}

	public List<GameItem> GetOwnedItemsBack()
	{
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem childItem in backInvinvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem))
			{
				list.Add(childItem);
			}
		}
		return list;
	}

	public List<GameItem> GetAllOwnedItems(bool isBurglary = false, bool includeAfterhour = true)
	{
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(invElement))
		{
			if (GeneralHelper.IsItemOwned(item))
			{
				list.Add(item);
			}
		}
		foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(backInvinvElement))
		{
			if (GeneralHelper.IsItemOwned(item2))
			{
				list.Add(item2);
			}
		}
		foreach (GameItem item3 in GraphUtils.FindAllChildrenType<GameItem>(backInvinvElementCounter))
		{
			if (GeneralHelper.IsItemOwned(item3))
			{
				list.Add(item3);
			}
		}
		foreach (GameItem item4 in GraphUtils.FindAllChildrenType<GameItem>(frontInvinvElement))
		{
			if (GeneralHelper.IsItemOwned(item4))
			{
				list.Add(item4);
			}
		}
		foreach (GameItem item5 in GraphUtils.FindAllChildrenType<GameItem>(docInvElement))
		{
			if (GeneralHelper.IsItemOwned(item5))
			{
				list.Add(item5);
			}
		}
		foreach (GameItem item6 in GraphUtils.FindAllChildrenType<GameItem>(showcaseElement))
		{
			if (GeneralHelper.IsItemOwned(item6))
			{
				list.Add(item6);
			}
		}
		foreach (GameItem item7 in GraphUtils.FindAllChildrenType<GameItem>(trashInvElement))
		{
			if (GeneralHelper.IsItemOwned(item7))
			{
				list.Add(item7);
			}
		}
		foreach (GameItem item8 in GraphUtils.FindAllChildrenType<GameItem>(vendingFountainElement))
		{
			if (GeneralHelper.IsItemOwned(item8))
			{
				list.Add(item8);
			}
		}
		foreach (GameItem item9 in GraphUtils.FindAllChildrenType<GameItem>(vendingMachineElement))
		{
			if (GeneralHelper.IsItemOwned(item9))
			{
				list.Add(item9);
			}
		}
		if (includeAfterhour)
		{
			foreach (GameItem item10 in GraphUtils.FindAllChildrenType<GameItem>(afterhourWindow))
			{
				if (GeneralHelper.IsItemOwned(item10))
				{
					list.Add(item10);
				}
			}
		}
		if (!isBurglary)
		{
			foreach (GameItem item11 in GraphUtils.FindAllChildrenType<GameItem>(hiddenElement))
			{
				if (GeneralHelper.IsItemOwned(item11))
				{
					list.Add(item11);
				}
			}
		}
		return list;
	}

	public List<GameItem> GetAllOwnedItemsByTag(string tag)
	{
		return (from item in GetAllOwnedItems()
			where item.IsTag(tag)
			select item).ToList();
	}

	public List<GameItem> GetAllOwnedItemsShallow(bool isBurglary = false)
	{
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem childItem in invElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem))
			{
				list.Add(childItem);
			}
		}
		foreach (GameItem childItem2 in backInvinvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem2))
			{
				list.Add(childItem2);
			}
		}
		foreach (GameItem childItem3 in backInvinvElementCounter.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem3))
			{
				list.Add(childItem3);
			}
		}
		foreach (GameItem childItem4 in frontInvinvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem4))
			{
				list.Add(childItem4);
			}
		}
		foreach (GameItem childItem5 in docInvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem5))
			{
				list.Add(childItem5);
			}
		}
		foreach (GameItem childItem6 in showcaseElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem6))
			{
				list.Add(childItem6);
			}
		}
		foreach (GameItem childItem7 in trashInvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem7))
			{
				list.Add(childItem7);
			}
		}
		if (!isBurglary)
		{
			foreach (GameItem childItem8 in hiddenElement.childItems)
			{
				if (GeneralHelper.IsItemOwned(childItem8))
				{
					list.Add(childItem8);
				}
			}
		}
		return list;
	}

	public List<GameItem> GetAllItems()
	{
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(invElement))
		{
			list.Add(item);
		}
		foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(backInvinvElement))
		{
			list.Add(item2);
		}
		foreach (GameItem item3 in GraphUtils.FindAllChildrenType<GameItem>(frontInvinvElement))
		{
			list.Add(item3);
		}
		foreach (GameItem item4 in GraphUtils.FindAllChildrenType<GameItem>(docInvElement))
		{
			list.Add(item4);
		}
		foreach (GameItem item5 in GraphUtils.FindAllChildrenType<GameItem>(showcaseElement))
		{
			list.Add(item5);
		}
		foreach (GameItem item6 in GraphUtils.FindAllChildrenType<GameItem>(trashInvElement))
		{
			list.Add(item6);
		}
		foreach (GameItem item7 in GraphUtils.FindAllChildrenType<GameItem>(hiddenElement))
		{
			list.Add(item7);
		}
		foreach (GameItem item8 in GraphUtils.FindAllChildrenType<GameItem>(afterhourWindow))
		{
			list.Add(item8);
		}
		foreach (GameItem item9 in GraphUtils.FindAllChildrenType<GameItem>(vendingFountainElement))
		{
			list.Add(item9);
		}
		foreach (GameItem item10 in GraphUtils.FindAllChildrenType<GameItem>(vendingMachineElement))
		{
			list.Add(item10);
		}
		return list;
	}

	public int GetAllNonOwendItemCount()
	{
		return GetAllNonOwnedItem().Count;
	}

	public void TransferOwnedItemBackToInv()
	{
		foreach (GameItem childItem in backInvinvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem))
			{
				GraphUtils.TryAcceptAll(invElement, childItem);
			}
		}
	}

	public void TransferOwnedItemFrontToInv()
	{
		foreach (GameItem childItem in frontInvinvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem))
			{
				GraphUtils.TryAcceptAll(invElement, childItem);
			}
		}
	}

	public void RemoveNonOwnedItem()
	{
		if (PlayerStore.Instance.currentClientInstance != null)
		{
			return;
		}
		Instance.DeselectDragItem(nonOwnedOnly: true);
		Debug.Log("Cleaned up non owned items");
		foreach (GameItem childItem in frontInvinvElement.childItems)
		{
			if (!GeneralHelper.IsItemOwned(childItem))
			{
				GunHelper.ReturnPlayerParts(childItem);
				childItem.Destroy();
			}
		}
		foreach (GameItem childItem2 in backInvinvElement.childItems)
		{
			if (!GeneralHelper.IsItemOwned(childItem2))
			{
				GunHelper.ReturnPlayerParts(childItem2);
				childItem2.Destroy();
			}
		}
	}

	public bool CanInsertIntoPlayerInventories(GameItem item)
	{
		return GeneralHelper.IsItemOwned(item);
	}

	public void SetAspectRatio()
	{
		LetterboxManager.Refresh();
	}

	public static void ResetData()
	{
		UnityEngine.Object.Destroy(Instance);
	}

	public void UpgradeDisplayCaseI()
	{
		showcaseElement.SetShape(8, 6);
		showcaseElement.Validate();
		showcaseWindow.Validate();
		StoreUIManager.Instance.OnDisplayCaseUpgradeI();
	}

	public void UpgradeDisplayCaseII()
	{
		showcaseElement.SetShape(9, 7);
		showcaseElement.Validate();
		displayCaseX = 5.63f;
		displayCaseY = 24f;
		Validate();
		StoreUIManager.Instance.OnDisplayCaseUpgradeII();
	}

	public bool IsFrontCounterEmpty()
	{
		if (frontInvinvElement.childItems.Count() == 0)
		{
			return true;
		}
		return false;
	}

	public bool IsFrontCounterEmptyNonOwned()
	{
		if (frontInvinvElement.childItems.Count() == 0)
		{
			return true;
		}
		foreach (GameItem childItem in frontInvinvElement.childItems)
		{
			if (!GeneralHelper.IsItemOwned(childItem))
			{
				return false;
			}
		}
		return true;
	}

	public void TryAddToPlayerInv(GameItem gameItem)
	{
		if (GraphUtils.TryAccept(backInvinvElement, gameItem) == -1 && GraphUtils.TryAccept(invElement, gameItem) == -1)
		{
			GraphUtils.TryAccept(showcaseElement, gameItem);
			_ = -1;
		}
	}

	public void TryAddToAfterhourInv(GameItem gameItem)
	{
		GraphUtils.TryAccept(afterhourInventory, gameItem);
		_ = -1;
	}

	public GameItem FindItemByUniqueID(int id)
	{
		return GetAllItems().FirstOrDefault((GameItem item) => item.uniqueId.Equals(id));
	}

	public void DeselectDragItem(bool nonOwnedOnly = false)
	{
		ItemSelectHandler.current.EndSelect(nonOwnedOnly);
		ItemMouseDragHandler.current.CustomCancel(nonOwnedOnly);
		ItemContextHandler.current.Stop(nonOwnedOnly);
		if (ItemMultiSelectHandler.current != null)
		{
			ItemMultiSelectHandler.current.CustomCancel(nonOwnedOnly);
		}
	}

	public void RemoveTemporaryClientFeatureFromItems()
	{
		foreach (GameItem allItem in GetAllItems())
		{
			allItem.RemoveTemporaryClientFeature();
			allItem.RemoveFeatureTemporarySelling();
		}
	}

	public void HandleLocale()
	{
		string text = LocHelper.ResolveStartupLocale();
		Debug.Log("Setting locale to: " + text);
		ChangeLanguage(text);
		PreLoadLocalizationTable();
	}

	public void PreLoadLocalizationTable()
	{
		LocalizationSettings.StringDatabase.GetTableAsync("Hint");
		LocalizationSettings.StringDatabase.GetTableAsync("UI");
	}

	public void SetBigInv()
	{
		invElement.SetShape(28, 10);
		invElement.Validate();
		invWindow.Validate();
	}

	public void SetSmallInv()
	{
		int width = (StartingPerk.IsAscetic() ? 16 : 20);
		invElement.SetShape(width, 10);
		invElement.Validate();
		invWindow.Validate();
	}

	public int GetItemCountById(string id)
	{
		int num = 0;
		foreach (GameItem allItem in GetAllItems())
		{
			if (allItem.identifier.Equals(id))
			{
				num++;
			}
		}
		return num;
	}

	public Vector2 GetFrontInvPosition(int newCanvasWidth, int newCanvasHeight)
	{
		Vector2? vector = ((MapUIManager.Instance != null) ? MapUIManager.Instance.Current : null)?.GetFrontCounterPosition(newCanvasWidth, newCanvasHeight);
		if (vector.HasValue)
		{
			return frontInvWindow.rectTransform.anchoredPosition = vector.Value;
		}
		return new Vector2(backInvWindow.rectTransform.anchoredPosition.x, backInvWindow.rectTransform.anchoredPosition.y + (float)frontInvWindow.heightPixels);
	}

	public Vector2 GetBackInvPosition(int newCanvasWidth, int newCanvasHeight)
	{
		Vector2? vector = ((MapUIManager.Instance != null) ? MapUIManager.Instance.Current : null)?.GetBackCounterPosition(newCanvasWidth, newCanvasHeight);
		if (vector.HasValue)
		{
			return vector.Value;
		}
		return new Vector2(newCanvasWidth / 2 - backInvWindow.widthPixels / 2, 0f - (float)(newCanvasHeight / 100) * 52.2f);
	}

	public GameItem GetItemFromSoldElement(int itemUniqueId)
	{
		foreach (GameItem childItem in soldElement.childItems)
		{
			if (childItem.uniqueId == itemUniqueId)
			{
				return childItem;
			}
		}
		Debug.LogWarning("Cannot find item in soldElement with uniqueId: " + itemUniqueId);
		return null;
	}

	public void ShowFrontCounters()
	{
		backInvinvElement.SetTransparency(0.75f);
		backInvWindow.SetTransparency(0.99f);
		showcaseElement.SetTransparency(0.75f);
		showcaseWindow.SetTransparency(0.99f);
	}

	public void HideFrontCounters()
	{
		backInvinvElement.SetTransparency(1f);
		backInvWindow.SetTransparency(1f);
		showcaseElement.SetTransparency(1f);
		showcaseWindow.SetTransparency(1f);
	}

	public void InitResponseWindow()
	{
		responseWindow = new PixelWindow(canMove: false, "Choices");
		responseInventory = new GameGridInventory(24, 12);
		responseWindow.Attach(responseInventory);
		responseInventory.onActivateSlotItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			FindChoiceAction(item)?.Invoke();
		};
		responseInventory.onSlotItemTargetItemFunc = delegate(GameItem item, GameInventory inv, SlotMarker slot, GameItem oitem)
		{
			item?.ActivateSlotItem();
		};
		responseWindow.handler.transform.SetAsLastSibling();
		responseWindowClosable = new PixelWindow(canMove: true, "Choices");
		responseInventoryClosable = new GameGridInventory(24, 12);
		responseWindowClosable.Attach(responseInventoryClosable);
		responseInventoryClosable.onActivateSlotItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			FindChoiceAction(item)?.Invoke();
		};
		responseInventoryClosable.onSlotItemTargetItemFunc = delegate(GameItem item, GameInventory inv, SlotMarker slot, GameItem oitem)
		{
			item?.ActivateSlotItem();
		};
		responseWindowClosable.handler.transform.SetAsLastSibling();
	}

	public Action FindChoiceAction(GameItem choiceItem)
	{
		foreach (var choice in choices)
		{
			if (choice.Item1 == choiceItem)
			{
				return choice.Item2;
			}
		}
		return null;
	}

	public void SetResponseWindow(bool closable, List<(GameItem, Action)> choices, string title = "Choices")
	{
		this.choices = choices;
		GameInventory currentInv = (closable ? responseInventoryClosable : responseInventory);
		PixelWindow pixelWindow = (closable ? responseWindowClosable : responseWindow);
		if (closable)
		{
			pixelWindow.closeCallback = delegate
			{
				HideResponseWindow();
			};
		}
		currentInv.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => true;
		pixelWindow.SetTitle(title);
		choices.ForEach(delegate((GameItem, Action) t)
		{
			GameItem item2 = t.Item1;
			Dialogue dialogue = null;
			if (dialogue == null || !dialogue.IsHidden())
			{
				if (GraphUtils.TryAcceptAll(currentInv, item2) > 0)
				{
					Debug.LogWarning("Warning, failed to add a choice '" + item2.GetDisplayName() + "' into dialogue choices inventory.");
				}
				if (item2 is GameItemElement gameItemElement)
				{
					if (dialogue != null && dialogue.IsDisabled())
					{
						gameItemElement.ToggleBackground(enabled: true, 0.5f, 0.5f);
						gameItemElement.forceDisableUse = true;
						gameItemElement.forceDisableActivate = true;
					}
					else
					{
						gameItemElement.ToggleBackground(enabled: true);
						gameItemElement.forceDisableUse = false;
						gameItemElement.forceDisableActivate = false;
					}
				}
			}
		});
		currentInv.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => false;
	}

	public void ShowTestResponseWindow()
	{
		StoreResponse response = ResponseList.CreateTestResponse();
		ShowResponseWindow(response);
	}

	public void ShowResponseWindow(StoreResponse response)
	{
		SetResponseWindow(response.closable, response.choices, response.title);
		PixelWindow obj = (response.closable ? responseWindowClosable : responseWindow);
		obj.Show();
		obj.Validate();
		Interactable.isPaused = true;
		TooltipContainer.isPaused = true;
	}

	private static void DestroyChoiceItems(GameInventory inventory)
	{
		foreach (GameItem item in inventory.childItems.ToList())
		{
			item.Destroy();
		}
	}

	public void HideResponseWindow()
	{
		responseWindow.Hide();
		DestroyChoiceItems(responseInventory);
		responseWindowClosable.Hide();
		DestroyChoiceItems(responseInventoryClosable);
		if (choices != null)
		{
			choices.Clear();
		}
		Interactable.isPaused = false;
		TooltipContainer.isPaused = false;
	}

	public static (GameItem, Action) PrepChoice(GameItem choiceItem, string name, bool isRealItem = false, string description = "", Func<bool> choiceEnabled = null, Func<bool> choiceShow = null, Action choiceAction = null, bool isBlackout = false, bool canActivate = true)
	{
		choiceItem.name = name;
		choiceItem.shortDescription = description;
		choiceItem.flavorText = "";
		OverlayHelper.TransformItemChoice(choiceItem, isRealItem);
		GeneralHelper.SetItemOwned(choiceItem);
		if (!isRealItem)
		{
			choiceItem.RemoveAllGameItemType();
		}
		if (isBlackout)
		{
			choiceItem.getItemSpriteModifierForItemFunc = (GameItem machine) => new List<ItemSpriteModifier>
			{
				new ItemSpriteModifier(RenderHandler.LoadFromAtlas(choiceItem.spriteAtlasPath, choiceItem.spritePath), new Vector2(0f, 0f), 0, isFlipped: false, 0, haveMask: false, MetaHelper.HexToColor("000000"))
			};
		}
		if (!canActivate)
		{
			choiceItem.canActivateSlotItemFunc = null;
			choiceItem.mayActivateSlotItemFunc = null;
		}
		return (choiceItem, choiceAction);
	}

	public void ShowAfterhourInv()
	{
		afterhourWindow.Show();
	}

	public void HideAfterhourInv()
	{
		afterhourWindow.Hide();
	}

	public void TransferAfterhourToInv(bool countsDumpedWaste = false)
	{
		if (afterhourPocketSlotInvBackpack.childItem != null)
		{
			TryAddToPlayerInv(afterhourPocketSlotInvBackpack.childItem);
		}
		if (afterhourPocketSlotInvRight.childItem != null)
		{
			TryAddToPlayerInv(afterhourPocketSlotInvRight.childItem);
		}
		if (afterhourPocketSlotInvLeft.childItem != null)
		{
			TryAddToPlayerInv(afterhourPocketSlotInvLeft.childItem);
		}
		foreach (GameItem item in afterhourInventory.childItems.ToList())
		{
			if (countsDumpedWaste && item.identifier.Equals("nuclear_waste"))
			{
				PlayerStore.GetStoreClientData().nuclearWasteDumped++;
			}
			afterhourInventory.Expel(item);
		}
	}

	public void TransferBackpackToInv()
	{
		if (selectedBackpack != null && selectedBackpack.parentInventory != null)
		{
			GraphUtils.TryAcceptAll(afterhourPocketSlotInvBackpack, selectedBackpack);
		}
		selectedBackpack = null;
	}

	public List<GameItem> GetAllAfterhourOwnedItems()
	{
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(afterhourInventory))
		{
			if (GeneralHelper.IsItemOwned(item))
			{
				list.Add(item);
			}
		}
		if (afterhourPocketSlotInvBackpack.childItem != null)
		{
			foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(afterhourPocketSlotInvBackpack))
			{
				if (GeneralHelper.IsItemOwned(item2))
				{
					list.Add(item2);
				}
			}
		}
		if (afterhourPocketSlotInvRight.childItem != null)
		{
			foreach (GameItem item3 in GraphUtils.FindAllChildrenType<GameItem>(afterhourPocketSlotInvRight))
			{
				if (GeneralHelper.IsItemOwned(item3))
				{
					list.Add(item3);
				}
			}
		}
		if (afterhourPocketSlotInvLeft.childItem != null)
		{
			foreach (GameItem item4 in GraphUtils.FindAllChildrenType<GameItem>(afterhourPocketSlotInvLeft))
			{
				if (GeneralHelper.IsItemOwned(item4))
				{
					list.Add(item4);
				}
			}
		}
		return list;
	}

	public bool IsAfterhourHaveOwnedItems(string identifier)
	{
		foreach (GameItem allAfterhourOwnedItem in GetAllAfterhourOwnedItems())
		{
			if (allAfterhourOwnedItem.identifier.Equals(identifier))
			{
				return true;
			}
		}
		return false;
	}

	public GameItem GetAfterhourItemById(string identifier)
	{
		foreach (GameItem allAfterhourOwnedItem in GetAllAfterhourOwnedItems())
		{
			if (allAfterhourOwnedItem.identifier.Equals(identifier))
			{
				return allAfterhourOwnedItem;
			}
		}
		return null;
	}

	public void SetupAfterhourInv()
	{
		afterhourWindow = new PixelWindow(canMove: false, LocHelper.GetLocalizedUI("ui_afterhour_ground_title"), WindowStyle.Machinery);
		afterhourInventory = new GameGridInventory(12, 9);
		afterhourPocketSlotInvLeft = new GameSlotInventory();
		afterhourPocketSlotInvLeft.SetTooltipName("[" + LocHelper.GetLocalizedItem("item_expedition_box_label_pocket1") + "]");
		afterhourPocketSlotInvLeft.SetBackgroundFadeSprite("Items/items_container", "pouch");
		afterhourPocketSlotInvBackpack = new GameSlotInventory(3, 3, stealUI: true);
		afterhourPocketSlotInvBackpack.SetBackgroundFadeSprite("Items/items_backpack2", "simple_backpack_medium");
		afterhourPocketSlotInvRight = new GameSlotInventory();
		afterhourPocketSlotInvRight.SetTooltipName("[" + LocHelper.GetLocalizedItem("item_expedition_box_label_pocket2") + "]");
		afterhourPocketSlotInvRight.SetBackgroundFadeSprite("Items/items_container", "pouch");
		ContainerHelper.AllowOnlyOwnedItems(afterhourInventory);
		ContainerHelper.AllowOnlyOwnedItems(afterhourPocketSlotInvLeft);
		ContainerHelper.AllowOnlyTaggedItem(afterhourPocketSlotInvBackpack, "BACKPACK_TAG", ownedOnly: true);
		ContainerHelper.AllowOnlyOwnedItems(afterhourPocketSlotInvRight);
		afterhourWindow.Attach(new MachineryPanelRow(MachineHelper.Panel(null, afterhourInventory, "ground"), MachineHelper.Panel(LocHelper.GetLocalizedItem("item_expedition_box_label_pocket1"), afterhourPocketSlotInvLeft, "pocket1"), MachineHelper.Panel(LocHelper.GetLocalizedItem("item_expedition_box_label_backpack"), afterhourPocketSlotInvBackpack, "backpack"), MachineHelper.Panel(LocHelper.GetLocalizedItem("item_expedition_box_label_pocket2"), afterhourPocketSlotInvRight, "pocket2")));
		afterhourInventory.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory inv)
		{
			if (GeneralHelper.IsHazardousWaste(item) && (MapUIManager.Instance.Current == null || !MapUIManager.Instance.Current.AllowsHazardousWaste))
			{
				return false;
			}
			return GeneralHelper.IsItemOwned(item) ? true : false;
		};
		afterhourWindow.Validate();
		afterhourInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 4;
	}

	public void CloseItemWindowUIWindow()
	{
		WindowsHandler.HideAllMovable();
	}
}

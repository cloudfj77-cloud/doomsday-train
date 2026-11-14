using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using QFSW.QC;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class StoreUIManager : MonoBehaviour
{
	public GameObject darknessPanel;

	public StoreGlass StoreGlass;

	public StoreClientMono clientGameObject;

	public GameObject clientGameObjectParent;

	public GenericClientMono secGameObject;

	public GenericClientMono secGameObject2;

	public Vector2 defaultClientPosition;

	public GameObject backgroundBlocking;

	public Image blackscreen;

	public GameObject tutorialGroup;

	public GameObject closeShutterConfirmationPanel;

	public Sprite spriteGroup;

	private int mouseLockFlag;

	public GameObject controlHintOverlay;

	public Sprite pinkBubble;

	public TypewriterByCharacter clientTypewriter;

	public TypewriterByCharacter playerTypewriter;

	public float dialogSpeed = 1f;

	public DialogBubbleList dialogBubbleList;

	public GameOptionUIManager gameOptionUIManager;

	public EscapeUIManager escapeUIManager;

	public Canvas primaryCanvas;

	public Canvas secondaryCanvas;

	public Canvas additionalCanvas;

	public bool hoveringStoreUI;

	public GameObject paperUICanvasObject;

	public SpriteRenderer showcaseRenderer;

	public Sprite showcaseMedium;

	public Sprite showcaseLarge;

	public GameObject debugPanel;

	public TextMeshProUGUI savingTMP;

	public StoreCreditDisplay storeCreditDisplay;

	public StoreCalendar storeCalendar;

	public GameObject clientCounter;

	public GameObject pcScreenObject;

	public TextMeshProUGUI versionTMP;

	public GameObject mainCamera;

	public GameObject shootingCamera;

	public bool isShootingMode;

	private int shootingModeSequence;

	public GameObject arm;

	public SpriteRenderer muzzleFlash;

	public GameObject inventoryCanvas;

	public GameObject worldInteractCanvas;

	public GameObject body;

	public bool isShot;

	public StoreClientInstance shotClientInstance;

	public GameObject shootingUI;

	public GameObject shotgun;

	private bool isTyping;

	public bool isReportedAug;

	public ConfirmPanelUIManager confirmPanelUIManager;

	public bool isAboutToStartMainDialog;

	public Sprite wall1;

	public Sprite wall2;

	public SpriteRenderer wallRenderer;

	public Sprite wire1;

	public Sprite wire2;

	public SpriteRenderer wireRenderer;

	public SpriteRenderer shootingBodyRenderer;

	public SpriteRenderer shootingWoundRenderer;

	public List<Sprite> shootingWoundSprites = new List<Sprite>();

	public List<Sprite> shootingBodySprites = new List<Sprite>();

	[Header("Shooting animation timing")]
	public float woundFrameDuration = 0.06f;

	public float bodyFrameDuration = 0.06f;

	public Color shotDarkenColor = new Color(19f / 85f, 0.18039216f, 0.23529412f, 1f);

	public GameObject magnifyingScreen;

	public GameObject magnifyingScreenCollapsed;

	public bool isMagnifyingMode;

	public float magnifyZoomFactor = 2f;

	public float magnifyZoomCameraY = 1.6f;

	[Header("Magnifying mode look/feel")]
	public float magnifyPanFactor = 0.08f;

	public float magnifyPanSmoothing = 8f;

	public float magnifyBreathAmplitudeX = 0.02f;

	public float magnifyBreathAmplitudeY = 0.035f;

	public float magnifyBreathFrequency = 0.22f;

	private float magnifyBaseOrthoSize;

	private float magnifyBaseCameraX;

	private float magnifyBaseCameraY;

	private float magnifyBaseCameraZ;

	private Vector3 magnifyPanCurrent = Vector3.zero;

	private bool isMagnifyZoomedIn;

	public GameObject zoomUI;

	private int lastEscapeResolvedFrame = -1;

	private Action pendingSurvivedShot;

	public const int SURVIVED_SHOT_WOUND_FRAMES = 3;

	public const float SURVIVED_SHOT_HOLD = 0.6f;

	public static StoreUIManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		Instance = this;
		ItemPinHelper.ExitPinMode();
	}

	private void Start()
	{
		defaultClientPosition = clientGameObjectParent.transform.localPosition;
		SetTypewriterSpeed();
		tutorialGroup.SetActive(PlayerStore.Instance.tutorialEnable);
		versionTMP.text = "Game: " + PlayerStore.VERSION + "\nSave: " + PlayerStore.Instance.saveVersion;
		foreach (Transform item in paperUICanvasObject.transform)
		{
			if (item.GetComponent<PaperPanelFocusStamp>() == null)
			{
				item.gameObject.AddComponent<PaperPanelFocusStamp>();
			}
		}
	}

	private void Update()
	{
		if (isMagnifyingMode)
		{
			UpdateMagnifyCamera();
		}
		if (gameOptionUIManager != null)
		{
			gameOptionUIManager.PollKeyCapture();
		}
		if (KeyBindHelper.GetKeyDown("control_hints") && !IsTyping())
		{
			ToggleControlHintOverlay();
		}
		if (KeyBindHelper.GetKeyDown("trash_item") && !IsTyping())
		{
			TryTrashHoveredItem();
		}
		if (!IsTyping())
		{
			ToolboxHelper.HandleHotkeys();
			MachineStorageCoordinator.HandleInput();
			HopperHelper.HandleInput();
			ItemPinHelper.HandleInput();
		}
		bool flag = GameOptionUIManager.ConsumedEscapeThisFrame() || ItemPinHelper.ConsumedEscapeThisFrame() || MachineStorageCoordinator.ConsumedEscapeThisFrame() || HopperHelper.ConsumedEscapeThisFrame();
		if (KeyBindHelper.GetBackDown() && !flag)
		{
			ResolveEscape();
		}
		if (KeyBindHelper.GetKeyDown("advance_dialogue"))
		{
			if (TutorialUIManager.Instance != null)
			{
				TutorialUIManager.Instance.DismissAdvanceDialogHint();
			}
			DialogUIManager.Instance.OnAdvancePressed();
			DialoguePhoneUIManager.Instance.OnAdvancePressed();
		}
		if (Input.GetKeyDown("d") && !PlayerStore.Instance.isProduction && !IsTyping())
		{
			PlayerStore.Instance.devMode = !PlayerStore.Instance.devMode;
			Debug.Log("Dev Mode " + PlayerStore.Instance.devMode);
		}
		if (PlayerStore.Instance.devMode && !IsTyping())
		{
			if (Input.GetKeyDown("g"))
			{
				WildNetworkUI.Open(debug: true);
			}
			if (Input.GetKeyDown("h"))
			{
				EmporiumEntry.Instance.DeselectDragItem();
			}
		}
		if (Input.GetKeyDown("f9") && DevTools.IsEnabled && !NewGameData.Instance.isShowcase)
		{
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				CustomUIDebugPanel.ToggleLegacyPanel();
			}
			else
			{
				CustomUIDebugPanel.Toggle();
			}
		}
		CheckRaycast();
	}

	private void TryTrashHoveredItem()
	{
		if (PlayerStore.Instance.playerLocation != 0 || (MapUIManager.Instance != null && MapUIManager.Instance.isOutside) || EmporiumEntry.Instance == null || EmporiumEntry.Instance.trashcan == null || (ItemMouseDragHandler.current != null && ItemMouseDragHandler.current.IsDraggingItem) || UIDragHandler.isDraggingAnyUI)
		{
			return;
		}
		GameItem gameItem = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
		if (gameItem != null && GeneralHelper.IsItemOwned(gameItem) && gameItem != EmporiumEntry.Instance.trashcan && gameItem.MayRemove() && gameItem.MaxNumRemove() >= 1 && EmporiumEntry.Instance.trashcan.contentWindow?.child is GameInventory gameInventory && gameItem.parentInventory != gameInventory)
		{
			int unitCount = gameItem.unitCount;
			if (GraphUtils.TryAcceptAll(gameInventory, gameItem) < unitCount)
			{
				AudioManager.Instance.TryPlayOne(gameItem.soundDragEnd, gameItem.soundDragStart);
			}
		}
	}

	public void CheckRaycast()
	{
		Vector2 position = Input.mousePosition;
		PointerEventData pointerData = new PointerEventData(EventSystem.current)
		{
			position = position
		};
		int num = DockCanvasVerdict(pointerData);
		if (num != 0)
		{
			hoveringStoreUI = num > 0;
		}
		else if (RaisedFloatingWindowHit(pointerData))
		{
			hoveringStoreUI = false;
		}
		else if (PerformRaycastOnCanvas(primaryCanvas, pointerData))
		{
			hoveringStoreUI = true;
		}
		else if (PerformRaycastOnCanvas(secondaryCanvas, pointerData))
		{
			hoveringStoreUI = true;
		}
		else if (PerformRaycastOnCanvas(additionalCanvas, pointerData))
		{
			hoveringStoreUI = true;
		}
		else
		{
			hoveringStoreUI = false;
		}
	}

	private int DockCanvasVerdict(PointerEventData pointerData)
	{
		Canvas dockCanvas = LetterboxManager.DockCanvas;
		if (dockCanvas == null)
		{
			return 0;
		}
		GraphicRaycaster component = dockCanvas.GetComponent<GraphicRaycaster>();
		if (component == null)
		{
			return 0;
		}
		List<RaycastResult> list = new List<RaycastResult>();
		component.Raycast(pointerData, list);
		foreach (RaycastResult item in list)
		{
			if (item.gameObject.GetComponentInParent<StoreUI>() != null)
			{
				return 1;
			}
			if (item.gameObject.GetComponent<TreeNodeRender>() != null)
			{
				return -1;
			}
		}
		return 0;
	}

	private bool RaisedFloatingWindowHit(PointerEventData pointerData)
	{
		Canvas raisedCanvas = UIFamilyLayer.RaisedCanvas;
		if (raisedCanvas == null || !raisedCanvas.gameObject.activeInHierarchy)
		{
			return false;
		}
		GraphicRaycaster component = raisedCanvas.GetComponent<GraphicRaycaster>();
		if (component == null)
		{
			return false;
		}
		List<RaycastResult> list = new List<RaycastResult>();
		component.Raycast(pointerData, list);
		foreach (RaycastResult item in list)
		{
			if (item.gameObject.GetComponent<TreeNodeRender>() != null)
			{
				return true;
			}
		}
		return false;
	}

	public bool PerformRaycastOnCanvas(Canvas canvas, PointerEventData pointerData)
	{
		GraphicRaycaster component = canvas.GetComponent<GraphicRaycaster>();
		if (component == null)
		{
			Debug.LogError("No GraphicRaycaster found on the Canvas.");
			return false;
		}
		List<RaycastResult> list = new List<RaycastResult>();
		component.Raycast(pointerData, list);
		foreach (RaycastResult item in list)
		{
			if (item.gameObject.GetComponentInParent<StoreUI>() != null)
			{
				return true;
			}
		}
		return false;
	}

	public bool ResolveEscape()
	{
		if (lastEscapeResolvedFrame == Time.frameCount)
		{
			return true;
		}
		lastEscapeResolvedFrame = Time.frameCount;
		if (RunSummaryUI.IsOpen)
		{
			return true;
		}
		if (gameOptionUIManager.IsOpen)
		{
			gameOptionUIManager.CloseUI();
			return true;
		}
		if (EndOfDayUIManager.Instance.IsOpen)
		{
			EndOfDayUIManager.Instance.CloseUI();
			return true;
		}
		if (escapeUIManager.menuPanel.activeSelf)
		{
			escapeUIManager.CloseEscapeMenu();
			return true;
		}
		if (WildNetworkUI.IsOpen)
		{
			WildNetworkUI.Close();
			return true;
		}
		if (TransitUI.IsOpen && MapUIManager.Instance != null)
		{
			bool flag = blackscreen != null && blackscreen.gameObject.activeSelf;
			if (!MapUIManager.Instance.IsInTransit && !flag)
			{
				MapUIManager.Instance.CloseUI();
			}
			return true;
		}
		if (OverlayHandler.current != null && OverlayHandler.current.Cancel())
		{
			return true;
		}
		if (ItemSelectHandler.current != null && ItemSelectHandler.current.Cancel())
		{
			return true;
		}
		if (ItemMultiSelectHandler.current != null && ItemMultiSelectHandler.current.Cancel())
		{
			return true;
		}
		if (ItemMouseDragHandler.current != null && ItemMouseDragHandler.current.Cancel())
		{
			return true;
		}
		if (CloseTopmostEscapeTarget())
		{
			return true;
		}
		escapeUIManager.OpenEscapeMenu();
		return true;
	}

	private bool CloseTopmostEscapeTarget()
	{
		PixelWindow pixelWindow = null;
		foreach (PixelWindow visibleWindow in WindowsHandler.current.visibleWindows)
		{
			if (visibleWindow.canMove && (pixelWindow == null || visibleWindow.focusStamp >= pixelWindow.focusStamp))
			{
				pixelWindow = visibleWindow;
			}
		}
		GameObject gameObject = null;
		long num = -1L;
		foreach (Transform item in paperUICanvasObject.transform)
		{
			if (item.gameObject.activeSelf)
			{
				long stamp = PaperPanelFocusStamp.GetStamp(item.gameObject);
				if (gameObject == null || stamp >= num)
				{
					gameObject = item.gameObject;
					num = stamp;
				}
			}
		}
		foreach (GameObject activeDockedPanel in UIDragHandler.GetActiveDockedPanels(paperUICanvasObject.transform))
		{
			long stamp2 = PaperPanelFocusStamp.GetStamp(activeDockedPanel);
			if (gameObject == null || stamp2 >= num)
			{
				gameObject = activeDockedPanel;
				num = stamp2;
			}
		}
		if (pixelWindow == null && gameObject == null)
		{
			return false;
		}
		if (gameObject == null || (pixelWindow != null && pixelWindow.focusStamp > num))
		{
			WindowsHandler.CloseWindow(pixelWindow);
			return true;
		}
		if (gameObject == NegociationUIManager.Instance.uiObject)
		{
			NegociationUIManager.Instance.CloseUI();
		}
		else if (PhoneUIManager.Instance != null && gameObject == PhoneUIManager.Instance.phonePanel)
		{
			PhoneUIManager.Instance.CloseUI();
		}
		else
		{
			gameObject.SetActive(value: false);
		}
		return true;
	}

	public bool IsInnateWindow(PixelWindow window)
	{
		if (window != EmporiumEntry.Instance.invWindow && window != EmporiumEntry.Instance.docInvWindow && window != EmporiumEntry.Instance.faucetWindow && window != EmporiumEntry.Instance.cassettePlayerWindow && window != EmporiumEntry.Instance.hiddenWindow && window != EmporiumEntry.Instance.backInvWindow && window != EmporiumEntry.Instance.drainInvWindow && window != EmporiumEntry.Instance.frontInvWindow && window != EmporiumEntry.Instance.showcaseWindow && window != EmporiumEntry.Instance.trashInvWindow && window != EmporiumEntry.Instance.responseWindow && window != EmporiumEntry.Instance.responseWindowClosable && window != EmporiumEntry.Instance.afterhourWindow && window != EmporiumEntry.Instance.hirelingWindow && window != EmporiumEntry.Instance.vendingMachineWindow && window != EmporiumEntry.Instance.vendingFountainWindow)
		{
			return window == OverlayHandler.current.GetTooltipWindow();
		}
		return true;
	}

	public int GetNonInnateWindowCount()
	{
		int num = 0;
		foreach (PixelWindow visibleWindow in WindowsHandler.current.visibleWindows)
		{
			if (!IsInnateWindow(visibleWindow))
			{
				num++;
			}
		}
		return num;
	}

	public void SetTypewriterSpeed()
	{
		float typewriterSpeed = dialogSpeed * GetLocaleSpeedMultiplier();
		clientTypewriter.SetTypewriterSpeed(typewriterSpeed);
		playerTypewriter.SetTypewriterSpeed(typewriterSpeed);
	}

	private static float GetLocaleSpeedMultiplier()
	{
		switch ((LocalizationSettings.SelectedLocale != null) ? LocalizationSettings.SelectedLocale.Identifier.Code : "en")
		{
		case "ko":
		case "ja":
		case "zh":
		case "zh-Hans":
			return 0.35f;
		default:
			return 1f;
		}
	}

	public void QueueClientArriveAnimation()
	{
		Debug.Log("Arriving");
		PlayerStore.Instance.isClientArrived = false;
		PlayerStore.Instance.isClientBeingArrested = false;
		PlayerStore.Instance.isHandlingShootout = false;
		PlayerStore.Instance.ClientFlagUp();
		DisplayClient();
		clientGameObject.StartApprochingStore();
	}

	public void QueueClientLeaveAnimation(bool right = false)
	{
		if (right)
		{
			clientGameObject.StartLeavingStoreRight();
		}
		else
		{
			clientGameObject.StartLeavingStore();
		}
	}

	public void OnClientStartLeaving(bool right = false)
	{
		Debug.Log("Leaving");
		if (PlayerStore.Instance.isClientArrived)
		{
			PlayerStore.Instance.ClientFlagUp();
		}
		PlayerStore.Instance.isClientBeingArrested = false;
		if (!Instance.isShot)
		{
			PlayerStore.Instance.isHandlingShootout = false;
		}
		StoreSecButton.Instance.ResetLight();
		if (isShootingMode && !isShot)
		{
			ExitShootingMode();
		}
		QueueClientLeaveAnimation(right);
	}

	public void DisplayClient()
	{
		Debug.Log("Displaying Client");
		clientGameObject.gameObject.SetActive(value: true);
	}

	public void HideClient()
	{
		Debug.Log("Hiding Client");
		clientGameObject.gameObject.SetActive(value: false);
	}

	public void OpenDebtsUI()
	{
	}

	public void OpenServiceUI()
	{
	}

	public void OpenTabletUI()
	{
	}

	public void OnNextClientArrived()
	{
		StoreClientInstance storeClientInstance = PlayerStore.Instance.currentClientInstance;
		if (storeClientInstance == null)
		{
			Debug.Log("OnNextClientArrived with no customer, ignoring");
			return;
		}
		if (storeClientInstance.GetClientBlueprint().OnArrival != null)
		{
			storeClientInstance.GetClientBlueprint().OnArrival();
		}
		Debug.Log(storeClientInstance.GetClientBlueprint().identifier + " " + storeClientInstance.GetClientBlueprint().clientIntent);
		PlayerStore.Instance.OnCriminalVisit(storeClientInstance.GetClientBlueprint());
		PlayerStore.Instance.RefreshCounterItem();
		isAboutToStartMainDialog = true;
		StartCoroutine(MetaHelper.WaitAndDo(0.25f, delegate
		{
			if (PlayerStore.Instance.currentClientInstance == null)
			{
				isAboutToStartMainDialog = false;
			}
			else
			{
				StoreClient clientBlueprint = storeClientInstance.GetClientBlueprint();
				if (clientBlueprint.mainDialogue != null)
				{
					clientBlueprint.mainDialogue.isMainDialog = true;
					RecorderUIManager.Instance.AddSeperator();
					ModSourceHelper.RecordClientSource(clientBlueprint);
				}
				if (clientBlueprint.mainDialogue != null)
				{
					if (clientBlueprint.clientIntent == StoreClient.ClientIntent.DIALOGUE)
					{
						clientBlueprint.StartMainDialogue();
					}
					else if (storeClientInstance.GetClientBlueprint().clientIntent == StoreClient.ClientIntent.SELL)
					{
						clientBlueprint.StartMainDialogue();
					}
					else if (clientBlueprint.clientIntent == StoreClient.ClientIntent.BUY)
					{
						clientBlueprint.StartMainDialogue();
					}
					else if (clientBlueprint.clientIntent == StoreClient.ClientIntent.INSPECTION)
					{
						clientBlueprint.StartMainDialogue();
					}
					else
					{
						clientBlueprint.StartMainDialogue();
					}
				}
				isAboutToStartMainDialog = false;
			}
		}));
		foreach (GameItem childItem in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			childItem.AddClientItemBuyingFeature();
		}
	}

	public void EnableBackingBlock()
	{
		backgroundBlocking.gameObject.SetActive(value: true);
	}

	public void DisableBackingBlock()
	{
		backgroundBlocking.gameObject.SetActive(value: false);
	}

	public void StartBlackscreenSequence(bool playFullSequence = true, Action actionDuringBlack = null, Action actionAfterBlack = null, float waitTime = 0.25f, float sequenceTime = 0.2f)
	{
		StartCoroutine(BlackScreenSequence(playFullSequence, actionDuringBlack, actionAfterBlack, waitTime, sequenceTime));
	}

	private IEnumerator BlackScreenSequence(bool playFullSequence = true, Action actionDuringBlack = null, Action actionAfterBlack = null, float waitTime = 0.25f, float sequenceTime = 0.2f)
	{
		blackscreen.gameObject.SetActive(value: true);
		Color color = new Color(0.16078432f, 2f / 15f, 0.20392157f, 0f);
		blackscreen.color = color;
		float t2 = 0f;
		while (t2 < sequenceTime)
		{
			t2 += Time.deltaTime;
			color.a = Mathf.Clamp01(t2 / sequenceTime);
			blackscreen.color = color;
			yield return null;
		}
		try
		{
			actionDuringBlack?.Invoke();
		}
		catch (Exception ex)
		{
			Debug.LogError("[BlackScreenSequence] actionDuringBlack threw, continuing sequence: " + ex);
		}
		if (!playFullSequence)
		{
			yield break;
		}
		yield return new WaitForSeconds(waitTime);
		if (GameOverUIManager.Instance != null && GameOverUIManager.Instance.IsShowing)
		{
			blackscreen.color = Color.black;
		}
		else
		{
			t2 = 0f;
			while (t2 < sequenceTime)
			{
				t2 += Time.deltaTime;
				color.a = Mathf.Clamp01(1f - t2 / sequenceTime);
				blackscreen.color = color;
				yield return null;
			}
			blackscreen.gameObject.SetActive(value: false);
		}
		try
		{
			actionAfterBlack?.Invoke();
		}
		catch (Exception ex2)
		{
			Debug.LogError("[BlackScreenSequence] actionAfterBlack threw: " + ex2);
		}
	}

	public void FinishBlackscreenSequence(bool fast = false)
	{
		StartCoroutine(PlayerFinishBlackScreenSequence(fast));
	}

	private IEnumerator PlayerFinishBlackScreenSequence(bool fast)
	{
		Debug.Log("Starting");
		int counter = 100;
		Color color = new Color(0f, 0f, 0f, 1f);
		blackscreen.color = color;
		blackscreen.gameObject.SetActive(value: true);
		if (!fast)
		{
			yield return new WaitForSeconds(0.5f);
		}
		while (counter > 75)
		{
			color.a -= 0.01f;
			blackscreen.color = color;
			yield return new WaitForSeconds(0.002f);
			counter--;
		}
		while (counter > 0)
		{
			color.a -= 0.01f;
			blackscreen.color = color;
			yield return new WaitForSeconds(0.002f);
			counter--;
		}
		blackscreen.gameObject.SetActive(value: false);
	}

	public void CloseAllUI()
	{
		NotebookUIManager.Instance.CloseUI();
		NewsUIManager.Instance.CloseUI();
		NegociationUIManager.Instance.CloseUI();
	}

	public void IncrementMouseFlag()
	{
		mouseLockFlag++;
		OnMouseFlagChanged();
	}

	public void DecrementMouseFlag()
	{
		mouseLockFlag--;
		OnMouseFlagChanged();
	}

	public void OnMouseFlagChanged()
	{
		if (mouseLockFlag > 0)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		else
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

	public void ToggleControlHintOverlay()
	{
		if (controlHintOverlay.activeSelf)
		{
			controlHintOverlay.SetActive(value: false);
			TutorialUIManager.Instance.OnHintClosed();
		}
		else
		{
			controlHintOverlay.SetActive(value: true);
		}
	}

	public void OnDisplayCaseUpgradeI()
	{
		showcaseRenderer.sprite = showcaseMedium;
	}

	public void OnDisplayCaseUpgradeII()
	{
		showcaseRenderer.sprite = showcaseLarge;
	}

	public void StartArrestSequence(float delayBeforeSecArrive = 0f)
	{
		PlayerStore.Instance.isClientBeingArrested = true;
		PlayerStore.Instance.ClientFlagUp();
		PlayerStore.Instance.ClientFlagUp();
		if (delayBeforeSecArrive <= 0f)
		{
			secGameObject.StartArrive();
			secGameObject2.StartArrive();
			return;
		}
		StartCoroutine(MetaHelper.WaitAndDo(delayBeforeSecArrive, delegate
		{
			secGameObject.StartArrive();
			secGameObject2.StartArrive();
		}));
	}

	public void StartShootoutSequence()
	{
		PlayerStore.Instance.isHandlingShootout = true;
		PlayerStore.Instance.ClientFlagUp();
		PlayerStore.Instance.ClientFlagUp();
		secGameObject.StartArrive();
	}

	public void OnArrestDialoguePlayed()
	{
		PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false, right: false);
		StartLeaveSec();
	}

	public void StartLeaveSec()
	{
		Debug.Log("[StoreUIManager] StartLeaveSec arrested=" + PlayerStore.Instance.isClientBeingArrested + " shootout=" + PlayerStore.Instance.isHandlingShootout + " client=" + ((PlayerStore.Instance.currentClientInstance != null) ? PlayerStore.Instance.currentClientInstance.GetClientBlueprint().identifier : "none"));
		Instance.isShot = false;
		Instance.shotClientInstance = null;
		PlayerStore.Instance.isHandlingShootout = false;
		DialogUIManager.Instance.isSecMonologuePlaying = false;
		secGameObject.StartLeave();
		secGameObject2.StartLeave();
	}

	public void ClearArrestState()
	{
		if (PlayerStore.Instance.isClientBeingArrested || secGameObject.gameObject.activeSelf || secGameObject2.gameObject.activeSelf || StoreGlass.IsClosed)
		{
			Debug.Log("<color=red>Arrest state leaked into the night, clearing it</color>");
			PlayerStore.Instance.isClientBeingArrested = false;
			PlayerStore.Instance.isHandlingShootout = false;
			PlayerStore.Instance.onSecAlarmLeftAction = null;
			DialogUIManager.Instance.isSecMonologuePlaying = false;
			isReportedAug = false;
			isShot = false;
			shotClientInstance = null;
			pendingSurvivedShot = null;
			DialogUIManager.Instance.ClearPausedConversation();
			secGameObject.gameObject.SetActive(value: false);
			secGameObject2.gameObject.SetActive(value: false);
			StoreGlass.OpenGlassDoor();
			PlayerStore.Instance.lockNextBellFlag = 0;
		}
	}

	public void OnGenericArrived(string id)
	{
		if (!id.Equals("SEC"))
		{
			return;
		}
		if (PlayerStore.Instance.isHandlingShootout)
		{
			DialogUIManager.Instance.isSecMonologuePlaying = true;
			int shootResult = AugHelper.GetShootResult();
			DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecShootoutDialog(shootResult));
			return;
		}
		bool flag = false;
		if (isReportedAug)
		{
			isReportedAug = false;
			DialogUIManager.Instance.isSecMonologuePlaying = true;
			if (!DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecAugDialog(), forcedStart: true))
			{
				Debug.Log("Sec aug monologue refused, leaving immediately");
				StartLeaveSec();
				PlayerStore.Instance.isClientBeingArrested = false;
			}
		}
		else if (PlayerStore.Instance.currentClientInstance != null)
		{
			flag = PlayerStore.Instance.currentClientInstance.GetClientBlueprint().IsWanted();
			DialogUIManager.Instance.InterruptDialog();
			DialogUIManager.Instance.HideChoiceBubble();
			Debug.Log("Starting Arrest Dialogue");
			if (PlayerStore.Instance.currentClientInstance.GetClientBlueprint().customOnArrestDialogue != null)
			{
				DialogUIManager.Instance.StartDialogue(PlayerStore.Instance.currentClientInstance.GetClientBlueprint().customOnArrestDialogue);
			}
			else if (flag)
			{
				DialogUIManager.Instance.StartDialogue(StoreClientDialogList.ClientArrestDialogCriminal());
			}
			else
			{
				DialogUIManager.Instance.StartDialogue(StoreClientDialogList.ClientArrestDialog());
			}
		}
		else
		{
			Debug.Log("No customer found during arrest!");
			StartLeaveSec();
		}
	}

	public void OnGenericLeft(string id)
	{
		if (id.Equals("SEC"))
		{
			PlayerStore.Instance.ClientFlagDown();
			PlayerStore.Instance.ClientFlagDown();
			Instance.StoreGlass.OpenGlassDoor();
			if (PlayerStore.Instance.onSecAlarmLeftAction != null)
			{
				Debug.Log("Executing onSecAlarmLeftAction");
				PlayerStore.Instance.onSecAlarmLeftAction();
				PlayerStore.Instance.onSecAlarmLeftAction = null;
			}
		}
	}

	public void OnPowerOff()
	{
	}

	public void OnPowerOn()
	{
	}

	public void CloseUI(string identifier)
	{
		if (identifier == null || identifier.Equals(""))
		{
			return;
		}
		if (identifier.Equals("network_upgrade"))
		{
			WildUIManager.Instance.CloseUI();
		}
		else if (identifier.Equals("aquascan"))
		{
			WaterScannerUIManager.Instance.Close();
		}
		else if (identifier.Equals("chem_scanner"))
		{
			ChemScannerUIManager.Instance.Close();
		}
		else if (identifier.Equals("reputation"))
		{
			ReputationUIManager.Instance.CloseUI();
		}
		else if (identifier.Equals("adv_calendar"))
		{
			AdvCalendarUIManager.Instance.CloseUI();
		}
		else if (identifier.Equals("labeler_blue"))
		{
			LabelerBlueUIManager.Instance.CloseUI();
		}
		else if (identifier.Equals("wanted_paper"))
		{
			PaperUIManager.Instance.CloseWantedPaper();
		}
		else if (identifier.Equals("security_record"))
		{
			RecorderUIManager.Instance.CloseUI();
		}
		else if (identifier.Equals("aug_guide"))
		{
			PaperUIManager.Instance.CloseAugIdGuide();
		}
		else if (identifier.Equals("multi_page"))
		{
			MultiPageUIManager.Instance.CloseUI();
		}
		else if (identifier.Equals("negociation_ui"))
		{
			NegociationUIManager.Instance.CloseUI();
		}
		else if (identifier.Equals("mentor_note"))
		{
			PaperUIManager.Instance.CloseUI();
		}
		else if (identifier.Equals("recorder_ui"))
		{
			RecorderUIManager.Instance.CloseUI();
		}
		else if (identifier.Equals("cig_guide"))
		{
			PaperUIManager.Instance.CloseCigaretteGuide();
		}
		else if (identifier.Equals("injector_guide"))
		{
			PaperUIManager.Instance.CloseInjectorGuide();
		}
		else if (identifier.Equals("stamp_guide"))
		{
			PaperUIManager.Instance.CloseStampGuide();
		}
		else if (identifier.Equals("exchange_ui"))
		{
			ExchangeUIManager.Instance.ClosePanel();
		}
		else if (identifier.Equals("newspaper_ui"))
		{
			NewsUIManager.Instance.CloseUI();
		}
		else if (identifier.Equals("joe_card"))
		{
			if (PaperUIManager.Instance != null)
			{
				PaperUIManager.Instance.OnJoeCardClosed();
			}
		}
		else
		{
			Debug.LogError("StoreUIManager: CloseUI called with unknown identifier: " + identifier);
		}
	}

	public void RunMorningDialogue()
	{
		StartCoroutine(MetaHelper.WaitAndDo(2f, delegate
		{
			DialogUIManager.Instance.StartDialogue(PlayerStore.Instance.currentClientInstance.GetClientBlueprint().mainDialogue);
		}));
	}

	public void Notify(string message, string color = "default")
	{
		ReputationUIManager.Instance.Notify(message, color);
	}

	public void TriggerBombing(string ending = "bombed")
	{
		PlayerStore.Instance.callNextWhenPossible = false;
		PlayerStore.Instance.ClientFlagUp();
		PlayerStore.Instance.lockShutterButtonFlag++;
		AudioManager.Instance.Play2("countdown");
		StartCoroutine(MetaHelper.WaitAndDo(4.5f, delegate
		{
			PlayerStore.Instance.ExecuteGameOver(ending);
			AudioManager.Instance.Play2("explosionDebris");
		}));
	}

	public void ResetClientPosition()
	{
		_ = defaultClientPosition;
	}

	public void SetClientPositionToDrJackson()
	{
	}

	public void EnterShootingMode()
	{
		if (PlayerStore.Instance.currentClientInstance == null)
		{
			return;
		}
		int sequence = ++shootingModeSequence;
		Instance.StartBlackscreenSequence(playFullSequence: true, delegate
		{
			Instance.clientGameObject.SetImmediatlyArrive();
			mainCamera.SetActive(value: false);
			shootingCamera.SetActive(value: true);
			isShootingMode = true;
			arm.SetActive(value: true);
			inventoryCanvas.SetActive(value: false);
			UIFamilyLayer.SetRaisedCanvasActive(active: false);
			worldInteractCanvas.SetActive(value: false);
			paperUICanvasObject.gameObject.SetActive(value: false);
			isShot = false;
			shootingWoundRenderer.gameObject.SetActive(value: false);
			shootingBodyRenderer.gameObject.SetActive(value: false);
			body.gameObject.SetActive(value: false);
			clientGameObject.image.color = Color.white;
			DialogUIManager.Instance.SetShootingMode();
			StoreClient storeClient = PlayerStore.Instance.currentClientInstance?.GetClientBlueprint();
			if (storeClient != null && storeClient.holsterResumesDialogue)
			{
				DialogUIManager.Instance.PauseConversation();
			}
			else
			{
				DialogUIManager.Instance.InterruptDialog();
				DialogUIManager.Instance.RemoveCustomerDialogue();
			}
			AudioManager.Instance.Play2("shotgun_pump");
		}, delegate
		{
			if (sequence == shootingModeSequence && isShootingMode)
			{
				shootingUI.SetActive(value: true);
				DialogUIManager.Instance.StartDialogue(AugHelper.GetClientReactionUnholster());
			}
		}, 0.25f, 0.1f);
	}

	public void ExitShootingMode()
	{
		shootingModeSequence++;
		Instance.StartBlackscreenSequence(playFullSequence: true, delegate
		{
			NegociationUIManager.Instance.CloseUI();
			mainCamera.SetActive(value: true);
			shootingCamera.SetActive(value: false);
			isShootingMode = false;
			arm.SetActive(value: false);
			inventoryCanvas.SetActive(value: true);
			UIFamilyLayer.SetRaisedCanvasActive(active: true);
			worldInteractCanvas.SetActive(value: true);
			paperUICanvasObject.gameObject.SetActive(value: true);
			DialogUIManager.Instance.SetNormalMode();
			shootingUI.SetActive(value: false);
			shootingWoundRenderer.gameObject.SetActive(value: false);
			if (!isShot)
			{
				clientGameObject.image.color = Color.white;
			}
			AudioManager.Instance.Play2("shotgun_cock");
			if (!isShot)
			{
				bool flag = (PlayerStore.Instance.currentClientInstance?.GetClientBlueprint())?.holsterResumesDialogue ?? false;
				if (pendingSurvivedShot == null && !DialogUIManager.Instance.ResumeConversation() && !flag)
				{
					PlayerStore.Instance.DismissCurrentClient();
				}
			}
		}, delegate
		{
		}, 0.25f, 0.1f);
	}

	public void EnterMagnifyingMode()
	{
		if (!isMagnifyingMode && !isShootingMode && PlayerStore.Instance.currentClientInstance != null)
		{
			Instance.StartBlackscreenSequence(playFullSequence: true, delegate
			{
				Instance.clientGameObject.SetImmediatlyArrive();
				mainCamera.SetActive(value: false);
				shootingCamera.SetActive(value: true);
				isMagnifyingMode = true;
				inventoryCanvas.SetActive(value: false);
				UIFamilyLayer.SetRaisedCanvasActive(active: false);
				worldInteractCanvas.SetActive(value: false);
				magnifyingScreen.SetActive(value: true);
				magnifyingScreenCollapsed.SetActive(value: false);
				magnifyBaseOrthoSize = LetterboxManager.GetIntendedOrthographicSize(shootingCamera.GetComponent<Camera>());
				magnifyBaseCameraX = shootingCamera.transform.position.x;
				magnifyBaseCameraY = shootingCamera.transform.position.y;
				magnifyBaseCameraZ = shootingCamera.transform.position.z;
				magnifyPanCurrent = Vector3.zero;
				isMagnifyZoomedIn = false;
				zoomUI.SetActive(value: true);
				AudioManager.Instance.Play2("click_vintage");
				DialogUIManager.Instance.SetShootingMode();
			}, delegate
			{
			}, 0.25f, 0.1f);
		}
	}

	public void ExitMagnifyingMode()
	{
		if (isMagnifyingMode)
		{
			Instance.StartBlackscreenSequence(playFullSequence: true, delegate
			{
				mainCamera.SetActive(value: true);
				shootingCamera.SetActive(value: false);
				isMagnifyingMode = false;
				inventoryCanvas.SetActive(value: true);
				UIFamilyLayer.SetRaisedCanvasActive(active: true);
				worldInteractCanvas.SetActive(value: true);
				magnifyingScreen.SetActive(value: false);
				magnifyingScreenCollapsed.SetActive(value: true);
				LetterboxManager.SetIntendedOrthographicSize(shootingCamera.GetComponent<Camera>(), magnifyBaseOrthoSize);
				shootingCamera.transform.position = new Vector3(magnifyBaseCameraX, magnifyBaseCameraY, magnifyBaseCameraZ);
				magnifyPanCurrent = Vector3.zero;
				isMagnifyZoomedIn = false;
				zoomUI.SetActive(value: false);
				AudioManager.Instance.Play2("click_vintage");
				DialogUIManager.Instance.SetNormalMode();
			}, delegate
			{
			}, 0.25f, 0.1f);
		}
	}

	public void ToggleMagnifyZoom()
	{
		if (isMagnifyingMode)
		{
			isMagnifyZoomedIn = !isMagnifyZoomedIn;
			LetterboxManager.SetIntendedOrthographicSize(shootingCamera.GetComponent<Camera>(), isMagnifyZoomedIn ? (magnifyBaseOrthoSize / magnifyZoomFactor) : magnifyBaseOrthoSize);
			AudioManager.Instance.Play2("click_vintage");
		}
	}

	private void UpdateMagnifyCamera()
	{
		Camera component = shootingCamera.GetComponent<Camera>();
		float num = (isMagnifyZoomedIn ? magnifyZoomCameraY : magnifyBaseCameraY);
		float num2 = Mathf.Clamp((Input.mousePosition.x - (float)LetterboxManager.GameAreaX) / (float)LetterboxManager.GameAreaWidth * 2f - 1f, -1f, 1f);
		float num3 = Mathf.Clamp((Input.mousePosition.y - (float)LetterboxManager.GameAreaY) / (float)LetterboxManager.GameAreaHeight * 2f - 1f, -1f, 1f);
		float orthographicSize = component.orthographicSize;
		float num4 = orthographicSize * component.aspect;
		magnifyPanCurrent = Vector3.Lerp(b: new Vector3(num2 * num4 * magnifyPanFactor, num3 * orthographicSize * magnifyPanFactor, 0f), a: magnifyPanCurrent, t: 1f - Mathf.Exp((0f - magnifyPanSmoothing) * Time.deltaTime));
		float num5 = Time.time * magnifyBreathFrequency * MathF.PI * 2f;
		float num6 = Mathf.Sin(num5 * 0.7f) * magnifyBreathAmplitudeX;
		float num7 = Mathf.Sin(num5) * magnifyBreathAmplitudeY;
		shootingCamera.transform.position = new Vector3(magnifyBaseCameraX + magnifyPanCurrent.x + num6, num + magnifyPanCurrent.y + num7, magnifyBaseCameraZ);
	}

	public void Shoot()
	{
		Arm component = arm.GetComponent<Arm>();
		component.TriggerMuzzleFlash();
		component.TriggerRecoil();
		StoreClient storeClient = PlayerStore.Instance.currentClientInstance?.GetClientBlueprint();
		if (storeClient != null && storeClient.survivesShot)
		{
			ShootSurvivor(storeClient);
			return;
		}
		DialogUIManager.Instance.ClearPausedConversation();
		DialogUIManager.Instance.InterruptDialog();
		isShot = true;
		shotClientInstance = PlayerStore.Instance.currentClientInstance ?? PlayerStore.Instance.lastVisitedClient;
		if (shotClientInstance != null)
		{
			ChemicalFeedbackHelper.CancelFeedbackForShotClient(shotClientInstance.GetClientBlueprint().identifier);
		}
		PlayerStore.Instance.RemoveNotOwnedItemFromTable();
		InspectionUIManager.Instance.CloseConfrontPanel();
		AudioManager.Instance.Play2("shotgun_fire");
		shootingUI.SetActive(value: false);
		AchievementTracker.Instance.CheckAugHunter();
		StartCoroutine(WoundAnimationRoutine());
	}

	private IEnumerator WoundAnimationRoutine()
	{
		shootingWoundRenderer.gameObject.SetActive(value: true);
		bool bodyStarted = false;
		for (int i = 0; i < shootingWoundSprites.Count; i++)
		{
			shootingWoundRenderer.sprite = shootingWoundSprites[i];
			switch (i)
			{
			case 0:
				clientGameObject.image.color = Color.white;
				break;
			case 1:
				clientGameObject.image.color = shotDarkenColor;
				break;
			case 2:
				bodyStarted = true;
				clientGameObject.gameObject.SetActive(value: false);
				StartCoroutine(BodyFallRoutine());
				break;
			}
			yield return new WaitForSeconds(woundFrameDuration);
		}
		shootingWoundRenderer.gameObject.SetActive(value: false);
		if (!bodyStarted)
		{
			clientGameObject.gameObject.SetActive(value: false);
			StartCoroutine(BodyFallRoutine());
		}
	}

	private IEnumerator BodyFallRoutine()
	{
		shootingBodyRenderer.gameObject.SetActive(value: true);
		int collapseSoundFrame = Mathf.Max(0, shootingBodySprites.Count - 2);
		for (int i = 0; i < shootingBodySprites.Count; i++)
		{
			shootingBodyRenderer.sprite = shootingBodySprites[i];
			if (i == collapseSoundFrame)
			{
				AudioManager.Instance.Play2("body_collapse_floor");
			}
			yield return new WaitForSeconds(bodyFrameDuration);
		}
		body.gameObject.SetActive(value: true);
		shootingBodyRenderer.gameObject.SetActive(value: false);
		DialogUIManager.Instance.InterruptDialog();
		Instance.ExitShootingMode();
		StartShootoutSequence();
	}

	public bool CanShoot()
	{
		if (isShot || pendingSurvivedShot != null)
		{
			return false;
		}
		return true;
	}

	private void ShootSurvivor(StoreClient target)
	{
		DialogUIManager.Instance.ClearPausedConversation();
		DialogUIManager.Instance.InterruptDialog();
		target.nonShootable = true;
		if (target.onShotSurvived != null)
		{
			target.onShotSurvived();
		}
		pendingSurvivedShot = delegate
		{
			if (PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint() == target)
			{
				Dialogue dialogue = ((target.shotSurvivedDialogue != null) ? target.shotSurvivedDialogue() : null);
				if (dialogue == null || !DialogUIManager.Instance.StartDialogue(dialogue, forcedStart: true))
				{
					PlayerStore.Instance.DismissCurrentClient();
				}
			}
		};
		AudioManager.Instance.Play2("shotgun_fire");
		shootingUI.SetActive(value: false);
		StartCoroutine(SurvivedShotRoutine());
	}

	private IEnumerator SurvivedShotRoutine()
	{
		clientGameObject.FreezeIdle();
		clientGameObject.image.color = Color.white;
		shootingWoundRenderer.gameObject.SetActive(value: true);
		int frames = Mathf.Min(3, shootingWoundSprites.Count);
		for (int i = 0; i < frames; i++)
		{
			shootingWoundRenderer.sprite = shootingWoundSprites[i];
			if (i < frames - 1)
			{
				yield return new WaitForSeconds(woundFrameDuration);
			}
		}
		yield return new WaitForSeconds(0.6f);
		if (isShootingMode)
		{
			ExitShootingMode();
		}
		while (isShootingMode)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.3f);
		Action action = pendingSurvivedShot;
		pendingSurvivedShot = null;
		action?.Invoke();
	}

	public void StartTyping()
	{
		isTyping = true;
	}

	public void StopTyping()
	{
		isTyping = false;
	}

	public bool IsTyping()
	{
		if (!isTyping)
		{
			return IsTextFieldFocused();
		}
		return true;
	}

	public static bool IsTextFieldFocused()
	{
		if (QuantumConsole.Instance != null && QuantumConsole.Instance.IsFocused)
		{
			return true;
		}
		EventSystem current = EventSystem.current;
		if (current == null)
		{
			return false;
		}
		GameObject currentSelectedGameObject = current.currentSelectedGameObject;
		if (currentSelectedGameObject == null)
		{
			return false;
		}
		TMP_InputField component = currentSelectedGameObject.GetComponent<TMP_InputField>();
		if (component != null)
		{
			return component.isFocused;
		}
		InputField component2 = currentSelectedGameObject.GetComponent<InputField>();
		if (component2 != null)
		{
			return component2.isFocused;
		}
		return false;
	}

	public static bool IsAnyTyping()
	{
		if (Instance != null && Instance.isTyping)
		{
			return true;
		}
		return IsTextFieldFocused();
	}
}

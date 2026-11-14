using System;
using UnityEngine;

public class TutorialUIManager : MonoBehaviour
{
	[Serializable]
	public struct TabletHintPlacement
	{
		public Vector2 anchor;

		public Vector2 pivot;

		public Vector2 offset;

		public TabletHintPlacement(Vector2 anchor, Vector2 pivot, Vector2 offset)
		{
			this.anchor = anchor;
			this.pivot = pivot;
			this.offset = offset;
		}
	}

	private int tutorialStage = -1;

	private bool closeShutterHintPlayed;

	private bool nightlyServicePlayed;

	private bool isSecondDay;

	private int sleepCount;

	private bool displayCaseTutorialPlayed;

	public bool isIntroBrowsingUpgrade;

	private bool advanceDialogHintArmed;

	private bool sortMenuHintActive;

	public GameObject tutorialOpenShutterDoor;

	public GameObject tutorial2;

	public GameObject tutorial21;

	public GameObject tutorial3;

	public GameObject tutorialCloseShutterDoor;

	public GameObject tutorialDoubleClick;

	public GameObject tutorialDrag;

	public GameObject tutorialQuickTransfer;

	public GameObject tutorialRecorder;

	public GameObject tutorialDossier;

	public GameObject tutorialBuyDrag;

	public GameObject tutorialBuyTablet;

	public GameObject tutorialSellDrag;

	public GameObject tutorialSleep;

	public GameObject tutorialTransferButton;

	public GameObject tutorialBarter;

	public GameObject tutorialNightlyService;

	public GameObject tutorialAfterhoursActivity;

	public GameObject tutorialDisplayCase;

	public GameObject tutorialClienteleUI;

	public GameObject tutorialBargain;

	public GameObject tutorialBargain2;

	public GameObject tutorialReputation;

	public GameObject tutorialAdvanceDialog;

	public static readonly TabletHintPlacement DEFAULT_BARGAIN_HINT = new TabletHintPlacement(new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(150f, -150f));

	public static readonly TabletHintPlacement DEFAULT_BARGAIN2_HINT = new TabletHintPlacement(new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1150f, 0f));

	public static readonly TabletHintPlacement DEFAULT_NIGHTLY_HINT = new TabletHintPlacement(new Vector2(0.5f, 1f), new Vector2(0f, 0f), new Vector2(-250f, -175f));

	[Header("Tablet hint placement (on the generated tablets)")]
	public TabletHintPlacement bargainHintPlacement = DEFAULT_BARGAIN_HINT;

	public TabletHintPlacement bargain2HintPlacement = DEFAULT_BARGAIN2_HINT;

	public TabletHintPlacement nightlyServiceHintPlacement = DEFAULT_NIGHTLY_HINT;

	public GameObject tutorialMultiSelect;

	public GameObject tutorialSortMenu;

	public static TutorialUIManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	[ContextMenu("Reset tablet hint placements")]
	private void ResetTabletHintPlacements()
	{
		bargainHintPlacement = DEFAULT_BARGAIN_HINT;
		bargain2HintPlacement = DEFAULT_BARGAIN2_HINT;
		nightlyServiceHintPlacement = DEFAULT_NIGHTLY_HINT;
	}

	public void Start()
	{
	}

	public void TutorialStart()
	{
		if (!PlayerStore.Instance.tutorialEnable || tutorialStage == 999)
		{
			CloseAllTutorial();
			return;
		}
		tutorialStage = 110;
		StoreUIManager.Instance.ToggleControlHintOverlay();
	}

	public void ShutterClicked()
	{
		if (PlayerStore.Instance.tutorialEnable)
		{
			if (tutorialStage == 0)
			{
				tutorialOpenShutterDoor.SetActive(value: false);
				tutorial2.SetActive(value: true);
				tutorialStage++;
			}
			else if (tutorialStage == 11)
			{
				tutorialCloseShutterDoor.SetActive(value: false);
				tutorialStage++;
				tutorialSleep.SetActive(value: true);
			}
			else if (tutorialStage == 13)
			{
				tutorialOpenShutterDoor.SetActive(value: false);
				isSecondDay = true;
				tutorialStage = 14;
			}
			else if (tutorialStage == 15)
			{
				tutorialCloseShutterDoor.SetActive(value: false);
				tutorialAfterhoursActivity.SetActive(value: true);
				tutorialStage = 16;
			}
		}
	}

	public void OnHintClosed()
	{
		if (tutorialStage == 110)
		{
			tutorialOpenShutterDoor.SetActive(value: true);
			tutorialStage = 0;
		}
	}

	public void BellRing()
	{
		if (tutorialStage == 1)
		{
			tutorial2.SetActive(value: false);
			tutorialStage++;
		}
		else if (tutorialStage == 9)
		{
			tutorial2.SetActive(value: false);
			tutorialStage = 49;
		}
		else if (tutorialStage == 53)
		{
			tutorial2.SetActive(value: false);
			tutorialStage = 54;
		}
		else if (tutorialStage == 58)
		{
			tutorial2.SetActive(value: false);
			tutorialStage = 10;
		}
	}

	public void BuyCustomerArrived()
	{
		tutorialStage = 49;
		CloseAllTutorial();
		if (tutorialStage == 49)
		{
			tutorialMultiSelect.SetActive(value: true);
			tutorialBuyDrag.SetActive(value: true);
			tutorialStage = 50;
		}
	}

	public void BuyItemDrag()
	{
		if (tutorialStage == 50)
		{
			tutorialBuyDrag.SetActive(value: false);
			tutorialStage = 52;
		}
		if (tutorialStage == 55)
		{
			tutorialSellDrag.SetActive(value: false);
			tutorialStage = 57;
		}
	}

	public void BuyTabletOpened()
	{
		if (tutorialStage == 51)
		{
			tutorialBuyTablet.SetActive(value: false);
			tutorialStage = 52;
		}
		if (tutorialStage == 56)
		{
			tutorialBuyTablet.SetActive(value: false);
			tutorialStage = 57;
		}
	}

	public void BuyDealed()
	{
		if (tutorialStage == 52)
		{
			tutorialStage = 53;
			tutorial2.SetActive(value: true);
			if (!EmporiumOption.SortMenuDisabled)
			{
				sortMenuHintActive = true;
				if (tutorialSortMenu != null)
				{
					tutorialSortMenu.SetActive(value: true);
				}
			}
		}
		if (tutorialStage == 57)
		{
			tutorialStage = 58;
			tutorial2.SetActive(value: true);
		}
	}

	public void OnSellArrived()
	{
		tutorialStage = 54;
		if (tutorialStage == 54)
		{
			tutorialSellDrag.SetActive(value: true);
			tutorialStage = 55;
		}
	}

	public void InteractHint()
	{
		if (tutorialStage == 4)
		{
			tutorial3.SetActive(value: true);
			tutorial21.SetActive(value: true);
			tutorialStage++;
		}
	}

	public void OnPageRead()
	{
		if (tutorialStage == 5)
		{
			tutorial3.SetActive(value: false);
			tutorial21.SetActive(value: false);
			tutorialDoubleClick.SetActive(value: true);
			tutorialStage++;
		}
	}

	public void OnDoubleClick()
	{
		if (tutorialStage == 6)
		{
			tutorialDoubleClick.SetActive(value: false);
			tutorialStage++;
			tutorialStage = 8;
			tutorialRecorder.SetActive(value: true);
		}
	}

	public void OnMentorDone()
	{
		tutorialStage = 3;
		tutorialQuickTransfer.SetActive(value: true);
	}

	public void OnDrag()
	{
		if (tutorialStage == 2)
		{
			tutorialDrag.SetActive(value: false);
			tutorialStage++;
			tutorialQuickTransfer.SetActive(value: true);
		}
	}

	public void OnControlClick()
	{
		if (tutorialStage == 3)
		{
			tutorialQuickTransfer.SetActive(value: false);
			tutorialStage = 5;
			tutorial3.SetActive(value: true);
			tutorial21.SetActive(value: true);
		}
	}

	public void OnDossierInserted()
	{
		if (tutorialStage == 7)
		{
			tutorialDossier.SetActive(value: false);
			tutorialStage++;
			tutorialRecorder.SetActive(value: true);
		}
	}

	public void OnRecorderOpen()
	{
		if (tutorialStage == 8)
		{
			tutorialRecorder.SetActive(value: false);
			tutorialStage++;
			tutorial2.SetActive(value: true);
		}
	}

	public void NoMoreCustomer()
	{
		bool flag = PlayerStore.Instance.storeState == PlayerStore.StoreState.CLOSED;
		if (!closeShutterHintPlayed)
		{
			closeShutterHintPlayed = true;
			if (flag)
			{
				tutorialSleep.SetActive(value: true);
				tutorialStage = 12;
			}
			else
			{
				tutorialCloseShutterDoor.SetActive(value: true);
				tutorialStage = 11;
			}
		}
		else if (isSecondDay)
		{
			isSecondDay = false;
			if (flag)
			{
				tutorialAfterhoursActivity.SetActive(value: true);
				tutorialStage = 16;
			}
			else
			{
				tutorialCloseShutterDoor.SetActive(value: true);
				tutorialStage = 15;
			}
		}
	}

	public void BackFromOutside()
	{
	}

	public void OnSleep()
	{
		sortMenuHintActive = false;
		CloseAllTutorial();
		if (!PlayerStore.Instance.tutorialEnable)
		{
			return;
		}
		sleepCount++;
		if (sleepCount == 1)
		{
			tutorialOpenShutterDoor.SetActive(value: true);
			tutorialStage = 13;
			return;
		}
		tutorialStage = 999;
		if (displayCaseTutorialPlayed)
		{
			PlayerStore.Instance.tutorialEnable = false;
		}
	}

	public void OnDayBegin()
	{
		if (PlayerStore.Instance.tutorialEnable && StoreStation.Instance.dayCounter == 6 && !displayCaseTutorialPlayed)
		{
			displayCaseTutorialPlayed = true;
			tutorialDisplayCase.SetActive(value: true);
		}
	}

	public void OnInventorySorted()
	{
		sortMenuHintActive = false;
		if (tutorialSortMenu != null && tutorialSortMenu.activeSelf)
		{
			tutorialSortMenu.SetActive(value: false);
		}
	}

	public void OnMultiSelect()
	{
		if (tutorialMultiSelect != null && tutorialMultiSelect.activeSelf)
		{
			tutorialMultiSelect.SetActive(value: false);
		}
	}

	public void OnItemAddedToDisplayCase()
	{
		if (PlayerStore.Instance.tutorialEnable && tutorialDisplayCase.activeSelf)
		{
			tutorialDisplayCase.SetActive(value: false);
			tutorialClienteleUI.SetActive(value: true);
		}
	}

	public void OnClienteleUIOpened()
	{
		if (PlayerStore.Instance.tutorialEnable && tutorialClienteleUI.activeSelf)
		{
			tutorialClienteleUI.SetActive(value: false);
		}
	}

	public void OnStoreDoorClicked()
	{
		if (tutorialStage == 16)
		{
			tutorialAfterhoursActivity.SetActive(value: false);
			tutorialStage = 17;
		}
	}

	public void OnReturnFromAfterhours()
	{
		if (tutorialStage == 17)
		{
			tutorialSleep.SetActive(value: true);
			tutorialStage = 18;
		}
	}

	public void OnLeaveStore()
	{
		if (tutorialSleep != null)
		{
			tutorialSleep.SetActive(value: false);
		}
	}

	public void OnArriveStore()
	{
		if (tutorialSleep != null && (tutorialStage == 12 || tutorialStage == 18))
		{
			tutorialSleep.SetActive(value: true);
		}
	}

	public void OnWholesaleCustomerBought()
	{
		tutorialTransferButton.SetActive(value: true);
		tutorialStage = 201;
	}

	public void OnTransferButtonClicked()
	{
		if (tutorialStage == 201)
		{
			tutorialTransferButton.SetActive(value: false);
			tutorialStage++;
		}
	}

	public void OnBarterTutorialStart()
	{
		tutorialBarter.SetActive(value: true);
	}

	public void CloseBarterTutorial()
	{
		tutorialBarter.SetActive(value: false);
	}

	private static void AttachHintToTablet(GameObject hint, GameObject tabletRoot, TabletHintPlacement placement)
	{
		if (hint == null || tabletRoot == null)
		{
			return;
		}
		RectTransform component = hint.GetComponent<RectTransform>();
		if (!(component == null))
		{
			if (component.parent != tabletRoot.transform)
			{
				component.SetParent(tabletRoot.transform, worldPositionStays: false);
			}
			component.anchorMin = placement.anchor;
			component.anchorMax = placement.anchor;
			component.pivot = placement.pivot;
			component.anchoredPosition = placement.offset;
			component.localScale = Vector3.one;
			component.SetAsLastSibling();
		}
	}

	public string ShowTabletHintForDebug(string name, bool show)
	{
		GameObject gameObject;
		GameObject gameObject2;
		TabletHintPlacement placement;
		switch (name)
		{
		case "nightly":
			gameObject = tutorialNightlyService;
			gameObject2 = ((EndOfDayUIManager.Instance != null) ? EndOfDayUIManager.Instance.UI : null);
			placement = nightlyServiceHintPlacement;
			break;
		case "bargain":
			gameObject = tutorialBargain;
			gameObject2 = ((NegociationUIManager.Instance != null) ? NegociationUIManager.Instance.uiObject : null);
			placement = bargainHintPlacement;
			break;
		case "bargain2":
			gameObject = tutorialBargain2;
			gameObject2 = ((NegociationUIManager.Instance != null) ? NegociationUIManager.Instance.uiObject : null);
			placement = bargain2HintPlacement;
			break;
		default:
			return null;
		}
		if (gameObject == null)
		{
			return "The '" + name + "' bubble is not assigned on TutorialUIManager.";
		}
		if (!show)
		{
			gameObject.SetActive(value: false);
			return name + " bubble hidden.";
		}
		if (gameObject2 == null)
		{
			return "Its tablet does not exist yet (the store scene creates it on start).";
		}
		AttachHintToTablet(gameObject, gameObject2, placement);
		gameObject.SetActive(value: true);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		string text = ((component != null) ? $" placement anchor={placement.anchor} pivot={placement.pivot} offset={placement.offset}; now parent={component.parent.name} anchorMin={component.anchorMin} pivot={component.pivot} anchoredPosition={component.anchoredPosition} size={component.sizeDelta}" : "");
		return (gameObject2.activeSelf ? (name + " bubble shown on " + gameObject2.name + ".") : (name + " bubble shown on " + gameObject2.name + "; it appears once that tablet is open.")) + text;
	}

	public void OnEndOfDayOpen()
	{
		if (PlayerStore.Instance.tutorialEnable && !nightlyServicePlayed && EndOfDayUIManager.Instance != null && EndOfDayUIManager.Instance.HasToggleableService)
		{
			AttachHintToTablet(tutorialNightlyService, (EndOfDayUIManager.Instance != null) ? EndOfDayUIManager.Instance.UI : null, nightlyServiceHintPlacement);
			tutorialNightlyService.SetActive(value: true);
			nightlyServicePlayed = true;
		}
	}

	public void DisableNightlyServiceTutorial()
	{
		tutorialNightlyService.SetActive(value: false);
	}

	public void CloseAllTutorial()
	{
		foreach (Transform item in Instance.gameObject.transform)
		{
			item.gameObject.SetActive(value: false);
		}
		if (tutorialBargain != null)
		{
			tutorialBargain.SetActive(value: false);
		}
		if (tutorialBargain2 != null)
		{
			tutorialBargain2.SetActive(value: false);
		}
		if (sortMenuHintActive && PlayerStore.Instance.tutorialEnable && tutorialSortMenu != null && !EmporiumOption.SortMenuDisabled)
		{
			tutorialSortMenu.SetActive(value: true);
		}
	}

	public void OnLunArrived()
	{
		CloseAllTutorial();
	}

	public void OnTutorialCustomerArrived()
	{
		CloseAllTutorial();
	}

	public void OnBargainTutorialCustomerArrived()
	{
		if (StartingPerk.IsAwfulNegotiator())
		{
			if (PlayerStore.Instance.tutorialEnable && !PlayerStore.Instance.bargainTutorialPlayed)
			{
				PlayerStore.Instance.bargainTutorialPlayed = true;
				tutorialReputation.SetActive(value: true);
			}
		}
		else if (PlayerStore.Instance.tutorialEnable && !PlayerStore.Instance.bargainTutorialPlayed)
		{
			AttachHintToTablet(tutorialBargain, (NegociationUIManager.Instance != null) ? NegociationUIManager.Instance.uiObject : null, bargainHintPlacement);
			tutorialBargain.SetActive(value: true);
		}
	}

	public void OnBargainUIOpened()
	{
		if (PlayerStore.Instance.tutorialEnable && !PlayerStore.Instance.bargainTutorialPlayed)
		{
			PlayerStore.Instance.bargainTutorialPlayed = true;
			tutorialBargain.SetActive(value: false);
			AttachHintToTablet(tutorialBargain2, (NegociationUIManager.Instance != null) ? NegociationUIManager.Instance.uiObject : null, bargain2HintPlacement);
			tutorialBargain2.SetActive(value: true);
			tutorialReputation.SetActive(value: true);
		}
	}

	public void OnBargainUIClosed()
	{
		if (PlayerStore.Instance.tutorialEnable)
		{
			tutorialBargain2.SetActive(value: false);
		}
	}

	public void OnReputationTutorialOpened()
	{
		if (PlayerStore.Instance.tutorialEnable)
		{
			tutorialReputation.SetActive(value: false);
			tutorialBargain2.SetActive(value: false);
		}
	}

	public void ArmAdvanceDialogHint()
	{
		advanceDialogHintArmed = true;
	}

	public void ShowAdvanceDialogHintIfArmed()
	{
		if (advanceDialogHintArmed && !EmporiumOption.Instance.autoNextDialog)
		{
			tutorialAdvanceDialog.SetActive(value: true);
		}
	}

	public void DismissAdvanceDialogHint()
	{
		if (advanceDialogHintArmed)
		{
			advanceDialogHintArmed = false;
			tutorialAdvanceDialog.SetActive(value: false);
		}
	}
}

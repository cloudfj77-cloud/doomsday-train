using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoreShutterButton : TooltipInterface, IClickable, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public GameObject shutterGameObject;

	public GameObject blockerGameObject;

	public Image blockerMaskObject;

	public Vector3 startPosition;

	public Vector3 endPosition;

	public Vector3 blockerStartPosition;

	public Vector3 blockerEndPosition;

	private float duration = 2.75f;

	private float elapsedTime;

	private bool isClosed = true;

	public AudioSource audioSource;

	public AudioSource audioSourceShutterOpen;

	public AudioSource audioSourceShutterClose;

	public AudioSource audioSourceShutterClosed;

	[Header("Button Sprites")]
	public SpriteRenderer buttonRenderer;

	public Sprite spriteNormal;

	public Sprite spriteHover;

	public Sprite spriteClick;

	private bool isHoveringButton;

	public static StoreShutterButton Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		Instance = this;
		shutterGameObject.SetActive(value: false);
	}

	public void OnClick()
	{
		TutorialUIManager.Instance.ShutterClicked();
		if (PlayerStore.Instance.lockShutterButtonFlag > 0 || PlayerStore.Instance.isClientBeingArrested || PlayerStore.Instance.isHandlingShootout)
		{
			return;
		}
		if (PlayerStore.Instance.storeState == PlayerStore.StoreState.MORNING)
		{
			if (PlayerStore.Instance.CanStartDay())
			{
				OpenShutter();
			}
		}
		else if (PlayerStore.Instance.storeState == PlayerStore.StoreState.OPEN && (PlayerStore.Instance.CanEndDay() || PlayerStore.Instance.devMode))
		{
			CloseShutter();
		}
	}

	public void CloseShutter()
	{
		if (PlayerStore.Instance.storeClientManager.clientStack.Count == 0)
		{
			EndAndClose();
		}
		else
		{
			ShowConfirmCloseUI();
		}
	}

	public void OpenShutter()
	{
		PlayerStore.Instance.OnShutterOpened();
		PlayerStore.Instance.OpenShutter();
		ToggleShutterDoor();
		PlayerStore.Instance.lockShutterButtonFlag++;
		OnShutterDoorOpenMorning();
		EmporiumEntry.Instance.ShowFrontArea();
		PlayerStore.Instance.OnLateShutterOpen();
		StartCoroutine(MetaHelper.WaitAndDo(duration, delegate
		{
			PlayerStore.Instance.lockShutterButtonFlag--;
			blockerMaskObject.gameObject.SetActive(value: false);
		}));
		EmporiumEntry.Instance.Validate();
	}

	public void OnShutterDoorOpenMorning()
	{
		ModHook.FireOnShutterOpenedEarly();
		if (PlayerStore.Instance.isNewspaperUnlocked && !StartingPerk.IsUnsubscribed())
		{
			GameInventory gameInventory = (EmporiumOption.NewspaperToTrash ? (EmporiumEntry.Instance.trashcan?.contentWindow?.child as GameInventory) : null);
			GameItem gameItem = ((gameInventory != null) ? DirectoryMaster.Item("newspaper") : null);
			if (gameItem == null || GraphUtils.TryAcceptAll(gameInventory, gameItem) > 0)
			{
				if (gameItem != null)
				{
					GraphUtils.DestroyIfUnplaced(gameItem);
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("newspaper", isOwend: true);
			}
		}
		HandlePermit();
		if (NetworkUpgrade.IsMarketplaceRequestsUnlocked() && StoreReputation.IsPerkUnlocked("REV_REQUEST") && !StoreReputation.GetPerkByID("REV_REQUEST").firstTimeUnlockActionDone)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateRevCodeword(), isOwend: true);
			StoreReputation.GetPerkByID("REV_REQUEST").firstTimeUnlockActionDone = true;
		}
		if (!PlayerStore.Instance.skipIntro)
		{
			_ = StoreStation.Instance.dayCounter;
			_ = 1;
			if (StoreStation.Instance.dayCounter == 2)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("tutorial_book", isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNote13(), isOwend: true);
			}
			if (StoreStation.Instance.dayCounter == 4)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("labeler", isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateLabelerNote(), isOwend: true);
			}
			_ = StoreStation.Instance.dayCounter;
			_ = 5;
			if (StoreStation.Instance.dayCounter == 6)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteUpgrade(), isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNote21(), isOwend: true);
			}
			if (StoreStation.Instance.dayCounter == 9)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteContraband(), isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteInspection(), isOwend: true);
				PlayerStore.Instance.isContrabandIntroduced = true;
			}
			if (StoreStation.Instance.dayCounter == 11 && !StartingPerk.IsBannedCommissary())
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteStamp(), isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteCommissary(), isOwend: true);
				PlayerStore.Instance.isStampIntroduced = true;
				PlayerStore.Instance.AddDirectSellingItemToTable("stamp_guide", isOwend: true);
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("logo_checker"));
			}
			if (StoreStation.Instance.dayCounter == 13)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(IntelHelper.CreateMentorNoteStolenItem(), isOwend: true);
				PlayerStore.Instance.isStolenItemIntroduced = true;
			}
			_ = StoreStation.Instance.dayCounter;
			_ = 14;
			if (StoreStation.Instance.dayCounter == 25)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("injector_guide", isOwend: true);
				PlayerStore.Instance.isInjectorIntroduced = true;
			}
		}
		StartingPerkContent.HandleSecretAdmirerOnOpenShutter();
		StartingPerkContent.HandleWelfareProgram();
		ShareholderHelper.HandleDividends();
		AppraisalRankHelper.ReissueLostCertificate();
		PlayerStore.Instance.CheckSkinningKnifeReward();
		PlayerStore.Instance.CheckButcherCleaver();
		LouOperativeHelper.OnMorning();
		LotteryHelper.CheckGamblersCoinReward();
		for (int i = 0; i < PlayerStore.GetStoreClientData().nuclearWasteLeftOnCounter; i++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("nuclear_waste", isOwend: true);
		}
		PlayerStore.GetStoreClientData().nuclearWasteLeftOnCounter = 0;
		ModHook.FireOnShutterOpenedLate();
	}

	public void HandlePermit()
	{
		Shotgun.ReturnIfLostWithPermit();
		if (NetworkUpgrade.IsGunPermitIUnlocked() && StoreReputation.GetSecReputation() >= 20 && !PermitHelper.HavePermit("FIREARM", 1))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("permit_gun_1", isOwend: true);
			PermitHelper.GrantedFromPermit(DirectoryMaster.Item("permit_gun_1"));
		}
		if (NetworkUpgrade.IsGunPermitIIUnlocked() && StoreReputation.GetSecReputation() >= 50 && !PermitHelper.HavePermit("FIREARM", 2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("permit_gun_2", isOwend: true);
			PermitHelper.GrantedFromPermit(DirectoryMaster.Item("permit_gun_2"));
		}
		if (NetworkUpgrade.IsGunPermitIIIUnlocked() && StoreReputation.GetSecReputation() >= 80 && !PermitHelper.HavePermit("FIREARM", 3))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("permit_gun_3", isOwend: true);
			PermitHelper.GrantedFromPermit(DirectoryMaster.Item("permit_gun_3"));
		}
	}

	public void EndAndClose()
	{
		ModHook.FireOnShutterClosedEarly();
		PlayerStore.Instance.visitLeftTonight = 1;
		StoreUIManager.Instance.closeShutterConfirmationPanel.SetActive(value: false);
		PlayerStore.Instance.RemoveNotOwnedItemFromTable();
		PlayerStore.Instance.lockShutterButtonFlag++;
		PlayerStore.Instance.healthData.wentToAfterhoursLastNight = false;
		DialogUIManager.Instance.InterruptDialog();
		DialogUIManager.Instance.HideChoiceBubble();
		if (PlayerStore.Instance.currentClientInstance != null)
		{
			Dialogue dialogue = StoreClientDialogList.CreateShutterDownClose(PlayerStore.Instance.currentClientInstance.GetClientBlueprint().displayName);
			if (dialogue != null)
			{
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		}
		PlayerStore.Instance.EndDay();
		ToggleShutterDoor();
		StartCoroutine(MetaHelper.WaitAndDo(duration, delegate
		{
			EmporiumEntry.Instance.HideFrontArea();
		}));
		StoreClientInstance clientAtClose = PlayerStore.Instance.currentClientInstance;
		StoreUIManager.Instance.StartCoroutine(MetaHelper.WaitAndDo(duration + 1.25f, delegate
		{
			if (PlayerStore.Instance.currentClientInstance == clientAtClose)
			{
				StoreUIManager.Instance.HideClient();
				bool interruptDialog = PlayerStore.Instance.playerLocation == PlayerStore.PlayerLocation.IN_STORE;
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false, right: true, interruptDialog);
			}
		}));
		StartCoroutine(MetaHelper.WaitAndDo(duration - 0.2f, delegate
		{
			audioSourceShutterClosed.Play();
		}));
		ModHook.FireOnShutterClosedLate();
	}

	public void ShowConfirmCloseUI()
	{
		StoreUIManager.Instance.closeShutterConfirmationPanel.SetActive(value: true);
	}

	public void ToggleShutterDoor()
	{
		if (shutterGameObject != null)
		{
			audioSource.Play();
			if (isClosed)
			{
				audioSourceShutterOpen.Play();
			}
			else
			{
				audioSourceShutterClose.Play();
				blockerMaskObject.gameObject.SetActive(value: true);
			}
			isClosed = !isClosed;
			elapsedTime = 0f;
		}
		else
		{
			Debug.Log("ERROR: ShutterGameObject is null");
		}
	}

	private void Start()
	{
		startPosition = shutterGameObject.transform.position;
		endPosition = shutterGameObject.transform.position;
		endPosition += new Vector3(0f, 2.42f, 0f);
		blockerStartPosition = blockerGameObject.transform.localPosition;
		blockerEndPosition = blockerGameObject.transform.localPosition;
		blockerEndPosition += new Vector3(0f, 241f, 0f);
		int siblingIndex = EmporiumEntry.Instance.frontInvWindow.handler.gameObject.transform.GetSiblingIndex();
		Instance.blockerGameObject.transform.parent.transform.SetParent(EmporiumEntry.Instance.frontInvWindow.handler.gameObject.transform.parent, worldPositionStays: false);
		blockerMaskObject.rectTransform.anchoredPosition = new Vector2(480f, blockerMaskObject.rectTransform.anchoredPosition.y);
		Instance.blockerGameObject.transform.parent.transform.SetSiblingIndex(siblingIndex + 1);
		SetButtonSprite(spriteNormal);
	}

	private void Update()
	{
		LerpShutter(shutterGameObject);
	}

	public void LerpShutter(GameObject targetObject)
	{
		if (!isClosed && (shutterGameObject.transform.position != endPosition || blockerGameObject.transform.localPosition != blockerEndPosition))
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			shutterGameObject.transform.position = Vector3.Lerp(startPosition, endPosition, t);
			blockerGameObject.transform.localPosition = Vector3.Lerp(blockerStartPosition, blockerEndPosition, t);
		}
		else if (isClosed && (shutterGameObject.transform.position != startPosition || blockerGameObject.transform.localPosition != blockerStartPosition))
		{
			elapsedTime += Time.deltaTime;
			float t2 = elapsedTime / duration;
			shutterGameObject.transform.position = Vector3.Lerp(endPosition, startPosition, t2);
			blockerGameObject.transform.localPosition = Vector3.Lerp(blockerEndPosition, blockerStartPosition, t2);
		}
	}

	public void ForceImmediateClose()
	{
		isClosed = true;
		elapsedTime = 0f;
		shutterGameObject.transform.position = startPosition;
		blockerGameObject.transform.localPosition = blockerStartPosition;
		if (audioSource != null)
		{
			audioSource.Stop();
		}
		if (audioSourceShutterClose != null)
		{
			audioSourceShutterClose.Stop();
		}
		if (audioSourceShutterClosed != null)
		{
			audioSourceShutterClosed.Stop();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isHoveringButton = true;
		SetButtonSprite(spriteHover);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHoveringButton = false;
		SetButtonSprite(spriteNormal);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		SetButtonSprite(spriteClick);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		SetButtonSprite(isHoveringButton ? spriteHover : spriteNormal);
	}

	private void SetButtonSprite(Sprite sprite)
	{
		if (buttonRenderer != null && sprite != null)
		{
			buttonRenderer.sprite = sprite;
		}
	}

	public override string GetTooltip()
	{
		if (PlayerStore.Instance.storeState == PlayerStore.StoreState.OPEN)
		{
			if (PlayerStore.Instance.lockShutterButtonFlag > 0 || PlayerStore.Instance.isClientBeingArrested || PlayerStore.Instance.isHandlingShootout)
			{
				return LocHelper.GetLocalizedUI("inte_shutter_please_wait");
			}
			if (PlayerStore.Instance.CanEndDay())
			{
				return LocHelper.GetLocalizedUI("inte_shutter_dismiss");
			}
			return LocHelper.GetLocalizedUI("inte_shutter_cant");
		}
		if (PlayerStore.Instance.storeState == PlayerStore.StoreState.MORNING)
		{
			if (PlayerStore.Instance.lockShutterButtonFlag > 0)
			{
				return LocHelper.GetLocalizedUI("inte_shutter_please_wait");
			}
			return LocHelper.GetLocalizedUI("inte_shutter_start");
		}
		return LocHelper.GetLocalizedUI("inte_shutter_ended");
	}

	public override bool IsTooltipEnabled()
	{
		if (PlayerStore.Instance.cinematicMode)
		{
			return false;
		}
		return true;
	}
}

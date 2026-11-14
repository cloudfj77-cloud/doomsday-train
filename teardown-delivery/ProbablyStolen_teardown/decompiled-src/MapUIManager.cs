using System;
using TMPro;
using UnityEngine;

public class MapUIManager : MonoBehaviour
{
	public const string PANEL_WINDOW_ID = "afterhour_location";

	[Tooltip("UI3/Prefab/TransitSystem.prefab, from Build > Generate Transit UI Prefab.")]
	public GameObject transitPrefab;

	public GameObject mapPanel;

	public GameObject confirmationPanel;

	[Header("The vanilla scenes and panels (the location classes return these as their StageObject and PanelObject)")]
	public GameObject commissaryScene;

	public GameObject upgradeStoreScene;

	public GameObject scavengingScene;

	public GameObject UICommissaryPanel;

	public GameObject UIUpgradeMerchantPanel;

	public GameObject UIScavengingPanel;

	public TextMeshProUGUI scavTimeRemainingTMP;

	public TextMeshProUGUI minorWoundChanceTMP;

	public TextMeshProUGUI majorWoundChanceTMP;

	[Tooltip("Where the location panel sits on the meta canvas (anchored position of the window).")]
	public Vector2 panelPosition = new Vector2(620f, 260f);

	private bool inTransit;

	private bool bagShown;

	private SpriteRenderer stage;

	private GameObject activeStageObject;

	private CustomUIWindow locationPanel;

	private GameObject activePanelObject;

	public static MapUIManager Instance { get; private set; }

	public AfterhourLocation Current { get; private set; }

	public bool IsOutside => Current != null;

	public string LastVisitedTonightId { get; private set; } = "";


	public bool isOutside => Current != null;

	public bool IsInTransit => inTransit;

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

	private void Update()
	{
		if (StoreUIManager.Instance.storeCreditDisplay.creditDisplayText.IsActive())
		{
			StoreUIManager.Instance.storeCreditDisplay.creditDisplayText.text = PlayerStore.Instance.GetCash() + GeneralHelper.CreditIcon();
		}
		if (Current != null)
		{
			try
			{
				Current.Tick();
			}
			catch (Exception arg)
			{
				Debug.LogError($"[MapUIManager] '{Current.Id}' Tick threw: {arg}");
			}
		}
	}

	public void OpenUI()
	{
		AudioManager.Instance.Play2("close_door");
		StoreUIManager.Instance.StartBlackscreenSequence(playFullSequence: true, delegate
		{
			LastVisitedTonightId = "";
			TransitUI.Open(transitPrefab);
			if (TransitUI.Root != null)
			{
				mapPanel = TransitUI.Root;
			}
			EmporiumEntry.Instance.OnLeaveStore();
			StoreShutterButton.Instance.ForceImmediateClose();
		});
	}

	public void CloseUI(bool restoreStoreUI = true)
	{
		TransitUI.Close();
		if (mapPanel != null && mapPanel != TransitUI.Root)
		{
			mapPanel.SetActive(value: false);
		}
		EmporiumEntry.Instance.OnArriveStore(fromMap: true);
		if (restoreStoreUI)
		{
			EmporiumEntry.Instance.RestoreStoreUIAfterReturn();
		}
	}

	public void Sleep()
	{
		CloseUI();
		PaperUIManager.Instance.CloseAllPaperUIForSleep();
		EndOfDayUIManager.Instance.OpenUI();
		if (TutorialUIManager.Instance != null)
		{
			TutorialUIManager.Instance.OnSleep();
		}
	}

	public void OpenGoOutsideConfirm()
	{
		confirmationPanel.SetActive(value: true);
	}

	public void OnGoOutsideConfirm()
	{
		confirmationPanel.SetActive(value: false);
		StoreResponse.ShowAfterhourResponse();
	}

	public bool IsShowingGoOutsideConfirm()
	{
		return EmporiumEntry.Instance.backInvinvElement.childItems.Count > 0;
	}

	public bool IsAfterhourPanel(GameObject panel)
	{
		if (panel == null)
		{
			return false;
		}
		if (mapPanel != null && panel.transform.IsChildOf(mapPanel.transform))
		{
			return true;
		}
		if (locationPanel != null && locationPanel.IsAlive && panel.transform.IsChildOf(locationPanel.root.transform))
		{
			return true;
		}
		if (activePanelObject != null && panel.transform.IsChildOf(activePanelObject.transform))
		{
			return true;
		}
		return false;
	}

	public void Travel(AfterhourLocation location)
	{
		if (location == null || inTransit || !location.IsAvailable())
		{
			return;
		}
		inTransit = true;
		StoreUIManager.Instance.StartBlackscreenSequence(playFullSequence: true, delegate
		{
			ModHook.FireOnAfterhourArriveEarly(location.Id);
			OnGoingOutside(location);
			Current = location;
			LastVisitedTonightId = location.Id;
			ShowStage(location);
			try
			{
				location.Arrive();
			}
			catch (Exception arg)
			{
				Debug.LogError($"[MapUIManager] '{location.Id}' Arrive threw: {arg}");
			}
			OpenLocationPanel(location);
			AfterhourLocationState afterhourState = PlayerStore.Instance.GetAfterhourState(location.Id);
			afterhourState.visits++;
			afterhourState.lastVisitDay = StoreStation.Instance.dayCounter;
			EmporiumEntry.Instance.Validate();
			if (!string.IsNullOrEmpty(location.AmbienceId))
			{
				StoreSoundPlayer.Instance.PlayAmbient(location.AmbienceId);
			}
			ModHook.FireOnAfterhourArriveLate(location.Id);
		}, delegate
		{
			if (StoreClientData.IsButcherHunting())
			{
				StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("mech_afterhour_butcher_watching"));
			}
		});
	}

	public void Return()
	{
		if (Current == null)
		{
			return;
		}
		if (EmporiumEntry.Instance.afterhourInventory.childItems.Count > 0)
		{
			string bodyText = null;
			try
			{
				bodyText = Current.GetLeaveWarning();
			}
			catch (Exception arg)
			{
				Debug.LogError($"[MapUIManager] '{Current.Id}' GetLeaveWarning threw: {arg}");
			}
			CheckItemLeftOnGround(ReturnNow, bodyText);
		}
		else
		{
			ReturnNow();
		}
	}

	public void ReturnNow()
	{
		if (Current == null || inTransit)
		{
			return;
		}
		inTransit = true;
		AfterhourLocation location = Current;
		StoreUIManager.Instance.StartBlackscreenSequence(playFullSequence: true, delegate
		{
			ModHook.FireOnAfterhourLeaveEarly(location.Id);
			CloseLocationPanel();
			try
			{
				location.Leave();
			}
			catch (Exception arg)
			{
				Debug.LogError($"[MapUIManager] '{location.Id}' Leave threw: {arg}");
			}
			HideStage();
			OnReturning();
			ModHook.FireOnAfterhourLeaveLate(location.Id);
		});
	}

	private void OnGoingOutside(AfterhourLocation location)
	{
		CloseUI(restoreStoreUI: false);
		PaperUIManager.Instance.CloseAllPaperUI();
		EmporiumEntry.Instance.CloseItemWindowUIWindow();
		EmporiumEntry.Instance.OnLeaveStore();
		inTransit = false;
		bagShown = location.UsesAfterhourBag;
		if (bagShown)
		{
			EmporiumEntry.Instance.ShowAfterhourInv();
			EmporiumEntry.Instance.TransferBackpackToInv();
		}
		PlayerStore.Instance.healthData.wentToAfterhoursLastNight = true;
		PlayerStore.Instance.lastAfterhourOutingDay = StoreStation.Instance.dayCounter;
		StoreShutterButton.Instance.blockerGameObject.transform.parent.gameObject.SetActive(value: false);
	}

	private void OnReturning()
	{
		AfterhourLocation current = Current;
		Current = null;
		EmporiumEntry.Instance.OnArriveStore();
		PaperUIManager.Instance.CloseAllPaperUI();
		EmporiumEntry.Instance.CloseItemWindowUIWindow();
		AudioManager.Instance.Play2("close_door");
		inTransit = false;
		if (bagShown)
		{
			EmporiumEntry.Instance.HideAfterhourInv();
			EmporiumEntry.Instance.TransferAfterhourToInv(current?.CountsDumpedWaste ?? false);
		}
		bagShown = false;
		StoreShutterButton.Instance.blockerGameObject.transform.parent.gameObject.SetActive(value: true);
		StoreSoundPlayer.Instance.PlayStoreAmbient();
		EmporiumEntry.Instance.RestoreStoreUIAfterReturn();
		if (TutorialUIManager.Instance != null)
		{
			TutorialUIManager.Instance.OnReturnFromAfterhours();
		}
	}

	private void ShowStage(AfterhourLocation location)
	{
		HideStage();
		GameObject stageObject = location.StageObject;
		if (stageObject != null)
		{
			activeStageObject = stageObject;
			stageObject.SetActive(value: true);
			return;
		}
		if (stage == null)
		{
			GameObject gameObject = new GameObject("AfterhourStage");
			gameObject.transform.position = Vector3.zero;
			stage = gameObject.AddComponent<SpriteRenderer>();
		}
		stage.sprite = ResolveBackground(location.BackgroundKey);
		stage.sortingOrder = location.BackgroundSortingOrder;
		stage.gameObject.SetActive(stage.sprite != null);
		if (stage.sprite == null)
		{
			Debug.LogWarning("[MapUIManager] No background sprite for '" + location.BackgroundKey + "' (location '" + location.Id + "')");
		}
	}

	private void HideStage()
	{
		if (activeStageObject != null)
		{
			activeStageObject.SetActive(value: false);
		}
		activeStageObject = null;
		if (stage != null)
		{
			stage.gameObject.SetActive(value: false);
		}
	}

	private Sprite ResolveBackground(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		if (!(CustomUIManager.Instance != null))
		{
			return null;
		}
		return CustomUIManager.Instance.GetSprite(key);
	}

	private void OpenLocationPanel(AfterhourLocation location)
	{
		CloseLocationPanel();
		GameObject panelObject = location.PanelObject;
		if (panelObject != null)
		{
			activePanelObject = panelObject;
			panelObject.SetActive(value: true);
		}
		else if (!(CustomUIManager.Instance == null))
		{
			string title;
			try
			{
				title = location.GetPanelTitle();
			}
			catch (Exception arg)
			{
				Debug.LogError($"[MapUIManager] '{location.Id}' GetPanelTitle threw: {arg}");
				title = location.Id;
			}
			CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("afterhour_location", title, "meta");
			try
			{
				location.BuildPanel(customUIBuilder);
			}
			catch (Exception arg2)
			{
				Debug.LogError($"[MapUIManager] '{location.Id}' BuildPanel threw: {arg2}");
			}
			customUIBuilder.AddSpace(8f);
			customUIBuilder.AddButton(LocHelper.GetLocalizedUI("afterhour_return_action"), Return);
			customUIBuilder.SetPosition(panelPosition);
			locationPanel = customUIBuilder.Show();
		}
	}

	private void CloseLocationPanel()
	{
		if (activePanelObject != null)
		{
			activePanelObject.SetActive(value: false);
		}
		activePanelObject = null;
		if (locationPanel != null && locationPanel.IsAlive)
		{
			locationPanel.Close();
		}
		locationPanel = null;
	}

	public void LeaveCommissary()
	{
		Return();
	}

	public void LeaveUpgradeMerchant()
	{
		Return();
	}

	public void LeaveScavenging()
	{
		Return();
	}

	public void CommissaryTradeButton()
	{
		CommissaryUIManager.Instance.OpenUI();
	}

	public void ScavengeButtonClick()
	{
		ScavHelper.ScavengeDumpingGrounds();
		UpdateScavUI();
	}

	public void UpdateScavUI()
	{
		if (!(scavTimeRemainingTMP == null))
		{
			string text = (PlayerStore.Instance.healthData.isWoundedFresh ? LocHelper.GetLocalizedMechanic("afterhour_attempts_wounded") : LocHelper.GetLocalizedMechanic("afterhour_attempts_remaining", ScavHelper.GetScavTimeLeft()));
			string localizedMechanic = LocHelper.GetLocalizedMechanic("afterhour_minor_wound_chance", ScavHelper.GetMinorWoundChance().ToString("F2"));
			string localizedMechanic2 = LocHelper.GetLocalizedMechanic("afterhour_major_wound_chance", ScavHelper.GetMajorWoundChance().ToString("F2"));
			if (scavTimeRemainingTMP.text != text)
			{
				scavTimeRemainingTMP.text = text;
			}
			if (minorWoundChanceTMP.text != localizedMechanic)
			{
				minorWoundChanceTMP.text = localizedMechanic;
			}
			if (majorWoundChanceTMP.text != localizedMechanic2)
			{
				majorWoundChanceTMP.text = localizedMechanic2;
			}
		}
	}

	public void ActivateClientOutsideStore()
	{
		PlayerStore.Instance.currentClientInstance = StoreClientInstance.CreateClientInstance(StoreClientList.CreateInventor());
		StoreClientMono.Instance.image.sprite = SpriteDict.Instance.ShowClientSprite(PlayerStore.Instance.currentClientInstance.GetClientBlueprint().GetSprite());
		StoreUIManager.Instance.HideClient();
		PlayerStore.Instance.isClientArrived = true;
	}

	public void DeactivateClientOutsideStore()
	{
		PlayerStore.Instance.currentClientInstance = null;
		NegociationUIManager.Instance.currentNegociatedItem = null;
		PlayerStore.Instance.isClientArrived = false;
		StoreUIManager.Instance.ResetClientPosition();
	}

	public void TransferBackInvToCounter()
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			EmporiumEntry.Instance.backInvinvElement.Expel(childItem);
			EmporiumEntry.Instance.backInvinvElementCounter.UncheckedAccept(childItem);
		}
	}

	public void TransferCounterToBackInv()
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.backInvinvElementCounter.childItems)
		{
			EmporiumEntry.Instance.backInvinvElementCounter.Expel(childItem);
			EmporiumEntry.Instance.backInvinvElement.UncheckedAccept(childItem);
		}
	}

	public bool CheckItemLeftOnGround(Action action = null, string bodyText = null)
	{
		if (EmporiumEntry.Instance.afterhourInventory.childItems.Count > 0)
		{
			string bodyText2 = bodyText ?? LocHelper.GetLocalizedMechanic("afterhour_items_lost_warning");
			GenericConfirmationUIManager.Instance.OpenPanel(bodyText2, LocHelper.GetLocalizedMechanic("afterhour_confirm_leave"), LocHelper.GetLocalizedMechanic("afterhour_confirm_cancel"), delegate
			{
				GenericConfirmationUIManager.Instance.ClosePanel();
				action?.Invoke();
			}, delegate
			{
				GenericConfirmationUIManager.Instance.ClosePanel();
			});
			return true;
		}
		return false;
	}

	public void SetCreditDisplayPositionToDrJackson()
	{
		StoreUIManager.Instance.storeCreditDisplay.creditDisplayText.rectTransform.anchoredPosition = new Vector2(-525.9f, 215.9f);
		ColorUtility.TryParseHtmlString("#CC7755", out var color);
		StoreUIManager.Instance.storeCreditDisplay.creditDisplayText.color = color;
	}

	public void ResetCreditDisplayPosition()
	{
		StoreUIManager.Instance.storeCreditDisplay.creditDisplayText.rectTransform.anchoredPosition = StoreUIManager.Instance.storeCreditDisplay.DefaultAnchoredPosition;
		StoreUIManager.Instance.storeCreditDisplay.creditDisplayText.color = StoreUIManager.Instance.storeCreditDisplay.DefaultColor;
	}
}

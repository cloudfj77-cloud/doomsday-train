using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NegociationUIManager : MonoBehaviour
{
	private enum Mode
	{
		None,
		Buying,
		Selling,
		Barter,
		InfoDealer,
		GunOrder
	}

	private class CachedRow
	{
		public TabletListRow row;

		public string key;

		public bool used;
	}

	public const string WINDOW_ID = "negotiation_tablet";

	public const string STORE_UI_IDENTIFIER = "negociation_ui";

	[Tooltip("UI3/Prefab/NegotiationTablet.prefab, from Build > Generate Tablet UI Prefabs.")]
	public GameObject tabletPrefab;

	public GameObject uiObject;

	public AudioSource audioSourceCashRegister;

	public GameItem currentNegociatedItem;

	private CustomUIWindow window;

	private NegotiationTabletFrame frame;

	private StoreUI storeUI;

	private Mode mode;

	private GameItem selectedItem;

	private GameItem autoSelectedItem;

	private GameItem autoDeclinedItem;

	private string builtSignature;

	private TabletButton primaryButton;

	private TabletButton secondaryButton;

	private Button selectorPrev;

	private Button selectorNext;

	private readonly List<GameItem> listedItems = new List<GameItem>();

	private float refreshTimer;

	private const float RefreshInterval = 0.2f;

	private bool refreshQueued;

	private readonly Dictionary<GameItem, CachedRow> cachedRows = new Dictionary<GameItem, CachedRow>();

	public static NegociationUIManager Instance { get; private set; }

	public bool IsOpen
	{
		get
		{
			if (uiObject != null)
			{
				return uiObject.activeSelf;
			}
			return false;
		}
	}

	public float CurrentZoomScale
	{
		get
		{
			if (!(uiObject != null))
			{
				return 1f;
			}
			return uiObject.transform.localScale.x;
		}
	}

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

	private void Start()
	{
		CreateWindow();
	}

	private void Update()
	{
		if (!(uiObject == null) && uiObject.activeSelf)
		{
			refreshTimer += Time.deltaTime;
			if (refreshTimer >= 0.2f)
			{
				refreshTimer = 0f;
				RefreshUI();
			}
		}
	}

	private void LateUpdate()
	{
		if (refreshQueued)
		{
			refreshQueued = false;
			RefreshNow();
		}
	}

	private void CreateWindow()
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[NegociationUIManager] No CustomUIManager in the scene, the negotiation tablet cannot be created");
			return;
		}
		if (tabletPrefab == null)
		{
			Debug.LogError("[NegociationUIManager] tabletPrefab is not assigned. Run Build > Generate Tablet UI Prefabs in the store scene.");
			return;
		}
		window = CustomUIManager.Instance.CreateWindow("negotiation_tablet", null, "paper", tabletPrefab).SetDraggable(draggable: true).GetWindow();
		frame = ((window.root != null) ? window.root.GetComponent<NegotiationTabletFrame>() : null);
		cachedRows.Clear();
		if (frame == null)
		{
			Debug.LogError("[NegociationUIManager] tabletPrefab has no NegotiationTabletFrame on its root; regenerate it");
			window.Close();
			window = null;
			return;
		}
		uiObject = window.root;
		uiObject.name = "NegotiationTablet";
		storeUI = uiObject.GetComponent<StoreUI>();
		if (storeUI == null)
		{
			storeUI = uiObject.AddComponent<StoreUI>();
		}
		storeUI.isPaperUI = true;
		storeUI.identifier = "negociation_ui";
		storeUI.zoomable = true;
		storeUI.zoomRoot = uiObject.transform;
		storeUI.zoomScaleNormal = 1f;
		storeUI.zoomScaleAlt = 0.5f;
		storeUI.zoomScaleAlt2 = 0.75f;
		frame.closeButton.onClick.AddListener(CloseUI);
		frame.resizeButton.onClick.AddListener(OnResizeClick);
		frame.bargainButton.onClick.AddListener(OnBargainClick);
		TabletUI.NoKeyboardFocus(frame.closeButton, frame.resizeButton, frame.bargainButton);
		frame.confirmRoot.SetActive(value: false);
		uiObject.SetActive(value: false);
	}

	private void HideConfirmPanel()
	{
		if (frame != null && frame.confirmRoot != null)
		{
			frame.confirmRoot.SetActive(value: false);
		}
	}

	public void CloseUI()
	{
		if (!(uiObject == null))
		{
			HideConfirmPanel();
			if (uiObject.activeSelf)
			{
				uiObject.SetActive(value: false);
				BargainUIManager.Instance.CloseUI();
				EmporiumAudioPlayer.Instance.PlayUnMatched();
			}
		}
	}

	public void OpenUI()
	{
		if (!(uiObject == null))
		{
			HideConfirmPanel();
			bool activeSelf = uiObject.activeSelf;
			uiObject.SetActive(value: true);
			PaperPanelFocusStamp.BumpPanel(uiObject);
			if (activeSelf)
			{
				RefreshUI();
			}
			else
			{
				RefreshNow();
			}
			if (!activeSelf)
			{
				EmporiumAudioPlayer.Instance.PlayMatched();
			}
		}
	}

	public void ToggleUI()
	{
		if (!(uiObject == null))
		{
			if (uiObject.activeSelf)
			{
				CloseUI();
			}
			else
			{
				OpenUI();
			}
		}
	}

	private void OnResizeClick()
	{
		if (storeUI != null)
		{
			storeUI.ToggleZoom();
		}
		EmporiumAudioPlayer.Instance.audioButton1.Play();
	}

	private void OnBargainClick()
	{
		BargainUIManager.Instance.ToggleUI();
		RefreshBargainControl();
	}

	private static StoreClient Client()
	{
		if (PlayerStore.Instance == null || PlayerStore.Instance.currentClientInstance == null)
		{
			return null;
		}
		return PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
	}

	public bool IsNegociatingItemOwned()
	{
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count != 0)
		{
			return GeneralHelper.IsItemOwned(PlayerStore.Instance.currentNegociatedItem);
		}
		return false;
	}

	private static bool OnBackCounter(GameItem item)
	{
		if (item != null)
		{
			return item.parentInventory == EmporiumEntry.Instance.backInvinvElement;
		}
		return false;
	}

	private Mode ResolveMode(StoreClient storeClient)
	{
		if (storeClient == null || storeClient.isBusinessComplete)
		{
			return Mode.None;
		}
		switch (storeClient.clientIntent)
		{
		case StoreClient.ClientIntent.BUY:
			return Mode.Buying;
		case StoreClient.ClientIntent.SELL:
			return Mode.Selling;
		case StoreClient.ClientIntent.SELLNBUY:
		{
			bool flag = storeClient.isOfferWholesale && EmporiumEntry.Instance.GetAllNonOwendItemCount() != 0;
			if (OnBackCounter(currentNegociatedItem))
			{
				if (!GeneralHelper.IsItemOwned(currentNegociatedItem))
				{
					return Mode.Selling;
				}
				if (!flag || storeClient.IsClientBuyingThisItem(currentNegociatedItem))
				{
					return Mode.Buying;
				}
			}
			List<GameItem> ownedBackInvItems = GetOwnedBackInvItems();
			if (ownedBackInvItems.Count > 0 && (!flag || ownedBackInvItems.Any(storeClient.IsClientBuyingThisItem)))
			{
				return Mode.Buying;
			}
			if (GetNonOwnedBackInvItems().Count > 0)
			{
				return Mode.Selling;
			}
			if (flag)
			{
				return Mode.Selling;
			}
			return Mode.Buying;
		}
		case StoreClient.ClientIntent.BARTER:
			return Mode.Barter;
		case StoreClient.ClientIntent.INFORMATION_DEALER:
			return Mode.InfoDealer;
		case StoreClient.ClientIntent.GUNSMITH:
			return Mode.GunOrder;
		default:
			return Mode.None;
		}
	}

	private static string SectionTitle(StoreClient storeClient)
	{
		if (storeClient == null)
		{
			return "";
		}
		return storeClient.clientIntent switch
		{
			StoreClient.ClientIntent.BUY => LocHelper.GetLocalizedUI("ui_nego_title_buying"), 
			StoreClient.ClientIntent.SELL => LocHelper.GetLocalizedUI("ui_nego_title_selling"), 
			StoreClient.ClientIntent.SELLNBUY => LocHelper.GetLocalizedUI("ui_nego_title_buy_sell"), 
			StoreClient.ClientIntent.BARTER => LocHelper.GetLocalizedUI("ui_nego_title_barter"), 
			StoreClient.ClientIntent.INFORMATION_DEALER => LocHelper.GetLocalizedUI("ui_nego_title_info_dealer"), 
			StoreClient.ClientIntent.GUNSMITH => LocHelper.GetLocalizedUI("ui_nego_title_gunsmith"), 
			_ => "", 
		};
	}

	public List<GameItem> GetSellableItem()
	{
		StoreClient storeClient = Client();
		if (storeClient == null)
		{
			return new List<GameItem>();
		}
		return GetSellableBackInvItems(storeClient);
	}

	private static List<GameItem> GetOwnedBackInvItems()
	{
		return (from item in EmporiumEntry.Instance.GetBackInvItems()
			where GeneralHelper.IsItemOwned(item)
			select item).ToList();
	}

	private static List<GameItem> GetNonOwnedBackInvItems()
	{
		return (from item in EmporiumEntry.Instance.GetBackInvItems()
			where !GeneralHelper.IsItemOwned(item)
			select item).ToList();
	}

	private static List<GameItem> GetSellableBackInvItems(StoreClient storeClient)
	{
		return (from item in EmporiumEntry.Instance.GetBackInvItems()
			where GeneralHelper.IsItemOwned(item) && storeClient.IsClientBuyingThisItem(item)
			select item).ToList();
	}

	public string GetNotBuyingReason(GameItem gameItem)
	{
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count == 0)
		{
			return "";
		}
		StoreClient storeClient = Client();
		if (storeClient == null)
		{
			return "";
		}
		if (!storeClient.IsClientBuyingThisItem(gameItem))
		{
			return LocHelper.GetLocalizedUI("ui_nego_not_interested");
		}
		if (storeClient.useClientBudget && !storeClient.HasBudgetLeftToBuy(gameItem))
		{
			return LocHelper.GetLocalizedUI("ui_nego_no_budget");
		}
		if (storeClient.isRequireVariety && storeClient.IsItemInVarietyList(gameItem))
		{
			return LocHelper.GetLocalizedUI("ui_nego_no_variety");
		}
		return "";
	}

	private static string GetRefusalLabel(StoreClient storeClient, GameItem gameItem)
	{
		if (!storeClient.IsClientBuyingThisItem(gameItem))
		{
			return LocHelper.GetLocalizedUI("ui_nego_row_not_interested");
		}
		return "";
	}

	public int GetFinalOfferValue(GameItem gameItem)
	{
		return (int)gameItem.GetNegociatedValue();
	}

	private static int SumNegociatedValue(List<GameItem> items)
	{
		int num = 0;
		foreach (GameItem item in items)
		{
			num += (int)item.GetNegociatedValue();
		}
		return num;
	}

	public int GetMultiSellValue()
	{
		StoreClient storeClient = Client();
		if (storeClient == null)
		{
			return 0;
		}
		return SumNegociatedValue(GetSellableBackInvItems(storeClient));
	}

	public bool IsClientHaveEnoughToBuy()
	{
		StoreClient storeClient = Client();
		if (storeClient == null)
		{
			return false;
		}
		if (!storeClient.useClientBudget)
		{
			return true;
		}
		return GetMultiSellValue() <= storeClient.GetBudget();
	}

	public static bool IsTradeLocked()
	{
		if (PlayerStore.Instance != null && (PlayerStore.Instance.isClientBeingArrested || PlayerStore.Instance.isHandlingShootout))
		{
			return true;
		}
		if (StoreUIManager.Instance != null && StoreUIManager.Instance.isShot)
		{
			return true;
		}
		return false;
	}

	public void UpdateUI(GameItem gameItem)
	{
		if (gameItem != null && OnBackCounter(gameItem))
		{
			currentNegociatedItem = gameItem;
		}
		RefreshUI();
	}

	public void UpdateUI()
	{
		RefreshUI();
	}

	public void ClearUI()
	{
		selectedItem = null;
		RefreshUI();
	}

	public void RefreshUI()
	{
		if (!(frame == null) && !(uiObject == null) && uiObject.activeSelf)
		{
			refreshQueued = true;
		}
	}

	private void RefreshNow()
	{
		refreshQueued = false;
		if (!(frame == null) && !(uiObject == null) && uiObject.activeSelf)
		{
			if (currentNegociatedItem != null && !OnBackCounter(currentNegociatedItem))
			{
				currentNegociatedItem = null;
			}
			if (selectedItem != null && !OnBackCounter(selectedItem))
			{
				selectedItem = null;
			}
			StoreClient storeClient = Client();
			mode = ResolveMode(storeClient);
			AutoSelectLoneItem();
			RefreshClientHeader(storeClient);
			frame.sectionHeader.text = SectionTitle(storeClient);
			RefreshBargainControl();
			string text = BuildSignature(storeClient);
			if (text != builtSignature)
			{
				builtSignature = text;
				Rebuild(storeClient);
			}
		}
	}

	private void RefreshClientHeader(StoreClient storeClient)
	{
		if (storeClient == null)
		{
			frame.clientName.text = "";
			frame.clientSubtitle.text = "";
			frame.clientSubtitle.gameObject.SetActive(value: false);
			return;
		}
		frame.clientName.text = storeClient.displayName;
		bool flag = !string.IsNullOrEmpty(storeClient.attractedItemId) || !string.IsNullOrEmpty(storeClient.attractedType);
		if (flag)
		{
			string text = ((!string.IsNullOrEmpty(storeClient.attractedItemId)) ? GeneralHelper.GetDisplayNameFromID(storeClient.attractedItemId) : TypeHelper.GetTypeDisplayName(storeClient.attractedType));
			frame.clientSubtitle.text = LocHelper.GetLocalizedUI("nego_ui_attracted_by") + " " + LocHelper.GetLocalizedUI("nego_ui_attracted_reason", text);
		}
		frame.clientSubtitle.gameObject.SetActive(flag);
		WildNetworkUI.SetRect(frame.clientName.rectTransform, 48f, 70f + (flag ? 0f : 13f), 530f, 26f);
	}

	private void RefreshBargainControl()
	{
		StoreClient storeClient = Client();
		bool flag = storeClient != null && storeClient.isBargainAvailable && !MapUIManager.Instance.isOutside && PlayerStore.Instance.isBargainIntroduced && !StartingPerk.IsAwfulNegotiator();
		if (!flag)
		{
			BargainUIManager.Instance.CloseUI();
		}
		frame.bargainButton.gameObject.SetActive(flag);
		bool flag2 = BargainUIManager.Instance != null && BargainUIManager.Instance.IsOpen;
		if (frame.artBargainOpen != null && frame.artNormal != null)
		{
			frame.art.sprite = (flag2 ? frame.artBargainOpen : frame.artNormal);
		}
		frame.bargainImage.sprite = (flag2 ? frame.bargainActive : frame.bargainNormal);
		SpriteState spriteState = frame.bargainButton.spriteState;
		spriteState.highlightedSprite = (flag2 ? frame.bargainActive : frame.bargainHover);
		spriteState.pressedSprite = (flag2 ? frame.bargainActive : frame.bargainPressed);
		frame.bargainButton.spriteState = spriteState;
		RectTransform rectTransform = frame.bargainButton.transform as RectTransform;
		Canvas canvas = frame.bargainImage.canvas;
		Camera cam = ((canvas != null && canvas.renderMode != 0) ? canvas.worldCamera : null);
		bool flag3 = rectTransform != null && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, cam);
		frame.bargainImage.overrideSprite = (flag2 ? frame.bargainActive : (flag3 ? frame.bargainHover : null));
	}

	private string BuildSignature(StoreClient storeClient)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(mode).Append('|').Append(storeClient?.GetHashCode() ?? 0)
			.Append('|');
		stringBuilder.Append(IsTradeLocked() ? 'L' : 'u').Append('|');
		stringBuilder.Append(PlayerStore.Instance.playerCash).Append('|');
		stringBuilder.Append((selectedItem != null) ? selectedItem.GetHashCode() : 0).Append('|');
		if (storeClient == null)
		{
			return stringBuilder.ToString();
		}
		stringBuilder.Append(storeClient.isIntroduced ? 'i' : 'n').Append(storeClient.useClientBudget ? storeClient.GetBudget() : (-1)).Append('|');
		stringBuilder.Append(storeClient.isOfferWholesale ? PlayerStore.Instance.GetWholesalePrice() : (-1)).Append('|');
		stringBuilder.Append(storeClient.isRequireVariety).Append(storeClient.noAcceptingContraband).Append(storeClient.isSecurity)
			.Append('|');
		if (storeClient.clientBuyingTagList != null)
		{
			stringBuilder.Append(string.Join(",", storeClient.clientBuyingTagList)).Append('|');
		}
		if (storeClient.clientBlackTagList != null)
		{
			stringBuilder.Append(string.Join(",", storeClient.clientBlackTagList)).Append('|');
		}
		stringBuilder.Append(storeClient.attractedItemId).Append(storeClient.attractedType).Append('|');
		Dialogue dialogue = ((DialogUIManager.Instance != null) ? DialogUIManager.Instance.GetCurrentDialog() : null);
		stringBuilder.Append((dialogue != null && dialogue.isMainDialog) ? 'M' : 'm').Append('|');
		switch (mode)
		{
		case Mode.Buying:
		case Mode.Selling:
		{
			bool flag = IsDonationClient(storeClient);
			foreach (GameItem backInvItem in EmporiumEntry.Instance.GetBackInvItems())
			{
				stringBuilder.Append(backInvItem.GetHashCode()).Append(':').Append(backInvItem.GetNegociatedValue())
					.Append(':')
					.Append(GeneralHelper.IsItemOwned(backInvItem) ? 'o' : 'c')
					.Append(':')
					.Append(GetRefusalLabel(storeClient, backInvItem))
					.Append(';');
				if (flag)
				{
					stringBuilder.Append(backInvItem.GetDonationCreditValue()).Append(';');
				}
				if (backInvItem != selectedItem)
				{
					continue;
				}
				foreach (ItemFeature itemFeature in backInvItem.itemFeatures)
				{
					stringBuilder.Append(itemFeature.IsExposed() ? 'E' : 'h').Append(itemFeature.isDisabled ? 'd' : 'a');
				}
				stringBuilder.Append(';');
			}
			break;
		}
		case Mode.Barter:
			if (storeClient.barterOffer != null)
			{
				stringBuilder.Append(storeClient.barterOffer.currentSetIndex).Append('/').Append(storeClient.barterOffer.OfferSetCount())
					.Append('|');
				stringBuilder.Append(storeClient.barterOffer.IsBarterAcceptable() ? 'A' : 'r').Append('|');
			}
			foreach (GameItem item in EmporiumEntry.Instance.GetAllNonOwnedItem())
			{
				stringBuilder.Append(item.GetHashCode()).Append(';');
			}
			break;
		case Mode.InfoDealer:
			stringBuilder.Append(storeClient.serviceCost);
			break;
		case Mode.GunOrder:
		{
			GameItem gameItem = GunOrderData.ResolveOrderGun();
			stringBuilder.Append(gameItem?.GetHashCode() ?? 0).Append(':').Append(gameItem?.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L) ?? 0)
				.Append('|');
			if (storeClient.gunOrderData == null)
			{
				break;
			}
			if (gameItem != null)
			{
				foreach (GunOrderRequirement mandatoryRequirement in storeClient.gunOrderData.mandatoryRequirements)
				{
					stringBuilder.Append(gameItem.GetTagReadonly(mandatoryRequirement.id).GetInt()).Append(',');
				}
				foreach (GunOrderRequirement bonusRequirement in storeClient.gunOrderData.bonusRequirements)
				{
					stringBuilder.Append(gameItem.GetTagReadonly(bonusRequirement.id).GetInt()).Append(',');
				}
				stringBuilder.Append('|');
			}
			stringBuilder.Append(storeClient.gunOrderData.GetTotalRewardDisplay());
			break;
		}
		}
		return stringBuilder.ToString();
	}

	private void Rebuild(StoreClient storeClient)
	{
		float verticalNormalizedPosition = frame.scroll.verticalNormalizedPosition;
		frame.infoHost.gameObject.SetActive(value: true);
		TabletUI.ClearChildren(frame.infoHost);
		ClearContentKeepingRows();
		TabletUI.ClearChildren(frame.footer);
		TabletUI.ClearChildren(frame.buttonHost);
		primaryButton = null;
		secondaryButton = null;
		selectorPrev = null;
		selectorNext = null;
		listedItems.Clear();
		bool locked = IsTradeLocked();
		float infoHeight = 0f;
		bool withSeparator = true;
		switch (mode)
		{
		case Mode.Buying:
			infoHeight = BuildBuying(storeClient, locked);
			break;
		case Mode.Selling:
			BuildSelling(storeClient, locked);
			break;
		case Mode.Barter:
			infoHeight = BuildBarter(storeClient, locked);
			break;
		case Mode.InfoDealer:
			BuildInfoDealer(storeClient, locked);
			withSeparator = false;
			break;
		case Mode.GunOrder:
			BuildGunOrder(storeClient, locked);
			withSeparator = false;
			break;
		default:
			if (storeClient != null && !storeClient.isBusinessComplete)
			{
				infoHeight = BuildInfoBlock(storeClient);
			}
			break;
		}
		DropUnusedRows();
		TabletUI.LayoutSection(frame, infoHeight, withSeparator, 432f);
		Canvas.ForceUpdateCanvases();
		frame.scroll.verticalNormalizedPosition = (float.IsNaN(verticalNormalizedPosition) ? 1f : verticalNormalizedPosition);
	}

	private float BuildInfoBlock(StoreClient storeClient)
	{
		float num = 0f;
		List<TabletUI.TagSpec> list = new List<TabletUI.TagSpec>();
		if (storeClient.isRequireVariety)
		{
			list.Add(new TabletUI.TagSpec(LocHelper.GetLocalizedUI("ui_nego_tag_variety"), TabletStyle.TAG_VARIETY_BG, TabletStyle.TAG_VARIETY_TEXT, frame.infoLight, () => LocHelper.GetLocalizedUI("ui_nego_no_variety")));
		}
		if (storeClient.noAcceptingContraband)
		{
			list.Add(new TabletUI.TagSpec(LocHelper.GetLocalizedUI("ui_nego_tag_no_contraband"), TabletStyle.BARGAIN_RED, TabletStyle.TAG_RED_TEXT, frame.infoTan, () => LocHelper.GetLocalizedUI("ui_nego_tag_no_contraband_info")));
		}
		if (storeClient.isSecurity)
		{
			list.Add(new TabletUI.TagSpec(LocHelper.GetLocalizedUI("ui_nego_tag_security"), TabletStyle.BARGAIN_RED, TabletStyle.TAG_RED_TEXT, frame.infoTan, () => LocHelper.GetLocalizedUI("ui_nego_tag_security_info")));
		}
		if (storeClient.IsClientBuying() && storeClient.clientBlackTagList != null)
		{
			foreach (string item in storeClient.clientBlackTagList.Where((string t) => TypeHelper.ALL_TYPES_LIST.Contains(t)))
			{
				list.Add(new TabletUI.TagSpec(TypeHelper.GetTypeDisplayName(item), TabletStyle.BARGAIN_RED, TabletStyle.TAG_RED_TEXT, frame.infoTan, () => LocHelper.GetLocalizedUI("ui_nego_not_interested")));
			}
		}
		if (list.Count > 0)
		{
			num += TabletUI.InfoRow(frame, frame.infoHost, num, LocHelper.GetLocalizedUI("ui_nego_restrictions"), list) + 8f;
		}
		if (storeClient.IsClientBuying() && storeClient.clientBuyingTagList != null)
		{
			List<TabletUI.TagSpec> list2 = new List<TabletUI.TagSpec>();
			foreach (string item2 in storeClient.clientBuyingTagList.Where((string t) => TypeHelper.ALL_TYPES_LIST.Contains(t)))
			{
				string typeConst = ((item2 == "ALL_TYPE_EXCEPT_CONTRABAND") ? "ALL_TYPE" : item2);
				list2.Add(new TabletUI.TagSpec(TypeHelper.GetTypeDisplayName(typeConst), TabletStyle.TAG_INTEREST_BG, TabletStyle.TAG_INTEREST_TEXT));
			}
			if (list2.Count > 0)
			{
				num += TabletUI.InfoRow(frame, frame.infoHost, num, LocHelper.GetLocalizedUI("ui_nego_interests"), list2) + 8f;
			}
		}
		if (!(num > 0f))
		{
			return 0f;
		}
		return num - 8f;
	}

	private float BuildBuying(StoreClient storeClient, bool locked)
	{
		float result = BuildInfoBlock(storeClient);
		List<GameItem> ownedBackInvItems = GetOwnedBackInvItems();
		List<GameItem> sellableBackInvItems = GetSellableBackInvItems(storeClient);
		foreach (GameItem item in ownedBackInvItems)
		{
			string refusalLabel = GetRefusalLabel(storeClient, item);
			BuildItemRow(item, refusalLabel, (refusalLabel.Length == 0) ? TabletUI.Credits(item.GetNegociatedValue()) : refusalLabel);
		}
		int num = SumNegociatedValue(sellableBackInvItems);
		float num2 = 0f;
		bool flag = storeClient.useClientBudget && num > storeClient.GetBudget();
		if (storeClient.useClientBudget)
		{
			TabletUI.FooterLine(frame, frame.footer, num2, LocHelper.GetLocalizedUI("ui_nego_client_budget"), TabletUI.Credits(storeClient.GetBudget()));
			num2 += 35f;
		}
		else
		{
			num2 = 17.5f;
		}
		TabletUI.FooterLine(frame, frame.footer, num2, LocHelper.GetLocalizedUI("ui_nego_total"), TabletUI.Credits(num), flag ? new Color?(TabletStyle.RED) : null);
		string label = (IsDonationClient(storeClient) ? LocHelper.GetLocalizedUIOrDefault("ui_nego_donate_items", "Donate ({0})", TabletUI.Credits(SumDonationCreditValue(sellableBackInvItems))) : LocHelper.GetLocalizedUI("ui_nego_sell_items", sellableBackInvItems.Count));
		primaryButton = TabletUI.BigButton(frame, frame.buttonHost, 153f, 0f, label, OnSellAllClick);
		primaryButton.SetInteractable(sellableBackInvItems.Count > 0 && !locked);
		return result;
	}

	private static bool IsDonationClient(StoreClient storeClient)
	{
		if (storeClient.clientItemFeatureBuying != null)
		{
			return storeClient.clientItemFeatureBuying.Contains("bribeDonation");
		}
		return false;
	}

	private static long SumDonationCreditValue(List<GameItem> items)
	{
		long num = 0L;
		foreach (GameItem item in items)
		{
			num += item.GetDonationCreditValue();
		}
		return num;
	}

	public void OnSellAllClick()
	{
		if (PlayerStore.Instance.currentClientInstance == null || IsTradeLocked())
		{
			return;
		}
		TutorialUIManager.Instance.BuyDealed();
		StoreClient storeClient = Client();
		if (!storeClient.isBusinessComplete)
		{
			if (IsClientHaveEnoughToBuy())
			{
				SellMultiple(GetSellableBackInvItems(storeClient));
			}
			else
			{
				OpenConfirmPanel();
			}
		}
	}

	private void BuildSelling(StoreClient storeClient, bool locked)
	{
		List<GameItem> nonOwnedBackInvItems = GetNonOwnedBackInvItems();
		bool flag = storeClient.isOfferWholesale && EmporiumEntry.Instance.GetAllNonOwendItemCount() != 0;
		if (flag)
		{
			TabletUI.SubHeader(frame, frame.content, LocHelper.GetLocalizedUI("ui_nego_wholesale_offer", Mathf.Abs(storeClient.wholesaleDiscount)), 38f, frame.infoCream, () => LocHelper.GetLocalizedUI("intel_mentor_note_wholesale_body"), TabletStyle.SCREEN);
		}
		foreach (GameItem item2 in nonOwnedBackInvItems)
		{
			BuildItemRow(item2, "", TabletUI.Credits(item2.GetNegociatedValue()));
		}
		List<GameItem> list = nonOwnedBackInvItems;
		if (storeClient.isMultiBuyDisabled && nonOwnedBackInvItems.Count > 1)
		{
			GameItem item = ((selectedItem != null && nonOwnedBackInvItems.Contains(selectedItem)) ? selectedItem : nonOwnedBackInvItems[0]);
			list = new List<GameItem> { item };
		}
		int num = SumNegociatedValue(list);
		int playerCash = PlayerStore.Instance.playerCash;
		int num2 = (flag ? PlayerStore.Instance.GetWholesalePrice() : 0);
		TabletUI.FooterLine(frame, frame.footer, 0f, LocHelper.GetLocalizedUI("ui_nego_your_funds"), TabletUI.Credits(playerCash));
		if (flag)
		{
			string value = TabletUI.Credits(num) + " / " + TabletUI.Credits(num2) + " <color=#" + ColorUtility.ToHtmlStringRGB(TabletStyle.GREEN) + ">(" + storeClient.wholesaleDiscount + "%)</color>";
			TabletUI.FooterLine(frame, frame.footer, 35f, LocHelper.GetLocalizedUI("ui_nego_total_wholesale"), value, (num > playerCash && num2 > playerCash) ? new Color?(TabletStyle.RED) : null);
		}
		else
		{
			TabletUI.FooterLine(frame, frame.footer, 35f, LocHelper.GetLocalizedUI("ui_nego_total"), TabletUI.Credits(num), (num > playerCash) ? new Color?(TabletStyle.RED) : null);
		}
		float x = (flag ? 51f : 185f) - 32f;
		primaryButton = TabletUI.BigButton(frame, frame.buttonHost, x, 0f, LocHelper.GetLocalizedUI("ui_nego_buy_items", list.Count), OnBuyAllClick);
		primaryButton.SetInteractable(list.Count > 0 && !storeClient.isRegularSellDisable && num <= playerCash && !locked);
		if (flag)
		{
			secondaryButton = TabletUI.BigButton(frame, frame.buttonHost, 287f, 0f, LocHelper.GetLocalizedUI("ui_nego_accept_wholesale"), OnWholesaleClick);
			secondaryButton.SetInteractable(playerCash >= num2 && !locked);
		}
	}

	public void OnBuyAllClick()
	{
		if (PlayerStore.Instance.currentClientInstance == null || IsTradeLocked())
		{
			return;
		}
		TutorialUIManager.Instance.BuyDealed();
		StoreClient storeClient = Client();
		if (!storeClient.isBusinessComplete && !storeClient.isRegularSellDisable)
		{
			List<GameItem> list = GetNonOwnedBackInvItems();
			if (storeClient.isMultiBuyDisabled && list.Count > 1)
			{
				GameItem item = ((selectedItem != null && list.Contains(selectedItem)) ? selectedItem : list[0]);
				list = new List<GameItem> { item };
			}
			if (list.Count != 0 && SumNegociatedValue(list) <= PlayerStore.Instance.playerCash)
			{
				BuyMultipleItem(list);
			}
		}
	}

	private float BuildBarter(StoreClient storeClient, bool locked)
	{
		float result = 0f;
		if (storeClient.barterOffer != null && !string.IsNullOrEmpty(storeClient.barterOffer.barterTextBody))
		{
			result = TabletUI.InfoLine(frame, frame.infoHost, 0f, "", storeClient.barterOffer.barterTextBody);
		}
		foreach (GameItem item in EmporiumEntry.Instance.GetAllNonOwnedItem())
		{
			TabletUI.TextRow(frame, frame.content, item.GetDisplayName(), 18f, TabletStyle.INK);
		}
		bool flag = DialogUIManager.Instance.GetCurrentDialog()?.isMainDialog ?? false;
		bool flag2 = storeClient.barterOffer != null;
		if (flag2 && storeClient.barterOffer.OfferSetCount() > 1 && storeClient.isIntroduced && !flag)
		{
			RectTransform parent = TabletUI.NewRect(frame.footer, "Selector", 12f, 21f, 506f, 28f);
			selectorPrev = TabletUI.ArrowButton(parent, "Prev", 16f, 0f, 28f, frame.prevNormal, frame.prevHover, OnLeftArrowButtonClick);
			selectorNext = TabletUI.ArrowButton(parent, "Next", 462f, 0f, 28f, frame.nextNormal, frame.nextHover, OnRightArrowButtonClick);
			TextMeshProUGUI textMeshProUGUI = TabletUI.NewText(frame, parent, "Label", 44f, 0f, 418f, 28f, FontLoader.FontRole.Noto_Sans, 18f, TabletStyle.INK, TextAlignmentOptions.Center);
			textMeshProUGUI.text = LocHelper.GetLocalizedUI("ui_nego_choose_offer", storeClient.barterOffer.currentSetIndex + 1, storeClient.barterOffer.OfferSetCount());
			TabletUI.AutoSize(textMeshProUGUI);
		}
		primaryButton = TabletUI.BigButton(frame, frame.buttonHost, 153f, 0f, LocHelper.GetLocalizedUI("ui_nego_accept_offer"), OnOfferAcceptClick);
		bool flag3 = flag2 && !flag && storeClient.barterOffer.IsBarterAcceptable() && storeClient.isIntroduced;
		primaryButton.SetInteractable(flag3 && !locked);
		return result;
	}

	private void BuildInfoDealer(StoreClient storeClient, bool locked)
	{
		TabletUI.ListRow(frame, frame.content, LocHelper.GetLocalizedUI("ui_nego_info_body"), TabletUI.Credits(storeClient.serviceCost), refused: false, null, null);
		TabletUI.FooterLine(frame, frame.footer, 17.5f, LocHelper.GetLocalizedUI("ui_nego_total"), TabletUI.Credits(storeClient.serviceCost), PlayerStore.Instance.CanPay(storeClient.serviceCost) ? null : new Color?(TabletStyle.RED));
		primaryButton = TabletUI.BigButton(frame, frame.buttonHost, 153f, 0f, LocHelper.GetLocalizedUI("ui_nego_purchase_info"), OnOfferAcceptClick);
		primaryButton.SetInteractable(PlayerStore.Instance.CanPay(storeClient.serviceCost) && storeClient.isIntroduced && !locked);
	}

	private void BuildGunOrder(StoreClient storeClient, bool locked)
	{
		GunOrderData gunOrderData = storeClient.gunOrderData;
		if (gunOrderData == null)
		{
			Debug.LogError("Gunsmith StoreClient gunOrderData is null, cannot show the gun order.");
			return;
		}
		GameItem gameItem = GunOrderData.ResolveOrderGun();
		TabletUI.SubHeader(frame, frame.content, LocHelper.GetLocalizedUI("ui_nego_gun_base_requirements"), 28f);
		foreach (GunOrderRequirement mandatoryRequirement in gunOrderData.mandatoryRequirements)
		{
			bool met = gameItem != null && mandatoryRequirement.IsMet(gameItem);
			GunRequirementRow(mandatoryRequirement, met, "", gameItem);
		}
		bool flag = gameItem != null && gunOrderData.IsAccepted();
		if (gunOrderData.isBestOrder)
		{
			TabletUI.SubHeader(frame, frame.content, LocHelper.GetLocalizedUI("ui_nego_gun_bonus_requirements"), 28f);
			GameItem gun = (flag ? gameItem : null);
			GunRequirementRow(LocHelper.GetLocalizedUIOrDefault("ui_nego_gun_market_price", "Market price"), flag, "+" + TabletUI.Credits(GunOrderData.GetMarketPrice(gun)));
			GunRequirementRow(LocHelper.GetLocalizedUIOrDefault("ui_nego_gun_best_premium", "Best of its kind, +{0}%", 50), flag, "+" + TabletUI.Credits(GunOrderData.GetBestPremium(gun)));
		}
		else if (gunOrderData.bonusRequirements.Count > 0)
		{
			TabletUI.SubHeader(frame, frame.content, LocHelper.GetLocalizedUI("ui_nego_gun_bonus_requirements"), 28f);
			foreach (GunOrderRequirement bonusRequirement in gunOrderData.bonusRequirements)
			{
				bool met2 = gameItem != null && bonusRequirement.IsMet(gameItem);
				GunRequirementRow(bonusRequirement, met2, "+" + TabletUI.Credits(bonusRequirement.GetReward()), gameItem);
			}
		}
		if (gunOrderData.isServiceOrder)
		{
			int partsValue = GunOrderData.GetPartsValue(gameItem);
			GunRequirementRow(LocHelper.GetLocalizedUIOrDefault("ui_nego_gun_service_parts", "Your parts on the gun"), partsValue > 0, "+" + TabletUI.Credits(partsValue));
		}
		List<NegociationData> priceEventLines = GunOrderData.GetPriceEventLines();
		if (priceEventLines.Count > 0)
		{
			TabletUI.SubHeader(frame, frame.content, LocHelper.GetLocalizedUI("ui_nego_market"), 28f);
			foreach (NegociationData item in priceEventLines)
			{
				TabletUI.TextRow(frame, frame.content, ItemFeature.CreateFeatureFromNegociationData(item).GetPublicDisplay(), 18f, TabletStyle.INK);
			}
		}
		string value = (flag ? gunOrderData.GetTotalRewardDisplay() : TabletUI.Credits(gunOrderData.GetBaseReward()));
		TabletUI.FooterLine(frame, frame.footer, 17.5f, LocHelper.GetLocalizedUI("ui_request_reward"), value);
		primaryButton = TabletUI.BigButton(frame, frame.buttonHost, 153f, 0f, LocHelper.GetLocalizedUI("ui_nego_accept_offer"), OnOfferAcceptClick);
		primaryButton.SetInteractable(flag && !locked);
	}

	private void GunRequirementRow(GunOrderRequirement requirement, bool met, string reward, GameItem gun)
	{
		GunRequirementRow(requirement.GetDisplayName() + CurrentValueSuffix(requirement, met, gun), met, reward);
	}

	private static string CurrentValueSuffix(GunOrderRequirement requirement, bool met, GameItem gun)
	{
		if (gun == null || !gun.IsTag("ADV_GUN_TAG") || requirement.IsFlag())
		{
			return "";
		}
		string text = (requirement.isCaliber ? GunHelper.GetCaliberName(gun.GetTagReadonly("GUN_CALIBER_TAG").GetInt()) : gun.GetTagReadonly(requirement.id).GetInt().ToString());
		string text2 = ColorUtility.ToHtmlStringRGB(met ? TabletStyle.GREEN : TabletStyle.RED);
		return " <color=#" + text2 + ">(" + text + ")</color>";
	}

	private void GunRequirementRow(string name, bool met, string reward)
	{
		RectTransform parent = TabletUI.TextRow(frame, frame.content, name, 18f, met ? TabletStyle.GREEN : TabletStyle.INK);
		if (reward.Length > 0)
		{
			TabletUI.NewText(frame, parent, "Reward", 368f, 0f, 130f, 28f, FontLoader.FontRole.Noto_Sans, 20f, met ? TabletStyle.GREEN : TabletStyle.INK, TextAlignmentOptions.MidlineRight).text = reward;
		}
	}

	private void BuildItemRow(GameItem item, string refusal, string right)
	{
		listedItems.Add(item);
		bool flag = refusal.Length > 0;
		Func<string> func = ((((mode == Mode.Buying) ? GetNotBuyingReason(item) : "").Length > 0) ? ((Func<string>)(() => GetNotBuyingReason(item))) : null);
		if (item == selectedItem)
		{
			BuildExpandedRow(item, right, flag, func);
			return;
		}
		string displayName = item.GetDisplayName();
		string text = displayName + "\n" + right + "\n" + (flag ? "r" : "-") + ((func != null) ? "t" : "-");
		if (cachedRows.TryGetValue(item, out var value) && value.row != null && value.key == text)
		{
			value.used = true;
			value.row.transform.SetAsLastSibling();
			value.row.selected = false;
			return;
		}
		if (value != null && value.row != null)
		{
			RemoveRow(value.row.gameObject);
		}
		TabletListRow tabletListRow = TabletUI.ListRow(frame, frame.content, displayName, right, flag, func, delegate
		{
			OnRowClick(item);
		});
		tabletListRow.selected = false;
		cachedRows[item] = new CachedRow
		{
			row = tabletListRow,
			key = text,
			used = true
		};
	}

	private void ClearContentKeepingRows()
	{
		HashSet<Transform> hashSet = new HashSet<Transform>();
		foreach (CachedRow value in cachedRows.Values)
		{
			value.used = false;
			if (value.row != null)
			{
				hashSet.Add(value.row.transform);
			}
		}
		for (int num = frame.content.childCount - 1; num >= 0; num--)
		{
			Transform child = frame.content.GetChild(num);
			if (!hashSet.Contains(child))
			{
				RemoveRow(child.gameObject);
			}
		}
	}

	private void DropUnusedRows()
	{
		List<GameItem> list = null;
		foreach (KeyValuePair<GameItem, CachedRow> cachedRow in cachedRows)
		{
			if (!cachedRow.Value.used || !(cachedRow.Value.row != null))
			{
				(list ?? (list = new List<GameItem>())).Add(cachedRow.Key);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (GameItem item in list)
		{
			TabletListRow row = cachedRows[item].row;
			if (row != null)
			{
				RemoveRow(row.gameObject);
			}
			cachedRows.Remove(item);
		}
	}

	private static void RemoveRow(GameObject row)
	{
		row.SetActive(value: false);
		UnityEngine.Object.Destroy(row);
	}

	private void AutoSelectLoneItem()
	{
		List<GameItem> list = ((mode == Mode.Buying) ? GetOwnedBackInvItems() : ((mode == Mode.Selling) ? GetNonOwnedBackInvItems() : null));
		if (list == null)
		{
			return;
		}
		if (list.Count == 1)
		{
			GameItem gameItem = list[0];
			if ((selectedItem == null || !list.Contains(selectedItem)) && autoDeclinedItem != gameItem)
			{
				selectedItem = gameItem;
				autoSelectedItem = gameItem;
			}
		}
		else
		{
			if (selectedItem != null && selectedItem == autoSelectedItem)
			{
				selectedItem = null;
			}
			autoSelectedItem = null;
			autoDeclinedItem = null;
		}
	}

	private void OnRowClick(GameItem item)
	{
		bool flag = selectedItem == item;
		selectedItem = (flag ? null : item);
		autoDeclinedItem = ((flag && item == autoSelectedItem) ? item : null);
		autoSelectedItem = null;
		if (selectedItem != null)
		{
			currentNegociatedItem = selectedItem;
		}
		EmporiumAudioPlayer.Instance.audioButton1.Play();
		RefreshUI();
	}

	private void BuildExpandedRow(GameItem item, string right, bool refused, Func<string> tooltip)
	{
		List<(string, List<ItemFeature>)> list = new List<(string, List<ItemFeature>)>();
		AddFeatureSection(item, ItemFeature.ValueStage.Innate, LocHelper.GetLocalizedUI("ui_nego_condition"), list);
		AddFeatureSection(item, ItemFeature.ValueStage.Market, LocHelper.GetLocalizedUI("ui_nego_market"), list);
		AddFeatureSection(item, ItemFeature.ValueStage.Final, LocHelper.GetLocalizedUI("bargain_ui_title"), list);
		float num = 0f;
		if (list.Count > 0)
		{
			num = 8f;
			foreach (var item3 in list)
			{
				List<ItemFeature> item2 = item3.Item2;
				num += 36f;
				num += (float)item2.Count * 32f;
			}
			num += 2f;
		}
		float num2 = 32f + num;
		RectTransform rectTransform = TabletUI.RowRoot(frame.content, "SelectedRow", num2);
		Image image = rectTransform.gameObject.AddComponent<Image>();
		image.color = Color.clear;
		image.raycastTarget = true;
		TabletListRow tabletListRow = rectTransform.gameObject.AddComponent<TabletListRow>();
		tabletListRow.selected = true;
		tabletListRow.onClick = delegate
		{
			OnRowClick(item);
		};
		TabletUI.NewFlat(rectTransform, "SelectedBg", 0f, 0f, 502f, 32f, TabletStyle.ROW_SELECTED_BG);
		GameObject gameObject = (tabletListRow.hoverVisual = TabletUI.RowFrame(frame, rectTransform, num2, 32f));
		RectTransform rectTransform2 = TabletUI.NewRect(rectTransform, "Texts", 0f, 2f, 502f, 28f);
		if (refused)
		{
			CanvasGroup canvasGroup = rectTransform2.gameObject.AddComponent<CanvasGroup>();
			canvasGroup.alpha = 0.3f;
			canvasGroup.blocksRaycasts = false;
		}
		TextMeshProUGUI textMeshProUGUI = TabletUI.NewText(frame, rectTransform2, "Name", 12f, 0f, 360f, 28f, FontLoader.FontRole.Noto_Sans, 18f, TabletStyle.INK, TextAlignmentOptions.MidlineLeft);
		textMeshProUGUI.text = item.GetDisplayName();
		TabletUI.AutoSize(textMeshProUGUI);
		TextMeshProUGUI textMeshProUGUI2 = TabletUI.NewText(frame, rectTransform2, "Right", 366f, 0f, 130f, 28f, FontLoader.FontRole.Noto_Sans, refused ? 16f : 20f, TabletStyle.INK, TextAlignmentOptions.MidlineRight);
		textMeshProUGUI2.text = right;
		TabletUI.AutoSize(textMeshProUGUI2);
		if (tooltip != null)
		{
			TabletUI.AddTooltip(rectTransform2.gameObject, tooltip);
		}
		float num3 = 40f;
		float x = 12f;
		float num4 = 478f;
		foreach (var (text, list2) in list)
		{
			TabletUI.NewFlat(rectTransform, "SubHeaderPlate", x, num3, num4, 28f, TabletStyle.INK);
			TextMeshProUGUI textMeshProUGUI3 = TabletUI.NewText(frame, rectTransform, "SubHeader", x, num3, num4, 28f, FontLoader.FontRole.Noto_Sans, 18f, TabletStyle.SUB_HEADER_TEXT, TextAlignmentOptions.Center);
			textMeshProUGUI3.text = text;
			TabletUI.AutoSize(textMeshProUGUI3);
			num3 += 36f;
			foreach (ItemFeature item4 in list2)
			{
				TextMeshProUGUI textMeshProUGUI4 = TabletUI.NewText(frame, rectTransform, "Feature", x, num3, num4, 24f, FontLoader.FontRole.Noto_Sans, 18f, TabletStyle.INK, TextAlignmentOptions.MidlineLeft, wrap: true);
				textMeshProUGUI4.text = (item4.IsExposed() ? item4.GetExposedString() : item4.GetPublicDisplay());
				textMeshProUGUI4.raycastTarget = true;
				ItemFeatureUI itemFeatureUI = textMeshProUGUI4.gameObject.AddComponent<ItemFeatureUI>();
				itemFeatureUI.itemFeature = item4;
				Button button = textMeshProUGUI4.gameObject.AddComponent<Button>();
				button.transition = Selectable.Transition.None;
				TabletUI.NoKeyboardFocus(button);
				button.onClick.AddListener(itemFeatureUI.OnFeatureUIClick);
				textMeshProUGUI4.ForceMeshUpdate(ignoreActiveState: true, forceTextReparsing: true);
				float num5 = Mathf.Max(24f, Mathf.Ceil(textMeshProUGUI4.preferredHeight));
				if (num5 > 24f)
				{
					textMeshProUGUI4.rectTransform.sizeDelta = new Vector2(num4, num5);
					num2 += num5 - 24f;
				}
				num3 += num5 + 8f;
			}
		}
		if (num2 > rectTransform.sizeDelta.y)
		{
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, num2);
			LayoutElement component = rectTransform.GetComponent<LayoutElement>();
			if (component != null)
			{
				component.preferredHeight = num2;
				component.minHeight = num2;
			}
			RectTransform obj = gameObject.transform as RectTransform;
			obj.sizeDelta = new Vector2(obj.sizeDelta.x, num2);
		}
	}

	private static void AddFeatureSection(GameItem item, ItemFeature.ValueStage stage, string title, List<(string, List<ItemFeature>)> sections)
	{
		List<ItemFeature> list = new List<ItemFeature>();
		for (int i = 0; i < item.itemFeatures.Count; i++)
		{
			ItemFeature itemFeature = item.itemFeatures[i];
			if (itemFeature.GetValueStage() == stage && !itemFeature.isDisabled && itemFeature.IsInitiallyShown() && (!itemFeature.GetPublicHidden() || itemFeature.IsExposed()))
			{
				list.Add(itemFeature);
			}
		}
		if (list.Count > 0)
		{
			sections.Add((title, list));
		}
	}

	public void OnWholesaleClick()
	{
		if (PlayerStore.Instance.currentClientInstance == null || IsTradeLocked())
		{
			return;
		}
		StoreClient storeClient = Client();
		if (!storeClient.isOfferWholesale)
		{
			return;
		}
		audioSourceCashRegister.Play();
		int wholesaleDiscount = storeClient.wholesaleDiscount;
		EmporiumEntry.Instance.SwapInv();
		int wholesalePrice = PlayerStore.Instance.GetWholesalePrice();
		PlayerStore.Instance.playerCash -= wholesalePrice;
		List<GameItem> allNonOwnedItem = EmporiumEntry.Instance.GetAllNonOwnedItem();
		foreach (GameItem item in allNonOwnedItem)
		{
			int cost = RNG.CalculatePercentageOff((int)item.GetNegociatedValue(), wholesaleDiscount);
			PlayerStore.Instance.OnItemBought(item, cost);
		}
		switch (PlayerStore.Instance.GetCurrentClientIntent())
		{
		case StoreClient.ClientIntent.SELL:
			storeClient.CompleteBusiness();
			if (storeClient.acceptDealDialogue != null)
			{
				DialogUIManager.Instance.StartDialogue(storeClient.acceptDealDialogue);
			}
			else if (storeClient.allDoneDialogue != null)
			{
				DialogUIManager.Instance.StartDialogue(storeClient.allDoneDialogue);
			}
			break;
		case StoreClient.ClientIntent.SELLNBUY:
			if (storeClient.acceptDealDialogue != null)
			{
				DialogUIManager.Instance.StartDialogue(storeClient.acceptDealDialogue);
			}
			break;
		}
		storeClient.isOfferWholesale = false;
		EmporiumEntry.Instance.SwapInv();
		foreach (GameItem item2 in allNonOwnedItem)
		{
			PlayerStore.Instance.MoveDealItemToInventory(item2);
		}
		RefreshUI();
	}

	public void OnOfferAcceptClick()
	{
		if (PlayerStore.Instance.currentClientInstance == null || IsTradeLocked())
		{
			return;
		}
		TutorialUIManager.Instance.BuyDealed();
		StoreClient storeClient = Client();
		StoreClient.ClientIntent clientIntent = storeClient.clientIntent;
		if (clientIntent == StoreClient.ClientIntent.UNDEFINED || storeClient.isBusinessComplete)
		{
			return;
		}
		bool flag = IsNegociatingItemOwned();
		if (!flag && clientIntent == StoreClient.ClientIntent.SELL)
		{
			BuyItem();
			audioSourceCashRegister.Play();
		}
		else if (flag && clientIntent == StoreClient.ClientIntent.BUY)
		{
			TrySellItem();
		}
		else
		{
			switch (clientIntent)
			{
			case StoreClient.ClientIntent.SELLNBUY:
				if (flag)
				{
					TrySellItem();
					break;
				}
				BuyItem();
				audioSourceCashRegister.Play();
				break;
			case StoreClient.ClientIntent.BARTER:
				if (storeClient.barterOffer == null || !storeClient.barterOffer.IsBarterAcceptable() || !storeClient.isIntroduced)
				{
					return;
				}
				PlayerStore.Instance.AcceptBarter();
				DialogUIManager.Instance.StartDialogue(storeClient.acceptDealDialogue);
				audioSourceCashRegister.Play();
				break;
			case StoreClient.ClientIntent.INFORMATION_DEALER:
				if (!PlayerStore.Instance.CanPay(storeClient.serviceCost) || !storeClient.isIntroduced)
				{
					return;
				}
				PlayerStore.Instance.playerCash -= storeClient.serviceCost;
				storeClient.CompleteBusiness();
				DialogUIManager.Instance.InterruptDialog();
				DialogUIManager.Instance.StartDialogue(storeClient.acceptDealDialogue);
				audioSourceCashRegister.Play();
				break;
			case StoreClient.ClientIntent.GUNSMITH:
			{
				if (storeClient.gunOrderData == null || !storeClient.gunOrderData.IsAccepted())
				{
					return;
				}
				GameItem gameItem = GunOrderData.ResolveOrderGun();
				storeClient.gunOrderData.OnOrderAccepted();
				if (storeClient.gunOrderData.isServiceOrder)
				{
					foreach (GameItem playerPart in GunHelper.GetPlayerParts(gameItem))
					{
						PlayerStore.Instance.secData.CheckSoldContraband(playerPart, storeClient);
					}
				}
				else
				{
					PlayerStore.Instance.secData.CheckSoldContraband(gameItem, storeClient);
				}
				gameItem.CloseContentWindow(includeChildren: true);
				Dialogue dialogue = StoreClientListGun.CreateDeliveryFollowUp(storeClient, gameItem);
				gameItem.Destroy();
				if (dialogue != null)
				{
					DialogUIManager.Instance.StartDialogue(dialogue);
				}
				else
				{
					storeClient.CompleteBusiness();
					DialogUIManager.Instance.StartDialogue(storeClient.acceptDealDialogue);
				}
				audioSourceCashRegister.Play();
				break;
			}
			}
		}
		RefreshUI();
	}

	public void BuyMultipleItem(List<GameItem> items)
	{
		StoreClient storeClient = Client();
		for (int num = items.Count - 1; num >= 0; num--)
		{
			PlayerStore.Instance.BuyItem(items[num]);
		}
		audioSourceCashRegister.Play();
		RefreshUI();
		if (!PlayAcceptDealDialogueOnce(storeClient))
		{
			storeClient.OnDealAccepted();
		}
	}

	public void BuyItem()
	{
		StoreClient storeClient = Client();
		PlayerStore.Instance.BuyItemFromTable();
		RefreshUI();
		if (!PlayAcceptDealDialogueOnce(storeClient))
		{
			storeClient.OnDealAccepted();
		}
	}

	public void SellMultiple(List<GameItem> items, bool ignoreBudget = false)
	{
		int num = 0;
		bool flag = false;
		for (int num2 = items.Count - 1; num2 >= 0; num2--)
		{
			if (PlayerStore.Instance.TryReinspectBeforeSale(items[num2]))
			{
				flag = true;
			}
			else if (PlayerStore.Instance.CanSellThisItem(items[num2], ignoreBudget))
			{
				PlayerStore.Instance.SellItem(items[num2]);
				num++;
			}
		}
		if (num == 0)
		{
			if (flag)
			{
				RefreshUI();
			}
			return;
		}
		audioSourceCashRegister.Play();
		RefreshUI();
		StoreClient storeClient = Client();
		if (!PlayAcceptDealDialogueOnce(storeClient))
		{
			storeClient.OnDealAccepted();
		}
	}

	private static bool PlayAcceptDealDialogueOnce(StoreClient storeClient)
	{
		if (storeClient.acceptDealDialogue == null)
		{
			return false;
		}
		if (storeClient.acceptDealDialoguePlayed)
		{
			return false;
		}
		if (!DialogUIManager.Instance.StartDialogue(storeClient.acceptDealDialogue))
		{
			return false;
		}
		storeClient.acceptDealDialoguePlayed = true;
		return true;
	}

	public void TrySellItem()
	{
		SellItem();
	}

	public void SellItem()
	{
		PlayerStore.Instance.SellItemFromTable();
		RefreshUI();
	}

	public void OnInterogationButtonClick()
	{
		StoreClient storeClient = Client();
		if (storeClient != null && storeClient.interogationDialogue != null)
		{
			DialogUIManager.Instance.StartDialogue(storeClient.interogationDialogue);
		}
	}

	public void OnGlasseButtonClick()
	{
		StoreClient storeClient = Client();
		if (storeClient != null && storeClient.glasseDialogue != null)
		{
			DialogUIManager.Instance.StartDialogue(storeClient.glasseDialogue);
		}
	}

	public void OpenConfirmPanel()
	{
		if (!(frame == null))
		{
			StoreClient storeClient = Client();
			if (storeClient != null)
			{
				int num = GetMultiSellValue() - storeClient.GetBudget();
				frame.confirmText.text = LocHelper.GetLocalizedUI("ui_nego_budget_warning", storeClient.GetBudget(), num);
				TabletUI.ClearChildren(frame.confirmButtonHost);
				float num2 = 57f;
				TabletUI.BigButton(frame, frame.confirmButtonHost, num2, 0f, LocHelper.GetLocalizedMainMenu("yes"), OnConfirmPanelYes, 200f);
				TabletUI.BigButton(frame, frame.confirmButtonHost, num2 + 200f + 16f, 0f, LocHelper.GetLocalizedMainMenu("no"), OnConfirmPanelNo, 200f);
				frame.confirmRoot.SetActive(value: true);
				EmporiumAudioPlayer.Instance.audioButton1.Play();
			}
		}
	}

	public void OnConfirmPanelYes()
	{
		if (!(frame == null))
		{
			if (PlayerStore.Instance.currentClientInstance == null)
			{
				frame.confirmRoot.SetActive(value: false);
				return;
			}
			if (IsTradeLocked())
			{
				frame.confirmRoot.SetActive(value: false);
				return;
			}
			StoreClient storeClient = Client();
			SellMultiple(GetSellableBackInvItems(storeClient), ignoreBudget: true);
			frame.confirmRoot.SetActive(value: false);
		}
	}

	public void OnConfirmPanelNo()
	{
		if (!(frame == null))
		{
			frame.confirmRoot.SetActive(value: false);
			EmporiumAudioPlayer.Instance.audioButton3.Play();
		}
	}

	public void OnLeftArrowButtonClick()
	{
		CycleBarterOfferSet(-1);
	}

	public void OnRightArrowButtonClick()
	{
		CycleBarterOfferSet(1);
	}

	private void CycleBarterOfferSet(int offset)
	{
		StoreClient storeClient = Client();
		if (storeClient != null && storeClient.barterOffer != null && storeClient.barterOffer.UseOfferSets() && storeClient.isIntroduced)
		{
			storeClient.barterOffer.ShowSetByOffset(offset);
			EmporiumAudioPlayer.Instance.audioButton1.Play();
			RefreshUI();
		}
	}
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BargainUIManager : MonoBehaviour
{
	private class Group
	{
		public BargainType type;

		public GameObject root;

		public readonly List<TabletButton> buttons = new List<TabletButton>();

		public readonly List<int> percents = new List<int>();
	}

	private const float DISCOUNT_BUDGET_FACTOR = 2f;

	private static readonly int[] MARKUP_BUDGET_CUT_PERCENTS = new int[4] { 10, 15, 20, 25 };

	private const float MARKUP_ACCEPT_FACTOR = 2f;

	private const float MARKUP_LEAVE_FLOOR_FACTOR = 1.2f;

	private const int MARKUP_EVENT_BONUS = 15;

	private const int MARKUP_REP_DIVISOR = 6;

	private const int NEGATIVE_REP_PENALTY_MULTIPLIER = 2;

	private const int MARKUP_LEAVE_REP_PENALTY = -3;

	private const int BLACKMAIL_PERCENT = 20;

	private const int BLACKMAIL_BASE_ACCEPT_CHANCE = 60;

	private const int BLACKMAIL_EVENT_BONUS = 15;

	private const int BLACKMAIL_REP_DIVISOR = 4;

	private const int BLACKMAIL_REJECT_REP_PENALTY = -5;

	private const int BLACKMAIL_INNOCENT_REJECT_REP_PENALTY = -7;

	private const int BLACKMAIL_INNOCENT_LEAVE_CHANCE = 50;

	private const int BLACKMAIL_INNOCENT_LEAVE_REP_PENALTY = -10;

	private const float BLACKMAIL_TRADE_REP_PENALTY = 6f;

	private const int THREATEN_PERCENT = 20;

	private const int THREATEN_FALSE_REJECT_REP_PENALTY = -7;

	private const int THREATEN_FALSE_LEAVE_CHANCE = 50;

	private const int THREATEN_FALSE_LEAVE_REP_PENALTY = -10;

	private StoreClient trackedClient;

	private double baseTradeRepMultiplier = 1.0;

	private bool discountOffered;

	private int discountPercent;

	private bool markupOffered;

	private int markupPercent;

	private bool markupApplied;

	private bool blackmailOffered;

	private bool blackmailApplied;

	private bool buyingMarkupOffered;

	private int buyingMarkupPercent;

	private bool buyingDiscountOffered;

	private int buyingDiscountPercent;

	private bool buyingDiscountApplied;

	private bool threatenOffered;

	private bool threatenApplied;

	private bool repPreviewActive;

	private BargainType repPreviewType;

	private int repPreviewPercent;

	public const string WINDOW_ID = "bargain_panel";

	[Tooltip("UI3/Prefab/BargainPanel.prefab, from Build > Generate Tablet UI Prefabs.")]
	public GameObject panelPrefab;

	public GameObject uiObject;

	private CustomUIWindow window;

	private BargainPanelFrame frame;

	private ZoomFollower follower;

	private BargainPanelDock dock;

	private static readonly int[] PRICE_PERCENTS = new int[4] { 10, 25, 50, 75 };

	private readonly List<Group> groups = new List<Group>();

	private string builtLayoutKey;

	private TextMeshProUGUI repLine;

	private Image repIcon;

	private const string REP_COLOR_DEFAULT = "#CC7755";

	private const string REP_COLOR_PREVIEW_GAIN = "#7ab053";

	private const string REP_COLOR_PREVIEW_LOSS = "#853737";

	private static readonly string[] BARGAIN_FEATURE_IDS = new string[8] { "bargainDiscount", "bargainMarkup", "bargainBuyingMarkup", "bargainBuyingDiscount", "blackmailDiscount", "blackmailMarkup", "threatenDiscount", "threatenMarkup" };

	private static List<string> acceptGratefulLines;

	private static List<string> acceptVeryGratefulLines;

	private static List<string> acceptReluctantLines;

	private static List<string> acceptReluctantEventLines;

	private static List<string> acceptReluctantAugLines;

	private static List<string> acceptVeryReluctantLines;

	private static List<string> rejectLines;

	private static List<string> hardRejectLines;

	private static List<string> leaveLines;

	private static List<string> budgetIncreaseLines;

	private static List<string> budgetDecreaseLines;

	private static List<string> talkBadLines;

	private static List<string> innocentBlackmailLines;

	private static List<string> falseThreatenLines;

	private const int VERY_THRESHOLD = 50;

	public static BargainUIManager Instance { get; private set; }

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

	public bool DiscountOffered
	{
		get
		{
			Refresh();
			return discountOffered;
		}
	}

	public int DiscountPercent
	{
		get
		{
			Refresh();
			return discountPercent;
		}
	}

	public bool MarkupOffered
	{
		get
		{
			Refresh();
			return markupOffered;
		}
	}

	public int MarkupPercent
	{
		get
		{
			Refresh();
			return markupPercent;
		}
	}

	public bool BlackmailOffered
	{
		get
		{
			Refresh();
			return blackmailOffered;
		}
	}

	public bool BuyingMarkupOffered
	{
		get
		{
			Refresh();
			return buyingMarkupOffered;
		}
	}

	public int BuyingMarkupPercent
	{
		get
		{
			Refresh();
			return buyingMarkupPercent;
		}
	}

	public bool BuyingDiscountOffered
	{
		get
		{
			Refresh();
			return buyingDiscountOffered;
		}
	}

	public int BuyingDiscountPercent
	{
		get
		{
			Refresh();
			return buyingDiscountPercent;
		}
	}

	public bool ThreatenOffered
	{
		get
		{
			Refresh();
			return threatenOffered;
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
		InitializeDialogueLines();
		CreateWindow();
	}

	private void CreateWindow()
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[BargainUIManager] No CustomUIManager in the scene, the bargain panel cannot be created");
			return;
		}
		if (panelPrefab == null)
		{
			Debug.LogError("[BargainUIManager] panelPrefab is not assigned. Run Build > Generate Tablet UI Prefabs in the store scene.");
			return;
		}
		window = CustomUIManager.Instance.CreateWindow("bargain_panel", null, "paper", panelPrefab).GetWindow();
		frame = ((window.root != null) ? window.root.GetComponent<BargainPanelFrame>() : null);
		if (frame == null)
		{
			Debug.LogError("[BargainUIManager] panelPrefab has no BargainPanelFrame on its root; regenerate it");
			window.Close();
			window = null;
			return;
		}
		uiObject = window.root;
		uiObject.name = "BargainPanel";
		follower = uiObject.AddComponent<ZoomFollower>();
		dock = uiObject.AddComponent<BargainPanelDock>();
		frame.header.text = LocHelper.GetLocalizedUI("bargain_ui_title");
		frame.repHeader.text = LocHelper.GetLocalizedUI("bargain_ui_rep_mult");
		float num = TabletUI.MeasureWidth(frame.repHeader, frame.repHeader.text);
		float x = (314f + num) * 0.5f + 8f;
		TabletUI.InfoIcon(frame.footer, x, 7f, frame.infoCream, () => LocHelper.GetLocalizedUI("bargain_ui_rep_mult_info"));
		repLine = TabletUI.NewText(frame, frame.repLineHost, "Line", 0f, 0f, 296f, 25f, FontLoader.FontRole.Noto_Sans, 18f, TabletStyle.BARGAIN_RED, TextAlignmentOptions.Center);
		TabletUI.AutoSize(repLine);
		repIcon = TabletUI.InfoIcon(frame.repLineHost, 0f, 5.5f, frame.infoRed, () => LocHelper.GetLocalizedUI("bargain_ui_rep_mult_info2"));
		uiObject.SetActive(value: false);
	}

	private void Update()
	{
		if (IsOpen)
		{
			RefreshUI();
		}
	}

	public void RefreshUI()
	{
		if (frame == null)
		{
			return;
		}
		UpdateGroupVisibility();
		bool flag = IsDialoguePlaying() || IsClientMoving() || StoreUIManager.Instance.isAboutToStartMainDialog || IsStoreDisrupted();
		foreach (Group group in groups)
		{
			bool interactable = !flag && CanOffer(group.type);
			for (int i = 0; i < group.buttons.Count; i++)
			{
				group.buttons[i].SetInteractable(interactable);
				group.buttons[i].SetSelected(IsOffered(group.type, group.percents[i]));
			}
		}
	}

	private bool IsOffered(BargainType type, int percent)
	{
		switch (type)
		{
		case BargainType.Discount:
			if (discountOffered)
			{
				return discountPercent == percent;
			}
			return false;
		case BargainType.Markup:
			if (markupOffered)
			{
				return markupPercent == percent;
			}
			return false;
		case BargainType.BuyingMarkup:
			if (buyingMarkupOffered)
			{
				return buyingMarkupPercent == percent;
			}
			return false;
		case BargainType.BuyingDiscount:
			if (buyingDiscountOffered)
			{
				return buyingDiscountPercent == percent;
			}
			return false;
		case BargainType.Blackmail:
			return blackmailOffered;
		case BargainType.Threaten:
			return threatenOffered;
		default:
			return false;
		}
	}

	private static bool IsDialoguePlaying()
	{
		if (DialogUIManager.Instance == null)
		{
			return false;
		}
		if (DialogUIManager.Instance.GetCurrentDialog() == null)
		{
			return false;
		}
		return true;
	}

	private static bool IsClientMoving()
	{
		if (StoreClientMono.Instance != null)
		{
			return StoreClientMono.Instance.IsMoving();
		}
		return false;
	}

	private static bool IsStoreDisrupted()
	{
		return NegociationUIManager.IsTradeLocked();
	}

	private void UpdateGroupVisibility()
	{
		Refresh();
		bool flag = trackedClient != null;
		bool num = flag && trackedClient.IsClientBuying();
		bool flag2 = flag && trackedClient.IsClientSelling();
		bool flag3 = flag && PlayerStore.Instance != null && PlayerStore.Instance.isAugIntroduced;
		List<BargainType> list = new List<BargainType>();
		if (num)
		{
			list.Add(BargainType.Discount);
			list.Add(BargainType.Markup);
		}
		if (flag2)
		{
			list.Add(BargainType.BuyingMarkup);
			list.Add(BargainType.BuyingDiscount);
		}
		if (flag)
		{
			list.Add(BargainType.Blackmail);
		}
		if (flag3)
		{
			list.Add(BargainType.Threaten);
		}
		string text = string.Join(",", list) + "|" + (flag ? trackedClient.GetHashCode() : 0);
		if (text != builtLayoutKey)
		{
			builtLayoutKey = text;
			BuildGroups(list);
		}
		frame.footer.gameObject.SetActive(flag);
		if (flag)
		{
			UpdateRepGainText();
		}
	}

	private void BuildGroups(List<BargainType> shown)
	{
		TabletUI.ClearChildren(frame.groupsHost);
		groups.Clear();
		float num = 0f;
		foreach (BargainType item in shown)
		{
			groups.Add(BuildGroup(item, num));
			num += 82f;
		}
		float y = Mathf.Max(399f, num - 8f);
		frame.groupsHost.sizeDelta = new Vector2(frame.groupsHost.sizeDelta.x, y);
		if (frame.groupsScroll != null)
		{
			frame.groupsScroll.verticalNormalizedPosition = 1f;
		}
	}

	private Group BuildGroup(BargainType type, float y)
	{
		Group group = new Group
		{
			type = type
		};
		RectTransform rectTransform = TabletUI.NewRect(frame.groupsHost, "Group_" + type, 12f, y, 290f, 74f);
		group.root = rectTransform.gameObject;
		TabletUI.NewStretched(rectTransform, "Frame", frame.frame2, TabletStyle.INK);
		float num = 2f;
		float num2 = 286f;
		TabletUI.NewFlat(rectTransform, "HeaderPlate", num, 2f, num2, 28f, TabletStyle.INK);
		string localizedUI = LocHelper.GetLocalizedUI(TitleKey(type));
		TextMeshProUGUI textMeshProUGUI = TabletUI.NewText(frame, rectTransform, "Header", num, 2f, num2, 28f, FontLoader.FontRole.Noto_Sans, 18f, TabletStyle.SUB_HEADER_TEXT, TextAlignmentOptions.Center);
		textMeshProUGUI.text = localizedUI;
		float num3 = TabletUI.MeasureWidth(textMeshProUGUI, localizedUI);
		float num4 = 11f;
		textMeshProUGUI.rectTransform.anchoredPosition += new Vector2(0f - num4, 0f);
		TabletUI.AutoSize(textMeshProUGUI);
		string infoKey = TitleKey(type) + "_info";
		TabletUI.InfoIcon(rectTransform, num + (num2 + num3) * 0.5f + 8f - num4, 9f, frame.infoCream, () => LocHelper.GetLocalizedUI(infoKey));
		bool flag = type == BargainType.Blackmail || type == BargainType.Threaten;
		int num5 = (flag ? 1 : PRICE_PERCENTS.Length);
		float y2 = 36f;
		float num6 = (num2 - 12f - (float)(num5 - 1) * 8f) / (float)num5;
		for (int i = 0; i < num5; i++)
		{
			int percent = (flag ? ((type == BargainType.Blackmail) ? 20 : 20) : PRICE_PERCENTS[i]);
			float x = num + 6f + (float)i * (num6 + 8f);
			BargainType captured = type;
			TabletButton tabletButton = TabletUI.SmallButton(frame, rectTransform, x, y2, num6, 32f, ButtonLabel(type, percent), delegate
			{
				OnBargainButton(captured, percent);
			});
			tabletButton.gameObject.AddComponent<BargainButton>().Setup(type, percent);
			group.buttons.Add(tabletButton);
			group.percents.Add(percent);
		}
		return group;
	}

	private static string TitleKey(BargainType type)
	{
		return type switch
		{
			BargainType.Discount => "bargain_ui_offer_discount", 
			BargainType.Markup => "bargain_ui_add_markup", 
			BargainType.BuyingMarkup => "bargain_ui_offer_markup", 
			BargainType.BuyingDiscount => "bargain_ui_demand_discount", 
			BargainType.Blackmail => "bargain_ui_blackmail", 
			_ => "bargain_ui_threathen", 
		};
	}

	private static string ButtonLabel(BargainType type, int percent)
	{
		switch (type)
		{
		case BargainType.Discount:
		case BargainType.BuyingDiscount:
			return "-" + percent + "%";
		case BargainType.Markup:
		case BargainType.BuyingMarkup:
			return "+" + percent + "%";
		default:
			return percent + "%";
		}
	}

	private void OnBargainButton(BargainType type, int percent)
	{
		EmporiumAudioPlayer.Instance.audioButton1.Play();
		switch (type)
		{
		case BargainType.Discount:
			OfferDiscount(percent);
			break;
		case BargainType.Markup:
			OfferMarkup(percent);
			break;
		case BargainType.BuyingMarkup:
			OfferBuyingMarkup(percent);
			break;
		case BargainType.BuyingDiscount:
			DemandBuyingDiscount(percent);
			break;
		case BargainType.Blackmail:
			Blackmail();
			break;
		case BargainType.Threaten:
			ThreatenAug();
			break;
		}
		RefreshUI();
	}

	private void UpdateRepGainText()
	{
		if (repLine == null || trackedClient == null)
		{
			return;
		}
		bool flag = repPreviewActive && CanOffer(repPreviewType);
		double num;
		double num2;
		if (flag)
		{
			(num, num2) = ComputePreviewMultipliers(repPreviewType, repPreviewPercent);
		}
		else
		{
			double tradeRepMultiplier = trackedClient.GetTradeRepMultiplier(isPlayerSold: true);
			double tradeRepMultiplier2 = trackedClient.GetTradeRepMultiplier(isPlayerSold: false);
			num = tradeRepMultiplier;
			num2 = tradeRepMultiplier2;
		}
		double repPer = ComputeRepPer1000Credits(num, trackedClient.clientFaction);
		double repPer2 = ComputeRepPer1000Credits(num2, trackedClient.clientFaction);
		bool num3 = trackedClient.IsClientBuying();
		bool flag2 = trackedClient.IsClientSelling();
		string text = "";
		if (num3)
		{
			text += BuildRepGainLine(LocHelper.GetLocalizedUI("ui_nego_title_selling"), num, repPer, flag && BargainAffectsSellSide(repPreviewType));
		}
		if (flag2)
		{
			if (text.Length > 0)
			{
				text += "   ";
			}
			text += BuildRepGainLine(LocHelper.GetLocalizedUI("ui_nego_title_buying"), num2, repPer2, flag && BargainAffectsBuySide(repPreviewType));
		}
		repLine.text = text;
		float num4 = Mathf.Min(TabletUI.MeasureWidth(repLine, text), repLine.rectTransform.sizeDelta.x);
		float x = (repLine.rectTransform.sizeDelta.x + num4) * 0.5f + 4f;
		repIcon.rectTransform.anchoredPosition = new Vector2(x, repIcon.rectTransform.anchoredPosition.y);
	}

	private static string BuildRepGainLine(string label, double mult, double repPer1000, bool highlighted)
	{
		string text = ((!highlighted) ? "#CC7755" : ((repPer1000 > 0.0) ? "#7ab053" : "#853737"));
		return $"<color={text}>{label}: x{mult:F2} [{repPer1000:F2}]</color>";
	}

	private static double ComputeRepPer1000Credits(double tradeMult, string factionId)
	{
		double num = 1000.0 * tradeMult;
		if (factionId != null)
		{
			if (factionId.Equals("FACTION_LOWER_LEVEL"))
			{
				num *= 0.66;
			}
			else if (factionId.Equals("FACTION_UPPER_LEVEL"))
			{
				num *= 1.2;
			}
		}
		if (StartingPerk.IsDiplomatic() && num > 0.0)
		{
			num *= 1.33;
		}
		return num / 400.0;
	}

	public bool IsCurrentClientCoerced()
	{
		Refresh();
		if (!blackmailOffered)
		{
			return threatenOffered;
		}
		return true;
	}

	public bool IsCriminal()
	{
		return GetClient()?.IsWanted() ?? false;
	}

	public bool IsEventCustomer()
	{
		StoreClient client = GetClient();
		if (client != null)
		{
			return !string.IsNullOrEmpty(client.eventSourceId);
		}
		return false;
	}

	public bool CanOfferDiscount()
	{
		Refresh();
		if (trackedClient == null)
		{
			return false;
		}
		if (discountOffered || markupOffered)
		{
			return false;
		}
		if (blackmailOffered || threatenOffered)
		{
			return false;
		}
		return trackedClient.IsClientBuying();
	}

	public bool CanOfferMarkup()
	{
		Refresh();
		if (trackedClient == null)
		{
			return false;
		}
		if (discountOffered || markupOffered)
		{
			return false;
		}
		return trackedClient.IsClientBuying();
	}

	public bool CanBlackmail()
	{
		Refresh();
		if (trackedClient == null)
		{
			return false;
		}
		if (blackmailOffered)
		{
			return false;
		}
		if (trackedClient.isSecurity)
		{
			return false;
		}
		if (!trackedClient.IsClientBuying())
		{
			return trackedClient.IsClientSelling();
		}
		return true;
	}

	public bool CanOfferBuyingMarkup()
	{
		Refresh();
		if (trackedClient == null)
		{
			return false;
		}
		if (buyingMarkupOffered || buyingDiscountOffered)
		{
			return false;
		}
		if (blackmailOffered || threatenOffered)
		{
			return false;
		}
		return trackedClient.IsClientSelling();
	}

	public bool CanDemandBuyingDiscount()
	{
		Refresh();
		if (trackedClient == null)
		{
			return false;
		}
		if (buyingMarkupOffered || buyingDiscountOffered)
		{
			return false;
		}
		return trackedClient.IsClientSelling();
	}

	public bool CanThreatenAug()
	{
		Refresh();
		if (trackedClient == null)
		{
			return false;
		}
		if (threatenOffered)
		{
			return false;
		}
		if (trackedClient.isSecurity)
		{
			return false;
		}
		if (!trackedClient.IsClientBuying())
		{
			return trackedClient.IsClientSelling();
		}
		return true;
	}

	public void OfferDiscount(int percent)
	{
		if (CanOfferDiscount())
		{
			percent = Mathf.Abs(percent);
			discountOffered = true;
			discountPercent = percent;
			ApplyOwnedItemFeature(() => ItemFeatureList.BargainDiscount(percent), "bargainDiscount");
			if (trackedClient.useClientBudget)
			{
				int budget = trackedClient.GetBudget();
				trackedClient.ModBudget(RNG.MultiplyAndRound(budget, (float)percent / 100f * 2f));
			}
			RecomputeTradeRepMultiplier();
			PlayerStore.Instance.RefreshCounterItem();
			Dialogue dialogue = CreateAcceptGratefulDialog(percent);
			if (trackedClient.useClientBudget)
			{
				AppendClientLine(dialogue, budgetIncreaseLines);
			}
			DialogUIManager.Instance.StartDialogue(dialogue);
		}
	}

	public void OfferMarkup(int percent)
	{
		if (!CanOfferMarkup())
		{
			return;
		}
		percent = Mathf.Abs(percent);
		markupOffered = true;
		markupPercent = percent;
		int factionRepBonus = GetFactionRepBonus(trackedClient.clientFaction);
		int num = (IsEventCustomer() ? 15 : 0);
		int num2 = Mathf.RoundToInt(100f - (float)percent * 2f + (float)factionRepBonus + (float)num + (float)GetDealMakerBonus());
		int num3 = Mathf.RoundToInt(100f - (float)percent * 1.2f + (float)factionRepBonus + (float)num);
		if (trackedClient.alwaysRejectBadBargain)
		{
			num2 = 0;
		}
		if (trackedClient.isAug)
		{
			RefuseAugHaggle(percent);
			return;
		}
		int randomInt = RNG.GetRandomInt(0, 99);
		if (randomInt < num2)
		{
			markupApplied = true;
			ApplyOwnedItemFeature(() => ItemFeatureList.BargainMarkup(percent), "bargainMarkup");
			if (trackedClient.useClientBudget)
			{
				int budget = trackedClient.GetBudget();
				trackedClient.ModBudget(-RNG.MultiplyAndRound(budget, (float)GetMarkupBudgetCutPercent(percent) / 100f));
			}
			RecomputeTradeRepMultiplier();
			PlayerStore.Instance.RefreshCounterItem();
			Dialogue dialogue = CreateAcceptReluctantDialog(percent);
			if (trackedClient.useClientBudget)
			{
				AppendClientLine(dialogue, budgetDecreaseLines);
			}
			DialogUIManager.Instance.StartDialogue(dialogue);
		}
		else if (randomInt < num3 || trackedClient.staysAfterFailedBargain)
		{
			DialogUIManager.Instance.StartDialogue(CreateRejectDialog(percent));
		}
		else
		{
			StoreReputation.ModReputation(trackedClient.clientFaction, -3);
			trackedClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
			Dialogue dialogue2 = CreateLeaveDialog();
			AppendClientLine(dialogue2, talkBadLines).SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
			DialogUIManager.Instance.StartDialogue(dialogue2);
		}
	}

	private static int GetMarkupBudgetCutPercent(int percent)
	{
		int result = MARKUP_BUDGET_CUT_PERCENTS[0];
		for (int i = 0; i < PRICE_PERCENTS.Length && i < MARKUP_BUDGET_CUT_PERCENTS.Length; i++)
		{
			if (percent >= PRICE_PERCENTS[i])
			{
				result = MARKUP_BUDGET_CUT_PERCENTS[i];
			}
		}
		return result;
	}

	private static int GetFactionRepBonus(string factionId)
	{
		StoreReputation storeReputation = StoreReputation.GetStoreReputation(factionId);
		if (storeReputation == null)
		{
			return 0;
		}
		int num = storeReputation.GetReputation() / 6;
		if (num < 0)
		{
			num *= 2;
		}
		return num;
	}

	private static int GetDealMakerBonus()
	{
		if (PlayerStore.Instance == null)
		{
			return 0;
		}
		return PlayerStore.Instance.dealMakerBargainBonus;
	}

	public void Blackmail()
	{
		if (!CanBlackmail())
		{
			return;
		}
		blackmailOffered = true;
		if (!IsCriminal())
		{
			HandleInnocentBlackmail();
			return;
		}
		int num = ((trackedClient.clientFaction != null) ? GetFactionRepBonusBlackmail(trackedClient.clientFaction) : 0);
		int num2 = (IsEventCustomer() ? 15 : 0);
		int num3 = 60 + num + num2 + GetDealMakerBonus();
		if (trackedClient.alwaysRejectBadBargain)
		{
			num3 = 0;
		}
		if (RNG.GetRandomInt(0, 99) < num3)
		{
			blackmailApplied = true;
			if (trackedClient.IsClientSelling())
			{
				ApplyNonOwnedItemFeature(() => ItemFeatureList.BlackmailDiscount(20), "blackmailDiscount");
			}
			if (trackedClient.IsClientBuying())
			{
				ApplyOwnedItemFeature(() => ItemFeatureList.BlackmailMarkup(20), "blackmailMarkup");
			}
			RecomputeTradeRepMultiplier();
			PlayerStore.Instance.RefreshCounterItem();
			DialogUIManager.Instance.StartDialogue(CreateAcceptReluctantDialog(20));
		}
		else
		{
			StoreReputation.ModReputation(trackedClient.clientFaction, -5);
			Dialogue dialogue = CreateRejectDialog(20);
			AppendClientLine(dialogue, talkBadLines);
			DialogUIManager.Instance.StartDialogue(dialogue);
		}
	}

	private void HandleInnocentBlackmail()
	{
		bool num = RNG.Roll(50);
		int value = (num ? (-10) : (-7));
		StoreReputation.ModReputation(trackedClient.clientFaction, value);
		if (num)
		{
			trackedClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		}
		Dialogue dialogue = CreateInnocentBlackmailDialog();
		Dialogue d = dialogue;
		if (num)
		{
			d = AppendClientLine(d, leaveLines);
		}
		d = AppendClientLine(d, talkBadLines);
		if (num)
		{
			d.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		}
		DialogUIManager.Instance.StartDialogue(dialogue);
	}

	private static int GetFactionRepBonusBlackmail(string factionId)
	{
		StoreReputation storeReputation = StoreReputation.GetStoreReputation(factionId);
		if (storeReputation == null)
		{
			return 0;
		}
		int num = storeReputation.GetReputation() / 4;
		if (num < 0)
		{
			num *= 2;
		}
		return num;
	}

	public void OfferBuyingMarkup(int percent)
	{
		if (CanOfferBuyingMarkup())
		{
			percent = Mathf.Abs(percent);
			buyingMarkupOffered = true;
			buyingMarkupPercent = percent;
			ApplyNonOwnedItemFeature(() => ItemFeatureList.BargainBuyingMarkup(percent), "bargainBuyingMarkup");
			RecomputeTradeRepMultiplier();
			PlayerStore.Instance.RefreshCounterItem();
			DialogUIManager.Instance.StartDialogue(CreateAcceptGratefulDialog(percent));
		}
	}

	public void DemandBuyingDiscount(int percent)
	{
		if (!CanDemandBuyingDiscount())
		{
			return;
		}
		percent = Mathf.Abs(percent);
		buyingDiscountOffered = true;
		buyingDiscountPercent = percent;
		int factionRepBonus = GetFactionRepBonus(trackedClient.clientFaction);
		int num = (IsEventCustomer() ? 15 : 0);
		int num2 = Mathf.RoundToInt(100f - (float)percent * 2f + (float)factionRepBonus + (float)num + (float)GetDealMakerBonus());
		int num3 = Mathf.RoundToInt(100f - (float)percent * 1.2f + (float)factionRepBonus + (float)num);
		if (trackedClient.alwaysRejectBadBargain)
		{
			num2 = 0;
		}
		if (trackedClient.isAug)
		{
			RefuseAugHaggle(percent);
			return;
		}
		int randomInt = RNG.GetRandomInt(0, 99);
		if (randomInt < num2)
		{
			buyingDiscountApplied = true;
			ApplyNonOwnedItemFeature(() => ItemFeatureList.BargainBuyingDiscount(percent), "bargainBuyingDiscount");
			RecomputeTradeRepMultiplier();
			PlayerStore.Instance.RefreshCounterItem();
			DialogUIManager.Instance.StartDialogue(CreateAcceptReluctantDialog(percent));
		}
		else if (randomInt < num3 || trackedClient.staysAfterFailedBargain)
		{
			DialogUIManager.Instance.StartDialogue(CreateRejectDialog(percent));
		}
		else
		{
			StoreReputation.ModReputation(trackedClient.clientFaction, -3);
			trackedClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
			Dialogue dialogue = CreateLeaveDialog();
			AppendClientLine(dialogue, talkBadLines).SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
			DialogUIManager.Instance.StartDialogue(dialogue);
		}
	}

	private void RefuseAugHaggle(int percent)
	{
		DialogUIManager.Instance.StartDialogue(CreateRejectDialog(percent));
	}

	public void ThreatenAug()
	{
		if (!CanThreatenAug())
		{
			return;
		}
		threatenOffered = true;
		if (!trackedClient.isAug)
		{
			HandleFalseThreaten();
			return;
		}
		threatenApplied = true;
		if (trackedClient.IsClientSelling())
		{
			ApplyNonOwnedItemFeature(() => ItemFeatureList.ThreatenDiscount(20), "threatenDiscount");
		}
		if (trackedClient.IsClientBuying())
		{
			ApplyOwnedItemFeature(() => ItemFeatureList.ThreatenMarkup(20), "threatenMarkup");
		}
		RecomputeTradeRepMultiplier();
		PlayerStore.Instance.RefreshCounterItem();
		DialogUIManager.Instance.StartDialogue(CreateAcceptReluctantDialog(20));
	}

	private void HandleFalseThreaten()
	{
		bool num = RNG.Roll(50);
		int value = (num ? (-10) : (-7));
		StoreReputation.ModReputation(trackedClient.clientFaction, value);
		if (num)
		{
			trackedClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		}
		Dialogue dialogue = CreateFalseThreatenDialog();
		Dialogue d = dialogue;
		if (num)
		{
			d = AppendClientLine(d, leaveLines);
		}
		d = AppendClientLine(d, talkBadLines);
		if (num)
		{
			d.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		}
		DialogUIManager.Instance.StartDialogue(dialogue);
	}

	private StoreClient GetClient()
	{
		if (PlayerStore.Instance == null)
		{
			return null;
		}
		if (PlayerStore.Instance.currentClientInstance == null)
		{
			return null;
		}
		return PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
	}

	private void Refresh()
	{
		StoreClient client = GetClient();
		if (trackedClient != client)
		{
			trackedClient = client;
			baseTradeRepMultiplier = client?.tradeReputationMultiplier ?? 1.0;
			client?.ClearTradeRepMultiplierOverrides();
			discountOffered = false;
			discountPercent = 0;
			markupOffered = false;
			markupPercent = 0;
			markupApplied = false;
			blackmailOffered = false;
			blackmailApplied = false;
			buyingMarkupOffered = false;
			buyingMarkupPercent = 0;
			buyingDiscountOffered = false;
			buyingDiscountPercent = 0;
			buyingDiscountApplied = false;
			threatenOffered = false;
			threatenApplied = false;
			repPreviewActive = false;
		}
	}

	private void RecomputeTradeRepMultiplier()
	{
		if (trackedClient != null)
		{
			var (value, value2) = ComputeTradeRepMultipliers(threatenApplied, markupApplied, markupPercent, buyingDiscountApplied, buyingDiscountPercent, blackmailApplied, discountOffered, discountPercent, buyingMarkupOffered, buyingMarkupPercent);
			trackedClient.sellTradeRepMultiplierOverride = value;
			trackedClient.buyTradeRepMultiplierOverride = value2;
		}
	}

	private (double sell, double buy) ComputeTradeRepMultipliers(bool threaten, bool markupApplied, int markupPercent, bool buyingDiscountApplied, int buyingDiscountPercent, bool blackmailApplied, bool discountOffered, int discountPercent, bool buyingMarkupOffered, int buyingMarkupPercent)
	{
		double item = ComputeDirectionalMultiplier(threaten, blackmailApplied, markupApplied, markupPercent, discountOffered, discountPercent);
		double item2 = ComputeDirectionalMultiplier(threaten, blackmailApplied, buyingDiscountApplied, buyingDiscountPercent, buyingMarkupOffered, buyingMarkupPercent);
		return (sell: item, buy: item2);
	}

	private double ComputeDirectionalMultiplier(bool threaten, bool blackmailApplied, bool badApplied, int badPercent, bool niceOffered, int nicePercent)
	{
		if (threaten)
		{
			return 0.0;
		}
		float num = 0f;
		if (badApplied)
		{
			num += GetBadBargainPenalty(badPercent);
		}
		if (blackmailApplied)
		{
			num += 6f;
		}
		if (num > 0f)
		{
			return 0f - num;
		}
		double num2 = baseTradeRepMultiplier;
		if (niceOffered && baseTradeRepMultiplier > 0.0)
		{
			num2 += (double)GetNiceBargainBonus(nicePercent);
		}
		return num2;
	}

	private (double sell, double buy) ComputePreviewMultipliers(BargainType type, int percent)
	{
		bool threaten = threatenApplied;
		bool flag = markupApplied;
		int num = markupPercent;
		bool flag2 = buyingDiscountApplied;
		int num2 = buyingDiscountPercent;
		bool flag3 = blackmailApplied;
		bool flag4 = discountOffered;
		int num3 = discountPercent;
		bool flag5 = buyingMarkupOffered;
		int num4 = buyingMarkupPercent;
		int num5 = Mathf.Abs(percent);
		switch (type)
		{
		case BargainType.Discount:
			flag4 = true;
			num3 = num5;
			break;
		case BargainType.Markup:
			flag = true;
			num = num5;
			break;
		case BargainType.BuyingMarkup:
			flag5 = true;
			num4 = num5;
			break;
		case BargainType.BuyingDiscount:
			flag2 = true;
			num2 = num5;
			break;
		case BargainType.Blackmail:
			flag3 = true;
			break;
		case BargainType.Threaten:
			threaten = true;
			break;
		}
		return ComputeTradeRepMultipliers(threaten, flag, num, flag2, num2, flag3, flag4, num3, flag5, num4);
	}

	private static bool BargainAffectsSellSide(BargainType type)
	{
		if (type != 0 && type != BargainType.Markup && type != BargainType.Blackmail)
		{
			return type == BargainType.Threaten;
		}
		return true;
	}

	private static bool BargainAffectsBuySide(BargainType type)
	{
		if (type != BargainType.BuyingMarkup && type != BargainType.BuyingDiscount && type != BargainType.Blackmail)
		{
			return type == BargainType.Threaten;
		}
		return true;
	}

	private bool CanOffer(BargainType type)
	{
		return type switch
		{
			BargainType.Discount => CanOfferDiscount(), 
			BargainType.Markup => CanOfferMarkup(), 
			BargainType.BuyingMarkup => CanOfferBuyingMarkup(), 
			BargainType.BuyingDiscount => CanDemandBuyingDiscount(), 
			BargainType.Blackmail => CanBlackmail(), 
			BargainType.Threaten => CanThreatenAug(), 
			_ => false, 
		};
	}

	public void SetRepPreview(BargainType type, int percent)
	{
		repPreviewActive = true;
		repPreviewType = type;
		repPreviewPercent = percent;
	}

	public void ClearRepPreview()
	{
		repPreviewActive = false;
	}

	public int GetBargainSuccessChance(BargainType type, int percent)
	{
		Refresh();
		if (trackedClient == null)
		{
			return 0;
		}
		int num = Mathf.Abs(percent);
		switch (type)
		{
		case BargainType.Discount:
		case BargainType.BuyingMarkup:
			return 100;
		case BargainType.Markup:
		case BargainType.BuyingDiscount:
		{
			if (trackedClient.alwaysRejectBadBargain)
			{
				return 0;
			}
			int factionRepBonus = GetFactionRepBonus(trackedClient.clientFaction);
			int num4 = (IsEventCustomer() ? 15 : 0);
			return Mathf.Clamp(Mathf.RoundToInt(100f - (float)num * 2f + (float)factionRepBonus + (float)num4 + (float)GetDealMakerBonus()), 0, 100);
		}
		case BargainType.Blackmail:
		{
			if (trackedClient.alwaysRejectBadBargain)
			{
				return 0;
			}
			int num2 = ((trackedClient.clientFaction != null) ? GetFactionRepBonusBlackmail(trackedClient.clientFaction) : 0);
			int num3 = (IsEventCustomer() ? 15 : 0);
			return Mathf.Clamp(60 + num2 + num3 + GetDealMakerBonus(), 0, 100);
		}
		case BargainType.Threaten:
			return 100;
		default:
			return 0;
		}
	}

	public int GetBargainLeaveChance(BargainType type, int percent)
	{
		Refresh();
		if (trackedClient == null)
		{
			return 0;
		}
		if (!BargainCanLeave(type))
		{
			return 0;
		}
		if (trackedClient.staysAfterFailedBargain)
		{
			return 0;
		}
		int num = Mathf.Abs(percent);
		int factionRepBonus = GetFactionRepBonus(trackedClient.clientFaction);
		int num2 = (IsEventCustomer() ? 15 : 0);
		int a = ((!trackedClient.alwaysRejectBadBargain) ? Mathf.RoundToInt(100f - (float)num * 2f + (float)factionRepBonus + (float)num2 + (float)GetDealMakerBonus()) : 0);
		int b = Mathf.RoundToInt(100f - (float)num * 1.2f + (float)factionRepBonus + (float)num2);
		int num3 = Mathf.Max(a, b);
		return Mathf.Clamp(100 - num3, 0, 100);
	}

	private static bool BargainUsesRoll(BargainType type)
	{
		if (type != BargainType.Markup && type != BargainType.BuyingDiscount && type != BargainType.Blackmail)
		{
			return type == BargainType.Threaten;
		}
		return true;
	}

	private static bool BargainCanLeave(BargainType type)
	{
		if (type != BargainType.Markup)
		{
			return type == BargainType.BuyingDiscount;
		}
		return true;
	}

	public string GetBargainTooltip(BargainType type, int percent)
	{
		int bargainSuccessChance = GetBargainSuccessChance(type, percent);
		if (!BargainUsesRoll(type))
		{
			return LocHelper.GetLocalizedMechanic("mech_bargain_always_accepted");
		}
		int bargainLeaveChance = GetBargainLeaveChance(type, percent);
		int num = Mathf.Max(0, 100 - bargainSuccessChance - bargainLeaveChance);
		string localizedMechanic = LocHelper.GetLocalizedMechanic("mech_bargain_accepts", bargainSuccessChance);
		if (BargainCanLeave(type))
		{
			localizedMechanic = localizedMechanic + "\n" + LocHelper.GetLocalizedMechanic("mech_bargain_refuses", num);
			localizedMechanic = localizedMechanic + "\n" + LocHelper.GetLocalizedMechanic("mech_bargain_walks_out", bargainLeaveChance);
		}
		else
		{
			localizedMechanic = localizedMechanic + "\n" + LocHelper.GetLocalizedMechanic("mech_bargain_refuses", num);
		}
		int dealMakerBonus = GetDealMakerBonus();
		if (dealMakerBonus > 0 && trackedClient != null && !trackedClient.alwaysRejectBadBargain)
		{
			localizedMechanic = localizedMechanic + "\n\n" + LocHelper.GetLocalizedMechanic("mech_bargain_deal_maker_bonus", dealMakerBonus);
		}
		return localizedMechanic;
	}

	private static float GetBadBargainPenalty(int percent)
	{
		if (percent <= 10)
		{
			return 0.5f;
		}
		if (percent <= 25)
		{
			return 1.5f;
		}
		if (percent <= 50)
		{
			return 3f;
		}
		return 5f;
	}

	private static float GetNiceBargainBonus(int percent)
	{
		if (percent <= 10)
		{
			return 0.65f;
		}
		if (percent <= 25)
		{
			return 1.2f;
		}
		if (percent <= 50)
		{
			return 2f;
		}
		return 2.9f;
	}

	public static int GetPreBargainValue(GameItem item, int value)
	{
		if (item == null)
		{
			return value;
		}
		double num = 1.0;
		string[] bARGAIN_FEATURE_IDS = BARGAIN_FEATURE_IDS;
		foreach (string id in bARGAIN_FEATURE_IDS)
		{
			ItemFeature itemFeature = item.FindItemFeatureByID(id);
			if (itemFeature != null && !itemFeature.isDisabled)
			{
				num *= Math.Max(0.0, 1.0 + (double)itemFeature.GetCurrentValueMod() / 100.0);
			}
		}
		if (num <= 0.0)
		{
			return value;
		}
		return (int)Math.Round((double)value / num);
	}

	private void ApplyOwnedItemFeature(Func<ItemFeature> creator, string featureId)
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem) && childItem.FindItemFeatureByID(featureId) == null)
			{
				childItem.AddItemFeature(creator());
			}
		}
		StoreClient storeClient = trackedClient;
		storeClient.onItemAddedToWeighted = (Action<GameItem>)Delegate.Combine(storeClient.onItemAddedToWeighted, (Action<GameItem>)delegate(GameItem item)
		{
			if (GeneralHelper.IsItemOwned(item) && item.FindItemFeatureByID(featureId) == null)
			{
				item.AddItemFeature(creator());
			}
		});
	}

	private void ApplyNonOwnedItemFeature(Func<ItemFeature> creator, string featureId)
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			if (!GeneralHelper.IsItemOwned(childItem) && childItem.FindItemFeatureByID(featureId) == null)
			{
				childItem.AddItemFeature(creator());
			}
		}
		foreach (GameItem childItem2 in EmporiumEntry.Instance.frontInvinvElement.childItems)
		{
			if (!GeneralHelper.IsItemOwned(childItem2) && childItem2.FindItemFeatureByID(featureId) == null)
			{
				childItem2.AddItemFeature(creator());
			}
		}
		StoreClient storeClient = trackedClient;
		storeClient.onItemAddedToCounters = (Action<GameItem>)Delegate.Combine(storeClient.onItemAddedToCounters, (Action<GameItem>)delegate(GameItem item)
		{
			if (!GeneralHelper.IsItemOwned(item) && item.FindItemFeatureByID(featureId) == null)
			{
				item.AddItemFeature(creator());
			}
		});
	}

	private static void InitializeDialogueLines()
	{
		acceptGratefulLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_grateful_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_grateful_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_grateful_3")
		};
		acceptVeryGratefulLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_very_grateful_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_very_grateful_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_very_grateful_3")
		};
		acceptReluctantLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_reluctant_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_reluctant_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_reluctant_3")
		};
		acceptReluctantEventLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_reluctant_event_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_reluctant_event_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_reluctant_event_3")
		};
		acceptReluctantAugLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_reluctant_aug_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_reluctant_aug_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_reluctant_aug_3")
		};
		acceptVeryReluctantLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_very_reluctant_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_very_reluctant_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_accept_very_reluctant_3")
		};
		rejectLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_reject_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_reject_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_reject_3")
		};
		hardRejectLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_hard_reject_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_hard_reject_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_hard_reject_3")
		};
		leaveLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_leave_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_leave_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_leave_3")
		};
		budgetIncreaseLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_budget_increase_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_budget_increase_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_budget_increase_3")
		};
		budgetDecreaseLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_budget_decrease_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_budget_decrease_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_budget_decrease_3")
		};
		talkBadLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_talk_bad_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_talk_bad_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_talk_bad_3")
		};
		innocentBlackmailLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_innocent_blackmail_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_innocent_blackmail_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_innocent_blackmail_3")
		};
		falseThreatenLines = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_bargain_false_threaten_1"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_false_threaten_2"),
			LocHelper.GetLocalizedDialogue("dialog_bargain_false_threaten_3")
		};
	}

	private Dialogue CreateAcceptGratefulDialog(int percent)
	{
		if (percent >= 50)
		{
			return CreateClientDialog(acceptVeryGratefulLines);
		}
		return CreateClientDialog(acceptGratefulLines);
	}

	private Dialogue CreateAcceptReluctantDialog(int percent)
	{
		if (trackedClient != null && trackedClient.isAug)
		{
			return CreateClientDialog(acceptReluctantAugLines);
		}
		if (IsEventCustomer())
		{
			return CreateClientDialog(acceptReluctantEventLines);
		}
		if (percent >= 50)
		{
			return CreateClientDialog(acceptVeryReluctantLines);
		}
		return CreateClientDialog(acceptReluctantLines);
	}

	private Dialogue CreateRejectDialog(int percent)
	{
		if (percent >= 50)
		{
			return CreateClientDialog(hardRejectLines);
		}
		return CreateClientDialog(rejectLines);
	}

	private Dialogue CreateLeaveDialog()
	{
		return CreateClientDialog(leaveLines);
	}

	private Dialogue CreateInnocentBlackmailDialog()
	{
		return CreateClientDialog(innocentBlackmailLines);
	}

	private Dialogue CreateFalseThreatenDialog()
	{
		return CreateClientDialog(falseThreatenLines);
	}

	private static Dialogue CreateClientDialog(List<string> lines)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 6;
		dialogue.SetText("CLIENT", lines[RNG.GetRandomInt(0, lines.Count - 1)]);
		return dialogue;
	}

	private static Dialogue AppendClientLine(Dialogue d, List<string> lines)
	{
		return d.NextDialogue().SetText("CLIENT", lines[RNG.GetRandomInt(0, lines.Count - 1)]);
	}

	public void OpenUI()
	{
		if (!(uiObject == null))
		{
			DockToTablet();
			uiObject.SetActive(value: true);
			builtLayoutKey = null;
			RefreshUI();
			EmporiumAudioPlayer.Instance.PlayMatched();
			TutorialUIManager.Instance.OnBargainUIOpened();
		}
	}

	public void CloseUI()
	{
		if (!(uiObject == null) && uiObject.activeSelf)
		{
			uiObject.SetActive(value: false);
			EmporiumAudioPlayer.Instance.PlayUnMatched();
			TutorialUIManager.Instance.OnBargainUIClosed();
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

	private void DockToTablet()
	{
		GameObject gameObject = ((NegociationUIManager.Instance != null) ? NegociationUIManager.Instance.uiObject : null);
		if (!(gameObject == null))
		{
			RectTransform rectTransform = gameObject.transform as RectTransform;
			RectTransform rectTransform2 = uiObject.transform as RectTransform;
			if (rectTransform2.parent != rectTransform.parent)
			{
				rectTransform2.SetParent(rectTransform.parent, worldPositionStays: false);
			}
			float x = rectTransform.localScale.x;
			float num = 476f;
			rectTransform2.anchorMin = rectTransform.anchorMin;
			rectTransform2.anchorMax = rectTransform.anchorMax;
			rectTransform2.pivot = rectTransform.pivot;
			rectTransform2.anchoredPosition = rectTransform.anchoredPosition + new Vector2(num * x, 0f);
			rectTransform2.localScale = new Vector3(x, x, 1f);
			follower.SetTarget(rectTransform, 1f, x);
			dock.tablet = rectTransform;
		}
	}
}

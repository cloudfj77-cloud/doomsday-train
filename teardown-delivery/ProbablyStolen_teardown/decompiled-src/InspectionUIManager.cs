using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectionUIManager : MonoBehaviour
{
	public GameObject panel;

	public GameItem currentInspectingItem;

	private string pendingConfrontItemId = "";

	private string pendingConfrontType = "";

	public CigaretteUI cigaretteUISmall;

	public CigaretteUI cigaretteUILarge;

	public FuelUI fuelUISmall;

	public FuelUI fuelUILarge;

	public StampUI stampUISmall;

	public StampUI stampUILarge;

	public InjectorUI injectorUISmall;

	public InjectorUI injectorUILarge;

	public RibwichUI ribwichUISmall;

	public RibwichUI ribwichUILarge;

	public AppHandUI appHandUISmall;

	public AppHandUI appHandUILarge;

	public InsBerryUI insBerryUISmall;

	public InsBerryUI insBerryUILarge;

	public GameObject confrontPanel;

	public Toggle toggle1;

	public Toggle toggle2;

	public Toggle toggle3;

	public Toggle toggle4;

	public Toggle toggle5;

	private TextMeshProUGUI toggle1TMP;

	private TextMeshProUGUI toggle2TMP;

	private TextMeshProUGUI toggle3TMP;

	private TextMeshProUGUI toggle4TMP;

	private TextMeshProUGUI toggle5TMP;

	public GameObject magnifyingGlass;

	public GameObject blockerTray;

	public Vector3 cigaretteInitialPosition;

	public Vector3 fuelInitialPosition;

	public Vector3 stampInitialPosition;

	public Vector3 injectorInitialPosition;

	public Vector3 ribwichInitialPosition;

	public Vector3 appHandInitialPosition;

	public Vector3 insBerryInitialPosition;

	public TextMeshProUGUI confrontTitleTMP;

	public GameObject appraisalPanel;

	public TMP_InputField apprisalServiceInputField;

	public bool isEditingAppraisalField;

	public static InspectionUIManager Instance { get; private set; }

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
		cigaretteInitialPosition = cigaretteUILarge.transform.localPosition;
		fuelInitialPosition = fuelUILarge.transform.localPosition;
		stampInitialPosition = stampUILarge.transform.localPosition;
		injectorInitialPosition = injectorUILarge.transform.localPosition;
		ribwichInitialPosition = ribwichUILarge.transform.localPosition;
		appHandInitialPosition = appHandUILarge.transform.localPosition;
		insBerryInitialPosition = insBerryUILarge.transform.localPosition;
		toggle1TMP = toggle1.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
		toggle2TMP = toggle2.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
		toggle3TMP = toggle3.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
		toggle4TMP = toggle4.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
		toggle5TMP = toggle5.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		if (appraisalPanel.activeSelf)
		{
			UpdateAppraisalPanel();
		}
	}

	public void OpenUI()
	{
		panel.SetActive(value: true);
		magnifyingGlass.transform.localPosition = new Vector3(-20f, 0f, 0f);
		blockerTray.transform.localPosition = new Vector3(0f, 0f, 0f);
		cigaretteUILarge.transform.localPosition = cigaretteInitialPosition;
		fuelUILarge.transform.localPosition = fuelInitialPosition;
		stampUILarge.transform.localPosition = stampInitialPosition;
		injectorUILarge.transform.localPosition = injectorInitialPosition;
		ribwichUILarge.transform.localPosition = ribwichInitialPosition;
		appHandUILarge.transform.localPosition = appHandInitialPosition;
		insBerryUILarge.transform.localPosition = insBerryInitialPosition;
	}

	public void CloseUI()
	{
		currentInspectingItem = null;
		panel.SetActive(value: false);
		EmporiumAudioPlayer.Instance.audioButton1.Play();
	}

	public void CloseConfrontPanel()
	{
		if (panel.activeSelf && confrontPanel.activeSelf)
		{
			confrontPanel.SetActive(value: false);
		}
	}

	public void OnInspectItem(GameItem gameItem)
	{
		EmporiumEntry.Instance.frontInvinvElement.ClearHighlight();
		ResetUI();
		currentInspectingItem = gameItem;
		OpenUI();
		if (gameItem.IsTag("CIGARETTE_TAG"))
		{
			HandleCigarette(gameItem);
		}
		if (gameItem.IsTag("PLASMA_CELL_TAG"))
		{
			HandlePlasma(gameItem);
		}
		if (gameItem.IsTag("FOOD_STAMP_TAG"))
		{
			HandleStamp(gameItem);
		}
		if (gameItem.IsTag("INS_INJECTOR_TAG"))
		{
			HandleInjector(gameItem);
		}
		if (gameItem.IsTag("RIBWICH_TAG"))
		{
			HandleRibwich(gameItem);
		}
		if (gameItem.IsTag("HAND_TAG"))
		{
			HandleAppHand(gameItem);
		}
		if (gameItem.IsTag("BERRY_TAG"))
		{
			HandleInsBerry(gameItem);
		}
		if (gameItem.IsTag("APPRAISAL_ITEM"))
		{
			appraisalPanel.SetActive(value: true);
		}
	}

	public void ResetUI()
	{
		cigaretteUISmall.gameObject.SetActive(value: false);
		cigaretteUILarge.gameObject.SetActive(value: false);
		fuelUISmall.gameObject.SetActive(value: false);
		fuelUILarge.gameObject.SetActive(value: false);
		stampUISmall.gameObject.SetActive(value: false);
		stampUILarge.gameObject.SetActive(value: false);
		injectorUISmall.gameObject.SetActive(value: false);
		injectorUILarge.gameObject.SetActive(value: false);
		ribwichUISmall.gameObject.SetActive(value: false);
		ribwichUILarge.gameObject.SetActive(value: false);
		appHandUISmall.gameObject.SetActive(value: false);
		appHandUILarge.gameObject.SetActive(value: false);
		insBerryUISmall.gameObject.SetActive(value: false);
		insBerryUILarge.gameObject.SetActive(value: false);
		confrontPanel.SetActive(value: false);
		appraisalPanel.SetActive(value: false);
		ResetAllConfrontToggle();
	}

	public void HandleCigarette(GameItem gameItem)
	{
		cigaretteUISmall.gameObject.SetActive(value: true);
		cigaretteUILarge.gameObject.SetActive(value: true);
		cigaretteUISmall.OnInit(gameItem);
		cigaretteUILarge.OnInit(gameItem);
		if (!CanOpenConfrontPanel(gameItem))
		{
			confrontPanel.SetActive(value: false);
		}
		else
		{
			OpenCigaretteConfrontPanel(gameItem);
		}
	}

	public void HandlePlasma(GameItem gameItem)
	{
		fuelUISmall.gameObject.SetActive(value: true);
		fuelUILarge.gameObject.SetActive(value: true);
		fuelUISmall.OnInit(gameItem);
		fuelUILarge.OnInit(gameItem);
		PlasmaHelper.PrintDebugFuel(gameItem);
	}

	public void HandleRibwich(GameItem gameItem)
	{
		ribwichUISmall.gameObject.SetActive(value: true);
		ribwichUILarge.gameObject.SetActive(value: true);
		ribwichUISmall.OnInit(gameItem);
		ribwichUILarge.OnInit(gameItem);
	}

	public void HandleAppHand(GameItem gameItem)
	{
		appHandUISmall.gameObject.SetActive(value: true);
		appHandUILarge.gameObject.SetActive(value: true);
		appHandUISmall.OnInit(gameItem);
		appHandUILarge.OnInit(gameItem);
	}

	public void HandleInsBerry(GameItem gameItem)
	{
		insBerryUISmall.gameObject.SetActive(value: true);
		insBerryUILarge.gameObject.SetActive(value: true);
		insBerryUISmall.OnInit(gameItem);
		insBerryUILarge.OnInit(gameItem);
	}

	public void HandleStamp(GameItem gameItem)
	{
		stampUISmall.gameObject.SetActive(value: true);
		stampUILarge.gameObject.SetActive(value: true);
		stampUISmall.OnInit(gameItem);
		stampUILarge.OnInit(gameItem);
		if (!CanOpenConfrontPanel(gameItem))
		{
			confrontPanel.SetActive(value: false);
		}
		else
		{
			OpenStampConfrontPanel(gameItem);
		}
	}

	public void HandleInjector(GameItem gameItem)
	{
		injectorUISmall.gameObject.SetActive(value: true);
		injectorUILarge.gameObject.SetActive(value: true);
		injectorUISmall.OnInit(gameItem);
		injectorUILarge.OnInit(gameItem);
		if (!CanOpenConfrontPanel(gameItem))
		{
			confrontPanel.SetActive(value: false);
		}
		else
		{
			OpenInjectorConfrontPanel(gameItem);
		}
	}

	public static bool IsConfrontLocked()
	{
		return NegociationUIManager.IsTradeLocked();
	}

	private static bool CanOpenConfrontPanel(GameItem gameItem)
	{
		if (GeneralHelper.IsItemOwned(gameItem))
		{
			return false;
		}
		if (gameItem.IsTag("ALREADY_CONFRONTED_TAG"))
		{
			return false;
		}
		if (IsConfrontLocked())
		{
			return false;
		}
		return true;
	}

	public void OnConfront()
	{
		if (IsConfrontLocked() || PlayerStore.Instance.currentClientInstance == null || currentInspectingItem == null || !IsAtLeastOneIssueSelected())
		{
			return;
		}
		if (currentInspectingItem.IsTag("CIGARETTE_TAG"))
		{
			if (currentInspectingItem.FindItemFeatureByCategory("CATEGORY_GENUINE_CIGARETTE").GetCurrentItemCondition().identifier.Equals("genuineCigarette"))
			{
				pendingConfrontItemId = currentInspectingItem.identifier;
				pendingConfrontType = "confront";
				OnCigaretteConfront();
			}
			else
			{
				pendingConfrontItemId = currentInspectingItem.identifier;
				pendingConfrontType = "talk";
				OnCigaretteTalk();
			}
		}
		else if (currentInspectingItem.IsTag("FOOD_STAMP_TAG"))
		{
			if (currentInspectingItem.FindItemFeatureByCategory("CATEGORY_GENUINE_STAMP").GetCurrentItemCondition().identifier.Equals("genuineStamp"))
			{
				pendingConfrontItemId = currentInspectingItem.identifier;
				pendingConfrontType = "confront";
				OnStampConfront();
			}
			else
			{
				pendingConfrontItemId = currentInspectingItem.identifier;
				pendingConfrontType = "talk";
				OnStampTalk();
			}
		}
		else if (currentInspectingItem.IsTag("INS_INJECTOR_TAG") && currentInspectingItem.FindItemFeatureByCategory("CATEGORY_GENUINE_INJECTOR").GetCurrentItemCondition().identifier.Equals("genuineInjector"))
		{
			pendingConfrontItemId = currentInspectingItem.identifier;
			pendingConfrontType = "confront";
			OnInjectorConfront();
		}
	}

	public bool IsAtLeastOneIssueSelected()
	{
		if (!toggle1.isOn && !toggle2.isOn && !toggle3.isOn && !toggle4.isOn)
		{
			return toggle5.isOn;
		}
		return true;
	}

	public void OnCigaretteConfront()
	{
		string reason = "";
		if (toggle1.isOn)
		{
			reason = "CIGARETTE_LETTERING_ISSUE";
		}
		if (toggle2.isOn)
		{
			reason = "CIGARETTE_COLOR_ISSUE";
		}
		if (toggle3.isOn)
		{
			reason = "CIGARETTE_SEAL_ISSUE";
		}
		if (toggle4.isOn)
		{
			reason = "CIGARETTE_LOGO_ISSUE";
		}
		if (toggle5.isOn)
		{
			reason = "CIGARETTE_BOX_ISSUE";
		}
		Dialogue dialogue = InsCigaretteHelper.CreateConfrontDialogue(reason, currentInspectingItem);
		DialogUIManager.Instance.StartDialogue(dialogue);
		CloseUI();
	}

	public void OnStampConfront()
	{
		string reason = "";
		if (toggle1.isOn)
		{
			reason = "STAMP_LETTERING_ISSUE";
		}
		if (toggle2.isOn)
		{
			reason = "STAMP_LOGO_ISSUE";
		}
		if (toggle3.isOn)
		{
			reason = "STAMP_SEAL_ISSUE";
		}
		if (toggle4.isOn)
		{
			reason = "STAMP_AMOUNT_ISSUE";
		}
		if (toggle5.isOn)
		{
			reason = "STAMP_CHIP_ISSUE";
		}
		Dialogue dialogue = InsStampHelper.CreateConfrontDialogue(reason, currentInspectingItem);
		DialogUIManager.Instance.StartDialogue(dialogue);
		CloseUI();
	}

	public void OnCigaretteTalk()
	{
		Dialogue dialogue = InsCigaretteHelper.CreateTalkDialogue(currentInspectingItem);
		DialogUIManager.Instance.StartDialogue(dialogue);
		CloseUI();
	}

	public void OnInjectorConfront()
	{
		string reason = "";
		if (toggle1.isOn)
		{
			reason = "ISSUE_INDICATOR";
		}
		if (toggle2.isOn)
		{
			reason = "ISSUE_LABEL";
		}
		if (toggle3.isOn)
		{
			reason = "ISSUE_SERIAL";
		}
		if (toggle4.isOn)
		{
			reason = "ISSUE_BODY";
		}
		if (toggle5.isOn)
		{
			reason = "ISSUE_COLOR";
		}
		Dialogue dialogue = InsInjectorHelper.CreateConfrontDialogue(reason, currentInspectingItem);
		DialogUIManager.Instance.StartDialogue(dialogue);
		CloseUI();
	}

	public void OnStampTalk()
	{
		Dialogue dialogue = InsStampHelper.CreateTalkDialogue(currentInspectingItem);
		DialogUIManager.Instance.StartDialogue(dialogue);
		CloseUI();
	}

	public void OnConfrontSuccess()
	{
		EmporiumAnalytics.TrackConfront(pendingConfrontItemId, pendingConfrontType, "success");
	}

	public void OnConfrontFailed()
	{
		EmporiumAnalytics.TrackConfront(pendingConfrontItemId, pendingConfrontType, "fail");
		string text = PlayerStore.Instance.currentClientInstance?.GetClientBlueprint()?.clientFaction;
		if (string.IsNullOrEmpty(text) || StoreReputation.GetStoreReputation(text) == null)
		{
			text = "FACTION_LOWER_LEVEL";
		}
		StoreReputation.ModReputation(text, -2);
		EmporiumAudioPlayer.Instance.audioFail.Play();
	}

	public void OnGenuineClaim()
	{
		confrontTitleTMP.text = LocHelper.GetLocalizedUI("ui_inspection_panel_title") + " <link=\"ui_info_inspection_genuine_claim\"><sprite name=\"info\" tint=1></link>";
	}

	public void OnCounterfeitClaim()
	{
		confrontTitleTMP.text = LocHelper.GetLocalizedUI("ui_inspection_panel_title") + " <link=\"ui_info_inspection_counterfeit_claim\"><sprite name=\"info\" tint=1></link>";
	}

	public void OpenCigaretteConfrontPanel(GameItem gameItem)
	{
		confrontPanel.SetActive(value: true);
		ResetAllToggle();
		if (gameItem.FindItemFeatureByCategory("CATEGORY_GENUINE_CIGARETTE").GetCurrentItemCondition().identifier.Equals("genuineCigarette"))
		{
			toggle1.gameObject.SetActive(value: true);
			toggle2.gameObject.SetActive(value: true);
			toggle3.gameObject.SetActive(value: true);
			toggle4.gameObject.SetActive(value: true);
			toggle5.gameObject.SetActive(value: true);
			toggle1TMP.text = LocHelper.GetLocalizedUI("inspection_lettering_issues");
			toggle2TMP.text = LocHelper.GetLocalizedUI("inspection_color_issues");
			toggle3TMP.text = LocHelper.GetLocalizedUI("inspection_seal_issues");
			toggle4TMP.text = LocHelper.GetLocalizedUI("inspection_logo_issues");
			toggle5TMP.text = LocHelper.GetLocalizedUI("inspection_box_issues");
			OnGenuineClaim();
		}
		else
		{
			toggle1.gameObject.SetActive(value: true);
			toggle1TMP.text = LocHelper.GetLocalizedUI("inspection_genuine");
			OnCounterfeitClaim();
		}
	}

	public void OpenStampConfrontPanel(GameItem gameItem)
	{
		confrontPanel.SetActive(value: true);
		ResetAllToggle();
		if (gameItem.FindItemFeatureByCategory("CATEGORY_GENUINE_STAMP").GetCurrentItemCondition().identifier.Equals("genuineStamp"))
		{
			toggle1.gameObject.SetActive(value: true);
			toggle2.gameObject.SetActive(value: true);
			toggle3.gameObject.SetActive(value: true);
			toggle4.gameObject.SetActive(value: true);
			toggle5.gameObject.SetActive(value: true);
			toggle1TMP.text = LocHelper.GetLocalizedUI("inspection_lettering_issues");
			toggle2TMP.text = LocHelper.GetLocalizedUI("inspection_logo_issues");
			toggle3TMP.text = LocHelper.GetLocalizedUI("inspection_seal_issues");
			toggle4TMP.text = LocHelper.GetLocalizedUI("inspection_value_issues");
			toggle5TMP.text = LocHelper.GetLocalizedUI("inspection_chip_issues");
			OnGenuineClaim();
		}
		else
		{
			toggle1.gameObject.SetActive(value: true);
			toggle1TMP.text = LocHelper.GetLocalizedUI("inspection_genuine");
			OnCounterfeitClaim();
		}
	}

	public void OpenInjectorConfrontPanel(GameItem gameItem)
	{
		confrontPanel.SetActive(value: true);
		ResetAllToggle();
		if (gameItem.FindItemFeatureByCategory("CATEGORY_GENUINE_INJECTOR").GetCurrentItemCondition().identifier.Equals("genuineInjector"))
		{
			toggle1.gameObject.SetActive(value: true);
			toggle2.gameObject.SetActive(value: true);
			toggle3.gameObject.SetActive(value: true);
			toggle4.gameObject.SetActive(value: true);
			toggle5.gameObject.SetActive(value: true);
			toggle1TMP.text = LocHelper.GetLocalizedUI("inspection_indicator_issues");
			toggle2TMP.text = LocHelper.GetLocalizedUI("inspection_label_issues");
			toggle3TMP.text = LocHelper.GetLocalizedUI("inspection_serial_issues");
			toggle4TMP.text = LocHelper.GetLocalizedUI("inspection_body_issues");
			toggle5TMP.text = LocHelper.GetLocalizedUI("inspection_color_issues");
			OnGenuineClaim();
		}
		else
		{
			confrontPanel.SetActive(value: false);
		}
	}

	public void ResetAllToggle()
	{
		toggle1.gameObject.SetActive(value: false);
		toggle2.gameObject.SetActive(value: false);
		toggle3.gameObject.SetActive(value: false);
		toggle4.gameObject.SetActive(value: false);
		toggle5.gameObject.SetActive(value: false);
		toggle1TMP.text = "";
		toggle2TMP.text = "";
		toggle3TMP.text = "";
		toggle4TMP.text = "";
		toggle5TMP.text = "";
	}

	public void ResetAllConfrontToggle()
	{
		toggle1.isOn = false;
		toggle2.isOn = false;
		toggle3.isOn = false;
		toggle4.isOn = false;
		toggle5.isOn = false;
	}

	public void UpdateAppraisalPanel()
	{
		if (currentInspectingItem == null || !currentInspectingItem.IsTag("APPRAISAL_ITEM"))
		{
			appraisalPanel.SetActive(value: false);
			return;
		}
		if (PlayerStore.Instance.currentClientInstance == null)
		{
			appraisalPanel.SetActive(value: false);
			return;
		}
		if (PlayerStore.Instance.currentClientInstance.GetClientBlueprint().clientIntent != StoreClient.ClientIntent.APPRAISAL_SERVICE)
		{
			appraisalPanel.SetActive(value: false);
			return;
		}
		appraisalPanel.SetActive(value: true);
		if (!isEditingAppraisalField)
		{
			apprisalServiceInputField.text = currentInspectingItem.GetTagReadonly("APPRAISAL_DEFAULT_VALUE").GetInt().ToString();
		}
	}

	public void StartEditingAppraisalField()
	{
		isEditingAppraisalField = true;
		StoreUIManager.Instance.StartTyping();
	}

	public void FinishEditingAppraisalField()
	{
		isEditingAppraisalField = false;
		StoreUIManager.Instance.StopTyping();
		if (int.TryParse(apprisalServiceInputField.text, out var val))
		{
			currentInspectingItem.ModifyTag("APPRAISAL_DEFAULT_VALUE", delegate(TagState state)
			{
				state.Enable().SetInt(val);
			});
		}
	}

	public void ChangeAppraisalServicePrice(int mod)
	{
		if (int.TryParse(apprisalServiceInputField.text, out var val))
		{
			if (mod > 0)
			{
				val += mod;
			}
			else
			{
				val -= Math.Abs(mod);
			}
			currentInspectingItem.ModifyTag("APPRAISAL_DEFAULT_VALUE", delegate(TagState state)
			{
				state.Enable().SetInt(val);
			});
		}
	}

	public void OnValueChanged(string value)
	{
		if (int.TryParse(apprisalServiceInputField.text, out var val))
		{
			currentInspectingItem.ModifyTag("APPRAISAL_DEFAULT_VALUE", delegate(TagState state)
			{
				state.Enable().SetInt(val);
			});
		}
	}

	public void OnAppraisalServiceEstimated()
	{
		if (int.TryParse(apprisalServiceInputField.text, out var result))
		{
			AppraisalServiceHelper.OnEstimate(currentInspectingItem, result);
			CloseUI();
		}
	}
}

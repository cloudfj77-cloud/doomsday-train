using System.Collections.Generic;

public class ItemFeature
{
	public enum FeatureType
	{
		Normale,
		Event,
		Special,
		Temporary,
		TemporaryBuying,
		TemporarySelling,
		ClientTemporary
	}

	public enum ValueStage
	{
		Innate,
		Market,
		Final
	}

	public const string NEGO_TOOL_SCALE = "NEGO_TOOL_SCALE";

	[ES3Serializable]
	public FeatureType featureType;

	[ES3Serializable]
	public ValueStage valueStage;

	[ES3Serializable]
	public int parentItemUniqueId;

	[ES3Serializable]
	private string publicDisplay;

	[ES3Serializable]
	private string actualDisplay;

	[ES3Serializable]
	private string removedByTool;

	[ES3Serializable]
	private int valueModifier;

	[ES3Serializable]
	private bool initiallyShown = true;

	[ES3Serializable]
	private bool isFeatureExposed;

	[ES3Serializable]
	public bool isExposing;

	[ES3Serializable]
	private bool isFeatureDiscovered;

	[ES3Serializable]
	public ItemCondition fakeCondition;

	[ES3Serializable]
	public ItemCondition realCondition;

	[ES3Serializable]
	public bool useCondition;

	[ES3Serializable]
	private bool usePreExposeValue;

	[ES3Serializable]
	private int preExposeValueModifier;

	[ES3Serializable]
	public bool isExposable = true;

	[ES3Serializable]
	public bool isPublicHidden;

	[ES3Serializable]
	public int customIntValue1;

	[ES3Serializable]
	public bool isDisabled;

	private static readonly HashSet<string> legacyFinalStageIds = new HashSet<string> { "bargainDiscount", "bargainMarkup", "bargainBuyingMarkup", "bargainBuyingDiscount", "blackmailDiscount", "blackmailMarkup", "threatenDiscount", "threatenMarkup" };

	private static List<string> waterScamDiscoveredLine = new List<string> { "dialog_expose_water_scam_1", "dialog_expose_water_scam_2", "dialog_expose_water_scam_3" };

	[ES3Serializable]
	public string identifier { get; set; }

	[ES3Serializable]
	public string category { get; set; }

	[ES3Serializable]
	public bool isFeatureMatch { get; set; }

	public ItemFeature()
	{
		featureType = FeatureType.Normale;
		category = "";
	}

	public ItemFeature(string category)
	{
		this.category = category;
		featureType = FeatureType.Normale;
	}

	public ItemFeature(FeatureType featureType)
	{
		category = "";
		this.featureType = featureType;
	}

	public ValueStage GetValueStage()
	{
		if (valueStage == ValueStage.Innate)
		{
			if (featureType == FeatureType.Event)
			{
				return ValueStage.Market;
			}
			if (identifier != null && legacyFinalStageIds.Contains(identifier))
			{
				return ValueStage.Final;
			}
		}
		return valueStage;
	}

	public ItemFeature SetValueStage(ValueStage stage)
	{
		valueStage = stage;
		return this;
	}

	public bool CanExposeFeature()
	{
		if (PlayerStore.Instance.currentClientInstance != null)
		{
			return true;
		}
		return false;
	}

	public void ExposeFeature(bool lowkey = false, bool playBad = false)
	{
		SetExposed(value: true);
		isExposing = false;
		if (!lowkey)
		{
			EmporiumAudioPlayer.Instance.PlayBeatVictory();
		}
		if (playBad)
		{
			EmporiumAudioPlayer.Instance.audioExposeScam.Play();
		}
		NegociationUIManager.Instance.RefreshUI();
	}

	public bool IsInitiallyShown()
	{
		return initiallyShown;
	}

	public bool IsExposed()
	{
		return isFeatureExposed;
	}

	public int GetActualValueModifier()
	{
		if (useCondition)
		{
			return realCondition.modValue;
		}
		return valueModifier;
	}

	public int GetFakeValueModifier()
	{
		if (useCondition)
		{
			return fakeCondition.modValue;
		}
		return preExposeValueModifier;
	}

	public string GetPublicDisplay()
	{
		_ = usePreExposeValue;
		return "-" + GetBaseFakeDisplay() + GetFormattedFakeModifier();
	}

	public string GetActualDisplay()
	{
		return "-" + GetBaseRealDisplay();
	}

	public string GetFormattedFakeModifier()
	{
		string text = "";
		if (GetFakeValueModifier() > 0)
		{
			text = "+";
		}
		return " (" + text + GetFakeValueModifier() + "%)";
	}

	public string GetFormattedActualModifier()
	{
		string text = "";
		if (realCondition.modValue > 0)
		{
			text = "+";
		}
		return " (" + text + GetActualValueModifier() + "%)";
	}

	public string GetExposedString()
	{
		if (IsFeatureMatch())
		{
			return "<color=#39a78e>" + GetPublicDisplay() + LocHelper.GetLocalizedMechanic("mech_feature_match") + "</color>";
		}
		if (useCondition)
		{
			if (realCondition.modValue == 0)
			{
				if (!realCondition.hiddenAsPublic)
				{
					return "<color=#808080><s>" + GetPublicDisplay() + "</s> </color> \n " + GetBaseRealDisplay();
				}
				return GetActualDisplay();
			}
			if (!realCondition.hiddenAsPublic)
			{
				return "<color=#808080><s>" + GetPublicDisplay() + "</s> </color> \n " + GetBaseRealDisplay() + GetFormattedActualModifier();
			}
			return GetActualDisplay() + GetFormattedActualModifier();
		}
		if (valueModifier == 0)
		{
			if (!GetPublicHidden())
			{
				return "<color=#808080><s>" + GetPublicDisplay() + "</s> </color> \n " + GetBaseRealDisplay();
			}
			return GetActualDisplay();
		}
		if (!GetPublicHidden())
		{
			return "<color=#808080><s>" + GetPublicDisplay() + "</s> </color> \n " + GetBaseRealDisplay() + GetFormattedActualModifier();
		}
		return GetActualDisplay() + GetFormattedActualModifier();
	}

	public string GetBaseCurrentDisplay()
	{
		if (useCondition)
		{
			if (IsExposed())
			{
				return GetBaseRealDisplay();
			}
			return GetBaseFakeDisplay();
		}
		if (IsExposed())
		{
			return actualDisplay;
		}
		return publicDisplay;
	}

	public string GetBaseRealDisplay()
	{
		if (!useCondition)
		{
			return actualDisplay;
		}
		return realCondition.display;
	}

	public string GetBaseFakeDisplay()
	{
		if (!useCondition)
		{
			return publicDisplay;
		}
		return fakeCondition.display;
	}

	public void DiscoverFeature()
	{
		isFeatureDiscovered = true;
		NotebookUIManager.Instance.UpdateUI();
		NotebookUIManager.Instance.OnFeatureDiscovered();
	}

	public bool IsFeatureDiscovered()
	{
		return isFeatureDiscovered;
	}

	public void SetFeatureDiscovered(bool value)
	{
		isFeatureDiscovered = value;
	}

	public static ItemFeature CreateFeatureFromNegociationData(NegociationData negociationData)
	{
		ItemFeature itemFeature = new ItemFeature(FeatureType.Event);
		itemFeature.usePreExposeValue = true;
		itemFeature.preExposeValueModifier = negociationData.value;
		itemFeature.publicDisplay = LocHelper.GetLocalizedMechanic("mech_market_price_change", TypeHelper.GetTypeDisplayName(negociationData.itemType), negociationData.reason);
		itemFeature.valueModifier = negociationData.value;
		itemFeature.identifier = negociationData.identifier;
		itemFeature.featureType = FeatureType.Event;
		itemFeature.valueStage = ValueStage.Market;
		itemFeature.isExposable = false;
		return itemFeature;
	}

	public static void CreateAndAddFeatureFromEvent(GameItem gameItem, StoreEvent storeEvent)
	{
		foreach (NegociationData negociationData in storeEvent.negociationDatas)
		{
			foreach (string item in gameItem.GetGameItemType())
			{
				if (item.Equals(negociationData.itemType) && !gameItem.IsAlreadyContainFeatureWithId(negociationData.identifier))
				{
					ItemFeature itemFeature = CreateFeatureFromNegociationData(negociationData);
					gameItem.AddItemFeature(itemFeature);
					break;
				}
			}
		}
	}

	public ItemFeature SetPublicDisplay(string value)
	{
		publicDisplay = value;
		return this;
	}

	public ItemFeature SetActualDisplay(string value)
	{
		actualDisplay = value;
		return this;
	}

	public ItemFeature SetValueModifier(int value)
	{
		valueModifier = value;
		return this;
	}

	public ItemFeature SetExposedValueMod(int value)
	{
		valueModifier = value;
		return this;
	}

	public ItemFeature SetPreExposedValueMod(int value, bool flag = true)
	{
		preExposeValueModifier = value;
		usePreExposeValue = flag;
		return this;
	}

	public ItemCondition GetCurrentItemCondition()
	{
		if (IsExposed())
		{
			return realCondition;
		}
		return fakeCondition;
	}

	public int GetCurrentValueMod()
	{
		if (!useCondition)
		{
			if (IsExposed())
			{
				return valueModifier;
			}
			if (usePreExposeValue)
			{
				return preExposeValueModifier;
			}
			return 0;
		}
		if (IsExposed())
		{
			return realCondition.modValue;
		}
		return fakeCondition.modValue;
	}

	public ItemFeature SetTool(string value)
	{
		removedByTool = value;
		return this;
	}

	public string GetToolNeeded()
	{
		return removedByTool;
	}

	public Dialogue GetExposeDialog()
	{
		if (category == "DURABILITY_TAG")
		{
			return DurabilityHelper.GetExposeDialog(this);
		}
		if (category == "CATEGORY_WATER_PURITY")
		{
			return WaterFeatureHelper.GetWaterFeatureDialog(this);
		}
		if (category == "CATEGORY_CHEMICAL_PURITY")
		{
			return ChemicalProductHelper.GetChemicalFeatureDialog(this);
		}
		Debug.Log("ERROR: ExposeDialog not found for this feature");
		return null;
	}

	public GameItem GetParentItem()
	{
		return EmporiumEntry.Instance.FindItemByUniqueID(parentItemUniqueId);
	}

	public Dialogue GetClientExposeDialog()
	{
		Dialogue dialogue = CreateCustomerConfrontDialog();
		dialogue.priority = 6;
		if (dialogue == null)
		{
			Debug.Log("ERROR onClientExposedDialogue not found");
		}
		return dialogue;
	}

	public void SetExposed(bool value)
	{
		isFeatureExposed = value;
		if (useCondition)
		{
			if (isFeatureExposed && realCondition.HasOnActivateAction())
			{
				realCondition.ExecuteOnActivateAction(EmporiumEntry.Instance.FindItemByUniqueID(parentItemUniqueId));
			}
			else if (!isFeatureExposed && fakeCondition.HasOnActivateAction())
			{
				fakeCondition.ExecuteOnActivateAction(EmporiumEntry.Instance.FindItemByUniqueID(parentItemUniqueId));
			}
		}
	}

	public bool WillClientExposeFeature()
	{
		if (useCondition)
		{
			if (realCondition.modValue < fakeCondition.modValue)
			{
				return true;
			}
			return false;
		}
		if (usePreExposeValue)
		{
			if (valueModifier < preExposeValueModifier)
			{
				return false;
			}
			return true;
		}
		if (valueModifier > 0)
		{
			return true;
		}
		return false;
	}

	public bool IsRealFeatureCheaper()
	{
		if (useCondition)
		{
			if (fakeCondition.modValue == 0 && realCondition.modValue == 0)
			{
				return fakeCondition.compareValue > realCondition.compareValue;
			}
			return fakeCondition.modValue > realCondition.modValue;
		}
		if (usePreExposeValue)
		{
			if (preExposeValueModifier > valueModifier)
			{
				return false;
			}
			return true;
		}
		if (valueModifier > 0)
		{
			return true;
		}
		return false;
	}

	public void SetPublicHidden(bool val)
	{
		isPublicHidden = val;
	}

	public bool GetPublicHidden()
	{
		if (useCondition)
		{
			return fakeCondition.hiddenAsPublic;
		}
		return isPublicHidden;
	}

	public bool IsFeatureMatch()
	{
		if (useCondition)
		{
			if (realCondition.identifier.Equals(fakeCondition.identifier))
			{
				return true;
			}
			return false;
		}
		return isFeatureMatch;
	}

	public string GetCategory()
	{
		if (category != null)
		{
			return category;
		}
		return "";
	}

	public bool IsCategory(string cate)
	{
		if (GetCategory().Equals(cate))
		{
			return true;
		}
		return false;
	}

	public void SetConditions(ItemCondition fakeCondition, ItemCondition realCondition)
	{
		useCondition = true;
		this.realCondition = realCondition;
		this.fakeCondition = fakeCondition;
	}

	public static string GetRandomWaterScamDiscoveredLine()
	{
		return LocHelper.GetLocalizedDialogue(waterScamDiscoveredLine[RNG.GetRandomInt(0, waterScamDiscoveredLine.Count - 1)]);
	}

	public Dialogue CreateCustomerConfrontDialog()
	{
		Dialogue dialogue = null;
		if (category.Equals("CATEGORY_GENUINE_CIGARETTE"))
		{
			dialogue = InsCigaretteHelper.GetCustomerConfrontDialogue(EmporiumEntry.Instance.FindItemByUniqueID(parentItemUniqueId));
		}
		else if (category.Equals("CATEGORY_OPEN_BOX"))
		{
			dialogue = new Dialogue();
		}
		else if (category.Equals("CATEGORY_RUSTED"))
		{
			dialogue = new Dialogue();
		}
		else if (category.Equals("CATEGORY_WATER_PURITY"))
		{
			string baseRealDisplay = GetBaseRealDisplay();
			if (baseRealDisplay.Equals(WaterFeatureHelper.GUTTERFLOW_WATER_DISPLAY))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_water_gutterflow"));
			}
			else if (baseRealDisplay.Equals(WaterFeatureHelper.RUST_WATER_DISPLAY))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_water_rustwater"));
			}
			else if (baseRealDisplay.Equals(WaterFeatureHelper.GHOSTWATER_DISPLAY))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_water_ghostwater"));
			}
			else if (baseRealDisplay.Equals(WaterFeatureHelper.BASE_WATER_DISPLAY))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_water_basewater"));
			}
			else if (baseRealDisplay.Equals(WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_water_highquality"));
			}
			else
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", GetRandomWaterScamDiscoveredLine());
			}
		}
		else if (category.Equals("CATEGORY_GENUINE_INJECTOR"))
		{
			if (realCondition.identifier.Equals("unusableInjector"))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_injector_unusable"));
			}
			else if (realCondition.identifier.Equals("expiredInjector"))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_injector_expired"));
			}
			else
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_injector_label_wrong"));
			}
		}
		else if (category.Equals("CATEGORY_CHEMICAL_PURITY"))
		{
			if (realCondition.identifier.Equals("pharma_grade"))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_chemical_pharma_grade"));
			}
			else if (realCondition.identifier.Equals("near_pharma"))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_chemical_near_pharma"));
			}
			else if (realCondition.identifier.Equals("street_standard"))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_chemical_street_standard"));
			}
			else if (realCondition.identifier.Equals("cut_product"))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_chemical_cut_product"));
			}
			else if (realCondition.identifier.Equals("junk_grade"))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_chemical_junk_grade"));
			}
			else if (realCondition.identifier.Equals("starch_grade"))
			{
				dialogue = new Dialogue();
				dialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_expose_chemical_starch_grade"));
			}
		}
		if (dialogue == null || dialogue.text == null)
		{
			dialogue = new Dialogue();
			dialogue.SetText("CLIENT", LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_expose_label_generic", "That label is a lie. I'll count it for what it really is."));
		}
		return dialogue;
	}

	public void StartExposing()
	{
		isExposing = true;
	}
}

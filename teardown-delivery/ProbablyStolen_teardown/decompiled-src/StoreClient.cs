using System;
using System.Collections.Generic;
using System.Linq;

public class StoreClient
{
	public enum AugIdleTell
	{
		Default,
		Normal,
		Freeze,
		Seizure
	}

	public enum ClientIntent
	{
		UNDEFINED,
		BUY,
		SELL,
		SELLNBUY,
		INSPECTION,
		DIALOGUE,
		BARTER,
		RENT,
		LOAN_SHARK,
		SPECIAL,
		INFORMATION_DEALER,
		PROCUREMENT_OFFER,
		PROCUREMENT_COLLECT,
		WHOLESALE,
		APPRAISAL_SERVICE,
		GUNSMITH,
		EXPEDITION
	}

	public const string FACTION_LOWER = "FACTION_LOWER_LEVEL";

	public const string FACTION_UPPER = "FACTION_UPPER_LEVEL";

	public const string FACTION_MIDDLE = "FACTION_MIDDLE";

	public const string FACTION_SECURITY = "FACTION_SECURITY";

	public const string FACTION_CRIME = "FACTION_BLACK_MARKET";

	public const string FACTION_TOURIST = "FACTION_TOURIST";

	public string displayName = "DEFAULT DISPLAY NAME";

	public string identifier = "NO_ID";

	public string sourceModId;

	public string spriteName = "";

	public List<string> possibleSprites;

	public int clientCash;

	private int clientBudget;

	public bool useClientBudget;

	public int buyPriceModifier;

	public int sellPriceModifier;

	public List<string> clientBuyingTagList;

	public List<string> clientBuyingIdList;

	public List<string> clientBlackTagList;

	public List<string> clientBlackIdList;

	public bool dismissable = true;

	public bool arrestable = true;

	public bool nonDismissableArrestable;

	public bool nonShootable;

	public Dialogue mainDialogue;

	public bool isMainDialogueStarted;

	public Dialogue acceptDealDialogue;

	public bool acceptDealDialoguePlayed;

	public Dialogue acceptDealDialogueLastLine;

	public Dialogue placedWrongItemWhenSellingToDialogue;

	public Dialogue placeRightItemWhenSellingToDialogue;

	public Dialogue allDoneDialogue;

	public Dialogue repeatDialogue;

	public Dialogue interogationDialogue;

	public Dialogue glasseDialogue;

	public Dialogue customOnArrestDialogue;

	public bool isIntroduced;

	public BarterOffer barterOffer;

	public ProcurementData procurementData;

	public bool isBusinessComplete;

	public int serviceCost;

	public bool isOfferWholesale;

	public int wholesaleDiscount = -15;

	public bool isRegularSellDisable;

	public bool isRequireVariety;

	public List<string> varietyTrackingList;

	public int cooldown = 1;

	public bool cooldownStartsOnArrival;

	public Func<ItemFeature, StoreClient, bool> canClientExposeFeature;

	public Action<GameItem> onItemAddedToWeighted;

	public Action<GameItem> onItemAddedToClient;

	public Action<GameItem> OnItemAddedToShowcase;

	public Action<GameItem> onItemRemovedToWeighted;

	public Action<GameItem> onItemAddedToCounters;

	public Action<GameItem> onItemRemovedToCounters;

	public Action<GameItem, int> onItemSold;

	public Action<GameItem, int> onItemBought;

	public Action onDealAccepted;

	private Func<GameItem, bool> willAcceptItemPreference;

	public Action OnArrival;

	public Action OnDismiss;

	public Func<bool> tryInterceptDismiss;

	public Func<bool> tryInterceptReport;

	public bool holsterResumesDialogue;

	public Func<Dialogue> unholsterDialogue;

	public bool shotCountsAsCapture;

	public Func<Dialogue> shotCaptureDialogue;

	public bool survivesShot;

	public Func<Dialogue> shotSurvivedDialogue;

	public bool forceShootable;

	public Action onShotDead;

	public Action onShotSurvived;

	public List<string> clientNegociationCheck;

	public string clientFaction = "NONE";

	public string bubbleColor = "DEFAULT";

	public List<string> tag;

	public bool isSecurity;

	public bool isSentenceOfficer;

	public bool isWanted;

	public string wantedPosterId = "";

	public string realName = "";

	public int reportReward;

	public int reputationPenalty = -6;

	public bool reportDegradesFaction = true;

	public bool reportImprovesFaction;

	public bool isStealthWanted;

	public Action onCaptureAction;

	public Action onReportAction;

	public int budgetConversionRate = 50;

	public int clientImpatience;

	public bool reported;

	public bool isMultiBuyDisabled;

	public List<string> clientItemFeatureBuying;

	public bool chargesContrabandMarkup;

	public bool acceptRetailMarkup = true;

	public int perfumeInfluence;

	public bool isNoContributeToEvidence;

	public double tradeReputationMultiplier = 1.0;

	public double? sellTradeRepMultiplierOverride;

	public double? buyTradeRepMultiplierOverride;

	public GunOrderData gunOrderData;

	public bool noAcceptingContraband;

	public bool AcceptingContrabandOverride;

	public bool IsPersonalConsumption = true;

	public bool IsClientAddict;

	public bool CanReturn = true;

	public bool CanReturnAsBuyer = true;

	public bool CanExposeChem;

	public Dialogue resumeAfterConfrontDialogue;

	public bool refusesHazardousWaterLabel;

	public bool alwaysFeedback;

	public int acceptedChemicalProductGrade = 1;

	public Hireling hirelingData;

	public bool isLazyInspector;

	public bool isAug;

	public int augTier;

	public bool isAugScanned;

	public AugIdleTell augIdleTell;

	public string attractedItemId;

	public string attractedType;

	public string eventSourceId;

	public bool alwaysRejectBadBargain;

	public bool isBargainAvailable = true;

	public bool staysAfterFailedBargain;

	public ClientIntent clientIntent;

	private GameItem lastFilterLineItem;

	private Dialogue lastPostDealGraft;

	public Func<GameItem, bool> willAcceptItem
	{
		get
		{
			if (willAcceptItemPreference == null)
			{
				return null;
			}
			return delegate(GameItem gameItem)
			{
				if (!willAcceptItemPreference(gameItem))
				{
					return false;
				}
				return (!GeneralHelper.IsItemOwned(gameItem) || !IsClientRefusingItem(gameItem)) ? true : false;
			};
		}
		set
		{
			willAcceptItemPreference = value;
		}
	}

	public void CompleteBusiness()
	{
		if (isBusinessComplete)
		{
			Debug.LogWarning($"<color=orange>[StoreClient] CompleteBusiness called twice for '{identifier}' ({clientIntent}): a deal path ran again after the business was already complete.</color>");
		}
		else
		{
			isBusinessComplete = true;
		}
	}

	public double GetTradeRepMultiplier(bool isPlayerSold)
	{
		return (isPlayerSold ? sellTradeRepMultiplierOverride : buyTradeRepMultiplierOverride) ?? tradeReputationMultiplier;
	}

	public void ClearTradeRepMultiplierOverrides()
	{
		sellTradeRepMultiplierOverride = null;
		buyTradeRepMultiplierOverride = null;
	}

	public StoreClient()
	{
		clientBuyingTagList = new List<string>();
		clientBuyingIdList = new List<string>();
		clientBlackTagList = new List<string>();
		clientBlackIdList = new List<string>();
		varietyTrackingList = new List<string>();
		mainDialogue = new Dialogue();
		clientNegociationCheck = new List<string>();
		possibleSprites = new List<string>();
		tag = new List<string>();
		clientItemFeatureBuying = new List<string>();
	}

	public void OnAnyAction()
	{
		if (clientIntent == ClientIntent.SELL || clientIntent == ClientIntent.APPRAISAL_SERVICE)
		{
			if (EmporiumEntry.Instance.GetAllNonOwendItemCount() == 0 && allDoneDialogue != null)
			{
				DialogUIManager.Instance.StartDialogue(allDoneDialogue);
			}
		}
		else if (clientIntent == ClientIntent.EXPEDITION && allDoneDialogue != null)
		{
			DialogUIManager.Instance.StartDialogue(allDoneDialogue);
		}
	}

	public bool IsClientValid()
	{
		if (clientIntent == ClientIntent.UNDEFINED)
		{
			return false;
		}
		return true;
	}

	public bool IsClientRefusingItem(GameItem gameItem, bool ignoreContents = false)
	{
		if (gameItem.IsTag("NOT_FOR_RESALE"))
		{
			return true;
		}
		if (clientBlackIdList.Contains(gameItem.identifier))
		{
			return true;
		}
		if (gameItem.GetGameItemType().Any((string item) => clientBlackTagList.Contains(item)))
		{
			return true;
		}
		if (noAcceptingContraband && ContrabandHelper.IsContraband(gameItem))
		{
			return true;
		}
		if (isRequireVariety && IsItemInVarietyList(gameItem))
		{
			return true;
		}
		if (ChemicalProductHelper.HasChemicalPurity(gameItem) && !ChemicalProductHelper.WillAcceptNarcotic(gameItem, acceptedChemicalProductGrade))
		{
			return true;
		}
		if (GeneralHelper.IsHazardousWaste(gameItem))
		{
			return true;
		}
		if (refusesHazardousWaterLabel && WaterFeatureHelper.IsLabelledHazardousWater(gameItem))
		{
			return true;
		}
		if (IsRefusingLuxuryItem(gameItem))
		{
			return true;
		}
		if (!ignoreContents && !LockHelper.IsCurrentlyLocked(gameItem) && gameItem.contentWindow != null && !gameItem.IsTag("ADV_GUN_TAG"))
		{
			foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(gameItem))
			{
				if (!IsClientBuyingThisItem(item))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsRefusingLuxuryItem(GameItem gameItem)
	{
		if (!gameItem.IsTag("LUXURY_ITEM"))
		{
			return false;
		}
		if (!clientBuyingTagList.Contains("WATER"))
		{
			return false;
		}
		if (IsFaction("FACTION_TOURIST"))
		{
			return false;
		}
		if (IsFaction("FACTION_UPPER_LEVEL"))
		{
			return false;
		}
		return true;
	}

	public bool IsClientBuyingThisItem(GameItem gameItem)
	{
		return IsClientBuyingThisItem(gameItem, ignoreContents: false);
	}

	public bool IsClientBuyingThisItem(GameItem gameItem, bool ignoreContents)
	{
		if (IsClientRefusingItem(gameItem, ignoreContents))
		{
			return false;
		}
		if (clientBuyingTagList.Contains("ALL_TYPE"))
		{
			return true;
		}
		if (clientBuyingIdList.Contains(gameItem.identifier))
		{
			return true;
		}
		if (clientBuyingTagList.Contains("ALL_TYPE_EXCEPT_CONTRABAND"))
		{
			if (gameItem.IsTag("CONTRABAND"))
			{
				return false;
			}
			return true;
		}
		foreach (string clientBuyingTag in clientBuyingTagList)
		{
			if (gameItem.IsTag(clientBuyingTag))
			{
				return true;
			}
		}
		if (willAcceptItemPreference != null && willAcceptItemPreference(gameItem))
		{
			return true;
		}
		return false;
	}

	public bool IsRefusingForFilterInside(GameItem gameItem)
	{
		if (WaterHelper.GetFilterItem(gameItem) == null)
		{
			return false;
		}
		if (IsClientBuyingThisItem(gameItem))
		{
			return false;
		}
		return IsClientBuyingThisItem(gameItem, ignoreContents: true);
	}

	public bool TrySayRemoveFilterLine(GameItem item)
	{
		if (!GeneralHelper.IsItemOwned(item))
		{
			return false;
		}
		if (!IsRefusingForFilterInside(item))
		{
			return false;
		}
		if (lastFilterLineItem == item)
		{
			return true;
		}
		lastFilterLineItem = item;
		Dialogue dialogue = new Dialogue();
		dialogue.SetText(displayName, LocHelper.GetLocalizedOrDefault("Dialogue", "dialog_water_buyer_filter_inside", "Take that filter out first. I only want to buy the water."));
		dialogue.priority = 4;
		DialogUIManager.Instance.StartDialogue(dialogue);
		return true;
	}

	public void CompleteClientCreation(bool IgnoreExposeFunc = false)
	{
		onItemAddedToWeighted = (Action<GameItem>)Delegate.Combine(onItemAddedToWeighted, (Action<GameItem>)delegate(GameItem item)
		{
			TrySayRemoveFilterLine(item);
		});
		onItemRemovedToWeighted = (Action<GameItem>)Delegate.Combine(onItemRemovedToWeighted, (Action<GameItem>)delegate(GameItem item)
		{
			if (lastFilterLineItem == item)
			{
				lastFilterLineItem = null;
			}
		});
		onItemAddedToWeighted = (Action<GameItem>)Delegate.Combine(onItemAddedToWeighted, (Action<GameItem>)delegate(GameItem item)
		{
			if (GeneralHelper.IsItemOwned(item) && IsRefusingLuxuryItem(item))
			{
				Dialogue dialogue3 = new Dialogue();
				dialogue3.SetText(displayName, LocHelper.GetLocalizedDialogue("dialogue_no_luxury"));
				dialogue3.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue3);
			}
		});
		if (acceptDealDialogueLastLine != null)
		{
			Dialogue dialogue = acceptDealDialogueLastLine;
			dialogue.startAction = (Action)Delegate.Combine(dialogue.startAction, (Action)delegate
			{
				OnDealAccepted(acceptDealDialogueLastLine);
			});
		}
		else if (acceptDealDialogue != null)
		{
			Dialogue dialogue2 = acceptDealDialogue;
			dialogue2.startAction = (Action)Delegate.Combine(dialogue2.startAction, (Action)delegate
			{
				OnDealAccepted(acceptDealDialogue);
			});
		}
		if (isWanted)
		{
			mainDialogue.endActionDelay = 0f;
		}
		if (!IgnoreExposeFunc)
		{
			if (IsTag("SCAV"))
			{
				canClientExposeFeature = ClientCanExposeFunc.ScavClientExposeCapacity;
			}
			else if (clientFaction.Equals("FACTION_LOWER_LEVEL") || clientFaction.Equals("FACTION_BLACK_MARKET"))
			{
				canClientExposeFeature = ClientCanExposeFunc.LLClientExposeCapacity;
			}
			else if (clientFaction.Equals("FACTION_TOURIST"))
			{
				canClientExposeFeature = ClientCanExposeFunc.TouristClientExposeCapacity;
			}
			else if (clientFaction.Equals("FACTION_SECURITY"))
			{
				canClientExposeFeature = ClientCanExposeFunc.SecClientExposeCapacity;
			}
			else if (clientFaction.Equals("FACTION_UPPER_LEVEL") || clientFaction.Equals("FACTION_TOURIST"))
			{
				canClientExposeFeature = ClientCanExposeFunc.ULClientExposeCapacity;
			}
			else
			{
				canClientExposeFeature = ClientCanExposeFunc.BasicClientExposeCapacity;
			}
		}
		if (isSecurity)
		{
			noAcceptingContraband = true;
		}
		if (IsTag("SCAV") && RNG.Roll(GetScavNoAcceptContrabandChance()))
		{
			noAcceptingContraband = true;
		}
		if (IsTag("LOWER_LEVEL_CITIZEN") && RNG.Roll(35))
		{
			noAcceptingContraband = true;
		}
		if (clientIntent != ClientIntent.BUY && clientIntent != ClientIntent.SELLNBUY)
		{
			noAcceptingContraband = false;
		}
		if (noAcceptingContraband && AcceptingContrabandOverride)
		{
			noAcceptingContraband = false;
		}
	}

	public static int GetScavNoAcceptContrabandChance()
	{
		return 25 - StoreReputation.GetLLReputation() / 4;
	}

	public void AddBasicDialog()
	{
		acceptDealDialogue = StoreClientDialogList.GetRandomDialogAccepted();
		allDoneDialogue = StoreClientDialogList.GetRandomDialogDone().SetEndAction(delegate
		{
			PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
		});
	}

	public void OnDealAccepted(Dialogue hostLine = null)
	{
		if (onDealAccepted != null)
		{
			onDealAccepted();
		}
		if (PlayerStore.Instance.currentClientInstance == null || (hostLine != null && hostLine.nextDialogue != null && hostLine.nextDialogue != lastPostDealGraft))
		{
			return;
		}
		Dialogue dialogue = null;
		if (StoreClientInstance.IsClientBusinessDone(PlayerStore.Instance.currentClientInstance))
		{
			Debug.Log("Business All Done");
			dialogue = ((allDoneDialogue == null) ? StoreClientDialogList.CreateDefaultAllDoneDialog(displayName) : allDoneDialogue);
		}
		else
		{
			Debug.Log("Business Not Done");
			if (PlayerStore.Instance.GetCurrentClientIntent() == ClientIntent.BUY)
			{
				dialogue = StoreClientDialogList.CreateAfterSellingToClientDialog(displayName);
			}
			else if (PlayerStore.Instance.GetCurrentClientIntent() == ClientIntent.SELL)
			{
				dialogue = StoreClientDialogList.CreateAfterBuyingFromClientDialog(displayName);
			}
		}
		if (hostLine != null)
		{
			hostLine.SetNextDialogue(dialogue);
			lastPostDealGraft = dialogue;
		}
		else if (dialogue != null)
		{
			DialogUIManager.Instance.StartDialogue(dialogue);
		}
	}

	public void TrackItemVariety(GameItem gameItem)
	{
		varietyTrackingList.Add(gameItem.identifier);
	}

	public bool IsItemInVarietyList(GameItem gameItem)
	{
		foreach (string varietyTracking in varietyTrackingList)
		{
			if (varietyTracking.Equals(gameItem.identifier))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasBudgetLeftToBuy(GameItem gameItem)
	{
		if (!useClientBudget)
		{
			return true;
		}
		if (gameItem.GetNegociatedValue() > clientBudget)
		{
			return false;
		}
		return true;
	}

	public bool CanClientExposeAnyFeature(GameItem gameItem)
	{
		foreach (ItemFeature itemFeature in gameItem.itemFeatures)
		{
			if (CanClientExposeThisFeature(itemFeature))
			{
				return true;
			}
		}
		if (AmmoBoxHelper.IsAmmoBox(gameItem) && AmmoBoxHelper.IsBoxAccepted(gameItem, this))
		{
			return true;
		}
		return false;
	}

	public bool CanClientExposeThisFeature(ItemFeature itemFeature)
	{
		if (itemFeature.isDisabled)
		{
			return false;
		}
		if (itemFeature.useCondition)
		{
			if (itemFeature.IsFeatureMatch())
			{
				return false;
			}
			if (!itemFeature.IsRealFeatureCheaper())
			{
				return false;
			}
			if (itemFeature.IsExposed())
			{
				return false;
			}
			if (canClientExposeFeature != null && canClientExposeFeature(itemFeature, this))
			{
				return true;
			}
		}
		if (!itemFeature.IsExposed() && itemFeature.isExposable && !itemFeature.IsRealFeatureCheaper() && !itemFeature.IsFeatureMatch() && canClientExposeFeature != null && canClientExposeFeature(itemFeature, this))
		{
			return true;
		}
		return false;
	}

	public void OnItemPlaced(GameItem gameItem)
	{
		if (CanClientExposeAnyFeature(gameItem))
		{
			ClientExposeFeature(gameItem);
		}
		AmmoBoxHelper.HandleAmmoBoxCheck(gameItem, this);
	}

	public static List<GameItem> GetInspectableChildren(GameItem gameItem)
	{
		if (gameItem.contentWindow == null || LockHelper.IsCurrentlyLocked(gameItem))
		{
			return new List<GameItem>();
		}
		return GraphUtils.FindAllChildrenType<GameItem>(gameItem);
	}

	public bool CanClientExposeAnyFeatureDeep(GameItem gameItem)
	{
		if (CanClientExposeAnyFeature(gameItem))
		{
			return true;
		}
		foreach (GameItem inspectableChild in GetInspectableChildren(gameItem))
		{
			if (CanClientExposeAnyFeature(inspectableChild))
			{
				return true;
			}
		}
		return false;
	}

	public void InspectItemDeep(GameItem gameItem)
	{
		foreach (GameItem inspectableChild in GetInspectableChildren(gameItem))
		{
			if (CanClientExposeAnyFeature(inspectableChild))
			{
				OnItemPlaced(inspectableChild);
			}
		}
		if (CanClientExposeAnyFeature(gameItem))
		{
			OnItemPlaced(gameItem);
		}
	}

	public void ClientExposeFeature(GameItem gameItem)
	{
		foreach (ItemFeature itemFeature in gameItem.itemFeatures)
		{
			if (CanClientExposeThisFeature(itemFeature))
			{
				DialogUIManager.Instance.StartDialogue(AttachConfrontResume(itemFeature.GetClientExposeDialog()));
				if (!isAug)
				{
					StoreReputation.ModReputation(clientFaction, -4);
				}
				itemFeature.ExposeFeature(lowkey: true, playBad: true);
			}
		}
	}

	public Dialogue AttachConfrontResume(Dialogue confront)
	{
		if (confront == null || resumeAfterConfrontDialogue == null || !IsClientBuying())
		{
			return confront;
		}
		Dialogue dialogue = confront;
		while (dialogue.nextDialogue != null)
		{
			if (dialogue == resumeAfterConfrontDialogue)
			{
				return confront;
			}
			dialogue = dialogue.nextDialogue;
		}
		if (dialogue == resumeAfterConfrontDialogue)
		{
			return confront;
		}
		if (dialogue.choices.Count == 0)
		{
			dialogue.SetNextDialogue(resumeAfterConfrontDialogue);
		}
		return confront;
	}

	public bool ContainCheck(string check)
	{
		return clientNegociationCheck.Contains(check);
	}

	public void AddBasicCheck()
	{
		clientNegociationCheck.Add("CHECK_WATER_FLASHLIGHT");
		clientNegociationCheck.Add("CHECK_OPEN_BOX");
	}

	public void AddCheck(string check)
	{
		if (check.Equals("CHECK_WATER_STRIP"))
		{
			clientNegociationCheck.Add(check);
			clientNegociationCheck.Remove("CHECK_WATER_STRIP");
		}
		else if (check.Equals("CHECK_INGOT_FLASHLIGHT"))
		{
			clientNegociationCheck.Add(check);
			clientNegociationCheck.Remove("CHECK_INGOT_FLASHLIGHT");
		}
		else
		{
			clientNegociationCheck.Add(check);
		}
	}

	public string GetSprite()
	{
		if (spriteName.Equals("") && possibleSprites.Count == 0)
		{
			return "default";
		}
		if (spriteName.Equals("") && possibleSprites.Count != 0)
		{
			spriteName = ClientSpriteVariety.Pick(possibleSprites);
			return spriteName;
		}
		if (!spriteName.Equals(""))
		{
			return spriteName;
		}
		return "default";
	}

	public void OnIntroduced()
	{
		isIntroduced = true;
		if (isOfferWholesale)
		{
			NegociationUIManager.Instance.OpenUI();
		}
	}

	public void SetClientBudget(int amount)
	{
		useClientBudget = true;
		clientBudget = amount;
	}

	public void SetClientBudget(int amount, int additionalRange)
	{
		useClientBudget = true;
		clientBudget = amount + RNG.GetRandomInt(0, additionalRange);
	}

	public void OnItemSold(GameItem item, int soldAmount)
	{
		if (onItemSold != null)
		{
			onItemSold(item, soldAmount);
		}
		if (useClientBudget)
		{
			clientBudget += soldAmount * budgetConversionRate / 100;
		}
	}

	public void OnItemBought(GameItem item, int cost)
	{
		if (onItemBought != null)
		{
			onItemBought(item, cost);
		}
	}

	public void SetVariety()
	{
		isRequireVariety = true;
		onItemAddedToCounters = (Action<GameItem>)Delegate.Combine(onItemAddedToCounters, (Action<GameItem>)delegate(GameItem item)
		{
			if (GeneralHelper.IsItemOwned(item))
			{
				item.AddItemFeature(ItemFeatureList.VarietyBuyerBonus());
			}
		});
		onItemRemovedToCounters = (Action<GameItem>)Delegate.Combine(onItemRemovedToCounters, (Action<GameItem>)delegate(GameItem item)
		{
			if (GeneralHelper.IsItemOwned(item))
			{
				item.itemFeatures.Remove(item.FindItemFeatureByID("varietyBuyerBonus"));
			}
		});
		StampOwnedCounterItems(ItemFeatureList.VarietyBuyerBonus);
	}

	public void SetTourist()
	{
		onItemAddedToCounters = (Action<GameItem>)Delegate.Combine(onItemAddedToCounters, (Action<GameItem>)delegate(GameItem item)
		{
			if (GeneralHelper.IsItemOwned(item))
			{
				item.AddItemFeature(ItemFeatureList.TouristTax());
			}
		});
		onItemRemovedToCounters = (Action<GameItem>)Delegate.Combine(onItemRemovedToCounters, (Action<GameItem>)delegate(GameItem item)
		{
			if (GeneralHelper.IsItemOwned(item))
			{
				item.itemFeatures.Remove(item.FindItemFeatureByID("touristTax"));
			}
		});
		StampOwnedCounterItems(ItemFeatureList.TouristTax);
	}

	private void StampOwnedCounterItems(Func<ItemFeature> feature)
	{
		if (PlayerStore.Instance == null || PlayerStore.Instance.currentClientInstance == null || PlayerStore.Instance.currentClientInstance.GetClientBlueprint() != this || EmporiumEntry.Instance == null)
		{
			return;
		}
		bool flag = false;
		foreach (GameItem childItem in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem))
			{
				childItem.AddItemFeature(feature());
				flag = true;
			}
		}
		foreach (GameItem childItem2 in EmporiumEntry.Instance.frontInvinvElement.childItems)
		{
			if (GeneralHelper.IsItemOwned(childItem2))
			{
				childItem2.AddItemFeature(feature());
				flag = true;
			}
		}
		if (flag)
		{
			PlayerStore.Instance.OnWeightedAreaChanged();
		}
	}

	public void SetBudget(int budget)
	{
		useClientBudget = true;
		clientBudget = budget;
		ApplyBudgetModifier();
	}

	public void OverrideBudget(int budget)
	{
		clientBudget = budget;
	}

	public void ModBudget(int budgetMod)
	{
		clientBudget += budgetMod;
	}

	public void SetBudget(int budget, int budgetConversion)
	{
		useClientBudget = true;
		clientBudget = budget;
		budgetConversionRate = budgetConversion;
		ApplyBudgetModifier();
	}

	public int GetBudget()
	{
		return clientBudget;
	}

	public void ApplyBudgetModifier()
	{
		if (useClientBudget)
		{
			int num = clientBudget;
			if (StoreReputation.IsPerkUnlocked("UL_TOP_REVIEW"))
			{
				clientBudget += RNG.MultiplyAndRound(num, 0.25);
			}
			if (StoreReputation.IsPerkUnlocked("SEC_RELIABLE_VENDOR"))
			{
				clientBudget += RNG.MultiplyAndRound(num, 0.5);
			}
			if (StartingPerk.IsSycophant() && IsFaction("FACTION_UPPER_LEVEL"))
			{
				clientBudget += RNG.MultiplyAndRound(num, 0.10000000149011612);
			}
			int endorsementTier = NetworkUpgrade.GetEndorsementTier();
			if (endorsementTier > 0)
			{
				clientBudget += RNG.MultiplyAndRound(num, (float)(endorsementTier * 25) / 100f);
			}
			int currentStoreAttractiveness = PlayerStore.Instance.GetCurrentStoreAttractiveness();
			if (currentStoreAttractiveness >= 900)
			{
				clientBudget += num;
			}
			else if (currentStoreAttractiveness >= 750)
			{
				clientBudget += RNG.MultiplyAndRound(num, 0.75);
			}
			else if (currentStoreAttractiveness >= 600)
			{
				clientBudget += RNG.MultiplyAndRound(num, 0.5);
			}
			else if (currentStoreAttractiveness >= 450)
			{
				clientBudget += RNG.MultiplyAndRound(num, 0.25);
			}
			else if (currentStoreAttractiveness >= 300)
			{
				clientBudget += RNG.MultiplyAndRound(num, 0.10000000149011612);
			}
		}
	}

	public void SetWholesale(int percentage)
	{
		isOfferWholesale = true;
		wholesaleDiscount = -Math.Abs(percentage);
	}

	public void SetWanted(int reward, string wantedRealName = "", bool isStealthWanted = false)
	{
		isWanted = true;
		reportReward = reward;
		this.isStealthWanted = isStealthWanted;
		if (wantedPosterId.Equals(""))
		{
			wantedPosterId = identifier;
		}
		if (wantedRealName.Equals(""))
		{
			realName = displayName;
		}
		else
		{
			realName = wantedRealName;
		}
	}

	public void CompleteSecClient()
	{
		isSecurity = true;
		bubbleColor = "sec";
		onItemAddedToClient = (Action<GameItem>)Delegate.Combine(onItemAddedToClient, (Action<GameItem>)delegate(GameItem item)
		{
			if (!isSentenceOfficer)
			{
				if (GeneralHelper.IsItemOwned(item) && item.IsTag("CONTRABAND_ITEM_TAG"))
				{
					if (DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogOnContrabandOnCounter()))
					{
						dismissable = false;
					}
				}
				else if (GeneralHelper.IsItemOwned(item) && item.IsTag("STOLEN_TAG") && RNG.Roll(StolenHelper.GetFoundChance(item)) && DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogOnStolenOnCounter()))
				{
					StolenHelper.SetHeat(item, 100);
					dismissable = false;
				}
			}
		});
		onItemAddedToWeighted = (Action<GameItem>)Delegate.Combine(onItemAddedToWeighted, (Action<GameItem>)delegate(GameItem item)
		{
			if (!isSentenceOfficer)
			{
				if (GeneralHelper.IsItemOwned(item) && item.IsTag("CONTRABAND_ITEM_TAG"))
				{
					if (DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogOnContrabandOnCounter()))
					{
						dismissable = false;
					}
				}
				else if (GeneralHelper.IsItemOwned(item) && item.IsTag("STOLEN_TAG") && RNG.Roll(StolenHelper.GetFoundChance(item)) && DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogOnStolenOnCounter()))
				{
					StolenHelper.SetHeat(item, 100);
					dismissable = false;
				}
			}
		});
		OnItemAddedToShowcase = (Action<GameItem>)Delegate.Combine(OnItemAddedToShowcase, (Action<GameItem>)delegate(GameItem item)
		{
			if (!isSentenceOfficer)
			{
				if (GeneralHelper.IsItemOwned(item) && item.IsTag("CONTRABAND_ITEM_TAG"))
				{
					if (DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogOnContrabandOnCounter()))
					{
						dismissable = false;
					}
				}
				else if (GeneralHelper.IsItemOwned(item) && item.IsTag("STOLEN_TAG") && RNG.Roll(StolenHelper.GetFoundChance(item)) && DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogOnStolenOnCounter()))
				{
					StolenHelper.SetHeat(item, 100);
					dismissable = false;
				}
			}
		});
		Dialogue lastLine = StoreClientDialogList.SecurityDialogInspectionLastLine();
		OnArrival = (Action)Delegate.Combine(OnArrival, (Action)delegate
		{
			if (!isSentenceOfficer)
			{
				GameItem gameItem = ContrabandHelper.SecurityCheckCounter();
				GameItem gameItem2 = ContrabandHelper.SecurityCheckDisplayCase();
				if (gameItem != null && ContrabandHelper.IsContraband(gameItem))
				{
					dismissable = false;
					mainDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_arrival_contraband_counter")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_contraband_counter_2"))
						.SetNextDialogue(lastLine);
				}
				else if (gameItem != null && StolenHelper.IsStolenItem(gameItem))
				{
					StolenHelper.SetHeat(gameItem, 100);
					dismissable = false;
					mainDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_arrival_stolen_counter")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_contraband_counter_2"))
						.SetNextDialogue(lastLine);
				}
				else if (gameItem2 != null && ContrabandHelper.IsContraband(gameItem2))
				{
					dismissable = false;
					mainDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_arrival_contraband_display")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_contraband_counter_2"))
						.SetNextDialogue(lastLine);
				}
				else if (gameItem2 != null && StolenHelper.IsStolenItem(gameItem2))
				{
					StolenHelper.SetHeat(gameItem2, 100);
					dismissable = false;
					mainDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_arrival_stolen_display")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_sec_inspect_contraband_counter_2"))
						.SetNextDialogue(lastLine);
				}
			}
		});
	}

	public bool IsWanted()
	{
		if (isWanted)
		{
			return true;
		}
		if (IsSellingContraband())
		{
			return true;
		}
		if (IsSellingStolen())
		{
			return true;
		}
		return false;
	}

	public bool IsSellingContraband()
	{
		foreach (GameItem item in EmporiumEntry.Instance.GetAllNonOwnedItem())
		{
			if (ContrabandHelper.IsSeriousContraband(item))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSellingStolen()
	{
		foreach (GameItem item in EmporiumEntry.Instance.GetAllNonOwnedItem())
		{
			if (StolenHelper.IsStolenItem(item))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsClientBuying()
	{
		if (clientIntent == ClientIntent.BUY || clientIntent == ClientIntent.SELLNBUY)
		{
			return true;
		}
		return false;
	}

	public bool IsClientSelling()
	{
		if (clientIntent == ClientIntent.SELL || clientIntent == ClientIntent.SELLNBUY)
		{
			return true;
		}
		return false;
	}

	public void AddTag(string tag)
	{
		this.tag.Add(tag);
	}

	public bool IsTag(string tag)
	{
		return this.tag.Contains(tag);
	}

	public void SetBarterOffer(BarterOffer barterOffer)
	{
		this.barterOffer = barterOffer;
		barterOffer.ownerClient = this;
		clientIntent = ClientIntent.BARTER;
	}

	public void StartMainDialogue()
	{
		DialogUIManager.Instance.StartDialogue(mainDialogue);
		isMainDialogueStarted = true;
	}

	public bool CanReport()
	{
		if (reported)
		{
			return false;
		}
		if (StoreUIManager.Instance.isShot)
		{
			return false;
		}
		if (BargainUIManager.Instance != null && BargainUIManager.Instance.IsCurrentClientCoerced())
		{
			return false;
		}
		if (!dismissable && nonDismissableArrestable)
		{
			return true;
		}
		if (!arrestable)
		{
			return false;
		}
		if (!dismissable)
		{
			return false;
		}
		return true;
	}

	public bool IsFaction(string faction)
	{
		return clientFaction.Equals(faction);
	}
}

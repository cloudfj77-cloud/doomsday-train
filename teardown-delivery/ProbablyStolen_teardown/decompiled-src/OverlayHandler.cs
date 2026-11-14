using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverlayHandler : MonoBehaviour, Cancelable
{
	private enum NavScanStatus
	{
		None,
		Created,
		Scanning,
		Done
	}

	public static OverlayHandler current;

	private PixelWindow tooltipWindow;

	private RichTextElement tooltipElement;

	private TreeNodeRender fadeBackgroundNode;

	private TreeNodeRender blockBackgroundNode;

	private TreeNodeRender confirmBackgroundNode;

	private PixelWindow confirmWindow;

	private RichTextElement confirmTextElement;

	private GridPixelElement confirmYesNoElement;

	private ButtonElement confirmYesButton;

	private ButtonElement confirmNoButton;

	public PixelWindow itemChooseWindow;

	public GameGridScrollableInventory itemChooseInventory;

	private ButtonElement chooseNoButton;

	public PixelWindow barterWindow;

	private GridPixelElement barterGridElementTopLeft;

	private GridPixelElement barterGridElementRight;

	private GridPixelElement barterGridElementBottomLeft;

	private TagElement barterNameTag;

	private TagElement barterScoreTag;

	private GameGridScrollableInventory currentBarterBuyInventory;

	private GameGridScrollableInventory currentBarterSellInventory;

	private GameGridScrollableInventory currentBarterInventory;

	public PixelWindow navWindow;

	private GridPixelElement navGridOuter;

	private GridPixelElement navGridInner;

	private RichTextElement navScreen;

	private ButtonElement navButton;

	private NavScanStatus navStatus;

	public const int MAX_NAV_NUM = 5;

	private List<BannerLocationElement> navList = new List<BannerLocationElement>();

	private int navSize;

	public PixelWindow eventWindow;

	private SpritePixelElement eventSprite;

	private ScrollableRichTextElement eventLog;

	private ButtonElement eventButton;

	public static int defaultBannerWidth = 128;

	public static int defaultBannerHeight = 256;

	public static int defaultButtonWidth = 100;

	public static int defaultButtonHeight = 30;

	private Action currentCancelAction;

	private bool hideNextAutoShowEvent;

	private bool doUpdateInventoryUINextFrame;

	private bool doUpdateBarterNextFrame;

	private bool isFadeBlock;

	private int mouseBlockStacks;

	private RichText previousRichTextCache;

	public PixelWindow logWindow { get; private set; }

	public ScrollableRichTextElement logElement { get; private set; }

	public GameCharacterItem currentBarterCharacter { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	private void Start()
	{
		logWindow = new PixelWindow(canMove: true, "Log");
		logElement = new ScrollableRichTextElement(constWidth: true, canResize: false);
		logWindow.Attach(logElement);
		logWindow.ResizePixels(400, 300);
		logWindow.Validate();
		InitChoose();
		InitShade();
		InitBarter();
		InitNav();
	}

	private void Update()
	{
		if (doUpdateBarterNextFrame)
		{
			UpdateBarterImmediately();
			doUpdateBarterNextFrame = false;
		}
		if (doUpdateInventoryUINextFrame)
		{
			UpdateInventoryStateUIImmediately();
			doUpdateInventoryUINextFrame = false;
		}
	}

	private void InitChoose()
	{
		GridPixelElement gridPixelElement = new GridPixelElement(1, 2, isCentered: true);
		itemChooseWindow = new PixelWindow();
		itemChooseWindow.Attach(gridPixelElement);
		itemChooseWindow.handler.SetParent(RenderHandler.current.overlayRoot);
		itemChooseWindow.SetTitle("Choose");
		itemChooseWindow.Validate();
		chooseNoButton = new ButtonElement("Cancel", defaultButtonWidth, defaultButtonHeight, null, ButtonElement.Color.Red);
		itemChooseInventory = new GameGridScrollableInventory(30, 20);
		itemChooseInventory.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		gridPixelElement.Attach(itemChooseInventory);
		gridPixelElement.Attach(chooseNoButton);
		tooltipWindow = new PixelWindow();
		tooltipWindow.handler.SetParent(RenderHandler.current.overlayRoot);
		tooltipWindow.handler.name = "Tooltip Window";
		tooltipElement = new RichTextElement(canResize: false, -1, -1, hasRaycast: false);
		tooltipElement.SetPadding(8, 8, 8, 8);
		tooltipWindow.Attach(tooltipElement);
		confirmWindow = new PixelWindow(canMove: false, "Confirmation");
		confirmYesButton = new ButtonElement("Confirm", defaultButtonWidth, defaultButtonHeight, null, ButtonElement.Color.Green);
		confirmNoButton = new ButtonElement("Cancel", defaultButtonWidth, defaultButtonHeight, null, ButtonElement.Color.Red);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2);
		GridPixelElement gridPixelElement3 = (confirmYesNoElement = new GridPixelElement(2, 1));
		gridPixelElement2.isCentered = true;
		gridPixelElement3.isCentered = true;
		confirmTextElement = new RichTextElement();
		gridPixelElement3.Attach(confirmYesButton, defaultButtonWidth * 3 / 2);
		gridPixelElement3.Attach(confirmNoButton, defaultButtonWidth * 3 / 2);
		gridPixelElement2.Attach(confirmTextElement);
		gridPixelElement2.Attach(gridPixelElement3);
		confirmWindow.Attach(gridPixelElement2);
		confirmWindow.handler.SetParent(RenderHandler.current.overlayRoot);
	}

	private void InitShade()
	{
		confirmBackgroundNode = TreeNodeRender.Instantiate().SetParent(RenderHandler.current.overlayRoot);
		confirmBackgroundNode.rectTransform.sizeDelta = new Vector2(100000f, 100000f);
		confirmBackgroundNode.rectTransform.position = new Vector3(0f, 100000f, confirmBackgroundNode.rectTransform.position.z);
		confirmBackgroundNode.image.color = new Color32(0, 0, 0, 200);
		confirmBackgroundNode.InitEventHandler();
		fadeBackgroundNode = TreeNodeRender.Instantiate().SetParent(RenderHandler.current.overlayRoot);
		fadeBackgroundNode.rectTransform.sizeDelta = new Vector2(100000f, 100000f);
		fadeBackgroundNode.rectTransform.position = new Vector3(0f, 100000f, fadeBackgroundNode.rectTransform.position.z);
		fadeBackgroundNode.image.color = new Color32(0, 0, 0, 200);
		fadeBackgroundNode.InitEventHandler();
		blockBackgroundNode = TreeNodeRender.Instantiate().SetParentWindowRoot();
		blockBackgroundNode.rectTransform.sizeDelta = new Vector2(100000f, 100000f);
		blockBackgroundNode.rectTransform.position = new Vector3(0f, 100000f, blockBackgroundNode.rectTransform.position.z);
		blockBackgroundNode.image.color = new Color32(0, 0, 0, 0);
		blockBackgroundNode.InitEventHandler();
	}

	private void InitBarter()
	{
		barterWindow = new PixelWindow(canMove: true, "Barter");
		barterWindow.isImportant = true;
		barterWindow.AddInventoryStateChangeAction(delegate
		{
			UpdateBarter();
		});
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1, isCentered: true);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered: true);
		barterGridElementRight = new GridPixelElement(1, 2, isCentered: true);
		gridPixelElement.Attach(gridPixelElement2);
		gridPixelElement.Attach(barterGridElementRight);
		barterGridElementTopLeft = new GridPixelElement(2, 2, isCentered: true);
		barterGridElementBottomLeft = new GridPixelElement(1, 3, isCentered: true);
		gridPixelElement2.Attach(barterGridElementTopLeft);
		gridPixelElement2.Attach(barterGridElementBottomLeft);
		ButtonElement buttonElement = new ButtonElement("<- Confirm ->", defaultButtonWidth + 48, defaultButtonHeight, null, ButtonElement.Color.Green);
		ButtonElement buttonElement2 = new ButtonElement("--- Unload ->", defaultButtonWidth + 48, defaultButtonHeight, null, ButtonElement.Color.Red);
		buttonElement.SetCallback(delegate
		{
			if (currentBarterInventory != null)
			{
				int score = GetScore();
				if (score >= 0)
				{
					SwapTrade(currentBarterSellInventory, currentBarterBuyInventory);
					currentBarterCharacter.barterStoredScore = score;
					UpdateBarterImmediately();
				}
			}
		});
		buttonElement2.SetCallback(delegate
		{
			if (currentBarterInventory != null)
			{
				List<GameItem> list = new List<GameItem>();
				list.AddRange(currentBarterBuyInventory.childItems);
				currentBarterBuyInventory.ExpelAll();
				currentBarterInventory.AddFastUnchecked(list);
				UpdateBarterImmediately();
			}
		});
		barterScoreTag = new TagElement();
		barterGridElementBottomLeft.AttachPos(barterScoreTag.SetText("Barter score: 0"), 0, 0);
		barterGridElementBottomLeft.AttachPos(buttonElement, 0, 1);
		barterGridElementBottomLeft.AttachPos(buttonElement2, 0, 2);
		barterNameTag = new TagElement();
		barterGridElementTopLeft.AttachPos(new TagElement().SetText("Sell"), 0, 0);
		barterGridElementTopLeft.AttachPos(new TagElement().SetText("Buy"), 1, 0);
		barterGridElementRight.AttachPos(barterNameTag.SetText("Trader"), 0, 0);
		barterWindow.closeCallback = delegate
		{
			PauseBarter();
		};
		barterWindow.Attach(gridPixelElement);
		barterWindow.Validate();
	}

	private void InitNav()
	{
		navWindow = new PixelWindow(canMove: false, "Navigation");
		navWindow.handler.SetParent(RenderHandler.current.overlayRoot);
		navWindow.isImportant = true;
		navWindow.closeCallback = delegate
		{
			HideNav();
		};
		navGridInner = new GridPixelElement(5, 1, isCentered: true);
		navGridOuter = new GridPixelElement(1, 3, isCentered: true);
		for (int i = 0; i < 5; i++)
		{
			navList.Add(new BannerLocationElement("TEST", "?", defaultBannerWidth, defaultBannerHeight));
		}
		navScreen = new RichTextElement(canResize: false, UISettings.current.fontWidth * 17).SetText("Ready to scan.");
		navButton = new ButtonElement("Begin Scan", defaultButtonWidth + 48, defaultButtonHeight, null, ButtonElement.Color.Green);
		navButton.SetCallback(delegate
		{
			switch (navStatus)
			{
			case NavScanStatus.Created:
				ScanNav();
				break;
			case NavScanStatus.None:
			case NavScanStatus.Scanning:
			case NavScanStatus.Done:
				break;
			}
		});
		navGridOuter.Attach(navGridInner);
		navGridOuter.Attach(navScreen);
		navGridOuter.Attach(navButton);
		navGridOuter.Validate();
		navWindow.Attach(navGridOuter);
		navWindow.Validate();
		current.StartCoroutine(ScanCoroutine());
	}

	private void InitEvent()
	{
		eventWindow = new PixelWindow(canMove: false, "Event");
		eventWindow.handler.SetParent(RenderHandler.current.overlayRoot);
		eventWindow.isImportant = true;
		eventWindow.closeCallback = delegate
		{
			HideEvent();
		};
		GridPixelElement gridPixelElement = new GridPixelElement(2, 1, isCentered: true);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered: true);
		eventSprite = new SpritePixelElement("EventCards/EmptyCard");
		eventLog = new ScrollableRichTextElement(constWidth: true);
		eventLog.ResizePixels(384, 300);
		eventButton = new ButtonElement("Choose Action", defaultBannerWidth, defaultButtonHeight);
		gridPixelElement2.Attach(eventSprite, 0, 270);
		gridPixelElement2.Attach(eventButton);
		gridPixelElement2.Validate();
		gridPixelElement.Attach(eventLog);
		gridPixelElement.Attach(gridPixelElement2);
		gridPixelElement.Validate();
		eventWindow.Attach(gridPixelElement);
		eventWindow.Validate();
	}

	public void HideEvent()
	{
		if (eventWindow.IsVisible())
		{
			confirmBackgroundNode.image.enabled = false;
			eventWindow.Hide();
			currentCancelAction = null;
		}
	}

	public void PushEvent()
	{
		EventData currentEvent = GameMaster.current.shipManager.currentEvent;
		eventWindow.SetTitle(currentEvent.title);
		eventLog.AppendRichText(currentEvent.text);
		eventLog.AppendRichText(new RichTextBuilder());
		eventLog.SetScroll(1f);
		if (currentEvent.imagePath != null)
		{
			eventSprite.SetSprite(currentEvent.imagePath);
			eventSprite.handler.enabled = true;
		}
		else
		{
			eventSprite.handler.enabled = false;
		}
	}

	public void PushEventRichText(RichTextBuilder text)
	{
		eventLog.AppendRichText(text);
		eventLog.AppendRichText(new RichTextBuilder());
		eventLog.SetScroll(1f);
	}

	public void ShowEvent()
	{
		hideNextAutoShowEvent = false;
		EventData currentEvent = GameMaster.current.shipManager.currentEvent;
		if (currentEvent != null && currentEvent.IsRunning())
		{
			bool flag = currentEvent.choices.Count() > 0;
			bool flag2 = currentEvent.choices.FindAll(((GameItem, Func<bool>, Func<bool>, Action) triplet) => triplet.Item2?.Invoke() ?? true).Any();
			bool flag3 = currentEvent.canEndFunc?.Invoke() ?? true;
			if (flag && flag2)
			{
				eventButton.Set("Choose Action").Enable();
				eventButton.SetCallback(delegate
				{
					HideEvent();
					Func<GameItem, bool> chooseFunc = delegate(GameItem item)
					{
						bool flag4 = false;
						foreach (var (gameItem, _, func, action) in currentEvent.choices)
						{
							if (item == gameItem && (func == null || func()) && gameItem.MayActivateSlotItem() && gameItem.CanActivateSlotItem())
							{
								action?.Invoke();
								flag4 = true;
							}
						}
						if (flag4 && !hideNextAutoShowEvent)
						{
							ShowEvent();
						}
						return flag4;
					};
					Action noAction = delegate
					{
						ShowEvent();
					};
					ShowChooseItem((from quadruplet in currentEvent.choices
						where quadruplet.Item3?.Invoke() ?? true
						select quadruplet.Item1).ToList(), "Choose Action", chooseFunc, noAction);
					currentEvent.choices.ForEach(delegate((GameItem, Func<bool>, Func<bool>, Action) quadruplet)
					{
						Func<bool> item2 = quadruplet.Item2;
						if (item2 == null || item2())
						{
							EventData.OnChoiceEnable(quadruplet.Item1);
						}
						else
						{
							EventData.OnChoiceDisable(quadruplet.Item1);
						}
					});
				});
			}
			else if (flag3)
			{
				if (currentEvent.endAction == null)
				{
					eventButton.Set("End").Enable();
					eventButton.SetCallback(delegate
					{
						currentEvent.ForceEnd();
						HideEvent();
					});
				}
				else
				{
					eventButton.Set("Next").Enable();
					eventButton.SetCallback(delegate
					{
						currentEvent.ForceEnd();
						RefreshEvent();
					});
				}
			}
			else if (flag && !flag2)
			{
				eventButton.Set("No Choices").Disable();
			}
			else
			{
				eventButton.Set("Waiting").Disable();
			}
		}
		else
		{
			eventButton.Set("Done").Disable();
		}
		confirmBackgroundNode.image.enabled = true;
		confirmBackgroundNode.image.rectTransform.SetAsLastSibling();
		eventWindow.Validate();
		eventWindow.Show();
		eventWindow.Center();
		eventWindow.rectTransform.SetAsLastSibling();
		eventLog.SetScroll(1f);
		currentCancelAction = HideEvent;
	}

	public void RefreshEvent()
	{
		HideEvent();
		ShowEvent();
	}

	public void ClearEventLog()
	{
		eventWindow.SetTitle("Event");
		eventLog.ClearRichText();
	}

	public void HideNav()
	{
		if (navWindow.IsVisible())
		{
			confirmBackgroundNode.image.enabled = false;
			navWindow.Hide();
			currentCancelAction = null;
		}
	}

	public void ShowNav()
	{
		if (navStatus != 0)
		{
			bool flag = NavigationHelper.CanScan();
			if (flag && navStatus == NavScanStatus.Created)
			{
				navButton.Enable();
				navScreen.SetText("Ready to scan.");
			}
			else if (flag)
			{
				navButton.Disable();
				navScreen.SetText("Scanned.");
			}
			else
			{
				navButton.Disable();
				navScreen.SetText("Not enough power.");
			}
			confirmBackgroundNode.image.enabled = true;
			confirmBackgroundNode.image.rectTransform.SetAsLastSibling();
			navGridInner.Validate();
			navGridOuter.Validate();
			navWindow.Validate();
			navWindow.Show();
			navWindow.Center();
			navWindow.rectTransform.SetAsLastSibling();
			currentCancelAction = HideNav;
		}
	}

	public void CreateNav(List<OverworldPOI> scannedEncounters)
	{
		if (navStatus != 0)
		{
			ResetNav();
		}
		int count = scannedEncounters.Count;
		navSize = Math.Min(navList.Count(), count);
		for (int i = 0; i < navSize; i++)
		{
			BannerLocationElement bannerLocationElement = navList[i];
			navGridInner.Attach(bannerLocationElement);
			if (GameMaster.current.isDebugMode)
			{
				bannerLocationElement.Set(scannedEncounters[i].displayName + " (DEBUG:" + scannedEncounters[i].identifier + ")", scannedEncounters[i].distance.ToString(), scannedEncounters[i].customSpritePath).Disable();
			}
			else
			{
				bannerLocationElement.Set(scannedEncounters[i].displayName, scannedEncounters[i].distance.ToString(), scannedEncounters[i].customSpritePath).Disable();
			}
			OverworldPOI currentEncounter = scannedEncounters[i];
			bannerLocationElement.SetCallback(delegate
			{
				HideNav();
				object obj;
				if (currentEncounter != null)
				{
					OverworldPOI overworldPOI = currentEncounter;
					obj = overworldPOI;
				}
				else
				{
					obj = null;
				}
				OverworldPOI es = (OverworldPOI)obj;
				OverlayHelper.CheckFuelConfirmation(delegate
				{
					HideBarter();
					GameItem activePropulsionSystem = PropulsionHelper.GetActivePropulsionSystem();
					int @int = activePropulsionSystem.GetTagReadonly("PROPULSION_FUEL_DRAIN_INT").GetInt();
					int shipSpeed = activePropulsionSystem.GetTagReadonly("PROPULSION_SPEED_INT").GetInt();
					PropulsionHelper.RemoveFuel(currentEncounter.distance / @int);
					current.FadeBlock(0.2f, 0.6f, delegate
					{
						GameMaster.current.shipManager.StartTransit(currentEncounter.distance / shipSpeed, es);
						GameMaster.current.turnManager.NextTurn();
					});
				}, currentEncounter.distance);
			});
		}
		navGridInner.Validate();
		navGridOuter.Validate();
		navStatus = NavScanStatus.Created;
	}

	private IEnumerator ScanCoroutine(float scanTime = 1.5f, float fps = 30f)
	{
		if (fps < 0.01f)
		{
			fps = 0.01f;
		}
		if (fps > 240f)
		{
			fps = 240f;
		}
		double lastTime = -1.0;
		while (true)
		{
			if (navStatus == NavScanStatus.Scanning)
			{
				BannerLocationElement bannerLocationElement = null;
				foreach (BannerLocationElement nav in navList)
				{
					if (nav.isDisabled)
					{
						if (!nav.isScanning)
						{
							lastTime = Time.timeAsDouble;
							nav.isScanning = true;
						}
						if (nav.isScanning)
						{
							bannerLocationElement = nav;
							break;
						}
					}
				}
				if (bannerLocationElement != null)
				{
					bannerLocationElement.OnScanning();
					if (lastTime >= 0.0 && Time.timeAsDouble - lastTime > (double)scanTime)
					{
						bannerLocationElement.Enable();
						if (bannerLocationElement != navList.Last())
						{
							AudioManager.Instance.Play("PING_SONAR2");
						}
					}
				}
				else
				{
					navStatus = NavScanStatus.Done;
					navScreen.SetText("Scan done.");
					navGridOuter.Validate();
				}
			}
			else
			{
				lastTime = -1.0;
			}
			yield return new WaitForSeconds(1f / fps);
		}
	}

	public void ScanNav()
	{
		if (navStatus == NavScanStatus.Created)
		{
			navButton.Disable();
			navScreen.SetText("Scanning...");
			navGridOuter.Validate();
			navStatus = NavScanStatus.Scanning;
			NavigationHelper.ResetNavProgress();
		}
	}

	public void ResetNav()
	{
		HideNav();
		navStatus = NavScanStatus.None;
		navList.ForEach(delegate(BannerLocationElement e)
		{
			e.Disable();
		});
		navGridInner.DetachAll();
		navButton.Show();
	}

	private void SwapTrade(GameInventory sellInventory, GameInventory buyInventory)
	{
		List<GameItem> list = new List<GameItem>();
		List<GameItem> list2 = new List<GameItem>();
		list.AddRange(sellInventory.childItems);
		list2.AddRange(buyInventory.childItems);
		sellInventory.ExpelAll();
		buyInventory.ExpelAll();
		sellInventory.UncheckedAcceptAll(list2);
		buyInventory.UncheckedAcceptAll(list);
	}

	public void ShowInventory(GameItem item)
	{
		hideNextAutoShowEvent = true;
		ItemMouseDoubleClickHandler.current.OpenContentAction(item, Input.mousePosition);
		item.contentWindow?.Show();
		item.contentWindow?.Center();
	}

	public void ShowBarter(GameCharacterItem gci)
	{
		hideNextAutoShowEvent = true;
		HideBarter();
		if (gci.barterInventory != null)
		{
			currentBarterCharacter = gci;
			currentBarterBuyInventory = gci.barterBuyInventory;
			currentBarterSellInventory = gci.barterSellInventory;
			currentBarterInventory = gci.barterInventory;
			barterGridElementTopLeft.AttachPos(gci.barterSellInventory, 0, 1);
			barterGridElementTopLeft.AttachPos(gci.barterBuyInventory, 1, 1);
			barterGridElementRight.AttachPos(gci.barterInventory, 0, 1);
			UpdateBarterImmediately();
			barterWindow.Show();
			barterWindow.Center();
		}
	}

	public void HideBarter()
	{
		barterGridElementTopLeft.children.Where((GraphNodeStorage inv) => inv is GameInventory).ToList().ForEach(delegate(GraphNodeStorage inv)
		{
			barterGridElementTopLeft.Detach((PixelElement)inv);
		});
		barterGridElementRight.children.Where((GraphNodeStorage inv) => inv is GameInventory).ToList().ForEach(delegate(GraphNodeStorage inv)
		{
			barterGridElementRight.Detach((PixelElement)inv);
		});
		currentBarterInventory = null;
		currentBarterSellInventory = null;
		currentBarterBuyInventory = null;
		currentBarterCharacter = null;
		barterWindow.Hide();
	}

	public void PauseBarter()
	{
		currentBarterCharacter = null;
	}

	private int GetScore()
	{
		if (currentBarterCharacter == null)
		{
			return 0;
		}
		List<GameCharacterItem> currentPartyGCI = MetaHelper.GetCurrentPartyGCI();
		long num = currentBarterSellInventory.GetTotalValue(currentPartyGCI, currentBarterCharacter) - currentBarterBuyInventory.GetTotalValue(currentPartyGCI, currentBarterCharacter) + currentBarterCharacter.barterStoredScore;
		if (num < int.MinValue)
		{
			num = -2147483648L;
		}
		else if (num > int.MaxValue)
		{
			num = 2147483647L;
		}
		return (int)num;
	}

	public void UpdateInventoryStateUI()
	{
		doUpdateInventoryUINextFrame = true;
	}

	public void UpdateInventoryStateUIImmediately()
	{
		WindowsHandler.current.visibleWindows.ForEach(delegate(PixelWindow window)
		{
			window?.stateChangeActionFunc?.Invoke();
		});
	}

	public void UpdateBarter()
	{
		if (currentBarterCharacter != null)
		{
			doUpdateBarterNextFrame = true;
		}
	}

	private void UpdateBarterImmediately()
	{
		if (currentBarterCharacter != null)
		{
			barterScoreTag.SetText("Barter score: " + GetScore());
			barterNameTag.SetText(currentBarterCharacter.GetDisplayName());
			barterGridElementRight.Validate();
			barterGridElementBottomLeft.Validate();
		}
	}

	public void FadeBlock(float fadeSeconds = 0.2f, float darkSeconds = 0.6f, Action fadeMiddleAction = null, Action fadeMiddleEndAction = null, Action fadeBeginAction = null, Action fadeEndAction = null, float darkness = 1f)
	{
		fadeSeconds = Mathf.Clamp(fadeSeconds, 0.1f, 20f);
		if (!isFadeBlock)
		{
			isFadeBlock = true;
			current.StartCoroutine(FadeCoroutine(Time.timeAsDouble, Time.timeAsDouble + (double)fadeSeconds, Time.timeAsDouble + (double)fadeSeconds + (double)darkSeconds + (double)fadeSeconds, darkSeconds, fadeMiddleAction, fadeMiddleEndAction, fadeBeginAction, fadeEndAction, darkness));
		}
	}

	public void TimedBlock(float blockSeconds, Action blockMiddleAction = null, Action blockMiddleEndAction = null, Action blockBeginAction = null, Action blockEndAction = null)
	{
		blockSeconds = Mathf.Clamp(blockSeconds, 0.1f, 60f);
		if (!isFadeBlock)
		{
			isFadeBlock = true;
			current.StartCoroutine(FadeCoroutine(Time.timeAsDouble, Time.timeAsDouble + (double)(blockSeconds / 2f), Time.timeAsDouble + (double)blockSeconds, 0f, blockMiddleAction, blockMiddleEndAction, blockBeginAction, blockEndAction, 0f));
		}
	}

	public void BlockMouse()
	{
		mouseBlockStacks++;
		if (mouseBlockStacks > 0)
		{
			blockBackgroundNode.image.enabled = true;
		}
	}

	public void UnblockMouse()
	{
		if (mouseBlockStacks > 0)
		{
			mouseBlockStacks--;
		}
		if (mouseBlockStacks <= 0)
		{
			blockBackgroundNode.image.enabled = false;
		}
	}

	public void ForceUnblockMouse()
	{
		mouseBlockStacks = 0;
		blockBackgroundNode.image.enabled = false;
	}

	private IEnumerator FadeCoroutine(double fadeBeginTime, double fadeMiddleTime, double fadeEndTime, float darkSecondsAdjustment, Action fadeMiddleAction, Action fadeMiddleEndAction, Action fadeBeginAction, Action fadeEndAction, float darkness, float fps = 30f, float exponent = 2f)
	{
		if (fps < 0.01f)
		{
			fps = 0.01f;
		}
		if (fps > 240f)
		{
			fps = 240f;
		}
		fadeBackgroundNode.image.color = new Color32(0, 0, 0, 0);
		fadeBackgroundNode.image.enabled = true;
		fadeBackgroundNode.image.raycastTarget = true;
		fadeBackgroundNode.rectTransform.SetAsLastSibling();
		SafeFadeInvoke(fadeBeginAction, "begin");
		int count = 0;
		bool isAction = false;
		bool isActionEnd = false;
		while (true)
		{
			if (Time.timeAsDouble < fadeMiddleTime)
			{
				double value = (Time.timeAsDouble - fadeBeginTime) / (fadeMiddleTime - fadeBeginTime);
				value = Math.Clamp(value, 0.0, 1.0);
				value = Math.Pow(value, exponent);
				fadeBackgroundNode.image.color = new Color(0f, 0f, 0f, (float)value * darkness);
			}
			else if (Time.timeAsDouble > fadeMiddleTime + (double)darkSecondsAdjustment)
			{
				if (!isAction)
				{
					isAction = true;
					SafeFadeInvoke(fadeMiddleAction, "middle");
				}
				if (isAction && !isActionEnd)
				{
					isActionEnd = true;
					SafeFadeInvoke(fadeMiddleEndAction, "middle end");
				}
				double value2 = (fadeEndTime - Time.timeAsDouble) / (fadeEndTime - (fadeMiddleTime + (double)darkSecondsAdjustment));
				value2 = Math.Clamp(value2, 0.0, 1.0);
				value2 = Math.Pow(value2, exponent);
				if (value2 < 0.8)
				{
					fadeBackgroundNode.image.raycastTarget = false;
				}
				fadeBackgroundNode.image.color = new Color(0f, 0f, 0f, (float)value2 * darkness);
			}
			else
			{
				fadeBackgroundNode.image.color = new Color(0f, 0f, 0f, darkness);
				yield return new WaitForSeconds(0f);
				if (!isAction)
				{
					isAction = true;
					SafeFadeInvoke(fadeMiddleAction, "middle");
				}
				if (Time.timeAsDouble > fadeMiddleTime + (double)darkSecondsAdjustment)
				{
					double num = Time.timeAsDouble - (fadeMiddleTime + (double)darkSecondsAdjustment);
					fadeMiddleTime += num;
					fadeEndTime += num;
				}
			}
			if (Time.timeAsDouble >= fadeEndTime || (float)count > 1000f * fps)
			{
				break;
			}
			count++;
			yield return new WaitForSeconds(1f / fps);
		}
		fadeBackgroundNode.image.color = new Color32(0, 0, 0, 0);
		fadeBackgroundNode.image.enabled = false;
		isFadeBlock = false;
		SafeFadeInvoke(fadeEndAction, "end");
	}

	private static void SafeFadeInvoke(Action action, string stage)
	{
		if (action == null)
		{
			return;
		}
		try
		{
			action();
		}
		catch (Exception arg)
		{
			Debug.LogError($"[OverlayHandler] fade {stage} action threw: {arg}");
		}
	}

	public void ShowConfirmation(string title = "Confirmation", RichText richText = null, Action yesAction = null, Action noAction = null, bool canYes = true, bool showYes = true, string yes = "Confirm", string no = "Cancel", ButtonElement.Color yesColor = ButtonElement.Color.Green, ButtonElement.Color noColor = ButtonElement.Color.Red)
	{
		if (richText == null)
		{
			richText = new RichTextBuilder().Add("Do you want to confirm this action?").Build();
		}
		confirmYesButton.Set(yes);
		confirmNoButton.Set(no);
		confirmBackgroundNode.image.enabled = true;
		confirmBackgroundNode.image.rectTransform.SetAsLastSibling();
		confirmTextElement.SetRichText(richText);
		if (showYes)
		{
			confirmYesButton.Show();
		}
		else
		{
			confirmYesButton.Hide();
		}
		confirmYesNoElement.Validate();
		confirmWindow.Validate();
		confirmWindow.Show();
		confirmWindow.Center();
		confirmWindow.rectTransform.SetAsLastSibling();
		confirmWindow.SetTitle(title);
		if (canYes)
		{
			confirmYesButton.SetColor(yesColor).Enable();
		}
		else
		{
			confirmYesButton.SetColor(yesColor).Disable();
		}
		confirmNoButton.SetColor(noColor).Enable();
		confirmYesButton.SetCallback(delegate
		{
			HideConfirmation();
			yesAction?.Invoke();
		});
		currentCancelAction = delegate
		{
			HideConfirmation();
			noAction?.Invoke();
		};
		confirmNoButton.SetCallback(currentCancelAction);
	}

	public void HideConfirmation()
	{
		confirmBackgroundNode.image.enabled = false;
		confirmWindow.Hide();
		currentCancelAction = null;
	}

	public void ShowChooseItem(List<GameItem> items, string title = "Choose Item", Action<GameItem> chooseAction = null, Action noAction = null, string no = "Cancel", ButtonElement.Color noColor = ButtonElement.Color.Red, bool isNoDisabled = false)
	{
		ShowChooseItem(items, title, delegate(GameItem i)
		{
			chooseAction?.Invoke(i);
			return true;
		}, noAction, no, noColor, isNoDisabled);
	}

	public void ShowChooseItem(List<GameItem> items, string title = "Choose Item", Func<GameItem, bool> chooseFunc = null, Action noAction = null, string no = "Cancel", ButtonElement.Color noColor = ButtonElement.Color.Red, bool isNoDisabled = false)
	{
		ClearChooseItems();
		itemChooseInventory.AddFastUnchecked(items);
		chooseNoButton.Set(no);
		if (isNoDisabled)
		{
			chooseNoButton.SetColor(noColor).Disable();
		}
		else
		{
			chooseNoButton.SetColor(noColor).Enable();
		}
		confirmBackgroundNode.image.enabled = true;
		confirmBackgroundNode.image.rectTransform.SetAsLastSibling();
		itemChooseWindow.Validate();
		itemChooseInventory.Validate();
		itemChooseWindow.Show();
		itemChooseWindow.Center();
		itemChooseWindow.rectTransform.SetAsLastSibling();
		itemChooseWindow.SetTitle(title);
		RecipeSelectHandler.current.callback = delegate(GameItem item)
		{
			Func<GameItem, bool> func = chooseFunc;
			if (func != null && func(item))
			{
				HideChooseItem();
			}
		};
		currentCancelAction = delegate
		{
			HideChooseItem();
			noAction?.Invoke();
		};
		chooseNoButton.SetCallback(currentCancelAction);
	}

	private void ClearChooseItems()
	{
		foreach (GameItem item in itemChooseInventory.childItems.ToList())
		{
			if (item.IsTag("CHOICE_PREVIEW_TAG"))
			{
				item.Destroy();
			}
			else
			{
				itemChooseInventory.Expel(item);
			}
		}
	}

	public void HideChooseItem()
	{
		RecipeSelectHandler.current.callback = null;
		confirmBackgroundNode.image.enabled = false;
		itemChooseWindow.Hide();
		ClearChooseItems();
		currentCancelAction = null;
	}

	public void ShowTooltip(Vector2 position, RichText richText, int maxCharWidth = 60)
	{
		ShowTooltip(position, Vector2.zero, richText, maxCharWidth);
	}

	public void ShowTooltip2(Vector2 position, Vector2 localTranslation, RichText richText, int maxCharWidth = 60)
	{
		tooltipElement.SetRichText(richText, maxCharWidth);
		tooltipWindow.Validate();
		tooltipWindow.Show(position, localTranslation, fixTooltipBounds: true);
		tooltipWindow.handler.rectTransform.SetAsLastSibling();
	}

	public void ShowTooltip(Vector2 position, Vector2 localTranslation, RichText richText, int maxCharWidth = 60)
	{
		if (richText != null)
		{
			if (!richText.QuickEquals(previousRichTextCache))
			{
				tooltipElement.SetRichText(richText, maxCharWidth);
				tooltipWindow.Validate();
			}
			previousRichTextCache = richText;
			tooltipWindow.Show(position, localTranslation, fixTooltipBounds: true);
			tooltipWindow.ToFront(force: true);
		}
	}

	public void HideTooltip()
	{
		tooltipWindow.Hide();
	}

	public void AppendLog(string text, bool scrollDown = true)
	{
		AppendLog(new RichTextBuilder().Add(text), scrollDown);
	}

	public void AppendLog(RichTextBuilder textBuilder, bool scrollDown = true)
	{
		logElement.AppendRichText(textBuilder);
		if (scrollDown)
		{
			logElement.SetScroll(1f);
		}
	}

	public void ClearLog()
	{
		logElement.SetRichText(new RichTextBuilder());
	}

	public bool Cancel()
	{
		if (currentCancelAction != null)
		{
			currentCancelAction?.Invoke();
			return true;
		}
		return false;
	}

	public PixelWindow GetTooltipWindow()
	{
		return tooltipWindow;
	}
}

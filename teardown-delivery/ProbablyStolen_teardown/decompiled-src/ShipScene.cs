using System;
using UnityEngine;

public class ShipScene
{
	public PixelWindow topBarWindow;

	private static int TIME_WIDTH = 27;

	private static int BAR_HEIGHT = 3;

	public PixelWindow shipWindow;

	public StealPixelElement stealShipElement;

	public GameItemElement shipElement;

	private ButtonElement buttonEvent;

	private ButtonElement buttonDock;

	private ButtonElement buttonNav;

	private ButtonElement buttonJournal;

	private ButtonElement buttonLog;

	private ButtonElement buttonNext;

	private ButtonElement buttonFast;

	private ButtonElement buttonUntil;

	public PixelWindow rightBarWindow;

	public SpritePixelElement spaceBackground;

	private bool isActive;

	public static int defaultButtonWidth = 100;

	public static int defaultButtonHeight = 30;

	private int canvasWidth = -1;

	private int canvasHeight = -1;

	private RichTextElement timeElement { get; }

	public ShipScene()
	{
		spaceBackground = new SpritePixelElement("Backgrounds/space_nebula");
		spaceBackground.handler.SetParentWindowRoot();
		spaceBackground.Hide();
		topBarWindow = new PixelWindow();
		timeElement = new RichTextElement(canResize: false, UISettings.current.fontWidth * TIME_WIDTH, UISettings.current.fontHeight * BAR_HEIGHT);
		topBarWindow.Attach(timeElement);
		shipWindow = new PixelWindow(canMove: false, "Ship");
		stealShipElement = new StealPixelElement();
		shipWindow.Attach(stealShipElement);
		GridPixelElement gridPixelElement = new GridPixelElement(1, 7, isCentered: true);
		GridPixelElement gridPixelElement2 = new GridPixelElement(2, 1, isCentered: true);
		rightBarWindow = new PixelWindow();
		buttonEvent = new ButtonElement("Event", defaultButtonWidth, defaultButtonHeight);
		buttonDock = new ButtonElement("Dock", defaultButtonWidth, defaultButtonHeight);
		buttonNav = new ButtonElement("Navigation", defaultButtonWidth, defaultButtonHeight);
		buttonJournal = new ButtonElement("Journal", defaultButtonWidth, defaultButtonHeight).SetColor(ButtonElement.Color.Purple);
		buttonLog = new ButtonElement("Log", defaultButtonWidth, defaultButtonHeight).SetColor(ButtonElement.Color.Purple);
		buttonNext = new ButtonElement("Wait", defaultButtonWidth, defaultButtonHeight).SetDescription("Wait for 20 minutes").SetColor(ButtonElement.Color.Blue);
		int num = (int)((double)defaultButtonWidth * 0.7);
		int width = defaultButtonWidth - num - rightBarWindow.xPadding;
		buttonFast = new ButtonElement(">>", num, defaultButtonHeight).SetDescription("Wait for 2 hours").SetColor(ButtonElement.Color.Blue);
		buttonUntil = new ButtonElement("@", width, defaultButtonHeight).SetDescription("Smart wait").SetColor(ButtonElement.Color.Blue);
		buttonLog.SetCallback(delegate
		{
			OverlayHandler.current.logWindow?.Show();
			OverlayHandler.current.logWindow?.ToFront();
			OverlayHandler.current.logWindow?.Center();
		});
		buttonEvent.SetCallback(delegate
		{
			OverlayHandler.current.ShowEvent();
		});
		buttonNav.SetCallback(delegate
		{
			OverlayHandler.current.ShowNav();
		});
		Action departAction = delegate
		{
			OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
			{
				GameMaster.current.shipManager.Dock();
			}, null, delegate
			{
				AudioManager.Instance.Play("SHIP_DOCK");
			});
		};
		buttonDock.SetCallback(delegate
		{
			OverlayHelper.CheckDockValidParty(delegate
			{
				OverlayHelper.ConfirmDock(departAction);
			});
		});
		buttonNext.SetCallback(delegate
		{
			OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
			{
				GameMaster.current.turnManager.NextTurn();
			});
		});
		buttonFast.SetCallback(delegate
		{
			OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
			{
				GameMaster.current.turnManager.NextNTurns(10);
			});
		});
		buttonUntil.SetCallback(delegate
		{
			OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
			{
				if (GameMaster.current.shipManager.isTransit)
				{
					int transitETA = GameMaster.current.shipManager.transitETA;
					for (int i = 0; i < transitETA; i++)
					{
						GameMaster.current.turnManager.NextTurn();
					}
				}
				else
				{
					int turnUntilCharged = NavigationHelper.GetTurnUntilCharged(NavigationHelper.GetActiveNavSystem());
					for (int j = 0; j < turnUntilCharged; j++)
					{
						GameMaster.current.turnManager.NextTurn();
					}
				}
			});
		});
		gridPixelElement.Attach(buttonEvent);
		gridPixelElement.Attach(buttonDock);
		gridPixelElement.Attach(buttonNav);
		gridPixelElement.Attach(buttonJournal);
		gridPixelElement.Attach(buttonLog);
		gridPixelElement.Attach(buttonNext);
		gridPixelElement2.Attach(buttonFast);
		gridPixelElement2.Attach(buttonUntil);
		gridPixelElement2.Validate();
		gridPixelElement.Attach(gridPixelElement2);
		gridPixelElement.Validate();
		rightBarWindow.Attach(gridPixelElement);
		rightBarWindow.handler.SetParentWindowRoot();
		rightBarWindow.Validate();
		gridPixelElement.Validate();
		gridPixelElement2.Validate();
		Reset();
	}

	public void UpdateBlockedButtons()
	{
		ShipManager shipManager = GameMaster.current.shipManager;
		RaidManager raidManager = GameMaster.current.raidManager;
		EventData currentEvent = shipManager.currentEvent;
		bool num = currentEvent != null && currentEvent.IsRunning() && (currentEvent?.blockTurns ?? false);
		bool num2 = (currentEvent != null && currentEvent.IsRunning() && currentEvent != null && currentEvent.blockTravel) || shipManager.isTransit;
		bool num3 = (currentEvent != null && currentEvent.IsRunning() && currentEvent != null && currentEvent.blockStation) || shipManager.isTransit || raidManager.station == null;
		bool num4 = !(currentEvent?.IsRunning() ?? false);
		bool hasEventRun = shipManager.hasEventRun;
		if (!num4)
		{
			buttonEvent.SetColor(ButtonElement.Color.Red).Enable();
		}
		else if (hasEventRun)
		{
			buttonEvent.SetColor(ButtonElement.Color.None).Enable();
		}
		else
		{
			buttonEvent.SetColor(ButtonElement.Color.None).Disable();
		}
		if (!num3)
		{
			if (buttonDock.isDisabled)
			{
				buttonDock.Enable();
			}
		}
		else if (!buttonDock.isDisabled)
		{
			buttonDock.Disable();
		}
		if (!num2)
		{
			if (buttonNav.isDisabled)
			{
				buttonNav.Set("Navigation").Enable();
			}
		}
		else
		{
			if (!buttonNav.isDisabled)
			{
				buttonNav.Disable();
			}
			if (shipManager.isTransit)
			{
				buttonNav.Set("ETA: " + shipManager.transitETA);
			}
			else
			{
				buttonNav.Set("Navigation");
			}
		}
		if (!num)
		{
			if (buttonNext.isDisabled)
			{
				buttonNext.Enable();
			}
		}
		else if (!buttonNext.isDisabled)
		{
			buttonNext.Disable();
		}
	}

	public void Validate(bool force = true)
	{
		if (!isActive)
		{
			return;
		}
		Debug.Log("Validated From ShipScene");
		RectTransform component = RenderHandler.current.windowUICanvasObject.GetComponent<RectTransform>();
		int num = (int)component.sizeDelta.x;
		int num2 = (int)component.sizeDelta.y;
		UpdateBlockedButtons();
		OverlayHelper.UpdateTopRightElement(timeElement);
		if (num != canvasWidth || num2 != canvasHeight || force)
		{
			while (num2 > 540 || num > 960)
			{
				RenderHandler.current.windowUICanvas.scaleFactor += 1f;
				num = (int)component.sizeDelta.x;
				num2 = (int)component.sizeDelta.y;
			}
			while ((num2 < 500 || num < 900) && RenderHandler.current.windowUICanvas.scaleFactor > 1f)
			{
				RenderHandler.current.windowUICanvas.scaleFactor -= 1f;
				num = (int)component.sizeDelta.x;
				num2 = (int)component.sizeDelta.y;
			}
			num = (int)component.sizeDelta.x;
			num2 = (int)component.sizeDelta.y;
			topBarWindow.Validate();
			topBarWindow.rectTransform.anchoredPosition = new Vector2(num - topBarWindow.widthPixels, 0f);
			int heightPixels = topBarWindow.heightPixels;
			rightBarWindow.Validate();
			rightBarWindow.rectTransform.anchoredPosition = new Vector2(num - rightBarWindow.widthPixels, -num2 + rightBarWindow.heightPixels);
			shipElement.rectTransform.anchoredPosition = new Vector2(0f, 0f);
			shipElement.Validate();
			stealShipElement.Validate();
			shipWindow.Validate();
			int num3 = num - rightBarWindow.widthPixels - shipElement.widthPixels - shipWindow.widthPixels;
			int num4 = (num2 - topBarWindow.heightPixels - shipWindow.heightPixels) / 2;
			int num5 = num3 / 2;
			shipWindow.rectTransform.anchoredPosition = new Vector2(shipElement.widthPixels + num5, -heightPixels - num4);
			spaceBackground.rectTransform.anchoredPosition = Vector2.zero;
			spaceBackground.background.image.SetNativeSize();
			spaceBackground.rectTransform.anchoredPosition = new Vector2(((float)num - spaceBackground.background.rectTransform.sizeDelta.x) / 2f, (0f - ((float)num2 - spaceBackground.background.rectTransform.sizeDelta.y)) / 2f);
			spaceBackground.background.rectTransform.localScale = new Vector2(1f, 1f);
			if ((float)num > spaceBackground.background.rectTransform.sizeDelta.x || (float)num2 > spaceBackground.background.rectTransform.sizeDelta.y)
			{
				spaceBackground.background.rectTransform.localScale = new Vector2(2f, 2f);
				spaceBackground.rectTransform.anchoredPosition = new Vector2(((float)num - spaceBackground.background.rectTransform.sizeDelta.x * 2f) / 2f, (0f - ((float)num2 - spaceBackground.background.rectTransform.sizeDelta.y * 2f)) / 2f);
			}
		}
		canvasWidth = num;
		canvasHeight = num2;
	}

	public void Reset()
	{
		isActive = false;
		EndAllAnimations();
		if (shipElement != null)
		{
			shipElement.handler.SetParentTempRoot();
			shipElement.rectTransform.anchoredPosition = Vector2.zero;
		}
		topBarWindow.Hide();
		rightBarWindow.Hide();
		shipWindow.Hide();
		stealShipElement.UnStealItem();
		ItemDefaultActivateHandler.current.Disable();
		ItemMouseDoubleClickHandler.current.Disable();
		ItemMouseDragHandler.current.Disable();
		ItemMouseHandler.current.Disable();
		ItemQuickTransferHandler.current.Disable();
		ItemSelectHandler.current.Disable();
		ItemTooltipHandler.current.Disable();
		ItemContextHandler.current.Disable();
		spaceBackground.Hide();
		WindowsHandler.HideAllMovable();
	}

	public void StartShip(GameItem ship)
	{
		Reset();
		if (ship != null)
		{
			if (ship is GameItemElement gameItemElement)
			{
				shipElement = gameItemElement;
				shipElement.parentInventory?.Expel(shipElement);
				shipElement.handler.SetParentWindowRoot();
				shipElement.rectTransform.anchoredPosition = Vector2.zero;
			}
			topBarWindow.Show();
			rightBarWindow.Show();
			stealShipElement.StealItem(ship);
			shipWindow.SetTitle(ship.GetDisplayName());
			shipWindow.Validate();
			shipWindow.Show();
			ItemDefaultActivateHandler.current.Enable();
			ItemMouseDoubleClickHandler.current.Enable();
			ItemMouseDragHandler.current.Enable();
			ItemMouseHandler.current.Enable();
			ItemQuickTransferHandler.current.Enable();
			ItemSelectHandler.current.Enable();
			ItemSelectHandler.current.auto = false;
			ItemTooltipHandler.current.Enable();
			ItemContextHandler.current.Enable();
			spaceBackground.ToBack();
			spaceBackground.Show();
			isActive = true;
			Validate();
		}
	}

	public ShipScene SetShipWindowTransparency(float shipWindowTransparency)
	{
		shipWindow.SetTransparency(shipWindowTransparency);
		return this;
	}

	public float GetShipWindowTransparency()
	{
		return shipWindow.transparency;
	}

	public void EndAllAnimations()
	{
	}
}

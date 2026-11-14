using System.Collections.Generic;
using UnityEngine;

public class MenuScene
{
	public static int DEFAULT_PADDING = 10;

	public PixelWindow rightBarWindow;

	public PixelWindow nextButtonWindow;

	public PixelWindow prevButtonWindow;

	private ButtonElement nextButton;

	private ButtonElement prevButton;

	public PixelWindow topBarWindow;

	public RichTextElement topBarText;

	private static int BAR_WIDTH = 54;

	private static int BAR_HEIGHT = 3;

	private int selectState;

	public PixelWindow difficultySelectWindow;

	public PixelWindow factionSelectWindow;

	public GameItemElement shipElement;

	public PixelWindow scenarioDescriptionWindow;

	public ScrollableRichTextElement scenarioDescription;

	private PixelWindow shipNextButtonWindow;

	private PixelWindow shipPrevButtonWindow;

	private ButtonElement shipNextButton;

	private ButtonElement shipPrevButton;

	public static int defaultShipButtonWidth = 40;

	public static int defaultShipButtonHeight = 40;

	private int scenarioSelectState;

	private List<Scenario> availableScenarios;

	public PixelWindow rightCharacterSelectWindow;

	public PixelWindow shipWindow;

	public StealPixelElement stealShipElement;

	private ButtonElement buttonContinue;

	private bool isActive;

	public static int defaultButtonWidth = 120;

	public static int defaultButtonHeight = 40;

	public SpritePixelElement spaceBackground;

	private int canvasWidth = -1;

	private int canvasHeight = -1;

	private void InitMain()
	{
		GridPixelElement gridPixelElement = new GridPixelElement(1, 5);
		buttonContinue = new ButtonElement("Continue", defaultButtonWidth, defaultButtonHeight);
		ButtonElement buttonElement = new ButtonElement("New Game", defaultButtonWidth, defaultButtonHeight);
		ButtonElement element = new ButtonElement("Options", defaultButtonWidth, defaultButtonHeight);
		ButtonElement buttonElement2 = new ButtonElement("Quit", defaultButtonWidth, defaultButtonHeight);
		buttonElement2.SetCallback(delegate
		{
			GameMaster.current.QuitToDesktop();
		});
		buttonElement2.SetColor(ButtonElement.Color.Red);
		buttonContinue.Disable();
		buttonContinue.SetCallback(delegate
		{
			if (SaveManager.HasSave())
			{
				OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
				{
					GameMaster.current.ContinueGame();
				});
			}
		});
		buttonElement.SetCallback(delegate
		{
			StartFactionSelect();
		});
		gridPixelElement.Attach(buttonContinue);
		gridPixelElement.Attach(buttonElement);
		gridPixelElement.Attach(element);
		gridPixelElement.Attach(buttonElement2);
		gridPixelElement.Validate();
		rightBarWindow = new PixelWindow();
		rightBarWindow.Attach(gridPixelElement);
		rightBarWindow.Validate();
	}

	private void InitSelect()
	{
		nextButtonWindow = new PixelWindow();
		prevButtonWindow = new PixelWindow();
		topBarWindow = new PixelWindow();
		topBarWindow.AddInventoryStateChangeAction(delegate
		{
			UpdateTopBarText();
		});
		nextButton = new ButtonElement("Next", defaultButtonWidth, defaultButtonHeight);
		prevButton = new ButtonElement("Back", defaultButtonWidth, defaultButtonHeight);
		nextButton.SetCallback(delegate
		{
			switch (selectState)
			{
			case 1:
				StartScenarioSelect();
				break;
			case 2:
				StartCharSelect();
				break;
			case 3:
				StartEquipmentSelect();
				break;
			case 4:
				OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder().Add("Are you sure you want to start the game?").Build(), delegate
				{
					AudioHelper.ShuttleJump();
					OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
					{
						GameMaster.current.NewGame();
					});
				});
				break;
			default:
				StartMenu();
				break;
			}
		});
		prevButton.SetCallback(delegate
		{
			switch (selectState)
			{
			case 2:
				StartFactionSelect();
				break;
			case 3:
				StartScenarioSelect();
				break;
			case 4:
				StartCharSelect();
				break;
			default:
				OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder().Add("Are you sure you want to cancel and go back to the main menu?").Build(), StartMenu);
				break;
			}
		});
		nextButtonWindow.Attach(nextButton);
		prevButtonWindow.Attach(prevButton);
		topBarText = new RichTextElement(canResize: false, UISettings.current.fontWidth * BAR_WIDTH, UISettings.current.fontHeight * BAR_HEIGHT);
		topBarWindow.Attach(topBarText);
	}

	private void InitFactionSelect()
	{
		difficultySelectWindow = new PixelWindow();
		factionSelectWindow = new PixelWindow();
	}

	private void InitScenarioSelect()
	{
		scenarioDescriptionWindow = new PixelWindow();
		scenarioDescription = new ScrollableRichTextElement(constWidth: true);
		scenarioDescriptionWindow.Attach(scenarioDescription);
		shipNextButtonWindow = new PixelWindow();
		shipPrevButtonWindow = new PixelWindow();
		shipNextButton = new ButtonElement(">", defaultShipButtonWidth, defaultButtonHeight, null, ButtonElement.Color.None, "click_vintage");
		shipPrevButton = new ButtonElement("<", defaultShipButtonWidth, defaultButtonHeight, null, ButtonElement.Color.None, "click_vintage");
		shipNextButtonWindow.Attach(shipNextButton);
		shipPrevButtonWindow.Attach(shipPrevButton);
		shipNextButton.SetCallback(delegate
		{
			scenarioSelectState++;
			scenarioSelectState %= availableScenarios.Count;
			UpdateScenario();
		});
		shipPrevButton.SetCallback(delegate
		{
			scenarioSelectState--;
			scenarioSelectState = (scenarioSelectState + availableScenarios.Count) % availableScenarios.Count;
			UpdateScenario();
		});
	}

	private void InitCharacterSelect()
	{
		rightCharacterSelectWindow = new PixelWindow();
		shipWindow = new PixelWindow();
		stealShipElement = new StealPixelElement();
		shipWindow.Attach(stealShipElement);
	}

	public MenuScene()
	{
		spaceBackground = new SpritePixelElement("Backgrounds/space_nebula");
		spaceBackground.handler.SetParentWindowRoot();
		spaceBackground.Hide();
		InitMain();
		InitSelect();
		InitFactionSelect();
		InitScenarioSelect();
		InitCharacterSelect();
		Reset();
	}

	public void Validate(bool force = true)
	{
		if (!isActive)
		{
			return;
		}
		Debug.Log("Validated From MenuScene");
		RectTransform component = RenderHandler.current.windowUICanvasObject.GetComponent<RectTransform>();
		int num = (int)component.sizeDelta.x;
		int num2 = (int)component.sizeDelta.y;
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
			rightBarWindow.Validate();
			rightBarWindow.rectTransform.anchoredPosition = new Vector2((num - rightBarWindow.widthPixels) / 8 * 7, -(num2 - rightBarWindow.heightPixels) / 2);
			topBarWindow.Validate();
			topBarWindow.rectTransform.anchoredPosition = new Vector2((num - topBarWindow.widthPixels) / 2, -DEFAULT_PADDING);
			nextButtonWindow.Validate();
			nextButtonWindow.rectTransform.anchoredPosition = topBarWindow.rectTransform.anchoredPosition + new Vector2(topBarWindow.widthPixels + DEFAULT_PADDING, -(topBarWindow.heightPixels - nextButtonWindow.heightPixels) / 2);
			prevButtonWindow.Validate();
			prevButtonWindow.rectTransform.anchoredPosition = topBarWindow.rectTransform.anchoredPosition + new Vector2(-nextButtonWindow.widthPixels - DEFAULT_PADDING, -(topBarWindow.heightPixels - prevButtonWindow.heightPixels) / 2);
			scenarioDescriptionWindow.ResizePixels(num / 3, num2 - (int)(0f - topBarWindow.rectTransform.anchoredPosition.y) - topBarWindow.heightPixels - DEFAULT_PADDING * 2);
			scenarioDescriptionWindow.rectTransform.anchoredPosition = new Vector2(DEFAULT_PADDING, topBarWindow.rectTransform.anchoredPosition.y - (float)topBarWindow.heightPixels - (float)DEFAULT_PADDING);
			shipNextButtonWindow.Validate();
			shipNextButtonWindow.rectTransform.anchoredPosition = new Vector2(num - shipNextButtonWindow.widthPixels - DEFAULT_PADDING, scenarioDescriptionWindow.rectTransform.anchoredPosition.y - (float)((scenarioDescriptionWindow.heightPixels - shipNextButtonWindow.heightPixels) / 2));
			shipPrevButtonWindow.Validate();
			shipPrevButtonWindow.rectTransform.anchoredPosition = new Vector2(scenarioDescriptionWindow.rectTransform.anchoredPosition.x + (float)scenarioDescriptionWindow.widthPixels + (float)DEFAULT_PADDING, scenarioDescriptionWindow.rectTransform.anchoredPosition.y - (float)((scenarioDescriptionWindow.heightPixels - shipNextButtonWindow.heightPixels) / 2));
			float num3 = shipPrevButtonWindow.rectTransform.anchoredPosition.x + (float)shipPrevButtonWindow.widthPixels + (float)DEFAULT_PADDING;
			float num4 = shipNextButtonWindow.rectTransform.anchoredPosition.x - (float)DEFAULT_PADDING;
			float num5 = 0f - topBarWindow.rectTransform.anchoredPosition.y + (float)topBarWindow.heightPixels + (float)DEFAULT_PADDING;
			float num6 = num2 - DEFAULT_PADDING;
			if (shipElement != null && selectState == 2)
			{
				shipElement.parentInventory?.Expel(shipElement);
				shipElement.handler.SetParentWindowRoot();
				float y = 0f - num5;
				if ((float)shipElement.heightPixels < num6 - num5)
				{
					y = 0f - (num5 + (num6 - num5 - (float)shipElement.heightPixels) / 2f);
				}
				shipElement.rectTransform.anchoredPosition = new Vector2(num3 + (num4 - num3 - (float)shipElement.widthPixels) / 2f, y);
			}
			rightCharacterSelectWindow.Validate();
			shipWindow.Validate();
			float num7 = (num - (rightCharacterSelectWindow.widthPixels + DEFAULT_PADDING + shipWindow.widthPixels)) / 2;
			shipWindow.rectTransform.anchoredPosition = new Vector2(num7, topBarWindow.rectTransform.anchoredPosition.y - (float)topBarWindow.heightPixels - (float)DEFAULT_PADDING);
			rightCharacterSelectWindow.rectTransform.anchoredPosition = new Vector2(num7 + (float)shipWindow.widthPixels + (float)DEFAULT_PADDING, topBarWindow.rectTransform.anchoredPosition.y - (float)topBarWindow.heightPixels - (float)DEFAULT_PADDING);
		}
		canvasWidth = num;
		canvasHeight = num2;
	}

	public void Reset()
	{
		selectState = 0;
		isActive = false;
		EndAllAnimations();
		if (shipElement != null)
		{
			shipElement.handler.SetParentTempRoot();
			shipElement.rectTransform.anchoredPosition = Vector2.zero;
		}
		rightBarWindow.Hide();
		nextButtonWindow.Hide();
		prevButtonWindow.Hide();
		topBarWindow.Hide();
		prevButton.Set("Back").SetColor(ButtonElement.Color.None);
		nextButton.Set("Next").SetColor(ButtonElement.Color.None);
		difficultySelectWindow.Hide();
		factionSelectWindow.Hide();
		shipNextButtonWindow.Hide();
		shipPrevButtonWindow.Hide();
		scenarioDescriptionWindow.Hide();
		rightCharacterSelectWindow.Hide();
		rightCharacterSelectWindow.Detach();
		shipWindow.Hide();
		stealShipElement.UnStealItem();
		spaceBackground.Hide();
		WindowsHandler.HideAllMovable();
		ItemDefaultActivateHandler.current.Disable();
		ItemMouseDoubleClickHandler.current.Disable();
		ItemMouseDragHandler.current.Disable();
		ItemMouseHandler.current.Disable();
		ItemQuickTransferHandler.current.Disable();
		ItemSelectHandler.current.Disable();
		ItemTooltipHandler.current.Disable();
		ItemContextHandler.current.Disable();
	}

	public void StartMenu()
	{
		Reset();
		scenarioSelectState = 0;
		availableScenarios = null;
		shipElement = null;
		rightBarWindow.Show();
		if (SaveManager.HasSave())
		{
			buttonContinue.SetColor(ButtonElement.Color.Green).Enable();
		}
		else
		{
			buttonContinue.SetColor(ButtonElement.Color.None).Disable();
		}
		spaceBackground.ToBack();
		spaceBackground.Show();
		isActive = true;
		Validate();
	}

	public void StartFactionSelect()
	{
		Reset();
		selectState = 1;
		UpdateTopBarText();
		prevButton.Set("Cancel").SetColor(ButtonElement.Color.Red);
		nextButtonWindow.Show();
		prevButtonWindow.Show();
		topBarWindow.Show();
		spaceBackground.ToBack();
		spaceBackground.Show();
		isActive = true;
		Validate();
	}

	private void ResetScenarios()
	{
		if (shipElement != null)
		{
			shipElement.handler.SetParentTempRoot();
			shipElement.rectTransform.anchoredPosition = Vector2.zero;
		}
		scenarioSelectState = 0;
		availableScenarios = null;
		shipElement = null;
		GameMaster.current.menuManager.selectedScenario = null;
		availableScenarios = GameMaster.current.menuManager.GetAvailableFactionScenarios();
	}

	private void UpdateScenario()
	{
		if (shipElement != null)
		{
			shipElement.handler.SetParentTempRoot();
			shipElement.rectTransform.anchoredPosition = Vector2.zero;
		}
		GameMaster.current.menuManager.selectedScenario = availableScenarios[scenarioSelectState];
		if (GameMaster.current.menuManager.selectedScenario.ship is GameItemElement gameItemElement)
		{
			shipElement = gameItemElement;
			shipElement.parentInventory?.Expel(shipElement);
			shipElement.handler.SetParentWindowRoot();
			shipElement.rectTransform.anchoredPosition = Vector2.zero;
		}
		scenarioDescription.SetRichText(GameMaster.current.menuManager.selectedScenario.richTextDescription);
		UpdateTopBarText();
		Validate();
	}

	public void StartScenarioSelect()
	{
		Reset();
		selectState = 2;
		UpdateTopBarText();
		nextButtonWindow.Show();
		prevButtonWindow.Show();
		topBarWindow.Show();
		if (availableScenarios == null)
		{
			ResetScenarios();
		}
		UpdateScenario();
		scenarioDescriptionWindow.Show();
		shipPrevButtonWindow.Show();
		shipNextButtonWindow.Show();
		spaceBackground.ToBack();
		spaceBackground.Show();
		isActive = true;
		Validate();
	}

	public void StartCharSelect()
	{
		Reset();
		selectState = 3;
		UpdateTopBarText();
		stealShipElement.StealItem(GameMaster.current.menuManager.selectedScenario.ship);
		shipWindow.SetTitle(GameMaster.current.menuManager.selectedScenario.ship.GetDisplayName());
		GraphUtils.FindAllChildrenType<GameInventory>(GameMaster.current.menuManager.selectedScenario.ship).ForEach(delegate(GameInventory inv)
		{
			inv.Validate();
		});
		shipWindow.Validate();
		shipWindow.Show();
		rightCharacterSelectWindow.Attach(GameMaster.current.menuManager.selectedScenario.selectableCharacters);
		rightCharacterSelectWindow.Show();
		nextButtonWindow.Show();
		prevButtonWindow.Show();
		topBarWindow.Show();
		spaceBackground.ToBack();
		spaceBackground.Show();
		isActive = true;
		Validate();
		ItemDefaultActivateHandler.current.Enable();
		ItemMouseDoubleClickHandler.current.Enable();
		ItemMouseDragHandler.current.Enable();
		ItemMouseHandler.current.Enable();
		ItemQuickTransferHandler.current.Enable();
		ItemSelectHandler.current.Enable();
		ItemTooltipHandler.current.Enable();
		ItemContextHandler.current.Enable();
	}

	public void StartEquipmentSelect()
	{
		Reset();
		selectState = 4;
		UpdateTopBarText();
		nextButton.Set("Begin!").SetColor(ButtonElement.Color.Red);
		nextButtonWindow.Show();
		prevButtonWindow.Show();
		topBarWindow.Show();
		spaceBackground.ToBack();
		spaceBackground.Show();
		isActive = true;
		Validate();
	}

	private void UpdateTopBarText()
	{
		RichTextBuilder richTextBuilder = new RichTextBuilder();
		switch (selectState)
		{
		case 1:
			richTextBuilder.AddLine(OverlayHelper.GetLineCentered(BAR_WIDTH, "Faction Selection"), ignoreEmpty: true, RenderHandler.ColorPalette.White, bold: true);
			break;
		case 2:
			richTextBuilder.AddLine(OverlayHelper.GetLineCentered(BAR_WIDTH, "Scenario Selection"), ignoreEmpty: true, RenderHandler.ColorPalette.White, bold: true);
			break;
		case 3:
			richTextBuilder.AddLine(OverlayHelper.GetLineCentered(BAR_WIDTH, "Character Selection"), ignoreEmpty: true, RenderHandler.ColorPalette.White, bold: true);
			break;
		case 4:
			richTextBuilder.AddLine(OverlayHelper.GetLineCentered(BAR_WIDTH, "Equipment Selection"), ignoreEmpty: true, RenderHandler.ColorPalette.White, bold: true);
			break;
		}
		long remainingPoints = GetRemainingPoints();
		string text = "Points: ";
		richTextBuilder.Add(OverlayHelper.GetSpacesCentered(BAR_WIDTH, text + remainingPoints));
		richTextBuilder.Add(text);
		richTextBuilder.Add(remainingPoints.ToString(), (remainingPoints >= 0) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightRed);
		topBarText.SetRichText(richTextBuilder.Build());
	}

	private long GetRemainingPoints()
	{
		if (GameMaster.current.menuManager.selectedScenario == null)
		{
			return 0L;
		}
		List<GameInventory> list = GraphUtils.FindChildrenType<GameInventory>(GameMaster.current.menuManager.selectedScenario.ship);
		long num = 0L;
		foreach (GameInventory item in list)
		{
			num += item.GetTotalValue();
		}
		long value = GameMaster.current.menuManager.selectedScenario.ship.GetValue();
		return GameMaster.current.menuManager.selectedScenario.totalPoints - value - num;
	}

	public void EndAllAnimations()
	{
	}
}

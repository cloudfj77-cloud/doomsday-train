using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class RaidScene
{
	public PixelWindow topBarWindow;

	private static int LOG_WIDTH = 27;

	private static int TIME_WIDTH = 27;

	private static int BAR_HEIGHT = 3;

	public PixelWindow groundLootWindow;

	public PixelWindow shipWindow;

	public ButtonElement shipButton;

	public ButtonElement undockButton;

	public ButtonElement loadButton;

	public PixelWindow playerPartyWindow;

	public PixelWindow selectionInventoryWindow;

	public StealPixelElement stealInventorySelectElement;

	public PixelWindow selectionAbilityWindow;

	public StealPixelElement stealAbilitySelectElement;

	public GameCharacterItem currentCharacter;

	public PixelWindow npcPartyWindow;

	public PixelWindow dialogueWindow;

	public ScrollableRichTextElement dialogueElement;

	public Dialogue currentDialogue;

	private float currentDialogueAnimationLength;

	private bool isDialogueAnimationCoroutineStarted;

	public PixelWindow portraitWindow;

	public SpritePixelElement portraitElement;

	public PixelWindow responseWindow;

	public GameGridScrollableInventory responseInventory;

	public GameGridScrollableInventory responseInventoryBackup;

	private readonly TreeNodeRender selectArrow;

	public PixelWindow rightBarWindow;

	public PixelWindow rightBarWindowFill;

	private ButtonElement buttonSearch;

	private ButtonElement buttonJournal;

	private ButtonElement buttonLog;

	private ButtonElement buttonNext;

	private ButtonElement buttonFast;

	private ButtonElement buttonUntil;

	private bool isActive;

	public static int defaultButtonWidth = 100;

	public static int defaultShipButtonWidth = 200;

	public static int defaultButtonHeight = 30;

	private int canvasWidth = -1;

	private int canvasHeight = -1;

	public bool dialogueNew;

	private RichTextElement logElement { get; }

	private RichTextElement newsElement { get; }

	private RichTextElement timeElement { get; }

	public RaidScene()
	{
		topBarWindow = new PixelWindow();
		GridPixelElement gridPixelElement = new GridPixelElement(3, 1);
		logElement = new RichTextElement(canResize: false, UISettings.current.fontWidth * LOG_WIDTH, UISettings.current.fontHeight * BAR_HEIGHT);
		newsElement = new RichTextElement(canResize: false, -1, UISettings.current.fontHeight * BAR_HEIGHT);
		timeElement = new RichTextElement(canResize: false, UISettings.current.fontWidth * TIME_WIDTH, UISettings.current.fontHeight * BAR_HEIGHT);
		gridPixelElement.Attach(logElement);
		gridPixelElement.Attach(newsElement);
		gridPixelElement.Attach(timeElement);
		topBarWindow.Attach(gridPixelElement);
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 5, isCentered: true);
		GridPixelElement gridPixelElement3 = new GridPixelElement(2, 1, isCentered: true);
		rightBarWindow = new PixelWindow();
		rightBarWindowFill = new PixelWindow();
		rightBarWindowFill.Attach(new EmptyPixelElement());
		buttonSearch = new ButtonElement("Search", defaultButtonWidth, defaultButtonHeight).SetDescription("Search current location for 20 minutes").SetColor(ButtonElement.Color.None);
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
		buttonSearch.SetCallback(delegate
		{
			if (!GameMaster.current.InCombat())
			{
				OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
				{
					GameMaster.current.turnManager.NextTurn();
				});
			}
		});
		buttonNext.SetCallback(delegate
		{
			if (!GameMaster.current.InCombat())
			{
				OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
				{
					GameMaster.current.turnManager.NextTurn();
				});
			}
		});
		buttonFast.SetCallback(delegate
		{
			if (!GameMaster.current.InCombat())
			{
				OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
				{
					GameMaster.current.turnManager.NextNTurns(10);
				});
			}
		});
		gridPixelElement2.Attach(buttonSearch);
		gridPixelElement2.Attach(buttonJournal);
		gridPixelElement2.Attach(buttonLog);
		gridPixelElement2.Attach(buttonNext);
		gridPixelElement3.Attach(buttonFast);
		gridPixelElement3.Attach(buttonUntil);
		gridPixelElement2.Attach(gridPixelElement3);
		rightBarWindow.Attach(gridPixelElement2);
		rightBarWindow.handler.SetParentWindowRoot();
		rightBarWindow.Validate();
		gridPixelElement2.Validate();
		gridPixelElement3.Validate();
		groundLootWindow = new PixelWindow(canMove: false, "Ground");
		shipWindow = new PixelWindow(canMove: false, "Ship");
		GridPixelElement gridPixelElement4 = new GridPixelElement(1, 3);
		undockButton = new ButtonElement("Undock", defaultShipButtonWidth, defaultButtonHeight, null, ButtonElement.Color.Red);
		loadButton = new ButtonElement("Load Party", defaultShipButtonWidth, defaultButtonHeight);
		shipButton = new ButtonElement("Access Ship", defaultShipButtonWidth, defaultButtonHeight);
		gridPixelElement4.Attach(undockButton);
		gridPixelElement4.Attach(loadButton);
		gridPixelElement4.Attach(shipButton);
		shipWindow.Attach(gridPixelElement4);
		shipButton.SetCallback(delegate
		{
			ItemMouseDoubleClickHandler.current.OpenContentAction(GameMaster.current.shipManager.ship, Input.mousePosition);
		});
		loadButton.SetCallback(delegate
		{
			foreach (GameCharacterItem characterItem in GameMaster.current.raidManager.playerParty.GetCharacterItems())
			{
				GameMaster.current.shipManager.ship.TryAcceptAll(characterItem);
			}
		});
		Action recallAction = delegate
		{
			OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
			{
				GameMaster.current.shipManager.Undock();
			}, null, delegate
			{
				AudioManager.Instance.Play("SHIP_UNDOCK");
			});
		};
		undockButton.SetCallback(delegate
		{
			OverlayHelper.CheckUndockValidParty(delegate
			{
				OverlayHelper.ConfirmUndock(delegate
				{
					OverlayHelper.WarnUndockMissingCharacters(recallAction);
				});
			});
		});
		playerPartyWindow = new PixelWindow();
		playerPartyWindow.SetTransparency(1f);
		selectionInventoryWindow = new PixelWindow(canMove: false, "Inventory (None)");
		stealInventorySelectElement = new StealPixelElement();
		selectionInventoryWindow.Attach(stealInventorySelectElement);
		selectionAbilityWindow = new PixelWindow(canMove: false, "Abilities");
		stealAbilitySelectElement = new StealPixelElement();
		selectionAbilityWindow.Attach(stealAbilitySelectElement);
		npcPartyWindow = new PixelWindow();
		npcPartyWindow.SetTransparency(1f);
		dialogueWindow = new PixelWindow(canMove: false, "Dialogue");
		dialogueElement = new ScrollableRichTextElement();
		dialogueWindow.Attach(dialogueElement);
		portraitWindow = new PixelWindow(canMove: false, "Name");
		portraitElement = new SpritePixelElement("Portraits/Unknown", "Idle");
		portraitWindow.Attach(portraitElement);
		portraitElement.Validate();
		portraitWindow.Validate();
		responseWindow = new PixelWindow(canMove: false, "Choices");
		responseInventory = new GameGridScrollableInventory(8, 12, 400, 0.1f);
		responseInventoryBackup = new GameGridScrollableInventory(8, 12, 400, 0.1f);
		responseWindow.Attach(responseInventory);
		responseInventory.onActivateSlotItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			currentDialogue?.FindChoiceAction(item)?.Invoke();
			Dialogue dialogue = currentDialogue?.FindChoiceDialogue(item);
			currentDialogue.endAction?.Invoke();
			if (dialogue != null && !dialogue.isDummyChoiceDialogue)
			{
				GameMaster.current.raidScene.UpdateDialogue(dialogue);
				dialogue.startAction?.Invoke();
			}
			else
			{
				GameMaster.current.raidManager.EnterRoom(GameMaster.current.raidManager.currentRoom);
			}
		};
		responseInventory.onSlotItemTargetItemFunc = delegate(GameItem item, GameInventory inv, SlotMarker slot, GameItem oitem)
		{
			item?.ActivateSlotItem();
		};
		selectArrow = TreeNodeRender.Instantiate().SetParent(RenderHandler.current.overlayRoot);
		selectArrow.image.sprite = RenderHandler.LoadFromAtlas("Items/characteratlas", "arrow");
		selectArrow.image.SetNativeSize();
		Reset();
	}

	private void UpdateShipButtons()
	{
		if ((GameMaster.current.raidManager.playerParty?.childItems?.Count()).GetValueOrDefault() > 0)
		{
			if (loadButton.isDisabled)
			{
				loadButton.SetColor(ButtonElement.Color.Green).Enable();
			}
		}
		else if (!loadButton.isDisabled)
		{
			loadButton.SetColor(ButtonElement.Color.None).Disable();
		}
		if ((from gci in GraphUtils.FindChildrenType<GameCharacterItem>(GameMaster.current.shipManager.ship)
			select !gci.isDead).Any())
		{
			if (undockButton.isDisabled)
			{
				undockButton.SetColor(ButtonElement.Color.Red).Enable();
			}
		}
		else if (!undockButton.isDisabled)
		{
			undockButton.SetColor(ButtonElement.Color.None).Disable();
		}
	}

	public void Validate(bool force = true)
	{
		if (!isActive)
		{
			return;
		}
		Debug.Log("Validated From RaidScene");
		RectTransform component = RenderHandler.current.windowUICanvasObject.GetComponent<RectTransform>();
		int num = (int)component.sizeDelta.x;
		int num2 = (int)component.sizeDelta.y;
		UpdateShipButtons();
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
			topBarWindow.rectTransform.anchoredPosition = new Vector2(0f, 0f);
			int fixedWidth = num - topBarWindow.xPadding * 4 - logElement.widthPixels - timeElement.widthPixels;
			newsElement.SetFixedSize(fixedWidth, logElement.heightPixels);
			topBarWindow.Validate();
			int heightPixels = topBarWindow.heightPixels;
			groundLootWindow.rectTransform.anchoredPosition = new Vector2(num - groundLootWindow.widthPixels, -heightPixels);
			groundLootWindow.Validate();
			shipWindow.ResizePixels(defaultShipButtonWidth, groundLootWindow.heightPixels);
			shipWindow.rectTransform.anchoredPosition = new Vector2(num - groundLootWindow.widthPixels - shipWindow.widthPixels, -heightPixels);
			rightBarWindow.rectTransform.anchoredPosition = new Vector2(num - rightBarWindow.widthPixels, -(num2 - rightBarWindow.heightPixels));
			rightBarWindow.Validate();
			int widthPixels = rightBarWindow.widthPixels;
			int num3 = Math.Min(num2 - topBarWindow.heightPixels - groundLootWindow.heightPixels, 201);
			int height = num2 - topBarWindow.heightPixels - num3;
			playerPartyWindow.rectTransform.anchoredPosition = new Vector2(0f, -heightPixels);
			playerPartyWindow.ResizePixels((num - widthPixels) / 2, height);
			npcPartyWindow.rectTransform.anchoredPosition = new Vector2((num - widthPixels) / 2, -heightPixels);
			npcPartyWindow.ResizePixels((num - widthPixels) / 2, height);
			heightPixels += groundLootWindow.heightPixels;
			responseWindow.Validate();
			portraitElement.Validate();
			portraitWindow.Validate();
			rightBarWindowFill.ResizePixels(rightBarWindow.widthPixels, num3 - rightBarWindow.heightPixels);
			rightBarWindowFill.rectTransform.anchoredPosition = new Vector2(rightBarWindow.rectTransform.anchoredPosition.x, rightBarWindow.rectTransform.anchoredPosition.y + (float)rightBarWindowFill.heightPixels);
			portraitWindow.rectTransform.anchoredPosition = new Vector2(0f, -(num2 - num3));
			dialogueWindow.ResizePixels(num - widthPixels - responseWindow.widthPixels - portraitWindow.widthPixels, num3);
			dialogueWindow.rectTransform.anchoredPosition = new Vector2(portraitWindow.widthPixels, -(num2 - dialogueWindow.heightPixels));
			responseWindow.rectTransform.anchoredPosition = new Vector2(portraitWindow.widthPixels + dialogueWindow.widthPixels, -(num2 - dialogueWindow.heightPixels));
			selectionAbilityWindow.ResizePixels(140, num3);
			selectionAbilityWindow.rectTransform.anchoredPosition = new Vector2(0f, -(num2 - selectionAbilityWindow.heightPixels));
			selectionInventoryWindow.ResizePixels(num - widthPixels - selectionAbilityWindow.widthPixels, num3);
			selectionInventoryWindow.rectTransform.anchoredPosition = new Vector2(selectionAbilityWindow.widthPixels, -(num2 - selectionInventoryWindow.heightPixels));
		}
		canvasWidth = num;
		canvasHeight = num2;
		UpdateArrow();
	}

	public void Reset()
	{
		isActive = false;
		EndAllAnimations();
		currentDialogue = null;
		topBarWindow.Hide();
		playerPartyWindow.Hide();
		npcPartyWindow.Hide();
		groundLootWindow.Hide();
		shipWindow.Hide();
		selectionInventoryWindow.Hide();
		selectionAbilityWindow.Hide();
		portraitWindow.Hide();
		dialogueWindow.Hide();
		responseWindow.Hide();
		rightBarWindow.Hide();
		rightBarWindowFill.Hide();
		playerPartyWindow.Detach();
		npcPartyWindow.Detach();
		groundLootWindow.Detach();
		stealInventorySelectElement.UnStealItem();
		if (currentCharacter != null)
		{
			GameCharacterRaidInventory playerParty = GameMaster.current.raidManager.playerParty;
			if (playerParty == null || !playerParty.childItems.Contains(currentCharacter))
			{
				RestoreCharacterInventory();
			}
		}
		ItemDefaultActivateHandler.current.Disable();
		ItemMouseDoubleClickHandler.current.Disable();
		ItemMouseDragHandler.current.Disable();
		ItemMouseHandler.current.Disable();
		ItemQuickTransferHandler.current.Disable();
		ItemSelectHandler.current.Disable();
		ItemSelectHandler.current.auto = false;
		ItemTooltipHandler.current.Disable();
		ItemContextHandler.current.Disable();
		WindowsHandler.HideAllNonImportantMovable();
		UpdateArrow();
	}

	public void StartLoot(GameCharacterRaidInventory playerParty, GameGridInventory floorInventory, string roomName = "Loot", GameItem ship = null, GameCharacterRaidInventory npcParty = null)
	{
		Reset();
		playerPartyWindow.Attach(playerParty);
		playerPartyWindow.Validate();
		playerPartyWindow.Show();
		if (npcParty != null)
		{
			npcPartyWindow.Attach(npcParty);
			npcPartyWindow.Validate();
			npcPartyWindow.Show();
		}
		if (ship != null)
		{
			shipWindow.Validate();
			shipWindow.Show();
		}
		groundLootWindow.Attach(floorInventory);
		groundLootWindow.SetTitle(roomName);
		groundLootWindow.Validate();
		groundLootWindow.Show();
		topBarWindow.Show();
		selectionInventoryWindow.Show();
		selectionAbilityWindow.Show();
		rightBarWindow.Show();
		rightBarWindowFill.Show();
		buttonSearch.Enable();
		buttonNext.Enable();
		buttonFast.Enable();
		buttonUntil.Enable();
		ItemDefaultActivateHandler.current.Enable();
		ItemMouseDoubleClickHandler.current.Enable();
		ItemMouseDragHandler.current.Enable();
		ItemMouseHandler.current.Enable();
		ItemQuickTransferHandler.current.Enable();
		ItemSelectHandler.current.Enable();
		ItemSelectHandler.current.auto = false;
		ItemTooltipHandler.current.Enable();
		ItemContextHandler.current.Enable();
		if (currentCharacter != null)
		{
			StealCharacterInventory(currentCharacter);
		}
		isActive = true;
		Validate();
		UpdateArrow();
	}

	public void StartDialogue(GameCharacterRaidInventory playerParty, GameCharacterRaidInventory npcParty = null)
	{
		Reset();
		playerPartyWindow.Attach(playerParty);
		playerPartyWindow.Validate();
		playerPartyWindow.Show();
		if (npcParty != null)
		{
			npcPartyWindow.Attach(npcParty);
			npcPartyWindow.Validate();
			npcPartyWindow.Show();
		}
		topBarWindow.Show();
		portraitWindow.Show();
		dialogueWindow.Show();
		responseWindow.Show();
		rightBarWindow.Show();
		rightBarWindowFill.Show();
		buttonSearch.Disable();
		buttonNext.Disable();
		buttonFast.Disable();
		buttonUntil.Disable();
		ItemDefaultActivateHandler.current.Enable();
		ItemMouseHandler.current.Enable();
		ItemSelectHandler.current.Enable();
		ItemSelectHandler.current.auto = true;
		ItemTooltipHandler.current.Enable();
		isActive = true;
		Validate();
		UpdateArrow();
	}

	public void UpdateDialogue(Dialogue dialogue)
	{
		currentDialogue = dialogue;
		dialogueElement.ClearRichText();
		currentDialogueAnimationLength = 0f;
		UpdateDialogueAnimation();
		if (!isDialogueAnimationCoroutineStarted)
		{
			OverlayHandler.current.StartCoroutine(DialogueCoroutine());
			isDialogueAnimationCoroutineStarted = true;
		}
		portraitWindow.SetTitle(currentDialogue?.title ?? "Unknown");
		if ((object)currentDialogue?.portrait != null)
		{
			portraitElement.SetSprite(currentDialogue.portrait);
		}
		else
		{
			portraitElement.SetSprite("Portraits/Unknown", "Idle");
		}
		responseInventory.ExpelAll();
		currentDialogue?.choices.ForEach(delegate((GameItem, Dialogue, Action) t)
		{
			var (gameItem, dialogue2, _) = t;
			if (dialogue2 == null || !dialogue2.IsHidden())
			{
				if (GraphUtils.TryAcceptAll(responseInventory, gameItem) > 0)
				{
					Debug.LogWarning("Warning, failed to add a choice '" + gameItem.GetDisplayName() + "' into dialogue choices inventory.");
				}
				if (gameItem is GameItemElement gameItemElement)
				{
					if (dialogue2 != null && dialogue2.IsDisabled())
					{
						gameItemElement.ToggleBackground(enabled: true, 0.5f, 0.5f);
						gameItemElement.forceDisableUse = true;
						gameItemElement.forceDisableActivate = true;
					}
					else
					{
						gameItemElement.ToggleBackground(enabled: true);
						gameItemElement.forceDisableUse = false;
						gameItemElement.forceDisableActivate = false;
					}
				}
			}
		});
		dialogueNew = true;
		Validate();
	}

	private void UpdateDialogueAnimation()
	{
		dialogueElement.ClearRichText();
		if (currentDialogue != null)
		{
			dialogueElement.AppendRichText(currentDialogue.text, (int)currentDialogueAnimationLength);
			RichTextBuilder text = currentDialogue.text;
			if (text == null || !text.Build((int)currentDialogueAnimationLength).isTruncated)
			{
				currentDialogueAnimationLength = -1f;
			}
			dialogueNew = false;
		}
	}

	public void CompleteDialogueAnimation()
	{
		currentDialogueAnimationLength = -1f;
		UpdateDialogueAnimation();
	}

	public bool IsCompletedDialogueAnimation()
	{
		return currentDialogueAnimationLength < 0f;
	}

	private IEnumerator DialogueCoroutine(float speed = 60f, float fps = 30f)
	{
		if (fps < 0.01f)
		{
			fps = 0.01f;
		}
		if (fps > 240f)
		{
			fps = 240f;
		}
		while (true)
		{
			if (currentDialogueAnimationLength >= 0f)
			{
				currentDialogueAnimationLength += 1f / fps * speed;
				UpdateDialogueAnimation();
				dialogueElement.SetScroll(1f);
			}
			yield return new WaitForSeconds(1f / fps);
			dialogueNew = false;
		}
	}

	public void StartCombat(GameCharacterRaidInventory playerParty, GameCharacterRaidInventory npcParty)
	{
		Reset();
		playerPartyWindow.Attach(playerParty);
		playerPartyWindow.Validate();
		playerPartyWindow.Show();
		if (npcParty != null)
		{
			npcPartyWindow.Attach(npcParty);
			npcPartyWindow.Validate();
			npcPartyWindow.Show();
		}
		topBarWindow.Show();
		selectionInventoryWindow.Show();
		selectionAbilityWindow.Show();
		rightBarWindow.Show();
		rightBarWindowFill.Show();
		buttonSearch.Disable();
		buttonNext.Disable();
		buttonFast.Disable();
		buttonUntil.Disable();
		ItemMouseHandler.current.Enable();
		ItemSelectHandler.current.Enable();
		ItemSelectHandler.current.auto = true;
		ItemTooltipHandler.current.Enable();
		isActive = true;
		Validate();
		UpdateArrow();
	}

	public void EndAllAnimations()
	{
	}

	public void StealCharacterInventory(GameCharacterItem character)
	{
		if (character != null)
		{
			if (currentCharacter != null)
			{
				RestoreCharacterInventory();
			}
			stealInventorySelectElement.StealItem(character);
			currentCharacter = character;
			stealInventorySelectElement.Validate();
			selectionInventoryWindow.SetTitle("Inventory (" + character.name + ")");
			PixelElement pixelElement = GraphUtils.FindChildType<GameGridInventory>(character.examineWindow);
			if (pixelElement != null)
			{
				stealAbilitySelectElement.Attach(pixelElement);
				stealAbilitySelectElement.Validate();
			}
			selectionAbilityWindow.SetTitle(character.name);
			UpdateArrow();
		}
	}

	public void RestoreCharacterInventory()
	{
		RestoreCharacterInventory(currentCharacter);
	}

	public void RestoreCharacterInventory(GameCharacterItem character)
	{
		if (character != null && currentCharacter == character)
		{
			stealInventorySelectElement.UnStealItem(character);
			selectionInventoryWindow.SetTitle("Inventory (None)");
			PixelElement pixelElement = GraphUtils.FindChildType<GameGridInventory>(character.examineWindow);
			if (pixelElement != null)
			{
				stealAbilitySelectElement.Detach(pixelElement);
				stealAbilitySelectElement.Validate();
			}
			selectionAbilityWindow.SetTitle("Abilities");
			currentCharacter = null;
			UpdateArrow();
		}
	}

	private void UpdateArrow()
	{
		if (currentCharacter != null)
		{
			selectArrow.rectTransform.position = currentCharacter.rectTransform.position;
			selectArrow.rectTransform.anchoredPosition += new Vector2(36f, 20f);
			selectArrow.image.enabled = true;
		}
		else
		{
			selectArrow.image.enabled = false;
		}
	}
}

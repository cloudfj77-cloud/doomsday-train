using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemContextHandler : EmptyActionHandler
{
	public static ItemContextHandler current;

	[SerializeField]
	public KeyCode key;

	[SerializeField]
	public Vector2 contextTranslation;

	[SerializeField]
	public RenderHandler.BackgroundColor contextHighlight;

	private GameItem currentItem;

	private GameGridInventory currentInventory;

	private bool delayFrame;

	private HighlightManager highlightManager = new HighlightManager();

	private PixelWindow contextWindow;

	private PixelWindow sortWindow;

	private RenderHandler.ColorPalette textColor = RenderHandler.ColorPalette.LightGray;

	private RenderHandler.ColorPalette textDisabledColor = RenderHandler.ColorPalette.DarkGray;

	private RenderHandler.ColorPalette textSelectColor = RenderHandler.ColorPalette.VeryLightGray;

	private RenderHandler.ColorPalette textSelectDisabledColor = RenderHandler.ColorPalette.DarkRed;

	private RichTextElement buttonOpen;

	private RichTextElement buttonUse;

	private RichTextElement buttonEquip;

	private RichTextElement buttonActivate;

	private RichTextElement buttonConfigure;

	private GridPixelElement contextButtons;

	private RichTextElement buttonToggle;

	private RichTextElement buttonUnload;

	private RichTextElement buttonPin;

	private RichTextElement buttonSortBigStart;

	private RichTextElement buttonSortSmallStart;

	private RichTextElement buttonSortBigEnd;

	private RichTextElement buttonSortSmallEnd;

	private RichTextElement buttonSortUndo;

	private bool canOpen;

	private bool canUse;

	private bool canActivate;

	private bool canConfigure;

	private bool canToggle;

	private bool canUnload;

	private bool canPin;

	private bool canSort;

	private bool canUndoSort;

	public Canvas overlayCanvas;

	public override List<KeyCode> keyListeners => new KeyCode[3] { key, triggerKey, sortMenuKey }.ToList();

	public KeyCode sortMenuKey => KeyBindHelper.Get("sort_menu");

	public KeyCode triggerKey => KeyBindHelper.Get("secondary_click");

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	protected override bool OnEventInit()
	{
		contextWindow = new PixelWindow();
		contextWindow.forceRaycast = true;
		contextWindow.handler.SetParent(RenderHandler.current.overlayRoot);
		contextWindow.xPadding = 2;
		contextWindow.yPadding = 2;
		contextWindow.handler.gameObject.name = "ContextMenuNode";
		contextWindow.handler.gameObject.tag = "ContextMenuNode";
		GridPixelElement gridPixelElement = (contextButtons = new GridPixelElement(1, 8));
		buttonOpen = new RichTextElement();
		buttonUse = new RichTextElement();
		buttonEquip = new RichTextElement();
		buttonActivate = new RichTextElement();
		buttonConfigure = new RichTextElement();
		buttonToggle = new RichTextElement();
		buttonUnload = new RichTextElement();
		buttonPin = new RichTextElement();
		ResetButtonColor();
		buttonActivate.handler.name = "nodeButtonActivate";
		buttonOpen.handler.tag = "ContextMenuNode";
		buttonUse.handler.tag = "ContextMenuNode";
		buttonEquip.handler.tag = "ContextMenuNode";
		buttonActivate.handler.tag = "ContextMenuNode";
		buttonConfigure.handler.tag = "ContextMenuNode";
		buttonToggle.handler.tag = "ContextMenuNode";
		buttonUnload.handler.tag = "ContextMenuNode";
		buttonPin.handler.tag = "ContextMenuNode";
		buttonOpen.handler.gameObject.name = "nodeButtonOpen";
		buttonUse.handler.gameObject.name = "nodeButtonUse";
		buttonEquip.handler.gameObject.name = "nodeButtonEquip";
		buttonActivate.handler.gameObject.name = "nodeButtonActivate";
		buttonConfigure.handler.gameObject.name = "nodeButtonConfigure";
		buttonToggle.handler.gameObject.name = "nodeButtonToggle";
		buttonUnload.handler.gameObject.name = "nodeButtonUnload";
		buttonPin.handler.gameObject.name = "nodeButtonPin";
		gridPixelElement.Attach(buttonOpen);
		gridPixelElement.Attach(buttonUse);
		gridPixelElement.Attach(buttonEquip);
		gridPixelElement.Attach(buttonActivate);
		gridPixelElement.Attach(buttonConfigure);
		gridPixelElement.Attach(buttonToggle);
		gridPixelElement.Attach(buttonUnload);
		gridPixelElement.Attach(buttonPin);
		gridPixelElement.Validate();
		contextWindow.Attach(gridPixelElement);
		Transform[] componentsInChildren = contextWindow.handler.GetComponentsInChildren<Transform>(includeInactive: true);
		foreach (Transform transform in componentsInChildren)
		{
			if (transform != base.transform)
			{
				transform.gameObject.tag = "ContextMenuNode";
			}
		}
		contextWindow.Validate();
		sortWindow = new PixelWindow();
		sortWindow.forceRaycast = true;
		sortWindow.handler.SetParent(RenderHandler.current.overlayRoot);
		sortWindow.xPadding = 2;
		sortWindow.yPadding = 2;
		sortWindow.handler.gameObject.name = "SortContextMenuNode";
		sortWindow.handler.gameObject.tag = "ContextMenuNode";
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 5);
		buttonSortBigStart = new RichTextElement();
		buttonSortSmallStart = new RichTextElement();
		buttonSortBigEnd = new RichTextElement();
		buttonSortSmallEnd = new RichTextElement();
		buttonSortUndo = new RichTextElement();
		ResetSortButtonColor();
		buttonSortBigStart.handler.tag = "ContextMenuNode";
		buttonSortSmallStart.handler.tag = "ContextMenuNode";
		buttonSortBigEnd.handler.tag = "ContextMenuNode";
		buttonSortSmallEnd.handler.tag = "ContextMenuNode";
		buttonSortUndo.handler.tag = "ContextMenuNode";
		buttonSortBigStart.handler.gameObject.name = "nodeButtonSortBigStart";
		buttonSortSmallStart.handler.gameObject.name = "nodeButtonSortSmallStart";
		buttonSortBigEnd.handler.gameObject.name = "nodeButtonSortBigEnd";
		buttonSortSmallEnd.handler.gameObject.name = "nodeButtonSortSmallEnd";
		buttonSortUndo.handler.gameObject.name = "nodeButtonSortUndo";
		gridPixelElement2.Attach(buttonSortBigStart);
		gridPixelElement2.Attach(buttonSortSmallStart);
		gridPixelElement2.Attach(buttonSortBigEnd);
		gridPixelElement2.Attach(buttonSortSmallEnd);
		gridPixelElement2.Attach(buttonSortUndo);
		gridPixelElement2.Validate();
		sortWindow.Attach(gridPixelElement2);
		componentsInChildren = sortWindow.handler.GetComponentsInChildren<Transform>(includeInactive: true);
		foreach (Transform transform2 in componentsInChildren)
		{
			if (transform2 != base.transform)
			{
				transform2.gameObject.tag = "ContextMenuNode";
			}
		}
		sortWindow.Validate();
		return true;
	}

	private void ResetButtonColor()
	{
		SetButtonColor(buttonOpen);
		SetButtonColor(buttonUse);
		SetButtonColor(buttonEquip);
		SetButtonColor(buttonActivate);
		SetButtonColor(buttonConfigure);
		SetButtonColor(buttonToggle);
		SetButtonColor(buttonUnload);
		SetButtonColor(buttonPin);
		ResetSortButtonColor();
	}

	private void ResetSortButtonColor()
	{
		SetSortButtonColor(buttonSortBigStart);
		SetSortButtonColor(buttonSortSmallStart);
		SetSortButtonColor(buttonSortBigEnd);
		SetSortButtonColor(buttonSortSmallEnd);
		SetSortButtonColor(buttonSortUndo);
	}

	private void UpdateConditions()
	{
		if (currentItem != null)
		{
			GameItem gameItem = currentItem;
			canOpen = currentItem.contentWindow != null && !currentItem.contentWindow.isLocked && GeneralHelper.IsItemOwned(currentItem);
			canUnload = canOpen && !currentItem.IsTag("IS_UNLOAD_DISABLED_TAG");
			canUse = gameItem.MaySelectSlotItem() && gameItem.CanSelectSlotItem();
			canActivate = gameItem.MayActivateSlotItem() && gameItem.CanActivateSlotItem();
			bool flag = gameItem.MayConfigureSlotItem();
			canConfigure = flag && gameItem.CanConfigureSlotItem();
			buttonConfigure.handler.gameObject.SetActive(flag);
			canToggle = gameItem.MayToggleSlotItem() && gameItem.CanToggleSlotItem();
			canPin = ItemPinHelper.MayPin(currentItem);
		}
	}

	private void UpdateSelectButtonColor(RichTextElement selectedButton)
	{
		if (currentItem != null)
		{
			SetButtonColor(buttonOpen, selectedButton, canOpen);
			SetButtonColor(buttonUse, selectedButton, canUse);
			SetButtonColor(buttonEquip, selectedButton, canUse);
			SetButtonColor(buttonActivate, selectedButton, canActivate);
			SetButtonColor(buttonConfigure, selectedButton, canConfigure);
			SetButtonColor(buttonToggle, selectedButton, canToggle);
			SetButtonColor(buttonUnload, selectedButton, canUnload);
			SetButtonColor(buttonPin, selectedButton, canPin);
		}
	}

	private void UpdateSortSelectButtonColor(RichTextElement selectedButton)
	{
		if (currentInventory != null)
		{
			SetSortButtonColor(buttonSortBigStart, selectedButton, canSort);
			SetSortButtonColor(buttonSortSmallStart, selectedButton, canSort);
			SetSortButtonColor(buttonSortBigEnd, selectedButton, canSort);
			SetSortButtonColor(buttonSortSmallEnd, selectedButton, canSort);
			SetSortButtonColor(buttonSortUndo, selectedButton, canUndoSort);
		}
	}

	private void SetSortButtonColor(RichTextElement button, RichTextElement selectedButton = null, bool isEnabled = false)
	{
		if (button != null)
		{
			bool flag = button == selectedButton;
			RenderHandler.ColorPalette color = ((!isEnabled) ? (flag ? textSelectDisabledColor : textDisabledColor) : (flag ? textSelectColor : textColor));
			if (button == buttonSortBigStart)
			{
				button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_sort_big"), 10000, color);
			}
			else if (button == buttonSortSmallStart)
			{
				button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_sort_small"), 10000, color);
			}
			else if (button == buttonSortBigEnd)
			{
				button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_sort_big_end"), 10000, color);
			}
			else if (button == buttonSortSmallEnd)
			{
				button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_sort_small_end"), 10000, color);
			}
			else if (button == buttonSortUndo)
			{
				button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_sort_undo"), 10000, color);
			}
		}
	}

	private void EqualizeSortButtonWidths()
	{
		RichTextElement[] array = new RichTextElement[5] { buttonSortBigStart, buttonSortSmallStart, buttonSortBigEnd, buttonSortSmallEnd, buttonSortUndo };
		RichTextElement[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SetFixedSize(-1, -1);
		}
		int num = 0;
		array2 = array;
		foreach (RichTextElement richTextElement in array2)
		{
			if (richTextElement.widthPixels > num)
			{
				num = richTextElement.widthPixels;
			}
		}
		array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SetFixedSize(num, -1);
		}
	}

	private void EqualizeButtonWidths()
	{
		List<RichTextElement> list = new List<RichTextElement> { buttonOpen, buttonUse, buttonEquip, buttonActivate, buttonToggle, buttonUnload, buttonPin };
		if (buttonConfigure.handler.gameObject.activeSelf)
		{
			list.Add(buttonConfigure);
		}
		foreach (RichTextElement item in list)
		{
			item.SetFixedSize(-1, -1);
		}
		int num = 0;
		foreach (RichTextElement item2 in list)
		{
			if (item2.widthPixels > num)
			{
				num = item2.widthPixels;
			}
		}
		foreach (RichTextElement item3 in list)
		{
			item3.SetFixedSize(num, -1);
		}
	}

	private void SetButtonColor(RichTextElement button, RichTextElement selectedButton = null, bool isEnabled = false)
	{
		if (button == null)
		{
			return;
		}
		bool flag = button == selectedButton;
		RenderHandler.ColorPalette color = ((!isEnabled) ? (flag ? textSelectDisabledColor : textDisabledColor) : (flag ? textSelectColor : textColor));
		if (button == buttonOpen)
		{
			button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_open"), 10000, color);
		}
		else if (button == buttonUse)
		{
			button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_use"), 10000, color);
		}
		else if (button == buttonEquip)
		{
			button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_equip"), 10000, color);
		}
		else if (button == buttonActivate)
		{
			if (currentItem != null)
			{
				if (AppraisalRankHelper.IsCertificate(currentItem))
				{
					button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_activate"), 10000, color);
				}
				else if (currentItem.IsTag("DOCUMENT"))
				{
					button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_read"), 10000, color);
				}
				else if (currentItem.IsTag("NARCOTIC") || currentItem.IsTag("SUBSTANCE") || currentItem.IsTag("ALCOHOL") || currentItem.IsTag("MEDICAL") || currentItem.IsTag("LIQUID_LUCK_TAG"))
				{
					button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_consume"), 10000, color);
				}
				else
				{
					button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_activate"), 10000, color);
				}
			}
		}
		else if (button == buttonConfigure)
		{
			button.SetText(LocHelper.GetLocalizedMechanicOrDefault("handler_btn_configure", "Configure"), 10000, color);
		}
		else if (button == buttonToggle)
		{
			if (currentItem == null)
			{
				return;
			}
			if (currentItem.identifier.Equals("ammo_box"))
			{
				if (currentItem.IsTag("AMMO_BOX_PACKAGED_TAG"))
				{
					button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_unpack"), 10000, color);
				}
				else
				{
					button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_pack"), 10000, color);
				}
			}
			else
			{
				button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_toggle"), 10000, color);
			}
		}
		else if (button == buttonUnload)
		{
			button.SetText(LocHelper.GetLocalizedMechanic("handler_btn_unload"), 10000, color);
		}
		else if (button == buttonPin)
		{
			string text = (ItemPinHelper.IsPinned(currentItem) ? "handler_btn_unpin" : "handler_btn_pin");
			button.SetText(LocHelper.GetLocalizedMechanic(text), 10000, color);
		}
	}

	private void TriggerButton(RichTextElement button)
	{
		if (currentInventory != null)
		{
			GameGridInventory inventory = currentInventory;
			if (canSort && (button == buttonSortBigStart || button == buttonSortSmallStart || button == buttonSortBigEnd || button == buttonSortSmallEnd))
			{
				bool bigFirst = button == buttonSortBigStart || button == buttonSortBigEnd;
				bool fromEnd = button == buttonSortBigEnd || button == buttonSortSmallEnd;
				OnEventReset();
				InventorySortHelper.Sort(inventory, bigFirst, fromEnd);
				if (TutorialUIManager.Instance != null)
				{
					TutorialUIManager.Instance.OnInventorySorted();
				}
			}
			else if (canUndoSort && button == buttonSortUndo)
			{
				OnEventReset();
				InventorySortHelper.Undo(inventory);
			}
		}
		else if (currentItem != null)
		{
			GameItem gameItem = currentItem;
			GameItem gameItem2 = currentItem;
			Vector2 mousePosition = Input.mousePosition;
			if (button == buttonOpen && canOpen)
			{
				OnEventReset();
				ItemMouseDoubleClickHandler.current.OpenContentAction(gameItem, mousePosition);
			}
			else if (button == buttonUse && canUse)
			{
				OnEventReset();
				ItemSelectHandler.current.TrySelectItem(gameItem);
			}
			else if (button == buttonEquip && canUse)
			{
				OnEventReset();
				ItemSelectHandler.current.TryEquipItem(gameItem);
			}
			else if (button == buttonActivate && canActivate)
			{
				OnEventReset();
				gameItem2.ActivateSlotItem();
			}
			else if (button == buttonConfigure && canConfigure)
			{
				OnEventReset();
				gameItem2.ConfigureSlotItem();
			}
			else if (button == buttonToggle && canToggle)
			{
				OnEventReset();
				gameItem2.ToggleSlotItem();
			}
			else if (button == buttonUnload && canUnload)
			{
				OnEventReset();
				gameItem2.TryUnloadAll(skipPinned: true);
			}
			else if (button == buttonPin && canPin)
			{
				OnEventReset();
				ItemPinHelper.TogglePinned(gameItem);
			}
		}
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		Vector2 vector = Input.mousePosition;
		if (keysPressed.Contains(triggerKey))
		{
			if (ItemSelectHandler.current.IsEquipped || ItemPinHelper.BlocksClicks || ItemMouseDragHandler.current.IsDraggingItem)
			{
				return;
			}
			GameItem gameItem = RenderHandler.RaycastElement<GameItem>(vector);
			if (!GeneralHelper.IsDisplayOnly(gameItem))
			{
				if (gameItem != null)
				{
					InputActionManager.current.ResetAfter(this);
					sortWindow.Hide();
					currentInventory = null;
					currentItem = gameItem;
					GameItem gameItem2 = currentItem;
					UpdateConditions();
					UpdateSelectButtonColor(null);
					EqualizeButtonWidths();
					contextButtons.Validate();
					contextWindow.Validate();
					contextWindow.Show(vector, contextTranslation);
					delayFrame = true;
					SlotMarker incompleteSlotInInventory = gameItem2.GetIncompleteSlotInInventory();
					highlightManager.Try(incompleteSlotInInventory, contextHighlight);
				}
				else if (!EmporiumOption.SortMenuDisabled)
				{
					TryOpenSortMenu(vector);
				}
			}
		}
		else if (keysPressed.Contains(sortMenuKey) && !ItemPinHelper.BlocksClicks && RenderHandler.RaycastElement<GameItem>(vector) == null)
		{
			TryOpenSortMenu(vector);
		}
	}

	private bool TryOpenSortMenu(Vector2 mousePosition)
	{
		GameGridInventory gameGridInventory = RenderHandler.RaycastElement<GameGridInventory>(mousePosition);
		if (gameGridInventory == null || !InventorySortHelper.MaySort(gameGridInventory))
		{
			return false;
		}
		InputActionManager.current.ResetAfter(this);
		contextWindow.Hide();
		currentItem = null;
		currentInventory = gameGridInventory;
		canSort = InventorySortHelper.CanSort(currentInventory);
		canUndoSort = InventorySortHelper.CanUndo(currentInventory);
		UpdateSortSelectButtonColor(null);
		EqualizeSortButtonWidths();
		sortWindow.Validate();
		sortWindow.Show(mousePosition, contextTranslation);
		delayFrame = true;
		SlotMarker slotMarker = new SlotMarker();
		slotMarker.inventory = currentInventory;
		slotMarker.itemGridShape = new GridShapeBuilder(currentInventory.inventoryShape.width, currentInventory.inventoryShape.height).SetDataFill(1).Build();
		highlightManager.Try(slotMarker, contextHighlight);
		return true;
	}

	protected override void OnEventRelease(ISet<KeyCode> keysReleased)
	{
		Vector2 eventDataPosition = Input.mousePosition;
		if (keysReleased.Contains(key))
		{
			RichTextElement richTextElement = FindRichTextNode(eventDataPosition, overlayCanvas);
			if (richTextElement != null)
			{
				TriggerButton(richTextElement);
			}
		}
	}

	protected override void OnEventMouseMove(Vector3 mousePosition, Vector3 mouseDelta)
	{
	}

	protected override bool OnEventUpdate()
	{
		if (currentItem == null && currentInventory == null)
		{
			return false;
		}
		Vector2 vector = Input.mousePosition;
		RichTextElement richTextElement = null;
		richTextElement = FindRichTextNode(vector, overlayCanvas);
		UpdateSelectButtonColor(richTextElement);
		UpdateSortSelectButtonColor(richTextElement);
		RenderHandler.RaycastFindParentElement<PixelWindow>(vector);
		if (FindContextMenuNode(vector, overlayCanvas) == null && !delayFrame)
		{
			OnEventReset();
			return false;
		}
		delayFrame = false;
		return true;
	}

	public GameObject FindContextMenuNode(Vector2 eventDataPosition, Canvas canvas)
	{
		GraphicRaycaster component = canvas.GetComponent<GraphicRaycaster>();
		if (component == null)
		{
			Debug.LogError("No GraphicRaycaster found on the canvas.");
			return null;
		}
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = eventDataPosition
		};
		List<RaycastResult> list = new List<RaycastResult>();
		component.Raycast(eventData, list);
		foreach (RaycastResult item in list)
		{
			if (item.gameObject.CompareTag("ContextMenuNode"))
			{
				return item.gameObject;
			}
		}
		return null;
	}

	public RichTextElement FindRichTextNode(Vector2 eventDataPosition, Canvas canvas)
	{
		GraphicRaycaster component = canvas.GetComponent<GraphicRaycaster>();
		if (component == null)
		{
			Debug.LogError("No GraphicRaycaster found on the canvas.");
			return null;
		}
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = eventDataPosition
		};
		List<RaycastResult> list = new List<RaycastResult>();
		component.Raycast(eventData, list);
		foreach (RaycastResult item in list)
		{
			TreeNodeRender component2 = item.gameObject.GetComponent<TreeNodeRender>();
			if (component2.parent.element is RichTextElement)
			{
				return (RichTextElement)component2.parent.element;
			}
		}
		return null;
	}

	protected override void OnEventReset()
	{
		highlightManager.Clear();
		contextWindow.Hide();
		sortWindow.Hide();
		ResetButtonColor();
		currentItem = null;
		currentInventory = null;
		delayFrame = false;
	}

	public void Stop(bool nonOwnedOnly = false)
	{
		if (nonOwnedOnly)
		{
			if ((currentItem != null && GeneralHelper.IsItemOwned(currentItem)) || currentInventory != null)
			{
				OnEventReset();
			}
		}
		else
		{
			OnEventReset();
		}
	}
}

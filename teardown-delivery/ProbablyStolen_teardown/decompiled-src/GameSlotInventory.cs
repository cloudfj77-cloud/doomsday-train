using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSlotInventory : GameInventory, PixelElement, TooltipListener
{
	public bool stealUI;

	public bool isUIDown;

	private PixelElement stolenUI;

	public readonly int defaultOrientation;

	private readonly TreeNodeRender background;

	private readonly TreeNodeRender insideBorder;

	private readonly TreeNodeRender stolenInner;

	private TreeNodeRender backgroundImage;

	private GameItem currentItem;

	private readonly TreeNodeRender highlight;

	private readonly int defaultGridWidth;

	private readonly int defaultGridHeight;

	private string tooltipName;

	private TreeNodeRender cellNode;

	private bool validateLock;

	public override List<GraphNodeStorage> children
	{
		get
		{
			List<GraphNodeStorage> list = new List<GraphNodeStorage>();
			if (currentItem != null)
			{
				list.Add(currentItem);
			}
			if (stolenUI is GraphNodeStorage item)
			{
				list.Add(item);
			}
			return list;
		}
	}

	public override List<GameItem> childItems
	{
		get
		{
			List<GameItem> list = new List<GameItem>();
			if (currentItem != null)
			{
				list.Add(currentItem);
			}
			return list;
		}
	}

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	public GameItem childItem => currentItem;

	public float transparency { get; private set; }

	public bool isMachineryStyle => cellNode != null;

	public bool isHidden { get; private set; }

	public GameSlotInventory()
		: this(1, 1)
	{
	}

	public GameSlotInventory(int defaultGridWidth, int defaultGridHeight, bool stealUI = false, bool isUIDown = false)
	{
		this.defaultGridWidth = defaultGridWidth;
		this.defaultGridHeight = defaultGridHeight;
		this.stealUI = stealUI;
		this.isUIDown = isUIDown;
		handler = TreeNodeRender.Instantiate(this);
		background = TreeNodeRender.Instantiate().SetParent(handler);
		inner = TreeNodeRender.Instantiate().SetParent(handler);
		insideBorder = TreeNodeRender.Instantiate().SetParent(handler);
		stolenInner = TreeNodeRender.Instantiate().SetParent(handler);
		highlight = TreeNodeRender.Instantiate().SetParent(background);
		RenderHandler.current.SetHighlightColor(background.image, RenderHandler.BackgroundColor.Tan, RenderHandler.BackgroundType.Smooth);
		background.image.SetNativeSize();
		background.InitEventHandler();
		background.eventHandler.mouseDownFunc.Add(delegate
		{
			background.FindParentWindow()?.ToFront();
			return true;
		});
		insideBorder.image.sprite = RenderHandler.current.inside;
		insideBorder.image.SetNativeSize();
		insideBorder.image.raycastTarget = false;
		insideBorder.image.fillCenter = false;
		background.image.enabled = true;
		insideBorder.image.enabled = true;
		Validate();
	}

	public GameSlotInventory(string backgroundAtlas, string backgroundSprite, bool stealUI = false, bool isUIDown = false)
		: this(1, 1, stealUI, isUIDown)
	{
		backgroundImage = TreeNodeRender.Instantiate().SetParent(highlight);
		backgroundImage.image.sprite = RenderHandler.LoadFromAtlas(backgroundAtlas, backgroundSprite);
		backgroundImage.image.SetNativeSize();
		backgroundImage.image.enabled = true;
		Validate();
	}

	public GameSlotInventory SetTransparency(float transparency)
	{
		this.transparency = transparency;
		Validate();
		return this;
	}

	public GameSlotInventory SetMachineryStyle()
	{
		if (cellNode != null)
		{
			return this;
		}
		cellNode = TreeNodeRender.Instantiate().SetParent(background);
		cellNode.rectTransform.SetAsFirstSibling();
		MachineryStyle.SetFlat(cellNode, MachineryStyle.CELL_COLOR);
		cellNode.image.raycastTarget = false;
		background.image.color = new Color(1f, 1f, 1f, 0f);
		insideBorder.gameObject.SetActive(value: false);
		Validate();
		return this;
	}

	public GameSlotInventory SetBackgroundFadeSprite(string backgroundAtlas, string backgroundSprite, float opacity = 0.5f, float lightness = 0.5f)
	{
		return SetBackgroundSprite(backgroundAtlas, backgroundSprite, new Color(lightness, lightness, lightness, opacity));
	}

	public GameSlotInventory SetBackgroundSprite(string backgroundAtlas, string backgroundSprite, float opacity = 1f, float lightness = 1f)
	{
		return SetBackgroundSprite(backgroundAtlas, backgroundSprite, new Color(lightness, lightness, lightness, opacity));
	}

	public GameSlotInventory SetBackgroundSprite(string backgroundAtlas, string backgroundSprite, Color color)
	{
		if ((object)backgroundImage == null)
		{
			backgroundImage = TreeNodeRender.Instantiate().SetParent(highlight);
		}
		backgroundImage.image.sprite = RenderHandler.LoadFromAtlas(backgroundAtlas, backgroundSprite);
		backgroundImage.image.color = color;
		backgroundImage.image.SetNativeSize();
		backgroundImage.image.enabled = true;
		Validate();
		return this;
	}

	public GameSlotInventory ClearBackgroundSprite()
	{
		if ((object)backgroundImage != null)
		{
			backgroundImage.image.enabled = false;
		}
		Validate();
		return this;
	}

	public int WidthPixelsClamp(int width)
	{
		return widthPixels;
	}

	public int HeightPixelsClamp(int height)
	{
		return heightPixels;
	}

	public void ResizePixels(int width, int height)
	{
	}

	public int XClamp(int x)
	{
		return 0;
	}

	public int YClamp(int y)
	{
		return 0;
	}

	public void Hide()
	{
		isHidden = true;
		Validate();
	}

	public void Show()
	{
		isHidden = false;
		Validate();
	}

	public override void Validate()
	{
		if (validateLock)
		{
			return;
		}
		validateLock = true;
		insideBorder.image.color = new Color(1f, 1f, 1f, 1f - transparency);
		background.image.color = new Color(1f, 1f, 1f, 1f - transparency);
		if (cellNode != null)
		{
			background.image.color = new Color(1f, 1f, 1f, 0f);
		}
		if (currentItem?.modifiedShape is GridShapeBuilder gridShapeBuilder)
		{
			gridShapeBuilder.SetPosition(0, 0);
		}
		int gridSize = UISettings.current.gridSize;
		widthPixels = defaultGridWidth * gridSize;
		heightPixels = defaultGridHeight * gridSize;
		if ((object)backgroundImage != null)
		{
			backgroundImage.image.enabled = true;
			widthPixels = (int)backgroundImage.rectTransform.sizeDelta.x;
			heightPixels = (int)backgroundImage.rectTransform.sizeDelta.y;
		}
		if (currentItem != null)
		{
			widthPixels = currentItem.modifiedShape.globalWidth * gridSize;
			heightPixels = currentItem.modifiedShape.globalHeight * gridSize;
			if ((object)backgroundImage != null)
			{
				backgroundImage.image.enabled = false;
			}
		}
		background.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		highlight.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		insideBorder.rectTransform.sizeDelta = new Vector2(widthPixels + 2, heightPixels + 2);
		insideBorder.rectTransform.anchoredPosition = new Vector2(-1f, 1f);
		if (cellNode != null)
		{
			MachineryStyle.Place(cellNode, 1, 1, Mathf.Max(0, widthPixels - 2), Mathf.Max(0, heightPixels - 2));
		}
		if (stolenUI != null)
		{
			int num = handler.FindParentWindow()?.xPadding ?? 0;
			int num2 = handler.FindParentWindow()?.yPadding ?? 0;
			stolenInner.rectTransform.anchoredPosition = (isUIDown ? new Vector2(0f, -heightPixels - num2) : new Vector2(widthPixels + num, 0f));
			widthPixels = (isUIDown ? Math.Max(widthPixels, stolenUI.widthPixels) : (widthPixels + stolenUI.widthPixels + num));
			heightPixels = (isUIDown ? (heightPixels + stolenUI.heightPixels + num2) : Math.Max(heightPixels, stolenUI.heightPixels));
		}
		if (isHidden)
		{
			widthPixels = 0;
			heightPixels = 0;
		}
		handler.gameObject.SetActive(!isHidden);
		currentItem?.Validate();
		handler.parent?.FindParentElement<GraphNodeStorage>()?.Validate();
		validateLock = false;
	}

	public bool Attach(PixelElement element)
	{
		throw new NotImplementedException();
	}

	public bool Detach(PixelElement element)
	{
		throw new NotImplementedException();
	}

	public override bool AddParent(GraphNodeStorage node)
	{
		if (base.parent != null)
		{
			return false;
		}
		base.parent = node;
		return true;
	}

	public override bool RemoveParent(GraphNodeStorage node)
	{
		if (base.parent != node)
		{
			return false;
		}
		base.parent = null;
		return true;
	}

	public override bool IsAdjacent(SlotMarker slot, SlotMarker slot2)
	{
		return false;
	}

	public override List<TreeNodeRender> HighlightSlot(SlotMarker slot, RenderHandler.BackgroundColor color, bool doReplace)
	{
		List<TreeNodeRender> list = new List<TreeNodeRender>();
		if (slot == null)
		{
			return list;
		}
		if (slot.inventory != this)
		{
			return list;
		}
		if (!highlight.image.enabled || doReplace)
		{
			RenderHandler.current.SetHighlightColor(highlight.image, color, RenderHandler.BackgroundType.Border);
			list.Add(highlight);
		}
		return list;
	}

	public override void ClearHighlight()
	{
		RenderHandler.current.SetHighlightColor(highlight.image, RenderHandler.BackgroundColor.None, RenderHandler.BackgroundType.Border);
	}

	public override List<TreeNodeRender> HighlightWholeInventory(RenderHandler.BackgroundColor color)
	{
		SlotMarker slotMarker = new SlotMarker();
		slotMarker.inventory = this;
		return HighlightSlot(slotMarker, color, doReplace: true);
	}

	public override bool MayHaveValidInventorySlot(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		if (IsInsertLocked())
		{
			return false;
		}
		if (GraphUtils.FindParentEquals(this, item))
		{
			return false;
		}
		if (!GeneralHelper.MayOwnershipAcceptItem(item, this))
		{
			return false;
		}
		Func<GameItem, GameInventory, bool> func = item.mayInventoryAddItemFunc;
		if (func != null && !func.InvokeAllReduce(item, this, InvokeFuncExtensions.BoolReduceOR))
		{
			return false;
		}
		Func<GameItem, GameInventory, bool> func2 = base.mayInventoryAddItemFunc;
		if (func2 != null && !func2.InvokeAllReduce(item, this, InvokeFuncExtensions.BoolReduceOR))
		{
			return false;
		}
		return true;
	}

	private Vector2 GetLocalPosition(Vector2 position)
	{
		return rectTransform.InverseTransformPoint(position);
	}

	public override bool CheckInventorySlotStillValid(GameItem item, GridShape tempShape, TagSystem tempState)
	{
		SlotMarker slotMarker = new SlotMarker();
		slotMarker.item = item;
		slotMarker.inventory = this;
		if (item == null)
		{
			return false;
		}
		if (tempShape == null)
		{
			tempShape = item.modifiedShape;
		}
		if (tempState == null)
		{
			tempState = item.modifiedState;
		}
		slotMarker.itemGridShape = tempShape;
		if (!MayHaveValidInventorySlot(item))
		{
			return false;
		}
		if (currentItem != null && currentItem != item)
		{
			return false;
		}
		slotMarker.itemGridShape = new GridShapeBuilder(tempShape).SetTransform(0, 0, flipped: false, defaultOrientation);
		if (!item.CheckIsStateValid(this, slotMarker, tempShape, tempState))
		{
			return false;
		}
		if (!CheckChangeParentShapeStateModifier(item, slotMarker, tempShape, tempState))
		{
			return false;
		}
		if (item.MaxNumAdd(slotMarker) < 1)
		{
			return false;
		}
		return true;
	}

	public override SlotMarker TryInventorySlot(GameItem item, int requestedNum, Vector2 itemGraphicsCenterPosition, GridShape tempShape, TagSystem tempState)
	{
		SlotMarker slotMarker = new SlotMarker();
		slotMarker.item = item;
		slotMarker.inventory = this;
		slotMarker.itemGridShape = tempShape;
		_ = UISettings.current.gridSize;
		if (item == null)
		{
			return slotMarker;
		}
		if (tempShape == null)
		{
			tempShape = item.modifiedShape;
		}
		if (tempState == null)
		{
			tempState = item.modifiedState;
		}
		if (currentItem != null && currentItem != item)
		{
			return slotMarker;
		}
		if (!MayHaveValidInventorySlot(item))
		{
			return slotMarker;
		}
		if (float.IsFinite(itemGraphicsCenterPosition.x) && !background.rectTransform.rect.Contains(GetLocalPosition(itemGraphicsCenterPosition)))
		{
			return slotMarker;
		}
		slotMarker.itemGridShape = new GridShapeBuilder(tempShape).SetTransform(0, 0, flipped: false, defaultOrientation);
		if (!item.CheckIsStateValid(this, slotMarker, tempShape, tempState))
		{
			return slotMarker;
		}
		if (!CheckAcceptParentShapeStateModifier(item, slotMarker, tempShape, tempState))
		{
			return slotMarker;
		}
		int val = item.MaxNumRemove();
		int val2 = item.MaxNumAdd(slotMarker);
		slotMarker.numTransfer = Math.Min(Math.Min(val, val2), requestedNum);
		if (slotMarker.numTransfer < 1)
		{
			return slotMarker;
		}
		slotMarker.priority = Math.Max(base.slotPriorityFunc?.Invoke(item, this, slotMarker) ?? 0, item.slotPriorityFunc?.Invoke(item, this, slotMarker) ?? 0);
		return slotMarker;
	}

	public override SlotMarker GetIncompleteItemInventorySlot(GameItem item)
	{
		return new SlotMarker
		{
			item = item,
			inventory = this,
			itemGridShape = item.modifiedShape,
			index = childItems.IndexOf(item)
		};
	}

	public override SlotMarker GetItemInventorySlot(GameItem item)
	{
		SlotMarker slotMarker = TryInventorySlot(item, item.unitCount, item.modifiedShape);
		slotMarker.index = childItems.IndexOf(item);
		return slotMarker;
	}

	public override bool UncheckedAccept(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		if (item.parentInventory != null)
		{
			return false;
		}
		if (currentItem == item)
		{
			return false;
		}
		base.onSlotAddItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		currentItem = item;
		item.AddParent(this);
		if (item is PixelElement pixelElement)
		{
			pixelElement.SetRenderingParent(this);
		}
		item.onSlotAddItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		base.onSlotAddItemEndFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		base.onSlotAddItemLateFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		if (item.IsRootedInBackInv())
		{
			PlayerStore.Instance.OnWeightedAreaChanged();
		}
		if (stealUI && item.child is PixelWindow pixelWindow)
		{
			stolenUI = pixelWindow.childElement;
			stolenUI?.SetRenderingParent(this);
			stolenUI?.handler?.rectTransform?.SetParent(stolenInner.rectTransform, worldPositionStays: false);
			pixelWindow.Lock();
		}
		item.ValidateShapeState();
		return true;
	}

	public override bool Expel(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		if (item.parentInventory != this)
		{
			return false;
		}
		if (currentItem != item)
		{
			return false;
		}
		item.onSlotRemoveItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		base.onSlotRemoveItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		if (stolenUI != null && item.child is PixelWindow pixelWindow)
		{
			stolenUI.SetRenderingParent(pixelWindow);
			stolenUI = null;
			pixelWindow.Unlock();
		}
		currentItem = null;
		item.RemoveParent(this);
		if (item is PixelElement pixelElement)
		{
			pixelElement.RemoveRenderingParent(this);
		}
		base.onSlotRemoveItemEndFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		base.onSlotRemoveItemLateFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		Validate();
		GameItem gameItem = GraphUtils.FindParentType<GameItem>(this);
		gameItem?.ValidateShapeState();
		gameItem?.Validate();
		OverlayHandler.current.UpdateInventoryStateUI();
		return true;
	}

	public override SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		int num = item.MaxNumRemove();
		if (num < 1)
		{
			return null;
		}
		SlotMarker slotMarker = new SlotMarker();
		slotMarker.item = item;
		slotMarker.inventory = this;
		slotMarker.itemGridShape = item.modifiedShape;
		if (item != currentItem && currentItem != null)
		{
			Func<GameItem, GameInventory, SlotMarker, GameItem, bool> func = currentItem.slotSearchPassthroughForItemFunc;
			if (func == null || !func(item, this, slotMarker, currentItem))
			{
				Func<GameItem, GameInventory, SlotMarker, GameItem, bool> func2 = base.slotSearchPassthroughForItemFunc;
				if (func2 == null || !func2(item, this, slotMarker, currentItem))
				{
					goto IL_008f;
				}
			}
			SlotMarker slotMarker2 = currentItem.TryFindOneValidInventorySlot(item);
			if (slotMarker2 != null)
			{
				return slotMarker2;
			}
		}
		goto IL_008f;
		IL_008f:
		if (currentItem != null && item != currentItem)
		{
			return null;
		}
		if (!MayHaveValidInventorySlot(item))
		{
			return null;
		}
		if (item != currentItem)
		{
			GameItem gameItem = currentItem;
			if (gameItem != null && gameItem.MayStack(item))
			{
				slotMarker.targetItem = currentItem;
				slotMarker.numTransfer = Math.Min(num, currentItem.MaxNumAdd());
				slotMarker.priority = Math.Max(base.slotPriorityFunc?.Invoke(item, this, slotMarker) ?? 0, item.slotPriorityFunc?.Invoke(item, this, slotMarker) ?? 0);
				if (slotMarker.numTransfer > 0)
				{
					return slotMarker;
				}
			}
		}
		slotMarker = TryInventorySlot(item, num, item.modifiedShape.Clone());
		if (slotMarker.numTransfer > 0)
		{
			return slotMarker;
		}
		return null;
	}

	public GameSlotInventory SetTooltipName(string name)
	{
		tooltipName = name;
		return this;
	}

	public bool HasTooltip()
	{
		if (childItem != null)
		{
			return false;
		}
		return tooltipName != null;
	}

	public RichText GetTooltip()
	{
		RichTextBuilder richTextBuilder = new RichTextBuilder(40);
		if (tooltipName != null)
		{
			richTextBuilder.AddLine(tooltipName, ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
		}
		return richTextBuilder.Build();
	}

	public RichText GetTooltipAdvanced()
	{
		return GetTooltip();
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameGridScrollableInventory : GameInventory, PixelElement
{
	private readonly TreeNodeRender background;

	private readonly TreeNodeRender insideBorder;

	private readonly TreeNodeRender highlightNode;

	private readonly TreeNodeRender innerMask;

	private readonly List<GameItem> items = new List<GameItem>();

	private readonly List<TreeNodeRender> highlightSquares = new List<TreeNodeRender>();

	private int leftPadding = 2;

	private int rightPadding = 3;

	private int topPadding = 3;

	public int scrollPadding = 3;

	private int minButtonHeight = 6;

	private int minButtonWidth = 6;

	private readonly TreeNodeRender scrollBar;

	private readonly TreeNodeRender scrollButton;

	private float scrollOverflow;

	private float savedBarRelativePosition;

	private float savedBackgroundRelativePosition;

	public bool alwaysStack;

	public override List<GraphNodeStorage> children => new List<GraphNodeStorage>(items);

	public override List<GameItem> childItems => new List<GameItem>(items);

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	public int width { get; private set; }

	public int height { get; private set; }

	private int currentScroll { get; set; }

	public override bool SupportsGroupPlacement => !alwaysStack;

	private GameGridScrollableInventory()
	{
		handler = TreeNodeRender.Instantiate(this);
		background = TreeNodeRender.Instantiate().SetParent(handler);
		innerMask = TreeNodeRender.Instantiate().SetParent(handler);
		innerMask.mask.enabled = true;
		inner = TreeNodeRender.Instantiate().SetParent(innerMask);
		highlightNode = TreeNodeRender.Instantiate().SetParent(inner);
		insideBorder = TreeNodeRender.Instantiate().SetParent(handler);
		RenderHandler.current.SetHighlightColor(background.image, RenderHandler.BackgroundColor.Black, RenderHandler.BackgroundType.Grid);
		background.image.SetNativeSize();
		background.InitEventHandler();
		background.eventHandler.mouseDownFunc.Add(delegate
		{
			background.FindParentWindow()?.ToFront();
			return true;
		});
		background.eventHandler.mouseScrollFunc.Add(delegate(PointerEventData r)
		{
			Scroll(r.scrollDelta);
			return false;
		});
		insideBorder.image.sprite = RenderHandler.current.inside;
		insideBorder.image.SetNativeSize();
		insideBorder.image.raycastTarget = false;
		insideBorder.image.fillCenter = false;
		background.image.enabled = true;
		insideBorder.image.enabled = true;
		scrollBar = TreeNodeRender.Instantiate().SetParent(handler);
		scrollButton = TreeNodeRender.Instantiate().SetParent(handler);
		scrollBar.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_right_scrollbar");
		scrollBar.image.SetNativeSize();
		scrollButton.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_right_scrollbar_button");
		scrollButton.image.SetNativeSize();
		scrollBar.image.enabled = true;
		scrollButton.image.enabled = true;
		scrollBar.InitEventHandler();
		scrollButton.InitEventHandler();
		scrollButton.eventHandler.mouseDownFunc.Add(delegate(PointerEventData r)
		{
			savedBarRelativePosition = handler.rectTransform.InverseTransformPoint(r.position).y - scrollButton.rectTransform.anchoredPosition.y;
			return false;
		});
		scrollButton.eventHandler.mouseDragFunc.Add(delegate(PointerEventData r)
		{
			PlaceButtonAtMousePositionRelative(r.position, savedBarRelativePosition);
			SetScroll(GetButtonScroll());
			return false;
		});
		scrollBar.eventHandler.mouseDownFunc.Add(delegate(PointerEventData r)
		{
			PlaceButtonAtMousePositionCenter(r.position);
			SetScroll(GetButtonScroll());
			return false;
		});
		scrollBar.eventHandler.mouseDragFunc.Add(delegate(PointerEventData r)
		{
			PlaceButtonAtMousePositionCenter(r.position);
			SetScroll(GetButtonScroll());
			return false;
		});
	}

	public GameGridScrollableInventory(int width, int displayHeight, int height = 400, float scrollOverflow = 0.5f)
		: this()
	{
		this.width = width;
		if (height <= 0)
		{
			height = 1;
		}
		this.height = height;
		heightPixels = displayHeight * UISettings.current.gridSize;
		this.scrollOverflow = scrollOverflow;
		Validate();
	}

	public GameGridScrollableInventory SetShape(int width, int displayHeight, int height = 400)
	{
		this.width = width;
		if (height <= 0)
		{
			height = 1;
		}
		this.height = height;
		heightPixels = displayHeight * UISettings.current.gridSize;
		Validate();
		base.parent?.Validate();
		return this;
	}

	public void Scroll(Vector2 scrollDelta)
	{
		currentScroll += -Math.Sign(scrollDelta.x + scrollDelta.y);
		Validate(validateItems: false);
	}

	private Vector2 GetLocalPosition(Vector2 position)
	{
		return rectTransform.InverseTransformPoint(position);
	}

	private GridShape GetGridPosition(Vector2 position, GridShape shape)
	{
		float num = UISettings.current.gridSize;
		float num2 = (float)(shape.maxX - shape.minX + 1) * num;
		float num3 = (float)(shape.maxY - shape.minY + 1) * num;
		position = GetLocalPosition(position);
		position.y = 0f - position.y;
		position.x -= num2 / 2f;
		position.y -= num3 / 2f;
		return new GridShapeBuilder(shape).SetPosition((int)Math.Round(position.x / num), (int)Math.Round(position.y / num) + currentScroll).Build();
	}

	public int WidthPixelsClamp(int width)
	{
		return widthPixels;
	}

	public int HeightPixelsClamp(int height)
	{
		return height;
	}

	public void ResizePixels(int width, int height)
	{
		heightPixels = height;
		Validate();
	}

	public int XClamp(int x)
	{
		return 0;
	}

	public int YClamp(int y)
	{
		return 0;
	}

	public override void Validate()
	{
		Validate();
	}

	public void Validate(bool validateItems = true)
	{
		int gridSize = UISettings.current.gridSize;
		int num = width * gridSize;
		widthPixels = num + scrollPadding + minButtonWidth + 2;
		if (heightPixels <= 0)
		{
			heightPixels = 1;
		}
		int num2 = heightPixels % gridSize;
		if (num2 > 0)
		{
			heightPixels += gridSize - num2;
		}
		int num3 = GetMinVisibleHeight() - GetVisibleHeight();
		if (currentScroll < 0)
		{
			currentScroll = 0;
		}
		if (currentScroll > num3)
		{
			currentScroll = num3;
		}
		innerMask.rectTransform.sizeDelta = new Vector2(num, heightPixels);
		inner.rectTransform.anchoredPosition = new Vector2(0f, currentScroll * gridSize);
		insideBorder.gameObject.SetActive(value: true);
		background.image.enabled = true;
		background.rectTransform.sizeDelta = new Vector2(num, heightPixels);
		insideBorder.rectTransform.sizeDelta = new Vector2(num + 2, heightPixels + 2);
		insideBorder.rectTransform.anchoredPosition = new Vector2(-1f, 1f);
		if (validateItems)
		{
			foreach (GameItem item in items)
			{
				item.Validate();
			}
		}
		int num4 = heightPixels - 2;
		int num5 = (int)Math.Round((float)GetVisibleHeight() / (float)GetMinVisibleHeight() * (float)num4);
		if (num5 < minButtonHeight)
		{
			num5 = minButtonHeight;
		}
		ValidateScrollBar(num5, num4);
	}

	private void ValidateScrollBar(int buttonHeight, int barInnerHeight)
	{
		int gridSize = UISettings.current.gridSize;
		int num = width * gridSize;
		scrollBar.rectTransform.sizeDelta = new Vector2(minButtonWidth + 2, heightPixels);
		scrollBar.rectTransform.anchoredPosition = new Vector2(num + scrollPadding, 0f);
		int num2 = (int)Math.Round(GetScroll() * (float)(barInnerHeight - buttonHeight));
		scrollButton.rectTransform.sizeDelta = new Vector2(minButtonWidth, buttonHeight);
		scrollButton.rectTransform.anchoredPosition = new Vector2(num + scrollPadding + 1, -1 - num2);
	}

	private void PlaceButtonAtMousePositionCenter(Vector2 mousePosition)
	{
		int num = (int)scrollButton.rectTransform.sizeDelta.y;
		int num2 = Math.Clamp((int)(handler.rectTransform.InverseTransformPoint(mousePosition).y + scrollButton.rectTransform.sizeDelta.y / 2f), -heightPixels + 1 + num, -1);
		scrollButton.rectTransform.anchoredPosition = new Vector2(scrollButton.rectTransform.anchoredPosition.x, num2);
	}

	private void PlaceButtonAtMousePositionRelative(Vector2 mousePosition, float relativePosition)
	{
		int num = (int)scrollButton.rectTransform.sizeDelta.y;
		int num2 = Math.Clamp((int)(handler.rectTransform.InverseTransformPoint(mousePosition).y - relativePosition), -heightPixels + 1 + num, -1);
		scrollButton.rectTransform.anchoredPosition = new Vector2(scrollButton.rectTransform.anchoredPosition.x, num2);
	}

	private float GetButtonScroll()
	{
		int num = heightPixels - 2;
		float y = scrollButton.rectTransform.sizeDelta.y;
		return (-1f - scrollButton.rectTransform.anchoredPosition.y) / ((float)num - y);
	}

	public float GetScroll()
	{
		int num = GetMinVisibleHeight() - GetVisibleHeight();
		return (float)currentScroll / (float)num;
	}

	public void SetScroll(float percentage)
	{
		int num = GetMinVisibleHeight() - GetVisibleHeight();
		if (num > 0)
		{
			percentage = Math.Clamp(percentage, 0f, 1f);
			currentScroll = (int)Math.Round(percentage * (float)num);
			Validate();
		}
	}

	public int GetVisibleHeight()
	{
		return heightPixels / UISettings.current.gridSize;
	}

	public int GetMinVisibleHeight()
	{
		int visibleHeight = GetVisibleHeight();
		return Math.Min(Math.Max(childItems.Select((GameItem item) => item.modifiedShape.minY + item.modifiedShape.globalHeight).DefaultIfEmpty().Max(), visibleHeight) + (int)((float)visibleHeight * scrollOverflow), height);
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
		GameItem item = slot.item;
		GameItem item2 = slot2.item;
		if (!childItems.Contains(item))
		{
			return false;
		}
		if (!childItems.Contains(item2))
		{
			return false;
		}
		GridShapeBuilder gridShapeBuilder = new GridShapeBuilder(item.modifiedShape);
		gridShapeBuilder.minX++;
		if (item2.modifiedShape.Intersects(gridShapeBuilder))
		{
			return true;
		}
		gridShapeBuilder.minX--;
		gridShapeBuilder.minX--;
		if (item2.modifiedShape.Intersects(gridShapeBuilder))
		{
			return true;
		}
		gridShapeBuilder.minX++;
		gridShapeBuilder.minY++;
		if (item2.modifiedShape.Intersects(gridShapeBuilder))
		{
			return true;
		}
		gridShapeBuilder.minY--;
		gridShapeBuilder.minY--;
		if (item2.modifiedShape.Intersects(gridShapeBuilder))
		{
			return true;
		}
		gridShapeBuilder.minY++;
		return false;
	}

	private void InitHighlightSquare()
	{
		TreeNodeRender treeNodeRender = TreeNodeRender.Instantiate().SetParent(highlightNode);
		treeNodeRender.image.raycastTarget = false;
		RenderHandler.current.SetHighlightColor(treeNodeRender.image, RenderHandler.BackgroundColor.Black, RenderHandler.BackgroundType.Grid);
		treeNodeRender.image.enabled = false;
		treeNodeRender.image.SetNativeSize();
		highlightSquares.Add(treeNodeRender);
	}

	private TreeNodeRender SetHighlightSquare(int x, int y, RenderHandler.BackgroundColor color, bool doReplace)
	{
		int gridSize = UISettings.current.gridSize;
		int num = y * width + x;
		while (num >= highlightSquares.Count)
		{
			InitHighlightSquare();
		}
		highlightSquares[num].rectTransform.anchoredPosition = new Vector2(x * gridSize, -y * gridSize);
		if (!highlightSquares[num].image.enabled || doReplace)
		{
			RenderHandler.current.SetHighlightColor(highlightSquares[num].image, color, RenderHandler.BackgroundType.Grid);
			return highlightSquares[num];
		}
		return null;
	}

	public override List<TreeNodeRender> HighlightSlot(SlotMarker slot, RenderHandler.BackgroundColor color, bool doReplace)
	{
		GridShape itemGridShape = slot.itemGridShape;
		List<TreeNodeRender> list = new List<TreeNodeRender>();
		for (int i = 0; i < itemGridShape.height; i++)
		{
			for (int j = 0; j < itemGridShape.width; j++)
			{
				var (num, num2) = itemGridShape.ForwardTransform(j, i);
				if (itemGridShape.GetLocal(j, i) != 0 && num >= 0 && num < width && num2 >= 0 && num2 < height)
				{
					TreeNodeRender treeNodeRender = SetHighlightSquare(num, num2, color, doReplace);
					if ((object)treeNodeRender != null)
					{
						list.Add(treeNodeRender);
					}
				}
			}
		}
		return list;
	}

	public override void ClearHighlight()
	{
		foreach (TreeNodeRender highlightSquare in highlightSquares)
		{
			highlightSquare.image.enabled = false;
		}
	}

	public override List<TreeNodeRender> HighlightWholeInventory(RenderHandler.BackgroundColor color)
	{
		int num = Math.Max(1, heightPixels / UISettings.current.gridSize);
		SlotMarker slotMarker = new SlotMarker();
		slotMarker.inventory = this;
		slotMarker.itemGridShape = new GridShapeBuilder(width, num).SetDataFill(1).SetPosition(0, currentScroll).Build();
		return HighlightSlot(slotMarker, color, doReplace: true);
	}

	public override bool CheckShapePlacement(GameItem item, GridShape shape, List<GameItem> ignoredItems)
	{
		if (item == null || shape == null)
		{
			return false;
		}
		if (!shape.FullyIntersectsRect(width, height, (byte b, byte bo) => b != 1 || bo == 0))
		{
			return false;
		}
		if (!items.All((GameItem o) => item == o || ignoredItems.Contains(o) || !shape.Intersects(o.modifiedShape)))
		{
			return false;
		}
		SlotMarker slotMarker = new SlotMarker();
		slotMarker.item = item;
		slotMarker.inventory = this;
		slotMarker.itemGridShape = shape;
		return item.CheckIsStateValid(this, slotMarker, shape, item.modifiedState);
	}

	public override bool CheckInventorySlotStillValid(GameItem item, GridShape tempShape, TagSystem tempState)
	{
		SlotMarker slotMarker = new SlotMarker();
		slotMarker.item = item;
		slotMarker.inventory = this;
		slotMarker.itemGridShape = tempShape;
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
		if (!tempShape.FullyIntersectsRect(width, height, (byte b, byte bo) => b != 1 || bo == 0))
		{
			return false;
		}
		if (!items.All((GameItem o) => item == o || !tempShape.Intersects(o.modifiedShape)))
		{
			return false;
		}
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
		if (!MayHaveValidInventorySlot(item))
		{
			return slotMarker;
		}
		if (float.IsFinite(itemGraphicsCenterPosition.x))
		{
			tempShape = GetGridPosition(itemGraphicsCenterPosition, tempShape);
		}
		slotMarker.itemGridShape = tempShape;
		if (!tempShape.FullyIntersectsRect(width, height, (byte b, byte bo) => b != 1 || bo == 0))
		{
			return slotMarker;
		}
		if (!items.All((GameItem o) => (item == o && requestedNum >= item.unitCount) || !tempShape.Intersects(o.modifiedShape)))
		{
			return slotMarker;
		}
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
		SlotMarker slotMarker = TryInventorySlot(item, item.modifiedShape);
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
		if (items.Contains(item))
		{
			return false;
		}
		if (alwaysStack && FastStackUnchecked(item) != null)
		{
			OverlayHandler.current.UpdateInventoryStateUI();
			return true;
		}
		base.onSlotAddItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		items.Add(item);
		item.AddParent(this);
		if (item is PixelElement pixelElement)
		{
			pixelElement.SetRenderingParent(this);
		}
		item.onSlotAddItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		base.onSlotAddItemEndFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
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
		if (!items.Contains(item))
		{
			return false;
		}
		item.onSlotRemoveItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		base.onSlotRemoveItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		bool flag = items.Remove(item);
		if (!flag)
		{
			return false;
		}
		item.RemoveParent(this);
		if (item is PixelElement pixelElement)
		{
			pixelElement.RemoveRenderingParent(this);
		}
		base.onSlotRemoveItemEndFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		GameItem gameItem = GraphUtils.FindParentType<GameItem>(this);
		gameItem?.ValidateShapeState();
		gameItem?.Validate();
		OverlayHandler.current.UpdateInventoryStateUI();
		return flag;
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
		if ((item.modifiedShape.width > width || item.modifiedShape.height > height) && (item.modifiedShape.width > height || item.modifiedShape.height > width))
		{
			return false;
		}
		Func<GameItem, GameInventory, bool> func = item.mayInventoryAddItemFunc;
		if (func != null && !func.InvokeAllReduce(item, this, InvokeFuncExtensions.BoolReduceOR))
		{
			return false;
		}
		if (HopperHelper.MayBypassGridRules(item, this))
		{
			return true;
		}
		Func<GameItem, GameInventory, bool> func2 = base.mayInventoryAddItemFunc;
		if (func2 != null && !func2.InvokeAllReduce(item, this, InvokeFuncExtensions.BoolReduceOR))
		{
			return false;
		}
		return true;
	}

	public override SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		return TryFindOneValidInventorySlot(item);
	}

	public SlotMarker TryFindOneValidInventorySlot(GameItem item, bool keepOrientation = false)
	{
		int num = item.MaxNumRemove();
		if (num < 1)
		{
			return null;
		}
		items.Sort(delegate(GameItem x, GameItem y)
		{
			if (x.modifiedShape.minX == y.modifiedShape.minX && x.modifiedShape.minY == y.modifiedShape.minY)
			{
				return 0;
			}
			return ((x.modifiedShape.minX > y.modifiedShape.minX && x.modifiedShape.minY == y.modifiedShape.minY) || x.modifiedShape.minY > y.modifiedShape.minY) ? 1 : (-1);
		});
		SlotMarker slotMarker = new SlotMarker();
		slotMarker.item = item;
		slotMarker.inventory = this;
		slotMarker.itemGridShape = item.modifiedShape;
		foreach (GameItem item2 in items)
		{
			if (item == item2)
			{
				continue;
			}
			Func<GameItem, GameInventory, SlotMarker, GameItem, bool> func = item2.slotSearchPassthroughForItemFunc;
			if (func == null || !func(item, this, slotMarker, item2))
			{
				Func<GameItem, GameInventory, SlotMarker, GameItem, bool> func2 = base.slotSearchPassthroughForItemFunc;
				if (func2 == null || !func2(item, this, slotMarker, item2))
				{
					continue;
				}
			}
			SlotMarker slotMarker2 = item2.TryFindOneValidInventorySlot(item);
			if (slotMarker2 != null)
			{
				return slotMarker2;
			}
		}
		if (!MayHaveValidInventorySlot(item))
		{
			return null;
		}
		foreach (GameItem item3 in items)
		{
			if (item != item3 && item.MayStack(item3))
			{
				slotMarker.targetItem = item3;
				slotMarker.numTransfer = Math.Min(num, item3.MaxNumAdd());
				if (slotMarker.numTransfer >= 1)
				{
					slotMarker.priority = Math.Max(base.slotPriorityFunc?.Invoke(item, this, slotMarker) ?? 0, item.slotPriorityFunc?.Invoke(item, this, slotMarker) ?? 0);
					return slotMarker;
				}
			}
		}
		GridShapeBuilder gridShapeBuilder = new GridShapeBuilder(item.modifiedShape);
		int num2 = (keepOrientation ? 1 : 4);
		int num3 = (keepOrientation ? 1 : 2);
		for (int i = 0; i < num3; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				for (int k = 0; k < height; k++)
				{
					for (int l = 0; l < width; l++)
					{
						int orientation = 4 - j;
						bool flipped = i != 0;
						gridShapeBuilder.SetTransform(l, k, flipped, orientation);
						slotMarker = TryInventorySlot(item, num, gridShapeBuilder);
						if (slotMarker.numTransfer > 0)
						{
							return slotMarker;
						}
					}
				}
			}
		}
		return null;
	}

	public GameItem FastStackUnchecked(GameItem item)
	{
		foreach (GameItem item2 in items)
		{
			if (item.MayStack(item2))
			{
				SlotMarker.StackItemUnchecked(item, item2);
				return item2;
			}
		}
		return null;
	}

	public void AddFastUnchecked(List<GameItem> allItems)
	{
		List<GameItem> list = new List<GameItem>();
		List<GameItem> list2 = new List<GameItem>();
		foreach (GameItem allItem in allItems)
		{
			if (FastStackUnchecked(allItem) != null)
			{
				list2.Add(allItem);
			}
			else
			{
				list.Add(allItem);
			}
		}
		list2.ForEach(delegate(GameItem item)
		{
			item.Destroy();
		});
		allItems = list;
		allItems = allItems.FindAll(delegate(GameItem item)
		{
			if (item.modifiedShape.width > width)
			{
				item.SetShape(new GridShapeBuilder(item.shape).SetOrientation(item.modifiedShape.orientation + 1));
				if (item.modifiedShape.width > width)
				{
					Debug.LogWarning("Trying to fast add GameItem " + item.identifier + " in ScrollableInventory when the item is wider than the inventory, ignoring item...");
					return false;
				}
			}
			return true;
		});
		allItems.Sort(delegate(GameItem o1, GameItem o2)
		{
			if (o1.modifiedShape.width == o2.modifiedShape.width && o1.modifiedShape.height == o2.modifiedShape.height)
			{
				return 0;
			}
			return ((o1.modifiedShape.width > o2.modifiedShape.width && o1.modifiedShape.height == o2.modifiedShape.height) || o1.modifiedShape.height > o2.modifiedShape.height) ? 1 : (-1);
		});
		int num = 0;
		int num2 = 0;
		foreach (GameItem allItem2 in allItems)
		{
			GridShape tempShape;
			GridShapeBuilder gridShapeBuilder = (GridShapeBuilder)(tempShape = new GridShapeBuilder(allItem2.modifiedShape));
			while (true)
			{
				if (num2 >= height)
				{
					Debug.LogWarning("AddFast in ScrollableInventory ran out of space, ignoring remaining items...");
					return;
				}
				gridShapeBuilder.SetTransform(num, num2, flipped: false, 0);
				bool flag = true;
				if (!tempShape.FullyIntersectsRect(width, height, (byte b, byte bo) => b != 1 || bo == 0))
				{
					flag = false;
				}
				if (!items.All((GameItem o) => !tempShape.Intersects(o.modifiedShape)))
				{
					flag = false;
				}
				if (flag)
				{
					break;
				}
				num = 0;
				num2++;
			}
			allItem2.modifiedShape = new GridShapeBuilder(allItem2.modifiedShape).SetTransform(gridShapeBuilder);
			UncheckedAccept(allItem2);
			num += allItem2.modifiedShape.width;
		}
	}
}

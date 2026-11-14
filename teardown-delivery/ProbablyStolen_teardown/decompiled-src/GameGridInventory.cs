using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameGridInventory : GameInventory, PixelElement
{
	private readonly TreeNodeRender dynamicBackgroundBase;

	private readonly TreeNodeRender background;

	private readonly TreeNodeRender insideBorder;

	public bool renderDynamicBackground;

	private readonly List<TreeNodeRender> dynamicBackgrounds = new List<TreeNodeRender>();

	private readonly List<TreeNodeRender> dynamicR0 = new List<TreeNodeRender>();

	private readonly List<TreeNodeRender> dynamicR1 = new List<TreeNodeRender>();

	private readonly List<TreeNodeRender> dynamicR2 = new List<TreeNodeRender>();

	private readonly List<TreeNodeRender> dynamicR3 = new List<TreeNodeRender>();

	private readonly List<GameItem> items = new List<GameItem>();

	private readonly List<TreeNodeRender> highlightSquares = new List<TreeNodeRender>();

	private readonly Dictionary<(int x, int y), Sprite> cellBackdropCells = new Dictionary<(int, int), Sprite>();

	private readonly List<TreeNodeRender> cellBackdrops = new List<TreeNodeRender>();

	private int _lastShapeWidth = -1;

	private int _lastShapeHeight = -1;

	private int _lastShapeHash;

	private bool _lastRenderDynamicBackground;

	private bool hideBorder;

	private bool machineryStyle;

	private Sprite solidSprite;

	public override List<GraphNodeStorage> children => new List<GraphNodeStorage>(items);

	public override List<GameItem> childItems => new List<GameItem>(items);

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	public float transparency { get; private set; }

	public GridShape inventoryShape { get; private set; }

	public bool isMachineryStyle => machineryStyle;

	public override bool SupportsGroupPlacement => true;

	public GameGridInventory SetCellBackdrop(int x, int y, Sprite sprite)
	{
		if (sprite == null)
		{
			cellBackdropCells.Remove((x, y));
		}
		else
		{
			cellBackdropCells[(x, y)] = sprite;
		}
		ValidateCellBackdrops();
		return this;
	}

	public bool HasCellBackdrop(int x, int y)
	{
		return cellBackdropCells.ContainsKey((x, y));
	}

	public GameGridInventory ClearCellBackdrops()
	{
		cellBackdropCells.Clear();
		ValidateCellBackdrops();
		return this;
	}

	public bool HasFreeCell()
	{
		if (inventoryShape == null)
		{
			return false;
		}
		for (int i = 0; i < inventoryShape.height; i++)
		{
			for (int j = 0; j < inventoryShape.width; j++)
			{
				if (inventoryShape.Get(j, i) != 0)
				{
					continue;
				}
				bool flag = false;
				foreach (GameItem item in items)
				{
					if (item.modifiedShape.Intersects(j, i))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CanTakeAnything()
	{
		if (HasFreeCell())
		{
			return true;
		}
		if (base.slotSearchPassthroughForItemFunc != null)
		{
			return true;
		}
		foreach (GameItem item in items)
		{
			if (item.MaxNumAdd() > 0)
			{
				return true;
			}
			if (item.slotSearchPassthroughForItemFunc != null)
			{
				return true;
			}
		}
		return false;
	}

	private GameGridInventory()
	{
		handler = TreeNodeRender.Instantiate(this);
		dynamicBackgroundBase = TreeNodeRender.Instantiate().SetParent(handler);
		background = TreeNodeRender.Instantiate().SetParent(handler);
		inner = TreeNodeRender.Instantiate().SetParent(handler);
		inner.mask.enabled = true;
		int gridSize = UISettings.current.gridSize;
		inner.mask.padding = new Vector4(-gridSize, -gridSize, -gridSize, -gridSize);
		insideBorder = TreeNodeRender.Instantiate().SetParent(handler);
		RenderHandler.current.SetHighlightColor(background.image, RenderHandler.BackgroundColor.Tan, RenderHandler.BackgroundType.Grid);
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
	}

	public GameGridInventory(int width, int height)
		: this()
	{
		inventoryShape = new GridShapeBuilder(width, height).SetDataFill(0).SetDataOutside(1).Build();
		Validate();
	}

	public GameGridInventory(string shape, int width)
		: this()
	{
		inventoryShape = new GridShapeBuilder().SetData(shape, width).SetDataOutside(1).Build();
		renderDynamicBackground = true;
		Validate();
	}

	public GameGridInventory SetShape(int width, int height)
	{
		inventoryShape = new GridShapeBuilder(width, height).SetDataFill(0).SetDataOutside(1).Build();
		Validate();
		return this;
	}

	public GameGridInventory SetShape(string shape, int width)
	{
		inventoryShape = new GridShapeBuilder().SetData(shape, width).SetDataOutside(1).Build();
		renderDynamicBackground = true;
		ForceRecomputeDynamicBackground();
		Validate();
		return this;
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
		return new GridShapeBuilder(shape).SetPosition((int)Math.Round(position.x / num), (int)Math.Round(position.y / num)).Build();
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

	public override void Validate()
	{
		int gridSize = UISettings.current.gridSize;
		widthPixels = inventoryShape.width * gridSize;
		heightPixels = inventoryShape.height * gridSize;
		ValidateBackground();
		ValidateCellBackdrops();
		foreach (GameItem item in items)
		{
			item.Validate();
		}
		insideBorder.image.color = new Color(1f, 1f, 1f, 1f - transparency);
		background.image.color = new Color(1f, 1f, 1f, 1f - transparency);
		if (hideBorder)
		{
			insideBorder.image.enabled = false;
		}
	}

	public GameGridInventory SetTransparency(float transparency)
	{
		this.transparency = transparency;
		Validate();
		return this;
	}

	public GameGridInventory SetBackgroundSprite(string atlasPath, string spritePath)
	{
		Sprite sprite = RenderHandler.LoadFromAtlas(atlasPath, spritePath);
		if (sprite == null)
		{
			return this;
		}
		background.image.sprite = sprite;
		if (renderDynamicBackground)
		{
			ForceRecomputeDynamicBackground();
		}
		Validate();
		return this;
	}

	public GameGridInventory SetMachineryStyle()
	{
		if (machineryStyle)
		{
			return this;
		}
		machineryStyle = true;
		HideBorder();
		SetBackgroundSprite("Window/machinery", "cell");
		return this;
	}

	public GameGridInventory SetSolidSprite(string atlasPath, string spritePath)
	{
		solidSprite = RenderHandler.LoadFromAtlas(atlasPath, spritePath);
		if (renderDynamicBackground)
		{
			ForceRecomputeDynamicBackground();
		}
		Validate();
		return this;
	}

	private void ValidateBackground()
	{
		inner.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		if (!renderDynamicBackground)
		{
			if (dynamicBackgroundBase.gameObject.activeSelf)
			{
				dynamicBackgrounds.ForEach(delegate(TreeNodeRender n)
				{
					n.image.enabled = false;
				});
				dynamicR0.ForEach(delegate(TreeNodeRender n)
				{
					n.image.enabled = false;
				});
				dynamicR1.ForEach(delegate(TreeNodeRender n)
				{
					n.image.enabled = false;
				});
				dynamicR2.ForEach(delegate(TreeNodeRender n)
				{
					n.image.enabled = false;
				});
				dynamicR3.ForEach(delegate(TreeNodeRender n)
				{
					n.image.enabled = false;
				});
			}
			dynamicBackgroundBase.gameObject.SetActive(value: false);
			insideBorder.gameObject.SetActive(value: true);
			background.image.enabled = true;
			background.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
			insideBorder.rectTransform.sizeDelta = new Vector2(widthPixels + 2, heightPixels + 2);
			insideBorder.rectTransform.anchoredPosition = new Vector2(-1f, 1f);
			return;
		}
		int num = ComputeShapeHash(inventoryShape);
		if (_lastRenderDynamicBackground == renderDynamicBackground && _lastShapeWidth == inventoryShape.width && _lastShapeHeight == inventoryShape.height && _lastShapeHash == num)
		{
			return;
		}
		_lastRenderDynamicBackground = renderDynamicBackground;
		_lastShapeWidth = inventoryShape.width;
		_lastShapeHeight = inventoryShape.height;
		_lastShapeHash = num;
		dynamicBackgroundBase.gameObject.SetActive(value: true);
		insideBorder.gameObject.SetActive(value: false);
		background.image.enabled = false;
		for (int i = 0; i < inventoryShape.height + 2; i++)
		{
			for (int j = 0; j < inventoryShape.width + 2; j++)
			{
				int num2 = i * (inventoryShape.width + 2) + j;
				int num3 = i - 1;
				int num4 = j - 1;
				while (num2 >= dynamicBackgrounds.Count)
				{
					TreeNodeRender treeNodeRender = TreeNodeRender.Instantiate().SetParent(dynamicBackgroundBase);
					treeNodeRender.image.raycastTarget = true;
					dynamicBackgrounds.Add(treeNodeRender);
					treeNodeRender = TreeNodeRender.Instantiate().SetParent(dynamicBackgroundBase);
					treeNodeRender.image.fillCenter = false;
					dynamicR0.Add(treeNodeRender);
					treeNodeRender = TreeNodeRender.Instantiate().SetParent(dynamicBackgroundBase);
					treeNodeRender.image.fillCenter = false;
					dynamicR1.Add(treeNodeRender);
					treeNodeRender = TreeNodeRender.Instantiate().SetParent(dynamicBackgroundBase);
					treeNodeRender.image.fillCenter = false;
					dynamicR2.Add(treeNodeRender);
					treeNodeRender = TreeNodeRender.Instantiate().SetParent(dynamicBackgroundBase);
					treeNodeRender.image.fillCenter = false;
					dynamicR3.Add(treeNodeRender);
				}
				bool flag = num4 >= 0 && num3 >= 0 && num4 < inventoryShape.width && num3 < inventoryShape.height;
				RenderHandler.current.SetGridFillBackground(inventoryShape, num4, num3, dynamicBackgrounds[num2], dynamicR0[num2], dynamicR1[num2], dynamicR2[num2], dynamicR3[num2], background.image.sprite, flag ? solidSprite : null, !machineryStyle);
			}
		}
	}

	public void ForceRecomputeDynamicBackground()
	{
		_lastShapeWidth = -1;
		_lastShapeHeight = -1;
		_lastShapeHash = 0;
		_lastRenderDynamicBackground = !renderDynamicBackground;
		ValidateBackground();
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

	private void ValidateCellBackdrops()
	{
		int gridSize = UISettings.current.gridSize;
		int num = 0;
		foreach (KeyValuePair<(int, int), Sprite> cellBackdropCell in cellBackdropCells)
		{
			while (num >= cellBackdrops.Count)
			{
				InitCellBackdrop();
			}
			TreeNodeRender treeNodeRender = cellBackdrops[num];
			num++;
			treeNodeRender.image.sprite = cellBackdropCell.Value;
			treeNodeRender.image.SetNativeSize();
			treeNodeRender.rectTransform.anchoredPosition = new Vector2(cellBackdropCell.Key.Item1 * gridSize, -cellBackdropCell.Key.Item2 * gridSize);
			treeNodeRender.image.enabled = true;
		}
		for (int i = num; i < cellBackdrops.Count; i++)
		{
			cellBackdrops[i].image.enabled = false;
		}
	}

	private void InitCellBackdrop()
	{
		TreeNodeRender treeNodeRender = TreeNodeRender.Instantiate().SetParent(background);
		treeNodeRender.rectTransform.SetAsFirstSibling();
		treeNodeRender.image.raycastTarget = false;
		treeNodeRender.image.enabled = false;
		cellBackdrops.Add(treeNodeRender);
	}

	private void InitHighlightSquare()
	{
		TreeNodeRender treeNodeRender = TreeNodeRender.Instantiate().SetParent(background);
		treeNodeRender.image.raycastTarget = false;
		RenderHandler.current.SetHighlightColor(treeNodeRender.image, RenderHandler.BackgroundColor.Tan, RenderHandler.BackgroundType.Grid);
		treeNodeRender.image.enabled = false;
		treeNodeRender.image.SetNativeSize();
		highlightSquares.Add(treeNodeRender);
	}

	private TreeNodeRender SetHighlightSquare(int x, int y, RenderHandler.BackgroundColor color, bool doReplace)
	{
		int gridSize = UISettings.current.gridSize;
		int num = y * inventoryShape.width + x;
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
				var (x, y) = itemGridShape.ForwardTransform(j, i);
				if (itemGridShape.GetLocal(j, i) != 0 && !inventoryShape.Intersects(x, y))
				{
					TreeNodeRender treeNodeRender = SetHighlightSquare(x, y, color, doReplace);
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
		SlotMarker slotMarker = new SlotMarker();
		slotMarker.inventory = this;
		slotMarker.itemGridShape = new GridShapeBuilder(inventoryShape.width, inventoryShape.height).SetDataFill(1).Build();
		return HighlightSlot(slotMarker, color, doReplace: true);
	}

	public override bool CheckShapePlacement(GameItem item, GridShape shape, List<GameItem> ignoredItems)
	{
		if (item == null || shape == null)
		{
			return false;
		}
		if (!shape.FullyIntersects(inventoryShape, (byte b, byte bo) => b != 1 || bo == 0))
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
		if (!tempShape.FullyIntersects(inventoryShape, (byte b, byte bo) => b != 1 || bo == 0))
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
		if (!tempShape.FullyIntersects(inventoryShape, (byte b, byte bo) => b != 1 || bo == 0))
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
		base.onSlotAddItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		items.Add(item);
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
		base.onSlotRemoveItemLateFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
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
		if ((item.modifiedShape.width > inventoryShape.width || item.modifiedShape.height > inventoryShape.height) && (item.modifiedShape.width > inventoryShape.height || item.modifiedShape.height > inventoryShape.width))
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
				for (int k = 0; k < inventoryShape.height; k++)
				{
					for (int l = 0; l < inventoryShape.width; l++)
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

	public SlotMarker TryFindOneValidInventorySlot_TopMiddle(GameItem item, bool keepOrientation = false)
	{
		int num = item.MaxNumRemove();
		if (num < 1)
		{
			return null;
		}
		items.Sort(delegate(GameItem x, GameItem y)
		{
			if (x.modifiedShape.minY == y.modifiedShape.minY && x.modifiedShape.minX == y.modifiedShape.minX)
			{
				return 0;
			}
			return ((x.modifiedShape.minY > y.modifiedShape.minY && x.modifiedShape.minX == y.modifiedShape.minX) || x.modifiedShape.minX > y.modifiedShape.minX) ? 1 : (-1);
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
		int num4 = (inventoryShape.width - item.shape.width) / 2;
		int num5 = (inventoryShape.height - item.shape.height) / 2;
		for (int i = 0; i < num3; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				for (int k = 0; k <= num5; k++)
				{
					for (int l = 0; l <= num4; l++)
					{
						int num6 = num4 - l;
						int num7 = num4 + l;
						int num8 = num5 - k;
						int num9 = num5 + k;
						if (num6 >= 0 && num8 >= 0)
						{
							gridShapeBuilder.SetTransform(num6, num8, i != 0, 4 - j);
							slotMarker = TryInventorySlot(item, num, gridShapeBuilder);
							if (slotMarker.numTransfer > 0)
							{
								return slotMarker;
							}
						}
						if (num7 < inventoryShape.width && num9 < inventoryShape.height && (num7 != num6 || num9 != num8))
						{
							gridShapeBuilder.SetTransform(num7, num9, i != 0, 4 - j);
							slotMarker = TryInventorySlot(item, num, gridShapeBuilder);
							if (slotMarker.numTransfer > 0)
							{
								return slotMarker;
							}
						}
					}
				}
			}
		}
		return null;
	}

	private int ComputeShapeHash(GridShape shape)
	{
		return (((((17 * 31 + shape.width) * 31 + shape.height) * 31 + shape.minX) * 31 + shape.minY) * 31 + shape.maxX) * 31 + shape.maxY;
	}

	public void HideBorder()
	{
		insideBorder.gameObject.SetActive(value: false);
		hideBorder = true;
	}

	public void ShowBorder()
	{
		insideBorder.gameObject.SetActive(value: true);
		hideBorder = false;
	}
}

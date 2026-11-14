using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameCharacterRaidInventory : GameInventory, PixelElement
{
	private readonly TreeNodeRender background;

	private readonly TreeNodeRender insideBorder;

	private readonly List<GameCharacterItem> characterItems = new List<GameCharacterItem>();

	private readonly List<TreeNodeRender> highlightSquares = new List<TreeNodeRender>();

	public int gridWidth = 1;

	public int gridHeight = 1;

	public readonly bool flipped;

	private bool updatedCharacterPositions;

	public override List<GraphNodeStorage> children => new List<GraphNodeStorage>(characterItems);

	public override List<GameItem> childItems => new List<GameItem>(characterItems);

	public bool canResize => true;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	private GameCharacterRaidInventory()
	{
		handler = TreeNodeRender.Instantiate(this, "Game Character Inventory");
		background = TreeNodeRender.Instantiate().SetParent(handler);
		inner = TreeNodeRender.Instantiate().SetParent(handler);
		insideBorder = TreeNodeRender.Instantiate().SetParent(handler);
		RenderHandler.current.SetHighlightColor(background.image, RenderHandler.BackgroundColor.Black, RenderHandler.BackgroundType.Smooth);
		background.image.SetNativeSize();
		background.InitEventHandler();
		background.image.color = new Vector4(0f, 0f, 0f, 0f);
		background.image.enabled = true;
	}

	public GameCharacterRaidInventory(bool flipped = false)
		: this()
	{
		this.flipped = flipped;
		Validate();
	}

	public List<GameCharacterItem> GetCharacterItems()
	{
		return characterItems.ToList();
	}

	public void Lock()
	{
		base.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		base.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		characterItems.ForEach(delegate(GameCharacterItem c)
		{
			c.Lock();
		});
	}

	public void Unlock()
	{
		base.mayInventoryRemoveItemFunc = null;
		base.mayInventoryAddItemFunc = null;
		characterItems.ForEach(delegate(GameCharacterItem c)
		{
			c.Unlock();
		});
	}

	public List<CombatEntity> GetTeam()
	{
		List<CombatEntity> list = new List<CombatEntity>();
		characterItems.ForEach(delegate(GameCharacterItem c)
		{
			list.Add(c);
		});
		return list;
	}

	public GameCharacterItem Find(CombatEntity combatEntity)
	{
		return characterItems.Find((GameCharacterItem c) => c == combatEntity);
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
		return width;
	}

	public int HeightPixelsClamp(int height)
	{
		return height;
	}

	public void ResizePixels(int width, int height)
	{
		int gridSize = UISettings.current.gridSize;
		gridWidth = width / gridSize;
		gridHeight = height / gridSize;
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
		int gridSize = UISettings.current.gridSize;
		widthPixels = gridWidth * gridSize;
		heightPixels = gridHeight * gridSize;
		ValidateBackground();
		if (!updatedCharacterPositions || !flipped)
		{
			characterItems.Sort(delegate(GameCharacterItem x, GameCharacterItem y)
			{
				if (x.modifiedShape.minX == y.modifiedShape.minX && x.modifiedShape.minY == y.modifiedShape.minY)
				{
					return 0;
				}
				return ((x.modifiedShape.minX > y.modifiedShape.minX && x.modifiedShape.minY == y.modifiedShape.minY) || x.modifiedShape.minY > y.modifiedShape.minY) ? 1 : (-1);
			});
			int num = 0;
			foreach (GameCharacterItem characterItem in characterItems)
			{
				characterItem.modifiedShape = new GridShapeBuilder(characterItem.modifiedShape).SetTransform(((num % 2 != 0) ? 4 : 0) + num / 2, num % 2 * 2 + num / 2 * 6 + 1, flipped, 0);
				if (characterItem is PixelElement pixelElement)
				{
					pixelElement.handler.rectTransform.SetAsLastSibling();
				}
				num++;
			}
			updatedCharacterPositions = true;
		}
		foreach (GameCharacterItem characterItem2 in characterItems)
		{
			characterItem2.Validate();
		}
	}

	private void ValidateBackground()
	{
		insideBorder.gameObject.SetActive(value: true);
		background.image.enabled = true;
		background.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		insideBorder.rectTransform.sizeDelta = new Vector2(widthPixels + 2, heightPixels + 2);
		insideBorder.rectTransform.anchoredPosition = new Vector2(-1f, 1f);
	}

	public List<CombatEntity> GetCombatEntities()
	{
		return (from i in childItems.FindAll((GameItem i) => i is CombatEntity)
			select (CombatEntity)i).ToList();
	}

	public void ValidateDeaths()
	{
		foreach (GameCharacterItem childItem in childItems)
		{
			if (childItem.isDead)
			{
				GameMaster.current.raidScene.RestoreCharacterInventory(childItem);
				Expel(childItem);
				(GameMaster.current.raidManager.currentRoom?.floorInventory?.TryFindOneValidInventorySlot(childItem))?.TryAcceptOnce();
			}
		}
		Validate();
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

	private TreeNodeRender SetHighlightSquare(int x, int y, RenderHandler.BackgroundColor color, bool doReplace)
	{
		int gridSize = UISettings.current.gridSize;
		int num = y * gridWidth + x;
		while (num >= highlightSquares.Count)
		{
			TreeNodeRender treeNodeRender = TreeNodeRender.Instantiate().SetParent(background);
			treeNodeRender.image.raycastTarget = false;
			RenderHandler.current.SetHighlightColor(treeNodeRender.image, RenderHandler.BackgroundColor.Black, RenderHandler.BackgroundType.Smooth);
			treeNodeRender.image.enabled = false;
			treeNodeRender.image.SetNativeSize();
			highlightSquares.Add(treeNodeRender);
		}
		highlightSquares[num].rectTransform.anchoredPosition = new Vector2(x * gridSize, -y * gridSize);
		if (!highlightSquares[num].image.enabled || doReplace)
		{
			RenderHandler.current.SetHighlightColor(highlightSquares[num].image, color, RenderHandler.BackgroundType.Smooth);
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
				if (itemGridShape.GetLocal(j, i) != 0 && num >= 0 && num2 >= 0 && num < gridWidth && num2 < gridHeight)
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
		if (!MayHaveValidInventorySlot(item))
		{
			return false;
		}
		slotMarker.itemGridShape = tempShape;
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
		if (!(item is GameCharacterItem))
		{
			return false;
		}
		if (item == null)
		{
			return false;
		}
		if (item.parentInventory != null)
		{
			return false;
		}
		if (characterItems.Contains(item))
		{
			return false;
		}
		base.onSlotAddItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		characterItems.Add((GameCharacterItem)item);
		item.AddParent(this);
		if (item is PixelElement pixelElement)
		{
			pixelElement.SetRenderingParent(this);
		}
		item.modifiedShape = new GridShapeBuilder(item.modifiedShape).SetTransform(100, 100, flipped: false, 0);
		if (item is GameCharacterItem gameCharacterItem)
		{
			gameCharacterItem.Uncard();
		}
		item.onSlotAddItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		base.onSlotAddItemEndFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		updatedCharacterPositions = false;
		item.ValidateShapeState();
		return true;
	}

	public override bool Expel(GameItem item)
	{
		if (!(item is GameCharacterItem))
		{
			return false;
		}
		if (item == null)
		{
			return false;
		}
		if (item.parentInventory != this)
		{
			return false;
		}
		if (!characterItems.Contains(item))
		{
			return false;
		}
		item.onSlotRemoveItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		base.onSlotRemoveItemFunc?.Invoke(item, this, GetIncompleteItemInventorySlot(item));
		bool flag = characterItems.Remove((GameCharacterItem)item);
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
		if (item is GameCharacterItem gameCharacterItem)
		{
			gameCharacterItem.Card();
		}
		Validate();
		OverlayHandler.current.UpdateInventoryStateUI();
		return flag;
	}

	public override bool MayHaveValidInventorySlot(GameItem item)
	{
		if (!(item is GameCharacterItem))
		{
			return false;
		}
		if (item is GameCharacterItem gameCharacterItem)
		{
			if (gameCharacterItem == null)
			{
				return false;
			}
			if (gameCharacterItem.health <= 0)
			{
				return false;
			}
		}
		if (item == null)
		{
			return false;
		}
		if (GraphUtils.FindParentEquals(this, item))
		{
			return false;
		}
		if (!(item is GameCharacterItem))
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

	public override SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		if (item == null)
		{
			return null;
		}
		if (!MayHaveValidInventorySlot(item))
		{
			return null;
		}
		return new SlotMarker
		{
			item = item,
			inventory = this,
			itemGridShape = item.modifiedShape,
			numTransfer = 1
		};
	}
}

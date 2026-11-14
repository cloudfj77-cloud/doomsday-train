using System;
using System.Collections.Generic;
using UnityEngine;

public class GridPixelElement : PixelElement, MachineryStorageOwner, GraphNodeStorage
{
	private List<GraphNodeStorage> linkedContainerStorage = new List<GraphNodeStorage>();

	public bool searchUsingPriority = true;

	private readonly TreeNodeRender[] childInners;

	private readonly PixelElement[] childElements;

	private readonly (int, int)[] childMinimumSize;

	public readonly int gridWidth;

	public readonly int gridHeight;

	public bool isCentered;

	public bool justifyWidth = true;

	private int _xSpacing = -1;

	private int _ySpacing = -1;

	private bool validateLock;

	public List<GraphNodeStorage> parents
	{
		get
		{
			List<GraphNodeStorage> list = new List<GraphNodeStorage>();
			GraphNodeStorage graphNodeStorage = parent;
			if (graphNodeStorage != null)
			{
				list.Add(graphNodeStorage);
			}
			return list;
		}
	}

	public List<GraphNodeStorage> children
	{
		get
		{
			List<GraphNodeStorage> nodes = new List<GraphNodeStorage>();
			Array.ForEach(childElements, delegate(PixelElement c)
			{
				if (c is GraphNodeStorage item)
				{
					nodes.Add(item);
				}
				else if (c is MachineryContainer machineryContainer)
				{
					machineryContainer.CollectStorage(nodes);
				}
			});
			return nodes;
		}
	}

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	public GraphNodeStorage parent { get; set; }

	public void RelinkStorage()
	{
		List<GraphNodeStorage> current = new List<GraphNodeStorage>();
		Array.ForEach(childElements, delegate(PixelElement c)
		{
			if (c is MachineryContainer machineryContainer)
			{
				machineryContainer.CollectStorage(current);
			}
		});
		MachineryContainerUtils.Relink(this, linkedContainerStorage, current);
		linkedContainerStorage = current;
	}

	public GridPixelElement SetSpacing(int x, int y)
	{
		_xSpacing = x;
		_ySpacing = y;
		Validate();
		return this;
	}

	public GridPixelElement(int gridWidth, int gridHeight, bool isCentered = false)
	{
		handler = TreeNodeRender.Instantiate(this);
		inner = TreeNodeRender.Instantiate().SetParent(handler);
		this.gridWidth = gridWidth;
		this.gridHeight = gridHeight;
		this.isCentered = isCentered;
		childInners = new TreeNodeRender[gridWidth * gridHeight];
		childElements = new PixelElement[gridWidth * gridHeight];
		childMinimumSize = new(int, int)[gridWidth * gridHeight];
		for (int i = 0; i < childInners.Length; i++)
		{
			childInners[i] = TreeNodeRender.Instantiate().SetParent(inner);
		}
		Validate();
	}

	public void Validate()
	{
		if (validateLock)
		{
			return;
		}
		validateLock = true;
		_ = UISettings.current.gridSize;
		PixelElement[] array = childElements;
		for (int i = 0; i < array.Length; i++)
		{
			MachineryContainerUtils.Refresh(array[i]);
		}
		int[] array2 = new int[gridWidth];
		int[] array3 = new int[gridHeight];
		for (int j = 0; j < gridWidth; j++)
		{
			array2[j] = GetColumWidth(j);
		}
		for (int k = 0; k < gridHeight; k++)
		{
			array3[k] = GetRowHeight(k);
		}
		bool[] array4 = new bool[gridWidth];
		bool[] array5 = new bool[gridHeight];
		for (int l = 0; l < gridWidth; l++)
		{
			array4[l] = GetColumPad(l);
		}
		for (int m = 0; m < gridHeight; m++)
		{
			array5[m] = GetRowPad(m);
		}
		int num = ((_xSpacing >= 0) ? _xSpacing : (handler.FindParentWindow()?.xPadding ?? 0));
		int num2 = ((_ySpacing >= 0) ? _ySpacing : (handler.FindParentWindow()?.yPadding ?? 0));
		int num3 = 0;
		int val = 0;
		for (int n = 0; n < gridHeight; n++)
		{
			int num4 = 0;
			for (int num5 = 0; num5 < gridWidth; num5++)
			{
				int num6 = n * gridWidth + num5;
				bool flag = IsChildHidden(num6);
				int num7 = (justifyWidth ? array2[num5] : ((!flag) ? Math.Max(childElements[num6]?.widthPixels ?? 0, childMinimumSize[num6].Item1) : 0));
				if (isCentered)
				{
					int num8 = childElements[num6]?.widthPixels ?? 0;
					int num9 = childElements[num6]?.heightPixels ?? 0;
					int num10 = (num7 - num8) / 2;
					int num11 = (array3[n] - num9) / 2;
					childInners[num6].rectTransform.anchoredPosition = new Vector2(num4 + num10, -num3 - num11);
					childInners[num6].rectTransform.sizeDelta = new Vector2(num8, num9);
				}
				else
				{
					childInners[num6].rectTransform.anchoredPosition = new Vector2(num4, -num3);
					childInners[num6].rectTransform.sizeDelta = new Vector2(num7, array3[n]);
				}
				num4 += num7 + (array4[num5] ? num : 0);
			}
			num3 += array3[n] + (array5[n] ? num2 : 0);
			val = Math.Max(num4, val);
		}
		widthPixels = val;
		heightPixels = num3;
		handler.parent?.FindParentElement<GraphNodeStorage>()?.Validate();
		validateLock = false;
	}

	private bool IsChildHidden(int i)
	{
		if (childElements[i] == null)
		{
			return true;
		}
		if (childElements[i] is ButtonElement { isHidden: not false })
		{
			return true;
		}
		if (childElements[i].handler != null && !childElements[i].handler.gameObject.activeSelf)
		{
			return true;
		}
		return false;
	}

	private int GetColumWidth(int x)
	{
		if (x < 0 || x >= gridWidth)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < gridHeight; i++)
		{
			int num2 = i * gridWidth + x;
			if (!IsChildHidden(num2))
			{
				int val = Math.Max(childMinimumSize[num2].Item1, childElements[num2].widthPixels);
				num = Math.Max(num, val);
			}
		}
		return num;
	}

	private int GetRowHeight(int y)
	{
		if (y < 0 || y >= gridHeight)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < gridWidth; i++)
		{
			int num2 = y * gridWidth + i;
			if (!IsChildHidden(num2))
			{
				int val = Math.Max(childMinimumSize[num2].Item2, childElements[num2].heightPixels);
				num = Math.Max(num, val);
			}
		}
		return num;
	}

	private bool GetColumPad(int x)
	{
		if (x < 0 || x >= gridWidth - 1)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < gridHeight; i++)
		{
			int i2 = i * gridWidth + x;
			if (!IsChildHidden(i2))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private bool GetRowPad(int y)
	{
		if (y < 0 || y >= gridHeight - 1)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < gridWidth; i++)
		{
			int i2 = y * gridWidth + i;
			if (!IsChildHidden(i2))
			{
				result = true;
				break;
			}
		}
		return result;
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

	public bool Attach(PixelElement element)
	{
		return Attach(element, 0, 0);
	}

	public bool Attach(PixelElement element, int minimumWidth = 0, int minimumHeight = 0)
	{
		for (int i = 0; i < gridHeight; i++)
		{
			for (int j = 0; j < gridWidth; j++)
			{
				if (Attach(element, j, i, minimumWidth, minimumHeight))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool AttachPos(PixelElement element, int x, int y)
	{
		return Attach(element, x, y, 0, 0);
	}

	public bool Attach(PixelElement element, int x, int y, int minimumWidth = 0, int minimumHeight = 0)
	{
		int num = y * gridWidth + x;
		if (num < 0 || num >= childElements.Length)
		{
			return false;
		}
		if (childElements[num] != null)
		{
			return false;
		}
		if (element is GraphNodeStorage graphNodeStorage && !graphNodeStorage.AddParent(this))
		{
			return false;
		}
		childElements[num] = element;
		childMinimumSize[num] = (minimumWidth, minimumHeight);
		element?.handler.SetParent(childInners[num]);
		if ((object)element?.handler != null)
		{
			element.handler.rectTransform.anchoredPosition = Vector2.zero;
		}
		if (element is MachineryContainer machineryContainer)
		{
			machineryContainer.owner = this;
			RelinkStorage();
		}
		Validate();
		return true;
	}

	public bool Detach(PixelElement element)
	{
		int num = Array.FindIndex(childElements, (PixelElement c) => c == element);
		if (num < 0 || num >= childElements.Length)
		{
			return false;
		}
		if (element is GraphNodeStorage graphNodeStorage && !graphNodeStorage.RemoveParent(this))
		{
			return false;
		}
		childElements[num] = null;
		element?.handler.ResetParent();
		if (element is MachineryContainer machineryContainer)
		{
			RelinkStorage();
			machineryContainer.owner = null;
		}
		Validate();
		return true;
	}

	public void DetachAll()
	{
		PixelElement[] array = childElements;
		foreach (PixelElement element in array)
		{
			Detach(element);
		}
	}

	public PixelElement GetElement(int x, int y)
	{
		int num = y * gridWidth + x;
		return childElements[num];
	}

	public bool AddParent(GraphNodeStorage node)
	{
		if (parent != null)
		{
			return false;
		}
		parent = node;
		return true;
	}

	public bool RemoveParent(GraphNodeStorage node)
	{
		if (parent != node)
		{
			return false;
		}
		parent = null;
		return true;
	}

	public bool MayHaveValidInventorySlot(GameItem item)
	{
		foreach (GraphNodeStorage child in children)
		{
			if (child != null && child.MayHaveValidInventorySlot(item))
			{
				return true;
			}
		}
		return false;
	}

	public SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		if (!searchUsingPriority)
		{
			for (int i = 0; i < children.Count; i++)
			{
				int index = children.Count - 1 - i;
				SlotMarker slotMarker = children[index]?.TryFindOneValidInventorySlot(item);
				if (slotMarker != null && slotMarker.numTransfer > 0)
				{
					return slotMarker;
				}
			}
			return null;
		}
		SlotMarker slotMarker2 = null;
		foreach (GraphNodeStorage child in children)
		{
			SlotMarker slotMarker3 = child?.TryFindOneValidInventorySlot(item);
			if (slotMarker3 != null && slotMarker3.numTransfer > 0 && slotMarker3.priority >= (slotMarker2?.priority ?? int.MinValue))
			{
				slotMarker2 = slotMarker3;
			}
		}
		return slotMarker2;
	}
}

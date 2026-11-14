using System;
using System.Collections.Generic;
using UnityEngine;

public class StealPixelElement : PixelElement, GraphNodeStorage
{
	private GraphNodeStorage parent;

	private PixelElement stolenUI;

	private PixelElement stolenUIParentElement;

	public bool centered;

	private bool validateLock;

	public List<GraphNodeStorage> parents
	{
		get
		{
			List<GraphNodeStorage> list = new List<GraphNodeStorage>();
			if (parent != null)
			{
				list.Add(parent);
			}
			return list;
		}
	}

	public List<GraphNodeStorage> children
	{
		get
		{
			List<GraphNodeStorage> list = new List<GraphNodeStorage>();
			if (stolenUI is GraphNodeStorage item)
			{
				list.Add(item);
			}
			return list;
		}
	}

	public bool canResize => true;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	public StealPixelElement(bool centered = true)
	{
		this.centered = centered;
		handler = TreeNodeRender.Instantiate(this);
		inner = TreeNodeRender.Instantiate().SetParent(handler);
	}

	public int WidthPixelsClamp(int width)
	{
		return Math.Max(width, stolenUI?.widthPixels ?? 0);
	}

	public int HeightPixelsClamp(int height)
	{
		return Math.Max(height, stolenUI?.heightPixels ?? 0);
	}

	public void ResizePixels(int width, int height)
	{
		widthPixels = width;
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

	public void Validate()
	{
		if (validateLock)
		{
			return;
		}
		validateLock = true;
		_ = UISettings.current.gridSize;
		widthPixels = WidthPixelsClamp(widthPixels);
		heightPixels = HeightPixelsClamp(heightPixels);
		if (stolenUI != null)
		{
			stolenUI.Validate();
			widthPixels = WidthPixelsClamp(widthPixels);
			heightPixels = HeightPixelsClamp(heightPixels);
			int num = widthPixels - stolenUI.widthPixels;
			int num2 = heightPixels - stolenUI.heightPixels;
			if (centered)
			{
				inner.rectTransform.anchoredPosition = new Vector2(num / 2, -(num2 / 2));
			}
			else
			{
				inner.rectTransform.anchoredPosition = new Vector2(0f, 0f);
			}
		}
		validateLock = false;
	}

	public bool StealItem(GameItem item)
	{
		if (item?.child?.child is PixelElement element)
		{
			return Attach(element);
		}
		return false;
	}

	public bool UnStealItem()
	{
		if (stolenUI != null)
		{
			return Detach(stolenUI);
		}
		return false;
	}

	public bool UnStealItem(GameItem item)
	{
		if (item?.child?.child is PixelElement element)
		{
			return Detach(element);
		}
		return false;
	}

	public bool Attach(PixelElement element)
	{
		if (stolenUI != null)
		{
			Detach(stolenUI);
		}
		if (element == null)
		{
			return false;
		}
		stolenUI = element;
		stolenUIParentElement = ((element is GraphNodeStorage graphNodeStorage && graphNodeStorage.parents.Count > 0) ? GraphUtils.FindParentType<PixelElement>(graphNodeStorage) : null);
		stolenUI.SetRenderingParent(this);
		if (stolenUIParentElement is PixelWindow pixelWindow)
		{
			pixelWindow.Lock();
		}
		return true;
	}

	public bool Detach(PixelElement element)
	{
		if (stolenUI == null)
		{
			return false;
		}
		if (stolenUI != element)
		{
			return false;
		}
		if (stolenUIParentElement != null)
		{
			stolenUI.SetRenderingParent(stolenUIParentElement);
		}
		else
		{
			stolenUI.handler.ResetParent();
		}
		if (stolenUIParentElement is PixelWindow pixelWindow)
		{
			pixelWindow.Unlock();
		}
		stolenUI = null;
		stolenUIParentElement = null;
		return true;
	}

	public bool MayHaveValidInventorySlot(GameItem item)
	{
		if (!(stolenUI is GraphNodeStorage graphNodeStorage))
		{
			return false;
		}
		return graphNodeStorage.MayHaveValidInventorySlot(item);
	}

	public SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		if (!(stolenUI is GraphNodeStorage graphNodeStorage))
		{
			return null;
		}
		return graphNodeStorage.TryFindOneValidInventorySlot(item);
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
}

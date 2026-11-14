using System.Collections.Generic;
using UnityEngine;

public class ScrollElement : PixelElement, GraphNodeStorage
{
	private readonly TreeNodeRender scrollBar;

	private readonly TreeNodeRender scrollButton;

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
			List<GraphNodeStorage> list = new List<GraphNodeStorage>();
			if (childElement is GraphNodeStorage item)
			{
				list.Add(item);
			}
			return list;
		}
	}

	public bool canResize
	{
		get
		{
			if (childElement == null)
			{
				return false;
			}
			return childElement.canResize;
		}
	}

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	public PixelElement childElement { get; private set; }

	public GraphNodeStorage parent { get; set; }

	public ScrollElement()
	{
		handler = TreeNodeRender.Instantiate(this);
		inner = TreeNodeRender.Instantiate().SetParent(handler);
		scrollBar = TreeNodeRender.Instantiate().SetParent(handler);
		scrollButton = TreeNodeRender.Instantiate().SetParent(handler);
		scrollBar.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_right_scrollbar");
		scrollBar.image.SetNativeSize();
		scrollButton.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_right_scrollbar_button");
		scrollButton.image.SetNativeSize();
		scrollBar.image.enabled = true;
		scrollButton.image.enabled = true;
		inner.mask.enabled = true;
		Validate();
	}

	public void Validate()
	{
		if (!validateLock)
		{
			validateLock = true;
			if (childElement != null)
			{
				_ = 1;
				inner.rectTransform.sizeDelta = new Vector2(widthPixels - 20, heightPixels);
			}
			handler.parent?.FindParentElement<GraphNodeStorage>()?.Validate();
			validateLock = false;
		}
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
		widthPixels = WidthPixelsClamp(width);
		heightPixels = HeightPixelsClamp(height);
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
		if (childElement != null)
		{
			Detach(childElement);
		}
		if (element is GraphNodeStorage graphNodeStorage)
		{
			graphNodeStorage.AddParent(this);
		}
		childElement = element;
		element?.SetRenderingParent(this);
		widthPixels = childElement?.widthPixels ?? 0;
		heightPixels = childElement?.heightPixels ?? 0;
		Validate();
		return true;
	}

	public bool Detach(PixelElement element)
	{
		if (childElement != element)
		{
			return false;
		}
		if (element is GraphNodeStorage graphNodeStorage)
		{
			graphNodeStorage.RemoveParent(this);
		}
		childElement = null;
		element?.RemoveRenderingParent(this);
		Validate();
		return true;
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
		if (childElement is GraphNodeStorage graphNodeStorage)
		{
			return graphNodeStorage.MayHaveValidInventorySlot(item);
		}
		return false;
	}

	public SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		if (childElement is GraphNodeStorage graphNodeStorage)
		{
			return graphNodeStorage.TryFindOneValidInventorySlot(item);
		}
		return null;
	}
}

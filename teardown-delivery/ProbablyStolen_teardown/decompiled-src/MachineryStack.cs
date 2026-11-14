using System.Collections.Generic;
using UnityEngine;

public class MachineryStack : PixelElement, MachineryContainer
{
	public enum Align
	{
		Start,
		Center,
		End
	}

	public readonly bool vertical;

	public int gap;

	public Align align = Align.Center;

	public Align mainAlign = Align.Center;

	private readonly List<PixelElement> children = new List<PixelElement>();

	private PixelElement stretch;

	private int requestedWidth;

	private int requestedHeight;

	private bool validateLock;

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	public MachineryStorageOwner owner { get; set; }

	public IReadOnlyList<PixelElement> elements => children;

	public MachineryStack(bool vertical = true, int gap = 4)
	{
		this.vertical = vertical;
		this.gap = gap;
		handler = TreeNodeRender.Instantiate(this, vertical ? "MachineryStack (vertical)" : "MachineryStack (horizontal)");
		inner = TreeNodeRender.Instantiate().SetParent(handler);
		inner.image.enabled = false;
		Validate();
	}

	public static MachineryStack Vertical(params PixelElement[] elements)
	{
		MachineryStack machineryStack = new MachineryStack();
		foreach (PixelElement element in elements)
		{
			machineryStack.Add(element);
		}
		return machineryStack;
	}

	public static MachineryStack Horizontal(params PixelElement[] elements)
	{
		MachineryStack machineryStack = new MachineryStack(vertical: false);
		foreach (PixelElement element in elements)
		{
			machineryStack.Add(element);
		}
		return machineryStack;
	}

	public MachineryStack SetAlign(Align align)
	{
		this.align = align;
		Validate();
		return this;
	}

	public MachineryStack SetMainAlign(Align align)
	{
		mainAlign = align;
		Validate();
		return this;
	}

	public MachineryStack SetStretch(PixelElement element)
	{
		stretch = element;
		Validate();
		return this;
	}

	public MachineryStack Add(PixelElement element)
	{
		Attach(element);
		return this;
	}

	public bool Attach(PixelElement element)
	{
		if (element == null || children.Contains(element))
		{
			return false;
		}
		children.Add(element);
		element.SetRenderingParent(this);
		element.handler.rectTransform.anchoredPosition = Vector2.zero;
		MachineryContainerUtils.SetOwner(element, owner);
		owner?.RelinkStorage();
		Validate();
		return true;
	}

	public bool Detach(PixelElement element)
	{
		if (element == null || !children.Remove(element))
		{
			return false;
		}
		element.RemoveRenderingParent(this);
		if (stretch == element)
		{
			stretch = null;
		}
		owner?.RelinkStorage();
		Validate();
		return true;
	}

	public void CollectStorage(List<GraphNodeStorage> list)
	{
		foreach (PixelElement child in children)
		{
			MachineryContainerUtils.Collect(child, list);
		}
	}

	private PixelElement StretchTarget()
	{
		if (stretch != null)
		{
			return stretch;
		}
		foreach (PixelElement child in children)
		{
			if (child is MachineryIntake && !MachineryContainerUtils.IsHidden(child))
			{
				return child;
			}
		}
		return null;
	}

	public void Validate()
	{
		if (validateLock)
		{
			return;
		}
		validateLock = true;
		List<PixelElement> list = children.FindAll((PixelElement c) => !MachineryContainerUtils.IsHidden(c));
		foreach (PixelElement item in list)
		{
			MachineryContainerUtils.Refresh(item);
		}
		int num = 0;
		int a = 0;
		foreach (PixelElement item2 in list)
		{
			num += (vertical ? item2.heightPixels : item2.widthPixels);
			a = Mathf.Max(a, vertical ? item2.widthPixels : item2.heightPixels);
		}
		if (list.Count > 1)
		{
			num += gap * (list.Count - 1);
		}
		int b = (vertical ? requestedHeight : requestedWidth);
		int b2 = (vertical ? requestedWidth : requestedHeight);
		int num2 = Mathf.Max(num, b);
		int num3 = Mathf.Max(a, b2);
		PixelElement pixelElement = StretchTarget();
		int num4 = num2 - num;
		foreach (PixelElement item3 in list)
		{
			bool num5 = item3 is MachineryContainer || (item3 is MachineryWidget { fillWidth: not false } && vertical);
			int num6 = (vertical ? item3.heightPixels : item3.widthPixels) + ((item3 == pixelElement) ? num4 : 0);
			int num7 = (num5 ? num3 : (vertical ? item3.widthPixels : item3.heightPixels));
			if (num5 || item3 == pixelElement)
			{
				if (vertical)
				{
					MachineryContainerUtils.TryResize(item3, num7, num6);
				}
				else
				{
					MachineryContainerUtils.TryResize(item3, num6, num7);
				}
			}
		}
		int num8 = 0;
		if (pixelElement == null && num4 > 0)
		{
			num8 = ((mainAlign == Align.Center) ? (num4 / 2) : ((mainAlign == Align.End) ? num4 : 0));
		}
		foreach (PixelElement item4 in list)
		{
			int num9 = (vertical ? item4.heightPixels : item4.widthPixels);
			int num10 = (vertical ? item4.widthPixels : item4.heightPixels);
			int num11 = ((align != 0) ? ((align == Align.End) ? (num3 - num10) : ((num3 - num10) / 2)) : 0);
			item4.handler.rectTransform.anchoredPosition = (vertical ? new Vector2(num11, -num8) : new Vector2(num8, -num11));
			num8 += num9 + gap;
		}
		widthPixels = (vertical ? num3 : num2);
		heightPixels = (vertical ? num2 : num3);
		MachineryStyle.Place(inner, 0, 0, widthPixels, heightPixels);
		validateLock = false;
	}

	public void ResizePixels(int width, int height)
	{
		requestedWidth = width;
		requestedHeight = height;
		Validate();
	}

	public int WidthPixelsClamp(int width)
	{
		return widthPixels;
	}

	public int HeightPixelsClamp(int height)
	{
		return heightPixels;
	}

	public int XClamp(int x)
	{
		return 0;
	}

	public int YClamp(int y)
	{
		return 0;
	}
}

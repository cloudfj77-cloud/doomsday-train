using System;
using UnityEngine;

public class EmptyPixelElement : PixelElement
{
	public bool canResize { get; }

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner => handler;

	public EmptyPixelElement()
		: this(0, 0, canResize: true)
	{
	}

	public EmptyPixelElement(int width, int height, bool canResize = false)
	{
		widthPixels = width;
		heightPixels = height;
		this.canResize = canResize;
		handler = TreeNodeRender.Instantiate(this);
	}

	public int WidthPixelsClamp(int width)
	{
		if (canResize)
		{
			return Math.Max(width, 0);
		}
		return widthPixels;
	}

	public int HeightPixelsClamp(int height)
	{
		if (canResize)
		{
			return Math.Max(height, 0);
		}
		return heightPixels;
	}

	public void ResizePixels(int width, int height)
	{
		widthPixels = WidthPixelsClamp(width);
		heightPixels = HeightPixelsClamp(height);
		Validate();
	}

	public void Validate()
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
		throw new NotImplementedException();
	}

	public bool Detach(PixelElement element)
	{
		throw new NotImplementedException();
	}
}

using System;
using UnityEngine;

public class ColoredPixelElement : PixelElement
{
	private readonly TreeNodeRender background;

	private readonly TreeNodeRender insideBorder;

	private int maxWidth;

	private int maxHeight;

	private int minWidth;

	private int minHeight;

	public bool canResize { get; }

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner => handler;

	public ColoredPixelElement(int minWidth, int minHeight, int maxWidth, int maxHeight, bool canResize)
	{
		widthPixels = minWidth;
		heightPixels = minHeight;
		this.canResize = canResize;
		handler = TreeNodeRender.Instantiate(this);
		this.maxWidth = maxWidth;
		this.maxHeight = maxHeight;
		this.minWidth = minWidth;
		this.minHeight = minHeight;
		background = TreeNodeRender.Instantiate().SetParent(handler);
		insideBorder = TreeNodeRender.Instantiate().SetParent(handler);
		background.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_disabled_orange");
		background.image.SetNativeSize();
		background.InitEventHandler();
		background.eventHandler.mouseDownFunc.Add(delegate
		{
			background.FindParentWindow()?.ToFront();
			return true;
		});
		insideBorder.image.sprite = RenderHandler.LoadFromAtlas("Window/border", "inside");
		insideBorder.image.SetNativeSize();
		background.image.enabled = true;
		insideBorder.image.enabled = true;
		Validate();
	}

	public int WidthPixelsClamp(int width)
	{
		if (canResize)
		{
			return Math.Clamp(width, minWidth, maxWidth);
		}
		return widthPixels;
	}

	public int HeightPixelsClamp(int height)
	{
		if (canResize)
		{
			return Math.Clamp(height, minHeight, maxHeight);
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
		background.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		insideBorder.rectTransform.sizeDelta = new Vector2(widthPixels + 2, heightPixels + 2);
		insideBorder.rectTransform.anchoredPosition = new Vector2(-1f, 1f);
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

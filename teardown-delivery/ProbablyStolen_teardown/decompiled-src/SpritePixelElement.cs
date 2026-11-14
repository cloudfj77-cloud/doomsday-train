using System;
using UnityEngine;

public class SpritePixelElement : PixelElement
{
	public readonly TreeNodeRender background;

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner => handler;

	public SpritePixelElement(string atlasPath, string spriteName)
	{
		handler = TreeNodeRender.Instantiate(this);
		background = TreeNodeRender.Instantiate().SetParent(handler);
		SetSprite(atlasPath, spriteName);
		background.image.enabled = true;
		Validate();
	}

	public SpritePixelElement(string spritePath)
	{
		handler = TreeNodeRender.Instantiate(this);
		background = TreeNodeRender.Instantiate().SetParent(handler);
		SetSprite(spritePath);
		background.image.enabled = true;
		Validate();
	}

	public void Hide()
	{
		handler.gameObject.SetActive(value: false);
	}

	public void Show()
	{
		handler.gameObject.SetActive(value: true);
	}

	public void ToFront()
	{
		handler.rectTransform.SetAsLastSibling();
	}

	public void ToBack()
	{
		handler.rectTransform.SetAsFirstSibling();
	}

	public void ResizePixels(int width, int height)
	{
		Validate();
	}

	public void SetSprite(string atlasPath, string spriteName)
	{
		background.image.sprite = RenderHandler.LoadFromAtlas(atlasPath, spriteName);
		background.image.SetNativeSize();
	}

	public void SetSprite(string spritePath)
	{
		background.image.sprite = RenderHandler.LoadFromFile(spritePath);
		background.image.SetNativeSize();
	}

	public void SetSprite(Sprite sprite)
	{
		if ((object)sprite != null)
		{
			background.image.sprite = sprite;
			background.image.SetNativeSize();
		}
	}

	public void Validate()
	{
		background.image.SetNativeSize();
		widthPixels = (int)background.rectTransform.sizeDelta.x;
		heightPixels = (int)background.rectTransform.sizeDelta.y;
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

	public int WidthPixelsClamp(int width)
	{
		return widthPixels;
	}

	public int HeightPixelsClamp(int height)
	{
		return heightPixels;
	}
}

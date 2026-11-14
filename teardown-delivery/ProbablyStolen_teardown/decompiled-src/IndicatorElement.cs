using System;
using System.Linq;
using UnityEngine;

public class IndicatorElement : PixelElement
{
	public enum IndicatorColor
	{
		None,
		Red,
		Orange,
		Yellow,
		Green,
		Cyan,
		Purple
	}

	public readonly bool canMove;

	private readonly TreeNodeRender frameNode;

	private readonly TreeNodeRender node;

	private int forceWidth;

	private int forceHeight;

	private IndicatorColor currentColor;

	private IndicatorColor currentTextColor;

	private bool isLight;

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner => handler;

	public string text { get; private set; }

	public bool hasText
	{
		get
		{
			if (text != null)
			{
				return text.Count() > 0;
			}
			return false;
		}
	}

	public static string ColorString(IndicatorColor currentColor, bool isLight)
	{
		string text = (isLight ? "light" : "dark");
		switch (currentColor)
		{
		case IndicatorColor.Red:
			text += "_red";
			break;
		case IndicatorColor.Orange:
			text += "_orange";
			break;
		case IndicatorColor.Yellow:
			text += "_yellow";
			break;
		case IndicatorColor.Green:
			text += "_green";
			break;
		case IndicatorColor.Cyan:
			text += "_cyan";
			break;
		case IndicatorColor.Purple:
			text += "_purple";
			break;
		}
		return text;
	}

	public static RenderHandler.ColorPalette ColorText(IndicatorColor currentTextColor, bool isLight)
	{
		switch (currentTextColor)
		{
		case IndicatorColor.Red:
			if (!isLight)
			{
				return RenderHandler.ColorPalette.Red;
			}
			return RenderHandler.ColorPalette.VeryDarkRed;
		case IndicatorColor.Orange:
			if (!isLight)
			{
				return RenderHandler.ColorPalette.Orange;
			}
			return RenderHandler.ColorPalette.VeryDarkRed;
		case IndicatorColor.Yellow:
			if (!isLight)
			{
				return RenderHandler.ColorPalette.Yellow;
			}
			return RenderHandler.ColorPalette.VeryDarkGreen;
		case IndicatorColor.Green:
			if (!isLight)
			{
				return RenderHandler.ColorPalette.Green;
			}
			return RenderHandler.ColorPalette.VeryDarkGreen;
		case IndicatorColor.Cyan:
			if (!isLight)
			{
				return RenderHandler.ColorPalette.Cyan;
			}
			return RenderHandler.ColorPalette.VeryDarkCyan;
		case IndicatorColor.Purple:
			if (!isLight)
			{
				return RenderHandler.ColorPalette.Purple;
			}
			return RenderHandler.ColorPalette.VeryDarkRed;
		default:
			if (!isLight)
			{
				return RenderHandler.ColorPalette.LightGray;
			}
			return RenderHandler.ColorPalette.VeryDarkGray;
		}
	}

	public IndicatorElement()
	{
		handler = TreeNodeRender.Instantiate(this, "PushButton");
		node = TreeNodeRender.Instantiate().SetParent(handler);
		frameNode = TreeNodeRender.Instantiate().SetParent(handler);
		forceWidth = 10;
		forceHeight = 10;
		node.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_inner", ColorString(currentColor, isLight));
		frameNode.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "indicator_frame");
		node.image.enabled = true;
		frameNode.image.enabled = true;
		SetColor(IndicatorColor.None);
		Validate();
	}

	public IndicatorColor GetColor()
	{
		return currentColor;
	}

	public IndicatorColor GetTextColor()
	{
		return currentTextColor;
	}

	public IndicatorElement SetColor(IndicatorColor color)
	{
		if (color != currentColor)
		{
			currentColor = color;
			ValidateSprite();
		}
		return this;
	}

	public bool IsLight()
	{
		return isLight;
	}

	public bool IsDark()
	{
		return !isLight;
	}

	public IndicatorElement Light()
	{
		if (!isLight)
		{
			isLight = true;
			ValidateSprite();
		}
		return this;
	}

	public IndicatorElement Dark()
	{
		if (isLight)
		{
			isLight = false;
			ValidateSprite();
		}
		return this;
	}

	public IndicatorElement ToggleLight()
	{
		if (isLight)
		{
			Dark();
		}
		else
		{
			Light();
		}
		return this;
	}

	private void ValidateSprite()
	{
		node.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_inner", ColorString(currentColor, isLight));
	}

	public void Validate()
	{
		ValidateSprite();
		widthPixels = forceWidth;
		heightPixels = forceHeight;
		node.rectTransform.sizeDelta = new Vector2(widthPixels - 4, heightPixels - 4);
		frameNode.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		node.rectTransform.anchoredPosition = new Vector2(2f, -2f);
	}

	public void ResizePixels(int width, int height)
	{
	}

	public bool Attach(PixelElement element)
	{
		throw new NotImplementedException();
	}

	public bool Detach(PixelElement element)
	{
		throw new NotImplementedException();
	}

	public int XClamp(int x)
	{
		throw new NotImplementedException();
	}

	public int YClamp(int y)
	{
		throw new NotImplementedException();
	}

	public int WidthPixelsClamp(int width)
	{
		throw new NotImplementedException();
	}

	public int HeightPixelsClamp(int height)
	{
		throw new NotImplementedException();
	}
}

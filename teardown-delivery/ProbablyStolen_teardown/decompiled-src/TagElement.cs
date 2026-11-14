using System;
using TMPro;
using UnityEngine;

public class TagElement : PixelElement
{
	protected readonly TreeNodeRender textNode;

	protected readonly TreeNodeRender textBackgroundNode;

	protected readonly TreeNodeRender textUnderlineNode;

	protected readonly TreeNodeRender textStrikethroughNode;

	protected readonly TreeNodeRender background;

	private int leftPadding = 2;

	private int rightPadding = 3;

	private int topPadding = 2;

	private int fixedWidth;

	public virtual bool canResize { get; }

	public int widthPixels { get; protected set; }

	public int heightPixels { get; protected set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner => handler;

	public int maxCharWidth { get; private set; }

	public TagElement(int fixedWidth = -1, bool hasRaycast = true)
	{
		maxCharWidth = 10000;
		this.fixedWidth = fixedWidth;
		handler = TreeNodeRender.Instantiate(this);
		background = TreeNodeRender.Instantiate().SetParent(handler);
		textBackgroundNode = TreeNodeRender.InstantiateText().SetParent(background);
		textNode = TreeNodeRender.InstantiateText().SetParent(background);
		textUnderlineNode = TreeNodeRender.InstantiateText().SetParent(background);
		textStrikethroughNode = TreeNodeRender.InstantiateText().SetParent(background);
		background.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "text_background_in");
		background.image.SetNativeSize();
		if (hasRaycast)
		{
			background.InitEventHandler();
		}
		background.image.enabled = true;
		background.mask.enabled = true;
		InitTextNode(textBackgroundNode);
		InitTextNode(textNode);
		InitTextNode(textUnderlineNode);
		InitTextNode(textStrikethroughNode);
		textNode.text.horizontalAlignment = HorizontalAlignmentOptions.Center;
		textNode.text.verticalAlignment = VerticalAlignmentOptions.Middle;
		widthPixels = WidthPixelsClamp(0);
		heightPixels = UISettings.current.fontHeight;
		heightPixels = HeightPixelsClamp(0);
		Validate();
	}

	private void InitTextNode(TreeNodeRender textNode)
	{
		textNode.text.text = "";
		textNode.text.color = RenderHandler.current.textColorWhite;
		textNode.text.fontStyle = FontStyles.Normal;
		textNode.text.rectTransform.sizeDelta = new Vector2(UISettings.current.fontWidth, UISettings.current.fontHeight);
		textNode.text.enabled = true;
		textNode.text.overflowMode = TextOverflowModes.Overflow;
		textNode.text.enableWordWrapping = false;
	}

	public virtual int WidthPixelsClamp(int width)
	{
		if (fixedWidth >= 0)
		{
			return fixedWidth;
		}
		textNode.text.rectTransform.sizeDelta = new Vector2(widthPixels, 1f);
		float num = textNode.text.preferredWidth;
		if (num <= 1f)
		{
			num = widthPixels;
		}
		return Math.Min(UISettings.current.effectiveFontWidth * maxCharWidth, (int)num) + leftPadding + rightPadding;
	}

	public virtual int HeightPixelsClamp(int height)
	{
		textNode.text.rectTransform.sizeDelta = new Vector2(widthPixels, 1f);
		float num = textNode.text.preferredHeight;
		if (num <= 1f)
		{
			num = heightPixels;
		}
		if (canResize)
		{
			return Math.Max(height, (int)num) + topPadding;
		}
		return (int)num + topPadding;
	}

	public virtual void ResizePixels(int width, int height)
	{
		widthPixels = WidthPixelsClamp(width);
		heightPixels = HeightPixelsClamp(height);
		Validate();
	}

	public TagElement SetRichText(RichText richText, int maxWidth = 10000)
	{
		if (richText == null)
		{
			return this;
		}
		if (fixedWidth >= 0)
		{
			maxWidth = Math.Min(maxWidth, fixedWidth / UISettings.current.gridSize);
		}
		textBackgroundNode.text.enabled = true;
		textUnderlineNode.text.enabled = true;
		textStrikethroughNode.text.enabled = true;
		textNode.text.enabled = true;
		textNode.text.overflowMode = TextOverflowModes.Overflow;
		textNode.text.enableWordWrapping = false;
		textNode.text.horizontalAlignment = HorizontalAlignmentOptions.Center;
		textNode.text.verticalAlignment = VerticalAlignmentOptions.Middle;
		textNode.text.text = richText.richTextForeground;
		textBackgroundNode.text.text = richText.richTextBackground;
		textUnderlineNode.text.text = richText.richTextUnderline;
		textStrikethroughNode.text.text = richText.richTextStrikethrough;
		maxCharWidth = richText.recommendedWidth;
		if (maxCharWidth > maxWidth)
		{
			maxCharWidth = maxWidth;
		}
		ResizePixels(widthPixels, heightPixels);
		return this;
	}

	public TagElement SetText(string text, int maxWidth = 10000, RenderHandler.ColorPalette color = RenderHandler.ColorPalette.VeryLightRed)
	{
		textBackgroundNode.text.enabled = false;
		textUnderlineNode.text.enabled = false;
		textStrikethroughNode.text.enabled = false;
		textNode.text.enabled = true;
		textNode.text.text = text;
		textNode.text.overflowMode = TextOverflowModes.Overflow;
		textNode.text.enableWordWrapping = false;
		textNode.text.color = RenderHandler.GetColorFromPalette(color);
		maxCharWidth = maxWidth;
		ResizePixels(widthPixels, heightPixels);
		return this;
	}

	public string GetText()
	{
		return textNode.text.text;
	}

	public void Validate()
	{
		textNode.rectTransform.sizeDelta = new Vector2(widthPixels - 2 * leftPadding, heightPixels - 2 * topPadding);
		textNode.rectTransform.anchoredPosition = new Vector2(leftPadding, -topPadding);
		textBackgroundNode.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		textBackgroundNode.rectTransform.anchoredPosition = new Vector2(leftPadding, -topPadding);
		textUnderlineNode.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		textUnderlineNode.rectTransform.anchoredPosition = new Vector2(leftPadding, -topPadding);
		textStrikethroughNode.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		textStrikethroughNode.rectTransform.anchoredPosition = new Vector2(leftPadding, -topPadding + 2);
		background.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
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

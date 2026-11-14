using System;
using TMPro;
using UnityEngine;

public class RichTextElement : PixelElement
{
	protected readonly TreeNodeRender textNode;

	protected readonly TreeNodeRender textBackgroundNode;

	protected readonly TreeNodeRender textUnderlineNode;

	protected readonly TreeNodeRender textStrikethroughNode;

	protected readonly TreeNodeRender insideBorder;

	protected readonly TreeNodeRender background;

	private int leftPadding = 2;

	private int rightPadding = 3;

	private int topPadding = 3;

	private int bottomPadding;

	public virtual bool canResize { get; }

	public int widthPixels { get; protected set; }

	public int heightPixels { get; protected set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner => handler;

	public int maxCharWidth { get; private set; }

	public int fixedWidth { get; private set; }

	public int fixedHeight { get; private set; }

	public RichTextElement(bool canResize = false, int fixedWidth = -1, int fixedHeight = -1, bool hasRaycast = true)
	{
		maxCharWidth = 10000;
		this.fixedWidth = fixedWidth;
		this.fixedHeight = fixedHeight;
		this.canResize = canResize;
		handler = TreeNodeRender.Instantiate(this);
		background = TreeNodeRender.Instantiate().SetParent(handler);
		insideBorder = TreeNodeRender.Instantiate().SetParent(handler);
		textBackgroundNode = TreeNodeRender.InstantiateText().SetParent(background);
		textNode = TreeNodeRender.InstantiateText().SetParent(background);
		textUnderlineNode = TreeNodeRender.InstantiateText().SetParent(background);
		textStrikethroughNode = TreeNodeRender.InstantiateText().SetParent(background);
		RenderHandler.current.SetHighlightColor(background.image, RenderHandler.BackgroundColor.Tan, RenderHandler.BackgroundType.Smooth);
		background.image.SetNativeSize();
		if (hasRaycast)
		{
			background.InitEventHandler();
		}
		insideBorder.image.sprite = RenderHandler.current.inside;
		insideBorder.image.SetNativeSize();
		insideBorder.image.raycastTarget = false;
		insideBorder.image.fillCenter = false;
		background.image.enabled = true;
		insideBorder.image.enabled = true;
		background.mask.enabled = true;
		InitTextNode(textBackgroundNode);
		InitTextNode(textNode);
		InitTextNode(textUnderlineNode);
		InitTextNode(textStrikethroughNode);
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
		if (fixedHeight >= 0)
		{
			return fixedHeight;
		}
		textNode.text.rectTransform.sizeDelta = new Vector2(widthPixels, 1f);
		float num = textNode.text.preferredHeight;
		if (num <= 1f)
		{
			num = heightPixels;
		}
		if (canResize)
		{
			return Math.Max(height, (int)num) + topPadding + bottomPadding;
		}
		return (int)num + topPadding + bottomPadding;
	}

	public void SetPadding(int left, int right, int top, int bottom = 0)
	{
		leftPadding = left;
		rightPadding = right;
		topPadding = top;
		bottomPadding = bottom;
		ResizePixels(widthPixels, heightPixels);
	}

	public void SetFixedSize(int fixedWidth, int fixedHeight)
	{
		this.fixedWidth = fixedWidth;
		this.fixedHeight = fixedHeight;
		ResizePixels(fixedWidth, fixedHeight);
	}

	public virtual void ResizePixels(int width, int height)
	{
		widthPixels = WidthPixelsClamp(width);
		heightPixels = HeightPixelsClamp(height);
		Validate();
	}

	public RichTextElement SetRichText(RichText richText, int maxWidth = 10000)
	{
		if (richText == null)
		{
			return this;
		}
		if (fixedWidth >= 0)
		{
			maxWidth = Math.Min(maxWidth, fixedWidth / UISettings.current.effectiveFontWidth);
		}
		textBackgroundNode.text.enabled = true;
		textUnderlineNode.text.enabled = true;
		textStrikethroughNode.text.enabled = true;
		textNode.text.enabled = true;
		textNode.text.overflowMode = TextOverflowModes.Overflow;
		textNode.text.enableWordWrapping = false;
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

	public RichTextElement ClearText()
	{
		textBackgroundNode.text.enabled = false;
		textUnderlineNode.text.enabled = false;
		textStrikethroughNode.text.enabled = false;
		textNode.text.enabled = true;
		textNode.text.text = "";
		textNode.text.overflowMode = TextOverflowModes.Overflow;
		textNode.text.enableWordWrapping = true;
		ResizePixels(widthPixels, heightPixels);
		return this;
	}

	public RichTextElement SetText(string text, int maxWidth = 10000, RenderHandler.ColorPalette color = RenderHandler.ColorPalette.LightGray)
	{
		if (text == null)
		{
			return this;
		}
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

	public TreeNodeRender GetText()
	{
		return textNode;
	}

	public void Validate()
	{
		textNode.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		textNode.rectTransform.anchoredPosition = new Vector2(leftPadding, -topPadding);
		textBackgroundNode.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		textBackgroundNode.rectTransform.anchoredPosition = new Vector2(leftPadding, -topPadding);
		textUnderlineNode.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		textUnderlineNode.rectTransform.anchoredPosition = new Vector2(leftPadding, -topPadding);
		textStrikethroughNode.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		textStrikethroughNode.rectTransform.anchoredPosition = new Vector2(leftPadding, -topPadding + 2);
		background.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		insideBorder.rectTransform.sizeDelta = new Vector2(widthPixels + 2, heightPixels + 2);
		insideBorder.rectTransform.anchoredPosition = new Vector2(-1f, 1f);
	}

	public void HideBackground()
	{
		background.image.enabled = false;
		insideBorder.image.enabled = false;
	}

	public void ShowBackground()
	{
		background.image.enabled = true;
		insideBorder.image.enabled = true;
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

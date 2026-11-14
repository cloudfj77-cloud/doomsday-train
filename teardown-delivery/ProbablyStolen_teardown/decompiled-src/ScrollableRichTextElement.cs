using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollableRichTextElement : PixelElement
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

	public int scrollPadding = 3;

	private int minButtonHeight = 6;

	private int minButtonWidth = 6;

	private readonly TreeNodeRender scrollBar;

	private readonly TreeNodeRender scrollButton;

	private float savedBarRelativePosition;

	private float savedBackgroundRelativePosition;

	public int absoluteMinWidth = UISettings.current.fontWidth * 4;

	public int absoluteMinHeight = UISettings.current.fontHeight;

	public bool canResize { get; set; }

	public int widthPixels { get; protected set; }

	public int heightPixels { get; protected set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	private bool constWidth { get; set; }

	public ScrollableRichTextElement(bool constWidth = false, bool canResize = true)
	{
		this.constWidth = constWidth;
		this.canResize = canResize;
		handler = TreeNodeRender.Instantiate(this);
		inner = TreeNodeRender.Instantiate().SetParent(handler);
		background = TreeNodeRender.Instantiate().SetParent(inner);
		insideBorder = TreeNodeRender.Instantiate().SetParent(handler);
		textBackgroundNode = TreeNodeRender.InstantiateText().SetParent(background);
		textNode = TreeNodeRender.InstantiateText().SetParent(background);
		textUnderlineNode = TreeNodeRender.InstantiateText().SetParent(background);
		textStrikethroughNode = TreeNodeRender.InstantiateText().SetParent(background);
		RenderHandler.current.SetHighlightColor(background.image, RenderHandler.BackgroundColor.Tan, RenderHandler.BackgroundType.Smooth);
		background.image.SetNativeSize();
		background.InitEventHandler();
		background.eventHandler.mouseScrollFunc.Add(delegate(PointerEventData r)
		{
			background.rectTransform.anchoredPosition = new Vector2(background.rectTransform.anchoredPosition.x, (int)(background.rectTransform.anchoredPosition.y - (r.scrollDelta.x + r.scrollDelta.y) * (float)UISettings.current.fontHeight * 0.7f));
			Validate();
			return false;
		});
		background.eventHandler.mouseDownFunc.Add(delegate(PointerEventData r)
		{
			savedBackgroundRelativePosition = handler.rectTransform.InverseTransformPoint(r.position).y - background.rectTransform.anchoredPosition.y;
			return false;
		});
		background.eventHandler.mouseDragFunc.Add(delegate(PointerEventData r)
		{
			background.rectTransform.anchoredPosition = new Vector2(background.rectTransform.anchoredPosition.x, (int)(handler.rectTransform.InverseTransformPoint(r.position).y - savedBackgroundRelativePosition));
			Validate();
			return false;
		});
		insideBorder.image.sprite = RenderHandler.current.inside;
		insideBorder.image.SetNativeSize();
		insideBorder.image.raycastTarget = false;
		insideBorder.image.fillCenter = false;
		background.image.enabled = true;
		insideBorder.image.enabled = true;
		inner.mask.enabled = true;
		InitTextNode(textBackgroundNode);
		InitTextNode(textNode);
		InitTextNode(textUnderlineNode);
		InitTextNode(textStrikethroughNode);
		scrollBar = TreeNodeRender.Instantiate().SetParent(handler);
		scrollButton = TreeNodeRender.Instantiate().SetParent(handler);
		scrollBar.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_right_scrollbar");
		scrollBar.image.SetNativeSize();
		scrollButton.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_outer", "border_right_scrollbar_button");
		scrollButton.image.SetNativeSize();
		scrollBar.image.enabled = true;
		scrollButton.image.enabled = true;
		scrollBar.InitEventHandler();
		scrollButton.InitEventHandler();
		scrollButton.eventHandler.mouseDownFunc.Add(delegate(PointerEventData r)
		{
			savedBarRelativePosition = handler.rectTransform.InverseTransformPoint(r.position).y - scrollButton.rectTransform.anchoredPosition.y;
			return false;
		});
		scrollButton.eventHandler.mouseDragFunc.Add(delegate(PointerEventData r)
		{
			PlaceButtonAtMousePositionRelative(r.position, savedBarRelativePosition);
			SetScroll(GetButtonScroll());
			return false;
		});
		scrollBar.eventHandler.mouseDownFunc.Add(delegate(PointerEventData r)
		{
			PlaceButtonAtMousePositionCenter(r.position);
			SetScroll(GetButtonScroll());
			return false;
		});
		scrollBar.eventHandler.mouseDragFunc.Add(delegate(PointerEventData r)
		{
			PlaceButtonAtMousePositionCenter(r.position);
			SetScroll(GetButtonScroll());
			return false;
		});
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

	public int WidthPixelsClamp(int width)
	{
		if (width < absoluteMinWidth)
		{
			width = absoluteMinWidth;
		}
		return width;
	}

	public int HeightPixelsClamp(int height)
	{
		if (height < absoluteMinHeight)
		{
			height = absoluteMinHeight;
		}
		return height;
	}

	public void ResizePixels(int width, int height)
	{
		widthPixels = WidthPixelsClamp(width);
		heightPixels = HeightPixelsClamp(height);
		Validate();
	}

	private int GetInnerWidth()
	{
		return widthPixels - scrollPadding - minButtonWidth - 2;
	}

	public int GetMaxCharWidth()
	{
		return (GetInnerWidth() - leftPadding - rightPadding) / UISettings.current.fontWidth;
	}

	public void SetRichText(RichTextBuilder richTextBuilder, int truncateLength = -1)
	{
		if (richTextBuilder != null)
		{
			richTextBuilder.maxCharWidth = GetMaxCharWidth();
			RichText richText = richTextBuilder.Build(truncateLength);
			textBackgroundNode.text.enabled = true;
			textUnderlineNode.text.enabled = true;
			textStrikethroughNode.text.enabled = true;
			textNode.text.enabled = true;
			textNode.text.text = richText.richTextForeground;
			textBackgroundNode.text.text = richText.richTextBackground;
			textUnderlineNode.text.text = richText.richTextUnderline;
			textStrikethroughNode.text.text = richText.richTextStrikethrough;
			Validate();
		}
	}

	public void ClearRichText()
	{
		textNode.text.text = "";
		textBackgroundNode.text.text = "";
		textUnderlineNode.text.text = "";
		textStrikethroughNode.text.text = "";
		Validate();
	}

	public void AppendRichText(RichTextBuilder richTextBuilder, int truncateLength = -1)
	{
		if (richTextBuilder != null)
		{
			if (textNode.text.text.Length <= 0)
			{
				SetRichText(richTextBuilder, truncateLength);
				return;
			}
			richTextBuilder.maxCharWidth = GetMaxCharWidth();
			RichText richText = richTextBuilder.Build(truncateLength);
			TextMeshProUGUI text = textNode.text;
			text.text = text.text + "\n" + richText.richTextForeground;
			TextMeshProUGUI text2 = textBackgroundNode.text;
			text2.text = text2.text + "\n" + richText.richTextBackground;
			TextMeshProUGUI text3 = textUnderlineNode.text;
			text3.text = text3.text + "\n" + richText.richTextUnderline;
			TextMeshProUGUI text4 = textStrikethroughNode.text;
			text4.text = text4.text + "\n" + richText.richTextStrikethrough;
			Validate();
		}
	}

	public void Validate()
	{
		int innerWidth = GetInnerWidth();
		int num = (int)textNode.text.preferredWidth;
		int num2 = (int)textNode.text.preferredHeight;
		int num3 = (constWidth ? innerWidth : Math.Max(num + leftPadding + rightPadding, innerWidth));
		int num4 = Math.Max(num2 + topPadding, heightPixels);
		background.rectTransform.sizeDelta = new Vector2(num3, num4);
		background.rectTransform.anchoredPosition = new Vector2(background.rectTransform.anchoredPosition.x, Math.Clamp(background.rectTransform.anchoredPosition.y, 0f, num4 - heightPixels));
		textNode.rectTransform.sizeDelta = new Vector2(num, num2);
		textBackgroundNode.rectTransform.sizeDelta = new Vector2(num, num2);
		textUnderlineNode.rectTransform.sizeDelta = new Vector2(num, num2);
		textStrikethroughNode.rectTransform.sizeDelta = new Vector2(num, num2);
		textNode.rectTransform.anchoredPosition = new Vector2(leftPadding, -topPadding);
		textBackgroundNode.rectTransform.anchoredPosition = new Vector2(leftPadding, -topPadding);
		textUnderlineNode.rectTransform.anchoredPosition = new Vector2(leftPadding, -topPadding);
		textStrikethroughNode.rectTransform.anchoredPosition = new Vector2(leftPadding, -topPadding + 2);
		inner.rectTransform.sizeDelta = new Vector2(innerWidth, heightPixels);
		inner.rectTransform.anchoredPosition = new Vector2(0f, 0f);
		insideBorder.rectTransform.sizeDelta = new Vector2(innerWidth + 2, heightPixels + 2);
		insideBorder.rectTransform.anchoredPosition = new Vector2(-1f, 1f);
		int num5 = heightPixels - 2;
		int num6 = (int)Math.Round((float)heightPixels / (float)num4 * (float)num5);
		if (num6 < minButtonHeight)
		{
			num6 = minButtonHeight;
		}
		ValidateScrollBar(num6, num5);
	}

	private void ValidateScrollBar(int buttonHeight, int barInnerHeight)
	{
		int innerWidth = GetInnerWidth();
		Math.Max((int)textNode.text.preferredHeight + topPadding, heightPixels);
		scrollBar.rectTransform.sizeDelta = new Vector2(minButtonWidth + 2, heightPixels);
		scrollBar.rectTransform.anchoredPosition = new Vector2(innerWidth + scrollPadding, 0f);
		int num = (int)Math.Round(GetScroll() * (float)(barInnerHeight - buttonHeight));
		scrollButton.rectTransform.sizeDelta = new Vector2(minButtonWidth, buttonHeight);
		scrollButton.rectTransform.anchoredPosition = new Vector2(innerWidth + scrollPadding + 1, -1 - num);
	}

	private void PlaceButtonAtMousePositionCenter(Vector2 mousePosition)
	{
		int num = (int)scrollButton.rectTransform.sizeDelta.y;
		int num2 = Math.Clamp((int)(handler.rectTransform.InverseTransformPoint(mousePosition).y + scrollButton.rectTransform.sizeDelta.y / 2f), -heightPixels + 1 + num, -1);
		scrollButton.rectTransform.anchoredPosition = new Vector2(scrollButton.rectTransform.anchoredPosition.x, num2);
	}

	private void PlaceButtonAtMousePositionRelative(Vector2 mousePosition, float relativePosition)
	{
		int num = (int)scrollButton.rectTransform.sizeDelta.y;
		int num2 = Math.Clamp((int)(handler.rectTransform.InverseTransformPoint(mousePosition).y - relativePosition), -heightPixels + 1 + num, -1);
		scrollButton.rectTransform.anchoredPosition = new Vector2(scrollButton.rectTransform.anchoredPosition.x, num2);
	}

	private float GetButtonScroll()
	{
		int num = heightPixels - 2;
		float y = scrollButton.rectTransform.sizeDelta.y;
		return (-1f - scrollButton.rectTransform.anchoredPosition.y) / ((float)num - y);
	}

	public float GetScroll()
	{
		int num = Math.Max((int)textNode.text.preferredHeight + topPadding, heightPixels);
		if (num <= heightPixels)
		{
			return 0f;
		}
		return Math.Clamp(background.rectTransform.anchoredPosition.y / (float)(num - heightPixels), 0f, 1f);
	}

	public void SetScroll(float percentage)
	{
		int num = Math.Max((int)textNode.text.preferredHeight + topPadding, heightPixels);
		if (num > heightPixels)
		{
			percentage = Math.Clamp(percentage, 0f, 1f);
			background.rectTransform.anchoredPosition = new Vector2(background.rectTransform.anchoredPosition.x, percentage * (float)(num - heightPixels));
			Validate();
		}
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

using System;
using TMPro;
using UnityEngine;

public class MachineryText : PixelElement, MachineryWidget
{
	private readonly TreeNodeRender textNode;

	private int requestedWidth;

	private int naturalWidth;

	private int minHeight = 10;

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner => handler;

	public bool fillWidth { get; set; } = true;


	public MachineryText(string text = null, int size = 6, HorizontalAlignmentOptions alignment = HorizontalAlignmentOptions.Center, bool uppercase = true)
	{
		handler = TreeNodeRender.Instantiate(this, "MachineryText");
		textNode = TreeNodeRender.InstantiateText().SetParent(handler);
		MachineryStyle.InitText(textNode, size, MachineryStyle.BODY_TEXT, alignment, uppercase);
		if (text != null)
		{
			SetText(text);
		}
		Validate();
	}

	public MachineryText WithInfoLink(string linkId, Func<string> tooltip)
	{
		TMPIconHover.CustomLinks[linkId] = tooltip;
		string text = GetText();
		string text2 = "<link=\"" + linkId + "\"><sprite name=\"info\" tint=1></link>";
		SetText(string.IsNullOrEmpty(text) ? text2 : (text + " " + text2));
		textNode.text.raycastTarget = true;
		if (textNode.text.GetComponent<TMPIconHover>() == null)
		{
			textNode.text.gameObject.AddComponent<TMPIconHover>();
		}
		return this;
	}

	public static MachineryText Status(string label, string value)
	{
		MachineryText machineryText = new MachineryText();
		machineryText.SetStatus(label, value);
		return machineryText;
	}

	public MachineryText SetStatus(string label, string value)
	{
		string text = ColorUtility.ToHtmlStringRGB(MachineryStyle.VALUE_TEXT);
		return SetText(label + ": <color=#" + text + ">" + value + "</color>");
	}

	public string GetText()
	{
		return textNode.text.text;
	}

	public MachineryText SetText(string text)
	{
		textNode.text.text = text ?? "";
		Validate();
		MachineryContainerUtils.NotifySizeChanged(this);
		return this;
	}

	public MachineryText SetColor(Color color)
	{
		textNode.text.color = color;
		return this;
	}

	public MachineryText SetSize(int size)
	{
		textNode.text.fontSize = size;
		Validate();
		return this;
	}

	public MachineryText SetAlignment(HorizontalAlignmentOptions alignment)
	{
		textNode.text.horizontalAlignment = alignment;
		return this;
	}

	public MachineryText SetUppercase(bool uppercase)
	{
		textNode.text.fontStyle = (uppercase ? FontStyles.UpperCase : FontStyles.Normal);
		Validate();
		return this;
	}

	public MachineryText SetFillWidth(bool fill)
	{
		fillWidth = fill;
		Validate();
		return this;
	}

	public MachineryText SetMinHeight(int height)
	{
		minHeight = height;
		Validate();
		return this;
	}

	public void Validate()
	{
		naturalWidth = MachineryStyle.TextWidth(textNode);
		widthPixels = (fillWidth ? Mathf.Max(naturalWidth, requestedWidth) : naturalWidth);
		heightPixels = Mathf.Max(minHeight, MachineryStyle.TextHeight(textNode));
		MachineryStyle.Place(textNode, 0, 0, widthPixels, heightPixels);
	}

	public void ResizePixels(int width, int height)
	{
		requestedWidth = width;
		Validate();
	}

	public bool Attach(PixelElement element)
	{
		return false;
	}

	public bool Detach(PixelElement element)
	{
		return false;
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

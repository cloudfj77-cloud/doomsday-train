using TMPro;
using UnityEngine;

public class MachineryLabel : PixelElement
{
	private readonly TreeNodeRender plate;

	private readonly TreeNodeRender textNode;

	private readonly TreeNodeRender iconNode;

	private string text;

	private Sprite icon;

	private int fixedWidth = -1;

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner => handler;

	public MachineryLabel(string text = null)
	{
		handler = TreeNodeRender.Instantiate(this, "MachineryLabel");
		plate = TreeNodeRender.Instantiate().SetParent(handler);
		textNode = TreeNodeRender.InstantiateText().SetParent(handler);
		iconNode = TreeNodeRender.Instantiate().SetParent(handler);
		MachineryStyle.SetSliced(plate, MachineryStyle.PlateNotched, MachineryStyle.LABEL_PLATE);
		plate.image.raycastTarget = false;
		MachineryStyle.InitText(textNode, 6, MachineryStyle.LABEL_TEXT, HorizontalAlignmentOptions.Center, uppercase: true);
		iconNode.image.enabled = false;
		iconNode.image.raycastTarget = false;
		if (text != null)
		{
			SetText(text);
		}
		Validate();
	}

	public static MachineryLabel Icon(Sprite sprite)
	{
		return new MachineryLabel().SetIcon(sprite);
	}

	public static MachineryLabel Power()
	{
		return Icon(MachineryStyle.IconPower);
	}

	public static MachineryLabel Note()
	{
		return Icon(MachineryStyle.IconNote);
	}

	public string GetText()
	{
		return text;
	}

	public MachineryLabel SetText(string text)
	{
		this.text = text;
		icon = null;
		textNode.text.text = text ?? "";
		textNode.text.enabled = !string.IsNullOrEmpty(text);
		iconNode.image.enabled = false;
		Validate();
		MachineryContainerUtils.NotifySizeChanged(this);
		return this;
	}

	public MachineryLabel SetIcon(Sprite sprite)
	{
		icon = sprite;
		text = null;
		textNode.text.enabled = false;
		MachineryStyle.SetSimple(iconNode, sprite, Color.white);
		iconNode.image.enabled = sprite != null;
		Validate();
		return this;
	}

	public MachineryLabel SetIcon(string atlasPath, string spriteName)
	{
		return SetIcon(RenderHandler.LoadFromAtlas(atlasPath, spriteName));
	}

	public MachineryLabel SetPlateColor(Color color)
	{
		plate.image.color = color;
		return this;
	}

	public MachineryLabel SetTextColor(Color color)
	{
		textNode.text.color = color;
		return this;
	}

	public MachineryLabel SetWidth(int width)
	{
		fixedWidth = width;
		Validate();
		return this;
	}

	public void Validate()
	{
		heightPixels = 10;
		int value;
		if (icon != null)
		{
			value = Mathf.Max(21, (int)icon.rect.width + 6);
		}
		else
		{
			value = ((!string.IsNullOrEmpty(text)) ? (MachineryStyle.TextWidth(textNode) + 12) : 0);
			value = Mathf.Max(value, 32);
		}
		widthPixels = ((fixedWidth >= 0) ? fixedWidth : MachineryStyle.Even(value));
		MachineryStyle.Place(plate, 0, 0, widthPixels, heightPixels);
		MachineryStyle.Place(textNode, 0, 0, widthPixels, heightPixels);
		if (icon != null)
		{
			int num = (int)icon.rect.width;
			int num2 = (int)icon.rect.height;
			MachineryStyle.Place(iconNode, (widthPixels - num) / 2, (heightPixels - num2) / 2, num, num2);
		}
	}

	public void ResizePixels(int width, int height)
	{
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

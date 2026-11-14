using System.Collections.Generic;
using UnityEngine;

public class MachineryPanel : PixelElement, MachineryContainer
{
	private MachineryStorageOwner _owner;

	public string role;

	private readonly TreeNodeRender plate;

	private readonly TreeNodeRender labelHost;

	private int requestedWidth;

	private int requestedHeight;

	private int minWidth;

	private bool validateLock;

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	public MachineryStorageOwner owner
	{
		get
		{
			return _owner;
		}
		set
		{
			_owner = value;
			MachineryContainerUtils.SetOwner(content, value);
		}
	}

	public MachineryLabel label { get; private set; }

	public PixelElement content { get; private set; }

	public bool isHidden { get; private set; }

	public MachineryPanel(string labelText = null, PixelElement content = null, string role = null)
	{
		this.role = role;
		handler = TreeNodeRender.Instantiate(this, "MachineryPanel" + ((role != null) ? (" (" + role + ")") : ""));
		plate = TreeNodeRender.Instantiate().SetParent(handler);
		labelHost = TreeNodeRender.Instantiate().SetParent(handler);
		inner = TreeNodeRender.Instantiate().SetParent(handler);
		MachineryStyle.SetSliced(plate, MachineryStyle.BevelFrame);
		plate.image.raycastTarget = false;
		labelHost.image.enabled = false;
		inner.image.enabled = false;
		if (labelText != null)
		{
			SetLabel(labelText);
		}
		if (content != null)
		{
			SetContent(content);
		}
		Validate();
	}

	public MachineryPanel(Sprite icon, PixelElement content = null, string role = null)
		: this((string)null, content, role)
	{
		SetLabel(MachineryLabel.Icon(icon));
	}

	public MachineryPanel SetLabel(string text)
	{
		if (text == null)
		{
			return SetLabel((MachineryLabel)null);
		}
		if (label != null)
		{
			label.SetText(text);
		}
		else
		{
			SetLabel(new MachineryLabel(text));
		}
		Validate();
		return this;
	}

	public MachineryPanel SetLabelIcon(Sprite icon)
	{
		if (label != null)
		{
			label.SetIcon(icon);
		}
		else
		{
			SetLabel(MachineryLabel.Icon(icon));
		}
		Validate();
		return this;
	}

	public MachineryPanel SetLabel(MachineryLabel element)
	{
		if (label != null)
		{
			label.handler.ResetParent();
		}
		label = element;
		if (label != null)
		{
			label.handler.SetParent(labelHost);
			label.handler.rectTransform.anchoredPosition = Vector2.zero;
		}
		Validate();
		return this;
	}

	public MachineryPanel SetContent(PixelElement element)
	{
		if (content != null)
		{
			Detach(content);
		}
		if (element == null)
		{
			return this;
		}
		content = element;
		element.SetRenderingParent(this);
		element.handler.rectTransform.anchoredPosition = Vector2.zero;
		MachineryContainerUtils.SetOwner(element, owner);
		owner?.RelinkStorage();
		Validate();
		return this;
	}

	public MachineryPanel SetRole(string role)
	{
		this.role = role;
		return this;
	}

	public MachineryPanel SetMinWidth(int width)
	{
		minWidth = width;
		Validate();
		return this;
	}

	public MachineryPanel SetHidden(bool hidden)
	{
		isHidden = hidden;
		handler.gameObject.SetActive(!hidden);
		if (owner is PixelElement pixelElement)
		{
			pixelElement.Validate();
		}
		return this;
	}

	public bool Attach(PixelElement element)
	{
		SetContent(element);
		return content == element;
	}

	public bool Detach(PixelElement element)
	{
		if (element == null || element != content)
		{
			return false;
		}
		content = null;
		element.RemoveRenderingParent(this);
		owner?.RelinkStorage();
		Validate();
		return true;
	}

	public void CollectStorage(List<GraphNodeStorage> list)
	{
		MachineryContainerUtils.Collect(content, list);
	}

	public void Validate()
	{
		if (!validateLock)
		{
			validateLock = true;
			int num = 4;
			label?.Validate();
			MachineryContainerUtils.Refresh(content);
			int num2 = ((label != null) ? 18 : num);
			int num3 = label?.widthPixels ?? 0;
			int b = content?.widthPixels ?? 0;
			int num4 = content?.heightPixels ?? 0;
			int num5 = Mathf.Max(num3, b) + 2 * num;
			int a = num2 + num4 + num;
			widthPixels = Mathf.Max(num5, requestedWidth, minWidth);
			heightPixels = Mathf.Max(a, requestedHeight);
			MachineryStyle.Place(plate, 0, 0, widthPixels, heightPixels);
			if (label != null)
			{
				MachineryStyle.Place(labelHost, (widthPixels - num3) / 2, 4, num3, 10);
			}
			int num6 = widthPixels - 2 * num;
			int num7 = heightPixels - num2 - num;
			if (content != null)
			{
				MachineryContainerUtils.TryResize(content, num6, num7);
				b = content.widthPixels;
				num4 = content.heightPixels;
				content.handler.rectTransform.anchoredPosition = new Vector2((num6 - b) / 2, -((num7 - num4) / 2));
			}
			MachineryStyle.Place(inner, num, num2, num6, num7);
			validateLock = false;
		}
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

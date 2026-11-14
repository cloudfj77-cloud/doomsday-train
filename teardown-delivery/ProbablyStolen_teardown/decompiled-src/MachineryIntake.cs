using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineryIntake : PixelElement, MachineryContainer
{
	public bool vents = true;

	private readonly TreeNodeRender plate;

	private readonly TreeNodeRender ventHost;

	private readonly List<TreeNodeRender> ventBars = new List<TreeNodeRender>();

	private int requestedWidth;

	private int requestedHeight;

	private readonly bool styleInventories;

	private bool validateLock;

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	public MachineryStorageOwner owner { get; set; }

	public PixelElement content { get; private set; }

	public MachineryIntake(PixelElement content = null, bool vents = true, bool styleInventories = true)
	{
		this.vents = vents;
		this.styleInventories = styleInventories;
		handler = TreeNodeRender.Instantiate(this, "MachineryIntake");
		plate = TreeNodeRender.Instantiate().SetParent(handler);
		ventHost = TreeNodeRender.Instantiate().SetParent(handler);
		inner = TreeNodeRender.Instantiate().SetParent(handler);
		MachineryStyle.SetSliced(plate, MachineryStyle.Intake);
		plate.image.raycastTarget = false;
		ventHost.image.enabled = false;
		inner.image.enabled = false;
		if (content != null)
		{
			SetContent(content);
		}
		Validate();
	}

	public MachineryIntake SetContent(PixelElement element)
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
		if (styleInventories)
		{
			MachineryStyle.ApplyInventoryStyle(element);
		}
		element.SetRenderingParent(this);
		element.handler.rectTransform.anchoredPosition = Vector2.zero;
		MachineryContainerUtils.SetOwner(element, owner);
		owner?.RelinkStorage();
		Validate();
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
			MachineryContainerUtils.Refresh(content);
			int num2 = content?.widthPixels ?? 0;
			int num3 = content?.heightPixels ?? 0;
			widthPixels = Mathf.Max(num2 + 2 * num, requestedWidth);
			heightPixels = Mathf.Max(num3 + 2 * num, requestedHeight);
			MachineryStyle.Place(plate, 0, 0, widthPixels, heightPixels);
			int num4 = (widthPixels - num2) / 2;
			int num5 = (heightPixels - num3) / 2;
			MachineryStyle.Place(inner, num4, num5, num2, num3);
			LayoutVents(num4, num5, num2, num3);
			validateLock = false;
		}
	}

	private void LayoutVents(int childX, int childY, int childW, int childH)
	{
		int num = 0;
		if (vents && content != null && childW > 0)
		{
			int num2 = childY - 7 - 5;
			int num3 = heightPixels - (childY + childH) - 7 - 5;
			if (childY >= 16)
			{
				int num4 = childY - 5;
				while (num4 - 3 >= 7 && num2 > 0)
				{
					num4 -= 3;
					PlaceVent(num++, childX, num4, childW);
					num4--;
				}
			}
			if (heightPixels - (childY + childH) >= 16)
			{
				for (int i = childY + childH + 5; i + 3 <= heightPixels - 7; i += 4)
				{
					if (num3 <= 0)
					{
						break;
					}
					PlaceVent(num++, childX, i, childW);
				}
			}
		}
		for (int j = num; j < ventBars.Count; j++)
		{
			ventBars[j].image.enabled = false;
		}
	}

	private void PlaceVent(int index, int x, int y, int width)
	{
		while (index >= ventBars.Count)
		{
			TreeNodeRender treeNodeRender = TreeNodeRender.Instantiate().SetParent(ventHost);
			treeNodeRender.image.sprite = MachineryStyle.Vent;
			treeNodeRender.image.type = Image.Type.Simple;
			treeNodeRender.image.raycastTarget = false;
			ventBars.Add(treeNodeRender);
		}
		ventBars[index].image.enabled = true;
		MachineryStyle.Place(ventBars[index], x, y, width, 3);
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

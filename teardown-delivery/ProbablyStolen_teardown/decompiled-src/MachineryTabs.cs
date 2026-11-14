using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MachineryTabs : PixelElement, MachineryContainer
{
	private class Tab
	{
		public string caption;

		public PixelElement page;

		public TreeNodeRender plate;

		public TreeNodeRender text;

		public int width;
	}

	private MachineryStorageOwner _owner;

	public Action<int> onChanged;

	public Action<int> onRenameRequest;

	private int hovered = -1;

	private readonly List<Tab> tabs = new List<Tab>();

	private readonly TreeNodeRender strip;

	private readonly TreeNodeRender underline;

	private int requestedWidth;

	private int requestedHeight;

	private static MachineryTabs dragHoverTabs;

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
			foreach (Tab tab in tabs)
			{
				MachineryContainerUtils.SetOwner(tab.page, value);
			}
		}
	}

	public int selected { get; private set; } = -1;


	public int count => tabs.Count;

	public MachineryTabs()
	{
		handler = TreeNodeRender.Instantiate(this, "MachineryTabs");
		strip = TreeNodeRender.Instantiate().SetParent(handler);
		underline = TreeNodeRender.Instantiate().SetParent(handler);
		inner = TreeNodeRender.Instantiate().SetParent(handler);
		strip.image.enabled = false;
		inner.image.enabled = false;
		MachineryStyle.SetFlat(underline, MachineryStyle.TAB_UNDERLINE);
		underline.image.raycastTarget = false;
		Validate();
	}

	public int AddTab(string caption, PixelElement page)
	{
		Tab tab = new Tab
		{
			caption = caption,
			page = page
		};
		tab.plate = TreeNodeRender.Instantiate().SetParent(strip);
		tab.text = TreeNodeRender.InstantiateText().SetParent(strip);
		MachineryStyle.SetSliced(tab.plate, MachineryStyle.TabPlate, MachineryStyle.TAB_PLATE);
		MachineryStyle.InitText(tab.text, 6, MachineryStyle.TAB_TEXT, HorizontalAlignmentOptions.Center, uppercase: true);
		tab.text.text.text = caption ?? "";
		int index = tabs.Count;
		tab.plate.InitEventHandler();
		tab.plate.eventHandler.mouseClickFunc.Add(delegate
		{
			Select(index, notify: true);
			return true;
		});
		tab.plate.eventHandler.mouseDownFunc.Add(delegate
		{
			handler.FindParentWindow()?.ToFront();
			return true;
		});
		tab.plate.eventHandler.mouseDoubleClickFunc.Add(delegate
		{
			if (onRenameRequest != null && index == selected)
			{
				onRenameRequest(index);
			}
			return true;
		});
		tab.plate.eventHandler.mouseEnterFunc.Add(delegate
		{
			hovered = index;
			RefreshTabs();
			return true;
		});
		tab.plate.eventHandler.mouseExitFunc.Add(delegate
		{
			if (hovered == index)
			{
				hovered = -1;
			}
			RefreshTabs();
			return true;
		});
		tabs.Add(tab);
		if (page != null)
		{
			page.SetRenderingParent(this);
			page.handler.rectTransform.anchoredPosition = Vector2.zero;
			MachineryContainerUtils.SetOwner(page, owner);
		}
		owner?.RelinkStorage();
		if (selected < 0)
		{
			Select(0);
		}
		else
		{
			RefreshTabs();
		}
		Validate();
		return index;
	}

	public PixelElement GetPage(int index)
	{
		if (index < 0 || index >= tabs.Count)
		{
			return null;
		}
		return tabs[index].page;
	}

	public string GetCaption(int index)
	{
		if (index < 0 || index >= tabs.Count)
		{
			return null;
		}
		return tabs[index].caption;
	}

	public static GameInventory FindDropInventoryAt(Vector2 screenPosition, TreeNodeRender ignoreNode = null, PixelElement ignoreElement = null)
	{
		if (!FindTabAt(screenPosition, ignoreNode, ignoreElement, out var machineryTabs, out var index))
		{
			return null;
		}
		return machineryTabs.GetTabInventory(index);
	}

	private static bool FindTabAt(Vector2 screenPosition, TreeNodeRender ignoreNode, PixelElement ignoreElement, out MachineryTabs tabs, out int index)
	{
		tabs = null;
		index = -1;
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = screenPosition
		};
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		foreach (RaycastResult item in list)
		{
			TreeNodeRender node = item.gameObject.GetComponent<TreeNodeRender>();
			if (node == null || node == ignoreNode || (ignoreElement != null && node.FindParentElement() == ignoreElement))
			{
				continue;
			}
			tabs = node.FindParentElement<MachineryTabs>();
			if (tabs != null)
			{
				index = tabs.tabs.FindIndex((Tab t) => t.plate == node || t.text == node);
			}
			return index >= 0;
		}
		return false;
	}

	public static void UpdateDragHover(Vector2 screenPosition, TreeNodeRender ignoreNode = null, PixelElement ignoreElement = null)
	{
		FindTabAt(screenPosition, ignoreNode, ignoreElement, out var machineryTabs, out var index);
		if (dragHoverTabs != null && dragHoverTabs != machineryTabs)
		{
			ClearDragHover();
		}
		if (machineryTabs != null)
		{
			dragHoverTabs = machineryTabs;
			if (machineryTabs.hovered != index)
			{
				machineryTabs.hovered = index;
				machineryTabs.RefreshTabs();
			}
		}
	}

	public static void ClearDragHover()
	{
		MachineryTabs machineryTabs = dragHoverTabs;
		dragHoverTabs = null;
		if (machineryTabs != null && !(machineryTabs.handler == null))
		{
			machineryTabs.hovered = -1;
			machineryTabs.RefreshTabs();
		}
	}

	public GameInventory GetTabInventory(TreeNodeRender node)
	{
		return GetTabInventory(tabs.FindIndex((Tab t) => t.plate == node || t.text == node));
	}

	public GameInventory GetTabInventory(int index)
	{
		if (index < 0 || index >= tabs.Count)
		{
			return null;
		}
		Tab tab = tabs[index];
		return ((tab.page is MachineryIntake machineryIntake) ? machineryIntake.content : tab.page) as GameInventory;
	}

	public MachineryTabs SetCaption(int index, string caption)
	{
		if (index < 0 || index >= tabs.Count)
		{
			return this;
		}
		tabs[index].caption = caption;
		tabs[index].text.text.text = caption ?? "";
		Validate();
		return this;
	}

	public MachineryTabs Select(int index, bool notify = false)
	{
		if (index < 0 || index >= tabs.Count)
		{
			return this;
		}
		bool num = index != selected;
		selected = index;
		RefreshTabs();
		if (num && notify)
		{
			if (!string.IsNullOrEmpty("interface_click"))
			{
				AudioManager.Instance.Play("interface_click");
			}
			onChanged?.Invoke(index);
		}
		return this;
	}

	private void RefreshTabs()
	{
		for (int i = 0; i < tabs.Count; i++)
		{
			bool flag = i == selected;
			Color tAB_PLATE = MachineryStyle.TAB_PLATE;
			tAB_PLATE.a = (flag ? 0.3f : 0.15f);
			tabs[i].plate.image.color = tAB_PLATE;
			bool flag2 = flag && onRenameRequest != null;
			bool flag3 = (ItemMultiSelectHandler.current != null && ItemMultiSelectHandler.current.IsGroupDragging) || (ItemMouseDragHandler.current != null && ItemMouseDragHandler.current.IsDraggingItem);
			bool flag4 = i == hovered && (flag2 || flag3);
			tabs[i].text.text.color = (flag4 ? MachineryStyle.TAB_TEXT_HOVER : (flag ? MachineryStyle.TAB_TEXT_ACTIVE : MachineryStyle.TAB_TEXT));
			tabs[i].page?.handler.gameObject.SetActive(flag);
		}
	}

	public bool Attach(PixelElement element)
	{
		AddTab(null, element);
		return true;
	}

	public bool Detach(PixelElement element)
	{
		int num = tabs.FindIndex((Tab t) => t.page == element);
		if (num < 0)
		{
			return false;
		}
		Tab tab = tabs[num];
		tab.page?.RemoveRenderingParent(this);
		tab.plate.RemoveParent();
		tab.text.RemoveParent();
		tabs.RemoveAt(num);
		if (selected >= tabs.Count)
		{
			selected = tabs.Count - 1;
		}
		owner?.RelinkStorage();
		RefreshTabs();
		Validate();
		return true;
	}

	public void CollectStorage(List<GraphNodeStorage> list)
	{
		foreach (Tab tab in tabs)
		{
			MachineryContainerUtils.Collect(tab.page, list);
		}
	}

	public void Validate()
	{
		if (validateLock)
		{
			return;
		}
		validateLock = true;
		int num = 4;
		foreach (Tab tab in tabs)
		{
			int num2 = ((!string.IsNullOrEmpty(tab.caption)) ? MachineryStyle.TextWidth(tab.text) : 0);
			tab.width = MachineryStyle.Even(Mathf.Max(32, num2 + 8));
			num += tab.width + 2;
		}
		if (tabs.Count > 0)
		{
			num -= 2;
		}
		num += 4;
		int num3 = 0;
		int num4 = 0;
		foreach (Tab tab2 in tabs)
		{
			if (tab2.page != null)
			{
				MachineryContainerUtils.Refresh(tab2.page);
				num3 = Mathf.Max(num3, tab2.page.widthPixels);
				num4 = Mathf.Max(num4, tab2.page.heightPixels);
			}
		}
		int num5 = 14;
		widthPixels = Mathf.Max(num, num3, requestedWidth);
		heightPixels = Mathf.Max(num5 + num4, requestedHeight);
		int num6 = heightPixels - num5;
		int num7 = 4;
		foreach (Tab tab3 in tabs)
		{
			MachineryStyle.Place(tab3.plate, num7, 0, tab3.width, 10);
			MachineryStyle.Place(tab3.text, num7, 0, tab3.width, 10);
			num7 += tab3.width + 2;
			if (tab3.page != null)
			{
				MachineryContainerUtils.TryResize(tab3.page, widthPixels, num6);
				int num8 = tab3.page.widthPixels;
				int num9 = tab3.page.heightPixels;
				tab3.page.handler.rectTransform.anchoredPosition = new Vector2((widthPixels - num8) / 2, -((num6 - num9) / 2));
			}
		}
		MachineryStyle.Place(strip, 0, 0, widthPixels, 10);
		MachineryStyle.Place(underline, 1, 10, Mathf.Max(0, widthPixels - 2), 1);
		MachineryStyle.Place(inner, 0, num5, widthPixels, num6);
		validateLock = false;
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

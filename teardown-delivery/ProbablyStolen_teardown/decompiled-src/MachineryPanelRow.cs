using System.Collections.Generic;
using UnityEngine;

public class MachineryPanelRow : PixelElement, MachineryStorageOwner, GraphNodeStorage
{
	public bool searchUsingPriority = true;

	public int columns;

	private readonly List<MachineryPanel> panels = new List<MachineryPanel>();

	private readonly List<TreeNodeRender> separators = new List<TreeNodeRender>();

	private readonly List<TreeNodeRender> separatorBackings = new List<TreeNodeRender>();

	private readonly TreeNodeRender separatorHost;

	private List<GraphNodeStorage> linked = new List<GraphNodeStorage>();

	private int[] displayOrder;

	private bool validateLock;

	public List<GraphNodeStorage> parents
	{
		get
		{
			List<GraphNodeStorage> list = new List<GraphNodeStorage>();
			GraphNodeStorage graphNodeStorage = parent;
			if (graphNodeStorage != null)
			{
				list.Add(graphNodeStorage);
			}
			return list;
		}
	}

	public List<GraphNodeStorage> children => CollectStorage();

	public GraphNodeStorage parent { get; set; }

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner { get; private set; }

	public IReadOnlyList<MachineryPanel> Panels => panels;

	public MachineryPanelRow()
	{
		handler = TreeNodeRender.Instantiate(this, "MachineryPanelRow");
		separatorHost = TreeNodeRender.Instantiate().SetParent(handler);
		inner = TreeNodeRender.Instantiate().SetParent(handler);
		separatorHost.image.enabled = false;
		inner.image.enabled = false;
		Validate();
	}

	public MachineryPanelRow(params MachineryPanel[] panels)
		: this()
	{
		foreach (MachineryPanel panel in panels)
		{
			Add(panel);
		}
	}

	public MachineryPanelRow SetDisplayOrder(params int[] order)
	{
		displayOrder = ((order != null && order.Length != 0) ? order : null);
		Validate();
		return this;
	}

	private List<MachineryPanel> DisplayPanels()
	{
		if (displayOrder == null)
		{
			return panels;
		}
		List<MachineryPanel> list = new List<MachineryPanel>(panels.Count);
		int[] array = displayOrder;
		foreach (int num in array)
		{
			if (num >= 0 && num < panels.Count && !list.Contains(panels[num]))
			{
				list.Add(panels[num]);
			}
		}
		foreach (MachineryPanel panel in panels)
		{
			if (!list.Contains(panel))
			{
				list.Add(panel);
			}
		}
		return list;
	}

	public MachineryPanelRow SetColumns(int columns)
	{
		this.columns = columns;
		Validate();
		return this;
	}

	public MachineryPanelRow Add(MachineryPanel panel)
	{
		Attach(panel);
		return this;
	}

	public bool Attach(PixelElement element)
	{
		if (element == null)
		{
			return false;
		}
		MachineryPanel machineryPanel = (element as MachineryPanel) ?? new MachineryPanel((string)null, element, (string)null);
		if (panels.Contains(machineryPanel))
		{
			return false;
		}
		panels.Add(machineryPanel);
		((PixelElement)machineryPanel).SetRenderingParent((PixelElement)this);
		machineryPanel.handler.rectTransform.anchoredPosition = Vector2.zero;
		machineryPanel.owner = this;
		RelinkStorage();
		Validate();
		return true;
	}

	public bool Detach(PixelElement element)
	{
		MachineryPanel machineryPanel = (element as MachineryPanel) ?? panels.Find((MachineryPanel p) => p.content == element);
		if (machineryPanel == null || !panels.Remove(machineryPanel))
		{
			return false;
		}
		((PixelElement)machineryPanel).RemoveRenderingParent((PixelElement)this);
		machineryPanel.owner = null;
		RelinkStorage();
		Validate();
		return true;
	}

	public MachineryPanel GetPanel(int index)
	{
		if (index < 0 || index >= panels.Count)
		{
			return null;
		}
		return panels[index];
	}

	public MachineryPanel GetPanel(string role)
	{
		return panels.Find((MachineryPanel p) => p.role == role);
	}

	public GraphNodeStorage GetStorage(int index)
	{
		List<GraphNodeStorage> list = CollectStorage();
		if (index < 0 || index >= list.Count)
		{
			return null;
		}
		return list[index];
	}

	private List<GraphNodeStorage> CollectStorage()
	{
		List<GraphNodeStorage> list = new List<GraphNodeStorage>();
		foreach (MachineryPanel panel in panels)
		{
			panel.CollectStorage(list);
		}
		return list;
	}

	public void RelinkStorage()
	{
		List<GraphNodeStorage> current = CollectStorage();
		MachineryContainerUtils.Relink(this, linked, current);
		linked = current;
	}

	public void Validate()
	{
		if (validateLock)
		{
			return;
		}
		validateLock = true;
		List<MachineryPanel> list = DisplayPanels().FindAll((MachineryPanel p) => !p.isHidden);
		foreach (MachineryPanel item in list)
		{
			item.Validate();
		}
		int num = ((columns > 0) ? columns : Mathf.Max(1, list.Count));
		int num2 = ((list.Count != 0) ? ((list.Count + num - 1) / num) : 0);
		int[] array = new int[num2];
		int[] array2 = new int[num];
		for (int i = 0; i < list.Count; i++)
		{
			int num3 = i / num;
			int num4 = i % num;
			array[num3] = Mathf.Max(array[num3], list[i].heightPixels);
			array2[num4] = Mathf.Max(array2[num4], list[i].widthPixels);
		}
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		for (int j = 0; j < num2; j++)
		{
			int num8 = 0;
			int num9 = j * num;
			int num10 = Mathf.Min(num, list.Count - num9);
			for (int k = 0; k < num10; k++)
			{
				MachineryPanel machineryPanel = list[num9 + k];
				int num11 = ((num2 > 1) ? array2[k] : machineryPanel.widthPixels);
				machineryPanel.ResizePixels(num11, array[j]);
				machineryPanel.handler.rectTransform.anchoredPosition = new Vector2(num8, -num6);
				num8 += num11;
				if (k < num10 - 1)
				{
					PlaceSeparator(num5++, num8, num6, 1, array[j] - 4, -1, -1, array[j]);
					num8++;
				}
			}
			num7 = Mathf.Max(num7, num8);
			num6 += array[j];
			if (j < num2 - 1)
			{
				PlaceSeparator(num5++, 4, num6, Mathf.Max(0, num7 - 8), 1, 0, num7);
				num6++;
			}
		}
		for (int l = num5; l < separators.Count; l++)
		{
			separators[l].image.enabled = false;
			separatorBackings[l].image.enabled = false;
		}
		widthPixels = num7;
		heightPixels = num6;
		MachineryStyle.Place(inner, 0, 0, widthPixels, heightPixels);
		handler.parent?.FindParentElement<GraphNodeStorage>()?.Validate();
		validateLock = false;
	}

	private void PlaceSeparator(int index, int x, int y, int width, int height, int backingX = -1, int backingWidth = -1, int backingHeight = -1)
	{
		while (index >= separators.Count)
		{
			TreeNodeRender treeNodeRender = TreeNodeRender.Instantiate().SetParent(separatorHost);
			MachineryStyle.SetFlat(treeNodeRender, MachineryStyle.PANEL);
			treeNodeRender.image.raycastTarget = false;
			separatorBackings.Add(treeNodeRender);
			TreeNodeRender treeNodeRender2 = TreeNodeRender.Instantiate().SetParent(separatorHost);
			MachineryStyle.SetFlat(treeNodeRender2, MachineryStyle.RULE);
			treeNodeRender2.image.raycastTarget = false;
			separators.Add(treeNodeRender2);
		}
		separatorBackings[index].image.enabled = true;
		MachineryStyle.Place(separatorBackings[index], (backingX >= 0) ? backingX : x, y, (backingWidth >= 0) ? backingWidth : width, (backingHeight >= 0) ? backingHeight : height);
		separators[index].image.enabled = true;
		MachineryStyle.Place(separators[index], x, y, width, height);
	}

	public void ResizePixels(int width, int height)
	{
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

	public bool AddParent(GraphNodeStorage node)
	{
		if (parent != null)
		{
			return false;
		}
		parent = node;
		return true;
	}

	public bool RemoveParent(GraphNodeStorage node)
	{
		if (parent != node)
		{
			return false;
		}
		parent = null;
		return true;
	}

	public bool MayHaveValidInventorySlot(GameItem item)
	{
		foreach (GraphNodeStorage child in children)
		{
			if (child != null && child.MayHaveValidInventorySlot(item))
			{
				return true;
			}
		}
		return false;
	}

	public SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		List<GraphNodeStorage> list = children;
		List<GraphNodeStorage> list2 = new List<GraphNodeStorage>();
		foreach (MachineryPanel panel in panels)
		{
			MachineryContainerUtils.CollectSelectedTabStorage(panel, list2);
		}
		if (!searchUsingPriority)
		{
			foreach (GraphNodeStorage item2 in list2)
			{
				SlotMarker slotMarker = item2?.TryFindOneValidInventorySlot(item);
				if (slotMarker != null && slotMarker.numTransfer > 0)
				{
					return slotMarker;
				}
			}
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (!list2.Contains(list[num]))
				{
					SlotMarker slotMarker2 = list[num]?.TryFindOneValidInventorySlot(item);
					if (slotMarker2 != null && slotMarker2.numTransfer > 0)
					{
						return slotMarker2;
					}
				}
			}
			return null;
		}
		SlotMarker slotMarker3 = null;
		bool flag = false;
		foreach (GraphNodeStorage item3 in list)
		{
			SlotMarker slotMarker4 = item3?.TryFindOneValidInventorySlot(item);
			if (slotMarker4 != null && slotMarker4.numTransfer > 0)
			{
				bool flag2 = list2.Contains(item3);
				int num2 = slotMarker3?.priority ?? int.MinValue;
				if (slotMarker4.priority > num2 || (slotMarker4.priority == num2 && (flag2 || !flag)))
				{
					slotMarker3 = slotMarker4;
					flag = flag2;
				}
			}
		}
		return slotMarker3;
	}
}

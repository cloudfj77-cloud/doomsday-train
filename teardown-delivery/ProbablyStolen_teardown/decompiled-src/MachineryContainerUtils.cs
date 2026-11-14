using System.Collections.Generic;

public static class MachineryContainerUtils
{
	public static void Collect(PixelElement element, List<GraphNodeStorage> list)
	{
		if (element != null)
		{
			if (element is GraphNodeStorage item)
			{
				list.Add(item);
			}
			else if (element is MachineryContainer machineryContainer)
			{
				machineryContainer.CollectStorage(list);
			}
		}
	}

	public static List<GraphNodeStorage> Collect(PixelElement element)
	{
		List<GraphNodeStorage> list = new List<GraphNodeStorage>();
		Collect(element, list);
		return list;
	}

	public static void CollectSelectedTabStorage(PixelElement element, List<GraphNodeStorage> list)
	{
		if (element == null)
		{
			return;
		}
		if (element is MachineryTabs machineryTabs)
		{
			Collect(machineryTabs.GetPage(machineryTabs.selected), list);
		}
		else if (element is MachineryPanel machineryPanel)
		{
			CollectSelectedTabStorage(machineryPanel.content, list);
		}
		else if (element is MachineryIntake machineryIntake)
		{
			CollectSelectedTabStorage(machineryIntake.content, list);
		}
		else
		{
			if (!(element is MachineryStack machineryStack))
			{
				return;
			}
			foreach (PixelElement element2 in machineryStack.elements)
			{
				CollectSelectedTabStorage(element2, list);
			}
		}
	}

	public static void SetOwner(PixelElement element, MachineryStorageOwner owner)
	{
		if (element is MachineryContainer machineryContainer)
		{
			machineryContainer.owner = owner;
		}
	}

	public static void Relink(MachineryStorageOwner owner, List<GraphNodeStorage> previous, List<GraphNodeStorage> current)
	{
		foreach (GraphNodeStorage previou in previous)
		{
			if (!current.Contains(previou))
			{
				previou.RemoveParent(owner);
			}
		}
		foreach (GraphNodeStorage item in current)
		{
			if (!previous.Contains(item))
			{
				item.AddParent(owner);
			}
		}
	}

	public static bool IsHidden(PixelElement element)
	{
		if (element == null)
		{
			return true;
		}
		if (element is SelectableElement { isHidden: not false })
		{
			return true;
		}
		if (element is ButtonElement { isHidden: not false })
		{
			return true;
		}
		if (element.handler != null && !element.handler.gameObject.activeSelf)
		{
			return true;
		}
		return false;
	}

	public static void Refresh(PixelElement element)
	{
		if (element is MachineryContainer || element is MachineryWidget)
		{
			element.Validate();
		}
	}

	public static void NotifySizeChanged(PixelElement element)
	{
		element?.handler?.parent?.FindParentElement<GraphNodeStorage>()?.Validate();
	}

	public static void TryResize(PixelElement element, int width, int height)
	{
		if (element is MachineryContainer || element is MachineryWidget)
		{
			element.ResizePixels(width, height);
		}
	}
}

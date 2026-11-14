using System.Collections.Generic;
using UnityEngine;

public class WindowsHandler : MonoBehaviour, Cancelable
{
	public static WindowsHandler current;

	public int _maxVisibleFloatingWindows = 25;

	public List<PixelWindow> visibleWindows { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
		visibleWindows = new List<PixelWindow>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (GetVisibleClosableWindows() > _maxVisibleFloatingWindows)
		{
			CloseFirstClosableWindows();
		}
	}

	private int GetVisibleClosableWindows()
	{
		int num = 0;
		foreach (PixelWindow visibleWindow in visibleWindows)
		{
			if (visibleWindow.canMove)
			{
				num++;
			}
		}
		return num;
	}

	private void CloseFirstClosableWindows()
	{
		foreach (PixelWindow visibleWindow in visibleWindows)
		{
			if (visibleWindow.canMove)
			{
				visibleWindow.closeCallback?.Invoke();
				visibleWindow.Hide();
				break;
			}
		}
	}

	private bool CloseLastClosableWindows()
	{
		for (int num = visibleWindows.Count - 1; num >= 0; num--)
		{
			if (visibleWindows[num].canMove)
			{
				PixelWindow item = visibleWindows[num];
				visibleWindows[num].closeCallback?.Invoke();
				if (visibleWindows.Contains(item))
				{
					visibleWindows[num].Hide();
				}
				return true;
			}
		}
		return false;
	}

	public static void CloseWindow(PixelWindow window)
	{
		if (window != null)
		{
			window.closeCallback?.Invoke();
			if (ContainsWindow(window))
			{
				window.Hide();
			}
		}
	}

	public static bool TryAddWindow(PixelWindow window)
	{
		if (window == null || current?.visibleWindows == null)
		{
			return false;
		}
		if (!current.visibleWindows.Contains(window) && window.IsVisible())
		{
			current.visibleWindows.Add(window);
			return true;
		}
		return false;
	}

	public static bool TryRemoveWindow(PixelWindow window)
	{
		if (window == null || current?.visibleWindows == null)
		{
			return false;
		}
		if (current.visibleWindows.Contains(window) && !window.IsVisible())
		{
			current.visibleWindows.Remove(window);
			return true;
		}
		return false;
	}

	public static bool ContainsWindow(PixelWindow window)
	{
		if (window == null)
		{
			return false;
		}
		return current.visibleWindows.Contains(window);
	}

	public static void HideAllMovable()
	{
		current?.visibleWindows?.FindAll((PixelWindow w) => w.canMove).ForEach(delegate(PixelWindow w)
		{
			w.Hide();
		});
	}

	public static void HideAllNonImportantMovable()
	{
		current?.visibleWindows?.FindAll((PixelWindow w) => w.canMove && !w.isImportant).ForEach(delegate(PixelWindow w)
		{
			w.Hide();
		});
	}

	public bool Cancel()
	{
		return CloseLastClosableWindows();
	}
}

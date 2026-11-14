using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIFamilyLayer
{
	private const int FLOATING_RAISED_ORDER = 3;

	private static Canvas floatingCanvas;

	private static RectTransform floatingRoot;

	private static readonly List<RectTransform> adopted = new List<RectTransform>();

	private static readonly List<Transform> adoptedHomes = new List<Transform>();

	public static Canvas RaisedCanvas => floatingCanvas;

	private static Canvas WindowCanvas
	{
		get
		{
			if (RenderHandler.current == null)
			{
				return null;
			}
			return RenderHandler.current.windowUICanvas;
		}
	}

	private static bool EnsureCanvas()
	{
		if (floatingCanvas != null && floatingRoot != null)
		{
			return true;
		}
		if (WindowCanvas == null)
		{
			return false;
		}
		adopted.Clear();
		adoptedHomes.Clear();
		Canvas[] array = Object.FindObjectsOfType<Canvas>(includeInactive: true);
		foreach (Canvas canvas in array)
		{
			if (canvas.isRootCanvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay && canvas.sortingOrder >= 3)
			{
				canvas.sortingOrder++;
			}
		}
		GameObject gameObject = new GameObject("FloatingWindowCanvas", typeof(RectTransform));
		floatingCanvas = gameObject.AddComponent<Canvas>();
		floatingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		floatingCanvas.sortingOrder = 3;
		gameObject.AddComponent<GraphicRaycaster>();
		floatingRoot = (RectTransform)new GameObject("FloatingWindowRoot", typeof(RectTransform)).transform;
		floatingRoot.SetParent(floatingCanvas.transform, worldPositionStays: false);
		floatingRoot.anchorMin = Vector2.zero;
		floatingRoot.anchorMax = Vector2.one;
		floatingRoot.offsetMin = Vector2.zero;
		floatingRoot.offsetMax = Vector2.zero;
		floatingRoot.pivot = new Vector2(0f, 1f);
		SyncScale();
		return true;
	}

	public static void SetRaisedCanvasActive(bool active)
	{
		if (!(floatingCanvas == null))
		{
			floatingCanvas.gameObject.SetActive(active);
		}
	}

	public static void SyncScale()
	{
		if (!(floatingCanvas == null) && !(WindowCanvas == null))
		{
			floatingCanvas.scaleFactor = WindowCanvas.scaleFactor;
		}
	}

	public static void BringWindowsToFront(PixelWindow focused)
	{
		if (!EnsureCanvas())
		{
			return;
		}
		SyncScale();
		if (WindowsHandler.current != null)
		{
			foreach (PixelWindow visibleWindow in WindowsHandler.current.visibleWindows)
			{
				if (visibleWindow.canMove && visibleWindow != focused)
				{
					Adopt(visibleWindow.rectTransform);
				}
			}
		}
		if (focused != null && focused.canMove)
		{
			Adopt(focused.rectTransform);
		}
	}

	public static void BringPanelsToFront()
	{
		for (int i = 0; i < adopted.Count; i++)
		{
			RectTransform rectTransform = adopted[i];
			Transform transform = adoptedHomes[i];
			if (!(rectTransform == null) && !(transform == null) && !(rectTransform.parent != floatingRoot))
			{
				rectTransform.SetParent(transform, worldPositionStays: false);
				rectTransform.SetAsLastSibling();
			}
		}
		adopted.Clear();
		adoptedHomes.Clear();
	}

	public static void ReleaseWindow(PixelWindow window)
	{
		if (window == null || floatingRoot == null)
		{
			return;
		}
		int num = adopted.IndexOf(window.rectTransform);
		if (num >= 0)
		{
			RectTransform rectTransform = adopted[num];
			Transform transform = adoptedHomes[num];
			adopted.RemoveAt(num);
			adoptedHomes.RemoveAt(num);
			if (rectTransform != null && transform != null && rectTransform.parent == floatingRoot)
			{
				rectTransform.SetParent(transform, worldPositionStays: false);
			}
		}
	}

	private static void Adopt(RectTransform windowRect)
	{
		if (!(windowRect == null))
		{
			if (windowRect.parent == floatingRoot)
			{
				windowRect.SetAsLastSibling();
			}
			else if (!(LetterboxManager.DockRoot != null) || !(windowRect.parent == LetterboxManager.DockRoot))
			{
				adopted.Add(windowRect);
				adoptedHomes.Add(windowRect.parent);
				windowRect.SetParent(floatingRoot, worldPositionStays: false);
				windowRect.SetAsLastSibling();
			}
		}
	}
}

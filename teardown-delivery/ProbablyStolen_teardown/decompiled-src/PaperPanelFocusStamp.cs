using UnityEngine;

public class PaperPanelFocusStamp : MonoBehaviour
{
	public long stamp;

	private void OnEnable()
	{
		Bump();
	}

	public void Bump()
	{
		stamp = PixelWindow.NextFocusStamp();
		UIFamilyLayer.BringPanelsToFront();
	}

	public static long GetStamp(GameObject panel)
	{
		if (panel == null)
		{
			return 0L;
		}
		PaperPanelFocusStamp component = panel.GetComponent<PaperPanelFocusStamp>();
		if (!(component != null))
		{
			return 0L;
		}
		return component.stamp;
	}

	public static void BumpPanel(GameObject panel)
	{
		if (!(panel == null))
		{
			PaperPanelFocusStamp paperPanelFocusStamp = panel.GetComponent<PaperPanelFocusStamp>();
			if (paperPanelFocusStamp == null)
			{
				paperPanelFocusStamp = panel.AddComponent<PaperPanelFocusStamp>();
			}
			paperPanelFocusStamp.Bump();
			panel.transform.SetAsLastSibling();
		}
	}
}

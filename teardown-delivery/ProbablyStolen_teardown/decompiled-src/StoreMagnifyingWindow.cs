using UnityEngine;

public class StoreMagnifyingWindow : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (StoreUIManager.Instance.isMagnifyingMode)
		{
			if (KeyBindHelper.GetKeyDown("secondary_click"))
			{
				StoreUIManager.Instance.ExitMagnifyingMode();
			}
			else if (Input.GetMouseButtonDown(0) && !StoreUIManager.Instance.hoveringStoreUI && !UIDragHandler.isDraggingAnyUI)
			{
				StoreUIManager.Instance.ToggleMagnifyZoom();
			}
		}
	}
}

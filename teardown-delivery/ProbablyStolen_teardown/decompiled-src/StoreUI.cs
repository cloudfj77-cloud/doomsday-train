using System.Collections.Generic;
using UnityEngine;

public class StoreUI : MonoBehaviour
{
	public bool isPaperUI;

	public string identifier;

	[Header("Zoom (per-save)")]
	public bool zoomable;

	public Transform zoomRoot;

	public float zoomScaleNormal = 1f;

	public float zoomScaleAlt = 0.5f;

	public float zoomScaleAlt2 = 0.75f;

	private const int ZoomLevelCount = 3;

	private int currentZoomLevel = 1;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnEnable()
	{
		if (isPaperUI)
		{
			base.transform.SetAsLastSibling();
		}
		if (zoomable)
		{
			ApplyZoom(GetSavedZoom());
		}
	}

	private int GetSavedZoom()
	{
		if (string.IsNullOrEmpty(identifier) || PlayerStore.Instance == null)
		{
			return currentZoomLevel;
		}
		Dictionary<string, int> storeUIZoomLevels = PlayerStore.Instance.storeUIZoomLevels;
		if (storeUIZoomLevels == null || !storeUIZoomLevels.TryGetValue(identifier, out var value))
		{
			return 1;
		}
		return value;
	}

	public void ToggleZoom()
	{
		int num = GetSavedZoom() % 3 + 1;
		if (!string.IsNullOrEmpty(identifier) && PlayerStore.Instance != null)
		{
			PlayerStore.Instance.storeUIZoomLevels[identifier] = num;
		}
		ApplyZoom(num);
	}

	private void ApplyZoom(int level)
	{
		currentZoomLevel = level;
		Transform obj = ((zoomRoot != null) ? zoomRoot : base.transform);
		float num = level switch
		{
			3 => zoomScaleAlt2, 
			2 => zoomScaleAlt, 
			_ => zoomScaleNormal, 
		};
		if (obj == base.transform && TryGetComponent<UIDragHandler>(out var component))
		{
			component.SetHomeScale(num);
			num *= component.DockScaleRatio;
		}
		obj.localScale = new Vector3(num, num, obj.localScale.z);
	}

	public void OnDisable()
	{
		if (isPaperUI)
		{
			base.transform.SetAsFirstSibling();
		}
		if (StoreUIManager.Instance != null)
		{
			StoreUIManager.Instance.CloseUI(identifier);
		}
	}
}

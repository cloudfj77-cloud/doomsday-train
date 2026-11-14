using System;
using UnityEngine;

namespace Duckov.CustomOptions;

public class CustomOptionsPanel : MonoBehaviour
{
	private RectTransform rectTransform;

	public static Action<RectTransform> OnPanelEnabled;

	private void OnEnable()
	{
		if (rectTransform == null)
		{
			rectTransform = base.transform as RectTransform;
			if (rectTransform == null)
			{
				Debug.LogError("[Custom Options] Failed to convert to rect transform.");
				return;
			}
		}
		OnPanelEnabled?.Invoke(rectTransform);
	}

	private void OnDisable()
	{
	}
}

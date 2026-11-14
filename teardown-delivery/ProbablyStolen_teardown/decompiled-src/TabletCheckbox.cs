using System;
using UnityEngine;
using UnityEngine.UI;

public class TabletCheckbox : MonoBehaviour
{
	public Image frame;

	public Image fill;

	public Button button;

	public Action<bool> onChanged;

	private bool isOn;

	private bool interactable = true;

	public bool IsOn => isOn;

	public void SetOn(bool value)
	{
		isOn = value;
		if (fill != null)
		{
			fill.enabled = value;
		}
	}

	public void SetInteractable(bool value)
	{
		interactable = value;
		if (button != null)
		{
			button.interactable = value;
		}
	}

	public void Toggle()
	{
		if (interactable)
		{
			SetOn(!isOn);
			onChanged?.Invoke(isOn);
		}
	}
}

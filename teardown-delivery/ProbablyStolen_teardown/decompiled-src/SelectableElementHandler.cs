using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectableElementHandler : EmptyActionHandler
{
	public static SelectableElementHandler current;

	[SerializeField]
	public KeyCode key;

	private SelectableElement currentMouseElement;

	private SelectableElement currentPressElement;

	private bool holdActive;

	private float holdStartTime;

	public override List<KeyCode> keyListeners => new KeyCode[1] { key }.ToList();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		SelectableElement selectableElement = RenderHandler.RaycastElement<SelectableElement>(Input.mousePosition);
		if (selectableElement != null && (selectableElement.isDisabled || selectableElement.isHidden))
		{
			selectableElement = null;
		}
		if (StoreUIManager.Instance.hoveringStoreUI)
		{
			selectableElement = null;
		}
		if (selectableElement != null && selectableElement == currentMouseElement)
		{
			selectableElement.Press();
			BeginHoldIfNeeded(selectableElement);
		}
		currentPressElement = selectableElement;
	}

	protected override void OnEventRelease(ISet<KeyCode> keysPressed)
	{
		if (currentPressElement != null && currentPressElement == currentMouseElement)
		{
			if (!holdActive || !currentPressElement.requiresHold || IsHoldComplete(currentPressElement))
			{
				if (currentPressElement.isSelected)
				{
					currentPressElement.Deselect();
				}
				else
				{
					currentPressElement.Select();
				}
			}
			currentMouseElement?.Defocus();
			currentMouseElement = null;
		}
		CancelHold();
		currentPressElement?.Defocus();
		currentPressElement = null;
	}

	private void BeginHoldIfNeeded(SelectableElement element)
	{
		if (element != null && element.requiresHold)
		{
			holdActive = true;
			holdStartTime = Time.unscaledTime;
			CustomUIManager.Instance?.ShowHoldRing(0f);
		}
		else
		{
			holdActive = false;
		}
	}

	private bool IsHoldComplete(SelectableElement element)
	{
		return Time.unscaledTime - holdStartTime >= element.holdDuration;
	}

	private void UpdateHold()
	{
		if (holdActive)
		{
			if (currentPressElement == null || currentPressElement != currentMouseElement || currentPressElement.isDisabled || currentPressElement.isHidden)
			{
				CancelHold();
				return;
			}
			float progress = Mathf.Clamp01((Time.unscaledTime - holdStartTime) / currentPressElement.holdDuration);
			CustomUIManager.Instance?.ShowHoldRing(progress);
		}
	}

	private void CancelHold()
	{
		if (holdActive)
		{
			holdActive = false;
			CustomUIManager.Instance?.HideHoldRing();
		}
	}

	protected override bool OnEventUpdate()
	{
		SelectableElement selectableElement = RenderHandler.RaycastElement<SelectableElement>(Input.mousePosition);
		if (selectableElement != null && (selectableElement.isDisabled || selectableElement.isHidden))
		{
			selectableElement = null;
		}
		if (StoreUIManager.Instance.hoveringStoreUI)
		{
			selectableElement = null;
		}
		if (selectableElement != currentMouseElement)
		{
			if (currentMouseElement != null)
			{
				currentMouseElement.Defocus();
			}
			if (selectableElement != null)
			{
				if (currentPressElement == selectableElement)
				{
					selectableElement.Press();
				}
				else
				{
					selectableElement.Hover();
				}
			}
		}
		currentMouseElement = selectableElement;
		UpdateHold();
		return base.OnEventUpdate();
	}

	protected override void OnEventReset()
	{
		currentMouseElement?.Defocus();
		currentPressElement?.Defocus();
		currentMouseElement = null;
		currentPressElement = null;
		CancelHold();
	}
}

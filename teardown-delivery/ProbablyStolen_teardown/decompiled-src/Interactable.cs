using System.Linq;
using EPOOutline;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Outlinable))]
public class Interactable : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
	public bool isControlable = true;

	public bool isAvailable = true;

	private Outlinable outlinable;

	public bool doNotOutline;

	public static bool isPaused = false;

	private static Color HOVER_COLOR = new Color32(225, 209, 156, byte.MaxValue);

	private static Color SELECT_COLOR = Color.green;

	private static Color UNAVAILABLE_COLOR = Color.red;

	private static Color OUT_OF_RANGE_COLOR = Color.gray;

	private bool isCurrentlySelected;

	private float clicked;

	private float clicktime;

	private float clickdelay = 0.5f;

	public static bool isHoveringInteractable = false;

	public static Interactable currentHoveringInteractable;

	private bool holdActive;

	private float holdStartTime;

	private float holdDuration = 1.25f;

	private bool isPointerOver;

	public void Awake()
	{
		outlinable = GetComponent<Outlinable>();
		outlinable.OutlineParameters.BlurShift = 0f;
		outlinable.OutlineParameters.DilateShift = 1f;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		DetectDoubleClick();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		HandleClick();
	}

	public void HandleClick()
	{
		if (isPaused)
		{
			return;
		}
		if (isControlable)
		{
			if (isAvailable)
			{
				OnSelected();
			}
		}
		else if (isAvailable && !TryBeginHold())
		{
			OnClick();
		}
	}

	private bool TryBeginHold()
	{
		if (!base.gameObject.TryGetComponent<IHoldClickable>(out var component))
		{
			return false;
		}
		if (!component.ShouldRequireHold())
		{
			return false;
		}
		holdActive = true;
		holdStartTime = Time.unscaledTime;
		holdDuration = ((component.HoldDuration > 0f) ? component.HoldDuration : 1.25f);
		isPointerOver = true;
		CustomUIManager.Instance?.ShowHoldRing(0f);
		return true;
	}

	private void CancelHold()
	{
		if (holdActive)
		{
			holdActive = false;
			CustomUIManager.Instance?.HideHoldRing();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (holdActive)
		{
			bool num = Time.unscaledTime - holdStartTime >= holdDuration;
			CancelHold();
			if (num && isPointerOver && isAvailable && !isPaused)
			{
				OnClick();
			}
		}
	}

	public void OnDoubleClick()
	{
		if (isControlable)
		{
			OnDeselected();
		}
		if (isAvailable && base.gameObject.TryGetComponent<IDoubleClickable>(out var component))
		{
			component.OnDoubleClick();
		}
	}

	public void OnClick()
	{
		if (base.gameObject.TryGetComponent<IClickable>(out var component))
		{
			component.OnClick();
		}
	}

	public void DetectDoubleClick(PointerEventData eventData)
	{
		if (eventData.clickCount == 2)
		{
			OnDoubleClick();
		}
	}

	public void DetectDoubleClick()
	{
		clicked += 1f;
		if (clicked == 1f)
		{
			clicktime = Time.time;
		}
		if (clicked > 1f && Time.time - clicktime < clickdelay)
		{
			clicked = 0f;
			clicktime = 0f;
			OnDoubleClick();
		}
		else if (clicked > 2f || Time.time - clicktime > 1f)
		{
			clicked = 0f;
		}
	}

	public static void DeselectCurrents()
	{
		RTSController.previousSelectedObjects.Clear();
		foreach (GameObject item in RTSController.selectedObjects.ToList())
		{
			InteractableUI component2;
			if (item.TryGetComponent<Interactable>(out var component))
			{
				RTSController.previousSelectedObjects.Add(component.gameObject);
				component.OnDeselected();
			}
			else if (item.TryGetComponent<InteractableUI>(out component2))
			{
				RTSController.previousSelectedObjects.Add(component2.gameObject);
				component2.OnDeselected();
			}
		}
	}

	public void OnSelected(bool emptyBefore = false)
	{
		if (emptyBefore)
		{
			DeselectCurrents();
		}
		RTSController.selectedObjects.Add(base.gameObject);
		isCurrentlySelected = true;
		if (!doNotOutline && !PlayerStore.Instance.cinematicMode)
		{
			outlinable.enabled = true;
			outlinable.OutlineParameters.Color = SELECT_COLOR;
		}
		EmporiumRTSController.Instance.OnSelected();
	}

	public static void OnSelected(GameObject targetGameObject)
	{
		if (targetGameObject.TryGetComponent<Interactable>(out var component))
		{
			RTSController.selectedObjects.Add(targetGameObject);
			component.isCurrentlySelected = true;
			component.outlinable.enabled = true;
			component.outlinable.OutlineParameters.Color = SELECT_COLOR;
		}
	}

	public void OnDeselected()
	{
		if (!doNotOutline && !PlayerStore.Instance.cinematicMode)
		{
			outlinable.enabled = false;
		}
		isCurrentlySelected = false;
		RTSController.selectedObjects.Remove(base.gameObject);
		EmporiumRTSController.Instance.OnDeselected();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (isPaused)
		{
			return;
		}
		if (!isCurrentlySelected && !doNotOutline && PlayerStore.IsInstanceExist() && !PlayerStore.Instance.cinematicMode)
		{
			outlinable.enabled = true;
			if (!isAvailable)
			{
				outlinable.OutlineParameters.Color = UNAVAILABLE_COLOR;
			}
			else
			{
				outlinable.OutlineParameters.Color = HOVER_COLOR;
			}
		}
		isHoveringInteractable = true;
		currentHoveringInteractable = this;
		isPointerOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!isCurrentlySelected && !doNotOutline && PlayerStore.IsInstanceExist() && !PlayerStore.Instance.cinematicMode)
		{
			outlinable.enabled = false;
		}
		isHoveringInteractable = false;
		currentHoveringInteractable = null;
		isPointerOver = false;
	}

	private void Start()
	{
		if (outlinable != null)
		{
			outlinable.enabled = false;
		}
	}

	private void Update()
	{
		UpdateHold();
	}

	private void UpdateHold()
	{
		if (holdActive)
		{
			if (!isPointerOver || isPaused || !isAvailable)
			{
				CancelHold();
				return;
			}
			float progress = Mathf.Clamp01((Time.unscaledTime - holdStartTime) / holdDuration);
			CustomUIManager.Instance?.ShowHoldRing(progress);
		}
	}

	private void OnDisable()
	{
		CancelHold();
	}

	public static void TryDeselectObject(GameObject currentObject)
	{
		if (currentObject.TryGetComponent<Interactable>(out var component))
		{
			component.OnDeselected();
		}
	}
}

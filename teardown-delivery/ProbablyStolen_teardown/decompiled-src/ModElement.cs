using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ModElement : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	public TextMeshProUGUI modNameTMP;

	private RectTransform rectTransform;

	private CanvasGroup canvasGroup;

	private Canvas rootCanvas;

	private Transform activeParent;

	private GameObject placeholder;

	private bool isDragging;

	private float lastClickTime;

	private const float DoubleClickThreshold = 0.3f;

	public string ModID { get; private set; }

	public string ModName { get; private set; }

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void Init(string id, string name)
	{
		ModID = id;
		ModName = name;
		ScriptFontSwap.Apply(modNameTMP, name);
	}

	public void OnClick()
	{
		float num = Time.time - lastClickTime;
		lastClickTime = Time.time;
		if (num <= 0.3f)
		{
			ModUIManager.Instance.OnModElementDoubleClicked(ModID, this);
		}
		else
		{
			ModUIManager.Instance.OnModElementClicked(ModID, this);
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!(ModUIManager.Instance == null) && !ModUIManager.Instance.IsFiltering)
		{
			activeParent = ModUIManager.Instance.activeModLayoutObject.transform;
			if (!(base.transform.parent != activeParent))
			{
				isDragging = true;
				rootCanvas = GetComponentInParent<Canvas>();
				placeholder = new GameObject("DragPlaceholder");
				placeholder.transform.SetParent(activeParent, worldPositionStays: false);
				placeholder.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, rectTransform.rect.height);
				LayoutElement layoutElement = placeholder.AddComponent<LayoutElement>();
				layoutElement.preferredHeight = rectTransform.rect.height;
				layoutElement.flexibleWidth = 1f;
				placeholder.transform.SetSiblingIndex(base.transform.GetSiblingIndex());
				base.transform.SetParent(rootCanvas.transform, worldPositionStays: true);
				canvasGroup.blocksRaycasts = false;
				canvasGroup.alpha = 0.7f;
			}
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (isDragging)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localPoint);
			rectTransform.localPosition = localPoint;
			placeholder.transform.SetSiblingIndex(GetInsertIndex(eventData.position));
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (isDragging)
		{
			isDragging = false;
			canvasGroup.alpha = 1f;
			canvasGroup.blocksRaycasts = true;
			base.transform.SetParent(activeParent, worldPositionStays: false);
			base.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
			Object.Destroy(placeholder);
			placeholder = null;
			activeParent = null;
			ModUIManager.Instance.OnModOrderChanged();
		}
	}

	private int GetInsertIndex(Vector2 screenPos)
	{
		for (int i = 0; i < activeParent.childCount; i++)
		{
			Transform child = activeParent.GetChild(i);
			if (!(child.gameObject == placeholder) && screenPos.y > (child as RectTransform).position.y)
			{
				return i;
			}
		}
		return activeParent.childCount - 1;
	}
}

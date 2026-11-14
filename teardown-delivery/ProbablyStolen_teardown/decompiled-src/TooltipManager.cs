using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
	public static TooltipManager _instance;

	public TextMeshProUGUI content;

	private RectTransform rectTransform;

	private Canvas tooltipCanvas;

	public GameObject tooltipGameobject;

	private Transform tooltipHomeParent;

	private int tooltipHomeSiblingIndex;

	private Vector3 tooltipHomeScale;

	private Component tooltipOwner;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
		rectTransform = tooltipGameobject.GetComponent<RectTransform>();
		tooltipCanvas = tooltipGameobject.GetComponentInParent<Canvas>();
		tooltipHomeParent = tooltipGameobject.transform.parent;
		tooltipHomeSiblingIndex = tooltipGameobject.transform.GetSiblingIndex();
		tooltipHomeScale = tooltipGameobject.transform.localScale;
	}

	private bool EnsureTooltipLayer()
	{
		Transform transform = tooltipGameobject.transform;
		RectTransform dockRoot = LetterboxManager.DockRoot;
		Transform transform2 = ((dockRoot != null) ? dockRoot : tooltipHomeParent);
		if (transform2 != null && transform.parent != transform2)
		{
			transform.SetParent(transform2, worldPositionStays: true);
			Vector3 localPosition = transform.localPosition;
			localPosition.z = 0f;
			transform.localPosition = localPosition;
			if (transform2 == tooltipHomeParent)
			{
				transform.SetSiblingIndex(tooltipHomeSiblingIndex);
				transform.localScale = tooltipHomeScale;
			}
			tooltipCanvas = tooltipGameobject.GetComponentInParent<Canvas>();
		}
		bool result = dockRoot != null && transform.parent == dockRoot;
		transform.SetAsLastSibling();
		return result;
	}

	private void Start()
	{
		Cursor.visible = true;
	}

	private void Update()
	{
		if (tooltipGameobject.activeSelf)
		{
			if (IsOwnerGone())
			{
				HideTooltip();
			}
			else
			{
				UpdateTooltipPosition();
			}
		}
	}

	private bool IsOwnerGone()
	{
		if ((object)tooltipOwner == null)
		{
			return false;
		}
		if (tooltipOwner == null)
		{
			return true;
		}
		if (!tooltipOwner.gameObject.activeInHierarchy)
		{
			return true;
		}
		if (tooltipOwner is Behaviour behaviour)
		{
			return !behaviour.enabled;
		}
		return false;
	}

	public void SetAndShowTooltip(string message)
	{
		SetAndShowTooltip(message, null);
	}

	public void SetAndShowTooltip(string message, Component owner)
	{
		tooltipOwner = owner;
		tooltipGameobject.SetActive(value: true);
		content.text = message;
		float x = ((content.GetPreferredValues(message).x > 240f) ? 400f : 240f);
		rectTransform.sizeDelta = new Vector2(x, rectTransform.sizeDelta.y);
		UpdateTooltipPosition();
	}

	public void HideTooltip()
	{
		tooltipOwner = null;
		if (tooltipGameobject != null)
		{
			tooltipGameobject.SetActive(value: false);
		}
	}

	public void HideTooltip(Component owner)
	{
		if (!(owner != null) || (object)tooltipOwner == null || (object)tooltipOwner == owner)
		{
			HideTooltip();
		}
	}

	public void UpdateTooltipPosition()
	{
		bool num = EnsureTooltipLayer();
		float num2 = (num ? tooltipGameobject.transform.lossyScale.x : ((tooltipCanvas != null) ? tooltipCanvas.scaleFactor : 1f));
		Vector3 position = (num ? UIHelper.GetTooltipPositionScreenBounds(rectTransform.rect.width * num2, rectTransform.rect.height * num2) : UIHelper.GetTooltipPosition(rectTransform.rect.width * num2, rectTransform.rect.height * num2));
		position.z = tooltipGameobject.transform.position.z;
		tooltipGameobject.transform.position = position;
	}
}

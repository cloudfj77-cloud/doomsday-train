using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTextColor : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public TextMeshProUGUI text;

	private readonly Color normal = new Color32(138, 191, 187, byte.MaxValue);

	private readonly Color highlight = new Color32(224, 208, 155, byte.MaxValue);

	private readonly Color pressed = new Color32(69, 145, 139, byte.MaxValue);

	private bool isHovered;

	private void Awake()
	{
		ResetState();
	}

	private void OnDisable()
	{
		ResetState();
	}

	private void ResetState()
	{
		isHovered = false;
		if (text != null)
		{
			text.color = normal;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isHovered = true;
		text.color = highlight;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHovered = false;
		text.color = normal;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		text.color = pressed;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		text.color = (isHovered ? highlight : normal);
	}
}

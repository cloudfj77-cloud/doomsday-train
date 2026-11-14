using TMPro;
using UnityEngine;

public class StoreCreditDisplay : MonoBehaviour, IClickable
{
	public TextMeshProUGUI creditDisplayText;

	public Vector2 DefaultAnchoredPosition { get; private set; }

	public Color DefaultColor { get; private set; }

	public void OnClick()
	{
		AdvCalendarUIManager.Instance.OpenUI();
		AdvCalendarUIManager.Instance.OnStatusButtonClicked();
	}

	private void Start()
	{
		if (creditDisplayText != null)
		{
			DefaultAnchoredPosition = creditDisplayText.rectTransform.anchoredPosition;
			DefaultColor = creditDisplayText.color;
		}
	}

	private void Update()
	{
		if (creditDisplayText != null)
		{
			creditDisplayText.text = PlayerStore.Instance.GetCash() + GeneralHelper.CreditIcon();
		}
	}
}

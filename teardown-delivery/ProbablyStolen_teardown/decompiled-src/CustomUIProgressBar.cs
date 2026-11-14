using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomUIProgressBar : MonoBehaviour
{
	public Image fillImage;

	[Tooltip("Optional text over the bar, e.g. \"45%\".")]
	public TextMeshProUGUI label;

	public void SetProgress(float normalized)
	{
		normalized = Mathf.Clamp01(normalized);
		if (fillImage != null)
		{
			if (fillImage.type == Image.Type.Filled)
			{
				fillImage.fillAmount = normalized;
				return;
			}
			Vector3 localScale = fillImage.rectTransform.localScale;
			fillImage.rectTransform.localScale = new Vector3(normalized, localScale.y, localScale.z);
		}
	}

	public void SetLabel(string text)
	{
		if (label != null)
		{
			label.text = text;
		}
	}
}

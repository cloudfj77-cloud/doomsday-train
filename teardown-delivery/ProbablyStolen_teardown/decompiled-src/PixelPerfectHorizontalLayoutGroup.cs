using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Layout/Pixel Perfect Horizontal Layout Group")]
public class PixelPerfectHorizontalLayoutGroup : HorizontalLayoutGroup
{
	[Header("Pixel Perfect Settings")]
	[SerializeField]
	private bool snapToPixels = true;

	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
		SnapChildrenToPixels();
	}

	public override void CalculateLayoutInputVertical()
	{
		base.CalculateLayoutInputVertical();
	}

	public override void SetLayoutHorizontal()
	{
		base.SetLayoutHorizontal();
		SnapChildrenToPixels();
	}

	public override void SetLayoutVertical()
	{
		base.SetLayoutVertical();
		SnapChildrenToPixels();
	}

	private void SnapChildrenToPixels()
	{
		if (!snapToPixels)
		{
			return;
		}
		for (int i = 0; i < base.rectTransform.childCount; i++)
		{
			RectTransform rectTransform = base.rectTransform.GetChild(i) as RectTransform;
			if (!(rectTransform == null))
			{
				Vector2 anchoredPosition = rectTransform.anchoredPosition;
				anchoredPosition.x = Mathf.Round(anchoredPosition.x);
				anchoredPosition.y = Mathf.Round(anchoredPosition.y);
				rectTransform.anchoredPosition = anchoredPosition;
				Vector2 sizeDelta = rectTransform.sizeDelta;
				sizeDelta.x = Mathf.Round(sizeDelta.x);
				sizeDelta.y = Mathf.Round(sizeDelta.y);
				rectTransform.sizeDelta = sizeDelta;
			}
		}
	}
}

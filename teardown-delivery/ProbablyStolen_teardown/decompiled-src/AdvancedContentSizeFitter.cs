using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class AdvancedContentSizeFitter : ContentSizeFitter
{
	public enum FitModeWithMax
	{
		Unconstrained,
		PreferredSizeWithMaxConstraint
	}

	[Header("Advanced Fit Mode Settings")]
	public FitModeWithMax horizontalFitWithMax;

	public FitModeWithMax verticalFitWithMax;

	[SerializeField]
	public float maxWidth = float.PositiveInfinity;

	[SerializeField]
	public float maxHeight = float.PositiveInfinity;

	private RectTransform rectTransform;

	protected override void OnEnable()
	{
		base.OnEnable();
		rectTransform = GetComponent<RectTransform>();
		AdjustSize();
	}

	private void Update()
	{
		AdjustSize();
	}

	private void AdjustSize()
	{
		if (!(rectTransform == null))
		{
			if (horizontalFitWithMax == FitModeWithMax.PreferredSizeWithMaxConstraint)
			{
				float size = Mathf.Min(1f, maxWidth);
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			}
			if (verticalFitWithMax == FitModeWithMax.PreferredSizeWithMaxConstraint)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);
			}
		}
	}
}

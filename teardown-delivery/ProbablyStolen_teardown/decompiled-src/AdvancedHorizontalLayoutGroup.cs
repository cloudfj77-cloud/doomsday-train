using UnityEngine;
using UnityEngine.UI;

public class AdvancedHorizontalLayoutGroup : LayoutGroup
{
	public enum JustifyOptions
	{
		Left,
		Right,
		Center,
		EvenlyDistributed
	}

	public float spacing = 10f;

	public bool childForceExpandWidth;

	public bool childForceExpandHeight;

	public bool childControlWidth = true;

	public bool childControlHeight = true;

	public new TextAnchor childAlignment;

	public JustifyOptions justifyContent;

	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
		SetLayoutAlongAxis(0);
	}

	public override void CalculateLayoutInputVertical()
	{
		SetLayoutAlongAxis(1);
	}

	public override void SetLayoutHorizontal()
	{
		SetLayoutAlongAxis(0);
	}

	public override void SetLayoutVertical()
	{
		SetLayoutAlongAxis(1);
	}

	private void SetLayoutAlongAxis(int axis)
	{
		float width = base.rectTransform.rect.width;
		float height = base.rectTransform.rect.height;
		float num = base.padding.left;
		float num2 = height - (float)base.padding.vertical;
		float num3 = 0f;
		for (int i = 0; i < base.rectChildren.Count; i++)
		{
			RectTransform rect = base.rectChildren[i];
			float num4 = LayoutUtility.GetPreferredSize(rect, 0);
			float preferredSize = LayoutUtility.GetPreferredSize(rect, 1);
			if (childControlWidth)
			{
				num4 = Mathf.Max(num4, LayoutUtility.GetMinSize(rect, 0));
			}
			if (childControlHeight)
			{
				preferredSize = Mathf.Max(preferredSize, LayoutUtility.GetMinSize(rect, 1));
			}
			num3 += num4 + ((i < base.rectChildren.Count - 1) ? spacing : 0f);
		}
		switch (justifyContent)
		{
		case JustifyOptions.Right:
			num = width - num3 - (float)base.padding.right;
			break;
		case JustifyOptions.Center:
			num = (width - num3) / 2f;
			break;
		case JustifyOptions.EvenlyDistributed:
			spacing = (width - num3) / (float)(base.rectChildren.Count + 1);
			num = (float)base.padding.left + spacing;
			break;
		}
		for (int j = 0; j < base.rectChildren.Count; j++)
		{
			RectTransform rect2 = base.rectChildren[j];
			float num5 = LayoutUtility.GetPreferredSize(rect2, 0);
			float num6 = LayoutUtility.GetPreferredSize(rect2, 1);
			if (childControlWidth)
			{
				num5 = Mathf.Max(num5, LayoutUtility.GetMinSize(rect2, 0));
			}
			if (childControlHeight)
			{
				num6 = Mathf.Max(num6, LayoutUtility.GetMinSize(rect2, 1));
			}
			if (childForceExpandWidth)
			{
				num5 = (width - (float)base.padding.horizontal - (float)(base.rectChildren.Count - 1) * spacing) / (float)base.rectChildren.Count;
			}
			if (childForceExpandHeight)
			{
				num6 = num2;
			}
			float pos = 0f;
			switch (childAlignment)
			{
			case TextAnchor.UpperLeft:
			case TextAnchor.UpperCenter:
			case TextAnchor.UpperRight:
				pos = base.padding.top;
				break;
			case TextAnchor.MiddleLeft:
			case TextAnchor.MiddleCenter:
			case TextAnchor.MiddleRight:
				pos = (height - num6) / 2f;
				break;
			case TextAnchor.LowerLeft:
			case TextAnchor.LowerCenter:
			case TextAnchor.LowerRight:
				pos = height - (float)base.padding.bottom - num6;
				break;
			}
			SetChildAlongAxis(rect2, 0, num, num5);
			SetChildAlongAxis(rect2, 1, pos, num6);
			num += num5 + spacing;
		}
	}
}

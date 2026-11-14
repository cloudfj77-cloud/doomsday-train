using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedHorizontalFlowLayoutGroup : LayoutGroup
{
	public enum JustifyOptions
	{
		Left,
		Right,
		Center,
		EvenlyDistributed
	}

	public float spacingX = 10f;

	public float spacingY = 10f;

	public bool childForceExpandWidth;

	public bool childForceExpandHeight;

	public bool uniformRowHeight;

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
		float num = base.padding.left;
		float num2 = base.padding.top;
		List<List<RectTransform>> list = new List<List<RectTransform>>();
		List<float> list2 = new List<float>();
		float num3 = 0f;
		List<RectTransform> list3 = new List<RectTransform>();
		for (int i = 0; i < base.rectChildren.Count; i++)
		{
			RectTransform rectTransform = base.rectChildren[i];
			float num4 = LayoutUtility.GetPreferredSize(rectTransform, 0) + spacingX;
			float b = LayoutUtility.GetPreferredSize(rectTransform, 1) + spacingY;
			if (num + num4 > width - (float)base.padding.right)
			{
				list.Add(list3);
				list2.Add(num3);
				list3 = new List<RectTransform>();
				num = base.padding.left;
				num2 += num3 + spacingY;
				num3 = 0f;
			}
			list3.Add(rectTransform);
			num += num4;
			num3 = Mathf.Max(num3, b);
		}
		if (list3.Count > 0)
		{
			list.Add(list3);
			list2.Add(num3);
		}
		num2 = base.padding.top;
		for (int j = 0; j < list.Count; j++)
		{
			float num5 = list2[j];
			List<RectTransform> list4 = list[j];
			num = CalculateStartXForRow(list4, num5, width);
			for (int k = 0; k < list4.Count; k++)
			{
				RectTransform rect = list4[k];
				float num6 = LayoutUtility.GetPreferredSize(rect, 0);
				float num7 = LayoutUtility.GetPreferredSize(rect, 1);
				if (childForceExpandWidth)
				{
					num6 = (width - (float)base.padding.horizontal - (float)(list4.Count - 1) * spacingX) / (float)list4.Count;
				}
				if (childForceExpandHeight)
				{
					num7 = num5 - spacingY;
				}
				float pos = 0f;
				switch (childAlignment)
				{
				case TextAnchor.UpperLeft:
				case TextAnchor.UpperCenter:
				case TextAnchor.UpperRight:
					pos = num2;
					break;
				case TextAnchor.MiddleLeft:
				case TextAnchor.MiddleCenter:
				case TextAnchor.MiddleRight:
					pos = num2 + (num5 - num7) / 2f;
					break;
				case TextAnchor.LowerLeft:
				case TextAnchor.LowerCenter:
				case TextAnchor.LowerRight:
					pos = num2 + num5 - num7;
					break;
				}
				SetChildAlongAxis(rect, 0, num, num6);
				SetChildAlongAxis(rect, 1, pos, num7);
				num += num6 + spacingX;
			}
			num2 += num5 + spacingY;
		}
	}

	private float CalculateStartXForRow(List<RectTransform> row, float rowHeight, float parentWidth)
	{
		float num = 0f;
		for (int i = 0; i < row.Count; i++)
		{
			float num2 = LayoutUtility.GetPreferredSize(row[i], 0) + spacingX;
			num += num2;
		}
		num -= spacingX;
		float result = base.padding.left;
		switch (justifyContent)
		{
		case JustifyOptions.Left:
			result = base.padding.left;
			break;
		case JustifyOptions.Right:
			result = parentWidth - (float)base.padding.right - num;
			break;
		case JustifyOptions.Center:
			result = (parentWidth - num) / 2f;
			break;
		case JustifyOptions.EvenlyDistributed:
			spacingX = (parentWidth - (float)base.padding.left - (float)base.padding.right - num) / (float)(row.Count - 1);
			result = base.padding.left;
			break;
		}
		return result;
	}
}

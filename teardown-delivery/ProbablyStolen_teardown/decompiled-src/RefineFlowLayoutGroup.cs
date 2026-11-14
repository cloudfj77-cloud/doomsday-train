using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefineFlowLayoutGroup : LayoutGroup
{
	public enum JustifyOptions
	{
		Left,
		Right,
		Center,
		EvenlyDistributed
	}

	private struct Row
	{
		public List<RectTransform> children;

		public float height;

		public float contentWidth;
	}

	public float spacingX = 10f;

	public float spacingY = 10f;

	public bool childForceExpandWidth;

	public bool childForceExpandHeight;

	public bool uniformRowHeight;

	public TextAnchor verticalChildAlignment;

	public JustifyOptions justifyContent;

	private List<Row> cachedRows;

	private float cachedParentWidth = -1f;

	private bool layoutDirty = true;

	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
		layoutDirty = true;
		SetLayoutInputForAxis(base.padding.horizontal, base.padding.horizontal, -1f, 0);
	}

	public override void CalculateLayoutInputVertical()
	{
		layoutDirty = true;
		SetLayoutInputForAxis(base.padding.vertical, base.padding.vertical, -1f, 1);
	}

	public override void SetLayoutHorizontal()
	{
		LayoutChildren();
	}

	public override void SetLayoutVertical()
	{
		if (layoutDirty)
		{
			LayoutChildren();
		}
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		layoutDirty = true;
	}

	private void LayoutChildren()
	{
		float width = base.rectTransform.rect.width;
		int count = base.rectChildren.Count;
		if (width <= 0f || count == 0 || width <= (float)base.padding.horizontal)
		{
			return;
		}
		if (layoutDirty || cachedParentWidth != width)
		{
			float[] array = new float[count];
			float[] array2 = new float[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = LayoutUtility.GetPreferredSize(base.rectChildren[i], 0);
				array2[i] = LayoutUtility.GetPreferredSize(base.rectChildren[i], 1);
			}
			cachedRows = BuildRows(width, array, array2);
			cachedParentWidth = width;
			layoutDirty = false;
		}
		if (cachedRows == null || cachedRows.Count == 0)
		{
			return;
		}
		float num = 0f;
		if (uniformRowHeight)
		{
			foreach (Row cachedRow in cachedRows)
			{
				num = Mathf.Max(num, cachedRow.height);
			}
		}
		float num2 = base.padding.top;
		foreach (Row cachedRow2 in cachedRows)
		{
			if (cachedRow2.children == null || cachedRow2.children.Count == 0)
			{
				continue;
			}
			float num3 = (uniformRowHeight ? num : cachedRow2.height);
			float num4 = spacingX;
			if (justifyContent == JustifyOptions.EvenlyDistributed && cachedRow2.children.Count > 1)
			{
				num4 = (width - (float)base.padding.horizontal - cachedRow2.contentWidth) / (float)(cachedRow2.children.Count - 1);
				num4 = Mathf.Max(0f, num4);
			}
			float num5 = CalculateRowStartX(width, cachedRow2.contentWidth, num4, cachedRow2.children.Count);
			float num6 = 0f;
			if (childForceExpandWidth && cachedRow2.children.Count > 0)
			{
				float num7 = width - (float)base.padding.horizontal;
				if (cachedRow2.children.Count > 1)
				{
					num7 -= num4 * (float)(cachedRow2.children.Count - 1);
				}
				num6 = num7 / (float)cachedRow2.children.Count;
				num6 = Mathf.Max(0f, num6);
			}
			foreach (RectTransform child in cachedRow2.children)
			{
				if (!(child == null))
				{
					float num8 = (childForceExpandWidth ? num6 : LayoutUtility.GetPreferredSize(child, 0));
					float num9 = (childForceExpandHeight ? num3 : LayoutUtility.GetPreferredSize(child, 1));
					float verticalOffset = GetVerticalOffset(num2, num3, num9);
					SetChildAlongAxis(child, 0, num5, num8);
					SetChildAlongAxis(child, 1, verticalOffset, num9);
					num5 += num8 + num4;
				}
			}
			num2 += num3 + spacingY;
		}
	}

	private List<Row> BuildRows(float parentWidth, float[] widths, float[] heights)
	{
		List<Row> list = new List<Row>();
		float num = parentWidth - (float)base.padding.horizontal;
		Row row = default(Row);
		row.children = new List<RectTransform>();
		row.height = 0f;
		row.contentWidth = 0f;
		Row item = row;
		for (int i = 0; i < base.rectChildren.Count; i++)
		{
			if (!(base.rectChildren[i] == null))
			{
				float num2 = widths[i];
				float num3 = item.contentWidth + num2;
				if (item.children.Count > 0)
				{
					num3 += spacingX;
				}
				if (num3 > num && item.children.Count > 0)
				{
					list.Add(item);
					row = default(Row);
					row.children = new List<RectTransform>();
					row.height = 0f;
					row.contentWidth = 0f;
					item = row;
				}
				item.children.Add(base.rectChildren[i]);
				item.contentWidth += num2;
				item.height = Mathf.Max(item.height, heights[i]);
			}
		}
		if (item.children.Count > 0)
		{
			list.Add(item);
		}
		return list;
	}

	private float CalculateRowStartX(float parentWidth, float contentWidth, float spacing, int childCount)
	{
		if (childCount <= 0)
		{
			return base.padding.left;
		}
		float num = contentWidth;
		if (childCount > 1)
		{
			num += spacing * (float)(childCount - 1);
		}
		return justifyContent switch
		{
			JustifyOptions.Right => parentWidth - (float)base.padding.right - num, 
			JustifyOptions.Center => (float)base.padding.left + (parentWidth - (float)base.padding.horizontal - num) / 2f, 
			_ => base.padding.left, 
		};
	}

	private float GetVerticalOffset(float yPos, float rowHeight, float childHeight)
	{
		switch (verticalChildAlignment)
		{
		case TextAnchor.MiddleLeft:
		case TextAnchor.MiddleCenter:
		case TextAnchor.MiddleRight:
			return yPos + (rowHeight - childHeight) / 2f;
		case TextAnchor.LowerLeft:
		case TextAnchor.LowerCenter:
		case TextAnchor.LowerRight:
			return yPos + rowHeight - childHeight;
		default:
			return yPos;
		}
	}
}

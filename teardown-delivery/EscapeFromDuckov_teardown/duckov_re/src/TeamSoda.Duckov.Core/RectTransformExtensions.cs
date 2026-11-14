using UnityEngine;

public static class RectTransformExtensions
{
	public static Camera GetUICamera()
	{
		return null;
	}

	public static bool MatchWorldPosition(this RectTransform rectTransform, Vector3 worldPosition, Vector3 worldSpaceOffset = default(Vector3))
	{
		RectTransform rectTransform2 = rectTransform.parent as RectTransform;
		if (rectTransform2 == null)
		{
			return false;
		}
		worldPosition += worldSpaceOffset;
		Camera main = Camera.main;
		if (main == null)
		{
			return false;
		}
		Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(main, worldPosition);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform2, screenPoint, GetUICamera(), out var localPoint);
		Vector3 rhs = worldPosition - main.transform.position;
		bool result = Vector3.Dot(main.transform.forward, rhs) > 0f;
		rectTransform.localPosition = localPoint;
		return result;
	}
}

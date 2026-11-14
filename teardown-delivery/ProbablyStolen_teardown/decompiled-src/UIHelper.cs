using UnityEngine;

public class UIHelper
{
	private const float xOffset = 0f;

	private const float yOffset = 0f;

	public static Vector3 GetTooltipPosition(float tooltipWidth, float tooltipHeight)
	{
		float num = LetterboxManager.GameAreaX;
		float num2 = LetterboxManager.GameAreaY;
		float num3 = num + (float)LetterboxManager.GameAreaWidth;
		float max = num2 + (float)LetterboxManager.GameAreaHeight;
		float num4 = Input.mousePosition.x + 0f + (float)LetterboxManager.GameAreaWidth * 0.007f;
		float num5 = Input.mousePosition.y + 0f;
		num4 += 1f;
		num5 += 2f;
		float x = Mathf.Clamp(num4, num, num3 - tooltipWidth);
		float y = Mathf.Clamp(num5, num2 + tooltipHeight, max);
		return new Vector3(x, y, 0f);
	}

	public static Vector3 GetTooltipPositionScreenBounds(float tooltipWidth, float tooltipHeight)
	{
		float value = Input.mousePosition.x + 0f + (float)LetterboxManager.GameAreaWidth * 0.007f + 1f;
		float value2 = Input.mousePosition.y + 0f + 2f;
		float x = Mathf.Clamp(value, 0f, (float)Screen.width - tooltipWidth);
		float y = Mathf.Clamp(value2, tooltipHeight, Screen.height);
		return new Vector3(x, y, 0f);
	}

	public static Vector3 GetTooltipPosition(float tooltipWidth, float tooltipHeight, float xCustomOffset, float yCustomOffset)
	{
		float x = Input.mousePosition.x + xCustomOffset + 1f;
		float num = Input.mousePosition.y + yCustomOffset + 2f;
		if (num < (float)LetterboxManager.GameAreaY + tooltipHeight)
		{
			num = (float)LetterboxManager.GameAreaY + tooltipHeight;
		}
		return new Vector3(x, num, 0f);
	}

	public static Vector3 GetTooltipPositionSimple()
	{
		float x = Input.mousePosition.x + 25f;
		float y = Input.mousePosition.y + 25f;
		return new Vector3(x, y, 0f);
	}
}

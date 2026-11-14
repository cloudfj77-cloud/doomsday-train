using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class UISettings : MonoBehaviour
{
	public static UISettings current;

	public float snapAnimationSpeed = 20f;

	public int gridSize = 16;

	public float dragOpacity = 0.7f;

	public int fontWidth = 8;

	public int fontHeight = 16;

	public int floorLootGridHeight = 16;

	private float charWidthScaleCache = 1f;

	private string charWidthScaleLocale;

	public int effectiveFontWidth => Mathf.Max(1, Mathf.RoundToInt((float)fontWidth * GetCharWidthScale()));

	public float GetCharWidthScale()
	{
		string text = null;
		try
		{
			Locale selectedLocale = LocalizationSettings.SelectedLocale;
			if (selectedLocale != null)
			{
				text = selectedLocale.Identifier.Code;
			}
		}
		catch
		{
			text = null;
		}
		if (text != charWidthScaleLocale)
		{
			charWidthScaleLocale = text;
			charWidthScaleCache = ResolveCharWidthScale(text);
		}
		return charWidthScaleCache;
	}

	private static float ResolveCharWidthScale(string code)
	{
		if (string.IsNullOrEmpty(code))
		{
			return 1f;
		}
		if (code.StartsWith("ja", StringComparison.OrdinalIgnoreCase))
		{
			return 2f;
		}
		if (code.StartsWith("ko", StringComparison.OrdinalIgnoreCase))
		{
			return 2f;
		}
		if (code.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
		{
			return 2f;
		}
		return 1f;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}

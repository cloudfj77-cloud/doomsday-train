using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalObjectEnabler : MonoBehaviour
{
	[Tooltip("The object to enable/disable. Must NOT be the GameObject this component is on.")]
	public GameObject target;

	[Tooltip("Locale code the target is shown for, matching the Locale asset's code (e.g. en, fr, zh, ptBR).")]
	public string localeCode;

	private void OnEnable()
	{
		LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
		if (LocalizationSettings.InitializationOperation.IsDone)
		{
			Apply(LocalizationSettings.SelectedLocale);
			return;
		}
		Apply(null);
		StartCoroutine(ApplyWhenReady());
	}

	private void OnDisable()
	{
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
	}

	private IEnumerator ApplyWhenReady()
	{
		yield return LocalizationSettings.InitializationOperation;
		Apply(LocalizationSettings.SelectedLocale);
	}

	private void OnLocaleChanged(Locale locale)
	{
		Apply(locale);
	}

	private void Apply(Locale locale)
	{
		if (!(target == null))
		{
			if (target == base.gameObject)
			{
				Debug.LogError("LocalObjectEnabler on '" + base.name + "' targets its own GameObject; it would never re-enable after a locale switch. Point it at a child object instead.");
				return;
			}
			bool active = locale != null && locale.Identifier.Code.Equals(localeCode, StringComparison.OrdinalIgnoreCase);
			target.SetActive(active);
		}
	}
}

using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(LocalizeStringEvent))]
public class TMPLocalizer : MonoBehaviour
{
	private TextMeshProUGUI textMeshPro;

	private LocalizeStringEvent localizeStringEvent;

	private TMP_FontAsset originalFont;

	private TooltipContainer tooltipContainer;

	public bool ChangeFontOnly;

	public bool IsTooltipContainer;

	[TextArea]
	[Tooltip("Appended verbatim after the localized string. Use for non-translatable suffixes like <link>/<sprite> tags so translators don't need to author them.")]
	public string postfix;

	public FontLoader.FontRole fontRole;

	private void Awake()
	{
		if (IsTooltipContainer)
		{
			localizeStringEvent = GetComponent<LocalizeStringEvent>();
			tooltipContainer = GetComponent<TooltipContainer>();
			localizeStringEvent.OnUpdateString.AddListener(delegate(string localizedText)
			{
				tooltipContainer.content = localizedText + postfix;
			});
			return;
		}
		textMeshPro = GetComponent<TextMeshProUGUI>();
		if (textMeshPro == null)
		{
			Debug.LogError("TMPLocalizer requires a TextMeshProUGUI component on the same GameObject.");
			return;
		}
		originalFont = textMeshPro.font;
		if (!ChangeFontOnly)
		{
			localizeStringEvent = GetComponent<LocalizeStringEvent>();
			localizeStringEvent.OnUpdateString.AddListener(delegate(string localizedText)
			{
				textMeshPro.text = localizedText + postfix;
			});
		}
	}

	private void Start()
	{
		if (localizeStringEvent != null)
		{
			localizeStringEvent.RefreshString();
		}
	}

	private void Update()
	{
	}

	protected virtual void OnLocaleChanged(Locale newLocale)
	{
		if (FontLoader.Instance == null)
		{
			Debug.LogError("TMPLocalizer requires a TextMeshProUGUI component on the same GameObject.");
		}
		textMeshPro.font = FontLoader.Instance.GetFontByRole(fontRole, newLocale);
	}

	protected virtual void OnEnable()
	{
		if (!IsTooltipContainer)
		{
			LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
			OnLocaleChanged(LocalizationSettings.SelectedLocale);
		}
	}

	protected virtual void OnDisable()
	{
		if (!IsTooltipContainer)
		{
			LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
		}
	}
}

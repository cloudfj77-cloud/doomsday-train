using SodaCraft.Localizations;
using UnityEngine;

public class CustomerServiceInfo : MonoBehaviour
{
	[SerializeField]
	private GameObject chineseInfo;

	[SerializeField]
	private GameObject nonChineseInfo;

	private void Awake()
	{
		LocalizationManager.OnSetLanguage += Refresh;
	}

	private void OnDestroy()
	{
		LocalizationManager.OnSetLanguage -= Refresh;
	}

	private void OnEnable()
	{
		Refresh(LocalizationManager.CurrentLanguage);
	}

	private void Refresh(SystemLanguage language)
	{
		bool flag = language == SystemLanguage.ChineseSimplified;
		chineseInfo.SetActive(flag);
		nonChineseInfo.SetActive(!flag);
	}
}

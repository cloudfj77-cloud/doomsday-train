using UnityEngine;
using UnityEngine.UI;

public class DialogSettingUIManager : MonoBehaviour
{
	public Slider dialogueSpeedSlider;

	public Slider dialogueVolumeSlider;

	public Toggle typewriterEffectToggle;

	public Toggle autoNextDialogToggle;

	public GameObject optionPanel;

	public static DialogSettingUIManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public void CloseUI()
	{
		optionPanel.SetActive(value: false);
		if (StoreUIManager.Instance != null)
		{
			StoreUIManager.Instance.SetTypewriterSpeed();
		}
	}

	public void OpenUI()
	{
		optionPanel.SetActive(value: true);
		dialogueSpeedSlider.value = EmporiumOption.Instance.dialogSpeed;
		dialogueVolumeSlider.value = EmporiumOption.Instance.dialogVolume;
		typewriterEffectToggle.isOn = EmporiumOption.Instance.enableTypewriterEffect;
		autoNextDialogToggle.isOn = EmporiumOption.Instance.autoNextDialog;
	}

	public void OnApplySetting()
	{
		bool autoNextDialog = EmporiumOption.Instance.autoNextDialog;
		EmporiumOption.Instance.dialogSpeed = dialogueSpeedSlider.value;
		EmporiumOption.Instance.dialogVolume = dialogueVolumeSlider.value;
		EmporiumOption.Instance.enableTypewriterEffect = typewriterEffectToggle.isOn;
		EmporiumOption.Instance.autoNextDialog = autoNextDialogToggle.isOn;
		EmporiumOption.Instance.ApplySetting(applyResolution: false);
		EmporiumOption.Instance.OnAutoAdvanceSettingApplied(autoNextDialog);
	}
}

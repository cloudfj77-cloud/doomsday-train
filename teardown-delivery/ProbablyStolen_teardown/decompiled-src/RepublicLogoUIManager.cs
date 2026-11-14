using UnityEngine;
using UnityEngine.UI;

public class RepublicLogoUIManager : MonoBehaviour
{
	public GameObject panel;

	public Slider slider;

	public Image image;

	public static RepublicLogoUIManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		slider.onValueChanged.AddListener(HandleSliderValueChanged);
	}

	private void HandleSliderValueChanged(float value)
	{
		Color color = image.color;
		color.a = Mathf.Clamp01(value);
		image.color = color;
	}

	public void OpenPanel()
	{
		PaperUIManager.Instance.audioSourcePickup.Play();
		panel.SetActive(value: true);
	}

	public void ClosePanel()
	{
		PaperUIManager.Instance.audioSourceDrop.Play();
		panel.SetActive(value: false);
	}
}

using System;
using TMPro;
using UnityEngine;

public class GenericConfirmationUIManager : MonoBehaviour
{
	public TextMeshProUGUI bodyTMP;

	public TextMeshProUGUI confirmButtonTMP;

	public TextMeshProUGUI cancelButtonTMP;

	public GameObject panel;

	public GameObject confirmationDarkness;

	public GameObject confirmButton;

	public GameObject cancelButton;

	private Action onConfirm;

	private Action onCancel;

	public static GenericConfirmationUIManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OpenPanel(string bodyText, string confirmText, string cancelText, Action onConfirm = null, Action onCancel = null)
	{
		bodyTMP.text = bodyText;
		confirmButtonTMP.text = confirmText;
		cancelButtonTMP.text = cancelText;
		cancelButton.SetActive(value: true);
		this.onConfirm = onConfirm;
		this.onCancel = onCancel;
		panel.SetActive(value: true);
		confirmationDarkness.SetActive(value: true);
	}

	public void OpenPanelSingleButton(string bodyText, string confirmText, Action onConfirm = null)
	{
		bodyTMP.text = bodyText;
		confirmButtonTMP.text = confirmText;
		cancelButton.SetActive(value: false);
		this.onConfirm = onConfirm;
		onCancel = null;
		panel.SetActive(value: true);
		confirmationDarkness.SetActive(value: true);
	}

	public void ClosePanel()
	{
		panel.SetActive(value: false);
		confirmationDarkness.SetActive(value: false);
	}

	public void ConfirmButtonPressed()
	{
		onConfirm?.Invoke();
		ClosePanel();
	}

	public void CancelButtonPressed()
	{
		onCancel?.Invoke();
		ClosePanel();
	}
}

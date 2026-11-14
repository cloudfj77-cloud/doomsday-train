using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanelUIManager : MonoBehaviour
{
	public GameObject panel;

	public TextMeshProUGUI bodyTMP;

	public TextMeshProUGUI yesTMP;

	public TextMeshProUGUI noTMP;

	public Action yesAction;

	public Action noAction;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OpenPanel(string body, Action yesAction = null, Action noAction = null, bool hideNoButton = false)
	{
		panel.SetActive(value: true);
		bodyTMP.text = body;
		this.yesAction = yesAction;
		this.noAction = noAction;
		Button componentInParent = noTMP.GetComponentInParent<Button>(includeInactive: true);
		((componentInParent != null) ? componentInParent.gameObject : noTMP.gameObject).SetActive(!hideNoButton);
	}

	public void OnYesClick()
	{
		panel.SetActive(value: false);
		if (yesAction != null)
		{
			yesAction();
		}
	}

	public void OnNoClick()
	{
		panel.SetActive(value: false);
		if (noAction != null)
		{
			noAction();
		}
	}
}

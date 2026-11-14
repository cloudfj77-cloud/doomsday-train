using TMPro;
using UnityEngine;

public class ChemFinishElement : MonoBehaviour
{
	public TextMeshProUGUI displayTMP;

	public TextMeshProUGUI volumeTMP;

	public TextMeshProUGUI dryTMP;

	public string identifier;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnButtonClick()
	{
		EmporiumAudioPlayer.Instance.audioButton2.Play();
		ChemFinisherUIManager.Instance.OnChemRecipeSelected(identifier);
	}
}

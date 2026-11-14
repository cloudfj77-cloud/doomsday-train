using UnityEngine;

public class ContextualMoneyAndCash : MonoBehaviour
{
	[SerializeField]
	private GameObject moneyDisplay;

	[SerializeField]
	private GameObject cashDisplay;

	private void OnEnable()
	{
		moneyDisplay.SetActive(LevelConfig.Instance == null || LevelConfig.Instance.AccountAvailable);
		cashDisplay.SetActive(value: true);
	}
}

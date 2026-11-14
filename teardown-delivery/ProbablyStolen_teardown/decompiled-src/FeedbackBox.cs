using UnityEngine;

public class FeedbackBox : MonoBehaviour, IClickable
{
	public void OnClick()
	{
		ReputationUIManager.Instance.ToggleUI();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}

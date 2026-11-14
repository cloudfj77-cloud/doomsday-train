using UnityEngine;

public class StoreRecorder : MonoBehaviour, IClickable
{
	public void OnClick()
	{
		RecorderUIManager.Instance.Toggle();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}

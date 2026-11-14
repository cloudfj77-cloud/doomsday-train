using UnityEngine;

public class StoreClienteleDisplay : MonoBehaviour, IClickable
{
	public void OnClick()
	{
		ClienteleUIManager.Instance.ToggleUI();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}

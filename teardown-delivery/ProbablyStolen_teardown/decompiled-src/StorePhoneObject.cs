using UnityEngine;

public class StorePhoneObject : MonoBehaviour, IClickable
{
	public void OnClick()
	{
		PhoneUIManager.Instance.OpenUI();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}

using UnityEngine;

public class NegociationTarget : MonoBehaviour, IClickable
{
	public void OnClick()
	{
		Debug.Log("NegociationTarget Clicked");
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}

using TMPro;
using UnityEngine;

public class NotebookElement : MonoBehaviour
{
	public TextMeshProUGUI tmp;

	public ItemFeature itemFeature;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void ElementClicked()
	{
		Debug.Log("Element Ciclked");
		EmporiumAudioPlayer.Instance.PlayMatched();
		if (itemFeature.GetPublicHidden() && itemFeature.CanExposeFeature() && !itemFeature.IsExposed())
		{
			Debug.Log("Exposing Feature without public display");
			itemFeature.ExposeFeature();
		}
	}
}

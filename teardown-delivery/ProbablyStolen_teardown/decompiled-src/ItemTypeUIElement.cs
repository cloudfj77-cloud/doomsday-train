using UnityEngine;

public class ItemTypeUIElement : MonoBehaviour
{
	public string type;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnClick()
	{
		if (!EmporiumRTSController.IsNegociationToolPreviouslySelected())
		{
			return;
		}
		NegociationElement negociationElementPrevious = EmporiumRTSController.GetNegociationElementPrevious();
		if (negociationElementPrevious.GetNegociationData() == null)
		{
			return;
		}
		foreach (NegociationData negociationDatum in negociationElementPrevious.GetNegociationData())
		{
			Debug.Log(negociationDatum.itemType + " " + type);
			if (negociationDatum.itemType.Equals(type))
			{
				if (PlayerStore.Instance.GetCurrentlyNegociatedItem().IsAlreadyContainFeatureWithId(negociationDatum.identifier))
				{
				}
			}
			else
			{
				Debug.Log("Mismatch");
			}
		}
	}
}

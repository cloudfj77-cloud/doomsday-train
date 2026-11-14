using System;

public class NegociationData
{
	[ES3Serializable]
	public string itemType;

	[ES3Serializable]
	public int value;

	[ES3Serializable]
	public string publicDisplay;

	[ES3Serializable]
	public string identifier;

	[ES3Serializable]
	public string reason;

	public NegociationData(string itemType, int value, string publicDisplay, string displayName)
	{
		this.itemType = itemType;
		this.value = value;
		this.publicDisplay = publicDisplay;
		reason = displayName;
		identifier = Guid.NewGuid().ToString();
	}

	public bool IsNegociationDataAffectItem(GameItem gameItem)
	{
		foreach (string item in gameItem.GetGameItemType())
		{
			if (itemType.Equals(item))
			{
				return true;
			}
		}
		return false;
	}
}

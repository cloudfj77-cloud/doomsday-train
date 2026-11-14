using System;

public class SavePreviewData
{
	[ES3Serializable]
	public int slotId;

	[ES3Serializable]
	public int credit;

	[ES3Serializable]
	public int dayCount;

	[ES3Serializable]
	public DateTime lastSaveDateTime;

	[ES3Serializable]
	public string startName;

	[ES3Serializable]
	public string version;

	[ES3Serializable]
	public NewGameData.StartType startType;

	[ES3Serializable]
	public string startScenarioId;

	[ES3Serializable]
	public string startLocale;

	[ES3Serializable]
	public string customName;

	public string GetDisplayName()
	{
		if (!string.IsNullOrEmpty(customName))
		{
			return customName;
		}
		return NewGameData.GetStartDisplayName(startType, startScenarioId);
	}

	public SavePreviewData CloneForSlot(int newSlotId)
	{
		return new SavePreviewData
		{
			slotId = newSlotId,
			credit = credit,
			dayCount = dayCount,
			lastSaveDateTime = lastSaveDateTime,
			startName = startName,
			version = version,
			startType = startType,
			startScenarioId = startScenarioId,
			startLocale = startLocale,
			customName = customName
		};
	}

	public static SavePreviewData GenerateData()
	{
		return new SavePreviewData
		{
			slotId = PlayerStore.Instance.saveSlotId,
			credit = PlayerStore.Instance.playerCash,
			dayCount = StoreStation.Instance.dayCounter,
			lastSaveDateTime = DateTime.Now,
			startName = NewGameData.GetStartDisplayName(PlayerStore.Instance.startType),
			version = PlayerStore.VERSION,
			startType = PlayerStore.Instance.startType,
			startScenarioId = PlayerStore.Instance.startScenarioId,
			startLocale = PlayerStore.Instance.startLocale
		};
	}
}

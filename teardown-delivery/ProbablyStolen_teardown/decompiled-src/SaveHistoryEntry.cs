using System;

public class SaveHistoryEntry
{
	[ES3Serializable]
	public string file;

	[ES3Serializable]
	public int dayCount;

	[ES3Serializable]
	public int credit;

	[ES3Serializable]
	public DateTime saveDateTime;

	[ES3Serializable]
	public string version;
}

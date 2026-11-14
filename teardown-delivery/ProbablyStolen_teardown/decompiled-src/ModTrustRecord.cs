using System;
using System.Collections.Generic;
using System.Globalization;

[Serializable]
public class ModTrustRecord
{
	public string id;

	public string source;

	public string workshopId;

	public string manifestHash;

	public List<string> files = new List<string>();

	public string combinedHash;

	public List<string> declared = new List<string>();

	public List<string> detected = new List<string>();

	public string gameVersion;

	public string approvedAt;

	public string updatedAt;

	public bool developerMode;

	public int scannerVersion;

	public DateTime? ApprovedAtUtc => ParseTime(approvedAt);

	public DateTime? UpdatedAtUtc => ParseTime(updatedAt);

	private static DateTime? ParseTime(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (!DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var result))
		{
			return null;
		}
		return result;
	}
}

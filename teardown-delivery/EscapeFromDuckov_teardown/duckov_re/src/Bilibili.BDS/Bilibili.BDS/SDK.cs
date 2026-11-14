using UnityEngine;

namespace Bilibili.BDS;

public static class SDK
{
	public enum ENV
	{
		cb,
		beta,
		preview,
		ob
	}

	public const string GameID = "14457";

	public static int SDKInitBDS(ENV env = ENV.cb, string channelID = "None")
	{
		string strENV = env.ToString();
		int result = InitBDS("14457", channelID, strENV);
		Debug.Log("Initialize Bilibili BDS: " + result);
		return result;
	}

	public static int SDKInitBDS(string channelID, ENV env)
	{
		string strENV = env.ToString();
		return InitBDS("14457", channelID, strENV);
	}

	private static int InitBDS(string strGameId, string strChannelId, string strENV)
	{
		return 0;
	}

	public static int ReportCustomEvent(string strEventName, string strPlayerInfo, string strExtension, string strCpParam)
	{
		return 0;
	}

	public static int ReportCustomEventSync(string strEventName, string strPlayerInfo, string strExtension, string strCpParam)
	{
		return 0;
	}
}

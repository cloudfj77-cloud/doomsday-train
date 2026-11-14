using System;
using System.Collections.Generic;
using System.Linq;

public static class ModSteamRules
{
	private const string NS = "Steamworks.";

	private static readonly HashSet<string> UgcWriteMethods = new HashSet<string>(StringComparer.Ordinal)
	{
		"CreateItem", "StartItemUpdate", "SubmitItemUpdate", "DeleteItem", "SubscribeItem", "AddItemKeyValueTag", "RemoveItemKeyValueTags", "RemoveAllItemKeyValueTags", "AddItemPreviewFile", "AddItemPreviewVideo",
		"UpdateItemPreviewFile", "UpdateItemPreviewVideo", "RemoveItemPreview", "AddContentDescriptor", "RemoveContentDescriptor", "SetAllowLegacyUpload", "SetUserItemVote", "AddItemToFavorites", "RemoveItemFromFavorites", "AddDependency",
		"RemoveDependency", "AddAppDependency", "RemoveAppDependency", "StartPlaytimeTracking", "StopPlaytimeTracking", "StopPlaytimeTrackingForAllItems", "BInitWorkshopForGameServer", "SuspendDownloads", "SetSubscriptionsLoadOrder"
	};

	private static readonly HashSet<string> UgcGameMethods = new HashSet<string>(StringComparer.Ordinal) { "DownloadItem", "UnsubscribeItem" };

	private static readonly HashSet<string> RemoteStorageWriteMethods = new HashSet<string>(StringComparer.Ordinal)
	{
		"FileShare", "FileDelete", "FileForget", "SetSyncPlatforms", "BeginFileWriteBatch", "EndFileWriteBatch", "PublishWorkshopFile", "PublishVideo", "CreatePublishedFileUpdateRequest", "CommitPublishedFileUpdate",
		"SubscribePublishedFile", "UnsubscribePublishedFile", "DeletePublishedFile", "SetUserPublishedFileAction", "UpdateUserPublishedItemVote", "UGCDownload", "UGCDownloadToLocation"
	};

	private static readonly HashSet<string> UserAccountMethods = new HashSet<string>(StringComparer.Ordinal)
	{
		"GetAuthSessionTicket", "GetAuthTicketForWebApi", "RequestEncryptedAppTicket", "GetEncryptedAppTicket", "BeginAuthSession", "InitiateGameConnection", "InitiateGameConnection_DEPRECATED", "RequestStoreAuthURL", "StartVoiceRecording", "GetVoice",
		"GetAvailableVoice"
	};

	private static readonly HashSet<string> FriendsMessagingMethods = new HashSet<string>(StringComparer.Ordinal) { "ReplyToFriendMessage", "SendClanChatMessage", "InviteUserToGame", "ActivateGameOverlayInviteDialog", "ActivateGameOverlayInviteDialogConnectString" };

	private static readonly HashSet<string> MatchmakingMessagingMethods = new HashSet<string>(StringComparer.Ordinal) { "SendLobbyChatMsg", "InviteUserToLobby" };

	private static readonly HashSet<string> InternalTypes = new HashSet<string>(StringComparer.Ordinal) { "SteamAPI", "SteamClient", "NativeMethods", "CSteamAPIContext", "CSteamGameServerAPIContext", "SteamEncryptedAppTicket", "GameServer" };

	private static readonly HashSet<string> GuardTypes = new HashSet<string>(StringComparer.Ordinal)
	{
		"Steamworks.SteamUGC", "Steamworks.SteamGameServerUGC", "Steamworks.SteamRemoteStorage", "Steamworks.SteamUser", "Steamworks.SteamFriends", "Steamworks.SteamMatchmaking", "Steamworks.SteamAPI", "Steamworks.SteamClient", "Steamworks.NativeMethods", "Steamworks.CSteamAPIContext",
		"Steamworks.CSteamGameServerAPIContext", "Steamworks.InteropHelp", "Steamworks.SteamEncryptedAppTicket"
	};

	public static readonly string[] RuntimeGuardedTypes = new string[5] { "SteamUGC", "SteamRemoteStorage", "SteamUser", "SteamFriends", "SteamMatchmaking" };

	public static readonly string[] InternalStrings = new string[11]
	{
		"steam_api.dll", "steam_api64.dll", "steamclient.dll", "steamclient64.dll", "sdkencryptedappticket", "steamapi_isteam", "steamapi_init", "isteamugc_", "isteamremotestorage_", "isteamuser_",
		"isteamclient_"
	};

	public static ModSteamUse Classify(string declaringTypeFullName, string methodName)
	{
		if (string.IsNullOrEmpty(declaringTypeFullName) || !declaringTypeFullName.StartsWith("Steamworks.", StringComparison.Ordinal))
		{
			return ModSteamUse.None;
		}
		string text = declaringTypeFullName.Substring("Steamworks.".Length).Split('/', '+')[0];
		switch (text)
		{
		case "SteamUGC":
		case "SteamGameServerUGC":
			if (!IsUgcWrite(methodName))
			{
				return ModSteamUse.Capability;
			}
			return ModSteamUse.WorkshopPublish;
		case "SteamRemoteStorage":
			if (!IsRemoteStorageWrite(methodName))
			{
				return ModSteamUse.Capability;
			}
			return ModSteamUse.WorkshopPublish;
		case "SteamUser":
			if (UserAccountMethods.Contains(methodName))
			{
				return ModSteamUse.Account;
			}
			break;
		}
		if (text == "SteamFriends" && FriendsMessagingMethods.Contains(methodName))
		{
			return ModSteamUse.Account;
		}
		if (text == "SteamMatchmaking" && MatchmakingMessagingMethods.Contains(methodName))
		{
			return ModSteamUse.Account;
		}
		if (text == "SteamAPI" && methodName == "IsSteamRunning")
		{
			return ModSteamUse.Capability;
		}
		if (InternalTypes.Contains(text) || text.StartsWith("SteamGameServer", StringComparison.Ordinal))
		{
			return ModSteamUse.Account;
		}
		if (text.StartsWith("Steam", StringComparison.Ordinal) || text == "Callback`1" || text == "CallResult`1")
		{
			return ModSteamUse.Capability;
		}
		return ModSteamUse.None;
	}

	private static bool IsUgcWrite(string method)
	{
		if (!method.StartsWith("SetItem", StringComparison.Ordinal) && !UgcWriteMethods.Contains(method))
		{
			return UgcGameMethods.Contains(method);
		}
		return true;
	}

	private static bool IsRemoteStorageWrite(string method)
	{
		if (!method.StartsWith("FileWrite", StringComparison.Ordinal) && !method.StartsWith("UpdatePublishedFile", StringComparison.Ordinal))
		{
			return RemoteStorageWriteMethods.Contains(method);
		}
		return true;
	}

	public static bool IsGuardedAtRuntime(string typeName, string methodName)
	{
		if (!RuntimeGuardedTypes.Contains(typeName) || UgcGameMethods.Contains(methodName))
		{
			return false;
		}
		ModSteamUse modSteamUse = Classify("Steamworks." + typeName, methodName);
		if (modSteamUse != ModSteamUse.WorkshopPublish)
		{
			return modSteamUse == ModSteamUse.Account;
		}
		return true;
	}

	public static bool IsGuardType(string fullName)
	{
		if (string.IsNullOrEmpty(fullName))
		{
			return false;
		}
		string text = fullName.Split('/', '+')[0];
		if (!text.Contains('.'))
		{
			text = "Steamworks." + text;
		}
		if (!GuardTypes.Contains(text))
		{
			return text.StartsWith("Steamworks.SteamGameServer", StringComparison.Ordinal);
		}
		return true;
	}

	public static bool IsSteamNativeLibrary(string module)
	{
		if (string.IsNullOrEmpty(module))
		{
			return false;
		}
		string text = module.ToLowerInvariant();
		if (!text.Contains("steam"))
		{
			return text.Contains("sdkencryptedappticket");
		}
		return true;
	}
}

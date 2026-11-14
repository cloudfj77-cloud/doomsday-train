using System;
using System.Collections.Generic;

public class StartingPerk
{
	public enum StartingPerkType
	{
		POSITIVE,
		NEGATIVE,
		NEUTRAL
	}

	[ES3Serializable]
	public string id;

	[ES3Serializable]
	public int cost;

	[ES3Serializable]
	public int maxSlot;

	[ES3Serializable]
	public List<string> incompatiblePerks;

	[ES3Serializable]
	public StartingPerkType type;

	[ES3Serializable]
	public bool mindEditable;

	[ES3Serializable]
	public string rawName;

	[ES3Serializable]
	public string rawDescription;

	[ES3NonSerializable]
	public string source;

	[ES3NonSerializable]
	public string iconKey;

	[ES3NonSerializable]
	public Func<string> nameProvider;

	[ES3NonSerializable]
	public Func<string> descriptionProvider;

	private static readonly Dictionary<string, string> PendingEnglish = new Dictionary<string, string>();

	private static readonly object[] NoArgs = new object[0];

	private static readonly object[] ArgsSaving = new object[1] { 150 };

	private static readonly object[] ArgsBadLease = new object[1] { 40 };

	private static readonly object[] ArgsConvict = new object[2] { -30, 25 };

	private static readonly object[] ArgsModelCitizen = new object[2] { 30, 50 };

	private static readonly object[] ArgsWellConnected = new object[1] { 15 };

	private static readonly object[] ArgsPublicEnemy = new object[1] { -15 };

	private static readonly object[] ArgsAscetic = new object[1] { 16 };

	private static readonly object[] ArgsWildFavorite = new object[2] { 10, 50 };

	private static readonly object[] ArgsWildBadSide = new object[2] { 10, 50 };

	private static readonly object[] ArgsAugMenace = new object[1] { 25 };

	private static readonly object[] ArgsDiplomatic = new object[1] { 33 };

	private static readonly object[] ArgsIntimidating = new object[1] { 33 };

	private static readonly object[] ArgsAddiction = new object[1] { 8 };

	private static readonly object[] ArgsColorfulCharacter = new object[1] { 3 };

	private static readonly object[] ArgsRevolutionSympathizer = new object[1] { 25 };

	private static readonly object[] ArgsPrimeRealEstate = new object[1] { 75 };

	private static readonly object[] ArgsOsteoarthritis = new object[3] { 3, 5, 2 };

	private static readonly object[] ArgsKnownSnitch = new object[1] { -10 };

	private static readonly object[] ArgsTinkerer = new object[1] { 20 };

	private static readonly object[] ArgsCarefulScavenger = new object[1] { 2 };

	private static readonly object[] ArgsSycophant = new object[3] { 15, 25, 10 };

	private static readonly object[] ArgsAgoraphobia = new object[1] { 2 };

	private static readonly object[] ArgsAwfulNegotiator = new object[1] { 5 };

	public string GetLocalizedDisplayName()
	{
		if (!string.IsNullOrEmpty(rawName))
		{
			return rawName;
		}
		Func<string> func = nameProvider ?? StartingPerkList.GetModPerk(id)?.nameProvider;
		if (func != null)
		{
			return ResolveProvider(func);
		}
		return GetPerkTableText("perk_" + id + "_name", NoArgs);
	}

	public string GetLocalizedDescription()
	{
		if (!string.IsNullOrEmpty(rawDescription))
		{
			return rawDescription;
		}
		Func<string> func = descriptionProvider ?? StartingPerkList.GetModPerk(id)?.descriptionProvider;
		if (func != null)
		{
			return ResolveProvider(func);
		}
		return GetPerkTableText(GetDescriptionKey(), GetDescriptionArguments());
	}

	private string GetDescriptionKey()
	{
		if (id == "agoraphobia")
		{
			return "perk_agoraphobia_rest_desc";
		}
		return "perk_" + id + "_desc";
	}

	private static string GetPerkTableText(string key, object[] arguments)
	{
		if (PendingEnglish.TryGetValue(key, out var value))
		{
			return LocHelper.GetLocalizedOrDefault("PerkTable", key, value, arguments);
		}
		return LocHelper.GetLocalizedPerkTable(key, arguments);
	}

	private string ResolveProvider(Func<string> provider)
	{
		try
		{
			return provider() ?? "";
		}
		catch (Exception ex)
		{
			Debug.LogError("[StartingPerk] Text provider of '" + id + "' threw: " + ex);
			return id;
		}
	}

	private object[] GetDescriptionArguments()
	{
		switch (id)
		{
		case "saving":
			return ArgsSaving;
		case "bad_lease":
			return ArgsBadLease;
		case "convict":
			return ArgsConvict;
		case "model_citizen":
			return ArgsModelCitizen;
		case "well_connected":
			return ArgsWellConnected;
		case "public_enemy":
			return ArgsPublicEnemy;
		case "ascetic":
			return ArgsAscetic;
		case "wild_favorite":
			return ArgsWildFavorite;
		case "wild_bad_side":
			return ArgsWildBadSide;
		case "aug_menace":
			return ArgsAugMenace;
		case "diplomatic":
			return ArgsDiplomatic;
		case "intimidating":
			return ArgsIntimidating;
		case "nicotine_addiction":
		case "narcotic_addiction":
		case "alcohol_addiction":
			return ArgsAddiction;
		case "colorful_character":
			return ArgsColorfulCharacter;
		case "blabbermouth":
			return ArgsIntimidating;
		case "revolution_sympathizer":
			return ArgsRevolutionSympathizer;
		case "prime_real_estate":
			return ArgsPrimeRealEstate;
		case "degenerative_osteoarthritis":
			return ArgsOsteoarthritis;
		case "known_snitch":
			return ArgsKnownSnitch;
		case "tinkerer":
			return ArgsTinkerer;
		case "careful_scavenger":
			return ArgsCarefulScavenger;
		case "sycophant":
			return ArgsSycophant;
		case "agoraphobia":
			return ArgsAgoraphobia;
		case "awful_negotiator":
			return ArgsAwfulNegotiator;
		default:
			return NoArgs;
		}
	}

	public StartingPerk()
	{
		incompatiblePerks = new List<string>();
	}

	private static bool IsFullOnlyPerkActive(string perkId)
	{
		return IsPerkActive(perkId);
	}

	public static bool IsPerkActive(string perkId)
	{
		for (int i = 0; i < PlayerStore.Instance.startingPerks.Count; i++)
		{
			if (PlayerStore.Instance.startingPerks[i].id.Equals(perkId))
			{
				return true;
			}
		}
		return false;
	}

	public static void RemovePerk(string perkId)
	{
		for (int i = 0; i < PlayerStore.Instance.startingPerks.Count; i++)
		{
			if (PlayerStore.Instance.startingPerks[i].id.Equals(perkId))
			{
				PlayerStore.Instance.startingPerks.RemoveAt(i);
				break;
			}
		}
	}

	public static StartingPerk GetPerkById(string perkId)
	{
		for (int i = 0; i < PlayerStore.Instance.startingPerks.Count; i++)
		{
			if (PlayerStore.Instance.startingPerks[i].id.Equals(perkId))
			{
				return PlayerStore.Instance.startingPerks[i];
			}
		}
		return null;
	}

	public static bool IsSaving()
	{
		return IsPerkActive("saving");
	}

	public static bool IsBadLease()
	{
		return IsPerkActive("bad_lease");
	}

	public static bool IsLoan()
	{
		return IsPerkActive("loan");
	}

	public static bool IsDebt()
	{
		return IsPerkActive("debt");
	}

	public static bool IsConvict()
	{
		return IsPerkActive("convict");
	}

	public static bool IsModelCitizen()
	{
		return IsPerkActive("model_citizen");
	}

	public static bool IsWellConnected()
	{
		return IsPerkActive("well_connected");
	}

	public static bool IsPublicEnemy()
	{
		return IsPerkActive("public_enemy");
	}

	public static bool IsSycophant()
	{
		return IsPerkActive("sycophant");
	}

	public static bool IsAwfulNegotiator()
	{
		return IsPerkActive("awful_negotiator");
	}

	public static bool IsBannedCommissary()
	{
		return IsPerkActive("banned_commissary");
	}

	public static bool IsHoarder()
	{
		return IsPerkActive("hoarder");
	}

	public static bool IsMakeshiftReactor()
	{
		return IsPerkActive("makeshift_reactor");
	}

	public static bool IsAlert()
	{
		return IsPerkActive("alert");
	}

	public static bool IsSawyerCrew()
	{
		return IsPerkActive("sawyer_crew");
	}

	public static bool IsDiplomatic()
	{
		return IsPerkActive("diplomatic");
	}

	public static bool IsIntimidating()
	{
		return IsPerkActive("intimidating");
	}

	public static bool IsSideStreet()
	{
		return IsPerkActive("side_street");
	}

	public static bool IsNicotineAddiction()
	{
		return IsPerkActive("nicotine_addiction");
	}

	public static bool IsNarcoticAddiction()
	{
		return IsPerkActive("narcotic_addiction");
	}

	public static bool IsAlcoholAddiction()
	{
		return IsPerkActive("alcohol_addiction");
	}

	public static bool IsSecretAdmirer()
	{
		return IsPerkActive("secret_admirer");
	}

	public static bool IsBlackmail()
	{
		return IsPerkActive("blackmail");
	}

	public static bool IsBlabbermouth()
	{
		return IsPerkActive("blabbermouth");
	}

	public static bool IsUnsubscribed()
	{
		return IsPerkActive("unsubscribed");
	}

	public static bool IsMinimalist()
	{
		return IsPerkActive("minimalist");
	}

	public static bool IsAscetic()
	{
		return IsPerkActive("ascetic");
	}

	public static bool IsSentinel()
	{
		return IsPerkActive("sentinel");
	}

	public static bool IsExperiencedChemist()
	{
		return true;
	}

	public static bool IsSpeedy()
	{
		return IsPerkActive("speedy");
	}

	public static bool IsAgoraphobia()
	{
		return IsFullOrDemoPerkActive("agoraphobia");
	}

	public static bool IsWelfareProgram()
	{
		return IsPerkActive("welfare_program");
	}

	public static bool IsProficientScavenger()
	{
		return IsPerkActive("proficient_scavenger");
	}

	public static bool IsMaimed()
	{
		return IsPerkActive("maimed");
	}

	public static bool IsCarefulScavenger()
	{
		return IsPerkActive("careful_scavenger");
	}

	public static bool IsRevolutionSympathizer()
	{
		return IsFullOnlyPerkActive("revolution_sympathizer");
	}

	public static bool IsGamblingAddiction()
	{
		return IsPerkActive("gambling_addiction");
	}

	public static bool IsWildFavorite()
	{
		return IsPerkActive("wild_favorite");
	}

	public static bool IsWildBadSide()
	{
		return IsPerkActive("wild_bad_side");
	}

	public static bool IsAugMenace()
	{
		return IsPerkActive("aug_menace");
	}

	public static bool IsLucky()
	{
		return IsPerkActive("lucky");
	}

	public static bool IsUnlucky()
	{
		return IsPerkActive("unlucky");
	}

	public static bool IsTapehead()
	{
		return IsPerkActive("tapehead");
	}

	public static bool IsHandy()
	{
		return IsPerkActive("handy");
	}

	public static bool IsTinkerer()
	{
		return IsPerkActive("tinkerer");
	}

	public static bool IsExpeditionOrganizer()
	{
		return IsPerkActive("expedition_organizer");
	}

	public static bool IsVending()
	{
		return IsFullOnlyPerkActive("vending");
	}

	public static bool IsShortSleeper()
	{
		return IsFullOnlyPerkActive("short_sleeper");
	}

	public static bool IsWorkingPipes()
	{
		return IsFullOnlyPerkActive("working_pipes");
	}

	public static bool IsIlliterate()
	{
		return IsFullOnlyPerkActive("illiterate");
	}

	public static bool IsPrimeRealEstate()
	{
		return IsFullOnlyPerkActive("prime_real_estate");
	}

	private static bool IsFullOrDemoPerkActive(string perkId)
	{
		return IsPerkActive(perkId);
	}

	public static bool IsDegenerativeOsteoarthritis()
	{
		return IsFullOrDemoPerkActive("degenerative_osteoarthritis");
	}

	public static bool IsKnownSnitch()
	{
		return IsFullOrDemoPerkActive("known_snitch");
	}
}

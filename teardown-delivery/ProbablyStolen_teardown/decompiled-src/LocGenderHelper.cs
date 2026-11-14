using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LocGenderHelper
{
	public enum LocGender
	{
		Neutral,
		Male,
		Female
	}

	public enum LocRegister
	{
		Formal,
		Informal
	}

	public const string SPEAKS_FORMAL_TAG = "SPEAKS_FORMAL";

	public const string SPEAKS_INFORMAL_TAG = "SPEAKS_INFORMAL";

	private const string TOKEN_PREFIX = "[g:";

	private const string REGISTER_TOKEN_PREFIX = "[r:";

	private static readonly Regex genderTokenRegex = new Regex("\\[g:([^\\[\\]|]*)\\|([^\\[\\]|]*)(?:\\|([^\\[\\]|]*))?\\]", RegexOptions.Compiled);

	private static readonly Regex registerTokenRegex = new Regex("\\[r:([^\\[\\]|]*)\\|([^\\[\\]|]*)\\]", RegexOptions.Compiled);

	private static readonly Dictionary<string, LocGender> uniqueSpriteGender = new Dictionary<string, LocGender>
	{
		{
			"eleanor1",
			LocGender.Female
		},
		{
			"eleanor2",
			LocGender.Female
		},
		{
			"eleanor3",
			LocGender.Female
		},
		{
			"eleanor_scav",
			LocGender.Female
		},
		{
			"eleanor_rev",
			LocGender.Female
		},
		{
			"mentor",
			LocGender.Male
		},
		{
			"sawyer",
			LocGender.Male
		},
		{
			"joe",
			LocGender.Male
		},
		{
			"george",
			LocGender.Male
		},
		{
			"serpico",
			LocGender.Male
		},
		{
			"nellis",
			LocGender.Female
		},
		{
			"grandpa_cheng",
			LocGender.Male
		},
		{
			"landlord",
			LocGender.Male
		},
		{
			"officer_lun",
			LocGender.Male
		},
		{
			"merchant",
			LocGender.Male
		},
		{
			"shadyMerchant",
			LocGender.Male
		},
		{
			"admin",
			LocGender.Female
		},
		{
			"greytide",
			LocGender.Male
		},
		{
			"scavElder",
			LocGender.Female
		},
		{
			"waffle_enjoyer",
			LocGender.Male
		},
		{
			"case_enjoyer",
			LocGender.Male
		},
		{
			"serpico_scav",
			LocGender.Male
		},
		{
			"serpico_rev",
			LocGender.Male
		},
		{
			"courrier",
			LocGender.Male
		},
		{
			"inventor",
			LocGender.Male
		},
		{
			"retired_farmer",
			LocGender.Male
		},
		{
			"retired_chemist",
			LocGender.Female
		},
		{
			"retired_aquafarmer",
			LocGender.Female
		},
		{
			"cartel_handler",
			LocGender.Male
		},
		{
			"blackmailer",
			LocGender.Female
		},
		{
			"wanted1",
			LocGender.Male
		},
		{
			"wanted2",
			LocGender.Male
		},
		{
			"wanted3",
			LocGender.Male
		},
		{
			"wanted4",
			LocGender.Male
		},
		{
			"wanted5",
			LocGender.Female
		},
		{
			"lou_sec",
			LocGender.Female
		},
		{
			"wanted6",
			LocGender.Male
		},
		{
			"wanted7",
			LocGender.Male
		},
		{
			"wanted8",
			LocGender.Male
		},
		{
			"wanted9",
			LocGender.Male
		},
		{
			"wanted_bait1",
			LocGender.Male
		},
		{
			"trailer_scav1",
			LocGender.Female
		},
		{
			"trailer_scav4",
			LocGender.Female
		},
		{
			"trailer_scav5",
			LocGender.Female
		},
		{
			"trailer_rev1",
			LocGender.Male
		},
		{
			"billy",
			LocGender.Neutral
		},
		{
			"sha3",
			LocGender.Neutral
		},
		{
			"cul1",
			LocGender.Neutral
		}
	};

	public static LocGender GetGenderFromSpriteName(string spriteName)
	{
		if (string.IsNullOrEmpty(spriteName))
		{
			return LocGender.Neutral;
		}
		if (uniqueSpriteGender.TryGetValue(spriteName, out var value))
		{
			return value;
		}
		if (spriteName.IndexOf("female", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return LocGender.Female;
		}
		if (spriteName.IndexOf("male", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return LocGender.Male;
		}
		return LocGender.Neutral;
	}

	public static LocGender GetClientGender(StoreClient storeClient)
	{
		if (storeClient == null)
		{
			return LocGender.Neutral;
		}
		return GetGenderFromSpriteName(storeClient.GetSprite());
	}

	public static LocGender GetCurrentClientGender()
	{
		if (PlayerStore.Instance == null || PlayerStore.Instance.currentClientInstance == null)
		{
			return LocGender.Neutral;
		}
		return GetClientGender(PlayerStore.Instance.currentClientInstance.GetClientBlueprint());
	}

	public static LocRegister GetClientRegister(StoreClient storeClient)
	{
		if (storeClient == null)
		{
			return LocRegister.Formal;
		}
		if (storeClient.IsTag("SPEAKS_FORMAL"))
		{
			return LocRegister.Formal;
		}
		if (storeClient.IsTag("SPEAKS_INFORMAL"))
		{
			return LocRegister.Informal;
		}
		if (storeClient.isSecurity)
		{
			return LocRegister.Formal;
		}
		return GetFactionRegister(storeClient.clientFaction);
	}

	public static LocRegister GetFactionRegister(string faction)
	{
		switch (faction)
		{
		case "FACTION_UPPER_LEVEL":
		case "FACTION_TOURIST":
		case "FACTION_SECURITY":
		case "FACTION_CHURCH":
			return LocRegister.Formal;
		case "FACTION_LOWER_LEVEL":
		case "FACTION_BLACK_MARKET":
		case "FACTION_REVOLUTION":
		case "FACTION_CARTEL":
			return LocRegister.Informal;
		default:
			return LocRegister.Formal;
		}
	}

	public static LocRegister GetCurrentClientRegister()
	{
		if (PlayerStore.Instance == null || PlayerStore.Instance.currentClientInstance == null)
		{
			return LocRegister.Formal;
		}
		return GetClientRegister(PlayerStore.Instance.currentClientInstance.GetClientBlueprint());
	}

	public static string Resolve(string text, LocGender gender)
	{
		if (string.IsNullOrEmpty(text) || !text.Contains("[g:"))
		{
			return text;
		}
		return genderTokenRegex.Replace(text, delegate(Match match)
		{
			string value = match.Groups[1].Value;
			string value2 = match.Groups[2].Value;
			string text2 = (match.Groups[3].Success ? match.Groups[3].Value : value);
			return gender switch
			{
				LocGender.Male => value, 
				LocGender.Female => value2, 
				_ => text2, 
			};
		});
	}

	public static string ResolveRegister(string text, LocRegister register)
	{
		if (string.IsNullOrEmpty(text) || !text.Contains("[r:"))
		{
			return text;
		}
		return registerTokenRegex.Replace(text, (Match match) => (register != LocRegister.Informal) ? match.Groups[2].Value : match.Groups[1].Value);
	}

	public static string Resolve(string text, LocGender gender, LocRegister register)
	{
		return ResolveRegister(Resolve(text, gender), register);
	}

	public static string ResolveForCurrentClient(string text)
	{
		return Resolve(text, GetCurrentClientGender(), GetCurrentClientRegister());
	}

	public static string ResolveForSoldItemClient(string text, SoldItem soldItem)
	{
		if (soldItem == null)
		{
			return Resolve(text, LocGender.Neutral, LocRegister.Formal);
		}
		return Resolve(text, GetGenderFromSpriteName(soldItem.clientSprite), GetFactionRegister(soldItem.factionId));
	}

	public static void ResolveClientNames(StoreClient storeClient)
	{
		if (storeClient != null)
		{
			LocGender clientGender = GetClientGender(storeClient);
			LocRegister clientRegister = GetClientRegister(storeClient);
			storeClient.displayName = Resolve(storeClient.displayName, clientGender, clientRegister);
			storeClient.realName = Resolve(storeClient.realName, clientGender, clientRegister);
		}
	}

	public static void ResolveDialogueLine(Dialogue dialogue, LocGender gender, LocRegister register = LocRegister.Formal)
	{
		if (dialogue != null)
		{
			dialogue.plainText = Resolve(dialogue.plainText, gender, register);
		}
	}

	public static void ResolveDialogueLine(Dialogue dialogue)
	{
		ResolveDialogueLine(dialogue, GetCurrentClientGender(), GetCurrentClientRegister());
	}
}

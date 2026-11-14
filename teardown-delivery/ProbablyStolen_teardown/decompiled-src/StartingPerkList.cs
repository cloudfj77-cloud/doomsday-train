using System.Collections.Generic;
using UnityEngine;

public class StartingPerkList
{
	public const string SAVING = "saving";

	public const string BAD_LEASE = "bad_lease";

	public const string LOAN = "loan";

	public const string DEBT = "debt";

	public const string CONVICT = "convict";

	public const string MODEL_CITIZEN = "model_citizen";

	public const string WELL_CONNECTED = "well_connected";

	public const string ALERT = "alert";

	public const string SAWYER_CREW = "sawyer_crew";

	public const string DIPLOMATIC = "diplomatic";

	public const string INTIMIDATING = "intimidating";

	public const string SIDE_STREET = "side_street";

	public const string NICOTINE_ADDICTION = "nicotine_addiction";

	public const string NARCOTIC_ADDICTION = "narcotic_addiction";

	public const string ALCOHOL_ADDICTION = "alcohol_addiction";

	public const string SECRET_ADMIRER = "secret_admirer";

	public const string COLORFUL_CHARACTER = "colorful_character";

	public const string BLACKMAIL = "blackmail";

	public const string BLABBERMOUTH = "blabbermouth";

	public const string UNSUBSCRIBED = "unsubscribed";

	public const string MINIMALIST = "minimalist";

	public const string SENTINEL = "sentinel";

	public const string SPEEDY = "speedy";

	public const string AGORAPHOBIA = "agoraphobia";

	public const string WELFARE_PROGRAM = "welfare_program";

	public const string PROFICIENT_SCAVENGER = "proficient_scavenger";

	public const string REVOLUTION_SYMPATHIZER = "revolution_sympathizer";

	public const string GAMBLING_ADDICTION = "gambling_addiction";

	public const string TAPEHEAD = "tapehead";

	public const string EXPEDITION_ORGANIZER = "expedition_organizer";

	public const string VENDING = "vending";

	public const string HANDY = "handy";

	public const string PUBLIC_ENEMY = "public_enemy";

	public const string ASCETIC = "ascetic";

	public const string MAIMED = "maimed";

	public const string WILD_FAVORITE = "wild_favorite";

	public const string WILD_BAD_SIDE = "wild_bad_side";

	public const string AUG_MENACE = "aug_menace";

	public const string LUCKY = "lucky";

	public const string UNLUCKY = "unlucky";

	public const string TINKERER = "tinkerer";

	public const string CAREFUL_SCAVENGER = "careful_scavenger";

	public const string SYCOPHANT = "sycophant";

	public const string AWFUL_NEGOTIATOR = "awful_negotiator";

	public const string BANNED_COMMISSARY = "banned_commissary";

	public const string HOARDER = "hoarder";

	public const string MAKESHIFT_REACTOR = "makeshift_reactor";

	public const string SHORT_SLEEPER = "short_sleeper";

	public const string WORKING_PIPES = "working_pipes";

	public const string ILLITERATE = "illiterate";

	public const string PRIME_REAL_ESTATE = "prime_real_estate";

	public const string DEGENERATIVE_OSTEOARTHRITIS = "degenerative_osteoarthritis";

	public const string KNOWN_SNITCH = "known_snitch";

	public const int SAVING_CREDITS = 150;

	public const int BAD_LEASE_VALUE = 40;

	public const int CONVICT_VALUE = -30;

	public const int CONVICT_VALUE_REWARD = 25;

	public const int MODEL_CITIZEN_VALUE = 30;

	public const int MODEL_CITIZEN_VALUE_REWARD = 50;

	public const int WELL_CONNECTED_VALUE = 15;

	public const int PUBLIC_ENEMY_VALUE = -15;

	public const int DIPLOMATIC_VALUE = 33;

	public const int INTIMIDATING_VALUE = 33;

	public const int ADDICTION_PENALTY = 8;

	public const int COLORFUL_CHARACTER_VALUE = 3;

	public const int BLABBERMOUTH_VALUE = 50;

	public const int BLACKMAIL_VALUE = -30;

	public const int REVOLUTION_SYMPATHIZER_VALUE = 25;

	public const int PRIME_REAL_ESTATE_VALUE = 75;

	public const int KNOWN_SNITCH_REP_PENALTY = -10;

	public const int OSTEOARTHRITIS_MIN_ATTEMPTS = 3;

	public const int OSTEOARTHRITIS_BANDAGE_COUNT = 2;

	public const int HANDY_TOOLBOX_SLOTS = 4;

	public const int MINIMALIST_INVENTORY_WIDTH = 20;

	public const int ASCETIC_INVENTORY_WIDTH = 16;

	public const int WILD_FAVORITE_UPGRADE_DISCOUNT = 10;

	public const int WILD_FAVORITE_INSURANCE_DISCOUNT = 50;

	public const int WILD_BAD_SIDE_UPGRADE_MARKUP = 10;

	public const int WILD_BAD_SIDE_INSURANCE_MARKUP = 50;

	public const int AUG_MENACE_KILL_REWARD_BONUS = 25;

	public const int AUG_MENACE_INTRODUCED_DAY = 1;

	public const int AUG_MENACE_FIRST_AUG_MIN_DAYS = 2;

	public const int AUG_MENACE_FIRST_AUG_MAX_DAYS = 4;

	public const int LUCKY_LUCK = 15;

	public const int UNLUCKY_LUCK = -15;

	public const int TINKERER_KEEP_CHANCE = 20;

	public const int CAREFUL_SCAVENGER_ATTEMPT_PENALTY = 2;

	public const int SYCOPHANT_START_REP = 15;

	public const int SYCOPHANT_REP_GAIN_BONUS = 25;

	public const int SYCOPHANT_BUDGET_BONUS = 10;

	public const int AWFUL_NEGOTIATOR_PRICE_PENALTY = 5;

	public const int AGORAPHOBIA_REST_NIGHTS = 2;

	[ES3NonSerializable]
	public static List<StartingPerk> Perks;

	private static readonly List<StartingPerk> modPerks = new List<StartingPerk>();

	public static void InitStartingPerk()
	{
		Perks = new List<StartingPerk>
		{
			Saving(),
			BadLease(),
			Convict(),
			ModelCitizen(),
			WellConnected(),
			PublicEnemy(),
			Alert(),
			SawyerCrew(),
			Diplomatic(),
			Intimidating(),
			SideStreet(),
			NicotineAddiction(),
			NarcoticAddiction(),
			AlcoholAddiction(),
			SecretAdmirer(),
			ColorfulCharacter(),
			Blackmail(),
			Blabbermouth(),
			Minimalist(),
			Ascetic(),
			Sentinel(),
			WelfareProgram(),
			ProficientScavenger(),
			Maimed(),
			GamblingAddiction(),
			Tapehead(),
			Handy(),
			WildFavorite(),
			WildBadSide(),
			AugMenace(),
			Lucky(),
			Unlucky(),
			Tinkerer(),
			CarefulScavenger(),
			Sycophant(),
			AwfulNegotiator(),
			BannedCommissary(),
			Hoarder(),
			MakeshiftReactor()
		};
		Perks.Add(ShortSleeper());
		Perks.Add(WorkingPipes());
		Perks.Add(Illiterate());
		Perks.Add(PrimeRealEstate());
		Perks.Add(VendingMachine());
		Perks.Add(RevolutionSympathizer());
		Perks.Add(Agoraphobia());
		Perks.Add(DegenerativeOsteoarthritis());
		Perks.Add(KnownSnitch());
		Perks.AddRange(modPerks);
	}

	public static void RegisterModPerk(StartingPerk perk)
	{
		if (perk == null || string.IsNullOrEmpty(perk.id))
		{
			UnityEngine.Debug.LogError("[StartingPerkList] A mod perk needs an id");
			return;
		}
		foreach (StartingPerk item in AllPerkDefinitions())
		{
			if (item.id == perk.id)
			{
				UnityEngine.Debug.LogError("[StartingPerkList] '" + perk.id + "' is a vanilla perk id; pick another (prefix it with the mod id)");
				return;
			}
		}
		modPerks.RemoveAll((StartingPerk p) => p.id == perk.id);
		modPerks.Add(perk);
		if (Perks != null)
		{
			Perks.RemoveAll((StartingPerk p) => p.id == perk.id);
			Perks.Add(perk);
		}
	}

	public static int RemoveModPerks(string modId)
	{
		if (string.IsNullOrEmpty(modId))
		{
			return 0;
		}
		int result = modPerks.RemoveAll((StartingPerk p) => p.source == modId);
		if (Perks != null)
		{
			Perks.RemoveAll((StartingPerk p) => p.source == modId);
		}
		return result;
	}

	public static StartingPerk GetModPerk(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		foreach (StartingPerk modPerk in modPerks)
		{
			if (modPerk.id == id)
			{
				return modPerk;
			}
		}
		return null;
	}

	public static List<StartingPerk> AllPerkDefinitions()
	{
		return new List<StartingPerk>
		{
			Saving(),
			BadLease(),
			Convict(),
			ModelCitizen(),
			WellConnected(),
			Alert(),
			SawyerCrew(),
			Diplomatic(),
			Intimidating(),
			SideStreet(),
			NicotineAddiction(),
			NarcoticAddiction(),
			AlcoholAddiction(),
			SecretAdmirer(),
			ColorfulCharacter(),
			Blackmail(),
			Blabbermouth(),
			Minimalist(),
			Sentinel(),
			Agoraphobia(),
			WelfareProgram(),
			ProficientScavenger(),
			RevolutionSympathizer(),
			GamblingAddiction(),
			Tapehead(),
			VendingMachine(),
			ShortSleeper(),
			WorkingPipes(),
			Illiterate(),
			PrimeRealEstate(),
			DegenerativeOsteoarthritis(),
			KnownSnitch(),
			Handy(),
			PublicEnemy(),
			Ascetic(),
			Maimed(),
			WildFavorite(),
			WildBadSide(),
			AugMenace(),
			Lucky(),
			Unlucky(),
			Tinkerer(),
			CarefulScavenger(),
			Sycophant(),
			AwfulNegotiator(),
			BannedCommissary(),
			Hoarder(),
			MakeshiftReactor()
		};
	}

	public static List<StartingPerk> AllPerkDefinitionsWithMods()
	{
		List<StartingPerk> list = AllPerkDefinitions();
		list.AddRange(modPerks);
		return list;
	}

	public static StartingPerk Saving()
	{
		return new StartingPerk
		{
			id = "saving",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE
		};
	}

	public static StartingPerk BadLease()
	{
		return new StartingPerk
		{
			id = "bad_lease",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE
		};
	}

	public static StartingPerk Convict()
	{
		return new StartingPerk
		{
			id = "convict",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			incompatiblePerks = new List<string> { "model_citizen" }
		};
	}

	public static StartingPerk ModelCitizen()
	{
		return new StartingPerk
		{
			id = "model_citizen",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE,
			incompatiblePerks = new List<string> { "convict" }
		};
	}

	public static StartingPerk WellConnected()
	{
		return new StartingPerk
		{
			id = "well_connected",
			cost = 2,
			type = StartingPerk.StartingPerkType.POSITIVE,
			incompatiblePerks = new List<string> { "public_enemy" }
		};
	}

	public static StartingPerk Sycophant()
	{
		return new StartingPerk
		{
			id = "sycophant",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE
		};
	}

	public static StartingPerk AwfulNegotiator()
	{
		return new StartingPerk
		{
			id = "awful_negotiator",
			cost = -3,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			mindEditable = true
		};
	}

	public static StartingPerk PublicEnemy()
	{
		return new StartingPerk
		{
			id = "public_enemy",
			cost = -2,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			incompatiblePerks = new List<string> { "well_connected" }
		};
	}

	public static StartingPerk Alert()
	{
		return new StartingPerk
		{
			id = "alert",
			cost = 2,
			type = StartingPerk.StartingPerkType.POSITIVE,
			mindEditable = true
		};
	}

	public static StartingPerk SawyerCrew()
	{
		return new StartingPerk
		{
			id = "sawyer_crew",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE
		};
	}

	public static StartingPerk Diplomatic()
	{
		return new StartingPerk
		{
			id = "diplomatic",
			cost = 2,
			type = StartingPerk.StartingPerkType.POSITIVE,
			mindEditable = true
		};
	}

	public static StartingPerk Intimidating()
	{
		return new StartingPerk
		{
			id = "intimidating",
			cost = 2,
			type = StartingPerk.StartingPerkType.POSITIVE,
			mindEditable = true
		};
	}

	public static StartingPerk SideStreet()
	{
		return new StartingPerk
		{
			id = "side_street",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			incompatiblePerks = new List<string> { "prime_real_estate" }
		};
	}

	public static StartingPerk NicotineAddiction()
	{
		return new StartingPerk
		{
			id = "nicotine_addiction",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			mindEditable = true
		};
	}

	public static StartingPerk NarcoticAddiction()
	{
		return new StartingPerk
		{
			id = "narcotic_addiction",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			mindEditable = true
		};
	}

	public static StartingPerk AlcoholAddiction()
	{
		return new StartingPerk
		{
			id = "alcohol_addiction",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			mindEditable = true
		};
	}

	public static StartingPerk SecretAdmirer()
	{
		return new StartingPerk
		{
			id = "secret_admirer",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE
		};
	}

	public static StartingPerk ColorfulCharacter()
	{
		return new StartingPerk
		{
			id = "colorful_character",
			cost = 1,
			maxSlot = 3,
			type = StartingPerk.StartingPerkType.NEUTRAL
		};
	}

	public static StartingPerk Blackmail()
	{
		return new StartingPerk
		{
			id = "blackmail",
			cost = -2,
			type = StartingPerk.StartingPerkType.NEGATIVE
		};
	}

	public static StartingPerk Blabbermouth()
	{
		return new StartingPerk
		{
			id = "blabbermouth",
			cost = -2,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			mindEditable = true
		};
	}

	public static StartingPerk Minimalist()
	{
		return new StartingPerk
		{
			id = "minimalist",
			cost = -2,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			incompatiblePerks = new List<string> { "ascetic" },
			mindEditable = true
		};
	}

	public static StartingPerk Ascetic()
	{
		return new StartingPerk
		{
			id = "ascetic",
			cost = -4,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			incompatiblePerks = new List<string> { "minimalist" },
			mindEditable = true
		};
	}

	public static StartingPerk Sentinel()
	{
		return new StartingPerk
		{
			id = "sentinel",
			cost = -3,
			type = StartingPerk.StartingPerkType.NEGATIVE
		};
	}

	public static StartingPerk Agoraphobia()
	{
		return new StartingPerk
		{
			id = "agoraphobia",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			incompatiblePerks = new List<string> { "speedy" },
			mindEditable = true
		};
	}

	public static StartingPerk WelfareProgram()
	{
		return new StartingPerk
		{
			id = "welfare_program",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE,
			incompatiblePerks = new List<string> { "banned_commissary" }
		};
	}

	public static StartingPerk MakeshiftReactor()
	{
		return new StartingPerk
		{
			id = "makeshift_reactor",
			cost = 1,
			type = StartingPerk.StartingPerkType.NEUTRAL
		};
	}

	public static StartingPerk Hoarder()
	{
		return new StartingPerk
		{
			id = "hoarder",
			cost = 0,
			type = StartingPerk.StartingPerkType.NEUTRAL
		};
	}

	public static StartingPerk BannedCommissary()
	{
		return new StartingPerk
		{
			id = "banned_commissary",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			incompatiblePerks = new List<string> { "welfare_program" }
		};
	}

	public static StartingPerk ProficientScavenger()
	{
		return new StartingPerk
		{
			id = "proficient_scavenger",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE,
			incompatiblePerks = new List<string> { "maimed" },
			mindEditable = true
		};
	}

	public static StartingPerk CarefulScavenger()
	{
		return new StartingPerk
		{
			id = "careful_scavenger",
			cost = 0,
			type = StartingPerk.StartingPerkType.NEUTRAL,
			incompatiblePerks = new List<string> { "maimed" },
			mindEditable = true
		};
	}

	public static StartingPerk Maimed()
	{
		return new StartingPerk
		{
			id = "maimed",
			cost = -2,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			incompatiblePerks = new List<string> { "proficient_scavenger", "degenerative_osteoarthritis", "careful_scavenger" }
		};
	}

	public static StartingPerk RevolutionSympathizer()
	{
		return new StartingPerk
		{
			id = "revolution_sympathizer",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE
		};
	}

	public static StartingPerk GamblingAddiction()
	{
		return new StartingPerk
		{
			id = "gambling_addiction",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			mindEditable = true
		};
	}

	public static StartingPerk Tapehead()
	{
		return new StartingPerk
		{
			id = "tapehead",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE
		};
	}

	public static StartingPerk WildFavorite()
	{
		return new StartingPerk
		{
			id = "wild_favorite",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE,
			incompatiblePerks = new List<string> { "wild_bad_side" }
		};
	}

	public static StartingPerk WildBadSide()
	{
		return new StartingPerk
		{
			id = "wild_bad_side",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			incompatiblePerks = new List<string> { "wild_favorite" }
		};
	}

	public static StartingPerk AugMenace()
	{
		return new StartingPerk
		{
			id = "aug_menace",
			cost = 1,
			type = StartingPerk.StartingPerkType.NEUTRAL
		};
	}

	public static StartingPerk Lucky()
	{
		return new StartingPerk
		{
			id = "lucky",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE,
			incompatiblePerks = new List<string> { "unlucky" }
		};
	}

	public static StartingPerk Unlucky()
	{
		return new StartingPerk
		{
			id = "unlucky",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			incompatiblePerks = new List<string> { "lucky" }
		};
	}

	public static StartingPerk Handy()
	{
		return new StartingPerk
		{
			id = "handy",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE
		};
	}

	public static StartingPerk Tinkerer()
	{
		return new StartingPerk
		{
			id = "tinkerer",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE
		};
	}

	public static StartingPerk VendingMachine()
	{
		return new StartingPerk
		{
			id = "vending",
			cost = 2,
			type = StartingPerk.StartingPerkType.POSITIVE
		};
	}

	public static StartingPerk ShortSleeper()
	{
		return new StartingPerk
		{
			id = "short_sleeper",
			cost = 1,
			type = StartingPerk.StartingPerkType.POSITIVE,
			mindEditable = true
		};
	}

	public static StartingPerk WorkingPipes()
	{
		return new StartingPerk
		{
			id = "working_pipes",
			cost = 2,
			type = StartingPerk.StartingPerkType.POSITIVE
		};
	}

	public static StartingPerk Illiterate()
	{
		return new StartingPerk
		{
			id = "illiterate",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			mindEditable = true
		};
	}

	public static StartingPerk PrimeRealEstate()
	{
		return new StartingPerk
		{
			id = "prime_real_estate",
			cost = 0,
			type = StartingPerk.StartingPerkType.NEUTRAL,
			incompatiblePerks = new List<string> { "side_street" }
		};
	}

	public static StartingPerk DegenerativeOsteoarthritis()
	{
		return new StartingPerk
		{
			id = "degenerative_osteoarthritis",
			cost = -1,
			type = StartingPerk.StartingPerkType.NEGATIVE,
			incompatiblePerks = new List<string> { "maimed" }
		};
	}

	public static StartingPerk KnownSnitch()
	{
		return new StartingPerk
		{
			id = "known_snitch",
			cost = -2,
			type = StartingPerk.StartingPerkType.NEGATIVE
		};
	}
}

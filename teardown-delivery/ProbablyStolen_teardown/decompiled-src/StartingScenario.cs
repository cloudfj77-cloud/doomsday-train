using System;
using System.Collections.Generic;

public class StartingScenario
{
	public enum TagTone
	{
		Positive,
		Warning,
		Danger
	}

	public string id;

	public NewGameData.StartType startType = NewGameData.StartType.MODDED;

	public string source;

	public string iconKey;

	public Func<string> titleProvider;

	public Func<string> descriptionProvider;

	public readonly List<Func<string>> startWithProviders = new List<Func<string>>();

	public Func<string> tagProvider;

	public TagTone tagTone = TagTone.Warning;

	public bool comingSoon;

	public bool skipIntro;

	public bool hidden;

	public Action onStart;

	private static List<StartingScenario> vanilla;

	private static readonly List<StartingScenario> modded = new List<StartingScenario>();

	public bool IsMod => startType == NewGameData.StartType.MODDED;

	public string GetTitle()
	{
		return Resolve(titleProvider);
	}

	public string GetDescription()
	{
		return Resolve(descriptionProvider);
	}

	public string GetTag()
	{
		return Resolve(tagProvider);
	}

	public List<string> GetStartWith()
	{
		List<string> list = new List<string>();
		foreach (Func<string> startWithProvider in startWithProviders)
		{
			string text = Resolve(startWithProvider);
			if (!string.IsNullOrEmpty(text))
			{
				list.Add(text);
			}
		}
		return list;
	}

	public bool IsAvailableInBuild()
	{
		return true;
	}

	public bool CanStart()
	{
		if (comingSoon || !IsAvailableInBuild())
		{
			return false;
		}
		if (NewGameData.Instance != null && NewGameData.Instance.isShowcase && startType != NewGameData.StartType.APPRENTICE)
		{
			return false;
		}
		return true;
	}

	private string Resolve(Func<string> provider)
	{
		if (provider == null)
		{
			return "";
		}
		try
		{
			return provider() ?? "";
		}
		catch (Exception arg)
		{
			Debug.LogError($"[StartingScenario] Text provider of '{id}' threw: {arg}");
			return "";
		}
	}

	public static List<StartingScenario> GetAll()
	{
		List<StartingScenario> list = new List<StartingScenario>(Vanilla());
		list.AddRange(modded);
		return list;
	}

	public static List<StartingScenario> GetVisible()
	{
		return GetAll().FindAll((StartingScenario s) => !s.hidden);
	}

	public static StartingScenario Get(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		foreach (StartingScenario item in modded)
		{
			if (item.id == id)
			{
				return item;
			}
		}
		foreach (StartingScenario item2 in Vanilla())
		{
			if (item2.id == id)
			{
				return item2;
			}
		}
		return null;
	}

	public static StartingScenario GetByStartType(NewGameData.StartType startType)
	{
		foreach (StartingScenario item in Vanilla())
		{
			if (item.startType == startType)
			{
				return item;
			}
		}
		return null;
	}

	public static string ResolveId(NewGameData.StartType startType, string startScenarioId)
	{
		if (!string.IsNullOrEmpty(startScenarioId))
		{
			return startScenarioId;
		}
		StartingScenario byStartType = GetByStartType(startType);
		if (byStartType == null)
		{
			return "";
		}
		return byStartType.id;
	}

	public static StartingScenario GetCurrent()
	{
		if (PlayerStore.Instance != null)
		{
			string value = ResolveId(PlayerStore.Instance.startType, PlayerStore.Instance.startScenarioId);
			if (!string.IsNullOrEmpty(value))
			{
				return Get(value);
			}
		}
		if (NewGameData.Instance != null)
		{
			return Get(ResolveId(NewGameData.Instance.startType, NewGameData.Instance.startScenarioId));
		}
		return null;
	}

	public static void Register(StartingScenario scenario)
	{
		if (scenario == null || string.IsNullOrEmpty(scenario.id))
		{
			Debug.LogError("[StartingScenario] A scenario needs an id");
			return;
		}
		if (scenario.startType != NewGameData.StartType.MODDED)
		{
			Debug.LogError("[StartingScenario] Mod scenario '" + scenario.id + "' must use StartType.MODDED");
			return;
		}
		foreach (StartingScenario item in Vanilla())
		{
			if (item.id == scenario.id)
			{
				Debug.LogError("[StartingScenario] '" + scenario.id + "' is a vanilla scenario id; pick another (prefix it with the mod id)");
				return;
			}
		}
		modded.RemoveAll((StartingScenario s) => s.id == scenario.id);
		modded.Add(scenario);
	}

	public static int RemoveMod(string modId)
	{
		if (string.IsNullOrEmpty(modId))
		{
			return 0;
		}
		return modded.RemoveAll((StartingScenario s) => s.source == modId);
	}

	private static List<StartingScenario> Vanilla()
	{
		if (vanilla != null)
		{
			return vanilla;
		}
		List<StartingScenario> list = new List<StartingScenario>();
		list.Add(Create("apprentice", NewGameData.StartType.APPRENTICE, "generalist", "generalist_body", new string[3] { "generalist_gear", "generalist_gear2", "generalist_gear3" }, "new_player_hint", TagTone.Positive));
		list.Add(Create("water_merchant", NewGameData.StartType.WATER_MERCHANT, "water_merchant", "water_merchant_body", new string[3] { "water_merchant_gear", "water_merchant_gear1", "water_merchant_gear2" }));
		list.Add(Create("fence", NewGameData.StartType.FENCE, "fence", "fence_body", new string[6] { "fence_gear", "fence_gear2", "fence_gear3", "fence_gear4", "fence_gear5", "fence_gear6" }, null, TagTone.Warning, comingSoon: false, skipIntro: true));
		list.Add(Create("rock_bottom", NewGameData.StartType.ROCK_BOTTOM, "rock_bottom", "rock_bottom_body", new string[1] { "rock_bottom_gear" }, "extreme_challenge_start_note", TagTone.Danger, comingSoon: false, skipIntro: true));
		list.Add(Create("hydroponic_farmer", NewGameData.StartType.FARMER, "hydroponic_farmer", "hydroponic_farmer_body", new string[4] { "hydroponic_farmer_gear", "hydroponic_farmer_gear2", "hydroponic_farmer_gear3", "hydroponic_farmer_gear4" }, "advanced_start"));
		list.Add(Create("gunsmith", NewGameData.StartType.GUNSMITH, "gunsmith", "gunsmith_body", new string[3] { "gunsmith_gear", "gunsmith_gear2", "gunsmith_gear3" }, "advanced_start", TagTone.Warning, comingSoon: false, skipIntro: true));
		list.Add(Create("chemist", NewGameData.StartType.DRUG_DEALER, "chemist", "chemist_body", new string[2] { "chemist_gear", "chemist_gear2" }, "advanced_start", TagTone.Warning, comingSoon: false, skipIntro: true));
		list.Add(Create("rancher", NewGameData.StartType.RANCHER, "rancher", "rancher_body", new string[3] { "rancher_gear1", "rancher_gear2", "rancher_gear3" }, "advanced_start"));
		list.Add(CreatePending("aquafarmer", NewGameData.StartType.AQUAFARMER, "aquafarmer", "Aquafarmer", "aquafarmer_body", "During your training with The Mentor, you learned to raise fish in tanks for meat. You start with a small recirculating system and a stock of starter fish.", new(string, string)[3]
		{
			("aquafarmer_gear", "Recirculating Aquaculture Systems"),
			("aquafarmer_gear2", "Start with a stock of Tilapia and Fish Food"),
			("aquafarmer_gear3", "Increased starting rent")
		}, "icons_aquaculture", "advanced_start"));
		list.Add(Create("junker", NewGameData.StartType.SCAV, "junker", "junker_body", new string[3] { "junker_gear", "junker_gear2", "junker_gear3" }, "wip", TagTone.Warning, comingSoon: true));
		list.Add(Create("organ_trader", NewGameData.StartType.ORGAN_TRADER, "organ_dealer", "organ_trader_body", new string[3] { "organ_trader_gear", "organ_trader_gear2", "organ_trader_gear3" }, "wip", TagTone.Warning, comingSoon: true));
		list.Add(Create("street_food_vendor", NewGameData.StartType.STREET_FOOD_VENDOR, "street_food_vendor", "street_food_vendor_body", new string[3] { "street_food_vendor_gear", "street_food_vendor_gear2", "street_food_vendor_gear3" }, "wip", TagTone.Warning, comingSoon: true));
		list.Add(Create("moonshiner", NewGameData.StartType.MOONSHINER, "moonshiner", "moonshiner_body", new string[2] { "moonshiner_gear", "moonshiner_gear2" }, "wip", TagTone.Warning, comingSoon: true, skipIntro: false, hidden: true));
		list.Add(Create("pharmacist", NewGameData.StartType.PHARMACIST, "pharmacist", "pharmacist_body", new string[3] { "pharmacist_gear", "pharmacist_gear2", "pharmacist_gear3" }, "wip", TagTone.Warning, comingSoon: true, skipIntro: false, hidden: true));
		vanilla = list;
		return vanilla;
	}

	private static StartingScenario CreatePending(string id, NewGameData.StartType startType, string nameKey, string nameEnglish, string bodyKey, string bodyEnglish, (string key, string english)[] gear, string iconKey = null, string tagKey = null, TagTone tone = TagTone.Warning, bool comingSoon = false, bool skipIntro = false, bool hidden = false)
	{
		StartingScenario startingScenario = new StartingScenario
		{
			id = id,
			startType = startType,
			iconKey = iconKey,
			titleProvider = () => LocHelper.GetLocalizedOrDefault("NewGame", nameKey, nameEnglish),
			descriptionProvider = () => LocHelper.GetLocalizedOrDefault("NewGame", bodyKey, bodyEnglish),
			tagProvider = ((tagKey != null) ? ((Func<string>)(() => LocHelper.GetLocalizedNewGame(tagKey))) : null),
			tagTone = tone,
			comingSoon = comingSoon,
			skipIntro = skipIntro,
			hidden = hidden
		};
		foreach ((string, string) tuple in gear)
		{
			(string key, string english) captured = tuple;
			startingScenario.startWithProviders.Add(() => LocHelper.GetLocalizedOrDefault("NewGame", captured.key, captured.english));
		}
		return startingScenario;
	}

	private static StartingScenario Create(string id, NewGameData.StartType startType, string nameKey, string bodyKey, string[] gearKeys, string tagKey = null, TagTone tone = TagTone.Warning, bool comingSoon = false, bool skipIntro = false, bool hidden = false)
	{
		StartingScenario startingScenario = new StartingScenario
		{
			id = id,
			startType = startType,
			titleProvider = () => LocHelper.GetLocalizedNewGame(nameKey),
			descriptionProvider = () => LocHelper.GetLocalizedNewGame(bodyKey),
			tagProvider = ((tagKey != null) ? ((Func<string>)(() => LocHelper.GetLocalizedNewGame(tagKey))) : null),
			tagTone = tone,
			comingSoon = comingSoon,
			skipIntro = skipIntro,
			hidden = hidden
		};
		foreach (string text in gearKeys)
		{
			string key = text;
			startingScenario.startWithProviders.Add(() => LocHelper.GetLocalizedNewGame(key));
		}
		return startingScenario;
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ModHelper
{
	public const string MOD_ID_TAG = "MOD_ID_TAG";

	public const string MOD_NAME_TAG = "MOD_NAME_TAG";

	private static readonly HashSet<string> warnedLocalizationKeys = new HashSet<string>();

	public const string DEBUG_CATEGORY_STORE = "Store";

	public const string DEBUG_CATEGORY_COMMAND = "Command";

	public const string DEBUG_CATEGORY_SPAWN = "Spawn";

	public const string DEBUG_CATEGORY_SPECIAL = "Special";

	public const string DEBUG_CATEGORY_MODDING = "Modding";

	public const string DEBUG_CATEGORY_MARKETING = "Marketing";

	public const string OPTION_SECTION_GAMEPLAY = "gameplay";

	public const string OPTION_SECTION_AUDIO = "audio";

	public const string OPTION_SECTION_DISPLAY = "display";

	public const string OPTION_SECTION_DIALOGUE = "dialogue";

	public const string OPTION_SECTION_KEYBIND = "keybind";

	public static void AddCredit(int amount, bool playSound = true)
	{
		PlayerStore.Instance.ModCash(amount, playSound);
	}

	public static GameItem SpawnItem(string id)
	{
		return ItemSpawner.Spawn(id);
	}

	public static GameItem SetItemOwnerShip(GameItem item, bool isOwn)
	{
		GeneralHelper.SetItemOwned(item, isOwn);
		return item;
	}

	public static void MoveItemToFrontCounter(GameItem item)
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.frontInvinvElement, item);
	}

	public static void MoveItemToBackCounter(GameItem item)
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.backInvinvElement, item);
	}

	public static void MoveItemToInventory(GameItem item)
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, item);
	}

	public static void MoveItemToTrash(GameItem item)
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.trashInvElement, item);
	}

	public static void MoveItemToDisplayCase(GameItem item)
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.showcaseElement, item);
	}

	public static void MoveItemToAfterhours(GameItem item)
	{
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.afterhourInventory, item);
	}

	public static Locale GetCurrentLocale()
	{
		return LocalizationSettings.SelectedLocale;
	}

	public static string GetLocalized(string modId, string key, params object[] args)
	{
		LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId);
		string text = null;
		if (loadedMod != null)
		{
			Locale selectedLocale = LocalizationSettings.SelectedLocale;
			string key2 = ((selectedLocale != null) ? selectedLocale.Identifier.Code : "en");
			Dictionary<string, string> value3;
			string value4;
			if (loadedMod.Localization.TryGetValue(key2, out var value) && value.TryGetValue(key, out var value2))
			{
				text = value2;
			}
			else if (loadedMod.Localization.TryGetValue("en", out value3) && value3.TryGetValue(key, out value4))
			{
				text = value4;
			}
		}
		if (text == null)
		{
			if (warnedLocalizationKeys.Add(modId + ":" + key))
			{
				Debug.LogWarning("[ModHelper] Localization key '" + key + "' not found for mod '" + modId + "' (current locale or en). Returning the key itself.");
			}
			return key;
		}
		if (args != null && args.Length != 0)
		{
			try
			{
				return string.Format(text, args);
			}
			catch (FormatException)
			{
				if (warnedLocalizationKeys.Add(modId + ":" + key + ":format"))
				{
					Debug.LogWarning("[ModHelper] Localization entry '" + key + "' in mod '" + modId + "' is not a valid format string.");
				}
				return text;
			}
		}
		return text;
	}

	public static string GetSoundIdentifier(string modId, string soundKey)
	{
		return modId + ":" + soundKey;
	}

	public static bool PlaySound(string modId, string soundKey)
	{
		return AudioManager.Instance.Play(GetSoundIdentifier(modId, soundKey));
	}

	public static bool PlaySound2(string modId, string soundKey)
	{
		return AudioManager.Instance.Play2(GetSoundIdentifier(modId, soundKey));
	}

	public static bool PlaySound3(string modId, string soundKey)
	{
		return AudioManager.Instance.Play3(GetSoundIdentifier(modId, soundKey));
	}

	public static void SetSource(GameItem item, string modId, string modName)
	{
		item.ModifyTag("MOD_ID_TAG", delegate(TagState state)
		{
			state.SetEnabled(enabled: true).SetString(modId);
		});
		item.ModifyTag("MOD_NAME_TAG", delegate(TagState state)
		{
			state.SetEnabled(enabled: true).SetString(modName);
		});
	}

	public static void SetClientSource(StoreClient client, string modId)
	{
		if (client != null)
		{
			client.sourceModId = modId ?? "";
		}
	}

	public static void AddDebugFunction(string modId, string category, string name, Action action, bool tall = false)
	{
		CustomUIDebugPanel.Add(category, name, action, tall, modId);
	}

	public static int RemoveDebugFunctions(string modId)
	{
		return CustomUIDebugPanel.Remove(modId);
	}

	public static ModConfig GetConfig(string modId)
	{
		return ModConfig.For(modId);
	}

	public static string GameOptionId(string modId, string optionId)
	{
		return modId + ":" + optionId;
	}

	private static string GameOptionSectionId(string modId, string sectionId)
	{
		if (!GameOptionRegistry.IsVanillaSection(sectionId))
		{
			return modId + ":" + sectionId;
		}
		return sectionId;
	}

	public static void AddGameOptionSection(string modId, string sectionId, Func<string> label)
	{
		GameOptionRegistry.GetOrAddSection(GameOptionSectionId(modId, sectionId), label, modId);
	}

	public static void AddGameOptionToggle(string modId, string sectionId, string optionId, Func<string> label, Func<bool> get, Action<bool> set, Func<string> tooltip = null)
	{
		GameOptionRegistry.Entry entry = GameOptionRegistry.Toggle(GameOptionId(modId, optionId), label, get, set, tooltip);
		entry.source = modId;
		GameOptionRegistry.Add(GameOptionSectionId(modId, sectionId), entry);
	}

	public static void AddGameOptionSlider(string modId, string sectionId, string optionId, Func<string> label, Func<float> get, Action<float> set, float min = 0f, float max = 1f, bool wholeNumbers = false, Func<string> tooltip = null, Func<float, string> valueText = null)
	{
		GameOptionRegistry.Entry entry = GameOptionRegistry.Slider(GameOptionId(modId, optionId), label, get, set, min, max, wholeNumbers, tooltip, valueText);
		entry.source = modId;
		GameOptionRegistry.Add(GameOptionSectionId(modId, sectionId), entry);
	}

	public static void AddGameOptionSlider(string modId, string sectionId, string optionId, Func<string> label, Func<float> get, Action<float> set, float min, float max, bool wholeNumbers, Func<string> tooltip)
	{
		AddGameOptionSlider(modId, sectionId, optionId, label, get, set, min, max, wholeNumbers, tooltip, null);
	}

	public static void AddGameOptionDropdown(string modId, string sectionId, string optionId, Func<string> label, Func<IList<string>> options, Func<int> get, Action<int> set, Func<string> tooltip = null)
	{
		GameOptionRegistry.Entry entry = GameOptionRegistry.Dropdown(GameOptionId(modId, optionId), label, options, get, set, tooltip);
		entry.source = modId;
		GameOptionRegistry.Add(GameOptionSectionId(modId, sectionId), entry);
	}

	public static void AddGameOptionKeyBind(string modId, string sectionId, string optionId, Func<string> label, Func<KeyCode> get, Action<KeyCode> set, Func<string> tooltip = null)
	{
		GameOptionRegistry.Entry entry = GameOptionRegistry.KeyBind(GameOptionId(modId, optionId), label, get, set, tooltip);
		entry.source = modId;
		GameOptionRegistry.Add(GameOptionSectionId(modId, sectionId), entry);
	}

	public static bool RemoveGameOption(string modId, string optionId)
	{
		return GameOptionRegistry.Remove(GameOptionId(modId, optionId));
	}

	public static int RemoveGameOptions(string modId)
	{
		return GameOptionRegistry.RemoveBySource(modId);
	}

	public static void RegisterLootTable(string modId, string tableId, params (string itemId, float weight)[] entries)
	{
		LootRegistry.RegisterTable(modId, tableId, entries);
	}

	public static void AddNetworkUpgrade(string modId, string id, int cost, string subtitle, Func<string> title, Func<string> description, Action onUnlock, string tab = null, string iconKey = null, string[] prerequisites = null, int cooldownDays = 0, Func<string> subtitleLabel = null, Func<string> alreadyBought = null, bool recommended = false)
	{
		if (string.IsNullOrEmpty(modId) || string.IsNullOrEmpty(id))
		{
			Debug.LogError("[ModHelper] AddNetworkUpgrade needs a mod id and an upgrade id");
			return;
		}
		NetworkUpgrade networkUpgrade = new NetworkUpgrade(id)
		{
			cost = cost,
			subtitle = (subtitle ?? ""),
			tab = (tab ?? ""),
			iconKey = iconKey,
			source = modId,
			recommended = recommended,
			state = 1,
			titleProvider = title,
			descriptionProvider = description,
			subtitleProvider = subtitleLabel,
			alreadyBoughtProvider = alreadyBought,
			isRepeatable = (cooldownDays > 0),
			cooldownDuration = ((cooldownDays > 0) ? cooldownDays : 0)
		};
		if (prerequisites != null)
		{
			networkUpgrade.prerequisite.AddRange(prerequisites);
		}
		NetworkUpgrade.RegisterModUpgrade(networkUpgrade, onUnlock);
	}

	public static void SetNetworkTabLabel(string tab, Func<string> label)
	{
		NetworkUpgrade.RegisterTabLabel(tab, label);
	}

	public static int RemoveNetworkUpgrades(string modId)
	{
		return NetworkUpgrade.RemoveModUpgrades(modId);
	}

	public static bool IsNetworkUpgradeUnlocked(string id)
	{
		if (PlayerStore.Instance != null && PlayerStore.Instance.networkUpgrade != null)
		{
			return NetworkUpgrade.IsUnlocked(id);
		}
		return false;
	}

	public static NetworkUpgrade GetNetworkUpgrade(string id)
	{
		if (PlayerStore.Instance == null || PlayerStore.Instance.networkUpgrade == null || string.IsNullOrEmpty(id))
		{
			return null;
		}
		if (!PlayerStore.Instance.networkUpgrade.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public static void OpenWildNetwork()
	{
		WildNetworkUI.Open();
	}

	public static void CloseWildNetwork()
	{
		WildNetworkUI.Close();
	}

	public static void AddStartingScenario(string modId, string id, Func<string> title, Func<string> description, Func<string>[] startWith, Action onStart, string iconKey = null, Func<string> tag = null, StartingScenario.TagTone tagTone = StartingScenario.TagTone.Warning, bool skipIntro = false, bool comingSoon = false)
	{
		if (string.IsNullOrEmpty(modId) || string.IsNullOrEmpty(id))
		{
			Debug.LogError("[ModHelper] AddStartingScenario needs a mod id and a scenario id");
			return;
		}
		StartingScenario startingScenario = new StartingScenario
		{
			id = id,
			startType = NewGameData.StartType.MODDED,
			source = modId,
			iconKey = iconKey,
			titleProvider = title,
			descriptionProvider = description,
			tagProvider = tag,
			tagTone = tagTone,
			skipIntro = skipIntro,
			comingSoon = comingSoon,
			onStart = onStart
		};
		if (startWith != null)
		{
			startingScenario.startWithProviders.AddRange(startWith);
		}
		StartingScenario.Register(startingScenario);
	}

	public static int RemoveStartingScenarios(string modId)
	{
		return StartingScenario.RemoveMod(modId);
	}

	public static string GetStartingScenarioId()
	{
		if (PlayerStore.Instance == null)
		{
			return "";
		}
		return StartingScenario.ResolveId(PlayerStore.Instance.startType, PlayerStore.Instance.startScenarioId);
	}

	public static bool IsStartingScenario(string id)
	{
		if (!string.IsNullOrEmpty(id))
		{
			return GetStartingScenarioId() == id;
		}
		return false;
	}

	public static void OpenNewGameSpecialization()
	{
		NewGameSpecializationUI.Open();
	}

	public static void CloseNewGameSpecialization()
	{
		NewGameSpecializationUI.Close();
	}

	public static void AddStartingPerk(string modId, string id, int cost, StartingPerk.StartingPerkType type, Func<string> name, Func<string> description, string iconKey = null, string[] incompatiblePerks = null, int extraPerkSlots = 0, bool mindEditable = false)
	{
		if (string.IsNullOrEmpty(modId) || string.IsNullOrEmpty(id))
		{
			Debug.LogError("[ModHelper] AddStartingPerk needs a mod id and a perk id");
			return;
		}
		StartingPerk startingPerk = new StartingPerk
		{
			id = id,
			cost = cost,
			type = type,
			maxSlot = extraPerkSlots,
			mindEditable = mindEditable,
			source = modId,
			iconKey = iconKey,
			nameProvider = name,
			descriptionProvider = description
		};
		if (incompatiblePerks != null)
		{
			startingPerk.incompatiblePerks.AddRange(incompatiblePerks);
		}
		StartingPerkList.RegisterModPerk(startingPerk);
	}

	public static int RemoveStartingPerks(string modId)
	{
		return StartingPerkList.RemoveModPerks(modId);
	}

	public static bool IsStartingPerkActive(string id)
	{
		if (PlayerStore.Instance != null && PlayerStore.Instance.startingPerks != null && !string.IsNullOrEmpty(id))
		{
			return StartingPerk.IsPerkActive(id);
		}
		return false;
	}

	public static List<string> GetStartingPerkIds()
	{
		List<string> list = new List<string>();
		if (PlayerStore.Instance == null || PlayerStore.Instance.startingPerks == null)
		{
			return list;
		}
		foreach (StartingPerk startingPerk in PlayerStore.Instance.startingPerks)
		{
			list.Add(startingPerk.id);
		}
		return list;
	}

	public static void OpenNewGamePerks()
	{
		NewGamePerksUI.Open();
	}

	public static void CloseNewGamePerks()
	{
		NewGamePerksUI.Close();
	}

	public static void AddDifficulty(string modId, string id, int order, Func<string> title, Func<string> subtitle, Action<DifficultyRules> configure = null, string basedOn = "normal", Func<string>[] notes = null, string iconKey = null)
	{
		if (string.IsNullOrEmpty(modId) || string.IsNullOrEmpty(id))
		{
			Debug.LogError("[ModHelper] AddDifficulty needs a mod id and a difficulty id");
			return;
		}
		Difficulty difficulty = Difficulty.Get(basedOn) ?? Difficulty.Normal;
		Difficulty difficulty2 = new Difficulty
		{
			id = id,
			order = order,
			source = modId,
			titleProvider = title,
			subtitleProvider = subtitle,
			iconKey = iconKey,
			rules = difficulty.rules.Clone()
		};
		if (notes != null)
		{
			difficulty2.notes.AddRange(notes);
		}
		try
		{
			configure?.Invoke(difficulty2.rules);
		}
		catch (Exception ex)
		{
			Debug.LogError("[ModHelper] AddDifficulty configure of '" + id + "' threw: " + ex);
		}
		Difficulty.Register(difficulty2);
	}

	public static void ModifyDifficulty(string id, Action<Difficulty> modify)
	{
		Difficulty difficulty = Difficulty.Get(id);
		if (difficulty == null)
		{
			Debug.LogError("[ModHelper] ModifyDifficulty: no difficulty '" + id + "'");
			return;
		}
		try
		{
			modify?.Invoke(difficulty);
		}
		catch (Exception ex)
		{
			Debug.LogError("[ModHelper] ModifyDifficulty of '" + id + "' threw: " + ex);
		}
	}

	public static int RemoveDifficulties(string modId)
	{
		return Difficulty.RemoveMod(modId);
	}

	public static string GetDifficultyId()
	{
		return Difficulty.Current.id;
	}

	public static DifficultyRules GetDifficultyRules()
	{
		return Difficulty.Rules;
	}

	public static bool IsDifficultyAtLeast(string id)
	{
		return Difficulty.Current.IsAtLeast(id);
	}

	public static void AddAppraisalItem(string modId, string id, string itemId, int unlockRank, Func<GameItem> createRandom, Func<GameItem, int> getCorrectValue, Func<GameItem, bool> isType = null, string guideItemId = null, Func<string> displayName = null)
	{
		if (string.IsNullOrEmpty(modId) || string.IsNullOrEmpty(id))
		{
			Debug.LogError("[ModHelper] AddAppraisalItem needs a mod id and an appraisal item id");
			return;
		}
		AppraisalItemRegistry.Register(new AppraisalItemType
		{
			id = id,
			itemId = itemId,
			unlockRank = unlockRank,
			createRandom = createRandom,
			getCorrectValue = getCorrectValue,
			isType = isType,
			guideItemId = guideItemId,
			displayName = displayName,
			source = modId
		});
	}

	public static int RemoveAppraisalItems(string modId)
	{
		return AppraisalItemRegistry.RemoveAll(modId);
	}

	public static int GetAppraiserRank()
	{
		if (PlayerStore.Instance == null)
		{
			return 0;
		}
		return AppraisalRankHelper.GetRank();
	}

	public static DelegateAfterhourLocation AddAfterhourLocation(string modId, string id, Func<string> title, Action onArrive = null, Action onLeave = null, Action<CustomUIBuilder> buildPanel = null, string imageKey = null, string backgroundKey = null, string ambienceId = null, Func<bool> isDiscovered = null, Func<bool> isAvailable = null)
	{
		if (string.IsNullOrEmpty(modId) || string.IsNullOrEmpty(id))
		{
			Debug.LogError("[ModHelper] AddAfterhourLocation needs a mod id and a location id");
			return null;
		}
		DelegateAfterhourLocation obj = new DelegateAfterhourLocation
		{
			id = id,
			source = modId,
			title = title,
			imageKey = imageKey,
			backgroundKey = backgroundKey,
			ambienceId = (ambienceId ?? ""),
			isDiscovered = isDiscovered,
			isAvailable = isAvailable,
			onArrive = onArrive,
			onLeave = onLeave,
			buildPanel = buildPanel
		};
		AfterhourLocation.Register(obj);
		return obj;
	}

	public static void AddAfterhourLocation(AfterhourLocation location)
	{
		AfterhourLocation.Register(location);
	}

	public static PaperUI OpenPaper(string modId, string bookId)
	{
		return PaperBookRegistry.Open(PaperBookRegistry.FullId(modId, bookId));
	}

	public static PaperUI OpenPaper(string fullId)
	{
		return PaperBookRegistry.Open(fullId);
	}

	public static void ClosePaper(string modId, string bookId)
	{
		PaperBookRegistry.Close(PaperBookRegistry.FullId(modId, bookId));
	}

	public static bool IsPaperOpen(string modId, string bookId)
	{
		return PaperBookRegistry.IsOpen(PaperBookRegistry.FullId(modId, bookId));
	}

	public static PaperUI GetOpenPaper(string modId, string bookId)
	{
		return PaperBookRegistry.GetOpen(PaperBookRegistry.FullId(modId, bookId));
	}

	public static void OnPaperOpened(string modId, string bookId, Action<PaperUI> callback)
	{
		PaperBookRegistry.OnOpened(PaperBookRegistry.FullId(modId, bookId), callback);
	}

	public static void RegisterPaperBook(string modId, PaperBookData data)
	{
		PaperBookRegistry.Register(modId, data);
	}

	public static PaperBookData ParsePaperBook(string json)
	{
		return PaperJson.ParseBook(json, "ModHelper.ParsePaperBook");
	}

	public static int RemovePaperBooks(string modId)
	{
		return PaperBookRegistry.RemoveMod(modId);
	}

	public static List<string> GetPaperBookIds(string modId)
	{
		List<string> list = new List<string>();
		foreach (PaperBookRegistry.Entry item in PaperBookRegistry.OfMod(modId))
		{
			if (item.data != null)
			{
				list.Add(item.bookId);
			}
		}
		return list;
	}

	public static PaperUI CreatePaper(string modId, string bookId, string background, float width = 0f, float height = 0f)
	{
		return PaperUI.Create(PaperBookRegistry.FullId(modId, bookId), background, width, height, modId);
	}

	public static GameItem SpawnPaperItem(string modId, string bookId)
	{
		return PaperBookRegistry.SpawnItem(PaperBookRegistry.FullId(modId, bookId));
	}

	public static int RemoveAfterhourLocations(string modId)
	{
		return AfterhourLocation.RemoveSource(modId);
	}

	public static AfterhourLocation GetAfterhourLocation(string id)
	{
		return AfterhourLocation.Get(id);
	}

	public static string GetCurrentAfterhourLocationId()
	{
		if (!(MapUIManager.Instance != null) || MapUIManager.Instance.Current == null)
		{
			return "";
		}
		return MapUIManager.Instance.Current.Id;
	}

	public static bool IsOutsideStore()
	{
		if (MapUIManager.Instance != null)
		{
			return MapUIManager.Instance.IsOutside;
		}
		return false;
	}

	public static void ReturnFromAfterhour(bool askAboutGroundItems = true)
	{
		if (!(MapUIManager.Instance == null))
		{
			if (askAboutGroundItems)
			{
				MapUIManager.Instance.Return();
			}
			else
			{
				MapUIManager.Instance.ReturnNow();
			}
		}
	}

	public static bool RegisterAfterhourAmbience(string modId, string id, string soundKey)
	{
		LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest != null && m.Manifest.ID == modId);
		if (loadedMod == null || !loadedMod.Sounds.TryGetValue(soundKey, out var value) || value == null)
		{
			Debug.LogWarning("[ModHelper] RegisterAfterhourAmbience: sound '" + soundKey + "' of mod '" + modId + "' is not loaded (register from OnModAssetsLoaded)");
			return false;
		}
		return RegisterAfterhourAmbience(id, value);
	}

	public static bool RegisterAfterhourAmbience(string id, AudioClip clip)
	{
		if (StoreSoundPlayer.Instance == null || clip == null)
		{
			return false;
		}
		StoreSoundPlayer.Instance.RegisterAmbient(id, clip);
		return true;
	}

	public static int GetAfterhourVisits(string locationId)
	{
		if (PlayerStore.Instance == null)
		{
			return 0;
		}
		return PlayerStore.Instance.GetAfterhourState(locationId).visits;
	}

	public static int GetAfterhourCounter(string locationId, string key, int fallback = 0)
	{
		if (PlayerStore.Instance == null)
		{
			return fallback;
		}
		return PlayerStore.Instance.GetAfterhourState(locationId).GetCounter(key, fallback);
	}

	public static void SetAfterhourCounter(string locationId, string key, int value)
	{
		if (PlayerStore.Instance != null)
		{
			PlayerStore.Instance.GetAfterhourState(locationId).SetCounter(key, value);
		}
	}

	public static AfterhourEventDefinition AddAfterhourEvent(string modId, string id, string locationId, Func<string> title, int durationDays, Func<string> description = null, IEnumerable<AfterhourEffect> effects = null, float dailyChancePercent = 0f, AfterhourEventTone? tone = null, Func<bool> canStart = null, Action onStart = null, Action onEnd = null, IEnumerable<AfterhourWeekday> weekdays = null, bool closesLocation = false)
	{
		if (string.IsNullOrEmpty(modId) || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(locationId))
		{
			Debug.LogError("[ModHelper] AddAfterhourEvent needs a mod id, an event id and a location id");
			return null;
		}
		AfterhourEventDefinition afterhourEventDefinition = new AfterhourEventDefinition
		{
			id = id,
			locationId = locationId,
			source = modId,
			title = title,
			description = description,
			toneOverride = tone,
			durationDays = durationDays,
			dailyChancePercent = dailyChancePercent,
			canStart = canStart,
			onStart = onStart,
			onEnd = onEnd
		};
		if (effects != null)
		{
			foreach (AfterhourEffect effect in effects)
			{
				if (effect != null)
				{
					afterhourEventDefinition.effects.Add(effect);
				}
			}
		}
		if (weekdays != null)
		{
			foreach (AfterhourWeekday weekday in weekdays)
			{
				afterhourEventDefinition.Every(weekday);
			}
		}
		afterhourEventDefinition.closesLocation = closesLocation;
		AfterhourEvents.Register(afterhourEventDefinition);
		return afterhourEventDefinition;
	}

	public static AfterhourEffect AfterhourEffect(Func<string> text, string modifierKey = null, float modifierValue = 0f, AfterhourEffectPolarity polarity = AfterhourEffectPolarity.Neutral)
	{
		return new AfterhourEffect(text, modifierKey, modifierValue, polarity);
	}

	public static int RemoveAfterhourEvents(string modId)
	{
		return AfterhourEvents.RemoveSource(modId);
	}

	public static bool StartAfterhourEvent(string eventId, int durationDays = 0)
	{
		return AfterhourEvents.Start(eventId, durationDays);
	}

	public static bool EndAfterhourEvent(string eventId)
	{
		return AfterhourEvents.End(eventId);
	}

	public static void EndAllAfterhourEvents(string locationId)
	{
		AfterhourEvents.EndAll(locationId);
	}

	public static bool IsAfterhourEventActive(string eventId)
	{
		return AfterhourEvents.IsActive(eventId);
	}

	public static List<string> GetActiveAfterhourEventIds(string locationId)
	{
		List<string> list = new List<string>();
		foreach (AfterhourEventDefinition item in AfterhourEvents.GetActive(locationId))
		{
			list.Add(item.id);
		}
		return list;
	}

	public static float GetAfterhourModifier(string locationId, string key)
	{
		return AfterhourLocation.Get(locationId)?.GetModifier(key) ?? 0f;
	}

	public static void AddLootEntry(string modId, string tableId, string itemId, float weight)
	{
		LootRegistry.AddEntry(modId, tableId, itemId, weight);
	}

	public static void RemoveLootEntry(string modId, string tableId, string itemId)
	{
		LootRegistry.RemoveEntry(modId, tableId, itemId);
	}

	public static void SetLootWeight(string modId, string tableId, string itemId, float weight)
	{
		LootRegistry.SetWeight(modId, tableId, itemId, weight);
	}

	public static void ClearLootTable(string modId, string tableId)
	{
		LootRegistry.ClearTable(modId, tableId);
	}

	public static void RegisterLootGroup(string modId, string groupId, params (string tableId, float weight)[] entries)
	{
		LootRegistry.RegisterGroup(modId, groupId, entries);
	}

	public static void AddLootGroupEntry(string modId, string groupId, string tableId, float weight)
	{
		LootRegistry.AddGroupEntry(modId, groupId, tableId, weight);
	}

	public static void RemoveLootGroupEntry(string modId, string groupId, string tableId)
	{
		LootRegistry.RemoveGroupEntry(modId, groupId, tableId);
	}

	public static void SetLootGroupWeight(string modId, string groupId, string tableId, float weight)
	{
		LootRegistry.SetGroupWeight(modId, groupId, tableId, weight);
	}

	public static void ClearLootGroup(string modId, string groupId)
	{
		LootRegistry.ClearGroup(modId, groupId);
	}

	public static List<LootEntry> GetLootEntries(string tableId)
	{
		return LootRegistry.GetTableEntries(tableId);
	}

	public static List<LootEntry> GetLootGroupEntries(string groupId)
	{
		return LootRegistry.GetGroupEntries(groupId);
	}

	public static string RollLootTable(string tableId)
	{
		return LootRegistry.Roll(tableId);
	}

	public static string RollLootGroup(string groupId)
	{
		return LootRegistry.RollGroup(groupId);
	}

	public static GameItem SpawnFromLootTable(string tableId)
	{
		return ItemSpawner.SpawnFromTable(tableId);
	}

	public static GameItem SpawnFromLootGroup(string groupId)
	{
		return ItemSpawner.SpawnFromTableGroup(groupId);
	}

	public static bool RequestSteamAuthTicket(string serverIdentity, Action<byte[]> onTicket)
	{
		return ModSteamAuth.Request(serverIdentity, onTicket);
	}

	public static void CancelSteamAuthTicket()
	{
		ModSteamAuth.Cancel();
	}

	private static string ModDataKey(string modId, string key)
	{
		return modId + ":" + key;
	}

	private static Dictionary<string, string> ModDataStore(bool warn)
	{
		Dictionary<string, string> obj = PlayerStore.Instance?.ModData();
		if (obj == null && warn)
		{
			Debug.LogWarning("[ModHelper] Mod data is only available while a run is loaded");
		}
		return obj;
	}

	public static void SetModData(string modId, string key, string value)
	{
		Dictionary<string, string> dictionary = ModDataStore(warn: true);
		if (dictionary != null)
		{
			dictionary[ModDataKey(modId, key)] = value ?? "";
		}
	}

	public static string GetModData(string modId, string key, string fallback = null)
	{
		Dictionary<string, string> dictionary = ModDataStore(warn: false);
		if (dictionary != null && dictionary.TryGetValue(ModDataKey(modId, key), out var value))
		{
			return value;
		}
		return fallback;
	}

	public static bool HasModData(string modId, string key)
	{
		return ModDataStore(warn: false)?.ContainsKey(ModDataKey(modId, key)) ?? false;
	}

	public static bool RemoveModData(string modId, string key)
	{
		return ModDataStore(warn: true)?.Remove(ModDataKey(modId, key)) ?? false;
	}

	public static List<string> GetModDataKeys(string modId)
	{
		Dictionary<string, string> dictionary = ModDataStore(warn: false);
		if (dictionary == null)
		{
			return new List<string>();
		}
		string prefix = modId + ":";
		return (from k in dictionary.Keys
			where k.StartsWith(prefix)
			select k.Substring(prefix.Length)).ToList();
	}

	public static int ClearModData(string modId)
	{
		Dictionary<string, string> dictionary = ModDataStore(warn: true);
		if (dictionary == null)
		{
			return 0;
		}
		string prefix = modId + ":";
		List<string> list = dictionary.Keys.Where((string k) => k.StartsWith(prefix)).ToList();
		foreach (string item in list)
		{
			dictionary.Remove(item);
		}
		return list.Count;
	}

	public static void SetModDataInt(string modId, string key, int value)
	{
		SetModData(modId, key, value.ToString(CultureInfo.InvariantCulture));
	}

	public static int GetModDataInt(string modId, string key, int fallback = 0)
	{
		string modData = GetModData(modId, key);
		if (modData == null || !int.TryParse(modData, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
		{
			return fallback;
		}
		return result;
	}

	public static void SetModDataFloat(string modId, string key, float value)
	{
		SetModData(modId, key, value.ToString("R", CultureInfo.InvariantCulture));
	}

	public static float GetModDataFloat(string modId, string key, float fallback = 0f)
	{
		string modData = GetModData(modId, key);
		if (modData == null || !float.TryParse(modData, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
		{
			return fallback;
		}
		return result;
	}

	public static void SetModDataBool(string modId, string key, bool value)
	{
		SetModData(modId, key, value ? "true" : "false");
	}

	public static bool GetModDataBool(string modId, string key, bool fallback = false)
	{
		string modData = GetModData(modId, key);
		if (modData == null || !bool.TryParse(modData, out var result))
		{
			return fallback;
		}
		return result;
	}

	public static string GetModFolder(string modId)
	{
		return ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId)?.FolderPath;
	}

	public static string GetModDataFolder(string modId)
	{
		string text = Path.Combine(Application.persistentDataPath, "ModData", SafeFolderName(modId));
		System.IO.Directory.CreateDirectory(text);
		return text;
	}

	public static bool ModFileExists(string modId, string relativePath)
	{
		string text = ResolveInside(GetModFolder(modId), relativePath);
		if (text != null)
		{
			return File.Exists(text);
		}
		return false;
	}

	public static byte[] ReadModFileBytes(string modId, string relativePath)
	{
		string path = RequireInside(GetModFolder(modId), relativePath, modId);
		if (!File.Exists(path))
		{
			return null;
		}
		return File.ReadAllBytes(path);
	}

	public static string ReadModFileText(string modId, string relativePath)
	{
		string path = RequireInside(GetModFolder(modId), relativePath, modId);
		if (!File.Exists(path))
		{
			return null;
		}
		return File.ReadAllText(path);
	}

	public static AssetBundle LoadModAssetBundle(string modId, string relativePath)
	{
		string path = RequireInside(GetModFolder(modId), relativePath, modId);
		if (!File.Exists(path))
		{
			return null;
		}
		return AssetBundle.LoadFromFile(path);
	}

	public static bool ModDataFileExists(string modId, string relativePath)
	{
		string text = ResolveInside(GetModDataFolder(modId), relativePath);
		if (text != null)
		{
			return File.Exists(text);
		}
		return false;
	}

	public static byte[] ReadModDataFileBytes(string modId, string relativePath)
	{
		string path = RequireInside(GetModDataFolder(modId), relativePath, modId);
		if (!File.Exists(path))
		{
			return null;
		}
		return File.ReadAllBytes(path);
	}

	public static string ReadModDataFileText(string modId, string relativePath)
	{
		string path = RequireInside(GetModDataFolder(modId), relativePath, modId);
		if (!File.Exists(path))
		{
			return null;
		}
		return File.ReadAllText(path);
	}

	public static void WriteModDataFileBytes(string modId, string relativePath, byte[] bytes)
	{
		RequireOwnDataFolder(modId);
		string path = RequireInside(GetModDataFolder(modId), relativePath, modId);
		System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
		File.WriteAllBytes(path, bytes ?? new byte[0]);
	}

	public static void WriteModDataFileText(string modId, string relativePath, string text)
	{
		RequireOwnDataFolder(modId);
		string path = RequireInside(GetModDataFolder(modId), relativePath, modId);
		System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
		File.WriteAllText(path, text ?? "");
	}

	public static bool DeleteModDataFile(string modId, string relativePath)
	{
		RequireOwnDataFolder(modId);
		string path = RequireInside(GetModDataFolder(modId), relativePath, modId);
		if (!File.Exists(path))
		{
			return false;
		}
		File.Delete(path);
		return true;
	}

	public static List<string> ListModDataFiles(string modId)
	{
		string modDataFolder = GetModDataFolder(modId);
		string fullRoot = Path.GetFullPath(modDataFolder).TrimEnd('\\', '/') + Path.DirectorySeparatorChar;
		return (from p in System.IO.Directory.GetFiles(modDataFolder, "*", SearchOption.AllDirectories)
			select Path.GetFullPath(p).Substring(fullRoot.Length).Replace('\\', '/')).OrderBy((string p) => p, StringComparer.Ordinal).ToList();
	}

	private static string SafeFolderName(string modId)
	{
		char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
		char[] array = (modId ?? "").ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (Array.IndexOf(invalidFileNameChars, array[i]) >= 0 || array[i] == '.')
			{
				array[i] = '_';
			}
		}
		string text = new string(array).Trim();
		if (text.Length != 0)
		{
			return text;
		}
		return "_";
	}

	private static void RequireOwnDataFolder(string modId)
	{
		string text = ModLoader.CallingModId();
		string folder = SafeFolderName(modId);
		bool flag = ModLoader.Mods.Any((LoadedMod m) => string.Equals(SafeFolderName(m.Manifest.ID), folder, StringComparison.OrdinalIgnoreCase));
		if (text == null)
		{
			if (flag)
			{
				throw new UnauthorizedAccessException("Cannot write or delete in the data folder of '" + modId + "' from code that belongs to no mod: an installed mod's data folder is read-only to others");
			}
		}
		else if ((!(text == modId) || !OwnsDataFolder(modId)) && (!string.Equals(SafeFolderName(modId), SafeFolderName(text), StringComparison.OrdinalIgnoreCase) || !OwnsDataFolder(text)) && flag)
		{
			throw new UnauthorizedAccessException("'" + text + "' cannot write or delete in the data folder of '" + modId + "': another mod's data folder is read-only");
		}
	}

	private static bool OwnsDataFolder(string modId)
	{
		string folder = SafeFolderName(modId);
		List<string> list = (from m in ModLoader.Mods
			select m.Manifest.ID into id
			where string.Equals(SafeFolderName(id), folder, StringComparison.OrdinalIgnoreCase)
			select id).Distinct().ToList();
		if (list.Count <= 1)
		{
			return true;
		}
		List<string> list2 = list.Where((string id) => SafeFolderName(id) == id).ToList();
		if (list2.Count == 1)
		{
			return list2[0] == modId;
		}
		return false;
	}

	private static string RequireInside(string root, string relativePath, string modId)
	{
		if (root == null)
		{
			throw new ArgumentException("Unknown mod '" + modId + "'");
		}
		string text = ResolveInside(root, relativePath);
		if (text == null)
		{
			throw new ArgumentException("'" + relativePath + "' is not a relative path inside the folder of '" + modId + "'");
		}
		return text;
	}

	private static string ResolveInside(string root, string relativePath)
	{
		if (root == null || string.IsNullOrEmpty(relativePath) || Path.IsPathRooted(relativePath))
		{
			return null;
		}
		string text = Path.GetFullPath(root).TrimEnd('\\', '/') + Path.DirectorySeparatorChar;
		string fullPath = Path.GetFullPath(Path.Combine(text, relativePath));
		if (!fullPath.StartsWith(text, StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}
		return fullPath;
	}
}

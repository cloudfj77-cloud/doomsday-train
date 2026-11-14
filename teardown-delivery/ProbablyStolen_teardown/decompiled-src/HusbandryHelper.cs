using System;
using System.Collections.Generic;
using UnityEngine;

public class HusbandryHelper
{
	public const string ANIMAL_ITEM_TAG = "ANIMAL_ITEM_TAG";

	public const string ANIMAL_SEXE_TAG = "ANIMAL_SEXE_TAG";

	public const string MALE = "MALE";

	public const string FEMALE = "FEMALE";

	public const string ANIMAL_AGE_TAG = "ANIMAL_AGE_TAG";

	public const string ANIMAL_WEIGHT_TAG = "ANIMAL_WEIGHT_TAG";

	public const string ANIMAL_HUNGER_TAG = "ANIMAL_HUNGER_TAG";

	public const string ANIMAL_MAX_HUNGER_TAG = "ANIMAL_MAX_HUNGER_TAG";

	public const string ANIMAL_GROWTH_RATE_TAG = "ANIMAL_GROWTH_RATE_TAG";

	public const string ANIMAL_HUNGER_RATE_TAG = "ANIMAL_HUNGER_RATE_TAG";

	public const string ANIMAL_HEALTH_TAG = "ANIMAL_HEALTH_TAG";

	public const string ANIMAL_MAX_HEALTH_TAG = "ANIMAL_MAX_HEALTH_TAG";

	public const string ANIMAL_LONGEVITY_TAG = "ANIMAL_LONGEVITY_TAG";

	public const string ANIMAL_LITTER_SIZE_TAG = "ANIMAL_LITTER_SIZE_TAG";

	public const string ANIMAL_LITTER_SIZE_VARIANCE_TAG = "ANIMAL_LITTER_SIZE_VARIANCE_TAG";

	public const string ANIMAL_FERTILITY_TAG = "ANIMAL_FERTILITY_TAG";

	public const string ANIMAL_CURRENT_FERTILITY_TAG = "ANIMAL_CURRENT_FERTILITY_TAG";

	public const string ANIMAL_GESTATION_PERIOD_TAG = "ANIMAL_GESTATION_PERIOD_TAG";

	public const string ANIMAL_PREGNANT_TAG = "ANIMAL_PREGNANT_TAG";

	public const string ANIMAL_PREGNANT_PROGRESS_TAG = "ANIMAL_PREGNANT_PROGRESS_TAG";

	public const string ANIMAL_IS_DEAD_TAG = "ANIMAL_IS_DEAD_TAG";

	public const string ANIMAL_MAX_MATING_COUNT_TAG = "ANIMAL_MAX_MATING_COUNT_TAG";

	public const string ANIMAL_CURRENT_MATING_COUNT_TAG = "ANIMAL_CURRENT_MATING_COUNT_TAG";

	public const string ANIMAL_IS_STARVING_TAG = "ANIMAL_IS_STARVING_TAG";

	public const string ANIMAL_MISCARRIED_TAG = "ANIMAL_MISCARRIED_TAG";

	public const string ANIMAL_NEUTERED_TAG = "ANIMAL_NEUTERED_TAG";

	public const string ANIMAL_DISEASED_TAG = "ANIMAL_DISEASED_TAG";

	public const string ANIMAL_PRION_TAG = "ANIMAL_PRION_TAG";

	public const string BLADE_ITEM_TAG = "BLADE_ITEM_TAG";

	public const string BUTCHER_TALLY_TAG = "BUTCHER_TALLY_TAG";

	public const string ANIMAL_SPECIES_TAG = "ANIMAL_SPECIES_TAG";

	public const string ANIMAL_RAT = "ANIMAL_RAT";

	public const string MEAT_TYPE_TAG = "MEAT_TYPE_TAG";

	public const string VACUUM_INCINERATOR_UPGRADED_TAG = "VACUUM_INCINERATOR_UPGRADED_TAG";

	public const string VACUUM_INCINERATOR_ON_TAG = "VACUUM_INCINERATOR_ON_TAG";

	public const string ANIMAL_FECES_TAG = "ANIMAL_FECES_TAG";

	public const string ANIMAL_IMMUNITY_TAG = "ANIMAL_IMMUNITY_TAG";

	public const string ANIMAL_OVERCROWDING_TAG = "ANIMAL_OVERCROWDING_TAG";

	public const string ANIMAL_SCRACH_MARK_TAG = "ANIMAL_SCRACH_MARK_TAG";

	public static readonly Dictionary<string, bool> StatHigherIsBetter = new Dictionary<string, bool>
	{
		{ "ANIMAL_GROWTH_RATE_TAG", true },
		{ "ANIMAL_HUNGER_RATE_TAG", false },
		{ "ANIMAL_FERTILITY_TAG", true },
		{ "ANIMAL_LITTER_SIZE_TAG", true },
		{ "ANIMAL_IMMUNITY_TAG", true },
		{ "ANIMAL_MAX_HEALTH_TAG", true },
		{ "ANIMAL_HEALTH_TAG", true },
		{ "ANIMAL_LONGEVITY_TAG", true }
	};

	public static bool CanUpgradeVacuumRobot(GameItem item)
	{
		if (!item.identifier.Equals("vacuum_robot"))
		{
			return false;
		}
		return !item.IsTag("VACUUM_INCINERATOR_UPGRADED_TAG");
	}

	public static void UpgradeVacuumRobot(GameItem robot, GameItem upgradeItem)
	{
		if (robot.identifier.Equals("vacuum_robot") && CanUpgradeVacuumRobot(robot))
		{
			robot.EnableTag("VACUUM_INCINERATOR_UPGRADED_TAG");
			robot.EnableTag("VACUUM_INCINERATOR_ON_TAG");
			robot.unitValue += upgradeItem.unitValue;
			StartingPerkContent.ConsumeUpgradeMaterial(upgradeItem);
			EmporiumAudioPlayer.Instance.ratchet.Play();
		}
	}

	public static bool IsIncineratorOn(GameItem item)
	{
		return item.IsTag("VACUUM_INCINERATOR_ON_TAG");
	}

	public static void CreateVacuumRobotTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("vacuum_robot"))
		{
			if (CanUpgradeVacuumRobot(item))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_vacuum_upgrade_hint"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			else if (IsIncineratorOn(item))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_vacuum_incinerator_on"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_vacuum_incinerator_desc"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			else
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_vacuum_incinerator_off"), ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
		}
	}

	private static RenderHandler.ColorPalette GetStatCompareColor(string tagID, float value, float baseValue)
	{
		bool value2;
		bool flag = !StatHigherIsBetter.TryGetValue(tagID, out value2) || value2;
		if (value > baseValue * 1.05f)
		{
			if (!flag)
			{
				return RenderHandler.ColorPalette.LightRed;
			}
			return RenderHandler.ColorPalette.Green;
		}
		if (value < baseValue * 0.95f)
		{
			if (!flag)
			{
				return RenderHandler.ColorPalette.Green;
			}
			return RenderHandler.ColorPalette.LightRed;
		}
		return RenderHandler.ColorPalette.LightCyan;
	}

	public static void InitAnimalItem(GameItem item, AnimalSpecies species, string forcedSex = null)
	{
		item.EnableTag("ANIMAL_ITEM_TAG");
		item.ModifyTag("ANIMAL_SPECIES_TAG", delegate(TagState state)
		{
			state.Enable().SetString(species.speciesID);
		});
		IngesterHelper.InitIngester(item);
		MachineAutoButcher.InitDropOnButcher(item);
		foreach (KeyValuePair<string, float> baseStat in species.baseStats)
		{
			float value = ((baseStat.Value < 0f) ? ((float)RNG.GetRandomDouble(0.5, 1.5)) : baseStat.Value);
			item.ModifyTag(baseStat.Key, delegate(TagState state)
			{
				state.Enable().SetFloat(value);
			});
		}
		GeneticHelper.InitGenetics(item, species);
		item.ModifyTag("ANIMAL_WEIGHT_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(species.weightStages[0]);
		});
		string sex = forcedSex ?? (RNG.Roll(50) ? "MALE" : "FEMALE");
		item.ModifyTag("ANIMAL_SEXE_TAG", delegate(TagState state)
		{
			state.Enable().SetString(sex);
		});
		float initialFertility = item.GetTagReadonly("ANIMAL_FERTILITY_TAG").GetFloat();
		item.ModifyTag("ANIMAL_CURRENT_FERTILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(initialFertility);
		});
		float maxHealth = GeneticHelper.GetExpressedValue(item, "ANIMAL_MAX_HEALTH_TAG");
		item.ModifyTag("ANIMAL_HEALTH_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(maxHealth);
		});
		item.onCycleEndSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			OnAnimalItemDayEnd(curItem, curInv);
		};
		item.onLoaded = (Action<GameItem>)Delegate.Combine(item.onLoaded, (Action<GameItem>)delegate(GameItem curItem)
		{
			if (curItem.IsTag("ANIMAL_IS_DEAD_TAG"))
			{
				AudioHelper.InitMeaty(curItem);
			}
		});
	}

	public static int GetAnimalOvercrowdingLevel(GameInventory inventory)
	{
		if (!(inventory is GameGridInventory gameGridInventory))
		{
			return 0;
		}
		int num = gameGridInventory.inventoryShape.width * gameGridInventory.inventoryShape.height;
		if (num <= 0)
		{
			return 0;
		}
		int num2 = 0;
		foreach (GameItem childItem in inventory.childItems)
		{
			if (childItem.IsTag("ANIMAL_ITEM_TAG"))
			{
				num2 += childItem.GetItemSize();
			}
		}
		float num3 = (float)num2 / (float)num;
		if (num3 > 0.9f)
		{
			return 3;
		}
		if (num3 > 0.7f)
		{
			return 2;
		}
		if (num3 > 0.55f)
		{
			return 1;
		}
		return 0;
	}

	public static string GetAnimalSprite(GameItem gameItem)
	{
		AnimalSpecies species = GetSpecies(gameItem);
		if (species == null)
		{
			return null;
		}
		float @float = gameItem.GetTagReadonly("ANIMAL_WEIGHT_TAG").GetFloat();
		string text = null;
		foreach (KeyValuePair<int, string> item in species.spriteByWeight)
		{
			if (@float >= (float)item.Key)
			{
				text = item.Value;
				break;
			}
		}
		if (text == null)
		{
			return null;
		}
		string text2 = ((gameItem.GetTagReadonly("ANIMAL_SEXE_TAG").GetString() == "MALE") ? "m" : "f");
		if (!gameItem.IsTag("ANIMAL_IS_DEAD_TAG"))
		{
			return species.spritePrefix + "_" + text + "_" + text2;
		}
		return species.spritePrefix + "_" + text + "_" + text2 + "_d";
	}

	public static bool TryChangeSprite(GameItem gameItem)
	{
		string animalSprite = GetAnimalSprite(gameItem);
		if (animalSprite == null || animalSprite == gameItem.spritePath)
		{
			return false;
		}
		string spritePath = gameItem.spritePath;
		gameItem.SetSpriteAndShape(gameItem.spriteAtlasPath, animalSprite);
		if (gameItem.parentInventory == null)
		{
			return true;
		}
		if (gameItem.parentInventory.CheckInventorySlotStillValid(gameItem, gameItem.modifiedShape, gameItem.modifiedState))
		{
			return true;
		}
		if (gameItem.parentInventory is GameGridInventory gameGridInventory)
		{
			GridShape modifiedShape = gameItem.modifiedShape;
			List<(int, int)> list = new List<(int, int)>();
			for (int i = 0; i < gameGridInventory.inventoryShape.height; i++)
			{
				for (int j = 0; j < gameGridInventory.inventoryShape.width; j++)
				{
					list.Add((j, i));
				}
			}
			for (int num = list.Count - 1; num > 0; num--)
			{
				int num2 = UnityEngine.Random.Range(0, num + 1);
				List<(int, int)> list2 = list;
				int index = num;
				List<(int, int)> list3 = list;
				int index2 = num2;
				(int, int) tuple = list[num2];
				(int, int) tuple2 = list[num];
				(int, int) tuple4 = (list2[index] = tuple);
				tuple4 = (list3[index2] = tuple2);
			}
			foreach (var item3 in list)
			{
				int item = item3.Item1;
				int item2 = item3.Item2;
				GridShape tempShape = new GridShapeBuilder(modifiedShape).SetPosition(item, item2).Build();
				SlotMarker slotMarker = gameGridInventory.TryInventorySlot(gameItem, tempShape);
				if (slotMarker.numTransfer > 0)
				{
					slotMarker.TryAcceptOnce();
					return true;
				}
			}
		}
		gameItem.SetSpriteAndShape(gameItem.spriteAtlasPath, spritePath);
		return false;
	}

	public static void InitRatItem(GameItem item, string forcedSex = null)
	{
		InitAnimalItem(item, AnimalSpecies.Rat, forcedSex);
	}

	public static void OnAnimalItemDayEnd(GameItem item, GameInventory inventory)
	{
		item.unitValue = 0L;
		if (item.IsTag("ANIMAL_IS_DEAD_TAG"))
		{
			return;
		}
		item.ModifyTag("ANIMAL_AGE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(state.GetInt() + 1);
		});
		CheckOldAge(item);
		if (item.IsTag("ANIMAL_IS_DEAD_TAG"))
		{
			return;
		}
		HandleHunger(item);
		MoveAround(item, inventory);
		TryFeed(item, inventory);
		HandleStarvation(item);
		if (item.IsTag("ANIMAL_IS_DEAD_TAG"))
		{
			return;
		}
		CheckHygiene(item, inventory);
		CheckImmunityDisease(item);
		HandleRegen(item);
		HandleDisease(item);
		HandleHealth(item);
		if (!item.IsTag("ANIMAL_IS_DEAD_TAG"))
		{
			HandleOvercrowding(item, inventory);
			UpdateFertility(item, inventory);
			GrowAnimal(item);
			item.ModifyTag("ANIMAL_CURRENT_MATING_COUNT_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
			CheckMate(item, inventory);
			HandlePregnancyProgress(item, inventory);
			IngesterHelper.TickIngester(item);
		}
	}

	public static void HandleOvercrowding(GameItem item, GameInventory inventory)
	{
		int animalOvercrowdingLevel = GetAnimalOvercrowdingLevel(inventory);
		int num = 0;
		if (animalOvercrowdingLevel == 2)
		{
			num = 5;
		}
		else if (animalOvercrowdingLevel >= 3)
		{
			num = 15;
		}
		if (num <= 0 || !RNG.Roll(num))
		{
			return;
		}
		float @float = item.GetTagReadonly("ANIMAL_WEIGHT_TAG").GetFloat();
		GameItem gameItem = null;
		float num2 = float.MaxValue;
		foreach (GameItem childItem in inventory.childItems)
		{
			if (childItem != item && childItem.IsTag("ANIMAL_ITEM_TAG") && !childItem.IsTag("ANIMAL_IS_DEAD_TAG"))
			{
				float float2 = childItem.GetTagReadonly("ANIMAL_WEIGHT_TAG").GetFloat();
				if (float2 < @float && float2 < num2)
				{
					gameItem = childItem;
					num2 = float2;
				}
			}
		}
		if (gameItem != null)
		{
			gameItem.EnableTag("ANIMAL_SCRACH_MARK_TAG");
			OnAnimalDead(gameItem);
		}
	}

	public static void CheckHygiene(GameItem item, GameInventory inventory)
	{
		if (!(inventory is GameGridInventory gameGridInventory))
		{
			return;
		}
		int num = gameGridInventory.inventoryShape.width * gameGridInventory.inventoryShape.height;
		if (num <= 0)
		{
			return;
		}
		int num2 = 0;
		foreach (GameItem childItem in inventory.childItems)
		{
			if (childItem.IsTag("ANIMAL_FECES_TAG"))
			{
				num2 += childItem.GetItemSize();
			}
			if (childItem.IsTag("ANIMAL_IS_DEAD_TAG"))
			{
				num2 += childItem.GetItemSize();
			}
		}
		float num3 = (float)num2 / (float)num;
		float num4 = 0f;
		if (num3 < 0.25f)
		{
			return;
		}
		if (num3 < 0.35f)
		{
			if (RNG.Roll(15))
			{
				num4 = 0.15f;
			}
		}
		else if (RNG.Roll(35))
		{
			num4 = 0.35f;
		}
		int animalOvercrowdingLevel = GetAnimalOvercrowdingLevel(inventory);
		if (animalOvercrowdingLevel > 0)
		{
			num4 += 0.2f * (float)animalOvercrowdingLevel;
		}
		if (num4 > 0f && RNG.Roll((int)(num4 * 100f)))
		{
			item.EnableTag("ANIMAL_DISEASED_TAG");
		}
	}

	public static void HandleRegen(GameItem item)
	{
		AnimalSpecies species = GetSpecies(item);
		if (species != null)
		{
			float currentStat = GeneticHelper.GetCurrentStat(item, "ANIMAL_IMMUNITY_TAG");
			float currentStat2 = GeneticHelper.GetCurrentStat(item, "ANIMAL_MAX_HEALTH_TAG");
			float immunityRegenThreshold = species.immunityRegenThreshold;
			float a = (currentStat - immunityRegenThreshold) / immunityRegenThreshold * 0.3f * currentStat2;
			a = Mathf.Min(a, currentStat2 * 0.3f);
			ModifyHealth(item, a);
		}
	}

	public static void ModifyHealth(GameItem item, float delta)
	{
		float maxHealth = GeneticHelper.GetCurrentStat(item, "ANIMAL_MAX_HEALTH_TAG");
		float health = item.GetTagReadonly("ANIMAL_HEALTH_TAG").GetFloat();
		item.ModifyTag("ANIMAL_HEALTH_TAG", delegate(TagState state)
		{
			state.SetFloat(Mathf.Clamp(health + delta, 0f, maxHealth));
		});
	}

	public static void SetHealthToZero(GameItem item)
	{
		item.ModifyTag("ANIMAL_HEALTH_TAG", delegate(TagState state)
		{
			state.SetFloat(0f);
		});
	}

	public static void HandleHealth(GameItem item)
	{
		if (item.GetTagReadonly("ANIMAL_HEALTH_TAG").GetFloat() <= 0f)
		{
			OnAnimalDead(item);
		}
	}

	public static void HandleDisease(GameItem item)
	{
		if (item.IsTag("ANIMAL_DISEASED_TAG"))
		{
			float currentStat = GeneticHelper.GetCurrentStat(item, "ANIMAL_MAX_HEALTH_TAG");
			ModifyHealth(item, 0f - currentStat * 0.25f);
			if (RNG.Roll((int)GeneticHelper.GetCurrentStat(item, "ANIMAL_IMMUNITY_TAG")))
			{
				item.DisableTag("ANIMAL_DISEASED_TAG");
			}
		}
	}

	public static void OnGetPrionDisease(GameItem item)
	{
		GeneticHelper.ModBonusPercent(item, "ANIMAL_GROWTH_RATE_TAG", -0.2f);
		GeneticHelper.ModBonusPercent(item, "ANIMAL_IMMUNITY_TAG", -0.2f);
		GeneticHelper.ModBonusPercent(item, "ANIMAL_LONGEVITY_TAG", -0.2f);
		item.EnableTag("ANIMAL_PRION_TAG");
	}

	public static void CheckImmunityDisease(GameItem item)
	{
		if (item.IsTag("ANIMAL_DISEASED_TAG"))
		{
			return;
		}
		AnimalSpecies species = GetSpecies(item);
		if (species != null)
		{
			float currentStat = GeneticHelper.GetCurrentStat(item, "ANIMAL_IMMUNITY_TAG");
			if (!(currentStat >= species.immunityThreshold) && RNG.Roll((int)(Mathf.Lerp(0.25f, 0.05f, currentStat / species.immunityThreshold) * 100f)))
			{
				item.EnableTag("ANIMAL_DISEASED_TAG");
			}
		}
	}

	public static void CheckOldAge(GameItem item)
	{
		AnimalSpecies species = GetSpecies(item);
		if (species != null)
		{
			int @int = item.GetTagReadonly("ANIMAL_AGE_TAG").GetInt();
			float currentStat = GeneticHelper.GetCurrentStat(item, "ANIMAL_LONGEVITY_TAG");
			if ((float)@int >= (float)species.growthStages[5] * currentStat && RNG.Roll(75))
			{
				OnAnimalDead(item);
			}
		}
	}

	public static void OnAnimalDead(GameItem item, bool playSound = true)
	{
		if (playSound)
		{
			PlayKillSound(item);
		}
		AudioHelper.InitMeaty(item);
		item.EnableTag("ANIMAL_IS_DEAD_TAG");
		item.DisableTag("ANIMAL_PREGNANT_TAG");
		item.ModifyTag("ANIMAL_PREGNANT_PROGRESS_TAG", delegate(TagState state)
		{
			state.SetFloat(0f);
		});
		item.DisableTag("ANIMAL_IS_STARVING_TAG");
		SetHealthToZero(item);
		TryChangeSprite(item);
	}

	public static void PlayKillSound(GameItem item)
	{
		string text = GetSpecies(item)?.killSoundEffect;
		if (text != null)
		{
			AudioManager.Instance.Play2(text);
		}
	}

	public static bool IsAnimalDead(GameItem item)
	{
		return item.IsTag("ANIMAL_IS_DEAD_TAG");
	}

	public static void HandleHunger(GameItem item)
	{
		GetSpecies(item);
		float currentStat = GeneticHelper.GetCurrentStat(item, "ANIMAL_HUNGER_RATE_TAG");
		float @float = item.GetTagReadonly("ANIMAL_HUNGER_TAG").GetFloat();
		float newHunger = Mathf.Max(0f, @float - currentStat);
		item.ModifyTag("ANIMAL_HUNGER_TAG", delegate(TagState state)
		{
			state.SetFloat(newHunger);
		});
	}

	private static IEnumerable<GameItem> GetReachableDispensers(GameInventory inventory)
	{
		GameItem slottedDispenser = AnimalCage.GetSlottedDispenser(inventory);
		if (slottedDispenser != null)
		{
			yield return slottedDispenser;
		}
		foreach (GameItem childItem in inventory.childItems)
		{
			if (childItem.IsTag("FEED_DISPENSER_TAG"))
			{
				yield return childItem;
			}
		}
	}

	public static void TryFeed(GameItem item, GameInventory inventory)
	{
		if (item.GetTagReadonly("ANIMAL_HUNGER_TAG").GetFloat() >= GetMaxHunger(item) * 0.75f)
		{
			return;
		}
		foreach (GameItem reachableDispenser in GetReachableDispensers(inventory))
		{
			int animalKcal = MachineFeedDispenser.GetAnimalKcal(reachableDispenser);
			if (animalKcal > 0)
			{
				float currentHunger2 = item.GetTagReadonly("ANIMAL_HUNGER_TAG").GetFloat();
				float dispenserTarget = GetMaxHunger(item) * MachineFeedDispenser.GetTargetRatio(reachableDispenser);
				float num = dispenserTarget - currentHunger2;
				if (num <= 0f)
				{
					break;
				}
				string @string = item.GetTagReadonly("ANIMAL_SPECIES_TAG").GetString();
				string storedTag = MachineFeedDispenser.GetStoredTag(@string);
				int @int = reachableDispenser.GetTagReadonly(storedTag).GetInt();
				bool guaranteed = reachableDispenser.IsTag(MachineFeedDispenser.GetPrionStoredTag(@string));
				int drawn = Mathf.Min(animalKcal, Mathf.CeilToInt(num));
				MachineFeedDispenser.DrawKcal(reachableDispenser, drawn);
				if (reachableDispenser.GetTagReadonly(storedTag).GetInt() < @int)
				{
					CheckCannibalism(item, @string, guaranteed);
				}
				item.ModifyTag("ANIMAL_HUNGER_TAG", delegate(TagState state)
				{
					state.SetFloat(Mathf.Min(dispenserTarget, currentHunger2 + (float)drawn));
				});
				Debug.Log(string.Format("Animal fed {0} kcal from dispenser, hunger is now {1}", drawn, item.GetTagReadonly("ANIMAL_HUNGER_TAG").GetFloat()));
				if (item.GetTagReadonly("ANIMAL_HUNGER_TAG").GetFloat() >= GetMaxHunger(item) * 0.75f)
				{
					return;
				}
				break;
			}
		}
		foreach (GameItem childItem in inventory.childItems)
		{
			if (!GutterflowPelletHelper.IsPellet(childItem) && !IndustrialFishFeedHelper.IsFeed(childItem) && childItem.IsTag("CALORIE_VALUE_TAG") && !childItem.IsTag("CANNED_FOOD_TAG"))
			{
				string string2 = childItem.GetTagReadonly("MEAT_TYPE_TAG").GetString();
				string string3 = item.GetTagReadonly("ANIMAL_SPECIES_TAG").GetString();
				if (!string.IsNullOrEmpty(string2) && string2.Equals(string3))
				{
					CheckCannibalism(item, string3, childItem.IsTag("ANIMAL_PRION_TAG"));
				}
				float calories = childItem.GetTagReadonly("CALORIE_VALUE_TAG").GetInt();
				float currentHunger = item.GetTagReadonly("ANIMAL_HUNGER_TAG").GetFloat();
				childItem.Destroy();
				item.ModifyTag("ANIMAL_HUNGER_TAG", delegate(TagState state)
				{
					state.SetFloat(Mathf.Min(GetMaxHunger(item), currentHunger + calories));
				});
				Debug.Log(string.Format("Animal fed with {0} calories, hunger is now {1}", calories, item.GetTagReadonly("ANIMAL_HUNGER_TAG").GetFloat()));
				break;
			}
		}
	}

	public static void CheckCannibalism(GameItem item, string speciesID, bool guaranteed = false)
	{
		if (item.IsTag("ANIMAL_PRION_TAG") || !item.GetTagReadonly("ANIMAL_SPECIES_TAG").GetString().Equals(speciesID))
		{
			return;
		}
		if (guaranteed)
		{
			OnGetPrionDisease(item);
			return;
		}
		float currentStat = GeneticHelper.GetCurrentStat(item, "ANIMAL_IMMUNITY_TAG");
		if (RNG.Roll(Mathf.Clamp(100 - Mathf.RoundToInt(currentStat), 0, 100)))
		{
			OnGetPrionDisease(item);
		}
	}

	public static void HandleStarvation(GameItem item)
	{
		if (item.GetTagReadonly("ANIMAL_HUNGER_TAG").GetFloat() <= 0f)
		{
			if (item.IsTag("ANIMAL_IS_STARVING_TAG"))
			{
				OnAnimalDead(item);
			}
			else
			{
				item.EnableTag("ANIMAL_IS_STARVING_TAG");
			}
		}
		else
		{
			item.DisableTag("ANIMAL_IS_STARVING_TAG");
		}
	}

	public static int GetAnimalCurrentWeightChange(GameItem item)
	{
		AnimalSpecies species = GetSpecies(item);
		if (species == null)
		{
			return 0;
		}
		int[] growthStages = species.growthStages;
		int[] weightStages = species.weightStages;
		int @int = item.GetTagReadonly("ANIMAL_AGE_TAG").GetInt();
		int num = GetStageIndex(@int, growthStages);
		if (num >= growthStages.Length - 1)
		{
			float currentStat = GeneticHelper.GetCurrentStat(item, "ANIMAL_LONGEVITY_TAG");
			if (currentStat <= 1f || (float)@int >= (float)growthStages[^1] * currentStat)
			{
				return 0;
			}
			num = growthStages.Length - 2;
		}
		float num2 = weightStages[num + 1] - weightStages[num];
		float num3 = growthStages[num + 1] - growthStages[num];
		float num4 = num2 / num3;
		float currentStat2 = GeneticHelper.GetCurrentStat(item, "ANIMAL_GROWTH_RATE_TAG");
		num4 *= currentStat2;
		float @float = item.GetTagReadonly("ANIMAL_HUNGER_TAG").GetFloat();
		if (item.IsTag("ANIMAL_IS_STARVING_TAG"))
		{
			num4 = (0f - Mathf.Abs(num4)) * 0.5f;
		}
		else if (@float < 400f)
		{
			float num5 = (400f - @float) / 400f * 0.5f;
			num4 *= 1f - num5;
		}
		return Mathf.RoundToInt(num4);
	}

	public static void GrowAnimal(GameItem item)
	{
		AnimalSpecies species = GetSpecies(item);
		if (species != null)
		{
			int animalCurrentWeightChange = GetAnimalCurrentWeightChange(item);
			int stageIndex = GetStageIndex(item.GetTagReadonly("ANIMAL_AGE_TAG").GetInt(), species.growthStages);
			int num = Mathf.RoundToInt((float)species.weightStages[Mathf.Min(stageIndex, species.weightStages.Length - 1)] * 0.25f);
			float @float = item.GetTagReadonly("ANIMAL_WEIGHT_TAG").GetFloat();
			float newWeight = Mathf.Max(num, @float + (float)animalCurrentWeightChange);
			item.ModifyTag("ANIMAL_WEIGHT_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(newWeight);
			});
			TryChangeSprite(item);
			Poop(item, animalCurrentWeightChange);
		}
	}

	public static void Poop(GameItem gameItem, int growth)
	{
		if (growth >= 30)
		{
			float @float = gameItem.GetTagReadonly("ANIMAL_WEIGHT_TAG").GetFloat();
			string itemID = ((@float >= 1000f) ? "feces_large" : ((!(@float >= 600f)) ? "feces_small" : "feces_medium"));
			GraphUtils.TryAcceptAll(gameItem.parentInventory, ItemSpawner.Spawn(itemID));
		}
	}

	public static void UpdateFertility(GameItem item, GameInventory inventory)
	{
		float currentStat = GeneticHelper.GetCurrentStat(item, "ANIMAL_FERTILITY_TAG");
		float expressedValue = GeneticHelper.GetExpressedValue(item, "ANIMAL_FERTILITY_TAG");
		float num = 0f;
		float @float = item.GetTagReadonly("ANIMAL_HUNGER_TAG").GetFloat();
		if (@float < 200f)
		{
			num += expressedValue * (200f - @float) / 200f * 0.4f;
		}
		AnimalSpecies species = GetSpecies(item);
		int @int = item.GetTagReadonly("ANIMAL_AGE_TAG").GetInt();
		float num2 = ((species != null) ? GeneticHelper.GetCurrentStat(item, "ANIMAL_LONGEVITY_TAG") : 1f);
		if (species != null && (float)@int >= (float)species.growthStages[4] * num2)
		{
			num += expressedValue * 0.15f;
		}
		if (item.IsTag("ANIMAL_DISEASED_TAG"))
		{
			num += expressedValue * 0.75f;
		}
		int animalOvercrowdingLevel = GetAnimalOvercrowdingLevel(inventory);
		if (animalOvercrowdingLevel > 0)
		{
			num += expressedValue * 0.2f * (float)animalOvercrowdingLevel;
		}
		float currentFertility = Mathf.Max(0f, currentStat - num);
		item.ModifyTag("ANIMAL_CURRENT_FERTILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(currentFertility);
		});
	}

	public static bool CanMate(GameItem item)
	{
		if (item.IsTag("ANIMAL_IS_DEAD_TAG"))
		{
			return false;
		}
		if (item.IsTag("ANIMAL_IS_STARVING_TAG"))
		{
			return false;
		}
		if (item.IsTag("ANIMAL_PREGNANT_TAG"))
		{
			return false;
		}
		if (item.IsTag("ANIMAL_NEUTERED_TAG"))
		{
			return false;
		}
		if (!IsMature(item))
		{
			return false;
		}
		if (item.GetTagReadonly("ANIMAL_CURRENT_MATING_COUNT_TAG").GetInt() >= (int)item.GetTagReadonly("ANIMAL_MAX_MATING_COUNT_TAG").GetFloat())
		{
			return false;
		}
		return true;
	}

	public static void CheckMate(GameItem item, GameInventory inventory)
	{
		if (!CanMate(item))
		{
			return;
		}
		item.DisableTag("ANIMAL_MISCARRIED_TAG");
		string @string = item.GetTagReadonly("ANIMAL_SPECIES_TAG").GetString();
		string string2 = item.GetTagReadonly("ANIMAL_SEXE_TAG").GetString();
		int num = (int)item.GetTagReadonly("ANIMAL_MAX_MATING_COUNT_TAG").GetFloat();
		int currentMatingCount = 0;
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem childItem in inventory.childItems)
		{
			if (childItem != item && !(childItem.GetTagReadonly("ANIMAL_SPECIES_TAG").GetString() != @string) && !(childItem.GetTagReadonly("ANIMAL_SEXE_TAG").GetString() == string2) && CanMate(childItem))
			{
				list.Add(childItem);
			}
		}
		for (int num2 = list.Count - 1; num2 > 0; num2--)
		{
			int num3 = UnityEngine.Random.Range(0, num2 + 1);
			List<GameItem> list2 = list;
			int index = num2;
			List<GameItem> list3 = list;
			int index2 = num3;
			GameItem gameItem = list[num3];
			GameItem gameItem2 = list[num2];
			GameItem gameItem4 = (list2[index] = gameItem);
			gameItem4 = (list3[index2] = gameItem2);
		}
		foreach (GameItem item2 in list)
		{
			if (currentMatingCount >= num)
			{
				break;
			}
			if (!item2.IsTag("ANIMAL_PREGNANT_TAG"))
			{
				TryMate(item, item2);
				currentMatingCount++;
				item.ModifyTag("ANIMAL_CURRENT_MATING_COUNT_TAG", delegate(TagState state)
				{
					state.SetInt(currentMatingCount);
				});
				item2.ModifyTag("ANIMAL_CURRENT_MATING_COUNT_TAG", delegate(TagState state)
				{
					state.SetInt(state.GetInt() + 1);
				});
			}
		}
	}

	public static void TryMate(GameItem animal1, GameItem animal2)
	{
		if (RNG.Roll((int)Mathf.Min(animal1.GetTagReadonly("ANIMAL_CURRENT_FERTILITY_TAG").GetFloat(), animal2.GetTagReadonly("ANIMAL_CURRENT_FERTILITY_TAG").GetFloat())))
		{
			bool num = animal1.GetTagReadonly("ANIMAL_SEXE_TAG").GetString().Equals("FEMALE");
			GameItem female = (num ? animal1 : animal2);
			GameItem male = (num ? animal2 : animal1);
			OnPregnant(female, male);
		}
	}

	public static void OnPregnant(GameItem female, GameItem male)
	{
		female.EnableTag("ANIMAL_PREGNANT_TAG");
		female.ModifyTag("ANIMAL_PREGNANT_PROGRESS_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(0f);
		});
		AnimalSpecies species = GetSpecies(female);
		if (species != null)
		{
			GeneticHelper.StorePartnerGenes(female, male, species);
		}
	}

	public static void HandlePregnancyProgress(GameItem item, GameInventory inventory)
	{
		if (!item.IsTag("ANIMAL_PREGNANT_TAG"))
		{
			return;
		}
		if (item.IsTag("ANIMAL_IS_STARVING_TAG") && RNG.Roll(75))
		{
			Miscarry(item);
			return;
		}
		float progress = item.GetTagReadonly("ANIMAL_PREGNANT_PROGRESS_TAG").GetFloat();
		float @float = item.GetTagReadonly("ANIMAL_GESTATION_PERIOD_TAG").GetFloat();
		progress++;
		item.ModifyTag("ANIMAL_PREGNANT_PROGRESS_TAG", delegate(TagState state)
		{
			state.SetFloat(progress);
		});
		if (progress >= @float)
		{
			GiveBirth(item, inventory);
		}
	}

	public static void Miscarry(GameItem item)
	{
		item.DisableTag("ANIMAL_PREGNANT_TAG");
		item.ModifyTag("ANIMAL_PREGNANT_PROGRESS_TAG", delegate(TagState state)
		{
			state.SetFloat(0f);
		});
		item.EnableTag("ANIMAL_MISCARRIED_TAG");
		AnimalSpecies species = GetSpecies(item);
		if (species != null)
		{
			GeneticHelper.ClearPartnerGenes(item, species);
		}
	}

	public static bool CanOperateOnAnimal(GameItem item)
	{
		if (!item.IsTag("ANIMAL_ITEM_TAG") || IsAnimalDead(item))
		{
			return false;
		}
		if (!item.IsTag("ANIMAL_PREGNANT_TAG"))
		{
			return !item.IsTag("ANIMAL_NEUTERED_TAG");
		}
		return true;
	}

	public static void OperateOnAnimal(GameItem item)
	{
		if (CanOperateOnAnimal(item))
		{
			if (item.IsTag("ANIMAL_PREGNANT_TAG"))
			{
				Miscarry(item);
			}
			else
			{
				item.EnableTag("ANIMAL_NEUTERED_TAG");
			}
			string text = GetSpecies(item)?.killSoundEffect;
			if (text != null)
			{
				AudioManager.Instance.Play2(text);
			}
		}
	}

	public static void InitAnimalSurgeryTool(GameItem tool)
	{
		tool.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		tool.canSelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		tool.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(tool.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && t.IsTag("ANIMAL_ITEM_TAG") && !IsAnimalDead(t)));
		tool.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(tool.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && CanOperateOnAnimal(t)));
		tool.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(tool.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t.IsTag("ANIMAL_ITEM_TAG"))
			{
				OperateOnAnimal(t);
			}
		});
	}

	public static void GiveBirth(GameItem mother, GameInventory inventory)
	{
		float currentStat = GeneticHelper.GetCurrentStat(mother, "ANIMAL_LITTER_SIZE_TAG");
		int num = (int)mother.GetTagReadonly("ANIMAL_LITTER_SIZE_VARIANCE_TAG").GetFloat();
		int num2 = Mathf.FloorToInt(currentStat);
		int num3 = Mathf.RoundToInt((currentStat - (float)num2) * 100f);
		if (num3 > 0 && RNG.Roll(num3))
		{
			num2++;
		}
		int num4 = Mathf.Max(1, num2 + UnityEngine.Random.Range(-num, num + 1));
		AnimalSpecies species = GetSpecies(mother);
		for (int i = 0; i < num4; i++)
		{
			GameItem child = ItemSpawner.Spawn(mother.identifier);
			if (species != null)
			{
				GeneticHelper.ApplyInheritedGenetics(child, mother, species);
			}
			TryChangeSprite(child);
			child.ModifyTag("ANIMAL_HUNGER_TAG", delegate(TagState state)
			{
				state.SetFloat(GetMaxHunger(child) * 0.25f);
			});
			GraphUtils.EmporiumTryAdd(inventory, child);
		}
		mother.DisableTag("ANIMAL_PREGNANT_TAG");
		mother.ModifyTag("ANIMAL_PREGNANT_PROGRESS_TAG", delegate(TagState state)
		{
			state.SetFloat(0f);
		});
		if (species != null)
		{
			GeneticHelper.ClearPartnerGenes(mother, species);
		}
	}

	public static AnimalSpecies GetSpecies(GameItem item)
	{
		string @string = item.GetTagReadonly("ANIMAL_SPECIES_TAG").GetString();
		AnimalSpecies.All.TryGetValue(@string, out var value);
		return value;
	}

	public static float GetMaxHunger(GameItem item)
	{
		float @float = item.GetTagReadonly("ANIMAL_MAX_HUNGER_TAG").GetFloat();
		if (!(@float > 0f))
		{
			return 1000f;
		}
		return @float;
	}

	public static int GetStageIndex(int age, int[] stages)
	{
		int result = 0;
		for (int num = stages.Length - 1; num >= 0; num--)
		{
			if (age >= stages[num])
			{
				result = num;
				break;
			}
		}
		return result;
	}

	public static bool IsMature(GameItem item)
	{
		AnimalSpecies species = GetSpecies(item);
		if (species == null)
		{
			return false;
		}
		return item.GetTagReadonly("ANIMAL_AGE_TAG").GetInt() >= species.growthStages[2];
	}

	public static string GetAnimalGrowthStageDisplay(GameItem item)
	{
		AnimalSpecies species = GetSpecies(item);
		if (species == null)
		{
			return LocHelper.GetLocalizedMechanic("husbandry_stage_unknown");
		}
		int @int = item.GetTagReadonly("ANIMAL_AGE_TAG").GetInt();
		int[] growthStages = species.growthStages;
		if (@int >= growthStages[4])
		{
			return LocHelper.GetLocalizedMechanic("husbandry_stage_elder");
		}
		if (@int >= growthStages[3])
		{
			return LocHelper.GetLocalizedMechanic("husbandry_stage_adult");
		}
		if (@int >= growthStages[2])
		{
			return LocHelper.GetLocalizedMechanic("husbandry_stage_adolescent");
		}
		if (@int >= growthStages[1])
		{
			return LocHelper.GetLocalizedMechanic("husbandry_stage_juvenile");
		}
		return LocHelper.GetLocalizedMechanic("husbandry_stage_infant");
	}

	public static string GetSexeDisplay(string sexe)
	{
		if (!sexe.Equals("MALE"))
		{
			return LocHelper.GetLocalizedMechanic("husbandry_sex_female");
		}
		return LocHelper.GetLocalizedMechanic("husbandry_sex_male");
	}

	public static bool TryMoveToRandomSpotInGrid(GameItem item)
	{
		if (!(item.parentInventory is GameGridInventory gameGridInventory))
		{
			return false;
		}
		GridShape modifiedShape = item.modifiedShape;
		List<(int, int)> list = new List<(int, int)>();
		for (int i = 0; i < gameGridInventory.inventoryShape.height; i++)
		{
			for (int j = 0; j < gameGridInventory.inventoryShape.width; j++)
			{
				list.Add((j, i));
			}
		}
		for (int num = list.Count - 1; num > 0; num--)
		{
			int num2 = UnityEngine.Random.Range(0, num + 1);
			List<(int, int)> list2 = list;
			int index = num;
			List<(int, int)> list3 = list;
			int index2 = num2;
			(int, int) tuple = list[num2];
			(int, int) tuple2 = list[num];
			(int, int) tuple4 = (list2[index] = tuple);
			tuple4 = (list3[index2] = tuple2);
		}
		foreach (var item4 in list)
		{
			int item2 = item4.Item1;
			int item3 = item4.Item2;
			GridShape tempShape = new GridShapeBuilder(modifiedShape).SetPosition(item2, item3).Build();
			SlotMarker slotMarker = gameGridInventory.TryInventorySlot(item, tempShape);
			if (slotMarker.numTransfer > 0)
			{
				slotMarker.TryAcceptOnce();
				return true;
			}
		}
		Debug.LogWarning("Failed to move animal to random spot in grid");
		return false;
	}

	private static bool InventoryHasFood(GameInventory inventory)
	{
		foreach (GameItem childItem in inventory.childItems)
		{
			if (childItem.IsTag("CALORIE_VALUE_TAG") && !childItem.IsTag("CANNED_FOOD_TAG"))
			{
				return true;
			}
		}
		return false;
	}

	private static GameInventory FindOtherInventoryWithFood(GameInventory[] mainInventories, GameInventory current)
	{
		List<GameInventory> list = new List<GameInventory>();
		foreach (GameInventory gameInventory in mainInventories)
		{
			if (gameInventory != current)
			{
				list.Add(gameInventory);
			}
		}
		for (int num = list.Count - 1; num > 0; num--)
		{
			int num2 = UnityEngine.Random.Range(0, num + 1);
			List<GameInventory> list2 = list;
			int i = num;
			List<GameInventory> list3 = list;
			int index = num2;
			GameInventory gameInventory2 = list[num2];
			GameInventory gameInventory3 = list[num];
			GameInventory gameInventory5 = (list2[i] = gameInventory2);
			gameInventory5 = (list3[index] = gameInventory3);
		}
		foreach (GameInventory item in list)
		{
			if (InventoryHasFood(item))
			{
				return item;
			}
		}
		return null;
	}

	public static void MoveAround(GameItem gameItem, GameInventory gameInventory)
	{
		GameInventory[] array = new GameInventory[3]
		{
			EmporiumEntry.Instance.invElement,
			EmporiumEntry.Instance.backInvinvElement,
			EmporiumEntry.Instance.showcaseElement
		};
		bool flag = false;
		GameInventory[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			if (array2[i] == gameInventory)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (gameItem.GetTagReadonly("ANIMAL_HUNGER_TAG").GetFloat() < 300f)
			{
				if (!InventoryHasFood(gameInventory))
				{
					GameInventory gameInventory2 = FindOtherInventoryWithFood(array, gameInventory);
					if (gameInventory2 != null)
					{
						GraphUtils.TryAcceptAll(gameInventory2, gameItem);
					}
				}
			}
			else if (RNG.Roll(40))
			{
				TryMoveToRandomSpotInGrid(gameItem);
			}
			else
			{
				if (!RNG.Roll(30))
				{
					return;
				}
				List<GameInventory> list = new List<GameInventory>();
				array2 = array;
				foreach (GameInventory gameInventory3 in array2)
				{
					if (gameInventory3 != gameInventory)
					{
						list.Add(gameInventory3);
					}
				}
				GraphUtils.TryAcceptAll(list[RNG.GetRandomInt(0, list.Count - 1)], gameItem);
			}
		}
		else if (gameInventory.IsTag("CAGE_TAG"))
		{
			GameItem parentItem = gameInventory.GetParentItem();
			if (parentItem != null && RNG.Roll(AnimalCage.GetEscapeChance(parentItem)))
			{
				switch (RNG.GetRandomInt(0, 2))
				{
				case 0:
					GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, gameItem);
					break;
				case 1:
					GraphUtils.TryAcceptAll(EmporiumEntry.Instance.backInvinvElement, gameItem);
					break;
				default:
					GraphUtils.TryAcceptAll(EmporiumEntry.Instance.showcaseElement, gameItem);
					break;
				}
			}
			else
			{
				TryMoveToRandomSpotInGrid(gameItem);
			}
		}
		else if (!gameInventory.IsTag("MOUSE_TRAP_TAG"))
		{
			TryMoveToRandomSpotInGrid(gameItem);
		}
	}

	public static void CreateItemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("ANIMAL_ITEM_TAG"))
		{
			return;
		}
		AnimalSpecies species = GetSpecies(item);
		string @string = item.GetTagReadonly("ANIMAL_SEXE_TAG").GetString();
		int @int = item.GetTagReadonly("ANIMAL_AGE_TAG").GetInt();
		float @float = item.GetTagReadonly("ANIMAL_HUNGER_TAG").GetFloat();
		float float2 = item.GetTagReadonly("ANIMAL_FERTILITY_TAG").GetFloat();
		float float3 = item.GetTagReadonly("ANIMAL_CURRENT_FERTILITY_TAG").GetFloat();
		int num = Mathf.RoundToInt(item.GetTagReadonly("ANIMAL_WEIGHT_TAG").GetFloat());
		float float4 = item.GetTagReadonly("ANIMAL_HEALTH_TAG").GetFloat();
		float currentStat = GeneticHelper.GetCurrentStat(item, "ANIMAL_MAX_HEALTH_TAG");
		string text = ((species != null) ? species.GetDisplayName() : LocHelper.GetLocalizedMechanic("husbandry_stage_unknown"));
		string localizedMechanic = LocHelper.GetLocalizedMechanic("husbandry_tooltip_header", GetSexeDisplay(@string), GetAnimalGrowthStageDisplay(item), text);
		LocGenderHelper.LocGender gender = (@string.Equals("MALE") ? LocGenderHelper.LocGender.Male : LocGenderHelper.LocGender.Female);
		localizedMechanic = LocGenderHelper.Resolve(localizedMechanic, gender);
		builder.AddLine(localizedMechanic, ignoreEmpty: true, RenderHandler.ColorPalette.Cyan);
		builder.AddLine(LocHelper.GetLocalizedMechanic("husbandry_tooltip_age", @int, num), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		builder.AddLine(" ");
		RenderHandler.ColorPalette color = ((float4 < currentStat * 0.25f) ? RenderHandler.ColorPalette.LightRed : ((float4 < currentStat * 0.6f) ? RenderHandler.ColorPalette.Yellow : RenderHandler.ColorPalette.Green));
		builder.Add(LocHelper.GetLocalizedMechanic("husbandry_tooltip_health"), RenderHandler.ColorPalette.LightCyan);
		builder.AddProgressBar(float4 / currentStat, 10, color, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
		builder.AddLine("(" + Mathf.RoundToInt(float4) + "/" + Mathf.RoundToInt(currentStat) + ")", ignoreEmpty: true, color);
		builder.Add(LocHelper.GetLocalizedMechanic("husbandry_tooltip_hunger"), RenderHandler.ColorPalette.LightCyan);
		float maxHunger = GetMaxHunger(item);
		builder.AddProgressBar(@float / maxHunger, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
		builder.AddLine("(" + Mathf.RoundToInt(@float) + "/" + Mathf.RoundToInt(maxHunger) + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		if (IsMature(item))
		{
			if (Mathf.RoundToInt(float3) != Mathf.RoundToInt(float2))
			{
				RenderHandler.ColorPalette color2 = ((float3 < float2) ? RenderHandler.ColorPalette.LightRed : RenderHandler.ColorPalette.Green);
				builder.Add(LocHelper.GetLocalizedMechanic("husbandry_tooltip_fertility"), RenderHandler.ColorPalette.LightCyan);
				builder.AddLine(LocHelper.GetLocalizedMechanic("husbandry_tooltip_fertility_mod", Mathf.RoundToInt(float3), Mathf.RoundToInt(float2)), ignoreEmpty: true, color2);
			}
			else
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("husbandry_tooltip_fertility") + Mathf.RoundToInt(float2) + "%", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
		if (species != null)
		{
			AddStatLine("husbandry_stat_growth_rate", "ANIMAL_GROWTH_RATE_TAG");
			AddStatLine("husbandry_stat_litter_size", "ANIMAL_LITTER_SIZE_TAG");
			AddStatLine("husbandry_stat_immunity", "ANIMAL_IMMUNITY_TAG");
			AddStatLine("husbandry_stat_max_health", "ANIMAL_MAX_HEALTH_TAG");
			AddStatLine("husbandry_stat_longevity", "ANIMAL_LONGEVITY_TAG");
			AddStatLine("husbandry_stat_hunger_rate", "ANIMAL_HUNGER_RATE_TAG");
		}
		if (item.IsTag("ANIMAL_PREGNANT_TAG"))
		{
			float float5 = item.GetTagReadonly("ANIMAL_PREGNANT_PROGRESS_TAG").GetFloat();
			float float6 = item.GetTagReadonly("ANIMAL_GESTATION_PERIOD_TAG").GetFloat();
			builder.Add(LocHelper.GetLocalizedMechanic("husbandry_tooltip_pregnancy"), RenderHandler.ColorPalette.LightCyan);
			builder.AddProgressBar(float5 / float6, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
			builder.AddLine(LocHelper.GetLocalizedMechanic("husbandry_tooltip_pregnancy_days", Mathf.RoundToInt(float5), Mathf.RoundToInt(float6)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		builder.AddLine(" ");
		if (item.IsTag("ANIMAL_IS_DEAD_TAG"))
		{
			builder.Add("[" + Status("husbandry_status_dead") + "] ", RenderHandler.ColorPalette.LightRed);
		}
		if (item.IsTag("ANIMAL_IS_STARVING_TAG"))
		{
			builder.Add("[" + Status("husbandry_status_starving") + "] ", RenderHandler.ColorPalette.Orange);
		}
		if (item.IsTag("ANIMAL_PREGNANT_TAG"))
		{
			builder.Add("[" + Status("husbandry_status_pregnant") + "] ", RenderHandler.ColorPalette.Green);
		}
		if (item.IsTag("ANIMAL_MISCARRIED_TAG"))
		{
			builder.Add("[" + Status("husbandry_status_miscarried") + "] ", RenderHandler.ColorPalette.LightRed);
		}
		if (item.IsTag("ANIMAL_NEUTERED_TAG"))
		{
			builder.Add("[" + Status("husbandry_status_neutered") + "] ", RenderHandler.ColorPalette.Cyan);
		}
		if (item.IsTag("ANIMAL_DISEASED_TAG"))
		{
			builder.Add("[" + Status("husbandry_status_diseased") + "] ", RenderHandler.ColorPalette.LightRed);
		}
		if (item.IsTag("ANIMAL_PRION_TAG"))
		{
			builder.Add("[" + Status("husbandry_status_prion") + "] ", RenderHandler.ColorPalette.LightRed);
		}
		if (item.IsTag("ANIMAL_SCRACH_MARK_TAG"))
		{
			builder.Add("[" + Status("husbandry_status_scratch_mark") + "] ", RenderHandler.ColorPalette.LightRed);
		}
		if (IsMature(item))
		{
			builder.Add("[" + Status("husbandry_status_mature") + "] ", RenderHandler.ColorPalette.Cyan);
		}
		builder.AddLine(" ");
		AnimalSpecies species2 = GetSpecies(item);
		bool flag = false;
		if (species2 != null)
		{
			float currentStat2 = GeneticHelper.GetCurrentStat(item, "ANIMAL_IMMUNITY_TAG");
			if (currentStat2 >= species2.immunityRegenThreshold * 2f)
			{
				builder.Add(Status("husbandry_status_strong_immunity") + " ", RenderHandler.ColorPalette.Green);
				flag = true;
			}
			else if (currentStat2 < species2.immunityRegenThreshold)
			{
				builder.Add(Status("husbandry_status_immunocomp") + " ", RenderHandler.ColorPalette.Orange);
				flag = true;
			}
		}
		int animalOvercrowdingLevel = GetAnimalOvercrowdingLevel(item.parentInventory);
		if (animalOvercrowdingLevel > 0)
		{
			builder.Add(LocGenderHelper.Resolve(LocHelper.GetLocalizedMechanic("husbandry_status_overcrowding", animalOvercrowdingLevel), gender), RenderHandler.ColorPalette.LightRed);
			flag = true;
		}
		if (flag)
		{
			builder.AddLine("");
		}
		if (PlayerStore.Instance.devMode)
		{
			CreateGeneticTooltip(builder, item, species);
		}
		void AddStatLine(string labelKey, string tagID)
		{
			float expressedValue = GeneticHelper.GetExpressedValue(item, tagID);
			float currentStat3 = GeneticHelper.GetCurrentStat(item, tagID);
			bool num2 = Mathf.Abs(currentStat3 - expressedValue) > expressedValue * 0.001f;
			builder.Add(LocHelper.GetLocalizedMechanic(labelKey) + ": ", RenderHandler.ColorPalette.LightCyan);
			if (num2)
			{
				RenderHandler.ColorPalette color3 = RenderHandler.ColorPalette.LightPurple;
				if (StatHigherIsBetter.TryGetValue(tagID, out var value))
				{
					color3 = ((currentStat3 > expressedValue == value) ? RenderHandler.ColorPalette.Green : RenderHandler.ColorPalette.LightRed);
				}
				builder.Add(Mathf.Round(currentStat3 * 100f) / 100f + " (" + Mathf.Round(expressedValue * 100f) / 100f + ")", color3);
			}
			else
			{
				builder.Add((Mathf.Round(expressedValue * 100f) / 100f).ToString(), RenderHandler.ColorPalette.LightCyan);
			}
			if (PlayerStore.Instance.isGeneScannerScanning && species.geneticTraitIDs.Contains(tagID))
			{
				float float7 = item.GetTagReadonly(tagID + "_ALLELE_A").GetFloat();
				float float8 = item.GetTagReadonly(tagID + "_ALLELE_B").GetFloat();
				float baseValue = species.geneticBaseValues[tagID];
				float num3 = Mathf.Round(float7 * 100f) / 100f;
				float num4 = Mathf.Round(float8 * 100f) / 100f;
				RenderHandler.ColorPalette statCompareColor = GetStatCompareColor(tagID, float7, baseValue);
				RenderHandler.ColorPalette statCompareColor2 = GetStatCompareColor(tagID, float8, baseValue);
				builder.Add(" (", RenderHandler.ColorPalette.DarkCyan);
				builder.Add(num3.ToString(), statCompareColor);
				builder.Add(" / ", RenderHandler.ColorPalette.DarkCyan);
				builder.Add(num4.ToString(), statCompareColor2);
				builder.Add(")", RenderHandler.ColorPalette.DarkCyan);
			}
			builder.AddLine("");
		}
		string Status(string key)
		{
			return LocGenderHelper.Resolve(LocHelper.GetLocalizedMechanic(key), gender);
		}
	}

	public static void CreateGeneticTooltip(RichTextBuilder builder, GameItem item, AnimalSpecies species)
	{
		if (species == null)
		{
			return;
		}
		builder.AddLine(" ");
		builder.AddLine("── Genetics ──", ignoreEmpty: true, RenderHandler.ColorPalette.Purple);
		foreach (string geneticTraitID in species.geneticTraitIDs)
		{
			float @float = item.GetTagReadonly(geneticTraitID + "_ALLELE_A").GetFloat();
			float float2 = item.GetTagReadonly(geneticTraitID + "_ALLELE_B").GetFloat();
			float num = (@float + float2) / 2f;
			float num2 = species.geneticBaseValues[geneticTraitID];
			string text = geneticTraitID switch
			{
				"ANIMAL_GROWTH_RATE_TAG" => "Growth Rate", 
				"ANIMAL_FERTILITY_TAG" => "Fertility", 
				"ANIMAL_LITTER_SIZE_TAG" => "Litter Size", 
				"ANIMAL_IMMUNITY_TAG" => "Immunity", 
				_ => geneticTraitID, 
			};
			RenderHandler.ColorPalette statCompareColor = GetStatCompareColor(geneticTraitID, num, num2);
			float num3 = Mathf.Round(@float * 100f) / 100f;
			float num4 = Mathf.Round(float2 * 100f) / 100f;
			float num5 = Mathf.Round(num * 100f) / 100f;
			float num6 = Mathf.Round(num2 * 100f) / 100f;
			builder.Add(text + ": ", RenderHandler.ColorPalette.LightCyan);
			builder.AddLine(num5 + " (base " + num6 + ")", ignoreEmpty: true, statCompareColor);
			builder.Add("  Alleles: ", RenderHandler.ColorPalette.DarkCyan);
			RenderHandler.ColorPalette statCompareColor2 = GetStatCompareColor(geneticTraitID, @float, num2);
			RenderHandler.ColorPalette statCompareColor3 = GetStatCompareColor(geneticTraitID, float2, num2);
			builder.Add("[" + num3 + "]", statCompareColor2);
			builder.AddLine(" [" + num4 + "]", ignoreEmpty: true, statCompareColor3);
			if (Mathf.Max(@float, float2) > num * 1.1f)
			{
				builder.AddLine("  ^ Hidden potential", ignoreEmpty: true, RenderHandler.ColorPalette.Purple);
			}
		}
		if (!item.IsTag("ANIMAL_PREGNANT_TAG"))
		{
			return;
		}
		builder.AddLine(" ");
		builder.AddLine("── Partner Genes ──", ignoreEmpty: true, RenderHandler.ColorPalette.Purple);
		foreach (string geneticTraitID2 in species.geneticTraitIDs)
		{
			float float3 = item.GetTagReadonly(geneticTraitID2 + "_PARTNER_ALLELE_A").GetFloat();
			float float4 = item.GetTagReadonly(geneticTraitID2 + "_PARTNER_ALLELE_B").GetFloat();
			float baseValue = species.geneticBaseValues[geneticTraitID2];
			string text2 = geneticTraitID2 switch
			{
				"ANIMAL_GROWTH_RATE_TAG" => "Growth Rate", 
				"ANIMAL_FERTILITY_TAG" => "Fertility", 
				"ANIMAL_LITTER_SIZE_TAG" => "Litter Size", 
				_ => geneticTraitID2, 
			};
			float num7 = Mathf.Round(float3 * 100f) / 100f;
			float num8 = Mathf.Round(float4 * 100f) / 100f;
			RenderHandler.ColorPalette statCompareColor4 = GetStatCompareColor(geneticTraitID2, float3, baseValue);
			RenderHandler.ColorPalette statCompareColor5 = GetStatCompareColor(geneticTraitID2, float4, baseValue);
			builder.Add(text2 + ": ", RenderHandler.ColorPalette.LightCyan);
			builder.Add("[" + num7 + "]", statCompareColor4);
			builder.AddLine(" [" + num8 + "]", ignoreEmpty: true, statCompareColor5);
		}
	}

	public static void OnButcher(GameItem gameItem, Action<GameItem> place = null)
	{
		if (gameItem is GameItemElement gameItemElement && gameItemElement.IsDestroyed())
		{
			return;
		}
		List<GameItem> list = CreateButcherCuts(gameItem);
		if (list == null)
		{
			return;
		}
		GraphNodeStorage target = gameItem.parentInventory;
		CompleteButcher(gameItem);
		if (place == null)
		{
			place = delegate(GameItem cut)
			{
				GraphUtils.EmporiumTryAdd(target, cut);
			};
		}
		foreach (GameItem item in list)
		{
			place(item);
		}
	}

	public static List<GameItem> CreateButcherCuts(GameItem gameItem)
	{
		string speciesID = gameItem.GetTagReadonly("ANIMAL_SPECIES_TAG").GetString();
		if (!speciesID.Equals("ANIMAL_RAT"))
		{
			return null;
		}
		int num = Mathf.RoundToInt(gameItem.GetTagReadonly("ANIMAL_WEIGHT_TAG").GetFloat());
		if (gameItem.IsTag("ANIMAL_DISEASED_TAG"))
		{
			num = Mathf.RoundToInt((float)num * 0.75f);
		}
		int num2 = num;
		int num3 = num2 / 400;
		int num4 = num2 - num3 * 400;
		int num5 = num4 / 200;
		int num6 = num4 - num5 * 200;
		int num7 = num6 / 100;
		int num8 = num6 - num7 * 100;
		int num9 = num8 / 50;
		if (num8 - num9 * 50 > 0)
		{
			num9++;
		}
		bool flag = gameItem.IsTag("ANIMAL_PRION_TAG");
		List<GameItem> list = new List<GameItem>();
		for (int i = 0; i < num3; i++)
		{
			GameItem gameItem2 = ItemSpawner.Spawn("raw_meat");
			gameItem2.ModifyTag("MEAT_TYPE_TAG", delegate(TagState state)
			{
				state.Enable().SetString(speciesID);
			});
			if (flag)
			{
				gameItem2.EnableTag("ANIMAL_PRION_TAG");
			}
			list.Add(gameItem2);
		}
		for (int j = 0; j < num5; j++)
		{
			GameItem gameItem3 = ItemSpawner.Spawn("small_raw_meat");
			gameItem3.ModifyTag("MEAT_TYPE_TAG", delegate(TagState state)
			{
				state.Enable().SetString(speciesID);
			});
			if (flag)
			{
				gameItem3.EnableTag("ANIMAL_PRION_TAG");
			}
			list.Add(gameItem3);
		}
		for (int k = 0; k < num7; k++)
		{
			GameItem gameItem4 = ItemSpawner.Spawn("morsel");
			gameItem4.ModifyTag("MEAT_TYPE_TAG", delegate(TagState state)
			{
				state.Enable().SetString(speciesID);
			});
			if (flag)
			{
				gameItem4.EnableTag("ANIMAL_PRION_TAG");
			}
			list.Add(gameItem4);
		}
		for (int l = 0; l < num9; l++)
		{
			GameItem gameItem5 = ItemSpawner.Spawn("small_morsel");
			gameItem5.ModifyTag("MEAT_TYPE_TAG", delegate(TagState state)
			{
				state.Enable().SetString(speciesID);
			});
			if (flag)
			{
				gameItem5.EnableTag("ANIMAL_PRION_TAG");
			}
			list.Add(gameItem5);
		}
		return list;
	}

	public static void CompleteButcher(GameItem gameItem)
	{
		PlayerStore.Instance.animalsButcheredTotal++;
		gameItem.Destroy();
	}

	public static void CreateButcherTallyTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("BUTCHER_TALLY_TAG"))
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_husbandry_butcher_tracker", PlayerStore.Instance.animalsButcheredTotal), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		}
	}

	public static void InitBladeItem(GameItem item)
	{
		item.EnableTag("BLADE_ITEM_TAG");
		AudioHelper.InitBlade(item);
		item.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		item.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && t.IsTag("ANIMAL_ITEM_TAG")));
		item.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && t.IsTag("ANIMAL_ITEM_TAG")));
		item.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(item.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t.IsTag("ANIMAL_ITEM_TAG"))
			{
				if (t.IsTag("ANIMAL_IS_DEAD_TAG"))
				{
					OnButcher(t);
				}
				else
				{
					OnAnimalDead(t);
				}
			}
		});
		item.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && t.IsTag("FISH_ITEM_TAG")));
		item.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && t.IsTag("FISH_ITEM_TAG")));
		item.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(item.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t.IsTag("FISH_ITEM_TAG"))
			{
				AquacultureHelper.OnBladeUsed(t);
			}
		});
	}

	public static float SimulateWeightAtAge(GameItem item, AnimalSpecies species, int age)
	{
		float num = GeneticHelper.GetCurrentStat(item, "ANIMAL_GROWTH_RATE_TAG");
		if (num <= 0f)
		{
			num = 1f;
		}
		float b = species.weightStages[0];
		for (int i = 0; i < species.growthStages.Length - 1; i++)
		{
			int num2 = species.growthStages[i];
			int num3 = species.growthStages[i + 1];
			float num4 = species.weightStages[i];
			float num5 = ((float)species.weightStages[i + 1] - num4) / (float)(num3 - num2) * num;
			if (age <= num2)
			{
				break;
			}
			int num6 = Mathf.Min(age, num3) - num2;
			b = num4 + num5 * (float)num6;
			if (age <= num3)
			{
				break;
			}
		}
		int stageIndex = GetStageIndex(age, species.growthStages);
		return Mathf.Max(Mathf.RoundToInt((float)species.weightStages[Mathf.Min(stageIndex, species.weightStages.Length - 1)] * 0.25f), b);
	}
}

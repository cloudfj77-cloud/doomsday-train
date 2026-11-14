using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AquacultureHelper
{
	private sealed class TankOccupancy
	{
		private const byte SOLID = 1;

		public readonly int width;

		public readonly int height;

		public readonly int freeCells;

		private readonly bool[] taken;

		public TankOccupancy(GridShape inventoryShape, List<GameItem> items)
		{
			width = inventoryShape.width;
			height = inventoryShape.height;
			taken = new bool[width * height];
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					taken[i * width + j] = inventoryShape.Get(j, i) != 0;
				}
			}
			List<Vector2Int> cells = new List<Vector2Int>();
			foreach (GameItem item in items)
			{
				GetSolidCells(item.modifiedShape, cells);
				Set(item.modifiedShape.minX, item.modifiedShape.minY, cells, isTaken: true);
			}
			bool[] array = taken;
			for (int k = 0; k < array.Length; k++)
			{
				if (!array[k])
				{
					freeCells++;
				}
			}
		}

		public static void GetSolidCells(GridShape shape, List<Vector2Int> cells)
		{
			cells.Clear();
			for (int i = shape.minY; i <= shape.maxY; i++)
			{
				for (int j = shape.minX; j <= shape.maxX; j++)
				{
					if (shape.Get(j, i) == 1)
					{
						cells.Add(new Vector2Int(j - shape.minX, i - shape.minY));
					}
				}
			}
		}

		public bool Fits(int originX, int originY, List<Vector2Int> cells)
		{
			foreach (Vector2Int cell in cells)
			{
				if (taken[(originY + cell.y) * width + originX + cell.x])
				{
					return false;
				}
			}
			return true;
		}

		public bool FindFit(List<Vector2Int> cells, int maxX, int maxY, int preferX, int preferY, out int x, out int y)
		{
			x = preferX;
			y = preferY;
			if (preferX <= maxX && preferY <= maxY && Fits(preferX, preferY, cells))
			{
				return true;
			}
			int num = maxX + 1;
			int num2 = num * (maxY + 1);
			int randomInt = RNG.GetRandomInt(0, num2 - 1);
			for (int i = 0; i < num2; i++)
			{
				int num3 = (randomInt + i) % num2;
				x = num3 % num;
				y = num3 / num;
				if (Fits(x, y, cells))
				{
					return true;
				}
			}
			return false;
		}

		public void Set(int originX, int originY, List<Vector2Int> cells, bool isTaken)
		{
			foreach (Vector2Int cell in cells)
			{
				int num = originX + cell.x;
				int num2 = originY + cell.y;
				if (num >= 0 && num < width && num2 >= 0 && num2 < height)
				{
					taken[num2 * width + num] = isTaken;
				}
			}
		}
	}

	public const string FISH_ITEM_TAG = "FISH_ITEM_TAG";

	public const string FISH_GROUP_TAG = "FISH_GROUP_TAG";

	public const string FISH_ROE_TAG = "FISH_ROE_TAG";

	public const string FISH_FRY_TAG = "FISH_FRY_TAG";

	public const string FISH_SPECIES_TAG = "FISH_SPECIES_TAG";

	public const string FISH_AGE_TAG = "FISH_AGE_TAG";

	public const string FISH_HUNGER_TAG = "FISH_HUNGER_TAG";

	public const string FISH_HEALTH_TAG = "FISH_HEALTH_TAG";

	public const string FISH_SEX_TAG = "FISH_SEX_TAG";

	public const string FISH_IS_DEAD_TAG = "FISH_IS_DEAD_TAG";

	public const string FISH_BELLY_UP_TAG = "FISH_BELLY_UP_TAG";

	public const string FISH_IS_STARVING_TAG = "FISH_IS_STARVING_TAG";

	public const string FISH_FERTILIZED_TAG = "FISH_FERTILIZED_TAG";

	public const string FISH_GROUP_COUNT_TAG = "FISH_GROUP_COUNT_TAG";

	public const string FISH_KCAL_EATEN_TAG = "FISH_KCAL_EATEN_TAG";

	public const string FISH_KCAL_PELLET_TAG = "FISH_KCAL_PELLET_TAG";

	public const string FISH_KCAL_INDUSTRIAL_TAG = "FISH_KCAL_INDUSTRIAL_TAG";

	public const string FISH_RECENT_KCAL_TAG = "FISH_RECENT_KCAL_TAG";

	public const string FISH_RECENT_INDUSTRIAL_TAG = "FISH_RECENT_INDUSTRIAL_TAG";

	public const string FISH_RECENT_MEAT_TAG = "FISH_RECENT_MEAT_TAG";

	public const string FISH_INDUSTRIAL_DIET_TAG = "FISH_INDUSTRIAL_DIET_TAG";

	public const string FISH_MEAT_DIET_TAG = "FISH_MEAT_DIET_TAG";

	public const string FISH_GROWTH_PROGRESS_TAG = "FISH_GROWTH_PROGRESS_TAG";

	public const string FISH_KCAL_GROWN_TAG = "FISH_KCAL_GROWN_TAG";

	public const string FISH_EGG_GRAMS_TAG = "FISH_EGG_GRAMS_TAG";

	public const string FISH_SURGERY_JARS_TAG = "FISH_SURGERY_JARS_TAG";

	public const string FISH_CARCASS_KCAL_TAG = "FISH_CARCASS_KCAL_TAG";

	public const string FISH_CARCASS_MAX_KCAL_TAG = "FISH_CARCASS_MAX_KCAL_TAG";

	public const float UNPOWERED_HEALTH_LOSS = 0.34f;

	public const float WASTE_DAMAGE_PERCENT = 3f;

	public const float WASTE_DEATH_PERCENT = 8f;

	public const float WATER_DEATH_RATIO = 0.5f;

	public const float WASTE_MAX_DAMAGE_RATIO = 0.5f;

	public const float FEED_THRESHOLD_RATIO = 0.75f;

	public const float DISPENSER_TARGET_RATIO = 0.85f;

	public const float REGEN_RATIO = 0.1f;

	public const float REGEN_HUNGER_RATIO = 0.5f;

	public const float PREDATION_WEIGHT_RATIO = 0.2f;

	public const float ELDER_UPKEEP_MULTIPLIER = 0.5f;

	public const int OLD_AGE_DEATH_CHANCE_PERCENT = 50;

	public const int WANDER_CHANCE_PERCENT = 40;

	public const int WANDER_MAX_TRIES = 8;

	private static readonly Dictionary<FishSpecies, float[]> fullyFedCurves = new Dictionary<FishSpecies, float[]>();

	private static readonly Dictionary<(string atlas, string sprite), GridShape> spriteShapes = new Dictionary<(string, string), GridShape>();

	public const float MIN_DRAIN_KCAL = 5f;

	public const float FRY_YOLK_KCAL = 300f;

	public static readonly string[] REMEDIATED_LIQUIDS = new string[3] { "heavy_metal", "chemical_contaminant", "physical_contaminant" };

	public static FishSpecies GetSpecies(GameItem item)
	{
		string @string = item.GetTagReadonly("FISH_SPECIES_TAG").GetString();
		if (string.IsNullOrEmpty(@string))
		{
			return null;
		}
		FishSpecies.All.TryGetValue(@string, out var value);
		return value;
	}

	public static bool IsFish(GameItem item)
	{
		return item.IsTag("FISH_ITEM_TAG");
	}

	public static bool IsGroup(GameItem item)
	{
		return item.IsTag("FISH_GROUP_TAG");
	}

	public static bool IsDead(GameItem item)
	{
		return item.IsTag("FISH_IS_DEAD_TAG");
	}

	public static bool IsLiveStock(GameItem item)
	{
		if (!IsFish(item) || IsDead(item))
		{
			return IsGroup(item);
		}
		return true;
	}

	public static bool IsCarcass(GameItem item)
	{
		if (IsFish(item))
		{
			return IsDead(item);
		}
		return false;
	}

	public static FishStage GetStage(GameItem item)
	{
		FishSpecies species = GetSpecies(item);
		if (species == null)
		{
			return FishStage.Juvenile;
		}
		if (item.IsTag("FISH_ROE_TAG"))
		{
			return FishStage.Roe;
		}
		if (item.IsTag("FISH_FRY_TAG"))
		{
			return FishStage.Fry;
		}
		return species.GetStage(item.GetTagReadonly("FISH_AGE_TAG").GetInt());
	}

	public static int GetWeightGrams(GameItem item)
	{
		FishSpecies species = GetSpecies(item);
		if (species == null)
		{
			return 0;
		}
		if (IsGroup(item))
		{
			return species.stageWeightGrams[(int)GetStage(item)];
		}
		return GetWeightForBodyKcal(species, GetGrownBodyKcal(item, species));
	}

	private static float[] GetFullyFedCurve(FishSpecies species)
	{
		if (fullyFedCurves.TryGetValue(species, out var value))
		{
			return value;
		}
		int num = species.stageStartDays[2];
		value = new float[species.stageStartDays[5] - num + 1];
		for (int i = 0; i < value.Length; i++)
		{
			value[i] = GetExpectedKcalEatenAtAge(species, num + i);
		}
		fullyFedCurves[species] = value;
		return value;
	}

	public static int GetWeightForBodyKcal(FishSpecies species, float bodyKcal)
	{
		int num = species.stageStartDays[2];
		float[] fullyFedCurve = GetFullyFedCurve(species);
		if (bodyKcal <= fullyFedCurve[0])
		{
			return species.stageWeightGrams[2];
		}
		for (int i = 1; i < fullyFedCurve.Length; i++)
		{
			if (!(bodyKcal > fullyFedCurve[i]))
			{
				float num2 = fullyFedCurve[i] - fullyFedCurve[i - 1];
				float t = ((num2 <= 0f) ? 1f : ((bodyKcal - fullyFedCurve[i - 1]) / num2));
				return Mathf.RoundToInt(Mathf.Lerp(GetWeightAtAge(species, num + i - 1), GetWeightAtAge(species, num + i), t));
			}
		}
		float num3 = bodyKcal - fullyFedCurve[^1];
		return Mathf.RoundToInt((float)species.stageWeightGrams[5] + num3 / species.growthKcalPerGram);
	}

	public static int GetTargetWeightGrams(GameItem item)
	{
		FishSpecies species = GetSpecies(item);
		if (species == null)
		{
			return 0;
		}
		int num = Mathf.Min((int)(GetStage(item) + 1), species.stageWeightGrams.Length - 1);
		return species.stageWeightGrams[num];
	}

	public static int GetWaterNeedMl(GameItem item)
	{
		FishSpecies species = GetSpecies(item);
		if (species == null)
		{
			return 0;
		}
		return species.stageWaterNeedMl[(int)GetStage(item)];
	}

	public static float GetHunger(GameItem item)
	{
		return item.GetTagReadonly("FISH_HUNGER_TAG").GetFloat();
	}

	public static float GetHealth(GameItem item)
	{
		return item.GetTagReadonly("FISH_HEALTH_TAG").GetFloat();
	}

	public static void ModifyHealth(GameItem item, float delta)
	{
		float max = GetSpecies(item)?.maxHealth ?? 100f;
		float health = GetHealth(item);
		item.ModifyTag("FISH_HEALTH_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(Mathf.Clamp(health + delta, 0f, max));
		});
	}

	public static bool IsInTank(GameInventory inventory)
	{
		return inventory?.IsTag("RAS_TANK_GRID_TAG") ?? false;
	}

	private static void InitCommon(GameItem item, FishSpecies species)
	{
		item.ModifyTag("FISH_SPECIES_TAG", delegate(TagState state)
		{
			state.Enable().SetString(species.speciesID);
		});
		item.ModifyTag("FISH_HEALTH_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(species.maxHealth);
		});
		item.ModifyTag("FISH_HUNGER_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(species.maxHunger * 0.5f);
		});
	}

	public static void InitRoeItem(GameItem item, FishSpecies species)
	{
		item.EnableTag("FISH_GROUP_TAG");
		item.EnableTag("FISH_ROE_TAG");
		InitCommon(item, species);
		item.ModifyTag("FISH_AGE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		item.ModifyTag("FISH_GROUP_COUNT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(species.groupCount);
		});
		item.name = species.GetRoeName(fertilized: false);
		item.onLoaded = (Action<GameItem>)Delegate.Combine(item.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			if (loaded.IsTag("FISH_FERTILIZED_TAG"))
			{
				ApplyFertilizedState(loaded);
			}
		});
	}

	public static void SetFertilized(GameItem roe)
	{
		if (!roe.IsTag("FISH_FERTILIZED_TAG"))
		{
			roe.EnableTag("FISH_FERTILIZED_TAG");
			roe.ModifyTag("FISH_AGE_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
			ApplyFertilizedState(roe);
		}
	}

	private static void ApplyFertilizedState(GameItem roe)
	{
		roe.RemoveGameItemType("FOOD");
		roe.RemoveGameItemType("MEDICAL");
		roe.DisableTag("ITEM_MEDICAL_TAG");
		roe.mayThisTargetItemFunc = (GameItem _, GameItem _) => false;
		roe.canThisTargetItemFunc = (GameItem _, GameItem _) => false;
		FishSpecies species = GetSpecies(roe);
		roe.flavorText = species?.GetFlavorText() ?? "";
		if (species != null)
		{
			roe.name = species.GetRoeName(fertilized: true);
		}
		if (species != null && !string.IsNullOrEmpty(species.fertilizedRoeSprite) && roe.spritePath != species.fertilizedRoeSprite)
		{
			roe.SetSpriteAndShape(species.spriteAtlas, species.fertilizedRoeSprite);
		}
	}

	public static void InitFryItem(GameItem item, FishSpecies species)
	{
		item.EnableTag("FISH_GROUP_TAG");
		item.EnableTag("FISH_FRY_TAG");
		InitCommon(item, species);
		item.ModifyTag("FISH_AGE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(species.stageStartDays[1]);
		});
		item.ModifyTag("FISH_GROUP_COUNT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(species.groupCount);
		});
		item.name = species.GetFryName();
		string flavorText = species.GetFlavorText();
		if (!string.IsNullOrEmpty(flavorText))
		{
			item.flavorText = flavorText;
		}
		item.onCycleEndSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			if (!IsInTank(curInv))
			{
				curItem.Destroy();
			}
		};
	}

	public static void InitFishItem(GameItem item, FishSpecies species, string forcedSex = null)
	{
		item.EnableTag("FISH_ITEM_TAG");
		InitCommon(item, species);
		MachineAutoButcher.InitDropOnButcher(item);
		item.ModifyTag("FISH_HUNGER_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(species.maxHunger * 0.85f);
		});
		item.ModifyTag("FISH_AGE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(species.stageStartDays[2]);
		});
		SeedKcalEatenForAge(item, species);
		string sex = forcedSex ?? species.fixedSex ?? (RNG.Roll(50) ? "MALE" : "FEMALE");
		item.ModifyTag("FISH_SEX_TAG", delegate(TagState state)
		{
			state.Enable().SetString(sex);
		});
		UpdateNameAndSprite(item);
		item.onCycleEndSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			if (!IsDead(curItem) && !IsInTank(curInv))
			{
				OnFishDead(curItem);
			}
		};
		item.onLoaded = (Action<GameItem>)Delegate.Combine(item.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			if (IsDead(loaded))
			{
				AudioHelper.InitMeaty(loaded);
			}
			if (float.IsNaN(GetKcalEaten(loaded)))
			{
				SeedKcalEatenForAge(loaded, species);
			}
			UpdateNameAndSprite(loaded);
		});
	}

	public static void SeedKcalEatenForAge(GameItem item, FishSpecies species)
	{
		float expected = GetExpectedKcalEatenAtAge(species, item.GetTagReadonly("FISH_AGE_TAG").GetInt());
		item.ModifyTag("FISH_KCAL_EATEN_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(expected);
		});
		item.ModifyTag("FISH_KCAL_GROWN_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(expected);
		});
		item.ModifyTag("FISH_KCAL_PELLET_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(0f);
		});
	}

	public static void UpdateNameAndSprite(GameItem item)
	{
		FishSpecies species = GetSpecies(item);
		if (species != null && IsFish(item))
		{
			FishStage stage = GetStage(item);
			item.name = species.GetDisplayName();
			string text = species.stageSprites[(int)stage];
			if (!string.IsNullOrEmpty(text) && (text != item.spritePath || species.spriteAtlas != item.spriteAtlasPath))
			{
				TrySetSpriteAndShape(item, species.spriteAtlas, text);
			}
			if (IsDead(item) && !item.IsTag("FISH_BELLY_UP_TAG"))
			{
				TurnBellyUp(item);
			}
		}
	}

	private static bool TrySetSpriteAndShape(GameItem item, string atlas, string sprite)
	{
		string spriteAtlasPath = item.spriteAtlasPath;
		string spritePath = item.spritePath;
		item.SetSpriteAndShape(atlas, sprite);
		if (item.parentInventory == null || item.parentInventory.CheckInventorySlotStillValid(item, null, null))
		{
			return true;
		}
		item.SetSpriteAndShape(spriteAtlasPath, spritePath);
		return false;
	}

	private static void TurnBellyUp(GameItem item)
	{
		item.EnableTag("FISH_BELLY_UP_TAG");
		GridShapeBuilder gridShapeBuilder = new GridShapeBuilder(item.modifiedShape).SetFlipped(!item.modifiedShape.flipped).SetOrientation((item.modifiedShape.orientation + 2) % 4);
		if (item.parentInventory == null || item.parentInventory.CheckInventorySlotStillValid(item, gridShapeBuilder, null))
		{
			item.modifiedShape = gridShapeBuilder;
			item.ValidateShapeState();
		}
	}

	public static float GetKcalEaten(GameItem item)
	{
		return item.GetTagReadonly("FISH_KCAL_EATEN_TAG").GetFloat();
	}

	public static int GetMeatKcal(GameItem item)
	{
		FishSpecies species = GetSpecies(item);
		if (species == null)
		{
			return 0;
		}
		return Mathf.FloorToInt(GetMeatFeedKcal(item, species) * species.conversionEfficiency);
	}

	public static float GetMeatFeedKcal(GameItem item, FishSpecies species)
	{
		return Mathf.Max(0f, GetKcalEaten(item) - GetEggGrams(item) * species.eggKcalPerGram);
	}

	public static float GetGrownBodyKcal(GameItem item, FishSpecies species)
	{
		return Mathf.Max(0f, item.GetTagReadonly("FISH_KCAL_GROWN_TAG").GetFloat() - GetEggGrams(item) * species.eggKcalPerGram);
	}

	public static int GetSkinGraftCount(FishSpecies species, int weightGrams)
	{
		if (species == null || species.skinGraftGrams <= 0 || weightGrams <= 0)
		{
			return 0;
		}
		return weightGrams / species.skinGraftGrams;
	}

	public static float GetEggGrams(GameItem item)
	{
		return item.GetTagReadonly("FISH_EGG_GRAMS_TAG").GetFloat();
	}

	public static int GetSurgeryJars(GameItem item)
	{
		return item.GetTagReadonly("FISH_SURGERY_JARS_TAG").GetInt();
	}

	public static int GetEggTargetGrams(GameItem item)
	{
		FishSpecies species = GetSpecies(item);
		if (species == null || !species.LaysEggs || !IsFish(item))
		{
			return 0;
		}
		FishStage stage = GetStage(item);
		if (stage < FishStage.Adult)
		{
			return 0;
		}
		return Mathf.RoundToInt((float)species.stageWeightGrams[(int)stage] * species.eggLoadBodyRatio);
	}

	public static bool CanGrowEggs(GameItem item, FishSpecies species)
	{
		return GetHealth(item) >= species.maxHealth * species.eggRegrowHealthRatio;
	}

	public static bool IsGravid(GameItem item)
	{
		int eggTargetGrams = GetEggTargetGrams(item);
		if (eggTargetGrams > 0)
		{
			return GetEggGrams(item) >= (float)eggTargetGrams;
		}
		return false;
	}

	public static int GetEggJars(GameItem item)
	{
		return Mathf.FloorToInt(GetEggGrams(item) / 100f);
	}

	private static void RemoveEggGrams(GameItem fish, FishSpecies species, float grams)
	{
		grams = Mathf.Clamp(grams, 0f, GetEggGrams(fish));
		float eggCost = grams * species.eggKcalPerGram;
		fish.ModifyTag("FISH_KCAL_EATEN_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(Mathf.Max(0f, state.GetFloat() - eggCost));
		});
		fish.ModifyTag("FISH_KCAL_GROWN_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(Mathf.Max(0f, state.GetFloat() - eggCost));
		});
		fish.ModifyTag("FISH_EGG_GRAMS_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(Mathf.Max(0f, state.GetFloat() - grams));
		});
	}

	private static void SpawnEggJars(GameItem fish, FishSpecies species, int caviar, int roe)
	{
		GraphNodeStorage parentInventory = fish.parentInventory;
		foreach (GameItem item in CreateEggJars(fish, species, caviar, roe))
		{
			GraphUtils.EmporiumTryAdd(parentInventory, item);
		}
	}

	private static List<GameItem> CreateEggJars(GameItem fish, FishSpecies species, int caviar, int roe)
	{
		bool flag = IsPelletTainted(fish);
		List<GameItem> list = new List<GameItem>();
		for (int i = 0; i < caviar + roe; i++)
		{
			GameItem gameItem = ItemSpawner.Spawn((i < caviar) ? "caviar" : "sturgeon_roe");
			if (gameItem != null)
			{
				gameItem.ModifyTag("MEAT_TYPE_TAG", delegate(TagState state)
				{
					state.Enable().SetString(species.speciesID);
				});
				if (flag)
				{
					GutterflowPelletHelper.ApplyTrashGrade(gameItem);
				}
				list.Add(gameItem);
			}
		}
		return list;
	}

	private static List<GameItem> CreateKillCaviar(GameItem fish, FishSpecies species)
	{
		return CreateEggJars(fish, species, GetEggJars(fish), 0);
	}

	public static (int caviar, int roe) GetSurgeryYield(GameItem fish, FishSpecies species, int jars)
	{
		int surgeryJars = GetSurgeryJars(fish);
		int num = 0;
		for (int i = 1; i <= jars; i++)
		{
			int num2 = surgeryJars + i;
			if (num2 <= species.surgeryCaviarJars)
			{
				num++;
			}
			else if (num2 <= species.surgeryCaviarJars + species.surgeryMixedJars && (num2 - species.surgeryCaviarJars) % 2 == 1)
			{
				num++;
			}
		}
		return (caviar: num, roe: jars - num);
	}

	public static float GetSurgeryHealthCost(GameItem fish, FishSpecies species)
	{
		int eggTargetGrams = GetEggTargetGrams(fish);
		float num = ((eggTargetGrams > 0) ? Mathf.Clamp01(GetEggGrams(fish) / (float)eggTargetGrams) : 1f);
		return species.maxHealth * (species.surgeryHealthCostFull + species.surgeryHealthCostEarly * (1f - num));
	}

	public static bool CanStripEggs(GameItem fish)
	{
		FishSpecies species = GetSpecies(fish);
		if (species == null || !species.LaysEggs || !IsFish(fish) || IsDead(fish))
		{
			return false;
		}
		if (GetEggJars(fish) < 1)
		{
			return false;
		}
		return GetHealth(fish) > GetSurgeryHealthCost(fish, species);
	}

	public static void StripEggs(GameItem fish)
	{
		FishSpecies species = GetSpecies(fish);
		if (species != null && CanStripEggs(fish))
		{
			int jars = GetEggJars(fish);
			float surgeryHealthCost = GetSurgeryHealthCost(fish, species);
			var (caviar, roe) = GetSurgeryYield(fish, species, jars);
			SpawnEggJars(fish, species, caviar, roe);
			RemoveEggGrams(fish, species, jars * 100);
			fish.ModifyTag("FISH_SURGERY_JARS_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(state.GetInt() + jars);
			});
			ModifyHealth(fish, 0f - surgeryHealthCost);
		}
	}

	public static void InitEggSurgeryTool(GameItem tool)
	{
		tool.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		tool.canSelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		tool.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(tool.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && IsFish(t) && (GetSpecies(t)?.LaysEggs ?? false)));
		tool.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(tool.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && CanStripEggs(t)));
		tool.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(tool.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (IsFish(t))
			{
				StripEggs(t);
				AudioManager.Instance.Play3("click_vintage");
			}
		});
	}

	private static int GetWeightAtAge(FishSpecies species, int age)
	{
		int stage = (int)species.GetStage(age);
		int[] stageWeightGrams = species.stageWeightGrams;
		int[] stageStartDays = species.stageStartDays;
		if (stage >= stageWeightGrams.Length - 1)
		{
			return stageWeightGrams[stage];
		}
		int num = stageStartDays[stage + 1] - stageStartDays[stage];
		if (num <= 0)
		{
			return stageWeightGrams[stage];
		}
		float t = Mathf.Clamp01((float)(age - stageStartDays[stage]) / (float)num);
		return Mathf.RoundToInt(Mathf.Lerp(stageWeightGrams[stage], stageWeightGrams[stage + 1], t));
	}

	public static float GetUpkeepKcal(FishSpecies species, int age)
	{
		float num = (float)GetWeightAtAge(species, age) / 100f * species.maintenanceKcalPer100gPerNight;
		if (species.GetStage(age) < FishStage.Elder)
		{
			return num;
		}
		return num * 0.5f;
	}

	public static float GetNightlyDrain(FishSpecies species, int age, float gainedGrams)
	{
		return GetUpkeepKcal(species, age) + GetGrowthKcal(species, gainedGrams);
	}

	private static float GetGrowthKcal(FishSpecies species, float gainedGrams)
	{
		return Mathf.Max(0f, gainedGrams) * species.growthKcalPerGram;
	}

	private static float GetFryGainPerNight(FishSpecies species, int count)
	{
		int num = Mathf.Max(1, species.stageStartDays[2] - species.stageStartDays[1]);
		return (float)(count * species.stageWeightGrams[2]) / (float)num;
	}

	public static float GetExpectedKcalEatenAtAge(FishSpecies species, int age)
	{
		int num = species.stageStartDays[1];
		int num2 = species.stageStartDays[2];
		float num3 = 0f;
		if (species.groupCount > 0)
		{
			num3 = GetGrowthKcal(species, GetFryGainPerNight(species, species.groupCount)) * (float)(num2 - num) / (float)species.groupCount;
		}
		for (int i = num2 + 1; i <= age; i++)
		{
			num3 += GetNightlyDrain(species, i, GetWeightAtAge(species, i) - GetWeightAtAge(species, i - 1));
		}
		return num3;
	}

	public static void OnFishDead(GameItem item)
	{
		if (!IsDead(item))
		{
			int kcal = GetMeatKcal(item);
			item.EnableTag("FISH_IS_DEAD_TAG");
			item.DisableTag("FISH_IS_STARVING_TAG");
			item.ModifyTag("FISH_HEALTH_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(0f);
			});
			item.ModifyTag("FISH_CARCASS_KCAL_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(kcal);
			});
			item.ModifyTag("FISH_CARCASS_MAX_KCAL_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(kcal);
			});
			AudioHelper.InitMeaty(item);
			UpdateNameAndSprite(item);
		}
	}

	public static void OnBladeUsed(GameItem fish)
	{
		if (!IsDead(fish))
		{
			OnFishDead(fish);
		}
		else
		{
			OnFillet(fish);
		}
	}

	public static void OnFillet(GameItem fish, Action<GameItem> place = null)
	{
		if (fish is GameItemElement gameItemElement && gameItemElement.IsDestroyed())
		{
			return;
		}
		List<GameItem> list = CreateFilletPieces(fish);
		if (list == null)
		{
			return;
		}
		GraphNodeStorage target = fish.parentInventory;
		fish.Destroy();
		if (place == null)
		{
			place = delegate(GameItem piece)
			{
				GraphUtils.EmporiumTryAdd(target, piece);
			};
		}
		foreach (GameItem item in list)
		{
			place(item);
		}
	}

	public static List<GameItem> CreateFilletPieces(GameItem fish)
	{
		FishSpecies species = GetSpecies(fish);
		if (species == null)
		{
			return null;
		}
		List<GameItem> list = new List<GameItem>();
		if (species.LaysEggs && GetEggJars(fish) > 0)
		{
			list.AddRange(CreateKillCaviar(fish, species));
		}
		int skinGraftCount = GetSkinGraftCount(species, GetWeightGrams(fish));
		for (int i = 0; i < skinGraftCount; i++)
		{
			GameItem gameItem = ItemSpawner.Spawn("tilapia_skin_graft");
			if (gameItem != null)
			{
				list.Add(gameItem);
			}
		}
		int kcal = (IsDead(fish) ? fish.GetTagReadonly("FISH_CARCASS_KCAL_TAG").GetInt() : GetMeatKcal(fish));
		List<GameItem> list2 = new List<GameItem>();
		foreach (string item in AquacultureItems.BreakDownMeat(species, kcal))
		{
			list2.Add(ItemSpawner.Spawn(item));
		}
		int leftoverScrapKcal = AquacultureItems.GetLeftoverScrapKcal(species, kcal);
		if (leftoverScrapKcal > 0)
		{
			list2.Add(AquacultureItems.SpawnFishScrap(leftoverScrapKcal));
		}
		bool flag = IsPelletTainted(fish);
		foreach (GameItem item2 in list2)
		{
			if (item2 != null)
			{
				item2.ModifyTag("MEAT_TYPE_TAG", delegate(TagState state)
				{
					state.Enable().SetString(species.speciesID);
				});
				if (flag)
				{
					GutterflowPelletHelper.ApplyTrashGrade(item2);
				}
				list.Add(item2);
			}
		}
		return list;
	}

	public static void TickTank(GameItem machine, GameGridInventory tankGrid, GameItem waterTank, List<GameSlotInventory> filterSlots, GameSlotInventory dispenserSlot, bool isPowered)
	{
		if (tankGrid == null)
		{
			return;
		}
		if (!isPowered)
		{
			foreach (GameItem item in tankGrid.childItems.Where(IsLiveStock).ToList())
			{
				FishSpecies species = GetSpecies(item);
				if (species != null)
				{
					ModifyHealth(item, (0f - species.maxHealth) * 0.34f);
				}
			}
		}
		Dictionary<GameItem, float> dictionary = new Dictionary<GameItem, float>();
		foreach (GameItem item2 in tankGrid.childItems.Where(IsLiveStock).ToList())
		{
			AgeOne(item2, tankGrid, dictionary);
		}
		foreach (GameItem item3 in tankGrid.childItems.Where(IsLiveStock).ToList())
		{
			if (!item3.IsTag("FISH_ROE_TAG") && tankGrid.childItems.Contains(item3))
			{
				float value;
				bool flag = dictionary.TryGetValue(item3, out value);
				HandleHunger(item3, flag ? value : 0f, flag);
				TryFeed(item3, tankGrid, dispenserSlot);
				HandleStarvation(item3);
			}
		}
		float num = 0f;
		foreach (GameItem item4 in tankGrid.childItems.Where((GameItem s) => IsFish(s) || IsGroup(s)))
		{
			FishSpecies species2 = GetSpecies(item4);
			if (species2 != null)
			{
				num += (float)GetWeightGrams(item4) / 100f * species2.wasteMlPer100gPerNight;
			}
		}
		if (waterTank != null)
		{
			AddWaste(waterTank, num);
		}
		if (waterTank != null)
		{
			float num2 = 0f;
			foreach (GameItem item5 in tankGrid.childItems.Where((GameItem s) => IsFish(s) && !IsDead(s)))
			{
				FishSpecies species3 = GetSpecies(item5);
				if (species3 != null && !(species3.remediationMlPer100gPerNight <= 0f))
				{
					num2 += (float)GetWeightGrams(item5) / 100f * species3.remediationMlPer100gPerNight;
				}
			}
			if (num2 > 0f)
			{
				RunRemediation(waterTank, num2);
			}
		}
		if (isPowered && waterTank != null)
		{
			foreach (GameSlotInventory filterSlot in filterSlots)
			{
				GameItem gameItem = filterSlot?.childItem;
				if (gameItem != null && gameItem.IsTag("WATER_FILTER"))
				{
					RunFilter(waterTank, gameItem);
				}
			}
		}
		List<GameItem> list = tankGrid.childItems.Where(IsLiveStock).ToList();
		ApplyWaterChecks(list, waterTank, isPowered);
		foreach (GameItem item6 in list)
		{
			if (!(GetHealth(item6) > 0f))
			{
				if (IsGroup(item6))
				{
					item6.Destroy();
				}
				else
				{
					OnFishDead(item6);
				}
			}
		}
		TryBreed(tankGrid);
		WanderStock(tankGrid);
	}

	private static void AgeOne(GameItem stock, GameGridInventory tankGrid, Dictionary<GameItem, float> gainedGrams)
	{
		FishSpecies species = GetSpecies(stock);
		if (species == null)
		{
			return;
		}
		if (stock.IsTag("FISH_ROE_TAG"))
		{
			if (!stock.IsTag("FISH_FERTILIZED_TAG"))
			{
				if (HasFedAdultOfSex(tankGrid, species, "MALE"))
				{
					SetFertilized(stock);
				}
				return;
			}
			int roeAge = stock.GetTagReadonly("FISH_AGE_TAG").GetInt() + 1;
			stock.ModifyTag("FISH_AGE_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(roeAge);
			});
			if (roeAge >= species.stageStartDays[1])
			{
				HatchRoe(stock, species, tankGrid);
			}
			return;
		}
		UpdateRecentDiet(stock);
		if (GetHunger(stock) < species.maxHunger * 0.75f * 0.5f)
		{
			return;
		}
		int num = AdvanceGrowthProgress(stock);
		for (int i = 0; i < num; i++)
		{
			if (!AgeOneDay(stock, species, tankGrid, gainedGrams))
			{
				return;
			}
		}
		if (num > 0)
		{
			UpdateNameAndSprite(stock);
		}
	}

	private static bool AgeOneDay(GameItem stock, FishSpecies species, GameGridInventory tankGrid, Dictionary<GameItem, float> gainedGrams)
	{
		int age = stock.GetTagReadonly("FISH_AGE_TAG").GetInt() + 1;
		stock.ModifyTag("FISH_AGE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(age);
		});
		if (stock.IsTag("FISH_FRY_TAG"))
		{
			if (age >= species.stageStartDays[2])
			{
				GrowFry(stock, species, tankGrid, gainedGrams);
				return false;
			}
			int @int = stock.GetTagReadonly("FISH_GROUP_COUNT_TAG").GetInt();
			AddGainedGrams(gainedGrams, stock, GetFryGainPerNight(species, @int));
			return true;
		}
		if (age >= species.oldAgeDays && RNG.Roll(50))
		{
			OnFishDead(stock);
			return false;
		}
		float num = GetWeightAtAge(species, age) - GetWeightAtAge(species, age - 1);
		if (species.LaysEggs && species.GetStage(age) >= FishStage.Adult && CanGrowEggs(stock, species))
		{
			float eggGain = Mathf.Clamp((float)GetEggTargetGrams(stock) - GetEggGrams(stock), 0f, species.eggGramsPerNight);
			if (eggGain > 0f)
			{
				stock.ModifyTag("FISH_EGG_GRAMS_TAG", delegate(TagState state)
				{
					state.Enable().SetFloat(GetEggGrams(stock) + eggGain);
				});
				num += eggGain * species.eggKcalPerGram / species.growthKcalPerGram;
			}
		}
		AddGainedGrams(gainedGrams, stock, num);
		return true;
	}

	private static void AddGainedGrams(Dictionary<GameItem, float> gainedGrams, GameItem stock, float grams)
	{
		gainedGrams.TryGetValue(stock, out var value);
		gainedGrams[stock] = value + grams;
	}

	private static void WanderStock(GameGridInventory tankGrid)
	{
		List<GameItem> childItems = tankGrid.childItems;
		TankOccupancy tankOccupancy = new TankOccupancy(tankGrid.inventoryShape, childItems);
		if (tankOccupancy.freeCells == 0)
		{
			return;
		}
		List<Vector2Int> cells = new List<Vector2Int>();
		List<Vector2Int> grownCells = new List<Vector2Int>();
		foreach (GameItem item in childItems)
		{
			if (CanWander(item) && !TryGrowIntoRoom(item, tankOccupancy, cells, grownCells) && RNG.Roll(40))
			{
				TryWander(item, tankOccupancy, cells);
			}
		}
	}

	private static bool CanWander(GameItem item)
	{
		if (IsLiveStock(item))
		{
			return !item.IsTag("FISH_ROE_TAG");
		}
		return false;
	}

	private static bool TryGrowIntoRoom(GameItem item, TankOccupancy occupancy, List<Vector2Int> cells, List<Vector2Int> grownCells)
	{
		FishSpecies species = GetSpecies(item);
		string pendingGrowthSprite = GetPendingGrowthSprite(item, species);
		if (pendingGrowthSprite == null)
		{
			return false;
		}
		GridShape modifiedShape = item.modifiedShape;
		GridShape spriteShape = GetSpriteShape(species.spriteAtlas, pendingGrowthSprite, modifiedShape.flipped, modifiedShape.orientation);
		int num = occupancy.width - spriteShape.globalWidth;
		int num2 = occupancy.height - spriteShape.globalHeight;
		if (num < 0 || num2 < 0)
		{
			return false;
		}
		TankOccupancy.GetSolidCells(modifiedShape, cells);
		TankOccupancy.GetSolidCells(spriteShape, grownCells);
		occupancy.Set(modifiedShape.minX, modifiedShape.minY, cells, isTaken: false);
		if (!occupancy.FindFit(grownCells, num, num2, modifiedShape.minX, modifiedShape.minY, out var x, out var y))
		{
			occupancy.Set(modifiedShape.minX, modifiedShape.minY, cells, isTaken: true);
			return false;
		}
		occupancy.Set(x, y, grownCells, isTaken: true);
		item.SetSpriteAndShape(species.spriteAtlas, pendingGrowthSprite);
		item.modifiedShape = new GridShapeBuilder(item.modifiedShape).SetPosition(x, y);
		item.Validate();
		return true;
	}

	private static string GetPendingGrowthSprite(GameItem item, FishSpecies species)
	{
		if (species == null || !IsFish(item) || IsDead(item))
		{
			return null;
		}
		string text = species.stageSprites[(int)GetStage(item)];
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (!(text == item.spritePath) || !(species.spriteAtlas == item.spriteAtlasPath))
		{
			return text;
		}
		return null;
	}

	private static GridShape GetSpriteShape(string atlas, string sprite, bool flipped, int orientation)
	{
		if (!spriteShapes.TryGetValue((atlas, sprite), out var value))
		{
			value = GameItem.BuildShapeFromSprite(atlas, sprite);
			spriteShapes[(atlas, sprite)] = value;
		}
		return new GridShapeBuilder(value).SetTransform(0, 0, flipped, orientation);
	}

	private static void TryWander(GameItem item, TankOccupancy occupancy, List<Vector2Int> cells)
	{
		GridShape modifiedShape = item.modifiedShape;
		int num = occupancy.width - modifiedShape.globalWidth;
		int num2 = occupancy.height - modifiedShape.globalHeight;
		if (num < 0 || num2 < 0)
		{
			return;
		}
		TankOccupancy.GetSolidCells(modifiedShape, cells);
		occupancy.Set(modifiedShape.minX, modifiedShape.minY, cells, isTaken: false);
		for (int i = 0; i < 8; i++)
		{
			int randomInt = RNG.GetRandomInt(0, num);
			int randomInt2 = RNG.GetRandomInt(0, num2);
			if ((randomInt != modifiedShape.minX || randomInt2 != modifiedShape.minY) && occupancy.Fits(randomInt, randomInt2, cells))
			{
				occupancy.Set(randomInt, randomInt2, cells, isTaken: true);
				item.modifiedShape = new GridShapeBuilder(modifiedShape).SetPosition(randomInt, randomInt2);
				item.Validate();
				return;
			}
		}
		occupancy.Set(modifiedShape.minX, modifiedShape.minY, cells, isTaken: true);
	}

	private static void HatchRoe(GameItem roe, FishSpecies species, GameGridInventory tankGrid)
	{
		int count = roe.GetTagReadonly("FISH_GROUP_COUNT_TAG").GetInt();
		roe.Destroy();
		GameItem gameItem = ItemSpawner.Spawn(species.fryIdentifier);
		if (gameItem != null)
		{
			gameItem.ModifyTag("FISH_GROUP_COUNT_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(count);
			});
			gameItem.ModifyTag("FISH_HUNGER_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(Mathf.Min(species.maxHunger, 300f));
			});
			GraphUtils.TryAcceptAllOrDestroy(tankGrid, gameItem, "ras hatch");
		}
	}

	private static void GrowFry(GameItem fry, FishSpecies species, GameGridInventory tankGrid, Dictionary<GameItem, float> gainedGrams)
	{
		int num = Mathf.Max(1, fry.GetTagReadonly("FISH_GROUP_COUNT_TAG").GetInt());
		float hungerEach = GetHunger(fry) / (float)num;
		float grownEach = fry.GetTagReadonly("FISH_KCAL_GROWN_TAG").GetFloat() / (float)num;
		float eatenEach = GetKcalEaten(fry) / (float)num;
		float pelletEach = GetKcalFromPellets(fry) / (float)num;
		float value = GetFryGainPerNight(species, num) / (float)num;
		fry.Destroy();
		for (int i = 0; i < num; i++)
		{
			GameItem gameItem = ItemSpawner.Spawn(species.fishIdentifier);
			if (gameItem != null)
			{
				gameItem.ModifyTag("FISH_AGE_TAG", delegate(TagState state)
				{
					state.Enable().SetInt(species.stageStartDays[2]);
				});
				gameItem.ModifyTag("FISH_HUNGER_TAG", delegate(TagState state)
				{
					state.Enable().SetFloat(hungerEach);
				});
				gameItem.ModifyTag("FISH_KCAL_EATEN_TAG", delegate(TagState state)
				{
					state.Enable().SetFloat(eatenEach);
				});
				gameItem.ModifyTag("FISH_KCAL_GROWN_TAG", delegate(TagState state)
				{
					state.Enable().SetFloat(grownEach);
				});
				gameItem.ModifyTag("FISH_KCAL_PELLET_TAG", delegate(TagState state)
				{
					state.Enable().SetFloat(pelletEach);
				});
				UpdateNameAndSprite(gameItem);
				gainedGrams[gameItem] = value;
				GraphUtils.TryAcceptAllOrDestroy(tankGrid, gameItem, "ras fry growth");
			}
		}
	}

	public static int GetPowerOutput(GameItem stock)
	{
		if (!IsFish(stock) || IsDead(stock) || stock.IsTag("FISH_IS_STARVING_TAG"))
		{
			return 0;
		}
		FishSpecies species = GetSpecies(stock);
		if (species?.stagePowerPerNight == null)
		{
			return 0;
		}
		return species.stagePowerPerNight[(int)GetStage(stock)];
	}

	public static int GetTankPowerOutput(GameInventory tankGrid)
	{
		return ((IEnumerable<GameItem>)tankGrid?.childItems).Sum((Func<GameItem, int>)GetPowerOutput) ?? 0;
	}

	public static void GeneratePower(GameItem ras, GameInventory tankGrid)
	{
		int tankPowerOutput = GetTankPowerOutput(tankGrid);
		if (tankPowerOutput <= 0)
		{
			return;
		}
		MachineHelper.RechargeMachineBattery(ras, tankPowerOutput, ignoreBlackout: true);
		foreach (GameItem item in ras.FindAdjacentType<GameItem>())
		{
			if (item != ras)
			{
				if (item.IsTag("power_source_item"))
				{
					PowerHelper.ChargeBattery(item, tankPowerOutput, ignoreBlackout: true);
				}
				else if (item.IsTag("MACHINE_RECHARGABLE_TAG"))
				{
					MachineHelper.RechargeMachineBattery(item, tankPowerOutput, ignoreBlackout: true);
				}
			}
		}
	}

	private static void HandleHunger(GameItem stock, float gainedGrams, bool aged)
	{
		FishSpecies species = GetSpecies(stock);
		if (species == null)
		{
			return;
		}
		float before = GetHunger(stock);
		float num = Mathf.Max(5f, GetStockDrain(stock, species, stock.GetTagReadonly("FISH_AGE_TAG").GetInt(), gainedGrams));
		float hunger = Mathf.Max(0f, before - num);
		stock.ModifyTag("FISH_HUNGER_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(hunger);
		});
		if (aged)
		{
			stock.ModifyTag("FISH_KCAL_GROWN_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(state.GetFloat() + (before - hunger));
			});
		}
	}

	private static float GetStockDrain(GameItem stock, FishSpecies species, int age, float gainedGrams)
	{
		if (!IsGroup(stock))
		{
			return GetNightlyDrain(species, age, gainedGrams);
		}
		return GetGrowthKcal(species, gainedGrams);
	}

	private static void HandleStarvation(GameItem stock)
	{
		if (GetHunger(stock) > 0f)
		{
			stock.DisableTag("FISH_IS_STARVING_TAG");
		}
		else if (!stock.IsTag("FISH_IS_STARVING_TAG"))
		{
			stock.EnableTag("FISH_IS_STARVING_TAG");
		}
		else if (IsGroup(stock))
		{
			stock.Destroy();
		}
		else
		{
			OnFishDead(stock);
		}
	}

	public static bool CanEat(FishSpecies species, GameItem food)
	{
		if (!food.IsTag("CALORIE_VALUE_TAG"))
		{
			return false;
		}
		bool flag = FoodItemHelper.IsMeatFood(food);
		return species.diet switch
		{
			FishDiet.Herbivore => !flag, 
			FishDiet.Carnivore => flag, 
			_ => true, 
		};
	}

	private static void AddHunger(GameItem stock, FishSpecies species, float kcal, float cap, float pelletShare = 0f, float industrialShare = 0f, float meatShare = 0f)
	{
		float hunger2 = GetHunger(stock);
		float hunger = Mathf.Min(cap, hunger2 + kcal);
		float absorbed = Mathf.Max(0f, hunger - hunger2);
		stock.ModifyTag("FISH_HUNGER_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(hunger);
		});
		if (absorbed <= 0f)
		{
			return;
		}
		stock.ModifyTag("FISH_KCAL_EATEN_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(state.GetFloat() + absorbed);
		});
		float pellet = absorbed * Mathf.Clamp01(pelletShare);
		if (pellet > 0f)
		{
			stock.ModifyTag("FISH_KCAL_PELLET_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(state.GetFloat() + pellet);
			});
		}
		float industrial = absorbed * Mathf.Clamp01(industrialShare);
		if (industrial > 0f)
		{
			stock.ModifyTag("FISH_KCAL_INDUSTRIAL_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(state.GetFloat() + industrial);
			});
		}
		stock.ModifyTag("FISH_RECENT_KCAL_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(state.GetFloat() + absorbed);
		});
		if (industrial > 0f)
		{
			stock.ModifyTag("FISH_RECENT_INDUSTRIAL_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(state.GetFloat() + industrial);
			});
		}
		float meat = absorbed * Mathf.Clamp01(meatShare);
		if (meat > 0f)
		{
			stock.ModifyTag("FISH_RECENT_MEAT_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(state.GetFloat() + meat);
			});
		}
	}

	public static float GetGrowthSpeed(GameItem item)
	{
		float num = Mathf.Clamp01(item.GetTagReadonly("FISH_INDUSTRIAL_DIET_TAG").GetFloat());
		float num2 = Mathf.Clamp01(item.GetTagReadonly("FISH_MEAT_DIET_TAG").GetFloat());
		float num3 = GetSpecies(item)?.meatGrowthBoost ?? 0f;
		return 1f - num * 0.25f + num2 * num3;
	}

	private static void UpdateRecentDiet(GameItem stock)
	{
		float @float = stock.GetTagReadonly("FISH_RECENT_KCAL_TAG").GetFloat();
		if (!(@float <= 0f))
		{
			float industrial = Mathf.Clamp01(stock.GetTagReadonly("FISH_RECENT_INDUSTRIAL_TAG").GetFloat() / @float);
			float meat = Mathf.Clamp01(stock.GetTagReadonly("FISH_RECENT_MEAT_TAG").GetFloat() / @float);
			stock.ModifyTag("FISH_INDUSTRIAL_DIET_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(industrial);
			});
			stock.ModifyTag("FISH_MEAT_DIET_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(meat);
			});
			stock.ModifyTag("FISH_RECENT_KCAL_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(0f);
			});
			stock.ModifyTag("FISH_RECENT_INDUSTRIAL_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(0f);
			});
			stock.ModifyTag("FISH_RECENT_MEAT_TAG", delegate(TagState state)
			{
				state.Enable().SetFloat(0f);
			});
		}
	}

	private static int AdvanceGrowthProgress(GameItem stock)
	{
		float progress = stock.GetTagReadonly("FISH_GROWTH_PROGRESS_TAG").GetFloat() + GetGrowthSpeed(stock);
		int num = Mathf.FloorToInt(progress + 0.001f);
		progress = Mathf.Max(0f, progress - (float)num);
		stock.ModifyTag("FISH_GROWTH_PROGRESS_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(progress);
		});
		return num;
	}

	public static float GetIndustrialFeedShare(GameItem item)
	{
		float kcalEaten = GetKcalEaten(item);
		if (!(kcalEaten <= 0f))
		{
			return Mathf.Clamp01(item.GetTagReadonly("FISH_KCAL_INDUSTRIAL_TAG").GetFloat() / kcalEaten);
		}
		return 0f;
	}

	public static float GetKcalFromPellets(GameItem item)
	{
		return item.GetTagReadonly("FISH_KCAL_PELLET_TAG").GetFloat();
	}

	public static float GetPelletShare(GameItem item)
	{
		float kcalEaten = GetKcalEaten(item);
		if (!(kcalEaten <= 0f))
		{
			return Mathf.Clamp01(GetKcalFromPellets(item) / kcalEaten);
		}
		return 0f;
	}

	public static bool IsPelletTainted(GameItem item)
	{
		FishSpecies species = GetSpecies(item);
		if (species != null && species.pelletProof)
		{
			return false;
		}
		return GetPelletShare(item) > 0.25f;
	}

	private static void TryFeed(GameItem stock, GameGridInventory tankGrid, GameSlotInventory dispenserSlot)
	{
		FishSpecies species = GetSpecies(stock);
		if (species == null)
		{
			return;
		}
		float num = species.maxHunger * 0.75f;
		if (GetHunger(stock) >= num)
		{
			return;
		}
		GameItem gameItem = dispenserSlot?.childItem;
		if (gameItem != null && gameItem.IsTag("FEED_DISPENSER_TAG"))
		{
			bool allowPellet = species.diet != FishDiet.Carnivore;
			bool allowMeat = species.diet != FishDiet.Herbivore;
			bool allowOther = species.diet != FishDiet.Carnivore;
			int availableKcal = MachineFeedDispenser.GetAvailableKcal(gameItem, allowPellet, allowMeat, allowOther);
			float num2 = species.maxHunger * MachineFeedDispenser.GetTargetRatio(gameItem);
			int num3 = Mathf.CeilToInt(num2 - GetHunger(stock));
			if (availableKcal > 0 && num3 > 0)
			{
				int pelletDrawn;
				int industrialDrawn;
				int meatDrawn;
				int num4 = MachineFeedDispenser.DrawKcal(gameItem, Mathf.Min(availableKcal, num3), allowPellet, allowMeat, allowOther, out pelletDrawn, out industrialDrawn, out meatDrawn);
				if (num4 > 0)
				{
					AddHunger(stock, species, num4, num2, (float)pelletDrawn / (float)num4, (float)industrialDrawn / (float)num4, (float)meatDrawn / (float)num4);
				}
				if (GetHunger(stock) >= num)
				{
					return;
				}
			}
		}
		if (species.diet != FishDiet.Herbivore)
		{
			float num5 = species.maxHunger * 0.85f;
			foreach (GameItem item in tankGrid.childItems.Where((GameItem c) => IsCarcass(c) && c.GetTagReadonly("FISH_CARCASS_KCAL_TAG").GetInt() > 0).ToList())
			{
				int num6 = Mathf.CeilToInt(num5 - GetHunger(stock));
				if (num6 <= 0)
				{
					break;
				}
				int left = item.GetTagReadonly("FISH_CARCASS_KCAL_TAG").GetInt();
				int drawn = Mathf.Min(left, num6);
				item.ModifyTag("FISH_CARCASS_KCAL_TAG", delegate(TagState state)
				{
					state.Enable().SetInt(left - drawn);
				});
				AddHunger(stock, species, drawn, num5, 0f, 0f, 1f);
				if (left - drawn <= 0)
				{
					item.Destroy();
				}
			}
			if (GetHunger(stock) >= num)
			{
				return;
			}
		}
		foreach (GameItem item2 in tankGrid.childItems.Where((GameItem f) => f != stock && !IsFish(f) && !IsGroup(f) && !f.IsTag("CANNED_FOOD_TAG") && CanEat(species, f)).ToList())
		{
			if (GutterflowPelletHelper.IsPellet(item2))
			{
				float num7 = species.maxHunger * 0.85f;
				int num8 = Mathf.CeilToInt(num7 - GetHunger(stock));
				if (num8 > 0)
				{
					int num9 = GutterflowPelletHelper.Draw(item2, num8);
					if (num9 > 0)
					{
						AddHunger(stock, species, num9, num7, 1f);
					}
					if (GetHunger(stock) >= num)
					{
						return;
					}
				}
				continue;
			}
			if (IndustrialFishFeedHelper.IsFeed(item2))
			{
				float num10 = species.maxHunger * 0.85f;
				int num11 = Mathf.CeilToInt(num10 - GetHunger(stock));
				if (num11 > 0)
				{
					int num12 = IndustrialFishFeedHelper.Draw(item2, num11);
					if (num12 > 0)
					{
						AddHunger(stock, species, num12, num10, 0f, 1f);
					}
					if (GetHunger(stock) >= num)
					{
						return;
					}
				}
				continue;
			}
			int @int = item2.GetTagReadonly("CALORIE_VALUE_TAG").GetInt();
			float meatShare = (FoodItemHelper.IsMeatFood(item2) ? 1f : 0f);
			item2.Destroy();
			AddHunger(stock, species, @int, species.maxHunger, 0f, 0f, meatShare);
			return;
		}
		TryPredation(stock, species, tankGrid);
	}

	public static bool CanPreyOn(GameItem predator, GameItem prey)
	{
		if (predator == prey || !IsFish(predator) || IsDead(predator))
		{
			return false;
		}
		if (!IsLiveStock(prey))
		{
			return false;
		}
		FishSpecies species = GetSpecies(predator);
		if (species == null || species.diet == FishDiet.Herbivore)
		{
			return false;
		}
		int weightGrams = GetWeightGrams(predator);
		if (weightGrams <= 0)
		{
			return false;
		}
		return (float)GetWeightGrams(prey) <= (float)weightGrams * 0.2f;
	}

	public static int GetPreyKcal(GameItem prey)
	{
		if (prey.IsTag("FISH_ROE_TAG"))
		{
			return prey.GetTagReadonly("CALORIE_VALUE_TAG").GetInt();
		}
		return GetMeatKcal(prey);
	}

	public static int GetPredatorWeightThreshold(GameItem prey)
	{
		return Mathf.CeilToInt((float)GetWeightGrams(prey) / 0.2f);
	}

	private static void TryPredation(GameItem predator, FishSpecies species, GameGridInventory tankGrid)
	{
		if (species.diet != FishDiet.Herbivore && !(GetHunger(predator) >= species.maxHunger * 0.75f))
		{
			GameItem gameItem = tankGrid.childItems.Where((GameItem p) => CanPreyOn(predator, p)).OrderBy(GetWeightGrams).FirstOrDefault();
			if (gameItem != null)
			{
				int preyKcal = GetPreyKcal(gameItem);
				bool flag = gameItem.IsTag("FISH_ROE_TAG");
				gameItem.Destroy();
				AddHunger(predator, species, preyKcal, species.maxHunger, 0f, 0f, flag ? 0f : 1f);
			}
		}
	}

	public static int GetWaterMl(GameItem waterTank)
	{
		if (waterTank != null)
		{
			return WaterHelper.GetTotalVolume(waterTank) / 1000;
		}
		return 0;
	}

	public static float GetWasteDamagePercent(FishSpecies species)
	{
		return 3f / Mathf.Max(0.01f, species.wasteSensitivity);
	}

	public static float GetWasteDeathPercent(FishSpecies species)
	{
		return 8f / Mathf.Max(0.01f, species.wasteSensitivity);
	}

	public static float GetWastePercent(GameItem waterTank)
	{
		if (waterTank == null)
		{
			return 0f;
		}
		int totalVolume = WaterHelper.GetTotalVolume(waterTank);
		if (totalVolume <= 0)
		{
			return 0f;
		}
		return (float)waterTank.GetTagReadonly(Liquid.GetCurrentPartFromId("organic_waste")).GetInt() / (float)totalVolume * 100f;
	}

	public static int GetTotalWaterNeedMl(GameInventory tankGrid)
	{
		return tankGrid?.childItems.Where(IsLiveStock).Sum((Func<GameItem, int>)GetWaterNeedMl) ?? 0;
	}

	private static void AddWaste(GameItem waterTank, float wasteMl)
	{
		int num = Mathf.RoundToInt(wasteMl * 1000f);
		if (num <= 0)
		{
			return;
		}
		string currentPartFromId = Liquid.GetCurrentPartFromId("water");
		string currentPartFromId2 = Liquid.GetCurrentPartFromId("organic_waste");
		int water = waterTank.GetTagReadonly(currentPartFromId).GetInt();
		int converted = Mathf.Min(water, num);
		if (converted > 0)
		{
			int waste = waterTank.GetTagReadonly(currentPartFromId2).GetInt();
			waterTank.ModifyTag(currentPartFromId, delegate(TagState state)
			{
				state.Enable().SetInt(water - converted);
			});
			waterTank.ModifyTag(currentPartFromId2, delegate(TagState state)
			{
				state.Enable().SetInt(waste + converted);
			});
		}
		int num2 = num - converted;
		if (num2 > 0)
		{
			WaterHelper.AddLiquid(waterTank, "organic_waste", num2);
		}
		WaterFeatureHelper.UpdateWaterFeature(waterTank, isExpose: false);
		SpriteHelper.UpdateWaterContainerSprite(waterTank);
	}

	private static void RunRemediation(GameItem waterTank, float ml)
	{
		int num = Mathf.RoundToInt(ml * 1000f);
		if (num <= 0)
		{
			return;
		}
		string currentPartFromId = Liquid.GetCurrentPartFromId("water");
		int removed = 0;
		string[] rEMEDIATED_LIQUIDS = REMEDIATED_LIQUIDS;
		foreach (string text in rEMEDIATED_LIQUIDS)
		{
			Liquid liquidFromId = Liquid.GetLiquidFromId(text);
			if (liquidFromId != null)
			{
				removed += WaterHelper.RemoveContaminantFromContainer(waterTank, text, num, liquidFromId.GetMaxPurityFromMachine(1.0));
			}
		}
		if (removed > 0)
		{
			int water = waterTank.GetTagReadonly(currentPartFromId).GetInt();
			waterTank.ModifyTag(currentPartFromId, delegate(TagState state)
			{
				state.Enable().SetInt(water + removed);
			});
			WaterFeatureHelper.UpdateWaterFeature(waterTank, isExpose: false);
			SpriteHelper.UpdateWaterContainerSprite(waterTank);
		}
	}

	private static void RunFilter(GameItem waterTank, GameItem filter)
	{
		string currentPartFromId = Liquid.GetCurrentPartFromId("water");
		bool flag = false;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			if (liquid.identifier == "water")
			{
				continue;
			}
			string identifier = liquid.identifier;
			string currentPart = liquid.currentPart;
			int @int = waterTank.GetTagReadonly(currentPart).GetInt();
			if (@int <= 0)
			{
				continue;
			}
			int remaining = WaterHelper.HandleFilter(identifier, @int, waterTank, filter);
			int removed = @int - remaining;
			if (removed > 0)
			{
				int water = waterTank.GetTagReadonly(currentPartFromId).GetInt();
				waterTank.ModifyTag(currentPart, delegate(TagState state)
				{
					state.Enable().SetInt(remaining);
				});
				waterTank.ModifyTag(currentPartFromId, delegate(TagState state)
				{
					state.Enable().SetInt(water + removed);
				});
				flag = true;
			}
		}
		if (flag)
		{
			WaterFeatureHelper.UpdateWaterFeature(waterTank, isExpose: false);
			SpriteHelper.UpdateWaterContainerSprite(waterTank);
		}
	}

	private static void ApplyWaterChecks(List<GameItem> live, GameItem waterTank, bool isPowered)
	{
		if (live.Count == 0)
		{
			return;
		}
		int waterMl = GetWaterMl(waterTank);
		int num = ((IEnumerable<GameItem>)live).Sum((Func<GameItem, int>)GetWaterNeedMl);
		float num2 = ((num <= 0) ? 1f : ((float)waterMl / (float)num));
		float wastePercent = GetWastePercent(waterTank);
		foreach (GameItem item in live)
		{
			FishSpecies species = GetSpecies(item);
			if (species == null)
			{
				continue;
			}
			bool flag = !species.ignoresWaste;
			float wasteDamagePercent = GetWasteDamagePercent(species);
			float wasteDeathPercent = GetWasteDeathPercent(species);
			if (num2 < 0.5f || (flag && wastePercent >= wasteDeathPercent))
			{
				item.ModifyTag("FISH_HEALTH_TAG", delegate(TagState state)
				{
					state.Enable().SetFloat(0f);
				});
				continue;
			}
			float num3 = 0f;
			if (num2 < 1f)
			{
				num3 += species.maxHealth * (1f - num2);
			}
			if (flag && wastePercent > wasteDamagePercent)
			{
				num3 += species.maxHealth * 0.5f * (wastePercent - wasteDamagePercent) / (wasteDeathPercent - wasteDamagePercent);
			}
			if (num3 > 0f)
			{
				ModifyHealth(item, 0f - num3);
			}
			else if (isPowered && (item.IsTag("FISH_ROE_TAG") || GetHunger(item) >= species.maxHunger * 0.5f))
			{
				ModifyHealth(item, species.maxHealth * 0.1f);
			}
		}
	}

	private static bool IsFedAdult(GameItem stock, FishSpecies species)
	{
		if (!IsFish(stock) || IsDead(stock))
		{
			return false;
		}
		if (GetSpecies(stock) != species)
		{
			return false;
		}
		if (GetStage(stock) < FishStage.Adult)
		{
			return false;
		}
		return GetHunger(stock) >= species.maxHunger * species.breedHungerRatio;
	}

	private static bool HasFedAdultOfSex(GameGridInventory tankGrid, FishSpecies species, string sex)
	{
		return tankGrid.childItems.Any((GameItem s) => IsFedAdult(s, species) && s.GetTagReadonly("FISH_SEX_TAG").GetString() == sex);
	}

	private static void TryBreed(GameGridInventory tankGrid)
	{
		foreach (FishSpecies species in FishSpecies.All.Values)
		{
			if (!species.CanBreed)
			{
				continue;
			}
			foreach (GameItem item in tankGrid.childItems.Where((GameItem s) => IsFedAdult(s, species) && s.GetTagReadonly("FISH_SEX_TAG").GetString() == "FEMALE").ToList())
			{
				_ = item;
				if (RNG.Roll(species.breedChancePercent))
				{
					GameItem gameItem = ItemSpawner.Spawn(species.roeIdentifier);
					if (gameItem != null)
					{
						GraphUtils.TryAcceptAllOrDestroy(tankGrid, gameItem, "ras breeding");
					}
				}
			}
		}
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!IsFish(item) && !IsGroup(item))
		{
			return;
		}
		FishSpecies species = GetSpecies(item);
		if (species != null)
		{
			AddStatusLine(builder, item, species);
			if (item.IsTag("FISH_ROE_TAG"))
			{
				CreateRoeTooltip(builder, item, species);
			}
			else if (item.IsTag("FISH_FRY_TAG"))
			{
				CreateFryTooltip(builder, item, species);
			}
			else if (IsDead(item))
			{
				CreateCarcassTooltip(builder, item, species);
			}
			else
			{
				CreateFishTooltip(builder, item, species);
			}
			if (PlayerStore.Instance.devMode)
			{
				CreateDevTooltip(builder, item, species);
			}
		}
	}

	private static void CreateRoeTooltip(RichTextBuilder builder, GameItem item, FishSpecies species)
	{
		int @int = item.GetTagReadonly("FISH_GROUP_COUNT_TAG").GetInt();
		string text = (item.IsTag("FISH_FERTILIZED_TAG") ? LocHelper.GetLocalizedMechanic("aquaculture_roe_eggs_hatch", @int, Mathf.Max(0, species.stageStartDays[1] - item.GetTagReadonly("FISH_AGE_TAG").GetInt())) : LocHelper.GetLocalizedMechanic("aquaculture_roe_eggs", @int));
		builder.AddLine(text, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		AddHealthLine(builder, item, species);
		builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_water_need_clutch", GetWaterNeedMl(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
	}

	private static void CreateFryTooltip(RichTextBuilder builder, GameItem item, FishSpecies species)
	{
		int @int = item.GetTagReadonly("FISH_GROUP_COUNT_TAG").GetInt();
		int num = Mathf.Max(0, species.stageStartDays[2] - item.GetTagReadonly("FISH_AGE_TAG").GetInt());
		builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_fry_count", @int, num), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		AddHealthLine(builder, item, species);
		AddHungerLine(builder, item, species);
		AddPelletLine(builder, item);
		AddGrowthSpeedLine(builder, item);
		builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_water_need_clutch", GetWaterNeedMl(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
	}

	private static void CreateCarcassTooltip(RichTextBuilder builder, GameItem item, FishSpecies species)
	{
		int @int = item.GetTagReadonly("FISH_CARCASS_KCAL_TAG").GetInt();
		int int2 = item.GetTagReadonly("FISH_CARCASS_MAX_KCAL_TAG").GetInt();
		AddHeaderLine(builder, item, species);
		builder.AddLine(LocHelper.GetLocalizedMechanic("husbandry_tooltip_age", item.GetTagReadonly("FISH_AGE_TAG").GetInt(), GetWeightGrams(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_carcass_meat", @int, int2), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
	}

	private static string DietKey(FishDiet diet)
	{
		return diet switch
		{
			FishDiet.Herbivore => "aquaculture_diet_herbivore", 
			FishDiet.Carnivore => "aquaculture_diet_carnivore", 
			_ => "aquaculture_diet_omnivore", 
		};
	}

	private static void CreateFishTooltip(RichTextBuilder builder, GameItem item, FishSpecies species)
	{
		FishStage stage = GetStage(item);
		int @int = item.GetTagReadonly("FISH_AGE_TAG").GetInt();
		int weightGrams = GetWeightGrams(item);
		AddHeaderLine(builder, item, species);
		string localizedMechanic = LocHelper.GetLocalizedMechanic("husbandry_tooltip_age", @int, weightGrams);
		string localizedMechanic2 = LocHelper.GetLocalizedMechanic("mech_feed_dispenser_meat", GetMeatKcal(item));
		builder.AddLine(localizedMechanic + "  " + localizedMechanic2, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		string text = LocHelper.GetLocalizedMechanic(DietKey(species.diet));
		if (species.diet == FishDiet.Carnivore)
		{
			text = RichTextBuilder.Color(RichTextBuilder.Style(text, bold: true), RenderHandler.ColorPalette.Red);
		}
		builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_diet", text), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		if (species.ignoresWaste)
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_trait_ignores_waste"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		else if (species.wasteSensitivity > 1f)
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_trait_sensitive_waste"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		if (species.remediationMlPer100gPerNight > 0f)
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_trait_absorbs_contaminants"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		if (species.pelletProof)
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_trait_pellet_proof"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		if (species.meatGrowthBoost > 0f)
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_trait_meat_growth", Mathf.RoundToInt(species.meatGrowthBoost * 100f)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		AddHealthLine(builder, item, species);
		AddHungerLine(builder, item, species);
		AddPelletLine(builder, item);
		AddGrowthSpeedLine(builder, item);
		builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_water_need", GetWaterNeedMl(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		if (species.stagePowerPerNight != null)
		{
			int powerOutput = GetPowerOutput(item);
			if (powerOutput > 0)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_power_output", powerOutput), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
			}
			else if (item.IsTag("FISH_IS_STARVING_TAG"))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_power_starving"), ignoreEmpty: true, RenderHandler.ColorPalette.Orange);
			}
			else
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_power_from_adolescent", species.stageStartDays[3]), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
		if (stage < FishStage.Elder)
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_next_stage", Mathf.Max(0, species.stageStartDays[(int)(stage + 1)] - @int)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		else if (@int >= species.oldAgeDays)
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_old_age"), ignoreEmpty: true, RenderHandler.ColorPalette.Orange);
		}
		else
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_old_age_in", species.oldAgeDays - @int), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		if (species.LaysEggs && stage >= FishStage.Adult)
		{
			int num = Mathf.RoundToInt(GetEggGrams(item));
			int eggTargetGrams = GetEggTargetGrams(item);
			if (IsGravid(item))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_eggs_full", num, eggTargetGrams), ignoreEmpty: true, RenderHandler.ColorPalette.Green);
			}
			else if (!CanGrowEggs(item, species))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_eggs_paused_healing", num, eggTargetGrams, Mathf.RoundToInt(species.eggRegrowHealthRatio * 100f)), ignoreEmpty: true, RenderHandler.ColorPalette.Orange);
			}
			else
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_eggs", num, eggTargetGrams), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			int eggJars = GetEggJars(item);
			if (PlayerStore.Instance.devMode && eggJars > 0)
			{
				(int caviar, int roe) surgeryYield = GetSurgeryYield(item, species, eggJars);
				int item2 = surgeryYield.caviar;
				int item3 = surgeryYield.roe;
				int num2 = Mathf.RoundToInt(GetSurgeryHealthCost(item, species) / species.maxHealth * 100f);
				string text2 = ((item2 > 0 && item3 > 0) ? (item2 + " caviar + " + item3 + " roe") : ((item2 > 0) ? (item2 + " caviar") : (item3 + " roe")));
				bool flag = CanStripEggs(item);
				builder.AddLine("Surgery now: " + text2 + ", -" + num2 + "% health" + (flag ? "" : " (too weak)"), ignoreEmpty: true, flag ? RenderHandler.ColorPalette.LightCyan : RenderHandler.ColorPalette.Orange);
			}
		}
	}

	private static void AddRaiseOutlookLine(RichTextBuilder builder, GameItem item, FishSpecies species, RenderHandler.ColorPalette color)
	{
		int @int = item.GetTagReadonly("FISH_AGE_TAG").GetInt();
		int num = species.stageStartDays[5];
		if (@int < num)
		{
			float num2 = Mathf.Max(0f, GetExpectedKcalEatenAtAge(species, num) - GetExpectedKcalEatenAtAge(species, @int));
			int kcal = Mathf.FloorToInt((GetMeatFeedKcal(item, species) + num2) * species.conversionEfficiency);
			int leftoverScrapKcal = AquacultureItems.GetLeftoverScrapKcal(species, kcal);
			int num3 = ((IEnumerable<string>)AquacultureItems.BreakDownMeat(species, kcal)).Sum((Func<string, int>)AquacultureItems.GetMeatValue) + ((leftoverScrapKcal > 0) ? AquacultureItems.GetFishScrapValue(leftoverScrapKcal) : 0);
			if (IsPelletTainted(item))
			{
				num3 = num3 * 25 / 100;
			}
			string text = ((species.diet == FishDiet.Carnivore) ? " kcal of meat" : " kcal of feed");
			builder.AddLine("Raised to elder (day " + num + "): meat worth about " + num3 + " credits for " + Mathf.RoundToInt(num2) + text, ignoreEmpty: true, color);
		}
	}

	private static void CreateDevTooltip(RichTextBuilder builder, GameItem item, FishSpecies species)
	{
		RenderHandler.ColorPalette color = RenderHandler.ColorPalette.Gray;
		builder.AddLine("[dev]", ignoreEmpty: true, color);
		if (item.IsTag("FISH_ROE_TAG"))
		{
			builder.AddLine(item.IsTag("FISH_FERTILIZED_TAG") ? ("Fertilized: ages only inside a powered tank, then hatches into " + species.groupCount + " fry") : "Unfertilized: an adult male in the tank fertilizes it overnight; plain roe is FOOD and sells", ignoreEmpty: true, color);
			builder.AddLine("Any hungry meat-eating tank mate swallows it once other food runs out", ignoreEmpty: true, color);
			return;
		}
		if (item.IsTag("FISH_FRY_TAG"))
		{
			AddConversionLine(builder, item, species, color);
			builder.AddLine("Drain tonight about " + Mathf.RoundToInt(GetExpectedDrainTonight(item, species)) + " kcal (pays growth for its future juveniles in advance)", ignoreEmpty: true, color);
			builder.AddLine("Dies outside a tank; any hungry meat-eating tank mate swallows the clutch once other food runs out", ignoreEmpty: true, color);
			return;
		}
		if (IsDead(item))
		{
			builder.AddLine("Hungry tank mates feed on the carcass like a dispenser; it still fouls the water while it sits in the tank", ignoreEmpty: true, color);
			int @int = item.GetTagReadonly("FISH_CARCASS_KCAL_TAG").GetInt();
			builder.AddLine("Blade: " + DescribeEggYield(item, species) + DescribeYield(species, @int, verbose: true) + (IsPelletTainted(item) ? ", trash grade" : "") + DescribeGraftYield(item, species), ignoreEmpty: true, color);
			return;
		}
		FishStage stage = GetStage(item);
		int weightGrams = GetWeightGrams(item);
		AddConversionLine(builder, item, species, color);
		AddRaiseOutlookLine(builder, item, species, color);
		if (GetTargetWeightGrams(item) > weightGrams)
		{
			builder.AddLine("Growing to " + GetTargetWeightGrams(item) + " g by the next stage", ignoreEmpty: true, color);
		}
		builder.AddLine("Weight follows absorbed feed: a fully fed fish is " + GetWeightAtAge(species, item.GetTagReadonly("FISH_AGE_TAG").GetInt()) + " g at this age", ignoreEmpty: true, color);
		builder.AddLine("Drain tonight about " + Mathf.RoundToInt(GetExpectedDrainTonight(item, species)) + " kcal (upkeep " + species.maintenanceKcalPer100gPerNight + "/100 g of table weight + growth " + species.growthKcalPerGram + "/g)", ignoreEmpty: true, color);
		if (stage >= FishStage.Elder)
		{
			builder.AddLine("Elder: upkeep appetite x" + 0.5f + ", everything absorbed keeps building body at " + species.growthKcalPerGram + " kcal/g", ignoreEmpty: true, color);
		}
		builder.AddLine("Old age: " + 50 + "% a night from day " + species.oldAgeDays + " (age only counts fed nights)", ignoreEmpty: true, color);
		if (species.stagePowerPerNight != null)
		{
			builder.AddLine("Power by stage (juvenile/adolescent/adult/elder): " + string.Join("/", species.stagePowerPerNight.Skip(2)) + " kJ a night, summed over the tank, to the RAS battery and each adjacent battery or rechargeable machine; ignores station power, blackouts and commercial power; none while starving", ignoreEmpty: true, color);
		}
		builder.AddLine("Waste " + ((float)weightGrams / 100f * species.wasteMlPer100gPerNight).ToString("0.0") + " ml a night; dies outside a tank", ignoreEmpty: true, color);
		if (species.ignoresWaste)
		{
			builder.AddLine("Ignores fouled water: only low water or a dead pump can hurt it", ignoreEmpty: true, color);
		}
		else
		{
			builder.AddLine("Waste hurts above " + GetWasteDamagePercent(species).ToString("0.0") + "%, kills at " + GetWasteDeathPercent(species).ToString("0.0") + "%", ignoreEmpty: true, color);
		}
		if (species.skinGraftGrams > 0)
		{
			builder.AddLine("Skin: one graft per " + species.skinGraftGrams + " g of body weight (rounded down), on top of the meat, never trash grade", ignoreEmpty: true, color);
		}
		if (species.remediationMlPer100gPerNight > 0f)
		{
			builder.AddLine("Absorbs " + ((float)weightGrams / 100f * species.remediationMlPer100gPerNight).ToString("0.0") + " ml each of heavy metal, chemical and physical contaminant a night (base purifier floor), power or not", ignoreEmpty: true, color);
		}
		if (species.diet != FishDiet.Herbivore)
		{
			builder.AddLine("Eats live tank mates up to " + Mathf.FloorToInt((float)weightGrams * 0.2f) + " g once other food runs out", ignoreEmpty: true, color);
		}
		builder.AddLine("Prey for meat-eating tank mates of " + GetPredatorWeightThreshold(item) + " g or more", ignoreEmpty: true, color);
		if (stage >= FishStage.Adult)
		{
			builder.AddLine(species.CanBreed ? ("Breeds: " + species.breedChancePercent + "% a night per fed female with a fed male in the tank") : "Raise only: never breeds in a tank", ignoreEmpty: true, color);
		}
		if (species.LaysEggs)
		{
			if (stage < FishStage.Adult)
			{
				builder.AddLine("Starts building eggs as an adult (day " + species.stageStartDays[4] + ")", ignoreEmpty: true, color);
			}
			else
			{
				builder.AddLine("Eggs +" + Mathf.RoundToInt(species.eggGramsPerNight) + " g a night while fed, target " + Mathf.RoundToInt(species.eggLoadBodyRatio * 100f) + "% of stage weight, " + species.eggKcalPerGram + " kcal per g", ignoreEmpty: true, color);
			}
			builder.AddLine("Surgery jars taken so far: " + GetSurgeryJars(item) + " (caviar up to " + species.surgeryCaviarJars + ", mixed to " + (species.surgeryCaviarJars + species.surgeryMixedJars) + ", roe after)", ignoreEmpty: true, color);
			builder.AddLine("Surgery cost: " + Mathf.RoundToInt(species.surgeryHealthCostFull * 100f) + "% health on a full load, up to +" + Mathf.RoundToInt(species.surgeryHealthCostEarly * 100f) + "% when early; eggs regrow from " + Mathf.RoundToInt(species.eggRegrowHealthRatio * 100f) + "% health; the blade gives every full jar as caviar", ignoreEmpty: true, color);
		}
		builder.AddLine("Blade: " + DescribeEggYield(item, species) + DescribeYield(species, GetMeatKcal(item), verbose: true) + (IsPelletTainted(item) ? ", trash grade" : "") + DescribeGraftYield(item, species), ignoreEmpty: true, color);
	}

	public static GameItem GetHostRAS(GameItem stock)
	{
		GameInventory gameInventory = stock?.parentInventory;
		if (!IsInTank(gameInventory))
		{
			return null;
		}
		GameItem parentItem = gameInventory.GetParentItem();
		if (parentItem == null || !parentItem.IsTag("RAS_TAG"))
		{
			return null;
		}
		return parentItem;
	}

	public static bool WillSuffocateTonight(GameItem stock)
	{
		if (IsDead(stock))
		{
			return false;
		}
		GameItem hostRAS = GetHostRAS(stock);
		if (hostRAS == null)
		{
			return !stock.IsTag("FISH_ROE_TAG");
		}
		return !MachineHelper.CanPower(hostRAS);
	}

	private static void AddStatusLine(RichTextBuilder builder, GameItem item, FishSpecies species)
	{
		bool any = false;
		LocGenderHelper.LocGender gender = GetLocGender(item);
		if (IsDead(item))
		{
			Status(LocHelper.GetLocalizedMechanic("husbandry_status_dead"), RenderHandler.ColorPalette.LightRed);
		}
		if (WillSuffocateTonight(item))
		{
			Status(LocHelper.GetLocalizedMechanic("aquaculture_status_suffocating"), RenderHandler.ColorPalette.LightRed);
		}
		if (!IsDead(item) && !item.IsTag("FISH_ROE_TAG"))
		{
			float hunger = GetHunger(item);
			if (item.IsTag("FISH_IS_STARVING_TAG"))
			{
				Status(LocHelper.GetLocalizedMechanic("husbandry_status_starving"), RenderHandler.ColorPalette.Orange);
			}
			else if (hunger < species.maxHunger * 0.75f)
			{
				Status(LocHelper.GetLocalizedMechanic("aquaculture_status_hungry"), RenderHandler.ColorPalette.Yellow);
			}
		}
		GameItem hostRAS = GetHostRAS(item);
		if (hostRAS != null && !IsDead(item))
		{
			GameItem gameItem = MachineRAS.GetWaterSlot(hostRAS)?.childItem;
			int totalWaterNeedMl = GetTotalWaterNeedMl(MachineRAS.GetTankGrid(hostRAS));
			int waterMl = GetWaterMl(gameItem);
			if (gameItem == null)
			{
				Status(LocHelper.GetLocalizedMechanic("aquaculture_status_no_water"), RenderHandler.ColorPalette.LightRed);
			}
			else if (totalWaterNeedMl > 0 && waterMl < totalWaterNeedMl)
			{
				Status(LocHelper.GetLocalizedMechanic("aquaculture_status_low_water"), ((float)waterMl < (float)totalWaterNeedMl * 0.5f) ? RenderHandler.ColorPalette.LightRed : RenderHandler.ColorPalette.Yellow);
			}
			float wastePercent = GetWastePercent(gameItem);
			if (!species.ignoresWaste)
			{
				if (wastePercent >= GetWasteDeathPercent(species))
				{
					Status(LocHelper.GetLocalizedMechanic("aquaculture_status_toxic_water"), RenderHandler.ColorPalette.LightRed);
				}
				else if (wastePercent > GetWasteDamagePercent(species))
				{
					Status(LocHelper.GetLocalizedMechanic("aquaculture_status_polluted_water"), RenderHandler.ColorPalette.Yellow);
				}
			}
		}
		if (item.IsTag("FISH_ROE_TAG"))
		{
			Status(LocHelper.GetLocalizedMechanic(item.IsTag("FISH_FERTILIZED_TAG") ? "aquaculture_status_fertilized" : "aquaculture_status_unfertilized"), item.IsTag("FISH_FERTILIZED_TAG") ? RenderHandler.ColorPalette.Green : RenderHandler.ColorPalette.Cyan);
		}
		if (IsFish(item) && !IsDead(item) && GetStage(item) >= FishStage.Adult)
		{
			Status(species.CanBreed ? LocHelper.GetLocalizedMechanic("husbandry_status_mature") : LocHelper.GetLocalizedMechanic("aquaculture_status_raise_only"), RenderHandler.ColorPalette.Cyan);
		}
		if (IsFish(item) && IsGravid(item))
		{
			Status(LocHelper.GetLocalizedMechanic("aquaculture_status_gravid"), RenderHandler.ColorPalette.Green);
		}
		if (IsFish(item) && !IsDead(item) && species.LaysEggs && GetStage(item) >= FishStage.Adult && !CanGrowEggs(item, species))
		{
			Status(LocHelper.GetLocalizedMechanic("aquaculture_status_recovering"), RenderHandler.ColorPalette.Orange);
		}
		if (any)
		{
			builder.AddLine(" ");
		}
		void Status(string text, RenderHandler.ColorPalette color)
		{
			builder.Add("[" + LocGenderHelper.Resolve(text, gender) + "] ", color);
			any = true;
		}
	}

	private static string DescribeYield(FishSpecies species, int kcal, bool verbose)
	{
		List<string> list = AquacultureItems.BreakDownMeat(species, kcal);
		int leftoverScrapKcal = AquacultureItems.GetLeftoverScrapKcal(species, kcal);
		if (list.Count == 0 && leftoverScrapKcal == 0)
		{
			if (!verbose)
			{
				return "nothing yet";
			}
			return "nothing yet (" + kcal + " kcal, a fish scrap needs " + 25 + ")";
		}
		List<string> list2 = (from id in list
			group id by id into g
			select ((g.Count() > 1) ? (g.Count() + "x ") : "") + g.Key.Replace('_', ' ')).ToList();
		if (leftoverScrapKcal > 0)
		{
			list2.Add("fish scrap (" + leftoverScrapKcal + " kcal)");
		}
		string text = string.Join(" + ", list2);
		if (!verbose)
		{
			return text;
		}
		return text + " (" + (((IEnumerable<string>)list).Sum((Func<string, int>)AquacultureItems.GetMeatKcal) + leftoverScrapKcal) + " of " + kcal + " kcal)";
	}

	private static LocGenderHelper.LocGender GetLocGender(GameItem item)
	{
		if (!item.IsTag("FISH_SEX_TAG"))
		{
			return LocGenderHelper.LocGender.Neutral;
		}
		if (!(item.GetTagReadonly("FISH_SEX_TAG").GetString() == "FEMALE"))
		{
			return LocGenderHelper.LocGender.Male;
		}
		return LocGenderHelper.LocGender.Female;
	}

	private static void AddHeaderLine(RichTextBuilder builder, GameItem item, FishSpecies species)
	{
		string sexeDisplay = HusbandryHelper.GetSexeDisplay(item.GetTagReadonly("FISH_SEX_TAG").GetString());
		string localizedMechanic = LocHelper.GetLocalizedMechanic("husbandry_tooltip_header", sexeDisplay, FishSpecies.GetStageDisplay(GetStage(item)), species.GetDisplayName());
		builder.AddLine(LocGenderHelper.Resolve(localizedMechanic, GetLocGender(item)), ignoreEmpty: true, RenderHandler.ColorPalette.Cyan);
	}

	private static string DescribeGraftYield(GameItem item, FishSpecies species)
	{
		int skinGraftCount = GetSkinGraftCount(species, GetWeightGrams(item));
		if (skinGraftCount <= 0)
		{
			return "";
		}
		return " + " + skinGraftCount + "x skin graft";
	}

	private static string DescribeEggYield(GameItem item, FishSpecies species)
	{
		if (!species.LaysEggs || GetEggJars(item) <= 0)
		{
			return "";
		}
		return GetEggJars(item) + "x caviar + ";
	}

	private static void AddConversionLine(RichTextBuilder builder, GameItem item, FishSpecies species, RenderHandler.ColorPalette color)
	{
		builder.AddLine("Fed " + Mathf.RoundToInt(GetKcalEaten(item)) + " kcal so far, worth " + GetMeatKcal(item) + " kcal of meat (" + Mathf.RoundToInt(species.conversionEfficiency * 100f) + "%)", ignoreEmpty: true, color);
		float pelletShare = GetPelletShare(item);
		if (pelletShare > 0f)
		{
			builder.AddLine(Mathf.RoundToInt(pelletShare * 100f) + "% of it from gutterflow pellets, trash grade above " + Mathf.RoundToInt(25f) + "%", ignoreEmpty: true, color);
		}
	}

	private static void AddPelletLine(RichTextBuilder builder, GameItem item)
	{
		float pelletShare = GetPelletShare(item);
		if (!(pelletShare <= 0f))
		{
			bool flag = IsPelletTainted(item);
			builder.AddLine(LocHelper.GetLocalizedMechanic(flag ? "aquaculture_pellet_share_trash" : "aquaculture_pellet_share", Mathf.RoundToInt(pelletShare * 100f)), ignoreEmpty: true, flag ? RenderHandler.ColorPalette.LightRed : RenderHandler.ColorPalette.Yellow);
		}
	}

	private static void AddGrowthSpeedLine(RichTextBuilder builder, GameItem item)
	{
		float growthSpeed = GetGrowthSpeed(item);
		if (!(Mathf.Abs(growthSpeed - 1f) < 0.001f))
		{
			RenderHandler.ColorPalette color = ((growthSpeed < 1f) ? RenderHandler.ColorPalette.Yellow : RenderHandler.ColorPalette.Green);
			builder.AddLine(LocHelper.GetLocalizedMechanic("aquaculture_growth_speed", Mathf.RoundToInt(growthSpeed * 100f)), ignoreEmpty: true, color);
		}
	}

	private static void AddHealthLine(RichTextBuilder builder, GameItem item, FishSpecies species)
	{
		float health = GetHealth(item);
		float maxHealth = species.maxHealth;
		RenderHandler.ColorPalette color = ((health < maxHealth * 0.25f) ? RenderHandler.ColorPalette.LightRed : ((health < maxHealth * 0.6f) ? RenderHandler.ColorPalette.Yellow : RenderHandler.ColorPalette.Green));
		builder.Add(LocHelper.GetLocalizedMechanic("husbandry_tooltip_health"), RenderHandler.ColorPalette.LightCyan);
		builder.AddProgressBar((maxHealth > 0f) ? (health / maxHealth) : 0f, 10, color, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
		builder.AddLine("(" + Mathf.RoundToInt(health) + "/" + Mathf.RoundToInt(maxHealth) + ")", ignoreEmpty: true, color);
	}

	private static void AddHungerLine(RichTextBuilder builder, GameItem item, FishSpecies species)
	{
		float hunger = GetHunger(item);
		float maxHunger = species.maxHunger;
		builder.Add(LocHelper.GetLocalizedMechanic("husbandry_tooltip_hunger"), RenderHandler.ColorPalette.LightCyan);
		builder.AddProgressBar((maxHunger > 0f) ? (hunger / maxHunger) : 0f, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
		builder.AddLine("(" + Mathf.RoundToInt(hunger) + "/" + Mathf.RoundToInt(maxHunger) + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
	}

	public static float GetExpectedDrainTonight(GameItem item, FishSpecies species)
	{
		int @int = item.GetTagReadonly("FISH_AGE_TAG").GetInt();
		float gainedGrams = 0f;
		if (item.IsTag("FISH_FRY_TAG"))
		{
			gainedGrams = GetFryGainPerNight(species, Mathf.Max(1, item.GetTagReadonly("FISH_GROUP_COUNT_TAG").GetInt()));
		}
		else if (IsFish(item))
		{
			gainedGrams = GetWeightAtAge(species, @int + 1) - GetWeightAtAge(species, @int);
		}
		return Mathf.Max(5f, GetStockDrain(item, species, @int + 1, gainedGrams));
	}
}

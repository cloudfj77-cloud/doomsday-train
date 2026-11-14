using System;
using System.Collections.Generic;
using UnityEngine;

public class ModuleEffectHelper
{
	public class ModuleEffect
	{
		public enum ModuleEffectType
		{
			Universal,
			ModuleOnly,
			NodeOnly
		}

		public ModuleEffectType effectType;

		public string identifier;

		public string displayName;

		public string description;

		public Action<GameItem, int> onCreateAction;

		public Action<GameItem> onRemoveAction;

		public Action<GameItem, GameInventory> onAnyAction;

		public Action<GameItem> onUsedAction;

		public List<string> incompatibleWithEffects;

		public ModuleEffect()
		{
			effectType = ModuleEffectType.Universal;
			incompatibleWithEffects = new List<string>();
		}
	}

	public const string LEARNING_ALGO_MAX_PERFORMANCE_INT = "LEARNING_ALGO_MAX_PERFORMANCE_INT";

	public const string LEARNING_ALGO_MAX_EFFICIENCY_INT = "LEARNING_ALGO_MAX_EFFICIENCY_INT";

	public const string LEARNING_ALGO_MAX_QUALITY_INT = "LEARNING_ALGO_MAX_QUALITY_INT";

	private const float PREMIUM_STAT_MULT = 1.2f;

	private const int PREMIUM_VALUE_ADJUST = 40;

	private const float INFERIOR_STAT_MULT = 0.8f;

	private const int INFERIOR_VALUE_ADJUST = -40;

	private const int RECEIVER_VALUE_ADJUST = 15;

	private const int RECEIVER_BONUS_PER_NODE = 10;

	private const int TOP_MOUNT_VALUE_ADJUST = 15;

	private const int TOP_MOUNT_BONUS = 20;

	private const int SIDE_MOUNT_VALUE_ADJUST = 15;

	private const int SIDE_MOUNT_BONUS = 20;

	private const float OVERVOLTED_STAT_MULT = 2f;

	private const int OVERVOLTED_RUIN_CHANCE = 10;

	private const float DEGRADING_STAT_MULT = 2f;

	private const int DEGRADING_LOSS_PER_USE = 1;

	private const float NEGATIVE_FEEDBACK_STAT_MULT = 1.25f;

	private const int NEGATIVE_FEEDBACK_PENALTY_DIVISOR = 4;

	private const int SUPPORT_NODE_BONUS_DIVISOR = 5;

	private const int LEARNING_ALGO_MAX_MULT = 2;

	private const int LEARNING_ALGO_GAIN_PER_USE = 1;

	public static Dictionary<string, ModuleEffect> moduleEffects = new Dictionary<string, ModuleEffect>
	{
		{
			"MODULE_EFFECT_PREMIUM",
			new ModuleEffect
			{
				identifier = "MODULE_EFFECT_PREMIUM",
				effectType = ModuleEffect.ModuleEffectType.ModuleOnly,
				displayName = LocHelper.GetLocalizedMechanic("mech_module_effect_premium_name"),
				description = LocHelper.GetLocalizedMechanic("mech_module_effect_premium_desc", 20),
				onCreateAction = delegate(GameItem item, int slot)
				{
					ModuleHelper.SetBaseStatMult(item, 1.2f);
					AdjustValue(item, 40);
				},
				onAnyAction = delegate
				{
				},
				incompatibleWithEffects = new List<string> { "MODULE_EFFECT_INFERIOR", "MODULE_EFFECT_LEARNING_ALGORITHM" }
			}
		},
		{
			"MODULE_EFFECT_INFERIOR",
			new ModuleEffect
			{
				identifier = "MODULE_EFFECT_INFERIOR",
				effectType = ModuleEffect.ModuleEffectType.ModuleOnly,
				displayName = LocHelper.GetLocalizedMechanic("mech_module_effect_inferior_name"),
				description = LocHelper.GetLocalizedMechanic("mech_module_effect_inferior_desc", 19),
				onCreateAction = delegate(GameItem item, int slot)
				{
					ModuleHelper.SetBaseStatMult(item, 0.8f);
					AdjustValue(item, -40);
				},
				onAnyAction = delegate
				{
				},
				incompatibleWithEffects = new List<string> { "MODULE_EFFECT_PREMIUM", "MODULE_EFFECT_LEARNING_ALGORITHM" }
			}
		},
		{
			"MODULE_EFFECT_RECEIVER",
			new ModuleEffect
			{
				identifier = "MODULE_EFFECT_RECEIVER",
				effectType = ModuleEffect.ModuleEffectType.ModuleOnly,
				displayName = LocHelper.GetLocalizedMechanic("mech_module_effect_receiver_name"),
				description = LocHelper.GetLocalizedMechanic("mech_module_effect_receiver_desc", 10),
				onCreateAction = delegate(GameItem item, int slot)
				{
					AdjustValue(item, 15);
				},
				onAnyAction = delegate(GameItem item, GameInventory inventory)
				{
					List<GameItem> list3 = item.FindAdjacentType<GameItem>();
					int num = 0;
					foreach (GameItem item3 in list3)
					{
						if (item3.IsTag("NODE"))
						{
							num++;
						}
					}
					ModifyTempStatFromBaseByPercentage(item, 10 * num);
				}
			}
		},
		{
			"MODULE_EFFECT_TOP_MOUNT",
			new ModuleEffect
			{
				identifier = "MODULE_EFFECT_TOP_MOUNT",
				effectType = ModuleEffect.ModuleEffectType.ModuleOnly,
				displayName = LocHelper.GetLocalizedMechanic("mech_module_effect_top_mount_name"),
				description = LocHelper.GetLocalizedMechanic("mech_module_effect_top_mount_desc", 20),
				onCreateAction = delegate(GameItem item, int slot)
				{
					AdjustValue(item, 15);
				},
				onAnyAction = delegate(GameItem item, GameInventory inventory)
				{
					if (item.modifiedShape.IsTouchingTop())
					{
						ModifyTempStatFromBaseByPercentage(item, 20);
					}
				}
			}
		},
		{
			"MODULE_EFFECT_SIDE_MOUNT",
			new ModuleEffect
			{
				identifier = "MODULE_EFFECT_SIDE_MOUNT",
				effectType = ModuleEffect.ModuleEffectType.ModuleOnly,
				displayName = LocHelper.GetLocalizedMechanic("mech_module_effect_side_mount_name"),
				description = LocHelper.GetLocalizedMechanic("mech_module_effect_side_mount_desc", 20),
				onCreateAction = delegate(GameItem item, int slot)
				{
					AdjustValue(item, 15);
				},
				onAnyAction = delegate(GameItem item, GameInventory inventory)
				{
					if (item.modifiedShape.IsTouchingLeft())
					{
						ModifyTempStatFromBaseByPercentage(item, 20);
					}
				}
			}
		},
		{
			"MODULE_EFFECT_OVERVOLTED",
			new ModuleEffect
			{
				identifier = "MODULE_EFFECT_OVERVOLTED",
				effectType = ModuleEffect.ModuleEffectType.ModuleOnly,
				displayName = LocHelper.GetLocalizedMechanic("mech_module_effect_overvolted_name"),
				description = LocHelper.GetLocalizedMechanic("mech_module_effect_overvolted_desc", 200, 10),
				onCreateAction = delegate(GameItem item, int slot)
				{
					ModuleHelper.SetBaseStatMult(item, 2f);
				},
				onUsedAction = delegate(GameItem item)
				{
					if (RNG.Roll(10))
					{
						GameInventory parentInventory = item.parentInventory;
						item.Destroy();
						GameItem item2 = ItemSpawner.Spawn("system_module_ruined");
						GraphUtils.TryAcceptAll(parentInventory, item2);
					}
				},
				incompatibleWithEffects = new List<string> { "MODULE_EFFECT_LEARNING_ALGORITHM" }
			}
		},
		{
			"MODULE_EFFECT_DEGRADING",
			new ModuleEffect
			{
				identifier = "MODULE_EFFECT_DEGRADING",
				effectType = ModuleEffect.ModuleEffectType.ModuleOnly,
				displayName = LocHelper.GetLocalizedMechanic("mech_module_effect_degrading_name"),
				description = LocHelper.GetLocalizedMechanic("mech_module_effect_degrading_desc", 1),
				onCreateAction = delegate(GameItem item, int slot)
				{
					ModuleHelper.SetBaseStatMult(item, 2f);
				},
				onUsedAction = delegate(GameItem item)
				{
					Degrade(item, 1);
				},
				incompatibleWithEffects = new List<string> { "MODULE_EFFECT_LEARNING_ALGORITHM" }
			}
		},
		{
			"MODULE_EFFECT_LEARNING_ALGORITHM",
			new ModuleEffect
			{
				identifier = "MODULE_EFFECT_LEARNING_ALGORITHM",
				effectType = ModuleEffect.ModuleEffectType.ModuleOnly,
				displayName = LocHelper.GetLocalizedMechanic("mech_module_effect_learning_algorithm_name"),
				description = LocHelper.GetLocalizedMechanic("mech_module_effect_learning_algorithm_desc", 1),
				onCreateAction = delegate(GameItem item, int slot)
				{
					InitLearningAlgo(item);
				},
				onUsedAction = delegate(GameItem item)
				{
					OnUsedLearningAlgo(item);
				},
				incompatibleWithEffects = new List<string> { "MODULE_EFFECT_DEGRADING", "MODULE_EFFECT_OVERVOLTED", "MODULE_EFFECT_PREMIUM", "MODULE_EFFECT_INFERIOR", "MODULE_EFFECT_NEGATIVE_FEEDBACK" }
			}
		},
		{
			"MODULE_EFFECT_NEGATIVE_FEEDBACK",
			new ModuleEffect
			{
				identifier = "MODULE_EFFECT_NEGATIVE_FEEDBACK",
				effectType = ModuleEffect.ModuleEffectType.ModuleOnly,
				displayName = LocHelper.GetLocalizedMechanic("mech_module_effect_negative_feedback_name"),
				description = LocHelper.GetLocalizedMechanic("mech_module_effect_negative_feedback_desc", 25, 25),
				onCreateAction = delegate(GameItem item, int slot)
				{
					ModuleHelper.SetBaseStatMult(item, 1.25f);
				},
				onAnyAction = delegate(GameItem item, GameInventory inventory)
				{
					List<GameItem> list2 = item.FindAdjacentType<GameItem>();
					int totalNegativePerformance = 0;
					int totalNegativeEfficiency = 0;
					int totalNegativeQuality = 0;
					foreach (GameItem item4 in list2)
					{
						if (item4 != item)
						{
							int @int = item4.GetTagReadonly("BONUS_PERCENTAGE_PERFORMANCE_INT").GetInt();
							int int2 = item4.GetTagReadonly("BONUS_PERCENTAGE_EFFICIENCY_INT").GetInt();
							int int3 = item4.GetTagReadonly("BONUS_PERCENTAGE_QUALITY_INT").GetInt();
							if (@int < 0)
							{
								totalNegativePerformance += @int;
							}
							if (int2 < 0)
							{
								totalNegativeEfficiency += int2;
							}
							if (int3 < 0)
							{
								totalNegativeQuality += int3;
							}
						}
					}
					int currentTempPerformance = item.GetTagReadonly("TEMP_PERCENTAGE_PERFORMANCE_INT").GetInt();
					int currentTempEfficiency = item.GetTagReadonly("TEMP_PERCENTAGE_EFFICIENCY_INT").GetInt();
					int currentTempQuality = item.GetTagReadonly("TEMP_PERCENTAGE_QUALITY_INT").GetInt();
					item.ModifyTag("TEMP_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
					{
						state.Enable().SetInt(currentTempPerformance + totalNegativePerformance / 4);
					});
					item.ModifyTag("TEMP_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
					{
						state.Enable().SetInt(currentTempEfficiency + totalNegativeEfficiency / 4);
					});
					item.ModifyTag("TEMP_PERCENTAGE_QUALITY_INT", delegate(TagState state)
					{
						state.Enable().SetInt(currentTempQuality + totalNegativeQuality / 4);
					});
				},
				incompatibleWithEffects = new List<string> { "MODULE_EFFECT_LEARNING_ALGORITHM" }
			}
		},
		{
			"NODE_EFFECT_SUPPORT",
			new ModuleEffect
			{
				identifier = "NODE_EFFECT_SUPPORT",
				effectType = ModuleEffect.ModuleEffectType.NodeOnly,
				displayName = LocHelper.GetLocalizedMechanic("mech_module_effect_support_node_name"),
				description = LocHelper.GetLocalizedMechanic("mech_module_effect_support_node_desc", 20),
				onCreateAction = delegate
				{
				},
				onAnyAction = delegate(GameItem item, GameInventory inventory)
				{
					List<GameItem> list = item.FindAdjacentType<GameItem>();
					int bonusPerformance = 0;
					int bonusEfficiency = 0;
					int bonusQuality = 0;
					foreach (GameItem item5 in list)
					{
						if (item5 != item && item5.IsGameItemType("MODULE"))
						{
							bonusPerformance += item5.GetTagReadonly("BONUS_PERCENTAGE_PERFORMANCE_INT").GetInt();
							bonusEfficiency += item5.GetTagReadonly("BONUS_PERCENTAGE_EFFICIENCY_INT").GetInt();
							bonusQuality += item5.GetTagReadonly("BONUS_PERCENTAGE_QUALITY_INT").GetInt();
						}
					}
					item.ModifyTag("TEMP_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
					{
						state.Enable().SetInt(bonusPerformance / 5);
					});
					item.ModifyTag("TEMP_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
					{
						state.Enable().SetInt(bonusEfficiency / 5);
					});
					item.ModifyTag("TEMP_PERCENTAGE_QUALITY_INT", delegate(TagState state)
					{
						state.Enable().SetInt(bonusQuality / 5);
					});
				}
			}
		}
	};

	public const string MODULE_EFFECT1_TAG = "MODULE_EFFECT1_TAG";

	public const string MODULE_EFFECT2_TAG = "MODULE_EFFECT2_TAG";

	public const string MODULE_EFFECT_PREMIUM = "MODULE_EFFECT_PREMIUM";

	public const string MODULE_EFFECT_INFERIOR = "MODULE_EFFECT_INFERIOR";

	public const string MODULE_EFFECT_RECEIVER = "MODULE_EFFECT_RECEIVER";

	public const string MODULE_EFFECT_TOP_MOUNT = "MODULE_EFFECT_TOP_MOUNT";

	public const string MODULE_EFFECT_SIDE_MOUNT = "MODULE_EFFECT_SIDE_MOUNT";

	public const string MODULE_EFFECT_NEGATIVE_FEEDBACK = "MODULE_EFFECT_NEGATIVE_FEEDBACK";

	public const string MODULE_EFFECT_DEGRADING = "MODULE_EFFECT_DEGRADING";

	public const string MODULE_EFFECT_LEARNING_ALGORITHM = "MODULE_EFFECT_LEARNING_ALGORITHM";

	public const string MODULE_EFFECT_OVERVOLTED = "MODULE_EFFECT_OVERVOLTED";

	public const string NODE_EFFECT_SUPPORT = "NODE_EFFECT_SUPPORT";

	public static void InitNode(GameItem gameItem)
	{
		gameItem.SetGameItemType("NODE");
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL");
		gameItem.RemoveGameItemType("MODULE");
	}

	public static void CreateModuleEffectTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("MODULE_EFFECT1_TAG"))
		{
			string displayName = moduleEffects[item.GetTagReadonly("MODULE_EFFECT1_TAG").GetString()].displayName;
			string description = moduleEffects[item.GetTagReadonly("MODULE_EFFECT1_TAG").GetString()].description;
			builder.AddLine(displayName, ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue, bold: true);
			builder.AddLine(description, ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
		}
		if (item.IsTag("MODULE_EFFECT2_TAG"))
		{
			string displayName2 = moduleEffects[item.GetTagReadonly("MODULE_EFFECT2_TAG").GetString()].displayName;
			string description2 = moduleEffects[item.GetTagReadonly("MODULE_EFFECT2_TAG").GetString()].description;
			builder.AddLine(displayName2, ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue, bold: true);
			builder.AddLine(description2, ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
		}
	}

	public static void AddEffectToModule(GameItem item, string effectIdentifier)
	{
		if (!item.IsTag("MODULE_EFFECT1_TAG"))
		{
			item.ModifyTag("MODULE_EFFECT1_TAG", delegate(TagState state)
			{
				state.Enable().SetString(effectIdentifier);
			});
			if (moduleEffects.ContainsKey(effectIdentifier) && moduleEffects[effectIdentifier].onCreateAction != null)
			{
				moduleEffects[effectIdentifier].onCreateAction(item, 1);
			}
		}
		else
		{
			if (item.IsTag("MODULE_EFFECT2_TAG") || (item.IsTag("MODULE_EFFECT1_TAG") && item.GetTagReadonly("MODULE_EFFECT1_TAG").GetString().Equals(effectIdentifier)))
			{
				return;
			}
			string @string = item.GetTagReadonly("MODULE_EFFECT1_TAG").GetString();
			if (!moduleEffects.ContainsKey(@string) || !moduleEffects[@string].incompatibleWithEffects.Contains(effectIdentifier))
			{
				item.ModifyTag("MODULE_EFFECT2_TAG", delegate(TagState state)
				{
					state.Enable().SetString(effectIdentifier);
				});
				if (moduleEffects.ContainsKey(effectIdentifier) && moduleEffects[effectIdentifier].onCreateAction != null)
				{
					moduleEffects[effectIdentifier].onCreateAction(item, 2);
				}
			}
		}
	}

	public static void RemoveEffectFromModule(GameItem item, string effectIdentifier)
	{
		if (item.IsTag("MODULE_EFFECT1_TAG") && item.GetTagReadonly("MODULE_EFFECT1_TAG").GetString() == effectIdentifier)
		{
			if (moduleEffects.ContainsKey(effectIdentifier) && moduleEffects[effectIdentifier].onRemoveAction != null)
			{
				moduleEffects[effectIdentifier].onRemoveAction(item);
			}
			item.ModifyTag("MODULE_EFFECT1_TAG", delegate(TagState state)
			{
				state.SetString("");
			});
			item.DisableTag("MODULE_EFFECT1_TAG");
		}
		else if (item.IsTag("MODULE_EFFECT2_TAG") && item.GetTagReadonly("MODULE_EFFECT2_TAG").GetString() == effectIdentifier)
		{
			if (moduleEffects.ContainsKey(effectIdentifier) && moduleEffects[effectIdentifier].onRemoveAction != null)
			{
				moduleEffects[effectIdentifier].onRemoveAction(item);
			}
			item.ModifyTag("MODULE_EFFECT2_TAG", delegate(TagState state)
			{
				state.SetString("");
			});
			item.DisableTag("MODULE_EFFECT2_TAG");
		}
	}

	public static void HandleModuleEffectAction(GameItem item, GameInventory inventory)
	{
		if (item.IsTag("MODULE_EFFECT1_TAG"))
		{
			string @string = item.GetTagReadonly("MODULE_EFFECT1_TAG").GetString();
			if (moduleEffects.ContainsKey(@string) && moduleEffects[@string].onAnyAction != null)
			{
				moduleEffects[@string].onAnyAction(item, inventory);
			}
		}
		if (item.IsTag("MODULE_EFFECT2_TAG"))
		{
			string string2 = item.GetTagReadonly("MODULE_EFFECT2_TAG").GetString();
			if (moduleEffects.ContainsKey(string2) && moduleEffects[string2].onAnyAction != null)
			{
				moduleEffects[string2].onAnyAction(item, inventory);
			}
		}
	}

	public static void ResetAllTempStat(GameItem module)
	{
		int permanentPerformance = module.GetTagReadonly("BONUS_PERCENTAGE_PERFORMANCE_INT").GetInt();
		int permanentEfficiency = module.GetTagReadonly("BONUS_PERCENTAGE_EFFICIENCY_INT").GetInt();
		int permanentQuality = module.GetTagReadonly("BONUS_PERCENTAGE_QUALITY_INT").GetInt();
		module.ModifyTag("TEMP_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
		{
			state.SetInt(permanentPerformance);
		});
		module.ModifyTag("TEMP_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
		{
			state.SetInt(permanentEfficiency);
		});
		module.ModifyTag("TEMP_PERCENTAGE_QUALITY_INT", delegate(TagState state)
		{
			state.SetInt(permanentQuality);
		});
	}

	public static void ComputeAllModuleTempStat(List<GameItem> moduleList)
	{
		foreach (GameItem module in moduleList)
		{
			ResetAllTempStat(module);
		}
		foreach (GameItem module2 in moduleList)
		{
			ComputeTemporaryStat(module2);
		}
	}

	public static void OnAddedModuleToGrid(GameItem module, GameItem machine, List<GameItem> moduleList)
	{
		AudioManager.Instance.Play3("module_insert");
		ComputeAllModuleTempStat(moduleList);
		AchievementTracker.Instance.CheckOverload(ModuleHelper.SumQualityBonus(moduleList));
	}

	public static void OnRemovedModuleFromGrid(GameItem module, GameItem machine, List<GameItem> moduleList)
	{
		AudioManager.Instance.Play3("module_remove");
		ComputeAllModuleTempStat(moduleList);
		ResetAllTempStat(module);
	}

	public static void ComputeTemporaryStat(GameItem item)
	{
		HandleModuleEffectAction(item, null);
	}

	public static void ModifyTempStatFromBaseByPercentage(GameItem item, int percentage)
	{
		int basePerformance = item.GetTagReadonly("BONUS_PERCENTAGE_PERFORMANCE_INT").GetInt();
		int baseEfficiency = item.GetTagReadonly("BONUS_PERCENTAGE_EFFICIENCY_INT").GetInt();
		int baseQuality = item.GetTagReadonly("BONUS_PERCENTAGE_QUALITY_INT").GetInt();
		int currentTempPerformance = item.GetTagReadonly("TEMP_PERCENTAGE_PERFORMANCE_INT").GetInt();
		int currentTempEfficiency = item.GetTagReadonly("TEMP_PERCENTAGE_EFFICIENCY_INT").GetInt();
		int currentTempQuality = item.GetTagReadonly("TEMP_PERCENTAGE_QUALITY_INT").GetInt();
		item.ModifyTag("TEMP_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
		{
			state.SetInt(currentTempPerformance + basePerformance * percentage / 100);
		});
		item.ModifyTag("TEMP_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
		{
			state.SetInt(currentTempEfficiency + baseEfficiency * percentage / 100);
		});
		item.ModifyTag("TEMP_PERCENTAGE_QUALITY_INT", delegate(TagState state)
		{
			state.SetInt(currentTempQuality + baseQuality * percentage / 100);
		});
	}

	public static void Degrade(GameItem gameItem, int amount)
	{
		int basePerformance = gameItem.GetTagReadonly("BONUS_PERCENTAGE_PERFORMANCE_INT").GetInt();
		int baseEfficiency = gameItem.GetTagReadonly("BONUS_PERCENTAGE_EFFICIENCY_INT").GetInt();
		int baseQuality = gameItem.GetTagReadonly("BONUS_PERCENTAGE_QUALITY_INT").GetInt();
		if (basePerformance > 0)
		{
			gameItem.ModifyTag("BONUS_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
			{
				state.SetInt(Mathf.Max(0, basePerformance - amount));
			});
		}
		if (baseEfficiency > 0)
		{
			gameItem.ModifyTag("BONUS_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
			{
				state.SetInt(Mathf.Max(0, baseEfficiency - amount));
			});
		}
		if (baseQuality > 0)
		{
			gameItem.ModifyTag("BONUS_PERCENTAGE_QUALITY_INT", delegate(TagState state)
			{
				state.SetInt(Mathf.Max(0, baseQuality - amount));
			});
		}
	}

	public static void InitLearningAlgo(GameItem gameItem)
	{
		int basePerformance = gameItem.GetTagReadonly("BONUS_PERCENTAGE_PERFORMANCE_INT").GetInt();
		int baseEfficiency = gameItem.GetTagReadonly("BONUS_PERCENTAGE_EFFICIENCY_INT").GetInt();
		int baseQuality = gameItem.GetTagReadonly("BONUS_PERCENTAGE_QUALITY_INT").GetInt();
		if (basePerformance > 0)
		{
			gameItem.ModifyTag("LEARNING_ALGO_MAX_PERFORMANCE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(basePerformance * 2);
			});
			gameItem.ModifyTag("BONUS_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
			{
				state.SetInt(0);
			});
		}
		if (baseEfficiency > 0)
		{
			gameItem.ModifyTag("LEARNING_ALGO_MAX_EFFICIENCY_INT", delegate(TagState state)
			{
				state.Enable().SetInt(baseEfficiency * 2);
			});
			gameItem.ModifyTag("BONUS_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
			{
				state.SetInt(0);
			});
		}
		if (baseQuality > 0)
		{
			gameItem.ModifyTag("LEARNING_ALGO_MAX_QUALITY_INT", delegate(TagState state)
			{
				state.Enable().SetInt(baseQuality * 2);
			});
			gameItem.ModifyTag("BONUS_PERCENTAGE_QUALITY_INT", delegate(TagState state)
			{
				state.SetInt(0);
			});
		}
		ResetAllTempStat(gameItem);
	}

	public static void OnUsedLearningAlgo(GameItem gameItem)
	{
		int @int = gameItem.GetTagReadonly("LEARNING_ALGO_MAX_PERFORMANCE_INT").GetInt();
		int int2 = gameItem.GetTagReadonly("LEARNING_ALGO_MAX_EFFICIENCY_INT").GetInt();
		int int3 = gameItem.GetTagReadonly("LEARNING_ALGO_MAX_QUALITY_INT").GetInt();
		int currentPerformance = gameItem.GetTagReadonly("BONUS_PERCENTAGE_PERFORMANCE_INT").GetInt();
		int currentEfficiency = gameItem.GetTagReadonly("BONUS_PERCENTAGE_EFFICIENCY_INT").GetInt();
		int currentQuality = gameItem.GetTagReadonly("BONUS_PERCENTAGE_QUALITY_INT").GetInt();
		if (currentPerformance < @int)
		{
			gameItem.ModifyTag("BONUS_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
			{
				state.SetInt(currentPerformance + 1);
			});
		}
		if (currentEfficiency < int2)
		{
			gameItem.ModifyTag("BONUS_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
			{
				state.SetInt(currentEfficiency + 1);
			});
		}
		if (currentQuality < int3)
		{
			gameItem.ModifyTag("BONUS_PERCENTAGE_QUALITY_INT", delegate(TagState state)
			{
				state.SetInt(currentQuality + 1);
			});
		}
	}

	public static void AdjustValue(GameItem item, int value)
	{
		int @int = item.GetTagReadonly("MODULE_TIER_INT").GetInt();
		item.unitValue += value * @int;
		if (item.unitValue < 1)
		{
			item.unitValue = 1L;
		}
	}

	public static void ActivateOnUseAction(GameItem item, GameInventory moduleGrid, GameItem machine)
	{
		if (item.IsTag("MODULE_EFFECT1_TAG"))
		{
			string @string = item.GetTagReadonly("MODULE_EFFECT1_TAG").GetString();
			if (moduleEffects.ContainsKey(@string) && moduleEffects[@string].onUsedAction != null)
			{
				moduleEffects[@string].onUsedAction(item);
			}
		}
		if (item.IsTag("MODULE_EFFECT2_TAG"))
		{
			string string2 = item.GetTagReadonly("MODULE_EFFECT2_TAG").GetString();
			if (moduleEffects.ContainsKey(string2) && moduleEffects[string2].onUsedAction != null)
			{
				moduleEffects[string2].onUsedAction(item);
			}
		}
		ComputeAllModuleTempStat(moduleGrid.childItems);
		ModuleHelper.ComputeModuleEffect(moduleGrid, null, machine);
		ModuleHelper.ApplyBasicModuleEffect(moduleGrid, null, machine);
	}

	public static void InitRandomEffect(GameItem gameItem, int count = -1)
	{
		bool flag = gameItem.IsGameItemType("NODE");
		if (!flag && !PlayerStore.Instance.skipIntro && StoreStation.Instance.dayCounter < 21)
		{
			return;
		}
		List<string> list = new List<string>(moduleEffects.Keys);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			string key = list[num];
			ModuleEffect moduleEffect = moduleEffects[key];
			if (flag && moduleEffect.effectType == ModuleEffect.ModuleEffectType.ModuleOnly)
			{
				list.RemoveAt(num);
			}
			else if (!flag && moduleEffect.effectType == ModuleEffect.ModuleEffectType.NodeOnly)
			{
				list.RemoveAt(num);
			}
		}
		if (count == -1)
		{
			count = RNG.GetRandomInt(0, 2);
		}
		if (flag)
		{
			count = 1;
		}
		for (int i = 0; i < count; i++)
		{
			if (list.Count == 0)
			{
				break;
			}
			int randomInt = RNG.GetRandomInt(0, list.Count - 1);
			string effectIdentifier = list[randomInt];
			list.RemoveAt(randomInt);
			AddEffectToModule(gameItem, effectIdentifier);
		}
	}
}

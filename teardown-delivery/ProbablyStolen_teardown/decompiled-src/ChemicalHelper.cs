using System;
using System.Collections.Generic;

public class ChemicalHelper
{
	public const int AMBIENT_TEMPERATURE = 26;

	public const int HEATING_PLATE_LOW = 100000;

	public const int HEATING_PLATE_HIGH = 400000;

	public const string CHEM_ITEM_TAG = "CHEM_ITEM_TAG";

	public const string BLENDABLE_TAG = "BLENDABLE_TAG";

	public const string SOLUBLE_TAG = "SOLUBLE_TAG";

	public const string CONTAINER_TEMP_TAG = "CONTAINER_TEMP_TAG";

	public const string CHEMICAL_MACHINE_UPGRADE_COUNT = "CHEMICAL_MACHINE_UPGRADE_COUNT";

	public const string HEATING_TAG = "HEATING_TAG";

	private static readonly Action<GameItem, GameInventory, SlotMarker> containerTempTick = delegate(GameItem container, GameInventory _, SlotMarker _)
	{
		HandleAmbientTempDrop(container);
		HandleTemperatureReaction(container);
	};

	public const double NOMORE_MIN_TEMP = 45.0;

	public const double NOMORE_FAST_TEMP = 50.0;

	public const double NOMORE_MAX_TEMP = 55.0;

	public const double CHLORINE_POTENCY = 5.1;

	private const double killThreshold = 70.0;

	private const double instantKill = 100.0;

	private const double killRatePerSecond = 0.2;

	public static void InitChemItem(GameItem item, bool isBlendable = false, bool isSoluble = false)
	{
		item.EnableTag("CHEM_ITEM_TAG");
		if (isBlendable)
		{
			item.EnableTag("BLENDABLE_TAG");
		}
		if (isSoluble)
		{
			item.EnableTag("SOLUBLE_TAG");
			item.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
			item.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
			item.mayThisTargetItemFunc = (GameItem s, GameItem t) => (t.IsTag("LIQUID_CONTAINER_TAG") && GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s)) ? true : false;
			item.canThisTargetItemFunc = (GameItem s, GameItem t) => (t.IsTag("LIQUID_CONTAINER_TAG") && GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s)) ? true : false;
			item.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
			{
				if (t.IsTag("LIQUID_CONTAINER_TAG"))
				{
					DissolveInto(item, t);
				}
			};
		}
		foreach (Liquid liquid in Liquid.Liquids)
		{
			item.ModifyTag(liquid.currentPart, delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
	}

	public static void SetChemContentML(GameItem item, string liquidID, double volume)
	{
		item.ModifyTag(Liquid.GetCurrentPartFromId(liquidID), delegate(TagState state)
		{
			state.Enable().SetInt((int)(volume * 1000.0));
		});
	}

	public static void SetChemContentUL(GameItem item, string liquidID, int volume)
	{
		item.ModifyTag(Liquid.GetCurrentPartFromId(liquidID), delegate(TagState state)
		{
			state.Enable().SetInt(volume);
		});
	}

	public static void AddChemContentUL(GameItem item, string liquidID, int volume)
	{
		item.ModifyTag(Liquid.GetCurrentPartFromId(liquidID), delegate(TagState state)
		{
			state.Enable().SetInt(state.GetInt() + volume);
		});
	}

	public static void BlendInto(GameItem chemicalItem, GameItem liquidContainer, int loss = 0)
	{
		int num = 10;
		double num2 = (double)Math.Max(0, Math.Min(100, loss)) / 100.0;
		Dictionary<Liquid, int> dictionary = new Dictionary<Liquid, int>();
		foreach (Liquid liquid in Liquid.Liquids)
		{
			int num3 = (int)Math.Round((double)chemicalItem.GetTagReadonly(liquid.currentPart).GetInt() * (1.0 - num2), MidpointRounding.AwayFromZero);
			if (num3 > 0)
			{
				dictionary[liquid] = num3;
			}
		}
		for (int i = 0; i < num; i++)
		{
			foreach (KeyValuePair<Liquid, int> item in dictionary)
			{
				int num4 = item.Value / num + ((i < item.Value % num) ? 1 : 0);
				if (num4 > 0)
				{
					WaterHelper.AddLiquid(liquidContainer, item.Key.identifier, num4);
				}
			}
		}
		WaterHelper.RemoveExcessLiquid(liquidContainer);
		chemicalItem.Destroy();
	}

	public static void DissolveInto(GameItem chemicalItem, GameItem liquidContainer)
	{
		int freeCapacity = WaterHelper.GetFreeCapacity(liquidContainer);
		int totalVolume = GetTotalVolume(chemicalItem);
		if (totalVolume <= freeCapacity)
		{
			foreach (Liquid liquid in Liquid.Liquids)
			{
				int amount = chemicalItem.GetTagReadonly(liquid.currentPart).GetInt();
				int currentAmount = liquidContainer.GetTagReadonly(liquid.currentPart).GetInt();
				liquidContainer.ModifyTag(liquid.currentPart, delegate(TagState state)
				{
					state.Enable().SetInt(currentAmount + amount);
				}, skipValidate: true);
			}
		}
		else
		{
			int volumeInPart = totalVolume - freeCapacity;
			WaterHelper.Remove(liquidContainer, volumeInPart);
			foreach (Liquid liquid2 in Liquid.Liquids)
			{
				int amount2 = chemicalItem.GetTagReadonly(liquid2.currentPart).GetInt();
				int currentAmount2 = liquidContainer.GetTagReadonly(liquid2.currentPart).GetInt();
				liquidContainer.ModifyTag(liquid2.currentPart, delegate(TagState state)
				{
					state.Enable().SetInt(currentAmount2 + amount2);
				}, skipValidate: true);
			}
		}
		chemicalItem.Destroy();
		WaterFeatureHelper.UpdateWaterFeature(liquidContainer, isExpose: false);
		SpriteHelper.UpdateWaterContainerSprite(liquidContainer);
	}

	private static double GetCooledTemp(double initialTemp, double ambientTemp, int volume, double timeSeconds, double coolingFactor = 0.05)
	{
		if (volume <= 0)
		{
			return initialTemp;
		}
		double num = coolingFactor / (Math.Sqrt(volume) + 1.0 + 10.0);
		return ambientTemp + (initialTemp - ambientTemp) * Math.Exp((0.0 - num) * timeSeconds);
	}

	public static void HandleAmbientTempDrop(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return;
		}
		if (container.IsTag("HEATING_TAG"))
		{
			container.DisableTag("HEATING_TAG", skipValidate: true);
			return;
		}
		double @double = container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
		double newTemp = GetCooledTemp(@double, 26.0, WaterHelper.GetTotalVolume(container), 125.0);
		container.ModifyTag("CONTAINER_TEMP_TAG", delegate(TagState state)
		{
			state.Enable().SetDouble(newTemp);
		}, skipValidate: true);
	}

	public static void InitContainerTempFunction(GameItem container)
	{
		container.onOneSecondItemFunc = containerTempTick;
	}

	public static int GetTotalVolume(GameItem chemicalItem)
	{
		if (!chemicalItem.IsTag("CHEM_ITEM_TAG"))
		{
			return 0;
		}
		int num = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num += chemicalItem.GetTagReadonly(liquid.currentPart).GetInt();
		}
		return num;
	}

	public static int RemoveVolume(GameItem chemicalItem, int percentToRemove)
	{
		if (!chemicalItem.IsTag("CHEM_ITEM_TAG"))
		{
			return 0;
		}
		int totalVolume = GetTotalVolume(chemicalItem);
		int num = (int)((long)totalVolume * (long)Math.Max(0, Math.Min(100, percentToRemove)) / 100);
		if (num <= 0)
		{
			return 0;
		}
		if (totalVolume <= 0)
		{
			return 0;
		}
		int[] liquidParts = WaterHelper.GetLiquidParts(chemicalItem);
		int[] array = WaterHelper.SplitProportionally(liquidParts, num);
		for (int i = 0; i < Liquid.Liquids.Count; i++)
		{
			if (array[i] > 0)
			{
				int newAmount = liquidParts[i] - array[i];
				chemicalItem.ModifyTag(Liquid.Liquids[i].currentPart, delegate(TagState state)
				{
					state.Enable().SetInt(newAmount);
				}, skipValidate: true);
			}
		}
		return num;
	}

	public static void HandleTemperatureReaction(GameItem container)
	{
		if (!WaterHelper.IsEmpty(container) && 0 + HandleMicrobeHeat(container) + HandleMicrobeAlcohol(container) + HandleOrganicWasteHeat(container) + HandleDreamCapMushReaction(container) + HandleNightmareCapMushReaction(container) + HandleCoffeeBeanExtractionSolvent(container) + HandleCoffeeBeanExtractionWater(container) + HandleNomoreReaction(container) + HandleDreamCapExtractDegradation(container) + HandleDreamCapMushDegradation(container) + HandleNightmareCapExtractDegradation(container) + HandleNightmareCapMushDegradation(container) + HandleOrganicWasteMineralization(container) + HandleAcidBaseMicrobeKill(container) + HandleStabilizingAgentSedimentation(container) + HandleProteinPrecipitation(container) + HandleLipobondAgentSedimentation(container) + HandleAcidBaseNeutralization(container) + HandleFantaHeat(container) + HandleMethylolHeatReaction(container) + HandleChlorine(container) + HandleBoilingReaction(container) > 0)
		{
			ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(container);
			WaterFeatureHelper.OnLiquidChanged(container, waterFeature.IsExposed());
		}
	}

	public static int HandleDreamCapExtractDegradation(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		double @double = container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
		int extractAmount = container.GetTagReadonly(Liquid.GetCurrentPartFromId("dream_cap_extract")).GetInt();
		if (extractAmount <= 0)
		{
			return 0;
		}
		if (@double < 60.0)
		{
			return 0;
		}
		double num = 0.0;
		if (@double >= 60.0 && @double < 70.0)
		{
			num = 0.1 * ((@double - 60.0) / 10.0);
		}
		else if (@double >= 70.0 && @double < 100.0)
		{
			num = 0.1 + 0.8 * ((@double - 70.0) / 30.0);
		}
		else if (@double >= 100.0)
		{
			num = 0.9 + 0.1 * Math.Min((@double - 100.0) / 20.0, 1.0);
		}
		int destroyedAmount = (int)((double)extractAmount * num);
		if (destroyedAmount > 0)
		{
			container.ModifyTag(Liquid.GetCurrentPartFromId("dream_cap_extract"), delegate(TagState state)
			{
				state.Enable().SetInt(extractAmount - destroyedAmount);
			}, skipValidate: true);
		}
		return destroyedAmount;
	}

	public static int HandleBoilingReaction(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		foreach (Liquid liquid in Liquid.Liquids)
		{
			if (liquid.isEvaporative && container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble() > liquid.boilingPoint)
			{
				int amountToRemove = (int)((double)container.GetTagReadonly(liquid.currentPart).GetInt() * 0.02);
				amountToRemove = Math.Max(amountToRemove, 100);
				int originalAmount = container.GetTagReadonly(liquid.currentPart).GetInt();
				if (amountToRemove > originalAmount)
				{
					amountToRemove = originalAmount;
				}
				container.ModifyTag(Liquid.GetCurrentPartFromId(liquid.identifier), delegate(TagState state)
				{
					state.Enable().SetInt(originalAmount - amountToRemove);
				}, skipValidate: true);
			}
		}
		return 1;
	}

	public static int HandleDreamCapMushDegradation(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		double @double = container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
		int mushAmount = container.GetTagReadonly(Liquid.GetCurrentPartFromId("dream_cap_mush")).GetInt();
		if (mushAmount <= 0)
		{
			return 0;
		}
		if (@double < 50.0)
		{
			return 0;
		}
		double num = 0.0;
		if (@double >= 50.0 && @double < 70.0)
		{
			num = 0.1 * ((@double - 50.0) / 10.0);
		}
		else if (@double >= 70.0 && @double < 100.0)
		{
			num = 0.1 + 0.8 * ((@double - 70.0) / 30.0);
		}
		else if (@double >= 100.0)
		{
			num = 0.9 + 0.1 * Math.Min((@double - 100.0) / 20.0, 1.0);
		}
		int degradedAmount = (int)((double)mushAmount * num);
		if (degradedAmount > 0)
		{
			container.ModifyTag(Liquid.GetCurrentPartFromId("dream_cap_mush"), delegate(TagState state)
			{
				state.Enable().SetInt(mushAmount - degradedAmount);
			}, skipValidate: true);
			int currentPlantMatter = container.GetTagReadonly(Liquid.GetCurrentPartFromId("plant_matter")).GetInt();
			container.ModifyTag(Liquid.GetCurrentPartFromId("plant_matter"), delegate(TagState state)
			{
				state.Enable().SetInt(currentPlantMatter + degradedAmount);
			}, skipValidate: true);
		}
		return degradedAmount;
	}

	public static int HandleCoffeeBeanExtractionSolvent(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		if (container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble() < 50.0)
		{
			return 0;
		}
		int mushPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("coffee_mush")).GetInt();
		int @int = container.GetTagReadonly(Liquid.GetCurrentPartFromId("alcohol")).GetInt();
		if (mushPart <= 0 || @int <= 0)
		{
			return 0;
		}
		int mushToProcess = Math.Max(1, (int)Math.Round((double)mushPart * 0.15));
		double num = (double)@int / (double)mushToProcess;
		double num2 = 0.75;
		double val;
		if (num < 5.0)
		{
			val = num2 * (num / 5.0);
			val = Math.Max(0.0, val);
		}
		else
		{
			val = num2;
		}
		int extractAmount = (int)Math.Round((double)mushToProcess * val);
		int plantMatterAmount = mushToProcess - extractAmount;
		container.ModifyTag(Liquid.GetCurrentPartFromId("coffee_mush"), delegate(TagState state)
		{
			state.Enable().SetInt(mushPart - mushToProcess);
		}, skipValidate: true);
		int currentExtract = container.GetTagReadonly(Liquid.GetCurrentPartFromId("caffeine")).GetInt();
		container.ModifyTag(Liquid.GetCurrentPartFromId("caffeine"), delegate(TagState state)
		{
			state.Enable().SetInt(currentExtract + extractAmount);
		}, skipValidate: true);
		int currentPlantMatter = container.GetTagReadonly(Liquid.GetCurrentPartFromId("plant_matter")).GetInt();
		container.ModifyTag(Liquid.GetCurrentPartFromId("plant_matter"), delegate(TagState state)
		{
			state.Enable().SetInt(currentPlantMatter + plantMatterAmount);
		}, skipValidate: true);
		return 1;
	}

	public static int HandleCoffeeBeanExtractionWater(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		if (container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble() < 80.0)
		{
			return 0;
		}
		int mushPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("coffee_mush")).GetInt();
		int @int = container.GetTagReadonly(Liquid.GetCurrentPartFromId("water")).GetInt();
		if (mushPart <= 0 || @int <= 0)
		{
			return 0;
		}
		int mushToProcess = Math.Max(1, (int)Math.Round((double)mushPart * 0.05));
		double num = (double)@int / (double)mushToProcess;
		double num2 = 0.125;
		double val;
		if (num < 5.0)
		{
			val = num2 * (num / 5.0);
			val = Math.Max(0.0, val);
		}
		else
		{
			val = num2;
		}
		int extractAmount = (int)Math.Round((double)mushToProcess * val);
		int plantMatterAmount = mushToProcess - extractAmount;
		container.ModifyTag(Liquid.GetCurrentPartFromId("coffee_mush"), delegate(TagState state)
		{
			state.Enable().SetInt(mushPart - mushToProcess);
		}, skipValidate: true);
		int currentExtract = container.GetTagReadonly(Liquid.GetCurrentPartFromId("caffeine")).GetInt();
		container.ModifyTag(Liquid.GetCurrentPartFromId("caffeine"), delegate(TagState state)
		{
			state.Enable().SetInt(currentExtract + extractAmount);
		}, skipValidate: true);
		int currentPlantMatter = container.GetTagReadonly(Liquid.GetCurrentPartFromId("plant_matter")).GetInt();
		container.ModifyTag(Liquid.GetCurrentPartFromId("plant_matter"), delegate(TagState state)
		{
			state.Enable().SetInt(currentPlantMatter + plantMatterAmount);
		}, skipValidate: true);
		return 1;
	}

	public static int HandleDreamCapMushReaction(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		if (container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble() < 40.0)
		{
			return 0;
		}
		int dreamCapMushPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("dream_cap_mush")).GetInt();
		int @int = container.GetTagReadonly(Liquid.GetCurrentPartFromId("alcohol")).GetInt();
		if (dreamCapMushPart <= 0 || @int <= 0)
		{
			return 0;
		}
		int mushToProcess = Math.Max(2000, (int)Math.Round((double)dreamCapMushPart * 0.1));
		mushToProcess = Math.Min(mushToProcess, dreamCapMushPart);
		double num = (double)@int / (double)dreamCapMushPart;
		double num2 = ((num >= 5.0) ? 0.5 : Math.Max(0.0, 0.5 * (num / 5.0)));
		int extractAmount = (int)Math.Round((double)mushToProcess * num2);
		int plantMatterAmount = mushToProcess - extractAmount;
		container.ModifyTag(Liquid.GetCurrentPartFromId("dream_cap_mush"), delegate(TagState state)
		{
			state.Enable().SetInt(dreamCapMushPart - mushToProcess);
		}, skipValidate: true);
		int currentExtract = container.GetTagReadonly(Liquid.GetCurrentPartFromId("dream_cap_extract")).GetInt();
		container.ModifyTag(Liquid.GetCurrentPartFromId("dream_cap_extract"), delegate(TagState state)
		{
			state.Enable().SetInt(currentExtract + extractAmount);
		}, skipValidate: true);
		int currentPlantMatter = container.GetTagReadonly(Liquid.GetCurrentPartFromId("plant_matter")).GetInt();
		container.ModifyTag(Liquid.GetCurrentPartFromId("plant_matter"), delegate(TagState state)
		{
			state.Enable().SetInt(currentPlantMatter + plantMatterAmount);
		}, skipValidate: true);
		return 1;
	}

	public static int HandleNightmareCapMushReaction(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		if (container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble() < 40.0)
		{
			return 0;
		}
		int nightmareCapMushPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("nightmare_cap_mush")).GetInt();
		int @int = container.GetTagReadonly(Liquid.GetCurrentPartFromId("alcohol")).GetInt();
		if (nightmareCapMushPart <= 0 || @int <= 0)
		{
			return 0;
		}
		int mushToProcess = Math.Max(2000, (int)Math.Round((double)nightmareCapMushPart * 0.1));
		mushToProcess = Math.Min(mushToProcess, nightmareCapMushPart);
		double num = (double)@int / (double)nightmareCapMushPart;
		double num2 = ((num >= 5.0) ? 0.5 : Math.Max(0.0, 0.5 * (num / 5.0)));
		int extractAmount = (int)Math.Round((double)mushToProcess * num2);
		int plantMatterAmount = mushToProcess - extractAmount;
		container.ModifyTag(Liquid.GetCurrentPartFromId("nightmare_cap_mush"), delegate(TagState state)
		{
			state.Enable().SetInt(nightmareCapMushPart - mushToProcess);
		}, skipValidate: true);
		int currentExtract = container.GetTagReadonly(Liquid.GetCurrentPartFromId("nightmare_extract")).GetInt();
		container.ModifyTag(Liquid.GetCurrentPartFromId("nightmare_extract"), delegate(TagState state)
		{
			state.Enable().SetInt(currentExtract + extractAmount);
		}, skipValidate: true);
		int currentPlantMatter = container.GetTagReadonly(Liquid.GetCurrentPartFromId("plant_matter")).GetInt();
		container.ModifyTag(Liquid.GetCurrentPartFromId("plant_matter"), delegate(TagState state)
		{
			state.Enable().SetInt(currentPlantMatter + plantMatterAmount);
		}, skipValidate: true);
		return 1;
	}

	public static int HandleNomoreReaction(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		double @double = container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
		if (@double < 45.0 || @double > 55.0)
		{
			return 0;
		}
		int extractPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("dream_cap_extract")).GetInt();
		int reducingAgentPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("reducing_agent")).GetInt();
		int @int = container.GetTagReadonly(Liquid.GetCurrentPartFromId("lipobond_agent")).GetInt();
		int int2 = container.GetTagReadonly(Liquid.GetCurrentPartFromId("alcohol")).GetInt();
		int nomorePart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("nomore")).GetInt();
		int debrisPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("debris")).GetInt();
		if (extractPart <= 0 || reducingAgentPart <= 0 || @int <= 0)
		{
			return 0;
		}
		if ((long)int2 * 10L > WaterHelper.GetTotalVolume(container))
		{
			return 0;
		}
		double num = ((@double >= 50.0) ? 0.1 : 0.05);
		double num2 = Math.Min(1.0, (double)@int * 4.0 / (double)extractPart);
		int num3 = Math.Min(extractPart, reducingAgentPart);
		int amountToReact = (int)Math.Round((double)num3 * num * num2);
		amountToReact = Math.Max(amountToReact, Math.Min(num3, 100));
		amountToReact = Math.Min(amountToReact, num3);
		int nomoreProduced = (int)Math.Round((double)amountToReact * 0.6);
		int debrisProduced = amountToReact - nomoreProduced;
		container.ModifyTag(Liquid.GetCurrentPartFromId("dream_cap_extract"), delegate(TagState state)
		{
			state.Enable().SetInt(extractPart - amountToReact);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("reducing_agent"), delegate(TagState state)
		{
			state.Enable().SetInt(reducingAgentPart - amountToReact);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("nomore"), delegate(TagState state)
		{
			state.Enable().SetInt(nomorePart + nomoreProduced);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("debris"), delegate(TagState state)
		{
			state.Enable().SetInt(debrisPart + debrisProduced);
		}, skipValidate: true);
		return 1;
	}

	public static int HandleNightmareCapExtractDegradation(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		double @double = container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
		int extractAmount = container.GetTagReadonly(Liquid.GetCurrentPartFromId("nightmare_extract")).GetInt();
		if (extractAmount <= 0)
		{
			return 0;
		}
		if (@double < 50.0)
		{
			return 0;
		}
		double num = 0.0;
		if (@double >= 50.0 && @double < 70.0)
		{
			num = 0.1 * ((@double - 50.0) / 2.0);
		}
		else if (@double >= 70.0 && @double < 100.0)
		{
			num = 0.1 + 0.8 * ((@double - 70.0) / 30.0);
		}
		else if (@double >= 100.0)
		{
			num = 0.9 + 0.1 * Math.Min((@double - 100.0) / 20.0, 1.0);
		}
		int destroyedAmount = (int)((double)extractAmount * num);
		if (destroyedAmount > 0)
		{
			container.ModifyTag(Liquid.GetCurrentPartFromId("nightmare_extract"), delegate(TagState state)
			{
				state.Enable().SetInt(extractAmount - destroyedAmount);
			}, skipValidate: true);
		}
		return destroyedAmount;
	}

	public static int HandleNightmareCapMushDegradation(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		double @double = container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
		int mushAmount = container.GetTagReadonly(Liquid.GetCurrentPartFromId("nightmare_cap_mush")).GetInt();
		if (mushAmount <= 0)
		{
			return 0;
		}
		if (@double < 50.0)
		{
			return 0;
		}
		double num = 0.0;
		if (@double >= 50.0 && @double < 70.0)
		{
			num = 0.1 * ((@double - 50.0) / 10.0);
		}
		else if (@double >= 70.0 && @double < 100.0)
		{
			num = 0.1 + 0.8 * ((@double - 70.0) / 30.0);
		}
		else if (@double >= 100.0)
		{
			num = 0.9 + 0.1 * Math.Min((@double - 100.0) / 20.0, 1.0);
		}
		int degradedAmount = (int)((double)mushAmount * num);
		if (degradedAmount > 0)
		{
			container.ModifyTag(Liquid.GetCurrentPartFromId("nightmare_cap_mush"), delegate(TagState state)
			{
				state.Enable().SetInt(mushAmount - degradedAmount);
			}, skipValidate: true);
			int currentPlantMatter = container.GetTagReadonly(Liquid.GetCurrentPartFromId("plant_matter")).GetInt();
			container.ModifyTag(Liquid.GetCurrentPartFromId("plant_matter"), delegate(TagState state)
			{
				state.Enable().SetInt(currentPlantMatter + degradedAmount);
			}, skipValidate: true);
		}
		return degradedAmount;
	}

	public static int HandleChlorine(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		int @int = container.GetTagReadonly(Liquid.GetCurrentPartFromId("chlorine")).GetInt();
		int microbePart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("microbe")).GetInt();
		int int2 = container.GetTagReadonly(Liquid.GetCurrentPartFromId("chemical_contaminant")).GetInt();
		int int3 = container.GetTagReadonly(Liquid.GetCurrentPartFromId("water")).GetInt();
		int totalVolume = WaterHelper.GetTotalVolume(container);
		if (@int <= 0 || int3 <= 0)
		{
			return 0;
		}
		int num = Math.Min(@int, 15000);
		long num2 = (long)((double)num * 5.1);
		int num3 = (int)((float)totalVolume * 0.007f);
		int num4 = (int)((int2 > num3) ? Math.Min(int2 - num3, num2) : 0);
		num2 -= num4;
		int newChemicals = int2 - num4;
		int microbesDestroyed = (int)Math.Min(microbePart, num2);
		int chlorineAfterReaction = @int - num;
		int selfDegraded = Math.Min(chlorineAfterReaction, Math.Max(1000, (int)((double)chlorineAfterReaction * 0.2)));
		container.ModifyTag(Liquid.GetCurrentPartFromId("chlorine"), delegate(TagState state)
		{
			state.Enable().SetInt(chlorineAfterReaction - selfDegraded);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("microbe"), delegate(TagState state)
		{
			state.Enable().SetInt(microbePart - microbesDestroyed);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("chemical_contaminant"), delegate(TagState state)
		{
			state.Enable().SetInt(newChemicals);
		}, skipValidate: true);
		return 1;
	}

	public static int HandleAcidBaseNeutralization(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		int acidPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("acid")).GetInt();
		int basePart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("base")).GetInt();
		if (acidPart <= 0 || basePart <= 0)
		{
			return 0;
		}
		int num = Math.Min(acidPart, basePart);
		int amountToNeutralize = Math.Min(num, Math.Max(1000, (int)((double)num * 0.1)));
		container.ModifyTag(Liquid.GetCurrentPartFromId("acid"), delegate(TagState state)
		{
			state.Enable().SetInt(acidPart - amountToNeutralize);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("base"), delegate(TagState state)
		{
			state.Enable().SetInt(basePart - amountToNeutralize);
		}, skipValidate: true);
		return 1;
	}

	public static int HandleOrganicWasteMineralization(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		int acidPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("acid")).GetInt();
		int basePart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("base")).GetInt();
		int organicWastePart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("organic_waste")).GetInt();
		int mineralPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("mineral")).GetInt();
		if (organicWastePart <= 0)
		{
			return 0;
		}
		int num = acidPart + basePart;
		if (num <= 0)
		{
			return 0;
		}
		int organicWasteToProcess = (int)Math.Min(organicWastePart, (long)num * 40L);
		if (organicWasteToProcess <= 0)
		{
			return 0;
		}
		int num2 = (organicWasteToProcess + 39) / 40;
		int mineralProduced = organicWasteToProcess / 10;
		int acidConsumed = Math.Min(acidPart, num2);
		int baseConsumed = Math.Min(basePart, num2 - acidConsumed);
		container.ModifyTag(Liquid.GetCurrentPartFromId("organic_waste"), delegate(TagState state)
		{
			state.Enable().SetInt(organicWastePart - organicWasteToProcess);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("mineral"), delegate(TagState state)
		{
			state.Enable().SetInt(mineralPart + mineralProduced);
		}, skipValidate: true);
		if (acidConsumed > 0)
		{
			container.ModifyTag(Liquid.GetCurrentPartFromId("acid"), delegate(TagState state)
			{
				state.Enable().SetInt(acidPart - acidConsumed);
			}, skipValidate: true);
		}
		if (baseConsumed > 0)
		{
			container.ModifyTag(Liquid.GetCurrentPartFromId("base"), delegate(TagState state)
			{
				state.Enable().SetInt(basePart - baseConsumed);
			}, skipValidate: true);
		}
		return 1;
	}

	public static int HandleAcidBaseMicrobeKill(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		int acidPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("acid")).GetInt();
		int basePart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("base")).GetInt();
		int microbePart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("microbe")).GetInt();
		if (microbePart <= 0)
		{
			return 0;
		}
		int num = acidPart + basePart;
		if (num <= 0)
		{
			return 0;
		}
		int microbeToKill = (int)Math.Min(microbePart, (long)num * 20L);
		if (microbeToKill <= 0)
		{
			return 0;
		}
		int num2 = (microbeToKill + 99) / 100;
		int acidConsumed = Math.Min(acidPart, num2);
		int baseConsumed = Math.Min(basePart, num2 - acidConsumed);
		container.ModifyTag(Liquid.GetCurrentPartFromId("microbe"), delegate(TagState state)
		{
			state.Enable().SetInt(microbePart - microbeToKill);
		}, skipValidate: true);
		if (acidConsumed > 0)
		{
			container.ModifyTag(Liquid.GetCurrentPartFromId("acid"), delegate(TagState state)
			{
				state.Enable().SetInt(acidPart - acidConsumed);
			}, skipValidate: true);
		}
		if (baseConsumed > 0)
		{
			container.ModifyTag(Liquid.GetCurrentPartFromId("base"), delegate(TagState state)
			{
				state.Enable().SetInt(basePart - baseConsumed);
			}, skipValidate: true);
		}
		return 1;
	}

	public static int HandleStabilizingAgentSedimentation(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		int acidPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("acid")).GetInt();
		int basePart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("base")).GetInt();
		int stabilizingAgentPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("stabilizing_agent")).GetInt();
		int debrisPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("debris")).GetInt();
		if (stabilizingAgentPart <= 0)
		{
			return 0;
		}
		if (acidPart <= 0 && basePart <= 0)
		{
			return 0;
		}
		int num = Math.Min(acidPart + basePart, stabilizingAgentPart);
		int amountToNeutralize = Math.Min(num, Math.Max(1000, (int)((double)num * 0.2)));
		container.ModifyTag(Liquid.GetCurrentPartFromId("stabilizing_agent"), delegate(TagState state)
		{
			state.Enable().SetInt(stabilizingAgentPart - amountToNeutralize);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("debris"), delegate(TagState state)
		{
			state.Enable().SetInt(debrisPart + amountToNeutralize);
		}, skipValidate: true);
		int acidToRemove = Math.Min(acidPart, Math.Max(100, (int)((double)acidPart * 0.05)));
		int baseToRemove = Math.Min(basePart, Math.Max(100, (int)((double)basePart * 0.05)));
		container.ModifyTag(Liquid.GetCurrentPartFromId("acid"), delegate(TagState state)
		{
			state.Enable().SetInt(Math.Max(0, acidPart - acidToRemove));
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("base"), delegate(TagState state)
		{
			state.Enable().SetInt(Math.Max(0, basePart - baseToRemove));
		}, skipValidate: true);
		return 1;
	}

	public static int HandleProteinPrecipitation(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		int acidPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("acid")).GetInt();
		int basePart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("base")).GetInt();
		int proteinPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("protein")).GetInt();
		int debrisPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("debris")).GetInt();
		if (proteinPart <= 0)
		{
			return 0;
		}
		int num = Math.Max(0, acidPart - basePart);
		int num2 = Math.Max(0, basePart - acidPart);
		int num3 = num + num2;
		if (num3 <= 0)
		{
			return 0;
		}
		int num4 = Math.Min(num3, proteinPart);
		int amountToDenature = Math.Max(1, (int)((double)num4 * 0.2));
		amountToDenature = Math.Min(amountToDenature, num4);
		int acidToRemove = Math.Min(num, amountToDenature);
		int baseToRemove = Math.Min(num2, amountToDenature - acidToRemove);
		container.ModifyTag(Liquid.GetCurrentPartFromId("protein"), delegate(TagState state)
		{
			state.Enable().SetInt(proteinPart - amountToDenature);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("debris"), delegate(TagState state)
		{
			state.Enable().SetInt(debrisPart + amountToDenature);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("acid"), delegate(TagState state)
		{
			state.Enable().SetInt(Math.Max(0, acidPart - acidToRemove));
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("base"), delegate(TagState state)
		{
			state.Enable().SetInt(Math.Max(0, basePart - baseToRemove));
		}, skipValidate: true);
		return 1;
	}

	public static int HandleLipobondAgentSedimentation(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		int lipobondAgentPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("lipobond_agent")).GetInt();
		int oilPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("fat")).GetInt();
		int debrisPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("debris")).GetInt();
		if (lipobondAgentPart <= 0)
		{
			return 0;
		}
		if (oilPart <= 0)
		{
			return 0;
		}
		int num = (int)Math.Min(oilPart, (long)lipobondAgentPart * 8L);
		int oilToSediment = Math.Min(num, Math.Max(1000, (int)((double)num * 0.2)));
		int lipobondConsumed = oilToSediment / 8;
		container.ModifyTag(Liquid.GetCurrentPartFromId("fat"), delegate(TagState state)
		{
			state.Enable().SetInt(oilPart - oilToSediment);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("lipobond_agent"), delegate(TagState state)
		{
			state.Enable().SetInt(lipobondAgentPart - lipobondConsumed);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("debris"), delegate(TagState state)
		{
			state.Enable().SetInt(debrisPart + oilToSediment + lipobondConsumed);
		}, skipValidate: true);
		return 1;
	}

	public static int HandleMethylolHeatReaction(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		int methylolPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("methylol")).GetInt();
		int reducingAgentPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("reducing_agent")).GetInt();
		int fantaPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("fanta")).GetInt();
		int @int = container.GetTagReadonly(Liquid.GetCurrentPartFromId("stabilizing_agent")).GetInt();
		double @double = container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
		if (methylolPart <= 0)
		{
			return 0;
		}
		if (methylolPart <= @int)
		{
			return 0;
		}
		int num = Math.Min(methylolPart - @int, reducingAgentPart);
		double num2 = 0.0;
		if (@double >= 110.0 && @double < 120.0)
		{
			num2 = 0.025;
		}
		else
		{
			if (!(@double >= 120.0) || !(@double < 130.0))
			{
				return 0;
			}
			num2 = 0.15;
		}
		int amountToReact = Math.Min(num, Math.Max(1000, (int)((double)num * num2)));
		container.ModifyTag(Liquid.GetCurrentPartFromId("methylol"), delegate(TagState state)
		{
			state.Enable().SetInt(methylolPart - amountToReact);
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("reducing_agent"), delegate(TagState state)
		{
			state.Enable().SetInt(Math.Max(0, reducingAgentPart - (int)Math.Round((double)amountToReact * 0.75)));
		}, skipValidate: true);
		container.ModifyTag(Liquid.GetCurrentPartFromId("fanta"), delegate(TagState state)
		{
			state.Enable().SetInt(fantaPart + (int)Math.Round((double)amountToReact * 0.1));
		}, skipValidate: true);
		return 1;
	}

	public static int HandleFantaHeat(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		double @double = container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
		int fantaPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("fanta")).GetInt();
		if (fantaPart <= 0)
		{
			return 0;
		}
		if (@double >= 130.0)
		{
			fantaPart = (int)Math.Max(0.0, (double)fantaPart * 0.98);
		}
		container.ModifyTag(Liquid.GetCurrentPartFromId("fanta"), delegate(TagState state)
		{
			state.Enable().SetInt(fantaPart);
		}, skipValidate: true);
		return 1;
	}

	public static int HandleMicrobeHeat(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		double @double = container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
		int microbePart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("microbe")).GetInt();
		if (microbePart <= 0)
		{
			return 0;
		}
		if (@double >= 100.0)
		{
			microbePart = 0;
		}
		else if (@double >= 70.0)
		{
			microbePart = (int)Math.Max(0.0, (double)microbePart * 0.8);
		}
		container.ModifyTag(Liquid.GetCurrentPartFromId("microbe"), delegate(TagState state)
		{
			state.Enable().SetInt(microbePart);
		}, skipValidate: true);
		return 1;
	}

	public static int HandleMicrobeAlcohol(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		int @int = container.GetTagReadonly(Liquid.GetCurrentPartFromId("alcohol")).GetInt();
		int microbePart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("microbe")).GetInt();
		int totalVolume = WaterHelper.GetTotalVolume(container);
		if (@int <= 0 || microbePart <= 0)
		{
			return 0;
		}
		if ((double)@int < (double)totalVolume * 0.4)
		{
			return 0;
		}
		int num = Math.Min(microbePart, @int);
		microbePart -= num;
		container.ModifyTag(Liquid.GetCurrentPartFromId("microbe"), delegate(TagState state)
		{
			state.Enable().SetInt(microbePart);
		}, skipValidate: true);
		return 1;
	}

	public static int HandleOrganicWasteHeat(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			return 0;
		}
		double @double = container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
		int organicWastePart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("organic_waste")).GetInt();
		if (organicWastePart <= 0)
		{
			return 0;
		}
		if (@double >= 80.0)
		{
			int val = ((!(@double >= 100.0)) ? ((int)Math.Ceiling((double)organicWastePart * 0.15)) : organicWastePart);
			val = Math.Min(val, organicWastePart);
			int num = (int)Math.Round((double)val * 0.33);
			organicWastePart -= val;
			int physicalContaminantPart = container.GetTagReadonly(Liquid.GetCurrentPartFromId("physical_contaminant")).GetInt();
			physicalContaminantPart += num;
			container.ModifyTag(Liquid.GetCurrentPartFromId("organic_waste"), delegate(TagState state)
			{
				state.Enable().SetInt(organicWastePart);
			}, skipValidate: true);
			container.ModifyTag(Liquid.GetCurrentPartFromId("physical_contaminant"), delegate(TagState state)
			{
				state.Enable().SetInt(physicalContaminantPart);
			}, skipValidate: true);
		}
		return 1;
	}

	private static double MixTemperature(int receiverVolume, double receiverTemp, int incomingVolume, double incomingTemp)
	{
		long num = (long)receiverVolume + (long)incomingVolume;
		if (num == 0L)
		{
			return 0.0;
		}
		return ((double)receiverVolume * receiverTemp + (double)incomingVolume * incomingTemp) / (double)num;
	}

	public static void HandleReceiveLiquidTemp(GameItem container, int volume, double sourceTemp = 26.0)
	{
		if (volume > 0)
		{
			int totalVolume = WaterHelper.GetTotalVolume(container);
			double @double = container.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
			double newTemp = MixTemperature(totalVolume, @double, volume, sourceTemp);
			container.ModifyTag("CONTAINER_TEMP_TAG", delegate(TagState state)
			{
				state.Enable().SetDouble(newTemp);
			}, skipValidate: true);
		}
	}

	public static void HandleReceiveLiquidTemp(GameItem container, int volume, GameItem sourceContainer)
	{
		double @double = sourceContainer.GetTagReadonly("CONTAINER_TEMP_TAG").GetDouble();
		HandleReceiveLiquidTemp(container, volume, @double);
	}

	public static void HandleEmptyContainerTemp(GameItem container)
	{
		if (WaterHelper.IsEmpty(container))
		{
			container.ModifyTag("CONTAINER_TEMP_TAG", delegate(TagState state)
			{
				state.Enable().SetDouble(24.0);
			}, skipValidate: true);
		}
	}

	public static void InitContainerTemp(GameItem container)
	{
		if (!container.IsTag("CONTAINER_TEMP_TAG"))
		{
			container.ModifyTag("CONTAINER_TEMP_TAG", delegate(TagState state)
			{
				state.Enable().SetDouble(24.0);
			}, skipValidate: true);
		}
	}

	public static double ConvertCtoK(double celsius)
	{
		return celsius + 273.15;
	}

	public static void TransferLiquidToChem(GameItem container, GameItem chemicalItem, int volumeInPart)
	{
		List<KeyValuePair<string, int>> liquidList = WaterHelper.RemoveAndReturn(container, volumeInPart);
		AddLiquidToChemFromList(chemicalItem, liquidList);
	}

	public static void AddLiquidToChemFromList(GameItem chemicalItem, List<KeyValuePair<string, int>> liquidList)
	{
		foreach (KeyValuePair<string, int> liquid in liquidList)
		{
			chemicalItem.ModifyTag(Liquid.GetCurrentPartFromId(liquid.Key), delegate(TagState state)
			{
				state.Enable().SetInt(liquid.Value);
			});
		}
	}

	public static void RemoveAllLiquid(GameItem chemicalItem)
	{
		foreach (Liquid liquid in Liquid.Liquids)
		{
			chemicalItem.ModifyTag(liquid.currentPart, delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
	}

	public static void RunNightlyChemicalSimulation()
	{
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem allItem in EmporiumEntry.Instance.GetAllItems())
		{
			if (allItem != null && allItem.onOneSecondItemFunc != null && (!(allItem.onOneSecondItemFunc == containerTempTick) || !WaterHelper.IsEmpty(allItem)))
			{
				list.Add(allItem);
			}
		}
		for (int i = 0; i < 360; i++)
		{
			foreach (GameItem item in list)
			{
				if (item.onOneSecondItemFunc != null && (!(item is GameItemElement gameItemElement) || !gameItemElement.IsDestroyed()))
				{
					item.onOneSecondItemFunc(item, item.parentInventory, null);
				}
			}
		}
	}
}

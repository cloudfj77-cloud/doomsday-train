using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MachineFeedDispenser
{
	public const string FEED_DISPENSER_TAG = "FEED_DISPENSER_TAG";

	public const string STORED_KCAL_TAG = "STORED_KCAL_TAG";

	public const string STORED_PREFIX = "STORED_";

	public const string STORED_OTHER_KEY = "STORED_OTHER";

	public const string STORED_MEAT_KEY = "STORED_MEAT";

	public const int MAX_KCAL = 40000;

	public const float TOOLTIP_WIDTH_MULT = 1.5f;

	public const string STORED_PELLET_KEY = "STORED_GUTTERFLOW_PELLET";

	public const string STORED_INDUSTRIAL_FEED_KEY = "STORED_INDUSTRIAL_FISH_FEED";

	public const string AUTO_BLEND_BOOL = "FEED_DISPENSER_AUTO_BLEND_BOOL";

	public const string TARGET_PERCENT_INT = "FEED_DISPENSER_TARGET_PERCENT_INT";

	public const int TARGET_PERCENT_DEFAULT = 85;

	public const int TARGET_PERCENT_MIN = 0;

	public const int TARGET_PERCENT_MAX = 100;

	public const int TARGET_PERCENT_STEP = 5;

	private static bool blending;

	private static readonly Dictionary<GameItem, (GameGridInventory grid, bool blended)> pendingAutoBlend = new Dictionary<GameItem, (GameGridInventory, bool)>();

	private static bool autoBlendPassQueued;

	private static int autoBlendPassFrame;

	public static string GetStoredTag(string speciesID)
	{
		return "STORED_" + speciesID;
	}

	public static string GetPrionStoredTag(string speciesID)
	{
		return "STORED_PRION_" + speciesID;
	}

	public static GameItem CreateFeedDispenser()
	{
		GameGridInventory storageGrid = new GameGridInventory(6, 4);
		MachineryButton machineryButton = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_blender_btn_start"));
		MachineryButton machineryButton2 = new MachineryButton(LocHelper.GetLocalizedMechanic("livestock_feed_dispenser_empty"));
		MachineryText targetCaption = new MachineryText(TargetCaption(85), 6, HorizontalAlignmentOptions.Center, uppercase: false);
		MachinerySlider targetSlider = new MachinerySlider(85f).SetRange(0f, 100f, 5f);
		PixelWindow contentWindow = MachineHelper.CreateMachineryWindow(true, MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_blender_label_container"), storageGrid, "storage"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_blender_label_control"), MachineryStack.Vertical(machineryButton, machineryButton2, targetCaption, targetSlider), "control"));
		storageGrid.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_husbandry", "animal_feeder");
		machine.name = LocHelper.GetLocalizedItem("livestock_feed_dispenser_name");
		machine.shortDescription = LocHelper.GetLocalizedItem("livestock_feed_dispenser_desc");
		machine.unitValue = 300L;
		machine.EnableTag("FEED_DISPENSER_TAG");
		machine.EnableTag("ALLOWED_IN_CAGE_TAG");
		machine.ModifyTag("STORED_KCAL_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		machine.ModifyTag("FEED_DISPENSER_TARGET_PERCENT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(85);
		});
		machine.SetGameItemType("MACHINE");
		targetSlider.onChanged = delegate(float value)
		{
			int percent = Mathf.RoundToInt(value);
			machine.ModifyTag("FEED_DISPENSER_TARGET_PERCENT_INT", delegate(TagState state)
			{
				state.Enable().SetInt(percent);
			});
			targetCaption.SetText(TargetCaption(percent));
		};
		GameItem gameItem = machine;
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			int targetPercent = GetTargetPercent(loaded);
			targetSlider.SetInt(targetPercent);
			targetCaption.SetText(TargetCaption(targetPercent));
		});
		GameItem gameItem2 = machine;
		gameItem2.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem2.onLoaded, (Action<GameItem>)delegate(GameItem item)
		{
			item.EnableTag("ALLOWED_IN_CAGE_TAG");
		});
		AudioHelper.InitMetalMachine(machine);
		storageGrid.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(machine, item) && (item.IsTag("CALORIE_VALUE_TAG") || item.IsTag("ANIMAL_ITEM_TAG") || IsBlendableFish(item));
		GameGridInventory gameGridInventory = storageGrid;
		gameGridInventory.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameGridInventory.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem added, GameInventory _, SlotMarker _)
		{
			OnItemInserted(machine, storageGrid, added);
		});
		machineryButton.SetCallback(delegate
		{
			Blend(machine, storageGrid);
		});
		machine.ModifyTag("FEED_DISPENSER_AUTO_BLEND_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(val: false);
		});
		machine.mayToggleSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		machine.canToggleSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		machine.onToggleSlotItemFunc = delegate(GameItem s, GameInventory _, SlotMarker _)
		{
			bool on = !IsAutoBlendOn(s);
			s.ModifyTag("FEED_DISPENSER_AUTO_BLEND_BOOL", delegate(TagState state)
			{
				state.Enable().SetBool(on);
			});
			AudioManager.Instance.Play3("click_vintage");
			if (on)
			{
				Blend(machine, storageGrid);
			}
		};
		machineryButton2.SetCallback(delegate
		{
			int @int = machine.GetTagReadonly("STORED_KCAL_TAG").GetInt();
			if (@int > 0)
			{
				GenericConfirmationUIManager.Instance.OpenPanel(LocHelper.GetLocalizedMechanic("mech_feed_dispenser_empty_confirm", @int), LocHelper.GetLocalizedMainMenu("yes"), LocHelper.GetLocalizedMainMenu("no"), delegate
				{
					GenericConfirmationUIManager.Instance.ClosePanel();
					EmptyStorage(machine);
				}, delegate
				{
					GenericConfirmationUIManager.Instance.ClosePanel();
				});
			}
		});
		return machine;
	}

	public static GameGridInventory GetStorageGrid(GameItem dispenser)
	{
		return MachineHelper.GetBodyStorage(dispenser, 0) as GameGridInventory;
	}

	private static bool IsBlendableFish(GameItem item)
	{
		return AquacultureHelper.IsFish(item);
	}

	public static bool IsAutoBlendOn(GameItem dispenser)
	{
		return dispenser.GetTagReadonly("FEED_DISPENSER_AUTO_BLEND_BOOL").GetBool();
	}

	public static int GetTargetPercent(GameItem dispenser)
	{
		if (!dispenser.IsTag("FEED_DISPENSER_TARGET_PERCENT_INT"))
		{
			return 85;
		}
		return Mathf.Clamp(dispenser.GetTagReadonly("FEED_DISPENSER_TARGET_PERCENT_INT").GetInt(), 0, 100);
	}

	public static float GetTargetRatio(GameItem dispenser)
	{
		return (float)GetTargetPercent(dispenser) / 100f;
	}

	private static string TargetCaption(int percent)
	{
		return LocHelper.GetLocalizedMechanicOrDefault("mech_feed_dispenser_feed_up_to", "Feed up to: {0}% hunger", percent);
	}

	private static void OnItemInserted(GameItem machine, GameGridInventory storageGrid, GameItem added)
	{
		if (!blending && !SaveManager.IsDecoding && IsAutoBlendOn(machine))
		{
			bool flag = BlendContents(machine, storageGrid, added);
			pendingAutoBlend.TryGetValue(machine, out (GameGridInventory, bool) value);
			pendingAutoBlend[machine] = (storageGrid, value.Item2 || flag);
			if ((!autoBlendPassQueued || Time.frameCount - autoBlendPassFrame > 1) && !(OverlayHandler.current == null))
			{
				autoBlendPassQueued = true;
				autoBlendPassFrame = Time.frameCount;
				OverlayHandler.current.StartCoroutine(RunPendingAutoBlends());
			}
		}
	}

	private static IEnumerator RunPendingAutoBlends()
	{
		yield return null;
		autoBlendPassQueued = false;
		List<KeyValuePair<GameItem, (GameGridInventory grid, bool blended)>> list = pendingAutoBlend.ToList();
		pendingAutoBlend.Clear();
		foreach (KeyValuePair<GameItem, (GameGridInventory, bool)> item in list)
		{
			GameItem key = item.Key;
			if (!(key is GameItemElement gameItemElement) || !gameItemElement.IsDestroyed())
			{
				bool flag = item.Value.Item2;
				if (IsAutoBlendOn(key) && BlendContents(key, item.Value.Item1, null))
				{
					flag = true;
				}
				if (flag)
				{
					AudioManager.Instance.Play3("blender");
				}
			}
		}
	}

	public static void Blend(GameItem machine, GameGridInventory storageGrid)
	{
		if (BlendContents(machine, storageGrid, null))
		{
			AudioManager.Instance.Play3("blender");
		}
	}

	private static bool BlendContents(GameItem machine, GameGridInventory storageGrid, GameItem except)
	{
		if (machine == null || storageGrid == null || blending)
		{
			return false;
		}
		blending = true;
		try
		{
			return BlendContentsUnguarded(machine, storageGrid, except);
		}
		finally
		{
			blending = false;
		}
	}

	private static bool BlendContentsUnguarded(GameItem machine, GameGridInventory storageGrid, GameItem except)
	{
		bool anyBlended = false;
		foreach (GameItem item in storageGrid.childItems.ToList())
		{
			if (item != except && item.IsTag("ANIMAL_ITEM_TAG"))
			{
				if (!item.IsTag("ANIMAL_IS_DEAD_TAG"))
				{
					HusbandryHelper.OnAnimalDead(item);
				}
				HusbandryHelper.OnButcher(item, BlendOrStore);
			}
		}
		foreach (GameItem item2 in storageGrid.childItems.Where(AquacultureHelper.IsFish).ToList())
		{
			if (item2 != except)
			{
				if (!AquacultureHelper.IsDead(item2))
				{
					AquacultureHelper.OnFishDead(item2);
				}
				AquacultureHelper.OnFillet(item2, BlendOrStore);
			}
		}
		foreach (GameItem item3 in storageGrid.childItems.ToList())
		{
			if (item3 != except && TryBlendItem(machine, item3))
			{
				anyBlended = true;
			}
		}
		return anyBlended;
		void BlendOrStore(GameItem cut)
		{
			if (TryBlendItem(machine, cut))
			{
				anyBlended = true;
			}
			else
			{
				GraphUtils.EmporiumTryAdd(storageGrid, cut);
			}
		}
	}

	public static bool TryBlendItem(GameItem machine, GameItem foodItem)
	{
		if (!foodItem.IsTag("CALORIE_VALUE_TAG"))
		{
			return false;
		}
		int currentKcal = machine.GetTagReadonly("STORED_KCAL_TAG").GetInt();
		int calories = foodItem.GetTagReadonly("CALORIE_VALUE_TAG").GetInt();
		if (currentKcal + calories > 40000)
		{
			return false;
		}
		string @string = foodItem.GetTagReadonly("MEAT_TYPE_TAG").GetString();
		string tagName = ((!string.IsNullOrEmpty(@string) && AnimalSpecies.All.ContainsKey(@string)) ? GetStoredTag(@string) : ((!FoodItemHelper.IsMeatFood(foodItem)) ? "STORED_OTHER" : "STORED_MEAT"));
		if (GutterflowPelletHelper.IsPellet(foodItem))
		{
			tagName = "STORED_GUTTERFLOW_PELLET";
		}
		if (IndustrialFishFeedHelper.IsFeed(foodItem))
		{
			tagName = "STORED_INDUSTRIAL_FISH_FEED";
		}
		int bucketCurrent = machine.GetTagReadonly(tagName).GetInt();
		machine.ModifyTag(tagName, delegate(TagState state)
		{
			state.Enable().SetInt(bucketCurrent + calories);
		});
		if (foodItem.IsTag("ANIMAL_PRION_TAG") && !string.IsNullOrEmpty(@string) && AnimalSpecies.All.ContainsKey(@string))
		{
			machine.EnableTag(GetPrionStoredTag(@string));
		}
		machine.ModifyTag("STORED_KCAL_TAG", delegate(TagState state)
		{
			state.SetInt(currentKcal + calories);
		});
		foodItem.Destroy();
		return true;
	}

	public static void EmptyStorage(GameItem machine)
	{
		machine.ModifyTag("STORED_KCAL_TAG", delegate(TagState state)
		{
			state.SetInt(0);
		});
		machine.ModifyTag("STORED_OTHER", delegate(TagState state)
		{
			state.SetInt(0);
		});
		machine.ModifyTag("STORED_MEAT", delegate(TagState state)
		{
			state.SetInt(0);
		});
		machine.ModifyTag("STORED_GUTTERFLOW_PELLET", delegate(TagState state)
		{
			state.SetInt(0);
		});
		machine.ModifyTag("STORED_INDUSTRIAL_FISH_FEED", delegate(TagState state)
		{
			state.SetInt(0);
		});
		foreach (string key in AnimalSpecies.All.Keys)
		{
			machine.ModifyTag(GetStoredTag(key), delegate(TagState state)
			{
				state.SetInt(0);
			});
			machine.DisableTag(GetPrionStoredTag(key));
		}
	}

	public static int GetPelletKcal(GameItem dispenser)
	{
		return dispenser.GetTagReadonly("STORED_GUTTERFLOW_PELLET").GetInt();
	}

	public static int GetIndustrialFeedKcal(GameItem dispenser)
	{
		return dispenser.GetTagReadonly("STORED_INDUSTRIAL_FISH_FEED").GetInt();
	}

	public static int GetFishFeedKcal(GameItem dispenser)
	{
		return GetPelletKcal(dispenser) + GetIndustrialFeedKcal(dispenser);
	}

	public static int GetAnimalKcal(GameItem dispenser)
	{
		return Mathf.Max(0, dispenser.GetTagReadonly("STORED_KCAL_TAG").GetInt() - GetFishFeedKcal(dispenser));
	}

	public static int GetMeatKcal(GameItem dispenser)
	{
		int num = dispenser.GetTagReadonly("STORED_MEAT").GetInt();
		foreach (string key in AnimalSpecies.All.Keys)
		{
			num += dispenser.GetTagReadonly(GetStoredTag(key)).GetInt();
		}
		return num;
	}

	public static int GetAvailableKcal(GameItem dispenser, bool allowPellet, bool allowMeat, bool allowOther)
	{
		int num = 0;
		if (allowPellet)
		{
			num += GetFishFeedKcal(dispenser);
		}
		if (allowMeat)
		{
			num += GetMeatKcal(dispenser);
		}
		if (allowOther)
		{
			num += dispenser.GetTagReadonly("STORED_OTHER").GetInt();
		}
		return num;
	}

	public static int DrawKcal(GameItem dispenser, int amount)
	{
		int pelletDrawn;
		return DrawKcal(dispenser, amount, allowPellet: false, allowMeat: true, allowOther: true, out pelletDrawn);
	}

	public static int DrawKcal(GameItem dispenser, int amount, bool allowPellet, out int pelletDrawn)
	{
		return DrawKcal(dispenser, amount, allowPellet, allowMeat: true, allowOther: true, out pelletDrawn);
	}

	public static int DrawKcal(GameItem dispenser, int amount, bool allowPellet, bool allowMeat, bool allowOther, out int pelletDrawn)
	{
		int industrialDrawn;
		int meatDrawn;
		return DrawKcal(dispenser, amount, allowPellet, allowMeat, allowOther, out pelletDrawn, out industrialDrawn, out meatDrawn);
	}

	public static int DrawKcal(GameItem dispenser, int amount, bool allowPellet, bool allowMeat, bool allowOther, out int pelletDrawn, out int industrialDrawn, out int meatDrawn)
	{
		pelletDrawn = 0;
		industrialDrawn = 0;
		meatDrawn = 0;
		int @int = dispenser.GetTagReadonly("STORED_KCAL_TAG").GetInt();
		if (@int <= 0 || amount <= 0)
		{
			return 0;
		}
		List<string> list = new List<string>();
		if (allowPellet)
		{
			list.Add("STORED_GUTTERFLOW_PELLET");
			list.Add("STORED_INDUSTRIAL_FISH_FEED");
		}
		if (allowOther)
		{
			list.Add("STORED_OTHER");
		}
		List<string> list2 = new List<string>();
		if (allowMeat)
		{
			list2.Add("STORED_MEAT");
			foreach (string key in AnimalSpecies.All.Keys)
			{
				list2.Add(GetStoredTag(key));
			}
			list.AddRange(list2);
		}
		Dictionary<string, int> dictionary = SplitProportionally(dispenser, list, amount);
		int num = 0;
		foreach (KeyValuePair<string, int> kvp in dictionary)
		{
			if (kvp.Value > 0)
			{
				int bucketVal = dispenser.GetTagReadonly(kvp.Key).GetInt();
				dispenser.ModifyTag(kvp.Key, delegate(TagState state)
				{
					state.SetInt(bucketVal - kvp.Value);
				});
				num += kvp.Value;
				if (list2.Contains(kvp.Key))
				{
					meatDrawn += kvp.Value;
				}
			}
		}
		if (dictionary.TryGetValue("STORED_GUTTERFLOW_PELLET", out var value))
		{
			pelletDrawn = value;
		}
		if (dictionary.TryGetValue("STORED_INDUSTRIAL_FISH_FEED", out var value2))
		{
			industrialDrawn = value2;
		}
		int newTotal = Mathf.Max(0, @int - num);
		dispenser.ModifyTag("STORED_KCAL_TAG", delegate(TagState state)
		{
			state.SetInt(newTotal);
		});
		foreach (string key2 in AnimalSpecies.All.Keys)
		{
			if (dispenser.IsTag(GetPrionStoredTag(key2)) && dispenser.GetTagReadonly(GetStoredTag(key2)).GetInt() <= 0)
			{
				dispenser.DisableTag(GetPrionStoredTag(key2));
			}
		}
		return num;
	}

	public static Dictionary<string, int> SplitProportionally(GameItem dispenser, List<string> buckets, int amount)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Dictionary<string, int> stored = new Dictionary<string, int>();
		long num = 0L;
		foreach (string bucket in buckets)
		{
			int @int = dispenser.GetTagReadonly(bucket).GetInt();
			if (@int > 0)
			{
				stored[bucket] = @int;
				num += @int;
			}
		}
		if (num <= 0 || amount <= 0)
		{
			return dictionary;
		}
		if (amount >= num)
		{
			foreach (KeyValuePair<string, int> item in stored)
			{
				dictionary[item.Key] = item.Value;
			}
			return dictionary;
		}
		int num2 = 0;
		Dictionary<string, long> dictionary2 = new Dictionary<string, long>();
		foreach (KeyValuePair<string, int> item2 in stored)
		{
			long num3 = (long)amount * (long)item2.Value;
			dictionary[item2.Key] = (int)(num3 / num);
			dictionary2[item2.Key] = num3 % num;
			num2 += dictionary[item2.Key];
		}
		int num4 = amount - num2;
		foreach (string item3 in from kvp in dictionary2
			orderby kvp.Value descending, stored[kvp.Key] descending
			select kvp.Key)
		{
			if (num4 <= 0)
			{
				break;
			}
			if (dictionary[item3] < stored[item3])
			{
				dictionary[item3]++;
				num4--;
			}
		}
		return dictionary;
	}

	public static void CreateSlottedTooltip(RichTextBuilder builder, GameItem dispenser)
	{
		if (dispenser != null && dispenser.IsTag("FEED_DISPENSER_TAG"))
		{
			int @int = dispenser.GetTagReadonly("STORED_KCAL_TAG").GetInt();
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_feed_dispenser_stored", @int, 40000), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("FEED_DISPENSER_TAG"))
		{
			return;
		}
		int @int = item.GetTagReadonly("STORED_KCAL_TAG").GetInt();
		builder.AddLine(" ");
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_feed_dispenser_stored", @int, 40000), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		foreach (KeyValuePair<string, AnimalSpecies> item2 in AnimalSpecies.All)
		{
			int int2 = item.GetTagReadonly(GetStoredTag(item2.Key)).GetInt();
			if (int2 > 0)
			{
				builder.AddLine("  " + LocHelper.GetLocalizedMechanic("mech_feed_dispenser_species_meat", item2.Value.GetDisplayName(), int2), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
		int int3 = item.GetTagReadonly("STORED_MEAT").GetInt();
		if (int3 > 0)
		{
			builder.AddLine("  " + LocHelper.GetLocalizedMechanic("mech_feed_dispenser_mystery_meat", int3), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		int int4 = item.GetTagReadonly("STORED_OTHER").GetInt();
		if (int4 > 0)
		{
			builder.AddLine("  " + LocHelper.GetLocalizedMechanic("mech_feed_dispenser_other_food", int4), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		int pelletKcal = GetPelletKcal(item);
		if (pelletKcal > 0)
		{
			builder.AddLine("  " + LocHelper.GetLocalizedMechanic("mech_feed_dispenser_pellet", pelletKcal), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		int industrialFeedKcal = GetIndustrialFeedKcal(item);
		if (industrialFeedKcal > 0)
		{
			builder.AddLine("  " + LocHelper.GetLocalizedMechanic("mech_feed_dispenser_industrial", industrialFeedKcal), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		builder.AddLine(TargetCaption(GetTargetPercent(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		bool num = IsAutoBlendOn(item);
		string localizedMechanic = LocHelper.GetLocalizedMechanic(num ? "mech_feed_dispenser_auto_blend_on" : "mech_feed_dispenser_auto_blend_off");
		RenderHandler.ColorPalette color = (num ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightRed);
		int num2 = Mathf.Max(localizedMechanic.LastIndexOf(':'), localizedMechanic.LastIndexOf('：'));
		if (num2 < 0)
		{
			builder.AddLine(localizedMechanic, ignoreEmpty: true, color, bold: true);
			return;
		}
		builder.Add(localizedMechanic.Substring(0, num2 + 1), RenderHandler.ColorPalette.LightGray);
		builder.AddLine(localizedMechanic.Substring(num2 + 1), ignoreEmpty: true, color, bold: true);
	}
}

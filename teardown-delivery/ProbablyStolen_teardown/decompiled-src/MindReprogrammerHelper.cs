using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MindReprogrammerHelper
{
	private class PerkHoverTooltip : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		public StartingPerk perk;

		private bool hovered;

		public void OnPointerEnter(PointerEventData eventData)
		{
			PerkUIController instance = PerkUIController.Instance;
			if (!(instance == null) && perk != null)
			{
				instance.startingPerkTooltipTitle.text = perk.GetLocalizedDisplayName();
				instance.startingPerkTooltipDescription.text = perk.GetLocalizedDescription();
				instance.startingPerkTooltipCost.text = perk.cost + " " + LocHelper.GetLocalizedPerkTable("perk_points");
				instance.startingPerkTooltipIncompatible.text = "";
				hovered = true;
				FollowMouse(instance);
				instance.ShowTooltip();
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			Release();
		}

		private void OnDisable()
		{
			Release();
		}

		private void Update()
		{
			if (hovered)
			{
				PerkUIController instance = PerkUIController.Instance;
				if (!(instance == null))
				{
					FollowMouse(instance);
				}
			}
		}

		private void Release()
		{
			if (hovered)
			{
				hovered = false;
				if (PerkUIController.Instance != null)
				{
					PerkUIController.Instance.HideTooltip();
				}
			}
		}

		private static void FollowMouse(PerkUIController controller)
		{
			if (!(controller.startingPerkTooltip == null))
			{
				Vector3 tooltipPositionSimple = UIHelper.GetTooltipPositionSimple();
				tooltipPositionSimple.z = controller.startingPerkTooltip.transform.position.z;
				controller.startingPerkTooltip.transform.position = tooltipPositionSimple;
			}
		}
	}

	public const string MIND_REPROGRAMMER_TAG = "MIND_REPROGRAMMER_TAG";

	public const string CHARGE_INT = "MIND_REPROGRAMMER_CHARGE_INT";

	public const string COOLDOWN_DAYS_INT = "MIND_REPROGRAMMER_COOLDOWN_DAYS_INT";

	public const int COOLDOWN_DAYS_PER_CHARGE = 5;

	private const int NORMAL_INVENTORY_WIDTH = 24;

	private const int NORMAL_INVENTORY_HEIGHT = 10;

	private const string WINDOW_ID = "mind_reprogrammer";

	private const float WINDOW_WIDTH = 560f;

	private const float WINDOW_HEIGHT = 480f;

	private const float LIST_HEIGHT = 340f;

	private const float PERK_ICON_SIZE = 32f;

	private const float PERK_CELL_SIZE = 40f;

	private static GameItem openItem;

	private static RectTransform listHost;

	public static void InitMindReprogrammer(GameItem item)
	{
		item.EnableTag("MIND_REPROGRAMMER_TAG");
		item.ModifyTag("MIND_REPROGRAMMER_CHARGE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		item.ModifyTag("MIND_REPROGRAMMER_COOLDOWN_DAYS_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		item.EnableTag("TURBO_BOOST_ELIGIBLE_TAG");
		item.onLoaded = (Action<GameItem>)Delegate.Combine(item.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			loaded.EnableTag("TURBO_BOOST_ELIGIBLE_TAG");
		});
		item.onCycleEndSlotItemFunc = delegate(GameItem self, GameInventory _, SlotMarker _)
		{
			OnNight(self);
		};
		item.canItemTargetThisFunc = (GameItem source, GameItem self) => !MachineTurboBoosterAdv.IsTurboBooster(source) || GetCooldown(self) > 0;
		item.mayActivateSlotItemFunc = (GameItem self, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(self);
		item.canActivateSlotItemFunc = (GameItem self, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(self);
		item.onActivateSlotItemFunc = delegate(GameItem self, GameInventory _, SlotMarker _)
		{
			OpenWindow(self);
		};
	}

	public static int GetCharge(GameItem item)
	{
		return item.GetTagReadonly("MIND_REPROGRAMMER_CHARGE_INT").GetInt();
	}

	public static int GetCooldown(GameItem item)
	{
		return item.GetTagReadonly("MIND_REPROGRAMMER_COOLDOWN_DAYS_INT").GetInt();
	}

	public static bool CanAddCharge(GameItem target)
	{
		return target?.IsTag("MIND_REPROGRAMMER_TAG") ?? false;
	}

	public static void AddCharge(GameItem item, GameItem electronic)
	{
		if (CanAddCharge(item) && electronic != null)
		{
			int count = Mathf.Max(1, electronic.unitCount);
			item.ModifyTag("MIND_REPROGRAMMER_CHARGE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(GetCharge(item) + count);
			});
			item.unitValue += electronic.unitValue * count;
			electronic.Destroy();
			EmporiumAudioPlayer.Instance.ratchet.Play();
			if (openItem == item)
			{
				RebuildList();
			}
		}
	}

	private static void OnNight(GameItem item)
	{
		int days = GetCooldown(item);
		if (days > 0)
		{
			days--;
			item.ModifyTag("MIND_REPROGRAMMER_COOLDOWN_DAYS_INT", delegate(TagState state)
			{
				state.Enable().SetInt(days);
			});
			if (openItem == item)
			{
				RebuildList();
			}
		}
	}

	public static List<StartingPerk> GetEditablePerks()
	{
		List<StartingPerk> list = new List<StartingPerk>();
		foreach (StartingPerk item in StartingPerkList.AllPerkDefinitions())
		{
			if (item.mindEditable)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static int GetCost(StartingPerk definition)
	{
		return Mathf.Abs(definition.cost);
	}

	private static StartingPerk FindIncompatibleActivePerk(StartingPerk definition)
	{
		foreach (StartingPerk startingPerk in PlayerStore.Instance.startingPerks)
		{
			if (definition.incompatiblePerks != null && definition.incompatiblePerks.Contains(startingPerk.id))
			{
				return startingPerk;
			}
			if (startingPerk.incompatiblePerks != null && startingPerk.incompatiblePerks.Contains(definition.id))
			{
				return startingPerk;
			}
		}
		return null;
	}

	public static bool CanApply(GameItem item, StartingPerk definition)
	{
		bool flag = StartingPerk.IsPerkActive(definition.id);
		if (GetCooldown(item) > 0)
		{
			return false;
		}
		if (!flag && definition.type == StartingPerk.StartingPerkType.NEGATIVE)
		{
			return false;
		}
		if (!flag && FindIncompatibleActivePerk(definition) != null)
		{
			return false;
		}
		if (GetCharge(item) < GetCost(definition))
		{
			return false;
		}
		return true;
	}

	private static string GetBlockTooltip(StartingPerk definition)
	{
		if (StartingPerk.IsPerkActive(definition.id))
		{
			return null;
		}
		StartingPerk startingPerk = FindIncompatibleActivePerk(definition);
		if (startingPerk == null)
		{
			return null;
		}
		return LocHelper.GetLocalizedUI("ui_mind_reprogrammer_incompatible", startingPerk.GetLocalizedDisplayName());
	}

	public static void Apply(GameItem item, StartingPerk definition)
	{
		if (CanApply(item, definition))
		{
			if (StartingPerk.IsPerkActive(definition.id))
			{
				StartingPerk.RemovePerk(definition.id);
				OnPerkRemoved(definition.id);
			}
			else
			{
				PlayerStore.Instance.startingPerks.Add(definition);
				OnPerkAdded(definition.id);
			}
			int cost = GetCost(definition);
			item.ModifyTag("MIND_REPROGRAMMER_CHARGE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(GetCharge(item) - cost);
			});
			item.ModifyTag("MIND_REPROGRAMMER_COOLDOWN_DAYS_INT", delegate(TagState state)
			{
				state.Enable().SetInt(cost * 5);
			});
			AudioManager.Instance.Play2("turbo");
		}
	}

	private static void OnPerkAdded(string perkId)
	{
		if (perkId.Equals("nicotine_addiction"))
		{
			PlayerStore.Instance.healthData.daysSinceNicotineConsumption = 0;
		}
		if (perkId.Equals("alcohol_addiction"))
		{
			PlayerStore.Instance.healthData.daysSinceAlcoholConsumption = 0;
		}
		if (perkId.Equals("narcotic_addiction"))
		{
			PlayerStore.Instance.healthData.daysSinceNarcoticConsumption = 0;
			PlayerStore.Instance.healthData.daysSinceNarcoticRelief = 999;
		}
	}

	private static void OnPerkRemoved(string perkId)
	{
		if (perkId.Equals("nicotine_addiction"))
		{
			PlayerStore.Instance.healthData.daysSinceNicotineConsumption = 0;
		}
		if (perkId.Equals("alcohol_addiction"))
		{
			PlayerStore.Instance.healthData.daysSinceAlcoholConsumption = 0;
		}
		if (perkId.Equals("narcotic_addiction"))
		{
			PlayerStore.Instance.healthData.daysSinceNarcoticConsumption = 0;
			PlayerStore.Instance.healthData.daysSinceNarcoticRelief = 999;
		}
		if (perkId.Equals("minimalist") || perkId.Equals("ascetic"))
		{
			EmporiumEntry.Instance.invElement.SetShape(24, 10);
			EmporiumEntry.Instance.invElement.Validate();
			EmporiumEntry.Instance.invWindow.Validate();
		}
	}

	public static void OpenWindow(GameItem item)
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[MindReprogrammer] No CustomUIManager, cannot open the picker");
			return;
		}
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("mind_reprogrammer");
		Vector2? vector = ((window != null && window.IsAlive) ? new Vector2?(window.Rect.anchoredPosition) : null);
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("mind_reprogrammer", LocHelper.GetLocalizedItem("item_mind_reprogrammer_name"), "paper").SetSize(560f, 480f).SetDraggable(draggable: true)
			.SetCloseOnEscape()
			.OnClose(delegate
			{
				openItem = null;
				listHost = null;
			});
		openItem = item;
		listHost = null;
		customUIBuilder.BeginScroll(340f);
		listHost = customUIBuilder.CurrentContainer;
		customUIBuilder.End();
		if (vector.HasValue)
		{
			customUIBuilder.SetPosition(vector.Value);
		}
		else
		{
			customUIBuilder.Center();
		}
		customUIBuilder.Show();
		RebuildList();
	}

	public static void CloseWindow()
	{
		if (!(CustomUIManager.Instance == null))
		{
			CustomUIManager.Instance.CloseWindow("mind_reprogrammer");
		}
	}

	private static Sprite GetPerkIcon(StartingPerk definition)
	{
		return StartingPerkIconLoader.GetIcon(definition);
	}

	private static Color GetPerkIconColor(StartingPerk definition)
	{
		PerkUIController instance = PerkUIController.Instance;
		switch (definition.type)
		{
		case StartingPerk.StartingPerkType.POSITIVE:
			if (!(instance != null))
			{
				return Color.green;
			}
			return instance.green;
		case StartingPerk.StartingPerkType.NEGATIVE:
			if (!(instance != null))
			{
				return Color.red;
			}
			return instance.red;
		default:
			if (!(instance != null))
			{
				return new Color(1f, 0.65f, 0f);
			}
			return instance.orange;
		}
	}

	private static Sprite GetPerkBackground(bool active)
	{
		AdvCalendarUIManager instance = AdvCalendarUIManager.Instance;
		if (instance == null)
		{
			return null;
		}
		if (active && instance.highlightPerkBackground != null)
		{
			return instance.highlightPerkBackground;
		}
		StartingPerkElement startingPerkElement = ((instance.perkElementPrefab != null) ? instance.perkElementPrefab.GetComponent<StartingPerkElement>() : null);
		if (startingPerkElement != null && startingPerkElement.background != null && startingPerkElement.background.sprite != null)
		{
			return startingPerkElement.background.sprite;
		}
		return instance.highlightPerkBackground;
	}

	private static void AddPerkIcon(CustomUIBuilder builder, StartingPerk definition, bool active)
	{
		Sprite perkBackground = GetPerkBackground(active);
		CustomUIElement customUIElement = null;
		if (perkBackground != null)
		{
			builder.AddImage(perkBackground, 40f, 40f);
			customUIElement = builder.Last;
			AttachPerkTooltip(customUIElement, definition);
		}
		builder.AddImage(GetPerkIcon(definition), 32f, 32f);
		CustomUIElement last = builder.Last;
		last?.SetImageColor(GetPerkIconColor(definition));
		AttachPerkTooltip(last, definition);
		if (!(customUIElement?.root == null) && !(last?.Rect == null))
		{
			RectTransform rect = last.Rect;
			rect.SetParent(customUIElement.root.transform, worldPositionStays: false);
			rect.anchorMin = new Vector2(0.5f, 0.5f);
			rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = new Vector2(32f, 32f);
		}
	}

	private static void AttachPerkTooltip(CustomUIElement element, StartingPerk definition)
	{
		if (element != null && !(element.root == null))
		{
			if (element.image != null)
			{
				element.image.raycastTarget = true;
			}
			if (element.text != null)
			{
				element.text.raycastTarget = true;
			}
			element.root.AddComponent<PerkHoverTooltip>().perk = definition;
		}
	}

	private static void RebuildList()
	{
		CustomUIWindow customUIWindow = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("mind_reprogrammer") : null);
		if (customUIWindow == null || !customUIWindow.IsAlive || listHost == null || openItem == null)
		{
			return;
		}
		CustomUIBuilder.ClearContainer(listHost);
		CustomUIBuilder customUIBuilder = new CustomUIBuilder(CustomUIManager.Instance, customUIWindow, listHost);
		foreach (StartingPerk definition in GetEditablePerks())
		{
			bool flag = StartingPerk.IsPerkActive(definition.id);
			if (!flag && definition.type != 0)
			{
				continue;
			}
			bool flag2 = CanApply(openItem, definition);
			string text = (flag2 ? null : GetBlockTooltip(definition));
			customUIBuilder.BeginRow(4f, TextAnchor.MiddleLeft);
			AddPerkIcon(customUIBuilder, definition, flag);
			customUIBuilder.AddLabel(definition.GetLocalizedDisplayName() + " (" + GetCost(definition) + ")" + (flag ? ("  " + LocHelper.GetLocalizedUI("ui_mind_reprogrammer_active")) : ""));
			AttachPerkTooltip(customUIBuilder.Last, definition);
			customUIBuilder.AddFlexibleSpace();
			customUIBuilder.AddButton(flag ? LocHelper.GetLocalizedUI("ui_mind_reprogrammer_remove") : LocHelper.GetLocalizedUI("ui_mind_reprogrammer_add"), delegate
			{
				if (openItem != null)
				{
					Apply(openItem, definition);
					RebuildList();
				}
			});
			if (text != null)
			{
				customUIBuilder.WithTooltip(text);
			}
			customUIBuilder.Last?.SetInteractable(flag2);
			customUIBuilder.End();
		}
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("MIND_REPROGRAMMER_TAG"))
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_mind_reprogrammer_charge", GetCharge(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			int cooldown = GetCooldown(item);
			if (cooldown > 0)
			{
				builder.AddLine((cooldown == 1) ? LocHelper.GetLocalizedMechanic("mech_mind_reprogrammer_cooldown_one") : LocHelper.GetLocalizedMechanic("mech_mind_reprogrammer_cooldown", cooldown), ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
			else
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_mind_reprogrammer_ready"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_mind_reprogrammer_charge_hint"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
		}
	}
}

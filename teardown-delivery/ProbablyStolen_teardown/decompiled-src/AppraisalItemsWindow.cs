using System.Collections.Generic;
using UnityEngine;

public static class AppraisalItemsWindow
{
	private const string WINDOW_ID = "appraisal_items";

	private const float WINDOW_WIDTH = 420f;

	private const float WINDOW_HEIGHT = 300f;

	private const float LIST_HEIGHT = 170f;

	public static void Open()
	{
		Open(playSound: true);
	}

	private static void Open(bool playSound)
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[AppraisalItemsWindow] No CustomUIManager, cannot open the window");
			return;
		}
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("appraisal_items");
		Vector2? vector = ((window != null && window.IsAlive) ? new Vector2?(window.Rect.anchoredPosition) : null);
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("appraisal_items", LocHelper.GetLocalizedUI("ui_appraisal_items_title"), "paper").SetSize(420f, 300f).SetDraggable(draggable: true)
			.SetCloseOnEscape();
		customUIBuilder.AddLabel(LocHelper.GetLocalizedUI("ui_appraisal_items_intro"));
		customUIBuilder.BeginScroll(170f);
		int rank = AppraisalRankHelper.GetRank();
		foreach (AppraisalItemType item in AppraisalItemRegistry.GetUnlocked(rank))
		{
			AddRow(customUIBuilder, item, rank);
		}
		customUIBuilder.End();
		customUIBuilder.BeginRow();
		customUIBuilder.AddFlexibleSpace();
		customUIBuilder.AddButton(LocHelper.GetLocalizedOrDefault("MainMenu", "close", "Close"), delegate
		{
			CustomUIManager.Instance.CloseWindow("appraisal_items");
		});
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
		if (playSound && PaperUIManager.Instance != null && PaperUIManager.Instance.audioSourcePickup != null)
		{
			PaperUIManager.Instance.audioSourcePickup.Play();
		}
	}

	private static void AddRow(CustomUIBuilder builder, AppraisalItemType type, int rank)
	{
		bool flag = AppraisalItemRegistry.CanBlock(type, rank);
		bool flag2 = AppraisalItemRegistry.IsLastAllowed(type, rank);
		bool num = !flag || flag2;
		builder.BeginRow(6f, TextAnchor.MiddleLeft);
		builder.AddLabel(type.GetDisplayName());
		builder.AddFlexibleSpace();
		if (num)
		{
			string tooltipText = (flag ? LocHelper.GetLocalizedUIOrDefault("ui_appraisal_items_locked_last", "At least one item must stay allowed.") : (AppraisalRankHelper.CanProgress() ? LocHelper.GetLocalizedUI("ui_appraisal_items_locked") : LocHelper.GetLocalizedUI("ui_appraisal_items_locked_final")));
			builder.AddLabel("<sprite name=\"info\" tint=1>").WithTooltip(tooltipText);
		}
		builder.AddToggle(LocHelper.GetLocalizedUI("ui_appraisal_items_allowed"), !AppraisalItemRegistry.IsBlocked(type, rank), delegate(bool allowed)
		{
			SetAllowed(type, allowed);
		});
		if (num)
		{
			builder.Last?.SetInteractable(interactable: false);
		}
		builder.End();
	}

	private static void SetAllowed(AppraisalItemType type, bool allowed)
	{
		int rank = AppraisalRankHelper.GetRank();
		if (AppraisalItemRegistry.CanBlock(type, rank) && (allowed || !AppraisalItemRegistry.IsLastAllowed(type, rank)))
		{
			List<string> list = PlayerStore.Instance.appraisalBlacklist ?? (PlayerStore.Instance.appraisalBlacklist = new List<string>());
			list.Remove(type.id);
			if (!allowed)
			{
				list.Add(type.id);
			}
			Open(playSound: false);
		}
	}
}

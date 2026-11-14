using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class CustomUIAfterhourEventsPanel
{
	public const string WINDOW_ID = "afterhour_events_panel";

	private const float COLUMN_WIDTH = 260f;

	private const float COLUMN_SPACING = 4f;

	private const float SCROLL_HEIGHT = 480f;

	private const float ROW_SPACING = 2f;

	private const float BUTTON_HEIGHT = 26f;

	private const float HEADER_HEIGHT = 44f;

	private const float BACKGROUND_ALPHA = 0.8f;

	private const int SIDE_MARGIN = 4;

	private const int MARGIN = 2;

	private const float WINDOW_HEADER_HEIGHT = 32f;

	private const float LAYOUT_SPACING = 4f;

	private const float MIN_FONT_SIZE = 9f;

	private static readonly int[] FIXED_DAYS = new int[3] { 1, 3, 7 };

	private static readonly Color ACTIVE_COLOR = new Color(0.55f, 0.9f, 0.55f);

	private static readonly Color CLOSED_COLOR = new Color(1f, 0.55f, 0.45f);

	private static readonly Color MUTED_COLOR = new Color(0.75f, 0.72f, 0.65f);

	public static void Toggle()
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[CustomUI] No CustomUIManager, cannot open the afterhour events panel");
			return;
		}
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("afterhour_events_panel");
		if (window != null && window.IsAlive)
		{
			window.Toggle();
		}
		else
		{
			Open();
		}
	}

	public static void Open()
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[CustomUI] No CustomUIManager, cannot open the afterhour events panel");
			return;
		}
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("afterhour_events_panel");
		Vector2? vector = ((window != null && window.IsAlive) ? new Vector2?(window.Rect.anchoredPosition) : null);
		List<AfterhourLocation> all = AfterhourLocation.GetAll();
		int num = Mathf.Max(1, all.Count);
		float width = (float)num * 260f + (float)(num - 1) * 4f + 16f;
		float height = 576f;
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("afterhour_events_panel", "Afterhour Events", "rear").SetSize(width, height).SetDraggable(draggable: true)
			.SetCloseOnEscape();
		if (all.Count == 0)
		{
			customUIBuilder.AddLabel("No afterhour location is registered in this scene.");
		}
		else if (!PlayerStore.HasInstance)
		{
			customUIBuilder.AddLabel("Afterhour events need a running game (PlayerStore).");
		}
		else
		{
			customUIBuilder.BeginRow();
			foreach (AfterhourLocation item in all)
			{
				BuildLocationColumn(customUIBuilder, item);
			}
			customUIBuilder.End();
		}
		if (vector.HasValue)
		{
			customUIBuilder.SetPosition(vector.Value);
		}
		else
		{
			customUIBuilder.Center();
		}
		CustomUIWindow window2 = customUIBuilder.Show();
		CustomUIDebugPanel.SetBackgroundAlpha(window2, 0.8f);
		CompactWindowPadding(window2);
		CustomUIDebugPanel.ForwardDragsToWindow(window2);
	}

	private static void Rebuild()
	{
		Open();
	}

	private static void BuildLocationColumn(CustomUIBuilder builder, AfterhourLocation location)
	{
		builder.BeginColumn();
		builder.AddLabel(LocationHeader(location));
		builder.Last?.SetPreferredSize(260f, 44f);
		CenterLabel(builder.Last);
		builder.BeginScroll(480f);
		CustomUIDebugPanel.ConfigureColumnScroll(builder.CurrentContainer, 260f, 2f);
		List<AfterhourEventDefinition> active = AfterhourEvents.GetActive(location.Id);
		int maxConcurrentEvents = location.MaxConcurrentEvents;
		string text = ((maxConcurrentEvents > 0) ? $"{active.Count}/{maxConcurrentEvents}" : active.Count.ToString());
		builder.AddLabel("<b>Active (" + text + ")</b>");
		if (active.Count == 0)
		{
			builder.AddLabel("(none)");
			builder.Last?.SetTextColor(MUTED_COLOR);
		}
		foreach (AfterhourEventDefinition item in active)
		{
			BuildActiveRow(builder, item);
		}
		if (active.Count > 1)
		{
			string locationId = location.Id;
			builder.AddButton("End all", delegate
			{
				AfterhourEvents.EndAll(locationId);
				Rebuild();
			});
			StyleButton(builder.Last, 260f);
		}
		builder.AddSpace(6f);
		builder.AddLabel("<b>Available</b>");
		int num = 0;
		foreach (AfterhourEventDefinition item2 in AfterhourEvents.GetAll())
		{
			if (!(item2.locationId != location.Id) && !AfterhourEvents.IsActive(item2.id))
			{
				num++;
				BuildDefinitionRow(builder, item2, maxConcurrentEvents > 0 && active.Count >= maxConcurrentEvents);
			}
		}
		if (num == 0)
		{
			builder.AddLabel("(no other event defined here)");
			builder.Last?.SetTextColor(MUTED_COLOR);
		}
		builder.End();
		builder.End();
	}

	private static string LocationHeader(AfterhourLocation location)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<b>").Append(location.GetTitle()).Append("</b>\n<size=80%>")
			.Append(location.Id);
		if (location.Source != "vanilla")
		{
			stringBuilder.Append(" (").Append(location.Source).Append(')');
		}
		if (!location.IsDiscovered())
		{
			stringBuilder.Append(", undiscovered");
		}
		if (location.IsClosedByEvent())
		{
			stringBuilder.Append(", <color=#FF8C73>closed by event</color>");
		}
		else if (!location.IsAvailable())
		{
			stringBuilder.Append(", unavailable");
		}
		stringBuilder.Append("</size>");
		return stringBuilder.ToString();
	}

	private static void BuildActiveRow(CustomUIBuilder builder, AfterhourEventDefinition definition)
	{
		int daysLeft = AfterhourEvents.GetDaysLeft(definition.id);
		string text = ((daysLeft == 1) ? "today only" : (daysLeft + " days left"));
		builder.AddLabel($"<b>{definition.id}</b>\n<size=80%>{definition.GetTitle()}\n{text}, until day {AfterhourEvents.GetEndsOnDay(definition.id)}</size>");
		StyleRowLabel(builder.Last);
		builder.Last?.SetTextColor(definition.closesLocation ? CLOSED_COLOR : ACTIVE_COLOR);
		builder.WithTooltip(DefinitionTooltip(definition));
		string eventId = definition.id;
		builder.AddButton("End", delegate
		{
			AfterhourEvents.End(eventId);
			Rebuild();
		});
		StyleButton(builder.Last, 260f);
	}

	private static void BuildDefinitionRow(CustomUIBuilder builder, AfterhourEventDefinition definition, bool locationFull)
	{
		builder.AddLabel("<b>" + definition.id + "</b>\n<size=80%>" + definition.GetTitle() + "</size>");
		StyleRowLabel(builder.Last);
		builder.WithTooltip(DefinitionTooltip(definition));
		float num = 260f - (float)FIXED_DAYS.Length * 2f;
		float num2 = num * 0.2f;
		builder.BeginRow(2f);
		string eventId = definition.id;
		builder.AddButton($"Start ({definition.durationDays}d)", delegate
		{
			Start(eventId, 0);
		});
		StyleButton(builder.Last, num - (float)FIXED_DAYS.Length * num2);
		if (locationFull)
		{
			builder.Last?.SetInteractable(interactable: false);
		}
		int[] fIXED_DAYS = FIXED_DAYS;
		for (int i = 0; i < fIXED_DAYS.Length; i++)
		{
			int num3 = fIXED_DAYS[i];
			int captured = num3;
			builder.AddButton(num3 + "d", delegate
			{
				Start(eventId, captured);
			});
			StyleButton(builder.Last, num2);
			if (locationFull)
			{
				builder.Last?.SetInteractable(interactable: false);
			}
		}
		builder.End();
		if (locationFull)
		{
			builder.AddLabel("(location at its event limit)");
			builder.Last?.SetTextColor(MUTED_COLOR);
		}
	}

	private static void Start(string eventId, int days)
	{
		if (!AfterhourEvents.Start(eventId, days))
		{
			Debug.LogWarning("[CustomUI] Afterhour event '" + eventId + "' could not start (unknown id or full location)");
		}
		Rebuild();
	}

	private static string DefinitionTooltip(AfterhourEventDefinition definition)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(definition.id).Append(" (").Append(definition.source)
			.Append(")\n");
		string description = definition.GetDescription();
		if (!string.IsNullOrEmpty(description))
		{
			stringBuilder.Append(description).Append('\n');
		}
		foreach (string effectText in definition.GetEffectTexts())
		{
			stringBuilder.Append("- ").Append(effectText).Append('\n');
		}
		stringBuilder.Append("Duration: ").Append(definition.durationDays).Append(" day(s)");
		if (definition.dailyChancePercent > 0f)
		{
			stringBuilder.Append(", daily chance ").Append(definition.dailyChancePercent).Append('%');
		}
		if (definition.weekdays.Count > 0)
		{
			stringBuilder.Append(", every ").Append(string.Join("/", definition.weekdays));
		}
		if (definition.closesLocation)
		{
			stringBuilder.Append(", closes the location");
		}
		if (definition.canStart != null)
		{
			stringBuilder.Append(", has a start condition");
		}
		return stringBuilder.ToString();
	}

	private static void StyleRowLabel(CustomUIElement element)
	{
		if (element != null)
		{
			element.SetFixedWidth(260f);
			if (element.text != null)
			{
				element.text.alignment = TextAlignmentOptions.TopLeft;
				element.text.enableWordWrapping = true;
				element.text.overflowMode = TextOverflowModes.Overflow;
			}
		}
	}

	private static void StyleButton(CustomUIElement element, float width)
	{
		if (element != null)
		{
			element.SetFixedWidth(width);
			element.SetPreferredSize(width, 26f);
			if (element.text != null)
			{
				element.text.alignment = TextAlignmentOptions.Center;
				element.text.enableWordWrapping = false;
				element.text.overflowMode = TextOverflowModes.Overflow;
				element.text.fontSizeMax = element.text.fontSize;
				element.text.fontSizeMin = 9f;
				element.text.enableAutoSizing = true;
			}
		}
	}

	private static void CenterLabel(CustomUIElement element)
	{
		if (element != null && !(element.text == null))
		{
			element.text.alignment = TextAlignmentOptions.Center;
		}
	}

	private static void CompactWindowPadding(CustomUIWindow window)
	{
		if (window != null)
		{
			SetPadding((window.root != null) ? window.root.GetComponent<VerticalLayoutGroup>() : null);
			SetPadding((window.ContentRoot != null) ? window.ContentRoot.GetComponent<VerticalLayoutGroup>() : null);
		}
	}

	private static void SetPadding(HorizontalOrVerticalLayoutGroup group)
	{
		if (!(group == null))
		{
			group.padding = new RectOffset(4, 4, 2, 2);
		}
	}
}

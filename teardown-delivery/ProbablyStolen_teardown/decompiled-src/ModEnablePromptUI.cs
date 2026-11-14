using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ModEnablePromptUI
{
	public const float PANEL_WIDTH = 950f;

	public const float PADDING = 30f;

	public const float SPACING = 15f;

	public const float BUTTON_WIDTH = 275f;

	private static GameObject backdrop;

	private static Transform root;

	private static string label;

	private static List<string> capabilities;

	private static ulong workshopId;

	private static ModWorkshop.ItemStats? previewStats;

	private static Action onActivate;

	private static bool waitingForDetails;

	public static bool IsOpen => backdrop != null;

	public static bool Show(Transform canvasRoot, string modLabel, IEnumerable<string> declared, ulong publishedFileId, Action activate, ModWorkshop.ItemStats? preview = null)
	{
		Close();
		root = canvasRoot;
		label = modLabel;
		capabilities = (declared ?? Enumerable.Empty<string>()).ToList();
		workshopId = publishedFileId;
		previewStats = preview;
		onActivate = activate;
		if (workshopId != 0L && !previewStats.HasValue && !ModWorkshop.TryGetStats(workshopId, out var _))
		{
			ModWorkshop.RequestDetails(new ulong[1] { workshopId });
			WaitForDetails(wait: true);
		}
		if (Build())
		{
			return true;
		}
		Close();
		return false;
	}

	private static bool Build()
	{
		if (backdrop != null)
		{
			UnityEngine.Object.Destroy(backdrop);
		}
		backdrop = ModWarningUI.CreateDialog(root, "mod_enable_prompt", 950f, out var builder);
		if (backdrop == null)
		{
			return false;
		}
		builder.BeginPanel(950f, -1f, TextAnchor.UpperLeft, 30f, 15f, "frame");
		builder.AddHeader(label, "title");
		AddWorkshopLines(builder);
		builder.AddLabel(LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_header", "{0} asks for these capabilities:", label), "body");
		builder.AddLabel(FormatCapabilities(capabilities), "capabilities");
		bool flag = capabilities.Any((string capability) => ModCapability.RiskOf(capability) == ModCapability.Risk.Severe);
		builder.AddLabel(flag ? LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_warning_severe", "A mod with these capabilities can do anything on this computer that you can. Only activate mods from authors you trust.") : LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_warning", "Only activate mods from authors you trust."), "warning");
		builder.AddSpace(10f);
		builder.BeginRow(15f, TextAnchor.MiddleCenter);
		builder.AddFlexibleSpace();
		builder.AddButton(LocHelper.GetLocalizedOrDefault("MainMenu", "load_cancel", "Cancel"), Close, "cancel");
		builder.Last.SetFixedWidth(275f);
		builder.AddButton(LocHelper.GetLocalizedUI("ui_mods_activate"), Activate, "activate");
		builder.Last.SetFixedWidth(275f);
		builder.AddFlexibleSpace();
		builder.End();
		builder.End();
		return true;
	}

	private static void AddWorkshopLines(CustomUIBuilder b)
	{
		if (workshopId == 0L && !previewStats.HasValue)
		{
			b.AddLabel(LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_local", "Not installed from the Steam Workshop: its upload date and ratings cannot be verified."), "workshop");
			return;
		}
		ModWorkshop.ItemStats stats;
		if (previewStats.HasValue)
		{
			stats = previewStats.Value;
		}
		else if (!ModWorkshop.TryGetStats(workshopId, out stats))
		{
			if (ModWorkshop.IsDetailsPending(workshopId))
			{
				b.AddLabel(LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_checking", "Checking the Workshop for the upload date and ratings..."), "workshop");
				return;
			}
			b.AddLabel(Emphasis(ModCapability.Risk.Severe, LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_unknown", "The Workshop did not answer: the upload date and ratings are unknown.")), "workshop");
			b.Last.SetTextColor(ColorOf(ModCapability.Risk.Severe));
			return;
		}
		string ago;
		ModCapability.Risk risk = Age(stats.createdUtc, out ago);
		b.AddLabel(Emphasis(risk, LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_uploaded", "First uploaded to the Workshop on {0} ({1})", ModUIManager.FormatDate(stats.createdUtc), ago)), "uploaded");
		b.Last.SetTextColor(ColorOf(risk));
		ModCapability.Risk risk2 = RatingRisk(stats.Ratings);
		b.AddLabel(Emphasis(risk2, LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_ratings", "Number of users who rated it: {0}", stats.Ratings)), "ratings");
		b.Last.SetTextColor(ColorOf(risk2));
	}

	private static string Emphasis(ModCapability.Risk risk, string text)
	{
		if (risk != ModCapability.Risk.Severe)
		{
			return text;
		}
		return "<b>" + text + "</b>";
	}

	private static ModCapability.Risk Age(DateTime createdUtc, out string ago)
	{
		DateTime utcNow = DateTime.UtcNow;
		TimeSpan elapsed = ((utcNow > createdUtc) ? (utcNow - createdUtc) : TimeSpan.Zero);
		int i;
		for (i = 0; createdUtc.AddMonths(i + 1) <= utcNow; i++)
		{
		}
		ago = AgoText(elapsed, i);
		if (elapsed.TotalDays < 3.0)
		{
			return ModCapability.Risk.Severe;
		}
		if (elapsed.TotalDays < 14.0)
		{
			return ModCapability.Risk.High;
		}
		if (i < 3)
		{
			return ModCapability.Risk.Medium;
		}
		return ModCapability.Risk.Low;
	}

	private static string AgoText(TimeSpan elapsed, int months)
	{
		if (elapsed.TotalHours < 1.0)
		{
			return LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_ago_now", "less than an hour ago");
		}
		if (elapsed.TotalDays < 1.0)
		{
			int num = (int)elapsed.TotalHours;
			if (num != 1)
			{
				return LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_ago_hours", "{0} hours ago", num);
			}
			return LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_ago_hour", "1 hour ago");
		}
		if (elapsed.TotalDays < 14.0)
		{
			int num2 = (int)elapsed.TotalDays;
			if (num2 != 1)
			{
				return LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_ago_days", "{0} days ago", num2);
			}
			return LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_ago_day", "1 day ago");
		}
		if (months < 2)
		{
			return LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_ago_weeks", "{0} weeks ago", (int)(elapsed.TotalDays / 7.0));
		}
		if (months < 12)
		{
			return LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_ago_months", "{0} months ago", months);
		}
		int num3 = months / 12;
		if (num3 != 1)
		{
			return LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_ago_years", "{0} years ago", num3);
		}
		return LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_ago_year", "1 year ago");
	}

	private static ModCapability.Risk RatingRisk(uint ratings)
	{
		switch (ratings)
		{
		case 0u:
		case 1u:
		case 2u:
		case 3u:
		case 4u:
		case 5u:
		case 6u:
		case 7u:
		case 8u:
		case 9u:
		case 10u:
		case 11u:
		case 12u:
		case 13u:
		case 14u:
			return ModCapability.Risk.Severe;
		case 15u:
		case 16u:
		case 17u:
		case 18u:
		case 19u:
		case 20u:
		case 21u:
		case 22u:
		case 23u:
		case 24u:
		case 25u:
		case 26u:
		case 27u:
		case 28u:
		case 29u:
		case 30u:
		case 31u:
		case 32u:
		case 33u:
		case 34u:
		case 35u:
		case 36u:
		case 37u:
		case 38u:
		case 39u:
		case 40u:
		case 41u:
		case 42u:
		case 43u:
		case 44u:
		case 45u:
		case 46u:
		case 47u:
		case 48u:
		case 49u:
			return ModCapability.Risk.High;
		case 50u:
		case 51u:
		case 52u:
		case 53u:
		case 54u:
		case 55u:
		case 56u:
		case 57u:
		case 58u:
		case 59u:
		case 60u:
		case 61u:
		case 62u:
		case 63u:
		case 64u:
		case 65u:
		case 66u:
		case 67u:
		case 68u:
		case 69u:
		case 70u:
		case 71u:
		case 72u:
		case 73u:
		case 74u:
		case 75u:
		case 76u:
		case 77u:
		case 78u:
		case 79u:
		case 80u:
		case 81u:
		case 82u:
		case 83u:
		case 84u:
		case 85u:
		case 86u:
		case 87u:
		case 88u:
		case 89u:
		case 90u:
		case 91u:
		case 92u:
		case 93u:
		case 94u:
		case 95u:
		case 96u:
		case 97u:
		case 98u:
		case 99u:
			return ModCapability.Risk.Medium;
		default:
			return ModCapability.Risk.Low;
		}
	}

	public static Color ColorOf(ModCapability.Risk risk)
	{
		return risk switch
		{
			ModCapability.Risk.Severe => NewGameStyle.TAG_DANGER, 
			ModCapability.Risk.High => NewGameStyle.WARNING, 
			ModCapability.Risk.Medium => NewGameStyle.BUTTON_HOVER, 
			_ => NewGameStyle.TAG_POSITIVE, 
		};
	}

	public static string FormatCapabilities(IEnumerable<string> declared)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in ModCapability.Order(declared).OrderByDescending(ModCapability.RiskOf))
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append('\n');
			}
			ModCapability.Risk risk = ModCapability.RiskOf(item);
			string text = ModCapability.DisplayName(item);
			if (risk == ModCapability.Risk.Severe)
			{
				text = LocHelper.GetLocalizedUIOrDefault("ui_mods_prompt_dangerous", "POTENTIALLY DANGEROUS: {0}", text);
			}
			stringBuilder.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(ColorOf(risk))).Append('>')
				.Append(Emphasis(risk, "- " + text))
				.Append("</color>");
		}
		return stringBuilder.ToString();
	}

	private static void WaitForDetails(bool wait)
	{
		if (wait != waitingForDetails)
		{
			waitingForDetails = wait;
			if (wait)
			{
				ModWorkshop.OnDetailsUpdated += OnDetailsUpdated;
			}
			else
			{
				ModWorkshop.OnDetailsUpdated -= OnDetailsUpdated;
			}
		}
	}

	private static void OnDetailsUpdated()
	{
		if (IsOpen && !ModWorkshop.IsDetailsPending(workshopId))
		{
			WaitForDetails(wait: false);
			Build();
		}
	}

	private static void Activate()
	{
		Action action = onActivate;
		Close();
		action?.Invoke();
	}

	public static void Close()
	{
		WaitForDetails(wait: false);
		if (backdrop != null)
		{
			UnityEngine.Object.Destroy(backdrop);
		}
		backdrop = null;
		onActivate = null;
	}
}

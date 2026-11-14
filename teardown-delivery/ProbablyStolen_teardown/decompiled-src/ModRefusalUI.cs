using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class ModRefusalUI
{
	private static GameObject backdrop;

	private static string report;

	public static bool IsOpen => backdrop != null;

	public static bool Show(Transform canvasRoot, string modLabel, string reason, string log)
	{
		Close();
		backdrop = ModWarningUI.CreateDialog(canvasRoot, "mod_refusal", 950f, out var builder);
		if (backdrop == null)
		{
			return false;
		}
		report = log;
		UnityEngine.Debug.Log("[ModUI] Activation refused, report:\n" + log);
		builder.BeginPanel(950f, -1f, TextAnchor.UpperLeft, 30f, 15f, "frame");
		builder.AddHeader(modLabel, "title");
		builder.AddLabel("<b>" + LocHelper.GetLocalizedUIOrDefault("ui_mods_refused_heading", "This mod cannot be activated.") + "</b>", "heading");
		builder.Last.SetTextColor(ModEnablePromptUI.ColorOf(ModCapability.Risk.Severe));
		builder.AddLabel(reason, "reason");
		builder.AddLabel(LocHelper.GetLocalizedUIOrDefault("ui_mods_refused_hint", "Copy the log and send it to the mod's author, or attach it to a bug report."), "hint");
		builder.AddSpace(10f);
		builder.BeginRow(15f, TextAnchor.MiddleCenter);
		builder.AddFlexibleSpace();
		CustomUIElement copyButton = null;
		builder.AddButton(LocHelper.GetLocalizedUIOrDefault("ui_mods_refused_copy_log", "Copy log"), delegate
		{
			Copy(copyButton);
		}, "copy_log");
		copyButton = builder.Last;
		copyButton.SetFixedWidth(275f);
		builder.AddButton(LocHelper.GetLocalizedOrDefault("MainMenu", "close", "Close"), Close, "close");
		builder.Last.SetFixedWidth(275f);
		builder.AddFlexibleSpace();
		builder.End();
		builder.End();
		return true;
	}

	private static void Copy(CustomUIElement button)
	{
		GUIUtility.systemCopyBuffer = report ?? "";
		button?.SetText(LocHelper.GetLocalizedUIOrDefault("ui_mods_refused_copied", "Copied"));
	}

	public static string BuildLog(ModEnableCheck check)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Probably Stolen: mod activation refused");
		stringBuilder.AppendLine("Game: VERSION 100-Rev12");
		stringBuilder.AppendLine($"Date (UTC): {DateTime.UtcNow:yyyy-MM-dd HH:mm}");
		LoadedMod mod = check.Mod;
		if (mod != null)
		{
			stringBuilder.AppendLine("Mod: " + mod.Manifest.Name + " (" + mod.Manifest.ID + "), version " + mod.Manifest.ModVersion);
			stringBuilder.AppendLine((mod.Source == ModSource.Workshop) ? $"Source: Steam Workshop item {mod.WorkshopId}" : ("Source: local folder " + Path.GetFileName(mod.FolderPath)));
		}
		if (check.Fingerprint != null)
		{
			stringBuilder.AppendLine("Version hash: " + check.Fingerprint.Combined);
		}
		if (check.Verdict != 0)
		{
			stringBuilder.AppendLine($"Trust list: {check.Verdict}");
		}
		stringBuilder.AppendLine($"Scanner version: {3}");
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("Reason:");
		stringBuilder.AppendLine(check.Refusal ?? "-");
		if (check.Scan != null)
		{
			stringBuilder.AppendLine();
			if (check.Scan.Error != null)
			{
				stringBuilder.AppendLine("Scan error: " + check.Scan.Error);
			}
			if (check.Undeclared.Count > 0)
			{
				stringBuilder.AppendLine("Undeclared capabilities: " + string.Join(", ", check.Undeclared));
			}
			string text = check.Scan.FormatFindings(200);
			stringBuilder.AppendLine("Scanner findings:");
			stringBuilder.AppendLine(string.IsNullOrEmpty(text) ? "none" : text);
		}
		return ScrubUserProfile(stringBuilder.ToString());
	}

	private static string ScrubUserProfile(string text)
	{
		string text2 = null;
		try
		{
			text2 = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		}
		catch
		{
		}
		if (string.IsNullOrEmpty(text2))
		{
			return text;
		}
		text2 = text2.TrimEnd('\\', '/');
		string[] array = new string[2]
		{
			text2,
			text2.Replace('\\', '/')
		};
		foreach (string text3 in array)
		{
			int num;
			while ((num = text.IndexOf(text3, StringComparison.OrdinalIgnoreCase)) >= 0)
			{
				text = text.Substring(0, num) + "%USERPROFILE%" + text.Substring(num + text3.Length);
			}
		}
		return text;
	}

	public static void Close()
	{
		if (backdrop != null)
		{
			UnityEngine.Object.Destroy(backdrop);
		}
		backdrop = null;
		report = null;
	}
}

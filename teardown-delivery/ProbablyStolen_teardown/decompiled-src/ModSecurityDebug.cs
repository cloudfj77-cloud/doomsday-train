using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QFSW.QC;
using UnityEngine;

public static class ModSecurityDebug
{
	[Command("mod-scan", "Scans a mod's assemblies and prints the capabilities it would need to declare, every finding, and any hard flag", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Scan(string idOrFolder)
	{
		LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == idOrFolder);
		string text;
		ModManifest modManifest;
		if (loadedMod != null)
		{
			text = loadedMod.FolderPath;
			modManifest = loadedMod.Manifest;
		}
		else
		{
			if (!System.IO.Directory.Exists(idOrFolder))
			{
				return "No mod with ID '" + idOrFolder + "' and no such folder.";
			}
			text = idOrFolder;
			modManifest = ModLoader.ParseManifestForTools(text, out var problem);
			if (modManifest == null)
			{
				return "Cannot scan: " + problem;
			}
		}
		ModScanResult modScanResult = ModScanner.Scan(ModLoader.ListedAssemblyPaths(text, modManifest), null, text, loadedMod == null || loadedMod.Source == ModSource.Local);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(ModLoader.GetModLabel(modManifest) + " (" + text + ")");
		stringBuilder.AppendLine("  author: \"" + modManifest.Author + "\"");
		stringBuilder.AppendLine("  assemblies: " + string.Join(", ", modManifest.Assemblies));
		List<string> list = ModLoader.UnlistedAssemblyFiles(text, modManifest);
		if (list.Count > 0)
		{
			stringBuilder.AppendLine("  unlisted DLLs (never loaded): " + string.Join(", ", list));
		}
		stringBuilder.AppendLine($"  methods scanned: {modScanResult.MethodsScanned}");
		if (modScanResult.Error != null)
		{
			stringBuilder.AppendLine("  ERROR: " + modScanResult.Error);
		}
		stringBuilder.AppendLine("  declared: " + ModCapability.FormatIds(modManifest.Capabilities));
		stringBuilder.AppendLine("  detected: " + ModCapability.FormatIds(modScanResult.Capabilities));
		List<string> list2 = ModCapability.Undeclared(modScanResult.Capabilities, modManifest.Capabilities);
		stringBuilder.AppendLine((list2.Count == 0) ? "  manifest covers everything detected" : ("  UNDECLARED (enable would be refused): " + string.Join(", ", list2)));
		if (modScanResult.HardFlags.Count > 0)
		{
			stringBuilder.AppendLine($"  HARD FLAGS ({modScanResult.HardFlags.Count}, enable would be refused):");
			foreach (ModScanFinding hardFlag in modScanResult.HardFlags)
			{
				stringBuilder.AppendLine("    " + hardFlag);
			}
		}
		if (modScanResult.Findings.Count > 0)
		{
			stringBuilder.AppendLine($"  findings ({modScanResult.Findings.Count}):");
			foreach (ModScanFinding finding in modScanResult.Findings)
			{
				stringBuilder.AppendLine("    " + finding);
			}
		}
		ModFingerprint modFingerprint = ModFingerprint.Compute(text, modManifest);
		stringBuilder.AppendLine("  fingerprint: " + modFingerprint.Combined);
		foreach (var (text2, text3) in modFingerprint.Files)
		{
			stringBuilder.AppendLine("    " + text2 + ": " + text3);
		}
		ulong num = loadedMod?.WorkshopId ?? 0;
		ulong ownerSteamId;
		ulong ownerSteamId2 = ((num != 0L && ModWorkshop.TryGetOwner(num, out ownerSteamId)) ? ownerSteamId : 0);
		string reason;
		ModListVerdict modListVerdict = ModTrustList.Check(modManifest.ID, modManifest.Author, modFingerprint, num, ownerSteamId2, out reason);
		stringBuilder.AppendLine(string.Format("  trust list: {0}{1}", modListVerdict, (modListVerdict == ModListVerdict.Blocked) ? (" (" + reason + ")") : ""));
		return stringBuilder.ToString();
	}

	[Command("mod-trust", "Lists the trust records on this computer (what the player approved, with hashes and capabilities), or one mod's record", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Trust(string id = null)
	{
		StringBuilder stringBuilder = new StringBuilder();
		IEnumerable<ModTrustRecord> enumerable = ModTrust.Records;
		if (!string.IsNullOrEmpty(id))
		{
			enumerable = enumerable.Where((ModTrustRecord r) => r.id == id);
		}
		int num = 0;
		foreach (ModTrustRecord item in enumerable)
		{
			num++;
			stringBuilder.AppendLine(item.id + "  source=" + item.source + " workshop=" + item.workshopId + " dev=" + (item.developerMode ? "on" : "off"));
			stringBuilder.AppendLine("  approved " + item.approvedAt + "  updated " + item.updatedAt + "  game " + item.gameVersion);
			stringBuilder.AppendLine("  declared: " + ModCapability.FormatIds(item.declared) + "   detected: " + ModCapability.FormatIds(item.detected));
			stringBuilder.AppendLine("  hash: " + item.combinedHash);
			foreach (string file in item.files)
			{
				stringBuilder.AppendLine("    " + file);
			}
		}
		if (num == 0)
		{
			stringBuilder.AppendLine(string.IsNullOrEmpty(id) ? "No trust records." : ("No trust record for '" + id + "'."));
		}
		stringBuilder.AppendLine("Mods this session (workshop item id, publisher Steam id and manifest author are what the trust list's blockedWorkshopItems / blockedPublishers / blockedAuthors take):");
		foreach (LoadedMod mod in ModLoader.Mods)
		{
			ulong ownerSteamId;
			string text = ((mod.WorkshopId != 0L && ModWorkshop.TryGetOwner(mod.WorkshopId, out ownerSteamId)) ? ownerSteamId.ToString() : ((mod.WorkshopId != 0L) ? "unknown yet" : "-"));
			stringBuilder.AppendLine(string.Format("  {0}: {1}{2}{3}  workshop={4} publisher={5} author=\"{6}\"", mod.Manifest.ID, mod.TrustState, (mod.ListVerdict != 0) ? $", {mod.ListVerdict}" : "", (mod.TrustNote != null) ? (" (" + mod.TrustNote + ")") : "", mod.WorkshopId, text, mod.Manifest.Author));
		}
		return stringBuilder.ToString();
	}

	[Command("mod-steam-guard", "Shows the runtime Steam tripwire (methods guarded, calls blocked this session) and the mod versions it stopped on this computer", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string SteamGuard()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"{ModSteamGuard.GuardedCount} Steam method(s) guarded, {ModSteamGuard.BlockedCount} call(s) blocked this session.");
		foreach (ModTrust.StoppedEntry item in ModTrust.Stopped)
		{
			stringBuilder.AppendLine("  stopped " + item.id + " at " + item.at + ": " + item.reason + "  hash=" + item.hash);
		}
		return stringBuilder.ToString();
	}

	[Command("mod-trust-list", "Shows the state of the remote trust list (issued date, counts, kill switch, last fetch error)", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string TrustList()
	{
		return ModTrustList.Describe();
	}

	[Command("mod-trust-list-load", "devMode only: loads an unsigned trust list payload from a file for testing; not cached", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string TrustListLoad(string path)
	{
		if (!File.Exists(path))
		{
			return "No such file: " + path;
		}
		if (!ModTrustList.LoadUnsigned(path, out var error))
		{
			return "Not loaded: " + error;
		}
		return "Loaded (unsigned, this session only). " + ModTrustList.Describe();
	}

	[Command("mod-trust-list-fetch", "Fetches the remote trust list now instead of waiting for the next start", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string TrustListFetch()
	{
		if (!ModTrustList.IsConfigured)
		{
			return "Remote list disabled (no public key configured).";
		}
		if (ModLoader.Instance == null)
		{
			return "ModLoader is not running.";
		}
		ModLoader.Instance.StartCoroutine(ModTrustList.Fetch());
		return "Fetching... (mod-trust-list shows the result)";
	}

	[Command("mod-preview-warning", "Shows the mod disclaimer even if it was accepted on this computer; confirming it records nothing", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string PreviewWarning()
	{
		Transform transform = PreviewCanvasRoot();
		if (transform == null)
		{
			return "No main menu canvas to show it on: run this from the main menu.";
		}
		if (!ModWarningUI.ShowPreview(transform))
		{
			return "Not shown: no CustomUIManager in this scene.";
		}
		return "Disclaimer shown (preview: confirming records nothing).";
	}

	private static Transform PreviewCanvasRoot()
	{
		Canvas canvas = null;
		if (ModUIManager.Instance != null && ModUIManager.Instance.panel != null)
		{
			canvas = ModUIManager.Instance.panel.GetComponentInParent<Canvas>(includeInactive: true);
		}
		ConfirmPanelUIManager confirmPanelUIManager = ((MainMenuUIController.Instance != null) ? MainMenuUIController.Instance.confirmPanelUIManager : null);
		if (canvas == null && confirmPanelUIManager != null && confirmPanelUIManager.panel != null)
		{
			canvas = confirmPanelUIManager.panel.GetComponentInParent<Canvas>(includeInactive: true);
		}
		if (!(canvas != null))
		{
			return null;
		}
		return canvas.rootCanvas.transform;
	}

	[Command("mod-preview-refusal", "Shows the refusal dialog: with a mod id, that mod's real activation check (nothing is approved); without one, a sample scanner refusal. Copy log works", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string PreviewRefusal(string modId = "")
	{
		Transform transform = PreviewCanvasRoot();
		if (transform == null)
		{
			return "No main menu canvas to show it on: run this from the main menu.";
		}
		if (string.IsNullOrEmpty(modId))
		{
			string localizedUIOrDefault = LocHelper.GetLocalizedUIOrDefault("ui_mods_enable_refused_flags", "{0} was refused by the mod scanner:\n{1}", "Sample Mod", "[process-start] SampleMod.Init: System.Diagnostics.Process.Start");
			string log = "Probably Stolen: mod activation refused (preview sample)\nMod: Sample Mod (sample_mod), version 1.0\n\nReason:\n" + localizedUIOrDefault;
			if (!ModRefusalUI.Show(transform, "Sample Mod", localizedUIOrDefault, log))
			{
				return "Not shown: no CustomUIManager in this scene.";
			}
			return "Refusal shown (sample).";
		}
		LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId);
		if (loadedMod == null)
		{
			return "No mod with the id '" + modId + "'.";
		}
		ModEnableCheck modEnableCheck = ModLoader.CheckEnable(loadedMod);
		if (modEnableCheck.Refusal == null)
		{
			return "'" + modId + "' would not be refused: it would " + (modEnableCheck.NeedsPrompt ? "ask for its capabilities" : "be enabled directly") + ".";
		}
		if (!ModRefusalUI.Show(transform, ModLoader.GetModLabel(loadedMod.Manifest), modEnableCheck.Refusal, ModRefusalUI.BuildLog(modEnableCheck)))
		{
			return "Not shown: no CustomUIManager in this scene.";
		}
		return "Refusal shown for '" + modId + "' (nothing approved).";
	}

	[Command("mod-preview-prompt", "Shows the capability confirmation for a sample mod; capabilities separated by spaces or commas (in quotes), all of them when omitted or \"\"; then the Workshop upload age in days (fractions allowed) and the number of ratings (no age: installed by hand). Activate approves nothing", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string PreviewPrompt(string capabilities = "", float uploadedDaysAgo = -1f, int ratings = 0)
	{
		Transform transform = PreviewCanvasRoot();
		if (transform == null)
		{
			return "No main menu canvas to show it on: run this from the main menu.";
		}
		List<string> list = (string.IsNullOrWhiteSpace(capabilities) ? ModCapability.All.ToList() : capabilities.Split(new char[2] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
		List<string> list2 = list.Where((string c) => !ModCapability.All.Contains(c)).ToList();
		if (list2.Count > 0)
		{
			return "Unknown capability: " + string.Join(", ", list2) + ". Known: " + string.Join(", ", ModCapability.All);
		}
		ModWorkshop.ItemStats? preview = null;
		if (uploadedDaysAgo >= 0f)
		{
			preview = new ModWorkshop.ItemStats
			{
				createdUtc = DateTime.UtcNow.AddDays(0f - uploadedDaysAgo),
				votesUp = (uint)Mathf.Max(0, ratings)
			};
		}
		if (!ModEnablePromptUI.Show(transform, "Sample Mod", list, 0uL, null, preview))
		{
			return "Not shown: no CustomUIManager in this scene.";
		}
		return "Confirmation shown (preview: Activate approves nothing).";
	}
}

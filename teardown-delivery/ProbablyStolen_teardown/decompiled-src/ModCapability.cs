using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ModCapability
{
	public enum Risk
	{
		Low,
		Medium,
		High,
		Severe
	}

	public const string Files = "files";

	public const string Network = "network";

	public const string Process = "process";

	public const string DynamicCode = "dynamic-code";

	public const string Native = "native";

	public const string ReflectionExternal = "reflection-external";

	public const string Steam = "steam";

	public const string SteamAuth = "steam-auth";

	public static readonly string[] All = new string[8] { "files", "network", "process", "dynamic-code", "native", "reflection-external", "steam", "steam-auth" };

	public static bool IsKnown(string capability)
	{
		return All.Contains(capability);
	}

	public static string DisplayName(string capability)
	{
		return capability switch
		{
			"files" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_files", "Read and write files anywhere on this computer"), 
			"network" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_network", "Access the internet"), 
			"process" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_links", "Open web links in your browser"), 
			"dynamic-code" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_dynamic_code", "Load or generate code at runtime"), 
			"native" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_native", "Call native system libraries"), 
			"reflection-external" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_reflection_external", "Reach into system and engine internals through reflection"), 
			"steam" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_steam", "Use Steam features with your account (achievements, friends, overlay)"), 
			"steam-auth" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_steam_auth", "Prove that you own the game to this mod's online service"), 
			_ => capability, 
		};
	}

	public static Risk RiskOf(string capability)
	{
		switch (capability)
		{
		case "native":
		case "dynamic-code":
		case "files":
		case "reflection-external":
		case "network":
			return Risk.Severe;
		case "steam-auth":
			return Risk.High;
		case "steam":
		case "process":
			return Risk.Medium;
		default:
			return Risk.Severe;
		}
	}

	public static string FormatList(IEnumerable<string> capabilities)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in Order(capabilities))
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append('\n');
			}
			stringBuilder.Append("- ").Append(DisplayName(item));
		}
		return stringBuilder.ToString();
	}

	public static string FormatIds(IEnumerable<string> capabilities)
	{
		List<string> list = Order(capabilities).ToList();
		if (list.Count != 0)
		{
			return string.Join(", ", list);
		}
		return "none";
	}

	public static IEnumerable<string> Order(IEnumerable<string> capabilities)
	{
		HashSet<string> hashSet = new HashSet<string>(capabilities ?? Enumerable.Empty<string>());
		return All.Where(hashSet.Contains).Concat(from c in hashSet
			where !IsKnown(c)
			orderby c
			select c);
	}

	public static List<string> Undeclared(IEnumerable<string> detected, IEnumerable<string> declared)
	{
		HashSet<string> declaredSet = new HashSet<string>(declared ?? Enumerable.Empty<string>());
		return (from c in Order(detected)
			where !declaredSet.Contains(c)
			select c).ToList();
	}
}

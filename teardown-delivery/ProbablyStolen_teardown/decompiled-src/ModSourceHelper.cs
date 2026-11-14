using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

public static class ModSourceHelper
{
	private static readonly Dictionary<Assembly, LoadedMod> modByAssembly = new Dictionary<Assembly, LoadedMod>();

	private static int modByAssemblyCount = -1;

	public static LoadedMod FindModByAssembly(Assembly assembly)
	{
		if (assembly == null)
		{
			return null;
		}
		if (modByAssemblyCount != ModLoader.Mods.Count)
		{
			modByAssembly.Clear();
			modByAssemblyCount = ModLoader.Mods.Count;
		}
		if (modByAssembly.TryGetValue(assembly, out var value))
		{
			return value;
		}
		LoadedMod loadedMod = null;
		foreach (LoadedMod mod in ModLoader.Mods)
		{
			if (mod.Assembly == assembly)
			{
				loadedMod = mod;
				break;
			}
		}
		if (loadedMod == null)
		{
			loadedMod = FindModByLocation(assembly);
		}
		modByAssembly[assembly] = loadedMod;
		return loadedMod;
	}

	private static LoadedMod FindModByLocation(Assembly assembly)
	{
		try
		{
			if (assembly.IsDynamic || string.IsNullOrEmpty(assembly.Location))
			{
				return null;
			}
			string fullPath = Path.GetFullPath(assembly.Location);
			foreach (LoadedMod mod in ModLoader.Mods)
			{
				if (!(mod.Assembly == null) && !string.IsNullOrEmpty(mod.FolderPath))
				{
					string value = Path.GetFullPath(mod.FolderPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
					if (fullPath.StartsWith(value, StringComparison.OrdinalIgnoreCase))
					{
						return mod;
					}
				}
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	public static LoadedMod FindModByDelegate(Delegate handler)
	{
		if ((object)handler != null)
		{
			return FindModByAssembly(handler.Method.DeclaringType?.Assembly);
		}
		return null;
	}

	public static string GetModDisplayName(string modId)
	{
		if (string.IsNullOrEmpty(modId))
		{
			return null;
		}
		foreach (LoadedMod mod in ModLoader.Mods)
		{
			if (mod.Manifest != null && mod.Manifest.ID == modId)
			{
				return ModLoader.GetModLabel(mod.Manifest);
			}
		}
		return null;
	}

	public static string GetItemModName(GameItem item)
	{
		if (item == null)
		{
			return null;
		}
		if (item.IsTag("MOD_ID_TAG"))
		{
			string @string = item.GetTagReadonly("MOD_ID_TAG").GetString();
			if (!string.IsNullOrEmpty(@string))
			{
				string text = (item.IsTag("MOD_NAME_TAG") ? item.GetTagReadonly("MOD_NAME_TAG").GetString() : null);
				string text2 = GetModDisplayName(@string);
				if (text2 == null)
				{
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
					text2 = @string;
				}
				return text2;
			}
		}
		if (ModLoader.Mods.Count == 0)
		{
			return null;
		}
		LoadedMod loadedMod = FindModByDelegate(DirectoryMaster.GetFactory<GameItem>(item.identifier));
		if (loadedMod == null)
		{
			return null;
		}
		return ModLoader.GetModLabel(loadedMod.Manifest);
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (builder != null && EmporiumOption.ShowModSourceInTooltip)
		{
			string itemModName = GetItemModName(item);
			itemModName = (string.IsNullOrEmpty(itemModName) ? null : Regex.Replace(itemModName, "<[^>]*>", "").Trim());
			if (!string.IsNullOrEmpty(itemModName))
			{
				builder.AddLine(itemModName, ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue, bold: false, italic: true);
			}
		}
	}

	public static void MarkNewClients(List<StoreClient> clientStack, HashSet<StoreClient> before, Delegate handler)
	{
		if (clientStack == null || before == null)
		{
			return;
		}
		LoadedMod loadedMod = FindModByDelegate(handler);
		if (loadedMod == null || loadedMod.Manifest == null)
		{
			return;
		}
		foreach (StoreClient item in clientStack)
		{
			if (item != null && !before.Contains(item) && item.sourceModId == null)
			{
				item.sourceModId = loadedMod.Manifest.ID;
			}
		}
	}

	public static void MarkFromFactory(StoreClient client, Delegate factory)
	{
		if (client != null && client.sourceModId == null && ModLoader.Mods.Count != 0)
		{
			LoadedMod loadedMod = FindModByDelegate(factory);
			if (loadedMod != null && loadedMod.Manifest != null)
			{
				client.sourceModId = loadedMod.Manifest.ID;
			}
		}
	}

	public static string GetClientModName(StoreClient client)
	{
		if (client == null || string.IsNullOrEmpty(client.sourceModId))
		{
			return null;
		}
		return GetModDisplayName(client.sourceModId) ?? client.sourceModId;
	}

	public static void RecordClientSource(StoreClient client)
	{
		if (EmporiumOption.ShowModSourceInRecorder && !(DialogUIManager.Instance == null))
		{
			string clientModName = GetClientModName(client);
			if (!string.IsNullOrEmpty(clientModName))
			{
				DialogUIManager.Instance.UpdateDialogHistory("<i>" + LocHelper.GetLocalizedUI("ui_recorder_from_mod", clientModName) + "</i>");
			}
		}
	}
}

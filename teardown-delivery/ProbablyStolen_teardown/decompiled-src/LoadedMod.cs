using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

public class LoadedMod
{
	public IMod Instance;

	public ModManifest Manifest;

	public Assembly Assembly;

	public Harmony HarmonyInstance;

	public string FolderPath;

	public string DllPath;

	public bool IsEnabled;

	public string LoadError;

	public ModSource Source;

	public ulong WorkshopId;

	public ulong ShadowedWorkshopId;

	public ModTrustState TrustState;

	public string TrustNote;

	public ModFingerprint Fingerprint;

	public ModTrustRecord TrustRecord;

	public ModListVerdict ListVerdict;

	public ModScanResult LastScan;

	public List<string> UnlistedDlls = new List<string>();

	public Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

	public Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

	public Dictionary<string, Sprite> PaperSprites = new Dictionary<string, Sprite>();

	public Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>();

	public Dictionary<string, Dictionary<string, string>> Localization = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
}

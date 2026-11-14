using UnityEngine;

public class ModLog
{
	private readonly string modName;

	public ModLog(ModManifest manifest)
	{
		modName = manifest.Name;
	}

	public void Log(object message)
	{
		UnityEngine.Debug.Log($"[{modName}]: {message}");
	}

	public void Warning(object message)
	{
		Debug.LogWarning($"[{modName}]: {message}");
	}

	public void Error(object message)
	{
		Debug.LogError($"[{modName}]: {message}");
	}
}

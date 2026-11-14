using QFSW.QC;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DevConsoleStripper
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Init()
	{
		if (!DevTools.IsEnabled)
		{
			SceneManager.sceneLoaded += delegate
			{
				DestroyConsoles();
			};
			DestroyConsoles();
		}
	}

	private static void DestroyConsoles()
	{
		QuantumConsole[] array = Object.FindObjectsOfType<QuantumConsole>(includeInactive: true);
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
	}
}

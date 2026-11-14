using UnityEngine;

public class SpriteAnimationManager : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Bootstrap()
	{
		GameObject obj = new GameObject("SpriteAnimationManager");
		Object.DontDestroyOnLoad(obj);
		obj.AddComponent<SpriteAnimationManager>();
	}

	private void Update()
	{
		SpriteAnimationHelper.Tick();
	}
}

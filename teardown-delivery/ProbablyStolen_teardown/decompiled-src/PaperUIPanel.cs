using UnityEngine;

public class PaperUIPanel : MonoBehaviour
{
	public PaperUI owner;

	private static bool quitting;

	private void OnApplicationQuit()
	{
		quitting = true;
	}

	private void OnDisable()
	{
		if (!quitting && owner != null && base.gameObject.scene.isLoaded)
		{
			owner.NotifyHidden();
		}
	}

	private void OnDestroy()
	{
		if (!quitting && owner != null)
		{
			owner.NotifyDestroyed(base.gameObject.activeSelf && base.gameObject.scene.isLoaded);
		}
	}
}

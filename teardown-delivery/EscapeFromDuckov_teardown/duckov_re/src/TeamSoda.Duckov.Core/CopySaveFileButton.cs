using System.IO;
using Saves;
using UnityEngine;
using UnityEngine.UI;

public class CopySaveFileButton : MonoBehaviour
{
	[SerializeField]
	private string fromFile = "Saves/Backup.sav";

	[SerializeField]
	private Button button;

	private void Awake()
	{
		button.onClick.AddListener(Apply);
	}

	public void Apply()
	{
		string text = Path.Combine(Application.streamingAssetsPath, fromFile);
		if (File.Exists(text))
		{
			ES3.CacheFile(text);
			ES3.DeleteFile(SavesSystem.CurrentFilePath);
			ES3.CopyFile(text, SavesSystem.CurrentFilePath);
			ES3.StoreCachedFile(SavesSystem.CurrentFilePath);
			SavesSystem.SetFile(SavesSystem.CurrentSlot);
		}
	}
}

using Duckov.UI;
using Saves;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseSceneUtilities : MonoBehaviour
{
	[SerializeField]
	private float saveCoolTime = 5f;

	private float saveCoolTimer = 5f;

	private bool dirty;

	private float lastTimeSaved = float.MinValue;

	private float TimeSinceLastSave => Time.realtimeSinceStartup - lastTimeSaved;

	private void Save()
	{
		LevelManager.Instance.SaveMainCharacter();
		SavesSystem.CollectSaveData();
		SavesSystem.SaveFile();
		lastTimeSaved = Time.realtimeSinceStartup;
	}

	private void Awake()
	{
		lastTimeSaved = Time.realtimeSinceStartup;
		ManagedUIElement.onOpen += OnViewOpenClose;
		ManagedUIElement.onClose += OnViewOpenClose;
	}

	private void OnDestroy()
	{
		ManagedUIElement.onOpen -= OnViewOpenClose;
		ManagedUIElement.onClose -= OnViewOpenClose;
	}

	private void Update()
	{
		if (LevelManager.LevelInited)
		{
			if (Keyboard.current.altKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
			{
				TrySetDirty();
			}
			if (saveCoolTimer > 0f)
			{
				saveCoolTimer -= Time.unscaledDeltaTime;
			}
			if (dirty && saveCoolTimer <= 0f)
			{
				Save();
				dirty = false;
				saveCoolTimer = saveCoolTime;
			}
		}
	}

	private void OnViewOpenClose(ManagedUIElement uiElement)
	{
		TrySetDirty();
	}

	private void TrySetDirty()
	{
		if (!(saveCoolTime - saveCoolTimer < 0.5f))
		{
			dirty = true;
		}
	}
}

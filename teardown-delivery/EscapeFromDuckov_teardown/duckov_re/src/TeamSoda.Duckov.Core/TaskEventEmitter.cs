using Cysharp.Threading.Tasks;
using UnityEngine;

public class TaskEventEmitter : MonoBehaviour
{
	[SerializeField]
	private string eventKey;

	[SerializeField]
	private bool emitOnAwake;

	private bool destroied;

	public void SetKey(string key)
	{
		eventKey = key;
	}

	private void OnDestroy()
	{
		destroied = true;
	}

	private void Awake()
	{
		if (emitOnAwake)
		{
			TryEmitEventOnAwake().Forget();
		}
	}

	private async UniTaskVoid TryEmitEventOnAwake()
	{
		while (!LevelManager.LevelInited)
		{
			if (base.gameObject == null || destroied)
			{
				return;
			}
			await UniTask.WaitForEndOfFrame();
		}
		EmitEvent();
	}

	public void EmitEvent()
	{
		TaskEvent.EmitTaskEvent(eventKey);
	}
}

using Cysharp.Threading.Tasks;
using Duckov;
using UnityEngine;

public class ExplosionProxy : MonoBehaviour
{
	public Transform exposionCenter;

	private bool triggered;

	private bool destroied;

	public float delay;

	public bool hasTriggerSound;

	public string triggerSoundKey;

	public float damageRange;

	public DamageInfo dmgInfo;

	public ExplosionFxTypes fxType;

	public GameObject customFx;

	[Range(0f, 1f)]
	public float explosionShakeStrength;

	public bool destroyOnExplode;

	private bool stopped;

	public void Stop()
	{
		stopped = true;
	}

	private void Awake()
	{
	}

	private void OnDestroy()
	{
		destroied = true;
	}

	private async UniTaskVoid StartExplode()
	{
		if (hasTriggerSound && triggerSoundKey != "")
		{
			AudioManager.Post(triggerSoundKey, base.gameObject);
		}
		await UniTask.WaitForSeconds(delay);
		if (!destroied && !(LevelManager.Instance == null) && !stopped)
		{
			DoExplode();
		}
	}

	private void DoExplode()
	{
		Vector3 position = base.transform.position;
		if ((bool)exposionCenter)
		{
			position = exposionCenter.position;
		}
		dmgInfo.isExplosion = true;
		LevelManager.Instance.ExplosionManager.CreateExplosion(position, damageRange, dmgInfo, fxType, explosionShakeStrength, canHurtSelf: false);
		if (destroyOnExplode)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void Trigger()
	{
		if (!triggered && !stopped)
		{
			triggered = true;
			StartExplode().Forget();
		}
	}

	public void ExplodeImmediately()
	{
		if (!triggered)
		{
			triggered = true;
			DoExplode();
		}
	}
}

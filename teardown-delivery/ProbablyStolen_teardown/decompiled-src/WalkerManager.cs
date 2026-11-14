using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerManager : MonoBehaviour
{
	public const float FAR_Y = 1.804f;

	public const float FAR_X_LEFT = -1.011f;

	public const float FAR_X_RIGHT = 1.011f;

	[Header("Far Walkers")]
	public List<GameObject> farWalkerPrefab;

	public Transform farWalkerParent;

	[Header("Far Pooling")]
	public int farPoolSize = 10;

	public const float MID_Y = 1.606f;

	public const float MID_X_LEFT = -1.534f;

	public const float MID_X_RIGHT = 1.49f;

	[Header("Mid Walkers")]
	public List<GameObject> midWalkerPrefab;

	public Transform midWalkerParent;

	[Header("Mid Pooling")]
	public int midPoolSize = 10;

	[Header("Spawn Timing")]
	public float minSpawnDelay = 2f;

	public float maxSpawnDelay = 5f;

	[Header("Speed")]
	public float farBaseSpeed = 0.2f;

	public float midBaseSpeed = 0.3f;

	[Range(0f, 0.5f)]
	public float speedVariance = 0.15f;

	private readonly Queue<Walker> farPool = new Queue<Walker>();

	private readonly Queue<Walker> midPool = new Queue<Walker>();

	public static WalkerManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		WarmFarPool();
		WarmMidPool();
		StartCoroutine(FarWalkerSpawner());
		StartCoroutine(MidWalkerSpawner());
	}

	private void WarmFarPool()
	{
		for (int i = 0; i < farPoolSize; i++)
		{
			Walker component = Object.Instantiate(farWalkerPrefab[Random.Range(0, farWalkerPrefab.Count)], farWalkerParent).GetComponent<Walker>();
			component.Deactivate();
			farPool.Enqueue(component);
		}
	}

	private Walker GetFarWalker()
	{
		if (farPool.Count > 0)
		{
			return farPool.Dequeue();
		}
		return Object.Instantiate(farWalkerPrefab[Random.Range(0, farWalkerPrefab.Count)], farWalkerParent).GetComponent<Walker>();
	}

	public void DespawnFarWalker(Walker walker)
	{
		walker.Deactivate();
		farPool.Enqueue(walker);
	}

	private void WarmMidPool()
	{
		for (int i = 0; i < midPoolSize; i++)
		{
			Walker component = Object.Instantiate(midWalkerPrefab[Random.Range(0, midWalkerPrefab.Count)], midWalkerParent).GetComponent<Walker>();
			component.Deactivate();
			midPool.Enqueue(component);
		}
	}

	private Walker GetMidWalker()
	{
		if (midPool.Count > 0)
		{
			return midPool.Dequeue();
		}
		return Object.Instantiate(midWalkerPrefab[Random.Range(0, midWalkerPrefab.Count)], midWalkerParent).GetComponent<Walker>();
	}

	public void DespawnMidWalker(Walker walker)
	{
		walker.Deactivate();
		midPool.Enqueue(walker);
	}

	private IEnumerator FarWalkerSpawner()
	{
		while (true)
		{
			float spawnRateMultiplier = GetSpawnRateMultiplier();
			if (!farWalkerParent.gameObject.activeInHierarchy)
			{
				yield return new WaitForSeconds(1f);
			}
			else if (spawnRateMultiplier > 0f)
			{
				SpawnFarWalker();
				float seconds = Random.Range(minSpawnDelay, maxSpawnDelay) / spawnRateMultiplier;
				yield return new WaitForSeconds(seconds);
			}
			else
			{
				yield return new WaitForSeconds(1f);
			}
		}
	}

	private void SpawnFarWalker()
	{
		Walker farWalker = GetFarWalker();
		int num = ((Random.value > 0.5f) ? 1 : (-1));
		float x = ((num == 1) ? (-1.011f) : 1.011f);
		farWalker.transform.position = new Vector3(x, 1.804f, 0f);
		farWalker.Init(this, farBaseSpeed, speedVariance, num, Walker.WalkerLayer.Far);
	}

	private IEnumerator MidWalkerSpawner()
	{
		while (true)
		{
			float spawnRateMultiplier = GetSpawnRateMultiplier();
			if (!midWalkerParent.gameObject.activeInHierarchy)
			{
				yield return new WaitForSeconds(1f);
			}
			else if (spawnRateMultiplier > 0f)
			{
				SpawnMidWalker();
				float seconds = Random.Range(minSpawnDelay, maxSpawnDelay) / spawnRateMultiplier;
				yield return new WaitForSeconds(seconds);
			}
			else
			{
				yield return new WaitForSeconds(1f);
			}
		}
	}

	private void SpawnMidWalker()
	{
		Walker midWalker = GetMidWalker();
		int num = ((Random.value > 0.5f) ? 1 : (-1));
		float x = ((num == 1) ? (-1.534f) : 1.49f);
		midWalker.transform.position = new Vector3(x, 1.606f, 0f);
		midWalker.Init(this, midBaseSpeed, speedVariance, num, Walker.WalkerLayer.Mid);
	}

	public bool IsOutOfFarBounds(float x)
	{
		if (!(x < -1.2110001f))
		{
			return x > 1.2110001f;
		}
		return true;
	}

	public bool IsOutOfMidBounds(float x)
	{
		if (!(x < -1.7340001f))
		{
			return x > 1.69f;
		}
		return true;
	}

	public int GetCustomerCount()
	{
		return PlayerStore.Instance.storeClientManager.clientStack.Count;
	}

	private float GetSpawnRateMultiplier()
	{
		int customerCount = GetCustomerCount();
		if (customerCount <= 0)
		{
			return 0f;
		}
		if (customerCount <= 2)
		{
			return 0.25f;
		}
		return 1f;
	}
}

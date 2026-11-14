using UnityEngine;

public class RogueClosureManager : MonoBehaviour
{
	public int enemyKilledCount;

	public int bossKilledCount;

	private static RogueClosureManager _instance;

	public static RogueClosureManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType<RogueClosureManager>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
		Health.OnDead += OnHealthDead;
	}

	public void OnDestroy()
	{
		Health.OnDead -= OnHealthDead;
	}

	private void OnHealthDead(Health health, DamageInfo dmgInfo)
	{
		if ((bool)dmgInfo.fromCharacter && dmgInfo.fromCharacter.IsMainCharacter)
		{
			CharacterMainControl characterMainControl = health.TryGetCharacter();
			if ((bool)characterMainControl)
			{
				ReportKilled(characterMainControl.isBossCharacter);
			}
		}
	}

	public void ReportKilled(bool isBoss)
	{
		enemyKilledCount++;
		if (isBoss)
		{
			bossKilledCount++;
		}
	}
}

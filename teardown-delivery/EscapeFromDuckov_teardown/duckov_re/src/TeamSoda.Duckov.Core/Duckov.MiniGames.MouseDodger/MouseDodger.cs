using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Duckov.Utilities;
using Saves;
using TMPro;
using UnityEngine;

namespace Duckov.MiniGames.MouseDodger;

public class MouseDodger : MiniGameBehaviour
{
	public enum Status
	{
		None,
		Title,
		Gameplay,
		GameOver
	}

	[Serializable]
	public struct RunData
	{
		public int hp;

		public int score;

		public float gameTime;

		public float goalSpawnBuffer;

		public float obstacleSpawnBuffer;

		public float invincibleBuffer;

		public static RunData New(int hp)
		{
			return new RunData
			{
				hp = hp,
				score = 0
			};
		}
	}

	public MouseDodgerSettings settings;

	[SerializeField]
	private FadeGroup titleScreenFadeGroup;

	[SerializeField]
	private FadeGroup gameOverScreenFaderGroup;

	[SerializeField]
	private List<ParticleSystem> goalFXs;

	[SerializeField]
	private List<ParticleSystem> damageFXs;

	[SerializeField]
	private ParticleSystem moveFX;

	[SerializeField]
	private ParticleSystem deadFX;

	[SerializeField]
	private TextMeshProUGUI mouseSensitivityText;

	[SerializeField]
	private FadeGroup gameplayFadeGroup;

	[SerializeField]
	private List<GameObject> healthHearts;

	[SerializeField]
	private TextMeshProUGUI scoreText;

	[SerializeField]
	private TextMeshProUGUI gameOverScoreText;

	[SerializeField]
	private GameObject gameOverHighscoreIndicator;

	private MouseDodgerEntity mainEntity;

	private List<MouseDodgerEntity> entities = new List<MouseDodgerEntity>();

	public Status status;

	private RunData runData;

	public bool forceGameOver;

	private bool isHighScore;

	private const string HighScoreSaveKey = "MiniGame/MouseDodge/HighScore";

	private const string MouseSensitivitySaveKey = "MiniGame/MouseDodge/MouseSensitivity";

	private bool gameObjectDestroied;

	private bool Invincible
	{
		get
		{
			if (runData.invincibleBuffer > 0f)
			{
				return true;
			}
			return false;
		}
	}

	public static int HighScore
	{
		get
		{
			return SavesSystem.Load<int>("MiniGame/MouseDodge/HighScore");
		}
		set
		{
			SavesSystem.Save("MiniGame/MouseDodge/HighScore", value);
		}
	}

	private static int MouseSensitivity
	{
		get
		{
			if (SavesSystem.KeyExisits("MiniGame/MouseDodge/MouseSensitivity"))
			{
				return SavesSystem.Load<int>("MiniGame/MouseDodge/MouseSensitivity");
			}
			return 5;
		}
		set
		{
			int num = value;
			if (num < 1)
			{
				num = 1;
			}
			if (num > 9)
			{
				num = 9;
			}
			SavesSystem.Save("MiniGame/MouseDodge/MouseSensitivity", num);
		}
	}

	private void Clear()
	{
		isHighScore = false;
		if (mainEntity != null)
		{
			UnityEngine.Object.Destroy(mainEntity.gameObject);
		}
		foreach (MouseDodgerEntity entity in entities)
		{
			UnityEngine.Object.Destroy(entity.gameObject);
		}
		mainEntity = null;
		entities.Clear();
	}

	protected override void Start()
	{
		base.Start();
		gameplayFadeGroup.SkipHide();
		gameOverScreenFaderGroup.SkipHide();
		GameProcess().Forget();
	}

	protected override void OnUpdate(float deltaTime)
	{
		base.OnUpdate(deltaTime);
		if (base.Game.GetButtonDown(MiniGame.Button.Down))
		{
			MouseSensitivity--;
			RefreshMouseSensitivity();
		}
		if (base.Game.GetButtonDown(MiniGame.Button.Up))
		{
			MouseSensitivity++;
			RefreshMouseSensitivity();
		}
	}

	public async UniTask GameProcess()
	{
		RefreshMouseSensitivity();
		while (!CheckAbortion())
		{
			ChangeStatus(Status.Title);
			await TitleUIIn();
			if (CheckAbortion())
			{
				break;
			}
			await WaitForConfirmInteract();
			if (CheckAbortion())
			{
				break;
			}
			await TitleUIOut();
			if (!CheckAbortion())
			{
				ChangeStatus(Status.Gameplay);
				await Gameplay();
				if (!CheckAbortion())
				{
					ChangeStatus(Status.GameOver);
					await GameOverUIIn();
					if (CheckAbortion())
					{
						break;
					}
					await WaitForConfirmInteract();
					if (CheckAbortion())
					{
						break;
					}
					await GameOverUIOut();
					if (CheckAbortion())
					{
						break;
					}
					continue;
				}
				break;
			}
			break;
		}
	}

	private async UniTask TitleUIIn()
	{
		await titleScreenFadeGroup.ShowAndReturnTask();
	}

	private async UniTask TitleUIOut()
	{
		await titleScreenFadeGroup.HideAndReturnTask();
	}

	private async UniTask GameOverUIIn()
	{
		gameOverScoreText.text = runData.score.ToString();
		gameOverHighscoreIndicator.SetActive(isHighScore);
		if (isHighScore && runData.score > HighScore)
		{
			HighScore = runData.score;
		}
		await gameOverScreenFaderGroup.ShowAndReturnTask();
	}

	private async UniTask GameOverUIOut()
	{
		await gameOverScreenFaderGroup.HideAndReturnTask();
	}

	private async UniTask WaitForConfirmInteract()
	{
		while (!base.Game.GetButtonDown(MiniGame.Button.A) && !base.Game.GetButtonDown(MiniGame.Button.Start))
		{
			await UniTask.Yield();
			if (CheckAbortion())
			{
				break;
			}
		}
	}

	private void RefreshMouseSensitivity()
	{
		mouseSensitivityText.text = MouseSensitivity.ToString();
	}

	private bool CheckAbortion()
	{
		if (gameObjectDestroied)
		{
			return true;
		}
		return false;
	}

	private async UniTask Gameplay()
	{
		Clear();
		mainEntity = MouseDodgerEntity.Create(settings.mainSprite, settings.mainRadius, base.transform, OnMainEnter);
		UnityEngine.Object.Instantiate(moveFX, mainEntity.transform, worldPositionStays: false).transform.localPosition = Vector3.zero;
		mainEntity.rigidbody.isKinematic = true;
		mainEntity.transform.localPosition = Vector3.zero;
		runData = RunData.New(settings.maxHP);
		forceGameOver = false;
		RefreshGameplayUI();
		gameplayFadeGroup.Show();
		while (!CheckGameOver())
		{
			runData.gameTime += Time.deltaTime;
			UpdateEntitySpawner();
			UpdateGameplay();
			await UniTask.Yield();
		}
		if (!(mainEntity == null))
		{
			UnityEngine.Object.Instantiate(deadFX, mainEntity.transform, worldPositionStays: false).transform.localPosition = Vector3.zero;
			mainEntity.rigidbody.isKinematic = false;
			gameplayFadeGroup.Hide();
		}
	}

	private void RefreshGameplayUI()
	{
		scoreText.text = runData.score.ToString();
		for (int i = 0; i < healthHearts.Count; i++)
		{
			healthHearts[i].SetActive(i < runData.hp);
		}
	}

	private bool CheckGameOver()
	{
		if (gameObjectDestroied)
		{
			return true;
		}
		if (forceGameOver)
		{
			return true;
		}
		if (runData.hp <= 0)
		{
			return true;
		}
		if (runData.score >= int.MaxValue)
		{
			return true;
		}
		return false;
	}

	private void ChangeStatus(Status newStatus)
	{
		status = newStatus;
	}

	private void UpdateGameplay()
	{
		if (mainEntity == null)
		{
			return;
		}
		Vector2 vector = UIInputManager.MouseDelta * settings.moveSpeed * MouseSensitivity * 0.1f;
		mainEntity.rigidbody.velocity = vector;
		Vector2 position = mainEntity.rigidbody.position + vector * Time.deltaTime;
		position.x = Mathf.Clamp(position.x, settings.bounds.min.x, settings.bounds.max.x);
		position.y = Mathf.Clamp(position.y, settings.bounds.min.y, settings.bounds.max.y);
		mainEntity.rigidbody.position = position;
		if (vector.magnitude > 0.1f)
		{
			float rotation = mainEntity.rigidbody.rotation;
			float num = Vector2.SignedAngle(Vector2.up, vector.normalized);
			if (Mathf.Abs(num - rotation) > 180f)
			{
				num = ((!(num > rotation)) ? (num + 360f) : (num - 360f));
			}
			mainEntity.rigidbody.rotation = Mathf.Lerp(rotation, num, 0.1f);
		}
		runData.invincibleBuffer = Mathf.MoveTowards(runData.invincibleBuffer, -0.01f, Time.deltaTime);
		if (runData.invincibleBuffer > 0f)
		{
			mainEntity.spriteRenderer.color = new Color(1f, 1f, 1f, (Mathf.Sin(Time.time * 50f) > 0f) ? 1f : 0f);
		}
		else
		{
			mainEntity.spriteRenderer.color = Color.white;
		}
		List<MouseDodgerEntity> list = new List<MouseDodgerEntity>();
		foreach (MouseDodgerEntity entity in entities)
		{
			if (CheckOutOfBound(entity))
			{
				list.Add(entity);
			}
		}
		foreach (MouseDodgerEntity item in list)
		{
			Release(item);
		}
	}

	private bool CheckOutOfBound(MouseDodgerEntity entity)
	{
		float x = settings.bounds.extents.x;
		float y = settings.bounds.extents.y;
		float num = Mathf.Sqrt(x * x + y * y);
		return entity.rigidbody.position.magnitude > num + 20f;
	}

	private void Release(MouseDodgerEntity entity)
	{
		entities.Remove(entity);
		UnityEngine.Object.Destroy(entity.gameObject);
	}

	private void UpdateEntitySpawner()
	{
		if (entities.Count <= settings.maxEntityCount)
		{
			float goalSpawnRate = settings.goalSpawnRate;
			runData.goalSpawnBuffer += Time.deltaTime * goalSpawnRate;
			if (runData.goalSpawnBuffer > 1f)
			{
				runData.goalSpawnBuffer -= 1f;
				SpawnGoal();
			}
			float obstaleSpawnRate = settings.obstaleSpawnRate;
			runData.obstacleSpawnBuffer += Time.deltaTime * obstaleSpawnRate;
			if (runData.obstacleSpawnBuffer > 1f)
			{
				runData.obstacleSpawnBuffer -= 1f;
				SpawnObstacle();
			}
		}
	}

	private void SpawnGoal()
	{
		MouseDodgerEntity mouseDodgerEntity = MouseDodgerEntity.Create(settings.goalSprites.GetRandom(), settings.mainRadius, base.transform);
		mouseDodgerEntity.transform.position = RandomSpawnPosition();
		mouseDodgerEntity.score = 100;
		mouseDodgerEntity.rigidbody.velocity = UnityEngine.Random.insideUnitCircle * settings.maxEntitySpeed;
		entities.Add(mouseDodgerEntity);
	}

	private Vector3 RandomSpawnPosition()
	{
		float x = settings.bounds.extents.x;
		float y = settings.bounds.extents.y;
		float num = Mathf.Sqrt(x * x + y * y) + 10f;
		float f = UnityEngine.Random.Range(0f, 1f) * MathF.PI * 2f;
		float x2 = num * Mathf.Sin(f);
		float y2 = num * Mathf.Cos(f);
		return new Vector3(x2, y2);
	}

	private void SpawnObstacle()
	{
		MouseDodgerEntity mouseDodgerEntity = MouseDodgerEntity.Create(settings.obstacleSprites.GetRandom(), settings.mainRadius, base.transform);
		mouseDodgerEntity.transform.position = RandomSpawnPosition();
		float num = UnityEngine.Random.Range(settings.obstacleScaleRange.x, settings.obstacleScaleRange.y);
		mouseDodgerEntity.transform.localScale = num * Vector3.one;
		mouseDodgerEntity.damage = 1;
		mouseDodgerEntity.rigidbody.velocity = UnityEngine.Random.insideUnitCircle * settings.maxEntitySpeed;
		mouseDodgerEntity.rigidbody.mass = num * num * num * 1f;
		entities.Add(mouseDodgerEntity);
	}

	private void OnMainEnter(MouseDodgerEntity self, Collision2D other)
	{
		if (status == Status.Gameplay)
		{
			MouseDodgerEntity component = other.gameObject.GetComponent<MouseDodgerEntity>();
			if (component.score > 0)
			{
				Score(component.score);
				Release(component);
				ParticleSystem random = goalFXs.GetRandom();
				PlayFX(random, component.rigidbody.position);
			}
			if (component.damage != 0 && !Invincible)
			{
				Damage(component.damage);
				PlayFX(damageFXs.GetRandom(), other.contacts[0].point);
			}
		}
	}

	private void PlayFX(ParticleSystem fx, Vector3 worldPosition)
	{
		if (!(fx == null))
		{
			FXPool.Play(fx, worldPosition, Quaternion.identity);
		}
	}

	private void Score(int score)
	{
		try
		{
			int num = checked(runData.score + score);
			runData.score = num;
			if (num > HighScore)
			{
				isHighScore = true;
			}
		}
		catch
		{
			runData.score = int.MaxValue;
		}
		RefreshGameplayUI();
	}

	private void Damage(int damage)
	{
		runData.hp -= damage;
		if (runData.hp < 0)
		{
			runData.hp = 0;
		}
		if (runData.hp > settings.maxHP)
		{
			runData.hp = settings.maxHP;
		}
		runData.invincibleBuffer = settings.invincibleAfterDamage;
		RefreshGameplayUI();
	}

	private void OnDestroy()
	{
		gameObjectDestroied = true;
	}
}

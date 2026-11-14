using System;
using UnityEngine;

namespace Duckov.MiniGames.MouseDodger;

[Serializable]
public struct MouseDodgerSettings
{
	public Sprite mainSprite;

	public float mainRadius;

	public float moveSpeed;

	public Bounds bounds;

	public int maxHP;

	public float goalSpawnRate;

	public float obstaleSpawnRate;

	public Vector2 obstacleScaleRange;

	public int maxEntityCount;

	public float maxEntitySpeed;

	public float invincibleAfterDamage;

	public Sprite[] obstacleSprites;

	public Sprite[] goalSprites;
}

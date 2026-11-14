using System;
using UnityEngine;

namespace Duckov.MiniGames.MouseDodger;

public class MouseDodgerEntity : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	[SerializeField]
	private CircleCollider2D collider;

	public Rigidbody2D rigidbody;

	public int score;

	public int damage;

	public Sprite sprite;

	public Action<MouseDodgerEntity, Collision2D> onCollisionEnter;

	public Action<MouseDodgerEntity, Collision2D> onCollisionStay;

	public Action<MouseDodgerEntity, Collision2D> onCollisionExit;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		onCollisionEnter?.Invoke(this, collision);
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		onCollisionExit?.Invoke(this, collision);
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		onCollisionStay?.Invoke(this, collision);
	}

	public static MouseDodgerEntity Create(Sprite sprite, float radius, Transform parent = null, Action<MouseDodgerEntity, Collision2D> onEnter = null, Action<MouseDodgerEntity, Collision2D> onStay = null, Action<MouseDodgerEntity, Collision2D> onExit = null)
	{
		GameObject gameObject = new GameObject(sprite.name);
		gameObject.layer = 30;
		gameObject.transform.SetParent(parent);
		MouseDodgerEntity mouseDodgerEntity = gameObject.AddComponent<MouseDodgerEntity>();
		mouseDodgerEntity.spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		mouseDodgerEntity.collider = gameObject.AddComponent<CircleCollider2D>();
		mouseDodgerEntity.rigidbody = gameObject.AddComponent<Rigidbody2D>();
		mouseDodgerEntity.rigidbody.gravityScale = 0.25f;
		mouseDodgerEntity.Setup(sprite, radius, onEnter, onStay, onExit);
		return mouseDodgerEntity;
	}

	private void Setup(Sprite sprite, float radius, Action<MouseDodgerEntity, Collision2D> onEnter, Action<MouseDodgerEntity, Collision2D> onStay, Action<MouseDodgerEntity, Collision2D> onExit)
	{
		this.sprite = sprite;
		spriteRenderer.sprite = sprite;
		collider.radius = radius;
		onCollisionEnter = onEnter;
		onCollisionStay = onStay;
		onCollisionExit = onExit;
	}
}

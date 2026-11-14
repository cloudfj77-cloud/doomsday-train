using UnityEngine;

public class Walker : MonoBehaviour
{
	public enum WalkerLayer
	{
		Far,
		Mid
	}

	private float speed;

	private int direction;

	private WalkerLayer layer;

	private SpriteRenderer spriteRenderer;

	private WalkerManager manager;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Init(WalkerManager owner, float baseSpeed, float speedVariance, int moveDirection, WalkerLayer walkerLayer)
	{
		manager = owner;
		direction = moveDirection;
		layer = walkerLayer;
		if (spriteRenderer == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
		speed = baseSpeed * Random.Range(1f - speedVariance, 1f + speedVariance);
		spriteRenderer.flipX = direction < 0;
		base.gameObject.SetActive(value: true);
	}

	private void Update()
	{
		base.transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
		float x = base.transform.position.x;
		switch (layer)
		{
		case WalkerLayer.Far:
			if (manager.IsOutOfFarBounds(x))
			{
				manager.DespawnFarWalker(this);
			}
			break;
		case WalkerLayer.Mid:
			if (manager.IsOutOfMidBounds(x))
			{
				manager.DespawnMidWalker(this);
			}
			break;
		}
	}

	public void Deactivate()
	{
		base.gameObject.SetActive(value: false);
	}
}

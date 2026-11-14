using UnityEngine;

public class ScratchFlake : MonoBehaviour
{
	private Vector2 velocity;

	private float lifetime;

	private float rotationSpeed;

	public void Init(Vector2 dir)
	{
		velocity = dir * Random.Range(40f, 80f);
		lifetime = Random.Range(0.3f, 0.6f);
		rotationSpeed = Random.Range(-360f, 360f);
	}

	private void Update()
	{
		base.transform.localPosition += (Vector3)(velocity * Time.deltaTime);
		base.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
		velocity *= 0.92f;
		lifetime -= Time.deltaTime;
		if (lifetime <= 0f)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}

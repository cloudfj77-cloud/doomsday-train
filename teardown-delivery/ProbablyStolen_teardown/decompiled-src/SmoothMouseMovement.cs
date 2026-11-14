using UnityEngine;

public class SmoothMouseMovement : MonoBehaviour
{
	[Header("Mouse Smooth Settings")]
	public float smoothSpeed = 0.1f;

	public float maxSpeed = 5f;

	public bool isEnabled;

	private Vector2 mouseDelta;

	private Vector2 smoothDelta;

	private Vector2 previousDelta;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			isEnabled = !isEnabled;
		}
		if (isEnabled)
		{
			SmoothMouseMovementUpdate();
		}
		else
		{
			ResetMouseMovement();
		}
	}

	private void SmoothMouseMovementUpdate()
	{
		mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		smoothDelta = Vector2.Lerp(previousDelta, mouseDelta, smoothSpeed);
		smoothDelta = Vector2.ClampMagnitude(smoothDelta, maxSpeed);
		previousDelta = smoothDelta;
		base.transform.Rotate(Vector3.up, smoothDelta.x);
		base.transform.Rotate(Vector3.left, smoothDelta.y);
	}

	private void ResetMouseMovement()
	{
		smoothDelta = Vector2.zero;
		previousDelta = Vector2.zero;
	}
}

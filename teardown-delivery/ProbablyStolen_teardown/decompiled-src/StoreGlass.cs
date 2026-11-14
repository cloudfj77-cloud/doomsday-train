using UnityEngine;

public class StoreGlass : MonoBehaviour
{
	public new GameObject gameObject;

	public Vector3 startPosition;

	public Vector3 endPosition;

	private float duration = 1.35f;

	private float elapsedTime;

	private bool isClosed;

	public bool IsClosed => isClosed;

	private void Start()
	{
		startPosition = gameObject.transform.position;
		endPosition = gameObject.transform.position;
		endPosition += new Vector3(0f, 2.42f, 0f);
	}

	private void Update()
	{
		LerpShutter(gameObject);
	}

	public void LerpShutter(GameObject targetObject)
	{
		if (!isClosed && gameObject.transform.position != endPosition)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			gameObject.transform.position = Vector3.Lerp(startPosition, endPosition, t);
		}
		else if (isClosed && gameObject.transform.position != startPosition)
		{
			elapsedTime += Time.deltaTime;
			float t2 = elapsedTime / duration;
			gameObject.transform.position = Vector3.Lerp(endPosition, startPosition, t2);
		}
	}

	public void ToggleShutterDoor()
	{
		if (gameObject != null)
		{
			Debug.Log("Glass Toggled");
			if (isClosed)
			{
				EmporiumAudioPlayer.Instance.airlock_ext_open.Play();
			}
			else
			{
				EmporiumAudioPlayer.Instance.airlock_ext_open.Play();
			}
			isClosed = !isClosed;
			elapsedTime = 0f;
		}
	}

	public void OpenGlassDoor()
	{
		if (isClosed)
		{
			ToggleShutterDoor();
		}
	}
}

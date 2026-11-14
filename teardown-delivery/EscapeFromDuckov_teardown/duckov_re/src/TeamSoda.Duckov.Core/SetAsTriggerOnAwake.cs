using UnityEngine;

public class SetAsTriggerOnAwake : MonoBehaviour
{
	private Collider collider;

	private void Awake()
	{
		collider = GetComponent<Collider>();
		if (collider != null)
		{
			collider.isTrigger = true;
		}
	}
}

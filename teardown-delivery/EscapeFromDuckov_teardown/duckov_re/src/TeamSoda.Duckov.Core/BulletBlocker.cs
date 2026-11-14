using UnityEngine;

public class BulletBlocker : MonoBehaviour
{
	public Teams team;

	public bool bulletBack;

	public bool checkDirection;

	public CharacterMainControl from;

	private void OnValidate()
	{
		base.gameObject.layer = LayerMask.NameToLayer("BlockBullet");
	}
}

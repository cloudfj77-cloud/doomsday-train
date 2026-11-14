using UnityEngine;

public class AISpecialAttachment_Hackable : AISpecialAttachmentBase
{
	public int requireLevel;

	public float range;

	public bool needVision = true;

	public InteractableVehicle interactable;

	public SphereCollider sphereCollider;

	private CharacterMainControl mainCharacter;

	private int hackSkillLevel;

	private float hackRangeFactor = 1f;

	private float hackTimeFactor = 1f;

	private bool levelAvaliable;

	protected override void OnInited()
	{
		base.OnInited();
	}

	private void Update()
	{
		if (!mainCharacter)
		{
			mainCharacter = CharacterMainControl.Main;
			if (!mainCharacter)
			{
				return;
			}
			hackSkillLevel = mainCharacter.HackSkillLevel;
			hackTimeFactor = mainCharacter.HackTimeFactor;
			hackRangeFactor = mainCharacter.HackRangeFactor;
			levelAvaliable = hackSkillLevel >= requireLevel;
			if (!levelAvaliable)
			{
				interactable.gameObject.SetActive(value: false);
			}
		}
		if (levelAvaliable)
		{
			bool flag = aiCharacterController.searchedEnemy != mainCharacter;
			if (!needVision || aiCharacterController.CharacterMainControl.Hidden)
			{
				flag = false;
			}
			if (interactable.gameObject.activeInHierarchy != flag)
			{
				interactable.gameObject.SetActive(flag);
			}
			if (sphereCollider.radius != hackRangeFactor * range)
			{
				sphereCollider.radius = hackRangeFactor * range;
			}
		}
	}
}

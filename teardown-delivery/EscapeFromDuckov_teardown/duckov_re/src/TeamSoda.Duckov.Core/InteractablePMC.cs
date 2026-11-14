using UnityEngine;

public class InteractablePMC : InteractableBase
{
	[SerializeField]
	private AISpecialAttachmentBase specialAttachment;

	private CharacterMainControl targetCharacter;

	protected override bool IsInteractable()
	{
		return true;
	}

	protected override void Awake()
	{
		finishWhenTimeOut = true;
		disableOnFinish = true;
	}

	protected override void OnInteractStart(CharacterMainControl character)
	{
		targetCharacter = character;
	}

	protected override void OnInteractFinished()
	{
		if (!(specialAttachment == null) && !(specialAttachment.aiCharacterController == null) && !(targetCharacter == null))
		{
			specialAttachment.aiCharacterController.leader = targetCharacter;
			specialAttachment.aiCharacterController.TakeOutWeapon();
		}
	}
}

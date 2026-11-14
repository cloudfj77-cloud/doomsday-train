using UnityEngine;

public class InteractableVehicle : InteractableBase
{
	[SerializeField]
	private AISpecialAttachmentBase specialAttachment;

	private CharacterMainControl targetCharacter;

	public bool timeLimit;

	public float limitTime = 10f;

	protected override bool IsInteractable()
	{
		return true;
	}

	protected override void Awake()
	{
		base.Awake();
		finishWhenTimeOut = true;
	}

	protected override void OnInteractStart(CharacterMainControl character)
	{
		targetCharacter = character;
	}

	protected override void OnInteractFinished()
	{
		if (!(specialAttachment == null) && !(specialAttachment.aiCharacterController == null) && !(targetCharacter == null))
		{
			targetCharacter.CurrentAction.StopAction();
			CharacterMainControl characterMainControl = specialAttachment.aiCharacterController.CharacterMainControl;
			if ((bool)characterMainControl && targetCharacter.IsMainCharacter && (bool)LevelManager.Instance)
			{
				targetCharacter.ControlOtherCharacter(characterMainControl, timeLimit ? limitTime : (-1f));
			}
		}
	}
}

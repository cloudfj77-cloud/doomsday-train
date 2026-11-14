using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using UnityEngine;

public class AISpecialAttachment_Horse : AISpecialAttachmentBase
{
	public InteractableBase interactable;

	public CharacterMainControl master;

	public float rideRotateSpeed = 1800f;

	public float selfRotateSpeed = 5000f;

	private Stat rotateSpeedStat;

	private bool riding;

	private float pathFindingSpaceTimer = 0.4f;

	protected override void OnInited()
	{
		interactable.OnInteractFinishedEvent.AddListener(OnRideByCharacter);
		rotateSpeedStat = character.CharacterItem.GetStat("TurnSpeed");
		character.mainDamageReceiver.gameObject.SetActive(value: false);
		TakoutWeapon().Forget();
	}

	private void OnDestroy()
	{
		interactable.OnInteractFinishedEvent.RemoveListener(OnRideByCharacter);
	}

	private void Update()
	{
		if (!aiCharacterController || !master)
		{
			return;
		}
		if (riding && master.ControlOtherCharacterAction.targetCharacter != character)
		{
			riding = false;
			character.mainDamageReceiver.gameObject.SetActive(value: false);
			if (rotateSpeedStat != null)
			{
				rotateSpeedStat.BaseValue = selfRotateSpeed;
			}
		}
		bool flag = aiCharacterController.IsMoving();
		if (master == null)
		{
			flag = false;
			aiCharacterController.StopMove();
		}
		else if (flag && Vector3.Distance(master.transform.position, aiCharacterController.transform.position) < 7f)
		{
			flag = false;
			aiCharacterController.StopMove();
		}
		if (flag)
		{
			pathFindingSpaceTimer -= Time.deltaTime;
			if (pathFindingSpaceTimer <= 0f)
			{
				pathFindingSpaceTimer = 0.4f;
				MoveToPos(master.transform.position);
			}
		}
	}

	private async UniTaskVoid TakoutWeapon()
	{
		await UniTask.WaitForSeconds(2f);
		if ((bool)aiCharacterController)
		{
			aiCharacterController.TakeOutWeapon();
			await UniTask.WaitForSeconds(1f);
			if ((bool)aiCharacterController && (bool)aiCharacterController.CharacterMainControl)
			{
				aiCharacterController.CharacterMainControl.TryToReload();
			}
		}
	}

	public void MoveToPos(Vector3 targetPos)
	{
		aiCharacterController.MoveToPos(targetPos);
	}

	public void OnRideByCharacter(CharacterMainControl interactByCharacter, InteractableBase interactable)
	{
		if ((bool)interactByCharacter)
		{
			character.mainDamageReceiver.gameObject.SetActive(value: true);
			riding = true;
			master = interactByCharacter;
			interactByCharacter.horseAI = this;
			aiCharacterController.StopMove();
			if (rotateSpeedStat != null)
			{
				rotateSpeedStat.BaseValue = rideRotateSpeed;
			}
		}
	}
}

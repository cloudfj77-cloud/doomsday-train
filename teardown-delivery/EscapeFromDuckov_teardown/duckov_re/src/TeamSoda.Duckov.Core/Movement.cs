using ECM2;
using UnityEngine;

public class Movement : MonoBehaviour
{
	public CharacterMainControl characterController;

	[SerializeField]
	private CharacterMovement characterMovement;

	public bool onlyMoveForward;

	public bool turnAccelration;

	private bool _movementEnabled = true;

	public Vector3 targetAimDirection;

	private Vector3 moveInput;

	private bool running;

	private bool moving;

	private Vector3 currentMoveDirectionXZ;

	private float turnAngleThisFrame;

	public bool forceMove;

	public Vector3 forceMoveVelocity;

	private const float movingInputThreshold = 0.02f;

	private float gravityFactor = 1f;

	private Vector3 targetVelocity;

	private float realTurnSpeed;

	public bool MovementEnabled
	{
		get
		{
			return _movementEnabled;
		}
		set
		{
			_movementEnabled = value;
			if (value)
			{
				characterMovement.enabled = true;
				characterMovement.collider.enabled = true;
			}
			else
			{
				characterMovement.enabled = false;
				characterMovement.collider.enabled = false;
			}
		}
	}

	public float walkSpeed => characterController.CharacterWalkSpeed * (characterController.IsInAdsInput ? characterController.AdsWalkSpeedMultiplier : 1f);

	public float originWalkSpeed => characterController.CharacterOriginWalkSpeed;

	public float runSpeed => characterController.CharacterRunSpeed;

	public float originRunSpeed => characterController.CharacterOriginRunSpeed;

	public float walkAcc => characterController.CharacterWalkAcc;

	public float runAcc => characterController.CharacterRunAcc;

	public float turnSpeed => characterController.CharacterTurnSpeed;

	public float aimTurnSpeed => characterController.CharacterAimTurnSpeed;

	public Vector3 MoveInput => moveInput;

	public bool Running => running;

	public bool Moving => moving;

	public bool IsOnGround => characterMovement.isOnGround;

	public bool StandStill
	{
		get
		{
			if (!moving)
			{
				return characterMovement.velocity.magnitude < 0.1f;
			}
			return false;
		}
	}

	private bool checkCanMove => characterController.CanMove();

	private bool checkCanRun => characterController.CanRun();

	public Vector3 CurrentMoveDirectionXZ => currentMoveDirectionXZ;

	public Transform rotationRoot => characterController.modelRoot;

	public float TurnAngleThisFrame => turnAngleThisFrame;

	public Vector3 Velocity => characterMovement.velocity;

	public Vector3 TargetVelocity => targetVelocity;

	private void Awake()
	{
		characterMovement.constrainToGround = true;
	}

	public void SetGravityFactor(float factor)
	{
		gravityFactor = factor;
		if (gravityFactor < 0.1f)
		{
			characterMovement.constrainToGround = false;
		}
		else
		{
			characterMovement.constrainToGround = true;
		}
	}

	public void SetMoveInput(Vector3 _moveInput)
	{
		_moveInput.y = 0f;
		moveInput = _moveInput;
		moving = false;
		if (checkCanMove && moveInput.magnitude > 0.02f)
		{
			moving = true;
		}
	}

	public void SetForceMoveVelocity(Vector3 _forceMoveVelocity)
	{
		forceMove = true;
		forceMoveVelocity = _forceMoveVelocity;
	}

	public void SetYVelocity(float _yVelocity)
	{
		characterMovement.velocity.y = _yVelocity;
	}

	public void SetAimDirection(Vector3 _aimDirection)
	{
		targetAimDirection = _aimDirection;
		targetAimDirection.y = 0f;
		targetAimDirection.Normalize();
	}

	public void SetAimDirectionToTarget(Vector3 targetPoint, Transform aimHandler)
	{
		Vector3 position = base.transform.position;
		position.y = 0f;
		Vector3 position2 = aimHandler.position;
		position2.y = 0f;
		targetPoint.y = 0f;
		float num = Vector3.Distance(position, targetPoint);
		float num2 = Vector3.Distance(position, position2);
		if (!(num < num2 + 0.25f))
		{
			float num3 = Mathf.Asin(num2 / num) * 57.29578f;
			targetAimDirection = Quaternion.Euler(0f, 0f - num3, 0f) * (targetPoint - position).normalized;
		}
	}

	public void SetPushCharacter(bool push)
	{
		characterMovement.AllowPushCharacters = push;
	}

	private void UpdateAiming()
	{
		Vector3 currentAimPoint = characterController.GetCurrentAimPoint();
		currentAimPoint.y = base.transform.position.y;
		bool flag = Vector3.Distance(currentAimPoint, base.transform.position) > 0.6f;
		if (onlyMoveForward)
		{
			SetAimDirection(moveInput.normalized);
		}
		else if (flag && characterController.IsAiming() && characterController.CanControlAim())
		{
			SetAimDirectionToTarget(currentAimPoint, characterController.CurrentUsingAimSocket);
		}
		else if (Moving)
		{
			SetAimDirection(CurrentMoveDirectionXZ);
		}
	}

	public void UpdateMovement()
	{
		if (MovementEnabled)
		{
			bool flag = checkCanRun;
			bool flag2 = checkCanMove;
			if (moveInput.magnitude <= 0.02f || !flag2)
			{
				moving = false;
				running = false;
			}
			else
			{
				moving = true;
			}
			if (!flag)
			{
				running = false;
			}
			if (moving && flag)
			{
				running = true;
			}
			if (forceMove)
			{
				UpdateForceMove();
				forceMove = false;
			}
			else
			{
				UpdateNormalMove();
			}
			UpdateAiming();
			UpdateRotation(Time.deltaTime);
			characterMovement.velocity += Physics.gravity * Time.deltaTime * gravityFactor;
			characterMovement.Move(characterMovement.velocity, Time.deltaTime);
		}
	}

	private void Update()
	{
	}

	public void ForceSetPosition(Vector3 Pos)
	{
		characterMovement.PauseGroundConstraint(1f);
		characterMovement.SetPosition(Pos);
		characterMovement.velocity = Vector3.zero;
	}

	private void UpdateNormalMove()
	{
		Vector3 velocity = characterMovement.velocity;
		targetVelocity = Vector3.zero;
		float num = walkAcc;
		if (moving)
		{
			targetVelocity = moveInput * (running ? runSpeed : walkSpeed);
			if (onlyMoveForward)
			{
				targetVelocity = rotationRoot.forward * (running ? runSpeed : walkSpeed);
			}
			num = (running ? runAcc : walkAcc);
		}
		if (characterController.Swimming)
		{
			targetVelocity *= 0.8f;
			num *= 0.5f;
		}
		else if (characterController.HeadInWater)
		{
			targetVelocity *= 0.3f;
			num *= 0.3f;
		}
		else if (characterController.BodyInWater)
		{
			targetVelocity *= 0.6f;
			num *= 0.75f;
		}
		targetVelocity.y = velocity.y;
		velocity = Vector3.MoveTowards(velocity, targetVelocity, num * Time.deltaTime);
		Vector3 vector = velocity;
		vector.y = 0f;
		if (vector.magnitude > 0.02f)
		{
			currentMoveDirectionXZ = vector.normalized;
		}
		characterMovement.velocity = velocity;
	}

	private void UpdateForceMove()
	{
		Vector3 velocity = characterMovement.velocity;
		targetVelocity = forceMoveVelocity;
		_ = walkAcc;
		targetVelocity.y = velocity.y;
		velocity = targetVelocity;
		Vector3 vector = velocity;
		vector.y = 0f;
		if (vector.magnitude > 0.02f)
		{
			currentMoveDirectionXZ = vector.normalized;
		}
		characterMovement.velocity = velocity;
	}

	public void ForceTurnTo(Vector3 direction)
	{
		targetAimDirection = direction.normalized;
		Quaternion rotation = Quaternion.Euler(0f, Quaternion.LookRotation(targetAimDirection, Vector3.up).eulerAngles.y, 0f);
		rotationRoot.rotation = rotation;
	}

	private void UpdateRotation(float deltaTime)
	{
		if (targetAimDirection.magnitude < 0.1f)
		{
			targetAimDirection = rotationRoot.forward;
		}
		float num = turnSpeed;
		if (characterController.IsAiming() && characterController.IsMainCharacter)
		{
			num = aimTurnSpeed;
		}
		if (targetAimDirection.magnitude > 0.1f)
		{
			Quaternion to = Quaternion.Euler(0f, Quaternion.LookRotation(targetAimDirection, Vector3.up).eulerAngles.y, 0f);
			Quaternion rotation = rotationRoot.rotation;
			rotationRoot.rotation = Quaternion.RotateTowards(rotation, to, num * deltaTime);
			turnAngleThisFrame = rotationRoot.rotation.eulerAngles.y - rotation.eulerAngles.y;
		}
	}

	public void ForceSetAimDirectionToAimPoint()
	{
		UpdateRotation(99999f);
	}

	public float GetMoveAnimationValue()
	{
		float num = 0f;
		float magnitude = characterMovement.velocity.magnitude;
		if (moving && running)
		{
			num = Mathf.InverseLerp(walkSpeed, runSpeed, magnitude) + 1f;
			num *= walkSpeed / originWalkSpeed;
		}
		else
		{
			num = Mathf.Clamp01(magnitude / walkSpeed);
			num *= walkSpeed / originWalkSpeed;
		}
		if (walkSpeed <= 0f)
		{
			num = 0f;
		}
		return num;
	}

	public Vector2 GetLocalMoveDirectionAnimationValue()
	{
		Vector2 up = Vector2.up;
		if (!StandStill)
		{
			Vector3 direction = currentMoveDirectionXZ;
			Vector3 vector = rotationRoot.InverseTransformDirection(direction);
			up.x = vector.x;
			up.y = vector.z;
		}
		return up;
	}

	private void FixedUpdate()
	{
	}
}

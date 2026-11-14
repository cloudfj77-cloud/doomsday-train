using System;
using System.Collections.Generic;
using Duckov;
using FMOD.Studio;
using UnityEngine;

public class CharacterAnimationControl_Moto : MonoBehaviour
{
	public CharacterMainControl characterMainControl;

	public CharacterModel characterModel;

	public Animator animator;

	public Transform motoRoateRoot;

	public float maxRollAngle = 15f;

	private float rollValue;

	private int hash_MoveSpeed = Animator.StringToHash("MoveSpeed");

	private int hash_MoveDirX = Animator.StringToHash("MoveDirX");

	private int hash_MoveDirY = Animator.StringToHash("MoveDirY");

	private HashSet<int> animatorHashes = new HashSet<int>();

	private float weight;

	private bool ride;

	private DuckovItemAgent holdAgent;

	public string rideSFX = "SFX/Special/MotorBike/motorbike_start";

	public string stopRideSFX = "SFX/Special/MotorBike/motorbike_stop";

	public string beginMovingSFX = "SFX/Special/MotorBike/motorbike_beginMoving";

	public string motorbikeMotionSoundKey = "MotorbikeMotion";

	private bool wantMove;

	public List<Transform> wheels;

	public Vector3 wheelRotAxis = Vector3.forward;

	private Vector3 wheelRot;

	public float wheelRadius = 0.5f;

	private void Update()
	{
		if (!characterMainControl)
		{
			return;
		}
		float turnAngleThisFrame = characterMainControl.movementControl.TurnAngleThisFrame;
		float num = 0f;
		float num2 = 0f;
		num = ((turnAngleThisFrame > 0.05f) ? maxRollAngle : ((!(turnAngleThisFrame < -0.05f)) ? 0f : (0f - maxRollAngle)));
		rollValue = Mathf.MoveTowards(maxDelta: ((num == 0f) ? 30f : ((!(num * rollValue < 0.01f)) ? 45f : 100f)) * Time.deltaTime, current: rollValue, target: num);
		motoRoateRoot.localEulerAngles = new Vector3(0f, 0f, 0f - rollValue);
		bool flag = characterMainControl.TargetVelocity.magnitude > 0.02f;
		if (wantMove != flag)
		{
			wantMove = flag;
			if (wantMove)
			{
				AudioObject.GetOrCreate(base.gameObject).Post(beginMovingSFX, base.gameObject);
			}
		}
		float num3 = characterMainControl.Velocity.magnitude;
		float characterRunSpeed = characterMainControl.CharacterRunSpeed;
		if (wantMove)
		{
			num3 = Mathf.Max(num3, characterRunSpeed * 0.25f);
		}
		wheelRot += wheelRotAxis * (num3 / (wheelRadius * 6.28f)) * 360f * Time.deltaTime;
		foreach (Transform wheel in wheels)
		{
			if ((bool)wheel)
			{
				wheel.localRotation = Quaternion.Euler(wheelRot);
			}
		}
		AudioObject.GetOrCreate(base.gameObject).SetParameterByName(motorbikeMotionSoundKey, Mathf.Clamp01(num3 / characterRunSpeed));
	}

	private void InitHash()
	{
		AnimatorControllerParameter[] parameters = animator.parameters;
		foreach (AnimatorControllerParameter animatorControllerParameter in parameters)
		{
			animatorHashes.Add(animatorControllerParameter.nameHash);
		}
	}

	private void SetAnimatorBool(int hash, bool value)
	{
		if (animatorHashes.Contains(hash))
		{
			animator.SetBool(hash, value);
		}
	}

	private void SetAnimatorFloat(int hash, float value)
	{
		if (animatorHashes.Contains(hash))
		{
			animator.SetFloat(hash, value);
		}
	}

	private void SetAnimatorInteger(int hash, int value)
	{
		if (animatorHashes.Contains(hash))
		{
			animator.SetInteger(hash, value);
		}
	}

	private void SetAnimatorTrigger(int hash)
	{
		if (animatorHashes.Contains(hash))
		{
			animator.SetTrigger(hash);
		}
	}

	private void Awake()
	{
		if (!characterModel)
		{
			characterModel = GetComponent<CharacterModel>();
		}
		characterModel.OnCharacterSetEvent += OnCharacterSet;
		if ((bool)characterModel.characterMainControl)
		{
			characterMainControl = characterModel.characterMainControl;
		}
		InitHash();
		if ((bool)animator)
		{
			CharacterModelAudioPoster characterModelAudioPoster = animator.gameObject.GetComponent<CharacterModelAudioPoster>();
			if (!characterModelAudioPoster)
			{
				characterModelAudioPoster = animator.gameObject.AddComponent<CharacterModelAudioPoster>();
			}
			characterModelAudioPoster.characterModel = characterModel;
		}
		LevelManager.OnControllingCharacterChanged = (Action<CharacterMainControl>)Delegate.Combine(LevelManager.OnControllingCharacterChanged, new Action<CharacterMainControl>(OnControllingCharacterChanged));
	}

	private void OnControllingCharacterChanged(CharacterMainControl character)
	{
		if (character == characterMainControl)
		{
			AudioObject.GetOrCreate(base.gameObject).Post(rideSFX, base.gameObject);
			ride = true;
		}
		else if (ride && character != characterMainControl)
		{
			StopSound();
			ride = false;
		}
	}

	private void StopSound()
	{
		AudioObject.GetOrCreate(base.gameObject).StopAll(STOP_MODE.ALLOWFADEOUT);
		AudioObject.GetOrCreate(base.gameObject).Post(stopRideSFX, base.gameObject);
	}

	private void OnDestroy()
	{
		if ((bool)characterModel)
		{
			characterModel.OnCharacterSetEvent -= OnCharacterSet;
		}
		LevelManager.OnControllingCharacterChanged = (Action<CharacterMainControl>)Delegate.Remove(LevelManager.OnControllingCharacterChanged, new Action<CharacterMainControl>(OnControllingCharacterChanged));
	}

	private void OnCharacterSet()
	{
		characterMainControl = characterModel.characterMainControl;
	}
}

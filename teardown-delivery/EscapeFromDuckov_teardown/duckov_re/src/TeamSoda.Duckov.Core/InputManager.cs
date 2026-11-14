using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.Options;
using Duckov.UI;
using Duckov.UI.DialogueBubbles;
using Duckov.Utilities;
using SodaCraft.Localizations;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	public enum InputDevices
	{
		mouseKeyboard,
		touch
	}

	private static InputDevices inputDevice = InputDevices.mouseKeyboard;

	public CharacterMainControl characterMainControl;

	private CharacterMainControl controllingCharacter;

	public AimTargetFinder aimTargetFinder;

	public float runThreshold = 0.85f;

	private Vector3 worldMoveInput;

	public static Action OnInteractButtonDown;

	private Transform aimTargetCol;

	private LayerMask obsticleLayers;

	private RaycastHit[] obsticleHits;

	private RaycastHit hittedCharacterDmgReceiverInfo;

	private RaycastHit hittedObsticleInfo;

	private RaycastHit hittedHead;

	private LayerMask aimCheckLayers;

	private CharacterMainControl foundCharacter;

	public static readonly int PrimaryWeaponSlotHash = "PrimaryWeapon".GetHashCode();

	public static readonly int SecondaryWeaponSlotHash = "SecondaryWeapon".GetHashCode();

	public static readonly int MeleeWeaponSlotHash = "MeleeWeapon".GetHashCode();

	private Camera mainCam;

	private float checkGunDurabilityCoolTimer;

	private float checkGunDurabilityCoolTime = 2f;

	private Transform aimTarget;

	private Vector2 joystickAxisInput;

	private Vector2 moveAxisInput;

	private Vector2 aimScreenPoint;

	private Vector3 inputAimPoint;

	public static bool useRunInputBuffer = false;

	private HashSet<GameObject> blockInputSources = new HashSet<GameObject>();

	private int inputActiveCoolCounter;

	private bool adsInput;

	private bool runInputBuffer;

	private bool runInput;

	private bool runInptutThisFrame;

	private bool newRecoil;

	private ItemAgent_Gun recoilGun;

	private float recoilV;

	private float recoilH;

	private float recoilRecover;

	private bool triggerInput;

	private Vector2 recoilNeedToRecover;

	private Vector2 inputMousePosition;

	private Vector2 _aimMousePosCache;

	private bool aimMousePosFirstSynced;

	private bool cursorVisable = true;

	private bool aimingEnemyHead;

	private bool currentFocus = true;

	private float fovCache = -1f;

	private float _oppositeDelta;

	private float recoilTimer;

	private float recoilTime = 0.04f;

	private float recoilRecoverTime = 0.1f;

	private Vector2 recoilThisShot;

	public static InputDevices InputDevice => inputDevice;

	public CharacterMainControl ControllingCharacter => controllingCharacter;

	public Vector3 WorldMoveInput => worldMoveInput;

	public Transform AimTarget => aimTargetCol;

	public Vector2 MoveAxisInput => moveAxisInput;

	public Vector2 AimScreenPoint => aimScreenPoint;

	public Vector3 InputAimPoint => inputAimPoint;

	private static InputManager instance
	{
		get
		{
			if (LevelManager.Instance == null)
			{
				return null;
			}
			return LevelManager.Instance.InputManager;
		}
	}

	public static bool InputActived
	{
		get
		{
			if (!instance)
			{
				return false;
			}
			if (GameManager.Paused)
			{
				return false;
			}
			if (CameraMode.Active)
			{
				return false;
			}
			if (!LevelManager.LevelInited)
			{
				return false;
			}
			if (!CharacterMainControl.Main || CharacterMainControl.Main.Health.IsDead)
			{
				return false;
			}
			return instance.inputActiveCoolCounter <= 0;
		}
	}

	public Vector2 MousePos => inputMousePosition;

	public bool TriggerInput => triggerInput;

	private Vector2 AimMousePosition
	{
		get
		{
			if (!aimMousePosFirstSynced)
			{
				aimMousePosFirstSynced = true;
				if (Mouse.current != null)
				{
					_aimMousePosCache = Mouse.current.position.ReadValue();
				}
			}
			return _aimMousePosCache;
		}
		set
		{
			if (!aimMousePosFirstSynced)
			{
				aimMousePosFirstSynced = true;
				if (Mouse.current != null)
				{
					_aimMousePosCache = Mouse.current.position.ReadValue();
				}
			}
			_aimMousePosCache = value;
		}
	}

	public bool AimingEnemyHead => aimingEnemyHead;

	public static event Action OnInputDeviceChanged;

	public static event Action<int> OnSwitchBulletTypeInput;

	public static event Action<int> OnSwitchWeaponInput;

	private void OnDestroy()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		LevelManager.OnControllingCharacterChanged = (Action<CharacterMainControl>)Delegate.Remove(LevelManager.OnControllingCharacterChanged, new Action<CharacterMainControl>(RefreshCharacter));
	}

	private void Start()
	{
		obsticleHits = new RaycastHit[3];
		obsticleLayers = (int)GameplayDataSettings.Layers.wallLayerMask | (int)GameplayDataSettings.Layers.groundLayerMask;
		LevelManager.OnControllingCharacterChanged = (Action<CharacterMainControl>)Delegate.Combine(LevelManager.OnControllingCharacterChanged, new Action<CharacterMainControl>(RefreshCharacter));
		RefreshCharacter(LevelManager.Instance.ControllingCharacter);
	}

	public void RefreshCharacter(CharacterMainControl newCharacter)
	{
		if ((bool)LevelManager.Instance)
		{
			if ((bool)controllingCharacter)
			{
				SetTrigger(trigger: false, triggerThisFrame: false, releaseThisFrame: false);
				SetMoveInput(Vector3.zero);
			}
			if ((bool)newCharacter)
			{
				controllingCharacter = newCharacter;
			}
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		currentFocus = hasFocus;
		if (!currentFocus)
		{
			Cursor.lockState = CursorLockMode.None;
		}
	}

	private void Awake()
	{
		if (blockInputSources == null)
		{
			blockInputSources = new HashSet<GameObject>();
		}
	}

	public static void DisableInput(GameObject source)
	{
		if (!(source == null) && !(instance == null))
		{
			instance.inputActiveCoolCounter = 2;
			instance.blockInputSources.Add(source);
		}
	}

	public static void ActiveInput(GameObject source)
	{
		if (!(source == null))
		{
			instance.blockInputSources.Remove(source);
		}
	}

	public static void SetInputDevice(InputDevices _inputDevice)
	{
		InputManager.OnInputDeviceChanged?.Invoke();
	}

	private void UpdateCursor()
	{
		if (LevelManager.Instance == null || controllingCharacter == null || !controllingCharacter.gameObject.activeInHierarchy)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			return;
		}
		bool flag = !controllingCharacter || controllingCharacter.Health.IsDead;
		bool flag2 = true;
		if (InputActived && !flag)
		{
			flag2 = false;
		}
		if (CameraMode.Active)
		{
			flag2 = false;
		}
		if (View.ActiveView != null)
		{
			flag2 = true;
		}
		if (!Application.isFocused)
		{
			flag2 = true;
		}
		if (cursorVisable != flag2)
		{
			cursorVisable = !cursorVisable;
		}
		if (cursorVisable)
		{
			recoilNeedToRecover = Vector2.zero;
			if (Mouse.current != null)
			{
				AimMousePosition = Mouse.current.position.ReadValue();
			}
		}
		if (Application.isFocused)
		{
			Cursor.visible = cursorVisable;
		}
		else
		{
			Cursor.visible = true;
		}
		bool flag3 = false;
		if (CameraMode.Active)
		{
			flag3 = true;
		}
		if (currentFocus)
		{
			Cursor.lockState = (flag3 ? CursorLockMode.Locked : CursorLockMode.Confined);
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
		}
	}

	private void Update()
	{
		if (!controllingCharacter)
		{
			return;
		}
		characterMainControl = controllingCharacter;
		if (!mainCam)
		{
			mainCam = LevelManager.Instance.GameCamera.renderCamera;
			return;
		}
		UpdateInputActived();
		UpdateCursor();
		if (runInput)
		{
			if (runInptutThisFrame)
			{
				runInputBuffer = !runInputBuffer;
			}
		}
		else if (moveAxisInput.magnitude < 0.1f)
		{
			runInputBuffer = false;
		}
		else if (adsInput)
		{
			runInputBuffer = false;
		}
		controllingCharacter.SetRunInput(useRunInputBuffer ? runInputBuffer : runInput);
		SetMoveInput(moveAxisInput);
		if (InputDevice == InputDevices.touch)
		{
			UpdateJoystickAim();
			UpdateAimWhileUsingTouch();
		}
		if (checkGunDurabilityCoolTimer <= checkGunDurabilityCoolTime)
		{
			checkGunDurabilityCoolTimer += Time.deltaTime;
		}
		runInptutThisFrame = false;
	}

	private void UpdateInputActived()
	{
		blockInputSources.RemoveWhere((GameObject x) => x == null || !x.activeInHierarchy);
		if (blockInputSources.Count > 0)
		{
			instance.inputActiveCoolCounter = 2;
		}
		else if (instance.inputActiveCoolCounter > 0)
		{
			instance.inputActiveCoolCounter--;
		}
	}

	private void UpdateAimWhileUsingTouch()
	{
	}

	public void SetTrigger(bool trigger, bool triggerThisFrame, bool releaseThisFrame)
	{
		triggerInput = false;
		if (!controllingCharacter)
		{
			return;
		}
		if (!InputActived)
		{
			controllingCharacter.Trigger(trigger: false, triggerThisFrame: false, releaseThisFrame: false);
			return;
		}
		triggerInput = trigger;
		controllingCharacter.Trigger(trigger, triggerThisFrame, releaseThisFrame);
		if (trigger)
		{
			CheckGunDurability();
		}
		if (triggerThisFrame)
		{
			runInputBuffer = false;
			controllingCharacter.Attack();
		}
	}

	private void CheckAttack()
	{
		if (InputDevice == InputDevices.touch && (!controllingCharacter.CurrentAction || !controllingCharacter.CurrentAction.Running))
		{
			ItemAgent_MeleeWeapon meleeWeapon = controllingCharacter.GetMeleeWeapon();
			if (!(meleeWeapon == null) && meleeWeapon.AttackableTargetInRange())
			{
				controllingCharacter.Attack();
			}
		}
	}

	private void CheckGunDurability()
	{
		if (!(checkGunDurabilityCoolTimer <= checkGunDurabilityCoolTime))
		{
			ItemAgent_Gun gun = controllingCharacter.GetGun();
			if (gun != null && gun.Item.Durability <= 0f)
			{
				DialogueBubblesManager.Show("Pop_GunBroken".ToPlainText(), controllingCharacter.transform, 2.5f).Forget();
			}
		}
	}

	private Vector3 TrnasAxisInputToWorld(Vector2 axisInput)
	{
		Vector3 zero = Vector3.zero;
		if (!mainCam)
		{
			return zero;
		}
		if (!controllingCharacter)
		{
			return zero;
		}
		if (MoveDirectionOptions.MoveViaCharacterDirection)
		{
			Vector3 vector = inputAimPoint - controllingCharacter.transform.position;
			vector.y = 0f;
			if (vector.magnitude < 1f)
			{
				return controllingCharacter.transform.forward;
			}
			vector.Normalize();
			Vector3 vector2 = Quaternion.Euler(0f, 90f, 0f) * vector;
			return axisInput.x * vector2 + axisInput.y * vector;
		}
		Vector3 right = mainCam.transform.right;
		right.y = 0f;
		right.Normalize();
		Vector3 forward = mainCam.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		return axisInput.x * right + axisInput.y * forward;
	}

	public void SetSwitchBulletTypeInput(int dir)
	{
		if ((bool)controllingCharacter && InputActived)
		{
			InputManager.OnSwitchBulletTypeInput?.Invoke(dir);
		}
	}

	public void SetSwitchWeaponInput(int dir)
	{
		if ((bool)controllingCharacter && !(controllingCharacter != CharacterMainControl.Main) && InputActived)
		{
			InputManager.OnSwitchWeaponInput?.Invoke(dir);
			controllingCharacter.SwitchWeapon(dir);
		}
	}

	public void SetSwitchInteractInput(int dir)
	{
		if ((bool)controllingCharacter && InputActived)
		{
			controllingCharacter.SwitchInteractSelection((dir <= 0) ? 1 : (-1));
		}
	}

	public void SetMoveInput(Vector2 axisInput)
	{
		moveAxisInput = axisInput;
		if (!controllingCharacter)
		{
			return;
		}
		if (!InputActived)
		{
			controllingCharacter.SetMoveInput(Vector3.zero);
			return;
		}
		worldMoveInput = TrnasAxisInputToWorld(axisInput);
		Vector3 normalized = worldMoveInput;
		if (normalized.magnitude > 0.02f)
		{
			normalized = normalized.normalized;
		}
		controllingCharacter.SetMoveInput(normalized);
	}

	public void SetRunInput(bool run)
	{
		if ((bool)controllingCharacter)
		{
			if (!InputActived)
			{
				runInput = false;
				runInptutThisFrame = false;
				controllingCharacter.SetRunInput(_runInput: false);
			}
			else
			{
				runInptutThisFrame = !runInput && run;
				runInput = run;
			}
		}
	}

	public void SetAdsInput(bool ads)
	{
		if ((bool)controllingCharacter)
		{
			if (!InputActived)
			{
				controllingCharacter.SetAdsInput(_adsInput: false);
				adsInput = false;
			}
			else
			{
				adsInput = ads;
				controllingCharacter.SetAdsInput(ads);
			}
		}
	}

	public void ToggleView()
	{
		if ((bool)controllingCharacter && InputActived)
		{
			CameraArm.ToggleView();
		}
	}

	public void ToggleNightVision()
	{
		if ((bool)controllingCharacter && InputActived)
		{
			controllingCharacter.ToggleNightVision();
		}
	}

	public void SetAimInputUsingJoystick(Vector2 _joystickAxisInput)
	{
		if (InputDevice != InputDevices.mouseKeyboard && (bool)controllingCharacter)
		{
			if (!InputActived)
			{
				joystickAxisInput = Vector3.zero;
			}
			else
			{
				joystickAxisInput = _joystickAxisInput;
			}
		}
	}

	private void UpdateJoystickAim()
	{
	}

	public void SetAimType(AimTypes aimType)
	{
		if ((bool)controllingCharacter && InputActived)
		{
			SkillBase currentRunningSkill = controllingCharacter.GetCurrentRunningSkill();
			if (aimType != controllingCharacter.AimType && currentRunningSkill != null)
			{
				Debug.Log("skill is running:" + currentRunningSkill.name);
			}
			else
			{
				controllingCharacter.SetAimType(aimType);
			}
		}
	}

	public void SetMousePosition(Vector2 mousePosition)
	{
		inputMousePosition = mousePosition;
	}

	public void SetAimInputUsingMouse(Vector2 mouseDelta)
	{
		aimingEnemyHead = false;
		AimMousePosition += mouseDelta * OptionsManager.MouseSensitivity / 10f;
		if (!controllingCharacter || !InputActived)
		{
			return;
		}
		ItemAgent_Gun gun = controllingCharacter.GetGun();
		if ((bool)gun)
		{
			AimMousePosition = ProcessMousePosViaRecoil(AimMousePosition, mouseDelta, gun);
		}
		Vector2 deltaValue = default(Vector2);
		if (Application.isFocused && InputActived && !Application.isEditor)
		{
			Vector2 mousePosition = AimMousePosition;
			ClampMousePosInWindow(ref mousePosition, ref deltaValue);
			AimMousePosition = mousePosition;
		}
		aimScreenPoint = AimMousePosition;
		controllingCharacter.GetCurrentRunningSkill();
		Ray ray = LevelManager.Instance.GameCamera.renderCamera.ScreenPointToRay(aimScreenPoint);
		Plane plane = new Plane(Vector3.up, Vector3.up * (controllingCharacter.transform.position.y + 0.5f));
		float enter = 0f;
		plane.Raycast(ray, out enter);
		Vector3 vector = ray.origin + ray.direction * enter;
		Debug.DrawLine(vector, vector + Vector3.up * 3f, Color.yellow);
		Vector3 aimPoint = vector;
		if ((bool)gun && controllingCharacter.CanControlAim())
		{
			if (Physics.Raycast(ray, out hittedHead, 100f, 1 << LayerMask.NameToLayer("HeadCollider")))
			{
				aimingEnemyHead = true;
			}
			Vector3 position = controllingCharacter.transform.position;
			if ((bool)gun)
			{
				position = gun.muzzle.transform.position;
			}
			Vector3 vector2 = vector - position;
			vector2.y = 0f;
			vector2.Normalize();
			Vector3 axis = Vector3.Cross(vector2, ray.direction);
			aimCheckLayers = GameplayDataSettings.Layers.damageReceiverLayerMask;
			for (int i = 0; (float)i < 45f; i++)
			{
				int num = i;
				if (i > 23)
				{
					num = -(i - 23);
				}
				float num2 = 1.5f;
				Vector3 vector3 = Quaternion.AngleAxis(-2f * (float)num, axis) * vector2;
				Ray ray2 = new Ray(position + num2 * vector3, vector3);
				if (Physics.SphereCast(ray2, 0.02f, out hittedCharacterDmgReceiverInfo, gun.BulletDistance, aimCheckLayers, QueryTriggerInteraction.Ignore) && hittedCharacterDmgReceiverInfo.distance > 0.1f && !Physics.SphereCast(ray2, 0.1f, out hittedObsticleInfo, hittedCharacterDmgReceiverInfo.distance, obsticleLayers, QueryTriggerInteraction.Ignore))
				{
					aimPoint = hittedCharacterDmgReceiverInfo.point;
					break;
				}
			}
		}
		if (aimingEnemyHead)
		{
			Vector3 direction = ray.direction;
			Vector3 rhs = hittedHead.collider.transform.position - hittedHead.point;
			float num3 = Vector3.Dot(direction, rhs);
			aimPoint = hittedHead.point + direction * num3 * 0.5f;
		}
		inputAimPoint = vector;
		controllingCharacter.SetAimPoint(aimPoint);
		if (Application.isFocused && currentFocus && InputActived)
		{
			Mouse.current.WarpCursorPosition(AimMousePosition);
		}
	}

	private Vector2 ProcessMousePosViaCameraChange(Vector2 inputMousePos)
	{
		Camera renderCamera = LevelManager.Instance.GameCamera.renderCamera;
		if (fovCache < 0f)
		{
			fovCache = renderCamera.fieldOfView;
			return inputMousePos;
		}
		float fieldOfView = renderCamera.fieldOfView;
		Vector2 vector = new Vector2(inputMousePos.x / (float)Screen.width * 2f - 1f, inputMousePos.y / (float)Screen.height * 2f - 1f);
		float num = Mathf.Tan(fovCache * (MathF.PI / 180f) / 2f) / Mathf.Tan(fieldOfView * (MathF.PI / 180f) / 2f);
		Vector2 vector2 = vector * num;
		Vector2 result = new Vector2((vector2.x + 1f) * 0.5f * (float)Screen.width, (vector2.y + 1f) * 0.5f * (float)Screen.height);
		fovCache = fieldOfView;
		return result;
	}

	private void ClampMousePosInWindow(ref Vector2 mousePosition, ref Vector2 deltaValue)
	{
		Vector2 zero = Vector2.zero;
		zero.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
		zero.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
		deltaValue = zero - mousePosition;
		mousePosition = zero;
	}

	public void Interact()
	{
		if ((bool)controllingCharacter && InputActived)
		{
			controllingCharacter.Interact();
			OnInteractButtonDown?.Invoke();
		}
	}

	public void PutAway()
	{
		if ((bool)controllingCharacter && InputActived && !(controllingCharacter != CharacterMainControl.Main))
		{
			controllingCharacter.ChangeHoldItem(null);
		}
	}

	public void Quack()
	{
		if ((bool)controllingCharacter && InputActived)
		{
			controllingCharacter.Quack();
		}
	}

	public void SwitchItemAgent(int index)
	{
		if ((bool)controllingCharacter && !(controllingCharacter != CharacterMainControl.Main) && InputActived)
		{
			switch (index)
			{
			case 1:
				controllingCharacter.SwitchHoldAgentInSlot(PrimaryWeaponSlotHash);
				break;
			case 2:
				controllingCharacter.SwitchHoldAgentInSlot(SecondaryWeaponSlotHash);
				break;
			case 3:
				controllingCharacter.SwitchHoldAgentInSlot(MeleeWeaponSlotHash);
				break;
			}
		}
	}

	public void StopAction()
	{
		CharacterMainControl main = CharacterMainControl.Main;
		if ((bool)main && InputActived && (bool)main.CurrentAction && main.CurrentAction.IsStopable())
		{
			main.CurrentAction.StopAction();
		}
	}

	private bool CheckInAimAngleAndNoObsticle()
	{
		if (!controllingCharacter)
		{
			return false;
		}
		if (aimTarget == null || controllingCharacter.CurrentUsingAimSocket == null)
		{
			return false;
		}
		Vector3 position = controllingCharacter.CurrentUsingAimSocket.position;
		position.y = 0f;
		Vector3 position2 = aimTarget.position;
		position2.y = 0f;
		Vector3 vector = position2 - position;
		float magnitude = vector.magnitude;
		vector.Normalize();
		float num = Mathf.Atan(0.25f / magnitude) * 57.29578f;
		if (!(Vector3.Angle(controllingCharacter.CurrentAimDirection, vector) < num))
		{
			return false;
		}
		Vector3 origin = position + Vector3.up * controllingCharacter.CurrentUsingAimSocket.position.y;
		Vector3 direction = vector;
		return Physics.SphereCastNonAlloc(origin, 0.1f, direction, obsticleHits, magnitude, obsticleLayers, QueryTriggerInteraction.Ignore) <= 0;
	}

	public void ReleaseItemSkill()
	{
		if (InputActived)
		{
			controllingCharacter.ReleaseSkill(SkillTypes.itemSkill);
		}
	}

	public void ReleaseCharacterSkill()
	{
		if (InputActived)
		{
			controllingCharacter.ReleaseSkill(SkillTypes.characterSkill);
		}
	}

	public bool CancleSkill()
	{
		if (!controllingCharacter)
		{
			return false;
		}
		return controllingCharacter.CancleSkill();
	}

	public void Dash()
	{
		if ((bool)controllingCharacter && InputActived)
		{
			controllingCharacter.TryCatchFishInput();
			controllingCharacter.Dash();
		}
	}

	public void StartCharacterSkillAim()
	{
		if ((bool)controllingCharacter && InputActived && !(controllingCharacter.skillAction.characterSkillKeeper.Skill == null) && controllingCharacter.StartSkillAim(SkillTypes.characterSkill) && (bool)controllingCharacter.skillAction.CurrentRunningSkill && controllingCharacter.skillAction.CurrentRunningSkill.SkillContext.releaseOnStartAim)
		{
			controllingCharacter.ReleaseSkill(SkillTypes.characterSkill);
		}
	}

	public void StartItemSkillAim()
	{
		if ((bool)controllingCharacter && InputActived && (bool)controllingCharacter.agentHolder.Skill && controllingCharacter.StartSkillAim(SkillTypes.itemSkill) && (bool)controllingCharacter.skillAction.CurrentRunningSkill && controllingCharacter.skillAction.CurrentRunningSkill.SkillContext.releaseOnStartAim)
		{
			controllingCharacter.ReleaseSkill(SkillTypes.itemSkill);
		}
	}

	public void AddRecoil(ItemAgent_Gun gun)
	{
		if ((bool)gun)
		{
			recoilGun = gun;
			float recoilMultiplier = LevelManager.Rule.RecoilMultiplier;
			recoilV = UnityEngine.Random.Range(gun.RecoilVMin, gun.RecoilVMax) * gun.RecoilScaleV * (1f / gun.CharacterRecoilControl) * recoilMultiplier;
			recoilH = UnityEngine.Random.Range(gun.RecoilHMin, gun.RecoilHMax) * gun.RecoilScaleH * (1f / gun.CharacterRecoilControl) * recoilMultiplier;
			recoilRecover = gun.RecoilRecover;
			recoilTime = Mathf.Min(gun.RecoilTime, 1f / gun.ShootSpeed);
			recoilRecoverTime = gun.RecoilRecoverTime;
			recoilTimer = 0f;
			newRecoil = true;
		}
	}

	private Vector2 ProcessMousePosViaRecoil(Vector2 mousePos, Vector2 mouseDelta, ItemAgent_Gun gun)
	{
		if (!gun || recoilGun != gun)
		{
			newRecoil = false;
			recoilNeedToRecover = Vector2.zero;
			return mousePos;
		}
		Vector3 position = controllingCharacter.transform.position;
		if (newRecoil)
		{
			Vector2 vector = LevelManager.Instance.GameCamera.renderCamera.WorldToScreenPoint(position);
			Vector2 normalized = (mousePos - vector).normalized;
			recoilThisShot = normalized * recoilV + recoilH * -Vector2.Perpendicular(normalized);
		}
		Vector3.Distance(InputAimPoint, position);
		float num = Time.deltaTime;
		if (recoilTimer + num >= recoilTime)
		{
			num = recoilTime - recoilTimer;
		}
		if (num > 0f)
		{
			Vector2 vector2 = recoilThisShot * num / recoilTime * Screen.height / 1440f;
			mousePos += vector2;
			recoilNeedToRecover += vector2;
			Vector2 deltaValue = Vector2.zero;
			ClampMousePosInWindow(ref mousePos, ref deltaValue);
			recoilNeedToRecover += deltaValue;
		}
		if (num <= 0f && recoilTimer > recoilRecoverTime && recoilNeedToRecover.magnitude > 0f)
		{
			float num2 = Time.deltaTime;
			if (recoilTimer - num2 < recoilRecoverTime)
			{
				num2 = recoilTimer - recoilRecoverTime;
			}
			Vector2 vector3 = Vector2.MoveTowards(recoilNeedToRecover, Vector2.zero, num2 * recoilRecover * (float)Screen.height / 1440f);
			mousePos += vector3 - recoilNeedToRecover;
			recoilNeedToRecover = vector3;
		}
		float num3 = Vector2.Dot(-recoilNeedToRecover.normalized, mouseDelta);
		if (num3 > 0f)
		{
			_oppositeDelta = 0f;
			recoilNeedToRecover = Vector2.MoveTowards(recoilNeedToRecover, Vector2.zero, num3);
		}
		else
		{
			_oppositeDelta += mouseDelta.magnitude;
			if (_oppositeDelta > 15f * (float)Screen.height / 1440f)
			{
				_oppositeDelta = 0f;
				recoilNeedToRecover = Vector3.zero;
			}
		}
		recoilTimer += Time.deltaTime;
		newRecoil = false;
		return mousePos;
	}
}

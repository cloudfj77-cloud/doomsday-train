using System.Collections.Generic;
using Duckov;
using Duckov.Utilities;
using UnityEngine;

public class CA_ControlOtherCharacter : CharacterActionBase, IProgress
{
	public CharacterMainControl targetCharacter;

	public float controlTime = 1f;

	private bool vehicleControl;

	private Transform vehicleSocket;

	private Collider[] checkColliders = new Collider[5];

	private RaycastHit[] hits = new RaycastHit[5];

	public override ActionPriorities ActionPriority()
	{
		return ActionPriorities.ControlOtherCharacter;
	}

	public override bool CanMove()
	{
		return false;
	}

	public override bool IsStopable()
	{
		return true;
	}

	private Vector3 GetExitPoint()
	{
		Vector3 result = base.transform.position;
		if (targetCharacter == null || targetCharacter.Health == null || targetCharacter.Health.IsDead)
		{
			return result;
		}
		if (!targetCharacter.isVehicle)
		{
			return result;
		}
		List<Vector3> modelExitPoints = GetModelExitPoints();
		int layerMask = (int)GameplayDataSettings.Layers.groundLayerMask | (int)GameplayDataSettings.Layers.wallLayerMask | 1;
		foreach (Vector3 item in modelExitPoints)
		{
			if (Physics.OverlapSphereNonAlloc(item + Vector3.up * 0.6f, 0.3f, checkColliders, layerMask, QueryTriggerInteraction.Ignore) <= 0)
			{
				Vector3 vector = targetCharacter.transform.position + Vector3.up * 0.4f;
				Vector3 vector2 = item + Vector3.up * 0.4f;
				if (Physics.RaycastNonAlloc(new Ray(vector, (vector2 - vector).normalized), hits, (vector2 - vector).magnitude, layerMask, QueryTriggerInteraction.Ignore) <= 0)
				{
					result = item;
					return result;
				}
			}
		}
		Debug.Log("有障碍物，不能下车");
		return result;
	}

	private List<Vector3> GetModelExitPoints()
	{
		List<Vector3> list = new List<Vector3>();
		if (targetCharacter == null)
		{
			list.Add(base.transform.position);
			return list;
		}
		Transform[] exitVehicleSockets = targetCharacter.ExitVehicleSockets;
		if (exitVehicleSockets.Length != 0)
		{
			Transform[] array = exitVehicleSockets;
			foreach (Transform transform in array)
			{
				if (transform != null)
				{
					list.Add(transform.transform.position);
				}
			}
		}
		else
		{
			list.Add(targetCharacter.transform.position + targetCharacter.modelRoot.right * 1.5f);
		}
		return list;
	}

	public override bool CanRun()
	{
		return false;
	}

	public override bool CanUseHand()
	{
		return false;
	}

	public override bool CanControlAim()
	{
		return false;
	}

	public Progress GetProgress()
	{
		Progress result = default(Progress);
		if (base.Running)
		{
			result.inProgress = true;
			if (controlTime > 0f)
			{
				result.total = controlTime;
				result.current = Mathf.Max(0f, controlTime - base.ActionTimer);
			}
			else
			{
				result.total = 1f;
				result.current = 1f;
			}
		}
		else
		{
			result.inProgress = false;
		}
		return result;
	}

	public override bool IsReady()
	{
		return !base.Running;
	}

	protected override bool OnStart()
	{
		if (!targetCharacter)
		{
			return false;
		}
		vehicleSocket = null;
		LevelManager.Instance.SetControllingCharacter(targetCharacter);
		vehicleControl = targetCharacter.isVehicle;
		if (vehicleControl)
		{
			vehicleSocket = targetCharacter.VehicleSocket;
			characterController.ridingVehicleType = targetCharacter.vehicleAnimationType;
			if (!vehicleSocket)
			{
				vehicleSocket = targetCharacter.VehicleSocket.transform;
			}
			characterController.movementControl.MovementEnabled = false;
		}
		return true;
	}

	protected override void OnStop()
	{
		if ((bool)LevelManager.Instance)
		{
			LevelManager.Instance.SetControllingCharacter(characterController);
		}
		if (vehicleControl)
		{
			characterController.SetPosition(GetExitPoint());
		}
		controlTime = -1f;
		characterController.ridingVehicleType = 0;
		characterController.movementControl.MovementEnabled = true;
		vehicleSocket = null;
	}

	protected override void OnUpdateAction(float deltaTime)
	{
		if ((controlTime > 0f && actionTimer > controlTime) || !targetCharacter || !targetCharacter.Health || targetCharacter.Health.IsDead)
		{
			StopAction();
		}
	}

	private void LateUpdate()
	{
		if (base.Running && (bool)vehicleSocket)
		{
			characterController.transform.position = vehicleSocket.position;
			characterController.modelRoot.transform.rotation = vehicleSocket.rotation;
		}
	}
}

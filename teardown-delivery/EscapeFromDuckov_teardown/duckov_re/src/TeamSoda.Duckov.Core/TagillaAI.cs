using System;
using UnityEngine;

public class TagillaAI : MonoBehaviour
{
	[Serializable]
	public struct CombatStateInfo
	{
		public float combatMoveRange;

		public Vector2 shootAttackTimeRange;

		public Vector2 shootAttackTimeSpaceRange;

		public bool shootAttackCanMove;

		[Range(0f, 1f)]
		public float traceTargetChance;

		public float meleeWaitTime;

		public float meleeAlertTime;
	}

	public AICharacterController aiCharacterController;

	public float meleeDistance = 8f;

	public float gunDistance = 12f;

	public CombatStateInfo meleeInfo;

	public CombatStateInfo gunInfo;

	public float switchCoolTime = 4f;

	private float switchTimer;

	private void Start()
	{
		aiCharacterController.melee = false;
		SetToInfo(gunInfo);
	}

	private void Update()
	{
		switchTimer -= Time.deltaTime;
		if (switchTimer < 0f && (bool)aiCharacterController.searchedEnemy)
		{
			float num = Vector3.Distance(base.transform.position, aiCharacterController.searchedEnemy.transform.position);
			if (aiCharacterController.melee && num > gunDistance)
			{
				aiCharacterController.melee = false;
				SetToInfo(gunInfo);
				switchTimer = switchCoolTime;
			}
			else if (!aiCharacterController.melee && num < meleeDistance)
			{
				aiCharacterController.melee = true;
				SetToInfo(gunInfo);
				switchTimer = switchCoolTime;
			}
		}
	}

	private void ToggleTrees()
	{
		aiCharacterController.melee = !aiCharacterController.melee;
		if (aiCharacterController.melee)
		{
			SetToInfo(meleeInfo);
		}
		else
		{
			SetToInfo(gunInfo);
		}
	}

	private void SetToInfo(CombatStateInfo info)
	{
		aiCharacterController.combatMoveRange = info.combatMoveRange;
		aiCharacterController.shootTimeRange = info.shootAttackTimeRange;
		aiCharacterController.shootTimeSpaceRange = info.shootAttackTimeSpaceRange;
		aiCharacterController.shootCanMove = info.shootAttackCanMove;
		aiCharacterController.traceTargetChance = info.traceTargetChance;
		aiCharacterController.meleeWaitTime = info.meleeWaitTime;
		aiCharacterController.meleeAlertTime = info.meleeAlertTime;
	}
}

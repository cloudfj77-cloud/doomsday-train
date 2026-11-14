using UnityEngine;

public class AISpecialAttachment_BoomCar : AISpecialAttachmentBase
{
	private bool exploed;

	public float damageRange;

	public bool isDangerForAi = true;

	private float makeSoundTimeSpace = 0.5f;

	private float makeSoundTimer;

	public ExplosionFxTypes fxType;

	public float explosionShakeStrength = 1f;

	public DamageInfo damageInfo;

	protected override void OnInited()
	{
		base.OnInited();
		if (character.Team == Teams.player)
		{
			damageInfo.fromCharacter = CharacterMainControl.Main;
		}
		if ((bool)character)
		{
			character.OnAttackInputEvent += OnAttackInput;
		}
	}

	private void OnDestroy()
	{
		if ((bool)character)
		{
			character.OnAttackInputEvent -= OnAttackInput;
		}
	}

	private void OnAttackInput()
	{
		Explode();
	}

	private void Update()
	{
		if (makeSoundTimer < 0f)
		{
			makeSoundTimer = makeSoundTimeSpace;
		}
		if (!exploed && LevelManager.Instance.ControllingCharacter != character)
		{
			Explode();
		}
	}

	private void MakeSound()
	{
		AISound sound = new AISound
		{
			fromObject = base.gameObject,
			pos = base.transform.position
		};
		if ((bool)damageInfo.fromCharacter)
		{
			sound.fromTeam = damageInfo.fromCharacter.Team;
		}
		else
		{
			sound.fromTeam = Teams.all;
		}
		sound.soundType = SoundTypes.unknowNoise;
		if (isDangerForAi)
		{
			sound.soundType = SoundTypes.grenadeDropSound;
		}
		sound.radius = 20f;
		AIMainBrain.MakeSound(sound);
	}

	private void Explode()
	{
		if (exploed)
		{
			return;
		}
		exploed = true;
		if (!(LevelManager.Instance == null))
		{
			damageInfo.isExplosion = true;
			if (aiCharacterController.CharacterMainControl.Team == Teams.player)
			{
				GameCamera.Instance.FreezeCamera(1f);
			}
			LevelManager.Instance.ExplosionManager.CreateExplosion(base.transform.position, damageRange, damageInfo, fxType, explosionShakeStrength);
			Object.Destroy(aiCharacterController.CharacterMainControl.gameObject);
		}
	}
}

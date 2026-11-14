using System;
using UnityEngine;

public class CharacterSoundMaker : MonoBehaviour
{
	public enum FootStepTypes
	{
		walkLight,
		walkHeavy,
		runLight,
		runHeavy,
		swim
	}

	public CharacterMainControl characterMainControl;

	private float moveSoundTimer;

	public float walkSoundFrequence = 4f;

	public float runSoundFrequence = 7f;

	public static Action<Vector3, FootStepTypes, CharacterMainControl> OnFootStepSound;

	public CharacterModelAudioPoster characterModelAudioPoster;

	public float walkSoundDistance
	{
		get
		{
			if (!characterMainControl)
			{
				return 0f;
			}
			return characterMainControl.WalkSoundRange;
		}
	}

	public float runSoundDistance
	{
		get
		{
			if (!characterMainControl)
			{
				return 0f;
			}
			return characterMainControl.RunSoundRange;
		}
	}

	private void Update()
	{
		if (!characterMainControl)
		{
			return;
		}
		if (characterMainControl.movementControl.Velocity.magnitude < 0.5f)
		{
			moveSoundTimer = 0f;
			return;
		}
		moveSoundTimer += Time.deltaTime;
		bool running = characterMainControl.Running;
		float num = 1f / (running ? runSoundFrequence : walkSoundFrequence);
		if (moveSoundTimer >= num)
		{
			moveSoundTimer = 0f;
			if (!characterModelAudioPoster)
			{
				PostFootStepSound();
			}
		}
	}

	public void PostFootStepSound()
	{
		if (!characterMainControl || characterMainControl.IsInAdsInput || !characterMainControl.CharacterItem)
		{
			return;
		}
		bool flag = characterMainControl.CharacterItem.TotalWeight / characterMainControl.MaxWeight >= 0.75f;
		AISound sound = new AISound
		{
			pos = base.transform.position,
			fromTeam = characterMainControl.Team,
			soundType = SoundTypes.unknowNoise,
			fromObject = characterMainControl.gameObject,
			fromCharacter = characterMainControl
		};
		FootStepTypes arg = FootStepTypes.walkLight;
		bool flag2 = false;
		if (characterMainControl.Running)
		{
			if (runSoundDistance > 0f)
			{
				sound.radius = runSoundDistance * (flag ? 1.5f : 1f);
				arg = (flag ? FootStepTypes.runHeavy : FootStepTypes.runLight);
				flag2 = true;
			}
		}
		else if (walkSoundDistance > 0f)
		{
			sound.radius = walkSoundDistance * (flag ? 1.5f : 1f);
			arg = (flag ? FootStepTypes.walkHeavy : FootStepTypes.walkLight);
			flag2 = true;
		}
		if (characterMainControl.FootInWater)
		{
			arg = FootStepTypes.swim;
		}
		if (flag2)
		{
			OnFootStepSound?.Invoke(base.transform.position, arg, characterMainControl);
		}
		AIMainBrain.MakeSound(sound);
	}
}

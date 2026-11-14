using UnityEngine;

public class CharacterModelAudioPoster : MonoBehaviour
{
	public CharacterModel characterModel;

	private CharacterSoundMaker characterSoundMaker;

	private float lastTimePost = -999f;

	private float minTimeSpace = 0.08f;

	public void PostFootStepSound()
	{
		if (!(Time.time - lastTimePost < minTimeSpace))
		{
			lastTimePost = Time.time;
			if (CheckSoundMaker())
			{
				characterSoundMaker.PostFootStepSound();
			}
		}
	}

	private bool CheckSoundMaker()
	{
		if ((bool)characterSoundMaker)
		{
			return true;
		}
		if (!characterModel)
		{
			return false;
		}
		CharacterMainControl characterMainControl = characterModel.characterMainControl;
		if (!characterMainControl)
		{
			return false;
		}
		characterSoundMaker = characterMainControl.GetComponent<CharacterSoundMaker>();
		if (!characterSoundMaker)
		{
			return false;
		}
		characterSoundMaker.characterModelAudioPoster = this;
		return true;
	}
}

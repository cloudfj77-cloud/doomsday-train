using ItemStatsSystem;
using UnityEngine;

public class ChangeModelAction : EffectAction
{
	public CharacterRandomPreset characterPreset;

	private CharacterMainControl _mainControl;

	private CharacterModel modelInstance;

	private CharacterMainControl MainControl
	{
		get
		{
			if (_mainControl == null)
			{
				_mainControl = base.Master?.Item?.GetCharacterMainControl();
			}
			return _mainControl;
		}
	}

	protected override void OnTriggered(bool positive)
	{
		modelInstance = Object.Instantiate(characterPreset.CharacterModel);
		if ((bool)modelInstance && (bool)characterPreset.FacePreset)
		{
			modelInstance.SetFaceFromPreset(characterPreset.FacePreset);
		}
		MainControl.SetCharacterModel(modelInstance);
	}

	private void OnDestroy()
	{
		if ((bool)MainControl && (bool)modelInstance)
		{
			MainControl.SetCharacterModel(null);
		}
	}
}

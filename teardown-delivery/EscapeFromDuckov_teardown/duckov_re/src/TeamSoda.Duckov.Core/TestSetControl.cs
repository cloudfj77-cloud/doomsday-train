using UnityEngine;

public class TestSetControl : MonoBehaviour
{
	public CharacterSpawnerRoot spawnerRoot;

	public void SetControl()
	{
		foreach (CharacterMainControl createdCharacter in spawnerRoot.CreatedCharacters)
		{
			if (!(createdCharacter == null) && !(LevelManager.Instance == null))
			{
				CharacterMainControl.Main.CurrentAction.StopAction();
				CharacterMainControl.Main.ControlOtherCharacter(createdCharacter);
				break;
			}
		}
	}

	public void FinishControl()
	{
		LevelManager.Instance.SetControllingCharacter(null);
	}
}

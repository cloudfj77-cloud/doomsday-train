using Duckov.Buffs;
using UnityEngine;

public class AISpecialAttachmentBase : MonoBehaviour
{
	public AICharacterController aiCharacterController;

	public CharacterMainControl character;

	public void Init(AICharacterController _ai, CharacterMainControl _character)
	{
		aiCharacterController = _ai;
		character = _character;
		OnInited();
	}

	public void AddBuff(Buff buffPfb)
	{
		if ((bool)aiCharacterController && (bool)aiCharacterController.CharacterMainControl)
		{
			aiCharacterController.CharacterMainControl.AddBuff(buffPfb);
		}
	}

	protected virtual void OnInited()
	{
	}
}

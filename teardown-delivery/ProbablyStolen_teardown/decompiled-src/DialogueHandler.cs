using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueHandler : EmptyActionHandler
{
	public static DialogueHandler current;

	[SerializeField]
	public KeyCode key;

	public override List<KeyCode> keyListeners => new KeyCode[2] { key, key2 }.ToList();

	public KeyCode key2 => KeyBindHelper.Get("advance_dialogue");

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		RaidScene raidScene = GameMaster.current.raidScene;
		Dialogue dialogue = raidScene?.currentDialogue;
		if (dialogue == null)
		{
			return;
		}
		if (raidScene.IsCompletedDialogueAnimation() && !dialogue.HasChoices())
		{
			dialogue.ExecuteEndAction();
			if (dialogue.nextDialogue != null && !dialogue.nextDialogue.isDummyChoiceDialogue)
			{
				GameMaster.current.raidScene.UpdateDialogue(dialogue.nextDialogue);
				dialogue.nextDialogue.startAction?.Invoke();
			}
			else
			{
				GameMaster.current.raidManager.EnterRoom(GameMaster.current.raidManager.currentRoom);
			}
		}
		else if (!raidScene.dialogueNew)
		{
			raidScene.CompleteDialogueAnimation();
		}
	}

	protected override void OnEventRelease(ISet<KeyCode> keysPressed)
	{
	}
}

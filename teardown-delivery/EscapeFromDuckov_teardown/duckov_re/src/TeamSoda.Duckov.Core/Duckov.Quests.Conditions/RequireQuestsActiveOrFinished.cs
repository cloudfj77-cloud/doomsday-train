using UnityEngine;

namespace Duckov.Quests.Conditions;

public class RequireQuestsActiveOrFinished : Condition
{
	[SerializeField]
	private int[] requiredQuestIDs;

	public int[] RequiredQuestIDs => requiredQuestIDs;

	public override bool Evaluate()
	{
		return QuestManager.AreQuestsActiveOrFinished(requiredQuestIDs);
	}
}

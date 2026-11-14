using Duckov.MiniGames.MouseDodger;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov.Quests;

public class QuestTask_MouseDodger_Score : Task
{
	[SerializeField]
	private int targetScore;

	private bool finished;

	[LocalizationKey("Default")]
	private string descriptionKey
	{
		get
		{
			return "Task_MouseDodger_Score";
		}
		set
		{
		}
	}

	public override string Description => descriptionKey.ToPlainText().Format(new
	{
		level = targetScore
	});

	public override object GenerateSaveData()
	{
		return finished;
	}

	public override void SetupSaveData(object data)
	{
	}

	protected override bool CheckFinished()
	{
		return MouseDodger.HighScore >= targetScore;
	}
}

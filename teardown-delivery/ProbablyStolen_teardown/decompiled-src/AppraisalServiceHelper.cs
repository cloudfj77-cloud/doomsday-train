using UnityEngine;

public class AppraisalServiceHelper
{
	public static Dialogue GetTooHighDialog()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedMechanic("appraisal_too_high_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("appraisal_too_high_2"));
		return dialogue;
	}

	public static Dialogue GetTooLowDialog()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedMechanic("appraisal_too_high_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("appraisal_too_low_2"));
		return dialogue;
	}

	public static Dialogue GetPatienceRunOutDialog()
	{
		Dialogue dialogue = new Dialogue();
		dialogue.SetText("CLIENT", LocHelper.GetLocalizedMechanic("appraisal_patience_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("appraisal_patience_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		return dialogue;
	}

	public static string SetString(int fee, int baseFee, int percentage, string quality, string slipValue, int estimate, int correctValue)
	{
		int num = 100 - Mathf.RoundToInt((float)Mathf.Abs(estimate - correctValue) * 100f / (float)correctValue);
		slipValue = LocHelper.GetLocalizedMechanic("appraisal_slip_title", quality) + " (" + num + "%)\n" + LocHelper.GetLocalizedUI("ui_appraisal_estimate", estimate) + "\n" + LocHelper.GetLocalizedUI("ui_appraisal_true_value", correctValue) + "\n\n" + LocHelper.GetLocalizedMechanic("appraisal_slip_body", fee, percentage, baseFee);
		return slipValue;
	}

	public static void GiveAppraisalFee(int value, int precision, int correctValue)
	{
		StallWatchdog.Crumb("appraisal fee for estimate " + value);
		int baseFee = AppraisalRankHelper.GetBaseFee();
		int feePercent = AppraisalRankHelper.GetFeePercent(precision);
		int num = RNG.CalculatePercentage(value, feePercent);
		string text = SetString(num, baseFee, feePercent, GetQualityName(precision), "", value, correctValue);
		PlayerStore.Instance.playerCash += num + baseFee;
		NegociationUIManager.Instance.audioSourceCashRegister.Play();
		PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePaperWithText(text, LocHelper.GetLocalizedMechanic("appraisal_slip_name")), isOwend: true);
	}

	private static string GetQualityName(int precision)
	{
		return precision switch
		{
			1 => LocHelper.GetLocalizedMechanic("appraisal_quality_perfect"), 
			2 => LocHelper.GetLocalizedMechanic("appraisal_quality_excellent"), 
			3 => LocHelper.GetLocalizedMechanic("appraisal_quality_good"), 
			4 => LocHelper.GetLocalizedMechanic("appraisal_quality_mediocre"), 
			_ => LocHelper.GetLocalizedMechanic("appraisal_quality_awful"), 
		};
	}

	public static Dialogue GetAcceptEstimateDialog(GameItem gameItem, int value)
	{
		int correctBaseValue = AppraisalHelper.GetCorrectBaseValue(gameItem);
		int estimatePrecision = GetEstimatePrecision(correctBaseValue, value);
		bool isLow = value < correctBaseValue;
		GiveAppraisalFee(value, estimatePrecision, correctBaseValue);
		return BuildAcceptDialogue(estimatePrecision, isLow);
	}

	private static int GetEstimatePrecision(int correctValue, int value)
	{
		int num = RNG.CalculatePercentage(correctValue, 95);
		int num2 = RNG.CalculatePercentage(correctValue, 105);
		if (value >= num && value <= num2)
		{
			return 1;
		}
		if (value < num)
		{
			if (value <= RNG.CalculatePercentage(correctValue, 75))
			{
				return 5;
			}
			if (value <= RNG.CalculatePercentage(correctValue, 85))
			{
				return 4;
			}
			if (value <= RNG.CalculatePercentage(correctValue, 90))
			{
				return 3;
			}
			return 2;
		}
		if (value >= RNG.CalculatePercentage(correctValue, 125))
		{
			return 5;
		}
		if (value >= RNG.CalculatePercentage(correctValue, 115))
		{
			return 4;
		}
		if (value >= RNG.CalculatePercentage(correctValue, 110))
		{
			return 3;
		}
		return 2;
	}

	private static Dialogue BuildAcceptDialogue(int precision, bool isLow)
	{
		return precision switch
		{
			1 => BuildClientDialogue("appraisal_accept_perfect_1", "appraisal_accept_thanks"), 
			2 => BuildClientDialogue("appraisal_accept_great", "appraisal_accept_thanks"), 
			3 => BuildClientDialogue("appraisal_accept_good", "appraisal_accept_thanks"), 
			4 => BuildClientDialogue("appraisal_accept_about_right", isLow ? "appraisal_accept_bit_lower" : "appraisal_accept_bit_higher", "appraisal_accept_thanks"), 
			_ => BuildClientDialogue("appraisal_accept_hmm", isLow ? "appraisal_accept_lower" : "appraisal_accept_higher", "appraisal_accept_thanks"), 
		};
	}

	private static Dialogue BuildClientDialogue(params string[] localizationKeys)
	{
		Dialogue dialogue = new Dialogue();
		Dialogue dialogue2 = dialogue;
		for (int i = 0; i < localizationKeys.Length; i++)
		{
			if (i > 0)
			{
				dialogue2 = dialogue2.NextDialogue();
			}
			dialogue2.SetText("CLIENT", LocHelper.GetLocalizedMechanic(localizationKeys[i]));
		}
		return dialogue;
	}

	public static int IsAppraisalAcceptable(GameItem gameItem, int value)
	{
		int correctBaseValue = AppraisalHelper.GetCorrectBaseValue(gameItem);
		Debug.Log("CorrectValue" + correctBaseValue);
		if (value < RNG.CalculatePercentage(correctBaseValue, 50))
		{
			return -1;
		}
		if (value > RNG.CalculatePercentage(correctBaseValue, 150))
		{
			return 1;
		}
		return 0;
	}

	public static void OnEstimate(GameItem gameItem, int value)
	{
		StallWatchdog.Crumb("appraisal estimate " + gameItem.identifier + " " + value);
		StoreClient storeClient = PlayerStore.Instance.currentClientInstance?.GetClientBlueprint();
		if (storeClient != null && storeClient.clientImpatience >= 100)
		{
			DialogUIManager.Instance.StartDialogue(GetPatienceRunOutDialog());
			return;
		}
		int correctBaseValue = AppraisalHelper.GetCorrectBaseValue(gameItem);
		EmporiumAnalytics.TrackAppraisalSent(gameItem, value, correctBaseValue);
		int num = IsAppraisalAcceptable(gameItem, value);
		int estimatePrecision = GetEstimatePrecision(correctBaseValue, value);
		if (num == 0 && !AppraisalRankHelper.IsGradeAccepted(estimatePrecision))
		{
			num = ((value >= correctBaseValue) ? 1 : (-1));
		}
		AchievementTracker.Instance.CheckMasterAppraiser(estimatePrecision == 1);
		if (num != 0)
		{
			AppraisalRankHelper.OnEstimateRefused(estimatePrecision);
			StoreClient clientBlueprint = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
			clientBlueprint.clientImpatience += 49;
			if (clientBlueprint.clientImpatience >= 100)
			{
				AppraisalRankHelper.OnCustomerWalkout();
				DialogUIManager.Instance.StartDialogue(GetPatienceRunOutDialog());
				return;
			}
		}
		switch (num)
		{
		case -1:
			DialogUIManager.Instance.StartDialogue(GetTooLowDialog());
			return;
		case 1:
			DialogUIManager.Instance.StartDialogue(GetTooHighDialog());
			return;
		}
		Dialogue acceptEstimateDialog = GetAcceptEstimateDialog(gameItem, value);
		AppraisalRankHelper.OnEstimateAccepted(estimatePrecision);
		gameItem.Destroy();
		if (PlayerStore.Instance.currentClientInstance != null && EmporiumEntry.Instance.GetAllNonOwendItemCount() == 0)
		{
			StoreClient clientBlueprint2 = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
			clientBlueprint2.CompleteBusiness();
			Dialogue dialogue = acceptEstimateDialog;
			while (dialogue.nextDialogue != null)
			{
				dialogue = dialogue.nextDialogue;
			}
			if (clientBlueprint2.allDoneDialogue != null)
			{
				dialogue.SetNextDialogue(clientBlueprint2.allDoneDialogue);
			}
			else
			{
				dialogue.SetEndAction(delegate
				{
					PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
				});
			}
		}
		DialogUIManager.Instance.StartDialogue(acceptEstimateDialog);
	}
}

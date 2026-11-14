public class IntelHelper
{
	public const string SLIP_TITLE = "SLIP_TITLE";

	public const string SLIP_BODY = "SLIP_BODY";

	public const string SLIP_DATA = "SLIP_DATA";

	public const string TUTORIAL_PAGE_TITLE = "TUTORIAL_PAGE_TITLE";

	public const string TUTORIAL_PAGE_BODY = "TUTORIAL_PAGE_BODY";

	public const string TUTORIAL_PAGE_SIGNATURE = "TUTORIAL_PAGE_SIGNATURE";

	public static void InitTutorialPage(GameItem gameItem, string title, string body, string signature)
	{
		gameItem.ModifyTag("TUTORIAL_PAGE_TITLE", delegate(TagState state)
		{
			state.Enable().SetString(title);
		});
		gameItem.ModifyTag("TUTORIAL_PAGE_BODY", delegate(TagState state)
		{
			state.Enable().SetString(body);
		});
		gameItem.ModifyTag("TUTORIAL_PAGE_SIGNATURE", delegate(TagState state)
		{
			state.Enable().SetString(signature);
		});
	}

	public static GameItem CreateMentorNoteBasicTrading()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_basic_trading_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_basic_trading_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNoteFood()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_food_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_food_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNoteAfterhours()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_afterhours_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_afterhours_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNote13()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_wholesale_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_wholesale_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNoteContraband()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_contraband_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_contraband_body", PlayerStore.Instance.contrabandMarkupLow, PlayerStore.Instance.contrabandMarkupMid, PlayerStore.Instance.contrabandMarkupHigh, PlayerStore.Instance.contrabandMarkupCritial);
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNoteStolenItem()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_stolen_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_stolen_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNoteInspection()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_inspection_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_inspection_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNoteAppraisal()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_appraisal_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_appraisal_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_note_appraisal_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNote2()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_events_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_events_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNote21()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_barter_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_barter_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNoteDisplayCase()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_display_case_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_display_case_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNoteUpgrade()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_upgrades_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_upgrades_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNoteVisualInspection()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_visual_inspection_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_visual_inspection_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNoteStamp()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_stamp_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_stamp_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateMentorNoteCommissary()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_commissary_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_commissary_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateLabelerNote()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_labels_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_labels_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateSentinel()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_scd_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_scd_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}

	public static GameItem CreateRevCodeword()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_rev_codeword_title");
		GameItem gameItem = PreBuiltItemHelper.CreatePaperWithText(LocHelper.GetLocalizedUI("intel_rev_codeword_body"), localizedUI);
		gameItem.shortDescription = LocHelper.GetLocalizedUI("intel_rev_codeword_short_desc");
		return gameItem;
	}

	public static GameItem CreateMentorNoteShotgun()
	{
		string localizedUI = LocHelper.GetLocalizedUI("intel_mentor_note_shotgun_title");
		string localizedUI2 = LocHelper.GetLocalizedUI("intel_mentor_note_shotgun_body");
		string localizedUI3 = LocHelper.GetLocalizedUI("intel_mentor_signature");
		GameItem gameItem = DirectoryMaster.Item("tutorial_page");
		InitTutorialPage(gameItem, localizedUI, localizedUI2, localizedUI3);
		gameItem.name = localizedUI;
		return gameItem;
	}
}

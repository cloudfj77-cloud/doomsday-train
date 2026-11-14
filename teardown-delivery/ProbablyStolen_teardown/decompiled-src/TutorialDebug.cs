using QFSW.QC;

public static class TutorialDebug
{
	[Command("tutorial-bubble", "Shows (or hides with 'off') a tablet tutorial bubble: nightly (end-of-day tablet), bargain or bargain2 (negotiation tablet). Open the tablet first", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Show(string bubble, string state = "on")
	{
		if (TutorialUIManager.Instance == null)
		{
			return "No TutorialUIManager in this scene.";
		}
		bool show = state != "off";
		string text = TutorialUIManager.Instance.ShowTabletHintForDebug(bubble, show);
		if (text == null)
		{
			return "Unknown bubble '" + bubble + "'. Use nightly, bargain or bargain2.";
		}
		return text;
	}
}

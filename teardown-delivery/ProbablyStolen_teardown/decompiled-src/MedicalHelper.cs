public class MedicalHelper
{
	public static void InitPillItem(GameItem item, bool enableMedical = true)
	{
		AudioHelper.InitPillAudio(item);
		if (enableMedical)
		{
			item.SetGameItemType("MEDICAL");
		}
		item.EnableTag("ITEM_MEDICAL_TAG");
	}
}

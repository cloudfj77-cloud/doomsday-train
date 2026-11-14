using System.Collections.Generic;

public class AmenitiesManager
{
	private const int HYGENE_CAP = 20;

	private const int SUBSTANCE_CAP = 50;

	private const int NUTRITION_CAP = 30;

	private const int MEDICAL_CAP = 30;

	private const int ENTERTRAINMENT_CAP = 10;

	private const int WELLBEING_CAP = 10;

	private const int AMENITIES_CAP = 100;

	public const string AMENITIES_HYGENE_TAG = "amenities_hygene";

	public const string AMENITIES_SUBSTANCE_TAG = "amenities_substance";

	public const string AMENITIES_NUTRITION_TAG = "amenities_nutrition";

	public const string AMENITIES_MEDICAL_TAG = "amenities_medical";

	public const string AMENITIES_ENTERTRAINMENT_TAG = "amenities_entertrainment";

	public const string AMENITIES_WELLBEING_TAG = "amenities_wellbeing";

	private string[] amenitiesType = new string[6] { "hygene", "substance", "nutrition", "medical", "entertrainment", "wellbeing" };

	private string[] amenitiesTypeDisplay = new string[6] { "Hygene", "Substance", "Nutrition", "Medical", "Entertrainment", "Wellbeing" };

	private string[] amenitiesDisplaySpacer = new string[6] { "         ", "      ", "      ", "        ", " ", "      " };

	private string[] amenitiesTags = new string[6] { "amenities_hygene", "amenities_substance", "amenities_nutrition", "amenities_medical", "amenities_entertrainment", "amenities_wellbeing" };

	private int[] amenitiesCap = new int[6] { 20, 50, 30, 30, 10, 10 };

	private Dictionary<string, int> amenities_values;

	private Dictionary<string, string> amenities_values_display;

	private Dictionary<string, int> past_amenities_values;

	private Dictionary<string, string> past_amenities_values_display;

	private Dictionary<string, int> duplicateTracker;

	private int totalAmenitiesValue;

	private string totalAmenitiesValueDisplay;

	private int pastTotalAmenitiesValue;

	private string pastTotalAmenitiesValueDisplay;

	private static AmenitiesManager instance = null;

	private static readonly object padlock = new object();

	public static AmenitiesManager Instance
	{
		get
		{
			lock (padlock)
			{
				if (instance == null)
				{
					instance = new AmenitiesManager();
					instance.amenities_values = new Dictionary<string, int>();
					instance.amenities_values_display = new Dictionary<string, string>();
					instance.past_amenities_values = new Dictionary<string, int>();
					instance.past_amenities_values_display = new Dictionary<string, string>();
					instance.duplicateTracker = new Dictionary<string, int>();
					for (int i = 0; i < instance.amenitiesType.Length; i++)
					{
						string key = instance.amenitiesType[i];
						instance.amenities_values[key] = 0;
						instance.past_amenities_values[key] = 0;
						instance.amenities_values_display[key] = "";
						instance.past_amenities_values_display[key] = "";
					}
				}
				return instance;
			}
		}
	}

	public void Update()
	{
		List<GameItem> allAmenitiesStorage = GetAllAmenitiesStorage();
		List<GameItem> allAmenitiesItems = GetAllAmenitiesItems(allAmenitiesStorage);
		ComputeAmenitiesValue(allAmenitiesItems);
		UpdateUse(allAmenitiesItems);
		for (int i = 0; i < instance.amenitiesType.Length; i++)
		{
			string key = amenitiesType[i];
			past_amenities_values[key] = amenities_values[key];
		}
		pastTotalAmenitiesValue = totalAmenitiesValue;
	}

	public void Reset()
	{
		for (int i = 0; i < instance.amenitiesType.Length; i++)
		{
			string key = amenitiesType[i];
			amenities_values[key] = 0;
		}
		totalAmenitiesValue = 0;
		duplicateTracker = new Dictionary<string, int>();
	}

	public void Preview()
	{
		List<GameItem> allAmenitiesStorage = GetAllAmenitiesStorage();
		List<GameItem> allAmenitiesItems = GetAllAmenitiesItems(allAmenitiesStorage);
		ComputeAmenitiesValue(allAmenitiesItems);
		UpdateDisplay();
	}

	public void UpdateDisplay()
	{
		for (int i = 0; i < instance.amenitiesType.Length; i++)
		{
			string key = amenitiesType[i];
			amenities_values_display[key] = amenities_values[key] + "/" + amenitiesCap[i];
		}
		totalAmenitiesValueDisplay = totalAmenitiesValue + "/" + 100;
		for (int j = 0; j < instance.amenitiesType.Length; j++)
		{
			string key2 = amenitiesType[j];
			past_amenities_values_display[key2] = past_amenities_values[key2] + "/" + amenitiesCap[j];
		}
		pastTotalAmenitiesValueDisplay = pastTotalAmenitiesValue + "/" + 100;
	}

	private List<GameItem> GetAllAmenitiesStorage()
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(GameMaster.current.shipManager.ship);
		List<GameItem> list2 = new List<GameItem>();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].IsTag("amenities_storage"))
			{
				list2.Add(list[num]);
			}
		}
		return list2;
	}

	private List<GameItem> GetAllAmenitiesItems(List<GameItem> amenityStorage)
	{
		List<GameItem> list = new List<GameItem>();
		for (int num = amenityStorage.Count - 1; num >= 0; num--)
		{
			list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(amenityStorage[num]));
		}
		return list;
	}

	private int ComputeAmenitiesValue(List<GameItem> list)
	{
		int num = 0;
		Reset();
		foreach (GameItem item in list)
		{
			for (int i = 0; i < instance.amenitiesType.Length; i++)
			{
				if (item.IsTag(amenitiesTags[i]) && !duplicateTracker.ContainsKey(item.identifier))
				{
					string key = amenitiesType[i];
					amenities_values[key] += item.GetTagReadonly("amenities").valueInt;
					if (amenities_values[key] > amenitiesCap[i])
					{
						amenities_values[key] = amenitiesCap[i];
					}
					duplicateTracker[item.identifier] = 1;
				}
			}
		}
		for (int j = 0; j < instance.amenitiesType.Length; j++)
		{
			string key2 = amenitiesType[j];
			num += amenities_values[key2];
		}
		totalAmenitiesValue = num;
		return num;
	}

	private void UpdateUse(List<GameItem> list)
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].IsTag("amenities"))
			{
				int num2 = list[num].GetTagReadonly("amenities_use").valueInt - 1;
				list[num].GetTagReadonly("amenities_use").valueInt = num2;
				if (num2 <= 0)
				{
					list[num].Destroy();
				}
			}
		}
	}

	private string GetScore(int amenities)
	{
		if (amenities >= 100)
		{
			return "Utopic Living Condition";
		}
		if (amenities >= 90)
		{
			return "Luxurious Living Condition";
		}
		if (amenities >= 75)
		{
			return "Excellent Living Condition";
		}
		if (amenities >= 60)
		{
			return "Good Living Condition";
		}
		if (amenities >= 40)
		{
			return "Bad Living Condition";
		}
		if (amenities >= 20)
		{
			return "Terrible Living Condition";
		}
		return "Unbearable Living Condition";
	}

	public static string ConvertTagToDisplay(string tag)
	{
		if (tag.Equals("amenities_hygene"))
		{
			return "Hygene Amenities";
		}
		if (tag.Equals("amenities_substance"))
		{
			return "Substance Amenities";
		}
		if (tag.Equals("amenities_nutrition"))
		{
			return "Nutrition Amenities";
		}
		if (tag.Equals("amenities_medical"))
		{
			return "Medical Amenities";
		}
		if (tag.Equals("amenities_entertrainment"))
		{
			return "Entertrainment Amenities";
		}
		if (tag.Equals("amenities_wellbeing"))
		{
			return "Well Being Amenities";
		}
		return "Error";
	}

	public RichTextBuilder GetAmenitiesRichText()
	{
		RichTextBuilder richTextBuilder = new RichTextBuilder();
		richTextBuilder.AddLine("Expected Amenities:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
		for (int i = 0; i < instance.amenitiesType.Length; i++)
		{
			string key = amenitiesType[i];
			richTextBuilder.AddLine(amenitiesTypeDisplay[i] + amenitiesDisplaySpacer[i] + " : " + amenities_values_display[key]);
		}
		richTextBuilder.AddLine("Total Amenities : " + totalAmenitiesValueDisplay, ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
		richTextBuilder.AddLine("Predicted Score : " + GetScore(totalAmenitiesValue));
		richTextBuilder.AddLine(" ");
		richTextBuilder.AddLine("Last Turn Amenities:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
		for (int j = 0; j < instance.amenitiesType.Length; j++)
		{
			string key2 = amenitiesType[j];
			richTextBuilder.AddLine(amenitiesTypeDisplay[j] + amenitiesDisplaySpacer[j] + " : " + past_amenities_values_display[key2]);
		}
		richTextBuilder.AddLine("Total Amenities : " + pastTotalAmenitiesValueDisplay, ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
		richTextBuilder.AddLine("Score : " + GetScore(pastTotalAmenitiesValue));
		return richTextBuilder;
	}

	public static int GetAmenitiesItemValue(GameItem item)
	{
		if (item.IsTag("amenities"))
		{
			return item.GetTagReadonly("amenities").valueInt * item.GetTagReadonly("amenities_use").valueInt / 2;
		}
		return 0;
	}
}

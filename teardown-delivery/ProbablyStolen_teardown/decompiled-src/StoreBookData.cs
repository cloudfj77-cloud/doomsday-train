using System.Collections.Generic;

public class StoreBookData
{
	public string title;

	public string[] subtitle;

	public string[] body;

	public int pageCount;

	public static Dictionary<string, StoreBookData> bookDatas;

	public const string WATER_BOOK = "WATER_BOOK";

	public const string INGOT_BOOK = "INGOT_BOOK";

	public static void InitBookDatas()
	{
		bookDatas = new Dictionary<string, StoreBookData>();
		bookDatas["WATER_BOOK"] = CreateWaterBook();
	}

	public static StoreBookData GetBookById(string id)
	{
		return bookDatas[id];
	}

	public static StoreBookData CreateWaterBook()
	{
		StoreBookData storeBookData = new StoreBookData();
		storeBookData.title = "Water Trading Guide";
		storeBookData.subtitle = new string[4] { "Introduction", "Water Types", "Testings Water Purity", "Purification Methods" };
		string text = "Water is one of the most vital resources on the station, but its quality varies dramatically depending on where you are. Due to aging infrastructure and missing systems, particularly on the lower levels, access to clean water is severely limited. The station relies heavily on water trade to supply the needs of its residents, but with so many different types of water in circulation, it's crucial to understand their uses and risks. This guide provides essential information for traders and residents alike, from identifying different water types to testing and purifying what you have.";
		string text2 = "Water on the station is categorized based on its purity, and each type serves different purposes. Here's a quick breakdown:\n \n    Pure Water: The most refined and scarce, even the elite have limited access to it. It's incredibly expensive and used primarily in sensitive environments like hospitals or laboratories.\n    High Quality Water: The standard on the upper levels. Safe for drinking, cooking, and most household uses, it's more accessible but still costly.\n    Basewater: The standard for lower levels. Mostly safe for short-term use, but prolonged exposure can lead to health issues.\n    Ghostwater: Similar in appearance to other highers quality water but not safe to drink. It's often used in farming and industrial applications.\n    Rustwater: Poor quality water, unsuitable for long-term farming and dangerous for drinking. Can only be used in specific applications like waste processing. Can be visually identified with a flashlight.\n    Gutterflow: Heavily contaminated and toxic. Drinking it requires immediate medical attention. It's often found in the lower levels, a sign of the worst degradation. Can be visually identified with a flashlight.\n ";
		string text3 = "Given the range of water qualities available, testing is vital before use, especially for traders and those on the lower levels. There are two main ways to test water purity:\n \n    Water Testing Strip: These strips allow for rapid on-site testing. They check for contaminants and determine the water purity. Ideal for traders assessing water before making a deal.\n    Flashlight Check: Gutterflow and Rustwater have a high concentration of contaminants and can be visually identified with a flashlight.";
		string text4 = "Purifying water can be expensive, but it's a necessity for those on the lower levels who can't afford access to High Quality or Pure Water. Several methods are in use on the station:\n \n    Chemical Purification Tablets: Common and easy to use, these tablets can eliminate microbial and some chemical contaminants but leave behind heavy metals and organic waste.\n ";
		storeBookData.body = new string[4] { text, text2, text3, text4 };
		storeBookData.pageCount = 4;
		return storeBookData;
	}

	public void CheckBookCreation(StoreBookData storeBookData)
	{
		_ = storeBookData.subtitle.Length;
		_ = storeBookData.body.Length;
	}
}

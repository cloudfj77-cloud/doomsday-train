public class BookHelper
{
	public const string CURRENT_BOOK_PAGE = "CURRENT_BOOK_PAGE";

	public const string BOOK_ID = "BOOK_ID";

	public const string BOOK_TAG = "BOOK_TAG";

	public static void InitBookItem(GameItem gameItem, string bookID)
	{
		gameItem.EnableTag("DOCUMENT");
		gameItem.EnableTag("BOOK_TAG");
		gameItem.ModifyTag("BOOK_ID", delegate(TagState state)
		{
			state.Enable().SetString(bookID);
		});
		gameItem.ModifyTag("CURRENT_BOOK_PAGE", delegate(TagState state)
		{
			state.Enable().SetInt(-1);
		});
	}

	public static bool CanFlipLeft(GameItem gameItem)
	{
		if (gameItem.GetTagReadonly("CURRENT_BOOK_PAGE").GetInt() == -1)
		{
			return false;
		}
		return true;
	}

	public static bool CanFlipRight(GameItem gameItem)
	{
		string @string = gameItem.GetTagReadonly("BOOK_ID").GetString();
		if (gameItem.GetTagReadonly("CURRENT_BOOK_PAGE").GetInt() + 1 < StoreBookData.GetBookById(@string).pageCount)
		{
			return true;
		}
		return false;
	}

	public static void InitGuide(GameItem gameItem)
	{
		AudioHelper.InitBook(gameItem);
	}

	public static void InitPaperGuide(GameItem gameItem)
	{
		AudioHelper.InitPaperSheet(gameItem);
	}
}

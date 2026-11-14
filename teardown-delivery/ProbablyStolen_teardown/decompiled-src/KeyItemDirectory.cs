public class KeyItemDirectory : ItemDirectory
{
	protected override void InitDirectory()
	{
	}

	private GameItem PastelKeycard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "keycard11");
		gameItem.name = "pastel Disposable Keycard";
		gameItem.shortDescription = "A one-time-use pastel access override keycard. It can open every pastel lock.";
		gameItem.unitValue = 250L;
		return gameItem;
	}

	private GameItem PastelVIPKeycard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "keycard21");
		gameItem.name = "pastel VIP Keycard";
		gameItem.shortDescription = "A pastel access override keycard. It can open every pastel lock. This one does not expire on use.";
		gameItem.unitValue = 1000L;
		return gameItem;
	}

	private GameItem RepublicKeycard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "keycard14");
		gameItem.name = "Republic Disposable Keycard";
		gameItem.shortDescription = "A one-time-use pastel access override keycard. It can open every Republic lock.";
		gameItem.unitValue = 250L;
		return gameItem;
	}

	private GameItem RepublicVIPKeycard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "keycard24");
		gameItem.name = "Republic VIP Keycard";
		gameItem.shortDescription = "A Republic access override keycard. It can open every Republic lock. This one does not expire on use.";
		gameItem.unitValue = 1000L;
		return gameItem;
	}
}

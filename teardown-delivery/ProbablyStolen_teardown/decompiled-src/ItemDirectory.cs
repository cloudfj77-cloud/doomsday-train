using System;

public abstract class ItemDirectory : Directory<GameItem>
{
	public override GameItem Empty()
	{
		return CreateEmptyItem();
	}

	public static GameItem CreateEmptyItem(string identifier = null)
	{
		return new GameItemElement(identifier, "Items/itematlas", "unknown");
	}

	public static GameItem CreateEmptyItem<K>(K identifierValue) where K : Enum
	{
		return new GameItemElement(TagSystem.EnumToString(identifierValue), "Items/itematlas", "unknown")
		{
			identifierName = TagSystem.EnumToDisplayString(identifierValue)
		};
	}
}

using System;

public class StoreEventBlueprint
{
	public Func<StoreEvent> storeEventFunc;

	public int odd;

	public string identifier;

	public StoreEventBlueprint(Func<StoreEvent> storeEventFunc, int odd, string identifier)
	{
		this.storeEventFunc = storeEventFunc;
		this.odd = odd;
		this.identifier = identifier;
	}
}

public class StoreClientItem
{
	public string identifier;

	public int probability = 100;

	public int lowCount = 1;

	public int highCount = 1;

	public StoreClientItem(string identifier, int count)
	{
		this.identifier = identifier;
		lowCount = count;
		highCount = count;
	}

	public StoreClientItem(string identifier, int lowCount, int highCount)
	{
		this.identifier = identifier;
		this.lowCount = lowCount;
		this.highCount = highCount;
	}

	public StoreClientItem(string identifier, int probability, int lowCount, int highCount)
	{
		this.identifier = identifier;
		this.probability = probability;
		this.lowCount = lowCount;
		this.highCount = highCount;
	}
}

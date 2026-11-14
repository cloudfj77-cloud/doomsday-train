public class ModScanFinding
{
	public string Kind;

	public string Location;

	public string Target;

	public bool IsHardFlag;

	public override string ToString()
	{
		return Kind + ": " + Location + " -> " + Target;
	}
}

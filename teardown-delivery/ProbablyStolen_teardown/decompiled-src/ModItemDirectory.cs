public class ModItemDirectory : ItemDirectory
{
	protected override void InitDirectory()
	{
		ModHook.FireOnModItemDirectoryInit(this);
	}
}

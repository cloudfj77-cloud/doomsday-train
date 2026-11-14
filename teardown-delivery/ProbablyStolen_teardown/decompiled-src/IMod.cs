public interface IMod
{
	void Init(ModManifest manifest);

	void OnEnable();

	void OnDisable();
}

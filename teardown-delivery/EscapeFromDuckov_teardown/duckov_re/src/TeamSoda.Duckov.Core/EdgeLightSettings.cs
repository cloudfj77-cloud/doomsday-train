using Duckov.Options;
using SodaCraft.Localizations;
using UnityEngine.Rendering.Universal;

public class EdgeLightSettings : OptionsProviderBase
{
	[LocalizationKey("Default")]
	public string onKey = "Options_On";

	[LocalizationKey("Default")]
	public string offKey = "Options_Off";

	public UniversalRendererData rendererData;

	public string edgeLightSettingName = "EdgeLight";

	public override string Key => "EdgeLightSetting";

	public override string[] GetOptions()
	{
		return new string[2]
		{
			offKey.ToPlainText(),
			onKey.ToPlainText()
		};
	}

	public override string GetCurrentOption()
	{
		return OptionsManager.Load(Key, 1) switch
		{
			0 => offKey.ToPlainText(), 
			1 => onKey.ToPlainText(), 
			_ => onKey.ToPlainText(), 
		};
	}

	public override void Set(int index)
	{
		ScriptableRendererFeature scriptableRendererFeature = rendererData.rendererFeatures.Find((ScriptableRendererFeature e) => e.name == edgeLightSettingName);
		if (scriptableRendererFeature != null)
		{
			switch (index)
			{
			case 0:
				scriptableRendererFeature.SetActive(active: false);
				break;
			case 1:
				scriptableRendererFeature.SetActive(active: true);
				break;
			}
		}
		OptionsManager.Save(Key, index);
	}

	private void Awake()
	{
		LevelManager.OnLevelInitialized += RefreshOnLevelInited;
	}

	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= RefreshOnLevelInited;
	}

	private void RefreshOnLevelInited()
	{
		int index = OptionsManager.Load(Key, 1);
		Set(index);
	}
}

using Cysharp.Threading.Tasks;
using Duckov.Scenes;
using ItemStatsSystem;
using SodaCraft.Localizations;
using UnityEngine;

public class SpawnControlCharacter : UsageBehavior
{
	public CharacterRandomPreset characterPreset;

	public float maxControlTime = 15f;

	public GameObject spawnFx;

	private CharacterMainControl characterInstance;

	[LocalizationKey("Default")]
	public string descriptionKey = "Usage_Depoly";

	public override DisplaySettingsData DisplaySettings => new DisplaySettingsData
	{
		display = true,
		description = (descriptionKey.ToPlainText() ?? "")
	};

	public override bool CanBeUsed(Item item, object user)
	{
		return true;
	}

	protected override void OnUse(Item item, object user)
	{
		CharacterMainControl characterMainControl = user as CharacterMainControl;
		if (!(characterMainControl == null))
		{
			Spawn(characterMainControl).Forget();
		}
	}

	private async UniTaskVoid Spawn(CharacterMainControl Holder)
	{
		if (!Holder || !Holder.IsMainCharacter)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if ((bool)spawnFx)
		{
			Object.Instantiate(spawnFx, base.transform.position, Quaternion.identity);
		}
		characterInstance = await characterPreset.CreateCharacterAsync(Holder.transform.position + Holder.CurrentAimDirection * 0.5f + Vector3.up * 0.5f, Holder.CurrentAimDirection, MultiSceneCore.MainScene.Value.buildIndex, null, isLeader: false);
		if ((bool)Holder)
		{
			Holder.ControlOtherCharacter(characterInstance, maxControlTime);
		}
	}
}

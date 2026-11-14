using Duckov.Buffs;

namespace ItemStatsSystem;

[MenuPath("Buff/On Buff Layer Changed")]
public class OnBuffLayerChanged : EffectTrigger
{
	public Buff buff;

	public bool needMaxLayers;

	private void OnEnable()
	{
		RegisterEvents();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		UnregisterEvents();
	}

	protected override void OnMasterSetTargetItem(Effect effect, Item item)
	{
		RegisterEvents();
		OnBufflayerChanged();
	}

	private void RegisterEvents()
	{
		UnregisterEvents();
		buff.OnLayerChangedEvent += OnBufflayerChanged;
	}

	private void UnregisterEvents()
	{
		buff.OnLayerChangedEvent -= OnBufflayerChanged;
	}

	private void OnBufflayerChanged()
	{
		if (!needMaxLayers)
		{
			Trigger();
		}
		if (buff.CurrentLayers >= buff.MaxLayers)
		{
			Trigger();
		}
	}
}

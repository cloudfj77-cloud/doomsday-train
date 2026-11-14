using Duckov.Buffs;
using ItemStatsSystem;

[MenuPath("Buff/Buff层数")]
public class BuffLayersGreaterOrEqual : EffectFilter
{
	public Buff buff;

	public int layers = 1;

	private CharacterMainControl _mainControl;

	public override string DisplayName => "buff层数大于等于";

	private CharacterMainControl MainControl
	{
		get
		{
			if (_mainControl == null)
			{
				_mainControl = base.Master?.Item?.GetCharacterMainControl();
			}
			return _mainControl;
		}
	}

	protected override bool OnEvaluate(EffectTriggerEventContext context)
	{
		if (!buff)
		{
			return false;
		}
		return buff.CurrentLayers >= layers;
	}

	private void OnDestroy()
	{
	}
}

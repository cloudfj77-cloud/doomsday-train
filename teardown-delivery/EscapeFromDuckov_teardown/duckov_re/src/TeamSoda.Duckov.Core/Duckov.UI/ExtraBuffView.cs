using Duckov.Buffs;
using Duckov.UI.Animations;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.UI;

public class ExtraBuffView : View
{
	[SerializeField]
	private FadeGroup fadeGroup;

	[SerializeField]
	private ExtraBuffViewHoverPanel hoverPanel;

	[SerializeField]
	private ExtraBuffViewEntry entryTemplate;

	private PrefabPool<ExtraBuffViewEntry> _pool;

	private CharacterBuffManager buffManager;

	private PrefabPool<ExtraBuffViewEntry> Pool
	{
		get
		{
			if (_pool == null)
			{
				_pool = new PrefabPool<ExtraBuffViewEntry>(entryTemplate);
			}
			return _pool;
		}
	}

	protected override void OnOpen()
	{
		base.OnOpen();
		Setup();
		fadeGroup.Show();
	}

	protected override void OnClose()
	{
		TearDown();
		base.OnClose();
		fadeGroup.Hide();
	}

	private void Setup()
	{
		if (!(CharacterMainControl.Main == null))
		{
			buffManager = CharacterMainControl.Main.GetBuffManager();
			if (!(buffManager == null))
			{
				buffManager.onAddBuff += OnAddBuff;
				buffManager.onRemoveBuff += OnRemoveBuff;
				Refresh();
			}
		}
	}

	private void TearDown()
	{
		if (!(buffManager == null))
		{
			buffManager.onAddBuff -= OnAddBuff;
			buffManager.onRemoveBuff -= OnRemoveBuff;
		}
	}

	private void OnAddBuff(CharacterBuffManager manager, Buff buff)
	{
		Refresh();
	}

	private void OnRemoveBuff(CharacterBuffManager manager, Buff buff)
	{
		Refresh();
	}

	private void Refresh()
	{
		Pool.ReleaseAll();
		foreach (Buff buff in buffManager.Buffs)
		{
			if (!(buff == null) && buff.DisplayInExtraView)
			{
				Pool.Get().Setup(this, buff);
			}
		}
	}

	internal void NotifyPointerEnter(ExtraBuffViewEntry extraBuffViewEntry)
	{
		hoverPanel.NotifyEnter(extraBuffViewEntry);
	}

	internal void NotifyPointerExit(ExtraBuffViewEntry extraBuffViewEntry)
	{
		hoverPanel.NotifyExit(extraBuffViewEntry);
	}
}

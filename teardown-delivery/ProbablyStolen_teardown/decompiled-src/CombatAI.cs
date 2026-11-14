using System;

public class CombatAI
{
	private CombatManager combatManager;

	private CombatEntity parentEntity;

	public Action<CombatManager, CombatEntity> CustomHandleTurn;

	private void DefaultHandleTurn(CombatManager combatManager, CombatEntity entity)
	{
		combatManager.ChangePlayingEntityAbility(entity.GetEntityWeaponAbility());
		combatManager.UseAbility(combatManager.partiesList[0].getPriorityTarget());
	}

	public void PlayNextAction(CombatManager combatManager, CombatEntity entity)
	{
		if (CustomHandleTurn == null)
		{
			DefaultHandleTurn(combatManager, entity);
			return;
		}
		try
		{
			CustomHandleTurn(combatManager, entity);
		}
		catch (Exception)
		{
			GraphUtils.TryAcceptAll((GameCharacterItem)entity, DirectoryMaster.Item("brace_item_ability"));
			CombatAIHelper.AITryFindAndUse((GameCharacterItem)entity, combatManager, "brace_item_ability", entity, isConsumeItem: false);
		}
	}
}

using System;
using System.Collections;
using UnityEngine;

public class RaidAnimation
{
	public static void AnimateCastingAbility(GameCharacterItem caster, GameItemElement abilityItem, Action callback = null)
	{
		if (caster != null && abilityItem != null && abilityItem.ability != null)
		{
			_ = abilityItem.ability;
			abilityItem.ToggleFakeBackground(enabled: true);
			abilityItem.fakeMiddle.rectTransform.pivot = new Vector2(0.2f, 0.5f);
			Vector2 rotatorCenteredPosition = caster.GetRotatorCenteredPosition();
			abilityItem.fakeMiddle.rectTransform.position = new Vector3(rotatorCenteredPosition.x, rotatorCenteredPosition.y, abilityItem.fakeMiddle.rectTransform.position.z);
			abilityItem.fakeBackground.StartCoroutine(FadeItem(abilityItem, callback));
		}
	}

	private static IEnumerator FadeItem(GameItemElement element, Action callback = null)
	{
		int count = 0;
		float opacity = 1f;
		while (count < 150)
		{
			if (count > 50)
			{
				opacity -= 0.01f;
				element.ToggleFakeBackground(enabled: true, opacity);
			}
			count++;
			yield return new WaitForSeconds(0.01f);
		}
		element.ToggleFakeBackground(enabled: false, opacity);
		callback?.Invoke();
	}

	public static void AnimateReceivingAbility(GameCharacterItem receiver, GameItemElement abilityItem)
	{
		if (receiver != null && abilityItem != null && abilityItem.ability != null)
		{
			_ = abilityItem.ability;
		}
	}

	public static void AnimateReceivingEffect(GameCharacterItem receiver, CombatAbilityEffect effect)
	{
		if (receiver == null)
		{
		}
	}

	public static void AnimateDeath(GameCharacterItem receiver)
	{
	}
}

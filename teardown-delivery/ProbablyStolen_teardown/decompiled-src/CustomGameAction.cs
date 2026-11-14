using System;

public class CustomGameAction : GameAction
{
	public GameCharacterItem caster { get; protected set; }

	public CombatAbility casterAbility { get; protected set; }

	public GameCharacterItem receiver { get; protected set; }

	public CombatAbilityEffect receiverEffect { get; protected set; }

	public Func<CustomGameAction, double, bool> animationFunc { get; protected set; }

	public Action<CustomGameAction> beginAnimationFunc { get; protected set; }

	public Action<CustomGameAction> endAnimationFunc { get; protected set; }

	public CustomGameAction(GameCharacterItem caster, CombatAbility ability, GameCharacterItem receiver = null)
	{
		this.caster = caster;
		casterAbility = ability;
		this.receiver = receiver;
	}

	public CustomGameAction(CombatAbility ability, GameCharacterItem receiver)
	{
		casterAbility = ability;
		this.receiver = receiver;
	}

	public CustomGameAction(GameCharacterItem receiver, CombatAbilityEffect receiverEffect)
	{
		this.receiver = receiver;
		this.receiverEffect = receiverEffect;
	}

	public CustomGameAction SetAnimationFunc(Func<CustomGameAction, double, bool> animationFunc)
	{
		this.animationFunc = animationFunc;
		return this;
	}

	public CustomGameAction SetBeginAnimationFunc(Action<CustomGameAction> beginAnimationFunc)
	{
		this.beginAnimationFunc = beginAnimationFunc;
		return this;
	}

	public CustomGameAction SetEndAnimationFunc(Action<CustomGameAction> endAnimationFunc)
	{
		this.endAnimationFunc = endAnimationFunc;
		return this;
	}

	public CustomGameAction SetNextTime(double nextSeconds)
	{
		base.nextTimeSeconds = nextSeconds;
		return this;
	}

	public override bool IsValid()
	{
		if (caster != null && caster.isDead)
		{
			return false;
		}
		if (caster != null && casterAbility == null)
		{
			return false;
		}
		if (receiver != null && casterAbility == null && receiverEffect == null)
		{
			return false;
		}
		return true;
	}

	public override bool AnimationStep(double currentTime)
	{
		return animationFunc?.Invoke(this, currentTime) ?? false;
	}

	public override void AnimationBegin(double currentTime)
	{
		beginAnimationFunc?.Invoke(this);
	}

	public override void AnimationEnd(double currentTime)
	{
		endAnimationFunc?.Invoke(this);
	}
}

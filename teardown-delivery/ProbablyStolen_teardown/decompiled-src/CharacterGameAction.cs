using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterGameAction : GameAction
{
	public double startAnimationTime { get; protected set; }

	public double animationTimeSeconds { get; protected set; }

	private GameCharacterItem character { get; set; }

	private Func<double, Vector2> characterTranslationWithTimeFunc { get; set; }

	private RectTransform characterRectTransform { get; set; }

	private Vector2 characterOriginPosition { get; set; }

	private Func<double, Vector4> characterColorWithTimeFunc { get; set; }

	private Image characterImage { get; set; }

	private Vector4 characterOriginColor { get; set; }

	public CharacterGameAction(GameCharacterItem character)
	{
		this.character = character;
	}

	public CharacterGameAction ModulatePosition(Func<double, Vector2> translationWithTime)
	{
		if (character != null)
		{
			characterTranslationWithTimeFunc = translationWithTime;
			characterRectTransform = character?.characterBackground.rectTransform;
		}
		return this;
	}

	public CharacterGameAction ModulateColor(Func<double, Vector4> colorWithTime)
	{
		if (character != null)
		{
			characterColorWithTimeFunc = colorWithTime;
			characterImage = character?.characterBackground.image;
		}
		return this;
	}

	public CharacterGameAction SetSyncTime(double nextSeconds, double animationSeconds = -1.0)
	{
		if (animationSeconds == -1.0)
		{
			animationSeconds = nextSeconds;
		}
		base.nextTimeSeconds = nextSeconds;
		animationTimeSeconds = animationSeconds;
		return this;
	}

	public CharacterGameAction SetAsyncTime(double animationSeconds)
	{
		base.nextTimeSeconds = 0.0;
		animationTimeSeconds = animationSeconds;
		return this;
	}

	public override bool IsValid()
	{
		if (character == null)
		{
			return false;
		}
		if (character.isDead)
		{
			return false;
		}
		if (characterColorWithTimeFunc == null && characterTranslationWithTimeFunc == null)
		{
			return false;
		}
		return true;
	}

	public override bool AnimationStep(double currentTime)
	{
		Vector2 vector = characterTranslationWithTimeFunc?.Invoke(currentTime - startAnimationTime) ?? default(Vector2);
		Vector4 vector2 = characterColorWithTimeFunc?.Invoke(currentTime - startAnimationTime) ?? characterOriginColor;
		if ((object)characterRectTransform != null)
		{
			characterRectTransform.position = characterOriginPosition;
			characterRectTransform.anchoredPosition += vector;
		}
		if ((object)characterImage != null)
		{
			characterImage.color = vector2;
		}
		if (currentTime >= startAnimationTime + animationTimeSeconds)
		{
			return false;
		}
		return true;
	}

	public override void AnimationBegin(double currentTime)
	{
		startAnimationTime = Time.timeAsDouble;
		if ((object)characterRectTransform != null)
		{
			characterOriginPosition = characterRectTransform.position;
		}
		if ((object)characterImage != null)
		{
			characterOriginColor = characterImage.color;
		}
	}

	public override void AnimationEnd(double currentTime)
	{
		if ((object)characterRectTransform != null)
		{
			characterRectTransform.position = characterOriginPosition;
		}
		if ((object)characterImage != null)
		{
			characterImage.color = Vector4.one;
		}
	}
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemGameAction : GameAction
{
	public double startAnimationTime { get; protected set; }

	public double animationTimeSeconds { get; protected set; }

	private GameItemElement item { get; set; }

	private GameCharacterItem anchor { get; set; }

	private Func<double, Vector2> itemTranslationWithTimeFunc { get; set; }

	private RectTransform itemRectTransform { get; set; }

	private Vector2 itemOriginPosition { get; set; }

	private Func<double, Vector4> itemColorWithTimeFunc { get; set; }

	private Image itemImage { get; set; }

	private Vector4 itemOriginColor { get; set; }

	public ItemGameAction(GameItem gameItem, GameCharacterItem anchor)
	{
		if (gameItem is GameItemElement gameItemElement && anchor != null)
		{
			item = gameItemElement;
			this.anchor = anchor;
		}
	}

	public ItemGameAction ModulatePosition(Func<double, Vector2> translationWithTime)
	{
		if (item != null)
		{
			itemTranslationWithTimeFunc = translationWithTime;
			itemRectTransform = item?.fakeRotator.rectTransform;
		}
		return this;
	}

	public ItemGameAction ModulateCasterColor(Func<double, Vector4> colorWithTime)
	{
		if (item != null)
		{
			itemColorWithTimeFunc = colorWithTime;
			itemImage = item?.fakeRotator.image;
		}
		return this;
	}

	public ItemGameAction SetSyncTime(double nextSeconds, double animationSeconds = -1.0)
	{
		if (animationSeconds == -1.0)
		{
			animationSeconds = nextSeconds;
		}
		base.nextTimeSeconds = nextSeconds;
		animationTimeSeconds = animationSeconds;
		return this;
	}

	public ItemGameAction SetAsyncTime(double animationSeconds)
	{
		base.nextTimeSeconds = 0.0;
		animationTimeSeconds = animationSeconds;
		return this;
	}

	public override bool IsValid()
	{
		if (item == null)
		{
			return false;
		}
		if (itemColorWithTimeFunc == null && itemTranslationWithTimeFunc == null)
		{
			return false;
		}
		return true;
	}

	public override bool AnimationStep(double currentTime)
	{
		if (item != null && item.IsDestroyed())
		{
			return false;
		}
		if (anchor != null && anchor.IsDestroyed())
		{
			return false;
		}
		Vector2 vector = itemTranslationWithTimeFunc?.Invoke(currentTime) ?? default(Vector2);
		Vector4 color = itemColorWithTimeFunc?.Invoke(currentTime) ?? itemOriginColor;
		if ((object)itemRectTransform != null)
		{
			itemRectTransform.position = anchor.GetRotatorCenteredPosition();
			itemRectTransform.anchoredPosition += vector;
		}
		if ((object)itemImage != null)
		{
			item.ToggleFakeBackground(enabled: true, color);
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
		if (item == null || !item.IsDestroyed())
		{
			if ((object)itemRectTransform != null)
			{
				itemOriginPosition = itemRectTransform.position;
			}
			if ((object)itemImage != null)
			{
				itemOriginColor = itemImage.color;
				item.ToggleFakeBackground(enabled: true, itemOriginColor);
			}
		}
	}

	public override void AnimationEnd(double currentTime)
	{
		if (item == null || !item.IsDestroyed())
		{
			if ((object)itemRectTransform != null)
			{
				itemRectTransform.position = itemOriginPosition;
			}
			if ((object)itemImage != null)
			{
				itemImage.color = itemOriginColor;
				item.ToggleFakeBackground(enabled: false, itemOriginColor);
			}
		}
	}
}

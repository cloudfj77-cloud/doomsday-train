using UnityEngine;

public struct ItemSpriteModifier
{
	public readonly Sprite sprite;

	public readonly Vector2 position;

	public readonly int orientation;

	public readonly bool isFlipped;

	public readonly int priority;

	public readonly bool haveMask;

	public readonly Color color;

	public ItemSpriteModifier(Sprite sprite, Vector2 position, int orientation = 0, bool isFlipped = false, int priority = 0, bool haveMask = false, Color color = default(Color))
	{
		this.sprite = sprite;
		this.position = position;
		this.orientation = orientation;
		this.isFlipped = isFlipped;
		this.priority = priority;
		this.haveMask = haveMask;
		this.color = ((color == default(Color)) ? Color.white : color);
	}
}

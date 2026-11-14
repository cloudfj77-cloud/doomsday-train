using System.Collections.Generic;
using UnityEngine;

public class DialogBubbleScaler
{
	private class Entry
	{
		public RectTransform rect;

		public Vector3 baseScale;

		public Vector2 basePosition;

		public Entry leader;
	}

	private readonly List<Entry> entries = new List<Entry>();

	public void Register(Component bubble, Component leader = null)
	{
		if (!(bubble == null))
		{
			RectTransform rectTransform = bubble.transform as RectTransform;
			if (!(rectTransform == null))
			{
				Entry item = new Entry
				{
					rect = rectTransform,
					baseScale = rectTransform.localScale,
					basePosition = rectTransform.anchoredPosition,
					leader = ((leader != null) ? Find(leader) : null)
				};
				entries.Add(item);
			}
		}
	}

	public void Register(GameObject bubble, Component leader = null)
	{
		if (bubble != null)
		{
			Register(bubble.transform, leader);
		}
	}

	private Entry Find(Component bubble)
	{
		foreach (Entry entry in entries)
		{
			if (entry.rect == bubble.transform)
			{
				return entry;
			}
		}
		return null;
	}

	public void Apply()
	{
		Apply(EmporiumOption.DialogBubbleScale);
	}

	public void Apply(float factor)
	{
		foreach (Entry entry in entries)
		{
			if (!(entry.rect == null))
			{
				entry.rect.localScale = entry.baseScale * factor;
				if (entry.leader != null && entry.leader.rect != null)
				{
					Vector2 vector = entry.basePosition - entry.leader.basePosition;
					entry.rect.anchoredPosition = entry.leader.rect.anchoredPosition + vector * factor;
				}
			}
		}
	}
}

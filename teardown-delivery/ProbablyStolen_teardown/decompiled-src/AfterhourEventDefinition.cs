using System;
using System.Collections.Generic;

public class AfterhourEventDefinition
{
	public string id;

	public string locationId;

	public string source = "vanilla";

	public Func<string> title;

	public Func<string> description;

	public List<AfterhourEffect> effects = new List<AfterhourEffect>();

	public AfterhourEventTone? toneOverride;

	public int durationDays = 1;

	public float dailyChancePercent;

	public List<AfterhourWeekday> weekdays = new List<AfterhourWeekday>();

	public bool closesLocation;

	public Func<bool> canStart;

	public Action onStart;

	public Action onEnd;

	public string GetTitle()
	{
		return Resolve(title);
	}

	public string GetDescription()
	{
		return Resolve(description);
	}

	public AfterhourEventDefinition AddEffect(Func<string> text, string modifierKey = null, float modifierValue = 0f, AfterhourEffectPolarity polarity = AfterhourEffectPolarity.Neutral)
	{
		effects.Add(new AfterhourEffect(text, modifierKey, modifierValue, polarity));
		return this;
	}

	public AfterhourEventDefinition Every(params AfterhourWeekday[] days)
	{
		foreach (AfterhourWeekday item in days)
		{
			if (!weekdays.Contains(item))
			{
				weekdays.Add(item);
			}
		}
		return this;
	}

	public bool IsScheduledOn(int dayCounter)
	{
		if (weekdays != null)
		{
			return weekdays.Contains((AfterhourWeekday)(dayCounter % 7));
		}
		return false;
	}

	public AfterhourEventTone GetTone()
	{
		if (toneOverride.HasValue)
		{
			return toneOverride.Value;
		}
		bool flag = false;
		bool flag2 = false;
		if (effects != null)
		{
			foreach (AfterhourEffect effect in effects)
			{
				if (effect.polarity == AfterhourEffectPolarity.Good)
				{
					flag = true;
				}
				else if (effect.polarity == AfterhourEffectPolarity.Bad)
				{
					flag2 = true;
				}
			}
		}
		if (flag && !flag2)
		{
			return AfterhourEventTone.Good;
		}
		if (flag2 && !flag)
		{
			return AfterhourEventTone.Bad;
		}
		return AfterhourEventTone.Mixed;
	}

	public List<string> GetEffectTexts()
	{
		List<string> list = new List<string>();
		if (effects == null)
		{
			return list;
		}
		foreach (AfterhourEffect effect in effects)
		{
			string text = Resolve(effect.text);
			if (!string.IsNullOrEmpty(text))
			{
				list.Add(text);
			}
		}
		return list;
	}

	public float GetModifier(string key)
	{
		float num = 0f;
		if (effects == null || string.IsNullOrEmpty(key))
		{
			return num;
		}
		foreach (AfterhourEffect effect in effects)
		{
			if (effect.modifierKey == key)
			{
				num += effect.modifierValue;
			}
		}
		return num;
	}

	private string Resolve(Func<string> provider)
	{
		if (provider == null)
		{
			return "";
		}
		try
		{
			return provider() ?? "";
		}
		catch (Exception arg)
		{
			Debug.LogError($"[AfterhourEvents] event '{id}' text provider threw: {arg}");
			return "";
		}
	}
}

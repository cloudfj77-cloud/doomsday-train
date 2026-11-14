using System;

public class AfterhourEffect
{
	public Func<string> text;

	public string modifierKey;

	public float modifierValue;

	public AfterhourEffectPolarity polarity;

	public AfterhourEffect()
	{
	}

	public AfterhourEffect(Func<string> text, string modifierKey = null, float modifierValue = 0f, AfterhourEffectPolarity polarity = AfterhourEffectPolarity.Neutral)
	{
		this.text = text;
		this.modifierKey = modifierKey;
		this.modifierValue = modifierValue;
		this.polarity = polarity;
	}
}

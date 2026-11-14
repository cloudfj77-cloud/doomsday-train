using UnityEngine;

public class EmporiumScreen : MonoBehaviour
{
	private SpriteRenderer screen;

	[ColorUsage(false, true)]
	[SerializeField]
	private Color glowColor = new Color(0f, 2f, 2f);

	private MaterialPropertyBlock mpb;

	private void Start()
	{
		if (screen == null)
		{
			screen = GetComponent<SpriteRenderer>();
		}
		SetGlow(glowColor);
	}

	public void SetGlow(Color hdrColor)
	{
		if (mpb == null)
		{
			mpb = new MaterialPropertyBlock();
		}
		screen.GetPropertyBlock(mpb);
		mpb.SetColor("_Emission", hdrColor);
		screen.SetPropertyBlock(mpb);
	}
}

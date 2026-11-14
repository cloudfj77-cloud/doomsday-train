using TMPro;
using UnityEngine;

public class TabletSkin : MonoBehaviour
{
	[Header("Skin: drawn pieces")]
	[Tooltip("6x6 white frame, 2px border, sliced 2 (button borders, entry hover frame, group frames, checkbox).")]
	public Sprite frame2;

	[Tooltip("10x2 white dot every 10 px, tiled: the footer leaders.")]
	public Sprite dottedLine;

	[Tooltip("8x8 white rounded rectangle, radius 2, sliced 3: the info tags.")]
	public Sprite tagRounded;

	[Tooltip("12x12 corner sample of the notched 2px frame, sliced 6.")]
	public Sprite frameNotched;

	public Sprite arrows;

	[Header("Skin: info icons (14 px)")]
	public Sprite infoDark;

	public Sprite infoLight;

	public Sprite infoTan;

	public Sprite infoCream;

	public Sprite infoRed;

	[Header("Skin: selector arrows (32 px, drawn at 28)")]
	public Sprite prevNormal;

	public Sprite prevHover;

	public Sprite nextNormal;

	public Sprite nextHover;

	[Header("Skin: fonts (editor-time defaults, TMPLocalizer swaps per locale)")]
	public TMP_FontAsset fontLubrifont;

	public TMP_FontAsset fontNotoMedium;

	public TMP_FontAsset fontNotoRegular;
}

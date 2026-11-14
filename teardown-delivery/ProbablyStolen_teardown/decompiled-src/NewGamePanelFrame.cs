using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePanelFrame : MonoBehaviour
{
	[Header("Panel")]
	public RectTransform panel;

	public TextMeshProUGUI headerTitle;

	public Button backButton;

	[Header("Continue button")]
	public Button continueButton;

	public Image continueOuter;

	public Image continueInner;

	public TextMeshProUGUI continueLabel;

	[Header("Skin: arrows (32x32; the back button reuses the prev pair)")]
	public Sprite arrowPrevNormal;

	public Sprite arrowPrevHover;

	public Sprite arrowNextNormal;

	public Sprite arrowNextHover;

	[Tooltip("6x6 white frame, 2px border, square corners, sliced 2. Drawn by the generator; the Continue button borders.")]
	public Sprite frame2Square;

	[Header("Skin: fonts (editor-time defaults, TMPLocalizer swaps per locale)")]
	public TMP_FontAsset fontLubrifont;

	public TMP_FontAsset fontNotoMedium;

	public TMP_FontAsset fontNotoRegular;
}

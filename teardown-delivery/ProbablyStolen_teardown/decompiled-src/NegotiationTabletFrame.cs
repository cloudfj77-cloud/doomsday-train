using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NegotiationTabletFrame : TabletFrame
{
	[Header("Client header")]
	public TextMeshProUGUI clientName;

	public TextMeshProUGUI clientSubtitle;

	[Header("Controls")]
	public Button resizeButton;

	public Button bargainButton;

	public Image bargainImage;

	public Sprite bargainNormal;

	public Sprite bargainHover;

	public Sprite bargainPressed;

	public Sprite bargainActive;

	[Header("Art while the bargain panel is open (434:2309: no right spine)")]
	public Sprite artNormal;

	public Sprite artBargainOpen;

	[Header("Budget confirmation overlay")]
	public GameObject confirmRoot;

	public TextMeshProUGUI confirmText;

	public RectTransform confirmButtonHost;
}

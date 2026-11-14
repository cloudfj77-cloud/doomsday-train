using UnityEngine;
using UnityEngine.UI;

public class CreditsFrame : NewGamePanelFrame
{
	[Header("Scrolling column (sections built here at runtime)")]
	public ScrollRect scroll;

	public RectTransform content;

	public Scrollbar scrollbar;
}

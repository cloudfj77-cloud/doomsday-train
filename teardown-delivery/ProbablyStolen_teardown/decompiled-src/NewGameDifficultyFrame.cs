using UnityEngine;
using UnityEngine.UI;

public class NewGameDifficultyFrame : NewGamePanelFrame
{
	[Header("Plates (built here at runtime)")]
	public RectTransform platesHost;

	public Button prevButton;

	public Image prevImage;

	public Button nextButton;

	public Image nextImage;

	[Header("Options row")]
	public RectTransform optionsHost;

	public NewGameCheckbox skipIntro;

	public NewGameCheckbox endless;
}

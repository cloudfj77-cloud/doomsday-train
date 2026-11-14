using UnityEngine;

namespace Duckov.MiniGames.Island;

public class PressStartText : MonoBehaviour
{
	public CanvasGroup canvasGroup;

	public AnimationCurve breathingCurve;

	public AnimationCurve pressedCurve;

	public bool pressed;

	public void NotifyPressed()
	{
		pressed = true;
	}

	public void Reset()
	{
		pressed = false;
	}

	private void Update()
	{
		AnimationCurve animationCurve = (pressed ? pressedCurve : breathingCurve);
		canvasGroup.alpha = animationCurve.Evaluate(Time.time);
	}
}

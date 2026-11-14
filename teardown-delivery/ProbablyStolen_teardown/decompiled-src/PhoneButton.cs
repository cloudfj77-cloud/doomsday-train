using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhoneButton : MonoBehaviour
{
	public int number;

	private const float SIMULATED_PRESS_SECONDS = 0.12f;

	private const string PRESS_SOUND = "effect_button3";

	private Button button;

	private Coroutine releaseRoutine;

	private void Start()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (component != null && AudioManager.Instance.CanPlay("effect_button3"))
		{
			component.clip = AudioManager.Instance.audioDirectory.GetAudioClip("effect_button3");
		}
	}

	public void OnPressed()
	{
		PhoneUIManager.Instance.PressNumber(number);
	}

	public void SimulateClick()
	{
		if (button == null)
		{
			button = GetComponent<Button>();
		}
		if (button == null || !button.interactable || !base.isActiveAndEnabled)
		{
			OnPressed();
			return;
		}
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			button = PointerEventData.InputButton.Left
		};
		ExecuteEvents.Execute(base.gameObject, eventData, ExecuteEvents.pointerDownHandler);
		ExecuteEvents.Execute(base.gameObject, eventData, ExecuteEvents.pointerClickHandler);
		if (releaseRoutine != null)
		{
			StopCoroutine(releaseRoutine);
		}
		releaseRoutine = StartCoroutine(ReleaseAfter(eventData));
	}

	private IEnumerator ReleaseAfter(PointerEventData eventData)
	{
		yield return new WaitForSecondsRealtime(0.12f);
		ExecuteEvents.Execute(base.gameObject, eventData, ExecuteEvents.pointerUpHandler);
		releaseRoutine = null;
	}

	private void OnDisable()
	{
		if (releaseRoutine != null)
		{
			StopCoroutine(releaseRoutine);
			releaseRoutine = null;
		}
	}
}

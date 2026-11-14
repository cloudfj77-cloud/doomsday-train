using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogChoiceBubble : MonoBehaviour
{
	public TextMeshProUGUI TMP;

	public int index;

	public int type;

	public const float DISABLED_ALPHA = 0.9f;

	private bool isInteractable = true;

	private CanvasGroup canvasGroup;

	private Button button;

	private ColorBlock originalColors;

	private bool hasOriginalColors;

	private const float RECHECK_INTERVAL = 0.2f;

	private float nextRecheck;

	public void OnDialogChoiceClick()
	{
		if (isInteractable)
		{
			Debug.Log("Clicked " + index);
			if (type == 0)
			{
				DialogUIManager.Instance.SelectChoice(index);
			}
			else if (type == 1)
			{
				DialoguePhoneUIManager.Instance.SelectChoice(index);
			}
		}
	}

	public void SetInteractable(bool interactable)
	{
		isInteractable = interactable;
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
			}
		}
		if (button == null)
		{
			button = GetComponent<Button>();
		}
		canvasGroup.alpha = (interactable ? 1f : 0.9f);
		canvasGroup.interactable = interactable;
		canvasGroup.blocksRaycasts = true;
		if (button != null)
		{
			if (!hasOriginalColors)
			{
				originalColors = button.colors;
				hasOriginalColors = true;
			}
			if (interactable)
			{
				button.colors = originalColors;
			}
			else
			{
				ColorBlock colors = originalColors;
				colors.disabledColor = originalColors.normalColor;
				button.colors = colors;
			}
			button.interactable = interactable;
		}
	}

	public bool IsInteractable()
	{
		return isInteractable;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		nextRecheck = 0f;
	}

	private void Update()
	{
		if (Time.unscaledTime < nextRecheck)
		{
			return;
		}
		nextRecheck = Time.unscaledTime + 0.2f;
		Dialogue dialogue = ((type != 0) ? ((DialoguePhoneUIManager.Instance != null) ? DialoguePhoneUIManager.Instance.GetCurrentDialog() : null) : ((DialogUIManager.Instance != null) ? DialogUIManager.Instance.GetCurrentDialog() : null));
		if (dialogue != null && index < dialogue.choices.Count)
		{
			bool flag = dialogue.IsChoiceEnabledAt(index);
			if (flag != isInteractable)
			{
				SetInteractable(flag);
			}
		}
	}
}

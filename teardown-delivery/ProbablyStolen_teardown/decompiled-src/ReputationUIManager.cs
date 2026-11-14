using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReputationUIManager : MonoBehaviour
{
	public GameObject uiPanel;

	public GameObject layout;

	public GameObject reputationPrefab;

	public GameObject reputationPerkPrefab;

	public GameObject reputationTooltip;

	public RectTransform reputationTooltipRectTransform;

	public TextMeshProUGUI perkTitle;

	public TextMeshProUGUI perkStatus;

	public TextMeshProUGUI perkProgress;

	public TextMeshProUGUI perkdescription;

	public TextMeshProUGUI perkFlavorText;

	public GameObject reputationNotifier;

	public TextMeshProUGUI reputationNotifierTMP;

	public Image reputationNotifierImage;

	public float notifierTimerDelay = 3f;

	public float notifierTimer;

	public const float NOTIFIER_SECONDS = 3f;

	public const float NOTIFIER_MANUAL_ADVANCE_BONUS = 3f;

	public Sprite spriteStar;

	public Sprite spriteHappy;

	public Sprite spriteSmile;

	public Sprite spriteSad;

	public Sprite spriteAngry;

	public Sprite spriteSkull;

	public static ReputationUIManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private static float GetNotifierDuration()
	{
		bool flag = EmporiumOption.Instance != null && !EmporiumOption.Instance.autoNextDialog;
		return 3f + (flag ? 3f : 0f);
	}

	private void Start()
	{
	}

	private void Update()
	{
		UpdateTooltipPosition();
		if (notifierTimer > 0f)
		{
			notifierTimer -= Time.deltaTime;
		}
		else
		{
			reputationNotifier.SetActive(value: false);
		}
	}

	public void OpenUI()
	{
		perkTitle.text = "";
		perkStatus.text = "";
		perkProgress.text = "";
		perkdescription.text = "";
		perkFlavorText.text = "";
		EmporiumAudioPlayer.Instance.Play();
		uiPanel.SetActive(value: true);
		StoreReputation.UpdateReputation();
		for (int num = layout.transform.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(layout.transform.GetChild(num).gameObject);
		}
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			Object.Instantiate(reputationPrefab, layout.transform).GetComponent<ReputationElement>().Init(storeReputation);
		}
		TutorialUIManager.Instance.OnReputationTutorialOpened();
	}

	public void CloseUI()
	{
		EmporiumAudioPlayer.Instance.Play();
		uiPanel.SetActive(value: false);
		reputationTooltip.SetActive(value: false);
	}

	public void ToggleUI()
	{
		if (uiPanel.activeSelf)
		{
			CloseUI();
		}
		else
		{
			OpenUI();
		}
	}

	private void UpdateTooltipPosition()
	{
		if (reputationTooltip.activeSelf)
		{
			Vector3 tooltipPosition = UIHelper.GetTooltipPosition(reputationTooltipRectTransform.rect.width, reputationTooltipRectTransform.rect.height, 10f, 10f);
			tooltipPosition.z = reputationTooltip.transform.position.z;
			reputationTooltip.transform.position = tooltipPosition;
		}
	}

	public void UpdateToolTipContent(StoreReputationPerk perk)
	{
		perkTitle.text = perk.GetLocalizedPerkName();
		if (perk.isUnlocked || perk.isAlwaysActive)
		{
			TextMeshProUGUI textMeshProUGUI = perkTitle;
			textMeshProUGUI.text = textMeshProUGUI.text + " " + LocHelper.GetLocalizedUI("ui_rep_active") + " ";
		}
		else
		{
			TextMeshProUGUI textMeshProUGUI2 = perkTitle;
			textMeshProUGUI2.text = textMeshProUGUI2.text + " " + LocHelper.GetLocalizedUI("ui_rep_inactive") + " ";
		}
		TextMeshProUGUI textMeshProUGUI3 = perkTitle;
		textMeshProUGUI3.text = textMeshProUGUI3.text + perk.perkThreshold + "/200";
		perkdescription.text = perk.GetLocalizedDescription();
		perkFlavorText.text = LocHelper.WrapInLocalizedQuotes(perk.GetLocalizedFlavorText());
	}

	public void NotifyReputation(string factionId, int value)
	{
		reputationNotifier.SetActive(value: true);
		notifierTimer = GetNotifierDuration();
		string text = ((value > 0) ? LocHelper.GetLocalizedUI("ui_rep_gained", StoreReputation.GetFactionNameByID(factionId)) : LocHelper.GetLocalizedUI("ui_rep_lost", StoreReputation.GetFactionNameByID(factionId)));
		reputationNotifierTMP.text = text;
	}

	public void Notify(string message, string color = "default")
	{
		reputationNotifier.SetActive(value: true);
		notifierTimer = GetNotifierDuration();
		reputationNotifierImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite(color + "p");
		reputationNotifierTMP.text = message;
	}
}

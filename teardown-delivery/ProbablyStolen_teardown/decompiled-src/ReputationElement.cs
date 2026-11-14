using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReputationElement : MonoBehaviour
{
	public TextMeshProUGUI reputationTitle;

	public TextMeshProUGUI reputationStatus;

	public TextMeshProUGUI reputationNumber;

	public Image greenImage;

	public Image redImage;

	public const int BAR_HEIGHT = 262;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void UpdateBar(int value)
	{
		if (value <= 200 && value >= -200)
		{
			if (value > 0)
			{
				redImage.gameObject.SetActive(value: false);
				greenImage.gameObject.SetActive(value: true);
				greenImage.rectTransform.sizeDelta = new Vector2(24f, GetBarHeight(value));
			}
			else
			{
				greenImage.gameObject.SetActive(value: false);
				redImage.gameObject.SetActive(value: true);
				redImage.rectTransform.sizeDelta = new Vector2(24f, GetBarHeight(-value));
			}
		}
	}

	public float GetBarHeight(int value)
	{
		return 65.5f * ((float)value / 100f);
	}

	public void Init(StoreReputation storeReputation)
	{
		reputationTitle.text = storeReputation.factionDisplay;
		reputationTitle.color = MetaHelper.HexToColor(storeReputation.factionColor2);
		reputationStatus.text = StoreReputation.GetReputationStatus(storeReputation.GetReputation());
		reputationNumber.text = storeReputation.GetReputationExact().ToString("F2") + "/200";
		UpdateBar(storeReputation.GetReputation());
		foreach (StoreReputationPerk perk in storeReputation.perks)
		{
			PerkElement component = Object.Instantiate(ReputationUIManager.Instance.reputationPerkPrefab, base.transform).GetComponent<PerkElement>();
			component.Init(perk);
			component.GetComponent<RectTransform>().anchoredPosition = new Vector2(-19f, GetBarHeight(perk.perkThreshold));
		}
	}

	public void SetTitleColor()
	{
	}
}

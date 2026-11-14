using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerkElement : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Image image;

	private StoreReputationPerk perk;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetLocked()
	{
	}

	public void SetUnlocked()
	{
	}

	public void Init(StoreReputationPerk perk)
	{
		this.perk = perk;
		image.color = Color.white;
		if ((perk.perkType != -1 || !perk.isUnlocked) && perk.perkType == 1)
		{
			_ = perk.isUnlocked;
		}
		SetIcon();
	}

	public void SetIcon()
	{
		if (perk != null)
		{
			if (perk.perkThreshold >= 100)
			{
				image.sprite = ReputationUIManager.Instance.spriteStar;
			}
			else if (perk.perkThreshold >= 50)
			{
				image.sprite = ReputationUIManager.Instance.spriteHappy;
			}
			else if (perk.perkThreshold >= 0)
			{
				image.sprite = ReputationUIManager.Instance.spriteSmile;
			}
			else if (perk.perkThreshold >= -50)
			{
				image.sprite = ReputationUIManager.Instance.spriteSad;
			}
			else if (perk.perkThreshold > -100)
			{
				image.sprite = ReputationUIManager.Instance.spriteAngry;
			}
			else
			{
				image.sprite = ReputationUIManager.Instance.spriteSkull;
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ReputationUIManager.Instance.UpdateToolTipContent(perk);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkElement : MonoBehaviour
{
	public Button button;

	public GameObject upgradeContent;

	private Vector2 hoverPosition = new Vector2(0f, -2f);

	private Vector2 pressedPosition = new Vector2(0f, -4f);

	public Sprite unlockedSprite;

	public Image image;

	public Image icon;

	public bool isUnlocked;

	public string id;

	public GameObject repeatableIcon;

	public TextMeshProUGUI costTMP;

	public GameObject checkIcon;

	public GameObject padlockIcon;

	public Material greyMaterial;

	public Image costArea;

	public Sprite costAreaNormal;

	public Sprite costAreaBought;

	private void Start()
	{
	}

	private void Update()
	{
		UpdateElement();
	}

	public void OnHover()
	{
		if (!isUnlocked)
		{
			upgradeContent.transform.localPosition = hoverPosition;
		}
	}

	public void OnPressed()
	{
		if (id.Equals(""))
		{
			Debug.Log("Network Element Id not Set");
		}
		WildUIManager.Instance.OnElementClicked(id);
		upgradeContent.transform.localPosition = pressedPosition;
	}

	public void OnReleased()
	{
		if (!isUnlocked)
		{
			upgradeContent.transform.localPosition = Vector2.zero;
		}
	}

	public void Unlock()
	{
		image.sprite = unlockedSprite;
		upgradeContent.transform.localPosition = Vector2.zero;
		isUnlocked = true;
		icon.material = greyMaterial;
		costArea.sprite = costAreaBought;
	}

	public void UpdateElement()
	{
		if (PlayerStore.Instance.networkUpgrade[id].IsUnlocked())
		{
			Unlock();
			checkIcon.SetActive(value: true);
			padlockIcon.SetActive(value: false);
			icon.material = greyMaterial;
			costArea.sprite = costAreaBought;
		}
		else
		{
			checkIcon.SetActive(value: false);
			padlockIcon.SetActive(value: false);
			icon.material = null;
			costArea.sprite = costAreaNormal;
		}
		if (PlayerStore.Instance.networkUpgrade[id].isRepeatable)
		{
			repeatableIcon.SetActive(value: true);
		}
		else
		{
			repeatableIcon.SetActive(value: false);
		}
		costTMP.text = PlayerStore.Instance.networkUpgrade[id].GetCost().ToString() ?? "";
	}

	public void OnEnable()
	{
		costTMP.text = PlayerStore.Instance.networkUpgrade[id].GetCost().ToString() ?? "";
	}
}

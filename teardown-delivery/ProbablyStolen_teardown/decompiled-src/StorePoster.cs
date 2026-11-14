using UnityEngine;

public class StorePoster : MonoBehaviour, IClickable
{
	public SpriteRenderer spriteRenderer;

	public Sprite normale;

	public Sprite normaleNoShadow;

	public Sprite pulled;

	public bool isPulled;

	public static StorePoster Instance { get; private set; }

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

	public void OnClick()
	{
		if (isPulled)
		{
			PullDown();
		}
		else
		{
			PullUp();
		}
	}

	public void PullDown(bool mute = false)
	{
		if (!mute)
		{
			PaperUIManager.Instance.audioSourcePickup.Play();
		}
		if (!NetworkUpgrade.IsMarketplaceRequestsUnlocked())
		{
			spriteRenderer.sprite = normaleNoShadow;
		}
		else
		{
			spriteRenderer.sprite = normale;
		}
		EmporiumEntry.Instance.hiddenWindow.rectTransform.gameObject.SetActive(value: false);
		isPulled = false;
	}

	public void PullUp(bool mute = false)
	{
		if (!mute)
		{
			PaperUIManager.Instance.audioSourceDrop.Play();
		}
		spriteRenderer.sprite = pulled;
		EmporiumEntry.Instance.hiddenWindow.rectTransform.gameObject.SetActive(value: true);
		isPulled = true;
	}

	public static void LoadStorePoster()
	{
		if (PlayerStore.Instance.storePosterIsPulled)
		{
			Instance.PullUp(mute: true);
		}
		else
		{
			Instance.PullDown(mute: true);
		}
	}

	public void SaveStorePoster()
	{
		PlayerStore.Instance.storePosterIsPulled = isPulled;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}

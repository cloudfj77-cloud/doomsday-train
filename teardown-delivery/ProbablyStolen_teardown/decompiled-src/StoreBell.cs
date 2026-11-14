using UnityEngine;
using UnityEngine.EventSystems;

public class StoreBell : TooltipInterface, IHoldClickable, IClickable, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public AudioSource audioSource;

	[Header("Button Sprites")]
	public SpriteRenderer buttonRenderer;

	public Sprite spriteNormal;

	public Sprite spriteHover;

	public Sprite spriteClick;

	private bool isHoveringButton;

	private Interactable interactable;

	private bool keyHoldActive;

	private float keyHoldStartTime;

	public float HoldDuration => 0.4f;

	public bool ShouldRequireHold()
	{
		if (!EmporiumOption.Instance.instantCallNextCustomer)
		{
			return PlayerStore.Instance.IsClientPresent();
		}
		return false;
	}

	public void OnClick()
	{
		RingBell();
	}

	private void Start()
	{
		SetButtonSprite(spriteNormal);
		interactable = GetComponent<Interactable>();
	}

	private void Update()
	{
		HandleHotkey();
	}

	private bool CanUseHotkey()
	{
		if (StoreUIManager.Instance == null || StoreUIManager.Instance.IsTyping())
		{
			return false;
		}
		if (Interactable.isPaused)
		{
			return false;
		}
		if (interactable != null && !interactable.isAvailable)
		{
			return false;
		}
		return true;
	}

	private void HandleHotkey()
	{
		if (!CanUseHotkey())
		{
			CancelKeyHold();
		}
		else if (KeyBindHelper.GetKeyDown("call_next_customer"))
		{
			if (ShouldRequireHold())
			{
				keyHoldActive = true;
				keyHoldStartTime = Time.unscaledTime;
				SetButtonSprite(spriteClick);
				CustomUIManager.Instance?.ShowHoldRing(0f);
			}
			else
			{
				RingBell();
			}
		}
		else
		{
			if (!keyHoldActive)
			{
				return;
			}
			if (KeyBindHelper.GetKey("call_next_customer"))
			{
				float progress = Mathf.Clamp01((Time.unscaledTime - keyHoldStartTime) / HoldDuration);
				CustomUIManager.Instance?.ShowHoldRing(progress);
				return;
			}
			bool num = Time.unscaledTime - keyHoldStartTime >= HoldDuration;
			CancelKeyHold();
			if (num)
			{
				RingBell();
			}
		}
	}

	private void CancelKeyHold()
	{
		if (keyHoldActive)
		{
			keyHoldActive = false;
			SetButtonSprite(isHoveringButton ? spriteHover : spriteNormal);
			CustomUIManager.Instance?.HideHoldRing();
		}
	}

	private void OnDisable()
	{
		CancelKeyHold();
	}

	public void RingBell()
	{
		TutorialUIManager.Instance.BellRing();
		TutorialUIManager.Instance.CloseBarterTutorial();
		if (PlayerStore.Instance.lockNextBellFlag > 0)
		{
			return;
		}
		audioSource.Play();
		if (PlayerStore.Instance.storeState == PlayerStore.StoreState.CLOSED || PlayerStore.Instance.storeState != PlayerStore.StoreState.OPEN)
		{
			return;
		}
		if (PlayerStore.Instance.IsClientPresent())
		{
			if (PlayerStore.Instance.IsClientDimissable() || PlayerStore.Instance.devMode)
			{
				PlayerStore.Instance.TryPlayerDismissCurrentClient();
			}
		}
		else
		{
			PlayerStore.Instance.TryCallNextClient();
		}
	}

	public override string GetTooltip()
	{
		if (PlayerStore.Instance.lockNextBellFlag > 0)
		{
			return LocHelper.GetLocalizedUI("inte_bell_cant");
		}
		if (PlayerStore.Instance.storeState == PlayerStore.StoreState.CLOSED || PlayerStore.Instance.storeState == PlayerStore.StoreState.MORNING)
		{
			return LocHelper.GetLocalizedUI("inte_bell_closed");
		}
		if (PlayerStore.Instance.IsClientPresent())
		{
			if (PlayerStore.Instance.IsClientDimissable())
			{
				return LocHelper.GetLocalizedUI("inte_bell_dismiss");
			}
			return LocHelper.GetLocalizedUI("inte_bell_cant_dismiss");
		}
		return LocHelper.GetLocalizedUI("inte_bell_next");
	}

	public override bool IsTooltipEnabled()
	{
		return true;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isHoveringButton = true;
		SetButtonSprite(spriteHover);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHoveringButton = false;
		SetButtonSprite(spriteNormal);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		SetButtonSprite(spriteClick);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		SetButtonSprite(isHoveringButton ? spriteHover : spriteNormal);
	}

	private void SetButtonSprite(Sprite sprite)
	{
		if (buttonRenderer != null && sprite != null)
		{
			buttonRenderer.sprite = sprite;
		}
	}
}

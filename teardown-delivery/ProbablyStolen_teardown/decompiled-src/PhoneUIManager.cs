using TMPro;
using UnityEngine;

public class PhoneUIManager : MonoBehaviour
{
	public GameObject phonePanel;

	public AudioSource phonePickup;

	public AudioSource phoneDrop;

	public AudioSource phoneCalling;

	public AudioSource phoneNoNumber;

	public AudioSource phoneClientDrop;

	public AudioSource phoneClientPickup;

	public PhoneBookUIManager phoneBookUIManager;

	private long currentNumber;

	private float clickTimer;

	private const float clickWindow = 2.25f;

	private bool isCalling;

	private float callingTimer;

	private float callTimerRoll;

	private bool isTalking;

	private float sequenceTimer;

	public StorePhoneClient currentPhoneClient;

	public TextMeshProUGUI numberDisplay;

	public TextMeshProUGUI statusDisplay;

	private PhoneButton[] phoneButtons;

	public static PhoneUIManager Instance { get; private set; }

	public bool isQuickDialCall { get; private set; }

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

	private void Start()
	{
	}

	private void Update()
	{
		if (clickTimer > 0f && !isTalking)
		{
			clickTimer -= Time.deltaTime;
			if (clickTimer <= 0f)
			{
				StartCalling();
			}
		}
		if (isCalling && !isTalking)
		{
			callingTimer += Time.deltaTime;
			if (callingTimer >= callTimerRoll)
			{
				phoneCalling.Stop();
				isCalling = false;
				HandleCall(currentNumber);
			}
		}
		if (sequenceTimer > 0f && !isCalling)
		{
			sequenceTimer -= Time.deltaTime;
			if (sequenceTimer <= 0f)
			{
				StartPhoneDialog(currentNumber);
			}
		}
		if (phonePanel.activeSelf)
		{
			numberDisplay.text = ((currentNumber != 0L) ? currentNumber.ToString() : "");
			if (Input.GetKeyDown(KeyCode.Keypad0))
			{
				PressKeypad(0);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad1))
			{
				PressKeypad(1);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad2))
			{
				PressKeypad(2);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad3))
			{
				PressKeypad(3);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad4))
			{
				PressKeypad(4);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad5))
			{
				PressKeypad(5);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad6))
			{
				PressKeypad(6);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad7))
			{
				PressKeypad(7);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad8))
			{
				PressKeypad(8);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad9))
			{
				PressKeypad(9);
			}
		}
	}

	public void StartCalling()
	{
		Debug.Log(currentNumber);
		phoneCalling.Play();
		isCalling = true;
		callingTimer = 0f;
		statusDisplay.text = LocHelper.GetLocalizedUI("ui_phone_calling");
		callTimerRoll = RNG.GetRandomIntExculsive(1, 6);
	}

	public void HandleCall(long currentNumber)
	{
		Debug.Log("Handle Call");
		if (StorePhoneClient.IsNumberExist(currentNumber) && PlayerStore.Instance.PhoneClientDict[currentNumber].phoneState == StorePhoneClient.PhoneState.NotResponding)
		{
			this.currentNumber = 0L;
			phoneClientDrop.Play();
			statusDisplay.text = LocHelper.GetLocalizedUI("ui_phone_no_answer");
			return;
		}
		if (StorePhoneClient.IsNumberExist(currentNumber) && PlayerStore.Instance.PhoneClientDict[currentNumber].phoneState == StorePhoneClient.PhoneState.None)
		{
			this.currentNumber = 0L;
			NoNumber();
			return;
		}
		if (StorePhoneClient.IsPhoneClientAtStore(currentNumber))
		{
			this.currentNumber = 0L;
			phoneClientDrop.Play();
			statusDisplay.text = LocHelper.GetLocalizedUI("ui_phone_no_answer");
			return;
		}
		if (StorePhoneClient.IsNumberExist(currentNumber))
		{
			StorePhoneClient phoneClientByNumber = StorePhoneClient.GetPhoneClientByNumber(currentNumber);
			if (phoneClientByNumber != null && phoneClientByNumber.phoneClientType == StorePhoneClient.PhoneClientType.Supplier && StoreReputation.IsPerkUnlocked("LLAVOIDED"))
			{
				this.currentNumber = 0L;
				phoneClientDrop.Play();
				statusDisplay.text = LocHelper.GetLocalizedUI("ui_phone_no_answer");
				return;
			}
		}
		if (StorePhoneClient.IsNumberExist(currentNumber))
		{
			StartPhoneDialogSequence();
		}
		else
		{
			NoNumber();
		}
	}

	public void StartPhoneDialogSequence()
	{
		phoneClientPickup.Play();
		sequenceTimer = 1.5f;
		statusDisplay.text = LocHelper.GetLocalizedUI("ui_phone_in_call");
	}

	public void StartPhoneDialog(long currentNumber)
	{
		if (!PlayerStore.Instance.PhoneClientDict.ContainsKey(currentNumber))
		{
			Debug.Log("A number that should exist is not found");
			return;
		}
		Dialogue callDialog = PlayerStore.Instance.PhoneClientDict[currentNumber].GetCallDialog();
		if (callDialog != null)
		{
			currentPhoneClient = PlayerStore.Instance.PhoneClientDict[currentNumber];
			currentPhoneClient.dialedBefore = true;
			isTalking = true;
			DialoguePhoneUIManager.Instance.StartDialogue(callDialog);
		}
	}

	public void OnPhoneDialogEnd(bool playEndSound = true)
	{
		if (playEndSound)
		{
			phoneClientDrop.Play();
		}
		isTalking = false;
		currentNumber = 0L;
		currentPhoneClient = null;
		statusDisplay.text = LocHelper.GetLocalizedUI("ui_phone_ready");
	}

	public void NoNumber()
	{
		phoneNoNumber.Play();
		currentNumber = 0L;
		statusDisplay.text = LocHelper.GetLocalizedUI("ui_phone_no_answer");
	}

	public void OpenUI()
	{
		if (!phonePanel.activeSelf)
		{
			phonePickup.Play();
		}
		phonePanel.SetActive(value: true);
		if (IsPhoneBookShown())
		{
			phoneBookUIManager.OpenUI();
		}
		else
		{
			phoneBookUIManager.CloseUI();
		}
		statusDisplay.text = LocHelper.GetLocalizedUI("ui_phone_ready");
	}

	public void CloseUI()
	{
		phoneBookUIManager.CloseUI();
		DialoguePhoneUIManager.Instance.InterruptDialog();
		phonePanel.SetActive(value: false);
		phoneDrop.Play();
		StopCall();
	}

	private void PressKeypad(int number)
	{
		if (phoneButtons == null)
		{
			phoneButtons = phonePanel.GetComponentsInChildren<PhoneButton>(includeInactive: true);
		}
		PhoneButton[] array = phoneButtons;
		foreach (PhoneButton phoneButton in array)
		{
			if (phoneButton.number == number)
			{
				phoneButton.SimulateClick();
				return;
			}
		}
		PressNumber(number);
	}

	public void PressNumber(int number)
	{
		long num = number;
		if (number == -1)
		{
			StopCall();
		}
		else if (!isTalking)
		{
			clickTimer = 2.25f;
			statusDisplay.text = LocHelper.GetLocalizedUI("ui_phone_dialing");
			currentNumber = currentNumber * 10 + num;
			if (currentNumber > 999999999)
			{
				string text = currentNumber.ToString();
				text = text.Substring(1);
				currentNumber = long.Parse(text);
			}
		}
	}

	public void AutoCall(long number)
	{
		StopCall();
		currentNumber = number;
		if (WillAnswerCall(number))
		{
			isQuickDialCall = true;
			StartPhoneDialog(number);
			isQuickDialCall = false;
			if (isTalking)
			{
				phoneClientPickup.Play();
				statusDisplay.text = LocHelper.GetLocalizedUI("ui_phone_in_call");
				return;
			}
		}
		phoneClientPickup.Play();
		clickTimer = 2.25f;
	}

	public static bool WillAnswerCall(long number)
	{
		if (!StorePhoneClient.IsNumberExist(number))
		{
			return false;
		}
		if (PlayerStore.Instance.PhoneClientDict[number].phoneState == StorePhoneClient.PhoneState.NotResponding)
		{
			return false;
		}
		if (PlayerStore.Instance.PhoneClientDict[number].phoneState == StorePhoneClient.PhoneState.None)
		{
			return false;
		}
		if (StorePhoneClient.IsPhoneClientAtStore(number))
		{
			return false;
		}
		StorePhoneClient phoneClientByNumber = StorePhoneClient.GetPhoneClientByNumber(number);
		if (phoneClientByNumber != null && phoneClientByNumber.phoneClientType == StorePhoneClient.PhoneClientType.Supplier && StoreReputation.IsPerkUnlocked("LLAVOIDED"))
		{
			return false;
		}
		return true;
	}

	public void StopCall()
	{
		Debug.Log("Interrupt Call");
		clickTimer = -1f;
		callingTimer = 0f;
		sequenceTimer = 0f;
		currentNumber = 0L;
		DialoguePhoneUIManager.Instance.InterruptDialog();
		isTalking = false;
		isCalling = false;
		phoneCalling.Stop();
		phoneNoNumber.Stop();
		phoneClientPickup.Stop();
		statusDisplay.text = LocHelper.GetLocalizedUI("ui_phone_ready");
	}

	public bool IsPhoneBookShown()
	{
		foreach (StorePhoneClient value in PlayerStore.Instance.PhoneClientDict.Values)
		{
			if (value.ShownInPhoneBook())
			{
				return true;
			}
		}
		return false;
	}
}

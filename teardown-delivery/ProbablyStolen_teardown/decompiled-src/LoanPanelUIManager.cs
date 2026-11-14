using System.Globalization;
using TMPro;
using UnityEngine;

public class LoanPanelUIManager : MonoBehaviour
{
	public GameObject panel;

	public TextMeshProUGUI body;

	public static LoanPanelUIManager Instance { get; private set; }

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
	}

	public void OpenUI()
	{
		panel.SetActive(value: true);
		PaperUIManager.Instance.audioSourcePickup.Play();
		PlayerStore instance = PlayerStore.Instance;
		bool num = instance.propertyBoughtAt != 0;
		int num2 = (num ? instance.propertyBoughtAt : instance.GetStoreBuyPrice(applyDownPayment: true));
		int num3 = ((!num) ? instance.GetMortgageDownPayment() : ((instance.mortgageDownPaid != 0) ? instance.mortgageDownPaid : 3000));
		int num4 = ((!num) ? PlayerStore.GetMortgageWeeks() : ((instance.mortgageTotalWeeks != 0) ? instance.mortgageTotalWeeks : 14));
		int num5 = ((!num) ? Difficulty.Rules.mortgageInterestRate : ((instance.mortgageInterestRateTaken != 0) ? instance.mortgageInterestRateTaken : 100));
		int num6 = ((instance.mortgageWeeklyPayment != 0) ? instance.mortgageWeeklyPayment : PlayerStore.CalculateWeeklyPayment(num2, num5, num4));
		string text = ((float)num5 / 52f).ToString("0.###", CultureInfo.InvariantCulture);
		body.text = "\n" + LocHelper.GetLocalizedUI("ui_loan_contract_amount", num2) + "\n" + LocHelper.GetLocalizedUI("ui_loan_contract_down_payment", num3) + "\n" + LocHelper.GetLocalizedUI("ui_loan_contract_interest", num5, text) + "\n" + LocHelper.GetLocalizedUI("ui_loan_contract_repayment", num4, num6) + "\n";
	}

	public void CloseUI()
	{
		panel.SetActive(value: false);
		PaperUIManager.Instance.audioSourceDrop.Play();
	}
}

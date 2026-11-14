public class DifficultyRules
{
	public int retailMarkup;

	public float rentIncreaseMultiplier;

	public bool landlordGraceOwnedAtStart;

	public int powerCostMultiplier;

	public int insuranceCostMultiplier;

	public int startPowerCostBonus;

	public int startJanitorialCostBonus;

	public int startInsuranceCostBonus;

	public int appraisalJoinCostMultiplier;

	public int vendorPermitFee;

	public int newCustomerDiscount;

	public float upgradePriceMultiplier;

	public int inventorMarkup;

	public float supplierMarkupMultiplier;

	public int retiredSupplierMarkup;

	public int merchantDiscount;

	public int scalperMarkupBase;

	public float scalperMarkupPerDay;

	public int propertyBasePrice;

	public int propertyPriceIncreasePerDay;

	public int propertyPriceIncreaseCap;

	public int mortgageInterestRate;

	public int mortgageWeeks;

	public float mortgageDownPaymentShare;

	public float reputationGainMultiplier;

	public float reputationLossMultiplier;

	public float appraisalProgressGainMultiplier;

	public float appraisalProgressLossMultiplier;

	public float tradeFavorRate;

	public float wildServiceFavorRefund;

	public int networkIntroFavor;

	public int augKillPresenceDrop;

	public int augLeftAlivePresenceRise;

	public bool augWarning;

	public bool augGameOverAtFullPresence;

	public int louDemandValue;

	public bool wildBailout;

	public bool wildReminders;

	public bool displayCaseTip;

	public bool eventEndCountdown;

	public bool impostorTell;

	public DifficultyRules Clone()
	{
		return (DifficultyRules)MemberwiseClone();
	}
}

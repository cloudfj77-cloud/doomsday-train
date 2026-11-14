using System;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty
{
	public class DifficultyEffect
	{
		public string key;

		public Func<DifficultyRules, DifficultyRules, bool> differs;

		public Func<DifficultyRules, string> describe;
	}

	public const string COZY = "cozy";

	public const string NORMAL = "normal";

	public const string HARD = "hard";

	public const string CHALLENGE = "challenge";

	public string id;

	public int order;

	public string source;

	public Func<string> titleProvider;

	public Func<string> subtitleProvider;

	public string iconKey;

	public DifficultyRules rules;

	public readonly List<Func<string>> notes = new List<Func<string>>();

	private static List<Difficulty> vanilla;

	private static readonly List<Difficulty> modded = new List<Difficulty>();

	private static readonly HashSet<string> warnedUnknown = new HashSet<string>();

	private static List<DifficultyEffect> effects;

	public static Difficulty Normal => Get("normal");

	public static Difficulty Hard => Get("hard");

	public static Difficulty Current
	{
		get
		{
			if (PlayerStore.Instance != null)
			{
				return Get(ResolveId(PlayerStore.Instance.difficultyId, PlayerStore.Instance.isHardMode));
			}
			if (NewGameData.Instance != null)
			{
				return Get(ResolveId(NewGameData.Instance.difficultyId, NewGameData.Instance.hardMode));
			}
			return Normal;
		}
	}

	public static DifficultyRules Rules => Current.rules;

	public static List<DifficultyEffect> Effects => effects ?? (effects = CreateEffects());

	public string GetTitle()
	{
		return Resolve(titleProvider);
	}

	public string GetSubtitle()
	{
		return Resolve(subtitleProvider);
	}

	public bool IsAtLeast(string difficultyId)
	{
		Difficulty difficulty = Get(difficultyId);
		if (difficulty != null)
		{
			return order >= difficulty.order;
		}
		return false;
	}

	private string Resolve(Func<string> provider)
	{
		if (provider == null)
		{
			return id ?? "";
		}
		try
		{
			return provider() ?? "";
		}
		catch (Exception arg)
		{
			Debug.LogError($"[Difficulty] Text provider of '{id}' threw: {arg}");
			return id ?? "";
		}
	}

	public static List<Difficulty> GetAll()
	{
		List<Difficulty> list = new List<Difficulty>(Vanilla());
		list.AddRange(modded);
		list.Sort(delegate(Difficulty a, Difficulty b)
		{
			int num = a.order.CompareTo(b.order);
			return (num == 0) ? string.CompareOrdinal(a.id, b.id) : num;
		});
		return list;
	}

	public static Difficulty Get(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		foreach (Difficulty item in modded)
		{
			if (item.id == id)
			{
				return item;
			}
		}
		foreach (Difficulty item2 in Vanilla())
		{
			if (item2.id == id)
			{
				return item2;
			}
		}
		return null;
	}

	public static string ResolveId(string difficultyId, bool legacyHardMode)
	{
		if (!string.IsNullOrEmpty(difficultyId))
		{
			if (Get(difficultyId) != null)
			{
				return difficultyId;
			}
			if (!warnedUnknown.Contains(difficultyId))
			{
				warnedUnknown.Add(difficultyId);
				Debug.LogWarning("[Difficulty] Unknown difficulty '" + difficultyId + "' (a mod's?); playing it as " + (legacyHardMode ? "hard" : "normal"));
			}
		}
		if (!legacyHardMode)
		{
			return "normal";
		}
		return "hard";
	}

	public static bool IsHardOrAbove()
	{
		return Current.IsAtLeast("hard");
	}

	public static void Register(Difficulty difficulty)
	{
		if (difficulty == null || string.IsNullOrEmpty(difficulty.id))
		{
			Debug.LogError("[Difficulty] A difficulty needs an id");
			return;
		}
		foreach (Difficulty item in Vanilla())
		{
			if (item.id == difficulty.id)
			{
				Debug.LogError("[Difficulty] '" + difficulty.id + "' is a vanilla difficulty id; use ModHelper.ModifyDifficulty to change it, or pick another id");
				return;
			}
		}
		if (difficulty.rules == null)
		{
			difficulty.rules = Normal.rules.Clone();
		}
		modded.RemoveAll((Difficulty d) => d.id == difficulty.id);
		modded.Add(difficulty);
	}

	public static int RemoveMod(string modId)
	{
		if (string.IsNullOrEmpty(modId))
		{
			return 0;
		}
		return modded.RemoveAll((Difficulty d) => d.source == modId);
	}

	private static List<Difficulty> Vanilla()
	{
		if (vanilla != null)
		{
			return vanilla;
		}
		DifficultyRules difficultyRules = new DifficultyRules
		{
			retailMarkup = 20,
			rentIncreaseMultiplier = 1.025f,
			landlordGraceOwnedAtStart = true,
			powerCostMultiplier = 1,
			insuranceCostMultiplier = 1,
			startPowerCostBonus = 0,
			startJanitorialCostBonus = 0,
			startInsuranceCostBonus = 0,
			appraisalJoinCostMultiplier = 1,
			vendorPermitFee = 500,
			newCustomerDiscount = 75,
			upgradePriceMultiplier = 1f,
			supplierMarkupMultiplier = 1f,
			inventorMarkup = 0,
			retiredSupplierMarkup = 0,
			merchantDiscount = 10,
			scalperMarkupBase = 0,
			scalperMarkupPerDay = 0.5f,
			propertyBasePrice = 12000,
			propertyPriceIncreasePerDay = 25,
			propertyPriceIncreaseCap = 3000,
			mortgageInterestRate = 25,
			mortgageWeeks = 8,
			mortgageDownPaymentShare = 0.2f,
			reputationGainMultiplier = 1.5f,
			reputationLossMultiplier = 0.5f,
			appraisalProgressGainMultiplier = 1.25f,
			appraisalProgressLossMultiplier = 0.5f,
			tradeFavorRate = 0.1f,
			wildServiceFavorRefund = 0.75f,
			networkIntroFavor = 300,
			augKillPresenceDrop = 25,
			augLeftAlivePresenceRise = 10,
			augWarning = true,
			augGameOverAtFullPresence = false,
			louDemandValue = 500,
			wildBailout = true,
			wildReminders = true,
			displayCaseTip = true,
			eventEndCountdown = true,
			impostorTell = true
		};
		DifficultyRules difficultyRules2 = new DifficultyRules
		{
			retailMarkup = 15,
			rentIncreaseMultiplier = 1.05f,
			landlordGraceOwnedAtStart = true,
			powerCostMultiplier = 1,
			insuranceCostMultiplier = 1,
			startPowerCostBonus = 0,
			startJanitorialCostBonus = 0,
			startInsuranceCostBonus = 0,
			appraisalJoinCostMultiplier = 1,
			vendorPermitFee = 1500,
			newCustomerDiscount = 75,
			upgradePriceMultiplier = 1f,
			supplierMarkupMultiplier = 1f,
			inventorMarkup = 15,
			retiredSupplierMarkup = 15,
			merchantDiscount = 15,
			scalperMarkupBase = 15,
			scalperMarkupPerDay = 1f,
			propertyBasePrice = 12000,
			propertyPriceIncreasePerDay = 50,
			propertyPriceIncreaseCap = 5000,
			mortgageInterestRate = 50,
			mortgageWeeks = 12,
			mortgageDownPaymentShare = 0.15f,
			reputationGainMultiplier = 1f,
			reputationLossMultiplier = 1f,
			appraisalProgressGainMultiplier = 1f,
			appraisalProgressLossMultiplier = 1f,
			tradeFavorRate = 0.05f,
			wildServiceFavorRefund = 0.5f,
			networkIntroFavor = 200,
			augKillPresenceDrop = 25,
			augLeftAlivePresenceRise = 10,
			augWarning = true,
			augGameOverAtFullPresence = false,
			louDemandValue = 750,
			wildBailout = true,
			wildReminders = true,
			displayCaseTip = true,
			eventEndCountdown = true,
			impostorTell = true
		};
		DifficultyRules difficultyRules3 = new DifficultyRules
		{
			retailMarkup = 0,
			rentIncreaseMultiplier = 1.1f,
			landlordGraceOwnedAtStart = false,
			powerCostMultiplier = 2,
			insuranceCostMultiplier = 2,
			startPowerCostBonus = 3,
			startJanitorialCostBonus = 3,
			startInsuranceCostBonus = 4,
			appraisalJoinCostMultiplier = 2,
			vendorPermitFee = 2000,
			newCustomerDiscount = 35,
			upgradePriceMultiplier = 1.5f,
			supplierMarkupMultiplier = 2f,
			inventorMarkup = 50,
			retiredSupplierMarkup = 30,
			merchantDiscount = 20,
			scalperMarkupBase = 25,
			scalperMarkupPerDay = 2f,
			propertyBasePrice = 24000,
			propertyPriceIncreasePerDay = 50,
			propertyPriceIncreaseCap = 10000,
			mortgageInterestRate = 75,
			mortgageWeeks = 16,
			mortgageDownPaymentShare = 0.125f,
			reputationGainMultiplier = 0.75f,
			reputationLossMultiplier = 1.25f,
			appraisalProgressGainMultiplier = 1f,
			appraisalProgressLossMultiplier = 1.5f,
			tradeFavorRate = 0f,
			wildServiceFavorRefund = 0.25f,
			networkIntroFavor = 100,
			augKillPresenceDrop = 10,
			augLeftAlivePresenceRise = 15,
			augWarning = false,
			augGameOverAtFullPresence = true,
			louDemandValue = 1000,
			wildBailout = false,
			wildReminders = false,
			displayCaseTip = false,
			eventEndCountdown = false,
			impostorTell = false
		};
		DifficultyRules difficultyRules4 = new DifficultyRules
		{
			retailMarkup = 0,
			rentIncreaseMultiplier = 1.1f,
			landlordGraceOwnedAtStart = false,
			powerCostMultiplier = 2,
			insuranceCostMultiplier = 3,
			startPowerCostBonus = 3,
			startJanitorialCostBonus = 3,
			startInsuranceCostBonus = 5,
			appraisalJoinCostMultiplier = 3,
			vendorPermitFee = 3000,
			newCustomerDiscount = 10,
			upgradePriceMultiplier = 2f,
			supplierMarkupMultiplier = 3f,
			inventorMarkup = 100,
			retiredSupplierMarkup = 45,
			merchantDiscount = 25,
			scalperMarkupBase = 25,
			scalperMarkupPerDay = 3f,
			propertyBasePrice = 36000,
			propertyPriceIncreasePerDay = 100,
			propertyPriceIncreaseCap = 20000,
			mortgageInterestRate = 100,
			mortgageWeeks = 16,
			mortgageDownPaymentShare = 0.2f,
			reputationGainMultiplier = 0.5f,
			reputationLossMultiplier = 2f,
			appraisalProgressGainMultiplier = 0.75f,
			appraisalProgressLossMultiplier = 2f,
			tradeFavorRate = 0f,
			wildServiceFavorRefund = 0.15f,
			networkIntroFavor = 0,
			augKillPresenceDrop = 8,
			augLeftAlivePresenceRise = 16,
			augWarning = false,
			augGameOverAtFullPresence = true,
			louDemandValue = 1500,
			wildBailout = false,
			wildReminders = false,
			displayCaseTip = false,
			eventEndCountdown = false,
			impostorTell = false
		};
		vanilla = new List<Difficulty>
		{
			CreateVanilla("cozy", 100, "difficulty_cozy_title", "Price Gouge", "difficulty_cozy_subtitle", "Cozy Mode", difficultyRules),
			CreateVanilla("normal", 200, "difficulty_normal_title", "Mark Up", "difficulty_normal_subtitle", "Normal Mode", difficultyRules2),
			CreateVanilla("hard", 300, "difficulty_hard_title", "Thin Margins", "difficulty_hard_subtitle", "Hard Mode", difficultyRules3),
			CreateVanilla("challenge", 400, "difficulty_challenge_title", "Red Ink", "difficulty_challenge_subtitle", "Challenge Mode", difficultyRules4)
		};
		return vanilla;
	}

	private static Difficulty CreateVanilla(string id, int order, string titleKey, string titleEnglish, string subtitleKey, string subtitleEnglish, DifficultyRules rules)
	{
		return new Difficulty
		{
			id = id,
			order = order,
			titleProvider = () => LocHelper.GetLocalizedMainMenu(titleKey),
			subtitleProvider = () => LocHelper.GetLocalizedMainMenu(subtitleKey),
			rules = rules
		};
	}

	public static List<string> GetEffectLines(Difficulty difficulty)
	{
		List<string> list = new List<string>();
		if (difficulty == null)
		{
			return list;
		}
		DifficultyRules arg = Normal.rules;
		bool flag = difficulty.id == "normal";
		foreach (DifficultyEffect effect in Effects)
		{
			try
			{
				if (flag || effect.differs(difficulty.rules, arg))
				{
					string text = effect.describe(difficulty.rules);
					if (!string.IsNullOrEmpty(text))
					{
						list.Add(text);
					}
				}
			}
			catch (Exception arg2)
			{
				Debug.LogError($"[Difficulty] Effect '{effect.key}' threw: {arg2}");
			}
		}
		foreach (Func<string> note in difficulty.notes)
		{
			string text2 = difficulty.Resolve(note);
			if (!string.IsNullOrEmpty(text2))
			{
				list.Add(text2);
			}
		}
		return list;
	}

	private static string Text(string key, string english, params object[] args)
	{
		return LocHelper.GetLocalizedMainMenu(key, args);
	}

	private static int Percent(float rate)
	{
		return Mathf.RoundToInt(rate * 100f);
	}

	private static decimal Multiplier(float value)
	{
		return Math.Round((decimal)value, 2);
	}

	private static List<DifficultyEffect> CreateEffects()
	{
		return new List<DifficultyEffect>
		{
			new DifficultyEffect
			{
				key = "retail_markup",
				differs = (DifficultyRules r, DifficultyRules n) => r.retailMarkup != n.retailMarkup,
				describe = (DifficultyRules r) => (r.retailMarkup > 0) ? Text("difficulty_effect_markup", "Retail markup of {0}% when selling", r.retailMarkup) : Text("difficulty_effect_no_markup", "No retail markup when selling")
			},
			new DifficultyEffect
			{
				key = "rent_increase",
				differs = (DifficultyRules r, DifficultyRules n) => !Mathf.Approximately(r.rentIncreaseMultiplier, n.rentIncreaseMultiplier),
				describe = (DifficultyRules r) => Text("difficulty_effect_rent_increase", "Rent increases by {0}% each time", Percent(r.rentIncreaseMultiplier - 1f))
			},
			new DifficultyEffect
			{
				key = "service_fees",
				differs = (DifficultyRules r, DifficultyRules n) => r.powerCostMultiplier != n.powerCostMultiplier || r.insuranceCostMultiplier != n.insuranceCostMultiplier || r.startPowerCostBonus != n.startPowerCostBonus || r.startJanitorialCostBonus != n.startJanitorialCostBonus || r.startInsuranceCostBonus != n.startInsuranceCostBonus,
				describe = delegate(DifficultyRules r)
				{
					DifficultyRules difficultyRules4 = Normal.rules;
					int num5 = r.powerCostMultiplier + r.insuranceCostMultiplier + r.startPowerCostBonus + r.startJanitorialCostBonus + r.startInsuranceCostBonus;
					int num6 = difficultyRules4.powerCostMultiplier + difficultyRules4.insuranceCostMultiplier + difficultyRules4.startPowerCostBonus + difficultyRules4.startJanitorialCostBonus + difficultyRules4.startInsuranceCostBonus;
					return (num5 != num6) ? ((num5 <= num6) ? Text("difficulty_effect_lower_service_fees", "Lower nightly service fees") : Text("difficulty_effect_higher_service_fees", "Higher nightly service fees")) : Text("difficulty_effect_standard_service_fees", "Standard nightly service fees");
				}
			},
			new DifficultyEffect
			{
				key = "vendor_permit",
				differs = (DifficultyRules r, DifficultyRules n) => r.vendorPermitFee != n.vendorPermitFee,
				describe = (DifficultyRules r) => Text("difficulty_effect_vendor_permit", "Vendor permit costs {0}", r.vendorPermitFee)
			},
			new DifficultyEffect
			{
				key = "upgrade_prices",
				differs = (DifficultyRules r, DifficultyRules n) => !Mathf.Approximately(r.upgradePriceMultiplier, n.upgradePriceMultiplier),
				describe = (DifficultyRules r) => (!Mathf.Approximately(r.upgradePriceMultiplier, 1f)) ? ((!(r.upgradePriceMultiplier > 1f)) ? Text("difficulty_effect_upgrades_cheaper", "Upgrades cost {0}% less", Percent(1f - r.upgradePriceMultiplier)) : Text("difficulty_effect_upgrades_pricier", "Upgrades cost {0}% more", Percent(r.upgradePriceMultiplier - 1f))) : Text("difficulty_effect_standard_upgrade_prices", "Standard upgrade prices")
			},
			new DifficultyEffect
			{
				key = "supplier_markup",
				differs = (DifficultyRules r, DifficultyRules n) => !Mathf.Approximately(r.supplierMarkupMultiplier, n.supplierMarkupMultiplier) || r.retiredSupplierMarkup != n.retiredSupplierMarkup,
				describe = delegate(DifficultyRules r)
				{
					DifficultyRules difficultyRules3 = Normal.rules;
					int num4 = r.supplierMarkupMultiplier.CompareTo(difficultyRules3.supplierMarkupMultiplier) + r.retiredSupplierMarkup.CompareTo(difficultyRules3.retiredSupplierMarkup);
					return (num4 != 0) ? ((num4 <= 0) ? Text("difficulty_effect_supplier_cheaper", "Suppliers charge a lower markup") : Text("difficulty_effect_supplier_pricier", "Suppliers charge a higher markup")) : Text("difficulty_effect_standard_supplier_markup", "Standard supplier markup");
				}
			},
			new DifficultyEffect
			{
				key = "property",
				differs = (DifficultyRules r, DifficultyRules n) => r.propertyBasePrice != n.propertyBasePrice || r.propertyPriceIncreasePerDay != n.propertyPriceIncreasePerDay || r.propertyPriceIncreaseCap != n.propertyPriceIncreaseCap || r.mortgageInterestRate != n.mortgageInterestRate || r.mortgageWeeks != n.mortgageWeeks || !Mathf.Approximately(r.mortgageDownPaymentShare, n.mortgageDownPaymentShare),
				describe = delegate(DifficultyRules r)
				{
					DifficultyRules difficultyRules2 = Normal.rules;
					int num3 = ((r.propertyBasePrice != difficultyRules2.propertyBasePrice) ? r.propertyBasePrice.CompareTo(difficultyRules2.propertyBasePrice) : ((r.mortgageInterestRate != difficultyRules2.mortgageInterestRate) ? r.mortgageInterestRate.CompareTo(difficultyRules2.mortgageInterestRate) : (r.propertyPriceIncreasePerDay * r.propertyPriceIncreaseCap).CompareTo(difficultyRules2.propertyPriceIncreasePerDay * difficultyRules2.propertyPriceIncreaseCap)));
					return (num3 != 0) ? ((num3 <= 0) ? Text("difficulty_effect_lower_property_price", "Lower property price") : Text("difficulty_effect_higher_property_price", "Higher property price")) : Text("difficulty_effect_standard_property_price", "Standard property price");
				}
			},
			new DifficultyEffect
			{
				key = "reputation_gain",
				differs = (DifficultyRules r, DifficultyRules n) => !Mathf.Approximately(r.reputationGainMultiplier, n.reputationGainMultiplier),
				describe = (DifficultyRules r) => Text("difficulty_effect_reputation_gain", "Reputation gains x{0}", Multiplier(r.reputationGainMultiplier))
			},
			new DifficultyEffect
			{
				key = "reputation_loss",
				differs = (DifficultyRules r, DifficultyRules n) => !Mathf.Approximately(r.reputationLossMultiplier, n.reputationLossMultiplier),
				describe = (DifficultyRules r) => Text("difficulty_effect_reputation_loss", "Reputation losses x{0}", Multiplier(r.reputationLossMultiplier))
			},
			new DifficultyEffect
			{
				key = "wild_favor",
				differs = (DifficultyRules r, DifficultyRules n) => !Mathf.Approximately(r.tradeFavorRate, n.tradeFavorRate) || !Mathf.Approximately(r.wildServiceFavorRefund, n.wildServiceFavorRefund),
				describe = delegate(DifficultyRules r)
				{
					DifficultyRules difficultyRules = Normal.rules;
					float num = r.tradeFavorRate + r.wildServiceFavorRefund;
					float num2 = difficultyRules.tradeFavorRate + difficultyRules.wildServiceFavorRefund;
					return (!Mathf.Approximately(num, num2)) ? ((!(num > num2)) ? Text("difficulty_effect_less_wild_favor", "Reduced Wild's favor gains") : Text("difficulty_effect_more_wild_favor", "Increased Wild's favor gains")) : Text("difficulty_effect_standard_wild_favor", "Standard Wild's favor gains");
				}
			}
		};
	}
}

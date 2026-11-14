using System;
using System.Collections.Generic;

public class RibwichHelper
{
	public const int FIRST_PRODUCTION_DATE_YEAR_MIN = 425;

	public const int FIRST_PRODUCTION_DATE_YEAR_MAX = 429;

	public const int SECOND_PRODUCTION_DATE_YEAR_MIN = 432;

	public const int SECOND_PRODUCTION_DATE_YEAR_MAX = 438;

	public static readonly int[] HIGH_QUALITY_FACILITY_NUMBERS = new int[6] { 1, 2, 5, 8, 21, 34 };

	public static readonly int[] LOW_QUALITY_FACILITY_NUMBERS = new int[5] { 7, 15, 18, 26, 31 };

	private static int[] _normalQualityNumbers;

	public const int TOTAL_FACILITY_NUMBERS = 35;

	public const string RIBWICH_TAG = "RIBWICH_TAG";

	public const string RIBWICH_WRAP_TAG = "RIBWICH_WRAP_TAG";

	public const int RIBWICH_WRAP_NONE = -1;

	public const int RIBWICH_WRAP_CORRECT = 0;

	public const int RIBWICH_WRAP_OPEN_LEFT = 1;

	public const int RIBWICH_WRAP_OPEN_RIGHT = 2;

	public const string RIBWICH_PICKLES_TAG = "RIBWICH_PICKLES_TAG";

	public const int RIBWICH_PICKLES_NONE = -1;

	public const int RIBWICH_PICKLES_NORMAL = 0;

	public const int RIBWICH_PICKLES_EXTRA = 1;

	public const string RIBWICH_MEAT_TAG = "RIBWICH_MEAT_TAG";

	public const int RIBWICH_MEAT_NORMAL = 0;

	public const int RIBWICH_MEAT_DOUBLE = 1;

	public const string RIBWICH_BUN_TAG = "RIBWICH_BUN_TAG";

	public const int RIBWICH_BUN_NORMAL = 0;

	public const int RIBWICH_BUN_DELUXE = 1;

	public const string RIBWICH_DAVRE_LOGO_TAG = "RIBWICH_DAVRE_LOGO_TAG";

	public const int RIBWICH_DAVRE_LOGO_CORRECT = 0;

	public const string RIBWICH_RIBWICH_LOGO_TAG = "RIBWICH_RIBWICH_LOGO_TAG";

	public const int RIBWICH_RIBWICH_LOGO_CORRECT = 0;

	public const string RIBWICH_PRODUCTION_MONTH_TAG = "RIBWICH_PRODUCTION_MONTH_TAG";

	public const string RIBWICH_PRODUCTION_YEAR_TAG = "RIBWICH_PRODUCTION_YEAR_TAG";

	public const string RIBWICH_FACILITY_NUMBER_TAG = "RIBWICH_FACILITY_NUMBER_TAG";

	public const string RIBWICH_PRODUCTION_TAG = "RIBWICH_PRODUCTION_TAG";

	public const string RIBWICH_FACILITY_TAG = "RIBWICH_FACILITY_TAG";

	public const int BASE_PRICE = 150;

	public const int OPENED_PRICE = 40;

	public const int MIN_VALUE = 40;

	public const int COUNTERFEIT_PRICE = 40;

	public const int FIRST_MONTH_BONUS = 50;

	public const int FIRST_PRODUCTION_RUN_BONUS = 80;

	public const int DOUBLE_MEAT_BONUS = 800;

	public const int DELUXE_BONUS = 300;

	public const int HIGH_QUALITY_FACILITY_BONUS = 50;

	public const int LOW_QUALITY_FACILITY_PENALTY = -50;

	public const int INCORRECT_DAVRE_LOGO_COUNT = 7;

	public const int INCORRECT_RIBWICH_LOGO_COUNT = 3;

	public static void Init(GameItem gameItem)
	{
		gameItem.EnableTag("INSPECTABLE_TAG");
		gameItem.unitValue = 150L;
		AppraisalHelper.InitAppraisalItem(gameItem, 150);
		gameItem.EnableTag("RIBWICH_TAG");
		InitCorrectNormal(gameItem);
	}

	public static void InitCorrectNormal(GameItem gameItem)
	{
		(int, int) productionDate = GenerateCorrectProductionDate();
		gameItem.ModifyTag("RIBWICH_PRODUCTION_MONTH_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(productionDate.Item1);
		});
		gameItem.ModifyTag("RIBWICH_PRODUCTION_YEAR_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(productionDate.Item2);
		});
		gameItem.ModifyTag("RIBWICH_FACILITY_NUMBER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(GenerateRandomPackingFacilityNumber());
		});
		gameItem.ModifyTag("RIBWICH_MEAT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_PICKLES_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_WRAP_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_BUN_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_DAVRE_LOGO_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_PRODUCTION_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_FACILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (GetProductionPeriod(productionDate) == 2)
		{
			gameItem.ModifyTag("RIBWICH_RIBWICH_LOGO_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
		else
		{
			gameItem.ModifyTag("RIBWICH_RIBWICH_LOGO_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(-1);
			});
		}
	}

	public static void InitCorrectDoubleMeat(GameItem gameItem)
	{
		(int, int) productionDate = GenerateCorrectProductionDate();
		gameItem.ModifyTag("RIBWICH_PRODUCTION_MONTH_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(productionDate.Item1);
		});
		gameItem.ModifyTag("RIBWICH_PRODUCTION_YEAR_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(productionDate.Item2);
		});
		gameItem.ModifyTag("RIBWICH_FACILITY_NUMBER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(GenerateRandomPackingFacilityNumber());
		});
		gameItem.ModifyTag("RIBWICH_MEAT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		gameItem.ModifyTag("RIBWICH_PICKLES_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_WRAP_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_BUN_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_DAVRE_LOGO_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_PRODUCTION_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_FACILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (GetProductionPeriod(productionDate) == 2)
		{
			gameItem.ModifyTag("RIBWICH_RIBWICH_LOGO_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
		else
		{
			gameItem.ModifyTag("RIBWICH_RIBWICH_LOGO_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(-1);
			});
		}
	}

	public static void InitCorrectDeluxeEdition(GameItem gameItem)
	{
		(int, int) productionDate = GenerateCorrectProductionDate();
		gameItem.ModifyTag("RIBWICH_PRODUCTION_MONTH_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(productionDate.Item1);
		});
		gameItem.ModifyTag("RIBWICH_PRODUCTION_YEAR_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(productionDate.Item2);
		});
		gameItem.ModifyTag("RIBWICH_FACILITY_NUMBER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(GenerateRandomPackingFacilityNumber());
		});
		gameItem.ModifyTag("RIBWICH_MEAT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_PICKLES_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		gameItem.ModifyTag("RIBWICH_WRAP_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_BUN_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		gameItem.ModifyTag("RIBWICH_DAVRE_LOGO_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_PRODUCTION_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_FACILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (GetProductionPeriod(productionDate) == 2)
		{
			gameItem.ModifyTag("RIBWICH_RIBWICH_LOGO_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
		else
		{
			gameItem.ModifyTag("RIBWICH_RIBWICH_LOGO_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(-1);
			});
		}
	}

	public static void InitOpened(GameItem gameItem)
	{
		(int, int) productionDate = GenerateCorrectProductionDate();
		gameItem.ModifyTag("RIBWICH_PRODUCTION_MONTH_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(productionDate.Item1);
		});
		gameItem.ModifyTag("RIBWICH_PRODUCTION_YEAR_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(productionDate.Item2);
		});
		gameItem.ModifyTag("RIBWICH_FACILITY_NUMBER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(GenerateRandomPackingFacilityNumber());
		});
		gameItem.ModifyTag("RIBWICH_MEAT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_PICKLES_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		gameItem.ModifyTag("RIBWICH_WRAP_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(RNG.Roll(50) ? 1 : 2);
		});
		gameItem.ModifyTag("RIBWICH_BUN_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		gameItem.ModifyTag("RIBWICH_DAVRE_LOGO_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_PRODUCTION_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("RIBWICH_FACILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (GetProductionPeriod(productionDate) == 2)
		{
			gameItem.ModifyTag("RIBWICH_RIBWICH_LOGO_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
		else
		{
			gameItem.ModifyTag("RIBWICH_RIBWICH_LOGO_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(-1);
			});
		}
	}

	public static void InitCounterfeit(GameItem gameItem)
	{
		if (RNG.Roll(20))
		{
			InitCorrectDeluxeEdition(gameItem);
			SetRandomCounterfeitMistake(gameItem, isDeluxe: true);
		}
		else
		{
			InitCorrectNormal(gameItem);
			SetRandomCounterfeitMistake(gameItem, isDeluxe: false);
		}
	}

	public static void SetRandomCounterfeitMistake(GameItem gameItem, bool isDeluxe)
	{
		(int, int) incorrectDate = GenerateIncorrectProductionDate();
		if (isDeluxe && RNG.Roll(25))
		{
			if (RNG.Roll(50))
			{
				gameItem.ModifyTag("RIBWICH_BUN_TAG", delegate(TagState state)
				{
					state.Enable().SetInt(0);
				});
			}
			else
			{
				gameItem.ModifyTag("RIBWICH_PICKLES_TAG", delegate(TagState state)
				{
					state.Enable().SetInt(0);
				});
			}
		}
		else if (GetProductionPeriod(incorrectDate) == 2 && RNG.Roll(20))
		{
			int selected2 = RNG.GetRandomInt(0, 3);
			if (selected2 == 0)
			{
				selected2 = 1;
			}
			gameItem.ModifyTag("RIBWICH_RIBWICH_LOGO_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(selected2);
			});
		}
		else if (GetProductionPeriod(incorrectDate) == 1 && RNG.Roll(20))
		{
			gameItem.ModifyTag("RIBWICH_RIBWICH_LOGO_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
		else if (RNG.Roll(20))
		{
			int selected = RNG.GetRandomInt(0, 7);
			if (selected == 0)
			{
				selected = 1;
			}
			gameItem.ModifyTag("RIBWICH_DAVRE_LOGO_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(selected);
			});
		}
		else if (RNG.Roll(5))
		{
			gameItem.ModifyTag("RIBWICH_PRODUCTION_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(-1);
			});
		}
		else if (RNG.Roll(5))
		{
			gameItem.ModifyTag("RIBWICH_FACILITY_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(-1);
			});
		}
		else if (RNG.Roll(20))
		{
			gameItem.ModifyTag("RIBWICH_PRODUCTION_MONTH_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(incorrectDate.Item1);
			});
			gameItem.ModifyTag("RIBWICH_PRODUCTION_YEAR_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(incorrectDate.Item2);
			});
		}
		else
		{
			gameItem.ModifyTag("RIBWICH_FACILITY_NUMBER_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(GenerateIncorrectFacilityNumber());
			});
		}
	}

	public static int GetCorrectValue(GameItem gameItem)
	{
		if (!gameItem.IsTag("RIBWICH_TAG"))
		{
			return 0;
		}
		int num = 150;
		if (IsOpened(gameItem))
		{
			return 40;
		}
		if (IsCounterfeit(gameItem))
		{
			return 40;
		}
		if (gameItem.GetTagReadonly("RIBWICH_MEAT_TAG").GetInt() == 1)
		{
			num += 800;
		}
		if (IsDeluxeEdition(gameItem))
		{
			num += 300;
		}
		if (IsFirstOrLastProductionMonth(gameItem.GetTagReadonly("RIBWICH_PRODUCTION_MONTH_TAG").GetInt(), gameItem.GetTagReadonly("RIBWICH_PRODUCTION_YEAR_TAG").GetInt()))
		{
			num += 50;
		}
		if (IsFirstProduction(gameItem.GetTagReadonly("RIBWICH_PRODUCTION_YEAR_TAG").GetInt()))
		{
			num += 80;
		}
		int @int = gameItem.GetTagReadonly("RIBWICH_FACILITY_NUMBER_TAG").GetInt();
		num += GetFacilityBonus(@int);
		if (num < 40)
		{
			num = 40;
		}
		return num;
	}

	public static bool IsOpened(GameItem gameItem)
	{
		if (!gameItem.IsTag("RIBWICH_TAG"))
		{
			return false;
		}
		int @int = gameItem.GetTagReadonly("RIBWICH_WRAP_TAG").GetInt();
		if (@int != 1)
		{
			return @int == 2;
		}
		return true;
	}

	public static bool IsCounterfeit(GameItem gameItem)
	{
		if (!gameItem.IsTag("RIBWICH_TAG"))
		{
			return false;
		}
		if (!IsValidProductionDate(gameItem.GetTagReadonly("RIBWICH_PRODUCTION_MONTH_TAG").GetInt(), gameItem.GetTagReadonly("RIBWICH_PRODUCTION_YEAR_TAG").GetInt()))
		{
			return true;
		}
		if (!IsValidFacilityNumber(gameItem.GetTagReadonly("RIBWICH_FACILITY_NUMBER_TAG").GetInt()))
		{
			return true;
		}
		if (gameItem.GetTagReadonly("RIBWICH_DAVRE_LOGO_TAG").GetInt() != 0)
		{
			return true;
		}
		switch (GetProductionPeriod((gameItem.GetTagReadonly("RIBWICH_PRODUCTION_MONTH_TAG").GetInt(), gameItem.GetTagReadonly("RIBWICH_PRODUCTION_YEAR_TAG").GetInt())))
		{
		case 2:
			if (gameItem.GetTagReadonly("RIBWICH_RIBWICH_LOGO_TAG").GetInt() != 0)
			{
				return true;
			}
			break;
		case 1:
			if (gameItem.GetTagReadonly("RIBWICH_RIBWICH_LOGO_TAG").GetInt() != -1)
			{
				return true;
			}
			break;
		}
		if (gameItem.GetTagReadonly("RIBWICH_PRODUCTION_TAG").GetInt() == -1)
		{
			return true;
		}
		if (gameItem.GetTagReadonly("RIBWICH_FACILITY_TAG").GetInt() == -1)
		{
			return true;
		}
		if (!IsValidDeluxeEdition(gameItem))
		{
			return true;
		}
		return false;
	}

	public static bool IsValidProductionDate(int month, int year)
	{
		if (month < 1 || month > 12)
		{
			return false;
		}
		if ((year >= 425 && year <= 429) || (year >= 432 && year <= 438))
		{
			return true;
		}
		return false;
	}

	public static (int, int) GenerateCorrectProductionDate()
	{
		if (RNG.Roll(50))
		{
			return GenerateFirstProductionDate();
		}
		return GenerateSecondProductionDate();
	}

	public static (int, int) GenerateFirstProductionDate()
	{
		int randomInt = RNG.GetRandomInt(1, 12);
		int randomInt2 = RNG.GetRandomInt(425, 429);
		return (randomInt, randomInt2);
	}

	public static (int, int) GenerateSecondProductionDate()
	{
		int randomInt = RNG.GetRandomInt(1, 12);
		int randomInt2 = RNG.GetRandomInt(432, 438);
		return (randomInt, randomInt2);
	}

	public static (int, int) GenerateIncorrectProductionDate()
	{
		int randomInt = RNG.GetRandomInt(1, 12);
		int num = 0;
		if (RNG.Roll(20))
		{
			randomInt = RNG.GetRandomInt(13, 15);
			num = ((!RNG.Roll(50)) ? RNG.GetRandomInt(432, 438) : RNG.GetRandomInt(425, 429));
		}
		else
		{
			int randomInt2 = RNG.GetRandomInt(1, 100);
			num = ((randomInt2 <= 33) ? RNG.GetRandomInt(420, 424) : ((randomInt2 > 66) ? RNG.GetRandomInt(439, 443) : RNG.GetRandomInt(430, 431)));
		}
		return (randomInt, num);
	}

	public static int GenerateHighQualityPackingFacilityNumber()
	{
		return RNG.GetRandomElement(HIGH_QUALITY_FACILITY_NUMBERS);
	}

	public static int GenerateNormalQualityPackingFacilityNumber()
	{
		if (_normalQualityNumbers == null)
		{
			HashSet<int> hashSet = new HashSet<int>(HIGH_QUALITY_FACILITY_NUMBERS);
			hashSet.UnionWith(LOW_QUALITY_FACILITY_NUMBERS);
			List<int> list = new List<int>();
			for (int i = 0; i < 35; i++)
			{
				if (!hashSet.Contains(i))
				{
					list.Add(i);
				}
			}
			_normalQualityNumbers = list.ToArray();
		}
		return RNG.GetRandomElement(_normalQualityNumbers);
	}

	public static int GenerateLowQualityPackingFacilityNumber()
	{
		return RNG.GetRandomElement(LOW_QUALITY_FACILITY_NUMBERS);
	}

	public static int GenerateRandomPackingFacilityNumber()
	{
		return RNG.GetRandomInt(1, 35);
	}

	public static int GenerateIncorrectFacilityNumber()
	{
		return RNG.GetRandomInt(36, 45);
	}

	public static bool IsValidFacilityNumber(int facilityNumber)
	{
		if (facilityNumber > 0)
		{
			return facilityNumber <= 35;
		}
		return false;
	}

	public static int GetFacilityBonus(int facilityNumber)
	{
		if (Array.Exists(HIGH_QUALITY_FACILITY_NUMBERS, (int element) => element == facilityNumber))
		{
			return 50;
		}
		if (Array.Exists(LOW_QUALITY_FACILITY_NUMBERS, (int element) => element == facilityNumber))
		{
			return -50;
		}
		return 0;
	}

	public static int GetProductionPeriod((int, int) productionDate)
	{
		var (num, num2) = productionDate;
		if (num < 1 || num > 12)
		{
			return 0;
		}
		if (num2 >= 425 && num2 <= 429)
		{
			return 1;
		}
		if (num2 >= 432 && num2 <= 438)
		{
			return 2;
		}
		return 0;
	}

	public static bool IsFirstProduction(int year)
	{
		if (year >= 425)
		{
			return year <= 429;
		}
		return false;
	}

	public static bool IsFirstOrLastProductionMonth(int month, int year)
	{
		int productionPeriod = GetProductionPeriod((month, year));
		if (productionPeriod == 1 && month == 1 && year == 425)
		{
			return true;
		}
		if (productionPeriod == 2 && month == 12 && year == 438)
		{
			return true;
		}
		return false;
	}

	public static bool IsDeluxeEdition(GameItem gameItem)
	{
		if (!gameItem.IsTag("RIBWICH_TAG"))
		{
			return false;
		}
		int @int = gameItem.GetTagReadonly("RIBWICH_BUN_TAG").GetInt();
		int int2 = gameItem.GetTagReadonly("RIBWICH_PICKLES_TAG").GetInt();
		if (@int == 1)
		{
			return int2 == 1;
		}
		return false;
	}

	public static bool IsValidDeluxeEdition(GameItem gameItem)
	{
		if (!gameItem.IsTag("RIBWICH_TAG"))
		{
			return false;
		}
		int @int = gameItem.GetTagReadonly("RIBWICH_BUN_TAG").GetInt();
		int int2 = gameItem.GetTagReadonly("RIBWICH_PICKLES_TAG").GetInt();
		if (@int != 1 || int2 != 1)
		{
			if (@int == 0)
			{
				return int2 == 0;
			}
			return false;
		}
		return true;
	}

	public static void CreateDebugTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("RIBWICH_BUN_TAG") && PlayerStore.Instance.devMode)
		{
			builder.AddLine("Correct Value " + GetCorrectValue(item), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			if (IsCounterfeit(item))
			{
				builder.AddLine("Counterfeit", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (IsOpened(item))
			{
				builder.AddLine("Opened", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (item.GetTagReadonly("RIBWICH_MEAT_TAG").GetInt() == 1)
			{
				builder.AddLine("Double Meat", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (IsDeluxeEdition(item))
			{
				builder.AddLine("Deluxe Edition", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (!IsValidFacilityNumber(item.GetTagReadonly("RIBWICH_FACILITY_NUMBER_TAG").GetInt()))
			{
				builder.AddLine("-Invalid Facility Number", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
			if (!IsValidProductionDate(item.GetTagReadonly("RIBWICH_PRODUCTION_MONTH_TAG").GetInt(), item.GetTagReadonly("RIBWICH_PRODUCTION_YEAR_TAG").GetInt()))
			{
				builder.AddLine("-Invalid Production Date", ignoreEmpty: true, RenderHandler.ColorPalette.Red);
			}
		}
	}
}

public class AppNavHelper
{
	public const string NAVIGATION_PROCESSING_UNIT_TAG = "NAVIGATION_PROCESSING_UNIT_TAG";

	public const string NPU_CASING_TAG = "NPU_CASING_TAG";

	public const string NPU_CPU_TAG = "NPU_CPU_TAG";

	public const string NPU_BRAND_TAG = "NPU_BRAND_TAG";

	public const string NPU_SPU1_TAG = "NPU_SPU1_TAG";

	public const string NPU_SPU2_TAG = "NPU_SPU2_TAG";

	public const string NPU_SERIAL_TAG = "NPU_SERIAL_TAG";

	public const string NPU_LETTERING_TAG = "NPU_LETTERING_TAG";

	public const string NPU_LOGO_TAG = "NPU_LOGO_TAG";

	public const string NPU_OPERATIONAL_TAG = "NPU_OPERATIONAL_TAG";

	private static readonly (string, int)[] NavyOptions = new(string, int)[10]
	{
		("Republic Navi", 25),
		("Repulic Navy", 25),
		("Big Navy", 25),
		("epublic Navy", 25),
		("Repubic Navy", 20),
		("Replubic Navy", 15),
		("Republic Nvy", 15),
		("Republik Navy", 10),
		("Navi of Republic", 10),
		("Republic", 30)
	};

	public static void InitItem(GameItem gameItem)
	{
		gameItem.EnableTag("INSPECTABLE_TAG");
		AppraisalHelper.InitAppraisalItem(gameItem, 1000);
		gameItem.EnableTag("NAVIGATION_PROCESSING_UNIT_TAG");
		InitDefault(gameItem);
	}

	public static void InitDefault(GameItem gameItem)
	{
	}

	public static void InitRandom(GameItem gameItem)
	{
		if (RNG.Roll(20))
		{
			InitNavyNPU(gameItem);
		}
		else if (RNG.Roll(20))
		{
			InitCorporateNPU(gameItem);
		}
		else if (RNG.Roll(15))
		{
			InitFakeNavyNPU(gameItem);
		}
		else if (RNG.Roll(15))
		{
			InitFakeCorporateNPU(gameItem);
		}
		else
		{
			InitGenericNPU(gameItem);
		}
	}

	public static string GenerateValidNavyLettering()
	{
		return "Republic Navy";
	}

	public static string GenerateInvalidNavyLettering()
	{
		return RNG.GetOdd(NavyOptions);
	}

	public static string GenerateValidNavySerial()
	{
		return "M941 Serial " + RNG.GetRandomInt(100, 999) + "-" + RNG.GetRandomInt(100, 999);
	}

	public static string GenerateInvalidNavySerial()
	{
		switch (RNG.GetRandomInt(1, 5))
		{
		case 1:
		{
			int randomInt;
			for (randomInt = RNG.GetRandomInt(100, 999); randomInt == 941; randomInt = RNG.GetRandomInt(100, 999))
			{
			}
			return $"M{randomInt} Serial {RNG.GetRandomInt(100, 999)}-{RNG.GetRandomInt(100, 999)}";
		}
		case 2:
			return $"M941 Serial {RNG.GetRandomInt(1000, 9999)}-{RNG.GetRandomInt(1000, 9999)}";
		case 3:
			return $"M941 Serial {RNG.GetRandomInt(1, 9)}-{RNG.GetRandomInt(1, 9)}";
		case 4:
			return "M941 Serial";
		case 5:
			return $"{RNG.GetRandomInt(100, 999)}-{RNG.GetRandomInt(100, 999)}";
		default:
			return "M941 Serial";
		}
	}

	public static string GenerateValidCorporateSerial()
	{
		return "NPU-" + RNG.GetRandomInt(1000, 9999) + "-" + RNG.GetRandomInt(1000, 9999);
	}

	public static string GenerateInvalidCorporateSerial()
	{
		int randomInt = RNG.GetRandomInt(1, 5);
		string[] array = new string[4] { "NUP", "CPU", "NPUU", "N-PU" };
		return randomInt switch
		{
			1 => $"{array[RNG.GetRandomInt(0, array.Length - 1)]}-{RNG.GetRandomInt(1000, 9999)}-{RNG.GetRandomInt(1000, 9999)}", 
			2 => $"NPU-{RNG.GetRandomInt(10000, 99999)}-{RNG.GetRandomInt(10000, 99999)}", 
			3 => $"NPU-{RNG.GetRandomInt(0, 99)}-{RNG.GetRandomInt(0, 99)}", 
			4 => "NPU-", 
			5 => $"{RNG.GetRandomInt(1000, 9999)}-{RNG.GetRandomInt(1000, 9999)}", 
			_ => "NPU-", 
		};
	}

	public static void InitGenericNPU(GameItem gameItem)
	{
		gameItem.ModifyTag("NPU_BRAND_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("NPU_CPU_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("NPU_SPU1_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (RNG.Roll(50))
		{
			gameItem.ModifyTag("NPU_SPU1_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(RNG.GetRandomInt(1, 3));
			});
		}
		gameItem.ModifyTag("NPU_SPU2_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (RNG.Roll(25))
		{
			gameItem.ModifyTag("NPU_SPU2_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(RNG.GetRandomInt(1, 3));
			});
		}
		gameItem.ModifyTag("NPU_CASING_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(RNG.GetRandomInt(0, 3));
		});
		gameItem.ModifyTag("NPU_OPERATIONAL_TAG", delegate(TagState state)
		{
			state.Enable().SetBool(IsOperational(gameItem));
		});
	}

	public static void InitNavyNPU(GameItem gameItem)
	{
		gameItem.ModifyTag("NPU_BRAND_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		gameItem.ModifyTag("NPU_CPU_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
		gameItem.ModifyTag("NPU_SPU1_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (RNG.Roll(50))
		{
			gameItem.ModifyTag("NPU_SPU1_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(2);
			});
		}
		gameItem.ModifyTag("NPU_SPU2_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (RNG.Roll(25))
		{
			gameItem.ModifyTag("NPU_SPU2_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(2);
			});
		}
		gameItem.ModifyTag("NPU_CASING_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(RNG.GetRandomInt(0, 3));
		});
		gameItem.ModifyTag("NPU_SERIAL_TAG", delegate(TagState state)
		{
			state.Enable().SetString(GenerateValidNavySerial());
		});
		gameItem.ModifyTag("NPU_LETTERING_TAG", delegate(TagState state)
		{
			state.Enable().SetString(GenerateValidNavyLettering());
		});
		gameItem.ModifyTag("NPU_OPERATIONAL_TAG", delegate(TagState state)
		{
			state.Enable().SetBool(IsOperational(gameItem));
		});
	}

	public static void InitCorporateNPU(GameItem gameItem)
	{
		gameItem.ModifyTag("NPU_BRAND_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
		gameItem.ModifyTag("NPU_CPU_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(3);
		});
		gameItem.ModifyTag("NPU_SPU1_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (RNG.Roll(75))
		{
			gameItem.ModifyTag("NPU_SPU1_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(3);
			});
		}
		gameItem.ModifyTag("NPU_SPU2_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (RNG.Roll(50))
		{
			gameItem.ModifyTag("NPU_SPU2_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(3);
			});
		}
		gameItem.ModifyTag("NPU_CASING_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(RNG.GetRandomInt(0, 3));
		});
		gameItem.ModifyTag("NPU_SERIAL_TAG", delegate(TagState state)
		{
			state.Enable().SetString(GenerateValidCorporateSerial());
		});
		gameItem.ModifyTag("NPU_OPERATIONAL_TAG", delegate(TagState state)
		{
			state.Enable().SetBool(IsOperational(gameItem));
		});
	}

	public static void InitFakeNavyNPU(GameItem gameItem)
	{
		gameItem.ModifyTag("NPU_BRAND_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("NPU_CPU_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
		gameItem.ModifyTag("NPU_SPU1_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (RNG.Roll(75))
		{
			gameItem.ModifyTag("NPU_SPU1_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(3);
			});
		}
		gameItem.ModifyTag("NPU_SPU2_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (RNG.Roll(50))
		{
			gameItem.ModifyTag("NPU_SPU2_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(3);
			});
		}
		gameItem.ModifyTag("NPU_CASING_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(RNG.GetRandomInt(0, 3));
		});
		gameItem.ModifyTag("NPU_SERIAL_TAG", delegate(TagState state)
		{
			state.Enable().SetString(GenerateInvalidNavyLettering());
		});
		gameItem.ModifyTag("NPU_LETTERING_TAG", delegate(TagState state)
		{
			state.Enable().SetString(GenerateInvalidNavySerial());
		});
		gameItem.ModifyTag("NPU_OPERATIONAL_TAG", delegate(TagState state)
		{
			state.Enable().SetBool(IsOperational(gameItem));
		});
	}

	public static void InitFakeCorporateNPU(GameItem gameItem)
	{
		gameItem.ModifyTag("NPU_BRAND_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("NPU_CPU_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(3);
		});
		gameItem.ModifyTag("NPU_SPU1_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (RNG.Roll(50))
		{
			gameItem.ModifyTag("NPU_SPU1_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(2);
			});
		}
		gameItem.ModifyTag("NPU_SPU2_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (RNG.Roll(25))
		{
			gameItem.ModifyTag("NPU_SPU2_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(2);
			});
		}
		gameItem.ModifyTag("NPU_CASING_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(RNG.GetRandomInt(0, 3));
		});
		gameItem.ModifyTag("NPU_SERIAL_TAG", delegate(TagState state)
		{
			state.Enable().SetString(GenerateInvalidCorporateSerial());
		});
		gameItem.ModifyTag("NPU_OPERATIONAL_TAG", delegate(TagState state)
		{
			state.Enable().SetBool(IsOperational(gameItem));
		});
	}

	public static bool IsOperational(GameItem gameItem)
	{
		if (!gameItem.IsTag("NAVIGATION_PROCESSING_UNIT_TAG"))
		{
			return false;
		}
		if (gameItem.GetTagReadonly("NPU_CPU_TAG").GetInt() == 0)
		{
			return false;
		}
		return true;
	}

	public static int GetCorrectValue(GameItem gameItem)
	{
		if (!gameItem.IsTag("NAVIGATION_PROCESSING_UNIT_TAG"))
		{
			return 0;
		}
		int num = 500;
		if (!gameItem.IsTag("NPU_OPERATIONAL_TAG"))
		{
			num -= 250;
		}
		switch (gameItem.GetTagReadonly("NPU_BRAND_TAG").GetInt())
		{
		case 1:
			num += 100;
			break;
		case 2:
			num += 200;
			_ = 2;
			break;
		}
		int @int = gameItem.GetTagReadonly("NPU_CPU_TAG").GetInt();
		switch (@int)
		{
		case 1:
			num += 50;
			break;
		case 2:
			num += 100;
			break;
		case 3:
			num += 200;
			break;
		}
		switch (gameItem.GetTagReadonly("NPU_SPU1_TAG").GetInt())
		{
		case 1:
			num += 25;
			if (@int == 1)
			{
				num += 25;
			}
			break;
		case 2:
			num += 50;
			if (@int == 2)
			{
				num += 50;
			}
			break;
		case 3:
			num += 100;
			if (@int == 3)
			{
				num += 100;
			}
			break;
		}
		switch (gameItem.GetTagReadonly("NPU_SPU2_TAG").GetInt())
		{
		case 1:
			num += 25;
			if (@int == 1)
			{
				num += 25;
			}
			break;
		case 2:
			num += 50;
			if (@int == 2)
			{
				num += 50;
			}
			break;
		case 3:
			num += 100;
			if (@int == 3)
			{
				num += 100;
			}
			break;
		}
		switch (gameItem.GetTagReadonly("NPU_CASING_TAG").GetInt())
		{
		case 1:
			num += 25;
			break;
		case 2:
			num += 50;
			break;
		case 3:
			num += 100;
			break;
		}
		return num;
	}
}

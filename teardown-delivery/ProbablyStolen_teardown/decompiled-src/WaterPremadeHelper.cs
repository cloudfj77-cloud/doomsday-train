using System;
using System.Collections.Generic;

public class WaterPremadeHelper
{
	public static readonly Dictionary<string, float> ghostwaterContaminantDistribution = new Dictionary<string, float>
	{
		{
			Liquid.GetCurrentPartFromId("organic_waste"),
			0.25f
		},
		{
			Liquid.GetCurrentPartFromId("microbe"),
			0.3f
		},
		{
			Liquid.GetCurrentPartFromId("physical_contaminant"),
			0.1f
		},
		{
			Liquid.GetCurrentPartFromId("chemical_contaminant"),
			0.15f
		},
		{
			Liquid.GetCurrentPartFromId("mineral"),
			0.05f
		},
		{
			Liquid.GetCurrentPartFromId("heavy_metal"),
			0.15f
		}
	};

	public static readonly Dictionary<string, float> waterContaminantDistribution = new Dictionary<string, float>
	{
		{
			Liquid.GetCurrentPartFromId("organic_waste"),
			0.2f
		},
		{
			Liquid.GetCurrentPartFromId("microbe"),
			0.15f
		},
		{
			Liquid.GetCurrentPartFromId("physical_contaminant"),
			0.1f
		},
		{
			Liquid.GetCurrentPartFromId("chemical_contaminant"),
			0.3f
		},
		{
			Liquid.GetCurrentPartFromId("mineral"),
			0.15f
		},
		{
			Liquid.GetCurrentPartFromId("heavy_metal"),
			0.1f
		}
	};

	public static readonly Dictionary<string, float> rustWaterContaminantDistribution = new Dictionary<string, float>
	{
		{
			Liquid.GetCurrentPartFromId("organic_waste"),
			0.22f
		},
		{
			Liquid.GetCurrentPartFromId("microbe"),
			0.15f
		},
		{
			Liquid.GetCurrentPartFromId("physical_contaminant"),
			0.1f
		},
		{
			Liquid.GetCurrentPartFromId("chemical_contaminant"),
			0.3f
		},
		{
			Liquid.GetCurrentPartFromId("mineral"),
			0.15f
		},
		{
			Liquid.GetCurrentPartFromId("heavy_metal"),
			0.08f
		}
	};

	public static readonly Dictionary<string, float> gutterflowContaminantDistribution = new Dictionary<string, float>
	{
		{
			Liquid.GetCurrentPartFromId("organic_waste"),
			0.299f
		},
		{
			Liquid.GetCurrentPartFromId("microbe"),
			0.15f
		},
		{
			Liquid.GetCurrentPartFromId("physical_contaminant"),
			0.1f
		},
		{
			Liquid.GetCurrentPartFromId("chemical_contaminant"),
			0.3f
		},
		{
			Liquid.GetCurrentPartFromId("mineral"),
			0.15f
		},
		{
			Liquid.GetCurrentPartFromId("heavy_metal"),
			0.001f
		}
	};

	public static readonly Dictionary<string, float> baseWaterContaminantDistribution = new Dictionary<string, float>
	{
		{
			Liquid.GetCurrentPartFromId("organic_waste"),
			0.2f
		},
		{
			Liquid.GetCurrentPartFromId("microbe"),
			0.1f
		},
		{
			Liquid.GetCurrentPartFromId("physical_contaminant"),
			0.1f
		},
		{
			Liquid.GetCurrentPartFromId("chemical_contaminant"),
			0.3f
		},
		{
			Liquid.GetCurrentPartFromId("mineral"),
			0.15f
		},
		{
			Liquid.GetCurrentPartFromId("heavy_metal"),
			0.15f
		}
	};

	public static int[] waterOdds = new int[5] { 30, 20, 10, 10, 10 };

	public static Func<string, GameItem>[] waterActions = new Func<string, GameItem>[5]
	{
		(string size) => AccurateBaseWater(size),
		(string size) => AccurateGhostWater(size),
		(string size) => AccurateRustWater(size),
		(string size) => GhostWaterAsBaseWater(size),
		(string size) => BaseWaterAsHQWater(size)
	};

	public static GameItem AccurateHighQualityWater(string size = "bottled_water")
	{
		GameItem gameItem = DirectoryMaster.Item(size);
		WaterHelper.FillWithHighQualityWater(gameItem);
		return gameItem;
	}

	public static GameItem AccurateBaseWater(string size = "bottled_water")
	{
		GameItem gameItem = DirectoryMaster.Item(size);
		WaterHelper.FillWithBaseWater(gameItem);
		return gameItem;
	}

	public static GameItem AccurateGhostWater(string size = "bottled_water")
	{
		GameItem gameItem = DirectoryMaster.Item(size);
		WaterHelper.FillWithGhostwater(gameItem);
		return gameItem;
	}

	public static GameItem AccurateRustWater(string size = "bottled_water")
	{
		GameItem gameItem = DirectoryMaster.Item(size);
		WaterHelper.FillWithRustWaterLowEnd(gameItem);
		return gameItem;
	}

	public static GameItem GhostWaterAsBaseWater(string size = "bottled_water")
	{
		int randomInt = RNG.GetRandomInt(9800, 9899);
		GameItem gameItem = DirectoryMaster.Item(size);
		WaterHelper.FillWithGhostwaterHighEnd(gameItem);
		WaterFeatureHelper.UpdateWaterFeatureFake(gameItem, randomInt);
		return gameItem;
	}

	public static GameItem GhostWaterAsPureWater(string size = "bottled_water")
	{
		int randomInt = RNG.GetRandomInt(9990, 9999);
		GameItem gameItem = DirectoryMaster.Item(size);
		WaterHelper.FillWithGhostwater(gameItem);
		WaterFeatureHelper.UpdateWaterFeatureFake(gameItem, randomInt);
		return gameItem;
	}

	public static GameItem BaseWaterAsHQWater(string size = "bottled_water")
	{
		int randomInt = RNG.GetRandomInt(9900, 9989);
		GameItem gameItem = DirectoryMaster.Item(size);
		WaterHelper.FillWithBaseWater(gameItem);
		WaterFeatureHelper.UpdateWaterFeatureFake(gameItem, randomInt);
		return gameItem;
	}

	public static GameItem RustWaterAsBaseWater(string size = "bottled_water")
	{
		int randomInt = RNG.GetRandomInt(9800, 9899);
		GameItem gameItem = DirectoryMaster.Item(size);
		WaterHelper.FillWithRustWater(gameItem);
		WaterFeatureHelper.UpdateWaterFeatureFake(gameItem, randomInt);
		return gameItem;
	}

	public static GameItem GutterflowAsBaseWater(string size = "bottled_water")
	{
		int randomInt = RNG.GetRandomInt(9800, 9899);
		GameItem gameItem = DirectoryMaster.Item(size);
		WaterHelper.FillWithGutterflow(gameItem);
		WaterFeatureHelper.UpdateWaterFeatureFake(gameItem, randomInt);
		return gameItem;
	}

	public static GameItem GetRandomWater(string size = "bottled_water")
	{
		int odd = RNG.GetOdd(waterOdds);
		return waterActions[odd](size);
	}
}

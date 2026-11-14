using System.Collections.Generic;
using UnityEngine;

public class ThingConstants : MonoBehaviour
{
	public static Dictionary<MatterType, double> thingMolarMass;

	public static Dictionary<MatterType, double> thingMolarHeatCapacity;

	public static Dictionary<MatterType, double> thingMolarVolume;

	public static void SetMolarMass(MatterType type, double molarMass)
	{
		if (!thingMolarMass.TryAdd(type, molarMass))
		{
			thingMolarMass[type] = molarMass;
		}
	}

	public static void SetMolarHeatCapacity(MatterType type, double molarHeatCapacity)
	{
		if (!thingMolarHeatCapacity.TryAdd(type, molarHeatCapacity))
		{
			thingMolarHeatCapacity[type] = molarHeatCapacity;
		}
	}

	public static void SetMolarVolume(MatterType type, double molarVolume)
	{
		if (!thingMolarVolume.TryAdd(type, molarVolume))
		{
			thingMolarVolume[type] = molarVolume;
		}
	}

	public static void SetWithMols(MatterType type, double molarMass, double molarHeatCapacity, double molarVolume = -1.0)
	{
		if (molarMass > 0.0)
		{
			SetMolarMass(type, molarMass);
		}
		if (molarHeatCapacity > 0.0)
		{
			SetMolarHeatCapacity(type, molarHeatCapacity);
		}
		if (molarVolume > 0.0)
		{
			SetMolarVolume(type, molarVolume);
		}
	}

	public static void SetWithMass(MatterType type, double molarMass, double joulesPerGramPerKelvin, double mLPerGram = -1.0)
	{
		if (!(molarMass <= 0.0))
		{
			SetMolarMass(type, molarMass);
			if (joulesPerGramPerKelvin > 0.0)
			{
				SetMolarHeatCapacity(type, molarMass * joulesPerGramPerKelvin);
			}
			if (mLPerGram > 0.0)
			{
				SetMolarVolume(type, molarMass * mLPerGram);
			}
		}
	}

	public static double GetMolarMass(MatterType type)
	{
		return thingMolarMass.GetValueOrDefault(type, 1.0);
	}

	public static double GetMolarHeatCapacity(MatterType type)
	{
		return thingMolarHeatCapacity.GetValueOrDefault(type, 1.0);
	}

	public static double GetMolarVolume(MatterType type)
	{
		return thingMolarVolume.GetValueOrDefault(type, 1.0);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		thingMolarMass = new Dictionary<MatterType, double>();
		thingMolarHeatCapacity = new Dictionary<MatterType, double>();
		thingMolarVolume = new Dictionary<MatterType, double>();
	}
}

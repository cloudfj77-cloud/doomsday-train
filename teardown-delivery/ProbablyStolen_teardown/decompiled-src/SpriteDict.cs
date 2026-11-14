using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDict : MonoBehaviour
{
	[Serializable]
	public class SpriteEntry
	{
		public string key;

		public Sprite sprite;
	}

	[SerializeField]
	private List<SpriteEntry> spriteEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> scavMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> scavFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> wantedEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> secMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> secFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> medMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> medFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> engMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> engFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> secCorpoMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> secCorpoFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> lowerMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> lowerFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> revMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> revFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> bmMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> bmFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> upperMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> upperFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> chefMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> chefFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> farmerMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> farmerFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> mercMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> mercFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> sciMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> sciFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> secOffMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> secOffFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> specialEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> augLowerMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> augLowerFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> augBmMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> augBmFemaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> augUpperMaleEntries = new List<SpriteEntry>();

	[SerializeField]
	private List<SpriteEntry> augUpperFemaleEntries = new List<SpriteEntry>();

	[HideInInspector]
	public Dictionary<string, Sprite> spriteDictionary;

	private static readonly Dictionary<string, List<string>> poolCache = new Dictionary<string, List<string>>();

	public static SpriteDict Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
		InitializeDictionary();
	}

	private void InitializeDictionary()
	{
		spriteDictionary = new Dictionary<string, Sprite>();
		poolCache.Clear();
		ProcessEntriesToDict(spriteEntries);
		ProcessEntriesToDict(scavMaleEntries);
		ProcessEntriesToDict(scavFemaleEntries);
		ProcessEntriesToDict(wantedEntries);
		ProcessEntriesToDict(secMaleEntries);
		ProcessEntriesToDict(secFemaleEntries);
		ProcessEntriesToDict(medMaleEntries);
		ProcessEntriesToDict(medFemaleEntries);
		ProcessEntriesToDict(engMaleEntries);
		ProcessEntriesToDict(engFemaleEntries);
		ProcessEntriesToDict(lowerMaleEntries);
		ProcessEntriesToDict(lowerFemaleEntries);
		ProcessEntriesToDict(revMaleEntries);
		ProcessEntriesToDict(revFemaleEntries);
		ProcessEntriesToDict(secCorpoMaleEntries);
		ProcessEntriesToDict(secCorpoFemaleEntries);
		ProcessEntriesToDict(bmMaleEntries);
		ProcessEntriesToDict(bmFemaleEntries);
		ProcessEntriesToDict(upperMaleEntries);
		ProcessEntriesToDict(upperFemaleEntries);
		ProcessEntriesToDict(chefMaleEntries);
		ProcessEntriesToDict(chefFemaleEntries);
		ProcessEntriesToDict(farmerMaleEntries);
		ProcessEntriesToDict(farmerFemaleEntries);
		ProcessEntriesToDict(mercMaleEntries);
		ProcessEntriesToDict(mercFemaleEntries);
		ProcessEntriesToDict(sciMaleEntries);
		ProcessEntriesToDict(sciFemaleEntries);
		ProcessEntriesToDict(secOffMaleEntries);
		ProcessEntriesToDict(secOffFemaleEntries);
		ProcessEntriesToDict(specialEntries);
		ProcessEntriesToDict(augLowerMaleEntries);
		ProcessEntriesToDict(augLowerFemaleEntries);
		ProcessEntriesToDict(augBmMaleEntries);
		ProcessEntriesToDict(augBmFemaleEntries);
		ProcessEntriesToDict(augUpperMaleEntries);
		ProcessEntriesToDict(augUpperFemaleEntries);
	}

	public void ProcessEntriesToDict(List<SpriteEntry> entries)
	{
		foreach (SpriteEntry entry in entries)
		{
			if (!string.IsNullOrEmpty(entry.key) && entry.sprite != null)
			{
				if (!spriteDictionary.ContainsKey(entry.key))
				{
					spriteDictionary.Add(entry.key, entry.sprite);
				}
				else
				{
					Debug.LogWarning("Duplicate key detected: " + entry.key + ". Entry skipped.");
				}
			}
			else
			{
				Debug.LogWarning("Invalid sprite entry. Ensure all keys and sprites are assigned.");
			}
		}
	}

	public static List<string> GetPool(string prefix, int cap = int.MaxValue)
	{
		string key = ((cap == int.MaxValue) ? prefix : (prefix + "#" + cap));
		if (poolCache.TryGetValue(key, out var value))
		{
			return value;
		}
		value = new List<string>();
		if (Instance == null || Instance.spriteDictionary == null)
		{
			Debug.LogWarning("SpriteDict pool " + prefix + " requested before the dictionary exists");
			value.Add(prefix + 1);
			return value;
		}
		for (int i = 1; i <= cap; i++)
		{
			if (!Instance.spriteDictionary.ContainsKey(prefix + i))
			{
				break;
			}
			value.Add(prefix + i);
		}
		if (value.Count == 0)
		{
			Debug.LogWarning("SpriteDict pool " + prefix + " has no sprite");
			value.Add(prefix + 1);
		}
		poolCache[key] = value;
		return value;
	}

	public Sprite ShowClientSprite(string key)
	{
		ClientSpriteVariety.OnClientShown(key);
		return GetSprite(key);
	}

	public Sprite GetSprite(string key)
	{
		if (spriteDictionary.TryGetValue(key, out var value))
		{
			return value;
		}
		Debug.LogWarning("Sprite Value not found for key in Dict: " + key);
		return spriteDictionary[GetRandomScavSpriteName()];
	}

	public static string GetRandomUpperSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleUpper"), GetPool("femaleUpper"));
	}

	public static string GetRandomUpperMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleUpper"));
	}

	public static string GetRandomUpperFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleUpper"));
	}

	public static string GetRandomShadySpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleBM"), GetPool("femaleBM"));
	}

	public static string GetRandomBMMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleBM"));
	}

	public static string GetRandomBMFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleBM"));
	}

	public static string GetRandomRevSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleRev"), GetPool("femaleRev"));
	}

	public static string GetRandomRevMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleRev"));
	}

	public static string GetRandomRevFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleRev"));
	}

	public static string GetRandomLowerLevelSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleLower"), GetPool("femaleLower"));
	}

	public static string GetRandomLowerMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleLower"));
	}

	public static string GetRandomLowerFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleLower"));
	}

	public static string GetRandomScavSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleScav"), GetPool("femaleScav"));
	}

	public static string GetRandomScavMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleScav"));
	}

	public static string GetRandomScavFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleScav"));
	}

	public static string GetRandomSecSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleSec"), GetPool("femaleSec"));
	}

	public static string GetRandomSecMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleSec"));
	}

	public static string GetRandomSecFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleSec"));
	}

	public static string GetRandomSecCorpoSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleSecCorpo"), GetPool("femaleSecCorpo"));
	}

	public static string GetRandomSecCorpoMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleSecCorpo"));
	}

	public static string GetRandomSecCorpoFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleSecCorpo"));
	}

	public static string GetRandomFarmerSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleFarmer"), GetPool("femaleFarmer"));
	}

	public static string GetRandomFarmerMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleFarmer"));
	}

	public static string GetRandomFarmerFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleFarmer"));
	}

	public static string GetRandomChefSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleChef"), GetPool("femaleChef"));
	}

	public static string GetRandomChefMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleChef"));
	}

	public static string GetRandomChefFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleChef"));
	}

	public static string GetRandomMedSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleMed"), GetPool("femaleMed"));
	}

	public static string GetRandomMedMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleMed"));
	}

	public static string GetRandomMedFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleMed"));
	}

	public static string GetRandomEngSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleEng"), GetPool("femaleEng"));
	}

	public static string GetRandomEngMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleEng"));
	}

	public static string GetRandomEngFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleEng"));
	}

	public static string GetRandomMinerSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleScav", 2));
	}

	public static string GetRandomMercMale()
	{
		return ClientSpriteVariety.Pick(GetPool("maleMerc"));
	}

	public static string GetRandomMercFemale()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleMerc"));
	}

	public static string GetRandomMercSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleMerc"), GetPool("femaleMerc"));
	}

	public static string GetRandomSciMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleSci"));
	}

	public static string GetRandomSciFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleSci"));
	}

	public static string GetRandomSciSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleSci"), GetPool("femaleSci"));
	}

	public static string GetRandomSecOffMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("maleSecOff"));
	}

	public static string GetRandomSecOffFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("femaleSecOff"));
	}

	public static string GetRandomSecOffSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("maleSecOff"), GetPool("femaleSecOff"));
	}

	public static string GetRandomAugLowerMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("augLLMale"));
	}

	public static string GetRandomAugLowerFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("augLLFemale"));
	}

	public static string GetRandomAugLowerSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("augLLMale"), GetPool("augLLFemale"));
	}

	public static string GetRandomAugBMMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("augBMMale"));
	}

	public static string GetRandomAugBMFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("augBMFemale"));
	}

	public static string GetRandomAugBMSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("augBMMale"), GetPool("augBMFemale"));
	}

	public static string GetRandomAugUpperMaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("augULMale"));
	}

	public static string GetRandomAugUpperFemaleSpriteName()
	{
		return ClientSpriteVariety.Pick(GetPool("augULFemale"));
	}

	public static string GetRandomAugUpperSpriteName()
	{
		return ClientSpriteVariety.PickGendered(GetPool("augULMale"), GetPool("augULFemale"));
	}

	public static string GetRandomAugSpriteName()
	{
		List<string> list = new List<string>();
		list.AddRange(GetPool("augLLMale"));
		list.AddRange(GetPool("augBMMale"));
		list.AddRange(GetPool("augULMale"));
		List<string> list2 = new List<string>();
		list2.AddRange(GetPool("augLLFemale"));
		list2.AddRange(GetPool("augBMFemale"));
		list2.AddRange(GetPool("augULFemale"));
		return ClientSpriteVariety.PickGendered(list, list2);
	}

	public static string GetLoanSharkSpriteName()
	{
		return GetShadyMerchantSpriteName();
	}

	public static string GetRandomScientistSpriteName()
	{
		return GetRandomSciSpriteName();
	}

	public static string GetRandomOffDutySecSpriteName()
	{
		return GetRandomSecOffSpriteName();
	}

	public static string GetRandomMerchantSpriteName()
	{
		return "merchant";
	}

	public static string GetShadyMerchantSpriteName()
	{
		return "shadyMerchant";
	}

	public static string GetClerkSpriteName()
	{
		return "admin";
	}

	public static string GetGreytideSpriteName()
	{
		return "greytide";
	}

	public static string getRandomPilotSpriteName()
	{
		return GetRandomLowerLevelSpriteName();
	}

	public static string GetRandomElderSpriteName()
	{
		return "scavElder";
	}

	public static string GetFoodieSpriteName()
	{
		return "waffle_enjoyer";
	}

	public static string GetCaseEnjoyerSpriteName()
	{
		return "case_enjoyer";
	}

	public static string GetAshishSpriteName()
	{
		return "maleBM3";
	}

	public static string GetUniqueEleanorNormal()
	{
		return "eleanor1";
	}

	public static string GetUniqueEleanorNoJewel()
	{
		return "eleanor2";
	}

	public static string GetUniqueEleanorSad()
	{
		return "eleanor3";
	}

	public static string GetCourrierSpriteName()
	{
		return "courrier";
	}

	public static string GetInventorSpriteName()
	{
		return "inventor";
	}

	public static string GetRetiredFarmerSpriteName()
	{
		return "retired_farmer";
	}

	public static string GetRetiredChemistSpriteName()
	{
		return "retired_chemist";
	}

	public static string GetRetiredAquafarmerSpriteName()
	{
		return "retired_aquafarmer";
	}

	public static string GetCartelHandlerSpriteName()
	{
		return "cartel_handler";
	}

	public static string GetBlackmailerSpriteName()
	{
		return "blackmailer";
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaHelper
{
	public const string _LIST_LENGHT_STORAGE_INT = "_LIST_LENGHT_STORAGE_INT";

	public const string _LIST_STORAGE = "_LIST_STORAGE";

	public const string _LIST_STORAGE_INT = "_LIST_STORAGE_INT";

	public const string IGNORE_CHILD_VALUE = "IGNORE_CHILD_VALUE";

	public static void SetIgnoreChildValue(GameItem item)
	{
		item.EnableTag("IGNORE_CHILD_VALUE");
	}

	public static void RemoveIgnoreChildValue(GameItem item)
	{
		item.DisableTag("IGNORE_CHILD_VALUE");
	}

	public static string FormatTooltipInt(int value)
	{
		if (value > 0)
		{
			return "+" + value;
		}
		return value.ToString();
	}

	public static void StoreListInTagSystem(List<string> stringList, string targetTag, GameItem item)
	{
		int listLenght = stringList.Count;
		item.ModifyTag(targetTag + "_LIST_LENGHT_STORAGE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(listLenght);
		});
		int num = 0;
		foreach (string elem in stringList)
		{
			item.ModifyTag(targetTag + "_LIST_STORAGE" + num, delegate(TagState state)
			{
				state.Enable().SetString(elem);
			});
			num++;
		}
	}

	public static List<string> RetrieveListInTagSystem(string targetTag, GameItem item)
	{
		List<string> list = new List<string>();
		int @int = item.GetTagReadonly(targetTag + "_LIST_LENGHT_STORAGE_INT").GetInt();
		for (int i = 0; i < @int; i++)
		{
			list.Add(item.GetTagReadonly(targetTag + "_LIST_STORAGE" + i).GetString());
		}
		return list;
	}

	public static void StoreListInWorldTagSystem(List<string> stringList, string targetTag)
	{
		int count = stringList.Count;
		GameMaster.current.worldTags.GetTag(targetTag + "_LIST_LENGHT_STORAGE_INT").Enable().SetInt(count);
		int num = 0;
		foreach (string @string in stringList)
		{
			GameMaster.current.worldTags.GetTag(targetTag + "_LIST_STORAGE" + num).Enable().SetString(@string);
			num++;
		}
	}

	public static List<string> RetrieveListInWorldTagSystem(string targetTag)
	{
		List<string> list = new List<string>();
		int @int = GameMaster.current.worldTags.GetTag(targetTag + "_LIST_LENGHT_STORAGE_INT").GetInt();
		for (int i = 0; i < @int; i++)
		{
			list.Add(GameMaster.current.worldTags.GetTag(targetTag + "_LIST_STORAGE" + i).GetString());
		}
		return list;
	}

	public static void AppendStringInWorldTagSystem(string targetTag, string targetString)
	{
		int @int = GameMaster.current.worldTags.GetTag(targetTag + "_LIST_LENGHT_STORAGE_INT").GetInt();
		GameMaster.current.worldTags.GetTag(targetTag + "_LIST_STORAGE" + @int).Enable().SetString(targetString);
		GameMaster.current.worldTags.GetTag(targetTag + "_LIST_LENGHT_STORAGE_INT").Enable().SetInt(@int + 1);
	}

	public static (List<string>, List<int>) RetrieveListInTagSystemStringInt(string targetTag, GameItem item)
	{
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		int @int = item.GetTagReadonly(targetTag + "_LIST_LENGHT_STORAGE_INT").GetInt();
		for (int i = 0; i < @int; i++)
		{
			list.Add(item.GetTagReadonly(targetTag + "_LIST_STORAGE" + i).GetString());
			list2.Add(item.GetTagReadonly(targetTag + "_LIST_STORAGE_INT" + i).GetInt());
		}
		return (list, list2);
	}

	public static void StoreListInTagSystemStringInt(List<string> stringList, List<int> intList, string targetTag, GameItem item)
	{
		int listLenght = stringList.Count;
		item.ModifyTag(targetTag + "_LIST_LENGHT_STORAGE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(listLenght);
		});
		int i;
		for (i = 0; i < stringList.Count; i++)
		{
			item.ModifyTag(targetTag + "_LIST_STORAGE" + i, delegate(TagState state)
			{
				state.Enable().SetString(stringList[i]);
			});
			item.ModifyTag(targetTag + "_LIST_STORAGE_INT" + i, delegate(TagState state)
			{
				state.Enable().SetInt(intList[i]);
			});
		}
	}

	public static void WipeListInTagSystemStringInt(string targetTag, GameItem item)
	{
		int @int = item.GetTagReadonly(targetTag + "_LIST_LENGHT_STORAGE_INT").GetInt();
		for (int i = 0; i < @int; i++)
		{
			item.ModifyTag(targetTag + "_LIST_STORAGE" + i, delegate(TagState state)
			{
				state.SetString("");
			});
			item.ModifyTag(targetTag + "_LIST_STORAGE_INT" + i, delegate(TagState state)
			{
				state.SetInt(0);
			});
			item.ModifyTag(targetTag + "_LIST_STORAGE" + i, delegate(TagState state)
			{
				state.Disable();
			});
			item.ModifyTag(targetTag + "_LIST_STORAGE_INT" + i, delegate(TagState state)
			{
				state.Disable();
			});
		}
		item.ModifyTag(targetTag + "_LIST_LENGHT_STORAGE_INT", delegate(TagState state)
		{
			state.SetInt(0);
		});
		item.ModifyTag(targetTag + "_LIST_LENGHT_STORAGE_INT", delegate(TagState state)
		{
			state.Disable();
		});
	}

	public static List<GameCharacterItem> GetCurrentPartyGCI()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		if (GameMaster.current.InRaid())
		{
			list.AddRange(GameMaster.current.raidManager.playerParty.GetCharacterItems());
		}
		else
		{
			list.AddRange(GraphUtils.FindAllChildrenType<GameCharacterItem>(GameMaster.current.shipManager.ship));
		}
		return list;
	}

	public static IEnumerator WaitAndDo(float duration, Action action)
	{
		yield return new WaitForSeconds(duration);
		action();
	}

	public static Color HexToColor(string hex, float alpha = 1f)
	{
		if (!hex.StartsWith("#"))
		{
			hex = "#" + hex;
		}
		if (ColorUtility.TryParseHtmlString(hex, out var color))
		{
			color.a = Mathf.Clamp01(alpha);
			return color;
		}
		Debug.LogError("Invalid hex color string: " + hex);
		return Color.white;
	}

	public static string ColorToHex(Color color)
	{
		Color32 color2 = color;
		return $"{color2.r:X2}{color2.g:X2}{color2.b:X2}";
	}

	public static Color CalculateTint(Color original, Color gray)
	{
		float value = ((gray.r > 0f) ? (original.r / gray.r) : 1f);
		float value2 = ((gray.g > 0f) ? (original.g / gray.g) : 1f);
		float value3 = ((gray.b > 0f) ? (original.b / gray.b) : 1f);
		float r = Mathf.Clamp01(value);
		value2 = Mathf.Clamp01(value2);
		value3 = Mathf.Clamp01(value3);
		return new Color(r, value2, value3, 1f);
	}

	public static string FormatTemperature(int temperature)
	{
		return temperature + "°C";
	}
}

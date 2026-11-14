using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace SodaCraft.Localizations;

public static class CSVUtilities
{
	public static void SaveAsCSV(List<List<string>> table, string path)
	{
		string value = table.ToCSVString();
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		using FileStream stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
		using StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);
		streamWriter.Write(value);
	}

	public static List<List<string>> ReadCSV(string csvContent)
	{
		List<List<string>> list = new List<List<string>>();
		csvContent = csvContent.Replace("\r", "\n");
		string[] array = csvContent.Split('\n');
		List<char> tokenBuffer = new List<char>();
		bool flag = false;
		bool flag2 = false;
		string[] array2 = array;
		foreach (string text in array2)
		{
			List<string> list2 = new List<string>();
			tokenBuffer.Clear();
			foreach (char c in text)
			{
				if (flag2)
				{
					flag2 = false;
					if (c == 'n')
					{
						tokenBuffer.Add('\n');
					}
					else
					{
						tokenBuffer.Add(c);
					}
					continue;
				}
				if (flag)
				{
					switch (c)
					{
					case '\\':
						flag2 = true;
						break;
					case '"':
						flag = false;
						break;
					default:
						tokenBuffer.Add(c);
						break;
					}
					continue;
				}
				switch (c)
				{
				case '\\':
					flag2 = true;
					break;
				case '"':
					if (tokenBuffer.Count == 0)
					{
						flag = true;
					}
					break;
				case ',':
				{
					string text2 = FlushBuffer();
					text2 = text2.Trim();
					list2.Add(text2);
					break;
				}
				default:
					if (!char.IsWhiteSpace(c) || tokenBuffer.Count != 0)
					{
						tokenBuffer.Add(c);
					}
					break;
				}
			}
			if (tokenBuffer.Count > 0)
			{
				string text3 = FlushBuffer();
				text3 = text3.Trim();
				list2.Add(text3);
			}
			if (list2.Count > 0)
			{
				list.Add(list2);
			}
		}
		return list;
		string FlushBuffer()
		{
			string result = new string(tokenBuffer.ToArray());
			tokenBuffer.Clear();
			return result;
		}
	}

	public static string ToCSVString(this List<List<string>> table)
	{
		char value = ',';
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < table.Count; i++)
		{
			List<string> list = table[i];
			if (list.Count < 1)
			{
				continue;
			}
			for (int j = 0; j < list.Count; j++)
			{
				string text = list[j];
				if (text.Contains(','))
				{
					text = "\"" + text + "\"";
				}
				if (j > 0)
				{
					stringBuilder.Append(value);
				}
				if (text.Contains('\n'))
				{
					Debug.Log("Replacing \\n");
					text.Replace("\n", "\\n");
				}
				stringBuilder.Append(text);
			}
			stringBuilder.Append("\n");
		}
		return stringBuilder.ToString();
	}
}

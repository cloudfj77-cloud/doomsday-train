using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RichTextBuilder
{
	private class TextSlice
	{
		public string text = "";

		public bool bold;

		public bool italic;

		public bool underline;

		public bool strikethrough;

		public char underlineChar = '_';

		public char strikethroughChar = '─';

		public RenderHandler.ColorPalette color = RenderHandler.ColorPalette.None;

		public RenderHandler.ColorPalette backgroundColor = RenderHandler.ColorPalette.None;

		public RenderHandler.ColorPalette underlineColor = RenderHandler.ColorPalette.None;

		public RenderHandler.ColorPalette strikethroughColor = RenderHandler.ColorPalette.None;

		public bool isNewLine;

		public bool noBreak;

		private int length = -1;

		public TextSlice(bool newLine = false)
		{
			isNewLine = newLine;
		}

		public int GetLengthOld()
		{
			if (length < 0)
			{
				length = ((!isNewLine) ? text.Count((char c) => !char.IsControl(c)) : 0);
			}
			return length;
		}

		public int GetLength()
		{
			if (length < 0)
			{
				if (isNewLine)
				{
					length = 0;
				}
				else if (this.text != null)
				{
					bool flag = false;
					length = 0;
					string text = this.text;
					foreach (char c in text)
					{
						switch (c)
						{
						case '<':
							flag = true;
							continue;
						case '>':
							flag = false;
							continue;
						}
						if (!flag && !char.IsControl(c))
						{
							length++;
						}
					}
				}
				else
				{
					length = 0;
				}
			}
			return length;
		}

		public TextSlice Copy(string text = null)
		{
			if (text == null)
			{
				text = this.text;
			}
			return new TextSlice
			{
				text = text,
				bold = bold,
				italic = italic,
				underline = underline,
				strikethrough = strikethrough,
				underlineChar = underlineChar,
				strikethroughChar = strikethroughChar,
				color = color,
				backgroundColor = backgroundColor,
				underlineColor = underlineColor,
				strikethroughColor = strikethroughColor,
				isNewLine = isNewLine,
				noBreak = noBreak,
				length = -1
			};
		}

		public List<int> GetReversedIndices(char c)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < text.Length; i++)
			{
				int num = text.Length - i - 1;
				if (text[num] == c)
				{
					list.Add(num);
				}
			}
			return list;
		}

		public int GetFirstIndex(char c)
		{
			return text.IndexOf(c);
		}

		public (string, string, string, string, string) GetString(int truncate = -1)
		{
			string text = this.text;
			if (isNewLine)
			{
				return ("\n", "\n", "\n", "\n", "\n");
			}
			if (GetLength() <= 0)
			{
				return (text, text, text, text, text);
			}
			if (truncate > -1)
			{
				text = text[..Math.Min(text.Length, truncate)];
			}
			string fillString = GetFillString(text, ' ');
			string fillString2 = GetFillString(text, '█');
			if (underlineColor == RenderHandler.ColorPalette.None)
			{
				underlineColor = color;
			}
			if (strikethroughColor == RenderHandler.ColorPalette.None)
			{
				strikethroughColor = color;
			}
			if (underline)
			{
				text = "<u>" + text + "</u>";
			}
			string item = "";
			if (strikethrough)
			{
				text = "<s>" + text + "</s>";
			}
			string item2 = "";
			string item3 = ((backgroundColor == RenderHandler.ColorPalette.None) ? fillString : Color(fillString2, backgroundColor));
			text = Style(text, bold, italic);
			text = Color(text, color);
			string item4 = text;
			return (text, item2, item, item4, item3);
		}
	}

	private List<TextSlice> slices;

	private string nextBegin;

	public int maxCharWidth;

	public readonly char[] defaultStyle = new char[9] { '\u2000', '▏', '▎', '▍', '▌', '▋', '▊', '▉', '█' };

	public readonly char[] defaultStyleVertical = "⠀▁▂▃▄▅▆▇█".ToCharArray();

	public readonly char[] defaultStyleOldSchool = ".\\|".ToCharArray();

	public RichTextBuilder(int maxCharWidth = 100)
	{
		nextBegin = "";
		this.maxCharWidth = maxCharWidth;
		slices = new List<TextSlice>();
	}

	public bool IsEmpty()
	{
		return slices.Count == 0;
	}

	public static string GetFillString(string text, char replace)
	{
		return new string((from c in text.ToCharArray()
			select (!char.IsControl(c)) ? replace : c).ToArray());
	}

	public RichTextBuilder Add(RichTextBuilder builder)
	{
		slices.AddRange(builder.slices);
		nextBegin = builder.nextBegin;
		return this;
	}

	public RichTextBuilder AddProgressBarOld(float ratio, int length, RenderHandler.ColorPalette color = RenderHandler.ColorPalette.None, RenderHandler.ColorPalette backgroundColor = RenderHandler.ColorPalette.None, char[] style = null, string end = "", bool addFullLength = true, bool bold = false)
	{
		if (!float.IsFinite(ratio))
		{
			ratio = 0f;
		}
		ratio = Math.Clamp(ratio, 0f, 1f);
		if (style == null)
		{
			style = defaultStyle;
		}
		int num = style.Length;
		int num2 = (int)(ratio * (float)length);
		int value = (int)((ratio * (float)length - (float)num2) * (float)(num - 1));
		value = Math.Clamp(value, 0, num - 1);
		string text = ((num2 > 0) ? new string(style[num - 1], num2) : "");
		if (text.Length < length)
		{
			text += style[value];
		}
		if (text.Length < length && addFullLength)
		{
			text += new string(style[0], length - text.Length);
		}
		Add(text, color, end, bold, italic: false, underline: false, strikethrough: false, backgroundColor);
		return MarkLastSliceNoBreak();
	}

	public RichTextBuilder AddProgressTest(float ratio, int length, RenderHandler.ColorPalette color = RenderHandler.ColorPalette.None, RenderHandler.ColorPalette backgroundColor = RenderHandler.ColorPalette.None, char[] style = null, string end = "", bool addFullLength = true, bool bold = false)
	{
		if (!float.IsFinite(ratio))
		{
			ratio = 0f;
		}
		ratio = Math.Clamp(ratio, 0f, 1f);
		string[] array = new string[5] { "<sprite=1 tint=1>", "<sprite=2 tint=1>", "<sprite=3 tint=1>", "<sprite=4 tint=1>", "<sprite name=\"bar16\" tint=1>" };
		int[] array2 = new int[5] { 1, 2, 4, 8, 16 };
		int num = (int)(ratio * (float)length);
		int i = 0;
		string text = "";
		for (int num2 = array.Length - 1; num2 >= 0; num2--)
		{
			for (; i + array2[num2] <= num; i += array2[num2])
			{
				text += array[num2];
			}
		}
		if (i < num)
		{
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j] <= num - i)
				{
					text += array[j];
					i += array2[j];
					break;
				}
			}
		}
		if (addFullLength && i < length)
		{
			int num3 = length - i;
			while (num3 > 0)
			{
				int num4 = 0;
				text += array[num4];
				num3 -= array2[num4];
			}
		}
		Add(text, color, end, bold, italic: false, underline: false, strikethrough: false, backgroundColor);
		return MarkLastSliceNoBreak();
	}

	public RichTextBuilder AddProgressBar(float ratio, int length, RenderHandler.ColorPalette color = RenderHandler.ColorPalette.None, RenderHandler.ColorPalette backgroundColor = RenderHandler.ColorPalette.None, char[] style = null, string end = "", bool addFullLength = true, bool bold = false)
	{
		if (!float.IsFinite(ratio))
		{
			ratio = 0f;
		}
		ratio = Math.Clamp(ratio, 0f, 1f);
		int num = (int)(ratio * (float)length);
		int num2 = (addFullLength ? (length - num) : 0);
		StringBuilder stringBuilder = new StringBuilder(length + 2);
		stringBuilder.Append('[');
		if (num > 0)
		{
			stringBuilder.Append('=', num);
		}
		if (num2 > 0)
		{
			stringBuilder.Append(' ', num2);
		}
		stringBuilder.Append(']');
		backgroundColor = RenderHandler.ColorPalette.None;
		string text = stringBuilder.ToString();
		RenderHandler.ColorPalette backgroundColor2 = backgroundColor;
		Add(text, color, end, bold, italic: false, underline: false, strikethrough: false, backgroundColor2);
		return MarkLastSliceNoBreak();
	}

	private RichTextBuilder MarkLastSliceNoBreak()
	{
		for (int num = slices.Count - 1; num >= 0; num--)
		{
			if (!slices[num].isNewLine)
			{
				slices[num].noBreak = true;
				break;
			}
		}
		return this;
	}

	public RichTextBuilder AddLine(string text = null, bool ignoreEmpty = true, RenderHandler.ColorPalette color = RenderHandler.ColorPalette.None, bool bold = false, bool italic = false, bool underline = false, bool strikethrough = false, RenderHandler.ColorPalette backgroundColor = RenderHandler.ColorPalette.None, RenderHandler.ColorPalette underlineColor = RenderHandler.ColorPalette.None, RenderHandler.ColorPalette strikethroughColor = RenderHandler.ColorPalette.None)
	{
		if (text == null)
		{
			if (nextBegin == "\n")
			{
				ignoreEmpty = false;
			}
			text = "";
		}
		if (ignoreEmpty && text.Length <= 0)
		{
			nextBegin = "\n";
			return this;
		}
		return Add(text, color, "\n", bold, italic, underline, strikethrough, backgroundColor, underlineColor, strikethroughColor);
	}

	public RichTextBuilder NextLine()
	{
		nextBegin = "";
		slices.Add(new TextSlice(newLine: true));
		return this;
	}

	public RichTextBuilder AddEmptyLine(int heightPixels)
	{
		NextLine();
		Add($"<size={heightPixels}px> </size>");
		return this;
	}

	public RichTextBuilder Add(string text, RenderHandler.ColorPalette color = RenderHandler.ColorPalette.None, string end = "", bool bold = false, bool italic = false, bool underline = false, bool strikethrough = false, RenderHandler.ColorPalette backgroundColor = RenderHandler.ColorPalette.None, RenderHandler.ColorPalette underlineColor = RenderHandler.ColorPalette.None, RenderHandler.ColorPalette strikethroughColor = RenderHandler.ColorPalette.None)
	{
		if (text == null)
		{
			text = "";
		}
		TextSlice textSlice = new TextSlice();
		textSlice.isNewLine = true;
		TextSlice textSlice2 = new TextSlice();
		textSlice2.text = nextBegin;
		string[] array = nextBegin.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Length > 0)
			{
				slices.Add(textSlice2.Copy(array[i]));
			}
			if (i < array.Length - 1)
			{
				slices.Add(textSlice.Copy());
			}
		}
		bold &= RenderHandler.current.windowUICanvas.scaleFactor > 1f;
		TextSlice textSlice3 = new TextSlice();
		textSlice3.text = text;
		textSlice3.bold = bold;
		textSlice3.italic = italic;
		textSlice3.underline = underline;
		textSlice3.strikethrough = strikethrough;
		textSlice3.color = color;
		textSlice3.backgroundColor = backgroundColor;
		textSlice3.underlineColor = underlineColor;
		textSlice3.strikethroughColor = strikethroughColor;
		string[] array2 = text.Split('\n');
		for (int j = 0; j < array2.Length; j++)
		{
			if (array2[j].Length > 0)
			{
				slices.Add(textSlice3.Copy(array2[j]));
			}
			if (j < array2.Length - 1)
			{
				slices.Add(textSlice.Copy());
			}
		}
		nextBegin = end ?? "";
		return this;
	}

	public RichTextBuilder AddIcon(Sprite icon, string end = "")
	{
		return this;
	}

	public RichTextBuilder Clear()
	{
		slices.Clear();
		nextBegin = "";
		return this;
	}

	public RichTextBuilder AddRichTable(RichTableBuilder table)
	{
		return this;
	}

	private static (int, int) FindBestSlice(List<TextSlice> slices, int beginIndex, int endIndex, int maxLength, int currentLength)
	{
		for (int num = endIndex; num >= beginIndex; num--)
		{
			if (num >= 0 && num < slices.Count)
			{
				if (slices[num].noBreak)
				{
					currentLength -= slices[num].GetLength();
				}
				else
				{
					foreach (int reversedIndex in slices[num].GetReversedIndices(' '))
					{
						if (currentLength - slices[num].GetLength() + reversedIndex <= maxLength)
						{
							return (num, reversedIndex);
						}
					}
					currentLength -= slices[num].GetLength();
				}
			}
		}
		return (-1, -1);
	}

	private static (int, int) FindCharBreak(List<TextSlice> slices, int beginIndex, int endIndex, int maxLength)
	{
		int num = 0;
		for (int i = beginIndex; i <= endIndex; i++)
		{
			if (i < 0 || i >= slices.Count || slices[i].isNewLine)
			{
				continue;
			}
			int length = slices[i].GetLength();
			if (slices[i].noBreak)
			{
				if (num + length > maxLength && num > 0)
				{
					return (i, 0);
				}
				num += length;
				continue;
			}
			if (num + length > maxLength)
			{
				int val = Math.Max(1, maxLength - num);
				val = Math.Min(val, slices[i].text.Length);
				return (i, val);
			}
			num += length;
		}
		return (-1, -1);
	}

	private static (int, int) FindFirstSlice(List<TextSlice> slices, int beginIndex)
	{
		for (int i = beginIndex; i < slices.Count; i++)
		{
			if (i < 0 || i >= slices.Count)
			{
				continue;
			}
			if (slices[i].isNewLine)
			{
				return (i, -1);
			}
			if (!slices[i].noBreak)
			{
				int firstIndex = slices[i].GetFirstIndex(' ');
				if (firstIndex >= 0)
				{
					return (i, firstIndex);
				}
			}
		}
		return (-1, -1);
	}

	public RichText Build(int truncateLength = -1)
	{
		List<TextSlice> list = new List<TextSlice>(slices);
		float num = ((UISettings.current != null) ? UISettings.current.GetCharWidthScale() : 1f);
		float num2 = (list.Any((TextSlice s) => s.noBreak) ? 0.875f : 0.66f);
		int num3 = ((num > 1f) ? Math.Max(1, (int)((float)maxCharWidth * num2)) : maxCharWidth);
		int beginIndex = 0;
		int num4 = 0;
		TextSlice textSlice = new TextSlice();
		textSlice.isNewLine = true;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].isNewLine)
			{
				beginIndex = i + 1;
				num4 = 0;
				continue;
			}
			num4 += list[i].GetLength();
			if (num4 <= num3)
			{
				continue;
			}
			int num5;
			int num6;
			(num5, num6) = FindBestSlice(list, beginIndex, i, num3, num4);
			if (num5 < 0)
			{
				(num5, num6) = FindCharBreak(list, beginIndex, i, num3);
				if (num5 >= 0 && num6 <= 0)
				{
					list.Insert(num5, textSlice.Copy());
					i = num5;
					beginIndex = num5 + 1;
					num4 = 0;
					continue;
				}
				if (num5 >= 0)
				{
					string text = list[num5].text.Substring(0, num6);
					string text2 = list[num5].text.Substring(num6);
					TextSlice textSlice2 = list[num5];
					list.RemoveAt(num5);
					list.Insert(num5, textSlice2.Copy(text2));
					list.Insert(num5, textSlice.Copy());
					list.Insert(num5, textSlice2.Copy(text));
					i = num5;
					beginIndex = num5 + 2;
					num4 = 0;
					continue;
				}
				(num5, num6) = FindFirstSlice(list, i + 1);
				if (num5 < 0)
				{
					break;
				}
				if (num6 < 0)
				{
					i = num5;
					beginIndex = i + 1;
					num4 = 0;
					continue;
				}
				string text3 = list[num5].text.Substring(0, num6);
				string text4 = list[num5].text.Substring(num6 + 1);
				TextSlice textSlice3 = list[num5];
				list.RemoveAt(num5);
				list.Insert(num5, textSlice3.Copy(text4));
				list.Insert(num5, textSlice.Copy());
				list.Insert(num5, textSlice3.Copy(text3));
				i = num5;
			}
			else
			{
				string text5 = list[num5].text.Substring(0, num6);
				string text6 = list[num5].text.Substring(num6 + 1);
				TextSlice textSlice4 = list[num5];
				list.RemoveAt(num5);
				list.Insert(num5, textSlice4.Copy(text6));
				list.Insert(num5, textSlice.Copy());
				list.Insert(num5, textSlice4.Copy(text5));
				i = num5;
			}
		}
		string text7 = "";
		string text8 = "";
		string text9 = "";
		string text10 = "";
		string text11 = "";
		int num7 = 0;
		bool isTruncated = false;
		foreach (TextSlice item6 in list)
		{
			num7 += item6.text.Length;
			int num8 = -1;
			if (truncateLength > -1 && num7 > truncateLength)
			{
				num8 = item6.text.Length - (num7 - truncateLength);
			}
			(string, string, string, string, string) @string = item6.GetString(num8);
			string item = @string.Item1;
			string item2 = @string.Item2;
			string item3 = @string.Item3;
			string item4 = @string.Item4;
			string item5 = @string.Item5;
			text7 += item;
			text8 += item2;
			text9 += item3;
			text10 += item4;
			text11 += item5;
			if (num8 > -1)
			{
				isTruncated = true;
				break;
			}
		}
		int recommendedWidth = text7.Split("\n").Max((string s) => s.Count((char c) => !char.IsControl(c)));
		int recommendedHeight = list.Count((TextSlice s) => s.isNewLine) + 1;
		return new RichText(text7, text8, text9, text10, text11, recommendedWidth, recommendedHeight, isTruncated);
	}

	public static string Color(string text, RenderHandler.ColorPalette color)
	{
		return RenderHandler.ApplyRichTextColorTag(text, color);
	}

	public static string Style(string text, bool bold, bool italic = false)
	{
		if (bold)
		{
			text = "<b>" + text + "</b>";
		}
		if (italic)
		{
			text = "<i>" + text + "</i>";
		}
		return text;
	}
}

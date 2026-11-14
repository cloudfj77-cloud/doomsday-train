using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;

namespace FlexFramework.Excel;

public sealed class WorkBook : KeyedCollection<string, WorkSheet>
{
	public SharedStringCollection SharedStrings { get; private set; }

	public WorkBook(byte[] buffer)
	{
		using ZipFile zip = new ZipFile(new MemoryStream(buffer));
		Read(zip);
	}

	public WorkBook(Stream stream)
	{
		ZipFile zip = new ZipFile(stream);
		Read(zip);
	}

	public WorkBook(string fileName)
	{
		using ZipFile zip = new ZipFile(fileName);
		Read(zip);
	}

	private void Read(ZipFile zip)
	{
		List<string> list = new List<string>();
		ZipEntry entry = zip.GetEntry("xl/sharedStrings.xml");
		if (entry != null)
		{
			using XmlReader xmlReader = XmlReader.Create(zip.GetInputStream(entry));
			if (xmlReader.ReadToFollowing("sst"))
			{
				while (xmlReader.ReadToFollowing("si"))
				{
					using XmlReader xmlReader2 = xmlReader.ReadSubtree();
					StringBuilder stringBuilder = new StringBuilder();
					while (!xmlReader2.EOF)
					{
						if (xmlReader2.NodeType != XmlNodeType.Element)
						{
							xmlReader2.Read();
							continue;
						}
						switch (xmlReader2.Name)
						{
						case "t":
							stringBuilder.Append(xmlReader2.ReadElementContentAsString());
							break;
						case "r":
						case "si":
							xmlReader2.Read();
							break;
						default:
							xmlReader2.Skip();
							break;
						}
					}
					list.Add(stringBuilder.ToString());
				}
			}
		}
		using (XmlReader xmlReader3 = XmlReader.Create(zip.GetInputStream(zip.GetEntry("xl/workbook.xml"))))
		{
			if (xmlReader3.ReadToFollowing("sheets") && xmlReader3.ReadToDescendant("sheet"))
			{
				int num = 1;
				do
				{
					string attribute = xmlReader3.GetAttribute("name");
					string attribute2 = xmlReader3.GetAttribute("sheetId");
					using XmlReader xmlReader4 = XmlReader.Create(zip.GetInputStream(zip.GetEntry($"xl/worksheets/sheet{num++}.xml")));
					List<Row> list2 = new List<Row>();
					List<Range> list3 = new List<Range>();
					if (!xmlReader4.ReadToFollowing("sheetData"))
					{
						continue;
					}
					if (xmlReader4.ReadToDescendant("row"))
					{
						do
						{
							if (!xmlReader4.ReadToDescendant("c"))
							{
								continue;
							}
							List<Cell> list4 = new List<Cell>();
							do
							{
								string attribute3 = xmlReader4.GetAttribute("r");
								string attribute4 = xmlReader4.GetAttribute("t");
								Cell cell;
								if (xmlReader4.ReadToDescendant("v"))
								{
									switch (attribute4)
									{
									case "s":
									{
										int index = xmlReader4.ReadElementContentAsInt();
										cell = new Cell(list[index], new Address(attribute3));
										break;
									}
									case "b":
										cell = new Cell(xmlReader4.ReadElementContentAsBoolean(), new Address(attribute3));
										break;
									case "str":
									case "inlineStr":
									{
										string value = xmlReader4.ReadElementContentAsString();
										cell = new Cell(value, new Address(attribute3));
										break;
									}
									default:
									{
										string text = xmlReader4.ReadElementContentAsString();
										if (Regex.IsMatch(text, "^[-+]?\\d*(\\.\\d+)?$"))
										{
											if (Regex.IsMatch(text, "^[-+]?\\d+$"))
											{
												if (Regex.Matches(text, "\\d").Count <= 11)
												{
													cell = new Cell(int.Parse(text), new Address(attribute3));
												}
												else
												{
													cell = new Cell(long.Parse(text), new Address(attribute3));
												}
											}
											else if (Regex.Matches(text, "\\d").Count <= 7)
											{
												cell = new Cell(float.Parse(text), new Address(attribute3));
											}
											else
											{
												cell = new Cell(double.Parse(text), new Address(attribute3));
											}
										}
										else
										{
											cell = new Cell(text, new Address(attribute3));
										}
										break;
									}
									}
								}
								else
								{
									cell = new Cell(new Address(attribute3))
									{
										IsSpan = true
									};
								}
								Cell last = list4.LastOrDefault();
								if (last != null)
								{
									int num2 = cell.Address.Column - last.Address.Column - 1;
									if (num2 > 0)
									{
										list4.AddRange(from i in Enumerable.Range(1, num2)
											select new Cell(new Address(last.Address.Column + i, last.Address.Row)));
									}
								}
								else if (cell.Address.Column > 0)
								{
									int count = cell.Address.Column - 1;
									list4.AddRange(from i in Enumerable.Range(0, count)
										select new Cell(new Address(i, cell.Address.Row)));
								}
								list4.Add(cell);
							}
							while (xmlReader4.ReadToNextSibling("c"));
							list2.Add(new Row(list4));
						}
						while (xmlReader4.ReadToNextSibling("row"));
					}
					if (xmlReader4.ReadToFollowing("mergeCells") && xmlReader4.ReadToDescendant("mergeCell"))
					{
						do
						{
							string attribute5 = xmlReader4.GetAttribute("ref");
							list3.Add(new Range(attribute5));
						}
						while (xmlReader4.ReadToNextSibling("mergeCell"));
					}
					WorkSheet item = new WorkSheet(attribute2, attribute, list2, list3);
					Add(item);
				}
				while (xmlReader3.ReadToNextSibling("sheet"));
			}
		}
		SharedStrings = new SharedStringCollection(list);
	}

	protected override string GetKeyForItem(WorkSheet item)
	{
		return item.Name;
	}

	public void Merge()
	{
		using IEnumerator<WorkSheet> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Merge();
		}
	}
}

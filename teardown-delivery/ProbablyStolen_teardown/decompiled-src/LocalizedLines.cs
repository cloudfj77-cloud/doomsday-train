using System.Collections;
using System.Collections.Generic;

public sealed class LocalizedLines : IReadOnlyList<string>, IEnumerable<string>, IEnumerable, IReadOnlyCollection<string>
{
	private readonly string table;

	private readonly string[] keys;

	public int Count => keys.Length;

	public int Length => keys.Length;

	public string this[int index] => LocHelper.GetLocalized(table, keys[index]);

	public LocalizedLines(string table, params string[] keys)
	{
		this.table = table;
		this.keys = keys ?? new string[0];
	}

	public static LocalizedLines Dialogue(params string[] keys)
	{
		return new LocalizedLines("Dialogue", keys);
	}

	public static LocalizedLines Mechanic(params string[] keys)
	{
		return new LocalizedLines("Mechanic", keys);
	}

	public static LocalizedLines UI(params string[] keys)
	{
		return new LocalizedLines("UI", keys);
	}

	public string KeyAt(int index)
	{
		return keys[index];
	}

	public string Random()
	{
		return this[RNG.GetRandomIntExculsive(0, keys.Length)];
	}

	public IEnumerator<string> GetEnumerator()
	{
		for (int i = 0; i < keys.Length; i++)
		{
			yield return this[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

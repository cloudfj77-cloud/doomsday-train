using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public abstract class ConfigEntry
{
	public string Key { get; internal set; }

	public string Section { get; internal set; }

	public Func<string> Label { get; internal set; }

	public Func<string> Tooltip { get; internal set; }

	public bool RequiresRestart { get; internal set; }

	public ModConfig Config { get; internal set; }

	public bool HasRange { get; private set; }

	public float Min { get; private set; }

	public float Max { get; private set; }

	public string[] Choices { get; internal set; }

	public abstract Type ValueType { get; }

	public abstract object BoxedValue { get; }

	public abstract object BoxedDefault { get; }

	internal void SetRange(float min, float max)
	{
		HasRange = true;
		Min = Mathf.Min(min, max);
		Max = Mathf.Max(min, max);
	}

	public string Serialize()
	{
		return Format(BoxedValue);
	}

	internal abstract bool TryDeserialize(string text);

	internal abstract bool ApplyBoxed(object value);

	public static string Format(object value)
	{
		if (value != null)
		{
			if (!(value is bool))
			{
				if (!(value is int num))
				{
					if (!(value is float num2))
					{
						if (value is Enum @enum)
						{
							return @enum.ToString();
						}
						return value.ToString();
					}
					return num2.ToString("R", CultureInfo.InvariantCulture);
				}
				return num.ToString(CultureInfo.InvariantCulture);
			}
			if (!(bool)value)
			{
				return "false";
			}
			return "true";
		}
		return "";
	}
}
public class ConfigEntry<T> : ConfigEntry
{
	private T value;

	public T Value => value;

	public T Default { get; }

	public override Type ValueType => typeof(T);

	public override object BoxedValue => value;

	public override object BoxedDefault => Default;

	public event Action<T> OnChanged;

	internal ConfigEntry(T defaultValue)
	{
		Default = defaultValue;
		value = defaultValue;
	}

	public void Set(T newValue)
	{
		if (ApplyValue(newValue) && base.Config != null)
		{
			base.Config.Save();
		}
	}

	public void Reset()
	{
		Set(Default);
	}

	internal override bool ApplyBoxed(object boxed)
	{
		if (boxed is T newValue)
		{
			return ApplyValue(newValue);
		}
		try
		{
			return ApplyValue((T)Convert.ChangeType(boxed, typeof(T), CultureInfo.InvariantCulture));
		}
		catch (Exception)
		{
			Debug.LogWarning($"[ModConfig] '{base.Key}': cannot apply {boxed} as {typeof(T).Name}");
			return false;
		}
	}

	internal bool ApplyValue(T newValue)
	{
		newValue = Sanitize(newValue);
		if (EqualityComparer<T>.Default.Equals(value, newValue))
		{
			return false;
		}
		value = newValue;
		try
		{
			this.OnChanged?.Invoke(value);
		}
		catch (Exception arg)
		{
			Debug.LogError($"[ModConfig] OnChanged of '{base.Key}' threw: {arg}");
		}
		return true;
	}

	internal override bool TryDeserialize(string text)
	{
		Type typeFromHandle = typeof(T);
		object obj;
		if (typeFromHandle == typeof(bool))
		{
			if (!bool.TryParse(text, out var result))
			{
				return false;
			}
			obj = result;
		}
		else if (typeFromHandle == typeof(int))
		{
			if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result2))
			{
				return false;
			}
			obj = result2;
		}
		else if (typeFromHandle == typeof(float))
		{
			if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result3))
			{
				return false;
			}
			obj = result3;
		}
		else if (typeFromHandle == typeof(string))
		{
			obj = text ?? "";
		}
		else
		{
			if (!typeFromHandle.IsEnum)
			{
				return false;
			}
			try
			{
				obj = Enum.Parse(typeFromHandle, text, ignoreCase: true);
			}
			catch (Exception)
			{
				return false;
			}
		}
		value = Sanitize((T)obj);
		return true;
	}

	private T Sanitize(T candidate)
	{
		if (!(candidate is int num))
		{
			if (!(candidate is float num2))
			{
				if (candidate is string text && base.Choices != null && base.Choices.Length != 0 && Array.IndexOf(base.Choices, text) < 0)
				{
					return Default;
				}
			}
			else if (base.HasRange)
			{
				return (T)(object)Mathf.Clamp(num2, base.Min, base.Max);
			}
		}
		else if (base.HasRange)
		{
			return (T)(object)Mathf.Clamp(num, Mathf.RoundToInt(base.Min), Mathf.RoundToInt(base.Max));
		}
		return candidate;
	}
}
